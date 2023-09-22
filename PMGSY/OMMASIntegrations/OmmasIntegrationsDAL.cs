using PMGSY.Models;
using PMGSY.OmmasIntegrations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using PMGSY.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Web.Mvc;
using System.Data.Entity.Core;

namespace PMGSY.OMMASIntegrations
{
    public class OmmasIntegrationsDAL
    {
        PMGSYEntities dbContext = null;
        private static string ErrorLogBasePath = System.Configuration.ConfigurationSettings.AppSettings["OMMASIntegrationsErrorLogPath"];
        private static string ErrorLogDirectory = ErrorLogBasePath + "\\" + "ErrorLog_" + DateTime.Now.Month + "_" + DateTime.Now.Year;
        private string ErrorLogFilePath = ErrorLogDirectory + "//ErrorLog_" + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + ".txt";

        #region Works Information Scheme1OverallStatsDAL()
        /// <summary>
        /// Overall Stats for Scheme 1
        /// </summary>
        /// <returns></returns>
        public List<WorksInformationModel> Scheme1OverallStatsDAL(int StateCode, int DistrictCode, int BlockCode, int Scheme)
        {
            List<WorksInformationModel> lstModel = new List<WorksInformationModel>();
            dbContext = new PMGSYEntities();
            try
            {
                ((System.Data.Entity.Infrastructure.IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;

                //Scheme1 Overall Stats
                var pmgsy1Stats = dbContext.USP_PROJECT_STATISTICS_REPORT_OMMAS_INTEGRATION(StateCode, DistrictCode, BlockCode, Convert.ToByte(Scheme)).ToList();

                foreach (var item in pmgsy1Stats)
                {
                    WorksInformationModel model = new WorksInformationModel();
                    model.NoOfRoadWorksClearedScheme = Convert.ToInt32(item.UROAD + item.NROAD);
                    model.NewConnectivityScheme = Convert.ToInt32(item.NROAD);
                    model.UpgradationConnectivityScheme = Convert.ToInt32(item.UROAD);

                    model.CompletedRoadWorksScheme = Convert.ToInt32(item.UC_COUNT + item.NC_COUNT);
                    model.CompletedNewConnectivityScheme = item.NC_COUNT.HasValue ? item.NC_COUNT.Value : 0;
                    model.CompletedUpgradationScheme = item.UC_COUNT.HasValue ? item.UC_COUNT.Value : 0;
                    //model.InProgressRoadWorksScheme = Convert.ToInt32(pmgsy1Stats.UP_COUNT + pmgsy1Stats.NP_COUNT);

                    model.TotalLengthScheme = Convert.ToDecimal(item.ULEN + item.NLEN);
                    model.censusStateCode = Convert.ToInt32(item.CENSUS_STATE_CODE);
                    //model.censusDistrictCode = Convert.ToInt32(item.CENSUS_DISTRICT_CODE);
                    //model.censusBlockCode = Convert.ToInt32(item.CENSUS_BLOCK_CODE);

                    model.TotalSanctionedCost = Convert.ToDecimal(item.UCOST + item.NCOST);
                    model.TotalExpenditure = Convert.ToDecimal(item.UEXP + item.NEXP);

                    lstModel.Add(model);
                }

                return lstModel;
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
                    sw.WriteLine("Method : " + "OmmasIntegrationsDAL.cs - Scheme1OverallStatsDAL()");
                    sw.WriteLine("Exception : " + ex.Message);
                    sw.WriteLine("Exception : " + ex.StackTrace);
                    if (ex.InnerException != null)
                        sw.WriteLine("Inner Exception : " + ex.InnerException.StackTrace);
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

        /// <summary>
        /// This method returns the achievements for all state year wise
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public List<USP_TARGET_ACHIEVEMENT_Result> TargetAchievementDAL(int year)
        {
            dbContext = new PMGSYEntities();
            try
            {
                return dbContext.USP_TARGET_ACHIEVEMENT(year, 0).ToList();
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
                    sw.WriteLine("Method : " + "OmmasIntegrationsDAL.cs - TargetAchievementDAL()");
                    sw.WriteLine("Exception : " + ex.Message);
                    sw.WriteLine("Exception : " + ex.StackTrace);
                    if (ex.InnerException != null)
                        sw.WriteLine("Inner Exception : " + ex.InnerException.StackTrace);
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
        #endregion Works Information Scheme1OverallStatsDAL() Ends

        #region MORD
        public ShcemeOverAllStats OverallStatsMoRDDAL(int Level, int StateCode, int DistrictCode, int BlockCode, int Year, int Collaboration, int Scheme)
        {
            ShcemeOverAllStats model = new ShcemeOverAllStats();
            //List<WorksInformationModel> lstModel = new List<WorksInformationModel>();
            dbContext = new PMGSYEntities();
            try
            {
                ((System.Data.Entity.Infrastructure.IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;

                //Scheme1 Overall Stats
                var pmgsy1Stats = dbContext.USP_PROJECT_STATISTICS_REPORT(Level, StateCode, DistrictCode, BlockCode, Year, Collaboration, Convert.ToByte(Scheme)).FirstOrDefault();

                if (pmgsy1Stats != null)
                {

                    model.NoOfRoadWorksClearedScheme = Convert.ToInt32(pmgsy1Stats.UROAD + pmgsy1Stats.NROAD);
                    model.RoadLengthClearedScheme = Convert.ToDecimal(pmgsy1Stats.ULEN + pmgsy1Stats.NLEN);

                    model.CompletedRoadWorksScheme = Convert.ToInt32(pmgsy1Stats.UC_COUNT + pmgsy1Stats.NC_COUNT);
                    model.ExpenditureIncurred = Convert.ToDecimal(pmgsy1Stats.UEXP + pmgsy1Stats.NEXP);


                    //model.CompletedNewConnectivityScheme = item.NC_COUNT.HasValue ? item.NC_COUNT.Value : 0;
                    //model.CompletedUpgradationScheme = item.UC_COUNT.HasValue ? item.UC_COUNT.Value : 0;

                    //model.censusStateCode = Convert.ToInt32(item.CENSUS_STATE_CODE);
                    //model.TotalSanctionedCost = Convert.ToDecimal(item.UCOST + item.NCOST);

                }
                return model;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "OmmasIntegrationsDAL.OverallStatsDAL()");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public List<SanctionWorksViewModel> SanctionPendingWorksDAL(int Month, int Year)
        {
            List<SanctionWorksViewModel> lstModel = new List<SanctionWorksViewModel>();

            dbContext = new PMGSYEntities();
            try
            {
                ((System.Data.Entity.Infrastructure.IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;

                //Scheme1 Overall Stats
                var pmgsy1Stats = dbContext.USP_WORKS_SANCTION_PENDING(Month, Year,0).ToList();

                if (pmgsy1Stats != null)
                {
                    foreach (var item in pmgsy1Stats)
                    {
                        SanctionWorksViewModel model = new SanctionWorksViewModel();

                        model.StateName = item.MAST_STATE_NAME;
                        //model.SANCTION_NUMBER_ROAD = Convert.ToInt32(item.SANCTION_NUMBER_ROAD);
                        //model.SANCTION_LENGTH_ROAD = Convert.ToDecimal(item.SANCTION_LENGTH_ROAD);
                        //model.COMPLETED_NUMBER_ROAD = Convert.ToInt32(item.COMPLETED_NUMBER_ROAD);
                        //model.COMPLETED_LENGTH_ROAD = Convert.ToDecimal(item.COMPLETED_LENGTH_ROAD);

                        model.PROGRESS_NUMBER_ROAD = Convert.ToInt32(item.PROGRESS_NUMBER_ROAD);
                        model.PROGRESS_LENGTH_ROAD = Convert.ToDecimal(item.SANCTION_LENGTH_ROAD - item.COMPLETED_LENGTH_ROAD);
                        //model.PROGRESS_LENGTH_ROAD = Convert.ToDecimal(item.PROGRESS_LENGTH_ROAD);

                        //model.SANCTION_NUMBER_LSB = Convert.ToInt32(item.SANCTION_NUMBER_LSB);
                        //model.COMPLETED_NUMBER_LSB = Convert.ToInt32(item.COMPLETED_NUMBER_LSB);
                        
                        model.PROGRESS_NUMBER_LSB = Convert.ToInt32(item.PROGRESS_NUMBER_LSB);

                        model.PROGRESS_NUMBER_ROAD_LESS_THEN0 = Convert.ToInt32(item.PROGRESS_NUMBER_ROAD_LESS_THEN0);
                        model.PROGRESS_LENGTH_ROAD_LESS_THEN0 = Convert.ToDecimal(item.PROGRESS_LENGTH_ROAD_LESS_THEN0);
                        model.PROGRESS_NUMBER_ROAD_MORE_THAN1 = Convert.ToInt32(item.PROGRESS_NUMBER_ROAD_MORE_THAN1);
                        model.PROGRESS_LENGTH_ROAD_LESS_THEN1 = Convert.ToDecimal(item.PROGRESS_LENGTH_ROAD_LESS_THEN1);
                        model.PROGRESS_NUMBER_ROAD_MORE_THAN2 = Convert.ToInt32(item.PROGRESS_NUMBER_ROAD_MORE_THAN2);
                        model.PROGRESS_LENGTH_ROAD_LESS_THEN2 = Convert.ToDecimal(item.PROGRESS_LENGTH_ROAD_LESS_THEN2);
                        model.PROGRESS_NUMBER_ROAD_MORE_THAN3 = Convert.ToInt32(item.PROGRESS_NUMBER_ROAD_MORE_THAN3);
                        model.PROGRESS_LENGTH_ROAD_LESS_THEN3 = Convert.ToDecimal(item.PROGRESS_LENGTH_ROAD_LESS_THEN3);
                        model.PROGRESS_NUMBER_ROAD_MORE_THAN4 = Convert.ToInt32(item.PROGRESS_NUMBER_ROAD_MORE_THAN4);
                        model.PROGRESS_LENGTH_ROAD_MORE_THAN4 = Convert.ToDecimal(item.PROGRESS_LENGTH_ROAD_MORE_THAN4);
                        model.PROGRESS_NUMBER_LSB_LESS_THEN0 = Convert.ToInt32(item.PROGRESS_NUMBER_LSB_LESS_THEN0);
                        model.PROGRESS_NUMBER_LSB_MORE_THAN1 = Convert.ToInt32(item.PROGRESS_NUMBER_LSB_MORE_THAN1);
                        model.PROGRESS_NUMBER_LSB_MORE_THAN2 = Convert.ToInt32(item.PROGRESS_NUMBER_LSB_MORE_THAN2);
                        model.PROGRESS_NUMBER_LSB_MORE_THAN3 = Convert.ToInt32(item.PROGRESS_NUMBER_LSB_MORE_THAN3);
                        model.PROGRESS_NUMBER_LSB_MORE_THAN4 = Convert.ToInt32(item.PROGRESS_NUMBER_LSB_MORE_THAN4);


                        model.LGD_State_Code = Convert.ToInt32(item.censusStateCode);
                        lstModel.Add(model);
                    }
                }
                return lstModel;
            }
            catch (DbEntityValidationException e)
            {
                ModelStateDictionary modelstate = new ModelStateDictionary();

                foreach (var eve in e.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        //modelstate.AddModelError(ve.ErrorMessage, ve.ErrorMessage);
                        using (StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                        {
                            //sw.WriteLine("Date :" + DateTime.Now.ToString() + " UserName -" + PMGSYSession.Current.UserName);
                            sw.WriteLine("Method : " + "OmmasIntegrationsDAL.SanctionPendingWorksDAL(DbEntityValidationException ex)");
                            sw.WriteLine("Exception : " + e.ToString());
                            sw.WriteLine("Exception Message : " + ve.ErrorMessage);
                            sw.WriteLine("____________________________________________________");
                            sw.Close();
                        }
                    }
                }
                return null;
            }
            catch (DbUpdateException ex)
            {
                ErrorLog.LogError(ex, "OmmasIntegrationsDAL.SanctionPendingWorksDAL(DbUpdateException ex)");
                return null;
            }
            catch (OptimisticConcurrencyException ex)
            {
                ErrorLog.LogError(ex, "OmmasIntegrationsDAL.SanctionPendingWorksDAL(OptimisticConcurrencyException ex)");
                return null;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "OmmasIntegrationsDAL.SanctionPendingWorksDAL()");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public List<HabConnectivityStatusViewModel> HabConnectivityStatusDAL(int Month, int Year)
        {
            List<HabConnectivityStatusViewModel> lstModel = new List<HabConnectivityStatusViewModel>();
            dbContext = new PMGSYEntities();
            try
            {
                ((System.Data.Entity.Infrastructure.IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;

                //Scheme1 Overall Stats
                var pmgsy1Stats = dbContext.USP_HAB_COVERAGE_NEW_REPORT_MORD(Month, Year).ToList();

                if (pmgsy1Stats != null)
                {

                    foreach (var item in pmgsy1Stats)
                    {
                        HabConnectivityStatusViewModel model = new HabConnectivityStatusViewModel();

                        model.StateName = item.LOCATION_NAME;

                        model.NetEligible = Convert.ToInt32(item.UPOP1000) + Convert.ToInt32(item.UPOP999) + Convert.ToInt32(item.UPOPELJ499);
                        model.TotalCleared = Convert.ToInt32(item.CNPOP1000 - (model.StateName.Trim() == "Rajasthan" ? 6 : 0)) + Convert.ToInt32(item.CNPOP999) + Convert.ToInt32(item.CNELJ499);
                        model.TotalConnected = Convert.ToInt32(item.CNPOP1000_B) + Convert.ToInt32(item.CNPOP999_B) + Convert.ToInt32(item.CNELJ499_B) + Convert.ToInt32(item.SPOP1000) + Convert.ToInt32(item.SPOP999) + Convert.ToInt32(item.SPOPELJ499);
                        //model.TotalStateConnected = Convert.ToInt32(item.SPOP1000) + Convert.ToInt32(item.SPOP999) + Convert.ToInt32(item.SPOPELJ499);
                        model.LGD_State_Code = Convert.ToInt32(item.censusStateCode);
                        lstModel.Add(model);
                    }

                }
                return lstModel;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "OmmasIntegrationsDAL.OverallStatsDAL()");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public List<TargetAchievementViewModel> TargetAchievementMoRDDAL(int Year, int State)
        {

            TargetAchievementViewModel model = new TargetAchievementViewModel();

            List<TargetAchievementViewModel> list = new List<TargetAchievementViewModel>();
            dbContext = new PMGSYEntities();
            try
            {
                ((System.Data.Entity.Infrastructure.IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;

                //Scheme1 Overall Stats
                var result = dbContext.USP_TARGET_ACHIEVEMENT_MORD(Year, State).ToList();


                foreach (var item in result)
                {
                    model = new TargetAchievementViewModel();
                    //list = new List<TargetAchievementViewModel>();
                    model.Year = Year;
                    model.TargetLength = Convert.ToDecimal(item.LENGTH_TARGET);
                    model.CompletedLength = Convert.ToDecimal(item.LENGTH_COMPLETED);
                    model.TargetHabitations = Convert.ToInt32(item.HABITATION_TARGET);
                    model.CompletedHabitations = Convert.ToInt32(item.HABS_CONNECTED);
                    model.Expenditure = Convert.ToDecimal(item.EXPENDITURE);
                    model.LGD_STATE_CODE = Convert.ToInt32(item.MAST_NIC_STATE_CODE);
                    list.Add(model);
                }


                return list;

                //if (result != null)
                //{
                //    model.Year = Year;
                //    model.TargetLength = Convert.ToDecimal(result.LENGTH_TARGET);
                //    model.CompletedLength = Convert.ToDecimal(result.LENGTH_COMPLETED);
                //    model.TargetHabitations = Convert.ToInt32(result.HABITATION_TARGET);
                //    model.CompletedHabitations = Convert.ToInt32(result.HABS_CONNECTED);
                //    model.Expenditure = Convert.ToDecimal(result.EXPENDITURE);
                //    model.LED_STATE_CODE = Convert.ToInt32(result.MAST_NIC_STATE_CODE);

                //}
               
            }
            //TargetAchievementViewModel model = new TargetAchievementViewModel();

            //dbContext = new PMGSYEntities();
            //try
            //{
            //    ((System.Data.Entity.Infrastructure.IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
            //    dbContext.Configuration.LazyLoadingEnabled = false;

            //    //Scheme1 Overall Stats
            //    var result = dbContext.USP_TARGET_ACHIEVEMENT_MORD(Year, State).FirstOrDefault();

            //    if (result != null)
            //    {
            //        model.Year = Year;
            //        model.TargetLength = Convert.ToDecimal(result.LENGTH_TARGET);
            //        model.CompletedLength = Convert.ToDecimal(result.LENGTH_COMPLETED);
            //        model.TargetHabitations = Convert.ToInt32(result.HABITATION_TARGET);
            //        model.CompletedHabitations = Convert.ToInt32(result.HABS_CONNECTED);
            //        model.Expenditure = Convert.ToDecimal(result.EXPENDITURE);
            //    }
            //    return model;
            //}
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "OmmasIntegrationsDAL.TargetAchievementMoRDDAL()");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public List<PhysicalFinancialProgressViewModel> PhysicalFinancialProgressMoRDDAL()
        {
            PhysicalFinancialProgressViewModel model = new PhysicalFinancialProgressViewModel();
            List<PhysicalFinancialProgressViewModel> list = new List<PhysicalFinancialProgressViewModel>();

            dbContext = new PMGSYEntities();
            try
            {
                ((System.Data.Entity.Infrastructure.IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;

                //Scheme1 Overall Stats
                var result = dbContext.USP_MPR_ABSTRACT_REPORT_MORD().ToList();

                foreach (var item in result)
                {
                    model = new PhysicalFinancialProgressViewModel();
                    model.WorksSanctioned = Convert.ToInt32(item.NO_OF_ROADWORKS_SANCTIONED);
                    model.LengthSanctioned = Convert.ToDecimal(item.ROAD_LENGTH_SANCTIONED);
                    model.WorksCompleted = Convert.ToInt32(item.NO_OF_ROADWORKS_COMPLETED);
                    model.LengthCompleted = Convert.ToDecimal(item.ROADLENGTH_COMPLETED);
                    model.Expenditure = Convert.ToDecimal(item.EXPENDITURE);
                    model.StateCode = Convert.ToInt32(item.censusStateCode);


                    list.Add(model);
                }

                //if (result != null)
                //{
                //    //model.Year = Year;
                //    model.WorksSanctioned = Convert.ToInt32(result.NO_OF_ROADWORKS_SANCTIONED);
                //    model.LengthSanctioned = Convert.ToDecimal(result.ROAD_LENGTH_SANCTIONED);
                //    model.WorksCompleted = Convert.ToInt32(result.NO_OF_ROADWORKS_COMPLETED);
                //    model.LengthCompleted = Convert.ToDecimal(result.ROADLENGTH_COMPLETED);
                //    model.Expenditure = Convert.ToDecimal(result.EXPENDITURE);
                //    model.StateCode = Convert.ToInt32(result.censusStateCode);

                //}
                return list;
            }
            //PhysicalFinancialProgressViewModel model = new PhysicalFinancialProgressViewModel();

            //dbContext = new PMGSYEntities();
            //try
            //{
            //    ((System.Data.Entity.Infrastructure.IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
            //    dbContext.Configuration.LazyLoadingEnabled = false;

            //    //Scheme1 Overall Stats
            //    var result = dbContext.USP_MPR_ABSTRACT_REPORT_MORD().FirstOrDefault();

            //    if (result != null)
            //    {
            //        //model.Year = Year;
            //        model.WorksSanctioned = Convert.ToInt32(result.NO_OF_ROADWORKS_SANCTIONED);
            //        model.LengthSanctioned = Convert.ToDecimal(result.ROAD_LENGTH_SANCTIONED);
            //        model.WorksCompleted = Convert.ToInt32(result.NO_OF_ROADWORKS_COMPLETED);
            //        model.LengthCompleted = Convert.ToDecimal(result.ROADLENGTH_COMPLETED);
            //        model.Expenditure = Convert.ToDecimal(result.EXPENDITURE);
            //    }
            //    return model;
            //}
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "OmmasIntegrationsDAL.TargetAchievementMoRDDAL()");
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