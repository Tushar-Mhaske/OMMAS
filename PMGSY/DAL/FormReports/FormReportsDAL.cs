#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   FormReportsDAL.cs        
        * Description   :   Data Accessing Logic for all types of Form Reports
        * Author        :   Shyam Yadav 
        * Creation Date :   28/August/2013
 **/
#endregion

using PMGSY.Extensions;
using PMGSY.Models;
using PMGSY.Models.FormReports;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace PMGSY.DAL.FormReports
{
    public class FormReportsDAL : IFormReportsDAL
    {
        Models.PMGSYEntities dbContext;



        #region Form1

        /// <summary>
        /// List the Form1 Details for all States
        /// Details - No Of Districts, No Of Blocks, No Of Villages & Respective Population 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array Form1StateLevelListingDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords)
        {
            dbContext = new Models.PMGSYEntities();
            try
            {
                int StateCode = PMGSYSession.Current.StateCode > 0 ? PMGSYSession.Current.StateCode : 0;
                int DistrictCode = 0;
                int BlockCode = 0;
                int Level = 1;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;

                var itemList = dbContext.USP_FORM1_REPORT(Level, StateCode, DistrictCode, BlockCode, PMGSY).ToList<USP_FORM1_REPORT_Result>();

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
                    id = itemDetails.LOCATION_NAME.ToString().Trim(),
                    cell = new[] {       
                                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='viewForm1DistrictLevelReport(\"" +  itemDetails.LOCATION_CODE.ToString().Trim()  +  "\",\"" + itemDetails.LOCATION_NAME + "\"); return false;'>" + itemDetails.LOCATION_NAME + "</a>",
                                        //itemDetails.MAST_STATE_NAME,
                                        itemDetails.LOCATION_TYPE,
                                        itemDetails.TOTAL_DISTRICT.ToString(),
                                        itemDetails.TOTAL_BLOCK.ToString(),
                                        itemDetails.TOTAL_VILLAGE.ToString(),
                                        itemDetails.POP1000.ToString(),
                                        itemDetails.POP999.ToString(),
                                        itemDetails.POP499.ToString(),
                                        itemDetails.POP250.ToString()
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
        /// List the Form1 Details for all Districts Under Particular State
        /// Details - No Of Blocks, No Of Villages & Respective Population 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array Form1DistrictLevelListingDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode)
        {
            dbContext = new Models.PMGSYEntities();
            try
            {
                Int32 StateCode = stateCode == 0 ? PMGSYSession.Current.StateCode : stateCode;
                int DistrictCode = PMGSYSession.Current.DistrictCode > 0 ? PMGSYSession.Current.DistrictCode : 0;
                int BlockCode = 0;
                int Level = 2;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var itemList = dbContext.USP_FORM1_REPORT(Level, StateCode, DistrictCode, BlockCode, PMGSY).ToList<USP_FORM1_REPORT_Result>();


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


                //itemList = itemList.OrderBy(x => x.MAST_DISTRICT_NAME).ToList();
                return itemList.Select(itemDetails => new
                {
                    id = itemDetails.LOCATION_CODE.ToString().Trim(),
                    cell = new[] {       
                                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='viewForm1BlockLevelReport(\"" + StateCode.ToString().Trim() + "\",\"" + itemDetails.LOCATION_CODE.ToString().Trim() +  "\",\"" + itemDetails.LOCATION_NAME  + "\"); return false;'>" + itemDetails.LOCATION_NAME + "</a>",
                                        //itemDetails.MAST_DISTRICT_NAME,
                                        //itemDetails.MAST_NIC_DISTRICT_CODE.ToString(),
                                        itemDetails.LOCATION_TYPE,
                                        itemDetails.TOTAL_BLOCK.ToString(),
                                        itemDetails.TOTAL_VILLAGE.ToString(),
                                        itemDetails.POP1000.ToString(),
                                        itemDetails.POP999.ToString(),
                                        itemDetails.POP499.ToString(),
                                        itemDetails.POP250.ToString()
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
        /// List the Form1 Details for all Blocks Under Particular District
        /// Details - No Of Villages & Respective Population 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="stateCode"></param>
        /// <param name="districtCode"></param>
        /// <returns></returns>
        public Array Form1BlockLevelListingDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int districtCode)
        {
            dbContext = new Models.PMGSYEntities();
            try
            {
                Int32 StateCode = stateCode == 0 ? PMGSYSession.Current.StateCode : stateCode;
                Int32 DistrictCode = districtCode == 0 ? PMGSYSession.Current.DistrictCode : districtCode;
                Int32 BlockCode = 0;
                int Level = 3;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var itemList = dbContext.USP_FORM1_REPORT(Level, StateCode, DistrictCode, BlockCode, PMGSY).ToList<USP_FORM1_REPORT_Result>();

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


                //itemList = itemList.OrderBy(x => x.MAST_BLOCK_NAME).ToList();
                return itemList.Select(itemDetails => new
                {
                    id = itemDetails.LOCATION_CODE.ToString().Trim(),
                    cell = new[] {       
                                       "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='viewForm1VillageLevelReport(\"" +  StateCode.ToString().Trim() + "\",\"" + DistrictCode.ToString().Trim()  +  "\",\"" + itemDetails.LOCATION_CODE.ToString().Trim() +  "\",\"" + itemDetails.LOCATION_NAME  + "\"); return false;'>" + itemDetails.LOCATION_NAME + "</a>",
                                       // itemDetails.LOCATION_NAME,                                    
                                        itemDetails.LOCATION_TYPE, //Is Desert //Is Tribal
                                        itemDetails.TOTAL_VILLAGE.ToString(),
                                        itemDetails.POP1000.ToString(),
                                        itemDetails.POP999.ToString(),
                                        itemDetails.POP499.ToString(),
                                        itemDetails.POP250.ToString()
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
        /// List the Form1 Details for all Villages Under Particular Block
        /// Details - No Of Habitations & Respective Population 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="stateCode"></param>
        /// <param name="districtCode"></param>
        /// <param name="blockCode"></param>
        /// <returns></returns>
        public Array Form1VillageLevelListingDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int districtCode, int blockCode)
        {
            dbContext = new Models.PMGSYEntities();
            try
            {
                Int32 StateCode = stateCode == 0 ? PMGSYSession.Current.StateCode : stateCode;
                Int32 DistrictCode = districtCode == 0 ? PMGSYSession.Current.DistrictCode : districtCode;
                Int32 BlockCode = blockCode;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var itemList = dbContext.USP_FORM1_VILLAGE_REPORT(StateCode, DistrictCode, BlockCode, PMGSY).ToList<USP_FORM1_VILLAGE_REPORT_Result>();

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
                    id = itemDetails.LOCATION_CODE.ToString().Trim(),
                    cell = new[] {       
                                        itemDetails.LOCATION_NAME,                                      
                                        itemDetails.MAST_HAB_NAME,
                                        itemDetails.IS_SCHEDULE5,
                                        itemDetails.MAST_HAB_TOT_POP.ToString(),
                                        itemDetails.HAB_CONNECTED
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

        #endregion



        #region Form2


        /// <summary>
        /// List the Form2 Details for all States
        /// Statewise Total MLA/MP Constituency
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array Form2StateLevelListingDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, string constType, int constCode)
        {
            dbContext = new Models.PMGSYEntities();
            try
            {

                int StateCode = PMGSYSession.Current.StateCode > 0 ? PMGSYSession.Current.StateCode : 0;
                int DistrictCode = 0;
                int ConstCode = constCode;
                string ConstType = constType;
                int Level = 1;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;

                var itemList = dbContext.USP_FORM2_REPORT(Level, StateCode, DistrictCode, ConstCode, ConstType).ToList<USP_FORM2_REPORT_Result>();
                //3rd Parameter as districtCode applicable only for Constituency Level i.e. in PIU Login
                //itemList = dbContext.USP_FORM2_REPORT(1, StateCode, 0, 0, "").ToList<USP_FORM2_REPORT_Result>();         
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
                    id = itemDetails.MAST_STATE_CODE.ToString().Trim(),
                    cell = new[] {       
                                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='loadForm2DistrictLevelReportGrid(\"" +  itemDetails.MAST_STATE_CODE.ToString().Trim() +  "\",\"" + itemDetails.MAST_STATE_NAME + "\",\"" + ConstType.ToString().Trim() + "\",\"" + ConstCode.ToString().Trim() + "\"); return false;'>" + itemDetails.MAST_STATE_NAME + "</a>",
                                        itemDetails.TOTAL_MP_CONST.ToString(),
                                        itemDetails.TOTAL_MLA_CONST.ToString()
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
        /// Form2 District Level Listing
        /// Lists Constituency wise Details of Total Districts & Total Blocks under particular State
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="stateCode"></param>
        /// <param name="constType"></param>
        /// <returns></returns>
        public Array Form2DistrictLevelListingDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, string constType, int constCode)
        {
            dbContext = new Models.PMGSYEntities();
            try
            {
                List<USP_FORM2_DISTRICT_REPORT_Results> itemList = new List<USP_FORM2_DISTRICT_REPORT_Results>();
                Int32 StateCode = stateCode == 0 ? PMGSYSession.Current.StateCode : stateCode;
                int DistrictCode = 0;
                int ConstCode = constCode;
                string ConstType = constType;
                //3rd Parameter as districtCode applicable only for Constituency Level i.e. in PIU Login
                //itemList = dbContext.USP_FORM2_DISTRICT_REPORT(2, StateCode, 0, 0, constType).ToList<USP_FORM2_DISTRICT_REPORT_Result>();
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;

                itemList = dbContext.Database.SqlQuery<USP_FORM2_DISTRICT_REPORT_Results>("EXEC omms.USP_FORM2_REPORT @Level,@State,@District,@ConstCode,@ConstType",
                    new SqlParameter("Level", 2),
                    new SqlParameter("State", StateCode),
                    new SqlParameter("District", DistrictCode),
                    new SqlParameter("ConstCode", ConstCode),
                    new SqlParameter("ConstType", ConstType)
                    ).ToList<USP_FORM2_DISTRICT_REPORT_Results>();

                totalRecords = itemList.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        itemList = itemList.OrderBy(x => x.CONST_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        itemList = itemList.OrderByDescending(x => x.CONST_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    itemList = itemList.OrderBy(x => x.CONST_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return itemList.Select(itemDetails => new
                {
                    id = itemDetails.CONST_CODE.ToString().Trim(),
                    cell = new[] {       
                                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='loadForm2ConstituencyLevelReportGrid(\""+  StateCode + "\",\"" +  itemDetails.CONST_CODE.ToString().Trim()  +  "\",\"" + itemDetails.CONST_NAME+ "\",\"" + ConstType.ToString().Trim()+ "\"); return false;'>" + itemDetails.CONST_NAME + "</a>",
                                        itemDetails.TOTAL_DISTRICT.ToString(),
                                        itemDetails.TOTAL_BLOCK.ToString()
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
        /// Lists details for particular Constituency
        /// Districts & Blocks under particular Constituency
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="stateCode"></param>
        /// <param name="constCode"></param>
        /// <param name="constType"></param>
        /// <returns></returns>
        public Array Form2ConstituencyLevelListingDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int constCode, string constType)
        {
            dbContext = new Models.PMGSYEntities();
            try
            {
                List<USP_FORM2_CONSTITUENCY_REPORT_Results> itemList = new List<USP_FORM2_CONSTITUENCY_REPORT_Results>();

                Int32 StateCode = stateCode == 0 ? PMGSYSession.Current.StateCode : stateCode;
                int DistrictCode = PMGSYSession.Current.DistrictCode;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;

                //3rd Parameter as districtCode applicable only for Constituency Level i.e. in PIU Login
                //itemList = dbContext.USP_FORM2_CONSTITUENCY_REPORT(3, StateCode, DistrictCode, constCode, constType).ToList<USP_FORM2_CONSTITUENCY_REPORT_Result>();

                itemList = dbContext.Database.SqlQuery<USP_FORM2_CONSTITUENCY_REPORT_Results>("EXEC omms.USP_FORM2_REPORT @Level,@State,@District,@ConstCode,@ConstType",
                    new SqlParameter("Level", 3),
                    new SqlParameter("State", StateCode),
                    new SqlParameter("District", DistrictCode),
                    new SqlParameter("ConstCode", constCode),
                    new SqlParameter("ConstType", constType)
                    ).ToList<USP_FORM2_CONSTITUENCY_REPORT_Results>();

                totalRecords = itemList.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        itemList = itemList.OrderBy(x => x.CONST_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        itemList = itemList.OrderByDescending(x => x.CONST_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    itemList = itemList.OrderBy(x => x.CONST_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return itemList.Select(itemDetails => new
                {
                    id = itemDetails.CONST_CODE.ToString().Trim(),
                    cell = new[] {       
                                        //"<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='viewForm2ConstituencyLevelReport(\"" +  itemDetails.CONST_CODE.ToString().Trim() + "\"); return false;'>" + itemDetails.CONST_NAME + "</a>",
                                        itemDetails.CONST_NAME,
                                        itemDetails.MAST_DISTRICT_NAME.ToString(),
                                        itemDetails.MAST_BLOCK_NAME.ToString()
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


        #endregion



        #region Form3

        /// <summary>
        /// List the Form3 Details for all States
        /// Statewise Total / Unconnected / Benefitted / Balance population
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array Form3StateLevelListingDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords)
        {
            dbContext = new Models.PMGSYEntities();

            try
            {
                int StateCode = PMGSYSession.Current.StateCode > 0 ? PMGSYSession.Current.StateCode : 0;
                int DistrictCode = 0;
                int BlockCode = 0;
                int Level = 1;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 1400;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var itemList = dbContext.USP_FORM3_REPORT(Level, StateCode, DistrictCode, BlockCode, PMGSYSession.Current.PMGSYScheme).ToList<USP_FORM3_REPORT_Result>();

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


                //itemList = itemList.OrderBy(x => x.MAST_STATE_NAME).ToList();
                return itemList.Select(itemDetails => new
                {
                    id = itemDetails.LOCATION_CODE.ToString().Trim(),
                    cell = new[] {       
                                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='viewForm3DistrictLevelReport(\"" +  itemDetails.LOCATION_CODE.ToString().Trim() +  "\",\"" + itemDetails.LOCATION_NAME + "\"); return false;'>" + itemDetails.LOCATION_NAME + "</a>",
                                        //itemDetails.MAST_STATE_NAME,
                                        itemDetails.TPOP1000.ToString(),
                                        itemDetails.TPOP999.ToString(),
                                        itemDetails.TPOP499.ToString(),
                                        itemDetails.TPOP250.ToString(),
                                        itemDetails.UPOP1000.ToString(),
                                        itemDetails.UPOP999.ToString(),
                                        itemDetails.UPOP499.ToString(),
                                        itemDetails.UPOP250.ToString(),
                                        itemDetails.BPOP1000.ToString(),
                                        itemDetails.BPOP999.ToString(),
                                        itemDetails.BPOP499.ToString(),
                                        itemDetails.BPOP250.ToString(),
                                        itemDetails.POP1000.ToString(),
                                        itemDetails.POP999.ToString(),
                                        itemDetails.POP499.ToString(),
                                        itemDetails.POP250.ToString()
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
        /// List the Form3 Details for all Districts Under Particular State
        /// Districtwise Total / Unconnected / Benefitted / Balance population
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array Form3DistrictLevelListingDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode)
        {
            dbContext = new Models.PMGSYEntities();
            try
            {
                // List<USP_FORM3_REPORT_Result> itemList = new List<USP_FORM3_REPORT_Result>();
                Int32 StateCode = stateCode == 0 ? PMGSYSession.Current.StateCode : stateCode;
                int DistrictCode = PMGSYSession.Current.DistrictCode > 0 ? PMGSYSession.Current.DistrictCode : 0;
                int BlockCode = 0;
                int Level = 2;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 1400;
                dbContext.Configuration.LazyLoadingEnabled = false;

                var itemList = dbContext.USP_FORM3_REPORT(Level, StateCode, DistrictCode, BlockCode, PMGSYSession.Current.PMGSYScheme).ToList<USP_FORM3_REPORT_Result>();


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
                    id = itemDetails.LOCATION_CODE.ToString().Trim(),
                    cell = new[] {       
                                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='viewForm3BlockLevelReport(\"" +  StateCode.ToString().Trim() + "\",\"" + itemDetails.LOCATION_CODE.ToString().Trim()  +  "\",\"" + itemDetails.LOCATION_NAME + "\"); return false;'>" + itemDetails.LOCATION_NAME + "</a>",
                                        //itemDetails.MAST_DISTRICT_NAME,
                                        itemDetails.TPOP1000.ToString(),
                                        itemDetails.TPOP999.ToString(),
                                        itemDetails.TPOP499.ToString(),
                                        itemDetails.TPOP250.ToString(),
                                        itemDetails.UPOP1000.ToString(),
                                        itemDetails.UPOP999.ToString(),
                                        itemDetails.UPOP499.ToString(),
                                        itemDetails.UPOP250.ToString(),
                                        itemDetails.BPOP1000.ToString(),
                                        itemDetails.BPOP999.ToString(),
                                        itemDetails.BPOP499.ToString(),
                                        itemDetails.BPOP250.ToString(),
                                        itemDetails.POP1000.ToString(),
                                        itemDetails.POP999.ToString(),
                                        itemDetails.POP499.ToString(),
                                        itemDetails.POP250.ToString()
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
        /// List the Form3 Details for all Blocks Under Particular District
        /// Blockwise Total / Unconnected / Benefitted / Balance population
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="stateCode"></param>
        /// <param name="districtCode"></param>
        /// <returns></returns>
        public Array Form3BlockLevelListingDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int districtCode)
        {
            dbContext = new Models.PMGSYEntities();
            try
            {
                Int32 StateCode = stateCode == 0 ? PMGSYSession.Current.StateCode : stateCode;
                Int32 DistrictCode = districtCode == 0 ? PMGSYSession.Current.DistrictCode : districtCode;
                Int32 BlockCode = 0;
                int Level = 3;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 1400;
                dbContext.Configuration.LazyLoadingEnabled = false;

                var itemList = dbContext.USP_FORM3_REPORT(Level, StateCode, DistrictCode, BlockCode, PMGSYSession.Current.PMGSYScheme).ToList<USP_FORM3_REPORT_Result>();

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
                    id = itemDetails.LOCATION_CODE.ToString().Trim(),
                    cell = new[] {       
                                        itemDetails.LOCATION_NAME,
                                        itemDetails.TPOP1000.ToString(),
                                        itemDetails.TPOP999.ToString(),
                                        itemDetails.TPOP499.ToString(),
                                        itemDetails.TPOP250.ToString(),
                                        itemDetails.UPOP1000.ToString(),
                                        itemDetails.UPOP999.ToString(),
                                        itemDetails.UPOP499.ToString(),
                                        itemDetails.UPOP250.ToString(),
                                        itemDetails.BPOP1000.ToString(),
                                        itemDetails.BPOP999.ToString(),
                                        itemDetails.BPOP499.ToString(),
                                        itemDetails.BPOP250.ToString(),
                                        itemDetails.POP1000.ToString(),
                                        itemDetails.POP999.ToString(),
                                        itemDetails.POP499.ToString(),
                                        itemDetails.POP250.ToString()
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


        #endregion



        #region Form4

        /// <summary>
        /// List the Form4 Details for all States
        /// Statewise Proposal Details
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array Form4StateLevelListingDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int districtCode, int blockCode, int year, int batch, int collaboration, string propType)
        {
            dbContext = new Models.PMGSYEntities();

            try
            {
                int StateCode = PMGSYSession.Current.StateCode > 0 ? PMGSYSession.Current.StateCode : 0;
                int DistrictCode = districtCode;
                int BlockCode = blockCode;
                int Year = year;
                int Batch = batch;
                int Collaboration = collaboration;
                string PropType = propType;
                int PMGSY = PMGSYSession.Current.PMGSYScheme > 0 ? PMGSYSession.Current.PMGSYScheme : 0;
                int Level = 1;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 1440;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var itemList = dbContext.USP_FORM4_REPORT(Level, StateCode, DistrictCode, BlockCode, Year, Batch, Collaboration, PropType, PMGSY).ToList<USP_FORM4_REPORT_Result>();

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
                    id = itemDetails.LOCATION_CODE.ToString().Trim(),
                    cell = new[] {       
                                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='loadForm4DistrictLevelReportGrid(\"" +  itemDetails.LOCATION_CODE.ToString().Trim() +  "\",\"" + itemDetails.LOCATION_NAME + "\",\"" + PropType.ToString().Trim() + "\",\"" + Year.ToString().Trim() + "\",\"" + Batch.ToString().Trim() + "\",\"" + Collaboration.ToString().Trim() + "\"); return false;'>" + itemDetails.LOCATION_NAME + "</a>",
                                        //itemDetails.MAST_STATE_NAME,
                                        itemDetails.TOTAL_PROPOSAL.ToString(),
                                        itemDetails.ROAD_LENGTH.ToString(),
                                        itemDetails.BRIDGE_LENGTH.ToString(),
                                        itemDetails.ROAD_AMOUNT.ToString(),
                                        itemDetails.BRIDGE_AMOUNT.ToString(),
                                        itemDetails.STATE_RD_AMOUNT.ToString(),
                                        itemDetails.STATE_BR_AMOUNT.ToString(),
                                        itemDetails.MAINT_AMOUNT.ToString(),
                                        itemDetails.POP1000.ToString(),
                                        itemDetails.POP999.ToString(),
                                        itemDetails.POP499.ToString(),
                                        itemDetails.POP250.ToString()
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
        /// List the Form3 Details for all Districts Under Particular State
        /// Districtwise Proposal Details
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array Form4DistrictLevelListingDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int districtCode, int blockCode, int year, int batch, int collaboration, string propType)
        {
            dbContext = new Models.PMGSYEntities();
            try
            {
                Int32 StateCode = stateCode == 0 ? PMGSYSession.Current.StateCode : stateCode;
                int DistrictCode = PMGSYSession.Current.DistrictCode > 0 ? PMGSYSession.Current.DistrictCode : 0;
                int BlockCode = 0;
                int Year = year;
                int Batch = batch;
                int Collaboration = collaboration;
                string PropType = propType;
                int PMGSY = PMGSYSession.Current.PMGSYScheme > 0 ? PMGSYSession.Current.PMGSYScheme : 0;
                int Level = 2;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 1440;
                dbContext.Configuration.LazyLoadingEnabled = false;

                var itemList = dbContext.USP_FORM4_REPORT(Level, StateCode, DistrictCode, BlockCode, Year, Batch, Collaboration, PropType, PMGSY).ToList<USP_FORM4_REPORT_Result>();

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
                    id = itemDetails.LOCATION_CODE.ToString().Trim(),
                    cell = new[] {       
                                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='loadForm4BlockLevelReportGrid(\"" +  StateCode.ToString().Trim() + "\",\"" + itemDetails.LOCATION_CODE.ToString().Trim()  +  "\",\"" + itemDetails.LOCATION_NAME + "\",\"" + PropType.ToString().Trim() + "\",\"" + Year.ToString().Trim() + "\",\"" + Batch.ToString().Trim() + "\",\"" + Collaboration.ToString().Trim() + "\"); return false;'>" + itemDetails.LOCATION_NAME + "</a>",
                                        //itemDetails.MAST_DISTRICT_NAME,
                                        itemDetails.TOTAL_PROPOSAL.ToString(),
                                        itemDetails.ROAD_LENGTH.ToString(),
                                        itemDetails.BRIDGE_LENGTH.ToString(),
                                        itemDetails.ROAD_AMOUNT.ToString(),
                                        itemDetails.BRIDGE_AMOUNT.ToString(),
                                        itemDetails.STATE_RD_AMOUNT.ToString(),
                                        itemDetails.STATE_BR_AMOUNT.ToString(),
                                        itemDetails.MAINT_AMOUNT.ToString(),
                                        itemDetails.POP1000.ToString(),
                                        itemDetails.POP999.ToString(),
                                        itemDetails.POP499.ToString(),
                                        itemDetails.POP250.ToString()
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
        /// List the Form3 Details for all Blocks Under Particular District
        /// Blockwise Proposal Details 
        /// No Change
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="stateCode"></param>
        /// <param name="districtCode"></param>
        /// <returns></returns>
        public Array Form4BlockLevelListingDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int districtCode, int blockCode, int year, int batch, int collaboration, string propType)
        {
            dbContext = new Models.PMGSYEntities();
            try
            {
                int PMGSY = PMGSYSession.Current.PMGSYScheme > 0 ? PMGSYSession.Current.PMGSYScheme : 0;
                Int32 StateCode = stateCode == 0 ? PMGSYSession.Current.StateCode : stateCode;
                Int32 DistrictCode = districtCode == 0 ? PMGSYSession.Current.DistrictCode : districtCode;
                Int32 BlockCode = 0;
                int Year = year;
                int Batch = batch;
                int Collaboration = collaboration;
                string PropType = propType;
                int Level = 3;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 1440;
                dbContext.Configuration.LazyLoadingEnabled = false;

                var itemList = dbContext.USP_FORM4_REPORT(Level, StateCode, DistrictCode, BlockCode, Year, Batch, Collaboration, PropType, PMGSY).ToList<USP_FORM4_REPORT_Result>();

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
                    id = itemDetails.LOCATION_CODE.ToString().Trim(),
                    cell = new[] {       
                                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='loadForm4FinalLevelReportGrid(\"" +  StateCode.ToString().Trim() + "\",\"" + DistrictCode.ToString().Trim()  + "\",\"" + itemDetails.LOCATION_CODE.ToString().Trim()+ "\",\"" + itemDetails.LOCATION_NAME.ToString().Trim()  + "\",\"" + PropType.ToString().Trim()  + "\",\"" + Year.ToString().Trim() + "\",\"" + Batch.ToString().Trim() + "\",\"" + Collaboration.ToString().Trim() + "\"); return false;'>" + itemDetails.LOCATION_NAME + "</a>",
                                        //itemDetails.LOCATION_NAME,
                                        itemDetails.TOTAL_PROPOSAL.ToString(),
                                        itemDetails.ROAD_LENGTH.ToString(),
                                        itemDetails.BRIDGE_LENGTH.ToString(),
                                        itemDetails.ROAD_AMOUNT.ToString(),
                                        itemDetails.BRIDGE_AMOUNT.ToString(),
                                        itemDetails.STATE_RD_AMOUNT.ToString(),
                                        itemDetails.STATE_BR_AMOUNT.ToString(),
                                        itemDetails.MAINT_AMOUNT.ToString(),
                                        itemDetails.POP1000.ToString(),
                                        itemDetails.POP999.ToString(),
                                        itemDetails.POP499.ToString(),
                                        itemDetails.POP250.ToString()
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
        /// List the Form3 Details for all Blocks Under Particular District
        /// Blockwise Proposal Details 
        /// No Change
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="stateCode"></param>
        /// <param name="districtCode"></param>
        /// <returns></returns>
        public Array Form4FinalLevelListingDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int districtCode, int blockCode, int year, int batch, int collaboration, string propType)
        {
            dbContext = new Models.PMGSYEntities();
            try
            {
                int PMGSY = PMGSYSession.Current.PMGSYScheme > 0 ? PMGSYSession.Current.PMGSYScheme : 0;
                Int32 StateCode = stateCode == 0 ? PMGSYSession.Current.StateCode : stateCode;
                Int32 DistrictCode = districtCode == 0 ? PMGSYSession.Current.DistrictCode : districtCode;
                Int32 BlockCode = blockCode;
                int Year = year;
                int Batch = batch;
                int Collaboration = collaboration;
                string PropType = propType;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 1440;
                dbContext.Configuration.LazyLoadingEnabled = false;

                var itemList = dbContext.USP_FORM4_PROP_REPORT(StateCode, DistrictCode, BlockCode, Year, Batch, Collaboration, PropType, PMGSY).ToList<USP_FORM4_PROP_REPORT_Result>();

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
                    id = itemDetails.LOCATION_CODE.ToString().Trim(),
                    cell = new[] {       
                                        itemDetails.LOCATION_NAME,
                                        itemDetails.IMS_PACKAGE_ID.ToString(),
                                        itemDetails.IMS_ROAD_NAME.ToString(),
                                        itemDetails.PLAN_CN_ROAD_CODE.ToString(),                                       
                                        itemDetails.BRIDGE_NAME.ToString(),
                                       itemDetails.CARRIAGE_WIDTH.ToString(),
                                        itemDetails.IS_STAGED.ToString(),
                                        itemDetails.IMS_NO_OF_CDWORKS.ToString(),
                                        itemDetails.UPGRADE_CONNECT.ToString(),                                     
                                        itemDetails.ROAD_LENGTH.ToString(),
                                        itemDetails.BRIDGE_LENGTH.ToString(),
                                        itemDetails.ROAD_AMOUNT.ToString(),
                                        itemDetails.BRIDGE_AMOUNT.ToString(),
                                        itemDetails.STATE_RD_AMOUNT.ToString(),
                                        itemDetails.STATE_BR_AMOUNT.ToString(),
                                        itemDetails.MAINT_AMOUNT.ToString(),                                 
                                        itemDetails.POP1000.ToString(),
                                        itemDetails.POP999.ToString(),
                                        itemDetails.POP499.ToString(),
                                        itemDetails.POP250.ToString()
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


        #endregion



        #region Form5/6

        //Form 5 & 6 are in same report i.e. in Form5

        /// <summary>
        /// List the Form4 Details for all States
        /// Statewise MLA / MP Corenetworks & Proposed Road Counts
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array Form5StateLevelListingDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int year, string constType, int constCode)
        {
            dbContext = new Models.PMGSYEntities();

            try
            {
                // List<USP_FORM5_REPORT_Results> itemList = new List<USP_FORM5_REPORT_Results>();
                int StateCode = PMGSYSession.Current.StateCode > 0 ? PMGSYSession.Current.StateCode : 0;
                int ConstCode = constCode;
                string ConstType = constType;
                int Year = year;
                var itemList = dbContext.USP_FORM5_REPORT(1, Year, StateCode, ConstCode, ConstType).ToList<USP_FORM5_REPORT_Result>();

                //itemList = dbContext.Database.SqlQuery<USP_FORM5_REPORT_Results>("EXEC omms.USP_FORM5_REPORT @Level,@Year,@State,@ConstCode,@ConstType",
                //    new SqlParameter("Level", 1),
                //    new SqlParameter("Year", year),
                //    new SqlParameter("State", StateCode),
                //    new SqlParameter("ConstCode", ConstCode),
                //    new SqlParameter("ConstType", ConstType)
                //    ).ToList<USP_FORM5_REPORT_Results>();

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
                   // id = itemDetails.MAST_STATE_CODE == null ? 0 : itemDetails.MAST_STATE_CODE.ToString().Trim(),
                    cell = new[] {       
                                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='loadForm5DistrictLevelReportGrid(\"" +  itemDetails.MAST_STATE_CODE.ToString().Trim() +  "\",\"" + itemDetails.MAST_STATE_NAME + "\",\"" +Year.ToString().Trim() + "\",\"" + ConstType.ToString().Trim() + "\",\"" + ConstCode.ToString().Trim() + "\"); return false;'>" + itemDetails.MAST_STATE_NAME + "</a>",
                                        //itemDetails.MAST_STATE_NAME,
                                         itemDetails.MLA==null?"":  itemDetails.MLA.ToString(),
                                         itemDetails.MP==null?"": itemDetails.MP.ToString(),
                                         itemDetails.MLA_CN==null?"":itemDetails.MLA_CN.ToString(),
                                         itemDetails.MP_CN==null?"":itemDetails.MP_CN.ToString(),
                                         itemDetails.MLA_PR==null?"":itemDetails.MLA_PR.ToString(),
                                         itemDetails.MP_PR==null?"":itemDetails.MP_PR.ToString()
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
        /// List the Form5 Details for all Districts Under Particular State
        /// Districtwise MLA / MP Corenetworks & Proposed Road Counts
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array Form5DistrictLevelListingDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int year, int stateCode, string constType, int constCode)
        {
            dbContext = new Models.PMGSYEntities();
            try
            {
                List<USP_FORM5_DISTRICT_REPORT_Results> itemList = new List<USP_FORM5_DISTRICT_REPORT_Results>();
                Int32 StateCode = stateCode == 0 ? PMGSYSession.Current.StateCode : stateCode;
                int ConstCode = constCode;
                int Year = year;
                string ConstType = constType;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;

                //itemList = dbContext.USP_FORM5_DISTRICT_REPORT(2, year, StateCode, 0, constType).ToList<USP_FORM5_DISTRICT_REPORT_Result>();

                itemList = dbContext.Database.SqlQuery<USP_FORM5_DISTRICT_REPORT_Results>("EXEC omms.USP_FORM5_REPORT @Level,@Year,@State,@ConstCode,@ConstType",
                    new SqlParameter("Level", 2),
                    new SqlParameter("Year", Year),
                    new SqlParameter("State", StateCode),
                    new SqlParameter("ConstCode", ConstCode),
                    new SqlParameter("ConstType", ConstType)
                    ).ToList<USP_FORM5_DISTRICT_REPORT_Results>();

                totalRecords = itemList.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        itemList = itemList.OrderBy(x => x.CONST_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        itemList = itemList.OrderByDescending(x => x.CONST_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    itemList = itemList.OrderBy(x => x.CONST_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }


                return itemList.Select(itemDetails => new
                {
                    id = itemDetails.CONST_CODE.ToString().Trim(),
                    cell = new[] {       
                                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='loadForm5ConstituencyLevelReportGrid(\"" +  Year.ToString().Trim() + "\",\"" +  StateCode.ToString().Trim() + "\",\"" + itemDetails.CONST_CODE.ToString().Trim()  +  "\",\"" + itemDetails.CONST_NAME + "\",\"" + ConstType.ToString().Trim() + "\"); return false;'>" + itemDetails.CONST_NAME + "</a>",
                                        //itemDetails.CONST_NAME,
                                        itemDetails.PROP.ToString(),
                                        itemDetails.CN.ToString(),
                                        itemDetails.PR.ToString(),
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
        /// List the Form5 Details for all Blocks Under Particular District
        /// Blockwise MLA / MP Corenetworks & Proposed Road Counts
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="stateCode"></param>
        /// <param name="districtCode"></param>
        /// <returns></returns>
        public Array Form5ConstituencyLevelListingDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int year, int stateCode, int constCode, string constType)
        {
            dbContext = new Models.PMGSYEntities();
            try
            {
                List<USP_FORM5_CONSTITUENCY_REPORT_Results> itemList = new List<USP_FORM5_CONSTITUENCY_REPORT_Results>();
                Int32 StateCode = stateCode == 0 ? PMGSYSession.Current.StateCode : stateCode;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;

                //itemList = dbContext.USP_FORM5_CONSTITUENCY_REPORT(3, year, StateCode, constCode, constType).ToList<USP_FORM5_CONSTITUENCY_REPORT_Result>();

                itemList = dbContext.Database.SqlQuery<USP_FORM5_CONSTITUENCY_REPORT_Results>("EXEC omms.USP_FORM5_REPORT @Level,@Year,@State,@ConstCode,@ConstType",
                    new SqlParameter("Level", 3),
                    new SqlParameter("Year", year),
                    new SqlParameter("State", StateCode),
                    new SqlParameter("ConstCode", constCode),
                    new SqlParameter("ConstType", constType)
                    ).ToList<USP_FORM5_CONSTITUENCY_REPORT_Results>();

                totalRecords = itemList.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        itemList = itemList.OrderBy(x => x.CONST_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        itemList = itemList.OrderByDescending(x => x.CONST_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    itemList = itemList.OrderBy(x => x.CONST_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }


                return itemList.Select(itemDetails => new
                {
                    id = itemDetails.CONST_CODE.ToString().Trim(),
                    cell = new[] {       
                                        itemDetails.CONST_NAME,
                                        itemDetails.IMS_ROAD_DETAILS,
                                        itemDetails.INCLUDED_IN_CN,
                                        itemDetails.IMS_REASON_ID_1,
                                        itemDetails.PLAN_CN_ROAD_CODE,
                                        itemDetails.INCLUDED_IN_PR,
                                        itemDetails.IMS_REASON_ID_2,
                                        itemDetails.IMS_PR_ROAD_CODE
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


        #endregion



        #region Form7

        /// <summary>
        /// Listing of Statewise Proposals, Cost, Awards & Awarded amount
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array Form7StateLevelListingDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, string propType, int batch, int year, int collaboration)
        {
            dbContext = new Models.PMGSYEntities();

            try
            {

                int StateCode = PMGSYSession.Current.StateCode > 0 ? PMGSYSession.Current.StateCode : 0;
                int DistrictCode = 0;
                int BlockCode = 0;
                string PropType = propType;
                int Batch = batch;
                int Year = year;
                int Collaboration = collaboration;
                int Level = 1;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                var itemList = dbContext.USP_FORM7_REPORT(Level, StateCode, DistrictCode, BlockCode, PropType, Batch, Year, Collaboration, PMGSY).ToList<USP_FORM7_REPORT_Result>();


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
                    id = itemDetails.LOCATION_CODE.ToString().Trim(),
                    cell = new[] {       
                                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='loadForm7DistrictLevelReportGrid(\"" +  itemDetails.LOCATION_CODE.ToString().Trim() +  "\",\"" + itemDetails.LOCATION_NAME + "\",\"" +PropType + "\",\"" + Year + "\",\"" + Batch + "\",\"" + Collaboration + "\"); return false;'>" + itemDetails.LOCATION_NAME + "</a>",
                                        itemDetails.ROAD_PROPOSAL.ToString(),
                                        itemDetails.ROAD_COST.ToString(),
                                        itemDetails.BRIDGE_PROPOSAL.ToString(),
                                        itemDetails.BRIDGE_COST.ToString(),
                                        itemDetails.ROAD_AWARD.ToString(),
                                        itemDetails.BRIDGE_AWARD.ToString(),
                                        itemDetails.ROAD_AWARDED_AMOUNT.ToString(),
                                        itemDetails.BRIDGE_AWARDED_AMOUNT.ToString()
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
        /// Listing of Districtwise Proposals, Cost, Awards & Awarded amount
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array Form7DistrictLevelListingDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, string propType, int batch, int year, int collaboration)
        {
            dbContext = new Models.PMGSYEntities();
            try
            {
                Int32 StateCode = stateCode == 0 ? PMGSYSession.Current.StateCode : stateCode;
                int DistrictCode = 0;
                int BlockCode = 0;
                string PropType = propType;
                int Batch = batch;
                int Year = year;
                int Collaboration = collaboration;
                int Level = 2;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                var itemList = dbContext.USP_FORM7_REPORT(Level, StateCode, DistrictCode, BlockCode, PropType, Batch, Year, Collaboration, PMGSY).ToList<USP_FORM7_REPORT_Result>();

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
                    id = itemDetails.LOCATION_CODE.ToString().Trim(),
                    cell = new[] {       
                                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='loadForm7BlockLevelReportGrid(\"" +  StateCode.ToString().Trim() + "\",\"" + itemDetails.LOCATION_CODE.ToString().Trim()  +  "\",\"" + itemDetails.LOCATION_NAME +  "\",\"" + PropType +  "\",\"" + Year +  "\",\"" + Batch +  "\",\"" + Collaboration +  "\"); return false;'>" + itemDetails.LOCATION_NAME + "</a>",
                                        itemDetails.ROAD_PROPOSAL.ToString(),
                                        itemDetails.ROAD_COST.ToString(),
                                        itemDetails.BRIDGE_PROPOSAL.ToString(),
                                        itemDetails.BRIDGE_COST.ToString(),
                                        itemDetails.ROAD_AWARD.ToString(),
                                        itemDetails.BRIDGE_AWARD.ToString(),
                                        itemDetails.ROAD_AWARDED_AMOUNT.ToString(),
                                        itemDetails.BRIDGE_AWARDED_AMOUNT.ToString()
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
        /// List the Form7 Details for all Blocks Under Particular District
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="stateCode"></param>
        /// <param name="districtCode"></param>
        /// <returns></returns>
        public Array Form7BlockLevelListingDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int districtCode, string propType, int batch, int year, int collaboration)
        {
            dbContext = new Models.PMGSYEntities();
            try
            {
                Int32 StateCode = stateCode == 0 ? PMGSYSession.Current.StateCode : stateCode;
                Int32 DistrictCode = districtCode == 0 ? PMGSYSession.Current.DistrictCode : districtCode;
                int BlockCode = 0;
                string PropType = propType;
                int Batch = batch;
                int Year = year;
                int Collaboration = collaboration;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var itemList = dbContext.USP_FORM7_REPORT(3, StateCode, DistrictCode, BlockCode, propType, batch, year, Collaboration, PMGSY).ToList<USP_FORM7_REPORT_Result>();

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
                    id = itemDetails.LOCATION_CODE.ToString().Trim(),
                    cell = new[] {       
                                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='loadForm7FinalLevelReportGrid(\"" +  StateCode.ToString().Trim() + "\",\"" +  DistrictCode.ToString().Trim() + "\",\"" + itemDetails.LOCATION_CODE.ToString().Trim()  +  "\",\"" + itemDetails.LOCATION_NAME +  "\",\"" + PropType +  "\",\"" + Year +  "\",\"" + Batch +  "\",\"" + Collaboration +  "\"); return false;'>" + itemDetails.LOCATION_NAME + "</a>",
                                        itemDetails.ROAD_PROPOSAL.ToString(),
                                        itemDetails.ROAD_COST.ToString(),
                                        itemDetails.BRIDGE_PROPOSAL.ToString(),
                                        itemDetails.BRIDGE_COST.ToString(),
                                        itemDetails.ROAD_AWARD.ToString(),
                                        itemDetails.BRIDGE_AWARD.ToString(),
                                        itemDetails.ROAD_AWARDED_AMOUNT.ToString(),
                                        itemDetails.BRIDGE_AWARDED_AMOUNT.ToString()                       
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
        /// List the Form7 Details for all Blocks Under Particular District
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="stateCode"></param>
        /// <param name="districtCode"></param>
        /// <returns></returns>
        public Array Form7FinalLevelListingDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int districtCode, int blockCode, string propType, int batch, int year, int collaboration)
        {
            dbContext = new Models.PMGSYEntities();
            try
            {
                Int32 StateCode = stateCode == 0 ? PMGSYSession.Current.StateCode : stateCode;
                Int32 DistrictCode = districtCode == 0 ? PMGSYSession.Current.DistrictCode : districtCode;
                int BlockCode = blockCode;
                string PropType = propType;
                int Batch = batch;
                int Year = year;
                int Collaboration = collaboration;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var itemList = dbContext.USP_FORM7_PROP_REPORT(StateCode, DistrictCode, BlockCode, propType, batch, year, Collaboration, PMGSY).ToList<USP_FORM7_PROP_REPORT_Result>();


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
                    id = itemDetails.LOCATION_CODE.ToString().Trim(),
                    cell = new[] {       
                                        itemDetails.LOCATION_NAME,
                                        itemDetails.IMS_ROAD_NAME,
                                        itemDetails.IMS_YEAR.ToString() + "-" + (itemDetails.IMS_YEAR + 1).ToString(),
                                        itemDetails.IMS_SANCTIONED_DATE,
                                        itemDetails.BRIDGE_NAME,
                                        itemDetails.ROAD_LENGTH.ToString(),
                                        itemDetails.BRIDGE_LENGTH.ToString(),
                                        itemDetails.ROAD_AMOUNT.ToString(),
                                        itemDetails.BRIDGE_AMOUNT.ToString(),
                                        itemDetails.STATE_RD_AMOUNT.ToString(),
                                        itemDetails.STATE_BR_AMOUNT.ToString(),
                                        itemDetails.MAINT_AMOUNT.ToString(),
                                        itemDetails.AGREEMENT_AMOUNT.ToString(),
                                        itemDetails.AGREEMENT_MAINT_AMOUNT.ToString(),
                                        itemDetails.CONTRACTOR_NAME.ToString(),
                                        itemDetails.TEND_AGREEMENT_NUMBER.ToString(),
                                        itemDetails.TEND_DATE_OF_AGREEMENT,
                                        itemDetails.TEND_AGREEMENT_START_DATE,
                                        itemDetails.TEND_AGREEMENT_END_DATE,
                                        itemDetails.TEND_DATE_OF_COMMENCEMENT,
                                        itemDetails.TEND_DATE_OF_COMPLETION,
                                        itemDetails.TEND_DATE_OF_AWARD_WORK,
                                        itemDetails.TEND_DATE_OF_WORK_ORDER
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

        #endregion



        #region Form8

        /// <summary>
        /// Listing of Statewise Proposals, Cost, Awards & Awarded amount
        /// //Exist as it is
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array Form8StateLevelListingDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, string propType, int batch, int year, int collaboration)
        {
            dbContext = new Models.PMGSYEntities();

            try
            {
                int StateCode = PMGSYSession.Current.StateCode > 0 ? PMGSYSession.Current.StateCode : 0;
                int DistrictCode = 0;
                int BlockCode = 0;
                string PropType = propType;
                int Batch = batch;
                int Year = year;
                int Collaboration = collaboration;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;

                var itemList = dbContext.USP_FORM8_REPORT(1, StateCode, DistrictCode, BlockCode, PropType, Batch, Year, Collaboration, PMGSY).ToList<USP_FORM8_REPORT_Result>();

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
                    id = itemDetails.LOCATION_CODE.ToString().Trim(),
                    cell = new[] {       
                                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='loadForm8DistrictLevelReportGrid(\"" +  itemDetails.LOCATION_CODE.ToString().Trim() +  "\",\"" + itemDetails.LOCATION_NAME + "\",\"" + PropType + "\",\"" +Year + "\",\"" + Batch + "\",\"" + Collaboration + "\"); return false;'>" + itemDetails.LOCATION_NAME + "</a>",
                                        itemDetails.TOTAL_PROPOSAL.ToString(),
                                        itemDetails.TOTAL_AWARD.ToString(),
                                        itemDetails.TOTAL_COMPLETED_PROPOSAL.ToString(),
                                        itemDetails.TOTAL_LENGTH_COMPLETED.ToString(),
                                        itemDetails.TOTAL_PAYMENT_PROPOSAL.ToString(),
                                        itemDetails.TOTAL_PHY_LENGTH.ToString(),
                                        itemDetails.TOTAL_EXP.ToString()
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
        /// Listing of Districtwise Proposals, Cost, Awards & Awarded amount
        /// Exist as it is
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array Form8DistrictLevelListingDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, string propType, int batch, int year, int collaboration)
        {
            dbContext = new Models.PMGSYEntities();
            try
            {
                Int32 StateCode = stateCode == 0 ? PMGSYSession.Current.StateCode : stateCode;
                int DistrictCode = 0;
                int BlockCode = 0;
                string PropType = propType;
                int Batch = batch;
                int Year = year;
                int Collaboration = collaboration;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 720;
                dbContext.Configuration.LazyLoadingEnabled = false;


                var itemList = dbContext.USP_FORM8_REPORT(2, StateCode, DistrictCode, BlockCode, PropType, Batch, Year, Collaboration, PMGSY).ToList<USP_FORM8_REPORT_Result>();

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
                    id = itemDetails.LOCATION_CODE.ToString().Trim(),
                    cell = new[] {       
                                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='loadForm8BlockLevelReportGrid(\"" +  StateCode.ToString().Trim() + "\",\"" + itemDetails.LOCATION_CODE.ToString().Trim()  +  "\",\"" + itemDetails.LOCATION_NAME + "\",\"" + PropType + "\",\"" + Year + "\",\"" + Batch + "\",\"" + Collaboration + "\"); return false;'>" + itemDetails.LOCATION_NAME + "</a>",
                                        itemDetails.TOTAL_PROPOSAL.ToString(),
                                        itemDetails.TOTAL_AWARD.ToString(),
                                        itemDetails.TOTAL_COMPLETED_PROPOSAL.ToString(),
                                        itemDetails.TOTAL_LENGTH_COMPLETED.ToString(),
                                        itemDetails.TOTAL_PAYMENT_PROPOSAL.ToString(),
                                        itemDetails.TOTAL_PHY_LENGTH.ToString(),
                                        itemDetails.TOTAL_EXP.ToString()
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
        /// List the Form7 Details for all Blocks Under Particular District
        /// as it is exist
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="stateCode"></param>
        /// <param name="districtCode"></param>
        /// <returns></returns>
        public Array Form8BlockLevelListingDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int districtCode, string proposalType, int batch, int year, int collaboration)
        {
            dbContext = new Models.PMGSYEntities();
            try
            {
                Int32 StateCode = stateCode == 0 ? PMGSYSession.Current.StateCode : stateCode;
                Int32 DistrictCode = districtCode == 0 ? PMGSYSession.Current.DistrictCode : districtCode;
                int BlockCode = 0;
                string PropType = proposalType;
                int Batch = batch;
                int Year = year;
                int Collaboration = collaboration;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;

                var itemList = dbContext.USP_FORM8_REPORT(3, StateCode, DistrictCode, BlockCode, PropType, Batch, Year, Collaboration, PMGSY).ToList<USP_FORM8_REPORT_Result>();

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
                    id = itemDetails.LOCATION_CODE.ToString().Trim(),
                    cell = new[] {       
                                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='loadForm8FinalLevelReportGrid(\"" +  StateCode.ToString().Trim() + "\",\"" +  DistrictCode.ToString().Trim() + "\",\"" + itemDetails.LOCATION_CODE.ToString().Trim()  +  "\",\"" + itemDetails.LOCATION_NAME + "\",\"" + PropType + "\",\"" + Year + "\",\"" + Batch + "\",\"" + Collaboration + "\"); return false;'>" + itemDetails.LOCATION_NAME + "</a>",
                                        itemDetails.TOTAL_PROPOSAL.ToString(),
                                        itemDetails.TOTAL_AWARD.ToString(),
                                        itemDetails.TOTAL_COMPLETED_PROPOSAL.ToString(),
                                        itemDetails.TOTAL_LENGTH_COMPLETED.ToString(),
                                        itemDetails.TOTAL_PAYMENT_PROPOSAL.ToString(),
                                        itemDetails.TOTAL_PHY_LENGTH.ToString(),
                                        itemDetails.TOTAL_EXP.ToString()
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
        /// List the Form7 Details for all Blocks Under Particular District
        /// as it is exist
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="stateCode"></param>
        /// <param name="districtCode"></param>
        /// <returns></returns>
        public Array Form8FinalLevelListingDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int districtCode, int blockCode, string proposalType, int batch, int year, int collaboration)
        {
            dbContext = new Models.PMGSYEntities();
            try
            {
                Int32 StateCode = stateCode == 0 ? PMGSYSession.Current.StateCode : stateCode;
                Int32 DistrictCode = districtCode == 0 ? PMGSYSession.Current.DistrictCode : districtCode;
                int BlockCode = blockCode;
                string PropType = proposalType;
                int Batch = batch;
                int Year = year;
                int Collaboration = collaboration;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;

                var itemList = dbContext.USP_FORM8_PROP_REPORT(StateCode, DistrictCode, BlockCode, PropType, Batch, Year, Collaboration, PMGSY).ToList<USP_FORM8_PROP_REPORT_Result>();


                totalRecords = itemList.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        itemList = itemList.OrderBy(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        itemList = itemList.OrderByDescending(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    itemList = itemList.OrderBy(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }


                return itemList.Select(itemDetails => new
                {
                    id = itemDetails.MAST_BLOCK_CODE.ToString().Trim(),
                    cell = new[] {       
                                        itemDetails.MAST_BLOCK_NAME,
                                        itemDetails.PLAN_CN_ROAD_CODE,
                                        itemDetails.IMS_ROAD_NAME,
                                        itemDetails.IMS_YEAR.ToString() + "-" + (itemDetails.IMS_YEAR + 1).ToString(),
                                        itemDetails.BRIDGE_NAME,
                                        itemDetails.EXEC_COMPLETED.ToString(),
                                        itemDetails.EXEC_ISCOMPLETED.ToString(),
                                        itemDetails.EXEC_COMPLETION_DATE,
                                        itemDetails.EXEC_PAYMENT_LASTMONTH.ToString(),
                                        itemDetails.EXEC_PAYMENT_THISMONTH.ToString(),
                                        itemDetails.EXEC_VALUEOFWORK_LASTMONTH.ToString(),
                                        itemDetails.EXEC_VALUEOFWORK_THISMONTH.ToString(),
                                        itemDetails.EXEC_FINAL_PAYMENT_FLAG,
                                        itemDetails.EXEC_FINAL_PAYMENT_DATE
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

        #endregion


    }///class ends here







    /// <summary>
    /// Interface for Form Reports
    /// </summary>
    public interface IFormReportsDAL
    {
        #region Form1
        Array Form1StateLevelListingDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords);
        Array Form1DistrictLevelListingDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode);
        Array Form1BlockLevelListingDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int districtCode);
        Array Form1VillageLevelListingDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int districtCode, int blockCode);
        #endregion


        #region Form2
        Array Form2StateLevelListingDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, string constType, int constCode);
        Array Form2DistrictLevelListingDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, string constType, int constCode);
        Array Form2ConstituencyLevelListingDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int constCode, string constType);
        #endregion


        #region Form3
        Array Form3StateLevelListingDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords);
        Array Form3DistrictLevelListingDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode);
        Array Form3BlockLevelListingDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int districtCode);
        #endregion


        #region Form4
        Array Form4StateLevelListingDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int districtCode, int blockCode, int year, int batch, int collaboration, string propType);
        Array Form4DistrictLevelListingDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int districtCode, int blockCode, int year, int batch, int collaboration, string propType);
        Array Form4BlockLevelListingDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int districtCode, int blockCode, int year, int batch, int collaboration, string propType);
        Array Form4FinalLevelListingDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int districtCode, int blockCode, int year, int batch, int collaboration, string propType);

        #endregion


        #region Form5/6
        Array Form5StateLevelListingDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int year, string constType, int constCode);
        Array Form5DistrictLevelListingDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int year, int stateCode, string constType, int constCode);
        Array Form5ConstituencyLevelListingDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int year, int stateCode, int constCode, string constType);
        #endregion


        #region Form7
        Array Form7StateLevelListingDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, string propType, int batch, int year, int collaboration);
        Array Form7DistrictLevelListingDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, string propType, int batch, int year, int collaboration);
        Array Form7BlockLevelListingDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int districtCode, string ProposalType, int batch, int year, int collaboration);
        Array Form7FinalLevelListingDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int districtCode, int blockCode, string propType, int batch, int year, int collaboration);
        #endregion


        #region Form8
        Array Form8StateLevelListingDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, string propType, int batch, int year, int collaboration);
        Array Form8DistrictLevelListingDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, string propType, int batch, int year, int collaboration);
        Array Form8BlockLevelListingDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int districtCode, string ProposalType, int batch, int year, int collaboration);
        Array Form8FinalLevelListingDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int districtCode, int blockCode, string proposalType, int batch, int year, int collaboration);
        #endregion
    }
}