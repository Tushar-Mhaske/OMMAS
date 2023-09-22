using iTextSharp.text;
using PMGSY.Areas.EmargDataPush.Models;
using PMGSY.Areas.RESTAPIImplementation.DAL;
using PMGSY.Areas.RESTAPIImplementation.Models;
using PMGSY.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PMGSY.Controllers
{
    [AllowAnonymous]
    public class RestApiEmargPushDetailsController : ApiController
    {
        private static string ErrorLogBasePath = System.Configuration.ConfigurationSettings.AppSettings["EmargPushDetailsErrorPath"];
        private static string ErrorLogDirectory = ErrorLogBasePath + "\\" + "ErrorLog_" + DateTime.Now.Month + "_" + DateTime.Now.Year;
        private string ErrorLogFilePath = ErrorLogDirectory + "//ErrorLog_" + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + ".txt";


        public ValidateUserDetailscs User;

        #region Push Corrected Packages to Emarg
        [HttpGet]
        public HttpResponseMessage GetEmargDetails()
        {
            EmargPushDetailsDAL objDAL = new EmargPushDetailsDAL();
            string userName = string.Empty;
            string password = string.Empty;
            string PackageId = string.Empty;

            try
            {
                var UserName = Request.Headers.GetValues("UserName") == null ? null : Request.Headers.GetValues("UserName").ElementAt(0);
                var Password = Request.Headers.GetValues("Password") == null ? null : Request.Headers.GetValues("Password").ElementAt(0);
                var PackageID = Request.Headers.GetValues("PackageID") == null ? null : Request.Headers.GetValues("PackageID").ElementAt(0);

                userName = Convert.ToString(UserName);
                password = Convert.ToString(Password);
                PackageId = Convert.ToString(PackageID);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse<string>(HttpStatusCode.NoContent, "Header Values are Missing.");
            }


            //string userName = "Ommas_emarg";
            //string password = "1dab3e49df35d0bdadefe38fd3b6e046f2492ba5eb9d994d7000a36b6f3552c2";
            //string PackageId = "0";



            string finalPackageID = string.Empty;
            if (string.IsNullOrEmpty(PackageId))
            {
                // return "Please Enter Package ID.";
                return Request.CreateResponse<string>(HttpStatusCode.LengthRequired, "Please Enter Package ID.");
            }
            else
            {
                finalPackageID = PackageId.Trim();
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
                    if (ValidateUserDetailscs.validateSHA256Hash(password, actualPassword))
                    {//
                        var lstModel = objDAL.GetCorrectedEmargDetailsDAL(finalPackageID);
                        // var serializer = new JavaScriptSerializer();
                        // serializer.MaxJsonLength = Int32.MaxValue;
                        //  return (serializer.Serialize(lstModel));
                        return Request.CreateResponse<List<PMGSY.Models.USP_PUSH_CORRECTED_EMARG_DETAILS_Result>>(HttpStatusCode.OK, (List<PMGSY.Models.USP_PUSH_CORRECTED_EMARG_DETAILS_Result>)lstModel);
                    }
                    else
                        return Request.CreateResponse<string>(HttpStatusCode.Unauthorized, "Please Enter Valid Credentials.");
                    // return "Please Enter Valid Credentials.";
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
                return Request.CreateResponse<string>(HttpStatusCode.ServiceUnavailable, "Error occured while processing your request.");


            }
        }
        #endregion

        //Added on 02-08-2022
        #region Push Corrected Packages to Emarg Post DLP
        [HttpGet]
        public HttpResponseMessage GetEmargDetailsPostDLP()
        {
            EmargPushDetailsDAL objDAL = new EmargPushDetailsDAL();
            string userName = string.Empty;
            string password = string.Empty;
            //string PackageId = string.Empty;
            int RoadCode = -1;

            try
            {
                var UserName = Request.Headers.GetValues("UserName") == null ? null : Request.Headers.GetValues("UserName").ElementAt(0);
                var Password = Request.Headers.GetValues("Password") == null ? null : Request.Headers.GetValues("Password").ElementAt(0);
                //var PackageID = Request.Headers.GetValues("PackageID") == null ? null : Request.Headers.GetValues("PackageID").ElementAt(0);
                var RoadCode1 = Request.Headers.GetValues("RoadCode") == null ? null : Request.Headers.GetValues("RoadCode").ElementAt(0);

                userName = Convert.ToString(UserName);
                password = Convert.ToString(Password);
                RoadCode = Convert.ToInt32(RoadCode1);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse<string>(HttpStatusCode.NoContent, "Header Values are Missing.");
            }


            //string userName = "Ommas_emarg";
            //string password = "1dab3e49df35d0bdadefe38fd3b6e046f2492ba5eb9d994d7000a36b6f3552c2";
            //string PackageId = "0";



            //string finalPackageID = string.Empty;
            int finalRoadCode = -1;
            if (RoadCode == -1)
            {
                // return "Please Enter Package ID.";
                return Request.CreateResponse<string>(HttpStatusCode.LengthRequired, "Please Enter Road Code.");
            }
            else
            {
                finalRoadCode = RoadCode;
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
                    if (ValidateUserDetailscs.validateSHA256Hash(password, actualPassword))
                    {//
                        var lstModel = objDAL.GetCorrectedEmargDetailsPostDLPDAL(finalRoadCode);
                        // var serializer = new JavaScriptSerializer();
                        // serializer.MaxJsonLength = Int32.MaxValue;
                        //  return (serializer.Serialize(lstModel));
                        return Request.CreateResponse<List<PMGSY.Models.USP_PUSH_CORRECTED_EMARG_DETAILS_POST_DLP_Result>>(HttpStatusCode.OK, (List<PMGSY.Models.USP_PUSH_CORRECTED_EMARG_DETAILS_POST_DLP_Result>)lstModel);
                    }
                    else
                        return Request.CreateResponse<string>(HttpStatusCode.Unauthorized, "Please Enter Valid Credentials.");
                    // return "Please Enter Valid Credentials.";
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
                    sw.WriteLine("Method : " + "EmargPushDetails.asmx.cs - GetEmargDetailsPostDLP()");
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

        #region Push PACKAGE PIU MAPPING to Emarg

        [HttpGet]
        public HttpResponseMessage GetPackagePIUMappingEmargDetails()
        {
            EmargPushDetailsDAL objDAL = new EmargPushDetailsDAL();
            string userName = string.Empty;
            string password = string.Empty;
            //string PackageId = string.Empty;

            try
            {
                var UserName = Request.Headers.GetValues("UserName") == null ? null : Request.Headers.GetValues("UserName").ElementAt(0);
                var Password = Request.Headers.GetValues("Password") == null ? null : Request.Headers.GetValues("Password").ElementAt(0);
               // var PackageID = Request.Headers.GetValues("PackageID") == null ? null : Request.Headers.GetValues("PackageID").ElementAt(0);

                userName = Convert.ToString(UserName);
                password = Convert.ToString(Password);
                //PackageId = Convert.ToString(PackageID);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse<string>(HttpStatusCode.NoContent, "Header Values are Missing.");
            }


            //string userName = "Ommas_emarg";
            //string password = "1dab3e49df35d0bdadefe38fd3b6e046f2492ba5eb9d994d7000a36b6f3552c2";
            //string PackageId = "0";



            string finalPackageID = string.Empty;
            /*if (string.IsNullOrEmpty(PackageId))
            {
                
                return Request.CreateResponse<string>(HttpStatusCode.LengthRequired, "Please Enter Package ID.");
            }
            else
            {
                finalPackageID = PackageId.Trim();
            }*/

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                return Request.CreateResponse<string>(HttpStatusCode.LengthRequired, "Username and Password can not be Empty.");
            }

            try
            {
                if (true)
                {
                    string actualPassword = "Zcbm@1590";
                    if (ValidateUserDetailscs.validateSHA256Hash(password, actualPassword))
                    {
                        var lstModel = objDAL.GetPIUMappingEmargDetailsDAL();

                        return Request.CreateResponse<List<PMGSY.Models.USP_EMARG_PIU_MAPPING_Result>>(HttpStatusCode.OK, (List<PMGSY.Models.USP_EMARG_PIU_MAPPING_Result>)lstModel);
                    }
                    else
                        return Request.CreateResponse<string>(HttpStatusCode.Unauthorized, "Please Enter Valid Credentials.");

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
                    sw.WriteLine("Method : " + "EmargPushDetails.asmx.cs - GetPackagePIUMappingEmargDetails()");
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


        #region Push Acknowledgement of Second Level Service to Emarg
        public HttpResponseMessage GetSecondLevelServiceAcknowledgementDetails()
        {
            EmargPushDetailsDAL objDAL = new EmargPushDetailsDAL();
            string userName = string.Empty;
            string password = string.Empty;
            string PackageId = string.Empty;

            try
            {
                var UserName = Request.Headers.GetValues("UserName") == null ? null : Request.Headers.GetValues("UserName").ElementAt(0);
                var Password = Request.Headers.GetValues("Password") == null ? null : Request.Headers.GetValues("Password").ElementAt(0);
                var PackageID = Request.Headers.GetValues("PackageID") == null ? null : Request.Headers.GetValues("PackageID").ElementAt(0);

                userName = Convert.ToString(UserName);
                password = Convert.ToString(Password);
                PackageId = Convert.ToString(PackageID);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse<string>(HttpStatusCode.NoContent, "Header Values are Missing.");
            }

            string finalPackageID = string.Empty;
            if (string.IsNullOrEmpty(PackageId))
            {
                return Request.CreateResponse<string>(HttpStatusCode.LengthRequired, "Please Enter Package ID.");
            }
            else
            {
                finalPackageID = PackageId.Trim();
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
                    if (ValidateUserDetailscs.validateSHA256Hash(password, actualPassword))
                    {//
                        var lstModel = objDAL.GetSecondLevelAckDAL(finalPackageID);

                        return Request.CreateResponse<List<PMGSY.Models.USP_PUSH_ACK_SECOND_LEVEL_SERVICE_Result>>(HttpStatusCode.OK, (List<PMGSY.Models.USP_PUSH_ACK_SECOND_LEVEL_SERVICE_Result>)lstModel);
                    }
                    else
                        return Request.CreateResponse<string>(HttpStatusCode.Unauthorized, "Please Enter Valid Credentials.");

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
                    sw.WriteLine("Method : " + "EmargPushDetails.asmx.cs - GetSecondLevelServiceAcknowledgementDetails()");
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


        #region Push Acknowledgement of First Level Service to Emarg
        public HttpResponseMessage GetFirstLevelServiceAcknowledgementDetails()
        {
            EmargPushDetailsDAL objDAL = new EmargPushDetailsDAL();
            string userName = string.Empty;
            string password = string.Empty;
            string PackageId = string.Empty;

            try
            {
                var UserName = Request.Headers.GetValues("UserName") == null ? null : Request.Headers.GetValues("UserName").ElementAt(0);
                var Password = Request.Headers.GetValues("Password") == null ? null : Request.Headers.GetValues("Password").ElementAt(0);
                var PackageID = Request.Headers.GetValues("PackageID") == null ? null : Request.Headers.GetValues("PackageID").ElementAt(0);

                userName = Convert.ToString(UserName);
                password = Convert.ToString(Password);
                PackageId = Convert.ToString(PackageID);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse<string>(HttpStatusCode.NoContent, "Header Values are Missing.");
            }

            string finalPackageID = string.Empty;
            if (string.IsNullOrEmpty(PackageId))
            {
                return Request.CreateResponse<string>(HttpStatusCode.LengthRequired, "Please Enter Package ID.");
            }
            else
            {
                finalPackageID = PackageId.Trim();
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
                    if (ValidateUserDetailscs.validateSHA256Hash(password, actualPassword))
                    {//
                        var lstModel = objDAL.GetFirstLevelAckDAL(finalPackageID);

                        return Request.CreateResponse<List<PMGSY.Models.USP_PUSH_ACK_FIRST_LEVEL_SERVICE_Result>>(HttpStatusCode.OK, (List<PMGSY.Models.USP_PUSH_ACK_FIRST_LEVEL_SERVICE_Result>)lstModel);
                    }
                    else
                        return Request.CreateResponse<string>(HttpStatusCode.Unauthorized, "Please Enter Valid Credentials.");

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
                    sw.WriteLine("Method : " + "EmargPushDetails.asmx.cs - GetFirstLevelServiceAcknowledgementDetails()");
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


        #region Authorization Details
        [HttpGet]
        public HttpResponseMessage AuthorizationDetails()
        {
            EmargPushDetailsDAL objDAL = new EmargPushDetailsDAL();
            DateTime auth_date;
            String Auth_Type;

            Auth_Type = Request.Headers.GetValues("Auth_Type") == null ? null : Request.Headers.GetValues("Auth_Type").ElementAt(0);
            var Auth_Date = Request.Headers.GetValues("Auth_Date") == null ? null : Request.Headers.GetValues("Auth_Date").ElementAt(0);
            if (!DateTime.TryParseExact(Auth_Date, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out auth_date))
            {
                return Request.CreateResponse<string>(HttpStatusCode.BadRequest, "Invalid Date.DateFormat should be:dd/MM/yyyy");
            }

            try
            {
                if (auth_date != null && Auth_Type != null && ((Auth_Type == "I") || (Auth_Type == "O") || (Auth_Type == "W")))
                {
                    var lstModel = objDAL.AuthorizationDetailsDAL(Auth_Type, auth_date);

                    return Request.CreateResponse<List<PMGSY.Models.USP_EMARG_SRVC_PUSH_AUTHORIZATION_DETAILS_Result>>(HttpStatusCode.OK, (List<PMGSY.Models.USP_EMARG_SRVC_PUSH_AUTHORIZATION_DETAILS_Result>)lstModel);

                }
                else
                    return Request.CreateResponse<string>(HttpStatusCode.Unauthorized, "Please Enter Valid Auth Type either 'I','O' or 'W'.");
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
                    sw.WriteLine("Method : " + "EmargPushDetails.asmx.cs - AuthorizationDetails()");
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

        #endregion Authorization Details



        #region Payment Details
        [HttpPost]
        public HttpResponseMessage GetEmargPaymentDetails([FromBody] List<PaymentEmargDetails> EmargPaymentDetails)
        {
            EmargPushDetailsDAL objdal = new EmargPushDetailsDAL();
            try
            {
                if (EmargPaymentDetails != null)
                {
                    int result = objdal.getemargpaymentdetailsDAL(EmargPaymentDetails);
                    if (result > 0)
                    {
                        return Request.CreateResponse<string>(HttpStatusCode.OK, "Success");
                    }
                    else
                    {
                        return Request.CreateResponse<string>(HttpStatusCode.BadRequest, " Failure");
                    }
                }
                return Request.CreateResponse<string>(HttpStatusCode.BadRequest, "Emarg Payment Details is null or Incorrect JSON Syntax");
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
                    sw.WriteLine("Method : " + "EmargPushDetails.asmx.cs - GetEmargPaymentDetails()");
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


        #endregion Payment Details



        #region PIU MASTER
        [HttpGet]
        public HttpResponseMessage PIUMasterDetails()
        {
            EmargPushDetailsDAL objDAL = new EmargPushDetailsDAL();

            try
            {

                var lstModel = objDAL.PIUMasterDetailsDAL();

                return Request.CreateResponse<List<PIUMasterViewModel>>(HttpStatusCode.OK, (List<PIUMasterViewModel>)lstModel);

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
                    sw.WriteLine("Method : " + "RestAPIEmargPushDetailsController.cs - PIUMasterDetails()");
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



        #region Get Emarg package Data  : omms.USP_ROAD_PIU_DETAILS_EMARG
        [HttpGet]
        public HttpResponseMessage GetRoadPiuEmargDetails()
        {
            EmargPushDetailsDAL objDAL = new EmargPushDetailsDAL();
            string userName = string.Empty;
            string password = string.Empty;
            //    string PackageId = string.Empty;

            try
            {
                var UserName = Request.Headers.GetValues("UserName") == null ? null : Request.Headers.GetValues("UserName").ElementAt(0);
                var Password = Request.Headers.GetValues("Password") == null ? null : Request.Headers.GetValues("Password").ElementAt(0);
                //var PackageID = Request.Headers.GetValues("PackageID") == null ? null : Request.Headers.GetValues("PackageID").ElementAt(0);

                userName = Convert.ToString(UserName);
                password = Convert.ToString(Password);
                //  PackageId = Convert.ToString(PackageID);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse<string>(HttpStatusCode.NoContent, "Header Values are Missing.");
            }


            //string userName = "Ommas_emarg";
            //string password = "1dab3e49df35d0bdadefe38fd3b6e046f2492ba5eb9d994d7000a36b6f3552c2";
            //string PackageId = "0";



            //string finalPackageID = string.Empty;
            //if (string.IsNullOrEmpty(PackageId))
            //{
            //    // return "Please Enter Package ID.";
            //    return Request.CreateResponse<string>(HttpStatusCode.LengthRequired, "Please Enter Package ID.");
            //}
            //else
            //{
            //    finalPackageID = PackageId.Trim();
            //}

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                return Request.CreateResponse<string>(HttpStatusCode.LengthRequired, "Username and Password can not be Empty.");
            }

            try
            {
                if (true)
                {//
                    string actualPassword = "Zcbm@1590";
                    if (ValidateUserDetailscs.validateSHA256Hash(password, actualPassword))
                    {//
                        var lstModel = objDAL.GetRoadPiuEmargDetailsDAL();

                        return Request.CreateResponse<List<PMGSY.Models.USP_ROAD_PIU_DETAILS_EMARG_Result>>(HttpStatusCode.OK, (List<PMGSY.Models.USP_ROAD_PIU_DETAILS_EMARG_Result>)lstModel);
                    }
                    else
                        return Request.CreateResponse<string>(HttpStatusCode.Unauthorized, "Please Enter Valid Credentials.");
                    // return "Please Enter Valid Credentials.";
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
                    sw.WriteLine("Method : " + "EmargPushDetails.asmx.cs - GetRoadPiuEmargDetails()");
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



        #region STATE MASTER
        [HttpGet]
        public HttpResponseMessage STATEMasterDetails()
        {
            EmargPushDetailsDAL objDAL = new EmargPushDetailsDAL();

            try
            {

                var lstModel = objDAL.STATEMasterDetailsDAL();

                return Request.CreateResponse<List<STATEMasterViewModel>>(HttpStatusCode.OK, (List<STATEMasterViewModel>)lstModel);

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
                    sw.WriteLine("Method : " + "RestAPIEmargPushDetailsController.cs - STATEMasterDetails()");
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

         #region DISTRICT MASTER
        [HttpGet]
        public HttpResponseMessage DISTRICTMasterDetails()
        {
            EmargPushDetailsDAL objDAL = new EmargPushDetailsDAL();

            try
            {

                var lstModel = objDAL.DISTRICTMasterDetailsDAL();

                return Request.CreateResponse<List<DISTRICTMasterViewModel>>(HttpStatusCode.OK, (List<DISTRICTMasterViewModel>)lstModel);

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
                    sw.WriteLine("Method : " + "RestAPIEmargPushDetailsController.cs - DISTRICTMasterDetails()");
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

        #region BLOCK MASTER
        [HttpGet]
        public HttpResponseMessage BLOCKMasterDetails()
        {
            EmargPushDetailsDAL objDAL = new EmargPushDetailsDAL();

            try
            {

                var lstModel = objDAL.BLOCKMasterDetailsDAL();

                return Request.CreateResponse<List<BLOCKMasterViewModel>>(HttpStatusCode.OK, (List<BLOCKMasterViewModel>)lstModel);

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
                    sw.WriteLine("Method : " + "RestAPIEmargPushDetailsController.cs - BLOCKMasterDetails()");
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


        #region Emarg Acknowledgement
        [HttpGet]
        public HttpResponseMessage GetEmargAcknowledgement()
        {
            EmargPushDetailsDAL objDAL = new EmargPushDetailsDAL();
            string userName = string.Empty;
            string password = string.Empty;
            string VoucherNo = string.Empty;

            try
            {
                var UserName = Request.Headers.GetValues("UserName") == null ? null : Request.Headers.GetValues("UserName").ElementAt(0);
                var Password = Request.Headers.GetValues("Password") == null ? null : Request.Headers.GetValues("Password").ElementAt(0);
                var Vouchers = Request.Headers.GetValues("Vouchers") == null ? null : Request.Headers.GetValues("Vouchers").ElementAt(0);

                userName = Convert.ToString(UserName);
                password = Convert.ToString(Password);
                VoucherNo = Convert.ToString(Vouchers);


            }
            catch (Exception ex)
            {
                return Request.CreateResponse<string>(HttpStatusCode.NoContent, "Header Values are Missing.");
            }

            //string userName = "Ommas_emarg";
            //string password = "Zcbm@1590";

            string[] splitVoucher = new string[] { };

            if (string.IsNullOrEmpty(VoucherNo))
            {
                return Request.CreateResponse<string>(HttpStatusCode.LengthRequired, "Please Enter Vouchers.");
            }
            else
            {
                splitVoucher = VoucherNo.Split(',');

            }

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                return Request.CreateResponse<string>(HttpStatusCode.LengthRequired, "Username and Password can not be Empty.");
            }

            try
            {
                if (true)
                {
                    string actualPassword = "Zcbm@1590";
                    if (password.Equals(actualPassword))
                    {
                        var lstModel = objDAL.GetEmargAckDAL(splitVoucher);

                        if (lstModel != null)
                        {
                            return Request.CreateResponse<List<EmargPaymentMasterViewModel>>(HttpStatusCode.OK, (List<EmargPaymentMasterViewModel>)lstModel);
                            //return Request.CreateResponse<long[]>(HttpStatusCode.OK, BillId);
                        }
                        else
                        {
                            return Request.CreateResponse<long[]>(HttpStatusCode.BadRequest, null);
                        }
                    }
                    else
                        return Request.CreateResponse<string>(HttpStatusCode.Unauthorized, "Please Enter Valid Credentials.");

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
                    sw.WriteLine("Method : " + "EmargPushDetails.asmx.cs - GetEmargAcknowledgement()");
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


        #region Push PACKAGE Emarg PIU Details

        [HttpGet]
        public HttpResponseMessage GetPackageEmargPIUDetails()
        {
            EmargPushDetailsDAL objDAL = new EmargPushDetailsDAL();
            string userName = string.Empty;
            string password = string.Empty;
            //string PackageId = string.Empty;

            try
            {
                var UserName = Request.Headers.GetValues("UserName") == null ? null : Request.Headers.GetValues("UserName").ElementAt(0);
                var Password = Request.Headers.GetValues("Password") == null ? null : Request.Headers.GetValues("Password").ElementAt(0);
                //var PackageID = Request.Headers.GetValues("PackageID") == null ? null : Request.Headers.GetValues("PackageID").ElementAt(0);

                userName = Convert.ToString(UserName);
                password = Convert.ToString(Password);
                //PackageId = Convert.ToString(PackageID);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse<string>(HttpStatusCode.NoContent, "Header Values are Missing.");
            }


            //string userName = "Ommas_emarg";
            //string password = "1dab3e49df35d0bdadefe38fd3b6e046f2492ba5eb9d994d7000a36b6f3552c2";
            //string PackageId = "0";



            string finalPackageID = string.Empty;


            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                return Request.CreateResponse<string>(HttpStatusCode.LengthRequired, "Username and Password can not be Empty.");
            }

            try
            {
                if (true)
                {
                    string actualPassword = "Zcbm@1590";
                    if (ValidateUserDetailscs.validateSHA256Hash(password, actualPassword))
                    {
                        var lstModel = objDAL.GetEmargPIUDetailsDAL();

                        return Request.CreateResponse<List<PMGSY.Models.USP_EMARG_PIU_DETAILS_Result>>(HttpStatusCode.OK, (List<PMGSY.Models.USP_EMARG_PIU_DETAILS_Result>)lstModel);
                    }
                    else
                        return Request.CreateResponse<string>(HttpStatusCode.Unauthorized, "Please Enter Valid Credentials.");

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
                    sw.WriteLine("Method : " + "EmargPushDetails.asmx.cs - GetPackageEmargPIUDetails()");
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



        #region GetPaymentAckDetails

        [HttpGet]
        public HttpResponseMessage GetPaymentAckDetails()
        {
            EmargPushDetailsDAL objDAL = new EmargPushDetailsDAL();
            string userName = string.Empty;
            string password = string.Empty;
            //string PackageId = string.Empty;

            try
            {
                var UserName = Request.Headers.GetValues("UserName") == null ? null : Request.Headers.GetValues("UserName").ElementAt(0);
                var Password = Request.Headers.GetValues("Password") == null ? null : Request.Headers.GetValues("Password").ElementAt(0);
                //var PackageID = Request.Headers.GetValues("PackageID") == null ? null : Request.Headers.GetValues("PackageID").ElementAt(0);

                userName = Convert.ToString(UserName);
                password = Convert.ToString(Password);
                //PackageId = Convert.ToString(PackageID);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse<string>(HttpStatusCode.NoContent, "Header Values are Missing.");
            }


            //string userName = "Ommas_emarg";
            //string password = "1dab3e49df35d0bdadefe38fd3b6e046f2492ba5eb9d994d7000a36b6f3552c2";
            //string PackageId = "0";



            string finalPackageID = string.Empty;


            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                return Request.CreateResponse<string>(HttpStatusCode.LengthRequired, "Username and Password can not be Empty.");
            }

            try
            {
                if (true)
                {
                    string actualPassword = " @1590";
                    if (ValidateUserDetailscs.validateSHA256Hash(password, actualPassword))
                    {
                        var lstModel = objDAL.GetPaymentAckDetailsDAL();

                        return Request.CreateResponse<List<EmargPayment>>(HttpStatusCode.OK, (List<EmargPayment>)lstModel);


                    }
                    else
                        return Request.CreateResponse<string>(HttpStatusCode.Unauthorized, "Please Enter Valid Credentials.");

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
                    sw.WriteLine("Method : " + "EmargPushDetails.asmx.cs - GetPaymentAckDetails()");
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


        #region Get DRRP Emarg Road Details -- Added By Hrishikesh-----
        [HttpPost]
        public HttpResponseMessage GetDRRPRoadDetails()
        {
            EmargPushDetailsDAL objDAL = new EmargPushDetailsDAL();
            string key = string.Empty;
            string token = string.Empty;
            string state_code = string.Empty;
            //string PackageId = string.Empty;

            try
            {
                var key1 = Request.Headers.GetValues("key") == null ? null : Request.Headers.GetValues("key").ElementAt(0);
                var token1 = Request.Headers.GetValues("token") == null ? null : Request.Headers.GetValues("token").ElementAt(0);
                var state_code1 = Request.Headers.GetValues("state_code") == null ? null : Request.Headers.GetValues("state_code").ElementAt(0);

                key = Convert.ToString(key1);
                token = Convert.ToString(token1);
                state_code = Convert.ToString(state_code1);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse<string>(HttpStatusCode.NoContent, "Header Values are Missing.");
            }


            //string userName = "Ommas_emarg";
            //string password = "1dab3e49df35d0bdadefe38fd3b6e046f2492ba5eb9d994d7000a36b6f3552c2";
            //string PackageId = "0";



            string finalStateCode = string.Empty;
            if (string.IsNullOrEmpty(state_code))
            {
                // return "Please Enter State Code.";
                return Request.CreateResponse<string>(HttpStatusCode.LengthRequired, "Please Enter State Code.");
            }
            else
            {
                finalStateCode = state_code.Trim();
            }

            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(token))
            {
                return Request.CreateResponse<string>(HttpStatusCode.LengthRequired, "Key and Token can not be Empty.");
            }

            try
            {
                if (true)
                {//
                    string actualKey = "DRRPRoad@9804";
                    string actualToken = "Zcbm@1590";
                    if (ValidateUserDetailscs.validateSHA256Hash(key, actualKey) && ValidateUserDetailscs.validateSHA256Hash(token, actualToken))
                    {//
                        var lstModel = objDAL.GetDRRPRoadPullDAL(finalStateCode);
                        // var serializer = new JavaScriptSerializer();
                        // serializer.MaxJsonLength = Int32.MaxValue;
                        //  return (serializer.Serialize(lstModel));
                        return Request.CreateResponse<List<MasterDRRPRoadDetailsModel>>(HttpStatusCode.OK, (List<MasterDRRPRoadDetailsModel>)lstModel);
                    }
                    else
                        return Request.CreateResponse<string>(HttpStatusCode.Unauthorized, "Please Enter Valid Credentials.");
                    // return "Please Enter Valid Credentials.";
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
                    sw.WriteLine("Method : " + "EmargPushDetails.asmx.cs - GetDRRPRoadDetails()");
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

        #region Get SANCTION_DRRP_ROADS EMARG-- Added By Shreyas-----
        [HttpPost]
        public HttpResponseMessage GetSanctionDRRPRoads()
        {
            EmargPushDetailsDAL objDAL = new EmargPushDetailsDAL();
            string userName = string.Empty;
            string password = string.Empty;
            int StateCode = -1;

            try
            {
                var UserName = Request.Headers.GetValues("UserName") == null ? null : Request.Headers.GetValues("UserName").ElementAt(0);
                var Password = Request.Headers.GetValues("Password") == null ? null : Request.Headers.GetValues("Password").ElementAt(0);
                var StateCode1 = Request.Headers.GetValues("StateCode") == null ? null : Request.Headers.GetValues("StateCode").ElementAt(0);

                userName = Convert.ToString(UserName);
                password = Convert.ToString(Password);
                StateCode = Convert.ToInt32(StateCode1);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse<string>(HttpStatusCode.NoContent, "Header Values are Missing.");
            }


            //string UserName = "Ommas_emarg";
            //string Password = "1dab3e49df35d0bdadefe38fd3b6e046f2492ba5eb9d994d7000a36b6f3552c2";
            //string StateCode = "5";



            //string finalPackageID = string.Empty;
            int finalStateCode = -1;
            if (StateCode == -1)
            {
                // return "Please Enter Package ID.";
                return Request.CreateResponse<string>(HttpStatusCode.LengthRequired, "Please Enter State Code.");
            }
            else
            {
                finalStateCode = StateCode;
            }

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                return Request.CreateResponse<string>(HttpStatusCode.LengthRequired, "Username and Password can not be Empty.");
            }

            try
            {
                if (true)
                {
                    string actualPassword = "Zcbm@1590";
                    if (ValidateUserDetailscs.validateSHA256Hash(password, actualPassword))
                    {
                        var lstModel = objDAL.GetGetSanctionDRRPRoadsDAL(finalStateCode);
                        // var serializer = new JavaScriptSerializer();
                        // serializer.MaxJsonLength = Int32.MaxValue;
                        //  return (serializer.Serialize(lstModel));
                        return Request.CreateResponse<List<PMGSY.Models.USP_SANCTION_DRRP_ROADS_Result>>(HttpStatusCode.OK, (List<PMGSY.Models.USP_SANCTION_DRRP_ROADS_Result>)lstModel);
                    }
                    else
                        return Request.CreateResponse<string>(HttpStatusCode.Unauthorized, "Please Enter Valid Credentials.");
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
                    sw.WriteLine("Method : " + "EmargPushDetails.asmx.cs - GetSanctionDRRPRoads()");
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