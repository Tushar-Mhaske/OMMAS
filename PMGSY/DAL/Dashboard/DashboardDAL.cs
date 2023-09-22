#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   DashboardDAL.cs        
        * Description   :   Data Methods for Financial Report, Status Monitoring, General Information, 
        *                   Balancesheet, Annual account, Cummulative Expenditure, Implementation Summary .
        * Author        :   Shyam Yadav 
        * Creation Date :   20/Sep/2013
 **/
#endregion

using Microsoft.SqlServer.Server;
using PMGSY.Common;
using PMGSY.Extensions;
using PMGSY.Models;
using PMGSY.Models.Dashboard;
using System;
using System.Collections.Generic;
using System.Data.Entity;
//using System.Data.Entity.Common;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Infrastructure;
//using System.Data.Entity.EntityClient;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Common;

namespace PMGSY.DAL.Dashboard
{
    public class DashboardDAL : IDashboardDAL
    {
        private PMGSYEntities dbContext;

        #region Financial Details


        /// <summary>
        /// Lists Fund vs Expenditure Details Based on Role Type for Mord & World Bank
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="fundType"></param>
        /// <returns></returns>
        public Array FundVsExpenditureDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, string fundType)
        {
            dbContext = new PMGSYEntities();
            try
            {
                List<USP_ACC_DB_FUND_Expn_Details_Result> itemList = new List<USP_ACC_DB_FUND_Expn_Details_Result>();
                string param = PMGSYSession.Current.RoleCode == 45 ? "W" : PMGSYSession.Current.RoleCode == 25 ? "M" : string.Empty;
                itemList = dbContext.USP_ACC_DB_FUND_Expn_Details(fundType, param).ToList<USP_ACC_DB_FUND_Expn_Details_Result>();

                totalRecords = itemList.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        itemList = itemList.OrderBy(x => x.Admin_ND_Name).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        itemList = itemList.OrderByDescending(x => x.Admin_ND_Name).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    itemList = itemList.OrderBy(x => x.Admin_ND_Name).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return itemList.Select(itemDetails => new
                {
                    id = itemDetails.Parent_ND_Code.ToString(),
                    cell = new[] {           
                                        "<a href='#' title='Click here to view DPIU details' style='text-decoration:none;' onClick='viewDPIUDetails(\"" + itemDetails.Parent_ND_Code.ToString().Trim() + "\",\"" + itemDetails.Admin_ND_Name.ToString()  +"\"); return false;'>" + itemDetails.Admin_ND_Name + "</a>",
                                        itemDetails.Fund_Received.ToString(),
                                        itemDetails.Auth_Received.ToString(),
                                        itemDetails.Expn.ToString(),
                                        itemDetails.Bank.ToString()
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
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }


        /// <summary>
        /// Completed & In Progress Roads Expenditure Summary
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="fundType"></param>
        /// <param name="stateNDCode"></param>
        /// <param name="piuNDCode"></param>
        /// <param name="fundingAgency"></param>
        /// <returns></returns>
        public Array ExpenditureSummaryDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, string fundType, int stateNDCode, int piuNDCode, int fundingAgency)
        {
            dbContext = new PMGSYEntities();
            try
            {
                List<USP_ACC_DB_Expn_Summary_Result> itemList = new List<USP_ACC_DB_Expn_Summary_Result>();
                string param = PMGSYSession.Current.RoleCode == 45 ? "W" : PMGSYSession.Current.RoleCode == 25 ? "M" : string.Empty;
                
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;
                
                itemList = dbContext.USP_ACC_DB_Expn_Summary(fundType, param, stateNDCode, piuNDCode, fundingAgency).ToList<USP_ACC_DB_Expn_Summary_Result>();

                totalRecords = itemList.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        itemList = itemList.OrderBy(x => x.DESC).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        itemList = itemList.OrderByDescending(x => x.DESC).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    itemList = itemList.OrderBy(x => x.DESC).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return itemList.Select(itemDetails => new
                {
                    id = itemDetails.HEAD_ID.ToString(),
                    cell = new[] {       
                                        itemDetails.DESC,
                                        itemDetails.EXPN.ToString()
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
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }


        /// <summary>
        /// returns SRRDA wise - No. of PIUs, No. of PIUs Closed Accounts, Closed Till Month & Un Reconciled details
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="stateNDCode"></param>
        /// <param name="year"></param>
        /// <param name="durationType"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public Array StatusMonitoringReportDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, string fundType)
        {
            dbContext = new PMGSYEntities();
            try
            {
                List<USP_ACC_DB_Status_Monitoring_Result> itemList = new List<USP_ACC_DB_Status_Monitoring_Result>();
                string param = PMGSYSession.Current.RoleCode == 45 ? "W" : PMGSYSession.Current.RoleCode == 25 ? "M" : string.Empty;

                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;
                
                itemList = dbContext.USP_ACC_DB_Status_Monitoring(fundType, param).ToList<USP_ACC_DB_Status_Monitoring_Result>();

                totalRecords = itemList.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        itemList = itemList.OrderBy(x => x.Srrda_Name).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        itemList = itemList.OrderByDescending(x => x.Srrda_Name).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    itemList = itemList.OrderBy(x => x.Srrda_Name).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return itemList.Select(itemDetails => new
                {
                    id = itemDetails.MAST_PARENT_ND_CODE.ToString(),
                    cell = new[] {       
                                        itemDetails.Srrda_Name,
                                        itemDetails.No_of_PIUS.ToString(),
                                        itemDetails.Closed_PIUS_LM.ToString(),
                                        itemDetails.NA_CLOSING_MONTH.ToString(),
                                        itemDetails.UnReconcile_Auth.ToString(),
                                        itemDetails.UnReconcile_Bal.ToString()
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
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        
        /// <summary>
        /// Returns PIU wise -Last Month Closed, Closed Till Month, Auth Received & Expenditure
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="stateNDCode"></param>
        /// <param name="fundType"></param>
        /// <returns></returns>
        public Array StatusMonitoringPIUReportDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateNDCode, string fundType)
        {
            dbContext = new PMGSYEntities();
            try
            {
                List<USP_ACC_DB_DPIU_STATUS_MONITORING_Result> itemList = new List<USP_ACC_DB_DPIU_STATUS_MONITORING_Result>();

                itemList = dbContext.USP_ACC_DB_DPIU_STATUS_MONITORING(fundType, stateNDCode).ToList<USP_ACC_DB_DPIU_STATUS_MONITORING_Result>();

                totalRecords = itemList.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        itemList = itemList.OrderBy(x => x.ADMIN_ND_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        itemList = itemList.OrderByDescending(x => x.ADMIN_ND_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    itemList = itemList.OrderBy(x => x.ADMIN_ND_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return itemList.Select(itemDetails => new
                {
                    id = itemDetails.ADMIN_ND_CODE.ToString(),
                    cell = new[] {       
                                        "<a href='#' title='Click here to view DPIU wise Expenditure Details' style='text-decoration:none;' onClick='viewDPIUExpSummaryAndTrend(\"" + itemDetails.ADMIN_ND_CODE.ToString().Trim() + "\",\"" + itemDetails.ADMIN_ND_NAME.ToString()  +"\"); return false;'>" + itemDetails.ADMIN_ND_NAME + "</a>",
                                        itemDetails.Last_Closing_Month,
                                        itemDetails.Auth_Received.ToString(),
                                        itemDetails.Expn.ToString()
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
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }


        /// <summary>
        /// returns resultset for Yearwise & Cumulative Expenditure Details
        /// </summary>
        /// <param name="stateNDCode"></param>
        /// <param name="districtCode"></param>
        /// <param name="piuCode"></param>
        /// <param name="collaboration"></param>
        /// <param name="fundType"></param>
        /// <returns></returns>
        public List<USP_ACC_DB_Expn_Trend_Result> ExpenditureTrendDAL(string fundType, int stateNDCode, int piuCode, int collaboration)
        {
            try
            {
                dbContext = new PMGSYEntities();
                string param = PMGSYSession.Current.RoleCode == 45 ? "W" : PMGSYSession.Current.RoleCode == 25 ? "M" : string.Empty;
                List<USP_ACC_DB_Expn_Trend_Result> resultlist = dbContext.USP_ACC_DB_Expn_Trend(fundType, param, stateNDCode, piuCode, collaboration == -1 ? 0 : collaboration).ToList<USP_ACC_DB_Expn_Trend_Result>();
                return resultlist;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw ex;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }

        }


        #endregion

     
        #region Physical(Technical) Details


        /// <summary>
        /// All States Technical Details
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="fundingAgency"></param>
        /// <returns></returns>
        public Array AllStatesTechnicalDetailsDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int fundingAgency)
        {
            dbContext = new PMGSYEntities();
            try
            {
                List<USP_TECH_DASH_REPORT_Result> itemList = new List<USP_TECH_DASH_REPORT_Result>();
                Int32 stateCode = PMGSYSession.Current.StateCode == 0 ? 0 : PMGSYSession.Current.StateCode;

                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;
                
                itemList = dbContext.USP_TECH_DASH_REPORT(1, stateCode, 0, 0, fundingAgency == -1 ? 0 : fundingAgency, PMGSYSession.Current.PMGSYScheme).ToList<USP_TECH_DASH_REPORT_Result>();

                totalRecords = itemList.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        itemList = itemList.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        itemList = itemList.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    itemList = itemList.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return itemList.Select(itemDetails => new
                {
                    id = itemDetails.LOCATION_CODE.ToString(),
                    cell = new[] {       
                                        "<a href='#' title='Click here to view district details' style='text-decoration:none;' onClick='viewDistrictwiseTechnicalDetails(\"" + itemDetails.LOCATION_CODE.ToString().Trim() + "\",\"" + itemDetails.LOCATION_NAME.ToString()  +"\"); return false;'>" + itemDetails.LOCATION_NAME + "</a>",
                                       
                                        // Sanction
                                        itemDetails.TOTAL_WORKS.ToString(),
                                        itemDetails.TOTAL_LENGTH.ToString(),
                                        //Math.Round(itemDetails.WORK_TOTAL.Value + itemDetails.MAINT_TOTAL.Value,2).ToString(),
                                        itemDetails.WORK_TOTAL.Value.ToString(),
                                        //Awarded
                                        //itemDetails.AWARDED_WORKS.ToString(),
                                        //itemDetails.AWARDED_LEN.ToString(),
                                        //(itemDetails.AWARDED_AMOUNT + itemDetails.AWARDED_MAINT_AMOUNT).ToString(),
                                        
                                        //Completed
                                        itemDetails.COMP_WORKS.ToString(),
                                        itemDetails.COMP_LEN.ToString(),
                                        itemDetails.TOTAL_EXP.ToString(),

                                        //Completed
                                        itemDetails.PROG_WORKS.ToString(),
                                        itemDetails.PROG_LEN.ToString(),
                                        itemDetails.PROG_EXP.ToString(),

                                        //itemDetails.MAINT_EXP.ToString()
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
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }


        /// <summary>
        /// Technical Details of all Districts under particular State
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="fundingAgency"></param>
        /// <returns></returns>
        public Array DistrictwiseTechnicalDetailsDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int fundingAgency)
        {
            dbContext = new PMGSYEntities();
            try
            {
                List<USP_TECH_DASH_DISTRICT_REPORT_Result> itemList = new List<USP_TECH_DASH_DISTRICT_REPORT_Result>();
                stateCode = stateCode == 0 ? PMGSYSession.Current.StateCode : stateCode;
                Int32 districtCode = PMGSYSession.Current.DistrictCode == 0 ? 0 : PMGSYSession.Current.DistrictCode;
                Int32 block = 0;

                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;

                itemList = dbContext.Database.SqlQuery<USP_TECH_DASH_DISTRICT_REPORT_Result>("EXEC omms.USP_TECH_DASH_REPORT @Level,@State,@District,@Block,@Collaboration,@PMGSY",
                       new SqlParameter("Level", 2),
                       new SqlParameter("State", stateCode),
                       new SqlParameter("District", districtCode),
                       new SqlParameter("Block", block),
                       new SqlParameter("Collaboration", fundingAgency == -1 ? 0 : fundingAgency),
                       new SqlParameter("PMGSY", PMGSYSession.Current.PMGSYScheme)
                       ).ToList<USP_TECH_DASH_DISTRICT_REPORT_Result>();

                totalRecords = itemList.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        itemList = itemList.OrderBy(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        itemList = itemList.OrderByDescending(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    itemList = itemList.OrderBy(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return itemList.Select(itemDetails => new
                {
                    id = itemDetails.MAST_DISTRICT_CODE.ToString(),
                    cell = new[] {       
                                        "<a href='#' title='Click here to view block details' style='text-decoration:none;' onClick='viewBlockwiseTechnicalDetails(\"" + itemDetails.MAST_DISTRICT_CODE.ToString().Trim() + "\",\"" + itemDetails.MAST_DISTRICT_NAME.ToString()  + "\",\"" + stateCode.ToString()  +"\"); return false;'>" + itemDetails.MAST_DISTRICT_NAME + "</a>",
                                       
                                        // Sanction
                                        itemDetails.TOTAL_WORKS.ToString(),
                                        itemDetails.TOTAL_LENGTH.ToString(),
                                        (itemDetails.WORK_TOTAL + itemDetails.MAINT_TOTAL).ToString(),

                                        //Awarded
                                        //itemDetails.AWARDED_WORKS.ToString(),
                                        //itemDetails.AWARDED_LEN.ToString(),
                                        //(itemDetails.AWARDED_AMOUNT + itemDetails.AWARDED_MAINT_AMOUNT).ToString(),
                                        
                                        //Completed
                                        itemDetails.COMP_WORKS.ToString(),
                                        itemDetails.COMP_LEN.ToString(),
                                        itemDetails.TOTAL_EXP.ToString(),

                                        //Completed
                                        itemDetails.PROG_WORKS.ToString(),
                                        itemDetails.PROG_LEN.ToString(),
                                        itemDetails.PROG_EXP.ToString(),

                                        itemDetails.MAINT_EXP.ToString()
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
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }


        /// <summary>
        /// Technical Details of all Blocks under particular District
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="stateCode"></param>
        /// <param name="districtCode"></param>
        /// <param name="fundingAgency"></param>
        /// <returns></returns>
        public Array BlockwiseTechnicalDetailsDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int districtCode, int fundingAgency)
        {
            dbContext = new PMGSYEntities();
            try
            {
                List<USP_TECH_DASH_BLOCK_REPORT_Result> itemList = new List<USP_TECH_DASH_BLOCK_REPORT_Result>();
                stateCode = stateCode == 0 ? PMGSYSession.Current.StateCode : stateCode;
                districtCode = districtCode == 0 ? PMGSYSession.Current.DistrictCode : districtCode;
                Int32 block = 0;

                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;

                itemList = dbContext.Database.SqlQuery<USP_TECH_DASH_BLOCK_REPORT_Result>("EXEC omms.USP_TECH_DASH_REPORT @Level,@State,@District,@Block,@Collaboration,@PMGSY",
                       new SqlParameter("Level", 3),
                       new SqlParameter("State", stateCode),
                       new SqlParameter("District", districtCode),
                       new SqlParameter("Block", block),
                       new SqlParameter("Collaboration", fundingAgency == -1 ? 0 : fundingAgency),
                       new SqlParameter("PMGSY", PMGSYSession.Current.PMGSYScheme)
                       ).ToList<USP_TECH_DASH_BLOCK_REPORT_Result>();

                totalRecords = itemList.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        itemList = itemList.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        itemList = itemList.OrderByDescending(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    itemList = itemList.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return itemList.Select(itemDetails => new
                {
                    id = itemDetails.MAST_BLOCK_CODE.ToString(),
                    cell = new[] {       
                                       itemDetails.MAST_BLOCK_NAME,
                                        
                                       // Sanction
                                        itemDetails.TOTAL_WORKS.ToString(),
                                        itemDetails.TOTAL_LENGTH.ToString(),
                                        (itemDetails.WORK_TOTAL + itemDetails.MAINT_TOTAL).ToString(),

                                        //Awarded
                                        //itemDetails.AWARDED_WORKS.ToString(),
                                        //itemDetails.AWARDED_LEN.ToString(),
                                        //(itemDetails.AWARDED_AMOUNT + itemDetails.AWARDED_MAINT_AMOUNT).ToString(),
                                        
                                        //Completed
                                        itemDetails.COMP_WORKS.ToString(),
                                        itemDetails.COMP_LEN.ToString(),
                                        itemDetails.TOTAL_EXP.ToString(),

                                        //Completed
                                        itemDetails.PROG_WORKS.ToString(),
                                        itemDetails.PROG_LEN.ToString(),
                                        itemDetails.PROG_EXP.ToString(),

                                        itemDetails.MAINT_EXP.ToString()
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



        ///// <summary>
        ///// Sanction Vs Awarded vs Completed vs Progress works Details
        ///// </summary>
        ///// <param name="fundingAgency"></param>
        ///// <returns></returns>
        //public List<USP_TECH_DASH_R1_Result> WorksColumnChartDAL(int stateCode, int districtCode, int fundingAgency)
        //{
        //    dbContext = new PMGSYEntities();
        //    try
        //    {
        //        Int32 level = 0;
        //        if (PMGSYSession.Current.LevelId == 6 || PMGSYSession.Current.LevelId == 3 || PMGSYSession.Current.LevelId == 2)
        //        {
        //            if (districtCode != 0)
        //                level = 3;
        //            else if (stateCode != 0)
        //                level = 2;
        //            else
        //                level = 1;
        //        }
        //        else if (PMGSYSession.Current.LevelId == 4)
        //        {
        //            if (districtCode != 0)
        //                level = 3;
        //            else
        //                level = 2;
        //        }
        //        else if (PMGSYSession.Current.LevelId == 5)
        //        {
        //            level = 3;
        //        }

        //        stateCode = stateCode == 0 ? PMGSYSession.Current.StateCode : stateCode;
        //        districtCode = districtCode == 0 ? PMGSYSession.Current.DistrictCode : districtCode;

        //        ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
        //        dbContext.Configuration.LazyLoadingEnabled = false;

        //        List<USP_TECH_DASH_R1_Result> resultlist = dbContext.USP_TECH_DASH_R1(1, level, stateCode, districtCode, 0, fundingAgency == -1 ? 0 : fundingAgency, PMGSYSession.Current.PMGSYScheme).ToList<USP_TECH_DASH_R1_Result>();
        //        return resultlist;
        //    }
        //    catch (Exception ex)
        //    {
        //        Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
           
        //        throw ex;
        //    }
        //    finally
        //    {
        //        dbContext.Dispose();
        //    }
        //}

       

        ///// <summary>
        ///// Sanction Vs Awarded vs Completed vs Progress Length Details
        ///// </summary>
        ///// <param name="fundingAgency"></param>
        ///// <returns></returns>
        //public List<LengthColumnChartStoredProcModel> LengthColumnChartDAL(int stateCode, int districtCode, int fundingAgency)
        //{
        //    dbContext = new PMGSYEntities();
        //    try
        //    {
        //        Int32 level = 0;
        //        if (PMGSYSession.Current.LevelId == 6 || PMGSYSession.Current.LevelId == 3 || PMGSYSession.Current.LevelId == 2)
        //        {
        //            if (districtCode != 0)
        //                level = 3;
        //            else if (stateCode != 0)
        //                level = 2;
        //            else
        //                level = 1;
        //        }
        //        else if (PMGSYSession.Current.LevelId == 4)
        //        {
        //            if (districtCode != 0)
        //                level = 3;
        //            else
        //                level = 2;
        //        }
        //        else if (PMGSYSession.Current.LevelId == 5)
        //        {
        //            level = 3;
        //        }

        //        stateCode = stateCode == 0 ? PMGSYSession.Current.StateCode : stateCode;
        //        districtCode = districtCode == 0 ? PMGSYSession.Current.DistrictCode : districtCode;
        //        Int32 block = 0;

        //        ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
        //        dbContext.Configuration.LazyLoadingEnabled = false;

        //        List<LengthColumnChartStoredProcModel> resultlist = dbContext.Database.SqlQuery<LengthColumnChartStoredProcModel>("EXEC omms.USP_TECH_DASH_R1 @Report,@Level,@State,@District,@Block,@Collaboration,@PMGSY",
        //                new SqlParameter("Report", 2),    
        //                new SqlParameter("Level", level),
        //                new SqlParameter("State", stateCode),
        //                new SqlParameter("District", districtCode),
        //                new SqlParameter("Block", block),
        //                new SqlParameter("Collaboration", fundingAgency == -1 ? 0 : fundingAgency),
        //                new SqlParameter("PMGSY", PMGSYSession.Current.PMGSYScheme)
        //                ).ToList<LengthColumnChartStoredProcModel>();

        //        return resultlist;
        //    }
        //    catch (Exception ex)
        //    {
        //        Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
        //        throw ex;
        //    }
        //    finally
        //    {
        //        dbContext.Dispose();
        //    }
        //}


        ///// <summary>
        ///// Column chart for Sanction Cost, Awarded Cost, Maintenance Expenditure
        ///// </summary>
        ///// <param name="stateCode"></param>
        ///// <param name="fundingAgency"></param>
        ///// <returns></returns>
        //public List<CostColumnChartStoredProcModel> CostColumnChartDAL(int stateCode, int districtCode, int fundingAgency)
        //{
        //    dbContext = new PMGSYEntities();
        //    try
        //    {
        //        Int32 level = 0;
        //        if (PMGSYSession.Current.LevelId == 6 || PMGSYSession.Current.LevelId == 3 || PMGSYSession.Current.LevelId == 2)
        //        {
        //            if (districtCode != 0)
        //                level = 3;
        //            else if (stateCode != 0)
        //                level = 2;
        //            else
        //                level = 1;
        //        }
        //        else if (PMGSYSession.Current.LevelId == 4)
        //        {
        //            if (districtCode != 0)
        //                level = 3;
        //            else
        //                level = 2;
        //        }
        //        else if (PMGSYSession.Current.LevelId == 5)
        //        {
        //                level = 3;
        //        }

        //        stateCode = stateCode == 0 ? PMGSYSession.Current.StateCode : stateCode;
        //        districtCode = districtCode == 0 ? PMGSYSession.Current.DistrictCode : districtCode;
                
        //        Int32 block = 0;

        //        ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
        //        dbContext.Configuration.LazyLoadingEnabled = false;

        //        List<CostColumnChartStoredProcModel> resultlist = dbContext.Database.SqlQuery<CostColumnChartStoredProcModel>("EXEC omms.USP_TECH_DASH_R1 @Report,@Level,@State,@District,@Block,@Collaboration,@PMGSY",
        //                new SqlParameter("Report", 3),
        //                new SqlParameter("Level", level),
        //                new SqlParameter("State", stateCode),
        //                new SqlParameter("District", districtCode),
        //                new SqlParameter("Block", block),
        //                new SqlParameter("Collaboration", fundingAgency == -1 ? 0 : fundingAgency),
        //                new SqlParameter("PMGSY", PMGSYSession.Current.PMGSYScheme)
        //                ).ToList<CostColumnChartStoredProcModel>();

        //        return resultlist;
        //    }
        //    catch (Exception ex)
        //    {
        //        Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
        //        throw ex;
        //    }
        //    finally
        //    {
        //        dbContext.Dispose();
        //    }
        //}

        public Array WorkLenghtExpYearWiseStateWiseGrid(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int districtCode, int fundingAgency, int year, bool isYearWise)
        {
            dbContext = new PMGSYEntities();
            try
            {   
                stateCode = stateCode == 0 ? PMGSYSession.Current.StateCode : stateCode;
                districtCode = districtCode == 0 ? PMGSYSession.Current.DistrictCode : districtCode;
                Int32 block = 0;
                Int32 level = isYearWise ? 2 : 1;  //2-Yearly, 1 -StateWise

                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;

                List<USP_DSS_ACHIEVEMENT_REPORT_Result> resultlist = dbContext.USP_DSS_ACHIEVEMENT_REPORT(level, stateCode, districtCode, 0, year, 0, (fundingAgency == -1 ? 0 : fundingAgency), PMGSYSession.Current.PMGSYScheme).ToList<USP_DSS_ACHIEVEMENT_REPORT_Result>();

                totalRecords = resultlist.Count();

                //if (sidx.Trim() != string.Empty)
                //{
                //    if (sord.ToString() == "asc")
                //    {
                //        resultlist = resultlist.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                //    }
                //    else
                //    {
                //        resultlist = resultlist.OrderByDescending(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                //    }
                //}
                //else
                //{
                //    itemList = itemList.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                //}

                return resultlist.Select(itemDetails => new
                {
                    id = itemDetails.LOCATION_CODE.ToString(),
                    cell = new[] {       
                                    // itemDetails.LOCATION_CODE.ToString(),
                                     itemDetails.LOCATION_NAME,
                                     itemDetails.IMS_YEAR,
                                     itemDetails.PROPOSALS.ToString(),
                                     itemDetails.LENGTH_COMPLETED.ToString(),
                                     itemDetails.EXPENDITURE.ToString()
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


        /// <summary>
        /// Added by Abhishek kamble 12Mar2015
        /// Work vs Lenght vs Expenditure details
        /// </summary>
        /// <param name="stateCode"></param>
        /// <param name="districtCode"></param>
        /// <param name="fundingAgency"></param>
        /// <returns></returns>
        public List<USP_DSS_ACHIEVEMENT_REPORT_Result> WorksLengthExpYearWiseStateWiseColumnChartDAL(int stateCode, int districtCode, int fundingAgency, int year,bool isYearWise)
        {
            dbContext = new PMGSYEntities();
            try
            {
                Int32 level = isYearWise ? 2 : 1;  //2-Yearly, 1 -StateWise

                //Int32 level = 0;
                //if (PMGSYSession.Current.LevelId == 6 || PMGSYSession.Current.LevelId == 3 || PMGSYSession.Current.LevelId == 2)
                //{
                //    if (districtCode != 0)
                //        level = 3;
                //    else if (stateCode != 0)
                //        level = 2;
                //    else
                //        level = 1;
                //}
                //else if (PMGSYSession.Current.LevelId == 4)
                //{
                //    if (districtCode != 0)
                //        level = 3;
                //    else
                //        level = 2;
                //}
                //else if (PMGSYSession.Current.LevelId == 5)
                //{
                //    level = 3;
                //}

                stateCode = stateCode == 0 ? PMGSYSession.Current.StateCode : stateCode;
                districtCode = districtCode == 0 ? PMGSYSession.Current.DistrictCode : districtCode;

                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;

                List<USP_DSS_ACHIEVEMENT_REPORT_Result> resultlist = dbContext.USP_DSS_ACHIEVEMENT_REPORT(level, stateCode, districtCode, 0,year, 0, (fundingAgency == -1 ? 0 : fundingAgency), PMGSYSession.Current.PMGSYScheme).ToList<USP_DSS_ACHIEVEMENT_REPORT_Result>();

                return resultlist;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);

                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        #endregion


        #region QUality Details


        /// <summary>
        /// All States  NQM SQM wise Insp Count & Grading %
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="fundingAgency"></param>
        /// <returns></returns>
        public Array AllStatesInspectionDetailsDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords)
        {
            dbContext = new PMGSYEntities();
            try
            {
                List<USP_QM_DASH_S1_Result> itemList = new List<USP_QM_DASH_S1_Result>();
                itemList = dbContext.USP_QM_DASH_S1(1, 0, 0, 0).ToList<USP_QM_DASH_S1_Result>();

                totalRecords = itemList.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        itemList = itemList.OrderBy(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        itemList = itemList.OrderByDescending(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    itemList = itemList.OrderBy(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return itemList.Select(itemDetails => new
                {
                    id = itemDetails.MAST_STATE_CODE.ToString(),
                    cell = new[] {       
                                        "<a href='#' title='Click here to view district details' style='text-decoration:none;' onClick='viewDistrictwiseQualityDetails(\"" + itemDetails.MAST_STATE_CODE.ToString().Trim() + "\",\"" + itemDetails.MAST_STATE_NAME.ToString()  +"\"); return false;'>" + itemDetails.MAST_STATE_NAME + "</a>",
                                        //itemDetails.MAST_STATE_NAME,
                                        itemDetails.I_TOTAL.ToString(),
                                        itemDetails.S_TOTAL.ToString(),

                                        itemDetails.I_GRADE_1_PER.ToString(),
                                        itemDetails.I_GRADE_2_PER.ToString(),
                                        itemDetails.I_GRADE_3_PER.ToString(),
                                        
                                        itemDetails.S_GRADE_1_PER.ToString(),
                                        itemDetails.S_GRADE_2_PER.ToString(),
                                        itemDetails.S_GRADE_3_PER.ToString()
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



        /// <summary>
        /// District wise, NQM SQM wise Insp Count & Grading %
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="stateCode"></param>
        /// <returns></returns>
        public Array DistrictwiseInspectionDetailsDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode)
        {
            dbContext = new PMGSYEntities();
            try
            {
                List<USP_QM_DISTRICT_DASH_S1_Result> itemList = new List<USP_QM_DISTRICT_DASH_S1_Result>();
                stateCode = stateCode == 0 ? PMGSYSession.Current.StateCode : stateCode;
                int districtCode = 0;
                int blockCode = 0;

                itemList = dbContext.Database.SqlQuery<USP_QM_DISTRICT_DASH_S1_Result>("EXEC omms.USP_QM_DASH_S1 @LEVEL,@STATE,@DISTRICT,@BLOCK",
                       new SqlParameter("LEVEL", 2),
                       new SqlParameter("STATE", stateCode),
                       new SqlParameter("DISTRICT", districtCode),
                       new SqlParameter("BLOCK", blockCode)
                       ).ToList<USP_QM_DISTRICT_DASH_S1_Result>();

                totalRecords = itemList.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        itemList = itemList.OrderBy(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        itemList = itemList.OrderByDescending(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    itemList = itemList.OrderBy(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return itemList.Select(itemDetails => new
                {
                    id = itemDetails.MAST_DISTRICT_CODE.ToString(),
                    cell = new[] {       
                                        //"<a href='#' title='Click here to view district details' style='text-decoration:none;' onClick='viewDistrictwiseTechnicalDetails(\"" + itemDetails.MAST_STATE_CODE.ToString().Trim() + "\",\"" + itemDetails.MAST_STATE_NAME.ToString()  +"\"); return false;'>" + itemDetails.MAST_STATE_NAME + "</a>",
                                        itemDetails.MAST_DISTRICT_NAME,
                                        itemDetails.I_TOTAL.ToString(),
                                        itemDetails.S_TOTAL.ToString(),

                                        itemDetails.I_GRADE_1_PER.ToString(),
                                        itemDetails.I_GRADE_2_PER.ToString(),
                                        itemDetails.I_GRADE_3_PER.ToString(),
                                        
                                        itemDetails.S_GRADE_1_PER.ToString(),
                                        itemDetails.S_GRADE_2_PER.ToString(),
                                        itemDetails.S_GRADE_3_PER.ToString()
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


        /// <summary>
        /// Monitorwise Completed & In Progress Roads Grading Details
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="qmType"></param>
        /// <returns></returns>
        public Array MonitorwiseComplAndProgressDetailsDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, string qmType)
        {
            dbContext = new PMGSYEntities();
            try
            {
                List<USP_QM_DASH_S2_Result> itemList = new List<USP_QM_DASH_S2_Result>();
                stateCode = stateCode == 0 ? PMGSYSession.Current.StateCode : stateCode;
                itemList = dbContext.USP_QM_DASH_S2(stateCode, qmType).ToList<USP_QM_DASH_S2_Result>();

                totalRecords = itemList.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        itemList = itemList.OrderBy(x => x.QM_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        itemList = itemList.OrderByDescending(x => x.QM_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    itemList = itemList.OrderBy(x => x.QM_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return itemList.Select(itemDetails => new
                {
                    id = itemDetails.ADMIN_QM_CODE.ToString(),
                    cell = new[] {       
                                        "<a href='#' title='Click here to view monitors grading details' style='text-decoration:none;' onClick='viewMonitorwiseGradingColumnChart(\"" + itemDetails.ADMIN_QM_CODE.ToString().Trim() + "\",\"" + itemDetails.QM_NAME.ToString() + "\",\"" + qmType + "\",\"" + stateCode  +"\"); return false;'>" + itemDetails.QM_NAME + "</a>",
                                        //itemDetails.QM_NAME,
   
                                        itemDetails.GRADE_C1.ToString(),
                                        itemDetails.GRADE_C2.ToString(),
                                        itemDetails.GRADE_C3.ToString(),
                                        itemDetails.C_TOTAL.ToString(),
                                        
                                        itemDetails.GRADE_P1.ToString(),
                                        itemDetails.GRADE_P2.ToString(),
                                        itemDetails.GRADE_P3.ToString(),
                                        itemDetails.P_TOTAL.ToString()
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



        /// <summary>
        /// All States Grading Pie Chart for percentage of S, RI, U
        /// </summary>
        /// <param name="stateCode"></param>
        /// <param name="qmType"></param>
        /// <returns></returns>
        public List<AllStatesQualityPieChartModel> AllStatesGradingPieChartDAL(int stateCode, string qmType)
        {
            dbContext = new PMGSYEntities();
            List<AllStatesQualityPieChartModel> lstChart = new List<AllStatesQualityPieChartModel>();
            try
            {
                decimal grandTotalOfGrade1 = 0;
                decimal grandTotalOfGrade2 = 0;
                decimal grandTotalOfGrade3 = 0;

                int level = 0;
                if (PMGSYSession.Current.LevelId == 6 || PMGSYSession.Current.LevelId == 3 || PMGSYSession.Current.LevelId == 2)
                {
                    if (stateCode != 0)
                        level = 2;
                    else
                        level = 1;
                }
                else if (PMGSYSession.Current.LevelId == 4)
                {
                    level = 2;
                }

                stateCode = PMGSYSession.Current.StateCode == 0 ? stateCode : PMGSYSession.Current.StateCode;
                int districtCode = PMGSYSession.Current.DistrictCode == 0 ? 0 : PMGSYSession.Current.DistrictCode;
                int blockCode = 0;

                //int level = stateCode == 0 ? 1 : 2;
                
                List<USP_QM_STATE_DISTRICT_DASH_S1_Result> itemList = new List<USP_QM_STATE_DISTRICT_DASH_S1_Result>();
                itemList = dbContext.Database.SqlQuery<USP_QM_STATE_DISTRICT_DASH_S1_Result>("EXEC omms.USP_QM_DASH_S1 @LEVEL,@STATE,@DISTRICT,@BLOCK",
                        new SqlParameter("LEVEL", level),
                        new SqlParameter("STATE", stateCode),
                        new SqlParameter("DISTRICT", districtCode),
                        new SqlParameter("BLOCK", blockCode)
                        ).ToList<USP_QM_STATE_DISTRICT_DASH_S1_Result>();
               

                
                if(qmType.Equals("I"))
                {
                    foreach (var p in itemList)
                    {
                         grandTotalOfGrade1 = grandTotalOfGrade1 + Convert.ToDecimal(p.I_GRADE_1_PER); 
                         grandTotalOfGrade2 = grandTotalOfGrade2 + Convert.ToDecimal(p.I_GRADE_2_PER);       
                         grandTotalOfGrade3 = grandTotalOfGrade3 + Convert.ToDecimal(p.I_GRADE_3_PER);       
                    }

                    AllStatesQualityPieChartModel chart = new AllStatesQualityPieChartModel();
                    chart.Name = "Satisfactory";
                    chart.Value = (grandTotalOfGrade1 / itemList.Count);
                    lstChart.Add(chart);

                    AllStatesQualityPieChartModel chart2 = new AllStatesQualityPieChartModel();
                    chart2.Name = "Required Improvement";
                    chart2.Value = grandTotalOfGrade2 / itemList.Count;
                    lstChart.Add(chart2);

                    AllStatesQualityPieChartModel chart3 = new AllStatesQualityPieChartModel();
                    chart3.Name = "UnSatisfactory";
                    chart3.Value = grandTotalOfGrade3 / itemList.Count;
                    lstChart.Add(chart3);
                }
                else if(qmType.Equals("S"))
                {
                    foreach (var p in itemList)
                    {
                         grandTotalOfGrade1 = grandTotalOfGrade1 + Convert.ToDecimal(p.S_GRADE_1_PER); 
                         grandTotalOfGrade2 = grandTotalOfGrade2 + Convert.ToDecimal(p.S_GRADE_2_PER);       
                         grandTotalOfGrade3 = grandTotalOfGrade3 + Convert.ToDecimal(p.S_GRADE_3_PER);       
                    }

                    AllStatesQualityPieChartModel chart = new AllStatesQualityPieChartModel();
                    chart.Name = "Satisfactory";
                    chart.Value = (grandTotalOfGrade1 / itemList.Count);
                    lstChart.Add(chart);

                    AllStatesQualityPieChartModel chart2 = new AllStatesQualityPieChartModel();
                    chart2.Name = "Required Improvement";
                    chart2.Value = grandTotalOfGrade2 / itemList.Count;
                    lstChart.Add(chart2);

                    AllStatesQualityPieChartModel chart3 = new AllStatesQualityPieChartModel();
                    chart3.Name = "UnSatisfactory";
                    chart3.Value = grandTotalOfGrade3 / itemList.Count;
                }

                

                return lstChart;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                  return lstChart;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        /// <summary>
        /// Yearly % Grading for All States 
        /// as well as - In case of Level 4 report all Districts of particular State
        /// </summary>
        /// <param name="stateCode"></param>
        /// <returns></returns>
        public List<USP_QM_STATE_DISTRICT_DASH_S1_Result> YearlyGradingLineChartDAL(int stateCode)
        {
            dbContext = new PMGSYEntities();
            try
            {
                int level = 0;
                if (PMGSYSession.Current.LevelId == 6 || PMGSYSession.Current.LevelId == 3 || PMGSYSession.Current.LevelId == 2)
                {
                    if (stateCode != 0)
                        level = 4;
                    else
                        level = 3;
                }
                else if (PMGSYSession.Current.LevelId == 4)
                {
                        level = 4;
                }
               

                stateCode = PMGSYSession.Current.StateCode == 0 ? stateCode : PMGSYSession.Current.StateCode;
                int districtCode = PMGSYSession.Current.DistrictCode == 0 ? 0 : PMGSYSession.Current.DistrictCode;
                int blockCode = 0;

                //int level = stateCode == 0 ? 3 : 4;

                List<USP_QM_STATE_DISTRICT_DASH_S1_Result> itemList = new List<USP_QM_STATE_DISTRICT_DASH_S1_Result>();
                itemList = dbContext.Database.SqlQuery<USP_QM_STATE_DISTRICT_DASH_S1_Result>("EXEC omms.USP_QM_DASH_S1 @LEVEL,@STATE,@DISTRICT,@BLOCK",
                        new SqlParameter("LEVEL", level),
                        new SqlParameter("STATE", stateCode),
                        new SqlParameter("DISTRICT", districtCode),
                        new SqlParameter("BLOCK", blockCode)
                        ).ToList<USP_QM_STATE_DISTRICT_DASH_S1_Result>();

                return itemList;
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
        /// Monitorwise Grading % in inspected states
        /// </summary>
        /// <param name="stateCode"></param>
        /// <param name="monitorCode"></param>
        /// <param name="qmType"></param>
        /// <returns></returns>
        public List<USP_QM_STATE_DISTRICT_DASH_S3_Result> MonitorsGradingColumnChartDAL(int stateCode, int monitorCode, string qmType)
        {
            dbContext = new PMGSYEntities();
            try
            {
                int level = 0;
                if (PMGSYSession.Current.LevelId == 6 || PMGSYSession.Current.LevelId == 3 || PMGSYSession.Current.LevelId == 2)
                {
                    if (stateCode != 0)
                        level = 2;
                    else
                        level = 1;
                }
                else if (PMGSYSession.Current.LevelId == 4)
                {
                    level = 2;
                }


                stateCode = PMGSYSession.Current.StateCode == 0 ? stateCode : PMGSYSession.Current.StateCode;
               
                List<USP_QM_STATE_DISTRICT_DASH_S3_Result> itemList = new List<USP_QM_STATE_DISTRICT_DASH_S3_Result>();
                itemList = dbContext.Database.SqlQuery<USP_QM_STATE_DISTRICT_DASH_S3_Result>("EXEC omms.USP_QM_DASH_S3 @LEVEL,@STATE,@QMCODE,@QMTYPE",
                        new SqlParameter("LEVEL", level),
                        new SqlParameter("STATE", stateCode),
                        new SqlParameter("QMCODE", monitorCode),
                        new SqlParameter("QMTYPE", qmType)
                        ).ToList<USP_QM_STATE_DISTRICT_DASH_S3_Result>();

                return itemList;
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

        #endregion


        #region NRRDA_DASHBOARDS

        /// <summary>
        /// returns the data of chart according to the report type
        /// </summary>
        /// <param name="stateList">list of states </param>
        /// <param name="year"></param>
        /// <param name="reportType"></param>
        /// <param name="agency"></param>
        /// <param name="collaboration"></param>
        /// <returns></returns>
        public List<USP_TECH_TRENDS_CHART_PPT_Result> GetProposalTrendsDetailsDAL(string stateList, int year, int reportType, int agency, int collaboration)
        {
            dbContext = new PMGSYEntities();

            //IEnumerable<SqlDataRecord> sqlDataRecords = new List<SqlDataRecord>();
            SqlDataRecord record = new SqlDataRecord(new SqlMetaData[] { new SqlMetaData("MAST_STATE_CODE", SqlDbType.Int) });
            //List<SqlDataRecord> recordList = new List<SqlDataRecord>();
            List<USP_TECH_TRENDS_CHART_PPT_Result> lstDetails = new List<USP_TECH_TRENDS_CHART_PPT_Result>();
            string[] stateParams = stateList.Split(',');
            DataTable stateTable = new DataTable();
            stateTable.Columns.Add("MAST_STATE_CODE", typeof(int));

            agency = (agency <= 0 ? 0 : agency);
            collaboration = (collaboration <= 0 ? 0 :collaboration);

            foreach (var item in stateParams)
            {
                int i = 0;
                record.SetInt32(i,Convert.ToInt32(item));
                stateTable.Rows.Add(new object[] { Convert.ToInt32(item) });
                i++;
                //recordList.Add(record);
            }

            //sqlDataRecords = recordList;

            // Create a list of SqlDataRecord objects from your list of entities here
            
            SqlConnection storeConnection = new SqlConnection(dbContext.Database.Connection.ConnectionString);
            try
            {
                using (SqlCommand command = storeConnection.CreateCommand())
                {
                    command.Connection = storeConnection;
                    storeConnection.Open();

                    command.CommandText = "omms.USP_TECH_TRENDS_CHART_PPT";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Report", SqlDbType.Int)).Value = reportType;
                    command.Parameters.Add(new SqlParameter("@Level", SqlDbType.Int)).Value = 1;
                    command.Parameters.Add(new SqlParameter("@District", SqlDbType.Int)).Value = 0;
                    command.Parameters.Add(new SqlParameter("@Block", SqlDbType.Int)).Value = 0;
                    command.Parameters.Add(new SqlParameter("@Year", SqlDbType.Int)).Value = year;
                    command.Parameters.Add(new SqlParameter("@Collaboration", SqlDbType.Int)).Value = collaboration;
                    command.Parameters.Add(new SqlParameter("@PMGSY", SqlDbType.Int)).Value = Convert.ToInt32(PMGSYSession.Current.PMGSYScheme);
                    command.Parameters.Add(new SqlParameter("@Agency", SqlDbType.Int)).Value = agency;
                    
                    command.Parameters.AddWithValue("@Param", stateTable).SqlDbType = SqlDbType.Structured;
                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                USP_TECH_TRENDS_CHART_PPT_Result objResult = new USP_TECH_TRENDS_CHART_PPT_Result();
                                objResult.MAST_YEAR_CODE = reader.GetInt32(0);
                                objResult.MAST_YEAR_TEXT = reader.GetString(1);
                                objResult.LOCATION_CODE = reader.GetInt32(2);
                                objResult.LOCATION_NAME = reader.GetString(3);
                                objResult.ROAD_LEN = reader.GetDecimal(4);
                                objResult.LSB_LEN = reader.GetDecimal(5);
                                lstDetails.Add(objResult);
                            }
                        }
                    }
                }

                return lstDetails;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                storeConnection.Close();
            }
        }

        /// <summary>
        /// returns the details of habitation chart
        /// </summary>
        /// <param name="stateList"></param>
        /// <param name="year"></param>
        /// <param name="currentMonth"></param>
        /// <param name="agency"></param>
        /// <param name="collaboration"></param>
        /// <returns></returns>
        public List<USP_PROP_HAB_PPT_LIST_Result> GetHabitationDetailsMPRDAL(string stateList, int year, int currentMonth, int agency, int collaboration)
        {
            dbContext = new PMGSYEntities();

            try
            {
                IEnumerable<SqlDataRecord> sqlDataRecords = new List<SqlDataRecord>();
                SqlDataRecord record = new SqlDataRecord(new SqlMetaData[] { new SqlMetaData("MAST_STATE_CODE", SqlDbType.Int) });
                List<SqlDataRecord> recordList = new List<SqlDataRecord>();
                List<USP_PROP_HAB_PPT_LIST_Result> lstDetails = new List<USP_PROP_HAB_PPT_LIST_Result>();
                string[] stateParams = stateList.Split(',');
                DataTable stateTable = new DataTable();
                stateTable.Columns.Add("MAST_STATE_CODE", typeof(int));

                agency = (agency <= 0 ? 0 : agency);
                collaboration = (collaboration <= 0 ? 0 : collaboration);
                
                foreach (var item in stateParams)
                {
                    int i = 0;
                    record.SetInt32(i, Convert.ToInt32(item));
                    stateTable.Rows.Add(new object[] { Convert.ToInt32(item) });
                    i++;
                    recordList.Add(record);
                }

                sqlDataRecords = recordList;

                // Create a list of SqlDataRecord objects from your list of entities here

                SqlConnection storeConnection = new SqlConnection(dbContext.Database.Connection.ConnectionString);
                using (SqlCommand command = storeConnection.CreateCommand())
                {
                    command.Connection = storeConnection;
                    storeConnection.Open();

                    command.CommandText = "omms.USP_PROP_HAB_PPT_LIST";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Level", SqlDbType.Int)).Value = 1;
                    command.Parameters.Add(new SqlParameter("@State", SqlDbType.Int)).Value = 0;
                    command.Parameters.Add(new SqlParameter("@District", SqlDbType.Int)).Value = 0;
                    command.Parameters.Add(new SqlParameter("@Block", SqlDbType.Int)).Value = 0;
                    command.Parameters.Add(new SqlParameter("@Year", SqlDbType.Int)).Value = DateTime.Now.Year;
                    command.Parameters.Add(new SqlParameter("@CurMonth", SqlDbType.Int)).Value = currentMonth;
                    command.Parameters.Add(new SqlParameter("@PMGSY", SqlDbType.Int)).Value = 1;
                    command.Parameters.Add(new SqlParameter("@Agency", SqlDbType.Int)).Value = agency;
                    command.Parameters.Add(new SqlParameter("@Collaboration", SqlDbType.Int)).Value = collaboration;

                    command.Parameters.AddWithValue("@Param", stateTable).SqlDbType = SqlDbType.Structured;
                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                USP_PROP_HAB_PPT_LIST_Result objResult = new USP_PROP_HAB_PPT_LIST_Result();
                                objResult.LOCATION_CODE = reader.GetInt32(0);
                                objResult.LOCATION_NAME = reader.GetString(1);
                                objResult.TOTAL_HAB_CLR_NC = reader.GetInt32(2);
                                objResult.TOTAL_HAB_CLR = reader.GetInt32(3);
                                objResult.TOTAL_HABS = reader.GetInt32(4);
                                objResult.TOTAL_HAB_CN = reader.GetInt32(5);
                                objResult.TOTAL_BAL_HA = reader.GetInt32(6);
                                lstDetails.Add(objResult);
                            }
                        }
                    }
                }
                storeConnection.Close();
                return lstDetails;

            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// returns the details of maintenance for the last 4 years
        /// </summary>
        /// <param name="stateList"></param>
        /// <param name="year"></param>
        /// <param name="currentMonth"></param>
        /// <param name="agency"></param>
        /// <param name="collaboration"></param>
        /// <returns></returns>
        public List<USP_PROP_MAINT_PPT_LIST_Result> GetMaintenanceDetailsMPRDAL(string stateList, int year, int currentMonth, int agency, int collaboration)
        {
            dbContext = new PMGSYEntities();

            try
            {
                IEnumerable<SqlDataRecord> sqlDataRecords = new List<SqlDataRecord>();
                SqlDataRecord record = new SqlDataRecord(new SqlMetaData[] { new SqlMetaData("MAST_STATE_CODE", SqlDbType.Int) });
                List<SqlDataRecord> recordList = new List<SqlDataRecord>();
                List<USP_PROP_MAINT_PPT_LIST_Result> lstDetails = new List<USP_PROP_MAINT_PPT_LIST_Result>();
                string[] stateParams = stateList.Split(',');
                DataTable stateTable = new DataTable();
                stateTable.Columns.Add("MAST_STATE_CODE", typeof(int));

                agency = (agency <= 0 ? 0 : agency);
                collaboration = (collaboration <= 0 ? 0 : collaboration);

                foreach (var item in stateParams)
                {
                    int i = 0;
                    record.SetInt32(i, Convert.ToInt32(item));
                    stateTable.Rows.Add(new object[] { Convert.ToInt32(item) });
                    i++;
                    recordList.Add(record);
                }

                sqlDataRecords = recordList;

                // Create a list of SqlDataRecord objects from your list of entities here

                SqlConnection storeConnection = new SqlConnection(dbContext.Database.Connection.ConnectionString);
                using (SqlCommand command = storeConnection.CreateCommand())
                {
                    command.Connection = storeConnection;
                    storeConnection.Open();

                    command.CommandText = "omms.USP_PROP_MAINT_PPT_LIST";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Level", SqlDbType.Int)).Value = 1;
                    command.Parameters.Add(new SqlParameter("@State", SqlDbType.Int)).Value = 0;
                    command.Parameters.Add(new SqlParameter("@District", SqlDbType.Int)).Value = 0;
                    command.Parameters.Add(new SqlParameter("@Block", SqlDbType.Int)).Value = 0;
                    command.Parameters.Add(new SqlParameter("@Year", SqlDbType.Int)).Value = DateTime.Now.Year;
                    command.Parameters.Add(new SqlParameter("@CurMonth", SqlDbType.Int)).Value = currentMonth;
                    command.Parameters.Add(new SqlParameter("@PMGSY", SqlDbType.Int)).Value = 1;
                    command.Parameters.Add(new SqlParameter("@Agency", SqlDbType.Int)).Value = agency;
                    command.Parameters.Add(new SqlParameter("@Collaboration", SqlDbType.Int)).Value = collaboration;

                    command.Parameters.AddWithValue("@Param", stateTable).SqlDbType = SqlDbType.Structured;
                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                USP_PROP_MAINT_PPT_LIST_Result objResult = new USP_PROP_MAINT_PPT_LIST_Result();

                                objResult.LOCATION_CODE = reader.GetInt32(0);
                                objResult.LOCATION_NAME = reader.GetString(1);
                                objResult.FR_Year1 = (reader.GetDecimal(2) == null ? 0 : reader.GetDecimal(2));
                                objResult.FR_Year2 = (reader.GetDecimal(3)== null ? 0 : reader.GetDecimal(3));
                                objResult.FR_Year3 = (reader.GetDecimal(4)== null ? 0 : reader.GetDecimal(4));
                                objResult.FR_Year4 = (reader.GetDecimal(5)== null ? 0 : reader.GetDecimal(5));
                                objResult.FC_Year1 = (reader.GetDecimal(6)== null ? 0 : reader.GetDecimal(6));
                                objResult.FC_Year2 = (reader.GetDecimal(7)== null ? 0 : reader.GetDecimal(7));
                                objResult.FC_Year3 = (reader.GetDecimal(8)== null ? 0 : reader.GetDecimal(8));
                                objResult.FC_Year4 = (reader.GetDecimal(9)== null ? 0 : reader.GetDecimal(9));
                                objResult.FS_Year1 = (reader.GetDecimal(10)== null ? 0 : reader.GetDecimal(10));
                                objResult.FS_Year2 = (reader.GetDecimal(11)== null ? 0 : reader.GetDecimal(11));
                                objResult.FS_Year3 = (reader.GetDecimal(12)== null ? 0 : reader.GetDecimal(12));
                                objResult.FS_Year4 = (reader.GetDecimal(13)== null ? 0 : reader.GetDecimal(13));
                                objResult.FP_Year1 = (reader.GetDecimal(14)== null ? 0 : reader.GetDecimal(14));
                                objResult.FP_Year2 = (reader.GetDecimal(15)== null ? 0 : reader.GetDecimal(15));
                                objResult.FP_Year3 = (reader.GetDecimal(16)== null ? 0 : reader.GetDecimal(16));
                                objResult.FP_Year4 = (reader.GetDecimal(17)== null ? 0 : reader.GetDecimal(17));
                                lstDetails.Add(objResult);
                            }
                        }
                    }
                }
                storeConnection.Close();
                return lstDetails;

            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// populates all government agencies
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> PopulateGovernmentAgencies()
        {
            dbContext = new PMGSYEntities();

            try
            {
                List<SelectListItem> lstData = new List<SelectListItem>();
                var lstDetails = (from ma in dbContext.MASTER_AGENCY 
                             join md in dbContext.ADMIN_DEPARTMENT on ma.MAST_AGENCY_CODE equals md.MAST_AGENCY_CODE 
                             where md.MAST_ND_TYPE == "S"
                             select new
                             {
                                 Text = md.MASTER_STATE.MAST_STATE_NAME+" - " +md.ADMIN_ND_NAME,
                                 Value = ma.MAST_AGENCY_CODE,
                             }).OrderBy(c => c.Text).ToList().Distinct();

                lstData = new SelectList(lstDetails, "Value", "Text").ToList();

                lstData.Insert(0, new SelectListItem { Value = "0" , Text = "All Agencies"});

                return lstData;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        #endregion

    }

   
    public interface IDashboardDAL
    {
        #region Financial Details

        Array FundVsExpenditureDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, string fundType);
        Array ExpenditureSummaryDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, string fundType, int stateNDCode, int piuNDCode, int fundingAgency);
        Array StatusMonitoringReportDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, string fundType);
        Array StatusMonitoringPIUReportDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateNDCode, string fundType);

        List<USP_ACC_DB_Expn_Trend_Result> ExpenditureTrendDAL(string fundType, int stateNDCode, int piuCode, int collaboration);

        #endregion

   
        #region Physical(Technical) Details

        Array AllStatesTechnicalDetailsDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int fundingAgency);
        Array DistrictwiseTechnicalDetailsDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int fundingAgency);
        Array BlockwiseTechnicalDetailsDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int districtCode, int fundingAgency);
        //List<USP_TECH_DASH_R1_Result> WorksColumnChartDAL(int stateCode, int districtCode, int fundingAgency);
        //List<LengthColumnChartStoredProcModel> LengthColumnChartDAL(int stateCode, int districtCode, int fundingAgency);
        //List<CostColumnChartStoredProcModel> CostColumnChartDAL(int stateCode, int districtCode, int fundingAgency);

        Array WorkLenghtExpYearWiseStateWiseGrid(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int districtCode, int fundingAgency, int year, bool isYearWise);
        List<USP_DSS_ACHIEVEMENT_REPORT_Result> WorksLengthExpYearWiseStateWiseColumnChartDAL(int stateCode, int districtCode, int fundingAgency,int year,bool isYearWise);
        #endregion

       
        #region Quality Details

        Array AllStatesInspectionDetailsDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords);
        Array DistrictwiseInspectionDetailsDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode);
        Array MonitorwiseComplAndProgressDetailsDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, string qmType);
        List<AllStatesQualityPieChartModel> AllStatesGradingPieChartDAL(int stateCode, string qmType);
        List<USP_QM_STATE_DISTRICT_DASH_S1_Result> YearlyGradingLineChartDAL(int stateCode);
        List<USP_QM_STATE_DISTRICT_DASH_S3_Result> MonitorsGradingColumnChartDAL(int stateCode, int monitorCode, string qmType);
        #endregion


        #region NRRDA_DASHBOARDS

        List<USP_TECH_TRENDS_CHART_PPT_Result> GetProposalTrendsDetailsDAL(string stateList, int year, int reportType, int agency, int collaboration);
        List<USP_PROP_HAB_PPT_LIST_Result> GetHabitationDetailsMPRDAL(string stateList, int year, int currentMonth, int agency, int collaboration);
        List<USP_PROP_MAINT_PPT_LIST_Result> GetMaintenanceDetailsMPRDAL(string stateList, int year, int currentMonth, int agency, int collaboration);

        #endregion
    }
}