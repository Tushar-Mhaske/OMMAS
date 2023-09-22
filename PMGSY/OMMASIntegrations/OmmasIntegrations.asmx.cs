using PMGSY.OmmasIntegrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using System.Web.Script.Serialization;
using System.IO;
using PMGSY.Common;
using System.Web.Mvc;

namespace PMGSY.OMMASIntegrations
{
    /// <summary>
    /// Summary description for OmmasIntegrations
    /// </summary>
    //[WebService(Namespace = "http://tempuri.org/")]
    [WebService(Namespace = "https://online.omms.nic.in/")]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class OmmasIntegrations : System.Web.Services.WebService
    {

        private static string ErrorLogBasePath = System.Configuration.ConfigurationSettings.AppSettings["OMMASIntegrationsErrorLogPath"];
        private static string ErrorLogDirectory = ErrorLogBasePath + "\\" + "ErrorLog_" + DateTime.Now.Month + "_" + DateTime.Now.Year;
        private string ErrorLogFilePath = ErrorLogDirectory + "//ErrorLog_" + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + ".txt";

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        /// <summary>
        /// Scheme 1 Overall Stats
        /// </summary>
        /// <returns></returns>
        public void Scheme1OverallStats(int StateCode, int DistrictCode, int BlockCode, int Scheme)
        {
            OmmasIntegrationsDAL objDAL = new OmmasIntegrationsDAL();
            List<WorksInformationModel> model = new List<WorksInformationModel>();
            try
            {
                model = objDAL.Scheme1OverallStatsDAL(StateCode, DistrictCode, BlockCode, Scheme);

                //return model;

                //HttpContext.Current.Response.Write("[" + new JavaScriptSerializer().Serialize(model) + "]");

                //HttpContext.Current.Response.Write(new JavaScriptSerializer().Serialize(model));

                var serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = Int32.MaxValue;

                HttpContext.Current.Response.Write(serializer.Serialize(model));
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
                    sw.WriteLine("Method : " + "OmmasIntegrations.cs - Scheme1OverallStats()");
                    sw.WriteLine("Exception : " + ex.ToString());
                    sw.WriteLine("Exception : " + ex.StackTrace);
                    if (ex.InnerException != null)
                        sw.WriteLine("Inner Exception : " + ex.InnerException.ToString());
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                //return null;

                //HttpContext.Current.Response.Write(new JavaScriptSerializer().Serialize("Something went wrong, please try again"));
            }
        }


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        /// <summary>
        /// TargetAchievement Overall Stats
        /// </summary>
        /// <returns></returns>
        public void TargetAchievement(int year)
        {
            OmmasIntegrationsDAL objDAL = new OmmasIntegrationsDAL();
            try
            {
                var serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = Int32.MaxValue;
                if (year < 0)
                {
                    HttpContext.Current.Response.Write(serializer.Serialize(new { message = "Invalid Year"}));
                }
                HttpContext.Current.Response.Write(serializer.Serialize(objDAL.TargetAchievementDAL(year)));
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
                    sw.WriteLine("Method : " + "OmmasIntegrations.cs - TargetAchievement()");
                    sw.WriteLine("Exception : " + ex.ToString());
                    sw.WriteLine("Exception : " + ex.StackTrace);
                    if (ex.InnerException != null)
                        sw.WriteLine("Inner Exception : " + ex.InnerException.ToString());
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                //return null;

                //HttpContext.Current.Response.Write(new JavaScriptSerializer().Serialize("Something went wrong, please try again"));
            }
        }

        #region MORD 
        /*[WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        /// <summary>
        /// Scheme 1 Overall Stats
        /// </summary>
        /// <returns></returns>
        public void OverallStatsMoRD(int Level,int StateCode, int DistrictCode, int BlockCode, int Year, int Collaboration, int Scheme)
        {
            OmmasIntegrationsDAL objDAL = new OmmasIntegrationsDAL();
            ShcemeOverAllStats model = null;
            try
            {
                model = objDAL.OverallStatsMoRDDAL(Level,StateCode, DistrictCode, BlockCode, Year, Collaboration, Scheme);

                var serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = Int32.MaxValue;

                HttpContext.Current.Response.Write(serializer.Serialize(model));
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "OmmasIntegrations.OverallStatsMoRD()");
                //return null;

                //HttpContext.Current.Response.Write(new JavaScriptSerializer().Serialize("Something went wrong, please try again"));
            }
        }*/

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        /// <summary>
        /// Scheme 1 Overall Stats
        /// </summary>
        /// <returns></returns>
        public void SanctionPendingWorksMoRD(int Month, int Year)
        {
            OmmasIntegrationsDAL objDAL = new OmmasIntegrationsDAL();
            List<SanctionWorksViewModel> model = new List<SanctionWorksViewModel>();
            try
            {

                model = objDAL.SanctionPendingWorksDAL(Month, Year);

                var serializer = new JavaScriptSerializer();
                Context.Response.ContentType = "application/json";
                serializer.MaxJsonLength = Int32.MaxValue;

                HttpContext.Current.Response.Write(serializer.Serialize(model));
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "OmmasIntegrations.SanctionPendingWorks()");
                //HttpContext.Current.Response.Write(new JavaScriptSerializer().Serialize("Something went wrong, please try again"));
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        /// <summary>
        /// Scheme 1 Overall Stats
        /// </summary>
        /// <returns></returns>
        public void HabConnectivityStatusMoRD(int Month, int Year)
        {
            OmmasIntegrationsDAL objDAL = new OmmasIntegrationsDAL();
            List<HabConnectivityStatusViewModel> model = new List<HabConnectivityStatusViewModel>();
            try
            {
                model = objDAL.HabConnectivityStatusDAL(Month, Year);

                var serializer = new JavaScriptSerializer();
                Context.Response.ContentType = "application/json";
                serializer.MaxJsonLength = Int32.MaxValue;

                HttpContext.Current.Response.Write(serializer.Serialize(model));
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "OmmasIntegrations.HabConnectivityStatus()");
                //HttpContext.Current.Response.Write(new JavaScriptSerializer().Serialize("Something went wrong, please try again"));
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        /// <summary>
        /// Scheme 1 Overall Stats
        /// </summary>
        /// <returns></returns>
        public void TargetAchievementMoRD(int Year, int State)
        {
            OmmasIntegrationsDAL objDAL = new OmmasIntegrationsDAL();
            //TargetAchievementViewModel model = null;
            List<TargetAchievementViewModel> model = new List<TargetAchievementViewModel>();
            try
            {
                if (Year <= 0)
                {
                    HttpContext.Current.Response.Write(new JavaScriptSerializer().Serialize("Please select a valid Year"));
                }

                model = objDAL.TargetAchievementMoRDDAL(Year, State);

                var serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = Int32.MaxValue;

                HttpContext.Current.Response.Write(serializer.Serialize(model));
            }
            //OmmasIntegrationsDAL objDAL = new OmmasIntegrationsDAL();
            //TargetAchievementViewModel model = null;
            //try
            //{
            //    if (Year <= 0)
            //    {
            //        HttpContext.Current.Response.Write(new JavaScriptSerializer().Serialize("Please select a valid Year"));
            //    }

            //    model = objDAL.TargetAchievementMoRDDAL(Year, State);

            //    var serializer = new JavaScriptSerializer();
            //    serializer.MaxJsonLength = Int32.MaxValue;

            //    HttpContext.Current.Response.Write(serializer.Serialize(model));
            //}
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "OmmasIntegrations.OverallStatsMoRD()");
                //return null;

                //HttpContext.Current.Response.Write(new JavaScriptSerializer().Serialize("Something went wrong, please try again"));
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        /// <summary>
        /// Scheme 1 Overall Stats
        /// </summary>
        /// <returns></returns>
        public void PhysicalFinancialProgressMoRD()
        {
            //OmmasIntegrationsDAL objDAL = new OmmasIntegrationsDAL();
            //PhysicalFinancialProgressViewModel model = null;
            //try
            //{
            //    model = objDAL.PhysicalFinancialProgressMoRDDAL();

            //    var serializer = new JavaScriptSerializer();
            //    serializer.MaxJsonLength = Int32.MaxValue;

            //    HttpContext.Current.Response.Write(serializer.Serialize(model));
            //}

            OmmasIntegrationsDAL objDAL = new OmmasIntegrationsDAL();
            //PhysicalFinancialProgressViewModel model = null;
            List<PhysicalFinancialProgressViewModel> model = new List<PhysicalFinancialProgressViewModel>();
            try
            {
                model = objDAL.PhysicalFinancialProgressMoRDDAL();

                var serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = Int32.MaxValue;

                HttpContext.Current.Response.Write(serializer.Serialize(model));
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "OmmasIntegrations.PhysicalFinancialProgressMoRD()");
                //return null;
                //HttpContext.Current.Response.Write(new JavaScriptSerializer().Serialize("Something went wrong, please try again"));
            }
        }
        #endregion
    }
}
