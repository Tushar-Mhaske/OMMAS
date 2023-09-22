using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.Models;
using System.Data.SqlClient;
using PMGSY.Models.CoreNetworkReports;
using System.Data.Entity.Infrastructure;
using PMGSY.Extensions;
using System.Data.Entity;
using System.Web.Mvc;

namespace PMGSY.DAL.CoreNetworkReports
{
    public class CoreNetworkReportsDAL : ICoreNetworkReportsDAL
    {
        PMGSY.Models.PMGSYEntities dbContext;
        public Array CN1ReportListingDAL(string route, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            //List<PMGSY.Models.USP_CN1_REPORT_Result> listCN1Reports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 1;
                int State = 0;
                int District = 0;
                int Block = 0;
                string Route = route;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 720;
                dbContext.Configuration.LazyLoadingEnabled = false;

                var listCN1Reports = dbContext.USP_CN1_REPORT(Level, State, District, Block, route, PMGSYSession.Current.PMGSYScheme).ToList<USP_CN1_REPORT_Result>();

                totalRecords = listCN1Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listCN1Reports = listCN1Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listCN1Reports = listCN1Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listCN1Reports = listCN1Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listCN1Reports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[]{
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='CN1DistrictReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+Route.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                        x.TOTAL_CN.ToString(),
                        x.TOTAL_LEN.ToString(),
                        x.BT_LEN.ToString(),
                        x.WBM_LEN.ToString(),
                        x.GRAVEL_LEN.ToString(),
                        x.TRACK_LEN.ToString(),
                        x.OTHER_LEN.ToString()
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


        public Array CN1DistrictReportListingDAL(string route, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 2;
                int State = stateCode;
                int District = 0;
                int Block = 0;
                string Route = route;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 720;
                dbContext.Configuration.LazyLoadingEnabled = false;

                var listCN1Reports = dbContext.USP_CN1_REPORT(Level, State, District, Block, route, PMGSYSession.Current.PMGSYScheme).ToList<USP_CN1_REPORT_Result>();

                totalRecords = listCN1Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listCN1Reports = listCN1Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listCN1Reports = listCN1Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listCN1Reports = listCN1Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listCN1Reports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[] {
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='CN1BlockReportListing(\"" + stateCode.ToString() + "\",\"" + x.LOCATION_CODE.ToString().Trim() +"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+Route.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                        x.TOTAL_CN.ToString(),
                        x.TOTAL_LEN.ToString(),
                        x.BT_LEN.ToString(),
                        x.WBM_LEN.ToString(),
                        x.GRAVEL_LEN.ToString(),
                        x.TRACK_LEN.ToString(),
                        x.OTHER_LEN.ToString()               
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


        public Array CN1BlockReportListingDAL(string route, int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 3;
                int State = stateCode;
                int District = districtCode;
                int Block = 0;
                string Route = route;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 720;
                dbContext.Configuration.LazyLoadingEnabled = false;

                var listCN1Reports = dbContext.USP_CN1_REPORT(Level, State, District, Block, Route, PMGSYSession.Current.PMGSYScheme).ToList<USP_CN1_REPORT_Result>();

                totalRecords = listCN1Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listCN1Reports = listCN1Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listCN1Reports = listCN1Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listCN1Reports = listCN1Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listCN1Reports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[] {
                      "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='CN1FinalReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+districtCode.ToString()+"\",\""+stateCode.ToString()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+Route.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                     // x.LOCATION_NAME.ToString(),
                        x.TOTAL_CN.ToString(),
                        x.TOTAL_LEN.ToString(),
                        x.BT_LEN.ToString(),
                        x.WBM_LEN.ToString(),
                        x.GRAVEL_LEN.ToString(),
                        x.TRACK_LEN.ToString(),
                        x.OTHER_LEN.ToString()                        
                       
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


        public Array CN1FinalBlockReportListingDAL(int pop, int road, string route, int blockCode, int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            // List<USP_CN1_FINAL_REPORT_Result> listCN1Reports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 1;
                int State = stateCode;
                int District = districtCode;
                int Block = blockCode;
                int Pop = pop;
                int Road = road;
                string Route = route;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                var listCN1Reports = dbContext.USP_CN_REPORT(Level, State, District, Block, Pop, Road, Route, PMGSY).ToList<USP_CN_REPORT_Result>();
                //listCN1Reports = dbContext.Database.SqlQuery<USP_CN1_FINAL_REPORT_Result>("EXEC [omms].[USP_CN1_REPORT] @Level,@State,@District,@Block,@Route",
                //    new SqlParameter("@Level", Level),
                //    new SqlParameter("@State", State),
                //    new SqlParameter("@District", District),
                //    new SqlParameter("@Block", Block),
                //    new SqlParameter("@Route", '%')
                //    ).ToList<USP_CN1_FINAL_REPORT_Result>();

                totalRecords = listCN1Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listCN1Reports = listCN1Reports.OrderBy(x => x.PLAN_UNIQ_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listCN1Reports = listCN1Reports.OrderByDescending(x => x.PLAN_UNIQ_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listCN1Reports = listCN1Reports.OrderBy(x => x.PLAN_UNIQ_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listCN1Reports.Select(x => new
                {
                    id = x.PLAN_CN_ROAD_NUMBER,
                    cell = new[] {  
                        
                        x.PLAN_UNIQ_ID==null?"NA":x.PLAN_UNIQ_ID.ToString(),
                        x.PLAN_CN_ROAD_NUMBER == null ? "NA" : x.PLAN_CN_ROAD_NUMBER.ToString(),                       
                        x.RD_FROM == null ? "NA" : x.RD_FROM.ToString(),
                        x.RD_TO == null ? "NA" : x.RD_TO.ToString(),                        
                        x.PLAN_RD_LENGTH == null ? "0" : x.PLAN_RD_LENGTH.ToString(),                        
                        x.BT_LEN.ToString(),
                        x.BT_CONDITION == null ? "-": x.BT_CONDITION=="G"?"Good":x.BT_CONDITION=="B"?"Bad":x.BT_CONDITION=="F"?"Fair":x.BT_CONDITION,
                        x.WBM_LEN.ToString(),
                        x.WBM_CONDITION == null ? "-": x.WBM_CONDITION=="G"?"Good":x.WBM_CONDITION=="B"?"Bad":x.WBM_CONDITION=="F"?"Fair":x.WBM_CONDITION,                    
                        x.GRAVEL_LEN.ToString(),
                        x.GRAVEL_CONDITION == null ? "-": x.GRAVEL_CONDITION=="G"?"Good":x.GRAVEL_CONDITION=="B"?"Bad":x.GRAVEL_CONDITION=="F"?"Fair":x.GRAVEL_CONDITION,
                        x.TRACK_LEN.ToString(), 
                        x.TARCK_CONDITION == null ? "-": x.TARCK_CONDITION=="G"?"Good":x.TARCK_CONDITION=="B"?"Bad":x.TARCK_CONDITION=="F"?"Fair":x.TARCK_CONDITION,
                        x.MAST_HAB_TOT_POP.ToString(), //TOTAL_POPULATION_SERVED
                        x.MAST_HAB_NAME == null ? "NA" : x.MAST_HAB_NAME.ToString(),                       
                        x.MAST_HAB_TOT_POP.ToString(),
                        x.MAST_HAB_CONNECTED== null? "NA" : x.MAST_HAB_CONNECTED.ToString(),                       
                        x.KML_FILE == null ? "0" : x.KML_FILE.ToString()
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


        public Array CN2StateReportListingDAL(int pop, int page, int rows, string sidx, string sord, out int totalRecords)
        {

            //List<USP_CN2_REPORT_Result> listCN2Reports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 1;
                int State = 0;
                int District = 0;
                int Block = 0;
                int Population = pop;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;
                //listCN2Reports = dbContext.Database.SqlQuery<USP_CN2_REPORT_Result>("EXEC [omms].[USP_CN2_REPORT] @LEVEL,@STATE,@DISTRICT,@BLOCK,@POP,@PMGSY",
                //    new SqlParameter("@LEVEL", Level),
                //    new SqlParameter("@STATE", State),
                //    new SqlParameter("@DISTRICT", District),
                //    new SqlParameter("@BLOCK", Block),
                //    new SqlParameter("@POP", Population),
                //    new SqlParameter("@PMGSY", PMGSY)
                //    ).ToList<USP_CN2_REPORT_Result>();
                var listCN2Reports = dbContext.USP_CN2_REPORT(Level, State, District, Block, Population, PMGSY).ToList<USP_CN2_REPORT_Result>();

                totalRecords = listCN2Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listCN2Reports = listCN2Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listCN2Reports = listCN2Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listCN2Reports = listCN2Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listCN2Reports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[]{
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='CN2DistrictReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+Population.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",                       
                        x.TPOP1000.ToString(),
                        x.TPOP999.ToString(),
                        x.TPOP499.ToString(),
                        x.TPOP250.ToString(),                      
                        x.CPOP1000.ToString(),
                        x.CPOP999.ToString(),
                        x.CPOP499.ToString(),
                        x.CPOP250.ToString(),
                        x.UPOP1000.ToString(),
                        x.UPOP999.ToString(),
                        x.UPOP499.ToString(),
                        x.UPOP250.ToString()
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


        public Array CN2DistrictReportListingDAL(int pop, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            // List<USP_CN2_REPORT_Result> listCN2Reports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 2;
                int State = stateCode;
                int District = 0;
                int Block = 0;
                int Population = pop;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;

                //listCN2Reports = dbContext.Database.SqlQuery<USP_CN2_REPORT_Result>("EXEC [omms].[USP_CN2_REPORT] @LEVEL,@STATE,@DISTRICT,@BLOCK,@POP,@PMGSY",
                // new SqlParameter("@LEVEL", Level),
                // new SqlParameter("@STATE", State),
                // new SqlParameter("@DISTRICT", District),
                // new SqlParameter("@BLOCK", Block),
                // new SqlParameter("@POP", Population),
                // new SqlParameter("@PMGSY", PMGSY)
                // ).ToList<USP_CN2_REPORT_Result>();
                var listCN2Reports = dbContext.USP_CN2_REPORT(Level, State, District, Block, Population, PMGSY).ToList<USP_CN2_REPORT_Result>();


                totalRecords = listCN2Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listCN2Reports = listCN2Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listCN2Reports = listCN2Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listCN2Reports = listCN2Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listCN2Reports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[] {
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='CN2BlockReportListing(\""+stateCode.ToString().Trim()+"\",\""+x.LOCATION_CODE.ToString()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+Population.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                        x.TPOP1000.ToString(),
                        x.TPOP999.ToString(),
                        x.TPOP499.ToString(),
                        x.TPOP250.ToString(),                      
                        x.CPOP1000.ToString(),
                        x.CPOP999.ToString(),
                        x.CPOP499.ToString(),
                        x.CPOP250.ToString(),
                        x.UPOP1000.ToString(),
                        x.UPOP999.ToString(),
                        x.UPOP499.ToString(),
                        x.UPOP250.ToString()
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


        public Array CN2BlockReportListingDAL(int pop, int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            //List<USP_CN2_REPORT_Result> listCN2Reports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 3;
                int State = stateCode;
                int District = districtCode;
                int Block = 0;
                int Population = pop;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 720;
                dbContext.Configuration.LazyLoadingEnabled = false;
                //listCN2Reports = dbContext.Database.SqlQuery<USP_CN2_REPORT_Result>("EXEC [omms].[USP_CN2_REPORT] @LEVEL,@STATE,@DISTRICT,@BLOCK,@POP,@PMGSY",
                //  new SqlParameter("@LEVEL", Level),
                //  new SqlParameter("@STATE", State),
                //  new SqlParameter("@DISTRICT", District),
                //  new SqlParameter("@BLOCK", Block),
                //  new SqlParameter("@POP", Population),
                //  new SqlParameter("@PMGSY", PMGSY)
                //  ).ToList<USP_CN2_REPORT_Result>();

                var listCN2Reports = dbContext.USP_CN2_REPORT(Level, State, District, Block, Population, PMGSY).ToList<USP_CN2_REPORT_Result>();

                totalRecords = listCN2Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listCN2Reports = listCN2Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listCN2Reports = listCN2Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listCN2Reports = listCN2Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listCN2Reports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[] {
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='CN2FinalReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+districtCode.ToString()+"\",\""+stateCode.ToString()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+Population.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                        //x.LOCATION_NAME.ToString(),
                        x.TPOP1000.ToString(),
                        x.TPOP999.ToString(),
                        x.TPOP499.ToString(),
                        x.TPOP250.ToString(),                      
                        x.CPOP1000.ToString(),
                        x.CPOP999.ToString(),
                        x.CPOP499.ToString(),
                        x.CPOP250.ToString(),
                        x.UPOP1000.ToString(),
                        x.UPOP999.ToString(),
                        x.UPOP499.ToString(),
                        x.UPOP250.ToString()
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


        public Array CN2FinalListingDAL(int pop, int road, string route, int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            // List<USP_CN2_FINAL_REPORT_Result> listCN2Reports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 1;
                int State = stateCode;
                int District = districtCode;
                int Block = blockCode;
                int Pop = pop;
                int Road = road;
                string Route = route;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;

                var listCN2Reports = dbContext.USP_CN_REPORT(Level, State, District, Block, Pop, Road, Route, PMGSY).ToList<USP_CN_REPORT_Result>();

                // listCN2Reports = dbContext.Database.SqlQuery<USP_CN2_FINAL_REPORT_Result>("EXEC [omms].[USP_CN2_REPORT] @Level,@State,@District,@Block,@Population",
                //new SqlParameter("@Level", Level),
                //new SqlParameter("@State", State),
                //new SqlParameter("@District", District),
                //new SqlParameter("@Block", Block),
                //new SqlParameter("@Population", Population)
                //).ToList<USP_CN2_FINAL_REPORT_Result>();

                totalRecords = listCN2Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listCN2Reports = listCN2Reports.OrderBy(x => x.PLAN_RD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listCN2Reports = listCN2Reports.OrderByDescending(x => x.PLAN_RD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listCN2Reports = listCN2Reports.OrderBy(x => x.PLAN_RD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listCN2Reports.Select(x => new
                {
                    id = x.PLAN_CN_ROAD_NUMBER,
                    cell = new[] {
                       x.PLAN_CN_ROAD_NUMBER == null ? "NA" : x.PLAN_CN_ROAD_NUMBER.ToString() ,
                       x.PLAN_RD_NAME == null ? "NA" : x.PLAN_RD_NAME.ToString(),
                       x.MAST_ER_ROAD_NUMBER == null ? "NA" : x.MAST_ER_ROAD_NUMBER.ToString(),
                       x.MAST_ER_ROAD_NAME == null ? "NA" : x.MAST_ER_ROAD_NAME.ToString(),
                       x.PLAN_RD_LENGTH == null ? "0" : x.PLAN_RD_LENGTH.ToString(),
                       x.RD_FROM == null ? "NA" : x.RD_FROM.ToString(),
                       x.RD_TO == null ? "NA" :  x.RD_TO.ToString(),
                       x.PLAN_RD_ROUTE == null ? "NA" :  x.PLAN_RD_ROUTE.ToString(),
                       x.PLAN_RD_LENG == null ? "0" : x.PLAN_RD_LENG.ToString(),
                       x.MAST_HAB_NAME == null ? "NA" : x.MAST_HAB_NAME.ToString(),
                        x.MAST_HAB_TOT_POP.ToString() ,
                       x.MAST_HAB_CONNECTED== null? "NA" : x.MAST_HAB_CONNECTED.ToString()
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


        public Array CN3StateReportListingDAL(int roadcategory, string route, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            // List<USP_CN3_REPORT_Result> listCN3Reports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 1;
                int State = 0;
                int District = 0;
                int Block = 0;
                int RoadCat = roadcategory;
                string Route = route;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 1800;
                dbContext.Configuration.LazyLoadingEnabled = false;

                var listCN3Reports = dbContext.USP_CN3_REPORT(Level, State, District, Block, RoadCat, Route, PMGSY).ToList<USP_CN3_REPORT_Result>();

                //var listCN3Reports = dbContext.Database.SqlQuery<USP_CN3_REPORT_Result>("EXEC [omms].[USP_CN3_REPORT] @Level,@State,@District,@Block,@RoadCat,@Route,@PMGSY",
                //    new SqlParameter("@Level", Level),
                //    new SqlParameter("@State", State),
                //    new SqlParameter("@District", District),
                //    new SqlParameter("@Block", Block),
                //    new SqlParameter("@RoadCat", RoadCat),
                //    new SqlParameter("@Route", Route),
                //    new SqlParameter("@PMGSY", PMGSY)
                //    ).ToList<USP_CN3_REPORT_Result>();

                totalRecords = listCN3Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listCN3Reports = listCN3Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listCN3Reports = listCN3Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listCN3Reports = listCN3Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listCN3Reports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[]{
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='CN3DistrictReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+Route.ToString().Trim()+"\",\""+RoadCat.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",                       
                         x.BT_LEN.ToString(),
                         x.WBM_LEN.ToString(),
                         x.GRAVEL_LEN.ToString(),
                         x.TRACK_LEN.ToString(),
                         x.OTHER_LEN.ToString(),
                         (x.BT_LEN+x.WBM_LEN+x.GRAVEL_LEN+x.TRACK_LEN+x.OTHER_LEN).ToString(),                
                         x.CN_BT_LEN.ToString(),
                         x.CN_WBM_LEN.ToString(),
                         x.CN_GRAVEL_LEN.ToString(),
                         x.CN_TRACK_LEN.ToString(),
                         x.CN_OTHER_LEN.ToString(),
                        (x.CN_BT_LEN+x.CN_WBM_LEN+x.CN_GRAVEL_LEN+x.CN_TRACK_LEN+x.CN_OTHER_LEN).ToString()                        
                     
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


        public Array CN3DistrictReportListingDAL(int roadcategory, string route, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            //List<USP_CN3_REPORT_Result> listCN3Reports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 2;
                int State = stateCode;
                int District = 0;
                int Block = 0;
                int RoadCat = roadcategory;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                string Route = route;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 1800;
                dbContext.Configuration.LazyLoadingEnabled = false;
                //listCN3Reports = dbContext.Database.SqlQuery<USP_CN3_REPORT_Result>("EXEC [omms].[USP_CN3_REPORT] @Level,@State,@District,@Block,@RoadCat,@Route,@PMGSY",
                //    new SqlParameter("@Level", Level),
                //    new SqlParameter("@State", State),
                //    new SqlParameter("@District", District),
                //    new SqlParameter("@Block", Block),
                //    new SqlParameter("@RoadCat", RoadCat),
                //    new SqlParameter("@Route", Route),
                //    new SqlParameter("@PMGSY", PMGSY)
                //    ).ToList<USP_CN3_REPORT_Result>();
                var listCN3Reports = dbContext.USP_CN3_REPORT(Level, State, District, Block, RoadCat, Route, PMGSY).ToList<USP_CN3_REPORT_Result>();


                totalRecords = listCN3Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listCN3Reports = listCN3Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listCN3Reports = listCN3Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listCN3Reports = listCN3Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listCN3Reports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[]{
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='CN3BlockReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+stateCode+"\",\""+x.LOCATION_NAME+"\",\""+Route.ToString().Trim()+"\",\""+RoadCat.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",                  
                        x.BT_LEN.ToString(),
                         x.WBM_LEN.ToString(),
                         x.GRAVEL_LEN.ToString(),
                         x.TRACK_LEN.ToString(),
                         x.OTHER_LEN.ToString(),
                         (x.BT_LEN+x.WBM_LEN+x.GRAVEL_LEN+x.TRACK_LEN+x.OTHER_LEN).ToString(),                
                         x.CN_BT_LEN.ToString(),
                         x.CN_WBM_LEN.ToString(),
                         x.CN_GRAVEL_LEN.ToString(),
                         x.CN_TRACK_LEN.ToString(),
                         x.CN_OTHER_LEN.ToString(),
                        (x.CN_BT_LEN+x.CN_WBM_LEN+x.CN_GRAVEL_LEN+x.CN_TRACK_LEN+x.CN_OTHER_LEN).ToString()                        
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


        public Array CN3BlockReportListingDAL(int roadcategory, string route, int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            // List<USP_CN3_REPORT_Result> listCN3Reports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 3;
                int State = stateCode;
                int District = districtCode;
                int Block = 0;
                int RoadCat = roadcategory;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                string Route = route;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 720;
                dbContext.Configuration.LazyLoadingEnabled = false;
                //listCN3Reports = dbContext.Database.SqlQuery<USP_CN3_REPORT_Result>("EXEC [omms].[USP_CN3_REPORT] @Level,@State,@District,@Block,@RoadCat,@Route,@PMGSY",
                //    new SqlParameter("@Level", Level),
                //    new SqlParameter("@State", State),
                //    new SqlParameter("@District", District),
                //    new SqlParameter("@Block", Block),
                //    new SqlParameter("@RoadCat", RoadCat),
                //    new SqlParameter("@Route", Route),
                //    new SqlParameter("@PMGSY", PMGSY)
                //    ).ToList<USP_CN3_REPORT_Result>();
                var listCN3Reports = dbContext.USP_CN3_REPORT(Level, State, District, Block, RoadCat, Route, PMGSY).ToList<USP_CN3_REPORT_Result>();


                totalRecords = listCN3Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listCN3Reports = listCN3Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listCN3Reports = listCN3Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listCN3Reports = listCN3Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listCN3Reports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[]{
                       "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='CN3FinalReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+districtCode.ToString()+"\",\""+stateCode.ToString()+"\",\""+x.LOCATION_NAME.ToString()+"\",\""+Route.ToString().Trim()+"\",\""+RoadCat.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",                       
                       // x.LOCATION_NAME==null?0.ToString():x.LOCATION_NAME.ToString(),
                       x.BT_LEN.ToString(),
                         x.WBM_LEN.ToString(),
                         x.GRAVEL_LEN.ToString(),
                         x.TRACK_LEN.ToString(),
                         x.OTHER_LEN.ToString(),
                         (x.BT_LEN+x.WBM_LEN+x.GRAVEL_LEN+x.TRACK_LEN+x.OTHER_LEN).ToString(),                
                         x.CN_BT_LEN.ToString(),
                         x.CN_WBM_LEN.ToString(),
                         x.CN_GRAVEL_LEN.ToString(),
                         x.CN_TRACK_LEN.ToString(),
                         x.CN_OTHER_LEN.ToString(),
                        (x.CN_BT_LEN+x.CN_WBM_LEN+x.CN_GRAVEL_LEN+x.CN_TRACK_LEN+x.CN_OTHER_LEN).ToString()                        
                             
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


        public Array CN3FinalListingDAL(int pop, int roadcategory, string route, int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            //List<USP_CN3_FINAL_REPORT_Result> listCN3Reports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 2;
                int State = stateCode;
                int District = districtCode;
                int Block = blockCode;
                int Pop = pop;
                int Road = roadcategory;
                string Route = route;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;

                var listCN3Reports = dbContext.USP_CN_REPORT(Level, State, District, Block, Pop, Road, Route, PMGSY).ToList<USP_CN_REPORT_Result>();

                //listCN3Reports = dbContext.Database.SqlQuery<USP_CN3_FINAL_REPORT_Result>("EXEC [omms].[USP_CN3_REPORT] @Level,@State,@District,@Block,@RoadCat,@Route",
                //    new SqlParameter("@Level", Level),
                //    new SqlParameter("@State", State),
                //    new SqlParameter("@District", District),
                //    new SqlParameter("@Block", Block),
                //    new SqlParameter("@RoadCat", RoadCat),
                //    new SqlParameter("@Route", Route)
                //    ).ToList<USP_CN3_FINAL_REPORT_Result>();

                totalRecords = listCN3Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listCN3Reports = listCN3Reports.OrderBy(x => x.PLAN_RD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listCN3Reports = listCN3Reports.OrderByDescending(x => x.PLAN_RD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listCN3Reports = listCN3Reports.OrderBy(x => x.PLAN_RD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listCN3Reports.Select(x => new
                {
                    id = x.PLAN_CN_ROAD_NUMBER,
                    cell = new[]{
                        x.PLAN_CN_ROAD_NUMBER.ToString(),
                        x.PLAN_RD_NAME == null ? "NA" : x.PLAN_RD_NAME.ToString(),
                        x.RD_FROM == null ? "NA" : x.RD_FROM.ToString(),
                        x.RD_TO == null ? "NA" : x.RD_TO.ToString(),
                        x.PLAN_RD_LENGTH.ToString(),
                        x.TOTAL_HABS == null ? 0.ToString() : x.TOTAL_HABS.ToString(),  
                        x.MAST_HAB_TOT_POP.ToString()                     
                    }
                }).ToArray();
            }
            catch
            {
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        public Array CN4StateReportListingDAL(int roadcategory, string route, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            // List<USP_CN4_REPORT_Result> listCN4Reports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 1;
                int State = 0;
                int District = 0;
                int Block = 0;
                int RoadCategory = roadcategory;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                string Route = route;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 1800;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var listCN4Reports = dbContext.USP_CN4_REPORT(Level, State, District, Block, RoadCategory, Route, PMGSY).ToList<USP_CN4_REPORT_Result>();

                //listCN4Reports = dbContext.Database.SqlQuery<USP_CN4_REPORT_Result>("EXEC [omms].[USP_CN4_REPORT] @Level,@State,@District,@Block,@RoadCat,@Route,@PMGSY",
                //    new SqlParameter("@Level", Level),
                //    new SqlParameter("@State", State),
                //    new SqlParameter("@District", District),
                //    new SqlParameter("@Block", Block),
                //    new SqlParameter("@RoadCat", RoadCategory),
                //    new SqlParameter("@Route", Route),
                //    new SqlParameter("@PMGSY", PMGSY)
                //    ).ToList<USP_CN4_REPORT_Result>();

                totalRecords = listCN4Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listCN4Reports = listCN4Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listCN4Reports = listCN4Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listCN4Reports = listCN4Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listCN4Reports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[]{
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='CN4DistrictReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+Route.ToString().Trim()+"\",\""+RoadCategory.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",                       
                       x.TOTAL_CN == null ? 0.ToString() : x.TOTAL_CN.ToString(),
                        x.TOTAL_LEN.ToString(),
                        x.BT_LEN.ToString(),
                        x.WBM_LEN.ToString(),
                        x.GRAVEL_LEN.ToString(),
                        x.TRACK_LEN.ToString(),
                        "0",
                        x.NH_BT_LEN.ToString(),
                        x.NH_WBM_LEN.ToString(),
                         "0",
                        x.SH_BT_LEN.ToString(),
                        x.SH_WBM_LEN.ToString(),
                         "0",
                        x.MDR_BT_LEN.ToString(),
                        x.MDR_WBM_LEN.ToString(),
                        x.MDR_GRAVEL_LEN.ToString(),
                        x.MDR_TRACK_LEN.ToString(),
                         "0",
                        x.LR_BT_LEN.ToString(),
                        x.LR_WBM_LEN.ToString(),
                        x.LR_GRAVEL_LEN.ToString(),
                        x.LR_TRACK_LEN.ToString(),
                         "0",
                        x.TR_BT_LEN.ToString(),
                        x.TR_WBM_LEN.ToString(),
                        x.TR_GRAVEL_LEN.ToString(),
                        x.LR_TRACK_LEN.ToString(),
                         "0"
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


        public Array CN4DistrictReportListingDAL(int roadcategory, string route, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            // List<USP_CN4_REPORT_Result> listCN4Reports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 2;
                int State = stateCode;
                int District = 0;
                int Block = 0;
                int RoadCat = roadcategory;
                //  char Route ="%";
                string Route = route;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 1800;
                dbContext.Configuration.LazyLoadingEnabled = false;
                //listCN4Reports = dbContext.Database.SqlQuery<USP_CN4_REPORT_Result>("EXEC [omms].[USP_CN4_REPORT] @Level,@State,@District,@Block,@RoadCat,@Route,@PMGSY",
                //    new SqlParameter("@Level", Level),
                //    new SqlParameter("@State", State),
                //    new SqlParameter("@District", District),
                //    new SqlParameter("@Block", Block),
                //    new SqlParameter("@RoadCat", RoadCat),
                //    new SqlParameter("@Route", Route),
                //    new SqlParameter("@PMGSY", PMGSY)
                //    ).ToList<USP_CN4_REPORT_Result>();
                var listCN4Reports = dbContext.USP_CN4_REPORT(Level, State, District, Block, RoadCat, Route, PMGSY).ToList<USP_CN4_REPORT_Result>();


                totalRecords = listCN4Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listCN4Reports = listCN4Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listCN4Reports = listCN4Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listCN4Reports = listCN4Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listCN4Reports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[]{
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='CN4BlockReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+stateCode+"\",\""+x.LOCATION_NAME+"\",\""+Route.ToString().Trim()+"\",\""+RoadCat.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                       x.TOTAL_CN == null ? 0.ToString() : x.TOTAL_CN.ToString(),
                        x.TOTAL_LEN.ToString(),
                        x.BT_LEN.ToString(),
                        x.WBM_LEN.ToString(),
                        x.GRAVEL_LEN.ToString(),
                        x.TRACK_LEN.ToString(),
                        "0",
                        x.NH_BT_LEN.ToString(),
                        x.NH_WBM_LEN.ToString(),
                         "0",
                        x.SH_BT_LEN.ToString(),
                        x.SH_WBM_LEN.ToString(),
                         "0",
                        x.MDR_BT_LEN.ToString(),
                        x.MDR_WBM_LEN.ToString(),
                        x.MDR_GRAVEL_LEN.ToString(),
                        x.MDR_TRACK_LEN.ToString(),
                         "0",
                        x.LR_BT_LEN.ToString(),
                        x.LR_WBM_LEN.ToString(),
                        x.LR_GRAVEL_LEN.ToString(),
                        x.LR_TRACK_LEN.ToString(),
                         "0",
                        x.TR_BT_LEN.ToString(),
                        x.TR_WBM_LEN.ToString(),
                        x.TR_GRAVEL_LEN.ToString(),
                        x.LR_TRACK_LEN.ToString(),
                         "0"
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


        public Array CN4BlockReportListingDAL(int roadcategory, string route, int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            // List<USP_CN4_REPORT_Result> listCN4Reports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 3;
                int State = stateCode;
                int District = districtCode;
                int Block = 0;
                int RoadCat = roadcategory;
                string Route = route;
                //char Route = '%';

                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;
                //listCN4Reports = dbContext.Database.SqlQuery<USP_CN4_REPORT_Result>("EXEC [omms].[USP_CN4_REPORT] @Level,@State,@District,@Block,@RoadCat,@Route,@PMGSY",
                //    new SqlParameter("@Level", Level),
                //    new SqlParameter("@State", State),
                //    new SqlParameter("@District", District),
                //    new SqlParameter("@Block", Block),
                //    new SqlParameter("@RoadCat", RoadCat),
                //    new SqlParameter("@Route", Route),
                //    new SqlParameter("@PMGSY", PMGSY)
                //    ).ToList<USP_CN4_REPORT_Result>();
                var listCN4Reports = dbContext.USP_CN4_REPORT(Level, State, District, Block, RoadCat, Route, PMGSY).ToList<USP_CN4_REPORT_Result>();

                totalRecords = listCN4Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listCN4Reports = listCN4Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listCN4Reports = listCN4Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listCN4Reports = listCN4Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listCN4Reports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[]{
                       "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='CN4FinalReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+districtCode.ToString()+"\",\""+stateCode.ToString()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+Route.ToString().Trim()+"\",\""+RoadCat.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                        x.TOTAL_CN == null ? 0.ToString() : x.TOTAL_CN.ToString(),
                        x.TOTAL_LEN.ToString(),
                        x.BT_LEN.ToString(),
                        x.WBM_LEN.ToString(),
                        x.GRAVEL_LEN.ToString(),
                        x.TRACK_LEN.ToString(),
                        "0",
                        x.NH_BT_LEN.ToString(),
                        x.NH_WBM_LEN.ToString(),
                         "0",
                        x.SH_BT_LEN.ToString(),
                        x.SH_WBM_LEN.ToString(),
                         "0",
                        x.MDR_BT_LEN.ToString(),
                        x.MDR_WBM_LEN.ToString(),
                        x.MDR_GRAVEL_LEN.ToString(),
                        x.MDR_TRACK_LEN.ToString(),
                         "0",
                        x.LR_BT_LEN.ToString(),
                        x.LR_WBM_LEN.ToString(),
                        x.LR_GRAVEL_LEN.ToString(),
                        x.LR_TRACK_LEN.ToString(),
                         "0",
                        x.TR_BT_LEN.ToString(),
                        x.TR_WBM_LEN.ToString(),
                        x.TR_GRAVEL_LEN.ToString(),
                        x.LR_TRACK_LEN.ToString(),
                         "0"
                    }
                }).ToArray();
            }
            catch
            {
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        public Array CN4FinalListingDAL(int pop, int roadcategory, string route, int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            //List<USP_CN_ROADWISE_REPORT_Result> listCN4Reports;

            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {

                int Level = 2;
                int State = stateCode;
                int District = districtCode;
                int Block = blockCode;
                int Pop = pop;
                int Road = roadcategory;
                string Route = route;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 180;
                var listCN4Reports = dbContext.USP_CN_REPORT(Level, State, District, Block, Pop, Road, Route, PMGSY).ToList<USP_CN_REPORT_Result>();

                //listCN4Reports = dbContext.Database.SqlQuery<USP_CN_ROADWISE_REPORT_Result>("EXEC [omms].[USP_CN_REPORT] @LEVEL,@STATE,@DISTRICT,@BLOCK,@POP,@ROADCAT,@ROUTE,@PMGSY",
                //    new SqlParameter("@LEVEL", Level),
                //    new SqlParameter("@STATE", State),
                //    new SqlParameter("@DISTRICT", District),
                //    new SqlParameter("@BLOCK", Block),
                //     new SqlParameter("@POP", Pop),
                //    new SqlParameter("@ROADCAT", Road),
                //    new SqlParameter("@ROUTE", Route),
                //    new SqlParameter("@PMGSY", PMGSY)
                //).ToList<USP_CN_ROADWISE_REPORT_Result>();


                totalRecords = listCN4Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listCN4Reports = listCN4Reports.OrderBy(x => x.PLAN_RD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listCN4Reports = listCN4Reports.OrderByDescending(x => x.PLAN_RD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listCN4Reports = listCN4Reports.OrderBy(x => x.PLAN_RD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listCN4Reports.Select(x => new
                {
                    id = x.PLAN_CN_ROAD_NUMBER,
                    cell = new[]{
                        x.PLAN_CN_ROAD_NUMBER == null ? "NA" : x.PLAN_CN_ROAD_NUMBER.ToString() ,
                       x.PLAN_RD_NAME == null ? "NA" : x.PLAN_RD_NAME.ToString(),
                       x.MAST_ER_ROAD_NUMBER == null ? "NA" : x.MAST_ER_ROAD_NUMBER.ToString(),
                       x.MAST_ER_ROAD_NAME == null ? "NA" : x.MAST_ER_ROAD_NAME.ToString(),
                       x.PLAN_RD_LENGTH == null ? "0" : x.PLAN_RD_LENGTH.ToString(),
                       x.RD_FROM == null ? "NA" : x.RD_FROM.ToString(),
                       x.RD_TO == null ? "NA" :  x.RD_TO.ToString(),
                       x.PLAN_RD_ROUTE == null ? "NA" :  x.PLAN_RD_ROUTE.ToString(),
                       x.PLAN_RD_LENG == null ? "0" : x.PLAN_RD_LENG.ToString(),
                       x.MAST_HAB_NAME == null ? "NA" : x.MAST_HAB_NAME.ToString(),
                        x.MAST_HAB_TOT_POP.ToString()             
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



        public Array CN5StateReportListingDAL(int roadcategory, string route, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            //List<USP_CN4_REPORT_Result> listCN4Reports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 1;
                int State = 0;
                int District = 0;
                int Block = 0;
                int RoadCategory = roadcategory;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                string Route = route;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 1800;
                dbContext.Configuration.LazyLoadingEnabled = false;
                // var listCN4Reports = dbContext.USP_CN4_REPORT(Level, State, District, Block, RoadCat, Route, PMGSY).ToList<USP_CN4_REPORT_Result>();

                //listCN4Reports = dbContext.Database.SqlQuery<USP_CN4_REPORT_Result>("EXEC [omms].[USP_CN4_REPORT] @Level,@State,@District,@Block,@RoadCat,@Route,@PMGSY",
                //    new SqlParameter("@Level", Level),
                //    new SqlParameter("@State", State),
                //    new SqlParameter("@District", District),
                //    new SqlParameter("@Block", Block),
                //    new SqlParameter("@RoadCat", RoadCategory),
                //    new SqlParameter("@Route", Route),
                //    new SqlParameter("@PMGSY", PMGSY)
                //    ).ToList<USP_CN4_REPORT_Result>();
                var listCN4Reports = dbContext.USP_CN4_REPORT(Level, State, District, Block, RoadCategory, Route, PMGSY).ToList<USP_CN4_REPORT_Result>();


                totalRecords = listCN4Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listCN4Reports = listCN4Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listCN4Reports = listCN4Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listCN4Reports = listCN4Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listCN4Reports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[]{
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='CN5DistrictReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+Route.ToString().Trim()+"\",\""+RoadCategory.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",                       
                       x.TOTAL_CN == null ? 0.ToString() : x.TOTAL_CN.ToString(),
                        x.TOTAL_LEN.ToString(),
                        x.BT_LEN.ToString(),
                        x.WBM_LEN.ToString(),
                        x.GRAVEL_LEN.ToString(),
                        x.TRACK_LEN.ToString(),
                        "0",
                        x.NH_BT_LEN.ToString(),
                        x.NH_WBM_LEN.ToString(),
                         "0",
                        x.SH_BT_LEN.ToString(),
                        x.SH_WBM_LEN.ToString(),
                         "0",
                        x.MDR_BT_LEN.ToString(),
                        x.MDR_WBM_LEN.ToString(),
                        x.MDR_GRAVEL_LEN.ToString(),
                        x.MDR_TRACK_LEN.ToString(),
                         "0",
                        x.LR_BT_LEN.ToString(),
                        x.LR_WBM_LEN.ToString(),
                        x.LR_GRAVEL_LEN.ToString(),
                        x.LR_TRACK_LEN.ToString(),
                         "0",
                        x.TR_BT_LEN.ToString(),
                        x.TR_WBM_LEN.ToString(),
                        x.TR_GRAVEL_LEN.ToString(),
                        x.LR_TRACK_LEN.ToString(),
                         "0"
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


        public Array CN5DistrictReportListingDAL(int roadcategory, string route, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            //List<USP_CN4_REPORT_Result> listCN4Reports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 2;
                int State = stateCode;
                int District = 0;
                int Block = 0;
                int RoadCat = roadcategory;
                //  char Route ="%";
                string Route = route;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 1800;
                dbContext.Configuration.LazyLoadingEnabled = false;
                //listCN4Reports = dbContext.Database.SqlQuery<USP_CN4_REPORT_Result>("EXEC [omms].[USP_CN4_REPORT] @Level,@State,@District,@Block,@RoadCat,@Route,@PMGSY",
                //    new SqlParameter("@Level", Level),
                //    new SqlParameter("@State", State),
                //    new SqlParameter("@District", District),
                //    new SqlParameter("@Block", Block),
                //    new SqlParameter("@RoadCat", RoadCat),
                //    new SqlParameter("@Route", Route),
                //    new SqlParameter("@PMGSY", PMGSY)
                //    ).ToList<USP_CN4_REPORT_Result>();
                var listCN4Reports = dbContext.USP_CN4_REPORT(Level, State, District, Block, RoadCat, Route, PMGSY).ToList<USP_CN4_REPORT_Result>();


                totalRecords = listCN4Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listCN4Reports = listCN4Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listCN4Reports = listCN4Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listCN4Reports = listCN4Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listCN4Reports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[]{
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='CN5BlockReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+stateCode+"\",\""+x.LOCATION_NAME+"\",\""+Route.ToString().Trim()+"\",\""+RoadCat.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                       x.TOTAL_CN == null ? 0.ToString() : x.TOTAL_CN.ToString(),
                        x.TOTAL_LEN.ToString(),
                        x.BT_LEN.ToString(),
                        x.WBM_LEN.ToString(),
                        x.GRAVEL_LEN.ToString(),
                        x.TRACK_LEN.ToString(),
                        "0",
                        x.NH_BT_LEN.ToString(),
                        x.NH_WBM_LEN.ToString(),
                         "0",
                        x.SH_BT_LEN.ToString(),
                        x.SH_WBM_LEN.ToString(),
                         "0",
                        x.MDR_BT_LEN.ToString(),
                        x.MDR_WBM_LEN.ToString(),
                        x.MDR_GRAVEL_LEN.ToString(),
                        x.MDR_TRACK_LEN.ToString(),
                         "0",
                        x.LR_BT_LEN.ToString(),
                        x.LR_WBM_LEN.ToString(),
                        x.LR_GRAVEL_LEN.ToString(),
                        x.LR_TRACK_LEN.ToString(),
                         "0",
                        x.TR_BT_LEN.ToString(),
                        x.TR_WBM_LEN.ToString(),
                        x.TR_GRAVEL_LEN.ToString(),
                        x.LR_TRACK_LEN.ToString(),
                         "0"
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


        public Array CN5BlockReportListingDAL(int roadcategory, string route, int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            //  List<USP_CN4_REPORT_Result> listCN4Reports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 3;
                int State = stateCode;
                int District = districtCode;
                int Block = 0;
                int RoadCat = roadcategory;
                string Route = route;
                //char Route = '%';

                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;
                //listCN4Reports = dbContext.Database.SqlQuery<USP_CN4_REPORT_Result>("EXEC [omms].[USP_CN4_REPORT] @Level,@State,@District,@Block,@RoadCat,@Route,@PMGSY",
                //    new SqlParameter("@Level", Level),
                //    new SqlParameter("@State", State),
                //    new SqlParameter("@District", District),
                //    new SqlParameter("@Block", Block),
                //    new SqlParameter("@RoadCat", RoadCat),
                //    new SqlParameter("@Route", Route),
                //    new SqlParameter("@PMGSY", PMGSY)
                //    ).ToList<USP_CN4_REPORT_Result>();
                var listCN4Reports = dbContext.USP_CN4_REPORT(Level, State, District, Block, RoadCat, Route, PMGSY).ToList<USP_CN4_REPORT_Result>();


                totalRecords = listCN4Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listCN4Reports = listCN4Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listCN4Reports = listCN4Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listCN4Reports = listCN4Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listCN4Reports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[]{
                       "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='CN5FinalReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+districtCode.ToString()+"\",\""+stateCode.ToString()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+Route.ToString().Trim()+"\",\""+RoadCat.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                        x.TOTAL_CN == null ? 0.ToString() : x.TOTAL_CN.ToString(),
                        x.TOTAL_LEN.ToString(),
                        x.BT_LEN.ToString(),
                        x.WBM_LEN.ToString(),
                        x.GRAVEL_LEN.ToString(),
                        x.TRACK_LEN.ToString(),
                        "0",
                        x.NH_BT_LEN.ToString(),
                        x.NH_WBM_LEN.ToString(),
                         "0",
                        x.SH_BT_LEN.ToString(),
                        x.SH_WBM_LEN.ToString(),
                         "0",
                        x.MDR_BT_LEN.ToString(),
                        x.MDR_WBM_LEN.ToString(),
                        x.MDR_GRAVEL_LEN.ToString(),
                        x.MDR_TRACK_LEN.ToString(),
                         "0",
                        x.LR_BT_LEN.ToString(),
                        x.LR_WBM_LEN.ToString(),
                        x.LR_GRAVEL_LEN.ToString(),
                        x.LR_TRACK_LEN.ToString(),
                         "0",
                        x.TR_BT_LEN.ToString(),
                        x.TR_WBM_LEN.ToString(),
                        x.TR_GRAVEL_LEN.ToString(),
                        x.LR_TRACK_LEN.ToString(),
                         "0"
                    }
                }).ToArray();
            }
            catch
            {
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        public Array CN5FinalListingDAL(int pop, int roadcategory, string route, int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            //List<USP_CN_ROADWISE_REPORT_Result> listCN4Reports;

            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {

                int Level = 2;
                int State = stateCode;
                int District = districtCode;
                int Block = blockCode;
                int Pop = pop;
                int Road = roadcategory;
                string Route = route;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 180;
                var listCN4Reports = dbContext.USP_CN_REPORT(Level, State, District, Block, Pop, Road, Route, PMGSY).ToList<USP_CN_REPORT_Result>();

                //listCN4Reports = dbContext.Database.SqlQuery<USP_CN_ROADWISE_REPORT_Result>("EXEC [omms].[USP_CN_REPORT] @LEVEL,@STATE,@DISTRICT,@BLOCK,@POP,@ROADCAT,@ROUTE,@PMGSY",
                //    new SqlParameter("@LEVEL", Level),
                //    new SqlParameter("@STATE", State),
                //    new SqlParameter("@DISTRICT", District),
                //    new SqlParameter("@BLOCK", Block),
                //     new SqlParameter("@POP", Pop),
                //    new SqlParameter("@ROADCAT", Road),
                //    new SqlParameter("@ROUTE", Route),
                //    new SqlParameter("@PMGSY", PMGSY)
                //).ToList<USP_CN_ROADWISE_REPORT_Result>();


                totalRecords = listCN4Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listCN4Reports = listCN4Reports.OrderBy(x => x.PLAN_RD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listCN4Reports = listCN4Reports.OrderByDescending(x => x.PLAN_RD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listCN4Reports = listCN4Reports.OrderBy(x => x.PLAN_RD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listCN4Reports.Select(x => new
                {
                    id = x.PLAN_CN_ROAD_NUMBER,
                    cell = new[]{
                        x.PLAN_CN_ROAD_NUMBER == null ? "NA" : x.PLAN_CN_ROAD_NUMBER.ToString() ,
                       x.PLAN_RD_NAME == null ? "NA" : x.PLAN_RD_NAME.ToString(),
                       x.MAST_ER_ROAD_NUMBER == null ? "NA" : x.MAST_ER_ROAD_NUMBER.ToString(),
                       x.MAST_ER_ROAD_NAME == null ? "NA" : x.MAST_ER_ROAD_NAME.ToString(),
                       x.PLAN_RD_LENGTH == null ? "0" : x.PLAN_RD_LENGTH.ToString(),
                       x.RD_FROM == null ? "NA" : x.RD_FROM.ToString(),
                       x.RD_TO == null ? "NA" :  x.RD_TO.ToString(),
                       x.PLAN_RD_ROUTE == null ? "NA" :  x.PLAN_RD_ROUTE.ToString(),
                       x.PLAN_RD_LENG == null ? "0" : x.PLAN_RD_LENG.ToString(),
                       x.MAST_HAB_NAME == null ? "NA" : x.MAST_HAB_NAME.ToString(),
                        x.MAST_HAB_TOT_POP.ToString()             
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



        public Array CN6StateReportListingDAL(string route, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            //List<USP_CN6_REPORT_Result> listCN6Reports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 1;
                int State = 0;
                int District = 0;
                int Block = 0;
                // char Route = '%';
                string Route = route;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 180;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                //listCN6Reports = dbContext.Database.SqlQuery<USP_CN6_REPORT_Result>("EXEC [omms].[USP_CN6_REPORT] @Level,@State,@District,@Block,@Route,@PMGSY",
                //    new SqlParameter("@Level", Level),
                //    new SqlParameter("@State", State),
                //    new SqlParameter("@District", District),
                //    new SqlParameter("@Block", Block),
                //    new SqlParameter("@Route", Route),
                //    new SqlParameter("@PMGSY", PMGSY)
                //    ).ToList<USP_CN6_REPORT_Result>();
                var listCN6Reports = dbContext.USP_CN6_REPORT(Level, State, District, Block, Route, PMGSY).ToList<USP_CN6_REPORT_Result>();

                totalRecords = listCN6Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listCN6Reports = listCN6Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listCN6Reports = listCN6Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listCN6Reports = listCN6Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listCN6Reports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[]{
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='CN6DistrictReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+Route.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",                       
                        x.TOTAL_CN == null ? 0.ToString() : x.TOTAL_CN.ToString(),
                        x.TOTAL_LEN.ToString(),
                        x.TOTAL_HABS.ToString(),
                        x.TOTAL_POP.ToString()                       
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


        public Array CN6DistrictReportListingDAL(string route, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            // List<USP_CN6_REPORT_Result> listCN6Reports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 2;
                int State = stateCode;
                int District = 0;
                int Block = 0;
                //char Route = '%';
                string Route = route;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 180;
                //listCN6Reports = dbContext.Database.SqlQuery<USP_CN6_REPORT_Result>("EXEC [omms].[USP_CN6_REPORT] @Level,@State,@District,@Block,@Route,@PMGSY",
                //    new SqlParameter("@Level", Level),
                //    new SqlParameter("@State", State),
                //    new SqlParameter("@District", District),
                //    new SqlParameter("@Block", Block),
                //    new SqlParameter("@Route", Route),
                //    new SqlParameter("@PMGSY", PMGSY)
                //    ).ToList<USP_CN6_REPORT_Result>();
                var listCN6Reports = dbContext.USP_CN6_REPORT(Level, State, District, Block, Route, PMGSY).ToList<USP_CN6_REPORT_Result>();

                totalRecords = listCN6Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listCN6Reports = listCN6Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listCN6Reports = listCN6Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listCN6Reports = listCN6Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listCN6Reports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[]{
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='CN6BlockReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+stateCode+"\",\""+x.LOCATION_NAME+"\",\""+Route.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                        x.TOTAL_CN == null ? 0.ToString() : x.TOTAL_CN.ToString(),
                        x.TOTAL_LEN.ToString(),
                        x.TOTAL_HABS.ToString(),
                        x.TOTAL_POP.ToString()  
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


        public Array CN6BlockReportListingDAL(string route, int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            //List<USP_CN6_REPORT_Result> listCN6Reports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 3;
                int State = stateCode;
                int District = districtCode;
                int Block = 0;
                string Route = route;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 180;
                //listCN6Reports = dbContext.Database.SqlQuery<USP_CN6_REPORT_Result>("EXEC [omms].[USP_CN6_REPORT] @Level,@State,@District,@Block,@Route,@PMGSY",
                //    new SqlParameter("@Level", Level),
                //    new SqlParameter("@State", State),
                //    new SqlParameter("@District", District),
                //    new SqlParameter("@Block", Block),
                //    new SqlParameter("@Route", Route),
                //    new SqlParameter("@PMGSY", PMGSY)
                //    ).ToList<USP_CN6_REPORT_Result>();
                var listCN6Reports = dbContext.USP_CN6_REPORT(Level, State, District, Block, Route, PMGSY).ToList<USP_CN6_REPORT_Result>();

                totalRecords = listCN6Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listCN6Reports = listCN6Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listCN6Reports = listCN6Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listCN6Reports = listCN6Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listCN6Reports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[]{
                       "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='CN6FinalReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+districtCode.ToString()+"\",\""+stateCode.ToString()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+Route.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                      // x.LOCATION_NAME==null?0.ToString():x.LOCATION_NAME.ToString(),
                        x.TOTAL_CN == null ? 0.ToString() : x.TOTAL_CN.ToString(),
                        x.TOTAL_LEN.ToString(),
                        x.TOTAL_HABS.ToString(),
                        x.TOTAL_POP.ToString()                      
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


        public Array CN6FinalListingDAL(string route, int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            // List<USP_CN6_FinalLevel_REPORT_Result> listCN6Reports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                //int Level = 4;
                int State = stateCode;
                int District = districtCode;
                int Block = blockCode;
                string Route = route;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 180;
                var listCN6Reports = dbContext.USP_CN6LVL_REPORT(State, District, Block, Route, PMGSY).ToList<USP_CN6LVL_REPORT_Result>();
                //listCN6Reports = dbContext.Database.SqlQuery<USP_CN6_FinalLevel_REPORT_Result>("EXEC [omms].[USP_CN6LVL_REPORT] @State,@District,@Block,@Route,@PMGSY",
                //    new SqlParameter("@State", State),
                //    new SqlParameter("@District", District),
                //    new SqlParameter("@Block", Block),
                //    new SqlParameter("@Route", Route),
                //    new SqlParameter("@PMGSY", PMGSY)

                //    ).ToList<USP_CN6_FinalLevel_REPORT_Result>();


                totalRecords = listCN6Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listCN6Reports = listCN6Reports.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listCN6Reports = listCN6Reports.OrderByDescending(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listCN6Reports = listCN6Reports.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listCN6Reports.Select(x => new
                {
                    id = x.PLAN_CN_ROAD_NUMBER,
                    cell = new[]{
                        x.MAST_BLOCK_NAME.ToString(),
                        x.MAST_HAB_NAME.ToString(),
                        x.MAST_HAB_TOT_POP.ToString(),
                        x.RD_FROM== null ? "NA" : x.RD_FROM.ToString(),
                        x.RD_TO == null ? "NA" : x.RD_TO.ToString(),
                        x.PLAN_RD_LENGTH == null ? 0.ToString() :x.PLAN_RD_LENGTH.ToString(),      
                        x.PLAN_CN_ROAD_NUMBER.ToString(),
                        x.MAST_INC_HAB_NAME.ToString(),                   
                        x.MAST_INC_HAB_TOT_POP.ToString()
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

        public Array CNCPLStateListingDAL(string route, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            // List<USP_CNCPL_REPORT_Result> listCNCPLReports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 1;
                int State = 0;
                int District = 0;
                int Block = 0;
                //  char Route="%";
                string Route = route;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 180;
                //listCNCPLReports = dbContext.Database.SqlQuery<USP_CNCPL_REPORT_Result>("EXEC [omms].[USP_CNCPL_REPORT] @Level,@State,@District,@Block,@Route,@PMGSY",
                //    new SqlParameter("@Level", Level),
                //    new SqlParameter("@State", State),
                //    new SqlParameter("@District", District),
                //    new SqlParameter("@Block", Block),
                //    new SqlParameter("@Route", Route),
                //    new SqlParameter("@PMGSY", PMGSYSession.Current.PMGSYScheme)
                //).ToList<USP_CNCPL_REPORT_Result>();
                var listCNCPLReports = dbContext.USP_CNCPL_REPORT(Level, State, District, Block, Route, PMGSY).ToList<USP_CNCPL_REPORT_Result>();

                totalRecords = listCNCPLReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listCNCPLReports = listCNCPLReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listCNCPLReports = listCNCPLReports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listCNCPLReports = listCNCPLReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listCNCPLReports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[]{
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='CNCPLDistrictReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+Route.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",                       
                        x.TOTAL_CN == null ? 0.ToString() : x.TOTAL_CN.ToString(),
                        x.TOTAL_LEN.ToString(),
                        x.TOTAL_HABS.ToString(),
                        x.TARGET_POP.ToString()                       
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

        public Array CNCPLDistrictListingDAL(string route, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            // List<USP_CNCPL_REPORT_Result> listCNCPLReports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 2;
                int State = stateCode;
                int District = 0;
                int Block = 0;
                string Route = route;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;

                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 180;
                //listCNCPLReports = dbContext.Database.SqlQuery<USP_CNCPL_REPORT_Result>("EXEC [omms].[USP_CNCPL_REPORT] @Level,@State,@District,@Block,@Route,@PMGSY",
                //    new SqlParameter("@Level", Level),
                //    new SqlParameter("@State", State),
                //    new SqlParameter("@District", District),
                //    new SqlParameter("@Block", Block),
                //    new SqlParameter("@Route", Route),
                //    new SqlParameter("@PMGSY", PMGSY)
                //).ToList<USP_CNCPL_REPORT_Result>();
                var listCNCPLReports = dbContext.USP_CNCPL_REPORT(Level, State, District, Block, Route, PMGSY).ToList<USP_CNCPL_REPORT_Result>();


                totalRecords = listCNCPLReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listCNCPLReports = listCNCPLReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listCNCPLReports = listCNCPLReports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listCNCPLReports = listCNCPLReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listCNCPLReports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[]{
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='CNCPLBlockReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+stateCode+"\",\""+x.LOCATION_NAME+"\",\""+Route.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                        x.TOTAL_CN == null ? 0.ToString() : x.TOTAL_CN.ToString(),
                        x.TOTAL_LEN.ToString(),
                        x.TOTAL_HABS.ToString(),
                        x.TARGET_POP.ToString()                       
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

        public Array CNCPLBlockListingDAL(string route, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            //List<USP_CNCPL_REPORT_Result> listCNCPLReports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 3;
                int State = stateCode;
                int District = districtCode;
                int Block = 0;
                string Route = route;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 180;
                //listCNCPLReports = dbContext.Database.SqlQuery<USP_CNCPL_REPORT_Result>("EXEC [omms].[USP_CNCPL_REPORT] @Level,@State,@District,@Block,@Route,@PMGSY",
                //    new SqlParameter("@Level", Level),
                //    new SqlParameter("@State", State),
                //    new SqlParameter("@District", District),
                //    new SqlParameter("@Block", Block),
                //    new SqlParameter("@Route", Route),
                //    new SqlParameter("@PMGSY", Tinyint)
                //).ToList<USP_CNCPL_REPORT_Result>();
                var listCNCPLReports = dbContext.USP_CNCPL_REPORT(Level, State, District, Block, Route, PMGSY).ToList<USP_CNCPL_REPORT_Result>();

                totalRecords = listCNCPLReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listCNCPLReports = listCNCPLReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listCNCPLReports = listCNCPLReports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listCNCPLReports = listCNCPLReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listCNCPLReports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[]{
                       "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='CNCPLFinalReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+districtCode.ToString()+"\",\""+stateCode.ToString()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+Route.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                        x.TOTAL_CN == null ? 0.ToString() : x.TOTAL_CN.ToString(),
                        x.TOTAL_LEN.ToString(),
                        x.TOTAL_HABS.ToString(),
                        x.TARGET_POP.ToString()                       
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

        public Array CNCPLFinalListingDAL(string route, int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            //  List<USP_CNCPL_LVL_REPORT_Result> listCNCPLReports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                //int Level = 4;
                int State = stateCode;
                int District = districtCode;
                int Block = blockCode;
                string Route = route;
                byte Tinyint = PMGSYSession.Current.PMGSYScheme;

                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 180;
                var listCNCPLReports = dbContext.USP_CNCPL_LVL_REPORT(State, District, Block, Route, PMGSYSession.Current.PMGSYScheme).ToList();

                //listCNCPLReports = dbContext.Database.SqlQuery<USP_CNCPL_LVL_REPORT_Result>("EXEC [omms].[USP_CNCPL_LVL_REPORT] @State,@District,@Block,@Route,@PMGSY",

                //    new SqlParameter("@State", State),
                //    new SqlParameter("@District", District),
                //    new SqlParameter("@Block", Block),
                //    new SqlParameter("@Route", Route),
                //    new SqlParameter("@PMGSY", Tinyint)
                //).ToList<USP_CNCPL_LVL_REPORT_Result>();

                totalRecords = listCNCPLReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listCNCPLReports = listCNCPLReports.OrderBy(x => x.PLAN_RD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listCNCPLReports = listCNCPLReports.OrderByDescending(x => x.PLAN_RD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listCNCPLReports = listCNCPLReports.OrderBy(x => x.PLAN_RD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listCNCPLReports.Select(x => new
                {
                    id = x.PLAN_CN_ROAD_NUMBER,
                    cell = new[]{
                        x.PLAN_CN_ROAD_NUMBER,
                        x.PLAN_RD_NAME,
                        x.PLAN_RD_ROUTE,
                        x.PLAN_RD_LENG,
                        x.PLAN_RD_LENGTH.ToString(),	
                        x.RD_FROM,
                        x.RD_TO,
                        x.MAST_HAB_NAME,
                        x.MAST_HAB_TOT_POP.ToString(),
                        x.TOTAL_POP_SERVERD.ToString(),                      
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

        public Array HWCNStateListingDAL(string route, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            // List<USP_CN_HAB_REPORT_Result> listHWCNReports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 1;
                int State = 0;
                int District = 0;
                int Block = 0;
                //  char Route = '%';
                string Route = route;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;

                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 180;
                //listHWCNReports = dbContext.Database.SqlQuery<USP_CN_HAB_REPORT_Result>("EXEC [omms].[USP_CN_HAB_REPORT] @Level,@State,@District,@Block,@Route,@PMGSY",
                //    new SqlParameter("@Level", Level),
                //    new SqlParameter("@State", State),
                //    new SqlParameter("@District", District),
                //    new SqlParameter("@Block", Block),
                //    new SqlParameter("@Route", Route),
                //    new SqlParameter("@PMGSY", PMGSY)
                //).ToList<USP_CN_HAB_REPORT_Result>();
                var listHWCNReports = dbContext.USP_CN_HAB_REPORT(Level, State, District, Block, Route, PMGSY).ToList<USP_CN_HAB_REPORT_Result>();


                totalRecords = listHWCNReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listHWCNReports = listHWCNReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listHWCNReports = listHWCNReports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listHWCNReports = listHWCNReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listHWCNReports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[]{
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='HWCNDistrictReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+Route.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",                       
                        x.TOTAL_CN == null ? 0.ToString() : x.TOTAL_CN.ToString(),
                        x.TOTAL_LEN.ToString(),
                        x.TOTAL_HABS.ToString(),
                        x.TOTAL_POP.ToString()                       
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

        public Array HWCNDistrictListingDAL(string route, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            // List<USP_CN_HAB_REPORT_Result> listHWCNReports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 2;
                int State = stateCode;
                int District = 0;
                int Block = 0;
                string Route = route;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 180;

                //listHWCNReports = dbContext.Database.SqlQuery<USP_CN_HAB_REPORT_Result>("EXEC [omms].[USP_CN_HAB_REPORT] @Level,@State,@District,@Block,@Route,@PMGSY",
                //    new SqlParameter("@Level", Level),
                //    new SqlParameter("@State", State),
                //    new SqlParameter("@District", District),
                //    new SqlParameter("@Block", Block),
                //    new SqlParameter("@Route", Route),
                //    new SqlParameter("@PMGSY", PMGSY)
                //).ToList<USP_CN_HAB_REPORT_Result>();
                var listHWCNReports = dbContext.USP_CN_HAB_REPORT(Level, State, District, Block, Route, PMGSY).ToList<USP_CN_HAB_REPORT_Result>();


                totalRecords = listHWCNReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listHWCNReports = listHWCNReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listHWCNReports = listHWCNReports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listHWCNReports = listHWCNReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listHWCNReports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[]{
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='HWCNBlockReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+stateCode+"\",\""+x.LOCATION_NAME+"\",\""+Route.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                        x.TOTAL_CN == null ? 0.ToString() : x.TOTAL_CN.ToString(),
                        x.TOTAL_LEN.ToString(),
                        x.TOTAL_HABS.ToString(),
                        x.TOTAL_POP.ToString()  
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

        public Array HWCNBlockListingDAL(string route, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            // List<USP_CN_HAB_REPORT_Result> listHWCNReports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 3;
                int State = stateCode;
                int District = districtCode;
                int Block = 0;
                string Route = route;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme; ;

                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 180;
                //listHWCNReports = dbContext.Database.SqlQuery<USP_CN_HAB_REPORT_Result>("EXEC [omms].[USP_CN_HAB_REPORT] @Level,@State,@District,@Block,@Route,@PMGSY",
                //    new SqlParameter("@Level", Level),
                //    new SqlParameter("@State", State),
                //    new SqlParameter("@District", District),
                //    new SqlParameter("@Block", Block),
                //    new SqlParameter("@Route", Route),
                //    new SqlParameter("@PMGSY", Tinyint)
                //).ToList<USP_CN_HAB_REPORT_Result>();
                var listHWCNReports = dbContext.USP_CN_HAB_REPORT(Level, State, District, Block, Route, PMGSY).ToList<USP_CN_HAB_REPORT_Result>();

                totalRecords = listHWCNReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listHWCNReports = listHWCNReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listHWCNReports = listHWCNReports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listHWCNReports = listHWCNReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listHWCNReports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[]{
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='HWCNFinalReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+districtCode.ToString()+"\",\""+stateCode.ToString()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+Route.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                      // x.LOCATION_NAME==null?0.ToString():x.LOCATION_NAME.ToString(),
                        x.TOTAL_CN == null ? 0.ToString() : x.TOTAL_CN.ToString(),
                        x.TOTAL_LEN.ToString(),
                        x.TOTAL_HABS.ToString(),
                        x.TOTAL_POP.ToString()
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

        public Array HWCNFinalListingDAL(string route, int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            //List<USP_CN_HAB_FINAL_REPORT_Result> listHWCNReports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                //int Level = 4;
                int State = stateCode;
                int District = districtCode;
                int Block = blockCode;
                string Route = route;
                byte Tinyint = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 180;
                var listHWCNReports = dbContext.USP_CN_HABLVL_REPORT(State, District, Block, Route, Tinyint).ToList<USP_CN_HABLVL_REPORT_Result>();

                //listHWCNReports = dbContext.Database.SqlQuery<USP_CN_HAB_FINAL_REPORT_Result>("EXEC [omms].[USP_CN_HAB_REPORT] @Level,@State,@District,@Block,@Route,@PMGSY",
                //    new SqlParameter("@Level", Level),
                //    new SqlParameter("@State", State),
                //    new SqlParameter("@District", District),
                //    new SqlParameter("@Block", Block),
                //    new SqlParameter("@Route", Route),
                //    new SqlParameter("@PMGSY", Tinyint)
                //).ToList<USP_CN_HAB_FINAL_REPORT_Result>();

                totalRecords = listHWCNReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listHWCNReports = listHWCNReports.OrderBy(x => x.PLAN_RD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listHWCNReports = listHWCNReports.OrderByDescending(x => x.PLAN_RD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listHWCNReports = listHWCNReports.OrderBy(x => x.PLAN_RD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listHWCNReports.Select(x => new
                {
                    id = x.PLAN_CN_ROAD_NUMBER,
                    cell = new[]{
                        x.MAST_HAB_NAME,	
                        x.PLAN_CN_ROAD_NUMBER,	
                        x.PLAN_RD_NAME,	
                        x.PLAN_RD_ROUTE,
                        x.PLAN_RD_FROM_CHAINAGE==null?"0":x.PLAN_RD_FROM_CHAINAGE.ToString(),
                        x.PLAN_RD_TO_CHAINAGE==null?"0":x.PLAN_RD_TO_CHAINAGE.ToString(),
                        x.PLAN_RD_LENG==null?"0":x.PLAN_RD_LENG.ToString(),	
                        x.PLAN_RD_LENGTH==null?"0":x.PLAN_RD_LENGTH.ToString(),	
                        x.RD_FROM==null?"NA":x.RD_FROM.ToString(),	
                        x.RD_TO==null?"NA":x.RD_TO.ToString(),	
                        x.MAST_HAB_TOT_POP.ToString(),                      
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

        public Array RWCNStateListingDAL(string route, int road, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            //List<USP_CN_ROAD_REPORT_Result> listRWCNReports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 1;
                int State = 0;
                int District = 0;
                int Block = 0;
                //char Route = '%';
                string Route = route;
                int Road = road;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;

                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 180;
                //listRWCNReports = dbContext.Database.SqlQuery<USP_CN_ROAD_REPORT_Result>("EXEC [omms].[USP_CN_ROAD_REPORT] @Level,@State,@District,@Block,@Route,@Road,@PMGSY",
                //    new SqlParameter("@Level", Level),
                //    new SqlParameter("@State", State),
                //    new SqlParameter("@District", District),
                //    new SqlParameter("@Block", Block),
                //    new SqlParameter("@Route", Route),
                //    new SqlParameter("@Road", Road),
                //    new SqlParameter("@PMGSY", PMGSY)
                //).ToList<USP_CN_ROAD_REPORT_Result>();
                var listRWCNReports = dbContext.USP_CN_ROAD_REPORT(Level, State, District, Block, Route, Road, PMGSY).ToList<USP_CN_ROAD_REPORT_Result>();

                totalRecords = listRWCNReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listRWCNReports = listRWCNReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listRWCNReports = listRWCNReports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listRWCNReports = listRWCNReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listRWCNReports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[]{
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='RWCNDistrictReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+Route.ToString().Trim()+"\",\""+Road.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",                       
                        x.TOTAL_NH.ToString(),
                        x.TOTAL_NH_LEN.ToString(),
                        x.TOTAL_SH.ToString(),
                        x.TOTAL_SH_LEN.ToString(),
                        x.TOTAL_MDR.ToString(),
                        x.TOTAL_MDR_LEN.ToString() ,
                        x.TOTAL_OTHER.ToString(),
                        x.TOTAL_OTHER_LEN.ToString() ,
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

        public Array RWCNDistrictListingDAL(string route, int road, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            // List<USP_CN_ROAD_REPORT_Result> listRWCNReports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 2;
                int State = stateCode;
                int District = 0;
                int Block = 0;
                string Route = route;
                int Road = road;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme; ;

                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 180;
                //listRWCNReports = dbContext.Database.SqlQuery<USP_CN_ROAD_REPORT_Result>("EXEC [omms].[USP_CN_ROAD_REPORT] @Level,@State,@District,@Block,@Route,@Road,@PMGSY",
                //    new SqlParameter("@Level", Level),
                //    new SqlParameter("@State", State),
                //    new SqlParameter("@District", District),
                //    new SqlParameter("@Block", Block),
                //    new SqlParameter("@Route", Route),
                //    new SqlParameter("@Road", Road),
                //    new SqlParameter("@PMGSY", PMGSY)
                //).ToList<USP_CN_ROAD_REPORT_Result>();
                var listRWCNReports = dbContext.USP_CN_ROAD_REPORT(Level, State, District, Block, Route, Road, PMGSY).ToList<USP_CN_ROAD_REPORT_Result>();

                totalRecords = listRWCNReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listRWCNReports = listRWCNReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listRWCNReports = listRWCNReports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listRWCNReports = listRWCNReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listRWCNReports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[]{
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='RWCNBlockReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+stateCode+"\",\""+x.LOCATION_NAME+"\",\""+Route.ToString().Trim()+"\",\""+Road.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                        x.TOTAL_NH.ToString(),
                        x.TOTAL_NH_LEN.ToString(),
                        x.TOTAL_SH.ToString(),
                        x.TOTAL_SH_LEN.ToString(),
                        x.TOTAL_MDR.ToString(),
                        x.TOTAL_MDR_LEN.ToString() ,
                        x.TOTAL_OTHER.ToString(),
                        x.TOTAL_OTHER_LEN.ToString() ,                      
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

        public Array RWCNBlockListingDAL(string route, int road, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            // List<USP_CN_ROAD_REPORT_Result> listRWCNReports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 3;
                int State = stateCode;
                int District = districtCode;
                int Block = 0;
                string Route = route;
                int Road = road;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 180;
                //listRWCNReports = dbContext.Database.SqlQuery<USP_CN_ROAD_REPORT_Result>("EXEC [omms].[USP_CN_ROAD_REPORT] @Level,@State,@District,@Block,@Route,@Road,@PMGSY",
                //    new SqlParameter("@Level", Level),
                //    new SqlParameter("@State", State),
                //    new SqlParameter("@District", District),
                //    new SqlParameter("@Block", Block),
                //    new SqlParameter("@Route", Route),
                //    new SqlParameter("@Road", Road),
                //    new SqlParameter("@PMGSY", PMGSY)
                //).ToList<USP_CN_ROAD_REPORT_Result>();
                var listRWCNReports = dbContext.USP_CN_ROAD_REPORT(Level, State, District, Block, Route, Road, PMGSY).ToList<USP_CN_ROAD_REPORT_Result>();


                totalRecords = listRWCNReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listRWCNReports = listRWCNReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listRWCNReports = listRWCNReports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listRWCNReports = listRWCNReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listRWCNReports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[]{
                       "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='RWCNFinalReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+districtCode.ToString()+"\",\""+stateCode.ToString()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+Route.ToString().Trim()+"\",\""+Road.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                      // x.LOCATION_NAME.ToString(),
                       x.TOTAL_NH.ToString(),
                        x.TOTAL_NH_LEN.ToString(),
                        x.TOTAL_SH.ToString(),
                        x.TOTAL_SH_LEN.ToString(),
                        x.TOTAL_MDR.ToString(),
                        x.TOTAL_MDR_LEN.ToString() ,
                        x.TOTAL_OTHER.ToString(),
                        x.TOTAL_OTHER_LEN.ToString() ,                    
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

        public Array RWCNFinalListingDAL(int pop, string route, int road, int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            //List<USP_CN_ROADWISE_REPORT_Result> listRWCNReports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 2;
                int State = stateCode;
                int District = districtCode;
                int Block = blockCode;
                int Pop = pop;
                string Route = route;
                int Road = road;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 180;
                var listRWCNReports = dbContext.USP_CN_REPORT(Level, State, District, Block, Pop, Road, Route, PMGSY).ToList<USP_CN_REPORT_Result>();

                //listRWCNReports = dbContext.Database.SqlQuery<USP_CN_ROADWISE_REPORT_Result>("EXEC [omms].[USP_CN_REPORT] @LEVEL,@STATE,@DISTRICT,@BLOCK,@POP,@ROADCAT,@ROUTE,@PMGSY",
                //    new SqlParameter("@LEVEL", Level),
                //    new SqlParameter("@STATE", State),
                //    new SqlParameter("@DISTRICT", District),
                //    new SqlParameter("@BLOCK", Block),
                //     new SqlParameter("@POP", Pop),
                //    new SqlParameter("@ROADCAT", Road),
                //       new SqlParameter("@ROUTE", Route),
                //    new SqlParameter("@PMGSY", PMGSY)
                //).ToList<USP_CN_ROADWISE_REPORT_Result>();

                totalRecords = listRWCNReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listRWCNReports = listRWCNReports.OrderBy(x => x.PLAN_CN_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listRWCNReports = listRWCNReports.OrderByDescending(x => x.PLAN_CN_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listRWCNReports = listRWCNReports.OrderBy(x => x.PLAN_CN_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listRWCNReports.Select(x => new
                {
                    id = x.PLAN_CN_ROAD_NUMBER,
                    cell = new[]{
                        x.PLAN_CN_ROAD_NUMBER,	
                        x.PLAN_RD_NAME,	
                        x.PLAN_RD_ROUTE,	
                        x.PLAN_RD_FROM_CHAINAGE.ToString()	,
                        x.PLAN_RD_TO_CHAINAGE.ToString(),	
                        x.PLAN_RD_LENG,	
                        x.PLAN_RD_LENGTH.ToString(),	
                        x.RD_FROM,	
                        x.RD_TO,   
                        x.MAST_HAB_NAME.ToString(),
                        x.TOTAL_HABS.ToString(),
                        x.MAST_HAB_TOT_POP.ToString()
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

        public Array PCIAbstractStateListingDAL(string route, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            //List<USP_CN_PCI_REPORT_Result> listPCIAbstractReports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 1;
                int State = 0;
                int District = 0;
                int Block = 0;
                string Route = route;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 180;
                //listPCIAbstractReports = dbContext.Database.SqlQuery<USP_CN_PCI_REPORT_Result>("EXEC [omms].[USP_CN_PCI_REPORT] @Level,@State,@District,@Block,@Route,@PMGSY",
                //    new SqlParameter("@Level", Level),
                //    new SqlParameter("@State", State),
                //    new SqlParameter("@District", District),
                //    new SqlParameter("@Block", Block),
                //    new SqlParameter("@Route", Route),
                //    new SqlParameter("@PMGSY", Tinyint)
                //).ToList<USP_CN_PCI_REPORT_Result>();
                var listPCIAbstractReports = dbContext.USP_CN_PCI_REPORT(Level, State, District, Block, Route, PMGSY).ToList<USP_CN_PCI_REPORT_Result>();

                totalRecords = listPCIAbstractReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listPCIAbstractReports = listPCIAbstractReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listPCIAbstractReports = listPCIAbstractReports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listPCIAbstractReports = listPCIAbstractReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listPCIAbstractReports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[]{
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='PCIAbstractDistrictReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+Route.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",                       
                       x.TOTAL_CN.ToString(),
                       x.TOTAL_LEN.ToString(),
                       x.TOTAL_PCI.ToString(),
                       x.TOTAL_PCI_LEN.ToString(),
                       x.TOTAL_PCI_LEN1.ToString(),
                       x.TOTAL_PCI_LEN2.ToString(),
                       x.TOTAL_PCI_LEN3.ToString(),
                       x.TOTAL_PCI_LEN4.ToString(),
                       x.TOTAL_PCI_LEN5.ToString(),                    
                       x.TOTAL_PCI_LY3.ToString(),
                       x.TOTAL_PCI_LY2.ToString(),
                       x.TOTAL_PCI_LY1.ToString()
                    
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

        public Array PCIAbstractDistrictListingDAL(string route, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            // List<USP_CN_PCI_REPORT_Result> listPCIAbstractReports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 2;
                int State = stateCode;
                int District = 0;
                int Block = 0;
                string Route = route;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 180;
                //listPCIAbstractReports = dbContext.Database.SqlQuery<USP_CN_PCI_REPORT_Result>("EXEC [omms].[USP_CN_PCI_REPORT] @Level,@State,@District,@Block,@Route,@PMGSY",
                //    new SqlParameter("@Level", Level),
                //    new SqlParameter("@State", State),
                //    new SqlParameter("@District", District),
                //    new SqlParameter("@Block", Block),
                //    new SqlParameter("@Route", Route),
                //    new SqlParameter("@PMGSY", PMGSY)
                //).ToList<USP_CN_PCI_REPORT_Result>();
                var listPCIAbstractReports = dbContext.USP_CN_PCI_REPORT(Level, State, District, Block, Route, PMGSY).ToList<USP_CN_PCI_REPORT_Result>();

                totalRecords = listPCIAbstractReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listPCIAbstractReports = listPCIAbstractReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listPCIAbstractReports = listPCIAbstractReports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listPCIAbstractReports = listPCIAbstractReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listPCIAbstractReports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[]{
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='PCIAbstractBlockReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+stateCode+"\",\""+x.LOCATION_NAME+"\",\""+Route.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                        x.TOTAL_CN.ToString(),	
                        x.TOTAL_LEN.ToString(),	
                        x.TOTAL_PCI.ToString(),	
                        x.TOTAL_PCI_LEN.ToString(),	
                        x.TOTAL_PCI_LEN1.ToString(),	
                        x.TOTAL_PCI_LEN2.ToString(),	
                        x.TOTAL_PCI_LEN3.ToString(),	
                        x.TOTAL_PCI_LEN4.ToString(),	
                        x.TOTAL_PCI_LEN5.ToString(),
                        x.TOTAL_PCI_LY1.ToString(),	
                        x.TOTAL_PCI_LY2.ToString(),	
                        x.TOTAL_PCI_LY3.ToString()                  
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

        public Array PCIAbstractBlockListingDAL(string route, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            // List<USP_CN_PCI_REPORT_Result> listPCIAbstractReports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 3;
                int State = stateCode;
                int District = districtCode;
                int Block = 0;
                string Route = route;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 180;
                //listPCIAbstractReports = dbContext.Database.SqlQuery<USP_CN_PCI_REPORT_Result>("EXEC [omms].[USP_CN_PCI_REPORT] @Level,@State,@District,@Block,@Route,@PMGSY",
                //    new SqlParameter("@Level", Level),
                //    new SqlParameter("@State", State),
                //    new SqlParameter("@District", District),
                //    new SqlParameter("@Block", Block),
                //    new SqlParameter("@Route", Route),
                //    new SqlParameter("@PMGSY", PMGSY)
                //).ToList<USP_CN_PCI_REPORT_Result>();
                var listPCIAbstractReports = dbContext.USP_CN_PCI_REPORT(Level, State, District, Block, Route, PMGSY).ToList<USP_CN_PCI_REPORT_Result>();

                totalRecords = listPCIAbstractReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listPCIAbstractReports = listPCIAbstractReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listPCIAbstractReports = listPCIAbstractReports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listPCIAbstractReports = listPCIAbstractReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listPCIAbstractReports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[]{
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='PCIAbstractFinalReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+districtCode.ToString()+"\",\""+stateCode.ToString()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+Route.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                      // x.LOCATION_NAME.ToString(),
                       x.TOTAL_CN.ToString(),	
                        x.TOTAL_LEN.ToString(),	
                        x.TOTAL_PCI.ToString(),	
                        x.TOTAL_PCI_LEN.ToString(),	
                        x.TOTAL_PCI_LEN1.ToString(),	
                        x.TOTAL_PCI_LEN2.ToString(),	
                        x.TOTAL_PCI_LEN3.ToString(),	
                        x.TOTAL_PCI_LEN4.ToString(),	
                        x.TOTAL_PCI_LEN5.ToString(),
                        x.TOTAL_PCI_LY1.ToString(),	
                        x.TOTAL_PCI_LY2.ToString(),	
                        x.TOTAL_PCI_LY3.ToString()                  
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

        public Array PCIAbstractFinalListingDAL(string route, int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            List<USP_CN_PCILVL_REPORT_Result_FINAL_REPORT> listPCIAbstractReports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                // int Level = 4;
                int State = stateCode;
                int District = districtCode;
                int Block = blockCode;
                string Route = route;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 180;
               // var listPCIAbstractReports = dbContext.USP_CN_PCILVL_REPORT(State, District, Block, Route, PMGSY).ToList<USP_CN_PCILVL_REPORT_Result>();
                listPCIAbstractReports = dbContext.Database.SqlQuery<USP_CN_PCILVL_REPORT_Result_FINAL_REPORT>("EXEC [omms].[USP_CN_PCILVL_REPORT] @State,@District,@Block,@Route,@PMGSY",
                    new SqlParameter("@State", State),
                    new SqlParameter("@District", District),
                    new SqlParameter("@Block", Block),
                    new SqlParameter("@Route", Route),
                    new SqlParameter("@PMGSY", PMGSY)
                ).ToList<USP_CN_PCILVL_REPORT_Result_FINAL_REPORT>();

                totalRecords = listPCIAbstractReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listPCIAbstractReports = listPCIAbstractReports.OrderBy(x => x.PLAN_CN_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listPCIAbstractReports = listPCIAbstractReports.OrderByDescending(x => x.PLAN_CN_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listPCIAbstractReports = listPCIAbstractReports.OrderBy(x => x.PLAN_CN_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listPCIAbstractReports.Select(x => new
                {
                    id = x.PLAN_CN_ROAD_NUMBER,
                    cell = new[]{
                        x.PLAN_CN_ROAD_NUMBER,	
                        x.PLAN_RD_NAME,	
                        x.PLAN_RD_ROUTE,	
                        x.PLAN_RD_LENGTH.ToString(),	
                        x.MANE_PCI_YEAR.ToString(),	
                        x.MANE_SEGMENT_NO.ToString(),	
                        x.MANE_STR_CHAIN.ToString(),	
                        x.MANE_END_CHAIN.ToString(),	
                        x.MAST_SURFACE_NAME.ToString(),	
                        x.MANE_PCIINDEX.ToString(),                     
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


        public Array CUCPLStateListingDAL(string route, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            // List<USP_CUCPL_REPORT_Result> listCUCPLReports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 1;
                int State = 0;
                int District = 0;
                int Block = 0;
                // char Route = '%';
                string Route = route;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 180;
                //listCUCPLReports = dbContext.Database.SqlQuery<USP_CUCPL_REPORT_Result>("EXEC [omms].[USP_CUCPL_REPORT] @Level,@State,@District,@Block,@Route,@PMGSY",
                //    new SqlParameter("@Level", Level),
                //    new SqlParameter("@State", State),
                //    new SqlParameter("@District", District),
                //    new SqlParameter("@Block", Block),
                //    new SqlParameter("@Route", Route),
                //    new SqlParameter("@PMGSY", PMGSY)
                //).ToList<USP_CUCPL_REPORT_Result>();
                var listCUCPLReports = dbContext.USP_CUCPL_REPORT(Level, State, District, Block, Route, PMGSY).ToList<USP_CUCPL_REPORT_Result>();

                totalRecords = listCUCPLReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listCUCPLReports = listCUCPLReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listCUCPLReports = listCUCPLReports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listCUCPLReports = listCUCPLReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listCUCPLReports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[]{
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='CUCPLDistrictReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+Route.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",                       
                        x.TOTAL_CN.ToString(),
                        x.TOTAL_LEN.ToString(),	
                        x.PCI1.ToString(),	
                        x.PCI2.ToString(),	
                        x.PCI3.ToString(),	
                        x.PCI4.ToString(),
                        x.PCI5.ToString()
                       
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

        public Array CUCPLDistrictListingDAL(string route, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            //List<USP_CUCPL_REPORT_Result> listCUCPLReports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 2;
                int State = stateCode;
                int District = 0;
                int Block = 0;
                string Route = route;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 180;
                //listCUCPLReports = dbContext.Database.SqlQuery<USP_CUCPL_REPORT_Result>("EXEC [omms].[USP_CUCPL_REPORT] @Level,@State,@District,@Block,@Route,@PMGSY",
                //    new SqlParameter("@Level", Level),
                //    new SqlParameter("@State", State),
                //    new SqlParameter("@District", District),
                //    new SqlParameter("@Block", Block),
                //    new SqlParameter("@Route", Route),
                //    new SqlParameter("@PMGSY", PMGSY)
                //).ToList<USP_CUCPL_REPORT_Result>();
                var listCUCPLReports = dbContext.USP_CUCPL_REPORT(Level, State, District, Block, Route, PMGSY).ToList<USP_CUCPL_REPORT_Result>();

                totalRecords = listCUCPLReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listCUCPLReports = listCUCPLReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listCUCPLReports = listCUCPLReports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listCUCPLReports = listCUCPLReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listCUCPLReports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[]{
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='CUCPLBlockReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+stateCode+"\",\""+x.LOCATION_NAME+"\",\""+Route.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                        x.TOTAL_CN.ToString(),
                        x.TOTAL_LEN.ToString(),	
                        x.PCI1.ToString(),	
                        x.PCI2.ToString(),	
                        x.PCI3.ToString(),	
                        x.PCI4.ToString(),
                        x.PCI5.ToString()
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

        public Array CUCPLBlockListingDAL(string route, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            // List<USP_CUCPL_REPORT_Result> listCUCPLReports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 3;
                int State = stateCode;
                int District = districtCode;
                int Block = 0;
                string Route = route;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 180;
                //listCUCPLReports = dbContext.Database.SqlQuery<USP_CUCPL_REPORT_Result>("EXEC [omms].[USP_CUCPL_REPORT] @Level,@State,@District,@Block,@Route,@PMGSY",
                //    new SqlParameter("@Level", Level),
                //    new SqlParameter("@State", State),
                //    new SqlParameter("@District", District),
                //    new SqlParameter("@Block", Block),
                //    new SqlParameter("@Route", Route),
                //    new SqlParameter("@PMGSY", PMGSY)
                //).ToList<USP_CUCPL_REPORT_Result>();
                var listCUCPLReports = dbContext.USP_CUCPL_REPORT(Level, State, District, Block, Route, PMGSY).ToList<USP_CUCPL_REPORT_Result>();

                totalRecords = listCUCPLReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listCUCPLReports = listCUCPLReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listCUCPLReports = listCUCPLReports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listCUCPLReports = listCUCPLReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listCUCPLReports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[]{
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='CUCPLFinalReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+districtCode.ToString()+"\",\""+stateCode.ToString()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+Route.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                     // x.LOCATION_NAME.ToString(),
                       x.TOTAL_CN.ToString(),
                        x.TOTAL_LEN.ToString(),	
                        x.PCI1.ToString(),	
                        x.PCI2.ToString(),	
                        x.PCI3.ToString(),	
                        x.PCI4.ToString(),
                        x.PCI5.ToString()
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

        public Array CUCPLFinalListingDAL(string route, int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                //int Level = 4;
                int State = stateCode;
                int District = districtCode;
                int Block = blockCode;
                string Route = route;
                byte Tinyint = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 180;
                var listCUCPLReports = dbContext.USP_CUCPL_LVL_REPORT(State, District, Block, Route, PMGSYSession.Current.PMGSYScheme).ToList();

                totalRecords = listCUCPLReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listCUCPLReports = listCUCPLReports.OrderBy(x => x.PLAN_CN_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listCUCPLReports = listCUCPLReports.OrderByDescending(x => x.PLAN_CN_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listCUCPLReports = listCUCPLReports.OrderBy(x => x.PLAN_CN_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listCUCPLReports.Select(x => new
                {
                    id = x.PLAN_CN_ROAD_NUMBER,
                    cell = new[]{
                        x.PLAN_CN_ROAD_NUMBER,	
                        x.PLAN_RD_NAME,	
                        x.PLAN_RD_LENGTH.ToString(),	
                        x.MAST_CONS_YEAR.ToString(),	
                        x.MAST_RENEW_YEAR.ToString(),	
                        x.MANE_SEGMENT_NO.ToString(),	
                        x.MANE_PCI_YEAR.ToString(),	                     
                        x.MANE_STR_CHAIN.ToString(),	
                        x.MANE_END_CHAIN.ToString(),	
                        x.MANE_PCIINDEX.ToString(),	
                        x.AVG_PCI.ToString(),	
                        x.POP.ToString(),	
                        x.MAST_TI_YEAR.ToString(),	
                        x.MAST_COMM_TI.ToString(),   
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


        public Array CNR1StateReportListingDAL(int road, string route, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 1;
                int State = 0;
                int District = 0;
                int Block = 0;
                int Road = road;
                string Route = route;
                var listCNR1Reports = dbContext.USP_CN_R1_REPORT(Level, State, District, Block, Road, Route, PMGSYSession.Current.PMGSYScheme).ToList<USP_CN_R1_REPORT_Result>();

                totalRecords = listCNR1Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listCNR1Reports = listCNR1Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listCNR1Reports = listCNR1Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listCNR1Reports = listCNR1Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listCNR1Reports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[]{
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='CNR1DistrictReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+Route.ToString().Trim()+"\",\""+Road.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                        x.TOTAL_NH.ToString(),
                        x.NH_LEN.ToString(),
                        x.TOTAL_SH.ToString(),
                        x.SH_LEN.ToString(), 
                        x.TOTAL_MDR.ToString(),
                        x.MDR_LEN.ToString(), 
                        x.TOTAL_OTHERS.ToString(),
                        x.OTHERS_LEN.ToString(),
                       
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


        public Array CNR1DistrictReportListingDAL(int road, string route, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 2;
                int State = stateCode;
                int District = 0;
                int Block = 0;
                int Road = road;
                string Route = route;
                var listCNR1Reports = dbContext.USP_CN_R1_REPORT(Level, State, District, Block, Road, Route, PMGSYSession.Current.PMGSYScheme).ToList<USP_CN_R1_REPORT_Result>();


                totalRecords = listCNR1Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listCNR1Reports = listCNR1Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listCNR1Reports = listCNR1Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listCNR1Reports = listCNR1Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listCNR1Reports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[] {
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='CNR1BlockReportListing(\"" + State.ToString().Trim() + "\",\"" + x.LOCATION_CODE.ToString().Trim() +"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+Route.ToString().Trim()+"\",\""+Road.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                        x.TOTAL_NH.ToString(),
                        x.NH_LEN.ToString(),
                        x.TOTAL_SH.ToString(),
                        x.SH_LEN.ToString(), 
                        x.TOTAL_MDR.ToString(),
                        x.MDR_LEN.ToString(), 
                        x.TOTAL_OTHERS.ToString(),
                        x.OTHERS_LEN.ToString(),
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


        public Array CNR1BlockReportListingDAL(int road, string route, int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 3;
                int State = stateCode;
                int District = districtCode;
                int Block = 0;
                int Road = road;
                string Route = route;
                var listCNR1Reports = dbContext.USP_CN_R1_REPORT(Level, State, District, Block, Road, Route, PMGSYSession.Current.PMGSYScheme).ToList<USP_CN_R1_REPORT_Result>();

                totalRecords = listCNR1Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listCNR1Reports = listCNR1Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listCNR1Reports = listCNR1Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listCNR1Reports = listCNR1Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listCNR1Reports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[] {
                     "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='CNR1FinalReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+districtCode.ToString()+"\",\""+stateCode.ToString()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+Route.ToString().Trim()+"\",\""+Road.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                     // x.LOCATION_NAME.ToString(),  
                        x.TOTAL_NH.ToString(),
                        x.NH_LEN.ToString(),
                        x.TOTAL_SH.ToString(),
                        x.SH_LEN.ToString(), 
                        x.TOTAL_MDR.ToString(),
                        x.MDR_LEN.ToString(), 
                        x.TOTAL_OTHERS.ToString(),
                        x.OTHERS_LEN.ToString(),
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


        public Array CNR1FinalBlockReportListingDAL(int pop, int road, string route, int blockCode, int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 2;
                int State = stateCode;
                int District = districtCode;
                int Block = blockCode;
                int Pop = pop;
                int Road = road;
                string Route = route;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                var listCNR1Reports = dbContext.USP_CN_REPORT(Level, State, District, Block, Pop, Road, Route, PMGSY).ToList<USP_CN_REPORT_Result>();

                totalRecords = listCNR1Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listCNR1Reports = listCNR1Reports.OrderBy(x => x.PLAN_RD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listCNR1Reports = listCNR1Reports.OrderByDescending(x => x.PLAN_RD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listCNR1Reports = listCNR1Reports.OrderBy(x => x.PLAN_RD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listCNR1Reports.Select(x => new
                {
                    id = x.PLAN_CN_ROAD_NUMBER,
                    cell = new[] {                        
                        x.PLAN_CN_ROAD_NUMBER.ToString(),
                        x.PLAN_RD_NAME == null ? "NA" : x.PLAN_RD_NAME.ToString(),
                        x.MAST_ER_ROAD_CODE.ToString(),
                        x.MAST_ER_ROAD_NAME == null ? "NA" : x.MAST_ER_ROAD_NAME.ToString(),
                        x.PLAN_RD_LENGTH.ToString(),
                        x.RD_FROM == null ? "NA" : x.RD_FROM.ToString(),
                        x.RD_TO == null ? "NA" : x.RD_TO.ToString(),                        
                        x.PLAN_RD_ROUTE == null ? "NA" : x.PLAN_RD_ROUTE.ToString(),                        
                        x.BT_LEN.ToString(),
                        x.TOTAL_HABS.ToString(),                       
                        x.MAST_HAB_TOT_POP.ToString(),
                     
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


        public Array CNR2StateReportListingDAL(int road, string route, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            //List<PMGSY.Models.USP_CN1_REPORT_Result> listCN1Reports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 1;
                int State = 0;
                int District = 0;
                int Block = 0;
                int Road = road;
                //  string Route = "%";
                string Route = route;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 720;
                dbContext.Configuration.LazyLoadingEnabled = false;

                var listCNR2Reports = dbContext.USP_CN_R2_REPORT(Level, State, District, Block, Road, Route, PMGSYSession.Current.PMGSYScheme).ToList<USP_CN_R2_REPORT_Result>();

                totalRecords = listCNR2Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listCNR2Reports = listCNR2Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listCNR2Reports = listCNR2Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listCNR2Reports = listCNR2Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listCNR2Reports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[]{
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='CNR2DistrictReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+Route.ToString().Trim()+"\",\""+Road.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                        x.TOTAL_TR.ToString(),
                        x.TR_LEN.ToString(),
                        x.TOTAL_PTR.ToString(),
                        x.PTR_LEN.ToString(),                      
                        x.TOTAL_LR.ToString(),                        
                        x.LR_LEN.ToString(),
                        x.TOTAL_PLR.ToString(), 
                        x.PLR_LEN.ToString(),
                        x.TCN.ToString(),
                        x.TCN_LEN.ToString(),                        
                        x.TPR.ToString(),
                        x.TPR_LEN.ToString(),              
                        x.BAL_LEN.ToString(),
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


        public Array CNR2DistrictReportListingDAL(int road, string route, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 2;
                int State = stateCode;
                int District = 0;
                int Block = 0;
                int Road = road;
                string Route = route;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 720;
                dbContext.Configuration.LazyLoadingEnabled = false;

                var listCNR2Reports = dbContext.USP_CN_R2_REPORT(Level, State, District, Block, Road, Route, PMGSYSession.Current.PMGSYScheme).ToList<USP_CN_R2_REPORT_Result>();

                totalRecords = listCNR2Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listCNR2Reports = listCNR2Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listCNR2Reports = listCNR2Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listCNR2Reports = listCNR2Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listCNR2Reports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[] {
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='CNR2BlockReportListing(\"" + stateCode.ToString() + "\",\"" + x.LOCATION_CODE.ToString().Trim() +"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+Route.ToString().Trim()+"\",\""+Road.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                         x.TOTAL_TR.ToString(),
                        x.TR_LEN.ToString(),
                        x.TOTAL_PTR.ToString(),
                        x.PTR_LEN.ToString(),                      
                        x.TOTAL_LR.ToString(),                        
                        x.LR_LEN.ToString(),
                        x.TOTAL_PLR.ToString(), 
                        x.PLR_LEN.ToString(),
                        x.TCN.ToString(),
                        x.TCN_LEN.ToString(),                        
                        x.TPR.ToString(),
                        x.TPR_LEN.ToString(),              
                        x.BAL_LEN.ToString(),
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


        public Array CNR2BlockReportListingDAL(int road, string route, int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 3;
                int State = stateCode;
                int District = districtCode;
                int Block = 0;
                int Road = road;
                string Route = route;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 720;
                dbContext.Configuration.LazyLoadingEnabled = false;

                var listCNR2Reports = dbContext.USP_CN_R2_REPORT(Level, State, District, Block, Road, Route, PMGSYSession.Current.PMGSYScheme).ToList<USP_CN_R2_REPORT_Result>();


                totalRecords = listCNR2Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listCNR2Reports = listCNR2Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listCNR2Reports = listCNR2Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listCNR2Reports = listCNR2Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listCNR2Reports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[] {
                      "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='CNR2FinalReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+districtCode.ToString()+"\",\""+stateCode.ToString()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+Route.ToString().Trim()+"\",\""+Road.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                     // x.LOCATION_NAME.ToString(),  
                         x.TOTAL_TR.ToString(),
                        x.TR_LEN.ToString(),
                        x.TOTAL_PTR.ToString(),
                        x.PTR_LEN.ToString(),                      
                        x.TOTAL_LR.ToString(),                        
                        x.LR_LEN.ToString(),
                        x.TOTAL_PLR.ToString(), 
                        x.PLR_LEN.ToString(),
                        x.TCN.ToString(),
                        x.TCN_LEN.ToString(),                        
                        x.TPR.ToString(),
                        x.TPR_LEN.ToString(),              
                        x.BAL_LEN.ToString(),
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

        public Array CNR2FinalBlockReportListingDAL(int road, string route, int blockCode, int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            // List<USP_CN1_FINAL_REPORT_Result> listCN1Reports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                // int Level = 1;
                int State = stateCode;
                int District = districtCode;
                int Block = blockCode;
                //int Pop = 0;
                int Road = road;
                string Route = route;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 720;
                dbContext.Configuration.LazyLoadingEnabled = false;

                var listCNR2Reports = dbContext.USP_CN_R2LVL_REPORT(State, District, Block, Road, Route, PMGSY).ToList<USP_CN_R2LVL_REPORT_Result>();
                //listCN1Reports = dbContext.Database.SqlQuery<USP_CN1_FINAL_REPORT_Result>("EXEC [omms].[USP_CN1_REPORT] @Level,@State,@District,@Block,@Route",
                //    new SqlParameter("@Level", Level),
                //    new SqlParameter("@State", State),
                //    new SqlParameter("@District", District),
                //    new SqlParameter("@Block", Block),
                //    new SqlParameter("@Route", '%')
                //    ).ToList<USP_CN1_FINAL_REPORT_Result>();

                totalRecords = listCNR2Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listCNR2Reports = listCNR2Reports.OrderBy(x => x.PLAN_RD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listCNR2Reports = listCNR2Reports.OrderByDescending(x => x.PLAN_RD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listCNR2Reports = listCNR2Reports.OrderBy(x => x.PLAN_RD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listCNR2Reports.Select(x => new
                {
                    id = x.PLAN_CN_ROAD_NUMBER,
                    cell = new[] {                        
                        x.PLAN_CN_ROAD_NUMBER.ToString(),
                        x.PLAN_RD_NAME == null ? "NA" : x.PLAN_RD_NAME.ToString(),
                        x.MAST_ER_ROAD_CODE.ToString(),
                        x.MAST_ER_ROAD_NAME == null ? "NA" : x.MAST_ER_ROAD_NAME.ToString(),
                        x.PLAN_RD_LENGTH.ToString(),
                        x.RD_FROM == null ? "NA" : x.RD_FROM.ToString(),
                        x.RD_TO == null ? "NA" : x.RD_TO.ToString(),  
                        x.PLAN_RD_ROUTE == null ? "NA" : x.PLAN_RD_ROUTE.ToString(),                                       
                       x.PROP_LEN.ToString(),
                       x.PROP_HABS.ToString(),
                       x.TOTAL_HABS.ToString()                   
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



        public Array CNR3StateReportListingDAL(int road, string route, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            //List<PMGSY.Models.USP_CN1_REPORT_Result> listCN1Reports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 1;
                int State = 0;
                int District = 0;
                int Block = 0;
                int Road = road;
                // string Route = "%";
                string Route = route;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 720;
                dbContext.Configuration.LazyLoadingEnabled = false;

                var listCNR3Reports = dbContext.USP_CN_R2_REPORT(Level, State, District, Block, Road, Route, PMGSYSession.Current.PMGSYScheme).ToList<USP_CN_R2_REPORT_Result>();

                totalRecords = listCNR3Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listCNR3Reports = listCNR3Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listCNR3Reports = listCNR3Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listCNR3Reports = listCNR3Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listCNR3Reports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[]{
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='CNR3DistrictReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+Route.ToString().Trim()+"\",\""+Road.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                        x.TR_HAB.ToString(),
                        x.TR_LEN.ToString(),
                        x.PTR_HAB.ToString(),
                        x.PTR_LEN.ToString(),
                        x.LR_HAB.ToString(),                        
                        x.LR_LEN.ToString(),
                        x.PLR_HAB.ToString(),                        
                        x.PLR_LEN.ToString(),
                        x.TCN_HAB.ToString(),                        
                        x.TCN_LEN.ToString(),
                        x.TPR.ToString(),                        
                        x.TPR_LEN.ToString(),
                        x.BAL_HAB.ToString(),
                        x.BAL_LEN.ToString(),
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


        public Array CNR3DistrictReportListingDAL(int road, string route, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 2;
                int State = stateCode;
                int District = 0;
                int Block = 0;
                int Road = road;
                string Route = route;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 720;
                dbContext.Configuration.LazyLoadingEnabled = false;

                var listCNR3Reports = dbContext.USP_CN_R2_REPORT(Level, State, District, Block, Road, Route, PMGSYSession.Current.PMGSYScheme).ToList<USP_CN_R2_REPORT_Result>();


                totalRecords = listCNR3Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listCNR3Reports = listCNR3Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listCNR3Reports = listCNR3Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listCNR3Reports = listCNR3Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listCNR3Reports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[] {
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='CNR3BlockReportListing(\"" + stateCode.ToString() + "\",\"" + x.LOCATION_CODE.ToString().Trim() +"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+Route.ToString().Trim()+"\",\""+Road.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                         x.TR_HAB.ToString(),
                        x.TR_LEN.ToString(),
                        x.PTR_HAB.ToString(),
                        x.PTR_LEN.ToString(),
                        x.LR_HAB.ToString(),                        
                        x.LR_LEN.ToString(),
                        x.PLR_HAB.ToString(),                        
                        x.PLR_LEN.ToString(),
                        x.TCN_HAB.ToString(),                        
                        x.TCN_LEN.ToString(),
                        x.TPR.ToString(),                        
                        x.TPR_LEN.ToString(),
                        x.BAL_HAB.ToString(),
                        x.BAL_LEN.ToString(),
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


        public Array CNR3BlockReportListingDAL(int road, string route, int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 3;
                int State = stateCode;
                int District = districtCode;
                int Block = 0;
                int Road = road;
                string Route = route;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 720;
                dbContext.Configuration.LazyLoadingEnabled = false;

                var listCNR3Reports = dbContext.USP_CN_R2_REPORT(Level, State, District, Block, Road, Route, PMGSYSession.Current.PMGSYScheme).ToList<USP_CN_R2_REPORT_Result>();


                totalRecords = listCNR3Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listCNR3Reports = listCNR3Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listCNR3Reports = listCNR3Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listCNR3Reports = listCNR3Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listCNR3Reports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[] {
                      "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='CNR3FinalReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+districtCode.ToString()+"\",\""+stateCode.ToString()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+Route.ToString().Trim()+"\",\""+Road.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                     // x.LOCATION_NAME.ToString(),  
                         x.TR_HAB.ToString(),
                        x.TR_LEN.ToString(),
                        x.PTR_HAB.ToString(),
                        x.PTR_LEN.ToString(),
                        x.LR_HAB.ToString(),                        
                        x.LR_LEN.ToString(),
                        x.PLR_HAB.ToString(),                        
                        x.PLR_LEN.ToString(),
                        x.TCN_HAB.ToString(),                        
                        x.TCN_LEN.ToString(),
                        x.TPR.ToString(),                        
                        x.TPR_LEN.ToString(),
                        x.BAL_HAB.ToString(),
                        x.BAL_LEN.ToString(),
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

        public Array CNR3FinalBlockReportListingDAL(int road, string route, int blockCode, int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            // List<USP_CN1_FINAL_REPORT_Result> listCN1Reports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int State = stateCode;
                int District = districtCode;
                int Block = blockCode;
                int Road = road;
                string Route = route;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 720;
                dbContext.Configuration.LazyLoadingEnabled = false;

                var listCNR3Reports = dbContext.USP_CN_R2LVL_REPORT(State, District, Block, Road, Route, PMGSY).ToList<USP_CN_R2LVL_REPORT_Result>();


                totalRecords = listCNR3Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listCNR3Reports = listCNR3Reports.OrderBy(x => x.PLAN_RD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listCNR3Reports = listCNR3Reports.OrderByDescending(x => x.PLAN_RD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listCNR3Reports = listCNR3Reports.OrderBy(x => x.PLAN_RD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listCNR3Reports.Select(x => new
                {
                    id = x.PLAN_CN_ROAD_NUMBER,
                    cell = new[] {                        
                        x.PLAN_CN_ROAD_NUMBER.ToString(),
                        x.PLAN_RD_NAME == null ? "NA" : x.PLAN_RD_NAME.ToString(),
                        x.MAST_ER_ROAD_CODE.ToString(),
                        x.MAST_ER_ROAD_NAME == null ? "NA" : x.MAST_ER_ROAD_NAME.ToString(),
                        x.PLAN_RD_LENGTH.ToString(),
                        x.RD_FROM == null ? "NA" : x.RD_FROM.ToString(),
                        x.RD_TO == null ? "NA" : x.RD_TO.ToString(),   
                        x.PLAN_RD_ROUTE == null ? "NA" : x.PLAN_RD_ROUTE.ToString(),                                       
                        x.PROP_LEN.ToString(),
                        ((x.PLAN_RD_LENGTH)-(x.PROP_LEN)).ToString(), //Bal Length
                        x.TOTAL_HABS.ToString(),
                        x.PROP_HABS.ToString(),
                       ((x.TOTAL_HABS)-(x.PROP_HABS)).ToString(), //Bal Habitation
                    
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



        public Array CNR4StateReportListingDAL(int road, string route, string length, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            //List<PMGSY.Models.USP_CN1_REPORT_Result> listCN1Reports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 1;
                int State = 0;
                int District = 0;
                int Block = 0;
                int Road = road;
                string Route = route;
                string Length = length;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 720;
                dbContext.Configuration.LazyLoadingEnabled = false;

                var listCNR4Reports = dbContext.USP_CN_R4_REPORT(Level, State, District, Block, Road, Route, Length, PMGSYSession.Current.PMGSYScheme).ToList<USP_CN_R4_REPORT_Result>();

                totalRecords = listCNR4Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listCNR4Reports = listCNR4Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listCNR4Reports = listCNR4Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listCNR4Reports = listCNR4Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listCNR4Reports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[]{
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='CNR4DistrictReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+Route.ToString().Trim()+"\",\""+Road.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                        x.TOTAL_TRF.ToString(),
                        x.TOTAL_TRP.ToString(),
                        x.PTR.ToString(),
                        x.TOTAL_LRF.ToString(),
                        x.TOTAL_LRP.ToString(),                        
                        x.PLR.ToString(),                     
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


        public Array CNR4DistrictReportListingDAL(int road, string route, string length, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 2;
                int State = stateCode;
                int District = 0;
                int Block = 0;
                int Road = road;
                string Route = route;
                string Length = length;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 720;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var listCNR4Reports = dbContext.USP_CN_R4_REPORT(Level, State, District, Block, Road, Route, Length, PMGSYSession.Current.PMGSYScheme).ToList<USP_CN_R4_REPORT_Result>();

                totalRecords = listCNR4Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listCNR4Reports = listCNR4Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listCNR4Reports = listCNR4Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listCNR4Reports = listCNR4Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listCNR4Reports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[] {
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='CNR4BlockReportListing(\"" + stateCode.ToString() + "\",\"" + x.LOCATION_CODE.ToString().Trim() +"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+Route.ToString().Trim()+"\",\""+Road.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                        x.TOTAL_TRF.ToString(),
                        x.TOTAL_TRP.ToString(),
                        x.PTR.ToString(),
                        x.TOTAL_LRF.ToString(),
                        x.TOTAL_LRP.ToString(),                        
                        x.PLR.ToString(), 
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


        public Array CNR4BlockReportListingDAL(int road, string route, string length, int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 3;
                int State = stateCode;
                int District = districtCode;
                int Block = 0;
                int Road = road;
                string Route = route;
                string Length = length;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 720;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var listCNR4Reports = dbContext.USP_CN_R4_REPORT(Level, State, District, Block, Road, Route, Length, PMGSYSession.Current.PMGSYScheme).ToList<USP_CN_R4_REPORT_Result>();



                totalRecords = listCNR4Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listCNR4Reports = listCNR4Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listCNR4Reports = listCNR4Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listCNR4Reports = listCNR4Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listCNR4Reports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[] {
                      "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='CNR4FinalReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+districtCode.ToString()+"\",\""+stateCode.ToString()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+Route.ToString().Trim()+"\",\""+Road.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                     // x.LOCATION_NAME.ToString(),  
                       x.TOTAL_TRF.ToString(),
                        x.TOTAL_TRP.ToString(),
                        x.PTR.ToString(),
                        x.TOTAL_LRF.ToString(),
                        x.TOTAL_LRP.ToString(),                        
                        x.PLR.ToString(), 
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

        public Array CNR4FinalBlockReportListingDAL(int road, string route, string length, int blockCode, int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int State = stateCode;
                int District = districtCode;
                int Block = blockCode;
                int Road = road;
                string Route = route;
                string Length = length;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 720;
                dbContext.Configuration.LazyLoadingEnabled = false;

                var listCNR3Reports = dbContext.USP_CN_R4LVL_REPORT(State, District, Block, Road, Route, Length, PMGSY).ToList<USP_CN_R4LVL_REPORT_Result>();

                totalRecords = listCNR3Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listCNR3Reports = listCNR3Reports.OrderBy(x => x.PLAN_RD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listCNR3Reports = listCNR3Reports.OrderByDescending(x => x.PLAN_RD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listCNR3Reports = listCNR3Reports.OrderBy(x => x.PLAN_RD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listCNR3Reports.Select(x => new
                {
                    id = x.PLAN_CN_ROAD_NUMBER,
                    cell = new[] {                        
                        x.PLAN_CN_ROAD_NUMBER.ToString(),
                        x.PLAN_RD_NAME == null ? "NA" : x.PLAN_RD_NAME.ToString(),
                        x.MAST_ER_ROAD_CODE.ToString(),
                        x.MAST_ER_ROAD_NAME == null ? "NA" : x.MAST_ER_ROAD_NAME.ToString(),
                        x.PLAN_RD_LENGTH.ToString(),
                        x.RD_FROM == null ? "NA" : x.RD_FROM.ToString(),
                        x.RD_TO == null ? "NA" : x.RD_TO.ToString(),                     
                        x.PLAN_RD_ROUTE == null ? "NA" : x.PLAN_RD_ROUTE.ToString(),                                       
                        x.PROP_LEN.ToString(),
                        x.PROP_LEN.ToString(), //bal length
                        x.PROP_HABS.ToString(),
                                         
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



        public Array CNPriorityStateReportListingDAL(int priority, string route, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 1;
                int State = 0;
                int District = 0;
                int Block = 0;
                int Priority = priority;
                string Route = route;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 0;
                dbContext.Configuration.LazyLoadingEnabled = false;

                var listCNPriorityReports = dbContext.USP_CN_PRIORITY_REPORT(Level, State, District, Block, Priority, Route, PMGSYSession.Current.PMGSYScheme).ToList<USP_CN_PRIORITY_REPORT_Result>();

                totalRecords = listCNPriorityReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listCNPriorityReports = listCNPriorityReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listCNPriorityReports = listCNPriorityReports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listCNPriorityReports = listCNPriorityReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listCNPriorityReports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[]{
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='CNPriorityDistrictReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+Priority.ToString().Trim()+"\",\""+Route.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                        x.TOTAL_CN.ToString(),
                        x.TOTAL_LEN.ToString()                                           
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


        public Array CNPriorityDistrictReportListingDAL(int stateCode, int priority, string route, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 2;
                int State = stateCode;
                int District = 0;
                int Block = 0;
                int Priority = priority;
                string Route = route;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 0;
                dbContext.Configuration.LazyLoadingEnabled = false;

                var listCNPriorityReports = dbContext.USP_CN_PRIORITY_REPORT(Level, State, District, Block, Priority, Route, PMGSYSession.Current.PMGSYScheme).ToList<USP_CN_PRIORITY_REPORT_Result>();

                totalRecords = listCNPriorityReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listCNPriorityReports = listCNPriorityReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listCNPriorityReports = listCNPriorityReports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listCNPriorityReports = listCNPriorityReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listCNPriorityReports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[] {
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='CNPriorityBlockReportListing(\"" + stateCode.ToString() + "\",\"" + x.LOCATION_CODE.ToString().Trim() +"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+Priority.ToString().Trim()+"\",\""+Route.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                          x.TOTAL_CN.ToString(),
                         x.TOTAL_LEN.ToString()   
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


        public Array CNPriorityBlockReportListingDAL(int stateCode, int districtCode, int priority, string route, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 3;
                int State = stateCode;
                int District = districtCode;
                int Block = 0;
                int Priority = priority;
                string Route = route;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 0;
                dbContext.Configuration.LazyLoadingEnabled = false;

                var listCNPriorityReports = dbContext.USP_CN_PRIORITY_REPORT(Level, State, District, Block, Priority, Route, PMGSYSession.Current.PMGSYScheme).ToList<USP_CN_PRIORITY_REPORT_Result>();

                totalRecords = listCNPriorityReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listCNPriorityReports = listCNPriorityReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listCNPriorityReports = listCNPriorityReports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listCNPriorityReports = listCNPriorityReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listCNPriorityReports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[] {
                      "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='CNPriorityFinalReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+districtCode.ToString()+"\",\""+stateCode.ToString()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+Priority.ToString().Trim()+"\",\""+Route.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                     // x.LOCATION_NAME.ToString(),  
                        x.TOTAL_CN.ToString(),
                         x.TOTAL_LEN.ToString()  
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

        public Array CNPriorityFinalBlockReportListingDAL(int blockCode, int districtCode, int stateCode, int priority, string route, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            // List<USP_CN1_FINAL_REPORT_Result> listCN1Reports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                // int Level = 1;
                int State = stateCode;
                int District = districtCode;
                int Block = blockCode;

                int Priority = priority;
                string Route = route;

                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 0;
                dbContext.Configuration.LazyLoadingEnabled = false;

                var listCNR3Reports = dbContext.USP_CN_PRIORITY4_REPORT(State, District, Block, Priority, Route, PMGSY).ToList<USP_CN_PRIORITY4_REPORT_Result>();
                //listCN1Reports = dbContext.Database.SqlQuery<USP_CN1_FINAL_REPORT_Result>("EXEC [omms].[USP_CN1_REPORT] @Level,@State,@District,@Block,@Route",
                //    new SqlParameter("@Level", Level),
                //    new SqlParameter("@State", State),
                //    new SqlParameter("@District", District),
                //    new SqlParameter("@Block", Block),
                //    new SqlParameter("@Route", '%')
                //    ).ToList<USP_CN1_FINAL_REPORT_Result>();

                totalRecords = listCNR3Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listCNR3Reports = listCNR3Reports.OrderBy(x => x.PLAN_RD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listCNR3Reports = listCNR3Reports.OrderByDescending(x => x.PLAN_RD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listCNR3Reports = listCNR3Reports.OrderBy(x => x.PLAN_RD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listCNR3Reports.Select(x => new
                {
                    id = x.PLAN_CN_ROAD_NUMBER,
                    cell = new[] {                        
                        x.PLAN_CN_ROAD_NUMBER.ToString(),
                        x.PLAN_RD_NAME == null ? "NA" : x.PLAN_RD_NAME.ToString(),
                        x.PLAN_RD_ROUTE == null ? "NA" : x.PLAN_RD_ROUTE.ToString(),
                        x.PLAN_RD_LENGTH == null ? "0" : x.PLAN_RD_LENGTH.ToString(),                       
                        x.MAST_HAB_NAME.ToString(),
                        x.MAST_HAB_CONNECTED.ToString(),
                        x.MAST_HAB_TOT_POP.ToString()                
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

        public List<SelectListItem> PopulateRoadCategoryList()
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<SelectListItem> lstRoadCategory = new List<SelectListItem>();

                var list = (from rd in dbContext.MASTER_ROAD_CATEGORY

                            select new
                            {
                                MAST_ROAD_CAT_CODE = rd.MAST_ROAD_CAT_CODE,
                                MAST_ROAD_CAT_NAME = rd.MAST_ROAD_CAT_NAME + " (" + rd.MAST_ROAD_SHORT_DESC + ")"
                            }).Distinct().OrderBy(m => m.MAST_ROAD_CAT_NAME).ToList();


                lstRoadCategory = new SelectList(list.ToList(), "MAST_ROAD_CAT_CODE", "MAST_ROAD_CAT_NAME").ToList();
                lstRoadCategory.Insert(0, new SelectListItem { Text = "All", Value = "0" });
                return lstRoadCategory;

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

    }

    public interface ICoreNetworkReportsDAL
    {
        //CN1
        Array CN1ReportListingDAL(string route, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CN1DistrictReportListingDAL(string route, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CN1BlockReportListingDAL(string route, int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CN1FinalBlockReportListingDAL(int pop, int road, string route, int blockCode, int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords);

        //CN2
        Array CN2StateReportListingDAL(int pop, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CN2DistrictReportListingDAL(int pop, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CN2BlockReportListingDAL(int pop, int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CN2FinalListingDAL(int pop, int road, string route, int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);
        //CN3
        Array CN3StateReportListingDAL(int roadcategory, string route, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CN3DistrictReportListingDAL(int roadcategory, string route, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CN3BlockReportListingDAL(int roadcategory, string route, int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CN3FinalListingDAL(int pop, int roadcategory, string route, int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);

        //CN4
        Array CN4StateReportListingDAL(int roadcategory, string route, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CN4DistrictReportListingDAL(int roadcategory, string route, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CN4BlockReportListingDAL(int roadcategory, string route, int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CN4FinalListingDAL(int pop, int roadcategory, string route, int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);

        //CN5
        Array CN5StateReportListingDAL(int roadcategory, string route, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CN5DistrictReportListingDAL(int roadcategory, string route, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CN5BlockReportListingDAL(int roadcategory, string route, int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CN5FinalListingDAL(int pop, int roadcategory, string route, int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);

        //CN6
        Array CN6StateReportListingDAL(string route, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CN6DistrictReportListingDAL(string route, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CN6BlockReportListingDAL(string route, int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CN6FinalListingDAL(string route, int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);

        //CNCPL
        Array CNCPLStateListingDAL(string route, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CNCPLDistrictListingDAL(string route, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CNCPLBlockListingDAL(string route, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CNCPLFinalListingDAL(string route, int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);

        //HWCN
        Array HWCNStateListingDAL(string route, int page, int rows, string sidx, string sord, out int totalRecords);
        Array HWCNFinalListingDAL(string route, int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array HWCNBlockListingDAL(string route, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array HWCNDistrictListingDAL(string route, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);

        //RWCN
        Array RWCNBlockListingDAL(string route, int road, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array RWCNDistrictListingDAL(string route, int road, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array RWCNStateListingDAL(string route, int road, int page, int rows, string sidx, string sord, out int totalRecords);
        Array RWCNFinalListingDAL(int pop, string route, int road, int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);

        //PCI
        Array PCIAbstractStateListingDAL(string route, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PCIAbstractDistrictListingDAL(string route, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PCIAbstractBlockListingDAL(string route, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PCIAbstractFinalListingDAL(string route, int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);

        //CUCPL
        Array CUCPLStateListingDAL(string route, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CUCPLDistrictListingDAL(string route, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CUCPLBlockListingDAL(string route, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CUCPLFinalListingDAL(string route, int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);

        //CNR1
        Array CNR1StateReportListingDAL(int road, string route, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CNR1DistrictReportListingDAL(int road, string route, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CNR1BlockReportListingDAL(int road, string route, int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CNR1FinalBlockReportListingDAL(int pop, int road, string route, int blockCode, int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords);


        //CNR2
        Array CNR2StateReportListingDAL(int road, string route, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CNR2DistrictReportListingDAL(int road, string route, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CNR2BlockReportListingDAL(int road, string route, int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CNR2FinalBlockReportListingDAL(int road, string route, int blockCode, int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords);


        //CNR3
        Array CNR3StateReportListingDAL(int road, string route, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CNR3DistrictReportListingDAL(int road, string route, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CNR3BlockReportListingDAL(int road, string route, int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CNR3FinalBlockReportListingDAL(int road, string route, int blockCode, int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords);

        //CNR4
        Array CNR4StateReportListingDAL(int road, string route, string length, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CNR4DistrictReportListingDAL(int road, string route, string length, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CNR4BlockReportListingDAL(int road, string route, string length, int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CNR4FinalBlockReportListingDAL(int road, string route, string length, int blockCode, int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords);
        //CNPriority
        Array CNPriorityStateReportListingDAL(int priority, string route, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CNPriorityDistrictReportListingDAL(int stateCode, int priority, string route, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CNPriorityBlockReportListingDAL(int stateCode, int districtCode, int priority, string route, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CNPriorityFinalBlockReportListingDAL(int blockCode, int districtCode, int stateCode, int priority, string route, int page, int rows, string sidx, string sord, out int totalRecords);

    }
}