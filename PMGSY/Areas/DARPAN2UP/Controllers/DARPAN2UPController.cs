using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Script.Serialization;
using System.Security.Cryptography;
using System.Net;
using DARPAN2UP;

namespace PMGSY.Areas.DARPAN2UP.Controllers
{
    public class DARPAN2UPController : Controller
    {
        //
        // GET: /DARPAN2UP/DARPAN2UP/
        DarpanCommon objCommon = new DarpanCommon();
        BAL_Client obj = new BAL_Client();


        [HttpGet]
        public ActionResult DARPAN2UPLayout()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Index()      
        {
            DateTime now = DateTime.Now;
            BoInput objInput = new BoInput();
            //Get Values form Global Item Class-
            //====================================
            objInput.instance_code = GlobalItem.instance_Code;
            objInput.project_code = GlobalItem.project_Code;

            int dateExp = 0;
            string ResultMessage = "";
            string msg = "";
            string client_DataDate = null;
            string client_RemainingDataGroup = null;
            var clientGroupId = 0;
            DataTable dt_Output;
            string DecString = null;
            //Convert Date Range Json data into Table 
            //=======================================

            ResultMessage = ApiConnectWithDaterange(objInput);
            if (ResultMessage.Contains("Exception"))
            {
                dt_Output = (DataTable)JsonConvert.DeserializeObject(ResultMessage.Split('@')[1], (typeof(DataTable)));
                msg = dt_Output.Rows[0].ItemArray[1].ToString();
                dateExp = 4;

            }
            if (ResultMessage.Contains("Unauthorized"))
            {
                dateExp = 5;
                msg = "401 Unauthorized request.";

            }
            if (dateExp == 4)// In case of Exception                      //log Api Response in client Database
            {
                obj.Bal_MsgLog(now, 0, msg, clientGroupId, client_DataDate);
                return Json(new { success = false, message = msg });
               // return null;   // temporary changes by saurabh
            }
            else if (dateExp == 5)// In case of Exception     //log Api Response in client Database
            {
                obj.Bal_MsgLog(now, 0, msg, clientGroupId, client_DataDate);
                return Json(new { success = false, message = "401 Unauthorized request." });
               // return null;   // temporary changes by saurabh
            }
            else
            {
                DecString = objCommon.DecryptData(ApiConnectWithDaterange(objInput), GlobalItem.APIKEY);
            }

            DataTable dt_DateRange = (DataTable)JsonConvert.DeserializeObject(DecString, (typeof(DataTable)));


            if (dt_DateRange.Rows.Count > 0)
            {
                //Check the Valid Date
                bool isValidDate = objCommon.CheckValidateDateFormat(dt_DateRange.Rows[0][0].ToString());
                // string msg = "";
                if (isValidDate)
                {
                    //Process Data date one by one
                    //=============================
                    for (var i = 0; i <= dt_DateRange.Rows.Count - 1; i++)
                    {
                        dateExp = 0; ResultMessage = "";

                        //Check the Valid Date
                        if (!objCommon.CheckValidateDateFormat(dt_DateRange.Rows[i][0].ToString()))
                        {
                            dateExp = 3;
                            break;
                        }
                        //GET Data date
                        client_DataDate = dt_DateRange.Rows[i][0].ToString();

                        //GET Groups In Data dates
                        client_RemainingDataGroup = dt_DateRange.Rows[i][1].ToString();

                        string[] arr_clientGroupId = client_RemainingDataGroup.Split('#');

                        //Process GroupID of date one by one
                        //==================================

                        for (clientGroupId = 0; clientGroupId <= arr_clientGroupId.Count() - 1; clientGroupId++)
                        {
                            //Get Data for Data Date and Group ID
                            //===================================

                            DataTable dt_ClientDataForPort = GetDataForPorting(GlobalItem.instance_Code, GlobalItem.project_Code, int.Parse(arr_clientGroupId[clientGroupId]), client_DataDate);
                            if (dt_ClientDataForPort.Rows.Count > 0)
                            {
                                //Push Data to DARPAN API 
                                //=======================
                                dt_Output = null;
                                ResultMessage = ApiConnect(objCommon.ConvertToListnew(dt_ClientDataForPort), objInput);
                                if (ResultMessage.Contains("Exception"))
                                {
                                    dt_Output = (DataTable)JsonConvert.DeserializeObject(ResultMessage.Split('@')[1], (typeof(DataTable)));
                                    msg = dt_Output.Rows[0].ItemArray[1].ToString();
                                    dateExp = 4;
                                    break;
                                }
                                if (ResultMessage.Contains("Unauthorized"))
                                {
                                    dateExp = 5;
                                    msg = "401 Unauthorized request.";
                                    break;
                                }
                                //Convert Date Range Json data into Table 
                                string str = objCommon.DecryptData(ResultMessage, GlobalItem.APIKEY);
                                dt_Output = (DataTable)JsonConvert.DeserializeObject(str, (typeof(DataTable)));
                                msg = dt_Output.Rows[0].ItemArray[1].ToString();
                                // IF Porting Not Success go for next group(if exists) 
                                //if (!dt_Output.Rows[0].ItemArray[1].ToString().Contains("Success"))
                                //{
                                //    dateExp = 1;
                                //}
                                //else
                                //{
                                obj.Bal_MsgLog(now, 0, msg, clientGroupId, client_DataDate);
                                //}
                            }
                            else
                            {
                                dateExp = 2;
                                break;
                            }

                        }
                        if (dateExp == 1 || dateExp == 4)
                            break;
                    }


                    if (dateExp == 1)
                    {
                        //log Api Response in client Database
                        obj.Bal_MsgLog(now, 0, msg, clientGroupId, client_DataDate);
                        return Json(new { success = false, message = msg });  // changes by saurabh
                    }
                    else if (dateExp == 2)
                    {
                        //log Api Response in client Database
                        obj.Bal_MsgLog(now, 0, "No Record found for the given date range and group id in client table.", clientGroupId, client_DataDate);
                        return Json(new { success = false, message = "No Record found for the given date range and group id in client table." });   // changes by saurabh
                    }
                    else if (dateExp == 3)
                    {
                        //log Api Response in client Database
                        obj.Bal_MsgLog(now, 0, "Date format is incorrect.", clientGroupId, client_DataDate);
                        return Json(new { success = false, message = "Date format is incorrect." });  // changes by saurabh
                    }
                    else if (dateExp == 4)// In case of Exception  
                    {
                        //log Api Response in client Database
                        obj.Bal_MsgLog(now, 0, msg, clientGroupId, client_DataDate);
                        return Json(new { success = false, message = msg });  // changes by saurabh
                    }
                    else if (dateExp == 5)// In case of Exception   //log Api Response in client Database
                    {
                        obj.Bal_MsgLog(now, 0, msg, clientGroupId, client_DataDate);
                        return Json(new { success = false, message = msg });  // changes by saurabh
                    }
                    //else
                    //    //log Api Response in client Database
                    //    obj.Bal_MsgLog(now, 1, "Data Port Successfully", clientGroupId, client_DataDate);

                }
                else
                {
                    if (dt_DateRange.Rows[0][1].ToString().Split('#').Count() > 0)
                    {
                        //log Api Response in client Database
                        obj.Bal_MsgLog(now, 0, dt_DateRange.Rows[0][1].ToString().Split('#')[1], clientGroupId, client_DataDate);
                        return Json(new { success = true, message = dt_DateRange.Rows[0][1].ToString().Split('#')[1] });
                    }
                    else
                    { //log Api Response in client Database
                        obj.Bal_MsgLog(now, 0, dt_DateRange.Rows[0][1].ToString().Split('#')[0], clientGroupId, client_DataDate);
                        return Json(new { success = true, message = dt_DateRange.Rows[0][1].ToString().Split('#')[1] });
                    }
                }
            }

            return Json(new { success = true, message = msg });
        }


        private DataTable GetDataForPorting(int instance_Code, int project_Code, int clientGroupId, string client_DataDate)
        {
            BAL_Client obj = new BAL_Client();
            return obj.Bal_GetDataForPorting(instance_Code, project_Code, clientGroupId, client_DataDate, GlobalItem.SQLDBDataStoredProc);
        }

        protected string ApiConnectWithDaterange(BoInput api)
        {
            try
            {
                HttpClient httpClient = HttpClientFactory.Create();
                httpClient.DefaultRequestHeaders.Accept.Clear();
                var inputContent = (new JavaScriptSerializer() { MaxJsonLength = 2147483644 }).Serialize(api);
                //Prepare data for token
                ProjectKpiDetails rawdata = new ProjectKpiDetails()
                {
                    rawdata = inputContent,
                    projpara = api
                };
                string rawdatastring = JsonConvert.SerializeObject(rawdata);
                // Prepare prescribe payload input            
                ProjectKpiDetails PayloadData = new ProjectKpiDetails();
                PayloadData = rawdata;
                var inputContent_out = objCommon.CreateInputWithHash(PayloadData); // Prepare Input Payload            
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("DarpanToken", GetAuthHeaderValue(rawdatastring, GlobalItem.GetDateRangeAPIUrl, "POST"));
                var buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(inputContent_out));
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                ServicePointManager.SecurityProtocol = ((SecurityProtocolType)3072 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12);
                HttpResponseMessage response = httpClient.PostAsync(GlobalItem.GetDateRangeAPIUrl, byteContent).Result;
                if (response.ReasonPhrase == "Exception")
                {
                    return "Exception@" + response.Content.ReadAsStringAsync().Result;
                }
                else if (response.ReasonPhrase == "Unauthorized")   //401         
                    return "Unauthorized@" + response.Content.ReadAsStringAsync().Result;

                else
                {
                    return objCommon.ValidateOutPutData(response);
                }

            }
            catch (Exception ex)
            {
                object response = null;
                var r = new { Status = "0", Message = ex.Message };
                response = r;
                return response.ToString();
            }
            finally
            {
            }
        }

        protected string ApiConnect(List<Masterdata> Records, BoInput api)
        {
            try
            {
                if (Records.Count() > 1)
                {
                    return null;// Exception @Data for Multiple dates not allowed.
                }
                var inputContent = (new JavaScriptSerializer() { MaxJsonLength = 2147483644 }).Serialize(Records);
                //Prepare data for token
                ProjectKpiDetails rawdata = new ProjectKpiDetails()
                {
                    rawdata = inputContent,
                    projpara = api
                };
                string rawdatastring = JsonConvert.SerializeObject(rawdata);

                HttpClient httpClient = HttpClientFactory.Create();
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //Attach Authorization Token
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("DarpanToken", GetAuthHeaderValue(rawdatastring, GlobalItem.PushDataAPIUrl, "POST"));
                httpClient.Timeout = TimeSpan.FromMinutes(60.0);
                string compressData = objCommon.Compress(inputContent);

                //Prepare data for payload
                ProjectKpiDetailsCompressData PayloadData = new ProjectKpiDetailsCompressData()
                {
                    compresseddata = compressData,
                    projpara = api
                };
                // Prepare prescribe payload input
                var inputContent_out = objCommon.CreateInputWithHash(PayloadData);
             
                var buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(inputContent_out));
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                ServicePointManager.SecurityProtocol = ((SecurityProtocolType)3072 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12);
                HttpResponseMessage response = httpClient.PostAsync(GlobalItem.PushDataAPIUrl, byteContent).Result;
                if (response.ReasonPhrase == "Exception")
                {
                    return "Exception@" + response.Content.ReadAsStringAsync().Result;
                }
                else if (response.ReasonPhrase == "Unauthorized")   //401         
                    return "Unauthorized@" + response.Content.ReadAsStringAsync().Result;

                else
                {
                    return objCommon.ValidateOutPutData(response);
                }


            }
            catch (Exception ex)
            {
                object response = null;
                var r = new { Status = "0", Message = ex.Message };
                response = r;
                return response.ToString();
            }
            finally
            {
            }
        }

        private string GetAuthHeaderValue(string rawdata, string pushDataAPIUrl, string requestMethod)
        {
            string response = null;
            string requestContentBase64String = string.Empty;
            string requestUri = System.Web.HttpUtility.UrlEncode(pushDataAPIUrl.ToLower());// in lower case
            string requestHttpMethod = requestMethod;
            //Calculate UNIX time in for seconds (string size  should be 10) https://www.epochconverter.com/

            DateTime epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan timeSpan = DateTime.UtcNow - epochStart;
            string requestTimeStamp = Convert.ToUInt64(timeSpan.TotalSeconds).ToString();
            //string requestTimeStamp = "1667992364";
            if (requestTimeStamp.Length == 10)
            {
                //create random nonce for each request
                string nonce = Guid.NewGuid().ToString("N");
                //string nonce = "bae1818e34944aca8394f7864bf8801b";
                if (rawdata != null)
                {
                    DarpanCommon objComm = new DarpanCommon();
                    requestContentBase64String = objComm.ComputeSha512Hash(rawdata);
                }

                //Creating the raw signature string
                string RawSign = String.Format("{0}{1}{2}{3}{4}{5}{6}", GlobalItem.instance_Code, GlobalItem.project_Code, requestHttpMethod, requestUri, requestTimeStamp, nonce.ToString(), requestContentBase64String.ToString());
                var secretKeyByteArray = Convert.FromBase64String(GlobalItem.APIHMAC_KEY);
                byte[] signature = Encoding.UTF8.GetBytes(RawSign);
                using (HMACSHA512 hmac = new HMACSHA512(secretKeyByteArray))
                {
                    byte[] signatureBytes = hmac.ComputeHash(signature);
                    string requestSignatureBase64String = Convert.ToBase64String(signatureBytes);
                    response = string.Format("{0}:{1}:{2}:{3}:{4}", GlobalItem.instance_Code, GlobalItem.project_Code, requestSignatureBase64String, nonce, requestTimeStamp);
                }
            }


            return response;
        }




  



  




    }
}
