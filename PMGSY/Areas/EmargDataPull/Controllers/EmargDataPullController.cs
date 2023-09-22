 
using PMGSY.Areas.EmargDataPull.DAL;
using PMGSY.Common;
using PMGSY.WebServices.eMarg.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;
using PMGSY.Areas.EmargDataPull.Models;
using System.Net.Mail;
using PMGSY.Models;
using System.IO;
using Newtonsoft.Json;
using PMGSY.Extensions;
using System.Configuration;

namespace PMGSY.Areas.EmargDataPull.Controllers
{
    public class EmargDataPullController : Controller
    {
        #region View Methods

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult EmargRoadDetails()
        {
            return View();
        }
        #endregion

        #region First Level Ack Service
        public async Task<ActionResult> PullEmargAcknowledmentDetails()
        {

            try
            {
                EmargDAL emargDALDetails = new EmargDAL();
                ServicePointManager.Expect100Continue = true; // Added on 29 Dec 2020
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
               // string URL = "https://www.emarg.gov.in/emargrest/rest/AckGetEmargDetails";
                string URL = "https://emarg.gov.in/emargrest/rest/AckGetEmargDetails";
                Uri uri = new Uri(URL);
                WebClient wc = new WebClient();
                string token = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes("cdcweb" + ":" + "gis@cdc098"));
                wc.Headers.Add("Authorization", "Basic " + token);
                string results;
                results = wc.DownloadString(uri);
                JavaScriptSerializer ser = new JavaScriptSerializer();
                List<EmargAck> lst = ser.Deserialize<List<EmargAck>>(results);//str is JSON string.


                await Task.Run(() =>
                {
                    emargDALDetails.SaveAcknowledmentData(lst);
                });

                return Json(new { success = true, message = "Details received Successfully." }, JsonRequestBehavior.AllowGet);
                //return null; 
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EmargDataPullController.PullEmargAcknowledmentDetails()");
                //return Json(new { success = false, message = "Details not received." }, JsonRequestBehavior.AllowGet);
                return Json(new { success = true, message = "Details are not received. ( " + ex.Message + " )" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Second Level Get Data Service
        public async Task<ActionResult> PullEmargRoadDetails()
        {
           
            try
            {
                EmargDAL emargDALDetails = new EmargDAL();
                ServicePointManager.Expect100Continue = true; // Added on 29 Dec 2020
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                string URL = "https://emarg.gov.in/emargrest/rest/datadetail/getdatacorrectionrecord";
                Uri uri = new Uri(URL);
                WebClient wc = new WebClient();
                string token = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes("cdcweb" + ":" + "gis@cdc098"));
                wc.Headers.Add("Authorization", "Basic " + token);
                string results;
                results = wc.DownloadString(uri);
                JavaScriptSerializer ser = new JavaScriptSerializer();
                List<RoadDetails> lst = ser.Deserialize<List<RoadDetails>>(results);

                await Task.Run(() =>
                {
                    emargDALDetails.SaveData(lst);
                });

                return Json(new { success = true, message = "Details received Successfully." }, JsonRequestBehavior.AllowGet);
                // return null; // Uncommented on 30 March 2020
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EmargDataPullController.PullEmargRoadDetails()");
               // return Json(new { success = false, message = "Details not received." }, JsonRequestBehavior.AllowGet);
                return Json(new { success = true, message = "Details are not received. ( " + ex.Message + " )" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Get data in a table from Second Level Service.

        public async Task<ActionResult> GetDataFromSecondService()
        {

            try
            {
                EmargDAL emargDALDetails = new EmargDAL();
                ServicePointManager.Expect100Continue = true; // Added on 29 Dec 2020
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                string URL = "https://emarg.gov.in/emargrest/rest/datadetail/getdatacorrectionrecord";
                Uri uri = new Uri(URL);
                WebClient wc = new WebClient();
                string token = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes("cdcweb" + ":" + "gis@cdc098"));
                wc.Headers.Add("Authorization", "Basic " + token);
                string results;
                results = wc.DownloadString(uri);
                JavaScriptSerializer ser = new JavaScriptSerializer();
                List<RoadDetails> lst = ser.Deserialize<List<RoadDetails>>(results);
                int status = 0;
                await Task.Run(() =>
                {
                    status= emargDALDetails.SaveDataFromSecondLevelService(lst);
                });
                if (status > 0)
                {
                    string errorLogPath = System.Configuration.ConfigurationSettings.AppSettings["EmargErrorPath"];
                    using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine(errorLogPath + "\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                    {
                        sw.WriteLine("Date :" + DateTime.Now.ToString());
                        sw.WriteLine("STEP 7 : End Programme");
                        // sw.WriteLine("EID : " + data.EID);
                        sw.WriteLine("---------------------------------------------------------------------------------------");
                        sw.Close();
                    }
                    return Json(new { success = true, message = "Details received Successfully, number of rows affected = " + status }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string errorLogPath = System.Configuration.ConfigurationSettings.AppSettings["EmargErrorPath"];
                    using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine(errorLogPath + "\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                    {
                        sw.WriteLine("Date :" + DateTime.Now.ToString());
                        sw.WriteLine("STEP 7 : End Programme");
                        // sw.WriteLine("EID : " + data.EID);
                        sw.WriteLine("---------------------------------------------------------------------------------------");
                        sw.Close();
                    }
                    return Json(new { success = true, message = "No new records received, number of rows affected = " + status }, JsonRequestBehavior.AllowGet);
                }
                //return Json(new { success = true, message = "Details received Successfully." }, JsonRequestBehavior.AllowGet);

               // return null; // Uncommented on 30 March 2020
            }
            catch (Exception ex)
            {
                string errorLogPath = System.Configuration.ConfigurationSettings.AppSettings["EmargErrorPath"];
                ErrorLog.LogError(ex, "Inside Catch Method :: ! EmargDataPullController.GetDataFromSecondService()");
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine(errorLogPath+"\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Catch Method= : " + ex.Message.ToString());
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }
           //     return Json(new { success = false, message = "Details not received." }, JsonRequestBehavior.AllowGet);
                return Json(new { success = true, message = "Details are not received. ( " + ex.Message + " )" }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion 

        #region Get data in a table from First Level Service
        public async Task<ActionResult> GetDataFromFirstLevelDetails()
        {

            try
            {
                string errorLogPath = System.Configuration.ConfigurationSettings.AppSettings["EmargErrorPath"];

                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine(errorLogPath+"\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("STEP 1 : Inside  Controller GetDataFromFirstLevelDetails");

                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }

                EmargDAL emargDALDetails = new EmargDAL();
                ServicePointManager.Expect100Continue = true; // Added on 29 Dec 2020
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine(errorLogPath+"\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine(": Before URL Call");

                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }
                //string URL = "https://www.emarg.gov.in/emargrest/rest/AckGetEmargDetails";
                string URL = "https://emarg.gov.in/emargrest/rest/AckGetEmargDetails";
            //    string URL = "https://www.emarg.gov.in/emargrest_for_development/rest/AckGetEmargDetails";
                Uri uri = new Uri(URL);
                WebClient wc = new WebClient();
                string token = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes("cdcweb" + ":" + "gis@cdc098"));
                wc.Headers.Add("Authorization", "Basic " + token);
                string results;
                
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine(errorLogPath+"\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine(": After URL Call");

                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }
                results = wc.DownloadString(uri);
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine(errorLogPath+"\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("STEP 2 : After Download Operation");
                    if (results == null)
                    {
                        sw.WriteLine("----Result Received is NULL-------------");
                        sw.WriteLine("results 1 : " + results);
                    }
                    else
                    {
                        sw.WriteLine("----Result Received is Not NULL-------------");
                        sw.WriteLine("results 2 : " + results);
                    }
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }

                JavaScriptSerializer ser = new JavaScriptSerializer();
                ser.MaxJsonLength = int.MaxValue;

                List<EmargAck> lst = ser.Deserialize<List<EmargAck>>(results);    //str is JSON string.
                
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine(errorLogPath+"\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("STEP 3 : After Desialize");

                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }
                int status = 0;
                await Task.Run(() =>
                {
                   status= emargDALDetails.GetDataInAtableFromAcknowledmentDetails(lst);
                });
                if (status > 0)
                {
                   
                    using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine(errorLogPath+"\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                    {
                        sw.WriteLine("Date :" + DateTime.Now.ToString());
                        sw.WriteLine("STEP 7 : End Programme");
                        // sw.WriteLine("EID : " + data.EID);
                        sw.WriteLine("---------------------------------------------------------------------------------------");
                        sw.Close();
                    }
                    return Json(new { success = true, message = "Details received Successfully, number of rows affected = " +status }, JsonRequestBehavior.AllowGet); 
                }
                else
                {
                    
                    using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine(errorLogPath+"\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                    {
                        sw.WriteLine("Date :" + DateTime.Now.ToString());
                        sw.WriteLine("STEP 7 : End Programme");
                        // sw.WriteLine("EID : " + data.EID);
                        sw.WriteLine("---------------------------------------------------------------------------------------");
                        sw.Close();
                    }
                    return Json(new { success = true, message = "No new records received, number of rows affected = " +status }, JsonRequestBehavior.AllowGet); 
                }

            }
            catch (Exception ex)
            {
                string errorLogPath = System.Configuration.ConfigurationSettings.AppSettings["EmargErrorPath"];

                ErrorLog.LogError(ex, "Main EmargDataPullController.GetDataFromFirstLevelDetails()");
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine(errorLogPath+"\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Catch Method= : " + ex.Message.ToString());
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }              
                return Json(new { success = true, message = "Details are not received." }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Emarg Payment


	
        //[HttpPost]
       // public async Task<ActionResult> PullEmargPaymentDetails(EmargPaymentPullModel[] data)
        public async Task<ActionResult> PullEmargPaymentDetails()
        {
            int status = 0;
            int flag = 0;

            string EmargPaymentDetailsPath = System.Configuration.ConfigurationSettings.AppSettings["PullEmargPaymentDetails"];
            string EmargPaymentDetailsDirectory = EmargPaymentDetailsPath + "\\" + "EMARG_RoadwiseExpenditure";
            string EmargPaymentDetailsFile = EmargPaymentDetailsDirectory + "\\" + DateTime.Now.ToString("dd_MM_yyyy") + "_" + DateTime.Now.Millisecond.ToString() + ".txt";

            try
            {
                EmargDAL emargDAL = new EmargDAL();
                ServicePointManager.Expect100Continue = true; // Added on 29 Dec 2020
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                #region Test
                //List<EmargPaymentPullModel> data = new List<EmargPaymentPullModel>();
                //using (StreamReader file = System.IO.File.OpenText(@"E:\Bhushan\Documents\18_07_2022_923.txt"))
                //{
                //    JsonSerializer serializer = new JsonSerializer();
                //    data = (List<EmargPaymentPullModel>)serializer.Deserialize(file, typeof(List<EmargPaymentPullModel>));
                //}
                #endregion

              
                string URL = "https://emarg.gov.in/emargrest/rest/expenditure/getroadwiseexpenditure";
                //string URL = "https://emarg.gov.in/emargrest_for_development/rest/expenditure/getroadwiseexpenditure";

                Uri uri = new Uri(URL);
                WebClient wc = new WebClient();
                string token = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes("cdcweb" + ":" + "gis@cdc098"));
                wc.Headers.Add("Authorization", "Basic " + token);
                string results;
                string results1;
                try
                {

                    results = wc.DownloadString(uri);

                    //results1 = System.IO.File.ReadAllText(@"E:\\Bhushan\\Documents\\Emarg\\09_12_2022_373.txt");

                }
                catch (Exception ex)
                {
                    string errorLogPath = System.Configuration.ConfigurationSettings.AppSettings["EmargErrorPath"];

                    // ErrorLog.LogError(ex, "PullEmargPaymentDetails()");
                    using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine(errorLogPath + "\\eMARGErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                    {
                        sw.WriteLine("Date :" + DateTime.Now.ToString());
                        sw.WriteLine("PullEmargPaymentDetails() - wc.DownloadString(uri) ");
                        sw.WriteLine("Exception : " + ex.ToString());
                        sw.WriteLine("Exception : " + ex.StackTrace);
                        if (ex.InnerException != null)
                            sw.WriteLine("Inner Exception : " + ex.InnerException.ToString());
                        sw.WriteLine("---------------------------------------------------------------------------------------");
                        sw.Close();
                    }
                    return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
                }
                JavaScriptSerializer ser = new JavaScriptSerializer();
                ser.MaxJsonLength = Int32.MaxValue;
                List<EmargPaymentPullModel> lst = ser.Deserialize<List<EmargPaymentPullModel>>(results);//str is JSON string.
               


                List<EmargPaymentPullModel> Filteredlist1 = lst.FindAll(delegate (EmargPaymentPullModel listitem)
                {
                    //if ((listitem.BILL_MONTH+listitem.BILL_YEAR*12)>24269) // greater than 5 + (2022*12)
                    if ((listitem.BILL_MONTH+listitem.BILL_YEAR*12)>24276) // greater than 0 + (2023*12)
                        return true;
                    else
                        return false;
                });

                PMGSYEntities dbContext = new PMGSYEntities();

                //List<EMARG_PAYMENT_MASTER> AlreadyPulledDatalist = dbContext.EMARG_PAYMENT_MASTER.Where(x => x.BILL_MONTH + (x.BILL_YEAR * 12) > 24269 && x.EMARG_BILL_ID != null).ToList();// greater than 5 + (2022*12)
                
                List<EMARG_PAYMENT_MASTER> AlreadyPulledDatalist = dbContext.EMARG_PAYMENT_MASTER.Where(x => x.BILL_MONTH + (x.BILL_YEAR * 12) > 24276 && x.EMARG_BILL_ID != null).ToList();// greater than 0 + (2023*12)


                List<EmargPaymentPullModel> Filteredlist = Filteredlist1.FindAll(delegate (EmargPaymentPullModel listitem)
                {
                    if (!AlreadyPulledDatalist.Any(x=>x.BILL_ID==listitem.BILL_ID))
                        return true;
                    else
                        return false;
                });

                #region Test Region

                // string Filteredresult = ser.Serialize(Filteredlist);
                //using (System.IO.TextWriter tw = new System.IO.StreamWriter(EmargPaymentDetailsFile))
                //{

                //    tw.WriteLine(Filteredresult);

                //}
                #endregion



                await Task.Run(() =>
                {
                    status = emargDAL.EmargPaymentDAL(Filteredlist);
                });

                if (status > 0)
                    return Json(new { success = true, message = "Details received Successfully, number of rows affected = " + status }, JsonRequestBehavior.AllowGet); // Commented on 30 March 2020
                else
                    return Json(new { success = true, message = "No new records received, number of rows affected = " + status }, JsonRequestBehavior.AllowGet); // Commented on 30 March 2020

                //  return Json(new { success = true, message = "Details received Successfully" }, JsonRequestBehavior.AllowGet); // Commented on 30 March 2020
                //return null; // Uncommented on 30 March 2020
            }
            catch (Exception ex)
            {
                string errorLogPath = System.Configuration.ConfigurationSettings.AppSettings["EmargErrorPath"];

                if (!Directory.Exists(EmargPaymentDetailsDirectory))
                {
                    Directory.CreateDirectory(EmargPaymentDetailsDirectory);
                }
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine(errorLogPath+"\\eMARGErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Controller - PullEmargPaymentDetails()");
                    sw.WriteLine("Exception : " + ex.ToString());
                    sw.WriteLine("Exception : " + ex.StackTrace);
                    if (ex.InnerException != null)
                        sw.WriteLine("Inner Exception : " + ex.InnerException.ToString());
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }

                //  ErrorLog.LogError(ex, "PullEmargPaymentDetails()");
                return Json(new { success = false, message = "Error occured While pulling data" }, JsonRequestBehavior.AllowGet);
                //  throw ex;
            }
        }

        public async Task<ActionResult> PullEmargPaymentDetailsNew()
        {
            int status = 0;
            int flag = 0;

            string EmargPaymentDetailsPath = System.Configuration.ConfigurationSettings.AppSettings["PullEmargPaymentDetails"];
            string EmargPaymentDetailsDirectory = EmargPaymentDetailsPath + "\\" + "EMARG_RoadwiseExpenditure";
            string EmargPaymentDetailsFile = EmargPaymentDetailsDirectory + "\\" + DateTime.Now.ToString("dd_MM_yyyy") + "_" + DateTime.Now.Millisecond.ToString() + ".txt";

            try
            {
                EmargDAL emargDAL = new EmargDAL();
                ServicePointManager.Expect100Continue = true; // Added on 29 Dec 2020
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                string URL = "https://emarg.gov.in/emargrest/rest/expenditure/getroadwiseexpenditure";
                //string URL = "https://emarg.gov.in/emargrest_for_development/rest/expenditure/getroadwiseexpenditure";


                

                Uri uri = new Uri(URL);
                WebClient wc = new WebClient();
                string token = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes("cdcweb" + ":" + "gis@cdc098"));
                wc.Headers.Add("Authorization", "Basic " + token);
                string results;
                try
                {

                    results = wc.DownloadString(uri);

                }
                catch (Exception ex)
                {
                    string errorLogPath = System.Configuration.ConfigurationSettings.AppSettings["EmargErrorPath"];

                    // ErrorLog.LogError(ex, "PullEmargPaymentDetails()");
                    using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine(errorLogPath+"\\eMARGErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                    {
                        sw.WriteLine("Date :" + DateTime.Now.ToString());
                        sw.WriteLine("PullEmargPaymentDetailsNew() - wc.DownloadString(uri) ");
                        sw.WriteLine("Exception : " + ex.ToString());
                        sw.WriteLine("Exception : " + ex.StackTrace);
                        if (ex.InnerException != null)
                            sw.WriteLine("Inner Exception : " + ex.InnerException.ToString());
                        sw.WriteLine("---------------------------------------------------------------------------------------");
                        sw.Close();
                    }
                    return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
                }

                
                //  string EmargPaymentDetailsPath = System.Configuration.ConfigurationSettings.AppSettings["PullEmargPaymentDetails"];
                //  string EmargPaymentDetailsDirectory = EmargPaymentDetailsPath + "\\" + "EMARG_RoadwiseExpenditure";
                //string EmargPaymentDetailsFile = EmargPaymentDetailsDirectory + "\\" + DateTime.Now.ToString("dd_MM_yyyy") + "_" + DateTime.Now.Millisecond.ToString()   + ".txt";

                using (System.IO.TextWriter tw = new System.IO.StreamWriter(EmargPaymentDetailsFile))
                {
                 
                    tw.WriteLine(results);

                }



                JavaScriptSerializer ser = new JavaScriptSerializer();
                ser.MaxJsonLength = Int32.MaxValue;
                List<EmargPaymentPullModel> lst = ser.Deserialize<List<EmargPaymentPullModel>>(results);//str is JSON string.
                // var testlist = lst.Take(15).ToList();
                //check if u r getting response code in browser or not.
                //  Response.StatusCode = 200;




                //await Task.Run(() =>
                //{
                //    status = emargDAL.EmargPaymentNewDAL(lst);
                //});
                if (status > 0)
                    return Json(new { success = true, message = "Details received Successfully, number of rows affected = " + status }, JsonRequestBehavior.AllowGet); // Commented on 30 March 2020
                else
                    return Json(new { success = true, message = "No new records received, number of rows affected = " + status }, JsonRequestBehavior.AllowGet); // Commented on 30 March 2020

                //  return Json(new { success = true, message = "Details received Successfully" }, JsonRequestBehavior.AllowGet); // Commented on 30 March 2020
                //return null; // Uncommented on 30 March 2020
            }
            catch (Exception ex)
            {
                string errorLogPath = System.Configuration.ConfigurationSettings.AppSettings["EmargErrorPath"];

                if (!Directory.Exists(EmargPaymentDetailsDirectory))
                {
                    Directory.CreateDirectory(EmargPaymentDetailsDirectory);
                }

                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine(errorLogPath+"\\eMARGErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Controller - PullEmargPaymentDetailsNew()");
                    sw.WriteLine("Exception : " + ex.ToString());
                    sw.WriteLine("Exception : " + ex.StackTrace);
                    if (ex.InnerException != null)
                        sw.WriteLine("Inner Exception : " + ex.InnerException.ToString());
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }

                //  ErrorLog.LogError(ex, "PullEmargPaymentDetails()");
                return Json(new { success = false, message = "Error occured While pulling data" }, JsonRequestBehavior.AllowGet);
                //  throw ex;
            }
        }
        #endregion


        #region Emarg KPI Details State Ranking
        public async Task<ActionResult> GetKPIDetails()
        {

            try
            {
                EmargDAL emargDALDetails = new EmargDAL();
                ServicePointManager.Expect100Continue = true; // Added on 29 Dec 2020
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                string URL = "https://emarg.gov.in/emargrest/rest/datadetail/getkpidetails";
                Uri uri = new Uri(URL);
                WebClient wc = new WebClient();
                string token = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes("cdcweb" + ":" + "gis@cdc098"));
                wc.Headers.Add("Authorization", "Basic " + token);
                string results;
                results = wc.DownloadString(uri);
                JavaScriptSerializer ser = new JavaScriptSerializer();
                List<StateRankKPI> lst = ser.Deserialize<List<StateRankKPI>>(results);//str is JSON string.

                //PMGSY.Models.PMGSYEntities dbcontext = new PMGSY.Models.PMGSYEntities();

                //int Month= System.DateTime.Now.Month;
                //int Year = System.DateTime.Now.Year;
                //int Day = System.DateTime.Now.Day;


                //if (dbcontext.STATE_RANKING_KPI.Any(m => m.GENERATED_DATE == dt))
                //{
                //    return Json(new { success = true, message = "Details are already received today." }, JsonRequestBehavior.AllowGet);
                //}

                await Task.Run(() =>
                {
                    emargDALDetails.SaveStateRankingKPI(lst);
                });

                return Json(new { success = true, message = "Details Received Successfully." }, JsonRequestBehavior.AllowGet);

                // return null;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EmargDataPullController.GetKPIDetails()");
                return Json(new { success = true, message = "Details are not received. ( " + ex.Message + " )" }, JsonRequestBehavior.AllowGet);


                //  return Json(new { success = false, message = "Details not received." }, JsonRequestBehavior.AllowGet);

            }
        }

        #endregion

        // UPDATE ADDED ON 05-01-2022

        #region EMARG Balance Work Package Details Webservice :

        public async Task<ActionResult> GetRoadWiseBalanceWorkPackageDetails()
        {

            try
            {
                EmargDAL emargDALDetails = new EmargDAL();
                ServicePointManager.Expect100Continue = true; // Added on 29 Dec 2020
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

           //     string URL = "https://emarg.gov.in/emargrest_for_development/rest/balanceWorkPackageDetails/getBalanceWorkPackageDetails";
                string URL = "https://emarg.gov.in/emargrest/rest/balanceWorkPackageDetails/getBalanceWorkPackageDetails";

                Uri uri = new Uri(URL);
                WebClient wc = new WebClient();
                string token = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes("cdcweb" + ":" + "gis@cdc098"));
                wc.Headers.Add("Authorization", "Basic " + token);
                wc.UseDefaultCredentials = true;
                string results;
                results = wc.DownloadString(uri);
                JavaScriptSerializer ser = new JavaScriptSerializer();
                ser.MaxJsonLength = Int32.MaxValue;
                List<EmargRoadWiseBalanceWorks> lst = ser.Deserialize<List<EmargRoadWiseBalanceWorks>>(results);

                await Task.Run(() =>
                {
                    emargDALDetails.SaveRoadWiseBalanceWorkPackageDetails(lst);
                });

                return Json(new { success = true, message = "Details received Successfully." }, JsonRequestBehavior.AllowGet);
                // return null; // Uncommented on 30 March 2020
            }
            catch (Exception ex)
            {
                string errorLogPath = System.Configuration.ConfigurationSettings.AppSettings["EmargErrorPath"];

                ErrorLog.LogError(ex, "Inside Catch Method :: ! EmargDataPullController.GetRoadWiseBalanceWorkPackageDetails()");
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine(errorLogPath+"\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Catch Method= : " + ex.Message.ToString());

                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }
                //     return Json(new { success = false, message = "Details not received." }, JsonRequestBehavior.AllowGet);
                return Json(new { success = true, message = "Details are not received. ( " + ex.Message + " )" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

                

        #region Test Mails

        //public async Task<ActionResult> SendEmailToSQC()
        //{

        //    try
        //    {
        //        // select ADMIN_QC_EMAIL from omms.ADMIN_SQC where ADMIN_QC_EMAIL is not null  and ADMIN_QC_TYPE='S' and ADMIN_ACTIVE_STATUS='Y' -- SQC 
        //        PMGSYEntities dbcontext = new PMGSYEntities();

        //        var emailDetails = dbcontext.ADMIN_SQC.Where(m => m.ADMIN_QC_TYPE == "S" && m.ADMIN_ACTIVE_STATUS == "Y" && m.ADMIN_QC_EMAIL != null && m.ADMIN_QC_EMAIL != String.Empty).ToList();

        //        foreach (var email in emailDetails)
        //        {
        //            String toEmail = email.ADMIN_QC_EMAIL.Trim();// Request.Params["ToEmail"];
        //            String BccEmail = String.Empty;//Request.Params["BccEmail"];
        //            StringBuilder msgBody = new StringBuilder();


        //            var mailMessage = new Mvc.Mailer.MvcMailMessage
        //            {
        //                Subject = "Advertisement for Empanelment of National Quality Monitors (NQMs) for inspection of Road and Bridge Projects under PMGSY"
        //            };
        //            mailMessage.To.Add(toEmail);

        //            if (BccEmail != String.Empty && BccEmail != null)
        //            {
        //                mailMessage.Bcc.Add(BccEmail);

        //            }

        //            //mailMessage.Body = "This is Test email.";

        //            msgBody.Append("Sir, <br/>");
        //            msgBody.Append("      Pradhan Mantri Gram Sadak Yojana (PMGSY) is a flagship programme of<br/>");
        //            msgBody.Append("Government of India for providing good quality road connectivity in rural areas.<br/>");
        //            msgBody.Append("National Rural Infrastructure Development Agency (NRIDA), an agency of the<br/>");
        //            msgBody.Append("Union Ministry of Rural Development provides technical and managerial<br/>");
        //            msgBody.Append("support to the States for implementing the programme.<br/><br/>");

        //            msgBody.Append("      A three tier Quality Management System has been envisaged for<br/>");
        //            msgBody.Append("monitoring the quality of projects under the programme. First two tiers are<br/>");
        //            msgBody.Append("managed by the respective States implementing the programme. Under the <br/>");
        //            msgBody.Append("third tier; NRIDA empanels senior civil engineers from Central / State<br/>");
        //            msgBody.Append("Governments departments including PSUs as also serving / retired faculty<br/>");
        //            msgBody.Append("members of Government engineering colleges / IITs/ NITs/ Government<br/>");
        //            msgBody.Append("research institutes, as National Quality Monitors (NQMs). These NQMs are<br/>");
        //            msgBody.Append("required to carry out field inspections of PMGSY road and bridge projects, in a<br/>");
        //            msgBody.Append("span of 10-12 days in a month.<br/><br/>");

        //            msgBody.Append("NRIDA has advertised the requirement for empanelment of NQMs in leading<br/>");
        //            msgBody.Append("National news paper. The details of eligibility criteria, application format,<br/>");
        //            msgBody.Append("honorarium and travelling allowances etc. are available at <font color=blue> <u>www.pmgsy.nic.in</u></font>.<br/>");
        //            msgBody.Append("The same is also enclosed herewith for ready reference.<br/><br/>");


        //            msgBody.Append("Your Organization/ Institute dealing with civil infrastructure projects must be<br/>");
        //            msgBody.Append("having good repository of qualified and experienced engineers. I would therefore<br/>");
        //            msgBody.Append("request you to circulate our requirement amongst the interested civil engineers<br/>");
        //            msgBody.Append("of your department/ organization / institute and also upload it on your website<br/>");
        //            msgBody.Append("for wider circulation. You may also like to share it with SQMs of your state so<br/>");
        //            msgBody.Append("that if someone is willing to work as NQM, can apply. We believe that<br/>");
        //            msgBody.Append("knowledge sharing by the retired senior engineers and academicians would<br/>");
        //            msgBody.Append("urther augment the quality of built infrastructure under PMGSY.<br/><br/>");


        //            msgBody.Append("Encl. As above.<br/>");

        //            msgBody.Append("&nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp&nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp Best Wishes.<br/><br/>");
        //            msgBody.Append("&nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp&nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp <b>Dr. I. K. Pateriya</b><br/>");
        //            msgBody.Append("&nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp&nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp Director (Projects-III), NRIDA &<br/>");
        //            msgBody.Append("&nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp&nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp Chief Quality Coordinator (PMGSY)<br/>");

        //            mailMessage.Body = msgBody.ToString();


        //            string AttachmentFilePath = System.Configuration.ConfigurationSettings.AppSettings["MailAttachmentPath"];//"~/mdck/RevisedWeblinkForPublishing_041020.pdf";//"D:\\OmmasImages\\TRACEMAPSHAB\\RevisedWeblinkForPublishing_041020.pdf";
        //            string finalPath = AttachmentFilePath + "\\RevisedWeblinkForPublishing_041020.pdf";

        //            mailMessage.Attachments.Add(new System.Net.Mail.Attachment(finalPath));

        //            mailMessage.IsBodyHtml = true;
        //            mailMessage.Priority = MailPriority.High;
        //            await Task.Run(() =>
        //            {
        //                mailMessage.Send();
        //            });



        //        }



        //        return Json(new { success = true, message = "Mail sent to SQC Sucessfully." }, JsonRequestBehavior.AllowGet);

        //        // return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLog.LogError(ex, "EmargDataPullController.SendEmailToSQC()");
        //        return Json(new { success = true, message = "Mail not sent. ( " + ex.Message + " )" }, JsonRequestBehavior.AllowGet);


        //        //  return Json(new { success = false, message = "Details not received." }, JsonRequestBehavior.AllowGet);

        //    }
        //}





        public async Task<ActionResult> SendEmailToSQM1()
        {
            // select ADMIN_QM_EMAIL from omms.ADMIN_QUALITY_MONITORS where ADMIN_QM_TYPE='S' and ADMIN_QM_EMPANELLED='Y'  and ADMIN_QM_EMAIL is not null-- SQM
            try
            {
                  PMGSYEntities dbcontext = new PMGSYEntities();

                  var emailDetails = dbcontext.ADMIN_QUALITY_MONITORS.Where(m => m.ADMIN_QM_TYPE == "S" && m.ADMIN_QM_EMPANELLED == "Y" && m.ADMIN_QM_EMAIL != null && m.ADMIN_QM_EMAIL != String.Empty && m.ADMIN_QM_CODE >= 2831 && m.ADMIN_QM_CODE <= 2953).ToList();




                foreach (var email in emailDetails)
                {

                    String toEmail = email.ADMIN_QM_EMAIL.Trim();// Request.Params["ToEmail"];
                    String BccEmail = String.Empty;//Request.Params["BccEmail"];
                    StringBuilder msgBody = new StringBuilder();

                    //Commented by Srishti on 13-03-2023
                    //var mailMessage = new Mvc.Mailer.MvcMailMessage
                    // Added by Srishti on 13-03-2023
                    ServicePointManager.ServerCertificateValidationCallback += (s, ce, ca, p) => true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    var mailMessage = new MailMessage
                    {
                        Subject = "Advertisement for Empanelment of National Quality Monitors (NQMs) for inspection of Road and Bridge Projects under PMGSY"
                    };
                    mailMessage.To.Add(toEmail);

                    if (BccEmail != String.Empty && BccEmail != null)
                    {
                        mailMessage.Bcc.Add(BccEmail);

                    }



                    //mailMessage.Body = "This is Test email.";
                    msgBody.Append("Sir, <br/>");
                    msgBody.Append("      Pradhan Mantri Gram Sadak Yojana (PMGSY) is a flagship programme of <br/>");
                    msgBody.Append("Government of India for providing good quality road connectivity in rural areas.<br/>");
                    msgBody.Append("National Rural Infrastructure Development Agency (NRIDA), an agency of the<br/>");
                    msgBody.Append("Union Ministry of Rural Development provides technical and managerial<br/>");
                    msgBody.Append("support to the States for implementing the programme.<br/><br/>");

                    msgBody.Append("      A three tier Quality Management System has been envisaged for<br/>");
                    msgBody.Append("monitoring the quality of projects under the programme. First two tiers are<br/>");
                    msgBody.Append("managed by the respective States implementing the programme. Under the <br/>");
                    msgBody.Append("third tier; NRIDA empanels senior civil engineers from Central / State<br/>");
                    msgBody.Append("Governments departments including PSUs as also serving / retired faculty<br/>");
                    msgBody.Append("members of Government engineering colleges / IITs/ NITs/ Government<br/>");
                    msgBody.Append("research institutes, as National Quality Monitors (NQMs). These NQMs are<br/>");
                    msgBody.Append("required to carry out field inspections of PMGSY road and bridge projects, in a<br/>");
                    msgBody.Append("span of 10-12 days in a month.<br/><br/>");

                    msgBody.Append("NRIDA has advertised the requirement for empanelment of NQMs in leading<br/>");
                    msgBody.Append("National news paper. The details of eligibility criteria, application format,<br/>");
                    msgBody.Append("honorarium and travelling allowances etc. are available at <font color=blue> <u>www.pmgsy.nic.in</u></font>.<br/>");
                    msgBody.Append("The same is also enclosed herewith for ready reference.<br/><br/>");


                    msgBody.Append("You are working as State Quality Monitor in PMGSY and have exposure of <br/>");
                    msgBody.Append("PMGSY Three Quality Mechanism. You may like to work as National Quality<br/>");

                    msgBody.Append("Monitor or some other officers known to you may be willing to work. You may<br/>");
                    msgBody.Append("share the enclosed NQMs  requirement amongst the interested civil engineers<br/>");
                    msgBody.Append("We believe that knowledge sharing by the retired senior engineers and<br/>");
                    msgBody.Append("academicians would further augment the quality of built infrastructure under <br/>");
                    msgBody.Append("PMGSY.<br/><br/>");
                    msgBody.Append("Encl. As above.<br/>");

                    msgBody.Append("&nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp&nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp Best Wishes.<br/><br/>");
                    msgBody.Append("&nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp&nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp <b>Dr. I. K. Pateriya</b><br/>");
                    msgBody.Append("&nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp&nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp Director (Projects-III), NRIDA &<br/>");
                    msgBody.Append("&nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp&nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp Chief Quality Coordinator (PMGSY)<br/>");
                    mailMessage.Body = msgBody.ToString();
                    string AttachmentFilePath = System.Configuration.ConfigurationSettings.AppSettings["MailAttachmentPath"];//"~/mdck/RevisedWeblinkForPublishing_041020.pdf";//"D:\\OmmasImages\\TRACEMAPSHAB\\RevisedWeblinkForPublishing_041020.pdf";
                    string finalPath = AttachmentFilePath + "\\RevisedWeblinkForPublishing_041020.pdf";
                    mailMessage.Attachments.Add(new System.Net.Mail.Attachment(finalPath));
                    mailMessage.IsBodyHtml = true;
                    mailMessage.Priority = MailPriority.High;

                    // Added by Srishti on 13-03-2023
                    SmtpClient client = new SmtpClient();
                    string e_EuthHost = ConfigurationManager.AppSettings["e_EuthHost"];
                    string e_EuthPort = ConfigurationManager.AppSettings["e_EuthPort"];
                    string e_EuthMailUserName = ConfigurationManager.AppSettings["e_EuthMailUserName"];
                    string e_EuthMailPassword = ConfigurationManager.AppSettings["e_EuthMailPassword"];

                    client.Host = e_EuthHost;
                    client.Port = Convert.ToInt32(e_EuthPort);
                    client.UseDefaultCredentials = false;
                    client.EnableSsl = true; // Change to true
                    client.Credentials = new NetworkCredential(e_EuthMailUserName, e_EuthMailPassword);
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;

                    // Srishti changes end

                    await Task.Run(() =>
                    {
                        // Commented and added by Srishti on 13-03-2023
                        //mailMessage.Send();
                        client.Send(mailMessage);
                    });
                }

                return Json(new { success = true, message = "Mail sent to SQM Sucessfully." }, JsonRequestBehavior.AllowGet);

                // return null;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EmargDataPullController.SendEmailToSQM()");
                return Json(new { success = true, message = "Mail not sent. ( " + ex.Message + " )" }, JsonRequestBehavior.AllowGet);

            }
        }

        public async Task<ActionResult> SendEmailToSQM2()
        {
            // select ADMIN_QM_EMAIL from omms.ADMIN_QUALITY_MONITORS where ADMIN_QM_TYPE='S' and ADMIN_QM_EMPANELLED='Y'  and ADMIN_QM_EMAIL is not null-- SQM
            try
            {
                PMGSYEntities dbcontext = new PMGSYEntities();

                var emailDetails = dbcontext.ADMIN_QUALITY_MONITORS.Where(m => m.ADMIN_QM_TYPE == "S" && m.ADMIN_QM_EMPANELLED == "Y" && m.ADMIN_QM_EMAIL != null && m.ADMIN_QM_EMAIL != String.Empty && m.ADMIN_QM_CODE >= 2954 && m.ADMIN_QM_CODE <= 3057).ToList();




                foreach (var email in emailDetails)
                {

                    String toEmail = email.ADMIN_QM_EMAIL.Trim();// Request.Params["ToEmail"];
                    String BccEmail = String.Empty;//Request.Params["BccEmail"];
                    StringBuilder msgBody = new StringBuilder();


                    //Commented by Srishti on 13-03-2023
                    //var mailMessage = new Mvc.Mailer.MvcMailMessage
                    // Added by Srishti on 13-03-2023
                    ServicePointManager.ServerCertificateValidationCallback += (s, ce, ca, p) => true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    var mailMessage = new MailMessage
                    {
                        Subject = "Advertisement for Empanelment of National Quality Monitors (NQMs) for inspection of Road and Bridge Projects under PMGSY"
                    };
                    mailMessage.To.Add(toEmail);

                    if (BccEmail != String.Empty && BccEmail != null)
                    {
                        mailMessage.Bcc.Add(BccEmail);

                    }



                    //mailMessage.Body = "This is Test email.";
                    msgBody.Append("Sir, <br/>");
                    msgBody.Append("      Pradhan Mantri Gram Sadak Yojana (PMGSY) is a flagship programme of <br/>");
                    msgBody.Append("Government of India for providing good quality road connectivity in rural areas.<br/>");
                    msgBody.Append("National Rural Infrastructure Development Agency (NRIDA), an agency of the<br/>");
                    msgBody.Append("Union Ministry of Rural Development provides technical and managerial<br/>");
                    msgBody.Append("support to the States for implementing the programme.<br/><br/>");

                    msgBody.Append("      A three tier Quality Management System has been envisaged for<br/>");
                    msgBody.Append("monitoring the quality of projects under the programme. First two tiers are<br/>");
                    msgBody.Append("managed by the respective States implementing the programme. Under the <br/>");
                    msgBody.Append("third tier; NRIDA empanels senior civil engineers from Central / State<br/>");
                    msgBody.Append("Governments departments including PSUs as also serving / retired faculty<br/>");
                    msgBody.Append("members of Government engineering colleges / IITs/ NITs/ Government<br/>");
                    msgBody.Append("research institutes, as National Quality Monitors (NQMs). These NQMs are<br/>");
                    msgBody.Append("required to carry out field inspections of PMGSY road and bridge projects, in a<br/>");
                    msgBody.Append("span of 10-12 days in a month.<br/><br/>");

                    msgBody.Append("NRIDA has advertised the requirement for empanelment of NQMs in leading<br/>");
                    msgBody.Append("National news paper. The details of eligibility criteria, application format,<br/>");
                    msgBody.Append("honorarium and travelling allowances etc. are available at <font color=blue> <u>www.pmgsy.nic.in</u></font>.<br/>");
                    msgBody.Append("The same is also enclosed herewith for ready reference.<br/><br/>");


                    msgBody.Append("You are working as State Quality Monitor in PMGSY and have exposure of <br/>");
                    msgBody.Append("PMGSY Three Quality Mechanism. You may like to work as National Quality<br/>");

                    msgBody.Append("Monitor or some other officers known to you may be willing to work. You may<br/>");
                    msgBody.Append("share the enclosed NQMs  requirement amongst the interested civil engineers<br/>");
                    msgBody.Append("We believe that knowledge sharing by the retired senior engineers and<br/>");
                    msgBody.Append("academicians would further augment the quality of built infrastructure under <br/>");
                    msgBody.Append("PMGSY.<br/><br/>");
                    msgBody.Append("Encl. As above.<br/>");

                    msgBody.Append("&nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp&nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp Best Wishes.<br/><br/>");
                    msgBody.Append("&nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp&nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp <b>Dr. I. K. Pateriya</b><br/>");
                    msgBody.Append("&nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp&nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp Director (Projects-III), NRIDA &<br/>");
                    msgBody.Append("&nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp&nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp Chief Quality Coordinator (PMGSY)<br/>");
                    mailMessage.Body = msgBody.ToString();
                    string AttachmentFilePath = System.Configuration.ConfigurationSettings.AppSettings["MailAttachmentPath"];//"~/mdck/RevisedWeblinkForPublishing_041020.pdf";//"D:\\OmmasImages\\TRACEMAPSHAB\\RevisedWeblinkForPublishing_041020.pdf";
                    string finalPath = AttachmentFilePath + "\\RevisedWeblinkForPublishing_041020.pdf";
                    mailMessage.Attachments.Add(new System.Net.Mail.Attachment(finalPath));
                    mailMessage.IsBodyHtml = true;
                    mailMessage.Priority = MailPriority.High;

                    // Added by Srishti on 13-03-2023
                    SmtpClient client = new SmtpClient();
                    string e_EuthHost = ConfigurationManager.AppSettings["e_EuthHost"];
                    string e_EuthPort = ConfigurationManager.AppSettings["e_EuthPort"];
                    string e_EuthMailUserName = ConfigurationManager.AppSettings["e_EuthMailUserName"];
                    string e_EuthMailPassword = ConfigurationManager.AppSettings["e_EuthMailPassword"];

                    client.Host = e_EuthHost;
                    client.Port = Convert.ToInt32(e_EuthPort);
                    client.UseDefaultCredentials = false;
                    client.EnableSsl = true; // Change to true
                    client.Credentials = new NetworkCredential(e_EuthMailUserName, e_EuthMailPassword);
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;

                    // Srishti changes end

                    await Task.Run(() =>
                    {
                        // Commented and added by Srishti on 13-03-2023
                        //mailMessage.Send();
                        client.Send(mailMessage);
                    });
                }

                return Json(new { success = true, message = "Mail sent to SQM Sucessfully." }, JsonRequestBehavior.AllowGet);

                // return null;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EmargDataPullController.SendEmailToSQM()");
                return Json(new { success = true, message = "Mail not sent. ( " + ex.Message + " )" }, JsonRequestBehavior.AllowGet);

            }
        }

        public async Task<ActionResult> SendEmailToSQM3()
        {
            // select ADMIN_QM_EMAIL from omms.ADMIN_QUALITY_MONITORS where ADMIN_QM_TYPE='S' and ADMIN_QM_EMPANELLED='Y'  and ADMIN_QM_EMAIL is not null-- SQM
            try
            {
                PMGSYEntities dbcontext = new PMGSYEntities();

                var emailDetails = dbcontext.ADMIN_QUALITY_MONITORS.Where(m => m.ADMIN_QM_TYPE == "S" && m.ADMIN_QM_EMPANELLED == "Y" && m.ADMIN_QM_EMAIL != null && m.ADMIN_QM_EMAIL != String.Empty && m.ADMIN_QM_CODE >= 3058 && m.ADMIN_QM_CODE <= 3164).ToList();




                foreach (var email in emailDetails)
                {

                    String toEmail = email.ADMIN_QM_EMAIL.Trim();// Request.Params["ToEmail"];
                    String BccEmail = String.Empty;//Request.Params["BccEmail"];
                    StringBuilder msgBody = new StringBuilder();


                    //Commented by Srishti on 13-03-2023
                    //var mailMessage = new Mvc.Mailer.MvcMailMessage
                    // Added by Srishti on 13-03-2023
                    ServicePointManager.ServerCertificateValidationCallback += (s, ce, ca, p) => true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    var mailMessage = new MailMessage
                    {
                        Subject = "Advertisement for Empanelment of National Quality Monitors (NQMs) for inspection of Road and Bridge Projects under PMGSY"
                    };
                    mailMessage.To.Add(toEmail);

                    if (BccEmail != String.Empty && BccEmail != null)
                    {
                        mailMessage.Bcc.Add(BccEmail);

                    }



                    //mailMessage.Body = "This is Test email.";
                    msgBody.Append("Sir, <br/>");
                    msgBody.Append("      Pradhan Mantri Gram Sadak Yojana (PMGSY) is a flagship programme of <br/>");
                    msgBody.Append("Government of India for providing good quality road connectivity in rural areas.<br/>");
                    msgBody.Append("National Rural Infrastructure Development Agency (NRIDA), an agency of the<br/>");
                    msgBody.Append("Union Ministry of Rural Development provides technical and managerial<br/>");
                    msgBody.Append("support to the States for implementing the programme.<br/><br/>");

                    msgBody.Append("      A three tier Quality Management System has been envisaged for<br/>");
                    msgBody.Append("monitoring the quality of projects under the programme. First two tiers are<br/>");
                    msgBody.Append("managed by the respective States implementing the programme. Under the <br/>");
                    msgBody.Append("third tier; NRIDA empanels senior civil engineers from Central / State<br/>");
                    msgBody.Append("Governments departments including PSUs as also serving / retired faculty<br/>");
                    msgBody.Append("members of Government engineering colleges / IITs/ NITs/ Government<br/>");
                    msgBody.Append("research institutes, as National Quality Monitors (NQMs). These NQMs are<br/>");
                    msgBody.Append("required to carry out field inspections of PMGSY road and bridge projects, in a<br/>");
                    msgBody.Append("span of 10-12 days in a month.<br/><br/>");

                    msgBody.Append("NRIDA has advertised the requirement for empanelment of NQMs in leading<br/>");
                    msgBody.Append("National news paper. The details of eligibility criteria, application format,<br/>");
                    msgBody.Append("honorarium and travelling allowances etc. are available at <font color=blue> <u>www.pmgsy.nic.in</u></font>.<br/>");
                    msgBody.Append("The same is also enclosed herewith for ready reference.<br/><br/>");


                    msgBody.Append("You are working as State Quality Monitor in PMGSY and have exposure of <br/>");
                    msgBody.Append("PMGSY Three Quality Mechanism. You may like to work as National Quality<br/>");

                    msgBody.Append("Monitor or some other officers known to you may be willing to work. You may<br/>");
                    msgBody.Append("share the enclosed NQMs  requirement amongst the interested civil engineers<br/>");
                    msgBody.Append("We believe that knowledge sharing by the retired senior engineers and<br/>");
                    msgBody.Append("academicians would further augment the quality of built infrastructure under <br/>");
                    msgBody.Append("PMGSY.<br/><br/>");
                    msgBody.Append("Encl. As above.<br/>");

                    msgBody.Append("&nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp&nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp Best Wishes.<br/><br/>");
                    msgBody.Append("&nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp&nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp <b>Dr. I. K. Pateriya</b><br/>");
                    msgBody.Append("&nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp&nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp Director (Projects-III), NRIDA &<br/>");
                    msgBody.Append("&nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp&nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp Chief Quality Coordinator (PMGSY)<br/>");
                    mailMessage.Body = msgBody.ToString();
                    string AttachmentFilePath = System.Configuration.ConfigurationSettings.AppSettings["MailAttachmentPath"];//"~/mdck/RevisedWeblinkForPublishing_041020.pdf";//"D:\\OmmasImages\\TRACEMAPSHAB\\RevisedWeblinkForPublishing_041020.pdf";
                    string finalPath = AttachmentFilePath + "\\RevisedWeblinkForPublishing_041020.pdf";
                    mailMessage.Attachments.Add(new System.Net.Mail.Attachment(finalPath));
                    mailMessage.IsBodyHtml = true;
                    mailMessage.Priority = MailPriority.High;

                    // Added by Srishti on 13-03-2023
                    SmtpClient client = new SmtpClient();
                    string e_EuthHost = ConfigurationManager.AppSettings["e_EuthHost"];
                    string e_EuthPort = ConfigurationManager.AppSettings["e_EuthPort"];
                    string e_EuthMailUserName = ConfigurationManager.AppSettings["e_EuthMailUserName"];
                    string e_EuthMailPassword = ConfigurationManager.AppSettings["e_EuthMailPassword"];

                    client.Host = e_EuthHost;
                    client.Port = Convert.ToInt32(e_EuthPort);
                    client.UseDefaultCredentials = false;
                    client.EnableSsl = true; // Change to true
                    client.Credentials = new NetworkCredential(e_EuthMailUserName, e_EuthMailPassword);
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;

                    // Srishti changes end

                    await Task.Run(() =>
                    {
                        // Commented and added by Srishti on 13-03-2023
                        //mailMessage.Send();
                        client.Send(mailMessage);
                    });
                }

                return Json(new { success = true, message = "Mail sent to SQM Sucessfully." }, JsonRequestBehavior.AllowGet);

                // return null;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EmargDataPullController.SendEmailToSQM()");
                return Json(new { success = true, message = "Mail not sent. ( " + ex.Message + " )" }, JsonRequestBehavior.AllowGet);

            }
        }

        public async Task<ActionResult> SendEmailToSQM4()
        {
            // select ADMIN_QM_EMAIL from omms.ADMIN_QUALITY_MONITORS where ADMIN_QM_TYPE='S' and ADMIN_QM_EMPANELLED='Y'  and ADMIN_QM_EMAIL is not null-- SQM
            try
            {
                PMGSYEntities dbcontext = new PMGSYEntities();

                var emailDetails = dbcontext.ADMIN_QUALITY_MONITORS.Where(m => m.ADMIN_QM_TYPE == "S" && m.ADMIN_QM_EMPANELLED == "Y" && m.ADMIN_QM_EMAIL != null && m.ADMIN_QM_EMAIL != String.Empty && m.ADMIN_QM_CODE >= 3165 && m.ADMIN_QM_CODE <= 3273).ToList();




                foreach (var email in emailDetails)
                {

                    String toEmail = email.ADMIN_QM_EMAIL.Trim();// Request.Params["ToEmail"];
                    String BccEmail = String.Empty;//Request.Params["BccEmail"];
                    StringBuilder msgBody = new StringBuilder();


                    //Commented by Srishti on 13-03-2023
                    //var mailMessage = new Mvc.Mailer.MvcMailMessage
                    // Added by Srishti on 13-03-2023
                    ServicePointManager.ServerCertificateValidationCallback += (s, ce, ca, p) => true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    var mailMessage = new MailMessage
                    {
                        Subject = "Advertisement for Empanelment of National Quality Monitors (NQMs) for inspection of Road and Bridge Projects under PMGSY"
                    };
                    mailMessage.To.Add(toEmail);

                    if (BccEmail != String.Empty && BccEmail != null)
                    {
                        mailMessage.Bcc.Add(BccEmail);

                    }



                    //mailMessage.Body = "This is Test email.";
                    msgBody.Append("Sir, <br/>");
                    msgBody.Append("      Pradhan Mantri Gram Sadak Yojana (PMGSY) is a flagship programme of <br/>");
                    msgBody.Append("Government of India for providing good quality road connectivity in rural areas.<br/>");
                    msgBody.Append("National Rural Infrastructure Development Agency (NRIDA), an agency of the<br/>");
                    msgBody.Append("Union Ministry of Rural Development provides technical and managerial<br/>");
                    msgBody.Append("support to the States for implementing the programme.<br/><br/>");

                    msgBody.Append("      A three tier Quality Management System has been envisaged for<br/>");
                    msgBody.Append("monitoring the quality of projects under the programme. First two tiers are<br/>");
                    msgBody.Append("managed by the respective States implementing the programme. Under the <br/>");
                    msgBody.Append("third tier; NRIDA empanels senior civil engineers from Central / State<br/>");
                    msgBody.Append("Governments departments including PSUs as also serving / retired faculty<br/>");
                    msgBody.Append("members of Government engineering colleges / IITs/ NITs/ Government<br/>");
                    msgBody.Append("research institutes, as National Quality Monitors (NQMs). These NQMs are<br/>");
                    msgBody.Append("required to carry out field inspections of PMGSY road and bridge projects, in a<br/>");
                    msgBody.Append("span of 10-12 days in a month.<br/><br/>");

                    msgBody.Append("NRIDA has advertised the requirement for empanelment of NQMs in leading<br/>");
                    msgBody.Append("National news paper. The details of eligibility criteria, application format,<br/>");
                    msgBody.Append("honorarium and travelling allowances etc. are available at <font color=blue> <u>www.pmgsy.nic.in</u></font>.<br/>");
                    msgBody.Append("The same is also enclosed herewith for ready reference.<br/><br/>");


                    msgBody.Append("You are working as State Quality Monitor in PMGSY and have exposure of <br/>");
                    msgBody.Append("PMGSY Three Quality Mechanism. You may like to work as National Quality<br/>");

                    msgBody.Append("Monitor or some other officers known to you may be willing to work. You may<br/>");
                    msgBody.Append("share the enclosed NQMs  requirement amongst the interested civil engineers<br/>");
                    msgBody.Append("We believe that knowledge sharing by the retired senior engineers and<br/>");
                    msgBody.Append("academicians would further augment the quality of built infrastructure under <br/>");
                    msgBody.Append("PMGSY.<br/><br/>");
                    msgBody.Append("Encl. As above.<br/>");

                    msgBody.Append("&nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp&nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp Best Wishes.<br/><br/>");
                    msgBody.Append("&nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp&nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp <b>Dr. I. K. Pateriya</b><br/>");
                    msgBody.Append("&nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp&nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp Director (Projects-III), NRIDA &<br/>");
                    msgBody.Append("&nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp&nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp Chief Quality Coordinator (PMGSY)<br/>");
                    mailMessage.Body = msgBody.ToString();
                    string AttachmentFilePath = System.Configuration.ConfigurationSettings.AppSettings["MailAttachmentPath"];//"~/mdck/RevisedWeblinkForPublishing_041020.pdf";//"D:\\OmmasImages\\TRACEMAPSHAB\\RevisedWeblinkForPublishing_041020.pdf";
                    string finalPath = AttachmentFilePath + "\\RevisedWeblinkForPublishing_041020.pdf";
                    mailMessage.Attachments.Add(new System.Net.Mail.Attachment(finalPath));
                    mailMessage.IsBodyHtml = true;
                    mailMessage.Priority = MailPriority.High;

                    // Added by Srishti on 13-03-2023
                    SmtpClient client = new SmtpClient();
                    string e_EuthHost = ConfigurationManager.AppSettings["e_EuthHost"];
                    string e_EuthPort = ConfigurationManager.AppSettings["e_EuthPort"];
                    string e_EuthMailUserName = ConfigurationManager.AppSettings["e_EuthMailUserName"];
                    string e_EuthMailPassword = ConfigurationManager.AppSettings["e_EuthMailPassword"];

                    client.Host = e_EuthHost;
                    client.Port = Convert.ToInt32(e_EuthPort);
                    client.UseDefaultCredentials = false;
                    client.EnableSsl = true; // Change to true
                    client.Credentials = new NetworkCredential(e_EuthMailUserName, e_EuthMailPassword);
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;

                    // Srishti changes end

                    await Task.Run(() =>
                    {
                        // Commented and added by Srishti on 13-03-2023
                        //mailMessage.Send();
                        client.Send(mailMessage);
                    });
                }

                return Json(new { success = true, message = "Mail sent to SQM Sucessfully." }, JsonRequestBehavior.AllowGet);

                // return null;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EmargDataPullController.SendEmailToSQM()");
                return Json(new { success = true, message = "Mail not sent. ( " + ex.Message + " )" }, JsonRequestBehavior.AllowGet);

            }
        }

        public async Task<ActionResult> SendEmailToSQM5()
        {
            // select ADMIN_QM_EMAIL from omms.ADMIN_QUALITY_MONITORS where ADMIN_QM_TYPE='S' and ADMIN_QM_EMPANELLED='Y'  and ADMIN_QM_EMAIL is not null-- SQM
            try
            {
                PMGSYEntities dbcontext = new PMGSYEntities();

                var emailDetails = dbcontext.ADMIN_QUALITY_MONITORS.Where(m => m.ADMIN_QM_TYPE == "S" && m.ADMIN_QM_EMPANELLED == "Y" && m.ADMIN_QM_EMAIL != null && m.ADMIN_QM_EMAIL != String.Empty && m.ADMIN_QM_CODE >= 3274 && m.ADMIN_QM_CODE <= 3374).ToList();




                foreach (var email in emailDetails)
                {

                    String toEmail = email.ADMIN_QM_EMAIL.Trim();// Request.Params["ToEmail"];
                    String BccEmail = String.Empty;//Request.Params["BccEmail"];
                    StringBuilder msgBody = new StringBuilder();


                    //Commented by Srishti on 13-03-2023
                    //var mailMessage = new Mvc.Mailer.MvcMailMessage
                    // Added by Srishti on 13-03-2023
                    ServicePointManager.ServerCertificateValidationCallback += (s, ce, ca, p) => true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    var mailMessage = new MailMessage
                    {
                        Subject = "Advertisement for Empanelment of National Quality Monitors (NQMs) for inspection of Road and Bridge Projects under PMGSY"
                    };
                    mailMessage.To.Add(toEmail);

                    if (BccEmail != String.Empty && BccEmail != null)
                    {
                        mailMessage.Bcc.Add(BccEmail);

                    }



                    //mailMessage.Body = "This is Test email.";
                    msgBody.Append("Sir, <br/>");
                    msgBody.Append("      Pradhan Mantri Gram Sadak Yojana (PMGSY) is a flagship programme of <br/>");
                    msgBody.Append("Government of India for providing good quality road connectivity in rural areas.<br/>");
                    msgBody.Append("National Rural Infrastructure Development Agency (NRIDA), an agency of the<br/>");
                    msgBody.Append("Union Ministry of Rural Development provides technical and managerial<br/>");
                    msgBody.Append("support to the States for implementing the programme.<br/><br/>");

                    msgBody.Append("      A three tier Quality Management System has been envisaged for<br/>");
                    msgBody.Append("monitoring the quality of projects under the programme. First two tiers are<br/>");
                    msgBody.Append("managed by the respective States implementing the programme. Under the <br/>");
                    msgBody.Append("third tier; NRIDA empanels senior civil engineers from Central / State<br/>");
                    msgBody.Append("Governments departments including PSUs as also serving / retired faculty<br/>");
                    msgBody.Append("members of Government engineering colleges / IITs/ NITs/ Government<br/>");
                    msgBody.Append("research institutes, as National Quality Monitors (NQMs). These NQMs are<br/>");
                    msgBody.Append("required to carry out field inspections of PMGSY road and bridge projects, in a<br/>");
                    msgBody.Append("span of 10-12 days in a month.<br/><br/>");

                    msgBody.Append("NRIDA has advertised the requirement for empanelment of NQMs in leading<br/>");
                    msgBody.Append("National news paper. The details of eligibility criteria, application format,<br/>");
                    msgBody.Append("honorarium and travelling allowances etc. are available at <font color=blue> <u>www.pmgsy.nic.in</u></font>.<br/>");
                    msgBody.Append("The same is also enclosed herewith for ready reference.<br/><br/>");


                    msgBody.Append("You are working as State Quality Monitor in PMGSY and have exposure of <br/>");
                    msgBody.Append("PMGSY Three Quality Mechanism. You may like to work as National Quality<br/>");

                    msgBody.Append("Monitor or some other officers known to you may be willing to work. You may<br/>");
                    msgBody.Append("share the enclosed NQMs  requirement amongst the interested civil engineers<br/>");
                    msgBody.Append("We believe that knowledge sharing by the retired senior engineers and<br/>");
                    msgBody.Append("academicians would further augment the quality of built infrastructure under <br/>");
                    msgBody.Append("PMGSY.<br/><br/>");
                    msgBody.Append("Encl. As above.<br/>");

                    msgBody.Append("&nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp&nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp Best Wishes.<br/><br/>");
                    msgBody.Append("&nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp&nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp <b>Dr. I. K. Pateriya</b><br/>");
                    msgBody.Append("&nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp&nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp Director (Projects-III), NRIDA &<br/>");
                    msgBody.Append("&nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp&nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp Chief Quality Coordinator (PMGSY)<br/>");
                    mailMessage.Body = msgBody.ToString();
                    string AttachmentFilePath = System.Configuration.ConfigurationSettings.AppSettings["MailAttachmentPath"];//"~/mdck/RevisedWeblinkForPublishing_041020.pdf";//"D:\\OmmasImages\\TRACEMAPSHAB\\RevisedWeblinkForPublishing_041020.pdf";
                    string finalPath = AttachmentFilePath + "\\RevisedWeblinkForPublishing_041020.pdf";
                    mailMessage.Attachments.Add(new System.Net.Mail.Attachment(finalPath));
                    mailMessage.IsBodyHtml = true;
                    mailMessage.Priority = MailPriority.High;

                    // Added by Srishti on 13-03-2023
                    SmtpClient client = new SmtpClient();
                    string e_EuthHost = ConfigurationManager.AppSettings["e_EuthHost"];
                    string e_EuthPort = ConfigurationManager.AppSettings["e_EuthPort"];
                    string e_EuthMailUserName = ConfigurationManager.AppSettings["e_EuthMailUserName"];
                    string e_EuthMailPassword = ConfigurationManager.AppSettings["e_EuthMailPassword"];

                    client.Host = e_EuthHost;
                    client.Port = Convert.ToInt32(e_EuthPort);
                    client.UseDefaultCredentials = false;
                    client.EnableSsl = true; // Change to true
                    client.Credentials = new NetworkCredential(e_EuthMailUserName, e_EuthMailPassword);
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;

                    // Srishti changes end

                    await Task.Run(() =>
                    {
                        // Commented and added by Srishti on 13-03-2023
                        //mailMessage.Send();
                        client.Send(mailMessage);
                    });
                }

                return Json(new { success = true, message = "Mail sent to SQM Sucessfully." }, JsonRequestBehavior.AllowGet);

                // return null;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EmargDataPullController.SendEmailToSQM()");
                return Json(new { success = true, message = "Mail not sent. ( " + ex.Message + " )" }, JsonRequestBehavior.AllowGet);

            }
        }

        public async Task<ActionResult> SendEmailToSQM6()
        {
            // select ADMIN_QM_EMAIL from omms.ADMIN_QUALITY_MONITORS where ADMIN_QM_TYPE='S' and ADMIN_QM_EMPANELLED='Y'  and ADMIN_QM_EMAIL is not null-- SQM
            try
            {
                PMGSYEntities dbcontext = new PMGSYEntities();

                var emailDetails = dbcontext.ADMIN_QUALITY_MONITORS.Where(m => m.ADMIN_QM_TYPE == "S" && m.ADMIN_QM_EMPANELLED == "Y" && m.ADMIN_QM_EMAIL != null && m.ADMIN_QM_EMAIL != String.Empty && m.ADMIN_QM_CODE >= 3375 && m.ADMIN_QM_CODE <= 3423).ToList();




                foreach (var email in emailDetails)
                {

                    String toEmail = email.ADMIN_QM_EMAIL.Trim();// Request.Params["ToEmail"];
                    String BccEmail = String.Empty;//Request.Params["BccEmail"];
                    StringBuilder msgBody = new StringBuilder();


                    //Commented by Srishti on 13-03-2023
                    //var mailMessage = new Mvc.Mailer.MvcMailMessage
                    // Added by Srishti on 13-03-2023
                    ServicePointManager.ServerCertificateValidationCallback += (s, ce, ca, p) => true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    var mailMessage = new MailMessage
                    {
                        Subject = "Advertisement for Empanelment of National Quality Monitors (NQMs) for inspection of Road and Bridge Projects under PMGSY"
                    };
                    mailMessage.To.Add(toEmail);

                    if (BccEmail != String.Empty && BccEmail != null)
                    {
                        mailMessage.Bcc.Add(BccEmail);

                    }



                    //mailMessage.Body = "This is Test email.";
                    msgBody.Append("Sir, <br/>");
                    msgBody.Append("      Pradhan Mantri Gram Sadak Yojana (PMGSY) is a flagship programme of <br/>");
                    msgBody.Append("Government of India for providing good quality road connectivity in rural areas.<br/>");
                    msgBody.Append("National Rural Infrastructure Development Agency (NRIDA), an agency of the<br/>");
                    msgBody.Append("Union Ministry of Rural Development provides technical and managerial<br/>");
                    msgBody.Append("support to the States for implementing the programme.<br/><br/>");

                    msgBody.Append("      A three tier Quality Management System has been envisaged for<br/>");
                    msgBody.Append("monitoring the quality of projects under the programme. First two tiers are<br/>");
                    msgBody.Append("managed by the respective States implementing the programme. Under the <br/>");
                    msgBody.Append("third tier; NRIDA empanels senior civil engineers from Central / State<br/>");
                    msgBody.Append("Governments departments including PSUs as also serving / retired faculty<br/>");
                    msgBody.Append("members of Government engineering colleges / IITs/ NITs/ Government<br/>");
                    msgBody.Append("research institutes, as National Quality Monitors (NQMs). These NQMs are<br/>");
                    msgBody.Append("required to carry out field inspections of PMGSY road and bridge projects, in a<br/>");
                    msgBody.Append("span of 10-12 days in a month.<br/><br/>");

                    msgBody.Append("NRIDA has advertised the requirement for empanelment of NQMs in leading<br/>");
                    msgBody.Append("National news paper. The details of eligibility criteria, application format,<br/>");
                    msgBody.Append("honorarium and travelling allowances etc. are available at <font color=blue> <u>www.pmgsy.nic.in</u></font>.<br/>");
                    msgBody.Append("The same is also enclosed herewith for ready reference.<br/><br/>");


                    msgBody.Append("You are working as State Quality Monitor in PMGSY and have exposure of <br/>");
                    msgBody.Append("PMGSY Three Quality Mechanism. You may like to work as National Quality<br/>");

                    msgBody.Append("Monitor or some other officers known to you may be willing to work. You may<br/>");
                    msgBody.Append("share the enclosed NQMs  requirement amongst the interested civil engineers<br/>");
                    msgBody.Append("We believe that knowledge sharing by the retired senior engineers and<br/>");
                    msgBody.Append("academicians would further augment the quality of built infrastructure under <br/>");
                    msgBody.Append("PMGSY.<br/><br/>");
                    msgBody.Append("Encl. As above.<br/>");

                    msgBody.Append("&nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp&nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp Best Wishes.<br/><br/>");
                    msgBody.Append("&nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp&nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp <b>Dr. I. K. Pateriya</b><br/>");
                    msgBody.Append("&nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp&nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp Director (Projects-III), NRIDA &<br/>");
                    msgBody.Append("&nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp&nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp Chief Quality Coordinator (PMGSY)<br/>");
                    mailMessage.Body = msgBody.ToString();
                    string AttachmentFilePath = System.Configuration.ConfigurationSettings.AppSettings["MailAttachmentPath"];//"~/mdck/RevisedWeblinkForPublishing_041020.pdf";//"D:\\OmmasImages\\TRACEMAPSHAB\\RevisedWeblinkForPublishing_041020.pdf";
                    string finalPath = AttachmentFilePath + "\\RevisedWeblinkForPublishing_041020.pdf";
                    mailMessage.Attachments.Add(new System.Net.Mail.Attachment(finalPath));
                    mailMessage.IsBodyHtml = true;
                    mailMessage.Priority = MailPriority.High;

                    // Added by Srishti on 13-03-2023
                    SmtpClient client = new SmtpClient();
                    string e_EuthHost = ConfigurationManager.AppSettings["e_EuthHost"];
                    string e_EuthPort = ConfigurationManager.AppSettings["e_EuthPort"];
                    string e_EuthMailUserName = ConfigurationManager.AppSettings["e_EuthMailUserName"];
                    string e_EuthMailPassword = ConfigurationManager.AppSettings["e_EuthMailPassword"];

                    client.Host = e_EuthHost;
                    client.Port = Convert.ToInt32(e_EuthPort);
                    client.UseDefaultCredentials = false;
                    client.EnableSsl = true; // Change to true
                    client.Credentials = new NetworkCredential(e_EuthMailUserName, e_EuthMailPassword);
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;

                    // Srishti changes end

                    await Task.Run(() =>
                    {
                        // Commented and added by Srishti on 13-03-2023
                        //mailMessage.Send();
                        client.Send(mailMessage);
                    });
                }

                return Json(new { success = true, message = "Mail sent to SQM Sucessfully." }, JsonRequestBehavior.AllowGet);

                // return null;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EmargDataPullController.SendEmailToSQM()");
                return Json(new { success = true, message = "Mail not sent. ( " + ex.Message + " )" }, JsonRequestBehavior.AllowGet);

            }
        }


        #endregion

        // NEW ADDED ON 05-05-2022

        #region EMARG Roadwise Expenditure Balance Work

        public async Task<ActionResult> GetRoadWiseExpenditureBalanceWork()
        {

            try
            {
                EmargDAL emargDALDetails = new EmargDAL();
                ServicePointManager.Expect100Continue = true; // Added on 29 Dec 2021
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                string errorLogPath = System.Configuration.ConfigurationSettings.AppSettings["EmargErrorPath"];

                //string URL = "https://emarg.gov.in/emargrest_for_development/rest/expenditure/getroadwiseexpenditurebalancework";
                string URL = "https://emarg.gov.in/emargrest/rest/expenditure/getroadwiseexpenditurebalancework";

                Uri uri = new Uri(URL);
                WebClient wc = new WebClient();
                string token = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes("cdcweb" + ":" + "gis@cdc098"));
                wc.Headers.Add("Authorization", "Basic " + token);
                wc.UseDefaultCredentials = true;
                string results;
                results = wc.DownloadString(uri);

                //using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine(errorLogPath + "\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                //{
                //    sw.WriteLine("---------------------------------------------------------------------------------------");
                //    sw.WriteLine("Date :" + DateTime.Now.ToString());
                //    sw.WriteLine("Json Data from GetRoadWiseExpenditureBalanceWork()= : " + results);

                //    sw.WriteLine("---------------------------------------------------------------------------------------");
                //    sw.Close();
                //}

                JavaScriptSerializer ser = new JavaScriptSerializer();
                ser.MaxJsonLength = Int32.MaxValue;
                List<EMARG_ROADWISE_EXPENDITURE_BALANCEWORK> lst = ser.Deserialize<List<EMARG_ROADWISE_EXPENDITURE_BALANCEWORK>>(results);

                await Task.Run(() =>
                {
                    emargDALDetails.SaveRoadWiseExpenditureBalanceWork(lst);
                });

                return Json(new { success = true, message = "Details received Successfully." }, JsonRequestBehavior.AllowGet);
                // return null; // Uncommented on 30 March 2020
            }
            catch (Exception ex)
            {
                string errorLogPath = System.Configuration.ConfigurationSettings.AppSettings["EmargErrorPath"];

                ErrorLog.LogError(ex, "Inside Catch Method :: ! EmargDataPullController.GetRoadWiseExpenditureBalanceWork()");
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine(errorLogPath + "\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Catch Method= : " + ex.Message.ToString());

                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }
                //     return Json(new { success = false, message = "Details not received." }, JsonRequestBehavior.AllowGet);
                return Json(new { success = true, message = "Details are not received. ( " + ex.Message + " )" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        // NEW ADDED ON 05-05-2022

        #region EMARG Locked Package Record
        public async Task<ActionResult> GetLockedPackageRecord()
        {

            try
            {
                EmargDAL emargDALDetails = new EmargDAL();
                ServicePointManager.Expect100Continue = true; // Added on 29 Dec 2021
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                string URL = "https://emarg.gov.in/emargrest/rest/lockedPackageDetail/getlockedpackagerecord";
                //string URL = "https://emarg.gov.in/emargrest_for_development/rest/lockedPackageDetail/getlockedpackagerecord";

                Uri uri = new Uri(URL);
                WebClient wc = new WebClient();
                string token = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes("cdcweb" + ":" + "gis@cdc098"));
                wc.Headers.Add("Authorization", "Basic " + token);
                wc.UseDefaultCredentials = true;
                string results;
                results = wc.DownloadString(uri);
                JavaScriptSerializer ser = new JavaScriptSerializer();
                ser.MaxJsonLength = Int32.MaxValue;
                List<EMARG_LOCKED_PACKAGE_RECORD> lst = ser.Deserialize<List<EMARG_LOCKED_PACKAGE_RECORD>>(results);

                await Task.Run(() =>
                {
                    emargDALDetails.SaveLockedPackageRecord(lst);
                });

                return Json(new { success = true, message = "Details received Successfully." }, JsonRequestBehavior.AllowGet);
                // return null; // Uncommented on 30 March 2020
            }
            catch (Exception ex)
            {
                string errorLogPath = System.Configuration.ConfigurationSettings.AppSettings["EmargErrorPath"];

                ErrorLog.LogError(ex, "Inside Catch Method :: ! EmargDataPullController.GetLockedPackageRecord()");
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine(errorLogPath + "\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Catch Method= : " + ex.Message.ToString());

                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }
                //     return Json(new { success = false, message = "Details not received." }, JsonRequestBehavior.AllowGet);
                return Json(new { success = true, message = "Details are not received. ( " + ex.Message + " )" }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion


        //Added on 18-08-2022 by Shreyas
        #region Get KPI PM Awards Details
        public async Task<ActionResult> GetKPIPMAwardsDetails()
        {

            try
            {
                var userid = PMGSYSession.Current.UserId;
                var IPADDRESS = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                EmargDAL emargDALDetails = new EmargDAL();
                ServicePointManager.Expect100Continue = true; // Added on 29 Dec 2021
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                string URL = "https://emarg.gov.in/emargrest_for_development/rest/datadetail/getkpipmawardsdetails";
                //string URL = "https://emarg.gov.in/emargrest/rest/datadetail/getkpipmawardsdetails";

                Uri uri = new Uri(URL);
                WebClient wc = new WebClient();
                string token = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes("cdcweb" + ":" + "gis@cdc098"));
                string errorLogPath = System.Configuration.ConfigurationSettings.AppSettings["EmargErrorPath"];


                wc.Headers.Add("Authorization", "Basic " + token);
                wc.Headers.Add("dataDate", DateTime.Now.ToString("yyyy'-'MM'-'dd"));
                wc.UseDefaultCredentials = true;
                //string results;
                //results = wc.DownloadString(uri);
                var results = wc.UploadValues(uri, "POST", wc.Headers);
                string jsonStr = Encoding.UTF8.GetString(results);

                JavaScriptSerializer ser = new JavaScriptSerializer();
                ser.MaxJsonLength = Int32.MaxValue;
                List<Emarg_KPI_PM_Awardsdetails> lst = ser.Deserialize<List<Emarg_KPI_PM_Awardsdetails>>(jsonStr);

                int status = 0;
                await Task.Run(() =>
                {
                    status = emargDALDetails.SaveKPIPMAwardsDetails(lst, userid, IPADDRESS);
                });


                if (status > 0)
                {

                    using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine(errorLogPath+"\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                    {
                        sw.WriteLine("Date :" + DateTime.Now.ToString());
                        sw.WriteLine("STEP 7 : End Programme");
                        // sw.WriteLine("EID : " + data.EID);
                        sw.WriteLine("---------------------------------------------------------------------------------------");
                        sw.Close();
                    }
                    return Json(new { success = true, message = "Details received Successfully, number of rows affected = " + status }, JsonRequestBehavior.AllowGet);
                }
                else
                {

                    using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine(errorLogPath+"\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                    {
                        sw.WriteLine("Date :" + DateTime.Now.ToString());
                        sw.WriteLine("STEP 7 : End Programme");
                        // sw.WriteLine("EID : " + data.EID);
                        sw.WriteLine("---------------------------------------------------------------------------------------");
                        sw.Close();
                    }
                    return Json(new { success = true, message = "No new records received, number of rows affected = " + status }, JsonRequestBehavior.AllowGet);
                }

                //return Json(new { success = true, message = "Details received Successfully." }, JsonRequestBehavior.AllowGet);
                //return null; // Uncommented on 30 March 2020
            }
            catch (Exception ex)
            {
                string errorLogPath = System.Configuration.ConfigurationSettings.AppSettings["EmargErrorPath"];

                ErrorLog.LogError(ex, "Inside Catch Method :: ! EmargDataPullController.GetKPIPMAwardsDetails()");
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine(errorLogPath+"\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Catch Method= : " + ex.Message.ToString());

                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }
                //     return Json(new { success = false, message = "Details not received." }, JsonRequestBehavior.AllowGet);
                return Json(new { success = true, message = "Details are not received. ( " + ex.Message + " )" }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        //Added on 05-12-2022 by Bhushan
        #region Cancel Cheque Payment
        public async Task<ActionResult> CancelChequePayment()
        {

            try
            {
                EmargDAL emargDALDetails = new EmargDAL();
                ServicePointManager.Expect100Continue = true; // Added on 29 Dec 2020
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                string URL = "https://emarg.gov.in/emargrest/rest/expenditure/getroadwisedeclinedscrolldetails";

                Uri uri = new Uri(URL);
                WebClient wc = new WebClient();
                string token = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes("cdcweb" + ":" + "gis@cdc098"));
                wc.Headers.Add("Authorization", "Basic " + token);
                wc.UseDefaultCredentials = true;
                string results;
                results = wc.DownloadString(uri);


                JavaScriptSerializer ser = new JavaScriptSerializer();
                ser.MaxJsonLength = Int32.MaxValue;
                List<EMARG_CHEQUE_CANCELLATION_DETAILS_MODEL> lst = ser.Deserialize<List<EMARG_CHEQUE_CANCELLATION_DETAILS_MODEL>>(results);

                await Task.Run(() =>
                {
                    emargDALDetails.saveChequeCancellationDetails(lst);
                });

                return Json(new { success = true, message = "Details received Successfully." }, JsonRequestBehavior.AllowGet);
                // return null; // Uncommented on 30 March 2020
            }
            catch (Exception ex)
            {
                string errorLogPath = System.Configuration.ConfigurationSettings.AppSettings["EmargErrorPath"];

                ErrorLog.LogError(ex, "Inside Catch Method :: ! EmargDataPullController.CancelChequePayment()");
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine(errorLogPath + "\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Catch Method= : " + ex.Message.ToString());

                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }
                //     return Json(new { success = false, message = "Details not received." }, JsonRequestBehavior.AllowGet);
                return Json(new { success = true, message = "Details are not received. ( " + ex.Message + " )" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion



        //Test API
        public async Task<ActionResult> HoldingAPITest()
        {

            try
            {
                EmargDAL emargDALDetails = new EmargDAL();
                ServicePointManager.Expect100Continue = true; // Added on 29 Dec 2021
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                string errorLogPath = "D:\\OMMASII\\HoldingAPIResponse";

                if (!Directory.Exists(errorLogPath))
                {
                    Directory.CreateDirectory(errorLogPath);
                }

                //string URL = "https://emarg.gov.in/emargrest_for_development/rest/expenditure/getroadwiseexpenditurebalancework";
                string URL = "https://api.pfms.gov.in/api/BankAPI/GetBankDetail";



                Uri uri = new Uri(URL);
                WebClient wc = new WebClient();
                //string token = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes("NRIDA0037" + ":" + "cpsmsnrida@121!"));
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                wc.Headers.Add("userID", "NRIDA0037");
                wc.Headers.Add("pwd", "85ec62327d59d721335e430d164eaf39a5608ce77c5944aa699dbc56219ca209");
                wc.UseDefaultCredentials = true;
                string results;
                results = wc.DownloadString(uri);
                //results = wc.UploadString(uri.ToString(), wc.Headers.ToString());

                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine(errorLogPath + "\\HoldingAPIResponse" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Json Data from HoldingAPITest()= : " + results);

                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }

                JavaScriptSerializer ser = new JavaScriptSerializer();
                ser.MaxJsonLength = Int32.MaxValue;
                //List<EMARG_ROADWISE_EXPENDITURE_BALANCEWORK> lst = ser.Deserialize<List<EMARG_ROADWISE_EXPENDITURE_BALANCEWORK>>(results);

                //await Task.Run(() =>
                //{
                //    emargDALDetails.SaveRoadWiseExpenditureBalanceWork(lst);
                //});

                return Json(new { success = true, message = "Details received Successfully." }, JsonRequestBehavior.AllowGet);
                // return null; // Uncommented on 30 March 2020
            }
            catch (Exception ex)
            {
                string errorLogPath = System.Configuration.ConfigurationSettings.AppSettings["EmargErrorPath"];
                if (!Directory.Exists(errorLogPath))
                {
                    Directory.CreateDirectory(errorLogPath);
                }
                ErrorLog.LogError(ex, "Inside Catch Method :: ! EmargDataPullController.GetRoadWiseExpenditureBalanceWork()");
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine(errorLogPath + "\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Catch Method= : " + ex.Message.ToString());

                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }
                //     return Json(new { success = false, message = "Details not received." }, JsonRequestBehavior.AllowGet);
                return Json(new { success = true, message = "Details are not received. ( " + ex.Message + " )" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
