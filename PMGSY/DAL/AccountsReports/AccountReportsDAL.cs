/*--------------------------------------------------------
 * File Name:AccountReportsDAL.cs
 * Project: OMMAS-II
 * Created By: Ashish Markande
 * Creation Date:22/07/2013
 * Purpose: To intract with database.
 * ----------------------------------------------------------
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.Extensions;
using PMGSY.Models;
using System.Data.Entity;
using PMGSY.Models.Report;
using PMGSY.Models.AccountReports;
using PMGSY.Common;
using PMGSY.Models.AccountsReports;
using System.Data.SqlClient;
using PMGSY.Models.Report.Account;
using System.Data.Entity.Infrastructure;



namespace PMGSY.DAL.AccountsReports
{
    public partial class AccountReportsDAL : IAccountReportsDAL
    {

        PMGSYEntities dbcontext = null;
        private CommonFunctions commomFuncObj = new CommonFunctions();

        #region AnnualAccount
        /// <summary>
        /// getAllYears(): Method is used to populate the years.
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> getAllYears()
        {

            List<SelectListItem> yearList = new List<SelectListItem>();

            yearList.Add(
                new SelectListItem()
                {
                    Text = "Select Year",
                    Value = "0"
                }
                );

            for (int i = DateTime.Now.Year; i >= 2000; i--)
            {
                yearList.Add(
                    new SelectListItem()
                    {
                        Text = (i) + "-" + (i + 1),
                        Value = i.ToString()
                    });
            }
            return yearList;
        }

        /// <summary>
        /// getBalanceType(): Method is used to populate balance type.
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> getBalanceType()
        {
            List<SelectListItem> listBalance = new List<SelectListItem>();
            listBalance.Add(new SelectListItem()
                {
                    Text = "Select Balance",
                    Value = "%"
                });

            listBalance.Add(new SelectListItem()
            {
                Text = "Credit",
                Value = "C"
            });
            listBalance.Add(new SelectListItem()
            {
                Text = "Debit",
                Value = "D"
            });
            return listBalance;

        }

        public List<SelectListItem> PopulateSRRDA()
        {
            PMGSYEntities dbContext = new PMGSYEntities();

            try
            {
                //List<SelectListItem> lstSRRDA = new SelectList(dbContext.ADMIN_DEPARTMENT.Where(m => m.MAST_STATE_CODE == StateCode && m.MAST_ND_TYPE == "S"), "ADMIN_ND_CODE", "ADMIN_ND_NAME").ToList();
                //lstSRRDA.Insert(0, (new SelectListItem { Text = "Select SRRDA", Value = "0" }));
                //return lstSRRDA;
                List<SelectListItem> lstDetails = new List<SelectListItem>();
                var list = (from item in dbContext.ADMIN_DEPARTMENT
                            join state in dbContext.MASTER_STATE
                            on item.MAST_STATE_CODE equals state.MAST_STATE_CODE
                            where item.MAST_ND_TYPE == "S"
                            orderby state.MAST_STATE_NAME
                            select new
                            {
                                ADMIN_NAME = state.MAST_STATE_NAME + "(" + item.ADMIN_ND_NAME + ")",
                                ADMIN_CODE = item.ADMIN_ND_CODE
                            });

                foreach (var item in list)
                {
                    lstDetails.Add(new SelectListItem { Value = item.ADMIN_CODE.ToString(), Text = item.ADMIN_NAME.ToString().Trim() });

                }
                //lstTransaction = lstTransaction.OrderBy(m => m.Text).ToList();
                lstDetails.Insert(0, (new SelectListItem { Text = "Select State", Value = "" }));
                return lstDetails;
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

        
        /// <summary>
        /// GetNodalAgency(): Method is used to get Nodal Agency Name based on Admin_Nd_Code.
        /// </summary>
        /// <param name="AdminNdCode"></param>
        /// <returns></returns>
        public string GetNodalAgency(Int32 AdminNdCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                return dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == AdminNdCode).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
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

        /// <summary>
        /// GetMonthlyAccountList(): Method is used to populate account list based on parameter which are passed from BAL.
        /// </summary>
        /// <param name="objParam"></param>
        /// <returns></returns>
        public AnnualAccount GetAnnualAccountList(ReportFilter objParam)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            AnnualAccount account = new AnnualAccount();
            try
            {
                string rptType=string.Empty;
                string fundType = string.Empty;
                account.CreditDebit = objParam.CreditDebit;
                account.Year = objParam.Year;
                int levelId =0;
                if(PMGSYSession.Current.LevelId==4)
                {
                   rptType="O";
                }
                else if(PMGSYSession.Current.LevelId==5)
                {
                        rptType="A";
                }
                else if (objParam.Selection == "R")
                {
                    rptType = "O";
                }
                else if (objParam.Selection == "D" && objParam.DPIUSelection != "0")
                {
                    rptType = "A";
                }
                   

               // if (PMGSYSession.Current.FundType == "P")
               // {
               //     fundType = "PMGSY PROGRAM FUND";
               // }
               // else
               //     if (PMGSYSession.Current.FundType == "M")
               // {
               //     fundType = "PMGSY MAINTENANCE FUND";
               // }
               //else if (PMGSYSession.Current.FundType == "A")
               // {
               //     fundType = "PMGSY ADMINISTRATIVE FUND";
               // }

                if (objParam.Selection == "S")
                {
                    account.lstAnnualReport = dbContext.USP_RPT_SHOW_YEARLY_ACCOUNT_SELF(objParam.AdminNdCode, objParam.Year, objParam.FundType, objParam.CreditDebit).ToList<USP_RPT_SHOW_YEARLY_ACCOUNT_SELF_Result>();
                    account.lstReportDPIU = dbContext.USP_RPT_SHOW_YEARLY_ACCOUNT_ALLPIU(objParam.AdminNdCode, objParam.Year, objParam.FundType, objParam.CreditDebit).ToList<USP_RPT_SHOW_YEARLY_ACCOUNT_ALLPIU_Result>();
                    //account.TotalCreditDebit = 0;
                    //account.TotalOpeningAmount = 0;
                    //account.TotalCreditDebitDPIU = 0;
                    //account.TotalOpeningAmountDPIU = 0;

                    //foreach (var item in account.lstAnnualReport)
                    //{
                    //    account.TotalCreditDebit += item.OB_AMT;
                    //    account.TotalOpeningAmount += item.YEARLY_AMT;
                    //}
                    //foreach (var data in account.lstReportDPIU)
                    //{
                    //    account.TotalCreditDebitDPIU += data.OB_AMT;
                    //    account.TotalOpeningAmountDPIU += data.YEARLY_AMT;
                    //}


                    account.TotalCreditDebit = account.lstAnnualReport.Sum(m => m.OB_AMT);
                    account.TotalOpeningAmount = account.lstAnnualReport.Sum(m => m.YEARLY_AMT);

                    account.TotalCreditDebitDPIU = account.lstReportDPIU.Sum(m => m.OB_AMT);
                    account.TotalOpeningAmountDPIU = account.lstReportDPIU.Sum(m => m.YEARLY_AMT);
                    levelId = 6;
                }
               else if (objParam.Selection == "R")
                {
                    account.lstAnnualReport = dbContext.USP_RPT_SHOW_YEARLY_ACCOUNT_SELF(objParam.AdminNdCode, objParam.Year, objParam.FundType, objParam.CreditDebit).ToList<USP_RPT_SHOW_YEARLY_ACCOUNT_SELF_Result>();
                    //account.TotalCreditDebit = 0;
                    //account.TotalOpeningAmount = 0;
                    //foreach (var item in account.lstAnnualReport)
                    //{
                    //    account.TotalCreditDebit += item.OB_AMT;
                    //    account.TotalOpeningAmount += item.YEARLY_AMT;
                    //}


                    account.TotalCreditDebit = account.lstAnnualReport.Sum(m => m.OB_AMT);
                    account.TotalOpeningAmount = account.lstAnnualReport.Sum(m => m.YEARLY_AMT);
                    levelId = 4;
                }
                if (objParam.Selection == "D" || PMGSYSession.Current.LevelId==5)
                {
                    if (objParam.DPIUSelection == "0")
                    {

                        account.lstReportDPIU = dbContext.USP_RPT_SHOW_YEARLY_ACCOUNT_ALLPIU(objParam.AdminNdCode, objParam.Year, objParam.FundType, objParam.CreditDebit).ToList<USP_RPT_SHOW_YEARLY_ACCOUNT_ALLPIU_Result>();

                        account.TotalCreditDebit = account.lstReportDPIU.Sum(m => m.OB_AMT);
                        account.TotalOpeningAmount = account.lstReportDPIU.Sum(m => m.YEARLY_AMT);
                    }
                    else
                    {
                        account.lstAnnualReport = dbContext.USP_RPT_SHOW_YEARLY_ACCOUNT_SELF(objParam.LowerAdminNdCode, objParam.Year, objParam.FundType, objParam.CreditDebit).ToList<USP_RPT_SHOW_YEARLY_ACCOUNT_SELF_Result>();

                        account.TotalCreditDebitDPIU = account.lstAnnualReport.Sum(m => m.OB_AMT);
                        account.TotalOpeningAmountDPIU = account.lstAnnualReport.Sum(m => m.YEARLY_AMT);
                    }

                    levelId = 5;
                    //foreach (var data in account.lstReportDPIU)
                    //{
                    //    account.TotalCreditDebitDPIU += data.OB_AMT;
                    //    account.TotalOpeningAmountDPIU+= data.YEARLY_AMT;
                    //}
                }
                var rptHeader = dbContext.SP_ACC_Get_Report_Header_Information("AccountsReports", "AnnualAccountSearch", PMGSYSession.Current.FundType, levelId, rptType).ToList<SP_ACC_Get_Report_Header_Information_Result>().FirstOrDefault();


                if (rptHeader == null)
                {
                    account.FormNo = String.Empty;
                    account.ReportName = String.Empty;
                    account.ReportParaName = String.Empty;
                    account.FundType = String.Empty;
                }
                else
                {
                    account.FormNo = rptHeader.REPORT_FORM_NO;
                    account.ReportName = rptHeader.REPORT_NAME;
                    account.ReportParaName = rptHeader.REPORT_PARAGRAPH_NAME;
                    account.FundType = commomFuncObj.GetFundName(PMGSYSession.Current.FundType);
                }

                if (objParam.LevelId == 6 || objParam.LevelId== 4  && objParam.Selection=="D" && objParam.DPIUSelection !="0")
                {
                    account.PIU = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == objParam.LowerAdminNdCode).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
                    account.NodalAgency = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == objParam.AdminNdCode).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
                }

                if (objParam.LevelId == 5)
                {
                    account.PIU = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == objParam.LowerAdminNdCode).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
                    account.NodalAgency = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == (dbContext.ADMIN_DEPARTMENT.Where(p => p.ADMIN_ND_CODE == objParam.LowerAdminNdCode).Select(p => p.MAST_PARENT_ND_CODE).FirstOrDefault())).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
                }
                else
                {
                    account.NodalAgency = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == objParam.AdminNdCode).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
                }
               
                account.Selection = objParam.Selection;
                account.DPIU = objParam.DPIUSelection;
                return account;
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

        public string StateName(int adminNdCode)
        {
            //string StateName = string.Empty;
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
               
                int stateCode = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == adminNdCode).Select(m => m.MAST_STATE_CODE).FirstOrDefault();
                string StateName = dbContext.MASTER_STATE.Where(m => m.MAST_STATE_CODE == stateCode).Select(m => m.MAST_STATE_NAME).FirstOrDefault();
                return StateName;
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
        #endregion AnnualAccount

        #region BillDetails

        /// <summary>
        /// getBillDetails(): Method is used to populate bill details based on parameters which are passed from BAL.
        /// </summary>
        /// <param name="accModel"></param>
        /// <returns></returns>
        public AccountBillViewModel getBillDetails(AccountBillViewModel accModel)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            CommonFunctions objComm = new CommonFunctions();
            try
            {

                int AdminNDCode = 0;
                int AllPIU = 0;

                if (accModel.NodalAgency == "D")
                {
                   AllPIU=accModel.DPIU == 0 ? 1 : 0;
                }
                

                //check srrda/DPIU
                if (accModel.DPIU > 0)
                {
                    AdminNDCode = accModel.DPIU;
                }
                else
                {
                    AdminNDCode = PMGSYSession.Current.AdminNdCode;
                }

                DateTime? StartDate= null;
                DateTime? EndDate = null;
                if (accModel.StartDate !=null && accModel.EndDate != null)
                {
                    StartDate = objComm.GetStringToDateTime(accModel.StartDate);
                    EndDate = objComm.GetStringToDateTime(accModel.EndDate);
                }

                if (accModel.levelId == 5)
                {
                    accModel.DPIUBySRRDA = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == AdminNDCode).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
                    accModel.NodalAgency = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == (dbContext.ADMIN_DEPARTMENT.Where(p => p.ADMIN_ND_CODE == AdminNDCode).Select(p => p.MAST_PARENT_ND_CODE).FirstOrDefault())).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
                }
                else
                {
                    accModel.NodalAgency = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == AdminNDCode).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
                }


                accModel.lstAccountBillDetails = dbContext.SP_ACC_RPT_DISPALY_Bill_DETAILS(PMGSYSession.Current.FundType, AdminNDCode, accModel.Month, accModel.Year, StartDate, EndDate, accModel.BillType, accModel.rType, AllPIU).ToList<SP_ACC_RPT_DISPALY_Bill_DETAILS_Result>();
                accModel.TotalRecords = accModel.lstAccountBillDetails.Count();
                return accModel;
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
        /// lstTransactionDetails(): Method is used to populate transaction details based on parameters which are passsed from BAL.
        /// </summary>
        /// <param name="billId"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array lstTransactionDetails(string billId, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            Int64 billCode = Convert.ToInt64(billId);
            try
            {
                var lstTransaction = dbContext.SP_ACC_RPT_DISPALY_BILL_TRANSACTIONS_DETAILS(billCode).ToList();

                totalRecords = lstTransaction.Count();

                if (sidx.Trim() != string.Empty)
                {                   
                        lstTransaction = lstTransaction.OrderBy(m => m.CREDIT_DEBIT).ToList();                   
                }

                return lstTransaction.Select(details => new
                {
                    cell = new[]
                {
                    details.Head_desc,
                    details.Bill_Type,
                    details.Cash_Cheque,
                    details.CREDIT_DEBIT.ToString(),
                    details.amount.ToString(),
                    details.Company_Name,
                    details.Agreement_Number,
                    details.Work_Name,
                    details.Department_Name,
                    details.NARRATION
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

        #endregion BillDetails   
                
        #region ContractorLeadger

        public ContractorLedgerModel getLedgerDetails(ContractorLedgerModel contractorLedger)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                contractorLedger.lstContractorLedger = dbContext.SP_ACC_RPT_LEDGER_CONTRACTOR_WORK(contractorLedger.FundType, contractorLedger.PIUCode, contractorLedger.ContractorId, contractorLedger.AggrementId).ToList<SP_ACC_RPT_LEDGER_CONTRACTOR_WORK_Result>();

                var ReportHeader = dbContext.SP_ACC_Get_Report_Header_Information("AccountsReports", "ContrctorLedger", contractorLedger.FundType, contractorLedger.LevelId, "O").ToList<SP_ACC_Get_Report_Header_Information_Result>().FirstOrDefault();

                if (ReportHeader == null)
                {
                    contractorLedger.ReportNumber = String.Empty;
                    contractorLedger.ReportName = String.Empty;
                    contractorLedger.ReportPara = String.Empty;
                }
                else
                {
                    contractorLedger.ReportNumber = ReportHeader.REPORT_FORM_NO;
                    contractorLedger.ReportName = ReportHeader.REPORT_NAME;
                    contractorLedger.ReportPara = ReportHeader.REPORT_PARAGRAPH_NAME;
                    contractorLedger.FundType = commomFuncObj.GetFundName(contractorLedger.FundType);
                }
                
                return contractorLedger;

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

        public int? GetDistrictCode(int PIUCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();

            try
            {
                int? DistrictCode = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == PIUCode).Select(m => m.MAST_DISTRICT_CODE).FirstOrDefault();
                return DistrictCode;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);    
                return null;
            }
            finally
            {
                if (dbcontext != null)
                {
                    dbContext.Dispose();
                }
            }
           
        }

        public Array lstContLedgerDetails(int PIUCode,int ContrCode,int AggCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                var lstDetails = dbContext.SP_ACC_RPT_LEDGER_CONTRACTOR_WORK(PMGSYSession.Current.FundType, PIUCode, ContrCode, AggCode).ToList();
                totalRecords = lstDetails.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if(sord.ToString()=="asc")
                    {
                        lstDetails = lstDetails.OrderBy(m => m.BILL_NO).ToList();
                    }
                }
                return lstDetails.Select(details => new
                {
                    cell = new[]
                    {
                        details.BILL_NO.ToString(),
                        details.Bill_Date.ToString(),
                        details.AdvancePayment.ToString(),
                        details.SecuredAdvance.ToString(),
                        details.MobilisationAdvance.ToString(),
                        details.Machineryadvance.ToString(),
                        details.Materialsissued.ToString(),
                        details.NameOfWork.ToString(),
                        details.NARRATION,
                        details.GrossDebit.ToString(),
                        details.GrossCredit.ToString(),
                        details.TotalValue.ToString(),
                        details.Remarks
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

        public ContractorLedgerModel GerReportHeader(ContractorLedgerModel contractorLedger)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                var ReportHeader = dbContext.SP_ACC_Get_Report_Header_Information("AccountsReports", "ContrctorLedger", contractorLedger.FundType, contractorLedger.LevelId, "O").ToList<SP_ACC_Get_Report_Header_Information_Result>().FirstOrDefault();

                if (ReportHeader == null)
                {
                    contractorLedger.ReportNumber = String.Empty;
                    contractorLedger.ReportName = String.Empty;
                    contractorLedger.ReportPara = String.Empty;
                }
                else
                {
                    contractorLedger.ReportNumber = ReportHeader.REPORT_FORM_NO;
                    contractorLedger.ReportName = ReportHeader.REPORT_NAME;
                    contractorLedger.ReportPara = ReportHeader.REPORT_PARAGRAPH_NAME;
                    contractorLedger.FundName = commomFuncObj.GetFundName(contractorLedger.FundType);
                }
                return contractorLedger;
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
        #endregion ContractorLeadger

        #region Chequebook Details DAL

        /// <summary>
        /// getCheckBookDetails() method is used to get cheque book details from database based on search criteria.
        /// </summary>
        /// <param name="checkbookDetailsViewFilterModel"></param>
        /// <returns>
        /// Returns cheque book details.
        /// </returns>
        public CheckBookDetailsViewFilterModel getCheckBookDetails(CheckBookDetailsViewFilterModel checkbookDetailsViewFilterModel)
        {
            PMGSYEntities dbcontext = new PMGSYEntities();
            try
            {
                //store ADMIN_ND_CODE
                int AdminNDCode = 0;
                int SrrdaNDCode = 0;


                //if DPIU Dropdown is selected then AdminNDCode variable stores selected dropdown ADMIN_ND_CODE value 
                //old code
                //if (checkbookDetailsViewFilterModel.DPIU > 0)
                //{
                //    AdminNDCode = checkbookDetailsViewFilterModel.DPIU;
                //}
                //else
                //{ //else we get ADMIN_ND_CODE from session
                //    AdminNDCode = PMGSYSession.Current.AdminNdCode;
                //}
                
                //New Code for All PIU Option 15-July-2014 start                
                //Mord Level
                if (PMGSYSession.Current.LevelId==6 && checkbookDetailsViewFilterModel.DPIU==0)
                {
                    AdminNDCode = 0; //All PIU Nd Code
                    SrrdaNDCode = checkbookDetailsViewFilterModel.SRRDA;//SRRDA ND Code
                }
                else if (PMGSYSession.Current.LevelId == 6 && checkbookDetailsViewFilterModel.DPIU != 0)
                {
                    AdminNDCode = checkbookDetailsViewFilterModel.DPIU;//PIU ND Code
                    SrrdaNDCode = checkbookDetailsViewFilterModel.SRRDA;//SRRDA ND Code
                }//SRRDA Level
                else if (PMGSYSession.Current.LevelId == 4 && checkbookDetailsViewFilterModel.DPIU == 0)
                {
                    AdminNDCode = 0; //All PIU Nd Code
                    SrrdaNDCode = PMGSYSession.Current.AdminNdCode;//SRRDA ND Code
                }
                else if (PMGSYSession.Current.LevelId == 4 && checkbookDetailsViewFilterModel.DPIU != 0)
                {
                    AdminNDCode = checkbookDetailsViewFilterModel.DPIU;//PIU ND Code
                    SrrdaNDCode = PMGSYSession.Current.AdminNdCode;//SRRDA ND Code
                }  //PIU Level
                else if (PMGSYSession.Current.LevelId == 5)
                {
                    AdminNDCode = PMGSYSession.Current.AdminNdCode;//PIU ND Code                
                    SrrdaNDCode = PMGSYSession.Current.ParentNDCode.Value;//SRRDA ND Code
                }
                //New Code for All PIU Option 15-July-2014 end 



                //state Name
                if (PMGSYSession.Current.LevelId == 6)
                {
                    int ndcode = AdminNDCode == 0 ? SrrdaNDCode : AdminNDCode;

                    checkbookDetailsViewFilterModel.StateName = dbcontext.MASTER_STATE.Where(m => m.MAST_STATE_CODE == dbcontext.ADMIN_DEPARTMENT.Where(p => p.ADMIN_ND_CODE == ndcode).Select(a=>a.MAST_STATE_CODE).FirstOrDefault()).Select(s => s.MAST_STATE_NAME).FirstOrDefault();                    
                }
                else
                {
                    checkbookDetailsViewFilterModel.StateName = PMGSYSession.Current.StateName;
                }
                //PIU Name
                //Modified By Abhishek for All PIU Option 15-July-2014 if Condition Added start                
                if ((checkbookDetailsViewFilterModel.DPIU == 0) && (PMGSYSession.Current.LevelId == 6 || PMGSYSession.Current.LevelId == 4))
                {
                    checkbookDetailsViewFilterModel.PIUName = "All DPIU"; 
                }
                else
                {
                    checkbookDetailsViewFilterModel.PIUName = dbcontext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == AdminNDCode).Select(s => s.ADMIN_ND_NAME).FirstOrDefault();
                }
                //Modified By Abhishek for All PIU Option 15-July-2014 if Condition Added end

                //Bank Name                
                Boolean BankAccStatus = true;
                checkbookDetailsViewFilterModel.BankName = dbcontext.ACC_BANK_DETAILS.Where(m => m.ADMIN_ND_CODE == (dbcontext.ADMIN_DEPARTMENT.Where(t => t.ADMIN_ND_CODE == AdminNDCode).Select(w => w.MAST_PARENT_ND_CODE).FirstOrDefault()) && m.FUND_TYPE == PMGSYSession.Current.FundType && m.BANK_ACC_STATUS == BankAccStatus && m.ACCOUNT_TYPE == "S").Select(s => s.BANK_NAME).FirstOrDefault();

                //Nodal Agency Name
                if (PMGSYSession.Current.LevelId == 4)//SRRDA login we get nodal agency name directly from session
                {
                    checkbookDetailsViewFilterModel.NodalAgencyName = PMGSYSession.Current.DepartmentName;
                }
                else
                { //else find nodal agency name using selected ADMIN_ND_CODE from DPIU Dropdown and its Parent_Nd_Code

                    int ndcode = AdminNDCode ==0? SrrdaNDCode:AdminNDCode;
                    checkbookDetailsViewFilterModel.NodalAgencyName = dbcontext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == (dbcontext.ADMIN_DEPARTMENT.Where(w => w.ADMIN_ND_CODE == ndcode).Select(s => s.MAST_PARENT_ND_CODE).FirstOrDefault())).Select(t => t.ADMIN_ND_NAME).FirstOrDefault();
                }

                //call SP_ACC_RPT_PIU_CHEQUE_ISSUED_DETAILS
                checkbookDetailsViewFilterModel.lstCheckbookDetails = dbcontext.SP_ACC_RPT_PIU_CHEQUE_ISSUED_DETAILS(PMGSYSession.Current.FundType, AdminNDCode, checkbookDetailsViewFilterModel.Month, checkbookDetailsViewFilterModel.Year, checkbookDetailsViewFilterModel.CheckbookMonthYearWise, checkbookDetailsViewFilterModel.CheckbookSeries,SrrdaNDCode).ToList<SP_ACC_RPT_PIU_CHEQUE_ISSUED_DETAILS_Result>();

                //find total no of records
                checkbookDetailsViewFilterModel.totalRecords = checkbookDetailsViewFilterModel.lstCheckbookDetails.Count();

                if ((checkbookDetailsViewFilterModel.CheckbookMonthYearWise == "M") && (checkbookDetailsViewFilterModel.Month > 0) && (checkbookDetailsViewFilterModel.Year > 0))
                {
                    //call SP_ACC_RPT_PIU_CHEQUE_ISSUED_ABSTRACT_Result
                    checkbookDetailsViewFilterModel.lstChequeIssuedAbstract = dbcontext.SP_ACC_RPT_PIU_CHEQUE_ISSUED_ABSTRACT(PMGSYSession.Current.FundType, AdminNDCode, checkbookDetailsViewFilterModel.Month, checkbookDetailsViewFilterModel.Year, SrrdaNDCode).ToList<SP_ACC_RPT_PIU_CHEQUE_ISSUED_ABSTRACT_Result>();

                    //call SP_ACC_RPT_PIU_CHEQUE_OUTSTANDING_DETAILS_Result
                    checkbookDetailsViewFilterModel.lstChequeOutstandingDetails = dbcontext.SP_ACC_RPT_PIU_CHEQUE_OUTSTANDING_DETAILS(PMGSYSession.Current.FundType, AdminNDCode, checkbookDetailsViewFilterModel.Month, checkbookDetailsViewFilterModel.Year, SrrdaNDCode).ToList<SP_ACC_RPT_PIU_CHEQUE_OUTSTANDING_DETAILS_Result>();
                }

                //Set Report Header        
                CommonFunctions objCommonFunction = new CommonFunctions();
                int levelID = 0;

                //Modified by abhishek kamble 10-oct-2013
                if (PMGSYSession.Current.LevelId == 6)
                {
                    levelID = 5;
                }
                else {
                    levelID = PMGSYSession.Current.LevelId;
                }

                var ReportHeader = dbcontext.SP_ACC_Get_Report_Header_Information("AccountsReports", "ChequeBookDetails", PMGSYSession.Current.FundType, levelID, "O").ToList<SP_ACC_Get_Report_Header_Information_Result>().FirstOrDefault();

                if (ReportHeader == null)
                {
                    checkbookDetailsViewFilterModel.FundTypeName = objCommonFunction.GetFundName(PMGSYSession.Current.FundType);
                    checkbookDetailsViewFilterModel.ReportName = String.Empty;
                    checkbookDetailsViewFilterModel.ReportParagraphName = String.Empty;
                    checkbookDetailsViewFilterModel.ReportFormNumber = String.Empty;
                }
                else
                {
                    checkbookDetailsViewFilterModel.FundTypeName = objCommonFunction.GetFundName(PMGSYSession.Current.FundType);
                    checkbookDetailsViewFilterModel.ReportName = ReportHeader.REPORT_NAME;
                    checkbookDetailsViewFilterModel.ReportParagraphName = ReportHeader.REPORT_PARAGRAPH_NAME;
                    checkbookDetailsViewFilterModel.ReportFormNumber = ReportHeader.REPORT_FORM_NO;
                }

                return checkbookDetailsViewFilterModel;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);    
        
                return null;
            }
            finally
            {
                if (dbcontext != null)
                {
                    dbcontext.Dispose();
                }
            }
        }

        #endregion Chequebook Details DAL   

        #region Authorised signatory details

        public AuthorisedSignatoryViewModel getAuthSignatoryDetails(AuthorisedSignatoryViewModel authSignatoryViewModel)
        {

            try
            {
                dbcontext = new PMGSYEntities();

                //store ADMIN_ND_CODE
                int AdminNDCode = 0;
                int LevelID = 0;

                //if DPIU Dropdown is selected then AdminNDCode variable stores selected dropdown ADMIN_ND_CODE value 
                if (authSignatoryViewModel.DPIU > 0) //DPIU
                {
                    AdminNDCode = authSignatoryViewModel.DPIU;
                    LevelID = 5;
                }
                else//SRRDA
                { //else we get ADMIN_ND_CODE from session
                    AdminNDCode = PMGSYSession.Current.AdminNdCode;
                    LevelID = 4;
                }

                //call SP_ACC_RPT_PIU_CHEQUE_ISSUED_ABSTRACT_Result
                authSignatoryViewModel.lstAuthSignatoryDetails = dbcontext.SP_ACC_RPT_LIST_AUTH_SIGNATORY_DETAILS(AdminNDCode, LevelID).ToList<SP_ACC_RPT_LIST_AUTH_SIGNATORY_DETAILS_Result>();

                return authSignatoryViewModel;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);    
        
                return null;
            }
            finally
            {

                if (dbcontext != null)
                {
                    dbcontext.Dispose();
                }
            }
        }

        #endregion

        #region Asset Register Details DAL

        /// <summary>
        /// getAssetRegisterDetails() method is used to get Asset Register Details from the database based on the search criteria.
        /// </summary>
        /// <param name="assetRegisterViewModel"></param>
        /// <returns>Returns Asset Register Details</returns>
        public AssetRegisterViewModel getAssetRegisterDetails(AssetRegisterViewModel assetRegisterViewModel)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                CommonFunctions objCommonFunction = new CommonFunctions();

                //store ADMIN_ND_CODE
                int AdminNDCode = 0;

                //if DPIU Dropdown is selected then AdminNDCode variable stores selected dropdown ADMIN_ND_CODE value 
                if (assetRegisterViewModel.DPIU > 0)
                {
                    AdminNDCode = assetRegisterViewModel.DPIU;
                }
                else
                { //else we get ADMIN_ND_CODE from session
                    AdminNDCode = PMGSYSession.Current.AdminNdCode;
                }

                //PIU Name
                assetRegisterViewModel.DPIUName = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == AdminNDCode).Select(s => s.ADMIN_ND_NAME).FirstOrDefault();

                //Nodal Agency Name
                if (PMGSYSession.Current.LevelId == 4)//SRRDA login we get nodal agency name directly from session
                {
                    assetRegisterViewModel.NodalAgencyName = PMGSYSession.Current.DepartmentName;
                }
                else
                { //else find nodal agency name using selected ADMIN_ND_CODE from DPIU Dropdown and its Parent_Nd_Code
                    assetRegisterViewModel.NodalAgencyName = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == (dbContext.ADMIN_DEPARTMENT.Where(w => w.ADMIN_ND_CODE == AdminNDCode).Select(s => s.MAST_PARENT_ND_CODE).FirstOrDefault())).Select(t => t.ADMIN_ND_NAME).FirstOrDefault();
                }

                System.DateTime? FromDate = null;
                System.DateTime? ToDate = null;
                if (assetRegisterViewModel.FromDate != null && assetRegisterViewModel.ToDate != null)
                {
                    FromDate = objCommonFunction.GetStringToDateTime(assetRegisterViewModel.FromDate);
                    ToDate = objCommonFunction.GetStringToDateTime(assetRegisterViewModel.ToDate);
                }

                //set Month and Year
                short? Month = null;
                short? Year = null;
                if (assetRegisterViewModel.Month != 0 && assetRegisterViewModel.Year != 0)
                {
                    Month = assetRegisterViewModel.Month;
                    Year = assetRegisterViewModel.Year;
                }

                //Added By Abhishek kamble 17-feb-2014
                if (assetRegisterViewModel.monthlyPeriodicFundWise == "P")
                {
                    Month = null;
                    Year = null;
                }

                //call Stored procedure USP_ACC_RPT_REGISTER_DURABLE_ASSETS
                assetRegisterViewModel.lstAssetRegisterDetails = dbContext.USP_ACC_RPT_REGISTER_DURABLE_ASSETS(AdminNDCode, assetRegisterViewModel.monthlyPeriodicFundWise, Month, Year, FromDate, ToDate, PMGSYSession.Current.FundType, assetRegisterViewModel.FundCentralState).ToList<USP_ACC_RPT_REGISTER_DURABLE_ASSETS_Result>();

                if (assetRegisterViewModel.lstAssetRegisterDetails.Count > 0 )
                {
                    assetRegisterViewModel.TotalAmount = assetRegisterViewModel.lstAssetRegisterDetails.Sum(m => (Decimal?)m.TOTAL_AMOUNT);
                }

                //call  Function UDF_ACC_GET_ASSET_HEADS
                assetRegisterViewModel.lstAssetRegisterClassificationDetails = dbContext.UDF_ACC_GET_ASSET_HEADS(PMGSYSession.Current.FundType, assetRegisterViewModel.FundCentralState).ToList<UDF_ACC_GET_ASSET_HEADS_Result>();

                int counter = 1;
                int totalRecords = assetRegisterViewModel.lstAssetRegisterClassificationDetails.Count();

                foreach (var item in assetRegisterViewModel.lstAssetRegisterClassificationDetails)
                {
                    if (counter < totalRecords)
                    {
                        assetRegisterViewModel.ClassificationCode += item.HEAD_CODE + ", ";
                    }
                    else
                    {
                        assetRegisterViewModel.ClassificationCode += item.HEAD_CODE;
                    }
                    counter++;
                }

                //Set Report Header        
                var ReportHeader = new SP_ACC_Get_Report_Header_Information_Result();     
                ReportHeader = dbContext.SP_ACC_Get_Report_Header_Information("AccountsReports", "AssetRegisterDetails", PMGSYSession.Current.FundType, PMGSYSession.Current.LevelId, "O").ToList<SP_ACC_Get_Report_Header_Information_Result>().FirstOrDefault();

                if (ReportHeader == null)
                {
                    assetRegisterViewModel.FundTypeName = objCommonFunction.GetFundName(PMGSYSession.Current.FundType);
                    assetRegisterViewModel.ReportName = String.Empty;
                    assetRegisterViewModel.ReportParagraphName = String.Empty;
                    assetRegisterViewModel.ReportFormNumber = String.Empty;
                }
                else
                {
                    assetRegisterViewModel.FundTypeName = objCommonFunction.GetFundName(PMGSYSession.Current.FundType);
                    assetRegisterViewModel.ReportName = ReportHeader.REPORT_NAME;
                    assetRegisterViewModel.ReportParagraphName = ReportHeader.REPORT_PARAGRAPH_NAME;
                    assetRegisterViewModel.ReportFormNumber = ReportHeader.REPORT_FORM_NO;
                }

                return assetRegisterViewModel;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);    
        
                return new AssetRegisterViewModel();
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
        ///getFundForStateCentral action is used to populate Fund
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> getFundForStateCentral()
        {
            List<SelectListItem> lstFund = new List<SelectListItem>();

            lstFund.Add(new SelectListItem() { Text = "All", Value = "0", Selected = true });
            lstFund.Add(new SelectListItem() { Text = "Central Admin Fund", Value = "C" });
            lstFund.Add(new SelectListItem() { Text = "State Admin Fund", Value = "S" });

            return lstFund;
        }

        #endregion Asset Register Details DAL    

        #region FundTransfer

        public List<SelectListItem> PopulateFund()
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            List<SelectListItem> lstFund = new List<SelectListItem>();
            try
            {
               

                var lstFundType = dbContext.USP_ACC_RPT_Get_HEAD_for_Bank_Authorisation(PMGSYSession.Current.FundType).ToList();

                foreach (var item in lstFundType)
                {
                    lstFund.Add(new SelectListItem { Value = item.HEAD_ID.ToString(), Text = item.NAME.ToString().Trim() });
                }

                lstFund.Insert(0, new SelectListItem { Value = "0", Text = "Select Fund" });
                return lstFund;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return lstFund;
               
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        public FundTransferViewModel GetFundTransferDetails(ReportFilter ObjParam)
        {
            PMGSYEntities dbContext = new PMGSYEntities();

            FundTransferViewModel objFundTransferred=new FundTransferViewModel();

            try
            {
                objFundTransferred.lstFundTransfer = dbContext.USP_ACC_RPT_REGISTER_PIUWISE_FUND_TRANSFERRED(PMGSYSession.Current.FundType, ObjParam.Month, ObjParam.Year, ObjParam.Head, ObjParam.AdminNdCode, ObjParam.LowerAdminNdCode).ToList<USP_ACC_RPT_REGISTER_PIUWISE_FUND_TRANSFERRED_Result>();
                objFundTransferred.HeadName = ObjParam.HeadName;

                int count = 0;
                objFundTransferred.TotalCredit = 0;
                objFundTransferred.TotalDebit = 0;
                foreach (var item in objFundTransferred.lstFundTransfer)
                {
                    if (count == 0)
                    {
                        objFundTransferred.OpeningBalance = item.DEBIT_AMT;
                    }
                    else
                    {
                        objFundTransferred.TotalCredit += item.CREDIT_AMT==null?0:item.CREDIT_AMT;
                        objFundTransferred.TotalDebit += item.DEBIT_AMT==null?0:item.DEBIT_AMT;
                    }
                    count++;
                }

                //if (objFundTransferred.lstFundTransfer.Count == 1)
                //{
                //    objFundTransferred.OpeningBalance = objFundTransferred.lstFundTransfer.Sum(m => m.DEBIT_AMT);
                //}
                //else
                //{
                    
                //}
                var rptHeader = new SP_ACC_Get_Report_Header_Information_Result();
                if (PMGSYSession.Current.LevelId == 6)
                {
                     rptHeader = dbContext.SP_ACC_Get_Report_Header_Information("AccountsReports", "FundTransferView", PMGSYSession.Current.FundType, 4, "A").FirstOrDefault();
                }
                else
                {
                     rptHeader = dbContext.SP_ACC_Get_Report_Header_Information("AccountsReports", "FundTransferView", PMGSYSession.Current.FundType, PMGSYSession.Current.LevelId, "A").FirstOrDefault();
                }

                if (rptHeader != null)
                {
                    objFundTransferred.ReportNumber = rptHeader.REPORT_FORM_NO;
                    objFundTransferred.ReportPara = rptHeader.REPORT_PARAGRAPH_NAME;
                    objFundTransferred.ReportName = rptHeader.REPORT_NAME;
                    objFundTransferred.FundName = commomFuncObj.GetFundName(PMGSYSession.Current.FundType);
                }
                else
                {
                    objFundTransferred.ReportNumber = string.Empty;
                    objFundTransferred.ReportName = string.Empty;
                    objFundTransferred.ReportPara = string.Empty;                    
                }

               
               objFundTransferred.DPIUName = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == ObjParam.LowerAdminNdCode).Select(m=>m.ADMIN_ND_NAME).FirstOrDefault();
               objFundTransferred.StateName = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == (dbContext.ADMIN_DEPARTMENT.Where(p => p.ADMIN_ND_CODE == ObjParam.LowerAdminNdCode).Select(p => p.MAST_PARENT_ND_CODE).FirstOrDefault())).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
               objFundTransferred.TotalRecord = objFundTransferred.lstFundTransfer.Count;


                return objFundTransferred;
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
       

        #endregion FundTransfer

        #region AbstractFundTransfer

        //public AbstractFundTransferredViewModel AbstractFundDetails(ReportFilter objParam)
        //{
        //    PMGSYEntities dbContext = new PMGSYEntities();
        //    AbstractFundTransferredViewModel objAbstractFund=new AbstractFundTransferredViewModel();

        //    objAbstractFund.lstAbstractFund = dbContext.USP_ACC_RPT_REGISTER_ABSTRACT_PIUWISE_FUND_TRANSFERRED(PMGSYSession.Current.FundType, objParam.Year, objParam.Head, objParam.AdminNdCode, objParam.LowerAdminNdCode).ToList<USP_ACC_RPT_REGISTER_ABSTRACT_PIUWISE_FUND_TRANSFERRED_Result>();
        //    objAbstractFund.Year =objParam.Year.ToString();

        //    objAbstractFund.AprilTotal = objAbstractFund.lstAbstractFund.Sum(m => m.AprAmount);

        //    objAbstractFund.MayTotal = objAbstractFund.lstAbstractFund.Sum(m => m.MayAmount);

        //    objAbstractFund.JuneTotal = objAbstractFund.lstAbstractFund.Sum(m => m.JuneAmount);

        //    objAbstractFund.JulyTotal = objAbstractFund.lstAbstractFund.Sum(m => m.JulyAmount);

        //    objAbstractFund.AugestTotal = objAbstractFund.lstAbstractFund.Sum(m => m.AugAmount);

        //    objAbstractFund.SeptemberTotal = objAbstractFund.lstAbstractFund.Sum(m => m.SeptAmount);

        //    objAbstractFund.OctoberTotal = objAbstractFund.lstAbstractFund.Sum(m => m.OctAmount);

        //    objAbstractFund.OctoberTotal = objAbstractFund.lstAbstractFund.Sum(m => m.OctAmount);

        //    objAbstractFund.NovemberTotal = objAbstractFund.lstAbstractFund.Sum(m => m.NovAmount);

        //    objAbstractFund.DecemberTotal = objAbstractFund.lstAbstractFund.Sum(m => m.DecAmount);

        //    objAbstractFund.JanuaryTotal = objAbstractFund.lstAbstractFund.Sum(m => m.JanAmount);

        //    objAbstractFund.FebruaryTotal = objAbstractFund.lstAbstractFund.Sum(m => m.FebAmount);

        //    objAbstractFund.MarchTotal = objAbstractFund.lstAbstractFund.Sum(m => m.MarAmount);

        //    return objAbstractFund;



       // }

        public Array GetAbstractFundDetails(Int16 Year, Int16 Head, int AdminNdCode, int LowerAdminNdCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            PMGSYEntities dbContext=new PMGSYEntities();

            try
            {
                var lstAbstractFund = dbContext.USP_ACC_RPT_REGISTER_ABSTRACT_PIUWISE_FUND_TRANSFERRED(PMGSYSession.Current.FundType, Year, Head, AdminNdCode, LowerAdminNdCode).ToList();

                totalRecords = lstAbstractFund.Count;

                //var result = lstAbstractFund.Select(m => new { 

                //    m.ADMIN_ND_NAME,
                //    m.AprAmount,
                //    m.MayAmount,
                //    m.JuneAmount,
                //    m.JulyAmount,
                //    m.AugAmount,
                //    m.SeptAmount,
                //    m.OctAmount,
                //    m.NovAmount,
                //    m.DecAmount,
                //    m.JanAmount,
                //    m.FebAmount,
                //    m.MarAmount,

                //}).ToArray();

                return lstAbstractFund.Select(details => new
                {
                    cell = new[]
                {
                    details.ADMIN_ND_NAME == null?string.Empty:details.ADMIN_ND_NAME.ToString(),
                    string.Empty,
                    details.AprAmount== null?string.Empty:details.AprAmount.ToString(),
                    details.MayAmount== null?string.Empty:(details.MayAmount + details.AprAmount).ToString(),
                    details.JuneAmount== null?string.Empty:(details.AprAmount+details.MayAmount+ details.JuneAmount).ToString(),
                    details.JulyAmount== null?string.Empty:(details.AprAmount +details.MayAmount +details.JuneAmount+ details.JulyAmount).ToString(),
                    details.AugAmount== null?string.Empty:(details.AprAmount +details.MayAmount +details.JuneAmount+ details.JulyAmount + details.AugAmount).ToString(),
                    details.SeptAmount== null?string.Empty:(details.AprAmount+details.MayAmount+details.JuneAmount+details.JulyAmount+details.AugAmount+details.SeptAmount).ToString(),
                    details.OctAmount== null?string.Empty:(details.AprAmount+details.MayAmount+details.JuneAmount+details.JulyAmount+details.AugAmount+details.SeptAmount+ details.OctAmount).ToString(),
                    details.NovAmount== null?string.Empty:(details.AprAmount+details.MayAmount+details.JuneAmount+details.JulyAmount+details.AugAmount+details.SeptAmount+details.OctAmount+ details.NovAmount).ToString(),
                    details.DecAmount== null?string.Empty:(details.AprAmount+details.MayAmount+details.JuneAmount+details.JulyAmount+details.AugAmount+details.SeptAmount+details.OctAmount+details.NovAmount+details.DecAmount).ToString(),
                    details.JanAmount== null?string.Empty:(details.AprAmount+details.MayAmount+details.JuneAmount+details.JulyAmount+details.AugAmount+details.SeptAmount+details.OctAmount+details.NovAmount+details.DecAmount+details.JanAmount).ToString(),
                    details.FebAmount== null?string.Empty:(details.AprAmount+details.MayAmount+details.JuneAmount+details.JulyAmount+details.AugAmount+details.SeptAmount+details.OctAmount+details.NovAmount+details.DecAmount+details.JanAmount+details.FebAmount).ToString(),
                    details.MarAmount== null?string.Empty:(details.AprAmount+details.MayAmount+details.JuneAmount+details.JulyAmount+details.AugAmount+details.SeptAmount+details.OctAmount+details.NovAmount+details.DecAmount+details.JanAmount+details.FebAmount+details.MarAmount).ToString(),
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

        public AbstractFundTransferredViewModel GetReportHeaderInfo(AbstractFundTransferredViewModel objAbstractFund)
        {
            PMGSYEntities dbContext = new PMGSYEntities();

            try
            {
                // AbstractFundTransferredViewModel objAbstractFund = new AbstractFundTransferredViewModel();
                var lstHeader = new SP_ACC_Get_Report_Header_Information_Result();
                if (PMGSYSession.Current.LevelId == 6)
                {
                    lstHeader = dbContext.SP_ACC_Get_Report_Header_Information("AccountsReports", "AbstractFundView", PMGSYSession.Current.FundType, 4, "A").FirstOrDefault();
                }
                else
                {
                    lstHeader = dbContext.SP_ACC_Get_Report_Header_Information("AccountsReports", "AbstractFundView", PMGSYSession.Current.FundType, PMGSYSession.Current.LevelId, "A").FirstOrDefault();
                }

                if (lstHeader != null)
                {
                    objAbstractFund.ReportName = lstHeader.REPORT_NAME;
                    objAbstractFund.ReportNumber = lstHeader.REPORT_FORM_NO;
                    objAbstractFund.ReportPara = lstHeader.REPORT_PARAGRAPH_NAME;
                    objAbstractFund.FundName = commomFuncObj.GetFundName(PMGSYSession.Current.FundType);
                }
                else
                {
                    objAbstractFund.ReportName = string.Empty;
                    objAbstractFund.ReportNumber = string.Empty;
                    objAbstractFund.ReportPara = string.Empty;
                }


                return objAbstractFund;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);

                throw;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }

          
        }
        

        #endregion AbstractFundTransfer

        #region BankAuthrization

        public BankAuthrizationViewModel GetAuthrizationDetails(ReportFilter objParam)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            BankAuthrizationViewModel objBankAuthrization = new BankAuthrizationViewModel();
            try
            {
                objBankAuthrization.lstBankAuthrization = dbContext.USP_ACC_RPT_REGISTER_PIUWISE_BANK_AUTHORIZATION_ISSUED(PMGSYSession.Current.FundType, objParam.Month, objParam.Year, objParam.AdminNdCode, objParam.LowerAdminNdCode).ToList<USP_ACC_RPT_REGISTER_PIUWISE_BANK_AUTHORIZATION_ISSUED_Result>();

                objBankAuthrization.TotalOpeningBalance = 0;
                objBankAuthrization.TotalCredit = 0;
                objBankAuthrization.TotalDebit = 0;
                foreach (var item in objBankAuthrization.lstBankAuthrization)
                {
                    //modified by abhishek kamble 5-dec-2013
                    objBankAuthrization.TotalOpeningBalance = item.CREDIT_AMT;
                    break;                    
                }
                var lstHeader =new  SP_ACC_Get_Report_Header_Information_Result();
                if (PMGSYSession.Current.LevelId == 6)
                {
                    lstHeader = dbContext.SP_ACC_Get_Report_Header_Information("AccountsReports", "BankAuthrizationView", PMGSYSession.Current.FundType,4, "A").FirstOrDefault();
                }
                else
                {
                     lstHeader = dbContext.SP_ACC_Get_Report_Header_Information("AccountsReports", "BankAuthrizationView", PMGSYSession.Current.FundType, PMGSYSession.Current.LevelId, "A").FirstOrDefault();
                }
               
                if (lstHeader != null)
                {
                    objBankAuthrization.ReportNumber = lstHeader.REPORT_FORM_NO;
                    objBankAuthrization.ReportName = lstHeader.REPORT_NAME;
                    objBankAuthrization.ReportPara = lstHeader.REPORT_PARAGRAPH_NAME;
                    objBankAuthrization.FundName = commomFuncObj.GetFundName(PMGSYSession.Current.FundType);
                }
                else
                {
                    objBankAuthrization.ReportNumber = string.Empty;
                    objBankAuthrization.ReportName = string.Empty;
                    objBankAuthrization.ReportPara = string.Empty;
                }
                objBankAuthrization.TotalRecord = objBankAuthrization.lstBankAuthrization.Count();
                objBankAuthrization.DPIUName = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == objParam.LowerAdminNdCode).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
                objBankAuthrization.SRRDAName = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == (dbContext.ADMIN_DEPARTMENT.Where(p => p.ADMIN_ND_CODE == objParam.LowerAdminNdCode).Select(p => p.MAST_PARENT_ND_CODE).FirstOrDefault())).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
                return objBankAuthrization;
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

        #endregion BankAuthrization


        #region Abstract Bank Auth Details

            /// <summary>
            /// Set Report Header
            /// </summary>
            /// <param name="abstractBankAuthDetails"></param>
            /// <returns></returns>
            public AbstractBankAuthViewModel AbstractBankAuthDetails(AbstractBankAuthViewModel abstractBankAuthDetails)
            {

                dbcontext = new PMGSYEntities();
                try
                {
                    CommonFunctions objCommonFunction = new CommonFunctions();
                    var ReportHeader=new SP_ACC_Get_Report_Header_Information_Result();

                    if (PMGSYSession.Current.LevelId == 6)
                    {
                        ReportHeader= dbcontext.SP_ACC_Get_Report_Header_Information("AccountsReports", "AbstractBankAuthorization", PMGSYSession.Current.FundType,4, "O").ToList<SP_ACC_Get_Report_Header_Information_Result>().FirstOrDefault();
                    }
                    else {
                        ReportHeader = dbcontext.SP_ACC_Get_Report_Header_Information("AccountsReports", "AbstractBankAuthorization", PMGSYSession.Current.FundType, PMGSYSession.Current.LevelId, "O").ToList<SP_ACC_Get_Report_Header_Information_Result>().FirstOrDefault();
                    }

                    if (ReportHeader == null)
                    {
                        abstractBankAuthDetails.FundTypeName = objCommonFunction.GetFundName(PMGSYSession.Current.FundType);
                        abstractBankAuthDetails.ReportFormNumber = "-";
                        abstractBankAuthDetails.ReportName = "-";
                        abstractBankAuthDetails.ReportParagraphName = "-";
                    }
                    else
                    {
                        abstractBankAuthDetails.FundTypeName = objCommonFunction.GetFundName(PMGSYSession.Current.FundType);
                        abstractBankAuthDetails.ReportFormNumber = ReportHeader.REPORT_FORM_NO;
                        abstractBankAuthDetails.ReportName = ReportHeader.REPORT_NAME;
                        abstractBankAuthDetails.ReportParagraphName = ReportHeader.REPORT_PARAGRAPH_NAME;
                    }

                    return abstractBankAuthDetails;
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);    
                    return null;
                }
                finally
                {

                    if (dbcontext != null)
                    {
                        dbcontext.Dispose();
                    }

                }
            }

            //display Abstract Bank Authorization Details
            public Array ListAbstractBankAuthDetails(short Year,int StateCode,int DPIU,int page,int rows,string sidx,string sord,out long totalRecords)
            {

                dbcontext = new PMGSYEntities();

                try {

                    var lstAbstractBankAuthDetails = dbcontext.USP_ACC_RPT_REGISTER_ABSTRACT_PIUWISE_BANK_AUTHORIZATION(PMGSYSession.Current.FundType,Year,StateCode,DPIU).ToList();
                    totalRecords = lstAbstractBankAuthDetails.Count();

                    return lstAbstractBankAuthDetails.Select(bankDetails => new
                        {
                            cell = new[]
                             {                                  
                                     bankDetails.ADMIN_ND_NAME==null?string.Empty:bankDetails.ADMIN_ND_NAME.ToString(),
                                     String.Empty,
                                     bankDetails.AprAmount==null?string.Empty:bankDetails.AprAmount.ToString(),
                                     bankDetails.MayAmount==null?string.Empty:(bankDetails.AprAmount+bankDetails.MayAmount).ToString(),
                                     bankDetails.JuneAmount==null?string.Empty:(bankDetails.AprAmount+bankDetails.MayAmount+bankDetails.JuneAmount).ToString(),
                                     bankDetails.JulyAmount==null?string.Empty: (bankDetails.AprAmount+bankDetails.MayAmount+bankDetails.JuneAmount+bankDetails.JulyAmount).ToString(),
                                     bankDetails.AugAmount==null?string.Empty:(bankDetails.AprAmount+bankDetails.MayAmount+bankDetails.JuneAmount+bankDetails.JulyAmount+bankDetails.AugAmount).ToString(),
                                     bankDetails.SeptAmount==null?string.Empty:(bankDetails.AprAmount+bankDetails.MayAmount+bankDetails.JuneAmount+bankDetails.JulyAmount+bankDetails.AugAmount+bankDetails.SeptAmount).ToString(),
                                     bankDetails.OctAmount==null?string.Empty:(bankDetails.AprAmount+bankDetails.MayAmount+bankDetails.JuneAmount+bankDetails.JulyAmount+bankDetails.AugAmount+bankDetails.SeptAmount+bankDetails.OctAmount).ToString(),
                                     bankDetails.NovAmount==null?string.Empty:(bankDetails.AprAmount+bankDetails.MayAmount+bankDetails.JuneAmount+bankDetails.JulyAmount+bankDetails.AugAmount+bankDetails.SeptAmount+bankDetails.OctAmount+bankDetails.NovAmount).ToString(),
                                     bankDetails.DecAmount==null?string.Empty:(bankDetails.AprAmount+bankDetails.MayAmount+bankDetails.JuneAmount+bankDetails.JulyAmount+bankDetails.AugAmount+bankDetails.SeptAmount+bankDetails.OctAmount+bankDetails.NovAmount+bankDetails.DecAmount).ToString(),
                                     bankDetails.JanAmount==null?string.Empty:(bankDetails.AprAmount+bankDetails.MayAmount+bankDetails.JuneAmount+bankDetails.JulyAmount+bankDetails.AugAmount+bankDetails.SeptAmount+bankDetails.OctAmount+bankDetails.NovAmount+bankDetails.DecAmount+ bankDetails.JanAmount).ToString(),
                                     bankDetails.FebAmount==null?string.Empty:(bankDetails.AprAmount+bankDetails.MayAmount+bankDetails.JuneAmount+bankDetails.JulyAmount+bankDetails.AugAmount+bankDetails.SeptAmount+bankDetails.OctAmount+bankDetails.NovAmount+bankDetails.DecAmount+ bankDetails.JanAmount+bankDetails.FebAmount).ToString(),
                                     bankDetails.MarAmount==null?string.Empty:(bankDetails.AprAmount+bankDetails.MayAmount+bankDetails.JuneAmount+bankDetails.JulyAmount+bankDetails.AugAmount+bankDetails.SeptAmount+bankDetails.OctAmount+bankDetails.NovAmount+bankDetails.DecAmount+ bankDetails.JanAmount+bankDetails.FebAmount+bankDetails.MarAmount).ToString(),
                             }
                        }).ToArray();
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    totalRecords = 0;
                    return null;
                }
                finally {
                    if (dbcontext != null) {
                        dbcontext.Dispose();
                    }            
                }
        
            }
            
        #endregion

        #region ScheduleRoad
            public List<SelectListItem> lstFundingAgency()
            {
                dbcontext = new PMGSYEntities();
                try
                {
                    return new SelectList(dbcontext.MASTER_FUNDING_AGENCY.ToList(),"MAST_FUNDING_AGENCY_CODE","MAST_FUNDING_AGENCY_NAME").ToList();
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);    
            
                    return null;
                }
                finally
                {
                    dbcontext.Dispose();
                }
            }

            public List<SelectListItem> PopulateHead(int[] headId)
            {
                dbcontext = new PMGSYEntities();
                try
                {
                    //List<SelectListItem> lstDetails = new List<SelectListItem>();
                    var lstDetails = (from item in dbcontext.ACC_MASTER_HEAD
                                 where headId.Contains(item.HEAD_ID)
                                 select new 
                                 {
                                     HEAD_CODE = item.HEAD_ID,
                                     HEAD_NAME = item.HEAD_NAME
                                 }).ToList();
                   
                    //foreach (var item in headId)
                    //{
                    //  lstDetails.Add(new SelectList(dbcontext.ACC_MASTER_HEAD.Where(m => m.HEAD_ID == item).ToList(),"HEAD_ID","HEAD_NAME").ToList();
                    //}
                    //return null;

                    return new SelectList(lstDetails, "HEAD_CODE", "HEAD_NAME").ToList();

                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);    
                                return null;
                }
                finally
                {
                    dbcontext.Dispose();
                }
            }

            public Array ListScheduleDetails(int? state, int? month, int? year,Int16? head,string fundingAgenCode,int? page, int? rows, string sord, string sidx, out long totalRecords)
            {
                dbcontext = new PMGSYEntities();               
                try
                {
                    var lstSchedule = dbcontext.USP_ACC_RPT_SCHEDULE_OF_ROADS(state, month, year, PMGSYSession.Current.FundType, PMGSYSession.Current.LevelId, head, fundingAgenCode).ToList();

                    

                    totalRecords = lstSchedule.Count();

                    return lstSchedule.Select(details => new
                    {
                        cell=new[]
                        {
                            details.FA_CODE.ToString(),
                           "Roads funded by the:" +details.FA_NAME,
                            details.DURATION.ToString()=="-1"?"Completed Roads upto the end of the last month/year":details.DURATION.ToString()=="0"?"Completed during the month/year":"Roads in Progress",
                            details.FLAG.ToString(),
                            details.COMPLETION_STATUS,
                           "Phase:" +details.PHASE.ToString(),
                           "Package:" +details.PACKAGE_NUMBER.ToString(),
                            details.ROAD_CODE.ToString(),
                            details.IMS_ROAD_NAME,
                            details.AMOUNT.ToString(),
                            string.Empty,
                            string.Empty
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
                    dbcontext.Dispose();
                }

            }


            public ScheduleModel GetScheduleList(ReportFilter objParam)
            {
                dbcontext=new PMGSYEntities();

                try
                {
                    ScheduleModel objSchedule = new ScheduleModel();
                    objSchedule.lstSchedule = dbcontext.USP_ACC_RPT_SCHEDULE_OF_ROADS(objParam.LowerAdminNdCode, objParam.Month, objParam.Year, PMGSYSession.Current.FundType, 5, objParam.Head, objParam.AgencyCode).ToList<USP_ACC_RPT_SCHEDULE_OF_ROADS_Result>();
                    objSchedule.PiuName = dbcontext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == objParam.LowerAdminNdCode).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
                    objSchedule.StateName = dbcontext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == (dbcontext.ADMIN_DEPARTMENT.Where(p => p.ADMIN_ND_CODE == objParam.LowerAdminNdCode).Select(p => p.MAST_PARENT_ND_CODE).FirstOrDefault())).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();

                    var rptHeader = new SP_ACC_Get_Report_Header_Information_Result();
                    if (PMGSYSession.Current.LevelId == 6)
                    {
                        rptHeader = dbcontext.SP_ACC_Get_Report_Header_Information("AccountsReports", objParam.Selection, PMGSYSession.Current.FundType, 4, "A").FirstOrDefault();
                    }
                    else
                    {
                        rptHeader = dbcontext.SP_ACC_Get_Report_Header_Information("AccountsReports", objParam.Selection, PMGSYSession.Current.FundType, PMGSYSession.Current.LevelId, "A").FirstOrDefault();
                    }

                    if (rptHeader != null)
                    {
                        objSchedule.FormNumber = rptHeader.REPORT_FORM_NO;
                        objSchedule.Paragraph1 = rptHeader.REPORT_PARAGRAPH_NAME;
                        objSchedule.Header = rptHeader.REPORT_NAME;
                        objSchedule.FundType = commomFuncObj.GetFundName(PMGSYSession.Current.FundType);
                    }
                    else
                    {
                        objSchedule.FormNumber = string.Empty;
                        objSchedule.Paragraph1 = string.Empty;
                        objSchedule.Header = string.Empty;
                    }
                    return objSchedule;
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);

                    return null;
                }
                finally
                {
                    dbcontext.Dispose();
                }

               
            }
        #endregion ScheduleRoad

        #region MasterSheet

            /// <summary>
            /// Master Sheet Report
            /// </summary>
            /// <param name="year"></param>
            /// <returns></returns>
            public List<USP_ACC_MASTER_SHEET_PIU_RPT_Result> MasterSheetDAL(int year)
            {
                dbcontext = new PMGSYEntities();
                try
                {
                    return dbcontext.Database.SqlQuery<USP_ACC_MASTER_SHEET_PIU_RPT_Result>("EXEC omms.USP_ACC_MASTER_SHEET_RPT @Level,@State,@Agency,@Year,@Type",
                        new SqlParameter("Level", 2),
                        new SqlParameter("State", PMGSYSession.Current.StateCode),
                        new SqlParameter("Agency", dbcontext.ADMIN_DEPARTMENT.Where(c => c.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).Select(c => c.MAST_AGENCY_CODE).First()),
                        new SqlParameter("Year", year),
                        new SqlParameter("Type", PMGSYSession.Current.FundType)
                        ).ToList<USP_ACC_MASTER_SHEET_PIU_RPT_Result>();
                    
                    //return dbcontext.USP_ACC_MASTER_SHEET_RPT(2, 26, 22, 2011, "P").ToList<USP_ACC_MASTER_SHEET_PIU_RPT_Result>();

                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);    
                    return null;
                }
                finally
                {
                    if (dbcontext != null)
                    {
                        dbcontext.Dispose();
                    }
                }
            }
        #endregion 

        #region Final Bill Payment

            public List<SelectListItem> PopulateAgency(int stateCode)
            {
                dbcontext = new PMGSYEntities();
                try
                {
                    List<SelectListItem> lstAgency = new List<SelectListItem>();
                    if (stateCode != 0)
                    {
                        var lstData = (from item in dbcontext.ADMIN_DEPARTMENT
                                       where item.MAST_ND_TYPE == "S" &&
                                             item.MAST_STATE_CODE == stateCode
                                       select new
                                       {
                                           AgencyName = item.MASTER_AGENCY.MAST_AGENCY_NAME,
                                           AgencyCode = item.MASTER_AGENCY.MAST_AGENCY_CODE
                                       }).OrderBy(m => m.AgencyName).ToList().Distinct();


                        foreach (var item in lstData)
                        {
                            lstAgency.Add(new SelectListItem { Text = item.AgencyName, Value = item.AgencyCode.ToString() });
                        }
                    }
                    lstAgency.Insert(0, new SelectListItem { Text="Select Agency", Value="0"});
                    return lstAgency;
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    return null;
                }
                finally {
                    if (dbcontext != null)
                    {
                        dbcontext.Dispose();
                    }
                }
            }

            public FinalBillPaymentModel FinalBillPaymentHeaderInformation(FinalBillPaymentModel finalBillPaymentModel)
            {
                dbcontext = new PMGSYEntities();
                try
                {
                    CommonFunctions objCommonFunction = new CommonFunctions();
                    var ReportHeader = new SP_ACC_Get_Report_Header_Information_Result();

                    if (PMGSYSession.Current.LevelId == 6)
                    {
                        ReportHeader = dbcontext.SP_ACC_Get_Report_Header_Information("AccountsReports", "FinalBillPayment", PMGSYSession.Current.FundType, 4, "O").ToList<SP_ACC_Get_Report_Header_Information_Result>().FirstOrDefault();
                    }
                    else
                    {
                        ReportHeader = dbcontext.SP_ACC_Get_Report_Header_Information("AccountsReports", "FinalBillPayment", PMGSYSession.Current.FundType, PMGSYSession.Current.LevelId, "S").ToList<SP_ACC_Get_Report_Header_Information_Result>().FirstOrDefault();
                    }

                    if (ReportHeader == null)
                    {
                        finalBillPaymentModel.FundTypeName = objCommonFunction.GetFundName(PMGSYSession.Current.FundType);
                        finalBillPaymentModel.ReportFormNumber = "-";
                        finalBillPaymentModel.ReportName = "-";
                        finalBillPaymentModel.ReportParagraphName = "-";
                    }
                    else
                    {
                        finalBillPaymentModel.FundTypeName = objCommonFunction.GetFundName(PMGSYSession.Current.FundType);
                        finalBillPaymentModel.ReportFormNumber = ReportHeader.REPORT_FORM_NO;
                        finalBillPaymentModel.ReportName = ReportHeader.REPORT_NAME;
                        finalBillPaymentModel.ReportParagraphName = ReportHeader.REPORT_PARAGRAPH_NAME;
                    }

                    return finalBillPaymentModel;
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    return null;
                }
                finally
                {

                    if (dbcontext != null)
                    {
                        dbcontext.Dispose();
                    }

                }
            }

            public Array ListFinalBillPaymentCompletedDetails(int StateCode, int AgencyCode, int FundingAgencyCode, int page, int rows, string sidx, string sord, out long totalRecords)
            {
                dbcontext = new PMGSYEntities();
                //var objectContext = (dbcontext as IObjectContextAdapter).ObjectContext;
                //objectContext.CommandTimeout = 0;
                
                ((IObjectContextAdapter)dbcontext).ObjectContext.CommandTimeout = 360;
                dbcontext.Configuration.LazyLoadingEnabled = false;

                try
                {
                    //var lstFinalBillPaymentCompletedDetails = new USP_ACC_RPT_FINANCIAL_CLOSURE_Result().ToString().ToList();
                    
                    var lstFinalBillPaymentCompletedDetails = dbcontext.USP_ACC_RPT_FINANCIAL_CLOSURE(StateCode, AgencyCode, FundingAgencyCode, PMGSYSession.Current.PMGSYScheme).ToList();

                    //var lstFinalBillPaymentCompletedDetails = dbcontext.Database.SqlQuery<USP_ACC_RPT_FINANCIAL_CLOSURE_Result>("EXEC omms.USP_ACC_RPT_FINANCIAL_CLOSURE @P_INT_StateCode,@P_INT_AgencyCode,@P_INT_FACode,@P_INT_PMGSY_Version",
                    //    new SqlParameter("P_INT_StateCode", StateCode),
                    //    new SqlParameter("P_INT_AgencyCode", AgencyCode),
                    //    new SqlParameter("P_INT_FACode", FundingAgencyCode),
                    //    new SqlParameter("P_INT_PMGSY_Version", PMGSYSession.Current.PMGSYScheme)).ToList();

                    //string connectionString = "data source=10.208.36.217;initial catalog=OMMAS_DEV;persist security info=True;user id=omms;password=omms@sql2012;MultipleActiveResultSets=True;App=EntityFramework";

                    //using(SqlConnection connection=new SqlConnection(connectionString)){
                    //connection.Open();
                    //    using (SqlCommand command = new SqlCommand("omms.USP_ACC_RPT_FINANCIAL_CLOSURE", connection))
                    //    {
                    //        command.CommandType = CommandType.StoredProcedure;
                    //        command.Parameters.Add("@P_INT_StateCode", SqlDbType.Int).Value = StateCode;
                    //        command.Parameters.Add("@P_INT_AgencyCode", SqlDbType.Int).Value = AgencyCode;
                    //        command.Parameters.Add("@P_INT_FACode", SqlDbType.Int).Value = FundingAgencyCode;
                    //        command.Parameters.Add("@P_INT_PMGSY_Version", SqlDbType.Int).Value = PMGSYSession.Current.PMGSYScheme;
                    //        command.CommandTimeout = 0;
                    //       var data = command.ExecuteReader();

                    //    }
                    //}

                    totalRecords = lstFinalBillPaymentCompletedDetails.Count();

                    if (sidx.Trim() != string.Empty)
                    {

                        if (sord.ToString() == "asc")
                        {
                            switch (sidx)
                            {
                                case "SanctionedYear":
                                    lstFinalBillPaymentCompletedDetails = lstFinalBillPaymentCompletedDetails.OrderBy(x => x.IMS_YEAR).ToList();
                                    break;
                                case "SanctionedWorks":
                                    lstFinalBillPaymentCompletedDetails = lstFinalBillPaymentCompletedDetails.OrderBy(x => x.SANCTIONED_WORKS).ToList();
                                    break;
                                case "PhyCompletedWorks":
                                    lstFinalBillPaymentCompletedDetails = lstFinalBillPaymentCompletedDetails.OrderBy(x => x.PHY_COMP).ToList();
                                    break;
                                case "FinCompOfPhyCompletedWorks":
                                    lstFinalBillPaymentCompletedDetails = lstFinalBillPaymentCompletedDetails.OrderBy(x => x.ACC_COMP).ToList();
                                    break;
                                case "PendingFinCompOfPhyCompletedWorks":
                                    lstFinalBillPaymentCompletedDetails = lstFinalBillPaymentCompletedDetails.OrderBy(x => x.PEND_ACC_COMP).ToList();
                                    break;
                                default:
                                    lstFinalBillPaymentCompletedDetails = lstFinalBillPaymentCompletedDetails.OrderBy(x => x.IMS_YEAR).ToList();
                                    break;
                            }

                        }
                        else
                        {
                            switch (sidx)
                            {
                                case "SanctionedYear":
                                    lstFinalBillPaymentCompletedDetails = lstFinalBillPaymentCompletedDetails.OrderByDescending(x => x.IMS_YEAR).ToList();
                                    break;
                                case "SanctionedWorks":
                                    lstFinalBillPaymentCompletedDetails = lstFinalBillPaymentCompletedDetails.OrderByDescending(x => x.SANCTIONED_WORKS).ToList();
                                    break;
                                case "PhyCompletedWorks":
                                    lstFinalBillPaymentCompletedDetails = lstFinalBillPaymentCompletedDetails.OrderByDescending(x => x.PHY_COMP).ToList();
                                    break;
                                case "FinCompOfPhyCompletedWorks":
                                    lstFinalBillPaymentCompletedDetails = lstFinalBillPaymentCompletedDetails.OrderByDescending(x => x.ACC_COMP).ToList();
                                    break;
                                case "PendingFinCompOfPhyCompletedWorks":
                                    lstFinalBillPaymentCompletedDetails = lstFinalBillPaymentCompletedDetails.OrderByDescending(x => x.PEND_ACC_COMP).ToList();
                                    break;
                                default:
                                    lstFinalBillPaymentCompletedDetails = lstFinalBillPaymentCompletedDetails.OrderByDescending(x => x.IMS_YEAR).ToList();
                                    break;
                            }
                        }
                    }
                    else
                    {
                        lstFinalBillPaymentCompletedDetails = lstFinalBillPaymentCompletedDetails.OrderByDescending(x => x.IMS_YEAR).ToList();
                    }

                    return lstFinalBillPaymentCompletedDetails.Select(BillPaymentDetails => new
                    {
                        cell = new[]
                             {                                  
                                     BillPaymentDetails.IMS_YEAR.ToString(),
                                     BillPaymentDetails.SANCTIONED_WORKS.ToString(),
                                     BillPaymentDetails.PHY_COMP.ToString(),
                                     BillPaymentDetails.ACC_COMP.ToString(),
                                    //"<a href='#'  class='ui-icon ui-icon-zoomin ui-align-center' onClick='ShowDetails(\"" + propDetails.IMS_PR_ROAD_CODE.ToString().Trim() + "$" + propDetails.IMS_LOCK_STATUS +"\"); return false;'>Show Details</a>",
                                    BillPaymentDetails.PEND_ACC_COMP==0?BillPaymentDetails.PEND_ACC_COMP.ToString():BillPaymentDetails.PEND_ACC_COMP==null?"0":("<a href='#' onClick='ShowDetails(\""+StateCode.ToString().Trim()+"$"+AgencyCode.ToString().Trim()+"$"+FundingAgencyCode.ToString().Trim()+"$"+BillPaymentDetails.IMS_YEAR.ToString().Trim()+"\");return false;'>"+BillPaymentDetails.PEND_ACC_COMP.ToString()+"</a>"),
                             }
                    }).ToArray();

                    //return lstFinalBillPaymentCompletedDetails.ToArray();

                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    totalRecords = 0;
                    return null;
                }
                finally
                {
                    if (dbcontext != null)
                    {
                        dbcontext.Dispose();
                    }
                }

            }

            public Array ListFinalBillPaymentPendingDetails(int StateCode, int AgencyCode, int FundingAgencyCode, int Year, int page, int rows, string sidx, string sord, out long totalRecords)
            {
                dbcontext = new PMGSYEntities();

                try
                {
                    
                    var lstPendingFinalBillPaymentDetails = dbcontext.USP_ACC_RPT_FPP_PENDING_ROAD_DETAILS(StateCode, AgencyCode,FundingAgencyCode,Year, PMGSYSession.Current.PMGSYScheme).ToList();
                    totalRecords = lstPendingFinalBillPaymentDetails.Count();

                    lstPendingFinalBillPaymentDetails = lstPendingFinalBillPaymentDetails.OrderBy(o => o.DISTRICT_NAME).ThenBy(t => t.PACKAGE_ID).ToList();
                    
                    //query = query.OrderBy(c => c.CASH_CHQ == "Q").ThenBy(t => t.CASH_CHQ == "C").ThenBy(t => t.CASH_CHQ == "D");

                    //if (sidx.Trim() != string.Empty)
                    //{

                    //    if (sord.ToString() == "asc")
                    //    {
                    //        switch (sidx)
                    //        {
                    //            case "DistrictName":
                    //                lstPendingFinalBillPaymentDetails = lstPendingFinalBillPaymentDetails.OrderBy(x => x.DISTRICT_NAME).ToList();
                    //                break;
                    //            case "PackageNumber":
                    //                lstPendingFinalBillPaymentDetails = lstPendingFinalBillPaymentDetails.OrderBy(x => x.PACKAGE_ID).ToList();                                    
                    //                break;
                    //            case "RoadName":
                    //                lstPendingFinalBillPaymentDetails = lstPendingFinalBillPaymentDetails.OrderBy(x => x.ROAD_NAME).ToList();                                    
                    //                break;
                    //            case "FinancialRoadCategory":
                    //                lstPendingFinalBillPaymentDetails = lstPendingFinalBillPaymentDetails.OrderBy(x => x.CONNECTIVITY).ToList();                                    
                    //                break;
                    //            case "PavementLength":
                    //                lstPendingFinalBillPaymentDetails = lstPendingFinalBillPaymentDetails.OrderBy(x => x.PAVEMENT_LENGTH).ToList();                                    
                    //                break;
                    //            case "SanctionCost":
                    //                lstPendingFinalBillPaymentDetails = lstPendingFinalBillPaymentDetails.OrderBy(x => x.SANCTION_COST).ToList();
                    //                break;
                    //            case "Expenditure":
                    //                lstPendingFinalBillPaymentDetails = lstPendingFinalBillPaymentDetails.OrderBy(x => x.EXPENDITURE).ToList();
                    //                break;
                    //            default:
                    //                lstPendingFinalBillPaymentDetails = lstPendingFinalBillPaymentDetails.OrderBy(x => x.DISTRICT_NAME).ToList();
                    //                break;
                    //        }

                    //    }
                    //    else
                    //    {
                    //        switch (sidx)
                    //        {
                    //            case "DistrictName":
                    //                lstPendingFinalBillPaymentDetails = lstPendingFinalBillPaymentDetails.OrderByDescending(x => x.DISTRICT_NAME).ToList();
                    //                break;
                    //            case "PackageNumber":
                    //                lstPendingFinalBillPaymentDetails = lstPendingFinalBillPaymentDetails.OrderByDescending(x => x.PACKAGE_ID).ToList();
                    //                break;
                    //            case "RoadName":
                    //                lstPendingFinalBillPaymentDetails = lstPendingFinalBillPaymentDetails.OrderByDescending(x => x.ROAD_NAME).ToList();
                    //                break;
                    //            case "FinancialRoadCategory":
                    //                lstPendingFinalBillPaymentDetails = lstPendingFinalBillPaymentDetails.OrderByDescending(x => x.CONNECTIVITY).ToList();
                    //                break;
                    //            case "PavementLength":
                    //                lstPendingFinalBillPaymentDetails = lstPendingFinalBillPaymentDetails.OrderByDescending(x => x.PAVEMENT_LENGTH).ToList();
                    //                break;
                    //            case "SanctionCost":
                    //                lstPendingFinalBillPaymentDetails = lstPendingFinalBillPaymentDetails.OrderByDescending(x => x.SANCTION_COST).ToList();
                    //                break;
                    //            case "Expenditure":
                    //                lstPendingFinalBillPaymentDetails = lstPendingFinalBillPaymentDetails.OrderByDescending(x => x.EXPENDITURE).ToList();
                    //                break;
                    //            default:
                    //                lstPendingFinalBillPaymentDetails = lstPendingFinalBillPaymentDetails.OrderByDescending(x => x.DISTRICT_NAME).ToList();
                    //                break;
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    lstPendingFinalBillPaymentDetails = lstPendingFinalBillPaymentDetails.OrderByDescending(x => x.DISTRICT_NAME).ToList();

                    //}

                    return lstPendingFinalBillPaymentDetails.Select(BillPaymentDetails => new
                    {
                        cell = new[]
                             {                                  
                                     BillPaymentDetails.DISTRICT_NAME.ToString(),
                                     BillPaymentDetails.PACKAGE_ID.ToString(),
                                     BillPaymentDetails.ROAD_NAME.ToString(),
                                     BillPaymentDetails.CONNECTIVITY.ToString(),
                                     BillPaymentDetails.PAVEMENT_LENGTH.ToString(),
                                     BillPaymentDetails.SANCTION_COST.ToString(),
                                     BillPaymentDetails.EXPENDITURE.ToString(),
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
                    if (dbcontext != null)
                    {
                        dbcontext.Dispose();
                    }
                }

            }
                  
                      
        #endregion Final Bill Payment

        #region State Account Monitoring   
        
            public List<SelectListItem> PopulateAgencyByStateCode(int stateCode)
            {
                dbcontext = new PMGSYEntities();
                try
                {
                    List<SelectListItem> lstAgency = new SelectList(dbcontext.USP_ACC_DISPLAY_AGENCIES_DETAILS(stateCode), "MAST_AGENCY_CODE", "ADMIN_ND_NAME").OrderBy(o => o.Text).ToList();
                    lstAgency.Insert(0, new SelectListItem { Text = "Select Agency", Value = "0" });
                    return lstAgency;

                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    return null;
                }
                finally {
                    if (dbcontext != null)
                    {
                        dbcontext.Dispose();
                    }
                }
            }

            public List<SelectListItem> PopulateFundType()
            {
                dbcontext = new PMGSYEntities();
                try
                {
                    List<SelectListItem> lstFundType = new List<SelectListItem>();

                    lstFundType.Add(new SelectListItem { Text = "Select Fund Type" ,Value ="0" });
                    lstFundType.Add(new SelectListItem { Text = "Programme Fund", Value = "P" });
                    lstFundType.Add(new SelectListItem { Text = "Administrative Fund", Value = "A" });
                    lstFundType.Add(new SelectListItem { Text = "Maintenance Fund", Value = "M" });

                    return lstFundType;
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    return null;
                }
                finally {

                    if (dbcontext != null)
                    {
                        dbcontext.Dispose();
                    }                
                }
            }

            public StateAccountMonitoringViewModel StateAccMonitoringHeaderInformation(StateAccountMonitoringViewModel stateAccountViewModel)
            {
                dbcontext = new PMGSYEntities();
                try
                {
                    CommonFunctions objCommonFunction = new CommonFunctions();
                    var ReportHeader = new SP_ACC_Get_Report_Header_Information_Result();

                    if (PMGSYSession.Current.LevelId == 6)
                    {
                        ReportHeader = dbcontext.SP_ACC_Get_Report_Header_Information("AccountsReports", "StateAccountMonitoring", PMGSYSession.Current.FundType, 4, "S").ToList<SP_ACC_Get_Report_Header_Information_Result>().FirstOrDefault();
                    }
                    else
                    {
                        ReportHeader = dbcontext.SP_ACC_Get_Report_Header_Information("AccountsReports", "StateAccountMonitoring", PMGSYSession.Current.FundType, PMGSYSession.Current.LevelId, "O").ToList<SP_ACC_Get_Report_Header_Information_Result>().FirstOrDefault();
                    }

                    if (ReportHeader == null)
                    {
                        stateAccountViewModel.FundTypeName = objCommonFunction.GetFundName(PMGSYSession.Current.FundType);
                        stateAccountViewModel.ReportFormNumber = "-";
                        stateAccountViewModel.ReportName = "-";
                        stateAccountViewModel.ReportParagraphName = "-";
                    }
                    else
                    {
                        stateAccountViewModel.FundTypeName = objCommonFunction.GetFundName(PMGSYSession.Current.FundType);
                        stateAccountViewModel.ReportFormNumber = ReportHeader.REPORT_FORM_NO;
                        stateAccountViewModel.ReportName = ReportHeader.REPORT_NAME;
                        stateAccountViewModel.ReportParagraphName = ReportHeader.REPORT_PARAGRAPH_NAME;
                    }

                    return stateAccountViewModel;
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    return null;
                }
                finally
                {

                    if (dbcontext != null)
                    {
                        dbcontext.Dispose();
                    }

                }
            }

            public Array ListStateAccountMonitoringDetails(int StateCode, int AgencyCode, string FundType, int page, int rows, string sidx, string sord, out long totalRecords)
            {
                dbcontext = new PMGSYEntities();
                string IsStateSelected;
                
                string lowerNdCode = string.Empty;
                string parentNdCode = "0";

                //((IObjectContextAdapter)dbcontext).ObjectContext.CommandTimeout = 360;
                //dbcontext.Configuration.LazyLoadingEnabled = false;

                try
                {
                    var lstStateAccMonitoringDetails = dbcontext.USP_ACC_RPT_STATUS_ACCOUNT_MONITORING(StateCode, AgencyCode, FundType).ToList();

                    totalRecords = lstStateAccMonitoringDetails.Count();

                    if (StateCode == 0)
                    {
                        IsStateSelected = "0";
                    }
                    else
                    {
                        IsStateSelected = "1";

                        if (totalRecords > 0)
                        {
                            foreach (var item in lstStateAccMonitoringDetails)
                            {
                                parentNdCode = dbcontext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == item.ADMIN_ND_CODE).Select(s => s.MAST_PARENT_ND_CODE).FirstOrDefault().ToString(); 
                                break;
                            }
                        }
                    }

                    if (sidx.Trim() != string.Empty)
                    {
                        if (sord.ToString() == "asc")
                        {
                            switch (sidx)
                            {

                                case "StateDpiuName":
                                    lstStateAccMonitoringDetails = lstStateAccMonitoringDetails.OrderBy(o => o.ADMIN_ND_NAME).ToList();
                                    break;
                                case "OBMonYear":
                                    lstStateAccMonitoringDetails = lstStateAccMonitoringDetails.OrderBy(o => o.OB).ToList();
                                    break;
                                case "ClosedMonYear":
                                    lstStateAccMonitoringDetails = lstStateAccMonitoringDetails.OrderBy(o => o.CL_MYEAR).ToList();
                                    break;                                
                                default:
                                    lstStateAccMonitoringDetails = lstStateAccMonitoringDetails.OrderBy(o => o.ADMIN_ND_NAME).ToList();
                                    break;
                            }
                        }
                        else
                        {
                            switch (sidx)
                            {

                                case "StateDpiuName":
                                    lstStateAccMonitoringDetails = lstStateAccMonitoringDetails.OrderByDescending(o => o.ADMIN_ND_NAME).ToList();
                                    break;
                                case "OBMonYear":
                                    lstStateAccMonitoringDetails = lstStateAccMonitoringDetails.OrderByDescending(o => o.OB).ToList();
                                    break;
                                case "ClosedMonYear":
                                    lstStateAccMonitoringDetails = lstStateAccMonitoringDetails.OrderByDescending(o => o.CL_MYEAR).ToList();
                                    break;
                                default:
                                    lstStateAccMonitoringDetails = lstStateAccMonitoringDetails.OrderByDescending(o => o.ADMIN_ND_NAME).ToList();
                                    break;
                            }

                        }
                    }
                    else {
                        lstStateAccMonitoringDetails = lstStateAccMonitoringDetails.OrderByDescending(o => o.ADMIN_ND_NAME).ToList();
                    }


                    return lstStateAccMonitoringDetails.Select(stateAccDetails => new
                    {
                                 cell=new []{
                                 stateAccDetails.ADMIN_ND_NAME,
                                 stateAccDetails.OB,
                                 stateAccDetails.CL_MYEAR,
                                (stateAccDetails.closing_month==0 && stateAccDetails.closing_year==0)?"-": "<a style='color:blue' href='#' onClick='MonthlyAccount(\""+stateAccDetails.closing_month+"$"+stateAccDetails.closing_year+"$"+IsStateSelected+"$"+stateAccDetails.ADMIN_ND_CODE.ToString()+"$"+parentNdCode+"\");return false;'>Details</a>",
                                (stateAccDetails.closing_month==0 && stateAccDetails.closing_year==0)?"-":"<a style='color:blue' href='#' onClick='BalanceSheetAllPiu(\""+stateAccDetails.closing_month+"$"+stateAccDetails.closing_year+"$"+IsStateSelected+"$"+stateAccDetails.ADMIN_ND_CODE.ToString()+"$"+parentNdCode+"\");return false;'>Details</a>",
                                (stateAccDetails.closing_month==0 && stateAccDetails.closing_year==0)?"-":"<a style='color:blue' href='#' onClick='BalanceSheetSRRDA(\""+stateAccDetails.closing_month+"$"+stateAccDetails.closing_year+"$"+IsStateSelected+"$"+stateAccDetails.ADMIN_ND_CODE.ToString()+"$"+parentNdCode+"\");return false;'>Details</a>",
                                (stateAccDetails.closing_month==0 && stateAccDetails.closing_year==0)?"-":"<a style='color:blue' href='#' onClick='BalanceSheetState(\""+stateAccDetails.closing_month+"$"+stateAccDetails.closing_year+"$"+IsStateSelected+"$"+stateAccDetails.ADMIN_ND_CODE.ToString()+"$"+parentNdCode+"\");return false;'>Details</a>",
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
                    if (dbcontext != null)
                    {
                        dbcontext.Dispose();
                    }
                }
            }

        #endregion State Account Monitoring
    }
    public interface IAccountReportsDAL
    {
        #region AnnualAccount
        List<SelectListItem> getAllYears();
        List<SelectListItem> getBalanceType();
        string GetNodalAgency(Int32 AdminNdCode);
        AnnualAccount GetAnnualAccountList(ReportFilter objParam);
        List<SelectListItem> PopulateSRRDA();
        string StateName(int adminNdCode);
        #endregion AnnualAccount

        #region BillDetails
        AccountBillViewModel getBillDetails(AccountBillViewModel accModel);
        Array lstTransactionDetails(string billId, int page, int rows, string sidx, string sord, out long totalRecords);
        #endregion BillDetails

        #region ContractorLedger

        ContractorLedgerModel getLedgerDetails(ContractorLedgerModel contractorLedger);
        int? GetDistrictCode(int PIUCode);
        Array lstContLedgerDetails(int PIUCode, int ContrCode, int AggCode, int page, int rows, string sidx, string sord, out long totalRecords);
        ContractorLedgerModel GerReportHeader(ContractorLedgerModel contractorLedger);
        #endregion ContractorLedger

        #region Chequebook Details DAL

            CheckBookDetailsViewFilterModel getCheckBookDetails(CheckBookDetailsViewFilterModel checkbookDetailsViewFilterModel);

        #endregion Chequebook Details DAL

        #region Authorised signatory details

            AuthorisedSignatoryViewModel getAuthSignatoryDetails(AuthorisedSignatoryViewModel authSignatoryViewModel);

        #endregion  

        #region Asset Register Details DAL

            AssetRegisterViewModel getAssetRegisterDetails(AssetRegisterViewModel assetRegisterViewModel);
            List<SelectListItem> getFundForStateCentral();

        #endregion Asset Register Details DAL

        #region FundTransfer
        List<SelectListItem> PopulateFund();
        FundTransferViewModel GetFundTransferDetails(ReportFilter ObjParam);
        #endregion FundTransfer

        #region AbstractFundTransfer
       // AbstractFundTransferredViewModel AbstractFundDetails(ReportFilter objParam);
        AbstractFundTransferredViewModel GetReportHeaderInfo(AbstractFundTransferredViewModel objAbstractFund);
        Array GetAbstractFundDetails(Int16 Year, Int16 Head, int AdminNdCode, int LowerAdminNdCode, int page, int rows, string sidx, string sord, out long totalRecords);
        #endregion AbstractFundTransfer

        #region BankAuthrization
        BankAuthrizationViewModel GetAuthrizationDetails(ReportFilter objParam);
        #endregion BankAuthrization


        #region Abstract Bank Auth Details
                 
            AbstractBankAuthViewModel AbstractBankAuthDetails(AbstractBankAuthViewModel abstractBankAuthDetails);
            Array ListAbstractBankAuthDetails(short Year, int StateCode, int DPIU, int page, int rows, string sidx, string sord, out long totalRecords);

        #endregion

        #region ScheduleRoad
            List<SelectListItem> lstFundingAgency();
            List<SelectListItem> PopulateHead(int[] headId);
            Array ListScheduleDetails(int? state, int? month, int? year, Int16? head, string fundingAgenCode, int? page, int? rows, string sord, string sidx, out long totalRecords);
            ScheduleModel GetScheduleList(ReportFilter objParam);
        #endregion ScheduleRoad

        #region MasterSheet
            List<USP_ACC_MASTER_SHEET_PIU_RPT_Result> MasterSheetDAL(int year);
            #endregion 

        #region Final Bill Payment

            List<SelectListItem> PopulateAgency(int stateCode);
            FinalBillPaymentModel FinalBillPaymentHeaderInformation(FinalBillPaymentModel finalBillPaymentModel);
            Array ListFinalBillPaymentCompletedDetails(int StateCode, int AgencyCode, int FundingAgencyCode, int page, int rows, string sidx, string sord, out long totalRecords);
            Array ListFinalBillPaymentPendingDetails(int StateCode, int AgencyCode, int FundingAgencyCode, int Year, int page, int rows, string sidx, string sord, out long totalRecords);
        #endregion Final Bill Payment

        #region State Account Monitoring
            
            List<SelectListItem> PopulateAgencyByStateCode(int stateCode);
            List<SelectListItem> PopulateFundType();
            StateAccountMonitoringViewModel StateAccMonitoringHeaderInformation(StateAccountMonitoringViewModel stateAccountViewModel);
            Array ListStateAccountMonitoringDetails(int StateCode, int AgencyCode, string FundType, int page, int rows, string sidx, string sord, out long totalRecords);

        #endregion State Account Monitoring
    }
}