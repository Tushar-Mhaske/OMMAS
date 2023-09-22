#region HEADER
/*
 * Project Id:

 * Project Name:OMMAS-II

 * File Name: FundBAL.cs

 * Author : Vikram Nandanwar
 
 * Creation Date :05/June/2013

 * Desc : This class is used as BAL to call methods present in the DAL for Save,Edit,Update,Delete and listing of Fund Allocation and Release screens.  
 
 */
#endregion

using PMGSY.DAL.FundDAL;
using PMGSY.Models;
using PMGSY.Models.Fund;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.BAL.Fund
{
    public class FundBAL : IFundBAL
    {
        IFundDAL objDAL = new FundDAL();

        #region FUND_ALLOCATION

        public Array GetFundAllocationList(int stateCode, string fundType, int fundingAgencyCode, int yearCode, int page, int rows, string sord, string sidx, out long totalRecords)
        {
            return objDAL.GetFundAllocationList(stateCode, fundType, fundingAgencyCode, yearCode, page, rows, sord, sidx, out totalRecords);
        }

        public bool AddFundAllocation(FundAllocationViewModel fundModel, ref string message)
        {
            return objDAL.AddFundAllocation(fundModel, ref message);
        }

        public bool DeleteFundAllocation(int transactionCode, int stateCode, int adminCode, int yearCode, string fundType, int fundingAgencyCode,ref string message)
        {
            return objDAL.DeleteFundAllocation(transactionCode, stateCode, adminCode, yearCode, fundType, fundingAgencyCode,ref message);
        }

        public FundAllocationViewModel GetFundAllocationDetails(int transactionCode, int stateCode, int adminCode, int yearCode, string fundType, int fundingAgencyCode)
        {
            return objDAL.GetFundAllocationDetails(transactionCode, stateCode, adminCode, yearCode, fundType, fundingAgencyCode);
        }

        public bool EditFundAllocation(FundAllocationViewModel fundModel, ref string message)
        {
            return objDAL.EditFundAllocation(fundModel, ref message);
        }

        public bool AddFileUploadToTransaction(FundAllocationViewModel fundModel)
        {
            return objDAL.AddFileUploadToTransaction(fundModel);
        }

        public bool CheckReleaseAmount(int stateCode,int yearCode,int adminCode,int fundingAgencyCode,string fundType)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                decimal? TotalRelease = dbContext.MRD_FUND_RELEASE.Where(m => m.MAST_STATE_CODE == stateCode && m.ADMIN_NO_CODE == adminCode && m.MAST_YEAR == yearCode && m.MAST_FUNDING_AGENCY_CODE == fundingAgencyCode && m.MAST_FUND_TYPE == fundType).Sum(m => m.MAST_RELEASE_AMOUNT);
                decimal TotalAllocation = dbContext.MRD_FUND_ALLOCATION.Where(m => m.MAST_STATE_CODE == stateCode && m.ADMIN_NO_CODE == adminCode && m.MAST_YEAR == yearCode && m.MAST_FUNDING_AGENCY_CODE == fundingAgencyCode && m.MAST_FUND_TYPE == fundType).Sum(m => m.MAST_ALLOCATION_AMOUNT);
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


        #endregion

        #region FUND_RELEASE

        public Array GetFundReleaseList(int stateCode, string fundType, int fundingAgencyCode, int yearCode, string releaser,int page, int rows, string sord, string sidx, out long totalRecords)
        {
            return objDAL.GetFundReleaseList(stateCode, fundType, fundingAgencyCode, yearCode, releaser,page, rows, sord, sidx, out totalRecords);
        }

        public bool AddFundRelease(FundReleaseViewModel fundModel, ref string message)
        {
            return objDAL.AddFundRelease(fundModel, ref message);
        }

        public bool DeleteFundRelease(int transactionCode, int stateCode, int adminCode, int yearCode, string releaseType, string fundType, int fundingAgencyCode)
        {
            return objDAL.DeleteFundRelease(transactionCode, stateCode, adminCode, yearCode, releaseType, fundType, fundingAgencyCode);
        }

        public FundReleaseViewModel GetFundReleaseDetails(int transactionCode, int stateCode, int adminCode, int yearCode, string releaseType, string fundType, int fundingAgencyCode)
        {
            return objDAL.GetFundReleaseDetails(transactionCode, stateCode, adminCode, yearCode, releaseType, fundType, fundingAgencyCode);
        }

        public bool EditFundRelease(FundReleaseViewModel fundModel, ref string message)
        {
            return objDAL.EditFundRelease(fundModel, ref message);
        }

        public bool AddFileUploadToTransactionRelease(FundReleaseViewModel fundModel)
        {
            return objDAL.AddFileUploadToTransactionRelease(fundModel);
        }

        public Array GetListFilesBAL(int page, int rows, string sidx, string sord, out long totalRecords, int stateCode, int adminCode, string fundType, int agencyCode, int yearCode, int transactionCode)
        {
            return objDAL.GetListFilesDAL(page,rows,sidx,sord,out totalRecords,stateCode,adminCode,fundType,agencyCode,yearCode,transactionCode);
        }

        public Array GetListFundReleaseFilesBAL(int page, int rows, string sidx, string sord, out long totalRecords, int stateCode, int adminCode, string fundType, int agencyCode, int yearCode, int transactionCode,string releaseType)
        {
            return objDAL.GetListFundReleaseFilesDAL(page, rows, sidx, sord, out totalRecords, stateCode, adminCode, fundType, agencyCode, yearCode, transactionCode,releaseType);
        }

        public bool DeleteFundReleaseFile(string fileName, int stateCode,ref string message)
        {
            return objDAL.DeleteFundReleaseFile(fileName,stateCode,ref message);
        }

        #endregion
    }

    public interface IFundBAL
    {

        #region FUND_ALLOCATION

        Array GetFundAllocationList(int stateCode, string fundType, int fundingAgencyCode, int yearCode, int page, int rows, string sord, string sidx, out long totalRecords);
        bool AddFundAllocation(FundAllocationViewModel fundModel, ref string message);
        bool DeleteFundAllocation(int transactionCode, int stateCode, int adminCode, int yearCode, string fundType, int fundingAgencyCode,ref string message);
        FundAllocationViewModel GetFundAllocationDetails(int transactionCode, int stateCode, int adminCode, int yearCode, string fundType, int fundingAgencyCode);
        bool EditFundAllocation(FundAllocationViewModel fundModel, ref string message);
        bool AddFileUploadToTransaction(FundAllocationViewModel fundModel);


        #endregion

        #region FUND_RELEASE

        Array GetFundReleaseList(int stateCode, string fundType, int fundingAgencyCode, int yearCode, string releaser,int page, int rows, string sord, string sidx, out long totalRecords);
        bool AddFundRelease(FundReleaseViewModel fundModel, ref string message);
        bool DeleteFundRelease(int transactionCode, int stateCode, int adminCode, int yearCode, string releaseType, string fundType, int fundingAgencyCode);
        FundReleaseViewModel GetFundReleaseDetails(int transactionCode, int stateCode, int adminCode, int yearCode, string releaseType, string fundType, int fundingAgencyCode);
        bool EditFundRelease(FundReleaseViewModel fundModel, ref string message);
        bool AddFileUploadToTransactionRelease(FundReleaseViewModel fundModel);
        Array GetListFilesBAL(int page, int rows, string sidx, string sord, out long totalRecords, int stateCode, int adminCode, string fundType, int agencyCode, int yearCode, int transactionCode);
        Array  GetListFundReleaseFilesBAL(int page, int rows, string sidx, string sord, out long totalRecords, int stateCode, int adminCode, string fundType, int agencyCode, int yearCode, int transactionCode,string releaseType);
        bool DeleteFundReleaseFile(string fileName, int stateCode,ref string message);

        #endregion
    }
}