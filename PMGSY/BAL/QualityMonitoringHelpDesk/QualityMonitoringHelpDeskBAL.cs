using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.DAL.QualityMonitoringHelpDesk;
using PMGSY.Models.QualityMonitoringHelpDesk;

namespace PMGSY.BAL.QualityMonitoringHelpDesk
{

    public class QualityMonitoringHelpDeskBAL : IQualityMonitorHelpDesk
    {

        QualityMonitoringHelpDeskDAL qualityMonitoringHelpDeskDALContext = new QualityMonitoringHelpDeskDAL();

        public Array QMMonitorDetailsBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int getSelectedMonth, string getSelectedYear, string getSelectedQmType, string filters)
        {
            return qualityMonitoringHelpDeskDALContext.QMMonitorDetailsDAL(page, rows, sidx, sord, out totalRecords, getSelectedMonth, getSelectedYear, getSelectedQmType, filters);


        }


        public Array QMScheduleDetailsBAL(int page, int rows, string sidx, string sord, out int totalRecords, int rowID, int getSelectedMonth, string getSelectedYear, string getSelectedQmType, string filters)
        {
            return qualityMonitoringHelpDeskDALContext.QMScheduleDetailsDAL(page, rows, sidx, sord, out totalRecords, rowID, getSelectedMonth, getSelectedYear, getSelectedQmType, filters);
        }


        public Array QMLogDetailsBAL(int page, int rows, string sidx, string sord, out int totalRecords, int rowID)
        {
            return qualityMonitoringHelpDeskDALContext.QMLogDetailsDAL(page, rows, sidx, sord, out totalRecords, rowID);
        }


        public Int32 QMStoreImeiNumberBAL(int adminQmCode, string imeiNumber)
        {
            return qualityMonitoringHelpDeskDALContext.QMStoreImeiNumberDAL(adminQmCode, imeiNumber);
        }


        public string QMUserNameBAL(int adminQmCode)
        {
            return qualityMonitoringHelpDeskDALContext.QMUserNameDAL(adminQmCode);
        }


        public Array QMImageDetailsBAL(int page, int rows, string sidx, string sord, out int totalRecords, int adminQmCode, int roadCode)
        {
            return qualityMonitoringHelpDeskDALContext.QMImageDetailsDAL(page, rows, sidx, sord, out totalRecords, adminQmCode, roadCode);
        }


        public Array QMObservationDetailsBAL(int page, int rows, string sidx, string sord, out int totalRecords,string filters, DateTime fromDate, DateTime toDate, string getSelectedQmType)
        {
            return qualityMonitoringHelpDeskDALContext.QMObservationDetailsDAL(page, rows, sidx, sord, out totalRecords,filters, fromDate, toDate, getSelectedQmType);
        }


        public String QMIsScheduleDownloadBAL(int rowID, string getSelectedYear, int getSelectedMonth, string getSelectedQmType)
        {
            return qualityMonitoringHelpDeskDALContext.QMIsScheduleDownloadDAL(rowID, getSelectedYear, getSelectedMonth, getSelectedQmType);
        }


        public Int32 QMDefinalizeScheduleBAL(int rowID, int getSelectedYear, int getSelectedMonth, string getSelectedQmType)
        {
            return qualityMonitoringHelpDeskDALContext.QMDefinalizeScheduleDAL(rowID, getSelectedYear, getSelectedMonth, getSelectedQmType);
        }
        #region QM Messege Notification
        public Array QMNotificationDetailsBAL(int? page, int? rows, string sidx, string sord, out int totalRecords, string QMtype, int StateCode)
        {
            return qualityMonitoringHelpDeskDALContext.QMNotificationDetailsDAL(page, rows, sidx, sord, out totalRecords, QMtype, StateCode);

        }
        public bool SaveQMNotificationDetailsBAL(QualityMonitoringHelpDeskModel model_notification, ref string message)
        {
            return qualityMonitoringHelpDeskDALContext.SaveQMNotificationDetailsDAL(model_notification, ref message);

        }
        public QualityMonitoringHelpDeskModel GetQMNotificationDetailsBAL(int userId, int messageId, string qmType)
        {
            return qualityMonitoringHelpDeskDALContext.GetQMNotificationDetailsDAL(userId, messageId, qmType);
        }
        public bool UpdateQMNotificationDetailsBAL(QualityMonitoringHelpDeskModel model_notification, ref string message)
        {
            return qualityMonitoringHelpDeskDALContext.UpdateQMNotificationDetailsDAL(model_notification, ref message);

        }
        public bool DeleteQMNotificationDetailsBAL(int userId, int messageId, ref string message)
        {
            return qualityMonitoringHelpDeskDALContext.DeleteQMNotificationDetailsDAL(userId, messageId, ref message);

        }
        #endregion


        #region QM BroadCast Message Notification
        public Array QMBroadCastNotificationDetailsBAL(int? page, int? rows, string sidx, string sord, out int totalRecords, string QMtype, int StateCode)
        {
            return qualityMonitoringHelpDeskDALContext.QMBroadCastNotificationDetailsDAL(page, rows, sidx, sord, out totalRecords, QMtype, StateCode);

        }
        public bool SaveQMBroadCastNotificationDetailsBAL(QualityMonitoringBroadCastNotificationModel model_notification, ref string message)
        {
            return qualityMonitoringHelpDeskDALContext.SaveQMBroadCastNotificationDetailsDAL(model_notification, ref message);
        }
        public QualityMonitoringBroadCastNotificationModel GetQMBroadCastNotificationDetailsBAL(int broadCastId, string qmType)
        {
            return qualityMonitoringHelpDeskDALContext.GetQMBroadCastNotificationDetailsDAL(broadCastId, qmType);
        }
        public bool UpdateQMBroadCastNotificationDetailsBAL(QualityMonitoringBroadCastNotificationModel model_notification, ref string message)
        {
            return qualityMonitoringHelpDeskDALContext.UpdateQMBroadCastNotificationDetailsDAL(model_notification, ref message);

        }
        public bool DeleteQMBroadCastNotificationDetailsBAL(int broadCastId, ref string message)
        {
            return qualityMonitoringHelpDeskDALContext.DeleteQMBroadCastNotificationDetailsDAL(broadCastId, ref message);

        }

        #endregion


        #region IMEI Reset Details
        public Array QMResetIMEINoDetailsBAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, string filters)
        {
            return qualityMonitoringHelpDeskDALContext.QMResetIMEINoDetailsDAL(page, rows, sidx, sord, out totalRecords, filters);
        }
        public bool UpdateIMEIApplicationModeDetailsBAL(QM_HELPDESK_IMEI_MODEL_DETAILS model_imei, ref string message)
        {
            return qualityMonitoringHelpDeskDALContext.UpdateIMEIApplicationModeDetailsDAL(model_imei, ref message);

        }
        public bool UpdateIMEINoResetDetailsBAL(QM_HELPDESK_IMEI_MODEL_DETAILS model_imei, ref string message)
        {
            return qualityMonitoringHelpDeskDALContext.UpdateIMEINoResetDetailsDAL(model_imei, ref message);
        }
        #endregion

        #region added by Hrishikesh  "reset IMEI and REsetIMEI Count" Functionality to CQCAdmin Role
        //added by Hrishikesh to Provide "reset IMEI and REsetIMEI Count" Functionality to CQCAdmin -(To fetch the reset imei detail)
        public Array GetIMEIResetDetailsBAL(int userid, out int totalRecords, int page, int rows, string sidx, string sord, string filters)
        {
            return qualityMonitoringHelpDeskDALContext.GetIMEIResetDetailsDAL(userid, out totalRecords, page, rows, sidx, sord, filters);
        }//GetIMEIResetDetailsBAL()
        #endregion

        #region ATR Deletion

        public string UpdateATRStatusBAL(int obsId)
        {
            return qualityMonitoringHelpDeskDALContext.UpdateATRStatusDAL(obsId);
        }

        #endregion

        #region 2 tier atr vikky
        public string Update2TierATRStatusBAL(int obsId)
        {
            return qualityMonitoringHelpDeskDALContext.Update2TierATRStatusDAL(obsId);
        }
        #endregion

    }

    public interface IQualityMonitorHelpDesk
    {
        Array QMMonitorDetailsBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int getSelectedMonth, string getSelectedYear, string getSelectedQmType, string filters);
        Array QMScheduleDetailsBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int rowID, int getSelectedMonth, string getSelectedYear, string getSelectedQmType, string filters);
        Array QMLogDetailsBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int rowID);
        Int32 QMStoreImeiNumberBAL(int adminQmCode, string imeiNumber);
        String QMUserNameBAL(int adminQmCode);
        Array QMImageDetailsBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int adminQmCode, int roadCode);
        Array QMObservationDetailsBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, string filters, DateTime fromDate, DateTime toDate, string getSelectedQmType);
        String QMIsScheduleDownloadBAL(int rowID, string getSelectedYear, int getSelectedMonth, string getSelectedQmType);
        Int32 QMDefinalizeScheduleBAL(int rowID, int getSelectedYear, int getSelectedMonth, string getSelectedQmType);
        Array QMNotificationDetailsBAL(int? page, int? rows, string sidx, string sord, out int totalRecords, string QMtype, int StateCode);
        bool SaveQMNotificationDetailsBAL(QualityMonitoringHelpDeskModel model_notification, ref string message);
        QualityMonitoringHelpDeskModel GetQMNotificationDetailsBAL(int userId, int messageId, string qmType);
        bool UpdateQMNotificationDetailsBAL(QualityMonitoringHelpDeskModel model_notification, ref string message);
        bool DeleteQMNotificationDetailsBAL(int userId, int messageId, ref string message);
        Array QMBroadCastNotificationDetailsBAL(int? page, int? rows, string sidx, string sord, out int totalRecords, string QMtype, int StateCode);
        bool SaveQMBroadCastNotificationDetailsBAL(QualityMonitoringBroadCastNotificationModel model_notification, ref string message);
        QualityMonitoringBroadCastNotificationModel GetQMBroadCastNotificationDetailsBAL(int broadCastId, string qmType);
        bool UpdateQMBroadCastNotificationDetailsBAL(QualityMonitoringBroadCastNotificationModel model_notification, ref string message);
        bool DeleteQMBroadCastNotificationDetailsBAL(int broadCastId, ref string message);
        Array QMResetIMEINoDetailsBAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, string filters);

        #region added by Hrishikesh  "reset IMEI and REsetIMEI Count" Functionality to CQCAdmin Role
        //added by Hrishikesh to Provide "reset IMEI and REsetIMEI Count" Functionality to CQCAdmin -(To fetch the reset imei detail
        Array GetIMEIResetDetailsBAL(int userid, out int totalRecords, int page, int rows, string sidx, string sord, string filters);
        #endregion

        bool UpdateIMEIApplicationModeDetailsBAL(QM_HELPDESK_IMEI_MODEL_DETAILS model_imei, ref string message);
        bool UpdateIMEINoResetDetailsBAL(QM_HELPDESK_IMEI_MODEL_DETAILS model_imei, ref string message);
        string UpdateATRStatusBAL(int obsId);

        #region 2 tier atr vikky
        string Update2TierATRStatusBAL(int obsId);
        #endregion
    }
}