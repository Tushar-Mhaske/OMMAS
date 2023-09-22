using PMGSY.Models;
using PMGSY.Models.TransferEntryOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.Common;
using PMGSY.Models.Receipts;
using System.Data.Entity;
using System.Transactions;
using System.Data.Entity.Validation;
using PMGSY.Models.Common;
using PMGSY.Extensions;
using System.Web.Mvc;
using System.Data.Entity.Core;

namespace PMGSY.DAL.TransferEntryOrder
{
    public class TransferEntryOrderDAL : ITransferEntryOrderDAL
    {
        PMGSYEntities dbContext = null;
        CommonFunctions commonFuncObj = null;

        public Array TEOList(ReceiptFilterModel objFilter, out long totalRecords, bool isTransferofBalances = false)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<ListTEO> lstTEO = new List<ListTEO>();

                Dictionary<string, int> lstTransactionIDs = new Dictionary<string, int>();

                if (objFilter.FilterMode.Equals("view"))
                {
                    //lstBillMaster = dbContext.ACC_BILL_MASTER.Where(m => m.ADMIN_ND_CODE == objFilter.AdminNdCode).Where(m => m.LVL_ID == objFilter.LevelId).Where(m => m.FUND_TYPE == objFilter.FundType).Where(m => m.BILL_TYPE == objFilter.BillType).Where(m => m.BILL_MONTH == objFilter.Month).Where(m => m.BILL_YEAR == objFilter.Year).ToList<ACC_BILL_MASTER>();

                    lstTEO = (from m in dbContext.ACC_BILL_MASTER
                              where
                              m.ADMIN_ND_CODE == objFilter.AdminNdCode && m.LVL_ID == objFilter.LevelId
                              && m.FUND_TYPE == objFilter.FundType && m.BILL_TYPE == objFilter.BillType
                              && (objFilter.Month == 0 ? 1 : m.BILL_MONTH) == (objFilter.Month == 0 ? 1 : objFilter.Month)
                              && (objFilter.Year == 0 ? 1 : m.BILL_YEAR) == (objFilter.Year == 0 ? 1 : objFilter.Year)
                              // && objFilter.BillType == "J"
                              select new ListTEO
                              {
                                  BILL_ID = m.BILL_ID,
                                  BILL_NO = m.BILL_NO,
                                  BILL_DATE = m.BILL_DATE,
                                  GROSS_AMOUNT = m.GROSS_AMOUNT,
                                  CREDIT_AMOUNT = m.ACC_BILL_DETAILS.Any(t => t.BILL_ID == m.BILL_ID && t.CREDIT_DEBIT == "C") ? m.ACC_BILL_DETAILS.Where(t => t.CREDIT_DEBIT == "C").Sum(t => t.AMOUNT) : 0,
                                  DEBIT_AMOUNT = m.ACC_BILL_DETAILS.Any(t => t.BILL_ID == m.BILL_ID && t.CREDIT_DEBIT == "D") ? m.ACC_BILL_DETAILS.Where(t => t.CREDIT_DEBIT == "D").Sum(t => t.AMOUNT) : 0,
                                  TXN_DESC = m.ACC_MASTER_TXN.TXN_DESC,
                                  BILL_FINALIZED = m.BILL_FINALIZED,
                                  BILL_MONTH = m.BILL_MONTH,
                                  BILL_YEAR = m.BILL_YEAR,
                                  ACTION_REQUIRED = m.ACTION_REQUIRED,
                                  TXN_ID = m.TXN_ID,
                                  ADMIN_ND_CODE = m.ADMIN_ND_CODE  //Added By Abhishek kamble
                              }).ToList<ListTEO>();


                    //remove master entries of imprest payment

                    var lstTEOToRemove = (from m in dbContext.ACC_BILL_MASTER
                                          join b in dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS on m.BILL_ID equals b.S_BIll_ID
                                          where
                                          m.ADMIN_ND_CODE == objFilter.AdminNdCode && m.LVL_ID == objFilter.LevelId
                                          && m.FUND_TYPE == objFilter.FundType && m.BILL_TYPE == objFilter.BillType
                                          && (objFilter.Month == 0 ? 1 : m.BILL_MONTH) == (objFilter.Month == 0 ? 1 : objFilter.Month)
                                          && (objFilter.Year == 0 ? 1 : m.BILL_YEAR) == (objFilter.Year == 0 ? 1 : objFilter.Year)
                                          select new
                                          {
                                              m.BILL_ID

                                          });

                    //commented by koustubh nakate on 26/08/2013 for remove foreach loop
                    //foreach(var item in lstTEOToRemove)
                    //{
                    //    lstTEO.RemoveAll(x => x.BILL_ID==item.BILL_ID);
                    //}
                    //added by koustubh nakate on 26/08/2013 


                    lstTEO = (from TEO in lstTEO
                              where !lstTEOToRemove.Any(TEOR => TEOR.BILL_ID == TEO.BILL_ID)
                              select TEO).ToList();


                }
                else
                {
                    DateTime fromDate = DateTime.Now;
                    commonFuncObj = new CommonFunctions();
                    DateTime toDate = DateTime.Now;
                    if (objFilter.FromDate != String.Empty)
                    {
                        fromDate = commonFuncObj.GetStringToDateTime(objFilter.FromDate);
                    }
                    else
                    {
                        fromDate = dbContext.ACC_BILL_MASTER.Min(m => m.BILL_DATE);
                    }
                    if (objFilter.ToDate != String.Empty)
                    {
                        toDate = commonFuncObj.GetStringToDateTime(objFilter.ToDate);
                    }
                    else
                    {
                        toDate = dbContext.ACC_BILL_MASTER.Max(m => m.BILL_DATE);
                    }

                    lstTEO = (from m in dbContext.ACC_BILL_MASTER
                              where
                              m.ADMIN_ND_CODE == objFilter.AdminNdCode && m.LVL_ID == objFilter.LevelId
                              && m.FUND_TYPE == objFilter.FundType && m.BILL_TYPE == objFilter.BillType
                              && m.BILL_DATE >= fromDate
                              && m.BILL_DATE <= toDate
                              && (objFilter.TransId == 0 ? 1 : m.TXN_ID) == (objFilter.TransId == 0 ? 1 : objFilter.TransId)
                              select new ListTEO
                              {
                                  BILL_ID = m.BILL_ID,
                                  BILL_NO = m.BILL_NO,
                                  BILL_DATE = m.BILL_DATE,
                                  GROSS_AMOUNT = m.GROSS_AMOUNT,
                                  CREDIT_AMOUNT = m.ACC_BILL_DETAILS.Any(t => t.BILL_ID == m.BILL_ID && t.CREDIT_DEBIT == "C") ? m.ACC_BILL_DETAILS.Where(t => t.CREDIT_DEBIT == "C").Sum(t => t.AMOUNT) : 0,
                                  DEBIT_AMOUNT = m.ACC_BILL_DETAILS.Any(t => t.BILL_ID == m.BILL_ID && t.CREDIT_DEBIT == "D") ? m.ACC_BILL_DETAILS.Where(t => t.CREDIT_DEBIT == "D").Sum(t => t.AMOUNT) : 0,
                                  TXN_DESC = m.ACC_MASTER_TXN.TXN_DESC,
                                  BILL_FINALIZED = m.BILL_FINALIZED,
                                  BILL_MONTH = m.BILL_MONTH,
                                  BILL_YEAR = m.BILL_YEAR,
                                  ACTION_REQUIRED = m.ACTION_REQUIRED,
                                  TXN_ID = m.TXN_ID,
                                  ADMIN_ND_CODE = m.ADMIN_ND_CODE  //Added By Abhishek kamble 9-June-2014

                              }).ToList<ListTEO>();


                    //remove master entries of imprest payment


                    var lstTEOToRemove = (from m in dbContext.ACC_BILL_MASTER
                                          join b in dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS on m.BILL_ID equals b.S_BIll_ID
                                          where
                                           m.ADMIN_ND_CODE == objFilter.AdminNdCode && m.LVL_ID == objFilter.LevelId
                                            && m.FUND_TYPE == objFilter.FundType && m.BILL_TYPE == objFilter.BillType
                                            && m.BILL_DATE >= fromDate
                                            && m.BILL_DATE <= toDate
                                            && (objFilter.TransId == 0 ? 1 : m.TXN_ID) == (objFilter.TransId == 0 ? 1 : objFilter.TransId)
                                          select new
                                          {
                                              m.BILL_ID

                                          });



                    //commented by koustubh nakate on 26/08/2013 for remove foreach loop
                    //foreach (var item in lstTEOToRemove)
                    //{
                    //    lstTEO.RemoveAll(x => x.BILL_ID == item.BILL_ID);
                    //}
                    //added by koustubh nakate on 26/08/2013
                    lstTEO = (from TEO in lstTEO
                              where !lstTEOToRemove.Any(TEOR => TEOR.BILL_ID == TEO.BILL_ID)
                              select TEO).ToList();

                    List<ACC_BILL_MASTER> lstBillMaster = new List<ACC_BILL_MASTER>();
                    lstBillMaster = dbContext.ACC_BILL_MASTER.Where(m => m.ADMIN_ND_CODE == objFilter.AdminNdCode).Where(m => m.LVL_ID == objFilter.LevelId).Where(m => m.FUND_TYPE == objFilter.FundType).Where(m => m.BILL_TYPE == "J").Where(m => m.BILL_DATE >= fromDate).ToList<ACC_BILL_MASTER>();

                }

                //added by Koustubh Nakate on 26/08/2013 to get proper list as per transaction id

                if (objFilter.FundType.Equals("P"))
                {
                    lstTransactionIDs = new Dictionary<string, int>() { 
                                                                           {"TXN_ID1",163}, 
                            
                                                                           {"TXN_ID2",164}, 
                                                                           
                                                                           {"TXN_ID3",165}, 
                                                                           {"TXN_ID4",1187},
                                                                           {"TXN_ID5",1550} ,//Added By Abhishek kamble 7Apr2015 -Balance Transfer between the road heads of same piu                       
                                                                            {"TXN_ID6",1664} ,
                                                                            {"TXN_ID7",1665} 
                                                                        };
                }
                else if (objFilter.FundType.Equals("M"))
                {
                    // lstTransactionIDs = new Dictionary<string, int>() { 1192, 1193, 1194, 1195 };

                    lstTransactionIDs = new Dictionary<string, int>() { 
                                                                           {"TXN_ID1",1192}, 
                            
                                                                           { "TXN_ID2",1193}, 
                                                                           
                                                                           {"TXN_ID3",1194}, 
                                                                           {"TXN_ID4",1195} };
                }


                if (isTransferofBalances)
                {

                    lstTEO = (from TEO in lstTEO
                              where lstTransactionIDs.Any(ID => ID.Value == TEO.TXN_ID)
                              select TEO).ToList();


                }
                else
                {
                    lstTEO = (from TEO in lstTEO
                              where !lstTransactionIDs.Any(ID => ID.Value == TEO.TXN_ID)
                              select TEO).ToList();
                }


                totalRecords = lstTEO.Count();

                if (objFilter.sidx.Trim() != string.Empty)
                {
                    if (objFilter.sord.ToString() == "asc")
                    {
                        switch (objFilter.sidx)
                        {
                            case "TEONumber":
                                lstTEO = lstTEO.OrderBy(x => x.BILL_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ListTEO>();
                                break;
                            case "TEODate":
                                lstTEO = lstTEO.OrderBy(x => x.BILL_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ListTEO>();
                                break;
                            case "TransactionName":
                                lstTEO = lstTEO.OrderBy(x => x.TXN_DESC).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ListTEO>();
                                break;
                            case "GrossAmount":
                                lstTEO = lstTEO.OrderBy(x => x.GROSS_AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ListTEO>();
                                break;
                            //case "DetailsAmount":
                            //    lstTEO = lstTEO.OrderBy(x => x.CREDIT_AMOUNT && x.DEBIT_AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ListTEO>();
                            //    break;
                            default:
                                lstTEO = lstTEO.OrderBy(x => x.BILL_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ListTEO>();
                                break;
                        }

                    }
                    else
                    {
                        switch (objFilter.sidx)
                        {
                            case "TEONumber":
                                lstTEO = lstTEO.OrderByDescending(x => x.BILL_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ListTEO>();
                                break;
                            case "TEODate":
                                lstTEO = lstTEO.OrderByDescending(x => x.BILL_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ListTEO>();
                                break;
                            case "TransactionName":
                                lstTEO = lstTEO.OrderByDescending(x => x.TXN_DESC).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ListTEO>();
                                break;
                            case "GrossAmount":
                                lstTEO = lstTEO.OrderByDescending(x => x.GROSS_AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ListTEO>();
                                break;
                            //case "DetailsAmount":
                            //    lstTEO = lstTEO.OrderByDescending(x => x.DETAILS_AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ListTEO>();
                            //    break;
                            default:
                                lstTEO = lstTEO.OrderByDescending(x => x.BILL_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ListTEO>();
                                break;
                        }
                    }
                }
                else
                {
                    lstTEO = lstTEO.OrderByDescending(x => x.BILL_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ListTEO>();
                }

                return lstTEO.Select(item => new
                {

                    id = URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim() }),
                    cell = new[] {                         
                                    //PF- 161,162   AF-397,398,400,401  MF-844,845,846,847,848,849,850,852,853,854,855,856,857,858
                                    item.BILL_NO,
                                    Convert.ToDateTime(item.BILL_DATE).ToString("dd/MM/yyyy"),
                                    ((item.TXN_ID==161||item.TXN_ID==162||item.TXN_ID==397||item.TXN_ID==398||item.TXN_ID==400||item.TXN_ID==401|| item.TXN_ID==844||item.TXN_ID==845||item.TXN_ID==846||item.TXN_ID==847||item.TXN_ID==848||item.TXN_ID==849||item.TXN_ID==850||item.TXN_ID==852||item.TXN_ID==853||item.TXN_ID==854||item.TXN_ID==855||item.TXN_ID==856||item.TXN_ID==857||item.TXN_ID==858) && (dbContext.ACC_BILL_DETAILS.Where(m=>m.BILL_ID==item.BILL_ID && m.ADMIN_ND_CODE!=null).Any()))? item.TXN_DESC.Trim() +" ( "+ dbContext.ADMIN_DEPARTMENT.Where(a=>a.ADMIN_ND_CODE==dbContext.ACC_BILL_DETAILS.Where(m=>m.BILL_ID==item.BILL_ID && m.CREDIT_DEBIT=="C").Select(s=>s.ADMIN_ND_CODE).FirstOrDefault()).Select(c=>c.ADMIN_ND_NAME).FirstOrDefault() +" ) ":item.TXN_DESC.Trim(),
                                    item.GROSS_AMOUNT.ToString(),
                                    item.CREDIT_AMOUNT.ToString()+"Cr/ "+item.DEBIT_AMOUNT.ToString()+"Dr",
                                    (item.GROSS_AMOUNT == item.DEBIT_AMOUNT && item.GROSS_AMOUNT == item.CREDIT_AMOUNT && item.BILL_FINALIZED == "N") ? "<center><a href='#' class='ui-icon ui-icon-unlocked' onclick='LockTEO(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()})+'$'+item.TXN_ID.ToString()+" \");return false;'>Lock</a></center>" : 
                                    ((item.GROSS_AMOUNT == item.DEBIT_AMOUNT && item.GROSS_AMOUNT == item.CREDIT_AMOUNT && item.BILL_FINALIZED == "Y") ? "<center><a href='#' class='ui-icon ui-icon-search' onclick='ViewTEO(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()})+ "\");return false;'>View</a></center>" : 
                                    //"<center><div style='width:50px;height:10px; background-color:#ffffff; border:0.1em solid gray; text-align:left' class='ui-corner-all' title='Rs "+(item.GROSS_AMOUNT - item.CREDIT_AMOUNT)+" remaining'><div style='width:"+(((item.CREDIT_AMOUNT/item.GROSS_AMOUNT)*100)/4)+"px;height:100%; background-color:#4eb305' class='ui-corner-left' title='Rs "+(item.CREDIT_AMOUNT)+" entered'></div><div style='width:"+(((item.DEBIT_AMOUNT/item.GROSS_AMOUNT)*100)/4)+"px;height:100%; background-color:#ce1212' class='ui-corner-left' title='Rs "+(item.CREDIT_AMOUNT)+" entered'></div></div></center>"),
                                    "<center><div style='width:54px;height:10px' class='ui-corner-all'><div title='Rs."+(item.GROSS_AMOUNT-item.CREDIT_AMOUNT)+" Remaining' style='width:25px;height:100%; float:left; background-color:#ffffff; border:0.1em solid gray; text-align:left' class='ui-corner-left'><div title='Rs."+(item.CREDIT_AMOUNT)+" Credited' style='width:"+ (item.GROSS_AMOUNT==0? 0: (((item.CREDIT_AMOUNT/(item.GROSS_AMOUNT))*100)/4)) +"px;height:100%; background-color:#4eb305;'></div></div><div title='Rs."+(item.GROSS_AMOUNT-item.DEBIT_AMOUNT)+" Remaining' style='width:25px;height:100%; float:right; background-color:#ffffff; border:0.1em solid gray; text-align:right' class='ui-corner-right'><div title='Rs."+(item.DEBIT_AMOUNT)+" Debited' style='width:"+ (item.GROSS_AMOUNT==0?0:(((item.DEBIT_AMOUNT/(item.GROSS_AMOUNT))*100)/4))+"px;height:100%; background-color:#ce1212; float:right'></div></div></div></center>"),
                                    item.BILL_FINALIZED=="N"? "<center><a href='#' class='ui-icon ui-icon-pencil' onclick='EditTEO(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()})+ "\");return false;'>Edit</a></center>" : "",
                                    item.BILL_FINALIZED=="N"? "<center><a href='#' class='ui-icon ui-icon-trash' onclick='DeleteTEO(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()})+"\",\"" +item.CREDIT_AMOUNT+'$'+item.DEBIT_AMOUNT+"\");return false;'>Delete</a></center>" :String.Empty,                                                               
                                    item.ACTION_REQUIRED == "C" ? "<center><span title='Transaction details Incorrect, Needs Correction' style=color:#1C94C4; font-weight:bold' class='C'>"+item.ACTION_REQUIRED+"</span></center>" : item.ACTION_REQUIRED == "O" ? "<center><span title='Wrong Head Entry, Delete records and insert correct transactions' style=color:#b83400; font-weight:bold' class='O'>"+item.ACTION_REQUIRED+"</span></center>" : item.ACTION_REQUIRED == "M" ? "<center><span  title='Details not present, Unfinalize this record and insert details transactions' style=color:##014421; font-weight:bold' class='M'>"+item.ACTION_REQUIRED+"</span></center>" : "<center><span  title='Correct Transaction Entry' class='ui-icon ui-icon-check'>"+item.ACTION_REQUIRED+"</span></center>"   
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

        public Int64 AddTEOMaster(TeoMasterModel teoMasterModel)
        {
            try
            {
                ACC_BILL_MASTER acc_bill_master = new ACC_BILL_MASTER();
                acc_bill_master = CloneMasterModel(teoMasterModel);
                dbContext = new PMGSYEntities();
                acc_bill_master.BILL_ID = dbContext.ACC_BILL_MASTER.Any() ? dbContext.ACC_BILL_MASTER.Max(m => m.BILL_ID) + 1 : 1;
                dbContext.ACC_BILL_MASTER.Add(acc_bill_master);
                int fiscalYear = 0;
                if (teoMasterModel.BILL_MONTH <= 3)
                {
                    fiscalYear = (teoMasterModel.BILL_YEAR - 1);
                }
                else
                {
                    fiscalYear = teoMasterModel.BILL_YEAR;
                }

                ACC_VOUCHER_NUMBER_MASTER oldVoucherNumberModel = new ACC_VOUCHER_NUMBER_MASTER();
                oldVoucherNumberModel = dbContext.ACC_VOUCHER_NUMBER_MASTER.Where(x => x.ADMIN_ND_CODE == teoMasterModel.ADMIN_ND_CODE && x.FUND_TYPE == teoMasterModel.FUND_TYPE && x.BILL_TYPE == "J" && x.FISCAL_YEAR == fiscalYear).FirstOrDefault();
                ACC_VOUCHER_NUMBER_MASTER newMvoucherNumberModel = new ACC_VOUCHER_NUMBER_MASTER();

                newMvoucherNumberModel.ADMIN_ND_CODE = teoMasterModel.ADMIN_ND_CODE;
                newMvoucherNumberModel.FUND_TYPE = teoMasterModel.FUND_TYPE;
                newMvoucherNumberModel.BILL_TYPE ="J";
                newMvoucherNumberModel.FISCAL_YEAR = fiscalYear;
                newMvoucherNumberModel.SLNO = oldVoucherNumberModel.SLNO + 1;

                dbContext.Entry(oldVoucherNumberModel).CurrentValues.SetValues(newMvoucherNumberModel);

                dbContext.SaveChanges();
                return acc_bill_master.BILL_ID;
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
        /// function to check if imprest amount is valid 
        /// user in custom validation od imprest settlement
        /// </summary>
        /// <param name="pBillId"> payment / opening balnace bill id</param>
        /// <param name="ImprestAmount"> imprest amount entered</param>
        /// <returns></returns>

        public bool IsImprestAmountValid(Int64 pBillId, Decimal ImprestAmount)
        {
            try
            {
                ACC_BILL_MASTER acc_bill_master = new ACC_BILL_MASTER();
                dbContext = new PMGSYEntities();
                List<ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS> lstBillIds = new List<ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS>();
                Decimal amountSettled = 0;
                Decimal grossAmount = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == pBillId).Select(m => m.GROSS_AMOUNT).FirstOrDefault();
                if (dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Where(p => p.P_BILL_ID == pBillId).Any())
                {
                    lstBillIds = dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Where(p => p.P_BILL_ID == pBillId).ToList<ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS>();
                }

                if (lstBillIds.Count > 0)
                {
                    foreach (ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS item in lstBillIds)
                    {
                        amountSettled += dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == item.S_BIll_ID && m.TXN_NO == item.P_TXN_ID).Select(m => m.AMOUNT).FirstOrDefault();
                    }
                    //amountSettled = (from c in dbContext.ACC_BILL_MASTER
                    //                 where
                    //                 lstBillIds.Contains(c.BILL_ID)
                    //                 select c.GROSS_AMOUNT).Sum();

                }
                if ((grossAmount - amountSettled) < ImprestAmount)
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
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// this action does the same functionality as above but it also take care of multiple imprest settlement entries
        /// </summary>
        /// <param name="pBillId"></param>
        /// <param name="ImprestAmount"></param>
        /// <returns></returns>
        //public bool IsImprestAmountValidMultiple(Int64 pBillId, Int16 TxnNo,Decimal ImprestAmount)
        //{
        //    try
        //    {
        //        ACC_BILL_MASTER acc_bill_master = new ACC_BILL_MASTER();
        //        dbContext = new PMGSYEntities();
        //        List<ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS> lstBillIds = new List<ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS>();
        //        Decimal amountSettled = 0;
        //        Decimal grossAmount = 0;
        //        if (dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == pBillId).Any(m => m.BILL_TYPE == "O"))
        //        {
        //            grossAmount = dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == pBillId && m.TXN_NO == TxnNo).Select(m => m.AMOUNT).FirstOrDefault();
        //        }
        //        else
        //        {
        //            grossAmount = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == pBillId).Select(m => m.GROSS_AMOUNT).FirstOrDefault();
        //        }
        //        if (dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Where(p => p.P_BILL_ID == pBillId).Any())
        //        {
        //            lstBillIds = dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Where(p => p.P_BILL_ID == pBillId).ToList<ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS>();
        //        }

        //        if (lstBillIds.Count > 0)
        //        {
        //            foreach (ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS item in lstBillIds)
        //            {
        //                // if settlement is through teo get total amount
        //                if (dbContext.ACC_BILL_MASTER.Where(c => c.BILL_ID == item.S_BIll_ID).Select(x => x.BILL_TYPE).First() == "J")
        //                {
        //                    amountSettled += (from c in dbContext.ACC_BILL_MASTER where item.S_BIll_ID == c.BILL_ID select c.GROSS_AMOUNT).FirstOrDefault();

        //                }
        //                else
        //                {

        //                    amountSettled += dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == item.S_BIll_ID && m.TXN_NO == item.P_TXN_ID).Select(m => m.AMOUNT).FirstOrDefault();
        //                }
        //            }
        //            //amountSettled = (from c in dbContext.ACC_BILL_MASTER
        //            //                 where
        //            //                 lstBillIds.Contains(c.BILL_ID)
        //            //                 select c.GROSS_AMOUNT).Sum();

        //        }
        //        if ((grossAmount - amountSettled) < ImprestAmount)
        //        {
        //            return false;
        //        }
        //        else
        //        {
        //            return true;
        //        }
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

        public bool IsImprestAmountValidMultiple(Int64 pBillId, Int16 TxnNo, Decimal ImprestAmount)
        {
            try
            {
                ACC_BILL_MASTER acc_bill_master = new ACC_BILL_MASTER();
                dbContext = new PMGSYEntities();
                List<PMGSY.Models.TransferEntryOrder.IsValidImprestAmount.ListImprest1> lstBillIds = new List<PMGSY.Models.TransferEntryOrder.IsValidImprestAmount.ListImprest1>();
                //var lstBillIds = (IEnumerable<dynamic>)null;
                Decimal amountSettled = 0;
                Decimal grossAmount = 0;
                if (dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == pBillId).Any(m => m.BILL_TYPE == "O"))
                {
                    grossAmount = dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == pBillId && m.TXN_NO == TxnNo).Select(m => m.AMOUNT).FirstOrDefault();
                }
                else
                {
                    grossAmount = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == pBillId).Select(m => m.GROSS_AMOUNT).FirstOrDefault();
                }

                if (dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == pBillId).Any(m => m.BILL_TYPE == "O"))
                {

                    if (dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Where(p => p.P_BILL_ID == pBillId).Any())
                    {

                        lstBillIds = (from item in dbContext.ACC_BILL_MASTER
                                      join sd in dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS
                                      on item.BILL_ID equals sd.P_BILL_ID
                                      where item.BILL_FINALIZED == "Y" &&
                                      sd.P_TXN_ID == TxnNo &&
                                      item.BILL_ID == pBillId
                                      select new PMGSY.Models.TransferEntryOrder.IsValidImprestAmount.ListImprest1
                                      {
                                          MAP_ID = sd.MAP_ID,
                                          P_BILL_ID = sd.P_BILL_ID,
                                          P_TXN_ID = sd.P_TXN_ID,
                                          S_BIll_ID = sd.S_BIll_ID,
                                          S_TXN_ID = sd.S_TXN_ID
                                      }).ToList<PMGSY.Models.TransferEntryOrder.IsValidImprestAmount.ListImprest1>();

                        //lstBillIds = dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Join(
                        //    dbContext.ACC_BILL_MASTER, sd => sd.S_BIll_ID, bm => bm.BILL_ID, (bm, sd) => new { Settlement = sd/*, Bill = bm */}).Where(p => p.Settlement.BILL_FINALIZED  == "Y" && p.Settlement.TXN_ID == TxnNo).ToList();
                    }
                }
                else
                {
                    if (dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Where(p => p.P_BILL_ID == pBillId).Any())
                    {
                        //lstBillIds = dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Where(p => p.P_BILL_ID == pBillId).ToList<ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS>();

                        lstBillIds = (from item in dbContext.ACC_BILL_MASTER
                                      join sd in dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS
                                      on item.BILL_ID equals sd.P_BILL_ID
                                      where item.BILL_FINALIZED == "Y" &&
                                      sd.P_TXN_ID == TxnNo &&
                                          item.BILL_ID == pBillId
                                      select new PMGSY.Models.TransferEntryOrder.IsValidImprestAmount.ListImprest1
                                      {
                                          MAP_ID = sd.MAP_ID,
                                          P_BILL_ID = sd.P_BILL_ID,
                                          P_TXN_ID = sd.P_TXN_ID,
                                          S_BIll_ID = sd.S_BIll_ID,
                                          S_TXN_ID = sd.S_TXN_ID
                                      }).ToList<PMGSY.Models.TransferEntryOrder.IsValidImprestAmount.ListImprest1>();
                    }
                }




                if (lstBillIds.Count() > 0)
                {
                    foreach (PMGSY.Models.TransferEntryOrder.IsValidImprestAmount.ListImprest1 item in lstBillIds.ToList())
                    {
                        // if settlement is through teo get total amount
                        if (dbContext.ACC_BILL_MASTER.Where(c => c.BILL_ID == item.S_BIll_ID).Select(x => x.BILL_TYPE).First() == "J")
                        {
                            amountSettled += (from c in dbContext.ACC_BILL_MASTER where item.S_BIll_ID == c.BILL_ID select c.GROSS_AMOUNT).FirstOrDefault();

                        }
                        else
                        {

                            amountSettled += dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == item.S_BIll_ID && m.TXN_NO == item.S_TXN_ID).Select(m => m.AMOUNT).FirstOrDefault();
                        }
                    }
                    //amountSettled = (from c in dbContext.ACC_BILL_MASTER
                    //                 where
                    //                 lstBillIds.Contains(c.BILL_ID)
                    //                 select c.GROSS_AMOUNT).Sum();

                }
                if ((grossAmount - amountSettled) < ImprestAmount)
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
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        /// <summary>
        /// function for edit operation
        /// </summary>
        /// <param name="pBillId"></param>
        /// <param name="ImprestAmount"></param>
        /// <returns></returns>
        public bool IsImprestAmountValidMultipleForAddEdit(Int64 pBillId, Decimal ImprestAmount, long BillIdToEdit, string AddOrEdit)
        {
            try
            {
                ACC_BILL_MASTER acc_bill_master = new ACC_BILL_MASTER();
                dbContext = new PMGSYEntities();
                List<ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS> lstBillIds = new List<ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS>();
                Decimal amountSettled = 0;
                Decimal grossAmount = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == pBillId).Select(m => m.GROSS_AMOUNT).FirstOrDefault();
                if (dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Where(p => p.P_BILL_ID == pBillId).Any())
                {
                    lstBillIds = dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Where(p => p.P_BILL_ID == pBillId).ToList<ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS>();
                }

                if (lstBillIds.Count > 0)
                {
                    foreach (ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS item in lstBillIds)
                    {

                        if (AddOrEdit == "E" && item.S_BIll_ID != BillIdToEdit)
                        {
                            // if settlement is through teo get total amount
                            if (dbContext.ACC_BILL_MASTER.Where(c => c.BILL_ID == item.S_BIll_ID).Select(x => x.BILL_TYPE).First() == "J")
                            {
                                amountSettled += (from c in dbContext.ACC_BILL_MASTER where item.S_BIll_ID == c.BILL_ID select c.GROSS_AMOUNT).FirstOrDefault();

                            }
                            else
                            {

                                amountSettled += dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == item.S_BIll_ID && m.TXN_NO == item.P_TXN_ID).Select(m => m.AMOUNT).FirstOrDefault();
                            }
                        }
                    }
                }
                if ((grossAmount - amountSettled) < ImprestAmount)
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
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }



        /// <summary>
        /// DAL function to add imprest master details
        /// </summary>
        /// <param name="teoMasterModel"></param>
        /// <returns></returns>
        public Int64 AddImprestMaster(TeoMasterModel teoMasterModel)
        {
            try
            {


                ACC_BILL_MASTER acc_bill_master = new ACC_BILL_MASTER();
                dbContext = new PMGSYEntities();
                //teoMasterModel.GROSS_AMOUNT = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == teoMasterModel.PBILL_ID).Select(m => m.GROSS_AMOUNT).FirstOrDefault();
                teoMasterModel.SUB_TXN_ID = dbContext.ACC_MASTER_TXN.Where(m => m.BILL_TYPE == "I" && m.OP_LVL_ID == teoMasterModel.LVL_ID && m.FUND_TYPE == PMGSYSession.Current.FundType).Select(m => m.TXN_ID).FirstOrDefault();
                acc_bill_master = CloneMasterModel(teoMasterModel);
                dbContext = new PMGSYEntities();
                acc_bill_master.BILL_ID = dbContext.ACC_BILL_MASTER.Any() ? dbContext.ACC_BILL_MASTER.Max(m => m.BILL_ID) + 1 : 1;
                dbContext.ACC_BILL_MASTER.Add(acc_bill_master);
                dbContext.SaveChanges();
                ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS map_imprest_settlement = new ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS();
                map_imprest_settlement.P_BILL_ID = teoMasterModel.PBILL_ID;
                map_imprest_settlement.S_BIll_ID = acc_bill_master.BILL_ID;
                map_imprest_settlement.P_TXN_ID = teoMasterModel.TXN_NO;
                if (dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Any())
                {
                    map_imprest_settlement.MAP_ID = dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Max(m => m.MAP_ID) + 1;
                }
                else
                {
                    map_imprest_settlement.MAP_ID = 1;
                }
                map_imprest_settlement.S_TXN_ID = null;

                //Added By Abhishek Kamble 29-nov-2013
                map_imprest_settlement.USERID = PMGSYSession.Current.UserId;
                map_imprest_settlement.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Add(map_imprest_settlement);

                dbContext.SaveChanges();


                int fiscalYear = 0;
                if (teoMasterModel.BILL_MONTH <= 3)
                {
                    fiscalYear = (teoMasterModel.BILL_YEAR - 1);
                }
                else
                {
                    fiscalYear = teoMasterModel.BILL_YEAR;
                }

               // ACC_VOUCHER_NUMBER_MASTER oldVoucherNumberModel = new ACC_VOUCHER_NUMBER_MASTER();
               // oldVoucherNumberModel = dbContext.ACC_VOUCHER_NUMBER_MASTER.Where(x => x.ADMIN_ND_CODE == teoMasterModel.ADMIN_ND_CODE && x.FUND_TYPE == teoMasterModel.FUND_TYPE && x.BILL_TYPE == "J" && x.FISCAL_YEAR == fiscalYear).FirstOrDefault();
                //ACC_VOUCHER_NUMBER_MASTER newMvoucherNumberModel = new ACC_VOUCHER_NUMBER_MASTER();

                //newMvoucherNumberModel.ADMIN_ND_CODE = teoMasterModel.ADMIN_ND_CODE;
                //newMvoucherNumberModel.FUND_TYPE = teoMasterModel.FUND_TYPE;
                //newMvoucherNumberModel.BILL_TYPE = "J";
                //newMvoucherNumberModel.FISCAL_YEAR = fiscalYear;
                //newMvoucherNumberModel.SLNO = oldVoucherNumberModel.SLNO + 1;

                //dbContext.Entry(oldVoucherNumberModel).CurrentValues.SetValues(newMvoucherNumberModel);

                ACC_VOUCHER_NUMBER_MASTER oldVoucherNumberModel = dbContext.ACC_VOUCHER_NUMBER_MASTER.First(x => x.ADMIN_ND_CODE == teoMasterModel.ADMIN_ND_CODE && x.FUND_TYPE == teoMasterModel.FUND_TYPE && x.BILL_TYPE == "J" && x.FISCAL_YEAR == fiscalYear);

                oldVoucherNumberModel.ADMIN_ND_CODE = teoMasterModel.ADMIN_ND_CODE;
                oldVoucherNumberModel.FUND_TYPE = teoMasterModel.FUND_TYPE;
                oldVoucherNumberModel.BILL_TYPE = "J";
                oldVoucherNumberModel.FISCAL_YEAR = fiscalYear;
                oldVoucherNumberModel.SLNO = oldVoucherNumberModel.SLNO + 1;


                dbContext.SaveChanges();

                
              //  dbContext.SaveChanges();
                return acc_bill_master.BILL_ID;
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
        /// check for model error while making teo cedit debit entry
        /// </summary>
        /// <param name="teoDetailsModel"></param>
        /// <param name="CreditDebit"></param>
        /// <returns></returns>
        public List<ModelErrorList> GetAddTEODetailsModelErrors(TeoDetailsModel teoDetailsModel, String CreditDebit)
        {
            try
            {
                dbContext = new PMGSYEntities();
                commonFuncObj = new CommonFunctions();
                List<ModelErrorList> lstModelError = new List<ModelErrorList>();
                ACC_TEO_SCREEN_DESIGN_PARAM_DETAILS designParams = new ACC_TEO_SCREEN_DESIGN_PARAM_DETAILS();
                Int16 transId = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == teoDetailsModel.BILL_ID).Select(m => m.TXN_ID).FirstOrDefault();
                designParams = commonFuncObj.getTEODesignParamDetails(0, transId);
                if (designParams == null)
                {
                    designParams = setDefualtDesignTransParam();
                }
                ACC_TEO_SCREEN_DESIGN_HEADWISE_DETAILS headParams = new ACC_TEO_SCREEN_DESIGN_HEADWISE_DETAILS();
                headParams = dbContext.ACC_TEO_SCREEN_DESIGN_HEADWISE_DETAILS.Where(m => m.HEAD_ID == teoDetailsModel.HEAD_ID && m.TXN_ID == transId).FirstOrDefault();
                if (headParams == null)
                {
                    headParams = setDefualtDesignHeadParam();
                }
                ACC_TEO_SCREEN_TXN_VALIDATIONS validationParams = new ACC_TEO_SCREEN_TXN_VALIDATIONS();
                validationParams = dbContext.ACC_TEO_SCREEN_TXN_VALIDATIONS.Where(m => m.TXN_ID == transId).FirstOrDefault();
                if (validationParams == null)
                {
                    validationParams = setDefaultValidationParam();
                }
                Int16 masterTransId = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == teoDetailsModel.BILL_ID).Select(m => m.TXN_ID).FirstOrDefault();
                ACC_BILL_DETAILS acc_bill_details = new ACC_BILL_DETAILS();
                if (CreditDebit == "C")
                {
                    acc_bill_details = dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == teoDetailsModel.BILL_ID && m.CREDIT_DEBIT == "D").FirstOrDefault();
                }
                else
                {
                    acc_bill_details = dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == teoDetailsModel.BILL_ID && m.CREDIT_DEBIT == "C").FirstOrDefault();
                }

                if (acc_bill_details != null)
                {
                    if (masterTransId == 164 || masterTransId == 165 || masterTransId == 1664 || masterTransId == 1665)
                    {
                        if (acc_bill_details.MAST_DISTRICT_CODE != null && teoDetailsModel.MAST_DISTRICT_CODE == acc_bill_details.MAST_DISTRICT_CODE)
                        {
                            ModelErrorList objModel = new ModelErrorList();
                            objModel.Key = "MAST_DISTRICT_CODE";
                            objModel.Value = "Different District Required. Do not select " + dbContext.MASTER_DISTRICT.Where(m => m.MAST_DISTRICT_CODE == teoDetailsModel.MAST_DISTRICT_CODE).Select(m => m.MAST_DISTRICT_NAME).FirstOrDefault() + " District";
                            lstModelError.Add(objModel);
                        }
                    }
                }

                if (designParams.DISTRICT_REQ == "Y" && teoDetailsModel.MAST_DISTRICT_CODE == 0)
                {
                    ModelErrorList objModel = new ModelErrorList();
                    objModel.Key = "MAST_DISTRICT_CODE";
                    objModel.Value = "District Required";
                    lstModelError.Add(objModel);
                }
                else if (validationParams.IS_DISTRICT_REPEAT == "Y" && acc_bill_details != null)
                {


                    if (acc_bill_details.MAST_DISTRICT_CODE != null && teoDetailsModel.MAST_DISTRICT_CODE != acc_bill_details.MAST_DISTRICT_CODE)
                    {
                        ModelErrorList objModel = new ModelErrorList();
                        objModel.Key = "MAST_DISTRICT_CODE";
                        objModel.Value = "Select " + dbContext.MASTER_DISTRICT.Where(m => m.MAST_DISTRICT_CODE == acc_bill_details.MAST_DISTRICT_CODE).Select(m => m.MAST_DISTRICT_NAME).FirstOrDefault() + " District";
                        lstModelError.Add(objModel);
                    }
                }


                //if district should not repeat 
                if (validationParams.IS_DISTRICT_REPEAT == "N" && acc_bill_details != null)
                {

                    if (acc_bill_details.MAST_DISTRICT_CODE != null && teoDetailsModel.MAST_DISTRICT_CODE == acc_bill_details.MAST_DISTRICT_CODE)
                    {
                        ModelErrorList objModel = new ModelErrorList();
                        objModel.Key = "MAST_DISTRICT_CODE";
                        objModel.Value = "Dont Select " + dbContext.MASTER_DISTRICT.Where(m => m.MAST_DISTRICT_CODE == acc_bill_details.MAST_DISTRICT_CODE).Select(m => m.MAST_DISTRICT_NAME).FirstOrDefault() + " District as its Balance Transfer between two DPIUs of different district";
                        lstModelError.Add(objModel);
                    }
                }

                //dpiu should not repeat
                if (validationParams.IS_DPIU_REPEAT == "N" && acc_bill_details != null)
                {
                    if (acc_bill_details.ADMIN_ND_CODE != null && teoDetailsModel.ADMIN_ND_CODE == acc_bill_details.ADMIN_ND_CODE)
                    {
                        ModelErrorList objModel = new ModelErrorList();
                        objModel.Key = "ADMIN_ND_CODE";
                        objModel.Value = "Dont Select " + dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == acc_bill_details.ADMIN_ND_CODE).Select(m => m.ADMIN_ND_NAME).FirstOrDefault() + "as its  Balance Transfer between two DPIUs of different district";
                        lstModelError.Add(objModel);
                    }
                }


                //diffrent dpiu is not allowed
                if (acc_bill_details != null)
                {
                    if (masterTransId == 163 || masterTransId == 1193)
                    {
                        if (acc_bill_details.ADMIN_ND_CODE != null && teoDetailsModel.ADMIN_ND_CODE == acc_bill_details.ADMIN_ND_CODE)
                        {
                            ModelErrorList objModel = new ModelErrorList();
                            objModel.Key = "ADMIN_ND_CODE";
                            objModel.Value = "Different PIU Required. Do not select " + dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == teoDetailsModel.ADMIN_ND_CODE).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
                            lstModelError.Add(objModel);
                        }
                    }
                }

                if (designParams.DPIU_REQ == "Y" && teoDetailsModel.ADMIN_ND_CODE == 0)
                {
                    ModelErrorList objModel = new ModelErrorList();
                    objModel.Key = "ADMIN_ND_CODE";
                    objModel.Value = "PIU Name Required";
                    lstModelError.Add(objModel);
                }
                else if (validationParams.IS_DPIU_REPEAT == "Y" && acc_bill_details != null)
                {
                    if (acc_bill_details.ADMIN_ND_CODE != null && teoDetailsModel.ADMIN_ND_CODE != acc_bill_details.ADMIN_ND_CODE)
                    {
                        ModelErrorList objModel = new ModelErrorList();
                        objModel.Key = "ADMIN_ND_CODE";
                        objModel.Value = "Select " + dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == acc_bill_details.ADMIN_ND_CODE).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
                        lstModelError.Add(objModel);
                    }
                }

                //change by amol 
                if ((teoDetailsModel.MAST_CON_ID == 0 || teoDetailsModel.MAST_CON_ID == null) && teoDetailsModel.MAST_CON_ID_TRANS != 0)
                {
                    teoDetailsModel.MAST_CON_ID = teoDetailsModel.MAST_CON_ID_TRANS;
                }

                if (designParams.CON_REQ == "Y" && teoDetailsModel.MAST_CON_ID == 0 && teoDetailsModel.MAST_CON_ID_TRANS == 0)
                {
                    ModelErrorList objModel = new ModelErrorList();
                    objModel.Key = "MAST_CON_ID";
                    objModel.Value = "Contractor Name Required";
                    lstModelError.Add(objModel);
                }
                else if (validationParams.IS_CON_REPEAT == "Y" && acc_bill_details != null)
                {
                    if (acc_bill_details.MAST_CON_ID != null && teoDetailsModel.MAST_CON_ID != acc_bill_details.MAST_CON_ID)
                    {
                        ModelErrorList objModel = new ModelErrorList();
                        objModel.Key = "MAST_CON_ID";
                        objModel.Value = "Select " + dbContext.MASTER_CONTRACTOR.Where(m => m.MAST_CON_ID == acc_bill_details.MAST_CON_ID).Select(m => m.MAST_CON_COMPANY_NAME).FirstOrDefault();
                        lstModelError.Add(objModel);
                    }
                }

                if (designParams.SUPP_REQ == "Y" && teoDetailsModel.MAST_CON_ID == 0)
                {
                    ModelErrorList objModel = new ModelErrorList();
                    objModel.Key = "MAST_CON_ID";
                    objModel.Value = "Supplier Name Required";
                    lstModelError.Add(objModel);
                }
                else if (validationParams.IS_SUP_REPEAT == "Y" && acc_bill_details != null)
                {
                    if (acc_bill_details.MAST_CON_ID != null && teoDetailsModel.MAST_CON_ID != acc_bill_details.MAST_CON_ID)
                    {
                        ModelErrorList objModel = new ModelErrorList();
                        objModel.Key = "MAST_CON_ID";
                        objModel.Value = "Select " + dbContext.MASTER_CONTRACTOR.Where(m => m.MAST_CON_ID == acc_bill_details.MAST_CON_ID).Select(m => m.MAST_CON_COMPANY_NAME).FirstOrDefault();
                        lstModelError.Add(objModel);
                    }
                }

                if (designParams.AFREEMENT_REQ == "Y" && (teoDetailsModel.IMS_AGREEMENT_CODE == 0 || teoDetailsModel.IMS_AGREEMENT_CODE == null))
                {
                    ModelErrorList objModel = new ModelErrorList();
                    objModel.Key = "IMS_AGREEMENT_CODE";
                    objModel.Value = "Agreement Name Required";
                    lstModelError.Add(objModel);
                }
                else if (validationParams.IS_AGREEMENT_REPEAT == "Y" && acc_bill_details != null)
                {
                    if (acc_bill_details.IMS_AGREEMENT_CODE != null && (teoDetailsModel.IMS_AGREEMENT_CODE != acc_bill_details.IMS_AGREEMENT_CODE))
                    {
                        ModelErrorList objModel = new ModelErrorList();
                        objModel.Key = "IMS_AGREEMENT_CODE";
                        objModel.Value = "Select " + dbContext.TEND_AGREEMENT_MASTER.Where(m => m.TEND_AGREEMENT_CODE == acc_bill_details.IMS_AGREEMENT_CODE).Select(m => m.TEND_AGREEMENT_NUMBER).FirstOrDefault();
                        lstModelError.Add(objModel);
                    }
                }
                else if (validationParams.IS_AGREEMENT_REPEAT == "N" && acc_bill_details != null)
                {
                    if (acc_bill_details.IMS_AGREEMENT_CODE != null && (teoDetailsModel.IMS_AGREEMENT_CODE == acc_bill_details.IMS_AGREEMENT_CODE))
                    {
                        ModelErrorList objModel = new ModelErrorList();
                        objModel.Key = "IMS_AGREEMENT_CODE";
                        objModel.Value = "Please select different Agreement.";
                        lstModelError.Add(objModel);
                    }
                }


                if (designParams.SAN_REQ == "Y" && teoDetailsModel.SANC_YEAR == 0)
                {
                    ModelErrorList objModel = new ModelErrorList();
                    objModel.Key = "SANC_YEAR";
                    objModel.Value = "Sanction Year Required";
                    lstModelError.Add(objModel);
                }




                if (designParams.ROAD_REQ == "Y" && (teoDetailsModel.IMS_PR_ROAD_CODE == 0 || teoDetailsModel.IMS_PR_ROAD_CODE == null))
                {
                    ModelErrorList objModel = new ModelErrorList();
                    objModel.Key = "IMS_PR_ROAD_CODE";
                    objModel.Value = "Road Name Required";
                    lstModelError.Add(objModel);
                }
                else if (validationParams.IS_ROAD_REPEAT == "Y" && acc_bill_details != null)
                {
                    if (acc_bill_details.IMS_PR_ROAD_CODE != null && (teoDetailsModel.IMS_PR_ROAD_CODE != acc_bill_details.IMS_PR_ROAD_CODE))
                    {
                        ModelErrorList objModel = new ModelErrorList();
                        objModel.Key = "IMS_PR_ROAD_CODE";
                        objModel.Value = "Select " + dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == acc_bill_details.IMS_PR_ROAD_CODE).Select(m => m.IMS_ROAD_NAME).FirstOrDefault();
                        lstModelError.Add(objModel);
                    }
                }

                if (designParams.PKG_REQ == "Y" && (teoDetailsModel.IMS_PACKAGE_ID == "0" || teoDetailsModel.IMS_PACKAGE_ID == null))
                {
                    ModelErrorList objModel = new ModelErrorList();
                    objModel.Key = "IMS_PACKAGE_ID";
                    objModel.Value = "Package Required";
                    lstModelError.Add(objModel);
                }

                //by amol jadhav
                if (designParams.MTXN_REQ != "Y")
                {
                    if (dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == teoDetailsModel.BILL_ID).Select(m => m.GROSS_AMOUNT).First() != teoDetailsModel.AMOUNT)
                    {
                        ModelErrorList objModel = new ModelErrorList();
                        objModel.Key = "AMOUNT";
                        objModel.Value = "Details Amount should be equal to master amount as multiple transaction is not allowed";
                        lstModelError.Add(objModel);
                        return lstModelError;
                    }

                }

                #region  amount validation e by amol jadhav
                //validation of amount the total credit/debit amount should be equal to or less than master amount
                //get the master amount 
                decimal masterAmount = 0;
                decimal detailsTotalAmount = 0;
                decimal prevAmount = 0; //amount being sactioned
                masterAmount = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == teoDetailsModel.BILL_ID).Select(m => m.GROSS_AMOUNT).First();

                //get amount already entered
                if (dbContext.ACC_BILL_DETAILS.Any(m => m.BILL_ID == teoDetailsModel.BILL_ID && m.CREDIT_DEBIT == CreditDebit))
                {
                    detailsTotalAmount = dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == teoDetailsModel.BILL_ID && m.CREDIT_DEBIT == CreditDebit).Select(m => m.AMOUNT).Sum();
                }
                //check if current entry is already exist (thats means its edit operation)

                if (teoDetailsModel.TXN_NO != 0)
                {
                    prevAmount = dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == teoDetailsModel.BILL_ID && m.TXN_NO == teoDetailsModel.TXN_NO).Select(m => m.AMOUNT).First();
                }

                //check for valid amount
                if (masterAmount < (detailsTotalAmount - prevAmount + teoDetailsModel.AMOUNT))
                {
                    ModelErrorList objModel = new ModelErrorList();
                    objModel.Key = "AMOUNT";
                    objModel.Value = "total Details Amount should be less than equal to master amount";
                    lstModelError.Add(objModel);
                    return lstModelError;
                }


                #endregion amount Validation

                if (teoDetailsModel.HEAD_ID == 0)
                {
                    ModelErrorList objModel = new ModelErrorList();
                    objModel.Key = "HEAD_ID";
                    objModel.Value = "Account Head Required";
                    lstModelError.Add(objModel);
                    return lstModelError;
                }
                else
                {
                    if (CreditDebit == "C" && acc_bill_details != null && validationParams.C_CREDIT_HEAD != 0 && validationParams.C_CREDIT_HEAD != null)
                    {
                        if (acc_bill_details.HEAD_ID != null && (teoDetailsModel.HEAD_ID != validationParams.C_CREDIT_HEAD))
                        {
                            ModelErrorList objModel = new ModelErrorList();
                            objModel.Key = "HEAD_ID";
                            objModel.Value = "Select " + dbContext.ACC_MASTER_HEAD.Where(m => m.HEAD_ID == validationParams.C_CREDIT_HEAD).Select(m => m.HEAD_NAME).FirstOrDefault();
                            lstModelError.Add(objModel);
                        }
                    }

                    if (CreditDebit == "D" && acc_bill_details != null && validationParams.C_DEBIT_HEAD != 0 && validationParams.C_DEBIT_HEAD != null)
                    {
                        if (acc_bill_details.HEAD_ID != null && (teoDetailsModel.HEAD_ID != validationParams.C_DEBIT_HEAD))
                        {
                            ModelErrorList objModel = new ModelErrorList();
                            objModel.Key = "HEAD_ID";
                            objModel.Value = "Select " + dbContext.ACC_MASTER_HEAD.Where(m => m.HEAD_ID == validationParams.C_DEBIT_HEAD).Select(m => m.HEAD_NAME).FirstOrDefault();
                            lstModelError.Add(objModel);
                        }
                    }


                    if (acc_bill_details != null)
                    {
                        if (validationParams.IS_HEAD_REPEAT == "Y" && (teoDetailsModel.HEAD_ID != acc_bill_details.HEAD_ID))
                        {
                            ModelErrorList objModel = new ModelErrorList();
                            objModel.Key = "HEAD_ID";
                            objModel.Value = "Select " + dbContext.ACC_MASTER_HEAD.Where(m => m.HEAD_ID == acc_bill_details.HEAD_ID).Select(m => m.HEAD_NAME).FirstOrDefault();
                            lstModelError.Add(objModel);
                        }
                    }

                    if (headParams == null)
                    {
                        return lstModelError;
                    }
                    else
                    {
                        if (headParams.CON_REQ == "Y")
                        {
                            if (teoDetailsModel.MAST_CON_ID == 0)
                            {
                                ModelErrorList objModel = new ModelErrorList();
                                objModel.Key = "MAST_CON_ID";
                                objModel.Value = "Contractor Name Required";
                                lstModelError.Add(objModel);
                            }
                            else if (acc_bill_details != null)
                            {
                                //if (teoDetailsModel.MAST_CON_ID != acc_bill_details.MAST_CON_ID)
                                //{
                                //    ModelErrorList objModel = new ModelErrorList();
                                //    objModel.Key = "MAST_CON_ID";
                                //    objModel.Value = "Select " + dbContext.MASTER_CONTRACTOR.Where(m => m.MAST_CON_ID == acc_bill_details.MAST_CON_ID).Select(m => m.MAST_CON_COMPANY_NAME).FirstOrDefault());
                                //    lstModelError.Add(objModel);
                                //}
                                if (transId != 3100)
                                {
                                    if (teoDetailsModel.MAST_CON_ID != acc_bill_details.MAST_CON_ID)
                                    {
                                        ModelErrorList objModel = new ModelErrorList();
                                        objModel.Key = "MAST_CON_ID";
                                        objModel.Value = "Select " + dbContext.MASTER_CONTRACTOR.Where(m => m.MAST_CON_ID == acc_bill_details.MAST_CON_ID).Select(m => m.MAST_CON_COMPANY_NAME).FirstOrDefault();
                                        lstModelError.Add(objModel);
                                    }
                                }
                                
                            }
                        }

                        if (headParams.AGREEMENT_REQ == "Y")
                        {
                            if (teoDetailsModel.IMS_AGREEMENT_CODE == 0 || teoDetailsModel.IMS_AGREEMENT_CODE == null)
                            {
                                //Below Code commented on 13-10-2022
                                //ModelErrorList objModel = new ModelErrorList();
                                //objModel.Key = "IMS_AGREEMENT_CODE";
                                //objModel.Value = "Agreement Name Required";
                                //lstModelError.Add(objModel);

                                //Below condition added on 13-10-2022
                                if (!(transId == 3100 && teoDetailsModel.IMS_AGREEMENT_CODE == null))
                                {
                                    ModelErrorList objModel = new ModelErrorList();
                                    objModel.Key = "IMS_AGREEMENT_CODE";
                                    objModel.Value = "Agreement Name Required";
                                    lstModelError.Add(objModel);
                                }
                            }
                            else if (acc_bill_details != null)
                            {
                                if (validationParams.IS_AGREEMENT_REPEAT == "Y" && teoDetailsModel.IMS_AGREEMENT_CODE != acc_bill_details.IMS_AGREEMENT_CODE)
                                {
                                    ModelErrorList objModel = new ModelErrorList();
                                    objModel.Key = "IMS_AGREEMENT_CODE";
                                    objModel.Value = "Select " + dbContext.TEND_AGREEMENT_MASTER.Where(m => m.TEND_AGREEMENT_CODE == acc_bill_details.IMS_AGREEMENT_CODE).Select(m => m.TEND_AGREEMENT_NUMBER).FirstOrDefault();
                                    lstModelError.Add(objModel);
                                }
                            }
                        }


                        if (headParams.SANC_YEAR_REQ == "Y" && teoDetailsModel.SANC_YEAR == 0)
                        {
                            ModelErrorList objModel = new ModelErrorList();
                            objModel.Key = "SANC_YEAR";
                            objModel.Value = "Sanction Year Required";
                            lstModelError.Add(objModel);
                        }

                        if (headParams.ROAD_REQ == "Y")
                        {
                            if (teoDetailsModel.IMS_PR_ROAD_CODE_Head != null && teoDetailsModel.IMS_PR_ROAD_CODE_Head != 0)
                            {
                                teoDetailsModel.IMS_PR_ROAD_CODE = teoDetailsModel.IMS_PR_ROAD_CODE_Head;
                            }
                        }

                        if (headParams.ROAD_REQ == "Y" && (teoDetailsModel.IMS_PR_ROAD_CODE == 0 || teoDetailsModel.IMS_PR_ROAD_CODE == null))
                        {
                            //Below code commented on 13-10-2022
                            //ModelErrorList objModel = new ModelErrorList();
                            //objModel.Key = "IMS_PR_ROAD_CODE";
                            //objModel.Value = "Road Name Required";
                            //lstModelError.Add(objModel);

                            //Below condition added on 13-10-2022
                            if (!(transId == 3100 && teoDetailsModel.IMS_PR_ROAD_CODE == null))
                            {
                                ModelErrorList objModel = new ModelErrorList();
                                objModel.Key = "IMS_PR_ROAD_CODE";
                                objModel.Value = "Road Name Required";
                                lstModelError.Add(objModel);
                            }
                        }
                        else if (validationParams.IS_ROAD_REPEAT == "Y" && acc_bill_details != null && teoDetailsModel.IMS_PR_ROAD_CODE != acc_bill_details.IMS_PR_ROAD_CODE)
                        {
                            ModelErrorList objModel = new ModelErrorList();
                            objModel.Key = "IMS_PR_ROAD_CODE";
                            objModel.Value = "Select " + dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == acc_bill_details.IMS_PR_ROAD_CODE).Select(m => m.IMS_ROAD_NAME).FirstOrDefault();
                            lstModelError.Add(objModel);
                        }

                        if (headParams.PKG_REQ == "Y" && (teoDetailsModel.IMS_PACKAGE_ID == "0" || teoDetailsModel.IMS_PACKAGE_ID == null))
                        {
                            ModelErrorList objModel = new ModelErrorList();
                            objModel.Key = "IMS_PACKAGE_ID";
                            objModel.Value = "Package Required";
                            lstModelError.Add(objModel);
                        }

                    }
                }

                return lstModelError;
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw Ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        /// <summary>
        /// check for model error while making teo cedit and debit entry for transfer of balances added by koustubh nakate on 27/08/2013
        /// </summary>
        /// <param name="teoDetailsModelForTOB"></param> 
        /// <returns>validation error list</returns>
        public List<ModelErrorList> GetAddTEODetailsModelErrorsForTOB(TeoDetailsModelForTOB teoDetailsModelForTOB)
        {
            try
            {

                Int32 DistrictC = 0;
                Int32 PIUC = 0;
                Int32 DistrictD = 0;
                Int32 PIUD = 0;
                Int32? ContractorC = 0;
                //Added By Abhishek kamble to set State D
                Int32? StateD = 0;


                dbContext = new PMGSYEntities();
                commonFuncObj = new CommonFunctions();
                List<ModelErrorList> lstModelError = new List<ModelErrorList>();
                ACC_TEO_SCREEN_DESIGN_PARAM_DETAILS designParams = new ACC_TEO_SCREEN_DESIGN_PARAM_DETAILS();
                Int16 transId = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == teoDetailsModelForTOB.BILL_ID).Select(m => m.TXN_ID).FirstOrDefault();
                designParams = commonFuncObj.getTEODesignParamDetails(0, transId);
                if (designParams == null)
                {
                    designParams = setDefualtDesignTransParam();
                }
                ACC_TEO_SCREEN_DESIGN_HEADWISE_DETAILS headParamsC = new ACC_TEO_SCREEN_DESIGN_HEADWISE_DETAILS();
                headParamsC = dbContext.ACC_TEO_SCREEN_DESIGN_HEADWISE_DETAILS.Where(m => m.HEAD_ID == teoDetailsModelForTOB.HEAD_ID_C && m.TXN_ID == transId).FirstOrDefault();

                ACC_TEO_SCREEN_DESIGN_HEADWISE_DETAILS headParamsD = dbContext.ACC_TEO_SCREEN_DESIGN_HEADWISE_DETAILS.Where(m => m.HEAD_ID == teoDetailsModelForTOB.HEAD_ID_D && m.TXN_ID == transId).FirstOrDefault();

                if (headParamsC == null)
                {
                    headParamsC = setDefualtDesignHeadParam();
                }

                if (headParamsD == null)
                {
                    headParamsD = setDefualtDesignHeadParam();
                }


                ACC_TEO_SCREEN_TXN_VALIDATIONS validationParams = new ACC_TEO_SCREEN_TXN_VALIDATIONS();
                validationParams = dbContext.ACC_TEO_SCREEN_TXN_VALIDATIONS.Where(m => m.TXN_ID == transId).FirstOrDefault();
                if (validationParams == null)
                {
                    validationParams = setDefaultValidationParam();
                }
                //Int16 masterTransId = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == teoDetailsModel.BILL_ID).Select(m => m.TXN_ID).FirstOrDefault();

                //   ACC_BILL_DETAILS acc_bill_details = new ACC_BILL_DETAILS();


                if (transId == 164 || transId == 165 || transId == 1194 || transId == 1195)//|| transId == 1664 || transId == 1665 modified by Abhishek 6Jan2016
                {
                    if (teoDetailsModelForTOB.MAST_DISTRICT_CODE_C == teoDetailsModelForTOB.MAST_DISTRICT_CODE_D)
                    {
                        ModelErrorList objModel = new ModelErrorList();
                        objModel.Key = "MAST_DISTRICT_CODE_D";
                        objModel.Value = "Different District Required. Do not select " + dbContext.MASTER_DISTRICT.Where(m => m.MAST_DISTRICT_CODE == teoDetailsModelForTOB.MAST_DISTRICT_CODE_C).Select(m => m.MAST_DISTRICT_NAME).FirstOrDefault() + " District";
                        lstModelError.Add(objModel);
                    }

                }

                if (designParams.DISTRICT_REQ == "Y" && (teoDetailsModelForTOB.MAST_DISTRICT_CODE_C == 0 || teoDetailsModelForTOB.MAST_DISTRICT_CODE_C == null))
                {
                    ModelErrorList objModel = new ModelErrorList();
                    objModel.Key = "MAST_DISTRICT_CODE_C";
                    objModel.Value = "Credit District Required";
                    lstModelError.Add(objModel);
                }

                if (designParams.DISTRICT_REQ == "Y" && (teoDetailsModelForTOB.MAST_DISTRICT_CODE_D == 0 || teoDetailsModelForTOB.MAST_DISTRICT_CODE_D == null))
                {
                    ModelErrorList objModel = new ModelErrorList();
                    objModel.Key = "MAST_DISTRICT_CODE_D";
                    objModel.Value = "Debit District Required";
                    lstModelError.Add(objModel);
                }

                //added by Koustubh Nakate on 22/10/2013 to maintain district ,PIU and Contractor same after entry of one transaction  

                bool exist = CheckForTransactionAlreadyExistDAL(teoDetailsModelForTOB.BILL_ID, ref DistrictC, ref PIUC, ref DistrictD, ref PIUD, ref ContractorC, ref StateD);
                dbContext = new PMGSYEntities();
                //added by Koustubh Nakate on 30/08/2012 for checking district has been shifted


                //transId == 165 || transId == 1194  commneted by Abhihsek kamble 20Mar2014
                if (designParams.DISTRICT_REQ == "Y" && (transId == 164 || transId == 1195))
                {
                    if (teoDetailsModelForTOB.IMS_PR_ROAD_CODE_C == null)
                    {
                        //Added by Abhishek kamble 3June2015 start
                        int? DistrictCode = teoDetailsModelForTOB.MAST_DISTRICT_CODE_C;

                        string CREDIT_DEBIT = dbContext.ACC_MASTER_HEAD.Where(m => m.HEAD_ID == teoDetailsModelForTOB.HEAD_ID_C).Select(s => s.CREDIT_DEBIT).FirstOrDefault();

                        if (!String.IsNullOrEmpty(CREDIT_DEBIT))
                        {
                            if (CREDIT_DEBIT.Trim().ToUpper().Equals("C"))
                            {
                                DistrictCode = teoDetailsModelForTOB.MAST_DISTRICT_CODE_D;
                            }
                            else if (CREDIT_DEBIT.Trim().ToUpper().Equals("D"))
                            {
                                DistrictCode = teoDetailsModelForTOB.MAST_DISTRICT_CODE_C;
                            }
                        }
                        //Added by Abhishek kamble 3June2015 end
                        ////// if (!dbContext.IMS_PROPOSAL_TRACKING.Any(pt => pt.MAST_DISTRICT_CODE == teoDetailsModelForTOB.MAST_DISTRICT_CODE_C))
                        ////if (!dbContext.IMS_PROPOSAL_TRACKING.Any(pt => pt.MAST_DISTRICT_CODE == DistrictCode))//Modified by Abhishek kamble 3June2015
                        ////{
                        ////    ModelErrorList objModel = new ModelErrorList();
                        ////    objModel.Key = "MAST_DISTRICT_CODE_C";
                        ////    objModel.Value = "Credit District not shifted, so you can not transfer balance.";
                        ////    lstModelError.Add(objModel);
                        ////}


                        if (!dbContext.IMS_PROPOSAL_TRACKING.Any(pt => pt.MAST_DISTRICT_CODE == teoDetailsModelForTOB.MAST_DISTRICT_CODE_D || pt.MAST_DISTRICT_CODE == teoDetailsModelForTOB.MAST_DISTRICT_CODE_C))//Modified by Abhishek kamble 3June2015
                        {
                            ModelErrorList objModel = new ModelErrorList();
                            objModel.Key = "MAST_DISTRICT_CODE_C";
                            objModel.Value = "District not shifted, so you can not transfer balance.";
                            lstModelError.Add(objModel);
                        }

                    }
                    else
                    {
                        if (!dbContext.IMS_PROPOSAL_TRACKING.Any(pt => pt.MAST_DISTRICT_CODE == teoDetailsModelForTOB.MAST_DISTRICT_CODE_C && pt.IMS_PR_ROAD_CODE == teoDetailsModelForTOB.IMS_PR_ROAD_CODE_C))
                        {
                            ModelErrorList objModel = new ModelErrorList();
                            objModel.Key = "IMS_PR_ROAD_CODE_C";
                            objModel.Value = "No roads are transferred from selected district, Select another district.";//"Credit Road not shifted, so you can not transfer balance.";
                            lstModelError.Add(objModel);
                        }

                    }

                }

                if (designParams.DISTRICT_REQ == "Y" && exist == true)
                {
                    if (teoDetailsModelForTOB.MAST_DISTRICT_CODE_C != DistrictC)
                    {
                        ModelErrorList objModel = new ModelErrorList();
                        objModel.Key = "MAST_DISTRICT_CODE_C";
                        objModel.Value = "Credit district should be same through out the transaction.";
                        lstModelError.Add(objModel);
                    }

                    if (teoDetailsModelForTOB.MAST_DISTRICT_CODE_D != DistrictD)
                    {
                        ModelErrorList objModel = new ModelErrorList();
                        objModel.Key = "MAST_DISTRICT_CODE_D";
                        objModel.Value = "Debit district should be same through out the transaction.";
                        lstModelError.Add(objModel);
                    }

                }


                if (validationParams.IS_DISTRICT_REPEAT == "Y")
                {

                    if (teoDetailsModelForTOB.MAST_DISTRICT_CODE_C != teoDetailsModelForTOB.MAST_DISTRICT_CODE_D)
                    {
                        ModelErrorList objModel = new ModelErrorList();
                        objModel.Key = "MAST_DISTRICT_CODE_D";
                        objModel.Value = "Select " + dbContext.MASTER_DISTRICT.Where(m => m.MAST_DISTRICT_CODE == teoDetailsModelForTOB.MAST_DISTRICT_CODE_C).Select(m => m.MAST_DISTRICT_NAME).FirstOrDefault() + " District";
                        lstModelError.Add(objModel);
                    }
                }

                //if district should not repeat 
                if (validationParams.IS_DISTRICT_REPEAT == "N")
                {

                    if (teoDetailsModelForTOB.MAST_DISTRICT_CODE_C == teoDetailsModelForTOB.MAST_DISTRICT_CODE_D)
                    {
                        ModelErrorList objModel = new ModelErrorList();
                        objModel.Key = "MAST_DISTRICT_CODE_D";
                        objModel.Value = "Dont Select " + dbContext.MASTER_DISTRICT.Where(m => m.MAST_DISTRICT_CODE == teoDetailsModelForTOB.MAST_DISTRICT_CODE_C).Select(m => m.MAST_DISTRICT_NAME).FirstOrDefault() + " District as its Balance Transfer between two DPIUs of different district";
                        lstModelError.Add(objModel);
                    }
                }

                //same dpiu is not allowed
                if (transId == 163 || transId == 1193)
                {
                    if (teoDetailsModelForTOB.ADMIN_ND_CODE_C == teoDetailsModelForTOB.ADMIN_ND_CODE_D)
                    {
                        ModelErrorList objModel = new ModelErrorList();
                        objModel.Key = "ADMIN_ND_CODE_D";
                        objModel.Value = "Different PIU Required. Do not select " + dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == teoDetailsModelForTOB.ADMIN_ND_CODE_C).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
                        lstModelError.Add(objModel);
                    }
                }


                //dpiu should not repeat
                if (validationParams.IS_DPIU_REPEAT == "N")
                {
                    if (teoDetailsModelForTOB.ADMIN_ND_CODE_C == teoDetailsModelForTOB.ADMIN_ND_CODE_D)
                    {
                        ModelErrorList objModel = new ModelErrorList();
                        objModel.Key = "ADMIN_ND_CODE_D";
                        objModel.Value = "Dont Select " + dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == teoDetailsModelForTOB.ADMIN_ND_CODE_C).Select(m => m.ADMIN_ND_NAME).FirstOrDefault() + " as its  Balance Transfer between two DPIUs of different district";
                        lstModelError.Add(objModel);
                    }
                }

                if (validationParams.IS_DPIU_REPEAT == "Y")
                {
                    if (teoDetailsModelForTOB.ADMIN_ND_CODE_C != teoDetailsModelForTOB.ADMIN_ND_CODE_D)
                    {
                        ModelErrorList objModel = new ModelErrorList();
                        objModel.Key = "ADMIN_ND_CODE_D";
                        objModel.Value = "Select " + dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == teoDetailsModelForTOB.ADMIN_ND_CODE_C).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
                        lstModelError.Add(objModel);
                    }
                }






                if (designParams.DPIU_REQ == "Y" && (teoDetailsModelForTOB.ADMIN_ND_CODE_C == 0 || teoDetailsModelForTOB.ADMIN_ND_CODE_C == null))
                {
                    ModelErrorList objModel = new ModelErrorList();
                    objModel.Key = "ADMIN_ND_CODE_C";
                    objModel.Value = "Credit PIU Name Required";
                    lstModelError.Add(objModel);
                }

                if (designParams.DPIU_REQ == "Y" && (teoDetailsModelForTOB.ADMIN_ND_CODE_D == 0 || teoDetailsModelForTOB.ADMIN_ND_CODE_D == null))
                {
                    ModelErrorList objModel = new ModelErrorList();
                    objModel.Key = "ADMIN_ND_CODE_D";
                    objModel.Value = "Debit PIU Name Required";
                    lstModelError.Add(objModel);
                }



                if (designParams.DPIU_REQ == "Y" && exist == true)
                {
                    if (teoDetailsModelForTOB.ADMIN_ND_CODE_C != PIUC)
                    {
                        ModelErrorList objModel = new ModelErrorList();
                        objModel.Key = "ADMIN_ND_CODE_C";
                        objModel.Value = "Credit PIU should be same through out the transaction.";
                        lstModelError.Add(objModel);
                    }

                    if (teoDetailsModelForTOB.ADMIN_ND_CODE_D != PIUD)
                    {
                        ModelErrorList objModel = new ModelErrorList();
                        objModel.Key = "ADMIN_ND_CODE_D";
                        objModel.Value = "Debit PIU should be same through out the transaction.";
                        lstModelError.Add(objModel);
                    }

                }



                //change by amol 
                if ((teoDetailsModelForTOB.MAST_CON_ID_C == 0 || teoDetailsModelForTOB.MAST_CON_ID_C == null) && teoDetailsModelForTOB.MAST_CON_ID_TRANS_C != 0 && teoDetailsModelForTOB.MAST_CON_ID_TRANS_C != null)
                {
                    teoDetailsModelForTOB.MAST_CON_ID_C = teoDetailsModelForTOB.MAST_CON_ID_TRANS_C;
                }

                if ((teoDetailsModelForTOB.MAST_CON_ID_D == 0 || teoDetailsModelForTOB.MAST_CON_ID_D == null) && teoDetailsModelForTOB.MAST_CON_ID_TRANS_D != 0 && teoDetailsModelForTOB.MAST_CON_ID_TRANS_D != null)
                {
                    teoDetailsModelForTOB.MAST_CON_ID_D = teoDetailsModelForTOB.MAST_CON_ID_TRANS_D;
                }


                if (designParams.CON_REQ == "Y" && teoDetailsModelForTOB.MAST_CON_ID_C == 0 && teoDetailsModelForTOB.MAST_CON_ID_TRANS_C == 0)
                {
                    ModelErrorList objModel = new ModelErrorList();
                    objModel.Key = "MAST_CON_ID_C";
                    objModel.Value = "Credit Contractor Name Required";
                    lstModelError.Add(objModel);
                }

                if (designParams.CON_REQ == "Y" && teoDetailsModelForTOB.MAST_CON_ID_D == 0 && teoDetailsModelForTOB.MAST_CON_ID_TRANS_D == 0)
                {
                    ModelErrorList objModel = new ModelErrorList();
                    objModel.Key = "MAST_CON_ID_D";
                    objModel.Value = "Debit Contractor Name Required";
                    lstModelError.Add(objModel);
                }


                if (designParams.CON_REQ == "Y" && exist == true && (transId == 1187 || transId == 1192))
                {
                    if (teoDetailsModelForTOB.MAST_CON_ID_C != ContractorC)
                    {
                        ModelErrorList objModel = new ModelErrorList();
                        objModel.Key = "MAST_CON_ID_C";
                        objModel.Value = "Credit company name should be same through out the transaction.";
                        lstModelError.Add(objModel);
                    }

                    if (teoDetailsModelForTOB.MAST_CON_ID_D != ContractorC)
                    {
                        ModelErrorList objModel = new ModelErrorList();
                        objModel.Key = "MAST_CON_ID_D";
                        objModel.Value = "Debit company name should be same through out the transaction.";
                        lstModelError.Add(objModel);
                    }

                }



                if (validationParams.IS_CON_REPEAT == "Y")
                {
                    teoDetailsModelForTOB.MAST_CON_ID_D = teoDetailsModelForTOB.MAST_CON_ID_D == null ? 0 : teoDetailsModelForTOB.MAST_CON_ID_D;
                    teoDetailsModelForTOB.MAST_CON_ID_C = teoDetailsModelForTOB.MAST_CON_ID_C == null ? 0 : teoDetailsModelForTOB.MAST_CON_ID_C;

                    if (teoDetailsModelForTOB.MAST_CON_ID_C != teoDetailsModelForTOB.MAST_CON_ID_D)
                    {
                        ModelErrorList objModel = new ModelErrorList();
                        objModel.Key = "MAST_CON_ID_D";
                        objModel.Value = "Select " + dbContext.MASTER_CONTRACTOR.Where(m => m.MAST_CON_ID == teoDetailsModelForTOB.MAST_CON_ID_C).Select(m => m.MAST_CON_COMPANY_NAME).FirstOrDefault();
                        lstModelError.Add(objModel);
                    }
                }

                if (designParams.SUPP_REQ == "Y" && teoDetailsModelForTOB.MAST_CON_ID_C == 0)
                {
                    ModelErrorList objModel = new ModelErrorList();
                    objModel.Key = "MAST_CON_ID_C";
                    objModel.Value = "Credit Supplier Name Required";
                    lstModelError.Add(objModel);
                }

                if (designParams.SUPP_REQ == "Y" && teoDetailsModelForTOB.MAST_CON_ID_D == 0)
                {
                    ModelErrorList objModel = new ModelErrorList();
                    objModel.Key = "MAST_CON_ID_D";
                    objModel.Value = "Debit Supplier Name Required";
                    lstModelError.Add(objModel);
                }


                if (validationParams.IS_SUP_REPEAT == "Y")
                {
                    teoDetailsModelForTOB.MAST_CON_ID_D = teoDetailsModelForTOB.MAST_CON_ID_D == null ? 0 : teoDetailsModelForTOB.MAST_CON_ID_D;
                    teoDetailsModelForTOB.MAST_CON_ID_C = teoDetailsModelForTOB.MAST_CON_ID_C == null ? 0 : teoDetailsModelForTOB.MAST_CON_ID_C;


                    if (teoDetailsModelForTOB.MAST_CON_ID_C != teoDetailsModelForTOB.MAST_CON_ID_D)
                    {
                        ModelErrorList objModel = new ModelErrorList();
                        objModel.Key = "MAST_CON_ID_D";
                        objModel.Value = "Select " + dbContext.MASTER_CONTRACTOR.Where(m => m.MAST_CON_ID == teoDetailsModelForTOB.MAST_CON_ID_C).Select(m => m.MAST_CON_COMPANY_NAME).FirstOrDefault();
                        lstModelError.Add(objModel);
                    }
                }


                if (designParams.AFREEMENT_REQ == "Y" && (teoDetailsModelForTOB.IMS_AGREEMENT_CODE_C == 0 || teoDetailsModelForTOB.IMS_AGREEMENT_CODE_C == null))
                {
                    ModelErrorList objModel = new ModelErrorList();
                    objModel.Key = "IMS_AGREEMENT_CODE_C";
                    objModel.Value = "Credit Agreement Name Required";
                    lstModelError.Add(objModel);
                }

                if (designParams.AFREEMENT_REQ == "Y" && (teoDetailsModelForTOB.IMS_AGREEMENT_CODE_D == 0 || teoDetailsModelForTOB.IMS_AGREEMENT_CODE_D == null))
                {
                    ModelErrorList objModel = new ModelErrorList();
                    objModel.Key = "IMS_AGREEMENT_CODE_D";
                    objModel.Value = "Debit Agreement Name Required";
                    lstModelError.Add(objModel);
                }

                if (validationParams.IS_AGREEMENT_REPEAT == "Y")
                {
                    teoDetailsModelForTOB.IMS_AGREEMENT_CODE_D = teoDetailsModelForTOB.IMS_AGREEMENT_CODE_D == null ? 0 : teoDetailsModelForTOB.IMS_AGREEMENT_CODE_D;
                    teoDetailsModelForTOB.IMS_AGREEMENT_CODE_C = teoDetailsModelForTOB.IMS_AGREEMENT_CODE_C == null ? 0 : teoDetailsModelForTOB.IMS_AGREEMENT_CODE_C;


                    if (teoDetailsModelForTOB.IMS_AGREEMENT_CODE_C != teoDetailsModelForTOB.IMS_AGREEMENT_CODE_D)
                    {
                        ModelErrorList objModel = new ModelErrorList();
                        objModel.Key = "IMS_AGREEMENT_CODE_D";
                        objModel.Value = "Select " + dbContext.TEND_AGREEMENT_MASTER.Where(m => m.TEND_AGREEMENT_CODE == teoDetailsModelForTOB.IMS_AGREEMENT_CODE_C).Select(m => m.TEND_AGREEMENT_NUMBER).FirstOrDefault();
                        lstModelError.Add(objModel);
                    }
                }
                else if (validationParams.IS_AGREEMENT_REPEAT == "N")
                {
                    if (teoDetailsModelForTOB.IMS_AGREEMENT_CODE_C == teoDetailsModelForTOB.IMS_AGREEMENT_CODE_D && (teoDetailsModelForTOB.IMS_AGREEMENT_CODE_C != null || teoDetailsModelForTOB.IMS_AGREEMENT_CODE_D != null))
                    {
                        ModelErrorList objModel = new ModelErrorList();
                        objModel.Key = "IMS_AGREEMENT_CODE_D";
                        objModel.Value = "Please select different Agreement.";
                        lstModelError.Add(objModel);
                    }
                }


                if (designParams.SAN_REQ == "Y" && (teoDetailsModelForTOB.SANC_YEAR_C == 0 || teoDetailsModelForTOB.SANC_YEAR_C == null))
                {
                    ModelErrorList objModel = new ModelErrorList();
                    objModel.Key = "SANC_YEAR_C";
                    objModel.Value = "Credit Sanction Year Required";
                    lstModelError.Add(objModel);
                }

                if (designParams.SAN_REQ == "Y" && (teoDetailsModelForTOB.SANC_YEAR_D == 0 || teoDetailsModelForTOB.SANC_YEAR_D == null))
                {
                    ModelErrorList objModel = new ModelErrorList();
                    objModel.Key = "SANC_YEAR_D";
                    objModel.Value = "Debit Sanction Year Required";
                    lstModelError.Add(objModel);
                }


                if (designParams.ROAD_REQ == "Y" && (teoDetailsModelForTOB.IMS_PR_ROAD_CODE_C == 0 || teoDetailsModelForTOB.IMS_PR_ROAD_CODE_C == null))
                {
                    ModelErrorList objModel = new ModelErrorList();
                    objModel.Key = "IMS_PR_ROAD_CODE_C";
                    objModel.Value = "Credit Road Name Required";
                    lstModelError.Add(objModel);
                }

                if (designParams.ROAD_REQ == "Y" && (teoDetailsModelForTOB.IMS_PR_ROAD_CODE_D == 0 || teoDetailsModelForTOB.IMS_PR_ROAD_CODE_D == null))
                {
                    ModelErrorList objModel = new ModelErrorList();
                    objModel.Key = "IMS_PR_ROAD_CODE_D";
                    objModel.Value = "Debit Road Name Required";
                    lstModelError.Add(objModel);
                }

                if (validationParams.IS_ROAD_REPEAT == "Y")
                {
                    teoDetailsModelForTOB.IMS_PR_ROAD_CODE_D = teoDetailsModelForTOB.IMS_PR_ROAD_CODE_D == null ? 0 : teoDetailsModelForTOB.IMS_PR_ROAD_CODE_D;
                    teoDetailsModelForTOB.IMS_PR_ROAD_CODE_C = teoDetailsModelForTOB.IMS_PR_ROAD_CODE_C == null ? 0 : teoDetailsModelForTOB.IMS_PR_ROAD_CODE_C;

                    if (teoDetailsModelForTOB.IMS_PR_ROAD_CODE_C != teoDetailsModelForTOB.IMS_PR_ROAD_CODE_D)
                    {
                        ModelErrorList objModel = new ModelErrorList();
                        objModel.Key = "IMS_PR_ROAD_CODE_D";
                        objModel.Value = "Select " + dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == teoDetailsModelForTOB.IMS_PR_ROAD_CODE_C).Select(m => m.IMS_ROAD_NAME).FirstOrDefault();
                        lstModelError.Add(objModel);
                    }
                }

                if (designParams.PKG_REQ == "Y" && (teoDetailsModelForTOB.IMS_PACKAGE_ID_C == "0" || teoDetailsModelForTOB.IMS_PACKAGE_ID_C == null))
                {
                    ModelErrorList objModel = new ModelErrorList();
                    objModel.Key = "IMS_PACKAGE_ID_C";
                    objModel.Value = "Credit Package Required";
                    lstModelError.Add(objModel);
                }

                if (designParams.PKG_REQ == "Y" && (teoDetailsModelForTOB.IMS_PACKAGE_ID_D == "0" || teoDetailsModelForTOB.IMS_PACKAGE_ID_D == null))
                {
                    ModelErrorList objModel = new ModelErrorList();
                    objModel.Key = "IMS_PACKAGE_ID_D";
                    objModel.Value = "Debit Package Required";
                    lstModelError.Add(objModel);
                }


                #region  amount validation e by amol jadhav
                //validation of amount the total credit/debit amount should be equal to or less than master amount
                //get the master amount 
                decimal masterAmount = 0;
                decimal? detailsTotalAmountC = 0;
                decimal? detailsTotalAmountD = 0;
                decimal prevAmount = 0; //amount being sanctioned
                masterAmount = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == teoDetailsModelForTOB.BILL_ID).Select(m => m.GROSS_AMOUNT).FirstOrDefault();


                detailsTotalAmountC = dbContext.ACC_BILL_DETAILS.Where(bd => bd.BILL_ID == teoDetailsModelForTOB.BILL_ID && bd.CREDIT_DEBIT == "C").Sum(bd => (decimal?)bd.AMOUNT);

                detailsTotalAmountD = dbContext.ACC_BILL_DETAILS.Where(bd => bd.BILL_ID == teoDetailsModelForTOB.BILL_ID && bd.CREDIT_DEBIT == "D").Sum(bd => (decimal?)bd.AMOUNT);

                detailsTotalAmountC = detailsTotalAmountC == null ? 0 : detailsTotalAmountC;
                detailsTotalAmountD = detailsTotalAmountD == null ? 0 : detailsTotalAmountD;

                //check if current entry is already exist (thats means its edit operation)

                //commented by Koustubh Nakate on 28/08/2013 for we are not providing edit facility now 
                //if (teoDetailsModelForTOB.TXN_NO != 0)
                //{
                //    prevAmount = dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == teoDetailsModelForTOB.BILL_ID && m.TXN_NO == teoDetailsModelForTOB.TXN_NO).Select(m => m.AMOUNT).FirstOrDefault();
                //}

                //check for valid amount
                if (masterAmount < (detailsTotalAmountC + teoDetailsModelForTOB.AMOUNT_C))
                {
                    ModelErrorList objModel = new ModelErrorList();
                    objModel.Key = "AMOUNT_C";
                    objModel.Value = "Total Credit Details Amount should be less than or equal to Master Amount";
                    lstModelError.Add(objModel);
                    return lstModelError;
                }

                if (masterAmount < (detailsTotalAmountD + teoDetailsModelForTOB.AMOUNT_D))
                {
                    ModelErrorList objModel = new ModelErrorList();
                    objModel.Key = "AMOUNT_D";
                    objModel.Value = "Total Debit Details Amount should be less than or equal to Master Amount";
                    lstModelError.Add(objModel);
                    return lstModelError;
                }

                #endregion amount Validation

                if (teoDetailsModelForTOB.HEAD_ID_C == 0)
                {
                    ModelErrorList objModel = new ModelErrorList();
                    objModel.Key = "HEAD_ID_C";
                    objModel.Value = "Credit Account Head Required";
                    lstModelError.Add(objModel);
                    return lstModelError;
                }
                else if (teoDetailsModelForTOB.HEAD_ID_D == 0)
                {
                    ModelErrorList objModel = new ModelErrorList();
                    objModel.Key = "HEAD_ID_D";
                    objModel.Value = "Debit Account Head Required";
                    lstModelError.Add(objModel);
                }
                else
                {
                    if (validationParams.C_CREDIT_HEAD != 0 && validationParams.C_CREDIT_HEAD != null)
                    {
                        if (teoDetailsModelForTOB.HEAD_ID_C != validationParams.C_CREDIT_HEAD)
                        {
                            ModelErrorList objModel = new ModelErrorList();
                            objModel.Key = "HEAD_ID_C";
                            objModel.Value = "Select " + dbContext.ACC_MASTER_HEAD.Where(m => m.HEAD_ID == validationParams.C_CREDIT_HEAD).Select(m => m.HEAD_NAME).FirstOrDefault();
                            lstModelError.Add(objModel);
                        }
                    }

                    if (validationParams.C_DEBIT_HEAD != 0 && validationParams.C_DEBIT_HEAD != null)
                    {
                        if (teoDetailsModelForTOB.HEAD_ID_D != validationParams.C_DEBIT_HEAD)
                        {
                            ModelErrorList objModel = new ModelErrorList();
                            objModel.Key = "HEAD_ID_D";
                            objModel.Value = "Select " + dbContext.ACC_MASTER_HEAD.Where(m => m.HEAD_ID == validationParams.C_DEBIT_HEAD).Select(m => m.HEAD_NAME).FirstOrDefault();
                            lstModelError.Add(objModel);
                        }
                    }


                    if (validationParams.IS_HEAD_REPEAT == "Y" && (teoDetailsModelForTOB.HEAD_ID_C != teoDetailsModelForTOB.HEAD_ID_D))
                    {
                        ModelErrorList objModel = new ModelErrorList();
                        objModel.Key = "HEAD_ID_D";
                        objModel.Value = "Select " + dbContext.ACC_MASTER_HEAD.Where(m => m.HEAD_ID == teoDetailsModelForTOB.HEAD_ID_C).Select(m => m.HEAD_NAME).FirstOrDefault();
                        lstModelError.Add(objModel);
                    }


                    //validations for headwise screen design
                    if (headParamsC.CON_REQ == "Y" && (teoDetailsModelForTOB.MAST_CON_ID_C == 0 || teoDetailsModelForTOB.MAST_CON_ID_C == null))
                    {
                        ModelErrorList objModel = new ModelErrorList();
                        objModel.Key = "MAST_CON_ID_C";
                        objModel.Value = "Credit Contractor Name Required";
                        lstModelError.Add(objModel);

                    }

                    if (headParamsD.CON_REQ == "Y" && (teoDetailsModelForTOB.MAST_CON_ID_D == 0 || teoDetailsModelForTOB.MAST_CON_ID_D == null))
                    {
                        ModelErrorList objModel = new ModelErrorList();
                        objModel.Key = "MAST_CON_ID_D";
                        objModel.Value = "Debit Contractor Name Required";
                        lstModelError.Add(objModel);
                    }


                    if (headParamsC.AGREEMENT_REQ == "Y" && (teoDetailsModelForTOB.IMS_AGREEMENT_CODE_C == 0 || teoDetailsModelForTOB.IMS_AGREEMENT_CODE_C == null))
                    {
                        ModelErrorList objModel = new ModelErrorList();
                        objModel.Key = "IMS_AGREEMENT_CODE_C";
                        objModel.Value = "Credit Agreement Name Required";
                        lstModelError.Add(objModel);
                    }

                    if (headParamsD.AGREEMENT_REQ == "Y" && (teoDetailsModelForTOB.IMS_AGREEMENT_CODE_D == 0 || teoDetailsModelForTOB.IMS_AGREEMENT_CODE_D == null))
                    {
                        ModelErrorList objModel = new ModelErrorList();
                        objModel.Key = "IMS_AGREEMENT_CODE_D";
                        objModel.Value = "Debit Agreement Name Required";
                        lstModelError.Add(objModel);
                    }

                    if (headParamsC.SANC_YEAR_REQ == "Y" && (teoDetailsModelForTOB.SANC_YEAR_C == 0 || teoDetailsModelForTOB.SANC_YEAR_D == 0))
                    {
                        ModelErrorList objModel = new ModelErrorList();
                        objModel.Key = "SANC_YEAR_C";
                        objModel.Value = "Credit Sanction Year Required";
                        lstModelError.Add(objModel);
                    }

                    if (headParamsD.SANC_YEAR_REQ == "Y" && (teoDetailsModelForTOB.SANC_YEAR_D == 0 || teoDetailsModelForTOB.SANC_YEAR_D == 0))
                    {
                        ModelErrorList objModel = new ModelErrorList();
                        objModel.Key = "SANC_YEAR_D";
                        objModel.Value = "Debit Sanction Year Required";
                        lstModelError.Add(objModel);
                    }

                    if (headParamsC.ROAD_REQ == "Y" && (teoDetailsModelForTOB.IMS_PR_ROAD_CODE_C == 0 || teoDetailsModelForTOB.IMS_PR_ROAD_CODE_C == null))
                    {
                        ModelErrorList objModel = new ModelErrorList();
                        objModel.Key = "IMS_PR_ROAD_CODE_C";
                        objModel.Value = "Credit Road Name Required";
                        lstModelError.Add(objModel);
                    }

                    if (headParamsD.ROAD_REQ == "Y" && (teoDetailsModelForTOB.IMS_PR_ROAD_CODE_D == 0 || teoDetailsModelForTOB.IMS_PR_ROAD_CODE_D == null))
                    {
                        ModelErrorList objModel = new ModelErrorList();
                        objModel.Key = "IMS_PR_ROAD_CODE_D";
                        objModel.Value = "Debit Road Name Required";
                        lstModelError.Add(objModel);
                    }

                    if (headParamsC.PKG_REQ == "Y" && (teoDetailsModelForTOB.IMS_PACKAGE_ID_C == "0" || teoDetailsModelForTOB.IMS_PACKAGE_ID_C == null))
                    {
                        ModelErrorList objModel = new ModelErrorList();
                        objModel.Key = "IMS_PACKAGE_ID_C";
                        objModel.Value = "Credit Package Required";
                        lstModelError.Add(objModel);
                    }

                    if (headParamsD.PKG_REQ == "Y" && (teoDetailsModelForTOB.IMS_PACKAGE_ID_D == "0" || teoDetailsModelForTOB.IMS_PACKAGE_ID_D == null))
                    {
                        ModelErrorList objModel = new ModelErrorList();
                        objModel.Key = "IMS_PACKAGE_ID_D";
                        objModel.Value = "Debit Package Required";
                        lstModelError.Add(objModel);
                    }

                }

                return lstModelError;
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw Ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public String AddCreditTEODetails(TeoDetailsModel teoDetailsModel, out Int16 transId)
        {
            try
            {
                dbContext = new PMGSYEntities();
                using (var scope = new TransactionScope())
                {
                    teoDetailsModel.CREDIT_DEBIT = "C";
                    teoDetailsModel.CASH_CHQ = "J";


                    ACC_TEO_SCREEN_DESIGN_PARAM_DETAILS trans_design_params = new ACC_TEO_SCREEN_DESIGN_PARAM_DETAILS();
                    ACC_TEO_SCREEN_DESIGN_HEADWISE_DETAILS head_design_params = new ACC_TEO_SCREEN_DESIGN_HEADWISE_DETAILS();
                    Int16 masterTransId = dbContext.ACC_BILL_MASTER.Where(b => b.BILL_ID == teoDetailsModel.BILL_ID).Select(b => b.TXN_ID).FirstOrDefault();
                    trans_design_params = dbContext.ACC_TEO_SCREEN_DESIGN_PARAM_DETAILS.Where(m => m.TXN_ID == masterTransId).FirstOrDefault();
                    head_design_params = dbContext.ACC_TEO_SCREEN_DESIGN_HEADWISE_DETAILS.Where(m => m.HEAD_ID == teoDetailsModel.HEAD_ID && m.TXN_ID == masterTransId).FirstOrDefault();
                    if (trans_design_params == null)
                    {
                        trans_design_params = setDefualtDesignTransParam();
                    }
                    if (head_design_params == null)
                    {
                        head_design_params = setDefualtDesignHeadParam();
                    }

                    if (trans_design_params.DISTRICT_REQ == "N")
                    {
                        teoDetailsModel.MAST_DISTRICT_CODE = null;
                    }
                    if (trans_design_params.DPIU_REQ == "N")
                    {
                        teoDetailsModel.ADMIN_ND_CODE = null;
                    }
                    if (trans_design_params.CON_REQ == "N" && trans_design_params.SUPP_REQ == "N" && head_design_params.CON_REQ == "N")
                    {
                        teoDetailsModel.MAST_CON_ID = null;
                    }
                    if (trans_design_params.AFREEMENT_REQ == "N" && head_design_params.AGREEMENT_REQ == "N")
                    {
                        teoDetailsModel.IMS_AGREEMENT_CODE = null;
                    }
                    if (trans_design_params.SAN_REQ == "N" && head_design_params.SANC_YEAR_REQ == "N")
                    {
                        teoDetailsModel.SANC_YEAR = null;
                    }
                    if (trans_design_params.PKG_REQ == "N" && head_design_params.PKG_REQ == "N")
                    {
                        teoDetailsModel.IMS_PACKAGE_ID = null;
                    }
                    if (trans_design_params.ROAD_REQ == "N" && head_design_params.ROAD_REQ == "N")
                    {
                        teoDetailsModel.IMS_PR_ROAD_CODE = null;
                        teoDetailsModel.FINAL_PAYMENT = false;
                    }

                    if (teoDetailsModel.IMS_PR_ROAD_CODE != null)
                    {
                        teoDetailsModel.MAS_FA_CODE = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == teoDetailsModel.IMS_PR_ROAD_CODE).Select(m => m.IMS_COLLABORATION).FirstOrDefault();
                    }

                    Int16? maxTxnId = dbContext.ACC_BILL_DETAILS.Where(item => item.BILL_ID == teoDetailsModel.BILL_ID).Max(T => (Int16?)T.TXN_NO);
                    if (maxTxnId == null)
                    {
                        teoDetailsModel.TXN_NO = 1;
                    }
                    else
                    {
                        teoDetailsModel.TXN_NO = Convert.ToInt16(maxTxnId + 1);
                    }
                    ACC_BILL_DETAILS acc_bill_details = new ACC_BILL_DETAILS();
                    acc_bill_details = CloneDetailsModel(teoDetailsModel);

                    dbContext.ACC_BILL_DETAILS.Add(acc_bill_details);
                    dbContext.SaveChanges();

                    transId = teoDetailsModel.TXN_NO;
                    scope.Complete();
                    return String.Empty;
                }
            }
            catch (DbEntityValidationException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                var errorMessages = ex.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.ErrorMessage);
                var fullErrorMessage = string.Join("; ", errorMessages);
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);
                transId = 0;
                return exceptionMessage.ToString();
                //throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }
            catch (EntityCommandExecutionException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                transId = 0;
                return ex.Message;
            }
            catch (EntityException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                transId = 0;
                return ex.Message;
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                transId = 0;
                return Ex.Message;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public String AddDebitTEODetails(TeoDetailsModel teoDetailsModel, out Int16 transId)
        {
            try
            {
                dbContext = new PMGSYEntities();
                using (var scope = new TransactionScope())
                {
                    teoDetailsModel.CREDIT_DEBIT = "D";
                    teoDetailsModel.CASH_CHQ = "J";
                    ACC_TEO_SCREEN_DESIGN_PARAM_DETAILS trans_design_params = new ACC_TEO_SCREEN_DESIGN_PARAM_DETAILS();
                    ACC_TEO_SCREEN_DESIGN_HEADWISE_DETAILS head_design_params = new ACC_TEO_SCREEN_DESIGN_HEADWISE_DETAILS();
                    Int16 masterTransId = dbContext.ACC_BILL_MASTER.Where(b => b.BILL_ID == teoDetailsModel.BILL_ID).Select(b => b.TXN_ID).FirstOrDefault();
                    trans_design_params = dbContext.ACC_TEO_SCREEN_DESIGN_PARAM_DETAILS.Where(m => m.TXN_ID == masterTransId).FirstOrDefault();
                    head_design_params = dbContext.ACC_TEO_SCREEN_DESIGN_HEADWISE_DETAILS.Where(m => m.HEAD_ID == teoDetailsModel.HEAD_ID && m.TXN_ID == masterTransId).FirstOrDefault();
                    if (trans_design_params == null)
                    {
                        trans_design_params = setDefualtDesignTransParam();
                    }
                    if (head_design_params == null)
                    {
                        head_design_params = setDefualtDesignHeadParam();
                    }

                    if (trans_design_params.DISTRICT_REQ == "N")
                    {
                        teoDetailsModel.MAST_DISTRICT_CODE = null;
                    }
                    if (trans_design_params.DPIU_REQ == "N")
                    {
                        teoDetailsModel.ADMIN_ND_CODE = null;
                    }
                    if (trans_design_params.CON_REQ == "N" && trans_design_params.SUPP_REQ == "N" && head_design_params.CON_REQ == "N")
                    {
                        teoDetailsModel.MAST_CON_ID = null;
                    }
                    if (trans_design_params.AFREEMENT_REQ == "N" && head_design_params.AGREEMENT_REQ == "N")
                    {
                        teoDetailsModel.IMS_AGREEMENT_CODE = null;
                    }
                    if (trans_design_params.SAN_REQ == "N" && head_design_params.SANC_YEAR_REQ == "N")
                    {
                        teoDetailsModel.SANC_YEAR = null;
                    }
                    if (trans_design_params.PKG_REQ == "N" && head_design_params.PKG_REQ == "N")
                    {
                        teoDetailsModel.IMS_PACKAGE_ID = null;
                    }
                    if (trans_design_params.ROAD_REQ == "N" && head_design_params.ROAD_REQ == "N")
                    {
                        teoDetailsModel.IMS_PR_ROAD_CODE = null;
                        teoDetailsModel.FINAL_PAYMENT = false;
                    }

                    if (teoDetailsModel.IMS_PR_ROAD_CODE != null)
                    {
                        teoDetailsModel.MAS_FA_CODE = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == teoDetailsModel.IMS_PR_ROAD_CODE).Select(m => m.IMS_COLLABORATION).FirstOrDefault();
                    }

                    Int16? maxTxnId = dbContext.ACC_BILL_DETAILS.Where(item => item.BILL_ID == teoDetailsModel.BILL_ID).Max(T => (Int16?)T.TXN_NO);
                    if (maxTxnId == null)
                    {
                        teoDetailsModel.TXN_NO = 1;
                    }
                    else
                    {
                        teoDetailsModel.TXN_NO = Convert.ToInt16(maxTxnId + 1);
                    }
                    //if (dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Where(m => m.S_BIll_ID == teoDetailsModel.BILL_ID).Any())
                    //{
                    //    ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS map_details = new ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS();
                    //    map_details = dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Where(m => m.S_BIll_ID == teoDetailsModel.BILL_ID).FirstOrDefault();
                    //    map_details.P_TXN_ID = teoDetailsModel.TXN_NO;
                    //    dbContext.Entry(map_details).State = System.Data.Entity.EntityState.Modified;
                    //    dbContext.SaveChanges();
                    //}
                    ACC_BILL_DETAILS acc_bill_details = new ACC_BILL_DETAILS();
                    acc_bill_details = CloneDetailsModel(teoDetailsModel);

                    dbContext.ACC_BILL_DETAILS.Add(acc_bill_details);
                    dbContext.SaveChanges();

                    transId = teoDetailsModel.TXN_NO;
                    scope.Complete();
                    return String.Empty;
                }
            }
            catch (EntityCommandExecutionException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                transId = 0;
                return ex.Message;
            }
            catch (EntityException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                transId = 0;
                return ex.Message;
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                transId = 0;
                return Ex.Message;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public String EditCreditTEODetails(TeoDetailsModel teoDetailsModel)
        {
            ACC_BILL_DETAILS old_acc_bill_details = new ACC_BILL_DETAILS();
            try
            {
                dbContext = new PMGSYEntities();
                using (var scope = new TransactionScope())
                {
                    teoDetailsModel.CREDIT_DEBIT = "C";
                    teoDetailsModel.CASH_CHQ = "J";
                    old_acc_bill_details = dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == teoDetailsModel.BILL_ID && m.TXN_NO == teoDetailsModel.TXN_NO).FirstOrDefault();

                    ACC_TEO_SCREEN_DESIGN_PARAM_DETAILS trans_design_params = new ACC_TEO_SCREEN_DESIGN_PARAM_DETAILS();
                    ACC_TEO_SCREEN_DESIGN_HEADWISE_DETAILS head_design_params = new ACC_TEO_SCREEN_DESIGN_HEADWISE_DETAILS();
                    Int16 masterTransId = dbContext.ACC_BILL_MASTER.Where(b => b.BILL_ID == teoDetailsModel.BILL_ID).Select(b => b.TXN_ID).FirstOrDefault();
                    trans_design_params = dbContext.ACC_TEO_SCREEN_DESIGN_PARAM_DETAILS.Where(m => m.TXN_ID == masterTransId).FirstOrDefault();
                    head_design_params = dbContext.ACC_TEO_SCREEN_DESIGN_HEADWISE_DETAILS.Where(m => m.HEAD_ID == teoDetailsModel.HEAD_ID && m.TXN_ID == masterTransId).FirstOrDefault();
                    if (trans_design_params == null)
                    {
                        trans_design_params = setDefualtDesignTransParam();
                    }
                    if (head_design_params == null)
                    {
                        head_design_params = setDefualtDesignHeadParam();
                    }

                    if (trans_design_params.DISTRICT_REQ == "N")
                    {
                        teoDetailsModel.MAST_DISTRICT_CODE = null;
                    }
                    if (trans_design_params.DPIU_REQ == "N")
                    {
                        teoDetailsModel.ADMIN_ND_CODE = null;
                    }
                    if (trans_design_params.CON_REQ == "N" && trans_design_params.SUPP_REQ == "N" && head_design_params.CON_REQ == "N")
                    {
                        teoDetailsModel.MAST_CON_ID = null;
                    }
                    if (trans_design_params.AFREEMENT_REQ == "N" && head_design_params.AGREEMENT_REQ == "N")
                    {
                        teoDetailsModel.IMS_AGREEMENT_CODE = null;
                    }
                    if (trans_design_params.SAN_REQ == "N" && head_design_params.SANC_YEAR_REQ == "N")
                    {
                        teoDetailsModel.SANC_YEAR = null;
                    }
                    if (trans_design_params.PKG_REQ == "N" && head_design_params.PKG_REQ == "N")
                    {
                        teoDetailsModel.IMS_PACKAGE_ID = null;
                    }
                    if (trans_design_params.ROAD_REQ == "N" && head_design_params.ROAD_REQ == "N")
                    {
                        teoDetailsModel.IMS_PR_ROAD_CODE = null;
                        teoDetailsModel.FINAL_PAYMENT = false;
                    }

                    if (teoDetailsModel.IMS_PR_ROAD_CODE != null)
                    {
                        teoDetailsModel.MAS_FA_CODE = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == teoDetailsModel.IMS_PR_ROAD_CODE).Select(m => m.IMS_COLLABORATION).FirstOrDefault();
                    }

                    ACC_BILL_DETAILS new_acc_bill_details = new ACC_BILL_DETAILS();
                    new_acc_bill_details = CloneDetailsModel(teoDetailsModel);
                    dbContext.Entry(old_acc_bill_details).CurrentValues.SetValues(new_acc_bill_details);
                    dbContext.SaveChanges();

                    scope.Complete();
                    return String.Empty;
                }
            }
            catch (DbEntityValidationException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                var errorMessages = ex.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.ErrorMessage);
                var fullErrorMessage = string.Join("; ", errorMessages);
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);
                return exceptionMessage.ToString();
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
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                return Ex.Message;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public String EditDebitTEODetails(TeoDetailsModel teoDetailsModel)
        {
            ACC_BILL_DETAILS old_acc_bill_details = new ACC_BILL_DETAILS();
            try
            {
                dbContext = new PMGSYEntities();
                using (var scope = new TransactionScope())
                {
                    teoDetailsModel.CREDIT_DEBIT = "D";
                    teoDetailsModel.CASH_CHQ = "J";
                    old_acc_bill_details = dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == teoDetailsModel.BILL_ID && m.TXN_NO == teoDetailsModel.TXN_NO).FirstOrDefault();

                    ACC_TEO_SCREEN_DESIGN_PARAM_DETAILS trans_design_params = new ACC_TEO_SCREEN_DESIGN_PARAM_DETAILS();
                    ACC_TEO_SCREEN_DESIGN_HEADWISE_DETAILS head_design_params = new ACC_TEO_SCREEN_DESIGN_HEADWISE_DETAILS();
                    Int16 masterTransId = dbContext.ACC_BILL_MASTER.Where(b => b.BILL_ID == teoDetailsModel.BILL_ID).Select(b => b.TXN_ID).FirstOrDefault();
                    trans_design_params = dbContext.ACC_TEO_SCREEN_DESIGN_PARAM_DETAILS.Where(m => m.TXN_ID == masterTransId).FirstOrDefault();
                    head_design_params = dbContext.ACC_TEO_SCREEN_DESIGN_HEADWISE_DETAILS.Where(m => m.HEAD_ID == teoDetailsModel.HEAD_ID && m.TXN_ID == masterTransId).FirstOrDefault();
                    if (trans_design_params == null)
                    {
                        trans_design_params = setDefualtDesignTransParam();
                    }
                    if (head_design_params == null)
                    {
                        head_design_params = setDefualtDesignHeadParam();
                    }

                    if (trans_design_params.DISTRICT_REQ == "N")
                    {
                        teoDetailsModel.MAST_DISTRICT_CODE = null;
                    }
                    if (trans_design_params.DPIU_REQ == "N")
                    {
                        teoDetailsModel.ADMIN_ND_CODE = null;
                    }
                    if (trans_design_params.CON_REQ == "N" && trans_design_params.SUPP_REQ == "N" && head_design_params.CON_REQ == "N")
                    {
                        teoDetailsModel.MAST_CON_ID = null;
                    }
                    if (trans_design_params.AFREEMENT_REQ == "N" && head_design_params.AGREEMENT_REQ == "N")
                    {
                        teoDetailsModel.IMS_AGREEMENT_CODE = null;
                    }
                    if (trans_design_params.SAN_REQ == "N" && head_design_params.SANC_YEAR_REQ == "N")
                    {
                        teoDetailsModel.SANC_YEAR = null;
                    }
                    if (trans_design_params.PKG_REQ == "N" && head_design_params.PKG_REQ == "N")
                    {
                        teoDetailsModel.IMS_PACKAGE_ID = null;
                    }
                    if (trans_design_params.ROAD_REQ == "N" && head_design_params.ROAD_REQ == "N")
                    {
                        teoDetailsModel.IMS_PR_ROAD_CODE = null;
                        teoDetailsModel.FINAL_PAYMENT = false;
                    }

                    if (teoDetailsModel.IMS_PR_ROAD_CODE != null)
                    {
                        teoDetailsModel.MAS_FA_CODE = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == teoDetailsModel.IMS_PR_ROAD_CODE).Select(m => m.IMS_COLLABORATION).FirstOrDefault();
                    }

                    ACC_BILL_DETAILS new_acc_bill_details = new ACC_BILL_DETAILS();
                    new_acc_bill_details = CloneDetailsModel(teoDetailsModel);
                    dbContext.Entry(old_acc_bill_details).CurrentValues.SetValues(new_acc_bill_details);
                    dbContext.SaveChanges();

                    scope.Complete();
                    return String.Empty;
                }
            }
            catch (DbEntityValidationException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                var errorMessages = ex.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.ErrorMessage);
                var fullErrorMessage = string.Join("; ", errorMessages);
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);
                return exceptionMessage.ToString();
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
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                return Ex.Message;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array TEOMasterList(ReceiptFilterModel objFilter)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<ACC_BILL_MASTER> lstBillMaster = null;
                lstBillMaster = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == objFilter.BillId).ToList<ACC_BILL_MASTER>();

                return lstBillMaster.Select(item => new
                {
                    id = URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim() }),
                    cell = new[] {                         
                                    
                                    item.BILL_NO,
                                    Convert.ToDateTime(item.BILL_DATE).ToString("dd/MM/yyyy"),
                                    //Added By Abhishek kamble 9-June-2014                                    
                                    ((item.TXN_ID==161||item.TXN_ID==162||item.TXN_ID==397||item.TXN_ID==398||item.TXN_ID==400||item.TXN_ID==401|| item.TXN_ID==844||item.TXN_ID==845||item.TXN_ID==846||item.TXN_ID==847||item.TXN_ID==848||item.TXN_ID==849||item.TXN_ID==850||item.TXN_ID==852||item.TXN_ID==853||item.TXN_ID==854||item.TXN_ID==855||item.TXN_ID==856||item.TXN_ID==857||item.TXN_ID==858) && (dbContext.ACC_BILL_DETAILS.Where(m=>m.BILL_ID==item.BILL_ID && m.ADMIN_ND_CODE!=null).Any()))?item.ACC_MASTER_TXN.TXN_DESC.Trim() +" ( "+ dbContext.ADMIN_DEPARTMENT.Where(a=>a.ADMIN_ND_CODE==dbContext.ACC_BILL_DETAILS.Where(m=>m.BILL_ID==item.BILL_ID && m.CREDIT_DEBIT=="C").Select(s=>s.ADMIN_ND_CODE).FirstOrDefault()).Select(c=>c.ADMIN_ND_NAME).FirstOrDefault() +" ) ":item.ACC_MASTER_TXN.TXN_DESC.Trim(),
                                    //item.ACC_MASTER_TXN.TXN_DESC.Trim(),
                                    item.GROSS_AMOUNT.ToString(),
                                    item.BILL_FINALIZED == "N" ? "<center><a href='#' class='ui-icon ui-icon-pencil' onclick='EditTEOMaster(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()})+ "\");return false;'>Edit</a></center>" :  "<center><span class='ui-icon ui-icon-locked'></span></center>",
                                    item.BILL_FINALIZED == "N" ? "<center><a href='#' class='ui-icon ui-icon-trash' onclick='DeleteTEOMaster(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()})+ "\");return false;'>Delete</a></center>" :  "<center><span class='ui-icon ui-icon-locked'></span></center>"
                   }
                }).ToArray();

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return ex.Message.ToArray();
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        /// <summary>
        /// DAL Function to get the details of the imprest entries
        /// </summary>
        /// <param name="objFilter"></param>
        /// <param name="totalRecords"></param>
        /// <param name="cAmountTotal"></param>
        /// <param name="dAmountTotal"></param>
        /// <param name="GrossAmount"></param>
        /// <returns></returns>
        public Array TEODetailsList(ReceiptFilterModel objFilter, out long totalRecords, out decimal cAmountTotal, out decimal dAmountTotal, out decimal GrossAmount, bool isTransferofBalances = false)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<ACC_BILL_DETAILS> lstBillDetails = null;
                lstBillDetails = dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == objFilter.BillId).ToList<ACC_BILL_DETAILS>();

                if (lstBillDetails.Count() > 0)
                {
                    cAmountTotal = lstBillDetails.Where(m => m.CREDIT_DEBIT == "C").Sum(m => m.AMOUNT);
                    dAmountTotal = lstBillDetails.Where(m => m.CREDIT_DEBIT == "D").Sum(m => m.AMOUNT);
                }
                else
                {
                    cAmountTotal = 0;
                    dAmountTotal = 0;
                }
                ACC_BILL_MASTER acc_bill_master = new ACC_BILL_MASTER();
                acc_bill_master = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == objFilter.BillId).FirstOrDefault();
                string isFinalize = acc_bill_master.BILL_FINALIZED;
                GrossAmount = acc_bill_master.GROSS_AMOUNT;

                totalRecords = lstBillDetails.Count();

                if (objFilter.sidx.Trim() != string.Empty)
                {
                    if (objFilter.sord.ToString() == "asc")
                    {
                        switch (objFilter.sidx)
                        {
                            case "CreditDebit":
                                lstBillDetails = lstBillDetails.OrderBy(x => x.CREDIT_DEBIT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_DETAILS>();
                                break;
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
                            default:
                                lstBillDetails = lstBillDetails.OrderBy(x => x.TXN_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_DETAILS>();
                                break;
                        }

                    }
                    else
                    {
                        switch (objFilter.sidx)
                        {
                            case "CreditDebit":
                                lstBillDetails = lstBillDetails.OrderByDescending(x => x.CREDIT_DEBIT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_DETAILS>();
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

                if (isTransferofBalances == true)
                {
                    return lstBillDetails.Select(item => new
                    {
                        id = URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim() + "$" + item.TXN_NO.ToString().Trim() }),
                        cell = new[] {                         
                                    
                                    item.TXN_NO.ToString(),
                                    item.CREDIT_DEBIT == "C" ? "Credit" : "Debit",
                                    item.ACC_MASTER_HEAD.HEAD_CODE,
                                    "<span class='ui-state-default' style='border:none'>"+item.ACC_MASTER_HEAD.HEAD_CODE.Trim()+"</span> "+item.ACC_MASTER_HEAD.HEAD_NAME.Trim(),
                                    item.MAST_CON_ID == null ? "-" : item.MASTER_CONTRACTOR.MAST_CON_COMPANY_NAME.ToString(),
                                    //old
                                    //item.IMS_AGREEMENT_CODE == null ? "-" : PMGSYSession.Current.FundType.ToLower() == "m" ? dbContext.MANE_IMS_CONTRACT.Where(t=>t.MANE_PR_CONTRACT_CODE == item.IMS_AGREEMENT_CODE).Select(t=>t.MANE_AGREEMENT_NUMBER).FirstOrDefault() : dbContext.TEND_AGREEMENT_MASTER.Where(t=>t.TEND_AGREEMENT_CODE == item.IMS_AGREEMENT_CODE).Select(t=>t.TEND_AGREEMENT_NUMBER).FirstOrDefault(),
                                    //Modified by Abhishek kamble TO get Agr No using MANE_CONTRACTOR_ID for MF 17Nov2014
                                    item.IMS_AGREEMENT_CODE == null ? "-" : PMGSYSession.Current.FundType.ToLower() == "m" ? dbContext.MANE_IMS_CONTRACT.Where(t=>t.MANE_CONTRACT_ID == item.IMS_AGREEMENT_CODE).Select(t=>t.MANE_AGREEMENT_NUMBER).FirstOrDefault() : dbContext.TEND_AGREEMENT_MASTER.Where(t=>t.TEND_AGREEMENT_CODE == item.IMS_AGREEMENT_CODE).Select(t=>t.TEND_AGREEMENT_NUMBER).FirstOrDefault(),
                                    item.IMS_PR_ROAD_CODE == null ? "-" :  item.FINAL_PAYMENT == true ? item.IMS_SANCTIONED_PROJECTS.IMS_ROAD_NAME  + " (Final Payment -Yes)" :
                                     item.IMS_SANCTIONED_PROJECTS.IMS_ROAD_NAME  + " ( Final Payment - No)" ,
                                    item.ADMIN_ND_CODE == null ? "-" : item.ADMIN_DEPARTMENT.ADMIN_ND_NAME, // dbContext.ADMIN_DEPARTMENT.Where(t=>t.ADMIN_ND_CODE == item.ADMIN_ND_CODE).Select(t=>t.ADMIN_ND_NAME).FirstOrDefault(),
                                    item.CREDIT_DEBIT == "C" ? item.AMOUNT.ToString() : "0.00",
                                    item.CREDIT_DEBIT == "D" ? item.AMOUNT.ToString() : "0.00",
                                    item.NARRATION, 
                                    string.Empty,
                                    (isFinalize == "N" && item.CREDIT_DEBIT=="C") ? "<center><a href='#' class='ui-icon ui-icon-trash' onclick='DeleteTEODetailsForTOB(\"" +URLEncrypt.EncryptParameters1(new string[] { "BILL_ID="+ item.BILL_ID.ToString().Trim(), "TXN_NO=" + item.TXN_NO.ToString().Trim() }) +"\");'>Delete TEO Details </a></center>" : "<center><span class='ui-icon ui-icon-locked'></span></center>"  ,
                                    (item.TXN_ID != 0 &&  item.TXN_ID != null) ? dbContext.ACC_MASTER_TXN.Where(x => x.TXN_ID == item.TXN_ID).Select(x => x.IS_OPERATIONAL).FirstOrDefault()==true ? "Correct Entry" :dbContext.ACC_MASTER_TXN.Where(x => x.TXN_ID == item.TXN_ID).Select(x => x.IS_REQ_AFTER_PORTING).FirstOrDefault() ==true ? "Edit And Correct the entry" :"Delete and Make new entry" : "Correct Entry"
                   }
                    }).ToArray();
                }


                return lstBillDetails.Select(item => new
                {
                    id = URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim() + "$" + item.TXN_NO.ToString().Trim() }),
                    cell = new[] {                         
                                    
                                    item.TXN_NO.ToString(),
                                    item.CREDIT_DEBIT == "C" ? "Credit" : "Debit",
                                    item.ACC_MASTER_HEAD.HEAD_CODE,
                                    "<span class='ui-state-default' style='border:none'>"+item.ACC_MASTER_HEAD.HEAD_CODE.Trim()+"</span> "+item.ACC_MASTER_HEAD.HEAD_NAME.Trim(),
                                    item.MAST_CON_ID == null ? "-" : item.MASTER_CONTRACTOR.MAST_CON_COMPANY_NAME.ToString(),
                                    
                                    //old
                                    //item.IMS_AGREEMENT_CODE == null ? "-" : PMGSYSession.Current.FundType.ToLower() == "m" ? dbContext.MANE_IMS_CONTRACT.Where(t=>t.MANE_PR_CONTRACT_CODE == item.IMS_AGREEMENT_CODE).Select(t=>t.MANE_AGREEMENT_NUMBER).FirstOrDefault() : dbContext.TEND_AGREEMENT_MASTER.Where(t=>t.TEND_AGREEMENT_CODE == item.IMS_AGREEMENT_CODE).Select(t=>t.TEND_AGREEMENT_NUMBER).FirstOrDefault(),
                                    //Modified by Abhishek kamble TO get Agr No using MANE_CONTRACTOR_ID for MF 17Nov2014
                                    item.IMS_AGREEMENT_CODE == null ? "-" : PMGSYSession.Current.FundType.ToLower() == "m" ? dbContext.MANE_IMS_CONTRACT.Where(t=>t.MANE_CONTRACT_ID == item.IMS_AGREEMENT_CODE).Select(t=>t.MANE_AGREEMENT_NUMBER).FirstOrDefault() : dbContext.TEND_AGREEMENT_MASTER.Where(t=>t.TEND_AGREEMENT_CODE == item.IMS_AGREEMENT_CODE).Select(t=>t.TEND_AGREEMENT_NUMBER).FirstOrDefault(),

                                    item.IMS_PR_ROAD_CODE == null ? "-" : item.IMS_SANCTIONED_PROJECTS.IMS_ROAD_NAME,
                                    item.ADMIN_ND_CODE == null ? "-" : item.ADMIN_DEPARTMENT.ADMIN_ND_NAME, // dbContext.ADMIN_DEPARTMENT.Where(t=>t.ADMIN_ND_CODE == item.ADMIN_ND_CODE).Select(t=>t.ADMIN_ND_NAME).FirstOrDefault(),
                                    item.CREDIT_DEBIT == "C" ? item.AMOUNT.ToString() : "0.00",
                                    item.CREDIT_DEBIT == "D" ? item.AMOUNT.ToString() : "0.00",
                                    item.NARRATION,
                                    isFinalize == "N" ? "<center><a href='#' class='ui-icon ui-icon-pencil' onclick='EditTEODetails(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim() + "$" + item.TXN_NO.ToString().Trim() })+ "\",\""+item.CREDIT_DEBIT+"\");return false;'>Edit</a></center>" : "<center><span class='ui-icon ui-icon-locked'></span></center>",
                                    isFinalize == "N" ? "<center><a href='#' class='ui-icon ui-icon-trash' onclick='DeleteTEODetails(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim() + "$" + item.TXN_NO.ToString().Trim() })+ "\",\""+item.CREDIT_DEBIT+"\");return false;'>Delete</a></center>" : "<center><span class='ui-icon ui-icon-locked'></span></center>"  ,
                                    (item.TXN_ID != 0 &&  item.TXN_ID != null) ? dbContext.ACC_MASTER_TXN.Where(x => x.TXN_ID == item.TXN_ID).Select(x => x.IS_OPERATIONAL).FirstOrDefault()==true ? "Correct Entry" :dbContext.ACC_MASTER_TXN.Where(x => x.TXN_ID == item.TXN_ID).Select(x => x.IS_REQ_AFTER_PORTING).FirstOrDefault() ==true ? "Edit And Correct the entry" :"Delete and Make new entry" : "Correct Entry"
                   }
                }).ToArray();

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                cAmountTotal = 0;
                dAmountTotal = 0;
                GrossAmount = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// DAL function to list the imprest voucher for settlement
        /// </summary>
        /// <param name="objFilter"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array ImprestMasterList(ReceiptFilterModel objFilter, out long totalRecords)
        {
            totalRecords = 0;
            try
            {
                dbContext = new PMGSYEntities();
                List<ListImprest> lstImprest = null;

                List<string> BilType = new List<string>();
                BilType.Add("P");
                BilType.Add("O");
                List<ListImprest> lstFinal = null;
                List<ListImprest> lstOpeningBalance;
                lstImprest = (from bm in dbContext.ACC_BILL_MASTER
                              join bd in dbContext.ACC_BILL_DETAILS on bm.BILL_ID equals bd.BILL_ID
                              // join cq in dbContext.ACC_CHEQUES_ISSUED on bm.BILL_ID equals cq.BILL_ID into cd //Commented by Abhishek 11Feb2015 for imp settlement list
                              //from bm_ad in cd.DefaultIfEmpty()
                              where bm.ADMIN_ND_CODE == objFilter.AdminNdCode &&
                              bm.LVL_ID == objFilter.LevelId &&
                              bm.FUND_TYPE == PMGSY.Extensions.PMGSYSession.Current.FundType && //added by koustubh Nakate on 09/10/2013 for fund type not considered previously
                                  //(bm.BILL_TYPE == "P" ? bm.TXN_ID : 1) == (bm.BILL_TYPE == "P" ? objFilter.TransId : 1) &&
                                  //new Code modified by Abhishek kamble 11Feb2015                               
                              ((bm.BILL_TYPE == "P" || bm.BILL_TYPE == "O") && bd.HEAD_ID == objFilter.HeadId && (bm.CHQ_EPAY == "Q" || bm.CHQ_EPAY == "O" || bm.CHQ_EPAY == "A" || bm.CHQ_EPAY == "E"))
                              &&
                              bm.BILL_FINALIZED == "Y" &&
                              BilType.Contains(bm.BILL_TYPE)//bm.BILL_TYPE == "P" 
                              && (bm.BILL_TYPE == "O" ? bd.HEAD_ID : 1) == (bm.BILL_TYPE == "O" ? objFilter.HeadId : 1) &&
                                  //bm.BILL_MONTH <= objFilter.Month && bm.BILL_YEAR <= objFilter.Year
                                  //new Code modified by Abhishek kamble 11Feb2015 
                              bm.BILL_MONTH + (bm.BILL_YEAR * 12) <= objFilter.Month + (objFilter.Year * 12)
                              && bd.CREDIT_DEBIT == "D"
                              // &&
                              // (objFilter.LevelId == 4 ? "%" : (bm_ad.CHEQUE_STATUS == null ? "%" : bm_ad.CHEQUE_STATUS)) == (objFilter.LevelId == 4 ? "%" : (bm_ad.CHEQUE_STATUS == null ? "%" : "N"))
                              select new ListImprest
                              {
                                  BILL_ID = bm.BILL_ID,
                                  BILL_NO = bm.BILL_NO,
                                  BILL_DATE = bm.BILL_DATE,
                                  CHQ_DATE = bm.CHQ_DATE,
                                  CHQ_NO = bm.CHQ_NO,
                                  GROSS_AMOUNT = bm.GROSS_AMOUNT,
                                  PAYEE_NAME = bm.BILL_TYPE == "P" ? bm.PAYEE_NAME : bd.NARRATION,
                                  C_SETTLED_AMOUNT = 0,
                                  D_SETTLED_AMOUNT = 0,
                                  P_TXN_ID = bd.TXN_NO,
                                  BILL_TYPE = bm.BILL_TYPE
                                  //C_SETTLED_AMOUNT = dbContext.ACC_BILL_DETAILS.Where(p => p.BILL_ID == bm.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Where(m => m.P_BILL_ID == bm.BILL_ID).Select(m => m.S_BIll_ID).FirstOrDefault() && p.CREDIT_DEBIT == "C").Sum(p => p.AMOUNT),
                                  //D_SETTLED_AMOUNT = dbContext.ACC_BILL_DETAILS.Where(p => p.BILL_ID == bm.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Where(m => m.P_BILL_ID == bm.BILL_ID).Select(m => m.S_BIll_ID).FirstOrDefault() && p.CREDIT_DEBIT == "D").Sum(p => p.AMOUNT)

                              }).Distinct().ToList<ListImprest>();




                //added by Koustubh Nakate on  01/10/2013 for distinct bill Id
                // lstImprest = lstImprest.GroupBy(lst => lst.P_TXN_ID).Select(lst => lst.FirstOrDefault()).ToList<ListImprest>();

                lstFinal = lstImprest.Where(l => l.BILL_TYPE == "P").GroupBy(l => l.BILL_ID).Select(l => l.FirstOrDefault()).ToList<ListImprest>();
                lstOpeningBalance = lstImprest.Where(m => m.BILL_TYPE == "O").ToList<ListImprest>();
                lstFinal = lstFinal.Union(lstOpeningBalance).ToList<ListImprest>();
                //.Union(lstImprest.Where(m=>m.BILL_TYPE=="O").ToList()).ToList<ListImprest>();

                foreach (ListImprest item in lstFinal)
                {
                    List<ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS> lstBillIds = new List<ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS>();
                    Decimal amountSettledMaster = 0;
                    Decimal amountSettledCredit = 0;
                    Decimal amountSettledDebit = 0;
                    Decimal amountSettledByTeo = 0;
                    //if (dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Where(p => p.P_BILL_ID == item.BILL_ID).Any())
                    if (dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Where(p => p.P_BILL_ID == item.BILL_ID && p.P_TXN_ID == item.P_TXN_ID).Any())
                    {

                        // lstBillIds = dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Where(p => p.P_BILL_ID == item.BILL_ID /*&& p.P_TXN_ID == item.P_TXN_ID*/).ToList<ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS>();

                        //changes by Koustubh Nakate to get setteled amount only for finalized transaction 
                        lstBillIds = (from billMaster in dbContext.ACC_BILL_MASTER
                                      join mappedImprest in dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS
                                      on billMaster.BILL_ID equals mappedImprest.S_BIll_ID
                                      where
                                      mappedImprest.P_BILL_ID == item.BILL_ID &&
                                      mappedImprest.P_TXN_ID == item.P_TXN_ID &&
                                      billMaster.BILL_FINALIZED == "Y"
                                      select mappedImprest).ToList<ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS>();

                    }

                    item.AddNewMaster = true;

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

                            }
                            else
                            {
                                amountSettledMaster += (from c in dbContext.ACC_BILL_DETAILS where imprestItem.S_BIll_ID == c.BILL_ID && c.TXN_NO == (imprestItem.S_TXN_ID) select c.AMOUNT).FirstOrDefault();
                                //if(dbContext.ACC_BILL_DETAILS.Where(m=>m.BILL_ID))                            
                                if ((from c in dbContext.ACC_BILL_DETAILS where imprestItem.S_BIll_ID == c.BILL_ID && c.TXN_NO == imprestItem.S_TXN_ID select c).Any())
                                {
                                    amountSettledCredit += (from c in dbContext.ACC_BILL_DETAILS where imprestItem.S_BIll_ID == c.BILL_ID && c.TXN_NO == (imprestItem.S_TXN_ID) select c.AMOUNT).FirstOrDefault();
                                }
                                if ((from c in dbContext.ACC_BILL_DETAILS where imprestItem.S_BIll_ID == c.BILL_ID && c.TXN_NO == (imprestItem.S_TXN_ID) select c).Any())
                                {
                                    amountSettledDebit += (from c in dbContext.ACC_BILL_DETAILS where imprestItem.S_BIll_ID == c.BILL_ID && c.TXN_NO == (imprestItem.S_TXN_ID) select c.AMOUNT).FirstOrDefault();
                                }
                                item.S_BIll_ID = imprestItem.S_BIll_ID;
                            }

                            //check whether to show plus icon to add new master imprest settlement entry in list 
                            //if total gross maount equals master amount to settle
                            item.AddNewMaster = true;
                            if (item.BILL_TYPE == "O")
                            {
                                decimal GROSS_AMOUNT = dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == item.BILL_ID && m.TXN_NO == item.P_TXN_ID).Select(m => m.AMOUNT).FirstOrDefault();
                                if (GROSS_AMOUNT == amountSettledMaster)
                                {
                                    item.AddNewMaster = false;
                                }

                            }
                            else if (item.GROSS_AMOUNT == amountSettledMaster)
                            {
                                item.AddNewMaster = false;
                            }
                            else
                            {
                                //get all settlement bill id and check if they are finalized
                                List<long> sbillList = new List<long>();
                                sbillList = dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Where(x => x.P_BILL_ID == item.BILL_ID).Select(c => c.S_BIll_ID).ToList<long>();
                                if (sbillList != null && sbillList.Count != 0)
                                {
                                    foreach (long bill in sbillList)
                                    {
                                        if (dbContext.ACC_BILL_MASTER.Where(c => c.BILL_ID == bill).Select(c => c.BILL_FINALIZED).First() == "N")
                                        {
                                            item.AddNewMaster = false;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    item.AddNewMaster = true;
                                }

                            }


                        }
                    }

                    item.MASTER_SETTLED_AMOUNT = amountSettledMaster;
                    item.C_SETTLED_AMOUNT = amountSettledCredit;
                    item.D_SETTLED_AMOUNT = amountSettledDebit;
                    item.Amount_Settled_By_TEO = amountSettledByTeo;

                    if (item.BILL_TYPE == "O")
                    {
                        item.GROSS_AMOUNT = dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == item.BILL_ID && m.TXN_NO == item.P_TXN_ID).Select(m => m.AMOUNT).FirstOrDefault();
                    }
                    //item.IS_FINALIZE = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == item.S_BIll_ID).Select(m => m.BILL_FINALIZED).FirstOrDefault();
                    //item.IS_FINALIZE = "N";
                }

                totalRecords = lstFinal.Count;

                if (objFilter.sidx.Trim() != string.Empty)
                {
                    if (objFilter.sord.ToString() == "asc")
                    {

                        switch (objFilter.sidx.Trim())
                        {
                            case "VoucherDate":
                                //lstFinal = lstFinal.OrderBy(x => x.BILL_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(Convert.ToInt16(objFilter.rows)).ToList();
                                lstFinal = lstFinal.OrderBy(x => x.BILL_DATE).ThenBy(t => t.BILL_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(Convert.ToInt16(objFilter.rows)).ToList();
                                break;

                            case "VoucherNumber":
                                lstFinal = lstFinal.OrderBy(x => x.BILL_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(Convert.ToInt16(objFilter.rows)).ToList();
                                break;

                            case "ChequeNo":
                                lstFinal = lstFinal.OrderBy(x => x.CHQ_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(Convert.ToInt16(objFilter.rows)).ToList();
                                break;

                            case "ChequeDate":
                                lstFinal = lstFinal.OrderBy(x => x.CHQ_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(Convert.ToInt16(objFilter.rows)).ToList();
                                break;

                            case "TEOAmount":
                                lstFinal = lstFinal.OrderBy(x => x.Amount_Settled_By_TEO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(Convert.ToInt16(objFilter.rows)).ToList();
                                break;

                            default:
                                lstFinal = lstFinal.OrderBy(x => x.BILL_DATE).ThenBy(t => t.BILL_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(Convert.ToInt16(objFilter.rows)).ToList();
                                break;
                        }



                    }
                    else
                    {


                        switch (objFilter.sidx.Trim())
                        {
                            case "VoucherDate":
                                //lstFinal = lstFinal.OrderByDescending(x => x.BILL_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(Convert.ToInt16(objFilter.rows)).ToList();
                                lstFinal = lstFinal.OrderByDescending(x => x.BILL_DATE).ThenByDescending(t => t.BILL_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(Convert.ToInt16(objFilter.rows)).ToList();
                                break;

                            case "VoucherNumber":
                                lstFinal = lstFinal.OrderByDescending(x => x.BILL_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(Convert.ToInt16(objFilter.rows)).ToList();
                                break;

                            case "ChequeNo":
                                lstFinal = lstFinal.OrderByDescending(x => x.CHQ_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(Convert.ToInt16(objFilter.rows)).ToList();
                                break;

                            case "ChequeDate":
                                lstFinal = lstFinal.OrderByDescending(x => x.CHQ_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(Convert.ToInt16(objFilter.rows)).ToList();
                                break;

                            case "TEOAmount":
                                lstFinal = lstFinal.OrderByDescending(x => x.Amount_Settled_By_TEO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(Convert.ToInt16(objFilter.rows)).ToList();
                                break;

                            default:
                                lstFinal = lstFinal.OrderByDescending(x => x.BILL_DATE).ThenByDescending(t => t.BILL_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(Convert.ToInt16(objFilter.rows)).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    //lstFinal = lstFinal.OrderBy(x => x.BILL_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(Convert.ToInt16(objFilter.rows)).ToList();
                    lstFinal = lstFinal.OrderBy(x => x.BILL_DATE).ThenBy(t => t.BILL_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(Convert.ToInt16(objFilter.rows)).ToList();
                }



                return lstFinal.Select(item => new
                {
                    //id = item.BILL_ID.ToString().Trim(),//+"$"+item.P_TXN_ID,
                    id = item.BILL_ID.ToString().Trim() + "_" + item.P_TXN_ID,
                    cell = new[] {                         
                                    
                                    item.BILL_NO,
                                    Convert.ToDateTime(item.BILL_DATE).ToString("dd/MM/yyyy"),
                                    item.CHQ_NO,
                                    item.CHQ_DATE == null ? "" : Convert.ToDateTime(item.CHQ_DATE).ToString("dd/MM/yyyy"),
                                    item.PAYEE_NAME,
                                    item.GROSS_AMOUNT.ToString(),
                                    item.Amount_Settled_By_TEO.ToString(),//amount settled by TEO
                                    (item.D_SETTLED_AMOUNT - item.Amount_Settled_By_TEO).ToString(),//amount settled by receipt
                                    item.D_SETTLED_AMOUNT.ToString(),//total amount settled

                                    //old
                                    //"<center><div style='width:54px;height:10px' class='ui-corner-all'><div title='Rs."+(item.GROSS_AMOUNT-item.C_SETTLED_AMOUNT)+" Remaining' style='width:25px;height:100%; float:left; background-color:#ffffff; border:0.1em solid gray; text-align:left' class='ui-corner-left'><div title='Rs."+(item.C_SETTLED_AMOUNT)+" Credited' style='width:"+(((item.C_SETTLED_AMOUNT/item.GROSS_AMOUNT)*100)/4)+"px;height:100%; background-color:#4eb305;'></div></div><div title='Rs."+(item.GROSS_AMOUNT-item.D_SETTLED_AMOUNT)+" Remaining' style='width:25px;height:100%; float:right; background-color:#ffffff; border:0.1em solid gray; text-align:right' class='ui-corner-right'><div title='Rs."+(item.D_SETTLED_AMOUNT)+" Debited' style='width:"+(((item.D_SETTLED_AMOUNT/item.GROSS_AMOUNT)*100)/4)+"px;height:100%; background-color:#ce1212; float:right'></div></div></div></center>",
                                    //Modified By Abhishek kamble 16-oct-2014
                                    "<center><div style='width:54px;height:10px' class='ui-corner-all'><div title='Rs."+(item.GROSS_AMOUNT-item.C_SETTLED_AMOUNT)+" Remaining' style='width:25px;height:100%; float:left; background-color:#ffffff; border:0.1em solid gray; text-align:left' class='ui-corner-left'><div title='Rs."+(item.C_SETTLED_AMOUNT)+" Credited' style='width:"+( item.GROSS_AMOUNT==0?0:(((item.C_SETTLED_AMOUNT/item.GROSS_AMOUNT)*100)/4) )+"px;height:100%; background-color:#4eb305;'></div></div><div title='Rs."+(item.GROSS_AMOUNT-item.D_SETTLED_AMOUNT)+" Remaining' style='width:25px;height:100%; float:right; background-color:#ffffff; border:0.1em solid gray; text-align:right' class='ui-corner-right'><div title='Rs."+(item.D_SETTLED_AMOUNT)+" Debited' style='width:"+(item.GROSS_AMOUNT==0?0: (((item.D_SETTLED_AMOUNT/item.GROSS_AMOUNT)*100)/4) )+"px;height:100%; background-color:#ce1212; float:right'></div></div></div></center>",

                                                                                                                                                                                                                                                                                                                                                                                       
                                    /*
                                    item.IS_FINALIZE == "Y" && (item.GROSS_AMOUNT == item.D_SETTLED_AMOUNT) 
                                    ? "<center><a href='#' class='ui-icon ui-icon-search' onclick='ViewTEO(\"" +URLEncrypt.EncryptParameters(new string[] { item.S_BIll_ID.ToString().Trim()})+ "\");return false;'>View</a></center>" 
                                    : item.IS_FINALIZE == "Y" && (item.GROSS_AMOUNT != item.D_SETTLED_AMOUNT) 
                                    ? "<center><a href='#' class='ui-icon ui-icon-pencil' onclick='AddImprestMaster(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()}) +"\");return false;'>Add Master</a></center>" 
                                    : (item.C_SETTLED_AMOUNT+item.D_SETTLED_AMOUNT) == 0 && item.S_BIll_ID == 0 
                                    ? "<center><a href='#' class='ui-icon ui-icon-pencil' onclick='AddImprestMaster(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()}) +"\");return false;'>Add Master</a></center>" 
                                    : "<center><a href='#' class='ui-icon ui-icon-pencil' onclick='AddImprestDetails(\"" +URLEncrypt.EncryptParameters(new string[] { item.S_BIll_ID.ToString().Trim()}) +"\");return false;'>Add Details</a></center>"
                                     */
                                    item.AddNewMaster
                                    ? "<center><a href='#' class='ui-icon ui-icon-circle-plus' onclick='AddImprestMaster(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()}) +"\",\""+item.P_TXN_ID.ToString().Trim()+"\");return false;'>Add Master</a></center>" 
                                    : String.Empty//"<center><a href='#' class='ui-icon ui-icon-pencil' onclick='AddImprestDetails(\"" +URLEncrypt.EncryptParameters(new string[] { item.S_BIll_ID.ToString().Trim()}) +"\");return false;'>Add Details</a></center>"
                                  , URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim() + "$" + item.P_TXN_ID.ToString().Trim() }),
                    
                                   
                    
                    
                    }
                }).ToArray();

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return ex.Message.ToArray();
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        /// <summary>
        /// DAL function to return the settlement master entry details against the imprest  
        /// </summary>
        /// <param name="objFilter"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array ImprestSettlementMasterList(ReceiptFilterModel objFilter, out long totalRecords)
        {
            totalRecords = 0;
            try
            {
                dbContext = new PMGSYEntities();
                List<ListImprest> lstImprest = null;

                /*get the all teo voucher against the imprest using ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS 
                  here we have the actual payment/opening balnace entry bill id and we ahve to find out the settlement vouchers against it               
                 */

                List<ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS> settlementVoucherList = new List<ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS>();

                //settlementVoucherList = dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Where(x => x.P_BILL_ID == objFilter.BillId).ToList<ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS>();
                settlementVoucherList = dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Where(x => x.P_BILL_ID == objFilter.BillId && x.P_TXN_ID == objFilter.TransId).ToList<ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS>();//new change done by Vikram

                List<long> settlementBillId = new List<long>();
                settlementBillId = settlementVoucherList.Select(x => x.S_BIll_ID).ToList<long>();

                lstImprest = (from bm in dbContext.ACC_BILL_MASTER

                              where settlementBillId.Contains(bm.BILL_ID)
                              && bm.BILL_TYPE == "J"

                              select new ListImprest
                              {
                                  BILL_ID = bm.BILL_ID,
                                  BILL_NO = bm.BILL_NO,
                                  BILL_DATE = bm.BILL_DATE,
                                  CHQ_DATE = bm.CHQ_DATE,
                                  CHQ_NO = bm.CHQ_NO,
                                  GROSS_AMOUNT = bm.GROSS_AMOUNT,
                                  C_SETTLED_AMOUNT = 0,
                                  D_SETTLED_AMOUNT = 0,
                                  BILL_TYPE = bm.BILL_TYPE,
                                  IS_FINALIZE = bm.BILL_FINALIZED,

                              }).ToList<ListImprest>();


                foreach (ListImprest item in lstImprest)
                {
                    List<ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS> lstBillIds = new List<ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS>();
                    Decimal amountSettledMaster = 0;
                    Decimal amountSettledCredit = 0;
                    Decimal amountSettledDebit = 0;

                    //get the transaction id from list
                    item.P_TXN_ID = settlementVoucherList.Where(x => x.S_BIll_ID == item.BILL_ID).Select(s => s.P_TXN_ID).FirstOrDefault();

                    amountSettledMaster += (from c in dbContext.ACC_BILL_DETAILS where item.BILL_ID == c.BILL_ID && c.TXN_NO == (item.P_TXN_ID) select c.AMOUNT).FirstOrDefault();

                    //if TEO entry details   

                    if ((from c in dbContext.ACC_BILL_DETAILS where item.BILL_ID == c.BILL_ID && c.CREDIT_DEBIT == "C" select c).Any())
                    {
                        amountSettledCredit += (from c in dbContext.ACC_BILL_DETAILS where item.BILL_ID == c.BILL_ID && c.CREDIT_DEBIT == "C" select c.AMOUNT).Sum();
                    }
                    if ((from c in dbContext.ACC_BILL_DETAILS where item.BILL_ID == c.BILL_ID && c.CREDIT_DEBIT == "D" select c).Any())
                    {
                        amountSettledDebit += (from c in dbContext.ACC_BILL_DETAILS where item.BILL_ID == c.BILL_ID && c.CREDIT_DEBIT == "D" select c.AMOUNT).Sum();
                    }


                    item.MASTER_SETTLED_AMOUNT = amountSettledMaster;
                    item.C_SETTLED_AMOUNT = amountSettledCredit;
                    item.D_SETTLED_AMOUNT = amountSettledDebit;

                    if (item.BILL_TYPE == "O")
                    {
                        item.GROSS_AMOUNT = dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == item.BILL_ID && m.TXN_NO == item.P_TXN_ID).Select(m => m.AMOUNT).FirstOrDefault();
                    }
                    //item.IS_FINALIZE = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == item.BILL_ID).Select(m => m.BILL_FINALIZED).FirstOrDefault();
                    //item.IS_FINALIZE = "N";
                }

                totalRecords = lstImprest.Count;

                return lstImprest.Select(item => new
                {
                    id = item.BILL_ID.ToString().Trim() + "$" + item.P_TXN_ID,
                    cell = new[] {                         
                                    
                                    item.BILL_NO,
                                    Convert.ToDateTime(item.BILL_DATE).ToString("dd/MM/yyyy"),
                                    item.GROSS_AMOUNT.ToString(),
                                    item.D_SETTLED_AMOUNT.ToString(),
                                    "<center><div style='width:54px;height:10px' class='ui-corner-all'><div title='Rs."+(item.GROSS_AMOUNT-item.C_SETTLED_AMOUNT)+" Remaining' style='width:25px;height:100%; float:left; background-color:#ffffff; border:0.1em solid gray; text-align:left' class='ui-corner-left'><div title='Rs."+(item.C_SETTLED_AMOUNT)+" Credited' style='width:"+(((item.C_SETTLED_AMOUNT/item.GROSS_AMOUNT)*100)/4)+"px;height:100%; background-color:#4eb305;'></div></div><div title='Rs."+(item.GROSS_AMOUNT-item.D_SETTLED_AMOUNT)+" Remaining' style='width:25px;height:100%; float:right; background-color:#ffffff; border:0.1em solid gray; text-align:right' class='ui-corner-right'><div title='Rs."+(item.D_SETTLED_AMOUNT)+" Debited' style='width:"+(((item.D_SETTLED_AMOUNT/item.GROSS_AMOUNT)*100)/4)+"px;height:100%; background-color:#ce1212; float:right'></div></div></div></center>",
                                     item.IS_FINALIZE == "N" ? "<center><a href='#' class='ui-icon ui-icon-pencil' onclick='AddImprestDetails(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()  }) +"\");return false;'>Add Details</a></center>":  "<center><span class='ui-icon ui-icon-search'onclick='AddImprestDetails(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()}) +"\");return false;'>View Details</span></center>",
                                   // item.IS_FINALIZE == "N" ? "<center><a href='#' class='ui-icon ui-icon-trash' onclick='DeleteTEO(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()})+ "\");return false;'>Delete</a></center>" :   "<center><span class='ui-icon ui-icon-search'onclick='AddImprestDetails(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()}) +"\");return false;'>View Details</span></center>"

                                   //chenges by Koustubh Nakate on 01/010/2013 for when finalized delete option not required
                                    item.IS_FINALIZE == "N" ? "<center><a href='#' class='ui-icon ui-icon-trash' onclick='DeleteTEO(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()})+ "\");return false;'>Delete</a></center>" :   "<center><span class='ui-icon ui-icon-locked'>Locked</span></center>"
                   }
                }).ToArray();

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return ex.Message.ToArray();
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public TeoMasterModel GetTEOMaster(Int64 billId)
        {
            try
            {
                dbContext = new PMGSYEntities();
                ACC_BILL_MASTER acc_bill_master = new ACC_BILL_MASTER();
                acc_bill_master = dbContext.ACC_BILL_MASTER.Find(billId);
                return CloneMasterObject(acc_bill_master);
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw Ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public TeoMasterModel CloneMasterObject(ACC_BILL_MASTER acc_bill_master)
        {
            try
            {
                dbContext = new PMGSYEntities();
                commonFuncObj = new CommonFunctions();
                TeoMasterModel teoMasterModel = new TeoMasterModel();
                teoMasterModel.BILL_ID = acc_bill_master.BILL_ID;
                teoMasterModel.BILL_NO = acc_bill_master.BILL_NO;
                teoMasterModel.BILL_DATE = commonFuncObj.GetDateTimeToString(acc_bill_master.BILL_DATE);
                teoMasterModel.BILL_MONTH = (byte)acc_bill_master.BILL_DATE.Month;
                teoMasterModel.BILL_YEAR = (short)acc_bill_master.BILL_DATE.Year;
                teoMasterModel.CHQ_NO = acc_bill_master.CHQ_NO;
                //teoMasterModel.CHQ_DATE = acc_bill_master.CHQ_DATE == null ? "" : Convert.ToDateTime(acc_bill_master.CHQ_DATE).ToString("dd/MM/yyyy");
                teoMasterModel.CHQ_AMOUNT = acc_bill_master.CHQ_AMOUNT;
                teoMasterModel.CASH_AMOUNT = acc_bill_master.CASH_AMOUNT;
                teoMasterModel.GROSS_AMOUNT = acc_bill_master.GROSS_AMOUNT;
                teoMasterModel.CHQ_EPAY = acc_bill_master.CHQ_EPAY;
                teoMasterModel.BILL_FINALIZED = acc_bill_master.BILL_FINALIZED;
                teoMasterModel.ADMIN_ND_CODE = acc_bill_master.ADMIN_ND_CODE;
                teoMasterModel.FUND_TYPE = acc_bill_master.FUND_TYPE;
                teoMasterModel.LVL_ID = acc_bill_master.LVL_ID;
                teoMasterModel.BILL_TYPE = acc_bill_master.BILL_TYPE;
                Int16? mainTrans = dbContext.ACC_MASTER_TXN.Where(m => m.TXN_ID == acc_bill_master.TXN_ID).Select(m => m.TXN_PARENT_ID).FirstOrDefault();
                if (mainTrans == null)
                {
                    teoMasterModel.TXN_ID = acc_bill_master.TXN_ID;
                    teoMasterModel.SUB_TXN_ID = 0;
                }
                else
                {
                    teoMasterModel.TXN_ID = Convert.ToInt16(mainTrans);
                    teoMasterModel.SUB_TXN_ID = acc_bill_master.TXN_ID;
                }
                return teoMasterModel;
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw Ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public ACC_BILL_MASTER CloneMasterModel(TeoMasterModel teoMasterModel)
        {
            try
            {
                commonFuncObj = new CommonFunctions();
                ACC_BILL_MASTER acc_bill_master = new ACC_BILL_MASTER();
                acc_bill_master.BILL_NO = teoMasterModel.BILL_NO;
                acc_bill_master.BILL_DATE = commonFuncObj.GetStringToDateTime(teoMasterModel.BILL_DATE);
                acc_bill_master.BILL_MONTH = teoMasterModel.BILL_MONTH;
                acc_bill_master.BILL_YEAR = teoMasterModel.BILL_YEAR;
                if (teoMasterModel.SUB_TXN_ID != 0 && teoMasterModel.SUB_TXN_ID != null)
                {
                    acc_bill_master.TXN_ID = teoMasterModel.SUB_TXN_ID;
                }
                else
                {
                    acc_bill_master.TXN_ID = teoMasterModel.TXN_ID;
                }
                acc_bill_master.CHQ_NO = teoMasterModel.CHQ_NO;
                acc_bill_master.CHQ_EPAY = teoMasterModel.CHQ_EPAY;
                acc_bill_master.GROSS_AMOUNT = Convert.ToDecimal(teoMasterModel.GROSS_AMOUNT);
                acc_bill_master.TEO_TRANSFER_TYPE = null;
                acc_bill_master.BILL_FINALIZED = "N";
                acc_bill_master.ADMIN_ND_CODE = teoMasterModel.ADMIN_ND_CODE;
                acc_bill_master.FUND_TYPE = teoMasterModel.FUND_TYPE;
                acc_bill_master.LVL_ID = (byte)teoMasterModel.LVL_ID;
                acc_bill_master.BILL_TYPE = "J";

                //Added By Abhishek Kamble 29-nov-2013
                acc_bill_master.USERID = PMGSYSession.Current.UserId;
                acc_bill_master.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                return acc_bill_master;
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw Ex;
            }
        }

        public ACC_BILL_DETAILS CloneDetailsModel(TeoDetailsModel teoDetailsModel)
        {
            try
            {
                ACC_BILL_DETAILS acc_bill_Details = new ACC_BILL_DETAILS();
                acc_bill_Details.BILL_ID = teoDetailsModel.BILL_ID;
                acc_bill_Details.CASH_CHQ = teoDetailsModel.CASH_CHQ;
                acc_bill_Details.AMOUNT = Convert.ToDecimal(teoDetailsModel.AMOUNT);
                acc_bill_Details.NARRATION = teoDetailsModel.NARRATION;
                acc_bill_Details.TXN_NO = teoDetailsModel.TXN_NO;
                acc_bill_Details.HEAD_ID = teoDetailsModel.HEAD_ID;
                acc_bill_Details.CREDIT_DEBIT = teoDetailsModel.CREDIT_DEBIT;
                acc_bill_Details.MAST_DISTRICT_CODE = teoDetailsModel.MAST_DISTRICT_CODE;
                acc_bill_Details.ADMIN_ND_CODE = teoDetailsModel.ADMIN_ND_CODE;
                acc_bill_Details.MAST_CON_ID = teoDetailsModel.MAST_CON_ID;
                acc_bill_Details.IMS_AGREEMENT_CODE = teoDetailsModel.IMS_AGREEMENT_CODE;
                acc_bill_Details.IMS_PR_ROAD_CODE = teoDetailsModel.IMS_PR_ROAD_CODE;
                if (teoDetailsModel.FINAL_PAYMENT == false)
                {
                    acc_bill_Details.FINAL_PAYMENT = null;
                }
                else
                {
                    acc_bill_Details.FINAL_PAYMENT = true;
                }
                acc_bill_Details.MAS_FA_CODE = teoDetailsModel.MAS_FA_CODE;

                //Added By Abhishek Kamble 29-nov-2013
                acc_bill_Details.USERID = PMGSYSession.Current.UserId;
                acc_bill_Details.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                return acc_bill_Details;
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw Ex;
            }
        }

        public TeoDetailsModel CloneDetailsObject(ACC_BILL_DETAILS acc_bill_details)
        {
            try
            {
                TeoDetailsModel teoDetailsModel = new TeoDetailsModel();
                teoDetailsModel.BILL_ID = acc_bill_details.BILL_ID;
                teoDetailsModel.TXN_NO = acc_bill_details.TXN_NO;
                teoDetailsModel.HEAD_ID = acc_bill_details.HEAD_ID;
                teoDetailsModel.ADMIN_ND_CODE = acc_bill_details.ADMIN_ND_CODE;
                teoDetailsModel.MAST_CON_ID = acc_bill_details.MAST_CON_ID;
                teoDetailsModel.IMS_AGREEMENT_CODE = acc_bill_details.IMS_AGREEMENT_CODE;
                teoDetailsModel.IMS_PR_ROAD_CODE = acc_bill_details.IMS_PR_ROAD_CODE;
                teoDetailsModel.MAST_DISTRICT_CODE = acc_bill_details.MAST_DISTRICT_CODE;
                teoDetailsModel.AMOUNT = acc_bill_details.AMOUNT;
                teoDetailsModel.NARRATION = acc_bill_details.NARRATION;
                teoDetailsModel.CREDIT_DEBIT = acc_bill_details.CREDIT_DEBIT;
                teoDetailsModel.FINAL_PAYMENT = acc_bill_details.FINAL_PAYMENT == null ? false : Convert.ToBoolean(acc_bill_details.FINAL_PAYMENT);
                commonFuncObj = new CommonFunctions();
                ACC_TEO_SCREEN_DESIGN_PARAM_DETAILS designparams = new ACC_TEO_SCREEN_DESIGN_PARAM_DETAILS();
                ACC_TEO_SCREEN_DESIGN_HEADWISE_DETAILS headDesignParams = new ACC_TEO_SCREEN_DESIGN_HEADWISE_DETAILS();
                TransactionParams objParam = new TransactionParams();
                objParam.BILL_ID = acc_bill_details.BILL_ID;
                objParam.HEAD_ID = acc_bill_details.HEAD_ID;
                designparams = commonFuncObj.getTEODesignParamDetails(acc_bill_details.BILL_ID, 0);
                if (designparams == null)
                {
                    designparams = setDefualtDesignTransParam();
                }
                headDesignParams = commonFuncObj.GetHeadwiseDesignParams(objParam);
                if (headDesignParams == null)
                {
                    headDesignParams = setDefualtDesignHeadParam();
                }
                if (designparams.SAN_REQ == "Y" || headDesignParams.SANC_YEAR_REQ == "Y")
                {
                    teoDetailsModel.SANC_YEAR = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == acc_bill_details.IMS_PR_ROAD_CODE).Select(m => m.IMS_YEAR).FirstOrDefault();
                }
                else
                {
                    teoDetailsModel.SANC_YEAR = null;
                }
                if (designparams.PKG_REQ == "Y" || headDesignParams.PKG_REQ == "Y")
                {
                    teoDetailsModel.IMS_PACKAGE_ID = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == acc_bill_details.IMS_PR_ROAD_CODE).Select(m => m.IMS_PACKAGE_ID).FirstOrDefault();
                }
                else
                {
                    teoDetailsModel.IMS_PACKAGE_ID = null;
                }
                return teoDetailsModel;
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw Ex;
            }
        }

        public String EditTEOMaster(TeoMasterModel teoMasterModel)
        {
            try
            {
                dbContext = new PMGSYEntities();
                ACC_BILL_MASTER exist_acc_bill_master = new ACC_BILL_MASTER();
                exist_acc_bill_master = dbContext.ACC_BILL_MASTER.Find(teoMasterModel.BILL_ID);
                teoMasterModel.FUND_TYPE = exist_acc_bill_master.FUND_TYPE;
                teoMasterModel.LVL_ID = exist_acc_bill_master.LVL_ID;
                teoMasterModel.ADMIN_ND_CODE = exist_acc_bill_master.ADMIN_ND_CODE;
                ACC_BILL_MASTER acc_bill_master = CloneMasterModel(teoMasterModel);
                acc_bill_master.BILL_ID = teoMasterModel.BILL_ID;
                dbContext.Entry(exist_acc_bill_master).CurrentValues.SetValues(acc_bill_master);
                dbContext.SaveChanges();
                return "";
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
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                return Ex.Message;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public String DeleteTEO(Int64 billId)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    dbContext = new PMGSYEntities();
                    List<ACC_BILL_DETAILS> lstBillDetails = dbContext.ACC_BILL_DETAILS.Where(w => w.BILL_ID == billId).ToList<ACC_BILL_DETAILS>();
                    foreach (ACC_BILL_DETAILS item in lstBillDetails)
                    {
                        //added by abhishek kamble 29-nov-2013
                        item.USERID = PMGSYSession.Current.UserId;
                        item.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        dbContext.Entry(item).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();

                        dbContext.ACC_BILL_DETAILS.Remove(item);
                    }
                    dbContext.SaveChanges();

                    ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS mapDetails = dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Where(m => m.S_BIll_ID == billId).FirstOrDefault();
                    if (mapDetails != null)
                    {
                        mapDetails.USERID = PMGSYSession.Current.UserId;
                        mapDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        dbContext.Entry(mapDetails).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();

                        dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Remove(mapDetails);
                        dbContext.SaveChanges();
                    }

                    ACC_NOTIFICATION_DETAILS accNitificationDetails = dbContext.ACC_NOTIFICATION_DETAILS.Where(m => m.INITIATION_BILL_ID == billId).FirstOrDefault();
                    if (accNitificationDetails != null)
                    {
                        accNitificationDetails.USERID = PMGSYSession.Current.UserId;
                        accNitificationDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        dbContext.Entry(accNitificationDetails).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                    }
                    //added by koustubh nakate on 21/08/2013 to delete details from ACC_NOTIFICATION_DETAILS             
                    dbContext.Database.ExecuteSqlCommand
                       ("DELETE [omms].ACC_NOTIFICATION_DETAILS Where INITIATION_BILL_ID = {0}", billId);

                    ACC_BILL_MASTER acc_bill_master = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == billId).FirstOrDefault();

                    //added by abhishek kamble 29-nov-2013
                    acc_bill_master.USERID = PMGSYSession.Current.UserId;
                    acc_bill_master.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    dbContext.Entry(acc_bill_master).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    dbContext.ACC_BILL_MASTER.Remove(acc_bill_master);
                    dbContext.SaveChanges();
                }
                catch (TransactionException ex)
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

        public String DeleteTEODetails(Int64 billId, Int16 TransNo)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    dbContext = new PMGSYEntities();
                    ACC_BILL_DETAILS acc_bill_details = new ACC_BILL_DETAILS();
                    acc_bill_details = dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == billId && m.TXN_NO == TransNo).FirstOrDefault();

                    //added by abhishek kamble 29-nov-2013
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
                finally
                {
                    scope.Complete();
                    dbContext.Dispose();
                }
            }
            return String.Empty;
        }

        public String FinalizeTEO(Int64 billId)
        {
            string fundType = PMGSYSession.Current.FundType;
            int adminNdCode = PMGSYSession.Current.AdminNdCode;
            short levelID = PMGSYSession.Current.LevelId;

            using (var scope = new TransactionScope())
            {
                try
                {
                    dbContext = new PMGSYEntities();

                    //get the txn id for validation first 
                    int txnId = 0;

                    txnId = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == billId).Select(x => x.TXN_ID).First();

                    if (txnId == 164 || txnId == 1194)
                    {
                        if (!ValidateRoadForTransferBetweenDistricts(billId))
                        {
                            scope.Complete();

                            return "Cannot Finalize, ";    //please complete the error message for user
                        }

                    }

                    decimal? cAmount = dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == billId && m.CREDIT_DEBIT == "C").Sum(m => (decimal?)m.AMOUNT);
                    decimal? dAmount = dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == billId && m.CREDIT_DEBIT == "D").Sum(m => (decimal?)m.AMOUNT);
                    decimal? grossAmount = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == billId).Sum(m => (decimal?)m.GROSS_AMOUNT);
                    cAmount = cAmount == null ? 0 : cAmount;
                    dAmount = dAmount == null ? 0 : dAmount;
                    grossAmount = grossAmount == null ? 0 : grossAmount;
                    if (grossAmount == cAmount && grossAmount == dAmount && grossAmount != 0)
                    {
                        ACC_BILL_MASTER acc_bill_master = dbContext.ACC_BILL_MASTER.Find(billId);
                        acc_bill_master.BILL_FINALIZED = "Y";
                        acc_bill_master.ACTION_REQUIRED = "N";

                        //Added by abhishek kamble 29-nov-2013
                        acc_bill_master.USERID = PMGSYSession.Current.UserId;
                        acc_bill_master.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                        dbContext.Entry(acc_bill_master).State = System.Data.Entity.EntityState.Modified;

                        dbContext.SaveChanges();

                        //added by Koustubh Nakate on 21/08/2013 to save notification in notification details table 
                        //var result = dbContext.USP_ACC_INSERT_ALERT_DETAILS(adminNdCode, fundType, "J", levelID, billId, PMGSYSession.Current.UserId, HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]);

                        scope.Complete();
                        return String.Empty;
                    }
                    else
                    {
                        scope.Complete();

                        return "Cannot Finalize, Master Amount and Details Amount Does not Match.";
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
                catch (Exception Ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                    return Ex.Message;
                }
                finally
                {
                    dbContext.Dispose();
                }
            }
        }

        /// <summary>
        /// function to validate the roads 
        /// </summary>
        /// <returns></returns>
        public bool ValidateRoadForTransferBetweenDistricts(long billid)
        {
            return true;
        }



        public String IsTEOFinalized(Int64 billId)
        {
            try
            {
                dbContext = new PMGSYEntities();
                return dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == billId).Select(m => m.BILL_FINALIZED).FirstOrDefault();
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw Ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Int16 GetMasterTransId(Int64 billId)
        {
            try
            {
                dbContext = new PMGSYEntities();
                return dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == billId).Select(m => m.TXN_ID).FirstOrDefault();
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw Ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public TeoDetailsModel GetTEODetailByTransNo(Int64 billId, Int16 transId)
        {
            try
            {
                dbContext = new PMGSYEntities();
                ACC_BILL_DETAILS acc_bill_details = new ACC_BILL_DETAILS();
                TeoDetailsModel teoDetailsModel = new TeoDetailsModel();
                acc_bill_details = dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == billId && m.TXN_NO == transId).FirstOrDefault();
                teoDetailsModel = CloneDetailsObject(acc_bill_details);
                return teoDetailsModel;
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw Ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public ACC_TEO_SCREEN_DESIGN_PARAM_DETAILS setDefualtDesignTransParam()
        {
            ACC_TEO_SCREEN_DESIGN_PARAM_DETAILS acc_design_param_details = new ACC_TEO_SCREEN_DESIGN_PARAM_DETAILS();
            acc_design_param_details.DISTRICT_REQ = "N";
            acc_design_param_details.DPIU_REQ = "N";
            acc_design_param_details.AFREEMENT_REQ = "N";
            acc_design_param_details.CON_REQ = "N";
            acc_design_param_details.SUPP_REQ = "N";
            acc_design_param_details.ROAD_REQ = "N";
            acc_design_param_details.SAN_REQ = "N";
            acc_design_param_details.PKG_REQ = "N";
            return acc_design_param_details;
        }

        public ACC_TEO_SCREEN_DESIGN_HEADWISE_DETAILS setDefualtDesignHeadParam()
        {
            ACC_TEO_SCREEN_DESIGN_HEADWISE_DETAILS acc_headwise_param_details = new ACC_TEO_SCREEN_DESIGN_HEADWISE_DETAILS();
            acc_headwise_param_details.AGREEMENT_REQ = "N";
            acc_headwise_param_details.CON_REQ = "N";
            acc_headwise_param_details.ROAD_REQ = "N";
            acc_headwise_param_details.SANC_YEAR_REQ = "N";
            acc_headwise_param_details.PKG_REQ = "N";
            return acc_headwise_param_details;
        }

        public ACC_TEO_SCREEN_TXN_VALIDATIONS setDefaultValidationParam()
        {
            ACC_TEO_SCREEN_TXN_VALIDATIONS validationParams = new ACC_TEO_SCREEN_TXN_VALIDATIONS();
            validationParams.C_CREDIT_HEAD = 0;
            validationParams.C_DEBIT_HEAD = 0;
            validationParams.IS_DISTRICT_REPEAT = null;
            validationParams.IS_DPIU_REPEAT = null;
            validationParams.IS_CON_REPEAT = null;
            validationParams.IS_SUP_REPEAT = null;
            validationParams.IS_AGREEMENT_REPEAT = null;
            validationParams.IS_ROAD_REPEAT = null;
            validationParams.IS_HEAD_REPEAT = null;
            return validationParams;
        }

        public ACC_BILL_DETAILS GetBillDetails(Int64 billId, string creditDebit)
        {
            try
            {
                dbContext = new PMGSYEntities();
                return dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == billId && m.CREDIT_DEBIT == creditDebit).FirstOrDefault();
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

        public string ValidateEditTEOMaster(TeoMasterModel teoMasterModel)
        {
            try
            {
                ACC_BILL_MASTER acc_bill_master = new ACC_BILL_MASTER();
                dbContext = new PMGSYEntities();
                Int64 pBillId = 0;
                Int16? txnNo = 0;
                if (dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Where(p => p.S_BIll_ID == teoMasterModel.BILL_ID).Any())
                {
                    pBillId = dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Where(p => p.S_BIll_ID == teoMasterModel.BILL_ID).Select(p => p.P_BILL_ID).FirstOrDefault();
                    txnNo = dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Where(p => p.S_BIll_ID == teoMasterModel.BILL_ID).Select(p => p.P_TXN_ID).FirstOrDefault();
                }
                else
                {
                    return "";
                }

                List<Int64> lstBillIds = new List<Int64>();
                List<Int64> lstOBBillIds = new List<Int64>();
                Decimal amountSettled = 0;
                Decimal grossAmount = 0;
                if (dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == pBillId).Select(m => m.BILL_TYPE).FirstOrDefault() == "O")
                {
                    grossAmount = dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == pBillId && m.TXN_NO == txnNo).Select(m => m.AMOUNT).FirstOrDefault();
                }
                else
                {
                    grossAmount = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == pBillId).Select(m => m.GROSS_AMOUNT).FirstOrDefault();
                }

                if (dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Where(p => p.P_BILL_ID == pBillId).Any())
                {
                    lstBillIds = dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Where(p => p.P_BILL_ID == pBillId && p.P_TXN_ID == txnNo && p.S_BIll_ID != teoMasterModel.BILL_ID).Select(p => p.S_BIll_ID).ToList<Int64>();
                }

                if (lstBillIds.Count > 0)
                {
                    amountSettled = (from c in dbContext.ACC_BILL_MASTER
                                     where
                                     lstBillIds.Contains(c.BILL_ID)
                                     select c.GROSS_AMOUNT).Sum();
                }
                else
                {
                    return "";
                }

                if ((grossAmount - amountSettled) < teoMasterModel.GROSS_AMOUNT)
                {
                    return "Invalid Amount. Imprest Settlement amount must not be greater than " + (grossAmount - amountSettled);
                }
                else
                {
                    return "";
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

        public String IsFinalPayment(String parameters, string id)
        {
            try
            {
                dbContext = new PMGSYEntities();
                // Int32 contId = Convert.ToInt32(parameters.Split('$')[0]);
                // Int32 aggId = Convert.ToInt32(parameters.Split('$')[1]);
                Int32 roadId = Convert.ToInt32(parameters.Split('$')[2]);
                //need to check only for road
                // bool? isFinalPayment = dbContext.ACC_BILL_DETAILS.Where(m => m.MAST_CON_ID == contId && m.IMS_AGREEMENT_CODE == aggId && m.IMS_PR_ROAD_CODE == roadId &&( m.FINAL_PAYMENT.Value==true )).Select(m => m.FINAL_PAYMENT).FirstOrDefault();

                //new change added by Vikram
                Int32 billId = 0;
                if (!(id == null || id.Equals(string.Empty)))
                {
                    //String encryptedParameters = parameters.Split('$')[3];
                    
                        String[] codes = URLEncrypt.DecryptParameters(new string[] { id.Split('/')[0], id.Split('/')[1], id.Split('/')[2] });
                        billId = Convert.ToInt32(codes[0]);
                    
                }

                var isValid = (from bm in dbContext.ACC_BILL_MASTER
                               join bd in dbContext.ACC_BILL_DETAILS
                               on bm.BILL_ID equals bd.BILL_ID
                               where
                               bd.IMS_PR_ROAD_CODE == roadId &&
                               bm.BILL_DATE <= (dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == billId).Select(m => m.BILL_DATE).FirstOrDefault())
                               select new
                               {
                                   bd.FINAL_PAYMENT
                               }).Distinct().ToList();

                if (isValid != null)
                {
                    if (isValid.Any(m => m.FINAL_PAYMENT == true))
                    {
                        return "1";
                    }
                    else
                    {
                        return "0";
                    }
                }
                else
                {
                    return "0";
                }



                bool? isFinalPayment = dbContext.ACC_BILL_DETAILS.Where(m => m.IMS_PR_ROAD_CODE == roadId && m.FINAL_PAYMENT.Value == true).Select(m => m.FINAL_PAYMENT).FirstOrDefault();

                if (isFinalPayment == null)
                {
                    return "0";
                }
                else
                {
                    return "1";
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return "0";
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public List<SelectListItem> PopulateDPIUForTOB(TransactionParams objParam)
        {
            dbContext = new PMGSYEntities();
            List<SelectListItem> lstDPIU = new List<SelectListItem>();
            try
            {
                lstDPIU = new SelectList((from bm in dbContext.ACC_BILL_MASTER
                                          join ad in dbContext.ADMIN_DEPARTMENT
                                          on bm.ADMIN_ND_CODE equals ad.ADMIN_ND_CODE
                                          where
                                          bm.BILL_FINALIZED == "Y" &&
                                          (bm.BILL_TYPE == "O" || bm.BILL_TYPE == "R") &&
                                          bm.FUND_TYPE == PMGSYSession.Current.FundType &&
                                          ad.MAST_DISTRICT_CODE == objParam.DISTRICT_CODE
                                          select new
                                          {
                                              ad.ADMIN_ND_CODE,
                                              ad.ADMIN_ND_NAME
                                          }).Distinct(), "ADMIN_ND_CODE", "ADMIN_ND_NAME", objParam.ADMIN_ND_CODE).ToList();



                //lstDPIU = new SelectList(dbContext.ADMIN_DEPARTMENT.Where(m => m.MAST_DISTRICT_CODE == objParam.DISTRICT_CODE).Join(dbContext.ACC_BILL_MASTER,m=>m.ADMIN_ND_CODE,b=>b.ADMIN_ND_CODE,(m,b)=>new{m.ADMIN_ND_CODE,m.ADMIN_ND_NAME,b.BILL_TYPE,b.BILL_FINALIZED,b.FUND_TYPE}).Where(m=>m.BILL_TYPE == "O" && m.BILL_FINALIZED == "Y" && m.FUND_TYPE == PMGSYSession.Current.FundType).Distinct().ToList(), "ADMIN_ND_CODE", "ADMIN_ND_NAME", objParam.ADMIN_ND_CODE).ToList();
                lstDPIU.Insert(0, (new SelectListItem { Text = "Select DPIU", Value = "0" }));
                return lstDPIU;
            }
            catch (Exception)
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
        /// to save  teo cedit and debit entry for transfer of balances added by koustubh nakate on 28/08/2013
        /// </summary>
        /// <param name="teoDetailsModelForTOB"></param> 
        /// <param name="transId"></param> 
        /// <returns>success or error message  </returns>

        public string AddCreditDebitTEODetailsforTOB(TeoDetailsModelForTOB teoDetailsModelForTOB, out short transId)
        {
            try
            {
                dbContext = new PMGSYEntities();
                using (var scope = new TransactionScope())
                {
                    //teoDetailsModel.CREDIT_DEBIT = "C";
                    teoDetailsModelForTOB.CASH_CHQ = "J";


                    ACC_TEO_SCREEN_DESIGN_PARAM_DETAILS trans_design_params = new ACC_TEO_SCREEN_DESIGN_PARAM_DETAILS();
                    // ACC_TEO_SCREEN_DESIGN_HEADWISE_DETAILS head_design_params_C = new ACC_TEO_SCREEN_DESIGN_HEADWISE_DETAILS();
                    Int16 masterTransId = dbContext.ACC_BILL_MASTER.Where(b => b.BILL_ID == teoDetailsModelForTOB.BILL_ID).Select(b => b.TXN_ID).FirstOrDefault();
                    trans_design_params = dbContext.ACC_TEO_SCREEN_DESIGN_PARAM_DETAILS.Where(m => m.TXN_ID == masterTransId).FirstOrDefault();
                    ACC_TEO_SCREEN_DESIGN_HEADWISE_DETAILS head_design_params_C = dbContext.ACC_TEO_SCREEN_DESIGN_HEADWISE_DETAILS.Where(m => m.HEAD_ID == teoDetailsModelForTOB.HEAD_ID_C && m.TXN_ID == masterTransId).FirstOrDefault();
                    ACC_TEO_SCREEN_DESIGN_HEADWISE_DETAILS head_design_params_D = dbContext.ACC_TEO_SCREEN_DESIGN_HEADWISE_DETAILS.Where(m => m.HEAD_ID == teoDetailsModelForTOB.HEAD_ID_D && m.TXN_ID == masterTransId).FirstOrDefault();

                    if (trans_design_params == null)
                    {
                        trans_design_params = setDefualtDesignTransParam();
                    }
                    if (head_design_params_C == null)
                    {
                        head_design_params_C = setDefualtDesignHeadParam();
                    }
                    if (head_design_params_D == null)
                    {
                        head_design_params_D = setDefualtDesignHeadParam();
                    }

                    if (trans_design_params.DISTRICT_REQ == "N")
                    {
                        teoDetailsModelForTOB.MAST_DISTRICT_CODE_C = null;
                        teoDetailsModelForTOB.MAST_DISTRICT_CODE_D = null;
                    }
                    if (trans_design_params.DPIU_REQ == "N")
                    {
                        teoDetailsModelForTOB.ADMIN_ND_CODE_C = null;
                        teoDetailsModelForTOB.ADMIN_ND_CODE_D = null;
                    }

                    if (trans_design_params.CON_REQ == "N" && trans_design_params.SUPP_REQ == "N" && head_design_params_C.CON_REQ == "N")
                    {
                        teoDetailsModelForTOB.MAST_CON_ID_C = null;
                    }

                    if (trans_design_params.CON_REQ == "N" && trans_design_params.SUPP_REQ == "N" && head_design_params_D.CON_REQ == "N")
                    {
                        teoDetailsModelForTOB.MAST_CON_ID_D = null;
                    }

                    if (trans_design_params.AFREEMENT_REQ == "N" && head_design_params_C.AGREEMENT_REQ == "N")
                    {
                        teoDetailsModelForTOB.IMS_AGREEMENT_CODE_C = null;
                    }

                    if (trans_design_params.AFREEMENT_REQ == "N" && head_design_params_D.AGREEMENT_REQ == "N")
                    {
                        teoDetailsModelForTOB.IMS_AGREEMENT_CODE_D = null;
                    }



                    if (trans_design_params.SAN_REQ == "N" && head_design_params_C.SANC_YEAR_REQ == "N")
                    {
                        teoDetailsModelForTOB.SANC_YEAR_C = null;
                    }

                    if (trans_design_params.SAN_REQ == "N" && head_design_params_D.SANC_YEAR_REQ == "N")
                    {
                        teoDetailsModelForTOB.SANC_YEAR_D = null;
                    }



                    if (trans_design_params.PKG_REQ == "N" && head_design_params_C.PKG_REQ == "N")
                    {
                        teoDetailsModelForTOB.IMS_PACKAGE_ID_C = null;
                    }
                    if (trans_design_params.PKG_REQ == "N" && head_design_params_D.PKG_REQ == "N")
                    {
                        teoDetailsModelForTOB.IMS_PACKAGE_ID_D = null;
                    }

                    if (trans_design_params.ROAD_REQ == "N" && head_design_params_C.ROAD_REQ == "N")
                    {
                        teoDetailsModelForTOB.IMS_PR_ROAD_CODE_C = null;
                        teoDetailsModelForTOB.FINAL_PAYMENT_C = false;
                    }

                    if (trans_design_params.ROAD_REQ == "N" && head_design_params_D.ROAD_REQ == "N")
                    {
                        teoDetailsModelForTOB.IMS_PR_ROAD_CODE_D = null;
                        teoDetailsModelForTOB.FINAL_PAYMENT_D = false;
                    }


                    if (teoDetailsModelForTOB.IMS_PR_ROAD_CODE_C != null)
                    {
                        teoDetailsModelForTOB.MAS_FA_CODE_C = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == teoDetailsModelForTOB.IMS_PR_ROAD_CODE_C).Select(m => m.IMS_COLLABORATION).FirstOrDefault();
                    }

                    if (teoDetailsModelForTOB.IMS_PR_ROAD_CODE_D != null)
                    {
                        teoDetailsModelForTOB.MAS_FA_CODE_D = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == teoDetailsModelForTOB.IMS_PR_ROAD_CODE_D).Select(m => m.IMS_COLLABORATION).FirstOrDefault();
                    }

                    //ACC_BILL_DETAILS acc_bill_details_C = new ACC_BILL_DETAILS();
                    //ACC_BILL_DETAILS acc_bill_details_D = new ACC_BILL_DETAILS();
                    CloneTEODetailsModelForTOB(dbContext, teoDetailsModelForTOB);

                    transId = teoDetailsModelForTOB.TXN_NO;
                    scope.Complete();
                    return String.Empty;
                }
            }
            catch (DbEntityValidationException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                var errorMessages = ex.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.ErrorMessage);
                var fullErrorMessage = string.Join("; ", errorMessages);
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);
                transId = 0;
                return exceptionMessage.ToString();
                //throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }
            catch (EntityCommandExecutionException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                transId = 0;
                return ex.Message;
            }
            catch (EntityException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                transId = 0;
                return ex.Message;
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                transId = 0;
                return Ex.Message;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public void CloneTEODetailsModelForTOB(PMGSYEntities dbContext, TeoDetailsModelForTOB teoDetailsModelForTOB)
        {
            try
            {
                ACC_BILL_DETAILS acc_bill_details_C = new ACC_BILL_DETAILS();
                ACC_BILL_DETAILS acc_bill_details_D = new ACC_BILL_DETAILS();

                //save credit details
                Int16? maxTxnId = dbContext.ACC_BILL_DETAILS.Where(item => item.BILL_ID == teoDetailsModelForTOB.BILL_ID).Max(T => (Int16?)T.TXN_NO);

                if (maxTxnId == null)
                {
                    acc_bill_details_C.TXN_NO = 1;
                }
                else
                {
                    acc_bill_details_C.TXN_NO = Convert.ToInt16(maxTxnId + 1);
                }

                acc_bill_details_C.BILL_ID = teoDetailsModelForTOB.BILL_ID;
                acc_bill_details_C.CASH_CHQ = teoDetailsModelForTOB.CASH_CHQ;
                acc_bill_details_C.AMOUNT = Convert.ToDecimal(teoDetailsModelForTOB.AMOUNT_C);
                acc_bill_details_C.NARRATION = teoDetailsModelForTOB.NARRATION_C;
                acc_bill_details_C.HEAD_ID = teoDetailsModelForTOB.HEAD_ID_C;
                acc_bill_details_C.CREDIT_DEBIT = "C";
                acc_bill_details_C.MAST_DISTRICT_CODE = teoDetailsModelForTOB.MAST_DISTRICT_CODE_C;
                acc_bill_details_C.ADMIN_ND_CODE = teoDetailsModelForTOB.ADMIN_ND_CODE_C;
                acc_bill_details_C.MAST_CON_ID = teoDetailsModelForTOB.MAST_CON_ID_C;
                acc_bill_details_C.IMS_AGREEMENT_CODE = teoDetailsModelForTOB.IMS_AGREEMENT_CODE_C;
                acc_bill_details_C.IMS_PR_ROAD_CODE = teoDetailsModelForTOB.IMS_PR_ROAD_CODE_C;
                //acc_bill_details_C.FINAL_PAYMENT = teoDetailsModelForTOB.FINAL_PAYMENT_C == true ? true : null;

                if (teoDetailsModelForTOB.FINAL_PAYMENT_C == true)
                {
                    acc_bill_details_C.FINAL_PAYMENT = true;
                }

                acc_bill_details_C.MAS_FA_CODE = teoDetailsModelForTOB.MAS_FA_CODE_C;

                //Added By Abhishek Kamble 29-nov-2013
                acc_bill_details_C.USERID = PMGSYSession.Current.UserId;
                acc_bill_details_C.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];


                dbContext.ACC_BILL_DETAILS.Add(acc_bill_details_C);
                dbContext.SaveChanges();

                //save debit details


                acc_bill_details_D.TXN_NO = (short)(acc_bill_details_C.TXN_NO + 1);

                acc_bill_details_D.BILL_ID = teoDetailsModelForTOB.BILL_ID;
                acc_bill_details_D.CASH_CHQ = teoDetailsModelForTOB.CASH_CHQ;
                acc_bill_details_D.AMOUNT = Convert.ToDecimal(teoDetailsModelForTOB.AMOUNT_D);
                acc_bill_details_D.NARRATION = teoDetailsModelForTOB.NARRATION_D;
                acc_bill_details_D.HEAD_ID = teoDetailsModelForTOB.HEAD_ID_D;
                acc_bill_details_D.CREDIT_DEBIT = "D";
                acc_bill_details_D.MAST_DISTRICT_CODE = teoDetailsModelForTOB.MAST_DISTRICT_CODE_D;
                acc_bill_details_D.ADMIN_ND_CODE = teoDetailsModelForTOB.ADMIN_ND_CODE_D;
                acc_bill_details_D.MAST_CON_ID = teoDetailsModelForTOB.MAST_CON_ID_D;
                acc_bill_details_D.IMS_AGREEMENT_CODE = teoDetailsModelForTOB.IMS_AGREEMENT_CODE_D;
                acc_bill_details_D.IMS_PR_ROAD_CODE = teoDetailsModelForTOB.IMS_PR_ROAD_CODE_D;
                //acc_bill_details_C.FINAL_PAYMENT = teoDetailsModelForTOB.FINAL_PAYMENT_C == true ? true : null;

                if (teoDetailsModelForTOB.FINAL_PAYMENT_D == true)
                {
                    acc_bill_details_D.FINAL_PAYMENT = true;
                }

                acc_bill_details_D.MAS_FA_CODE = teoDetailsModelForTOB.MAS_FA_CODE_D;

                //Added By Abhishek Kamble 29-nov-2013
                acc_bill_details_D.USERID = PMGSYSession.Current.UserId;
                acc_bill_details_D.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                dbContext.ACC_BILL_DETAILS.Add(acc_bill_details_D);
                dbContext.SaveChanges();
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw Ex;
            }
        }

        public bool DeleteTEODetailsForTOB(long billID, short txnNo, ref string message)
        {
            dbContext = new PMGSYEntities();
            try
            {
                //Added By Abhishek Kamble 29-nov-2013

                dbContext.Database.ExecuteSqlCommand("UPDATE [omms].ACC_BILL_DETAILS SET USERID={0},IPADD={1} Where BILL_ID = {2} AND TXN_NO IN ({3},{4})", PMGSYSession.Current.UserId, HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], billID, txnNo, txnNo + 1);

                dbContext.Database.ExecuteSqlCommand("DELETE [omms].ACC_BILL_DETAILS Where BILL_ID = {0} AND TXN_NO IN ({1},{2})", billID, txnNo, txnNo + 1);
                dbContext.SaveChanges();
                return true;
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "You can not delete this TEO details.";
                return false;
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

        //added by Koustubh Nakate on 30/09/2013 to check head is already exist or not for given BILL ID  
        public bool CheckHeadAlreadyExistDAL(long billID, short headID, string creditDebit)
        {

            dbContext = new PMGSYEntities();
            try
            {
                int count = dbContext.ACC_BILL_DETAILS.Where(bd => bd.BILL_ID == billID && bd.HEAD_ID == headID && bd.CREDIT_DEBIT == creditDebit).Count();

                if (count > 0)
                {
                    return true;
                }

                return false;
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

        #region NEW_CHANGE_TOB_AUTO_ENTRY
        //new method added by Vikram on 07-10-2013 for auto entry of TOB
        /// <summary>
        /// saves the details of transfer of balances at dpiu
        /// </summary>
        /// <param name="billId"></param>
        /// <returns></returns>
        public bool AddAutoEntryTOB(long billId)
        {
            dbContext = new PMGSYEntities();
            try
            {
                int result = dbContext.USP_ACC_TEO_INSERT_AUTOTXNS_ATPIU(billId, PMGSYSession.Current.FundType, PMGSYSession.Current.UserId, HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]);
            //var alertDetails =    dbContext.USP_ACC_INSERT_ALERT_DETAILS(PMGSYSession.Current.AdminNdCode, PMGSYSession.Current.FundType, "X", 4, billId, PMGSYSession.Current.UserId, HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]);
              
                
                if (result > 0 )
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

        public bool ValidateGrossAmount(TeoMasterModel teoMasterModel, ref string ValidationSummary)
        {
            dbContext = new PMGSYEntities();
            try
            {
                decimal? settledAmount = null;
                if (dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Any(m => m.S_BIll_ID == teoMasterModel.PBILL_ID))
                {
                    ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS mapDetails = dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Where(m => m.S_BIll_ID == teoMasterModel.PBILL_ID).FirstOrDefault();

                }
                decimal grossAmount = dbContext.ACC_BILL_MASTER.Where(bm => bm.BILL_ID == teoMasterModel.PBILL_ID).Select(bm => bm.GROSS_AMOUNT).FirstOrDefault();

                if (grossAmount < teoMasterModel.GROSS_AMOUNT)
                {
                    ValidationSummary = "Imprest amount must be less than Gross Amount";
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return true;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        public bool CheckForTransactionAlreadyExistDAL(long billID, ref int DistrictC, ref int PIUC, ref int DistrictD, ref int PIUD, ref int? ContractorC, ref int? StateD)
        {
            dbContext = new PMGSYEntities();
            Int16 TxnNo = 1;
            try
            {
                //commented by Vikram on 27-11-2013
                //ACC_BILL_DETAILS accBillDetailsC = dbContext.ACC_BILL_DETAILS.Where(acc => acc.BILL_ID == billID && acc.CREDIT_DEBIT == "C" && acc.TXN_NO==TxnNo).FirstOrDefault();
                //ACC_BILL_DETAILS accBillDetailsD = dbContext.ACC_BILL_DETAILS.Where(acc => acc.BILL_ID == billID && acc.CREDIT_DEBIT == "D" && acc.TXN_NO == TxnNo + 1).FirstOrDefault();

                //new change done by Vikram as if the transaction no 
                ACC_BILL_DETAILS accBillDetailsC = dbContext.ACC_BILL_DETAILS.Where(acc => acc.BILL_ID == billID && acc.CREDIT_DEBIT == "C").FirstOrDefault();
                ACC_BILL_DETAILS accBillDetailsD = dbContext.ACC_BILL_DETAILS.Where(acc => acc.BILL_ID == billID && acc.CREDIT_DEBIT == "D").FirstOrDefault();
                //end of change


                ContractorC = dbContext.ACC_BILL_DETAILS.Where(acc => acc.BILL_ID == billID && acc.MAST_CON_ID != null).Select(acc => acc.MAST_CON_ID).FirstOrDefault();

                if (accBillDetailsC != null && accBillDetailsD != null)
                {
                    DistrictC = accBillDetailsC.MAST_DISTRICT_CODE == null ? 0 : (Int32)accBillDetailsC.MAST_DISTRICT_CODE;
                    PIUC = accBillDetailsC.ADMIN_ND_CODE == null ? 0 : (Int32)accBillDetailsC.ADMIN_ND_CODE;

                    DistrictD = accBillDetailsD.MAST_DISTRICT_CODE == null ? 0 : (Int32)accBillDetailsD.MAST_DISTRICT_CODE;
                    PIUD = accBillDetailsD.ADMIN_ND_CODE == null ? 0 : (Int32)accBillDetailsD.ADMIN_ND_CODE;

                    StateD = dbContext.MASTER_DISTRICT.Where(m => m.MAST_DISTRICT_CODE == (accBillDetailsD.MAST_DISTRICT_CODE == null ? 0 : (Int32)accBillDetailsD.MAST_DISTRICT_CODE)).Select(s => s.MAST_STATE_CODE).FirstOrDefault();

                    return true;

                }
                return false;
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

        //added by Vikram on 15-10-2013 for validation of Transaction Head 
        /// <summary>
        /// validates the entered road matches with the type of account head selected
        /// </summary>
        /// <param name="proposalCode">id of road</param>
        /// <param name="upgradeConnectFlag">type of road</param>
        /// <returns>true/false</returns>
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
        /// checks whether the district is shifted or not
        /// </summary>
        /// <param name="DistrictC">credit side district</param>
        /// <param name="DistrictD">debit side district</param>
        /// <returns></returns>
        //public bool CheckIsDistrictShiftedDAL(int DistrictC, int DistrictD)
        //{
        //    dbContext = new PMGSYEntities();

        //    try
        //    {

        //        var lstRoads = from IMS in dbContext.IMS_SANCTIONED_PROJECTS
        //                       join Track in dbContext.IMS_PROPOSAL_TRACKING
        //                       on IMS.IMS_PR_ROAD_CODE equals Track.IMS_PR_ROAD_CODE
        //                       where //Commented By Abhishek kamble 3-March-2014
        //                           //Track.MAST_DISTRICT_CODE == DistrictC &&
        //                           //IMS.MAST_DISTRICT_CODE == DistrictD
        //                       (Track.MAST_DISTRICT_CODE == DistrictC || Track.MAST_DISTRICT_CODE == DistrictD) &&
        //                       (IMS.MAST_DISTRICT_CODE == DistrictC || IMS.MAST_DISTRICT_CODE == DistrictD)
        //                       select IMS.IMS_PR_ROAD_CODE;

        //        if (lstRoads.Count() > 0)
        //        {
        //            return true;
        //        }

        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
        //        return false;
        //    }
        //    finally
        //    {
        //        if (dbContext != null)
        //        {
        //            dbContext.Dispose();
        //        }
        //    }
        //}

        public bool CheckIsDistrictShiftedDAL(int DistrictC, int DistrictD)
        {
            dbContext = new PMGSYEntities();

            try
            {


                var lstRoads = from IMS in dbContext.IMS_SANCTIONED_PROJECTS
                               join Track in dbContext.IMS_PROPOSAL_TRACKING
                               on IMS.IMS_PR_ROAD_CODE equals Track.IMS_PR_ROAD_CODE
                               where //Commented By Abhishek kamble 3-March-2014
                                   Track.MAST_DISTRICT_CODE == DistrictC &&
                                   IMS.MAST_DISTRICT_CODE == DistrictD
                               //(Track.MAST_DISTRICT_CODE == DistrictC || Track.MAST_DISTRICT_CODE == DistrictD) &&
                               //(IMS.MAST_DISTRICT_CODE == DistrictC || IMS.MAST_DISTRICT_CODE == DistrictD)
                               select IMS.IMS_PR_ROAD_CODE;

                var lstRoads1 = from IMS in dbContext.IMS_SANCTIONED_PROJECTS
                                join Track in dbContext.IMS_PROPOSAL_TRACKING
                                on IMS.IMS_PR_ROAD_CODE equals Track.IMS_PR_ROAD_CODE
                                where //Commented By Abhishek kamble 3-March-2014
                                    Track.MAST_DISTRICT_CODE == DistrictD &&
                                    IMS.MAST_DISTRICT_CODE == DistrictC
                                //(Track.MAST_DISTRICT_CODE == DistrictC || Track.MAST_DISTRICT_CODE == DistrictD) &&
                                //(IMS.MAST_DISTRICT_CODE == DistrictC || IMS.MAST_DISTRICT_CODE == DistrictD)
                                select IMS.IMS_PR_ROAD_CODE;

                if (lstRoads.Count() > 0 || lstRoads1.Count() > 0)
                {
                    return true;
                }

                return false;
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


        /// <summary>
        /// validates whether the month is closed or not
        /// </summary>
        /// <param name="adminCode"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ValidateDPIUMonths(int adminCode, string id)
        {
            using (dbContext = new PMGSYEntities())
            {
                int billId = 0;
                try
                {
                    String[] parameters = URLEncrypt.DecryptParameters(new string[] { id.Split('/')[0].Replace(" ", "+"), id.Split('/')[1], id.Split('/')[2] });

                    if (!string.IsNullOrEmpty(parameters[0]))
                    {
                        billId = Convert.ToInt32(parameters[0]);
                        var lstMonthYear = (from item in dbContext.ACC_BILL_MASTER
                                            where item.BILL_ID == billId
                                            && item.FUND_TYPE == PMGSYSession.Current.FundType
                                            select new
                                            {
                                                item.BILL_MONTH,
                                                item.BILL_YEAR
                                            }).FirstOrDefault();

                        if (dbContext.ACC_RPT_MONTHWISE_SUMMARY.Any(m => m.ACC_MONTH == lstMonthYear.BILL_MONTH && m.ACC_YEAR == lstMonthYear.BILL_YEAR && m.ADMIN_ND_CODE == adminCode && m.FUND_TYPE == PMGSYSession.Current.FundType))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        #region OLD_DATA_IMPREST_PAYMENTS_MAPPING

        /// <summary>
        /// for populating the list of imprest payment details
        /// </summary>
        /// <param name="objFilter">contains the stored procedure parameters</param>
        /// <param name="totalRecords">no. of records</param>
        /// <returns></returns>
        public Array OldImprestPaymentListDAL(ReceiptFilterModel objFilter, out long totalRecords)
        {
            using (dbContext = new PMGSYEntities())
            {
                try
                {
                    var lstPaymentDetails = dbContext.USP_ACC_DISPLAY_IMPREST_PAYMENT_DETAILS(PMGSYSession.Current.FundType, PMGSYSession.Current.AdminNdCode, objFilter.Month, objFilter.Year).ToList();

                    totalRecords = lstPaymentDetails.Count();

                    if (objFilter.sidx.Trim() != string.Empty)
                    {
                        if (objFilter.sord.ToString() == "asc")
                        {
                            switch (objFilter.sidx)
                            {
                                case "BILL_NO":
                                    lstPaymentDetails = lstPaymentDetails.OrderBy(x => x.BILL_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                    break;
                                case "BILL_DATE":
                                    lstPaymentDetails = lstPaymentDetails.OrderBy(x => x.BILL_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                    break;
                                case "CHQ_NO":
                                    lstPaymentDetails = lstPaymentDetails.OrderBy(x => x.CHQ_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                    break;
                                case "PAYEE_NAME":
                                    lstPaymentDetails = lstPaymentDetails.OrderBy(x => x.PAYEE_NAME).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                    break;
                                case "Advance_Amount":
                                    lstPaymentDetails = lstPaymentDetails.OrderBy(x => x.Advance_Amount).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                    break;
                                case "Settled_Amount":
                                    lstPaymentDetails = lstPaymentDetails.OrderBy(x => x.Settled_Amount).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                    break;
                                case "MonthYear":
                                    lstPaymentDetails = lstPaymentDetails.OrderBy(x => x.MonthYear).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                    break;
                                default:
                                    lstPaymentDetails = lstPaymentDetails.OrderBy(x => x.BILL_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                    break;
                            }

                        }
                        else
                        {
                            switch (objFilter.sidx)
                            {
                                case "BILL_NO":
                                    lstPaymentDetails = lstPaymentDetails.OrderByDescending(x => x.BILL_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                    break;
                                case "BILL_DATE":
                                    lstPaymentDetails = lstPaymentDetails.OrderByDescending(x => x.BILL_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                    break;
                                case "CHQ_NO":
                                    lstPaymentDetails = lstPaymentDetails.OrderByDescending(x => x.CHQ_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                    break;
                                case "PAYEE_NAME":
                                    lstPaymentDetails = lstPaymentDetails.OrderByDescending(x => x.PAYEE_NAME).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                    break;
                                case "Advance_Amount":
                                    lstPaymentDetails = lstPaymentDetails.OrderByDescending(x => x.Advance_Amount).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                    break;
                                case "Settled_Amount":
                                    lstPaymentDetails = lstPaymentDetails.OrderByDescending(x => x.Settled_Amount).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                    break;
                                case "MonthYear":
                                    lstPaymentDetails = lstPaymentDetails.OrderByDescending(x => x.MonthYear).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                    break;
                                default:
                                    lstPaymentDetails = lstPaymentDetails.OrderByDescending(x => x.BILL_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                    break;
                            }
                        }
                    }
                    else
                    {
                        lstPaymentDetails = lstPaymentDetails.OrderByDescending(x => x.BILL_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                    }


                    return lstPaymentDetails.Select(item => new
                    {
                        cell = new[] 
                        {
                            item.BILL_NO == null?string.Empty:item.BILL_NO.ToString(),
                            item.BILL_DATE == null?"-":item.BILL_DATE.ToString("dd/MM/yyyy"),
                            item.MonthYear == null?string.Empty:item.MonthYear.ToString(),
                            item.CHQ_NO == null?string.Empty:item.CHQ_NO.ToString(),
                            item.PAYEE_NAME == null?"-":item.PAYEE_NAME.ToString(),
                            item.Advance_Amount == null?"0":item.Advance_Amount.ToString(),
                            item.Settled_Amount == null?"0":item.Settled_Amount.ToString(),
                            item.Settled_Amount < item.Advance_Amount?"<center><a href='#' class='ui-icon ui-icon-plusthick' onclick='MapTEODetails(\"" +URLEncrypt.EncryptParameters1(new string[] { "BILL_ID="+ item.BILL_ID.ToString().Trim()})+"\");return false;'>Edit</a></center>":"-",

                        }
                    }).ToArray();


                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    totalRecords = 0;
                    return null;
                }
            }
        }

        /// <summary>
        /// for populating the list of TEO details
        /// </summary>
        /// <param name="objFilter">contains the stored procedure parameters</param>
        /// <param name="totalRecords">no. of records</param>
        /// <returns></returns>
        public Array ListSettledTEORecieptDetailsDAL(ReceiptFilterModel objFilter, out long totalRecords)
        {
            using (dbContext = new PMGSYEntities())
            {
                try
                {
                    var _lstTEORecieptDetails = dbContext.USP_ACC_DISPLAY_IMPREST_Settlement_DETAILS(PMGSYSession.Current.FundType, PMGSYSession.Current.AdminNdCode, objFilter.Month, objFilter.Year).ToList();

                    _lstTEORecieptDetails = _lstTEORecieptDetails.Where(m => m.IsSettled == "N").ToList();

                    totalRecords = _lstTEORecieptDetails.Count();

                    if (objFilter.sidx.Trim() != string.Empty)
                    {
                        if (objFilter.sord.ToString() == "asc")
                        {
                            switch (objFilter.sidx)
                            {
                                case "BILL_NO":
                                    _lstTEORecieptDetails = _lstTEORecieptDetails.OrderBy(x => x.BILL_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                    break;
                                case "BILL_DATE":
                                    _lstTEORecieptDetails = _lstTEORecieptDetails.OrderBy(x => x.BILL_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                    break;
                                case "MonthYear":
                                    _lstTEORecieptDetails = _lstTEORecieptDetails.OrderBy(x => x.MonthYear).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                    break;
                                case "CHQ_EPAY":
                                    _lstTEORecieptDetails = _lstTEORecieptDetails.OrderBy(x => x.CHQ_EPAY).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                    break;
                                case "CHQ_NO":
                                    _lstTEORecieptDetails = _lstTEORecieptDetails.OrderBy(x => x.CHQ_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                    break;
                                case "TXN_DESC":
                                    _lstTEORecieptDetails = _lstTEORecieptDetails.OrderBy(x => x.TXN_DESC).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                    break;
                                case "AMOUNT":
                                    _lstTEORecieptDetails = _lstTEORecieptDetails.OrderBy(x => x.AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                    break;
                                case "NARRATION":
                                    _lstTEORecieptDetails = _lstTEORecieptDetails.OrderBy(x => x.NARRATION).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                    break;
                                default:
                                    _lstTEORecieptDetails = _lstTEORecieptDetails.OrderBy(x => x.CHQ_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                    break;
                            }

                        }
                        else
                        {
                            switch (objFilter.sidx)
                            {
                                case "BILL_NO":
                                    _lstTEORecieptDetails = _lstTEORecieptDetails.OrderByDescending(x => x.BILL_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                    break;
                                case "BILL_DATE":
                                    _lstTEORecieptDetails = _lstTEORecieptDetails.OrderByDescending(x => x.BILL_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                    break;
                                case "MonthYear":
                                    _lstTEORecieptDetails = _lstTEORecieptDetails.OrderByDescending(x => x.MonthYear).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                    break;
                                case "CHQ_EPAY":
                                    _lstTEORecieptDetails = _lstTEORecieptDetails.OrderByDescending(x => x.CHQ_EPAY).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                    break;
                                case "CHQ_NO":
                                    _lstTEORecieptDetails = _lstTEORecieptDetails.OrderByDescending(x => x.CHQ_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                    break;
                                case "TXN_DESC":
                                    _lstTEORecieptDetails = _lstTEORecieptDetails.OrderByDescending(x => x.TXN_DESC).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                    break;
                                case "AMOUNT":
                                    _lstTEORecieptDetails = _lstTEORecieptDetails.OrderByDescending(x => x.AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                    break;
                                case "NARRATION":
                                    _lstTEORecieptDetails = _lstTEORecieptDetails.OrderByDescending(x => x.NARRATION).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                    break;
                                default:
                                    _lstTEORecieptDetails = _lstTEORecieptDetails.OrderByDescending(x => x.CHQ_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                    break;
                            }
                        }
                    }
                    else
                    {
                        _lstTEORecieptDetails = _lstTEORecieptDetails.OrderByDescending(x => x.CHQ_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                    }

                    return _lstTEORecieptDetails.Select(item => new
                    {
                        id = item.BILL_ID.ToString() + "$" + item.TXN_NO.ToString(),
                        cell = new[] 
                        {
                            item.BILL_NO.ToString(),
                            item.BILL_DATE == null?"-":item.BILL_DATE.ToString(),
                            item.MonthYear == null?string.Empty:item.MonthYear.ToString(),
                            item.CHQ_EPAY == null?string.Empty:item.CHQ_EPAY.ToString(),
                            item.CHQ_NO == null?string.Empty:item.CHQ_NO.ToString(),
                            item.TXN_DESC == null?string.Empty:item.TXN_DESC.ToString(),
                            item.AMOUNT == null?"0":item.AMOUNT.ToString(),
                            item.NARRATION == null?"0":item.NARRATION.ToString(),
                            //"<input type='checkbox' id="+item.BILL_ID.ToString() + "$"+item.TXN_NO.ToString()+"/>"
                        }
                    }).ToArray();

                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    totalRecords = 0;
                    return null;
                }
            }
        }

        /// <summary>
        /// validates and add the entries into the mapping table
        /// </summary>
        /// <param name="s_billIds">array of settlement bill ids</param>
        /// <param name="P_BILL_ID">payment bill id</param>
        /// <param name="message">response message</param>
        /// <returns></returns>
        public Boolean ValidatePaymentAmountDAL(String[] s_billIds, int P_BILL_ID, out string message)
        {
            using (dbContext = new PMGSYEntities())
            {
                int sBillID = 0;
                int txnNo = 0;
                decimal? totalPaymentAmount = 0;
                decimal? totalSettledAmount = 0;
                DateTime? pBillDate = null;
                ACC_BILL_MASTER billMaster = null;
                try
                {
                    if (P_BILL_ID != 0)
                    {
                        billMaster = dbContext.ACC_BILL_MASTER.Find(P_BILL_ID);
                        totalPaymentAmount = billMaster.GROSS_AMOUNT;
                        if (dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Any(m => m.P_BILL_ID == P_BILL_ID))
                        {
                            List<ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS> lstMapDetails = dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Where(m => m.P_BILL_ID == P_BILL_ID).ToList();
                            if (lstMapDetails != null)
                            {
                                foreach (var item in lstMapDetails)
                                {
                                    totalPaymentAmount = totalPaymentAmount - (dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == item.S_BIll_ID && m.TXN_NO == item.S_TXN_ID).Select(m => m.AMOUNT).FirstOrDefault());
                                }
                            }
                        }
                        pBillDate = billMaster.BILL_DATE;
                    }

                    if (s_billIds != null)
                    {
                        foreach (var item in s_billIds)
                        {
                            sBillID = Convert.ToInt32(item.Split('$')[0]);
                            txnNo = Convert.ToInt32(item.Split('$')[1]);
                            totalSettledAmount = totalSettledAmount + dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == sBillID && m.TXN_NO == txnNo).Select(m => m.AMOUNT).FirstOrDefault();
                            if (pBillDate > dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == sBillID).Select(m => m.BILL_DATE).FirstOrDefault())
                            {
                                message = "The Voucher " + dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == sBillID).Select(m => m.BILL_NO).FirstOrDefault() + " has date less than the Payment date.";
                                return false;
                            }
                        }
                    }

                    if (totalPaymentAmount < totalSettledAmount)
                    {
                        message = "Total Settled amount is greater than total payment amount.";
                        return false;
                    }
                    else
                    {
                        using (var tScope = new TransactionScope())
                        {
                            foreach (var item in s_billIds)
                            {
                                sBillID = Convert.ToInt32(item.Split('$')[0]);
                                txnNo = Convert.ToInt32(item.Split('$')[1]);
                                ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS mapDetails = new ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS();
                                if (dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Any())
                                {
                                    mapDetails.MAP_ID = dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Max(m => m.MAP_ID) + 1;
                                }
                                else
                                {
                                    mapDetails.MAP_ID = 1;
                                }
                                mapDetails.P_BILL_ID = P_BILL_ID;
                                mapDetails.P_TXN_ID = 2;
                                mapDetails.S_BIll_ID = sBillID;
                                mapDetails.S_TXN_ID = Convert.ToInt16(txnNo);
                                mapDetails.USERID = PMGSYSession.Current.UserId;
                                mapDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                                dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Add(mapDetails);
                                dbContext.SaveChanges();
                            }
                            tScope.Complete();
                            message = "The selected vouchers are mapped successfully.";
                            return true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    message = "Error occurred while processing your request.";
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    return false;
                }
            }
        }

        #endregion

        public bool ValidateTransaction(int txnId, string billDate, ref string message)
        {
            try
            {
                commonFuncObj = new CommonFunctions();
                DateTime BILL_DATE = commonFuncObj.GetStringToDateTime(billDate);
                using (dbContext = new PMGSYEntities())
                {
                    if (dbContext.ACC_BILL_MASTER.Any(m => m.TXN_ID == 113 && m.FUND_TYPE == PMGSYSession.Current.FundType && m.LVL_ID == 5 && m.BILL_TYPE == "P" && m.BILL_FINALIZED == "Y" && m.BILL_MONTH == PMGSYSession.Current.AccMonth && m.BILL_YEAR == PMGSYSession.Current.AccYear && m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode))
                    {
                        if (dbContext.ACC_BILL_MASTER.Any(m => m.TXN_ID == 113 && m.FUND_TYPE == PMGSYSession.Current.FundType && m.LVL_ID == 5 && m.BILL_TYPE == "P" && m.BILL_FINALIZED == "Y" && m.BILL_MONTH == PMGSYSession.Current.AccMonth && m.BILL_YEAR == PMGSYSession.Current.AccYear && m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && m.BILL_DATE > BILL_DATE))
                        {
                            message = "TEO Date must be greater than or equal to Payment date of Department Works.";
                            return false;
                        }
                        return true;
                    }
                    else
                    {
                        message = "Payment has not been made or finalized to made this adjustment";
                        return false;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }


        //Added By Abhishek kamble To get Txn ID 16Oct2014 start
        public int GetMasterTXNId(Int64 billId)
        {
            dbContext = new PMGSYEntities();
            try
            {
                return dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == billId).Select(s => s.TXN_ID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return 0;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
        }
        //Added By Abhishek kamble To get Txn ID 16Oct2014 end

        //Added By Abhishek kamble To get Agr No start
        public string GetAgreementNumberForMF(int? ManeContractId)
        {
            try
            {
                dbContext = new PMGSYEntities();

                if (ManeContractId == null)
                {
                    return null;
                }

                return dbContext.MANE_IMS_CONTRACT.Where(m => m.MANE_CONTRACT_ID == ManeContractId).Select(s => s.MANE_AGREEMENT_NUMBER).FirstOrDefault();

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

        //Added By Abhishek kamble To get Agr No start


    }

    public interface ITransferEntryOrderDAL
    {
        Int64 AddTEOMaster(TeoMasterModel teoMasterModel);
        List<ModelErrorList> GetAddTEODetailsModelErrors(TeoDetailsModel teoDetailsModel, String CreditDebit);
        Array TEOMasterList(ReceiptFilterModel objFilter);
        Array TEODetailsList(ReceiptFilterModel objFilter, out long totalRecords, out decimal cTotalAmonut, out decimal dTotalAmount, out decimal GrossAmount, bool isTransferofBalances = false);
        TeoMasterModel GetTEOMaster(Int64 billId);
        String EditTEOMaster(TeoMasterModel teoMasterModel);
        Int16 GetMasterTransId(Int64 billId);
        String AddCreditTEODetails(TeoDetailsModel teoDetailsModel, out Int16 transId);
        String AddDebitTEODetails(TeoDetailsModel teoDetailsModel, out Int16 transId);
        String EditCreditTEODetails(TeoDetailsModel teoDetailsModel);
        String EditDebitTEODetails(TeoDetailsModel teoDetailsModel);
        String DeleteTEO(Int64 billId);
        String DeleteTEODetails(Int64 billId, Int16 TransNo);
        TeoDetailsModel GetTEODetailByTransNo(Int64 billId, Int16 transId);
        String FinalizeTEO(Int64 billId);
        String IsTEOFinalized(Int64 billId);
        Array ImprestMasterList(ReceiptFilterModel objFilter, out long totalRecords);
        Int64 AddImprestMaster(TeoMasterModel teoMasterModel);
        ACC_BILL_DETAILS GetBillDetails(Int64 billId, string creditDebit);
        String ValidateEditTEOMaster(TeoMasterModel teoMasterModel);
        String IsFinalPayment(String parameters, string id);
        bool ValidateRoadForTransferBetweenDistricts(long billid);

        Array TEOList(ReceiptFilterModel objFilter, out long totalRecords, bool isTransferofBalances);

        string AddCreditDebitTEODetailsforTOB(TeoDetailsModelForTOB teoDetailsModelForTOB, out short transId);

        Array ImprestSettlementMasterList(ReceiptFilterModel objFilter, out long totalRecords);

        bool DeleteTEODetailsForTOB(long billID, short txnNo, ref string message);

        //added by Koustubh Nakate on 30/09/2013 to check head is already exist or not for given BILL ID  
        bool CheckHeadAlreadyExistDAL(long billID, short headID, string creditDebit);

        //new method added by Vikram on 07-10-2013 for auto entry of TOB
        bool AddAutoEntryTOB(long billId);

        //added by Koustubh Nakate on 11/10/2013 to check entered amount exceeds its gross amount
        bool ValidateGrossAmount(TeoMasterModel teoMasterModel, ref string ValidationSummary);

        //added by Koustubh Nakate on 14/10/2013 to check transaction is already exist or not for given BILL ID  

        bool CheckForTransactionAlreadyExistDAL(long billID, ref int DistrictC, ref int PIUC, ref int DistrictD, ref int PIUD, ref int? ContractorC, ref int? StateD);

        //added by Vikram on 15-10-2013 for validation of Transaction Head 
        bool ValidateRoad(int proposalCode, string upgradeConnectFlag);


        //added by Koustubh Nakate on 17/10/2013 to check district has been shifted or not  
        bool CheckIsDistrictShiftedDAL(int DistrictC, int DistrictD);

        bool ValidateDPIUMonths(int adminCode, string id);

        #region OLD_DATA_IMPREST_PAYMENTS_MAPPING

        Array OldImprestPaymentListDAL(ReceiptFilterModel objFilter, out long totalRecords);
        Array ListSettledTEORecieptDetailsDAL(ReceiptFilterModel objFilter, out long totalRecords);
        Boolean ValidatePaymentAmountDAL(String[] s_billIds, int P_BILL_ID, out string message);
        #endregion


        bool ValidateTransaction(int txnId, string billDate, ref string message);



        //Added By Abhishek kamble To get Txn ID 16Oct2014 start
        int GetMasterTXNId(Int64 billId);

        string GetAgreementNumberForMF(int? ManeContractId);
    }
}