using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.Models;
using System.Data.SqlClient;
using PMGSY.Models.ExistingRoadsReports;
using System.Data.Entity.Infrastructure;
using PMGSY.Extensions;
using System.Data.Entity;
using System.Web.Mvc;

namespace PMGSY.DAL.ExistingRoadsReports
{
    public class ExistingRoadsReportsDAL : IExistingRoadsReportsDAL
    {
        PMGSY.Models.PMGSYEntities dbContext;
        public Array ERR1StateReportListingDAL(int page, int rows, string sidx, string sord, out int totalRecords)
        {

            //  List<USP_CN2_REPORT_Result> listERRReport;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 1;
                int State = 0;
                int District = 0;
                int Block = 0;
                int PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var listERR1Reports = dbContext.USP_DRRP_ROAD_REPORT(Level, State, District, Block, PMGSYSession.Current.PMGSYScheme).ToList<USP_DRRP_ROAD_REPORT_Result>();

                totalRecords = listERR1Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listERR1Reports = listERR1Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listERR1Reports = listERR1Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listERR1Reports = listERR1Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listERR1Reports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[]{
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='ERR1DistrictReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",                       
                        x.TOTAL_NH.ToString(),
                        x.TOTAL_NH_LEN.ToString(),
                        x.TOTAL_SH.ToString(),
                        x.TOTAL_SH_LEN.ToString(),
                        x.TOTAL_MDR.ToString(),
                        x.TOTAL_MDR_LEN.ToString(),
                        x.TOTAL_RR.ToString(), 
                        x.TOTAL_RR_LEN.ToString(),
                        x.TOTAL_OTHER.ToString(),
                        x.TOTAL_OTHER_LEN.ToString(),
                        x.TOTAL_ROADS.ToString(), 
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


        public Array ERR1DistrictReportListingDAL(int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            // List<USP_CN2_REPORT_Result> listERR1Reports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 2;
                int State = stateCode;
                int District = 0;
                int Block = 0;
                int PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var listERR1Reports = dbContext.USP_DRRP_ROAD_REPORT(Level, State, District, Block, PMGSYSession.Current.PMGSYScheme).ToList<USP_DRRP_ROAD_REPORT_Result>();


                totalRecords = listERR1Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listERR1Reports = listERR1Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listERR1Reports = listERR1Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listERR1Reports = listERR1Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listERR1Reports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[] {
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='ERR1BlockReportListing(\""+stateCode.ToString().Trim()+"\",\""+x.LOCATION_CODE.ToString()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                        x.TOTAL_NH.ToString(),
                        x.TOTAL_NH_LEN.ToString(),
                        x.TOTAL_SH.ToString(),
                        x.TOTAL_SH_LEN.ToString(),
                        x.TOTAL_MDR.ToString(),
                        x.TOTAL_MDR_LEN.ToString(),
                        x.TOTAL_RR.ToString(), 
                        x.TOTAL_RR_LEN.ToString(),
                        x.TOTAL_OTHER.ToString(),
                        x.TOTAL_OTHER_LEN.ToString(),
                        x.TOTAL_ROADS.ToString(), 
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


        public Array ERR1BlockReportListingDAL(int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            // List<USP_CN2_REPORT_Result> listERR1Reports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 3;
                int State = stateCode;
                int District = districtCode;
                int Block = 0;
                int PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var listERR1Reports = dbContext.USP_DRRP_ROAD_REPORT(Level, State, District, Block, PMGSYSession.Current.PMGSYScheme).ToList<USP_DRRP_ROAD_REPORT_Result>();



                totalRecords = listERR1Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listERR1Reports = listERR1Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listERR1Reports = listERR1Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listERR1Reports = listERR1Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listERR1Reports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[] {
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='ERR1FinalReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+districtCode.ToString()+"\",\""+stateCode.ToString()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                        //x.LOCATION_NAME.ToString(),
                        x.TOTAL_NH.ToString(),
                        x.TOTAL_NH_LEN.ToString(),
                        x.TOTAL_SH.ToString(),
                        x.TOTAL_SH_LEN.ToString(),
                        x.TOTAL_MDR.ToString(),
                        x.TOTAL_MDR_LEN.ToString(),
                        x.TOTAL_RR.ToString(), 
                        x.TOTAL_RR_LEN.ToString(),
                        x.TOTAL_OTHER.ToString(),
                        x.TOTAL_OTHER_LEN.ToString(),
                        x.TOTAL_ROADS.ToString(), 
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


        public Array ERR1FinalListingDAL(int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            // List<USP_CN2_FINAL_REPORT_Result> listERRReport;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                // int Level = 1;
                int State = stateCode;
                int District = districtCode;
                int Block = blockCode;
                string Type = "%";
                int SType = 0;
                int TType = 0;

                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;

                var listERR1Reports = dbContext.USP_DRRP_REPORT(State, District, Block, Type, SType, TType, PMGSY).ToList<USP_DRRP_REPORT_Result>();



                totalRecords = listERR1Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listERR1Reports = listERR1Reports.OrderBy(x => x.MAST_ER_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listERR1Reports = listERR1Reports.OrderByDescending(x => x.MAST_ER_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listERR1Reports = listERR1Reports.OrderBy(x => x.MAST_ER_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listERR1Reports.Select(x => new
                {
                    id = x.MAST_ER_ROAD_NUMBER,
                    cell = new[] {
                       x.MAST_ER_ROAD_NUMBER == null ? "NULL" : x.MAST_ER_ROAD_NUMBER.ToString() ,
                       x.MAST_ER_ROAD_NAME == null ? "NULL" : x.MAST_ER_ROAD_NAME.ToString(),
                       x.MAST_ROAD_SHORT_DESC.ToString(),
                       x.MAST_ER_ROAD_TYPE == null ? "NULL" : x.MAST_ER_ROAD_TYPE.ToString(),
                       x.ROAD_LENGTH.ToString(),
                       x.MAST_CONS_YEAR == null ? "-" : x.MAST_CONS_YEAR.ToString(),
                       x.MAST_CORE_NETWORK == null ? "NULL" :  x.MAST_CORE_NETWORK.ToString(),
                       x.MAST_HAB_STATUS==null?"":x.MAST_HAB_STATUS.ToString(),
                       x.MAST_HAB_NAME==null?"":x.MAST_HAB_NAME.ToString(),
                       x.MAST_HAB_TOT_POP.ToString(),
                       x.MAST_SOIL_TYPE_NAME == null ? "NULL" : x.MAST_SOIL_TYPE_NAME.ToString(),
                       x.MAST_TERRAIN_TYPE_NAME == null ? "NULL" : x.MAST_TERRAIN_TYPE_NAME.ToString()                      
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



        public Array ERR2StateReportListingDAL(int page, int rows, string sidx, string sord, out int totalRecords)
        {

            //  List<USP_CN2_REPORT_Result> listERRReport;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 1;
                int State = 0;
                int District = 0;
                int Block = 0;
                int PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var listERR2Reports = dbContext.USP_DRRP_R2_REPORT(Level, State, District, Block, PMGSYSession.Current.PMGSYScheme).ToList<USP_DRRP_R2_REPORT_Result>();

                totalRecords = listERR2Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listERR2Reports = listERR2Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listERR2Reports = listERR2Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listERR2Reports = listERR2Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listERR2Reports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[]{
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='ERR2DistrictReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",                       
                        x.TOTAL_NH.ToString(),
                        x.TOTAL_NH_LEN.ToString(),
                        x.TOTAL_CNH.ToString(),
                        x.TOTAL_CNH_LEN.ToString(),
                        x.TOTAL_SH.ToString(),
                        x.TOTAL_SH_LEN.ToString(),
                         x.TOTAL_CSH.ToString(),
                        x.TOTAL_CSH_LEN.ToString(),
                        x.TOTAL_MDR.ToString(),
                        x.TOTAL_MDR_LEN.ToString(),
                        x.TOTAL_CMDR.ToString(),
                        x.TOTAL_CMDR_LEN.ToString(),
                        x.TOTAL_RR.ToString(), 
                        x.TOTAL_RR_LEN.ToString(),
                        x.TOTAL_CRR.ToString(), 
                        x.TOTAL_CRR_LEN.ToString(),
                        x.TOTAL_OTHER.ToString(),
                        x.TOTAL_OTHER_LEN.ToString(),
                        x.TOTAL_COTHER.ToString(), 
                        x.TOTAL_COTHER_LEN.ToString(), 
                        x.TOTAL_ROADS.ToString(),
                        x.TOTAL_ROADS_LEN.ToString(),
                        x.TOTAL_CROADS.ToString(), 
                        x.TOTAL_CROADS_LEN.ToString() 
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


        public Array ERR2DistrictReportListingDAL(int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            // List<USP_CN2_REPORT_Result> listERR1Reports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 2;
                int State = stateCode;
                int District = 0;
                int Block = 0;
                int PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var listERR2Reports = dbContext.USP_DRRP_R2_REPORT(Level, State, District, Block, PMGSYSession.Current.PMGSYScheme).ToList<USP_DRRP_R2_REPORT_Result>();


                totalRecords = listERR2Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listERR2Reports = listERR2Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listERR2Reports = listERR2Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listERR2Reports = listERR2Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listERR2Reports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[] {
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='ERR2BlockReportListing(\""+stateCode.ToString().Trim()+"\",\""+x.LOCATION_CODE.ToString()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                        x.TOTAL_NH.ToString(),
                        x.TOTAL_NH_LEN.ToString(),
                        x.TOTAL_CNH.ToString(),
                        x.TOTAL_CNH_LEN.ToString(),
                        x.TOTAL_SH.ToString(),
                        x.TOTAL_SH_LEN.ToString(),
                         x.TOTAL_CSH.ToString(),
                        x.TOTAL_CSH_LEN.ToString(),
                        x.TOTAL_MDR.ToString(),
                        x.TOTAL_MDR_LEN.ToString(),
                        x.TOTAL_CMDR.ToString(),
                        x.TOTAL_CMDR_LEN.ToString(),
                        x.TOTAL_RR.ToString(), 
                        x.TOTAL_RR_LEN.ToString(),
                        x.TOTAL_CRR.ToString(), 
                        x.TOTAL_CRR_LEN.ToString(),
                        x.TOTAL_OTHER.ToString(),
                        x.TOTAL_OTHER_LEN.ToString(),
                        x.TOTAL_COTHER.ToString(), 
                        x.TOTAL_COTHER_LEN.ToString(), 
                        x.TOTAL_ROADS.ToString(),
                        x.TOTAL_ROADS_LEN.ToString(),
                        x.TOTAL_CROADS.ToString(), 
                        x.TOTAL_CROADS_LEN.ToString() 
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


        public Array ERR2BlockReportListingDAL(int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            // List<USP_CN2_REPORT_Result> listERR1Reports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 3;
                int State = stateCode;
                int District = districtCode;
                int Block = 0;
                int PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var listERR2Reports = dbContext.USP_DRRP_R2_REPORT(Level, State, District, Block, PMGSYSession.Current.PMGSYScheme).ToList<USP_DRRP_R2_REPORT_Result>();

                totalRecords = listERR2Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listERR2Reports = listERR2Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listERR2Reports = listERR2Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listERR2Reports = listERR2Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listERR2Reports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[] {
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='ERR2FinalReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+districtCode.ToString()+"\",\""+stateCode.ToString()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                        //x.LOCATION_NAME.ToString(),
                        x.TOTAL_NH.ToString(),
                        x.TOTAL_NH_LEN.ToString(),
                        x.TOTAL_CNH.ToString(),
                        x.TOTAL_CNH_LEN.ToString(),
                        x.TOTAL_SH.ToString(),
                        x.TOTAL_SH_LEN.ToString(),
                        x.TOTAL_CSH.ToString(),
                        x.TOTAL_CSH_LEN.ToString(),
                        x.TOTAL_MDR.ToString(),
                        x.TOTAL_MDR_LEN.ToString(),
                        x.TOTAL_CMDR.ToString(),
                        x.TOTAL_CMDR_LEN.ToString(),
                        x.TOTAL_RR.ToString(), 
                        x.TOTAL_RR_LEN.ToString(),
                        x.TOTAL_CRR.ToString(), 
                        x.TOTAL_CRR_LEN.ToString(),
                        x.TOTAL_OTHER.ToString(),
                        x.TOTAL_OTHER_LEN.ToString(),
                        x.TOTAL_COTHER.ToString(), 
                        x.TOTAL_COTHER_LEN.ToString(), 
                        x.TOTAL_ROADS.ToString(),
                        x.TOTAL_ROADS_LEN.ToString(),
                        x.TOTAL_CROADS.ToString(), 
                        x.TOTAL_CROADS_LEN.ToString() 
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


        public Array ERR2FinalListingDAL(int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            // List<USP_CN2_FINAL_REPORT_Result> listERRReport;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                // int Level = 1;
                int State = stateCode;
                int District = districtCode;
                int Block = blockCode;
                string Type = "%";
                int SType = 0;
                int TType = 0;

                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;

                var listERR2Reports = dbContext.USP_DRRP_REPORT(State, District, Block, Type, SType, TType, PMGSY).ToList<USP_DRRP_REPORT_Result>();



                totalRecords = listERR2Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listERR2Reports = listERR2Reports.OrderBy(x => x.MAST_ER_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listERR2Reports = listERR2Reports.OrderByDescending(x => x.MAST_ER_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listERR2Reports = listERR2Reports.OrderBy(x => x.MAST_ER_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listERR2Reports.Select(x => new
                {
                    id = x.MAST_ER_ROAD_NUMBER,
                    cell = new[] {
                       x.MAST_ER_ROAD_NUMBER == null ? "NULL" : x.MAST_ER_ROAD_NUMBER.ToString() ,
                       x.MAST_ER_ROAD_NAME == null ? "NULL" : x.MAST_ER_ROAD_NAME.ToString(),
                       x.MAST_ROAD_SHORT_DESC.ToString(),
                       x.MAST_ER_ROAD_TYPE == null ? "NULL" : x.MAST_ER_ROAD_TYPE.ToString(),
                       x.ROAD_LENGTH.ToString(),
                       x.MAST_CONS_YEAR == null ? "-" : x.MAST_CONS_YEAR.ToString(),
                       x.MAST_CORE_NETWORK == null ? "NULL" :  x.MAST_CORE_NETWORK.ToString(),
                       x.MAST_HAB_STATUS==null?"":x.MAST_HAB_STATUS.ToString(),
                       x.MAST_HAB_NAME==null?"":x.MAST_HAB_NAME.ToString(),                   
                       x.MAST_HAB_TOT_POP.ToString(),
                       x.MAST_SOIL_TYPE_NAME == null ? "NULL" : x.MAST_SOIL_TYPE_NAME.ToString(),
                       x.MAST_TERRAIN_TYPE_NAME == null ? "NULL" : x.MAST_TERRAIN_TYPE_NAME.ToString()                      
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



        public Array ERR3StateReportListingDAL(int page, int rows, string sidx, string sord, out int totalRecords)
        {

            //  List<USP_CN2_REPORT_Result> listERRReport;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 1;
                int State = 0;
                int District = 0;
                int Block = 0;
                int PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var listERR3Reports = dbContext.USP_DRRP_R3_REPORT(Level, State, District, Block, PMGSYSession.Current.PMGSYScheme).ToList<USP_DRRP_R3_REPORT_Result>();

                totalRecords = listERR3Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listERR3Reports = listERR3Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listERR3Reports = listERR3Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listERR3Reports = listERR3Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listERR3Reports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[]{
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='ERR3DistrictReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",                       
                        x.POP1000.ToString(),
                        x.POP999.ToString(),
                        x.POP499.ToString(),
                        x.POP250.ToString(),
                        (x.POP1000+x.POP999+x.POP499+x.POP250).ToString(), // Total
                        x.CN_POP1000.ToString(),
                        x.CN_POP999.ToString(),
                        x.CN_POP499.ToString(), 
                        x.CN_POP250.ToString(), 
                       (x.CN_POP1000+x.CN_POP999+x.CN_POP499+x.CN_POP250).ToString(), // Total                   
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


        public Array ERR3DistrictReportListingDAL(int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            // List<USP_CN2_REPORT_Result> listERR1Reports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 2;
                int State = stateCode;
                int District = 0;
                int Block = 0;
                int PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var listERR3Reports = dbContext.USP_DRRP_R3_REPORT(Level, State, District, Block, PMGSYSession.Current.PMGSYScheme).ToList<USP_DRRP_R3_REPORT_Result>();


                totalRecords = listERR3Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listERR3Reports = listERR3Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listERR3Reports = listERR3Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listERR3Reports = listERR3Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listERR3Reports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[] {
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='ERR3BlockReportListing(\""+stateCode.ToString().Trim()+"\",\""+x.LOCATION_CODE.ToString()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                        x.POP1000.ToString(),
                        x.POP999.ToString(),
                        x.POP499.ToString(),
                        x.POP250.ToString(),
                        (x.POP1000+x.POP999+x.POP499+x.POP250).ToString(), // Total
                        x.CN_POP1000.ToString(),
                        x.CN_POP999.ToString(),
                        x.CN_POP499.ToString(), 
                        x.CN_POP250.ToString(), 
                       (x.CN_POP1000+x.CN_POP999+x.CN_POP499+x.CN_POP250).ToString(), // Total                   
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


        public Array ERR3BlockReportListingDAL(int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            // List<USP_CN2_REPORT_Result> listERR1Reports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 3;
                int State = stateCode;
                int District = districtCode;
                int Block = 0;
                int PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;

                var listERR3Reports = dbContext.USP_DRRP_R3_REPORT(Level, State, District, Block, PMGSYSession.Current.PMGSYScheme).ToList<USP_DRRP_R3_REPORT_Result>();

                totalRecords = listERR3Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listERR3Reports = listERR3Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listERR3Reports = listERR3Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listERR3Reports = listERR3Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listERR3Reports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[] {
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='ERR3FinalReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+districtCode.ToString()+"\",\""+stateCode.ToString()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                        //x.LOCATION_NAME.ToString(),
                        x.POP1000.ToString(),
                        x.POP999.ToString(),
                        x.POP499.ToString(),
                        x.POP250.ToString(),
                        (x.POP1000+x.POP999+x.POP499+x.POP250).ToString(), // Total
                        x.CN_POP1000.ToString(),
                        x.CN_POP999.ToString(),
                        x.CN_POP499.ToString(), 
                        x.CN_POP250.ToString(), 
                        (x.CN_POP1000+x.CN_POP999+x.CN_POP499+x.CN_POP250).ToString() // Total                   
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


        public Array ERR3FinalListingDAL(int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            // List<USP_CN2_FINAL_REPORT_Result> listERRReport;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                // int Level = 1;
                int State = stateCode;
                int District = districtCode;
                int Block = blockCode;
                string Type = "%";
                int SType = 0;
                int TType = 0;

                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;

                var listERRReport = dbContext.USP_DRRP_REPORT(State, District, Block, Type, SType, TType, PMGSY).ToList<USP_DRRP_REPORT_Result>();



                totalRecords = listERRReport.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listERRReport = listERRReport.OrderBy(x => x.MAST_ER_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listERRReport = listERRReport.OrderByDescending(x => x.MAST_ER_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listERRReport = listERRReport.OrderBy(x => x.MAST_ER_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listERRReport.Select(x => new
                {
                    id = x.MAST_ER_ROAD_NUMBER,
                    cell = new[] {
                       x.MAST_ER_ROAD_NUMBER == null ? "NULL" : x.MAST_ER_ROAD_NUMBER.ToString() ,
                       x.MAST_ER_ROAD_NAME == null ? "NULL" : x.MAST_ER_ROAD_NAME.ToString(),
                       x.MAST_ROAD_SHORT_DESC.ToString(),
                       x.MAST_ER_ROAD_TYPE == null ? "NULL" : x.MAST_ER_ROAD_TYPE.ToString(),
                       x.ROAD_LENGTH.ToString(),
                       x.MAST_HAB_STATUS==null?"":x.MAST_HAB_STATUS.ToString(),
                       x.MAST_HAB_NAME==null?"":x.MAST_HAB_NAME.ToString(),
                        x.MAST_HAB_TOT_POP.ToString(),
                       x.MAST_CORE_NETWORK == null ? "NULL" : x.MAST_CORE_NETWORK.ToString()
                                         
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


        public Array ERR4StateReportListingDAL(int page, int rows, string sidx, string sord, out int totalRecords)
        {


            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 1;
                int State = 0;
                int District = 0;
                int Block = 0;
                int PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var listERR4Reports = dbContext.USP_DRRP_R4_REPORT(Level, State, District, Block, PMGSYSession.Current.PMGSYScheme).ToList<USP_DRRP_R4_REPORT_Result>();

                totalRecords = listERR4Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listERR4Reports = listERR4Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listERR4Reports = listERR4Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listERR4Reports = listERR4Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listERR4Reports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[]{
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='ERR4DistrictReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",                       
                        x.BT_GOOD.ToString(),
                        x.BT_FAIR.ToString(),
                        x.BT_BAD.ToString(),
                        x.BT_TOTAL.ToString(),
                        x.WBM_GOOD.ToString(), 
                        x.WBM_FAIR.ToString(),
                        x.WBM_BAD.ToString(), 
                        x.WBM_TOTAL.ToString(), 
                        x.GRAVEL_GOOD.ToString(),
                        x.GRAVEL_FAIR.ToString(),
                        x.GRAVEL_BAD.ToString(),
                        x.GRAVEL_TOTAL.ToString(),
                        x.TRACK_GOOD.ToString(),                      
                        x.TRACK_FAIR.ToString(),
                        x.TRACK_BAD.ToString(),
                        x.TRACK_TOTAL.ToString(),
                        x.OTHER_GOOD.ToString(),
                        x.OTHER_FAIR.ToString(),
                        x.OTHER_BAD.ToString(),
                        x.OTHER_TOTAL.ToString(),
                        x.TOTAL_GOOD.ToString(),
                        x.TOTAL_FAIR.ToString(),
                        x.TOTAL_BAD.ToString(),
                        x.GRAND_TOTAL.ToString()
                      
                   
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


        public Array ERR4DistrictReportListingDAL(int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            // List<USP_CN2_REPORT_Result> listERR1Reports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 2;
                int State = stateCode;
                int District = 0;
                int Block = 0;
                int PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var listERR4Reports = dbContext.USP_DRRP_R4_REPORT(Level, State, District, Block, PMGSYSession.Current.PMGSYScheme).ToList<USP_DRRP_R4_REPORT_Result>();


                totalRecords = listERR4Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listERR4Reports = listERR4Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listERR4Reports = listERR4Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listERR4Reports = listERR4Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listERR4Reports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[] {
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='ERR4BlockReportListing(\""+stateCode.ToString().Trim()+"\",\""+x.LOCATION_CODE.ToString()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                         x.BT_GOOD.ToString(),
                        x.BT_FAIR.ToString(),
                        x.BT_BAD.ToString(),
                        x.BT_TOTAL.ToString(),
                        x.WBM_GOOD.ToString(), 
                        x.WBM_FAIR.ToString(),
                        x.WBM_BAD.ToString(), 
                        x.WBM_TOTAL.ToString(), 
                        x.GRAVEL_GOOD.ToString(),
                        x.GRAVEL_FAIR.ToString(),
                        x.GRAVEL_BAD.ToString(),
                        x.GRAVEL_TOTAL.ToString(),
                        x.TRACK_GOOD.ToString(),                      
                        x.TRACK_FAIR.ToString(),
                        x.TRACK_BAD.ToString(),
                        x.TRACK_TOTAL.ToString(),
                        x.OTHER_GOOD.ToString(),
                        x.OTHER_FAIR.ToString(),
                        x.OTHER_BAD.ToString(),
                        x.OTHER_TOTAL.ToString(),
                        x.TOTAL_GOOD.ToString(),
                        x.TOTAL_FAIR.ToString(),
                        x.TOTAL_BAD.ToString(),
                        x.GRAND_TOTAL.ToString()
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


        public Array ERR4BlockReportListingDAL(int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            // List<USP_CN2_REPORT_Result> listERR1Reports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 3;
                int State = stateCode;
                int District = districtCode;
                int Block = 0;
                int PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;

                var listERR4Reports = dbContext.USP_DRRP_R4_REPORT(Level, State, District, Block, PMGSYSession.Current.PMGSYScheme).ToList<USP_DRRP_R4_REPORT_Result>();

                totalRecords = listERR4Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listERR4Reports = listERR4Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listERR4Reports = listERR4Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listERR4Reports = listERR4Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listERR4Reports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[] {
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='ERR4FinalReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+districtCode.ToString()+"\",\""+stateCode.ToString()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                        //x.LOCATION_NAME.ToString(),
                         x.BT_GOOD.ToString(),
                        x.BT_FAIR.ToString(),
                        x.BT_BAD.ToString(),
                        x.BT_TOTAL.ToString(),
                        x.WBM_GOOD.ToString(), 
                        x.WBM_FAIR.ToString(),
                        x.WBM_BAD.ToString(), 
                        x.WBM_TOTAL.ToString(), 
                        x.GRAVEL_GOOD.ToString(),
                        x.GRAVEL_FAIR.ToString(),
                        x.GRAVEL_BAD.ToString(),
                        x.GRAVEL_TOTAL.ToString(),
                        x.TRACK_GOOD.ToString(),                      
                        x.TRACK_FAIR.ToString(),
                        x.TRACK_BAD.ToString(),
                        x.TRACK_TOTAL.ToString(),
                        x.OTHER_GOOD.ToString(),
                        x.OTHER_FAIR.ToString(),
                        x.OTHER_BAD.ToString(),
                        x.OTHER_TOTAL.ToString(),
                        x.TOTAL_GOOD.ToString(),
                        x.TOTAL_FAIR.ToString(),
                        x.TOTAL_BAD.ToString(),
                        x.GRAND_TOTAL.ToString()
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


        public Array ERR4FinalListingDAL(int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            // List<USP_CN2_FINAL_REPORT_Result> listERRReport;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                // int Level = 1;
                int State = stateCode;
                int District = districtCode;
                int Block = blockCode;
                string Type = "%";
                int SType = 0;
                int TType = 0;

                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;

                 var listERRReport = dbContext.USP_DRRP_R4LVL_REPORT(State, District, Block, PMGSY).ToList<USP_DRRP_R4LVL_REPORT_Result>();
               //var listERRReport = dbContext.USP_DRRP_REPORT(State, District, Block,Type,SType,TType, PMGSY).ToList<USP_DRRP_REPORT_Result>();
            
                totalRecords = listERRReport.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listERRReport = listERRReport.OrderBy(x => x.MAST_ER_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listERRReport = listERRReport.OrderByDescending(x => x.MAST_ER_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listERRReport = listERRReport.OrderBy(x => x.MAST_ER_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listERRReport.Select(x => new
                {
                    id = x.MAST_ER_ROAD_NUMBER,
                    cell = new[] {
                       x.MAST_ER_ROAD_NUMBER == null ? "NULL" : x.MAST_ER_ROAD_NUMBER.ToString() ,
                       x.MAST_ER_ROAD_NAME == null ? "NULL" : x.MAST_ER_ROAD_NAME.ToString(),
                       x.MAST_ROAD_SHORT_DESC.ToString(),
                       x.MAST_ER_ROAD_TYPE == null ? "NULL" : x.MAST_ER_ROAD_TYPE.ToString(),
                       x.ROAD_LENGTH.ToString(),
                       x.MAST_CONS_YEAR == null ? "-" : x.MAST_CONS_YEAR.ToString(),
                       x.MAST_CORE_NETWORK == null ? "NULL" :  x.MAST_CORE_NETWORK.ToString(),
                      // x.MAST_HAB_STATUS==null?"":x.MAST_HAB_STATUS.ToString(), //
                      // x.MAST_HAB_NAME==null?"":x.MAST_HAB_NAME.ToString(), //
                      // x.MAST_HAB_TOT_POP.ToString(), //
                       x.MAST_SOIL_TYPE_NAME == null ? "NULL" : x.MAST_SOIL_TYPE_NAME.ToString(),
                       x.MAST_TERRAIN_TYPE_NAME == null ? "NULL" : x.MAST_TERRAIN_TYPE_NAME.ToString(),
                       x.MAST_ER_SURFACE_LENGTH.ToString() //Surface Length
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


        public Array ERR5StateReportListingDAL(int page, int rows, string sidx, string sord, out int totalRecords,int cbrValue)
        {


            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 1;
                int State = 0;
                int District = 0;
                int Block = 0;
                int CBR = cbrValue;
                int PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var listERR4Reports = dbContext.USP_DRRP_R5_REPORT(Level, State, District, Block, CBR, PMGSYSession.Current.PMGSYScheme).ToList<USP_DRRP_R5_REPORT_Result>();

                totalRecords = listERR4Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listERR4Reports = listERR4Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listERR4Reports = listERR4Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listERR4Reports = listERR4Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listERR4Reports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[]{
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='ERR5DistrictReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+CBR.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",                       
                        x.CBR3.ToString(),
                        x.CBR4.ToString(),
                        x.CBR9.ToString(),
                        x.CBR10.ToString(),
                        x.CBR_OTHER.ToString(),  
                        x.TOTAL_CBR.ToString()         
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


        public Array ERR5DistrictReportListingDAL(int stateCode, int page, int rows, string sidx, string sord, out int totalRecords, int cbrValue)
        {
            // List<USP_CN2_REPORT_Result> listERR1Reports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 2;
                int State = stateCode;
                int District = 0;
                int Block = 0;
                int CBR = cbrValue;
                int PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var listERR5Reports = dbContext.USP_DRRP_R5_REPORT(Level, State, District, Block, CBR, PMGSYSession.Current.PMGSYScheme).ToList<USP_DRRP_R5_REPORT_Result>();


                totalRecords = listERR5Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listERR5Reports = listERR5Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listERR5Reports = listERR5Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listERR5Reports = listERR5Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listERR5Reports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[] {
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='ERR5BlockReportListing(\""+stateCode.ToString().Trim()+"\",\""+x.LOCATION_CODE.ToString()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+CBR.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                         x.CBR3.ToString(),
                        x.CBR4.ToString(),
                        x.CBR9.ToString(),
                        x.CBR10.ToString(),
                        x.CBR_OTHER.ToString(),  
                        x.TOTAL_CBR.ToString()  
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


        public Array ERR5BlockReportListingDAL(int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords, int cbrValue)
        {
            // List<USP_CN2_REPORT_Result> listERR1Reports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 3;
                int State = stateCode;
                int District = districtCode;
                int Block = 0;
                int CBR = cbrValue;
                int PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var listERR5Reports = dbContext.USP_DRRP_R5_REPORT(Level, State, District, Block, CBR, PMGSYSession.Current.PMGSYScheme).ToList<USP_DRRP_R5_REPORT_Result>();

                totalRecords = listERR5Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listERR5Reports = listERR5Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listERR5Reports = listERR5Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listERR5Reports = listERR5Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listERR5Reports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[] {
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='ERR5FinalReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+districtCode.ToString()+"\",\""+stateCode.ToString()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+CBR.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                        //x.LOCATION_NAME.ToString(),
                        x.CBR3.ToString(),
                        x.CBR4.ToString(),
                        x.CBR9.ToString(),
                        x.CBR10.ToString(),
                        x.CBR_OTHER.ToString(),  
                        x.TOTAL_CBR.ToString()
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


        public Array ERR5FinalListingDAL(int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords, int cbrValue)
        {
            // List<USP_CN2_FINAL_REPORT_Result> listERRReport;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                // int Level = 1;
                int State = stateCode;
                int District = districtCode;
                int Block = blockCode;
                int CBRValue = cbrValue;
                //string Type = "%";
                //int SType = 0;
                //int TType = 0;               
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;

                //var listERRReport = dbContext.USP_DRRP_REPORT(State, District, Block, Type, SType, TType, PMGSY).ToList<USP_DRRP_REPORT_Result>();

              var listERRReport = dbContext.USP_DRRP_R5LVL_REPORT(State, District, Block,CBRValue, PMGSY).ToList<USP_DRRP_R5LVL_REPORT_Result>();
         

                totalRecords = listERRReport.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listERRReport = listERRReport.OrderBy(x => x.MAST_ER_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listERRReport = listERRReport.OrderByDescending(x => x.MAST_ER_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listERRReport = listERRReport.OrderBy(x => x.MAST_ER_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listERRReport.Select(x => new
                {
                    id = x.MAST_ER_ROAD_NUMBER,
                    cell = new[] {
                       x.MAST_ER_ROAD_NUMBER == null ? "NULL" : x.MAST_ER_ROAD_NUMBER.ToString() ,
                       x.MAST_ER_ROAD_NAME == null ? "NULL" : x.MAST_ER_ROAD_NAME.ToString(),
                       x.MAST_ROAD_SHORT_DESC.ToString(),
                       x.MAST_ER_ROAD_TYPE == null ? "NULL" : x.MAST_ER_ROAD_TYPE.ToString(),
                       x.ROAD_LENGTH.ToString(),
                       x.MAST_CONS_YEAR == null ? "-" : x.MAST_CONS_YEAR.ToString(),
                       x.MAST_CORE_NETWORK == null ? "NULL" :  x.MAST_CORE_NETWORK.ToString(),
                      // x.MAST_HAB_STATUS==null?"":x.MAST_HAB_STATUS.ToString(),//
                      // x.MAST_HAB_NAME==null?"":x.MAST_HAB_NAME.ToString(),//
                      // x.MAST_HAB_TOT_POP.ToString(),//
                       x.MAST_SOIL_TYPE_NAME == null ? "NULL" : x.MAST_SOIL_TYPE_NAME.ToString(),
                       x.MAST_TERRAIN_TYPE_NAME == null ? "NULL" : x.MAST_TERRAIN_TYPE_NAME.ToString(),
                      x.MAST_STR_CHAIN.ToString(), // Start Chainage
                      x.MAST_END_CHAIN.ToString(), // End Chainage
                      x.MAST_CBR_VALUE.ToString(), // CBR Vlaue
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


        public Array ERR6StateReportListingDAL(int page, int rows, string sidx, string sord, out int totalRecords,string roadType)
        {


            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 1;
                int State = 0;
                int District = 0;
                int Block = 0;
                string Type =roadType;
                int PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var listERR6Reports = dbContext.USP_DRRP_R6_REPORT(Level, State, District, Block, Type, PMGSYSession.Current.PMGSYScheme).ToList<USP_DRRP_R6_REPORT_Result>();

                totalRecords = listERR6Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listERR6Reports = listERR6Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listERR6Reports = listERR6Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listERR6Reports = listERR6Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listERR6Reports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[]{
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='ERR6DistrictReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+Type.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",                       
                        x.ROAD_ALL.ToString(),
                        x.ROAD_ALL_LEN.ToString(),
                        x.ROAD_CALL.ToString(),
                        x.ROAD_CALL_LEN.ToString(),
                        x.ROAD_FAIR.ToString(), 
                        x.ROAD_FAIR_LEN.ToString(),
                        x.ROAD_CFAIR.ToString(), 
                        x.ROAD_CFAIR_LEN.ToString(), 
                        x.ROAD_TOTAL.ToString(),
                        x.ROAD_TOTAL_LEN.ToString(),
                        x.ROAD_CTOTAL.ToString(),
                        x.ROAD_CTOTAL_LEN.ToString()              
                   
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


        public Array ERR6DistrictReportListingDAL(int stateCode, int page, int rows, string sidx, string sord, out int totalRecords,string roadType)
        {
            // List<USP_CN2_REPORT_Result> listERR1Reports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 2;
                int State = stateCode;
                int District = 0;
                int Block = 0;
                string Type = roadType;
                int PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var listERR6Reports = dbContext.USP_DRRP_R6_REPORT(Level, State, District, Block, Type, PMGSYSession.Current.PMGSYScheme).ToList<USP_DRRP_R6_REPORT_Result>();


                totalRecords = listERR6Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listERR6Reports = listERR6Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listERR6Reports = listERR6Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listERR6Reports = listERR6Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listERR6Reports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[] {
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='ERR6BlockReportListing(\""+stateCode.ToString().Trim()+"\",\""+x.LOCATION_CODE.ToString()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+Type.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                         x.ROAD_ALL.ToString(),
                        x.ROAD_ALL_LEN.ToString(),
                        x.ROAD_CALL.ToString(),
                        x.ROAD_CALL_LEN.ToString(),
                        x.ROAD_FAIR.ToString(), 
                        x.ROAD_FAIR_LEN.ToString(),
                        x.ROAD_CFAIR.ToString(), 
                        x.ROAD_CFAIR_LEN.ToString(), 
                        x.ROAD_TOTAL.ToString(),
                        x.ROAD_TOTAL_LEN.ToString(),
                        x.ROAD_CTOTAL.ToString(),
                        x.ROAD_CTOTAL_LEN.ToString()    
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


        public Array ERR6BlockReportListingDAL(int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords,string roadType)
        {
            // List<USP_CN2_REPORT_Result> listERR1Reports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 3;
                int State = stateCode;
                int District = districtCode;
                int Block = 0;
                string Type = roadType;
                int PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var listERR6Reports = dbContext.USP_DRRP_R6_REPORT(Level, State, District, Block, Type, PMGSYSession.Current.PMGSYScheme).ToList<USP_DRRP_R6_REPORT_Result>();


                totalRecords = listERR6Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listERR6Reports = listERR6Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listERR6Reports = listERR6Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listERR6Reports = listERR6Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listERR6Reports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[] {
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='ERR6FinalReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+districtCode.ToString()+"\",\""+stateCode.ToString()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+Type.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                        //x.LOCATION_NAME.ToString(),
                        x.ROAD_ALL.ToString(),
                        x.ROAD_ALL_LEN.ToString(),
                        x.ROAD_CALL.ToString(),
                        x.ROAD_CALL_LEN.ToString(),
                        x.ROAD_FAIR.ToString(), 
                        x.ROAD_FAIR_LEN.ToString(),
                        x.ROAD_CFAIR.ToString(), 
                        x.ROAD_CFAIR_LEN.ToString(), 
                        x.ROAD_TOTAL.ToString(),
                        x.ROAD_TOTAL_LEN.ToString(),
                        x.ROAD_CTOTAL.ToString(),
                        x.ROAD_CTOTAL_LEN.ToString()    
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


        public Array ERR6FinalListingDAL(int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords, string roadType)
        {
            // List<USP_CN2_FINAL_REPORT_Result> listERRReport;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                // int Level = 1;
                int State = stateCode;
                int District = districtCode;
                int Block = blockCode;
                string Type = "%";
                int SType = 0;
                int TType = 0;

                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;

                var listERRReport = dbContext.USP_DRRP_REPORT(State, District, Block, Type, SType, TType, PMGSY).ToList<USP_DRRP_REPORT_Result>();



                totalRecords = listERRReport.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listERRReport = listERRReport.OrderBy(x => x.MAST_ER_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listERRReport = listERRReport.OrderByDescending(x => x.MAST_ER_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listERRReport = listERRReport.OrderBy(x => x.MAST_ER_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listERRReport.Select(x => new
                {
                    id = x.MAST_ER_ROAD_NUMBER,
                    cell = new[] {
                       x.MAST_ER_ROAD_NUMBER == null ? "NULL" : x.MAST_ER_ROAD_NUMBER.ToString() ,
                       x.MAST_ER_ROAD_NAME == null ? "NULL" : x.MAST_ER_ROAD_NAME.ToString(),
                        x.MAST_ROAD_SHORT_DESC.ToString(),
                       x.MAST_ER_ROAD_TYPE == null ? "NULL" : x.MAST_ER_ROAD_TYPE.ToString(),
                       x.ROAD_LENGTH.ToString(),
                       x.MAST_CONS_YEAR == null ? "-" : x.MAST_CONS_YEAR.ToString(),
                       x.MAST_CORE_NETWORK == null ? "NULL" :  x.MAST_CORE_NETWORK.ToString(),
                        x.MAST_HAB_STATUS==null?"":x.MAST_HAB_STATUS.ToString(),
                       x.MAST_HAB_NAME==null?"":x.MAST_HAB_NAME.ToString(),
                     x.MAST_HAB_TOT_POP.ToString(),
                       x.MAST_SOIL_TYPE_NAME == null ? "NULL" : x.MAST_SOIL_TYPE_NAME.ToString(),
                       x.MAST_TERRAIN_TYPE_NAME == null ? "NULL" : x.MAST_TERRAIN_TYPE_NAME.ToString()                      
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


        public Array ERR7StateReportListingDAL(int terrainType,int page, int rows, string sidx, string sord, out int totalRecords)
        {

           // List<USP_DRRP_R7_StateLevel_REPORT> listERR7Reports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 1;
                int State = 0;
                int District = 0;
                int Block = 0;
                int Type = terrainType;
                int PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var listERR7Reports = dbContext.USP_DRRP_R7_REPORT(Level, State, District, Block, Type, PMGSYSession.Current.PMGSYScheme).ToList<USP_DRRP_R7_REPORT_Result>();
              //listERR7Reports = dbContext.Database.SqlQuery<USP_DRRP_R7_StateLevel_REPORT>("EXEC [omms].[USP_DRRP_R7_REPORT] @LEVEL,@STATE,@DISTRICT,@BLOCK,@TYPE,@PMGSY",
              //new SqlParameter("@LEVEL", Level),
              //new SqlParameter("@STATE", State),
              //new SqlParameter("@DISTRICT", District),
              //new SqlParameter("@BLOCK", Block),
              //new SqlParameter("@TYPE", Type),
              //new SqlParameter("@PMGSY", PMGSY)
              //).ToList<USP_DRRP_R7_StateLevel_REPORT>();

                totalRecords = listERR7Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listERR7Reports = listERR7Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listERR7Reports = listERR7Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listERR7Reports = listERR7Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listERR7Reports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[]{
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='ERR7DistrictReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+Type.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",                       
                        x.ROAD_PLAIN.ToString(),
                        x.ROAD_PLAIN_LEN.ToString(),
                        x.ROAD_CPLAIN.ToString(),
                        x.ROAD_CPLAIN_LEN.ToString(),
                        x.ROAD_ROLL.ToString(), 
                        x.ROAD_ROLL_LEN.ToString(),
                        x.ROAD_CROLL.ToString(), 
                        x.ROAD_CROLL_LEN.ToString(), 
                        x.ROAD_HILLY.ToString(),
                        x.ROAD_HILLY_LEN.ToString(),
                        x.ROAD_CHILLY.ToString(),
                        x.ROAD_CHILLY_LEN.ToString(),
                        x.ROAD_STEEP.ToString(), 
                        x.ROAD_STEEP_LEN.ToString(),
                        x.ROAD_CSTEEP.ToString(), 
                        x.ROAD_CSTEEP_LEN.ToString(), 
                        x.ROAD_TOTAL.ToString(),                        
                        x.ROAD_TOTAL_LEN.ToString(),
                        x.ROAD_CTOTAL.ToString(),
                        x.ROAD_CTOTAL_LEN.ToString()                  
                   
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


        public Array ERR7DistrictReportListingDAL(int stateCode,int terrainType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
           // List<USP_DRRP_R7_StateLevel_REPORT> listERR7Reports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 2;
                int State = stateCode;
                int District = 0;
                int Block = 0;
                int Type = terrainType;
                int PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var listERR7Reports = dbContext.USP_DRRP_R7_REPORT(Level, State, District, Block, Type, PMGSYSession.Current.PMGSYScheme).ToList<USP_DRRP_R7_REPORT_Result>();
                //listERR7Reports = dbContext.Database.SqlQuery<USP_DRRP_R7_StateLevel_REPORT>("EXEC [omms].[USP_DRRP_R7_REPORT] @LEVEL,@STATE,@DISTRICT,@BLOCK,@TYPE,@PMGSY",
                //new SqlParameter("@LEVEL", Level),
                //new SqlParameter("@STATE", State),
                //new SqlParameter("@DISTRICT", District),
                //new SqlParameter("@BLOCK", Block),
                //new SqlParameter("@TYPE", Type),
                //new SqlParameter("@PMGSY", PMGSY)
                //).ToList<USP_DRRP_R7_StateLevel_REPORT>();
                totalRecords = listERR7Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listERR7Reports = listERR7Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listERR7Reports = listERR7Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listERR7Reports = listERR7Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listERR7Reports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[] {
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='ERR7BlockReportListing(\""+stateCode.ToString().Trim()+"\",\""+x.LOCATION_CODE.ToString()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+Type.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                         x.ROAD_PLAIN.ToString(),
                        x.ROAD_PLAIN_LEN.ToString(),
                        x.ROAD_CPLAIN.ToString(),
                        x.ROAD_CPLAIN_LEN.ToString(),
                        x.ROAD_ROLL.ToString(), 
                        x.ROAD_ROLL_LEN.ToString(),
                        x.ROAD_CROLL.ToString(), 
                        x.ROAD_CROLL_LEN.ToString(), 
                        x.ROAD_HILLY.ToString(),
                        x.ROAD_HILLY_LEN.ToString(),
                        x.ROAD_CHILLY.ToString(),
                        x.ROAD_CHILLY_LEN.ToString(),
                        x.ROAD_STEEP.ToString(), 
                        x.ROAD_STEEP_LEN.ToString(),
                        x.ROAD_CSTEEP.ToString(), 
                        x.ROAD_CSTEEP_LEN.ToString(), 
                        x.ROAD_TOTAL.ToString(),                        
                        x.ROAD_TOTAL_LEN.ToString(),
                        x.ROAD_CTOTAL.ToString(),
                        x.ROAD_CTOTAL_LEN.ToString()                  
                   
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


        public Array ERR7BlockReportListingDAL(int stateCode, int districtCode,int terrainType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
           // List<USP_DRRP_R7_StateLevel_REPORT> listERR7Reports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 3;
                int State = stateCode;
                int District = districtCode;
                int Block = 0;
                int Type = terrainType;
                int PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var listERR7Reports = dbContext.USP_DRRP_R7_REPORT(Level, State, District, Block, Type, PMGSYSession.Current.PMGSYScheme).ToList<USP_DRRP_R7_REPORT_Result>();
                //listERR7Reports = dbContext.Database.SqlQuery<USP_DRRP_R7_StateLevel_REPORT>("EXEC [omms].[USP_DRRP_R7_REPORT] @LEVEL,@STATE,@DISTRICT,@BLOCK,@TYPE,@PMGSY",
                //new SqlParameter("@LEVEL", Level),
                //new SqlParameter("@STATE", State),
                //new SqlParameter("@DISTRICT", District),
                //new SqlParameter("@BLOCK", Block),
                //new SqlParameter("@TYPE", Type),
                //new SqlParameter("@PMGSY", PMGSY)
                //).ToList<USP_DRRP_R7_StateLevel_REPORT>();

                totalRecords = listERR7Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listERR7Reports = listERR7Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listERR7Reports = listERR7Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listERR7Reports = listERR7Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listERR7Reports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[] {
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='ERR7FinalReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+districtCode.ToString()+"\",\""+stateCode.ToString()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+Type.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                        //x.LOCATION_NAME.ToString(),
                       x.ROAD_PLAIN.ToString(),
                        x.ROAD_PLAIN_LEN.ToString(),
                        x.ROAD_CPLAIN.ToString(),
                        x.ROAD_CPLAIN_LEN.ToString(),
                        x.ROAD_ROLL.ToString(), 
                        x.ROAD_ROLL_LEN.ToString(),
                        x.ROAD_CROLL.ToString(), 
                        x.ROAD_CROLL_LEN.ToString(), 
                        x.ROAD_HILLY.ToString(),
                        x.ROAD_HILLY_LEN.ToString(),
                        x.ROAD_CHILLY.ToString(),
                        x.ROAD_CHILLY_LEN.ToString(),
                        x.ROAD_STEEP.ToString(), 
                        x.ROAD_STEEP_LEN.ToString(),
                        x.ROAD_CSTEEP.ToString(), 
                        x.ROAD_CSTEEP_LEN.ToString(), 
                        x.ROAD_TOTAL.ToString(),                        
                        x.ROAD_TOTAL_LEN.ToString(),
                        x.ROAD_CTOTAL.ToString(),
                        x.ROAD_CTOTAL_LEN.ToString()                  
                   
                      
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


        public Array ERR7FinalListingDAL(int blockCode, int districtCode, int stateCode,int terrainType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            // List<USP_CN2_FINAL_REPORT_Result> listERRReport;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                // int Level = 1;
                int State = stateCode;
                int District = districtCode;
                int Block = blockCode;
                string Type = "%";
                int SType = 0;
                int TType =terrainType;

                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;

                var listERRReport = dbContext.USP_DRRP_REPORT(State, District, Block, Type, SType, TType, PMGSY).ToList<USP_DRRP_REPORT_Result>();



                totalRecords = listERRReport.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listERRReport = listERRReport.OrderBy(x => x.MAST_ER_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listERRReport = listERRReport.OrderByDescending(x => x.MAST_ER_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listERRReport = listERRReport.OrderBy(x => x.MAST_ER_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listERRReport.Select(x => new
                {
                    id = x.MAST_ER_ROAD_NUMBER,
                    cell = new[] {
                       x.MAST_ER_ROAD_NUMBER == null ? "NULL" : x.MAST_ER_ROAD_NUMBER.ToString() ,
                       x.MAST_ER_ROAD_NAME == null ? "NULL" : x.MAST_ER_ROAD_NAME.ToString(),
                       x.MAST_ROAD_SHORT_DESC.ToString(),
                       x.MAST_ER_ROAD_TYPE == null ? "NULL" : x.MAST_ER_ROAD_TYPE.ToString(),
                       x.ROAD_LENGTH.ToString(),
                       x.MAST_CONS_YEAR == null ? "-" : x.MAST_CONS_YEAR.ToString(),
                       x.MAST_CORE_NETWORK == null ? "NULL" :  x.MAST_CORE_NETWORK.ToString(),
                        x.MAST_HAB_STATUS==null?"":x.MAST_HAB_STATUS.ToString(),
                       x.MAST_HAB_NAME==null?"":x.MAST_HAB_NAME.ToString(),
                       x.MAST_HAB_TOT_POP.ToString(),
                       x.MAST_SOIL_TYPE_NAME == null ? "NULL" : x.MAST_SOIL_TYPE_NAME.ToString(),
                       x.MAST_TERRAIN_TYPE_NAME == null ? "NULL" : x.MAST_TERRAIN_TYPE_NAME.ToString()                      
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


        public Array ERR8StateReportListingDAL(int soilType,int page, int rows, string sidx, string sord, out int totalRecords)
        {


            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 1;
                int State = 0;
                int District = 0;
                int Block = 0;
                int PMGSY = PMGSYSession.Current.PMGSYScheme;
                int Type =soilType;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var listERR8Reports = dbContext.USP_DRRP_R8_REPORT(Level, State, District, Block, Type, PMGSYSession.Current.PMGSYScheme).ToList<USP_DRRP_R8_REPORT_Result>();

                totalRecords = listERR8Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listERR8Reports = listERR8Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listERR8Reports = listERR8Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listERR8Reports = listERR8Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listERR8Reports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[]{
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='ERR8DistrictReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+Type.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",                       
                        x.ROAD_HARD.ToString(),
                        x.ROAD_HARD_LEN.ToString(),
                        x.ROAD_CHARD.ToString(),
                        x.ROAD_CHARD_LEN.ToString(),
                        x.ROAD_SOFT.ToString(), 
                        x.ROAD_SOFT_LEN.ToString(),
                        x.ROAD_CSOFT.ToString(), 
                        x.ROAD_CSOFT_LEN.ToString(), 
                        x.ROAD_SANDY.ToString(),
                        x.ROAD_SANDY_LEN.ToString(),
                        x.ROAD_CSANDY.ToString(),
                        x.ROAD_CSANDY_LEN.ToString(),
                        x.ROAD_RED.ToString(),                      
                        x.ROAD_RED_LEN.ToString(),
                        x.ROAD_CRED.ToString(),
                        x.ROAD_CRED_LEN.ToString(),
                        x.ROAD_OTHER.ToString(),
                        x.ROAD_OTHER_LEN.ToString(),
                        x.ROAD_COTHER.ToString(),
                        x.ROAD_COTHER_LEN.ToString(),
                        x.ROAD_TOTAL.ToString(),
                        x.ROAD_TOTAL_LEN.ToString(),
                        x.ROAD_CTOTAL.ToString(),
                        x.ROAD_CTOTAL_LEN.ToString()
                              
                   
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


        public Array ERR8DistrictReportListingDAL(int stateCode,int soilType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            // List<USP_CN2_REPORT_Result> listERR1Reports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 2;
                int State = stateCode;
                int District = 0;
                int Block = 0;
                int Type = soilType;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var listERR8Reports = dbContext.USP_DRRP_R8_REPORT(Level, State, District, Block, Type, PMGSYSession.Current.PMGSYScheme).ToList<USP_DRRP_R8_REPORT_Result>();


                totalRecords = listERR8Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listERR8Reports = listERR8Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listERR8Reports = listERR8Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listERR8Reports = listERR8Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listERR8Reports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[] {
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='ERR8BlockReportListing(\""+stateCode.ToString().Trim()+"\",\""+x.LOCATION_CODE.ToString()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+Type.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                      x.ROAD_HARD.ToString(),
                        x.ROAD_HARD_LEN.ToString(),
                        x.ROAD_CHARD.ToString(),
                        x.ROAD_CHARD_LEN.ToString(),
                        x.ROAD_SOFT.ToString(), 
                        x.ROAD_SOFT_LEN.ToString(),
                        x.ROAD_CSOFT.ToString(), 
                        x.ROAD_CSOFT_LEN.ToString(), 
                        x.ROAD_SANDY.ToString(),
                        x.ROAD_SANDY_LEN.ToString(),
                        x.ROAD_CSANDY.ToString(),
                        x.ROAD_CSANDY_LEN.ToString(),
                        x.ROAD_RED.ToString(),                      
                        x.ROAD_RED_LEN.ToString(),
                        x.ROAD_CRED.ToString(),
                        x.ROAD_CRED_LEN.ToString(),
                        x.ROAD_OTHER.ToString(),
                        x.ROAD_OTHER_LEN.ToString(),
                        x.ROAD_COTHER.ToString(),
                        x.ROAD_COTHER_LEN.ToString(),
                        x.ROAD_TOTAL.ToString(),
                        x.ROAD_TOTAL_LEN.ToString(),
                        x.ROAD_CTOTAL.ToString(),
                        x.ROAD_CTOTAL_LEN.ToString()
                              
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


        public Array ERR8BlockReportListingDAL(int stateCode, int districtCode,int soilType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            // List<USP_CN2_REPORT_Result> listERR1Reports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 3;
                int State = stateCode;
                int District = districtCode;
                int Block = 0;
                int Type = soilType;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var listERR8Reports = dbContext.USP_DRRP_R8_REPORT(Level, State, District, Block, Type, PMGSYSession.Current.PMGSYScheme).ToList<USP_DRRP_R8_REPORT_Result>();

                totalRecords = listERR8Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listERR8Reports = listERR8Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listERR8Reports = listERR8Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listERR8Reports = listERR8Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listERR8Reports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[] {
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='ERR8FinalReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+districtCode.ToString()+"\",\""+stateCode.ToString()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+Type.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                        //x.LOCATION_NAME.ToString(),
                        x.ROAD_HARD.ToString(),
                        x.ROAD_HARD_LEN.ToString(),
                        x.ROAD_CHARD.ToString(),
                        x.ROAD_CHARD_LEN.ToString(),
                        x.ROAD_SOFT.ToString(), 
                        x.ROAD_SOFT_LEN.ToString(),
                        x.ROAD_CSOFT.ToString(), 
                        x.ROAD_CSOFT_LEN.ToString(), 
                        x.ROAD_SANDY.ToString(),
                        x.ROAD_SANDY_LEN.ToString(),
                        x.ROAD_CSANDY.ToString(),
                        x.ROAD_CSANDY_LEN.ToString(),
                        x.ROAD_RED.ToString(),                      
                        x.ROAD_RED_LEN.ToString(),
                        x.ROAD_CRED.ToString(),
                        x.ROAD_CRED_LEN.ToString(),
                        x.ROAD_OTHER.ToString(),
                        x.ROAD_OTHER_LEN.ToString(),
                        x.ROAD_COTHER.ToString(),
                        x.ROAD_COTHER_LEN.ToString(),
                        x.ROAD_TOTAL.ToString(),
                        x.ROAD_TOTAL_LEN.ToString(),
                        x.ROAD_CTOTAL.ToString(),
                        x.ROAD_CTOTAL_LEN.ToString()
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


        public Array ERR8FinalListingDAL(int blockCode, int districtCode, int stateCode,int soilType,int page, int rows, string sidx, string sord, out int totalRecords)
        {
            // List<USP_CN2_FINAL_REPORT_Result> listERRReport;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                // int Level = 1;
                int State = stateCode;
                int District = districtCode;
                int Block = blockCode;
                string Type = "%";
                int SType =soilType;
                int TType = 0;

                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;

                var listERRReport = dbContext.USP_DRRP_REPORT(State, District, Block, Type, SType, TType, PMGSY).ToList<USP_DRRP_REPORT_Result>();



                totalRecords = listERRReport.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listERRReport = listERRReport.OrderBy(x => x.MAST_ER_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listERRReport = listERRReport.OrderByDescending(x => x.MAST_ER_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listERRReport = listERRReport.OrderBy(x => x.MAST_ER_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listERRReport.Select(x => new
                {
                    id = x.MAST_ER_ROAD_NUMBER,
                    cell = new[] {
                       x.MAST_ER_ROAD_NUMBER == null ? "NULL" : x.MAST_ER_ROAD_NUMBER.ToString() ,
                       x.MAST_ER_ROAD_NAME == null ? "NULL" : x.MAST_ER_ROAD_NAME.ToString(),
                       x.MAST_ROAD_SHORT_DESC.ToString(),
                       x.MAST_ER_ROAD_TYPE == null ? "NULL" : x.MAST_ER_ROAD_TYPE.ToString(),
                       x.ROAD_LENGTH.ToString(),
                       x.MAST_CONS_YEAR == null ? "-" : x.MAST_CONS_YEAR.ToString(),
                       x.MAST_CORE_NETWORK == null ? "NULL" :  x.MAST_CORE_NETWORK.ToString(),
                       x.MAST_HAB_STATUS==null?"":x.MAST_HAB_STATUS.ToString(),
                       x.MAST_HAB_NAME==null?"":x.MAST_HAB_NAME.ToString(),
                      x.MAST_HAB_TOT_POP.ToString(),
                       x.MAST_SOIL_TYPE_NAME == null ? "NULL" : x.MAST_SOIL_TYPE_NAME.ToString(),
                       x.MAST_TERRAIN_TYPE_NAME == null ? "NULL" : x.MAST_TERRAIN_TYPE_NAME.ToString()                      
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

        public List<SelectListItem> GetTerrainTypeList()
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<SelectListItem> lstAgency = new List<SelectListItem>();

                var list = (from ag in dbContext.MASTER_TERRAIN_TYPE
                            select new
                            {
                                Value = ag.MAST_TERRAIN_TYPE_CODE,
                                Text = ag.MAST_TERRAIN_TYPE_NAME
                            }).Distinct().OrderBy(m => m.Text).ToList();


                lstAgency = new SelectList(list.ToList(), "Value", "Text").ToList();
                lstAgency.Insert(0, new SelectListItem { Text = "All", Value = "0" });
                return lstAgency;

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

        public List<SelectListItem> GetSoilTypeList()
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<SelectListItem> lstAgency = new List<SelectListItem>();

                var list = (from ag in dbContext.MASTER_SOIL_TYPE
                            select new
                            {
                                Value = ag.MAST_SOIL_TYPE_CODE,
                                Text = ag.MAST_SOIL_TYPE_NAME
                            }).Distinct().OrderBy(m => m.Text).ToList();


                lstAgency = new SelectList(list.ToList(), "Value", "Text").ToList();
                lstAgency.Insert(0, new SelectListItem { Text = "All", Value = "0" });
                return lstAgency;

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

    public interface IExistingRoadsReportsDAL
    {

        Array ERR1StateReportListingDAL(int page, int rows, string sidx, string sord, out int totalRecords);
        Array ERR1DistrictReportListingDAL(int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array ERR1BlockReportListingDAL(int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array ERR1FinalListingDAL(int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);

        Array ERR2StateReportListingDAL(int page, int rows, string sidx, string sord, out int totalRecords);
        Array ERR2DistrictReportListingDAL(int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array ERR2BlockReportListingDAL(int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array ERR2FinalListingDAL(int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);


        Array ERR3StateReportListingDAL(int page, int rows, string sidx, string sord, out int totalRecords);
        Array ERR3DistrictReportListingDAL(int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array ERR3BlockReportListingDAL(int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array ERR3FinalListingDAL(int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);



        Array ERR4StateReportListingDAL(int page, int rows, string sidx, string sord, out int totalRecords);
        Array ERR4DistrictReportListingDAL(int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array ERR4BlockReportListingDAL(int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array ERR4FinalListingDAL(int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);


        Array ERR5StateReportListingDAL(int page, int rows, string sidx, string sord, out int totalRecords, int cbrValue);
        Array ERR5DistrictReportListingDAL(int stateCode, int page, int rows, string sidx, string sord, out int totalRecords, int cbrValue);
        Array ERR5BlockReportListingDAL(int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords, int cbrValue);
        Array ERR5FinalListingDAL(int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords, int cbrValue);

        Array ERR6StateReportListingDAL(int page, int rows, string sidx, string sord, out int totalRecords,string roadType);
        Array ERR6DistrictReportListingDAL(int stateCode, int page, int rows, string sidx, string sord, out int totalRecords, string roadType);
        Array ERR6BlockReportListingDAL(int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords, string roadType);
        Array ERR6FinalListingDAL(int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords, string roadType);

        Array ERR7StateReportListingDAL(int terrainType, int page, int rows, string sidx, string sord, out int totalRecords);
        Array ERR7DistrictReportListingDAL(int stateCode, int terrainType, int page, int rows, string sidx, string sord, out int totalRecords);
        Array ERR7BlockReportListingDAL(int stateCode, int districtCode, int terrainType, int page, int rows, string sidx, string sord, out int totalRecords);
        Array ERR7FinalListingDAL(int blockCode, int districtCode, int stateCode, int terrainType, int page, int rows, string sidx, string sord, out int totalRecords);

        Array ERR8StateReportListingDAL(int soilType, int page, int rows, string sidx, string sord, out int totalRecords);
        Array ERR8DistrictReportListingDAL(int stateCode, int soilType, int page, int rows, string sidx, string sord, out int totalRecords);
        Array ERR8BlockReportListingDAL(int stateCode, int districtCode, int soilType, int page, int rows, string sidx, string sord, out int totalRecords);
        Array ERR8FinalListingDAL(int blockCode, int districtCode, int stateCode, int soilType, int page, int rows, string sidx, string sord, out int totalRecords);

    }
}