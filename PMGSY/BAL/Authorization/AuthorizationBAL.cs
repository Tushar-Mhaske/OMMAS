using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.DAL.Authorization;
using PMGSY.Models.Authorization;
using PMGSY.Models;
using System.Web.Mvc;

namespace PMGSY.BAL.Authorization
{
    public class AuthorizationBAL : IAuthorizationBAL
    {
        IAuthorizationDAL objAuthDAL = new AuthorizationDAL();

        public Array AuthorizationRequestList(AuthorizationFilter objFilter, out long totalRecords)
        {
            return objAuthDAL.AuthorizationRequestList(objFilter, out totalRecords);
        }

        public String AddRequestTrackingDetails(ListAutorizationRequestModel model)
        {
            return objAuthDAL.AddRequestTrackingDetails(model);
        }

        public ACC_AUTH_REQUEST_MASTER GetAuthorizationRequestMaster(Int64 authId)
        {
            return objAuthDAL.GetAuthorizationRequestMaster(authId);
        }

        public String AddReceiptDetails(ReceiptDetailsModel receiptModel, ref string message)
        {
            return objAuthDAL.AddReceiptDetails(receiptModel, ref message);
        }

        public String AddPaymentDetails(PaymentDetailsModel paymentModel)
        {
            return objAuthDAL.AddPaymentDetails(paymentModel);
        }

        public ACC_BILL_MASTER GetBillMaster(Int64 billId)
        {
            return objAuthDAL.GetBillMaster(billId);
        }

        # region authorization request


        /// <summary>
        /// function to list the authorization request details for master page list
        /// </summary>
        /// <param name="objFilter"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array ListAuthorizationRequestDetails(PaymentFilterModel objFilter, out long totalRecords)
        {
            try
            {
                return objAuthDAL.ListAuthorizationRequestDetails(objFilter, out totalRecords);
            }
            catch (Exception ex)
            {

                throw new Exception("Error while getting Authorization request list...");
            }
        }

        /// <summary>
        /// function to get the authorization master entry
        /// </summary>
        /// <param name="objFilter"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array ListAuthorizationMasterDetails(PaymentFilterModel objFilter, out long totalRecords)
        {
            try
            {
                return objAuthDAL.ListAuthorizationMasterDetails(objFilter, out totalRecords);
            }
            catch (Exception ex)
            {

                throw new Exception("Error while getting Authorization master details...");
            }
        }




        /// <summary>
        /// BAL function to Add and edit the authorization master request page
        /// </summary>
        /// <param name="model"> authorization model to add edit</param>
        /// <param name="operation"> A for Add E for Edit</param>
        /// <param name="Auth_Id"> In case of Edit its auth id of the auth request to edit</param>
        /// <returns>result from DAL fuction </returns>
        public Int64 AddEditMasterAuthorizationDetails(AuthorizationRequestMasterModel model, string operation, Int64 Auth_Id)
        {
            try
            {
                return objAuthDAL.AddEditMasterAuthorizationDetails(model, operation, Auth_Id);
            }
            catch (Exception ex)
            {

                throw new Exception("Error while adding Authorization request master details list...");
            }
        }


        /// <summary>
        /// BAL function  to get the authorization number
        /// </summary>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <param name="StateCode"></param>
        /// <param name="AdminNdCode"></param>
        /// <returns></returns>
        public string GetAuthorizationNumber(short month, short year, int StateCode, int AdminNdCode)
        {
            try
            {
                return objAuthDAL.GetAuthorizationNumber(month, year, StateCode, AdminNdCode);
            }
            catch (Exception ex)
            {

                throw new Exception("Error while adding Authorization request master details list...");
            }
        }


        /// <summary>
        /// function to calculate the authorization balances
        /// </summary>
        /// <param name="authID"></param>
        /// <returns></returns>
        public AmountCalculationModel GetAuthorizationAmountBalance(Int64 authID)
        {
            try
            {
                return objAuthDAL.GetAuthorizationAmountBalance(authID);
            }
            catch (Exception ex)
            {

                throw new Exception("Error while getting Authorization balance details...");
            }
        }


        /// <summary>
        /// function to delete the authorization details
        /// </summary>
        /// <param name="auth_id"></param>
        /// <returns></returns>
        public String DeleteAuthorizationRequest(long auth_id)
        {

            try
            {
                return objAuthDAL.DeleteAuthorizationRequest(auth_id);
            }
            catch (Exception ex)
            {

                throw new Exception("Error while deleting authorization details...");
            }
        }

        /// <summary>
        /// function to get master authorization details
        /// </summary>
        /// <param name="auth_id"></param>
        /// <returns></returns>
        public ACC_AUTH_REQUEST_MASTER GetMasterAuthorizationDetails(long auth_id)
        {
            try
            {
                return objAuthDAL.GetMasterAuthorizationDetails(auth_id);
            }
            catch (Exception ex)
            {

                throw new Exception("Error while getting authorization master details...");
            }

        }

        /// <summary>
        /// function to  get the authorization transaction details
        /// </summary>
        /// <param name="auth_id"></param>
        /// <returns></returns>
        public ACC_AUTH_REQUEST_DETAILS GetAuthTransactionDetails(long auth_id, int transId)
        {
            try
            {
                return objAuthDAL.GetAuthTransactionDetails(auth_id, transId);
            }
            catch (Exception ex)
            {

                throw new Exception("Error while getting authorization transaction  details...");
            }

        }

        /// <summary>
        /// function to finalize the authorization
        /// </summary>
        /// <param name="auth_id"></param>
        /// <returns></returns>
        public String FinlizeAuthorization(long auth_id)
        {

            try
            {
                return objAuthDAL.FinlizeAuthorization(auth_id);
            }
            catch (Exception ex)
            {

                throw new Exception("Error while finalizing authorization details...");
            }

        }

        public Array ListAuthDeductionDetailsForDataEntry(PaymentFilterModel objFilter, out long totalRecords)
        {
            try
            {
                return objAuthDAL.ListAuthDeductionDetailsForDataEntry(objFilter, out totalRecords);
            }
            catch (Exception ex)
            {

                throw new Exception("Error while getting authorization transaction  list...");
            }
        }


        /// <summary>
        /// function to add edit transaction details
        /// </summary>
        /// <param name="model"></param>
        /// <param name="operationType"></param>
        /// <param name="Bill_id"></param>
        /// <param name="AddorEdit"></param>
        /// <param name="tranNumber"></param>
        /// <returns></returns>
        public Boolean AddEditTransactionDeductionPaymentDetails(AuthorizationRequestDetailsModel model, string operationType, Int64 Bill_id, String AddorEdit, Int16 tranNumber)
        {
            try
            {
                return objAuthDAL.AddEditTransactionDeductionPaymentDetails(model, operationType, Bill_id, AddorEdit, tranNumber);
            }
            catch (Exception ex)
            {

                throw new Exception("Error while adding  payment details...");
            }
        }

        /// <summary>
        /// functiont to delete the transaction details
        /// </summary>
        /// <param name="master_Bill_Id"></param>
        /// <param name="tranNumber"></param>
        /// <param name="paymentDeduction"></param>
        /// <returns></returns>
        public Int32 DeleteAuthorizationTransDetails(Int64 master_Bill_Id, Int32 tranNumber, String paymentDeduction)
        {
            try
            {
                return objAuthDAL.DeleteAuthorizationTransDetails(master_Bill_Id, tranNumber, paymentDeduction);
            }
            catch (Exception ex)
            {

                throw new Exception("Error while deleting  payment details...");
            }
        }


        /// <summary>
        /// BAL function to get the agrrement details for deduction transaction details
        /// </summary>
        /// <param name="bill_id"></param>
        /// <returns></returns>
        public String GetAgreemntNumberForAuthorization(Int64 bill_id)
        {
            try
            {
                return objAuthDAL.GetAgreemntNumberForAuthorization(bill_id);
            }
            catch (Exception ex)
            {

                throw new Exception("Error while Getting transaction payment details...");
            }

        }

        /// <summary>
        /// function to get the final autorization details
        /// </summary>
        /// <param name="BILL_ID"></param>
        /// <param name="roadID"></param>
        /// <returns></returns>
        public List<SelectListItem> GetFinalPaymentDetails(Int64 BILL_ID, Int32 roadID)
        {
            try
            {
                return objAuthDAL.GetFinalPaymentDetails(BILL_ID, roadID);
            }
            catch (Exception ex)
            {

                throw new Exception("Error while getting final authorization details...");
            }

        }

        /// <summary>
        /// Action to check if bank authorization is enabled
        /// </summary>
        /// <param name="adminNdCode"></param>
        /// <returns></returns>
        public bool CheckIfBankRequestEnabled(long adminNdCode)
        {
            try
            {
                return objAuthDAL.CheckIfBankRequestEnabled(adminNdCode);
            }
            catch (Exception ex)
            {

                throw new Exception("Error while checking if bank authorization request is enabled");
            }
        
        }

        # endregion  authorization request

        //new method added by Vikram on 29-08-2013
        public bool ValidateDPIUBankAuthBAL(int adminCode)
        {
            return objAuthDAL.ValidateDPIUBankAuthDAL(adminCode);
        }


    }

    public interface IAuthorizationBAL
    {
        Array AuthorizationRequestList(AuthorizationFilter objFilter, out long totalRecords);
        String AddRequestTrackingDetails(ListAutorizationRequestModel model);
        ACC_AUTH_REQUEST_MASTER GetAuthorizationRequestMaster(Int64 authId);
        String AddReceiptDetails(ReceiptDetailsModel receiptModel,  ref string message);
        String AddPaymentDetails(PaymentDetailsModel paymentModel);
        ACC_BILL_MASTER GetBillMaster(Int64 billId);

        # region authorization request
        Array ListAuthorizationRequestDetails(PaymentFilterModel objFilter, out long totalRecords);
        Array ListAuthorizationMasterDetails(PaymentFilterModel objFilter, out long totalRecords);
        long AddEditMasterAuthorizationDetails(AuthorizationRequestMasterModel model, string operation, Int64 Auth_Id);
        string GetAuthorizationNumber(short month, short year, int StateCode, int AdminNdCode);
        AmountCalculationModel GetAuthorizationAmountBalance(Int64 authID);
        String DeleteAuthorizationRequest(long auth_id);
        ACC_AUTH_REQUEST_MASTER GetMasterAuthorizationDetails(long auth_id);
        ACC_AUTH_REQUEST_DETAILS GetAuthTransactionDetails(long auth_id, int transId);
        String FinlizeAuthorization(long auth_id);
        Array ListAuthDeductionDetailsForDataEntry(PaymentFilterModel objFilter, out long totalRecords);
        Boolean AddEditTransactionDeductionPaymentDetails(AuthorizationRequestDetailsModel model, string operationType, Int64 Bill_id, String AddorEdit, Int16 tranNumber);
        Int32 DeleteAuthorizationTransDetails(Int64 master_Bill_Id, Int32 tranNumber, String paymentDeduction);
        String GetAgreemntNumberForAuthorization(Int64 bill_id);
        List<SelectListItem> GetFinalPaymentDetails(Int64 BILL_ID, Int32 roadID);
        bool CheckIfBankRequestEnabled(long adminNdCode);
        # endregion  authorization request

        bool ValidateDPIUBankAuthBAL(int adminCode);

    }

}