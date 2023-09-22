using PMGSY.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.QMSApi.DAL
{
    public class QMSApiDAL
    {
        #region Download Schedule DAL 

        private static string ErrorLogBasePath = System.Configuration.ConfigurationSettings.AppSettings["ABAProgressErrorPath"];
        private static string ErrorLogDirectory = ErrorLogBasePath + "\\" + "ErrorLog_" + DateTime.Now.Month + "_" + DateTime.Now.Year;
        private string ErrorLogFilePath = ErrorLogDirectory + "//ErrorLog_" + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + ".txt";


        PMGSYEntities dbContext = null;

        public dynamic DownloadScheduleDAL(int monitorCode)
        {
            dbContext = new PMGSYEntities();
            int month = DateTime.Now.Month;
            int year = DateTime.Now.Year;
            try
            {
                ((System.Data.Entity.Infrastructure.IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;

                var scheduleDetails = dbContext.USP_MABQMS_DOWNLOAD_SCHEDULE(monitorCode, month, year).ToList();

                return scheduleDetails;

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
                    sw.WriteLine("Method : " + "QMSApi.DAL.QMSApiDAL.DownloadScheduleDAL()");
                    sw.WriteLine("Exception : " + ex.ToString());
                    sw.WriteLine("Exception : " + ex.StackTrace);
                    if (ex.InnerException != null)
                        sw.WriteLine("Inner Exception : " + ex.InnerException.ToString());
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion 
    }
}