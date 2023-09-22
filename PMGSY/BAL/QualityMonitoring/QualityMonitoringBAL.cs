#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   QualityMonitoringDAL.cs        
        * Description   :   BAL Methods for calling Data Methods of DAL
        *                   for Creation of Schedules, Fill Observations, Uploading Images & ATRs, Correcting Observations, Creation of Monitors etc.
        * Author        :   Shyam Yadav 
        * Creation Date :   10/Jun/2013
 **/
#endregion


using Mvc.Mailer;
using PMGSY.Common;
using PMGSY.DAL.QualityMonitoring;
using PMGSY.Extensions;
using PMGSY.Mailers;
using PMGSY.Models;
using PMGSY.Models.Common;
using PMGSY.Models.Master;
using PMGSY.Models.QualityMonitoring;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.BAL.QualityMonitoring
{
    public class QualityMonitoringBAL : IQualityMonitoringBAL
    {
        private IQualityMonitoringDAL qualityDAL;
        private PMGSYEntities dbContext;


        #region CQCAdmin


        /// <summary>
        /// Populate CQC LIST
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        public Array GetScheduleListBAL(int month, int year, int page, int rows, string sidx, string sord, out Int32 totalRecords, string filters)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.GetScheduleListDAL(month, year, page, rows, sidx, sord, out totalRecords, filters);
        }

        /// <summary>
        /// SQC Letter
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="filters"></param>
        /// <param name="inspMonth"></param>
        /// <param name="inspYear"></param>
        /// <returns></returns>
        public Array QMSQCLetterListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, string filters, int inspMonth, int inspYear)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.QMSQCLetterListDAL(page, rows, sidx, sord, out totalRecords, filters, inspMonth, inspYear);
        }

        /// <summary>
        /// Add SQC Letter Details
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userType"></param>
        /// <param name="inspMonth"></param>
        /// <param name="inspYear"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool AddSQCLetterDetailsBAL(int id, short inspMonth, short inspYear, ref string message)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.AddSQCLetterDetailsDAL(id, inspMonth, inspYear, ref message);
        }

        /// <summary>
        /// Add NQM Letter Details
        /// </summary>
        /// <param name="scheduleCode"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool AddNQMLetterDetailsBAL(int scheduleCode, ref string message)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.AddNQMLetterDetailsDAL(scheduleCode, ref message);
        }

        /// <summary>
        /// Send Mail - Prepare all options for sending mail.
        /// </summary>
        /// <param name="qmLetterModel"></param>
        /// <param name="ErrorMessage"></param>
        /// <returns></returns>
        public virtual MailMessage SendMailCustomFunc(QMLetterModel qmLetterModel, ref String ErrorMessage)
        {
            SendMailCustomFuncModel eMailModel = new SendMailCustomFuncModel();
            dbContext = new PMGSYEntities();
            ADMIN_SQC sqcDetails = null;
            ADMIN_QUALITY_MONITORS qmDetails = null;
            QUALITY_QM_LETTER letterDetails = null;
            QUALITY_QM_SCHEDULE scheduleDetails = null;
            try
            {
                if (qmLetterModel.QC_TYPE.Equals("S"))  //SQC
                {
                    sqcDetails = dbContext.ADMIN_SQC.Find(qmLetterModel.QC_CODE);
                    letterDetails = dbContext.QUALITY_QM_LETTER.Find(qmLetterModel.LETTER_ID);
                    eMailModel.RoleCode = 8;
                    eMailModel.EmailRecepient = sqcDetails.ADMIN_QC_EMAIL;
                    DateTime dtDate = new DateTime(letterDetails.ADMIN_IM_YEAR, letterDetails.ADMIN_IM_MONTH, 1);
                    qmLetterModel.MONTH_TEXT = dtDate.ToString("MMMM");
                    eMailModel.EmailSubject = "Inspection Schedule of " + qmLetterModel.MONTH_TEXT + ", " + letterDetails.ADMIN_IM_YEAR.ToString();
                    eMailModel.EmailCC = ConfigurationManager.AppSettings["EMAIL_CC"].ToString();
                    eMailModel.AttachedFilePath = ConfigurationManager.AppSettings["QUALITY_QM_LETTER_SQC"].ToString() + qmLetterModel.LETTER_ID + ".pdf";
                    eMailModel.EmailDate = DateTime.Now.ToString();
                    eMailModel.RecepientName = sqcDetails.ADMIN_QC_NAME;
                    eMailModel.RecepientState = dbContext.MASTER_STATE.Where(c => c.MAST_STATE_CODE == sqcDetails.MAST_STATE_CODE).Select(c => c.MAST_STATE_NAME).FirstOrDefault();
                    eMailModel.RecepientDesignation = dbContext.MASTER_DESIGNATION.Where(c => c.MAST_DESIG_CODE == sqcDetails.ADMIN_QC_DESG).Select(c => c.MAST_DESIG_NAME).FirstOrDefault();
                    eMailModel.RecepientAddress = sqcDetails.ADMIN_QC_ADDRESS1 + (sqcDetails.ADMIN_QC_ADDRESS2 != null ? (", " + sqcDetails.ADMIN_QC_ADDRESS2) : "");

                }
                else if (qmLetterModel.QC_TYPE.Equals("I")) //NQM
                {
                    //Extract all relative Details 
                    scheduleDetails = dbContext.QUALITY_QM_SCHEDULE.Find(qmLetterModel.SCHEDULE_CODE);
                    letterDetails = dbContext.QUALITY_QM_LETTER.Where(c => c.ADMIN_SCHEDULE_CODE == qmLetterModel.SCHEDULE_CODE).OrderByDescending(z => z.LETTER_ID).First();
                    qmDetails = dbContext.ADMIN_QUALITY_MONITORS.Find(scheduleDetails.ADMIN_QM_CODE);

                    //assign letter details
                    qmLetterModel.LETTER_ID = letterDetails.LETTER_ID;

                    eMailModel.RoleCode = 6;
                    eMailModel.EmailRecepient = qmDetails.ADMIN_QM_EMAIL;
                    DateTime dtDate = new DateTime(scheduleDetails.ADMIN_IM_YEAR, scheduleDetails.ADMIN_IM_MONTH, 1);
                    qmLetterModel.MONTH_TEXT = dtDate.ToString("MMMM");
                    eMailModel.EmailSubject = "Inspection Schedule of " + qmLetterModel.MONTH_TEXT + ", " + scheduleDetails.ADMIN_IM_YEAR.ToString();
                    eMailModel.EmailCC = ConfigurationManager.AppSettings["EMAIL_CC"].ToString();
                    eMailModel.AttachedFilePath = ConfigurationManager.AppSettings["QUALITY_QM_LETTER_NQM"].ToString() + qmLetterModel.LETTER_ID + ".pdf";
                    eMailModel.EmailDate = DateTime.Now.ToString();
                    eMailModel.RecepientName = qmDetails.ADMIN_QM_FNAME + (qmDetails.ADMIN_QM_MNAME != null ? (" " + qmDetails.ADMIN_QM_MNAME) : "") + (qmDetails.ADMIN_QM_LNAME != null ? (" " + qmDetails.ADMIN_QM_LNAME) : "");
                    eMailModel.RecepientState = dbContext.MASTER_STATE.Where(c => c.MAST_STATE_CODE == qmDetails.MAST_STATE_CODE).Select(c => c.MAST_STATE_NAME).FirstOrDefault();
                    eMailModel.RecepientDesignation = dbContext.MASTER_DESIGNATION.Where(c => c.MAST_DESIG_CODE == qmDetails.ADMIN_QM_DESG).Select(c => c.MAST_DESIG_NAME).FirstOrDefault();
                    eMailModel.RecepientAddress = qmDetails.ADMIN_QM_ADDRESS1 + (qmDetails.ADMIN_QM_ADDRESS2 != null ? (", " + qmDetails.ADMIN_QM_ADDRESS2) : "");
                }

                string headerPath = System.Web.Hosting.HostingEnvironment.MapPath(@"~/Content/images/Header-e-gov-6.png");

                // Changed by Srishti on 13-03-2023
                //var mailMessage = new MvcMailMessage
                var mailMessage = new MailMessage
                {
                    Subject = eMailModel.EmailSubject
                };
                mailMessage.To.Add(eMailModel.EmailRecepient);
                if (eMailModel.EmailCC != "")
                {
                    mailMessage.CC.Add(eMailModel.EmailCC);
                }

                if (ConfigurationManager.AppSettings["EMAIL_CC1"] != null && ConfigurationManager.AppSettings["EMAIL_CC1"].ToString() != "")
                {
                    mailMessage.CC.Add(ConfigurationManager.AppSettings["EMAIL_CC1"].ToString());
                }

                if (eMailModel.AttachedFilePath != "")
                {
                    mailMessage.Attachments.Add(new Attachment(eMailModel.AttachedFilePath));
                }

                var resources = new Dictionary<string, string>();
                resources["logo"] = headerPath;

                IUserMailer iuserMailer = new UserMailer();
                var mailMsg = iuserMailer.SendMailCustomFunc(eMailModel, mailMessage, "~/Views/UserMailer/SendMailCustomFunc.cshtml", resources);
                if (mailMsg != null)
                {
                    letterDetails.MAIL_DELIVERY_STATUS = true;
                    letterDetails.MAIL_DELIVERY_DATE = Convert.ToDateTime(eMailModel.EmailDate);
                    dbContext.Entry(letterDetails).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                }
                return mailMessage;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoringBAL.SendMailCustomFunc()");
                throw new Exception("Error while Sending Letter");
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        /// <summary>
        /// Assign Model properties to Entity Model
        /// </summary>
        /// <param name="qmScheduleModel"></param>
        /// <returns></returns>
        /// 

        public string QMCreateScheduleBAL(QMScheduleViewModel qmScheduleModel)
        {
            dbContext = new PMGSYEntities();
            qualityDAL = new QualityMonitoringDAL();
            QUALITY_QM_SCHEDULE objQualityQMSchedule = new QUALITY_QM_SCHEDULE();
            try
            {

                // Added on 09 Feb 2021 as per suggestion by Srinivasa Sir.
                //if (dbContext.QUALITY_QM_SCHEDULE.Where(m => m.ADMIN_QM_CODE == qmScheduleModel.ADMIN_QM_CODE && m.ADMIN_IM_MONTH == qmScheduleModel.ADMIN_IM_MONTH && m.ADMIN_IM_YEAR == qmScheduleModel.ADMIN_IM_YEAR).Any())
                //{
                //    return "Schedule is already created for selected Month and Year against selected Monitor.";
                //}
                //Above validation is commented on 26 March 2021 as directed by Srinivasa Sir.
                string[] SplittedArr = qmScheduleModel.ASSIGNED_DISTRICT_LIST.Split(',');

                //if (PMGSYSession.Current.RoleCode == 5)      //CQC for NQM
                if (PMGSYSession.Current.RoleCode == 5 || PMGSYSession.Current.RoleCode == 9)      //CQC for NQM
                {
                    if (SplittedArr.Length > 3)
                    {
                        return "Maximum 3 Districts can be visited in a schedule.";
                    }
                }
                else if (PMGSYSession.Current.RoleCode == 8 || PMGSYSession.Current.RoleCode == 69 || PMGSYSession.Current.RoleCode == 48)      //SQC for SQM
                {
                    if (SplittedArr.Length > 2)
                    {
                        return "Maximum 2 Districts can be visited in a schedule.";
                    }

                    ///Added for RCPLWE Scheme on 20JUL2018
                    if (PMGSYSession.Current.PMGSYScheme == 3 && SplittedArr.Length == 1)
                    {
                        int districtCode = Convert.ToInt32(SplittedArr[0]);
                        bool IsRCPLWEWorksExist = dbContext.IMS_SANCTIONED_PROJECTS.Any(x => x.MAST_DISTRICT_CODE == districtCode && x.MAST_PMGSY_SCHEME == 3);
                        if (IsRCPLWEWorksExist == false)
                        {
                            return "No Rcplwe works found for selected district, cannot create schedule.";
                        }
                    }
                }


                List<int> lstDistricts = new List<int>();
                foreach (var dist in SplittedArr)
                {
                    lstDistricts.Add(Convert.ToInt32(dist));
                }

                string qmType = string.Empty;
                //if (PMGSYSession.Current.RoleCode == 5) //CQC
                if (PMGSYSession.Current.RoleCode == 5 || PMGSYSession.Current.RoleCode == 9) //CQC
                {
                    qmType = "I";
                }
                else if (PMGSYSession.Current.RoleCode == 8 || PMGSYSession.Current.RoleCode == 69 || PMGSYSession.Current.RoleCode == 48) //SQC
                {
                    qmType = "S";
                }

                // Validation removed for SQM 
                // FOr SQM - Same District can be allocated to differnet SQM in same Month
                // For NQM - Same District can't be allocated to two different monitors in same month (This Validation is removed on 19/06/2014 - Anand Singh's request)
                //if (qmType.Equals("I"))
                //{
                //    //First Check, is Schduled district already added for month & Year to any of the Monitors
                //    var isAlreadySchExist = (from qqs in dbContext.QUALITY_QM_SCHEDULE
                //                             join aqm in dbContext.ADMIN_QUALITY_MONITORS on qqs.ADMIN_QM_CODE equals aqm.ADMIN_QM_CODE
                //                             where aqm.ADMIN_QM_TYPE == qmType &&
                //                             qqs.ADMIN_IM_MONTH == qmScheduleModel.ADMIN_IM_MONTH &&
                //                             qqs.ADMIN_IM_YEAR == qmScheduleModel.ADMIN_IM_YEAR &&
                //                             qqs.MAST_STATE_CODE == qmScheduleModel.MAST_STATE_CODE
                //                             select qqs).ToList();

                //    foreach (var item in isAlreadySchExist)
                //    {
                //        if (lstDistricts.Contains(item.MAST_DISTRICT_CODE) || lstDistricts.Contains(Convert.ToInt32(item.MAST_DISTRICT_CODE2)) || lstDistricts.Contains(Convert.ToInt32(item.MAST_DISTRICT_CODE3)))
                //        {
                //            return "Selected District is already assigned for current month & year to one of the monitors, please select other district";
                //        }
                //    }
                //}
                //else if (qmType.Equals("S"))
                //{
                //First Check, is Schduled district already added for month & Year to particular Monitor
                var isAlreadySchExist = (from qqs in dbContext.QUALITY_QM_SCHEDULE
                                         join aqm in dbContext.ADMIN_QUALITY_MONITORS on qqs.ADMIN_QM_CODE equals aqm.ADMIN_QM_CODE
                                         where aqm.ADMIN_QM_TYPE == qmType &&
                                         qqs.ADMIN_IM_MONTH == qmScheduleModel.ADMIN_IM_MONTH &&
                                         qqs.ADMIN_IM_YEAR == qmScheduleModel.ADMIN_IM_YEAR &&
                                         qqs.MAST_STATE_CODE == qmScheduleModel.MAST_STATE_CODE &&
                                         qqs.ADMIN_QM_CODE == qmScheduleModel.ADMIN_QM_CODE     //Extra contion - same monitor can't have same district two times in same month&year
                                         select qqs).ToList();

                foreach (var item in isAlreadySchExist)
                {
                    if (lstDistricts.Contains(item.MAST_DISTRICT_CODE) || lstDistricts.Contains(Convert.ToInt32(item.MAST_DISTRICT_CODE2)) || lstDistricts.Contains(Convert.ToInt32(item.MAST_DISTRICT_CODE3)))
                    {
                        return "Selected District is already assigned for current month & year to selected monitor, please select other district";
                    }
                }
                //}


                objQualityQMSchedule.MAST_DISTRICT_CODE = Convert.ToInt32(SplittedArr[0]);
                if (SplittedArr.Length > 1)
                {
                    objQualityQMSchedule.MAST_DISTRICT_CODE2 = Convert.ToInt32(SplittedArr[1]);
                }

                if (SplittedArr.Length > 2)
                {
                    objQualityQMSchedule.MAST_DISTRICT_CODE3 = Convert.ToInt32(SplittedArr[2]);
                }



                objQualityQMSchedule.ADMIN_IS_ENQUIRY = qmScheduleModel.ADMIN_IS_ENQUIRY;
                objQualityQMSchedule.ADMIN_QM_CODE = qmScheduleModel.ADMIN_QM_CODE;
                objQualityQMSchedule.ADMIN_IM_MONTH = qmScheduleModel.ADMIN_IM_MONTH;
                objQualityQMSchedule.ADMIN_IM_YEAR = qmScheduleModel.ADMIN_IM_YEAR;
                objQualityQMSchedule.SCHEDULE_DATE = DateTime.Now;
                objQualityQMSchedule.MAST_STATE_CODE = qmScheduleModel.MAST_STATE_CODE;


                //if (PMGSYSession.Current.RoleCode == 5)
                if (PMGSYSession.Current.RoleCode == 5 || PMGSYSession.Current.RoleCode == 9)
                    objQualityQMSchedule.FINALIZE_FLAG = "CQC";
                else if (PMGSYSession.Current.RoleCode == 8 || PMGSYSession.Current.RoleCode == 69 || PMGSYSession.Current.RoleCode == 48)
                    objQualityQMSchedule.FINALIZE_FLAG = "SQC";

                objQualityQMSchedule.INSP_STATUS_FLAG = "S";

                return qualityDAL.QMCreateScheduleDAL(objQualityQMSchedule);


            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoring.QMCreateScheduleBAL()");
                return "An Error Occurred While Processing Your Request.";
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// Get District for schedule creation
        /// </summary>
        /// <param name="selectedState"></param>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public List<SelectListItem> GetDistrictForScheduleCreationBAL(int selectedState, int month, int year)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.GetDistrictForScheduleCreationDAL(selectedState, month, year);
        }

        /// <summary>
        /// Monitor Details
        /// </summary>
        /// <param name="monitorCode"></param>
        /// <returns></returns>
        public MasterAdminQualityMonitorViewModel MonitorDetailsBAL(int monitorCode)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.MonitorDetailsDAL(monitorCode);
        }

        /// <summary>
        /// Get Unassigned Districts for Particular Schedule to Assign them befor finalization of Schedule
        /// </summary>
        /// <param name="scheduleCode"></param>
        /// <param name="isAssignedDistricts"></param>
        /// <returns></returns>
        public List<SelectListItem> GetScheduledDistrictListBAL(int scheduleCode, bool isAssignedDistricts)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.GetScheduledDistrictListDAL(scheduleCode, isAssignedDistricts);
        }

        /// <summary>
        /// Assign/add District in a schedule
        /// </summary>
        /// <param name="qmCQCAddDistrict"></param>
        /// <returns></returns>
        public string CQCAddDistrictsBAL(QMCQCAddDistrictModel qmCQCAddDistrict)
        {
            dbContext = new PMGSYEntities();
            qualityDAL = new QualityMonitoringDAL();
            try
            {
                //First Check is Schduled district already added for month & Year for particular Monitor
                var query = (from qqs in dbContext.QUALITY_QM_SCHEDULE
                             where qqs.ADMIN_SCHEDULE_CODE == qmCQCAddDistrict.ADMIN_SCHEDULE_CODE
                             select qqs).First();

                //var isAlreadySchExist = (from qqs in dbContext.QUALITY_QM_SCHEDULE
                //                         where //qqs.ADMIN_QM_CODE == query.ADMIN_QM_CODE &&
                //                         qqs.ADMIN_IM_MONTH == query.ADMIN_IM_MONTH &&
                //                         qqs.ADMIN_IM_YEAR == query.ADMIN_IM_YEAR &&
                //                         qqs.MAST_STATE_CODE == query.MAST_STATE_CODE
                //                         select qqs).ToList();

                string qmType = string.Empty;
                if (PMGSYSession.Current.RoleCode == 5) //CQC
                {
                    qmType = "I";
                }
                else if (PMGSYSession.Current.RoleCode == 8) //SQC
                {
                    qmType = "S";
                }

                var isAlreadySchExist = (from qqs in dbContext.QUALITY_QM_SCHEDULE
                                         join aqm in dbContext.ADMIN_QUALITY_MONITORS on qqs.ADMIN_QM_CODE equals aqm.ADMIN_QM_CODE
                                         where aqm.ADMIN_QM_TYPE == qmType &&
                                         qqs.ADMIN_IM_MONTH == query.ADMIN_IM_MONTH &&
                                         qqs.ADMIN_IM_YEAR == query.ADMIN_IM_YEAR &&
                                         qqs.MAST_STATE_CODE == query.MAST_STATE_CODE
                                         select qqs).ToList();

                foreach (var item in isAlreadySchExist)
                {
                    if (qmCQCAddDistrict.MAST_DISTRICT_CODE == item.MAST_DISTRICT_CODE ||
                    qmCQCAddDistrict.MAST_DISTRICT_CODE == item.MAST_DISTRICT_CODE2 ||
                    qmCQCAddDistrict.MAST_DISTRICT_CODE == item.MAST_DISTRICT_CODE3)
                    {
                        return "Selected District is already assigned for current month & year, please select other district";
                    }
                }

                return qualityDAL.CQCAddDistrictsDAL(qmCQCAddDistrict);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return "An Error Occurred While Processing Your Request.";
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// Populate Road LIST to assign it in schedule
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        public Array GetRoadListToAssignBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, string filters, int districtCode, int adminSchCode, int techCode)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.GetRoadListToAssignDAL(page, rows, sidx, sord, out totalRecords, filters, districtCode, adminSchCode, techCode);
        }

        /// <summary>
        /// Save Details for new road in Schedule
        /// </summary>
        /// <param name="prRoadCode"></param>
        /// <param name="adminSchCode"></param>
        /// <returns></returns>
        //public string QMAssignRoadsBAL(int prRoadCode, int adminSchCode, string isEnquiry)
        //{
        //    qualityDAL = new QualityMonitoringDAL();
        //    try
        //    {
        //        QUALITY_QM_SCHEDULE_DETAILS qmAssignRoadsModel = new QUALITY_QM_SCHEDULE_DETAILS();
        //        qmAssignRoadsModel.ADMIN_SCHEDULE_CODE = adminSchCode;
        //        qmAssignRoadsModel.IMS_PR_ROAD_CODE = prRoadCode;
        //        qmAssignRoadsModel.DEVICE_FLAG = "N";

        //        //Conditionallly set Flags as per Login Role
        //        if (PMGSYSession.Current.RoleCode == 5)  //For CQC
        //        {
        //            qmAssignRoadsModel.FINALIZE_FLAG = "CQC";
        //            qmAssignRoadsModel.SCHEDULE_ASSIGNED = "C";
        //        }
        //        else if (PMGSYSession.Current.RoleCode == 8)    //For SQC
        //        {
        //            qmAssignRoadsModel.FINALIZE_FLAG = "SQC";
        //            qmAssignRoadsModel.SCHEDULE_ASSIGNED = "S";
        //        }
        //        else if (PMGSYSession.Current.RoleCode == 6)    //For NQM
        //        {
        //            qmAssignRoadsModel.FINALIZE_FLAG = "NQM";
        //            qmAssignRoadsModel.SCHEDULE_ASSIGNED = "N";
        //        }
        //        else if (PMGSYSession.Current.RoleCode == 7)    //For SQM
        //        {
        //            qmAssignRoadsModel.FINALIZE_FLAG = "SQM";
        //            qmAssignRoadsModel.SCHEDULE_ASSIGNED = "M";
        //        }
        //        else if (PMGSYSession.Current.RoleCode == 22)    //For PIU
        //        {
        //            qmAssignRoadsModel.FINALIZE_FLAG = "DPIU";
        //            qmAssignRoadsModel.SCHEDULE_ASSIGNED = "D";
        //        }

        //        qmAssignRoadsModel.INSP_STATUS_FLAG = "S";
        //        qmAssignRoadsModel.CQC_FORWARD_FLAG = "N";
        //        qmAssignRoadsModel.IS_SCHEDULE_DOWNLOAD = "N";
        //        qmAssignRoadsModel.TOTAL_IMAGE_COUNT = 10;
        //        qmAssignRoadsModel.ADMIN_IS_ENQUIRY = isEnquiry;

        //        return qualityDAL.QMAssignRoadsDAL(qmAssignRoadsModel);
        //    }
        //    catch (Exception ex)
        //    {
        //        Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
        //        return "An Error Occurred While Processing Your Request.";
        //    }
        //}

        /// <summary>
        /// Save Details for new Works in Schedule
        /// </summary>
        /// <param name="prRoadCode"></param>
        /// <param name="adminSchCode"></param>
        /// <returns></returns>
        public string QMAssignWorksBAL(string[] arrWorks, int adminSchCode, string[] arrEnquiry)
        {
            qualityDAL = new QualityMonitoringDAL();
            try
            {
                List<QUALITY_QM_SCHEDULE_DETAILS> lstSchedule = new List<QUALITY_QM_SCHEDULE_DETAILS>();

                foreach (var workItem in arrWorks)
                {
                    QUALITY_QM_SCHEDULE_DETAILS qmAssignRoadsModel = new QUALITY_QM_SCHEDULE_DETAILS();
                    qmAssignRoadsModel.ADMIN_SCHEDULE_CODE = adminSchCode;
                    qmAssignRoadsModel.DEVICE_FLAG = "N";
                    //Conditionallly set Flags as per Login Role
                    if (PMGSYSession.Current.RoleCode == 5 || PMGSYSession.Current.RoleCode == 9)  //For CQC
                    {
                        qmAssignRoadsModel.FINALIZE_FLAG = "CQC";
                        qmAssignRoadsModel.SCHEDULE_ASSIGNED = "C";
                    }
                    else if (PMGSYSession.Current.RoleCode == 8 || PMGSYSession.Current.RoleCode == 48 || PMGSYSession.Current.RoleCode == 69)    //For SQC
                    {
                        qmAssignRoadsModel.FINALIZE_FLAG = "SQC";
                        qmAssignRoadsModel.SCHEDULE_ASSIGNED = "S";
                    }
                    else if (PMGSYSession.Current.RoleCode == 6)    //For NQM
                    {
                        qmAssignRoadsModel.FINALIZE_FLAG = "NQM";
                        qmAssignRoadsModel.SCHEDULE_ASSIGNED = "N";
                    }
                    else if (PMGSYSession.Current.RoleCode == 7)    //For SQM
                    {
                        qmAssignRoadsModel.FINALIZE_FLAG = "SQM";
                        qmAssignRoadsModel.SCHEDULE_ASSIGNED = "M";
                    }
                    else if (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38 || PMGSYSession.Current.RoleCode == 54)    //For PIU & PIUOA
                    {
                        qmAssignRoadsModel.FINALIZE_FLAG = "DPIU";
                        qmAssignRoadsModel.SCHEDULE_ASSIGNED = "D";
                    }

                    qmAssignRoadsModel.INSP_STATUS_FLAG = "S";
                    qmAssignRoadsModel.CQC_FORWARD_FLAG = "N";
                    qmAssignRoadsModel.IS_SCHEDULE_DOWNLOAD = "N";
                    qmAssignRoadsModel.TOTAL_IMAGE_COUNT = 10;

                    qmAssignRoadsModel.IMS_PR_ROAD_CODE = Convert.ToInt32(workItem);
                    qmAssignRoadsModel.ADMIN_IS_ENQUIRY = arrEnquiry != null ? (arrEnquiry.Contains(workItem)) ? "Y" : "N" : "N";

                    lstSchedule.Add(qmAssignRoadsModel);
                }

                return qualityDAL.QMAssignWorksDAL(lstSchedule);
            }
            catch (Exception ex)
            {
                return "An Error Occurred While Processing Your Request.";
            }
        }

        public Array GetRoadPhysicalProgressList(int page, int rows, string sidx, string sord, out long totalRecords, int proposalCode)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.GetRoadPhysicalProgressList(page, rows, sidx, sord, out totalRecords, proposalCode);
        }

        /// <summary>
        /// View District Wise Schedule details for Finalization
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="filters"></param>
        /// <param name="districtCode"></param>
        /// <param name="sanctionYear"></param>
        /// <param name="rdStatus"></param>
        /// <param name="adminSchCode"></param>
        /// <returns></returns>
        public Array QMViewScheduleDetailsBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int adminSchCode)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.QMViewScheduleDetailsDAL(page, rows, sidx, sord, out totalRecords, adminSchCode);
        }

        public string QMDeleteSchRoadsBAL(int prRoadCode, int scheduleCode)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.QMDeleteSchRoadsDAL(prRoadCode, scheduleCode);
        }


        public string CQCDeleteDistrictBAL(int districtCode, int scheduleCode)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.CQCDeleteDistrictDAL(districtCode, scheduleCode);
        }

        public string FinalizeDistrictsBAL(int scheduleCode)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.FinalizeDistrictsDAL(scheduleCode);
        }

        public string FinalizeRoadBAL(int prRoadCode, int scheduleCode, bool isFinalizeAllRoads)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.FinalizeRoadDAL(prRoadCode, scheduleCode, isFinalizeAllRoads);
        }

        public string ForwardScheduleBAL(int scheduleCode)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.ForwardScheduleDAL(scheduleCode);
        }

        public string UnlockScheduleBAL(int scheduleCode)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.UnlockScheduleDAL(scheduleCode);
        }

        public Array QMViewInspectionDetailsBAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords,
                                                int stateCode, int monitorCode, int fromMonth, int fromYear,
                                                int toMonth, int toYear, int schemeType, string roadStatus, string roadOrBridge, string gradeType, string eFormStatusType)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.QMViewInspectionDetailsDAL(page, rows, sidx, sord, out totalRecords,
                                                       stateCode, monitorCode, fromMonth, fromYear, toMonth, toYear, schemeType, roadStatus, roadOrBridge, gradeType, eFormStatusType);
        }


        public Array QMViewInspectionDetails2TierCQCBAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords,
                                                int stateCode, int monitorCode, int fromMonth, int fromYear,
                                                int toMonth, int toYear, string ROAD_STATUS, int schemeType, string roadOrBridge, string gradeType, string eFormStatus)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.QMViewInspectionDetails2TierCQCDAL(page, rows, sidx, sord, out totalRecords,
                                                       stateCode, monitorCode, fromMonth, fromYear, toMonth, toYear, ROAD_STATUS, schemeType, roadOrBridge, gradeType, eFormStatus);
        }


        public Array QMViewATRDetailsBAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords,
                                                int stateCode, int monitorCode, int fromMonth, int fromYear,
                                                int toMonth, int toYear, string atrStatus, string rdStatus)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.QMViewATRDetailsDAL(page, rows, sidx, sord, out totalRecords,
                                                       stateCode, monitorCode, fromMonth, fromYear, toMonth, toYear, atrStatus, rdStatus);
        }


        public Array QMViewBulkATRListBAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int duration)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.QMViewBulkATRListDAL(page, rows, sidx, sord, out totalRecords, stateCode, duration);
        }


        public Array GetPrevScheduleListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int monitorId, int prevMonth, int prevYear, bool is3Tier)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.GetPrevScheduleListDAL(page, rows, sidx, sord, out totalRecords, monitorId, prevMonth, prevYear, is3Tier);
        }



        public QMFillObservationModel QMGradingCorrectionBAL(int obsId)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.QMGradingCorrectionDAL(obsId);
        }

        /// <summary>
        /// Post to save Corrections
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        public string QMGradingCorrectionBAL(FormCollection formCollection)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.QMGradingCorrectionDAL(formCollection);
        }


        public string QMDeleteObservationBAL(int obsId)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.QMDeleteObservationDAL(obsId);
        }


        public QMFillObservationModel QMObservationDetailsBAL(int obsId)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.QMObservationDetailsDAL(obsId);
        }

        public QMFillObservationModel QMObservationDetailsATRBAL(int obsId)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.QMObservationDetailsATRDAL(obsId);
        }

        public QMFillObservationModel QMObservationDetails2TierCQCBAL(int obsId)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.QMObservationDetails2TierCQCDAL(obsId);
        }

        public string QMDeleteATRDetailsBAL(int obsId, int atrId)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.QMDeleteATRDetailsDAL(obsId, atrId);
        }

        /// <summary>
        /// Populate 2 Tier Schedule CQC LIST
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        public Array Get2TierScheduleListCQCBAL(int state, int month, int year, int page, int rows, string sidx, string sord, out Int32 totalRecords, string filters)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.Get2TierScheduleListCQCDAL(state, month, year, page, rows, sidx, sord, out totalRecords, filters);
        }

        public Array ListQualityMonitor(string qmTypeName, int stateCode, int districtCode, string isEmpanelled, string filters, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.ListQualityMonitor(qmTypeName, stateCode, districtCode, isEmpanelled, filters, page, rows, sidx, sord, out totalRecords);
        }
        #endregion


        #region SQC 3-Tier

        public Array QMViewInspectionDetailsSQCPIUBAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords,
                                                int stateCode, int monitorCode, int fromMonth, int fromYear,
                                                int toMonth, int toYear, string qmType, int schemeType, string roadStatus, string roadOrBridge, string gradeType, string eFormStatus)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.QMViewInspectionDetailsSQCPIUDAL(page, rows, sidx, sord, out totalRecords,
                                                       stateCode, monitorCode, fromMonth, fromYear, toMonth, toYear, qmType, schemeType, roadStatus, roadOrBridge, gradeType, eFormStatus);
        }

        public Array GetSqc3TierScheduleListBAL(int month, int year, int page, int rows, string sidx, string sord, out Int32 totalRecords, string filters)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.GetSqc3TierScheduleListDAL(month, year, page, rows, sidx, sord, out totalRecords, filters);
        }

        public List<SelectListItem> PopulateMonitorsBAL(int state, int inspMonth, int inspYear, int districtCode, string qmType)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.PopulateMonitorsDAL(state, inspMonth, inspYear, districtCode, qmType);
        }

        public QMFillObservationModel QMObservationDetails3TierSQCBAL(int obsId)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.QMObservationDetails3TierSQCDAL(obsId);
        }
        #endregion


        #region PIU

        public Array GetPIU3TierScheduleListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, string filters)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.GetPIU3TierScheduleListDAL(page, rows, sidx, sord, out totalRecords, filters);
        }

        public Array GetPIU2TierScheduleListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, string filters)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.GetPIU2TierScheduleListDAL(page, rows, sidx, sord, out totalRecords, filters);
        }

        #endregion


        #region Monitors

        public Array GetMonitorsCurrScheduleListBAL(int month, int year, int page, int rows, string sidx, string sord, out Int32 totalRecords)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.GetMonitorsCurrScheduleListDAL(month, year, page, rows, sidx, sord, out totalRecords);
        }


        public Array QMMonitorInspListBAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords,
                                                int fromMonth, int fromYear,
                                                int toMonth, int toYear)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.QMMonitorInspListDAL(page, rows, sidx, sord, out totalRecords,
                                                   fromMonth, fromYear, toMonth, toYear);
        }

        public Array GetMonitorsScheduledRoadListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int inspMonth, int inspYear)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.GetMonitorsScheduledRoadListDAL(page, rows, sidx, sord, out totalRecords, inspMonth, inspYear);
        }

        public QMFillObservationModel QMFillObservationsBAL(int adminSchCode, int prRoadCode, string roadStatus)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.QMFillObservationsDAL(adminSchCode, prRoadCode, roadStatus);
        }


        public string QMSaveObservationsBAL(FormCollection formCollection)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.QMSaveObservationsDAL(formCollection);
        }


        public QMTourViewModel GetTourDetailsBAL(int scheduleCode)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.GetTourDetailsDAL(scheduleCode);
        }

        public string SaveTourDetailsBAL(QMTourViewModel model)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.SaveTourDetailsDAL(model);
        }

        public Array GetTourListBAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int state, int qmCode, int frmMonth, int frmYear, int toMonth, int toYear)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.GetTourListDAL(page, rows, sidx, sord, out totalRecords, state, qmCode, frmMonth, frmYear, toMonth, toYear);
        }

        public QMTourViewModel GetTourDetailsForUpdateBAL(int tourId)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.GetTourDetailsForUpdateDAL(tourId);
        }

        public string UpdateTourDetailsBAL(QMTourViewModel model)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.UpdateTourDetailsDAL(model);
        }

        public string DeleteTourDetailsBAL(int tourId)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.DeleteTourDetailsDAL(tourId);
        }


        public string FinalizeTourDetailsBAL(int tourId, string flagValue)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.FinalizeTourDetailsDAL(tourId, flagValue);
        }
        #endregion


        #region Upload File Details

        public Array GetFilesListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int obsId)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.GetFilesListDAL(page, rows, sidx, sord, out totalRecords, obsId);
        }

        public string GetStartEndLatLongBAL(int obsId)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.GetStartEndLatLongDAL(obsId);
        }

        public string GetLatLongBAL(int obsId)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.GetLatLongDAL(obsId);
        }

        public Array GetPDFFilesListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int obsId, int QM_ATR_ID)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.GetPDFFilesListDAL(page, rows, sidx, sord, out totalRecords, obsId, QM_ATR_ID);
        }


        public string AddATRDetailsBAL(List<PMGSY.Models.QualityMonitoring.FileUploadViewModel> lstFileUploadViewModel)
        {
            qualityDAL = new QualityMonitoringDAL();
            dbContext = new PMGSYEntities();
            try
            {
                // Image Upload
                QUALITY_ATR_FILE qualityATRFile = new QUALITY_ATR_FILE();
                foreach (PMGSY.Models.QualityMonitoring.FileUploadViewModel model in lstFileUploadViewModel)
                {
                    qualityATRFile.QM_OBSERVATION_ID = model.QM_OBSERVATION_ID;
                    qualityATRFile.ATR_FILE_NAME = model.name;
                    qualityATRFile.ATR_ENTRY_DATE = DateTime.Now;
                    qualityATRFile.ATR_REGRADE_STATUS = "U";   //Submitted
                    qualityATRFile.ATR_REGRADE_REMARKS = null;
                    qualityATRFile.ATR_IS_DELETED = "N";

                    //Added By Abhishek Kamble 30-nov-2013
                    qualityATRFile.USERID = PMGSYSession.Current.UserId;
                    qualityATRFile.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }

                return qualityDAL.AddATRDetailsDAL(qualityATRFile);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "QualityMonitoring.AddATRDetailsBAL()");
                return ("Error Occurred While Processing Request.");
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public string UpdateImageDetailsBAL(PMGSY.Models.QualityMonitoring.FileUploadViewModel fileuploadViewModel)
        {
            qualityDAL = new QualityMonitoringDAL();
            QUALITY_QM_INSPECTION_FILE qm_inspection_files = new QUALITY_QM_INSPECTION_FILE();
            qm_inspection_files.QM_FILE_ID = Convert.ToInt32(fileuploadViewModel.QM_FILE_ID);
            qm_inspection_files.QM_FILE_DESCR = fileuploadViewModel.Image_Description;

            return qualityDAL.UpdateImageDetailsDAL(qm_inspection_files);
        }


        public string DeleteFileDetails(int QM_FILE_ID)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.DeleteFileDetailsDAL(QM_FILE_ID);
        }



        public string AddFileUploadDetailsBAL(List<PMGSY.Models.QualityMonitoring.FileUploadViewModel> lstFileUploadViewModel)
        {
            qualityDAL = new QualityMonitoringDAL();
            dbContext = new PMGSYEntities();
            //List<QUALITY_QM_INSPECTION_FILE> lst_qm_inspection_files = new List<QUALITY_QM_INSPECTION_FILE>();
            try
            {
                // Image Upload
                QUALITY_QM_INSPECTION_FILE qualityQMInspectionFile = new QUALITY_QM_INSPECTION_FILE();
                foreach (FileUploadViewModel model in lstFileUploadViewModel)
                {
                    qualityQMInspectionFile.ADMIN_SCHEDULE_CODE = model.ADMIN_SCHEDULE_CODE;
                    qualityQMInspectionFile.IMS_PR_ROAD_CODE = model.IMS_PR_ROAD_CODE;
                    //qualityQMInspectionFile.QM_OBSERVATION_ID = dbContext.QUALITY_QM_OBSERVATION_MASTER.Where(a => a.ADMIN_SCHEDULE_CODE == model.ADMIN_SCHEDULE_CODE && a.IMS_PR_ROAD_CODE == model.IMS_PR_ROAD_CODE).Select(a => a.QM_OBSERVATION_ID).First();
                    qualityQMInspectionFile.QM_OBSERVATION_ID = model.QM_OBSERVATION_ID;
                    qualityQMInspectionFile.QM_FILE_NAME = model.name;
                    qualityQMInspectionFile.QM_FILE_DESCR = model.Image_Description;
                    qualityQMInspectionFile.QM_FILE_UPLOAD_DATE = DateTime.Now;

                    //Added By Abhishek Kamble 30-nov-2013
                    qualityQMInspectionFile.USERID = PMGSYSession.Current.UserId;
                    qualityQMInspectionFile.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }

                return qualityDAL.AddFileUploadDetailsDAL(qualityQMInspectionFile);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return ("Error Occurred While Processing Request.");
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        #endregion

        #region QCR Part-I PDF by Srishti and Vikki

        public Array GetExecutionList(int yearCode, int districtCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                qualityDAL = new QualityMonitoringDAL();
                return qualityDAL.GetExecutionList(yearCode, districtCode, page, rows, sidx, sord, out totalRecords);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
        }



        public bool AddQCRdDetails(AddUploadQCRDetailsModel model, out String IsValid)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.AddQCRdDetails(model, out IsValid);
        }

        public Array GetQCRList(int roadCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                qualityDAL = new QualityMonitoringDAL();
                return qualityDAL.GetQCRList(roadCode, page, rows, sidx, sord, out totalRecords);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
        }

        #endregion

        #region View Uploaded QCR PDF

        public Array GetExecutionListView(int yearCode, int districtCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                qualityDAL = new QualityMonitoringDAL();
                return qualityDAL.GetExecutionListView(yearCode, districtCode, page, rows, sidx, sord, out totalRecords);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
        }

        public Array GetQCRListToView(int roadCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                qualityDAL = new QualityMonitoringDAL();
                return qualityDAL.GetQCRListToView(roadCode, page, rows, sidx, sord, out totalRecords);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
        }

        #endregion

        #region PDF Upload By Monitor

        /// <summary>
        /// Get the PDF Files List
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        /// <returns></returns>
        public Array GetInspReportFilesListDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int obsId, string isATRPage)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.GetInspReportFilesListDAL(page, rows, sidx, sord, out totalRecords, obsId,  isATRPage);
        }


        /// <summary>
        /// Validates the PDF File
        /// </summary>
        /// <param name="FileSize"></param>
        /// <param name="FileExtension"></param>
        /// <returns></returns>
        public string ValidatePDFFile(int FileSize, string FileExtension)
        {
            if (FileExtension.ToUpper() != ".PDF")
            {
                return "File is not PDF File";
            }
            if (FileSize > Convert.ToInt32(ConfigurationManager.AppSettings["PROPOSAL_PDF_FILE_MAX_SIZE"]))
            {
                return "File Size Exceed the Maximum File Limit";
            }

            return string.Empty;
        }


        public string AddPdfUploadDetailsBAL(List<PMGSY.Models.QualityMonitoring.FileUploadViewModel> lstFileUploadViewModel)
        {
            qualityDAL = new QualityMonitoringDAL();
            dbContext = new PMGSYEntities();
            try
            {
                // Image Upload
                QUALITY_INSPECTION_REPORT_FILE qualityQMInspectionFile = new QUALITY_INSPECTION_REPORT_FILE();
                foreach (FileUploadViewModel model in lstFileUploadViewModel)
                {
                    qualityQMInspectionFile.QM_OBSERVATION_ID = model.QM_OBSERVATION_ID;
                    qualityQMInspectionFile.FILE_NAME = model.name;
                    qualityQMInspectionFile.FILE_TYPE = "I";
                    qualityQMInspectionFile.FILE_DESCRIPTION = model.PdfDescription;
                    qualityQMInspectionFile.FILE_UPLOAD_DATE = DateTime.Now;

                    //Added By Abhishek Kamble 30-nov-2013
                    qualityQMInspectionFile.USERID = PMGSYSession.Current.UserId;
                    qualityQMInspectionFile.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }

                return qualityDAL.AddPdfUploadDetailsDAL(qualityQMInspectionFile);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return ("Error Occurred While Processing Request.");
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        /// <summary>
        /// Update the PDF File Details
        /// </summary>
        /// <param name="fileuploadViewModel"></param>
        /// <returns></returns>
        public string UpdateInspPDFDetailsBAL(FileUploadViewModel fileuploadViewModel)
        {
            try
            {
                qualityDAL = new QualityMonitoringDAL();
                QUALITY_INSPECTION_REPORT_FILE qm_files = new QUALITY_INSPECTION_REPORT_FILE();

                qm_files.FILE_ID = Convert.ToInt32(fileuploadViewModel.QM_FILE_ID);
                qm_files.QM_OBSERVATION_ID = fileuploadViewModel.QM_OBSERVATION_ID;
                qm_files.FILE_TYPE = "I";
                qm_files.FILE_DESCRIPTION = fileuploadViewModel.PdfDescription;

                return qualityDAL.UpdatePDFDetailsDAL(qm_files);
            }
            catch (Exception ex)
            {
                return ("Error Occurred While Processing Request.");
            }
        }

        public string DeleteInspFileDetailsBAL(int fileId, int obsId, string fileType)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.DeleteInspFileDetailsBAL(fileId, obsId, fileType);
        }

        //added by abhinav pathak on 12-08-2019
        #region upload multiple file by monitor.
        public string AddMultiplePdfUploadDetailsBAL(List<PMGSY.Models.QualityMonitoring.FileUploadViewModel> lstFileUploadViewModel)
        {
            qualityDAL = new QualityMonitoringDAL();
            dbContext = new PMGSYEntities();
            try
            {
                QUALITY_QM_INSPECTION_FILES qualityQMInspectionFile = new QUALITY_QM_INSPECTION_FILES();
                foreach (FileUploadViewModel model in lstFileUploadViewModel)
                {
                    qualityQMInspectionFile.QM_OBSERVATION_ID = model.QM_OBSERVATION_ID;
                    qualityQMInspectionFile.QM_FILE_NAME = model.name;
                    //qualityQMInspectionFile.QM_FILE_TYPE = "P";
                    qualityQMInspectionFile.QM_FILE_DESCR = model.PdfDescription;
                    qualityQMInspectionFile.QM_FILE_UPLOAD_DATE = DateTime.Now;
                    qualityQMInspectionFile.QM_FILES_FINALIZED = "N";
                    qualityQMInspectionFile.IMS_PR_ROAD_CODE = model.IMS_PR_ROAD_CODE;


                    //Added By Abhishek Kamble 30-nov-2013
                    qualityQMInspectionFile.USERID = PMGSYSession.Current.UserId;
                    qualityQMInspectionFile.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }

                return qualityDAL.AddMultiplePdfUploadDetailsDAL(qualityQMInspectionFile);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return ("Error Occurred While Processing Request.");
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array GetInspMultipleFilesListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int obsId)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.GetInspMultipleFilesListDAL(page, rows, sidx, sord, out totalRecords, obsId);
        }

        public bool FinalisePDFDeatilsBAL(int id)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.FinalisePDFDeatilsDAL(id);
        }

        public string UpdateMultipleInspPDFDetailsBAL(FileUploadViewModel fileuploadViewModel)
        {
            try
            {
                qualityDAL = new QualityMonitoringDAL();
                QUALITY_QM_INSPECTION_FILES qm_files = new QUALITY_QM_INSPECTION_FILES();

                qm_files.QM_FILE_ID = Convert.ToInt32(fileuploadViewModel.QM_FILE_ID);
                qm_files.QM_OBSERVATION_ID = fileuploadViewModel.QM_OBSERVATION_ID;
                //qm_files.FILE_TYPE = "I";
                qm_files.QM_FILE_DESCR = fileuploadViewModel.PdfDescription;

                return qualityDAL.UpdateMultiplePDFDetailsDAL(qm_files);
            }
            catch (Exception ex)
            {
                return ("Error Occurred While Processing Request.");
            }
        }

        public string DeleteMultipleInspFileDetailsBAL(int fileId, int obsId)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.DeleteMultipleInspFileDetailsDAL(fileId, obsId);
        }
        #endregion

        #endregion


        #region ATR
        public List<qm_inspection_list_atrr_Result> ATRDetailssBAL(int stateCode, int monitorCode, int fromMonth, int fromYear,
                                                int toMonth, int toYear, string atrStatus, string rdStatus, int PmgsyScheme, string flag)//ATR_Change
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.ATRDetailssDAL(stateCode, monitorCode, fromMonth, fromYear, toMonth, toYear, atrStatus, rdStatus, PmgsyScheme, flag);
        }

        public List<qm_inspection_list_atr_Result> ATRDetailsBAL(int stateCode, int monitorCode, int fromMonth, int fromYear,
                                                int toMonth, int toYear, string atrStatus, string rdStatus, int PmgsyScheme)//ATR_Change
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.ATRDetailsDAL(stateCode, monitorCode, fromMonth, fromYear, toMonth, toYear, atrStatus, rdStatus, PmgsyScheme);
        }

        public string QMATRRegradeBAL(QMATRRegradeModel qmATRRegradeModel)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.QMATRRegradeDAL(qmATRRegradeModel);
        }


        #endregion


        #region 2 tier atr vikky
        public List<qm_inspection_list_2_Tier_atrr_Result> ATR2TierDetailssBAL(int stateCode, int monitorCode, int fromMonth, int fromYear,
                                               int toMonth, int toYear, string atrStatus, string rdStatus, int PmgsyScheme, string flag)//ATR_Change
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.ATR2TierDetailssDAL(stateCode, monitorCode, fromMonth, fromYear, toMonth, toYear, atrStatus, rdStatus, PmgsyScheme, flag);
        }


        public Array Get2TierPDFFilesListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int obsId, int QM_ATR_ID)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.Get2TierPDFFilesListDAL(page, rows, sidx, sord, out totalRecords, obsId, QM_ATR_ID);
        }

        public string QM2TierSaveATRRegradeBAL(QMATRRegradeModel qmATRRegradeModel)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.QM2TierSaveATRRegradeDAL(qmATRRegradeModel);
        }

        #endregion


        #region CQC

        public Array CQCMonitorsScheduledRoadListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int monitorCode, int inspMonth, int inspYear)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.CQCMonitorsScheduledRoadListDAL(page, rows, sidx, sord, out totalRecords, monitorCode, inspMonth, inspYear);
        }

        #endregion


        #region Reports

        public Array QMInspectionReportBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.QMInspectionReportDAL(page, rows, sidx, sord, out totalRecords);
        }


        public Array QMOverallDistrictwiseInspDetailsReportBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, Int32 stateCode, Int32 districtCode, string qmType)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.QMOverallDistrictwiseInspDetailsReportDAL(page, rows, sidx, sord, out totalRecords, stateCode, districtCode, qmType);
        }


        public Array QMGradingAndATRListingBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int fromYear, int toYear, int fromMonth, int toMonth, string qmType)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.QMGradingAndATRListingDAL(page, rows, sidx, sord, out totalRecords, fromYear, toYear, fromMonth, toMonth, qmType);
        }

        public Array QMGradingComparisionListingBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int state, string district, int year, string month)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.QMGradingComparisionListingDAL(page, rows, sidx, sord, out totalRecords, state, district, year, month);
        }

        public Array QMMonthwiseInspectionListingBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int state, int year, string qmType)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.QMMonthwiseInspectionListingDAL(page, rows, sidx, sord, out totalRecords, state, year, qmType);
        }


        public Array QMATRDetailsReportBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, Int32 fromyear, Int32 frommonth, Int32 toyear, Int32 tomonth)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.QMATRDetailsReportDAL(page, rows, sidx, sord, out totalRecords, fromyear, frommonth, toyear, tomonth);
        }

        public Array QMItemwiseNQMInspectionReportBAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, string state, int grade, Int32 fromyear, Int32 frommonth, Int32 toyear, Int32 tomonth, int citem, string qmType)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.QMItemwiseNQMInspectionReportDAL(page, rows, sidx, sord, out totalRecords, state, grade, fromyear, frommonth, toyear, tomonth, citem, qmType);
        }

        public List<USP_QM_UNSATISFACTORY_WORKS_FOR_STATE_Result> UnsatisfactoryWorkDetailsBAL(int stateCode, string qmType)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.UnsatisfactoryWorkDetailsDAL(stateCode, qmType);
        }

        public List<USP_QM_COMMENCED_WORKS_Result> CommencedWorkDetailsBAL()
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.CommencedWorkDetailsDAL();
        }

        public List<USP_QM_COMMENCED_INSP_DETAILS_Result> CommencedInspDetailsBAL(int state, int duration, string qmType)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.CommencedInspDetailsDAL(state, duration, qmType);
        }

        public List<USP_QM_COMMENCED_WORKS_DETAILS_Result> CommencedRoadDetailsBAL(int state, int duration)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.CommencedRoadDetailsDAL(state, duration);
        }

        public List<USP_QM_COMPLETED_WORKS_Result> CompletedWorksBAL(string frmDate, string toDate)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.CompletedWorksDAL(frmDate, toDate);
        }

        public List<USP_QM_COMPLETED_INSP_DETAILS_Result> CompletedInspDetailsBAL(int roadCode, string qmType)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.CompletedInspDetailsDAL(roadCode, qmType);
        }

        public List<USP_QM_DEFFECTIVE_GRAPH_Result> DefectiveGradingLineChartBAL(int state, int year, string rdStatus, string valueType)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.DefectiveGradingLineChartDAL(state, year, rdStatus, valueType);
        }
        #endregion


        #region Mainatenance_Inspection

        public bool SaveMaintenanceInspectionBAL(MaintenanceInspectionViewModel model, out string message)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.SaveMaintenanceInspectionDAL(model, out message);
        }

        public List<SelectListItem> PopulateMaintenanceMonitorsBAL()
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.PopulateMaintenanceMonitorsDAL();
        }

        public List<SelectListItem> PopulateMaintenanceInspectionRoadsBAL(string id)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.PopulateMaintenanceInspectionRoadsDAL(id);
        }

        public decimal? GetProposalLengthBAL(int proposalCode)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.GetProposalLengthDAL(proposalCode);
        }

        public List<SelectListItem> PopulateRoadByPackageBAL(string package)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.PopulateRoadByPackageDAL(package);
        }

        public List<SelectListItem> PopulatePackageBAL(string id)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.PopulatePackageDAL(id);
        }

        #endregion


        #region LabDetails --- developed by Anand Singh (Integrated on 09/09/2014 by Shyam Yadav)

        public Array GetPIU1TierLabDetailListBAL(int state, int district, string level, out long totalRecords)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.GetPIU1TierLabDetailListDAL(state, district, level, out totalRecords);
        }

        public bool LabDetailSave(int agreementCode, string packageId, string labEshtablishedDate, ref string message)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.LabDetailSave(agreementCode, packageId, labEshtablishedDate, ref message);
        }

        public bool LabDetailDeleteFinalizeBAL(int id, string type, ref string message)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.LabDetailDeleteFinalizeDAL(id, type, ref message);
        }

        public Array GetLabFilesListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int labId)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.GetLabFilesListDAL(page, rows, sidx, sord, out totalRecords, labId);
        }

        public string AddLabFileUploadDetailsBAL(List<PMGSY.Models.QualityMonitoring.LabFileUploadViewModel> lstFileUploadViewModel)
        {
            qualityDAL = new QualityMonitoringDAL();
            dbContext = new PMGSYEntities();
            try
            {
                // Image Upload
                QUALITY_QM_LAB_DETAILS qualityQMLabDetails = new QUALITY_QM_LAB_DETAILS();
                foreach (LabFileUploadViewModel model in lstFileUploadViewModel)
                {
                    qualityQMLabDetails.QM_LAB_ID = model.QM_LAB_ID;

                    qualityQMLabDetails.QM_LAB_FILE_NAME = model.name;
                    qualityQMLabDetails.QM_LAB_FILE_DESC = model.Image_Description;
                    qualityQMLabDetails.QM_LAB_FILE_UPLOAD_DATE = DateTime.Now;
                    qualityQMLabDetails.QM_LAB_FILE_LATITUDE = model.Latitude;
                    qualityQMLabDetails.QM_LAB_FILE_LONGITUDE = model.Longitude;
                    //Added By Abhishek Kamble 30-nov-2013
                    qualityQMLabDetails.USERID = PMGSYSession.Current.UserId;
                    qualityQMLabDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }

                return qualityDAL.AddLabFileUploadDetailsDAL(qualityQMLabDetails);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return ("Error Occurred While Processing Request.");
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        public bool AddLABSaveDetailsBAL(LabDateViewModel labDateViewModel, ref string message)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.AddLABSaveDetailsDAL(labDateViewModel, ref message);
        }

        public string DeleteLabFileDetailsBAL(int QM_FILE_ID)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.DeleteLabFileDetailsDAL(QM_FILE_ID);
        }

        public string UpdateLabImageDetailsBAL(LabFileUploadViewModel labfileuploadViewModel)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.UpdateLabImageDetailsDAL(labfileuploadViewModel);
        }
        #endregion


        #region MP Visit

        public Array GetRoadListForMPVisitBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, string filters, int stateCode, int districtCode, int blockCode)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.GetRoadListForMPVisitDAL(page, rows, sidx, sord, out totalRecords, filters, stateCode, districtCode, blockCode);
        }

        public bool AddMPVisitDetails(FillMPVisitModel model, ref string message)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.AddMPVisitDetailsDAL(model, ref message);
        }


        public Array GetMPVisitListBAL(int? page, int? rows, string sidx, string sord, out long totalRecords, int prRoadCode)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.GetMPVisitListDAL(page, rows, sidx, sord, out totalRecords, prRoadCode);
        }

        public FillMPVisitModel GetMPVisitDetailsBAL(int visitCode)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.GetMPVisitDetailsDAL(visitCode);
        }

        public int GetBlockCode(int prRoadCode)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.GetBlockCodeDAL(prRoadCode);

        }

        public bool UpdateMPVisitBAL(FillMPVisitModel mpvisitModel, ref string message)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.UpdateMpVisitDAL(mpvisitModel, ref message);
        }

        public bool DeleteMPBAL(int visitCode)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.DeleteMPDAL(visitCode);
        }

        public QUALITY_QM_MP_VISIT GetVisitDetailsBAL(int VisitCode)
        {
            qualityDAL = new QualityMonitoringDAL();

            return qualityDAL.GetVisitDetails(VisitCode);
        }

        public IMS_SANCTIONED_PROJECTS GetRoadDetailsBAL(int prRoadCode)
        {
            qualityDAL = new QualityMonitoringDAL();

            return qualityDAL.GetRoadDetails(prRoadCode);
        }


        #region File Upload

        /// <summary>
        ///  Get List Image 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        /// <returns></returns>
        public Array GetImageListMPVisitBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int MP_VISIT_ID)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.GetImageListMPVisitDAL(page, rows, sidx, sord, out totalRecords, MP_VISIT_ID);
        }

        /// <summary>
        /// Get PDF List
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        /// <returns></returns>
        public Array GetPDFListMPVisitBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int MP_VISIT_ID)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.GetPDFListMPVisitDAL(page, rows, sidx, sord, out totalRecords, MP_VISIT_ID);
        }

        /// <summary>
        /// Add File Upload Details
        /// </summary>
        /// <param name="lstFileUploadViewModel"></param>
        /// <param name="ISPF_TYPE"></param>
        /// <returns></returns>
        public string AddFileUploadMPVisitDetailsBAL(List<QMMPVisitFileUploadViewModel> lstFileUploadViewModel, string IS_PDF)
        {
            List<QUALITY_QM_MP_VISIT_FILES> lst_files = new List<QUALITY_QM_MP_VISIT_FILES>();
            qualityDAL = new QualityMonitoringDAL();
            // Image Upload
            if (IS_PDF.ToUpper() == "N")
            {

                foreach (QMMPVisitFileUploadViewModel model in lstFileUploadViewModel)
                {
                    lst_files.Add(
                        new QUALITY_QM_MP_VISIT_FILES()
                        {
                            MP_VISIT_ID = model.MP_VISIT_ID,
                            FILE_UPLOAD_DATE = DateTime.Now,
                            FILE_NAME = model.name,
                            IS_PDF = "N",
                        }
                   );
                }

            }
            //  PDF File Upload
            else if (IS_PDF.ToUpper() == "Y")
            {

                foreach (QMMPVisitFileUploadViewModel model in lstFileUploadViewModel)
                {
                    lst_files.Add(
                        new QUALITY_QM_MP_VISIT_FILES()
                        {
                            MP_VISIT_ID = model.MP_VISIT_ID,
                            FILE_UPLOAD_DATE = DateTime.Now,
                            FILE_NAME = model.name,
                            IS_PDF = "Y",

                        }
                   );
                }
            }
            return qualityDAL.AddFileUploadMPVisitDetailsDAL(lst_files);
        }

        /// <summary>
        ///  Delete File and File Details
        /// </summary>
        /// <param name="IMS_FILE_ID"></param>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        /// <param name="IMS_FILE_NAME"></param>
        /// <param name="ISPF_TYPE"></param>
        /// <returns></returns>
        public string DeleteMPVisitFileDetailsBAL(int FILE_ID, int MP_VISIT_ID, string FILE_NAME, string IS_PDF)
        {
            qualityDAL = new QualityMonitoringDAL();
            dbContext = new PMGSYEntities();
            QUALITY_QM_MP_VISIT_FILES ims_files = dbContext.QUALITY_QM_MP_VISIT_FILES.Where(
                a => a.FILE_ID == FILE_ID &&
                a.MP_VISIT_ID == MP_VISIT_ID &&
                a.IS_PDF.ToUpper() == IS_PDF &&
                a.FILE_NAME == FILE_NAME).FirstOrDefault();

            return qualityDAL.DeleteMPVisitFileDetailsDAL(ims_files);
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
        /// Validates the PDF File
        /// </summary>
        /// <param name="FileSize"></param>
        /// <param name="FileExtension"></param>
        /// <returns></returns>
        public string ValidateMPVisitPDFFile(int FileSize, string FileExtension)
        {
            if (FileExtension.ToUpper() != ".PDF")
            {
                return "File is not PDF File";
            }
            if (FileSize > Convert.ToInt32(ConfigurationManager.AppSettings["MPVISIT_PDF_FILE_MAX_SIZE"]))
            {
                return "File Size Exceed the Maximum File Limit";
            }

            return string.Empty;
        }

        /// <summary>
        /// Validates the Image File
        /// </summary>
        /// <param name="FileSize"></param>
        /// <param name="FileExtension"></param>
        /// <returns></returns>
        public string ValidateMPVisitImageFile(int FileSize, string FileExtension)
        {
            string ValidExtensions = ConfigurationManager.AppSettings["MPVISIT_IMAGE_VALID_FORMAT"];
            string[] arrValidFormats = ValidExtensions.Split('$');


            if (!arrValidFormats.Contains(FileExtension.ToLower()))
            {
                return "File is not Valid Image File";
            }
            if (FileSize > Convert.ToInt32(ConfigurationManager.AppSettings["MPVISIT_IMAGE_FILE_MAX_SIZE"]))
            {
                return "File Size Exceed the Maximum File Limit";
            }

            return string.Empty;
        }

        #endregion



        #endregion

        // Added By Aanad 10 DEC 2015
        #region Team Details
        public string QMTeamCreateDAL(QUALITY_QM_TEAM team)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.QMTeamCreateDAL(team);

        }
        public string QMTeamDeActivateBAL(int teamid)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.QMTeamDeActivateDAL(teamid);
        }


        /// <summary>
        /// Send Mail to Team
        /// </summary>
        /// <param name="qmLetterModel"></param>
        /// <param name="ErrorMessage"></param>
        /// <returns></returns>
        public virtual MailMessage SendMailCustomFuncToTeam(QMLetterModel qmLetterModel, ref String ErrorMessage)
        {
            SendMailCustomFuncModel eMailModel = new SendMailCustomFuncModel();
            dbContext = new PMGSYEntities();
            ADMIN_SQC sqcDetails = null;
            ADMIN_QUALITY_MONITORS qmDetails = null;
            QUALITY_QM_LETTER letterDetails = null;
            QUALITY_QM_SCHEDULE scheduleDetails = null;
            try
            {
                if (qmLetterModel.QC_TYPE.Equals("S"))  //SQC
                {
                    sqcDetails = dbContext.ADMIN_SQC.Find(qmLetterModel.QC_CODE);
                    letterDetails = dbContext.QUALITY_QM_LETTER.Find(qmLetterModel.LETTER_ID);
                    eMailModel.RoleCode = 8;
                    eMailModel.EmailRecepient = sqcDetails.ADMIN_QC_EMAIL;
                    DateTime dtDate = new DateTime(letterDetails.ADMIN_IM_YEAR, letterDetails.ADMIN_IM_MONTH, 1);
                    qmLetterModel.MONTH_TEXT = dtDate.ToString("MMMM");
                    eMailModel.EmailSubject = "Inspection Schedule of " + qmLetterModel.MONTH_TEXT + ", " + letterDetails.ADMIN_IM_YEAR.ToString();
                    eMailModel.EmailCC = ConfigurationManager.AppSettings["EMAIL_CC"].ToString();
                    eMailModel.AttachedFilePath = ConfigurationManager.AppSettings["QUALITY_QM_LETTER_TEAM"].ToString() + qmLetterModel.FILE_NAME + ".pdf";
                    eMailModel.EmailDate = DateTime.Now.ToString();
                    eMailModel.RecepientName = sqcDetails.ADMIN_QC_NAME;
                    eMailModel.RecepientState = dbContext.MASTER_STATE.Where(c => c.MAST_STATE_CODE == sqcDetails.MAST_STATE_CODE).Select(c => c.MAST_STATE_NAME).FirstOrDefault();
                    eMailModel.RecepientDesignation = dbContext.MASTER_DESIGNATION.Where(c => c.MAST_DESIG_CODE == sqcDetails.ADMIN_QC_DESG).Select(c => c.MAST_DESIG_NAME).FirstOrDefault();
                    eMailModel.RecepientAddress = sqcDetails.ADMIN_QC_ADDRESS1 + (sqcDetails.ADMIN_QC_ADDRESS2 != null ? (", " + sqcDetails.ADMIN_QC_ADDRESS2) : "");

                }
                else if (qmLetterModel.QC_TYPE.Equals("I")) //NQM
                {
                    //Extract all relative Details 
                    scheduleDetails = dbContext.QUALITY_QM_SCHEDULE.Find(qmLetterModel.SCHEDULE_CODE);
                    letterDetails = dbContext.QUALITY_QM_LETTER.Find(qmLetterModel.LETTER_ID);
                    qmDetails = dbContext.ADMIN_QUALITY_MONITORS.Find(scheduleDetails.ADMIN_QM_CODE);

                    //assign letter details
                    qmLetterModel.LETTER_ID = letterDetails.LETTER_ID;

                    eMailModel.RoleCode = 6;
                    eMailModel.EmailRecepient = qmDetails.ADMIN_QM_EMAIL;
                    DateTime dtDate = new DateTime(scheduleDetails.ADMIN_IM_YEAR, scheduleDetails.ADMIN_IM_MONTH, 1);
                    qmLetterModel.MONTH_TEXT = dtDate.ToString("MMMM");
                    eMailModel.EmailSubject = "Inspection Schedule of " + qmLetterModel.MONTH_TEXT + ", " + scheduleDetails.ADMIN_IM_YEAR.ToString();
                    eMailModel.EmailCC = ConfigurationManager.AppSettings["EMAIL_CC"].ToString();
                    eMailModel.AttachedFilePath = ConfigurationManager.AppSettings["QUALITY_QM_LETTER_TEAM"].ToString() + qmLetterModel.FILE_NAME + ".pdf";
                    eMailModel.EmailDate = DateTime.Now.ToString();
                    eMailModel.RecepientName = qmDetails.ADMIN_QM_FNAME + (qmDetails.ADMIN_QM_MNAME != null ? (" " + qmDetails.ADMIN_QM_MNAME) : "") + (qmDetails.ADMIN_QM_LNAME != null ? (" " + qmDetails.ADMIN_QM_LNAME) : "");
                    eMailModel.RecepientState = dbContext.MASTER_STATE.Where(c => c.MAST_STATE_CODE == qmDetails.MAST_STATE_CODE).Select(c => c.MAST_STATE_NAME).FirstOrDefault();
                    eMailModel.RecepientDesignation = dbContext.MASTER_DESIGNATION.Where(c => c.MAST_DESIG_CODE == qmDetails.ADMIN_QM_DESG).Select(c => c.MAST_DESIG_NAME).FirstOrDefault();
                    eMailModel.RecepientAddress = qmDetails.ADMIN_QM_ADDRESS1 + (qmDetails.ADMIN_QM_ADDRESS2 != null ? (", " + qmDetails.ADMIN_QM_ADDRESS2) : "");
                }

                string headerPath = System.Web.Hosting.HostingEnvironment.MapPath(@"~/Content/images/Header-e-gov-6.png");

                // Changed by Srishti on 13-0-2023
                //var mailMessage = new MvcMailMessage
                var mailMessage = new MailMessage
                {
                    Subject = eMailModel.EmailSubject
                };
                mailMessage.To.Add(eMailModel.EmailRecepient);
                if (eMailModel.EmailCC != "")
                {
                    mailMessage.CC.Add(eMailModel.EmailCC);
                }
                if (eMailModel.AttachedFilePath != "")
                {
                    mailMessage.Attachments.Add(new Attachment(eMailModel.AttachedFilePath));
                }

                var resources = new Dictionary<string, string>();
                resources["logo"] = headerPath;

                IUserMailer iuserMailer = new UserMailer();
                var mailMsg = iuserMailer.SendMailCustomFunc(eMailModel, mailMessage, "~/Views/UserMailer/SendMailCustomFunc.cshtml", resources);
                return mailMessage;
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw new Exception("Error while Sending Letter");
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public void GenerateLetterForTeam(QMLetterModel model)
        {
            qualityDAL = new QualityMonitoringDAL();
            qualityDAL.GenerateLetterForTeam(model);
        }
        #endregion

        #region Tour Generate Invoice
        public Array GetTourPaymentListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int IMS_YEAR, int Month, int Monitor)
        {
            QualityMonitoringDAL objDAL = new QualityMonitoringDAL();
            return objDAL.GetTourPaymentListDAL(page, rows, sidx, sord, out totalRecords, IMS_YEAR, Month, Monitor);
        }

        public Array GetTourInvoiceListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int scheduleCode)
        {
            QualityMonitoringDAL objDAL = new QualityMonitoringDAL();
            return objDAL.GetTourInvoiceListDAL(page, rows, sidx, sord, out totalRecords, scheduleCode);
        }

        public string AddTourInvoiceDetailsBAL(QMTourGenerateInvoice model)
        {
            QualityMonitoringDAL objDAL = new QualityMonitoringDAL();
            return objDAL.AddTourInvoiceDetailsDAL(model);
        }

        public bool DeleteTourGeneratedInvoiceBAL(int invoiceID, ref string message)
        {
            QualityMonitoringDAL objDAL = new QualityMonitoringDAL();
            return objDAL.DeleteTourGeneratedInvoiceDAL(invoiceID, ref message);
        }
        #endregion

        #region Tour Payment
        public Array ListTourPaymentInvoiceBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int IMS_YEAR, int Month, int Monitor)
        {
            QualityMonitoringDAL objDAL = new QualityMonitoringDAL();
            return objDAL.ListTourPaymentInvoiceDAL(page, rows, sidx, sord, out totalRecords, IMS_YEAR, Month, Monitor);
        }

        public Array GetTourPaymentListBAL(int invoiceCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            QualityMonitoringDAL objDAL = new QualityMonitoringDAL();
            return objDAL.GetTourPaymentListDAL(invoiceCode, page, rows, sidx, sord, out totalRecords);
        }

        public bool AddTourPaymentDetailsBAL(TourAddPaymentModel model, ref string message)
        {
            QualityMonitoringDAL objDAL = new QualityMonitoringDAL();
            return objDAL.AddTourPaymentDetailsDAL(model, ref message);
        }

        public TourAddPaymentModel GetTourPaymentDetailsBAL(int PayemntCode, int IMSInvoiceCode)
        {
            QualityMonitoringDAL objDAL = new QualityMonitoringDAL();
            return objDAL.GetTourPaymentDetailsDAL(PayemntCode, IMSInvoiceCode);
        }

        public bool UpdateTourPaymentDetailsBAL(TourAddPaymentModel model, ref string message)
        {
            QualityMonitoringDAL objDAL = new QualityMonitoringDAL();
            return objDAL.UpdateTourPaymentDetailsDAL(model, ref message);
        }

        public bool DeleteTourPaymentDetailsBAL(int paymentCode, int imsInvoiceCode, ref string message)
        {
            QualityMonitoringDAL objDAL = new QualityMonitoringDAL();
            return objDAL.DeleteTourPaymentDetailsDAL(paymentCode, imsInvoiceCode, ref message);
        }

        public bool FinalizeTourPaymentDetailsBAL(int paymentCode, int imsInvoiceCode, ref string message)
        {
            QualityMonitoringDAL objDAL = new QualityMonitoringDAL();
            return objDAL.FinalizeTourPaymentDetailsDAL(paymentCode, imsInvoiceCode, ref message);
        }
        #endregion

        #region Bank Details
        public Array ListBankDetailsQM(int adminQmCode, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            QualityMonitoringDAL objDAL = new QualityMonitoringDAL();
            return objDAL.ListBankDetailsQMDAL(adminQmCode, page, rows, sidx, sord, out totalRecords);
        }

        public bool AddBankDetailsQM(QMBankDetailsModel model, ref string message)
        {
            QualityMonitoringDAL objDAL = new QualityMonitoringDAL();
            return objDAL.AddBankDetailsQM(model, ref message);
        }

        public bool DeleteBankDetailsQM(int accountId, int customerId, ref string message)
        {
            QualityMonitoringDAL objDAL = new QualityMonitoringDAL();
            return objDAL.DeleteBankDetailsQM(accountId, customerId, ref message);
        }
        #endregion

        #region Joint Inspection
        public byte[] GenerateJointInspectionPDF(int id)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.GenerateJointInspectionPDF(id);
        }

        public Array GetJointInspectionDetailsList(int blockCode, string ptype, string inspstatus, int page, int rows, string sidx, string sord, out Int32 totalRecords)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.GetJointInspectionDetailsList(blockCode, ptype, inspstatus, page, rows, sidx, sord, out totalRecords);
        }

        public QMJIViewModel QMJIHeader(int roadCode)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.QMJIHeader(roadCode);
        }
        public bool SaveQMJointInspectionDetailsBAL(QMJIViewModel model, ref string message)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.SaveQMJointInspectionDetailsDAL(model, ref message);
        }
        public QMJIViewModel GetJIDetailsBAL(int jiCode)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.GetJIDetailsDAL(jiCode);

        }
        public string UpdateQMJointInspectionDetailsBAL(QMJIViewModel model)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.UpdateQMJointInspectionDetailsDAL(model);
        }
        public string QMJIDeleteBAL(int jiCode)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.QMJIDeleteDAL(jiCode);
        }

        public string QMJIATRAddBAL(QMJIATRModel model)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.QMJIATRAddDAL(model);
        }
        public string QMJIATRDeleteBAL(int atfileId)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.QMJIATRDeleteDAL(atfileId);
        }
        public List<QMJIATRModel> QMJIATRListBAL(int jiCode)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.QMJIATRListDAL(jiCode);
        }

        public Array GetJIDetailsListBAL(int prRoadCode, int page, int rows, string sidx, string sord, out Int32 totalRecords)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.GetJIDetailsListDAL(prRoadCode, page, rows, sidx, sord, out totalRecords);
        }
        #endregion

        #region Quality Complain

        public bool QMComplainAddBAL(QMComplainViewModel complainModel)
        {
            qualityDAL = new QualityMonitoringDAL();
            qualityDAL.QMComplainAddDAL(complainModel);

            return true;
        }

        public Array QMComplainListBAL(QMComplainFilterViewModel complainFilterModel)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.QMComplainListDAL(complainFilterModel);
        }

        public string QMComplainDeleteBAL(int complainId)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.QMComplainDeleteDAL(complainId);
        }

        public QMComplainDetailViewModel GetQMComplainBAL(int ComplainId)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.GetQMComplainDAL(ComplainId);
        }
        public bool QMComplainFileUploadBAL(QMComplainUploadViewModel uploadModel)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.QMComplainFileUploadDAL(uploadModel);
        }

        public bool QMComplainDetailFileUploadBAL(QMComplainUploadViewModel uploadModel)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.QMComplainDetailFileUploadDAL(uploadModel);
        }

        public string QMComplainDetailDeleteBAL(int complainId)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.QMComplainDetailDeleteDAL(complainId);
        }

        #endregion


        #region Populate Road LIST to assign it in schedule contractor wise added by deendayal
        /// Populate Road LIST to assign it in schedule contractor wise added by deendayal
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        public Array GetRoadListToAssignContractorwiseBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, string filters, int districtCode, int adminSchCode, int sanctionYear)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.GetRoadListToAssignContractorwiseDAL(page, rows, sidx, sord, out totalRecords, filters, districtCode, adminSchCode, sanctionYear);
        }

        #endregion


        #region Get current work status
        public string GetCurrentworkStatus(int road_code)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.GetCurrentworkStatus(road_code);
        }
        #endregion

        #region Allocate Roads to NQM
        public Array AllocateRoadsToNQmList(AllocateRoadsToNQMModel objFilter, out long totalRecords)
        {
            try
            {
                qualityDAL = new QualityMonitoringDAL();
                return qualityDAL.AllocateRoadsToNQmList(objFilter, out totalRecords);
            }
            catch (Exception ex)
            {

                throw new Exception("Error while Getting EAuthorization Request Details...");
            }
        }
        #endregion

        //added by abhinav pathak
        #region Delete uploaded inspection image uploaded through web
        public bool DeleteInspectionImageBAL(int fieldID, string filename, int obsID, string Year) // Changes Done BY saurabh here passes Year as Arguement.
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.DeleteInspectionImageDAL(fieldID, filename, obsID, Year); // Changes Done BY saurabh here passes Year as Arguement.
        }

        #endregion

        #region Upload Inspection by NRIDA Officials

        public Array GetInspRoadList(int stateCode, int districtCode, int blockCode, int yearCode, int batch, int scheme, string proposalType, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                qualityDAL = new QualityMonitoringDAL();
                return qualityDAL.GetInspRoadList(stateCode, districtCode, blockCode, yearCode, batch, scheme, proposalType, page, rows, sidx, sord, out totalRecords);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
        }

        public bool AddInspByNRIDADetails(FormCollection formCollection, HttpPostedFileBase postedBgFile, out String IsValid)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.AddInspByNRIDADetails(formCollection, postedBgFile, out IsValid);
        }

        public Array GetInspByNRIDADetailsList(int roadCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.GetInspByNRIDADetailsList(roadCode, page, rows, sidx, sord, out totalRecords);
        }

        public Array GetInspUploadedDetailsList(int roadCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.GetInspUploadedDetailsList(roadCode, page, rows, sidx, sord, out totalRecords);
        }

        #endregion

        #region Work List Added by Chandra Darshan Agrawal

        public Array GetRoadListBAL(int stateCode, int districtCode, int ddlTech,  int page, int rows, string sidx, string sord, out long totalRecords, string filter)
        {
            try
            {
                qualityDAL = new QualityMonitoringDAL();
                return qualityDAL.GetRoadListDAL(stateCode, districtCode, ddlTech, page, rows, sidx, sord, out totalRecords, filter);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
        }


        #endregion

        #region Monitor Proficiency Test Score

        public Array GetProficiencyTestScoreList(string filters, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                qualityDAL = new QualityMonitoringDAL();
                return qualityDAL.GetProficiencyTestScoreList(filters, page, rows, sidx, sord, out totalRecords);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
        }

        public Array GetProficiencyTestScoreListDetails(int examId, string filters, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                qualityDAL = new QualityMonitoringDAL();
                return qualityDAL.GetProficiencyTestScoreListDetails(examId, filters, page, rows, sidx, sord, out totalRecords);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
        }

        public bool AddProficiencyScore(FormCollection formCollection, out String IsValid)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.AddProficiencyScore(formCollection, out IsValid);
        }

        public bool EditProficiencyTestScore(FormCollection formCollection, out String IsValid)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.EditProficiencyTestScore(formCollection, out IsValid);
        }

        public Array GetProficiencyTestScoreListCQC(string filters, string monitorType, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                qualityDAL = new QualityMonitoringDAL();
                return qualityDAL.GetProficiencyTestScoreListCQC(filters, monitorType, page, rows, sidx, sord, out totalRecords);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
        }

        public bool EditProficiencyScoreDetails(FormCollection formCollection, out String IsValid)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.EditProficiencyScoreDetails(formCollection, out IsValid);
        }

        public bool GetUploadDetails(FormCollection formCollection, HttpPostedFileBase fileSrc, ref string message)
        {
            qualityDAL = new QualityMonitoringDAL();
            return qualityDAL.GetUploadDetails(formCollection, fileSrc, ref message);
        }

        #endregion

        #region Allocate Works to Technical Expert

        #region Add TE details and create user

        public Array LoadTechnicalExpertDetailsGrid(int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                qualityDAL = new QualityMonitoringDAL();
                return qualityDAL.LoadTechnicalExpertDetailsGrid(page, rows, sidx, sord, out totalRecords);
            }
            catch (Exception)
            {

                throw;
            }

        }

        public bool AddTechnicalExpertDetails(FormCollection formCollection, out String IsValid)
        {
            try
            {
                qualityDAL = new QualityMonitoringDAL();
                return qualityDAL.AddTechnicalExpertDetails(formCollection, out IsValid);
            }
            catch (Exception)
            {

                throw;
            }

        }

        public bool UpdateTechnicalExpertDetails(FormCollection formCollection, out String IsValid)
        {
            try
            {
                qualityDAL = new QualityMonitoringDAL();
                return qualityDAL.UpdateTechnicalExpertDetails(formCollection, out IsValid);
            }
            catch (Exception)
            {

                throw;
            }

        }

        public bool CreateTechnicalExpertUserBAL(int technicalExpertId)
        {
            try
            {
                qualityDAL = new QualityMonitoringDAL();
                return qualityDAL.CreateTechnicalExpertUserDAL(technicalExpertId);
            }
            catch (Exception)
            {

                throw;
            }

        }

        #endregion

        #region Allocate TE at CQC

        public Array QMInspectionDetailsAllocateTechExpertBAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int monitorCode, int fromMonth, int fromYear, int toMonth, int toYear, string qmType, int schemeType, string roadStatus, string roadOrBridge, string gradeType, out List<int> allocatedWorksObservationIdsList)
        {
            try
            {
                qualityDAL = new QualityMonitoringDAL();
                return qualityDAL.QMInspectionDetailsAllocateTechExpertDAL(page, rows, sidx, sord, out totalRecords,
                                                           stateCode, monitorCode, fromMonth, fromYear, toMonth, toYear, qmType, schemeType, roadStatus, roadOrBridge, gradeType, out allocatedWorksObservationIdsList);

            }
            catch (Exception)
            {

                throw;
            }

        }

        public bool AssignTechExpertBAL(int techExpertID, int[] submitarray, out string isValidMsg)
        {
            try
            {
                qualityDAL = new QualityMonitoringDAL();
                return qualityDAL.AssignTechExpertDAL(techExpertID, submitarray, out isValidMsg);
            }
            catch (Exception)
            {

                throw;
            }

        }

        public bool RemoveTechnicalExpertBAL(int observationId, out string isValidMsg)
        {
            try
            {
                qualityDAL = new QualityMonitoringDAL();
                return qualityDAL.RemoveTechnicalExpertDAL(observationId, out isValidMsg);
            }
            catch (Exception)
            {

                throw;
            }

        }


        public bool FinalizeTechnicalExpertBAL(string[] arrWorksToFinalize, out string isValidMsg)
        {
            try
            {
                qualityDAL = new QualityMonitoringDAL();
                return qualityDAL.FinalizeTechnicalExpertDAL(arrWorksToFinalize, out isValidMsg);
            }
            catch (Exception)
            {

                throw;
            }

        }

        public TEQMFillObservationModel TEQMObservationDetailsBAL(int obsId)
        {
            try
            {
                qualityDAL = new QualityMonitoringDAL();
                return qualityDAL.TEQMObservationDetailsDAL(obsId);
            }
            catch (Exception)
            {

                throw;
            }

        }


        #endregion

        #region Add remark by TE 
        public Array QMInspectionDetailsTechExpertReviewBAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords,
                                          int stateCode, int monitorCode, int fromMonth, int fromYear,
                                          int toMonth, int toYear, string qmType, int schemeType, string roadStatus, string roadOrBridge, string gradeType)
        {
            try
            {
                qualityDAL = new QualityMonitoringDAL();
                return qualityDAL.QMInspectionDetailsTechExpertReviewDAL(page, rows, sidx, sord, out totalRecords,
                                                           stateCode, monitorCode, fromMonth, fromYear, toMonth, toYear, qmType, schemeType, roadStatus, roadOrBridge, gradeType);

            }
            catch (Exception)
            {

                throw;
            }
        }

        public bool SaveTechExpertRemarksBAL(Dictionary<int, string> itemwiseRemark, int obsId, out string message, string generalObs)
        {
            try
            {
                qualityDAL = new QualityMonitoringDAL();
                return qualityDAL.SaveTechExpertRemarksDAL(itemwiseRemark, obsId, out message, generalObs);
            }
            catch (Exception)
            {

                throw;
            }

        }

        public bool UpdateTechExpertRemarksBAL(Dictionary<int, string> itemwiseRemark, int obsId, out string message, string generalObs)
        {
            try
            {
                qualityDAL = new QualityMonitoringDAL();
                return qualityDAL.UpdateTechExpertRemarksDAL(itemwiseRemark, obsId, out message, generalObs);
            }
            catch (Exception)
            {

                throw;
            }

        }
        #endregion

        #region Add remark by NQM
        #endregion

        #region Add Payment Information For TE 

        public Array TechnicalExpertPaymentDetailsBAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords)
        {
            try
            {
                qualityDAL = new QualityMonitoringDAL();
                return qualityDAL.TechnicalExpertPaymentDetailsDAL(page, rows, sidx, sord, out totalRecords);

            }
            catch (Exception)
            {

                throw;
            }

        }

        public Array AddTechnicalExpertPaymentListBAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int teId, out List<int> allocatedWorksObservationIdsList)
        {
            try
            {
                qualityDAL = new QualityMonitoringDAL();
                return qualityDAL.AddTechnicalExpertPaymentListDAL(page, rows, sidx, sord, out totalRecords, teId, out allocatedWorksObservationIdsList);

            }
            catch (Exception)
            {

                throw;
            }

        }

        public Array AddTechnicalExpertPaymentWiseListBAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, string teId, out List<int> allocatedWorksObservationIdsList)
        {
            try
            {
                qualityDAL = new QualityMonitoringDAL();
                return qualityDAL.AddTechnicalExpertPaymentWiseListDAL(page, rows, sidx, sord, out totalRecords, teId, out allocatedWorksObservationIdsList);

            }
            catch (Exception)
            {

                throw;
            }

        }

        public bool AddPaymentDetailsBAL(int[] submitarray, out string isValidMsg)
        {
            try
            {
                qualityDAL = new QualityMonitoringDAL();
                return qualityDAL.AddPaymentDetailsDAL(submitarray, out isValidMsg);
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

        #endregion

        //added by hrishikesh
        public Array GetmonitorDetailsListJSONBAL(string data, string filters, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                qualityDAL = new QualityMonitoringDAL();
                return qualityDAL.GetmonitorDetailsListJSONDAL(data, filters, page, rows, sidx, sord, out totalRecords);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
        }
    }


    public interface IQualityMonitoringBAL
    {
        #region Allocate Roads to NQM
        Array AllocateRoadsToNQmList(AllocateRoadsToNQMModel objFilter, out long totalRecords);

        #endregion

        #region CQCAdmin

        Array GetScheduleListBAL(int month, int year, int page, int rows, string sidx, string sord, out Int32 totalRecords, string filters);
        Array QMSQCLetterListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, string filters, int inspMonth, int inspYear);
        bool AddSQCLetterDetailsBAL(int id, short inspMonth, short inspYear, ref string message);
        bool AddNQMLetterDetailsBAL(int scheduleCode, ref string message);
        MailMessage SendMailCustomFunc(QMLetterModel qmLetterModel, ref String ErrorMessage);
        string QMCreateScheduleBAL(QMScheduleViewModel qmScheduleModel);
        List<SelectListItem> GetDistrictForScheduleCreationBAL(int selectedState, int month, int year);
        MasterAdminQualityMonitorViewModel MonitorDetailsBAL(int monitorCode);
        List<SelectListItem> GetScheduledDistrictListBAL(int scheduleCode, bool isAssignedDistricts);
        string CQCAddDistrictsBAL(QMCQCAddDistrictModel qmCQCAddDistrict);
        Array GetRoadListToAssignBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, string filters, int districtCode, int adminSchCode, int techCode);
        //string QMAssignRoadsBAL(int prRoadCode, int adminSchCode, string isEnquiry);
        string QMAssignWorksBAL(string[] arrWorks, int adminSchCode, string[] arrEnquiry);
        Array GetRoadPhysicalProgressList(int page, int rows, string sidx, string sord, out long totalRecords, int proposalCode);
        Array QMViewScheduleDetailsBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int adminSchCode);
        string QMDeleteSchRoadsBAL(int prRoadCode, int scheduleCode);
        string CQCDeleteDistrictBAL(int districtCode, int scheduleCode);
        string FinalizeDistrictsBAL(int scheduleCode);
        string FinalizeRoadBAL(int prRoadCode, int scheduleCode, bool isFinalizeAllRoads);
        string ForwardScheduleBAL(int scheduleCode);
        string UnlockScheduleBAL(int scheduleCode);
        Array QMViewInspectionDetailsBAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords,
                                                        int stateCode, int monitorCode, int fromMonth, int fromYear,
                                                        int toMonth, int toYear, int schemeType, string roadStatus, string roadOrBridge, string gradeType, string eFormStatusType);
        Array QMViewInspectionDetails2TierCQCBAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords,
                                                        int stateCode, int monitorCode, int fromMonth, int fromYear,
                                                        int toMonth, int toYear, string ROAD_STATUS, int schemeType, string roadOrBridge, string gradeType, string eFormStatus);
        Array QMViewATRDetailsBAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords,
                                                        int stateCode, int monitorCode, int fromMonth, int fromYear,
                                                        int toMonth, int toYear, string atrStatus, string rdStatus);
        Array QMViewBulkATRListBAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int duration);
        Array GetPrevScheduleListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int monitorId, int prevMonth, int prevYear, bool is3Tier);
        QMFillObservationModel QMGradingCorrectionBAL(int obsId);
        string QMGradingCorrectionBAL(FormCollection formCollection);
        string QMDeleteObservationBAL(int obsId);
        QMFillObservationModel QMObservationDetailsBAL(int obsId);
        QMFillObservationModel QMObservationDetailsATRBAL(int obsId);
        QMFillObservationModel QMObservationDetails2TierCQCBAL(int obsId);
        string QMDeleteATRDetailsBAL(int obsId, int atrId);
        Array Get2TierScheduleListCQCBAL(int state, int month, int year, int page, int rows, string sidx, string sord, out Int32 totalRecords, string filters);
        Array ListQualityMonitor(string qmTypeName, int stateCode, int districtCode, string isEmpanelled, string filters, int? page, int? rows, string sidx, string sord, out long totalRecords);
        #endregion


        #region SQC 3Tier
        Array QMViewInspectionDetailsSQCPIUBAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords,
                                                        int stateCode, int monitorCode, int fromMonth, int fromYear,
                                                        int toMonth, int toYear, string qmType, int schemeType, string roadStatus, string roadOrBridge, string gradeType, string eFormStatus);
        Array GetSqc3TierScheduleListBAL(int month, int year, int page, int rows, string sidx, string sord, out Int32 totalRecords, string filters);
        List<SelectListItem> PopulateMonitorsBAL(int state, int inspMonth, int inspYear, int districtCode, string qmType);
        QMFillObservationModel QMObservationDetails3TierSQCBAL(int obsId);
        #endregion


        #region PIU

        Array GetPIU3TierScheduleListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, string filters);
        Array GetPIU2TierScheduleListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, string filters);

        #endregion


        #region Monitors

        Array GetMonitorsCurrScheduleListBAL(int month, int year, int page, int rows, string sidx, string sord, out Int32 totalRecords);
        Array QMMonitorInspListBAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords,
                                                        int fromMonth, int fromYear,
                                                        int toMonth, int toYear);
        Array GetMonitorsScheduledRoadListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int inspMonth, int inspYear);
        QMFillObservationModel QMFillObservationsBAL(int adminSchCode, int prRoadCode, string roadStatus);
        string QMSaveObservationsBAL(FormCollection formCollection);
        QMTourViewModel GetTourDetailsBAL(int scheduleCode);
        string SaveTourDetailsBAL(QMTourViewModel model);
        Array GetTourListBAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int state, int qmCode, int frmMonth, int frmYear, int toMonth, int toYear);
        QMTourViewModel GetTourDetailsForUpdateBAL(int tourId);
        string UpdateTourDetailsBAL(QMTourViewModel model);
        string DeleteTourDetailsBAL(int tourId);
        string FinalizeTourDetailsBAL(int tourId, string flagValue);
        #endregion


        #region Upload File Details

        Array GetFilesListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int obsId);
        string GetStartEndLatLongBAL(int obsId);
        string GetLatLongBAL(int obsId);
        Array GetPDFFilesListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int obsId, int QM_ATR_ID);
        string AddATRDetailsBAL(List<PMGSY.Models.QualityMonitoring.FileUploadViewModel> lstFileUploadViewModel);
        string UpdateImageDetailsBAL(PMGSY.Models.QualityMonitoring.FileUploadViewModel fileuploadViewModel);
        string DeleteFileDetails(int QM_FILE_ID);
        string AddFileUploadDetailsBAL(List<PMGSY.Models.QualityMonitoring.FileUploadViewModel> lstFileUploadViewModel);

        #endregion


        #region PDF Upload BY Monitor

        string ValidatePDFFile(int FileSize, string FileExtension);
        Array GetInspReportFilesListDAL(int page, int rows, string sidx, string sord, out int totalRecords, int obsId,string isAtrPage);
        string AddPdfUploadDetailsBAL(List<FileUploadViewModel> lst_files);
        string UpdateInspPDFDetailsBAL(FileUploadViewModel qmFiles);
        string DeleteInspFileDetailsBAL(int fileId, int obsId, string fileType);

        //added by abhinav pathak on 12-08-2019
        #region for multiple pdf file upload by Monitor
        string AddMultiplePdfUploadDetailsBAL(List<FileUploadViewModel> lst_files);
        Array GetInspMultipleFilesListBAL(int page, int rows, string sidx, string sord, out int totalRecords, int obsId);
        bool FinalisePDFDeatilsBAL(int id);
        string UpdateMultipleInspPDFDetailsBAL(FileUploadViewModel qmFiles);
        string DeleteMultipleInspFileDetailsBAL(int fileId, int obsId);
        #endregion
        #endregion

        #region QCR Part-I PDF by Srishti and Vikki
        Array GetExecutionList(int yearCode, int districtCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetQCRList(int roadCode, int page, int rows, string sidx, string sord, out long totalRecords);
        bool AddQCRdDetails(AddUploadQCRDetailsModel model, out String IsValid);
        #endregion

        #region View Uploaded QCR PDF 

        Array GetExecutionListView(int yearCode, int districtCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetQCRListToView(int roadCode, int page, int rows, string sidx, string sord, out long totalRecords);

        #endregion

        #region ATR
        List<qm_inspection_list_atr_Result> ATRDetailsBAL(int stateCode, int monitorCode, int fromMonth, int fromYear,
                                                int toMonth, int toYear, string atrStatus, string rdStatus, int PmgsyScheme);//ATR_Change

        List<qm_inspection_list_atrr_Result> ATRDetailssBAL(int stateCode, int monitorCode, int fromMonth, int fromYear,
                                              int toMonth, int toYear, string atrStatus, string rdStatus, int PmgsyScheme, string flag);//ATR_Change


        string QMATRRegradeBAL(QMATRRegradeModel qmATRRegradeModel);

        #endregion

        #region 2 tier atr vikky
        List<qm_inspection_list_2_Tier_atrr_Result> ATR2TierDetailssBAL(int stateCode, int monitorCode, int fromMonth, int fromYear,
                                             int toMonth, int toYear, string atrStatus, string rdStatus, int PmgsyScheme, string flag);//ATR_Change

        Array Get2TierPDFFilesListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int obsId, int QM_ATR_ID);

        string QM2TierSaveATRRegradeBAL(QMATRRegradeModel qmATRRegradeModel);
        #endregion

        #region CQC

        Array CQCMonitorsScheduledRoadListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int monitorCode, int inspMonth, int inspYear);

        #endregion


        #region Reports

        Array QMInspectionReportBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords);
        Array QMOverallDistrictwiseInspDetailsReportBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, Int32 stateCode, Int32 districtCode, string qmType);
        Array QMGradingAndATRListingBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int fromYear, int toYear, int fromMonth, int toMonth, string qmType);
        Array QMGradingComparisionListingBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int state, string district, int year, string month);
        Array QMMonthwiseInspectionListingBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int state, int year, string qmType);
        Array QMATRDetailsReportBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, Int32 fromyear, Int32 frommonth, Int32 toyear, Int32 tomonth);
        Array QMItemwiseNQMInspectionReportBAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, string state, int grade, Int32 fromyear, Int32 frommonth, Int32 toyear, Int32 tomonth, int citem, string qmType);
        List<USP_QM_UNSATISFACTORY_WORKS_FOR_STATE_Result> UnsatisfactoryWorkDetailsBAL(int stateCode, string qmType);
        List<USP_QM_COMMENCED_WORKS_Result> CommencedWorkDetailsBAL();
        List<USP_QM_COMMENCED_INSP_DETAILS_Result> CommencedInspDetailsBAL(int state, int duration, string qmType);
        List<USP_QM_COMMENCED_WORKS_DETAILS_Result> CommencedRoadDetailsBAL(int state, int duration);
        List<USP_QM_COMPLETED_WORKS_Result> CompletedWorksBAL(string frmDate, string toDate);
        List<USP_QM_COMPLETED_INSP_DETAILS_Result> CompletedInspDetailsBAL(int roadCode, string qmType);
        List<USP_QM_DEFFECTIVE_GRAPH_Result> DefectiveGradingLineChartBAL(int state, int year, string rdStatus, string valueType);
        #endregion


        #region Maintenance_Inspection

        bool SaveMaintenanceInspectionBAL(MaintenanceInspectionViewModel model, out string message);
        List<SelectListItem> PopulateMaintenanceMonitorsBAL();
        List<SelectListItem> PopulateMaintenanceInspectionRoadsBAL(string id);
        decimal? GetProposalLengthBAL(int proposalCode);
        List<SelectListItem> PopulateRoadByPackageBAL(string package);
        List<SelectListItem> PopulatePackageBAL(string id);


        #endregion


        #region LabDetails --- developed by Anand Singh (Integrated on 09/09/2014 by Shyam Yadav)

        Array GetPIU1TierLabDetailListBAL(int state, int district, string level, out long totalRecords);
        bool LabDetailSave(int agreementCode, string packageId, string labEshtablishedDate, ref string message);
        string AddLabFileUploadDetailsBAL(List<PMGSY.Models.QualityMonitoring.LabFileUploadViewModel> lstFileUploadViewModel);
        Array GetLabFilesListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int labId);
        bool LabDetailDeleteFinalizeBAL(int id, string type, ref string message);
        bool AddLABSaveDetailsBAL(LabDateViewModel labDateViewModel, ref string message);
        string DeleteLabFileDetailsBAL(int QM_FILE_ID);
        string UpdateLabImageDetailsBAL(LabFileUploadViewModel labfileuploadViewModel);
        #endregion


        #region MP Visit

        Array GetRoadListForMPVisitBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, string filters, int stateCode, int districtCode, int blockCode);
        bool AddMPVisitDetails(FillMPVisitModel model, ref string message);
        Array GetMPVisitListBAL(int? page, int? rows, string sidx, string sord, out long totalRecords, int prRoadCode);
        FillMPVisitModel GetMPVisitDetailsBAL(int visitCode);
        bool UpdateMPVisitBAL(FillMPVisitModel mpvisitModel, ref string message);
        bool DeleteMPBAL(int visitCode);

        int GetBlockCode(int prRoadCode);

        QUALITY_QM_MP_VISIT GetVisitDetailsBAL(int VisitCode);

        IMS_SANCTIONED_PROJECTS GetRoadDetailsBAL(int prRoadCode);

        #region File Upload

        Array GetImageListMPVisitBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int MP_VISIT_ID);

        Array GetPDFListMPVisitBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int MP_VISIT_ID);

        string AddFileUploadMPVisitDetailsBAL(List<QMMPVisitFileUploadViewModel> lstFileUploadViewModel, string IS_PDF);

        string DeleteMPVisitFileDetailsBAL(int FILE_ID, int MP_VISIT_ID, string FILE_NAME, string IS_PDF);

        void CompressImage(HttpPostedFileBase httpPostedFileBase, string DestinitionPath, string ThumbnailPath);

        string ValidateMPVisitPDFFile(int FileSize, string FileExtension);

        string ValidateMPVisitImageFile(int FileSize, string FileExtension);

        #endregion


        #endregion

        // Added By Aanad 10 DEC 2015
        #region Team Details
        string QMTeamCreateDAL(QUALITY_QM_TEAM team);
        string QMTeamDeActivateBAL(int teamid);
        void GenerateLetterForTeam(QMLetterModel model);
        #endregion

        #region Tour Generate Invoice
        Array GetTourPaymentListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int IMS_YEAR, int Month, int Monitor);
        Array GetTourInvoiceListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int scheduleCode);
        string AddTourInvoiceDetailsBAL(QMTourGenerateInvoice model);
        bool DeleteTourGeneratedInvoiceBAL(int invoiceID, ref string message);
        #endregion

        #region Tour Payment
        Array ListTourPaymentInvoiceBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int IMS_YEAR, int Month, int Monitor);
        Array GetTourPaymentListBAL(int invoiceCode, int page, int rows, string sidx, string sord, out long totalRecords);
        bool AddTourPaymentDetailsBAL(TourAddPaymentModel model, ref string message);
        TourAddPaymentModel GetTourPaymentDetailsBAL(int PayemntCode, int IMSInvoiceCode);
        bool UpdateTourPaymentDetailsBAL(TourAddPaymentModel model, ref string message);
        bool DeleteTourPaymentDetailsBAL(int paymentCode, int imsInvoiceCode, ref string message);
        bool FinalizeTourPaymentDetailsBAL(int paymentCode, int imsInvoiceCode, ref string message);
        #endregion

        #region Bank Details
        Array ListBankDetailsQM(int adminQmCode, int? page, int? rows, string sidx, string sord, out long totalRecords);
        bool AddBankDetailsQM(QMBankDetailsModel model, ref string message);
        bool DeleteBankDetailsQM(int accountId, int customerId, ref string message);
        #endregion

        #region Joint Inspection
        byte[] GenerateJointInspectionPDF(int id);
        Array GetJointInspectionDetailsList(int blockCode, string ptype, string inspstatus, int page, int rows, string sidx, string sord, out Int32 totalRecords);
        QMJIViewModel QMJIHeader(int roadCode);
        bool SaveQMJointInspectionDetailsBAL(QMJIViewModel model, ref string message);
        QMJIViewModel GetJIDetailsBAL(int jiCode);
        string UpdateQMJointInspectionDetailsBAL(QMJIViewModel model);
        string QMJIDeleteBAL(int jiCode);
        string QMJIATRAddBAL(QMJIATRModel model);
        string QMJIATRDeleteBAL(int atfileId);
        List<QMJIATRModel> QMJIATRListBAL(int jiCode);
        Array GetJIDetailsListBAL(int jiCode, int page, int rows, string sidx, string sord, out Int32 totalRecords);
        #endregion

        #region Quality Complain
        bool QMComplainAddBAL(QMComplainViewModel complainModel);
        Array QMComplainListBAL(QMComplainFilterViewModel complainFilterModel);
        QMComplainDetailViewModel GetQMComplainBAL(int ComplainId);
        bool QMComplainDetailFileUploadBAL(QMComplainUploadViewModel uploadModel);
        bool QMComplainFileUploadBAL(QMComplainUploadViewModel uploadModel);
        string QMComplainDeleteBAL(int complainId);
        string QMComplainDetailDeleteBAL(int complainId);
        #endregion

        #region List of roads contractor wise added by deendayal
        Array GetRoadListToAssignContractorwiseBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, string filters, int districtCode, int adminSchCode, int sanctionYear);
        #endregion


        #region Get current work status
        string GetCurrentworkStatus(int road_code);
        #endregion

        #region Delete uploaded inspection image uploaded through web
        bool DeleteInspectionImageBAL(int fieldID, string filename, int obsID, string Year); // Changes Done BY saurabh here passes Year as Arguement.
        #endregion

        #region Upload Inspection by NRIDA Officials
        Array GetInspRoadList(int stateCode, int districtCode, int blockCode, int yearCode, int batch, int scheme, string proposalType, int page, int rows, string sidx, string sord, out long totalRecords);
        bool AddInspByNRIDADetails(FormCollection formCollection, HttpPostedFileBase postedBgFile, out String IsValid);
        Array GetInspByNRIDADetailsList(int roadCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetInspUploadedDetailsList(int roadCode, int page, int rows, string sidx, string sord, out long totalRecords);
        #endregion

        #region Work List Added by Chandra Darshan Agrawal

        Array GetRoadListBAL(int stateCode, int districtCode, int ddlTech, int page, int rows, string sidx, string sord, out long totalRecords, string filter);

        #endregion

        #region Monitor Proficiency Test Score

        Array GetProficiencyTestScoreList(string filters, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetProficiencyTestScoreListDetails(int examId, string filters, int page, int rows, string sidx, string sord, out long totalRecords);

        //added by hrishikesh 
        Array GetmonitorDetailsListJSONBAL(string data, string filters, int page, int rows, string sidx, string sord, out long totalRecords);

        bool AddProficiencyScore(FormCollection formCollection, out String IsValid);
        bool EditProficiencyTestScore(FormCollection formCollection, out String IsValid);
        Array GetProficiencyTestScoreListCQC(string filters, string monitorType, int page, int rows, string sidx, string sord, out long totalRecords);
        bool EditProficiencyScoreDetails(FormCollection formCollection, out String IsValid);
        bool GetUploadDetails(FormCollection formCollection, HttpPostedFileBase fileSrc, ref string message);

        #endregion

        #region Allocate Works to Technical Expert

        #region Add TE details and create user

        Array LoadTechnicalExpertDetailsGrid(int page, int rows, string sidx, string sord, out long totalRecords);
        bool AddTechnicalExpertDetails(FormCollection formCollection, out String IsValid);
        bool UpdateTechnicalExpertDetails(FormCollection formCollection, out String IsValid);
        bool CreateTechnicalExpertUserBAL(int technicalExpertId);

        #endregion

        #region Allocate TE at CQC

        Array QMInspectionDetailsAllocateTechExpertBAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int monitorCode, int fromMonth, int fromYear, int toMonth, int toYear, string qmType, int schemeType, string roadStatus, string roadOrBridge, string gradeType, out List<int> allocatedWorksObservationIdsList);
        bool AssignTechExpertBAL(int techExpertID, int[] submitarray, out string isValidMsg);
        bool RemoveTechnicalExpertBAL(int observationId, out string isValidMsg);
        bool FinalizeTechnicalExpertBAL(string[] arrWorksToFinalize, out string isValidMsg);
        TEQMFillObservationModel TEQMObservationDetailsBAL(int obsId);

        #endregion

        #region Add remark by TE 
        Array QMInspectionDetailsTechExpertReviewBAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords,
                                                     int stateCode, int monitorCode, int fromMonth, int fromYear,
                                                     int toMonth, int toYear, string qmType, int schemeType, string roadStatus, string roadOrBridge, string gradeType);
        bool SaveTechExpertRemarksBAL(Dictionary<int, string> itemwiseRemark, int obsId, out string message, string generalObs);

        bool UpdateTechExpertRemarksBAL(Dictionary<int, string> itemwiseRemark, int obsId, out string message, string generalObs);

        #endregion

        #region Add remark by NQM
        #endregion

        #region Add Payment Information For TE 

        Array TechnicalExpertPaymentDetailsBAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords);
        Array AddTechnicalExpertPaymentListBAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int teId, out List<int> allocatedWorksObservationIdsList);
        Array AddTechnicalExpertPaymentWiseListBAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, string teId, out List<int> allocatedWorksObservationIdsList);
        bool AddPaymentDetailsBAL(int[] submitarray, out string isValidMsg);

        #endregion

        #endregion
    }
}