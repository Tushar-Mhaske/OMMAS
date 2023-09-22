


using PMGSY.Areas.DynamicData.Models;
using PMGSY.Models;
using System;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.DynamicData.DAL
{
    public class DynamicDataDAL
    {
        private static string ErrorLogBasePath = System.Configuration.ConfigurationSettings.AppSettings["ABAProgressErrorPath"];
        private static string ErrorLogDirectory = ErrorLogBasePath + "\\" + "ErrorLog_" + DateTime.Now.Month + "_" + DateTime.Now.Year;
        private string ErrorLogFilePath = ErrorLogDirectory + "//ErrorLog_" + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + ".txt";


        public List<DYNAMIC_DASHBOARD> GeDynamictData()
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                    dbContext.Configuration.AutoDetectChangesEnabled = false;

                    List<DYNAMIC_DASHBOARD> objList = new List<DYNAMIC_DASHBOARD>();

                    objList = dbContext.DYNAMIC_DASHBOARD.ToList();

                    dbContext.Configuration.AutoDetectChangesEnabled = true;

                    return objList;
                   
                
            }
            catch (Exception ex)
            {
                dbContext.Configuration.AutoDetectChangesEnabled = true;
                return null;
            }
            finally
            {
                dbContext.Configuration.AutoDetectChangesEnabled = true;
            
            }

        }




        public dynamic GeDynamictDataByStateCode(int StateCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                var objList1 = (from DD in dbContext.DYNAMIC_DASHBOARD
                               where (StateCode==0) ? 1==1 : DD.SCODE == StateCode         // commented for current passing all state data
                                select new
                                {
                                    DD.MAST_STATE_NAME,
                                    DD.DISTRICT_NAME,
                                    DD.BLOCK_NAME,
                                    DD.IS_AWARDED,
                                    DD.SCHEME,
                                    DD.WTYPE,
                                    DD.RLENGTH,
                                    DD.BLENGTH,
                                    DD.CONNECTTYPE,
                                    DD.SCOST,
                                    DD.SYEAR,
                                    DD.SDDATE,

                                    DD.STAGED,
                                    DD.SPHASE,
                                    DD.PACKAGE,
                                    DD.RISCOMPLETED,
                                    DD.RBALANCE,
                                    DD.RPYEAR,
                                    DD.RPMONTH,
                                    DD.RCOMPLETIONDATE,
                                    DD.BPMONTH,
                                    DD.BPYEAR,
                                    DD.BCOMPLETIONDATE,
                                    DD.H1,
                                    DD.H2,
                                    DD.H3,
                                    DD.H4,

                                    DD.CH1,
                                    DD.CH2,
                                    DD.CH3,
                                    DD.CH4,
                                    DD.EXPENDITURE,
                                    DD.FBILLDATE,
                                    DD.FBILLSTATUS,
                                    DD.WORK_NAME,
                                    DD.GENERATED_DATE

                                }).ToList();
                return objList1;
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
                    sw.WriteLine("Method : " + "DynamicDataDAL.cs - GeDynamictDataByStateCode()");
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
                dbContext.Dispose();
            }

        }







    }
}