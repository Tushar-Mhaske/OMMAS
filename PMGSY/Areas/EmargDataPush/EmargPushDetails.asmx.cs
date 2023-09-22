using PMGSY.Common;
using PMGSY.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.Services.Protocols;

namespace PMGSY.Areas.EmargDataPush
{
    /// <summary>
    /// Summary description for EmargPushDetails
    /// </summary>
    
    [WebService(Namespace = "https://online.omms.nic.in/")]
//    [WebService(Namespace = "http://localhost:41666/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class EmargPushDetails : System.Web.Services.WebService
    {
        private static string ErrorLogBasePath = System.Configuration.ConfigurationSettings.AppSettings["EmargPushDetailsErrorPath"];
        private static string ErrorLogDirectory = ErrorLogBasePath + "\\" + "ErrorLog_" + DateTime.Now.Month + "_" + DateTime.Now.Year;
        private string ErrorLogFilePath = ErrorLogDirectory + "//ErrorLog_" + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + ".txt";


      public ValidateUserDetailscs User;
      [SoapHeader("User", Required = true)]
      [WebMethod]
      public string GetEmargDetails(string userName, string password, string PackageId)
        {
            EmargPushDetailsDAL objDAL = new EmargPushDetailsDAL();
            
          
            string finalPackageID = string.Empty;
            if (string.IsNullOrEmpty(PackageId))
            {
                return "Please Enter Package ID.";
            }
            else
            {
                 finalPackageID = PackageId.Trim();
            }

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                return "Username and Password can not be Empty.";
            }
            


            try
            {
                if (true)
                {//
                    string actualPassword = "Zcbm@1590";
                    if (ValidateUserDetailscs.validateSHA256Hash(password, actualPassword))
                    {//
                       var lstModel = objDAL.GetCorrectedEmargDetailsDAL(finalPackageID);
                        var serializer = new JavaScriptSerializer();
                        serializer.MaxJsonLength = Int32.MaxValue;
                        return (serializer.Serialize(lstModel));
                    }
                    else
                        return "Please Enter Valid Credentials.";
                }
                //else
                //{
                //    return "Please Enter Credentials.";
                //}
                //return model;
                //HttpContext.Current.Response.Write("[" + new JavaScriptSerializer().Serialize(model) + "]");
                //HttpContext.Current.Response.Write(new JavaScriptSerializer().Serialize(model));
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
                    sw.WriteLine("Method : " + "EmargPushDetails.asmx.cs - GetEmargDetails()");
                    sw.WriteLine("Exception : " + ex.ToString());
                    sw.WriteLine("Exception : " + ex.StackTrace);
                    if (ex.InnerException != null)
                        sw.WriteLine("Inner Exception : " + ex.InnerException.ToString());
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                return "Error occured while processing your request.";
            }
        }

        #region GetDRRPRoadDetails ---Added by Hrishikesh--
        [SoapHeader("User", Required = true)]
        [WebMethod]
        public string GetDRRPRoadDetails(string key, string token, int State_Code)
        {
            EmargPushDetailsDAL objDAL = new EmargPushDetailsDAL();

            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(token) || string.IsNullOrEmpty(Convert.ToString(State_Code)))
            {
                return "Key, Token and State Code can not be Empty.";
            }
            try
            {
                if (true)
                {
                    string actualKey = "DRRPRoad@9804";
                    string actualToken = "Zcbm@1590";
                    if (ValidateUserDetailscs.validateSHA256Hash(key, actualKey) && ValidateUserDetailscs.validateSHA256Hash(token, actualToken))
                    {
                        //var lstModel = objDAL.GetDRRPRoadDetailsDAL();
                        List<MasterDRRPRoadDetailsModel> lstModel = objDAL.GetDRRPRoadDetailsDAL(State_Code);
                        var serializer = new JavaScriptSerializer();
                        serializer.MaxJsonLength = Int32.MaxValue;

                        return (serializer.Serialize(lstModel));
                    }
                    else
                        return "Please Enter Valid Credentials.";
                }

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
                    sw.WriteLine("Method : " + "EmargPushDetails.asmx.cs - GetDRRPRoadDetails()");
                    sw.WriteLine("Exception : " + ex.ToString());
                    sw.WriteLine("Exception : " + ex.StackTrace);
                    if (ex.InnerException != null)
                        sw.WriteLine("Inner Exception : " + ex.InnerException.ToString());
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                return "Error occured while processing your request.";
            }
        }
        #endregion


    }








}
