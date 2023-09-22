using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.DAL.Accounts;
using PMGSY.Models.Accounts;
using PMGSY.Models;
using System.Web.Mvc;

namespace PMGSY.BAL.Accounts
{
    public class AccountsBAL : IAccountsBAL
    {
        IAccountsDAL objAccountsDAL = null;        
        public Array GetPFAuthorizationList(AccountsFilterModel objFilter, out long totalRecords)
        {
            totalRecords = 0;
            objAccountsDAL = new AccountsDAL();
            return objAccountsDAL.GetPFAuthorizationList(objFilter, out totalRecords);
        }   


         public Array GetAssetliabilityList(AccountsFilterModel objFilter, out long totalRecords)
        {
            totalRecords = 0;
            objAccountsDAL = new AccountsDAL();
            return objAccountsDAL.GetAssetliabilityList(objFilter, out totalRecords);
        }


         public Array GetAuthorizationReceivedList(AccountsFilterModel objFilter, out long totalRecords)
         {
             totalRecords = 0;
             objAccountsDAL = new AccountsDAL();
             return objAccountsDAL.GetAuthorizationReceivedList(objFilter, out totalRecords);
         }

        public Array GetLettestTransactionsList(AccountsFilterModel objFilter, out long totalRecords)
         {
             totalRecords = 0;
             objAccountsDAL = new AccountsDAL();
             return objAccountsDAL.GetLettestTransactionsList(objFilter, out totalRecords);
         }


         public Array GetSummaryList(AccountsFilterModel objFilter, out long totalRecords)
         {
             totalRecords = 0;
             objAccountsDAL = new AccountsDAL();
             return objAccountsDAL.GetSummaryList(objFilter, out totalRecords);
         }


         public List<SP_ACC_DB_ASSET_LIABILITIES_MAJOR_DETAILS_Result> GetAssetliabilityChart(AccountsFilterModel objFilter)
         {
             objAccountsDAL = new AccountsDAL();
             return objAccountsDAL.GetAssetliabilityChart(objFilter);
         }

         public List<SP_ACC_DB_ASSET_LIABILITIES_MINOR_DETAILS_Result> GetAssetliabilityDetailsChart(AccountsFilterModel objFilter)
         {
             objAccountsDAL = new AccountsDAL();
             return objAccountsDAL.GetAssetliabilityDetailsChart(objFilter);
         }


        //added by Koustubh Nakate on 20/08/2013 to get latest DPIU transaction details 
         public Array GetLatestDPIUTransactionsDetailsListBAL(int? page, int? rows, string sidx, string sord, out long totalRecords)
         {
             objAccountsDAL = new AccountsDAL();
             return objAccountsDAL.GetLatestDPIUTransactionsDetailsListDAL(page, rows, sidx, sord, out totalRecords);
         }

         //added by Koustubh Nakate on 21/08/2013 to get latest Notification 
         public List<string> GetNotificationsBAL()
         {
             objAccountsDAL = new AccountsDAL();
             return objAccountsDAL.GetNotificationsDAL();
         }


         public Array GetDPIUAutherizationIssuedDetailsListBAL(int page, int rows, string sidx, string sord, out long totalRecords)
         {
             objAccountsDAL = new AccountsDAL();
             return objAccountsDAL.GetDPIUAutherizationIssuedDetailsListDAL(page, rows, sidx, sord, out totalRecords);
         }

         public List<SelectListItem> PopulateSRRDA(int id=0) {
             objAccountsDAL = new AccountsDAL();
             return objAccountsDAL.PopulateSRRDA(id);
         }

         #region Account ATR upload(by Pradip Patil 26/05/2017 12:30 PM)

         public Boolean SaveObservationDetailsBAL(ObservationModel model, int state, int agency, int year, out string message)
         {
             objAccountsDAL = new AccountsDAL();
             return objAccountsDAL.SaveObservationDetailsDAL(model, state, agency, year, out message);
         }

         public Array GetObservationListBAL(int? page, int? rows, string sord, string sidx, out long totalrecords)
         {
             objAccountsDAL = new AccountsDAL();
             return objAccountsDAL.GetObservationListDAL(page, rows, sord, sidx, out totalrecords);
         }

         public Array GetDetailObservationListBAL(int? page, int? rows, string sord, string sidx, out long totalrecords, int id)
         {
             objAccountsDAL = new AccountsDAL();
             return objAccountsDAL.GetDetailObservationListDAL(page, rows, sord, sidx, out totalrecords, id);
         }

         public Array GetObservationReplyListBAL(int? page, int? rows, string sord, string sidx, out long totalrecords, int id)
         {
             objAccountsDAL = new AccountsDAL();
             return objAccountsDAL.GetObservationReplyListDAL(page, rows, sord, sidx, out totalrecords, id);
         }

         public Boolean UploadATRFileBAL(HttpPostedFileBase file, int masterAccId)
         {
             objAccountsDAL = new AccountsDAL();
             return objAccountsDAL.UploadATRFileDAL(file, masterAccId);
         }


         public Array GetATRFileListBAL(int? page, int? rows, string sord, string sidx, out long totalrecords, String id, out string filterValues)
         {
             objAccountsDAL = new AccountsDAL();
             return objAccountsDAL.GetATRFileListDAL(page, rows, sord, sidx, out totalrecords, id, out filterValues);
         }

         public Boolean DeleteObservationBAL(int ObservationId, int MasterObId, out String message)
         {
             objAccountsDAL = new AccountsDAL();
             return objAccountsDAL.DeleteObservationDAL(ObservationId, MasterObId, out message);
         }

         //public Boolean DeleteChildObservationBAL(int ObservationId, int MasterObId, out String message)
         //{
         //    objAccountsDAL = new AccountsDAL();
         //    return objAccountsDAL.DeleteChildObservationDAL(ObservationId, MasterObId, out message);
         //}
         public Boolean DeleteATRFIleBAL(String FIleName, int UploadId, out String message)
         {
             objAccountsDAL = new AccountsDAL();
             return objAccountsDAL.DeleteATRFIleDAL(FIleName, UploadId, out message);
         }
         #endregion
    }

    public interface IAccountsBAL
    {
        Array GetPFAuthorizationList(AccountsFilterModel objFilter, out long totalRecords);
        Array GetAssetliabilityList(AccountsFilterModel objFilter, out long totalRecords);
        List<SP_ACC_DB_ASSET_LIABILITIES_MAJOR_DETAILS_Result> GetAssetliabilityChart(AccountsFilterModel objFilter);
        List<SP_ACC_DB_ASSET_LIABILITIES_MINOR_DETAILS_Result> GetAssetliabilityDetailsChart(AccountsFilterModel objFilter);
        Array GetAuthorizationReceivedList(AccountsFilterModel objFilter, out long totalRecords);
        Array GetSummaryList(AccountsFilterModel objFilter, out long totalRecords);
        Array GetLettestTransactionsList(AccountsFilterModel objFilter, out long totalRecords);

        Array GetLatestDPIUTransactionsDetailsListBAL(int? page, int? rows, string sidx, string sord, out long totalRecords);

        List<string> GetNotificationsBAL();

        Array GetDPIUAutherizationIssuedDetailsListBAL(int page, int rows, string sidx, string sord, out long totalRecords);
        List<SelectListItem> PopulateSRRDA(int id);

        #region Account ATR upload(by Pradip Patil 26/05/2017 12:30 PM)

        Boolean SaveObservationDetailsBAL(ObservationModel model, int state, int agency, int year, out string message);

        Array GetObservationListBAL(int? page, int? rows, string sord, string sidx, out long totalrecords);

        Array GetDetailObservationListBAL(int? page, int? rows, string sord, string sidx, out long totalrecords, int id);

        Array GetObservationReplyListBAL(int? page, int? rows, string sord, string sidx, out long totalrecords, int id);

        Boolean UploadATRFileBAL(HttpPostedFileBase file, int masterAccId);

        Array GetATRFileListBAL(int? page, int? rows, string sord, string sidx, out long totalrecords, String id, out String filterValues);

        Boolean DeleteObservationBAL(int ObservationId, int MasterObId, out String message);

        //   Boolean DeleteChildObservationBAL(int ObservationId, int MasterObId, out String message);

        Boolean DeleteATRFIleBAL(String FIleName, int UploadId, out String message);
        #endregion
    }
}