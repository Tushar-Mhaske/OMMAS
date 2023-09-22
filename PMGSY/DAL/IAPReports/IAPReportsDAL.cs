using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.Models;
using System.Data.SqlClient;
using PMGSY.Models.IAPReports;
using System.Data.Entity.Infrastructure;
using PMGSY.Extensions;
using System.Data.Entity;

namespace PMGSY.DAL.IAPReports
{
    public class IAPReportsDAL : IIAPReportsDAL
    {
        PMGSY.Models.PMGSYEntities dbContext;
        public Array IAPDistrictHabitationDetailsDAL(int statecode, int month, int year, int page, int rows, string sidx, string sord, out int totalRecords)
        {

            //  List<USP_CN2_REPORT_Result> listIAPReport;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
               
                int State = statecode;
                int Month = month;
                int Year = year;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                //((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                //dbContext.Configuration.LazyLoadingEnabled = false;
                var listIAPHabReports = dbContext.USP_IAP_HAB_REPORT(State,month,year,PMGSY).ToList<USP_IAP_HAB_REPORT_Result>();

                totalRecords = listIAPHabReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listIAPHabReports = listIAPHabReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listIAPHabReports = listIAPHabReports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listIAPHabReports = listIAPHabReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listIAPHabReports.Select(x => new
                {
                  //  id = x.LOCATION_CODE,
                    cell = new[]{
                        x.LOCATION_NAME.ToString(),                       
                        x.MAST_DISTRICT_NAME == null ? "NULL" : x.MAST_DISTRICT_NAME.ToString(),                
                        x.TOTAL_EHABS.ToString(),
                        x.TOTAL_SHABS.ToString(),
                        x.TOTAL_CHABS.ToString(),
                        x.TOTAL_OHABS.ToString()  
                      
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array IAPDistrictPhysicalProgressDetailsDAL(int statecode, int month, int year, int page, int rows, string sidx, string sord, out int totalRecords)
        {

            //  List<USP_CN2_REPORT_Result> listIAPReport;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {

                int State = statecode;
                int Month = month;
                int Year = year;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                //((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                //dbContext.Configuration.LazyLoadingEnabled = false;
                var listIAPHabReports = dbContext.USP_IAP_PHY_REPORT(State, month, year, PMGSY).ToList<USP_IAP_PHY_REPORT_Result>();

                totalRecords = listIAPHabReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listIAPHabReports = listIAPHabReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listIAPHabReports = listIAPHabReports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listIAPHabReports = listIAPHabReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listIAPHabReports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[]{
                        x.LOCATION_NAME.ToString(),                       
                        x.MAST_DISTRICT_NAME == null ? "NULL" : x.MAST_DISTRICT_NAME.ToString(),                
                        x.TOTAL_SNEW.ToString(),
                        x.TOTAL_SNLEN.ToString(),
                        x.TOTAL_SUPGRADE.ToString(),
                        x.TOTAL_SUPLEN.ToString(),  
                        x.TOTAL_SANCTION.ToString(),
                        x.TOTAL_SLEN.ToString(),
                        x.TOTAL_CNEW.ToString(),
                        x.TOTAL_CNLEN.ToString(),
                        x.TOTAL_CUPGRADE.ToString(),
                        x.TOTAL_CUPLEN.ToString(),  
                        x.TOTAL_COMPLETE.ToString(),
                        x.TOTAL_CLEN.ToString()
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array IAPDistrictFinancialProgressDetailsDAL(int statecode, int month, int year, int page, int rows, string sidx, string sord, out int totalRecords)
        {

            //  List<USP_CN2_REPORT_Result> listIAPReport;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {

                int State = statecode;
                int Month = month;
                int Year = year;
                int PMGSY = PMGSYSession.Current.PMGSYScheme;
                //((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                //dbContext.Configuration.LazyLoadingEnabled = false;
                var listIAPHabReports = dbContext.USP_IAP_FIN_REPORT(State, month, year, PMGSYSession.Current.PMGSYScheme).ToList<USP_IAP_FIN_REPORT_Result>();

                totalRecords = listIAPHabReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listIAPHabReports = listIAPHabReports.OrderBy(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listIAPHabReports = listIAPHabReports.OrderByDescending(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listIAPHabReports = listIAPHabReports.OrderBy(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listIAPHabReports.Select(x => new
                {
                   id = x.MAST_STATE_CODE,
                    cell = new[]{
                        x.MAST_STATE_NAME.ToString(),                       
                        x.MAST_DISTRICT_NAME == null ? "NULL" : x.MAST_DISTRICT_NAME.ToString(),                
                        x.TOTAL_SNEW.ToString(),
                        x.TOTAL_SUPGRADE.ToString(),
                        x.TOTAL_SANCTION.ToString(),
                        x.TOTAL_ENEW.ToString(),  
                        x.TOTAL_EUPGRADE.ToString(),
                        x.TOTAL_EXP.ToString(), 
                        x.TOTAL_SANCTION==0?"0":((x.TOTAL_EXP/x.TOTAL_SANCTION)*100).ToString()                       
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array IAPDistrictExpenditureDetailsDAL(int statecode, int year, int page, int rows, string sidx, string sord, out int totalRecords)
        {

            //  List<USP_CN2_REPORT_Result> listIAPReport;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {

                int State = statecode;               
                int Year = year;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                //((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                //dbContext.Configuration.LazyLoadingEnabled = false;
                var listIAPHabReports = dbContext.USP_IAP_EXP_REPORT(State, year, PMGSY).ToList<USP_IAP_EXP_REPORT_Result>();

                totalRecords = listIAPHabReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listIAPHabReports = listIAPHabReports.OrderBy(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listIAPHabReports = listIAPHabReports.OrderByDescending(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listIAPHabReports = listIAPHabReports.OrderBy(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listIAPHabReports.Select(x => new
                {
                    id = x.MAST_STATE_CODE,
                    cell = new[]{
                        x.MAST_STATE_NAME.ToString(),                       
                        x.MAST_DISTRICT_NAME == null ? "NULL" : x.MAST_DISTRICT_NAME.ToString(),                
                        x.TOTAL_ENEW.ToString(),
                        x.TOTAL_EUPGRADE.ToString(),
                        x.TOTAL_EXP.ToString()             
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

    }
    public interface IIAPReportsDAL
    {

        Array IAPDistrictHabitationDetailsDAL(int statecode,int month,int year,int page, int rows, string sidx, string sord, out int totalRecords);
        Array IAPDistrictPhysicalProgressDetailsDAL(int statecode, int month, int year, int page, int rows, string sidx, string sord, out int totalRecords);
        Array IAPDistrictFinancialProgressDetailsDAL(int statecode, int month, int year, int page, int rows, string sidx, string sord, out int totalRecords);
        Array IAPDistrictExpenditureDetailsDAL(int statecode, int year, int page, int rows, string sidx, string sord, out int totalRecords);
  
    
    }
}