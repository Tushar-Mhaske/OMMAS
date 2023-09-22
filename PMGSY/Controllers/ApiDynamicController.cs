using Newtonsoft.Json;
using PMGSY.Areas.DynamicData.DAL;
using PMGSY.Common;
using PMGSY.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;

namespace PMGSY.Controllers
{
    [AllowAnonymous]
    public class ApiDynamicController : ApiController
    {

        private static string ErrorLogBasePath = System.Configuration.ConfigurationSettings.AppSettings["ABAProgressErrorPath"];
        private static string ErrorLogDirectory = ErrorLogBasePath + "\\" + "ErrorLog_" + DateTime.Now.Month + "_" + DateTime.Now.Year;
        private string ErrorLogFilePath = ErrorLogDirectory + "//ErrorLog_" + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + ".txt";

        [HttpGet]
        public HttpResponseMessage GetData()
        {
           
            //string userName = string.Empty;
            //string password = string.Empty;
            //int StateCode = 0;
            //int month = 0;
            //int year = 0;
            //try
            //{
            //    //string authenticationString = Request.Headers.Authorization.Parameter;
            //    //string originalString = Encoding.UTF8.GetString(Convert.FromBase64String(authenticationString));
            //    ////string DecodededDate = HttpUtility.UrlDecode();
            //    //string UserName = originalString.Split(':')[0];
            //    //string Password = originalString.Split(':')[1];

            //    //userName = Convert.ToString(UserName);
            //    //password = Convert.ToString(Password);

            //    StateCode = 0;
            //    month = System.DateTime.Now.Month;
            //    year = System.DateTime.Now.Year;
            //}
            //catch (Exception ex)
            //{
            //    return Request.CreateResponse<string>(HttpStatusCode.NoContent, "Header Values are Missing.");
            //}
            //if (month == 0 || year == 0)
            //{
            //    return Request.CreateResponse<string>(HttpStatusCode.LengthRequired, "Month or Year can not be Empty.");
            //}

            //if (StateCode == null || month == null || year == null)
            //{
            //    return Request.CreateResponse<string>(HttpStatusCode.LengthRequired, "State, Month or Year can not be Empty.");
            //}

            //if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            //{
            //    return Request.CreateResponse<string>(HttpStatusCode.LengthRequired, "Username and Password can not be Empty.");
            //}

            try
            {
                //if (true)
                //{
                //    string actualPassword = "Zcbm@1590";
                //    int i = 1;
                //   // if (password.Equals(actualPassword))
                //    if (i==1)
                //    {
                        DynamicDataDAL objDal = new DynamicDataDAL();
                        List<Models.DYNAMIC_DASHBOARD> objResult = objDal.GeDynamictData();
                     
                        return Request.CreateResponse<List<Models.DYNAMIC_DASHBOARD>>(HttpStatusCode.OK, objResult);
                //    }
                //    else
                //        return Request.CreateResponse<string>(HttpStatusCode.Unauthorized, "Please Enter Valid Credentials.");
                   
                //}

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
                    sw.WriteLine("Method : " + "ApiDynamicController.cs - GetData()");
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





        [HttpGet]
        public HttpResponseMessage GetDataByState(string token, string key, string statecode)
        {

            string Token = string.Empty;
            string encryptedKey = string.Empty;
            int StateCode;

            int month = 0;
            int year = 0;
            try
            {

                month = System.DateTime.Now.Month;
                year = System.DateTime.Now.Year;

                if (string.IsNullOrEmpty(token))
                {
                    return Request.CreateResponse<string>(HttpStatusCode.LengthRequired, "Token is required.");
                }
                else
                {
                    Token = token.Trim();
                }

                if (string.IsNullOrEmpty(key))
                {
                    return Request.CreateResponse<string>(HttpStatusCode.LengthRequired, "Key is required");
                }
                else
                {
                    // StateCode = key.Trim();
                    encryptedKey = key.Trim();
                }

                if (string.IsNullOrEmpty(statecode))
                {
                    if (!(Convert.ToInt16(statecode) >= 0 || Convert.ToInt16(statecode) <= 37))
                        return Request.CreateResponse<string>(HttpStatusCode.LengthRequired, "state code invalid");
                    else
                        return Request.CreateResponse<string>(HttpStatusCode.LengthRequired, "state code is required");
                }
                else
                    StateCode = Convert.ToInt16(statecode);


                string actualPassword = "Ommas$db@123";
                string actualKey = "NitiAayogApi$2023";

                //   string temphashToken = ComputeSHA256Hash(actualPassword);
                //  string TempKey= Cryptography.AESEncrypt("NitiAayogApi$2023");


                //----------- OLD CODE START -----------------
                //if (StateCode.Contains(' '))
                //{
                //    StateCode = StateCode.Replace(' ', '+');
                //}

                //string tempDecryState = Cryptography.AESDecrypt(StateCode);
                //int State_Code = Convert.ToInt32(tempDecryState);

                //if (validateSHA256Hash(Token, actualPassword) && (State_Code <= 37))
                //{
                //    DynamicDataDAL objDal = new DynamicDataDAL();

                //    IEnumerable<dynamic> objResult = objDal.GeDynamictDataByStateCode(State_Code);
                //    return Request.CreateResponse<IEnumerable<dynamic>>(HttpStatusCode.OK, objResult);
                //}
                //else
                //    return Request.CreateResponse<string>(HttpStatusCode.LengthRequired, "Enter Valid Credential.");
                //-----------OLD CODE END -----------------

                // ----------- NEW CODE START ---------------
                if (encryptedKey.Contains(' '))
                {
                    encryptedKey = encryptedKey.Replace(' ', '+');
                }

                string decryptedKey = Cryptography.AESDecrypt(encryptedKey);

                if (validateSHA256Hash(Token, actualPassword) && (actualKey.Equals(decryptedKey)))
                {
                    DynamicDataDAL objDal = new DynamicDataDAL();

                    IEnumerable<dynamic> objResult = objDal.GeDynamictDataByStateCode(StateCode);
                    return Request.CreateResponse<IEnumerable<dynamic>>(HttpStatusCode.OK, objResult);
                }
                else
                    return Request.CreateResponse<string>(HttpStatusCode.LengthRequired, "Enter Valid Credential.");

                //------------- NEW CODE END ------------

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
                    sw.WriteLine("Method : " + "ApiDynamicController.cs - GetDataByStateCode()");
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




        public static string ComputeSHA256Hash(string input)
        {
            StringBuilder stringBuilder = new StringBuilder();
            byte[] textBytes = Encoding.ASCII.GetBytes(input);

            using (SHA256 sha1 = SHA256.Create())
            {
                byte[] computeHash = sha1.ComputeHash(textBytes);
                for (int i = 0; i < computeHash.Length; i++)
                {
                    stringBuilder.Append(computeHash[i].ToString("x2"));
                }
            }
            return stringBuilder.ToString();
        }



        public static bool validateSHA256Hash(string password, string actualPassword)
        {
            string temphashPassword = ComputeSHA256Hash(actualPassword);
            bool flag;
            if (string.Compare(temphashPassword, password) == 0)
            {
                flag = true;
            }
            else
            {
                flag = false;
            }
            return flag;
        }





    }







}