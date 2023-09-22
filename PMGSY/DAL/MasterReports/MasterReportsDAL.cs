#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   MasterReportsDAL.cs        
        * Description   :   Listing of Records for all Form Reports
        * Author        :   Pranav Nerkar 
        * Creation Date :   4/October/2013
 **/
#endregion

using PMGSY.Extensions;
using PMGSY.Models;
using PMGSY.Models.MasterReports;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Reflection;

namespace PMGSY.DAL.MasterReports
{
    public class MasterReportsDAL : IMasterReportsDAL
    {
        PMGSYEntities dbContext;
        public Array StateDetailsListingDAL(string stateOrUnion, string stateType,string activeType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSYEntities();
            int State = 0;
            int District = 0;
            int Block = 0;
            int Village = 0;
            int Panchayat = 0;
            int CensusYear = 0;
            string ActiveType = activeType;
            string Type1 = stateOrUnion;
            string Type2 = stateType;
            try
            {
                List<USP_MAS_DATA_STATE_REPORT_Result> lstMasterReport = new List<USP_MAS_DATA_STATE_REPORT_Result>();
                lstMasterReport = dbContext.Database.SqlQuery<USP_MAS_DATA_STATE_REPORT_Result>(
                    "EXEC [omms].[USP_MAS_DATA_REPORT] @RptNo,@State,@District,@Block,@Village,@Panchayat,@CensusYear,@Type1,@Type2,@Type3,@Type4,@Type5,@Active",
                    new SqlParameter("RptNo", 1),
                    new SqlParameter("State", State),
                    new SqlParameter("District", District),
                    new SqlParameter("Block", Block),
                    new SqlParameter("Village", Village),
                    new SqlParameter("Panchayat", Panchayat),
                    new SqlParameter("CensusYear", CensusYear),
                    new SqlParameter("Type1", Type1),
                    new SqlParameter("Type2", Type2),
                    new SqlParameter("Type3", '%'),
                    new SqlParameter("Type4", '%'),
                    new SqlParameter("Type5", '%'),
                    new SqlParameter("Active", activeType)
                    ).ToList<USP_MAS_DATA_STATE_REPORT_Result>();

                totalRecords = lstMasterReport.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx.Trim())
                        {
                            case "MAST_STATE_UT":
                                lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_STATE_UT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;

                            case "MAST_STATE_TYPE":
                                lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_STATE_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;

                            default:
                                lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;

                        }

                    }
                    else
                    {
                        switch (sidx.Trim())
                        {
                            case "MAST_STATE_UT":
                                lstMasterReport = lstMasterReport.OrderByDescending(x => x.MAST_STATE_UT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;

                            case "MAST_STATE_TYPE":
                                lstMasterReport = lstMasterReport.OrderByDescending(x => x.MAST_STATE_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            default:
                                lstMasterReport = lstMasterReport.OrderByDescending(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;

                        }
                    }
                }
                else
                {
                    if (rows == 0)
                        lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(totalRecords)).ToList();
                    else
                        lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                if (rows == 0)
                {
                    return lstMasterReport.ToArray();
                }

                return lstMasterReport.Select(x => new
                {
                    id = x.MAST_STATE_CODE,
                    cell = new[]{
                        x.MAST_STATE_NAME,
                        x.MAST_STATE_UT.ToString(),
                        x.MAST_STATE_TYPE.ToString(),
                        x.MAST_STATE_SHORT_CODE.ToString(),
                        x.MAST_STATE_ACTIVE.ToString()
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


        public Array DistrictDetailsListingDAL(int stateCode, string pmgsyIncluded, string iapDistrict,string activeType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSYEntities();
            int State = stateCode;
            int District = 0;
            int Block = 0;
            int Village = 0;
            int Panchayat = 0;
            int CensusYear = 0;
            string ActiveType = activeType;
            string Type1 = iapDistrict == "All" ? "%" : iapDistrict;
            string Type2 = pmgsyIncluded == "All" ? "%" : pmgsyIncluded;
            try
            {
                List<USP_MAS_DATA_DISTRICT_REPORT_Result> lstMasterReport = new List<USP_MAS_DATA_DISTRICT_REPORT_Result>();
                lstMasterReport = dbContext.Database.SqlQuery<USP_MAS_DATA_DISTRICT_REPORT_Result>(
                    "EXEC [omms].[USP_MAS_DATA_REPORT] @RptNo,@State,@District,@Block,@Village,@Panchayat,@CensusYear,@Type1,@Type2,@Type3,@Type4,@Type5,@Active",
                    new SqlParameter("RptNo", 2),
                    new SqlParameter("State", State),
                    new SqlParameter("District", District),
                    new SqlParameter("Block", Block),
                    new SqlParameter("Village", Village),
                    new SqlParameter("Panchayat", Panchayat),
                    new SqlParameter("CensusYear", CensusYear),
                    new SqlParameter("Type1", Type1),
                    new SqlParameter("Type2", Type2),
                    new SqlParameter("Type3", '%'),
                    new SqlParameter("Type4", '%'),
                    new SqlParameter("Type5", '%'),
                    new SqlParameter("Active", ActiveType)
                    ).ToList<USP_MAS_DATA_DISTRICT_REPORT_Result>();

                totalRecords = lstMasterReport.Count();
                PropertyInfo propertyInfo = typeof(USP_MAS_DATA_DISTRICT_REPORT_Result).GetProperty(sidx.Trim());
                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstMasterReport = lstMasterReport.OrderBy(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        lstMasterReport = lstMasterReport.OrderByDescending(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }


                return lstMasterReport.Select(x => new
                {
                    id = x.MAST_DISTRICT_CODE,
                    cell = new[]{
                        x.MAST_STATE_NAME,
                        x.MAST_DISTRICT_NAME,
                        x.MAST_PMGSY_INCLUDED.ToString(),
                        x.MAST_IAP_DISTRICT.ToString(),
                        x.MAST_DISTRICT_ACTIVE.ToString()
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


        public Array BlockDetailsListingDAL(int districtCode, int stateCode, string isDesert, string isTribal, string pmgsyIncluded, string isSchedule5, string activeType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSYEntities();
            int State = stateCode;
            int District = districtCode;
            int Block = 0;
            int Village = 0;
            int Panchayat = 0;
            int CensusYear = 0;
            string Type1 = isDesert == "All" ? "%" : isDesert;
            string Type2 = pmgsyIncluded == "All" ? "%" : pmgsyIncluded;
            string Type3 = isSchedule5 == "All" ? "%" : isSchedule5;
            string Type4 = isTribal == "All" ? "%" : isTribal;
            string ActiveType = activeType;
            try
            {
                List<USP_MAS_DATA_BLOCK_REPORT_Result> lstMasterReport = new List<USP_MAS_DATA_BLOCK_REPORT_Result>();
                lstMasterReport = dbContext.Database.SqlQuery<USP_MAS_DATA_BLOCK_REPORT_Result>(
                    "EXEC [omms].[USP_MAS_DATA_REPORT] @RptNo,@State,@District,@Block,@Village,@Panchayat,@CensusYear,@Type1,@Type2,@Type3,@Type4,@Type5,@Active",
                    new SqlParameter("RptNo", 3),
                    new SqlParameter("State", State),
                    new SqlParameter("District", District),
                    new SqlParameter("Block", Block),
                    new SqlParameter("Village", Village),
                    new SqlParameter("Panchayat", Panchayat),
                    new SqlParameter("CensusYear", CensusYear),
                    new SqlParameter("Type1", Type1),
                    new SqlParameter("Type2", Type2),
                    new SqlParameter("Type3", Type3),
                    new SqlParameter("Type4", Type4),
                    new SqlParameter("Type5", '%'),
                    new SqlParameter("Active", ActiveType)
                    ).ToList<USP_MAS_DATA_BLOCK_REPORT_Result>();

                totalRecords = lstMasterReport.Count();
                PropertyInfo propertyInfo = typeof(USP_MAS_DATA_BLOCK_REPORT_Result).GetProperty(sidx.Trim());
                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstMasterReport = lstMasterReport.OrderBy(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        lstMasterReport = lstMasterReport.OrderByDescending(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }


                return lstMasterReport.Select(x => new
                {
                    id = x.MAST_BLOCK_CODE,
                    cell = new[]{
                        x.MAST_STATE_NAME,
                        x.MAST_DISTRICT_NAME,
                        x.MAST_BLOCK_NAME,
                        x.MAST_IS_DESERT,
                        x.MAST_IS_TRIBAL,
                        x.MAST_PMGSY_INCLUDED,
                        x.MAST_SCHEDULE5,
                        x.MAST_BLOCK_ACTIVE.ToString()

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


        public Array VillageDetailsListingDAL(int censusYear, int blockCode, int districtCode, int stateCode, string isSchedule5, string activeType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSYEntities();
            int State = stateCode;
            int District = districtCode;
            int Block = blockCode;
            int Village = 0;
            int Panchayat = 0;
            int CensusYear = censusYear;
            string Type1 = isSchedule5 == "All" ? "%" : isSchedule5;
            string ActiveType = activeType;
            try
            {
                List<USP_MAS_DATA_VILLAGE_REPORT_Result> lstMasterReport = new List<USP_MAS_DATA_VILLAGE_REPORT_Result>();
                lstMasterReport = dbContext.Database.SqlQuery<USP_MAS_DATA_VILLAGE_REPORT_Result>(
                    "EXEC [omms].[USP_MAS_DATA_REPORT] @RptNo,@State,@District,@Block,@Village,@Panchayat,@CensusYear,@Type1,@Type2,@Type3,@Type4,@Type5,@Active",
                    new SqlParameter("RptNo", 4),
                    new SqlParameter("State", State),
                    new SqlParameter("District", District),
                    new SqlParameter("Block", Block),
                    new SqlParameter("Village", Village),
                    new SqlParameter("Panchayat", Panchayat),
                    new SqlParameter("CensusYear", CensusYear),
                    new SqlParameter("Type1", Type1),
                    new SqlParameter("Type2", '%'),
                    new SqlParameter("Type3", '%'),
                    new SqlParameter("Type4", '%'),
                    new SqlParameter("Type5", '%'),
                    new SqlParameter("Active", ActiveType)
                    ).ToList<USP_MAS_DATA_VILLAGE_REPORT_Result>();

                totalRecords = lstMasterReport.Count();
                PropertyInfo propertyInfo = typeof(USP_MAS_DATA_VILLAGE_REPORT_Result).GetProperty(sidx.Trim());

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstMasterReport = lstMasterReport.OrderBy(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        lstMasterReport = lstMasterReport.OrderByDescending(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_VILLAGE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return lstMasterReport.Select(x => new
                {
                    id = x.MAST_VILLAGE_CODE,
                    cell = new[]{
                        x.MAST_STATE_NAME,
                        x.MAST_DISTRICT_NAME,
                        x.MAST_BLOCK_NAME,
                        x.MAST_VILLAGE_NAME,
                        x.MAST_SCHEDULE5,
                        x.VSCST_POP.ToString(),
                        x.VTOT_POP.ToString(),
                        x.MAST_VILLAGE_ACTIVE.ToString()

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


        public Array HabitationDetailsListingDAL(int censusYear, int villageCode, int blockCode, int districtCode, int stateCode, string habitationStatus, string isSchedule5, string activeType, int page, int rows, string sidx, string sord, out Int32 totalRecords)
        {
            dbContext = new PMGSYEntities();
            int State = stateCode;
            int District = districtCode;
            int Block = blockCode;
            int Village = villageCode;
            int Panchayat = 0;
            int CensusYear = censusYear;
            string Type1 = habitationStatus == "All" ? "%" : habitationStatus;
            string Type2 = isSchedule5 == "All" ? "%" : isSchedule5;
            string ActiveType = activeType;
            try
            {
                List<USP_MAS_DATA_HABITATION_REPORT_Result> lstMasterReport = new List<USP_MAS_DATA_HABITATION_REPORT_Result>();
                lstMasterReport = dbContext.Database.SqlQuery<USP_MAS_DATA_HABITATION_REPORT_Result>(
                    "EXEC [omms].[USP_MAS_DATA_REPORT] @RptNo,@State,@District,@Block,@Village,@Panchayat,@CensusYear,@Type1,@Type2,@Type3,@Type4,@Type5,@Active",
                    new SqlParameter("RptNo", 5),
                    new SqlParameter("State", State),
                    new SqlParameter("District", District),
                    new SqlParameter("Block", Block),
                    new SqlParameter("Village", Village),
                    new SqlParameter("Panchayat", Panchayat),
                    new SqlParameter("CensusYear", CensusYear),
                    new SqlParameter("Type1", Type1),
                    new SqlParameter("Type2", Type2),
                    new SqlParameter("Type3", '%'),
                    new SqlParameter("Type4", '%'),
                    new SqlParameter("Type5", '%'),
                    new SqlParameter("Active", ActiveType)
                    ).ToList<USP_MAS_DATA_HABITATION_REPORT_Result>();

                totalRecords = lstMasterReport.Count();
                PropertyInfo propertyInfo = typeof(USP_MAS_DATA_HABITATION_REPORT_Result).GetProperty(sidx.Trim());

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstMasterReport = lstMasterReport.OrderBy(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        lstMasterReport = lstMasterReport.OrderByDescending(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return lstMasterReport.Select(x => new
                {
                    id = x.MAST_HAB_CODE,
                    cell = new[]{
                        x.MAST_STATE_NAME,
                        x.MAST_DISTRICT_NAME,
                        x.MAST_BLOCK_NAME,
                        x.MAST_VILLAGE_NAME,
                        x.MAST_HAB_NAME,
                        x.MAST_MLA_CONST_NAME,
                        x.MAST_MP_CONST_NAME,
                        x.MAST_HAB_TOT_POP.ToString(),
                        x.MAST_HAB_SCST_POP.ToString(),
                        x.MAST_HAB_STATUS,
                        x.MAST_HAB_CONNECTED,
                        x.MAST_SCHEME,
                        x.MAST_PRIMARY_SCHOOL,
                        x.MAST_MIDDLE_SCHOOL,
                        x.MAST_HIGH_SCHOOL,
                        x.MAST_INTERMEDIATE_SCHOOL,
                        x.MAST_DEGREE_COLLEGE,
                        x.MAST_HEALTH_SERVICE,
                        x.MAST_DISPENSARY,
                        x.MAST_MCW_CENTERS,
                        x.MAST_PHCS,
                        x.MAST_VETNARY_HOSPITAL,
                        x.MAST_TELEGRAPH_OFFICE,
                        x.MAST_TELEPHONE_CONNECTION,
                        x.MAST_BUS_SERVICE,
                        x.MAST_RAILWAY_STATION,
                        x.MAST_ELECTRICTY,
                        x.MAST_PANCHAYAT_HQ,
                        x.MAST_TOURIST_PLACE,
                        x.MAST_HABITATION_ACTIVE.ToString(),
                        x.MAST_ELECTRICITY_ADD,
                        x.MAST_BLOCK_HQ,
                        x.MAST_SUB_TEHSIL,
                        x.MAST_PETROL_PUMP,
                        x.MAST_PUMP_ADD,
                        x.MAST_MANDI,
                        x.MAST_WAREHOUSE,
                        x.MAST_RETAIL_SHOP,
                       

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


        public Array MPConstituencyListingDAL(int stateCode, string activeType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSYEntities();
            int State = stateCode;
            int District = 0;
            int Block = 0;
            int Village = 0;
            int Panchayat = 0;
            int CensusYear = 0;
            string ActiveType = activeType;
            try
            {
                List<USP_MAS_DATA_MP_CONSTITUENCY_REPORT_Result> lstMasterReport = new List<USP_MAS_DATA_MP_CONSTITUENCY_REPORT_Result>();
                lstMasterReport = dbContext.Database.SqlQuery<USP_MAS_DATA_MP_CONSTITUENCY_REPORT_Result>(
                    "EXEC [omms].[USP_MAS_DATA_REPORT] @RptNo,@State,@District,@Block,@Village,@Panchayat,@CensusYear,@Type1,@Type2,@Type3,@Type4,@Type5,@Active",
                    new SqlParameter("RptNo", 6),
                    new SqlParameter("State", State),
                    new SqlParameter("District", District),
                    new SqlParameter("Block", Block),
                    new SqlParameter("Village", Village),
                    new SqlParameter("Panchayat", Panchayat),
                    new SqlParameter("CensusYear", CensusYear),
                    new SqlParameter("Type1", '%'),
                    new SqlParameter("Type2", '%'),
                    new SqlParameter("Type3", '%'),
                    new SqlParameter("Type4", '%'),
                    new SqlParameter("Type5", '%'),
                    new SqlParameter("Active", ActiveType)
                    ).ToList<USP_MAS_DATA_MP_CONSTITUENCY_REPORT_Result>();

                totalRecords = lstMasterReport.Count();
                PropertyInfo propertyInfo = typeof(USP_MAS_DATA_MP_CONSTITUENCY_REPORT_Result).GetProperty(sidx.Trim());

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstMasterReport = lstMasterReport.OrderBy(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        lstMasterReport = lstMasterReport.OrderByDescending(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_MP_CONST_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return lstMasterReport.Select(x => new
                {
                    id = x.MAST_MP_CONST_CODE,
                    cell = new[]{
                        x.MAST_STATE_NAME,
                        x.MAST_MP_CONST_NAME,
                        x.MAST_MP_CONST_ACTIVE
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


        public Array MLAConstituencyListingDAL(int stateCode, string activeType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSYEntities();
            int State = stateCode;
            int District = 0;
            int Block = 0;
            int Village = 0;
            int Panchayat = 0;
            int CensusYear = 0;
            string ActiveType = activeType;
            try
            {
                List<USP_MAS_DATA_MLA_CONSTITUENCY_REPORT_Result> lstMasterReport = new List<USP_MAS_DATA_MLA_CONSTITUENCY_REPORT_Result>();
                lstMasterReport = dbContext.Database.SqlQuery<USP_MAS_DATA_MLA_CONSTITUENCY_REPORT_Result>(
                    "EXEC [omms].[USP_MAS_DATA_REPORT] @RptNo,@State,@District,@Block,@Village,@Panchayat,@CensusYear,@Type1,@Type2,@Type3,@Type4,@Type5,@Active",
                    new SqlParameter("RptNo", 7),
                    new SqlParameter("State", State),
                    new SqlParameter("District", District),
                    new SqlParameter("Block", Block),
                    new SqlParameter("Village", Village),
                    new SqlParameter("Panchayat", Panchayat),
                    new SqlParameter("CensusYear", CensusYear),
                    new SqlParameter("Type1", '%'),
                    new SqlParameter("Type2", '%'),
                    new SqlParameter("Type3", '%'),
                    new SqlParameter("Type4", '%'),
                    new SqlParameter("Type5", '%'),
                    new SqlParameter("Active", ActiveType)
                    ).ToList<USP_MAS_DATA_MLA_CONSTITUENCY_REPORT_Result>();

                totalRecords = lstMasterReport.Count();
                PropertyInfo propertyInfo = typeof(USP_MAS_DATA_MLA_CONSTITUENCY_REPORT_Result).GetProperty(sidx.Trim());

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstMasterReport = lstMasterReport.OrderBy(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        lstMasterReport = lstMasterReport.OrderByDescending(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_MLA_CONST_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return lstMasterReport.Select(x => new
                {
                    id = x.MAST_MLA_CONST_CODE,
                    cell = new[]{
                        x.MAST_STATE_NAME,
                        x.MAST_MLA_CONST_NAME,
                        x.MAST_MLA_CONST_ACTIVE
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


        public Array MPBlockListingDAL(int constCode, int stateCode, string activeType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSYEntities();
            int State = stateCode;
            int District = 0;
            int Block = 0;
            int Village = 0;
            int Panchayat = constCode;
            int CensusYear = 0;
            string ActiveType = activeType;
            try
            {
                List<USP_MAS_DATA_MP_BLOCK_REPORT_Result> lstMasterReport = new List<USP_MAS_DATA_MP_BLOCK_REPORT_Result>();
                lstMasterReport = dbContext.Database.SqlQuery<USP_MAS_DATA_MP_BLOCK_REPORT_Result>(
                    "EXEC [omms].[USP_MAS_DATA_REPORT] @RptNo,@State,@District,@Block,@Village,@Panchayat,@CensusYear,@Type1,@Type2,@Type3,@Type4,@Type5,@Active",
                    new SqlParameter("RptNo", 8),
                    new SqlParameter("State", State),
                    new SqlParameter("District", District),
                    new SqlParameter("Block", Block),
                    new SqlParameter("Village", Village),
                    new SqlParameter("Panchayat", Panchayat),
                    new SqlParameter("CensusYear", CensusYear),
                    new SqlParameter("Type1", '%'),
                    new SqlParameter("Type2", '%'),
                    new SqlParameter("Type3", '%'),
                    new SqlParameter("Type4", '%'),
                    new SqlParameter("Type5", '%'),
                    new SqlParameter("Active", ActiveType)
                    ).ToList<USP_MAS_DATA_MP_BLOCK_REPORT_Result>();

                totalRecords = lstMasterReport.Count();
                PropertyInfo propertyInfo = typeof(USP_MAS_DATA_MP_BLOCK_REPORT_Result).GetProperty(sidx.Trim());

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstMasterReport = lstMasterReport.OrderBy(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        lstMasterReport = lstMasterReport.OrderByDescending(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return lstMasterReport.Select(x => new
                {
                    id = x.MAST_BLOCK_CODE,
                    cell = new[]{
                        x.MAST_STATE_NAME,
                        x.MAST_MP_CONST_NAME,
                        x.MAST_BLOCK_NAME,
                        x.MAST_MP_CONST_ACTIVE
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


        public Array MLABlockListingDAL(int constCode, int stateCode, string activeType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSYEntities();
            int State = stateCode;
            int District = 0;
            int Block = 0;
            int Village = 0;
            int Panchayat = constCode;
            int CensusYear = 0;
            string ActiveType = activeType;
            try
            {
                List<USP_MAS_DATA_MLA_BLOCK_REPORT_Result> lstMasterReport = new List<USP_MAS_DATA_MLA_BLOCK_REPORT_Result>();
                lstMasterReport = dbContext.Database.SqlQuery<USP_MAS_DATA_MLA_BLOCK_REPORT_Result>(
                    "EXEC [omms].[USP_MAS_DATA_REPORT] @RptNo,@State,@District,@Block,@Village,@Panchayat,@CensusYear,@Type1,@Type2,@Type3,@Type4,@Type5,@Active",
                    new SqlParameter("RptNo", 9),
                    new SqlParameter("State", State),
                    new SqlParameter("District", District),
                    new SqlParameter("Block", Block),
                    new SqlParameter("Village", Village),
                    new SqlParameter("Panchayat", Panchayat),
                    new SqlParameter("CensusYear", CensusYear),
                    new SqlParameter("Type1", '%'),
                    new SqlParameter("Type2", '%'),
                    new SqlParameter("Type3", '%'),
                    new SqlParameter("Type4", '%'),
                    new SqlParameter("Type5", '%'),
                    new SqlParameter("Active", ActiveType)
                    ).ToList<USP_MAS_DATA_MLA_BLOCK_REPORT_Result>();

                totalRecords = lstMasterReport.Count();
                PropertyInfo propertyInfo = typeof(USP_MAS_DATA_MLA_BLOCK_REPORT_Result).GetProperty(sidx.Trim());

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstMasterReport = lstMasterReport.OrderBy(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        lstMasterReport = lstMasterReport.OrderByDescending(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return lstMasterReport.Select(x => new
                {
                    id = x.MAST_BLOCK_CODE,
                    cell = new[]{
                        x.MAST_STATE_NAME,
                        x.MAST_MLA_CONST_NAME,
                        x.MAST_BLOCK_NAME,
                        x.MAST_MLA_CONST_ACTIVE
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

        /// <summary>
        /// For Panchayat Drop Down
        /// </summary>
        /// <param name="blockCode"></param>
        /// <param name="districtCode"></param>
        /// <param name="stateCode"></param>
        /// <returns></returns>
        public List<SelectListItem> PopulatePanchayat(int blockCode, int districtCode, int stateCode)
        {
            dbContext = new PMGSYEntities();
            int State = stateCode;
            int District = districtCode;
            int Block = blockCode;
            int Village = 0;
            int Panchayat = 0;
            int CensusYear = 0;
            try
            {
                List<USP_MAS_PANCHAYAT_REPORT_Result> lstMasterReport = new List<USP_MAS_PANCHAYAT_REPORT_Result>();
                lstMasterReport = dbContext.Database.SqlQuery<USP_MAS_PANCHAYAT_REPORT_Result>(
                    "EXEC [omms].[USP_MAS_DATA_REPORT] @RptNo,@State,@District,@Block,@Village,@Panchayat,@CensusYear,@Type1,@Type2,@Type3,@Type4,@Type5,@Active",
                    new SqlParameter("RptNo", 10),
                    new SqlParameter("State", State),
                    new SqlParameter("District", District),
                    new SqlParameter("Block", Block),
                    new SqlParameter("Village", Village),
                    new SqlParameter("Panchayat", Panchayat),
                    new SqlParameter("CensusYear", CensusYear),
                    new SqlParameter("Type1", '%'),
                    new SqlParameter("Type2", '%'),
                    new SqlParameter("Type3", '%'),
                    new SqlParameter("Type4", '%'),
                    new SqlParameter("Type5", '%'),
                    new SqlParameter("Active", 'Y')
                    ).ToList<USP_MAS_PANCHAYAT_REPORT_Result>();

                List<SelectListItem> list = new List<SelectListItem>();

                list = lstMasterReport.Select(x => new SelectListItem
                {
                    Text = x.MAST_PANCHAYAT_NAME,
                    Value = x.MAST_PANCHAYAT_CODE.ToString()
                }).ToList<SelectListItem>();

                return list;

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

        /// <summary>
        /// For Village Drop Down
        /// </summary>
        /// <param name="blockCode"></param>
        /// <returns></returns>
        public List<SelectListItem> PopulateVillage(int blockCode)
        {
            try
            {
                dbContext = new PMGSYEntities();

                List<SelectListItem> list = new SelectList(dbContext.MASTER_VILLAGE.Where(c => c.MAST_BLOCK_CODE == blockCode), "MAST_VILLAGE_CODE", "MAST_VILLAGE_NAME").ToList();
                return list;
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

        /// <summary>
        /// For Census Year Drop Down
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> PopulateCensus()
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<SelectListItem> list = new SelectList(dbContext.MASTER_CENSUS_YEAR, "MAST_CENSUS_YEAR", "MAST_YEAR").ToList();

                return list;
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

        /// <summary>
        /// Populate MP Constituency List Statewise
        /// </summary>
        /// <param name="stateCode"></param>
        /// <returns></returns>
        public List<SelectListItem> PopulateMPConstituency(int stateCode)
        {
            dbContext = new PMGSYEntities();
            try
            {
                List<SelectListItem> list = dbContext.MASTER_MP_CONSTITUENCY.Where(x => x.MAST_STATE_CODE == stateCode).OrderBy(x=>x.MAST_MP_CONST_NAME).ToList().Select(x => new SelectListItem
                {
                    Value = x.MAST_MP_CONST_CODE.ToString(),
                    Text = x.MAST_MP_CONST_NAME
                }).ToList();
                return list;
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

        /// <summary>
        /// Populate MLA Constituency List Statewise
        /// </summary>
        /// <param name="stateCode"></param>
        /// <returns></returns>
        public List<SelectListItem> PopulateMLAConstituency(int stateCode)
        {
            dbContext = new PMGSYEntities();
            try
            {
                List<SelectListItem> list = dbContext.MASTER_MLA_CONSTITUENCY.Where(x => x.MAST_STATE_CODE == stateCode).OrderBy(x=>x.MAST_MLA_CONST_NAME).ToList().Select(x => new SelectListItem
                {
                    Value = x.MAST_MLA_CONST_CODE.ToString(),
                    Text = x.MAST_MLA_CONST_NAME
                }).ToList();
                return list;
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

        /// <summary>
        /// Populate Statewise Region
        /// </summary>
        /// <param name="stateCode"></param>
        /// <returns></returns>
        public List<SelectListItem> PopulateRegion(int stateCode)
        {
            dbContext = new PMGSYEntities();
            try
            {
                List<SelectListItem> list = dbContext.MASTER_REGION.Where(x => x.MAST_STATE_CODE == stateCode).OrderBy(m=>m.MAST_REGION_NAME).ToList().Select(x => new SelectListItem
                {
                    Text = x.MAST_REGION_NAME,
                    Value = x.MAST_REGION_CODE.ToString()
                }).ToList();

                return list;
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

        /// <summary>
        /// Populate Lok Sabha Term
        /// </summary>
        /// <returns>Lok Sabha Term List</returns>
        public List<SelectListItem> PopulateLokSabhaTerm()
        {
            dbContext = new PMGSYEntities();
            try
            {
                List<SelectListItem> list = dbContext.MASTER_LOK_SABHA_TERM.ToList().Select(x => new SelectListItem
                {
                    Text = x.MAST_LS_TERM.ToString(),
                    Value = x.MAST_LS_TERM.ToString()
                }).Distinct().OrderBy(x=>x.Text).ToList();

                return list;
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

        public List<SelectListItem> PopulateVidhanSabhaTerm(int stateCode)
        {
            dbContext = new PMGSYEntities();
            try
            {
        

                var lstTerms = (from item in dbContext.MASTER_VIDHAN_SABHA_TERM
                                where item.MAST_STATE_CODE == stateCode
                                select new
                                {
                                    TERM_CODE = item.MAST_VS_TERM,
                                    TERM_NAME = item.MAST_VS_TERM
                                }).Distinct().OrderBy(x=>x.TERM_NAME).ToList();

                List<SelectListItem> list = new SelectList(lstTerms.ToList(), "TERM_CODE", "TERM_NAME").ToList();



                return list;
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


        /// <summary>
        /// Populate Agency List Statewise
        /// </summary>
        /// <param name="stateCode"></param>
        /// <returns></returns>
        public List<SelectListItem> PopulateAgency(int stateCode)
        {
            dbContext = new PMGSYEntities();
            try
            {
                var listItem=(from am in dbContext.MASTER_AGENCY
                          join dp in dbContext.ADMIN_DEPARTMENT on am.MAST_AGENCY_CODE equals dp.MAST_AGENCY_CODE
                          where dp.MAST_STATE_CODE==stateCode
                          select new
                          {
                             Agency_Code=am.MAST_AGENCY_CODE,
                             Agency_Name=am.MAST_AGENCY_NAME
                          }).OrderBy(a=>a.Agency_Name).Distinct().ToList();

                List<SelectListItem> list = new SelectList(listItem.ToList(), "Agency_Code", "Agency_Name").ToList();
             
                return list;
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

        public Array PanchayatListingDAL(int blockCode, int districtCode, int stateCode, string activeType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSYEntities();
            int State = stateCode;
            int District = districtCode;
            int Block = blockCode;
            int Village = 0;
            int Panchayat = 0;
            int CensusYear = 0;
            string ActiveType = activeType;
            try
            {
                List<USP_MAS_PANCHAYAT_REPORT_Result> lstMasterReport = new List<USP_MAS_PANCHAYAT_REPORT_Result>();
                lstMasterReport = dbContext.Database.SqlQuery<USP_MAS_PANCHAYAT_REPORT_Result>(
                    "EXEC [omms].[USP_MAS_DATA_REPORT] @RptNo,@State,@District,@Block,@Village,@Panchayat,@CensusYear,@Type1,@Type2,@Type3,@Type4,@Type5,@Active",
                    new SqlParameter("RptNo", 10),
                    new SqlParameter("State", State),
                    new SqlParameter("District", District),
                    new SqlParameter("Block", Block),
                    new SqlParameter("Village", Village),
                    new SqlParameter("Panchayat", Panchayat),
                    new SqlParameter("CensusYear", CensusYear),
                    new SqlParameter("Type1", '%'),
                    new SqlParameter("Type2", '%'),
                    new SqlParameter("Type3", '%'),
                    new SqlParameter("Type4", '%'),
                    new SqlParameter("Type5", '%'),
                    new SqlParameter("Active", ActiveType)
                    ).ToList<USP_MAS_PANCHAYAT_REPORT_Result>();

                totalRecords = lstMasterReport.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx.Trim())
                        {
                            case "MAST_DISTRICT_NAME":
                                lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;

                            case "MAST_BLOCK_NAME":
                                lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;

                            case "MAST_STATE_NAME":
                                lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "MAST_PANCHAYAT_ACTIVE":
                                lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_PANCHAYAT_ACTIVE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;

                            default:
                                lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_PANCHAYAT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;

                        }

                     }
                    else
                    {
                        switch (sidx.Trim())
                        {
                            case "MAST_DISTRICT_NAME":
                                lstMasterReport = lstMasterReport.OrderByDescending(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;

                            case "MAST_BLOCK_NAME":
                                lstMasterReport = lstMasterReport.OrderByDescending(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;

                            case "MAST_STATE_NAME":
                                lstMasterReport = lstMasterReport.OrderByDescending(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "MAST_PANCHAYAT_ACTIVE":
                                lstMasterReport = lstMasterReport.OrderByDescending(x => x.MAST_PANCHAYAT_ACTIVE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            default:
                                lstMasterReport = lstMasterReport.OrderByDescending(x => x.MAST_PANCHAYAT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;

                        }
                    }
                }
                else
                {
                    lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_PANCHAYAT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return lstMasterReport.Select(x => new
                {
                    id = x.MAST_PANCHAYAT_CODE,
                    cell = new[]{
                        x.MAST_STATE_NAME,
                        x.MAST_DISTRICT_NAME,
                        x.MAST_BLOCK_NAME,
                        x.MAST_PANCHAYAT_NAME,
                        x.MAST_PANCHAYAT_ACTIVE
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


        public Array PanchayatHabitationListingDAL(int panchayatCode, int blockCode, int districtCode, int stateCode, string activeType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSYEntities();
            int State = stateCode;
            int District = districtCode;
            int Block = blockCode;
            int Village = 0;
            int Panchayat = panchayatCode;
            int CensusYear = 0;
            string ActiveType = activeType;
            try
            {
                List<USP_MAS_PANCHAYAT_HAB_REPORT_Result> lstMasterReport = new List<USP_MAS_PANCHAYAT_HAB_REPORT_Result>();
                lstMasterReport = dbContext.Database.SqlQuery<USP_MAS_PANCHAYAT_HAB_REPORT_Result>(
                    "EXEC [omms].[USP_MAS_DATA_REPORT] @RptNo,@State,@District,@Block,@Village,@Panchayat,@CensusYear,@Type1,@Type2,@Type3,@Type4,@Type5,@Active",
                    new SqlParameter("RptNo", 11),
                    new SqlParameter("State", State),
                    new SqlParameter("District", District),
                    new SqlParameter("Block", Block),
                    new SqlParameter("Village", Village),
                    new SqlParameter("Panchayat", Panchayat),
                    new SqlParameter("CensusYear", CensusYear),
                    new SqlParameter("Type1", '%'),
                    new SqlParameter("Type2", '%'),
                    new SqlParameter("Type3", '%'),
                    new SqlParameter("Type4", '%'),
                    new SqlParameter("Type5", '%'),
                    new SqlParameter("Active", ActiveType)
                    ).ToList<USP_MAS_PANCHAYAT_HAB_REPORT_Result>();

                totalRecords = lstMasterReport.Count();
                PropertyInfo propertyInfo = typeof(USP_MAS_PANCHAYAT_HAB_REPORT_Result).GetProperty(sidx.Trim());

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstMasterReport = lstMasterReport.OrderBy(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        lstMasterReport = lstMasterReport.OrderByDescending(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return lstMasterReport.Select(x => new
                {
                    id = x.MAST_HAB_CODE,
                    cell = new[]{
                        x.MAST_STATE_NAME,
                        x.MAST_DISTRICT_NAME,
                        x.MAST_BLOCK_NAME,
                        x.MAST_PANCHAYAT_NAME,
                        x.MAST_HAB_NAME,
                        x.MAST_PANCHAYAT_ACTIVE
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


        public Array RegionListingDAL(int stateCode, string activeType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSYEntities();
            int State = stateCode;
            int District = 0;
            int Block = 0;
            int Village = 0;
            int Panchayat = 0;
            int CensusYear = 0;
            string ActiveType = activeType;
            try
            {
                List<USP_MAS_REGION_REPORT_Result> lstMasterReport = new List<USP_MAS_REGION_REPORT_Result>();
                lstMasterReport = dbContext.Database.SqlQuery<USP_MAS_REGION_REPORT_Result>(
                    "EXEC [omms].[USP_MAS_DATA_REPORT] @RptNo,@State,@District,@Block,@Village,@Panchayat,@CensusYear,@Type1,@Type2,@Type3,@Type4,@Type5,@Active",
                    new SqlParameter("RptNo", 12),
                    new SqlParameter("State", State),
                    new SqlParameter("District", District),
                    new SqlParameter("Block", Block),
                    new SqlParameter("Village", Village),
                    new SqlParameter("Panchayat", Panchayat),
                    new SqlParameter("CensusYear", CensusYear),
                    new SqlParameter("Type1", '%'),
                    new SqlParameter("Type2", '%'),
                    new SqlParameter("Type3", '%'),
                    new SqlParameter("Type4", '%'),
                    new SqlParameter("Type5", '%'),
                    new SqlParameter("Active", ActiveType)
                    ).ToList<USP_MAS_REGION_REPORT_Result>();

                totalRecords = lstMasterReport.Count();
                PropertyInfo propertyInfo = typeof(USP_MAS_REGION_REPORT_Result).GetProperty(sidx.Trim());

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstMasterReport = lstMasterReport.OrderBy(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        lstMasterReport = lstMasterReport.OrderByDescending(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_REGION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return lstMasterReport.Select(x => new
                {
                    id = x.MAST_REGION_CODE,
                    cell = new[]{
                        x.MAST_STATE_NAME,
                        x.MAST_REGION_NAME,
                        x.MAST_REGION_ACTIVE
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


        public Array RegionDistrictListingDAL(int regionCode, int stateCode, string activeType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSYEntities();
            int State = stateCode;
            int District = 0;
            int Block = 0;
            int Village = 0;
            int Panchayat = regionCode;
            int CensusYear = 0;
            string ActiveType = activeType;
            try
            {
                List<USP_MAS_REGION_DISTRICT_REPORT_Result> lstMasterReport = new List<USP_MAS_REGION_DISTRICT_REPORT_Result>();
                lstMasterReport = dbContext.Database.SqlQuery<USP_MAS_REGION_DISTRICT_REPORT_Result>(
                    "EXEC [omms].[USP_MAS_DATA_REPORT] @RptNo,@State,@District,@Block,@Village,@Panchayat,@CensusYear,@Type1,@Type2,@Type3,@Type4,@Type5,@Active",
                    new SqlParameter("RptNo", 13),
                    new SqlParameter("State", State),
                    new SqlParameter("District", District),
                    new SqlParameter("Block", Block),
                    new SqlParameter("Village", Village),
                    new SqlParameter("Panchayat", Panchayat),
                    new SqlParameter("CensusYear", CensusYear),
                    new SqlParameter("Type1", '%'),
                    new SqlParameter("Type2", '%'),
                    new SqlParameter("Type3", '%'),
                    new SqlParameter("Type4", '%'),
                    new SqlParameter("Type5", '%'),
                    new SqlParameter("Active",ActiveType)
                    ).ToList<USP_MAS_REGION_DISTRICT_REPORT_Result>();

                totalRecords = lstMasterReport.Count();
                PropertyInfo propertyInfo = typeof(USP_MAS_REGION_DISTRICT_REPORT_Result).GetProperty(sidx.Trim());

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstMasterReport = lstMasterReport.OrderBy(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        lstMasterReport = lstMasterReport.OrderByDescending(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return lstMasterReport.Select(x => new
                {
                    id = x.MAST_DISTRICT_CODE,
                    cell = new[]{
                        x.MAST_STATE_NAME,
                        x.MAST_REGION_NAME,
                        x.MAST_DISTRICT_NAME,
                        x.MAST_REGION_ACTIVE                        
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


        public Array UnitListingDAL(int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSYEntities();
            int State = 0;
            int District = 0;
            int Block = 0;
            int Village = 0;
            int Panchayat = 0;
            int CensusYear = 0;
            try
            {
                List<USP_MAS_UNIT_REPORT_Result> lstMasterReport = new List<USP_MAS_UNIT_REPORT_Result>();
                lstMasterReport = dbContext.Database.SqlQuery<USP_MAS_UNIT_REPORT_Result>(
                    "EXEC [omms].[USP_MAS_DATA_REPORT] @RptNo,@State,@District,@Block,@Village,@Panchayat,@CensusYear,@Type1,@Type2,@Type3,@Type4,@Type5,@Active",
                    new SqlParameter("RptNo", 14),
                    new SqlParameter("State", State),
                    new SqlParameter("District", District),
                    new SqlParameter("Block", Block),
                    new SqlParameter("Village", Village),
                    new SqlParameter("Panchayat", Panchayat),
                    new SqlParameter("CensusYear", CensusYear),
                    new SqlParameter("Type1", '%'),
                    new SqlParameter("Type2", '%'),
                    new SqlParameter("Type3", '%'),
                    new SqlParameter("Type4", '%'),
                    new SqlParameter("Type5", '%'),
                    new SqlParameter("Active", 'Y')
                    ).ToList<USP_MAS_UNIT_REPORT_Result>();

                totalRecords = lstMasterReport.Count();
                PropertyInfo propertyInfo = typeof(USP_MAS_UNIT_REPORT_Result).GetProperty(sidx.Trim());

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstMasterReport = lstMasterReport.OrderBy(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        lstMasterReport = lstMasterReport.OrderByDescending(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_UNIT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return lstMasterReport.Select(x => new
                {
                    id = x.MAST_UNIT_CODE,
                    cell = new[]{
                        x.MAST_UNIT_NAME,
                        x.MAST_UNIT_SHORT_NAME,
                        x.MAST_UNIT_DIMENSION.ToString()
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


        public Array RoadCategoryListingDAL(int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSYEntities();
            int State = 0;
            int District = 0;
            int Block = 0;
            int Village = 0;
            int Panchayat = 0;
            int CensusYear = 0;
            try
            {
                List<USP_MAS_ROAD_CATEGORY_REPORT_Result> lstMasterReport = new List<USP_MAS_ROAD_CATEGORY_REPORT_Result>();
                lstMasterReport = dbContext.Database.SqlQuery<USP_MAS_ROAD_CATEGORY_REPORT_Result>(
                    "EXEC [omms].[USP_MAS_DATA_REPORT] @RptNo,@State,@District,@Block,@Village,@Panchayat,@CensusYear,@Type1,@Type2,@Type3,@Type4,@Type5,@Active",
                    new SqlParameter("RptNo", 15),
                    new SqlParameter("State", State),
                    new SqlParameter("District", District),
                    new SqlParameter("Block", Block),
                    new SqlParameter("Village", Village),
                    new SqlParameter("Panchayat", Panchayat),
                    new SqlParameter("CensusYear", CensusYear),
                    new SqlParameter("Type1", '%'),
                    new SqlParameter("Type2", '%'),
                    new SqlParameter("Type3", '%'),
                    new SqlParameter("Type4", '%'),
                    new SqlParameter("Type5", '%'),
                    new SqlParameter("Active", 'Y')
                    ).ToList<USP_MAS_ROAD_CATEGORY_REPORT_Result>();

                totalRecords = lstMasterReport.Count();
                PropertyInfo propertyInfo = typeof(USP_MAS_ROAD_CATEGORY_REPORT_Result).GetProperty(sidx.Trim());

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstMasterReport = lstMasterReport.OrderBy(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        lstMasterReport = lstMasterReport.OrderByDescending(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_ROAD_CAT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return lstMasterReport.Select(x => new
                {
                    id = x.MAST_ROAD_CAT_CODE,
                    cell = new[]{
                        x.MAST_ROAD_CAT_NAME,
                        x.MAST_ROAD_SHORT_DESC
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


        public Array ScourFoundationListingDAL(string scourFoundationType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSYEntities();
            int State = 0;
            int District = 0;
            int Block = 0;
            int Village = 0;
            int Panchayat = 0;
            int CensusYear = 0;
            string Type1 = scourFoundationType == "All" ? "%" : scourFoundationType;
            try
            {
                List<USP_MAS_SCOUR_FOUNDATION_REPORT_Result> lstMasterReport = new List<USP_MAS_SCOUR_FOUNDATION_REPORT_Result>();
                lstMasterReport = dbContext.Database.SqlQuery<USP_MAS_SCOUR_FOUNDATION_REPORT_Result>(
                    "EXEC [omms].[USP_MAS_DATA_REPORT] @RptNo,@State,@District,@Block,@Village,@Panchayat,@CensusYear,@Type1,@Type2,@Type3,@Type4,@Type5,@Active",
                    new SqlParameter("RptNo", 16),
                    new SqlParameter("State", State),
                    new SqlParameter("District", District),
                    new SqlParameter("Block", Block),
                    new SqlParameter("Village", Village),
                    new SqlParameter("Panchayat", Panchayat),
                    new SqlParameter("CensusYear", CensusYear),
                    new SqlParameter("Type1", Type1),
                    new SqlParameter("Type2", '%'),
                    new SqlParameter("Type3", '%'),
                    new SqlParameter("Type4", '%'),
                    new SqlParameter("Type5", '%'),
                    new SqlParameter("Active", 'Y')
                    ).ToList<USP_MAS_SCOUR_FOUNDATION_REPORT_Result>();

                totalRecords = lstMasterReport.Count();
                PropertyInfo propertyInfo = typeof(USP_MAS_SCOUR_FOUNDATION_REPORT_Result).GetProperty(sidx.Trim());

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstMasterReport = lstMasterReport.OrderBy(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        lstMasterReport = lstMasterReport.OrderByDescending(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    lstMasterReport = lstMasterReport.OrderBy(x => x.IMS_SC_FD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return lstMasterReport.Select(x => new
                {
                    id = x.IMS_SC_FD_CODE,
                    cell = new[]{
                        x.IMS_SC_FD_NAME,
                        x.IMS_SC_FD_TYPE
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


        public Array SoilListingDAL(int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSYEntities();
            int State = 0;
            int District = 0;
            int Block = 0;
            int Village = 0;
            int Panchayat = 0;
            int CensusYear = 0;
            try
            {
                List<USP_MAS_SOIL_REPORT_Result> lstMasterReport = new List<USP_MAS_SOIL_REPORT_Result>();
                lstMasterReport = dbContext.Database.SqlQuery<USP_MAS_SOIL_REPORT_Result>(
                    "EXEC [omms].[USP_MAS_DATA_REPORT] @RptNo,@State,@District,@Block,@Village,@Panchayat,@CensusYear,@Type1,@Type2,@Type3,@Type4,@Type5,@Active",
                    new SqlParameter("RptNo", 17),
                    new SqlParameter("State", State),
                    new SqlParameter("District", District),
                    new SqlParameter("Block", Block),
                    new SqlParameter("Village", Village),
                    new SqlParameter("Panchayat", Panchayat),
                    new SqlParameter("CensusYear", CensusYear),
                    new SqlParameter("Type1", '%'),
                    new SqlParameter("Type2", '%'),
                    new SqlParameter("Type3", '%'),
                    new SqlParameter("Type4", '%'),
                    new SqlParameter("Type5", '%'),
                    new SqlParameter("Active", 'Y')
                    ).ToList<USP_MAS_SOIL_REPORT_Result>();

                totalRecords = lstMasterReport.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_SOIL_TYPE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        lstMasterReport = lstMasterReport.OrderByDescending(x => x.MAST_SOIL_TYPE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_SOIL_TYPE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return lstMasterReport.Select(x => new
                {
                    id = x.MAST_SOIL_TYPE_CODE,
                    cell = new[]{
                        x.MAST_SOIL_TYPE_NAME
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


        public Array StreamListingDAL(string streamType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSYEntities();
            int State = 0;
            int District = 0;
            int Block = 0;
            int Village = 0;
            int Panchayat = 0;
            int CensusYear = 0;
            string Type1 = streamType == "All" ? "%" : streamType;
            try
            {
                List<USP_MAS_STREAM_REPORT_Result> lstMasterReport = new List<USP_MAS_STREAM_REPORT_Result>();
                lstMasterReport = dbContext.Database.SqlQuery<USP_MAS_STREAM_REPORT_Result>(
                    "EXEC [omms].[USP_MAS_DATA_REPORT] @RptNo,@State,@District,@Block,@Village,@Panchayat,@CensusYear,@Type1,@Type2,@Type3,@Type4,@Type5,@Active",
                    new SqlParameter("RptNo", 18),
                    new SqlParameter("State", State),
                    new SqlParameter("District", District),
                    new SqlParameter("Block", Block),
                    new SqlParameter("Village", Village),
                    new SqlParameter("Panchayat", Panchayat),
                    new SqlParameter("CensusYear", CensusYear),
                    new SqlParameter("Type1", Type1),
                    new SqlParameter("Type2", '%'),
                    new SqlParameter("Type3", '%'),
                    new SqlParameter("Type4", '%'),
                    new SqlParameter("Type5", '%'),
                    new SqlParameter("Active", 'Y')
                    ).ToList<USP_MAS_STREAM_REPORT_Result>();

                totalRecords = lstMasterReport.Count();
                PropertyInfo propertyInfo = typeof(USP_MAS_STREAM_REPORT_Result).GetProperty(sidx.Trim());

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstMasterReport = lstMasterReport.OrderBy(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        lstMasterReport = lstMasterReport.OrderByDescending(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_STREAM_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return lstMasterReport.Select(x => new
                {
                    id = x.MAST_STREAM_CODE,
                    cell = new[]{
                        x.MAST_STREAM_NAME,
                        x.MAST_STREAM_TYPE
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


        public Array TerrainListingDAL(int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSYEntities();
            int State = 0;
            int District = 0;
            int Block = 0;
            int Village = 0;
            int Panchayat = 0;
            int CensusYear = 0;
            try
            {
                List<USP_MAS_TERRIAN_REPORT_Result> lstMasterReport = new List<USP_MAS_TERRIAN_REPORT_Result>();
                lstMasterReport = dbContext.Database.SqlQuery<USP_MAS_TERRIAN_REPORT_Result>(
                    "EXEC [omms].[USP_MAS_DATA_REPORT] @RptNo,@State,@District,@Block,@Village,@Panchayat,@CensusYear,@Type1,@Type2,@Type3,@Type4,@Type5,@Active",
                    new SqlParameter("RptNo", 19),
                    new SqlParameter("State", State),
                    new SqlParameter("District", District),
                    new SqlParameter("Block", Block),
                    new SqlParameter("Village", Village),
                    new SqlParameter("Panchayat", Panchayat),
                    new SqlParameter("CensusYear", CensusYear),
                    new SqlParameter("Type1", '%'),
                    new SqlParameter("Type2", '%'),
                    new SqlParameter("Type3", '%'),
                    new SqlParameter("Type4", '%'),
                    new SqlParameter("Type5", '%'),
                    new SqlParameter("Active", 'Y')
                    ).ToList<USP_MAS_TERRIAN_REPORT_Result>();

                totalRecords = lstMasterReport.Count();
                PropertyInfo propertyInfo = typeof(USP_MAS_TERRIAN_REPORT_Result).GetProperty(sidx.Trim());

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstMasterReport = lstMasterReport.OrderBy(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        lstMasterReport = lstMasterReport.OrderByDescending(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_TERRAIN_TYPE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return lstMasterReport.Select(x => new
                {
                    id = x.MAST_TERRAIN_TYPE_CODE,
                    cell = new[]{
                        x.MAST_TERRAIN_TYPE_NAME,
                        x.MAST_TERRAIN_ROADWAY_WIDTH.ToString(),
                        x.MAST_TERRAIN_SLOP_FROM == null ? "0": x.MAST_TERRAIN_SLOP_FROM.ToString(),
                        x.MAST_TERRAIN_SLOP_TO == null ? "0": x.MAST_TERRAIN_SLOP_TO.ToString()
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



        public Array CDWorksLengthListingDAL(int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSYEntities();
            int State = 0;
            int District = 0;
            int Block = 0;
            int Village = 0;
            int Panchayat = 0;
            int CensusYear = 0;
            try
            {
                List<USP_MAS_CD_WORKS_LENGTH_REPORT_Result> lstMasterReport = new List<USP_MAS_CD_WORKS_LENGTH_REPORT_Result>();
                lstMasterReport = dbContext.Database.SqlQuery<USP_MAS_CD_WORKS_LENGTH_REPORT_Result>(
                    "EXEC [omms].[USP_MAS_DATA_REPORT] @RptNo,@State,@District,@Block,@Village,@Panchayat,@CensusYear,@Type1,@Type2,@Type3,@Type4,@Type5,@Active",
                    new SqlParameter("RptNo", 20),
                    new SqlParameter("State", State),
                    new SqlParameter("District", District),
                    new SqlParameter("Block", Block),
                    new SqlParameter("Village", Village),
                    new SqlParameter("Panchayat", Panchayat),
                    new SqlParameter("CensusYear", CensusYear),
                    new SqlParameter("Type1", '%'),
                    new SqlParameter("Type2", '%'),
                    new SqlParameter("Type3", '%'),
                    new SqlParameter("Type4", '%'),
                    new SqlParameter("Type5", '%'),
                    new SqlParameter("Active", 'Y')
                    ).ToList<USP_MAS_CD_WORKS_LENGTH_REPORT_Result>();

                totalRecords = lstMasterReport.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_CDWORKS_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        lstMasterReport = lstMasterReport.OrderByDescending(x => x.MAST_CDWORKS_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_CDWORKS_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return lstMasterReport.Select(x => new
                {
                    id = x.MAST_CDWORKS_CODE,
                    cell = new[]{
                        x.MAST_CDWORKS_NAME,
                       
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


        public Array CDWorksTypeListingDAL(int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSYEntities();
            int State = 0;
            int District = 0;
            int Block = 0;
            int Village = 0;
            int Panchayat = 0;
            int CensusYear = 0;
            try
            {
                List<USP_MAS_CD_WORKS_TYPE_REPORT_Result> lstMasterReport = new List<USP_MAS_CD_WORKS_TYPE_REPORT_Result>();
                lstMasterReport = dbContext.Database.SqlQuery<USP_MAS_CD_WORKS_TYPE_REPORT_Result>(
                    "EXEC [omms].[USP_MAS_DATA_REPORT] @RptNo,@State,@District,@Block,@Village,@Panchayat,@CensusYear,@Type1,@Type2,@Type3,@Type4,@Type5,@Active",
                    new SqlParameter("RptNo", 21),
                    new SqlParameter("State", State),
                    new SqlParameter("District", District),
                    new SqlParameter("Block", Block),
                    new SqlParameter("Village", Village),
                    new SqlParameter("Panchayat", Panchayat),
                    new SqlParameter("CensusYear", CensusYear),
                    new SqlParameter("Type1", '%'),
                    new SqlParameter("Type2", '%'),
                    new SqlParameter("Type3", '%'),
                    new SqlParameter("Type4", '%'),
                    new SqlParameter("Type5", '%'),
                    new SqlParameter("Active", 'Y')
                    ).ToList<USP_MAS_CD_WORKS_TYPE_REPORT_Result>();

                totalRecords = lstMasterReport.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_CDWORKS_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        lstMasterReport = lstMasterReport.OrderByDescending(x => x.MAST_CDWORKS_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_CDWORKS_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return lstMasterReport.Select(x => new
                {
                    id = x.MAST_CDWORKS_CODE,
                    cell = new[]{
                        x.MAST_CDWORKS_NAME,
                       
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


        public Array ComponentListingDAL(int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSYEntities();
            int State = 0;
            int District = 0;
            int Block = 0;
            int Village = 0;
            int Panchayat = 0;
            int CensusYear = 0;
            try
            {
                List<USP_MAS_COMPONENT_REPORT_Result> lstMasterReport = new List<USP_MAS_COMPONENT_REPORT_Result>();
                lstMasterReport = dbContext.Database.SqlQuery<USP_MAS_COMPONENT_REPORT_Result>(
                    "EXEC [omms].[USP_MAS_DATA_REPORT] @RptNo,@State,@District,@Block,@Village,@Panchayat,@CensusYear,@Type1,@Type2,@Type3,@Type4,@Type5,@Active",
                    new SqlParameter("RptNo", 22),
                    new SqlParameter("State", State),
                    new SqlParameter("District", District),
                    new SqlParameter("Block", Block),
                    new SqlParameter("Village", Village),
                    new SqlParameter("Panchayat", Panchayat),
                    new SqlParameter("CensusYear", CensusYear),
                    new SqlParameter("Type1", '%'),
                    new SqlParameter("Type2", '%'),
                    new SqlParameter("Type3", '%'),
                    new SqlParameter("Type4", '%'),
                    new SqlParameter("Type5", '%'),
                    new SqlParameter("Active", 'Y')
                    ).ToList<USP_MAS_COMPONENT_REPORT_Result>();

                totalRecords = lstMasterReport.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_COMPONENT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        lstMasterReport = lstMasterReport.OrderByDescending(x => x.MAST_COMPONENT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_COMPONENT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return lstMasterReport.Select(x => new
                {
                    id = x.MAST_COMPONENT_CODE,
                    cell = new[]{
                        x.MAST_COMPONENT_NAME,
                       
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


        public Array GradeListingDAL(int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSYEntities();
            int State = 0;
            int District = 0;
            int Block = 0;
            int Village = 0;
            int Panchayat = 0;
            int CensusYear = 0;
            try
            {
                List<USP_MAS_GRADE_REPORT_Result> lstMasterReport = new List<USP_MAS_GRADE_REPORT_Result>();
                lstMasterReport = dbContext.Database.SqlQuery<USP_MAS_GRADE_REPORT_Result>(
                    "EXEC [omms].[USP_MAS_DATA_REPORT] @RptNo,@State,@District,@Block,@Village,@Panchayat,@CensusYear,@Type1,@Type2,@Type3,@Type4,@Type5,@Active",
                    new SqlParameter("RptNo", 23),
                    new SqlParameter("State", State),
                    new SqlParameter("District", District),
                    new SqlParameter("Block", Block),
                    new SqlParameter("Village", Village),
                    new SqlParameter("Panchayat", Panchayat),
                    new SqlParameter("CensusYear", CensusYear),
                    new SqlParameter("Type1", '%'),
                    new SqlParameter("Type2", '%'),
                    new SqlParameter("Type3", '%'),
                    new SqlParameter("Type4", '%'),
                    new SqlParameter("Type5", '%'),
                    new SqlParameter("Active", 'Y')
                    ).ToList<USP_MAS_GRADE_REPORT_Result>();

                totalRecords = lstMasterReport.Count();
                PropertyInfo propertyInfo = typeof(USP_MAS_GRADE_REPORT_Result).GetProperty(sidx.Trim());

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstMasterReport = lstMasterReport.OrderBy(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        lstMasterReport = lstMasterReport.OrderByDescending(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_GRADE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return lstMasterReport.Select(x => new
                {
                    id = x.MAST_GRADE_CODE,
                    cell = new[]{
                        x.MAST_GRADE_NAME,
                        x.MAST_GRADE_SHORT_NAME
                       
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


        public Array DesignationListingDAL(string designationType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSYEntities();
            int State = 0;
            int District = 0;
            int Block = 0;
            int Village = 0;
            int Panchayat = 0;
            int CensusYear = 0;
            string Type5 = designationType == "%" ? "%%" : designationType.Trim();
            try
            {
                List<USP_MAS_DESIGNATION_REPORT_Result> lstMasterReport = new List<USP_MAS_DESIGNATION_REPORT_Result>();
                lstMasterReport = dbContext.Database.SqlQuery<USP_MAS_DESIGNATION_REPORT_Result>(
                    "EXEC [omms].[USP_MAS_DATA_REPORT] @RptNo,@State,@District,@Block,@Village,@Panchayat,@CensusYear,@Type1,@Type2,@Type3,@Type4,@Type5,@Active",
                    new SqlParameter("RptNo", 24),
                    new SqlParameter("State", State),
                    new SqlParameter("District", District),
                    new SqlParameter("Block", Block),
                    new SqlParameter("Village", Village),
                    new SqlParameter("Panchayat", Panchayat),
                    new SqlParameter("CensusYear", CensusYear),
                    new SqlParameter("Type1",'%'),
                    new SqlParameter("Type2",'%'),
                    new SqlParameter("Type3",'%'),
                    new SqlParameter("Type4",'%'),
                    new SqlParameter("Type5",Type5),
                    new SqlParameter("Active", 'Y')
                    ).ToList<USP_MAS_DESIGNATION_REPORT_Result>();

                totalRecords = lstMasterReport.Count();
                PropertyInfo propertyInfo = typeof(USP_MAS_DESIGNATION_REPORT_Result).GetProperty(sidx.Trim());

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstMasterReport = lstMasterReport.OrderBy(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        lstMasterReport = lstMasterReport.OrderByDescending(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_DESIG_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return lstMasterReport.Select(x => new
                {
                    id = x.MAST_DESIG_CODE,
                    cell = new[]{
                        x.MAST_DESIG_NAME,
                        x.MAST_DESIG_TYPE
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


        public Array ReasonListingDAL(string reasonType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSYEntities();
            int State = 0;
            int District = 0;
            int Block = 0;
            int Village = 0;
            int Panchayat = 0;
            int CensusYear = 0;
            string Type1 = reasonType == "All" ? "%" : reasonType;
            try
            {
                List<USP_MAS_REASON_REPORT_Result> lstMasterReport = new List<USP_MAS_REASON_REPORT_Result>();
                lstMasterReport = dbContext.Database.SqlQuery<USP_MAS_REASON_REPORT_Result>(
                    "EXEC [omms].[USP_MAS_DATA_REPORT] @RptNo,@State,@District,@Block,@Village,@Panchayat,@CensusYear,@Type1,@Type2,@Type3,@Type4,@Type5,@Active",
                    new SqlParameter("RptNo", 25),
                    new SqlParameter("State", State),
                    new SqlParameter("District", District),
                    new SqlParameter("Block", Block),
                    new SqlParameter("Village", Village),
                    new SqlParameter("Panchayat", Panchayat),
                    new SqlParameter("CensusYear", CensusYear),
                    new SqlParameter("Type1", Type1),
                    new SqlParameter("Type2", '%'),
                    new SqlParameter("Type3", '%'),
                    new SqlParameter("Type4", '%'),
                    new SqlParameter("Type5", '%'),
                    new SqlParameter("Active", 'Y')
                    ).ToList<USP_MAS_REASON_REPORT_Result>();

                totalRecords = lstMasterReport.Count();
                PropertyInfo propertyInfo = typeof(USP_MAS_REASON_REPORT_Result).GetProperty(sidx.Trim());

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstMasterReport = lstMasterReport.OrderBy(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        lstMasterReport = lstMasterReport.OrderByDescending(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_REASON_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return lstMasterReport.Select(x => new
                {
                    id = x.MAST_REASON_CODE,
                    cell = new[]{
                        x.MAST_REASON_NAME,
                        x.MAST_REASON_TYPE
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


        public Array CheckListPointListingDAL(string checkListActive, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSYEntities();
            int State = 0;
            int District = 0;
            int Block = 0;
            int Village = 0;
            int Panchayat = 0;
            int CensusYear = 0;
            string Type1 = checkListActive == "All" ? "%" : checkListActive;
            try
            {
                List<USP_MAS_CHECKLIST_POINT_REPORT_Result> lstMasterReport = new List<USP_MAS_CHECKLIST_POINT_REPORT_Result>();
                lstMasterReport = dbContext.Database.SqlQuery<USP_MAS_CHECKLIST_POINT_REPORT_Result>(
                    "EXEC [omms].[USP_MAS_DATA_REPORT] @RptNo,@State,@District,@Block,@Village,@Panchayat,@CensusYear,@Type1,@Type2,@Type3,@Type4,@Type5,@Active",
                    new SqlParameter("RptNo", 26),
                    new SqlParameter("State", State),
                    new SqlParameter("District", District),
                    new SqlParameter("Block", Block),
                    new SqlParameter("Village", Village),
                    new SqlParameter("Panchayat", Panchayat),
                    new SqlParameter("CensusYear", CensusYear),
                    new SqlParameter("Type1", Type1),
                    new SqlParameter("Type2", '%'),
                    new SqlParameter("Type3", '%'),
                    new SqlParameter("Type4", '%'),
                    new SqlParameter("Type5", '%'),
                    new SqlParameter("Active", 'Y')
                    ).ToList<USP_MAS_CHECKLIST_POINT_REPORT_Result>();

                totalRecords = lstMasterReport.Count();
                PropertyInfo propertyInfo = typeof(USP_MAS_CHECKLIST_POINT_REPORT_Result).GetProperty(sidx.Trim());

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstMasterReport = lstMasterReport.OrderBy(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        lstMasterReport = lstMasterReport.OrderByDescending(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_CHECKLIST_ISSUES).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return lstMasterReport.Select(x => new
                {
                    id = x.MAST_CHECKLIST_POINTID,
                    cell = new[]{
                        x.MAST_CHECKLIST_ISSUES,
                        x.MAST_CHECKLIST_ACTIVE
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


        public Array ExecutionItemListingDAL(string headType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSYEntities();
            int State = 0;
            int District = 0;
            int Block = 0;
            int Village = 0;
            int Panchayat = 0;
            int CensusYear = 0;
            string Type1 = headType == "All" ? "%" : headType;
            try
            {
                List<USP_MAS_EXECUTION_ITEM_REPORT_Result> lstMasterReport = new List<USP_MAS_EXECUTION_ITEM_REPORT_Result>();
                lstMasterReport = dbContext.Database.SqlQuery<USP_MAS_EXECUTION_ITEM_REPORT_Result>(
                    "EXEC [omms].[USP_MAS_DATA_REPORT] @RptNo,@State,@District,@Block,@Village,@Panchayat,@CensusYear,@Type1,@Type2,@Type3,@Type4,@Type5,@Active",
                    new SqlParameter("RptNo", 27),
                    new SqlParameter("State", State),
                    new SqlParameter("District", District),
                    new SqlParameter("Block", Block),
                    new SqlParameter("Village", Village),
                    new SqlParameter("Panchayat", Panchayat),
                    new SqlParameter("CensusYear", CensusYear),
                    new SqlParameter("Type1", Type1),
                    new SqlParameter("Type2", '%'),
                    new SqlParameter("Type3", '%'),
                    new SqlParameter("Type4", '%'),
                    new SqlParameter("Type5", '%'),
                    new SqlParameter("Active", 'Y')
                    ).ToList<USP_MAS_EXECUTION_ITEM_REPORT_Result>();

                totalRecords = lstMasterReport.Count();
                PropertyInfo propertyInfo = typeof(USP_MAS_EXECUTION_ITEM_REPORT_Result).GetProperty(sidx.Trim());

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstMasterReport = lstMasterReport.OrderBy(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        lstMasterReport = lstMasterReport.OrderByDescending(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_HEAD_DESC).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return lstMasterReport.Select(x => new
                {
                    id = x.MAST_HEAD_CODE,
                    cell = new[]{
                        x.MAST_HEAD_DESC,
                        x.MAST_HEAD_SH_DESC,
                        x.MAST_HEAD_TYPE
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


        public Array TaxesListingDAL(int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSYEntities();
            int State = 0;
            int District = 0;
            int Block = 0;
            int Village = 0;
            int Panchayat = 0;
            int CensusYear = 0;
            try
            {
                List<USP_MAS_TAXES_REPORT_Result> lstMasterReport = new List<USP_MAS_TAXES_REPORT_Result>();
                lstMasterReport = dbContext.Database.SqlQuery<USP_MAS_TAXES_REPORT_Result>(
                    "EXEC [omms].[USP_MAS_DATA_REPORT] @RptNo,@State,@District,@Block,@Village,@Panchayat,@CensusYear,@Type1,@Type2,@Type3,@Type4,@Type5,@Active",
                    new SqlParameter("RptNo", 30),
                    new SqlParameter("State", State),
                    new SqlParameter("District", District),
                    new SqlParameter("Block", Block),
                    new SqlParameter("Village", Village),
                    new SqlParameter("Panchayat", Panchayat),
                    new SqlParameter("CensusYear", CensusYear),
                    new SqlParameter("Type1", '%'),
                    new SqlParameter("Type2", '%'),
                    new SqlParameter("Type3", '%'),
                    new SqlParameter("Type4", '%'),
                    new SqlParameter("Type5", '%'),
                    new SqlParameter("Active", 'Y')
                    ).ToList<USP_MAS_TAXES_REPORT_Result>();

                totalRecords = lstMasterReport.Count();
                PropertyInfo propertyInfo = typeof(USP_MAS_TAXES_REPORT_Result).GetProperty(sidx.Trim());

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstMasterReport = lstMasterReport.OrderBy(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        lstMasterReport = lstMasterReport.OrderByDescending(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_TDS).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return lstMasterReport.Select(x => new
                {
                    id = x.MAST_TDS_ID,
                    cell = new[]{
                        x.MAST_TDS.ToString(),
                        x.MAST_TDS_SC.ToString(),
                        x.MAST_EFFECTIVE_DATE.ToString()
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


        public Array AgencyListingDAL(string agencyType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSYEntities();
            int State = 0;
            int District = 0;
            int Block = 0;
            int Village = 0;
            int Panchayat = 0;
            int CensusYear = 0;
            string Type1 = agencyType == "All" ? "%" : agencyType;
            try
            {
                List<USP_MAS_AGENCY_REPORT_Result> lstMasterReport = new List<USP_MAS_AGENCY_REPORT_Result>();
                lstMasterReport = dbContext.Database.SqlQuery<USP_MAS_AGENCY_REPORT_Result>(
                    "EXEC [omms].[USP_MAS_DATA_REPORT] @RptNo,@State,@District,@Block,@Village,@Panchayat,@CensusYear,@Type1,@Type2,@Type3,@Type4,@Type5,@Active",
                    new SqlParameter("RptNo", 34),
                    new SqlParameter("State", State),
                    new SqlParameter("District", District),
                    new SqlParameter("Block", Block),
                    new SqlParameter("Village", Village),
                    new SqlParameter("Panchayat", Panchayat),
                    new SqlParameter("CensusYear", CensusYear),
                    new SqlParameter("Type1", Type1),
                    new SqlParameter("Type2", '%'),
                    new SqlParameter("Type3", '%'),
                    new SqlParameter("Type4", '%'),
                    new SqlParameter("Type5", '%'),
                    new SqlParameter("Active", 'Y')
                    ).ToList<USP_MAS_AGENCY_REPORT_Result>();

                totalRecords = lstMasterReport.Count();
                PropertyInfo propertyInfo = typeof(USP_MAS_AGENCY_REPORT_Result).GetProperty(sidx.Trim());

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstMasterReport = lstMasterReport.OrderBy(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        lstMasterReport = lstMasterReport.OrderByDescending(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_AGENCY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return lstMasterReport.Select(x => new
                {
                    id = x.MAST_AGENCY_CODE,
                    cell = new[]{
                        x.MAST_AGENCY_NAME.ToString(),
                        x.MAST_AGENCY_TYPE.ToString()
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


        public Array TrafficListingDAL(string trafficStatus, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSYEntities();
            int State = 0;
            int District = 0;
            int Block = 0;
            int Village = 0;
            int Panchayat = 0;
            int CensusYear = 0;
            string Active =trafficStatus;
            try
            {
                List<USP_MAS_TRAFFIC_REPORT_Result> lstMasterReport = new List<USP_MAS_TRAFFIC_REPORT_Result>();
                lstMasterReport = dbContext.Database.SqlQuery<USP_MAS_TRAFFIC_REPORT_Result>(
                    "EXEC [omms].[USP_MAS_DATA_REPORT] @RptNo,@State,@District,@Block,@Village,@Panchayat,@CensusYear,@Type1,@Type2,@Type3,@Type4,@Type5,@Active",
                    new SqlParameter("RptNo", 42),
                    new SqlParameter("State", State),
                    new SqlParameter("District", District),
                    new SqlParameter("Block", Block),
                    new SqlParameter("Village", Village),
                    new SqlParameter("Panchayat", Panchayat),
                    new SqlParameter("CensusYear", CensusYear),
                    new SqlParameter("Type1", '%'),
                    new SqlParameter("Type2", '%'),
                    new SqlParameter("Type3", '%'),
                    new SqlParameter("Type4", '%'),
                    new SqlParameter("Type5", '%'),
                    new SqlParameter("Active", Active)
                    ).ToList<USP_MAS_TRAFFIC_REPORT_Result>();


               // var lstMasterReport1 = dbContext.USP_MAS_DATA_REPORT(42, 0, 0, 0, 0, 0, 0, "%", "%", "%", "%", "%", "%").ToList<USP_MAS_DATA_REPORT_Result>(); 

                totalRecords = lstMasterReport.Count();
                PropertyInfo propertyInfo = typeof(USP_MAS_TRAFFIC_REPORT_Result).GetProperty(sidx.Trim());

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstMasterReport = lstMasterReport.OrderBy(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        lstMasterReport = lstMasterReport.OrderByDescending(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_TRAFFIC_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return lstMasterReport.Select(x => new
                {
                    id = x.MAST_TRAFFIC_CODE,
                    cell = new[]{
                        x.MAST_TRAFFIC_NAME.ToString(),
                        x.MAST_TRAFFIC_STATUS.ToString()
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


        public Array FundingAgencyListingDAL(int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSYEntities();
            int State = 0;
            int District = 0;
            int Block = 0;
            int Village = 0;
            int Panchayat = 0;
            int CensusYear = 0;
            try
            {
                List<USP_MAS_FUNDING_AGENCY_REPORT_Result> lstMasterReport = new List<USP_MAS_FUNDING_AGENCY_REPORT_Result>();
                lstMasterReport = dbContext.Database.SqlQuery<USP_MAS_FUNDING_AGENCY_REPORT_Result>(
                    "EXEC [omms].[USP_MAS_DATA_REPORT] @RptNo,@State,@District,@Block,@Village,@Panchayat,@CensusYear,@Type1,@Type2,@Type3,@Type4,@Type5,@Active",
                    new SqlParameter("RptNo", 43),
                    new SqlParameter("State", State),
                    new SqlParameter("District", District),
                    new SqlParameter("Block", Block),
                    new SqlParameter("Village", Village),
                    new SqlParameter("Panchayat", Panchayat),
                    new SqlParameter("CensusYear", CensusYear),
                    new SqlParameter("Type1", '%'),
                    new SqlParameter("Type2", '%'),
                    new SqlParameter("Type3", '%'),
                    new SqlParameter("Type4", '%'),
                    new SqlParameter("Type5", '%'),
                    new SqlParameter("Active", 'Y')
                    ).ToList<USP_MAS_FUNDING_AGENCY_REPORT_Result>();

                totalRecords = lstMasterReport.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_FUNDING_AGENCY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        lstMasterReport = lstMasterReport.OrderByDescending(x => x.MAST_FUNDING_AGENCY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_FUNDING_AGENCY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return lstMasterReport.Select(x => new
                {
                    id = x.MAST_FUNDING_AGENCY_CODE,
                    cell = new[]{
                        x.MAST_FUNDING_AGENCY_NAME.ToString()
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


        public Array QualificationListingDAL(int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSYEntities();
            int State = 0;
            int District = 0;
            int Block = 0;
            int Village = 0;
            int Panchayat = 0;
            int CensusYear = 0;
            try
            {
                List<USP_MAS_QUALIFICATION_REPORT_Result> lstMasterReport = new List<USP_MAS_QUALIFICATION_REPORT_Result>();
                lstMasterReport = dbContext.Database.SqlQuery<USP_MAS_QUALIFICATION_REPORT_Result>(
                    "EXEC [omms].[USP_MAS_DATA_REPORT] @RptNo,@State,@District,@Block,@Village,@Panchayat,@CensusYear,@Type1,@Type2,@Type3,@Type4,@Type5,@Active",
                    new SqlParameter("RptNo", 44),
                    new SqlParameter("State", State),
                    new SqlParameter("District", District),
                    new SqlParameter("Block", Block),
                    new SqlParameter("Village", Village),
                    new SqlParameter("Panchayat", Panchayat),
                    new SqlParameter("CensusYear", CensusYear),
                    new SqlParameter("Type1", '%'),
                    new SqlParameter("Type2", '%'),
                    new SqlParameter("Type3", '%'),
                    new SqlParameter("Type4", '%'),
                    new SqlParameter("Type5", '%'),
                    new SqlParameter("Active", 'Y')
                    ).ToList<USP_MAS_QUALIFICATION_REPORT_Result>();

                totalRecords = lstMasterReport.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_QUALIFICATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        lstMasterReport = lstMasterReport.OrderByDescending(x => x.MAST_QUALIFICATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_QUALIFICATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return lstMasterReport.Select(x => new
                {
                    id = x.MAST_QUALIFICATION_CODE,
                    cell = new[]{
                        x.MAST_QUALIFICATION_NAME.ToString()
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


        public Array LokSabhaTermListingDAL(int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSYEntities();
            int State = 0;
            int District = 0;
            int Block = 0;
            int Village = 0;
            int Panchayat = 0;
            int CensusYear = 0;
            try
            {
                List<USP_MAS_LOK_SABHA_TERM_REPORT_Result> lstMasterReport = new List<USP_MAS_LOK_SABHA_TERM_REPORT_Result>();
                lstMasterReport = dbContext.Database.SqlQuery<USP_MAS_LOK_SABHA_TERM_REPORT_Result>(
                    "EXEC [omms].[USP_MAS_DATA_REPORT] @RptNo,@State,@District,@Block,@Village,@Panchayat,@CensusYear,@Type1,@Type2,@Type3,@Type4,@Type5,@Active",
                    new SqlParameter("RptNo", 48),
                    new SqlParameter("State", State),
                    new SqlParameter("District", District),
                    new SqlParameter("Block", Block),
                    new SqlParameter("Village", Village),
                    new SqlParameter("Panchayat", Panchayat),
                    new SqlParameter("CensusYear", CensusYear),
                    new SqlParameter("Type1", '%'),
                    new SqlParameter("Type2", '%'),
                    new SqlParameter("Type3", '%'),
                    new SqlParameter("Type4", '%'),
                    new SqlParameter("Type5", '%'),
                    new SqlParameter("Active", 'Y')
                    ).ToList<USP_MAS_LOK_SABHA_TERM_REPORT_Result>();

                totalRecords = lstMasterReport.Count();
                PropertyInfo propertyInfo = typeof(USP_MAS_LOK_SABHA_TERM_REPORT_Result).GetProperty(sidx.Trim());
                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstMasterReport = lstMasterReport.OrderBy(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        lstMasterReport = lstMasterReport.OrderByDescending(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_LS_TERM).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return lstMasterReport.Select(x => new
                {
                    id = x.MAST_LS_TERM,
                    cell = new[]{
                        x.MAST_LS_TERM.ToString(),
                        x.MAST_LS_START_DATE,
                        x.MAST_LS_END_DATE
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


        public Array ContractorSupplierListingDAL(int stateCode, string contractorSupplierFlag, string contractStatus, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSYEntities();
            int State =stateCode;
            int District = PMGSYSession.Current.DistrictCode;
            int Block =0;
            int Village = 0;
            int Panchayat = 0;
            int CensusYear = 0;
            string Type1 = contractorSupplierFlag == "All" ? "%" : contractorSupplierFlag;
            string Active = contractStatus == "All" ? "%" : contractStatus;
            try
            {
                List<USP_MAS_CONTRACTOR_SUPPLIER_REPORT_Result> lstMasterReport = new List<USP_MAS_CONTRACTOR_SUPPLIER_REPORT_Result>();
                lstMasterReport = dbContext.Database.SqlQuery<USP_MAS_CONTRACTOR_SUPPLIER_REPORT_Result>(
                    "EXEC [omms].[USP_MAS_DATA_REPORT] @RptNo,@State,@District,@Block,@Village,@Panchayat,@CensusYear,@Type1,@Type2,@Type3,@Type4,@Type5,@Active",
                    new SqlParameter("RptNo", 31),
                    new SqlParameter("State", State),
                    new SqlParameter("District", District),
                    new SqlParameter("Block", Block),
                    new SqlParameter("Village", Village),
                    new SqlParameter("Panchayat", Panchayat),
                    new SqlParameter("CensusYear", CensusYear),
                    new SqlParameter("Type1", Type1),
                    new SqlParameter("Type2", '%'),
                    new SqlParameter("Type3", '%'),
                    new SqlParameter("Type4", '%'),
                    new SqlParameter("Type5", '%'),
                    new SqlParameter("Active", Active)
                    ).ToList<USP_MAS_CONTRACTOR_SUPPLIER_REPORT_Result>();

                totalRecords = lstMasterReport.Count();
                PropertyInfo propertyInfo = typeof(USP_MAS_CONTRACTOR_SUPPLIER_REPORT_Result).GetProperty(sidx.Trim());

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstMasterReport = lstMasterReport.OrderBy(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        lstMasterReport = lstMasterReport.OrderByDescending(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_CON_COMPANY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return lstMasterReport.Select(x => new
                {
                    id = x.MAST_CON_ID,
                    cell = new[]{                        	
                        x.MAST_CON_SUP_FLAG	,
                        x.MAST_CON_COMPANY_NAME,	
                        x.MAST_CON_NAME	,
                        x.MAST_CON_ADDR,	
                        x.MAST_CON_CONTACT,	
                        x.MAST_CON_PAN,	
                        x.MAST_CON_LEGAL_HEIR_NAME,	
                        x.MAST_CON_EXPIRY_DATE,	
                        x.MAST_CON_REMARKS	,
                        x.MAST_CON_STATUS,
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


        public Array AutonomousBodyListingDAL(int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSYEntities();
            int State = stateCode;
            int District = 0;
            int Block = 0;
            int Village = 0;
            int Panchayat = 0;
            int CensusYear = 0;
            try
            {
                List<USP_MAS_AUTONOMOUS_BODY_REPORT_Result> lstMasterReport = new List<USP_MAS_AUTONOMOUS_BODY_REPORT_Result>();
                lstMasterReport = dbContext.Database.SqlQuery<USP_MAS_AUTONOMOUS_BODY_REPORT_Result>(
                    "EXEC [omms].[USP_MAS_DATA_REPORT] @RptNo,@State,@District,@Block,@Village,@Panchayat,@CensusYear,@Type1,@Type2,@Type3,@Type4,@Type5,@Active",
                    new SqlParameter("RptNo", 40),
                    new SqlParameter("State", State),
                    new SqlParameter("District", District),
                    new SqlParameter("Block", Block),
                    new SqlParameter("Village", Village),
                    new SqlParameter("Panchayat", Panchayat),
                    new SqlParameter("CensusYear", CensusYear),
                    new SqlParameter("Type1", '%'),
                    new SqlParameter("Type2", '%'),
                    new SqlParameter("Type3", '%'),
                    new SqlParameter("Type4", '%'),
                    new SqlParameter("Type5", '%'),
                    new SqlParameter("Active", 'Y')
                    ).ToList<USP_MAS_AUTONOMOUS_BODY_REPORT_Result>();

                totalRecords = lstMasterReport.Count();
                PropertyInfo propertyInfo = typeof(USP_MAS_AUTONOMOUS_BODY_REPORT_Result).GetProperty(sidx.Trim());

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstMasterReport = lstMasterReport.OrderBy(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        lstMasterReport = lstMasterReport.OrderByDescending(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }



                return lstMasterReport.Select(x => new
                {
                    id = x.MAST_STATE_CODE,
                    cell = new[]{
                        x.MAST_STATE_NAME,
                        x.ADMIN_AUTONOMOUS_BODY
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


        public Array ContractorClassTypeListingDAL(int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSYEntities();
            int State = stateCode;
            int District = 0;
            int Block = 0;
            int Village = 0;
            int Panchayat = 0;
            int CensusYear = 0;
            try
            {
                List<USP_MAS_CONTRACTOR_CLASS_TYPE_REPORT_Result> lstMasterReport = new List<USP_MAS_CONTRACTOR_CLASS_TYPE_REPORT_Result>();
                lstMasterReport = dbContext.Database.SqlQuery<USP_MAS_CONTRACTOR_CLASS_TYPE_REPORT_Result>(
                    "EXEC [omms].[USP_MAS_DATA_REPORT] @RptNo,@State,@District,@Block,@Village,@Panchayat,@CensusYear,@Type1,@Type2,@Type3,@Type4,@Type5,@Active",
                    new SqlParameter("RptNo", 28),
                    new SqlParameter("State", State),
                    new SqlParameter("District", District),
                    new SqlParameter("Block", Block),
                    new SqlParameter("Village", Village),
                    new SqlParameter("Panchayat", Panchayat),
                    new SqlParameter("CensusYear", CensusYear),
                    new SqlParameter("Type1", '%'),
                    new SqlParameter("Type2", '%'),
                    new SqlParameter("Type3", '%'),
                    new SqlParameter("Type4", '%'),
                    new SqlParameter("Type5", '%'),
                    new SqlParameter("Active", 'Y')
                    ).ToList<USP_MAS_CONTRACTOR_CLASS_TYPE_REPORT_Result>();

                totalRecords = lstMasterReport.Count();
                PropertyInfo propertyInfo = typeof(USP_MAS_CONTRACTOR_CLASS_TYPE_REPORT_Result).GetProperty(sidx.Trim());
                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstMasterReport = lstMasterReport.OrderBy(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        lstMasterReport = lstMasterReport.OrderByDescending(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return lstMasterReport.Select(x => new
                {
                    id = x.MAST_CON_CLASS,
                    cell = new[]{
                        x.MAST_STATE_NAME,
                        x.MAST_CON_CLASS_TYPE_NAME
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


        public Array ContractorRegistrationListingDAL(string activeStatus, string registrationStatus, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSYEntities();
            int State = stateCode;
            int District = 0;
            int Block = 0;
            int Village = 0;
            int Panchayat = 0;
            int CensusYear = 0;
            string Active = activeStatus;
            string Type2 = registrationStatus;
            try
            {
                List<USP_MAS_CONTRACTOR_REGISTRATION_REPORT_Result> lstMasterReport = new List<USP_MAS_CONTRACTOR_REGISTRATION_REPORT_Result>();
                lstMasterReport = dbContext.Database.SqlQuery<USP_MAS_CONTRACTOR_REGISTRATION_REPORT_Result>(
                    "EXEC [omms].[USP_MAS_DATA_REPORT] @RptNo,@State,@District,@Block,@Village,@Panchayat,@CensusYear,@Type1,@Type2,@Type3,@Type4,@Type5,@Active",
                    new SqlParameter("RptNo", 32),
                    new SqlParameter("State", State),
                    new SqlParameter("District", District),
                    new SqlParameter("Block", Block),
                    new SqlParameter("Village", Village),
                    new SqlParameter("Panchayat", Panchayat),
                    new SqlParameter("CensusYear", CensusYear),
                    new SqlParameter("Type1", '%'),
                    new SqlParameter("Type2", Type2),
                    new SqlParameter("Type3", '%'),
                    new SqlParameter("Type4", '%'),
                    new SqlParameter("Type5", '%'),
                    new SqlParameter("Active", Active)
                    ).ToList<USP_MAS_CONTRACTOR_REGISTRATION_REPORT_Result>();

                totalRecords = lstMasterReport.Count();
                PropertyInfo propertyInfo = typeof(USP_MAS_CONTRACTOR_REGISTRATION_REPORT_Result).GetProperty(sidx.Trim());

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstMasterReport = lstMasterReport.OrderBy(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        lstMasterReport = lstMasterReport.OrderByDescending(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_CON_CLASS_TYPE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }


                return lstMasterReport.Select(x => new
                {
                    id = x.MAST_CON_ID,
                    cell = new[]{
                        x.MAST_CON_SUP_FLAG	,
                        x.MAST_CON_COMPANY_NAME,	
                        x.MAST_CON_NAME,	
                        x.MAST_CON_PAN,	
                        x.MAST_CON_STATUS,
                        x.MAST_CON_REG_NO,
                        x.MAST_CON_CLASS_TYPE_NAME,	
                        x.MAST_CON_VALID_FROM,	
                        x.MAST_CON_VALID_TO,
                        x.MAST_STATE_NAME,	
                        x.MAST_REG_OFFICE,	
                        x.MAST_REG_STATUS,
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


        public Array SQCListingDAL(string activeStatus, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSYEntities();
            int State = stateCode;
            int District = 0;
            int Block = 0;
            int Village = 0;
            int Panchayat = 0;
            int CensusYear = 0;
            string Active = activeStatus;
            try
            {
                List<USP_MAS_SQC_REPORT_Result> lstMasterReport = new List<USP_MAS_SQC_REPORT_Result>();
                lstMasterReport = dbContext.Database.SqlQuery<USP_MAS_SQC_REPORT_Result>(
                    "EXEC [omms].[USP_MAS_DATA_REPORT] @RptNo,@State,@District,@Block,@Village,@Panchayat,@CensusYear,@Type1,@Type2,@Type3,@Type4,@Type5,@Active",
                    new SqlParameter("RptNo", 39),
                    new SqlParameter("State", State),
                    new SqlParameter("District", District),
                    new SqlParameter("Block", Block),
                    new SqlParameter("Village", Village),
                    new SqlParameter("Panchayat", Panchayat),
                    new SqlParameter("CensusYear", CensusYear),
                    new SqlParameter("Type1", '%'),
                    new SqlParameter("Type2", '%'),
                    new SqlParameter("Type3", '%'),
                    new SqlParameter("Type4", '%'),
                    new SqlParameter("Type5", '%'),
                    new SqlParameter("Active", Active)
                    ).ToList<USP_MAS_SQC_REPORT_Result>();

                totalRecords = lstMasterReport.Count();
                PropertyInfo propertyInfo = typeof(USP_MAS_SQC_REPORT_Result).GetProperty(sidx.Trim());

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstMasterReport = lstMasterReport.OrderBy(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        lstMasterReport = lstMasterReport.OrderByDescending(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return lstMasterReport.Select(x => new
                {
                    id = x.ADMIN_QC_CODE,
                    cell = new[]{
                        x.MAST_STATE_NAME,	
                        x.ADMIN_QC_NAME,	
                        x.MAST_DESIG_NAME,	
                        x.ADMIN_QC_ADDRESS,
                        x.ADMIN_QC_CONTACT,	
                        x.ADMIN_ACTIVE_STATUS,	
                        x.ADMIN_ACTIVE_ENDDATE,
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


        public Array SRRDAListingDAL(int stateCode, int agencyCode, string officeType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSYEntities();
            int State = stateCode;
            int District = 0;
            int Block = 0;
            int Village = 0;
            int Panchayat = agencyCode;
            int CensusYear = 0;
            string Type1 = officeType;
            try
            {
                List<USP_MAS_SRRDA_DPIU_REPORT_Result> lstMasterReport = new List<USP_MAS_SRRDA_DPIU_REPORT_Result>();
                lstMasterReport = dbContext.Database.SqlQuery<USP_MAS_SRRDA_DPIU_REPORT_Result>(
                    "EXEC [omms].[USP_MAS_DATA_REPORT] @RptNo,@State,@District,@Block,@Village,@Panchayat,@CensusYear,@Type1,@Type2,@Type3,@Type4,@Type5,@Active",
                    new SqlParameter("RptNo", 35),
                    new SqlParameter("State", State),
                    new SqlParameter("District", District),
                    new SqlParameter("Block", Block),
                    new SqlParameter("Village", Village),
                    new SqlParameter("Panchayat", Panchayat),
                    new SqlParameter("CensusYear", CensusYear),
                    new SqlParameter("Type1", Type1),
                    new SqlParameter("Type2", '%'),
                    new SqlParameter("Type3", '%'),
                    new SqlParameter("Type4", '%'),
                    new SqlParameter("Type5", '%'),
                    new SqlParameter("Active", 'Y')
                    ).ToList<USP_MAS_SRRDA_DPIU_REPORT_Result>();

                totalRecords = lstMasterReport.Count();
                PropertyInfo propertyInfo = typeof(USP_MAS_SRRDA_DPIU_REPORT_Result).GetProperty(sidx.Trim());

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstMasterReport = lstMasterReport.OrderBy(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        lstMasterReport = lstMasterReport.OrderByDescending(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return lstMasterReport.Select(x => new
                {
                    id = x.ADMIN_ND_CODE,
                    cell = new[]{
                        x.MAST_STATE_NAME,
                        x.MAST_AGENCY_NAME,
                        x.MAST_PARENT_ND_NAME,                        
                        x.MAST_ND_TYPE,
                        x.ADMIN_ND_NAME,
                        x.ADMIN_ND_ADDRESS,
                        x.ADMIN_ND_CONTACT,
                        x.ADMIN_ND_TAN_NO,
                        x.ADMIN_ND_REMARKS
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


        public Array VidhanSabhaTermListingDAL(int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSYEntities();
            int State = stateCode;
            int District = 0;
            int Block = 0;
            int Village = 0;
            int Panchayat = 0;
            int CensusYear = 0;
            try
            {
                List<USP_MAS_VIDHAN_SABHA_TERM_REPORT_Result> lstMasterReport = new List<USP_MAS_VIDHAN_SABHA_TERM_REPORT_Result>();
                lstMasterReport = dbContext.Database.SqlQuery<USP_MAS_VIDHAN_SABHA_TERM_REPORT_Result>(
                    "EXEC [omms].[USP_MAS_DATA_REPORT] @RptNo,@State,@District,@Block,@Village,@Panchayat,@CensusYear,@Type1,@Type2,@Type3,@Type4,@Type5,@Active",
                    new SqlParameter("RptNo", 47),
                    new SqlParameter("State", State),
                    new SqlParameter("District", District),
                    new SqlParameter("Block", Block),
                    new SqlParameter("Village", Village),
                    new SqlParameter("Panchayat", Panchayat),
                    new SqlParameter("CensusYear", CensusYear),
                    new SqlParameter("Type1", '%'),
                    new SqlParameter("Type2", '%'),
                    new SqlParameter("Type3", '%'),
                    new SqlParameter("Type4", '%'),
                    new SqlParameter("Type5", '%'),
                    new SqlParameter("Active", 'Y')
                    ).ToList<USP_MAS_VIDHAN_SABHA_TERM_REPORT_Result>();

                totalRecords = lstMasterReport.Count();
                PropertyInfo propertyInfo = typeof(USP_MAS_VIDHAN_SABHA_TERM_REPORT_Result).GetProperty(sidx.Trim());
                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstMasterReport = lstMasterReport.OrderBy(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        lstMasterReport = lstMasterReport.OrderByDescending(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_VS_TERM).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return lstMasterReport.Select(x => new
                {
                    id = x.MAST_VS_TERM,
                    cell = new[]{
                        x.MAST_STATE_NAME,
                        x.MAST_VS_TERM.ToString(),
                        x.MAST_VS_START_DATE,
                        x.MAST_VS_END_DATE
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


        public Array NodalOfficerListingDAL(int stateCode, int agencyCode, string officeType, string activeType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSYEntities();
            int State = stateCode;
            int District = 0;
            int Block = 0;
            int Village = 0;
            int Panchayat = agencyCode;
            string Type1 = officeType;
            string Active = activeType;
            int CensusYear = 0;
            try
            {
                List<USP_MAS_NODAL_OFFICER_REPORT_Result> lstMasterReport = new List<USP_MAS_NODAL_OFFICER_REPORT_Result>();
                lstMasterReport = dbContext.Database.SqlQuery<USP_MAS_NODAL_OFFICER_REPORT_Result>(
                    "EXEC [omms].[USP_MAS_DATA_REPORT] @RptNo,@State,@District,@Block,@Village,@Panchayat,@CensusYear,@Type1,@Type2,@Type3,@Type4,@Type5,@Active",
                    new SqlParameter("RptNo", 37),
                    new SqlParameter("State", State),
                    new SqlParameter("District", District),
                    new SqlParameter("Block", Block),
                    new SqlParameter("Village", Village),
                    new SqlParameter("Panchayat", Panchayat),
                    new SqlParameter("CensusYear", CensusYear),
                    new SqlParameter("Type1", Type1),
                    new SqlParameter("Type2", '%'),
                    new SqlParameter("Type3", '%'),
                    new SqlParameter("Type4", '%'),
                    new SqlParameter("Type5", '%'),
                    new SqlParameter("Active", Active)
                    ).ToList<USP_MAS_NODAL_OFFICER_REPORT_Result>();

                totalRecords = lstMasterReport.Count();
                PropertyInfo propertyInfo = typeof(USP_MAS_NODAL_OFFICER_REPORT_Result).GetProperty(sidx.Trim());

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstMasterReport = lstMasterReport.OrderBy(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        lstMasterReport = lstMasterReport.OrderByDescending(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    lstMasterReport = lstMasterReport.OrderBy(x => x.ADMIN_ND_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return lstMasterReport.Select(x => new
                {
                    id = x.ADMIN_ND_CODE,
                    cell = new[]{
                        x.MAST_STATE_NAME,	
                        x.MAST_AGENCY_NAME,
                        x.MAST_PARENT_ND_NAME,	
                        x.MAST_ND_TYPE,	
                        x.ADMIN_ND_NAME,	
                        x.ADMIN_NO_NAME,	
                        x.MAST_DESIG_NAME,	
                        x.ADMIN_NO_ADDRESS,	
                        x.ADMIN_NO_CONTACT	,
                        x.ADMIN_NO_LEVEL,	
                        x.ADMIN_ACTIVE_STATUS,	
                        x.ADMIN_ACTIVE_START_DATE,	
                        x.ADMIN_ACTIVE_END_DATE,
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


        public Array QualityMonitorListingDAL(int stateCode, string qmType, string activeType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSYEntities();
            int State = stateCode;
            int District = 0;
            int Block = 0;
            int Village = 0;
            int Panchayat = 0;
            int CensusYear = 0;
            string Type1 = qmType;
            string Active = activeType;
            try
            {
                List<USP_MAS_QUALITY_MONITOR_REPORT_Result> lstMasterReport = new List<USP_MAS_QUALITY_MONITOR_REPORT_Result>();
                lstMasterReport = dbContext.Database.SqlQuery<USP_MAS_QUALITY_MONITOR_REPORT_Result>(
                    "EXEC [omms].[USP_MAS_DATA_REPORT] @RptNo,@State,@District,@Block,@Village,@Panchayat,@CensusYear,@Type1,@Type2,@Type3,@Type4,@Type5,@Active",
                    new SqlParameter("RptNo", 41),
                    new SqlParameter("State", State),
                    new SqlParameter("District", District),
                    new SqlParameter("Block", Block),
                    new SqlParameter("Village", Village),
                    new SqlParameter("Panchayat", Panchayat),
                    new SqlParameter("CensusYear", CensusYear),
                    new SqlParameter("Type1", Type1),
                    new SqlParameter("Type2", '%'),
                    new SqlParameter("Type3", '%'),
                    new SqlParameter("Type4", '%'),
                    new SqlParameter("Type5", '%'),
                    new SqlParameter("Active", Active)
                    ).ToList<USP_MAS_QUALITY_MONITOR_REPORT_Result>();

                totalRecords = lstMasterReport.Count();
                PropertyInfo propertyInfo = typeof(USP_MAS_QUALITY_MONITOR_REPORT_Result).GetProperty(sidx.Trim());

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstMasterReport = lstMasterReport.OrderBy(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        lstMasterReport = lstMasterReport.OrderByDescending(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    lstMasterReport = lstMasterReport.OrderBy(x => x.ADMIN_QM_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return lstMasterReport.Select(x => new
                {
                    id = x.ADMIN_QM_CODE,
                    cell = new[]{
                        x.MAST_STATE_NAME,
	                    x.ADMIN_QM_TYPE,
                        x.ADMIN_QM_NAME	,
                        x.MAST_DESIG_NAME,	
                        x.ADMIN_QM_ADDRESS,	
                        x.ADMIN_QM_CONTACT,	
                        x.ADMIN_QM_PAN,
                        x.ADMIN_QM_EMPANELLED,
                        x.ADMIN_QM_EMPANELLED_YEAR.ToString(),
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


        public Array TechnicalAgencyListingDAL(int stateCode,string taType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSYEntities();
            int State = stateCode;
            int District = 0;
            int Block = 0;
            int Village = 0;
            int Panchayat = 0;
            int CensusYear = 0;
            string Type1 = taType;
            try
            {
                List<USP_MAS_TECHNICAL_AGENCY_REPORT_Result> lstMasterReport = new List<USP_MAS_TECHNICAL_AGENCY_REPORT_Result>();
                lstMasterReport = dbContext.Database.SqlQuery<USP_MAS_TECHNICAL_AGENCY_REPORT_Result>(
                    "EXEC [omms].[USP_MAS_DATA_REPORT] @RptNo,@State,@District,@Block,@Village,@Panchayat,@CensusYear,@Type1,@Type2,@Type3,@Type4,@Type5,@Active",
                    new SqlParameter("RptNo", 38),
                    new SqlParameter("State", State),
                    new SqlParameter("District", District),
                    new SqlParameter("Block", Block),
                    new SqlParameter("Village", Village),
                    new SqlParameter("Panchayat", Panchayat),
                    new SqlParameter("CensusYear", CensusYear),
                    new SqlParameter("Type1", taType),
                    new SqlParameter("Type2", '%'),
                    new SqlParameter("Type3", '%'),
                    new SqlParameter("Type4", '%'),
                    new SqlParameter("Type5", '%'),
                    new SqlParameter("Active", 'Y')
                    ).ToList<USP_MAS_TECHNICAL_AGENCY_REPORT_Result>();

                totalRecords = lstMasterReport.Count();
                PropertyInfo propertyInfo = typeof(USP_MAS_TECHNICAL_AGENCY_REPORT_Result).GetProperty(sidx.Trim());

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstMasterReport = lstMasterReport.OrderBy(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        lstMasterReport = lstMasterReport.OrderByDescending(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    lstMasterReport = lstMasterReport.OrderBy(x => x.ADMIN_TA_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return lstMasterReport.Select(x => new
                {
                    id = x.ADMIN_TA_CODE,
                    cell = new[]{
                        x.ADMIN_TA_TYPE,	
                        x.ADMIN_TA_NAME,	
                        x.ADMIN_TA_ADDRESS,
                        x.ADMIN_TA_CONTACT,
                        x.MAST_DESIG_NAME,	
                        x.ADMIN_TA_CONTACT_NAME
                      
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

        public Array TechnicalAgencyStateMappingListingDAL(int stateCode, string taType,string activeType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSYEntities();
            int State = stateCode;
            int District = 0;
            int Block = 0;
            int Village = 0;
            int Panchayat = 0;
            int CensusYear = 0;
            string Type1 = taType;
            string ActiveType = activeType;
            try
            {
                List<USP_MAS_TECHNICAL_AGENCY_StateMapping_REPORT_Result> lstMasterReport = new List<USP_MAS_TECHNICAL_AGENCY_StateMapping_REPORT_Result>();
                lstMasterReport = dbContext.Database.SqlQuery<USP_MAS_TECHNICAL_AGENCY_StateMapping_REPORT_Result>(
                    "EXEC [omms].[USP_MAS_DATA_REPORT] @RptNo,@State,@District,@Block,@Village,@Panchayat,@CensusYear,@Type1,@Type2,@Type3,@Type4,@Type5,@Active",
                    new SqlParameter("RptNo", 54),
                    new SqlParameter("State", State),
                    new SqlParameter("District", District),
                    new SqlParameter("Block", Block),
                    new SqlParameter("Village", Village),
                    new SqlParameter("Panchayat", Panchayat),
                    new SqlParameter("CensusYear", CensusYear),
                    new SqlParameter("Type1", taType),
                    new SqlParameter("Type2", '%'),
                    new SqlParameter("Type3", '%'),
                    new SqlParameter("Type4", '%'),
                    new SqlParameter("Type5", '%'),
                    new SqlParameter("Active", ActiveType)
                    ).ToList<USP_MAS_TECHNICAL_AGENCY_StateMapping_REPORT_Result>();

                totalRecords = lstMasterReport.Count();
                PropertyInfo propertyInfo = typeof(USP_MAS_TECHNICAL_AGENCY_StateMapping_REPORT_Result).GetProperty(sidx.Trim());

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstMasterReport = lstMasterReport.OrderBy(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        lstMasterReport = lstMasterReport.OrderByDescending(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    lstMasterReport = lstMasterReport.OrderBy(x => x.ADMIN_TA_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return lstMasterReport.Select(x => new
                {
                    id = x.ADMIN_TA_CODE,
                    cell = new[]{
                        x.ADMIN_TA_TYPE.ToString(),	
                        x.ADMIN_TA_NAME.ToString(),                       
                        x.MAST_DESIG_NAME.ToString(),	
                        x.ADMIN_TA_CONTACT_NAME.ToString(),
                        x.MAST_STATE_NAME.ToString(),
                        x.MAST_DISTRICT_NAME.ToString(),
                        x.MAST_START_DATE.ToString(),
                        x.MAST_END_DATE.ToString(),
                        x.MAST_IS_ACTIVE.ToString()
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

        public Array MLAMemberListingDAL(string constituency, string term, int stateCode,string activeType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSYEntities();
            int State = stateCode;
            int District = 0;
            int Block = 0;
            int Village = Convert.ToInt32(term);
            int Panchayat = Convert.ToInt32(constituency);
            int CensusYear = 0;
            string ActiveType = activeType;
            try
            {
                List<USP_MAS_MLA_MEMBER_REPORT_Result> lstMasterReport = new List<USP_MAS_MLA_MEMBER_REPORT_Result>();
                lstMasterReport = dbContext.Database.SqlQuery<USP_MAS_MLA_MEMBER_REPORT_Result>(
                    "EXEC [omms].[USP_MAS_DATA_REPORT] @RptNo,@State,@District,@Block,@Village,@Panchayat,@CensusYear,@Type1,@Type2,@Type3,@Type4,@Type5,@Active",
                    new SqlParameter("RptNo", 52),
                    new SqlParameter("State", State),
                    new SqlParameter("District", District),
                    new SqlParameter("Block", Block),
                    new SqlParameter("Village", Village),
                    new SqlParameter("Panchayat", Panchayat),
                    new SqlParameter("CensusYear", CensusYear),
                    new SqlParameter("Type1", '%'),
                    new SqlParameter("Type2", '%'),
                    new SqlParameter("Type3", '%'),
                    new SqlParameter("Type4", '%'),
                    new SqlParameter("Type5", '%'),
                    new SqlParameter("Active", ActiveType)
                    ).ToList<USP_MAS_MLA_MEMBER_REPORT_Result>();

                totalRecords = lstMasterReport.Count();
                PropertyInfo propertyInfo = typeof(USP_MAS_MLA_MEMBER_REPORT_Result).GetProperty(sidx.Trim());

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstMasterReport = lstMasterReport.OrderBy(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        lstMasterReport = lstMasterReport.OrderByDescending(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_MLA_CONST_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return lstMasterReport.Select(x => new
                {
                    id = x.MAST_MLA_CONST_CODE,
                    cell = new[]{
                        x.MAST_STATE_NAME,
                        x.MAST_MLA_CONST_NAME,                        
                        x.MAST_VS_TERM.ToString(),
                        x.MAST_VS_START_DATE,
                        x.MAST_VS_END_DATE,
                        x.MAST_MEMBER,
                        x.MAST_MEMBER_PARTY,
                        x.MAST_MEMBER_START_DATE,
                        x.MAST_MEMBER_END_DATE,
                        x.MAST_MLA_CONST_ACTIVE
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


        public Array MPMemberListingDAL(string constituency, string term, int stateCode, string activeType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSYEntities();
            int State = stateCode;
            int District = 0;
            int Block = 0;
            int Village = Convert.ToInt32(term);
            int Panchayat = Convert.ToInt32(constituency);
            int CensusYear = 0;
            string ActiveType = activeType;
            try
            {
                List<USP_MAS_MP_MEMBER_REPORT_Result> lstMasterReport = new List<USP_MAS_MP_MEMBER_REPORT_Result>();
                lstMasterReport = dbContext.Database.SqlQuery<USP_MAS_MP_MEMBER_REPORT_Result>(
                    "EXEC [omms].[USP_MAS_DATA_REPORT] @RptNo,@State,@District,@Block,@Village,@Panchayat,@CensusYear,@Type1,@Type2,@Type3,@Type4,@Type5,@Active",
                    new SqlParameter("RptNo", 53),
                    new SqlParameter("State", State),
                    new SqlParameter("District", District),
                    new SqlParameter("Block", Block),
                    new SqlParameter("Village", Village),
                    new SqlParameter("Panchayat", Panchayat),
                    new SqlParameter("CensusYear", CensusYear),
                    new SqlParameter("Type1", '%'),
                    new SqlParameter("Type2", '%'),
                    new SqlParameter("Type3", '%'),
                    new SqlParameter("Type4", '%'),
                    new SqlParameter("Type5", '%'),
                    new SqlParameter("Active", ActiveType)
                    ).ToList<USP_MAS_MP_MEMBER_REPORT_Result>();

                totalRecords = lstMasterReport.Count();
                PropertyInfo propertyInfo = typeof(USP_MAS_MP_MEMBER_REPORT_Result).GetProperty(sidx.Trim());

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstMasterReport = lstMasterReport.OrderBy(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        lstMasterReport = lstMasterReport.OrderByDescending(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_MP_CONST_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return lstMasterReport.Select(x => new
                {
                    id = x.MAST_MP_CONST_CODE,
                    cell = new[]{
                         x.MAST_STATE_NAME,
                        x.MAST_MP_CONST_NAME,                       
                        x.MAST_LS_TERM.ToString(),
                        x.MAST_VS_START_DATE,
                        x.MAST_VS_END_DATE,
                        x.MAST_MEMBER,
                        x.MAST_MEMBER_PARTY,
                        x.MAST_MEMBER_START_DATE,
                        x.MAST_MEMBER_END_DATE,
                        x.MAST_MP_CONST_ACTIVE
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


        public Array SurfaceListingDAL(int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSYEntities();
            int State = stateCode;
            int District = 0;
            int Block = 0;
            int Village = 0;
            int Panchayat = 0;
            int CensusYear = 0;
            try
            {
                List<USP_MAS_SURFACE_REPORT_Result> lstMasterReport = new List<USP_MAS_SURFACE_REPORT_Result>();
                lstMasterReport = dbContext.Database.SqlQuery<USP_MAS_SURFACE_REPORT_Result>(
                    "EXEC [omms].[USP_MAS_DATA_REPORT] @RptNo,@State,@District,@Block,@Village,@Panchayat,@CensusYear,@Type1,@Type2,@Type3,@Type4,@Type5,@Active",
                    new SqlParameter("RptNo", 51),
                    new SqlParameter("State", State),
                    new SqlParameter("District", District),
                    new SqlParameter("Block", Block),
                    new SqlParameter("Village", Village),
                    new SqlParameter("Panchayat", Panchayat),
                    new SqlParameter("CensusYear", CensusYear),
                    new SqlParameter("Type1", '%'),
                    new SqlParameter("Type2", '%'),
                    new SqlParameter("Type3", '%'),
                    new SqlParameter("Type4", '%'),
                    new SqlParameter("Type5", '%'),
                    new SqlParameter("Active", 'Y')
                    ).ToList<USP_MAS_SURFACE_REPORT_Result>();

                totalRecords = lstMasterReport.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_SURFACE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        lstMasterReport = lstMasterReport.OrderByDescending(x => x.MAST_SURFACE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_SURFACE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return lstMasterReport.Select(x => new
                {
                    id = x.MAST_SURFACE_CODE,
                    cell = new[]{
                        x.MAST_SURFACE_NAME                        
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

        public Array OfficerCategoryListingDAL(int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSYEntities();
            int State = 0;
            int District = 0;
            int Block = 0;
            int Village = 0;
            int Panchayat = 0;
            int CensusYear = 0;
            try
            {
                List<USP_MAS_Officer_Category_REPORT_Result> lstMasterReport = new List<USP_MAS_Officer_Category_REPORT_Result>();
                lstMasterReport = dbContext.Database.SqlQuery<USP_MAS_Officer_Category_REPORT_Result>(
                    "EXEC [omms].[USP_MAS_DATA_REPORT] @RptNo,@State,@District,@Block,@Village,@Panchayat,@CensusYear,@Type1,@Type2,@Type3,@Type4,@Type5,@Active",
                    new SqlParameter("RptNo", 29),
                    new SqlParameter("State", State),
                    new SqlParameter("District", District),
                    new SqlParameter("Block", Block),
                    new SqlParameter("Village", Village),
                    new SqlParameter("Panchayat", Panchayat),
                    new SqlParameter("CensusYear", CensusYear),
                    new SqlParameter("Type1", '%'),
                    new SqlParameter("Type2", '%'),
                    new SqlParameter("Type3", '%'),
                    new SqlParameter("Type4", '%'),
                    new SqlParameter("Type5", '%'),
                    new SqlParameter("Active", 'Y')
                    ).ToList<USP_MAS_Officer_Category_REPORT_Result>();

                totalRecords = lstMasterReport.Count();
                PropertyInfo propertyInfo = typeof(USP_MAS_Officer_Category_REPORT_Result).GetProperty(sidx.Trim());
                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstMasterReport = lstMasterReport.OrderBy(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        lstMasterReport = lstMasterReport.OrderByDescending(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_OFFICER_CATEGORY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return lstMasterReport.Select(x => new
                {                  
                    cell = new[]{
                        x.MAST_OFFICER_CATEGORY_NAME.ToString()
                       
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

        public Array ContractorRegistrationBankListingDAL(int stateCode, string contractRegFlag, string contractStatus, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSYEntities();
            int State = stateCode;
            int District = 0;
            int Block = 0;
            int Village = 0;
            int Panchayat = 0;
            int CensusYear = 0;
            string Type1 = contractRegFlag;
            string Type2 = contractStatus;
            try
            {
                List<USP_MAS_CONTRACTOR_Registration_Bank_REPORT_Result> lstMasterReport = new List<USP_MAS_CONTRACTOR_Registration_Bank_REPORT_Result>();
                lstMasterReport = dbContext.Database.SqlQuery<USP_MAS_CONTRACTOR_Registration_Bank_REPORT_Result>(
                    "EXEC [omms].[USP_MAS_DATA_REPORT] @RptNo,@State,@District,@Block,@Village,@Panchayat,@CensusYear,@Type1,@Type2,@Type3,@Type4,@Type5,@Active",
                    new SqlParameter("RptNo", 33),
                    new SqlParameter("State", State),
                    new SqlParameter("District", District),
                    new SqlParameter("Block", Block),
                    new SqlParameter("Village", Village),
                    new SqlParameter("Panchayat", Panchayat),
                    new SqlParameter("CensusYear", CensusYear),
                    new SqlParameter("Type1", Type1),
                    new SqlParameter("Type2", Type2),
                    new SqlParameter("Type3", '%'),
                    new SqlParameter("Type4", '%'),
                    new SqlParameter("Type5", '%'),
                    new SqlParameter("Active", 'Y')
                    ).ToList<USP_MAS_CONTRACTOR_Registration_Bank_REPORT_Result>();

                totalRecords = lstMasterReport.Count();
                PropertyInfo propertyInfo = typeof(USP_MAS_CONTRACTOR_Registration_Bank_REPORT_Result).GetProperty(sidx.Trim());
                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstMasterReport = lstMasterReport.OrderBy(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        lstMasterReport = lstMasterReport.OrderByDescending(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_CON_SUP_FLAG).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return lstMasterReport.Select(x => new
                {
                    cell = new[]{
                        x.MAST_CON_SUP_FLAG.ToString(),                      
                        x.MAST_CON_COMPANY_NAME.ToString(),
                        x.MAST_CON_NAME.ToString(),
                        x.MAST_CON_PAN.ToString(),
                        x.MAST_CON_STATUS.ToString(),
                        x.MAST_CON_REG_NO.ToString(),
                        x.MAST_CON_CLASS.ToString(),                       
                        x.MAST_CON_VALID_FROM.ToString(),
                        x.MAST_CON_VALID_TO.ToString(),                      
                        x.MAST_STATE_NAME.ToString(),
                        x.MAST_REG_OFFICE.ToString(),
                        x.MAST_REG_STATUS.ToString(),
                        x.MAST_DISTRICT_NAME.ToString(),
                        x.MAST_ACCOUNT_NUMBER.ToString(),
                        x.MAST_BANK_NAME.ToString(),
                        x.MAST_IFSC_CODE.ToString(),
                        x.MAST_ACCOUNT_STATUS.ToString()                
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

        public Array DepartmentDistrictListingDAL(int stateCode, int agencyCode, string officeType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSYEntities();
            int State = stateCode;
            int District = 0;
            int Block = 0;
            int Village = 0;
            int Panchayat = agencyCode;
            int CensusYear = 0;
            string Type1 = officeType;
            try
            {
                List<USP_MAS_Department_District_REPORT_Result> lstMasterReport = new List<USP_MAS_Department_District_REPORT_Result>();
                lstMasterReport = dbContext.Database.SqlQuery<USP_MAS_Department_District_REPORT_Result>(
                    "EXEC [omms].[USP_MAS_DATA_REPORT] @RptNo,@State,@District,@Block,@Village,@Panchayat,@CensusYear,@Type1,@Type2,@Type3,@Type4,@Type5,@Active",
                    new SqlParameter("RptNo", 36),
                    new SqlParameter("State", State),
                    new SqlParameter("District", District),
                    new SqlParameter("Block", Block),
                    new SqlParameter("Village", Village),
                    new SqlParameter("Panchayat", Panchayat),
                    new SqlParameter("CensusYear", CensusYear),
                    new SqlParameter("Type1",officeType),
                    new SqlParameter("Type2", '%'),
                    new SqlParameter("Type3", '%'),
                    new SqlParameter("Type4", '%'),
                    new SqlParameter("Type5", '%'),
                    new SqlParameter("Active", 'Y')
                    ).ToList<USP_MAS_Department_District_REPORT_Result>();

                totalRecords = lstMasterReport.Count();
                PropertyInfo propertyInfo = typeof(USP_MAS_Department_District_REPORT_Result).GetProperty(sidx.Trim());
                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstMasterReport = lstMasterReport.OrderBy(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        lstMasterReport = lstMasterReport.OrderByDescending(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_AGENCY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return lstMasterReport.Select(x => new
                {
                    cell = new[]{
                        x.MAST_STATE_NAME.ToString(),                      
                        x.MAST_AGENCY_NAME.ToString(),
                        x.MAST_PARENT_ND_NAME.ToString(),
                        x.MAST_ND_TYPE.ToString(),
                        x.ADMIN_ND_NAME.ToString(),
                        x.MAST_DISTRICT_NAME.ToString()                           
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

        public Array CarriageListingDAL(string carriageStatus, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSYEntities();
            int State = 0;
            int District = 0;
            int Block = 0;
            int Village = 0;
            int Panchayat = 0;
            int CensusYear = 0;
            string Active = carriageStatus;
            try
            {
                List<USP_MAS_Carriage_REPORT_Result> lstMasterReport = new List<USP_MAS_Carriage_REPORT_Result>();
                lstMasterReport = dbContext.Database.SqlQuery<USP_MAS_Carriage_REPORT_Result>(
                    "EXEC [omms].[USP_MAS_DATA_REPORT] @RptNo,@State,@District,@Block,@Village,@Panchayat,@CensusYear,@Type1,@Type2,@Type3,@Type4,@Type5,@Active",
                    new SqlParameter("RptNo", 45),
                    new SqlParameter("State", State),
                    new SqlParameter("District", District),
                    new SqlParameter("Block", Block),
                    new SqlParameter("Village", Village),
                    new SqlParameter("Panchayat", Panchayat),
                    new SqlParameter("CensusYear", CensusYear),
                    new SqlParameter("Type1", '%'),
                    new SqlParameter("Type2", '%'),
                    new SqlParameter("Type3", '%'),
                    new SqlParameter("Type4", '%'),
                    new SqlParameter("Type5", '%'),
                    new SqlParameter("Active", Active)
                    ).ToList<USP_MAS_Carriage_REPORT_Result>();

                totalRecords = lstMasterReport.Count();
                PropertyInfo propertyInfo = typeof(USP_MAS_Carriage_REPORT_Result).GetProperty(sidx.Trim());
                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstMasterReport = lstMasterReport.OrderBy(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        lstMasterReport = lstMasterReport.OrderByDescending(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_CARRIAGE_WIDTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return lstMasterReport.Select(x => new
                {
                    cell = new[]{
                        x.MAST_CARRIAGE_WIDTH.ToString(),                      
                        x.MAST_CARRIAGE_STATUS.ToString()                                             
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

        public Array QMItemsListingDAL(string qmType, string qmItemActive, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSYEntities();
            int State = 0;
            int District = 0;
            int Block = 0;
            int Village = 0;
            int Panchayat = 0;
            int CensusYear = 0;
            string Type1 = qmType;
            string Active = qmItemActive;
            try
            {
                List<USP_QM_Items_REPORT_Result> lstMasterReport = new List<USP_QM_Items_REPORT_Result>();
                lstMasterReport = dbContext.Database.SqlQuery<USP_QM_Items_REPORT_Result>(
                    "EXEC [omms].[USP_MAS_DATA_REPORT] @RptNo,@State,@District,@Block,@Village,@Panchayat,@CensusYear,@Type1,@Type2,@Type3,@Type4,@Type5,@Active",
                    new SqlParameter("RptNo", 46),
                    new SqlParameter("State", State),
                    new SqlParameter("District", District),
                    new SqlParameter("Block", Block),
                    new SqlParameter("Village", Village),
                    new SqlParameter("Panchayat", Panchayat),
                    new SqlParameter("CensusYear", CensusYear),
                    new SqlParameter("Type1", Type1),
                    new SqlParameter("Type2", '%'),
                    new SqlParameter("Type3", '%'),
                    new SqlParameter("Type4", '%'),
                    new SqlParameter("Type5", '%'),
                    new SqlParameter("Active", qmItemActive)
                    ).ToList<USP_QM_Items_REPORT_Result>();

                totalRecords = lstMasterReport.Count();
                PropertyInfo propertyInfo = typeof(USP_QM_Items_REPORT_Result).GetProperty(sidx.Trim());
                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstMasterReport = lstMasterReport.OrderBy(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        lstMasterReport = lstMasterReport.OrderByDescending(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_QM_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return lstMasterReport.Select(x => new
                {
                    cell = new[]{
                        x.MAST_QM_TYPE.ToString(),                      
                        x.MAST_ITEM_NAME.ToString(), 
                        x.MAST_ITEM_ACTIVE.ToString(),   
                        x.MAST_ITEM_ACTIVATION_DATE.ToString(),  
                        x.MAST_ITEM_DEACTIVATION_DATE.ToString(),
                        x.MAST_ITEM_STATUS.ToString(),
                        x.MAST_GRADE_NAME.ToString()                      
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

        public Array TechnologyListingDAL(int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSYEntities();
            int State = 0;
            int District = 0;
            int Block = 0;
            int Village = 0;
            int Panchayat = 0;
            int CensusYear = 0;
            try
            {
                List<USP_MAS_Technology_REPORT_Result> lstMasterReport = new List<USP_MAS_Technology_REPORT_Result>();
                lstMasterReport = dbContext.Database.SqlQuery<USP_MAS_Technology_REPORT_Result>(
                    "EXEC [omms].[USP_MAS_DATA_REPORT] @RptNo,@State,@District,@Block,@Village,@Panchayat,@CensusYear,@Type1,@Type2,@Type3,@Type4,@Type5,@Active",
                    new SqlParameter("RptNo", 49),
                    new SqlParameter("State", State),
                    new SqlParameter("District", District),
                    new SqlParameter("Block", Block),
                    new SqlParameter("Village", Village),
                    new SqlParameter("Panchayat", Panchayat),
                    new SqlParameter("CensusYear", CensusYear),
                    new SqlParameter("Type1", '%'),
                    new SqlParameter("Type2", '%'),
                    new SqlParameter("Type3", '%'),
                    new SqlParameter("Type4", '%'),
                    new SqlParameter("Type5", '%'),
                    new SqlParameter("Active", 'Y')
                    ).ToList<USP_MAS_Technology_REPORT_Result>();

                totalRecords = lstMasterReport.Count();
                PropertyInfo propertyInfo = typeof(USP_MAS_Technology_REPORT_Result).GetProperty(sidx.Trim());
                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstMasterReport = lstMasterReport.OrderBy(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        lstMasterReport = lstMasterReport.OrderByDescending(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_TECH_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return lstMasterReport.Select(x => new
                {
                    cell = new[]{
                        x.MAST_TECH_NAME.ToString(),                      
                        x.MAST_TECH_DESC.ToString(), 
                        x.MAST_TECH_STATUS.ToString()                                          
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
        public Array TestListingDAL(int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSYEntities();
            int State = 0;
            int District = 0;
            int Block = 0;
            int Village = 0;
            int Panchayat = 0;
            int CensusYear = 0;
            try
            {
                List<USP_MAS_Test_REPORT_Result> lstMasterReport = new List<USP_MAS_Test_REPORT_Result>();
                lstMasterReport = dbContext.Database.SqlQuery<USP_MAS_Test_REPORT_Result>(
                    "EXEC [omms].[USP_MAS_DATA_REPORT] @RptNo,@State,@District,@Block,@Village,@Panchayat,@CensusYear,@Type1,@Type2,@Type3,@Type4,@Type5,@Active",
                    new SqlParameter("RptNo", 50),
                    new SqlParameter("State", State),
                    new SqlParameter("District", District),
                    new SqlParameter("Block", Block),
                    new SqlParameter("Village", Village),
                    new SqlParameter("Panchayat", Panchayat),
                    new SqlParameter("CensusYear", CensusYear),
                    new SqlParameter("Type1", '%'),
                    new SqlParameter("Type2", '%'),
                    new SqlParameter("Type3", '%'),
                    new SqlParameter("Type4", '%'),
                    new SqlParameter("Type5", '%'),
                    new SqlParameter("Active", 'Y')
                    ).ToList<USP_MAS_Test_REPORT_Result>();

                totalRecords = lstMasterReport.Count();
                PropertyInfo propertyInfo = typeof(USP_MAS_Test_REPORT_Result).GetProperty(sidx.Trim());
                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstMasterReport = lstMasterReport.OrderBy(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        lstMasterReport = lstMasterReport.OrderByDescending(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_TEST_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return lstMasterReport.Select(x => new
                {
                    cell = new[]{
                        x.MAST_TEST_NAME.ToString(),                      
                        x.MAST_TEST_DESC.ToString(), 
                        x.MAST_TEST_STATUS.ToString()                                          
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
     
        public Array FeedbackListingDAL(int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSYEntities();
            int State = 0;
            int District = 0;
            int Block = 0;
            int Village = 0;
            int Panchayat = 0;
            int CensusYear = 0;
            try
            {
                List<USP_MAS_Feedback_REPORT_Result> lstMasterReport = new List<USP_MAS_Feedback_REPORT_Result>();
                lstMasterReport = dbContext.Database.SqlQuery<USP_MAS_Feedback_REPORT_Result>(
                    "EXEC [omms].[USP_MAS_DATA_REPORT] @RptNo,@State,@District,@Block,@Village,@Panchayat,@CensusYear,@Type1,@Type2,@Type3,@Type4,@Type5,@Active",
                    new SqlParameter("RptNo", 55),
                    new SqlParameter("State", State),
                    new SqlParameter("District", District),
                    new SqlParameter("Block", Block),
                    new SqlParameter("Village", Village),
                    new SqlParameter("Panchayat", Panchayat),
                    new SqlParameter("CensusYear", CensusYear),
                    new SqlParameter("Type1", '%'),
                    new SqlParameter("Type2", '%'),
                    new SqlParameter("Type3", '%'),
                    new SqlParameter("Type4", '%'),
                    new SqlParameter("Type5", '%'),
                    new SqlParameter("Active", 'Y')
                    ).ToList<USP_MAS_Feedback_REPORT_Result>();

                totalRecords = lstMasterReport.Count();
                PropertyInfo propertyInfo = typeof(USP_MAS_Feedback_REPORT_Result).GetProperty(sidx.Trim());
                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstMasterReport = lstMasterReport.OrderBy(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        lstMasterReport = lstMasterReport.OrderByDescending(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    lstMasterReport = lstMasterReport.OrderBy(x => x.MAST_FEED_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return lstMasterReport.Select(x => new
                {
                    cell = new[]{
                        x.MAST_FEED_NAME.ToString()              
                                                         
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

    public interface IMasterReportsDAL
    {
        Array StateDetailsListingDAL(string stateOrUnion, string stateType, string activeType, int page, int rows, string sidx, string sord, out Int32 totalRecords);
        Array DistrictDetailsListingDAL(int stateCode, string pmgsyIncluded, string iapDistrict, string activeType, int page, int rows, string sidx, string sord, out Int32 totalRecords);
        Array BlockDetailsListingDAL(int districtCode, int stateCode, string isDesert, string isTribal, string pmgsyIncluded, string isSchedule5, string activeType, int page, int rows, string sidx, string sord, out int totalRecords);
        Array VillageDetailsListingDAL(int censusYear, int blockCode, int districtCode, int stateCode, string isSchedule5, string activeType, int page, int rows, string sidx, string sord, out Int32 totalRecords);
        Array HabitationDetailsListingDAL(int censusYear, int villageCode, int blockCode, int districtCode, int stateCode, string habitationStatus, string isSchedule5, string activeType, int page, int rows, string sidx, string sord, out Int32 totalRecords);
        Array MPConstituencyListingDAL(int stateCode, string activeType, int page, int rows, string sidx, string sord, out Int32 totalRecords);
        Array MLAConstituencyListingDAL(int stateCode, string activeType, int page, int rows, string sidx, string sord, out Int32 totalRecords);
        Array MPBlockListingDAL(int constCode, int stateCode, string activeType, int page, int rows, string sidx, string sord, out Int32 totalRecords);
        Array MLABlockListingDAL(int constCode, int stateCode, string activeType, int page, int rows, string sidx, string sord, out Int32 totalRecords);
        Array PanchayatListingDAL(int blockCode, int districtCode, int stateCode, string activeType, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PanchayatHabitationListingDAL(int panchayatCode, int blockCode, int districtCode, int stateCode, string activeType, int page, int rows, string sidx, string sord, out int totalRecords);
        Array RegionListingDAL(int stateCode, string activeType, int page, int rows, string sidx, string sord, out int totalRecords);
        Array RegionDistrictListingDAL(int regionCode, int stateCode, string activeType, int page, int rows, string sidx, string sord, out int totalRecords);
        Array UnitListingDAL(int page, int rows, string sidx, string sord, out int totalRecords);
        Array RoadCategoryListingDAL(int page, int rows, string sidx, string sord, out int totalRecords);
        Array ScourFoundationListingDAL(string scourFoundationType, int page, int rows, string sidx, string sord, out int totalRecords);
        Array SoilListingDAL(int page, int rows, string sidx, string sord, out int totalRecords);
        Array StreamListingDAL(string streamType, int page, int rows, string sidx, string sord, out int totalRecords);
        Array TerrainListingDAL(int page, int rows, string sidx, string sord, out int totalRecords);
        Array CDWorksLengthListingDAL(int page, int rows, string sidx, string sord, out int totalRecords);
        Array CDWorksTypeListingDAL(int page, int rows, string sidx, string sord, out int totalRecords);
        Array ComponentListingDAL(int page, int rows, string sidx, string sord, out int totalRecords);
        Array GradeListingDAL(int page, int rows, string sidx, string sord, out int totalRecords);
        Array DesignationListingDAL(string designationType, int page, int rows, string sidx, string sord, out int totalRecords);
        Array ReasonListingDAL(string reasonType, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CheckListPointListingDAL(string checkListActive, int page, int rows, string sidx, string sord, out int totalRecords);
        Array ExecutionItemListingDAL(string headType, int page, int rows, string sidx, string sord, out int totalRecords);
        Array TaxesListingDAL(int page, int rows, string sidx, string sord, out int totalRecords);
        Array AgencyListingDAL(string agencyType, int page, int rows, string sidx, string sord, out int totalRecords);
        Array TrafficListingDAL(string trafficStatus, int page, int rows, string sidx, string sord, out int totalRecords);
        Array FundingAgencyListingDAL(int page, int rows, string sidx, string sord, out int totalRecords);
        Array QualificationListingDAL(int page, int rows, string sidx, string sord, out int totalRecords);
        Array LokSabhaTermListingDAL(int page, int rows, string sidx, string sord, out int totalRecords);
        Array ContractorSupplierListingDAL(int stateCode,string contractorSupplierFlag, string contractStatus, int page, int rows, string sidx, string sord, out int totalRecords);

        Array AutonomousBodyListingDAL(int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);

        Array ContractorClassTypeListingDAL(int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);

        Array ContractorRegistrationListingDAL(string activeStatus, string registrationStatus, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);

        Array SQCListingDAL(string activeStatus, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);

        Array SRRDAListingDAL(int stateCode, int agencyCode, string officeType, int page, int rows, string sidx, string sord, out int totalRecords);

        Array VidhanSabhaTermListingDAL(int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);

        Array NodalOfficerListingDAL(int stateCode, int agencyCode, string officeType, string activeType, int page, int rows, string sidx, string sord, out int totalRecords);

        Array QualityMonitorListingDAL(int stateCode,string qmType,string activeType, int page, int rows, string sidx, string sord, out int totalRecords);

        Array TechnicalAgencyListingDAL(int stateCode, string taType, int page, int rows, string sidx, string sord, out int totalRecords);
        Array TechnicalAgencyStateMappingListingDAL(int stateCode, string taType, string activeType, int page, int rows, string sidx, string sord, out int totalRecords);

        Array MLAMemberListingDAL(string constituency, string term, int stateCode, string activeType, int page, int rows, string sidx, string sord, out int totalRecords);

        Array MPMemberListingDAL(string constituency, string term, int stateCode, string activeType, int page, int rows, string sidx, string sord, out int totalRecords);

        Array SurfaceListingDAL(int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array OfficerCategoryListingDAL(int page, int rows, string sidx, string sord, out int totalRecords);
        Array ContractorRegistrationBankListingDAL(int stateCode, string contractRegFlag, string contractStatus, int page, int rows, string sidx, string sord, out int totalRecords);
        Array DepartmentDistrictListingDAL(int stateCode, int agencyCode, string officeType,int page, int rows, string sidx, string sord, out int totalRecords);
        Array CarriageListingDAL(string carriageStatus, int page, int rows, string sidx, string sord, out int totalRecords);
        Array QMItemsListingDAL(string qmType, string qmItemActive, int page, int rows, string sidx, string sord, out int totalRecords);
        Array TechnologyListingDAL(int page, int rows, string sidx, string sord, out int totalRecords);
        Array TestListingDAL(int page, int rows, string sidx, string sord, out int totalRecords);
        Array FeedbackListingDAL(int page, int rows, string sidx, string sord, out int totalRecords);

    }
}