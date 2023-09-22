using PMGSY.Common;
using PMGSY.Models;
using PMGSY.Models.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.OverallStatus
{
    public class OverallStatusDAL
    {
        PMGSYEntities dbContext = null;

        //private static string ErrorLogBasePath = System.Configuration.ConfigurationSettings.AppSettings["CGSErrorLogPath"];
        //private static string ErrorLogDirectory = ErrorLogBasePath + "\\" + "ErrorLog_" + DateTime.Now.Month + "_" + DateTime.Now.Year;
        //private string ErrorLogFilePath = ErrorLogDirectory + "//CGSErrorLog_" + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + ".txt";


        #region GetOverallStatusDataDAL()
        /// <summary>
        /// Get SQC Details
        /// </summary>
        /// <returns></returns>
        public OverallStatusViewModel GetOverallStatusDataDAL()
        {
            List<OverallStatusViewModel> lstModel = new List<OverallStatusViewModel>();
            OverallStatusViewModel model = new OverallStatusViewModel();
            dbContext = new PMGSYEntities();
            try
            {
                ((System.Data.Entity.Infrastructure.IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;

                //Get LGD Habitations
                var pmgsy1Stats = dbContext.USP_PROJECT_STATISTICS_REPORT(1, 0, 0, 0, 0, 0, Convert.ToByte(1)).First();

                model.NoOfRoadWorksCleared = Convert.ToInt32(pmgsy1Stats.UROAD + pmgsy1Stats.NROAD);
                model.NewConnectivityWorks = Convert.ToInt32(pmgsy1Stats.NROAD);
                model.UpgradationConnectivityWorks = Convert.ToInt32(pmgsy1Stats.UROAD);
                model.CompletedRoadWorks = Convert.ToInt32(pmgsy1Stats.UC_COUNT + pmgsy1Stats.NC_COUNT);
                model.InProgressRoadWorks = Convert.ToInt32(model.NoOfRoadWorksCleared - model.CompletedRoadWorks);
                model.TotalLength = Convert.ToDecimal(pmgsy1Stats.ULEN + pmgsy1Stats.NLEN);

                model.NoOfComplaints = dbContext.ADMIN_FEEDBACK.Where(x => x.CITIZEN_ID != null).Select(x => x.CITIZEN_ID).Count();

               // lstModel.Add(model);

                return model;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "OverallStatusDAL.GetOverallStatusDataDAL()");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion Get LGD GetSQCDetailsDAL() Ends
    }
}