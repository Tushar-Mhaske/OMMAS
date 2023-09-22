using PMGSY.Models.Bank;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.DAL.Bank;
using PMGSY.Models;

namespace PMGSY.BAL.Bank
{
    public class BankBAL : IBankBAL
    {

        IBankDAL BankDALObj = null;

        public Array BankReconciliationList(BankFilterModel objFilter, out long totalRecords)
        {
            BankDALObj = new BankDAL();
            return BankDALObj.BankReconciliationList(objFilter, out totalRecords);
        }

        public String SaveBankReconciliedCheques(BankFilterModel objParam, ref string message)
        {
            BankDALObj = new BankDAL();
            return BankDALObj.SaveBankReconciliedCheques(objParam, ref message);
        }

        public bool AddBankDetails(BankDetailsViewModel bankModel, ref string message)
        {
            try
            {
                BankDALObj = new BankDAL();
                return BankDALObj.AddBankDetails(bankModel, ref message);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Error occurred while processing your request.";
                return false;
            }
        }

        public bool EditBankDetails(BankDetailsViewModel bankModel, ref string message)
        {
            try
            {
                BankDALObj = new BankDAL();
                return BankDALObj.EditBankDetails(bankModel, ref message);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Error occurred while processing your request.";
                return false;
            }
        }

        public BankDetailsViewModel GetBankDetails(int bankCode)
        {
            try
            {
                BankDALObj = new BankDAL();
                return BankDALObj.GetBankDetails(bankCode);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return null;
            }
        }

        public Array GetBankDetailsList(int? page, int? rows, string sidx, string sord, out long totalRecords, int stateCode)
        {
            try
            {
                BankDALObj = new BankDAL();
                return BankDALObj.GetBankDetailsList(page, rows, sidx, sord, out totalRecords, stateCode);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);

                totalRecords = 0;
                return null;
            }
        }

        public Array DisplayDPIUStatusList(int? page, int? rows, string sidx, string sord, out long totalRecords, int adminCode)
        {
            try
            {
                BankDALObj = new BankDAL();
                return BankDALObj.DisplayDPIUStatusList(page, rows, sidx, sord, out totalRecords, adminCode);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
        }

        public bool UpdateDPIUDetailsBAL(AdminDepartmentViewModel adminModel, ref string message)
        {
            try
            {
                BankDALObj = new BankDAL();
                return BankDALObj.UpdateDPIUDetailsDAL(adminModel, ref message);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
        }

        #region AUTH_SIGNATORY_AUTH_CODE

        public Array DisplayAuthSignatoryList(int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            BankDALObj = new BankDAL();
            return BankDALObj.DisplayAuthSignatoryList(page, rows, sidx, sord, out totalRecords);
        }

        public string GenerateKey()
        {
            BankDALObj = new BankDAL();
            return BankDALObj.GenerateKey();
        }

        public bool SaveAuthSigKey(int AdminNoOfficerCode, string AuthSigKey, ref string sendEmailTo)
        {
            BankDALObj = new BankDAL();
            return BankDALObj.SaveAuthSigKey(AdminNoOfficerCode, AuthSigKey, ref sendEmailTo);
        }

        #endregion

        #region SRRDA_PDF_OPEN_KEY

        public ADMIN_DEPARTMENT GetDepartmentDetails()
        {
            BankDALObj = new BankDAL();
            return BankDALObj.GetDepartmentDetails();
        }

        public ACC_EPAY_MAIL_OTHER GetEpayMailOtherDetails()
        {
            BankDALObj = new BankDAL();
            return BankDALObj.GetEpayMailOtherDetails();
        }

        public bool AddEditSRRDAPDFKeyDetails(SRRDAPdfKeyViewModel PdfKeymodel, int MailID, ref string message)
        {
            BankDALObj = new BankDAL();
            return BankDALObj.AddEditSRRDAPDFKeyDetails(PdfKeymodel, MailID, ref message);
        }


        #endregion SRRDA_PDF_OPEN_KEY         

        #region BankPDFKeyGeneration
        public Array DisplayBankDetailList(int? page, int? rows, string sidx, string sord, out long totalRecords, int agencyCode, String FundType)
        {
            BankDALObj = new BankDAL();
            return BankDALObj.DisplayBankDetailList(page, rows, sidx, sord, out totalRecords, agencyCode, FundType);
        }

        public bool SaveBankPDFOpenKey(int BankCode, String BankPDFKey, ref String sendEmailTo, String StateAgencyName,ref String message)
        {
            BankDALObj = new BankDAL();
            return BankDALObj.SaveBankPDFOpenKey(BankCode, BankPDFKey, ref sendEmailTo, StateAgencyName, ref message);
        }

        #endregion BankPDFKeyGeneration
    }

    public interface IBankBAL
    {
        Array BankReconciliationList(BankFilterModel objFilter, out long totalRecords);
        String SaveBankReconciliedCheques(BankFilterModel objParam, ref string message);
        bool AddBankDetails(BankDetailsViewModel bankModel, ref string message);
        bool EditBankDetails(BankDetailsViewModel bankModel, ref string message);
        BankDetailsViewModel GetBankDetails(int bankCode);
        Array GetBankDetailsList(int? page, int? rows, string sidx, string sord, out long totalRecords, int stateCode);
        Array DisplayDPIUStatusList(int? page, int? rows, string sidx, string sord, out long totalRecords, int adminCode);
        bool UpdateDPIUDetailsBAL(AdminDepartmentViewModel adminModel, ref string message);

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
}