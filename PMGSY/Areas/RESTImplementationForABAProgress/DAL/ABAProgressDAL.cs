using PMGSY.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.RESTImplementationForABAProgress.DAL
{
    public class ABAProgressDAL
    {
        #region Aatmanirbhar Bharat
        private static string ErrorLogBasePath = System.Configuration.ConfigurationSettings.AppSettings["ABAProgressErrorPath"];
        private static string ErrorLogDirectory = ErrorLogBasePath + "\\" + "ErrorLog_" + DateTime.Now.Month + "_" + DateTime.Now.Year;
        private string ErrorLogFilePath = ErrorLogDirectory + "//ErrorLog_" + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + ".txt";


        PMGSYEntities dbContext = null;

        public dynamic GetABAProgressDAL(string generatedDate)
        {
            dbContext = new PMGSYEntities();
           // ABA_MIS_DATA table = new ABA_MIS_DATA();
            DateTime GeneratedDate = Convert.ToDateTime(generatedDate);

            try
            {
                ((System.Data.Entity.Infrastructure.IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;

             //   var abaProgressDetails = dbContext.USP_ABA_PROGRESS_DETAILS(DateTime.Now.Month,DateTime.Now.Year,0,0).ToList();
                var abaProgressDetails = dbContext.GET_DATA_FROM_ABA_MIS_DATA_SERVICE(GeneratedDate).ToList();


                return abaProgressDetails;
              
            }
            catch (Exception ex)
            {
             //   dbContext.Configuration.AutoDetectChangesEnabled = true;
                if (!Directory.Exists(ErrorLogDirectory))
                {
                    Directory.CreateDirectory(ErrorLogDirectory);
                }
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "RESTImplementationForABAProgress.ABAProgressDAL.cs - GetABAProgressDAL()");
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
               // dbContext.Configuration.AutoDetectChangesEnabled = true;
                dbContext.Dispose();
            }
        }
        #endregion 

        #region MORD Physical Progress Dashboard
        public dynamic GetMORDDashboardDAL(byte scheme, int month, int year)
        {
            dbContext = new PMGSYEntities();
           
            try
            {
                ((System.Data.Entity.Infrastructure.IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;

                var mordDashborad = dbContext.USP_MORD_DASHBOAED_PHYSICAL(scheme,month,year).ToList();


                return mordDashborad;

            }
            catch (Exception ex)
            {
                //   dbContext.Configuration.AutoDetectChangesEnabled = true;
                if (!Directory.Exists(ErrorLogDirectory))
                {
                    Directory.CreateDirectory(ErrorLogDirectory);
                }
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "RESTImplementationForABAProgress.ABAProgressDAL.cs - GetMORDDashboardDAL()");
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
                // dbContext.Configuration.AutoDetectChangesEnabled = true;
                dbContext.Dispose();
            }
        }
        #endregion

        #region MORD Physical Progress Dashboard PMGSY3
        public dynamic GetMORDDashboardPMGSY3DAL(byte scheme, int month, int year)
        {
            dbContext = new PMGSYEntities();

            try
            {
                ((System.Data.Entity.Infrastructure.IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;

                var mordDashboradpmgsy3 = dbContext.USP_MORD_DASHBOAED_PHYSICAL_PMGSY3(scheme, month, year).ToList();


                return mordDashboradpmgsy3;

            }
            catch (Exception ex)
            {
                //   dbContext.Configuration.AutoDetectChangesEnabled = true;
                if (!Directory.Exists(ErrorLogDirectory))
                {
                    Directory.CreateDirectory(ErrorLogDirectory);
                }
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "RESTImplementationForABAProgress.ABAProgressDAL.cs - GetMORDDashboardPMGSY3DAL()");
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
                // dbContext.Configuration.AutoDetectChangesEnabled = true;
                dbContext.Dispose();
            }
        }
        #endregion



        #region
        public dynamic GetStateRankDetailsDAL(int  StateCode, int month, int year)
        {
            dbContext = new PMGSYEntities();

            try
            {
                ((System.Data.Entity.Infrastructure.IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;

                var stateRank = dbContext.USP_STATE_RANK_OVERALLRANK_CALCULATION(StateCode, month, year).ToList();


                return stateRank;

            }
            catch (Exception ex)
            {
                //   dbContext.Configuration.AutoDetectChangesEnabled = true;
                if (!Directory.Exists(ErrorLogDirectory))
                {
                    Directory.CreateDirectory(ErrorLogDirectory);
                }
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "RESTImplementationForABAProgress.ABAProgressDAL.cs - GetStateRankDetailsDAL()");
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
                // dbContext.Configuration.AutoDetectChangesEnabled = true;
                dbContext.Dispose();
            }
        }
        #endregion
    }
}