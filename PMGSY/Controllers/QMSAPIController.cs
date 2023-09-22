using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using PMGSY.Areas.QMSApi.DAL;
using PMGSY.Areas.QMSApi;
using PMGSY.Areas.QMSApi.Models;

namespace PMGSY.Controllers
{
    [AllowAnonymous]
    public class QMSAPIController : ApiController
    {
        #region Download Schedule 

        private static string ErrorLogBasePath = System.Configuration.ConfigurationSettings.AppSettings["ABAProgressErrorPath"];
        private static string ErrorLogDirectory = ErrorLogBasePath + "\\" + "ErrorLog_" + DateTime.Now.Month + "_" + DateTime.Now.Year;
        private string ErrorLogFilePath = ErrorLogDirectory + "//ErrorLog_" + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + ".txt";


       // public ValidateUserDetailscs User;
        [HttpGet]
        public HttpResponseMessage DownloadScheduleAPI(string monitorCode)
        {
            QMSApiDAL objDAL = new QMSApiDAL();
            string userName = string.Empty;
            string password = string.Empty;
            Int32 monitorCodeFinal = 0;

            try
            {
                string authenticationString = Request.Headers.Authorization.Parameter;
                string originalString = Encoding.UTF8.GetString(Convert.FromBase64String(authenticationString));
                //string DecodededDate = HttpUtility.UrlDecode();
                string UserName = originalString.Split(':')[0];
                string Password = originalString.Split(':')[1];

                if (string.IsNullOrEmpty(monitorCode))
                {
                    return Request.CreateResponse<string>(HttpStatusCode.LengthRequired, "Monitor Code can not be Empty.");
                }

                if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password))
                {
                    return Request.CreateResponse<string>(HttpStatusCode.LengthRequired, "Username and Password can not be Empty.");
                }
                //var generatedDatetakenFromUser = GeneratedDate;//Request.Headers.GetValues("GeneratedDate") == null ? null : Request.Headers.GetValues("GeneratedDate").ElementAt(0);

                userName = Convert.ToString(UserName);
                password = Convert.ToString(Password);
                monitorCodeFinal = Convert.ToInt32(monitorCode);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse<string>(HttpStatusCode.NoContent, "Header Values are Missing.");
            }


            try
            {
                    string actualPassword = "Qms@@321";
                    if (password.Equals(actualPassword))
                    {
                        var lstModel = objDAL.DownloadScheduleDAL(monitorCodeFinal);
                        JsonFormat js = new JsonFormat();
                        js.status = true;
                        js.Result = lstModel;
                        return Request.CreateResponse<JsonFormat>(HttpStatusCode.OK, js);
                    }
                    else
                        return Request.CreateResponse<string>(HttpStatusCode.Unauthorized, "Please Enter Valid Credentials.");
                    
                
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
                    sw.WriteLine("Method : " + "QMSAPIController.cs - DownloadScheduleAPI()");
                    sw.WriteLine("Exception : " + ex.ToString());
                    sw.WriteLine("Exception : " + ex.StackTrace);
                    if (ex.InnerException != null)
                        sw.WriteLine("Inner Exception : " + ex.InnerException.ToString());
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                return Request.CreateResponse<string>(HttpStatusCode.ServiceUnavailable, "Error occured while processing your request.");


            }
        }
        #endregion


    }
}
