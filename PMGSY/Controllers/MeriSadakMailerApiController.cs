using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AttributeRouting.Web.Mvc;
using System.IO;
using PMGSY.Models.Common;
using PMGSY.DAL.MeriSadak_MailToSQC;
using System.Net.Mail;
using System.Configuration;

namespace PMGSY.Controllers
{
    [AllowAnonymous]
    public class MeriSadakMailerApiController : ApiController
    {
        private static string ErrorLogBasePath = System.Configuration.ConfigurationSettings.AppSettings["MABProgressErrorLogPath"];
        private static string ErrorLogDirectory = ErrorLogBasePath + "\\" + "ErrorLog_" + DateTime.Now.Month + "_" + DateTime.Now.Year;
        private string ErrorLogFilePath = ErrorLogDirectory + "//MeriSadak_MailToSQCLog_" + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + ".txt";	

        /// <summary>
        /// Send Mail
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/MeriSadakMailer/SendMail")]
        public string SendMail()
        {
            try
            {
                MeriSadak_MailToSQCDAL objDAL = new MeriSadak_MailToSQCDAL();
                List<SendMailCustomFuncModel> lstItems = new List<SendMailCustomFuncModel>();
                lstItems = objDAL.GetSQCData();
                foreach (var item in lstItems)
                {
                    // Chaged by Srishti on 13-03-2023
                    //objDAL.SendMailCustomFunc(item).Send();

                    // Added by Srishti on 13-03-2023
                    ServicePointManager.ServerCertificateValidationCallback += (s, ce, ca, p) => true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    MailMessage ms = objDAL.SendMailCustomFunc(item);

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
                }
                return "success";
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
                    sw.WriteLine("File : " + "MeriSadakMailerApiController.cs");
                    sw.WriteLine("Method : " + "SendMail()");
                    sw.WriteLine("Exception : " + ex.Message);
                    sw.WriteLine("Exception StackTrace: " + ex.StackTrace.ToString());
                    if (ex.InnerException != null)
                    {
                        sw.WriteLine("Inner Exception : " + ex.InnerException.Message);
                        sw.WriteLine("Exception StackTrace: " + (ex.InnerException.StackTrace != null ? ex.InnerException.StackTrace.ToString() : ""));
                    }
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                return "Error while sending mail (In Controller)";
            }
            
        }
    }
}