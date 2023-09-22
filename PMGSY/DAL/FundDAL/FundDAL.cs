#region HEADER
/*
 * Project Id:

 * Project Name:OMMAS-II

 * File Name: FundDAL.cs

 * Author : Vikram Nandanwar

 * Creation Date :05/June/2013

 * Desc : This class is used as data access layer to perform Save,Edit,Delete and listing of Fund Allocation and Release screens.  
*/

#endregion

using PMGSY.Common;
using PMGSY.Extensions;
using PMGSY.Models;
using PMGSY.Models.Fund;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.DAL.FundDAL
{
    public class FundDAL : IFundDAL
    {

        Dictionary<string, string> decryptedParameters = null;
        string[] encryptedParameters = null;
        const int roleCode = 25;

        #region FUND_ALLOCATION

        /// <summary>
        /// returns the list of Fund Allocation details to display it on grid
        /// </summary>
        /// <param name="stateCode"></param>
        /// <param name="fundType"></param>
        /// <param name="fundingAgencyCode"></param>
        /// <param name="yearCode"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array GetFundAllocationList(int stateCode, string fundType, int fundingAgencyCode, int yearCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            totalRecords = 0;
            try
            {

                string flag = string.Empty;
                if (PMGSYSession.Current.LevelId == 6)
                {
                    flag = "C";
                }
                else
                {
                    flag = "S";
                }

                var listFundAllocation = (from item in dbContext.MRD_FUND_ALLOCATION
                                          join agency in dbContext.MASTER_FUNDING_AGENCY on item.MAST_FUNDING_AGENCY_CODE equals agency.MAST_FUNDING_AGENCY_CODE
                                          join admin in dbContext.ADMIN_DEPARTMENT on item.ADMIN_NO_CODE equals admin.ADMIN_ND_CODE
                                          join year in dbContext.MASTER_YEAR on item.MAST_YEAR equals year.MAST_YEAR_CODE
                                          where
                                          (stateCode == 0 ? 1 : item.MAST_STATE_CODE) == (stateCode == 0 ? 1 : stateCode) &&
                                          (fundType == "0" ? "%" : item.MAST_FUND_TYPE) == (fundType == "0" ? "%" : fundType) &&
                                          (fundingAgencyCode == 0 ? 1 : item.MAST_FUNDING_AGENCY_CODE) == (fundingAgencyCode == 0 ? 1 : fundingAgencyCode) &&
                                          (yearCode == 0 ? 1 : item.MAST_YEAR) == (yearCode == 0 ? 1 : yearCode) &&
                                          (flag == "C" ? 1 == 1 : (item.ADMIN_NO_CODE == PMGSYSession.Current.AdminNdCode))
                                          select new
                                          {
                                              item.MAST_ALLOCATION_AMOUNT,
                                              item.MAST_ALLOCATION_DATE,
                                              item.MAST_ALLOCATION_FILE,
                                              item.MAST_ALLOCATION_ORDER,
                                              item.MAST_FUND_TYPE,
                                              agency.MAST_FUNDING_AGENCY_NAME,
                                              item.MAST_STATE_CODE,
                                              item.MAST_TRANSACTION_NO,
                                              year.MAST_YEAR_TEXT,
                                              item.MASTER_FUNDING_AGENCY,
                                              admin.ADMIN_ND_NAME,
                                              item.ADMIN_NO_CODE,
                                              item.MAST_YEAR,
                                              item.MAST_FUNDING_AGENCY_CODE,
                                          });

                totalRecords = listFundAllocation.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "MAST_YEAR_TEXT":
                                listFundAllocation = listFundAllocation.OrderBy(x => x.MAST_YEAR_TEXT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_TRANSACTION_NO":
                                listFundAllocation = listFundAllocation.OrderBy(x => x.MAST_TRANSACTION_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "ADMIN_ND_NAME":
                                listFundAllocation = listFundAllocation.OrderBy(x => x.ADMIN_ND_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_FUNDING_AGENCY_NAME":
                                listFundAllocation = listFundAllocation.OrderBy(x => x.MAST_FUNDING_AGENCY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_ALLOCATION_AMOUNT":
                                listFundAllocation = listFundAllocation.OrderBy(x => x.MAST_ALLOCATION_AMOUNT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_ALLOCATION_DATE":
                                listFundAllocation = listFundAllocation.OrderBy(x => x.MAST_ALLOCATION_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_ALLOCATION_ORDER":
                                listFundAllocation = listFundAllocation.OrderBy(x => x.MAST_ALLOCATION_ORDER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                listFundAllocation = listFundAllocation.OrderByDescending(x => x.MAST_YEAR_TEXT).ThenByDescending(x => x.MAST_TRANSACTION_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "MAST_YEAR_TEXT":
                                listFundAllocation = listFundAllocation.OrderByDescending(x => x.MAST_YEAR_TEXT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_TRANSACTION_NO":
                                listFundAllocation = listFundAllocation.OrderByDescending(x => x.MAST_TRANSACTION_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "ADMIN_ND_NAME":
                                listFundAllocation = listFundAllocation.OrderByDescending(x => x.ADMIN_ND_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_FUNDING_AGENCY_NAME":
                                listFundAllocation = listFundAllocation.OrderByDescending(x => x.MAST_FUNDING_AGENCY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_ALLOCATION_AMOUNT":
                                listFundAllocation = listFundAllocation.OrderByDescending(x => x.MAST_ALLOCATION_AMOUNT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_ALLOCATION_DATE":
                                listFundAllocation = listFundAllocation.OrderByDescending(x => x.MAST_ALLOCATION_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_ALLOCATION_ORDER":
                                listFundAllocation = listFundAllocation.OrderByDescending(x => x.MAST_ALLOCATION_ORDER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                listFundAllocation = listFundAllocation.OrderByDescending(x => x.MAST_YEAR_TEXT).ThenByDescending(x => x.MAST_TRANSACTION_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                }
                else
                {
                    listFundAllocation = listFundAllocation.OrderBy(x => x.MAST_YEAR_TEXT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }



                var result = listFundAllocation.Select(fundDetails => new
                {
                    fundDetails.MAST_FUNDING_AGENCY_CODE,
                    fundDetails.MAST_FUND_TYPE,
                    fundDetails.MAST_STATE_CODE,
                    fundDetails.MAST_YEAR_TEXT,
                    fundDetails.MAST_ALLOCATION_ORDER,
                    fundDetails.MAST_FUNDING_AGENCY_NAME,
                    fundDetails.MAST_ALLOCATION_AMOUNT,
                    fundDetails.MAST_ALLOCATION_DATE,
                    fundDetails.MAST_TRANSACTION_NO,
                    fundDetails.ADMIN_ND_NAME,
                    fundDetails.MAST_YEAR,
                    fundDetails.ADMIN_NO_CODE,
                    fundDetails.MAST_ALLOCATION_FILE
                }).ToArray();



                var gridData = result.Select(fundDetails => new
                {
                    cell = new[]
                                    {
                                        fundDetails.MAST_YEAR_TEXT == null?string.Empty:fundDetails.MAST_YEAR_TEXT.ToString(),
                                        fundDetails.MAST_TRANSACTION_NO == null?string.Empty:fundDetails.MAST_TRANSACTION_NO.ToString(),
                                        fundDetails.ADMIN_ND_NAME == null?string.Empty:fundDetails.ADMIN_ND_NAME.ToString(),
                                        fundDetails.MAST_FUNDING_AGENCY_NAME == null?string.Empty:fundDetails.MAST_FUNDING_AGENCY_NAME.ToString(),
                                        fundDetails.MAST_ALLOCATION_AMOUNT == null?string.Empty:Math.Round(fundDetails.MAST_ALLOCATION_AMOUNT,2).ToString(),
                                        fundDetails.MAST_ALLOCATION_DATE == null?string.Empty:Convert.ToDateTime(fundDetails.MAST_ALLOCATION_DATE).ToString("dd/MM/yyyy"),
                                        fundDetails.MAST_ALLOCATION_ORDER==null?string.Empty:fundDetails.MAST_ALLOCATION_ORDER.ToString(),
                                        flag=="C"?"<a href='#' id='uploadFile' title='Click here to Upload File' class='ui-icon ui-icon-plusthick ui-align-center' onClick=UploadFundAllocationFiles('" + URLEncrypt.EncryptParameters1(new string[]{"TId="+fundDetails.MAST_TRANSACTION_NO.ToString().Trim(),"StateId=" +fundDetails.MAST_STATE_CODE.ToString().Trim(),"AdminCode="+fundDetails.ADMIN_NO_CODE.ToString().Trim(),"YearCode="+fundDetails.MAST_YEAR.ToString().Trim(),"FundType=" +fundDetails.MAST_FUND_TYPE.ToString().Trim(),"FundingAgency="+fundDetails.MAST_FUNDING_AGENCY_CODE.ToString().Trim()})+"'); return false;'>Upload</a>":"<span class='ui-icon ui-icon-locked ui-align-center'></span>",
                                        //flag=="C"?(fundDetails.MAST_ALLOCATION_FILE==null?string.Empty:URLEncrypt.EncryptParameters(new string[]{fundDetails.MAST_ALLOCATION_FILE})):string.Empty,
                                        //dbContext.MRD_FUND_RELEASE.Any(m=>m.MAST_YEAR == fundDetails.MAST_YEAR && m.MAST_STATE_CODE == fundDetails.MAST_STATE_CODE) == true?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":(flag=="C"?"<a href='#' title='Click here to edit the Fund Allocation Details' class='ui-icon ui-icon-pencil ui-align-center' onClick=EditFundAllocationDetails('" + URLEncrypt.EncryptParameters1(new string[]{"TId="+fundDetails.MAST_TRANSACTION_NO.ToString().Trim(),"StateId=" +fundDetails.MAST_STATE_CODE.ToString().Trim(),"AdminCode="+fundDetails.ADMIN_NO_CODE.ToString().Trim(),"YearCode="+fundDetails.MAST_YEAR.ToString().Trim(),"FundType=" +fundDetails.MAST_FUND_TYPE.ToString().Trim(),"FundingAgency="+fundDetails.MAST_FUNDING_AGENCY_CODE.ToString().Trim()})+"'); return false;'>Delete</a>":"<span class='ui-icon ui-icon-locked ui-align-center'></span>"),
                                        //dbContext.MRD_FUND_RELEASE.Any(m=>m.MAST_YEAR == fundDetails.MAST_YEAR && m.MAST_STATE_CODE == fundDetails.MAST_STATE_CODE) == true?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":(flag=="C"?"<a href='#' title='Click here to delete the File and File Details' class='ui-icon ui-icon-trash ui-align-center' onClick=DeleteFundAllocationDetails('" +URLEncrypt.EncryptParameters1(new string[]{"TId="+fundDetails.MAST_TRANSACTION_NO.ToString().Trim(),"StateId=" +fundDetails.MAST_STATE_CODE.ToString().Trim(),"AdminCode="+fundDetails.ADMIN_NO_CODE.ToString().Trim(),"YearCode="+fundDetails.MAST_YEAR.ToString().Trim(),"FundType=" +fundDetails.MAST_FUND_TYPE.ToString().Trim(),"FundingAgency="+fundDetails.MAST_FUNDING_AGENCY_CODE.ToString().Trim()})+"'); return false;'>Delete</a>":"<span class='ui-icon ui-icon-locked ui-align-center'></span>"),
                                        fundDetails.MAST_TRANSACTION_NO >= dbContext.MRD_FUND_ALLOCATION.Where(m=>m.MAST_YEAR == fundDetails.MAST_YEAR && m.MAST_STATE_CODE == fundDetails.MAST_STATE_CODE && m.MAST_FUND_TYPE == fundDetails.MAST_FUND_TYPE && m.ADMIN_NO_CODE == fundDetails.ADMIN_NO_CODE && m.MAST_FUNDING_AGENCY_CODE == fundDetails.MAST_FUNDING_AGENCY_CODE).OrderByDescending(m=>m.MAST_TRANSACTION_NO).Select(m=>m.MAST_TRANSACTION_NO).FirstOrDefault() == true?(flag=="C"?"<a href='#' title='Click here to edit the Fund Allocation Details' class='ui-icon ui-icon-pencil ui-align-center' onClick=EditFundAllocationDetails('" + URLEncrypt.EncryptParameters1(new string[]{"TId="+fundDetails.MAST_TRANSACTION_NO.ToString().Trim(),"StateId=" +fundDetails.MAST_STATE_CODE.ToString().Trim(),"AdminCode="+fundDetails.ADMIN_NO_CODE.ToString().Trim(),"YearCode="+fundDetails.MAST_YEAR.ToString().Trim(),"FundType=" +fundDetails.MAST_FUND_TYPE.ToString().Trim(),"FundingAgency="+fundDetails.MAST_FUNDING_AGENCY_CODE.ToString().Trim()})+"'); return false;'>Delete</a>":"<span class='ui-icon ui-icon-locked ui-align-center'></span>"):"<span class='ui-icon ui-icon-locked ui-align-center'></span>",
                                        fundDetails.MAST_TRANSACTION_NO >= dbContext.MRD_FUND_ALLOCATION.Where(m=>m.MAST_YEAR == fundDetails.MAST_YEAR && m.MAST_STATE_CODE == fundDetails.MAST_STATE_CODE && m.MAST_FUND_TYPE == fundDetails.MAST_FUND_TYPE && m.ADMIN_NO_CODE == fundDetails.ADMIN_NO_CODE && m.MAST_FUNDING_AGENCY_CODE == fundDetails.MAST_FUNDING_AGENCY_CODE).OrderByDescending(m=>m.MAST_TRANSACTION_NO).Select(m=>m.MAST_TRANSACTION_NO).FirstOrDefault() == true?(flag=="C"?"<a href='#' title='Click here to delete the File and File Details' class='ui-icon ui-icon-trash ui-align-center' onClick=DeleteFundAllocationDetails('" +URLEncrypt.EncryptParameters1(new string[]{"TId="+fundDetails.MAST_TRANSACTION_NO.ToString().Trim(),"StateId=" +fundDetails.MAST_STATE_CODE.ToString().Trim(),"AdminCode="+fundDetails.ADMIN_NO_CODE.ToString().Trim(),"YearCode="+fundDetails.MAST_YEAR.ToString().Trim(),"FundType=" +fundDetails.MAST_FUND_TYPE.ToString().Trim(),"FundingAgency="+fundDetails.MAST_FUNDING_AGENCY_CODE.ToString().Trim()})+"'); return false;'>Delete</a>":"<span class='ui-icon ui-icon-locked ui-align-center'></span>"):"<span class='ui-icon ui-icon-locked ui-align-center'></span>",

                                    }
                }).ToArray();

                return gridData;
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
        /// returns the list of states to populate dropdown of state
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetAllStates()
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                List<SelectListItem> list = new SelectList(dbContext.MASTER_STATE.ToList(), "MAST_STATE_CODE", "MAST_STATE_NAME").ToList();
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
        /// returns the SRRDA list 
        /// </summary>
        /// <param name="stateCode"></param>
        /// <returns></returns>
        public List<SelectListItem> GetExecutingAgencyByStateCode(int stateCode, int selectedCode = 0)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                var listFundingAgency = (from item in dbContext.ADMIN_DEPARTMENT
                                         where item.MAST_ND_TYPE == "S" &&
                                         item.MAST_STATE_CODE == stateCode
                                         select new
                                         {
                                             item.ADMIN_ND_CODE,
                                             item.ADMIN_ND_NAME
                                         }).ToList();

                List<SelectListItem> lstFundingAgency = new SelectList(listFundingAgency, "ADMIN_ND_CODE", "ADMIN_ND_NAME", selectedCode).ToList<SelectListItem>();
                return lstFundingAgency;
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
        /// returns Fund Type list
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetFundType(bool isAdd = false)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                if (isAdd == false)
                {
                    List<SelectListItem> lstFundType = new List<SelectListItem>();
                    lstFundType.Add(new SelectListItem { Value = "0", Text = "--All--" });
                    lstFundType.Add(new SelectListItem { Value = "P", Text = "Programme Fund" });
                    lstFundType.Add(new SelectListItem { Value = "A", Text = "Administrative Fund" });
                    lstFundType.Add(new SelectListItem { Value = "M", Text = "Maintenance Fund" });
                    return lstFundType;
                }
                else
                {
                    List<SelectListItem> lstFundType = new List<SelectListItem>();
                    lstFundType.Add(new SelectListItem { Value = "0", Text = "--Select Fund Type--" });
                    lstFundType.Add(new SelectListItem { Value = "P", Text = "Programme Fund" });
                    lstFundType.Add(new SelectListItem { Value = "A", Text = "Administrative Fund" });
                    lstFundType.Add(new SelectListItem { Value = "M", Text = "Maintenance Fund" });
                    return lstFundType;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
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
        /// populates the years
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetAllYear()
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                List<SelectListItem> lstYear = new SelectList(dbContext.MASTER_YEAR.Where(m => m.MAST_YEAR_CODE <= DateTime.Now.Year).ToList(), "MAST_YEAR_CODE", "MAST_YEAR_TEXT").ToList();
                return lstYear;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
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
        /// populates the Funding agency dropdown
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetFundingAgency()
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                List<SelectListItem> lstFundingAgency = new SelectList(dbContext.MASTER_FUNDING_AGENCY.ToList(), "MAST_FUNDING_AGENCY_CODE", "MAST_FUNDING_AGENCY_NAME").ToList<SelectListItem>();
                return lstFundingAgency;
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
        /// saves the data of fund allocation
        /// </summary>
        /// <param name="fundModel"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool AddFundAllocation(FundAllocationViewModel fundModel, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                CommonFunctions objFunc = new CommonFunctions();
                int recordCount = 0;//dbContext.MRD_FUND_ALLOCATION.Where(m => m.MAST_STATE_CODE == fundModel.MAST_STATE_CODE && m.ADMIN_NO_CODE == fundModel.ADMIN_NO_CODE && m.MAST_YEAR == fundModel.MAST_YEAR && m.MAST_TRANSACTION_NO != fundModel.MAST_TRANSACTION_NO).Count();
                int maxCount = 0;
                if (recordCount > 0)
                {
                    message = "Fund Allocation information already exist.";
                    return false;
                }

                MRD_FUND_ALLOCATION master = new MRD_FUND_ALLOCATION();
                maxCount = dbContext.MRD_FUND_ALLOCATION.Where(m => m.MAST_STATE_CODE == fundModel.MAST_STATE_CODE && m.ADMIN_NO_CODE == fundModel.ADMIN_NO_CODE && m.MAST_YEAR == fundModel.MAST_YEAR && m.MAST_FUND_TYPE == fundModel.MAST_FUND_TYPE && m.MAST_FUNDING_AGENCY_CODE == fundModel.MAST_FUNDING_AGENCY_CODE).OrderByDescending(m => m.MAST_TRANSACTION_NO).Select(m => m.MAST_TRANSACTION_NO).FirstOrDefault();
                master.MAST_TRANSACTION_NO = maxCount + 1;
                master.ADMIN_NO_CODE = fundModel.ADMIN_NO_CODE;
                master.MAST_FUND_TYPE = fundModel.MAST_FUND_TYPE;
                master.MAST_FUNDING_AGENCY_CODE = fundModel.MAST_FUNDING_AGENCY_CODE;
                master.MAST_ALLOCATION_AMOUNT = fundModel.MAST_ALLOCATION_AMOUNT;
                if (fundModel.MAST_ALLOCATION_DATE != null)
                {
                    master.MAST_ALLOCATION_DATE = objFunc.GetStringToDateTime(fundModel.MAST_ALLOCATION_DATE);
                }
                master.MAST_ALLOCATION_FILE = fundModel.MAST_ALLOCATION_FILE;
                master.MAST_ALLOCATION_ORDER = fundModel.MAST_ALLOCATION_ORDER;
                master.MAST_YEAR = fundModel.MAST_YEAR;
                master.MAST_STATE_CODE = fundModel.MAST_STATE_CODE;
                master.MAST_YEAR = fundModel.MAST_YEAR;

                //added by abhishek kamble 27-nov-2013
                master.USERID = PMGSYSession.Current.UserId;
                master.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                dbContext.MRD_FUND_ALLOCATION.Add(master);
                dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// deletes the data of fund allocation
        /// </summary>
        /// <param name="transactionCode"></param>
        /// <param name="stateCode"></param>
        /// <param name="adminCode"></param>
        /// <param name="yearCode"></param>
        /// <param name="fundType"></param>
        /// <param name="fundingAgencyCode"></param>
        /// <returns></returns>
        public bool DeleteFundAllocation(int transactionCode, int stateCode, int adminCode, int yearCode, string fundType, int fundingAgencyCode, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                MRD_FUND_ALLOCATION master = dbContext.MRD_FUND_ALLOCATION.Where(m => m.MAST_STATE_CODE == stateCode && m.ADMIN_NO_CODE == adminCode && m.MAST_YEAR == yearCode && m.MAST_FUND_TYPE == fundType && m.MAST_FUNDING_AGENCY_CODE == fundingAgencyCode && m.MAST_TRANSACTION_NO == transactionCode).FirstOrDefault();
                decimal? totalRelease = dbContext.MRD_FUND_RELEASE.Where(m => m.MAST_STATE_CODE == stateCode && m.ADMIN_NO_CODE == adminCode && m.MAST_YEAR == yearCode && m.MAST_FUND_TYPE == fundType && m.MAST_FUNDING_AGENCY_CODE == fundingAgencyCode && m.MAST_RELEASE_TYPE == "C").Sum(m => (Decimal?)m.MAST_RELEASE_AMOUNT);
                decimal? totalAllocation = dbContext.MRD_FUND_ALLOCATION.Where(m => m.MAST_STATE_CODE == stateCode && m.ADMIN_NO_CODE == adminCode && m.MAST_YEAR == yearCode && m.MAST_FUND_TYPE == fundType && m.MAST_FUNDING_AGENCY_CODE == fundingAgencyCode).Sum(m => (Decimal?)m.MAST_ALLOCATION_AMOUNT);
                totalAllocation = totalAllocation - master.MAST_ALLOCATION_AMOUNT;
                if (totalAllocation < totalRelease)
                {
                    message = "Allocation details can not be deleted as Fund is already released for this allocation.";
                    return false;
                }

                //added by abhishek kamble 27-nov-2013
                master.USERID = PMGSYSession.Current.UserId;
                master.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                dbContext.Entry(master).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                dbContext.MRD_FUND_ALLOCATION.Remove(master);
                dbContext.SaveChanges();

                if (transactionCode > 1)
                {
                    var list = (from item in dbContext.MRD_FUND_ALLOCATION
                                where item.MAST_TRANSACTION_NO > transactionCode &&
                                item.ADMIN_NO_CODE == adminCode &&
                                item.MAST_FUND_TYPE == fundType &&
                                item.MAST_FUNDING_AGENCY_CODE == fundingAgencyCode &&
                                item.MAST_STATE_CODE == stateCode &&
                                item.MAST_YEAR == yearCode
                                select new { item.MAST_TRANSACTION_NO });

                }
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// returns the details of fund allocation to update
        /// </summary>
        /// <param name="transactionCode"></param>
        /// <param name="stateCode"></param>
        /// <param name="adminCode"></param>
        /// <param name="yearCode"></param>
        /// <param name="fundType"></param>
        /// <param name="fundingAgencyCode"></param>
        /// <returns></returns>
        public FundAllocationViewModel GetFundAllocationDetails(int transactionCode, int stateCode, int adminCode, int yearCode, string fundType, int fundingAgencyCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                MRD_FUND_ALLOCATION master = dbContext.MRD_FUND_ALLOCATION.Where(m => m.MAST_TRANSACTION_NO == transactionCode && m.MAST_STATE_CODE == stateCode && m.ADMIN_NO_CODE == adminCode && m.MAST_YEAR == yearCode && m.MAST_FUND_TYPE == fundType && m.MAST_FUNDING_AGENCY_CODE == fundingAgencyCode).FirstOrDefault();
                FundAllocationViewModel model = null;
                if (master != null)
                {
                    model = new FundAllocationViewModel()
                    {
                        EncryptedFundCode = URLEncrypt.EncryptParameters1(new string[] { "FundCode=" + master.MAST_TRANSACTION_NO.ToString().Trim() }),
                        ADMIN_NO_CODE = master.ADMIN_NO_CODE,
                        MAST_FUND_TYPE = master.MAST_FUND_TYPE,
                        MAST_FUNDING_AGENCY_CODE = master.MAST_FUNDING_AGENCY_CODE,
                        MAST_ALLOCATION_AMOUNT = master.MAST_ALLOCATION_AMOUNT,
                        MAST_ALLOCATION_DATE = (master.MAST_ALLOCATION_DATE == null ? "" : objCommon.GetDateTimeToString(master.MAST_ALLOCATION_DATE.Value).ToString()),
                        MAST_ALLOCATION_FILE = master.MAST_ALLOCATION_FILE,
                        MAST_ALLOCATION_ORDER = master.MAST_ALLOCATION_ORDER,
                        MAST_STATE_CODE = master.MAST_STATE_CODE,
                        MAST_TRANSACTION_NO = master.MAST_TRANSACTION_NO,
                        MAST_YEAR = master.MAST_YEAR,
                        transactionCount = master.MAST_TRANSACTION_NO
                    };

                    model.TotalRelease = dbContext.MRD_FUND_RELEASE.Where(m => m.ADMIN_NO_CODE == adminCode && m.MAST_STATE_CODE == stateCode && m.MAST_RELEASE_TYPE == "C" && m.MAST_FUND_TYPE == fundType && m.MAST_FUNDING_AGENCY_CODE == fundingAgencyCode && m.MAST_YEAR == yearCode).Sum(m => (Decimal?)m.MAST_RELEASE_AMOUNT);
                }
                return model;
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
        /// updates the fund allocation data
        /// </summary>
        /// <param name="fundModel"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool EditFundAllocation(FundAllocationViewModel fundModel, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                int fundCode = 0;

                decimal? TotalRelease = dbContext.MRD_FUND_RELEASE.Where(m => m.MAST_STATE_CODE == fundModel.MAST_STATE_CODE && m.ADMIN_NO_CODE == fundModel.ADMIN_NO_CODE && m.MAST_FUND_TYPE == fundModel.MAST_FUND_TYPE && m.MAST_FUNDING_AGENCY_CODE == fundModel.MAST_FUNDING_AGENCY_CODE && m.MAST_YEAR == fundModel.MAST_YEAR && m.MAST_RELEASE_TYPE == "C").Sum(m => (Decimal?)m.MAST_RELEASE_AMOUNT);
                CommonFunctions objCommon = new CommonFunctions();
                encryptedParameters = fundModel.EncryptedFundCode.Split('/');
                if (!(encryptedParameters.Length == 3))
                {
                    return false;
                }

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                fundCode = Convert.ToInt32(decryptedParameters["FundCode"].ToString());

                MRD_FUND_ALLOCATION master = dbContext.MRD_FUND_ALLOCATION.Where(m => m.MAST_TRANSACTION_NO == fundCode && m.MAST_YEAR == fundModel.MAST_YEAR && m.MAST_STATE_CODE == fundModel.MAST_STATE_CODE && m.ADMIN_NO_CODE == fundModel.ADMIN_NO_CODE && m.MAST_FUND_TYPE == fundModel.MAST_FUND_TYPE).FirstOrDefault();
                decimal? TotalAllocation = dbContext.MRD_FUND_ALLOCATION.Where(m => m.MAST_TRANSACTION_NO != fundCode && m.MAST_YEAR == fundModel.MAST_YEAR && m.MAST_STATE_CODE == fundModel.MAST_STATE_CODE && m.ADMIN_NO_CODE == fundModel.ADMIN_NO_CODE && m.MAST_FUND_TYPE == fundModel.MAST_FUND_TYPE).Sum(m => (Decimal?)m.MAST_ALLOCATION_AMOUNT);
                TotalAllocation = TotalAllocation + fundModel.MAST_ALLOCATION_AMOUNT;
                if (TotalAllocation < TotalRelease)
                {
                    message = "Total Allocation amount is less than Total Release Amount.";
                    return false;
                }


                if (master != null)
                {
                    master.ADMIN_NO_CODE = fundModel.ADMIN_NO_CODE;
                    master.MAST_FUND_TYPE = fundModel.MAST_FUND_TYPE;
                    master.MAST_FUNDING_AGENCY_CODE = fundModel.MAST_FUNDING_AGENCY_CODE;
                    master.MAST_ALLOCATION_AMOUNT = fundModel.MAST_ALLOCATION_AMOUNT;
                    if (fundModel.MAST_ALLOCATION_DATE == null)
                    {
                        master.MAST_ALLOCATION_DATE = null;
                    }
                    else
                    {
                        master.MAST_ALLOCATION_DATE = objCommon.GetStringToDateTime(fundModel.MAST_ALLOCATION_DATE);
                    }
                    master.MAST_ALLOCATION_FILE = fundModel.MAST_ALLOCATION_FILE;
                    master.MAST_ALLOCATION_ORDER = fundModel.MAST_ALLOCATION_ORDER;
                    master.MAST_STATE_CODE = fundModel.MAST_STATE_CODE;
                    master.MAST_YEAR = fundModel.MAST_YEAR;

                    //added by abhishek kamble 27-nov-2013
                    master.USERID = PMGSYSession.Current.UserId;
                    master.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    dbContext.Entry(master).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// adds the file data to the particular record
        /// </summary>
        /// <param name="fundModel"></param>
        /// <returns></returns>
        public bool AddFileUploadToTransaction(FundAllocationViewModel fundModel)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                MRD_FUND_ALLOCATION master = dbContext.MRD_FUND_ALLOCATION.Where(m => m.ADMIN_NO_CODE == fundModel.ADMIN_NO_CODE && m.MAST_FUND_TYPE == fundModel.MAST_FUND_TYPE && m.MAST_FUNDING_AGENCY_CODE == fundModel.MAST_FUNDING_AGENCY_CODE && m.MAST_STATE_CODE == fundModel.MAST_STATE_CODE && m.MAST_TRANSACTION_NO == fundModel.transactionCount && m.MAST_YEAR == fundModel.MAST_YEAR).FirstOrDefault();
                master.MAST_ALLOCATION_FILE = fundModel.name;

                //added by abhishek kamble 27-nov-2013
                master.USERID = PMGSYSession.Current.UserId;
                master.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                dbContext.Entry(master).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// list of Files uploaded along the allocation/release
        /// </summary>
        /// <param name="page">no. of pages</param>
        /// <param name="rows">no. of rows</param>
        /// <param name="sidx">sort column field</param>
        /// <param name="sord">sort order</param>
        /// <param name="totalRecords">total no. of records</param>
        /// <param name="stateCode">id of state</param>
        /// <param name="adminCode">Nodal officer code</param>
        /// <param name="fundType">type of fund</param>
        /// <param name="agencyCode">type of agency</param>
        /// <param name="yearCode">id of phase year</param>
        /// <param name="transactionCode"></param>
        /// <returns></returns>
        public Array GetListFilesDAL(int page, int rows, string sidx, string sord, out long totalRecords, int stateCode, int adminCode, string fundType, int agencyCode, int yearCode, int transactionCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                var lstFiles = (from item in dbContext.MRD_FUND_ALLOCATION
                                where item.MAST_STATE_CODE == stateCode &&
                                item.ADMIN_NO_CODE == adminCode &&
                                item.MAST_FUND_TYPE == fundType &&
                                item.MAST_FUNDING_AGENCY_CODE == agencyCode &&
                                item.MAST_YEAR == yearCode &&
                                item.MAST_TRANSACTION_NO == transactionCode
                                select new
                                {
                                    item.MAST_ALLOCATION_FILE,
                                    item.MAST_TRANSACTION_NO,
                                    item.MAST_STATE_CODE,
                                    item.MAST_FUNDING_AGENCY_CODE,
                                    item.MAST_FUND_TYPE,
                                    item.MAST_YEAR,
                                    item.ADMIN_NO_CODE
                                });

                if (lstFiles.Any((m => m.MAST_ALLOCATION_FILE == null)))
                {
                    totalRecords = 0;
                }
                else
                {
                    totalRecords = 1;
                }

                var gridData = lstFiles.Select(files => new
                {
                    files.MAST_ALLOCATION_FILE,
                    files.ADMIN_NO_CODE,
                    files.MAST_FUND_TYPE,
                    files.MAST_FUNDING_AGENCY_CODE,
                    files.MAST_STATE_CODE,
                    files.MAST_TRANSACTION_NO,
                    files.MAST_YEAR
                }).ToArray();

                return gridData.Select(m => new
                {
                    cell = new[]
                    {
                        //m.MAST_FUND_TYPE.ToString(),
                        //m.MAST_FUNDING_AGENCY_CODE.ToString(),
                        //m.MAST_STATE_CODE.ToString(),
                        //m.MAST_TRANSACTION_NO.ToString(),
                        //m.MAST_YEAR.ToString(),
                        //m.ADMIN_NO_CODE.ToString()
                        URLEncrypt.EncryptParameters1(new string[]{m.MAST_ALLOCATION_FILE.ToString().Trim()}),
                        "<a href='#' title='Click here to delete the File' class='ui-icon ui-icon-trash ui-align-center' onClick=DeleteFundAllocationFile('" +URLEncrypt.EncryptParameters1(new string[]{"FileName="+m.MAST_ALLOCATION_FILE.ToString().Trim(),"State="+m.MAST_STATE_CODE.ToString().Trim()})+"'); return false;'>Delete</a>",
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
        /// deletes the file associated with the allocation
        /// </summary>
        /// <param name="file">filename</param>
        /// <param name="stateCode">state id</param>
        /// <param name="message">response message</param>
        /// <returns></returns>
        public bool DeleteAllocationFile(string file, int stateCode, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                string[] fileString = file.Split('-');
                int transCode = Convert.ToInt32(fileString[1]);
                int yearCode = Convert.ToInt32(fileString[0]);
                string fundType = fileString[2];
                MRD_FUND_ALLOCATION fundAllocationMaster = (dbContext.MRD_FUND_ALLOCATION.Where(m => m.MAST_YEAR == yearCode && m.MAST_TRANSACTION_NO == transCode && m.MAST_FUND_TYPE == fundType && m.MAST_STATE_CODE == stateCode)).FirstOrDefault();
                fundAllocationMaster.MAST_ALLOCATION_FILE = null;

                //added by abhishek kamble 27-nov-2013
                fundAllocationMaster.USERID = PMGSYSession.Current.UserId;
                fundAllocationMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                dbContext.Entry(fundAllocationMaster).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                message = "File deleted successfully.";
                return true;
            }
            catch (DbUpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "File is in use and can not be deleted.";
                return false;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "File is in use and can not be deleted.";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        #endregion

        #region FUND_RELEASE

        /// <summary>
        /// returns the list of fund released
        /// </summary>
        /// <param name="stateCode"></param>
        /// <param name="fundType"></param>
        /// <param name="fundingAgencyCode"></param>
        /// <param name="yearCode"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array GetFundReleaseList(int stateCode, string fundType, int fundingAgencyCode, int yearCode, string releaser, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                string flag = string.Empty;
                if (PMGSYSession.Current.LevelId == 6)
                {
                    flag = "C";
                }
                else
                {
                    flag = "S";
                }

                var listFundRelease = (from item in dbContext.MRD_FUND_RELEASE
                                       join agency in dbContext.MASTER_FUNDING_AGENCY on item.MAST_FUNDING_AGENCY_CODE equals agency.MAST_FUNDING_AGENCY_CODE
                                       join adminDepartment in dbContext.ADMIN_DEPARTMENT on item.ADMIN_NO_CODE equals adminDepartment.ADMIN_ND_CODE
                                       join year in dbContext.MASTER_YEAR on item.MAST_YEAR equals year.MAST_YEAR_CODE
                                       where
                                       (stateCode == 0 ? 1 : item.MAST_STATE_CODE) == (stateCode == 0 ? 1 : stateCode) &&
                                       (fundType == "0" ? "%" : item.MAST_FUND_TYPE) == (fundType == "0" ? "%" : fundType) &&
                                       (fundingAgencyCode == 0 ? 1 : item.MAST_FUNDING_AGENCY_CODE) == (fundingAgencyCode == 0 ? 1 : fundingAgencyCode) &&
                                       (yearCode == 0 ? 1 : item.MAST_YEAR) == (yearCode == 0 ? 1 : yearCode) &&
                                       (flag == "C" ? item.MAST_RELEASE_TYPE == "C" : (releaser == "0" ? "%" : item.MAST_RELEASE_TYPE) == (releaser == "0" ? "%" : releaser)) &&
                                       (flag == "C" ? 1 == 1 : (item.ADMIN_NO_CODE == PMGSYSession.Current.AdminNdCode))
                                       select new
                                       {
                                           item.MAST_RELEASE_AMOUNT,
                                           item.MAST_RELEASE_DATE,
                                           item.MAST_RELEASE_FILE,
                                           item.MAST_RELEASE_YEAR,
                                           item.MAST_RELEASE_ORDER,
                                           item.MAST_FUND_TYPE,
                                           adminDepartment.ADMIN_ND_NAME,
                                           agency.MAST_FUNDING_AGENCY_NAME,
                                           item.MAST_STATE_CODE,
                                           item.MAST_TRANSACTION_NO,
                                           item.MAST_YEAR,
                                           item.MASTER_FUNDING_AGENCY,
                                           item.ADMIN_NO_CODE,
                                           item.MAST_RELEASE_TYPE,
                                           year.MAST_YEAR_TEXT,
                                           item.MAST_FUNDING_AGENCY_CODE
                                       });

                totalRecords = listFundRelease.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "MAST_YEAR":
                                listFundRelease = listFundRelease.OrderBy(x => x.MAST_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_TRANSACTION_NO":
                                listFundRelease = listFundRelease.OrderBy(x => x.MAST_TRANSACTION_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_RELEASE_YEAR":
                                listFundRelease = listFundRelease.OrderBy(x => x.MAST_RELEASE_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "ADMIN_ND_NAME":
                                listFundRelease = listFundRelease.OrderBy(x => x.ADMIN_ND_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_FUNDING_AGENCY_NAME":
                                listFundRelease = listFundRelease.OrderBy(x => x.MAST_FUNDING_AGENCY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_RELEASE_AMOUNT":
                                listFundRelease = listFundRelease.OrderBy(x => x.MAST_RELEASE_AMOUNT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_RELEASE_DATE":
                                listFundRelease = listFundRelease.OrderBy(x => x.MAST_RELEASE_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_RELEASE_ORDER":
                                listFundRelease = listFundRelease.OrderBy(x => x.MAST_RELEASE_ORDER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_RELEASE_TYPE":
                                listFundRelease = listFundRelease.OrderBy(x => x.MAST_RELEASE_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                listFundRelease = listFundRelease.OrderByDescending(x => x.MAST_YEAR).ThenByDescending(x => x.MAST_TRANSACTION_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "MAST_YEAR":
                                listFundRelease = listFundRelease.OrderByDescending(x => x.MAST_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_TRANSACTION_NO":
                                listFundRelease = listFundRelease.OrderByDescending(x => x.MAST_TRANSACTION_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_RELEASE_YEAR":
                                listFundRelease = listFundRelease.OrderByDescending(x => x.MAST_RELEASE_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "ADMIN_ND_NAME":
                                listFundRelease = listFundRelease.OrderByDescending(x => x.ADMIN_ND_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_FUNDING_AGENCY_NAME":
                                listFundRelease = listFundRelease.OrderByDescending(x => x.MAST_FUNDING_AGENCY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_RELEASE_AMOUNT":
                                listFundRelease = listFundRelease.OrderByDescending(x => x.MAST_RELEASE_AMOUNT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_RELEASE_DATE":
                                listFundRelease = listFundRelease.OrderByDescending(x => x.MAST_RELEASE_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_RELEASE_ORDER":
                                listFundRelease = listFundRelease.OrderByDescending(x => x.MAST_RELEASE_ORDER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_RELEASE_TYPE":
                                listFundRelease = listFundRelease.OrderByDescending(x => x.MAST_RELEASE_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                listFundRelease = listFundRelease.OrderByDescending(x => x.MAST_YEAR).ThenByDescending(x => x.MAST_TRANSACTION_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                }
                else
                {
                    listFundRelease = listFundRelease.OrderBy(x => x.MAST_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }


                var result = listFundRelease.Select(fundDetails => new
                {
                    fundDetails.MAST_YEAR_TEXT,
                    fundDetails.MAST_RELEASE_TYPE,
                    fundDetails.ADMIN_NO_CODE,
                    fundDetails.MAST_YEAR,
                    fundDetails.MAST_RELEASE_YEAR,
                    fundDetails.ADMIN_ND_NAME,
                    fundDetails.MAST_FUNDING_AGENCY_NAME,
                    fundDetails.MAST_RELEASE_AMOUNT,
                    fundDetails.MAST_RELEASE_DATE,
                    fundDetails.MAST_TRANSACTION_NO,
                    fundDetails.MAST_STATE_CODE,
                    fundDetails.MAST_RELEASE_ORDER,
                    fundDetails.MAST_FUND_TYPE,
                    fundDetails.MAST_RELEASE_FILE,
                    fundDetails.MAST_FUNDING_AGENCY_CODE
                }).ToArray();



                var gridData = result.Select(fundDetails => new
                {
                    cell = new[]
                                    {
                                        fundDetails.MAST_YEAR_TEXT == null?string.Empty:fundDetails.MAST_YEAR_TEXT.ToString(),
                                        fundDetails.MAST_TRANSACTION_NO == null?string.Empty:fundDetails.MAST_TRANSACTION_NO.ToString(),
                                        fundDetails.MAST_RELEASE_TYPE=="S"?"SRRDA":"MORD",
                                        fundDetails.MAST_RELEASE_YEAR ==null?"-":(from item in dbContext.MASTER_YEAR where item.MAST_YEAR_CODE==fundDetails.MAST_RELEASE_YEAR select item.MAST_YEAR_TEXT).FirstOrDefault().ToString(),
                                        fundDetails.ADMIN_ND_NAME == null?string.Empty:fundDetails.ADMIN_ND_NAME.ToString(),
                                        fundDetails.MAST_FUNDING_AGENCY_NAME == null?string.Empty:fundDetails.MAST_FUNDING_AGENCY_NAME.ToString(),
                                        fundDetails.MAST_RELEASE_AMOUNT == null?string.Empty:Math.Round(fundDetails.MAST_RELEASE_AMOUNT,2).ToString(),
                                        fundDetails.MAST_RELEASE_DATE == null?string.Empty:Convert.ToDateTime(fundDetails.MAST_RELEASE_DATE).ToString("dd/MM/yyyy"),
                                        fundDetails.MAST_RELEASE_ORDER == null?string.Empty:fundDetails.MAST_RELEASE_ORDER.ToString(),
                                        //fundDetails.MAST_RELEASE_TYPE==flag?"<a href='#' title='Click here to Upload File' class='ui-icon ui-icon-plusthick ui-align-center' onClick=UploadFundReleaseFiles('" + URLEncrypt.EncryptParameters1(new string[]{"TId="+fundDetails.MAST_TRANSACTION_NO.ToString().Trim(),"StateId=" +fundDetails.MAST_STATE_CODE.ToString().Trim(),"AdminCode="+fundDetails.ADMIN_NO_CODE.ToString().Trim(),"YearCode="+fundDetails.MAST_YEAR.ToString().Trim(),"ReleaseType="+fundDetails.MAST_RELEASE_TYPE.ToString().Trim(),"FundType=" +fundDetails.MAST_FUND_TYPE.ToString().Trim(),"FundingAgency="+fundDetails.MAST_FUNDING_AGENCY_CODE.ToString().Trim()})+"'); return false;'>Upload</a>":"<a href='#' class='ui-icon ui-icon-locked ui-align-center'><span></span></a>",
                                        "<a href='#' title='Click here to Upload File' class='ui-icon ui-icon-plusthick ui-align-center' onClick=UploadFundReleaseFiles('" + URLEncrypt.EncryptParameters1(new string[]{"TId="+fundDetails.MAST_TRANSACTION_NO.ToString().Trim(),"StateId=" +fundDetails.MAST_STATE_CODE.ToString().Trim(),"AdminCode="+fundDetails.ADMIN_NO_CODE.ToString().Trim(),"YearCode="+fundDetails.MAST_YEAR.ToString().Trim(),"ReleaseType="+fundDetails.MAST_RELEASE_TYPE.ToString().Trim(),"FundType=" +fundDetails.MAST_FUND_TYPE.ToString().Trim(),"FundingAgency="+fundDetails.MAST_FUNDING_AGENCY_CODE.ToString().Trim()})+"'); return false;'>Upload</a>",
                                        //fundDetails.MAST_RELEASE_TYPE==flag?fundDetails.MAST_RELEASE_FILE==null?string.Empty:URLEncrypt.EncryptParameters(new string[] { fundDetails.MAST_RELEASE_FILE  }): string.Empty ,
                                        //fundDetails.MAST_RELEASE_TYPE==flag?"<a href='#' title='Click here to edit the Fund Release Details' class='ui-icon ui-icon-pencil ui-align-center' onClick=EditFundReleaseDetails('" +URLEncrypt.EncryptParameters1(new string[]{"TId="+fundDetails.MAST_TRANSACTION_NO.ToString().Trim(),"StateId=" +fundDetails.MAST_STATE_CODE.ToString().Trim(),"AdminCode="+fundDetails.ADMIN_NO_CODE.ToString().Trim(),"YearCode="+fundDetails.MAST_YEAR.ToString().Trim(),"ReleaseType="+fundDetails.MAST_RELEASE_TYPE.ToString().Trim(),"FundType=" +fundDetails.MAST_FUND_TYPE.ToString().Trim(),"FundingAgency="+fundDetails.MAST_FUNDING_AGENCY_CODE.ToString().Trim()})+"'); return false;'>Delete</a>":"<span class='ui-icon ui-icon-locked ui-align-center'></span>",
                                        //fundDetails.MAST_RELEASE_TYPE==flag?"<a href='#' title='Click here to delete the Fund Release Details' class='ui-icon ui-icon-trash ui-align-center' onClick=DeleteFundReleaseDetails('" +URLEncrypt.EncryptParameters1(new string[]{"TId="+fundDetails.MAST_TRANSACTION_NO.ToString().Trim(),"StateId=" +fundDetails.MAST_STATE_CODE.ToString().Trim(),"AdminCode="+fundDetails.ADMIN_NO_CODE.ToString().Trim(),"YearCode="+fundDetails.MAST_YEAR.ToString().Trim(),"ReleaseType="+fundDetails.MAST_RELEASE_TYPE.ToString().Trim(),"FundType=" +fundDetails.MAST_FUND_TYPE.ToString().Trim(),"FundingAgency="+fundDetails.MAST_FUNDING_AGENCY_CODE.ToString().Trim()})+"'); return false;'>Delete</a>":"<span class='ui-icon ui-icon-locked ui-align-center'></span>",
                                        //(fundDetails.MAST_TRANSACTION_NO >= dbContext.MRD_FUND_RELEASE.Where(m=>m.MAST_YEAR == fundDetails.MAST_YEAR && m.MAST_STATE_CODE == fundDetails.MAST_STATE_CODE && m.MAST_FUND_TYPE == fundDetails.MAST_FUND_TYPE && m.ADMIN_NO_CODE == fundDetails.ADMIN_NO_CODE && m.MAST_FUNDING_AGENCY_CODE == fundDetails.MAST_FUNDING_AGENCY_CODE).OrderByDescending(m=>m.MAST_TRANSACTION_NO).Select(m=>m.MAST_TRANSACTION_NO).FirstOrDefault()) == true?(fundDetails.MAST_RELEASE_TYPE==flag?"<a href='#' title='Click here to edit the Fund Release Details' class='ui-icon ui-icon-pencil ui-align-center' onClick=EditFundReleaseDetails('" +URLEncrypt.EncryptParameters1(new string[]{"TId="+fundDetails.MAST_TRANSACTION_NO.ToString().Trim(),"StateId=" +fundDetails.MAST_STATE_CODE.ToString().Trim(),"AdminCode="+fundDetails.ADMIN_NO_CODE.ToString().Trim(),"YearCode="+fundDetails.MAST_YEAR.ToString().Trim(),"ReleaseType="+fundDetails.MAST_RELEASE_TYPE.ToString().Trim(),"FundType=" +fundDetails.MAST_FUND_TYPE.ToString().Trim(),"FundingAgency="+fundDetails.MAST_FUNDING_AGENCY_CODE.ToString().Trim()})+"'); return false;'>Delete</a>":"<span class='ui-icon ui-icon-locked ui-align-center'></span>"):"<span class='ui-icon ui-icon-locked ui-align-center'></span>",
                                        //(fundDetails.MAST_TRANSACTION_NO >= dbContext.MRD_FUND_RELEASE.Where(m=>m.MAST_YEAR == fundDetails.MAST_YEAR && m.MAST_STATE_CODE == fundDetails.MAST_STATE_CODE && m.MAST_FUND_TYPE == fundDetails.MAST_FUND_TYPE && m.ADMIN_NO_CODE == fundDetails.ADMIN_NO_CODE && m.MAST_FUNDING_AGENCY_CODE == fundDetails.MAST_FUNDING_AGENCY_CODE).OrderByDescending(m=>m.MAST_TRANSACTION_NO).Select(m=>m.MAST_TRANSACTION_NO).FirstOrDefault()) == true?(fundDetails.MAST_RELEASE_TYPE==flag?"<a href='#' title='Click here to delete the Fund Release Details' class='ui-icon ui-icon-trash ui-align-center' onClick=DeleteFundReleaseDetails('" +URLEncrypt.EncryptParameters1(new string[]{"TId="+fundDetails.MAST_TRANSACTION_NO.ToString().Trim(),"StateId=" +fundDetails.MAST_STATE_CODE.ToString().Trim(),"AdminCode="+fundDetails.ADMIN_NO_CODE.ToString().Trim(),"YearCode="+fundDetails.MAST_YEAR.ToString().Trim(),"ReleaseType="+fundDetails.MAST_RELEASE_TYPE.ToString().Trim(),"FundType=" +fundDetails.MAST_FUND_TYPE.ToString().Trim(),"FundingAgency="+fundDetails.MAST_FUNDING_AGENCY_CODE.ToString().Trim()})+"'); return false;'>Delete</a>":"<span class='ui-icon ui-icon-locked ui-align-center'></span>"):"<span class='ui-icon ui-icon-locked ui-align-center'></span>",
                                        PMGSYSession.Current.StateCode == 20 ? "<a href='#' title='Click here to edit the Fund Release Details' class='ui-icon ui-icon-pencil ui-align-center' onClick=EditFundReleaseDetails('" +URLEncrypt.EncryptParameters1(new string[]{"TId="+fundDetails.MAST_TRANSACTION_NO.ToString().Trim(),"StateId=" +fundDetails.MAST_STATE_CODE.ToString().Trim(),"AdminCode="+fundDetails.ADMIN_NO_CODE.ToString().Trim(),"YearCode="+fundDetails.MAST_YEAR.ToString().Trim(),"ReleaseType="+fundDetails.MAST_RELEASE_TYPE.ToString().Trim(),"FundType=" +fundDetails.MAST_FUND_TYPE.ToString().Trim(),"FundingAgency="+fundDetails.MAST_FUNDING_AGENCY_CODE.ToString().Trim()})+"'); return false;'>Delete</a>" : (CheckLatestRelease(fundDetails.MAST_STATE_CODE,fundDetails.ADMIN_NO_CODE,fundDetails.MAST_FUND_TYPE,fundDetails.MAST_FUNDING_AGENCY_CODE,fundDetails.MAST_YEAR,fundDetails.MAST_RELEASE_TYPE,fundDetails.MAST_TRANSACTION_NO)) == true?(fundDetails.MAST_RELEASE_TYPE==flag?"<a href='#' title='Click here to edit the Fund Release Details' class='ui-icon ui-icon-pencil ui-align-center' onClick=EditFundReleaseDetails('" +URLEncrypt.EncryptParameters1(new string[]{"TId="+fundDetails.MAST_TRANSACTION_NO.ToString().Trim(),"StateId=" +fundDetails.MAST_STATE_CODE.ToString().Trim(),"AdminCode="+fundDetails.ADMIN_NO_CODE.ToString().Trim(),"YearCode="+fundDetails.MAST_YEAR.ToString().Trim(),"ReleaseType="+fundDetails.MAST_RELEASE_TYPE.ToString().Trim(),"FundType=" +fundDetails.MAST_FUND_TYPE.ToString().Trim(),"FundingAgency="+fundDetails.MAST_FUNDING_AGENCY_CODE.ToString().Trim()})+"'); return false;'>Delete</a>":"<span class='ui-icon ui-icon-locked ui-align-center'></span>"):"<span class='ui-icon ui-icon-locked ui-align-center'></span>",
                                        (CheckLatestRelease(fundDetails.MAST_STATE_CODE,fundDetails.ADMIN_NO_CODE,fundDetails.MAST_FUND_TYPE,fundDetails.MAST_FUNDING_AGENCY_CODE,fundDetails.MAST_YEAR,fundDetails.MAST_RELEASE_TYPE,fundDetails.MAST_TRANSACTION_NO)) == true?(fundDetails.MAST_RELEASE_TYPE==flag?"<a href='#' title='Click here to delete the Fund Release Details' class='ui-icon ui-icon-trash ui-align-center' onClick=DeleteFundReleaseDetails('" +URLEncrypt.EncryptParameters1(new string[]{"TId="+fundDetails.MAST_TRANSACTION_NO.ToString().Trim(),"StateId=" +fundDetails.MAST_STATE_CODE.ToString().Trim(),"AdminCode="+fundDetails.ADMIN_NO_CODE.ToString().Trim(),"YearCode="+fundDetails.MAST_YEAR.ToString().Trim(),"ReleaseType="+fundDetails.MAST_RELEASE_TYPE.ToString().Trim(),"FundType=" +fundDetails.MAST_FUND_TYPE.ToString().Trim(),"FundingAgency="+fundDetails.MAST_FUNDING_AGENCY_CODE.ToString().Trim()})+"'); return false;'>Delete</a>":"<span class='ui-icon ui-icon-locked ui-align-center'></span>"):"<span class='ui-icon ui-icon-locked ui-align-center'></span>",
                                    }
                }).ToArray();

                return gridData;
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
        /// saves the fund release data
        /// </summary>
        /// <param name="fundModel"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool AddFundRelease(FundReleaseViewModel fundModel, ref string message)
        {
            PMGSYEntities dbContext = null;
            CommonFunctions objCommonFunc = new CommonFunctions();
            try
            {
                int maxCount = 0;
                double? totalRelease = GetFundReleaseTotal(fundModel.MAST_STATE_CODE, fundModel.MAST_YEAR, fundModel.MAST_FUND_TYPE, fundModel.ADMIN_NO_CODE, fundModel.MAST_RELEASE_TYPE, fundModel.MAST_FUNDING_AGENCY_CODE);
                double? totalAllocation = GetFundAllocationTotal(fundModel.MAST_STATE_CODE, fundModel.MAST_YEAR, fundModel.MAST_FUND_TYPE, fundModel.ADMIN_NO_CODE, fundModel.MAST_RELEASE_TYPE, fundModel.MAST_FUNDING_AGENCY_CODE);
                totalRelease = Convert.ToDouble(fundModel.MAST_RELEASE_AMOUNT) + totalRelease;
                if (fundModel.MAST_RELEASE_TYPE == "C")
                {
                    if (totalRelease > totalAllocation)
                    {
                        //message = "Release Amount is greater than Allocation Amount.";
                        //return false;
                    }
                }

                MRD_FUND_RELEASE master = new MRD_FUND_RELEASE();
                master.MAST_RELEASE_TYPE = (fundModel.MAST_RELEASE_TYPE == null ? "C" : fundModel.MAST_RELEASE_TYPE);
                if (fundModel.MAST_RELEASE_DATE != null)
                {
                    master.MAST_RELEASE_DATE = objCommonFunc.GetStringToDateTime(fundModel.MAST_RELEASE_DATE);
                }

                dbContext = new PMGSYEntities();

                maxCount = dbContext.MRD_FUND_RELEASE.Where(m => m.MAST_STATE_CODE == fundModel.MAST_STATE_CODE && m.ADMIN_NO_CODE == fundModel.ADMIN_NO_CODE && m.MAST_YEAR == fundModel.MAST_YEAR && m.MAST_RELEASE_TYPE == fundModel.MAST_RELEASE_TYPE && m.MAST_FUND_TYPE == fundModel.MAST_FUND_TYPE).OrderByDescending(m => m.MAST_TRANSACTION_NO).Select(m => m.MAST_TRANSACTION_NO).FirstOrDefault();
                master.MAST_TRANSACTION_NO = maxCount + 1;
                master.ADMIN_NO_CODE = fundModel.ADMIN_NO_CODE;
                master.MAST_FUND_TYPE = fundModel.MAST_FUND_TYPE;
                master.MAST_FUNDING_AGENCY_CODE = fundModel.MAST_FUNDING_AGENCY_CODE;
                master.MAST_RELEASE_AMOUNT = fundModel.MAST_RELEASE_AMOUNT;

                master.MAST_RELEASE_FILE = fundModel.MAST_RELEASE_FILE;
                master.MAST_RELEASE_ORDER = fundModel.MAST_RELEASE_ORDER;
                master.MAST_RELEASE_YEAR = fundModel.MAST_RELEASE_YEAR;
                master.MAST_STATE_CODE = fundModel.MAST_STATE_CODE;
                master.MAST_YEAR = fundModel.MAST_YEAR;


                //added by abhishek kamble 27-nov-2013
                master.USERID = PMGSYSession.Current.UserId;
                master.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];


                dbContext.MRD_FUND_RELEASE.Add(master);
                dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// deletes the fund release data
        /// </summary>
        /// <param name="transactionCode"></param>
        /// <param name="stateCode"></param>
        /// <param name="adminCode"></param>
        /// <param name="yearCode"></param>
        /// <param name="releaseType"></param>
        /// <param name="fundType"></param>
        /// <param name="fundingAgencyCode"></param>
        /// <returns></returns>
        public bool DeleteFundRelease(int transactionCode, int stateCode, int adminCode, int yearCode, string releaseType, string fundType, int fundingAgencyCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                MRD_FUND_RELEASE master = dbContext.MRD_FUND_RELEASE.Where(m => m.MAST_TRANSACTION_NO == transactionCode && m.MAST_STATE_CODE == stateCode && m.ADMIN_NO_CODE == adminCode && m.MAST_YEAR == yearCode && m.MAST_RELEASE_TYPE == releaseType && m.MAST_FUND_TYPE == fundType && m.MAST_FUNDING_AGENCY_CODE == fundingAgencyCode).FirstOrDefault();

                //added by abhishek kamble 27-nov-2013
                master.USERID = PMGSYSession.Current.UserId;
                master.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                dbContext.Entry(master).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                dbContext.MRD_FUND_RELEASE.Remove(master);
                dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// returns the fund release data for updating
        /// </summary>
        /// <param name="transactionCode"></param>
        /// <param name="stateCode"></param>
        /// <param name="adminCode"></param>
        /// <param name="yearCode"></param>
        /// <param name="releaseType"></param>
        /// <param name="fundType"></param>
        /// <param name="fundingAgencyCode"></param>
        /// <returns></returns>
        public FundReleaseViewModel GetFundReleaseDetails(int transactionCode, int stateCode, int adminCode, int yearCode, string releaseType, string fundType, int fundingAgencyCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                MRD_FUND_RELEASE master = dbContext.MRD_FUND_RELEASE.Where(m => m.MAST_TRANSACTION_NO == transactionCode && m.MAST_STATE_CODE == stateCode && m.ADMIN_NO_CODE == adminCode && m.MAST_YEAR == yearCode && m.MAST_RELEASE_TYPE == releaseType && m.MAST_FUND_TYPE == fundType && m.MAST_FUNDING_AGENCY_CODE == fundingAgencyCode).FirstOrDefault();
                FundReleaseViewModel model = new FundReleaseViewModel();
                if (master != null)
                {
                    model.EncryptedReleaseCode = URLEncrypt.EncryptParameters1(new string[] { "FundCode=" + master.MAST_TRANSACTION_NO.ToString().Trim() });
                    model.ADMIN_NO_CODE = master.ADMIN_NO_CODE;
                    model.MAST_FUND_TYPE = master.MAST_FUND_TYPE;
                    model.MAST_FUNDING_AGENCY_CODE = master.MAST_FUNDING_AGENCY_CODE;
                    model.MAST_RELEASE_AMOUNT = master.MAST_RELEASE_AMOUNT;
                    model.MAST_RELEASE_DATE = (master.MAST_RELEASE_DATE == null ? "" : objCommon.GetDateTimeToString(master.MAST_RELEASE_DATE.Value).ToString());
                    model.MAST_RELEASE_FILE = master.MAST_RELEASE_FILE;
                    model.MAST_RELEASE_ORDER = master.MAST_RELEASE_ORDER;
                    model.MAST_RELEASE_TYPE = master.MAST_RELEASE_TYPE;
                    model.MAST_RELEASE_YEAR = master.MAST_RELEASE_YEAR;
                    model.MAST_STATE_CODE = master.MAST_STATE_CODE;
                    model.MAST_TRANSACTION_NO = master.MAST_TRANSACTION_NO;
                    model.MAST_YEAR = master.MAST_YEAR;

                    if (PMGSYSession.Current.RoleCode == roleCode)
                    {
                        model.TotalAvailable = (from item in dbContext.MRD_FUND_ALLOCATION where item.MAST_STATE_CODE == stateCode && item.MAST_FUND_TYPE == fundType && item.MAST_YEAR == yearCode && item.ADMIN_NO_CODE == adminCode && item.MAST_FUNDING_AGENCY_CODE == fundingAgencyCode select (Decimal?)item.MAST_ALLOCATION_AMOUNT).Sum();
                        model.TotalRelease = (dbContext.MRD_FUND_RELEASE.Where(m => m.MAST_YEAR == yearCode && m.MAST_STATE_CODE == stateCode && m.MAST_FUND_TYPE == fundType && m.MAST_FUNDING_AGENCY_CODE == fundingAgencyCode && m.ADMIN_NO_CODE == adminCode && m.MAST_RELEASE_TYPE == "C").Sum(m => (Decimal?)m.MAST_RELEASE_AMOUNT) == null ? 0 : dbContext.MRD_FUND_RELEASE.Where(m => m.MAST_YEAR == yearCode && m.MAST_STATE_CODE == stateCode && m.MAST_FUND_TYPE == fundType && m.MAST_FUNDING_AGENCY_CODE == fundingAgencyCode && m.ADMIN_NO_CODE == adminCode && m.MAST_RELEASE_TYPE == "C").Sum(m => m.MAST_RELEASE_AMOUNT));
                        //model.TotalRelease = model.TotalRelease - master.MAST_RELEASE_AMOUNT;
                        model.TotalAvailable = (model.TotalAvailable - model.TotalRelease) + master.MAST_RELEASE_AMOUNT;
                        model.TotalAllocation = dbContext.MRD_FUND_ALLOCATION.Where(m => m.MAST_YEAR == yearCode && m.MAST_STATE_CODE == stateCode && m.MAST_FUND_TYPE == fundType && m.MAST_FUNDING_AGENCY_CODE == fundingAgencyCode && m.ADMIN_NO_CODE == adminCode).Sum(m => (Decimal?)m.MAST_ALLOCATION_AMOUNT);
                    }
                }
                return model;
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
        /// returns total fund allocation amount
        /// </summary>
        /// <param name="stateCode"></param>
        /// <param name="yearCode"></param>
        /// <param name="fundType"></param>
        /// <param name="executingAgencyCode"></param>
        /// <param name="releaseType"></param>
        /// <param name="agencyCode"></param>
        /// <returns></returns>
        public double? GetFundAllocationTotal(int stateCode, int yearCode, string fundType, int executingAgencyCode, string releaseType, int agencyCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                double totalAllocation = 0;
                var allocationList = (from item in dbContext.MRD_FUND_ALLOCATION
                                      where
                                      item.MAST_STATE_CODE == stateCode &&
                                      item.MAST_YEAR == yearCode &&
                                      item.MAST_FUND_TYPE == fundType &&
                                      item.ADMIN_NO_CODE == executingAgencyCode &&
                                      item.MAST_FUNDING_AGENCY_CODE == agencyCode
                                      select new { item.MAST_ALLOCATION_AMOUNT });

                foreach (var item in allocationList)
                {
                    totalAllocation = totalAllocation + Convert.ToDouble(item.MAST_ALLOCATION_AMOUNT);
                }
                return totalAllocation;

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
        /// returns the total fund release total
        /// </summary>
        /// <param name="stateCode"></param>
        /// <param name="yearCode"></param>
        /// <param name="fundType"></param>
        /// <param name="executingAgencyCode"></param>
        /// <param name="releaseType"></param>
        /// <param name="agencyCode"></param>
        /// <returns></returns>
        public double? GetFundReleaseTotal(int stateCode, int yearCode, string fundType, int executingAgencyCode, string releaseType, int agencyCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                double totalAllocation = 0;
                var allocationList = (from item in dbContext.MRD_FUND_RELEASE
                                      where
                                      item.MAST_STATE_CODE == stateCode &&
                                      item.MAST_YEAR == yearCode &&
                                      item.MAST_FUND_TYPE == fundType &&
                                      item.ADMIN_NO_CODE == executingAgencyCode &&
                                      item.MAST_FUNDING_AGENCY_CODE == agencyCode &&
                                      item.MAST_RELEASE_TYPE == releaseType
                                      select new { item.MAST_RELEASE_AMOUNT });

                foreach (var item in allocationList)
                {
                    totalAllocation = totalAllocation + Convert.ToDouble(item.MAST_RELEASE_AMOUNT);
                }
                return totalAllocation;

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
        /// updates the fund release data
        /// </summary>
        /// <param name="fundModel"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool EditFundRelease(FundReleaseViewModel fundModel, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                int fundCode = 0;
                encryptedParameters = fundModel.EncryptedReleaseCode.Split('/');
                if (!(encryptedParameters.Length == 3))
                {
                    return false;
                }

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                fundCode = Convert.ToInt32(decryptedParameters["FundCode"].ToString());

                int recordCount = 0;//dbContext.MRD_FUND_RELEASE.Where(m => m.MAST_STATE_CODE == fundModel.MAST_STATE_CODE && m.MAST_YEAR == fundModel.MAST_YEAR && m.ADMIN_NO_CODE == fundModel.ADMIN_NO_CODE && m.MAST_RELEASE_TYPE == fundModel.MAST_RELEASE_TYPE && m.MAST_TRANSACTION_NO != fundCode).Count();
                if (recordCount > 0)
                {
                    message = "Fund Release information already exist.";
                    return false;
                }

                MRD_FUND_RELEASE master = dbContext.MRD_FUND_RELEASE.Where(m => m.MAST_TRANSACTION_NO == fundCode && m.MAST_STATE_CODE == fundModel.MAST_STATE_CODE && m.MAST_YEAR == fundModel.MAST_YEAR && m.ADMIN_NO_CODE == fundModel.ADMIN_NO_CODE && m.MAST_RELEASE_TYPE == fundModel.MAST_RELEASE_TYPE && m.MAST_FUND_TYPE == fundModel.MAST_FUND_TYPE).FirstOrDefault();
                if (master != null)
                {
                    master.ADMIN_NO_CODE = fundModel.ADMIN_NO_CODE;
                    master.MAST_FUND_TYPE = fundModel.MAST_FUND_TYPE;
                    master.MAST_FUNDING_AGENCY_CODE = fundModel.MAST_FUNDING_AGENCY_CODE;
                    master.MAST_RELEASE_AMOUNT = fundModel.MAST_RELEASE_AMOUNT;
                    if (fundModel.MAST_RELEASE_DATE != null)
                    {
                        master.MAST_RELEASE_DATE = new CommonFunctions().GetStringToDateTime(fundModel.MAST_RELEASE_DATE);
                    }
                    master.MAST_RELEASE_FILE = fundModel.MAST_RELEASE_FILE;
                    master.MAST_RELEASE_ORDER = fundModel.MAST_RELEASE_ORDER;
                    master.MAST_RELEASE_TYPE = fundModel.MAST_RELEASE_TYPE;
                    master.MAST_RELEASE_YEAR = fundModel.MAST_RELEASE_YEAR;
                    master.MAST_STATE_CODE = fundModel.MAST_STATE_CODE;
                    master.MAST_YEAR = fundModel.MAST_YEAR;

                    //added by abhishek kamble 27-nov-2013
                    master.USERID = PMGSYSession.Current.UserId;
                    master.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    dbContext.Entry(master).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// returns the remaining fund allocation data
        /// </summary>
        /// <param name="stateCode"></param>
        /// <param name="yearCode"></param>
        /// <param name="fundType"></param>
        /// <param name="executingAgencyCode"></param>
        /// <param name="agencyCode"></param>
        /// <returns></returns>
        public double? GetRemainingFundAllocationTotal(int stateCode, int yearCode, string fundType, int executingAgencyCode, int agencyCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            double availableAmount = 0;
            try
            {
                var availableAmountList = (from item in dbContext.MRD_FUND_RELEASE
                                           where
                                           item.MAST_STATE_CODE == stateCode &&
                                           item.MAST_YEAR == yearCode &&
                                           item.MAST_FUND_TYPE == fundType &&
                                           item.ADMIN_NO_CODE == executingAgencyCode &&
                                           item.MAST_FUNDING_AGENCY_CODE == agencyCode
                                           select new { item.MAST_RELEASE_AMOUNT });

                foreach (var item in availableAmountList)
                {
                    availableAmount = availableAmount + Convert.ToDouble(item.MAST_RELEASE_AMOUNT);
                }
                return availableAmount;
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
        /// adds the file details to the fund release
        /// </summary>
        /// <param name="fundModel"></param>
        /// <returns></returns>
        public bool AddFileUploadToTransactionRelease(FundReleaseViewModel fundModel)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                MRD_FUND_RELEASE master = dbContext.MRD_FUND_RELEASE.Where(m => m.ADMIN_NO_CODE == fundModel.ADMIN_NO_CODE && m.MAST_FUND_TYPE == fundModel.MAST_FUND_TYPE && m.MAST_FUNDING_AGENCY_CODE == fundModel.MAST_FUNDING_AGENCY_CODE && m.MAST_STATE_CODE == fundModel.MAST_STATE_CODE && m.MAST_TRANSACTION_NO == fundModel.MAST_TRANSACTION_NO && m.MAST_YEAR == fundModel.MAST_YEAR && m.MAST_RELEASE_TYPE == fundModel.MAST_RELEASE_TYPE).FirstOrDefault();
                master.MAST_RELEASE_FILE = fundModel.name;

                //added by abhishek kamble 27-nov-2013
                master.USERID = PMGSYSession.Current.UserId;
                master.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                dbContext.Entry(master).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// list of files associated with the Release Amount
        /// </summary>
        /// <param name="page">no. of pages of list</param>
        /// <param name="rows">no. of rows</param>
        /// <param name="sidx">sort column name</param>
        /// <param name="sord">sort order</param>
        /// <param name="totalRecords">total no. of records</param>
        /// <param name="stateCode">state id</param>
        /// <param name="adminCode">nodal officer id</param>
        /// <param name="fundType">type of fund</param>
        /// <param name="agencyCode">id of funding agency</param>
        /// <param name="yearCode">id of year</param>
        /// <param name="transactionCode">transaction id</param>
        /// <param name="releaseType">type of releaser(s,c)</param>
        /// <returns></returns>
        public Array GetListFundReleaseFilesDAL(int page, int rows, string sidx, string sord, out long totalRecords, int stateCode, int adminCode, string fundType, int agencyCode, int yearCode, int transactionCode, string releaseType)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                var lstFiles = (from item in dbContext.MRD_FUND_RELEASE
                                where item.MAST_STATE_CODE == stateCode &&
                                item.ADMIN_NO_CODE == adminCode &&
                                item.MAST_FUND_TYPE == fundType &&
                                item.MAST_FUNDING_AGENCY_CODE == agencyCode &&
                                item.MAST_YEAR == yearCode &&
                                item.MAST_TRANSACTION_NO == transactionCode &&
                                item.MAST_RELEASE_TYPE == releaseType
                                select new
                                {
                                    item.MAST_RELEASE_FILE,
                                    item.MAST_TRANSACTION_NO,
                                    item.MAST_STATE_CODE,
                                    item.MAST_FUNDING_AGENCY_CODE,
                                    item.MAST_FUND_TYPE,
                                    item.MAST_YEAR,
                                    item.ADMIN_NO_CODE
                                });

                if (lstFiles.Any((m => m.MAST_RELEASE_FILE == null)))
                {
                    totalRecords = 0;
                }
                else
                {
                    totalRecords = 1;
                }

                var gridData = lstFiles.Select(files => new
                {
                    files.MAST_RELEASE_FILE,
                    files.ADMIN_NO_CODE,
                    files.MAST_FUND_TYPE,
                    files.MAST_FUNDING_AGENCY_CODE,
                    files.MAST_STATE_CODE,
                    files.MAST_TRANSACTION_NO,
                    files.MAST_YEAR
                }).ToArray();

                return gridData.Select(m => new
                {
                    cell = new[]
                    {
                        URLEncrypt.EncryptParameters1(new string[]{m.MAST_RELEASE_FILE.ToString().Trim()}),
                        "<a href='#' title='Click here to delete the File' class='ui-icon ui-icon-trash ui-align-center' onClick=DeleteFundReleaseFile('" +URLEncrypt.EncryptParameters1(new string[]{"FileName="+m.MAST_RELEASE_FILE.ToString().Trim(),"State="+m.MAST_STATE_CODE.ToString().Trim()})+"'); return false;'>Delete</a>",
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
        /// deletes the file associated with the release transaction
        /// </summary>
        /// <param name="fileName">name of file</param>
        /// <param name="stateCode">id of state</param>
        /// <param name="message">response message</param>
        /// <returns></returns>
        public bool DeleteFundReleaseFile(string fileName, int stateCode, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                string[] strFiles = fileName.Split('-');
                int yearCode = Convert.ToInt32(strFiles[0]);
                int transactionCode = Convert.ToInt32(strFiles[1]);
                string fundType = strFiles[2];
                string releaseType = strFiles[3];
                MRD_FUND_RELEASE masterFundRelease = dbContext.MRD_FUND_RELEASE.Where(m => m.MAST_STATE_CODE == stateCode && m.MAST_TRANSACTION_NO == transactionCode && m.MAST_YEAR == yearCode && m.MAST_RELEASE_TYPE == releaseType && m.MAST_FUND_TYPE == fundType).FirstOrDefault();
                masterFundRelease.MAST_RELEASE_FILE = null;

                //added by abhishek kamble 27-nov-2013
                masterFundRelease.USERID = PMGSYSession.Current.UserId;
                masterFundRelease.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];


                dbContext.Entry(masterFundRelease).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                message = "File deleted successfully.";
                return true;

            }
            catch (DbUpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "File is in use and can not be deleted.";
                return false;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "File is in use and can not be deleted.";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// calculates the total allocation
        /// </summary>
        /// <param name="stateCode">id of state</param>
        /// <param name="yearCode">id of year</param>
        /// <param name="fundType">type of fund</param>
        /// <param name="adminCode">id of nodal officer</param>
        /// <param name="agencyCode">id of funding agency</param>
        /// <returns></returns>
        public decimal? GetTotalAllocation(int stateCode, int yearCode, string fundType, int adminCode, int agencyCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                return dbContext.MRD_FUND_ALLOCATION.Where(m => m.MAST_STATE_CODE == stateCode && m.MAST_YEAR == yearCode && m.MAST_FUND_TYPE == fundType && m.MAST_FUNDING_AGENCY_CODE == agencyCode && m.ADMIN_NO_CODE == adminCode).Sum(m => m.MAST_ALLOCATION_AMOUNT);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
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

        public bool CheckLatestRelease(int stateCode, int adminCode, string fundType, int fundingAgencyCode, int yearCode, string releaseType, int transanction)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                int maxTransactionNo = dbContext.MRD_FUND_RELEASE.Where(m => m.MAST_STATE_CODE == stateCode && m.MAST_FUND_TYPE == fundType && m.ADMIN_NO_CODE == adminCode && m.MAST_FUNDING_AGENCY_CODE == fundingAgencyCode && m.MAST_RELEASE_TYPE == releaseType && m.MAST_YEAR == yearCode).OrderByDescending(m => m.MAST_TRANSACTION_NO).Select(m => m.MAST_TRANSACTION_NO).FirstOrDefault();
                if (maxTransactionNo == transanction)
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
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
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


        #endregion



    }

    public interface IFundDAL
    {
        #region FUND_ALLOCATION


        Array GetFundAllocationList(int stateCode, string fundType, int fundingAgencyCode, int yearCode, int page, int rows, string sidx, string sord, out long totalRecords);
        bool AddFundAllocation(FundAllocationViewModel fundModel, ref string message);
        bool DeleteFundAllocation(int transactionCode, int stateCode, int adminCode, int yearCode, string fundType, int fundingAgencyCode, ref string message);
        FundAllocationViewModel GetFundAllocationDetails(int transactionCode, int stateCode, int adminCode, int yearCode, string fundType, int fundingAgencyCode);
        bool EditFundAllocation(FundAllocationViewModel fundModel, ref string message);
        bool AddFileUploadToTransaction(FundAllocationViewModel fundModel);

        #endregion

        #region FUND_RELEASE


        Array GetFundReleaseList(int stateCode, string fundType, int fundingAgencyCode, int yearCode, string releaser, int page, int rows, string sidx, string sord, out long totalRecords);
        bool AddFundRelease(FundReleaseViewModel fundModel, ref string message);
        bool DeleteFundRelease(int transactionCode, int stateCode, int adminCode, int yearCode, string releaseType, string fundType, int fundingAgencyCode);
        FundReleaseViewModel GetFundReleaseDetails(int transactionCode, int stateCode, int adminCode, int yearCode, string releaseType, string fundType, int fundingAgencyCode);
        bool EditFundRelease(FundReleaseViewModel fundModel, ref string message);
        bool AddFileUploadToTransactionRelease(FundReleaseViewModel fundModel);
        Array GetListFilesDAL(int page, int rows, string sidx, string sord, out long totalRecords, int stateCode, int adminCode, string fundType, int agencyCode, int yearCode, int transactionCode);
        Array GetListFundReleaseFilesDAL(int page, int rows, string sidx, string sord, out long totalRecords, int stateCode, int adminCode, string fundType, int agencyCode, int yearCode, int transactionCode, string releaseType);
        bool DeleteFundReleaseFile(string fileName, int stateCode, ref string message);
        decimal? GetTotalAllocation(int stateCode, int yearCode, string fundType, int agencyCode, int adminCode);
        #endregion



    }
}