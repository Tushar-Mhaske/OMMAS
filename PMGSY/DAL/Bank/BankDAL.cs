using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.Models;
using PMGSY.Models.Bank;
using PMGSY.Common;
using System.Transactions;
using PMGSY.Extensions;
using System.Data.Entity;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Text;
using Mvc.Mailer;
using System.Web.Mvc;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace PMGSY.DAL.Bank
{
    public class BankDAL : IBankDAL
    {
        PMGSYEntities dbContext = null;
        CommonFunctions objCommonFunc = null;
        DateTime pfmsStartDate = new DateTime(2018, 08, 02);

        public Array BankReconciliationList(BankFilterModel objFilter, out long totalRecords)
        {
            try
            {
                dbContext = new PMGSYEntities();
                objCommonFunc = new CommonFunctions();

                //List<BankReconciliationModel> lstBankReconcile = new List<BankReconciliationModel>();

                //lstBankReconcile = (from m in dbContext.ACC_BILL_MASTER
                //                    where 
                //                    m.ADMIN_ND_CODE == objFilter.AdminNdCode 
                //                    && m.BILL_MONTH == objFilter.Month 
                //                    && m.BILL_YEAR == objFilter.Year
                //                    && m.BILL_FINALIZED == "Y" 
                //                    && m.FUND_TYPE == objFilter.FundType 
                //                    && m.BILL_TYPE == "P"
                //                    && (m.CHQ_EPAY == "Q" || m.CHQ_EPAY == "E")
                //                    //Added By Abhishek kamble to View Only Finalize Epay/Erem Vouchers By Sriniwas sir 27-June-2014
                //                    && (m.CHQ_EPAY=="E"? ( dbContext.ACC_EPAY_MAIL_MASTER.Where(s=>s.BILL_ID==m.BILL_ID).Any()? true:false ):true)

                //                    select new BankReconciliationModel
                //                    {
                //                        BILL_ID = m.BILL_ID,
                //                        CHQ_NO = m.CHQ_NO == null ? string.Empty : m.CHQ_NO, //modified by abhishek kamble 22-8-2013
                //                        CHQ_DATE = m.CHQ_DATE,
                //                        PAYEE_NAME = m.PAYEE_NAME + (m.MASTER_CONTRACTOR.MAST_CON_COMPANY_NAME == null ? "" :" ("+m.MASTER_CONTRACTOR.MAST_CON_COMPANY_NAME + ")"),
                //                        CHQ_AMOUNT = m.CHQ_AMOUNT,
                //                        IS_CHQ_RECONCILE_BANK = m.ACC_CHEQUES_ISSUED.IS_CHQ_RECONCILE_BANK == null ? false : m.ACC_CHEQUES_ISSUED.IS_CHQ_RECONCILE_BANK,
                //                        CHQ_RECONCILE_DATE = m.ACC_CHEQUES_ISSUED.CHQ_RECONCILE_DATE,
                //                        CHQ_RECONCILE_REMARKS = m.ACC_CHEQUES_ISSUED.CHQ_RECONCILE_REMARKS == null ? string.Empty : m.ACC_CHEQUES_ISSUED.CHQ_RECONCILE_REMARKS, //modified by abhishek kamble 22-8-2013
                //                        CHQ_EPAY = m.CHQ_EPAY ,
                //                        REMIT_TYPE = m.REMIT_TYPE //Added By Abhishek to disp epay voucher popup 27-June-2014
                //                    }).ToList<BankReconciliationModel>();

                DateTime? searchBillDate = (objFilter.SearchBillDate == null || objFilter.SearchBillDate == String.Empty) ? System.DateTime.Now : objCommonFunc.GetStringToDateTime(objFilter.SearchBillDate);

                //List<USP_ACC_BANK_RECONCILIATION_LIST_Result> lstBankReconcile = dbContext.USP_ACC_BANK_RECONCILIATION_LIST(objFilter.AdminNdCode, objFilter.Month, objFilter.Year, objFilter.FundType, objFilter.MonthDateWise, searchBillDate).ToList<USP_ACC_BANK_RECONCILIATION_LIST_Result>();
                List<USP_ACC_BANK_RECONCILIATION_LIST_Result> lstBankReconcile = dbContext.USP_ACC_BANK_RECONCILIATION_LIST(objFilter.AdminNdCode, objFilter.Month, objFilter.Year, objFilter.FundType, objFilter.MonthDateWise, searchBillDate, PMGSYSession.Current.AdminNdCode, objFilter.LevelID).ToList<USP_ACC_BANK_RECONCILIATION_LIST_Result>();

                //PFMS Validations
                if (PMGSYSession.Current.FundType == "P")
                {
                    List<USP_ACC_BANK_RECONCILIATION_LIST_Result> lstBankReconcile1 = lstBankReconcile.Where(x => x.CHQ_EPAY == "E" && x.CHQ_DATE >= pfmsStartDate).ToList();
                    
                    lstBankReconcile = lstBankReconcile.Except(lstBankReconcile1).ToList();
                }

                if (lstBankReconcile == null)
                {
                    totalRecords = 0;
                }
                else
                {
                    //BankReconciliationModel selAllRow = new BankReconciliationModel();
                    //selAllRow.BILL_ID = 0;
                    //selAllRow.CHQ_NO = "";
                    //selAllRow.CHQ_NO = null;
                    //selAllRow.PAYEE_NAME = "";
                    //selAllRow.CHQ_AMOUNT = 0;
                    //selAllRow.IS_CHQ_RECONCILE_BANK = false;
                    //selAllRow.CHQ_RECONCILE_DATE = null;
                    //selAllRow.CHQ_RECONCILE_REMARKS = "";
                    //selAllRow.CHQ_EPAY = "";
                    //lstBankReconcile.Add(selAllRow);
                    totalRecords = lstBankReconcile.Count();
                }


                //if (objFilter.sidx.Trim() != string.Empty)
                //{
                //    if (objFilter.sord.ToString() == "asc")
                //    {
                //        switch (objFilter.sidx)
                //        {
                //            case "CHQ_NO":
                //                lstBankReconcile = lstBankReconcile.OrderBy(x => x.CHQ_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<BankReconciliationModel>();
                //                break;
                //            case "CHQ_DATE":
                //                lstBankReconcile = lstBankReconcile.OrderBy(x => x.CHQ_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<BankReconciliationModel>();
                //                break;
                //            case "CHQ_AMOUNT":
                //                lstBankReconcile = lstBankReconcile.OrderBy(x => x.CHQ_AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<BankReconciliationModel>();
                //                break;                          
                //            default:
                //                lstBankReconcile = lstBankReconcile.OrderBy(x => x.CHQ_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<BankReconciliationModel>();
                //                break;
                //        }

                //    }
                //    else
                //    {
                //        switch (objFilter.sidx)
                //        {
                //            case "CHQ_NO":
                //                lstBankReconcile = lstBankReconcile.OrderByDescending(x => x.CHQ_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<BankReconciliationModel>();
                //                break;
                //            case "CHQ_DATE":
                //                lstBankReconcile = lstBankReconcile.OrderByDescending(x => x.CHQ_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<BankReconciliationModel>();
                //                break;
                //            case "CHQ_AMOUNT":
                //                lstBankReconcile = lstBankReconcile.OrderByDescending(x => x.CHQ_AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<BankReconciliationModel>();
                //                break;
                //            default:
                //                lstBankReconcile = lstBankReconcile.OrderByDescending(x => x.CHQ_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<BankReconciliationModel>();
                //                break;
                //        }
                //    }
                //}
                //else
                //{
                //    lstBankReconcile = lstBankReconcile.OrderBy(x => x.CHQ_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<BankReconciliationModel>();
                //}



                if (objFilter.sidx.Trim() != string.Empty)
                {
                    if (objFilter.sord.ToString() == "asc")
                    {
                        switch (objFilter.sidx)
                        {
                            case "CHQ_NO":
                                lstBankReconcile = lstBankReconcile.OrderBy(x => x.CHQ_NO).ToList<USP_ACC_BANK_RECONCILIATION_LIST_Result>();
                                break;
                            case "CHQ_DATE":
                                lstBankReconcile = lstBankReconcile.OrderBy(x => x.CHQ_DATE).ToList<USP_ACC_BANK_RECONCILIATION_LIST_Result>();
                                break;
                            case "CHQ_AMOUNT":
                                lstBankReconcile = lstBankReconcile.OrderBy(x => x.CHQ_AMOUNT).ToList<USP_ACC_BANK_RECONCILIATION_LIST_Result>();
                                break;
                            default:
                                lstBankReconcile = lstBankReconcile.OrderBy(x => x.CHQ_DATE).ThenBy(t => t.CHQ_NO).ToList<USP_ACC_BANK_RECONCILIATION_LIST_Result>();
                                break;
                        }

                    }
                    else
                    {
                        switch (objFilter.sidx)
                        {
                            case "CHQ_NO":
                                lstBankReconcile = lstBankReconcile.OrderByDescending(x => x.CHQ_NO).ToList<USP_ACC_BANK_RECONCILIATION_LIST_Result>();
                                break;
                            case "CHQ_DATE":
                                lstBankReconcile = lstBankReconcile.OrderByDescending(x => x.CHQ_DATE).ToList<USP_ACC_BANK_RECONCILIATION_LIST_Result>();
                                break;
                            case "CHQ_AMOUNT":
                                lstBankReconcile = lstBankReconcile.OrderByDescending(x => x.CHQ_AMOUNT).ToList<USP_ACC_BANK_RECONCILIATION_LIST_Result>();
                                break;
                            default:
                                lstBankReconcile = lstBankReconcile.OrderByDescending(x => x.CHQ_DATE).ThenByDescending(t => t.CHQ_NO).ToList<USP_ACC_BANK_RECONCILIATION_LIST_Result>();
                                break;
                        }
                    }
                }
                else
                {
                    lstBankReconcile = lstBankReconcile.OrderBy(x => x.CHQ_DATE).ThenBy(t => t.CHQ_NO).ToList<USP_ACC_BANK_RECONCILIATION_LIST_Result>();
                }

                return lstBankReconcile.Select(item => new
                {

                    id = item.BILL_ID.ToString(),
                    cell = new[] {                         
                                    //item.CHQ_NO,

                                    item.CHQ_EPAY=="E"?( (item.REMIT_TYPE==null ? "<center><a href='#' style='color:blue' onclick='ViewEpayOrder(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim() + "$"+ ( dbContext.ACC_EPAY_MAIL_MASTER.Where(c=>c.BILL_ID == item.BILL_ID).Any() ? "Y":"N")  })+ "\");return false;'> "+ item.CHQ_NO +" </a></center>" :  "<center><a href='#' style='color:blue' onclick='ViewEremOrder(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim() + "$"+ ( dbContext.ACC_EPAY_MAIL_MASTER.Where(c=>c.BILL_ID == item.BILL_ID).Any() ? "Y":"N")  })+ "\");return false;'> "+item.CHQ_NO+"</a></center>")):item.CHQ_NO,

                                    item.CHQ_DATE == null ? "" : Convert.ToDateTime(item.CHQ_DATE).ToString("dd/MM/yyyy"),
                                    item.PAYEE_NAME,
                                    item.CHQ_AMOUNT.ToString(),
                                    item.IS_CHQ_RECONCILE_BANK==1 ? "Yes" : "No",
                                    item.CHQ_RECONCILE_DATE == null ? "" : Convert.ToDateTime(item.CHQ_RECONCILE_DATE).ToString("dd/MM/yyyy"),
                                    item.CHQ_RECONCILE_REMARKS == null ? item.CHQ_EPAY == "Q" ? "Cheuqe No: "+item.CHQ_NO+" reconciled by Bank" : "EPay No: "+item.CHQ_NO+" reconciled by Bank" : item.CHQ_RECONCILE_REMARKS,
                                    URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim() })
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

        public Array BankReconciliationPFMSList(BankFilterModel objFilter, out long totalRecords)
        {
            try
            {
                dbContext = new PMGSYEntities();
                objCommonFunc = new CommonFunctions();

                DateTime? searchBillDate = (objFilter.SearchBillDate == null || objFilter.SearchBillDate == String.Empty) ? System.DateTime.Now : objCommonFunc.GetStringToDateTime(objFilter.SearchBillDate);

                List<USP_ACC_BANK_RECONCILIATION_LIST_Result> lstBankReconcile = dbContext.USP_ACC_BANK_RECONCILIATION_LIST(objFilter.AdminNdCode, objFilter.Month, objFilter.Year, objFilter.FundType, objFilter.MonthDateWise, searchBillDate, PMGSYSession.Current.AdminNdCode, objFilter.LevelID).ToList<USP_ACC_BANK_RECONCILIATION_LIST_Result>();

                //PFMS Validations
                if (PMGSYSession.Current.FundType == "P")
                {
                    //List<USP_ACC_BANK_RECONCILIATION_LIST_Result> lstBankReconcile1 = lstBankReconcile.Where(x => x.CHQ_EPAY == "E" && x.CHQ_DATE >= pfmsStartDate).ToList();

                    //lstBankReconcile = lstBankReconcile.Except(lstBankReconcile1).ToList();
                    lstBankReconcile = lstBankReconcile.Where(x => x.CHQ_EPAY == "E" && x.CHQ_DATE >= pfmsStartDate).ToList();
                }

                if (lstBankReconcile == null)
                {
                    totalRecords = 0;
                }
                else
                {
                    totalRecords = lstBankReconcile.Count();
                }

                if (objFilter.sidx.Trim() != string.Empty)
                {
                    if (objFilter.sord.ToString() == "asc")
                    {
                        switch (objFilter.sidx)
                        {
                            case "CHQ_NO":
                                lstBankReconcile = lstBankReconcile.OrderBy(x => x.CHQ_NO).ToList<USP_ACC_BANK_RECONCILIATION_LIST_Result>();
                                break;
                            case "CHQ_DATE":
                                lstBankReconcile = lstBankReconcile.OrderBy(x => x.CHQ_DATE).ToList<USP_ACC_BANK_RECONCILIATION_LIST_Result>();
                                break;
                            case "CHQ_AMOUNT":
                                lstBankReconcile = lstBankReconcile.OrderBy(x => x.CHQ_AMOUNT).ToList<USP_ACC_BANK_RECONCILIATION_LIST_Result>();
                                break;
                            default:
                                lstBankReconcile = lstBankReconcile.OrderBy(x => x.CHQ_DATE).ThenBy(t => t.CHQ_NO).ToList<USP_ACC_BANK_RECONCILIATION_LIST_Result>();
                                break;
                        }

                    }
                    else
                    {
                        switch (objFilter.sidx)
                        {
                            case "CHQ_NO":
                                lstBankReconcile = lstBankReconcile.OrderByDescending(x => x.CHQ_NO).ToList<USP_ACC_BANK_RECONCILIATION_LIST_Result>();
                                break;
                            case "CHQ_DATE":
                                lstBankReconcile = lstBankReconcile.OrderByDescending(x => x.CHQ_DATE).ToList<USP_ACC_BANK_RECONCILIATION_LIST_Result>();
                                break;
                            case "CHQ_AMOUNT":
                                lstBankReconcile = lstBankReconcile.OrderByDescending(x => x.CHQ_AMOUNT).ToList<USP_ACC_BANK_RECONCILIATION_LIST_Result>();
                                break;
                            default:
                                lstBankReconcile = lstBankReconcile.OrderByDescending(x => x.CHQ_DATE).ThenByDescending(t => t.CHQ_NO).ToList<USP_ACC_BANK_RECONCILIATION_LIST_Result>();
                                break;
                        }
                    }
                }
                else
                {
                    lstBankReconcile = lstBankReconcile.OrderBy(x => x.CHQ_DATE).ThenBy(t => t.CHQ_NO).ToList<USP_ACC_BANK_RECONCILIATION_LIST_Result>();
                }

                return lstBankReconcile.Select(item => new
                {
                    id = item.BILL_ID.ToString(),
                    cell = new[] {                         
                                    //item.CHQ_NO,

                                    item.CHQ_EPAY=="E"?( (item.REMIT_TYPE==null ? "<center><a href='#' style='color:blue' onclick='ViewEpayOrder(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim() + "$"+ ( dbContext.ACC_EPAY_MAIL_MASTER.Where(c=>c.BILL_ID == item.BILL_ID).Any() ? "Y":"N")  })+ "\");return false;'> "+ item.CHQ_NO +" </a></center>" :  "<center><a href='#' style='color:blue' onclick='ViewEremOrder(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim() + "$"+ ( dbContext.ACC_EPAY_MAIL_MASTER.Where(c=>c.BILL_ID == item.BILL_ID).Any() ? "Y":"N")  })+ "\");return false;'> "+item.CHQ_NO+"</a></center>")):item.CHQ_NO,

                                    item.CHQ_DATE == null ? "" : Convert.ToDateTime(item.CHQ_DATE).ToString("dd/MM/yyyy"),
                                    item.PAYEE_NAME,
                                    item.CHQ_AMOUNT.ToString(),
                                    item.IS_CHQ_RECONCILE_BANK==1 ? "Yes" : "No",
                                    item.CHQ_RECONCILE_DATE == null ? "" : Convert.ToDateTime(item.CHQ_RECONCILE_DATE).ToString("dd/MM/yyyy"),
                                    item.CHQ_RECONCILE_REMARKS == null ? item.CHQ_EPAY == "Q" ? "Cheuqe No: "+item.CHQ_NO+" reconciled by Bank" : "EPay No: "+item.CHQ_NO+" reconciled by Bank" : item.CHQ_RECONCILE_REMARKS,
                                    URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim() })
                   }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "BankDAL.BankReconciliationPFMSList()");

                totalRecords = 0;
                //throw ex;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public String SaveBankReconciliedCheques(BankFilterModel objParam, ref string message)
        {

            try
            {
                //using (var scope = new TransactionScope())
                //{
                //    dbContext = new PMGSYEntities();
                //    objCommonFunc = new CommonFunctions();
                //    objParam.BankCode = dbContext.ACC_BANK_DETAILS.Where(m => m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).Select(m => m.BANK_CODE).FirstOrDefault();

                //    if (objParam.jqGridData == null)
                //    {
                //        List<ACC_BILL_MASTER> lstBillMaster = new List<ACC_BILL_MASTER>();
                //        lstBillMaster = dbContext.ACC_BILL_MASTER.Where(m => m.ADMIN_ND_CODE == objParam.AdminNdCode && m.BILL_MONTH == objParam.Month && m.BILL_YEAR == objParam.Year && m.BILL_FINALIZED == "Y" && m.FUND_TYPE == objParam.FundType && m.BILL_TYPE == "P" && (m.CHQ_EPAY == "Q" || m.CHQ_EPAY == "E") && (objParam.HeaderReconcile.ToLower() == "yes" ? m.ACC_CHEQUES_ISSUED.IS_CHQ_RECONCILE_BANK : m.ACC_CHEQUES_ISSUED.IS_CHQ_RECONCILE_BANK) == (objParam.HeaderReconcile.ToLower() == "yes" ? false : true)).ToList<ACC_BILL_MASTER>();
                //        foreach (ACC_BILL_MASTER item in lstBillMaster)
                //        {
                //            ACC_TXN_BANK acc_txn_bank = new ACC_TXN_BANK();
                //            acc_txn_bank.BANK_TXN_ID = dbContext.ACC_TXN_BANK.Any() ? dbContext.ACC_TXN_BANK.Max(m => m.BANK_TXN_ID) + 1 : 1;
                //            acc_txn_bank.BILL_ID = item.BILL_ID;
                //            acc_txn_bank.TXN_MONTH = (byte)item.BILL_MONTH;
                //            acc_txn_bank.TXN_YEAR = item.BILL_YEAR;
                //            acc_txn_bank.TXN_TYPE_CODE = objParam.HeaderReconcile.ToLower() == "yes" ? "R" : "U";
                //            acc_txn_bank.BANK_CODE = objParam.BankCode;
                //            acc_txn_bank.FUND_TYPE = item.FUND_TYPE;
                //            acc_txn_bank.TOOL_NO = item.CHQ_NO;
                //            acc_txn_bank.TOOL_TYPE = item.CHQ_EPAY;
                //            acc_txn_bank.TXN_DATE = objCommonFunc.GetStringToDateTime(objParam.HeaderDate);
                //            acc_txn_bank.TXN_REMARKS = objParam.HeaderRemarks;
                //            acc_txn_bank.SAS_LOGIN_NAME = PMGSYSession.Current.UserName;
                //            acc_txn_bank.SAS_REQUEST_IP = objParam.ClientIP;
                //            acc_txn_bank.SAS_DATA_ENTRY_DATE = DateTime.Now;
                //            acc_txn_bank.PAYEE_NAME = item.PAYEE_NAME;
                //            acc_txn_bank.AMOUNT = item.CHQ_AMOUNT;
                //            acc_txn_bank.ADMIN_ND_CODE = item.ADMIN_ND_CODE;
                //            acc_txn_bank.LVL_ID = item.LVL_ID;

                //            dbContext.ACC_TXN_BANK.Add(acc_txn_bank);
                //            dbContext.SaveChanges();

                //            ACC_CHEQUES_ISSUED existing_acc_cheque_issued = new ACC_CHEQUES_ISSUED();
                //            ACC_CHEQUES_ISSUED acc_cheque_issued = new ACC_CHEQUES_ISSUED();
                //            existing_acc_cheque_issued = dbContext.ACC_CHEQUES_ISSUED.Find(item.BILL_ID);
                //            acc_cheque_issued.BILL_ID = existing_acc_cheque_issued.BILL_ID;
                //            acc_cheque_issued.BANK_CODE = existing_acc_cheque_issued.BANK_CODE;
                //            acc_cheque_issued.IS_CHQ_ENCASHED_NA = existing_acc_cheque_issued.IS_CHQ_ENCASHED_NA;
                //            acc_cheque_issued.NA_BILL_ID = existing_acc_cheque_issued.NA_BILL_ID;
                //            acc_cheque_issued.CHEQUE_STATUS = existing_acc_cheque_issued.CHEQUE_STATUS;
                //            acc_cheque_issued.IS_CHQ_RECONCILE_BANK = objParam.HeaderReconcile.ToLower() == "yes" ? true : false; 
                //            acc_cheque_issued.CHQ_RECONCILE_DATE = objCommonFunc.GetStringToDateTime(objParam.HeaderDate);
                //            acc_cheque_issued.CHQ_RECONCILE_REMARKS = objParam.HeaderRemarks;

                //            dbContext.Entry(existing_acc_cheque_issued).CurrentValues.SetValues(acc_cheque_issued);
                //            dbContext.SaveChanges();
                //        }            
                //    }
                //    else
                //    {
                //        for (int i = 0; i < objParam.jqGridData.Length; i++)
                //        {
                //            String BillId = objParam.jqGridData[i].ENC_BILL_ID;
                //            string[] strParameters = URLEncrypt.DecryptParameters(new string[] { objParam.jqGridData[i].ENC_BILL_ID.Split('/')[0], objParam.jqGridData[i].ENC_BILL_ID.Split('/')[1], objParam.jqGridData[i].ENC_BILL_ID.Split('/')[2] });
                //            objParam.jqGridData[i].BILL_ID = Convert.ToInt64(strParameters[0]);
                //            Int64 billID = objParam.jqGridData[i].BILL_ID;
                //            ACC_BILL_MASTER acc_bill_master = new ACC_BILL_MASTER();
                //            acc_bill_master = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == billID).FirstOrDefault();
                //            objParam.jqGridData[i].CHQ_EPAY = acc_bill_master.CHQ_EPAY;
                //            objParam.jqGridData[i].BILL_DATE = acc_bill_master.BILL_DATE;
                //            objParam.jqGridData[i].SAS_REQUEST_IP = objParam.ClientIP;
                //            objParam.jqGridData[i].SAS_LOGIN_NAME = PMGSYSession.Current.UserName;
                //            objParam.jqGridData[i].BILL_MONTH = acc_bill_master.BILL_MONTH;
                //            objParam.jqGridData[i].BILL_YEAR = acc_bill_master.BILL_YEAR;
                //            objParam.jqGridData[i].ADMIN_ND_CODE = acc_bill_master.ADMIN_ND_CODE;
                //            objParam.jqGridData[i].LVL_ID = acc_bill_master.LVL_ID;
                //            objParam.jqGridData[i].BANK_CODE = objParam.BankCode;
                //            objParam.jqGridData[i].FUND_TYPE = acc_bill_master.FUND_TYPE;

                //            ACC_CHEQUES_ISSUED existing_acc_cheque_issued = new ACC_CHEQUES_ISSUED();
                //            ACC_CHEQUES_ISSUED acc_cheque_issued = new ACC_CHEQUES_ISSUED();
                //            existing_acc_cheque_issued = dbContext.ACC_CHEQUES_ISSUED.Find(objParam.jqGridData[i].BILL_ID);
                //            acc_cheque_issued.BILL_ID = existing_acc_cheque_issued.BILL_ID;
                //            acc_cheque_issued.BANK_CODE = existing_acc_cheque_issued.BANK_CODE;
                //            acc_cheque_issued.IS_CHQ_ENCASHED_NA = existing_acc_cheque_issued.IS_CHQ_ENCASHED_NA;
                //            acc_cheque_issued.NA_BILL_ID = existing_acc_cheque_issued.NA_BILL_ID;
                //            acc_cheque_issued.CHEQUE_STATUS = existing_acc_cheque_issued.CHEQUE_STATUS;
                //            if(objParam.jqGridData[i].IS_CHQ_RECONCILE.ToLower() == "yes")
                //                objParam.jqGridData[i].IS_CHQ_RECONCILE_BANK = true;
                //            else
                //                objParam.jqGridData[i].IS_CHQ_RECONCILE_BANK = false;

                //            acc_cheque_issued.IS_CHQ_RECONCILE_BANK = objParam.jqGridData[i].IS_CHQ_RECONCILE_BANK;
                //            acc_cheque_issued.CHQ_RECONCILE_DATE = objCommonFunc.GetStringToDateTime(objParam.jqGridData[i].CHQEPAY_RECONCILE_DATE);
                //            acc_cheque_issued.CHQ_RECONCILE_REMARKS = objParam.jqGridData[i].CHQ_RECONCILE_REMARKS;

                //            if (objParam.jqGridData[i].IS_CHQ_RECONCILE_BANK)
                //            {
                //                ACC_TXN_BANK acc_txn_bank = new ACC_TXN_BANK();
                //                acc_txn_bank.BANK_TXN_ID = dbContext.ACC_TXN_BANK.Any() ? dbContext.ACC_TXN_BANK.Max(m => m.BANK_TXN_ID) + 1 : 1;
                //                acc_txn_bank.BILL_ID = billID;
                //                acc_txn_bank.TXN_MONTH = (byte)objParam.jqGridData[i].BILL_MONTH;
                //                acc_txn_bank.TXN_YEAR = objParam.jqGridData[i].BILL_YEAR;
                //                acc_txn_bank.TXN_TYPE_CODE = "R";
                //                acc_txn_bank.BANK_CODE = objParam.jqGridData[i].BANK_CODE;
                //                acc_txn_bank.FUND_TYPE = objParam.jqGridData[i].FUND_TYPE;
                //                acc_txn_bank.TOOL_NO = objParam.jqGridData[i].CHQ_NO;
                //                acc_txn_bank.TOOL_TYPE = objParam.jqGridData[i].CHQ_EPAY;
                //                acc_txn_bank.TXN_DATE = objCommonFunc.GetStringToDateTime(objParam.jqGridData[i].CHQEPAY_RECONCILE_DATE);
                //                acc_txn_bank.TXN_REMARKS = objParam.jqGridData[i].CHQ_RECONCILE_REMARKS;
                //                acc_txn_bank.SAS_LOGIN_NAME = objParam.jqGridData[i].SAS_LOGIN_NAME;
                //                acc_txn_bank.SAS_REQUEST_IP = objParam.jqGridData[i].SAS_REQUEST_IP;
                //                acc_txn_bank.SAS_DATA_ENTRY_DATE = DateTime.Now;
                //                acc_txn_bank.PAYEE_NAME = objParam.jqGridData[i].PAYEE_NAME;
                //                acc_txn_bank.AMOUNT = objParam.jqGridData[i].CHQ_AMOUNT;
                //                acc_txn_bank.ADMIN_ND_CODE = objParam.jqGridData[i].ADMIN_ND_CODE;
                //                acc_txn_bank.LVL_ID = objParam.jqGridData[i].LVL_ID;

                //                dbContext.ACC_TXN_BANK.Add(acc_txn_bank);
                //                dbContext.SaveChanges();                              
                //            }
                //            else
                //            {
                //                if (existing_acc_cheque_issued.IS_CHQ_RECONCILE_BANK)
                //                {
                //                    ACC_TXN_BANK acc_txn_bank = new ACC_TXN_BANK();
                //                    acc_txn_bank.BANK_TXN_ID = dbContext.ACC_TXN_BANK.Any() ? dbContext.ACC_TXN_BANK.Max(m => m.BANK_TXN_ID) + 1 : 1;
                //                    acc_txn_bank.BILL_ID = billID;
                //                    acc_txn_bank.TXN_MONTH = (byte)objParam.jqGridData[i].BILL_MONTH;
                //                    acc_txn_bank.TXN_YEAR = objParam.jqGridData[i].BILL_YEAR;
                //                    acc_txn_bank.TXN_TYPE_CODE = "U";
                //                    acc_txn_bank.BANK_CODE = objParam.jqGridData[i].BANK_CODE;
                //                    acc_txn_bank.FUND_TYPE = objParam.jqGridData[i].FUND_TYPE;
                //                    acc_txn_bank.TOOL_NO = objParam.jqGridData[i].CHQ_NO;
                //                    acc_txn_bank.TOOL_TYPE = objParam.jqGridData[i].CHQ_EPAY;
                //                    acc_txn_bank.TXN_DATE = objCommonFunc.GetStringToDateTime(objParam.jqGridData[i].CHQEPAY_RECONCILE_DATE);
                //                    acc_txn_bank.TXN_REMARKS = existing_acc_cheque_issued.CHQ_RECONCILE_REMARKS;
                //                    acc_txn_bank.SAS_LOGIN_NAME = objParam.jqGridData[i].SAS_LOGIN_NAME;
                //                    acc_txn_bank.SAS_REQUEST_IP = objParam.jqGridData[i].SAS_REQUEST_IP;
                //                    acc_txn_bank.SAS_DATA_ENTRY_DATE = DateTime.Now;
                //                    acc_txn_bank.PAYEE_NAME = objParam.jqGridData[i].PAYEE_NAME;
                //                    acc_txn_bank.AMOUNT = objParam.jqGridData[i].CHQ_AMOUNT;
                //                    acc_txn_bank.ADMIN_ND_CODE = objParam.jqGridData[i].ADMIN_ND_CODE;
                //                    acc_txn_bank.LVL_ID = objParam.jqGridData[i].LVL_ID;

                //                    dbContext.ACC_TXN_BANK.Add(acc_txn_bank);
                //                    dbContext.SaveChanges();
                //                }                               
                //            }

                //            dbContext.Entry(existing_acc_cheque_issued).CurrentValues.SetValues(acc_cheque_issued);
                //            dbContext.SaveChanges();
                //        }
                //    }
                //    scope.Complete();

                //    return String.Empty;
                //}


                //change by koustubh nakate on 24/07/2013 to only update existing reconcile entries
                using (var scope = new TransactionScope())
                {
                    dbContext = new PMGSYEntities();
                    objCommonFunc = new CommonFunctions();
                    objParam.BankCode = dbContext.ACC_BANK_DETAILS.Where(m => m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && m.FUND_TYPE==PMGSYSession.Current.FundType && m.BANK_ACC_STATUS==true && m.ACCOUNT_TYPE == "S").Select(m => m.BANK_CODE).FirstOrDefault();
                    DateTime? checkDate = null;

                    if (objParam.jqGridData == null)
                    {
                        List<ACC_BILL_MASTER> lstBillMaster = new List<ACC_BILL_MASTER>();

                        //modified by abhishek 21-8-2013 start 
                        //old query
                        //lstBillMaster = dbContext.ACC_BILL_MASTER.Where(m => m.ADMIN_ND_CODE == objParam.AdminNdCode && m.BILL_MONTH == objParam.Month && m.BILL_YEAR == objParam.Year && m.BILL_FINALIZED == "Y" && m.FUND_TYPE == objParam.FundType && m.BILL_TYPE == "P" && (m.CHQ_EPAY == "Q" || m.CHQ_EPAY == "E") && (objParam.HeaderReconcile.ToLower() == "yes" ? m.ACC_CHEQUES_ISSUED.IS_CHQ_RECONCILE_BANK : m.ACC_CHEQUES_ISSUED.IS_CHQ_RECONCILE_BANK) == (objParam.HeaderReconcile.ToLower() == "yes" ? false : true)).ToList<ACC_BILL_MASTER>();


                        //Old Code on 17 sep 2014 star
                        //if (dbContext.ACC_BILL_MASTER.Where(m => m.ADMIN_ND_CODE == objParam.AdminNdCode && m.BILL_MONTH == objParam.Month && m.BILL_YEAR == objParam.Year && m.BILL_FINALIZED == "Y" && m.FUND_TYPE == objParam.FundType && m.BILL_TYPE == "P" && (m.CHQ_EPAY == "Q" || m.CHQ_EPAY == "E") && (objParam.HeaderReconcile.ToLower() == "yes" ? m.ACC_CHEQUES_ISSUED.IS_CHQ_RECONCILE_BANK : m.ACC_CHEQUES_ISSUED.IS_CHQ_RECONCILE_BANK) == (objParam.HeaderReconcile.ToLower() == "yes" ? false : true)).ToList<ACC_BILL_MASTER>().Any())
                        //{
                        //    if (objParam.HeaderReconcile.ToLower() == "yes")
                        //    {
                        //        //lstBillMaster = dbContext.ACC_BILL_MASTER.Where(m => m.ADMIN_ND_CODE == objParam.AdminNdCode && m.BILL_MONTH == objParam.Month && m.BILL_YEAR == objParam.Year && m.BILL_FINALIZED == "Y" && m.FUND_TYPE == objParam.FundType && m.BILL_TYPE == "P" && (m.CHQ_EPAY == "Q" || m.CHQ_EPAY == "E")).ToList<ACC_BILL_MASTER>();
                        //        lstBillMaster = dbContext.ACC_BILL_MASTER.Where(m => m.ADMIN_ND_CODE == objParam.AdminNdCode && m.BILL_MONTH == objParam.Month && m.BILL_YEAR == objParam.Year && m.BILL_FINALIZED == "Y" && m.FUND_TYPE == objParam.FundType && m.BILL_TYPE == "P" && (m.CHQ_EPAY == "Q" || m.CHQ_EPAY == "E") && (m.ACC_CHEQUES_ISSUED.IS_CHQ_RECONCILE_BANK == false)).ToList<ACC_BILL_MASTER>();
                        //    }
                        //    else {
                        //        lstBillMaster = dbContext.ACC_BILL_MASTER.Where(m => m.ADMIN_ND_CODE == objParam.AdminNdCode && m.BILL_MONTH == objParam.Month && m.BILL_YEAR == objParam.Year && m.BILL_FINALIZED == "Y" && m.FUND_TYPE == objParam.FundType && m.BILL_TYPE == "P" && (m.CHQ_EPAY == "Q" || m.CHQ_EPAY == "E") && (m.ACC_CHEQUES_ISSUED.IS_CHQ_RECONCILE_BANK == true)).ToList<ACC_BILL_MASTER>();                            
                        //    }
                        //}
                        //else
                        //{
                        //    if (objParam.HeaderReconcile.ToLower() == "yes")
                        //    {
                        //        message = "Chqeues can not be Reconciled as no cheques to Reconcile.";
                        //    }
                        //    else
                        //    {
                        //        message = "Cheques can not be UnReconciled as no cheques to UnReconcile.";
                        //    }

                        //    return "false";                             
                        //}
                        //Old Code on 17 sep 2014 end

                        //new Code on 17 sep 2014 start

                        if (String.IsNullOrEmpty(objParam.MonthDateWise))
                        {
                            message = "Please select Month Wise / Date Wise.";
                        }
                        else
                        {
                            if (objParam.MonthDateWise == "M")//Month Wise
                            {
                                if (dbContext.ACC_BILL_MASTER.Where(m => m.ADMIN_ND_CODE == objParam.AdminNdCode && m.BILL_MONTH == objParam.Month && m.BILL_YEAR == objParam.Year && m.BILL_FINALIZED == "Y" && m.FUND_TYPE == objParam.FundType && m.BILL_TYPE == "P" && (m.CHQ_EPAY == "Q" || m.CHQ_EPAY == "E") && (objParam.HeaderReconcile.ToLower() == "yes" ? m.ACC_CHEQUES_ISSUED.IS_CHQ_RECONCILE_BANK : m.ACC_CHEQUES_ISSUED.IS_CHQ_RECONCILE_BANK) == (objParam.HeaderReconcile.ToLower() == "yes" ? false : true)).ToList<ACC_BILL_MASTER>().Any())
                                {
                                    if (objParam.HeaderReconcile.ToLower() == "yes")
                                    {
                                        //lstBillMaster = dbContext.ACC_BILL_MASTER.Where(m => m.ADMIN_ND_CODE == objParam.AdminNdCode && m.BILL_MONTH == objParam.Month && m.BILL_YEAR == objParam.Year && m.BILL_FINALIZED == "Y" && m.FUND_TYPE == objParam.FundType && m.BILL_TYPE == "P" && (m.CHQ_EPAY == "Q" || m.CHQ_EPAY == "E")).ToList<ACC_BILL_MASTER>();
                                        lstBillMaster = dbContext.ACC_BILL_MASTER.Where(m => m.ADMIN_ND_CODE == objParam.AdminNdCode && m.LVL_ID == objParam.LevelID && m.BILL_MONTH == objParam.Month && m.BILL_YEAR == objParam.Year && m.BILL_FINALIZED == "Y" && m.FUND_TYPE == objParam.FundType && m.BILL_TYPE == "P" && (m.CHQ_EPAY == "Q" || m.CHQ_EPAY == "E") && (m.ACC_CHEQUES_ISSUED.IS_CHQ_RECONCILE_BANK == false)).ToList<ACC_BILL_MASTER>();




                                    }
                                    else
                                    {
                                        lstBillMaster = dbContext.ACC_BILL_MASTER.Where(m => m.ADMIN_ND_CODE == objParam.AdminNdCode && m.LVL_ID == objParam.LevelID && m.BILL_MONTH == objParam.Month && m.BILL_YEAR == objParam.Year && m.BILL_FINALIZED == "Y" && m.FUND_TYPE == objParam.FundType && m.BILL_TYPE == "P" && (m.CHQ_EPAY == "Q" || m.CHQ_EPAY == "E") && (m.ACC_CHEQUES_ISSUED.IS_CHQ_RECONCILE_BANK == true)).ToList<ACC_BILL_MASTER>();
                                    }
                                }
                                else
                                {
                                    if (objParam.HeaderReconcile.ToLower() == "yes")
                                    {
                                        message = "Chqeues can not be Reconciled as no cheques to Reconcile.";
                                    }
                                    else
                                    {
                                        message = "Cheques can not be UnReconciled as no cheques to UnReconcile.";
                                    }

                                    return "false";
                                }
                            }
                            else if (objParam.MonthDateWise == "D")//Date Wise
                            {
                                DateTime billDate = objCommonFunc.GetStringToDateTime(objParam.SearchBillDate);

                                //if (dbContext.ACC_BILL_MASTER.Where(m => m.ADMIN_ND_CODE == objParam.AdminNdCode && m.BILL_MONTH == objParam.Month && m.BILL_YEAR == objParam.Year && m.BILL_DATE == billDate && m.BILL_FINALIZED == "Y" && m.FUND_TYPE == objParam.FundType && m.BILL_TYPE == "P" && (m.CHQ_EPAY == "Q" || m.CHQ_EPAY == "E") && (objParam.HeaderReconcile.ToLower() == "yes" ? m.ACC_CHEQUES_ISSUED.IS_CHQ_RECONCILE_BANK : m.ACC_CHEQUES_ISSUED.IS_CHQ_RECONCILE_BANK) == (objParam.HeaderReconcile.ToLower() == "yes" ? false : true)).ToList<ACC_BILL_MASTER>().Any())
                                //validate 
                                if (ValidateIsBankReconciliationDetailsExist(billDate, objParam))
                                {
                                    if (objParam.HeaderReconcile.ToLower() == "yes")
                                    {
                                        //lstBillMaster = dbContext.ACC_BILL_MASTER.Where(m => m.ADMIN_ND_CODE == objParam.AdminNdCode && m.BILL_MONTH == objParam.Month && m.BILL_YEAR == objParam.Year && m.BILL_FINALIZED == "Y" && m.FUND_TYPE == objParam.FundType && m.BILL_TYPE == "P" && (m.CHQ_EPAY == "Q" || m.CHQ_EPAY == "E")).ToList<ACC_BILL_MASTER>();

                                        if (objParam.AdminNdCode == 0)//ALL DPIU
                                        {
                                            lstBillMaster = (from bm in dbContext.ACC_BILL_MASTER
                                                             join ad in dbContext.ADMIN_DEPARTMENT
                                                             on bm.ADMIN_ND_CODE equals ad.ADMIN_ND_CODE
                                                             where
                                                               ad.MAST_PARENT_ND_CODE == PMGSYSession.Current.AdminNdCode &&
                                                               bm.BILL_DATE == billDate &&
                                                               bm.BILL_FINALIZED == "Y" &&
                                                               bm.FUND_TYPE == objParam.FundType &&
                                                               bm.BILL_TYPE == "P" &&
                                                               (bm.CHQ_EPAY == "Q" || bm.CHQ_EPAY == "E") &&
                                                               (bm.ACC_CHEQUES_ISSUED.IS_CHQ_RECONCILE_BANK == false) &&
                                                               bm.LVL_ID == objParam.LevelID//Added By Abhishek kamble to Show All DPIU cheques                                                               
                                                             select bm
                                                              ).ToList<ACC_BILL_MASTER>();
                                            //lstBillMaster=dbContext.ACC_BILL_MASTER.Join(dbContext.ADMIN_DEPARTMENT,c=>c.ADMIN_ND_CODE,cm=>cm.ADMIN_ND_CODE,(c)=>new {c.ADMIN_ND_CODE}).Where(m=>m.)
                                        }
                                        else
                                        { //Selected DPIU    or SRRDA
                                            lstBillMaster = dbContext.ACC_BILL_MASTER.Where(m => m.ADMIN_ND_CODE == objParam.AdminNdCode && m.LVL_ID == objParam.LevelID && m.BILL_DATE == billDate && m.BILL_FINALIZED == "Y" && m.FUND_TYPE == objParam.FundType && m.BILL_TYPE == "P" && (m.CHQ_EPAY == "Q" || m.CHQ_EPAY == "E") && (m.ACC_CHEQUES_ISSUED.IS_CHQ_RECONCILE_BANK == false)).ToList<ACC_BILL_MASTER>();
                                        }

                                    }
                                    else
                                    {
                                        if (objParam.AdminNdCode == 0)//ALL DPIU
                                        {
                                            lstBillMaster = (from bm in dbContext.ACC_BILL_MASTER
                                                             join ad in dbContext.ADMIN_DEPARTMENT
                                                             on bm.ADMIN_ND_CODE equals ad.ADMIN_ND_CODE
                                                             where
                                                               ad.MAST_PARENT_ND_CODE == PMGSYSession.Current.AdminNdCode &&
                                                               bm.BILL_DATE == billDate &&
                                                               bm.BILL_FINALIZED == "Y" &&
                                                               bm.FUND_TYPE == objParam.FundType &&
                                                               bm.BILL_TYPE == "P" &&
                                                               (bm.CHQ_EPAY == "Q" || bm.CHQ_EPAY == "E") &&
                                                               (bm.ACC_CHEQUES_ISSUED.IS_CHQ_RECONCILE_BANK == true) &&
                                                               bm.LVL_ID == objParam.LevelID//Added By Abhishek kamble to Show All DPIU cheques                                                               
                                                             select bm
                                                              ).ToList<ACC_BILL_MASTER>();
                                            //lstBillMaster=dbContext.ACC_BILL_MASTER.Join(dbContext.ADMIN_DEPARTMENT,c=>c.ADMIN_ND_CODE,cm=>cm.ADMIN_ND_CODE,(c)=>new {c.ADMIN_ND_CODE}).Where(m=>m.)
                                        }
                                        else
                                        { //Selected DPIU or SRRDA
                                            lstBillMaster = dbContext.ACC_BILL_MASTER.Where(m => m.ADMIN_ND_CODE == objParam.AdminNdCode && m.LVL_ID == objParam.LevelID && m.BILL_DATE == billDate && m.BILL_FINALIZED == "Y" && m.FUND_TYPE == objParam.FundType && m.BILL_TYPE == "P" && (m.CHQ_EPAY == "Q" || m.CHQ_EPAY == "E") && (m.ACC_CHEQUES_ISSUED.IS_CHQ_RECONCILE_BANK == true)).ToList<ACC_BILL_MASTER>();
                                        }
                                    }
                                }
                                else
                                {
                                    if (objParam.HeaderReconcile.ToLower() == "yes")
                                    {
                                        message = "Chqeues can not be Reconciled as no cheques to Reconcile.";
                                    }
                                    else
                                    {
                                        message = "Cheques can not be UnReconciled as no cheques to UnReconcile.";
                                    }

                                    return "false";
                                }
                            }
                        }
                        //new Code on 17 sep 2014 end


                        //if (lstBillMaster.Count == 0)
                        //{
                        //    message = "Chqeues are not Reconciled.";
                        //    return "false";
                        //}

                        //modified by abhishek 21-8-2013 end

                        //added by koustubh nakate on 26/07/2013 for checking reconcile date should be greater than or eqaul to Bill Date
                        checkDate = lstBillMaster.Max(lbm => lbm.BILL_DATE);

                        if (checkDate > objCommonFunc.GetStringToDateTime(objParam.HeaderDate))
                        {
                            message = "Reconcile/UnReconcile date should be greater than or equal to Chqeue/EPay date.";
                            return "false";
                        }


                        foreach (ACC_BILL_MASTER item in lstBillMaster)
                        {
                            ACC_TXN_BANK acc_txn_bank = null;
                            if (!dbContext.ACC_TXN_BANK.Any(tb => tb.BILL_ID == item.BILL_ID))
                            {
                                acc_txn_bank = new ACC_TXN_BANK();
                                acc_txn_bank.BANK_TXN_ID = dbContext.ACC_TXN_BANK.Any() ? dbContext.ACC_TXN_BANK.Max(m => m.BANK_TXN_ID) + 1 : 1;
                                acc_txn_bank.BILL_ID = item.BILL_ID;
                                acc_txn_bank.TXN_MONTH = (byte)item.BILL_MONTH;
                                acc_txn_bank.TXN_YEAR = item.BILL_YEAR;
                                acc_txn_bank.TXN_TYPE_CODE = objParam.HeaderReconcile.ToLower() == "yes" ? "R" : "U";
                                acc_txn_bank.BANK_CODE = objParam.BankCode;
                                acc_txn_bank.FUND_TYPE = item.FUND_TYPE;
                                acc_txn_bank.TOOL_NO = item.CHQ_NO;
                                acc_txn_bank.TOOL_TYPE = item.CHQ_EPAY;
                                acc_txn_bank.TXN_DATE = objCommonFunc.GetStringToDateTime(objParam.HeaderDate);
                                acc_txn_bank.TXN_REMARKS = objParam.HeaderRemarks;
                                acc_txn_bank.SAS_LOGIN_NAME = PMGSYSession.Current.UserName;
                                acc_txn_bank.SAS_REQUEST_IP = objParam.ClientIP;
                                acc_txn_bank.SAS_DATA_ENTRY_DATE = DateTime.Now;
                                acc_txn_bank.PAYEE_NAME = item.PAYEE_NAME;
                                acc_txn_bank.AMOUNT = item.CHQ_AMOUNT;
                                acc_txn_bank.ADMIN_ND_CODE = item.ADMIN_ND_CODE;
                                acc_txn_bank.LVL_ID = item.LVL_ID;

                                dbContext.ACC_TXN_BANK.Add(acc_txn_bank);
                                dbContext.SaveChanges();
                            }
                            else
                            {
                                acc_txn_bank = dbContext.ACC_TXN_BANK.Where(tb => tb.BILL_ID == item.BILL_ID).FirstOrDefault();

                                if (acc_txn_bank != null)
                                {
                                    acc_txn_bank.TXN_TYPE_CODE = objParam.HeaderReconcile.ToLower() == "yes" ? "R" : "U";
                                    acc_txn_bank.TXN_DATE = objCommonFunc.GetStringToDateTime(objParam.HeaderDate);
                                    acc_txn_bank.TXN_REMARKS = objParam.HeaderRemarks;

                                    dbContext.Entry(acc_txn_bank).State = System.Data.Entity.EntityState.Modified;
                                    dbContext.SaveChanges();
                                }

                            }

                            ACC_CHEQUES_ISSUED existing_acc_cheque_issued = new ACC_CHEQUES_ISSUED();
                            ACC_CHEQUES_ISSUED acc_cheque_issued = new ACC_CHEQUES_ISSUED();
                            existing_acc_cheque_issued = dbContext.ACC_CHEQUES_ISSUED.Find(item.BILL_ID);
                            acc_cheque_issued.BILL_ID = existing_acc_cheque_issued.BILL_ID;
                            acc_cheque_issued.BANK_CODE = existing_acc_cheque_issued.BANK_CODE;
                            acc_cheque_issued.IS_CHQ_ENCASHED_NA = existing_acc_cheque_issued.IS_CHQ_ENCASHED_NA;
                            acc_cheque_issued.NA_BILL_ID = existing_acc_cheque_issued.NA_BILL_ID;
                            acc_cheque_issued.CHEQUE_STATUS = existing_acc_cheque_issued.CHEQUE_STATUS;
                            acc_cheque_issued.IS_CHQ_RECONCILE_BANK = objParam.HeaderReconcile.ToLower() == "yes" ? true : false;
                            acc_cheque_issued.CHQ_RECONCILE_DATE = objCommonFunc.GetStringToDateTime(objParam.HeaderDate);
                            acc_cheque_issued.CHQ_RECONCILE_REMARKS = objParam.HeaderRemarks;

                            dbContext.Entry(existing_acc_cheque_issued).CurrentValues.SetValues(acc_cheque_issued);
                            dbContext.SaveChanges();
                        }
                    }
                    else
                    {
                        for (int i = 0; i < objParam.jqGridData.Length; i++)
                        {
                            //commented by koustubh nakate on 25/07/2013 
                            //String BillId = objParam.jqGridData[i].ENC_BILL_ID;
                            //string[] strParameters = URLEncrypt.DecryptParameters(new string[] { objParam.jqGridData[i].ENC_BILL_ID.Split('/')[0], objParam.jqGridData[i].ENC_BILL_ID.Split('/')[1], objParam.jqGridData[i].ENC_BILL_ID.Split('/')[2] });
                            //objParam.jqGridData[i].BILL_ID = Convert.ToInt64(strParameters[0]);
                            //Int64 billID = objParam.jqGridData[i].BILL_ID;


                            string[] strParameters = URLEncrypt.DecryptParameters(new string[] { objParam.BillID.Split('/')[0], objParam.BillID.Split('/')[1], objParam.BillID.Split('/')[2] });


                            Int64 billID = Convert.ToInt64(strParameters[0]);


                            ACC_BILL_MASTER acc_bill_master = new ACC_BILL_MASTER();
                            acc_bill_master = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == billID).FirstOrDefault();


                            //added by koustubh nakate on 26/07/2013 for checking reconcile date should be greater than or eqaul to Bill Date
                            if (acc_bill_master.BILL_DATE > objCommonFunc.GetStringToDateTime(objParam.HeaderDate))
                            {
                                message = "Reconcile/UnReconcile date should be greater than or equal to Chqeue/EPay date.";
                                return "false";
                            }

                            objParam.jqGridData[i].BILL_ID = billID;
                            objParam.jqGridData[i].CHQ_EPAY = acc_bill_master.CHQ_EPAY;
                            objParam.jqGridData[i].BILL_DATE = acc_bill_master.BILL_DATE;
                            objParam.jqGridData[i].SAS_REQUEST_IP = objParam.ClientIP;
                            objParam.jqGridData[i].SAS_LOGIN_NAME = PMGSYSession.Current.UserName;
                            objParam.jqGridData[i].BILL_MONTH = acc_bill_master.BILL_MONTH;
                            objParam.jqGridData[i].BILL_YEAR = acc_bill_master.BILL_YEAR;
                            objParam.jqGridData[i].ADMIN_ND_CODE = acc_bill_master.ADMIN_ND_CODE;
                            objParam.jqGridData[i].LVL_ID = acc_bill_master.LVL_ID;
                            objParam.jqGridData[i].BANK_CODE = objParam.BankCode;
                            objParam.jqGridData[i].PAYEE_NAME = acc_bill_master.PAYEE_NAME;
                            objParam.jqGridData[i].CHQ_AMOUNT = acc_bill_master.CHQ_AMOUNT;

                            objParam.jqGridData[i].CHQ_NO = acc_bill_master.CHQ_NO;
                            objParam.jqGridData[i].CHQ_EPAY = acc_bill_master.CHQ_EPAY;

                            objParam.jqGridData[i].FUND_TYPE = acc_bill_master.FUND_TYPE;
                            objParam.jqGridData[i].CHQEPAY_RECONCILE_DATE = objParam.HeaderDate;
                            objParam.jqGridData[i].CHQ_RECONCILE_REMARKS = objParam.HeaderRemarks;

                            ACC_CHEQUES_ISSUED existing_acc_cheque_issued = new ACC_CHEQUES_ISSUED();
                            ACC_CHEQUES_ISSUED acc_cheque_issued = new ACC_CHEQUES_ISSUED();
                            existing_acc_cheque_issued = dbContext.ACC_CHEQUES_ISSUED.Find(objParam.jqGridData[i].BILL_ID);
                            acc_cheque_issued.BILL_ID = existing_acc_cheque_issued.BILL_ID;
                            acc_cheque_issued.BANK_CODE = existing_acc_cheque_issued.BANK_CODE;
                            acc_cheque_issued.IS_CHQ_ENCASHED_NA = existing_acc_cheque_issued.IS_CHQ_ENCASHED_NA;
                            acc_cheque_issued.NA_BILL_ID = existing_acc_cheque_issued.NA_BILL_ID;
                            acc_cheque_issued.CHEQUE_STATUS = existing_acc_cheque_issued.CHEQUE_STATUS;
                            if (objParam.HeaderReconcile.ToLower() == "yes")
                                objParam.jqGridData[i].IS_CHQ_RECONCILE_BANK = true;
                            else
                                objParam.jqGridData[i].IS_CHQ_RECONCILE_BANK = false;

                            acc_cheque_issued.IS_CHQ_RECONCILE_BANK = objParam.jqGridData[i].IS_CHQ_RECONCILE_BANK;
                            acc_cheque_issued.CHQ_RECONCILE_DATE = objCommonFunc.GetStringToDateTime(objParam.jqGridData[i].CHQEPAY_RECONCILE_DATE);
                            acc_cheque_issued.CHQ_RECONCILE_REMARKS = objParam.jqGridData[i].CHQ_RECONCILE_REMARKS;

                            if (objParam.jqGridData[i].IS_CHQ_RECONCILE_BANK)
                            {
                                ACC_TXN_BANK acc_txn_bank = null;
                                if (!dbContext.ACC_TXN_BANK.Any(tb => tb.BILL_ID == billID))
                                {
                                    acc_txn_bank = new ACC_TXN_BANK();
                                    acc_txn_bank.BANK_TXN_ID = dbContext.ACC_TXN_BANK.Any() ? dbContext.ACC_TXN_BANK.Max(m => m.BANK_TXN_ID) + 1 : 1;
                                    acc_txn_bank.BILL_ID = billID;
                                    acc_txn_bank.TXN_MONTH = (byte)objParam.jqGridData[i].BILL_MONTH;
                                    acc_txn_bank.TXN_YEAR = objParam.jqGridData[i].BILL_YEAR;
                                    acc_txn_bank.TXN_TYPE_CODE = "R";
                                    acc_txn_bank.BANK_CODE = objParam.jqGridData[i].BANK_CODE;
                                    acc_txn_bank.FUND_TYPE = objParam.jqGridData[i].FUND_TYPE;
                                    acc_txn_bank.TOOL_NO = objParam.jqGridData[i].CHQ_NO;
                                    acc_txn_bank.TOOL_TYPE = objParam.jqGridData[i].CHQ_EPAY;
                                    acc_txn_bank.TXN_DATE = objCommonFunc.GetStringToDateTime(objParam.jqGridData[i].CHQEPAY_RECONCILE_DATE);
                                    acc_txn_bank.TXN_REMARKS = objParam.jqGridData[i].CHQ_RECONCILE_REMARKS;
                                    acc_txn_bank.SAS_LOGIN_NAME = objParam.jqGridData[i].SAS_LOGIN_NAME;
                                    acc_txn_bank.SAS_REQUEST_IP = objParam.jqGridData[i].SAS_REQUEST_IP;
                                    acc_txn_bank.SAS_DATA_ENTRY_DATE = DateTime.Now;
                                    acc_txn_bank.PAYEE_NAME = objParam.jqGridData[i].PAYEE_NAME;
                                    acc_txn_bank.AMOUNT = objParam.jqGridData[i].CHQ_AMOUNT;
                                    acc_txn_bank.ADMIN_ND_CODE = objParam.jqGridData[i].ADMIN_ND_CODE;
                                    acc_txn_bank.LVL_ID = objParam.jqGridData[i].LVL_ID;

                                    dbContext.ACC_TXN_BANK.Add(acc_txn_bank);
                                    dbContext.SaveChanges();
                                }
                                else
                                {
                                    acc_txn_bank = dbContext.ACC_TXN_BANK.Where(tb => tb.BILL_ID == billID).FirstOrDefault();

                                    if (acc_txn_bank != null)
                                    {
                                        acc_txn_bank.TXN_TYPE_CODE = objParam.HeaderReconcile.ToLower() == "yes" ? "R" : "U";
                                        acc_txn_bank.TXN_DATE = objCommonFunc.GetStringToDateTime(objParam.HeaderDate);
                                        acc_txn_bank.TXN_REMARKS = objParam.HeaderRemarks;

                                        dbContext.Entry(acc_txn_bank).State = System.Data.Entity.EntityState.Modified;
                                        dbContext.SaveChanges();
                                    }

                                }
                            }
                            else
                            {
                                // if (existing_acc_cheque_issued.IS_CHQ_RECONCILE_BANK)
                                {
                                    ACC_TXN_BANK acc_txn_bank = null;
                                    if (!dbContext.ACC_TXN_BANK.Any(tb => tb.BILL_ID == billID))
                                    {
                                        acc_txn_bank = new ACC_TXN_BANK();
                                        acc_txn_bank.BANK_TXN_ID = dbContext.ACC_TXN_BANK.Any() ? dbContext.ACC_TXN_BANK.Max(m => m.BANK_TXN_ID) + 1 : 1;
                                        acc_txn_bank.BILL_ID = billID;
                                        acc_txn_bank.TXN_MONTH = (byte)objParam.jqGridData[i].BILL_MONTH;
                                        acc_txn_bank.TXN_YEAR = objParam.jqGridData[i].BILL_YEAR;
                                        acc_txn_bank.TXN_TYPE_CODE = "U";
                                        acc_txn_bank.BANK_CODE = objParam.jqGridData[i].BANK_CODE;
                                        acc_txn_bank.FUND_TYPE = objParam.jqGridData[i].FUND_TYPE;
                                        acc_txn_bank.TOOL_NO = objParam.jqGridData[i].CHQ_NO;
                                        acc_txn_bank.TOOL_TYPE = objParam.jqGridData[i].CHQ_EPAY;
                                        acc_txn_bank.TXN_DATE = objCommonFunc.GetStringToDateTime(objParam.jqGridData[i].CHQEPAY_RECONCILE_DATE);
                                        acc_txn_bank.TXN_REMARKS = existing_acc_cheque_issued.CHQ_RECONCILE_REMARKS;
                                        acc_txn_bank.SAS_LOGIN_NAME = objParam.jqGridData[i].SAS_LOGIN_NAME;
                                        acc_txn_bank.SAS_REQUEST_IP = objParam.jqGridData[i].SAS_REQUEST_IP;
                                        acc_txn_bank.SAS_DATA_ENTRY_DATE = DateTime.Now;
                                        acc_txn_bank.PAYEE_NAME = objParam.jqGridData[i].PAYEE_NAME;
                                        acc_txn_bank.AMOUNT = objParam.jqGridData[i].CHQ_AMOUNT;
                                        acc_txn_bank.ADMIN_ND_CODE = objParam.jqGridData[i].ADMIN_ND_CODE;
                                        acc_txn_bank.LVL_ID = objParam.jqGridData[i].LVL_ID;

                                        dbContext.ACC_TXN_BANK.Add(acc_txn_bank);
                                        dbContext.SaveChanges();
                                    }
                                    else
                                    {
                                        acc_txn_bank = dbContext.ACC_TXN_BANK.Where(tb => tb.BILL_ID == billID).FirstOrDefault();

                                        if (acc_txn_bank != null)
                                        {
                                            acc_txn_bank.TXN_TYPE_CODE = objParam.HeaderReconcile.ToLower() == "yes" ? "R" : "U";
                                            acc_txn_bank.TXN_DATE = objCommonFunc.GetStringToDateTime(objParam.HeaderDate);
                                            acc_txn_bank.TXN_REMARKS = objParam.HeaderRemarks;

                                            dbContext.Entry(acc_txn_bank).State = System.Data.Entity.EntityState.Modified;
                                            dbContext.SaveChanges();
                                        }

                                    }
                                }
                            }

                            dbContext.Entry(existing_acc_cheque_issued).CurrentValues.SetValues(acc_cheque_issued);
                            dbContext.SaveChanges();
                        }
                    }
                    scope.Complete();

                    return String.Empty;
                }
            }
            catch (Exception ex)
            {
                //throw ex;
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);

                message = "An error occured while proccessing your request.";
                return "false";
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }


        public bool ValidateIsBankReconciliationDetailsExist(System.DateTime billDate, BankFilterModel objParam)
        {
            PMGSYEntities dbContextObj = new PMGSYEntities();
            try
            {
                if (objParam.AdminNdCode == 0)//All DPIU
                {
                    return (from bm in dbContextObj.ACC_BILL_MASTER
                            join ad in dbContextObj.ADMIN_DEPARTMENT
                            on bm.ADMIN_ND_CODE equals ad.ADMIN_ND_CODE
                            where
                               ad.MAST_PARENT_ND_CODE == PMGSYSession.Current.AdminNdCode &&
                               bm.BILL_DATE == billDate &&
                               bm.BILL_FINALIZED == "Y" &&
                               bm.FUND_TYPE == objParam.FundType &&
                               bm.BILL_TYPE == "P" &&
                               (bm.CHQ_EPAY == "Q" || bm.CHQ_EPAY == "E") &&
                               (bm.ACC_CHEQUES_ISSUED.IS_CHQ_RECONCILE_BANK == (objParam.HeaderReconcile.ToLower() == "yes" ? false : true))
                            select bm
                            ).Any();
                }
                else
                {
                    return dbContextObj.ACC_BILL_MASTER.Where(m => m.ADMIN_ND_CODE == objParam.AdminNdCode && m.BILL_DATE == billDate && m.BILL_FINALIZED == "Y" && m.FUND_TYPE == objParam.FundType && m.BILL_TYPE == "P" && (m.CHQ_EPAY == "Q" || m.CHQ_EPAY == "E") && (objParam.HeaderReconcile.ToLower() == "yes" ? m.ACC_CHEQUES_ISSUED.IS_CHQ_RECONCILE_BANK : m.ACC_CHEQUES_ISSUED.IS_CHQ_RECONCILE_BANK) == (objParam.HeaderReconcile.ToLower() == "yes" ? false : true)).ToList<ACC_BILL_MASTER>().Any();
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
            finally
            {
                if (dbContextObj != null)
                    dbContextObj.Dispose();
            }
        }

        /// <summary>
        /// returns the Bank details list 
        /// </summary>
        /// <param name="page">no. of pages</param>
        /// <param name="rows">no. of rows</param>
        /// <param name="sidx">sort column name</param>
        /// <param name="sord">sort order</param>
        /// <param name="totalRecords">total no. of records</param>
        /// <param name="stateCode">indicates the state id</param>
        /// <returns></returns>
        public Array GetBankDetailsList(int? page, int? rows, string sidx, string sord, out long totalRecords, int stateCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                int? parentCode = 0;
                if (PMGSYSession.Current.LevelId == 5)
                {
                    parentCode = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).Select(m => m.MAST_PARENT_ND_CODE).FirstOrDefault();
                }
                else
                {
                    parentCode = PMGSYSession.Current.AdminNdCode;
                }
                var bankDetailsList = (from item in dbContext.ACC_BANK_DETAILS
                                       where
                                       item.MAST_STATE_CODE == PMGSYSession.Current.StateCode &&
                                       item.FUND_TYPE == PMGSYSession.Current.FundType &&
                                       item.ADMIN_ND_CODE == parentCode
                                       select new
                                       {
                                           item.ADMIN_ND_CODE,
                                           item.BANK_ACC_CLOSE_DATE,
                                           item.BANK_ACC_NO,
                                           item.BANK_ACC_OPEN_DATE,
                                           item.BANK_ACC_STATUS,
                                           item.BANK_ADDRESS1,
                                           item.BANK_ADDRESS2,
                                           item.BANK_AGREEMENT_DATE,
                                           item.BANK_BRANCH,
                                           item.BANK_CODE,
                                           item.BANK_EMAIL,
                                           item.BANK_FAX,
                                           item.BANK_NAME,
                                           item.BANK_PHONE1,
                                           item.BANK_PHONE2,
                                           item.BANK_PIN,
                                           item.BANK_REMARKS,
                                           item.Bank_SEC_CODE,
                                           item.BANK_STD_FAX,
                                           item.BANK_STD1,
                                           item.BANK_STD2,
                                           item.FUND_TYPE,
                                           item.MAST_DISTRICT_CODE,
                                           item.MAST_STATE_CODE,
                                           item.MAST_IFSC_CODE,
                                           item.ACCOUNT_TYPE,
                                           item.ACCOUNT_HOLDER_NAME
                                       });

                totalRecords = bankDetailsList.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "BANK_NAME":
                                bankDetailsList = bankDetailsList.OrderBy(m => m.BANK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "BANK_BRANCH":
                                bankDetailsList = bankDetailsList.OrderBy(m => m.BANK_BRANCH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "BANK_ACC_NO":
                                bankDetailsList = bankDetailsList.OrderBy(m => m.BANK_ACC_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "BANK_AGREEMENT_DATE":
                                bankDetailsList = bankDetailsList.OrderBy(m => m.BANK_AGREEMENT_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "ADDRESS":
                                bankDetailsList = bankDetailsList.OrderBy(m => m.BANK_ADDRESS1).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "PHONE":
                                bankDetailsList = bankDetailsList.OrderBy(m => m.BANK_STD1).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "BANK_ACC_OPEN_DATE":
                                bankDetailsList = bankDetailsList.OrderBy(m => m.BANK_ACC_OPEN_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "BANK_ACC_CLOSE_DATE":
                                bankDetailsList = bankDetailsList.OrderBy(m => m.BANK_ACC_CLOSE_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                bankDetailsList = bankDetailsList.OrderBy(m => m.BANK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "BANK_NAME":
                                bankDetailsList = bankDetailsList.OrderByDescending(m => m.BANK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "BANK_BRANCH":
                                bankDetailsList = bankDetailsList.OrderByDescending(m => m.BANK_BRANCH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "BANK_ACC_NO":
                                bankDetailsList = bankDetailsList.OrderByDescending(m => m.BANK_ACC_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "BANK_AGREEMENT_DATE":
                                bankDetailsList = bankDetailsList.OrderByDescending(m => m.BANK_AGREEMENT_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "ADDRESS":
                                bankDetailsList = bankDetailsList.OrderByDescending(m => m.BANK_ADDRESS1).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "PHONE":
                                bankDetailsList = bankDetailsList.OrderByDescending(m => m.BANK_STD1).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "BANK_ACC_OPEN_DATE":
                                bankDetailsList = bankDetailsList.OrderByDescending(m => m.BANK_ACC_OPEN_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "BANK_ACC_CLOSE_DATE":
                                bankDetailsList = bankDetailsList.OrderByDescending(m => m.BANK_ACC_CLOSE_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                bankDetailsList = bankDetailsList.OrderByDescending(m => m.BANK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                }
                else
                {
                    bankDetailsList = bankDetailsList.OrderByDescending(m => m.BANK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                var gridData = bankDetailsList.Select(m => new
                {
                    m.ADMIN_ND_CODE,
                    m.BANK_ACC_CLOSE_DATE,
                    m.BANK_ACC_NO,
                    m.BANK_ACC_OPEN_DATE,
                    m.BANK_ACC_STATUS,
                    m.BANK_ADDRESS1,
                    m.BANK_ADDRESS2,
                    m.BANK_AGREEMENT_DATE,
                    m.BANK_BRANCH,
                    m.BANK_CODE,
                    m.BANK_EMAIL,
                    m.BANK_FAX,
                    m.BANK_NAME,
                    m.BANK_PHONE1,
                    m.BANK_PHONE2,
                    m.BANK_PIN,
                    m.BANK_REMARKS,
                    m.Bank_SEC_CODE,
                    m.BANK_STD_FAX,
                    m.BANK_STD1,
                    m.BANK_STD2,
                    m.FUND_TYPE,
                    m.MAST_DISTRICT_CODE,
                    m.MAST_STATE_CODE,
                    m.MAST_IFSC_CODE,
                    m.ACCOUNT_TYPE,
                    m.ACCOUNT_HOLDER_NAME
                }).ToArray();

                return gridData.Select(bank => new
                {
                    id = bank.BANK_CODE.ToString(),
                    cell = new[]
                    {
                        bank.BANK_NAME == null?string.Empty:bank.BANK_NAME.ToString(),
                        bank.BANK_BRANCH == null?string.Empty:bank.BANK_BRANCH.ToString(),
                        bank.ACCOUNT_TYPE == "S" ? "Saving" : bank.ACCOUNT_TYPE == "H" ? "Holding" : bank.ACCOUNT_TYPE == "D" ? "Security Deposit Account" : "--",
                        (bank.ACCOUNT_HOLDER_NAME == null || bank.ACCOUNT_HOLDER_NAME == string.Empty) ? "--" : bank.ACCOUNT_HOLDER_NAME,
                        bank.BANK_ACC_NO.ToString(),
                        bank.MAST_IFSC_CODE == null ? "-" : bank.MAST_IFSC_CODE,
                        bank.BANK_AGREEMENT_DATE == null?string.Empty:Convert.ToDateTime(bank.BANK_AGREEMENT_DATE).ToString("dd/MM/yyyy"),
                        bank.BANK_ADDRESS1==null?string.Empty:bank.BANK_ADDRESS1.ToString(),
                        (bank.BANK_STD1 == null?string.Empty:bank.BANK_STD1.ToString()) +" "+(bank.BANK_PHONE1==null?string.Empty:bank.BANK_PHONE1.ToString()),
                        bank.BANK_ACC_OPEN_DATE==null?string.Empty:Convert.ToDateTime(bank.BANK_ACC_OPEN_DATE).ToString("dd/MM/yyyy"),
                        bank.BANK_ACC_CLOSE_DATE==null?"-":Convert.ToDateTime(bank.BANK_ACC_CLOSE_DATE).ToString("dd/MM/yyyy"),
                        bank.BANK_ACC_STATUS == true?"Active":"Inactive",
                        //bank.BANK_ACC_STATUS == false? "<a href='#'  class='ui-icon ui-icon-zoomin ui-align-center' onClick='ShowDetails(\"" +URLEncrypt.EncryptParameters1(new string[]{"BankCode="+ bank.BANK_CODE.ToString().Trim()}) +"\"); return false;'>Show Details</a>": "<a href='#' class='ui-icon ui-icon-locked ui-align-center'></a>",//bank.BANK_ACC_STATUS == false?"<a href='#'  class='ui-icon ui-icon-zoomin ui-align-center' onClick='ShowDetails(\"" +URLEncrypt.EncryptParameters1(new string[]{"BankCode="+ bank.BANK_CODE.ToString().Trim()}) +"\"); return false;'>Show Details</a>":"<a href='#'  class='ui-icon ui-icon-zoomin ui-align-center' onClick='EditDetails(\"" +bank.BANK_CODE.ToString() +"\"); return false;'>Show Details</a>",
                        bank.BANK_ACC_STATUS == true
                        ? PMGSYSession.Current.LevelId == 4
                                ? "<a href='#' class='ui-icon ui-icon-pencil ui-align-center' onClick='EditDetails(\"" +URLEncrypt.EncryptParameters1(new string[]{"BankCode="+ bank.BANK_CODE.ToString().Trim()}) +"\"); return false;'>Edit Details</a>"
                                : "<a href='#'  class='ui-icon ui-icon-zoomin ui-align-center' onClick='ShowDetails(\"" +URLEncrypt.EncryptParameters1(new string[]{"BankCode="+ bank.BANK_CODE.ToString().Trim()}) +"\"); return false;'>Show Details</a>"
                        : "<a href='#'  class='ui-icon ui-icon-zoomin ui-align-center' onClick='ShowDetails(\"" +URLEncrypt.EncryptParameters1(new string[]{"BankCode="+ bank.BANK_CODE.ToString().Trim()}) +"\"); return false;'>Show Details</a>"
                        //: "<a href='#' class='ui-icon ui-icon-locked ui-align-center'></a>"

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
        /// save the bank details
        /// </summary>
        /// <param name="bankModel">model containing bank details information</param>
        /// <param name="message">response message</param>
        /// <returns></returns>
        public bool AddBankDetails(BankDetailsViewModel bankModel, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            objCommonFunc = new CommonFunctions();
            try
            {
                //Modified By Abhishek kamble 5-Mar-2014
                //&& m.BANK_ACC_STATUS == bankModel.BANK_ACC_STATUS
                if (dbContext.ACC_BANK_DETAILS.Any(m => m.ADMIN_ND_CODE == bankModel.ADMIN_ND_CODE && m.FUND_TYPE == bankModel.FUND_TYPE && m.MAST_STATE_CODE == bankModel.MAST_STATE_CODE && m.BANK_NAME == bankModel.BANK_NAME && m.BANK_ACC_NO == bankModel.BANK_ACC_NO))
                {
                    message = "Bank details information already exists.";
                    return false;
                }

                // Added by Srishti Restrict duplicate data entry for same acc. type
                if (dbContext.ACC_BANK_DETAILS.Any(m => m.ADMIN_ND_CODE == bankModel.ADMIN_ND_CODE && m.FUND_TYPE == bankModel.FUND_TYPE && m.ACCOUNT_TYPE == bankModel.BANK_ACC_TYPE && m.BANK_ACC_STATUS == true))
                {
                    message = "Bank details with same account type already exists.";
                    return false;
                }

                // Added by Srishti 
                if ((bankModel.BANK_ACC_TYPE == "H" || bankModel.BANK_ACC_TYPE == "D") && (bankModel.ACCOUNT_HOLDER_NAME == null || bankModel.ACCOUNT_HOLDER_NAME == string.Empty))
                {
                    message = "Please enter Account Hoder Name.";
                    return false;
                }

                ACC_BANK_DETAILS bankMaster = new ACC_BANK_DETAILS();
                if (dbContext.ACC_BANK_DETAILS.Any())
                {
                    bankMaster.BANK_CODE = Convert.ToInt16(dbContext.ACC_BANK_DETAILS.Select(m => m.BANK_CODE).Max() + 1);
                }
                else
                {
                    bankMaster.BANK_CODE = 1;
                }
                bankMaster.ADMIN_ND_CODE = bankModel.ADMIN_ND_CODE;
                if (bankModel.BANK_ACC_CLOSE_DATE != null)
                {
                    bankMaster.BANK_ACC_CLOSE_DATE = objCommonFunc.GetStringToDateTime(bankModel.BANK_ACC_CLOSE_DATE);
                }
                bankMaster.BANK_ACC_NO = bankModel.BANK_ACC_NO;
                if (bankModel.BANK_ACC_OPEN_DATE != null)
                {
                    bankMaster.BANK_ACC_OPEN_DATE = objCommonFunc.GetStringToDateTime(bankModel.BANK_ACC_OPEN_DATE);
                }
                bankMaster.BANK_ACC_STATUS = (bankModel.BANK_ACC_STATUS == null ? true : false);
                bankMaster.BANK_ADDRESS1 = bankModel.BANK_ADDRESS1;
                bankMaster.BANK_ADDRESS2 = bankModel.BANK_ADDRESS2;
                if (bankModel.BANK_AGREEMENT_DATE != null)
                {
                    bankMaster.BANK_AGREEMENT_DATE = objCommonFunc.GetStringToDateTime(bankModel.BANK_AGREEMENT_DATE);
                }
                else
                {
                    bankMaster.BANK_AGREEMENT_DATE = null;
                }
                bankMaster.BANK_BRANCH = bankModel.BANK_BRANCH;
                bankMaster.BANK_EMAIL = bankModel.BANK_EMAIL;
                bankMaster.BANK_FAX = bankModel.BANK_FAX;
                bankMaster.BANK_NAME = bankModel.BANK_NAME;
                bankMaster.BANK_PHONE1 = bankModel.BANK_PHONE1;
                bankMaster.BANK_PHONE2 = bankModel.BANK_PHONE2;
                bankMaster.BANK_PIN = bankModel.BANK_PIN;
                bankMaster.BANK_REMARKS = bankModel.BANK_REMARKS;
                bankMaster.Bank_SEC_CODE = bankModel.Bank_SEC_CODE;
                bankMaster.BANK_STD_FAX = bankModel.BANK_STD_FAX;
                bankMaster.BANK_STD1 = bankModel.BANK_STD1;
                bankMaster.BANK_STD2 = bankModel.BANK_STD2;
                bankMaster.FUND_TYPE = bankModel.FUND_TYPE;
                bankMaster.MAST_DISTRICT_CODE = bankModel.MAST_DISTRICT_CODE;
                bankMaster.MAST_STATE_CODE = bankModel.MAST_STATE_CODE;
                bankMaster.Bank_SEC_CODE = bankModel.Bank_SEC_CODE;
                //Added by Srishti 
                bankMaster.ACCOUNT_TYPE = bankModel.BANK_ACC_TYPE;
                bankMaster.ACCOUNT_HOLDER_NAME = bankModel.ACCOUNT_HOLDER_NAME;

                if (!(string.IsNullOrEmpty(bankModel.MAST_IFSC_CODE)))
                {
                    bankMaster.MAST_IFSC_CODE = bankModel.MAST_IFSC_CODE;
                }

                //Added by abhishek kamble
                bankMaster.USERID = PMGSYSession.Current.UserId;
                bankMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                dbContext.ACC_BANK_DETAILS.Add(bankMaster);
                dbContext.SaveChanges();
                message = "Bank Details Added Successfully.";
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Error occurred while processing the request.";
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
        /// updates the bank details information
        /// </summary>
        /// <param name="bankModel">model containing updated bank details</param>
        /// <param name="message">response message along with status</param>
        /// <returns></returns>
        public bool EditBankDetails(BankDetailsViewModel bankModel, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            Dictionary<string, string> decryptedParameters = null;
            String[] encryptedParameters = null;
            try
            {
                objCommonFunc = new CommonFunctions();
                encryptedParameters = bankModel.EncryptedBankCode.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                int bankCode = Convert.ToInt32(decryptedParameters["BankCode"]);

                //Validation Added by Abhishek kamble to check duplicate bank details isExist 5-Mar-2014

                if (dbContext.ACC_BANK_DETAILS.Where(m => m.ADMIN_ND_CODE == bankModel.ADMIN_ND_CODE && m.FUND_TYPE == PMGSYSession.Current.FundType && m.MAST_STATE_CODE == bankModel.MAST_STATE_CODE && m.BANK_NAME == bankModel.BANK_NAME && m.BANK_ACC_NO == bankModel.BANK_ACC_NO && m.BANK_CODE != bankCode).Any())
                {
                    message = "Bank details already exist.";
                    return false;
                }

                if (dbContext.ACC_BANK_DETAILS.Any(m => m.ADMIN_ND_CODE == bankModel.ADMIN_ND_CODE && m.FUND_TYPE == bankModel.FUND_TYPE && m.MAST_STATE_CODE == bankModel.MAST_STATE_CODE && m.BANK_NAME == bankModel.BANK_NAME && m.ACCOUNT_TYPE == bankModel.BANK_ACC_TYPE && m.BANK_ACC_STATUS == true && m.BANK_CODE != bankCode))
                {
                    message = "Bank details with same account type already exists.";
                    return false;
                }

                if ((bankModel.BANK_ACC_TYPE == "H" || bankModel.BANK_ACC_TYPE == "D") && (bankModel.ACCOUNT_HOLDER_NAME == null || bankModel.ACCOUNT_HOLDER_NAME == string.Empty))
                {
                    message = "Please enter Account Hoder Name.";
                    return false;
                }

                ACC_BANK_DETAILS bankMaster = dbContext.ACC_BANK_DETAILS.Find(bankCode);
                if (bankMaster != null)
                {
                    bankMaster.ADMIN_ND_CODE = bankModel.ADMIN_ND_CODE;
                    if (bankModel.BANK_ACC_CLOSE_DATE != null)
                    {
                        bankMaster.BANK_ACC_CLOSE_DATE = objCommonFunc.GetStringToDateTime(bankModel.BANK_ACC_CLOSE_DATE);
                        bankMaster.BANK_ACC_STATUS = false;
                    }
                    else
                    {
                        bankMaster.BANK_ACC_CLOSE_DATE = null;
                    }
                    bankMaster.BANK_ACC_NO = bankModel.BANK_ACC_NO;
                    if (bankModel.BANK_ACC_OPEN_DATE != null)
                    {
                        bankMaster.BANK_ACC_OPEN_DATE = objCommonFunc.GetStringToDateTime(bankModel.BANK_ACC_OPEN_DATE);
                    }
                    //bankMaster.BANK_ACC_STATUS = bankModel.BANK_ACC_STATUS;
                    bankMaster.BANK_ADDRESS1 = bankModel.BANK_ADDRESS1;
                    bankMaster.BANK_ADDRESS2 = bankModel.BANK_ADDRESS2;
                    if (bankModel.BANK_AGREEMENT_DATE != null)
                    {
                        bankMaster.BANK_AGREEMENT_DATE = objCommonFunc.GetStringToDateTime(bankModel.BANK_AGREEMENT_DATE);
                    }
                    else
                    {
                        bankMaster.BANK_AGREEMENT_DATE = null;
                    }
                    bankMaster.BANK_BRANCH = bankModel.BANK_BRANCH;
                    bankMaster.BANK_EMAIL = bankModel.BANK_EMAIL;
                    bankMaster.BANK_FAX = bankModel.BANK_FAX;
                    bankMaster.BANK_NAME = bankModel.BANK_NAME;
                    bankMaster.BANK_PHONE1 = bankModel.BANK_PHONE1;
                    bankMaster.BANK_PHONE2 = bankModel.BANK_PHONE2;
                    bankMaster.BANK_PIN = bankModel.BANK_PIN;
                    bankMaster.BANK_REMARKS = bankModel.BANK_REMARKS;
                    //  bankMaster.Bank_SEC_CODE = bankModel.Bank_SEC_CODE;
                    bankMaster.BANK_STD_FAX = bankModel.BANK_STD_FAX;
                    bankMaster.BANK_STD1 = bankModel.BANK_STD1;
                    bankMaster.BANK_STD2 = bankModel.BANK_STD2;
                    bankMaster.FUND_TYPE = bankModel.FUND_TYPE;
                    bankMaster.MAST_DISTRICT_CODE = bankModel.MAST_DISTRICT_CODE;
                    bankMaster.MAST_STATE_CODE = bankModel.MAST_STATE_CODE;
                    bankMaster.ACCOUNT_TYPE = bankModel.BANK_ACC_TYPE;
                    bankMaster.ACCOUNT_HOLDER_NAME = bankModel.ACCOUNT_HOLDER_NAME;


                    if (!(string.IsNullOrEmpty(bankModel.MAST_IFSC_CODE)))
                    {
                        bankMaster.MAST_IFSC_CODE = bankModel.MAST_IFSC_CODE;
                    }

                    //Added by abhishek kamble
                    bankMaster.USERID = PMGSYSession.Current.UserId;
                    bankMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    dbContext.Entry(bankMaster).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    message = "Bank Details Updated Successfully.";
                    return true;
                }
                message = "Error occurred while processing the request.";
                return false;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Error occurred while processing the request.";
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
        /// returns the bank details of associated state
        /// </summary>
        /// <param name="bankCode">represent the particular bank details id</param>
        /// <returns></returns>
        public BankDetailsViewModel GetBankDetails(int bankCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                ACC_BANK_DETAILS bankMaster = dbContext.ACC_BANK_DETAILS.Find(bankCode);
                CommonFunctions objCommon = new CommonFunctions();
                if (bankMaster != null)
                {
                    BankDetailsViewModel bankModel = new BankDetailsViewModel();
                    bankModel.EncryptedBankCode = URLEncrypt.EncryptParameters1(new string[] { "BankCode=" + bankMaster.BANK_CODE.ToString().Trim() });
                    bankModel.ADMIN_ND_CODE = bankMaster.ADMIN_ND_CODE;
                    if (bankMaster.BANK_ACC_CLOSE_DATE != null)
                    {
                        bankModel.BANK_ACC_CLOSE_DATE = objCommon.GetDateTimeToString(bankMaster.BANK_ACC_CLOSE_DATE.Value); //Convert.ToDateTime(bankMaster.BANK_ACC_CLOSE_DATE).ToShortDateString();
                    }
                    bankModel.BANK_ACC_NO = bankMaster.BANK_ACC_NO;
                    if (bankMaster.BANK_ACC_OPEN_DATE != null)
                    {
                        bankModel.BANK_ACC_OPEN_DATE = objCommon.GetDateTimeToString(bankMaster.BANK_ACC_OPEN_DATE.Value); //Convert.ToDateTime(bankMaster.BANK_ACC_OPEN_DATE).ToShortDateString();
                    }
                    bankModel.BANK_ACC_STATUS = bankMaster.BANK_ACC_STATUS;
                    bankModel.BANK_ADDRESS1 = bankMaster.BANK_ADDRESS1;
                    bankModel.BANK_ADDRESS2 = bankMaster.BANK_ADDRESS2;
                    if (bankMaster.BANK_AGREEMENT_DATE != null)
                    {
                        bankModel.BANK_AGREEMENT_DATE = objCommon.GetDateTimeToString(bankMaster.BANK_AGREEMENT_DATE.Value); //Convert.ToDateTime(bankMaster.BANK_AGREEMENT_DATE).ToShortDateString();
                    }
                    bankModel.BANK_BRANCH = bankMaster.BANK_BRANCH;
                    bankModel.BANK_EMAIL = bankMaster.BANK_EMAIL;
                    bankModel.BANK_FAX = bankMaster.BANK_FAX;
                    bankModel.BANK_NAME = bankMaster.BANK_NAME;
                    bankModel.BANK_PHONE1 = bankMaster.BANK_PHONE1;
                    bankModel.BANK_PHONE2 = bankMaster.BANK_PHONE2;
                    bankModel.BANK_PIN = bankMaster.BANK_PIN;
                    bankModel.BANK_REMARKS = bankMaster.BANK_REMARKS;
                    bankModel.Bank_SEC_CODE = bankMaster.Bank_SEC_CODE;
                    bankModel.BANK_STD_FAX = bankMaster.BANK_STD_FAX;
                    bankModel.BANK_STD1 = bankMaster.BANK_STD1;
                    bankModel.BANK_STD2 = bankMaster.BANK_STD2;
                    bankModel.FUND_TYPE = bankMaster.FUND_TYPE;
                    bankModel.MAST_DISTRICT_CODE = bankMaster.MAST_DISTRICT_CODE;
                    bankModel.MAST_STATE_CODE = bankMaster.MAST_STATE_CODE;
                    bankModel.Operation = "E";
                    bankModel.DistrictName = dbContext.MASTER_DISTRICT.Where(m => m.MAST_DISTRICT_CODE == bankMaster.MAST_DISTRICT_CODE).Select(m => m.MAST_DISTRICT_NAME).FirstOrDefault();
                    bankModel.MAST_IFSC_CODE = bankMaster.MAST_IFSC_CODE;
                    bankModel.BANK_ACC_TYPE = bankMaster.ACCOUNT_TYPE;
                    bankModel.ACCOUNT_HOLDER_NAME = bankMaster.ACCOUNT_HOLDER_NAME;
                    return bankModel;
                }
                return null;
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
        /// returns bank code of active status of particular state
        /// </summary>
        /// <returns></returns>
        public int BankDetailsStatus()
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                if (PMGSYSession.Current.LevelId == 5)
                {
                    int adminCode = PMGSYSession.Current.AdminNdCode;
                    int? parentCode = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == adminCode).Select(m => m.MAST_PARENT_ND_CODE).FirstOrDefault();
                    ACC_BANK_DETAILS bankMaster = dbContext.ACC_BANK_DETAILS.Where(m => m.ADMIN_ND_CODE == parentCode && m.FUND_TYPE == PMGSYSession.Current.FundType && m.BANK_ACC_STATUS == true).FirstOrDefault();
                    if (bankMaster.BANK_ACC_STATUS == true)
                    {
                        return bankMaster.BANK_CODE;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    ACC_BANK_DETAILS bankMaster = dbContext.ACC_BANK_DETAILS.Where(m => m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && m.FUND_TYPE == PMGSYSession.Current.FundType && m.BANK_ACC_STATUS == true).FirstOrDefault();
                    if (bankMaster.BANK_ACC_STATUS == true)
                    {
                        return bankMaster.BANK_CODE;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return 0;
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
        /// return the parent admin code for the current logged in DPIU
        /// </summary>
        /// <returns></returns>
        public int GetParentAdminCode()
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                if (PMGSYSession.Current.LevelId == 5)
                {
                    int adminCode = PMGSYSession.Current.AdminNdCode;
                    int? parentCode = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == adminCode).Select(m => m.MAST_PARENT_ND_CODE).FirstOrDefault();
                    return parentCode.Value;
                }
                else
                {
                    return PMGSYSession.Current.AdminNdCode;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return 0;
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
        /// returns the date of latest inactive bank details
        /// </summary>
        /// <returns></returns>
        public string GetOldCloseDate()
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                //string oldDate = Convert.ToDateTime(dbContext.ACC_BANK_DETAILS.Where(m => m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && m.FUND_TYPE == PMGSYSession.Current.FundType && m.BANK_ACC_STATUS == false).OrderByDescending(m => m.BANK_CODE).OrderByDescending(m => m.BANK_ACC_CLOSE_DATE).Select(m => m.BANK_ACC_CLOSE_DATE).FirstOrDefault()).ToShortDateString();
                string oldDate = Convert.ToDateTime(dbContext.ACC_BANK_DETAILS.Where(m => m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && m.FUND_TYPE == PMGSYSession.Current.FundType && m.BANK_ACC_STATUS == false).OrderByDescending(m => m.BANK_CODE).OrderByDescending(m => m.BANK_ACC_CLOSE_DATE).Select(m => m.BANK_ACC_CLOSE_DATE).FirstOrDefault()).ToString("dd/MM/yyyy");
                return oldDate;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return string.Empty;
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
        /// returns the list of DPIUs
        /// </summary>
        /// <param name="page">no. of pages in list</param>
        /// <param name="rows">no. of rows on each page.</param>
        /// <param name="sidx">sort column name</param>
        /// <param name="sord">sort order</param>
        /// <param name="totalRecords">total no. of records.</param>
        /// <param name="adminCode">admin id from session</param>
        /// <returns>returns the list of DPIUs of the particular state from session</returns>
        public Array DisplayDPIUStatusList(int? page, int? rows, string sidx, string sord, out long totalRecords, int adminCode)
        {
            dbContext = new PMGSYEntities();
            try
            {
                var dpiuList = (from item in dbContext.ADMIN_DEPARTMENT
                                where item.MAST_ND_TYPE == "D" &&
                                item.MAST_PARENT_ND_CODE == adminCode
                                select new
                                {
                                    item.ADMIN_ND_NAME,
                                    item.ADMIN_EPAY_ENABLE_DATE,
                                    item.ADMIN_BA_ENABLE_DATE,
                                    item.ADMIN_EREMIT_ENABLED_DATE,
                                    item.ADMIN_ND_CODE,
                                    item.ADMIN_BANK_AUTH_ENABLED
                                });

                totalRecords = dpiuList.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "ADMIN_ND_NAME":
                                dpiuList = dpiuList.OrderBy(m => m.ADMIN_ND_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "ADMIN_EPAY_ENABLE_DATE":
                                dpiuList = dpiuList.OrderBy(m => m.ADMIN_EPAY_ENABLE_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "ADMIN_BA_ENABLE_DATE":
                                dpiuList = dpiuList.OrderBy(m => m.ADMIN_BA_ENABLE_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "ADMIN_EREMIT_ENABLED_DATE":
                                dpiuList = dpiuList.OrderBy(m => m.ADMIN_EREMIT_ENABLED_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                dpiuList = dpiuList.OrderBy(m => m.ADMIN_ND_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "ADMIN_ND_NAME":
                                dpiuList = dpiuList.OrderByDescending(m => m.ADMIN_ND_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "ADMIN_EPAY_ENABLE_DATE":
                                dpiuList = dpiuList.OrderByDescending(m => m.ADMIN_EPAY_ENABLE_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "ADMIN_BA_ENABLE_DATE":
                                dpiuList = dpiuList.OrderByDescending(m => m.ADMIN_BA_ENABLE_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "ADMIN_EREMIT_ENABLED_DATE":
                                dpiuList = dpiuList.OrderByDescending(m => m.ADMIN_EREMIT_ENABLED_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                dpiuList = dpiuList.OrderByDescending(m => m.ADMIN_ND_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                }
                else
                {
                    dpiuList = dpiuList.OrderBy(m => m.ADMIN_ND_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                var gridData = dpiuList.Select(list => new
                {
                    list.ADMIN_BA_ENABLE_DATE,
                    list.ADMIN_EPAY_ENABLE_DATE,
                    list.ADMIN_EREMIT_ENABLED_DATE,
                    list.ADMIN_ND_CODE,
                    list.ADMIN_ND_NAME,
                    list.ADMIN_BANK_AUTH_ENABLED
                }).ToArray();

                return gridData.Select(status => new
                {
                    id = status.ADMIN_ND_CODE.ToString(),
                    cell = new[]
                    {
                        status.ADMIN_ND_NAME.ToString(),
                        status.ADMIN_EPAY_ENABLE_DATE == null?string.Empty:(status.ADMIN_EPAY_ENABLE_DATE.Value).ToString("dd/MM/yyyy"),
                        status.ADMIN_EREMIT_ENABLED_DATE == null?string.Empty:(status.ADMIN_EREMIT_ENABLED_DATE.Value).ToString("dd/MM/yyyy"),
                        status.ADMIN_BA_ENABLE_DATE == null?string.Empty:(status.ADMIN_BA_ENABLE_DATE.Value).ToString("dd/MM/yyyy"),
                        status.ADMIN_BANK_AUTH_ENABLED == "Y" ? "<a href='#' onClick=ChangeAuthorizationStatus('"+status.ADMIN_ND_CODE.ToString().Trim()+"')>Disable</a>" : "<a href='#' onClick=ChangeAuthorizationStatus('"+status.ADMIN_ND_CODE.ToString().Trim()+"')>Enable</a>",
                        (status.ADMIN_EPAY_ENABLE_DATE !=null && status.ADMIN_EREMIT_ENABLED_DATE != null )?"<span>-</span>":"<a href='#' title='Click here to Edit the DPIU Details' class='ui-icon ui-icon-pencil ui-align-center' onClick=EditDetails('" +  status.ADMIN_ND_CODE.ToString().Trim()+"'); return false;>Edit</a>",
                        "<center><table><tr><td style='border:none'><a href='#' style='float:left' id='btnSave"+  status.ADMIN_ND_CODE.ToString().Trim()+"' title='Click here to Save the DPIU Details' class='ui-icon ui-icon-disk ui-align-center' onClick=SaveDetails('" +  status.ADMIN_ND_CODE.ToString().Trim() +"');></a><a href='#' style='float:right' id='btnCancel" +  status.ADMIN_ND_CODE.ToString().Trim()+"' title='Click here to Cancel the DPIU Editing' class='ui-icon ui-icon-closethick ui-align-center' onClick= CancelSaveDetails('" +  status.ADMIN_ND_CODE.ToString().Trim() +"');></a></td></tr></table></center>",
                        //URLEncrypt.EncryptParameters1(new string[]{"AdminCode="+status.ADMIN_ND_CODE.ToString()}),
                        //URLEncrypt.EncryptParameters1(new string[]{"AdminCode="+status.ADMIN_ND_CODE.ToString()}),
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
        /// updates the epay,eremmitance and bank authorization status with dates.
        /// </summary>
        /// <param name="adminModel">form collection containing the admin department details</param>
        /// <param name="message">response message</param>
        /// <returns></returns>
        public bool UpdateDPIUDetailsDAL(AdminDepartmentViewModel adminModel, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            CommonFunctions objCommon = new CommonFunctions();
            try
            {
                ADMIN_DEPARTMENT master = dbContext.ADMIN_DEPARTMENT.Find(adminModel.ADMIN_ND_CODE);
                if (adminModel.ADMIN_BA_ENABLE_DATE != null)
                {
                    if (objCommon.GetStringToDateTime(adminModel.ADMIN_BA_ENABLE_DATE) < master.ADMIN_BA_ENABLE_DATE)
                    {
                        message = "Disable date must be greater than previous enable date.";
                        return false;
                    }
                }

                if (master != null)
                {
                    if (adminModel.ADMIN_BA_ENABLE_DATE != null && adminModel.ADMIN_BA_ENABLE_DATE != string.Empty)
                    {
                        master.ADMIN_BA_ENABLE_DATE = objCommon.GetStringToDateTime(adminModel.ADMIN_BA_ENABLE_DATE);
                        master.ADMIN_BANK_AUTH_ENABLED = master.ADMIN_BANK_AUTH_ENABLED == "Y" ? "N" : "Y";
                    }

                    if (adminModel.ADMIN_EPAY_ENABLE_DATE != null && adminModel.ADMIN_EPAY_ENABLE_DATE != string.Empty)
                    {
                        master.ADMIN_EPAY_ENABLE_DATE = objCommon.GetStringToDateTime(adminModel.ADMIN_EPAY_ENABLE_DATE);
                    }

                    if (adminModel.ADMIN_EREMIT_ENABLED_DATE != null && adminModel.ADMIN_EREMIT_ENABLED_DATE != string.Empty)
                    {
                        master.ADMIN_EREMIT_ENABLED_DATE = objCommon.GetStringToDateTime(adminModel.ADMIN_EREMIT_ENABLED_DATE);
                        master.ADMIN_EREMITTANCE_ENABLED = "Y";
                    }
                    if (adminModel.ADMIN_BANK_AUTH_ENABLED == "No")
                    {
                        master.ADMIN_BANK_AUTH_ENABLED = "N";
                        //master.ADMIN_BA_ENABLE_DATE = null;
                    }

                    //Added by abhishek kamble
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
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        #region AUTH_SIGNATORY_AUTH_CODE

        /// <summary>
        /// returns the list of Active Authorized signatory of particular state
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>
        public Array DisplayAuthSignatoryList(int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                dbContext = new PMGSYEntities();

                var lstAuthorizedSignatory = dbContext.USP_ACC_DISPLAY_AUTHORISE_SIGNATORY_DETAILS(PMGSYSession.Current.ParentNDCode, 0, PMGSYSession.Current.LevelId, "Y").ToList();

                lstAuthorizedSignatory = lstAuthorizedSignatory.Where(m => m.ADMIN_ACTIVE_STATUS == "Currently Working").ToList();

                totalRecords = lstAuthorizedSignatory.Count();

                return lstAuthorizedSignatory.Select(m => new
                {
                    cell = new[]
                    {
                        m.ADMIN_ND_NAME == null?string.Empty:m.ADMIN_ND_NAME.ToString(),
                        m.ADMIN_NO_NAME == null?string.Empty:m.ADMIN_NO_NAME.ToString(),
                        m.ADMIN_ACTIVE_START_DATE == null?"-":m.ADMIN_ACTIVE_START_DATE.ToString(),
                        m.ADMIN_NO_MOBILE == null?"-":m.ADMIN_NO_MOBILE.ToString(),                                                                                                                                                                                                                                                                                                                                                                           // URLEncrypt.EncryptParameters1(new string[]{"ConstructionCode =" + constructionDetails.MAST_CDWORKS_CODE.ToString().Trim()})
                        m.ADMIN_NO_EMAIL == null?"-":m.ADMIN_NO_EMAIL.ToString(),
                        m.ADMIN_AUTH_CODE==null?"No":"Yes",
                        m.ADMIN_NO_EMAIL == null?"<a href=# onClick=AlertEmailEntry() class='ui-icon ui-icon-plusthick ui-align-center'></a>":(Regex.IsMatch(m.ADMIN_NO_EMAIL,@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$") == true?"<center><table><tr><td style='border:none;cursor:pointer;'><span class='ui-icon ui-icon-plusthick ui-align-center' title='Click to Generate Authentication Code' onClick ='GenerateAuthCode(\"" + URLEncrypt.EncryptParameters1(new string[]{"ADMIN_NO_OFFICER_CODE="+m.ADMIN_NO_OFFICER_CODE.ToString().Trim()}) + "\");'></span></td></tr></table></center>":"<a href='#' class='ui-icon ui-icon-plusthick ui-align-center' title='Click to Generate Authentication Code' onClick=AlertWrongEmail()/>")                        
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                totalRecords = 0;
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
        /// Generate key Function is used to Generates Random epayment password
        /// </summary>
        /// <returns></returns>
        public string GenerateKey()
        {
            try
            {
                dbContext = new PMGSYEntities();
                var data = dbContext.USP_ACC_GENERATE_EPAY_PASSWORD().FirstOrDefault();
                return data;
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
        /// Save the Authorised signatory key and send an email to authorised signatory.
        /// </summary>
        /// <param name="AdminNoOfficerCode"></param>
        /// <param name="AuthSigKey"></param>
        /// <param name="sendEmailTo"></param>
        /// <returns></returns>
        public bool SaveAuthSigKey(int AdminNoOfficerCode, string AuthSigKey, ref string sendEmailTo)
        {

            using (TransactionScope ts = new TransactionScope())
            {
                dbContext = new PMGSYEntities();
                try
                {
                    //Generate MD5 Code Start    
                    //Declarations                
                    Byte[] OriginalBytes;
                    Byte[] EncodedBytes;
                    MD5 md5;
                    md5 = new MD5CryptoServiceProvider();
                    OriginalBytes = ASCIIEncoding.Default.GetBytes(AuthSigKey);
                    EncodedBytes = md5.ComputeHash(OriginalBytes);

                    //encode bytes back to 'readable' string

                    StringBuilder sbHashCode = new StringBuilder();
                    foreach (byte b in EncodedBytes)
                        sbHashCode.Append(b.ToString("x2").ToUpper());
                    //Generate MD5 Code end

                    //Save Encoded key

                    ADMIN_NODAL_OFFICERS adminNodalOfficerModel = null;
                    adminNodalOfficerModel = dbContext.ADMIN_NODAL_OFFICERS.Where(m => m.ADMIN_NO_OFFICER_CODE == AdminNoOfficerCode).FirstOrDefault();

                    if (adminNodalOfficerModel == null)
                    {
                        return false;
                    }
                    else
                    {
                        adminNodalOfficerModel.ADMIN_AUTH_CODE = sbHashCode.ToString();
                        dbContext.Entry(adminNodalOfficerModel).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                    }

                    ADMIN_DEPARTMENT AdminDepartmentModel = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == adminNodalOfficerModel.ADMIN_ND_CODE).FirstOrDefault();

                    //Send Mail
                    sendEmailTo = adminNodalOfficerModel.ADMIN_NO_EMAIL;
                    // Changed by Srishti on 13-03-2023
                    //var mailMessage = new MvcMailMessage
                    ServicePointManager.ServerCertificateValidationCallback += (s, ce, ca, p) => true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    var mailMessage = new MailMessage
                    {
                        Subject = "Authorization key for " + AdminDepartmentModel.ADMIN_ND_NAME
                    };
                    mailMessage.To.Add(sendEmailTo);  
                       
                    String EmailCC = ConfigurationManager.AppSettings["EMAIL_CC"].ToString();
                    if (!(String.IsNullOrEmpty(EmailCC)))
                    {                    
                          mailMessage.CC.Add(EmailCC);
                    }

                    mailMessage.Body = " Dear Sir/Madam,\n\n Please find the Authorization Key for: " + AdminDepartmentModel.ADMIN_ND_NAME + "\n\n Authorized Signatory: " + adminNodalOfficerModel.ADMIN_NO_FNAME + " " + adminNodalOfficerModel.ADMIN_NO_MNAME + " " + adminNodalOfficerModel.ADMIN_NO_LNAME + "\n\n Authorization Key: " + AuthSigKey + "\n\n Please note the key is case sensitive and do not disclose the key for security reasons. \n\n\n\n With Regards,\n\n OMMAS Team.";
                    //mailMessage.Send();

                    // Added by Srishti on 13-03-2023
                    SmtpClient client = new SmtpClient();

                    string e_EuthHost = ConfigurationManager.AppSettings["e_EuthHost"];
                    string e_EuthPort = ConfigurationManager.AppSettings["e_EuthPort"];
                    string e_EuthMailUserName = ConfigurationManager.AppSettings["e_EuthMailUserName"];
                    string e_EuthMailPassword = ConfigurationManager.AppSettings["e_EuthMailPassword"];

                    client.Host = e_EuthHost;
                    client.Port = Convert.ToInt32(e_EuthPort);
                    client.UseDefaultCredentials = false;
                    client.EnableSsl = true; // Change to true
                    client.Credentials = new NetworkCredential(e_EuthMailUserName, e_EuthMailPassword);
                    client.Send(mailMessage);

                    ts.Complete();
                    return true;
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
        }

        #endregion

        #region SRRDA_PDF_OPEN_KEY

        /// <summary>
        /// Get Agency details such as email address
        /// </summary>
        /// <returns></returns>
        public ADMIN_DEPARTMENT GetDepartmentDetails()
        {
            dbContext = new PMGSYEntities();
            try
            {
                return dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && m.MAST_ND_TYPE == "S").FirstOrDefault();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return null;
            }
            finally
            {
                if(dbContext!=null)
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// Get Agency details such as email address
        /// </summary>
        /// <returns></returns>
        public ACC_EPAY_MAIL_OTHER GetEpayMailOtherDetails()
        {
            dbContext = new PMGSYEntities();
            try
            {
                return dbContext.ACC_EPAY_MAIL_OTHER.Where(m => m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return null;
            }
            finally
            {
                if (dbContext != null)
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// Save generated pdf open key and send email to SRRDA.
        /// </summary>
        /// <param name="PdfKeymodel"></param>
        /// <param name="MailID"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool AddEditSRRDAPDFKeyDetails(SRRDAPdfKeyViewModel PdfKeymodel, int MailID, ref string message)
        {
            dbContext = new PMGSYEntities();
            ACC_EPAY_MAIL_OTHER epayMailOtherModel = new ACC_EPAY_MAIL_OTHER();

            using (TransactionScope ts = new TransactionScope())
            {
                try
                {
                    if (MailID == 0)//Add details
                    {
                        //validation 
                        bool status = dbContext.ACC_EPAY_MAIL_OTHER.Where(m => m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).Any();
                        if (status)
                        {
                            message = "PDF Open key details already exist.";
                            return false;
                        }
                        epayMailOtherModel.MAIL_ID = dbContext.ACC_EPAY_MAIL_OTHER.Any() ? dbContext.ACC_EPAY_MAIL_OTHER.Max(m => m.MAIL_ID) + 1 : 1;
                        epayMailOtherModel.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                        epayMailOtherModel.EMAIL_CC = PdfKeymodel.EMAIL_CC;
                        epayMailOtherModel.PDF_OPEN_KEY = PdfKeymodel.PDF_OPEN_KEY;
                        epayMailOtherModel.GENERATED_DATE = System.DateTime.Now;

                        dbContext.ACC_EPAY_MAIL_OTHER.Add(epayMailOtherModel);
                        dbContext.SaveChanges();
                        message = "Details are added successfully and Generated Key is send at " + PdfKeymodel.EMAIL_CC;
                    }
                    else
                    { //Edit details                                          
                        epayMailOtherModel = dbContext.ACC_EPAY_MAIL_OTHER.Where(m => m.MAIL_ID == MailID).FirstOrDefault();
                        epayMailOtherModel.EMAIL_CC = PdfKeymodel.EMAIL_CC;
                        epayMailOtherModel.PDF_OPEN_KEY = PdfKeymodel.PDF_OPEN_KEY;
                        epayMailOtherModel.GENERATED_DATE = System.DateTime.Now;

                        dbContext.Entry(epayMailOtherModel).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                        message = "Details are Updated Successfully and Generated Key is send at " + PdfKeymodel.EMAIL_CC;
                    }

                    //Send Mail                    
                    // Changed by Srishti on 13-03-2023
                    //var mailMessage = new MvcMailMessage
                    ServicePointManager.ServerCertificateValidationCallback += (s, ce, ca, p) => true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    var mailMessage = new MailMessage
                    {
                        Subject = "SRRDA PDF Open key for " + PMGSYSession.Current.StateName + " - " + PMGSYSession.Current.DepartmentName
                    };
                    mailMessage.To.Add(PdfKeymodel.EMAIL_CC);
                    //mailMessage.To.Add("kabhishek@cdac.in");                    
                    //mailMessage.To.Add("jyotiz@cdac.in");                    

                    String EmailCC = ConfigurationManager.AppSettings["EMAIL_CC"].ToString();
                    if (!(String.IsNullOrEmpty(EmailCC)))
                    {
                        mailMessage.CC.Add(EmailCC);
                    }

                    mailMessage.Body = " Dear Sir/Madam,\n\n Please find the PDF Open Key for: " + PMGSYSession.Current.StateName + " - " + PMGSYSession.Current.DepartmentName + "\n\n PDF Open Key: " + PdfKeymodel.PDF_OPEN_KEY + "\n\n Please note the key is case sensitive and do not disclose the key for security reasons. \n\n\n\n With Regards,\n\n OMMAS Team.";
                    //mailMessage.Send();

                    // Added by Srishti on 13-03-2023
                    SmtpClient client = new SmtpClient();

                    string e_EuthHost = ConfigurationManager.AppSettings["e_EuthHost"];
                    string e_EuthPort = ConfigurationManager.AppSettings["e_EuthPort"];
                    string e_EuthMailUserName = ConfigurationManager.AppSettings["e_EuthMailUserName"];
                    string e_EuthMailPassword = ConfigurationManager.AppSettings["e_EuthMailPassword"];

                    client.Host = e_EuthHost;
                    client.Port = Convert.ToInt32(e_EuthPort);
                    client.UseDefaultCredentials = false;
                    client.EnableSsl = true; // Change to true
                    client.Credentials = new NetworkCredential(e_EuthMailUserName, e_EuthMailPassword);
                    client.Send(mailMessage);

                    ts.Complete();
                    return true;
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    return false;
                }
                finally
                {
                    if (dbContext != null)
                    dbContext.Dispose();
                }
            }

        }

        #endregion SRRDA_PDF_OPEN_KEY

        #region BankPDFKeyGeneration

        /// <summary>
        /// Display bank details list.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="agencyCode"></param>
        /// <param name="FundType"></param>
        /// <returns></returns>
        public Array DisplayBankDetailList(int? page, int? rows, string sidx, string sord, out long totalRecords, int agencyCode, String FundType)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                var bankDetailsList = (from item in dbContext.ACC_BANK_DETAILS
                                       where
                                       item.FUND_TYPE == FundType &&
                                       ((agencyCode == 0 ? 1 : item.ADMIN_ND_CODE) == (agencyCode == 0 ? 1 : agencyCode)) &&
                                       item.BANK_ACC_STATUS == true
                                       select item).ToList();

                totalRecords = bankDetailsList.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "STATE_NAME":
                                bankDetailsList = bankDetailsList.OrderBy(m => m.MAST_STATE_CODE).ToList();
                                break;
                            case "AGENCY_NAME":
                                bankDetailsList = bankDetailsList.OrderBy(m => m.ADMIN_DEPARTMENT.ADMIN_ND_NAME).ToList();
                                break;
                            case "BANK_NAME":
                                bankDetailsList = bankDetailsList.OrderBy(m => m.BANK_NAME).ToList();
                                break;
                            case "BANK_BRANCH":
                                bankDetailsList = bankDetailsList.OrderBy(m => m.BANK_BRANCH).ToList();
                                break;
                            case "BANK_ACC_NO":
                                bankDetailsList = bankDetailsList.OrderBy(m => m.BANK_ACC_NO).ToList();
                                break;
                            case "ADDRESS":
                                bankDetailsList = bankDetailsList.OrderBy(m => m.BANK_ADDRESS1).ToList();
                                break;
                            case "PHONE":
                                bankDetailsList = bankDetailsList.OrderBy(m => m.BANK_STD1).ToList();
                                break;
                            case "BANK_EMAIL":
                                bankDetailsList = bankDetailsList.OrderBy(m => m.BANK_EMAIL).ToList();
                                break;
                            case "BANK_ACC_OPEN_DATE":
                                bankDetailsList = bankDetailsList.OrderBy(m => m.BANK_ACC_OPEN_DATE).ToList();
                                break;
                            case "BANK_ACC_CLOSE_DATE":
                                bankDetailsList = bankDetailsList.OrderBy(m => m.BANK_ACC_CLOSE_DATE).ToList();
                                break;
                            case "Bank_SEC_CODE":
                                bankDetailsList = bankDetailsList.OrderBy(m => m.Bank_SEC_CODE).ToList();
                                break;
                            default:
                                bankDetailsList = bankDetailsList.OrderBy(m => m.BANK_NAME).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "STATE_NAME":
                                bankDetailsList = bankDetailsList.OrderByDescending(m => m.MAST_STATE_CODE).ToList();
                                break;
                            case "AGENCY_NAME":
                                bankDetailsList = bankDetailsList.OrderByDescending(m => m.ADMIN_DEPARTMENT.ADMIN_ND_NAME).ToList();
                                break;
                            case "BANK_NAME":
                                bankDetailsList = bankDetailsList.OrderByDescending(m => m.BANK_NAME).ToList();
                                break;
                            case "BANK_BRANCH":
                                bankDetailsList = bankDetailsList.OrderByDescending(m => m.BANK_BRANCH).ToList();
                                break;
                            case "BANK_ACC_NO":
                                bankDetailsList = bankDetailsList.OrderByDescending(m => m.BANK_ACC_NO).ToList();
                                break;
                            case "ADDRESS":
                                bankDetailsList = bankDetailsList.OrderByDescending(m => m.BANK_ADDRESS1).ToList();
                                break;
                            case "PHONE":
                                bankDetailsList = bankDetailsList.OrderByDescending(m => m.BANK_STD1).ToList();
                                break;
                            case "BANK_EMAIL":
                                bankDetailsList = bankDetailsList.OrderByDescending(m => m.BANK_EMAIL).ToList();
                                break;
                            case "BANK_ACC_OPEN_DATE":
                                bankDetailsList = bankDetailsList.OrderByDescending(m => m.BANK_ACC_OPEN_DATE).ToList();
                                break;
                            case "BANK_ACC_CLOSE_DATE":
                                bankDetailsList = bankDetailsList.OrderByDescending(m => m.BANK_ACC_CLOSE_DATE).ToList();
                                break;
                            case "Bank_SEC_CODE":
                                bankDetailsList = bankDetailsList.OrderByDescending(m => m.Bank_SEC_CODE).ToList();
                                break;
                            default:
                                bankDetailsList = bankDetailsList.OrderByDescending(m => m.BANK_NAME).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    bankDetailsList = bankDetailsList.OrderByDescending(m => m.BANK_NAME).ToList();
                }
                return bankDetailsList.Select(bank => new
                {
                    id = bank.BANK_CODE.ToString(),
                    cell = new[]
                    {
                        bank.MASTER_STATE.MAST_STATE_NAME,
                        bank.ADMIN_DEPARTMENT.ADMIN_ND_NAME,
                        bank.BANK_NAME == null?string.Empty:bank.BANK_NAME,
                        bank.BANK_BRANCH == null?string.Empty:bank.BANK_BRANCH,
                        bank.BANK_ACC_NO.ToString(),
                        bank.BANK_ADDRESS1==null?string.Empty:bank.BANK_ADDRESS1,                        
                        //bank.BANK_AGREEMENT_DATE == null?string.Empty:Convert.ToDateTime(bank.BANK_AGREEMENT_DATE).ToString("dd/MM/yyyy"),
                        (bank.BANK_STD1 == null?string.Empty:bank.BANK_STD1.ToString()) +" "+(bank.BANK_PHONE1==null?string.Empty:bank.BANK_PHONE1.ToString()),
                        bank.BANK_EMAIL==null?String.Empty:bank.BANK_EMAIL,
                        bank.BANK_ACC_OPEN_DATE==null?string.Empty:Convert.ToDateTime(bank.BANK_ACC_OPEN_DATE).ToString("dd/MM/yyyy"),
                        bank.BANK_ACC_CLOSE_DATE==null?"-":Convert.ToDateTime(bank.BANK_ACC_CLOSE_DATE).ToString("dd/MM/yyyy"),                        
                        bank.Bank_SEC_CODE==null?"No":"Yes",
                        //String.IsNullOrEmpty(bank.BANK_EMAIL)? ("<a href='#' class='ui-icon ui-icon-info ui-align-center' onClick='BankEmailNotPresent(); return false;'>Bank Email Address Not Present</a>"): ( (new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(bank.BANK_EMAIL))? ("<a href='#'  class='ui-icon ui-icon-plusthick ui-align-center' onClick='GeneratePDFOpenKey(\"" +URLEncrypt.EncryptParameters1(new string[]{"BankCode="+ bank.BANK_CODE.ToString().Trim()}) +"\"); return false;'>Generate PDF Open Key</a>"):  ("<a href='#' class='ui-icon ui-icon-alert ui-align-center' onClick='BankEmailNotValid(); return false;'>Bank Email Address is incorrect</a>")) ,                        
                        String.IsNullOrEmpty(bank.BANK_EMAIL)? ("<a href='#' class='ui-icon ui-icon-info ui-align-center' onClick='BankEmailNotPresent(); return false;'>Bank Email Address Not Present</a>"): ("<a href='#'  class='ui-icon ui-icon-plusthick ui-align-center' onClick='GeneratePDFOpenKey(\"" +URLEncrypt.EncryptParameters1(new string[]{"BankCode="+ bank.BANK_CODE.ToString().Trim()}) +"\"); return false;'>Generate PDF Open Key</a>"), 
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
        /// Save generated bank pdf open key details and send email to bank
        /// </summary>
        /// <param name="BankCode"></param>
        /// <param name="BankPDFKey"></param>
        /// <param name="sendEmailTo"></param>
        /// <param name="StateAgencyName"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool SaveBankPDFOpenKey(int BankCode, String BankPDFKey, ref String sendEmailTo, String StateAgencyName,ref String message)
        {
            dbContext = new PMGSYEntities();
            ACC_BANK_DETAILS bankDetailsModel = new ACC_BANK_DETAILS();

            using (TransactionScope ts = new TransactionScope())
            {
                try
                {
                    String FundType = String.Empty;   //Fund Type P/M/A
                    String SubjectDisc = String.Empty; //Email Subject 
                    String AgencyName = String.Empty; //Agency Name

                    bankDetailsModel = dbContext.ACC_BANK_DETAILS.Where(m => m.BANK_CODE == BankCode).FirstOrDefault();

                    if (bankDetailsModel == null)
                    {
                        return false;
                    }

                    bankDetailsModel.Bank_SEC_CODE = BankPDFKey;
                    sendEmailTo = bankDetailsModel.BANK_EMAIL;

                    //Email Validation  
                    //if(!(new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(bankDetailsModel.BANK_EMAIL)))
                    //{                                                                                                           
                    //    return false;
                    //}                 

                    dbContext.Entry(bankDetailsModel).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    FundType = bankDetailsModel.FUND_TYPE == "P" ? ("Program Fund") : (bankDetailsModel.FUND_TYPE == "A" ? "Administrative Expenses Fund" : "Maintenance Fund");

                    if (StateAgencyName.ToString().Trim().Equals("--All--"))
                    {
                        AgencyName = bankDetailsModel.MASTER_STATE.MAST_STATE_NAME + " ( " + bankDetailsModel.ADMIN_DEPARTMENT.ADMIN_ND_NAME + " )";
                        SubjectDisc = "Bank PDF Open key for " + FundType + " - " + AgencyName + " - " + bankDetailsModel.BANK_NAME;
                    }
                    else
                    {
                        AgencyName = StateAgencyName;
                        SubjectDisc = "Bank PDF Open key for " + FundType + " - " + AgencyName + " - " + bankDetailsModel.BANK_NAME;
                    }

                    //Send Mail                              

                    // Chagned by srishti on 13-03-2023
                    //var mailMessage = new MvcMailMessage
                    ServicePointManager.ServerCertificateValidationCallback += (s, ce, ca, p) => true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    var mailMessage = new MailMessage
                    {
                        Subject = SubjectDisc
                    };
                    mailMessage.To.Add(bankDetailsModel.BANK_EMAIL);
                    //mailMessage.To.Add("kabhishek@cdac.in");
                    //mailMessage.To.Add("jyotiz@cdac.in");                   

                    String EmailCC = ConfigurationManager.AppSettings["EMAIL_CC"].ToString();
                    if (!(String.IsNullOrEmpty(EmailCC)))
                    {
                        mailMessage.CC.Add(EmailCC);
                    }

                    mailMessage.Body = " Dear Sir/Madam,\n\n Please find the PDF Open Key for " + FundType + "\n\n State : " + AgencyName + "\n\n Bank : " + bankDetailsModel.BANK_NAME + "\n\n Branch : " + bankDetailsModel.BANK_BRANCH + "\n\n Address : " + bankDetailsModel.BANK_ADDRESS1 + "\n\n PDF Open Key: " + BankPDFKey + "\n\n Please note the key is case sensitive and do not disclose the key for security reasons. \n\n\n\n With Regards,\n\n OMMAS Team.";
                    //mailMessage.Send();

                    // Added by Srishti on 13-03-2023
                    SmtpClient client = new SmtpClient();

                    string e_EuthHost = ConfigurationManager.AppSettings["e_EuthHost"];
                    string e_EuthPort = ConfigurationManager.AppSettings["e_EuthPort"];
                    string e_EuthMailUserName = ConfigurationManager.AppSettings["e_EuthMailUserName"];
                    string e_EuthMailPassword = ConfigurationManager.AppSettings["e_EuthMailPassword"];

                    client.Host = e_EuthHost;
                    client.Port = Convert.ToInt32(e_EuthPort);
                    client.UseDefaultCredentials = false;
                    client.EnableSsl = true; // Change to true
                    client.Credentials = new NetworkCredential(e_EuthMailUserName, e_EuthMailPassword);
                    client.Send(mailMessage);

                    ts.Complete();
                    return true;
                }
                catch(FormatException ex)
                {
                    message = "Details are not saved because of bank email address is not valid";
                    return false;
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    return false;
                }
                finally
                {
                    if(dbContext!=null)
                    dbContext.Dispose();
                }
            }

        }


        #endregion BankPDFKeyGeneration
    }
}

public interface IBankDAL
{
    Array BankReconciliationList(BankFilterModel objFilter, out long totalRecords);
    String SaveBankReconciliedCheques(BankFilterModel objParam, ref string message);
    bool AddBankDetails(BankDetailsViewModel bankModel, ref string message);
    bool EditBankDetails(BankDetailsViewModel bankModel, ref string message);
    Array GetBankDetailsList(int? page, int? rows, string sidx, string sord, out long totalRecords, int stateCode);
    BankDetailsViewModel GetBankDetails(int bankCode);
    Array DisplayDPIUStatusList(int? page, int? rows, string sidx, string sord, out long totalRecords, int adminCode);
    bool UpdateDPIUDetailsDAL(AdminDepartmentViewModel adminModel, ref string message);

    #region AUTH_SIGNATORY_AUTH_CODE
    Array DisplayAuthSignatoryList(int? page, int? rows, string sidx, string sord, out long totalRecords);
    string GenerateKey();
    bool SaveAuthSigKey(int AdminNoOfficerCode, string AuthSigKey, ref string sendEmailTo);
    #endregion

    #region SRRDA_PDF_OPEN_KEY
    ADMIN_DEPARTMENT GetDepartmentDetails();
    ACC_EPAY_MAIL_OTHER GetEpayMailOtherDetails();
    bool AddEditSRRDAPDFKeyDetails(SRRDAPdfKeyViewModel PdfKeymodel, int MailID, ref string message);
    #endregion SRRDA_PDF_OPEN_KEY

    #region BankPDFKeyGeneration
    Array DisplayBankDetailList(int? page, int? rows, string sidx, string sord, out long totalRecords, int agencyCode, String FundType);
    bool SaveBankPDFOpenKey(int BankCode, String BankPDFKey, ref String sendEmailTo, String StateAgencyName,ref String message);
    #endregion BankPDFKeyGeneration
}

