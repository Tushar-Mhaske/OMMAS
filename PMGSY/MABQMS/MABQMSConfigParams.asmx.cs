#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   MABQMSConfigParams.asmx.cs        
        * Description   :   Web service for Quality Monitoring System Mobile Application's configuration parameters, based on a fixed url.
        * Author        :   Shyam Yadav 
        * Creation Date :   30/Sep/2013
 **/
#endregion

using PMGSY.DAL.MABQMS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace PMGSY.MABQMS
{
    /// <summary>
    /// Summary description for MABQMSConfigParams
    /// </summary>
    [WebService(Namespace = "https://online.omms.nic.in/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class MABQMSConfigParams : System.Web.Services.WebService
    {
        MABQMSDAL objMABQMSDAL = null;
        public static string key = "^%V{T%&]@08_01-";
        RijndaelCrypt cryptObj = new RijndaelCrypt(key);
        private static string ErrorLogBasePath = System.Configuration.ConfigurationSettings.AppSettings["MABQMSErrorLogPath"];
        private static string ErrorLogDirectory = ErrorLogBasePath + "\\" + "ErrorLog_" + DateTime.Now.Month + "_" + DateTime.Now.Year;
        private string ErrorLogFilePath = ErrorLogDirectory + "//MABQMSErrorLog_" + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + ".txt";	

       
        /// <summary>
        /// Get all configuration parameters
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="imei"></param>
        /// <returns></returns>
        [WebMethod]
        public string GetConfigDetails(string userName, string imei) 
        {
            try
            {
                objMABQMSDAL = new MABQMSDAL();
                string decUserName = (userName == null || userName.Trim().Equals("")) ? null : cryptObj.Decrypt(userName);
                string decImei = cryptObj.Decrypt(imei);
                return cryptObj.Encrypt(objMABQMSDAL.GetConfigParameters(decUserName, decImei));
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Webmethod : " + "GetConfigDetails()");
                    sw.WriteLine("Database Exception : " + ex.Message);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }


                return cryptObj.Encrypt("-1");
            }
        }


        /// <summary>
        /// Get all Image Descriptions
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public string GetImageDescription()
        {
            try
            {
                objMABQMSDAL = new MABQMSDAL();
                return cryptObj.Encrypt(objMABQMSDAL.GetImageDescription());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Webmethod : " + "GetImageDescription()");
                    sw.WriteLine("Database Exception : " + ex.Message);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }


                return cryptObj.Encrypt("-1");
            }
        }
    }
}
