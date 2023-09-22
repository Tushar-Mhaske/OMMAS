using iTextSharp.text;
using PMGSY.Areas.RESTImplementationForABAProgress.DAL;
using PMGSY.Areas.RESTImplementationForABAProgress.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;

namespace PMGSY.Controllers
{
    [AllowAnonymous]
    public class RestApiABAProgressController : ApiController
    {
        #region Aatmanirbhar Bharat

        private static string ErrorLogBasePath = System.Configuration.ConfigurationSettings.AppSettings["ABAProgressErrorPath"];
        private static string ErrorLogDirectory = ErrorLogBasePath + "\\" + "ErrorLog_" + DateTime.Now.Month + "_" + DateTime.Now.Year;
        private string ErrorLogFilePath = ErrorLogDirectory + "//ErrorLog_" + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + ".txt";


        public ValidateUserDetailscs User;
        [HttpGet]
        public HttpResponseMessage GetABAProgressDetails(string GeneratedDate)
        {
            ABAProgressDAL objDAL = new ABAProgressDAL();
            string userName = string.Empty;
            string password = string.Empty;
            string generatedDate = string.Empty;

            try
            {
                string authenticationString = Request.Headers.Authorization.Parameter;
                string originalString = Encoding.UTF8.GetString(Convert.FromBase64String(authenticationString));
                //string DecodededDate = HttpUtility.UrlDecode();
                string UserName = originalString.Split(':')[0];
                string Password = originalString.Split(':')[1];  

                //var UserName = Request.Headers.GetValues("UserName") == null ? null : Request.Headers.GetValues("UserName").ElementAt(0);
                //var Password = Request.Headers.GetValues("Password") == null ? null : Request.Headers.GetValues("Password").ElementAt(0);
                var generatedDatetakenFromUser = GeneratedDate;//Request.Headers.GetValues("GeneratedDate") == null ? null : Request.Headers.GetValues("GeneratedDate").ElementAt(0);

                userName = Convert.ToString(UserName);
                password = Convert.ToString(Password);
                generatedDate = Convert.ToString(generatedDatetakenFromUser);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse<string>(HttpStatusCode.NoContent, "Header Values are Missing.");
            }


            //string userName = "Ommas_emarg";
            //string password = "1dab3e49df35d0bdadefe38fd3b6e046f2492ba5eb9d994d7000a36b6f3552c2";
            if (string.IsNullOrEmpty(generatedDate))
            {
                return Request.CreateResponse<string>(HttpStatusCode.LengthRequired, "Generated Date can not be Empty.");
            }

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                return Request.CreateResponse<string>(HttpStatusCode.LengthRequired, "Username and Password can not be Empty.");
            }

            try
            {
                if (true)
                {//
                    string actualPassword = "Zcbm@1590";
                 //   if (ValidateUserDetailscs.validateSHA256Hash(password, actualPassword))
                    if (password.Equals(actualPassword))
                    {//
                        var lstModel = objDAL.GetABAProgressDAL(generatedDate);
                        JsonFormat js = new JsonFormat();
                        js.status = true;
                        js.Result = lstModel;
                        return Request.CreateResponse<JsonFormat>(HttpStatusCode.OK, js);
                    }
                    else
                        return Request.CreateResponse<string>(HttpStatusCode.Unauthorized, "Please Enter Valid Credentials.");
                    // return "Please Enter Valid Credentials.";
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
                    sw.WriteLine("Method : " + "RestApiEmargPushDetailsController.cs - GetABAProgressDetails()");
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


        #region MORD Physical Progress Details
      //  public ValidateUserDetailscs User;
        [HttpGet]
        public HttpResponseMessage GetMordPhysicalDashboardDetails(byte Scheme, int Month, int Year)
        {
            ABAProgressDAL objDAL = new ABAProgressDAL();
            string userName = string.Empty;
            string password = string.Empty;
            byte schemeFromUser = 0;
            int month = 0;
            int year = 0;
            try
            {
                string authenticationString = Request.Headers.Authorization.Parameter;
                string originalString = Encoding.UTF8.GetString(Convert.FromBase64String(authenticationString));
                //string DecodededDate = HttpUtility.UrlDecode();
                string UserName = originalString.Split(':')[0];
                string Password = originalString.Split(':')[1];

                //var UserName = Request.Headers.GetValues("UserName") == null ? null : Request.Headers.GetValues("UserName").ElementAt(0);
                //var Password = Request.Headers.GetValues("Password") == null ? null : Request.Headers.GetValues("Password").ElementAt(0);
                var SchemeFromUser = Scheme;//Request.Headers.GetValues("GeneratedDate") == null ? null : Request.Headers.GetValues("GeneratedDate").ElementAt(0);
                var mon = Month;
                var yr = Year;
                userName = Convert.ToString(UserName);
                password = Convert.ToString(Password);
                schemeFromUser = Convert.ToByte(SchemeFromUser);
                month = Convert.ToInt32(mon);
                year = Convert.ToInt32(yr);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse<string>(HttpStatusCode.NoContent, "Header Values are Missing.");
            }


            //string userName = "Ommas_emarg";
            //string password = "1dab3e49df35d0bdadefe38fd3b6e046f2492ba5eb9d994d7000a36b6f3552c2";
            if (schemeFromUser == null|| month == 0 || year == 0)
            {
                return Request.CreateResponse<string>(HttpStatusCode.LengthRequired, "Scheme, Month or Year can not be Empty.");
            }

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                return Request.CreateResponse<string>(HttpStatusCode.LengthRequired, "Username and Password can not be Empty.");
            }

            try
            {
                if (true)
                {//
                    string actualPassword = "Zcbm@1590";
                    //   if (ValidateUserDetailscs.validateSHA256Hash(password, actualPassword))
                    if (password.Equals(actualPassword))
                    {//
                        var lstModel = objDAL.GetMORDDashboardDAL(schemeFromUser,month,year);
                        JsonFormatForMordDashboard js = new JsonFormatForMordDashboard();
                        js.status = true;
                        js.Result = lstModel;
                        return Request.CreateResponse<JsonFormatForMordDashboard>(HttpStatusCode.OK, js);
                    }
                    else
                        return Request.CreateResponse<string>(HttpStatusCode.Unauthorized, "Please Enter Valid Credentials.");
                    // return "Please Enter Valid Credentials.";
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
                    sw.WriteLine("Method : " + "RestApiABAProgressController.cs - GetMordPhysicalDashboardDetails()");
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


        #region MORD Physical Progress Details PMGSY3
        //  public ValidateUserDetailscs User;
        [HttpGet]
        public HttpResponseMessage GetMordPhysicalDashboardDetailsPMGSY3(byte Scheme, int Month, int Year)
        {
            ABAProgressDAL objDAL = new ABAProgressDAL();
            string userName = string.Empty;
            string password = string.Empty;
            byte schemeFromUser = 0;
            int month = 0;
            int year = 0;

            try
            {
                string authenticationString = Request.Headers.Authorization.Parameter;
                string originalString = Encoding.UTF8.GetString(Convert.FromBase64String(authenticationString));
                //string DecodededDate = HttpUtility.UrlDecode();
                string UserName = originalString.Split(':')[0];
                string Password = originalString.Split(':')[1];

                //var UserName = Request.Headers.GetValues("UserName") == null ? null : Request.Headers.GetValues("UserName").ElementAt(0);
                //var Password = Request.Headers.GetValues("Password") == null ? null : Request.Headers.GetValues("Password").ElementAt(0);
                var SchemeFromUser = Scheme;//Request.Headers.GetValues("GeneratedDate") == null ? null : Request.Headers.GetValues("GeneratedDate").ElementAt(0);
                var mon = Month;
                var yr = Year;

                userName = Convert.ToString(UserName);
                password = Convert.ToString(Password);
                schemeFromUser = Convert.ToByte(SchemeFromUser);
                month = Convert.ToInt32(mon);
                year = Convert.ToInt32(yr);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse<string>(HttpStatusCode.NoContent, "Header Values are Missing.");
            }


            //string userName = "Ommas_emarg";
            //string password = "1dab3e49df35d0bdadefe38fd3b6e046f2492ba5eb9d994d7000a36b6f3552c2";
            if (schemeFromUser == null || month == 0 || year == 0)
            {
                return Request.CreateResponse<string>(HttpStatusCode.LengthRequired, "Scheme,Month,Year can not be Empty.");
            }

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                return Request.CreateResponse<string>(HttpStatusCode.LengthRequired, "Username and Password can not be Empty.");
            }

            try
            {
                if (true)
                {//
                    string actualPassword = "Zcbm@1590";
                    //   if (ValidateUserDetailscs.validateSHA256Hash(password, actualPassword))
                    if (password.Equals(actualPassword))
                    {//
                        var lstModel = objDAL.GetMORDDashboardPMGSY3DAL(schemeFromUser, month, year);
                        JsonFormatForMordDashboardPMGSY3 js = new JsonFormatForMordDashboardPMGSY3();
                        js.status = true;
                        js.Result = lstModel;
                        return Request.CreateResponse<JsonFormatForMordDashboardPMGSY3>(HttpStatusCode.OK, js);
                    }
                    else
                        return Request.CreateResponse<string>(HttpStatusCode.Unauthorized, "Please Enter Valid Credentials.");
                    // return "Please Enter Valid Credentials.";
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
                    sw.WriteLine("Method : " + "RestApiABAProgressController.cs - GetMordPhysicalDashboardDetailsPMGSY3()");
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


        #region State Rank Matrix API
        [HttpGet]
        public HttpResponseMessage GetStateRankDetails()
        { // int State, int Month, int Year
            ABAProgressDAL objDAL = new ABAProgressDAL();
            string userName = string.Empty;
            string password = string.Empty;
            int StateCode = 0;
            int month = 0;
            int year = 0;
            try
            {
                string authenticationString = Request.Headers.Authorization.Parameter;
                string originalString = Encoding.UTF8.GetString(Convert.FromBase64String(authenticationString));
                //string DecodededDate = HttpUtility.UrlDecode();
                string UserName = originalString.Split(':')[0];
                string Password = originalString.Split(':')[1];

                //var UserName = Request.Headers.GetValues("UserName") == null ? null : Request.Headers.GetValues("UserName").ElementAt(0);
                //var Password = Request.Headers.GetValues("Password") == null ? null : Request.Headers.GetValues("Password").ElementAt(0);


                //Request.Headers.GetValues("GeneratedDate") == null ? null : Request.Headers.GetValues("GeneratedDate").ElementAt(0);
                
                
                //var StateCodeDetails = State;
                //var mon = Month;
                //var yr = Year;


                userName = Convert.ToString(UserName);
                password = Convert.ToString(Password);

                StateCode =0;
                month = System.DateTime.Now.Month;
                year = System.DateTime.Now.Year;
            }
            catch (Exception ex)
            {
                return Request.CreateResponse<string>(HttpStatusCode.NoContent, "Header Values are Missing.");
            }


            //string userName = "Ommas_emarg";
            //string password = "1dab3e49df35d0bdadefe38fd3b6e046f2492ba5eb9d994d7000a36b6f3552c2";
            if (month == 0 || year == 0)
            {
                return Request.CreateResponse<string>(HttpStatusCode.LengthRequired, "Month or Year can not be Empty.");
            }

            if (StateCode == null || month == null || year == null)
            {
                return Request.CreateResponse<string>(HttpStatusCode.LengthRequired, "State, Month or Year can not be Empty.");
            }

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                return Request.CreateResponse<string>(HttpStatusCode.LengthRequired, "Username and Password can not be Empty.");
            }

            try
            {
                if (true)
                {//
                    string actualPassword = "Zcbm@1590";
                    //   if (ValidateUserDetailscs.validateSHA256Hash(password, actualPassword))
                    if (password.Equals(actualPassword))
                    {//
                        var lstModel = objDAL.GetStateRankDetailsDAL(StateCode, month, year);
                        JsonFormatForStateRank js = new JsonFormatForStateRank();
                        js.status = true;
                        js.Result = lstModel;
                        return Request.CreateResponse<JsonFormatForStateRank>(HttpStatusCode.OK, js);
                    }
                    else
                        return Request.CreateResponse<string>(HttpStatusCode.Unauthorized, "Please Enter Valid Credentials.");
                    // return "Please Enter Valid Credentials.";
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
                    sw.WriteLine("Method : " + "RestApiABAProgressController.cs - GetStateRankDetails()");
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