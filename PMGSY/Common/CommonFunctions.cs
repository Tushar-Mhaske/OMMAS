using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.Controllers;
using PMGSY.Models;
using PMGSY.Models.Common;
using PMGSY.Extensions;
using System.Globalization;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Text;
using PMGSY.Models.PaymentModel;
using System.Text.RegularExpressions;
using System.Data.Entity;
using System.IO;
using System.Drawing;
using System.Configuration;
using FileTypeDetective;
using System.Net;
using System.Data.SqlClient;
//using System.Data.Entity.Objects.SqlClient;
using System.Data.SqlClient;
using PMGSY.Models.ExistingRoads;
using System.Data.Entity.SqlServer;
using System.Data;
using PMGSY.Models.QualityMonitoring;

namespace PMGSY.Common
{
    public class CommonFunctions : Controller
    {
        PMGSYEntities dbContext = null;

        public static List<SelectListItem> PopulateYesNo()
        {
            List<SelectListItem> PopulateYesNo = new List<SelectListItem>();
            PopulateYesNo.Add(new SelectListItem { Text = "--Select--", Value = "" });
            PopulateYesNo.Add(new SelectListItem { Text = "Yes", Value = "true" });
            PopulateYesNo.Add(new SelectListItem { Text = "No", Value = "false" });
            return PopulateYesNo;
        }

        public static bool IsDate(string sdate)
        {
            DateTime dt;
            bool isDate = true;
            try
            {
                dt = DateTime.Parse(sdate);
            }
            catch
            {
                isDate = false;
            }
            return isDate;
        }

        public List<SelectListItem> PopulateMonthsforCurrentFinancialYear(bool isPopulateFirstItem = true)
        {
            List<SelectListItem> lstMonths = new List<SelectListItem>();
            SelectListItem lstItem;

            try
            {
                dbContext = new PMGSYEntities();

                var a = (from item in dbContext.MASTER_MONTH
                         where item.MAST_MONTH_CODE > 3 && item.MAST_MONTH_CODE <= 12//DateTime.Now.Month
                         select new
                         {
                             MAST_MONTH_CODE = item.MAST_MONTH_CODE,
                             MAST_MONTH_FULL_NAME = item.MAST_MONTH_FULL_NAME
                         });
                var b = (from item in dbContext.MASTER_MONTH
                         where item.MAST_MONTH_CODE <= DateTime.Now.Month
                         select new
                         {
                             MAST_MONTH_CODE = item.MAST_MONTH_CODE,
                             MAST_MONTH_FULL_NAME = item.MAST_MONTH_FULL_NAME
                         });
                #region Old Code
                //List<SelectListItem> lstMonths = new SelectList(dbContext.MASTER_MONTH.Where(x => x.MAST_MONTH_CODE > 3 && x.MAST_MONTH_CODE <= DateTime.Now.Month), "MAST_MONTH_CODE", "MAST_MONTH_FULL_NAME").Union(dbContext.MASTER_MONTH.Where(x => x.MAST_MONTH_CODE <= DateTime.Now.Month), "MAST_MONTH_CODE", "MAST_MONTH_FULL_NAME").ToList();
                /*var query = (DateTime.Now.Month < 3)
                            ? (from item in dbContext.MASTER_MONTH
                               where item.MAST_MONTH_CODE > 3 && item.MAST_MONTH_CODE <= 12//DateTime.Now.Month
                               select new
                               {
                                   MAST_MONTH_CODE = item.MAST_MONTH_CODE,
                                   MAST_MONTH_FULL_NAME = item.MAST_MONTH_FULL_NAME
                               }).Union(from item in dbContext.MASTER_MONTH
                                        where item.MAST_MONTH_CODE <= DateTime.Now.Month
                                        select new
                                        {
                                            MAST_MONTH_CODE = item.MAST_MONTH_CODE,
                                            MAST_MONTH_FULL_NAME = item.MAST_MONTH_FULL_NAME
                                        }).ToList()
                            : (from item in dbContext.MASTER_MONTH
                               where item.MAST_MONTH_CODE > 3 && item.MAST_MONTH_CODE <= DateTime.Now.Month
                               select new
                               {
                                   MAST_MONTH_CODE = item.MAST_MONTH_CODE,
                                   MAST_MONTH_FULL_NAME = item.MAST_MONTH_FULL_NAME
                               }).ToList();*/
                #endregion
                int currMonth = DateTime.Now.Month;

                //Avinash:-For Physical road progress April Month Relaxation..
                string AprilMonth = ConfigurationManager.AppSettings["PHYSICAL_ROAD_PROGRESS_APRIL_MONTH"]; //4
                int AprilMonthValue = Convert.ToInt16(AprilMonth);

                string AprilMonthDay = ConfigurationManager.AppSettings["PHYSICAL_ROAD_PROGRESS_APRIL_MONTH_DAY"];  //10
                int AprilMonthDayValue = Convert.ToInt16(AprilMonthDay);
                if (currMonth == AprilMonthValue)
                {
                    //var query = (DateTime.Now.Day <= AprilMonthDayValue) 
                    //            ? (from item in dbContext.MASTER_MONTH
                    //               where (
                    //                        (currMonth == 1)
                    //                            ? (item.MAST_MONTH_CODE == 12 || (item.MAST_MONTH_CODE == DateTime.Now.Month))
                    //                            : (item.MAST_MONTH_CODE == (currMonth - 1) || (item.MAST_MONTH_CODE == currMonth))
                    //                     )
                    //               select new
                    //               {
                    //                   MAST_MONTH_CODE = item.MAST_MONTH_CODE,
                    //                   MAST_MONTH_FULL_NAME = item.MAST_MONTH_FULL_NAME
                    //               }).ToList()
                    //            : (from item in dbContext.MASTER_MONTH
                    //               where item.MAST_MONTH_CODE == currMonth
                    //               select new
                    //               {
                    //                   MAST_MONTH_CODE = item.MAST_MONTH_CODE,
                    //                   MAST_MONTH_FULL_NAME = item.MAST_MONTH_FULL_NAME
                    //               }).ToList();

                    //If Day is Less than or equal to 10 ..then show only March...else Show April
                    var query = (DateTime.Now.Day <= AprilMonthDayValue)
                               ? (from item in dbContext.MASTER_MONTH
                                  where (item.MAST_MONTH_CODE == currMonth - 1)
                                  select new
                                  {
                                      MAST_MONTH_CODE = item.MAST_MONTH_CODE,
                                      MAST_MONTH_FULL_NAME = item.MAST_MONTH_FULL_NAME
                                  }).ToList()
                                : (from item in dbContext.MASTER_MONTH
                                   where item.MAST_MONTH_CODE == currMonth
                                   select new
                                   {
                                       MAST_MONTH_CODE = item.MAST_MONTH_CODE,
                                       MAST_MONTH_FULL_NAME = item.MAST_MONTH_FULL_NAME
                                   }).ToList();
                    if (query != null)
                    {
                        foreach (var itm in query)
                        {
                            lstItem = new SelectListItem();
                            lstItem.Value = itm.MAST_MONTH_CODE.ToString();
                            lstItem.Text = itm.MAST_MONTH_FULL_NAME.ToString();
                            lstMonths.Add(lstItem);

                        }
                    }
                }
                else
                {
                    //var query = (DateTime.Now.Day <= 5)
                    var query = (DateTime.Now.Day <= 5) //change by Pradip Patil as per sugg. by pankaj sir o n 28-03-2018
                                ? (from item in dbContext.MASTER_MONTH
                                   where (
                                       ///Added by SAMMED A. PATIL on 01JAN2018 for year change case
                                            (currMonth == 1)
                                                ? (item.MAST_MONTH_CODE == 12 || (item.MAST_MONTH_CODE == DateTime.Now.Month))
                                                : (item.MAST_MONTH_CODE == (currMonth - 1) || (item.MAST_MONTH_CODE == currMonth))//(item.MAST_MONTH_CODE == (DateTime.Now.Month - 1)) || (item.MAST_MONTH_CODE == DateTime.Now.Month)
                                         )
                                   select new
                                   {
                                       MAST_MONTH_CODE = item.MAST_MONTH_CODE,
                                       MAST_MONTH_FULL_NAME = item.MAST_MONTH_FULL_NAME
                                   }).ToList()
                                : (from item in dbContext.MASTER_MONTH
                                   where item.MAST_MONTH_CODE == currMonth//DateTime.Now.Month
                                   select new
                                   {
                                       MAST_MONTH_CODE = item.MAST_MONTH_CODE,
                                       MAST_MONTH_FULL_NAME = item.MAST_MONTH_FULL_NAME
                                   }).ToList();
                    if (query != null)
                    {
                        foreach (var itm in query)
                        {
                            lstItem = new SelectListItem();
                            lstItem.Value = itm.MAST_MONTH_CODE.ToString();
                            lstItem.Text = itm.MAST_MONTH_FULL_NAME.ToString();
                            lstMonths.Add(lstItem);

                        }
                    }
                }
                if (isPopulateFirstItem)
                {
                    lstMonths.Insert(0, (new SelectListItem { Text = "Select Month", Value = "0", Selected = true }));
                }
                return lstMonths;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public List<SelectListItem> PopulateMonthsUptoCurrentMonth(int month)
        {
            try
            {
                List<SelectListItem> lstMonths = new List<SelectListItem>();
                dbContext = new PMGSYEntities();
                //List<SelectListItem> lstMonths = new SelectList(dbContext.MASTER_MONTH, "MAST_MONTH_CODE", "MAST_MONTH_FULL_NAME", month).ToList();
                var query = (from item in dbContext.MASTER_MONTH
                             where item.MAST_MONTH_CODE <= month//DateTime.Now.Month
                             select new
                             {
                                 MAST_MONTH_CODE = item.MAST_MONTH_CODE,
                                 MAST_MONTH_FULL_NAME = item.MAST_MONTH_FULL_NAME
                             }).ToList();
                if (query != null)
                {
                    foreach (var itm in query)
                    {
                        SelectListItem lstItem = new SelectListItem();
                        lstItem.Value = itm.MAST_MONTH_CODE.ToString();
                        lstItem.Text = itm.MAST_MONTH_FULL_NAME.ToString();
                        lstMonths.Add(lstItem);

                    }
                }
                //lstMonths.Insert(0, (new SelectListItem { Text = "Select Month", Value = "0" }));
                return lstMonths;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public List<SelectListItem> PopulateMonths(bool isPopulateFirstItem = true)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<SelectListItem> lstMonths = new SelectList(dbContext.MASTER_MONTH, "MAST_MONTH_CODE", "MAST_MONTH_FULL_NAME").ToList();
                if (isPopulateFirstItem)
                {
                    lstMonths.Insert(0, (new SelectListItem { Text = "Select Month", Value = "0", Selected = true }));
                }
                return lstMonths;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public List<SelectListItem> PopulateMonths(int month)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<SelectListItem> lstMonths = new SelectList(dbContext.MASTER_MONTH, "MAST_MONTH_CODE", "MAST_MONTH_FULL_NAME", month).ToList();
                lstMonths.Insert(0, (new SelectListItem { Text = "Select Month", Value = "0" }));
                return lstMonths;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public List<SelectListItem> PopulateYears(bool isPopulateFirstItem = true)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<SelectListItem> lstYears = new SelectList(dbContext.MASTER_YEAR.Where(m => m.MAST_YEAR_CODE < (DateTime.Now.Year + 1)).OrderByDescending(m => m.MAST_YEAR_CODE), "MAST_YEAR_CODE", "MAST_YEAR_CODE").ToList();
                if (isPopulateFirstItem)
                {
                    lstYears.Insert(0, (new SelectListItem { Text = "Select Year", Value = "0", Selected = true }));
                }
                return lstYears;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public List<SelectListItem> PopulateYears(int year)
        {
            try
            {
                //int _currentYear = DateTime.Now.Year + 1;
                dbContext = new PMGSYEntities();

                //var _yearList = (from item in dbContext.MASTER_YEAR
                //                 where item.MAST_YEAR_CODE < _currentYear
                //                 select new
                //                 {
                //                     YEAR_CODE = item.MAST_YEAR_CODE,
                //                     YEAR_NAME = item.MAST_YEAR_CODE
                //                 }).OrderByDescending(m=>m.YEAR_CODE).ToList();

                //changes by Koustubh Nakate on 26/09/2013 to populate years in descending order  
                List<SelectListItem> lstYears = new SelectList(dbContext.MASTER_YEAR.Where(m => m.MAST_YEAR_CODE < (DateTime.Now.Year + 1)).OrderByDescending(m => m.MAST_YEAR_CODE).ToList(), "MAST_YEAR_CODE", "MAST_YEAR_CODE", year).ToList();
                //List<SelectListItem> lstYears = new SelectList(_yearList, "YEAR_CODE", "YEAR_NAME", year).ToList();
                lstYears.Insert(0, (new SelectListItem { Text = "Select Year", Value = "0" }));
                return lstYears;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public List<SelectListItem> PopulateAllYears(int year)
        {
            try
            {
                //int _currentYear = DateTime.Now.Year + 1;
                dbContext = new PMGSYEntities();

                //var _yearList = (from item in dbContext.MASTER_YEAR
                //                 where item.MAST_YEAR_CODE < _currentYear
                //                 select new
                //                 {
                //                     YEAR_CODE = item.MAST_YEAR_CODE,
                //                     YEAR_NAME = item.MAST_YEAR_CODE
                //                 }).OrderByDescending(m=>m.YEAR_CODE).ToList();

                //changes by Koustubh Nakate on 26/09/2013 to populate years in descending order  
                List<SelectListItem> lstYears = new SelectList(dbContext.MASTER_YEAR.Where(m => m.MAST_YEAR_CODE < (DateTime.Now.Year + 1)).OrderByDescending(m => m.MAST_YEAR_CODE).ToList(), "MAST_YEAR_CODE", "MAST_YEAR_CODE", year).ToList();
                //List<SelectListItem> lstYears = new SelectList(_yearList, "YEAR_CODE", "YEAR_NAME", year).ToList();
                lstYears.Insert(0, (new SelectListItem { Text = "All Years", Value = "0" }));
                return lstYears;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        /// <summary>
        ///  Modified this function to set selected district value on 06/07/2013
        /// </summary>
        /// <param name="StateCode"></param>
        /// <param name="isAllSelected"></param>
        /// <param name="selectedDistrictCode"></param>
        /// <returns></returns>


        public List<SelectListItem> PopulateDistrict(Int32 StateCode, bool isAllSelected = false, Int32 selectedDistrictCode = 0, bool IsPopulateInactiveDistrictsForTOB = false, bool IsPopulateAllActiveInactiveDistricts = false)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<SelectListItem> lstDistrict = null;

                if (IsPopulateAllActiveInactiveDistricts)//Added by Abhishek kamble 5Jan2015 for head 21.01 for asset and lib for state AP 
                {
                    lstDistrict = new SelectList(dbContext.MASTER_DISTRICT.Where(m => m.MAST_STATE_CODE == StateCode).OrderBy(s => s.MAST_DISTRICT_NAME), "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME", selectedDistrictCode).ToList();
                }
                else
                {
                    if (IsPopulateInactiveDistrictsForTOB)
                    {
                        lstDistrict = new SelectList(dbContext.MASTER_DISTRICT.Where(m => m.MAST_STATE_CODE == StateCode /*&& m.MAST_DISTRICT_ACTIVE == "N"*/).OrderBy(s => s.MAST_DISTRICT_NAME), "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME", selectedDistrictCode).ToList();
                    }
                    else
                    {
                        lstDistrict = new SelectList(dbContext.MASTER_DISTRICT.Where(m => m.MAST_STATE_CODE == StateCode && m.MAST_DISTRICT_ACTIVE == "Y").OrderBy(s => s.MAST_DISTRICT_NAME), "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME", selectedDistrictCode).ToList();
                    }

                }
                if (isAllSelected == false)
                {
                    lstDistrict.Insert(0, (new SelectListItem { Text = "Select District", Value = "0", Selected = true }));
                }
                else if (isAllSelected == true)
                {
                    lstDistrict.Insert(0, (new SelectListItem { Text = "All District", Value = "-1", Selected = true }));
                }
                return lstDistrict;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
  
       
        
        public List<SelectListItem> PopulateDistrictForSRRDA(Int32 StateCode, bool isAllSelected = false, Int32 selectedDistrictCode = 0, bool IsPopulateInactiveDistrictsForTOB = false, bool IsPopulateAllActiveInactiveDistricts = false)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<SelectListItem> lstDistrict = null;

                if (IsPopulateAllActiveInactiveDistricts)//Added by Abhishek kamble 5Jan2015 for head 21.01 for asset and lib for state AP 
                {
                    lstDistrict = new SelectList(dbContext.MASTER_DISTRICT.Where(m => m.MAST_STATE_CODE == StateCode).OrderBy(s => s.MAST_DISTRICT_NAME), "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME", selectedDistrictCode).ToList();
                }
                else
                {
                    if (IsPopulateInactiveDistrictsForTOB)
                    {
                        lstDistrict = new SelectList(dbContext.MASTER_DISTRICT.Where(m => m.MAST_STATE_CODE == StateCode && m.MAST_DISTRICT_ACTIVE == "N").OrderBy(s => s.MAST_DISTRICT_NAME), "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME", selectedDistrictCode).ToList();
                    }
                    else
                    {
                        lstDistrict = new SelectList(dbContext.MASTER_DISTRICT.Where(m => m.MAST_STATE_CODE == StateCode && m.MAST_DISTRICT_ACTIVE == "Y").OrderBy(s => s.MAST_DISTRICT_NAME), "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME", selectedDistrictCode).ToList();
                    }

                }
                if (isAllSelected == false)
                {
                    lstDistrict.Insert(0, (new SelectListItem { Text = "Select District", Value = "-1", Selected = true }));
                }
                else if (isAllSelected == true)
                {
                    lstDistrict.Insert(0, (new SelectListItem { Text = "All District", Value = "0", Selected = true }));
                }
                return lstDistrict;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        public List<SelectListItem> GetAllDistrictsByAdminNDCode(int stateCode, int adminCode)
        {
            try
            {

                dbContext = new PMGSYEntities();

                int agencyCode = dbContext.ADMIN_DEPARTMENT.Where(x => x.ADMIN_ND_CODE == adminCode).Select(x => x.MAST_AGENCY_CODE).FirstOrDefault();
                string agencyType = dbContext.MASTER_AGENCY.Where(x => x.MAST_AGENCY_CODE == agencyCode).Select(x => x.MAST_AGENCY_TYPE).FirstOrDefault();

                if (agencyType != "O")
                {
                    var list = dbContext.MASTER_DISTRICT.Where(m => m.MAST_STATE_CODE == stateCode && m.MAST_DISTRICT_ACTIVE == "Y").OrderBy(a => a.MAST_DISTRICT_NAME).ToList();
                    return new SelectList(list.ToList(), "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME").ToList();
                }
                else
                {

                    var list = (from aad in dbContext.ADMIN_AGENCY_DISTRICT
                                join md in dbContext.MASTER_DISTRICT on aad.MAST_DISTRICT_CODE equals md.MAST_DISTRICT_CODE
                                where aad.ADMIN_ND_CODE == adminCode &&
                                       md.MAST_DISTRICT_ACTIVE == "Y"
                                select md).OrderBy(a => a.MAST_DISTRICT_NAME).ToList();


                    return new SelectList(list.ToList(), "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME").ToList();

                }


            }
            catch (Exception ex)
            {
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

        public List<SelectListItem> PopulateMasterTransaction(TransactionParams objParam)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<SelectListItem> lstTransaction = null;
                if (objParam.TXN_ID == 0)
                {
                    lstTransaction = new SelectList(dbContext.ACC_MASTER_TXN.Where(m => m.BILL_TYPE == objParam.BILL_TYPE && m.FUND_TYPE == objParam.FUND_TYPE && m.OP_LVL_ID == objParam.LVL_ID && m.TXN_PARENT_ID == null), "TXN_ID", "TXN_DESC").ToList();
                    lstTransaction.Insert(0, (new SelectListItem { Text = "Select Transaction", Value = "0", Selected = true }));
                }
                else
                {
                    lstTransaction = new SelectList(dbContext.ACC_MASTER_TXN.Where(m => m.BILL_TYPE == objParam.BILL_TYPE && m.FUND_TYPE == objParam.FUND_TYPE && m.OP_LVL_ID == objParam.LVL_ID && m.TXN_PARENT_ID == objParam.TXN_ID && m.CASH_CHQ.Contains(objParam.CASH_CHQ)), "TXN_ID", "TXN_DESC").ToList();
                    lstTransaction.Insert(0, (new SelectListItem { Text = "Select SubTransaction", Value = "0", Selected = true }));
                }
                lstTransaction = lstTransaction.OrderBy(m => m.Text).ToList();
                return lstTransaction;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }

        }

        public List<SelectListItem> PopulateTransactions(TransactionParams objParam)
        {
            try
            {
                dbContext = new PMGSYEntities();

                List<SelectListItem> lstTransaction = new List<SelectListItem>();
                List<ACC_MASTER_TXN> test = new List<ACC_MASTER_TXN>();
                if (objParam.TXN_ID == 0)
                {
                    List<ACC_MASTER_TXN> masterTrans = dbContext.ACC_MASTER_TXN.Where(m => m.BILL_TYPE == objParam.BILL_TYPE && m.FUND_TYPE == objParam.FUND_TYPE && m.OP_LVL_ID == objParam.LVL_ID && m.TXN_PARENT_ID == null && m.IS_OPERATIONAL == true).OrderBy(m => m.TXN_ORDER).ToList<ACC_MASTER_TXN>();

                    foreach (ACC_MASTER_TXN item in masterTrans)
                    {
                        if (item.CASH_CHQ == null)
                        {
                            lstTransaction.Add(new SelectListItem { Value = item.TXN_ID.ToString().Trim(), Text = item.TXN_DESC });
                        }
                        else
                        {
                            lstTransaction.Add(new SelectListItem { Value = item.TXN_ID.ToString().Trim() + "$" + item.CASH_CHQ.ToString().Trim(), Text = item.TXN_DESC });
                        }
                    }
                    //lstTransaction = lstTransaction.OrderBy(m => m.Text).ToList();
                    lstTransaction.Insert(0, (new SelectListItem { Text = "Select Transaction", Value = "", Selected = true }));
                    return lstTransaction;
                }
                else
                {
                    List<ACC_MASTER_TXN> masterTrans = new List<ACC_MASTER_TXN>();
                    if (objParam.CASH_CHQ != null)
                    {
                        //masterTrans = dbContext.ACC_MASTER_TXN.Where(m => m.BILL_TYPE == objParam.BILL_TYPE && m.FUND_TYPE == objParam.FUND_TYPE && m.OP_LVL_ID == objParam.LVL_ID && m.TXN_PARENT_ID == objParam.TXN_ID && m.CASH_CHQ.Contains(objParam.CASH_CHQ) && (objParam.OP_MODE == "A" ? m.IS_OPERATIONAL : true) == (objParam.OP_MODE == "A" ? true : true)).OrderBy(m => m.TXN_ORDER).ToList<ACC_MASTER_TXN>(); //commented by Vikram for removing the non operational heads
                        //new change done by Vikram
                        masterTrans = dbContext.ACC_MASTER_TXN.Where(m => m.BILL_TYPE == objParam.BILL_TYPE && m.FUND_TYPE == objParam.FUND_TYPE && m.OP_LVL_ID == objParam.LVL_ID && m.TXN_PARENT_ID == objParam.TXN_ID && m.CASH_CHQ.Contains(objParam.CASH_CHQ) && (objParam.OP_MODE == "A" ? m.IS_OPERATIONAL : true) == (objParam.OP_MODE == "A" ? true : true)).OrderBy(m => m.TXN_ORDER).ToList<ACC_MASTER_TXN>();

                        if (objParam.OP_MODE == "E")
                        {
                            if (dbContext.ACC_BILL_MASTER.Any(m => m.BILL_ID == objParam.BILL_ID && m.ACTION_REQUIRED != "N"))
                            {
                                masterTrans = dbContext.ACC_MASTER_TXN.Where(m => m.BILL_TYPE == objParam.BILL_TYPE && m.FUND_TYPE == objParam.FUND_TYPE && m.OP_LVL_ID == objParam.LVL_ID && m.TXN_PARENT_ID == objParam.TXN_ID && m.CASH_CHQ.Contains(objParam.CASH_CHQ) && m.IS_REQ_AFTER_PORTING == true).OrderBy(m => m.TXN_ORDER).ToList<ACC_MASTER_TXN>();
                                //masterTrans = masterTrans.Where(m => m.IS_OPERATIONAL == true).ToList();
                                masterTrans = masterTrans.Where(m => m.IS_REQ_AFTER_PORTING == true).ToList();
                            }
                            else
                            {
                                masterTrans = dbContext.ACC_MASTER_TXN.Where(m => m.BILL_TYPE == objParam.BILL_TYPE && m.FUND_TYPE == objParam.FUND_TYPE && m.OP_LVL_ID == objParam.LVL_ID && m.TXN_PARENT_ID == objParam.TXN_ID && m.CASH_CHQ.Contains(objParam.CASH_CHQ) && m.IS_OPERATIONAL == true).OrderBy(m => m.TXN_ORDER).ToList<ACC_MASTER_TXN>();
                            }
                        }
                        //end of change
                    }
                    else
                    {
                        //masterTrans = dbContext.ACC_MASTER_TXN.Where(m => m.BILL_TYPE == objParam.BILL_TYPE && m.FUND_TYPE == objParam.FUND_TYPE && m.OP_LVL_ID == objParam.LVL_ID && m.TXN_PARENT_ID == objParam.TXN_ID && (objParam.OP_MODE == "A" ? m.IS_OPERATIONAL : true) == (objParam.OP_MODE == "A" ? true : true)).OrderBy(m => m.TXN_ORDER).ToList<ACC_MASTER_TXN>();
                        //new change done by Vikram on 24-09-2013
                        masterTrans = dbContext.ACC_MASTER_TXN.Where(m => m.BILL_TYPE == objParam.BILL_TYPE && m.FUND_TYPE == objParam.FUND_TYPE && m.OP_LVL_ID == objParam.LVL_ID && m.TXN_PARENT_ID == objParam.TXN_ID && ((m.IS_OPERATIONAL)) == (objParam.OP_MODE == "A" ? true : true)).OrderBy(m => m.TXN_ORDER).ToList<ACC_MASTER_TXN>();
                        if (objParam.OP_MODE == "E")
                        {
                            if (dbContext.ACC_BILL_MASTER.Any(m => m.BILL_ID == objParam.BILL_ID && m.ACTION_REQUIRED != "N"))
                            {
                                masterTrans = dbContext.ACC_MASTER_TXN.Where(m => m.BILL_TYPE == objParam.BILL_TYPE && m.FUND_TYPE == objParam.FUND_TYPE && m.OP_LVL_ID == objParam.LVL_ID && m.TXN_PARENT_ID == objParam.TXN_ID && ((m.IS_REQ_AFTER_PORTING)) == (true)).OrderBy(m => m.TXN_ORDER).ToList<ACC_MASTER_TXN>();
                                //masterTrans = masterTrans.Where(m => m.IS_OPERATIONAL == true).ToList();
                                masterTrans = masterTrans.Where(m => m.IS_REQ_AFTER_PORTING == true).ToList();
                            }
                        }

                        //end of change
                    }
                    if (masterTrans == null || masterTrans.Count == 0)
                    {
                        lstTransaction.Insert(0, (new SelectListItem { Text = "Select SubTransaction", Value = "0", Selected = true }));
                        return lstTransaction;
                    }

                    foreach (ACC_MASTER_TXN item in masterTrans)
                    {
                        if (item.CASH_CHQ == null)
                        {
                            if (item.IS_REQ_AFTER_PORTING == true)
                            {
                                lstTransaction.Add(new SelectListItem { Value = item.TXN_ID.ToString().Trim(), Text = item.TXN_DESC });
                            }
                            else
                            {
                                lstTransaction.Add(new SelectListItem { Value = item.TXN_ID.ToString().Trim(), Text = "$" + item.TXN_DESC });
                            }
                        }
                        else
                        {
                            if (item.IS_REQ_AFTER_PORTING == true)
                            {
                                lstTransaction.Add(new SelectListItem { Value = item.TXN_ID.ToString().Trim() + "$" + item.CASH_CHQ.ToString().Trim(), Text = item.TXN_DESC });
                            }
                            else
                            {
                                lstTransaction.Add(new SelectListItem { Value = item.TXN_ID.ToString().Trim() + "$" + item.CASH_CHQ.ToString().Trim(), Text = "$" + item.TXN_DESC });
                            }
                        }
                    }

                    lstTransaction.Insert(0, (new SelectListItem { Text = "Select SubTransaction", Value = "", Selected = true }));
                    return lstTransaction;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// function to check if head selected is corrrect or not
        /// case 1) is_required_after porting =1 and is_operational =1 => correct entry
        /// case 2) is_required_after porting =0 and is_operational =0 => wrong entry Cant be edited/finalized
        /// case 3) is_required_after porting =0 and is_operational =0 => wrong entry Can be edited/finalized after correction 
        /// </summary>
        /// <param name="txn_id"> transaction id of the entry to be checked should be zerp when finalization operation</param>
        /// <param name="bill_id">bill id of the transaction should be zero when add or edit operation </param>
        /// <param name="operation"> E for Edit/A => Add  F for Finalize </param>
        /// <returns></returns>
        public String ValidateHeadForCorrection(int txn_id, long bill_id, string operation)
        {
            try
            {
                dbContext = new PMGSYEntities();

                if (txn_id == 0 || txn_id == null)
                {
                    return "1";
                }
                //if  operation  is add or edit check for transaction only 
                if (operation == "E" || operation == "A")
                {
                    bool operational = dbContext.ACC_TXN_HEAD_MAPPING.Where(x => x.TXN_ID == txn_id).Select(x => x.IS_OPERATIONAL).FirstOrDefault();
                    if (operational)
                    {
                        return "1"; //ok  head is operational 
                    }
                    else //not operation
                    {
                        //get if  is_required_after porting 
                        bool isRequiredAfterPorting = dbContext.ACC_TXN_HEAD_MAPPING.Where(x => x.TXN_ID == txn_id).Select(x => x.IS_REQ_AFTER_PORTING).FirstOrDefault();

                        //check if if  is_required_after porting 
                        if (!isRequiredAfterPorting)
                        {
                            //if after porting it is not required; dont allow any operation
                            return "0";
                        }
                        else
                        {

                            if (operation == "E")
                            {
                                return "1"; //allow edit operation
                            }
                            else
                            {
                                return "0"; //dont allow add operation
                            }
                        }

                    }
                }
                else  //if operation is Finalization then
                {

                    //get all transaction id for the bill id selected
                    List<short> txnlist = new List<short>();

                    txnlist = dbContext.ACC_BILL_DETAILS.Where(x => x.BILL_ID == bill_id && x.CREDIT_DEBIT == "C" && x.TXN_ID != null).Select(x => x.TXN_ID.Value).ToList<short>();

                    //for each txn id in the list check 

                    bool isValidEntry = true;

                    foreach (short txnId in txnlist)
                    {

                        bool operational = dbContext.ACC_TXN_HEAD_MAPPING.Where(x => x.TXN_ID == txnId).Select(x => x.IS_OPERATIONAL).FirstOrDefault();

                        if (!operational)
                        {
                            //get if  is_required_after porting 
                            bool isRequiredAfterPorting = dbContext.ACC_TXN_HEAD_MAPPING.Where(x => x.TXN_ID == txnId).Select(x => x.IS_REQ_AFTER_PORTING).FirstOrDefault();

                            //check if if  is_required_after porting 
                            if (!isRequiredAfterPorting)
                            {
                                //if after porting it is not required; dont allow any operation
                                isValidEntry = false;
                                break;
                            }

                        }
                    }

                    if (isValidEntry)
                    {
                        return "1";
                    }
                    else
                    {
                        return "0";
                    }


                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// function to populate only authorization transaction
        /// </summary>
        /// <param name="objParam"></param>
        /// <returns></returns>
        public List<SelectListItem> PopulateAuthorizationTransaction(TransactionParams objParam)
        {

            try
            {
                dbContext = new PMGSYEntities();

                List<SelectListItem> lstTransaction = new List<SelectListItem>();
                List<ACC_MASTER_TXN> test = new List<ACC_MASTER_TXN>();
                if (objParam.TXN_ID == 0)
                {
                    var masterTrans =

                        (from master in dbContext.ACC_MASTER_TXN
                         join details in dbContext.ACC_SCREEN_DESIGN_PARAM_MASTER
                         on master.TXN_ID equals details.TXN_ID
                         where master.BILL_TYPE == objParam.BILL_TYPE
                          && master.FUND_TYPE == objParam.FUND_TYPE
                          && master.OP_LVL_ID == objParam.LVL_ID
                          && master.TXN_PARENT_ID == null
                          && master.IS_OPERATIONAL == true
                          && details.BAR_REQ == "Y"
                         select new
                         {
                             master.CASH_CHQ,
                             master.TXN_ID,
                             master.TXN_DESC

                         });


                    foreach (var item in masterTrans)
                    {
                        if (item.CASH_CHQ == null)
                        {
                            lstTransaction.Add(new SelectListItem { Value = item.TXN_ID.ToString().Trim(), Text = item.TXN_DESC });
                        }
                        else
                        {
                            lstTransaction.Add(new SelectListItem { Value = item.TXN_ID.ToString().Trim() + "$" + item.CASH_CHQ.ToString().Trim(), Text = item.TXN_DESC });
                        }
                    }
                    //lstTransaction = lstTransaction.OrderBy(m => m.Text).ToList();
                    lstTransaction.Insert(0, (new SelectListItem { Text = "Select Transaction", Value = "", Selected = true }));
                    return lstTransaction;
                }
                else
                {
                    List<ACC_MASTER_TXN> masterTrans = new List<ACC_MASTER_TXN>();
                    if (objParam.CASH_CHQ != null)
                    {
                        masterTrans = dbContext.ACC_MASTER_TXN.Where(m => m.BILL_TYPE == objParam.BILL_TYPE && m.FUND_TYPE == objParam.FUND_TYPE && m.OP_LVL_ID == objParam.LVL_ID && m.TXN_PARENT_ID == objParam.TXN_ID && m.CASH_CHQ.Contains(objParam.CASH_CHQ) && (objParam.OP_MODE == "A" ? m.IS_OPERATIONAL : true) == (objParam.OP_MODE == "A" ? true : true)).OrderBy(m => m.TXN_ORDER).ToList<ACC_MASTER_TXN>();
                    }
                    else
                    {
                        masterTrans = dbContext.ACC_MASTER_TXN.Where(m => m.BILL_TYPE == objParam.BILL_TYPE && m.FUND_TYPE == objParam.FUND_TYPE && m.OP_LVL_ID == objParam.LVL_ID && m.TXN_PARENT_ID == objParam.TXN_ID && (objParam.OP_MODE == "A" ? m.IS_OPERATIONAL : true) == (objParam.OP_MODE == "A" ? true : true)).OrderBy(m => m.TXN_ORDER).ToList<ACC_MASTER_TXN>();
                    }
                    if (masterTrans == null || masterTrans.Count == 0)
                    {
                        lstTransaction.Insert(0, (new SelectListItem { Text = "Select SubTransaction", Value = "0", Selected = true }));
                        return lstTransaction;
                    }

                    foreach (ACC_MASTER_TXN item in masterTrans)
                    {
                        if (item.CASH_CHQ == null)
                        {
                            if (item.IS_REQ_AFTER_PORTING == true)
                            {
                                lstTransaction.Add(new SelectListItem { Value = item.TXN_ID.ToString().Trim(), Text = item.TXN_DESC });
                            }
                            else
                            {
                                lstTransaction.Add(new SelectListItem { Value = item.TXN_ID.ToString().Trim(), Text = "$" + item.TXN_DESC });
                            }
                        }
                        else
                        {
                            if (item.IS_REQ_AFTER_PORTING == true)
                            {
                                lstTransaction.Add(new SelectListItem { Value = item.TXN_ID.ToString().Trim() + "$" + item.CASH_CHQ.ToString().Trim(), Text = item.TXN_DESC });
                            }
                            else
                            {
                                lstTransaction.Add(new SelectListItem { Value = item.TXN_ID.ToString().Trim() + "$" + item.CASH_CHQ.ToString().Trim(), Text = "$" + item.TXN_DESC });
                            }
                        }
                    }

                    lstTransaction.Insert(0, (new SelectListItem { Text = "Select SubTransaction", Value = "", Selected = true }));
                    return lstTransaction;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }

        }

        public DateTime GetStringToDateTime(string strDate)
        {
            string[] formats = { "dd/MM/yyyy" };
            return DateTime.ParseExact(strDate, formats, new CultureInfo("en-US"), DateTimeStyles.None);
        }

        public String GetDateTimeToString(DateTime date)
        {
            return date.ToString("dd/MM/yyyy");
        }

        #region old contractor supllier function
        /*  public List<SelectListItem> PopulateContractorSupplier(TransactionParams objParam)
        {
            try
            {
                dbContext = new PMGSYEntities();
                if (String.IsNullOrEmpty(objParam.MAST_CON_SUP_FLAG))
                {
                    objParam.MAST_CON_SUP_FLAG = "C";
                }
                List<SelectListItem> lstContDetails = new List<SelectListItem>();
               
                //populate for state
                if (objParam.DISTRICT_CODE == 0)
                {
                    //for maintanace fund
                    if (PMGSYSession.Current.FundType.ToLower() == "m")
                    {
                        var lstContractor = (from sancprj in dbContext.IMS_SANCTIONED_PROJECTS
                                             join mancon in dbContext.MANE_IMS_CONTRACT on sancprj.IMS_PR_ROAD_CODE equals mancon.IMS_PR_ROAD_CODE
                                             join con in dbContext.MASTER_CONTRACTOR on mancon.MAST_CON_ID equals con.MAST_CON_ID
                                             where sancprj.MAST_STATE_CODE == objParam.STATE_CODE && con.MAST_CON_SUP_FLAG == objParam.MAST_CON_SUP_FLAG
                                             orderby con.MAST_CON_COMPANY_NAME ascending
                                             select new
                                             {
                                                 MAST_CON_COMPANY_NAME = con.MAST_CON_COMPANY_NAME,
                                                 MAST_CON_ID = con.MAST_CON_ID
                                             }).ToList().Distinct();
                                            
                        if (lstContractor == null || lstContractor.Count() == 0)
                        {
                            if (objParam.MAST_CON_SUP_FLAG == "C")
                            {
                                lstContDetails.Insert(0, (new SelectListItem { Text = "Select Contractor", Value = "0", Selected = true }));
                            }
                            else
                            {
                                lstContDetails.Insert(0, (new SelectListItem { Text = "Select Supplier", Value = "0", Selected = true }));
                            }
                            return lstContDetails;
                        }
                        else
                        {
                            foreach (var item in lstContractor)
                            {
                                if (item.MAST_CON_ID == objParam.MAST_CONT_ID)
                                {
                                    lstContDetails.Add(new SelectListItem { Text = item.MAST_CON_COMPANY_NAME, Value = item.MAST_CON_ID.ToString(), Selected = true });
                                }
                                else
                                {
                                    lstContDetails.Add(new SelectListItem { Text = item.MAST_CON_COMPANY_NAME, Value = item.MAST_CON_ID.ToString() });
                                }
                            }
                        }
                        lstContDetails = lstContDetails.OrderBy(m => m.Text).ToList();
                        if (objParam.MAST_CON_SUP_FLAG == "C")
                        {
                            lstContDetails.Insert(0, (new SelectListItem { Text = "Select Contractor", Value = "0" }));
                        }
                        else
                        {
                            lstContDetails.Insert(0, (new SelectListItem { Text = "Select Supplier", Value = "0" }));
                        }
                    }
                    else
                    {
                        var lstContractor = (from sancprj in dbContext.IMS_SANCTIONED_PROJECTS
                                             join aggdet in dbContext.TEND_AGREEMENT_DETAIL on sancprj.IMS_PR_ROAD_CODE equals aggdet.IMS_PR_ROAD_CODE
                                             join aggmast in dbContext.TEND_AGREEMENT_MASTER on aggdet.TEND_AGREEMENT_CODE equals aggmast.TEND_AGREEMENT_CODE
                                             join con in dbContext.MASTER_CONTRACTOR on aggmast.MAST_CON_ID equals con.MAST_CON_ID
                                             where sancprj.MAST_STATE_CODE == objParam.STATE_CODE && con.MAST_CON_SUP_FLAG == objParam.MAST_CON_SUP_FLAG
                                             orderby con.MAST_CON_COMPANY_NAME ascending
                                             select new
                                             {
                                                 MAST_CON_COMPANY_NAME = con.MAST_CON_COMPANY_NAME,
                                                 MAST_CON_ID = con.MAST_CON_ID
                                             }).ToList().Distinct();

                        // get the DPR list and concat it with contractor Change date 13/08/2013

                        var lstDrp = (from tm in dbContext.TEND_AGREEMENT_MASTER
                                      join cn in dbContext.MASTER_CONTRACTOR on  tm.MAST_CON_ID equals cn.MAST_CON_ID
                                      where
                                     // tm.MAST_DISTRICT_CODE == 441
                                      tm.MAST_STATE_CODE == objParam.STATE_CODE
                                      &&  tm.TEND_AGREEMENT_TYPE =="D"
                                        orderby cn.MAST_CON_COMPANY_NAME ascending   
                                        select new
                                             {
                                                 MAST_CON_COMPANY_NAME = cn.MAST_CON_COMPANY_NAME,
                                                 MAST_CON_ID = cn.MAST_CON_ID
                                             }).ToList().Distinct();

                        if (lstDrp != null && lstDrp.Count() != 0)
                        {
                            lstContractor = lstContractor.Concat(lstDrp);
                        }

                        if (lstContractor == null || lstContractor.Count() == 0)
                        {
                            if (objParam.MAST_CON_SUP_FLAG == "C")
                            {
                                lstContDetails.Insert(0, (new SelectListItem { Text = "Select Contractor", Value = "0", Selected = true }));
                            }
                            else
                            {
                                lstContDetails.Insert(0, (new SelectListItem { Text = "Select Supplier", Value = "0", Selected = true }));
                            }
                            return lstContDetails;
                        }
                        else
                        {
                            foreach (var item in lstContractor)
                            {
                                if (item.MAST_CON_ID == objParam.MAST_CONT_ID)
                                {
                                    lstContDetails.Add(new SelectListItem { Text = item.MAST_CON_COMPANY_NAME, Value = item.MAST_CON_ID.ToString(), Selected = true });
                                }
                                else
                                {
                                    lstContDetails.Add(new SelectListItem { Text = item.MAST_CON_COMPANY_NAME, Value = item.MAST_CON_ID.ToString() });
                                }
                            }
                        }
                        lstContDetails = lstContDetails.OrderBy(m => m.Text).ToList();
                        if (objParam.MAST_CON_SUP_FLAG == "C")
                        {
                            lstContDetails.Insert(0, (new SelectListItem { Text = "Select Contractor", Value = "0" }));
                        }
                        else
                        {
                            lstContDetails.Insert(0, (new SelectListItem { Text = "Select Supplier", Value = "0" }));
                        }
                    }                    
                }
                else
                {
                    if (PMGSYSession.Current.FundType.ToLower() == "m")
                    {
                        var lstContractor = (from sancprj in dbContext.IMS_SANCTIONED_PROJECTS
                                             join mancon in dbContext.MANE_IMS_CONTRACT on sancprj.IMS_PR_ROAD_CODE equals mancon.IMS_PR_ROAD_CODE
                                             join con in dbContext.MASTER_CONTRACTOR on mancon.MAST_CON_ID equals con.MAST_CON_ID
                                             where sancprj.MAST_DISTRICT_CODE == objParam.DISTRICT_CODE && con.MAST_CON_SUP_FLAG == objParam.MAST_CON_SUP_FLAG
                                             select new
                                             {
                                                 MAST_CON_COMPANY_NAME = con.MAST_CON_COMPANY_NAME,
                                                 MAST_CON_ID = con.MAST_CON_ID
                                             }
                                            ).ToList().Distinct();
                        if (lstContractor == null || lstContractor.Count() == 0)
                        {
                            if (objParam.MAST_CON_SUP_FLAG == "C")
                            {
                                lstContDetails.Insert(0, (new SelectListItem { Text = "Select Contractor", Value = "0", Selected = true }));
                            }
                            else
                            {
                                lstContDetails.Insert(0, (new SelectListItem { Text = "Select Supplier", Value = "0", Selected = true }));
                            }
                            return lstContDetails;
                        }
                        else
                        {
                            foreach (var item in lstContractor)
                            {
                                if (item.MAST_CON_ID == objParam.MAST_CONT_ID)
                                {
                                    lstContDetails.Add(new SelectListItem { Text = item.MAST_CON_COMPANY_NAME, Value = item.MAST_CON_ID.ToString(), Selected = true });
                                }
                                else
                                {
                                    lstContDetails.Add(new SelectListItem { Text = item.MAST_CON_COMPANY_NAME, Value = item.MAST_CON_ID.ToString() });
                                }
                            }
                        }
                        lstContDetails = lstContDetails.OrderBy(m => m.Text).ToList();
                        if (objParam.MAST_CON_SUP_FLAG == "C")
                        {
                            lstContDetails.Insert(0, (new SelectListItem { Text = "Select Contractor", Value = "0" }));
                        }
                        else
                        {
                            lstContDetails.Insert(0, (new SelectListItem { Text = "Select Supplier", Value = "0" }));
                        }
                    }
                    else
                    {
                        var lstContractor = (from sancprj in dbContext.IMS_SANCTIONED_PROJECTS
                                             join aggdet in dbContext.TEND_AGREEMENT_DETAIL on sancprj.IMS_PR_ROAD_CODE equals aggdet.IMS_PR_ROAD_CODE
                                             join aggmast in dbContext.TEND_AGREEMENT_MASTER on aggdet.TEND_AGREEMENT_CODE equals aggmast.TEND_AGREEMENT_CODE
                                             join con in dbContext.MASTER_CONTRACTOR on aggmast.MAST_CON_ID equals con.MAST_CON_ID
                                             where sancprj.MAST_DISTRICT_CODE == objParam.DISTRICT_CODE && con.MAST_CON_SUP_FLAG == objParam.MAST_CON_SUP_FLAG
                                             select new
                                             {
                                                 MAST_CON_COMPANY_NAME = con.MAST_CON_COMPANY_NAME,
                                                 MAST_CON_ID = con.MAST_CON_ID
                                             }
                                            ).ToList().Distinct();

                        // get the DPR list and concat it with contractor Change date 13/08/2013

                        var lstDrp = (from tm in dbContext.TEND_AGREEMENT_MASTER
                                      join cn in dbContext.MASTER_CONTRACTOR on tm.MAST_CON_ID equals cn.MAST_CON_ID
                                      where tm.MAST_DISTRICT_CODE == objParam.DISTRICT_CODE  && tm.TEND_AGREEMENT_TYPE == "D"
                                      orderby cn.MAST_CON_COMPANY_NAME ascending
                                      select new
                                      {
                                          MAST_CON_COMPANY_NAME = cn.MAST_CON_COMPANY_NAME,
                                          MAST_CON_ID = cn.MAST_CON_ID
                                      }).ToList().Distinct();

                        if (lstDrp != null && lstDrp.Count() != 0)
                        {
                             lstContractor = lstContractor.Concat(lstDrp);
                        }


                        if (lstContractor == null || lstContractor.Count() == 0)
                        {
                            if (objParam.MAST_CON_SUP_FLAG == "C")
                            {
                                lstContDetails.Insert(0, (new SelectListItem { Text = "Select Contractor", Value = "0", Selected = true }));
                            }
                            else
                            {
                                lstContDetails.Insert(0, (new SelectListItem { Text = "Select Supplier", Value = "0", Selected = true }));
                            }
                            return lstContDetails;
                        }
                        else
                        {
                            foreach (var item in lstContractor)
                            {
                                if (item.MAST_CON_ID == objParam.MAST_CONT_ID)
                                {
                                    lstContDetails.Add(new SelectListItem { Text = item.MAST_CON_COMPANY_NAME, Value = item.MAST_CON_ID.ToString(), Selected = true });
                                }
                                else
                                {
                                    lstContDetails.Add(new SelectListItem { Text = item.MAST_CON_COMPANY_NAME, Value = item.MAST_CON_ID.ToString() });
                                }
                            }
                            lstContDetails = lstContDetails.OrderBy(m => m.Text).ToList();
                            if (objParam.MAST_CON_SUP_FLAG == "C")
                            {
                                lstContDetails.Insert(0, (new SelectListItem { Text = "Select Contractor", Value = "0" }));
                            }
                            else
                            {
                                lstContDetails.Insert(0, (new SelectListItem { Text = "Select Supplier", Value = "0" }));
                            }
                        }
                    }
                }

                return lstContDetails;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        */
        #endregion

        //public List<SelectListItem> PopulateContractorSupplier(TransactionParams objParam)
        //{
        //    try
        //    {
        //        dbContext = new PMGSYEntities();
        //        if (String.IsNullOrEmpty(objParam.MAST_CON_SUP_FLAG))
        //        {
        //            objParam.MAST_CON_SUP_FLAG = "C";
        //        }
        //        List<SelectListItem> lstContDetails = new List<SelectListItem>();

        //        //populate for state
        //        if (objParam.DISTRICT_CODE == 0)
        //        {
        //            //for maintanace fund
        //            if (PMGSYSession.Current.FundType.ToLower() == "m")
        //            {
        //                //change sby Koustubh Nakate on 03/10/2013 to populate supplier properly 

        //                var lstContractor = objParam.MAST_CON_SUP_FLAG == "C" ? (from sancprj in dbContext.IMS_SANCTIONED_PROJECTS
        //                                                                         join mancon in dbContext.MANE_IMS_CONTRACT on sancprj.IMS_PR_ROAD_CODE equals mancon.IMS_PR_ROAD_CODE
        //                                                                         join con in dbContext.MASTER_CONTRACTOR on mancon.MAST_CON_ID equals con.MAST_CON_ID
        //                                                                         where sancprj.MAST_STATE_CODE == objParam.STATE_CODE
        //                                                                         // && con.MAST_CON_SUP_FLAG == objParam.MAST_CON_SUP_FLAG
        //                                                                         orderby con.MAST_CON_COMPANY_NAME ascending
        //                                                                         select new
        //                                                                         {
        //                                                                             MAST_CON_COMPANY_NAME = con.MAST_CON_SUP_FLAG == "C" ?
        //                                                                            "CON- " + "(" + SqlFunctions.StringConvert((decimal)con.MAST_CON_ID).Trim() + ") "
        //                                                                               + ((con.MAST_CON_FNAME == null ? string.Empty : con.MAST_CON_FNAME) + " " + (con.MAST_CON_MNAME == null ? string.Empty : con.MAST_CON_MNAME) + " " + (con.MAST_CON_LNAME == null ? string.Empty : con.MAST_CON_LNAME))
        //                                                                               + "( " + con.MAST_CON_COMPANY_NAME + ")" + "PAN( " + (con.MAST_CON_PAN == null ? "-" : con.MAST_CON_PAN) + " )"
        //                                                                               : "SUP- " + "(" + SqlFunctions.StringConvert((decimal)con.MAST_CON_ID).Trim() + ") "
        //                                                                                + ((con.MAST_CON_FNAME == null ? string.Empty : con.MAST_CON_FNAME) + " " + (con.MAST_CON_MNAME == null ? string.Empty : con.MAST_CON_MNAME) + " " + (con.MAST_CON_LNAME == null ? string.Empty : con.MAST_CON_LNAME))
        //                                                                              + "( " + con.MAST_CON_COMPANY_NAME + ")" + "TAN( " + (con.MAST_CON_PAN == null ? "-" : con.MAST_CON_PAN) + " )",
        //                                                                             MAST_CON_ID = con.MAST_CON_ID,
        //                                                                             MAST_CON_SUP_FLAG = con.MAST_CON_SUP_FLAG
        //                                                                         }).ToList().Distinct() : (from aggmast in dbContext.TEND_AGREEMENT_MASTER
        //                                                                                                   join conmast in dbContext.MASTER_CONTRACTOR
        //                                                                                                   on aggmast.MAST_CON_ID equals conmast.MAST_CON_ID
        //                                                                                                   where
        //                                                                                                  aggmast.TEND_AGREEMENT_TYPE == "S" &&
        //                                                                                                  aggmast.MAST_STATE_CODE == objParam.STATE_CODE
        //                                                                                                   select new
        //                                                                                                   {
        //                                                                                                       MAST_CON_COMPANY_NAME = "SUP- " + "(" + SqlFunctions.StringConvert((decimal)conmast.MAST_CON_ID).Trim() + ") " + ((conmast.MAST_CON_FNAME == null ? string.Empty : conmast.MAST_CON_FNAME) + " " + (conmast.MAST_CON_MNAME == null ? string.Empty : conmast.MAST_CON_MNAME) + " " + (conmast.MAST_CON_LNAME == null ? string.Empty : conmast.MAST_CON_LNAME)) + "( " + conmast.MAST_CON_COMPANY_NAME + ")" + " " + "TAN( " + (conmast.MAST_CON_PAN == null ? "-" : conmast.MAST_CON_PAN) + " )",
        //                                                                                                       MAST_CON_ID = conmast.MAST_CON_ID,
        //                                                                                                       MAST_CON_SUP_FLAG = conmast.MAST_CON_SUP_FLAG
        //                                                                                                   }
        //                                                                                                       ).ToList().Distinct();

        //                //addition by Koustubh Nakate on 29/08/2013 to add proposal contractors from shifted tracking

        //                var lstContractorByPropasalTracking = (from propTracking in dbContext.IMS_PROPOSAL_TRACKING
        //                                                       join mancon in dbContext.MANE_IMS_CONTRACT
        //                                                       on propTracking.IMS_PR_ROAD_CODE equals mancon.IMS_PR_ROAD_CODE
        //                                                       join con in dbContext.MASTER_CONTRACTOR
        //                                                       on mancon.MAST_CON_ID equals con.MAST_CON_ID
        //                                                       where propTracking.MAST_STATE_CODE == objParam.STATE_CODE
        //                                                       orderby con.MAST_CON_COMPANY_NAME ascending
        //                                                       select new
        //                                                       {
        //                                                           MAST_CON_COMPANY_NAME = con.MAST_CON_SUP_FLAG == "C" ?
        //                                                          "CON- " + "(" + SqlFunctions.StringConvert((decimal)con.MAST_CON_ID).Trim() + ") "
        //                                                             + ((con.MAST_CON_FNAME == null ? string.Empty : con.MAST_CON_FNAME) + " " + (con.MAST_CON_MNAME == null ? string.Empty : con.MAST_CON_MNAME) + " " + (con.MAST_CON_LNAME == null ? string.Empty : con.MAST_CON_LNAME))
        //                                                             + "( " + con.MAST_CON_COMPANY_NAME + ")" + "PAN( " + (con.MAST_CON_PAN == null ? "-" : con.MAST_CON_PAN) + " )"
        //                                                             : "SUP- " + "(" + SqlFunctions.StringConvert((decimal)con.MAST_CON_ID).Trim() + ") "
        //                                                              + ((con.MAST_CON_FNAME == null ? string.Empty : con.MAST_CON_FNAME) + " " + (con.MAST_CON_MNAME == null ? string.Empty : con.MAST_CON_MNAME) + " " + (con.MAST_CON_LNAME == null ? string.Empty : con.MAST_CON_LNAME))
        //                                                            + "( " + con.MAST_CON_COMPANY_NAME + ")" + "TAN( " + (con.MAST_CON_PAN == null ? "-" : con.MAST_CON_PAN) + " )",
        //                                                           MAST_CON_ID = con.MAST_CON_ID,
        //                                                           MAST_CON_SUP_FLAG = con.MAST_CON_SUP_FLAG
        //                                                       }).ToList().Distinct();


        //                lstContractor = lstContractor.Union(lstContractorByPropasalTracking).ToList().Distinct();



        //                if (objParam.MAST_CON_SUP_FLAG == "C")
        //                {

        //                    lstContractor = lstContractor.Where(x => x.MAST_CON_SUP_FLAG == "C" || x.MAST_CON_SUP_FLAG == "R");

        //                }

        //                if (objParam.MAST_CON_SUP_FLAG == "S")
        //                {

        //                    lstContractor = lstContractor.Where(x => x.MAST_CON_SUP_FLAG == "S");

        //                }


        //                if (lstContractor == null || lstContractor.Count() == 0)
        //                {
        //                    if (objParam.MAST_CON_SUP_FLAG == "C")
        //                    {
        //                        lstContDetails.Insert(0, (new SelectListItem { Text = "--Select--", Value = "0", Selected = true }));
        //                    }
        //                    else
        //                    {
        //                        lstContDetails.Insert(0, (new SelectListItem { Text = "Select Supplier", Value = "0", Selected = true }));
        //                    }
        //                    return lstContDetails;
        //                }
        //                else
        //                {
        //                    foreach (var item in lstContractor)
        //                    {
        //                        if (item.MAST_CON_ID == objParam.MAST_CONT_ID)
        //                        {
        //                            lstContDetails.Add(new SelectListItem { Text = item.MAST_CON_COMPANY_NAME, Value = item.MAST_CON_ID.ToString(), Selected = true });
        //                        }
        //                        else
        //                        {
        //                            lstContDetails.Add(new SelectListItem { Text = item.MAST_CON_COMPANY_NAME, Value = item.MAST_CON_ID.ToString() });
        //                        }
        //                    }
        //                }
        //                lstContDetails = lstContDetails.OrderBy(m => m.Text).ToList();
        //                if (objParam.MAST_CON_SUP_FLAG == "C")
        //                {
        //                    lstContDetails.Insert(0, (new SelectListItem { Text = "--Select--", Value = "0" }));
        //                }
        //                else
        //                {
        //                    lstContDetails.Insert(0, (new SelectListItem { Text = "Select Supplier", Value = "0" }));
        //                }
        //            }
        //            else if (PMGSYSession.Current.FundType.ToLower() == "p") //Condition Added on 18-11-2021
        //            {
        //                //changes by Koustubh Nakate on 28/09/2013 to populate supplier properly 

        //                var lstContractor = objParam.MAST_CON_SUP_FLAG == "C" ? (from sancprj in dbContext.IMS_SANCTIONED_PROJECTS
        //                                                                         join aggdet in dbContext.TEND_AGREEMENT_DETAIL on sancprj.IMS_PR_ROAD_CODE equals aggdet.IMS_PR_ROAD_CODE
        //                                                                         join aggmast in dbContext.TEND_AGREEMENT_MASTER on aggdet.TEND_AGREEMENT_CODE equals aggmast.TEND_AGREEMENT_CODE
        //                                                                         join con in dbContext.MASTER_CONTRACTOR on aggmast.MAST_CON_ID equals con.MAST_CON_ID
        //                                                                         where sancprj.MAST_STATE_CODE == objParam.STATE_CODE
        //                                                                         //&& con.MAST_CON_SUP_FLAG == objParam.MAST_CON_SUP_FLAG
        //                                                                         orderby con.MAST_CON_COMPANY_NAME ascending
        //                                                                         select new
        //                                                                         {
        //                                                                             MAST_CON_COMPANY_NAME = con.MAST_CON_SUP_FLAG == "C" ?
        //                                                                             "CON- " + "(" + SqlFunctions.StringConvert((decimal)con.MAST_CON_ID).Trim() + ") "
        //                                                                                 + ((con.MAST_CON_FNAME == null ? string.Empty : con.MAST_CON_FNAME) + " " + (con.MAST_CON_MNAME == null ? string.Empty : con.MAST_CON_MNAME) + " " + (con.MAST_CON_LNAME == null ? string.Empty : con.MAST_CON_LNAME))
        //                                                                                 + "( " + con.MAST_CON_COMPANY_NAME + ")" + "PAN( " + (con.MAST_CON_PAN == null ? "-" : con.MAST_CON_PAN) + " )"
        //                                                                                 : "SUP- " + "(" + SqlFunctions.StringConvert((decimal)con.MAST_CON_ID).Trim() + ") "
        //                                                                                 + ((con.MAST_CON_FNAME == null ? string.Empty : con.MAST_CON_FNAME) + " " + (con.MAST_CON_MNAME == null ? string.Empty : con.MAST_CON_MNAME) + " " + (con.MAST_CON_LNAME == null ? string.Empty : con.MAST_CON_LNAME))
        //                                                                                 + "( " + con.MAST_CON_COMPANY_NAME + ")" + " " + "TAN( " + (con.MAST_CON_PAN == null ? "-" : con.MAST_CON_PAN) + " )",
        //                                                                             MAST_CON_ID = con.MAST_CON_ID,
        //                                                                             MAST_CON_SUP_FLAG = con.MAST_CON_SUP_FLAG
        //                                                                         }).ToList().Distinct() : (from aggmast in dbContext.TEND_AGREEMENT_MASTER
        //                                                                                                   join conmast in dbContext.MASTER_CONTRACTOR
        //                                                                                                   on aggmast.MAST_CON_ID equals conmast.MAST_CON_ID
        //                                                                                                   where
        //                                                                                                  aggmast.TEND_AGREEMENT_TYPE == "S" &&
        //                                                                                                  aggmast.MAST_STATE_CODE == objParam.STATE_CODE
        //                                                                                                   select new
        //                                                                                                   {
        //                                                                                                       MAST_CON_COMPANY_NAME = "SUP- " + "(" + SqlFunctions.StringConvert((decimal)conmast.MAST_CON_ID).Trim() + ") " + ((conmast.MAST_CON_FNAME == null ? string.Empty : conmast.MAST_CON_FNAME) + " " + (conmast.MAST_CON_MNAME == null ? string.Empty : conmast.MAST_CON_MNAME) + " " + (conmast.MAST_CON_LNAME == null ? string.Empty : conmast.MAST_CON_LNAME)) + "( " + conmast.MAST_CON_COMPANY_NAME + ")" + " " + "TAN( " + (conmast.MAST_CON_PAN == null ? "-" : conmast.MAST_CON_PAN) + " )",
        //                                                                                                       MAST_CON_ID = conmast.MAST_CON_ID,
        //                                                                                                       MAST_CON_SUP_FLAG = conmast.MAST_CON_SUP_FLAG
        //                                                                                                   }
        //                                                                                                       ).ToList().Distinct();


        //                //addition by Koustubh Nakate on 29/08/2013 to add proposal contractors from shifted tracking
        //                var lstContractorByPropasalTracking = (from propTracking in dbContext.IMS_PROPOSAL_TRACKING
        //                                                       join aggdet in dbContext.TEND_AGREEMENT_DETAIL
        //                                                       on propTracking.IMS_PR_ROAD_CODE equals aggdet.IMS_PR_ROAD_CODE
        //                                                       join aggmast in dbContext.TEND_AGREEMENT_MASTER
        //                                                       on aggdet.TEND_AGREEMENT_CODE equals aggmast.TEND_AGREEMENT_CODE
        //                                                       join con in dbContext.MASTER_CONTRACTOR
        //                                                       on aggmast.MAST_CON_ID equals con.MAST_CON_ID
        //                                                       where propTracking.MAST_STATE_CODE == objParam.STATE_CODE
        //                                                       orderby con.MAST_CON_COMPANY_NAME ascending
        //                                                       select new
        //                                                       {
        //                                                           MAST_CON_COMPANY_NAME = con.MAST_CON_SUP_FLAG == "C" ?
        //                                                           "CON- " + "(" + SqlFunctions.StringConvert((decimal)con.MAST_CON_ID).Trim() + ") "
        //                                                               + ((con.MAST_CON_FNAME == null ? string.Empty : con.MAST_CON_FNAME) + " " + (con.MAST_CON_MNAME == null ? string.Empty : con.MAST_CON_MNAME) + " " + (con.MAST_CON_LNAME == null ? string.Empty : con.MAST_CON_LNAME))
        //                                                               + "( " + con.MAST_CON_COMPANY_NAME + ")" + " " + "PAN( " + (con.MAST_CON_PAN == null ? "-" : con.MAST_CON_PAN) + " )"
        //                                                               : "SUP- " + "(" + SqlFunctions.StringConvert((decimal)con.MAST_CON_ID).Trim() + ") "
        //                                                               + ((con.MAST_CON_FNAME == null ? string.Empty : con.MAST_CON_FNAME) + " " + (con.MAST_CON_MNAME == null ? string.Empty : con.MAST_CON_MNAME) + " " + (con.MAST_CON_LNAME == null ? string.Empty : con.MAST_CON_LNAME))
        //                                                               + "( " + con.MAST_CON_COMPANY_NAME + ")" + " " + "TAN( " + (con.MAST_CON_PAN == null ? "-" : con.MAST_CON_PAN) + " )",
        //                                                           MAST_CON_ID = con.MAST_CON_ID,
        //                                                           MAST_CON_SUP_FLAG = con.MAST_CON_SUP_FLAG
        //                                                       }).ToList().Distinct();


        //                lstContractor = lstContractor.Union(lstContractorByPropasalTracking).ToList().Distinct();

        //                if (objParam.MAST_CON_SUP_FLAG == "C")
        //                {

        //                    lstContractor = lstContractor.Where(x => x.MAST_CON_SUP_FLAG == "C" || x.MAST_CON_SUP_FLAG == "R");

        //                    // get the DPR list and concat it with contractor Change date 13/08/2013

        //                    var lstDrp = (from tm in dbContext.TEND_AGREEMENT_MASTER
        //                                  join cn in dbContext.MASTER_CONTRACTOR on tm.MAST_CON_ID equals cn.MAST_CON_ID
        //                                  where
        //                                      // tm.MAST_DISTRICT_CODE == 441
        //                                  tm.MAST_STATE_CODE == objParam.STATE_CODE
        //                                  && tm.TEND_AGREEMENT_TYPE == "D"
        //                                  orderby cn.MAST_CON_COMPANY_NAME ascending
        //                                  select new
        //                                  {
        //                                      MAST_CON_COMPANY_NAME = "DPR- " + "(" + SqlFunctions.StringConvert((decimal)cn.MAST_CON_ID).Trim() + ") "
        //                                      + ((cn.MAST_CON_FNAME == null ? string.Empty : cn.MAST_CON_FNAME) + " " + (cn.MAST_CON_MNAME == null ? string.Empty : cn.MAST_CON_MNAME) + " " + (cn.MAST_CON_LNAME == null ? string.Empty : cn.MAST_CON_LNAME))
        //                                      + "(" + cn.MAST_CON_COMPANY_NAME + ")",
        //                                      MAST_CON_ID = cn.MAST_CON_ID,
        //                                      MAST_CON_SUP_FLAG = cn.MAST_CON_SUP_FLAG
        //                                  }).ToList().Distinct();


        //                    if (lstDrp != null && lstDrp.Count() != 0)
        //                    {
        //                        lstDrp = lstDrp.Except(lstContractor);
        //                        lstContractor = lstContractor.Concat(lstDrp);
        //                    }


        //                }

        //                if (objParam.MAST_CON_SUP_FLAG == "S")
        //                {

        //                    lstContractor = lstContractor.Where(x => x.MAST_CON_SUP_FLAG == "S");

        //                }



        //                if (lstContractor == null || lstContractor.Count() == 0)
        //                {
        //                    if (objParam.MAST_CON_SUP_FLAG == "C")
        //                    {
        //                        lstContDetails.Insert(0, (new SelectListItem { Text = "--Select--", Value = "0", Selected = true }));
        //                    }
        //                    else
        //                    {
        //                        lstContDetails.Insert(0, (new SelectListItem { Text = "Select Supplier", Value = "0", Selected = true }));
        //                    }
        //                    return lstContDetails;
        //                }
        //                else
        //                {
        //                    foreach (var item in lstContractor)
        //                    {
        //                        if (item.MAST_CON_ID == objParam.MAST_CONT_ID)
        //                        {
        //                            lstContDetails.Add(new SelectListItem { Text = item.MAST_CON_COMPANY_NAME, Value = item.MAST_CON_ID.ToString(), Selected = true });
        //                        }
        //                        else
        //                        {
        //                            lstContDetails.Add(new SelectListItem { Text = item.MAST_CON_COMPANY_NAME, Value = item.MAST_CON_ID.ToString() });
        //                        }
        //                    }
        //                }
        //                lstContDetails = lstContDetails.OrderBy(m => m.Text).ToList();
        //                if (objParam.MAST_CON_SUP_FLAG == "C")
        //                {
        //                    lstContDetails.Insert(0, (new SelectListItem { Text = "--Select--", Value = "0" }));
        //                }
        //                else
        //                {
        //                    lstContDetails.Insert(0, (new SelectListItem { Text = "Select Supplier", Value = "0" }));
        //                }
        //            }
        //                // for admin fund added on 18-11-2021
        //            else 
        //            {
        //                var lstContractor =
        //                     (from CN in dbContext.MASTER_CONTRACTOR
        //                      join BK in dbContext.MASTER_CONTRACTOR_BANK
        //                      on CN.MAST_CON_ID equals BK.MAST_CON_ID
        //                      join RN in dbContext.MASTER_CONTRACTOR_REGISTRATION
        //                      on CN.MAST_CON_ID equals RN.MAST_CON_ID
        //                      where CN.MAST_STATE_CODE_ADDR == PMGSYSession.Current.StateCode &&
        //                      CN.MAST_CON_STATUS == "A" && RN.FUND_TYPE == "A"
        //                      select new
        //                      {
        //                          MAST_CON_COMPANY_NAME = ((CN.MAST_CON_FNAME == null ? string.Empty : CN.MAST_CON_FNAME) + " " + (CN.MAST_CON_MNAME == null ? string.Empty :
        //                                                    CN.MAST_CON_MNAME) + " " + (CN.MAST_CON_LNAME == null ? string.Empty : CN.MAST_CON_LNAME))
        //                                                    + "( " + CN.MAST_CON_COMPANY_NAME + ")" + "TAN( " + (CN.MAST_CON_PAN == null ? "-" : CN.MAST_CON_PAN) + " )",
        //                          MAST_CON_ID = CN.MAST_CON_ID,
        //                          MAST_CON_SUP_FLAG = CN.MAST_CON_SUP_FLAG
        //                      }).ToList().Distinct();


        //                if (lstContractor == null || lstContractor.Count() == 0)
        //                {
        //                    lstContDetails.Insert(0, (new SelectListItem { Text = "Other", Value = "-1" }));
        //                    if (objParam.MAST_CON_SUP_FLAG == "C")
        //                    {
        //                        lstContDetails.Insert(0, (new SelectListItem { Text = "--Select--", Value = "0", Selected = true }));
        //                    }
        //                    else
        //                    {
        //                        lstContDetails.Insert(0, (new SelectListItem { Text = "Select Payee/Supplier", Value = "0", Selected = true }));
        //                    }
        //                    return lstContDetails;
        //                }
        //                else
        //                {
        //                    foreach (var item in lstContractor)
        //                    {
        //                        if (item.MAST_CON_ID == objParam.MAST_CONT_ID)
        //                        {
        //                            lstContDetails.Add(new SelectListItem { Text = item.MAST_CON_COMPANY_NAME, Value = item.MAST_CON_ID.ToString(), Selected = true });
        //                        }
        //                        else
        //                        {
        //                            lstContDetails.Add(new SelectListItem { Text = item.MAST_CON_COMPANY_NAME, Value = item.MAST_CON_ID.ToString() });
        //                        }
        //                    }
        //                }
        //                lstContDetails = lstContDetails.OrderBy(m => m.Text).ToList();
        //                lstContDetails.Insert(0, (new SelectListItem { Text = "Other", Value = "-1" }));
        //                if (objParam.MAST_CON_SUP_FLAG == "C")
        //                {
        //                    lstContDetails.Insert(0, (new SelectListItem { Text = "--Select--", Value = "0" }));
        //                }
        //                else
        //                {
        //                    lstContDetails.Insert(0, (new SelectListItem { Text = "Select Payee/Supplier", Value = "0" }));
        //                }
        //            }
                
        //        }
        //        else
        //        {

        //            if (PMGSYSession.Current.FundType.ToLower() == "m")
        //            {
        //                //change sby Koustubh Nakate on 03/10/2013 to populate supplier properly 

        //                var lstContractor = objParam.MAST_CON_SUP_FLAG == "C" ? (from sancprj in dbContext.IMS_SANCTIONED_PROJECTS
        //                                                                         join mancon in dbContext.MANE_IMS_CONTRACT on sancprj.IMS_PR_ROAD_CODE equals mancon.IMS_PR_ROAD_CODE
        //                                                                         join con in dbContext.MASTER_CONTRACTOR on mancon.MAST_CON_ID equals con.MAST_CON_ID
        //                                                                         where sancprj.MAST_DISTRICT_CODE == objParam.DISTRICT_CODE
        //                                                                         //&& con.MAST_CON_SUP_FLAG == objParam.MAST_CON_SUP_FLAG
        //                                                                         select new
        //                                                                         {
        //                                                                             MAST_CON_COMPANY_NAME = con.MAST_CON_SUP_FLAG == "C" ?
        //                                                                             "CON- " + "(" + SqlFunctions.StringConvert((decimal)con.MAST_CON_ID).Trim() + ") "
        //                                                                                   + ((con.MAST_CON_FNAME == null ? string.Empty : con.MAST_CON_FNAME) + " " + (con.MAST_CON_MNAME == null ? string.Empty : con.MAST_CON_MNAME) + " " + (con.MAST_CON_LNAME == null ? string.Empty : con.MAST_CON_LNAME))
        //                                                                                   + "( " + con.MAST_CON_COMPANY_NAME + ")" + " " + "PAN( " + (con.MAST_CON_PAN == null ? "-" : con.MAST_CON_PAN) + " )"
        //                                                                                   : "SUP- " + "(" + SqlFunctions.StringConvert((decimal)con.MAST_CON_ID).Trim() + ") "
        //                                                                                    + ((con.MAST_CON_FNAME == null ? string.Empty : con.MAST_CON_FNAME) + " " + (con.MAST_CON_MNAME == null ? string.Empty : con.MAST_CON_MNAME) + " " + (con.MAST_CON_LNAME == null ? string.Empty : con.MAST_CON_LNAME))
        //                                                                                  + "( " + con.MAST_CON_COMPANY_NAME + ")" + " " + "TAN( " + (con.MAST_CON_PAN == null ? "-" : con.MAST_CON_PAN) + " )",
        //                                                                             MAST_CON_ID = con.MAST_CON_ID,
        //                                                                             MAST_CON_SUP_FLAG = con.MAST_CON_SUP_FLAG
        //                                                                         }).ToList().Distinct() : (from aggmast in dbContext.TEND_AGREEMENT_MASTER
        //                                                                                                   join conmast in dbContext.MASTER_CONTRACTOR
        //                                                                                                   on aggmast.MAST_CON_ID equals conmast.MAST_CON_ID
        //                                                                                                   where
        //                                                                                                  aggmast.TEND_AGREEMENT_TYPE == "S" &&
        //                                                                                                  aggmast.MAST_DISTRICT_CODE == objParam.DISTRICT_CODE
        //                                                                                                   select new
        //                                                                                                   {
        //                                                                                                       MAST_CON_COMPANY_NAME = "SUP- " + "(" + SqlFunctions.StringConvert((decimal)conmast.MAST_CON_ID).Trim() + ") " + ((conmast.MAST_CON_FNAME == null ? string.Empty : conmast.MAST_CON_FNAME) + " " + (conmast.MAST_CON_MNAME == null ? string.Empty : conmast.MAST_CON_MNAME) + " " + (conmast.MAST_CON_LNAME == null ? string.Empty : conmast.MAST_CON_LNAME)) + "( " + conmast.MAST_CON_COMPANY_NAME + ")" + " " + "TAN( " + (conmast.MAST_CON_PAN == null ? "-" : conmast.MAST_CON_PAN) + " )",
        //                                                                                                       MAST_CON_ID = conmast.MAST_CON_ID,
        //                                                                                                       MAST_CON_SUP_FLAG = conmast.MAST_CON_SUP_FLAG
        //                                                                                                   }
        //                                                                                                       ).ToList().Distinct();

        //                //addition by Koustubh Nakate on 29/08/2013 to add proposal contractors from shifted tracking

        //                var lstContractorByPropasalTracking = (from propTracking in dbContext.IMS_PROPOSAL_TRACKING
        //                                                       join mancon in dbContext.MANE_IMS_CONTRACT
        //                                                       on propTracking.IMS_PR_ROAD_CODE equals mancon.IMS_PR_ROAD_CODE
        //                                                       join con in dbContext.MASTER_CONTRACTOR
        //                                                       on mancon.MAST_CON_ID equals con.MAST_CON_ID
        //                                                       where propTracking.MAST_DISTRICT_CODE == objParam.DISTRICT_CODE
        //                                                       select new
        //                                                       {
        //                                                           MAST_CON_COMPANY_NAME = con.MAST_CON_SUP_FLAG == "C" ?
        //                                                           "CON- " + "(" + SqlFunctions.StringConvert((decimal)con.MAST_CON_ID).Trim() + ") "
        //                                                                 + ((con.MAST_CON_FNAME == null ? string.Empty : con.MAST_CON_FNAME) + " " + (con.MAST_CON_MNAME == null ? string.Empty : con.MAST_CON_MNAME) + " " + (con.MAST_CON_LNAME == null ? string.Empty : con.MAST_CON_LNAME))
        //                                                                 + "( " + con.MAST_CON_COMPANY_NAME + ")" + " " + "PAN( " + (con.MAST_CON_PAN == null ? "-" : con.MAST_CON_PAN) + " )"
        //                                                                 : "SUP- " + "(" + SqlFunctions.StringConvert((decimal)con.MAST_CON_ID).Trim() + ") "
        //                                                                  + ((con.MAST_CON_FNAME == null ? string.Empty : con.MAST_CON_FNAME) + " " + (con.MAST_CON_MNAME == null ? string.Empty : con.MAST_CON_MNAME) + " " + (con.MAST_CON_LNAME == null ? string.Empty : con.MAST_CON_LNAME))
        //                                                                + "( " + con.MAST_CON_COMPANY_NAME + ")" + " " + "TAN( " + (con.MAST_CON_PAN == null ? "-" : con.MAST_CON_PAN) + " )",
        //                                                           MAST_CON_ID = con.MAST_CON_ID,
        //                                                           MAST_CON_SUP_FLAG = con.MAST_CON_SUP_FLAG
        //                                                       }).ToList().Distinct();


        //                lstContractor = lstContractor.Union(lstContractorByPropasalTracking).ToList().Distinct();




        //                if (objParam.MAST_CON_SUP_FLAG == "C")
        //                {

        //                    lstContractor = lstContractor.Where(x => x.MAST_CON_SUP_FLAG == "C" || x.MAST_CON_SUP_FLAG == "R");

        //                }

        //                if (objParam.MAST_CON_SUP_FLAG == "S")
        //                {

        //                    lstContractor = lstContractor.Where(x => x.MAST_CON_SUP_FLAG == "S");

        //                }


        //                if (lstContractor == null || lstContractor.Count() == 0)
        //                {
        //                    if (objParam.MAST_CON_SUP_FLAG == "C")
        //                    {
        //                        lstContDetails.Insert(0, (new SelectListItem { Text = "--Select--", Value = "0", Selected = true }));
        //                    }
        //                    else
        //                    {
        //                        lstContDetails.Insert(0, (new SelectListItem { Text = "Select Supplier", Value = "0", Selected = true }));
        //                    }
        //                    return lstContDetails;
        //                }
        //                else
        //                {
        //                    foreach (var item in lstContractor)
        //                    {
        //                        if (item.MAST_CON_ID == objParam.MAST_CONT_ID)
        //                        {
        //                            lstContDetails.Add(new SelectListItem { Text = item.MAST_CON_COMPANY_NAME, Value = item.MAST_CON_ID.ToString(), Selected = true });
        //                        }
        //                        else
        //                        {
        //                            lstContDetails.Add(new SelectListItem { Text = item.MAST_CON_COMPANY_NAME, Value = item.MAST_CON_ID.ToString() });
        //                        }
        //                    }
        //                }
        //                lstContDetails = lstContDetails.OrderBy(m => m.Text).ToList();
        //                if (objParam.MAST_CON_SUP_FLAG == "C")
        //                {
        //                    lstContDetails.Insert(0, (new SelectListItem { Text = "--Select--", Value = "0" }));
        //                }
        //                else
        //                {
        //                    lstContDetails.Insert(0, (new SelectListItem { Text = "Select Supplier", Value = "0" }));
        //                }
        //            }
        //            else if (PMGSYSession.Current.FundType.ToLower() == "a")
        //            {
        //                if (objParam.TXN_ID == 455)
        //                {
        //                    var lstContractor =
        //                      (from CN in dbContext.MASTER_CONTRACTOR
        //                       join BK in dbContext.MASTER_CONTRACTOR_BANK
        //                       on CN.MAST_CON_ID equals BK.MAST_CON_ID
        //                       where CN.MAST_STATE_CODE_ADDR == PMGSYSession.Current.StateCode &&
        //                       CN.MAST_DISTRICT_CODE_ADDR == objParam.DISTRICT_CODE &&
        //                       CN.MAST_CON_SUP_FLAG == "S" &&
        //                       CN.MAST_CON_STATUS == "A"
        //                       select new
        //                       {
        //                           MAST_CON_COMPANY_NAME = ((CN.MAST_CON_FNAME == null ? string.Empty : CN.MAST_CON_FNAME) + " " + (CN.MAST_CON_MNAME == null ? string.Empty :
        //                                                     CN.MAST_CON_MNAME) + " " + (CN.MAST_CON_LNAME == null ? string.Empty : CN.MAST_CON_LNAME))
        //                                                     + "( " + CN.MAST_CON_COMPANY_NAME + ")" + "TAN( " + (CN.MAST_CON_PAN == null ? "-" : CN.MAST_CON_PAN) + " )",
        //                           MAST_CON_ID = CN.MAST_CON_ID,
        //                           MAST_CON_SUP_FLAG = CN.MAST_CON_SUP_FLAG
        //                       }).ToList().Distinct();


        //                    if (lstContractor == null || lstContractor.Count() == 0)
        //                    {
        //                        lstContDetails.Insert(0, (new SelectListItem { Text = "Other", Value = "-1" }));
        //                        if (objParam.MAST_CON_SUP_FLAG == "C")
        //                        {
        //                            lstContDetails.Insert(0, (new SelectListItem { Text = "--Select--", Value = "0", Selected = true }));
        //                        }
        //                        else
        //                        {
        //                            lstContDetails.Insert(0, (new SelectListItem { Text = "Select Payee/Supplier", Value = "0", Selected = true }));
        //                        }
        //                        return lstContDetails;
        //                    }
        //                    else
        //                    {
        //                        foreach (var item in lstContractor)
        //                        {
        //                            if (item.MAST_CON_ID == objParam.MAST_CONT_ID)
        //                            {
        //                                lstContDetails.Add(new SelectListItem { Text = item.MAST_CON_COMPANY_NAME, Value = item.MAST_CON_ID.ToString(), Selected = true });
        //                            }
        //                            else
        //                            {
        //                                lstContDetails.Add(new SelectListItem { Text = item.MAST_CON_COMPANY_NAME, Value = item.MAST_CON_ID.ToString() });
        //                            }
        //                        }
        //                    }
        //                    lstContDetails = lstContDetails.OrderBy(m => m.Text).ToList();
        //                    lstContDetails.Insert(0, (new SelectListItem { Text = "Other", Value = "-1" }));
        //                    if (objParam.MAST_CON_SUP_FLAG == "C")
        //                    {
        //                        lstContDetails.Insert(0, (new SelectListItem { Text = "--Select--", Value = "0" }));
        //                    }
        //                    else
        //                    {
        //                        lstContDetails.Insert(0, (new SelectListItem { Text = "Select Payee/Supplier", Value = "0" }));
        //                    }

        //                }
        //                else if (objParam.TXN_ID == 472 || objParam.TXN_ID == 415)
        //                {
        //                    var lstContractor =
        //                      (from CN in dbContext.ADMIN_NODAL_OFFICERS
        //                       join BK in dbContext.ADMIN_NO_BANK
        //                       on CN.ADMIN_NO_OFFICER_CODE equals BK.ADMIN_NO_OFFICER_CODE
        //                       where
        //                       CN.MAST_DISTRICT_CODE == objParam.DISTRICT_CODE &&

        //                       CN.ADMIN_ACTIVE_STATUS == "Y"
        //                       select new
        //                       {
        //                           MAST_CON_COMPANY_NAME = ((CN.ADMIN_NO_FNAME == null ? string.Empty : CN.ADMIN_NO_FNAME) + " " + (CN.ADMIN_NO_MNAME == null ? string.Empty :
        //                                                     CN.ADMIN_NO_MNAME) + " " + (CN.ADMIN_NO_LNAME == null ? string.Empty : CN.ADMIN_NO_LNAME))
        //                                                    ,
        //                           MAST_CON_ID = CN.ADMIN_NO_OFFICER_CODE,
        //                           MAST_CON_SUP_FLAG = "S"
        //                       }).ToList().Distinct();


        //                    if (lstContractor == null || lstContractor.Count() == 0)
        //                    {
        //                        if (objParam.MAST_CON_SUP_FLAG == "C")
        //                        {
        //                            lstContDetails.Insert(0, (new SelectListItem { Text = "--Select--", Value = "0", Selected = true }));
        //                        }
        //                        else
        //                        {
        //                            lstContDetails.Insert(0, (new SelectListItem { Text = "Select Payee/Supplier", Value = "0", Selected = true }));
        //                        }
        //                        return lstContDetails;
        //                    }
        //                    else
        //                    {
        //                        foreach (var item in lstContractor)
        //                        {
        //                            if (item.MAST_CON_ID == objParam.MAST_CONT_ID)
        //                            {
        //                                lstContDetails.Add(new SelectListItem { Text = item.MAST_CON_COMPANY_NAME, Value = item.MAST_CON_ID.ToString(), Selected = true });
        //                            }
        //                            else
        //                            {
        //                                lstContDetails.Add(new SelectListItem { Text = item.MAST_CON_COMPANY_NAME, Value = item.MAST_CON_ID.ToString() });
        //                            }
        //                        }
        //                    }
        //                    lstContDetails = lstContDetails.OrderBy(m => m.Text).ToList();
        //                    lstContDetails.Insert(0, (new SelectListItem { Text = "Other", Value = "-1" }));
        //                    if (objParam.MAST_CON_SUP_FLAG == "C")
        //                    {
        //                        lstContDetails.Insert(0, (new SelectListItem { Text = "--Select--", Value = "0" }));
        //                    }
        //                    else
        //                    {
        //                        lstContDetails.Insert(0, (new SelectListItem { Text = "Select Payee/Supplier", Value = "0" }));
        //                    }

        //                    //lstContDetails = lstContDetails.OrderBy(m => m.Text).ToList();
        //                }

        //                //if (objParam.TXN_ID == 455)
        //                //{
        //                //    var lstContractor =
        //                //      (from CN in dbContext.MASTER_CONTRACTOR
        //                //       join BK in dbContext.MASTER_CONTRACTOR_BANK
        //                //       on CN.MAST_CON_ID equals BK.MAST_CON_ID
        //                //       where CN.MAST_STATE_CODE_ADDR == PMGSYSession.Current.StateCode &&
        //                //       CN.MAST_DISTRICT_CODE_ADDR == objParam.DISTRICT_CODE &&
        //                //       CN.MAST_CON_SUP_FLAG == "S" &&
        //                //       CN.MAST_CON_STATUS == "A"
        //                //       select new
        //                //       {
        //                //           MAST_CON_COMPANY_NAME = ((CN.MAST_CON_FNAME == null ? string.Empty : CN.MAST_CON_FNAME) + " " + (CN.MAST_CON_MNAME == null ? string.Empty :
        //                //                                     CN.MAST_CON_MNAME) + " " + (CN.MAST_CON_LNAME == null ? string.Empty : CN.MAST_CON_LNAME))
        //                //                                     + "( " + CN.MAST_CON_COMPANY_NAME + ")" + "TAN( " + (CN.MAST_CON_PAN == null ? "-" : CN.MAST_CON_PAN) + " )",
        //                //           MAST_CON_ID = CN.MAST_CON_ID,
        //                //           MAST_CON_SUP_FLAG = CN.MAST_CON_SUP_FLAG
        //                //       }).ToList().Distinct();


        //                //    if (lstContractor == null || lstContractor.Count() == 0)
        //                //    {
        //                //        lstContDetails.Insert(0, (new SelectListItem { Text = "Other", Value = "-1" }));
        //                //        if (objParam.MAST_CON_SUP_FLAG == "C")
        //                //        {
        //                //            lstContDetails.Insert(0, (new SelectListItem { Text = "--Select--", Value = "0", Selected = true }));
        //                //        }
        //                //        else
        //                //        {
        //                //            lstContDetails.Insert(0, (new SelectListItem { Text = "Select Payee/Supplier", Value = "0", Selected = true }));
        //                //        }
        //                //        return lstContDetails;
        //                //    }
        //                //    else
        //                //    {
        //                //        foreach (var item in lstContractor)
        //                //        {
        //                //            if (item.MAST_CON_ID == objParam.MAST_CONT_ID)
        //                //            {
        //                //                lstContDetails.Add(new SelectListItem { Text = item.MAST_CON_COMPANY_NAME, Value = item.MAST_CON_ID.ToString(), Selected = true });
        //                //            }
        //                //            else
        //                //            {
        //                //                lstContDetails.Add(new SelectListItem { Text = item.MAST_CON_COMPANY_NAME, Value = item.MAST_CON_ID.ToString() });
        //                //            }
        //                //        }
        //                //    }
        //                //    lstContDetails = lstContDetails.OrderBy(m => m.Text).ToList();
        //                //    lstContDetails.Insert(0, (new SelectListItem { Text = "Other", Value = "-1" }));
        //                //    if (objParam.MAST_CON_SUP_FLAG == "C")
        //                //    {
        //                //        lstContDetails.Insert(0, (new SelectListItem { Text = "--Select--", Value = "0" }));
        //                //    }
        //                //    else
        //                //    {
        //                //        lstContDetails.Insert(0, (new SelectListItem { Text = "Select Payee/Supplier", Value = "0" }));
        //                //    }

        //                //}
        //                //else if (objParam.TXN_ID == 472 || objParam.TXN_ID == 415)
        //                //{
        //                //    var lstContractor =
        //                //      (from CN in dbContext.ADMIN_NODAL_OFFICERS
        //                //       join BK in dbContext.ADMIN_NO_BANK
        //                //       on CN.ADMIN_NO_OFFICER_CODE equals BK.ADMIN_NO_OFFICER_CODE
        //                //       where
        //                //       CN.MAST_DISTRICT_CODE == objParam.DISTRICT_CODE &&

        //                //       CN.ADMIN_ACTIVE_STATUS == "Y"
        //                //       select new
        //                //       {
        //                //           MAST_CON_COMPANY_NAME = ((CN.ADMIN_NO_FNAME == null ? string.Empty : CN.ADMIN_NO_FNAME) + " " + (CN.ADMIN_NO_MNAME == null ? string.Empty :
        //                //                                     CN.ADMIN_NO_MNAME) + " " + (CN.ADMIN_NO_LNAME == null ? string.Empty : CN.ADMIN_NO_LNAME))
        //                //                                    ,
        //                //           MAST_CON_ID = CN.ADMIN_NO_OFFICER_CODE,
        //                //           MAST_CON_SUP_FLAG = "S"
        //                //       }).ToList().Distinct();


        //                //    if (lstContractor == null || lstContractor.Count() == 0)
        //                //    {
        //                //        if (objParam.MAST_CON_SUP_FLAG == "C")
        //                //        {
        //                //            lstContDetails.Insert(0, (new SelectListItem { Text = "--Select--", Value = "0", Selected = true }));
        //                //        }
        //                //        else
        //                //        {
        //                //            lstContDetails.Insert(0, (new SelectListItem { Text = "Select Payee/Supplier", Value = "0", Selected = true }));
        //                //        }
        //                //        return lstContDetails;
        //                //    }
        //                //    else
        //                //    {
        //                //        foreach (var item in lstContractor)
        //                //        {
        //                //            if (item.MAST_CON_ID == objParam.MAST_CONT_ID)
        //                //            {
        //                //                lstContDetails.Add(new SelectListItem { Text = item.MAST_CON_COMPANY_NAME, Value = item.MAST_CON_ID.ToString(), Selected = true });
        //                //            }
        //                //            else
        //                //            {
        //                //                lstContDetails.Add(new SelectListItem { Text = item.MAST_CON_COMPANY_NAME, Value = item.MAST_CON_ID.ToString() });
        //                //            }
        //                //        }
        //                //    }
        //                //    lstContDetails = lstContDetails.OrderBy(m => m.Text).ToList();
        //                //    lstContDetails.Insert(0, (new SelectListItem { Text = "Other", Value = "-1" }));
        //                //    if (objParam.MAST_CON_SUP_FLAG == "C")
        //                //    {
        //                //        lstContDetails.Insert(0, (new SelectListItem { Text = "--Select--", Value = "0" }));
        //                //    }
        //                //    else
        //                //    {
        //                //        lstContDetails.Insert(0, (new SelectListItem { Text = "Select Payee/Supplier", Value = "0" }));
        //                //    }

        //                //    //lstContDetails = lstContDetails.OrderBy(m => m.Text).ToList();
        //                //}

        //            }
        //            else
        //            {

        //                //change sby Koustubh Nakate on 28/09/2013 to populate supplier properly 

        //                var lstContractor = objParam.MAST_CON_SUP_FLAG == "C" ? (from sancprj in dbContext.IMS_SANCTIONED_PROJECTS
        //                                                                         join aggdet in dbContext.TEND_AGREEMENT_DETAIL on sancprj.IMS_PR_ROAD_CODE equals aggdet.IMS_PR_ROAD_CODE
        //                                                                         join aggmast in dbContext.TEND_AGREEMENT_MASTER on aggdet.TEND_AGREEMENT_CODE equals aggmast.TEND_AGREEMENT_CODE
        //                                                                         join con in dbContext.MASTER_CONTRACTOR on aggmast.MAST_CON_ID equals con.MAST_CON_ID
        //                                                                         where sancprj.MAST_DISTRICT_CODE == objParam.DISTRICT_CODE

        //                                                                         //&& con.MAST_CON_SUP_FLAG == objParam.MAST_CON_SUP_FLAG
        //                                                                         select new
        //                                                                         {
        //                                                                             MAST_CON_COMPANY_NAME = con.MAST_CON_SUP_FLAG == "C" ?
        //                                                                             "CON- " + "(" + SqlFunctions.StringConvert((decimal)con.MAST_CON_ID).Trim() + ") "
        //                                                                                   + ((con.MAST_CON_FNAME == null ? string.Empty : con.MAST_CON_FNAME) + " " + (con.MAST_CON_MNAME == null ? string.Empty : con.MAST_CON_MNAME) + " " + (con.MAST_CON_LNAME == null ? string.Empty : con.MAST_CON_LNAME))
        //                                                                                   + "( " + con.MAST_CON_COMPANY_NAME + ")" + " " + "PAN( " + (con.MAST_CON_PAN == null ? "-" : con.MAST_CON_PAN) + " )"
        //                                                                                   : "SUP- " + "(" + SqlFunctions.StringConvert((decimal)con.MAST_CON_ID).Trim() + ") "
        //                                                                                    + ((con.MAST_CON_FNAME == null ? string.Empty : con.MAST_CON_FNAME) + " " + (con.MAST_CON_MNAME == null ? string.Empty : con.MAST_CON_MNAME) + " " + (con.MAST_CON_LNAME == null ? string.Empty : con.MAST_CON_LNAME))
        //                                                                                  + "( " + con.MAST_CON_COMPANY_NAME + ")" + " " + "TAN( " + (con.MAST_CON_PAN == null ? "-" : con.MAST_CON_PAN) + " )",
        //                                                                             MAST_CON_ID = con.MAST_CON_ID,
        //                                                                             MAST_CON_SUP_FLAG = con.MAST_CON_SUP_FLAG
        //                                                                         }).ToList().Distinct() : (from aggmast in dbContext.TEND_AGREEMENT_MASTER
        //                                                                                                   join conmast in dbContext.MASTER_CONTRACTOR
        //                                                                                                   on aggmast.MAST_CON_ID equals conmast.MAST_CON_ID
        //                                                                                                   where
        //                                                                                                  aggmast.TEND_AGREEMENT_TYPE == "S" &&
        //                                                                                                  aggmast.MAST_DISTRICT_CODE == objParam.DISTRICT_CODE
        //                                                                                                   select new
        //                                                                                                   {
        //                                                                                                       MAST_CON_COMPANY_NAME = "SUP- " + "(" + SqlFunctions.StringConvert((decimal)conmast.MAST_CON_ID).Trim() + ") " + ((conmast.MAST_CON_FNAME == null ? string.Empty : conmast.MAST_CON_FNAME) + " " + (conmast.MAST_CON_MNAME == null ? string.Empty : conmast.MAST_CON_MNAME) + " " + (conmast.MAST_CON_LNAME == null ? string.Empty : conmast.MAST_CON_LNAME)) + "( " + conmast.MAST_CON_COMPANY_NAME + ")" + " " + "TAN( " + (conmast.MAST_CON_PAN == null ? "-" : conmast.MAST_CON_PAN) + " )",
        //                                                                                                       MAST_CON_ID = conmast.MAST_CON_ID,
        //                                                                                                       MAST_CON_SUP_FLAG = conmast.MAST_CON_SUP_FLAG
        //                                                                                                   }
        //                                                                                                       ).ToList().Distinct();



        //                //addition by Koustubh Nakate on 29/08/2013 to add proposal contractors from shifted tracking
        //                var lstContractorByPropasalTracking = (from propTracking in dbContext.IMS_PROPOSAL_TRACKING
        //                                                       join aggdet in dbContext.TEND_AGREEMENT_DETAIL
        //                                                       on propTracking.IMS_PR_ROAD_CODE equals aggdet.IMS_PR_ROAD_CODE
        //                                                       join aggmast in dbContext.TEND_AGREEMENT_MASTER
        //                                                       on aggdet.TEND_AGREEMENT_CODE equals aggmast.TEND_AGREEMENT_CODE
        //                                                       join con in dbContext.MASTER_CONTRACTOR on aggmast.MAST_CON_ID equals con.MAST_CON_ID
        //                                                       where propTracking.MAST_DISTRICT_CODE == objParam.DISTRICT_CODE
        //                                                       select new
        //                                                       {
        //                                                           MAST_CON_COMPANY_NAME = con.MAST_CON_SUP_FLAG == "C" ?
        //                                                           "CON- " + "(" + SqlFunctions.StringConvert((decimal)con.MAST_CON_ID).Trim() + ") "
        //                                                                 + ((con.MAST_CON_FNAME == null ? string.Empty : con.MAST_CON_FNAME) + " " + (con.MAST_CON_MNAME == null ? string.Empty : con.MAST_CON_MNAME) + " " + (con.MAST_CON_LNAME == null ? string.Empty : con.MAST_CON_LNAME))
        //                                                                 + "( " + con.MAST_CON_COMPANY_NAME + ")" + " " + "PAN( " + (con.MAST_CON_PAN == null ? "-" : con.MAST_CON_PAN) + " )"
        //                                                                 : "SUP- " + "(" + SqlFunctions.StringConvert((decimal)con.MAST_CON_ID).Trim() + ") "
        //                                                                  + ((con.MAST_CON_FNAME == null ? string.Empty : con.MAST_CON_FNAME) + " " + (con.MAST_CON_MNAME == null ? string.Empty : con.MAST_CON_MNAME) + " " + (con.MAST_CON_LNAME == null ? string.Empty : con.MAST_CON_LNAME))
        //                                                                + "( " + con.MAST_CON_COMPANY_NAME + ")" + " " + "TAN( " + (con.MAST_CON_PAN == null ? "-" : con.MAST_CON_PAN) + " )",
        //                                                           MAST_CON_ID = con.MAST_CON_ID,
        //                                                           MAST_CON_SUP_FLAG = con.MAST_CON_SUP_FLAG
        //                                                       }).ToList().Distinct();


        //                lstContractor = lstContractor.Union(lstContractorByPropasalTracking).ToList().Distinct();



        //                if (objParam.MAST_CON_SUP_FLAG == "C")
        //                {
        //                    //MAST_CON_SUP_FLAG=="S" is checked for TOB to populate suplier for Contractor Agreement. for balance transfer 2Sep2014 by Abhisehk kamble
        //                    lstContractor = lstContractor.Where(x => x.MAST_CON_SUP_FLAG == "C" || x.MAST_CON_SUP_FLAG == "R" || x.MAST_CON_SUP_FLAG == "S");

        //                    // get the DPR list and concat it with contractor Change date 13/08/2013

        //                    var lstDrp = (from tm in dbContext.TEND_AGREEMENT_MASTER
        //                                  join cn in dbContext.MASTER_CONTRACTOR on tm.MAST_CON_ID equals cn.MAST_CON_ID
        //                                  where tm.MAST_DISTRICT_CODE == objParam.DISTRICT_CODE && (tm.TEND_AGREEMENT_TYPE == "D" || tm.TEND_AGREEMENT_TYPE == "S")
        //                                  orderby cn.MAST_CON_COMPANY_NAME ascending
        //                                  select new
        //                                  {
        //                                      MAST_CON_COMPANY_NAME = cn.MAST_CON_SUP_FLAG == "D" ?
        //                                      "DPR- " + "(" + SqlFunctions.StringConvert((decimal)cn.MAST_CON_ID).Trim() + ") "
        //                                                    + ((cn.MAST_CON_FNAME == null ? string.Empty : cn.MAST_CON_FNAME) + " " + (cn.MAST_CON_MNAME == null ? string.Empty : cn.MAST_CON_MNAME) + " " + (cn.MAST_CON_LNAME == null ? string.Empty :                                                                      cn.MAST_CON_LNAME)) + "(" + cn.MAST_CON_COMPANY_NAME + ")"
        //                                      : "SUP- " + "(" + SqlFunctions.StringConvert((decimal)cn.MAST_CON_ID).Trim() + ") "
        //                                                                  + ((cn.MAST_CON_FNAME == null ? string.Empty : cn.MAST_CON_FNAME) + " " + (cn.MAST_CON_MNAME == null ? string.Empty : cn.MAST_CON_MNAME) + " " + (cn.MAST_CON_LNAME == null ?                                                             string.Empty : cn.MAST_CON_LNAME)) + "(" + cn.MAST_CON_COMPANY_NAME + ")"   
        //                                      ,
        //                                      MAST_CON_ID = cn.MAST_CON_ID,
        //                                      MAST_CON_SUP_FLAG = cn.MAST_CON_SUP_FLAG
        //                                  }).ToList().Distinct();

        //                    if (lstDrp != null && lstDrp.Count() != 0)
        //                    {
        //                        lstContractor = lstContractor.Concat(lstDrp);
        //                    }


        //                }



        //                if (objParam.MAST_CON_SUP_FLAG == "S")
        //                {

        //                    lstContractor = lstContractor.Where(x => x.MAST_CON_SUP_FLAG == "S");

        //                }





        //                if (lstContractor == null || lstContractor.Count() == 0)
        //                {
        //                    if (objParam.MAST_CON_SUP_FLAG == "C")
        //                    {
        //                        lstContDetails.Insert(0, (new SelectListItem { Text = "--Select--", Value = "0", Selected = true }));
        //                    }
        //                    else
        //                    {
        //                        lstContDetails.Insert(0, (new SelectListItem { Text = "Select Supplier", Value = "0", Selected = true }));
        //                    }
        //                    return lstContDetails;
        //                }
        //                else
        //                {
        //                    foreach (var item in lstContractor)
        //                    {
        //                        if (item.MAST_CON_ID == objParam.MAST_CONT_ID)
        //                        {
        //                            lstContDetails.Add(new SelectListItem { Text = item.MAST_CON_COMPANY_NAME, Value = item.MAST_CON_ID.ToString(), Selected = true });
        //                        }
        //                        else
        //                        {
        //                            lstContDetails.Add(new SelectListItem { Text = item.MAST_CON_COMPANY_NAME, Value = item.MAST_CON_ID.ToString() });
        //                        }
        //                    }
        //                    lstContDetails = lstContDetails.OrderBy(m => m.Text).ToList();
        //                    if (objParam.MAST_CON_SUP_FLAG == "C")
        //                    {
        //                        lstContDetails.Insert(0, (new SelectListItem { Text = "--Select--", Value = "0" }));
        //                    }
        //                    else
        //                    {
        //                        lstContDetails.Insert(0, (new SelectListItem { Text = "Select Supplier", Value = "0" }));
        //                    }
        //                }
        //            }
        //        }

        //        return lstContDetails;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        dbContext.Dispose();
        //    }
        //}

        public List<SelectListItem> PopulateContractorSupplier(TransactionParams objParam)
        {
            try
            {
                dbContext = new PMGSYEntities();
                if (String.IsNullOrEmpty(objParam.MAST_CON_SUP_FLAG))
                {
                    objParam.MAST_CON_SUP_FLAG = "C";
                }
                List<SelectListItem> lstContDetails = new List<SelectListItem>();

                //populate for state
                if (objParam.DISTRICT_CODE == 0)
                {
                    //for maintanace fund
                    if (PMGSYSession.Current.FundType.ToLower() == "m")
                    {
                        //change sby Koustubh Nakate on 03/10/2013 to populate supplier properly 

                        var lstContractor = objParam.MAST_CON_SUP_FLAG == "C" ? (from sancprj in dbContext.IMS_SANCTIONED_PROJECTS
                                                                                 join mancon in dbContext.MANE_IMS_CONTRACT on sancprj.IMS_PR_ROAD_CODE equals mancon.IMS_PR_ROAD_CODE
                                                                                 join con in dbContext.MASTER_CONTRACTOR on mancon.MAST_CON_ID equals con.MAST_CON_ID
                                                                                 where sancprj.MAST_STATE_CODE == objParam.STATE_CODE
                                                                                 // && con.MAST_CON_SUP_FLAG == objParam.MAST_CON_SUP_FLAG
                                                                                 orderby con.MAST_CON_COMPANY_NAME ascending
                                                                                 select new
                                                                                 {
                                                                                     MAST_CON_COMPANY_NAME = con.MAST_CON_SUP_FLAG == "C" ?
                                                                                    "CON- " + "(" + SqlFunctions.StringConvert((decimal)con.MAST_CON_ID).Trim() + ") "
                                                                                       + ((con.MAST_CON_FNAME == null ? string.Empty : con.MAST_CON_FNAME) + " " + (con.MAST_CON_MNAME == null ? string.Empty : con.MAST_CON_MNAME) + " " + (con.MAST_CON_LNAME == null ? string.Empty : con.MAST_CON_LNAME))
                                                                                       + "( " + con.MAST_CON_COMPANY_NAME + ")" + "PAN( " + (con.MAST_CON_PAN == null ? "-" : con.MAST_CON_PAN) + " )"
                                                                                       : "SUP- " + "(" + SqlFunctions.StringConvert((decimal)con.MAST_CON_ID).Trim() + ") "
                                                                                        + ((con.MAST_CON_FNAME == null ? string.Empty : con.MAST_CON_FNAME) + " " + (con.MAST_CON_MNAME == null ? string.Empty : con.MAST_CON_MNAME) + " " + (con.MAST_CON_LNAME == null ? string.Empty : con.MAST_CON_LNAME))
                                                                                      + "( " + con.MAST_CON_COMPANY_NAME + ")" + "TAN( " + (con.MAST_CON_PAN == null ? "-" : con.MAST_CON_PAN) + " )",
                                                                                     MAST_CON_ID = con.MAST_CON_ID,
                                                                                     MAST_CON_SUP_FLAG = con.MAST_CON_SUP_FLAG
                                                                                 }).ToList().Distinct() : (from aggmast in dbContext.TEND_AGREEMENT_MASTER
                                                                                                           join conmast in dbContext.MASTER_CONTRACTOR
                                                                                                           on aggmast.MAST_CON_ID equals conmast.MAST_CON_ID
                                                                                                           where
                                                                                                          aggmast.TEND_AGREEMENT_TYPE == "S" &&
                                                                                                          aggmast.MAST_STATE_CODE == objParam.STATE_CODE
                                                                                                           select new
                                                                                                           {
                                                                                                               MAST_CON_COMPANY_NAME = "SUP- " + "(" + SqlFunctions.StringConvert((decimal)conmast.MAST_CON_ID).Trim() + ") " + ((conmast.MAST_CON_FNAME == null ? string.Empty : conmast.MAST_CON_FNAME) + " " + (conmast.MAST_CON_MNAME == null ? string.Empty : conmast.MAST_CON_MNAME) + " " + (conmast.MAST_CON_LNAME == null ? string.Empty : conmast.MAST_CON_LNAME)) + "( " + conmast.MAST_CON_COMPANY_NAME + ")" + " " + "TAN( " + (conmast.MAST_CON_PAN == null ? "-" : conmast.MAST_CON_PAN) + " )",
                                                                                                               MAST_CON_ID = conmast.MAST_CON_ID,
                                                                                                               MAST_CON_SUP_FLAG = conmast.MAST_CON_SUP_FLAG
                                                                                                           }
                                                                                                               ).ToList().Distinct();

                        //addition by Koustubh Nakate on 29/08/2013 to add proposal contractors from shifted tracking

                        var lstContractorByPropasalTracking = (from propTracking in dbContext.IMS_PROPOSAL_TRACKING
                                                               join mancon in dbContext.MANE_IMS_CONTRACT
                                                               on propTracking.IMS_PR_ROAD_CODE equals mancon.IMS_PR_ROAD_CODE
                                                               join con in dbContext.MASTER_CONTRACTOR
                                                               on mancon.MAST_CON_ID equals con.MAST_CON_ID
                                                               where propTracking.MAST_STATE_CODE == objParam.STATE_CODE
                                                               orderby con.MAST_CON_COMPANY_NAME ascending
                                                               select new
                                                               {
                                                                   MAST_CON_COMPANY_NAME = con.MAST_CON_SUP_FLAG == "C" ?
                                                                  "CON- " + "(" + SqlFunctions.StringConvert((decimal)con.MAST_CON_ID).Trim() + ") "
                                                                     + ((con.MAST_CON_FNAME == null ? string.Empty : con.MAST_CON_FNAME) + " " + (con.MAST_CON_MNAME == null ? string.Empty : con.MAST_CON_MNAME) + " " + (con.MAST_CON_LNAME == null ? string.Empty : con.MAST_CON_LNAME))
                                                                     + "( " + con.MAST_CON_COMPANY_NAME + ")" + "PAN( " + (con.MAST_CON_PAN == null ? "-" : con.MAST_CON_PAN) + " )"
                                                                     : "SUP- " + "(" + SqlFunctions.StringConvert((decimal)con.MAST_CON_ID).Trim() + ") "
                                                                      + ((con.MAST_CON_FNAME == null ? string.Empty : con.MAST_CON_FNAME) + " " + (con.MAST_CON_MNAME == null ? string.Empty : con.MAST_CON_MNAME) + " " + (con.MAST_CON_LNAME == null ? string.Empty : con.MAST_CON_LNAME))
                                                                    + "( " + con.MAST_CON_COMPANY_NAME + ")" + "TAN( " + (con.MAST_CON_PAN == null ? "-" : con.MAST_CON_PAN) + " )",
                                                                   MAST_CON_ID = con.MAST_CON_ID,
                                                                   MAST_CON_SUP_FLAG = con.MAST_CON_SUP_FLAG
                                                               }).ToList().Distinct();


                        lstContractor = lstContractor.Union(lstContractorByPropasalTracking).ToList().Distinct();



                        if (objParam.MAST_CON_SUP_FLAG == "C")
                        {

                            lstContractor = lstContractor.Where(x => x.MAST_CON_SUP_FLAG == "C" || x.MAST_CON_SUP_FLAG == "R");

                        }

                        if (objParam.MAST_CON_SUP_FLAG == "S")
                        {

                            lstContractor = lstContractor.Where(x => x.MAST_CON_SUP_FLAG == "S");

                        }


                        if (lstContractor == null || lstContractor.Count() == 0)
                        {
                            if (objParam.MAST_CON_SUP_FLAG == "C")
                            {
                                lstContDetails.Insert(0, (new SelectListItem { Text = "--Select--", Value = "0", Selected = true }));
                            }
                            else
                            {
                                lstContDetails.Insert(0, (new SelectListItem { Text = "Select Supplier", Value = "0", Selected = true }));
                            }
                            return lstContDetails;
                        }
                        else
                        {
                            foreach (var item in lstContractor)
                            {
                                if (item.MAST_CON_ID == objParam.MAST_CONT_ID)
                                {
                                    lstContDetails.Add(new SelectListItem { Text = item.MAST_CON_COMPANY_NAME, Value = item.MAST_CON_ID.ToString(), Selected = true });
                                }
                                else
                                {
                                    lstContDetails.Add(new SelectListItem { Text = item.MAST_CON_COMPANY_NAME, Value = item.MAST_CON_ID.ToString() });
                                }
                            }
                        }
                        lstContDetails = lstContDetails.OrderBy(m => m.Text).ToList();
                        if (objParam.MAST_CON_SUP_FLAG == "C")
                        {
                            lstContDetails.Insert(0, (new SelectListItem { Text = "--Select--", Value = "0" }));
                        }
                        else
                        {
                            lstContDetails.Insert(0, (new SelectListItem { Text = "Select Supplier", Value = "0" }));
                        }
                    }
                    else if (PMGSYSession.Current.FundType.ToLower() == "p") //Condition Added on 18-11-2021 
                    {
                        //changes by Koustubh Nakate on 28/09/2013 to populate supplier properly 

                        var lstContractor = objParam.MAST_CON_SUP_FLAG == "C" ? (from sancprj in dbContext.IMS_SANCTIONED_PROJECTS
                                                                                 join aggdet in dbContext.TEND_AGREEMENT_DETAIL on sancprj.IMS_PR_ROAD_CODE equals aggdet.IMS_PR_ROAD_CODE
                                                                                 join aggmast in dbContext.TEND_AGREEMENT_MASTER on aggdet.TEND_AGREEMENT_CODE equals aggmast.TEND_AGREEMENT_CODE
                                                                                 join con in dbContext.MASTER_CONTRACTOR on aggmast.MAST_CON_ID equals con.MAST_CON_ID
                                                                                 where sancprj.MAST_STATE_CODE == objParam.STATE_CODE
                                                                                 //&& con.MAST_CON_SUP_FLAG == objParam.MAST_CON_SUP_FLAG
                                                                                 orderby con.MAST_CON_COMPANY_NAME ascending
                                                                                 select new
                                                                                 {
                                                                                     MAST_CON_COMPANY_NAME = con.MAST_CON_SUP_FLAG == "C" ?
                                                                                     "CON- " + "(" + SqlFunctions.StringConvert((decimal)con.MAST_CON_ID).Trim() + ") "
                                                                                         + ((con.MAST_CON_FNAME == null ? string.Empty : con.MAST_CON_FNAME) + " " + (con.MAST_CON_MNAME == null ? string.Empty : con.MAST_CON_MNAME) + " " + (con.MAST_CON_LNAME == null ? string.Empty : con.MAST_CON_LNAME))
                                                                                         + "( " + con.MAST_CON_COMPANY_NAME + ")" + "PAN( " + (con.MAST_CON_PAN == null ? "-" : con.MAST_CON_PAN) + " )"
                                                                                         : "SUP- " + "(" + SqlFunctions.StringConvert((decimal)con.MAST_CON_ID).Trim() + ") "
                                                                                         + ((con.MAST_CON_FNAME == null ? string.Empty : con.MAST_CON_FNAME) + " " + (con.MAST_CON_MNAME == null ? string.Empty : con.MAST_CON_MNAME) + " " + (con.MAST_CON_LNAME == null ? string.Empty : con.MAST_CON_LNAME))
                                                                                         + "( " + con.MAST_CON_COMPANY_NAME + ")" + " " + "TAN( " + (con.MAST_CON_PAN == null ? "-" : con.MAST_CON_PAN) + " )",
                                                                                     MAST_CON_ID = con.MAST_CON_ID,
                                                                                     MAST_CON_SUP_FLAG = con.MAST_CON_SUP_FLAG
                                                                                 }).ToList().Distinct() : (from aggmast in dbContext.TEND_AGREEMENT_MASTER
                                                                                                           join conmast in dbContext.MASTER_CONTRACTOR
                                                                                                           on aggmast.MAST_CON_ID equals conmast.MAST_CON_ID
                                                                                                           where
                                                                                                          aggmast.TEND_AGREEMENT_TYPE == "S" &&
                                                                                                          aggmast.MAST_STATE_CODE == objParam.STATE_CODE
                                                                                                           select new
                                                                                                           {
                                                                                                               MAST_CON_COMPANY_NAME = "SUP- " + "(" + SqlFunctions.StringConvert((decimal)conmast.MAST_CON_ID).Trim() + ") " + ((conmast.MAST_CON_FNAME == null ? string.Empty : conmast.MAST_CON_FNAME) + " " + (conmast.MAST_CON_MNAME == null ? string.Empty : conmast.MAST_CON_MNAME) + " " + (conmast.MAST_CON_LNAME == null ? string.Empty : conmast.MAST_CON_LNAME)) + "( " + conmast.MAST_CON_COMPANY_NAME + ")" + " " + "TAN( " + (conmast.MAST_CON_PAN == null ? "-" : conmast.MAST_CON_PAN) + " )",
                                                                                                               MAST_CON_ID = conmast.MAST_CON_ID,
                                                                                                               MAST_CON_SUP_FLAG = conmast.MAST_CON_SUP_FLAG
                                                                                                           }
                                                                                                               ).ToList().Distinct();


                        //addition by Koustubh Nakate on 29/08/2013 to add proposal contractors from shifted tracking
                        var lstContractorByPropasalTracking = (from propTracking in dbContext.IMS_PROPOSAL_TRACKING
                                                               join aggdet in dbContext.TEND_AGREEMENT_DETAIL
                                                               on propTracking.IMS_PR_ROAD_CODE equals aggdet.IMS_PR_ROAD_CODE
                                                               join aggmast in dbContext.TEND_AGREEMENT_MASTER
                                                               on aggdet.TEND_AGREEMENT_CODE equals aggmast.TEND_AGREEMENT_CODE
                                                               join con in dbContext.MASTER_CONTRACTOR
                                                               on aggmast.MAST_CON_ID equals con.MAST_CON_ID
                                                               where propTracking.MAST_STATE_CODE == objParam.STATE_CODE
                                                               orderby con.MAST_CON_COMPANY_NAME ascending
                                                               select new
                                                               {
                                                                   MAST_CON_COMPANY_NAME = con.MAST_CON_SUP_FLAG == "C" ?
                                                                   "CON- " + "(" + SqlFunctions.StringConvert((decimal)con.MAST_CON_ID).Trim() + ") "
                                                                       + ((con.MAST_CON_FNAME == null ? string.Empty : con.MAST_CON_FNAME) + " " + (con.MAST_CON_MNAME == null ? string.Empty : con.MAST_CON_MNAME) + " " + (con.MAST_CON_LNAME == null ? string.Empty : con.MAST_CON_LNAME))
                                                                       + "( " + con.MAST_CON_COMPANY_NAME + ")" + " " + "PAN( " + (con.MAST_CON_PAN == null ? "-" : con.MAST_CON_PAN) + " )"
                                                                       : "SUP- " + "(" + SqlFunctions.StringConvert((decimal)con.MAST_CON_ID).Trim() + ") "
                                                                       + ((con.MAST_CON_FNAME == null ? string.Empty : con.MAST_CON_FNAME) + " " + (con.MAST_CON_MNAME == null ? string.Empty : con.MAST_CON_MNAME) + " " + (con.MAST_CON_LNAME == null ? string.Empty : con.MAST_CON_LNAME))
                                                                       + "( " + con.MAST_CON_COMPANY_NAME + ")" + " " + "TAN( " + (con.MAST_CON_PAN == null ? "-" : con.MAST_CON_PAN) + " )",
                                                                   MAST_CON_ID = con.MAST_CON_ID,
                                                                   MAST_CON_SUP_FLAG = con.MAST_CON_SUP_FLAG
                                                               }).ToList().Distinct();


                        lstContractor = lstContractor.Union(lstContractorByPropasalTracking).ToList().Distinct();

                        if (objParam.MAST_CON_SUP_FLAG == "C")
                        {

                            lstContractor = lstContractor.Where(x => x.MAST_CON_SUP_FLAG == "C" || x.MAST_CON_SUP_FLAG == "R");

                            // get the DPR list and concat it with contractor Change date 13/08/2013

                            var lstDrp = (from tm in dbContext.TEND_AGREEMENT_MASTER
                                          join cn in dbContext.MASTER_CONTRACTOR on tm.MAST_CON_ID equals cn.MAST_CON_ID
                                          where
                                              // tm.MAST_DISTRICT_CODE == 441
                                          tm.MAST_STATE_CODE == objParam.STATE_CODE
                                          && tm.TEND_AGREEMENT_TYPE == "D"
                                          orderby cn.MAST_CON_COMPANY_NAME ascending
                                          select new
                                          {
                                              MAST_CON_COMPANY_NAME = "DPR- " + "(" + SqlFunctions.StringConvert((decimal)cn.MAST_CON_ID).Trim() + ") "
                                              + ((cn.MAST_CON_FNAME == null ? string.Empty : cn.MAST_CON_FNAME) + " " + (cn.MAST_CON_MNAME == null ? string.Empty : cn.MAST_CON_MNAME) + " " + (cn.MAST_CON_LNAME == null ? string.Empty : cn.MAST_CON_LNAME))
                                              + "(" + cn.MAST_CON_COMPANY_NAME + ")",
                                              MAST_CON_ID = cn.MAST_CON_ID,
                                              MAST_CON_SUP_FLAG = cn.MAST_CON_SUP_FLAG
                                          }).ToList().Distinct();


                            if (lstDrp != null && lstDrp.Count() != 0)
                            {
                                lstDrp = lstDrp.Except(lstContractor);
                                lstContractor = lstContractor.Concat(lstDrp);
                            }


                        }

                        if (objParam.MAST_CON_SUP_FLAG == "S")
                        {

                            lstContractor = lstContractor.Where(x => x.MAST_CON_SUP_FLAG == "S");

                        }



                        if (lstContractor == null || lstContractor.Count() == 0)
                        {
                            if (objParam.MAST_CON_SUP_FLAG == "C")
                            {
                                lstContDetails.Insert(0, (new SelectListItem { Text = "--Select--", Value = "0", Selected = true }));
                            }
                            else
                            {
                                lstContDetails.Insert(0, (new SelectListItem { Text = "Select Supplier", Value = "0", Selected = true }));
                            }
                            return lstContDetails;
                        }
                        else
                        {
                            foreach (var item in lstContractor)
                            {
                                if (item.MAST_CON_ID == objParam.MAST_CONT_ID)
                                {
                                    lstContDetails.Add(new SelectListItem { Text = item.MAST_CON_COMPANY_NAME, Value = item.MAST_CON_ID.ToString(), Selected = true });
                                }
                                else
                                {
                                    lstContDetails.Add(new SelectListItem { Text = item.MAST_CON_COMPANY_NAME, Value = item.MAST_CON_ID.ToString() });
                                }
                            }
                        }
                        lstContDetails = lstContDetails.OrderBy(m => m.Text).ToList();
                        if (objParam.MAST_CON_SUP_FLAG == "C")
                        {
                            lstContDetails.Insert(0, (new SelectListItem { Text = "--Select--", Value = "0" }));
                        }
                        else
                        {
                            lstContDetails.Insert(0, (new SelectListItem { Text = "Select Supplier", Value = "0" }));
                        }
                    }
                    // for admin fund added on 18-11-2021
                    else
                    {
                        var lstContractor =
                             (from CN in dbContext.MASTER_CONTRACTOR
                              join BK in dbContext.MASTER_CONTRACTOR_BANK
                              on CN.MAST_CON_ID equals BK.MAST_CON_ID
                              join RN in dbContext.MASTER_CONTRACTOR_REGISTRATION
                              on CN.MAST_CON_ID equals RN.MAST_CON_ID
                              //where CN.MAST_STATE_CODE_ADDR == PMGSYSession.Current.StateCode &&
                              //Below Line added on 28-12-2021 to get contr. list as per contractor registration state
                              where RN.MAST_REG_STATE == PMGSYSession.Current.StateCode &&
                              CN.MAST_CON_STATUS == "A" && RN.FUND_TYPE == "A"
                              select new
                              {
                                  //MAST_CON_COMPANY_NAME = ((CN.MAST_CON_FNAME == null ? string.Empty : CN.MAST_CON_FNAME) + " " + (CN.MAST_CON_MNAME == null ? string.Empty :
                                  //                          CN.MAST_CON_MNAME) + " " + (CN.MAST_CON_LNAME == null ? string.Empty : CN.MAST_CON_LNAME))
                                  //                          + "( " + CN.MAST_CON_COMPANY_NAME + ")" + "TAN( " + (CN.MAST_CON_PAN == null ? "-" : CN.MAST_CON_PAN) + " )",
                                  MAST_CON_COMPANY_NAME =  CN.MAST_CON_SUP_FLAG == "C" ?
                                                           "CON- " + "(" + SqlFunctions.StringConvert((decimal)RN.MAST_CON_ID).Trim() + ") "
                                                            + ((CN.MAST_CON_FNAME == null ? string.Empty : CN.MAST_CON_FNAME) + " " + (CN.MAST_CON_MNAME == null ? string.Empty :
                                                            CN.MAST_CON_MNAME) + " " + (CN.MAST_CON_LNAME == null ? string.Empty : CN.MAST_CON_LNAME))
                                                            + "( " + CN.MAST_CON_COMPANY_NAME + ")" + "TAN( " + (CN.MAST_CON_PAN == null ? "-" : CN.MAST_CON_PAN) + " )"
                                                            :"SUP- " + "(" + SqlFunctions.StringConvert((decimal)RN.MAST_CON_ID).Trim() + ") "
                                                            + ((CN.MAST_CON_FNAME == null ? string.Empty : CN.MAST_CON_FNAME) + " " + (CN.MAST_CON_MNAME == null ? string.Empty :
                                                            CN.MAST_CON_MNAME) + " " + (CN.MAST_CON_LNAME == null ? string.Empty : CN.MAST_CON_LNAME))
                                                            + "( " + CN.MAST_CON_COMPANY_NAME + ")" + "TAN( " + (CN.MAST_CON_PAN == null ? "-" : CN.MAST_CON_PAN) + " )" ,
                                                            
                                  MAST_CON_ID = CN.MAST_CON_ID,
                                  MAST_CON_SUP_FLAG = CN.MAST_CON_SUP_FLAG
                              }).ToList().Distinct();

                        #region Contractors List from 'Program Fund'
                        //Below Code Added on 22-12-2021 
                        var lstContractor_P = objParam.MAST_CON_SUP_FLAG == "C" ? (from sancprj in dbContext.IMS_SANCTIONED_PROJECTS
                                                                                   join aggdet in dbContext.TEND_AGREEMENT_DETAIL on sancprj.IMS_PR_ROAD_CODE equals aggdet.IMS_PR_ROAD_CODE
                                                                                   join aggmast in dbContext.TEND_AGREEMENT_MASTER on aggdet.TEND_AGREEMENT_CODE equals aggmast.TEND_AGREEMENT_CODE
                                                                                   join con in dbContext.MASTER_CONTRACTOR on aggmast.MAST_CON_ID equals con.MAST_CON_ID
                                                                                   where sancprj.MAST_STATE_CODE == objParam.STATE_CODE
                                                                                   //&& con.MAST_CON_SUP_FLAG == objParam.MAST_CON_SUP_FLAG
                                                                                   orderby con.MAST_CON_COMPANY_NAME ascending
                                                                                   select new
                                                                                   {
                                                                                       MAST_CON_COMPANY_NAME = con.MAST_CON_SUP_FLAG == "C" ?
                                                                                       "CON- " + "(" + SqlFunctions.StringConvert((decimal)con.MAST_CON_ID).Trim() + ") "
                                                                                           + ((con.MAST_CON_FNAME == null ? string.Empty : con.MAST_CON_FNAME) + " " + (con.MAST_CON_MNAME == null ? string.Empty : con.MAST_CON_MNAME) + " " + (con.MAST_CON_LNAME == null ? string.Empty : con.MAST_CON_LNAME))
                                                                                           + "( " + con.MAST_CON_COMPANY_NAME + ")" + "PAN( " + (con.MAST_CON_PAN == null ? "-" : con.MAST_CON_PAN) + " )"
                                                                                           : "SUP- " + "(" + SqlFunctions.StringConvert((decimal)con.MAST_CON_ID).Trim() + ") "
                                                                                           + ((con.MAST_CON_FNAME == null ? string.Empty : con.MAST_CON_FNAME) + " " + (con.MAST_CON_MNAME == null ? string.Empty : con.MAST_CON_MNAME) + " " + (con.MAST_CON_LNAME == null ? string.Empty : con.MAST_CON_LNAME))
                                                                                           + "( " + con.MAST_CON_COMPANY_NAME + ")" + " " + "TAN( " + (con.MAST_CON_PAN == null ? "-" : con.MAST_CON_PAN) + " )",
                                                                                       MAST_CON_ID = con.MAST_CON_ID,
                                                                                       MAST_CON_SUP_FLAG = con.MAST_CON_SUP_FLAG
                                                                                   }).ToList().Distinct() : (from aggmast in dbContext.TEND_AGREEMENT_MASTER
                                                                                                             join conmast in dbContext.MASTER_CONTRACTOR
                                                                                                             on aggmast.MAST_CON_ID equals conmast.MAST_CON_ID
                                                                                                             where
                                                                                                            aggmast.TEND_AGREEMENT_TYPE == "S" &&
                                                                                                            aggmast.MAST_STATE_CODE == objParam.STATE_CODE
                                                                                                             select new
                                                                                                             {
                                                                                                                 MAST_CON_COMPANY_NAME = "SUP- " + "(" + SqlFunctions.StringConvert((decimal)conmast.MAST_CON_ID).Trim() + ") " + ((conmast.MAST_CON_FNAME == null ? string.Empty : conmast.MAST_CON_FNAME) + " " + (conmast.MAST_CON_MNAME == null ? string.Empty : conmast.MAST_CON_MNAME) + " " + (conmast.MAST_CON_LNAME == null ? string.Empty : conmast.MAST_CON_LNAME)) + "( " + conmast.MAST_CON_COMPANY_NAME + ")" + " " + "TAN( " + (conmast.MAST_CON_PAN == null ? "-" : conmast.MAST_CON_PAN) + " )",
                                                                                                                 MAST_CON_ID = conmast.MAST_CON_ID,
                                                                                                                 MAST_CON_SUP_FLAG = conmast.MAST_CON_SUP_FLAG
                                                                                                             }
                                                                                                               ).ToList().Distinct();


                        //addition by Koustubh Nakate on 29/08/2013 to add proposal contractors from shifted tracking
                        var lstContractorByPropasalTracking = (from propTracking in dbContext.IMS_PROPOSAL_TRACKING
                                                               join aggdet in dbContext.TEND_AGREEMENT_DETAIL
                                                               on propTracking.IMS_PR_ROAD_CODE equals aggdet.IMS_PR_ROAD_CODE
                                                               join aggmast in dbContext.TEND_AGREEMENT_MASTER
                                                               on aggdet.TEND_AGREEMENT_CODE equals aggmast.TEND_AGREEMENT_CODE
                                                               join con in dbContext.MASTER_CONTRACTOR
                                                               on aggmast.MAST_CON_ID equals con.MAST_CON_ID
                                                               where propTracking.MAST_STATE_CODE == objParam.STATE_CODE
                                                               orderby con.MAST_CON_COMPANY_NAME ascending
                                                               select new
                                                               {
                                                                   MAST_CON_COMPANY_NAME = con.MAST_CON_SUP_FLAG == "C" ?
                                                                   "CON- " + "(" + SqlFunctions.StringConvert((decimal)con.MAST_CON_ID).Trim() + ") "
                                                                       + ((con.MAST_CON_FNAME == null ? string.Empty : con.MAST_CON_FNAME) + " " + (con.MAST_CON_MNAME == null ? string.Empty : con.MAST_CON_MNAME) + " " + (con.MAST_CON_LNAME == null ? string.Empty : con.MAST_CON_LNAME))
                                                                       + "( " + con.MAST_CON_COMPANY_NAME + ")" + " " + "PAN( " + (con.MAST_CON_PAN == null ? "-" : con.MAST_CON_PAN) + " )"
                                                                       : "SUP- " + "(" + SqlFunctions.StringConvert((decimal)con.MAST_CON_ID).Trim() + ") "
                                                                       + ((con.MAST_CON_FNAME == null ? string.Empty : con.MAST_CON_FNAME) + " " + (con.MAST_CON_MNAME == null ? string.Empty : con.MAST_CON_MNAME) + " " + (con.MAST_CON_LNAME == null ? string.Empty : con.MAST_CON_LNAME))
                                                                       + "( " + con.MAST_CON_COMPANY_NAME + ")" + " " + "TAN( " + (con.MAST_CON_PAN == null ? "-" : con.MAST_CON_PAN) + " )",
                                                                   MAST_CON_ID = con.MAST_CON_ID,
                                                                   MAST_CON_SUP_FLAG = con.MAST_CON_SUP_FLAG
                                                               }).ToList().Distinct();


                        lstContractor_P = lstContractor_P.Union(lstContractorByPropasalTracking).ToList().Distinct();


                        if (objParam.MAST_CON_SUP_FLAG == "C")
                        {

                            lstContractor_P = lstContractor_P.Where(x => x.MAST_CON_SUP_FLAG == "C" || x.MAST_CON_SUP_FLAG == "R");

                            // get the DPR list and concat it with contractor Change date 13/08/2013

                            var lstDrp = (from tm in dbContext.TEND_AGREEMENT_MASTER
                                          join cn in dbContext.MASTER_CONTRACTOR on tm.MAST_CON_ID equals cn.MAST_CON_ID
                                          where
                                              // tm.MAST_DISTRICT_CODE == 441
                                          tm.MAST_STATE_CODE == objParam.STATE_CODE
                                          && tm.TEND_AGREEMENT_TYPE == "D"
                                          orderby cn.MAST_CON_COMPANY_NAME ascending
                                          select new
                                          {
                                              MAST_CON_COMPANY_NAME = "DPR- " + "(" + SqlFunctions.StringConvert((decimal)cn.MAST_CON_ID).Trim() + ") "
                                              + ((cn.MAST_CON_FNAME == null ? string.Empty : cn.MAST_CON_FNAME) + " " + (cn.MAST_CON_MNAME == null ? string.Empty : cn.MAST_CON_MNAME) + " " + (cn.MAST_CON_LNAME == null ? string.Empty : cn.MAST_CON_LNAME))
                                              + "(" + cn.MAST_CON_COMPANY_NAME + ")",
                                              MAST_CON_ID = cn.MAST_CON_ID,
                                              MAST_CON_SUP_FLAG = cn.MAST_CON_SUP_FLAG
                                          }).ToList().Distinct();


                            if (lstDrp != null && lstDrp.Count() != 0)
                            {
                                lstDrp = lstDrp.Except(lstContractor_P);
                                lstContractor_P = lstContractor_P.Concat(lstDrp);
                            }


                        }

                        if (objParam.MAST_CON_SUP_FLAG == "S")
                        {

                            lstContractor_P = lstContractor_P.Where(x => x.MAST_CON_SUP_FLAG == "S");

                        }
                        #endregion

                        //Union of Both the contractors from admin fund and program fund

                        lstContractor = lstContractor.Union(lstContractor_P).ToList().Distinct();


                        if (lstContractor == null || lstContractor.Count() == 0)
                        {
                            //lstContDetails.Insert(0, (new SelectListItem { Text = "Other", Value = "-1" }));
                            if (objParam.MAST_CON_SUP_FLAG == "C")
                            {
                                lstContDetails.Insert(0, (new SelectListItem { Text = "--Select--", Value = "0", Selected = true }));
                            }
                            else
                            {
                                lstContDetails.Insert(0, (new SelectListItem { Text = "Select Payee/Supplier", Value = "0", Selected = true }));
                            }
                            return lstContDetails;
                        }
                        else
                        {
                            foreach (var item in lstContractor)
                            {
                                if (item.MAST_CON_ID == objParam.MAST_CONT_ID)
                                {
                                    lstContDetails.Add(new SelectListItem { Text = item.MAST_CON_COMPANY_NAME, Value = item.MAST_CON_ID.ToString(), Selected = true });
                                }
                                else
                                {
                                    lstContDetails.Add(new SelectListItem { Text = item.MAST_CON_COMPANY_NAME, Value = item.MAST_CON_ID.ToString() });
                                }
                            }
                        }
                        lstContDetails = lstContDetails.OrderBy(m => m.Text).ToList();
                        //lstContDetails.Insert(0, (new SelectListItem { Text = "Other", Value = "-1" }));
                        if (objParam.MAST_CON_SUP_FLAG == "C")
                        {
                            lstContDetails.Insert(0, (new SelectListItem { Text = "--Select--", Value = "0" }));
                        }
                        else
                        {
                            lstContDetails.Insert(0, (new SelectListItem { Text = "Select Payee/Supplier", Value = "0" }));
                        }
                    }
                }
                else
                {

                    if (PMGSYSession.Current.FundType.ToLower() == "m")
                    {
                        //change sby Koustubh Nakate on 03/10/2013 to populate supplier properly 

                        var lstContractor = objParam.MAST_CON_SUP_FLAG == "C" ? (from sancprj in dbContext.IMS_SANCTIONED_PROJECTS
                                                                                 join mancon in dbContext.MANE_IMS_CONTRACT on sancprj.IMS_PR_ROAD_CODE equals mancon.IMS_PR_ROAD_CODE
                                                                                 join con in dbContext.MASTER_CONTRACTOR on mancon.MAST_CON_ID equals con.MAST_CON_ID
                                                                                 where sancprj.MAST_DISTRICT_CODE == objParam.DISTRICT_CODE
                                                                                 //&& con.MAST_CON_SUP_FLAG == objParam.MAST_CON_SUP_FLAG
                                                                                 select new
                                                                                 {
                                                                                     MAST_CON_COMPANY_NAME = con.MAST_CON_SUP_FLAG == "C" ?
                                                                                     "CON- " + "(" + SqlFunctions.StringConvert((decimal)con.MAST_CON_ID).Trim() + ") "
                                                                                           + ((con.MAST_CON_FNAME == null ? string.Empty : con.MAST_CON_FNAME) + " " + (con.MAST_CON_MNAME == null ? string.Empty : con.MAST_CON_MNAME) + " " + (con.MAST_CON_LNAME == null ? string.Empty : con.MAST_CON_LNAME))
                                                                                           + "( " + con.MAST_CON_COMPANY_NAME + ")" + " " + "PAN( " + (con.MAST_CON_PAN == null ? "-" : con.MAST_CON_PAN) + " )"
                                                                                           : "SUP- " + "(" + SqlFunctions.StringConvert((decimal)con.MAST_CON_ID).Trim() + ") "
                                                                                            + ((con.MAST_CON_FNAME == null ? string.Empty : con.MAST_CON_FNAME) + " " + (con.MAST_CON_MNAME == null ? string.Empty : con.MAST_CON_MNAME) + " " + (con.MAST_CON_LNAME == null ? string.Empty : con.MAST_CON_LNAME))
                                                                                          + "( " + con.MAST_CON_COMPANY_NAME + ")" + " " + "TAN( " + (con.MAST_CON_PAN == null ? "-" : con.MAST_CON_PAN) + " )",
                                                                                     MAST_CON_ID = con.MAST_CON_ID,
                                                                                     MAST_CON_SUP_FLAG = con.MAST_CON_SUP_FLAG
                                                                                 }).ToList().Distinct() : (from aggmast in dbContext.TEND_AGREEMENT_MASTER
                                                                                                           join conmast in dbContext.MASTER_CONTRACTOR
                                                                                                           on aggmast.MAST_CON_ID equals conmast.MAST_CON_ID
                                                                                                           where
                                                                                                          aggmast.TEND_AGREEMENT_TYPE == "S" &&
                                                                                                          aggmast.MAST_DISTRICT_CODE == objParam.DISTRICT_CODE
                                                                                                           select new
                                                                                                           {
                                                                                                               MAST_CON_COMPANY_NAME = "SUP- " + "(" + SqlFunctions.StringConvert((decimal)conmast.MAST_CON_ID).Trim() + ") " + ((conmast.MAST_CON_FNAME == null ? string.Empty : conmast.MAST_CON_FNAME) + " " + (conmast.MAST_CON_MNAME == null ? string.Empty : conmast.MAST_CON_MNAME) + " " + (conmast.MAST_CON_LNAME == null ? string.Empty : conmast.MAST_CON_LNAME)) + "( " + conmast.MAST_CON_COMPANY_NAME + ")" + " " + "TAN( " + (conmast.MAST_CON_PAN == null ? "-" : conmast.MAST_CON_PAN) + " )",
                                                                                                               MAST_CON_ID = conmast.MAST_CON_ID,
                                                                                                               MAST_CON_SUP_FLAG = conmast.MAST_CON_SUP_FLAG
                                                                                                           }
                                                                                                               ).ToList().Distinct();

                        //addition by Koustubh Nakate on 29/08/2013 to add proposal contractors from shifted tracking

                        var lstContractorByPropasalTracking = (from propTracking in dbContext.IMS_PROPOSAL_TRACKING
                                                               join mancon in dbContext.MANE_IMS_CONTRACT
                                                               on propTracking.IMS_PR_ROAD_CODE equals mancon.IMS_PR_ROAD_CODE
                                                               join con in dbContext.MASTER_CONTRACTOR
                                                               on mancon.MAST_CON_ID equals con.MAST_CON_ID
                                                               where propTracking.MAST_DISTRICT_CODE == objParam.DISTRICT_CODE
                                                               select new
                                                               {
                                                                   MAST_CON_COMPANY_NAME = con.MAST_CON_SUP_FLAG == "C" ?
                                                                   "CON- " + "(" + SqlFunctions.StringConvert((decimal)con.MAST_CON_ID).Trim() + ") "
                                                                         + ((con.MAST_CON_FNAME == null ? string.Empty : con.MAST_CON_FNAME) + " " + (con.MAST_CON_MNAME == null ? string.Empty : con.MAST_CON_MNAME) + " " + (con.MAST_CON_LNAME == null ? string.Empty : con.MAST_CON_LNAME))
                                                                         + "( " + con.MAST_CON_COMPANY_NAME + ")" + " " + "PAN( " + (con.MAST_CON_PAN == null ? "-" : con.MAST_CON_PAN) + " )"
                                                                         : "SUP- " + "(" + SqlFunctions.StringConvert((decimal)con.MAST_CON_ID).Trim() + ") "
                                                                          + ((con.MAST_CON_FNAME == null ? string.Empty : con.MAST_CON_FNAME) + " " + (con.MAST_CON_MNAME == null ? string.Empty : con.MAST_CON_MNAME) + " " + (con.MAST_CON_LNAME == null ? string.Empty : con.MAST_CON_LNAME))
                                                                        + "( " + con.MAST_CON_COMPANY_NAME + ")" + " " + "TAN( " + (con.MAST_CON_PAN == null ? "-" : con.MAST_CON_PAN) + " )",
                                                                   MAST_CON_ID = con.MAST_CON_ID,
                                                                   MAST_CON_SUP_FLAG = con.MAST_CON_SUP_FLAG
                                                               }).ToList().Distinct();


                        lstContractor = lstContractor.Union(lstContractorByPropasalTracking).ToList().Distinct();




                        if (objParam.MAST_CON_SUP_FLAG == "C")
                        {

                            lstContractor = lstContractor.Where(x => x.MAST_CON_SUP_FLAG == "C" || x.MAST_CON_SUP_FLAG == "R");

                        }

                        if (objParam.MAST_CON_SUP_FLAG == "S")
                        {

                            lstContractor = lstContractor.Where(x => x.MAST_CON_SUP_FLAG == "S");

                        }


                        if (lstContractor == null || lstContractor.Count() == 0)
                        {
                            if (objParam.MAST_CON_SUP_FLAG == "C")
                            {
                                lstContDetails.Insert(0, (new SelectListItem { Text = "--Select--", Value = "0", Selected = true }));
                            }
                            else
                            {
                                lstContDetails.Insert(0, (new SelectListItem { Text = "Select Supplier", Value = "0", Selected = true }));
                            }
                            return lstContDetails;
                        }
                        else
                        {
                            foreach (var item in lstContractor)
                            {
                                if (item.MAST_CON_ID == objParam.MAST_CONT_ID)
                                {
                                    lstContDetails.Add(new SelectListItem { Text = item.MAST_CON_COMPANY_NAME, Value = item.MAST_CON_ID.ToString(), Selected = true });
                                }
                                else
                                {
                                    lstContDetails.Add(new SelectListItem { Text = item.MAST_CON_COMPANY_NAME, Value = item.MAST_CON_ID.ToString() });
                                }
                            }
                        }
                        lstContDetails = lstContDetails.OrderBy(m => m.Text).ToList();
                        if (objParam.MAST_CON_SUP_FLAG == "C")
                        {
                            lstContDetails.Insert(0, (new SelectListItem { Text = "--Select--", Value = "0" }));
                        }
                        else
                        {
                            lstContDetails.Insert(0, (new SelectListItem { Text = "Select Supplier", Value = "0" }));
                        }
                    }
                    else if (PMGSYSession.Current.FundType.ToLower() == "a")
                    {
                        //Below code Added on 13-Oct-2021 to get List of contractor for Fund_Type="A"

                        var lstContractor =
                                  (from CN in dbContext.MASTER_CONTRACTOR
                                   join BK in dbContext.MASTER_CONTRACTOR_BANK
                                   on CN.MAST_CON_ID equals BK.MAST_CON_ID
                                   join RN in dbContext.MASTER_CONTRACTOR_REGISTRATION
                                   on CN.MAST_CON_ID equals RN.MAST_CON_ID
                                   //where CN.MAST_STATE_CODE_ADDR == PMGSYSession.Current.StateCode &&
                                   //Below Line added on 28-12-2021 to get contr. list as per contractor registration state
                                   where RN.MAST_REG_STATE == PMGSYSession.Current.StateCode &&
                                   CN.MAST_CON_STATUS == "A" && RN.FUND_TYPE == "A"
                                   select new
                                   {
                                       //MAST_CON_COMPANY_NAME = ((CN.MAST_CON_FNAME == null ? string.Empty : CN.MAST_CON_FNAME) + " " + (CN.MAST_CON_MNAME == null ? string.Empty :
                                       //                          CN.MAST_CON_MNAME) + " " + (CN.MAST_CON_LNAME == null ? string.Empty : CN.MAST_CON_LNAME))
                                       //                          + "( " + CN.MAST_CON_COMPANY_NAME + ")" + "TAN( " + (CN.MAST_CON_PAN == null ? "-" : CN.MAST_CON_PAN) + " )",
                                       MAST_CON_COMPANY_NAME = CN.MAST_CON_SUP_FLAG == "C" ?
                                                           "CON- " + "(" + SqlFunctions.StringConvert((decimal)RN.MAST_CON_ID).Trim() + ") "
                                                            + ((CN.MAST_CON_FNAME == null ? string.Empty : CN.MAST_CON_FNAME) + " " + (CN.MAST_CON_MNAME == null ? string.Empty :
                                                            CN.MAST_CON_MNAME) + " " + (CN.MAST_CON_LNAME == null ? string.Empty : CN.MAST_CON_LNAME))
                                                            + "( " + CN.MAST_CON_COMPANY_NAME + ")" + "TAN( " + (CN.MAST_CON_PAN == null ? "-" : CN.MAST_CON_PAN) + " )"
                                                            : "SUP- " + "(" + SqlFunctions.StringConvert((decimal)RN.MAST_CON_ID).Trim() + ") "
                                                            + ((CN.MAST_CON_FNAME == null ? string.Empty : CN.MAST_CON_FNAME) + " " + (CN.MAST_CON_MNAME == null ? string.Empty :
                                                            CN.MAST_CON_MNAME) + " " + (CN.MAST_CON_LNAME == null ? string.Empty : CN.MAST_CON_LNAME))
                                                            + "( " + CN.MAST_CON_COMPANY_NAME + ")" + "TAN( " + (CN.MAST_CON_PAN == null ? "-" : CN.MAST_CON_PAN) + " )",
                                   
                                       MAST_CON_ID = CN.MAST_CON_ID,
                                       MAST_CON_SUP_FLAG = CN.MAST_CON_SUP_FLAG
                                   }).ToList().Distinct();

                        #region Contracors list from 'Program fund'

                        //Below Code Added on 22-12-2021 
                        var lstContractor_P = objParam.MAST_CON_SUP_FLAG == "C" ? (from sancprj in dbContext.IMS_SANCTIONED_PROJECTS
                                                                                   join aggdet in dbContext.TEND_AGREEMENT_DETAIL on sancprj.IMS_PR_ROAD_CODE equals aggdet.IMS_PR_ROAD_CODE
                                                                                   join aggmast in dbContext.TEND_AGREEMENT_MASTER on aggdet.TEND_AGREEMENT_CODE equals aggmast.TEND_AGREEMENT_CODE
                                                                                   join con in dbContext.MASTER_CONTRACTOR on aggmast.MAST_CON_ID equals con.MAST_CON_ID
                                                                                   where sancprj.MAST_DISTRICT_CODE == objParam.DISTRICT_CODE

                                                                                   //&& con.MAST_CON_SUP_FLAG == objParam.MAST_CON_SUP_FLAG
                                                                                   select new
                                                                                   {
                                                                                       MAST_CON_COMPANY_NAME = con.MAST_CON_SUP_FLAG == "C" ?
                                                                                       "CON- " + "(" + SqlFunctions.StringConvert((decimal)con.MAST_CON_ID).Trim() + ") "
                                                                                             + ((con.MAST_CON_FNAME == null ? string.Empty : con.MAST_CON_FNAME) + " " + (con.MAST_CON_MNAME == null ? string.Empty : con.MAST_CON_MNAME) + " " + (con.MAST_CON_LNAME == null ? string.Empty : con.MAST_CON_LNAME))
                                                                                             + "( " + con.MAST_CON_COMPANY_NAME + ")" + " " + "PAN( " + (con.MAST_CON_PAN == null ? "-" : con.MAST_CON_PAN) + " )"
                                                                                             : "SUP- " + "(" + SqlFunctions.StringConvert((decimal)con.MAST_CON_ID).Trim() + ") "
                                                                                              + ((con.MAST_CON_FNAME == null ? string.Empty : con.MAST_CON_FNAME) + " " + (con.MAST_CON_MNAME == null ? string.Empty : con.MAST_CON_MNAME) + " " + (con.MAST_CON_LNAME == null ? string.Empty : con.MAST_CON_LNAME))
                                                                                            + "( " + con.MAST_CON_COMPANY_NAME + ")" + " " + "TAN( " + (con.MAST_CON_PAN == null ? "-" : con.MAST_CON_PAN) + " )",
                                                                                       MAST_CON_ID = con.MAST_CON_ID,
                                                                                       MAST_CON_SUP_FLAG = con.MAST_CON_SUP_FLAG
                                                                                   }).ToList().Distinct() : (from aggmast in dbContext.TEND_AGREEMENT_MASTER
                                                                                                             join conmast in dbContext.MASTER_CONTRACTOR
                                                                                                             on aggmast.MAST_CON_ID equals conmast.MAST_CON_ID
                                                                                                             where
                                                                                                            aggmast.TEND_AGREEMENT_TYPE == "S" &&
                                                                                                            aggmast.MAST_DISTRICT_CODE == objParam.DISTRICT_CODE
                                                                                                             select new
                                                                                                             {
                                                                                                                 MAST_CON_COMPANY_NAME = "SUP- " + "(" + SqlFunctions.StringConvert((decimal)conmast.MAST_CON_ID).Trim() + ") " + ((conmast.MAST_CON_FNAME == null ? string.Empty : conmast.MAST_CON_FNAME) + " " + (conmast.MAST_CON_MNAME == null ? string.Empty : conmast.MAST_CON_MNAME) + " " + (conmast.MAST_CON_LNAME == null ? string.Empty : conmast.MAST_CON_LNAME)) + "( " + conmast.MAST_CON_COMPANY_NAME + ")" + " " + "TAN( " + (conmast.MAST_CON_PAN == null ? "-" : conmast.MAST_CON_PAN) + " )",
                                                                                                                 MAST_CON_ID = conmast.MAST_CON_ID,
                                                                                                                 MAST_CON_SUP_FLAG = conmast.MAST_CON_SUP_FLAG
                                                                                                             }
                                                                                                               ).ToList().Distinct();



                        //addition by Koustubh Nakate on 29/08/2013 to add proposal contractors from shifted tracking
                        var lstContractorByPropasalTracking = (from propTracking in dbContext.IMS_PROPOSAL_TRACKING
                                                               join aggdet in dbContext.TEND_AGREEMENT_DETAIL
                                                               on propTracking.IMS_PR_ROAD_CODE equals aggdet.IMS_PR_ROAD_CODE
                                                               join aggmast in dbContext.TEND_AGREEMENT_MASTER
                                                               on aggdet.TEND_AGREEMENT_CODE equals aggmast.TEND_AGREEMENT_CODE
                                                               join con in dbContext.MASTER_CONTRACTOR on aggmast.MAST_CON_ID equals con.MAST_CON_ID
                                                               where propTracking.MAST_DISTRICT_CODE == objParam.DISTRICT_CODE
                                                               select new
                                                               {
                                                                   MAST_CON_COMPANY_NAME = con.MAST_CON_SUP_FLAG == "C" ?
                                                                   "CON- " + "(" + SqlFunctions.StringConvert((decimal)con.MAST_CON_ID).Trim() + ") "
                                                                         + ((con.MAST_CON_FNAME == null ? string.Empty : con.MAST_CON_FNAME) + " " + (con.MAST_CON_MNAME == null ? string.Empty : con.MAST_CON_MNAME) + " " + (con.MAST_CON_LNAME == null ? string.Empty : con.MAST_CON_LNAME))
                                                                         + "( " + con.MAST_CON_COMPANY_NAME + ")" + " " + "PAN( " + (con.MAST_CON_PAN == null ? "-" : con.MAST_CON_PAN) + " )"
                                                                         : "SUP- " + "(" + SqlFunctions.StringConvert((decimal)con.MAST_CON_ID).Trim() + ") "
                                                                          + ((con.MAST_CON_FNAME == null ? string.Empty : con.MAST_CON_FNAME) + " " + (con.MAST_CON_MNAME == null ? string.Empty : con.MAST_CON_MNAME) + " " + (con.MAST_CON_LNAME == null ? string.Empty : con.MAST_CON_LNAME))
                                                                        + "( " + con.MAST_CON_COMPANY_NAME + ")" + " " + "TAN( " + (con.MAST_CON_PAN == null ? "-" : con.MAST_CON_PAN) + " )",
                                                                   MAST_CON_ID = con.MAST_CON_ID,
                                                                   MAST_CON_SUP_FLAG = con.MAST_CON_SUP_FLAG
                                                               }).ToList().Distinct();


                        lstContractor_P = lstContractor_P.Union(lstContractorByPropasalTracking).ToList().Distinct();



                        if (objParam.MAST_CON_SUP_FLAG == "C")
                        {
                            //MAST_CON_SUP_FLAG=="S" is checked for TOB to populate suplier for Contractor Agreement. for balance transfer 2Sep2014 by Abhisehk kamble
                            lstContractor_P = lstContractor_P.Where(x => x.MAST_CON_SUP_FLAG == "C" || x.MAST_CON_SUP_FLAG == "R" || x.MAST_CON_SUP_FLAG == "S");

                            // get the DPR list and concat it with contractor Change date 13/08/2013

                            var lstDrp = (from tm in dbContext.TEND_AGREEMENT_MASTER
                                          join cn in dbContext.MASTER_CONTRACTOR on tm.MAST_CON_ID equals cn.MAST_CON_ID
                                          where tm.MAST_DISTRICT_CODE == objParam.DISTRICT_CODE && (tm.TEND_AGREEMENT_TYPE == "D" || tm.TEND_AGREEMENT_TYPE == "S")
                                          orderby cn.MAST_CON_COMPANY_NAME ascending
                                          select new
                                          {
                                              MAST_CON_COMPANY_NAME = cn.MAST_CON_SUP_FLAG == "D" ?
                                              "DPR- " + "(" + SqlFunctions.StringConvert((decimal)cn.MAST_CON_ID).Trim() + ") "
                                                            + ((cn.MAST_CON_FNAME == null ? string.Empty : cn.MAST_CON_FNAME) + " " + (cn.MAST_CON_MNAME == null ? string.Empty : cn.MAST_CON_MNAME) + " " + (cn.MAST_CON_LNAME == null ? string.Empty : cn.MAST_CON_LNAME)) + "(" + cn.MAST_CON_COMPANY_NAME + ")"
                                              : "SUP- " + "(" + SqlFunctions.StringConvert((decimal)cn.MAST_CON_ID).Trim() + ") "
                                                                          + ((cn.MAST_CON_FNAME == null ? string.Empty : cn.MAST_CON_FNAME) + " " + (cn.MAST_CON_MNAME == null ? string.Empty : cn.MAST_CON_MNAME) + " " + (cn.MAST_CON_LNAME == null ? string.Empty : cn.MAST_CON_LNAME)) + "(" + cn.MAST_CON_COMPANY_NAME + ")"
                                              ,
                                              MAST_CON_ID = cn.MAST_CON_ID,
                                              MAST_CON_SUP_FLAG = cn.MAST_CON_SUP_FLAG
                                          }).ToList().Distinct();

                            if (lstDrp != null && lstDrp.Count() != 0)
                            {
                                lstContractor_P = lstContractor_P.Concat(lstDrp);
                            }


                        }



                        if (objParam.MAST_CON_SUP_FLAG == "S")
                        {

                            lstContractor_P = lstContractor_P.Where(x => x.MAST_CON_SUP_FLAG == "S");

                        }



                        #endregion

                        lstContractor = lstContractor.Union(lstContractor_P).ToList().Distinct();


                        if (lstContractor == null || lstContractor.Count() == 0)
                        {
                            //lstContDetails.Insert(0, (new SelectListItem { Text = "Other", Value = "-1" }));
                            if (objParam.MAST_CON_SUP_FLAG == "C")
                            {
                                lstContDetails.Insert(0, (new SelectListItem { Text = "--Select--", Value = "0", Selected = true }));
                            }
                            else
                            {
                                lstContDetails.Insert(0, (new SelectListItem { Text = "Select Payee/Supplier", Value = "0", Selected = true }));
                            }
                            return lstContDetails;
                        }
                        else
                        {
                            foreach (var item in lstContractor)
                            {
                                if (item.MAST_CON_ID == objParam.MAST_CONT_ID)
                                {
                                    lstContDetails.Add(new SelectListItem { Text = item.MAST_CON_COMPANY_NAME, Value = item.MAST_CON_ID.ToString(), Selected = true });
                                }
                                else
                                {
                                    lstContDetails.Add(new SelectListItem { Text = item.MAST_CON_COMPANY_NAME, Value = item.MAST_CON_ID.ToString() });
                                }
                            }
                        }
                        lstContDetails = lstContDetails.OrderBy(m => m.Text).ToList();
                        //lstContDetails.Insert(0, (new SelectListItem { Text = "Other", Value = "-1" }));
                        if (objParam.MAST_CON_SUP_FLAG == "C")
                        {
                            lstContDetails.Insert(0, (new SelectListItem { Text = "--Select--", Value = "0" }));
                        }
                        else
                        {
                            lstContDetails.Insert(0, (new SelectListItem { Text = "Select Payee/Supplier", Value = "0" }));
                        }

                        //Below code Commeneted on 13-Oct-2021 

                        //if (objParam.TXN_ID == 455)
                        //{
                        //    var lstContractor =
                        //      (from CN in dbContext.MASTER_CONTRACTOR
                        //       join BK in dbContext.MASTER_CONTRACTOR_BANK
                        //       on CN.MAST_CON_ID equals BK.MAST_CON_ID
                        //       where CN.MAST_STATE_CODE_ADDR == PMGSYSession.Current.StateCode &&
                        //       CN.MAST_DISTRICT_CODE_ADDR == objParam.DISTRICT_CODE &&
                        //       CN.MAST_CON_SUP_FLAG == "S" &&
                        //       CN.MAST_CON_STATUS == "A"
                        //       select new
                        //       {
                        //           MAST_CON_COMPANY_NAME = ((CN.MAST_CON_FNAME == null ? string.Empty : CN.MAST_CON_FNAME) + " " + (CN.MAST_CON_MNAME == null ? string.Empty :
                        //                                     CN.MAST_CON_MNAME) + " " + (CN.MAST_CON_LNAME == null ? string.Empty : CN.MAST_CON_LNAME))
                        //                                     + "( " + CN.MAST_CON_COMPANY_NAME + ")" + "TAN( " + (CN.MAST_CON_PAN == null ? "-" : CN.MAST_CON_PAN) + " )",
                        //           MAST_CON_ID = CN.MAST_CON_ID,
                        //           MAST_CON_SUP_FLAG = CN.MAST_CON_SUP_FLAG
                        //       }).ToList().Distinct();


                        //    if (lstContractor == null || lstContractor.Count() == 0)
                        //    {
                        //        lstContDetails.Insert(0, (new SelectListItem { Text = "Other", Value = "-1" }));
                        //        if (objParam.MAST_CON_SUP_FLAG == "C")
                        //        {
                        //            lstContDetails.Insert(0, (new SelectListItem { Text = "--Select--", Value = "0", Selected = true }));
                        //        }
                        //        else
                        //        {
                        //            lstContDetails.Insert(0, (new SelectListItem { Text = "Select Payee/Supplier", Value = "0", Selected = true }));
                        //        }
                        //        return lstContDetails;
                        //    }
                        //    else
                        //    {
                        //        foreach (var item in lstContractor)
                        //        {
                        //            if (item.MAST_CON_ID == objParam.MAST_CONT_ID)
                        //            {
                        //                lstContDetails.Add(new SelectListItem { Text = item.MAST_CON_COMPANY_NAME, Value = item.MAST_CON_ID.ToString(), Selected = true });
                        //            }
                        //            else
                        //            {
                        //                lstContDetails.Add(new SelectListItem { Text = item.MAST_CON_COMPANY_NAME, Value = item.MAST_CON_ID.ToString() });
                        //            }
                        //        }
                        //    }
                        //    lstContDetails = lstContDetails.OrderBy(m => m.Text).ToList();
                        //    lstContDetails.Insert(0, (new SelectListItem { Text = "Other", Value = "-1" }));
                        //    if (objParam.MAST_CON_SUP_FLAG == "C")
                        //    {
                        //        lstContDetails.Insert(0, (new SelectListItem { Text = "--Select--", Value = "0" }));
                        //    }
                        //    else
                        //    {
                        //        lstContDetails.Insert(0, (new SelectListItem { Text = "Select Payee/Supplier", Value = "0" }));
                        //    }

                        //}
                        //else if (objParam.TXN_ID == 472 || objParam.TXN_ID == 415)
                        //{
                        //    var lstContractor =
                        //      (from CN in dbContext.ADMIN_NODAL_OFFICERS
                        //       join BK in dbContext.ADMIN_NO_BANK
                        //       on CN.ADMIN_NO_OFFICER_CODE equals BK.ADMIN_NO_OFFICER_CODE
                        //       where
                        //       CN.MAST_DISTRICT_CODE == objParam.DISTRICT_CODE &&

                        //       CN.ADMIN_ACTIVE_STATUS == "Y"
                        //       select new
                        //       {
                        //           MAST_CON_COMPANY_NAME = ((CN.ADMIN_NO_FNAME == null ? string.Empty : CN.ADMIN_NO_FNAME) + " " + (CN.ADMIN_NO_MNAME == null ? string.Empty :
                        //                                     CN.ADMIN_NO_MNAME) + " " + (CN.ADMIN_NO_LNAME == null ? string.Empty : CN.ADMIN_NO_LNAME))
                        //                                    ,
                        //           MAST_CON_ID = CN.ADMIN_NO_OFFICER_CODE,
                        //           MAST_CON_SUP_FLAG = "S"
                        //       }).ToList().Distinct();


                        //    if (lstContractor == null || lstContractor.Count() == 0)
                        //    {
                        //        if (objParam.MAST_CON_SUP_FLAG == "C")
                        //        {
                        //            lstContDetails.Insert(0, (new SelectListItem { Text = "--Select--", Value = "0", Selected = true }));
                        //        }
                        //        else
                        //        {
                        //            lstContDetails.Insert(0, (new SelectListItem { Text = "Select Payee/Supplier", Value = "0", Selected = true }));
                        //        }
                        //        return lstContDetails;
                        //    }
                        //    else
                        //    {
                        //        foreach (var item in lstContractor)
                        //        {
                        //            if (item.MAST_CON_ID == objParam.MAST_CONT_ID)
                        //            {
                        //                lstContDetails.Add(new SelectListItem { Text = item.MAST_CON_COMPANY_NAME, Value = item.MAST_CON_ID.ToString(), Selected = true });
                        //            }
                        //            else
                        //            {
                        //                lstContDetails.Add(new SelectListItem { Text = item.MAST_CON_COMPANY_NAME, Value = item.MAST_CON_ID.ToString() });
                        //            }
                        //        }
                        //    }
                        //    lstContDetails = lstContDetails.OrderBy(m => m.Text).ToList();
                        //    lstContDetails.Insert(0, (new SelectListItem { Text = "Other", Value = "-1" }));
                        //    if (objParam.MAST_CON_SUP_FLAG == "C")
                        //    {
                        //        lstContDetails.Insert(0, (new SelectListItem { Text = "--Select--", Value = "0" }));
                        //    }
                        //    else
                        //    {
                        //        lstContDetails.Insert(0, (new SelectListItem { Text = "Select Payee/Supplier", Value = "0" }));
                        //    }

                        //    //lstContDetails = lstContDetails.OrderBy(m => m.Text).ToList();
                        //}

                    }
                    else
                    {

                        //change sby Koustubh Nakate on 28/09/2013 to populate supplier properly 

                        var lstContractor = objParam.MAST_CON_SUP_FLAG == "C" ? (from sancprj in dbContext.IMS_SANCTIONED_PROJECTS
                                                                                 join aggdet in dbContext.TEND_AGREEMENT_DETAIL on sancprj.IMS_PR_ROAD_CODE equals aggdet.IMS_PR_ROAD_CODE
                                                                                 join aggmast in dbContext.TEND_AGREEMENT_MASTER on aggdet.TEND_AGREEMENT_CODE equals aggmast.TEND_AGREEMENT_CODE
                                                                                 join con in dbContext.MASTER_CONTRACTOR on aggmast.MAST_CON_ID equals con.MAST_CON_ID
                                                                                 where sancprj.MAST_DISTRICT_CODE == objParam.DISTRICT_CODE

                                                                                 //&& con.MAST_CON_SUP_FLAG == objParam.MAST_CON_SUP_FLAG
                                                                                 select new
                                                                                 {
                                                                                     MAST_CON_COMPANY_NAME = con.MAST_CON_SUP_FLAG == "C" ?
                                                                                     "CON- " + "(" + SqlFunctions.StringConvert((decimal)con.MAST_CON_ID).Trim() + ") "
                                                                                           + ((con.MAST_CON_FNAME == null ? string.Empty : con.MAST_CON_FNAME) + " " + (con.MAST_CON_MNAME == null ? string.Empty : con.MAST_CON_MNAME) + " " + (con.MAST_CON_LNAME == null ? string.Empty : con.MAST_CON_LNAME))
                                                                                           + "( " + con.MAST_CON_COMPANY_NAME + ")" + " " + "PAN( " + (con.MAST_CON_PAN == null ? "-" : con.MAST_CON_PAN) + " )"
                                                                                           : "SUP- " + "(" + SqlFunctions.StringConvert((decimal)con.MAST_CON_ID).Trim() + ") "
                                                                                            + ((con.MAST_CON_FNAME == null ? string.Empty : con.MAST_CON_FNAME) + " " + (con.MAST_CON_MNAME == null ? string.Empty : con.MAST_CON_MNAME) + " " + (con.MAST_CON_LNAME == null ? string.Empty : con.MAST_CON_LNAME))
                                                                                          + "( " + con.MAST_CON_COMPANY_NAME + ")" + " " + "TAN( " + (con.MAST_CON_PAN == null ? "-" : con.MAST_CON_PAN) + " )",
                                                                                     MAST_CON_ID = con.MAST_CON_ID,
                                                                                     MAST_CON_SUP_FLAG = con.MAST_CON_SUP_FLAG
                                                                                 }).ToList().Distinct() : (from aggmast in dbContext.TEND_AGREEMENT_MASTER
                                                                                                           join conmast in dbContext.MASTER_CONTRACTOR
                                                                                                           on aggmast.MAST_CON_ID equals conmast.MAST_CON_ID
                                                                                                           where
                                                                                                          aggmast.TEND_AGREEMENT_TYPE == "S" &&
                                                                                                          aggmast.MAST_DISTRICT_CODE == objParam.DISTRICT_CODE
                                                                                                           select new
                                                                                                           {
                                                                                                               MAST_CON_COMPANY_NAME = "SUP- " + "(" + SqlFunctions.StringConvert((decimal)conmast.MAST_CON_ID).Trim() + ") " + ((conmast.MAST_CON_FNAME == null ? string.Empty : conmast.MAST_CON_FNAME) + " " + (conmast.MAST_CON_MNAME == null ? string.Empty : conmast.MAST_CON_MNAME) + " " + (conmast.MAST_CON_LNAME == null ? string.Empty : conmast.MAST_CON_LNAME)) + "( " + conmast.MAST_CON_COMPANY_NAME + ")" + " " + "TAN( " + (conmast.MAST_CON_PAN == null ? "-" : conmast.MAST_CON_PAN) + " )",
                                                                                                               MAST_CON_ID = conmast.MAST_CON_ID,
                                                                                                               MAST_CON_SUP_FLAG = conmast.MAST_CON_SUP_FLAG
                                                                                                           }
                                                                                                               ).ToList().Distinct();



                        //addition by Koustubh Nakate on 29/08/2013 to add proposal contractors from shifted tracking
                        var lstContractorByPropasalTracking = (from propTracking in dbContext.IMS_PROPOSAL_TRACKING
                                                               join aggdet in dbContext.TEND_AGREEMENT_DETAIL
                                                               on propTracking.IMS_PR_ROAD_CODE equals aggdet.IMS_PR_ROAD_CODE
                                                               join aggmast in dbContext.TEND_AGREEMENT_MASTER
                                                               on aggdet.TEND_AGREEMENT_CODE equals aggmast.TEND_AGREEMENT_CODE
                                                               join con in dbContext.MASTER_CONTRACTOR on aggmast.MAST_CON_ID equals con.MAST_CON_ID
                                                               where propTracking.MAST_DISTRICT_CODE == objParam.DISTRICT_CODE
                                                               select new
                                                               {
                                                                   MAST_CON_COMPANY_NAME = con.MAST_CON_SUP_FLAG == "C" ?
                                                                   "CON- " + "(" + SqlFunctions.StringConvert((decimal)con.MAST_CON_ID).Trim() + ") "
                                                                         + ((con.MAST_CON_FNAME == null ? string.Empty : con.MAST_CON_FNAME) + " " + (con.MAST_CON_MNAME == null ? string.Empty : con.MAST_CON_MNAME) + " " + (con.MAST_CON_LNAME == null ? string.Empty : con.MAST_CON_LNAME))
                                                                         + "( " + con.MAST_CON_COMPANY_NAME + ")" + " " + "PAN( " + (con.MAST_CON_PAN == null ? "-" : con.MAST_CON_PAN) + " )"
                                                                         : "SUP- " + "(" + SqlFunctions.StringConvert((decimal)con.MAST_CON_ID).Trim() + ") "
                                                                          + ((con.MAST_CON_FNAME == null ? string.Empty : con.MAST_CON_FNAME) + " " + (con.MAST_CON_MNAME == null ? string.Empty : con.MAST_CON_MNAME) + " " + (con.MAST_CON_LNAME == null ? string.Empty : con.MAST_CON_LNAME))
                                                                        + "( " + con.MAST_CON_COMPANY_NAME + ")" + " " + "TAN( " + (con.MAST_CON_PAN == null ? "-" : con.MAST_CON_PAN) + " )",
                                                                   MAST_CON_ID = con.MAST_CON_ID,
                                                                   MAST_CON_SUP_FLAG = con.MAST_CON_SUP_FLAG
                                                               }).ToList().Distinct();


                        lstContractor = lstContractor.Union(lstContractorByPropasalTracking).ToList().Distinct();



                        if (objParam.MAST_CON_SUP_FLAG == "C")
                        {
                            //MAST_CON_SUP_FLAG=="S" is checked for TOB to populate suplier for Contractor Agreement. for balance transfer 2Sep2014 by Abhisehk kamble
                            lstContractor = lstContractor.Where(x => x.MAST_CON_SUP_FLAG == "C" || x.MAST_CON_SUP_FLAG == "R" || x.MAST_CON_SUP_FLAG == "S");

                            // get the DPR list and concat it with contractor Change date 13/08/2013

                            var lstDrp = (from tm in dbContext.TEND_AGREEMENT_MASTER
                                          join cn in dbContext.MASTER_CONTRACTOR on tm.MAST_CON_ID equals cn.MAST_CON_ID
                                          where tm.MAST_DISTRICT_CODE == objParam.DISTRICT_CODE && (tm.TEND_AGREEMENT_TYPE == "D" || tm.TEND_AGREEMENT_TYPE == "S")
                                          orderby cn.MAST_CON_COMPANY_NAME ascending
                                          select new
                                          {
                                              MAST_CON_COMPANY_NAME = cn.MAST_CON_SUP_FLAG == "D" ?
                                              "DPR- " + "(" + SqlFunctions.StringConvert((decimal)cn.MAST_CON_ID).Trim() + ") "
                                                            + ((cn.MAST_CON_FNAME == null ? string.Empty : cn.MAST_CON_FNAME) + " " + (cn.MAST_CON_MNAME == null ? string.Empty : cn.MAST_CON_MNAME) + " " + (cn.MAST_CON_LNAME == null ? string.Empty : cn.MAST_CON_LNAME)) + "(" + cn.MAST_CON_COMPANY_NAME + ")"
                                              : "SUP- " + "(" + SqlFunctions.StringConvert((decimal)cn.MAST_CON_ID).Trim() + ") "
                                                                          + ((cn.MAST_CON_FNAME == null ? string.Empty : cn.MAST_CON_FNAME) + " " + (cn.MAST_CON_MNAME == null ? string.Empty : cn.MAST_CON_MNAME) + " " + (cn.MAST_CON_LNAME == null ? string.Empty : cn.MAST_CON_LNAME)) + "(" + cn.MAST_CON_COMPANY_NAME + ")"
                                              ,
                                              MAST_CON_ID = cn.MAST_CON_ID,
                                              MAST_CON_SUP_FLAG = cn.MAST_CON_SUP_FLAG
                                          }).ToList().Distinct();

                            if (lstDrp != null && lstDrp.Count() != 0)
                            {
                                lstContractor = lstContractor.Concat(lstDrp);
                            }


                        }



                        if (objParam.MAST_CON_SUP_FLAG == "S")
                        {

                            lstContractor = lstContractor.Where(x => x.MAST_CON_SUP_FLAG == "S");

                        }





                        if (lstContractor == null || lstContractor.Count() == 0)
                        {
                            if (objParam.MAST_CON_SUP_FLAG == "C")
                            {
                                lstContDetails.Insert(0, (new SelectListItem { Text = "--Select--", Value = "0", Selected = true }));
                            }
                            else
                            {
                                lstContDetails.Insert(0, (new SelectListItem { Text = "Select Supplier", Value = "0", Selected = true }));
                            }
                            return lstContDetails;
                        }
                        else
                        {
                            foreach (var item in lstContractor)
                            {
                                if (item.MAST_CON_ID == objParam.MAST_CONT_ID)
                                {
                                    lstContDetails.Add(new SelectListItem { Text = item.MAST_CON_COMPANY_NAME, Value = item.MAST_CON_ID.ToString(), Selected = true });
                                }
                                else
                                {
                                    lstContDetails.Add(new SelectListItem { Text = item.MAST_CON_COMPANY_NAME, Value = item.MAST_CON_ID.ToString() });
                                }
                            }
                            lstContDetails = lstContDetails.OrderBy(m => m.Text).ToList();
                            if (objParam.MAST_CON_SUP_FLAG == "C")
                            {
                                lstContDetails.Insert(0, (new SelectListItem { Text = "--Select--", Value = "0" }));
                            }
                            else
                            {
                                lstContDetails.Insert(0, (new SelectListItem { Text = "Select Supplier", Value = "0" }));
                            }
                        }
                    }
                }

                return lstContDetails;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        //added by hrishikesh for Security Deposit ACC OPening Balance Entry start
        public List<SelectListItem> PopulateContractorSupplierNew(TransactionParams objParam)
        {
            try
            {
                dbContext = new PMGSYEntities();

                List<SelectListItem> lstContDetails = new List<SelectListItem>();

                var stateCode = PMGSYSession.Current.StateCode;
                var contractorList = (from MC in dbContext.MASTER_CONTRACTOR
                                      join MCR in dbContext.MASTER_CONTRACTOR_REGISTRATION
                                      on MC.MAST_CON_ID equals MCR.MAST_CON_ID
                                      where MCR.MAST_REG_STATE == stateCode
                                      select new
                                      {

                                          MAST_CON_COMPANY_NAME = MC.MAST_CON_SUP_FLAG == "C" ?
                                                                                    "CON- " + "(" + SqlFunctions.StringConvert((decimal)MC.MAST_CON_ID).Trim() + ") "
                                                                                       + ((MC.MAST_CON_FNAME == null ? string.Empty : MC.MAST_CON_FNAME) + " " + (MC.MAST_CON_MNAME == null ? string.Empty : MC.MAST_CON_MNAME) + " " + (MC.MAST_CON_LNAME == null ? string.Empty : MC.MAST_CON_LNAME))
                                                                                       + "( " + MC.MAST_CON_COMPANY_NAME + ")" + "PAN( " + (MC.MAST_CON_PAN == null ? "-" : MC.MAST_CON_PAN) + " )"
                                                                                       : "SUP- " + "(" + SqlFunctions.StringConvert((decimal)MC.MAST_CON_ID).Trim() + ") "
                                                                                        + ((MC.MAST_CON_FNAME == null ? string.Empty : MC.MAST_CON_FNAME) + " " + (MC.MAST_CON_MNAME == null ? string.Empty : MC.MAST_CON_MNAME) + " " + (MC.MAST_CON_LNAME == null ? string.Empty : MC.MAST_CON_LNAME))
                                                                                      + "( " + MC.MAST_CON_COMPANY_NAME + ")" + "TAN( " + (MC.MAST_CON_PAN == null ? "-" : MC.MAST_CON_PAN) + " )",
                                          MAST_CON_ID = MC.MAST_CON_ID,
                                          MAST_CON_SUP_FLAG = MC.MAST_CON_SUP_FLAG

                                      }).ToList().Distinct();

                if (contractorList == null || contractorList.Count() == 0)
                {

                    lstContDetails.Insert(0, (new SelectListItem { Text = "--Select--", Value = "0", Selected = true }));

                    return lstContDetails;
                }
                else
                {
                    foreach (var item in contractorList)
                    {
                        /*  if (item.MAST_CON_ID == objParam.MAST_CONT_ID)
                          {
                              lstContDetails.Add(new SelectListItem { Text = item.MAST_CON_COMPANY_NAME, Value = item.MAST_CON_ID.ToString(), Selected = true });
                          }
                          else
                          {*/
                        lstContDetails.Add(new SelectListItem { Text = item.MAST_CON_COMPANY_NAME, Value = item.MAST_CON_ID.ToString() });
                        //}
                    }
                }
                lstContDetails = lstContDetails.OrderBy(m => m.Text).ToList();
                lstContDetails.Insert(0, (new SelectListItem { Text = "--Select--", Value = "0", Selected = true }));

                return lstContDetails;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        //added by hrishikesh end

        public List<SelectListItem> PopulateSancYear(TransactionParams objParam)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<SelectListItem> lstSancYear = new List<SelectListItem>();
                if (PMGSYSession.Current.FundType.ToLower() == "m")
                {
                    var sancYear = (from sancprj in dbContext.IMS_SANCTIONED_PROJECTS
                                    join aggdet in dbContext.MANE_IMS_CONTRACT on sancprj.IMS_PR_ROAD_CODE equals aggdet.IMS_PR_ROAD_CODE
                                    where sancprj.MAST_STATE_CODE == objParam.STATE_CODE && (objParam.DISTRICT_CODE == 0 ? 1 : sancprj.MAST_DISTRICT_CODE) == (objParam.DISTRICT_CODE == 0 ? 1 : objParam.DISTRICT_CODE)
                                    select new
                                    {
                                        IMS_YEAR_TEXT = sancprj.IMS_YEAR,
                                        IMS_YEAR_VALUE = sancprj.IMS_YEAR
                                    }).Distinct().ToList();
                    if (sancYear == null || sancYear.Count() == 0)
                    {
                        lstSancYear.Insert(0, (new SelectListItem { Text = "Select Year", Value = "0", Selected = true }));
                        return lstSancYear;
                    }
                    else
                    {
                        //changes by Koustubh Nakate on 27/09/2013 to Populate financial years
                        foreach (var item in sancYear)
                        {
                            if (item.IMS_YEAR_VALUE == objParam.SANC_YEAR)
                            {
                                // lstSancYear.Insert(0, (new SelectListItem { Text = item.IMS_YEAR_TEXT.ToString(), Value = item.IMS_YEAR_VALUE.ToString(), Selected = true }));
                                lstSancYear.Insert(0, (new SelectListItem { Text = item.IMS_YEAR_TEXT.ToString() + "-" + (item.IMS_YEAR_TEXT + 1).ToString(), Value = item.IMS_YEAR_VALUE.ToString(), Selected = true }));
                            }
                            else
                            {
                                //lstSancYear.Insert(0, (new SelectListItem { Text = item.IMS_YEAR_TEXT.ToString(), Value = item.IMS_YEAR_VALUE.ToString() }));

                                lstSancYear.Insert(0, (new SelectListItem { Text = item.IMS_YEAR_TEXT.ToString() + "-" + (item.IMS_YEAR_TEXT + 1).ToString(), Value = item.IMS_YEAR_VALUE.ToString() }));
                            }
                        }
                    }
                }
                else
                {
                    //var sancYear = (from sancprj in dbContext.IMS_SANCTIONED_PROJECTS
                    //                join aggdet in dbContext.TEND_AGREEMENT_DETAIL on sancprj.IMS_PR_ROAD_CODE equals aggdet.IMS_PR_ROAD_CODE
                    //                join aggmast in dbContext.TEND_AGREEMENT_MASTER on aggdet.TEND_AGREEMENT_CODE equals aggmast.TEND_AGREEMENT_CODE
                    //                where sancprj.MAST_STATE_CODE == objParam.STATE_CODE && (objParam.DISTRICT_CODE == 0 ? 1 : sancprj.MAST_DISTRICT_CODE) == (objParam.DISTRICT_CODE == 0 ? 1 : objParam.DISTRICT_CODE)
                    //                select new
                    //                {
                    //                    IMS_YEAR_TEXT = sancprj.IMS_YEAR,
                    //                    IMS_YEAR_VALUE = sancprj.IMS_YEAR
                    //                }).Distinct().ToList();

                    var sancYear = (from sancprj in dbContext.IMS_SANCTIONED_PROJECTS
                                    //join aggdet in dbContext.TEND_AGREEMENT_DETAIL on sancprj.IMS_PR_ROAD_CODE equals aggdet.IMS_PR_ROAD_CODE
                                    //join aggmast in dbContext.TEND_AGREEMENT_MASTER on aggdet.TEND_AGREEMENT_CODE equals aggmast.TEND_AGREEMENT_CODE
                                    where sancprj.MAST_STATE_CODE == objParam.STATE_CODE && (objParam.DISTRICT_CODE == 0 ? 1 : sancprj.MAST_DISTRICT_CODE) == (objParam.DISTRICT_CODE == 0 ? 1 : objParam.DISTRICT_CODE)
                                    select new
                                    {
                                        IMS_YEAR_TEXT = sancprj.IMS_YEAR,
                                        IMS_YEAR_VALUE = sancprj.IMS_YEAR
                                    }).Distinct().ToList();

                    if (sancYear == null || sancYear.Count() == 0)
                    {
                        lstSancYear.Insert(0, (new SelectListItem { Text = "Select Year", Value = "0", Selected = true }));
                        return lstSancYear;
                    }
                    else
                    {
                        foreach (var item in sancYear)
                        {
                            //changes by Koustubh Nakate on 27/09/2013 to Populate financial years
                            if (item.IMS_YEAR_VALUE == objParam.SANC_YEAR)
                            {
                                //lstSancYear.Insert(0, (new SelectListItem { Text = item.IMS_YEAR_TEXT.ToString(), Value = item.IMS_YEAR_VALUE.ToString(), Selected = true }));


                                lstSancYear.Insert(0, (new SelectListItem { Text = item.IMS_YEAR_TEXT.ToString() + "-" + (item.IMS_YEAR_TEXT + 1).ToString(), Value = item.IMS_YEAR_VALUE.ToString(), Selected = true }));
                            }
                            else
                            {
                                //lstSancYear.Insert(0, (new SelectListItem { Text = item.IMS_YEAR_TEXT.ToString(), Value = item.IMS_YEAR_VALUE.ToString() }));
                                lstSancYear.Insert(0, (new SelectListItem { Text = item.IMS_YEAR_TEXT.ToString() + "-" + (item.IMS_YEAR_TEXT + 1).ToString(), Value = item.IMS_YEAR_VALUE.ToString() }));
                            }
                        }
                    }
                }


                lstSancYear = lstSancYear.OrderByDescending(m => m.Value).ToList();
                lstSancYear.Insert(0, (new SelectListItem { Text = "Select Year", Value = "0" }));
                return lstSancYear;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        //public List<SelectListItem> PopulateRoad(TransactionParams objParam)
        //{
        //    try
        //    {
        //        dbContext = new PMGSYEntities();
        //        List<SelectListItem> lstRoad = new List<SelectListItem>();

        //        if (objParam.AGREEMENT_CODE != 0)
        //        {
        //            var query = (from sancprj in dbContext.IMS_SANCTIONED_PROJECTS
        //                         join aggdet in dbContext.TEND_AGREEMENT_DETAIL on sancprj.IMS_PR_ROAD_CODE equals aggdet.IMS_PR_ROAD_CODE
        //                         join aggmast in dbContext.TEND_AGREEMENT_MASTER on aggdet.TEND_AGREEMENT_CODE equals aggmast.TEND_AGREEMENT_CODE
        //                         where aggmast.TEND_AGREEMENT_CODE == objParam.AGREEMENT_CODE
        //                         select new
        //                         {
        //                             IMS_ROAD_NAME = sancprj.IMS_ROAD_NAME,
        //                             IMS_PR_ROAD_CODE = sancprj.IMS_PR_ROAD_CODE
        //                         }).ToList().Distinct();
        //            if (query == null || query.Count() == 0)
        //            {
        //                lstRoad.Insert(0, (new SelectListItem { Text = "Select Road", Value = "0", Selected = true }));
        //                return lstRoad;
        //            }
        //            foreach (var item in query)
        //            {
        //                if (item.IMS_PR_ROAD_CODE == objParam.ROAD_CODE)
        //                {
        //                    lstRoad.Add(new SelectListItem { Text = item.IMS_ROAD_NAME, Value = item.IMS_PR_ROAD_CODE.ToString(), Selected= true });
        //                }
        //                else
        //                {
        //                    lstRoad.Add(new SelectListItem { Text = item.IMS_ROAD_NAME, Value = item.IMS_PR_ROAD_CODE.ToString() });
        //                }
        //            }

        //            lstRoad = lstRoad.OrderBy(m => m.Text).ToList();
        //            lstRoad.Insert(0, (new SelectListItem { Text = "Select Road", Value = "0" }));
        //            return lstRoad;
        //        }
        //        else if (objParam.PACKAGE_ID != "0")
        //        {
        //            var query = (from sancprj in dbContext.IMS_SANCTIONED_PROJECTS
        //                         join aggdet in dbContext.TEND_AGREEMENT_DETAIL on sancprj.IMS_PR_ROAD_CODE equals aggdet.IMS_PR_ROAD_CODE
        //                         join aggmast in dbContext.TEND_AGREEMENT_MASTER on aggdet.TEND_AGREEMENT_CODE equals aggmast.TEND_AGREEMENT_CODE
        //                         where sancprj.IMS_PACKAGE_ID == objParam.PACKAGE_ID
        //                         select new
        //                         {
        //                             IMS_ROAD_NAME = sancprj.IMS_ROAD_NAME,
        //                             IMS_PR_ROAD_CODE = sancprj.IMS_PR_ROAD_CODE
        //                         }).ToList().Distinct();
        //            if (query == null || query.Count() == 0)
        //            {
        //                lstRoad.Insert(0, (new SelectListItem { Text = "Select Road", Value = "0", Selected = true }));
        //                return lstRoad;
        //            }
        //            foreach (var item in query)
        //            {
        //                lstRoad.Add(new SelectListItem { Text = item.IMS_ROAD_NAME, Value = item.IMS_PR_ROAD_CODE.ToString() });
        //            }

        //            lstRoad = lstRoad.OrderBy(m => m.Text).ToList();
        //            lstRoad.Insert(0, (new SelectListItem { Text = "Select Road", Value = "0", Selected = true }));
        //            return lstRoad;
        //        }
        //        //for all roads who dont have agreement i.e TEND_AGREEMENT_TYPE ="O"
        //        else
        //        {
        //            var query = (from sancprj in dbContext.IMS_SANCTIONED_PROJECTS
        //                         join aggdet in dbContext.TEND_AGREEMENT_DETAIL on sancprj.IMS_PR_ROAD_CODE equals aggdet.IMS_PR_ROAD_CODE
        //                         join aggmast in dbContext.TEND_AGREEMENT_MASTER on aggdet.TEND_AGREEMENT_CODE equals aggmast.TEND_AGREEMENT_CODE
        //                         where
        //                         aggmast.TEND_AGREEMENT_TYPE.ToLower() == "o"
        //                         orderby sancprj.IMS_ROAD_NAME ascending
        //                         select new
        //                         {
        //                             IMS_ROAD_NAME = sancprj.IMS_ROAD_NAME,
        //                             IMS_PR_ROAD_CODE = sancprj.IMS_PR_ROAD_CODE
        //                         }
        //                                ).ToList().Distinct();
        //            if (query == null || query.Count() == 0)
        //            {
        //                lstRoad.Insert(0, (new SelectListItem { Text = "Select Road", Value = "0", Selected = true }));
        //                return lstRoad;
        //            }
        //            foreach (var item in query)
        //            {
        //                lstRoad.Add(new SelectListItem { Text = item.IMS_ROAD_NAME, Value = item.IMS_PR_ROAD_CODE.ToString() });
        //            }

        //            lstRoad = lstRoad.OrderBy(m => m.Text).ToList();
        //            lstRoad.Insert(0, (new SelectListItem { Text = "Select Road", Value = "0", Selected = true }));
        //            return lstRoad;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        dbContext.Dispose();
        //    }
        //}



        //new changed method added by Vikram

        public List<SelectListItem> PopulateRoad(TransactionParams objParam)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<SelectListItem> lstRoad = new List<SelectListItem>();

                //new change done by Vikram for populating the road in Payment excluding if the TOB has done on it.

                List<int> lstTransactionIDs = new List<int>();

                if (PMGSYSession.Current.FundType.Equals("P"))
                {

                    lstTransactionIDs = new List<int>();
                    //lstTransactionIDs.Add(163);            //Commented By Abhishek 16 Sep 2014 Transaction Id commented To Populate Roads for 'Balance Transfer between two DPIUs of same district ' and 'Balance Transfer within PIU between Agreements'
                    lstTransactionIDs.Add(164);
                    lstTransactionIDs.Add(165);
                    //                    lstTransactionIDs.Add(1187);

                }
                else if (PMGSYSession.Current.FundType.Equals("M"))
                {
                    lstTransactionIDs = new List<int>();
                    //                    lstTransactionIDs.Add(1192);
                    //                    lstTransactionIDs.Add(1193);
                    lstTransactionIDs.Add(1194);
                    lstTransactionIDs.Add(1195);
                }

                //end of change


                //new change done by Vikram on 30-08-2013
                int headId = 0;
                string upgradeConnectFlag = string.Empty;

                if (objParam.TXN_ID == 0)
                {
                    objParam.TXN_ID = objParam.HEAD_ID;
                }

                if (objParam.HEAD_ID == 28 || objParam.HEAD_ID == 29)
                {
                    headId = objParam.HEAD_ID;
                }
                else if (objParam.HEAD_ID != 0 && objParam.HEAD_ID != null)
                {
                    headId = (from masterTransaction in dbContext.ACC_MASTER_TXN
                              join txnMapping in dbContext.ACC_TXN_HEAD_MAPPING
                              on masterTransaction.TXN_ID equals txnMapping.TXN_ID
                              where masterTransaction.TXN_ID == objParam.HEAD_ID &&
                              txnMapping.CREDIT_DEBIT == "D"
                              select txnMapping.HEAD_ID).FirstOrDefault();
                }


                switch (headId)
                {
                    case 28:
                        upgradeConnectFlag = "N";
                        break;
                    case 29:
                        upgradeConnectFlag = "U";
                        break;
                }

                //end of change




                if (objParam.AGREEMENT_CODE != 0)
                {
                    //var query = (from sancprj in dbContext.IMS_SANCTIONED_PROJECTS
                    //             join aggdet in dbContext.TEND_AGREEMENT_DETAIL on sancprj.IMS_PR_ROAD_CODE equals aggdet.IMS_PR_ROAD_CODE
                    //             join aggmast in dbContext.TEND_AGREEMENT_MASTER on aggdet.TEND_AGREEMENT_CODE equals aggmast.TEND_AGREEMENT_CODE
                    //             where aggmast.TEND_AGREEMENT_CODE == objParam.AGREEMENT_CODE &&
                    //             (headId == 28?(sancprj.IMS_UPGRADE_CONNECT == "N"):(headId == 29?(sancprj.IMS_UPGRADE_CONNECT == "U"):"%"=="%")) 
                    //             //(upgradeConnectFlag == string.Empty?"%":sancprj.IMS_UPGRADE_CONNECT) == (upgradeConnectFlag == string.Empty?"%":upgradeConnectFlag)
                    //             select new
                    //             {
                    //                 IMS_ROAD_NAME = sancprj.IMS_ROAD_NAME + (sancprj.IMS_UPGRADE_CONNECT == "U"?"- Upgrade -":(sancprj.IMS_UPGRADE_CONNECT == "N"?"- New -":"")) + "(Agency-"+sancprj.ADMIN_DEPARTMENT.MASTER_AGENCY.MAST_AGENCY_NAME+")",
                    //                 //sancprj.IMS_UPGRADE_CONNECT,
                    //                 IMS_PR_ROAD_CODE = sancprj.IMS_PR_ROAD_CODE
                    //             }).ToList().Distinct();

                    var query = (IEnumerable<dynamic>)null;

                    //var query = (from sancprj in dbContext.IMS_SANCTIONED_PROJECTS
                    //             join aggdet in dbContext.TEND_AGREEMENT_DETAIL on sancprj.IMS_PR_ROAD_CODE equals aggdet.IMS_PR_ROAD_CODE
                    //             join aggmast in dbContext.TEND_AGREEMENT_MASTER on aggdet.TEND_AGREEMENT_CODE equals aggmast.TEND_AGREEMENT_CODE
                    //             where aggmast.TEND_AGREEMENT_CODE == objParam.AGREEMENT_CODE &&
                    //             (headId == 28?(sancprj.IMS_UPGRADE_CONNECT == "N"):(headId == 29?(sancprj.IMS_UPGRADE_CONNECT == "U"):"%"=="%")) 
                    //             //(upgradeConnectFlag == string.Empty?"%":sancprj.IMS_UPGRADE_CONNECT) == (upgradeConnectFlag == string.Empty?"%":upgradeConnectFlag)
                    //             select new
                    //             {
                    //                 IMS_ROAD_NAME = sancprj.IMS_ROAD_NAME + (sancprj.IMS_UPGRADE_CONNECT == "U"?"- Upgrade -":(sancprj.IMS_UPGRADE_CONNECT == "N"?"- New -":"")) + "(Agency-"+sancprj.ADMIN_DEPARTMENT.MASTER_AGENCY.MAST_AGENCY_NAME+")",
                    //                 //sancprj.IMS_UPGRADE_CONNECT,
                    //                 IMS_PR_ROAD_CODE = sancprj.IMS_PR_ROAD_CODE
                    //             }).ToList().Distinct();

                    if (PMGSYSession.Current.FundType == "M")
                    {
                        query = (from sancprj in dbContext.IMS_SANCTIONED_PROJECTS
                                 join aggdet in dbContext.MANE_IMS_CONTRACT on sancprj.IMS_PR_ROAD_CODE equals aggdet.IMS_PR_ROAD_CODE
                                 //join aggmast in dbContext.TEND_AGREEMENT_MASTER on aggdet.TEND_AGREEMENT_CODE equals aggmast.TEND_AGREEMENT_CODE
                                 where aggdet.MANE_AGREEMENT_NUMBER == objParam.AGREEMENT_NUMBER &&
                                 aggdet.MAST_CON_ID == objParam.MAST_CONT_ID && //new change done by Vikram for populating road according to the Maintenance agreement and contractor

                                 (objParam.SANC_YEAR == 0 ? 1 : sancprj.IMS_YEAR) == (objParam.SANC_YEAR == 0 ? 1 : objParam.SANC_YEAR) && //new change done by Vikram as multiple roads are populating on the basis of Package with different sanction year
                                 (headId == 28 ? (sancprj.IMS_UPGRADE_CONNECT == "N") : (headId == 29 ? (sancprj.IMS_UPGRADE_CONNECT == "U") : "%" == "%"))
                                 && sancprj.IMS_SANCTIONED == "Y" //change done by Vikram - Road populated should be Mord Sanctioned.
                                 && sancprj.IMS_YEAR > 1950
                                 select new
                                 {         //Road Name Modified By Abhishek kamble 19-Mar-2014
                                     IMS_ROAD_NAME = (sancprj.IMS_PROPOSAL_TYPE == "P" ? sancprj.IMS_ROAD_NAME : sancprj.IMS_ROAD_NAME + (sancprj.IMS_BRIDGE_NAME == null ? "" : "(" + sancprj.IMS_BRIDGE_NAME + ")")) + (sancprj.IMS_UPGRADE_CONNECT == "U" ? "- Upgrade -" : (sancprj.IMS_UPGRADE_CONNECT == "N" ? "- New -" : "")) + "(Agency-" + sancprj.ADMIN_DEPARTMENT.MASTER_AGENCY.MAST_AGENCY_NAME + ")",
                                     IMS_PR_ROAD_CODE = sancprj.IMS_PR_ROAD_CODE
                                 }).ToList().Distinct();
                    }
                    else
                    {
                        if (objParam.TXN_ID == 1554 || objParam.TXN_ID == 1555 || objParam.TXN_ID == 1556 || objParam.TXN_ID == 1557)
                        {
                            query = (from sancprj in dbContext.IMS_SANCTIONED_PROJECTS
                                     join aggdet in dbContext.TEND_AGREEMENT_DETAIL on sancprj.IMS_PR_ROAD_CODE equals aggdet.IMS_PR_ROAD_CODE
                                     join aggmast in dbContext.TEND_AGREEMENT_MASTER on aggdet.TEND_AGREEMENT_CODE equals aggmast.TEND_AGREEMENT_CODE
                                     where aggmast.TEND_AGREEMENT_CODE == objParam.AGREEMENT_CODE
                                     && sancprj.IMS_SANCTIONED == "D" //change done by Vikram - Road populated should be Mord Sanctioned.
                                     && sancprj.IMS_YEAR > 1950
                                     select new
                                     {     //Road Name Modified By Abhishek kamble 19-Mar-2014
                                         IMS_ROAD_NAME = (sancprj.IMS_PROPOSAL_TYPE == "P" ? sancprj.IMS_ROAD_NAME : sancprj.IMS_ROAD_NAME + (sancprj.IMS_BRIDGE_NAME == null ? "" : "(" + sancprj.IMS_BRIDGE_NAME + ")")) + (sancprj.IMS_UPGRADE_CONNECT == "U" ? "- Upgrade -" : (sancprj.IMS_UPGRADE_CONNECT == "N" ? "- New -" : "")) + "(Agency-" + sancprj.ADMIN_DEPARTMENT.MASTER_AGENCY.MAST_AGENCY_NAME + ")",
                                         IMS_PR_ROAD_CODE = sancprj.IMS_PR_ROAD_CODE
                                     }).ToList().Distinct();
                        }
                        else
                        {
                            if (objParam.TXN_ID == 86 || objParam.TXN_ID == 87 || objParam.TXN_ID == 88 || objParam.TXN_ID == 89 || objParam.TXN_ID == 90 || objParam.TXN_ID == 91)   // Refund of Security Deposit to Contractor
                            {
                                query = (from sancprj in dbContext.IMS_SANCTIONED_PROJECTS
                                         join aggdet in dbContext.TEND_AGREEMENT_DETAIL on sancprj.IMS_PR_ROAD_CODE equals aggdet.IMS_PR_ROAD_CODE
                                         join aggmast in dbContext.TEND_AGREEMENT_MASTER on aggdet.TEND_AGREEMENT_CODE equals aggmast.TEND_AGREEMENT_CODE
                                         where aggmast.TEND_AGREEMENT_CODE == objParam.AGREEMENT_CODE &&
                                     //(headId == 28 ? (sancprj.IMS_UPGRADE_CONNECT == "N") : (headId == 29 ? (sancprj.IMS_UPGRADE_CONNECT == "U") : "%" == "%"))
                                     ///Changed by SAMMED A. PATIL on 20FEB2018 for RCPLWE Head 427,428/ 09JAN2020	for PMGSY3 Head 464
                                     ((headId == 28 || headId == 427) ? (sancprj.IMS_UPGRADE_CONNECT == "N") : ((headId == 29 || headId == 428 || headId == 464 || headId == 465) ? (sancprj.IMS_UPGRADE_CONNECT == "U") : "%" == "%"))
                                         //  && sancprj.IMS_SANCTIONED == "Y" //change done by Vikram - Road populated should be Mord Sanctioned.
                                         // Above Ims_Sanctioned Commented by Saurabh
                                         && sancprj.IMS_YEAR > 1950
                                         select new
                                         {     //Road Name Modified By Abhishek kamble 19-Mar-2014
                                             IMS_ROAD_NAME = (sancprj.IMS_PROPOSAL_TYPE == "P" ? sancprj.IMS_ROAD_NAME : sancprj.IMS_ROAD_NAME + (sancprj.IMS_BRIDGE_NAME == null ? "" : "(" + sancprj.IMS_BRIDGE_NAME + ")")) + (sancprj.IMS_UPGRADE_CONNECT == "U" ? "- Upgrade -" : (sancprj.IMS_UPGRADE_CONNECT == "N" ? "- New -" : "")) + "(Agency-" + sancprj.ADMIN_DEPARTMENT.MASTER_AGENCY.MAST_AGENCY_NAME + ")",
                                             IMS_PR_ROAD_CODE = sancprj.IMS_PR_ROAD_CODE
                                         }).ToList().Distinct();
                            }
                            else
                            {
                                query = (from sancprj in dbContext.IMS_SANCTIONED_PROJECTS
                                         join aggdet in dbContext.TEND_AGREEMENT_DETAIL on sancprj.IMS_PR_ROAD_CODE equals aggdet.IMS_PR_ROAD_CODE
                                         join aggmast in dbContext.TEND_AGREEMENT_MASTER on aggdet.TEND_AGREEMENT_CODE equals aggmast.TEND_AGREEMENT_CODE
                                         where aggmast.TEND_AGREEMENT_CODE == objParam.AGREEMENT_CODE &&
                                     //(headId == 28 ? (sancprj.IMS_UPGRADE_CONNECT == "N") : (headId == 29 ? (sancprj.IMS_UPGRADE_CONNECT == "U") : "%" == "%"))
                                     ///Changed by SAMMED A. PATIL on 20FEB2018 for RCPLWE Head 427,428/ 09JAN2020	for PMGSY3 Head 464
                                     ((headId == 28 || headId == 427) ? (sancprj.IMS_UPGRADE_CONNECT == "N") : ((headId == 29 || headId == 428 || headId == 464 || headId == 465) ? (sancprj.IMS_UPGRADE_CONNECT == "U") : "%" == "%"))
                                         && sancprj.IMS_SANCTIONED == "Y" //change done by Vikram - Road populated should be Mord Sanctioned.
                                         && sancprj.IMS_YEAR > 1950
                                         select new
                                         {     //Road Name Modified By Abhishek kamble 19-Mar-2014
                                             IMS_ROAD_NAME = (sancprj.IMS_PROPOSAL_TYPE == "P" ? sancprj.IMS_ROAD_NAME : sancprj.IMS_ROAD_NAME + (sancprj.IMS_BRIDGE_NAME == null ? "" : "(" + sancprj.IMS_BRIDGE_NAME + ")")) + (sancprj.IMS_UPGRADE_CONNECT == "U" ? "- Upgrade -" : (sancprj.IMS_UPGRADE_CONNECT == "N" ? "- New -" : "")) + "(Agency-" + sancprj.ADMIN_DEPARTMENT.MASTER_AGENCY.MAST_AGENCY_NAME + ")",
                                             IMS_PR_ROAD_CODE = sancprj.IMS_PR_ROAD_CODE
                                         }).ToList().Distinct();
                            }

                            //query = (from sancprj in dbContext.IMS_SANCTIONED_PROJECTS
                            //         join aggdet in dbContext.TEND_AGREEMENT_DETAIL on sancprj.IMS_PR_ROAD_CODE equals aggdet.IMS_PR_ROAD_CODE
                            //         join aggmast in dbContext.TEND_AGREEMENT_MASTER on aggdet.TEND_AGREEMENT_CODE equals aggmast.TEND_AGREEMENT_CODE
                            //         where aggmast.TEND_AGREEMENT_CODE == objParam.AGREEMENT_CODE &&
                            //             //(headId == 28 ? (sancprj.IMS_UPGRADE_CONNECT == "N") : (headId == 29 ? (sancprj.IMS_UPGRADE_CONNECT == "U") : "%" == "%"))
                            //             ///Changed by SAMMED A. PATIL on 20FEB2018 for RCPLWE Head 427,428/ 09JAN2020	for PMGSY3 Head 464
                            //         ((headId == 28 || headId == 427) ? (sancprj.IMS_UPGRADE_CONNECT == "N") : ((headId == 29 || headId == 428 || headId == 464 || headId == 465) ? (sancprj.IMS_UPGRADE_CONNECT == "U") : "%" == "%"))
                            //         && sancprj.IMS_SANCTIONED == "Y" //change done by Vikram - Road populated should be Mord Sanctioned.
                            //         && sancprj.IMS_YEAR > 1950
                            //         select new
                            //         {     //Road Name Modified By Abhishek kamble 19-Mar-2014
                            //             IMS_ROAD_NAME = (sancprj.IMS_PROPOSAL_TYPE == "P" ? sancprj.IMS_ROAD_NAME : sancprj.IMS_ROAD_NAME + (sancprj.IMS_BRIDGE_NAME == null ? "" : "(" + sancprj.IMS_BRIDGE_NAME + ")")) + (sancprj.IMS_UPGRADE_CONNECT == "U" ? "- Upgrade -" : (sancprj.IMS_UPGRADE_CONNECT == "N" ? "- New -" : "")) + "(Agency-" + sancprj.ADMIN_DEPARTMENT.MASTER_AGENCY.MAST_AGENCY_NAME + ")",
                            //             IMS_PR_ROAD_CODE = sancprj.IMS_PR_ROAD_CODE
                            //         }).ToList().Distinct();
                        }
                    }

                    // Commented on 13-05-2022 due to road list not populate for agreement

                    ////new change done by Vikram on 06 Jan 2014
                    //if (dbContext.ACC_MASTER_TXN.Where(m => m.TXN_ID == objParam.HEAD_ID).Select(m => m.BILL_TYPE).FirstOrDefault() == "P")
                    //{
                    //    var roadsToRemove = (from bm in dbContext.ACC_BILL_MASTER
                    //                         join bd in dbContext.ACC_BILL_DETAILS
                    //                         on bm.BILL_ID equals bd.BILL_ID
                    //                         where
                    //                         lstTransactionIDs.Contains(bm.TXN_ID) &&
                    //                         bd.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode &&
                    //                         bd.CREDIT_DEBIT == "C"
                    //                         select new
                    //                         {                      //Road Name Modified By Abhishek kamble 19-Mar-2014
                    //                             IMS_ROAD_NAME = (bd.IMS_SANCTIONED_PROJECTS.IMS_ROAD_NAME == null ? "" : (bd.IMS_SANCTIONED_PROJECTS.IMS_PROPOSAL_TYPE == "P" ? bd.IMS_SANCTIONED_PROJECTS.IMS_ROAD_NAME : bd.IMS_SANCTIONED_PROJECTS.IMS_ROAD_NAME + (bd.IMS_SANCTIONED_PROJECTS.IMS_BRIDGE_NAME == null ? "" : "(" + bd.IMS_SANCTIONED_PROJECTS.IMS_BRIDGE_NAME + ")"))) + (bd.IMS_SANCTIONED_PROJECTS.IMS_UPGRADE_CONNECT == null ? "" : (bd.IMS_SANCTIONED_PROJECTS.IMS_UPGRADE_CONNECT == "U" ? "- Upgrade -" : (bd.IMS_SANCTIONED_PROJECTS.IMS_UPGRADE_CONNECT == "N" ? "- New -" : ""))) + "(Agency-" + (bd.IMS_SANCTIONED_PROJECTS.ADMIN_DEPARTMENT.MASTER_AGENCY.MAST_AGENCY_NAME == null ? "" : bd.IMS_SANCTIONED_PROJECTS.ADMIN_DEPARTMENT.MASTER_AGENCY.MAST_AGENCY_NAME) + ")",
                    //                             //IMS_ROAD_NAME = bd.IMS_SANCTIONED_PROJECTS.IMS_ROAD_NAME,
                    //                             IMS_PR_ROAD_CODE = (bd.IMS_PR_ROAD_CODE == null ? 0 : bd.IMS_PR_ROAD_CODE.Value)
                    //                         }).ToList();

                    //    query = query.Except(roadsToRemove).ToList();



                    //}
                    ////end of change


                    ////new change done by Vikram on 06 Jan 2014
                    //if (dbContext.ACC_MASTER_TXN.Where(m => m.TXN_ID == objParam.HEAD_ID).Select(m => m.BILL_TYPE).FirstOrDefault() == "P")
                    //{
                    //    var roadsToRemove = (from bm in dbContext.ACC_BILL_MASTER
                    //                         join bd in dbContext.ACC_BILL_DETAILS
                    //                         on bm.BILL_ID equals bd.BILL_ID
                    //                         where
                    //                         lstTransactionIDs.Contains(bm.TXN_ID) &&
                    //                         bd.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode &&
                    //                         bd.CREDIT_DEBIT == "C"
                    //                         select new
                    //                         {         //Road Name Modified By Abhishek kamble 19-Mar-2014
                    //                             IMS_ROAD_NAME = (bd.IMS_SANCTIONED_PROJECTS.IMS_ROAD_NAME == null ? "" : (bd.IMS_SANCTIONED_PROJECTS.IMS_ROAD_NAME == null ? "" : (bd.IMS_SANCTIONED_PROJECTS.IMS_PROPOSAL_TYPE == "P" ? bd.IMS_SANCTIONED_PROJECTS.IMS_ROAD_NAME : bd.IMS_SANCTIONED_PROJECTS.IMS_ROAD_NAME + (bd.IMS_SANCTIONED_PROJECTS.IMS_BRIDGE_NAME == null ? "" : "(" + bd.IMS_SANCTIONED_PROJECTS.IMS_BRIDGE_NAME + ")")))) + (bd.IMS_SANCTIONED_PROJECTS.IMS_UPGRADE_CONNECT == null ? "" : (bd.IMS_SANCTIONED_PROJECTS.IMS_UPGRADE_CONNECT == "U" ? "- Upgrade -" : (bd.IMS_SANCTIONED_PROJECTS.IMS_UPGRADE_CONNECT == "N" ? "- New -" : ""))) + "(Agency-" + (bd.IMS_SANCTIONED_PROJECTS.ADMIN_DEPARTMENT.MASTER_AGENCY.MAST_AGENCY_NAME == null ? "" : bd.IMS_SANCTIONED_PROJECTS.ADMIN_DEPARTMENT.MASTER_AGENCY.MAST_AGENCY_NAME) + ")",
                    //                             //IMS_ROAD_NAME = bd.IMS_SANCTIONED_PROJECTS.IMS_ROAD_NAME,
                    //                             IMS_PR_ROAD_CODE = (bd.IMS_PR_ROAD_CODE == null ? 0 : bd.IMS_PR_ROAD_CODE.Value)
                    //                         }).ToList();

                    //    query = query.Except(roadsToRemove).ToList();



                    //}
                    ////end of change



                    if (query == null || query.Count() == 0)
                    {
                        lstRoad.Insert(0, (new SelectListItem { Text = "Select Road", Value = "0", Selected = true }));
                        return lstRoad;
                    }
                    foreach (var item in query)
                    {
                        if (item.IMS_PR_ROAD_CODE == objParam.ROAD_CODE)
                        {
                            lstRoad.Add(new SelectListItem { Text = item.IMS_ROAD_NAME, Value = item.IMS_PR_ROAD_CODE.ToString(), Selected = true });
                        }
                        else
                        {
                            lstRoad.Add(new SelectListItem { Text = item.IMS_ROAD_NAME, Value = item.IMS_PR_ROAD_CODE.ToString() });
                        }
                    }

                    lstRoad = lstRoad.OrderBy(m => m.Text).ToList();
                    lstRoad.Insert(0, (new SelectListItem { Text = "Select Road", Value = "0" }));
                    return lstRoad;
                }
                else if (objParam.PACKAGE_ID != "0")
                {
                    var query = (IEnumerable<dynamic>)null;
                    //new change done by Vikram on 17 Jan 2014 
                    //previously fund type was not considered while populating the road so in maintenance fund the road which are not in maintenance are also get populated
                    //now fund type is checked and accordingly join is applied.
                    if (PMGSYSession.Current.FundType == "M")
                    {
                        query = (from sancprj in dbContext.IMS_SANCTIONED_PROJECTS
                                 join aggdet in dbContext.MANE_IMS_CONTRACT on sancprj.IMS_PR_ROAD_CODE equals aggdet.IMS_PR_ROAD_CODE
                                 //join aggmast in dbContext.TEND_AGREEMENT_MASTER on aggdet.TEND_AGREEMENT_CODE equals aggmast.TEND_AGREEMENT_CODE
                                 where sancprj.IMS_PACKAGE_ID == objParam.PACKAGE_ID &&
                                 (objParam.SANC_YEAR == 0 ? 1 : sancprj.IMS_YEAR) == (objParam.SANC_YEAR == 0 ? 1 : objParam.SANC_YEAR) && //new change done by Vikram as multiple roads are populating on the basis of Package with different sanction year
                                 (headId == 28 ? (sancprj.IMS_UPGRADE_CONNECT == "N") : (headId == 29 ? (sancprj.IMS_UPGRADE_CONNECT == "U") : "%" == "%"))
                                     //(headId == 0 ? 1 == 1 : sancprj.IMS_UPGRADE_CONNECT == upgradeConnectFlag)
                                 && sancprj.IMS_SANCTIONED == "Y" //change done by Vikram - Road populated should be Mord Sanctioned.
                                 && sancprj.IMS_YEAR > 1950
                                 select new
                                 {       //Road Name Modified By Abhishek kamble 19-Mar-2014
                                     IMS_ROAD_NAME = (sancprj.IMS_PROPOSAL_TYPE == "P" ? sancprj.IMS_ROAD_NAME : sancprj.IMS_ROAD_NAME + (sancprj.IMS_BRIDGE_NAME == null ? "" : "(" + sancprj.IMS_BRIDGE_NAME + ")")) + (sancprj.IMS_UPGRADE_CONNECT == "U" ? "- Upgrade -" : (sancprj.IMS_UPGRADE_CONNECT == "N" ? "- New -" : "")) + "(Agency-" + sancprj.ADMIN_DEPARTMENT.MASTER_AGENCY.MAST_AGENCY_NAME + ")",
                                     IMS_PR_ROAD_CODE = sancprj.IMS_PR_ROAD_CODE
                                 }).ToList().Distinct();
                    }
                    else
                    {
                        query = (from sancprj in dbContext.IMS_SANCTIONED_PROJECTS
                                 //join aggdet in dbContext.TEND_AGREEMENT_DETAIL on sancprj.IMS_PR_ROAD_CODE equals aggdet.IMS_PR_ROAD_CODE
                                 //join aggmast in dbContext.TEND_AGREEMENT_MASTER on aggdet.TEND_AGREEMENT_CODE equals aggmast.TEND_AGREEMENT_CODE
                                 where sancprj.IMS_PACKAGE_ID == objParam.PACKAGE_ID &&
                                 (objParam.SANC_YEAR == 0 ? 1 : sancprj.IMS_YEAR) == (objParam.SANC_YEAR == 0 ? 1 : objParam.SANC_YEAR) && //new change done by Vikram as multiple roads are populating on the basis of Package with different sanction year
                                 (headId == 28 ? (sancprj.IMS_UPGRADE_CONNECT == "N") : (headId == 29 ? (sancprj.IMS_UPGRADE_CONNECT == "U") : "%" == "%"))
                                 && sancprj.IMS_SANCTIONED == "Y" //change done by Vikram - Road populated should be Mord Sanctioned.
                                     //(headId == 0 ? 1 == 1 : sancprj.IMS_UPGRADE_CONNECT == upgradeConnectFlag)
                                 && sancprj.IMS_YEAR > 1950
                                 select new
                                 {           //Road Name Modified By Abhishek kamble 19-Mar-2014
                                     IMS_ROAD_NAME = (sancprj.IMS_PROPOSAL_TYPE == "P" ? sancprj.IMS_ROAD_NAME : sancprj.IMS_ROAD_NAME + (sancprj.IMS_BRIDGE_NAME == null ? "" : "(" + sancprj.IMS_BRIDGE_NAME + ")")) + (sancprj.IMS_UPGRADE_CONNECT == "U" ? "- Upgrade -" : (sancprj.IMS_UPGRADE_CONNECT == "N" ? "- New -" : "")) + "(Agency-" + sancprj.ADMIN_DEPARTMENT.MASTER_AGENCY.MAST_AGENCY_NAME + ")",
                                     IMS_PR_ROAD_CODE = sancprj.IMS_PR_ROAD_CODE
                                 }).ToList().Distinct();



                        //var pkgColl = (from sancprj in dbContext.IMS_SANCTIONED_PROJECTS
                        //               where sancprj.IMS_PACKAGE_ID == objParam.PACKAGE_ID &&
                        //               (objParam.SANC_YEAR == 0 ? 1 : sancprj.IMS_YEAR) == (objParam.SANC_YEAR == 0 ? 1 : objParam.SANC_YEAR) &&
                        //               (headId == 28 ? (sancprj.IMS_UPGRADE_CONNECT == "N") : (headId == 29 ? (sancprj.IMS_UPGRADE_CONNECT == "U") : "%" == "%"))
                        //               && sancprj.IMS_SANCTIONED == "Y"
                        //               select new
                        //               {
                        //                   IMS_ROAD_NAME = (sancprj.IMS_PROPOSAL_TYPE == "P" ? sancprj.IMS_ROAD_NAME : sancprj.IMS_ROAD_NAME + (sancprj.IMS_BRIDGE_NAME == null ? 
                        //                   "" : "(" + sancprj.IMS_BRIDGE_NAME + ")")) + (sancprj.IMS_UPGRADE_CONNECT == "U" ? "- Upgrade -" : (sancprj.IMS_UPGRADE_CONNECT == 
                        //                   "N" ? "- New -" : "")) + "(Agency-" + sancprj.ADMIN_DEPARTMENT.MASTER_AGENCY.MAST_AGENCY_NAME + ")",
                        //                   IMS_PR_ROAD_CODE = sancprj.IMS_PR_ROAD_CODE
                        //               }).Concat
                        //               (from itemSP in dbContext.IMS_SANCTIONED_PROJECTS
                        //                join itemTk in dbContext.IMS_PROPOSAL_TRACKING
                        //                    on itemSP.IMS_PR_ROAD_CODE equals itemTk.IMS_PR_ROAD_CODE
                        //                where itemTk.MAST_DISTRICT_CODE == objParam.DISTRICT_CODE &&
                        //                itemSP.IMS_YEAR == objParam.SANC_YEAR
                        //                select new
                        //                {
                        //                    IMS_PACKAGE_TEXT = itemSP.IMS_PACKAGE_ID,
                        //                    IMS_PACKAGE_VALUE = itemSP.IMS_PACKAGE_ID
                        //                }).Distinct().ToList();

                    }

                    if (query == null || query.Count() == 0)
                    {
                        lstRoad.Insert(0, (new SelectListItem { Text = "Select Road", Value = "0", Selected = true }));
                        return lstRoad;
                    }
                    foreach (var item in query)
                    {
                        lstRoad.Add(new SelectListItem { Text = item.IMS_ROAD_NAME, Value = item.IMS_PR_ROAD_CODE.ToString() });
                    }

                    lstRoad = lstRoad.OrderBy(m => m.Text).ToList();
                    lstRoad.Insert(0, (new SelectListItem { Text = "Select Road", Value = "0", Selected = true }));
                    return lstRoad;
                }
                //for all roads who dont have agreement i.e TEND_AGREEMENT_TYPE ="O"
                else
                {
                    var query = (from sancprj in dbContext.IMS_SANCTIONED_PROJECTS
                                 join aggdet in dbContext.TEND_AGREEMENT_DETAIL on sancprj.IMS_PR_ROAD_CODE equals aggdet.IMS_PR_ROAD_CODE
                                 join aggmast in dbContext.TEND_AGREEMENT_MASTER on aggdet.TEND_AGREEMENT_CODE equals aggmast.TEND_AGREEMENT_CODE
                                 where
                                     //commenetd by Koustubh Nakate on 23/10/2013 for loading all roads when agreement code is 0
                                     // aggmast.TEND_AGREEMENT_TYPE.ToLower() == "o"
                                 aggmast.TEND_AGREEMENT_CODE == objParam.AGREEMENT_CODE //added by Koustubh Nakate on 23/10/2013
                                 && sancprj.IMS_SANCTIONED == "Y" //change done by Vikram - Road populated should be Mord Sanctioned.
                                 && sancprj.IMS_YEAR > 1950
                                 orderby sancprj.IMS_ROAD_NAME ascending
                                 select new
                                 {              //Road Name Modified By Abhishek kamble 19-Mar-2014
                                     IMS_ROAD_NAME = (sancprj.IMS_PROPOSAL_TYPE == "P" ? sancprj.IMS_ROAD_NAME : sancprj.IMS_ROAD_NAME + (sancprj.IMS_BRIDGE_NAME == null ? "" : "(" + sancprj.IMS_BRIDGE_NAME + ")")),
                                     IMS_PR_ROAD_CODE = sancprj.IMS_PR_ROAD_CODE
                                 }
                                        ).ToList().Distinct();
                    if (query == null || query.Count() == 0)
                    {
                        lstRoad.Insert(0, (new SelectListItem { Text = "Select Road", Value = "0", Selected = true }));
                        return lstRoad;
                    }
                    foreach (var item in query)
                    {
                        lstRoad.Add(new SelectListItem { Text = item.IMS_ROAD_NAME, Value = item.IMS_PR_ROAD_CODE.ToString() });
                    }

                    lstRoad = lstRoad.OrderBy(m => m.Text).ToList();
                    lstRoad.Insert(0, (new SelectListItem { Text = "Select Road", Value = "0", Selected = true }));
                    return lstRoad;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        public String GetContractorSupplierName(TransactionParams objParam)
        {
            try
            {
                dbContext = new PMGSYEntities();
                MASTER_CONTRACTOR master_Contractor = new MASTER_CONTRACTOR();
                master_Contractor = (from con in dbContext.MASTER_CONTRACTOR
                                     where con.MAST_CON_ID == objParam.MAST_CONT_ID
                                     select con).FirstOrDefault();

                if (master_Contractor == null)
                {
                    return "Contractor or supplier Name not Present";
                }
                else
                {
                    return (master_Contractor.MAST_CON_FNAME != null ? master_Contractor.MAST_CON_FNAME.Trim() : string.Empty) + " " + (master_Contractor.MAST_CON_MNAME != null ? master_Contractor.MAST_CON_MNAME.Trim() : string.Empty) + " " + (master_Contractor.MAST_CON_LNAME != null ? master_Contractor.MAST_CON_LNAME.Trim() : string.Empty);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CommonFunctions.GetContractorSupplierName");
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// function to populate the agreement
        /// </summary>
        /// <param name="objParam"></param>
        /// <returns></returns>
        public List<SelectListItem> PopulateAgreement(TransactionParams objParam)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<SelectListItem> lstAgreement = new List<SelectListItem>();

                if (objParam.MAST_CONT_ID != 0)
                {
                    string isSupCont = dbContext.MASTER_CONTRACTOR.Where(m => m.MAST_CON_ID == objParam.MAST_CONT_ID).Select(m => m.MAST_CON_SUP_FLAG).FirstOrDefault();

                    if (isSupCont.ToLower() == "s")
                    {
                        if (PMGSYSession.Current.FundType.ToLower() == "m")
                        {

                            //working but maintenance agreement not done for supplier so populate supplier from regular agreement

                            //var varAggrement = (from tendmast in dbContext.MANE_IMS_CONTRACT
                            //                    join sancproj in dbContext.IMS_SANCTIONED_PROJECTS on tendmast.IMS_PR_ROAD_CODE equals sancproj.IMS_PR_ROAD_CODE
                            //                    where sancproj.MAST_DISTRICT_CODE == objParam.DISTRICT_CODE && tendmast.MAST_CON_ID == objParam.MAST_CONT_ID
                            //                    select new
                            //                    {
                            //                        TEND_AGREEMENT_NUMBER = tendmast.MANE_AGREEMENT_NUMBER,
                            //                        TEND_AGREEMENT_CODE = tendmast.MANE_PR_CONTRACT_CODE
                            //                    }).Distinct().ToList();

                            //added by koustubh Nakate on 04/10/2013 
                            var varAggrement = (from aggmast in dbContext.TEND_AGREEMENT_MASTER
                                                join conmast in dbContext.MASTER_CONTRACTOR
                                                on aggmast.MAST_CON_ID equals conmast.MAST_CON_ID
                                                where
                                               aggmast.TEND_AGREEMENT_TYPE == "S" &&
                                               aggmast.MAST_DISTRICT_CODE == objParam.DISTRICT_CODE &&
                                               aggmast.MAST_CON_ID == objParam.MAST_CONT_ID &&
                                               aggmast.TEND_IS_AGREEMENT_FINALIZED == "Y"
                                                select new
                                                {
                                                    TEND_AGREEMENT_NUMBER = aggmast.TEND_AGREEMENT_NUMBER,
                                                    TEND_AGREEMENT_CODE = aggmast.TEND_AGREEMENT_CODE
                                                }).Distinct().ToList();



                            if (varAggrement == null || varAggrement.Count() == 0)
                            {
                                lstAgreement.Insert(0, (new SelectListItem { Text = "Select Agreement", Value = "0", Selected = true }));
                                return lstAgreement;
                            }
                            foreach (var item in varAggrement)
                            {
                                if (item.TEND_AGREEMENT_CODE == objParam.AGREEMENT_CODE)
                                {
                                    lstAgreement.Add(new SelectListItem { Text = item.TEND_AGREEMENT_NUMBER, Value = item.TEND_AGREEMENT_CODE.ToString(), Selected = true });
                                }
                                else
                                {
                                    lstAgreement.Add(new SelectListItem { Text = item.TEND_AGREEMENT_NUMBER, Value = item.TEND_AGREEMENT_CODE.ToString() });
                                }
                            }
                            lstAgreement = lstAgreement.OrderBy(m => m.Text).ToList();
                            lstAgreement.Insert(0, (new SelectListItem { Text = "Select Agreement", Value = "0" }));
                        }
                        else
                        {
                            //var varAggrement = (from tendmast in dbContext.TEND_AGREEMENT_MASTER
                            //                    join tenddet in dbContext.TEND_AGREEMENT_DETAIL on tendmast.TEND_AGREEMENT_CODE equals tenddet.TEND_AGREEMENT_CODE
                            //                    join sancproj in dbContext.IMS_SANCTIONED_PROJECTS on tenddet.IMS_PR_ROAD_CODE equals sancproj.IMS_PR_ROAD_CODE
                            //                    where tendmast.MAST_DISTRICT_CODE == objParam.DISTRICT_CODE && tendmast.MAST_CON_ID == objParam.MAST_CONT_ID
                            //                    select new
                            //                    {
                            //                        TEND_AGREEMENT_NUMBER = tendmast.TEND_AGREEMENT_NUMBER,
                            //                        TEND_AGREEMENT_CODE = tendmast.TEND_AGREEMENT_CODE
                            //                    }).Distinct().ToList();



                            var varAggrement = (from tendmast in dbContext.TEND_AGREEMENT_MASTER
                                                join tenddet in dbContext.TEND_AGREEMENT_DETAIL on tendmast.TEND_AGREEMENT_CODE equals tenddet.TEND_AGREEMENT_CODE into first
                                                from y in first.DefaultIfEmpty()
                                                join sancproj in dbContext.IMS_SANCTIONED_PROJECTS on y.IMS_PR_ROAD_CODE equals sancproj.IMS_PR_ROAD_CODE into second
                                                from x in second.DefaultIfEmpty()
                                                where tendmast.MAST_DISTRICT_CODE == objParam.DISTRICT_CODE && tendmast.MAST_CON_ID == objParam.MAST_CONT_ID &&
                                                tendmast.TEND_IS_AGREEMENT_FINALIZED == "Y"
                                                select new
                                                {
                                                    TEND_AGREEMENT_NUMBER = tendmast.TEND_AGREEMENT_NUMBER,
                                                    TEND_AGREEMENT_CODE = tendmast.TEND_AGREEMENT_CODE
                                                }).ToList().Distinct();

                            //Added By Abhishek kamble 12-Mar-2014 for Dpr start
                            var varAggrementSupDpr = (from tendmast in dbContext.TEND_AGREEMENT_MASTER
                                                      join tenddet in dbContext.TEND_AGREEMENT_DETAIL on tendmast.TEND_AGREEMENT_CODE equals tenddet.TEND_AGREEMENT_CODE into first
                                                      from y in first.DefaultIfEmpty()
                                                      //join sancproj in dbContext.IMS_SANCTIONED_PROJECTS on y.IMS_PR_ROAD_CODE equals sancproj.IMS_PR_ROAD_CODE into second
                                                      //from x in second.DefaultIfEmpty()
                                                      where tendmast.MAST_DISTRICT_CODE == objParam.DISTRICT_CODE && tendmast.MAST_CON_ID == objParam.MAST_CONT_ID
                                                      && tendmast.TEND_IS_AGREEMENT_FINALIZED == "Y"
                                                      select new
                                                      {
                                                          TEND_AGREEMENT_NUMBER = tendmast.TEND_AGREEMENT_NUMBER,
                                                          TEND_AGREEMENT_CODE = tendmast.TEND_AGREEMENT_CODE
                                                      }).ToList().Distinct();
                            varAggrement = varAggrementSupDpr.Union(varAggrement).ToList().Distinct();

                            //Added By Abhishek kamble 12-Mar-2014 for Dpr end


                            if (varAggrement == null || varAggrement.Count() == 0)
                            {
                                lstAgreement.Insert(0, (new SelectListItem { Text = "Select Agreement", Value = "0", Selected = true }));
                                return lstAgreement;
                            }
                            foreach (var item in varAggrement)
                            {
                                if (item.TEND_AGREEMENT_CODE == objParam.AGREEMENT_CODE)
                                {
                                    lstAgreement.Add(new SelectListItem { Text = item.TEND_AGREEMENT_NUMBER, Value = item.TEND_AGREEMENT_CODE.ToString(), Selected = true });
                                }
                                else
                                {
                                    lstAgreement.Add(new SelectListItem { Text = item.TEND_AGREEMENT_NUMBER, Value = item.TEND_AGREEMENT_CODE.ToString() });
                                }
                            }
                            lstAgreement = lstAgreement.OrderBy(m => m.Text).ToList();
                            lstAgreement.Insert(0, (new SelectListItem { Text = "Select Agreement", Value = "0" }));
                        }
                    }
                    else
                    {
                        if (PMGSYSession.Current.FundType.ToLower() == "m")
                        {
                            var varAggrement = (from tendmast in dbContext.MANE_IMS_CONTRACT
                                                join sancproj in dbContext.IMS_SANCTIONED_PROJECTS on tendmast.IMS_PR_ROAD_CODE equals sancproj.IMS_PR_ROAD_CODE
                                                where sancproj.MAST_DISTRICT_CODE == objParam.DISTRICT_CODE && tendmast.MAST_CON_ID == objParam.MAST_CONT_ID
                                                && tendmast.MANE_CONTRACT_FINALIZED == "Y"
                                                select new
                                                {
                                                    TEND_AGREEMENT_NUMBER = tendmast.MANE_AGREEMENT_NUMBER,
                                                    //Old 
                                                    //TEND_AGREEMENT_CODE = tendmast.MANE_PR_CONTRACT_CODE
                                                    TEND_AGREEMENT_CODE = tendmast.MANE_CONTRACT_ID//Modified by Abhishek kamble to get Unique Agrement Code 17Nov2014
                                                }).ToList().Distinct();


                            //Added By Abhishek kamble to get one agrement number for multiple roads whithin that agreement 19Nov2014
                            varAggrement = varAggrement.GroupBy(test => test.TEND_AGREEMENT_NUMBER).Select(grp => grp.First()).ToList();


                            //added by Koustubh Nakate on 21/10/2013 for considering shifting of roads between DPIU'S
                            var varAggrementPropTrack = (from tendmast in dbContext.MANE_IMS_CONTRACT

                                                         join sancproj in dbContext.IMS_SANCTIONED_PROJECTS on tendmast.IMS_PR_ROAD_CODE equals sancproj.IMS_PR_ROAD_CODE
                                                         join imsPropTrack in dbContext.IMS_PROPOSAL_TRACKING on sancproj.IMS_PR_ROAD_CODE equals imsPropTrack.IMS_PR_ROAD_CODE
                                                         where

                                                         imsPropTrack.MAST_DISTRICT_CODE == objParam.DISTRICT_CODE
                                                         && tendmast.MAST_CON_ID == objParam.MAST_CONT_ID
                                                         && tendmast.MANE_CONTRACT_FINALIZED == "Y"
                                                         select new
                                                         {
                                                             TEND_AGREEMENT_NUMBER = tendmast.MANE_AGREEMENT_NUMBER,
                                                             //Old
                                                             //TEND_AGREEMENT_CODE = tendmast.MANE_PR_CONTRACT_CODE
                                                             //Modified by Abhishek kamble to get Unique Agrement Code 17Nov2014
                                                             TEND_AGREEMENT_CODE = tendmast.MANE_CONTRACT_ID
                                                         }).ToList().Distinct();

                            //Added By Abhishek kamble to get one agrement number for multiple roads whithin that agreement 19Nov2014
                            varAggrementPropTrack = varAggrementPropTrack.GroupBy(test => test.TEND_AGREEMENT_NUMBER).Select(grp => grp.First()).ToList();

                            //Added By Abhishek kamble 12-Mar-2014 for Dpr start
                            var varAggrementSupDpr = (from tendmast in dbContext.MANE_IMS_CONTRACT
                                                      //join sancproj in dbContext.IMS_SANCTIONED_PROJECTS on tendmast.IMS_PR_ROAD_CODE equals sancproj.IMS_PR_ROAD_CODE
                                                      where tendmast.MAST_CON_ID == objParam.MAST_CONT_ID
                                                      && tendmast.MANE_CONTRACT_FINALIZED == "Y"
                                                      select new
                                                      {
                                                          TEND_AGREEMENT_NUMBER = tendmast.MANE_AGREEMENT_NUMBER,
                                                          //old
                                                          //TEND_AGREEMENT_CODE = tendmast.MANE_PR_CONTRACT_CODE
                                                          //Modified by Abhishek kamble to get Unique Agrement Code 17Nov2014
                                                          TEND_AGREEMENT_CODE = tendmast.MANE_CONTRACT_ID
                                                      }).ToList().Distinct();
                            //Added By Abhishek kamble 12-Mar-2014 end

                            //Added By Abhishek kamble to get one agrement number for multiple roads whithin that agreement 19Nov2014
                            varAggrementSupDpr = varAggrementSupDpr.GroupBy(test => test.TEND_AGREEMENT_NUMBER).Select(grp => grp.First()).ToList();

                            varAggrement = varAggrementSupDpr.Union(varAggrementPropTrack).ToList().Distinct();

                            //Added By Abhishek kamble 12-Mar-2014
                            varAggrement = varAggrement.Union(varAggrement).ToList().Distinct();


                            if (varAggrement == null || varAggrement.Count() == 0)
                            {
                                lstAgreement.Insert(0, (new SelectListItem { Text = "Select Agreement", Value = "0", Selected = true }));
                                return lstAgreement;
                            }
                            foreach (var item in varAggrement)
                            {
                                if (item.TEND_AGREEMENT_CODE == objParam.AGREEMENT_CODE)
                                {
                                    lstAgreement.Add(new SelectListItem { Text = item.TEND_AGREEMENT_NUMBER, Value = item.TEND_AGREEMENT_CODE.ToString(), Selected = true });
                                }
                                else
                                {
                                    lstAgreement.Add(new SelectListItem { Text = item.TEND_AGREEMENT_NUMBER, Value = item.TEND_AGREEMENT_CODE.ToString() });
                                }
                            }
                            lstAgreement = lstAgreement.OrderBy(m => m.Text).ToList();
                            lstAgreement.Insert(0, (new SelectListItem { Text = "Select Agreement", Value = "0" }));
                        }
                        else
                        {
                            //var varAggrement = (from tendmast in dbContext.TEND_AGREEMENT_MASTER
                            //                    join tenddet in dbContext.TEND_AGREEMENT_DETAIL on tendmast.TEND_AGREEMENT_CODE equals tenddet.TEND_AGREEMENT_CODE
                            //                    join sancproj in dbContext.IMS_SANCTIONED_PROJECTS on tenddet.IMS_PR_ROAD_CODE equals sancproj.IMS_PR_ROAD_CODE
                            //                    where tendmast.MAST_DISTRICT_CODE == objParam.DISTRICT_CODE && tendmast.MAST_CON_ID == objParam.MAST_CONT_ID
                            //                    select new
                            //                    {
                            //                        TEND_AGREEMENT_NUMBER = tendmast.TEND_AGREEMENT_NUMBER,
                            //                        TEND_AGREEMENT_CODE = tendmast.TEND_AGREEMENT_CODE
                            //                    }).Distinct().ToList();


                            var varAggrement = (from tendmast in dbContext.TEND_AGREEMENT_MASTER
                                                join tenddet in dbContext.TEND_AGREEMENT_DETAIL on tendmast.TEND_AGREEMENT_CODE equals tenddet.TEND_AGREEMENT_CODE into first
                                                from y in first.DefaultIfEmpty()
                                                join sancproj in dbContext.IMS_SANCTIONED_PROJECTS on y.IMS_PR_ROAD_CODE equals sancproj.IMS_PR_ROAD_CODE into second
                                                from x in second.DefaultIfEmpty()
                                                where
                                                    //tendmast.MAST_DISTRICT_CODE == objParam.DISTRICT_CODE
                                                x.MAST_DISTRICT_CODE == objParam.DISTRICT_CODE
                                                && tendmast.MAST_CON_ID == objParam.MAST_CONT_ID
                                                && tendmast.TEND_IS_AGREEMENT_FINALIZED == "Y"
                                                select new
                                                {
                                                    TEND_AGREEMENT_NUMBER = tendmast.TEND_AGREEMENT_NUMBER,
                                                    TEND_AGREEMENT_CODE = tendmast.TEND_AGREEMENT_CODE
                                                }).ToList().Distinct();



                            //added by Koustubh Nakate on 21/10/2013 for considering shifting of roads between DPIU'S

                            var varAggrementPropTrack = (from tendmast in dbContext.TEND_AGREEMENT_MASTER
                                                         join tenddet in dbContext.TEND_AGREEMENT_DETAIL on tendmast.TEND_AGREEMENT_CODE equals tenddet.TEND_AGREEMENT_CODE into first
                                                         from y in first.DefaultIfEmpty()
                                                         join sancproj in dbContext.IMS_SANCTIONED_PROJECTS on y.IMS_PR_ROAD_CODE equals sancproj.IMS_PR_ROAD_CODE into second
                                                         from x in second.DefaultIfEmpty()
                                                         join imsPropTrack in dbContext.IMS_PROPOSAL_TRACKING on x.IMS_PR_ROAD_CODE equals imsPropTrack.IMS_PR_ROAD_CODE into third
                                                         from z in third.DefaultIfEmpty()
                                                         where
                                                             //tendmast.MAST_DISTRICT_CODE == objParam.DISTRICT_CODE
                                                         z.MAST_DISTRICT_CODE == objParam.DISTRICT_CODE
                                                         && tendmast.MAST_CON_ID == objParam.MAST_CONT_ID
                                                         && tendmast.TEND_IS_AGREEMENT_FINALIZED == "Y"
                                                         select new
                                                         {
                                                             TEND_AGREEMENT_NUMBER = tendmast.TEND_AGREEMENT_NUMBER,
                                                             TEND_AGREEMENT_CODE = tendmast.TEND_AGREEMENT_CODE
                                                         }).ToList().Distinct();

                            //Added By Abhishek kamble 12-Mar-2014
                            var varAggrementSupDpr = (from tendmast in dbContext.TEND_AGREEMENT_MASTER
                                                      join tenddet in dbContext.TEND_AGREEMENT_DETAIL on tendmast.TEND_AGREEMENT_CODE equals tenddet.TEND_AGREEMENT_CODE into first
                                                      from y in first.DefaultIfEmpty()
                                                      //join sancproj in dbContext.IMS_SANCTIONED_PROJECTS on y.IMS_PR_ROAD_CODE equals sancproj.IMS_PR_ROAD_CODE into second
                                                      //from x in second.DefaultIfEmpty()
                                                      where
                                                          //tendmast.MAST_DISTRICT_CODE == objParam.DISTRICT_CODE
                                                          //x.MAST_DISTRICT_CODE == objParam.DISTRICT_CODE
                                                          //&&
                                                            tendmast.MAST_CON_ID == objParam.MAST_CONT_ID
                                                            && tendmast.TEND_IS_AGREEMENT_FINALIZED == "Y"
                                                      select new
                                                      {
                                                          TEND_AGREEMENT_NUMBER = tendmast.TEND_AGREEMENT_NUMBER,
                                                          TEND_AGREEMENT_CODE = tendmast.TEND_AGREEMENT_CODE
                                                      }).ToList().Distinct();

                            varAggrement = varAggrement.Union(varAggrementPropTrack).ToList().Distinct();

                            varAggrement = varAggrementSupDpr.Union(varAggrement).ToList().Distinct();

                            if (varAggrement == null || varAggrement.Count() == 0)
                            {
                                lstAgreement.Insert(0, (new SelectListItem { Text = "Select Agreement", Value = "0", Selected = true }));
                                return lstAgreement;
                            }
                            foreach (var item in varAggrement)
                            {
                                if (item.TEND_AGREEMENT_CODE == objParam.AGREEMENT_CODE)
                                {
                                    lstAgreement.Add(new SelectListItem { Text = item.TEND_AGREEMENT_NUMBER, Value = item.TEND_AGREEMENT_CODE.ToString(), Selected = true });
                                }
                                else
                                {
                                    lstAgreement.Add(new SelectListItem { Text = item.TEND_AGREEMENT_NUMBER, Value = item.TEND_AGREEMENT_CODE.ToString() });
                                }
                            }
                            lstAgreement = lstAgreement.OrderBy(m => m.Text).ToList();
                            lstAgreement.Insert(0, (new SelectListItem { Text = "Select Agreement", Value = "0" }));
                        }
                    }
                }
                else
                {
                    lstAgreement.Insert(0, (new SelectListItem { Text = "Select Agreement", Value = "0", Selected = true }));
                    return lstAgreement;
                }
                return lstAgreement;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CommonFunctions.PopulateAgreement");
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public List<SelectListItem> PopulatePackage(TransactionParams objParam)
        {
            try
            {
                dbContext = new PMGSYEntities();
                objParam.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                List<SelectListItem> lstPackage = new List<SelectListItem>();

                if (objParam.SANC_YEAR != 0)
                {
                    if (PMGSYSession.Current.FundType == null)
                    {
                        var pkgColl = (from item in dbContext.IMS_SANCTIONED_PROJECTS
                                       join aggdet in dbContext.TEND_AGREEMENT_DETAIL on item.IMS_PR_ROAD_CODE equals aggdet.IMS_PR_ROAD_CODE
                                       where item.MAST_DISTRICT_CODE == objParam.DISTRICT_CODE && item.IMS_YEAR == objParam.SANC_YEAR && item.MAST_DPIU_CODE == objParam.ADMIN_ND_CODE  //&& item.IMS_YEAR == objParam.SANC_YEAR
                                       select new
                                       {
                                           IMS_PACKAGE_TEXT = item.IMS_PACKAGE_ID,
                                           IMS_PACKAGE_VALUE = item.IMS_PACKAGE_ID
                                       }).OrderBy(m => m.IMS_PACKAGE_TEXT).Distinct().ToList();



                        if (pkgColl == null || pkgColl.Count() == 0)
                        {
                            if (objParam.ISSearch)
                            {
                                lstPackage.Insert(0, (new SelectListItem { Text = "All Packages", Value = "0", Selected = true }));
                            }
                            else
                            {
                                lstPackage.Insert(0, (new SelectListItem { Text = "Select Package", Value = "0", Selected = true }));
                            }
                            return lstPackage;
                        }
                        else
                        {
                            foreach (var item in pkgColl)
                            {
                                lstPackage.Insert(0, (new SelectListItem { Text = item.IMS_PACKAGE_TEXT.ToString(), Value = item.IMS_PACKAGE_VALUE.ToString() }));
                            }
                        }
                    }
                    else
                    {
                        if (PMGSYSession.Current.FundType.ToLower() == "m")
                        {
                            var pkgColl = (from item in dbContext.IMS_SANCTIONED_PROJECTS
                                           join aggdet in dbContext.MANE_IMS_CONTRACT on item.IMS_PR_ROAD_CODE equals aggdet.IMS_PR_ROAD_CODE
                                           where item.MAST_DISTRICT_CODE == objParam.DISTRICT_CODE && item.IMS_YEAR == objParam.SANC_YEAR
                                           //&& item.MAST_DPIU_CODE == objParam.ADMIN_ND_CODE
                                           //&& item.IMS_YEAR == objParam.SANC_YEAR
                                           select new
                                           {
                                               IMS_PACKAGE_TEXT = item.IMS_PACKAGE_ID,
                                               IMS_PACKAGE_VALUE = item.IMS_PACKAGE_ID
                                           }).OrderBy(m => m.IMS_PACKAGE_TEXT).Distinct().ToList();
                            if (pkgColl == null || pkgColl.Count() == 0)
                            {
                                if (objParam.ISSearch)
                                {
                                    lstPackage.Insert(0, (new SelectListItem { Text = "All Packages", Value = "0", Selected = true }));
                                }
                                else
                                {
                                    lstPackage.Insert(0, (new SelectListItem { Text = "Select Package", Value = "0", Selected = true }));
                                }
                                return lstPackage;
                            }
                            else
                            {
                                foreach (var item in pkgColl)
                                {
                                    lstPackage.Insert(0, (new SelectListItem { Text = item.IMS_PACKAGE_TEXT.ToString(), Value = item.IMS_PACKAGE_VALUE.ToString() }));
                                }
                            }
                        }
                        else
                        {
                            //var pkgColl = (from item in dbContext.IMS_SANCTIONED_PROJECTS
                            //               join aggdet in dbContext.TEND_AGREEMENT_DETAIL on item.IMS_PR_ROAD_CODE equals aggdet.IMS_PR_ROAD_CODE
                            //               where item.MAST_DISTRICT_CODE == objParam.DISTRICT_CODE && item.IMS_YEAR == objParam.SANC_YEAR && item.MAST_DPIU_CODE == objParam.ADMIN_ND_CODE  //&& item.IMS_YEAR == objParam.SANC_YEAR
                            //               select new
                            //               {
                            //                   IMS_PACKAGE_TEXT = item.IMS_PACKAGE_ID,
                            //                   IMS_PACKAGE_VALUE = item.IMS_PACKAGE_ID
                            //               }).Distinct().ToList();

                            //var pkgColl = (from item in dbContext.IMS_SANCTIONED_PROJECTS
                            //               where item.MAST_DISTRICT_CODE == objParam.DISTRICT_CODE && item.IMS_YEAR == objParam.SANC_YEAR
                            //               select new
                            //               {
                            //                   IMS_PACKAGE_TEXT = item.IMS_PACKAGE_ID,
                            //                   IMS_PACKAGE_VALUE = item.IMS_PACKAGE_ID
                            //               }).OrderBy(m => m.IMS_PACKAGE_TEXT).Distinct().ToList();

                            var pkgColl = (from item in dbContext.IMS_SANCTIONED_PROJECTS
                                           where item.MAST_DISTRICT_CODE == objParam.DISTRICT_CODE &&
                                           item.IMS_YEAR == objParam.SANC_YEAR
                                               ///Changed by SAMMED A. PATIL on 20FEB2018 check for RCPLWE Scheme
                                           && item.MAST_PMGSY_SCHEME == ((objParam.HEAD_ID == 1815 || objParam.HEAD_ID == 1816 || objParam.HEAD_ID == 1817 || objParam.HEAD_ID == 1819) ? 3 : item.MAST_PMGSY_SCHEME)
                                           select new
                                           {
                                               IMS_PACKAGE_TEXT = item.IMS_PACKAGE_ID,
                                               IMS_PACKAGE_VALUE = item.IMS_PACKAGE_ID
                                           }).Concat
                                         (from itemSP in dbContext.IMS_SANCTIONED_PROJECTS
                                          join itemTk in dbContext.IMS_PROPOSAL_TRACKING
                                              on itemSP.IMS_PR_ROAD_CODE equals itemTk.IMS_PR_ROAD_CODE
                                          where itemTk.MAST_DISTRICT_CODE == objParam.DISTRICT_CODE &&
                                          itemSP.IMS_YEAR == objParam.SANC_YEAR
                                              ///Changed by SAMMED A. PATIL on 20FEB2018 check for RCPLWE Scheme
                                           && ((objParam.HEAD_ID == 1815 || objParam.HEAD_ID == 1816 || objParam.HEAD_ID == 1817 || objParam.HEAD_ID == 1819) ? itemSP.MAST_PMGSY_SCHEME : 1) == ((objParam.HEAD_ID == 1815 || objParam.HEAD_ID == 1816 || objParam.HEAD_ID == 1817 || objParam.HEAD_ID == 1819) ? 3 : 1)
                                          select new
                                          {
                                              IMS_PACKAGE_TEXT = itemSP.IMS_PACKAGE_ID,
                                              IMS_PACKAGE_VALUE = itemSP.IMS_PACKAGE_ID
                                          }).Distinct().ToList();


                            if (pkgColl == null || pkgColl.Count() == 0)
                            {
                                if (objParam.ISSearch)
                                {
                                    lstPackage.Insert(0, (new SelectListItem { Text = "All Packages", Value = "0", Selected = true }));
                                }
                                else
                                {
                                    lstPackage.Insert(0, (new SelectListItem { Text = "Select Package", Value = "0", Selected = true }));
                                }
                                return lstPackage;
                            }
                            else
                            {
                                foreach (var item in pkgColl)
                                {
                                    lstPackage.Insert(0, (new SelectListItem { Text = item.IMS_PACKAGE_TEXT.ToString(), Value = item.IMS_PACKAGE_VALUE.ToString() }));
                                }
                            }
                        }
                    }
                }
                else if (objParam.DISTRICT_CODE != 0)
                {
                    var pkgColl = (from item in dbContext.IMS_SANCTIONED_PROJECTS
                                   where item.MAST_DISTRICT_CODE == objParam.DISTRICT_CODE && item.IMS_YEAR == objParam.SANC_YEAR && item.MAST_DPIU_CODE == objParam.ADMIN_ND_CODE
                                   select new
                                   {
                                       IMS_PACKAGE_TEXT = item.IMS_PACKAGE_ID,
                                       IMS_PACKAGE_VALUE = item.IMS_PACKAGE_ID
                                   }).OrderBy(m => m.IMS_PACKAGE_TEXT).Distinct().ToList();
                    if (pkgColl == null || pkgColl.Count() == 0)
                    {
                        if (objParam.ISSearch)
                        {
                            lstPackage.Insert(0, (new SelectListItem { Text = "All Packages", Value = "0", Selected = true }));
                        }
                        else
                        {
                            lstPackage.Insert(0, (new SelectListItem { Text = "Select Package", Value = "0", Selected = true }));
                        }
                        return lstPackage;
                    }
                    else
                    {
                        foreach (var item in pkgColl)
                        {
                            lstPackage.Insert(0, (new SelectListItem { Text = item.IMS_PACKAGE_TEXT.ToString(), Value = item.IMS_PACKAGE_VALUE.ToString() }));
                        }
                    }
                }
                else if (objParam.BlockCode != 0)
                {
                    var pkgColl = (from item in dbContext.IMS_SANCTIONED_PROJECTS
                                   where item.MAST_STATE_CODE == objParam.STATE_CODE && item.MAST_DISTRICT_CODE == objParam.DISTRICT_CODE && item.MAST_BLOCK_CODE == objParam.BlockCode && item.MAST_DPIU_CODE == objParam.ADMIN_ND_CODE && item.IMS_YEAR == objParam.SANC_YEAR
                                   select new
                                   {
                                       IMS_PACKAGE_TEXT = item.IMS_PACKAGE_ID,
                                       IMS_PACKAGE_VALUE = item.IMS_PACKAGE_ID
                                   }).OrderBy(m => m.IMS_PACKAGE_TEXT).Distinct().ToList();
                    if (pkgColl == null || pkgColl.Count() == 0)
                    {
                        lstPackage.Insert(0, (new SelectListItem { Text = "All Packages", Value = "0", Selected = true }));
                        return lstPackage;
                    }
                    else
                    {
                        foreach (var item in pkgColl)
                        {
                            lstPackage.Insert(0, (new SelectListItem { Text = item.IMS_PACKAGE_TEXT.ToString(), Value = item.IMS_PACKAGE_VALUE.ToString() }));
                        }
                    }
                }
                else
                {
                    var pkgColl = (from item in dbContext.IMS_SANCTIONED_PROJECTS
                                   where item.MAST_STATE_CODE == objParam.STATE_CODE && item.IMS_YEAR == objParam.SANC_YEAR && item.MAST_DPIU_CODE == objParam.ADMIN_ND_CODE
                                   select new
                                   {
                                       IMS_PACKAGE_TEXT = item.IMS_PACKAGE_ID,
                                       IMS_PACKAGE_VALUE = item.IMS_PACKAGE_ID
                                   }).OrderBy(m => m.IMS_PACKAGE_TEXT).Distinct().ToList();
                    if (pkgColl == null || pkgColl.Count() == 0)
                    {
                        lstPackage.Insert(0, (new SelectListItem { Text = "Select Package", Value = "0", Selected = true }));
                        return lstPackage;
                    }
                    else
                    {
                        foreach (var item in pkgColl)
                        {
                            lstPackage.Insert(0, (new SelectListItem { Text = item.IMS_PACKAGE_TEXT.ToString(), Value = item.IMS_PACKAGE_VALUE.ToString() }));
                        }
                    }
                }
                lstPackage = lstPackage.OrderBy(m => m.Text).ToList();
                if (objParam.ISSearch)
                {
                    lstPackage.Insert(0, (new SelectListItem { Text = "All Packages", Value = "0", Selected = true }));
                }
                else
                {

                    lstPackage.Insert(0, (new SelectListItem { Text = "Select Package", Value = "0", Selected = true }));
                }
                return lstPackage;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        //public List<SelectListItem> PopulateHead(TransactionParams objParam)
        //{
        //    try
        //    {
        //        dbContext = new PMGSYEntities();
        //        List<SelectListItem> lstHeads = new List<SelectListItem>();
        //        var varHeads = (from hmap in dbContext.ACC_TXN_HEAD_MAPPING
        //                        join hmast in dbContext.ACC_MASTER_HEAD
        //                        on hmap.HEAD_ID equals hmast.HEAD_ID
        //                        where hmap.TXN_ID == objParam.TXN_ID && hmap.CREDIT_DEBIT == objParam.CREDIT_DEBIT && hmast.FUND_TYPE == objParam.FUND_TYPE && hmap.IS_OPERATIONAL == true
        //                        select new
        //                        {
        //                            HEAD_ID = hmast.HEAD_ID,
        //                            HEAD_NAME = hmast.HEAD_NAME
        //                        }).ToList();
        //        if (varHeads == null || varHeads.Count() == 0)
        //        {
        //            lstHeads.Insert(0, (new SelectListItem { Text = "Select Head", Value = "0", Selected = true }));
        //            return lstHeads;
        //        }
        //        foreach (var item in varHeads)
        //        {
        //            if (item.HEAD_ID == objParam.HEAD_ID)
        //            {
        //                lstHeads.Add(new SelectListItem { Text = item.HEAD_NAME.Trim(), Value = item.HEAD_ID.ToString(), Selected= true });
        //            }
        //            else
        //            {
        //                lstHeads.Add(new SelectListItem { Text = item.HEAD_NAME.Trim(), Value = item.HEAD_ID.ToString() });
        //            }
        //        }
        //        lstHeads = lstHeads.OrderBy(m => m.Text).ToList();
        //        lstHeads.Insert(0, (new SelectListItem { Text = "Select Head", Value = "0" }));
        //        return lstHeads;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        dbContext.Dispose();
        //    }
        //}

        public List<SelectListItem> PopulateHead(TransactionParams objParam)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<SelectListItem> lstHeads = new List<SelectListItem>();
                var varHeads = (from hmap in dbContext.ACC_TXN_HEAD_MAPPING
                                join hmast in dbContext.ACC_MASTER_HEAD
                                on hmap.HEAD_ID equals hmast.HEAD_ID
                                where hmap.TXN_ID == objParam.TXN_ID && hmap.CREDIT_DEBIT == objParam.CREDIT_DEBIT && hmast.FUND_TYPE == objParam.FUND_TYPE && hmap.IS_OPERATIONAL == true
                                select new
                                {
                                    HEAD_ID = hmast.HEAD_ID,
                                    HEAD_NAME = hmast.HEAD_CODE + " - " + hmast.HEAD_NAME
                                }).ToList();
                if (varHeads == null || varHeads.Count() == 0)
                {
                    lstHeads.Insert(0, (new SelectListItem { Text = "Select Head", Value = "0", Selected = true }));
                    return lstHeads;
                }
                foreach (var item in varHeads)
                {
                    if (item.HEAD_ID == objParam.HEAD_ID)
                    {
                        lstHeads.Add(new SelectListItem { Text = item.HEAD_NAME.Trim(), Value = item.HEAD_ID.ToString(), Selected = true });
                    }
                    else
                    {
                        lstHeads.Add(new SelectListItem { Text = item.HEAD_NAME.Trim(), Value = item.HEAD_ID.ToString() });
                    }
                }
                lstHeads = lstHeads.OrderBy(m => m.Text).ToList();
                lstHeads.Insert(0, (new SelectListItem { Text = "Select Head", Value = "0" }));
                return lstHeads;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        public List<SelectListItem> PopulateDPIU(TransactionParams objParam)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<SelectListItem> lstDPIU = null;
                if (objParam.DISTRICT_CODE == 0)
                {
                    lstDPIU = new SelectList(dbContext.ADMIN_DEPARTMENT.Where(m => m.MAST_PARENT_ND_CODE == objParam.ADMIN_ND_CODE && m.MAST_ND_TYPE == "D"), "ADMIN_ND_CODE", "ADMIN_ND_NAME", objParam.ADMIN_ND_CODE).ToList();
                }
                else
                {
                    lstDPIU = new SelectList(dbContext.ADMIN_DEPARTMENT.Where(m => m.MAST_DISTRICT_CODE == objParam.DISTRICT_CODE), "ADMIN_ND_CODE", "ADMIN_ND_NAME", objParam.ADMIN_ND_CODE).ToList();
                }
                if (dbContext.ACC_TEO_SCREEN_DESIGN_PARAM_DETAILS.Where(m => m.TXN_ID == objParam.TXN_ID).Select(m => m.DPIU_REQ).FirstOrDefault() == "N")
                {
                    lstDPIU.Insert(0, (new SelectListItem { Text = "Select DPIU", Value = "0", Selected = true }));
                    return lstDPIU;
                }
                else if (lstDPIU == null || lstDPIU.Count() == 0)
                {
                    lstDPIU.Insert(0, (new SelectListItem { Text = "Select DPIU", Value = "0", Selected = true }));
                    return lstDPIU;
                }
                lstDPIU = lstDPIU.OrderBy(m => m.Text).ToList();
                lstDPIU.Insert(0, (new SelectListItem { Text = "Select DPIU", Value = "0" }));
                return lstDPIU;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// Populate Agencies of specific states
        /// </summary>
        /// <param name="stateCode"></param>
        /// <param name="isAllSelected"></param>
        /// <returns></returns>
        public List<SelectListItem> PopulateAgencies(int stateCode, bool isAllSelected = false)
        {
            List<SelectListItem> lstAgencies = new List<SelectListItem>();
            SelectListItem item;

            try
            {
                dbContext = new PMGSYEntities();
                var query = (from ma in dbContext.MASTER_AGENCY
                             join md in dbContext.ADMIN_DEPARTMENT on ma.MAST_AGENCY_CODE equals md.MAST_AGENCY_CODE
                             where md.MAST_STATE_CODE == stateCode &&
                             md.MAST_ND_TYPE == "S"
                             select new
                             {
                                 Text = md.ADMIN_ND_NAME,//ma.MAST_AGENCY_NAME,
                                 Value = ma.MAST_AGENCY_CODE,
                                 Selected = (ma.MAST_AGENCY_TYPE == "G" ? true : false)
                             }).OrderBy(c => c.Text).ToList().Distinct();

                foreach (var data in query)
                {
                    item = new SelectListItem();
                    item.Text = data.Text;
                    item.Value = data.Value.ToString();
                    item.Selected = data.Selected;
                    lstAgencies.Add(item);
                }

                if (isAllSelected == false)
                {
                    lstAgencies.Insert(0, (new SelectListItem { Text = "Select Agency", Value = "-1", Selected = true }));
                }
                else if (isAllSelected == true)
                {
                    lstAgencies.Insert(0, (new SelectListItem { Text = "All Agencies", Value = "0", Selected = true }));
                }

                return lstAgencies;
            }
            catch
            {
                return lstAgencies;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public List<SelectListItem> PopulateCollaborationsStateWise(int stateCode, bool isAllSelected = false)   //pop collaboration
        {
            List<SelectListItem> lstCollaborations = new List<SelectListItem>();
            SelectListItem item;

            try
            {
                dbContext = new PMGSYEntities();
                var query = (from ms in dbContext.MASTER_FUNDING_AGENCY
                             join isp in dbContext.IMS_SANCTIONED_PROJECTS on ms.MAST_FUNDING_AGENCY_CODE equals isp.IMS_COLLABORATION
                             where isp.MAST_STATE_CODE == stateCode
                             select new
                             {
                                 Text = ms.MAST_FUNDING_AGENCY_NAME,
                                 Value = ms.MAST_FUNDING_AGENCY_CODE
                             }).Distinct().OrderBy(c => c.Text).ToList();

                foreach (var data in query)
                {
                    item = new SelectListItem();
                    item.Text = data.Text;
                    item.Value = data.Value.ToString();
                    lstCollaborations.Add(item);
                }

                if (isAllSelected == false)
                {
                    lstCollaborations.Insert(0, (new SelectListItem { Text = "Select Collaboration", Value = "-1", Selected = true }));
                }
                else if (isAllSelected == true)
                {
                    lstCollaborations.Insert(0, (new SelectListItem { Text = "All Collaboration", Value = "0", Selected = true }));
                }

                return lstCollaborations;
            }
            catch
            {
                return lstCollaborations;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// Populate Agencies of specific states
        /// </summary>
        /// <param name="stateCode"></param>
        /// <param name="isAllSelected"></param>
        /// <returns></returns>
        public List<SelectListItem> PopulateAgenciesByStateAndDepartmentwise(int stateCode, int adminNdCode, bool isAllSelected = false)
        {
            List<SelectListItem> lstAgencies = new List<SelectListItem>();
            SelectListItem item;

            try
            {
                dbContext = new PMGSYEntities();
                var query = (from ma in dbContext.MASTER_AGENCY
                             join md in dbContext.ADMIN_DEPARTMENT on ma.MAST_AGENCY_CODE equals md.MAST_AGENCY_CODE
                             where md.MAST_STATE_CODE == stateCode &&
                             ((adminNdCode == 0 ? 1 : md.ADMIN_ND_CODE) == (adminNdCode == 0 ? 1 : adminNdCode))
                             select new
                             {
                                 Text = ma.MAST_AGENCY_NAME,
                                 Value = ma.MAST_AGENCY_CODE
                             }).OrderBy(c => c.Text).ToList().Distinct();

                foreach (var data in query)
                {
                    item = new SelectListItem();
                    item.Text = data.Text;
                    item.Value = data.Value.ToString();
                    lstAgencies.Add(item);
                }

                if (isAllSelected == false)
                {
                    lstAgencies.Insert(0, (new SelectListItem { Text = "Select Agency", Value = "-1", Selected = true }));
                }
                else if (isAllSelected == true)
                {
                    lstAgencies.Insert(0, (new SelectListItem { Text = "All Agencies", Value = "0", Selected = true }));
                }

                return lstAgencies;
            }
            catch
            {
                return lstAgencies;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Int16 getSancYearFromRoad(Int32 roadId)
        {
            try
            {
                dbContext = new PMGSYEntities();
                return dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == roadId).Select(m => (Int16)m.IMS_YEAR).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public String getPackageFromRoad(Int32 roadId)
        {
            try
            {
                dbContext = new PMGSYEntities();
                return dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == roadId).Select(m => m.IMS_PACKAGE_ID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        ///Changes for Supplier Payment Building Proposal
        public bool getContractorSupplierbyBillId(long billId)
        {
            bool isSupplier = false;
            string conSupplier = string.Empty;
            try
            {
                dbContext = new PMGSYEntities();

                conSupplier = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == billId).Select(m => m.MASTER_CONTRACTOR.MAST_CON_SUP_FLAG).FirstOrDefault();
                isSupplier = conSupplier.Trim() == "S" ? true : false; 

                //return dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == roadId).Select(m => m.IMS_PACKAGE_ID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Common.getContractorSupplierbyBillId()");
            }
            finally
            {
                dbContext.Dispose();
            }
            return isSupplier;
        }

        public ACC_SCREEN_DESIGN_PARAM_MASTER getMasterDesignParam(TransactionParams objParam)
        {
            try
            {
                dbContext = new PMGSYEntities();
                ACC_SCREEN_DESIGN_PARAM_MASTER acc_dsign_master = new ACC_SCREEN_DESIGN_PARAM_MASTER();
                acc_dsign_master = (from item in dbContext.ACC_SCREEN_DESIGN_PARAM_MASTER
                                    where item.TXN_ID == objParam.TXN_ID
                                    select item).FirstOrDefault();

                return acc_dsign_master;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public ACC_SCREEN_DESIGN_PARAM_DETAILS getDetailsDesignParam(TransactionParams objParam)
        {
            try
            {
                dbContext = new PMGSYEntities();
                ACC_SCREEN_DESIGN_PARAM_DETAILS acc_dsign_details = new ACC_SCREEN_DESIGN_PARAM_DETAILS();
                acc_dsign_details = (from item in dbContext.ACC_SCREEN_DESIGN_PARAM_DETAILS
                                     where item.TXN_ID == objParam.TXN_ID
                                     select item).FirstOrDefault();

                return acc_dsign_details;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public ACC_SCREEN_DESIGN_PARAM_DETAILS_CUSTOM getDetailsDesignParamForPayment(TransactionParams objParam)
        {
            try
            {
                dbContext = new PMGSYEntities();

                ACC_SCREEN_DESIGN_PARAM_DETAILS_CUSTOM Custom_acc_dsign_details = new ACC_SCREEN_DESIGN_PARAM_DETAILS_CUSTOM();

                ACC_SCREEN_DESIGN_PARAM_DETAILS acc_dsign_details = new ACC_SCREEN_DESIGN_PARAM_DETAILS();

                acc_dsign_details = (from item in dbContext.ACC_SCREEN_DESIGN_PARAM_DETAILS
                                     where item.TXN_ID == objParam.TXN_ID
                                     select item).FirstOrDefault();

                Custom_acc_dsign_details.TXN_ID = acc_dsign_details.TXN_ID;
                Custom_acc_dsign_details.CON_REQ = acc_dsign_details.CON_REQ;
                Custom_acc_dsign_details.AGREEMENT_REQ = acc_dsign_details.AGREEMENT_REQ;
                Custom_acc_dsign_details.ROAD_REQ = acc_dsign_details.ROAD_REQ;
                Custom_acc_dsign_details.PIU_REQ = acc_dsign_details.PIU_REQ;
                Custom_acc_dsign_details.SUPPLIER_REQ = acc_dsign_details.SUPPLIER_REQ;
                Custom_acc_dsign_details.YEAR_REQ = acc_dsign_details.YEAR_REQ;
                Custom_acc_dsign_details.PKG_REQ = acc_dsign_details.PKG_REQ;



                Custom_acc_dsign_details.MASTER_TXN_ID = dbContext.ACC_MASTER_TXN.Where(x => x.TXN_ID == objParam.TXN_ID).Select(c => c.TXN_PARENT_ID).First();

                String ContractorReqAtMaster = String.Empty;

                ContractorReqAtMaster = dbContext.ACC_SCREEN_DESIGN_PARAM_MASTER.Where(c => c.TXN_ID == Custom_acc_dsign_details.MASTER_TXN_ID).Select(x => x.MAST_CON_REQ).First();

                if (ContractorReqAtMaster == "N" && Custom_acc_dsign_details.CON_REQ == "Y")
                {
                    Custom_acc_dsign_details.SHOW_CON_AT_TRANSACTION = true;
                }
                else
                {
                    Custom_acc_dsign_details.SHOW_CON_AT_TRANSACTION = false;
                }

                return Custom_acc_dsign_details;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public ACC_TEO_SCREEN_DESIGN_PARAM_DETAILS getTEODesignParamDetails(Int64 billId, Int16 transId)
        {
            try
            {
                dbContext = new PMGSYEntities();
                ACC_TEO_SCREEN_DESIGN_PARAM_DETAILS designParams = new ACC_TEO_SCREEN_DESIGN_PARAM_DETAILS();
                if (transId == 0)
                {
                    designParams = dbContext.ACC_TEO_SCREEN_DESIGN_PARAM_DETAILS.Where(m => m.TXN_ID == (dbContext.ACC_BILL_MASTER.Where(n => n.BILL_ID == billId).Select(n => n.TXN_ID).FirstOrDefault())).FirstOrDefault();
                }
                else
                {
                    designParams = dbContext.ACC_TEO_SCREEN_DESIGN_PARAM_DETAILS.Where(m => m.TXN_ID == transId).FirstOrDefault();
                }

                if (designParams == null)
                {
                    designParams = SetDefaultTEOScreenDesignParams();
                }

                return designParams;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public ACC_TEO_SCREEN_DESIGN_PARAM_DETAILS SetDefaultTEOScreenDesignParams()
        {
            ACC_TEO_SCREEN_DESIGN_PARAM_DETAILS designParams = new ACC_TEO_SCREEN_DESIGN_PARAM_DETAILS();
            designParams.AFREEMENT_REQ = "N";
            designParams.CON_REQ = "N";
            designParams.DISTRICT_REQ = "N";
            designParams.DPIU_REQ = "N";
            designParams.PKG_REQ = "N";
            designParams.ROAD_REQ = "N";
            designParams.SAN_REQ = "N";
            designParams.SUPP_REQ = "N";
            designParams.MTXN_REQ = "N";
            return designParams;

        }

        public ACC_TEO_SCREEN_TXN_VALIDATIONS GetTEOValidationParams(Int64 billId, Int16 transID)
        {
            try
            {
                dbContext = new PMGSYEntities();
                ACC_TEO_SCREEN_TXN_VALIDATIONS designParams = new ACC_TEO_SCREEN_TXN_VALIDATIONS();
                if (transID == 0)
                {
                    designParams = dbContext.ACC_TEO_SCREEN_TXN_VALIDATIONS.Where(m => m.TXN_ID == (dbContext.ACC_BILL_MASTER.Where(n => n.BILL_ID == billId).Select(n => n.TXN_ID).FirstOrDefault())).FirstOrDefault();
                }
                else
                {
                    designParams = dbContext.ACC_TEO_SCREEN_TXN_VALIDATIONS.Where(m => m.TXN_ID == transID).FirstOrDefault();
                }

                if (designParams == null)
                {
                    designParams = SetDefaultTEOValidationParams();
                }

                return designParams;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public ACC_TEO_SCREEN_TXN_VALIDATIONS SetDefaultTEOValidationParams()
        {
            ACC_TEO_SCREEN_TXN_VALIDATIONS designParams = new ACC_TEO_SCREEN_TXN_VALIDATIONS();
            designParams.IS_AGREEMENT_REPEAT = "N";
            designParams.IS_CON_REPEAT = "N";
            designParams.IS_DISTRICT_REPEAT = "N";
            designParams.IS_DPIU_REPEAT = "N";
            designParams.IS_HEAD_REPEAT = "N";
            designParams.IS_ROAD_REPEAT = "N";
            designParams.IS_SUP_REPEAT = "N";

            return designParams;
        }

        public List<SelectListItem> PopulateRemittanceDepartment()
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<SelectListItem> lstRemDepartment = null;
                lstRemDepartment = new SelectList(dbContext.ACC_MASTER_REMIT_DEPT.Where(m => m.IS_VALID == true), "DEPT_ID", "DEPT_NAME").ToList();
                lstRemDepartment.Insert(0, (new SelectListItem { Text = "Select Department", Value = "0", Selected = true }));
                return lstRemDepartment;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public string GetRemDepartmentName(int rem_DeptId)
        {
            try
            {
                dbContext = new PMGSYEntities();
                var deptname = (from con in dbContext.ACC_MASTER_REMIT_DEPT
                                where con.IS_VALID == true && con.DEPT_ID == rem_DeptId
                                select con.PFF_PAYEE_NAME).FirstOrDefault();

                if (deptname != null)
                {
                    return deptname.ToString().Trim();
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        #region Common Functions by Shivkumar

        public string FormatErrorMessage(ModelStateDictionary modelstate)
        {
            StringBuilder strErrorMessage = new StringBuilder();
            int count = 0;
            foreach (var modelStateValue in modelstate.Values)
            {

                foreach (var error in modelStateValue.Errors)
                {
                    if (count == 0)
                    {
                        strErrorMessage.Append("<ul>");
                    }
                    count++;
                    strErrorMessage.Append("<li>");
                    strErrorMessage.Append(error.ErrorMessage);
                    strErrorMessage.Append("</li>");
                }
            }
            strErrorMessage.Append("</ul>");
            return strErrorMessage.ToString();
        }

        public bool IsPackageExists(int MAST_STATE_CODE, int MAST_DPIU_CODE, int IMS_YEAR, string IMS_PACKAGE_ID, int IMS_PR_ROAD_CODE)
        {
            try
            {
                dbContext = new PMGSYEntities();
                string StateShortCode = dbContext.MASTER_STATE.Where(a => a.MAST_STATE_CODE == MAST_STATE_CODE).Select(a => a.MAST_STATE_SHORT_CODE).First();
                IMS_PACKAGE_ID = (StateShortCode + (dbContext.MASTER_DISTRICT.Where(m => m.MAST_DISTRICT_CODE == PMGSYSession.Current.DistrictCode).Select(m => m.MAST_DISTRICT_ID).FirstOrDefault()) + IMS_PACKAGE_ID.TrimStart()).Trim();

                // Incase of Update Operation of Proposal
                if (Convert.ToInt32(IMS_PR_ROAD_CODE) != 0)
                {
                    if (dbContext.IMS_SANCTIONED_PROJECTS.Where(a => a.MAST_STATE_CODE == MAST_STATE_CODE && a.IMS_YEAR == IMS_YEAR && a.IMS_PACKAGE_ID == IMS_PACKAGE_ID && a.IMS_PR_ROAD_CODE != IMS_PR_ROAD_CODE).Any())
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                /// Incase of New Package And Add New Proposal
                else if (dbContext.IMS_SANCTIONED_PROJECTS.Where(a => a.MAST_STATE_CODE == MAST_STATE_CODE && a.IMS_YEAR == IMS_YEAR && a.IMS_PACKAGE_ID == IMS_PACKAGE_ID).Any())
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public SelectList PopulateFinancialYear(bool populateFirstItem = true, bool isAllYearsSelected = false)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<MASTER_YEAR> lstYears = dbContext.MASTER_YEAR.Where(y => y.MAST_YEAR_CODE <= DateTime.Now.Year).OrderByDescending(y => y.MAST_YEAR_CODE).ToList<MASTER_YEAR>();

                if (populateFirstItem && isAllYearsSelected)
                {
                    lstYears.Insert(0, new MASTER_YEAR() { MAST_YEAR_CODE = 0, MAST_YEAR_TEXT = "All Years" });
                }
                else
                {
                    lstYears.Insert(0, new MASTER_YEAR() { MAST_YEAR_CODE = 0, MAST_YEAR_TEXT = "Select Year " });
                }

                return new SelectList(lstYears, "MAST_YEAR_CODE", "MAST_YEAR_TEXT", DateTime.Now.Year);
            }
            catch
            {
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }



        public SelectList PopulateFinancialYears(bool populateFirstItem = true, bool isAllYearsSelected = false)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<MASTER_YEAR> lstYears = dbContext.MASTER_YEAR.Where(y => y.MAST_YEAR_CODE <= DateTime.Now.Year).OrderByDescending(y => y.MAST_YEAR_CODE).ToList<MASTER_YEAR>();

                if (populateFirstItem && isAllYearsSelected)
                {
                    lstYears.Insert(0, new MASTER_YEAR() { MAST_YEAR_CODE = 0, MAST_YEAR_TEXT = "All Years" });



                }
                else
                {
                    lstYears.Insert(0, new MASTER_YEAR() { MAST_YEAR_CODE = -1, MAST_YEAR_TEXT = "Select Year " });
                }

                return new SelectList(lstYears, "MAST_YEAR_CODE", "MAST_YEAR_TEXT", DateTime.Now.Year);
            }
            catch
            {
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// Populates the UNFREEZED Batchs
        /// From IMS_SANCTIONED Project 
        /// </summary>
        /// <param name="MAST_STATE_CODE"></param>
        /// <param name="IMS_YEAR"></param>
        /// <returns></returns>
        public List<SelectListItem> PopulateUnFreezedBatch(int MAST_STATE_CODE, int IMS_YEAR, bool isAllSelected = false, bool isPopulateOnlyTwoBatchs = false)
        {
            List<SelectListItem> batchList = new List<SelectListItem>();
            SelectListItem item;
            int count = 0;
            if (!isAllSelected)
            {
                item = new SelectListItem();
                item.Text = "Select Batch";
                item.Value = "0";
                item.Selected = true;
                batchList.Add(item);
            }
            else
            {
                item = new SelectListItem();
                item.Text = "All Batch";
                item.Value = "-1";
                item.Selected = true;
                batchList.Add(item);
            }
            if (MAST_STATE_CODE == 0 && IMS_YEAR == 0)
            {
                return batchList;
            }

            try
            {
                dbContext = new PMGSYEntities();

                var filter = (from d in dbContext.IMS_FREEZE_DETAILS
                              where
                                 d.MAST_STATE_CODE == MAST_STATE_CODE &&
                                 d.IMS_YEAR == IMS_YEAR &&
                                 d.IMS_FREEZE_STATUS == "F"
                                 ///Added by SAMMED A. PATIL on 03AUG2017
                                 && d.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme
                              select d.IMS_BATCH).Distinct().ToList();

                var query = (from c in dbContext.MASTER_BATCH
                             where
                             !filter.Contains(c.MAST_BATCH_CODE)
                             select new
                             {
                                 Text = c.MAST_BATCH_NAME,
                                 Value = c.MAST_BATCH_CODE
                             }).ToList();

                //var parentndcodeList = dbContext.ADMIN_DEPARTMENT.Where(x=>x.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).Select(x=>x.MAST_PARENT_ND_CODE).ToList();

                //var filter = (from c in dbContext.ADMIN_DEPARTMENT
                //                  where 
                //                    parentndcodeList.Contains(c.MAST_PARENT_ND_CODE)
                //                  select c.ADMIN_ND_CODE).ToList();

                //var batch = (from d in dbContext.IMS_SANCTIONED_PROJECTS
                //             where
                //                d.MAST_STATE_CODE == MAST_STATE_CODE &&
                //                d.IMS_YEAR == IMS_YEAR &&
                //                filter.Contains(d.MAST_DPIU_CODE)
                //             select d.IMS_BATCH).Distinct().ToList();

                //var query = (from b in dbContext.MASTER_BATCH
                //             where 
                //                 batch.Contains(b.MAST_BATCH_CODE)
                //                 select new {
                //                    Text = b.MAST_BATCH_NAME,
                //                    Value = b.MAST_BATCH_CODE
                //                 }).ToList();


                if (PMGSYSession.Current.PMGSYScheme == 1 || PMGSYSession.Current.PMGSYScheme == 3)///Changes for RCPLWE
                {
                    foreach (var data in query)
                    {
                        if (isPopulateOnlyTwoBatchs)
                        {
                            ++count;
                            if (count > 3)
                            {
                                break;
                            }
                        }

                        item = new SelectListItem();
                        item.Text = data.Text;
                        item.Value = data.Value.ToString();
                        batchList.Add(item);
                    }
                }
                //else if (PMGSYSession.Current.PMGSYScheme == 2 || PMGSYSession.Current.PMGSYScheme == 4) //only 3 batches are allowed for PMGSY Scheme II, PMGSY3
                else if (PMGSYSession.Current.PMGSYScheme == 2)
                {
                    foreach (var data in query)
                    {
                        ++count;
                        if (count > 3)
                        {
                            break;
                        }

                        item = new SelectListItem();
                        item.Text = data.Text;
                        item.Value = data.Value.ToString();
                        batchList.Add(item);
                    }
                }
                else if (PMGSYSession.Current.PMGSYScheme == 4)
                {
                    foreach (var data in query)
                    {
                        item = new SelectListItem();
                        item.Text = data.Text;
                        item.Value = data.Value.ToString();
                        batchList.Add(item);
                    }
                }
                // Added by Srishti on 03-07-2023
                else if (PMGSYSession.Current.PMGSYScheme == 5)
                {
                    foreach (var data in query)
                    {
                        item = new SelectListItem();
                        item.Text = data.Text;
                        item.Value = data.Value.ToString();
                        batchList.Add(item);
                    }
                }
                return batchList;
            }
            catch
            {
                return batchList;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        public int GetShareCodeByStateCode(int StateCode)
        {
            dbContext = new PMGSYEntities();

            try
            {

                var shareCodeDetails = (from StateFundSharingDetaisls in dbContext.MASTER_STATE_FUND_SHARING_MAPPING
                                        join SharingDetails in dbContext.MASTER_FUND_SHARING
                                        on StateFundSharingDetaisls.MAST_SHARE_CODE equals SharingDetails.MAST_SHARE_CODE
                                        where StateFundSharingDetaisls.MAST_STATE_CODE == StateCode
                                        select SharingDetails.MAST_SHARE_CODE).FirstOrDefault();
                return shareCodeDetails;
            }
            catch
            {
                return 0;
            }
            finally
            {
                dbContext.Dispose();
            }

        }
        /// <summary>
        /// Populates all Batches
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> PopulateBatch()
        {
            List<SelectListItem> batchList = new List<SelectListItem>();
            SelectListItem item;
            int count = 0;
            item = new SelectListItem();
            item.Text = "Select Batch";
            item.Value = "0";
            item.Selected = true;
            batchList.Add(item);

            try
            {
                dbContext = new PMGSYEntities();
                var query = (from c in dbContext.MASTER_BATCH
                             select new
                             {
                                 Text = c.MAST_BATCH_NAME,
                                 Value = c.MAST_BATCH_CODE
                             }).ToList();


                // Populate all batches
                if (PMGSYSession.Current.PMGSYScheme == 1 || PMGSYSession.Current.PMGSYScheme == 3)///Changes for RCPLWE
                {
                    foreach (var data in query)
                    {
                        item = new SelectListItem();
                        item.Text = data.Text;
                        item.Value = data.Value.ToString();
                        batchList.Add(item);
                    }
                }
                else if (PMGSYSession.Current.PMGSYScheme == 2 || PMGSYSession.Current.PMGSYScheme == 4) //only 3 batches are allowed for PMGSY Scheme II, PMGSY3
                {
                    foreach (var data in query)
                    {
                        //++count;
                        //if (count > 3)
                        //{
                        //    break;
                        //}

                        item = new SelectListItem();
                        item.Text = data.Text;
                        item.Value = data.Value.ToString();
                        batchList.Add(item);
                    }
                }
                //Changed By Hrishikesh For STA and PTA --start --12-07-2023
                else if (PMGSYSession.Current.PMGSYScheme == 5) //only 3 batches are allowed for PMGSY Scheme II, PMGSY3
                {
                    foreach (var data in query)
                    {
                        item = new SelectListItem();
                        item.Text = data.Text;
                        item.Value = data.Value.ToString();
                        batchList.Add(item);
                    }
                }
                //Changed By Hrishikesh For STA and PTA --End --12-07-2023

                return batchList;
            }
            catch
            {
                return batchList;
            }
            finally
            {
                dbContext.Dispose();
            }

        }

        public List<SelectListItem> PopulateFundingAgency(bool populateAllOption = false)
        {
            List<SelectListItem> FundingAgencyList = new List<SelectListItem>();
            SelectListItem item;

            if (populateAllOption)
            {
                item = new SelectListItem();
                item.Text = "All Funding Agency";
                item.Value = "-1";
                item.Selected = true;
                FundingAgencyList.Add(item);
            }
            else
            {
                item = new SelectListItem();
                item.Text = "Select Funding Agency";
                item.Value = "0";
                item.Selected = true;
                FundingAgencyList.Add(item);
            }
            try
            {
                dbContext = new PMGSYEntities();
                var query = (from c in dbContext.MASTER_FUNDING_AGENCY
                             select new
                             {
                                 Text = c.MAST_FUNDING_AGENCY_NAME,
                                 Value = c.MAST_FUNDING_AGENCY_CODE
                             }).OrderBy(c => c.Text).ToList();

                foreach (var data in query)
                {
                    item = new SelectListItem();
                    item.Text = data.Text;
                    item.Value = data.Value.ToString();
                    FundingAgencyList.Add(item);
                }
                return FundingAgencyList;
            }
            catch
            {
                return FundingAgencyList;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public List<SelectListItem> PopulateStates(bool isPopulateFirstItem = true)
        {
            List<SelectListItem> StatesList = new List<SelectListItem>();
            SelectListItem item;

            if (isPopulateFirstItem)
            {
                item = new SelectListItem();
                item.Text = "Select State";
                item.Value = "0";
                item.Selected = true;
                StatesList.Add(item);
            }
            else
            {
                item = new SelectListItem();
                item.Text = "All States";
                item.Value = "0";
                item.Selected = true;
                StatesList.Add(item);

            }

            try
            {
                dbContext = new PMGSYEntities();
                var query = (from c in dbContext.MASTER_STATE
                             where c.MAST_STATE_ACTIVE == "Y"
                             select new
                             {
                                 Text = c.MAST_STATE_NAME,
                                 Value = c.MAST_STATE_CODE
                             }).OrderBy(c => c.Text).ToList();

                foreach (var data in query)
                {
                    item = new SelectListItem();
                    item.Text = data.Text;
                    item.Value = data.Value.ToString();
                    StatesList.Add(item);
                }
                return StatesList;
            }
            catch
            {
                return StatesList;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public List<SelectListItem> PopulateStreams(string MAST_STREAM_TYPE = "", bool isAllStreamsSelected = false)
        {
            List<SelectListItem> StreamList = new List<SelectListItem>();
            SelectListItem item;
            if (!isAllStreamsSelected)
            {
                item = new SelectListItem();
                item.Text = "Select Technology Proposed";
                item.Value = "0";
                item.Selected = true;
                StreamList.Add(item);
            }
            else
            {
                item = new SelectListItem();
                item.Text = "All";
                item.Value = "-1";
                item.Selected = true;
                StreamList.Add(item);
            }
            try
            {
                if (MAST_STREAM_TYPE == "")
                {
                    dbContext = new PMGSYEntities();
                    var query = (from c in dbContext.MASTER_STREAMS
                                 select new
                                 {
                                     Text = c.MAST_STREAM_NAME,
                                     Value = c.MAST_STREAM_CODE
                                 }).OrderBy(c => c.Text).ToList();

                    foreach (var data in query)
                    {
                        item = new SelectListItem();
                        item.Text = data.Text;
                        item.Value = data.Value.ToString();
                        StreamList.Add(item);
                    }
                    return StreamList;
                }
                else
                {
                    dbContext = new PMGSYEntities();
                    var query = (from c in dbContext.MASTER_STREAMS
                                 where
                                 c.MAST_STREAM_TYPE == MAST_STREAM_TYPE
                                 select new
                                 {
                                     Text = c.MAST_STREAM_NAME,
                                     Value = c.MAST_STREAM_CODE
                                 }).OrderBy(c => c.Text).ToList();

                    foreach (var data in query)
                    {
                        item = new SelectListItem();
                        item.Text = data.Text;
                        item.Value = data.Value.ToString();
                        StreamList.Add(item);
                    }
                    return StreamList;
                }
            }
            catch
            {
                return StreamList;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public List<SelectListItem> PopulateBlocksforRCPLWE(int MAST_DISTRICT_CODE, bool isAllBlocksSelected = false)
        {

            List<SelectListItem> BlockList = new List<SelectListItem>();
            SelectListItem item;
            if (!isAllBlocksSelected)
            {
                item = new SelectListItem();
                item.Text = "Select Block";
                item.Value = "0";
                item.Selected = true;
                BlockList.Add(item);
            }
            else
            {
                item = new SelectListItem();
                item.Text = "All Blocks";
                item.Value = "-1";
                item.Selected = true;
                BlockList.Add(item);
            }
            try
            {
                dbContext = new PMGSYEntities();
                var query = (from c in dbContext.MASTER_BLOCK
                             where
                             c.MAST_DISTRICT_CODE == MAST_DISTRICT_CODE && c.MAST_BLOCK_ACTIVE == "Y" && c.MAST_IAP_BLOCK == "Y"
                             select new
                             {
                                 Text = c.MAST_BLOCK_NAME,
                                 Value = c.MAST_BLOCK_CODE
                             }).ToList();

                foreach (var data in query)
                {
                    item = new SelectListItem();
                    item.Text = data.Text;
                    item.Value = data.Value.ToString();
                    BlockList.Add(item);
                }
                return BlockList;
            }
            catch
            {
                return BlockList;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        public List<SelectListItem> PopulateBlocks(int MAST_DISTRICT_CODE, bool isAllBlocksSelected = false)
        {

            List<SelectListItem> BlockList = new List<SelectListItem>();
            SelectListItem item;
            if (!isAllBlocksSelected)
            {
                item = new SelectListItem();
                item.Text = "Select Block";
                item.Value = "0";
                item.Selected = true;
                BlockList.Add(item);
            }
            else
            {
                item = new SelectListItem();
                item.Text = "All Blocks";
                item.Value = "-1";
                item.Selected = true;
                BlockList.Add(item);
            }
            try
            {
                dbContext = new PMGSYEntities();
                var query = (from c in dbContext.MASTER_BLOCK
                             where
                             c.MAST_DISTRICT_CODE == MAST_DISTRICT_CODE && c.MAST_BLOCK_ACTIVE == "Y"
                             select new
                             {
                                 Text = c.MAST_BLOCK_NAME,
                                 Value = c.MAST_BLOCK_CODE
                             }).ToList();

                foreach (var data in query)
                {
                    item = new SelectListItem();
                    item.Text = data.Text;
                    item.Value = data.Value.ToString();
                    BlockList.Add(item);
                }
                return BlockList;
            }
            catch
            {
                return BlockList;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public List<SelectListItem> PopulateMPConstituency(int MAST_BLOCK_CODE)
        {
            List<SelectListItem> MPCList = new List<SelectListItem>();
            SelectListItem item;

            item = new SelectListItem();
            item.Text = "Select MP Constituency";
            item.Value = "0";
            item.Selected = true;
            MPCList.Add(item);

            if (MAST_BLOCK_CODE == 0)
            {
                return MPCList;
            }

            try
            {
                dbContext = new PMGSYEntities();
                List<int> filter = (from d in dbContext.MASTER_MP_BLOCKS
                                    where d.MAST_BLOCK_CODE == MAST_BLOCK_CODE &&
                                    d.MAST_MP_BLOCK_ACTIVE.Equals("Y")
                                    select d.MAST_MP_CONST_CODE).ToList<int>();


                var query = (from c in dbContext.MASTER_MP_CONSTITUENCY
                             where c.MAST_MP_CONST_ACTIVE.Equals("Y") &&
                             filter.Contains(c.MAST_MP_CONST_CODE)
                             select new
                             {
                                 Text = c.MAST_MP_CONST_NAME,
                                 Value = c.MAST_MP_CONST_CODE
                             }).ToList();

                foreach (var data in query)
                {
                    item = new SelectListItem();
                    item.Text = data.Text;
                    item.Value = data.Value.ToString();
                    MPCList.Add(item);
                }
                return MPCList;
            }
            catch
            {
                return MPCList;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public List<SelectListItem> PopulateMLAConstituency(int MAST_BLOCK_CODE)
        {
            List<SelectListItem> MLACList = new List<SelectListItem>();
            SelectListItem item;

            item = new SelectListItem();
            item.Text = "Select MLA Constituency";
            item.Value = "0";
            item.Selected = true;
            MLACList.Add(item);

            if (MAST_BLOCK_CODE == 0)
            {
                return MLACList;
            }
            try
            {
                dbContext = new PMGSYEntities();
                List<int> filter = (from d in dbContext.MASTER_MLA_BLOCKS
                                    where d.MAST_BLOCK_CODE == MAST_BLOCK_CODE &&
                                    d.MAST_MLA_BLOCK_ACTIVE.Equals("Y")
                                    select d.MAST_MLA_CONST_CODE).ToList<int>();    //

                var query = (from c in dbContext.MASTER_MLA_CONSTITUENCY
                             where
                             c.MAST_MLA_CONST_ACTIVE.Equals("Y") &&
                             filter.Contains(c.MAST_MLA_CONST_CODE)
                             select new
                             {
                                 Text = c.MAST_MLA_CONST_NAME,
                                 Value = c.MAST_MLA_CONST_CODE
                             }).ToList();

                foreach (var data in query)
                {
                    item = new SelectListItem();
                    item.Text = data.Text;
                    item.Value = data.Value.ToString();
                    MLACList.Add(item);
                }
                return MLACList;
            }
            catch
            {
                return MLACList;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public List<SelectListItem> PopulateReason(string MAST_REASON_TYPE, bool populateItems = true)
        {
            List<SelectListItem> ReasonList = new List<SelectListItem>();
            SelectListItem item;

            item = new SelectListItem();
            item.Text = "Select Reason";
            item.Value = "0";
            item.Selected = true;
            ReasonList.Add(item);

            if (populateItems)
            {
                try
                {
                    dbContext = new PMGSYEntities();
                    var query = (from c in dbContext.MASTER_REASON
                                 where c.MAST_REASON_TYPE == MAST_REASON_TYPE
                                 select new
                                 {
                                     Text = c.MAST_REASON_NAME,
                                     Value = c.MAST_REASON_CODE
                                 }).ToList();

                    foreach (var data in query)
                    {
                        item = new SelectListItem();
                        item.Text = data.Text;
                        item.Value = data.Value.ToString();
                        ReasonList.Add(item);
                    }
                    return ReasonList;
                }
                catch
                {
                    return ReasonList;
                }
                finally
                {
                    dbContext.Dispose();
                }
            }
            return ReasonList;
        }

        public List<SelectListItem> PopulateSurfaceType(bool PopulateFirstItem = true)
        {
            List<SelectListItem> SurfaceTypeList = new List<SelectListItem>();
            SelectListItem item;
            if (PopulateFirstItem)
            {
                item = new SelectListItem();
                item.Text = "Select Surface Type";
                item.Value = "0";
                item.Selected = true;
                SurfaceTypeList.Add(item);
            }
            try
            {
                dbContext = new PMGSYEntities();
                var query = (from c in dbContext.MASTER_SURFACE
                             select new
                             {
                                 Text = c.MAST_SURFACE_NAME,
                                 Value = c.MAST_SURFACE_CODE
                             }).OrderBy(c => c.Text).ToList();

                foreach (var data in query)
                {
                    item = new SelectListItem();
                    item.Text = data.Text;
                    item.Value = data.Value.ToString();
                    SurfaceTypeList.Add(item);
                }
                return SurfaceTypeList;
            }
            catch
            {
                return SurfaceTypeList;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public List<SelectListItem> PopulateTrafficType()
        {
            List<SelectListItem> TrafficTypeList = new List<SelectListItem>();
            SelectListItem item;

            item = new SelectListItem();
            item.Text = "Select Traffic Type";
            item.Value = "0";
            item.Selected = true;
            TrafficTypeList.Add(item);

            try
            {
                dbContext = new PMGSYEntities();
                var query = (from c in dbContext.MASTER_TRAFFIC_TYPE
                             where c.MAST_TRAFFIC_STATUS == "Y"
                             select new
                             {
                                 Text = c.MAST_TRAFFIC_NAME,
                                 Value = c.MAST_TRAFFIC_CODE
                             }).ToList();

                foreach (var data in query)
                {
                    item = new SelectListItem();
                    item.Text = data.Text;
                    item.Value = data.Value.ToString();
                    TrafficTypeList.Add(item);
                }
                return TrafficTypeList;
            }
            catch
            {
                return TrafficTypeList;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public List<SelectListItem> PopulateProposedSurface()
        {
            List<SelectListItem> surfaceType = new List<SelectListItem>();
            SelectListItem item = new SelectListItem();
            item.Text = "Select Surface Type";
            item.Value = "";
            item.Selected = true;
            surfaceType.Add(item);

            item = new SelectListItem();
            item.Text = "Sealed";
            item.Value = "S";
            surfaceType.Add(item);

            item = new SelectListItem();
            item.Text = "UnSealed";
            item.Value = "U";
            surfaceType.Add(item);

            return surfaceType;
        }

        public List<SelectListItem> PopulateProposalTypes()
        {
            List<SelectListItem> ProposalType = new List<SelectListItem>();
            SelectListItem item = new SelectListItem();
            item.Text = "Road";
            item.Value = "P";
            item.Selected = true;

            ProposalType.Add(item);

            item = new SelectListItem();
            item.Text = "Bridges";
            item.Value = "L";
            ProposalType.Add(item);

            item = new SelectListItem();
            item.Text = "Building";
            item.Value = "B";
            ProposalType.Add(item);

            item = new SelectListItem();
            item.Text = "All";
            item.Value = "A";
            ProposalType.Add(item);

            return ProposalType;
        }

        public List<SelectListItem> PopulateCarriageWidth()
        {
            List<SelectListItem> CarriageWidthList = new List<SelectListItem>();
            SelectListItem item;

            item = new SelectListItem();
            item.Text = "Select Carriage Width";
            item.Value = "0";
            item.Selected = true;
            CarriageWidthList.Add(item);

            try
            {
                dbContext = new PMGSYEntities();
                var query = (from c in dbContext.MASTER_CARRIAGE
                             where c.MAST_CARRIAGE_STATUS == "Y"
                             select new
                             {
                                 Text = c.MAST_CARRIAGE_WIDTH,
                                 Value = c.MAST_CARRIAGE_CODE
                             }).ToList();

                foreach (var data in query)
                {
                    item = new SelectListItem();
                    item.Text = data.Text.ToString();
                    item.Value = data.Value.ToString();
                    CarriageWidthList.Add(item);
                }
                return CarriageWidthList;
            }
            catch
            {
                return CarriageWidthList;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public List<SelectListItem> PopulateUpToYear(bool populateFirstItem = true, bool isAllYearsSelected = false, int FromYear = 0)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<SelectListItem> yearList = new List<SelectListItem>();
                SelectListItem item;

                if (populateFirstItem)
                {
                    item = new SelectListItem();
                    item.Text = "Select Year";
                    item.Value = "0";
                    item.Selected = true;
                    yearList.Add(item);
                }
                else
                {
                    item = new SelectListItem();
                    item.Text = "All Years";
                    item.Value = "-1";
                    item.Selected = true;
                    yearList.Add(item);
                }

                if (FromYear == 0)
                {
                    List<MASTER_YEAR> lstYears = dbContext.MASTER_YEAR.Where(y => y.MAST_YEAR_CODE <= DateTime.Now.Year).OrderByDescending(y => y.MAST_YEAR_CODE).ToList<MASTER_YEAR>();
                    foreach (MASTER_YEAR year in lstYears)
                    {
                        item = new SelectListItem();
                        item.Text = year.MAST_YEAR_TEXT;
                        item.Value = year.MAST_YEAR_CODE.ToString();
                        yearList.Add(item);
                    }
                }
                else
                {
                    List<MASTER_YEAR> lstYears = dbContext.MASTER_YEAR.Where(y => y.MAST_YEAR_CODE <= DateTime.Now.Year && y.MAST_YEAR_CODE >= FromYear).OrderByDescending(y => y.MAST_YEAR_CODE).ToList<MASTER_YEAR>();

                    foreach (MASTER_YEAR year in lstYears)
                    {
                        item = new SelectListItem();
                        item.Text = year.MAST_YEAR_TEXT;
                        item.Value = year.MAST_YEAR_CODE.ToString();
                        yearList.Add(item);
                    }
                }
                return yearList;
            }
            catch
            {
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        public List<SelectListItem> PopulateDesignationSpecific(bool populateAllOption = false)
        {
            List<SelectListItem> DesignationList = new List<SelectListItem>();
            SelectListItem item;

            if (populateAllOption)
            {
                item = new SelectListItem();
                item.Text = "All Designation";
                item.Value = "0";
                item.Selected = true;
                DesignationList.Add(item);
            }
            else
            {
                item = new SelectListItem();
                item.Text = "Select Designation";
                item.Value = "0";
                item.Selected = true;
                DesignationList.Add(item);
            }
            try
            {
                dbContext = new PMGSYEntities();

                var query = (from c in dbContext.MASTER_DESIGNATION
                             where
                             (c.MAST_DESIG_CODE == 117 ||  //Junior Engg
                                c.MAST_DESIG_CODE == 24 ||   //Assist. Engg.
                                c.MAST_DESIG_CODE == 80 ||   //Exec Engg
                                c.MAST_DESIG_CODE == 169 ||  //Sup. Engg
                                c.MAST_DESIG_CODE == 139      //Other Officer
                             )
                             select new
                             {
                                 Text = c.MAST_DESIG_NAME,
                                 Value = c.MAST_DESIG_CODE
                             }).OrderBy(c => c.Text).ToList();

                foreach (var data in query)
                {
                    item = new SelectListItem();
                    item.Text = data.Text;
                    item.Value = data.Value.ToString();
                    DesignationList.Add(item);
                }
                return DesignationList;
            }
            catch
            {
                return DesignationList;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        #endregion  

        #region LSB Proposal

        public List<SelectListItem> PopulateScourTypeFoundation(bool isFoundationType)
        {

            List<SelectListItem> FoundationTypeList = new List<SelectListItem>();
            SelectListItem item;
            string FD_Type = string.Empty;
            try
            {
                if (isFoundationType)
                    FD_Type = "F"; //for Foundation Type Dropdown
                else
                    FD_Type = "S"; // for Scour Type Dropdown

                dbContext = new PMGSYEntities();
                var query = (from c in dbContext.MASTER_SCOUR_FOUNDATION_TYPE
                             where c.IMS_SC_FD_TYPE == FD_Type
                             select new
                             {
                                 Text = c.IMS_SC_FD_NAME,
                                 Value = c.IMS_SC_FD_CODE
                             }).OrderBy(c => c.Text).ToList();

                foreach (var data in query)
                {
                    item = new SelectListItem();
                    item.Text = data.Text;
                    item.Value = data.Value.ToString();
                    FoundationTypeList.Add(item);
                }
                return FoundationTypeList;

            }
            catch
            {
                return FoundationTypeList;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        public List<SelectListItem> PopulateWidthOfBridge()
        {
            List<SelectListItem> lstWidthOfBridge = new List<SelectListItem>();
            SelectListItem item = new SelectListItem();
            item.Text = "4.25";
            item.Value = "4.25";
            lstWidthOfBridge.Add(item);

            item = new SelectListItem();
            item.Text = "5.50";
            item.Value = "5.50";
            lstWidthOfBridge.Add(item);

            item = new SelectListItem();
            item.Text = "7.50";
            item.Value = "7.50";
            lstWidthOfBridge.Add(item);

            item = new SelectListItem();
            item.Text = "8.50";
            item.Value = "8.50";
            lstWidthOfBridge.Add(item);
            return lstWidthOfBridge;
        }

        public List<SelectListItem> PopulateLSBComponentType(int pr_road_code)
        {
            List<SelectListItem> ComponentTypeList = new List<SelectListItem>();
            SelectListItem item;
            string FD_Type = string.Empty;
            try
            {
                dbContext = new PMGSYEntities();
                var filterComponets = (from bcd in dbContext.IMS_LSB_BRIDGE_COMPONENT_DETAIL
                                       where bcd.IMS_PR_ROAD_CODE == pr_road_code
                                       select bcd.IMS_COMPONENT_CODE);

                var query = (from c in dbContext.MASTER_COMPONENT_TYPE
                             where !filterComponets.Contains(c.MAST_COMPONENT_CODE)
                             select new
                             {
                                 Text = c.MAST_COMPONENT_NAME,
                                 Value = c.MAST_COMPONENT_CODE
                             }).OrderBy(c => c.Text).ToList();



                foreach (var data in query)
                {
                    item = new SelectListItem();
                    item.Text = data.Text;
                    item.Value = data.Value.ToString();
                    ComponentTypeList.Add(item);
                }
                return ComponentTypeList;

            }
            catch
            {
                return ComponentTypeList;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        #endregion

        #region added by Sammed Patil

        public string _IndianFormatNumber(string Amount)
        {
            double sAmount = Convert.ToDouble(Amount);

            bool isNegativeNumber = false;
            if (sAmount < 0)
            {
                isNegativeNumber = true;
                sAmount = sAmount * -1;
            }

            NumberFormatInfo nFmtInfo = new NumberFormatInfo();
            nFmtInfo.CurrencyDecimalDigits = 0;
            int[] INDSizes = { 3, 2, 2 };

            nFmtInfo.CurrencyDecimalSeparator = ".";
            nFmtInfo.CurrencyGroupSeparator = ",";
            nFmtInfo.CurrencySymbol = "Rs.";
            nFmtInfo.CurrencyGroupSizes = INDSizes;
            String cAmount = sAmount.ToString("C", nFmtInfo);
            //double dblValue = 1234567890;
            Int32 strLen = cAmount.Length - 1;
            try
            {
                String teast = cAmount.Substring(3).ToString();
                if (isNegativeNumber)
                    return "-" + teast;
                else
                    return teast;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        #endregion

        #region added by Sammed Patil

        public string _IndianFormatLength(string Amount)
        {
            double sAmount = Convert.ToDouble(Amount);

            bool isNegativeNumber = false;
            if (sAmount < 0)
            {
                isNegativeNumber = true;
                sAmount = sAmount * -1;
            }

            NumberFormatInfo nFmtInfo = new NumberFormatInfo();
            nFmtInfo.CurrencyDecimalDigits = 3;
            int[] INDSizes = { 3, 2, 2 };

            nFmtInfo.CurrencyDecimalSeparator = ".";
            nFmtInfo.CurrencyGroupSeparator = ",";
            nFmtInfo.CurrencySymbol = "Rs.";
            nFmtInfo.CurrencyGroupSizes = INDSizes;
            String cAmount = sAmount.ToString("C", nFmtInfo);
            //double dblValue = 1234567890;
            Int32 strLen = cAmount.Length - 1;
            try
            {
                String teast = cAmount.Substring(3).ToString();
                if (isNegativeNumber)
                    return "-" + teast;
                else
                    return teast;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        #endregion

        #region added by Shyam Yadav

        public string _IndianFormatAmount(string Amount)
        {
            double sAmount = Convert.ToDouble(Amount);

            bool isNegativeNumber = false;
            if (sAmount < 0)
            {
                isNegativeNumber = true;
                sAmount = sAmount * -1;
            }

            NumberFormatInfo nFmtInfo = new NumberFormatInfo();
            nFmtInfo.CurrencyDecimalDigits = 2;
            int[] INDSizes = { 3, 2, 2 };

            nFmtInfo.CurrencyDecimalSeparator = ".";
            nFmtInfo.CurrencyGroupSeparator = ",";
            nFmtInfo.CurrencySymbol = "Rs.";
            nFmtInfo.CurrencyGroupSizes = INDSizes;
            String cAmount = sAmount.ToString("C", nFmtInfo);
            //double dblValue = 1234567890;
            Int32 strLen = cAmount.Length - 1;
            try
            {
                String teast = cAmount.Substring(3).ToString();
                if (isNegativeNumber)
                    return "-" + teast;
                else
                    return teast;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        #endregion


        public List<SelectListItem> PopulateDPIUOfSRRDA(int SRRDACode)
        {
            List<SelectListItem> lstDPIU = null;
            try
            {
                dbContext = new PMGSYEntities();
                lstDPIU = new SelectList(dbContext.ADMIN_DEPARTMENT.Where(m => m.MAST_PARENT_ND_CODE == SRRDACode && m.MAST_ND_TYPE == "D"), "ADMIN_ND_CODE", "ADMIN_ND_NAME").ToList();

                //if (lstDPIU == null || lstDPIU.Count() == 0)
                //{
                //    lstDPIU.Insert(0, (new SelectListItem { Text = "Select Department", Value = "0", Selected = true }));
                //}
                lstDPIU.Insert(0, (new SelectListItem { Text = "Select Department", Value = "0", Selected = true }));
                return lstDPIU;
            }
            catch
            {
                return lstDPIU;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        //public List<SelectListItem> PopulateDPIUOfSRRDA(int SRRDACode)
        //{
        //    List<SelectListItem> lstDPIU = null;
        //    try
        //    {
        //        dbContext = new PMGSYEntities();
        //        lstDPIU = new SelectList(dbContext.ADMIN_DEPARTMENT.Where(m => m.MAST_PARENT_ND_CODE == SRRDACode && m.MAST_ND_TYPE == "D"), "ADMIN_ND_CODE", "ADMIN_ND_NAME").ToList();

        //        if (lstDPIU == null || lstDPIU.Count() == 0)
        //        {
        //            lstDPIU.Insert(0, (new SelectListItem { Text = "Select Department", Value = "0", Selected = true }));
        //        }

        //        return lstDPIU;
        //    }
        //    catch
        //    {
        //        return lstDPIU;
        //    }
        //    finally
        //    {
        //        dbContext.Dispose();
        //    }
        //}

        /// <summary>
        /// Populate 3rd Tier & 2nd Tier
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> PopulateQMTypes()
        {
            List<SelectListItem> lstQMTypes = new List<SelectListItem>();
            try
            {
                lstQMTypes.Insert(0, (new SelectListItem { Text = "3rd Tier Quality Monitoring", Value = "I", Selected = true }));
                lstQMTypes.Insert(1, (new SelectListItem { Text = "2nd Tier Quality Monitoring", Value = "S" }));

                return lstQMTypes;
            }
            catch
            {
                return lstQMTypes;
            }
        }

        /// <summary>
        /// Populate NQM/SQM Type
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> PopulateMonitorTypes()
        {
            List<SelectListItem> lstQMTypes = new List<SelectListItem>();
            try
            {
                lstQMTypes.Insert(0, (new SelectListItem { Text = "NQM", Value = "I", Selected = true }));
                lstQMTypes.Insert(1, (new SelectListItem { Text = "SQM", Value = "S" }));

                return lstQMTypes;
            }
            catch
            {
                return lstQMTypes;
            }
        }

        /// <summary>
        /// Get List of States in which monitor have done inspections.
        /// </summary>
        /// <param name="monitorCode"></param>
        /// <returns></returns>
        //public List<SelectListItem> PopulateMonitorsInspectedStates(int monitorCode, int frmMonth, int frmYear, int toMonth, int toYear)
        //{
        //    List<SelectListItem> lstMonitors = new List<SelectListItem>();
        //    dbContext = new PMGSYEntities();
        //    try
        //    {
        //        SelectListItem item = new SelectListItem();
        //         item.Text = "All States";
        //         item.Value = "0";
        //         item.Selected = true;
        //         lstMonitors.Add(item);

        //         if (monitorCode == 0)
        //         {
        //             var states = PopulateStates(false);
        //             foreach (var data in states)
        //             {
        //                 item = new SelectListItem();
        //                 item.Text = data.Text;
        //                 item.Value = data.Value.ToString();
        //                 lstMonitors.Add(item);
        //             }
        //         }
        //         else
        //         {
        //             DateTime frmDateToCompare = new DateTime(frmYear, frmMonth, 1);
        //             DateTime toDateToCompare = new DateTime(toYear, toMonth+1, 1);  //one month ahead
        //             var query = (from aqm in dbContext.ADMIN_QUALITY_MONITORS
        //                          join
        //                              qqs in dbContext.QUALITY_QM_SCHEDULE on aqm.ADMIN_QM_CODE equals qqs.ADMIN_QM_CODE
        //                          join
        //                              qqom in dbContext.QUALITY_QM_OBSERVATION_MASTER on qqs.ADMIN_SCHEDULE_CODE equals qqom.ADMIN_SCHEDULE_CODE
        //                          join
        //                              isp in dbContext.IMS_SANCTIONED_PROJECTS on qqom.IMS_PR_ROAD_CODE equals isp.IMS_PR_ROAD_CODE
        //                          join
        //                              ms in dbContext.MASTER_STATE on isp.MAST_STATE_CODE equals ms.MAST_STATE_CODE
        //                          where aqm.ADMIN_QM_CODE == monitorCode &&
        //                                qqom.QM_INSPECTION_DATE >= frmDateToCompare &&
        //                                qqom.QM_INSPECTION_DATE < toDateToCompare
        //                          select new
        //                          {
        //                              Value = ms.MAST_STATE_CODE,
        //                              Text = ms.MAST_STATE_NAME
        //                          }).Distinct().OrderBy(c => c.Text).ToList();

        //             foreach (var data in query)
        //             {
        //                 item = new SelectListItem();
        //                 item.Text = data.Text;
        //                 item.Value = data.Value.ToString();
        //                 lstMonitors.Add(item);
        //             }
        //         }


        //        return lstMonitors;
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //    finally
        //    {
        //        dbContext.Dispose();
        //    }
        //}

        /// <summary>
        /// Populate NQMs who have done inspection during particular time span
        /// </summary>
        /// <param name="frmMonth"></param>
        /// <param name="frmYear"></param>
        /// <param name="toMonth"></param>
        /// <param name="toYear"></param>
        /// <returns></returns>
        public List<SelectListItem> PopulateNQM(int frmMonth, int frmYear, int toMonth, int toYear)
        {
            List<SelectListItem> lstMonitors = new List<SelectListItem>();
            dbContext = new PMGSYEntities();
            try
            {
                SelectListItem item = new SelectListItem();
                item.Text = "All Monitors";
                item.Value = "0";
                item.Selected = true;
                lstMonitors.Add(item);

                DateTime frmDateToCompare = new DateTime(frmYear, frmMonth, 1);
                DateTime toDateToCompare = new DateTime(toYear, toMonth + 1, 1);  //one month ahead
                var query = (from aqm in dbContext.ADMIN_QUALITY_MONITORS
                             join
                                 qqs in dbContext.QUALITY_QM_SCHEDULE on aqm.ADMIN_QM_CODE equals qqs.ADMIN_QM_CODE
                             join
                                 qqom in dbContext.QUALITY_QM_OBSERVATION_MASTER on qqs.ADMIN_SCHEDULE_CODE equals qqom.ADMIN_SCHEDULE_CODE
                             join
                                 isp in dbContext.IMS_SANCTIONED_PROJECTS on qqom.IMS_PR_ROAD_CODE equals isp.IMS_PR_ROAD_CODE
                             join
                                 ms in dbContext.MASTER_STATE on isp.MAST_STATE_CODE equals ms.MAST_STATE_CODE
                             where qqom.QM_INSPECTION_DATE >= frmDateToCompare &&
                                   qqom.QM_INSPECTION_DATE < toDateToCompare &&
                                   aqm.ADMIN_QM_TYPE == "I"
                             select new
                             {
                                 Value = aqm.ADMIN_QM_CODE,
                                 Text = (aqm.ADMIN_QM_LNAME.Equals(null) ? "" : aqm.ADMIN_QM_LNAME) + " " + (aqm.ADMIN_QM_FNAME.Equals(null) ? "" : aqm.ADMIN_QM_FNAME) + " " + (aqm.ADMIN_QM_MNAME.Equals(null) ? "" : aqm.ADMIN_QM_MNAME)
                             }).Distinct().OrderBy(c => c.Text).ToList();

                foreach (var data in query)
                {
                    item = new SelectListItem();
                    item.Text = data.Text;
                    item.Value = data.Value.ToString();
                    lstMonitors.Add(item);
                }

                return lstMonitors;
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
        /// @author : Shyam Yadav
        /// Purposely taken isPopulateFirstSelect as String instead of bool, if null then no need to take default option 
        /// </summary>
        /// <param name="isPopulateFirstSelect"></param>
        /// <param name="qmType"></param>
        /// <param name="stateCode"></param>
        /// <returns></returns>
        public List<SelectListItem> PopulateMonitors(string isPopulateFirstSelect, string qmType, int stateCode)
        {
            List<SelectListItem> lstProfileNames = new List<SelectListItem>();
            dbContext = new PMGSYEntities();
            try
            {
                SelectListItem item = new SelectListItem();
                if (isPopulateFirstSelect.Equals("true"))
                {
                    item.Text = "Select Monitor";
                    item.Value = "0";
                    item.Selected = true;
                    lstProfileNames.Add(item);
                }
                else if (isPopulateFirstSelect.Equals("false"))
                {
                    item.Text = "All Monitors";
                    item.Value = "0";
                    item.Selected = true;
                    lstProfileNames.Add(item);
                }

                if (qmType.Equals("0"))
                {
                    return lstProfileNames;
                }

                if (stateCode == 0)
                {
                    var query = (from c in dbContext.ADMIN_QUALITY_MONITORS
                                 where c.ADMIN_QM_TYPE == qmType
                                 && c.ADMIN_QM_EMPANELLED == "Y"
                                 select new
                                 {
                                     Value = c.ADMIN_QM_CODE,
                                     Text = (c.ADMIN_QM_FNAME.Equals(null) ? "" : c.ADMIN_QM_FNAME) + " " + (c.ADMIN_QM_MNAME.Equals(null) ? "" : c.ADMIN_QM_MNAME) + " " + (c.ADMIN_QM_LNAME.Equals(null) ? "" : c.ADMIN_QM_LNAME)
                                 }).OrderBy(c => c.Text).ToList();

                    foreach (var data in query)
                    {
                        item = new SelectListItem();
                        item.Text = data.Text;
                        item.Value = data.Value.ToString();
                        lstProfileNames.Add(item);
                    }
                }
                else
                {
                    var query = dbContext.qm_statewise_inspection_monitosr_list(stateCode, qmType).ToList();

                    foreach (var data in query)
                    {
                        item = new SelectListItem();
                        item.Text = data.MONITOR_NAME;
                        item.Value = data.ADMIN_QM_CODE.ToString();
                        lstProfileNames.Add(item);
                    }
                }



                return lstProfileNames;
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
                return null;
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

        //Populate All Monitors
        public List<SelectListItem> PopulateAllMonitors(string isPopulateFirstSelect, string qmType, int stateCode)
        {
            List<SelectListItem> lstProfileNames = new List<SelectListItem>();
            dbContext = new PMGSYEntities();
            try
            {
                SelectListItem item = new SelectListItem();
                if (isPopulateFirstSelect.Equals("true"))
                {
                    item.Text = "Select Monitor";
                    item.Value = "0";
                    item.Selected = true;
                    lstProfileNames.Add(item);
                }
                else if (isPopulateFirstSelect.Equals("false"))
                {
                    item.Text = "All Monitors";
                    item.Value = "0";
                    item.Selected = true;
                    lstProfileNames.Add(item);
                }

                if (qmType.Equals("0"))
                {
                    return lstProfileNames;
                }

                if (stateCode == 0)
                {
                    var query = (from c in dbContext.ADMIN_QUALITY_MONITORS
                                 where c.ADMIN_QM_TYPE == qmType
                                 //&& c.ADMIN_QM_EMPANELLED == "Y"
                                 select new
                                 {
                                     Value = c.ADMIN_QM_CODE,
                                     Text = (c.ADMIN_QM_FNAME.Equals(null) ? "" : c.ADMIN_QM_FNAME) + " " + (c.ADMIN_QM_MNAME.Equals(null) ? "" : c.ADMIN_QM_MNAME) + " " + (c.ADMIN_QM_LNAME.Equals(null) ? "" : c.ADMIN_QM_LNAME)
                                 }).OrderBy(c => c.Text).ToList();

                    foreach (var data in query)
                    {
                        item = new SelectListItem();
                        item.Text = data.Text;
                        item.Value = data.Value.ToString();
                        lstProfileNames.Add(item);
                    }
                }
                else
                {
                    //var query = dbContext.qm_statewise_inspection_monitosr_list(stateCode, qmType).ToList();
                    var query = dbContext.qm_statewise_inspection_monitosr_list(stateCode, qmType).ToList();

                    foreach (var data in query)
                    {
                        item = new SelectListItem();
                        item.Text = data.MONITOR_NAME;
                        item.Value = data.ADMIN_QM_CODE.ToString();
                        lstProfileNames.Add(item);
                    }
                }



                return lstProfileNames;
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
                return null;
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
        /// Populate those monitors who have inspected works in particular District
        /// </summary>
        /// <param name="isPopulateFirstSelect"></param>
        /// <param name="qmType"></param>
        /// <param name="stateCode"></param>
        /// <param name="districtCode"></param>
        /// <returns></returns>
        public List<SelectListItem> PopulateMonitorsDistrictWise(string isPopulateFirstSelect, string qmType, int stateCode, int districtCode)
        {
            List<SelectListItem> lstProfileNames = new List<SelectListItem>();
            dbContext = new PMGSYEntities();
            try
            {
                SelectListItem item = new SelectListItem();
                if (isPopulateFirstSelect.Equals("true"))
                {
                    item.Text = "Select Monitor";
                    item.Value = "0";
                    item.Selected = true;
                    lstProfileNames.Add(item);
                }
                else if (isPopulateFirstSelect.Equals("false"))
                {
                    item.Text = "All Monitors";
                    item.Value = "0";
                    item.Selected = true;
                    lstProfileNames.Add(item);
                }

                if (qmType.Equals("0"))
                {
                    return lstProfileNames;
                }

                var query = dbContext.qm_districtwise_inspection_monitors_list(stateCode, districtCode, qmType).ToList();

                foreach (var data in query)
                {
                    item = new SelectListItem();
                    item.Text = data.MONITOR_NAME;
                    item.Value = data.ADMIN_QM_CODE.ToString();
                    lstProfileNames.Add(item);
                }
                return lstProfileNames;
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

        public List<SelectListItem> PopulateMonitorsForScheduleCreation(string isPopulateFirstSelect, string qmType, int stateCode, int month, int year)
        {
            List<SelectListItem> lstProfileNames = new List<SelectListItem>();
            dbContext = new PMGSYEntities();
            string monitorAge = "";
            int MonitorAge = 0;

            try
            {
                SelectListItem item = new SelectListItem();
                if (isPopulateFirstSelect.Equals("true"))
                {
                    item.Text = "Select Monitor";
                    item.Value = "0";
                    item.Selected = true;
                    lstProfileNames.Add(item);
                }
                else if (isPopulateFirstSelect.Equals("false"))
                {
                    item.Text = "All Monitors";
                    item.Value = "0";
                    item.Selected = true;
                    lstProfileNames.Add(item);
                }

                if (qmType.Equals("0"))
                {
                    return lstProfileNames;
                }
                //string monitorAge = dbContext.ADMIN_MODULE_CONFIGURATION.Where(m => m.Parameter == "MONITOR_AGE").Select(m => m.Value).FirstOrDefault();
                //int MonitorAge = Convert.ToInt32(monitorAge);
                if (String.Equals(qmType, "I"))
                {
                    monitorAge = dbContext.ADMIN_MODULE_CONFIGURATION.Where(m => m.Parameter == "MONITOR_AGE").Select(m => m.Value).FirstOrDefault();
                    MonitorAge = Convert.ToInt32(monitorAge);
                }
                else if (String.Equals(qmType, "S"))
                {
                    monitorAge = dbContext.ADMIN_MODULE_CONFIGURATION.Where(m => m.Parameter == "MONITOR_AGE_SQM").Select(m => m.Value).FirstOrDefault();
                    MonitorAge = Convert.ToInt32(monitorAge);
                }

                if (stateCode == 0)
                {
                    #region Old Code
                    //var filter = (from aqm in dbContext.ADMIN_QUALITY_MONITORS 
                    //             join qqs in dbContext.QUALITY_QM_SCHEDULE on aqm.ADMIN_QM_CODE equals qqs.ADMIN_QM_CODE
                    //             where aqm.ADMIN_QM_TYPE == qmType
                    //             && qqs.ADMIN_IM_MONTH == month 
                    //             && qqs.ADMIN_IM_YEAR == year
                    //             && aqm.ADMIN_QM_EMPANELLED == "Y"
                    //             select aqm.ADMIN_QM_CODE).Distinct().ToList();
                    #endregion

                    var query = (from aqm in dbContext.ADMIN_QUALITY_MONITORS
                                 where aqm.ADMIN_QM_TYPE == qmType
                                 && aqm.ADMIN_QM_EMPANELLED == "Y"
                                     //&& !filter.Contains(aqm.ADMIN_QM_CODE)
                                     //Added by SAMMED A. PATIL to restrict schedule assigning to monitors with age greater than 70
                                     //Added on 15 June 2021 to restrict schedule assigning to monitors with age greater than 67 years
                                 && (SqlFunctions.DateDiff("day", aqm.ADMIN_QM_BIRTH_DATE, DateTime.Now) <= MonitorAge || aqm.ADMIN_QM_BIRTH_DATE == null)
                                 select new
                                 {
                                     Value = aqm.ADMIN_QM_CODE,
                                     Text = (aqm.ADMIN_QM_LNAME.Equals(null) ? "" : aqm.ADMIN_QM_LNAME) + " " + (aqm.ADMIN_QM_FNAME.Equals(null) ? "" : aqm.ADMIN_QM_FNAME) + " " + (aqm.ADMIN_QM_MNAME.Equals(null) ? "" : aqm.ADMIN_QM_MNAME)
                                 }).OrderBy(aqm => aqm.Text).Distinct().OrderBy(a => a.Text).ToList();

                    foreach (var data in query)
                    {
                        item = new SelectListItem();
                        item.Text = data.Text;
                        item.Value = data.Value.ToString();
                        lstProfileNames.Add(item);
                    }
                }
                else
                {
                    #region Old code
                    //var filter = (from aqm in dbContext.ADMIN_QUALITY_MONITORS
                    //             join qqs in dbContext.QUALITY_QM_SCHEDULE on aqm.ADMIN_QM_CODE equals qqs.ADMIN_QM_CODE
                    //             join qqom in dbContext.QUALITY_QM_OBSERVATION_MASTER on qqs.ADMIN_SCHEDULE_CODE equals qqom.ADMIN_SCHEDULE_CODE
                    //             where aqm.ADMIN_QM_TYPE == qmType
                    //             && qqs.ADMIN_IM_MONTH == month
                    //             && qqs.ADMIN_IM_YEAR == year
                    //             && aqm.ADMIN_QM_EMPANELLED == "Y"
                    //             && (stateCode > 0 ? qqs.MAST_STATE_CODE : 1) == (stateCode > 0 ? stateCode : 1)
                    //             select aqm.ADMIN_QM_CODE).Distinct().ToList();
                    #endregion

                    //var query = (from aqm in dbContext.ADMIN_QUALITY_MONITORS
                    //             where aqm.ADMIN_QM_TYPE == qmType
                    //             && aqm.ADMIN_QM_EMPANELLED == "Y"
                    //             && (aqm.MAST_STATE_CODE == stateCode || aqm.MAST_STATE_CODE_ADDR == stateCode)

                    //             && (SqlFunctions.DateDiff("day", aqm.ADMIN_QM_BIRTH_DATE, DateTime.Now) <= MonitorAge || aqm.ADMIN_QM_BIRTH_DATE == null)
                    //             select new
                    //             {
                    //                 Value = aqm.ADMIN_QM_CODE,
                    //                 Text = (aqm.ADMIN_QM_LNAME.Equals(null) ? "" : aqm.ADMIN_QM_LNAME) + " " + (aqm.ADMIN_QM_FNAME.Equals(null) ? "" : aqm.ADMIN_QM_FNAME) + " " + (aqm.ADMIN_QM_MNAME.Equals(null) ? "" : aqm.ADMIN_QM_MNAME)
                    //             }).OrderBy(c => c.Text).Distinct().OrderBy(a => a.Text).ToList();


                    // Above query is commented and below query1 and query2 is added.

                    //var query1 = (from aqm in dbContext.ADMIN_QUALITY_MONITORS
                    //              where aqm.ADMIN_QM_TYPE == qmType
                    //              && aqm.ADMIN_QM_EMPANELLED == "Y"
                    //              && (aqm.MAST_STATE_CODE == stateCode || aqm.MAST_STATE_CODE_ADDR == stateCode)
                    //                  //Added on 15 June 2021 to restrict schedule assigning to monitors with age greater than 67 years
                    //              && (SqlFunctions.DateDiff("day", aqm.ADMIN_QM_BIRTH_DATE, DateTime.Now) <= MonitorAge || aqm.ADMIN_QM_BIRTH_DATE == null)
                    //              select new
                    //              {
                    //                  Value = aqm.ADMIN_QM_CODE,
                    //                  Text = (aqm.ADMIN_QM_LNAME.Equals(null) ? "" : aqm.ADMIN_QM_LNAME) + " " + (aqm.ADMIN_QM_FNAME.Equals(null) ? "" : aqm.ADMIN_QM_FNAME) + " " + (aqm.ADMIN_QM_MNAME.Equals(null) ? "" : aqm.ADMIN_QM_MNAME)
                    //              }).OrderBy(c => c.Text).Distinct().OrderBy(a => a.Text).ToList();


                    //Modified to assigning to monitors with age greater than 70 years
                    var query1 = (from aqm in dbContext.ADMIN_QUALITY_MONITORS
                                  where aqm.ADMIN_QM_TYPE == qmType
                                  && aqm.ADMIN_QM_EMPANELLED == "Y"
                                  && (aqm.MAST_STATE_CODE == stateCode || aqm.MAST_STATE_CODE_ADDR == stateCode)

                                  && (SqlFunctions.DateDiff("year", aqm.ADMIN_QM_BIRTH_DATE, DateTime.Now) <= MonitorAge || aqm.ADMIN_QM_BIRTH_DATE == null)
                                  select new
                                  {
                                      Value = aqm.ADMIN_QM_CODE,
                                      Text = (aqm.ADMIN_QM_LNAME.Equals(null) ? "" : aqm.ADMIN_QM_LNAME) + " " + (aqm.ADMIN_QM_FNAME.Equals(null) ? "" : aqm.ADMIN_QM_FNAME) + " " + (aqm.ADMIN_QM_MNAME.Equals(null) ? "" : aqm.ADMIN_QM_MNAME)
                                  }).OrderBy(c => c.Text).Distinct().OrderBy(a => a.Text).ToList();



                    var query2 = (from aqm in dbContext.ADMIN_QUALITY_MONITORS
                                  join interstate in dbContext.ADMIN_QUALITY_MONITORS_INTER_STATE on aqm.ADMIN_QM_CODE equals interstate.ADMIN_QM_CODE
                                  where interstate.ALLOWED_STATE_CODE == stateCode && interstate.APPROVED.Equals("Y") && aqm.ADMIN_QM_TYPE == qmType && aqm.ADMIN_QM_EMPANELLED == "Y"


                                  select new
                                  {
                                      Value = aqm.ADMIN_QM_CODE,
                                      Text = (aqm.ADMIN_QM_LNAME.Equals(null) ? "" : aqm.ADMIN_QM_LNAME) + " " + (aqm.ADMIN_QM_FNAME.Equals(null) ? "" : aqm.ADMIN_QM_FNAME) + " " + (aqm.ADMIN_QM_MNAME.Equals(null) ? "" : aqm.ADMIN_QM_MNAME)
                                  }).OrderBy(c => c.Text).Distinct().OrderBy(a => a.Text).ToList();

                    var query = query1.Union(query2);

                    foreach (var data in query)
                    {
                        item = new SelectListItem();
                        item.Text = data.Text;
                        item.Value = data.Value.ToString();
                        lstProfileNames.Add(item);
                    }
                }

                return lstProfileNames;
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CommonFunctions.PopulateMonitorsForScheduleCreation()");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public List<ProficiencyTestTemplateModel> PopulateMonitorsListForProficiencyTestScore(string qmType, int stateCode)
        {
            List<ProficiencyTestTemplateModel> lstProfileNames = new List<ProficiencyTestTemplateModel>();

            dbContext = new PMGSYEntities();
            string monitorAge = "";
            int MonitorAge = 0;
            int id = 0;

            try
            {
                //SelectListItem item = new SelectListItem();
                ProficiencyTestTemplateModel item = new ProficiencyTestTemplateModel();

                if (String.Equals(qmType, "I"))
                {
                    monitorAge = dbContext.ADMIN_MODULE_CONFIGURATION.Where(m => m.Parameter == "MONITOR_AGE").Select(m => m.Value).FirstOrDefault();
                    MonitorAge = Convert.ToInt32(monitorAge);
                }
                else if (String.Equals(qmType, "S"))
                {
                    monitorAge = dbContext.ADMIN_MODULE_CONFIGURATION.Where(m => m.Parameter == "MONITOR_AGE_SQM").Select(m => m.Value).FirstOrDefault();
                    MonitorAge = Convert.ToInt32(monitorAge);
                }

                if (stateCode == 0)
                {
                    var query = (from aqm in dbContext.ADMIN_QUALITY_MONITORS
                                 where aqm.ADMIN_QM_TYPE == qmType
                                 && aqm.ADMIN_QM_EMPANELLED == "Y"
                                 && (SqlFunctions.DateDiff("day", aqm.ADMIN_QM_BIRTH_DATE, DateTime.Now) <= MonitorAge || aqm.ADMIN_QM_BIRTH_DATE == null)
                                 select new
                                 {
                                     //Value = aqm.ADMIN_QM_CODE,
                                     //Text = (aqm.ADMIN_QM_LNAME.Equals(null) ? "" : aqm.ADMIN_QM_LNAME) + " " + (aqm.ADMIN_QM_FNAME.Equals(null) ? "" : aqm.ADMIN_QM_FNAME) + " " + (aqm.ADMIN_QM_MNAME.Equals(null) ? "" : aqm.ADMIN_QM_MNAME)
                                     aqm.ADMIN_QM_CODE,
                                     Name = (aqm.ADMIN_QM_FNAME.Equals(null) ? "" : aqm.ADMIN_QM_FNAME) + " " + (aqm.ADMIN_QM_MNAME.Equals(null) ? "" : aqm.ADMIN_QM_MNAME) + " " + (aqm.ADMIN_QM_LNAME.Equals(null) ? "" : aqm.ADMIN_QM_LNAME),
                                     aqm.ADMIN_QM_EMAIL,
                                     aqm.ADMIN_QM_MOBILE1
                                 }).OrderBy(aqm => aqm.Name).Distinct().ToList();

                    foreach (var data in query)
                    {
                        lstProfileNames.Add(new ProficiencyTestTemplateModel
                        {
                            ID = ++id,
                            MONITOR_NAME = data.Name,
                            ADMIN_QM_CODE = data.ADMIN_QM_CODE,
                            EMAIL = data.ADMIN_QM_EMAIL,
                            MOBILE_NUMBER = data.ADMIN_QM_MOBILE1,
                            MARKS = null,
                            MONITOR_STATUS = null
                        });
                    }

                }
                else
                {
                    var query1 = (from aqm in dbContext.ADMIN_QUALITY_MONITORS
                                  where aqm.ADMIN_QM_TYPE == qmType
                                  && aqm.ADMIN_QM_EMPANELLED == "Y"
                                  && (aqm.MAST_STATE_CODE == stateCode || aqm.MAST_STATE_CODE_ADDR == stateCode)

                                  && (SqlFunctions.DateDiff("year", aqm.ADMIN_QM_BIRTH_DATE, DateTime.Now) <= MonitorAge || aqm.ADMIN_QM_BIRTH_DATE == null)
                                  select new
                                  {
                                      //    Value = aqm.ADMIN_QM_CODE,
                                      //    Text = (aqm.ADMIN_QM_LNAME.Equals(null) ? "" : aqm.ADMIN_QM_LNAME) + " " + (aqm.ADMIN_QM_FNAME.Equals(null) ? "" : aqm.ADMIN_QM_FNAME) + " " + (aqm.ADMIN_QM_MNAME.Equals(null) ? "" : aqm.ADMIN_QM_MNAME)
                                      //}).OrderBy(c => c.Text).Distinct().OrderBy(a => a.Text).ToList();
                                      aqm.ADMIN_QM_CODE,
                                      Name = (aqm.ADMIN_QM_FNAME.Equals(null) ? "" : aqm.ADMIN_QM_FNAME) + " " + (aqm.ADMIN_QM_MNAME.Equals(null) ? "" : aqm.ADMIN_QM_MNAME) + " " + (aqm.ADMIN_QM_LNAME.Equals(null) ? "" : aqm.ADMIN_QM_LNAME),
                                      aqm.ADMIN_QM_EMAIL,
                                      aqm.ADMIN_QM_MOBILE1
                                  }).OrderBy(aqm => aqm.Name).Distinct().ToList();

                    var query2 = (from aqm in dbContext.ADMIN_QUALITY_MONITORS
                                  join interstate in dbContext.ADMIN_QUALITY_MONITORS_INTER_STATE on aqm.ADMIN_QM_CODE equals interstate.ADMIN_QM_CODE
                                  where interstate.ALLOWED_STATE_CODE == stateCode && interstate.APPROVED.Equals("Y") && aqm.ADMIN_QM_TYPE == qmType && aqm.ADMIN_QM_EMPANELLED == "Y"
                                  select new
                                  {
                                      //    Value = aqm.ADMIN_QM_CODE,
                                      //    Text = (aqm.ADMIN_QM_LNAME.Equals(null) ? "" : aqm.ADMIN_QM_LNAME) + " " + (aqm.ADMIN_QM_FNAME.Equals(null) ? "" : aqm.ADMIN_QM_FNAME) + " " + (aqm.ADMIN_QM_MNAME.Equals(null) ? "" : aqm.ADMIN_QM_MNAME)
                                      //}).OrderBy(c => c.Text).Distinct().OrderBy(a => a.Text).ToList();
                                      aqm.ADMIN_QM_CODE,
                                      Name = (aqm.ADMIN_QM_FNAME.Equals(null) ? "" : aqm.ADMIN_QM_FNAME) + " " + (aqm.ADMIN_QM_MNAME.Equals(null) ? "" : aqm.ADMIN_QM_MNAME) + " " + (aqm.ADMIN_QM_LNAME.Equals(null) ? "" : aqm.ADMIN_QM_LNAME),
                                      aqm.ADMIN_QM_EMAIL,
                                      aqm.ADMIN_QM_MOBILE1
                                  }).OrderBy(aqm => aqm.Name).Distinct().ToList();

                    var query = query1.Union(query2);

                    foreach (var data in query)
                    {
                        lstProfileNames.Add(new ProficiencyTestTemplateModel
                        {
                            ID = ++id,
                            MONITOR_NAME = data.Name,
                            ADMIN_QM_CODE = data.ADMIN_QM_CODE,
                            EMAIL = data.ADMIN_QM_EMAIL,
                            MOBILE_NUMBER = data.ADMIN_QM_MOBILE1,
                            MARKS = null,
                            MONITOR_STATUS = null
                        });
                    }
                }

                return lstProfileNames;
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CommonFunctions.PopulateMonitorsListForProficiencyTestScore()");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        //added by hrishikesh for generate monitors list 
        public List<ProficiencyTestTemplateModel> PopulateMonitorsListForProficiencyTestScoreNew(string qmType, int stateCode)
        {
            List<ProficiencyTestTemplateModel> lstProfileNames = new List<ProficiencyTestTemplateModel>();

            dbContext = new PMGSYEntities();
            string monitorAge = "";
            int MonitorAge = 0;
            int id = 0;

            try
            {
                //SelectListItem item = new SelectListItem();
                ProficiencyTestTemplateModel item = new ProficiencyTestTemplateModel();

                if (String.Equals(qmType, "I"))
                {
                    monitorAge = dbContext.ADMIN_MODULE_CONFIGURATION.Where(m => m.Parameter == "MONITOR_AGE").Select(m => m.Value).FirstOrDefault();
                    MonitorAge = Convert.ToInt32(monitorAge);
                }
                else if (String.Equals(qmType, "S"))
                {
                    monitorAge = dbContext.ADMIN_MODULE_CONFIGURATION.Where(m => m.Parameter == "MONITOR_AGE_SQM").Select(m => m.Value).FirstOrDefault();
                    MonitorAge = Convert.ToInt32(monitorAge);
                }

                if (stateCode == 0)
                {
                    var query = (from aqm in dbContext.ADMIN_QUALITY_MONITORS
                                 where aqm.ADMIN_QM_TYPE == qmType
                                 && aqm.ADMIN_QM_EMPANELLED == "Y"
                                 && (SqlFunctions.DateDiff("day", aqm.ADMIN_QM_BIRTH_DATE, DateTime.Now) <= MonitorAge || aqm.ADMIN_QM_BIRTH_DATE == null)
                                 select new
                                 {
                                     //Value = aqm.ADMIN_QM_CODE,
                                     //Text = (aqm.ADMIN_QM_LNAME.Equals(null) ? "" : aqm.ADMIN_QM_LNAME) + " " + (aqm.ADMIN_QM_FNAME.Equals(null) ? "" : aqm.ADMIN_QM_FNAME) + " " + (aqm.ADMIN_QM_MNAME.Equals(null) ? "" : aqm.ADMIN_QM_MNAME)
                                     aqm.ADMIN_QM_CODE,
                                     Name = (aqm.ADMIN_QM_FNAME.Equals(null) ? "" : aqm.ADMIN_QM_FNAME) + " " + (aqm.ADMIN_QM_MNAME.Equals(null) ? "" : aqm.ADMIN_QM_MNAME) + " " + (aqm.ADMIN_QM_LNAME.Equals(null) ? "" : aqm.ADMIN_QM_LNAME),
                                     aqm.ADMIN_QM_EMAIL,
                                     aqm.ADMIN_QM_MOBILE1
                                 }).OrderBy(aqm => aqm.Name).Distinct().ToList();

                    foreach (var data in query)
                    {
                        lstProfileNames.Add(new ProficiencyTestTemplateModel
                        {
                            ID = ++id,
                            MONITOR_NAME = data.Name,
                            ADMIN_QM_CODE = data.ADMIN_QM_CODE,
                            EMAIL = data.ADMIN_QM_EMAIL,
                            MOBILE_NUMBER = data.ADMIN_QM_MOBILE1,
                            MARKS = null,
                            MONITOR_STATUS = null
                        });
                    }

                }
                else
                {
                    var query1 = (from aqm in dbContext.ADMIN_QUALITY_MONITORS
                                  where aqm.ADMIN_QM_TYPE == qmType
                                  && aqm.ADMIN_QM_EMPANELLED == "Y"
                                  && (aqm.MAST_STATE_CODE == stateCode || aqm.MAST_STATE_CODE_ADDR == stateCode)

                                  && (SqlFunctions.DateDiff("year", aqm.ADMIN_QM_BIRTH_DATE, DateTime.Now) <= MonitorAge || aqm.ADMIN_QM_BIRTH_DATE == null)
                                  select new
                                  {
                                      //    Value = aqm.ADMIN_QM_CODE,
                                      //    Text = (aqm.ADMIN_QM_LNAME.Equals(null) ? "" : aqm.ADMIN_QM_LNAME) + " " + (aqm.ADMIN_QM_FNAME.Equals(null) ? "" : aqm.ADMIN_QM_FNAME) + " " + (aqm.ADMIN_QM_MNAME.Equals(null) ? "" : aqm.ADMIN_QM_MNAME)
                                      //}).OrderBy(c => c.Text).Distinct().OrderBy(a => a.Text).ToList();
                                      aqm.ADMIN_QM_CODE,
                                      Name = (aqm.ADMIN_QM_FNAME.Equals(null) ? "" : aqm.ADMIN_QM_FNAME) + " " + (aqm.ADMIN_QM_MNAME.Equals(null) ? "" : aqm.ADMIN_QM_MNAME) + " " + (aqm.ADMIN_QM_LNAME.Equals(null) ? "" : aqm.ADMIN_QM_LNAME),
                                      aqm.ADMIN_QM_EMAIL,
                                      aqm.ADMIN_QM_MOBILE1
                                  }).OrderBy(aqm => aqm.Name).Distinct().ToList();

                    /*var query2 = (from aqm in dbContext.ADMIN_QUALITY_MONITORS
                                  join interstate in dbContext.ADMIN_QUALITY_MONITORS_INTER_STATE on aqm.ADMIN_QM_CODE equals interstate.ADMIN_QM_CODE
                                  where interstate.ALLOWED_STATE_CODE == stateCode && interstate.APPROVED.Equals("Y") && aqm.ADMIN_QM_TYPE == qmType && aqm.ADMIN_QM_EMPANELLED == "Y"
                                  select new
                                  {
                                      //    Value = aqm.ADMIN_QM_CODE,
                                      //    Text = (aqm.ADMIN_QM_LNAME.Equals(null) ? "" : aqm.ADMIN_QM_LNAME) + " " + (aqm.ADMIN_QM_FNAME.Equals(null) ? "" : aqm.ADMIN_QM_FNAME) + " " + (aqm.ADMIN_QM_MNAME.Equals(null) ? "" : aqm.ADMIN_QM_MNAME)
                                      //}).OrderBy(c => c.Text).Distinct().OrderBy(a => a.Text).ToList();
                                      aqm.ADMIN_QM_CODE,
                                      Name = (aqm.ADMIN_QM_FNAME.Equals(null) ? "" : aqm.ADMIN_QM_FNAME) + " " + (aqm.ADMIN_QM_MNAME.Equals(null) ? "" : aqm.ADMIN_QM_MNAME) + " " + (aqm.ADMIN_QM_LNAME.Equals(null) ? "" : aqm.ADMIN_QM_LNAME),
                                      aqm.ADMIN_QM_EMAIL,
                                      aqm.ADMIN_QM_MOBILE1
                                  }).OrderBy(aqm => aqm.Name).Distinct().ToList();

                    var query = query1.Union(query2);*/

                    foreach (var data in query1)
                    {
                        lstProfileNames.Add(new ProficiencyTestTemplateModel
                        {
                            ID = ++id,
                            MONITOR_NAME = data.Name,
                            ADMIN_QM_CODE = data.ADMIN_QM_CODE,
                            EMAIL = data.ADMIN_QM_EMAIL,
                            MOBILE_NUMBER = data.ADMIN_QM_MOBILE1,
                            MARKS = null,
                            MONITOR_STATUS = null
                        });
                    }
                }

                return lstProfileNames;
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CommonFunctions.PopulateMonitorsListForProficiencyTestScoreNew()");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public List<SelectListItem> PopulateNQMForScheduleCreation(string isPopulateFirstSelect, string qmType, int stateCode, int month, int year)
        {
            List<SelectListItem> lstProfileNames = new List<SelectListItem>();
            dbContext = new PMGSYEntities();
            try
            {
                SelectListItem item = new SelectListItem();
                if (isPopulateFirstSelect.Equals("true"))
                {
                    item.Text = "Select Monitor";
                    item.Value = "0";
                    item.Selected = true;
                    lstProfileNames.Add(item);
                }
                else if (isPopulateFirstSelect.Equals("false"))
                {
                    item.Text = "All Monitors";
                    item.Value = "0";
                    item.Selected = true;
                    lstProfileNames.Add(item);
                }

                if (qmType.Equals("0"))
                {
                    return lstProfileNames;
                }

                if (stateCode == 0)
                {
                    var query = (from aqm in dbContext.ADMIN_QUALITY_MONITORS
                                 where aqm.ADMIN_QM_TYPE == qmType
                                 && aqm.ADMIN_QM_EMPANELLED == "Y"
                                     //&& !filter.Contains(aqm.ADMIN_QM_CODE)
                                     ///Added by SAMMED A. PATIL to restrict schedule assigning to monitors with age greater than 70
                                 && (SqlFunctions.DateDiff("day", aqm.ADMIN_QM_BIRTH_DATE, DateTime.Now) <= 25550 || aqm.ADMIN_QM_BIRTH_DATE == null)
                                 select new
                                 {
                                     Value = aqm.ADMIN_QM_CODE,
                                     Text = (aqm.ADMIN_QM_LNAME.Equals(null) ? "" : aqm.ADMIN_QM_LNAME) + " " + (aqm.ADMIN_QM_FNAME.Equals(null) ? "" : aqm.ADMIN_QM_FNAME) + " " + (aqm.ADMIN_QM_MNAME.Equals(null) ? "" : aqm.ADMIN_QM_MNAME),
                                     //+ " (" + aqm.ADMIN_QM_SERVICE_TYPE == "C" ? "Cental Govt." : aqm.ADMIN_QM_SERVICE_TYPE == "A" ? "Central Agency" : aqm.MAST_STATE_CODE + ")",
                                     aqm.ADMIN_QM_SERVICE_TYPE,
                                     aqm.MASTER_STATE.MAST_STATE_NAME
                                 }).OrderBy(aqm => aqm.Text).Distinct().OrderBy(a => a.Text).ToList();

                    foreach (var data in query)
                    {
                        item = new SelectListItem();
                        item.Text = data.Text.Trim() + " (" + (data.ADMIN_QM_SERVICE_TYPE == "C" ? "Cental Govt." : data.ADMIN_QM_SERVICE_TYPE == "A" ? "Cental Agency" : data.MAST_STATE_NAME) + ")";
                        item.Value = data.Value.ToString();
                        lstProfileNames.Add(item);
                    }
                }
                else
                {
                    #region Old code
                    //var filter = (from aqm in dbContext.ADMIN_QUALITY_MONITORS
                    //             join qqs in dbContext.QUALITY_QM_SCHEDULE on aqm.ADMIN_QM_CODE equals qqs.ADMIN_QM_CODE
                    //             join qqom in dbContext.QUALITY_QM_OBSERVATION_MASTER on qqs.ADMIN_SCHEDULE_CODE equals qqom.ADMIN_SCHEDULE_CODE
                    //             where aqm.ADMIN_QM_TYPE == qmType
                    //             && qqs.ADMIN_IM_MONTH == month
                    //             && qqs.ADMIN_IM_YEAR == year
                    //             && aqm.ADMIN_QM_EMPANELLED == "Y"
                    //             && (stateCode > 0 ? qqs.MAST_STATE_CODE : 1) == (stateCode > 0 ? stateCode : 1)
                    //             select aqm.ADMIN_QM_CODE).Distinct().ToList();
                    #endregion

                    var query = (from aqm in dbContext.ADMIN_QUALITY_MONITORS
                                 where aqm.ADMIN_QM_TYPE == qmType
                                 && aqm.ADMIN_QM_EMPANELLED == "Y"
                                 && (aqm.MAST_STATE_CODE == stateCode || aqm.MAST_STATE_CODE_ADDR == stateCode)
                                     //&& !filter.Contains(aqm.ADMIN_QM_CODE)
                                     ///Added by SAMMED A. PATIL to restrict schedule assigning to monitors with age greater than 70
                                 && (SqlFunctions.DateDiff("day", aqm.ADMIN_QM_BIRTH_DATE, DateTime.Now) <= 25550 || aqm.ADMIN_QM_BIRTH_DATE == null)
                                 select new
                                 {
                                     Value = aqm.ADMIN_QM_CODE,
                                     Text = (aqm.ADMIN_QM_LNAME.Equals(null) ? "" : aqm.ADMIN_QM_LNAME) + " " + (aqm.ADMIN_QM_FNAME.Equals(null) ? "" : aqm.ADMIN_QM_FNAME) + " " + (aqm.ADMIN_QM_MNAME.Equals(null) ? "" : aqm.ADMIN_QM_MNAME),
                                     //+ " (" + aqm.ADMIN_QM_SERVICE_TYPE == "C" ? "Cental Govt." : aqm.ADMIN_QM_SERVICE_TYPE == "C" ? "Central Agency" : aqm.MAST_STATE_CODE + ")",
                                     aqm.ADMIN_QM_SERVICE_TYPE,
                                     aqm.MASTER_STATE.MAST_STATE_NAME
                                 }).OrderBy(c => c.Text).Distinct().OrderBy(a => a.Text).ToList();

                    foreach (var data in query)
                    {
                        item = new SelectListItem();
                        item.Text = data.Text + " (" + (data.ADMIN_QM_SERVICE_TYPE == "C" ? "Cental Govt." : data.ADMIN_QM_SERVICE_TYPE == "A" ? "Cental Agency" : data.MAST_STATE_NAME) + ")";
                        item.Value = data.Value.ToString();
                        lstProfileNames.Add(item);
                    }
                }

                return lstProfileNames;
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CommonFunctions.PopulateMonitorsForScheduleCreation()");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public List<SelectListItem> QualityATRStatus()
        {
            List<SelectListItem> lstATRStatus = new List<SelectListItem>();
            try
            {
                lstATRStatus.Insert(0, (new SelectListItem { Text = "All", Value = "0", Selected = true }));
                lstATRStatus.Insert(1, (new SelectListItem { Text = "Not Submitted", Value = "N" }));
                lstATRStatus.Insert(2, (new SelectListItem { Text = "Submitted", Value = "U" }));
                lstATRStatus.Insert(3, (new SelectListItem { Text = "Accepted", Value = "A" }));
                lstATRStatus.Insert(4, (new SelectListItem { Text = "Rejected", Value = "R" }));
                lstATRStatus.Insert(5, (new SelectListItem { Text = "ATR Verification", Value = "V" }));
                lstATRStatus.Insert(6, (new SelectListItem { Text = "Technical Commitee", Value = "C" }));
                lstATRStatus.Insert(7, (new SelectListItem { Text = "Non Rectifiable", Value = "D" }));

                return lstATRStatus;
            }
            catch
            {
                return lstATRStatus;
            }
        }



        public List<SelectListItem> Quality2TierATRStatus()
        {
            List<SelectListItem> lstATRStatus = new List<SelectListItem>();
            try
            {
                lstATRStatus.Insert(0, (new SelectListItem { Text = "All", Value = "0", Selected = true }));
                lstATRStatus.Insert(1, (new SelectListItem { Text = "Not Submitted", Value = "N" }));
                lstATRStatus.Insert(2, (new SelectListItem { Text = "Submitted", Value = "U" }));
                lstATRStatus.Insert(3, (new SelectListItem { Text = "Accepted", Value = "A" }));
                lstATRStatus.Insert(4, (new SelectListItem { Text = "Rejected", Value = "R" }));
                //lstATRStatus.Insert(5, (new SelectListItem { Text = "ATR Verification", Value = "V" }));
                //lstATRStatus.Insert(6, (new SelectListItem { Text = "Technical Commitee", Value = "C" }));
                //lstATRStatus.Insert(7, (new SelectListItem { Text = "Non Rectifiable", Value = "D" }));

                return lstATRStatus;
            }
            catch
            {
                return lstATRStatus;
            }
        }
        public List<SelectListItem> QualityMaintenaceATRStatus()//Added by deendayal on 05-02-2018
        {
            List<SelectListItem> lstATRStatus = new List<SelectListItem>();
            try
            {
                lstATRStatus.Insert(0, (new SelectListItem { Text = "All", Value = "0", Selected = true }));
                lstATRStatus.Insert(1, (new SelectListItem { Text = "Not Submitted", Value = "N" }));
                lstATRStatus.Insert(2, (new SelectListItem { Text = "Submitted", Value = "U" }));
                lstATRStatus.Insert(3, (new SelectListItem { Text = "Accepted", Value = "A" }));
                lstATRStatus.Insert(4, (new SelectListItem { Text = "Rejected", Value = "R" }));
                lstATRStatus.Insert(5, (new SelectListItem { Text = "ATR Verification", Value = "V" }));
                return lstATRStatus;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CommonFunctions.QualityMaintenaceATRStatus()");
                return lstATRStatus;
            }
        }


        public List<SelectListItem> QualityATRSubmitDuration()
        {
            List<SelectListItem> lstATRSubmitDuration = new List<SelectListItem>();
            try
            {
                lstATRSubmitDuration.Insert(0, (new SelectListItem { Text = "All", Value = "0" }));
                lstATRSubmitDuration.Insert(1, (new SelectListItem { Text = "Since 3 Months", Value = "1", Selected = true }));
                lstATRSubmitDuration.Insert(2, (new SelectListItem { Text = "3-6 Months", Value = "2" }));
                lstATRSubmitDuration.Insert(3, (new SelectListItem { Text = "6-9 Months", Value = "3" }));
                lstATRSubmitDuration.Insert(4, (new SelectListItem { Text = "Before 9 Months", Value = "4" }));

                return lstATRSubmitDuration;
            }
            catch
            {
                return lstATRSubmitDuration;
            }
        }

        public List<SelectListItem> PopulateRoadStatus()
        {
            List<SelectListItem> statusType = new List<SelectListItem>();
            SelectListItem item = new SelectListItem();
            item.Text = "All";
            item.Value = "A";
            item.Selected = true;

            statusType.Add(item);

            item = new SelectListItem();
            item.Text = "Completed";
            item.Value = "C";
            statusType.Add(item);

            item = new SelectListItem();
            item.Text = "In Progress";
            item.Value = "P";
            statusType.Add(item);

            return statusType;
        }

        public String getMonthText(Int16 monthId)
        {
            try
            {
                dbContext = new PMGSYEntities();
                return dbContext.MASTER_MONTH.Where(m => m.MAST_MONTH_CODE == monthId).Select(m => m.MAST_MONTH_FULL_NAME).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public ACC_TEO_SCREEN_DESIGN_HEADWISE_DETAILS GetHeadwiseDesignParams(TransactionParams objParam)
        {
            try
            {
                dbContext = new PMGSYEntities();
                objParam.TXN_ID = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == objParam.BILL_ID).Select(m => m.TXN_ID).FirstOrDefault();
                return dbContext.ACC_TEO_SCREEN_DESIGN_HEADWISE_DETAILS.Where(m => m.HEAD_ID == objParam.HEAD_ID && m.TXN_ID == objParam.TXN_ID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// function to get the financial year based on month and year
        /// </summary>
        /// <param name="monthId"></param>
        /// <param name="yearId"></param>
        /// <returns></returns> 
        public String getFinancialYear(Int16 monthId, Int16 yearId)
        {
            String financialYear = string.Empty;
            Int16 nextPrevYear = 0;

            if (monthId >= 4)
            {
                nextPrevYear = Convert.ToInt16(yearId + 1);

                financialYear = yearId.ToString() + "-" + nextPrevYear.ToString().Substring(2, 2);
            }
            else
            {



                nextPrevYear = Convert.ToInt16(yearId - 1);

                financialYear = nextPrevYear.ToString() + "-" + yearId.ToString().Substring(2, 2);

            }

            return financialYear;

        }

        //<summary>
        //Class Required for Filtering in JqGrid
        //</summary>
        public class SearchJson
        {
            public string groupOp { get; set; }
            public List<rules> rules { get; set; }
        }

        ///// <summary>
        /////  Class Required for Filtering in JqGrid
        ///// </summary>
        public class rules
        {
            public string field { get; set; }
            public string op { get; set; }
            public string data { get; set; }
        }

        #region OB
        public List<SelectListItem> PopulateOBTransaction(TransactionParams objParam)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<ACC_MASTER_TXN> lstMastTxn = new List<ACC_MASTER_TXN>();
                List<SelectListItem> lstTransaction = new List<SelectListItem>();
                Int16 ParentId = 0;
                String cashCheque = String.Empty;

                if (objParam.TXN_ID == 0)
                {
                    ParentId = dbContext.ACC_MASTER_TXN.Where(m => m.BILL_TYPE == objParam.BILL_TYPE && m.FUND_TYPE == objParam.FUND_TYPE && m.OP_LVL_ID == objParam.LVL_ID && m.TXN_PARENT_ID == null).Select(m => m.TXN_ID).FirstOrDefault();
                }
                else
                {
                    ParentId = objParam.TXN_ID;
                }

                if (objParam.BILL_NO == "1")
                {
                    cashCheque = "D";
                }
                else
                {
                    cashCheque = "C";
                }

                //lstTransaction = new SelectList(dbContext.ACC_MASTER_TXN.Where(m => m.BILL_TYPE == objParam.BILL_TYPE && m.FUND_TYPE == objParam.FUND_TYPE && m.OP_LVL_ID == objParam.LVL_ID && m.TXN_PARENT_ID == ParentId && m.CASH_CHQ == cashCheque && (objParam.OP_MODE == "A" ? m.IS_OPERATIONAL : true) == (objParam.OP_MODE == "A" ? true : true)), "TXN_ID", "TXN_DESC").ToList();

                //old
                //lstMastTxn = dbContext.ACC_MASTER_TXN.Where(m => m.BILL_TYPE == objParam.BILL_TYPE && m.FUND_TYPE == objParam.FUND_TYPE && m.OP_LVL_ID == objParam.LVL_ID && m.TXN_PARENT_ID == ParentId && m.CASH_CHQ == cashCheque && (objParam.OP_MODE == "A" ? m.IS_OPERATIONAL : true) == (objParam.OP_MODE == "A" ? true : true)).OrderBy(m=>m.TXN_ORDER).ToList<ACC_MASTER_TXN>();

                //modified by abhishek kamble 22-10-2013
                lstMastTxn = dbContext.ACC_MASTER_TXN.Where(m => m.BILL_TYPE == objParam.BILL_TYPE && m.FUND_TYPE == objParam.FUND_TYPE && m.OP_LVL_ID == objParam.LVL_ID && m.TXN_PARENT_ID == ParentId && m.CASH_CHQ == cashCheque && (objParam.OP_MODE == "A" ? m.IS_OPERATIONAL : m.IS_OPERATIONAL) == (objParam.OP_MODE == "A" ? true : true)).OrderBy(m => m.TXN_ORDER).ToList<ACC_MASTER_TXN>();

                foreach (ACC_MASTER_TXN item in lstMastTxn)
                {
                    if (item.IS_REQ_AFTER_PORTING == true)
                    {
                        lstTransaction.Add(new SelectListItem { Value = item.TXN_ID.ToString().Trim(), Text = item.TXN_DESC });
                    }
                    else
                    {
                        lstTransaction.Add(new SelectListItem { Value = item.TXN_ID.ToString().Trim(), Text = "$" + item.TXN_DESC });
                    }
                }

                //lstTransaction = lstTransaction.OrderBy(m => m.Text).ToList();

                if (objParam.TXN_ID == 0)
                {
                    lstTransaction.Insert(0, (new SelectListItem { Text = "Select Transaction", Value = "0", Selected = true }));
                }
                else
                {
                    lstTransaction.Insert(0, (new SelectListItem { Text = "Select SubTransaction", Value = "0", Selected = true }));
                }

                return lstTransaction;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Boolean IsTransactionEditable(Int16 transId)
        {
            try
            {
                dbContext = new PMGSYEntities();
                return dbContext.ACC_MASTER_TXN.Where(m => m.TXN_ID == transId).Select(m => m.IS_REQ_AFTER_PORTING).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion

        // To get Transaction Narration and HeadCode : Head Desc
        public String GetTransNarration(Int16 transId, String billId, String billType)
        {
            try
            {
                String Narration = String.Empty;
                String HeadDesc = String.Empty;
                Int64 billID = 0;
                String CreditDebit = String.Empty;
                dbContext = new PMGSYEntities();
                if (billType == "O")
                {
                    CreditDebit = dbContext.ACC_MASTER_TXN.Where(m => m.TXN_ID == transId).Select(m => m.CASH_CHQ).FirstOrDefault();
                    CreditDebit = string.IsNullOrEmpty(CreditDebit) ? CreditDebit : CreditDebit.Trim();

                    if (CreditDebit == "C")
                    {
                        if (billId.Contains("_")) //added check by koustubh nakate for '_' not contains in billID
                        {
                            billID = Convert.ToInt64(billId.Split('_')[1]);
                        }
                        else
                        {
                            billID = Convert.ToInt64(billId);
                        }
                        CreditDebit = "C";
                    }
                    else
                    {
                        if (billId.Contains("_"))//added check by koustubh nakate for '_' not contains in billID
                        {
                            billID = Convert.ToInt64(billId.Split('_')[0]);
                        }
                        else
                        {
                            billID = Convert.ToInt64(billId);
                        }
                        CreditDebit = "D";
                    }
                }
                else if (billType == "R")
                {
                    billID = Convert.ToInt64(billId);
                    CreditDebit = "C";
                }
                else if (billType == "P")
                {
                    billID = Convert.ToInt64(billId);
                    CreditDebit = "D";
                }
                else if (billType == "T")
                {
                    billID = 0;
                    CreditDebit = "D";
                }

                Narration = dbContext.ACC_MASTER_TXN.Where(m => m.TXN_ID == transId).Select(m => m.TXN_NARRATION).FirstOrDefault();
                if (String.IsNullOrEmpty(Narration))
                {
                    Narration = "";
                }

                //for deduction transaction description
                String CashCheque = string.Empty;

                if (dbContext.ACC_MASTER_TXN.Where(m => m.TXN_ID == transId).Select(m => (string)m.BILL_TYPE).FirstOrDefault() == "D")
                {
                    CashCheque = "C";
                    CreditDebit = "C";
                }
                else
                {
                    CashCheque = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == billID).Select(m => m.CHQ_EPAY).FirstOrDefault();
                }
                //CashCheque = CashCheque == null ? "C" : CashCheque;
                if (billType == "T")
                {
                    CashCheque = "C";
                }

                if (CashCheque == "E")
                {
                    CashCheque = "Q";
                }

                var varHeadDesc = (from h in dbContext.ACC_MASTER_HEAD
                                   join map in dbContext.ACC_TXN_HEAD_MAPPING on h.HEAD_ID equals map.HEAD_ID
                                   where map.TXN_ID == transId && map.CASH_CHQ.Contains(CashCheque) && map.CREDIT_DEBIT == CreditDebit
                                   select new
                                   {
                                       headName = h.HEAD_CODE + ": " + h.HEAD_NAME
                                   }).FirstOrDefault();
                if (varHeadDesc == null)
                {
                    HeadDesc = "";
                }
                else
                {
                    HeadDesc = varHeadDesc.headName.ToString();
                }


                return Narration.Trim() + "$" + HeadDesc;
            }
            catch (Exception ex)
            {
                return string.Empty + "$" + string.Empty;
                //throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }




        public DateTime GetOBDate(TransactionParams objParam)
        {
            try
            {
                dbContext = new PMGSYEntities();
                return dbContext.ACC_BILL_MASTER.Where(m => m.ADMIN_ND_CODE == objParam.ADMIN_ND_CODE && m.FUND_TYPE == objParam.FUND_TYPE && m.LVL_ID == objParam.LVL_ID && m.BILL_TYPE == "O" && m.BILL_FINALIZED == "Y").Select(m => m.BILL_DATE).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        /// <summary>
        /// function to return the monthly closing status 
        /// </summary>
        /// <param name="objParam"></param>
        /// <returns>
        /// -111 if previous month  of the parameter month is not closed
        /// -222 if parameter month is not closed
        /// -333$Entrytype  if voucher is not finalized
        /// -444 if epayment is not finalized by authorized signatory
        /// 1 if none of the above
        /// -666 month is already closed
        /// -777 account start month & year should be greater than selected month and year
        /// -999 DPIU Unser SRRDA has not closed the month
        /// -5555 Account is not yet started and user is trying to close the month
        /// -123 if  DPIU is trying to close the month and SRRDA has already closed the month 
        /// </returns>
        public String MonthlyClosingStatus(int adminNdCode, short monthToClose, short yearToClose, string fundType, short levelID, bool CheckingForDPIU)
        {


            dbContext = new PMGSYEntities();
            try
            {
                short prevMonth = 0, prevYear = 0;
                Int32 minBillYear = 0;
                Int32 minBillMonth = 0;

                string voucherType = String.Empty;

                # region if dpiu is closing the month check if SRRDA has closed that month if yes dont allow closing

                if (levelID == 5)
                {
                    int? parentNdCode = dbContext.ADMIN_DEPARTMENT.Where(x => x.ADMIN_ND_CODE == adminNdCode).Select(x => x.MAST_PARENT_ND_CODE).First();

                    if (dbContext.ACC_BILL_MASTER.Where(c => c.ADMIN_ND_CODE == parentNdCode && c.FUND_TYPE == fundType).Any())
                    {

                        minBillYear = dbContext.ACC_BILL_MASTER.Where(c => c.ADMIN_ND_CODE == parentNdCode && c.LVL_ID == 4 && c.FUND_TYPE == fundType).Min(d => d.BILL_YEAR);
                        minBillMonth = dbContext.ACC_BILL_MASTER.Where(c => c.ADMIN_ND_CODE == parentNdCode && c.LVL_ID == 4 && c.BILL_YEAR == minBillYear && c.FUND_TYPE == fundType).Min(d => d.BILL_MONTH);

                        //if date of first entry by SRRDA is  less than month to be cloased bu DPIU 
                        if (minBillYear > yearToClose)
                        {
                            //no need to check
                        }
                        else if (minBillYear == yearToClose)
                        {
                            if (minBillMonth > monthToClose)
                            {
                                //no need to check
                            }
                            else
                            {
                                //check if month is closed by SRRDA
                                if (dbContext.ACC_RPT_MONTHWISE_SUMMARY.Where
                                     (m => m.ADMIN_ND_CODE == parentNdCode
                                     && m.FUND_TYPE == fundType
                                     && m.LVL_ID == 4
                                     && m.ACC_MONTH == monthToClose
                                     && m.ACC_YEAR == yearToClose
                                    ).Any())
                                {
                                    return "-123";// SRRDA has closed the month year which is being closed by dpiu
                                }


                            }
                        }
                        else
                        {

                            //check if month is closed by SRRDA
                            if (dbContext.ACC_RPT_MONTHWISE_SUMMARY.Where
                                 (m => m.ADMIN_ND_CODE == parentNdCode
                                 && m.FUND_TYPE == fundType
                                 && m.LVL_ID == 4
                                 && m.ACC_MONTH == monthToClose
                                 && m.ACC_YEAR == yearToClose
                                ).Any())
                            {
                                return "-123";// SRRDA has  closed the month year which is being closed by dpiu
                            }
                        }


                    }
                    else
                    {
                        //no entry by SRRDA //ok
                    }

                }

                #endregion

                //check if previous month is closed or not (check upto account start month && year)
                //if modiifed By Abhishek kamble 28-Aug-2014 for Monthly Closing of DPIU At SRRDA level start
                //old
                if (PMGSYSession.Current.LevelId == 5)
                //if (levelID == 5)
                {
                    if (dbContext.ACC_BILL_MASTER.Where(c => c.ADMIN_ND_CODE == adminNdCode &&
                        c.LVL_ID == levelID && c.FUND_TYPE == fundType).Any())
                    {

                        minBillYear = dbContext.ACC_BILL_MASTER.Where(c => c.ADMIN_ND_CODE == adminNdCode && c.LVL_ID == levelID && c.FUND_TYPE == fundType).Min(d => d.BILL_YEAR);
                        minBillMonth = dbContext.ACC_BILL_MASTER.Where(c => c.ADMIN_ND_CODE == adminNdCode && c.LVL_ID == levelID && c.BILL_YEAR == minBillYear && c.FUND_TYPE == fundType).Min(d => d.BILL_MONTH);
                    }
                    else
                    {
                        if (CheckingForDPIU)
                        {
                            return "-666";  //month is already closed
                        }

                        return "-5555";  // Account is not yet started and user is trying to close the month
                    }
                }
                else if (PMGSYSession.Current.LevelId == 4)
                {
                    //Old Code
                    //if (dbContext.ACC_BILL_MASTER.Where(c => c.ADMIN_ND_CODE == adminNdCode &&
                    //    c.LVL_ID == levelID && c.FUND_TYPE == fundType && c.BILL_YEAR == yearToClose && c.BILL_MONTH == monthToClose).Any())
                    //{

                    //    minBillYear = yearToClose;
                    //    minBillMonth = monthToClose;
                    //}    
                    if (dbContext.ACC_BILL_MASTER.Where(c => c.ADMIN_ND_CODE == adminNdCode &&
                c.LVL_ID == levelID && c.FUND_TYPE == fundType && ((c.BILL_MONTH + c.BILL_YEAR * 12) <= (monthToClose + yearToClose * 12))).Any())
                    {

                        //minBillYear = yearToClose;
                        //minBillMonth = monthToClose;
                        minBillYear = dbContext.ACC_BILL_MASTER.Where(c => c.ADMIN_ND_CODE == adminNdCode && c.LVL_ID == levelID && c.FUND_TYPE == fundType).Min(d => d.BILL_YEAR);
                        minBillMonth = dbContext.ACC_BILL_MASTER.Where(c => c.ADMIN_ND_CODE == adminNdCode && c.LVL_ID == levelID && c.BILL_YEAR == minBillYear && c.FUND_TYPE == fundType).Min(d => d.BILL_MONTH);
                    }
                    //New Code 3-July-2014  By Abhishek - PIU with no Payment/Re/TEO details also show in SRRDA list in month is not closed.
                    //if (dbContext.ACC_BILL_MASTER.Where(c => c.ADMIN_ND_CODE == adminNdCode && c.LVL_ID == levelID && c.FUND_TYPE == fundType).Any())
                    //{
                    //    minBillYear = dbContext.ACC_BILL_MASTER.Where(c => c.ADMIN_ND_CODE == adminNdCode && c.LVL_ID ==levelID && c.FUND_TYPE == fundType).Min(d => d.BILL_YEAR);
                    //    minBillMonth = dbContext.ACC_BILL_MASTER.Where(c => c.ADMIN_ND_CODE == adminNdCode && c.LVL_ID== levelID && c.BILL_YEAR == minBillYear && c.FUND_TYPE == fundType).Min(d => d.BILL_MONTH);
                    //}
                    else
                    {
                        if (CheckingForDPIU)
                        {
                            return "-666";  //month is already closed
                        }

                        return "-5555";  // Account is not yet started and user is trying to close the month
                    }

                }

                // account start month & year should be less than or equal than selected month and year selected for closing
                if (minBillYear > yearToClose)
                {
                    return "-777"; //account is started on the year greater than year to close
                }
                else if (minBillYear == yearToClose)
                {
                    if (minBillMonth > monthToClose)
                    {
                        return "-777";
                    }
                }

                //check if month is not closed 
                if (dbContext.ACC_RPT_MONTHWISE_SUMMARY.Where
                         (m => m.ADMIN_ND_CODE == adminNdCode
                         && m.FUND_TYPE == fundType
                         && m.LVL_ID == levelID
                         && m.ACC_MONTH == monthToClose
                         && m.ACC_YEAR == yearToClose
                        ).Any())
                {
                    return "-666"; //month is closed
                }

                //if Cheking for DPIU of the SRRDA
                if (CheckingForDPIU)
                {
                    return "-999"; //if month of DPIU under SRRDA is not closed do not check for other reasons 
                }

                if (monthToClose == 1)
                {
                    prevMonth = 12;
                    prevYear = Convert.ToInt16(yearToClose - 1);
                }
                else
                {
                    prevMonth = Convert.ToInt16(monthToClose - 1);
                    // prevYear =Convert.ToInt16(yearToClose-1);
                    prevYear = yearToClose; //changes by Koustubh Nakate on 08/10/2013 
                }

                //if account starting month and year is less than month year to be closed 
                // if (minBillYear <= prevYear) //&& minBillMonth < prevMonth //commenetd by koustubh Nakate on 08/10/2013

                if (!(minBillMonth + (minBillYear * 12) == monthToClose + (yearToClose * 12)))
                {
                    if (!dbContext.ACC_RPT_MONTHWISE_SUMMARY.Where
                              (m => m.ADMIN_ND_CODE == adminNdCode
                              && m.FUND_TYPE == fundType
                              && m.LVL_ID == levelID
                              && m.ACC_MONTH == prevMonth
                              && m.ACC_YEAR == prevYear
                               ).Any())
                    {
                        return "-111";
                    }
                }

                #region  check for unfinalized vouchers

                if (dbContext.ACC_BILL_MASTER.Where
                          (m => m.ADMIN_ND_CODE == adminNdCode
                          && m.FUND_TYPE == fundType
                          && m.LVL_ID == levelID
                          && m.BILL_MONTH == monthToClose
                          && m.BILL_YEAR == yearToClose
                          && m.BILL_FINALIZED == "N"
                         ).Any())
                {

                    voucherType = dbContext.ACC_BILL_MASTER.Where
                      (m => m.ADMIN_ND_CODE == adminNdCode
                      && m.FUND_TYPE == fundType
                      && m.LVL_ID == levelID
                      && m.BILL_MONTH == monthToClose
                      && m.BILL_YEAR == yearToClose
                      && m.BILL_FINALIZED == "N"
                     ).Select(x => x.BILL_TYPE).First();

                    return "-333$" + voucherType + "$" + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(monthToClose) + "$" + yearToClose;

                }

                #endregion

                #region  //check if all epayments has been finalized by authorized signatory
                //1) get all finalized epayments in given month and year  //removed after discussion with madam/sir on 21082013

                /*  List<long> epayBillsLists = new List<long>();
                
                  epayBillsLists = dbContext.ACC_BILL_MASTER.Where
                      (c => c.ADMIN_ND_CODE == adminNdCode 
                          && c.FUND_TYPE == fundType 
                          && c.BILL_MONTH == monthToClose
                          && c.BILL_YEAR == yearToClose 
                          && c.BILL_FINALIZED == "Y"
                          && c.CHQ_EPAY=="E"
                          ).Select(x=>x.BILL_ID).ToList();

                  if (epayBillsLists.Count != 0)
                  {
                      foreach (long item in epayBillsLists)
                      {
                          //check if any of it is not finalized by authorized signatory
                          if (!dbContext.ACC_EPAY_MAIL_MASTER.Where(c =>c.BILL_ID ==item).Any())
                          {
                              return "-444";
                          }  
                      }
                   
                  }
                
                  //check if authorization request entry is finalized

                  if (dbContext.ACC_AUTH_REQUEST_MASTER.Where
                           (m => m.ADMIN_ND_CODE == adminNdCode
                           && m.FUND_TYPE == fundType
                           && m.LVL_ID == levelID
                           && m.AUTH_MONTH == monthToClose
                           && m.AUTH_YEAR == yearToClose
                           && m.AUTH_FINALIZED=="N"
                          ).Any())
                  {
                      return "-333$A$" + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(monthToClose) + "$" + yearToClose;  //Authorization request not  Finalized
                  }*/
                #endregion

                //check if month is not closed 
                if (!dbContext.ACC_RPT_MONTHWISE_SUMMARY.Where
                         (m => m.ADMIN_ND_CODE == adminNdCode
                         && m.FUND_TYPE == fundType
                         && m.LVL_ID == levelID
                         && m.ACC_MONTH == monthToClose
                         && m.ACC_YEAR == yearToClose
                        ).Any())
                {
                    return "-222"; //month is not closed
                }

                return "-666"; //month is already closed
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }

        }

        //addition by koustubh nakate on 20/06/2013 to validate grid partameters
        /// <summary>
        /// Used to validate grid partameters
        /// </summary>
        /// <param name="homeFormCollection">FormCollection object</param>
        /// <returns>bool</returns>
        public bool ValidateGridParameters(GridParams gridParams)
        {

            try
            {
                bool outSearch;
                int outParam;
                bool result = true;
                long outNd;

                Regex regex = new Regex(@"^[a-zA-Z0-9 _,.-]*$");

                //string InvalidCharacters = "<>;!@#$%^&*()=+?'";
                //char[] Charray = InvalidCharacters.ToCharArray();

                if (!string.IsNullOrEmpty(gridParams.Search.ToString()))
                {
                    if (!Boolean.TryParse(gridParams.Search.ToString(), out outSearch))
                    {
                        result = false;
                    }

                }
                else
                {
                    result = false;
                }


                //if (!string.IsNullOrEmpty(Request.Params["nd"]))
                //{
                //    if (!long.TryParse(Request.Params["nd"].ToString(), out outNd))
                //    {
                //        result = false;
                //    }
                //}
                //else
                //{
                //    result = false;
                //}

                if (!long.TryParse(gridParams.Nd.ToString(), out outNd))
                {
                    result = false;
                }

                if (!int.TryParse(gridParams.Page.ToString(), out outParam))
                {
                    result = false;
                }

                if (!int.TryParse(gridParams.Rows.ToString(), out outParam))
                {
                    result = false;
                }

                if (!string.IsNullOrEmpty(gridParams.Sidx))
                {
                    if (!regex.IsMatch(gridParams.Sidx))
                    {
                        result = false;
                    }
                }
                else
                {
                    result = false;
                }

                if (!string.IsNullOrEmpty(gridParams.Sord))
                {

                    if (!regex.IsMatch(gridParams.Sord))
                    {
                        result = false;
                    }
                }
                else
                {
                    result = false;
                }



                //if (!string.IsNullOrEmpty(gridParams.Sidx))
                //{
                //    for (int i = 0; i < Charray.Length; i++)
                //    {
                //        int index = gridParams.Sidx.IndexOf(Charray[i]);
                //        if (index > -1)
                //        {
                //            result = false;
                //            break;
                //        }
                //    }
                //}
                //else
                //{
                //    result = false;
                //}

                //if (!string.IsNullOrEmpty(gridParams.Sord))
                //{

                //    for (int i = 0; i < Charray.Length; i++)
                //    {
                //        int index = gridParams.Sord.IndexOf(Charray[i]);
                //        if (index > -1)
                //        {
                //            result = false;
                //            break;
                //        }
                //    }

                //}
                //else
                //{
                //    result = false;
                //}

                // end addition by koustubh nakate on 25/06/2012 to validate grid partameters

                return result;
            }
            catch
            {
                return false;
            }

        }

        /// <summary>
        /// function to get the monthly closing status
        /// </summary>
        /// <param name="monthToClose"></param>
        /// <param name="yearToclose"></param>
        /// <param name="fundtype"></param>
        /// <param name="levelID"></param>
        /// <param name="adminNdCode"></param>
        /// <returns></returns>
        /// <summary>
        /// function to get the monthly closing status
        /// </summary>
        /// <param name="monthToClose"></param>
        /// <param name="yearToclose"></param>
        /// <param name="fundtype"></param>
        /// <param name="levelID"></param>
        /// <param name="adminNdCode"></param>
        /// <returns></returns>
        public string MonthlyClosingValidation(Int16 monthToClose, Int16 yearToclose, string fundtype, short levelID, int adminNdCode, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                short prevMonth = 0, minBillYear = 0;
                short prevYear = 0, minBillMonth = 0;

                if (monthToClose == 1)
                {
                    prevMonth = 12;
                    prevYear = Convert.ToInt16(yearToclose - 1);
                }
                else
                {
                    prevMonth = Convert.ToInt16(monthToClose - 1);
                    prevYear = yearToclose;

                }

                //check if previous month is closed or not (check upto account start month && year)
                if (dbContext.ACC_BILL_MASTER.Where(c => c.ADMIN_ND_CODE == adminNdCode && c.LVL_ID == levelID && c.FUND_TYPE == fundtype).Any())
                {
                    minBillYear = dbContext.ACC_BILL_MASTER.Where(c => c.ADMIN_ND_CODE == adminNdCode && c.LVL_ID == levelID && c.FUND_TYPE == fundtype).Min(d => d.BILL_YEAR);
                    minBillMonth = dbContext.ACC_BILL_MASTER.Where(c => c.ADMIN_ND_CODE == adminNdCode && c.LVL_ID == levelID && c.BILL_YEAR == minBillYear && c.FUND_TYPE == fundtype).Min(d => d.BILL_MONTH);
                }
                else
                {
                    return "1";//no previous data entry
                }

                //this Condition for if minimum and entry month for dpiu is same 
                //check if minimum data entry month and year is less then prev month and year
                //    if (minBillYear < prevYear && minBillMonth < prevMonth)

                if (!(minBillMonth + (minBillYear * 12) == monthToClose + (yearToclose * 12)))
                {
                    //check prev motnh 
                    if (!dbContext.ACC_RPT_MONTHWISE_SUMMARY.Where
                                        (m => m.ADMIN_ND_CODE == adminNdCode
                                        && m.FUND_TYPE == fundtype
                                        && m.LVL_ID == levelID
                                        && m.ACC_MONTH == prevMonth
                                        && m.ACC_YEAR == prevYear
                                    ).Any())
                    {
                        if (dbContext.ACC_RPT_MONTHWISE_SUMMARY.Any(m => m.ADMIN_ND_CODE == adminNdCode && m.FUND_TYPE == fundtype && m.LVL_ID == levelID))
                        {
                            string monthName = dbContext.MASTER_MONTH.Where(a => a.MAST_MONTH_CODE == (dbContext.ACC_RPT_MONTHWISE_SUMMARY.Where(m => m.ADMIN_ND_CODE == adminNdCode
                                           && m.FUND_TYPE == fundtype
                                           && m.LVL_ID == levelID).Select(m => m.ACC_MONTH).FirstOrDefault()) + 1).Select(a => a.MAST_MONTH_FULL_NAME).FirstOrDefault();
                            int yearName = dbContext.ACC_RPT_MONTHWISE_SUMMARY.Where(m => m.ADMIN_ND_CODE == adminNdCode
                                            && m.FUND_TYPE == fundtype
                                            && m.LVL_ID == levelID).Select(m => m.ACC_YEAR).FirstOrDefault();

                            //Commented by Abhishek kamble 14-Feb-2014
                            //message = monthName + " - " + yearName + " is not closed.";
                            message = "Previous month is not closed.";
                        }
                        else
                        {
                            message = "Account start month and year is not yet closed.";
                        }
                        return "-111"; //prev month is not closed
                    }

                }

                if (dbContext.ACC_RPT_MONTHWISE_SUMMARY.Where
                                   (m => m.ADMIN_ND_CODE == adminNdCode
                                   && m.FUND_TYPE == fundtype
                                   && m.LVL_ID == levelID
                                   && m.ACC_MONTH == monthToClose
                                   && m.ACC_YEAR == yearToclose
                           ).Any())
                {
                    return "-222"; // month is closed
                }
                else return "1";
            }
            catch (Exception ex)
            {
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
        /// check if bank details entered
        /// </summary>
        /// <param name="fundtype"></param>
        /// <param name="levelID"></param>
        /// <param name="adminNdCode"></param>
        /// <returns></returns>
        public bool IsBankDetailsEntered(string fundtype, short levelID, int adminNdCode)
        {
            dbContext = new PMGSYEntities();
            try
            {

                //district level
                if (levelID == 5)
                {
                    int? parentNdCode = dbContext.ADMIN_DEPARTMENT.Where(x => x.ADMIN_ND_CODE == adminNdCode).Select(x => x.MAST_PARENT_ND_CODE).First();
                    if (dbContext.ACC_BANK_DETAILS.Where(c => c.ADMIN_ND_CODE == parentNdCode && c.BANK_ACC_STATUS == true && c.FUND_TYPE == fundtype && c.ACCOUNT_TYPE == "S").Any())
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    if (dbContext.ACC_BANK_DETAILS.Any(c => c.ADMIN_ND_CODE == adminNdCode && c.BANK_ACC_STATUS == true && c.FUND_TYPE == fundtype && c.ACCOUNT_TYPE == "S"))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// Function to check whether opening balance entry is finalized
        /// </summary>
        /// <param name="objParam"></param>
        /// <returns></returns>
        public bool CheckIfOBFinalized(TransactionParams objParam)
        {
            try
            {
                dbContext = new PMGSYEntities();
                if (dbContext.ACC_BILL_MASTER.Where(m => m.ADMIN_ND_CODE == objParam.ADMIN_ND_CODE && m.FUND_TYPE == objParam.FUND_TYPE && m.LVL_ID == objParam.LVL_ID && m.BILL_TYPE == "O").Select(m => m.BILL_DATE).Any())
                {
                    if (dbContext.ACC_BILL_MASTER.Where(m => m.ADMIN_ND_CODE == objParam.ADMIN_ND_CODE && m.FUND_TYPE == objParam.FUND_TYPE && m.LVL_ID == objParam.LVL_ID && m.BILL_TYPE == "O").Select(m => m.BILL_FINALIZED).FirstOrDefault().ToString() == "N")
                    {
                        return false;
                    }
                    else return true;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        public bool CheckIAuthSignEnytered(TransactionParams objParam)
        {
            try
            {
                dbContext = new PMGSYEntities();
                if (dbContext.ADMIN_NODAL_OFFICERS.Where(m => m.ADMIN_ND_CODE == objParam.ADMIN_ND_CODE && m.ADMIN_NO_DESIGNATION == 26 && m.ADMIN_NO_LEVEL == "D" && m.ADMIN_MODULE == "A" && m.ADMIN_ACTIVE_STATUS == "Y").Any())
                {
                    return true;
                }
                else if (objParam.LVL_ID == 4)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// function to check whther chequebook details are entered
        /// </summary>
        /// <param name="fundtype"></param>
        /// <param name="levelID"></param>
        /// <param name="adminNdCode"></param>
        /// <returns></returns>
        public bool ChequeBookDetailsEntered(string fundtype, short levelID, int adminNdCode)
        {
            dbContext = new PMGSYEntities();
            try
            {
                if (levelID == 5)
                {
                    if (dbContext.ACC_CHQ_BOOK_DETAILS.Any(c => c.ADMIN_ND_CODE == adminNdCode && c.FUND_TYPE == fundtype && c.LVL_ID == levelID))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public bool CheckSrrdaOpeningBalance(TransactionParams objParam)
        {
            dbContext = new PMGSYEntities();
            try
            {
                if (PMGSYSession.Current.LevelId == 4)
                {
                    return true;
                }
                else if (PMGSYSession.Current.LevelId == 5)
                {
                    int? srrdaAdminCode = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).Select(m => m.MAST_PARENT_ND_CODE).FirstOrDefault();
                    if (dbContext.ACC_BILL_MASTER.Any(m => m.ADMIN_ND_CODE == srrdaAdminCode && m.FUND_TYPE == PMGSYSession.Current.FundType && m.BILL_TYPE == "O" && m.BILL_FINALIZED == "Y"))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }



        /// <summary>
        /// generic function for validation for accounting
        /// </summary>
        /// <param name="fundtype"></param>
        /// <param name="levelID"></param>
        /// <param name="adminNdCode"></param>
        /// <returns></returns>
        public AccountValidationModel GenericAccountingValidation(string fundtype, short levelID, int adminNdCode, string[] InputParameters)
        {
            dbContext = new PMGSYEntities();
            try
            {
                AccountValidationModel validationModel = new AccountValidationModel();
                TransactionParams transParam = new TransactionParams();
                transParam.ADMIN_ND_CODE = adminNdCode;
                transParam.LVL_ID = levelID;
                transParam.FUND_TYPE = fundtype;

                foreach (string parameter in InputParameters)
                {
                    switch (parameter)
                    {
                        case "BankDetails":
                            validationModel.BankDetailsEntered = IsBankDetailsEntered(fundtype, levelID, adminNdCode);
                            break;
                        case "ChequeBookDetails":
                            validationModel.ChequeBookDetailsEntered = ChequeBookDetailsEntered(fundtype, levelID, adminNdCode);
                            break;
                        case "OpeningBalanceDetails":
                            validationModel.OpeningBalanceFinalized = CheckIfOBFinalized(transParam);
                            break;
                        case "AuthSign":
                            validationModel.AuthSign = CheckIAuthSignEnytered(transParam);
                            break;
                        case "SrrdaOBEntered":
                            validationModel.SrrdaOBEntered = CheckSrrdaOpeningBalance(transParam);
                            break;
                        default:
                            validationModel.BankDetailsEntered = IsBankDetailsEntered(fundtype, levelID, adminNdCode);
                            validationModel.ChequeBookDetailsEntered = ChequeBookDetailsEntered(fundtype, levelID, adminNdCode);
                            validationModel.OpeningBalanceFinalized = CheckIfOBFinalized(transParam);
                            validationModel.AuthSign = CheckIAuthSignEnytered(transParam);
                            validationModel.SrrdaOBEntered = CheckSrrdaOpeningBalance(transParam);
                            break;
                    }
                }

                return validationModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        // addition by Koustubh Nakate on 12/07/2012 to populate list

        /// <summary>
        /// This function is used to get all contractor class as per logged in state code
        /// </summary>
        /// <param name="stateCode"></param>
        /// <param name="isSearch"></param>
        /// <returns></returns>
        public List<MASTER_CON_CLASS_TYPE> GetContractorClassByStateCode(int stateCode, bool isSearch)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {

                List<MASTER_CON_CLASS_TYPE> contractorClassList = dbContext.MASTER_CON_CLASS_TYPE.Where(cc => cc.MAST_STATE_CODE == stateCode).OrderBy(cc => cc.MAST_CON_CLASS_TYPE_NAME).ToList<MASTER_CON_CLASS_TYPE>();

                if (isSearch)
                {
                    contractorClassList.Insert(0, new MASTER_CON_CLASS_TYPE() { MAST_CON_CLASS = 0, MAST_CON_CLASS_TYPE_NAME = "All Contractor Class" });
                }
                else
                {
                    contractorClassList.Insert(0, new MASTER_CON_CLASS_TYPE() { MAST_CON_CLASS = 0, MAST_CON_CLASS_TYPE_NAME = "Select Contractor Class" });
                }

                return contractorClassList;

            }
            catch (Exception ex)
            {
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


        public List<IMS_SANCTIONED_PROJECTS> GetRoads(int sanctionedYear, string IMSPackageID, bool isSearch, bool isNIT = false, bool isEdit = false)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {

                List<IMS_SANCTIONED_PROJECTS> roadList = dbContext.IMS_SANCTIONED_PROJECTS.Where(IMS => IMS.IMS_YEAR == sanctionedYear && IMS.IMS_PACKAGE_ID.ToUpper() == IMSPackageID.ToUpper() && IMS.IMS_SANCTIONED == "Y" && IMS.IMS_DPR_STATUS == "N").OrderBy(IMS => IMS.IMS_ROAD_NAME).Distinct().ToList<IMS_SANCTIONED_PROJECTS>();

                //var query = from agreementDetails in dbContext.TEND_AGREEMENT_DETAIL
                //            join agreementMaster in dbContext.TEND_AGREEMENT_MASTER
                //            on agreementDetails.TEND_AGREEMENT_CODE equals agreementMaster.TEND_AGREEMENT_CODE
                //            where agreementMaster.TEND_AGREEMENT_STATUS != "W"
                //            select new
                //            {
                //                agreementDetails.IMS_PR_ROAD_CODE
                //            };


                //dbContext.TEND_AGREEMENT_DETAIL.Where(ad => ad.TEND_AGREEMENT_STATUS != "W").Select(ad => ad.IMS_PR_ROAD_CODE).Distinct();


                if (isNIT == true && isEdit == false)
                {
                    var query = (from agreementDetails in dbContext.TEND_AGREEMENT_DETAIL
                                 where agreementDetails.TEND_AGREEMENT_STATUS != "W"
                                 select new
                                 {
                                     agreementDetails.IMS_PR_ROAD_CODE
                                 }).Distinct();


                    roadList = (from roads in roadList
                                where !query.Any(IMSPRRoadCode => IMSPRRoadCode.IMS_PR_ROAD_CODE == roads.IMS_PR_ROAD_CODE)
                                select roads).ToList<IMS_SANCTIONED_PROJECTS>();

                    var NITRoads = (from NITDetails in dbContext.TEND_NIT_DETAILS

                                    select new
                                    {
                                        NITDetails.IMS_PR_ROAD_CODE
                                    }).Distinct();

                    roadList = (from roads in roadList
                                where !NITRoads.Any(IMSPRRoadCode => IMSPRRoadCode.IMS_PR_ROAD_CODE == roads.IMS_PR_ROAD_CODE)
                                select roads).ToList<IMS_SANCTIONED_PROJECTS>();
                }

                if (isSearch)
                {
                    roadList.Insert(0, new IMS_SANCTIONED_PROJECTS() { IMS_PR_ROAD_CODE = 0, IMS_ROAD_NAME = "All Roads" });
                }
                else
                {
                    roadList.Insert(0, new IMS_SANCTIONED_PROJECTS() { IMS_PR_ROAD_CODE = 0, IMS_ROAD_NAME = "Select Road" });

                }

                return roadList;

            }
            catch (Exception ex)
            {
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


        public List<IMS_PROPOSAL_WORK> GetWorks(int roadCode, bool isSearch)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {

                List<IMS_PROPOSAL_WORK> workList = dbContext.IMS_PROPOSAL_WORK.Where(pw => pw.IMS_PR_ROAD_CODE == roadCode).OrderBy(pw => pw.IMS_WORK_DESC).ToList<IMS_PROPOSAL_WORK>();




                if (isSearch)
                {
                    workList.Insert(0, new IMS_PROPOSAL_WORK() { IMS_WORK_CODE = 0, IMS_WORK_DESC = "All Works" });
                }
                else
                {
                    workList.Insert(0, new IMS_PROPOSAL_WORK() { IMS_WORK_CODE = 0, IMS_WORK_DESC = "Select Work" });

                }

                return workList;

            }
            catch (Exception ex)
            {
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

        public List<IMS_SANCTIONED_PROJECTS> GetPackages(int year, int block, bool isSearch)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {

                List<IMS_SANCTIONED_PROJECTS> packageList = (from sanctionProjects in dbContext.IMS_SANCTIONED_PROJECTS
                                                             where
                                                             (year == 0 ? 1 : sanctionProjects.IMS_YEAR) == (year == 0 ? 1 : year) &&
                                                                 //sanctionProjects.IMS_YEAR == year &&
                                                             (block <= 0 ? 1 : sanctionProjects.MAST_BLOCK_CODE) == (block <= 0 ? 1 : block) &&
                                                             sanctionProjects.IMS_SANCTIONED == "Y" &&
                                                             sanctionProjects.IMS_DPR_STATUS == "N" &&
                                                             sanctionProjects.MAST_STATE_CODE == PMGSYSession.Current.StateCode &&
                                                             sanctionProjects.MAST_DISTRICT_CODE == PMGSYSession.Current.DistrictCode &&
                                                             sanctionProjects.MAST_DPIU_CODE == PMGSYSession.Current.AdminNdCode &&
                                                             sanctionProjects.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme // new change done by Vikram as Packages should be populated based on the Scheme in session
                                                             select sanctionProjects).Distinct().ToList<IMS_SANCTIONED_PROJECTS>();


                //packageList=packageList.GroupBy(

                packageList = packageList.GroupBy(pl => pl.IMS_PACKAGE_ID).Select(pl => pl.FirstOrDefault()).ToList<IMS_SANCTIONED_PROJECTS>();

                if (isSearch)
                {
                    packageList.Insert(0, new IMS_SANCTIONED_PROJECTS() { IMS_ROAD_NAME = "0", IMS_PACKAGE_ID = "All Packages" });
                }
                else
                {
                    packageList.Insert(0, new IMS_SANCTIONED_PROJECTS() { IMS_ROAD_NAME = "0", IMS_PACKAGE_ID = "Select Package" });

                }

                return packageList.OrderBy(o => o.IMS_PACKAGE_ID).ToList();

            }
            catch (Exception ex)
            {
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
        /// This function is used to update agreement status 
        /// </summary>
        /// <param name="agreementCode"> id for agreement</param>

        /// <returns></returns>
        public void UpdateAgreementStatus(int agreementCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            TEND_AGREEMENT_MASTER agreementMaster = null;
            try
            {

                agreementMaster = dbContext.TEND_AGREEMENT_MASTER.Where(am => am.TEND_AGREEMENT_CODE == agreementCode).FirstOrDefault();

                if (dbContext.TEND_AGREEMENT_DETAIL.Any(ad => ad.TEND_AGREEMENT_CODE == agreementCode && ad.TEND_AGREEMENT_STATUS == "P"))
                {
                    agreementMaster.TEND_AGREEMENT_STATUS = "P";
                }
                else if (dbContext.TEND_AGREEMENT_DETAIL.Any(ad => ad.TEND_AGREEMENT_CODE == agreementCode && (ad.TEND_AGREEMENT_STATUS == "W" || ad.TEND_AGREEMENT_STATUS == "M")) && !dbContext.TEND_AGREEMENT_DETAIL.Any(ad => ad.TEND_AGREEMENT_CODE == agreementCode && ad.TEND_AGREEMENT_STATUS == "P"))
                {
                    agreementMaster.TEND_AGREEMENT_STATUS = "W";
                }
                else if (dbContext.TEND_AGREEMENT_DETAIL.All(ad => ad.TEND_AGREEMENT_CODE == agreementCode && ad.TEND_AGREEMENT_STATUS == "C"))
                {
                    agreementMaster.TEND_AGREEMENT_STATUS = "C";
                }

                dbContext.Entry(agreementMaster).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }

        }

        //end addition by Koustubh Nakate on 12/07/2012 to populate list

        //Added by Ashish Markande on 05/08/2013 to Get Fund Name
        public string GetFundName(string fundType)
        {
            string fundName = string.Empty;
            switch (fundType)
            {
                case "P":
                    fundName = "PMGSY PROGRAMME FUND";
                    break;
                case "M":
                    fundName = "PMGSY MAINTENANCE FUND";
                    break;
                case "A":
                    fundName = "PMGSY ADMINISTRATIVE FUND";
                    break;
            }
            return fundName;
        }

        /// <summary>
        /// function to validate the date of settlement entry against the receipt/ imprest settlement date
        /// </summary>
        /// <param name="billId"> payment/ ob bill id of imprest </param>
        /// <param name="imprestSettlementDate"></param>
        /// <returns></returns>
        public bool isValidImprestSttlementDate(long billId, string imprestSettlementDate)
        {
            PMGSYEntities dbContext = new PMGSYEntities();

            try
            {
                //get the date of imprest payment /ob 
                DateTime imprestDate = new DateTime();

                imprestDate = dbContext.ACC_BILL_MASTER.Where(x => x.BILL_ID == billId).Select(x => x.BILL_DATE).FirstOrDefault();

                if (imprestDate > this.GetStringToDateTime(imprestSettlementDate))
                {
                    return false;
                }
                else
                {
                    return true;
                }

            }
            catch (Exception ex)
            {
                return false;
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
        /// function to validate the date of settlement entry against the receipt/ imprest settlement date
        /// </summary>
        /// <param name="billId"> payment/ ob bill id of imprest </param>
        /// <param name="imprestSettlementDate"></param>
        /// <returns></returns>
        public bool isValidImprestSttlementDate(long billId, DateTime imprestSettlementDate)
        {
            PMGSYEntities dbContext = new PMGSYEntities();

            try
            {
                //get the date of imprest payment /ob 
                DateTime imprestDate = new DateTime();

                imprestDate = dbContext.ACC_BILL_MASTER.Where(x => x.BILL_ID == billId).Select(x => x.BILL_DATE).FirstOrDefault();

                if (imprestDate > imprestSettlementDate)
                {
                    return false;
                }
                else
                {
                    return true;
                }

            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }

        }

        #region by Vikram

        public List<SelectListItem> PopulateNodalAgencies(int stateCode)
        {
            List<SelectListItem> lstNodalAgency = new List<SelectListItem>();
            dbContext = new PMGSYEntities();
            try
            {

                var lstData = (from item in dbContext.ADMIN_DEPARTMENT
                               where item.MAST_ND_TYPE == "S" && item.MAST_STATE_CODE == stateCode
                               select new
                               {
                                   ADMIN_NAME = item.MASTER_STATE.MAST_STATE_NAME + "(" + item.MASTER_AGENCY.MAST_AGENCY_NAME + ")",
                                   ADMIN_CODE = item.ADMIN_ND_CODE
                               }).OrderBy(m => m.ADMIN_NAME).ToList().Distinct();

                foreach (var item in lstData)
                {
                    if (item.ADMIN_CODE == PMGSYSession.Current.AdminNdCode)
                    {
                        lstNodalAgency.Add(new SelectListItem { Text = item.ADMIN_NAME, Value = item.ADMIN_CODE.ToString(), Selected = true });
                    }
                    else
                    {
                        lstNodalAgency.Add(new SelectListItem { Text = item.ADMIN_NAME, Value = item.ADMIN_CODE.ToString() });
                    }
                }
                //lstNodalAgency.Insert(0, (new SelectListItem { Text = "--All--", Value = "0", Selected = true }));
                return lstNodalAgency;
            }
            catch (Exception)
            {
                return lstNodalAgency;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        public List<SelectListItem> PopulateNodalAgencies()
        {
            List<SelectListItem> lstNodalAgency = new List<SelectListItem>();
            dbContext = new PMGSYEntities();
            try
            {

                var lstData = (from item in dbContext.ADMIN_DEPARTMENT
                               where item.MAST_ND_TYPE == "S"
                               select new
                               {
                                   ADMIN_NAME = item.MASTER_STATE.MAST_STATE_NAME + " (" + item.MASTER_AGENCY.MAST_AGENCY_NAME + ")",
                                   ADMIN_CODE = item.ADMIN_ND_CODE
                               }).OrderBy(m => m.ADMIN_NAME).ToList().Distinct();


                foreach (var item in lstData)
                {
                    lstNodalAgency.Add(new SelectListItem { Text = item.ADMIN_NAME, Value = item.ADMIN_CODE.ToString() });
                }
                //lstNodalAgency.Insert(0, (new SelectListItem { Text = "--All--", Value = "0", Selected = true }));
                return lstNodalAgency;
            }
            catch (Exception)
            {
                return lstNodalAgency;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        public List<SelectListItem> PopulateSRRDA()
        {
            List<SelectListItem> lstNodalAgency = new List<SelectListItem>();
            dbContext = new PMGSYEntities();
            try
            {

                var lstData = (from item in dbContext.ADMIN_DEPARTMENT
                               where item.MAST_ND_TYPE == "S" && item.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode
                               select new
                               {
                                   ADMIN_NAME = item.MASTER_STATE.MAST_STATE_NAME + " (" + item.MASTER_AGENCY.MAST_AGENCY_NAME + ")",
                                   ADMIN_CODE = item.ADMIN_ND_CODE
                               }).OrderBy(m => m.ADMIN_NAME).ToList().Distinct();


                foreach (var item in lstData)
                {
                    lstNodalAgency.Add(new SelectListItem { Text = item.ADMIN_NAME, Value = item.ADMIN_CODE.ToString() });
                }
                lstNodalAgency.Insert(0, (new SelectListItem { Text = "--select SRRDA--", Value = "0", Selected = true }));
                return lstNodalAgency;
            }
            catch (Exception)
            {
                return lstNodalAgency;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        public List<SelectListItem> PopulateRoadExecutionItems()
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<SelectListItem> lstItems = new SelectList(dbContext.MASTER_EXECUTION_ITEM.Where(c => c.MAST_HEAD_TYPE.Equals("R")), "MAST_HEAD_CODE", "MAST_HEAD_DESC").ToList();
                lstItems.Insert(0, (new SelectListItem { Text = "Select Item", Value = "0" }));
                return lstItems;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public List<MASTER_TECHNOLOGY> PopulateTechnologyItems()
        {
            try
            {
                dbContext = new PMGSYEntities();
                //List<SelectListItem> lstItems = new SelectList(dbContext.MASTER_TECHNOLOGY, "MAST_TECH_CODE", "MAST_TECH_NAME").ToList();
                //lstItems.Insert(0, (new SelectListItem { Text = "Select Technology", Value = "0" }));
                //return lstItems;

                List<MASTER_TECHNOLOGY> lstItems = dbContext.MASTER_TECHNOLOGY.Where(m => m.MAST_TECH_STATUS == "Y").OrderBy(x => x.MAST_TECH_DESC).ToList();
                lstItems.Insert(0, (new MASTER_TECHNOLOGY { MAST_TECH_CODE = 0,MAST_TECH_DESC = "" , MAST_TECH_NAME = "Select Technology" , MAST_TECH_TYPE = "", MAST_TECH_STATUS = ""}));
                return lstItems;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }




        #endregion


        /// <summary>
        /// returns list of districts mapped with particular SRRDA Department
        /// </summary>
        /// <param name="adminNdCode"></param>
        /// <param name="isAllSelected"></param>
        /// <returns></returns>
        public List<SelectListItem> PopulateDistrictOfSRRDADept(Int32 adminNdCode, bool isAllSelected = false)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<SelectListItem> lstDistrict = new List<SelectListItem>();

                var lstDist = (from distMaster in dbContext.MASTER_DISTRICT
                               join deptMaster in dbContext.ADMIN_DEPARTMENT on distMaster.MAST_DISTRICT_CODE equals deptMaster.MAST_DISTRICT_CODE
                               where deptMaster.MAST_PARENT_ND_CODE == adminNdCode
                               select new
                               {
                                   distMaster.MAST_DISTRICT_CODE,
                                   distMaster.MAST_DISTRICT_NAME
                               }).OrderBy(d => d.MAST_DISTRICT_NAME).ToList().Distinct();

                foreach (var item in lstDist)
                {
                    lstDistrict.Add(new SelectListItem { Text = item.MAST_DISTRICT_NAME, Value = item.MAST_DISTRICT_CODE.ToString() });
                }

                if (isAllSelected == false)
                {
                    lstDistrict.Insert(0, (new SelectListItem { Text = "Select District", Value = "0", Selected = true }));
                }
                else if (isAllSelected == true)
                {
                    lstDistrict.Insert(0, (new SelectListItem { Text = "All District", Value = "-1", Selected = true }));
                }
                return lstDistrict;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        /// <summary>
        /// Populate Fund Types
        /// </summary>
        /// <param name="isAllSelected"></param>
        /// <returns></returns>
        public List<SelectListItem> PopulateFundTypes(bool isAllSelected)
        {
            try
            {
                List<SelectListItem> lstItems = new List<SelectListItem>();
                if (isAllSelected)
                {
                    lstItems.Insert(0, (new SelectListItem { Text = "All Fund Type", Value = "0" }));
                }

                lstItems.Insert(0, (new SelectListItem { Text = "Programme Fund", Value = "P" }));
                lstItems.Insert(1, (new SelectListItem { Text = "Administrative Fund", Value = "A" }));
                lstItems.Insert(2, (new SelectListItem { Text = "Maintenance Fund", Value = "M" }));

                return lstItems;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #region Image Thumbnail Generation & Creation Of Image File



        /// <summary>
        /// Calls CompressImage & GenerateThumbnail methods to save Image & Thumnails to Disk
        /// Creates Image from Byte Array 
        /// </summary>
        /// <param name="imgData"></param>
        /// <param name="basePath"></param>
        /// <param name="fullPath"></param>
        /// <param name="thumbnailPath"></param>
        /// <returns></returns>
        public int SaveImage(byte[] imgData, string basePath, string fullPath, string thumbnailPath)
        {
            MemoryStream memoryStream = null;
            string ErrorLogFilePath = System.Configuration.ConfigurationSettings.AppSettings["MABQMSErrorLogPath"] + "//MABQMSSaveImageErrorLog_" + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + ".txt";
            try
            {
                if (!Directory.Exists(basePath))
                {
                    Directory.CreateDirectory(basePath);
                }
                if (!Directory.Exists(basePath + "\\thumbnails\\"))
                {
                    Directory.CreateDirectory(basePath + "\\thumbnails\\");
                }

                memoryStream = new MemoryStream(imgData);
                if (CompressImage(fullPath, memoryStream))
                    GenerateThumbnail(thumbnailPath, memoryStream);

                return 1;
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = System.IO.File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "SaveImage()");
                    sw.WriteLine("Database Exception : " + ex.Message);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                return -1;
            }
            finally
            {
                if (memoryStream != null)
                    memoryStream.Close();
            }
        }


        /// <summary>
        /// Compress Image in consideration with actual Size of Image
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="memoryStream"></param>
        /// <returns></returns>
        public bool CompressImage(string fullPath, MemoryStream memoryStream)
        {
            Bitmap srcBmp = null;
            Bitmap target = null;
            try
            {
                srcBmp = new Bitmap(memoryStream);

                int ImageHeight = srcBmp.Height;
                int ImageWidth = srcBmp.Width;

                SizeF newSize = new SizeF(srcBmp.Width, srcBmp.Height);

                if (ImageWidth <= 1024 || ImageHeight <= 768)
                    newSize = new SizeF((srcBmp.Width * 80) / 100, (srcBmp.Height * 80) / 100);

                if ((ImageWidth > 1024 && ImageWidth <= 2048) || (ImageHeight > 768 && ImageHeight <= 1536))
                    newSize = new SizeF((srcBmp.Width * 40) / 100, (srcBmp.Height * 40) / 100);

                if ((ImageWidth > 2048) || (ImageHeight > 1536))
                    newSize = new SizeF((srcBmp.Width * 30) / 100, (srcBmp.Height * 20) / 100);

                else
                    newSize = new SizeF(800, 600);

                target = new Bitmap((int)newSize.Width, (int)newSize.Height);

                using (Graphics graphics = Graphics.FromImage(target))
                {
                    graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                    graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                    graphics.DrawImage(srcBmp, 0, 0, newSize.Width, newSize.Height);
                    target.Save(fullPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                    target.Dispose();
                    graphics.Dispose();
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                if (srcBmp != null)
                    srcBmp.Dispose();
                if (target != null)
                    target.Dispose();
            }
        }


        /// <summary>
        /// Generate Thumbnail of Image depending upon size of image
        /// </summary>
        /// <param name="ms"></param>
        /// <param name="Path"></param>
        /// <param name="FileName"></param>
        public void GenerateThumbnail(string thumbnailPath, MemoryStream memoryStream)
        {
            Bitmap srcBmp = null;
            Bitmap target = null;
            try
            {
                // Code to generate Thumbnail & save it
                srcBmp = new Bitmap(memoryStream);
                //calculate original height & width
                int ImageHeight = srcBmp.Height;
                int ImageWidth = srcBmp.Width;

                //Calculate new size
                SizeF newSize = new SizeF(srcBmp.Width, srcBmp.Height);

                if ((ImageHeight < 120 && ImageWidth < 120) || (ImageWidth < 120 && ImageHeight < 120))
                    newSize = new SizeF(srcBmp.Width, srcBmp.Height);

                else if ((ImageWidth <= 1024 && ImageHeight <= 768) || (ImageHeight <= 1024 && ImageWidth <= 768))
                    newSize = new SizeF((srcBmp.Width * 20) / 100, (srcBmp.Height * 20) / 100);

                else if ((ImageWidth > 1024 && ImageHeight < 768) || (ImageWidth < 768 && ImageHeight > 1024))
                    newSize = new SizeF((srcBmp.Width * 15) / 100, (srcBmp.Height * 15) / 100);

                else if (((ImageWidth > 1024 && ImageWidth <= 2048) && (ImageHeight > 768 && ImageHeight <= 1536)) ||
                        ((ImageHeight > 1024 && ImageHeight <= 2048) && (ImageWidth > 768 && ImageWidth <= 1536)))
                    newSize = new SizeF((srcBmp.Width * 10) / 100, (srcBmp.Height * 10) / 100);

                else if (((ImageWidth > 2048) && (ImageHeight > 1536)) || ((ImageHeight > 2048) && (ImageWidth > 1536)))
                    newSize = new SizeF((srcBmp.Width * 5) / 100, (srcBmp.Height * 5) / 100);

                else
                    newSize = new SizeF(120, 120);

                target = new Bitmap((int)newSize.Width, (int)newSize.Height);

                using (Graphics graphics = Graphics.FromImage(target))
                {
                    graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                    graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                    graphics.DrawImage(srcBmp, 0, 0, newSize.Width, newSize.Height);
                    target.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                    srcBmp.Dispose();
                    target.Dispose();
                    graphics.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (srcBmp != null)
                    srcBmp.Dispose();
                if (target != null)
                    target.Dispose();
            }
        }





        #endregion

        //Added by Aanand Singh
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stateCode"></param>
        /// <returns></returns>
        //public List<SelectListItem> PopulateNodalAgencies(int stateCode)
        //{
        //    List<SelectListItem> lstNodalAgency = new List<SelectListItem>();
        //    dbContext = new PMGSYEntities();
        //    try
        //    {

        //        var lstData = (from item in dbContext.ADMIN_DEPARTMENT
        //                       where item.MAST_ND_TYPE == "S" && item.MAST_STATE_CODE == stateCode
        //                       select new
        //                       {
        //                           ADMIN_NAME = item.MASTER_STATE.MAST_STATE_NAME + "(" + item.MASTER_AGENCY.MAST_AGENCY_NAME + ")",
        //                           ADMIN_CODE = item.ADMIN_ND_CODE
        //                       }).OrderBy(m => m.ADMIN_NAME).ToList().Distinct();


        //        foreach (var item in lstData)
        //        {
        //            lstNodalAgency.Add(new SelectListItem { Text = item.ADMIN_NAME, Value = item.ADMIN_CODE.ToString() });
        //        }
        //        //lstNodalAgency.Insert(0, (new SelectListItem { Text = "--All--", Value = "0", Selected = true }));
        //        return lstNodalAgency;
        //    }
        //    catch (Exception)
        //    {
        //        return lstNodalAgency;
        //    }
        //    finally
        //    {
        //        if (dbContext != null)
        //        {
        //            dbContext.Dispose();
        //        }
        //    }
        //}

        //public List<SelectListItem> PopulateNodalAgencies()
        //{
        //    List<SelectListItem> lstNodalAgency = new List<SelectListItem>();
        //    dbContext = new PMGSYEntities();
        //    try
        //    {

        //        var lstData = (from item in dbContext.ADMIN_DEPARTMENT
        //                       where item.MAST_ND_TYPE == "S"
        //                       select new
        //                       {
        //                           ADMIN_NAME = item.MASTER_STATE.MAST_STATE_NAME + "(" + item.MASTER_AGENCY.MAST_AGENCY_NAME + ")",
        //                           ADMIN_CODE = item.ADMIN_ND_CODE
        //                       }).OrderBy(m => m.ADMIN_NAME).ToList().Distinct();


        //        foreach (var item in lstData)
        //        {
        //            lstNodalAgency.Add(new SelectListItem { Text = item.ADMIN_NAME, Value = item.ADMIN_CODE.ToString() });
        //        }
        //        lstNodalAgency.Insert(0, (new SelectListItem { Text = "--Select--", Value = "0", Selected = true }));
        //        return lstNodalAgency;
        //    }
        //    catch (Exception)
        //    {
        //        return lstNodalAgency;
        //    }
        //    finally
        //    {
        //        if (dbContext != null)
        //        {
        //            dbContext.Dispose();
        //        }
        //    }
        //}




        public ACC_RPT_REPORT_MASTER GetReportId(int? reportLevel, string fundType, int menuId)
        {
            dbContext = new PMGSYEntities();
            ACC_RPT_REPORT_MASTER reportMaster = (from report in dbContext.ACC_RPT_REPORT_MASTER
                                                  where report.FUND_TYPE == fundType && report.LVL_ID_IN_DATA == reportLevel && report.MenuID == menuId
                                                  select report).FirstOrDefault();
            return reportMaster;
        }


        //new method added by Vikram on 19-10-2013
        //for validation of road whether it is new/upgraded road
        public bool ValidateRoad(int proposalCode, string upgradeConnectFlag)
        {
            dbContext = new PMGSYEntities();
            try
            {
                if (dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == proposalCode).Select(m => m.IMS_UPGRADE_CONNECT).FirstOrDefault() == upgradeConnectFlag)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        //new method added by Abhishek on 2 Sep 2014
        //for validation of road whether it is PMGSY I or PMGSY II road
        public bool ValidateRoadForPMGSYScheme(short headTxnID, int RoadCode, bool isSubTxnID = true)
        {
            dbContext = new PMGSYEntities();
            try
            {
                int HeadId = 0;
                //Find Head Id from transaction ID          
                if (isSubTxnID)
                {
                    var varHeadDesc = (from h in dbContext.ACC_MASTER_HEAD
                                       join map in dbContext.ACC_TXN_HEAD_MAPPING on h.HEAD_ID equals map.HEAD_ID
                                       where map.TXN_ID == headTxnID
                                       && map.CREDIT_DEBIT == "D"
                                       select new
                                       {
                                           map.HEAD_ID
                                       }).FirstOrDefault();
                    HeadId = varHeadDesc.HEAD_ID;
                }
                else
                {
                    HeadId = headTxnID;
                }
                //Road check for PMGSY1 Scheme (head 28,29,30,31,524)                
                if ((HeadId == 28) || (HeadId == 29) || (HeadId == 30) || (HeadId == 31) || (HeadId == 524) || (HeadId == 528) || (HeadId == 529) || (HeadId == 530))
                {
                    if (dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == RoadCode).Select(m => m.MAST_PMGSY_SCHEME).FirstOrDefault() == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                //Road check for PMGSY 2 Scheme (head 385,386,525) 
                else if ((HeadId == 385) || (HeadId == 386) || (HeadId == 525) || (HeadId == 531) || (HeadId == 532) || (HeadId == 533))      //Expenditure on Upgradation of PMGSY-II Roads in Plain Areas / Special Areas
                {                                       
                    if (dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == RoadCode).Select(m => m.MAST_PMGSY_SCHEME).FirstOrDefault() == 2)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                //Changed by SAMMED A. PATIL on 19FEB2018 Road check for RCPLWE Scheme (head 427, 428, 429, 430,527)
                else if ((HeadId == 427) || (HeadId == 428) || (HeadId == 429) || (HeadId == 430) || (HeadId == 527) || (HeadId == 538) || (HeadId == 539) || (HeadId == 540))
                {
                    if (dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == RoadCode).Select(m => m.MAST_PMGSY_SCHEME).FirstOrDefault() == 3)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                //Changed by SAMMED A. PATIL on 09JAN2010 Road check for PMGSY3 Scheme (head 464,465,526)
                else if (HeadId == 464 || HeadId == 465 || (HeadId == 526) || (HeadId == 534) || (HeadId == 535) || (HeadId == 536) || (HeadId == 537))
                {
                    if (dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == RoadCode).Select(m => m.MAST_PMGSY_SCHEME).FirstOrDefault() == 4)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Method added by Ujjwal Saket
        /// Populates States only mapped to particular PTA
        /// </summary>
        /// <param name="StateCode"></param>
        /// <param name="isAllSelected"></param>
        /// <param name="selectedDistrictCode"></param>
        /// <returns></returns>
        public List<SelectListItem> PopulateStatesOfPTA(bool isAllSelected = false, Int32 selectedStateCode = 0)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<SelectListItem> lstStates = null;

                Int32 taCode = dbContext.ADMIN_TECHNICAL_AGENCY.Where(a => a.ADMIN_USER_ID == PMGSYSession.Current.UserId).Select(a => a.ADMIN_TA_CODE).FirstOrDefault();

                var ptaMappedStates = dbContext.ADMIN_TA_STATE.Where(x => x.ADMIN_TA_CODE == taCode).Select(p => p.MAST_STATE_CODE).ToList();

                lstStates = new SelectList(dbContext.MASTER_STATE.Where(m => m.MAST_STATE_ACTIVE == "Y" && ptaMappedStates.Contains(m.MAST_STATE_CODE)).OrderBy(s => s.MAST_STATE_NAME), "MAST_STATE_CODE", "MAST_STATE_NAME", selectedStateCode).ToList();

                if (isAllSelected == false)
                {
                    lstStates.Insert(0, (new SelectListItem { Text = "Select State", Value = "0", Selected = true }));
                }
                else if (isAllSelected == true)
                {
                    lstStates.Insert(0, (new SelectListItem { Text = "All State", Value = "-1", Selected = true }));
                }
                return lstStates;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        /// <summary>
        /// Method added by shyam
        /// Populates districts only mapped to particular STA or PTA
        /// </summary>
        /// <param name="StateCode"></param>
        /// <param name="isAllSelected"></param>
        /// <param name="selectedDistrictCode"></param>
        /// <returns></returns>
        public List<SelectListItem> PopulateDistrictsOfTA(Int32 StateCode, bool isAllSelected = false, Int32 selectedDistrictCode = 0)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<SelectListItem> lstDistrict = null;

                Int32 taCode = dbContext.ADMIN_TECHNICAL_AGENCY.Where(a => a.ADMIN_USER_ID == PMGSYSession.Current.UserId).Select(a => a.ADMIN_TA_CODE).FirstOrDefault();

                //change by Ujjwal Saket on 7/1/2014 added (&& x.MAST_IS_FINALIZED=="Y" && x.MAST_IS_ACTIVE=="Y")
                //var taMappedDistricts = dbContext.ADMIN_TA_STATE.Where(x => x.ADMIN_TA_CODE == taCode && x.MAST_IS_FINALIZED == "Y" && x.MAST_IS_ACTIVE == "Y").Select(p => p.MAST_DISTRICT_CODE).ToList();

                //var query = dbContext.ADMIN_TA_STATE.Where(x => x.ADMIN_TA_CODE == taCode && x.MAST_IS_FINALIZED == "Y" && x.MAST_IS_ACTIVE == "Y" && (x.MAST_SCHEME == 3 || x.MAST_SCHEME == 5 || x.MAST_SCHEME == 6 || x.MAST_SCHEME == 7) && (x.MAST_END_DATE.HasValue ? x.MAST_END_DATE.Value >= DateTime.Now : x.MAST_END_DATE == null));

                ///Changes for STA RCPLWE Mapping
                /*var taMappedDistricts = PMGSYSession.Current.PMGSYScheme == 3
                                        ? dbContext.ADMIN_TA_STATE.Where(x => x.ADMIN_TA_CODE == taCode && x.MAST_IS_FINALIZED == "Y" && x.MAST_IS_ACTIVE == "Y" && (x.MAST_SCHEME == 3 || x.MAST_SCHEME == 5 || x.MAST_SCHEME == 6 || x.MAST_SCHEME == 7 || x.MAST_SCHEME == 11 || x.MAST_SCHEME == 13 || x.MAST_SCHEME == 14) && (x.MAST_END_DATE.HasValue ? x.MAST_END_DATE.Value >= DateTime.Now : x.MAST_END_DATE == null)).Select(p => p.MAST_DISTRICT_CODE).ToList()
                                        : PMGSYSession.Current.PMGSYScheme == 2
                                            ? dbContext.ADMIN_TA_STATE.Where(x => x.ADMIN_TA_CODE == taCode && x.MAST_IS_FINALIZED == "Y" && x.MAST_IS_ACTIVE == "Y" && (x.MAST_SCHEME == 2 || x.MAST_SCHEME == 4 || x.MAST_SCHEME == 5 || x.MAST_SCHEME == 7 || x.MAST_SCHEME == 10 || x.MAST_SCHEME == 12 || x.MAST_SCHEME == 14) && (x.MAST_END_DATE.HasValue ? x.MAST_END_DATE.Value >= DateTime.Now : x.MAST_END_DATE == null)).Select(p => p.MAST_DISTRICT_CODE).ToList()
                                            : PMGSYSession.Current.PMGSYScheme == 4
                                            ? dbContext.ADMIN_TA_STATE.Where(x => x.ADMIN_TA_CODE == taCode && x.MAST_IS_FINALIZED == "Y" && x.MAST_IS_ACTIVE == "Y" && (x.MAST_SCHEME == 8 || x.MAST_SCHEME == 9 || x.MAST_SCHEME == 10 || x.MAST_SCHEME == 11 || x.MAST_SCHEME == 12 || x.MAST_SCHEME == 13 || x.MAST_SCHEME == 14) && (x.MAST_END_DATE.HasValue ? x.MAST_END_DATE.Value >= DateTime.Now : x.MAST_END_DATE == null)).Select(p => p.MAST_DISTRICT_CODE).ToList()
                                            : dbContext.ADMIN_TA_STATE.Where(x => x.ADMIN_TA_CODE == taCode && x.MAST_IS_FINALIZED == "Y" && x.MAST_IS_ACTIVE == "Y" && (x.MAST_SCHEME == 1 || x.MAST_SCHEME == 4 || x.MAST_SCHEME == 6 || x.MAST_SCHEME == 7 || x.MAST_SCHEME == 9 || x.MAST_SCHEME == 12 || x.MAST_SCHEME == 13 || x.MAST_SCHEME == 14) && (x.MAST_END_DATE.HasValue ? x.MAST_END_DATE.Value >= DateTime.Now : x.MAST_END_DATE == null)).Select(p => p.MAST_DISTRICT_CODE).ToList();
*/
                //Changed By Hrishikesh For Vibrant Village Mapp Scheme --26-07-2023-- original is Commented Above --start
                var taMappedDistricts = PMGSYSession.Current.PMGSYScheme == 3
                                      ? dbContext.ADMIN_TA_STATE.Where(x => x.ADMIN_TA_CODE == taCode && x.MAST_IS_FINALIZED == "Y" && x.MAST_IS_ACTIVE == "Y" && (x.STR_MAST_SCHEME.Contains(",3,") || x.STR_MAST_SCHEME.EndsWith("3") || x.STR_MAST_SCHEME.StartsWith("3,")) && (x.MAST_END_DATE.HasValue ? x.MAST_END_DATE.Value >= DateTime.Now : x.MAST_END_DATE == null)).Select(p => p.MAST_DISTRICT_CODE).ToList()
                                      : PMGSYSession.Current.PMGSYScheme == 2
                                          ? dbContext.ADMIN_TA_STATE.Where(x => x.ADMIN_TA_CODE == taCode && x.MAST_IS_FINALIZED == "Y" && x.MAST_IS_ACTIVE == "Y" && (x.STR_MAST_SCHEME.Contains(",2,") || x.STR_MAST_SCHEME.EndsWith("2") || x.STR_MAST_SCHEME.StartsWith("2,")) && (x.MAST_END_DATE.HasValue ? x.MAST_END_DATE.Value >= DateTime.Now : x.MAST_END_DATE == null)).Select(p => p.MAST_DISTRICT_CODE).ToList()
                                          : PMGSYSession.Current.PMGSYScheme == 4
                                          ? dbContext.ADMIN_TA_STATE.Where(x => x.ADMIN_TA_CODE == taCode && x.MAST_IS_FINALIZED == "Y" && x.MAST_IS_ACTIVE == "Y" && (x.STR_MAST_SCHEME.Contains(",4,") || x.STR_MAST_SCHEME.EndsWith("4") || x.STR_MAST_SCHEME.StartsWith("4,")) && (x.MAST_END_DATE.HasValue ? x.MAST_END_DATE.Value >= DateTime.Now : x.MAST_END_DATE == null)).Select(p => p.MAST_DISTRICT_CODE).ToList()
                                          : PMGSYSession.Current.PMGSYScheme == 5
                                          ? dbContext.ADMIN_TA_STATE.Where(x => x.ADMIN_TA_CODE == taCode && x.MAST_IS_FINALIZED == "Y" && x.MAST_IS_ACTIVE == "Y" && (x.STR_MAST_SCHEME.Contains(",5,") || x.STR_MAST_SCHEME.EndsWith("5") || x.STR_MAST_SCHEME.StartsWith("5,")) && (x.MAST_END_DATE.HasValue ? x.MAST_END_DATE.Value >= DateTime.Now : x.MAST_END_DATE == null)).Select(p => p.MAST_DISTRICT_CODE).ToList()
                                          : dbContext.ADMIN_TA_STATE.Where(x => x.ADMIN_TA_CODE == taCode && x.MAST_IS_FINALIZED == "Y" && x.MAST_IS_ACTIVE == "Y" && (x.STR_MAST_SCHEME.Contains(",1,") || x.STR_MAST_SCHEME.EndsWith("1") || x.STR_MAST_SCHEME.StartsWith("1,")) && (x.MAST_END_DATE.HasValue ? x.MAST_END_DATE.Value >= DateTime.Now : x.MAST_END_DATE == null)).Select(p => p.MAST_DISTRICT_CODE).ToList();
                //Changed By Hrishikesh For Vibrant Village Mapp Scheme --26-07-2023 --end

                lstDistrict = new SelectList(dbContext.MASTER_DISTRICT.Where(m => m.MAST_STATE_CODE == StateCode && taMappedDistricts.Contains(m.MAST_DISTRICT_CODE) && m.MAST_DISTRICT_ACTIVE == "Y").OrderBy(s => s.MAST_DISTRICT_NAME), "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME", selectedDistrictCode).ToList();

                if (isAllSelected == false)
                {
                    lstDistrict.Insert(0, (new SelectListItem { Text = "Select District", Value = "0", Selected = true }));
                }
                else if (isAllSelected == true)
                {
                    lstDistrict.Insert(0, (new SelectListItem { Text = "All District", Value = "-1", Selected = true }));
                }
                return lstDistrict;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        //new method added by Vikram for populating the months which are not closed according to the financial year
        public List<SelectListItem> PopulateRunningMonthsByYear(int year, int AdminNdCode, int LevelId)
        {
            dbContext = new PMGSYEntities();
            try
            {

                var lstClosedMonths = (from item in dbContext.ACC_RPT_MONTHWISE_SUMMARY
                                       where item.ADMIN_ND_CODE == AdminNdCode &&
                                       item.FUND_TYPE == PMGSYSession.Current.FundType
                                       && item.LVL_ID == LevelId //added by abhishek kamble 14-feb-2014
                                       select new
                                       {
                                           MONTH = item.ACC_MONTH,
                                           YEAR = item.ACC_YEAR,
                                           ID = (item.ACC_MONTH + item.ACC_YEAR * 12)
                                       }).Distinct().ToList();


                //Added By Abhishek kamble 14-feb-2014 start
                int? entryYear = 0;
                Int16? entryMonth = 0;

                if (lstClosedMonths != null)
                {
                    entryYear = lstClosedMonths.Max(m => (int?)m.YEAR);

                    if (entryYear != null)
                    {
                        entryMonth = lstClosedMonths.Where(m => m.YEAR == entryYear).Max(s => (short?)s.MONTH);

                        if (entryMonth == 12)
                        {
                            entryMonth = 1;
                            //entryYear = entryYear + 1;
                        }
                        else
                        {
                            entryMonth = Convert.ToInt16(entryMonth + 1);
                            //entryYear = entryYear + 1;
                        }
                    }
                }
                //Added By Abhishek kamble 14-feb-2014 end

                //Commented By Abhishek kamble 14-feb-2014 start
                //var lstBillMaster = (from item in dbContext.ACC_BILL_MASTER
                //                     where item.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode &&
                //                     item.FUND_TYPE == PMGSYSession.Current.FundType
                //                     select new
                //                     {
                //                         MONTH = (Int32)item.BILL_MONTH,
                //                         YEAR = (Int32)item.BILL_YEAR,
                //                         ID = ((Int32)item.BILL_MONTH + (Int32)item.BILL_YEAR * 12)
                //                     }).Distinct().ToList();

                //lstBillMaster = lstBillMaster.Where(m => m.YEAR == year).ToList();

                //var list = lstBillMaster.Where(m=>!lstClosedMonths.Contains(m)).Select(m=>m.MONTH).ToList();
                //Commented By Abhishek kamble 14-feb-2014 end


                //        int? month = dbContext.ACC_BILL_MASTER.Where(m => m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && 
                //            (m.BILL_MONTH + (m.BILL_YEAR * 12) > 3 + (year * 12)) && (m.BILL_MONTH + (m.BILL_YEAR * 12) < 4 + ((year+1) * 12))
                //&& m.FUND_TYPE == PMGSYSession.Current.FundType).Min(m => (Int32?)(m.BILL_MONTH + m.BILL_YEAR * 12));

                //        var lstMonths = dbContext.ACC_RPT_MONTHWISE_SUMMARY.Where(m => m.ACC_YEAR == year && m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && m.FUND_TYPE == PMGSYSession.Current.FundType).Select(m=>m.ACC_MONTH).ToList();

                //        int yearToCheck;

                //        var lstAllMonths = from item in dbContext.MASTER_MONTH
                //                           where !lstMonths.Contains(item.MAST_MONTH_CODE) &&
                //                           //(item.MAST_MONTH_CODE + year *12) > (3 + year * 12) &&
                //                           (item.MAST_MONTH_CODE < 3 ?(item.MAST_MONTH_CODE + (year + 1) * 12) > (3 + (year+1) * 12):(item.MAST_MONTH_CODE + year *12) > (3 + year * 12) ) &&
                //                           (item.MAST_MONTH_CODE < 3?(item.MAST_MONTH_CODE + (year + 1) * 12) < (4 + ((year+1) * 12))  :(item.MAST_MONTH_CODE + (year) * 12) < (4 + (year+1) * 12)) &&
                //                           //(item.MAST_MONTH_CODE + (year) * 12) < (4 + ((year+1) * 12)) &&
                //                           item.MAST_MONTH_CODE < 3 ? (item.MAST_MONTH_CODE + (year + 1) * 12) >= month : (item.MAST_MONTH_CODE + (year) * 12) >= month
                //                           select new 
                //                           {
                //                               MONTHCODE = item.MAST_MONTH_CODE,
                //                               MONTHNAME = item.MAST_MONTH_FULL_NAME
                //                           };

                List<SelectListItem> lstMonths = new List<SelectListItem>();

                //Commented By Abhishek kamble 14-feb-2014 start
                //if (list.Count > 0)
                //{
                //    var lstMasterMonths = (from item in dbContext.MASTER_MONTH
                //                           where list.Contains((Int32)item.MAST_MONTH_CODE)
                //                           select new
                //                           {
                //                               MONTHCODE = item.MAST_MONTH_CODE,
                //                               MONTHNAME = item.MAST_MONTH_FULL_NAME
                //                           }).ToList();

                //    lstMonths = new SelectList(lstMasterMonths, "MONTHCODE", "MONTHNAME").ToList();
                //}
                //else
                //{
                //    lstMonths.Add(new SelectListItem { Value = "0",Text = "Select Month"});
                //}
                //Commented By Abhishek kamble 14-feb-2014 end

                //Added By Abhishek kamble 14-feb-2014 start
                if (entryMonth != 0 && entryMonth != null)
                {
                    var lstMasterMonths = (from item in dbContext.MASTER_MONTH
                                           where item.MAST_MONTH_CODE == entryMonth
                                           select new
                                           {
                                               MONTHCODE = item.MAST_MONTH_CODE,
                                               MONTHNAME = item.MAST_MONTH_FULL_NAME
                                           }).ToList();
                    lstMonths = new SelectList(lstMasterMonths, "MONTHCODE", "MONTHNAME", entryMonth).ToList();
                }
                else
                {
                    //Commented by Abhishek kamble 31-Mar-2014
                    //lstMonths.Add(new SelectListItem { Value = "0", Text = "Select Month" });

                    //Added By Abhishek kamble 31-Mar-2014
                    short? OBMonth = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_TYPE == "O" && m.LVL_ID == LevelId && m.FUND_TYPE == PMGSYSession.Current.FundType && m.ADMIN_ND_CODE == AdminNdCode).Select(s => s.BILL_MONTH).FirstOrDefault();

                    if (OBMonth != null)
                    {
                        var lstMasterMonths = (from item in dbContext.MASTER_MONTH
                                               where item.MAST_MONTH_CODE == OBMonth
                                               select new
                                               {
                                                   MONTHCODE = item.MAST_MONTH_CODE,
                                                   MONTHNAME = item.MAST_MONTH_FULL_NAME
                                               }).ToList();
                        lstMonths = new SelectList(lstMasterMonths, "MONTHCODE", "MONTHNAME", OBMonth).ToList();
                    }
                    else
                    {
                        lstMonths.Add(new SelectListItem { Value = "0", Text = "Select Month" });
                    }

                }
                //Added By Abhishek kamble 14-feb-2014 end

                return lstMonths;
                //return new SelectList(lstAllMonths, "MONTHCODE", "MONTHNAME").ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<SelectListItem> PopulateReportHeads(int headCategory)
        {
            dbContext = new PMGSYEntities();
            try
            {
                return new SelectList(dbContext.ACC_MASTER_HEAD.Where(m => m.HEAD_CATEGORY_ID == headCategory && m.FUND_TYPE == PMGSYSession.Current.FundType).ToList(), "HEAD_ID", "HEAD_NAME").ToList();
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
        /// Checks whether the master transaction cash_cheque flag matches with the screen design parameters or not
        /// </summary>
        /// <param name="billId">master transaction id</param>
        /// <returns>true/false</returns>
        public bool IsValidTransaction(long billId, ref string message)
        {
            dbContext = new PMGSYEntities();
            try
            {
                ACC_BILL_MASTER billMaster = dbContext.ACC_BILL_MASTER.Find(billId);
                if (billMaster != null)
                {
                    if (billMaster.CHQ_EPAY == "E" || billMaster.CHQ_EPAY == "A")//Added for Advice no 8Apr2015
                    {
                        billMaster.CHQ_EPAY = "Q";
                    }

                    if (dbContext.ACC_SCREEN_DESIGN_PARAM_MASTER.Any(m => m.TXN_ID == billMaster.TXN_ID && m.DED_REQ == "B"))
                    {
                        return true;
                    }

                    if (dbContext.ACC_SCREEN_DESIGN_PARAM_MASTER.Any(m => m.TXN_ID == billMaster.TXN_ID && (m.CASH_CHQ == billMaster.CHQ_EPAY || m.CASH_CHQ.Contains(billMaster.CHQ_EPAY))))
                    {
                        return true;
                    }
                    else
                    {
                        message = dbContext.ACC_MASTER_TXN.Where(m => m.TXN_ID == billMaster.TXN_ID).Select(m => m.TXN_DESC).FirstOrDefault() + " can not be " + (billMaster.CHQ_EPAY == "C" ? "Cash Transaction" : "Cheque Transaction");
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        /// <summary>
        /// Insert Notifications for NQMs in Table
        /// </summary>
        /// <param name="qmCode"></param>
        /// <param name="message"></param>
        /// <param name="ipAdd"></param>
        /// <returns></returns>
        public int NotificationForMonitors(int qmCode, string message, string ipAdd)
        {
            dbContext = new PMGSYEntities();
            try
            {
                int userId = Convert.ToInt32(dbContext.ADMIN_QUALITY_MONITORS.Where(c => c.ADMIN_QM_CODE == qmCode).Select(c => c.ADMIN_USER_ID).First());
                var maxMessageId = dbContext.QUALITY_QM_NOTIFICATIONS.Where(c => c.USER_ID == userId).Select(c => c.MESSAGE_ID).Max();
                QUALITY_QM_NOTIFICATIONS quality_qm_notifications = new QUALITY_QM_NOTIFICATIONS();
                quality_qm_notifications.USER_ID = userId;
                quality_qm_notifications.MESSAGE_ID = (maxMessageId + 1);
                quality_qm_notifications.MESSAGE_TEXT = message;
                quality_qm_notifications.MESSAGE_TYPE = "S";
                quality_qm_notifications.IS_DOWNLOAD = false;
                quality_qm_notifications.TIME_STAMP = DateTime.Now;
                quality_qm_notifications.USERID = PMGSYSession.Current.UserId;
                quality_qm_notifications.IPADD = ipAdd;

                dbContext.QUALITY_QM_NOTIFICATIONS.Add(quality_qm_notifications);
                return dbContext.SaveChanges();
            }
            catch (Exception)
            {
                return -1;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        /// <summary>
        /// Validate Is File type as PDF
        /// Validate as per the header information & Content information
        /// </summary>
        /// <param name="basePath"> Path for Storing file temporarily </param>
        /// <param name="request"> Current request to get file</param>
        /// <returns></returns>
        public bool ValidateIsPdf(string basePath, HttpRequestBase request)
        {
            try
            {
                HttpPostedFileBase tempFile = request.Files[0];
                tempFile.SaveAs(Path.Combine(basePath, tempFile.FileName));
                FileInfo fileinfo = new FileInfo(Path.Combine(basePath, tempFile.FileName));
                // returns true if the file is PDF
                if (fileinfo.IsPdf())
                {
                    System.IO.File.Delete(Path.Combine(basePath, tempFile.FileName));
                    return true;
                }
                else
                {
                    System.IO.File.Delete(Path.Combine(basePath, tempFile.FileName));
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public bool ValidateIsPdfNew(string basePath, HttpPostedFileBase file)
        {
            try
            {
                HttpPostedFileBase tempFile = file;
                tempFile.SaveAs(Path.Combine(basePath, tempFile.FileName));
                FileInfo fileinfo = new FileInfo(Path.Combine(basePath, tempFile.FileName));
                // returns true if the file is PDF
                if (fileinfo.IsPdf())
                {
                    System.IO.File.Delete(Path.Combine(basePath, tempFile.FileName));
                    return true;
                }
                else
                {
                    System.IO.File.Delete(Path.Combine(basePath, tempFile.FileName));
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        public bool ValidateIsExcel(string basePath, HttpPostedFileBase request, string fileType)
        {
            string ext;
            try
            {
                HttpPostedFileBase tempFile = request;
                tempFile.SaveAs(Path.Combine(basePath, tempFile.FileName));
                FileInfo fileinfo = new FileInfo(Path.Combine(basePath, tempFile.FileName));
                ext = fileinfo.Extension;
                // returns true if the file is Excel
                //if (fileinfo.IsExcel())
                if (ext == fileType)
                {
                    System.IO.File.Delete(Path.Combine(basePath, tempFile.FileName));
                    return true;
                }
                else
                {
                    System.IO.File.Delete(Path.Combine(basePath, tempFile.FileName));
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public bool ValidateIsWord(string basePath, HttpRequestBase request)
        {
            try
            {
                HttpPostedFileBase tempFile = request.Files[0];
                tempFile.SaveAs(Path.Combine(basePath, tempFile.FileName));
                FileInfo fileinfo = new FileInfo(Path.Combine(basePath, tempFile.FileName));
                // returns true if the file is DOC
                if (fileinfo.IsWord())
                {
                    System.IO.File.Delete(Path.Combine(basePath, tempFile.FileName));
                    return true;
                }
                else
                {
                    System.IO.File.Delete(Path.Combine(basePath, tempFile.FileName));
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        public bool ValidateIsWordNew(string basePath, HttpPostedFileBase file)
        {
            try
            {
                HttpPostedFileBase tempFile = file;
                tempFile.SaveAs(Path.Combine(basePath, tempFile.FileName));
                FileInfo fileinfo = new FileInfo(Path.Combine(basePath, tempFile.FileName));
                string ext = tempFile.FileName.Substring(tempFile.FileName.IndexOf(".") + 1).ToLower();
                // returns true if the file is DOC
                // if (fileinfo.IsWord())
                if (ext == "doc" || ext == "docx")
                {
                    System.IO.File.Delete(Path.Combine(basePath, tempFile.FileName));
                    return true;
                }
                else
                {
                    System.IO.File.Delete(Path.Combine(basePath, tempFile.FileName));
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public bool ValidateFileLength(string basePath, HttpRequestBase request, long maxFileSize)
        {
            try
            {
                HttpPostedFileBase tempFile = request.Files[0];
                tempFile.SaveAs(Path.Combine(basePath, tempFile.FileName));
                FileInfo fileinfo = new FileInfo(Path.Combine(basePath, tempFile.FileName));
                // returns true if the file is DOC


                long FileSize = fileinfo.Length;

                if (fileinfo.Length < maxFileSize)
                {
                    System.IO.File.Delete(Path.Combine(basePath, tempFile.FileName));
                    return true;
                }
                else
                {
                    System.IO.File.Delete(Path.Combine(basePath, tempFile.FileName));
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public bool ValidateIsKml(string basePath, HttpRequestBase request)
        {
            try
            {
                HttpPostedFileBase tempFile = request.Files[0];
                tempFile.SaveAs(Path.Combine(basePath, tempFile.FileName));
                FileInfo fileinfo = new FileInfo(Path.Combine(basePath, tempFile.FileName));
                // returns true if the file is PDF
                if (fileinfo.IsPdf() || fileinfo.IsExcel() || fileinfo.IsExe() || fileinfo.IsGif() || fileinfo.IsJpeg() || fileinfo.IsMsi() || fileinfo.IsPng() || fileinfo.IsPpt() || fileinfo.IsRar() || fileinfo.IsRtf() || fileinfo.IsWord() || fileinfo.IsZip())
                {
                    System.IO.File.Delete(Path.Combine(basePath, tempFile.FileName));
                    return false;
                }
                else
                {
                    System.IO.File.Delete(Path.Combine(basePath, tempFile.FileName));
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Validate Is File type as Jpeg/png/gif
        /// </summary>
        /// <param name="basePath"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public bool IsValidImageFile(string basePath, HttpRequestBase request, string[] arrTypesToValidate)
        {
            try
            {
                HttpPostedFileBase tempFile = request.Files[0];
                tempFile.SaveAs(Path.Combine(basePath, tempFile.FileName));
                FileInfo fileinfo = new FileInfo(Path.Combine(basePath, tempFile.FileName));
                // returns true if the file is jpeg
                if (arrTypesToValidate[0] != null && arrTypesToValidate[0] != string.Empty && arrTypesToValidate[0].Equals("jpeg"))
                {
                    if (fileinfo.IsJpeg())
                    {
                        System.IO.File.Delete(Path.Combine(basePath, tempFile.FileName));
                        return true;
                    }
                    else
                    {
                        System.IO.File.Delete(Path.Combine(basePath, tempFile.FileName));
                        return false;
                    }
                }

                if (arrTypesToValidate[1] != null && arrTypesToValidate[1] != string.Empty && arrTypesToValidate[1].Equals("png"))
                {
                    if (fileinfo.IsPng())
                    {
                        System.IO.File.Delete(Path.Combine(basePath, tempFile.FileName));
                        return true;
                    }
                    else
                    {
                        System.IO.File.Delete(Path.Combine(basePath, tempFile.FileName));
                        return false;
                    }
                }

                if (arrTypesToValidate[2] != null && arrTypesToValidate[2] != string.Empty && arrTypesToValidate[2].Equals("gif"))
                {
                    if (fileinfo.IsGif())
                    {
                        System.IO.File.Delete(Path.Combine(basePath, tempFile.FileName));
                        return true;
                    }
                    else
                    {
                        System.IO.File.Delete(Path.Combine(basePath, tempFile.FileName));
                        return false;
                    }
                }

                return false;

            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        ///Populate Route List
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> PopulateRoute()
        {
            List<SelectListItem> lstRoute = new List<SelectListItem>();
            SelectListItem item = new SelectListItem();

            item = new SelectListItem();
            item.Text = "All";
            item.Value = "0";
            lstRoute.Add(item);

            item = new SelectListItem();
            item.Text = "Link";
            item.Value = "L";
            lstRoute.Add(item);

            item = new SelectListItem();
            item.Text = "Through";
            item.Value = "T";
            lstRoute.Add(item);

            return lstRoute;
        }

        /// <summary>
        /// Populate Population list
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> PopulatePopulation()
        {
            List<SelectListItem> lstRoute = new List<SelectListItem>();
            SelectListItem item = new SelectListItem();

            item = new SelectListItem();
            item.Text = "All";
            item.Value = "0";
            lstRoute.Add(item);

            item = new SelectListItem();
            item.Text = "<250";
            item.Value = "4";
            lstRoute.Add(item);

            item = new SelectListItem();
            item.Text = "250-499";
            item.Value = "3";
            lstRoute.Add(item);

            item = new SelectListItem();
            item.Text = "499-999";
            item.Value = "2";
            lstRoute.Add(item);

            item = new SelectListItem();
            item.Text = "1000+";
            item.Value = "1";
            lstRoute.Add(item);

            return lstRoute;
        }

        /// <summary>
        /// Populate Road Category list
        /// </summary>
        /// <param name="isAllSelected"></param>
        /// <returns></returns>
        public List<SelectListItem> PopulateRoadCat(bool isAllSelected = false)
        {
            List<SelectListItem> batchList = new List<SelectListItem>();
            SelectListItem item;

            try
            {
                dbContext = new PMGSYEntities();
                var query = (from c in dbContext.MASTER_ROAD_CATEGORY
                             select new
                             {
                                 Text = c.MAST_ROAD_CAT_NAME,
                                 Value = c.MAST_ROAD_CAT_CODE
                             }).OrderBy(c => c.Text).ToList();

                foreach (var data in query)
                {
                    item = new SelectListItem();
                    item.Text = data.Text;
                    item.Value = data.Value.ToString();
                    batchList.Add(item);
                }

                if (isAllSelected == false)
                {
                    batchList.Insert(0, (new SelectListItem { Text = "All", Value = "-1", Selected = true }));
                }
                else if (isAllSelected == true)
                {
                    batchList.Insert(0, (new SelectListItem { Text = "All", Value = "0", Selected = true }));
                }

                return batchList;
            }
            catch
            {
                return batchList;
            }
            finally
            {
                dbContext.Dispose();
            }

        }

        /// <summary>
        /// Populates the Status of Propsoals 
        /// </summary>
        /// <param name="RollID"></param>
        /// <returns></returns>
        public List<SelectListItem> PopulateProposalStatus()
        {
            List<SelectListItem> lstProposalStatus = new List<SelectListItem>();
            SelectListItem item = new SelectListItem();



            item = new SelectListItem();
            item.Text = "Pending Proposals";
            item.Value = "N";
            lstProposalStatus.Add(item);

            item = new SelectListItem();
            item.Text = "Sanctioned Proposals";
            item.Value = "Y";
            lstProposalStatus.Add(item);

            item = new SelectListItem();
            item.Text = "Un-Sanctioned Proposals";
            item.Value = "U";
            lstProposalStatus.Add(item);

            item = new SelectListItem();
            item.Text = "Recommended Proposals";
            item.Value = "R";
            lstProposalStatus.Add(item);

            item = new SelectListItem();
            item.Text = "Droped Propsoal";
            item.Value = "D";
            lstProposalStatus.Add(item);

            item = new SelectListItem();
            item.Text = "All";
            item.Value = "A";
            item.Selected = true;
            lstProposalStatus.Add(item);



            return lstProposalStatus;
        }

        /// <summary>
        /// Populates all Batches
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> PopulateBatch(bool isAllSelected = false)
        {
            List<SelectListItem> batchList = new List<SelectListItem>();
            SelectListItem item;

            try
            {
                dbContext = new PMGSYEntities();
                var query = (from c in dbContext.MASTER_BATCH
                             select new
                             {
                                 Text = c.MAST_BATCH_NAME,
                                 Value = c.MAST_BATCH_CODE
                             }).ToList().Distinct();

                foreach (var data in query)
                {
                    item = new SelectListItem();
                    item.Text = data.Text;
                    item.Value = data.Value.ToString();
                    batchList.Add(item);
                }

                if (isAllSelected == false)
                {
                    batchList.Insert(0, (new SelectListItem { Text = "Select Batch", Value = "-1", Selected = true }));
                }
                else if (isAllSelected == true)
                {
                    batchList.Insert(0, (new SelectListItem { Text = "All Batches", Value = "0", Selected = true }));
                }

                return batchList;
            }
            catch
            {
                return batchList;
            }
            finally
            {
                dbContext.Dispose();
            }

        }

        /// <summary>
        /// Populates all Batches
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> PopulateNewUpgradeList(bool isAllSelected = false)
        {
            List<SelectListItem> newUpgradeList = new List<SelectListItem>();
            try
            {
                if (isAllSelected == true)
                {
                    newUpgradeList.Insert(0, new SelectListItem { Value = "0", Text = "All" });
                }
                else
                {
                    newUpgradeList.Insert(0, new SelectListItem { Value = "0", Text = "Select" });
                }
                newUpgradeList.Insert(1, new SelectListItem { Value = "N", Text = "New" });
                newUpgradeList.Insert(2, new SelectListItem { Value = "U", Text = "Upgradation" });
                return newUpgradeList;
            }
            catch
            {
                return newUpgradeList;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public List<SelectListItem> PopulateMaintenanceType()
        {
            List<SelectListItem> DlpList = new List<SelectListItem>();
            DlpList.Insert(0, new SelectListItem { Value = "-1", Text = "Select" });
            DlpList.Insert(1, new SelectListItem { Value = "1", Text = "DLP" });
            DlpList.Insert(2, new SelectListItem { Value = "2", Text = "POST DLP" });

            return DlpList;
        }

        public List<SelectListItem> PopulateSoilTypes(bool isAllSelected = false)
        {
            dbContext = new PMGSYEntities();
            List<SelectListItem> soilTypeList = new List<SelectListItem>();
            SelectListItem item;
            try
            {
                var query = (from c in dbContext.MASTER_SOIL_TYPE
                             select new
                             {
                                 Text = c.MAST_SOIL_TYPE_NAME,
                                 Value = c.MAST_SOIL_TYPE_CODE
                             }).ToList().Distinct();

                foreach (var data in query)
                {
                    item = new SelectListItem();
                    item.Text = data.Text;
                    item.Value = data.Value.ToString();
                    soilTypeList.Add(item);
                }

                if (isAllSelected == true)
                {
                    soilTypeList.Insert(0, new SelectListItem { Value = "0", Text = "All Types" });
                }
                else
                {
                    soilTypeList.Insert(0, new SelectListItem { Value = "0", Text = "Select Type" });
                }

                return soilTypeList;
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

        public List<SelectListItem> PopulateTerrainTypes(bool isAllSelected = false)
        {
            dbContext = new PMGSYEntities();
            List<SelectListItem> terrainTypeList = new List<SelectListItem>();
            SelectListItem item;
            try
            {
                var query = (from c in dbContext.MASTER_TERRAIN_TYPE
                             select new
                             {
                                 Text = c.MAST_TERRAIN_TYPE_NAME,
                                 Value = c.MAST_TERRAIN_TYPE_CODE
                             }).ToList().Distinct();

                foreach (var data in query)
                {
                    item = new SelectListItem();
                    item.Text = data.Text;
                    item.Value = data.Value.ToString();
                    terrainTypeList.Add(item);
                }

                if (isAllSelected == true)
                {
                    terrainTypeList.Insert(0, new SelectListItem { Value = "0", Text = "All Types" });
                }
                else
                {
                    terrainTypeList.Insert(0, new SelectListItem { Value = "0", Text = "Select Type" });
                }

                return terrainTypeList;
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

        public List<SelectListItem> PopulateMasterAgency(bool isAllSelected = false)
        {
            dbContext = new PMGSYEntities();
            List<SelectListItem> AgencyList = new List<SelectListItem>();
            SelectListItem item;
            try
            {
                var query = (from c in dbContext.MASTER_AGENCY
                             select new
                             {
                                 Text = c.MAST_AGENCY_NAME,
                                 Value = c.MAST_AGENCY_CODE
                             }).ToList().Distinct();

                foreach (var data in query)
                {
                    item = new SelectListItem();
                    item.Text = data.Text;
                    item.Value = data.Value.ToString();
                    AgencyList.Add(item);
                }

                if (isAllSelected == true)
                {
                    AgencyList.Insert(0, new SelectListItem { Value = "0", Text = "All Agency" });
                }
                else
                {
                    AgencyList.Insert(0, new SelectListItem { Value = "0", Text = "Select Agency" });
                }

                return AgencyList;
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


        public List<SelectListItem> PopulateDRRPWhetherTypes(bool isAllSelected = false)
        {
            List<SelectListItem> whetherTypeList = new List<SelectListItem>();
            try
            {
                whetherTypeList.Insert(0, new SelectListItem { Value = "A", Text = "All Weather" });
                whetherTypeList.Insert(1, new SelectListItem { Value = "F", Text = "Fair Weather" });
                return whetherTypeList;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// for population of grading in Quality Monitoring
        /// </summary>
        /// <param name="isAllSelected"></param>
        /// <returns></returns>
        public List<SelectListItem> PopulateQualityGrading(bool isAllSelected = false)
        {
            dbContext = new PMGSYEntities();
            try
            {
                var lstGrades = dbContext.MASTER_GRADE_TYPE.ToList();

                List<SelectListItem> lstQualityGrading = new SelectList(lstGrades.ToList(), "MAST_GRADE_CODE", "MAST_GRADE_NAME").ToList();

                lstQualityGrading.RemoveAt(3);

                if (isAllSelected == true)
                {
                    lstQualityGrading.Insert(0, new SelectListItem { Value = "0", Text = "All" });
                }
                else
                {
                    lstQualityGrading.Insert(0, new SelectListItem { Value = "0", Text = "Select Grade" });
                }

                return lstQualityGrading;
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

        public List<SelectListItem> PopulateCurrNextMonths(int month, bool populateFirstItem)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<SelectListItem> lstMonths = new SelectList(dbContext.MASTER_MONTH.Where(x => x.MAST_MONTH_CODE >= month), "MAST_MONTH_CODE", "MAST_MONTH_FULL_NAME", month).ToList();
                if (populateFirstItem == true)
                {
                    lstMonths.Insert(0, (new SelectListItem { Text = "Select Month", Value = "-1" }));
                }
                return lstMonths;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CommonFunctions.PopulateCurrNextMonths(int month)");
                throw ex;
            }
            finally
            {
                if (dbContext == null)
                {
                    dbContext.Dispose();
                }
            }
        }

        public List<SelectListItem> PopulateCurrYear(int year, bool populateFirstItem)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<SelectListItem> lstYears = new List<SelectListItem>();

                
                    lstYears = new SelectList(dbContext.MASTER_YEAR.Where(x => x.MAST_YEAR_CODE == year), "MAST_YEAR_CODE", "MAST_YEAR_CODE", year).ToList();
                
                
                if (populateFirstItem == true)
                {
                    lstYears.Insert(0, (new SelectListItem { Text = "Select Month", Value = "0" }));
                }

                return lstYears;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CommonFunctions.PopulateCurrYear(int year)");
                throw ex;
            }
            finally
            {
                if (dbContext == null)
                {
                    dbContext.Dispose();
                }
            }
        }

        /*
            SSRS Report Settings Starts Here
         */
        public class CustomReportCredentials : Microsoft.Reporting.WebForms.IReportServerCredentials
        {

            // local variable for network credential.
            private string _UserName;
            private string _PassWord;
            //private string _DomainName;
            public CustomReportCredentials(string UserName, string PassWord)//, string DomainName)
            {
                _UserName = UserName;
                _PassWord = PassWord;
                //_DomainName = DomainName;
            }
            public System.Security.Principal.WindowsIdentity ImpersonationUser
            {
                get
                {
                    return null;  // not use ImpersonationUser
                }
            }
            public ICredentials NetworkCredentials
            {
                get
                {

                    // use NetworkCredentials
                    return new NetworkCredential(_UserName, _PassWord);//, _DomainName);
                }
            }
            public bool GetFormsCredentials(out Cookie authCookie, out string user, out string password, out string authority)
            {

                // not use FormsCredentials unless you have implements a custom autentication.
                authCookie = null;
                user = password = authority = null;
                return false;
            }

            /*
               SSRS Report Settings Ends Here
            */
        }

        /// <summary>
        /// Populate Years
        /// </summary>
        /// <param name="year"></param>
        /// <param name="isAllYear"></param>
        /// <returns></returns>
        public List<SelectListItem> PopulateYears(int year, bool isAllYear = false)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<SelectListItem> lstYears = new SelectList(dbContext.MASTER_YEAR.Where(m => m.MAST_YEAR_CODE < (DateTime.Now.Year + 1)).OrderByDescending(m => m.MAST_YEAR_CODE).ToList(), "MAST_YEAR_CODE", "MAST_YEAR_CODE", year).ToList();

                if (isAllYear)
                {
                    lstYears.Insert(0, (new SelectListItem { Text = "All Year", Value = "0" }));
                }
                else
                {
                    lstYears.Insert(0, (new SelectListItem { Text = "Select Year", Value = "0" }));
                }
                return lstYears;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        /// <summary>
        /// Populate Months
        /// </summary>
        /// <param name="month"></param>
        /// <param name="isAllMonth"></param>
        /// <returns></returns>
        public List<SelectListItem> PopulateMonths(int month, bool isAllMonth = false)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<SelectListItem> lstMonths = new SelectList(dbContext.MASTER_MONTH, "MAST_MONTH_CODE", "MAST_MONTH_FULL_NAME", month).ToList();
                if (isAllMonth)
                {
                    lstMonths.Insert(0, (new SelectListItem { Text = "All Month", Value = "0" }));
                }
                else
                {
                    lstMonths.Insert(0, (new SelectListItem { Text = "Select Month", Value = "0" }));
                }
                return lstMonths;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        public List<SelectListItem> PopulateDistrictforTOB(Int32 StateCode, bool isAllSelected = false, Int32 selectedDistrictCode = 0, bool IsPopulateInactiveDistrictsForTOB = false, bool IsPopulateAllActiveInactiveDistricts = false)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<SelectListItem> lstDistrict = null;

                if (IsPopulateAllActiveInactiveDistricts)//Added by Abhishek kamble 5Jan2015 for head 21.01 for asset and lib for state AP 
                {
                    if (StateCode == 2)
                    {
                        lstDistrict = new SelectList(dbContext.MASTER_DISTRICT.Where(m => m.MAST_STATE_CODE == StateCode || m.MAST_STATE_CODE == 36).OrderBy(s => s.MAST_DISTRICT_NAME), "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME", selectedDistrictCode).ToList();
                    }
                    else
                    {
                        lstDistrict = new SelectList(dbContext.MASTER_DISTRICT.Where(m => m.MAST_STATE_CODE == StateCode).OrderBy(s => s.MAST_DISTRICT_NAME), "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME", selectedDistrictCode).ToList();
                    }
                }
                else
                {
                    if (IsPopulateInactiveDistrictsForTOB)
                    {
                        if (StateCode == 2)
                        {
                            lstDistrict = new SelectList(dbContext.MASTER_DISTRICT.Where(m => (m.MAST_STATE_CODE == StateCode || m.MAST_STATE_CODE == 36) && m.MAST_DISTRICT_ACTIVE == "N").OrderBy(s => s.MAST_DISTRICT_NAME), "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME", selectedDistrictCode).ToList();

                        }
                        else
                        {
                            lstDistrict = new SelectList(dbContext.MASTER_DISTRICT.Where(m => m.MAST_STATE_CODE == StateCode && m.MAST_DISTRICT_ACTIVE == "N").OrderBy(s => s.MAST_DISTRICT_NAME), "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME", selectedDistrictCode).ToList();
                        }
                    }
                    else
                    {
                        if (StateCode == 2)
                        {
                            lstDistrict = new SelectList(dbContext.MASTER_DISTRICT.Where(m => m.MAST_STATE_CODE == StateCode).OrderBy(s => s.MAST_DISTRICT_NAME), "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME", selectedDistrictCode).ToList();
                        }
                        else
                        {
                            lstDistrict = new SelectList(dbContext.MASTER_DISTRICT.Where(m => m.MAST_STATE_CODE == StateCode && m.MAST_DISTRICT_ACTIVE == "Y").OrderBy(s => s.MAST_DISTRICT_NAME), "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME", selectedDistrictCode).ToList();
                        }

                    }

                }
                if (isAllSelected == false)
                {
                    lstDistrict.Insert(0, (new SelectListItem { Text = "Select District", Value = "0", Selected = true }));
                }
                else if (isAllSelected == true)
                {
                    lstDistrict.Insert(0, (new SelectListItem { Text = "All District", Value = "-1", Selected = true }));
                }
                return lstDistrict;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public string GetPaymentReceiptNumber(int AdminNdCode, string FundType, string TxnType, int month, int year)
        {
            dbContext = new PMGSYEntities();

            string status = String.Empty;
            SqlConnection storeConnection = new SqlConnection(dbContext.Database.Connection.ConnectionString);
            using (SqlCommand command = storeConnection.CreateCommand())
            {
                command.Connection = storeConnection;
                storeConnection.Open();
                command.CommandText = "omms.USP_ACC_GENERATE_VOUCHER_NUMBER";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@AdminNDCode", SqlDbType.Int)).Value = AdminNdCode;
                command.Parameters.Add(new SqlParameter("@FundType", SqlDbType.Char)).Value = FundType;
                command.Parameters.Add(new SqlParameter("@BillType", SqlDbType.Char)).Value = TxnType;
                command.Parameters.Add(new SqlParameter("@BillMonth", SqlDbType.Int)).Value = month;
                command.Parameters.Add(new SqlParameter("@BillYear", SqlDbType.Int)).Value = year;

                status = command.ExecuteScalar().ToString();
                storeConnection.Close();


            }

            return status;
            //    scope.Complete();      



        }

        #region Quality Complain

        public List<SelectListItem> PopulateQMComplainItem(int parrentId)
        {
            List<SelectListItem> ComplainantList = new List<SelectListItem>();
            try
            {

                dbContext = new PMGSYEntities();
                var query = (from c in dbContext.QUALITY_QM_COMPLAIN_ITEM.AsEnumerable()
                             where
                             c.ParrentId == parrentId && c.IsActive == "Y"
                             select new SelectListItem
                             {
                                 Text = c.Title,
                                 Value = c.ItemId.ToString()
                             }).ToList<SelectListItem>();
                ComplainantList = query;
                ComplainantList.Insert(0, new SelectListItem { Text = "--Select--", Value = "0", Selected = true });

                /* foreach (var item in query)
                 {

                     ComplainantList.Add(new SelectListItem { Text = item.Text, Value = item.Value.ToString() });
                 }*/
                return ComplainantList;
            }
            catch (Exception ex)
            {
                return ComplainantList;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        #endregion

        #region// 28 Aug 2017 added by Rohit
        public List<SelectListItem> PopulateAllNodalAgencies()
        {
            List<SelectListItem> lstNodalAgency = new List<SelectListItem>();
            dbContext = new PMGSYEntities();
            try
            {

                var lstData = (from item in dbContext.ADMIN_DEPARTMENT
                               where item.MAST_ND_TYPE == "S"
                               select new
                               {
                                   ADMIN_NAME = item.MASTER_STATE.MAST_STATE_NAME + " (" + item.MASTER_AGENCY.MAST_AGENCY_NAME + ")",
                                   ADMIN_CODE = item.ADMIN_ND_CODE
                               }).OrderBy(m => m.ADMIN_NAME).ToList().Distinct();


                foreach (var item in lstData)
                {
                    lstNodalAgency.Add(new SelectListItem { Text = item.ADMIN_NAME, Value = item.ADMIN_CODE.ToString() });
                }
                lstNodalAgency.Insert(0, (new SelectListItem { Text = "--All--", Value = "0", Selected = true }));
                return lstNodalAgency;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CommonFunctions.PopulateAllNodalAgencies()");
                return lstNodalAgency;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        public List<SelectListItem> PopulateAllDPIUOfSRRDA(int SRRDACode)
        {
            List<SelectListItem> lstDPIU = null;
            try
            {
                dbContext = new PMGSYEntities();
                lstDPIU = new SelectList(dbContext.ADMIN_DEPARTMENT.Where(m => m.MAST_PARENT_ND_CODE == SRRDACode && m.MAST_ND_TYPE == "D"), "ADMIN_ND_CODE", "ADMIN_ND_NAME").ToList();

                if (lstDPIU == null || lstDPIU.Count() == 0)
                {
                    lstDPIU.Insert(0, (new SelectListItem { Text = "All DPIU", Value = "0", Selected = true }));
                }

                return lstDPIU;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CommonFunctions.PopulateAllDPIUOfSRRDA(int SRRDACode)");
                return lstDPIU;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        #endregion//

        #region ANNEXURE

        public List<SelectListItem> PopulateScheme()
        {
            List<SelectListItem> populateScheme = new List<SelectListItem>();
            try
            {
                populateScheme.Insert(0, (new SelectListItem { Text = "All Schemes", Value = "0", Selected = true }));
                populateScheme.Insert(1, (new SelectListItem { Text = "PMGSY 1", Value = "1" }));
                populateScheme.Insert(2, (new SelectListItem { Text = "PMGSY 2", Value = "2" }));
                populateScheme.Insert(3, (new SelectListItem { Text = "RCPLWE", Value = "3" }));
                populateScheme.Insert(4, (new SelectListItem { Text = "PMGSY 3", Value = "4" }));
                return populateScheme;
            }
            catch
            {
                return populateScheme;
            }
        }
        #endregion

        #region PFMS
        /// <summary>
        /// Populate PFMS Bank Names
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> PopulatePFMSBankNames()
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<SelectListItem> lstBank = new SelectList(dbContext.PFMS_BANK_MASTER.OrderBy(x => x.PFMS_BANK_NAME).ToList(), "PFMS_BANK_NAME", "PFMS_BANK_NAME").ToList();

                lstBank.Insert(0, (new SelectListItem { Text = "Select Bank", Value = "" }));

                return lstBank;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CommonFunctions.PopulatePFMSBankNames()");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// Populate PFMS Bank Names
        /// </summary>
        /// <param name="bankName"></param>
        /// <returns></returns>
        public List<SelectListItem> PopulatePFMSIfscByBank(string bankName)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<SelectListItem> lstIfscCodes = new SelectList(dbContext.PFMS_BANK_BRANCHMASTER.Where(x => x.BankName == bankName.Trim()).ToList(), "IFSCCode", "IFSCCode").ToList();

                lstIfscCodes.Insert(0, (new SelectListItem { Text = "Select Ifsc Code", Value = "" }));

                return lstIfscCodes;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CommonFunctions.PopulatePFMSBankNames()");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion

        #region Building Progress
        public List<SelectListItem> PopulateItemProgress()
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<SelectListItem> lstItemProgress = new List<SelectListItem>();

                lstItemProgress.Insert(0, new SelectListItem() { Text = "No", Value = "N" });
                lstItemProgress.Insert(1, new SelectListItem() { Text = "Yes", Value = "Y" });
                lstItemProgress.Insert(2, new SelectListItem() { Text = "Not Applicable", Value = "NA" });

                return lstItemProgress;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CommonFunctions.PopulateItemProgress()");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public List<SelectListItem> PopulateFoundationSubcomponent()
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<SelectListItem> lstSubcomponent = new SelectList(dbContext.MASTER_BUILDING_EXECUTION_ITEMS.Where(c => c.EXECUTION_ITEM.Trim() == "Foundation").OrderBy(x => x.ITEM_CODE).ToList(), "ITEM_CODE", "ITEM_SUB_COMPONENT").ToList();

                //lstSubcomponent.Insert(0, (new SelectListItem { Text = "Select Bank", Value = "" }));

                return lstSubcomponent;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CommonFunctions.PopulateFoundationSubcomponent()");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public List<SelectListItem> PopulateSuperstructureSubcomponent()
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<SelectListItem> lstSubcomponent = new SelectList(dbContext.MASTER_BUILDING_EXECUTION_ITEMS.Where(c => c.EXECUTION_ITEM.Trim() == "Superstructure").OrderBy(x => x.ITEM_CODE).ToList(), "ITEM_CODE", "ITEM_SUB_COMPONENT").ToList();

                return lstSubcomponent;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CommonFunctions.PopulateFoundationSubcomponent()");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public List<SelectListItem> PopulateSuperstructureFloor()
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<SelectListItem> lstFloor = new List<SelectListItem>(); //new SelectList(dbContext.MASTER_BUILDING_EXECUTION_ITEMS.Where(c => c.EXECUTION_ITEM.Trim() == "Superstructure").OrderBy(x => x.ITEM_CODE).ToList(), "ITEM_CODE", "ITEM_SUB_COMPONENT").ToList();
                lstFloor.Insert(0, new SelectListItem() { Text = "Ground Floor", Value = "1" });
                lstFloor.Insert(1, new SelectListItem() { Text = "First Floor", Value = "2" });
                lstFloor.Insert(2, new SelectListItem() { Text = "Second Floor", Value = "3" });
                lstFloor.Insert(3, new SelectListItem() { Text = "Third Floor", Value = "4" });
                lstFloor.Insert(4, new SelectListItem() { Text = "Covered Parking", Value = "5" });
                lstFloor.Insert(5, new SelectListItem() { Text = "Approach Road", Value = "6" });

                return lstFloor;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CommonFunctions.PopulateSuperstructureFloor()");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion

        #region DRRP - II PMGSY-I Mapping
        public List<SelectListItem> PopulateDRRPToMapUnderPMGSY3(int BlockID, string IMS_UPGRADE_CONNECT, string IMS_PROPOSAL_TYPE)
        {
            try
            {
                PMGSYEntities dbContext = new PMGSYEntities();
                List<SelectListItem> lstDistrict = new List<SelectListItem>();

                var lstDist = (from Master in dbContext.MASTER_EXISTING_ROADS
                               where Master.MAST_BLOCK_CODE == BlockID
                               && Master.MAST_PMGSY_SCHEME == 2 //Only PMGSY - 2 Scheme Details should be available for mapping 

                               select new
                               {
                                   Master.MAST_ER_ROAD_CODE,
                                   Master.MAST_ER_ROAD_NAME
                               }).OrderBy(d => d.MAST_ER_ROAD_NAME).ToList().Distinct();

                foreach (var item in lstDist)
                {
                    lstDistrict.Add(new SelectListItem { Text = item.MAST_ER_ROAD_NAME + " ( " + item.MAST_ER_ROAD_CODE.ToString() + " ) ", Value = item.MAST_ER_ROAD_CODE.ToString() });
                }
                lstDistrict.Insert(0, (new SelectListItem { Text = "Select DRRP", Value = "0", Selected = true }));

                return lstDistrict;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if(dbContext != null)
                dbContext.Dispose();
            }
        }
        #endregion

        #region PMGSY State
        public List<SelectListItem> PopulateStateDetails(bool isPopulateFirstItem = true)
        {
            List<SelectListItem> StatesList = new List<SelectListItem>();
            SelectListItem item;
            if (isPopulateFirstItem)
            {
                //item = new SelectListItem();
                //item.Text = "Select State";
                //item.Value = "0";
                //item.Selected = true;
                //StatesList.Add(item);
            }
            else
            {
                //item = new SelectListItem();
                //item.Text = "All States";
                //item.Value = "0";
                //item.Selected = true;
                //StatesList.Add(item);
            }

            try
            {
                dbContext = new PMGSYEntities();
                var exceptionList = dbContext.MASTER_PMGSY3.Select(e => new { e.MASTER_STATE.MAST_STATE_NAME, e.MAST_STATE_CODE });
                var query = (from c in dbContext.MASTER_STATE
                             where c.MAST_STATE_ACTIVE == "Y"
                             select new
                             {
                                 c.MAST_STATE_NAME,
                                 c.MAST_STATE_CODE
                             }).Except(exceptionList).OrderBy(c => c.MAST_STATE_CODE).ToList();

                foreach (var data in query)
                {
                    item = new SelectListItem();
                    item.Text = data.MAST_STATE_NAME;
                    item.Value = data.MAST_STATE_CODE.ToString();
                    StatesList.Add(item);
                }
                return StatesList;
            }
            catch
            {
                return StatesList;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        public enum PMGSYSessionRoleDetails
        {
            SRRDA = 2,
            PIU = 22
        }


        #endregion

        #region RSA
        public bool IsValidImageFileForContractor(string basePath, HttpRequestBase request, string[] arrTypesToValidate)
        {
            try
            {
                HttpPostedFileBase tempFile = request.Files[0];
                tempFile.SaveAs(Path.Combine(basePath, tempFile.FileName));
                FileInfo fileinfo = new FileInfo(Path.Combine(basePath, tempFile.FileName));
                // returns true if the file is jpeg

                if (fileinfo.IsPdf())
                {
                    System.IO.File.Delete(Path.Combine(basePath, tempFile.FileName));
                    return true;
                }

                else
                {
                    System.IO.File.Delete(Path.Combine(basePath, tempFile.FileName));
                    return false;
                }

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Common.IsValidImageFileForContractor");
                return false;
            }
        }

        public List<SelectListItem> PopulateIssueInRSA()
        {
            try
            {
                dbContext = new PMGSYEntities();

                List<SelectListItem> lstSubcomponent = new List<SelectListItem>();
                SelectListItem item;
                //List<SelectListItem> lstSubcomponent = new SelectList(dbContext.RCTRC_CONTACT_DETAILS.OrderBy(x => x.RCTRC_Contact_Id).ToList(), "RCTRC_Contact_Id", "RCTRC_Contact_Name").ToList();
                var query = (from c in dbContext.RSA_MASTER_ISSUE
                             where c.ISSUE_IS_ACTIVE == "Y"
                             select new
                             {
                                 Value = c.ISSUE_CODE,
                                 Text = (c.ISSUE_SHORT_DESC + " ( Severity : " + c.ISSUE_SEVERITY + " )")
                             }).OrderBy(c => c.Value).ToList();

                foreach (var data in query)
                {
                    item = new SelectListItem();
                    item.Text = data.Text;
                    item.Value = data.Value.ToString();
                    lstSubcomponent.Add(item);
                }


                lstSubcomponent.Insert(0, (new SelectListItem { Text = "--Select Issue Details--", Value = "-1", Selected = true }));
                return lstSubcomponent;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CommonFunctions.PopulateIssueInRSA()");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion 

        #region New PCI
        public List<SelectListItem> PopulateBlocksNew(int MAST_STATE_CODE, int MAST_DISTRICT_CODE, bool isAllBlocksSelected = false)
        {

            List<SelectListItem> BlockList = new List<SelectListItem>();
            SelectListItem item;
            if (!isAllBlocksSelected)
            {
                item = new SelectListItem();
                item.Text = "Select Block";
                item.Value = "0";
                item.Selected = true;
                BlockList.Add(item);
            }
            else
            {
                item = new SelectListItem();
                item.Text = "All Blocks";
                item.Value = "-1";
                item.Selected = true;
                BlockList.Add(item);
            }
            try
            {
                dbContext = new PMGSYEntities();
                var query = dbContext.USP_BLOCK_CUPL_PMGSY3_NOT_ELIGIBILITY(MAST_STATE_CODE, MAST_DISTRICT_CODE, 0, 0, 0, 0, "%").ToList();

                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "Common.PopulateBlocksNew()");
                    foreach (var data in query)
                    {
                        item = new SelectListItem();
                        item.Text = data.MAST_BLOCK_NAME;
                        item.Value = data.MAST_BLOCK_CODE.ToString();
                        sw.WriteLine("STATE_CODE : " + MAST_STATE_CODE.ToString() + " DIST_CODE : " + MAST_DISTRICT_CODE.ToString() + " MAST_BLOCK_CODE : " + data.MAST_BLOCK_CODE.ToString() + " MAST_BLOCK_NAME : " + data.MAST_BLOCK_NAME.ToString());
                        sw.WriteLine();
                    }
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }

                foreach (var data in query)
                {
                    item = new SelectListItem();
                    item.Text = data.MAST_BLOCK_NAME;
                    item.Value = data.MAST_BLOCK_CODE.ToString();
                    BlockList.Add(item);
                }
                return BlockList;
            }
            catch
            {
                return BlockList;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion

        #region PMGSY III  Non Feasible Roads
        public MapNotFeasibleRoads GetDetails(int PlanCNRoadCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                MapNotFeasibleRoads model = new MapNotFeasibleRoads();
                CUPL_PMGSY3 imsMaster = dbContext.CUPL_PMGSY3.Where(m => m.PLAN_CN_ROAD_CODE == PlanCNRoadCode).FirstOrDefault();
                model.WorkName = imsMaster.PLAN_RD_NAME;
                model.PackageName = imsMaster.PLAN_CN_ROAD_NUMBER;
                model.PLAN_CN_ROAD_CODE = PlanCNRoadCode;
                model.Block = imsMaster.MAST_BLOCK_CODE;
                model.District = imsMaster.MAST_DISTRICT_CODE;
                return model;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CommonFunctions.GetDetails()");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        public List<SelectListItem> PopulateReaonsPMGSY3()
        {
            try
            {
                PMGSYEntities dbContext = new PMGSYEntities();
                List<SelectListItem> lstDistrict = new List<SelectListItem>();

                var lstDist = (from Master in dbContext.MASTER_PMGSY3_NON_INCLUSION_REASON
                               where Master.MAST_PMGSY3_IS_VALID == "Y"
                               //  && Master.MAST_PMGSY_SCHEME == 2 //Only PMGSY - 2 Scheme Details should be available for mapping 

                               select new
                               {
                                   Master.MAST_PMGSY3_REASON_CODE,
                                   Master.MAST_PMGSY3_REASON
                               }).OrderBy(d => d.MAST_PMGSY3_REASON_CODE).ToList().Distinct();

                foreach (var item in lstDist)
                {
                    lstDistrict.Add(new SelectListItem { Text = item.MAST_PMGSY3_REASON, Value = item.MAST_PMGSY3_REASON_CODE.ToString() });
                }
                lstDistrict.Insert(0, (new SelectListItem { Text = "Select Reason", Value = "0", Selected = true }));

                return lstDistrict;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CommonFunctions.PopulateReaonsPMGSY3()");
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion 


        #region ER Shift
        public List<SelectListItem> PopulateInactiveBlocks(int MAST_DISTRICT_CODE, bool isAllBlocksSelected = false)
        {

            List<SelectListItem> BlockList = new List<SelectListItem>();
            SelectListItem item;
            if (!isAllBlocksSelected)
            {
                item = new SelectListItem();
                item.Text = "Select Block";
                item.Value = "0";
                item.Selected = true;
                BlockList.Add(item);
            }
            else
            {
                item = new SelectListItem();
                item.Text = "All Blocks";
                item.Value = "-1";
                item.Selected = true;
                BlockList.Add(item);
            }
            try
            {
                dbContext = new PMGSYEntities();
                var query = (from c in dbContext.MASTER_BLOCK
                             where
                             c.MAST_DISTRICT_CODE == MAST_DISTRICT_CODE // && c.MAST_BLOCK_ACTIVE == "N"  Condition is commented on 16 Dec 2019 As per suggestion By Srinivasa Sir
                             select new
                             {
                                 Text = c.MAST_BLOCK_NAME,
                                 Value = c.MAST_BLOCK_CODE
                             }).ToList();

                foreach (var data in query)
                {
                    item = new SelectListItem();
                    item.Text = data.Text;
                    item.Value = data.Value.ToString();
                    BlockList.Add(item);
                }
                return BlockList;
            }
            catch
            {
                return BlockList;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion 



        #region RCTRC
        public List<SelectListItem> PopulateEducation(string EducationType)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<SelectListItem> lstSubcomponent = new SelectList(dbContext.RCTRC_MASTER_EDUCATION.Where(c => c.RCTRC_EDUCATION_TYPE.Trim() == EducationType && c.RCTRC_EDUCATION_ACTIVE == "Y").OrderBy(x => x.RCTRC_EDUCATION_ID).ToList(), "RCTRC_EDUCATION_ID", "RCTRC_EDUCATION_NAME").ToList();
                lstSubcomponent.Insert(0, (new SelectListItem { Text = "Select Education", Value = "-1", Selected = true }));
                return lstSubcomponent;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CommonFunctions.PopulateEducation()");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public List<SelectListItem> PopulateContactPerson()
        {
            try
            {
                dbContext = new PMGSYEntities();

                List<SelectListItem> lstSubcomponent = new List<SelectListItem>();
                SelectListItem item;
                //List<SelectListItem> lstSubcomponent = new SelectList(dbContext.RCTRC_CONTACT_DETAILS.OrderBy(x => x.RCTRC_Contact_Id).ToList(), "RCTRC_Contact_Id", "RCTRC_Contact_Name").ToList();
                var query = (from c in dbContext.RCTRC_CONTACT_DETAILS
                             where c.USERID == PMGSYSession.Current.UserId
                             select new
                             {
                                 Value = c.RCTRC_Contact_Id,
                                 Text = (c.RCTRC_Contact_Name.Equals(null) ? "" : c.RCTRC_Contact_Name) + " (  " + (c.RCTRC_Contact_eMail.Equals(null) ? "" : c.RCTRC_Contact_eMail) + " ) "
                             }).OrderBy(c => c.Text).ToList();

                foreach (var data in query)
                {
                    item = new SelectListItem();
                    item.Text = data.Text;
                    item.Value = data.Value.ToString();
                    lstSubcomponent.Add(item);
                }


                lstSubcomponent.Insert(0, (new SelectListItem { Text = "Select Contact Person", Value = "-1", Selected = true }));
                return lstSubcomponent;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CommonFunctions.PopulateContactPerson()");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public List<SelectListItem> GetWorkAreaSubdetails(int ParentID)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<SelectListItem> lstSubcomponent = new SelectList(dbContext.RCTRC_MASTER_KEY_AREA.Where(m => m.RCTRC_KEY_AREA_PARENT_ID == ParentID && m.RCTRC_KEY_AREA_ACTIVE == "Y").OrderBy(x => x.RCTRC_KEY_AREA_PARENT_ID).ToList(), "RCTRC_KEY_AREA_ID", "RCTRC_KEY_AREA_NAME").ToList();
                lstSubcomponent.Insert(0, (new SelectListItem { Text = "Select", Value = "-1", Selected = true }));
                return lstSubcomponent;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CommonFunctions.GetWorkAreaSubdetails()");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public List<SelectListItem> GetMasterWorkArea()
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<SelectListItem> lstSubcomponent = new SelectList(dbContext.RCTRC_MASTER_WORK_AREA.Where(x => x.RCTRC_WORK_AREA_ACTIVE == "Y").OrderBy(x => x.RCTRC_WORK_AREA_ID).ToList(), "RCTRC_WORK_AREA_ID", "RCTRC_WORK_AREA_NAME").ToList();
                lstSubcomponent.Insert(0, (new SelectListItem { Text = "-Select Master Work Area-", Value = "-1", Selected = true }));
                return lstSubcomponent;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CommonFunctions.GetMasterWorkArea()");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public List<SelectListItem> GetRCTRCUsersForStateMapping()
        {
            List<SelectListItem> batchList = new List<SelectListItem>();
            SelectListItem item;

            try
            {
                dbContext = new PMGSYEntities();
                var query = (from um in dbContext.UM_User_Master

                             join qm in dbContext.RCTRC_ADMIN_QUALITY_MONITORS on um.UserID equals qm.ADMIN_USER_ID

                             select um).OrderBy(a => a.UserID).ToList();

                foreach (var data in query)
                {
                    item = new SelectListItem();
                    item.Text = data.UserName;
                    item.Value = data.UserID.ToString();
                    batchList.Add(item);
                }

                if (true)
                {
                    batchList.Insert(0, (new SelectListItem { Text = "Select User", Value = "-1", Selected = true }));
                }


                return batchList;


                //dbContext = new PMGSYEntities();

                //var list = (from um in dbContext.UM_User_Master

                //            join qm in dbContext.RCTRC_ADMIN_QUALITY_MONITORS on um.UserID equals qm.ADMIN_USER_ID

                //            select um).OrderBy(a => a.UserID).ToList();



                //return new SelectList(list.ToList(), "UserID", "UserName").ToList();




            }
            catch (Exception ex)
            {
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


        #region Interstate Monitor Mapping
        public List<SelectListItem> PopulateAllMonitorsForInterstate(string isPopulateFirstSelect, string qmType, int stateCode)
        {
            List<SelectListItem> lstProfileNames = new List<SelectListItem>();
            dbContext = new PMGSYEntities();
            try
            {
                SelectListItem item = new SelectListItem();

                if (qmType.Equals("0"))
                {
                    return lstProfileNames;
                }

                if (stateCode == 0)
                {
                    var query = (from c in dbContext.ADMIN_QUALITY_MONITORS
                                 where c.ADMIN_QM_TYPE == qmType
                                 && c.ADMIN_QM_EMPANELLED == "Y"
                                 select new
                                 {
                                     Value = c.ADMIN_QM_CODE,
                                     Text = (c.ADMIN_QM_FNAME.Equals(null) ? "" : c.ADMIN_QM_FNAME) + " " + (c.ADMIN_QM_MNAME.Equals(null) ? "" : c.ADMIN_QM_MNAME) + " " + (c.ADMIN_QM_LNAME.Equals(null) ? "" : c.ADMIN_QM_LNAME)
                                 }).OrderBy(c => c.Text).ToList();

                    foreach (var data in query)
                    {
                        item = new SelectListItem();
                        item.Text = data.Text;
                        item.Value = data.Value.ToString();
                        lstProfileNames.Add(item);
                    }
                }
                else
                {
                    //var query = dbContext.qm_statewise_inspection_monitosr_list(stateCode, qmType).ToList();
                    //var query = dbContext.qm_statewise_inspection_monitosr_list(stateCode, qmType).ToList();
                    var query1 = (from aqm in dbContext.ADMIN_QUALITY_MONITORS
                                  where aqm.ADMIN_QM_TYPE == qmType
                                  && aqm.ADMIN_QM_EMPANELLED == "Y"
                                  && (aqm.MAST_STATE_CODE == stateCode || aqm.MAST_STATE_CODE_ADDR == stateCode)
                                      //Added on 15 June 2021 to restrict schedule assigning to monitors with age greater than 66 year 11 months
                                  && (SqlFunctions.DateDiff("day", aqm.ADMIN_QM_BIRTH_DATE, DateTime.Now) <= 24424 || aqm.ADMIN_QM_BIRTH_DATE == null)
                                  select new
                                  {
                                      Value = aqm.ADMIN_QM_CODE,
                                      Text = (aqm.ADMIN_QM_LNAME.Equals(null) ? "" : aqm.ADMIN_QM_LNAME) + " " + (aqm.ADMIN_QM_FNAME.Equals(null) ? "" : aqm.ADMIN_QM_FNAME) + " " + (aqm.ADMIN_QM_MNAME.Equals(null) ? "" : aqm.ADMIN_QM_MNAME)
                                  }).OrderBy(c => c.Text).Distinct().OrderBy(a => a.Text).ToList();



                    var query2 = (from aqm in dbContext.ADMIN_QUALITY_MONITORS
                                  join interstate in dbContext.ADMIN_QUALITY_MONITORS_INTER_STATE on aqm.ADMIN_QM_CODE equals interstate.ADMIN_QM_CODE
                                  where interstate.ALLOWED_STATE_CODE == stateCode && interstate.APPROVED.Equals("Y") && aqm.ADMIN_QM_TYPE == qmType && aqm.ADMIN_QM_EMPANELLED == "Y"


                                  select new
                                  {
                                      Value = aqm.ADMIN_QM_CODE,
                                      Text = (aqm.ADMIN_QM_LNAME.Equals(null) ? "" : aqm.ADMIN_QM_LNAME) + " " + (aqm.ADMIN_QM_FNAME.Equals(null) ? "" : aqm.ADMIN_QM_FNAME) + " " + (aqm.ADMIN_QM_MNAME.Equals(null) ? "" : aqm.ADMIN_QM_MNAME)
                                  }).OrderBy(c => c.Text).Distinct().OrderBy(a => a.Text).ToList();

                    var query = query1.Union(query2);
                    foreach (var data in query)
                    {
                        item = new SelectListItem();
                        item.Text = data.Text;
                        item.Value = data.Value.ToString();
                        lstProfileNames.Add(item);
                    }
                    //foreach (var data in query)
                    //{
                    //    item = new SelectListItem();
                    //    item.Text = data.MONITOR_NAME;
                    //    item.Value = data.ADMIN_QM_CODE.ToString();
                    //    lstProfileNames.Add(item);
                    //}
                }



                return lstProfileNames;
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
                return null;
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

        #region Technical Expert

        public List<SelectListItem> PopulateTechnicalExpert()       //Add by Shreyas on 24-03-2023
        {
            List<SelectListItem> lstProfileNames = new List<SelectListItem>();
            dbContext = new PMGSYEntities();
            try
            {
                SelectListItem item = new SelectListItem();

                item.Text = "Select Technical Expert";
                item.Value = "0";
                item.Selected = true;
                lstProfileNames.Add(item);


                var query = (from c in dbContext.MASTER_TECHNICAL_EXPERT
                             where c.IS_ACTIVE == "Y"
                             && dbContext.UM_User_Master.Where(x => x.UserID == c.TECHNICAL_EXPERT_USER_ID).Any() // Added by Srishti on 12-04-2023
                             select new
                             {
                                 Value = c.ID,
                                 Text = ((c.TECHNICAL_EXPERT_FNAME.Equals(null) ? "" : c.TECHNICAL_EXPERT_FNAME) + " " + (c.TECHNICAL_EXPERT_MNAME.Equals(null) ? "" : c.TECHNICAL_EXPERT_MNAME) + " " + (c.TECHNICAL_EXPERT_LNAME.Equals(null) ? "" : c.TECHNICAL_EXPERT_LNAME) + " (" + c.TECHNICAL_EXPERT_PAN + ")")
                             }).OrderBy(c => c.Text).ToList();

                foreach (var data in query)
                {
                    item = new SelectListItem();
                    item.Text = data.Text;
                    item.Value = data.Value.ToString();
                    lstProfileNames.Add(item);
                }

                return lstProfileNames;
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
                return null;
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
}
