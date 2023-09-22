using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.Extensions;
using PMGSY.Models;
using PMGSY.Common;
using PMGSY.Models.Report;
using System.Transactions;
using System.Web.Mvc;
using PMGSY.Models.Report.Ledger;
//using PMGSY.Models.Report.MaintenanceFund;
//using PMGSY.Models.Report.ProgramFund;
//using PMGSY.Models.Report.AdminFund;
using PMGSY.Models.Report.Account;

using PMGSY.Models.Report.RegisterOfWorks;
using PMGSY.Models.Common;
using System.Data.SqlClient;
using System.Globalization;
using System.Data.Entity.Infrastructure;
namespace PMGSY.DAL.Reports
{
    public class ReportDAL : IReportDAL
    {
        PMGSYEntities dbContext = new PMGSYEntities();
        CommonFunctions objCommonFunc = new CommonFunctions();


        #region CASH_BOOK

        public CBHeader GetReportHeader(ReportFilter objParam)
        {
            try
            {
                dbContext = new PMGSYEntities();
                CBHeader cbHeader = new CBHeader();
                cbHeader.ReportAnnex = "PMGSY/IA/F-3";
                if (objParam.LevelId == 5)
                {
                    cbHeader.DistrictDepartment = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == objParam.AdminNdCode).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
                    cbHeader.StateDepartment = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == (dbContext.ADMIN_DEPARTMENT.Where(p => p.ADMIN_ND_CODE == objParam.AdminNdCode).Select(p => p.MAST_PARENT_ND_CODE).FirstOrDefault())).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
                }
                else
                {
                    cbHeader.StateDepartment = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == objParam.AdminNdCode).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
                }
                cbHeader.OpeningBalace = dbContext.UDF_ACC_GEN_GET_BA_CASH_Opening_Balances(objParam.FundType, objParam.AdminNdCode, objParam.Month, objParam.Year, objParam.LevelId).FirstOrDefault();
                cbHeader.ClosingBalace = dbContext.UDF_ACC_GEN_GET_BA_CASH_CLOSING_BALANCES(objParam.FundType, objParam.AdminNdCode, objParam.Month, objParam.Year, objParam.LevelId).FirstOrDefault();
                return cbHeader;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public CBSingleModel GetSingleCB(ReportFilter objParam)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    dbContext = new PMGSYEntities();
                    CBSingleModel cbSingle = new CBSingleModel();
                    List<SP_ACC_CASHBOOK_RECEIPT_SIDE_Result> lstReceiptCB = new List<SP_ACC_CASHBOOK_RECEIPT_SIDE_Result>();
                    List<SP_ACC_CASHBOOK_PAYMENT_SIDE_Result> lstPaymentCB = new List<SP_ACC_CASHBOOK_PAYMENT_SIDE_Result>();

                    short LevelId = 0;
                    if (objParam.Selection == "D" && objParam.Dpiu != 0)
                    {
                        LevelId = 5;
                    }
                    else
                    {
                        if (PMGSYSession.Current.LevelId == 6)
                        {
                            LevelId = 4;
                        }
                        else
                        {
                            LevelId = PMGSYSession.Current.LevelId;
                        }
                    }

                    //set Agency Name And DPIU name
                    cbSingle.DistrictDepartment = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == objParam.AdminNdCode).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
                    cbSingle.StateDepartment = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == (dbContext.ADMIN_DEPARTMENT.Where(p => p.ADMIN_ND_CODE == objParam.AdminNdCode).Select(p => p.MAST_PARENT_ND_CODE).FirstOrDefault())).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();

                    //list of Receipt
                    lstReceiptCB = dbContext.SP_ACC_CASHBOOK_RECEIPT_SIDE(objParam.FundType, objParam.AdminNdCode, objParam.Month, objParam.Year, LevelId).ToList<SP_ACC_CASHBOOK_RECEIPT_SIDE_Result>();

                    cbSingle.OpeningBalace = dbContext.UDF_ACC_GEN_GET_BA_CASH_Opening_Balances(objParam.FundType, objParam.AdminNdCode, objParam.Month, objParam.Year, LevelId).FirstOrDefault();
                    cbSingle.ClosingBalace = dbContext.UDF_ACC_GEN_GET_BA_CASH_CLOSING_BALANCES(objParam.FundType, objParam.AdminNdCode, objParam.Month, objParam.Year, LevelId).FirstOrDefault();


                    cbSingle.SingleCB.ListReceiptCB = lstReceiptCB;

                    cbSingle.TotalRecCash = cbSingle.OpeningBalace.cash + cbSingle.SingleCB.ListReceiptCB.Sum(m => m.cash);
                    cbSingle.TotalRecBank = cbSingle.OpeningBalace.bank_auth + cbSingle.SingleCB.ListReceiptCB.Sum(m => m.bank_auth);

                    //list of Payment
                    lstPaymentCB = dbContext.SP_ACC_CASHBOOK_PAYMENT_SIDE(objParam.FundType, objParam.AdminNdCode, objParam.Month, objParam.Year, LevelId).ToList<SP_ACC_CASHBOOK_PAYMENT_SIDE_Result>();

                    cbSingle.SingleCB.ListPaymentCB = lstPaymentCB;

                    cbSingle.TotalPayCash = cbSingle.SingleCB.ListPaymentCB.Sum(m => m.cash);
                    cbSingle.TotalPayBank = cbSingle.SingleCB.ListPaymentCB.Sum(m => m.bank_auth);

                    cbSingle.Month = objParam.Month;
                    cbSingle.Year = objParam.Year;

                    //Report Header Information
                    var rptHeader = dbContext.SP_ACC_Get_Report_Header_Information("Reports", "CashBook", PMGSYSession.Current.FundType, LevelId, "O").ToList<SP_ACC_Get_Report_Header_Information_Result>().FirstOrDefault();
                    if (rptHeader == null)
                    {
                        cbSingle.ReportNumber = String.Empty;
                        cbSingle.ReportName = String.Empty;
                        cbSingle.ReportParaName = String.Empty;
                        cbSingle.FundType = objCommonFunc.GetFundName(PMGSYSession.Current.FundType);
                    }
                    else
                    {
                        cbSingle.ReportNumber = rptHeader.REPORT_FORM_NO;
                        cbSingle.ReportName = rptHeader.REPORT_NAME;
                        cbSingle.ReportParaName = rptHeader.REPORT_PARAGRAPH_NAME;
                        cbSingle.FundType = objCommonFunc.GetFundName(PMGSYSession.Current.FundType);
                    }
                    scope.Complete();
                    return cbSingle;
                }
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public CBReceiptModel ReceiptCashBook(ReportFilter objParam)
        {
            try
            {
                dbContext = new PMGSYEntities();
                using (var scope = new TransactionScope())
                {
                    CBReceiptModel cbReceiptModel = new CBReceiptModel();
                    List<SP_ACC_CASHBOOK_RECEIPT_SIDE_Result> lstReceiptCB = new List<SP_ACC_CASHBOOK_RECEIPT_SIDE_Result>();
                    lstReceiptCB = dbContext.SP_ACC_CASHBOOK_RECEIPT_SIDE(objParam.FundType, objParam.AdminNdCode, objParam.Month, objParam.Year, objParam.LevelId).ToList<SP_ACC_CASHBOOK_RECEIPT_SIDE_Result>();
                    cbReceiptModel.ListReceiptCB = lstReceiptCB;
                    cbReceiptModel.OpeningBalace = dbContext.UDF_ACC_GEN_GET_BA_CASH_Opening_Balances(objParam.FundType, objParam.AdminNdCode, objParam.Month, objParam.Year, objParam.LevelId).FirstOrDefault();
                    cbReceiptModel.TotalRecCash = cbReceiptModel.ListReceiptCB.Sum(m => m.cash);
                    cbReceiptModel.TotalRecBank = cbReceiptModel.ListReceiptCB.Sum(m => m.bank_auth);
                    scope.Complete();
                    return cbReceiptModel;
                }
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public CBPaymentModel PaymentCashBook(ReportFilter objParam)
        {
            try
            {
                dbContext = new PMGSYEntities();
                using (var scope = new TransactionScope())
                {
                    CBPaymentModel cbPaymentModel = new CBPaymentModel();
                    List<SP_ACC_CASHBOOK_PAYMENT_SIDE_Result> lstPaymentCB = new List<SP_ACC_CASHBOOK_PAYMENT_SIDE_Result>();
                    lstPaymentCB = dbContext.SP_ACC_CASHBOOK_PAYMENT_SIDE(objParam.FundType, objParam.AdminNdCode, objParam.Month, objParam.Year, objParam.LevelId).ToList<SP_ACC_CASHBOOK_PAYMENT_SIDE_Result>();
                    cbPaymentModel.ListPaymentCB = lstPaymentCB;
                    cbPaymentModel.ClosingBalace = dbContext.UDF_ACC_GEN_GET_BA_CASH_CLOSING_BALANCES(objParam.FundType, objParam.AdminNdCode, objParam.Month, objParam.Year, objParam.LevelId).FirstOrDefault();
                    cbPaymentModel.TotalPayCash = cbPaymentModel.ListPaymentCB.Sum(m => m.cash);
                    cbPaymentModel.TotalPayBank = cbPaymentModel.ListPaymentCB.Sum(m => m.bank_auth);
                    scope.Complete();
                    return cbPaymentModel;
                }
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        #endregion

        #region  Ledger

        /// <summary>
        /// Action to get credit debit head list
        /// </summary>
        /// <param name="creditDebit"> credit or debit </param>
        /// <param name="fundType"></param>
        /// <param name="op_Level"></param>
        /// <returns></returns>
        public List<SelectListItem> GetLedgerHeadList(String creditDebit,String fundType,List<short> op_Level,String SRRDA_DPIU)
        {
            try
            {
                dbContext = new PMGSYEntities();

                List<SelectListItem> headList = new List<SelectListItem>();

               /* List<ACC_MASTER_HEAD> ACC_MASTER_HEAD_LIST = new List<ACC_MASTER_HEAD>();

                ACC_MASTER_HEAD_LIST = dbContext.ACC_MASTER_HEAD.Where(c => c.FUND_TYPE == fundType && op_Level.Contains(c.OP_LVL_ID.Value)
                    && c.CREDIT_DEBIT == creditDebit && c.PARENT_HEAD_ID != null).ToList<ACC_MASTER_HEAD>();
                //exec [omms].[SP_ACC_LEDGER_GET_CREDIT_DEBIT_HEADS] 'P', 4, 'D'
              
                
                foreach (var item in ACC_MASTER_HEAD_LIST)
                {
                     headList.Add(new SelectListItem { Text = item.HEAD_CODE +" | " +item.HEAD_NAME, Value = item.HEAD_ID.ToString() });
                }


                
              */
                // Modified by Abhishek kamble 7-oct-2013
                short LevelId = 0;

                if (SRRDA_DPIU=="S")
                {
                    LevelId = 4;
                }
                else if (SRRDA_DPIU == "D")
                {
                    LevelId = 5;
                }
                else
                {
                    LevelId = PMGSYSession.Current.LevelId;
                }

                List<SP_ACC_LEDGER_GET_CREDIT_DEBIT_HEADS_Result> list =dbContext.SP_ACC_LEDGER_GET_CREDIT_DEBIT_HEADS(fundType, LevelId,creditDebit).ToList<SP_ACC_LEDGER_GET_CREDIT_DEBIT_HEADS_Result>();

                SelectListItem head = null;
                foreach (SP_ACC_LEDGER_GET_CREDIT_DEBIT_HEADS_Result result in list)
                {
                    head = new SelectListItem();
                    head.Selected = false;
                    head.Text = result.HEAD_NAME;
                    head.Value = result.HEAD_ID.ToString();

                    headList.Add(head);
                }

                headList.Insert(0, (new SelectListItem { Text = " -- Select Head -- ", Value = "0", Selected = true }));
                return headList;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

       
        /// <summary>
        /// DAL method to return credit / debit ledger 
        /// </summary>
        /// <param name="objParam"></param>
        /// <returns></returns>
        public List<LedgerAmountModel> GetCreditDebitModel(ReportFilter objParam)
        {
            try
            {
                List<LedgerAmountModel> ledgerModelList = new List<LedgerAmountModel>();
                LedgerAmountModel ledgerAmountModel = null;
               
                //new change done by Vikram on 31Dec2013
                objCommonFunc = new CommonFunctions();

                using (var scope = new TransactionScope())
                {
                    if (objParam.RoadStatus== 'C')
                    {
                        List<SP_ACC_RPT_LEDGER_DETAILS_COMPLETED_WORKS_Result> result = dbContext.SP_ACC_RPT_LEDGER_DETAILS_COMPLETED_WORKS(objParam.AdminNdCode, objParam.Month,objParam.Year,objParam.FundType,objParam.Head).ToList<SP_ACC_RPT_LEDGER_DETAILS_COMPLETED_WORKS_Result>();

                        foreach (SP_ACC_RPT_LEDGER_DETAILS_COMPLETED_WORKS_Result item in result)
                        {
                            ledgerAmountModel = new LedgerAmountModel();

                            ledgerAmountModel.BILL_DATE = item.bill_date;
                            ledgerAmountModel.TEO_NUMBER = item.BILL_NO;
                            //ledgerAmountModel.CREDIT_AMOUNT = String.Format(new CultureInfo("en-IN"), "{0:C}", item.CREDIT).Substring(1);
                            ledgerAmountModel.CREDIT_AMOUNT = objCommonFunc._IndianFormatAmount(item.CREDIT==null?"0":item.CREDIT.ToString());
                            //ledgerAmountModel.DEBIT_AMOUNT = String.Format(new CultureInfo("en-IN"), "{0:C}", item.DEBIT).Substring(1);
                            ledgerAmountModel.DEBIT_AMOUNT = objCommonFunc._IndianFormatAmount(item.DEBIT==null?"0":item.DEBIT.ToString());
                            ledgerAmountModel.OPENING_BALANCE = objCommonFunc._IndianFormatAmount(item.OB==null?"0":item.OB.ToString());
                            ledgerAmountModel.CREDIT_DEBIT_BALANCE = objCommonFunc._IndianFormatAmount(item.DIFF==null?"0":item.DIFF.ToString());
                            ledgerAmountModel.NARRATION = item.NARRATION.ToString();
                            ledgerModelList.Add(ledgerAmountModel);
                        }
                    }
                    else if(objParam.RoadStatus=='P')
                    {
                        List<SP_ACC_RPT_LEDGER_DETAILS_INPROGRESS_WORKS_Result> result = dbContext.SP_ACC_RPT_LEDGER_DETAILS_INPROGRESS_WORKS(objParam.AdminNdCode, objParam.Month, objParam.Year, objParam.FundType, objParam.Head).ToList<SP_ACC_RPT_LEDGER_DETAILS_INPROGRESS_WORKS_Result>();

                        foreach (SP_ACC_RPT_LEDGER_DETAILS_INPROGRESS_WORKS_Result item in result)
                        {
                            ledgerAmountModel = new LedgerAmountModel();

                            ledgerAmountModel.BILL_DATE = item.bill_date;
                            ledgerAmountModel.TEO_NUMBER = item.BILL_NO;
                            //ledgerAmountModel.CREDIT_AMOUNT = String.Format(new CultureInfo("en-IN"), "{0:C}", item.CREDIT).Substring(1);
                            ledgerAmountModel.CREDIT_AMOUNT = objCommonFunc._IndianFormatAmount(item.CREDIT==null?"0":item.CREDIT.ToString());
                            //ledgerAmountModel.DEBIT_AMOUNT = String.Format(new CultureInfo("en-IN"), "{0:C}", item.DEBIT).Substring(1);
                            ledgerAmountModel.DEBIT_AMOUNT = objCommonFunc._IndianFormatAmount(item.DEBIT==null?"0":item.DEBIT.ToString());
                            ledgerAmountModel.OPENING_BALANCE = objCommonFunc._IndianFormatAmount(item.OB==null?"0":item.OB.ToString());
                            ledgerAmountModel.CREDIT_DEBIT_BALANCE = objCommonFunc._IndianFormatAmount(item.DIFF==null?"0":item.DIFF.ToString());
                            ledgerAmountModel.NARRATION = item.NARRATION.ToString();
                            ledgerModelList.Add(ledgerAmountModel);
                        }
                    }
                    else if (objParam.LowerAdminNdCode == -1)
                    {
                        List<SP_ACC_RPT_LEDGER_DETAILS_Result> result = dbContext.SP_ACC_RPT_LEDGER_DETAILS(objParam.FundType, objParam.AdminNdCode, objParam.Month, objParam.Year, objParam.LevelId, objParam.Head, objParam.CreditDebit).ToList<SP_ACC_RPT_LEDGER_DETAILS_Result>();

                        foreach (SP_ACC_RPT_LEDGER_DETAILS_Result item in result)
                        {
                            ledgerAmountModel = new LedgerAmountModel();
             
                            ledgerAmountModel.BILL_DATE = item.bill_date;
                            ledgerAmountModel.TEO_NUMBER = item.BILL_NO;
                            //ledgerAmountModel.CREDIT_AMOUNT = String.Format(new CultureInfo("en-IN"), "{0:C}", item.CREDIT).Substring(1);
                            ledgerAmountModel.CREDIT_AMOUNT = objCommonFunc._IndianFormatAmount(item.CREDIT==null?"0":item.CREDIT.ToString());
                            //ledgerAmountModel.DEBIT_AMOUNT = String.Format(new CultureInfo("en-IN"), "{0:C}", item.DEBIT).Substring(1);
                            ledgerAmountModel.DEBIT_AMOUNT = objCommonFunc._IndianFormatAmount(item.DEBIT==null?"0":item.DEBIT.ToString());
                            ledgerAmountModel.OPENING_BALANCE = objCommonFunc._IndianFormatAmount(item.OB==null?"0":item.OB.ToString());
                            ledgerAmountModel.CREDIT_DEBIT_BALANCE = objCommonFunc._IndianFormatAmount(item.DIFF==null?"0":item.DIFF.ToString()); 
                            ledgerAmountModel.NARRATION = item.NARRATION.ToString();
                            ledgerModelList.Add(ledgerAmountModel);
                        }
                    }//|| objParam.CreditDebit == "C" condition added to disp all piu option for head P-6,A-77,M-333 21-June-2014
                    else if ((objParam.CreditDebit == "D" || objParam.CreditDebit == "C") && objParam.LowerAdminNdCode != -1)
                    {
                        List<SP_ACC_RPT_LEDGER_DETAILS_PIU_WISE_Result> result = dbContext.SP_ACC_RPT_LEDGER_DETAILS_PIU_WISE(objParam.FundType, objParam.AdminNdCode,objParam.LowerAdminNdCode, objParam.Month, objParam.Year,  objParam.Head, objParam.CreditDebit).ToList<SP_ACC_RPT_LEDGER_DETAILS_PIU_WISE_Result>();

                        foreach (SP_ACC_RPT_LEDGER_DETAILS_PIU_WISE_Result item in result)
                        {
                            ledgerAmountModel = new LedgerAmountModel();
                            ledgerAmountModel.BILL_DATE = item.bill_date;
                            ledgerAmountModel.TEO_NUMBER = item.BILL_NO;   //modified by abhishek kamble 9-dec-2013
                            ledgerAmountModel.CREDIT_AMOUNT = objCommonFunc._IndianFormatAmount(item.CREDIT.ToString());
                            ledgerAmountModel.DEBIT_AMOUNT = objCommonFunc._IndianFormatAmount(item.DEBIT.ToString());
                            ledgerAmountModel.OPENING_BALANCE = objCommonFunc._IndianFormatAmount(item.OB==null?"0":item.OB.ToString());
                            ledgerAmountModel.CREDIT_DEBIT_BALANCE = objCommonFunc._IndianFormatAmount(item.DIFF==null?"0":item.DIFF.ToString());
                            ledgerAmountModel.NARRATION = item.NARRATION.ToString();
                            ledgerModelList.Add(ledgerAmountModel);
                        }
                        var dpiuName = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == objParam.LowerAdminNdCode).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
                        ledgerAmountModel.DPIUName = dpiuName;
                        //ledgerModelList.Add(ledgerAmountModel);

                    }
                    //Modified by abhishek kamble 7-oct-2013
                    var lstHeader = dbContext.SP_ACC_Get_Report_Header_Information("Reports", "Ledger", PMGSYSession.Current.FundType,objParam.LevelId, objParam.CreditDebit).ToList<SP_ACC_Get_Report_Header_Information_Result>().FirstOrDefault();
                    ledgerAmountModel = new LedgerAmountModel();

                    if (lstHeader != null)
                    {
                        ledgerAmountModel.ReportNumber = lstHeader.REPORT_FORM_NO;
                        ledgerAmountModel.ReportName = lstHeader.REPORT_NAME;
                        ledgerAmountModel.ReporPara = lstHeader.REPORT_PARAGRAPH_NAME;
                        ledgerAmountModel.FundType = objCommonFunc.GetFundName(PMGSYSession.Current.FundType);
                    }

                    ledgerModelList.Add(ledgerAmountModel);         
                    scope.Complete();

                    return ledgerModelList;
                }
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// DAL TO Return the name of  PIU and nodal agency
        /// </summary>
        /// <param name="objParam"></param>
        /// <returns></returns>
        public LedgerModel GetForContextData(ReportFilter objParam)
        {
             try {

                  dbContext = new PMGSYEntities();
                     
                  LedgerModel model = new LedgerModel();
                  
                        //Modified by Abhishek kamble 7-oct-2013
                        if ((objParam.LevelId == 5)|| (objParam.LowerAdminNdCode!=-1))
                        {
                           var department = (from distDepartment in dbContext.ADMIN_DEPARTMENT
                                       join stateDepartment in dbContext.ADMIN_DEPARTMENT
                                       on distDepartment.MAST_PARENT_ND_CODE equals stateDepartment.ADMIN_ND_CODE
                                       where distDepartment.ADMIN_ND_CODE == objParam.AdminNdCode
                                              select new { DistrictDepartment = distDepartment.ADMIN_ND_NAME, StateDepartment = stateDepartment.ADMIN_ND_NAME }).FirstOrDefault();
                                                                   
                           //Modified by Abhishek kamble 2-jan-2014
                           if (department != null)
                           {
                               model.DistrictDepartment = department.DistrictDepartment;
                               model.StateDepartment = department.StateDepartment;
                           }
                            if (objParam.Selection == "S")
                            {
                                model.DistrictDepartment = "-";
                            }
                        }
                        else
                        {
                            model.StateDepartment = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == objParam.AdminNdCode).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
                            model.DistrictDepartment = "-";
                        }

                      return model;
                    }
                    catch (Exception Ex)
                    {
                        throw Ex;
                    }
                    finally
                    {
                        dbContext.Dispose();
                    }
                 }

        #endregion

        #region MonthlyAccount
        public string GetNodalAgency(Int32 AdminNdCode)
        {
            try
            {
                dbContext = new PMGSYEntities();
                return dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == AdminNdCode).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            finally
            {
                dbContext.Dispose();
            }

        }

        public List<USP_RPT_SHOW_MONTHLY_ACCOUNT_SELF_Result> GetMonthlyAccountList(ReportFilter objParam)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<USP_RPT_SHOW_MONTHLY_ACCOUNT_SELF_Result> lstMonthlyAccount = new List<USP_RPT_SHOW_MONTHLY_ACCOUNT_SELF_Result>();
                lstMonthlyAccount = dbContext.USP_RPT_SHOW_MONTHLY_ACCOUNT_SELF(objParam.AdminNdCode, objParam.Month, objParam.Year, objParam.FundType, objParam.CreditDebit).ToList<USP_RPT_SHOW_MONTHLY_ACCOUNT_SELF_Result>();
                return lstMonthlyAccount;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        //added by abhishek 10-9-2013
        public List<USP_RPT_SHOW_MONTHLY_ACCOUNT_ALLPIU_Result> GetMonthlyAccountForAllPIU(ReportFilter objParam)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<USP_RPT_SHOW_MONTHLY_ACCOUNT_ALLPIU_Result> lstMonthlyAccountForAllPIU = new List<USP_RPT_SHOW_MONTHLY_ACCOUNT_ALLPIU_Result>();
                lstMonthlyAccountForAllPIU = dbContext.USP_RPT_SHOW_MONTHLY_ACCOUNT_ALLPIU(objParam.AdminNdCode, objParam.Month, objParam.Year, objParam.FundType, objParam.CreditDebit).ToList<USP_RPT_SHOW_MONTHLY_ACCOUNT_ALLPIU_Result>();
                return lstMonthlyAccountForAllPIU;
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


        public List<SelectListItem> PopulateStateSRRDA()
        {
            try
            {

                List<SelectListItem> lstStateSRRDA = new List<SelectListItem>();

                //List<SelectListItem> lstSRRDA = null;

                //lstSRRDA = new SelectList(dbContext.ADMIN_DEPARTMENT.Where(m => m.MAST_STATE_CODE == stateCode && m.MAST_ND_TYPE == "S").OrderBy(o => o.ADMIN_ND_NAME), "ADMIN_ND_CODE", "ADMIN_ND_NAME").ToList();

            //   lstSRRDA.Insert(0, new SelectListItem { Text="Select SRRDA"});


                var StateSrrda =
                (from adminDept in dbContext.ADMIN_DEPARTMENT
                 join State in dbContext.MASTER_STATE
                 on adminDept.MAST_STATE_CODE equals State.MAST_STATE_CODE
                 where adminDept.MAST_ND_TYPE == "S"
                 orderby State.MAST_STATE_NAME
                 select new { 
                    adminDept.ADMIN_ND_CODE,
                    adminDept.ADMIN_ND_NAME,
                    State.MAST_STATE_NAME
                 }
                );


                foreach (var item in StateSrrda)
                {
                    lstStateSRRDA.Add(new SelectListItem { Value=item.ADMIN_ND_CODE.ToString().Trim(),Text=item.MAST_STATE_NAME+"("+item.ADMIN_ND_NAME+")"});
                }

                return lstStateSRRDA;
            }

            catch {
                return null;            
            }
        }

        #endregion
        
        #region Transfer Entry Order
        public RptTrnasferEntryOrderList GetTransferEntryOrderListDAL(RptTransferEntryOrder teo, ReportFilter objParam)
        {
            dbContext = new PMGSYEntities();
            RptTrnasferEntryOrderList rptTeoList = new RptTrnasferEntryOrderList();
            rptTeoList.ListTeo=dbContext.SP_ACC_RPT_DISPLAY_TEO_DETAILS(teo.FundType, teo.AdminNDCode, teo.Month, teo.Year).ToList<SP_ACC_RPT_DISPLAY_TEO_DETAILS_Result>();
            var headerBalanceSheet = dbContext.SP_ACC_Get_Report_Header_Information("REPORTS", "TransferEntryOrder", objParam.FundType, objParam.LevelId,"O").ToList<SP_ACC_Get_Report_Header_Information_Result>().FirstOrDefault();
            try
            {
                if (headerBalanceSheet == null)
                {
                    rptTeoList.FormNumber = String.Empty;
                    rptTeoList.ReportName = String.Empty;
                    rptTeoList.Paragraph = String.Empty;
                }
                else {
                    rptTeoList.FormNumber = headerBalanceSheet.REPORT_FORM_NO;
                    rptTeoList.ReportName = headerBalanceSheet.REPORT_NAME;
                    rptTeoList.Paragraph = headerBalanceSheet.REPORT_PARAGRAPH_NAME;
                }

            }
            catch { }
            return rptTeoList;
        }
        #endregion Transfer Entry Order
        
        #region BalanceSheet
        public BalanceSheetList GetBalanceSheetDAL(BalanceSheet balanceSheet, ReportFilter objParam)
        {
            dbContext = new PMGSYEntities();
            BalanceSheetList balanceSheetList = new BalanceSheetList();
            NumberFormatInfo nfo = new NumberFormatInfo();
            nfo.CurrencyGroupSeparator = ",";
            nfo.CurrencyGroupSizes = new int[] { 3, 2 };

            //new added

            if (balanceSheet.ReportLevel == 'O')
            {
                balanceSheetList.Type = "SRRDA";
            }
            else if (balanceSheet.ReportLevel == 'S')
            {
                balanceSheetList.Type = "State";
            }
            else
            {
                balanceSheetList.Type = "PIU";
            }            

            decimal? liabilityCurrentYearTotalAmount = 0;
            decimal? liabilityPreviousYearTotalAmount = 0;
            decimal? assetCurrentYearTotalAmount = 0;
            decimal? assetPreviousYearTotalAmount = 0;
            string fundLiabilities = string.Empty;
            string fundAssets = string.Empty;


            //CommonFunctions objCommonFunction = new CommonFunctions();

            try
            {
                balanceSheetList.ListBalanceSheet = dbContext.USP_RPT_SHOW_BALSHEET(balanceSheet.ReportNumber, objParam.AdminNdCode, balanceSheet.Month, balanceSheet.Year, balanceSheet.ReportDPIU).ToList<USP_RPT_SHOW_BALSHEET_Result>();

                var headerBalanceSheet = dbContext.SP_ACC_Get_Report_Header_Information("REPORTS", "MaintenanceFundBalanceSheet", objParam.FundType, objParam.LevelId, balanceSheet.ReportLevel.ToString()).ToList<SP_ACC_Get_Report_Header_Information_Result>().FirstOrDefault();
                try
                {
                    if (headerBalanceSheet == null)
                    {
                        balanceSheetList.ReportFormNumber = String.Empty;
                        balanceSheetList.ReportHeader = String.Empty;
                        balanceSheetList.Section = String.Empty;
                    }
                    else
                    {
                        balanceSheetList.ReportFormNumber = headerBalanceSheet.REPORT_FORM_NO;
                        balanceSheetList.ReportHeader = headerBalanceSheet.REPORT_NAME;
                        balanceSheetList.Section = headerBalanceSheet.REPORT_PARAGRAPH_NAME;
                        balanceSheetList.FundType = objCommonFunc.GetFundName(PMGSYSession.Current.FundType);
                    }

                }
                catch { }

                balanceSheetList.Year = balanceSheet.Year;
                
                //commented by Vikram as Exception was occuring when yearly is selected
                //balanceSheetList.MonthName = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(balanceSheet.Month);

                if (balanceSheet.Month != 0)
                {
                    balanceSheetList.MonthName = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(balanceSheet.Month);
                }

                balanceSheetList.SelectionHeader = " Name Of SRRDA - Rural Works Organisation";
                if (objParam.LevelId == 5)
                {
                    balanceSheetList.SelectionHeader += "<br /> Name Of DPIU - Rural Works Organisation";
                }


                balanceSheetList.SelectionHeader += "<br /> Balance Sheet as at " + balanceSheetList.MonthName + "  -  " + balanceSheetList.Year;


                if(objParam.FundType=="A")
                {
                    fundLiabilities = "Administrative Fund Liabilities";
                    fundAssets = "Administrative Fund Assets";
                    
                }
                else  if(objParam.FundType=="P")
                {
                    fundLiabilities = "Programme Fund Liabilities";
                    fundAssets = "Programme Fund Assets";
                }
                else  if(objParam.FundType=="M")
                {
                    fundLiabilities = "Maintenance Fund Liabilities";
                    fundAssets = "Maintenance Fund Assets";
                }

                List<USP_RPT_SHOW_BALSHEET_Result> liabilitiesList = balanceSheetList.ListBalanceSheet.Where(bs => bs.GROUP_ID == "L").ToList<USP_RPT_SHOW_BALSHEET_Result>();
                liabilityCurrentYearTotalAmount = liabilitiesList.Sum(l => (decimal?)l.CURRENT_AMT);
                liabilityPreviousYearTotalAmount = liabilitiesList.Sum(l => (decimal?)l.PREVIOUS_AMT);
                
                liabilitiesList.Add(new USP_RPT_SHOW_BALSHEET_Result()
                {
                    GROUP_ID = "L",
                    ITEM_ID = -1,
                    ITEM_HEADING = "Liabilities Total",
                    CURRENT_AMT = liabilityCurrentYearTotalAmount,
                    PREVIOUS_AMT = liabilityPreviousYearTotalAmount,
                    LINK = ""

                });

                liabilitiesList.Insert(0, new USP_RPT_SHOW_BALSHEET_Result()
                {
                    GROUP_ID = "L",
                    ITEM_ID = 0,
                    ITEM_HEADING = fundLiabilities,
                    CURRENT_AMT = null,
                    PREVIOUS_AMT = null,
                    LINK = ""
                });

                List<USP_RPT_SHOW_BALSHEET_Result> assetList = balanceSheetList.ListBalanceSheet.Where(bs => bs.GROUP_ID == "A").ToList<USP_RPT_SHOW_BALSHEET_Result>();
                assetCurrentYearTotalAmount = assetList.Sum(a => (decimal?)a.CURRENT_AMT);
                assetPreviousYearTotalAmount = assetList.Sum(a => (decimal?)a.PREVIOUS_AMT);
                assetList.Add(new USP_RPT_SHOW_BALSHEET_Result()
                {
                    GROUP_ID = "A",
                    ITEM_ID = -1,
                    ITEM_HEADING = "Assets Total",
                    CURRENT_AMT = assetCurrentYearTotalAmount,
                    PREVIOUS_AMT = assetPreviousYearTotalAmount,
                    LINK = ""
                });

                assetList.Insert(0, new USP_RPT_SHOW_BALSHEET_Result()
                {
                    GROUP_ID = "A",
                    ITEM_ID = 0,
                    ITEM_HEADING = fundAssets,
                    CURRENT_AMT = null,
                    PREVIOUS_AMT = null,
                    LINK = ""
                });

                balanceSheetList.ListBalanceSheet.Clear();
                balanceSheetList.ListBalanceSheet.AddRange(liabilitiesList);
                balanceSheetList.ListBalanceSheet.AddRange(assetList);

                return balanceSheetList;
            }
            catch (Exception Ex)
            {
                return balanceSheetList;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }

        }

        public string GetDepartmentName(int adminCode)
        {
            dbContext = new PMGSYEntities();
            try
            {
                return dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == adminCode).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
            }
            catch (Exception)
            {
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
        #endregion BalanceSheet

        //#region ProgramFund
        //public ProgramFundBalanceSheetList GetProgramFundBalanceSheetDAL(ProgramFundBalanceSheet programFundBalanceSheet, ReportFilter objParam)
        //{
        //    dbContext = new PMGSYEntities();
        //    ProgramFundBalanceSheetList programFundBalanceSheetList = new ProgramFundBalanceSheetList();

        //    decimal? liabilityCurrentYearTotalAmount = 0;
        //    decimal? liabilityPreviousYearTotalAmount = 0;
        //    decimal? assetCurrentYearTotalAmount = 0;
        //    decimal? assetPreviousYearTotalAmount = 0;

        //    try
        //    {
        //        if (objParam.LevelId == 4)
        //        {
        //            if (programFundBalanceSheet.RptType == 2)
        //            {
        //                programFundBalanceSheetList.ListBalanceSheet = dbContext.USP_RPT_SHOW_BALSHEET(programFundBalanceSheet.RptType, objParam.AdminNdCode, programFundBalanceSheet.MONTH, programFundBalanceSheet.YEAR, 0).ToList<USP_RPT_SHOW_BALSHEET_Result>();
        //                programFundBalanceSheetList.Type = "SRRDA";
        //            }
        //            else if (programFundBalanceSheet.RptType == 9)
        //            {

        //                programFundBalanceSheetList.ListBalanceSheet = dbContext.USP_RPT_SHOW_BALSHEET(programFundBalanceSheet.RptType, objParam.AdminNdCode, programFundBalanceSheet.MONTH, programFundBalanceSheet.YEAR, 0).ToList<USP_RPT_SHOW_BALSHEET_Result>();
        //                programFundBalanceSheetList.Type = "State";
        //            }
        //            else if (programFundBalanceSheet.RptType == 8)
        //            {
        //                programFundBalanceSheetList.ListBalanceSheet = dbContext.USP_RPT_SHOW_BALSHEET(programFundBalanceSheet.RptType, objParam.AdminNdCode, programFundBalanceSheet.MONTH, programFundBalanceSheet.YEAR, 1).ToList<USP_RPT_SHOW_BALSHEET_Result>();
        //                programFundBalanceSheetList.Type = "AllDPIU";


        //            }
        //        }
        //        else if (objParam.LevelId == 5)
        //        {
        //            programFundBalanceSheetList.ListBalanceSheet = dbContext.USP_RPT_SHOW_BALSHEET(8, objParam.AdminNdCode, programFundBalanceSheet.MONTH, programFundBalanceSheet.YEAR, 0).ToList<USP_RPT_SHOW_BALSHEET_Result>();
        //            programFundBalanceSheetList.Type = "DPIU";
        //        }


        //        List<USP_RPT_SHOW_BALSHEET_Result> liabilitiesList = programFundBalanceSheetList.ListBalanceSheet.Where(bs => bs.GROUP_ID == "L").ToList<USP_RPT_SHOW_BALSHEET_Result>();
        //        liabilityCurrentYearTotalAmount = liabilitiesList.Sum(l => (decimal?)l.CURRENT_AMT);
        //        liabilityPreviousYearTotalAmount = liabilitiesList.Sum(l => (decimal?)l.PREVIOUS_AMT);

        //        liabilitiesList.Add(new USP_RPT_SHOW_BALSHEET_Result()
        //        {
        //            GROUP_ID = "L",
        //            ITEM_ID = -1,
        //            ITEM_HEADING = "Total Funds and Liabilities",
        //            CURRENT_AMT = liabilityCurrentYearTotalAmount,
        //            PREVIOUS_AMT = liabilityPreviousYearTotalAmount,
        //            LINK = ""

        //        });

        //        liabilitiesList.Insert(0, new USP_RPT_SHOW_BALSHEET_Result()
        //        {
        //            GROUP_ID = "L",
        //            ITEM_ID = -1,
        //            ITEM_HEADING = "PROGRAM FUNDS LIABILITIES",
        //            CURRENT_AMT = null,
        //            PREVIOUS_AMT = null,
        //            LINK = ""
        //        });

        //        List<USP_RPT_SHOW_BALSHEET_Result> assetList = programFundBalanceSheetList.ListBalanceSheet.Where(bs => bs.GROUP_ID == "A").ToList<USP_RPT_SHOW_BALSHEET_Result>();
        //        assetCurrentYearTotalAmount = assetList.Sum(a => (decimal?)a.CURRENT_AMT);
        //        assetPreviousYearTotalAmount = assetList.Sum(a => (decimal?)a.PREVIOUS_AMT);

        //        assetList.Add(new USP_RPT_SHOW_BALSHEET_Result()
        //        {
        //            GROUP_ID = "A",
        //            ITEM_ID = -1,
        //            ITEM_HEADING = "Total",
        //            CURRENT_AMT = assetCurrentYearTotalAmount,
        //            PREVIOUS_AMT = assetPreviousYearTotalAmount,
        //            LINK = ""
        //        });

        //        assetList.Insert(0, new USP_RPT_SHOW_BALSHEET_Result()
        //        {
        //            GROUP_ID = "A",
        //            ITEM_ID = -1,
        //            ITEM_HEADING = "ASSETS",
        //            CURRENT_AMT = null,
        //            PREVIOUS_AMT = null,
        //            LINK = ""
        //        });

        //        programFundBalanceSheetList.ListBalanceSheet.Clear();
        //        programFundBalanceSheetList.ListBalanceSheet.AddRange(liabilitiesList);
        //        programFundBalanceSheetList.ListBalanceSheet.AddRange(assetList);

        //        return programFundBalanceSheetList;
        //    }
        //    catch (Exception Ex)
        //    {
        //        return programFundBalanceSheetList;
        //    }
        //    finally
        //    {
        //        if (dbContext != null)
        //        {
        //            dbContext.Dispose();
        //        }
        //    }

        //}
        //#endregion ProgramFund

        //#region AdminFund
        //public AdminFundBalanceSheetList GetAdminFundBalanceSheetDAL(AdminFundBalanceSheet adminFundBalanceSheet, ReportFilter objParam)
        //{
        //    dbContext = new PMGSYEntities();
        //    AdminFundBalanceSheetList adminFundBalanceSheetList = new AdminFundBalanceSheetList();

        //    decimal? liabilityCurrentYearTotalAmount = 0;
        //    decimal? liabilityPreviousYearTotalAmount = 0;
        //    decimal? assetCurrentYearTotalAmount = 0;
        //    decimal? assetPreviousYearTotalAmount = 0;

        //    try
        //    {
        //        if (objParam.LevelId == 4)
        //        {
        //            if (adminFundBalanceSheet.RptType == 2)
        //            {
        //                adminFundBalanceSheetList.ListBalanceSheet = dbContext.USP_RPT_SHOW_BALSHEET(adminFundBalanceSheet.RptType, objParam.AdminNdCode, adminFundBalanceSheet.MONTH, adminFundBalanceSheet.YEAR, 0).ToList<USP_RPT_SHOW_BALSHEET_Result>();
        //                adminFundBalanceSheetList.Type = "SRRDA";
        //            }
        //            else if (adminFundBalanceSheet.RptType == 9)
        //            {

        //                adminFundBalanceSheetList.ListBalanceSheet = dbContext.USP_RPT_SHOW_BALSHEET(adminFundBalanceSheet.RptType, objParam.AdminNdCode, adminFundBalanceSheet.MONTH, adminFundBalanceSheet.YEAR, 0).ToList<USP_RPT_SHOW_BALSHEET_Result>();
        //                adminFundBalanceSheetList.Type = "State";
        //            }
        //            else if (adminFundBalanceSheet.RptType == 8)
        //            {
        //                adminFundBalanceSheetList.ListBalanceSheet = dbContext.USP_RPT_SHOW_BALSHEET(adminFundBalanceSheet.RptType, objParam.AdminNdCode, adminFundBalanceSheet.MONTH, adminFundBalanceSheet.YEAR, 1).ToList<USP_RPT_SHOW_BALSHEET_Result>();
        //                adminFundBalanceSheetList.Type = "AllDPIU";


        //            }
        //        }
        //        else if (objParam.LevelId == 5)
        //        {
        //            adminFundBalanceSheetList.ListBalanceSheet = dbContext.USP_RPT_SHOW_BALSHEET(6, objParam.AdminNdCode, adminFundBalanceSheet.MONTH, adminFundBalanceSheet.YEAR, 0).ToList<USP_RPT_SHOW_BALSHEET_Result>();
        //            adminFundBalanceSheetList.Type = "DPIU";
        //        }


        //        List<USP_RPT_SHOW_BALSHEET_Result> liabilitiesList = adminFundBalanceSheetList.ListBalanceSheet.Where(bs => bs.GROUP_ID == "L").ToList<USP_RPT_SHOW_BALSHEET_Result>();
        //        liabilityCurrentYearTotalAmount = liabilitiesList.Sum(l => (decimal?)l.CURRENT_AMT);
        //        liabilityPreviousYearTotalAmount = liabilitiesList.Sum(l => (decimal?)l.PREVIOUS_AMT);

        //        liabilitiesList.Add(new USP_RPT_SHOW_BALSHEET_Result()
        //        {
        //            GROUP_ID = "L",
        //            ITEM_ID = -1,
        //            ITEM_HEADING = "Total Funds and Liabilities",
        //            CURRENT_AMT = liabilityCurrentYearTotalAmount,
        //            PREVIOUS_AMT = liabilityPreviousYearTotalAmount,
        //            LINK = ""

        //        });

        //        liabilitiesList.Insert(0, new USP_RPT_SHOW_BALSHEET_Result()
        //        {
        //            GROUP_ID = "L",
        //            ITEM_ID = -1,
        //            ITEM_HEADING = "ADMIN FUNDS LIABILITIES",
        //            CURRENT_AMT = null,
        //            PREVIOUS_AMT = null,
        //            LINK = ""
        //        });

        //        List<USP_RPT_SHOW_BALSHEET_Result> assetList = adminFundBalanceSheetList.ListBalanceSheet.Where(bs => bs.GROUP_ID == "A").ToList<USP_RPT_SHOW_BALSHEET_Result>();
        //        assetCurrentYearTotalAmount = assetList.Sum(a => (decimal?)a.CURRENT_AMT);
        //        assetPreviousYearTotalAmount = assetList.Sum(a => (decimal?)a.PREVIOUS_AMT);

        //        assetList.Add(new USP_RPT_SHOW_BALSHEET_Result()
        //        {
        //            GROUP_ID = "A",
        //            ITEM_ID = -1,
        //            ITEM_HEADING = "Total",
        //            CURRENT_AMT = assetCurrentYearTotalAmount,
        //            PREVIOUS_AMT = assetPreviousYearTotalAmount,
        //            LINK = ""
        //        });

        //        assetList.Insert(0, new USP_RPT_SHOW_BALSHEET_Result()
        //        {
        //            GROUP_ID = "A",
        //            ITEM_ID = -1,
        //            ITEM_HEADING = "ASSETS",
        //            CURRENT_AMT = null,
        //            PREVIOUS_AMT = null,
        //            LINK = ""
        //        });

        //        adminFundBalanceSheetList.ListBalanceSheet.Clear();
        //        adminFundBalanceSheetList.ListBalanceSheet.AddRange(liabilitiesList);
        //        adminFundBalanceSheetList.ListBalanceSheet.AddRange(assetList);

        //        return adminFundBalanceSheetList;
        //    }
        //    catch (Exception Ex)
        //    {
        //        return adminFundBalanceSheetList;
        //    }
        //    finally
        //    {
        //        if (dbContext != null)
        //        {
        //            dbContext.Dispose();
        //        }
        //    }

        //}
        //#endregion AdminFund



        //#region MaintenanceFundBalanceFund
        //public MaintenanceFundBalanceSheetList GetMaintenanceFundBalanceSheetDAL(MaintenanceFundBalanceSheet maintenanceFundBalanceSheet, ReportFilter objParam)
        //{
        //    dbContext = new PMGSYEntities();
        //    MaintenanceFundBalanceSheetList maintenanceFundBalanceSheetList = new MaintenanceFundBalanceSheetList();

        //    decimal? liabilityCurrentYearTotalAmount = 0;
        //    decimal? liabilityPreviousYearTotalAmount = 0;
        //    decimal? assetCurrentYearTotalAmount = 0;
        //    decimal? assetPreviousYearTotalAmount = 0;
               
        //    try
        //    {
        //        if (objParam.LevelId == 4)
        //        {
        //            if (maintenanceFundBalanceSheet.RptType==4)
        //            {
        //                maintenanceFundBalanceSheetList.ListBalanceSheet = dbContext.USP_RPT_SHOW_BALSHEET(4, objParam.AdminNdCode, maintenanceFundBalanceSheet.MONTH, maintenanceFundBalanceSheet.YEAR, 0).ToList<USP_RPT_SHOW_BALSHEET_Result>();
        //                maintenanceFundBalanceSheetList.Type = "SRRDA";
        //            }
        //            else if (maintenanceFundBalanceSheet.RptType==1)
        //            {
                       
        //                    maintenanceFundBalanceSheetList.ListBalanceSheet = dbContext.USP_RPT_SHOW_BALSHEET(1, objParam.AdminNdCode, maintenanceFundBalanceSheet.MONTH, maintenanceFundBalanceSheet.YEAR, 0).ToList<USP_RPT_SHOW_BALSHEET_Result>();
        //                    maintenanceFundBalanceSheetList.Type = "State";
        //            }
        //            else if (maintenanceFundBalanceSheet.RptType == 3)
        //            {
        //                maintenanceFundBalanceSheetList.ListBalanceSheet = dbContext.USP_RPT_SHOW_BALSHEET(3, objParam.AdminNdCode, maintenanceFundBalanceSheet.MONTH, maintenanceFundBalanceSheet.YEAR, 1).ToList<USP_RPT_SHOW_BALSHEET_Result>();
        //                maintenanceFundBalanceSheetList.Type = "AllDPIU";


        //            }
        //        }
        //        else if (objParam.LevelId == 5)
        //        {
        //            maintenanceFundBalanceSheetList.ListBalanceSheet = dbContext.USP_RPT_SHOW_BALSHEET(3, objParam.AdminNdCode, maintenanceFundBalanceSheet.MONTH, maintenanceFundBalanceSheet.YEAR, 0).ToList<USP_RPT_SHOW_BALSHEET_Result>();
        //            maintenanceFundBalanceSheetList.Type = "DPIU";
        //        }


        //        List<USP_RPT_SHOW_BALSHEET_Result> liabilitiesList = maintenanceFundBalanceSheetList.ListBalanceSheet.Where(bs => bs.GROUP_ID == "L").ToList<USP_RPT_SHOW_BALSHEET_Result>();
        //        liabilityCurrentYearTotalAmount = liabilitiesList.Sum(l => (decimal?)l.CURRENT_AMT);
        //        liabilityPreviousYearTotalAmount = liabilitiesList.Sum(l => (decimal?)l.PREVIOUS_AMT);
                
        //        liabilitiesList.Add(new USP_RPT_SHOW_BALSHEET_Result() { 
        //        GROUP_ID="L",
        //        ITEM_ID=-1,
        //        ITEM_HEADING = "Total Funds and Liabilities",
        //        CURRENT_AMT = liabilityCurrentYearTotalAmount,
        //        PREVIOUS_AMT = liabilityPreviousYearTotalAmount,
        //        LINK = ""

        //        });

        //        liabilitiesList.Insert(0, new USP_RPT_SHOW_BALSHEET_Result()
        //        {
        //            GROUP_ID = "L",
        //            ITEM_ID = -1,
        //            ITEM_HEADING = "MAINTENANCE FUNDS LIABILITIES",
        //            CURRENT_AMT = null,
        //            PREVIOUS_AMT = null,
        //            LINK = ""
        //        });

        //        List<USP_RPT_SHOW_BALSHEET_Result> assetList = maintenanceFundBalanceSheetList.ListBalanceSheet.Where(bs => bs.GROUP_ID == "A").ToList<USP_RPT_SHOW_BALSHEET_Result>();
        //        assetCurrentYearTotalAmount = assetList.Sum(a => (decimal?)a.CURRENT_AMT);
        //        assetPreviousYearTotalAmount = assetList.Sum(a => (decimal?)a.PREVIOUS_AMT);

        //        assetList.Add(new USP_RPT_SHOW_BALSHEET_Result()
        //        {
        //            GROUP_ID = "A",
        //            ITEM_ID = -1,
        //            ITEM_HEADING = "Total",
        //            CURRENT_AMT = assetCurrentYearTotalAmount,
        //            PREVIOUS_AMT = assetPreviousYearTotalAmount,
        //            LINK = ""
        //        });

        //        assetList.Insert(0,new USP_RPT_SHOW_BALSHEET_Result()
        //        {
        //            GROUP_ID = "A",
        //            ITEM_ID = -1,
        //            ITEM_HEADING = "ASSETS",
        //            CURRENT_AMT = null,
        //            PREVIOUS_AMT = null,
        //            LINK = ""
        //        });

        //        maintenanceFundBalanceSheetList.ListBalanceSheet.Clear();
        //        maintenanceFundBalanceSheetList.ListBalanceSheet.AddRange(liabilitiesList);
        //        maintenanceFundBalanceSheetList.ListBalanceSheet.AddRange(assetList);

        //        return maintenanceFundBalanceSheetList;
        //    }
        //    catch (Exception Ex)
        //    {
        //        return maintenanceFundBalanceSheetList;
        //    }
        //    finally
        //    {
        //        if (dbContext != null)
        //        {
        //            dbContext.Dispose();
        //        }
        //    }

        //}
        //#endregion MaintenanceFundBalanceFund

        #region RegisterOFWorks


        public RegisterOfWorksModel RegisterOfWorksDAL(TransactionParams objparams)
        {
            var dbContext = new PMGSYEntities();
            RegisterOfWorksModel registerOfWorksModel = new RegisterOfWorksModel();
            try
            {
                //Header Info
                SP_ACC_RPT_PF_WORK_REGISTER_HEADER_INFORMATION_Result result = dbContext.SP_ACC_RPT_PF_WORK_REGISTER_HEADER_INFORMATION(objparams.FUND_TYPE,
                                                                                        objparams.ADMIN_ND_CODE, objparams.MAST_CONT_ID, objparams.AGREEMENT_CODE).FirstOrDefault();

                if (result != null)
                {

                    if (objparams.LVL_ID == 4)    //State
                    {
                        registerOfWorksModel.StateDepartment = result.Piu_Name;
                    }
                    else if (objparams.LVL_ID == 5)    //District
                    {
                        registerOfWorksModel.DistrictDepartment = result.Piu_Name;
                    }

                    registerOfWorksModel.AGREEMENT_DATE = result.Agreement_Date;
                    registerOfWorksModel.AGREEMENT_AMOUNT = result.Amount;
                    registerOfWorksModel.FUNDING_AGENCY = result.Funding_Agency;
                    registerOfWorksModel.AGREEMENT_NUMBER = result.Agreement_number;

                }

                registerOfWorksModel.ADMIN_ND_CODE = objparams.ADMIN_ND_CODE;
                registerOfWorksModel.MAST_CON_ID = objparams.MAST_CONT_ID;
                registerOfWorksModel.TEND_AGREEMENT_CODE = objparams.AGREEMENT_CODE;

                //Set Report Header        

                CommonFunctions objCommonFunction = new CommonFunctions();
                //registerOfWorksModel.NodalAgency = objReportBAL.GetNodalAgency(PMGSYSession.Current.AdminNdCode);
                var ReportHeader = dbContext.SP_ACC_Get_Report_Header_Information("Reports", "RegisterOfWorks", PMGSYSession.Current.FundType, PMGSYSession.Current.LevelId, "O").ToList<SP_ACC_Get_Report_Header_Information_Result>().FirstOrDefault();
                if (ReportHeader == null)
                {
                    registerOfWorksModel.FundTypeName = objCommonFunction.GetFundName(PMGSYSession.Current.FundType); ;
                    registerOfWorksModel.ReportName = String.Empty;
                    registerOfWorksModel.ReportParagraphName = String.Empty;
                    registerOfWorksModel.ReportFormNumber = String.Empty;
                }
                else {
                    registerOfWorksModel.FundTypeName = objCommonFunction.GetFundName(PMGSYSession.Current.FundType); ;
                    registerOfWorksModel.ReportName = ReportHeader.REPORT_NAME;
                    registerOfWorksModel.ReportParagraphName = ReportHeader.REPORT_PARAGRAPH_NAME;
                    registerOfWorksModel.ReportFormNumber = ReportHeader.REPORT_FORM_NO;                
                }

                return registerOfWorksModel;
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


        public Array RegisterOfWorksHeaderGridDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int adminNDCode, int contractorID, int agreementCode)
        {
            dbContext = new PMGSYEntities();
            List<SP_ACC_RPT_PF_WORK_REGISTER_Result> itemList = new List<SP_ACC_RPT_PF_WORK_REGISTER_Result>();

            try
            {
                itemList = dbContext.SP_ACC_RPT_PF_WORK_REGISTER(PMGSYSession.Current.FundType, adminNDCode, contractorID, agreementCode).ToList<SP_ACC_RPT_PF_WORK_REGISTER_Result>();
                totalRecords = itemList.Count();

                return itemList.Select(itemDetails => new
                {
                    cell = new[] {         
                                    itemDetails.IMS_ROAD_NAME, 
                                    (itemDetails.BILL_MONTH + " " + itemDetails.BILL_YEAR.ToString()) ,
                                    (itemDetails.BILL_NO).ToUpper().Equals("ZZZZ")? "<b><font color='green'>Total</b></font>": itemDetails.BILL_NO,
                                    (itemDetails.BILL_NO).ToUpper().Equals("ZZZZ")? "":itemDetails.BILL_DATE.ToString(),
                                    (itemDetails.BILL_NO).ToUpper().Equals("ZZZZ")? "<b><font color='green'>" + itemDetails.NARRATION + "</b></font>" : itemDetails.NARRATION,
                                    (itemDetails.BILL_NO).ToUpper().Equals("ZZZZ")? "<b><font color='green'>" + itemDetails.Total.ToString() + "</b></font>" : itemDetails.AMOUNT.ToString()
                        }
                }).ToArray();
            }
            catch(Exception)
            {
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        #endregion

        #region RUNNING_ACCOUNT

        public Array GetRunningAccountListDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, RunningAccountViewModel model, String ReportType)
        {

            List<USP_ACC_RPT_SHOW_RUNNING_ACCOUNT_SELF_Result> lstAccountDetails = new List<USP_ACC_RPT_SHOW_RUNNING_ACCOUNT_SELF_Result>();

            List<USP_ACC_RPT_SHOW_RUNNING_ACCOUNT_ALLPIU_Result> lstRunningAccountAllPIUDetails = new List<USP_ACC_RPT_SHOW_RUNNING_ACCOUNT_ALLPIU_Result>();

            //string procedureName = string.Empty;
            //SqlParameter[] parameters = new SqlParameter[5];

            try
            {
                //Call All PIU SP
                if (PMGSYSession.Current.LevelId != 5 && model.AdminCode == 0 && ReportType=="D")
                {

                    //Time out added by Abhishek kamble 3-July-2014
                    ((System.Data.Entity.Infrastructure.IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 1200;
                    dbContext.Configuration.LazyLoadingEnabled = false;


                    lstRunningAccountAllPIUDetails = dbContext.USP_ACC_RPT_SHOW_RUNNING_ACCOUNT_ALLPIU(PMGSYSession.Current.AdminNdCode, model.Month, model.Year, PMGSYSession.Current.FundType, model.Balance).ToList<USP_ACC_RPT_SHOW_RUNNING_ACCOUNT_ALLPIU_Result>();
                    totalRecords = lstAccountDetails.Count();

                    return lstRunningAccountAllPIUDetails.Select(itemDetails => new
                    {
                        cell = new[] 
                    {
                        itemDetails.HEAD_CODE == null?string.Empty:itemDetails.HEAD_CODE.ToString(),
                        //(itemDetails.HEAD_CODE == "11.01")||(itemDetails.HEAD_CODE == "11.02")?(itemDetails.HEAD_NAME == null?string.Empty:itemDetails.HEAD_NAME.ToString())+"Status:"+(itemDetails.HEAD_COMP_PROGRESS== null?string.Empty:itemDetails.HEAD_COMP_PROGRESS.ToString())+"Agency("+(itemDetails.IMS_COLLABORATION == null?string.Empty:itemDetails.IMS_COLLABORATION.ToString())+")":itemDetails.HEAD_NAME.ToString(),
                        itemDetails.HEAD_NAME == null?string.Empty:itemDetails.HEAD_NAME.ToString(),
                        itemDetails.HEAD_COMP_PROGRESS== null?string.Empty:itemDetails.HEAD_COMP_PROGRESS.ToString(),
                        itemDetails.IMS_COLLABORATION == null?string.Empty:itemDetails.IMS_COLLABORATION.ToString(),
                        itemDetails.OB_BALANCE_AMT == null?string.Empty:itemDetails.OB_BALANCE_AMT.ToString(),
                        itemDetails.MONTHLY_BALANCE_AMT== null?string.Empty:itemDetails.MONTHLY_BALANCE_AMT.ToString(),
                        (itemDetails.MONTHLY_BALANCE_AMT + itemDetails.OB_BALANCE_AMT) == 0?"0.00":(itemDetails.MONTHLY_BALANCE_AMT + itemDetails.OB_BALANCE_AMT).ToString()
                    }
                    }).ToArray();
                }
                else
                {
                    //Time out added by Abhishek kamble 3-July-2014
                    ((System.Data.Entity.Infrastructure.IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 1200;
                    dbContext.Configuration.LazyLoadingEnabled = false;

                    lstAccountDetails = dbContext.USP_ACC_RPT_SHOW_RUNNING_ACCOUNT_SELF(model.AdminCode, model.Month, model.Year, PMGSYSession.Current.FundType, model.Balance).ToList<USP_ACC_RPT_SHOW_RUNNING_ACCOUNT_SELF_Result>();
                    totalRecords = lstAccountDetails.Count();

                    return lstAccountDetails.Select(itemDetails => new
                    {
                        cell = new[] 
                    {
                        itemDetails.HEAD_CODE == null?string.Empty:itemDetails.HEAD_CODE.ToString(),
                        //(itemDetails.HEAD_CODE == "11.01")||(itemDetails.HEAD_CODE == "11.02")?(itemDetails.HEAD_NAME == null?string.Empty:itemDetails.HEAD_NAME.ToString())+"Status:"+(itemDetails.HEAD_COMP_PROGRESS== null?string.Empty:itemDetails.HEAD_COMP_PROGRESS.ToString())+"Agency("+(itemDetails.IMS_COLLABORATION == null?string.Empty:itemDetails.IMS_COLLABORATION.ToString())+")":itemDetails.HEAD_NAME.ToString(),
                        itemDetails.HEAD_NAME == null?string.Empty:itemDetails.HEAD_NAME.ToString(),
                        itemDetails.HEAD_COMP_PROGRESS== null?string.Empty:itemDetails.HEAD_COMP_PROGRESS.ToString(),
                        itemDetails.IMS_COLLABORATION == null?string.Empty:itemDetails.IMS_COLLABORATION.ToString(),
                        itemDetails.OB_BALANCE_AMT == null?string.Empty:itemDetails.OB_BALANCE_AMT.ToString(),
                        itemDetails.MONTHLY_BALANCE_AMT== null?string.Empty:itemDetails.MONTHLY_BALANCE_AMT.ToString(),
                        (itemDetails.MONTHLY_BALANCE_AMT + itemDetails.OB_BALANCE_AMT) == 0?"0.00":(itemDetails.MONTHLY_BALANCE_AMT + itemDetails.OB_BALANCE_AMT).ToString()
                    }
                    }).ToArray();
                }
                //lstAccountDetails = dbContext.Database.ExecuteSqlCommand("Exec USP_ACC_RPT_SHOW_RUNNING_ACCOUNT_SELF " + PMGSYSession.Current.AdminNdCode + "," + model.Month + "," + model.Year + "," + PMGSYSession.Current.FundType + "," + model.Balance);

                //procedureName = "[omms].[USP_ACC_RPT_SHOW_RUNNING_ACCOUNT_SELF] @P_INT_AdminNDCode, @P_INT_AccMonth, @P_INT_AccYear, @P_CHAR_fundType,@P_CHAR_CreditDebit";
                //parameters[0] = new SqlParameter("P_INT_AdminNDCode", PMGSYSession.Current.AdminNdCode);
                //parameters[1] = new SqlParameter("P_INT_AccMonth", model.Month);
                //parameters[2] = new SqlParameter("P_INT_AccYear", model.Year);
                //parameters[3] = new SqlParameter("P_CHAR_fundType", PMGSYSession.Current.FundType);
                //parameters[4] = new SqlParameter("P_CHAR_CreditDebit", "C");
                //lstAccountDetails = dbContext.Database.SqlQuery<RunningAccountList>(procedureName,parameters).ToList();

              
            }
            catch (Exception)
            {
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

        public SelectList PopulateYears(int AdminNdCode, int LevelId)
        {

        //model.ddlYear = new SelectList(objCommon.PopulateFinancialYear(false, false), "Value", "Text").ToList();
            try
            {

                int? entryYear = 0;
                Int16? entryMonth = 0;

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

                if (lstClosedMonths != null)
                {
                    entryYear = lstClosedMonths.Max(m => (int?)m.YEAR);

                    if (entryYear != null)
                    {
                        entryMonth = lstClosedMonths.Where(m => m.YEAR == entryYear).Max(s => (short?)s.MONTH);

                        if (entryMonth == 12)
                        {
                            //entryMonth = 1;
                            entryYear = entryYear + 1;
                        }
                        //else
                        //{
                        //    //entryMonth = Convert.ToInt16(entryMonth + 1);
                        //    entryYear = entryYear + 1;
                        //}
                    }
                }

                //List<SelectListItem> lstYears = new List<SelectListItem>();

                List<MASTER_YEAR> lstYears = new List<MASTER_YEAR>();
                if (entryYear != 0 && entryYear != null)
                {
                    lstYears = dbContext.MASTER_YEAR.Where(y => y.MAST_YEAR_CODE == entryYear).OrderByDescending(y => y.MAST_YEAR_CODE).ToList<MASTER_YEAR>();
                }
                else
                {
                    //Commented By Abhishek kamble 31-Mar-2014
                    //lstYears.Insert(0, new MASTER_YEAR() { MAST_YEAR_CODE = 0, MAST_YEAR_TEXT = "Select Year " });

                    //Added By Abhishek kamble 31-Mar-2014
                        short? OBYear = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_TYPE == "O" && m.LVL_ID == LevelId && m.FUND_TYPE == PMGSYSession.Current.FundType && m.ADMIN_ND_CODE == AdminNdCode).Select(s => s.BILL_YEAR).FirstOrDefault();

                        if (OBYear != null)
                        {
                            lstYears = dbContext.MASTER_YEAR.Where(y => y.MAST_YEAR_CODE == OBYear).OrderByDescending(y => y.MAST_YEAR_CODE).ToList<MASTER_YEAR>();
                        }
                        else {
                            lstYears.Insert(0, new MASTER_YEAR() { MAST_YEAR_CODE = 0, MAST_YEAR_TEXT = "Select Year " });                    
                        }
                   

                }
                return new SelectList(lstYears, "MAST_YEAR_CODE", "MAST_YEAR_TEXT", DateTime.Now.Year);
            }
            catch(Exception ex)
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

        #endregion

        #region REGISTER_HEADS

        public Array GetRegisterListDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords,RegisterViewModel model)
        {
            dbContext = new PMGSYEntities();
            try
            {
                List<USP_ACC_RPT_REGISTER_CREDIT_DEBIT_HEADS_Result> lstDetails = new List<USP_ACC_RPT_REGISTER_CREDIT_DEBIT_HEADS_Result>();

                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;

                lstDetails = dbContext.USP_ACC_RPT_REGISTER_CREDIT_DEBIT_HEADS(model.StateCode,model.Collaboration, model.AdminCode, model.Month, model.Year, PMGSYSession.Current.FundType, model.LevelId, model.HeadId, model.FundingAgencyCode).ToList();

                CommonFunctions objComFuc = new CommonFunctions();

                totalRecords = lstDetails.Count();
                return lstDetails.Select(itemDetails => new
                {
                    cell = new[] 
                    {
                        itemDetails.FIRST_DATE == null?string.Empty:itemDetails.FIRST_DATE,
                        //(itemDetails.HEAD_CODE == "11.01")||(itemDetails.HEAD_CODE == "11.02")?(itemDetails.HEAD_NAME == null?string.Empty:itemDetails.HEAD_NAME.ToString())+"Status:"+(itemDetails.HEAD_COMP_PROGRESS== null?string.Empty:itemDetails.HEAD_COMP_PROGRESS.ToString())+"Agency("+(itemDetails.IMS_COLLABORATION == null?string.Empty:itemDetails.IMS_COLLABORATION.ToString())+")":itemDetails.HEAD_NAME.ToString(),
                        itemDetails.CON_SUP_NAME == null?string.Empty:itemDetails.CON_SUP_NAME.ToString(),
                        itemDetails.AGREEMENT_NUMBER== null?string.Empty:itemDetails.AGREEMENT_NUMBER.ToString(),
                        itemDetails.OPENING_BALANCE == null?string.Empty:itemDetails.OPENING_BALANCE.ToString(),
                        itemDetails.BILL_NO == null?string.Empty:itemDetails.BILL_NO.ToString(),
                        itemDetails.BILL_DATE== null?string.Empty:itemDetails.BILL_DATE.ToString(),
                        //model.HeadCategoryId == 6?(itemDetails.CREDIT_DEBIT == "D"?objCommonFunc._IndianFormatAmount(itemDetails.AMOUNT.ToString()):(itemDetails.CREDIT_DEBIT =="C"?"0.00":"0.00")):(itemDetails.CREDIT_DEBIT == "C"?objCommonFunc._IndianFormatAmount(itemDetails.AMOUNT.ToString()):(itemDetails.CREDIT_DEBIT == "D"?"0.00":"0.00")).ToString(),// itemDetails.AMOUNT == null?"0":itemDetails.AMOUNT.ToString(),
                        //model.HeadCategoryId == 6?(itemDetails.CREDIT_DEBIT == "C"?objCommonFunc._IndianFormatAmount(itemDetails.AMOUNT.ToString()):(itemDetails.CREDIT_DEBIT =="D"?"0.00":"0.00")):(itemDetails.CREDIT_DEBIT == "D"?objCommonFunc._IndianFormatAmount(itemDetails.AMOUNT.ToString()):(itemDetails.CREDIT_DEBIT == "C"?"0.00":"0.00")).ToString(),
                        //model.HeadCategoryId == 6?(itemDetails.CREDIT_DEBIT == "C"?(itemDetails.OPENING_BALANCE + 0 - itemDetails.AMOUNT).ToString():(itemDetails.CREDIT_DEBIT == "D"?(itemDetails.OPENING_BALANCE + itemDetails.AMOUNT - 0 ).ToString():"0.00")):(itemDetails.CREDIT_DEBIT == "C"?(itemDetails.OPENING_BALANCE + itemDetails.AMOUNT - 0).ToString():(itemDetails.CREDIT_DEBIT == "D"?(itemDetails.OPENING_BALANCE + 0 - itemDetails.AMOUNT).ToString():"0.00")),
                        
                        //Old Code to show creadit debit amount 21-June-2014
                        //objCommonFunc._IndianFormatAmount(itemDetails.AMOUNT.ToString()),
                        //"0.00",
                        
                        //old Code to show creadit debit amount 21-June-2014
                        //itemDetails.CREDIT_DEBIT.ToString()=="C"?objCommonFunc._IndianFormatAmount(itemDetails.AMOUNT.ToString()):"0.00",
                        //itemDetails.CREDIT_DEBIT.ToString()=="D"?objCommonFunc._IndianFormatAmount(itemDetails.AMOUNT.ToString()):"0.00",
                        
                        //New Code to show creadit debit amount 14-July-2014
                        (dbContext.ACC_MASTER_HEAD.Where(m=>m.HEAD_ID==model.HeadId).Select(s=>s.CREDIT_DEBIT).FirstOrDefault()=="D") &&( itemDetails.CREDIT_DEBIT.ToString()=="D") ?objCommonFunc._IndianFormatAmount(itemDetails.AMOUNT.ToString()): (dbContext.ACC_MASTER_HEAD.Where(m=>m.HEAD_ID==model.HeadId).Select(s=>s.CREDIT_DEBIT).FirstOrDefault()=="C") &&( itemDetails.CREDIT_DEBIT.ToString()=="C") ?objCommonFunc._IndianFormatAmount(itemDetails.AMOUNT.ToString()): "0.00",                        
                        (dbContext.ACC_MASTER_HEAD.Where(m=>m.HEAD_ID==model.HeadId).Select(s=>s.CREDIT_DEBIT).FirstOrDefault()=="C") &&( itemDetails.CREDIT_DEBIT.ToString()=="C") ? ((dbContext.ACC_MASTER_HEAD.Where(m=>m.HEAD_ID==model.HeadId).Select(s=>s.CREDIT_DEBIT).FirstOrDefault()=="C")?"0.00": objCommonFunc._IndianFormatAmount(itemDetails.AMOUNT.ToString())): (dbContext.ACC_MASTER_HEAD.Where(m=>m.HEAD_ID==model.HeadId).Select(s=>s.CREDIT_DEBIT).FirstOrDefault()=="D") &&( itemDetails.CREDIT_DEBIT.ToString()=="D") ? "0.00":objCommonFunc._IndianFormatAmount(itemDetails.AMOUNT.ToString()),
                                 

                        //old Code 30-June-2014 Modified By Abhishek kable         
                        //objCommonFunc._IndianFormatAmount((itemDetails.OPENING_BALANCE + itemDetails.AMOUNT - 0).ToString()),
                        //new Code for closing balance 30-June-2014                        
                        //itemDetails.CREDIT_DEBIT.ToString()=="D"?objCommonFunc._IndianFormatAmount((itemDetails.OPENING_BALANCE + (itemDetails.AMOUNT - 0)).ToString()):itemDetails.CREDIT_DEBIT.ToString()=="C"?objCommonFunc._IndianFormatAmount((itemDetails.OPENING_BALANCE + (0-itemDetails.AMOUNT )).ToString()):objCommonFunc._IndianFormatAmount((itemDetails.OPENING_BALANCE).ToString()),
                        
                        itemDetails.CREDIT_DEBIT.ToString()=="D"? (dbContext.ACC_MASTER_HEAD.Where(m=>m.HEAD_ID==model.HeadId).Select(s=>s.CREDIT_DEBIT).FirstOrDefault()=="D"? (objCommonFunc._IndianFormatAmount((itemDetails.OPENING_BALANCE + (itemDetails.AMOUNT - 0)).ToString())) : (objCommonFunc._IndianFormatAmount((itemDetails.OPENING_BALANCE + (0-itemDetails.AMOUNT)).ToString()))) : itemDetails.CREDIT_DEBIT.ToString()=="C"? (dbContext.ACC_MASTER_HEAD.Where(m=>m.HEAD_ID==model.HeadId).Select(s=>s.CREDIT_DEBIT).FirstOrDefault()=="D"? (objCommonFunc._IndianFormatAmount((itemDetails.OPENING_BALANCE + ( 0-itemDetails.AMOUNT )).ToString())) : (objCommonFunc._IndianFormatAmount((itemDetails.OPENING_BALANCE + (itemDetails.AMOUNT-0)).ToString()))):objCommonFunc._IndianFormatAmount((itemDetails.OPENING_BALANCE).ToString()),

                        //dbContext.ACC_MASTER_HEAD.Where(m=>m.HEAD_ID==model.HeadId).Select(s=>s.CREDIT_DEBIT).FirstOrDefault()=="C" ? objCommonFunc._IndianFormatAmount((itemDetails.OPENING_BALANCE + ( itemDetails.AMOUNT - 0 ) ).ToString())

                        itemDetails.MAST_CON_ID.ToString(),
                        itemDetails.CREDIT_DEBIT.ToString(),
                    }
                }).ToArray();

            }
            catch (Exception)
            {
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

        public Array ListImprestSettlementsDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, ImprestSettlementViewModel model)
        {
            dbContext = new PMGSYEntities();
            try
            {
                List<USP_ACC_RPT_REGISTER_IMPREST_Settlement_DETAILS_Result> lstDetails = new List<USP_ACC_RPT_REGISTER_IMPREST_Settlement_DETAILS_Result>();

                lstDetails = dbContext.USP_ACC_RPT_REGISTER_IMPREST_Settlement_DETAILS(PMGSYSession.Current.FundType, model.AdminCode, model.FinancialYear).ToList();

                totalRecords = lstDetails.Count();
                return lstDetails.Select(itemDetails => new
                {
                    cell = new[] 
                    {
                        itemDetails.P_Bill_No == null?string.Empty:itemDetails.P_Bill_No.ToString(),
                        itemDetails.P_BILL_DATE == null?"-":itemDetails.P_BILL_DATE.ToString(),
                        itemDetails.P_AMOUNT.ToString(),
                        itemDetails.Payee_Name == null?string.Empty:itemDetails.Payee_Name.ToString(),
                        itemDetails.S_BILL_NO == null?string.Empty:itemDetails.S_BILL_NO.ToString(),
                        itemDetails.S_BILL_DATE== null?string.Empty:Convert.ToDateTime(itemDetails.S_BILL_DATE).ToString("dd/MM/yyyy"),
                        itemDetails.S_Amount.ToString(),
                    }
                }).ToArray();

            }
            catch (Exception)
            {
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


        #endregion

        #region GENERIC_FUNCTIONS
        public class ReportSPDAL<T>
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            public List<T> ExecuteSP(SpDetail spDetail)
            {
                try
                {
                    SqlParameter[] parameters = new SqlParameter[spDetail.ParamList.Count];
                    string spName = spDetail.SPName;
                    int cnt = 0;
                    foreach (SpParameters param in spDetail.ParamList)
                    {
                        parameters[cnt] = new SqlParameter(param.ParamName, param.ParamValue);
                        spName += " @" + param.ParamName + ",";
                        cnt++;
                    }
                    spName = spName.TrimEnd(',');
                    var list = dbContext.Database.SqlQuery<T>(spName, parameters).ToList<T>();

                    return list;
                }
                catch (Exception ex)
                {
                    string str = ex.Message;
                    str += "Error";
                    return new List<T>();
                }
            }
            public List<T> ExecuteProcedure(string procedureName, SqlParameter[] parameters)
            {
                try
                {
                    var list = dbContext.Database.SqlQuery<T>(procedureName, parameters).ToList<T>();

                    return list;
                }
                catch (Exception ex)
                {
                    string str = ex.Message;
                    str += "Error";
                    return new List<T>();
                }
            }

        }
        public class SpParameters
        {
            public string ParamName { get; set; }
            public string ParamValue { get; set; }



        }
        public class SpDetail
        {
            public string SPName { get; set; }
            public List<SpParameters> ParamList { get; set; }

        }
        #endregion

        #region Income and Expenditure

            /// <summary>
            /// Get data from 'USP_ACC_RPT_IE_STATE' SP to display IE details.
            /// </summary>
            /// <param name="Month"></param>
            /// <param name="Year"></param>
            /// <param name="ndCode"></param>
            /// <param name="rlevel"></param>
            /// <param name="allPiu"></param>
            /// <param name="duration"></param>
            /// <param name="totalRecords"></param>
            /// <returns></returns>
            public Array ListIncomeAndExpenditureDetails(int? Month, int? Year, int? ndCode, int? rlevel, int? allPiu, int? duration,out long totalRecords)
            {
                try
                {
                    if(rlevel==4)
                    {
                        var IncomeEquipmentDetails = dbContext.USP_ACC_RPT_IE_STATE(ndCode, Month, Year, PMGSYSession.Current.FundType).ToList();
                        totalRecords = IncomeEquipmentDetails.Count();
                        return IncomeEquipmentDetails.Select(details=>new{
                    
                            id=details.HEAD_CODE,
                            cell=new[]{          
                                string.Empty,
                                //details.HEAD_NAME,
                                details.HEAD_CODE.Count()!=5?details.HEAD_NAME:"<span class='ui-state-default' style='border:none'>"+details.HEAD_CODE+"</span> - "+details.HEAD_NAME,
                                details.SRRDA_OB_AMT==null?"0":details.SRRDA_OB_AMT.ToString(),
                                details.SRRDA_CURRENT_AMT==null?"0":details.SRRDA_CURRENT_AMT.ToString(),
                                details.DPIU_OB_AMT==null?"0":details.DPIU_OB_AMT.ToString(),
                                details.DPIU_CURRENT_AMT==null?"0":details.DPIU_CURRENT_AMT.ToString(),
                                //(Convert.ToInt32(details.SRRDA_CURRENT_AMT)+Convert.ToInt32(details.DPIU_CURRENT_AMT)).ToString()
                                //Added By Abhishek kamble 28-Apr-2014
                                (Convert.ToInt32(details.SRRDA_OB_AMT)+Convert.ToInt32(details.SRRDA_CURRENT_AMT)+Convert.ToInt32(details.DPIU_OB_AMT)+Convert.ToInt32(details.DPIU_CURRENT_AMT)).ToString()
                            }
                        }).ToArray();

                    }else if(rlevel==1 || rlevel==2){

                        var IncomeEquipmentDetails = dbContext.USP_ACC_RPT_IE_SRRDA_DPIU(ndCode, Month, Year, PMGSYSession.Current.FundType, rlevel, allPiu).ToList();
                        totalRecords = IncomeEquipmentDetails.Count();

                        return IncomeEquipmentDetails.Select(details => new
                        {
                            id = details.HEAD_CODE,
                            cell = new[]{
                                         String.Empty,
                                         details.HEAD_CODE.Count()!=5?details.HEAD_NAME:"<span class='ui-state-default' style='border:none;cursor: default'>"+details.HEAD_CODE+"</span> - "+details.HEAD_NAME,
                                         details.OB_AMT.ToString(),
                                         details.CURRENT_AMT.ToString(),
                                         (Convert.ToInt32(details.OB_AMT)+Convert.ToInt32(details.CURRENT_AMT)).ToString(),
                                        }
                        }).ToArray();
                    }
                    totalRecords = 0;
                    return null;
                }
                catch (Exception)
                {
                    totalRecords = 0;
                    return null;
                }
                finally {
                    if (dbContext != null)
                    {
                        dbContext.Dispose();    
                    }
                }
            }
        
        #endregion Income and Expenditure

        #region POPULATION_METHODS

            public string GetStateName(int adminCode)
            {
                string statename = string.Empty;
                using (dbContext = new PMGSYEntities())
                {
                    try
                    {
                        statename = dbContext.ADMIN_DEPARTMENT.Where(m=>m.ADMIN_ND_CODE == adminCode).Join(dbContext.MASTER_STATE,a=>a.MAST_STATE_CODE,s=>s.MAST_STATE_CODE,(a,s)=>new {s.MAST_STATE_NAME}).Select(m=>m.MAST_STATE_NAME).FirstOrDefault();
                    }
                    catch (Exception)
                    {
                        return "";
                    }
                }
                return statename;
            }

        #endregion
    }

    public interface IReportDAL
    {
        #region CASH_BOOK

        CBHeader GetReportHeader(ReportFilter objParam);
        CBSingleModel GetSingleCB(ReportFilter objParam);
        CBReceiptModel ReceiptCashBook(ReportFilter objParam);
        CBPaymentModel PaymentCashBook(ReportFilter objParam);
        string GetDepartmentName(int adminCode);

        #endregion

        #region  Ledger

        List<SelectListItem> GetLedgerHeadList(String creditDebit, String fundType, List<short> op_Level,String SRRDA_DPIU);
        List<LedgerAmountModel> GetCreditDebitModel(ReportFilter objParam);
        LedgerModel GetForContextData(ReportFilter objParam);
        #endregion

        #region MonthlyAccount
        string GetNodalAgency(Int32 AdminNdCode);
        List<USP_RPT_SHOW_MONTHLY_ACCOUNT_SELF_Result> GetMonthlyAccountList(ReportFilter objParam);

        //added by abhishek 10-9-2013
        List<USP_RPT_SHOW_MONTHLY_ACCOUNT_ALLPIU_Result> GetMonthlyAccountForAllPIU(ReportFilter objParam);

        //List<SelectListItem> PopulateSRRDA(int stateCode);
        
        List<SelectListItem> PopulateStateSRRDA();

        #endregion

        #region Account
        //MaintenanceFundBalanceSheetList GetMaintenanceFundBalanceSheetDAL(MaintenanceFundBalanceSheet maintenanceFundBalanceSheet, ReportFilter objParam);
        //ProgramFundBalanceSheetList GetProgramFundBalanceSheetDAL(ProgramFundBalanceSheet programFundBalanceSheet, ReportFilter objParam);
        //AdminFundBalanceSheetList GetAdminFundBalanceSheetDAL(AdminFundBalanceSheet adminFundBalanceSheet, ReportFilter objParam);
        BalanceSheetList GetBalanceSheetDAL(BalanceSheet balanceSheet, ReportFilter objParam);

        RptTrnasferEntryOrderList GetTransferEntryOrderListDAL(RptTransferEntryOrder teo, ReportFilter objParam);
        #endregion Account

        #region RegisterOfWorks

        RegisterOfWorksModel RegisterOfWorksDAL(TransactionParams objparams);
        Array RegisterOfWorksHeaderGridDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int adminNDCode, int contractorID, int agreementCode);
        #endregion

        #region RUNNING_ACCOUNT

        Array GetRunningAccountListDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, RunningAccountViewModel model, String ReportType);

        SelectList PopulateYears(int AdminNdCode, int LevelId);
        #endregion

        #region REGISTER_HEADS

        Array GetRegisterListDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, RegisterViewModel model);

        Array ListImprestSettlementsDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, ImprestSettlementViewModel model);

        #endregion

        #region Income and Expenditure

        Array ListIncomeAndExpenditureDetails(int? Month, int? Year, int? ndCode, int? rlevel, int? allPiu, int? duration, out long totalRecords);

        #endregion Income and Expenditure

    }
}