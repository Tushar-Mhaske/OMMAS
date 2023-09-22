using PMGSY.Common;
using PMGSY.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EmargDataPush
{
    public class EmargPushDetailsDAL
    {
        private static string ErrorLogBasePath = System.Configuration.ConfigurationSettings.AppSettings["EmargPushDetailsErrorPath"];
        private static string ErrorLogDirectory = ErrorLogBasePath + "\\" + "ErrorLog_" + DateTime.Now.Month + "_" + DateTime.Now.Year;
        private string ErrorLogFilePath = ErrorLogDirectory + "//ErrorLog_" + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + ".txt";


        PMGSYEntities dbContext = null;
        // GetCorrectedEmargDetailsDAL
        public dynamic GetCorrectedEmargDetailsDAL(string packageId)
        {//List<RoadDetails> 
           // List<RoadDetails> lstModel = new List<RoadDetails>();
        //    List<EMARG_COMPLETED_WORK_DETAILS_SERVICE> lstModel = new List<EMARG_COMPLETED_WORK_DETAILS_SERVICE>();

            //RoadDetails model = new RoadDetails();
            dbContext = new PMGSYEntities();


            try
            {
                ((System.Data.Entity.Infrastructure.IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;

              //  EMARG_COMPLETED_WORK_DETAILS_SERVICE model = new EMARG_COMPLETED_WORK_DETAILS_SERVICE();

                var emargStats = dbContext.USP_PUSH_CORRECTED_EMARG_DETAILS(packageId).ToList();

                return emargStats;
                //foreach (var item in emargStats)
                //{
                //    model.EID =item.EID;
                //    model.MAST_STATE_CODE = item.MAST_STATE_CODE;
                //    model.MAST_STATE_NAME = item.MAST_STATE_NAME;
                //    model.MAST_DISTRICT_CODE = item.MAST_DISTRICT_CODE;

                //    model.MAST_DISTRICT_NAME = item.MAST_DISTRICT_NAME;
                //    model.MAST_BLOCK_CODE = item.MAST_BLOCK_CODE;
                //    model.MAST_BLOCK_NAME = item.MAST_BLOCK_NAME;

                    
                //    model.PIU_CODE =item.PIU_CODE;
                //    model.PIU_NAME = item.PIU_NAME;
                //    model.ROAD_CODE =item.ROAD_CODE;

                //    model.SANCTION_YEAR = item.SANCTION_YEAR;
                   
                //    model.SANCTION_BATCH = item.SANCTION_BATCH;
                //    model.PACKAGE_NO = item.PACKAGE_NO;

                //    model.ROAD_NAME = item.ROAD_NAME;

                //    model.SANCTION_LENGTH = item.SANCTION_LENGTH;
                //    model.COMPLETED_LENGTH = item.COMPLETED_LENGTH;

                //    model.CC_LENGTH = item.CC_LENGTH;
                //    model.BT_LENGTH = item.BT_LENGTH;

                //    model.CDWorks = item.CDWorks;
                //    model.CN_CODE =item.CN_CODE;
                //    model.TRAFFIC_CATEGORY = item.TRAFFIC_CATEGORY;

                //    model.CARRIAGE_WAY_WIDTH = item.CARRIAGE_WAY_WIDTH;
                //    model.COMPLETION_DATE = item.COMPLETION_DATE;
                //    model.WORK_ORDER_NO = item.WORK_ORDER_NO;

                //    model.WORK_ORDER_DATE = item.WORK_ORDER_DATE;
                //    model.CONTRACTOR_NAME = item.CONTRACTOR_NAME;

                //    model.CONTRACTOR_PAN = item.CONTRACTOR_PAN;
                //    model.CONTRACTOR_ID = item.CONTRACTOR_ID;

                //    lstModel.Add(model);
                //}

               // return lstModel;
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
                    sw.WriteLine("Method : " + "EmargPushDetailsDAL.cs - GetCorrectedEmargDetailsDAL()");
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


        #region GetDRRPRoadDetailsDAL ---Added by Hrishikesh--

        public dynamic GetDRRPRoadDetailsDAL(int new_state_code)
        {
            dbContext = new PMGSYEntities();


            try
            {
                ((System.Data.Entity.Infrastructure.IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;
                //var DRRPRoadDetailsList = dbContext.MASTER_DRRP_ROAD.Where(x => x.STATE_CODE == mast_state_code).ToList();
               
                List<MasterDRRPRoadDetailsModel> DRRPRoadDetailsList = dbContext.MASTER_DRRP_ROAD.Where(x => x.STATE_CODE == new_state_code).Select(x => new MasterDRRPRoadDetailsModel
                {
                    ER_ROAD_CODE = x.ER_ROAD_CODE,
                    STATE_NAME = x.STATE_NAME,
                    DISTRICT_NAME = x.DISTRICT_NAME,
                    BLOCK_NAME = x.BLOCK_NAME,
                    ER_ROAD_NUMBER = x.ER_ROAD_NUMBER,
                    ROAD_CAT_NAME = x.ROAD_CAT_NAME,
                    ROAD_NAME = x.ROAD_NAME,
                    ROAD_STR_CHAINAGE = x.ROAD_STR_CHAINAGE,
                    ROAD_END_CHAINAGE = x.ROAD_END_CHAINAGE,
                    ROAD_C_WIDTH = x.ROAD_C_WIDTH,
                    ROAD_F_WIDTH = x.ROAD_F_WIDTH,
                    ROAD_L_WIDTH = x.ROAD_L_WIDTH,
                    ROAD_TYPE = x.ROAD_TYPE == "A" ? "All Weather" : x.ROAD_TYPE == "F" ? "Fair Weather" : "",
                    SOIL_TYPE_NAME = x.SOIL_TYPE_NAME,
                    TERRAIN_TYPE = x.TERRAIN_TYPE,
                    TERRAIN_NAME = x.TERRAIN_NAME,
                    DRRP_SCHEME = x.DRRP_SCHEME == 1 ? "PMGSY-I" : x.DRRP_SCHEME == 2 ? "PMGSY-II" : x.DRRP_SCHEME == 3 ? "RCPLWEA" : "PMGSY-III"
                }).ToList<MasterDRRPRoadDetailsModel>();

                return DRRPRoadDetailsList;

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
                    sw.WriteLine("Method : " + "EmargPushDetailsDAL.cs - GetDRRPRoadDetailsDAL()");
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
        #endregion
    }
}


 //model.EID = Convert.ToString(item.EID);
 //                   model.MAST_STATE_CODE = Convert.ToString(item.MAST_STATE_CODE);
 //                   model.MAST_STATE_NAME = item.MAST_STATE_NAME;
 //                   model.MAST_DISTRICT_CODE = Convert.ToString(item.MAST_DISTRICT_CODE);

 //                   model.MAST_DISTRICT_NAME = item.MAST_DISTRICT_NAME;
 //                   model.MAST_BLOCK_CODE = Convert.ToString(item.MAST_BLOCK_CODE);
 //                   model.MAST_BLOCK_NAME = item.MAST_BLOCK_NAME;

                    
 //                   model.PIU_CODE = Convert.ToString(item.PIU_CODE);
 //                   model.PIU_NAME = item.PIU_NAME;
 //                   model.ROAD_CODE = Convert.ToString(item.ROAD_CODE);

 //                   model.SANCTION_YEAR = item.SANCTION_YEAR;
                   
 //                   model.SANCTION_BATCH = Convert.ToString(item.SANCTION_BATCH);
 //                   model.PACKAGE_NO = item.PACKAGE_NO;

 //                   model.ROAD_NAME = item.ROAD_NAME;

 //                   model.SANCTION_LENGTH = Convert.ToString(item.SANCTION_LENGTH);
 //                   model.COMPLETED_LENGTH = Convert.ToString(item.COMPLETED_LENGTH);

 //                   model.CC_LENGTH = Convert.ToString(item.CC_LENGTH);
 //                   model.BT_LENGTH = Convert.ToString(item.BT_LENGTH);

 //                   model.CDWorks = Convert.ToString(item.CDWorks);
 //                   model.CN_CODE =item.CN_CODE;
 //                   model.SANCTION_BATCH = item.TRAFFIC_CATEGORY;

 //                   model.CARRIAGE_WAY_WIDTH = Convert.ToString(item.CARRIAGE_WAY_WIDTH);
 //                   model.COMPLETION_DATE = Convert.ToString(item.COMPLETION_DATE);
 //                   model.WORK_ORDER_NO = item.WORK_ORDER_NO;

 //                   model.WORK_ORDER_DATE = Convert.ToString(item.WORK_ORDER_DATE);
 //                   model.CONTRACTOR_NAME = item.CONTRACTOR_NAME;

 //                   model.CONTRACTOR_PAN = item.CONTRACTOR_PAN;
 //                   model.CONTRACTOR_ID = Convert.ToString(item.CONTRACTOR_ID);