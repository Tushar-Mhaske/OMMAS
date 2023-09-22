#region HEADER
/*
 * Project Id:

 * Project Name:OMMAS-II

 * File Name: DishaService.asmx..cs

 * Author : Pradip Patil

 * Creation Date :13-12-2017

 * Desc : DAL for Disha.  
*/

#endregion



using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.Models;
using Newtonsoft.Json;
using PMGSY.Common;
using System.Web.Script.Serialization;
using System.IO;
namespace PMGSY.Disha
{
    public class DishaServiceDAL
    {
        public  string errorLogPath = System.Configuration.ConfigurationSettings.AppSettings["OMMASErrorLogPath"];
           
        PMGSYEntities dbContext = null;
        public List<USP_NSP_PHASE_PROFILE_DISHA_Result> GetNSPPhaseProfileData(string level, string state, string district, string agency, string pmgsy)
        {
            dbContext = new PMGSYEntities();
            int LevelCode, StateCode, DistrictCode, AgencyCode = 0;
            byte PMGSYScheme = 0;
            try
            {
                LevelCode = Convert.ToInt32(level);
                StateCode = Convert.ToInt32(state);
                DistrictCode = Convert.ToInt32(district);
                AgencyCode = Convert.ToInt32(agency);
                PMGSYScheme = Convert.ToByte(pmgsy);

                var Result = dbContext.USP_NSP_PHASE_PROFILE_DISHA(LevelCode, StateCode, DistrictCode, AgencyCode, PMGSYScheme).ToList();

                //return JsonConvert.SerializeObject(Result);
                return Result;
            }
            catch (Exception ex)
            {
                LogError(ex, "GetNSPPhaseProfileData()");
                return null;
            }
            finally
            {
             if(dbContext!=null)
                 dbContext.Dispose();
            }
        }

        public List<USP_PROJECT_STATISTICS_REPORT_DISHA_Result> PMGSY_ReportStatisticDAL(string level, string lgdstate)
        {
            dbContext = new PMGSYEntities();
            int LevelCode, StateCode = 0;
            byte PMGSYScheme = 0;
            try
            {
                LevelCode = Convert.ToInt32(level);
                StateCode = Convert.ToInt32(lgdstate);
                var Result = dbContext.USP_PROJECT_STATISTICS_REPORT_DISHA(LevelCode, StateCode).ToList();
                //return JsonConvert.SerializeObject(Result);
                return Result;
            }
            catch (Exception ex)
            {
                LogError(ex, "PMGSY_ReportStatisticDAL()");
                return null;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
        }
      
        public List<USP_QM_ABSTRACT_GRADING_STATE_WISE_DISHA_Result> PMGSY_AbstractGradingStatewiseDAL(int level, int lgdstate,int fromyear,int frommonth,int toyear,int tomonth,string qmtype)
        {
            dbContext = new PMGSYEntities();
            try
            {
               
                var Result = dbContext.USP_QM_ABSTRACT_GRADING_STATE_WISE_DISHA(level, lgdstate,fromyear,frommonth,toyear,tomonth,qmtype).ToList();

                //return JsonConvert.SerializeObject(Result);
                return Result;
            }
            catch (Exception ex)
            {
                 LogError(ex, "PMGSY_AbstractGradingStatewiseDAL()");
                return null;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
        }

        public List<USP_WORKS_SANCTION_PENDING_DISHA_Result> SanctionPendingWorksDishaDAL(int level, int month, int year, int lgdstate)
        {
            dbContext = new PMGSYEntities();
            try
            {
                var Result = dbContext.USP_WORKS_SANCTION_PENDING_DISHA(level,month,year,lgdstate).ToList();
                return Result;
            }
            catch (Exception ex)
            {
                 LogError(ex, "SanctionPendingWorksDishaDAL()");
                return null;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
        }

        public List<USP_TARGET_ACHIEVEMENT_DISHA_Result> TargetAchievementDishaDAL(int year,int ldgstate)
        {
            dbContext = new PMGSYEntities();
            try
            {
                return dbContext.USP_TARGET_ACHIEVEMENT_DISHA(year, ldgstate).ToList();
            }
            catch (Exception ex)
            {
                LogError(ex, "TargetAchievementDAL()");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public List<USP_SANCTION_HABITATION_DISHA_Result> SanctionedHabitaionsDAL(int ldgState)
        {
            dbContext = new PMGSYEntities();
            try
            {
                return dbContext.USP_SANCTION_HABITATION_DISHA(ldgState).ToList<USP_SANCTION_HABITATION_DISHA_Result>();
            }
            catch (Exception ex)
            {
                LogError(ex, "DishaDAL.SanctionedHabitaionsDAL()");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        private void LogError(Exception ex, String Method)
        {
            if (!Directory.Exists(errorLogPath))
            {
                Directory.CreateDirectory(errorLogPath);
            }
            using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine(errorLogPath + "\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
            {
                sw.WriteLine("Date :" + DateTime.Now.ToString());
                sw.WriteLine("Method : " + Method);
                sw.WriteLine("Exception : " + ex.Message);
                sw.WriteLine("Exception StackTrace: " + ex.StackTrace.ToString());
                if (ex.InnerException != null)
                {
                    sw.WriteLine("Inner Exception : " + ex.InnerException.Message);
                    sw.WriteLine("Exception StackTrace: " + ex.InnerException.StackTrace.ToString());
                }
                sw.WriteLine("____________________________________________________");
                sw.Close();
            }
        
        }

    }
}