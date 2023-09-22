/*-------------------------------------------------------------
 * File Name:AccountReportsBAL.cs
 * Project: OMMAS-II
 * Created By: Ashish Markande
 * Creation Date: 22/07/2013
 * Purpose : To call the methods which are presents in DAL.
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.Models.AccountsReports;
using PMGSY.DAL.AccountsReports;
using PMGSY.Models.Report;
using PMGSY.Models.AccountReports;
using System.Web.Mvc;
using PMGSY.Models;
using PMGSY.Models.Report.Account;
namespace PMGSY.BAL.AccountReports
{
    public partial class AccountReportsBAL : IAccountReportsBAL
    {
        IAccountReportsDAL objAccountDAL = new AccountReportsDAL();

        #region AnnualAccount
        /// <summary>
        /// GetNodalAgency(): Method is used to call the method which is present in DAL
        /// </summary>
        /// <param name="AdminNdCode"></param>
        /// <returns></returns>
        public string GetNodalAgency(Int32 AdminNdCode)
        {
            return objAccountDAL.GetNodalAgency(AdminNdCode);
        }

        /// <summary>
        /// GetMonthlyAccountList(): Method is used to call the method which is present in DAL.
        /// </summary>
        /// <param name="objParam"></param>
        /// <returns></returns>
        public AnnualAccount GetAnnualAccountList(ReportFilter objParam)
        {
            return objAccountDAL.GetAnnualAccountList(objParam);
        }
        #endregion AnnualAccount

        #region BillDetails
        /// <summary>
        /// getBillDetails(): Method is used to call the method which is present in DAL.
        /// </summary>
        /// <param name="accModel"></param>
        /// <returns></returns>
        public AccountBillViewModel getBillDetails(AccountBillViewModel accModel)
        {
            return objAccountDAL.getBillDetails(accModel);
        }

        /// <summary>
        /// lstTransactionDetails(): Method is used to call the method which is present in DAL.
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
            return objAccountDAL.lstTransactionDetails(billId, page, rows, sidx, sord,out totalRecords);
        }
        #endregion BillDetails

        #region ContractorLedger
        public ContractorLedgerModel getLedgerDetails(ContractorLedgerModel contractorLedger)
        {
            return objAccountDAL.getLedgerDetails(contractorLedger);
        }

        public Array lstContLedgerDetails(int PIUCode, int ContrCode, int AggCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return objAccountDAL.lstContLedgerDetails(PIUCode, ContrCode, AggCode, page, rows, sidx, sord, out totalRecords);
        }

        public ContractorLedgerModel GerReportHeader(ContractorLedgerModel contractorLedger)
        {
            return objAccountDAL.GerReportHeader(contractorLedger);
        }

        #endregion ContractorLedger


        #region Chequebook Details BAL

        /// <summary>
        /// getCheckBookDetails() action is used to call DAL getCheckBookDetails() method
        /// </summary>
        /// <param name="checkbookDetailsViewFilterModel">
        ///if Monthwise search: contains selected Month and year 
        ///if Chequebook wise search: contains selected cheque book series
        /// </param>
        /// <returns>
        /// Cheque book details based on search criteria.
        /// </returns>
        public CheckBookDetailsViewFilterModel getCheckBookDetails(CheckBookDetailsViewFilterModel checkbookDetailsViewFilterModel)
        {

            return objAccountDAL.getCheckBookDetails(checkbookDetailsViewFilterModel);
        }

        #endregion Chequebook Details        

        #region Authorised Signatory Details

        public AuthorisedSignatoryViewModel getAuthSignatoryDetails(AuthorisedSignatoryViewModel authSignatoryViewModel)
        {
            return objAccountDAL.getAuthSignatoryDetails(authSignatoryViewModel);
        }

        #endregion Authorised Signatory Details

        #region Asset Resiter Details BAL

        /// <summary>
        /// getAssetRegisterDetails() action is used to call DAL getAssetRegisterDetails() method
        /// </summary>
        /// <param name="assetRegisterViewModel">
        ///if Monthwise search: contains selected Month and year 
        ///if Periodic wise search: contains entered from and to date
        /// </param>
        /// <returns>
        /// Asset Details details based on search criteria.
        /// </returns>
        public AssetRegisterViewModel getAssetRegisterDetails(AssetRegisterViewModel assetRegisterViewModel)
        {

            return objAccountDAL.getAssetRegisterDetails(assetRegisterViewModel);
        }

        #endregion Asset Resiter Details

        #region FundTransfer

        public List<SelectListItem> PopulateFund()
        {
            return objAccountDAL.PopulateFund();
        }

        public FundTransferViewModel GetFundTransferDetails(ReportFilter ObjParam)
        {
            return objAccountDAL.GetFundTransferDetails(ObjParam);
        }

        #endregion FundTransfer

        #region AbstractFundTransfer
        //public AbstractFundTransferredViewModel AbstractFundDetails(ReportFilter objParam)
        //{
        //   return  objAccountDAL.AbstractFundDetails(objParam);
        //}
        public Array GetAbstractFundDetails(Int16 Year, Int16 Head, int AdminNdCode, int LowerAdminNdCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return objAccountDAL.GetAbstractFundDetails(Year, Head, AdminNdCode, LowerAdminNdCode, page, rows, sidx, sord,out totalRecords);
        }

        public AbstractFundTransferredViewModel GetReportHeaderInfo(AbstractFundTransferredViewModel objAbstractFund)
        {
            return objAccountDAL.GetReportHeaderInfo(objAbstractFund);
        }

        #endregion AbstractFundTransfer

        #region BankAuthrization
        public BankAuthrizationViewModel GetAuthrizationDetails(ReportFilter objParam)
        {
            return objAccountDAL.GetAuthrizationDetails(objParam);
        }
        #endregion BankAuthrization

        #region Abstract Bank Authorization Details

            public AbstractBankAuthViewModel AbstractBankAuthDetails(AbstractBankAuthViewModel abstractBankAuthDetails)
            {
                return objAccountDAL.AbstractBankAuthDetails(abstractBankAuthDetails);
            }

            public Array ListAbstractBankAuthDetails(short Year, int StateCode, int DPIU, int page, int rows, string sidx, string sord, out long totalRecords)
            {
                return objAccountDAL.ListAbstractBankAuthDetails(Year, StateCode, DPIU, page, rows, sidx, sord,out totalRecords);            
            }

        #endregion

        #region ScheduleRoad
            public List<SelectListItem> lstFundingAgency()
            {
                return objAccountDAL.lstFundingAgency();
            }

            public List<SelectListItem> PopulateHead(int[] headId)
            {
                return objAccountDAL.PopulateHead(headId);
            }
            public Array ListScheduleDetails(int? state, int? month, int? year, Int16? head, string fundingAgenCode, int? page, int? rows, string sord, string sidx, out long totalRecords)
            {
                return objAccountDAL.ListScheduleDetails(state, month, year, head, fundingAgenCode, page, rows, sord, sidx, out totalRecords);
            }

            public ScheduleModel GetScheduleList(ReportFilter objParam)
            {
                return objAccountDAL.GetScheduleList(objParam);
            }
        #endregion ScheduleRoad


        #region Master Sheet

        public List<USP_ACC_MASTER_SHEET_PIU_RPT_Result> MasterSheetBAL(int year)
        {
            return objAccountDAL.MasterSheetDAL(year);
        }

        #endregion

        #region Final Bill Payment
        public FinalBillPaymentModel FinalBillPaymentHeaderInformation(FinalBillPaymentModel finalBillPaymentModel)
        {
            return objAccountDAL.FinalBillPaymentHeaderInformation(finalBillPaymentModel);        
        }

        public Array ListFinalBillPaymentCompletedDetails(int StateCode, int AgencyCode, int FundingAgencyCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return objAccountDAL.ListFinalBillPaymentCompletedDetails(StateCode, AgencyCode, FundingAgencyCode, page, rows, sidx, sord, out totalRecords);
        }

        public Array ListFinalBillPaymentPendingDetails(int StateCode, int AgencyCode, int FundingAgencyCode, int Year, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return objAccountDAL.ListFinalBillPaymentPendingDetails(StateCode, AgencyCode, FundingAgencyCode, Year, page, rows, sidx, sord, out totalRecords);
        }

        #endregion Final Bill Payment

        #region State Account Monitoring

        public StateAccountMonitoringViewModel StateAccMonitoringHeaderInformation(StateAccountMonitoringViewModel stateAccountViewModel)
        {
            return objAccountDAL.StateAccMonitoringHeaderInformation(stateAccountViewModel);
        }

        public Array ListStateAccountMonitoringDetails(int StateCode, int AgencyCode, string FundType, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return objAccountDAL.ListStateAccountMonitoringDetails(StateCode, AgencyCode, FundType, page, rows, sidx, sord, out totalRecords);
        }

        #endregion State Account Monitoring
    }
    
    public interface IAccountReportsBAL
    {
        #region AnnualAccount
        string GetNodalAgency(Int32 AdminNdCode);
        AnnualAccount GetAnnualAccountList(ReportFilter objParam);
        #endregion AnnualAccount

        #region BillDetails
        AccountBillViewModel getBillDetails(AccountBillViewModel accModel);
        Array lstTransactionDetails(string billId, int page, int rows, string sidx, string sord, out long totalRecords);
        #endregion BillDetails

        #region ContractorLedger
        ContractorLedgerModel getLedgerDetails(ContractorLedgerModel contractorLedger);
        Array lstContLedgerDetails(int PIUCode, int ContrCode, int AggCode, int page, int rows, string sidx, string sord, out long totalRecords);
        ContractorLedgerModel GerReportHeader(ContractorLedgerModel contractorLedger);
        #endregion ContractorLedger  


        #region Chequebook Details BAL

            CheckBookDetailsViewFilterModel getCheckBookDetails(CheckBookDetailsViewFilterModel checkbookDetailsViewFilterModel);

        #endregion Chequebook Details BAL   

        #region Authorised Signatory Details

            AuthorisedSignatoryViewModel getAuthSignatoryDetails(AuthorisedSignatoryViewModel authSignatoryViewModel);

        #endregion Authorised Signatory Details

        #region Asset Resiter Details BAL
            AssetRegisterViewModel getAssetRegisterDetails(AssetRegisterViewModel assetRegisterViewModel);
        #endregion Asset Resiter Details

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

        #region AbstractBankAuthDetails                                                                              
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
            List<USP_ACC_MASTER_SHEET_PIU_RPT_Result> MasterSheetBAL(int year);
        #endregion 

        #region Final Bill Payment
            FinalBillPaymentModel FinalBillPaymentHeaderInformation(FinalBillPaymentModel finalBillPaymentModel);
            Array ListFinalBillPaymentCompletedDetails(int StateCode, int AgencyCode, int FundingAgencyCode, int page, int rows, string sidx, string sord, out long totalRecords);
            Array ListFinalBillPaymentPendingDetails(int StateCode, int AgencyCode, int FundingAgencyCode, int Year, int page, int rows, string sidx, string sord, out long totalRecords);
        #endregion Final Bill Payment

        #region State Account Monitoring

             StateAccountMonitoringViewModel StateAccMonitoringHeaderInformation(StateAccountMonitoringViewModel stateAccountViewModel);
             Array ListStateAccountMonitoringDetails(int StateCode, int AgencyCode, string FundType, int page, int rows, string sidx, string sord, out long totalRecords);

        #endregion State Account Monitoring

    }            
}


