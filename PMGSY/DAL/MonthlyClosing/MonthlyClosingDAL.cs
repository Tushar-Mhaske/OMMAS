using PMGSY.Common;
using PMGSY.Extensions;
using PMGSY.Models;
using PMGSY.Models.Common;
using PMGSY.Models.MonthlyClosing;
using PMGSY.Models.PaymentModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.DAL.MonthlyClosing
{


    public class MonthlyClosingDAL : IMonthlyClosingDAL
    {
        PMGSYEntities dbContext =null;
        CommonFunctions common = null;

        /// <summary>
        /// DAL function to return closed month and year
        /// </summary>
        /// <param name="objparams"></param>
        /// <returns>1- id no month is closed else month year</returns>
        public string GetClosedMonthAndYear(TransactionParams objparams)
        {

           try
           {
              dbContext = new PMGSYEntities ();

              if (dbContext.ACC_RPT_MONTHWISE_SUMMARY.Where(c => c.ADMIN_ND_CODE == objparams.ADMIN_ND_CODE && c.LVL_ID == objparams.LVL_ID && c.FUND_TYPE == objparams.FUND_TYPE).Any())
               {

                   Int32 maxClosedYear = dbContext.ACC_RPT_MONTHWISE_SUMMARY.Where(c => c.ADMIN_ND_CODE == objparams.ADMIN_ND_CODE && c.LVL_ID == objparams.LVL_ID && c.FUND_TYPE==objparams.FUND_TYPE).Max(d => d.ACC_YEAR);
                   Int32 maxClosedMonth = dbContext.ACC_RPT_MONTHWISE_SUMMARY.Where(c => c.ADMIN_ND_CODE == objparams.ADMIN_ND_CODE && c.LVL_ID == objparams.LVL_ID && c.ACC_YEAR == maxClosedYear && c.FUND_TYPE == objparams.FUND_TYPE).Max(d => d.ACC_MONTH);



                   return CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(maxClosedMonth) + "$" + maxClosedYear;
               
               }
               else 
               {
                   return "-1";
               }
            
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw new Exception("Error While getting monthly closing details.");

            }
            finally
           {           
                dbContext.Dispose();
           
           }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objparams"></param>
        /// <returns></returns>
        public string GetAccountStartMonthandYear(TransactionParams objparams)
        {

            try
            {
                dbContext = new PMGSYEntities();

                if (dbContext.ACC_BILL_MASTER.Where(c => c.ADMIN_ND_CODE == objparams.ADMIN_ND_CODE &&
                    c.LVL_ID == objparams.LVL_ID && c.FUND_TYPE == objparams.FUND_TYPE).Any())
                {

                    Int32 minBillYear = dbContext.ACC_BILL_MASTER.Where(c => c.ADMIN_ND_CODE == objparams.ADMIN_ND_CODE && c.LVL_ID == objparams.LVL_ID && c.FUND_TYPE ==objparams.FUND_TYPE).Min(d => d.BILL_YEAR);
                    Int32 minBillMonth = dbContext.ACC_BILL_MASTER.Where(c => c.ADMIN_ND_CODE == objparams.ADMIN_ND_CODE && c.LVL_ID == objparams.LVL_ID && c.BILL_YEAR == minBillYear && c.FUND_TYPE == objparams.FUND_TYPE).Min(d => d.BILL_MONTH);

                    return CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(minBillMonth) + "$" + minBillYear;

                }
                else
                {
                    return "-1";
                }

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw new Exception("Error While getting monthly closing details.");

            }
            finally
            {
                dbContext.Dispose();

            }

        }



        /// <summary>
        /// function to list the DPIU details who has not closed the month
        /// </summary>
        /// <param name="objFilter"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array ListDPIUMonthNotClosed(PaymentFilterModel objFilter, short month, short year,out long totalRecords)
        {

             dbContext = new PMGSYEntities ();
             common = new CommonFunctions();
 
            try
            {
                               
                TransactionParams objParam = new TransactionParams();
                
                objParam.FUND_TYPE = objFilter.FundType;
                objParam.LVL_ID = objFilter.LevelId;
                objParam.ADMIN_ND_CODE = objFilter.AdminNdCode;
                objParam.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;

               //get all the pius 

                List<Int32> lstDPIU = new List<int> ();
                //var lstDPIU = (IEnumerable<dynamic>)null;
                dbContext = new PMGSYEntities();
              
                if (objParam.DISTRICT_CODE == 0)
                {
                    //lstDPIU = dbContext.ADMIN_DEPARTMENT.Where(m => m.MAST_PARENT_ND_CODE == objParam.ADMIN_ND_CODE && m.MAST_ND_TYPE == "D").Select(x=>x.ADMIN_ND_CODE).ToList();
                    lstDPIU = (from bm in dbContext.ACC_BILL_MASTER
                               join ad in dbContext.ADMIN_DEPARTMENT
                               on bm.ADMIN_ND_CODE equals ad.ADMIN_ND_CODE
                               where
                               bm.FUND_TYPE == PMGSYSession.Current.FundType &&
                               ad.MAST_PARENT_ND_CODE == PMGSYSession.Current.AdminNdCode &&
                               //Old
                               //(bm.BILL_MONTH + (bm.BILL_YEAR * 12)) <= (month + (year* 12)) &&
                               //modified By Abhishek kamble 17July2014 
                               (bm.BILL_MONTH + (bm.BILL_YEAR * 12)) <= ((month==0?objFilter.Month:month) + ((month==0?objFilter.Year:year) * 12)) &&
                               ad.MAST_ND_TYPE=="D"  //Added By Abhishek to show onli DPIU's 3-July-2014
                               //new
                               && (ad.ADMIN_ND_ACTIVE == "N" ? ((ad.ADMIN_ND_CLOSE_DATE.Value.Month + (ad.ADMIN_ND_CLOSE_DATE.Value.Year * 12)) >= ((month == 0 ? objFilter.Month : month) + ((month == 0 ? objFilter.Year : year) * 12))) : true)
                               
                               select bm.ADMIN_ND_CODE).Distinct().ToList();
                }
                else
                {
                    lstDPIU = dbContext.ADMIN_DEPARTMENT.Where(m => m.MAST_DISTRICT_CODE == objParam.DISTRICT_CODE).Select(x => x.ADMIN_ND_CODE).ToList();
                }


                #region for single month closing

                if (month == 0)
                {

                    //get the piu who has closed the month in given duration
                    List<int> DPIUListMonthClosed = dbContext.ACC_RPT_MONTHWISE_SUMMARY.Where
                      (c => lstDPIU.Contains(c.ADMIN_ND_CODE) && c.ACC_MONTH == objFilter.Month
                          && c.ACC_YEAR == objFilter.Year && c.FUND_TYPE ==objFilter.FundType).Select(d => d.ADMIN_ND_CODE).ToList();

                    //get the dpiu who has not closed
                    lstDPIU = lstDPIU.Except(DPIUListMonthClosed).ToList();

                    //remove the DPIU who have not started their account
                    List<int> DPIUAccountNotStarted =new List<int> ();
                    foreach (int dpiu in lstDPIU)
                    { 
                       if( dbContext.ACC_BILL_MASTER.Any(c => lstDPIU.Contains(c.ADMIN_ND_CODE) && c.FUND_TYPE == objFilter.FundType) == false)
                       {
                           DPIUAccountNotStarted.Add(dpiu);
                       }
                    }

                    lstDPIU = lstDPIU.Except(DPIUAccountNotStarted).ToList();

                    //get max month & year closed by above DPIU
                    List<MaxMonthYearClosedModel> ClosingDetailsList = new List<MaxMonthYearClosedModel>();

                    foreach (int dpiu in lstDPIU)
                    {
                        if (dbContext.ACC_RPT_MONTHWISE_SUMMARY.Any(c => lstDPIU.Contains(c.ADMIN_ND_CODE) && c.FUND_TYPE == objFilter.FundType) == true)
                        {
                            MaxMonthYearClosedModel ClosingDetails = new MaxMonthYearClosedModel();

                            ClosingDetails.CLOSING_YEAR =Convert.ToInt16(dbContext.ACC_RPT_MONTHWISE_SUMMARY.Where(c => c.ADMIN_ND_CODE == dpiu && c.FUND_TYPE == objFilter.FundType).Max(d => (short?)d.ACC_YEAR));
                            ClosingDetails.CLOSING_MONTH = Convert.ToInt16(dbContext.ACC_RPT_MONTHWISE_SUMMARY.Where(c => c.ADMIN_ND_CODE == dpiu && c.ACC_YEAR == ClosingDetails.CLOSING_YEAR && c.FUND_TYPE == objFilter.FundType).Max(d => (short?)d.ACC_MONTH));
                            ClosingDetails.ADMIN_ND_CODE = dpiu;

                            ClosingDetailsList.Add(ClosingDetails);
                        }
                    }

                   
                    //List<ACC_RPT_MONTHWISE_SUMMARY> ClosingDetails = dbContext.ACC_RPT_MONTHWISE_SUMMARY.Where(x => lstDPIU.Contains(x.ADMIN_ND_CODE)).Max(x=>x.ACC_YEAR).ToList();

                    List<ADMIN_DEPARTMENT> deptList = new List<ADMIN_DEPARTMENT>();

                    //get only thoes dpiu which did not closed month
                    deptList = dbContext.ADMIN_DEPARTMENT.Where(m => lstDPIU.Contains(m.ADMIN_ND_CODE)).ToList<ADMIN_DEPARTMENT>();

                    //order by admin name
                    deptList = deptList.OrderBy(x => x.ADMIN_ND_NAME).ToList();

                    totalRecords = deptList.Count();

                    if (objFilter.sidx.Trim() != string.Empty)
                    {
                        if (objFilter.sord == "asc")
                        {
                            deptList = deptList.OrderBy(m => m.ADMIN_ND_NAME).Skip(Convert.ToInt32(objFilter.page *objFilter.rows)).Take(Convert.ToInt32(objFilter.rows)).ToList();
                        }
                        else
                        {
                            deptList = deptList.OrderByDescending(m => m.ADMIN_ND_NAME).Skip(Convert.ToInt32(objFilter.page *objFilter.rows)).Take(Convert.ToInt32(objFilter.rows)).ToList();
                        }
                    }

                    return deptList.Select(item => new
                    {

                        cell = new[]
                        {                         
                                     item.ADMIN_ND_NAME,
                                    // CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(objFilter.Month) + " " + objFilter.Year
                                     ClosingDetailsList.Any(x=>x.ADMIN_ND_CODE ==item.ADMIN_ND_CODE) ? ClosingDetailsList.Where(x=>x.ADMIN_ND_CODE ==item.ADMIN_ND_CODE).Select(x=>x.CLOSING_MONTH).First() == 0 ?"Not Available":
                                     CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName( ClosingDetailsList.Where(x=>x.ADMIN_ND_CODE ==item.ADMIN_ND_CODE).Select(x=>(int)x.CLOSING_MONTH).First()) + " " + ClosingDetailsList.Where(x=>x.ADMIN_ND_CODE ==item.ADMIN_ND_CODE).Select(x=>x.CLOSING_YEAR).First()
                                     :"Monthly closing not done"
                        }
                    }).ToArray();
                }

                #endregion for single month closing


                #region for multiple month closing

                //get all months in between

                List<int> months = new List<int>();
                List<int> years = new List<int>();
                
                short fromMonth = objFilter.Month;
                short fromYear= objFilter.Year;


                if (year == objFilter.Year)
                {
                    var monthsList = Enumerable.Range(objFilter.Month, ((month+1) - objFilter.Month));

                    months = monthsList.Select(x => x).ToList();

                    foreach (int item in monthsList)
                    {

                        years.Add(year);

                    }
                }
                else {
                   
                    while (fromMonth != month && fromYear != year)
                    {
                        months.Add(fromMonth); fromMonth++;
                        years.Add(fromYear);

                        if (fromMonth == 13)
                        {
                            fromMonth = 1;
                            fromYear =Convert.ToInt16(fromYear + 1);
                        }
                    }

                    months.Add(month);
                    years.Add(year);
                
                }

                //foreach dpiu check whether it has closed the months and years in given duration
                 List<int> dPIUListMonthNotClosed =new List<int>();
                 List<int> notClosedMonth = new List<int>();
                 List<int> notClosedYear = new List<int>();

                foreach (var dpiu in lstDPIU)
                {
                    for (int i = 0; months.Count > i; i++)
                    {
                        int monthVal = months[i];
                        int yearVal = years[i];
                        
                        
                        //get the piu who has closed the month in given duration
                        if (dbContext.ACC_RPT_MONTHWISE_SUMMARY.Where
                          (c => c.ADMIN_ND_CODE == dpiu &&
                              c.ACC_MONTH == monthVal &&
                              c.ACC_YEAR == yearVal
                              && c.FUND_TYPE ==objFilter.FundType).Any())
                        {

                        }
                        else
                        {
                            dPIUListMonthNotClosed.Add(dpiu);
                            notClosedMonth.Add(monthVal);
                            notClosedYear.Add(yearVal);
                            break;
                        }

                    }

                }

                
                //remove the DPIU who have not started their account
                List<int> DPIUAccountNotStartedForMultipleMonth = new List<int>();
                foreach (int dpiu in lstDPIU)
                {
                    if (dbContext.ACC_BILL_MASTER.Any(c => dPIUListMonthNotClosed.Contains(c.ADMIN_ND_CODE) && c.FUND_TYPE == objFilter.FundType) == false)
                    {
                        DPIUAccountNotStartedForMultipleMonth.Add(dpiu);
                    }
                }

                dPIUListMonthNotClosed = dPIUListMonthNotClosed.Except(DPIUAccountNotStartedForMultipleMonth).Distinct().ToList();

                //get max month & year closed by above DPIU
                List<MaxMonthYearClosedModel> ClosingDetailsListForMultipleMonths = new List<MaxMonthYearClosedModel>();

                foreach (int dpiu in dPIUListMonthNotClosed)
                {
                    if (dbContext.ACC_RPT_MONTHWISE_SUMMARY.Any(c => lstDPIU.Contains(c.ADMIN_ND_CODE) && c.FUND_TYPE == objFilter.FundType) == true)
                    {
                        MaxMonthYearClosedModel ClosingDetails = new MaxMonthYearClosedModel();

                        ClosingDetails.CLOSING_YEAR = Convert.ToInt16(dbContext.ACC_RPT_MONTHWISE_SUMMARY.Where(c => c.ADMIN_ND_CODE == dpiu && c.FUND_TYPE == objFilter.FundType).Max(d => d.ACC_YEAR));
                        ClosingDetails.CLOSING_MONTH = Convert.ToInt16(dbContext.ACC_RPT_MONTHWISE_SUMMARY.Where(c => c.ADMIN_ND_CODE == dpiu && c.ACC_YEAR == ClosingDetails.CLOSING_YEAR && c.FUND_TYPE == objFilter.FundType).Max(d => d.ACC_MONTH));
                        ClosingDetails.ADMIN_ND_CODE = dpiu;

                        ClosingDetailsListForMultipleMonths.Add(ClosingDetails);
                    }
                }

                //get information about dpiu
                List<ADMIN_DEPARTMENT> dpiuDetails = dbContext.ADMIN_DEPARTMENT.Where(m => dPIUListMonthNotClosed.Contains(m.ADMIN_ND_CODE)).ToList<ADMIN_DEPARTMENT>();

                dpiuDetails = dpiuDetails.OrderBy(x => x.ADMIN_ND_NAME).ToList();

                totalRecords = dpiuDetails.Count();

                return dpiuDetails.Select(item => new
                {

                    cell = new[] {                         
                                    // item.ADMIN_ND_NAME,
                                    // CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(notClosedMonth[dPIUListMonthNotClosed.IndexOf(item.ADMIN_ND_CODE)]) + " " +  (notClosedYear[dPIUListMonthNotClosed.IndexOf(item.ADMIN_ND_CODE)])
                                     item.ADMIN_ND_NAME,
                                    // CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(objFilter.Month) + " " + objFilter.Year
                                     ClosingDetailsListForMultipleMonths.Any(x=>x.ADMIN_ND_CODE ==item.ADMIN_ND_CODE) ? (ClosingDetailsListForMultipleMonths.Where(x=>x.ADMIN_ND_CODE ==item.ADMIN_ND_CODE).Select(x=>x.CLOSING_MONTH).First() == 0 ?"Not Available":
                                     CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName( ClosingDetailsListForMultipleMonths.Where(x=>x.ADMIN_ND_CODE ==item.ADMIN_ND_CODE).Select(x=>(int)x.CLOSING_MONTH).First()) + " " + ClosingDetailsListForMultipleMonths.Where(x=>x.ADMIN_ND_CODE ==item.ADMIN_ND_CODE).Select(x=>x.CLOSING_YEAR).First())
                                     :"Monthly closing not done"
                        }
                }).ToArray();


                #endregion for multiple month closing

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
        /// dal function to close the month
        /// </summary>
        /// <param name="fromMonth1"></param>
        /// <param name="fromYear1"></param>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <param name="adminNdCode"></param>
        /// <param name="FundType"></param>
        /// <returns></returns>
        public String CloseMonth(short fromMonth1,short fromYear1, short month,short year,int adminNdCode,String FundType,short level_Id)
        {

            dbContext = new PMGSYEntities();
            common = new CommonFunctions();

            try
            {

                long bill_id = 0;
                dbContext = new PMGSYEntities();
                  //Trans scope Commented by Abhishke kamble 28-Feb-2014
                //using (var scope = new TransactionScope())
                //{

                    TransactionParams objParam = new TransactionParams();
                    objParam.FUND_TYPE = FundType;
                    //Parameter modified by Abhishek for Monthly Closing of DPIU At SRRDA level     28-Aug-2014                    
                    //old
                    //objParam.LVL_ID = PMGSYSession.Current.LevelId;
                    objParam.LVL_ID = level_Id;
                    objParam.ADMIN_ND_CODE = adminNdCode;
                    objParam.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;

                    //get all the pius 

                    List<Int32> lstDPIU = new List<int>();

                    dbContext = new PMGSYEntities();

                    if (objParam.DISTRICT_CODE == 0)
                    {
                        lstDPIU = dbContext.ADMIN_DEPARTMENT.Where(m => m.MAST_PARENT_ND_CODE == objParam.ADMIN_ND_CODE && m.MAST_ND_TYPE == "D").Select(x => x.ADMIN_ND_CODE).ToList();
                    }
                    else
                    {
                        lstDPIU = dbContext.ADMIN_DEPARTMENT.Where(m => m.MAST_DISTRICT_CODE == objParam.DISTRICT_CODE).Select(x => x.ADMIN_ND_CODE).ToList();
                    }

                    #region for single month closing

                    if (month == 0)
                    {
                        //modified By Abhishek Kamble 30-nov-2013

                        //Time out added by Abhishek kamble 11-June-2014
                        ((System.Data.Entity.Infrastructure.IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 1200;
                        dbContext.Configuration.LazyLoadingEnabled = false;

                        var results = dbContext.Database.SqlQuery<monthlyClosingResultModel>("EXEC omms.USP_RPT_INSERT_BALANCES_MONTHWISE @P_INT_AdminNDCode,@P_INT_AccMonth,@P_INT_AccYear,@P_CHAR_fundType,@Prm_Use_ID,@Prm_IPADDRESS",
                        new SqlParameter("P_INT_AdminNDCode", adminNdCode),
                        new SqlParameter("P_INT_AccMonth", fromMonth1),
                        new SqlParameter("P_INT_AccYear", fromYear1),
                        new SqlParameter("P_CHAR_fundType", FundType),
                        new SqlParameter("Prm_Use_ID",PMGSYSession.Current.UserId),
                        new SqlParameter("Prm_IPADDRESS",(HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]))
                        ).FirstOrDefault();
                     
                        dbContext.SaveChanges();

                        if (results.ERR_LINE_NUMBER == -111)
                        {
                            //successful operation

                            //added by Koustubh Nakate on 22/08/2013 to save notification in notification details table 

                            bill_id = fromMonth1 + (fromYear1 * 12);

                            //dbContext.USP_ACC_INSERT_ALERT_DETAILS(adminNdCode, FundType, "M", objParam.LVL_ID, bill_id,PMGSYSession.Current.UserId,HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]);
                        }
                        else
                        {
                            throw new Exception("Error while closing month");
                        }
                    }
                    else
                    {

                    #endregion for single month closing

                     #region for multiple month closing
                        //get all months in between
                        List<int> months = new List<int>();
                        List<int> years = new List<int>();
                        short fromMonth = fromMonth1;
                        short fromYear = fromYear1;

                        if (year == fromYear1)
                        {
                            var monthsList = Enumerable.Range(fromMonth1, ((month + 1) - fromMonth1));

                            months = monthsList.Select(x => x).ToList();
                            foreach (int item in monthsList)
                            {
                                years.Add(year);
                            }
                        }
                        else
                        {
                            //while (fromMonth != month && fromYear != year)
                            while (fromMonth != month && fromYear <= year)//Modified By Abhishek to close multiple months 17-July-2014
                            {
                                months.Add(fromMonth); fromMonth++;
                                years.Add(fromYear);

                                if (fromMonth == 13)
                                {
                                    fromMonth = 1;
                                    fromYear = Convert.ToInt16(fromYear + 1);
                                }
                            }

                            months.Add(month);
                            years.Add(year);
                        }

                        for (int i = 0; months.Count > i; i++)
                        {
                            //Time out added by Abhishek kamble 11-June-2014      
                            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 1200;
                            dbContext.Configuration.LazyLoadingEnabled = false;

                            var results = dbContext.Database.SqlQuery<monthlyClosingResultModel>("EXEC omms.USP_RPT_INSERT_BALANCES_MONTHWISE @P_INT_AdminNDCode,@P_INT_AccMonth,@P_INT_AccYear,@P_CHAR_fundType,@Prm_Use_ID,@Prm_IPADDRESS",
                            new SqlParameter("P_INT_AdminNDCode", adminNdCode),
                            new SqlParameter("P_INT_AccMonth", months[i]),
                            new SqlParameter("P_INT_AccYear", years[i]),
                            new SqlParameter("P_CHAR_fundType", FundType),
                             new SqlParameter("Prm_Use_ID",PMGSYSession.Current.UserId),
                        new SqlParameter("Prm_IPADDRESS",(HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]))
                            ).FirstOrDefault();

                            dbContext.SaveChanges();

                            if (results.ERR_LINE_NUMBER == -111)
                            {
                                //successful operation

                                //added by Koustubh Nakate on 22/08/2013 to save notification in notification details table 

                                bill_id = months[i] + (years[i] * 12);

                                //dbContext.USP_ACC_INSERT_ALERT_DETAILS(adminNdCode, FundType, "M", objParam.LVL_ID, bill_id,PMGSYSession.Current.UserId,HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]);

                            }
                            else
                            {
                                throw new Exception("Error while closing month");
                            }
                        }
                    }
                    #endregion for multiple month closing
                    
                    dbContext.SaveChanges();
                   // scope.Complete();
                    return "1";
               // }      end of Scope
            }
            catch(System.Data.Entity.Core.EntityException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return null;
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
        /// DAL  to validate if all dpiu have closed their month 
        /// </summary>
        /// <param name="adminNdCode"></param>
        /// <param name="monthToClose"></param>
        /// <param name="yearToClose"></param>
        /// <param name="fundType"></param>
        /// <param name="levelID"></param>
        /// <returns> -666 if all DPIU has closed month else reason</returns>
        public string CheckAllPiuForMonthlyClosing(int adminNdCode, short monthToClose, short yearToClose, string fundType, short levelID)
        {
               List<Int32> lstDPIU = new List<int>();

               common = new CommonFunctions();

               PMGSYEntities localdbContext2 = new PMGSYEntities();

               string monthlyClosingStatus = string.Empty;

            try
            {
                //old code
                //lstDPIU = localdbContext2.ADMIN_DEPARTMENT.Where(m => m.MAST_PARENT_ND_CODE == adminNdCode && m.MAST_ND_TYPE == "D").Select(x => x.ADMIN_ND_CODE).ToList();                 
                
                //New Code Modified by Abhishek kamble 7-Aug-2014 to skip inactiveted DPIU's and and whos ADMIN_ND_CLOSE_DATE < month and year to close.
                lstDPIU = localdbContext2.ADMIN_DEPARTMENT.Where(m => m.MAST_PARENT_ND_CODE == adminNdCode
                       && m.MAST_ND_TYPE == "D"                       
                       //&& (m.ADMIN_ND_ACTIVE == "N" ? ( (m.ADMIN_ND_CLOSE_DATE.Value.Month <= monthToClose) && (m.ADMIN_ND_CLOSE_DATE.Value.Year <= yearToClose) ):true)
                       && (m.ADMIN_ND_ACTIVE == "N" ? ( (m.ADMIN_ND_CLOSE_DATE.Value.Month+ (m.ADMIN_ND_CLOSE_DATE.Value.Year*12) ) >= (monthToClose+(yearToClose*12))) : true)
                       ).Select(x => x.ADMIN_ND_CODE).ToList();


                   foreach (int item in lstDPIU)
                   {
                      monthlyClosingStatus =  common.MonthlyClosingStatus(item, monthToClose, yearToClose, fundType, 5,true);

                      if (monthlyClosingStatus.Split('$')[0] != "-666")
                      {
                          return monthlyClosingStatus;
                      } 

                   }

                   return "-666";

            }
            catch (Exception ex)
            {

                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return null;
            }
            finally
            {
                localdbContext2.Dispose();
            }
        }

              
        //Validation Added By Abhishek kamble to Check PIU's Check Ack Status 18-July-2014 start
        public List<USP_ACC_VERIFY_PIUS_CHEQUEACK_Result> CheckAllPiuForChequeAck(string FundType, int AdminNdCode, int FromMonth, int FromYear, int ToMonth, int ToYear, String SingleMultipleMonth)
        {
            dbContext = new PMGSYEntities();
            try
            {
                //var data = dbContext.USP_ACC_VERIFY_PIUS_CHEQUEACK(FundType, AdminNdCode, FromMonth, FromYear, ToMonth,ToYear,SingleMultipleMonth).ToList();                                               

               List<USP_ACC_VERIFY_PIUS_CHEQUEACK_Result> piuDetails = dbContext.USP_ACC_VERIFY_PIUS_CHEQUEACK(FundType, AdminNdCode, FromMonth, FromYear, ToMonth, ToYear, SingleMultipleMonth).ToList();

               return piuDetails;
            }   
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return null;            
            }
            finally {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
                         
        }
        //Validation Added By Abhishek kamble to Check PIU's Check Ack Status 18-July-2014 end

        #region Closed Month and Year
        public string GetClosedMonthAndYearDAL(TransactionParams objparams)
        {

            try
            {
                dbContext = new PMGSYEntities();

                if (dbContext.ACC_RPT_MONTHWISE_SUMMARY.Where(c => c.ADMIN_ND_CODE == objparams.ADMIN_ND_CODE && c.LVL_ID == objparams.LVL_ID && c.FUND_TYPE == objparams.FUND_TYPE).Any())
                {

                    Int32 maxClosedYear = dbContext.ACC_RPT_MONTHWISE_SUMMARY.Where(c => c.ADMIN_ND_CODE == objparams.ADMIN_ND_CODE && c.LVL_ID == objparams.LVL_ID && c.FUND_TYPE == objparams.FUND_TYPE).Max(d => d.ACC_YEAR);
                    Int32 maxClosedMonth = dbContext.ACC_RPT_MONTHWISE_SUMMARY.Where(c => c.ADMIN_ND_CODE == objparams.ADMIN_ND_CODE && c.LVL_ID == objparams.LVL_ID && c.ACC_YEAR == maxClosedYear && c.FUND_TYPE == objparams.FUND_TYPE).Max(d => d.ACC_MONTH);

                    //return CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(maxClosedMonth) + "$" + maxClosedYear;
                    return maxClosedMonth + "$" + maxClosedYear;

                }
                else
                {
                    return "-1";
                }

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw new Exception("Error While getting monthly closing details.");

            }
            finally
            {
                dbContext.Dispose();

            }

        }
        #endregion
    }

    interface IMonthlyClosingDAL 
    {
        string GetClosedMonthAndYear(TransactionParams objparams);
        Array ListDPIUMonthNotClosed(PaymentFilterModel objFilter, short month, short year,out long totalRecords);
        String CloseMonth(short fromMonth, short fromYear, short toMonth, short toYear, int adminNdCode, string FundType, short level_Id);
        string GetAccountStartMonthandYear(TransactionParams objparams);
        string CheckAllPiuForMonthlyClosing(int adminNdCode, short monthToClose, short yearToClose, string fundType, short levelID);
        //Validation Added By Abhishek kamble to Check PIU's Check Ack Status 18-July-2014
        List<USP_ACC_VERIFY_PIUS_CHEQUEACK_Result> CheckAllPiuForChequeAck(string FundType, int AdminNdCode, int FromMonth, int FromYear, int ToMonth, int ToYear, String SingleMultipleMonth);
    }
    
}