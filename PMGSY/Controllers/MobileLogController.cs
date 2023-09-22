using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Controllers
{
    // This controller is required to write mobile applocation error log to the text file.
    // No [RequiredAuthentication] attribute applicable for the controller, because its usage is out of Session.
    public class MobileLogController : Controller
    {
        private static string ErrorLogBasePath = System.Configuration.ConfigurationSettings.AppSettings["MABQMSErrorLogPath"];
        private static string ErrorLogDirectory = ErrorLogBasePath + "\\" + "MobErrorLog_" + DateTime.Now.Month + "_" + DateTime.Now.Year;
        private string ErrorLogFilePath = ErrorLogDirectory + "//MobileErrorLog_" + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + ".txt";	
        

        /// <summary>
        /// Writes StackTrace for Mobile Errors
        /// Mobile application calls this action when any error/exception occurrs.
        /// This accpets InputStream in Request & write down to file
        /// </summary>
        public void WriteMobileLog()
        {
            try
            {
                if (!Directory.Exists(ErrorLogBasePath))
                {
                    Directory.CreateDirectory(ErrorLogBasePath);
                }
                if (!Directory.Exists(ErrorLogDirectory))
                {
                    Directory.CreateDirectory(ErrorLogDirectory);
                }

                using (StreamWriter sw = System.IO.File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    // Code to write json post data on file.
                    System.IO.StreamReader reader = new System.IO.StreamReader(HttpContext.Request.InputStream);
                    string requestFromPost = reader.ReadToEnd();
                    string[] splitArr = null;
                    splitArr = requestFromPost.Split(',');
                    int count = 1;
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("------------------------------------------");
                    foreach (string str in splitArr)
                    {
                        sw.WriteLine(count + ".    " + str);
                        sw.WriteLine();
                        count++;
                    }
                    sw.WriteLine("");
                    sw.WriteLine("");
                    sw.WriteLine("");
                    sw.Close();
                }

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);            
                using (StreamWriter sw = System.IO.File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Exception : " + ex.Message);
                    if (ex.InnerException != null)
                        sw.WriteLine("Inner Exception : " + ex.InnerException.Message);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
            }
        }

    }
}
