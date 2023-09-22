using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.Models;
using PMGSY.Models.Accounts;
using PMGSY.Common;
using PMGSY.Extensions;
using System.Web.Mvc;
using System.Transactions;
using System.Configuration;
using System.IO;

namespace PMGSY.DAL.Accounts
{
    public class AccountsDAL : IAccountsDAL
    {
        PMGSYEntities dbContext = null;
        CommonFunctions objCommonFunc = null;

        public Array GetPFAuthorizationList(AccountsFilterModel objFilter, out long totalRecords)
        {
            try
            {
                dbContext = new PMGSYEntities();
                objCommonFunc = new CommonFunctions();
 
                List<AuthorizationList> lstMasterList = new List<AuthorizationList>();

                lstMasterList = (from m in dbContext.ACC_AUTH_REQUEST_MASTER
                                 //from t in dbContext.ACC_AUTH_REQUEST_TRACKING.Where(p=>p.AUTH_ID == m.AUTH_ID && (p.AUTH_STATUS != "F" || p.AUTH_STATUS != "P")).DefaultIfEmpty()
                                 from t in dbContext.ACC_AUTH_REQUEST_TRACKING.Where(p => p.AUTH_ID == m.AUTH_ID && (p.AUTH_STATUS != "F")).DefaultIfEmpty()
                                 where
                                 m.ADMIN_ND_CODE == objFilter.AdminNdCode && m.LVL_ID == objFilter.LevelId
                                 //&& m.FUND_TYPE == objFilter.FundType && m.CURRENT_AUTH_STATUS == t.AUTH_STATUS && m.CURRENT_AUTH_STATUS != "F" && m.CURRENT_AUTH_STATUS != "P"
                                 && m.FUND_TYPE == objFilter.FundType && m.CURRENT_AUTH_STATUS == t.AUTH_STATUS && m.CURRENT_AUTH_STATUS != "F" 
                                 select new AuthorizationList
                                 {
                                     AUTH_ID = m.AUTH_ID,
                                     AUTH_NO = m.AUTH_NO,
                                     AUTH_DATE = m.AUTH_DATE,
                                     CASH_AMOUNT = m.CASH_AMOUNT,
                                     CHQ_AMOUNT = m.CHQ_AMOUNT,
                                     GROSS_AMOUNT = m.GROSS_AMOUNT,
                                     CON_NAME = m.MASTER_CONTRACTOR.MAST_CON_COMPANY_NAME,
                                     CURRENT_AUTH_STATUS = m.CURRENT_AUTH_STATUS,
                                     REJECTION_REMARKS = t.REMARKS,
                                     AUTH_REJECTION_DATE = t.DATE_OF_OPERATION.ToString(),
                                     BILL_ID = t.BILL_ID
                                 }).ToList<AuthorizationList>();


                totalRecords = lstMasterList.Count();

                if (objFilter.sidx.Trim() != string.Empty)
                {
                    if (objFilter.sord.ToString() == "asc")
                    {
                        switch (objFilter.sidx)
                        {
                            case "AuthNumber":
                                lstMasterList = lstMasterList.OrderBy(x => x.AUTH_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<AuthorizationList>();
                                break;
                            case "AuthDate":
                                lstMasterList = lstMasterList.OrderBy(x => x.AUTH_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<AuthorizationList>();
                                break;
                            case "Amount":
                                lstMasterList = lstMasterList.OrderBy(x => x.GROSS_AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<AuthorizationList>();
                                break;                           
                            default:
                                lstMasterList = lstMasterList.OrderBy(x => x.AUTH_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<AuthorizationList>();
                                break;
                        }

                    }
                    else
                    {
                        switch (objFilter.sidx)
                        {
                            case "AuthNumber":
                                lstMasterList = lstMasterList.OrderByDescending(x => x.AUTH_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<AuthorizationList>();
                                break;
                            case "AuthDate":
                                lstMasterList = lstMasterList.OrderByDescending(x => x.AUTH_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<AuthorizationList>();
                                break;
                            case "Amount":
                                lstMasterList = lstMasterList.OrderByDescending(x => x.GROSS_AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<AuthorizationList>();
                                break;
                            default:
                                lstMasterList = lstMasterList.OrderByDescending(x => x.AUTH_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<AuthorizationList>();
                                break;
                        }
                    }
                }
                else
                {
                    lstMasterList = lstMasterList.OrderByDescending(x => x.AUTH_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<AuthorizationList>();
                }

                return lstMasterList.Select(item => new
                {

                    id = URLEncrypt.EncryptParameters(new string[] { item.AUTH_ID.ToString().Trim() }),
                    cell = new[] {                         
                                    
                                    item.AUTH_NO,
                                    Convert.ToDateTime(item.AUTH_DATE).ToString("dd/MM/yyyy"),
                                    item.AUTH_REJECTION_DATE,
                                    "<span class='ui-qtp-dig' title='<table><tr><td><font color=#1C94C4>Payment Amount:</font></td><td>" +item.CHQ_AMOUNT+ "</td></tr><tr><td><font color=#1C94C4>Deduction Amount:</font></td><td>"+item.CASH_AMOUNT+"</td></tr></table>'>"+item.GROSS_AMOUNT.ToString()+"</span>",
                                    PMGSYSession.Current.RoleCode == 21 
                                        ? ( item.CURRENT_AUTH_STATUS == "A" 
                                            ? "<center><a href='#' class='ui-icon ui-icon-circle-plus' onclick='AddAuthDetails(\"" +URLEncrypt.EncryptParameters(new string[] { item.AUTH_ID.ToString()+"_0" })+ "\");return false;'>Add Authorization Details</a></center>" 
                                            : item.CURRENT_AUTH_STATUS == "R" 
                                                ? "<center><a href='#' class='ui-icon ui-icon-circle-plus' onclick='AddAuthDetails(\"" +URLEncrypt.EncryptParameters(new string[] { item.AUTH_ID.ToString()+"_"+item.BILL_ID })+ "\");return false;'>Add Payment Details</a></center>" 
                                                    : item.CURRENT_AUTH_STATUS == "P" 
                                                    ? "<center><a href='#' class='ui-icon ui-icon-search ui-qtp-dig-bill-details' title='<table><tr><td><font color=#1C94C4>Bill No.:</font></td><td>" +  dbContext.ACC_BILL_MASTER.Where(c => c.BILL_ID == item.BILL_ID).Select(c => c.BILL_NO).FirstOrDefault()  + "</td></tr><tr><td><font color=#1C94C4>Date:</font></td><td>"+dbContext.ACC_BILL_MASTER.Where(c => c.BILL_ID == item.BILL_ID).Select(c => c.BILL_DATE).FirstOrDefault().ToString("dd/MM/yyyy")+"</td></tr></table>'>Bill Details</a></center>" 
                                                        : item.CURRENT_AUTH_STATUS == "C" 
                                                            ? "<center><a href='#' class='ui-icon ui-icon-circle-close ui-qtp-dig' title='<table><tr><td><font color=#1C94C4>Remarks:</font></td><td>" +item.REJECTION_REMARKS+ "</td></tr><tr><td><font color=#1C94C4>Date:</font></td><td>"+Convert.ToDateTime(item.AUTH_REJECTION_DATE).ToString("dd/MM/yyyy")+"</td></tr></table>'>View Rejection Details</a></center>" 
                                                            : "<center><a href='#' class='ui-icon ui-icon-search' onclick='ViewAuthDetails(\"" +URLEncrypt.EncryptParameters(new string[] { item.AUTH_ID.ToString()+"_"+item.BILL_ID })+ "\");return false;'>View Details</a></center>"
                                          )
                                       
                                        :  (item.CURRENT_AUTH_STATUS == "R" 
                                                ? "<center><a href='#' class='ui-icon ui-icon-search ui-qtp-dig-receipt-details' title='<table><tr><td><font color=#1C94C4>Receipt No.:</font></td><td>" +  dbContext.ACC_BILL_MASTER.Where(c => c.BILL_ID == item.BILL_ID).Select(c => c.BILL_NO).FirstOrDefault()  + "</td></tr><tr><td><font color=#1C94C4>Receipt Date:</font></td><td>"+dbContext.ACC_BILL_MASTER.Where(c => c.BILL_ID == item.BILL_ID).Select(c => c.BILL_DATE).FirstOrDefault().ToString("dd/MM/yyyy")+"</td></tr></table>'>Receipt Details</a></center>" 
                                                : item.CURRENT_AUTH_STATUS == "P" 
                                                    ? "<center><a href='#' class='ui-icon ui-icon-search ui-qtp-dig-bill-details' title='<table><tr><td><font color=#1C94C4>Bill No.:</font></td><td>" +  dbContext.ACC_BILL_MASTER.Where(c => c.BILL_ID == item.BILL_ID).Select(c => c.BILL_NO).FirstOrDefault()  + "</td></tr><tr><td><font color=#1C94C4>Date:</font></td><td>"+dbContext.ACC_BILL_MASTER.Where(c => c.BILL_ID == item.BILL_ID).Select(c => c.BILL_DATE).FirstOrDefault().ToString("dd/MM/yyyy")+"</td></tr></table>'>Bill Details</a></center>" 
                                                    : item.CURRENT_AUTH_STATUS == "C" 
                                                        ? "<center><a href='#' class='ui-icon ui-icon-search ui-qtp-dig' title='<table><tr><td><font color=#1C94C4>Remarks:</font></td><td>" +item.REJECTION_REMARKS+ "</td></tr><tr><td><font color=#1C94C4>Date:</font></td><td>"+Convert.ToDateTime(item.AUTH_REJECTION_DATE).ToString("dd/MM/yyyy")+"</td></tr></table>'>View Rejection Details</a></center>" 
                                                        : "--"
                                        )
                    }
                }).ToArray();


            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);         
                totalRecords = 0;
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }



        /// <summary>
        /// action to get asset liability kist
        /// </summary>
        /// <param name="objFilter"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array GetAssetliabilityList(AccountsFilterModel objFilter, out long totalRecords)
        {

            try
            {
                dbContext = new PMGSYEntities();
                objCommonFunc = new CommonFunctions();
 
                List<SP_ACC_DB_ASSET_LIABILITIES_MAJOR_DETAILS_Result> resultlist =
                dbContext.SP_ACC_DB_ASSET_LIABILITIES_MAJOR_DETAILS(
                objFilter.FundType,
                objFilter.AdminNdCode,
                objFilter.month,
                objFilter.year,
                objFilter.LevelId, objFilter.lowercode, objFilter.ownLower, objFilter.rptId, objFilter.AssetOrliability
                ).ToList<SP_ACC_DB_ASSET_LIABILITIES_MAJOR_DETAILS_Result>();

                totalRecords = resultlist.Count();

                if (objFilter.sidx.Trim() != string.Empty)
                {
                    if (objFilter.sord.ToString() == "asc")
                    {
                        switch (objFilter.sidx)
                        {
                           
                            default:
                               resultlist = resultlist.OrderBy(x => x.amount).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<SP_ACC_DB_ASSET_LIABILITIES_MAJOR_DETAILS_Result>();
                                break;
                        }

                    }
                    else
                    {
                        switch (objFilter.sidx)
                        {
                                       
                            default:
                                resultlist = resultlist.OrderByDescending(x => x.amount).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<SP_ACC_DB_ASSET_LIABILITIES_MAJOR_DETAILS_Result>();
                                break;
                        }
                    }
                }
                else
                {
                    resultlist = resultlist.OrderByDescending(x => x.amount).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<SP_ACC_DB_ASSET_LIABILITIES_MAJOR_DETAILS_Result>();
                }

                return resultlist.Select(item => new
                {

                    id = URLEncrypt.EncryptParameters(new string[] { item.ITEM_ID.ToString().Trim() }),
                    cell = new[] 
                    {                                                   
                                   item.ITEM_ID.ToString(),
                                   item.ITEM_HEADING,
                                   item.amount.ToString()
                    }
                }).ToArray();


            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);   
                      totalRecords = 0;
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// DAL function to get authorization received details
        /// </summary>
        /// <param name="objFilter"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array GetAuthorizationReceivedList(AccountsFilterModel objFilter, out long totalRecords)
        {

            try
            {
                dbContext = new PMGSYEntities();
                objCommonFunc = new CommonFunctions();

                List<SP_ACC_DB_FUND_AUTH_RECEIVED_DETAILS_Result> resultlist =
               
                dbContext.SP_ACC_DB_FUND_AUTH_RECEIVED_DETAILS(
              
                objFilter.FundType,
                objFilter.AdminNdCode,
                objFilter.month,
                objFilter.year,
                objFilter.LevelId, 
                objFilter.lowercode, 
                objFilter.ownLower 
                ).ToList<SP_ACC_DB_FUND_AUTH_RECEIVED_DETAILS_Result>();

                totalRecords = resultlist.Count();


                if (objFilter.sidx.Trim() != string.Empty)
                {
                    if (objFilter.sord.ToString() == "asc")
                    {
                        switch (objFilter.sidx)
                        {

                            default:
                                resultlist = resultlist.OrderBy(x => x.Receipt_date).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<SP_ACC_DB_FUND_AUTH_RECEIVED_DETAILS_Result>();
                                break;
                        }

                    }
                    else
                    {
                        switch (objFilter.sidx)
                        {

                            default:
                                resultlist = resultlist.OrderByDescending(x => x.Receipt_date).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<SP_ACC_DB_FUND_AUTH_RECEIVED_DETAILS_Result>();
                                break;
                        }
                    }
                }
                else
                {
                    resultlist = resultlist.OrderByDescending(x => x.Receipt_date).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<SP_ACC_DB_FUND_AUTH_RECEIVED_DETAILS_Result>();
                }


                return resultlist.Select(item => new
                {

                    cell = new[] 
                    {                                                   
                                  item.RECEIPT_No==null?string.Empty: item.RECEIPT_No.ToString(),
                                   item.Receipt_date==null?string.Empty: item.Receipt_date,
                                    item.Reference_No==null?string.Empty:item.Reference_No.ToString(),
                                   item.Head_Desc==null?string.Empty:item.Head_Desc.ToString(),
                                   item.amount ==null ? String.Empty : item.amount.ToString()
                    }
                }).ToArray();


            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);   
                      totalRecords = 0;
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        /// <summary>
        /// function to get the lettest 30 transaction of an agency
        /// </summary>
        /// <param name="objFilter"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
         public Array GetLettestTransactionsList(AccountsFilterModel objFilter, out long totalRecords)
         {
            try
            {
                dbContext = new PMGSYEntities();
                objCommonFunc = new CommonFunctions();

                List<SP_ACC_ALERT_LETTEST_TRANSACTION_DETAILS_Result> resultlist =
                dbContext.SP_ACC_ALERT_LETTEST_TRANSACTION_DETAILS(
                objFilter.FundType,
                objFilter.AdminNdCode,
                objFilter.LevelId              
                ).ToList<SP_ACC_ALERT_LETTEST_TRANSACTION_DETAILS_Result>();
                totalRecords = resultlist.Count();
                            
               
                resultlist = resultlist.OrderByDescending(x => x.BILL_ID).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<SP_ACC_ALERT_LETTEST_TRANSACTION_DETAILS_Result>();
                

                return resultlist.Select(item => new
                {
                     id = URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString() }),
                                      
                    cell = new[] 
                    {                                                   
                                   
                                   
                                   item.Voucher_no.ToString(),
                                   item.voucher_date.ToString(),
                                   item.Transaction_type.ToString(),
                                   item.Cheque_no ==null ? String.Empty : item.Cheque_no ,
                                   item.Payee_Name==null ?String.Empty : item.Payee_Name,
                                   item.Amount.ToString()
                                  
                    }
                }).ToArray();


            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);   
      
                totalRecords = 0;
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
         
         }
      /// <summary>
      /// DAL function to get the summary for dashbord
      /// </summary>
      /// <param name="objFilter"></param>
      /// <param name="totalRecords"></param>
      /// <returns></returns>
        public Array GetSummaryList(AccountsFilterModel objFilter, out long totalRecords)
        {

            try
            {
                dbContext = new PMGSYEntities();
                objCommonFunc = new CommonFunctions();

                List<SP_ACC_DB_DISPLAY_SUMMARY_Result> resultlist =

                dbContext.SP_ACC_DB_DISPLAY_SUMMARY(

                objFilter.FundType,
                objFilter.AdminNdCode,
                objFilter.month,
                objFilter.year,
                objFilter.LevelId,
                objFilter.lowercode,
                objFilter.ownLower
                ).ToList<SP_ACC_DB_DISPLAY_SUMMARY_Result>();

                totalRecords = resultlist.Count();


                if (objFilter.sidx.Trim() != string.Empty)
                {
                    if (objFilter.sord.ToString() == "asc")
                    {
                        switch (objFilter.sidx)
                        {

                            default:
                                resultlist = resultlist.OrderBy(x => x.BILL_TYPE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<SP_ACC_DB_DISPLAY_SUMMARY_Result>();
                                break;
                        }

                    }
                    else
                    {
                        switch (objFilter.sidx)
                        {

                            default:
                                resultlist = resultlist.OrderByDescending(x => x.BILL_TYPE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<SP_ACC_DB_DISPLAY_SUMMARY_Result>();
                                break;
                        }
                    }
                }
                else
                {
                    resultlist = resultlist.OrderByDescending(x => x.BILL_TYPE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<SP_ACC_DB_DISPLAY_SUMMARY_Result>();
                }


                return resultlist.Select(item => new
                {

                    cell = new[] 
                    {                                                   
                                   item.BILL_TYPE.ToString(),
                                   item.Desc,
                                   item.Upto_count.HasValue ? item.Upto_count.Value.ToString():string.Empty,
                                   item.Upto_amount.HasValue ? item.Upto_amount.Value.ToString():string.Empty,
                                   item.month_count.HasValue ? item.month_count.Value.ToString() : String.Empty,
                                   item.month_amount.HasValue ? item.month_amount.Value.ToString():string.Empty

                                  
                    }
                }).ToArray();


            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);   
                      totalRecords = 0;
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
       
        
        }



        /// <summary>
        /// DAL function to get the asset liabilities details
        /// </summary>
        /// <param name="objFilter"></param>
        /// <returns></returns>
        public List<SP_ACC_DB_ASSET_LIABILITIES_MAJOR_DETAILS_Result> GetAssetliabilityChart(AccountsFilterModel objFilter)
        {

            try
            {
                dbContext = new PMGSYEntities();
                objCommonFunc = new CommonFunctions();
                

                List<SP_ACC_DB_ASSET_LIABILITIES_MAJOR_DETAILS_Result> resultlist =
                dbContext.SP_ACC_DB_ASSET_LIABILITIES_MAJOR_DETAILS(
                objFilter.FundType,
                objFilter.AdminNdCode,
                objFilter.month,
                objFilter.year,
                objFilter.LevelId, objFilter.lowercode, objFilter.ownLower, objFilter.rptId, objFilter.AssetOrliability
                ).ToList<SP_ACC_DB_ASSET_LIABILITIES_MAJOR_DETAILS_Result>();


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

    
        /// <summary>
        /// function to return  asset liability minor head details
        /// </summary>
        /// <param name="objFilter"></param>
        /// <returns></returns>
         public List<SP_ACC_DB_ASSET_LIABILITIES_MINOR_DETAILS_Result> GetAssetliabilityDetailsChart(AccountsFilterModel objFilter)
         {
             try
             {
                 dbContext = new PMGSYEntities();
                 objCommonFunc = new CommonFunctions();


                 List<SP_ACC_DB_ASSET_LIABILITIES_MINOR_DETAILS_Result> resultlist =
                 dbContext.SP_ACC_DB_ASSET_LIABILITIES_MINOR_DETAILS(
                 objFilter.FundType,
                 objFilter.AdminNdCode,
                 objFilter.month,
                 objFilter.year,
                 objFilter.LevelId, 
                 objFilter.lowercode, 
                 objFilter.ownLower,
                 objFilter.rptId,
                 objFilter.masterHead,
                 objFilter.AssetOrliability
                 ).ToList<SP_ACC_DB_ASSET_LIABILITIES_MINOR_DETAILS_Result>();

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


         /// <summary>
         ///  To get Latest DPIU transaction Details added by koustubh nakate on 20/08/2013
         /// </summary>
         /// <returns>Latest DPIU transaction Details List</returns>

         public Array GetLatestDPIUTransactionsDetailsListDAL(int? page, int? rows, string sidx, string sord, out long totalRecords)
         {
             PMGSYEntities dbContext = new PMGSYEntities();
             try
             {
                 string fundType = string.Empty;
                 int adminNDCode = 0;

                 fundType = PMGSY.Extensions.PMGSYSession.Current.FundType;
                 adminNDCode = PMGSY.Extensions.PMGSYSession.Current.AdminNdCode;


                 var query = dbContext.USP_ACC_ALERT_DPIU_LATEST_TRANSACTION_DETAILS(fundType, adminNDCode).ToList<USP_ACC_ALERT_DPIU_LATEST_TRANSACTION_DETAILS_Result>();


              
                 totalRecords = query == null ? 0 : query.Count();


                 if (sidx.Trim() != string.Empty)
                 {
                     if (sord.ToString() == "asc")
                     {
                         switch (sidx)
                         {
                             case "BillNumber":
                                 query = query.OrderBy(x => x.Bill_No).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                 break;
                             case "BillDate":
                                 query = query.OrderBy(x => x.C_Bill_Date).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                 break;
                             case "Amount":
                                 query = query.OrderBy(x => x.Amount).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                 break;
                             case "DPIUName":
                                 query = query.OrderBy(x => x.PIU_Name).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                 break;                   
                             default:
                                 query = query.OrderBy(x => x.C_Bill_Date).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                 break;

                         }


                     }
                     else
                     {
                         switch (sidx)
                         {
                             case "BillNumber":
                                 query = query.OrderByDescending(x => x.Bill_No).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                 break;
                             case "BillDate":
                                 query = query.OrderByDescending(x => x.C_Bill_Date).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                 break;
                             case "Amount":
                                 query = query.OrderByDescending(x => x.Amount).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                 break;
                             case "DPIUName":
                                 query = query.OrderByDescending(x => x.PIU_Name).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                 break;
                             default:
                                 query = query.OrderByDescending(x => x.C_Bill_Date).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                 break;
                         }

                     }
                 }
                 else
                 {
                     query = query = query.OrderByDescending(x => x.C_Bill_Date).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                 }

                 var result = query.Select(transactionDetails => new
                 {
                     transactionDetails.Bill_No,
                     transactionDetails.C_Bill_Date,
                     transactionDetails.Cheque_Epay,
                     transactionDetails.Amount,
                     transactionDetails.Txn_Desc,
                     transactionDetails.PIU_Name

                 }).ToArray();

                 return result.Select(transactionDetails => new
                 {

                     cell = new[] {                                                                               
                                     transactionDetails.Bill_No.ToString(),
                                     //Commented by Abhishek kamble 28-Feb-2014
                                     //Convert.ToDateTime(transactionDetails.C_Bill_Date).ToString("dd/MM/yyyy"), 
                                     transactionDetails.C_Bill_Date,
                                     transactionDetails.Cheque_Epay,
                                     transactionDetails.Txn_Desc,
                                     transactionDetails.Amount.ToString(), 
                                     transactionDetails.PIU_Name
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
         ///  To get Latest notifications added by koustubh nakate on 21/08/2013 called from HomeController/Index action
         /// </summary>
         /// <returns>Latest Notification List</returns>
         public List<string> GetNotificationsDAL()
         {
             PMGSYEntities dbContext = new PMGSYEntities();
             try
             {
                 string fundType = string.Empty;
                 int adminNDCode = 0;

                 fundType = PMGSY.Extensions.PMGSYSession.Current.FundType;
                 adminNDCode = PMGSY.Extensions.PMGSYSession.Current.AdminNdCode;


                 //List<string> query = null; // dbContext.USP_ACC_ALERT_DISPLAY_NOTIFICATION_DETAILS(fundType, adminNDCode).Select(dn => dn.REQUEST_NARRATION).ToList<string>();
                 //var query;
                 List<string> query = dbContext.USP_ACC_ALERT_DISPLAY_NOTIFICATION_DETAILS(fundType, adminNDCode).ToList<string>();


                 //Avinash_Start
                 List<string> query1 = new List<string>();


                 string ID = string.Empty;
                 string Narration = string.Empty;
                 string EncryptedID_Narration = string.Empty;
                 ACC_NOTIFICATION_DETAILS objDetails = new ACC_NOTIFICATION_DETAILS();

                 foreach (var item in query)
                 {

                     if (item.Contains("EAuthNo")) //EAuthorization Narration
                     {
                         string[] seperator = new string[] { "$e-Authorization" };
                         String[] strArray = item.Split(seperator, StringSplitOptions.None);

                         foreach (var item1 in strArray)
                         {
                             if (item1.Contains("EAuthNo"))
                             {
                                 if (strArray.Length > 1)
                                 {
                                     string NarratioN = strArray[1];
                                     if (NarratioN.Contains("EAuthNo"))
                                     {

                                         ID = strArray[0]; //Details ID
                                         Int32 DetailID = Convert.ToInt32(ID);
                                         objDetails = dbContext.ACC_NOTIFICATION_DETAILS.Where(x => x.DETAIL_ID == DetailID).FirstOrDefault();
                                         if (objDetails.RECEIVER_BILL_ID != null) //null Already Received Bill_ID..then Do Not Add in List
                                         {

                                         }
                                         else
                                         {
                                             Narration = strArray[1]; //narration
                                             string EncrypptedEAuthID = URLEncrypt.EncryptParameters(new string[] { ID.ToString().Trim() });
                                             EncryptedID_Narration = EncrypptedEAuthID + "$" + "e-Authorization" + Narration;
                                             query1.Add(EncryptedID_Narration);
                                         }



                                     }
                                 }
                             }
                         }



                     }
                     else  //Other than EAuthorization Narration
                     {
                         string[] splitArray = item.Split('$');
                         Narration = splitArray[1];
                         query1.Add(Narration);

                     }




                 }

                 query = query1;

                 //Avinash_End

                 return query;

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
         ///  To get DPIU Autherization Issued Details added by koustubh nakate on 23/08/2013
         /// </summary>
         /// <returns>DPIU Autherization Issued Details List</returns>
         /// 
         public Array GetDPIUAutherizationIssuedDetailsListDAL(int page, int rows, string sidx, string sord, out long totalRecords)
         {

             PMGSYEntities dbContext = new PMGSYEntities();
             try
             {
                 string fundType = string.Empty;
                 int adminNDCode = 0;

                 fundType = PMGSY.Extensions.PMGSYSession.Current.FundType;
                 adminNDCode = PMGSY.Extensions.PMGSYSession.Current.AdminNdCode;


                 var query = dbContext.USP_ACC_ALERT_DPIU_Dispaly_Authorization_Issued_Details(fundType, adminNDCode).ToList<USP_ACC_ALERT_DPIU_Dispaly_Authorization_Issued_Details_Result>();



                 totalRecords = query == null ? 0 : query.Count();


                 if (sidx.Trim() != string.Empty)
                 {
                     if (sord.ToString() == "asc")
                     {
                         switch (sidx)
                         {
                             case "BillNumber":
                                 query = query.OrderBy(x => x.BILL_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                 break;
                             case "BillDate":
                                 query = query.OrderBy(x => x.BILL_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                 break;
                             case "Amount":
                                 query = query.OrderBy(x => x.GROSS_AMOUNT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                 break;                        
                             default:
                                 query = query.OrderBy(x => x.BILL_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                 break;

                         }


                     }
                     else
                     {
                         switch (sidx)
                         {
                             case "BillNumber":
                                 query = query.OrderByDescending(x => x.BILL_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                 break;
                             case "BillDate":
                                 query = query.OrderByDescending(x => x.BILL_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                 break;
                             case "Amount":
                                 query = query.OrderByDescending(x => x.GROSS_AMOUNT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                 break;                         
                             default:
                                 query = query.OrderByDescending(x => x.BILL_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                 break;
                         }

                     }
                 }
                 else
                 {
                     query = query = query.OrderByDescending(x => x.BILL_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                 }

                 var result = query.Select(autherizationIssuedDetails => new
                 {
                     autherizationIssuedDetails.BILL_NO,
                     autherizationIssuedDetails.BILL_DATE,
                     autherizationIssuedDetails.GROSS_AMOUNT,
                     autherizationIssuedDetails.HEAD_DESC,
                     autherizationIssuedDetails.BILL_DATE1

                 }).ToArray();


                 return result.Select(autherizationIssuedDetails => new
                 {

                     cell = new[] {                                                                               
                                     autherizationIssuedDetails.BILL_NO.ToString(),
                                     //Convert.ToDateTime(autherizationIssuedDetails.BILL_DATE).ToString("dd/MM/yyyy"),                                 
                                     Convert.ToDateTime(autherizationIssuedDetails.BILL_DATE).ToString("dd-MMM-yy"),                                 
                                     autherizationIssuedDetails.HEAD_DESC,
                                     autherizationIssuedDetails.GROSS_AMOUNT.ToString(), 
                                   //  Convert.ToDateTime(autherizationIssuedDetails.BILL_DATE1).ToString("dd/MM/yyyy"), 
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

         //Added By Abhishek kamble 28-Feb-2014
         public List<SelectListItem> PopulateSRRDA(int id = 0)
         {
             try
             {

                 List<SelectListItem> lstStateSRRDA = new List<SelectListItem>();
                 dbContext = new PMGSYEntities();

                 var StateSrrda =
                 (from adminDept in dbContext.ADMIN_DEPARTMENT
                  join State in dbContext.MASTER_STATE
                  on adminDept.MAST_STATE_CODE equals State.MAST_STATE_CODE
                  where adminDept.MAST_ND_TYPE == "S" &&
                  (id == 0 ? 1 : adminDept.ADMIN_ND_CODE) == (id == 0 ? 1 : id)
                  orderby State.MAST_STATE_NAME
                  select new
                  {
                      adminDept.ADMIN_ND_CODE,
                      adminDept.ADMIN_ND_NAME,
                      State.MAST_STATE_NAME
                  }
                 );


                 foreach (var item in StateSrrda)
                 {
                     lstStateSRRDA.Add(new SelectListItem { Value = item.ADMIN_ND_CODE.ToString().Trim(), Text = item.MAST_STATE_NAME + "(" + item.ADMIN_ND_NAME + ")" });
                 }

                 return lstStateSRRDA;
             }

             catch
             {
                 return null;
             }
             finally {

                 if (dbContext != null)
                 {
                     dbContext.Dispose();
                 }
             }
         }

         #region Account ATR upload(by Pradip Patil 26/05/2017 12:30 PM)

         public Boolean SaveObservationDetailsDAL(ObservationModel model, int state, int agency, int year, out string message)
         {
             int masterObservationId = 0;
             int ChildObservationId = 0;
             dbContext = new PMGSYEntities();
             try
             {
                 ACC_ATR_MASTER masterModel = null;
                 using (TransactionScope ts = new TransactionScope())
                 {

                     //if (state > 0 && agency > 0 && year > 0)  // For NRRDA only [these filter not presetnt for SRRDA]
                     if (model.hdnMasterObId == null)
                     {
                         if (!dbContext.ACC_ATR_MASTER.Any(s => s.MAST_STATE_CODE == state && s.MAST_AGENCY_CODE == agency && s.FIN_YEAR == year))
                         {
                             masterModel = new ACC_ATR_MASTER();
                             masterModel.ACC_ATR_MASTER_ID = dbContext.ACC_ATR_MASTER.Any() ? (dbContext.ACC_ATR_MASTER.Max(x => x.ACC_ATR_MASTER_ID) + 1) : 1;
                             masterModel.FIN_YEAR = year;
                             masterModel.FUND_TYPE = PMGSYSession.Current.FundType;
                             masterModel.MAST_AGENCY_CODE = agency;
                             masterModel.MAST_STATE_CODE = state;
                             masterModel.USERID = PMGSYSession.Current.UserId;
                             masterModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                             dbContext.ACC_ATR_MASTER.Add(masterModel);
                             dbContext.SaveChanges();
                         }
                         else
                         {
                             masterModel = dbContext.ACC_ATR_MASTER.FirstOrDefault(s => s.MAST_STATE_CODE == state && s.MAST_AGENCY_CODE == agency && s.FIN_YEAR == year);
                         }
                     }
                     else
                     {
                         string[] encParam = model.hdnMasterObId.Split('/');
                         Dictionary<String, String> decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encParam[0], encParam[1], encParam[2] });
                         if (decryptedParameters.Count > 0)
                         {
                             ChildObservationId = Convert.ToInt32(decryptedParameters["ObservationId"].ToString());
                             masterObservationId = Convert.ToInt32(decryptedParameters["masterObId"].ToString());
                         }
                     }
                     ACC_ATR_OBSERVATION_DETAILS observationModel = new ACC_ATR_OBSERVATION_DETAILS();

                     observationModel.ACC_ATR_OBSERVATIONS_ID = dbContext.ACC_ATR_OBSERVATION_DETAILS.Any() ? (dbContext.ACC_ATR_OBSERVATION_DETAILS.Max(s => s.ACC_ATR_OBSERVATIONS_ID) + 1) : 1;
                     if (masterModel != null)
                     {
                         observationModel.ACC_ATR_MASTER_ID = masterModel.ACC_ATR_MASTER_ID;
                     }
                     else
                     {
                         observationModel.ACC_ATR_MASTER_ID = masterObservationId;
                     }

                     if (model.hdnMasterObId == null)
                     {
                         observationModel.PARENT_OBSERVATION_ID = null;
                     }
                     else //if (PMGSYSession.Current.RoleCode == 2)
                     {
                         observationModel.PARENT_OBSERVATION_ID = ChildObservationId; //temprary
                     }
                     observationModel.OBSERVATION_SUBJECT = model.Subject;
                     observationModel.OBSERVATIONS = model.Observation;
                     observationModel.OBSERVATION_STATUS = model.NonConforfance ?? "N";
                     observationModel.OBSERVATION_ORDER = dbContext.ACC_ATR_OBSERVATION_DETAILS.Any() ? (dbContext.ACC_ATR_OBSERVATION_DETAILS.Max(s => s.OBSERVATION_ORDER) + 1) : 1;
                     observationModel.OBSERVATION_DATE = DateTime.Now;

                     if (PMGSYSession.Current.RoleCode == 2)
                     {
                         observationModel.OBSERVATION_BY = "S";
                     }
                     else if (PMGSYSession.Current.RoleCode == 21)
                     {
                         observationModel.OBSERVATION_BY = "N";
                     }
                     else if (PMGSYSession.Current.RoleCode == 46)
                     {
                         observationModel.OBSERVATION_BY = "F";
                     }
                     observationModel.IS_ACTIVE_OBSERVATION = true;  //by default true
                     observationModel.USERID = PMGSYSession.Current.UserId;
                     observationModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                     dbContext.ACC_ATR_OBSERVATION_DETAILS.Add(observationModel);

                     dbContext.SaveChanges();
                     ts.Complete();
                     message = "Observation details added successfully";
                     return true; ;
                 }
             }
             catch (System.Data.Entity.Validation.DbEntityValidationException ex)
             {
                 ErrorLog.LogError(ex, "SaveObservationDetailsDAL()");
                 message = "Error ocurred while processing your request";
                 return false;
             }
             finally
             {
                 if (dbContext != null)
                     dbContext.Dispose();
             }
         }

         public Array GetObservationListDAL(int? page, int? rows, string sord, string sidx, out long totalrecords)
         {
             dbContext = new PMGSYEntities();

             try
             {
                 var lstAllrecords = dbContext.ACC_ATR_MASTER.ToList();

                 if (PMGSYSession.Current.RoleCode == 2)
                 {
                     lstAllrecords = lstAllrecords.Where(s => s.MAST_STATE_CODE == PMGSYSession.Current.StateCode).ToList();
                 }
                 if (PMGSYSession.Current.FundType != null)
                 {
                     lstAllrecords = lstAllrecords.Where(s => s.FUND_TYPE == PMGSYSession.Current.FundType).ToList();
                 }
                 totalrecords = lstAllrecords.Count;

                 if (sidx.Trim() != String.Empty)
                 {
                     if (sord == "asc")
                     {
                         switch (sidx)
                         {
                             case "state":
                                 lstAllrecords = lstAllrecords.OrderBy(s => s.MAST_STATE_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                 break;
                             default:
                                 lstAllrecords = lstAllrecords.OrderBy(s => s.ACC_ATR_MASTER_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                 break;
                         }
                     }
                     else
                     {
                         switch (sidx)
                         {
                             case "state":
                                 lstAllrecords = lstAllrecords.OrderByDescending(s => s.MAST_STATE_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                 break;
                             default:
                                 lstAllrecords = lstAllrecords.OrderByDescending(s => s.ACC_ATR_MASTER_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                 break;
                         }
                     }
                 }

                 var result = lstAllrecords.Select(x => new
                 {
                     MasterATRId = x.ACC_ATR_MASTER_ID,
                     StateName = x.MASTER_STATE.MAST_STATE_NAME,
                     StateCode = x.MAST_STATE_CODE,
                     AgencyName = x.MASTER_AGENCY.MAST_AGENCY_NAME,
                     AgencyCode = x.MASTER_AGENCY.MAST_AGENCY_CODE,
                     Year = x.FIN_YEAR,

                 }).ToList();

                 return result.Select(s => new
                 {
                     id = s.MasterATRId,
                     cell = new[]
                  {  
                    s.MasterATRId.ToString(),
                    s.StateName,
                    s.AgencyName,
                    s.Year + "-" +(s.Year +1),
                    dbContext.ACC_ATR_OBSERVATION_DETAILS.Any(x=>x.ACC_ATR_MASTER_ID==s.MasterATRId && x.OBSERVATION_STATUS=="Y")?"<center><a href='#' class='ui-icon ui-icon-locked' title='Conversation is closed.'></a></center>" :"<center><a href='#' class='ui-icon ui-icon-arrowthickstop-1-n  ui-align-center' onclick='UploadFile(\"" +Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(s.StateCode+":"+s.AgencyCode+":"+s.Year+":"+s.MasterATRId))+ "\");return false;'>Upload File</a></center>",
                     "<center><a href='#' class='ui-icon ui-icon-zoomin  ui-align-center' onclick='ViewUploadedFiles(\"" +URLEncrypt.EncryptParameters1(new string[] { "masterObId ="+s.MasterATRId })+ "\");return false;'>View Uploaded Files</a></center>"
                  }
                 }).ToArray();

             }
             catch (Exception ex)
             {
                 ErrorLog.LogError(ex, "GetObservationListDAL");
                 totalrecords = 0;
                 return null;
             }
             finally
             {
                 if (dbContext != null)
                     dbContext.Dispose();
             }
         }

         public Array GetDetailObservationListDAL(int? page, int? rows, string sord, string sidx, out long totalrecords, int id)
         {
             dbContext = new PMGSYEntities();

             try
             {
                 var lstAllrecords = dbContext.ACC_ATR_OBSERVATION_DETAILS.Where(x => x.ACC_ATR_MASTER_ID == id && x.PARENT_OBSERVATION_ID == null).Distinct().ToList();
                 totalrecords = lstAllrecords.Count;

                 if (sidx.Trim() != String.Empty)
                 {
                     if (sord == "asc")
                     {
                         switch (sidx)
                         {
                             case "subject":
                                 lstAllrecords = lstAllrecords.OrderBy(s => s.OBSERVATION_SUBJECT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                 break;
                             default:
                                 lstAllrecords = lstAllrecords.OrderBy(s => s.ACC_ATR_MASTER_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                 break;
                         }
                     }
                     else
                     {
                         switch (sidx)
                         {
                             case "subject":
                                 lstAllrecords = lstAllrecords.OrderByDescending(s => s.OBSERVATION_SUBJECT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                 break;
                             default:
                                 lstAllrecords = lstAllrecords.OrderByDescending(s => s.ACC_ATR_MASTER_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                 break;
                         }
                     }
                 }

                 var result = lstAllrecords.Select(x => new
                 {
                     MasterATRId = x.ACC_ATR_MASTER_ID,
                     ObservationId = x.ACC_ATR_OBSERVATIONS_ID,
                     Subject = x.OBSERVATION_SUBJECT,
                     Observation = x.OBSERVATIONS,
                     ObservationDate = x.OBSERVATION_DATE.ToString("dd/MM/yyyy"),
                 }).ToList();

                 return result.Select(s => new
                 {
                     id = s.ObservationId,
                     cell = new[]
                  {  
                    s.ObservationId.ToString(),
                    s.Subject,
                    s.Observation,
                    s.ObservationDate,
                    dbContext.ACC_ATR_OBSERVATION_DETAILS.Any(x=>x.PARENT_OBSERVATION_ID==s.ObservationId)?"<center><a href='#' class='ui-icon ui-icon-locked' onclick=''></a></center>":"<center><a href='#' class='ui-icon ui-icon-arrowreturnthick-1-w' onclick='ReplyObservation(\"" +URLEncrypt.EncryptParameters1(new string[] { "ObservationId ="+s.ObservationId,"masterObId ="+s.MasterATRId })+ "\");return false;'>Reply Observation</a></center>",
                    //URLEncrypt.EncryptParameters1(new string[] { "ObservationId ="+s.ObservationId,"masterObId ="+s.MasterATRId }),
                    dbContext.ACC_ATR_OBSERVATION_DETAILS.Any(x=>x.PARENT_OBSERVATION_ID==s.ObservationId)?"<center><a href='#' class='ui-icon ui-icon-locked' onclick=''></a></center>":"<center><a href='#' class='ui-icon ui-icon-trash' onclick='DeleteObservation(\"" +URLEncrypt.EncryptParameters1(new string[] { "ObservationId ="+s.ObservationId,"masterObId ="+s.MasterATRId })+ "\");return false;'>Delete Observation</a></center>"
                  }
                 }).ToArray();

             }
             catch (Exception ex)
             {
                 ErrorLog.LogError(ex, "GetDetailObservationListDAL()");
                 totalrecords = 0;
                 return null;
             }
             finally
             {
                 if (dbContext != null)
                     dbContext.Dispose();
             }
         }

         public Array GetObservationReplyListDAL(int? page, int? rows, string sord, string sidx, out long totalrecords, int id)
         {
             dbContext = new PMGSYEntities();

             try
             {
                 ACC_ATR_OBSERVATION_DETAILS model = null;
                 var lstAllParentrecords = dbContext.ACC_ATR_OBSERVATION_DETAILS.Where(x => x.PARENT_OBSERVATION_ID != null).Distinct().ToList();
                 List<ACC_ATR_OBSERVATION_DETAILS> lstAllrecords = new List<ACC_ATR_OBSERVATION_DETAILS>();
                 model = dbContext.ACC_ATR_OBSERVATION_DETAILS.Where(x => x.PARENT_OBSERVATION_ID == id).FirstOrDefault();
                 if (model != null)
                 {
                     lstAllrecords.Add(model);

                     for (int i = 0; i < lstAllParentrecords.Count; i++)
                     {
                         model = dbContext.ACC_ATR_OBSERVATION_DETAILS.FirstOrDefault(s => s.PARENT_OBSERVATION_ID == model.ACC_ATR_OBSERVATIONS_ID);
                         if (model != null)
                         {
                             lstAllrecords.Add(model);
                         }
                         else
                         {
                             break;
                         }
                     }
                 }

                 totalrecords = lstAllrecords.Count;

                 if (sidx.Trim() != String.Empty)
                 {
                     if (sord == "asc")
                     {
                         switch (sidx)
                         {
                             case "subject":
                                 lstAllrecords = lstAllrecords.OrderBy(s => s.OBSERVATION_SUBJECT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                 break;
                             default:
                                 lstAllrecords = lstAllrecords.OrderBy(s => s.ACC_ATR_MASTER_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                 break;
                         }
                     }
                     else
                     {
                         switch (sidx)
                         {
                             case "subject":
                                 lstAllrecords = lstAllrecords.OrderByDescending(s => s.OBSERVATION_SUBJECT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                 break;
                             default:
                                 lstAllrecords = lstAllrecords.OrderByDescending(s => s.ACC_ATR_MASTER_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                 break;
                         }
                     }
                 }

                 var result = lstAllrecords.Select(x => new
                 {
                     MasterATRId = x.ACC_ATR_MASTER_ID,
                     ObservationId = x.ACC_ATR_OBSERVATIONS_ID,
                     Subject = x.OBSERVATION_SUBJECT,
                     Observation = x.OBSERVATIONS,
                     ObservatioBy = x.OBSERVATION_BY,
                     ObservationStatus = x.OBSERVATION_STATUS,
                     ObservationDate = x.OBSERVATION_DATE.ToString("dd/MM/yyyy"),
                     StateCode = x.ACC_ATR_MASTER.MAST_STATE_CODE,
                     AgencyCode = x.ACC_ATR_MASTER.MAST_AGENCY_CODE,
                     Year = x.ACC_ATR_MASTER.FIN_YEAR,
                 }).ToList();

                 return result.Select(s => new
                 {
                     id = s.ObservationId,
                     cell = new[]
                  {  
                    s.ObservationId.ToString(),
                    s.Subject,
                    s.Observation,
                    s.ObservatioBy=="S"?"SRRDA":s.ObservatioBy=="N"?"NRRDA":"-",
                    s.ObservationDate,
                    (PMGSYSession.Current.RoleCode==21 && s.ObservatioBy=="N"&&s.ObservationStatus.Equals("Y",StringComparison.OrdinalIgnoreCase))?"<center><a href='#' class='ui-icon ui-icon-locked' title='Conversation is closed.'></a></center>":(PMGSYSession.Current.RoleCode==21 && s.ObservatioBy=="N")?"<center><a href='#' class='ui-icon ui-icon-locked' onclick=''></a></center>": (PMGSYSession.Current.RoleCode==2 && s.ObservatioBy=="S")?"<center><a href='#' class='ui-icon ui-icon-locked' onclick=''></a></center>":dbContext.ACC_ATR_OBSERVATION_DETAILS.Any(x=>x.PARENT_OBSERVATION_ID==s.ObservationId)?"<center><a href='#' class='ui-icon ui-icon-locked' onclick=''></a></center>":s.ObservationStatus.Equals("Y",StringComparison.OrdinalIgnoreCase)?"<center><a href='#' class='ui-icon ui-icon-locked' title='Conversation is closed.'></a></center>":"<center><a id='replyAnchor' href='#' class='ui-icon ui-icon-arrowreturnthick-1-w' onclick='ReplyObservation(\"" +URLEncrypt.EncryptParameters1(new string[] { "ObservationId ="+s.ObservationId,"masterObId ="+s.MasterATRId })+ "\",\"" + s.ObservationId + "\",event);return false;'>Reply Observation</a></center>",
                   // URLEncrypt.EncryptParameters1(new string[] { "ObservationId ="+s.ObservationId,"masterObId ="+s.MasterATRId }),
                  //(PMGSYSession.Current.RoleCode==21 && s.ObservatioBy=="N")? "<center><a href='#' class='ui-icon ui-icon-trash' onclick='DeleteChildObservation(\"" +URLEncrypt.EncryptParameters1(new string[] { "ObservationId ="+s.ObservationId,"masterObId ="+s.MasterATRId })+ "\");return false;'>Delete Observation</a></center>":(PMGSYSession.Current.RoleCode==2 && s.ObservatioBy=="S")? "<center><a href='#' class='ui-icon ui-icon-trash' onclick='DeleteChildObservation(\"" +URLEncrypt.EncryptParameters1(new string[] { "ObservationId ="+s.ObservationId,"masterObId ="+s.MasterATRId })+ "\");return false;'>Delete Observation</a></center>":"<center><a href='#' class='ui-icon ui-icon-locked' onclick=''></a></center>",
                    (PMGSYSession.Current.RoleCode==21 && s.ObservatioBy=="N")? "<center><a href='#' class='ui-icon ui-icon-trash' onclick='DeleteObservation(\"" +URLEncrypt.EncryptParameters1(new string[] { "ObservationId ="+s.ObservationId,"masterObId ="+s.MasterATRId })+ "\");return false;'>Delete Observation</a></center>":(PMGSYSession.Current.RoleCode==2 && s.ObservatioBy=="S")? "<center><a href='#' class='ui-icon ui-icon-trash' onclick='DeleteObservation(\"" +URLEncrypt.EncryptParameters1(new string[] { "ObservationId ="+s.ObservationId,"masterObId ="+s.MasterATRId })+ "\");return false;'>Delete Observation</a></center>":"<center><a href='#' class='ui-icon ui-icon-locked' onclick=''></a></center>",
                    Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(s.StateCode+":"+s.AgencyCode+":"+s.Year))
                  }
                 }).ToArray();

             }
             catch (Exception ex)
             {
                 ErrorLog.LogError(ex, "GetDetailObservationListDAL()");
                 totalrecords = 0;
                 return null;
             }
             finally
             {
                 if (dbContext != null)
                     dbContext.Dispose();
             }
         }

         public Boolean UploadATRFileDAL(HttpPostedFileBase file, int masterAccId)
         {
             try
             {
                 PMGSYEntities dbContext = new PMGSYEntities();

                 ACC_ATR_UPLOAD_DETAILS AccUpload = new ACC_ATR_UPLOAD_DETAILS();
                 AccUpload.ACC_ATR_UPLOAD_ID = dbContext.ACC_ATR_UPLOAD_DETAILS.Any() ? dbContext.ACC_ATR_UPLOAD_DETAILS.Max(x => x.ACC_ATR_UPLOAD_ID) + 1 : 1;
                 AccUpload.ACC_ATR_MASTER_ID = masterAccId;
                 AccUpload.IS_ACTIVE_OBSERVATION = true;
                 AccUpload.UPLOAD_DATE = DateTime.Now;
                 AccUpload.UPLOAD_BY = "-";
                 if (PMGSYSession.Current.RoleCode == 2)
                 {
                     AccUpload.UPLOAD_BY = "S";
                 }
                 else if (PMGSYSession.Current.RoleCode == 21)
                 {
                     AccUpload.UPLOAD_BY = "N";
                 }
                 else if (PMGSYSession.Current.RoleCode == 46)
                 {
                     AccUpload.UPLOAD_BY = "F";
                 }
                 AccUpload.FILE_PATH = "ATR_" + masterAccId + "_" + AccUpload.UPLOAD_BY + "_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_tt") + ".pdf";
                 AccUpload.UPLOAD_ORDER = dbContext.ACC_ATR_UPLOAD_DETAILS.Any() ? dbContext.ACC_ATR_UPLOAD_DETAILS.Max(x => x.UPLOAD_ORDER) + 1 : 1;
                 AccUpload.USERID = PMGSYSession.Current.UserId;
                 AccUpload.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                 dbContext.ACC_ATR_UPLOAD_DETAILS.Add(AccUpload);
                 int status = dbContext.SaveChanges();
                 if (status > 0)
                 {
                     String BasePath = ConfigurationManager.AppSettings["ATR_UPLOAD_MAIN"].ToString();
                     if (!Directory.Exists(BasePath))
                         Directory.CreateDirectory(BasePath);
                     file.SaveAs(Path.Combine(BasePath, AccUpload.FILE_PATH));
                     return true;
                 }
                 else
                 {
                     return false;
                 }

             }
             catch (Exception ex)
             {
                 ErrorLog.LogError(ex, "UploadATRFileDAL");
                 return false;
             }
             finally
             {
                 if (dbContext != null)
                     dbContext.Dispose();
             }

         }

         public Array GetATRFileListDAL(int? page, int? rows, string sord, string sidx, out long totalrecords, String id, out String filterValues)
         {
             dbContext = new PMGSYEntities();
             Int32 masterObId = 0;
             try
             {
                 Dictionary<String, String> DecrytedParams = URLEncrypt.DecryptParameters1(id.Split('/'));
                 if (DecrytedParams.Count > 0)
                 {
                     masterObId = Convert.ToInt32(DecrytedParams["masterObId"].ToString());
                 }

                 var lstAllrecords = dbContext.ACC_ATR_UPLOAD_DETAILS.Where(x => x.ACC_ATR_MASTER_ID == masterObId).ToList();

                 totalrecords = lstAllrecords.Count;

                 if (sidx.Trim() != String.Empty)
                 {
                     if (sord == "asc")
                     {
                         switch (sidx)
                         {
                             case "UploadDate":
                                 lstAllrecords = lstAllrecords.OrderBy(s => s.UPLOAD_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                 break;
                             default:
                                 lstAllrecords = lstAllrecords.OrderBy(s => s.ACC_ATR_UPLOAD_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                 break;
                         }
                     }
                     else
                     {
                         switch (sidx)
                         {
                             case "UploadDate":
                                 lstAllrecords = lstAllrecords.OrderByDescending(s => s.UPLOAD_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                 break;
                             default:
                                 lstAllrecords = lstAllrecords.OrderByDescending(s => s.ACC_ATR_UPLOAD_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                 break;
                         }
                     }
                 }

                 var result = lstAllrecords.Select(x => new
                 {
                     UploadId = x.ACC_ATR_UPLOAD_ID,
                     MasterATRId = x.ACC_ATR_MASTER_ID,
                     FileName = x.FILE_PATH,
                     UploadedBy = x.UPLOAD_BY == "N" ? "NRRDA" : x.UPLOAD_BY == "S" ? "SRRDA" : x.UPLOAD_BY == "F" ? "Finance" : "-",
                     UploadDate = x.UPLOAD_DATE.ToString("dd/MM/yyyy"),
                     StateCode = dbContext.ACC_ATR_MASTER.FirstOrDefault(s => s.ACC_ATR_MASTER_ID == x.ACC_ATR_MASTER_ID).MAST_STATE_CODE,
                     AgencyCode = dbContext.ACC_ATR_MASTER.FirstOrDefault(s => s.ACC_ATR_MASTER_ID == x.ACC_ATR_MASTER_ID).MAST_AGENCY_CODE,
                     Year = dbContext.ACC_ATR_MASTER.FirstOrDefault(s => s.ACC_ATR_MASTER_ID == x.ACC_ATR_MASTER_ID).FIN_YEAR,
                 }).ToList();

                 ACC_ATR_MASTER master = dbContext.ACC_ATR_MASTER.FirstOrDefault(x => x.ACC_ATR_MASTER_ID == masterObId);
                 filterValues = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(master.MAST_STATE_CODE + ":" + master.MAST_AGENCY_CODE + ":" + master.FIN_YEAR));
                 return result.Select(s => new
                 {
                     id = s.UploadId,
                     cell = new[]
                  {  
                    dbContext.ACC_ATR_MASTER.FirstOrDefault(x=>x.ACC_ATR_MASTER_ID==s.MasterATRId).MASTER_STATE.MAST_STATE_NAME,
                   // s.FileName,
                    s.UploadedBy,
                    s.UploadDate,
                    "<a href='/Accounts/GetATRFile?id="+URLEncrypt.EncryptParameters1(new String[]{"ATRFile="+s.FileName,"UploadId="+s.UploadId})+"' title='Click here to view ATR details' class='ui-icon ui-icon-arrowthickstop-1-s  ui-align-center' target=_blank></a>",
                    (PMGSYSession.Current.RoleCode==21 && s.UploadedBy=="NRRDA")?"<center><a href='#' class='ui-icon ui-icon-trash' onclick='DeleteATRFile(\"" +URLEncrypt.EncryptParameters1(new string[] { "FileName="+s.FileName,"UploadId="+s.UploadId })+ "\");return false;'>Delete File</a></center>": (PMGSYSession.Current.RoleCode==2 && s.UploadedBy=="SRRDA")? "<center><a href='#' class='ui-icon ui-icon-trash' onclick='DeleteATRFile(\"" +URLEncrypt.EncryptParameters1(new string[] { "FileName="+s.FileName,"UploadId="+s.UploadId })+ "\");return false;'>Delete File</a></center>":"<center><a href='#' class='ui-icon ui-icon-locked' onclick=''></a></center>",
                  }
                 }).ToArray();

             }
             catch (Exception ex)
             {
                 ErrorLog.LogError(ex, "GetDetailObservationListDAL()");
                 totalrecords = 0;
                 filterValues = String.Empty;
                 return null;
             }
             finally
             {
                 if (dbContext != null)
                     dbContext.Dispose();
             }

         }

         public Boolean DeleteObservationDAL(int ObservationId, int masterObId, out String message)  //
         {
             try
             {
                 dbContext = new PMGSYEntities();

                 if (dbContext.ACC_ATR_OBSERVATION_DETAILS.Any(x => x.PARENT_OBSERVATION_ID == ObservationId))
                 {
                     message = "Observation cannot be deleted.Conversation is present under this topic";
                     return false;
                 }

                 ACC_ATR_OBSERVATION_DETAILS DelObj = dbContext.ACC_ATR_OBSERVATION_DETAILS.FirstOrDefault(x => x.ACC_ATR_OBSERVATIONS_ID == ObservationId);
                 var lstsiblingObservation = dbContext.ACC_ATR_OBSERVATION_DETAILS.Where(x => x.ACC_ATR_MASTER_ID == DelObj.ACC_ATR_MASTER_ID).ToList();
                 if (lstsiblingObservation.Count == 1)
                 {
                     if (dbContext.ACC_ATR_UPLOAD_DETAILS.Any(x => x.ACC_ATR_MASTER_ID == masterObId))
                     {
                         message = "Observation cannnot be deleted. ATR file is uploaded against this observation";
                         return false;
                     }
                     ACC_ATR_OBSERVATION_DETAILS delObjdetail = dbContext.ACC_ATR_OBSERVATION_DETAILS.Remove(dbContext.ACC_ATR_OBSERVATION_DETAILS.FirstOrDefault(x => x.ACC_ATR_OBSERVATIONS_ID == ObservationId));
                     ACC_ATR_MASTER delobjmaster = dbContext.ACC_ATR_MASTER.Remove(dbContext.ACC_ATR_MASTER.FirstOrDefault(x => x.ACC_ATR_MASTER_ID == masterObId));
                 }
                 else
                 {
                     ACC_ATR_OBSERVATION_DETAILS delObjdetail = dbContext.ACC_ATR_OBSERVATION_DETAILS.Remove(dbContext.ACC_ATR_OBSERVATION_DETAILS.FirstOrDefault(x => x.ACC_ATR_OBSERVATIONS_ID == ObservationId));
                 }
                 dbContext.SaveChanges();

                 message = "Observation deleted successfully.";
                 return true;

             }
             catch (Exception ex)
             {
                 ErrorLog.LogError(ex, "DeleteObservationDAL()");
                 message = "Error ocured while processing your request";
                 return false;
             }
             finally
             {
                 if (dbContext != null)
                     dbContext.Dispose();
             }

         }

         //public Boolean DeleteChildObservationDAL(int ObservationId, int masterObId, out String message)  //[Discussion comments against Topic ]
         //{
         //    try
         //    {
         //        dbContext = new PMGSYEntities();

         //        if (dbContext.ACC_ATR_OBSERVATION_DETAILS.Any(x => x.PARENT_OBSERVATION_ID == ObservationId))
         //        {
         //            message = "Observation cannot be deleted.Conversation is present under this topic";
         //            return false;
         //        }


         //        //if (dbContext.ACC_ATR_UPLOAD_DETAILS.Any(x => x.ACC_ATR_MASTER_ID == masterObId))
         //        //{
         //        //    message = "Observation cannnot be deleted. ATR file is uploaded against this observation";
         //        //    return false;
         //        //}
         //        //else
         //        {
         //            ACC_ATR_OBSERVATION_DETAILS delObjdetail = dbContext.ACC_ATR_OBSERVATION_DETAILS.Remove(dbContext.ACC_ATR_OBSERVATION_DETAILS.FirstOrDefault(x => x.ACC_ATR_OBSERVATIONS_ID == ObservationId));
         //            dbContext.SaveChanges();

         //            message = "Observation deleted successfully.";
         //            return true;
         //        }
         //    }
         //    catch (Exception ex)
         //    {
         //        ErrorLog.LogError(ex, "DeleteObservationDAL()");
         //        message = "Error ocured while processing your request";
         //        return false;
         //    }
         //    finally
         //    { 
         //     if(dbContext!=null)
         //         dbContext.Dispose();
         //    }

         //}

         public Boolean DeleteATRFIleDAL(String FIleName, int UploadId, out String message)
         {
             try
             {
                 dbContext = new PMGSYEntities();
                 String BasePath = ConfigurationManager.AppSettings["ATR_UPLOAD_MAIN"].ToString();
                 ACC_ATR_UPLOAD_DETAILS DelObj = dbContext.ACC_ATR_UPLOAD_DETAILS.Remove(dbContext.ACC_ATR_UPLOAD_DETAILS.FirstOrDefault(x => x.ACC_ATR_UPLOAD_ID == UploadId));
                 dbContext.SaveChanges();
                 System.IO.File.Delete(System.IO.Path.Combine(BasePath, DelObj.FILE_PATH));
                 message = "File deleted successfully.";
                 return true;
             }
             catch (Exception ex)
             {
                 ErrorLog.LogError(ex, "DeleteObservationDAL()");
                 message = "Error ocured while processing your request";
                 return false;
             }
             finally
             {
                 if (dbContext != null)
                     dbContext.Dispose();
             }

         }

         #endregion
    }

    public interface IAccountsDAL
    {
        Array GetPFAuthorizationList(AccountsFilterModel objFilter, out long totalRecords);
        Array GetAssetliabilityList(AccountsFilterModel objFilter, out long totalRecords);
        Array GetAuthorizationReceivedList(AccountsFilterModel objFilter, out long totalRecords);
        List<SP_ACC_DB_ASSET_LIABILITIES_MAJOR_DETAILS_Result> GetAssetliabilityChart(AccountsFilterModel objFilter);
        List<SP_ACC_DB_ASSET_LIABILITIES_MINOR_DETAILS_Result> GetAssetliabilityDetailsChart(AccountsFilterModel objFilter);
        Array GetSummaryList(AccountsFilterModel objFilter, out long totalRecords);
        Array GetLettestTransactionsList(AccountsFilterModel objFilter, out long totalRecords);

      
        Array GetLatestDPIUTransactionsDetailsListDAL(int? page, int? rows, string sidx, string sord, out long totalRecords);

        List<string> GetNotificationsDAL();

        Array GetDPIUAutherizationIssuedDetailsListDAL(int page, int rows, string sidx, string sord, out long totalRecords);
        List<SelectListItem> PopulateSRRDA(int id);

        #region Account ATR upload(by Pradip Patil 26/05/2017 12:30 PM)
        Boolean SaveObservationDetailsDAL(ObservationModel model, int state, int agency, int year, out string message);
        Array GetObservationListDAL(int? page, int? rows, string sord, string sidx, out long totalrecords);
        Array GetDetailObservationListDAL(int? page, int? rows, string sord, string sidx, out long totalrecords, int id);
        Array GetObservationReplyListDAL(int? page, int? rows, string sord, string sidx, out long totalrecords, int id);
        Boolean UploadATRFileDAL(HttpPostedFileBase file, int masterAccId);
        Array GetATRFileListDAL(int? page, int? rows, string sord, string sidx, out long totalrecords, String id, out String filterValues);
        Boolean DeleteObservationDAL(int ObservationId, int masterObId, out String message);
        // Boolean DeleteChildObservationDAL(int ObservationId, int masterObId, out String message);
        Boolean DeleteATRFIleDAL(String FIleName, int UploadId, out String message);


        #endregion
    }
}