using PMGSY.Common;
using PMGSY.Controllers;
using PMGSY.Extensions;
using PMGSY.Models;
using PMGSY.Models.QualityMonitoringHelpDesk;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
//using System.Data.SqlClient;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Script.Serialization;

namespace PMGSY.DAL.QualityMonitoringHelpDesk
{
    public class QualityMonitoringHelpDeskDAL : IQualityMonitoringHelpDeskDAL
    {

        PMGSYEntities dbContext = null;
        #region MonitorDetails
        /// <summary>
        /// Monitor Details on selected year, month and QM Type
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="getSelectedMonth"></param>
        /// <param name="getSelectedYear"></param>
        /// <param name="getSelectedQmType"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        /// 
        public Array QMMonitorDetailsDAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int getSelectedMonth, string getSelectedYear, string getSelectedQmType, string filters)
        {
            dbContext = new PMGSYEntities();
            JavaScriptSerializer js = null;
            SearchJsonString test = new SearchJsonString();
            string monitorNameSearch = string.Empty;
            string userNameSearch = string.Empty;
            string stateNameSearch = string.Empty;
            try
            {

                if (filters != null)
                {
                    js = new JavaScriptSerializer();
                    test = js.Deserialize<SearchJsonString>(filters);
                    foreach (SearchRules item in test.rules)
                    {
                        switch (item.field)
                        {
                            case "Monitor_NAME": monitorNameSearch = item.data;
                                break;
                            case "USER_NAME": userNameSearch = item.data;
                                break;
                            case "State_NAME": stateNameSearch = item.data;
                                break;
                            default:
                                break;
                        }
                    }
                }



                List<USP_QM_HELPDESK_GET_MONITOR_DETAILS_Result> monitorList = new List<USP_QM_HELPDESK_GET_MONITOR_DETAILS_Result>();
                monitorList = dbContext.USP_QM_HELPDESK_GET_MONITOR_DETAILS(getSelectedMonth, Convert.ToInt32(getSelectedYear), getSelectedQmType)
                                                                        .Where
                                                                        (x => x.Monitor_NAME.ToLower().Contains(monitorNameSearch.Equals(string.Empty) ? "" : monitorNameSearch.ToLower()) &&
                                                                         x.USER_NAME.ToLower().Contains(userNameSearch.Equals(string.Empty) ? "" : userNameSearch.ToLower()) &&
                                                                         x.MAST_STATE_NAME.ToLower().Contains(stateNameSearch.Equals(string.Empty) ? "" : stateNameSearch.ToLower())
                                                                         ).OrderByDescending(x => x.ADMIN_QM_CODE).ToList<USP_QM_HELPDESK_GET_MONITOR_DETAILS_Result>();

                totalRecords = monitorList.Count();

                if (sidx.Trim() != string.Empty)
                {
                   
                    if (sord.ToString() == "asc")
                    { switch (sidx)
                        {
                            case "Monitor_NAME":
                                monitorList = monitorList.OrderBy(x => x.Monitor_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "USER_NAME":
                                monitorList = monitorList.OrderBy(x => x.USER_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "State_NAME":
                                monitorList = monitorList.OrderBy(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                          
                    }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "Monitor_NAME":
                                monitorList = monitorList.OrderByDescending(x => x.Monitor_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "USER_NAME":
                                monitorList = monitorList.OrderByDescending(x => x.USER_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "State_NAME":
                                monitorList = monitorList.OrderByDescending(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;

                        }
                    }
                }
                else
                {
                    monitorList = monitorList.OrderBy(x => x.USER_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }



                return monitorList.Select(monitorDetails => new
                {
                    id = monitorDetails.ADMIN_QM_CODE,
                    cell = new string[]
                     {
                             monitorDetails.ADMIN_QM_CODE.ToString(),
                             monitorDetails.Monitor_NAME==null?"": monitorDetails.Monitor_NAME.ToString(),
                             monitorDetails.USER_NAME==null?"": monitorDetails.USER_NAME.ToString(),
                             monitorDetails.MAST_STATE_NAME==null?"": monitorDetails.MAST_STATE_NAME.ToString(),                            
                             monitorDetails.ImeiNo.ToString(),
                             "<a id='"+monitorDetails.ADMIN_QM_CODE.ToString()+"' href='#' onclick=logDetails('"+monitorDetails.ADMIN_QM_CODE+"') title='Click here to search Log Details' class='logLinks' >"+"<span class='ui-icon ui-icon-search ui-align-center'></span>"+"</a>",
                             "<a id='"+monitorDetails.ADMIN_QM_CODE.ToString()+"' href='#' onClick=showScheduleDetails('"+monitorDetails.ADMIN_QM_CODE+"') title='Click here to search Schedule Details' class='viewSchedule' >"+"<span class='ui-icon ui-icon-search ui-align-center'></span>"+"</a>",
                             "<a id='"+monitorDetails.ADMIN_QM_CODE.ToString()+"' href='#' title='Click here to search Reset IMEI No.' class='resetIMEI' >"+"<span class='ui-icon ui-icon-plus ui-align-center'></span>"+"</a>",
                     }

                }).ToArray();

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return (null);
            }
            finally
            {
                dbContext.Dispose();
            }

        }

        #endregion


        #region ScheduleDetails
        /// <summary>
        /// Schedule Details on selecting Montior Name
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="getSelectedMonth"></param>
        /// <param name="getSelectedYear"></param>
        /// <param name="getSelectedQmType"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        /// 
        public Array QMScheduleDetailsDAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int rowID, int getSelectedMonth, string getSelectedYear, string getSelectedQmType, string filters)
        {
            dbContext = new PMGSYEntities();
            JavaScriptSerializer js = null;
            SearchJsonString test = new SearchJsonString();
            string stateNameSearch = string.Empty;
            string districtNameSearch = string.Empty;
            string roadNameSearch = string.Empty;
            try
            {

                if (filters != null)
                {
                    js = new JavaScriptSerializer();
                    test = js.Deserialize<SearchJsonString>(filters);
                    foreach (SearchRules item in test.rules)
                    {
                        switch (item.field)
                        {
                            case "STATE_NAME": stateNameSearch = item.data;
                                break;
                            case "DISTRICT_NAME": districtNameSearch = item.data;
                                break;
                            case "ROAD_NAME": roadNameSearch = item.data;
                                break;
                            default:
                                break;
                        }
                    }
                }



                List<USP_QM_HELPDESK_GET_SCHEDULE_DETAILS_Result> scheduleList = new List<USP_QM_HELPDESK_GET_SCHEDULE_DETAILS_Result>();
                scheduleList = dbContext.USP_QM_HELPDESK_GET_SCHEDULE_DETAILS(rowID, Convert.ToInt32(getSelectedYear), getSelectedMonth, getSelectedQmType
                                                                              ).Where(x => x.STATE_NAME.ToLower().Contains(stateNameSearch.Equals(string.Empty) ? "" : stateNameSearch.ToLower()) &&
                                                                                        x.DISTRICT_NAME.ToLower().Contains(districtNameSearch.Equals(string.Empty) ? "" : districtNameSearch.ToLower()) &&
                                                                                        x.ROAD_NAME.ToLower().Contains(roadNameSearch.Equals(string.Empty) ? "" : roadNameSearch.ToLower())
                                                                                        ).Distinct().ToList<USP_QM_HELPDESK_GET_SCHEDULE_DETAILS_Result>();

                totalRecords = scheduleList.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        scheduleList = scheduleList.OrderBy(x => x.STATE_NAME).OrderBy(x => x.DISTRICT_NAME).OrderBy(x => x.ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        scheduleList = scheduleList.OrderByDescending(x => x.STATE_NAME).OrderByDescending(x => x.DISTRICT_NAME).OrderByDescending(x => x.ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    scheduleList = scheduleList.OrderBy(x => x.STATE_NAME).OrderBy(x => x.DISTRICT_NAME).OrderBy(x => x.ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return scheduleList.Select(scheduleDetails => new
                {

                    id = scheduleDetails.ADMIN_SCHEDULE_CODE,
                    cell = new string[]{
                                      scheduleDetails.ADMIN_SCHEDULE_CODE.ToString(),
                                      scheduleDetails.STATE_NAME.ToString(),
                                      scheduleDetails.DISTRICT_NAME.ToString(),
                                      scheduleDetails.ROAD_NAME.ToString(),
                                      
                                      scheduleDetails.DEVICE_FLAG.ToString(),
                                      scheduleDetails.FINALIZE_FLAG.ToString(),
                                      scheduleDetails.INSP_STATUS_FLAG.ToString(),
                                      scheduleDetails.SCHEDULE_ASSIGNED.ToString(),
                                      scheduleDetails.ADMIN_IS_ENQUIRY.ToString(),
                                      scheduleDetails.IS_SCHEDULE_DOWNLOAD.ToString(),
                                      scheduleDetails.TOTAL_IMAGE_COUNT.ToString(),
                                      scheduleDetails.CQC_FORWARD_FLAG.ToString(),
                                      //"<a id='"+sl.ADMIN_SCHEDULE_CODE.ToString()+"' href='#' onClick=imageDetails('"+ sl.ADMIN_SCHEDULE_CODE +"','"+sl.IMS_PR_ROAD_CODE+"') title='Click here to search Image Details' class='imageLinks' >"+"<div style='text-align: center;'><span class='ui-icon ui-icon-search ui-align-center'></span></div>"+"</a>",
                          }

                }).ToArray();

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        #endregion


        #region LogDetails
        /// <summary>
        /// Log Details on desired Admin Code (rowID)
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="rowID"></param>
        /// <returns></returns>
        /// 
        public Array QMLogDetailsDAL(int? page, int? rows, string sidx, string sord, out int totalRecords, int rowID)
        {
            dbContext = new PMGSYEntities();
            try
            {


                List<USP_QM_HELPDESK_GET_LOG_DETAILS_Result> logList = new List<USP_QM_HELPDESK_GET_LOG_DETAILS_Result>();
                logList = dbContext.USP_QM_HELPDESK_GET_LOG_DETAILS(rowID).ToList<USP_QM_HELPDESK_GET_LOG_DETAILS_Result>();

                totalRecords = logList.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString().Trim() == "asc")
                    {
                        logList = logList.OrderBy(x => x.MOBILE_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        logList = logList.OrderByDescending(x => x.MOBILE_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    logList = logList.OrderBy(x => x.MOBILE_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return logList.Select(logDetails => new
                {
                    id = logDetails.LOG_ID,
                    cell = new string[]{
                             logDetails.LOG_ID.ToString(),
                             logDetails.MOBILE_NO.ToString(),
                             logDetails.IMEI_NO.ToString(),
                             logDetails.OS_VERSION.ToString(),
                             logDetails.MODEL_NAME.ToString(),
                             logDetails.NETWORK_PROVIDER.ToString(),
                             logDetails.LOGIN_DATE_TIME.ToString(),
                             logDetails.LOGOUT_DATE_TIME.ToString(),
                             logDetails.APP_VERSION.ToString(),
                             logDetails.LOG_MODE.ToString()
                             
                          }
                }).ToArray();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }

        }
        #endregion


        #region StoreImeiNumber
        /// <summary>
        /// Storing the IMEI number for Log and Updating the new IMEI number
        /// </summary>
        /// <param name="adminQmCode"></param>
        /// <param name="imeiNumber"></param>
        /// <returns></returns>
        /// 
        public Int32 QMStoreImeiNumberDAL(int adminQmCode, string imeiNumber)
        {
            dbContext = new PMGSYEntities();
            try
            {
                return dbContext.USP_QM_HELPDESK_INSERT_IMEI(adminQmCode, imeiNumber);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return 0;
            }
            finally
            {
                dbContext.Dispose();
            }

        }
        #endregion


        #region MonitorName
        /// <summary>
        /// Getting the Monitor Name for confirmation while updating the new IMEI number
        /// </summary>
        /// <param name="adminQmCode"></param>
        /// <returns></returns>
        /// 
        public string QMUserNameDAL(int adminQmCode)
        {
            dbContext = new PMGSYEntities();
            try
            {


                var result = (from AQM in dbContext.ADMIN_QUALITY_MONITORS
                              join UUM in dbContext.UM_User_Master
                              on AQM.ADMIN_USER_ID equals UUM.UserID
                              join QMIO in dbContext.QUALITY_MOB_IMEI_NO
                              on UUM.UserID equals QMIO.UserId
                              where AQM.ADMIN_QM_CODE == adminQmCode
                              select AQM.ADMIN_QM_FNAME + " " + ((AQM.ADMIN_QM_MNAME == null) ? string.Empty : AQM.ADMIN_QM_MNAME) + " " + ((AQM.ADMIN_QM_LNAME == null) ? string.Empty : AQM.ADMIN_QM_LNAME)).FirstOrDefault();

                return result == null ? string.Empty : result;

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        #endregion


        #region IsScheduleDownload
        /// <summary>
        /// Getting the value of IsScheduleDownload Whether 'Y' or 'N'
        /// </summary>
        /// <param name="rowID"></param>
        /// <param name="getSelectedYear"></param>
        /// <param name="getSelectedMonth"></param>
        /// <param name="getSelectedQmType"></param>
        /// <returns></returns>

        public String QMIsScheduleDownloadDAL(int rowID, string getSelectedYear, int getSelectedMonth, string getSelectedQmType)
        {
            dbContext = new PMGSYEntities();

            try
            {
                List<USP_QM_HELPDESK_GET_SCHEDULE_DETAILS_Result> isScheduleDownloadList = new List<USP_QM_HELPDESK_GET_SCHEDULE_DETAILS_Result>();
                isScheduleDownloadList = dbContext.USP_QM_HELPDESK_GET_SCHEDULE_DETAILS(Convert.ToInt32(rowID), Convert.ToInt32(getSelectedYear), getSelectedMonth, getSelectedQmType).Distinct().ToList();
                var result = (from sl in isScheduleDownloadList select sl.IS_SCHEDULE_DOWNLOAD).FirstOrDefault();
                return (result == null ? string.Empty : result);
            }
            catch (Exception ex)
            {

                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }

        }

        #endregion


        #region DefinalizeSchedule
        /// <summary>
        /// Definalizing the Schedule Details (updating some parameters in ScheduleDetails and Schedule if IsScheduleDownload is 'N')
        /// </summary>
        /// 
        /// <returns></returns>

        public Int32 QMDefinalizeScheduleDAL(int rowID, int getSelectedYear, int getSelectedMonth, string getSelectedQmType)
        {
            dbContext = new PMGSYEntities();

            try
            {

                return dbContext.USP_QM_HELPDESK_DEFINALIZE_SCHEDULE(Convert.ToInt32(rowID), Convert.ToInt32(getSelectedYear), getSelectedMonth, getSelectedQmType, PMGSYSession.Current.UserId, HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]);


            }
            catch (Exception ex)
            {

                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return 0;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion


        #region ImageDetails
        /// <summary>
        /// Getting the Image Details of desired Road and the Admin Code
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="adminQmCode"></param>
        /// <param name="roadCode"></param>
        /// <returns></returns>
        /// 

        public Array QMImageDetailsDAL(int? page, int? rows, string sidx, string sord, out int totalRecords, int adminQmCode, int roadCode)
        {
            dbContext = new PMGSYEntities();
            try
            {

                List<USP_QM_HELPDESK_GET_IMAGE_DETAILS_Result> imageList = new List<USP_QM_HELPDESK_GET_IMAGE_DETAILS_Result>();
                imageList = dbContext.USP_QM_HELPDESK_GET_IMAGE_DETAILS(adminQmCode, roadCode).ToList<USP_QM_HELPDESK_GET_IMAGE_DETAILS_Result>();

                totalRecords = imageList.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString().Trim() == "asc")
                    {
                        imageList = imageList.OrderBy(x => x.QM_FILE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        imageList = imageList.OrderByDescending(x => x.QM_FILE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    imageList = imageList.OrderBy(x => x.QM_FILE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return imageList.Select(imageDetails => new
                {
                    id = imageDetails.QM_FILE_ID,
                    cell = new string[]{
                             imageDetails.QM_FILE_ID.ToString(),
                             imageDetails.QM_FILE_DESCR.ToString(),
                             imageDetails.QM_FILE_NAME.ToString(),
                             imageDetails.QM_FILE_UPLOAD_DATE.ToString(),
                             imageDetails.QM_LATITUDE.ToString(),
                             imageDetails.QM_LONGITUDE.ToString()
                             
                          }
                }).ToArray();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion


        #region ObservationDetails
        /// <summary>
        /// Getting the Observation Details Between two Dates And the QMType("NQM" or "SQM")
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="getSelectedQmType"></param>
        /// <returns></returns>
        /// 
        public Array QMObservationDetailsDAL(int? page, int? rows, string sidx, string sord, out int totalRecords,string filters, DateTime fromDate, DateTime toDate, string getSelectedQmType)
        {
            dbContext = new PMGSYEntities();

            //for Filter search
            JavaScriptSerializer js = null;
            SearchJsonString test = new SearchJsonString();
          
            string monitorNameSearch = string.Empty;
            string stateNameSearch = string.Empty;
            string districtNameSearch = string.Empty;
            try
            {
                if (filters != null)
                {
                    js = new JavaScriptSerializer();
                    test = js.Deserialize<SearchJsonString>(filters);
                    foreach (SearchRules item in test.rules)
                    {
                        switch (item.field)
                        {                         
                            case "MONITER_NAME": monitorNameSearch = item.data;
                                break;
                            case "STATE_NAME": stateNameSearch = item.data;
                                break;
                            case "DISTRICT_NAME": districtNameSearch = item.data;
                                break;
                            default:
                                break;
                        }
                    }
                }

                List<USP_QM_HELPDESK_OBSERVATION_DETAILS_Result> observationList = new List<USP_QM_HELPDESK_OBSERVATION_DETAILS_Result>();
                observationList = dbContext.USP_QM_HELPDESK_OBSERVATION_DETAILS(fromDate, toDate, getSelectedQmType)
                     .Where(
                            x => x.MONITER_NAME.ToLower().Contains(monitorNameSearch.Equals(string.Empty) ? "" : monitorNameSearch.ToLower()) &&
                                 x.STATE_NAME.ToLower().Contains(stateNameSearch.Equals(string.Empty) ? "" : stateNameSearch.ToLower()) &&
                                 x.DISTRICT_NAME.ToLower().Contains(districtNameSearch.Equals(string.Empty) ? "" : districtNameSearch.ToLower())
                            )                    
                    .ToList<USP_QM_HELPDESK_OBSERVATION_DETAILS_Result>();

                totalRecords = observationList.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString().Trim() == "asc")
                    {
                        observationList = observationList.OrderBy(x => x.MONITER_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        observationList = observationList.OrderByDescending(x => x.MONITER_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    observationList = observationList.OrderBy(x => x.MONITER_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return observationList.Select(observationDetails => new
                {
                    id = observationDetails.QM_OBSERVATION_ID,
                    cell = new string[]
                        {
                             observationDetails.QM_OBSERVATION_ID.ToString(),
                             observationDetails.QM_INSPECTION_DATE.ToString(),
                             observationDetails.MONITER_NAME.ToString(),
                             observationDetails.STATE_NAME.ToString(),
                             observationDetails.DISTRICT_NAME.ToString(),
                             observationDetails.BLOCK_NAME.ToString(),
                             observationDetails.IMS_PACKAGE_ID.ToString(),
                             observationDetails.SANCTIONED_YEAR.ToString()+" - "+(observationDetails.SANCTIONED_YEAR+0001).ToString().Substring(2),
                             observationDetails.IMS_ROAD_NAME.ToString(),
                             observationDetails.ROAD_STATUS.ToString(),
                             observationDetails.QM_INSPECTED_START_CHAINAGE.ToString(),
                             observationDetails.QM_INSPECTED_END_CHAINAGE.ToString(),
                             observationDetails.OVERALL_GRADE.ToString(),
                             observationDetails.QM_START_LATITUDE.ToString(),
                             observationDetails.QM_END_LATITUDE.ToString(),
                             observationDetails.QM_START_LONGITUDE.ToString(),
                             observationDetails.QM_END_LONGITUDE.ToString(),
                             "<a href='#' onClick=imageDetailsObservation('"+ observationDetails.ADMIN_SCHEDULE_CODE +"','"+observationDetails.IMS_PR_ROAD_CODE+"') title='Click here to search Image Details' class='imageLinks' >"+"<div style='text-align: center;'><span class='ui-icon ui-icon-search ui-align-center'></span></div>"+"</a>"
                        }
                }).ToArray();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion

        #region QM Message Notifation Details
        /// <summary>
        /// Message Notification Details on desired QM Type
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="rowID"></param>
        /// <returns></returns>
        /// 
        public Array QMNotificationDetailsDAL(int? page, int? rows, string sidx, string sord, out int totalRecords, string QMtype, int stateCode)
        {
            dbContext = new PMGSYEntities();
            try
            {

                List<USP_QM_HELPDESK_NOTIFICATION_DETAILS_Result> NotificationList = new List<USP_QM_HELPDESK_NOTIFICATION_DETAILS_Result>();

                NotificationList = dbContext.USP_QM_HELPDESK_NOTIFICATION_DETAILS(QMtype, "%", stateCode).ToList<USP_QM_HELPDESK_NOTIFICATION_DETAILS_Result>();

                totalRecords = NotificationList.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString().Trim() == "asc")
                    {
                        NotificationList = NotificationList.OrderBy(x => x.Created_Date).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        NotificationList = NotificationList.OrderByDescending(x => x.Created_Date).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    NotificationList = NotificationList.OrderBy(x => x.Created_Date).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return NotificationList.Select(NotDetails => new
                {
                    id = NotDetails.User_Id + "$" + NotDetails.Message_Id,
                    cell = new string[]{
                             NotDetails.User_Name.ToString(),
                             NotDetails.Monitor_Name.ToString(),
                             NotDetails.Message_Text.ToString(),
                             NotDetails.Message_Type.ToString(),
                             NotDetails.Is_Download.ToString(),
                             NotDetails.Created_Date.ToString(),
                             "<a href='#' title='Click here to edit the notification' class='ui-icon ui-icon-pencil ui-align-center' onClick='EditDetails(\"" +NotDetails.User_Id +"$"+NotDetails.Message_Id +"\"); return false;'>Edit Details</a>",
                             "<a href='#' title='Click here to delete the notification' class='ui-icon ui-icon-trash ui-align-center' onClick='DeleteDetails(\"" +NotDetails.User_Id +"$"+NotDetails.Message_Id +"\"); return false;'>Delete Details</a>",

                          }
                }).ToArray();


            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }

        }
        /// <summary>
        /// Save Message Notification Details on desired QM Type
        /// </summary>
        /// <param name="QualityMonitoringHelpDeskModel"></param>
        /// <param name="message"></param>      
        /// <returns></returns>
        /// 
        public bool SaveQMNotificationDetailsDAL(QualityMonitoringHelpDeskModel model_notification, ref string message)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();

            using (TransactionScope objScope = new TransactionScope())
            {



                Models.QUALITY_QM_NOTIFICATIONS notificationDetails = null;

                try
                {


                    int? MonitorUserId = null;
                    int? MaxmessageId = null;
                    //List<int> StateCodeList = null;
                    List<int> MontiorCodeList = null;
                    if (model_notification.QM_Type != "S")
                    {
                        notificationDetails = new QUALITY_QM_NOTIFICATIONS();
                        MonitorUserId = dbContext.ADMIN_QUALITY_MONITORS.Where(m => m.ADMIN_QM_CODE == model_notification.Monitor_CODE).Select(s => s.ADMIN_USER_ID).FirstOrDefault();
                        if (MonitorUserId == null)
                        {
                            message = "Independent user not mapped with username.";
                            return false;
                        }
                        MaxmessageId = dbContext.QUALITY_QM_NOTIFICATIONS.Where(m => m.USER_ID == MonitorUserId).Max(s => (int?)s.MESSAGE_ID);

                        if (MaxmessageId == null)
                        {
                            MaxmessageId = 1;
                        }
                        else
                        {
                            MaxmessageId = MaxmessageId + 1;
                        }
                        notificationDetails.MESSAGE_ID = (Int32)MaxmessageId;
                        notificationDetails.USER_ID = (Int32)MonitorUserId;
                        notificationDetails.MESSAGE_TEXT = model_notification.MESSAGE_TEXT;
                        notificationDetails.MESSAGE_TYPE = model_notification.MESSAGE_TYPE;
                        notificationDetails.IS_DOWNLOAD = false;
                        notificationDetails.TIME_STAMP = DateTime.Now;
                        notificationDetails.USERID = PMGSYSession.Current.UserId;
                        notificationDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        dbContext.QUALITY_QM_NOTIFICATIONS.Add(notificationDetails);
                        dbContext.SaveChanges();

                        //  return true;

                    }

                    else
                    {
                        if (model_notification.MAST_STATE_CODE != 0)
                        {
                            notificationDetails = new QUALITY_QM_NOTIFICATIONS();
                            MonitorUserId = dbContext.ADMIN_QUALITY_MONITORS.Where(m => m.ADMIN_QM_CODE == model_notification.Monitor_CODE).Select(s => s.ADMIN_USER_ID).FirstOrDefault();
                            if (MonitorUserId == null)
                            {
                                message = "Independent user not mapped with username.";
                                return false;
                            }
                            MaxmessageId = dbContext.QUALITY_QM_NOTIFICATIONS.Where(m => m.USER_ID == MonitorUserId).Max(s => (int?)s.MESSAGE_ID);

                            if (MaxmessageId == null)
                            {
                                MaxmessageId = 1;
                            }
                            else
                            {
                                MaxmessageId = MaxmessageId + 1;
                            }
                            notificationDetails.MESSAGE_ID = (Int32)MaxmessageId;
                            notificationDetails.USER_ID = (Int32)MonitorUserId;
                            notificationDetails.MESSAGE_TEXT = model_notification.MESSAGE_TEXT;
                            notificationDetails.MESSAGE_TYPE = model_notification.MESSAGE_TYPE;
                            notificationDetails.IS_DOWNLOAD = false;
                            notificationDetails.TIME_STAMP = DateTime.Now;
                            notificationDetails.USERID = PMGSYSession.Current.UserId;
                            notificationDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                            dbContext.QUALITY_QM_NOTIFICATIONS.Add(notificationDetails);
                            dbContext.SaveChanges();

                            //  return true;


                        }
                        else
                        {

                            MontiorCodeList = (from item in dbContext.ADMIN_QUALITY_MONITORS
                                               where item.ADMIN_QM_TYPE == model_notification.QM_Type
                                              && item.ADMIN_QM_EMPANELLED == "Y"
                                              && item.ADMIN_USER_ID != null
                                               select item.ADMIN_QM_CODE).Distinct().ToList<int>();
                            foreach (var MonitorCode in MontiorCodeList)
                            {
                                notificationDetails = new QUALITY_QM_NOTIFICATIONS();
                                MonitorUserId = dbContext.ADMIN_QUALITY_MONITORS.Where(m => m.ADMIN_QM_CODE == MonitorCode).Select(s => s.ADMIN_USER_ID).FirstOrDefault();
                                if (MonitorUserId != null)
                                {
                                    MaxmessageId = dbContext.QUALITY_QM_NOTIFICATIONS.Where(m => m.USER_ID == MonitorUserId).Max(s => (int?)s.MESSAGE_ID);

                                    if (MaxmessageId == null)
                                    {
                                        MaxmessageId = 1;
                                    }
                                    else
                                    {
                                        MaxmessageId = MaxmessageId + 1;
                                    }
                                    notificationDetails.MESSAGE_ID = (Int32)MaxmessageId;
                                    notificationDetails.USER_ID = (Int32)MonitorUserId;
                                    notificationDetails.MESSAGE_TEXT = model_notification.MESSAGE_TEXT;
                                    notificationDetails.MESSAGE_TYPE = model_notification.MESSAGE_TYPE;
                                    notificationDetails.IS_DOWNLOAD = false;
                                    notificationDetails.TIME_STAMP = DateTime.Now;
                                    notificationDetails.USERID = PMGSYSession.Current.UserId;
                                    notificationDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                                    dbContext.QUALITY_QM_NOTIFICATIONS.Add(notificationDetails);
                                    dbContext.SaveChanges();
                                }

                            }
                            // return true;
                        }
                    }

                    objScope.Complete();
                    return true;

                }




                catch (OptimisticConcurrencyException ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    return false;
                }
                catch (UpdateException ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    return false;
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    return false;
                }
                finally
                {
                    if (dbContext != null)
                    {
                        dbContext.Dispose();
                    }
                }
            }
        }

        /// <summary>
        /// Get Edit Case  Message Notification Details on desired  QM Type
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="messageId"></param>      
        /// <param name="qmType"></param>      
        /// <returns>QualityMonitoringHelpDeskModel</returns>
        public QualityMonitoringHelpDeskModel GetQMNotificationDetailsDAL(int userId, int messageId, string qmType)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {

                Models.QUALITY_QM_NOTIFICATIONS blockDetails = dbContext.QUALITY_QM_NOTIFICATIONS.Where(b => b.USER_ID == userId && b.MESSAGE_ID == messageId).FirstOrDefault();

                QualityMonitoringHelpDeskModel model_QM = null;

                if (blockDetails != null)
                {

                    int? MonitorQMCode = dbContext.ADMIN_QUALITY_MONITORS.Where(m => m.ADMIN_USER_ID == userId).Select(s => s.ADMIN_QM_CODE).FirstOrDefault();
                    Int32 stateCode = 0;
                    if (qmType != "I")
                    {
                        stateCode = (from admin in dbContext.ADMIN_QUALITY_MONITORS where admin.ADMIN_QM_CODE == MonitorQMCode select (Int32)admin.MAST_STATE_CODE).FirstOrDefault();
                    }
                    model_QM = new QualityMonitoringHelpDeskModel()
                    {
                        Message_Id = messageId,
                        MAST_STATE_CODE = (Int32)stateCode,
                        Monitor_CODE = (Int32)MonitorQMCode,
                        MESSAGE_TEXT = blockDetails.MESSAGE_TEXT,
                        MESSAGE_TYPE = Convert.ToString(blockDetails.MESSAGE_TYPE),
                        IS_DOWNLOAD = blockDetails.IS_DOWNLOAD
                    };
                }

                return model_QM;

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
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
        /// <summary>
        /// Update Message Notification Details on desired QM Type
        /// </summary>
        /// <param name="QualityMonitoringHelpDeskModel"></param>
        /// <param name="message"></param>
        /// /// <returns></returns>
        public bool UpdateQMNotificationDetailsDAL(QualityMonitoringHelpDeskModel model_notification, ref string message)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {


                Models.QUALITY_QM_NOTIFICATIONS notificationDetails = new Models.QUALITY_QM_NOTIFICATIONS();
                int? MonitorUserId = dbContext.ADMIN_QUALITY_MONITORS.Where(m => m.ADMIN_QM_CODE == model_notification.Monitor_CODE).Select(s => s.ADMIN_USER_ID).FirstOrDefault();
                if (MonitorUserId == null)
                {
                    message = "Independent user not mapped with username.";
                    return false;
                }

                notificationDetails = dbContext.QUALITY_QM_NOTIFICATIONS.Where(m => m.USER_ID == MonitorUserId && m.MESSAGE_ID == model_notification.Message_Id).FirstOrDefault();

                notificationDetails.USER_ID = (Int32)MonitorUserId;
                notificationDetails.MESSAGE_ID = model_notification.Message_Id;
                notificationDetails.MESSAGE_TEXT = model_notification.MESSAGE_TEXT;
                notificationDetails.MESSAGE_TYPE = model_notification.MESSAGE_TYPE;
                notificationDetails.IS_DOWNLOAD = false;
                notificationDetails.TIME_STAMP = DateTime.Now;
                notificationDetails.USERID = PMGSYSession.Current.UserId;
                notificationDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];


                dbContext.Entry(notificationDetails).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                return true;
            }
            catch (OptimisticConcurrencyException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
            catch (UpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
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
        /// Delete Message Notification Details on desired QM Type
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="messageId"></param>      
        /// <param name="message"></param>   
        /// /// <returns></returns>
        public bool DeleteQMNotificationDetailsDAL(int userId, int messageId, ref string message)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {

                Models.QUALITY_QM_NOTIFICATIONS master_notification = dbContext.QUALITY_QM_NOTIFICATIONS.Where(a => a.USER_ID == userId && a.MESSAGE_ID == messageId).FirstOrDefault();
                bool IsDownLoaded = dbContext.QUALITY_QM_NOTIFICATIONS.Where(m => m.USER_ID == userId && m.MESSAGE_ID == messageId).Select(s => s.IS_DOWNLOAD).FirstOrDefault();
                if (IsDownLoaded == true)
                {
                    message = "You can not delete this Message Notification  details.";
                    return false;
                }
                if (master_notification == null)
                {
                    return false;
                }
                //
                master_notification.USERID = PMGSYSession.Current.UserId;
                master_notification.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                dbContext.Entry(master_notification).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                dbContext.QUALITY_QM_NOTIFICATIONS.Remove(master_notification);
                dbContext.SaveChanges();
                return true;

            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "You can not delete this Message Notification  details.";
                return false;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        #endregion

        #region  QM BroadCast Notification Details
        /// <summary>
        /// Message BroadCast Notification Details on desired QM Type
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="rowID"></param>
        /// <returns></returns>
        /// 
        public Array QMBroadCastNotificationDetailsDAL(int? page, int? rows, string sidx, string sord, out int totalRecords, string QMtype, int stateCode)
        {
            dbContext = new PMGSYEntities();
            try
            {

                List<USP_QM_HELPDESK_BROADCAST_NOTIFICATION_DETAILS_Result> NotificationList;

                //   NotificationList = dbContext.USP_QM_HELPDESK_NOTIFICATION_DETAILS(QMtype, "B").ToList<USP_QM_HELPDESK_BROADCAST_NOTIFICATION_DETAILS_Result>();
                NotificationList = dbContext.Database.SqlQuery<USP_QM_HELPDESK_BROADCAST_NOTIFICATION_DETAILS_Result>("EXEC [omms].[USP_QM_HELPDESK_NOTIFICATION_DETAILS] @MoniotrType,@MessageType",
                    new SqlParameter("@MoniotrType", QMtype),
                    new SqlParameter("@MessageType", "B"),
                    new SqlParameter("@StateCode", stateCode)

                ).ToList<USP_QM_HELPDESK_BROADCAST_NOTIFICATION_DETAILS_Result>();

                totalRecords = NotificationList.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString().Trim() == "asc")
                    {
                        NotificationList = NotificationList.OrderBy(x => x.Created_Date).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        NotificationList = NotificationList.OrderByDescending(x => x.Created_Date).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    NotificationList = NotificationList.OrderBy(x => x.Created_Date).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return NotificationList.Select(NotDetails => new
                {
                    id = NotDetails.BroadCast_Id,
                    cell = new string[]{
                             NotDetails.BroadCast_Id.ToString(),
                             NotDetails.Message_Text.ToString(),                                                     
                             NotDetails.Is_Download.ToString(),
                             NotDetails.Created_Date.ToString(),  
                             "<a href='#' title='Click here to edit the notification' class='ui-icon ui-icon-pencil ui-align-center' onClick='EditBroadCastDetails(\"" +NotDetails.BroadCast_Id+"\"); return false;'>Edit Details</a>",
                             NotDetails.Is_Download.ToString()=="Yes"?"<a href='#' title='Click here to delete the notification' class='ui-icon ui-icon-trash ui-align-center'>Delete Details</a>":"<a href='#' title='Click here to delete the notification' class='ui-icon ui-icon-trash ui-align-center' onClick='DeleteBroadCastDetails(\"" +NotDetails.BroadCast_Id+"\"); return false;'>Delete Details</a>",

                          }
                }).ToArray();


            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }

        }
        /// <summary>
        /// Save BroadCast Message Notification Details on desired QM Type
        /// </summary>
        /// <param name="QualityMonitoringHelpDeskModel"></param>
        /// <param name="message"></param>      
        /// <returns></returns>
        /// 
        public bool SaveQMBroadCastNotificationDetailsDAL(QualityMonitoringBroadCastNotificationModel model_notification, ref string message)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            using (TransactionScope ts = new TransactionScope())
            {
                try
                {


                    //int? MonitorUserId = null;
                    int? MaxmessageId = null;
                    int? MaxBroadCastId = null;
                    List<int?> MontiorCodeList = null;
                    Models.QUALITY_QM_NOTIFICATIONS notificationDetails = null;
                    MaxBroadCastId = dbContext.QUALITY_QM_NOTIFICATIONS.Max(s => (int?)s.BROADCAST_ID);
                    if (MaxBroadCastId == null)
                    {
                        MaxBroadCastId = 1;
                    }
                    else
                    {
                        MaxBroadCastId = MaxBroadCastId + 1;
                    }
                    if (model_notification.QM_Type != "S")
                    {
                        //All user id against monitor
                        //MontiorCodeList = dbContext.ADMIN_QUALITY_MONITORS.Where(m => m.ADMIN_QM_TYPE == model_notification.QM_Type && m.ADMIN_QM_EMPANELLED == "Y").Select(s => s.ADMIN_USER_ID).ToList();

                        MontiorCodeList = (from item in dbContext.ADMIN_QUALITY_MONITORS
                                           where item.ADMIN_QM_TYPE == model_notification.QM_Type
                                           && item.ADMIN_QM_EMPANELLED == "Y"
                                           && item.ADMIN_USER_ID != null
                                           select item.ADMIN_USER_ID).Distinct().ToList<int?>();
                        if (MontiorCodeList.Count == 0)
                        {
                            message = "Independent user not mapped with username.";
                            return false;
                        }
                        foreach (var MonitorCode in MontiorCodeList)
                        {
                            notificationDetails = new Models.QUALITY_QM_NOTIFICATIONS();
                            if (MonitorCode == null)
                            {
                                message = "Independent user not mapped with username.";
                                return false;
                            }
                            MaxmessageId = dbContext.QUALITY_QM_NOTIFICATIONS.Where(m => m.USER_ID == MonitorCode).Max(s => (int?)s.MESSAGE_ID);

                            if (MaxmessageId == null)
                            {
                                MaxmessageId = 1;
                            }
                            else
                            {
                                MaxmessageId = MaxmessageId + 1;
                            }
                            notificationDetails.MESSAGE_ID = (Int32)MaxmessageId;
                            notificationDetails.USER_ID = (Int32)MonitorCode;
                            notificationDetails.BROADCAST_ID = MaxBroadCastId;
                            notificationDetails.MESSAGE_TEXT = model_notification.MESSAGE_TEXT;
                            notificationDetails.MESSAGE_TYPE = "B";
                            notificationDetails.IS_DOWNLOAD = false;
                            notificationDetails.TIME_STAMP = DateTime.Now;
                            notificationDetails.USERID = PMGSYSession.Current.UserId;
                            notificationDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                            dbContext.QUALITY_QM_NOTIFICATIONS.Add(notificationDetails);
                            dbContext.SaveChanges();


                        }


                    }


                    else
                    {
                        //All user id against monitor
                        //MontiorCodeList = dbContext.ADMIN_QUALITY_MONITORS.Where(m => m.ADMIN_QM_TYPE == model_notification.QM_Type && m.ADMIN_QM_EMPANELLED == "Y").Select(s => s.ADMIN_USER_ID).ToList();
                        if (model_notification.MAST_STATE_CODE != 0)
                        {

                            MontiorCodeList = (from item in dbContext.ADMIN_QUALITY_MONITORS
                                               where item.ADMIN_QM_TYPE == model_notification.QM_Type
                                              && item.ADMIN_QM_EMPANELLED == "Y"
                                              && item.ADMIN_USER_ID != null
                                              && item.MAST_STATE_CODE == model_notification.MAST_STATE_CODE
                                               select item.ADMIN_USER_ID).Distinct().ToList<int?>();
                            if (MontiorCodeList.Count == 0)
                            {
                                message = "Independent user not mapped with username.";
                                return false;
                            }
                            foreach (var MonitorCode in MontiorCodeList)
                            {
                                notificationDetails = new Models.QUALITY_QM_NOTIFICATIONS();
                                if (MonitorCode == null)
                                {
                                    message = "Independent user not mapped with username.";
                                    return false;
                                }
                                MaxmessageId = dbContext.QUALITY_QM_NOTIFICATIONS.Where(m => m.USER_ID == MonitorCode).Max(s => (int?)s.MESSAGE_ID);

                                if (MaxmessageId == null)
                                {
                                    MaxmessageId = 1;
                                }
                                else
                                {
                                    MaxmessageId = MaxmessageId + 1;
                                }
                                notificationDetails.MESSAGE_ID = (Int32)MaxmessageId;
                                notificationDetails.USER_ID = (Int32)MonitorCode;
                                notificationDetails.BROADCAST_ID = (Int32)MaxBroadCastId;
                                notificationDetails.MESSAGE_TEXT = model_notification.MESSAGE_TEXT;
                                notificationDetails.MESSAGE_TYPE = "B";
                                notificationDetails.IS_DOWNLOAD = false;
                                notificationDetails.TIME_STAMP = DateTime.Now;
                                notificationDetails.USERID = PMGSYSession.Current.UserId;
                                notificationDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                                dbContext.QUALITY_QM_NOTIFICATIONS.Add(notificationDetails);
                                dbContext.SaveChanges();
                            }
                        }
                        else
                        {


                            MontiorCodeList = (from item in dbContext.ADMIN_QUALITY_MONITORS
                                               where item.ADMIN_QM_TYPE == model_notification.QM_Type
                                              && item.ADMIN_QM_EMPANELLED == "Y"
                                              && item.ADMIN_USER_ID != null
                                              && item.MAST_STATE_CODE != null
                                               select item.ADMIN_USER_ID).Distinct().ToList<int?>();
                            foreach (var MonitorCode in MontiorCodeList)
                            {
                                notificationDetails = new Models.QUALITY_QM_NOTIFICATIONS();

                                MaxmessageId = dbContext.QUALITY_QM_NOTIFICATIONS.Where(m => m.USER_ID == MonitorCode).Max(s => (int?)s.MESSAGE_ID);

                                if (MaxmessageId == null)
                                {
                                    MaxmessageId = 1;
                                }
                                else
                                {
                                    MaxmessageId = MaxmessageId + 1;
                                }
                                notificationDetails.MESSAGE_ID = (Int32)MaxmessageId;
                                notificationDetails.USER_ID = (Int32)MonitorCode;
                                notificationDetails.BROADCAST_ID = (Int32)MaxBroadCastId;
                                notificationDetails.MESSAGE_TEXT = model_notification.MESSAGE_TEXT;
                                notificationDetails.MESSAGE_TYPE = "B";
                                notificationDetails.IS_DOWNLOAD = false;
                                notificationDetails.TIME_STAMP = DateTime.Now;
                                notificationDetails.USERID = PMGSYSession.Current.UserId;
                                notificationDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                                dbContext.QUALITY_QM_NOTIFICATIONS.Add(notificationDetails);
                                dbContext.SaveChanges();
                            }
                        }




                    }


                    ts.Complete();
                    return true;

                }
                catch (OptimisticConcurrencyException ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    ts.Dispose();
                    return false;
                }
                catch (UpdateException ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    ts.Dispose();
                    return false;
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    ts.Dispose();
                    return false;
                }
                finally
                {
                    if (dbContext != null)
                    {
                        dbContext.Dispose();
                    }
                }
            }
        }

        /// <summary>
        /// Get  Edit Case BroadCast Message Notification Details on  desired QM Type
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="messageId"></param>      
        /// <param name="qmType"></param>      
        /// <returns>QualityMonitoringHelpDeskModel</returns>
        public QualityMonitoringBroadCastNotificationModel GetQMBroadCastNotificationDetailsDAL(int broadCastId, string qmType)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {
                // int? MessageId = null;
                QualityMonitoringBroadCastNotificationModel model_QM = null;
                List<int?> StateCodeList = null;
                //int? MonitorQMCode = null;
                Int32 StateCode = 0;
                //MessageId = (from item in dbContext.QUALITY_QM_NOTIFICATIONS
                //             where item.BROADCAST_ID == broadCastId
                //             select item.MESSAGE_ID).FirstOrDefault();

                Models.QUALITY_QM_NOTIFICATIONS notificationDetails = dbContext.QUALITY_QM_NOTIFICATIONS.Where(b => b.BROADCAST_ID == broadCastId).FirstOrDefault();
                if (notificationDetails != null)
                {
                    if (qmType == "S")
                    {
                        // MonitorQMCode = dbContext.ADMIN_QUALITY_MONITORS.Where(m => m.ADMIN_USER_ID == notificationDetails.USER_ID).Select(s => s.ADMIN_QM_CODE).FirstOrDefault();

                        //StateCode = (from admin in dbContext.ADMIN_QUALITY_MONITORS where admin.ADMIN_QM_CODE == notificationDetails.USER_ID select (Int32)admin.MAST_STATE_CODE).FirstOrDefault();
                        StateCodeList = (from QM in dbContext.ADMIN_QUALITY_MONITORS
                                         join QN in dbContext.QUALITY_QM_NOTIFICATIONS
                                         on QM.ADMIN_USER_ID equals QN.USER_ID
                                         where QN.BROADCAST_ID == broadCastId
                                         && QN.MESSAGE_TYPE == "B"
                                         select QM.MAST_STATE_CODE).Distinct().ToList<int?>();
                        if (StateCodeList.Count > 1)
                        {
                            StateCode = 0;
                        }
                        else
                        {
                            StateCode = (from admin in dbContext.ADMIN_QUALITY_MONITORS where admin.ADMIN_USER_ID == notificationDetails.USER_ID select (Int32)admin.MAST_STATE_CODE).FirstOrDefault();

                            //foreach (var item in StateCodeList)
                            //{
                            //        item.Value
                            //}

                            // StateCode=(StateCodeList.Select(s=>s.Value).FirstOrDefault();


                        }
                    }
                    model_QM = new QualityMonitoringBroadCastNotificationModel()
                    {
                        BroadCast_Id = broadCastId,
                        // Message_Id = notificationDetails.MESSAGE_ID,
                        MAST_STATE_CODE = (Int32)StateCode,
                        MESSAGE_TEXT = notificationDetails.MESSAGE_TEXT,
                        IS_DOWNLOAD = notificationDetails.IS_DOWNLOAD
                    };
                }

                return model_QM;

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
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
        /// <summary>
        /// Update Message Notification Details on desired QM Type
        /// </summary>
        /// <param name="QualityMonitoringHelpDeskModel"></param>
        /// <param name="message"></param>
        /// /// <returns></returns>
        public bool UpdateQMBroadCastNotificationDetailsDAL(QualityMonitoringBroadCastNotificationModel model_notification, ref string message)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            using (TransactionScope ts = new TransactionScope())
            {
                try
                {

                    List<int> MontiorCodeList = null;
                    int? MessageId = null;


                    MontiorCodeList = (from item in dbContext.QUALITY_QM_NOTIFICATIONS
                                       where item.BROADCAST_ID == model_notification.BroadCast_Id
                                      && item.BROADCAST_ID != null
                                       select item.USER_ID).ToList<int>();
                    if (MontiorCodeList.Count == 0)
                    {
                        message = "Independent user not mapped with username.";
                        return false;
                    }

                    foreach (var MonitorCode in MontiorCodeList)
                    {
                        Models.QUALITY_QM_NOTIFICATIONS notificationDetails = new Models.QUALITY_QM_NOTIFICATIONS();

                        MessageId = (from item in dbContext.QUALITY_QM_NOTIFICATIONS
                                     where item.USER_ID == MonitorCode
                                    && item.BROADCAST_ID == model_notification.BroadCast_Id
                                     select item.MESSAGE_ID).FirstOrDefault();

                        notificationDetails = dbContext.QUALITY_QM_NOTIFICATIONS.Where(m => m.USER_ID == MonitorCode && m.MESSAGE_ID == MessageId).FirstOrDefault();

                        notificationDetails.USER_ID = (Int32)MonitorCode;
                        notificationDetails.MESSAGE_ID = (Int32)MessageId;
                        notificationDetails.MESSAGE_TEXT = model_notification.MESSAGE_TEXT;
                        notificationDetails.MESSAGE_TYPE = "B";
                        notificationDetails.IS_DOWNLOAD = false;
                        notificationDetails.TIME_STAMP = DateTime.Now;
                        notificationDetails.USERID = PMGSYSession.Current.UserId;
                        notificationDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];


                        dbContext.Entry(notificationDetails).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                    }

                    ts.Complete();
                    return true;

                }
                catch (OptimisticConcurrencyException ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    ts.Dispose();
                    return false;
                }
                catch (UpdateException ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    ts.Dispose();
                    return false;
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    ts.Dispose();
                    return false;
                }
                finally
                {
                    if (dbContext != null)
                    {
                        dbContext.Dispose();
                    }
                }
            }
        }
        /// <summary>
        /// Delete Message Notification Details on desired QM Type
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="messageId"></param>      
        /// <param name="message"></param>   
        /// /// <returns></returns>
        public bool DeleteQMBroadCastNotificationDetailsDAL(int broadCastId, ref string message)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            using (TransactionScope ts = new TransactionScope())
            {
                try
                {
                    List<int> UserIdList = null;
                    int MessageId;
                    // Models.QUALITY_QM_NOTIFICATIONS master_notification = dbContext.QUALITY_QM_NOTIFICATIONS.Where(a => a.BROADCAST_ID == broadCastId).FirstOrDefault();
                    //var master_notification = from item in dbContext.QUALITY_QM_NOTIFICATIONS
                    //                          where item.BROADCAST_ID == broadCastId
                    //                          select item;
                    UserIdList = (from item in dbContext.QUALITY_QM_NOTIFICATIONS
                                  where item.BROADCAST_ID == broadCastId
                                  select item.USER_ID).ToList();
                    foreach (var UserId in UserIdList)
                    {
                        var master_notification = dbContext.QUALITY_QM_NOTIFICATIONS.Where(m => m.BROADCAST_ID == broadCastId && m.USER_ID == UserId).FirstOrDefault();
                        if (master_notification.IS_DOWNLOAD == true)
                        {
                            message = "You can not delete this Broadcast Message Notification  details.";
                            return false;
                        }

                        //
                        master_notification.USERID = PMGSYSession.Current.UserId;
                        master_notification.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        dbContext.Entry(master_notification).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();

                        dbContext.QUALITY_QM_NOTIFICATIONS.Remove(master_notification);
                        dbContext.SaveChanges();
                    }


                    ts.Complete();
                    return true;

                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    message = "You can not delete this Message Notification  details.";
                    ts.Dispose();
                    return false;
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    ts.Dispose();
                    return false;
                }
                finally
                {
                    if (dbContext != null)
                    {
                        dbContext.Dispose();
                    }
                }
            }
        }

        #endregion

        //Commenetd By Hrishikesh Check New Method Below
        /* #region Reset IMEI No Details
        public Array QMResetIMEINoDetailsDAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, string filters)
        {
            dbContext = new PMGSYEntities();
            JavaScriptSerializer js = null;
            SearchJsonString test = new SearchJsonString();
            string usernameSearch = string.Empty;
            string qMTypeSearch = string.Empty;
            string monitorNameSearch = string.Empty;
            string appModeSearch = string.Empty;
            string imeiNoSearch = string.Empty;
            try
            {

                if (filters != null)
                {
                    js = new JavaScriptSerializer();
                    test = js.Deserialize<SearchJsonString>(filters);
                    foreach (SearchRules item in test.rules)
                    {
                        switch (item.field)
                        {
                            case "UserName": usernameSearch = item.data;
                                break;
                            case "QMType": qMTypeSearch = item.data;
                                break;
                            case "MonitorName": monitorNameSearch = item.data;
                                break;
                            case "AppMode": appModeSearch = item.data;
                                break;
                            case "ImeiNo": imeiNoSearch = item.data;
                                break;
                            default:
                                break;
                        }
                    }
                }



                List<USP_QM_HELPDESK_GET_IMEI_DETAILS_Data_Result> monitorIMEIList = null;
                //monitorIMEIList = dbContext.USP_QM_HELPDESK_GET_IMEI_DETAILS().Where(x => x.UserName.ToLower().Contains(monitorNameSearch.Equals(string.Empty) ? "" : monitorNameSearch.ToLower())
                //                                                       ).OrderByDescending(x => x.ADMIN_QM_CODE).ToList<USP_QM_HELPDESK_GET_IMEI_DETAILS_Data_Result>();
                monitorIMEIList = dbContext.Database.SqlQuery<USP_QM_HELPDESK_GET_IMEI_DETAILS_Data_Result>("EXEC [omms].[USP_QM_HELPDESK_GET_IMEI_DETAILS]")
                                      .Where(
                                 x => x.UserName.ToLower().Contains(usernameSearch.Equals(string.Empty) ? "" : usernameSearch.ToLower()) &&
                                      x.ADMIN_QM_TYPE.ToLower().Contains(qMTypeSearch.Equals(string.Empty) ? "" : qMTypeSearch.ToLower()) &&
                                      x.MonitorName.ToLower().Contains(monitorNameSearch.Equals(string.Empty) ? "" : monitorNameSearch.ToLower()) &&
                                      x.ApplicationMode.ToLower().Contains(appModeSearch.Equals(string.Empty) ? "" : appModeSearch.ToLower()) &&
                                      x.ImeiNo.ToLower().Contains(imeiNoSearch.Equals(string.Empty) ? "" : imeiNoSearch.ToLower())
                                      ).ToList<USP_QM_HELPDESK_GET_IMEI_DETAILS_Data_Result>();

                totalRecords = monitorIMEIList.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        monitorIMEIList = monitorIMEIList.OrderBy(x => x.UserName).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        monitorIMEIList = monitorIMEIList.OrderByDescending(x => x.UserName).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    monitorIMEIList = monitorIMEIList.OrderBy(x => x.UserName).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }



                return monitorIMEIList.Select(monitorDetails => new
                {
                    id = monitorDetails.ADMIN_QM_CODE,
                    cell = new string[]
                     {
                             monitorDetails.UserId.ToString(),
                             monitorDetails.UserName==null?"NA":monitorDetails.UserName.ToString(),
                             monitorDetails.MonitorName==null?"NA":monitorDetails.MonitorName.ToString(),
                              monitorDetails.ADMIN_QM_TYPE==null?" ":monitorDetails.ADMIN_QM_TYPE.ToString(),
                              monitorDetails.ImeiNo==null?" ": monitorDetails.ImeiNo.ToString(),
                              monitorDetails.ApplicationMode==null?" ": monitorDetails.ApplicationMode.ToString(),
                              "<a  href='#' title='Click here to Reset IMEI Number' title='Reset IMEI Number'   onClick='ResetIMEIMobDetail(\""+monitorDetails.UserId.ToString()+"\"); return false' >"+"<span class='ui-icon ui-icon-plus ui-align-center'></span>"+"</a>",
                             // "<a  href='#' onclick=updateIMEIApplicationMode('"+monitorDetails.ADMIN_QM_CODE+"$"+monitorDetails.ImeiNo+"') title='Click here to search Log Details' class='logLinks' >"+"<span class='ui-icon ui-icon-search ui-align-center'></span>"+"</a>",
                              "<a href='#' title='Click here to change Application Mode of IMEI Number' title=' Change Application Mode' class='ui-icon ui-icon-pencil ui-align-center' onClick='UpdateIMEIApplicationMode(\""+monitorDetails.UserId+"$"+monitorDetails.ImeiNo+"$"+monitorDetails.ApplicationMode+"\"); return false;'>Update Details</a>",
              
                     }

                }).ToArray();

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return (null);
            }
            finally
            {
                dbContext.Dispose();
            }

        }*/


        #region Change by Hrishikesh  "reset IMEI and REsetIMEI Count Column" Functionality to CQCAdmin Role
        //Changed by Hrishikesh to Provide "reset IMEI and REsetIMEI Count" Functionality to CQCAdmin 
        public Array QMResetIMEINoDetailsDAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, string filters)
        {
            dbContext = new PMGSYEntities();
            JavaScriptSerializer js = null;
            SearchJsonString test = new SearchJsonString();
            string usernameSearch = string.Empty;
            string qMTypeSearch = string.Empty;
            string monitorNameSearch = string.Empty;
            string appModeSearch = string.Empty;
            string imeiNoSearch = string.Empty;
            try
            {

                if (filters != null)
                {
                    js = new JavaScriptSerializer();
                    test = js.Deserialize<SearchJsonString>(filters);
                    foreach (SearchRules item in test.rules)
                    {
                        switch (item.field)
                        {
                            case "UserName":
                                usernameSearch = item.data;
                                break;
                            case "QMType":
                                qMTypeSearch = item.data;
                                break;
                            case "MonitorName":
                                monitorNameSearch = item.data;
                                break;
                            case "AppMode":
                                appModeSearch = item.data;
                                break;
                            case "ImeiNo":
                                imeiNoSearch = item.data;
                                break;
                            default:
                                break;
                        }
                    }
                }



                List<USP_QM_HELPDESK_GET_IMEI_DETAILS_Data_Result> monitorIMEIList = null;
                //monitorIMEIList = dbContext.USP_QM_HELPDESK_GET_IMEI_DETAILS().Where(x => x.UserName.ToLower().Contains(monitorNameSearch.Equals(string.Empty) ? "" : monitorNameSearch.ToLower())
                //                                                       ).OrderByDescending(x => x.ADMIN_QM_CODE).ToList<USP_QM_HELPDESK_GET_IMEI_DETAILS_Data_Result>();
                monitorIMEIList = dbContext.Database.SqlQuery<USP_QM_HELPDESK_GET_IMEI_DETAILS_Data_Result>("EXEC [omms].[USP_QM_HELPDESK_GET_IMEI_DETAILS]")
                                      .Where(
                                 x => x.UserName.ToLower().Contains(usernameSearch.Equals(string.Empty) ? "" : usernameSearch.ToLower()) &&
                                      x.ADMIN_QM_TYPE.ToLower().Contains(qMTypeSearch.Equals(string.Empty) ? "" : qMTypeSearch.ToLower()) &&
                                      x.MonitorName.ToLower().Contains(monitorNameSearch.Equals(string.Empty) ? "" : monitorNameSearch.ToLower()) &&
                                      x.ApplicationMode.ToLower().Contains(appModeSearch.Equals(string.Empty) ? "" : appModeSearch.ToLower()) &&
                                      x.ImeiNo.ToLower().Contains(imeiNoSearch.Equals(string.Empty) ? "" : imeiNoSearch.ToLower())
                                      ).ToList<USP_QM_HELPDESK_GET_IMEI_DETAILS_Data_Result>();

                totalRecords = monitorIMEIList.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        monitorIMEIList = monitorIMEIList.OrderBy(x => x.UserName).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        monitorIMEIList = monitorIMEIList.OrderByDescending(x => x.UserName).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    monitorIMEIList = monitorIMEIList.OrderBy(x => x.UserName).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                //Changed by Hrishikesh to Provide "reset IMEI and REsetIMEI Count" Functionality to CQCAdmin (if & else)
                if (PMGSYSession.Current.RoleCode == 5)
                {

                    return monitorIMEIList.Select(monitorDetails => new
                    {
                        id = monitorDetails.ADMIN_QM_CODE,
                        cell = new string[]
                         {
                                  monitorDetails.UserId.ToString(),
                                  monitorDetails.UserName==null?"NA":monitorDetails.UserName.ToString(),
                                  monitorDetails.MonitorName==null?"NA":monitorDetails.MonitorName.ToString(),
                                  monitorDetails.ADMIN_QM_TYPE==null?" ":monitorDetails.ADMIN_QM_TYPE.ToString(),
                                  monitorDetails.ImeiNo==null?" ": monitorDetails.ImeiNo.ToString(),
                                  //monitorDetails.ApplicationMode==null?" ": monitorDetails.ApplicationMode.ToString(),
                                   "<a  href='#' title='Click here to Reset IMEI Number' title='Reset IMEI Number'   onClick='ResetIMEIMobDetail(\""+monitorDetails.UserId.ToString()+"\"); return false' >"+"<span class='ui-icon ui-icon-plus ui-align-center'></span>"+"</a>",
                                   
                             //"<a href='#' title='Click here to change Application Mode of IMEI Number' title=' Change Application Mode' class='ui-icon ui-icon-pencil ui-align-center' onClick='UpdateIMEIApplicationMode(\""+monitorDetails.UserId+"$"+monitorDetails.ImeiNo+"$"+monitorDetails.ApplicationMode+"\"); return false;'>Update Details</a>",
                                  monitorDetails.ResetIMEICount==0 ? "": monitorDetails.ResetIMEICount.ToString(),

                                  "<a  href='#' title='Click here to see IMEI reset details' title='IMEI reset details'   onClick='ShowIMEIResetDetails(\""+monitorDetails.UserId.ToString()+"\"); return false' >"+"<span class='ui-icon ui-icon-search ui-align-center'></span>"+"</a>",
                                   //"<a href='#' onClick='ShowIMEIResetDetails('"+monitorDetails.UserId.ToString() + "$"+monitorDetails.UserName+"') title='Click here to see IMEI reset details'>"  + "<span class='ui-icon ui-icon-search ui-align-center'></span>" + "</a>
                         }

                    }).ToArray();
                }
                else
                {
                    return monitorIMEIList.Select(monitorDetails => new
                    {
                        id = monitorDetails.ADMIN_QM_CODE,
                        cell = new string[]
                         {
                                  monitorDetails.UserId.ToString(),
                                  monitorDetails.UserName==null?"NA":monitorDetails.UserName.ToString(),
                                  monitorDetails.MonitorName==null?"NA":monitorDetails.MonitorName.ToString(),
                                  monitorDetails.ADMIN_QM_TYPE==null?" ":monitorDetails.ADMIN_QM_TYPE.ToString(),
                                  monitorDetails.ImeiNo==null?" ": monitorDetails.ImeiNo.ToString(),
                                  monitorDetails.ApplicationMode==null?" ": monitorDetails.ApplicationMode.ToString(),
                                   "<a  href='#' title='Click here to Reset IMEI Number' title='Reset IMEI Number'   onClick='ResetIMEIMobDetail(\""+monitorDetails.UserId.ToString()+"\"); return false' >"+"<span class='ui-icon ui-icon-plus ui-align-center'></span>"+"</a>",
                                  // "<a  href='#' onclick=updateIMEIApplicationMode('"+monitorDetails.ADMIN_QM_CODE+"$"+monitorDetails.ImeiNo+"') title='Click here to search Log Details' class='logLinks' >"+"<span class='ui-icon ui-icon-search ui-align-center'></span>"+"</a>",
                                   "<a href='#' title='Click here to change Application Mode of IMEI Number' title=' Change Application Mode' class='ui-icon ui-icon-pencil ui-align-center' onClick='UpdateIMEIApplicationMode(\""+monitorDetails.UserId+"$"+monitorDetails.ImeiNo+"$"+monitorDetails.ApplicationMode+"\"); return false;'>Update Details</a>",
                         }

                    }).ToArray();
                }

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return (null);
            }
            finally
            {
                dbContext.Dispose();
            }

        }


        #region added by Hrishikesh  "REsetIMEI Count" Functionality to CQCAdmin Role
        //added by Hrishikesh to Provide "reset IMEI and REsetIMEI Count" Functionality to CQCAdmin -(To fetch the reset imei detail)
        public Array GetIMEIResetDetailsDAL(int userId, out int totalRecords, int page, int rows, string sidx, string sord, string filters)
        {
            dbContext = new PMGSYEntities();

            try
            {
                List<QUALITY_MOB_IMEI_NO_SHADOW> qualitymobimeinolist = null;
                qualitymobimeinolist = dbContext.QUALITY_MOB_IMEI_NO_SHADOW.Where(x => x.UserId == userId).ToList();
                //string userName = dbContext.UM_User_Master.Where(x => x.UserID == userId).Select(x => x.UserName).FirstOrDefault();

                totalRecords = qualitymobimeinolist.Count();
                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        qualitymobimeinolist = qualitymobimeinolist.OrderBy(x => x.AuditDate).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        qualitymobimeinolist = qualitymobimeinolist.OrderByDescending(x => x.AuditDate).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    qualitymobimeinolist = qualitymobimeinolist.OrderBy(x => x.AuditDate).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }
                return qualitymobimeinolist.Select(
                    qualitymobimeinodetails => new
                    {
                        id = qualitymobimeinodetails.UserId,
                        cell = new string[]
                        {
                            qualitymobimeinodetails.UserId.ToString(),
                            //qualitymobimeinodetails.AuditAction==null? "NA" : userName,  //please note here--qualitymobimeinodetails obj does not have username property so we assign "userName to AuditAction"
                                                                                         //so dont consider AuditAction as AuditAction it binds userName in JQGrid
                            //qualitymobimeinodetails.AuditDate==null? "NA":(qualitymobimeinodetails.AuditDate).ToString().Split(' ')[0]  +"&nbsp; &nbsp; &nbsp;"+ (qualitymobimeinodetails.AuditDate).ToString().Split(' ')[1].Replace('-','/')
                            qualitymobimeinodetails.AuditDate==null? "NA":(qualitymobimeinodetails.AuditDate).ToString("dd/MM/yyyy hh:mm:ss tt").Split(' ')[0] +"&nbsp; &nbsp; &nbsp;"+(qualitymobimeinodetails.AuditDate).ToString("dd/MM/yyyy hh:mm:ss tt").Split(' ')[1].Split(':')[0]+":"+(qualitymobimeinodetails.AuditDate).ToString("dd/MM/yyyy hh:mm:ss tt").Split(' ')[1].Split(':')[1] +"&nbsp;"+(qualitymobimeinodetails.AuditDate).ToString("dd/MM/yyyy hh:mm:ss tt").Split(' ')[2]
                            //qualitymobimeinodetails.AuditUser==null? "NA": (qualitymobimeinodetails.AuditUser.Contains("sa") ? "CDAC Team" : qualitymobimeinodetails.AuditUser)
                        }

                    }).ToArray();

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }

        }//end GetIMEIResetDetailsDAL()
        #endregion

        /// <summary>
        /// Update IMEI Application  Details on desired QM Code and IMEI No
        /// </summary>
        /// <param name="QualityMonitoringHelpDeskModel"></param>
        /// <param name="message"></param>
        /// /// <returns></returns>
        public bool UpdateIMEIApplicationModeDetailsDAL(QM_HELPDESK_IMEI_MODEL_DETAILS model_imei, ref string message)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {

                Models.QUALITY_MOB_IMEI_NO imeiDetails = new Models.QUALITY_MOB_IMEI_NO();
                // int? Userid = dbContext.ADMIN_QUALITY_MONITORS.Where(m => m.ADMIN_QM_CODE == model_imei.ADMIN_QM_CODE).Select(a => a.ADMIN_USER_ID).FirstOrDefault();
                imeiDetails = dbContext.QUALITY_MOB_IMEI_NO.Where(m => m.UserId == model_imei.UserId).FirstOrDefault();
                if (imeiDetails != null)
                {
                    imeiDetails.ApplicationMode = model_imei.ApplicationMode.ToString().Substring(0, 1) == "D" ? "L" : "D";
                    dbContext.Entry(imeiDetails).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                }
                else
                {
                    message = "There is no imei number  to change Application Mode";
                }
                return true;
            }

            catch (OptimisticConcurrencyException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);

                return false;
            }
            catch (UpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);

                return false;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);

                return false;
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
        /// Update IMEI Application  Details on desired QM Code and IMEI No
        /// </summary>
        /// <param name="QualityMonitoringHelpDeskModel"></param>
        /// <param name="message"></param>
        /// /// <returns></returns>
        public bool UpdateIMEINoResetDetailsDAL(QM_HELPDESK_IMEI_MODEL_DETAILS model_imei, ref string message)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {

                Models.QUALITY_MOB_IMEI_NO imeiDetails = new Models.QUALITY_MOB_IMEI_NO();
                // int? Userid = dbContext.ADMIN_QUALITY_MONITORS.Where(m => m.ADMIN_QM_CODE == model_imei.ADMIN_QM_CODE).Select(a => a.ADMIN_USER_ID).FirstOrDefault();
                imeiDetails = dbContext.QUALITY_MOB_IMEI_NO.Where(m => m.UserId == model_imei.UserId).FirstOrDefault();
                if (imeiDetails != null)
                {
                    imeiDetails.ImeiNo = string.Empty;
                    dbContext.Entry(imeiDetails).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                }
                else
                {
                    message = "There is no imei number  to change Application Mode";
                }
                return true;
            }

            catch (OptimisticConcurrencyException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);

                return false;
            }
            catch (UpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);

                return false;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);

                return false;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }


        #endregion



        #region ATR Deletion

        /// <summary>
        /// Update ATR STatus
        /// </summary>
        /// <param name="obsId"></param>
        /// <returns></returns>
        public string UpdateATRStatusDAL(int obsId)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {
                int updateCount = dbContext.USP_QM_HELPDESK_REGRADE_ATR(obsId);
                if (updateCount > 0)
                    return string.Empty;
                else
                    return "An Error Occurred While Processing Your Request.";
            }
            catch (OptimisticConcurrencyException ex)
            {
                return "An Error Occurred While Processing Your Request.";
            }
            catch (UpdateException ex)
            {
                return "An Error Occurred While Processing Your Request.";
            }
            catch (Exception ex)
            {
                return "An Error Occurred While Processing Your Request.";
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        #endregion



        #region 2 tier atr vikky
        public string Update2TierATRStatusDAL(int obsId)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {
                int updateCount = dbContext.USP_QM_HELPDESK_REGRADE_SQM_ATR(obsId);
                if (updateCount > 0)
                    return string.Empty;
                else
                    return "An Error Occurred While Processing Your Request.";
            }
            catch (OptimisticConcurrencyException ex)
            {
                ErrorLog.LogError(ex, "QualitymonitoringController.Update2TierATRStatusDAL()");
                return "An Error Occurred While Processing Your Request.";
            }
            catch (UpdateException ex)
            {
                ErrorLog.LogError(ex, "QualitymonitoringController.Update2TierATRStatusDAL()");
                return "An Error Occurred While Processing Your Request.";
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualitymonitoringController.Update2TierATRStatusDAL()");
                return "An Error Occurred While Processing Your Request.";
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        #endregion

    }

    public interface IQualityMonitoringHelpDeskDAL
    {
        Array QMMonitorDetailsDAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int getSelectedMonth, string getSelectedYear, string getSelectedQmType, string filters);
        Array QMScheduleDetailsDAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int rowID, int getSelectedMonth, string getSelectedYear, string getSelectedQmType, string filters);
        Array QMLogDetailsDAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int rowID);
        Int32 QMStoreImeiNumberDAL(int adminQmCode, string imeiNumber);
        String QMUserNameDAL(int adminQmCode);
        Array QMImageDetailsDAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int adminQmCode, int roadCode);
        Array QMObservationDetailsDAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, string filters, DateTime fromDate, DateTime toDate, string getSelectedQmType);
        String QMIsScheduleDownloadDAL(int rowID, string getSelectedYear, int getSelectedMonth, string getSelectedQmType);
        Int32 QMDefinalizeScheduleDAL(int rowID, int getSelectedYear, int getSelectedMonth, string getSelectedQmType);
        Array QMNotificationDetailsDAL(int? page, int? rows, string sidx, string sord, out int totalRecords, string QMtype, int StateCode);
        bool SaveQMNotificationDetailsDAL(QualityMonitoringHelpDeskModel model_notification, ref string message);
        QualityMonitoringHelpDeskModel GetQMNotificationDetailsDAL(int userId, int messageId, string qmType);
        bool UpdateQMNotificationDetailsDAL(QualityMonitoringHelpDeskModel model_notification, ref string message);
        bool DeleteQMNotificationDetailsDAL(int userId, int messageId, ref string message);
        Array QMBroadCastNotificationDetailsDAL(int? page, int? rows, string sidx, string sord, out int totalRecords, string QMtype, int StateCode);
        bool SaveQMBroadCastNotificationDetailsDAL(QualityMonitoringBroadCastNotificationModel model_notification, ref string message);
        QualityMonitoringBroadCastNotificationModel GetQMBroadCastNotificationDetailsDAL(int broadCastId, string qmType);
        bool UpdateQMBroadCastNotificationDetailsDAL(QualityMonitoringBroadCastNotificationModel model_notification, ref string message);
        bool DeleteQMBroadCastNotificationDetailsDAL(int broadCastId, ref string message);
        Array QMResetIMEINoDetailsDAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, string filters);
        bool UpdateIMEIApplicationModeDetailsDAL(QM_HELPDESK_IMEI_MODEL_DETAILS model_imei, ref string message);
        bool UpdateIMEINoResetDetailsDAL(QM_HELPDESK_IMEI_MODEL_DETAILS model_imei, ref string message);
    }
}