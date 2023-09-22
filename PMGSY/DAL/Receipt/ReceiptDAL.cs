using PMGSY.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.Models.Receipts;
using PMGSY.Common;
using PMGSY.Models.Common;
using System.Data.Entity;
using System.Transactions;
using PMGSY.Extensions;
using System.Web.Mvc;
using PMGSY.Models.TransferEntryOrder;
using PMGSY.DAL.TransferEntryOrder;
using PMGSY.Controllers;
using System.Data.Entity.Core;

namespace PMGSY.DAL.Receipt
{
    public class ReceiptDAL : IReceiptDAL
    {
        private CommonFunctions commomFuncObj = null;
        private PMGSYEntities dbContext = null;
        public Array ReceiptList(ReceiptFilterModel objFilter, out long totalRecords)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<ACC_BILL_MASTER> lstBillMaster = null;

                if (objFilter.FilterMode.Equals("view"))
                {
                    lstBillMaster = dbContext.ACC_BILL_MASTER.Where(m => m.ADMIN_ND_CODE == objFilter.AdminNdCode && m.LVL_ID == objFilter.LevelId && m.FUND_TYPE == objFilter.FundType && m.BILL_TYPE == "R" && (objFilter.Month == 0 ? 1 : m.BILL_MONTH) == (objFilter.Month == 0 ? 1 : objFilter.Month) && (objFilter.Year == 0 ? 1 : m.BILL_YEAR) == (objFilter.Year == 0 ? 1 : objFilter.Year)).ToList<ACC_BILL_MASTER>();
                }
                else
                {
                    commomFuncObj = new CommonFunctions();
                    DateTime fromDate = DateTime.Now;
                    DateTime toDate = DateTime.Now;

                    if (objFilter.FromDate != String.Empty)
                    {
                        fromDate = commomFuncObj.GetStringToDateTime(objFilter.FromDate);
                    }
                    else
                    {
                        objFilter.FromDate = null;
                    }

                    if (objFilter.ToDate != String.Empty)
                    {
                        toDate = commomFuncObj.GetStringToDateTime(objFilter.ToDate);
                    }
                    else
                    {
                        objFilter.ToDate = null;
                    }

                    if (objFilter.FromDate != null && objFilter.ToDate == null && objFilter.TransId == 0)
                    {
                        lstBillMaster = dbContext.ACC_BILL_MASTER.Where(m => m.ADMIN_ND_CODE == objFilter.AdminNdCode).Where(m => m.LVL_ID == objFilter.LevelId).Where(m => m.FUND_TYPE == objFilter.FundType).Where(m => m.BILL_TYPE == "R").Where(m => m.BILL_DATE >= fromDate).ToList<ACC_BILL_MASTER>();
                    }
                    else if (objFilter.ToDate != null && objFilter.FromDate == null && objFilter.TransId == 0)
                    {
                        lstBillMaster = dbContext.ACC_BILL_MASTER.Where(m => m.ADMIN_ND_CODE == objFilter.AdminNdCode).Where(m => m.LVL_ID == objFilter.LevelId).Where(m => m.FUND_TYPE == objFilter.FundType).Where(m => m.BILL_TYPE == "R").Where(m => m.BILL_DATE <= toDate).ToList<ACC_BILL_MASTER>();
                    }
                    else if (objFilter.ToDate == null && objFilter.FromDate == null && objFilter.TransId != 0)
                    {
                        lstBillMaster = dbContext.ACC_BILL_MASTER.Where(m => m.ADMIN_ND_CODE == objFilter.AdminNdCode).Where(m => m.LVL_ID == objFilter.LevelId).Where(m => m.FUND_TYPE == objFilter.FundType).Where(m => m.BILL_TYPE == "R").Where(m => m.TXN_ID == objFilter.TransId).ToList<ACC_BILL_MASTER>();
                    }
                    else if (objFilter.ToDate == null && objFilter.FromDate != null && objFilter.TransId != 0)
                    {
                        lstBillMaster = dbContext.ACC_BILL_MASTER.Where(m => m.ADMIN_ND_CODE == objFilter.AdminNdCode).Where(m => m.LVL_ID == objFilter.LevelId).Where(m => m.FUND_TYPE == objFilter.FundType).Where(m => m.BILL_TYPE == "R").Where(m => m.BILL_DATE >= fromDate).Where(m => m.TXN_ID == objFilter.TransId).ToList<ACC_BILL_MASTER>();
                    }
                    else if (objFilter.ToDate != null && objFilter.FromDate == null && objFilter.TransId != 0)
                    {
                        lstBillMaster = dbContext.ACC_BILL_MASTER.Where(m => m.ADMIN_ND_CODE == objFilter.AdminNdCode).Where(m => m.LVL_ID == objFilter.LevelId).Where(m => m.FUND_TYPE == objFilter.FundType).Where(m => m.BILL_TYPE == "R").Where(m => m.BILL_DATE <= toDate).Where(m => m.TXN_ID == objFilter.TransId).ToList<ACC_BILL_MASTER>();
                    }
                    else if (objFilter.ToDate != null && objFilter.FromDate != null && objFilter.TransId == 0)
                    {
                        lstBillMaster = dbContext.ACC_BILL_MASTER.Where(m => m.ADMIN_ND_CODE == objFilter.AdminNdCode).Where(m => m.LVL_ID == objFilter.LevelId).Where(m => m.FUND_TYPE == objFilter.FundType).Where(m => m.BILL_TYPE == "R").Where(m => m.BILL_DATE >= fromDate).Where(m => m.BILL_DATE <= toDate).ToList<ACC_BILL_MASTER>();
                    }
                    else if (objFilter.ToDate != null && objFilter.FromDate != null && objFilter.TransId != 0)
                    {
                        lstBillMaster = dbContext.ACC_BILL_MASTER.Where(m => m.ADMIN_ND_CODE == objFilter.AdminNdCode).Where(m => m.LVL_ID == objFilter.LevelId).Where(m => m.FUND_TYPE == objFilter.FundType).Where(m => m.BILL_TYPE == "R").Where(m => m.BILL_DATE >= fromDate).Where(m => m.BILL_DATE <= toDate).Where(m => m.TXN_ID == objFilter.TransId).ToList<ACC_BILL_MASTER>();
                    }
                    else
                    {
                        lstBillMaster = dbContext.ACC_BILL_MASTER.Where(m => m.ADMIN_ND_CODE == objFilter.AdminNdCode).Where(m => m.LVL_ID == objFilter.LevelId).Where(m => m.FUND_TYPE == objFilter.FundType).Where(m => m.BILL_TYPE == "R").ToList<ACC_BILL_MASTER>();
                    }
                }

                totalRecords = lstBillMaster.Count();

                if (objFilter.sidx.Trim() != string.Empty)
                {
                    if (objFilter.sord.ToString() == "asc")
                    {
                        switch (objFilter.sidx)
                        {
                            case "ReceiptNumber":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.BILL_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "ReceiptDate":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.BILL_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "Cash/Cheque":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.CHQ_EPAY).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "TransactionName":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.ACC_MASTER_TXN.TXN_DESC).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "ChequeNo":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.CHQ_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "ChequeDate":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.CHQ_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "ChequeAmount":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.CHQ_AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "CashAmount":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.CASH_AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "GrossAmount":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.GROSS_AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            default:
                                lstBillMaster = lstBillMaster.OrderBy(x => x.BILL_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                        }

                    }
                    else
                    {
                        switch (objFilter.sidx)
                        {
                            case "ReceiptNumber":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.BILL_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "ReceiptDate":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.BILL_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "Cash/Cheque":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.CHQ_EPAY).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "TransactionName":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.ACC_MASTER_TXN.TXN_DESC).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "ChequeNo":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.CHQ_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "ChequeDate":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.CHQ_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "ChequeAmount":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.CHQ_AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "CashAmount":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.CASH_AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "GrossAmount":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.GROSS_AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            default:
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.BILL_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                        }
                    }
                }
                else
                {
                    lstBillMaster = lstBillMaster.OrderByDescending(x => x.BILL_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                }

                List<ACC_BILL_DETAILS> lstBillDetails = new List<ACC_BILL_DETAILS>();
                foreach (ACC_BILL_MASTER item in lstBillMaster)
                {
                    ACC_BILL_DETAILS acc_bill_details = new ACC_BILL_DETAILS();
                    if (dbContext.ACC_BILL_DETAILS.Any(m => m.BILL_ID == item.BILL_ID))
                    {
                        acc_bill_details.AMOUNT = dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == item.BILL_ID).Where(m => m.CREDIT_DEBIT == "C").Sum(m => m.AMOUNT);
                    }
                    else
                    {
                        acc_bill_details.AMOUNT = 0;
                    }

                    acc_bill_details.BILL_ID = item.BILL_ID;
                    lstBillDetails.Add(acc_bill_details);
                }
                Int16 rowNumber = 0;
                return lstBillMaster.Select(item => new
                {

                    id = URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim() }),
                    cell = new[] {                         
                                    
                                    item.BILL_NO,
                                    Convert.ToDateTime(item.BILL_DATE).ToString("dd/MM/yyyy"),
                                    item.CHQ_EPAY == null ? String.Empty : (item.CHQ_EPAY.Trim() == "C" ? "Cash" : (item.CHQ_EPAY.Trim() == "Q" ? "Cheque" : String.Empty)),
                                    item.ACC_MASTER_TXN.TXN_DESC.Trim(),
                                    item.CHQ_NO,
                                    item.CHQ_DATE == null ? String.Empty : Convert.ToDateTime(item.CHQ_DATE).ToString("dd/MM/yyyy"),
                                    item.CHQ_AMOUNT.ToString(),
                                    item.CASH_AMOUNT.ToString(),
                                    item.GROSS_AMOUNT.ToString(),
                                    item.GROSS_AMOUNT == lstBillDetails.Where(m=>m.BILL_ID == item.BILL_ID).Select(m=>m.AMOUNT).FirstOrDefault() ? (item.BILL_FINALIZED == "N" ? "<center><a href='#' class='ui-icon ui-icon-unlocked' onclick='LockReceipt(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()})+ "\");return false;'>Lock</a></center>" : "<center><a href='#' class='ui-icon ui-icon-search' onclick='ViewReceipt(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()})+ "\");return false;'>View</a></center>") : "<center><div style='width:50px;height:10px; background-color:#ffffff; border:0.1em solid gray; text-align:left' class='ui-corner-all' title='Rs "+((item.GROSS_AMOUNT) - lstBillDetails.Where(m=>m.BILL_ID == item.BILL_ID).Select(m=>m.AMOUNT).FirstOrDefault())+" remaining'><div style='width:"+(((lstBillDetails.Where(m=>m.BILL_ID == item.BILL_ID).Select(m=>m.AMOUNT).FirstOrDefault()/item.GROSS_AMOUNT)*100)/2)+"px;height:100%; background-color:#4eb305' class='ui-corner-left' title='Rs "+(lstBillDetails.Where(m=>m.BILL_ID == item.BILL_ID).Select(m=>m.AMOUNT).FirstOrDefault())+" entered'></div></div></center>",
                                    item.BILL_FINALIZED=="N"? "<center><a href='#' class='ui-icon ui-icon-pencil' onclick='EditReceipt(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()})+ "\");return false;'>Edit</a></center>" : "",
                                    item.BILL_FINALIZED=="N"? "<center><a href='#' class='ui-icon ui-icon-trash' onclick='DeleteReceipt(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()})+","+(++rowNumber)+ "\");return false;'>Delete</a></center>" : "<span class='ui-icon ui-icon-locked ui-align-center'></span>",//"<span class='"+(++rowNumber)+"'></span>",
                                    item.ACTION_REQUIRED == "C" ? "<center><span title='Transaction details Incorrect, Needs Correction' style=color:#1C94C4; font-weight:bold' class='C'>"+item.ACTION_REQUIRED+"</span></center>" : item.ACTION_REQUIRED == "O" ? "<center><span title='Wrong Head Entry, Delete records and insert correct transactions' style=color:#b83400; font-weight:bold' class='O'>"+item.ACTION_REQUIRED+"</span></center>" : item.ACTION_REQUIRED == "M" ? "<center><span  title='Details not present, Unfinalize this record and insert details transactions' style=color:##014421; font-weight:bold' class='M'>"+item.ACTION_REQUIRED+"</span></center>" : "<center><span  title='Correct Transaction Entry' class='ui-icon ui-icon-check'>"+item.ACTION_REQUIRED+"</span></center>"                            
                                    
                                    //+(Math.Floor(Convert.ToDouble((2500/item.GROSS_AMOUNT) * 100))).ToString()+
                                    
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

        public Array ReceiptMasterList(ReceiptFilterModel objFilter, out long totalRecords)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<ACC_BILL_MASTER> lstBillMaster = null;
                lstBillMaster = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == objFilter.BillId).ToList<ACC_BILL_MASTER>();


                totalRecords = lstBillMaster.Count();

                if (objFilter.sidx.Trim() != string.Empty)
                {
                    if (objFilter.sord.ToString() == "asc")
                    {
                        switch (objFilter.sidx)
                        {
                            case "ReceiptNumber":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.BILL_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "ReceiptDate":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.BILL_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "TransactionName":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.ACC_MASTER_TXN.TXN_DESC).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "ChequeNo":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.CHQ_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "ChequeDate":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.CHQ_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "GrossAmount":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.GROSS_AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            default:
                                lstBillMaster = lstBillMaster.OrderBy(x => x.BILL_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                        }

                    }
                    else
                    {
                        switch (objFilter.sidx)
                        {
                            case "ReceiptNumber":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.BILL_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "ReceiptDate":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.BILL_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "TransactionName":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.ACC_MASTER_TXN.TXN_DESC).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "ChequeNo":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.CHQ_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "ChequeDate":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.CHQ_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "GrossAmount":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.GROSS_AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            default:
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.BILL_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                        }
                    }
                }
                else
                {
                    lstBillMaster = lstBillMaster.OrderByDescending(x => x.BILL_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                }

                return lstBillMaster.Select(item => new
                {
                    id = URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim() }),
                    cell = new[] {                         
                                    
                                    item.BILL_NO,
                                    Convert.ToDateTime(item.BILL_DATE).ToString("dd/MM/yyyy"),
                                    item.ACC_MASTER_TXN.TXN_DESC.Trim(),
                                    item.CHQ_NO,
                                    item.CHQ_DATE == null ? String.Empty : Convert.ToDateTime(item.CHQ_DATE).ToString("dd/MM/yyyy"),
                                    item.GROSS_AMOUNT.ToString(),
                                    item.BILL_FINALIZED == "N" ? "<center><a href='#' class='ui-icon ui-icon-pencil' onclick='EditMasterReceipt(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()})+ "\");return false;'>Edit</a></center>" :  "<center><span class='ui-icon ui-icon-locked'></span></center>"
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
        /// DAL function to get the receeipt details list
        /// </summary>
        /// <param name="objFilter"></param>
        /// <param name="totalRecords"></param>
        /// <param name="totalAmount"></param>
        /// <returns></returns>
        public Array ReceiptDetailsList(ReceiptFilterModel objFilter, out long totalRecords, out decimal totalAmount)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<ACC_BILL_DETAILS> lstBillDetails = null;
                lstBillDetails = dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == objFilter.BillId && m.CREDIT_DEBIT == "C").ToList<ACC_BILL_DETAILS>();

                string isFinalize = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == objFilter.BillId).Select(m => m.BILL_FINALIZED).FirstOrDefault();

                totalRecords = lstBillDetails.Count();

                if (objFilter.sidx.Trim() != string.Empty)
                {
                    if (objFilter.sord.ToString() == "asc")
                    {
                        switch (objFilter.sidx)
                        {
                            case "TransactionName":
                                lstBillDetails = lstBillDetails.OrderBy(x => x.ACC_MASTER_TXN.TXN_DESC).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_DETAILS>();
                                break;
                            case "AccHeadcode":
                                lstBillDetails = lstBillDetails.OrderBy(x => x.ACC_MASTER_HEAD.HEAD_CODE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_DETAILS>();
                                break;
                            case "HeadName":
                                lstBillDetails = lstBillDetails.OrderBy(x => x.ACC_MASTER_HEAD.HEAD_NAME).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_DETAILS>();
                                break;
                            case "Amount":
                                lstBillDetails = lstBillDetails.OrderBy(x => x.AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_DETAILS>();
                                break;
                            case "Contractor":
                                lstBillDetails = lstBillDetails.OrderBy(x => x.MASTER_CONTRACTOR.MAST_CON_COMPANY_NAME).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_DETAILS>();
                                break;
                            case "Agreement":
                                lstBillDetails = lstBillDetails.OrderBy(x => x.IMS_AGREEMENT_CODE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_DETAILS>();
                                break;
                            case "DPIU":
                                lstBillDetails = lstBillDetails.OrderBy(x => x.ADMIN_DEPARTMENT.ADMIN_ND_NAME).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_DETAILS>();
                                break;
                            default:
                                lstBillDetails = lstBillDetails.OrderBy(x => x.TXN_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_DETAILS>();
                                break;
                        }

                    }
                    else
                    {
                        switch (objFilter.sidx)
                        {
                            case "TransactionNumber":
                                lstBillDetails = lstBillDetails.OrderByDescending(x => x.TXN_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_DETAILS>();
                                break;
                            case "TransactionName":
                                lstBillDetails = lstBillDetails.OrderByDescending(x => x.ACC_MASTER_TXN.TXN_DESC).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_DETAILS>();
                                break;
                            case "AccHeadcode":
                                lstBillDetails = lstBillDetails.OrderByDescending(x => x.ACC_MASTER_HEAD.HEAD_CODE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_DETAILS>();
                                break;
                            case "HeadName":
                                lstBillDetails = lstBillDetails.OrderByDescending(x => x.ACC_MASTER_HEAD.HEAD_NAME).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_DETAILS>();
                                break;
                            case "Amount":
                                lstBillDetails = lstBillDetails.OrderByDescending(x => x.AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_DETAILS>();
                                break;
                            case "Contractor":
                                lstBillDetails = lstBillDetails.OrderByDescending(x => x.MASTER_CONTRACTOR.MAST_CON_COMPANY_NAME).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_DETAILS>();
                                break;
                            case "Agreement":
                                lstBillDetails = lstBillDetails.OrderByDescending(x => x.IMS_AGREEMENT_CODE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_DETAILS>();
                                break;
                            case "DPIU":
                                lstBillDetails = lstBillDetails.OrderByDescending(x => x.ADMIN_ND_CODE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_DETAILS>();
                                break;
                            default:
                                lstBillDetails = lstBillDetails.OrderByDescending(x => x.TXN_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_DETAILS>();
                                break;
                        }
                    }
                }
                else
                {
                    lstBillDetails = lstBillDetails.OrderByDescending(x => x.TXN_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_DETAILS>();
                }

                totalAmount = lstBillDetails.Sum(m => m.AMOUNT);
                return lstBillDetails.Select(item => new
                {
                    id = URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim() + "$" + item.TXN_NO.ToString().Trim() }),
                    cell = new[] {                         
                                    
                                    item.TXN_NO.ToString(),
                                    dbContext.ACC_MASTER_TXN.Where(m=>m.TXN_ID == item.TXN_ID).Select(m=>m.TXN_DESC).FirstOrDefault().ToString(),
                                    "<span class='ui-state-default' style='border:none'>"+item.ACC_MASTER_HEAD.HEAD_CODE+"</span> "+item.ACC_MASTER_HEAD.HEAD_NAME,
                                    item.ACC_MASTER_HEAD.HEAD_NAME,
                                    item.MAST_CON_ID == null ? "-" : item.MASTER_CONTRACTOR.MAST_CON_COMPANY_NAME.ToString(),
                                    //old
                                    //item.IMS_AGREEMENT_CODE == null ? "-" : PMGSYSession.Current.FundType.ToLower() == "m" ? dbContext.MANE_IMS_CONTRACT.Where(t=>t.MANE_PR_CONTRACT_CODE == item.IMS_AGREEMENT_CODE).Select(t=>t.MANE_AGREEMENT_NUMBER).FirstOrDefault() : dbContext.TEND_AGREEMENT_MASTER.Where(t=>t.TEND_AGREEMENT_CODE == item.IMS_AGREEMENT_CODE).Select(t=>t.TEND_AGREEMENT_NUMBER).FirstOrDefault(),
                                    //Modified by Abhishek kamble to get Agr Code using MANE_CONTRACTOR_ID 17Nov2014
                                    item.IMS_AGREEMENT_CODE == null ? "-" : PMGSYSession.Current.FundType.ToLower() == "m" ? dbContext.MANE_IMS_CONTRACT.Where(t=>t.MANE_CONTRACT_ID == item.IMS_AGREEMENT_CODE).Select(t=>t.MANE_AGREEMENT_NUMBER).FirstOrDefault() : dbContext.TEND_AGREEMENT_MASTER.Where(t=>t.TEND_AGREEMENT_CODE == item.IMS_AGREEMENT_CODE).Select(t=>t.TEND_AGREEMENT_NUMBER).FirstOrDefault(),
                                    item.ADMIN_ND_CODE == null ? "-" : item.ADMIN_DEPARTMENT.ADMIN_ND_NAME, //dbContext.ADMIN_DEPARTMENT.Where(t=>t.ADMIN_ND_CODE == item.ADMIN_ND_CODE).Select(t=>t.ADMIN_ND_NAME).FirstOrDefault(),
                                    item.AMOUNT.ToString(),
                                    item.NARRATION,
                                    isFinalize == "N" ? "<center><a href='#' class='ui-icon ui-icon-pencil' onclick='EditReceiptDetails(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim() + "$" + item.TXN_NO.ToString().Trim() })+ "\");return false;'>Edit</a></center>" : "<center><span class='ui-icon ui-icon-locked'></span></center>",
                                    isFinalize == "N" ? "<center><a href='#' class='ui-icon ui-icon-trash' onclick='DeleteReceiptDetails(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim() + "$" + item.TXN_NO.ToString().Trim() })+ "\");return false;'>Delete</a></center>" : "<center><span class='ui-icon ui-icon-locked'></span></center>"   ,                                 
                                    (item.TXN_ID != 0 &&  item.TXN_ID != null) ? dbContext.ACC_MASTER_TXN.Where(x => x.TXN_ID == item.TXN_ID).Select(x => x.IS_OPERATIONAL).FirstOrDefault()==true ? "Correct Entry" :dbContext.ACC_MASTER_TXN.Where(x => x.TXN_ID == item.TXN_ID).Select(x => x.IS_REQ_AFTER_PORTING).FirstOrDefault() ==true ? "Edit And Correct the entry" :"Delete and Make new entry" : "Correct Entry"
                    }
                }).ToArray();

            }
             catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                totalAmount = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }

        }

        public String AddReceiptDetails(BillDetailsViewModel billDetailsViewModel)
        {
            try
            {

                using (var scope = new TransactionScope())
                {
                    Int16 txnId = Convert.ToInt16(billDetailsViewModel.TXN_ID.Split('$')[0]);
                    String cashCheque = billDetailsViewModel.TXN_ID.Split('$')[1].Trim();

                    ACC_BILL_DETAILS acc_bill_details = CloneModel(billDetailsViewModel);
                    dbContext = new PMGSYEntities();

                    acc_bill_details.HEAD_ID = (from item in dbContext.ACC_TXN_HEAD_MAPPING
                                                where item.TXN_ID == txnId && item.CASH_CHQ.Contains(cashCheque) && item.CREDIT_DEBIT == "C"
                                                select item.HEAD_ID).FirstOrDefault();
                    acc_bill_details.CREDIT_DEBIT = "C";
                    Int16? maxTxnId = dbContext.ACC_BILL_DETAILS.Where(item => item.BILL_ID == acc_bill_details.BILL_ID).Max(T => (Int16?)T.TXN_NO);
                    if (maxTxnId == null)
                    {
                        acc_bill_details.TXN_NO = 1;
                    }
                    else
                    {
                        acc_bill_details.TXN_NO = Convert.ToInt16(maxTxnId + 1);
                    }

                    //for imprest of staff
                    if (txnId == 179 || txnId == 1191 || txnId == 1190 || txnId == 572) //changes by Koustubh Nakate on 08/10/2013 txnId=to 572 
                    {
                        //added by Koustubh Nakate on 16/10/2013 to check remaining amount


                        ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS map_details = new ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS();
                        String[] strParams = billDetailsViewModel.IMPREST_BILL_ID.Split('/');
                        String[] strParameters = URLEncrypt.DecryptParameters(new string[] { strParams[0], strParams[1], strParams[2] });
                        map_details.P_BILL_ID = Convert.ToInt64(strParameters[0].Split('$')[0]);
                        map_details.S_BIll_ID = billDetailsViewModel.BILL_ID;
                        //map_details.P_TXN_ID = acc_bill_details.TXN_NO;
                        map_details.S_TXN_ID = acc_bill_details.TXN_NO;
                        map_details.P_TXN_ID = Convert.ToInt16(strParameters[0].Split('$')[1]);
                        if (dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Any())
                        {
                            map_details.MAP_ID = dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Max(m => m.MAP_ID) + 1;
                        }
                        else
                        {
                            map_details.MAP_ID = 1;
                        }

                        //ADDED By Abhishek kamble 28-nov-2013
                        map_details.USERID = PMGSYSession.Current.UserId;
                        map_details.IPADD=HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                        dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Add(map_details);
                        dbContext.SaveChanges();
                    }
                    commomFuncObj = new CommonFunctions();
                    TransactionParams objParam = new TransactionParams();
                    objParam.TXN_ID = txnId;
                    ACC_SCREEN_DESIGN_PARAM_DETAILS objDesignDetails = new ACC_SCREEN_DESIGN_PARAM_DETAILS();
                    objDesignDetails = commomFuncObj.getDetailsDesignParam(objParam);
                    if (objDesignDetails == null)
                    {
                        objDesignDetails = setDefaultDesignParams();
                    }
                    if (objDesignDetails.CON_REQ == "Y")
                    {
                        acc_bill_details.MAST_CON_ID = billDetailsViewModel.MAST_CON_ID;
                    }
                    else
                    {
                        acc_bill_details.MAST_CON_ID = null;
                    }

                    if (objDesignDetails.AGREEMENT_REQ == "Y")
                    {
                        acc_bill_details.IMS_AGREEMENT_CODE = billDetailsViewModel.IMS_AGREEMENT_CODE;
                    }
                    else
                    {
                        acc_bill_details.IMS_AGREEMENT_CODE = null;
                    }

                    if (objDesignDetails.PIU_REQ == "Y")
                    {
                        //Old
                        //acc_bill_details.ADMIN_ND_CODE = billDetailsViewModel.ADMIN_ND_CODE;
                        acc_bill_details.ADMIN_ND_CODE = billDetailsViewModel.ADMIN_ND_CODE == 0 ? null : billDetailsViewModel.ADMIN_ND_CODE;
                    }
                    else
                    {
                        acc_bill_details.ADMIN_ND_CODE = null;
                    }

                    //ADDED By Abhishek kamble 28-nov-2013
                    acc_bill_details.USERID = PMGSYSession.Current.UserId;
                    acc_bill_details.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    dbContext.ACC_BILL_DETAILS.Add(acc_bill_details);
                    dbContext.SaveChanges();

                    ACC_BILL_DETAILS new_acc_bill_details = new ACC_BILL_DETAILS();

                    new_acc_bill_details.BILL_ID = acc_bill_details.BILL_ID;
                    new_acc_bill_details.MAST_CON_ID = acc_bill_details.MAST_CON_ID;
                    new_acc_bill_details.IMS_AGREEMENT_CODE = acc_bill_details.IMS_AGREEMENT_CODE;
                    new_acc_bill_details.ADMIN_ND_CODE = acc_bill_details.ADMIN_ND_CODE;
                    new_acc_bill_details.AMOUNT = acc_bill_details.AMOUNT;
                    new_acc_bill_details.NARRATION = acc_bill_details.NARRATION;
                    new_acc_bill_details.CASH_CHQ = acc_bill_details.CASH_CHQ;
                    new_acc_bill_details.HEAD_ID = (from item in dbContext.ACC_TXN_HEAD_MAPPING
                                                    where item.TXN_ID == txnId && item.CASH_CHQ.Contains(cashCheque) && item.CREDIT_DEBIT == "D"
                                                    select item.HEAD_ID).FirstOrDefault();
                    new_acc_bill_details.CREDIT_DEBIT = "D";
                    new_acc_bill_details.TXN_NO = Convert.ToInt16(acc_bill_details.TXN_NO + 1);
                    new_acc_bill_details.TXN_ID = txnId;

                    //ADDED By Abhishek kamble 28-nov-2013
                    new_acc_bill_details.USERID = PMGSYSession.Current.UserId;
                    new_acc_bill_details.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    dbContext.ACC_BILL_DETAILS.Add(new_acc_bill_details);
                    dbContext.SaveChanges();
                    scope.Complete();
                    return "";
                }
            }
            catch (EntityCommandExecutionException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return ex.Message;
            }
            catch (EntityException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return ex.Message;
            }
              catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return ex.Message;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public String EditReceiptDetails(BillDetailsViewModel billDetailsViewModel)
        {
            try
            {
                String[] strParameters = null;
                Int16 txnId = Convert.ToInt16(billDetailsViewModel.TXN_ID.Split('$')[0]);
                
                ACC_BILL_DETAILS acc_bill_details = CloneModel(billDetailsViewModel);
                dbContext = new PMGSYEntities();
                acc_bill_details.HEAD_ID = (from item in dbContext.ACC_TXN_HEAD_MAPPING
                                            where item.TXN_ID == txnId && item.CASH_CHQ.Contains(acc_bill_details.CASH_CHQ) && item.CREDIT_DEBIT == "C"
                                            select item.HEAD_ID).FirstOrDefault();
                acc_bill_details.CREDIT_DEBIT = "C";

                ACC_BILL_DETAILS old_acc_bill_details = new ACC_BILL_DETAILS();
                old_acc_bill_details = dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == billDetailsViewModel.BILL_ID && m.TXN_NO == billDetailsViewModel.TXN_NO).FirstOrDefault();


                if (txnId == 179 || txnId == 1191 || txnId == 1190 || txnId == 572)//changes by Koustubh Nakate on 08/10/2013 txnId=472 to 572 
                {
                    ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS map_details = new ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS();
                    String[] strParams = billDetailsViewModel.IMPREST_BILL_ID.Split('/');
                    strParameters = URLEncrypt.DecryptParameters(new string[] { strParams[0], strParams[1], strParams[2] });
                    map_details.P_BILL_ID = Convert.ToInt64(strParameters[0].Split('$')[0]);
                    map_details.S_BIll_ID = billDetailsViewModel.BILL_ID;

                    //new change
                    map_details.P_TXN_ID = Convert.ToInt16(strParameters[0].Split('$')[1]);
                    //end change
                    //map_details.P_TXN_ID = acc_bill_details.TXN_NO;
                    map_details.S_TXN_ID = acc_bill_details.TXN_NO;
                    if (!dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Where(m => m.P_BILL_ID == map_details.P_BILL_ID && m.P_TXN_ID == map_details.P_TXN_ID && m.S_BIll_ID == map_details.S_BIll_ID && m.S_TXN_ID == map_details.S_TXN_ID).Any())
                    {
                        //ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS new_details = dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Where(m => m.P_BILL_ID == map_details.P_BILL_ID && m.P_TXN_ID == map_details.P_TXN_ID && m.S_BIll_ID == map_details.S_BIll_ID && m.S_TXN_ID == map_details.S_TXN_ID).FirstOrDefault();
                        //new_details.P_BILL_ID = map_details.P_BILL_ID;
                        //new_details.P_TXN_ID = map_details.P_TXN_ID;
                        //dbContext.Entry(new_details).State = System.Data.Entity.EntityState.Modified;
                        //map_details.MAP_ID = dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Max(m => m.MAP_ID) + 1;
                        if (dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Any())
                        {
                            map_details.MAP_ID = dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Max(m => m.MAP_ID) + 1;
                        }
                        else
                        {
                            map_details.MAP_ID = 1;
                        }

                        //ADDED By Abhishek kamble 28-nov-2013
                        map_details.USERID = PMGSYSession.Current.UserId;
                        map_details.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                        dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Add(map_details);
                        dbContext.SaveChanges();
                    }
                    
                    //if (!dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Where(m => m.P_BILL_ID == map_details.P_BILL_ID && m.S_BIll_ID == map_details.S_BIll_ID && m.P_TXN_ID == map_details.P_TXN_ID).Any())
                    //{
                    //    dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Add(map_details);
                    //    dbContext.SaveChanges();
                    //}
                }

                if ((old_acc_bill_details.TXN_ID == 179 && txnId != 179) || (old_acc_bill_details.TXN_ID == 1191 && txnId != 1191) || (old_acc_bill_details.TXN_ID == 1190 && txnId != 1190) || (old_acc_bill_details.TXN_ID == 572 && txnId != 572)) //changes by Koustubh Nakate on 08/10/2013 txnId=472 to 572 
                {
                    ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS map_details = new ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS();

                    map_details = dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Where(m => m.S_BIll_ID == old_acc_bill_details.BILL_ID && m.S_TXN_ID == old_acc_bill_details.TXN_NO).FirstOrDefault();
                    if (map_details != null)
                    {

                        //ADDED By Abhishek kamble 28-nov-2013
                        map_details.USERID = PMGSYSession.Current.UserId;
                        map_details.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        dbContext.Entry(map_details).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();

                        dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Remove(map_details);
                        dbContext.SaveChanges();
                    }
                }
                else if ((old_acc_bill_details.TXN_ID == 179 && txnId == 179) || (old_acc_bill_details.TXN_ID == 1191 && txnId == 1191) || (old_acc_bill_details.TXN_ID == 1190 && txnId == 1190) || (old_acc_bill_details.TXN_ID == 572 && txnId == 572)) //changes by Koustubh Nakate on 08/10/2013 txnId=472 to 572 
                {
                    ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS map_details = new ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS();
                    map_details = dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Where(m => m.S_BIll_ID == old_acc_bill_details.BILL_ID && m.S_TXN_ID == old_acc_bill_details.TXN_NO).FirstOrDefault();
                    if (map_details != null)
                    {

                        //ADDED By Abhishek kamble 28-nov-2013
                        map_details.USERID = PMGSYSession.Current.UserId;
                        map_details.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        dbContext.Entry(map_details).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();

                        dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Remove(map_details);
                        dbContext.SaveChanges();
                    }
                }

                commomFuncObj = new CommonFunctions();
                TransactionParams objParam = new TransactionParams();
                objParam.TXN_ID = txnId;
                ACC_SCREEN_DESIGN_PARAM_DETAILS objDesignDetails = new ACC_SCREEN_DESIGN_PARAM_DETAILS();
                objDesignDetails = commomFuncObj.getDetailsDesignParam(objParam);
                if (objDesignDetails == null)
                {
                    objDesignDetails = setDefaultDesignParams();
                }
                if (objDesignDetails.CON_REQ == "Y")
                {
                    acc_bill_details.MAST_CON_ID = billDetailsViewModel.MAST_CON_ID;
                }
                else
                {
                    acc_bill_details.MAST_CON_ID = null;
                }

                if (objDesignDetails.AGREEMENT_REQ == "Y")
                {
                    acc_bill_details.IMS_AGREEMENT_CODE = billDetailsViewModel.IMS_AGREEMENT_CODE;
                }
                else
                {
                    acc_bill_details.IMS_AGREEMENT_CODE = null;
                }

                if (objDesignDetails.PIU_REQ == "Y")
                {
                    //old
                    //acc_bill_details.ADMIN_ND_CODE = billDetailsViewModel.ADMIN_ND_CODE;                    
                    acc_bill_details.ADMIN_ND_CODE = billDetailsViewModel.ADMIN_ND_CODE==0?null:billDetailsViewModel.ADMIN_ND_CODE;
                }
                else
                {
                    acc_bill_details.ADMIN_ND_CODE = null;
                }

                dbContext.Entry(old_acc_bill_details).CurrentValues.SetValues(acc_bill_details);
                dbContext.SaveChanges();


                ACC_BILL_DETAILS new_acc_bill_details = new ACC_BILL_DETAILS();

                new_acc_bill_details.BILL_ID = acc_bill_details.BILL_ID;
                new_acc_bill_details.MAST_CON_ID = acc_bill_details.MAST_CON_ID;
                new_acc_bill_details.IMS_AGREEMENT_CODE = acc_bill_details.IMS_AGREEMENT_CODE;
                new_acc_bill_details.ADMIN_ND_CODE = acc_bill_details.ADMIN_ND_CODE;
                new_acc_bill_details.AMOUNT = acc_bill_details.AMOUNT;
                new_acc_bill_details.NARRATION = acc_bill_details.NARRATION;
                new_acc_bill_details.CASH_CHQ = acc_bill_details.CASH_CHQ;
                new_acc_bill_details.HEAD_ID = (from item in dbContext.ACC_TXN_HEAD_MAPPING
                                                where item.TXN_ID == txnId && item.CASH_CHQ.Contains(acc_bill_details.CASH_CHQ) && item.CREDIT_DEBIT == "D"
                                                select item.HEAD_ID).FirstOrDefault();
                new_acc_bill_details.CREDIT_DEBIT = "D";
                new_acc_bill_details.TXN_NO = Convert.ToInt16(acc_bill_details.TXN_NO + 1);
                new_acc_bill_details.TXN_ID = acc_bill_details.TXN_ID;

                //ADDED By Abhishek kamble 28-nov-2013
                new_acc_bill_details.USERID = PMGSYSession.Current.UserId;
                new_acc_bill_details.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
               
                old_acc_bill_details = null;
                old_acc_bill_details = dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == billDetailsViewModel.BILL_ID && m.TXN_NO == new_acc_bill_details.TXN_NO).FirstOrDefault();

                //if (txnId == 179 || txnId == 1191 || txnId == 1190 || txnId == 572)//changes by Koustubh Nakate on 08/10/2013 txnId=472 to 572 
                if ((old_acc_bill_details.TXN_ID != 179 && txnId == 179) || (old_acc_bill_details.TXN_ID != 1191 && txnId == 1191) || (old_acc_bill_details.TXN_ID != 1190 && txnId == 1190) || (old_acc_bill_details.TXN_ID != 572 && txnId == 572)) //changes by Koustubh Nakate on 08/10/2013 txnId=472 to 572 
                {
                    ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS map_details = new ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS();
                    String[] strParams = billDetailsViewModel.IMPREST_BILL_ID.Split('/');
                    strParameters = URLEncrypt.DecryptParameters(new string[] { strParams[0], strParams[1], strParams[2] });
                    map_details.P_BILL_ID = Convert.ToInt64(strParameters[0].Split('$')[0]);
                    map_details.S_BIll_ID = billDetailsViewModel.BILL_ID;
                    //map_details.P_TXN_ID = acc_bill_details.TXN_NO;
                    map_details.S_TXN_ID = acc_bill_details.TXN_NO;
                    map_details.P_TXN_ID = Convert.ToInt16(strParameters[0].Split('$')[1]);
                    if (!dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Where(m => m.P_BILL_ID == map_details.P_BILL_ID && m.S_BIll_ID == map_details.S_BIll_ID && m.P_TXN_ID == map_details.P_TXN_ID).Any())
                    {
                        //map_details.MAP_ID = dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Max(m => m.MAP_ID) + 1;
                        if (dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Any())
                        {
                            map_details.MAP_ID = dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Max(m => m.MAP_ID) + 1;
                        }
                        else
                        {
                            map_details.MAP_ID = 1;
                        }

                        //ADDED By Abhishek kamble 28-nov-2013
                        map_details.USERID = PMGSYSession.Current.UserId;
                        map_details.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                        dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Add(map_details);
                        dbContext.SaveChanges();
                    }
                }

                //new change done by Vikram on 30 Jan 2014 as if the same data is updated the entry was removed from mapping table

                if ((old_acc_bill_details.TXN_ID == 179 && txnId == 179) || (old_acc_bill_details.TXN_ID == 1191 && txnId == 1191) || (old_acc_bill_details.TXN_ID == 1190 && txnId == 1190) || (old_acc_bill_details.TXN_ID == 572 && txnId == 572)) //changes by Koustubh Nakate on 08/10/2013 txnId=472 to 572 
                {
                    ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS map_details = new ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS();
                    String[] strParams = billDetailsViewModel.IMPREST_BILL_ID.Split('/');
                    strParameters = URLEncrypt.DecryptParameters(new string[] { strParams[0], strParams[1], strParams[2] });
                    map_details.P_BILL_ID = Convert.ToInt64(strParameters[0].Split('$')[0]);
                    map_details.S_BIll_ID = billDetailsViewModel.BILL_ID;
                    //map_details.P_TXN_ID = acc_bill_details.TXN_NO;
                    map_details.S_TXN_ID = acc_bill_details.TXN_NO;
                    map_details.P_TXN_ID = Convert.ToInt16(strParameters[0].Split('$')[1]);
                    if (!dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Where(m => m.P_BILL_ID == map_details.P_BILL_ID && m.S_BIll_ID == map_details.S_BIll_ID && m.P_TXN_ID == map_details.P_TXN_ID).Any())
                    {
                        //map_details.MAP_ID = dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Max(m => m.MAP_ID) + 1;
                        if (dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Any())
                        {
                            map_details.MAP_ID = dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Max(m => m.MAP_ID) + 1;
                        }
                        else
                        {
                            map_details.MAP_ID = 1;
                        }

                        //ADDED By Abhishek kamble 28-nov-2013
                        map_details.USERID = PMGSYSession.Current.UserId;
                        map_details.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                        dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Add(map_details);
                        dbContext.SaveChanges();
                    }
                }

                //end of change



                if ((old_acc_bill_details.TXN_ID == 179 && txnId != 179) || (old_acc_bill_details.TXN_ID == 1191 && txnId != 1191) || (old_acc_bill_details.TXN_ID == 1190 && txnId != 1190) || (old_acc_bill_details.TXN_ID == 572 && txnId != 572)) //changes by Koustubh Nakate on 08/10/2013 txnId=472 to 572 
                {
                    ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS map_details = new ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS();
                    if (!billDetailsViewModel.IMPREST_BILL_ID.Equals('0'))
                    {
                        String[] strParams = billDetailsViewModel.IMPREST_BILL_ID.Split('/');
                        strParameters = URLEncrypt.DecryptParameters(new string[] { strParams[0], strParams[1], strParams[2] });

                        long P_BILL_ID = Convert.ToInt64(strParameters[0].Split('$')[0]);



                        Int16 P_TXN_ID = Convert.ToInt16(strParameters[0].Split('$')[1]);

                        map_details = dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Where(m => m.S_BIll_ID == old_acc_bill_details.BILL_ID && m.P_TXN_ID == P_TXN_ID && m.P_BILL_ID == P_BILL_ID).FirstOrDefault();
                    }
                    else
                    {
                        map_details = dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Where(m => m.S_BIll_ID == old_acc_bill_details.BILL_ID && m.S_TXN_ID == 1).FirstOrDefault();
                    }
                    if (map_details != null)
                    {
                        //ADDED By Abhishek kamble 28-nov-2013
                        map_details.USERID = PMGSYSession.Current.UserId;
                        map_details.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        dbContext.Entry(map_details).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();

                        dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Remove(map_details);
                        dbContext.SaveChanges();
                    }
                }

                dbContext.Entry(old_acc_bill_details).CurrentValues.SetValues(new_acc_bill_details);
                dbContext.SaveChanges();
                return "";
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

        public BillDetailsViewModel GetReceiptDetailByTransNo(Int64 billId, Int16 TransNo)
        {
            try
            {
                dbContext = new PMGSYEntities();
                ACC_BILL_DETAILS acc_bill_details = new ACC_BILL_DETAILS();
                BillDetailsViewModel billDetailsViewModel = new BillDetailsViewModel();
                acc_bill_details = dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == billId && m.TXN_NO == TransNo).FirstOrDefault();
                billDetailsViewModel = CloneObject(acc_bill_details);
                billDetailsViewModel.TXN_ID = acc_bill_details.TXN_ID.ToString().Trim() + "$" + (acc_bill_details.ACC_MASTER_TXN.CASH_CHQ.Trim());
                return billDetailsViewModel;
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

        public ACC_BILL_DETAILS CloneModel(BillDetailsViewModel billDetailsViewModel)
        {
            try
            {
                dbContext = new PMGSYEntities();
                ACC_BILL_DETAILS acc_bill_Details = new ACC_BILL_DETAILS();
                acc_bill_Details.BILL_ID = billDetailsViewModel.BILL_ID;
                acc_bill_Details.AMOUNT = Convert.ToDecimal(billDetailsViewModel.AMOUNT);
                acc_bill_Details.NARRATION = billDetailsViewModel.NARRATION;
                acc_bill_Details.CASH_CHQ = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == acc_bill_Details.BILL_ID).Select(m => m.CHQ_EPAY).FirstOrDefault(); //billDetailsViewModel.HEAD_ID.Split('$')[1].Trim();
                acc_bill_Details.TXN_NO = billDetailsViewModel.TXN_NO;
                acc_bill_Details.TXN_ID = Convert.ToInt16(billDetailsViewModel.TXN_ID.Trim().Split('$')[0]);

                //ADDED By Abhishek kamble 28-nov-2013
                acc_bill_Details.USERID = PMGSYSession.Current.UserId;
                acc_bill_Details.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                return acc_bill_Details;
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

        public BillDetailsViewModel CloneObject(ACC_BILL_DETAILS acc_bill_details)
        {
            BillDetailsViewModel billDetailsViewModel = new BillDetailsViewModel();
            billDetailsViewModel.BILL_ID = acc_bill_details.BILL_ID;
            billDetailsViewModel.TXN_NO = acc_bill_details.TXN_NO;
            billDetailsViewModel.ADMIN_ND_CODE = acc_bill_details.ADMIN_ND_CODE;
            billDetailsViewModel.MAST_CON_ID = acc_bill_details.MAST_CON_ID;
            billDetailsViewModel.IMS_AGREEMENT_CODE = acc_bill_details.IMS_AGREEMENT_CODE;
            billDetailsViewModel.AMOUNT = acc_bill_details.AMOUNT;
            billDetailsViewModel.NARRATION = acc_bill_details.NARRATION;
            billDetailsViewModel.CASH_CHQ = acc_bill_details.CASH_CHQ;
            return billDetailsViewModel;
        }

        public String DeleteReceiptDetails(Int64 billId, Int16 TransNo)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    dbContext = new PMGSYEntities();
                    ACC_BILL_DETAILS acc_bill_details = new ACC_BILL_DETAILS();
                    acc_bill_details = dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == billId && m.TXN_NO == TransNo).FirstOrDefault();
                    if (acc_bill_details.TXN_ID == 179 || acc_bill_details.TXN_ID == 1191 || acc_bill_details.TXN_ID == 1190 || acc_bill_details.TXN_ID == 572)//changes by Koustubh Nakate on 08/10/2013 txnId=472 to 572 
                    {
                        ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS map_details = new ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS();

                        map_details = dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Where(m => m.S_BIll_ID == acc_bill_details.BILL_ID && m.S_TXN_ID == acc_bill_details.TXN_NO).FirstOrDefault();
                        if (map_details != null)
                        {
                            //ADDED By Abhishek kamble 28-nov-2013
                            map_details.USERID = PMGSYSession.Current.UserId;
                            map_details.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                            dbContext.Entry(map_details).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();

                            dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Remove(map_details);
                            dbContext.SaveChanges();
                        }
                    }

                    //ADDED By Abhishek kamble 28-nov-2013
                    acc_bill_details.USERID = PMGSYSession.Current.UserId;
                    acc_bill_details.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    dbContext.Entry(acc_bill_details).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    dbContext.ACC_BILL_DETAILS.Remove(acc_bill_details);
                    dbContext.SaveChanges();
                    TransNo = Convert.ToInt16(TransNo + 1);
                    acc_bill_details = dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == billId && m.TXN_NO == TransNo).FirstOrDefault();

                    if (acc_bill_details.TXN_ID == 179 || acc_bill_details.TXN_ID == 1191 || acc_bill_details.TXN_ID == 1190 || acc_bill_details.TXN_ID == 572) //changes by Koustubh Nakate on 08/10/2013 txnId=472 to 572 
                    {
                        ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS map_details = new ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS();

                        map_details = dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Where(m => m.S_BIll_ID == acc_bill_details.BILL_ID && m.S_TXN_ID == acc_bill_details.TXN_NO).FirstOrDefault();
                        if (map_details != null)
                        {
                            //ADDED By Abhishek kamble 28-nov-2013
                            map_details.USERID = PMGSYSession.Current.UserId;
                            map_details.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                            dbContext.Entry(map_details).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();

                            dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Remove(map_details);
                            dbContext.SaveChanges();
                        }
                    }

                    //ADDED By Abhishek kamble 28-nov-2013
                    acc_bill_details.USERID = PMGSYSession.Current.UserId;
                    acc_bill_details.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    dbContext.Entry(acc_bill_details).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    dbContext.ACC_BILL_DETAILS.Remove(acc_bill_details);
                    dbContext.SaveChanges();
                }
                catch (TransactionException ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    return ex.Message;
                }
                 catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    throw ex;
                }
                finally
                {
                    scope.Complete();
                    dbContext.Dispose();
                }
            }
            return String.Empty;
        }

        public String FinalizeReceipt(Int64 billId)
        {
            string fundType = PMGSYSession.Current.FundType;
            int adminNdCode = PMGSYSession.Current.AdminNdCode;
            short levelID = PMGSYSession.Current.LevelId;

            try
            {
                dbContext = new PMGSYEntities();
                decimal? detailsAmount = dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == billId && m.CREDIT_DEBIT == "D").Sum(m => (decimal?)m.AMOUNT);
                detailsAmount = detailsAmount == null ? 0 : detailsAmount;
                if (dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == billId).Sum(m => m.GROSS_AMOUNT) == detailsAmount)
                {
                    //added by Koustubh Nakate on 21/08/2013 to save notification in notification details table 
                    using (var scope = new TransactionScope())
                    {
                        ACC_BILL_MASTER acc_bill_master = dbContext.ACC_BILL_MASTER.Find(billId);
                        acc_bill_master.BILL_FINALIZED = "Y";
                        acc_bill_master.ACTION_REQUIRED = "N";

                        //ADDED By Abhishek kamble 28-nov-2013
                        acc_bill_master.USERID = PMGSYSession.Current.UserId;
                        acc_bill_master.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        
                        dbContext.Entry(acc_bill_master).State = System.Data.Entity.EntityState.Modified;

                        dbContext.SaveChanges();

                        //var result = dbContext.USP_ACC_INSERT_ALERT_DETAILS(adminNdCode, fundType, "R", levelID, billId);
                        scope.Complete();
                        return String.Empty;
                    }
                }
                else
                {
                    return "Cannot Finalize, Master Amount and Details Amount Does not Match.";
                }
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

        public String DeleteReceipt(Int64 billId)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    dbContext = new PMGSYEntities();
                    List<ACC_BILL_DETAILS> lstBillDetails = dbContext.ACC_BILL_DETAILS.Where(w => w.BILL_ID == billId).ToList<ACC_BILL_DETAILS>();
                    foreach (ACC_BILL_DETAILS item in lstBillDetails)
                    {
                        //ADDED By Abhishek kamble 28-nov-2013
                        item.USERID = PMGSYSession.Current.UserId;
                        item.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        dbContext.Entry(item).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();

                        dbContext.ACC_BILL_DETAILS.Remove(item);
                    }
                    //dbContext.SaveChanges();


                    //ADDED By Abhishek kamble 28-nov-2013 
                    ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS imprestPaymentSettlementDetails = dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Where(m => m.S_BIll_ID == billId).FirstOrDefault();
                    if (imprestPaymentSettlementDetails != null)
                    {
                        imprestPaymentSettlementDetails.USERID = PMGSYSession.Current.UserId;
                        imprestPaymentSettlementDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        dbContext.Entry(imprestPaymentSettlementDetails).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();  
                    }

                    //delete the entry from [ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS]
                    dbContext.Database.ExecuteSqlCommand
                        ("DELETE [omms].ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS Where S_BIll_ID = {0}", billId);



                    //ADDED By Abhishek kamble 28-nov-2013 
                    ACC_NOTIFICATION_DETAILS notificationDetails = dbContext.ACC_NOTIFICATION_DETAILS.Where(m => m.INITIATION_BILL_ID == billId).FirstOrDefault();
                    if (notificationDetails != null)
                    {
                        notificationDetails.USERID = PMGSYSession.Current.UserId;
                        notificationDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        dbContext.Entry(notificationDetails).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                    }


                    //added by koustubh nakate on 21/08/2013 to delete details from ACC_NOTIFICATION_DETAILS             
                    dbContext.Database.ExecuteSqlCommand
                       ("DELETE [omms].ACC_NOTIFICATION_DETAILS Where INITIATION_BILL_ID = {0}", billId);

                    //   dbContext.ACC_NOTIFICATION_DETAILS.ToList<ACC_NOTIFICATION_DETAILS>().RemoveAll(nd => nd.INITIATION_BILL_ID == billId);


                    ACC_BILL_MASTER acc_bill_master = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == billId).FirstOrDefault();

                    acc_bill_master.USERID = PMGSYSession.Current.UserId;
                    acc_bill_master.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    dbContext.Entry(acc_bill_master).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    dbContext.ACC_BILL_MASTER.Remove(acc_bill_master);
                    // dbContext.SaveChanges();

                    dbContext.SaveChanges();
                }
                catch (TransactionException ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    return ex.Message;
                }
                 catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    return ex.Message;
                }
                finally
                {
                    scope.Complete();
                    dbContext.Dispose();
                }
            }
            return String.Empty;
        }

        public Decimal GetAmountAvailable(Int64 billId, Int16 transId)
        {
            try
            {
                dbContext = new PMGSYEntities();
                decimal oldAmount = 0;
                if (transId != 0)
                {
                    oldAmount = dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == billId && m.TXN_NO == transId).Select(m => m.AMOUNT).FirstOrDefault();
                }
                decimal grossAmount = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == billId).Sum(m => m.GROSS_AMOUNT);
                decimal? detailsAmount = dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == billId && m.CREDIT_DEBIT == "D").Sum(m => (decimal?)m.AMOUNT);
                return (detailsAmount == null ? grossAmount : Convert.ToDecimal(grossAmount - (detailsAmount - oldAmount)));
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

        public String ValidateAddReceiptDetails(BillDetailsViewModel billDetailsViewModel, String Op_mode)
        {
            Decimal maxAvailableAmount = GetAmountAvailable(billDetailsViewModel.BILL_ID, billDetailsViewModel.TXN_NO);
            CommonFunctions commonFuncObj = new CommonFunctions();
            Int16 txnId = Convert.ToInt16(billDetailsViewModel.TXN_ID.Split('$')[0]);
            dbContext = new PMGSYEntities();

            if (billDetailsViewModel.AMOUNT > maxAvailableAmount)
            {
                return "Amount must not be greater than " + maxAvailableAmount;
            }
            if (txnId == 179 || txnId == 1191 || txnId == 1190 || txnId == 572) //changes by Koustubh Nakate on 08/10/2013 txnId=472 to 572 
            {
                if (billDetailsViewModel.IMPREST_BILL_ID.Equals("0") || string.IsNullOrEmpty(billDetailsViewModel.IMPREST_BILL_ID))
                {
                    return "Please select Unsetteled Imprest Voucher.";
                }
                String[] strParams = billDetailsViewModel.IMPREST_BILL_ID.Split('/');
                String[] strParameters = URLEncrypt.DecryptParameters(new string[] { strParams[0], strParams[1], strParams[2] });
                Int64 pBillID = Convert.ToInt64(strParameters[0].Split('$')[0]);
                TransferEntryOrderDAL objTEODAL = new TransferEntryOrderDAL();
                
                //added by Koustubh Nakate on 19/09/2013 to check imprest settelement amount not greater than enterd amount
                 Int16 TXN_No = Convert.ToInt16(strParameters[0].Split('$')[1]);
                if (Op_mode.Equals("A"))
                {
                   
                    string unsetteledAmount = GetImprestAmount(pBillID, TXN_No);

                    dbContext = new PMGSYEntities();

                    //Decimal? grossAmount = dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == pBillID && m.TXN_NO == TXN_No).Select(m => m.AMOUNT).FirstOrDefault();

                    // grossAmount=grossAmount==null?0:grossAmount;

                    // Decimal? unsetteledAmount=grossAmount- Convert.ToDecimal(setteledAmount);

                   
                    if (billDetailsViewModel.AMOUNT > Convert.ToDecimal(unsetteledAmount))
                    {
                        return "Amount Exceeds the imprest gross amount.";
                    }

                }
                else if (Op_mode.Equals("E"))
                {
                    List<ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS> lstBillIds = new List<ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS>();
                    Decimal? amountSettled = 0;
                     // dbContext = new PMGSYEntities();

                    Decimal? grossAmount = dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == pBillID && m.TXN_NO == TXN_No).Select(m => m.AMOUNT).FirstOrDefault();

                    amountSettled = (from billDetails in dbContext.ACC_BILL_DETAILS
                                     join settelementDetails in dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS
                                         // on new { billDetails.BILL_ID, billDetails.TXN_NO } equals new { settelementDetails.S_BIll_ID, settelementDetails.P_TXN_ID }
                                     on billDetails.BILL_ID equals settelementDetails.S_BIll_ID
                                     where
                                    // billDetails.TXN_NO == settelementDetails.P_TXN_ID &&
                                     settelementDetails.P_BILL_ID == pBillID &&
                                     (billDetails.BILL_ID != billDetailsViewModel.BILL_ID ) &&
                                     billDetails.CREDIT_DEBIT=="C"
                                     select (Decimal?)billDetails.AMOUNT).Sum();


                    //var lstBillDetails = (from settelementDetails in dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS
                    //                      where
                    //                      settelementDetails.P_BILL_ID == pBillID &&
                    //                      settelementDetails.S_BIll_ID != billDetailsViewModel.BILL_ID
                    //                      select settelementDetails);


                    //amountSettled = (from billDetails in dbContext.ACC_BILL_DETAILS
                    //                 where lstBillDetails.Any(lst => lst.S_BIll_ID == billDetails.BILL_ID)//&& lst.P_TXN_ID == billDetails.TXN_NO)
                    //                 select (Decimal?)billDetails.AMOUNT).Sum();

                

                    Decimal? remainingUnsettledAmount = (grossAmount == null ? 0 : grossAmount) - (amountSettled == null ? 0 : amountSettled);

                    if (billDetailsViewModel.AMOUNT > remainingUnsettledAmount)
                    {
                        return "Amount Exceeds the imprest gross amount.";
                    }
                   
                }
                //end added by Koustubh Nakate on 19/09/2013 to check imprest settelement amount not greater than enterd amount

                if (objTEODAL.IsImprestAmountValidMultipleForAddEdit(pBillID, Convert.ToDecimal(billDetailsViewModel.AMOUNT), billDetailsViewModel.BILL_ID, Op_mode))
                {
                    DateTime billDate = dbContext.ACC_BILL_MASTER.Where(x => x.BILL_ID == billDetailsViewModel.BILL_ID).Select(x => x.BILL_DATE).FirstOrDefault();

                    //validation for date date of imprest should be greater than imprest payment/ ob date
                    if (!commonFuncObj.isValidImprestSttlementDate(pBillID, billDate))
                    {
                        return "Receipt Date should be greater than date of imprest payment or opening balance date.";
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return "Amount Exceeds the remaining Imprest gross amount.";
                }




            }
            else
            {
                return String.Empty;
            }

        }

        public String ValidateEditReceiptMaster(BillMasterViewModel billMasterViewModel)
        {
            try
            {
                dbContext = new PMGSYEntities();
                String validationSummary = String.Empty;
                Int64 count = dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == billMasterViewModel.BILL_ID && m.CREDIT_DEBIT == "D").Count();
                if (count == 0)
                {
                    return String.Empty;
                }
                else
                {
                    Decimal detailsAmount = dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == billMasterViewModel.BILL_ID && m.CREDIT_DEBIT == "D").Sum(m => m.AMOUNT);
                    ACC_BILL_MASTER acc_bill_master = new ACC_BILL_MASTER();
                    acc_bill_master = (from item in dbContext.ACC_BILL_MASTER
                                       where item.BILL_ID == billMasterViewModel.BILL_ID
                                       select item).FirstOrDefault();
                    if (detailsAmount > billMasterViewModel.GROSS_AMOUNT)
                    {
                        validationSummary = "Amount must not be less than " + detailsAmount;
                    }
                    else if (acc_bill_master.TXN_ID != Convert.ToInt16(billMasterViewModel.TXN_ID.Split('$')[0]))
                    {
                        //String transName = dbContext.ACC_MASTER_TXN.Where(m => m.TXN_ID == acc_bill_master.TXN_ID).Select(m => m.TXN_DESC).FirstOrDefault();
                        if (validationSummary == "")
                        {
                            validationSummary = "Cannot modify.Transaction details present for '" + acc_bill_master.ACC_MASTER_TXN.TXN_DESC + "' master transaction";
                        }
                        else
                        {
                            validationSummary += "<br/>Cannot modify.Transaction details present for '" + acc_bill_master.ACC_MASTER_TXN.TXN_DESC + "' master transaction";
                        }
                    }
                    else if (acc_bill_master.CHQ_EPAY != billMasterViewModel.CHQ_EPAY)
                    {
                        String cashCheque = String.Empty;
                        if (acc_bill_master.CHQ_EPAY == "C")
                        {
                            cashCheque = "Cash";
                        }
                        else if (acc_bill_master.CHQ_EPAY == "Q")
                        {
                            cashCheque = "Cheque";
                        }

                        if (validationSummary == "")
                        {
                            validationSummary = "Cannot modify.Transaction details present for '" + cashCheque + "' mode";
                        }
                        else
                        {
                            validationSummary += "<br/>Cannot modify.Transaction details present for '" + cashCheque + "' mode";
                        }
                    }

                    return validationSummary;
                }
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

        public Boolean IsDuplicateBillNo(Int16 month, Int16 year, Int32 adminNdCode, Int16 levelId, String fundType, String billType, String billNo)
        {
            try
            {
                dbContext = new PMGSYEntities();
                return dbContext.ACC_BILL_MASTER.Where(m => m.ADMIN_ND_CODE == adminNdCode && m.LVL_ID == levelId && m.FUND_TYPE == fundType && m.BILL_TYPE == billType && m.BILL_MONTH == month && m.BILL_YEAR == year && m.BILL_NO.ToUpper() == billNo.ToUpper()).Any();
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


        public List<SelectListItem> GetUnSettledVouchers(Int16 month, Int16 year, Int64 pBillId)
        {
            dbContext = new PMGSYEntities();
            List<SelectListItem> lstUnsettledVouchers = new List<SelectListItem>();
            Int32 AdminNdCode = PMGSYSession.Current.AdminNdCode;
            Int16 LevelId = PMGSYSession.Current.LevelId;
            String FundType = PMGSYSession.Current.FundType;
            commomFuncObj = new CommonFunctions();
            Int16 HeadId = 0;
            Int16 TransId = 0;
            if (LevelId == 5)
            {
                TransId = FundType == "P" ? Convert.ToInt16(118) : FundType == "A" ? Convert.ToInt16(472) : Convert.ToInt16(808);
            }
            else if (LevelId == 4)
            {
                TransId = FundType == "A" ? Convert.ToInt16(390) : Convert.ToInt16(0);
            }

            HeadId = FundType == "P" ? Convert.ToInt16(46) : FundType == "A" ? Convert.ToInt16(82) : Convert.ToInt16(328);

            List<string> BilType = new List<string>();
            BilType.Add("P");
            BilType.Add("O");
            List<ListImprest> lstImprest = null;

            //lstImprest = (from bm in dbContext.ACC_BILL_MASTER
            //              join bd in dbContext.ACC_BILL_DETAILS on bm.BILL_ID equals bd.BILL_ID
            //              where bm.ADMIN_ND_CODE == AdminNdCode &&
            //              //bm.LVL_ID == LevelId && (bm.BILL_TYPE == "P" ? bm.TXN_ID : 1) == (bm.BILL_TYPE == "P" ? TransId : 1) && bm.BILL_FINALIZED == "Y" && BilType.Contains(bm.BILL_TYPE) &&
            //              //(bm.BILL_TYPE == "O" ? bd.HEAD_ID : 1) == (bm.BILL_TYPE == "O" ? HeadId : 1) && bm.BILL_MONTH <= month && bm.BILL_YEAR <= year && bd.CREDIT_DEBIT == "D" && bm.TXN_ID == TransId
            //              bm.BILL_TYPE == "O" && bd.HEAD_ID == HeadId && bm.BILL_MONTH <= month && bm.BILL_YEAR <= year && bd.CREDIT_DEBIT == "D" && bm.TXN_ID == TransId
            //              select new ListImprest
            //              {
            //                  BILL_ID = bm.BILL_ID,
            //                  BILL_NO = bm.BILL_NO,
            //                  GROSS_AMOUNT = bm.GROSS_AMOUNT,
            //                  PAYEE_NAME = bm.BILL_TYPE == "P" ? bm.PAYEE_NAME : bd.NARRATION,
            //                  //PAYEE_NAME = bd.NARRATION,
            //                  C_SETTLED_AMOUNT = 0,
            //                  D_SETTLED_AMOUNT = 0,
            //                  P_TXN_ID = bd.TXN_NO,
            //                  BILL_TYPE = bm.BILL_TYPE,
            //                  BILL_DATE = (bm.BILL_DATE)
            //              }).ToList<ListImprest>();
            short txnNo = 2;
            //old Code 
            //lstImprest = (from bm in dbContext.ACC_BILL_MASTER
            //              join bd in dbContext.ACC_BILL_DETAILS on bm.BILL_ID equals bd.BILL_ID
            //              where bm.ADMIN_ND_CODE == AdminNdCode &&
            //              bm.LVL_ID == LevelId && (bm.BILL_TYPE == "P" ? bm.TXN_ID : 1) == (bm.BILL_TYPE == "P" ? TransId : 1) && bm.BILL_FINALIZED == "Y" && BilType.Contains(bm.BILL_TYPE)
            //              && (bm.BILL_TYPE == "O" ? bd.HEAD_ID : 1) == (bm.BILL_TYPE == "O" ? HeadId : 1) && bm.BILL_MONTH <= month && bm.BILL_YEAR <= year && bd.CREDIT_DEBIT == "D"
            //new Code modified by Abhishek kamble 11Feb2015 to get Unsettled vouchers
            lstImprest = (from bm in dbContext.ACC_BILL_MASTER
                          join bd in dbContext.ACC_BILL_DETAILS on bm.BILL_ID equals bd.BILL_ID
                          where bm.ADMIN_ND_CODE == AdminNdCode &&
                          bm.LVL_ID == LevelId                         
                          && bm.FUND_TYPE==  PMGSYSession.Current.FundType 
                          &&((bm.BILL_TYPE == "P" || bm.BILL_TYPE == "O") && bd.HEAD_ID == HeadId && (bm.CHQ_EPAY == "Q" || bm.CHQ_EPAY == "O"))
                          //&& (bm.BILL_TYPE == "P" ? bm.TXN_ID : 1) == (bm.BILL_TYPE == "P" ? TransId : 1)
                          //&&  && BilType.Contains(bm.BILL_TYPE)
                         // && (bm.BILL_TYPE == "O" ? bd.HEAD_ID : 1) == (bm.BILL_TYPE == "O" ? HeadId : 1) 
                          //&& bm.BILL_MONTH <= month && bm.BILL_YEAR <= year 
                           && bm.BILL_MONTH + (bm.BILL_YEAR * 12) <= month + (year * 12)
                          && bd.CREDIT_DEBIT == "D"
                          && bm.BILL_FINALIZED == "Y"
                          
                          //new change done by Vikram as 
                          //&&      
                          select new ListImprest
                          {
                              BILL_ID = bm.BILL_ID,
                              BILL_NO = bm.BILL_NO,
                              GROSS_AMOUNT = bm.GROSS_AMOUNT,
                              PAYEE_NAME = bm.BILL_TYPE == "P" ? bm.PAYEE_NAME : bd.NARRATION,
                              C_SETTLED_AMOUNT = 0,
                              D_SETTLED_AMOUNT = 0,
                              P_TXN_ID = (bm.BILL_TYPE == "P"?txnNo: bd.TXN_NO),
                              BILL_TYPE = bm.BILL_TYPE,
                              BILL_DATE = (bm.BILL_DATE)
                          }).ToList<ListImprest>();

            lstImprest = lstImprest.GroupBy(m => new { m.BILL_ID, m.P_TXN_ID }).Select(m => m.FirstOrDefault()).ToList<ListImprest>();

            List<ListImprest> lstToRemove = new List<ListImprest>();
            List<ListImprest> lstFinal = new List<ListImprest>();

            lstFinal = lstImprest;
            
            //lstToRemove = (from bm in dbContext.ACC_BILL_MASTER
            //               //join bd in dbContext.ACC_BILL_DETAILS on bm.BILL_ID equals bd.BILL_ID
            //               where bm.ADMIN_ND_CODE == AdminNdCode &&
            //               bm.LVL_ID == LevelId && (bm.BILL_TYPE == "P" ? bm.TXN_ID : 1) == (bm.BILL_TYPE == "P" ? TransId : 1) && bm.BILL_FINALIZED == "Y" && BilType.Contains(bm.BILL_TYPE) && bm.TXN_ID == TransId
            //               //&& (bm.BILL_TYPE == "O" ? bd.HEAD_ID : 1) == (bm.BILL_TYPE == "O" ? HeadId : 1) && bm.BILL_MONTH <= month && bm.BILL_YEAR <= year && bd.CREDIT_DEBIT == "D"
            //               select new ListImprest
            //               {
            //                   BILL_ID = bm.BILL_ID,
            //                   BILL_NO = bm.BILL_NO,
            //                   GROSS_AMOUNT = bm.GROSS_AMOUNT,
            //                   PAYEE_NAME = bm.PAYEE_NAME,
            //                   C_SETTLED_AMOUNT = 0,
            //                   D_SETTLED_AMOUNT = 0,
            //                   P_TXN_ID = 2,
            //                   BILL_TYPE = bm.BILL_TYPE,
            //                   BILL_DATE = (bm.BILL_DATE)
            //               }).ToList<ListImprest>();


            //lstFinal = lstImprest.Union(lstToRemove).ToList<ListImprest>();


            foreach (ListImprest item in lstFinal)
            {
                List<ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS> lstBillIds = new List<ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS>();
                Decimal amountSettledMaster = 0;
                Decimal amountSettledCredit = 0;
                Decimal amountSettledDebit = 0;
                Decimal amountSettledByTeo = 0;

                //get the settlement vouchers
                if (dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Where(p => p.P_BILL_ID == item.BILL_ID).Any())
                {
                    lstBillIds = dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Where(p => p.P_BILL_ID == item.BILL_ID /*&& p.P_TXN_ID == item.P_TXN_ID*/).ToList<ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS>();
                }

                //dont show the vouchers which are completely settled
                if (lstBillIds.Count > 0)
                {
                    foreach (ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS imprestItem in lstBillIds)
                    {
                        //changes by amol jadhav foe imprest settlement entry
                        if (dbContext.ACC_BILL_MASTER.Where(c => c.BILL_ID == imprestItem.S_BIll_ID).Select(x => x.BILL_TYPE).First() == "J")
                        {

                            if ((from c in dbContext.ACC_BILL_DETAILS where imprestItem.S_BIll_ID == c.BILL_ID && c.CREDIT_DEBIT == "C" select c).Any())
                            {
                                amountSettledCredit += (from c in dbContext.ACC_BILL_DETAILS where imprestItem.S_BIll_ID == c.BILL_ID && c.CREDIT_DEBIT == "C" select c.AMOUNT).Sum();
                            }
                            if ((from c in dbContext.ACC_BILL_DETAILS where imprestItem.S_BIll_ID == c.BILL_ID && c.CREDIT_DEBIT == "D" select c).Any())
                            {
                                decimal val = 0;
                                val = (from c in dbContext.ACC_BILL_DETAILS where imprestItem.S_BIll_ID == c.BILL_ID && c.CREDIT_DEBIT == "D" select c.AMOUNT).Sum();
                                amountSettledDebit += val;
                                amountSettledMaster += val;
                                amountSettledByTeo += val;
                            }
                            item.S_BIll_ID = imprestItem.S_BIll_ID;
                        }
                        else
                        {
                            amountSettledMaster += (from c in dbContext.ACC_BILL_DETAILS where imprestItem.S_BIll_ID == c.BILL_ID && c.TXN_NO == (imprestItem.P_TXN_ID) select c.AMOUNT).FirstOrDefault();
                            //if(dbContext.ACC_BILL_DETAILS.Where(m=>m.BILL_ID))                            
                            if ((from c in dbContext.ACC_BILL_DETAILS where imprestItem.S_BIll_ID == c.BILL_ID && c.TXN_NO == imprestItem.P_TXN_ID select c).Any())
                            {
                                amountSettledCredit += (from c in dbContext.ACC_BILL_DETAILS where imprestItem.S_BIll_ID == c.BILL_ID && c.TXN_NO == (imprestItem.P_TXN_ID) select c.AMOUNT).FirstOrDefault();
                            }
                            if ((from c in dbContext.ACC_BILL_DETAILS where imprestItem.S_BIll_ID == c.BILL_ID && c.TXN_NO == (imprestItem.P_TXN_ID) select c).Any())
                            {
                                amountSettledDebit += (from c in dbContext.ACC_BILL_DETAILS where imprestItem.S_BIll_ID == c.BILL_ID && c.TXN_NO == (imprestItem.P_TXN_ID) select c.AMOUNT).FirstOrDefault();
                            }
                            item.S_BIll_ID = imprestItem.S_BIll_ID;
                        }



                    }
                }


                item.MASTER_SETTLED_AMOUNT = amountSettledMaster;
                item.C_SETTLED_AMOUNT = amountSettledCredit;
                item.D_SETTLED_AMOUNT = amountSettledDebit;

                if (item.BILL_TYPE == "O")
                {
                    item.GROSS_AMOUNT = dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == item.BILL_ID && m.TXN_NO == item.P_TXN_ID).Select(m => m.AMOUNT).FirstOrDefault();
                }

                //if all settlement vouchers are not  finalized

                ACC_BILL_MASTER accBillMaster = dbContext.ACC_BILL_MASTER.Where(acc => acc.BILL_ID == item.S_BIll_ID).FirstOrDefault();

                if (accBillMaster != null && accBillMaster.BILL_FINALIZED.Equals("N")) // (item.GROSS_AMOUNT != amountSettledMaster ) (old condition) changes by koustubh nakate on 11/09/2013 
                {

                    if (item.BILL_ID == pBillId)
                    {
                        lstUnsettledVouchers.Add(new SelectListItem { Value = URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim() + "$" + item.P_TXN_ID }), Text = item.BILL_NO + " - " + commomFuncObj.GetDateTimeToString(item.BILL_DATE) + " - " + item.PAYEE_NAME, Selected = true });
                    }
                    else
                    {
                        lstUnsettledVouchers.Add(new SelectListItem { Value = URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim() + "$" + item.P_TXN_ID }), Text = item.BILL_NO + " - " + commomFuncObj.GetDateTimeToString(item.BILL_DATE) + " - " + item.PAYEE_NAME });
                    }
                }
                else
                {
                    if (item.BILL_ID == pBillId)
                    {
                        lstUnsettledVouchers.Add(new SelectListItem { Value = URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim() + "$" + item.P_TXN_ID }), Text = item.BILL_NO + " - " + commomFuncObj.GetDateTimeToString(item.BILL_DATE) + " - " + item.PAYEE_NAME, Selected = true });
                    }
                    else
                    {
                        lstUnsettledVouchers.Add(new SelectListItem { Value = URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim() + "$" + item.P_TXN_ID }), Text = item.BILL_NO + " - " + commomFuncObj.GetDateTimeToString(item.BILL_DATE) + " - " + item.PAYEE_NAME});
                    }
                }

            }
            lstUnsettledVouchers.Insert(0, (new SelectListItem { Text = "Select Unsettled Voucher", Value = "0" }));

            return lstUnsettledVouchers;
        }

        public String GetImprestAmount(Int64 billId, Int16 TxnId)
        {
            try
            {
                ACC_BILL_MASTER acc_bill_master = new ACC_BILL_MASTER();
                dbContext = new PMGSYEntities();
                List<ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS> lstBillIds = new List<ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS>();
                Decimal amountSettled = 0;
                Decimal grossAmount = 0;
                if (dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == billId).Select(m => m.BILL_TYPE).FirstOrDefault() == "O")
                {

                    grossAmount = dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == billId && m.TXN_NO == TxnId).Select(m => m.AMOUNT).FirstOrDefault();
                }
                else
                {
                    grossAmount = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == billId).Select(m => m.GROSS_AMOUNT).FirstOrDefault();
                }
                if (dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Where(p => p.P_BILL_ID == billId).Any())
                {
                    //lstBillIds = dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Where(p => p.P_BILL_ID == billId && p.P_TXN_ID == TxnId).ToList<ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS>();
                    lstBillIds = dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Where(p => p.P_BILL_ID == billId && p.P_TXN_ID == TxnId).ToList<ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS>();
                }

                if (lstBillIds.Count > 0)
                {
                    foreach (ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS item in lstBillIds)
                    {
                        //added by amol jadhav
                        if (dbContext.ACC_BILL_MASTER.Where(c => c.BILL_ID == item.S_BIll_ID).Select(x => x.BILL_TYPE).First() == "J")
                        {
                            amountSettled += (from c in dbContext.ACC_BILL_MASTER where c.BILL_ID == item.S_BIll_ID select c.GROSS_AMOUNT).FirstOrDefault();

                        }
                        else
                        {
                            amountSettled += dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == item.S_BIll_ID && m.TXN_NO == item.S_TXN_ID).Select(m => m.AMOUNT).FirstOrDefault();
                        }
                    }
                    //amountSettled = (from c in dbContext.ACC_BILL_DETAILS
                    //                 where
                    //                 lstBillIds.Contains( c.BILL_ID)
                    //                 select c.GROSS_AMOUNT).Sum();

                }
                return (grossAmount - amountSettled).ToString();

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

        public ACC_SCREEN_DESIGN_PARAM_DETAILS setDefaultDesignParams()
        {
            ACC_SCREEN_DESIGN_PARAM_DETAILS designParams = new ACC_SCREEN_DESIGN_PARAM_DETAILS();
            designParams.AGREEMENT_REQ = "N";
            designParams.CON_REQ = "N";
            designParams.PIU_REQ = "N";
            designParams.PKG_REQ = "N";
            designParams.ROAD_REQ = "N";
            designParams.SUPPLIER_REQ = "N";
            designParams.YEAR_REQ = "N";

            return designParams;
        }
    }

    public interface IReceiptDAL
    {
        Array ReceiptList(ReceiptFilterModel objFilter, out long totalRecords);
        Array ReceiptMasterList(ReceiptFilterModel objFilter, out long totalRecords);
        Array ReceiptDetailsList(ReceiptFilterModel objFilter, out long totalRecords, out decimal totalAmount);
        String AddReceiptDetails(BillDetailsViewModel billDetailsViewModel);
        String EditReceiptDetails(BillDetailsViewModel billDetailsViewModel);
        BillDetailsViewModel GetReceiptDetailByTransNo(Int64 billId, Int16 TransNo);
        String DeleteReceiptDetails(Int64 billId, Int16 TransNo);
        String FinalizeReceipt(Int64 billId);
        String DeleteReceipt(Int64 billId);
        Decimal GetAmountAvailable(Int64 billId, Int16 transId);
        String ValidateAddReceiptDetails(BillDetailsViewModel billDetailsViewModel, String Op_mode);
        String ValidateEditReceiptMaster(BillMasterViewModel billMasterViewModel);
        Boolean IsDuplicateBillNo(Int16 month, Int16 year, Int32 adminNdCode, Int16 levelId, String fundType, String billType, String billNo);
        List<SelectListItem> GetUnSettledVouchers(Int16 month, Int16 year, Int64 pBillId);
        String GetImprestAmount(Int64 billId, Int16 TxnId);
    }
}