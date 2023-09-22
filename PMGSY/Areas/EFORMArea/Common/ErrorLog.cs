using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.Common
{
    public class ErrorLog
    {
        public static void LogError(Exception err, string methodName)
        {
            string errorLogPath = System.Configuration.ConfigurationSettings.AppSettings["OMMASErrorLogPath"];
            if (!Directory.Exists(errorLogPath))
            {
                Directory.CreateDirectory(errorLogPath);
            }
            using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine(errorLogPath + "\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
            {
                sw.WriteLine("---------------------------------------------------------------------------------------");
                sw.WriteLine("Date :" + DateTime.Now.ToString());
                sw.WriteLine("Method : " + methodName);
                if (PMGSY.Extensions.PMGSYSession.Current != null)
                {
                    sw.WriteLine("UserName : " + PMGSY.Extensions.PMGSYSession.Current.UserName);
                }
                if (err != null)
                {
                    sw.WriteLine("Exception : " + err.ToString());
                }
                if (err.InnerException != null)
                {
                    sw.WriteLine("InnerException : " + err.InnerException.ToString());
                }
                //if (err.InnerException.InnerException != null)
                //{
                //    sw.WriteLine("InnerException.InnerException : " + err.InnerException.InnerException.ToString());
                //}
                sw.WriteLine("---------------------------------------------------------------------------------------");
                sw.Close();
            }
        }


    }
}