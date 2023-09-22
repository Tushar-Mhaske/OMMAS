using Mvc.Mailer;
using PMGSY.Mailers;
using PMGSY.Models;
using PMGSY.Models.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace PMGSY.DAL.MeriSadak_MailToSQC
{
    public class MeriSadak_MailToSQCDAL
    {
        PMGSYEntities dbContext = null;
        private static string ErrorLogBasePath = System.Configuration.ConfigurationSettings.AppSettings["MABProgressErrorLogPath"];
        private static string ErrorLogDirectory = ErrorLogBasePath + "\\" + "ErrorLog_" + DateTime.Now.Month + "_" + DateTime.Now.Year;
        private string ErrorLogFilePath = ErrorLogDirectory + "//MeriSadak_MailToSQCLog_" + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + ".txt";	

        /// <summary>
        /// Send Mail functions
        /// </summary>
        /// <returns></returns>
        public virtual MailMessage SendMailCustomFunc(SendMailCustomFuncModel eMailModel)
        {
            // Changed by Srishti on 13-03-2023
            MailMessage mailMessage = null;
            try
            {
                // Changed by Srishti on 13-03-2023
                //mailMessage = new MvcMailMessage
                mailMessage = new MailMessage
                {
                    Subject = eMailModel.EmailSubject
                };
                        
                mailMessage.To.Add(eMailModel.EmailRecepient);

                //if (ConfigurationManager.AppSettings["MeriSadakCC1"].ToString() != null && ConfigurationManager.AppSettings["MeriSadakCC1"].ToString() != string.Empty)
                //{
                //    mailMessage.CC.Add(ConfigurationManager.AppSettings["MeriSadakCC1"].ToString());
                //}

                if (ConfigurationManager.AppSettings["MeriSadakCC2"].ToString() != null && ConfigurationManager.AppSettings["MeriSadakCC2"].ToString() != string.Empty)
                {
                    mailMessage.CC.Add(ConfigurationManager.AppSettings["MeriSadakCC2"].ToString());
                }

                if (ConfigurationManager.AppSettings["MeriSadakCC3"].ToString() != null && ConfigurationManager.AppSettings["MeriSadakCC3"].ToString() != string.Empty)
                {
                    mailMessage.CC.Add(ConfigurationManager.AppSettings["MeriSadakCC3"].ToString());
                }

                //if (ConfigurationManager.AppSettings["MeriSadakCC4"].ToString() != null && ConfigurationManager.AppSettings["MeriSadakCC4"].ToString() != string.Empty)
                //{
                //    mailMessage.CC.Add(ConfigurationManager.AppSettings["MeriSadakCC4"].ToString());
                //}

                eMailModel.EmailBCC = ConfigurationManager.AppSettings["EMAIL_CC"].ToString();
                if (eMailModel.EmailBCC != "")
                {
                    mailMessage.Bcc.Add(eMailModel.EmailBCC);
                }

                string headerPath = System.Web.Hosting.HostingEnvironment.MapPath(@"~/Content/images/Header-e-gov-6.png");
                var resources = new Dictionary<string, string>();
                resources["logo"] = headerPath;

                IUserMailer iuserMailer = new UserMailer();
                var mailMsg = iuserMailer.SendMailCustomFunc(eMailModel, mailMessage, "~/Views/UserMailer/SendMailToSQC_MeriSadak.cshtml", resources);
 
                return mailMessage;
            }
            catch (Exception ex)
            {
                if (!Directory.Exists(ErrorLogDirectory))
                {
                    Directory.CreateDirectory(ErrorLogDirectory);
                }

                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("File : " + "MeriSadak_MailToSQCDAL.cs");
                    sw.WriteLine("Method : " + "SendMailCustomFunc()");
                    sw.WriteLine("Exception : " + ex.Message);
                    sw.WriteLine("Exception StackTrace: " + ex.StackTrace.ToString());
                    if (ex.InnerException != null)
                    {
                        sw.WriteLine("Inner Exception : " + ex.InnerException.Message);
                        sw.WriteLine("Exception StackTrace: " + ex.InnerException.StackTrace.ToString());
                    }
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                throw new Exception("Error while sending Mail");
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        /// <summary>
        /// Get SQC details
        /// </summary>
        /// <returns></returns>
        public List<SendMailCustomFuncModel> GetSQCData()
        {
            dbContext = new PMGSYEntities();
            List<SendMailCustomFuncModel> lstItems = new List<SendMailCustomFuncModel>();
            try
            {
                var sqcList = dbContext.ADMIN_SQC.Where(c => c.ADMIN_ACTIVE_STATUS.Equals("Y")).OrderBy(c => c.MAST_STATE_CODE).ToList();
                
                foreach (var item in sqcList)
                {
                    SendMailCustomFuncModel eMailModel = new SendMailCustomFuncModel();
                    try
                    {
                        eMailModel.RecepientState = dbContext.MASTER_STATE.Where(c => c.MAST_STATE_CODE == item.MAST_STATE_CODE).Select(c => c.MAST_STATE_NAME).First();
                        eMailModel.RecepientAddress = item.ADMIN_QC_ADDRESS1 + (item.ADMIN_QC_ADDRESS2 == null ? "" :( ", " +item.ADMIN_QC_ADDRESS2));

                        eMailModel.EmailRecepient = item.ADMIN_QC_EMAIL.Trim();
                        eMailModel.EmailSubject = "Kind Attn: \"Meri Sadak - Citizen Feedback System\" Nodal Officer (SQC," + eMailModel.RecepientState + ")";
                        eMailModel.EmailDate = DateTime.Now.ToString();
                        eMailModel.RecepientName = "SQC,";
                        eMailModel.RecepientDesignation = "SQC";
                        eMailModel.MobileWeeklyReportList = dbContext.SP_FEEDBACK_WEEKLY_REPORT(item.MAST_STATE_CODE, 1).ToList<SP_FEEDBACK_WEEKLY_REPORT_Result>();
                        eMailModel.WebWeeklyReportList = dbContext.SP_FEEDBACK_WEEKLY_REPORT(item.MAST_STATE_CODE, 0).ToList<SP_FEEDBACK_WEEKLY_REPORT_Result>();

                        if (eMailModel.MobileWeeklyReportList.Count == 0 && eMailModel.WebWeeklyReportList.Count == 0)
                        {
                            //Dont add to List
                            throw new Exception("No records available for Citizen Feedback System");
                        }

                        lstItems.Add(eMailModel);
                    }
                    catch (Exception ex)
                    {
                        //return nothing or not throwing anything - because on exception of single record, need to continue for others
                        if (!Directory.Exists(ErrorLogDirectory))
                        {
                            Directory.CreateDirectory(ErrorLogDirectory);
                        }

                        using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                        {
                            sw.WriteLine("Date :" + DateTime.Now.ToString());
                            sw.WriteLine("File : " + "MeriSadak_MailToSQCDAL.cs");
                            sw.WriteLine("Method : " + "SendMailCustomFunc()");
                            sw.WriteLine("State name : " + eMailModel.RecepientState);
                            sw.WriteLine("Exception : " + ex.Message);
                            sw.WriteLine("Exception StackTrace: " + ex.StackTrace.ToString());
                            if (ex.InnerException != null)
                            {
                                sw.WriteLine("Inner Exception : " + ex.InnerException.Message);
                                sw.WriteLine("Exception StackTrace: " + ex.InnerException.StackTrace.ToString());
                            }
                            sw.WriteLine("____________________________________________________");
                            sw.Close();
                        }
                    }
                }

                return lstItems;
            }
            catch (Exception ex)
            {
               if (!Directory.Exists(ErrorLogDirectory))
                {
                    Directory.CreateDirectory(ErrorLogDirectory);
                }

                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("File : " + "MeriSadak_MailToSQCDAL.cs");
                    sw.WriteLine("Method : " + "GetSQCData()");
                    sw.WriteLine("Exception : " + ex.Message);
                    sw.WriteLine("Exception StackTrace: " + ex.StackTrace.ToString());
                    if (ex.InnerException != null)
                    {
                        sw.WriteLine("Inner Exception : " + ex.InnerException.Message);
                        sw.WriteLine("Exception StackTrace: " + ex.InnerException.StackTrace.ToString());
                    }
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                return lstItems;
            }
        }
    }
}