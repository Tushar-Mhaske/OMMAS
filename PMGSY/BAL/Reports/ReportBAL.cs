using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.DAL.Reports;
using PMGSY.Models;
using PMGSY.Models.Report;
using System.Web.Mvc;
using PMGSY.Models.Report.Ledger;
//using PMGSY.Models.Report.MaintenanceFund;
using PMGSY.Models.Report.ProgramFund;
//using PMGSY.Models.Report.AdminFund;
using PMGSY.Models.Report.Account;
using PMGSY.Models.Report.RegisterOfWorks;
using PMGSY.Models.Common;
using System.Data.SqlClient;



namespace PMGSY.BAL.Reports
{
    public class ReportBAL : IReportBAL
    {
        IReportDAL objReportDAL = new ReportDAL();
        public CBHeader GetReportHeader(ReportFilter objParam)
        {
            return objReportDAL.GetReportHeader(objParam);
        }

        public CBSingleModel GetSingleCB(ReportFilter objParam)
        {
            return objReportDAL.GetSingleCB(objParam);
        }

        public CBReceiptModel ReceiptCashBook(ReportFilter objParam)
        {
            return objReportDAL.ReceiptCashBook(objParam);
        }

        public CBPaymentModel PaymentCashBook(ReportFilter objParam)
        {
            return objReportDAL.PaymentCashBook(objParam);
        }
        #region ledger

        public List<SelectListItem> GetLedgerHeadList(String creditDebit, String fundType, List<short> op_Level,String SRRDA_DPIU)
        {
            return objReportDAL.GetLedgerHeadList(creditDebit, fundType, op_Level,SRRDA_DPIU);
        }

        public List<LedgerAmountModel> GetCreditDebitModel(ReportFilter objParam)
        {
            return objReportDAL.GetCreditDebitModel(objParam);
        }


        public LedgerModel GetForContextData(ReportFilter objParam)
        {
            return objReportDAL.GetForContextData(objParam);
        }
        #endregion

        #region MonthlyAccount
        public string GetNodalAgency(Int32 AdminNdCode)
        {
            return objReportDAL.GetNodalAgency(AdminNdCode);
        }

        public List<USP_RPT_SHOW_MONTHLY_ACCOUNT_SELF_Result> GetMonthlyAccountList(ReportFilter objParam)
        {
            return objReportDAL.GetMonthlyAccountList(objParam);
        }

        //added by abhishek 10-9-2013
        public List<USP_RPT_SHOW_MONTHLY_ACCOUNT_ALLPIU_Result> GetMonthlyAccountForAllPIU(ReportFilter objParam)
        {
            return objReportDAL.GetMonthlyAccountForAllPIU(objParam);
        }

        #endregion
        #region Transfer Entry Order
        public RptTrnasferEntryOrderList GetTransferEntryOrderListBAL(RptTransferEntryOrder teo, ReportFilter objParam)
        {
            RptTrnasferEntryOrderList rptTeoList = new RptTrnasferEntryOrderList();
            rptTeoList = objReportDAL.GetTransferEntryOrderListDAL(teo, objParam);
            rptTeoList.CreditAmt = rptTeoList.ListTeo.Sum(mteo => (double)mteo.Credit_Amount);
            rptTeoList.DebitAmt = rptTeoList.ListTeo.Sum(mteo => (double)mteo.Debit_Amount);



            return rptTeoList;
        }

        #endregion Transfer Entry Order
        #region BalanceSheet
        //public MaintenanceFundBalanceSheetList GetMaintenanceFundBalanceSheetBAL(MaintenanceFundBalanceSheet maintenanceFundBalanceSheet, ReportFilter objParam)
        //{
        //    return objReportDAL.GetMaintenanceFundBalanceSheetDAL(maintenanceFundBalanceSheet, objParam);
        //}


        //public ProgramFundBalanceSheetList GetProgramFundBalanceSheetBAL(ProgramFundBalanceSheet programFundBalanceSheet, ReportFilter objParam)
        //{
        //    return objReportDAL.GetProgramFundBalanceSheetDAL(programFundBalanceSheet, objParam);
        //}
        //public AdminFundBalanceSheetList GetAdminFundBalanceSheetBAL(AdminFundBalanceSheet adminFundBalanceSheet, ReportFilter objParam)
        //{
        //    return objReportDAL.GetAdminFundBalanceSheetDAL(adminFundBalanceSheet, objParam);
        //}

        //public BalanceSheetList GetBalanceSheetBAL(BalanceSheet balanceSheet, ReportFilter objParam)
        //{


        //    if (objParam.LevelId == 4 && objParam.FundType == "P" && balanceSheet.ReportLevel == 'O')
        //    {
        //        balanceSheet.ReportNumber = 2;
        //        balanceSheet.ReportDPIU = 0;
        //    }
        //    else if (objParam.LevelId == 4 && objParam.FundType == "P" && balanceSheet.ReportLevel == 'S')
        //    {
        //        balanceSheet.ReportNumber = 9;
        //        balanceSheet.ReportDPIU = 0;
        //    }
        //    else if (objParam.LevelId == 4 && objParam.FundType == "P" && balanceSheet.ReportLevel == 'A')
        //    {
        //        balanceSheet.ReportNumber = 8;
        //        balanceSheet.ReportDPIU = 1;
        //    }
        //    else if (objParam.LevelId == 4 && objParam.FundType == "M" && balanceSheet.ReportLevel == 'O')
        //    {
        //        balanceSheet.ReportNumber = 4;
        //        balanceSheet.ReportDPIU = 0;
        //    }
        //    else if (objParam.LevelId == 4 && objParam.FundType == "M" && balanceSheet.ReportLevel == 'S')
        //    {
        //        balanceSheet.ReportNumber = 1;
        //        balanceSheet.ReportDPIU = 0;
        //    }
        //    else if (objParam.LevelId == 4 && objParam.FundType == "M" && balanceSheet.ReportLevel == 'A')
        //    {
        //        balanceSheet.ReportNumber = 3;
        //        balanceSheet.ReportDPIU = 1;
        //    }
        //    else if (objParam.LevelId == 4 && objParam.FundType == "A" && balanceSheet.ReportLevel == 'O')
        //    {
        //        balanceSheet.ReportNumber = 5;
        //        balanceSheet.ReportDPIU = 0;
        //    }
        //    else if (objParam.LevelId == 4 && objParam.FundType == "A" && balanceSheet.ReportLevel == 'S')
        //    {
        //        balanceSheet.ReportNumber = 7;
        //        balanceSheet.ReportDPIU = 0;
        //    }
        //    else if (objParam.LevelId == 4 && objParam.FundType == "A" && balanceSheet.ReportLevel == 'A')
        //    {
        //        balanceSheet.ReportNumber = 6;
        //        balanceSheet.ReportDPIU = 1;
        //    }
        //    else if (objParam.LevelId == 5 && objParam.FundType == "P" && balanceSheet.ReportLevel == 'A')
        //    {
        //        balanceSheet.ReportNumber = 8;
        //        balanceSheet.ReportDPIU = 0;
        //    }
        //    else if (objParam.LevelId == 5 && objParam.FundType == "M" && balanceSheet.ReportLevel == 'A')
        //    {
        //        balanceSheet.ReportNumber = 3;
        //        balanceSheet.ReportDPIU = 0;
        //    }
        //    else if (objParam.LevelId == 5 && objParam.FundType == "A" && balanceSheet.ReportLevel == 'A')
        //    {
        //        balanceSheet.ReportNumber = 6;
        //        balanceSheet.ReportDPIU = 0;
        //    }



        //    BalanceSheetList balanceSheetList = objReportDAL.GetBalanceSheetDAL(balanceSheet, objParam);
        //    //BalanceSheetList balanceSheetList = new BalanceSheetList();








        //    //return objReportDAL.GetBalanceSheetDAL(balanceSheet, objParam);
        //    return balanceSheetList;
        //}



        public BalanceSheetList GetBalanceSheetBAL(BalanceSheet balanceSheet, ReportFilter objParam)
        {


            if (objParam.LevelId == 4 && objParam.FundType == "P" && balanceSheet.ReportLevel == 'O')
            {
                balanceSheet.ReportNumber = 2;
                balanceSheet.ReportDPIU = 0;
            }
            else if (objParam.LevelId == 4 && objParam.FundType == "P" && balanceSheet.ReportLevel == 'S')
            {
                balanceSheet.ReportNumber = 9;
                balanceSheet.ReportDPIU = 0;
            }
            //new change done by Vikram
            else if (objParam.LevelId == 4 && objParam.FundType == "P" && balanceSheet.ReportLevel == 'A' && balanceSheet.AdminCode == 0)
            {
                //objParam.AdminNdCode = PMGSYSession.Current.AdminNdCode;
                balanceSheet.ReportNumber = 8;
                balanceSheet.ReportDPIU = 1;
            }
            //new change done by Vikram
            else if (objParam.LevelId == 4 && objParam.FundType == "P" && balanceSheet.ReportLevel == 'A' && balanceSheet.AdminCode != 0)
            {
                //objParam.AdminNdCode = balanceSheet.AdminCode;
                balanceSheet.ReportNumber = 8;
                balanceSheet.ReportDPIU = 0;
            }



            else if (objParam.LevelId == 4 && objParam.FundType == "M" && balanceSheet.ReportLevel == 'O')
            {
                balanceSheet.ReportNumber = 4;
                balanceSheet.ReportDPIU = 0;
            }
            else if (objParam.LevelId == 4 && objParam.FundType == "M" && balanceSheet.ReportLevel == 'S')
            {
                balanceSheet.ReportNumber = 1;
                balanceSheet.ReportDPIU = 0;
            }
            //new change done by Vikram
            else if (objParam.LevelId == 4 && objParam.FundType == "M" && balanceSheet.ReportLevel == 'A' && balanceSheet.AdminCode == 0)
            {
                //objParam.AdminNdCode = PMGSYSession.Current.AdminNdCode;
                balanceSheet.ReportNumber = 3;
                balanceSheet.ReportDPIU = 1;
            }
            //new change done by Vikram
            else if (objParam.LevelId == 4 && objParam.FundType == "M" && balanceSheet.ReportLevel == 'A' && balanceSheet.AdminCode != 0) // dropdown condition 
            {
                //objParam.AdminNdCode = balanceSheet.AdminCode;
                balanceSheet.ReportNumber = 3;
                balanceSheet.ReportDPIU = 0;
            }



            else if (objParam.LevelId == 4 && objParam.FundType == "A" && balanceSheet.ReportLevel == 'O')
            {
                balanceSheet.ReportNumber = 5;
                balanceSheet.ReportDPIU = 0;
            }
            else if (objParam.LevelId == 4 && objParam.FundType == "A" && balanceSheet.ReportLevel == 'S')
            {
                balanceSheet.ReportNumber = 7;
                balanceSheet.ReportDPIU = 0;
            }
            //new change done by Vikram
            else if (objParam.LevelId == 4 && objParam.FundType == "A" && balanceSheet.ReportLevel == 'A' && balanceSheet.AdminCode == 0)
            {
                //objParam.AdminNdCode = PMGSYSession.Current.AdminNdCode;
                balanceSheet.ReportNumber = 6;
                balanceSheet.ReportDPIU = 1;
            }
            //new change done by Vikram
            else if (objParam.LevelId == 4 && objParam.FundType == "A" && balanceSheet.ReportLevel == 'A' && balanceSheet.AdminCode != 0)// dropdown condition
            {
                //objParam.AdminNdCode = balanceSheet.AdminCode;
                balanceSheet.ReportNumber = 6;
                balanceSheet.ReportDPIU = 0;
            }



            else if (objParam.LevelId == 5 && objParam.FundType == "P" && balanceSheet.ReportLevel == 'O')
            {
                balanceSheet.ReportNumber = 8;
                balanceSheet.ReportDPIU = 0;
            }
            else if (objParam.LevelId == 5 && objParam.FundType == "M" && balanceSheet.ReportLevel == 'O')
            {
                balanceSheet.ReportNumber = 3;
                balanceSheet.ReportDPIU = 0;
            }
            else if (objParam.LevelId == 5 && objParam.FundType == "A" && balanceSheet.ReportLevel == 'O')
            {
                balanceSheet.ReportNumber = 6;
                balanceSheet.ReportDPIU = 0;
            }

            BalanceSheetList balanceSheetList = objReportDAL.GetBalanceSheetDAL(balanceSheet, objParam);
            //BalanceSheetList balanceSheetList = new BalanceSheetList();

            //return objReportDAL.GetBalanceSheetDAL(balanceSheet, objParam);
            return balanceSheetList;
        }

        public string GetDepartmentName(int adminCode)
        {
            return objReportDAL.GetDepartmentName(adminCode);
        }

        #endregion BalanceSheet

        #region RegisterOFWorks

        public RegisterOfWorksModel RegisterOfWorksBAL(TransactionParams objparams)
        {
            return objReportDAL.RegisterOfWorksDAL(objparams);
        }

        public Array RegisterOfWorksHeaderGridBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int adminNDCode, int contractorID, int agreementCode)
        {
            return objReportDAL.RegisterOfWorksHeaderGridDAL(page, rows, sidx, sord, out totalRecords, adminNDCode, contractorID, agreementCode);
        }
        #endregion

        #region
        public class ReportSPBAL<T>
        {
            PMGSY.DAL.Reports.ReportDAL.ReportSPDAL<T> objdal;
            public List<T> GetReportBAL(int id, object[] arrParams)
            {


                string procedureName = string.Empty;
                SqlParameter[] parameters = new SqlParameter[arrParams.Length];

                switch (id)
                {
                    case 1://SHDE
                        procedureName = "[omms].[USP_ACC_RPT_SHOW_RUNNING_ACCOUNT_SELF] @P_INT_AdminNDCode, @P_INT_AccMonth, @P_INT_AccYear, @P_CHAR_fundType,    @P_CHAR_CreditDebit";
                        parameters[0] = new SqlParameter("P_INT_AdminNDCode", arrParams[0]);
                        parameters[1] = new SqlParameter("P_INT_AccMonth", arrParams[1]);
                        parameters[2] = new SqlParameter("P_INT_AccYear", arrParams[2]);
                        parameters[3] = new SqlParameter("P_CHAR_fundType", arrParams[3]);
                        parameters[4] = new SqlParameter("P_CHAR_CreditDebit", arrParams[4]);

                        objdal = new DAL.Reports.ReportDAL.ReportSPDAL<T>();
                        return objdal.ExecuteProcedure(procedureName, parameters);

                    case 2:
                        procedureName = "[omms].[USP_RPT_SHOW_SCHEDULE] @PINTRptID, @PINTAdminNDCode, @PINTAccMonth, @PINTAccYear,    @PINTisAllPIU ";
                        parameters[0] = new SqlParameter("PINTRptID", arrParams[0]);
                        parameters[1] = new SqlParameter("PINTAdminNDCode", arrParams[1]);
                        parameters[2] = new SqlParameter("PINTAccMonth", arrParams[2]);
                        parameters[3] = new SqlParameter("PINTAccYear", arrParams[3]);
                        parameters[4] = new SqlParameter("PINTisAllPIU", arrParams[4]);

                        objdal = new DAL.Reports.ReportDAL.ReportSPDAL<T>();

                        return objdal.ExecuteProcedure(procedureName, parameters);
                    case 3:
                        procedureName = "[omms].[USP_RPT_SHOW_SCHEDULE] @PINTRptID, @PINTAdminNDCode, @PINTAccMonth, @PINTAccYear,    @PINTisAllPIU ";
                        parameters[0] = new SqlParameter("PINTRptID", arrParams[0]);
                        parameters[1] = new SqlParameter("PINTAdminNDCode", arrParams[1]);
                        parameters[2] = new SqlParameter("PINTAccMonth", arrParams[2]);
                        parameters[3] = new SqlParameter("PINTAccYear", arrParams[3]);
                        parameters[4] = new SqlParameter("PINTisAllPIU", arrParams[4]);

                        objdal = new DAL.Reports.ReportDAL.ReportSPDAL<T>();

                        return objdal.ExecuteProcedure(procedureName, parameters);

                    case 4:
                        procedureName = "[omms].[USP_RPT_SHOW_SCHEDULE] @PINTRptID, @PINTAdminNDCode, @PINTAccMonth, @PINTAccYear,    @PINTisAllPIU ";
                        parameters[0] = new SqlParameter("PINTRptID", arrParams[0]);
                        parameters[1] = new SqlParameter("PINTAdminNDCode", arrParams[1]);
                        parameters[2] = new SqlParameter("PINTAccMonth", arrParams[2]);
                        parameters[3] = new SqlParameter("PINTAccYear", arrParams[3]);
                        parameters[4] = new SqlParameter("PINTisAllPIU", arrParams[4]);

                        objdal = new DAL.Reports.ReportDAL.ReportSPDAL<T>();

                        return objdal.ExecuteProcedure(procedureName, parameters);
                    case 5:
                       procedureName = "[omms].[USP_RPT_SHOW_SCHEDULE] @PINTRptID, @PINTAdminNDCode, @PINTAccMonth, @PINTAccYear,    @PINTisAllPIU ";
                        parameters[0] = new SqlParameter("PINTRptID", arrParams[0]);
                        parameters[1] = new SqlParameter("PINTAdminNDCode", arrParams[1]);
                        parameters[2] = new SqlParameter("PINTAccMonth", arrParams[2]);
                        parameters[3] = new SqlParameter("PINTAccYear", arrParams[3]);
                        parameters[4] = new SqlParameter("PINTisAllPIU", arrParams[4]);

                        objdal = new DAL.Reports.ReportDAL.ReportSPDAL<T>();

                        return objdal.ExecuteProcedure(procedureName, parameters);
                    case 6:
                        procedureName = "[omms].[USP_RPT_SHOW_SCHEDULE] @PINTRptID, @PINTAdminNDCode, @PINTAccMonth, @PINTAccYear,    @PINTisAllPIU ";
                        parameters[0] = new SqlParameter("PINTRptID", arrParams[0]);
                        parameters[1] = new SqlParameter("PINTAdminNDCode", arrParams[1]);
                        parameters[2] = new SqlParameter("PINTAccMonth", arrParams[2]);
                        parameters[3] = new SqlParameter("PINTAccYear", arrParams[3]);
                        parameters[4] = new SqlParameter("PINTisAllPIU", arrParams[4]);

                        objdal = new DAL.Reports.ReportDAL.ReportSPDAL<T>();
                        return objdal.ExecuteProcedure(procedureName, parameters);
                    case 7:
                        procedureName = "[omms].[USP_RPT_SHOW_SCHEDULE] @PINTRptID, @PINTAdminNDCode, @PINTAccMonth, @PINTAccYear,    @PINTisAllPIU ";
                        parameters[0] = new SqlParameter("PINTRptID", arrParams[0]);
                        parameters[1] = new SqlParameter("PINTAdminNDCode", arrParams[1]);
                        parameters[2] = new SqlParameter("PINTAccMonth", arrParams[2]);
                        parameters[3] = new SqlParameter("PINTAccYear", arrParams[3]);
                        parameters[4] = new SqlParameter("PINTisAllPIU", arrParams[4]);

                        objdal = new DAL.Reports.ReportDAL.ReportSPDAL<T>();

                        return objdal.ExecuteProcedure(procedureName, parameters);
                    case 8:
                       procedureName = "[omms].[USP_RPT_SHOW_SCHEDULE] @PINTRptID, @PINTAdminNDCode, @PINTAccMonth, @PINTAccYear,    @PINTisAllPIU ";
                        parameters[0] = new SqlParameter("PINTRptID", arrParams[0]);
                        parameters[1] = new SqlParameter("PINTAdminNDCode", arrParams[1]);
                        parameters[2] = new SqlParameter("PINTAccMonth", arrParams[2]);
                        parameters[3] = new SqlParameter("PINTAccYear", arrParams[3]);
                        parameters[4] = new SqlParameter("PINTisAllPIU", arrParams[4]);

                        objdal = new DAL.Reports.ReportDAL.ReportSPDAL<T>();

                        return objdal.ExecuteProcedure(procedureName, parameters);
                    case 9:
                       procedureName = "[omms].[USP_RPT_SHOW_SCHEDULE] @PINTRptID, @PINTAdminNDCode, @PINTAccMonth, @PINTAccYear,    @PINTisAllPIU ";
                        parameters[0] = new SqlParameter("PINTRptID", arrParams[0]);
                        parameters[1] = new SqlParameter("PINTAdminNDCode", arrParams[1]);
                        parameters[2] = new SqlParameter("PINTAccMonth", arrParams[2]);
                        parameters[3] = new SqlParameter("PINTAccYear", arrParams[3]);
                        parameters[4] = new SqlParameter("PINTisAllPIU", arrParams[4]);

                        objdal = new DAL.Reports.ReportDAL.ReportSPDAL<T>();

                        return objdal.ExecuteProcedure(procedureName, parameters);
                    case 10:
                        procedureName = "[omms].[USP_RPT_SHOW_SCHEDULE] @PINTRptID, @PINTAdminNDCode, @PINTAccMonth, @PINTAccYear,    @PINTisAllPIU ";
                        parameters[0] = new SqlParameter("PINTRptID", arrParams[0]);
                        parameters[1] = new SqlParameter("PINTAdminNDCode", arrParams[1]);
                        parameters[2] = new SqlParameter("PINTAccMonth", arrParams[2]);
                        parameters[3] = new SqlParameter("PINTAccYear", arrParams[3]);
                        parameters[4] = new SqlParameter("PINTisAllPIU", arrParams[4]);

                        objdal = new DAL.Reports.ReportDAL.ReportSPDAL<T>();

                        return objdal.ExecuteProcedure(procedureName, parameters);
                    case 11:
                        procedureName = "[omms].[USP_RPT_SHOW_SCHEDULE] @PINTRptID, @PINTAdminNDCode, @PINTAccMonth, @PINTAccYear,    @PINTisAllPIU ";
                        parameters[0] = new SqlParameter("PINTRptID", arrParams[0]);
                        parameters[1] = new SqlParameter("PINTAdminNDCode", arrParams[1]);
                        parameters[2] = new SqlParameter("PINTAccMonth", arrParams[2]);
                        parameters[3] = new SqlParameter("PINTAccYear", arrParams[3]);
                        parameters[4] = new SqlParameter("PINTisAllPIU", arrParams[4]);

                        objdal = new DAL.Reports.ReportDAL.ReportSPDAL<T>();

                        return objdal.ExecuteProcedure(procedureName, parameters);
                    case 12:
                        procedureName = "[omms].[USP_RPT_SHOW_SCHEDULE] @PINTRptID, @PINTAdminNDCode, @PINTAccMonth, @PINTAccYear,    @PINTisAllPIU ";
                        parameters[0] = new SqlParameter("PINTRptID", arrParams[0]);
                        parameters[1] = new SqlParameter("PINTAdminNDCode", arrParams[1]);
                        parameters[2] = new SqlParameter("PINTAccMonth", arrParams[2]);
                        parameters[3] = new SqlParameter("PINTAccYear", arrParams[3]);
                        parameters[4] = new SqlParameter("PINTisAllPIU", arrParams[4]);

                        objdal = new DAL.Reports.ReportDAL.ReportSPDAL<T>();

                        return objdal.ExecuteProcedure(procedureName, parameters);
                    case 13:
                        procedureName = "[omms].[USP_RPT_SHOW_SCHEDULE] @PINTRptID, @PINTAdminNDCode, @PINTAccMonth, @PINTAccYear,    @PINTisAllPIU ";
                        parameters[0] = new SqlParameter("PINTRptID", arrParams[0]);
                        parameters[1] = new SqlParameter("PINTAdminNDCode", arrParams[1]);
                        parameters[2] = new SqlParameter("PINTAccMonth", arrParams[2]);
                        parameters[3] = new SqlParameter("PINTAccYear", arrParams[3]);
                        parameters[4] = new SqlParameter("PINTisAllPIU", arrParams[4]);

                        break;
                    case 14:
                        procedureName = "[omms].[USP_ACC_RPT_SCHEDULE_BA_UTILIZATION_REC_STMT] @Prm_Fund_Type,@Prm_Month,@Prm_Year,@Prm_Admin_ND_Code,@Prm_DPIU_ALLPIU";
                        parameters[0] = new SqlParameter("Prm_Fund_Type", arrParams[0]);
                        parameters[1] = new SqlParameter("Prm_Month", arrParams[1]);
                        parameters[2] = new SqlParameter("Prm_Year", arrParams[2]);
                        parameters[3] = new SqlParameter("Prm_Admin_ND_Code", arrParams[3]);
                        parameters[4] = new SqlParameter("Prm_DPIU_ALLPIU", arrParams[4]);
                        break;

                    case 15:
                        procedureName = "[omms].[USP_ACC_RPT_SCHEDULE_BANK_REMITTANCES_REC_STMT] @Prm_Fund_Type,@Prm_Month,@Prm_Year,@Prm_Admin_ND_Code,@Prm_DPIU_ALLPIU";
                        parameters[0] = new SqlParameter("Prm_Fund_Type", arrParams[0]);
                        parameters[1] = new SqlParameter("Prm_Month", arrParams[1]);
                        parameters[2] = new SqlParameter("Prm_Year", arrParams[2]);
                        parameters[3] = new SqlParameter("Prm_Admin_ND_Code", arrParams[3]);
                        parameters[4] = new SqlParameter("Prm_DPIU_ALLPIU", arrParams[4]);
                        break;
                    case 16://added by Abhishek Kamble
                        procedureName = "[omms].[USP_ACC_RPT_SCHEDULE_CURRENT_ASSETS] @PrmFundType,@PrmMonth,@PrmYear,@PrmSRRDANDCode,@PrmDPIUNDCODE,@PrmFlag";
                        parameters[0] = new SqlParameter("PrmFundType", arrParams[0]);
                        parameters[1] = new SqlParameter("PrmMonth", arrParams[1]);
                        parameters[2] = new SqlParameter("PrmYear", arrParams[2]);
                        parameters[3] = new SqlParameter("PrmSRRDANDCode", arrParams[3]);
                        parameters[4] = new SqlParameter("PrmDPIUNDCODE", arrParams[4]);
                        parameters[5] = new SqlParameter("PrmFlag", arrParams[5]);
                        break;
                    case 17://added by Abhishek Kamble
                        procedureName = "[omms].[USP_ACC_RPT_SCHEDULE_Durable_ASSETS] @PrmFundType,@PrmMonth,@PrmYear,@PrmSRRDANDCode,@PrmDPIUNDCODE,@PrmFlag";
                        parameters[0] = new SqlParameter("PrmFundType", arrParams[0]);
                        parameters[1] = new SqlParameter("PrmMonth", arrParams[1]);
                        parameters[2] = new SqlParameter("PrmYear", arrParams[2]);
                        parameters[3] = new SqlParameter("PrmSRRDANDCode", arrParams[3]);
                        parameters[4] = new SqlParameter("PrmDPIUNDCODE", arrParams[4]);
                        parameters[5] = new SqlParameter("PrmFlag", arrParams[5]);
                        break; 
                        
                    case 18:
                        procedureName = "[omms].[USP_ACC_RPT_SCHEDULE_FUND_REC_PIU_SRRDA] @Prm_Fund_Type,@Prm_Month,@Prm_Year,@Prm_SRRDA_ND_Code";
                        parameters[0] = new SqlParameter("Prm_Fund_Type", arrParams[0]);
                        parameters[1] = new SqlParameter("Prm_Month", arrParams[1]);
                        parameters[2] = new SqlParameter("Prm_Year", arrParams[2]);
                        parameters[3] = new SqlParameter("Prm_SRRDA_ND_Code", arrParams[3]);                       
                        break; 

                    case 19:
                        procedureName = "[omms].[USP_RPT_SHOW_BALSHEET] @PINTRptID ,@PINTAdminNDCode,@PINTAccMonth,@PINTAccYear,@PINTisAllPIU";
                        parameters[0] = new SqlParameter("PINTRptID", arrParams[0]);
                        parameters[1] = new SqlParameter("PINTAdminNDCode", arrParams[1]);
                        parameters[2] = new SqlParameter("PINTAccMonth", arrParams[2]);
                        parameters[3] = new SqlParameter("PINTAccYear", arrParams[3]);
                        parameters[4] = new SqlParameter("PINTisAllPIU", arrParams[4]);
                        break;

                    case 20:
                        procedureName = "[omms].[USP_ACC_RPT_SHOW_FUND_RECEIVED] @PINTRptID ,@PINTAdminNDCode,@PINTAccMonth,@PINTAccYear,@PINTisAllPIU";
                        parameters[0] = new SqlParameter("PINTRptID", arrParams[0]);
                        parameters[1] = new SqlParameter("PINTAdminNDCode", arrParams[1]);
                        parameters[2] = new SqlParameter("PINTAccMonth", arrParams[2]);
                        parameters[3] = new SqlParameter("PINTAccYear", arrParams[3]);
                        parameters[4] = new SqlParameter("PINTisAllPIU", arrParams[4]);
                        break; 
                     default:
                        return new List<T>();

                }
                objdal = new DAL.Reports.ReportDAL.ReportSPDAL<T>();

                return objdal.ExecuteProcedure(procedureName, parameters);
               //

            }

        }
        #endregion


        #region RUNNING_ACCOUNT

        public Array GetRunningAccountListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, RunningAccountViewModel model, String ReportType)
        {
            return objReportDAL.GetRunningAccountListDAL(page, rows, sidx, sord, out totalRecords, model,ReportType);
        }

        #endregion

        #region REGISTER_HEADS

        public Array GetRegisterListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, RegisterViewModel model)
        {
            return objReportDAL.GetRegisterListDAL(page, rows, sidx, sord, out totalRecords, model);
        }

        public Array ListImprestSettlementsBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, ImprestSettlementViewModel model)
        {
            return objReportDAL.ListImprestSettlementsDAL(page, rows, sidx, sord, out totalRecords, model);
        }

        #endregion

        #region Income and Expenditure

        public Array ListIncomeAndExpenditureDetails(int? Month, int? Year, int? ndCode, int? rlevel, int? allPiu, int? duration, out long totalRecords)
        {
            return objReportDAL.ListIncomeAndExpenditureDetails(Month, Year, ndCode, rlevel, allPiu, duration, out totalRecords);      
        }

        #endregion Income and Expenditure

    }

    public interface IReportBAL
    {
        CBHeader GetReportHeader(ReportFilter objParam);
        CBSingleModel GetSingleCB(ReportFilter objParam);
        CBReceiptModel ReceiptCashBook(ReportFilter objParam);
        CBPaymentModel PaymentCashBook(ReportFilter objParam);

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
        #endregion

        #region BalanceSheet
        //MaintenanceFundBalanceSheetList GetMaintenanceFundBalanceSheetBAL(MaintenanceFundBalanceSheet maintenanceFundBalanceSheet, ReportFilter objParam);
        //ProgramFundBalanceSheetList GetProgramFundBalanceSheetBAL(ProgramFundBalanceSheet programFundBalanceSheet, ReportFilter objParam);
        //AdminFundBalanceSheetList GetAdminFundBalanceSheetBAL(AdminFundBalanceSheet adminFundBalanceSheet, ReportFilter objParam);
        BalanceSheetList GetBalanceSheetBAL(BalanceSheet balanceSheet, ReportFilter objParam);
        RptTrnasferEntryOrderList GetTransferEntryOrderListBAL(RptTransferEntryOrder teo, ReportFilter objParam);


        string GetDepartmentName(int adminCode);

        #endregion BalanceSheet

        #region RegisterOfWorks

        RegisterOfWorksModel RegisterOfWorksBAL(TransactionParams objparams);
        Array RegisterOfWorksHeaderGridBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int adminNDCode, int contractorID, int agreementCode);
        #endregion


        #region RUNNING_ACCOUNT

        Array GetRunningAccountListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, RunningAccountViewModel model, String ReportType);

        #endregion

        #region REGISTER_HEADS

        Array GetRegisterListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, RegisterViewModel model);

        Array ListImprestSettlementsBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, ImprestSettlementViewModel model);

        #endregion

        #region Income and Expenditure

        Array ListIncomeAndExpenditureDetails(int? Month, int? Year, int? ndCode, int? rlevel, int? allPiu, int? duration, out long totalRecords);

        #endregion Income and Expenditure

    }
}