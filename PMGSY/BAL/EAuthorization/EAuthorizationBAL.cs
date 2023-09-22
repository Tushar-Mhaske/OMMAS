
#region File Header
/*
        * Project Id    :
        * Project Name  :   PMGSY
        * Name          :   EAuthorizationBAL.cs        
        * Description   :   EAuthorization PIU AND EO Details & Various Functions
        * Author        :   Avinash
        * Creation Date :   15/November/2018
 **/
#endregion
using PMGSY.Common;
using PMGSY.DAL.EAuthorization;
using PMGSY.Models;
using PMGSY.Models.Common;
using PMGSY.Models.EAuthorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.BAL.EAuthorization
{
    public class EAuthorizationBAL : IEAuthorizationBAL
    {

        #region Properties
        IEAuthorizationDAL objIEAuthorizationDAL = null;
        #endregion


        public EAuthorizationBAL()
        {
            objIEAuthorizationDAL = new EAuthorizationDAL();
        }
        #region methods

        public Array EAuthorizationRequestListView(EAuthorizationFilterModel objFilter, out long totalRecords)
        {
            try
            {
                return objIEAuthorizationDAL.EAuthorizationRequestListView(objFilter, out totalRecords);
            }
            catch (Exception ex)
            {

                throw new Exception("Error while getting Authorization request list...");
            }

        }

        public EAuthorizationMasterModel AddEAuthorizationMasterDetails(EAuthorizationMasterModel model)
        {
            try
            {
                
                return objIEAuthorizationDAL.AddEAuthorizationMasterDetails(model);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while adding Authorization request master details list...");
            }

        }



        public string GetAuthorizationNumber(int month, int year, int stateCode, int adminNdCode)
        {
            try
            {
                string AuthorizationNo = objIEAuthorizationDAL.GetAuthorizationNumber(month, year, stateCode, adminNdCode);
                return AuthorizationNo;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while Getting Authorization Number");
            }

        }

        public ACC_EAUTH_MASTER GetMasterAuthorizationDetails(long auth_id)
        {
            try
            {
                return objIEAuthorizationDAL.GetMasterAuthorizationDetails(auth_id);
            }
            catch (Exception ex)
            {

                throw new Exception("Error while getting authorization master details...");
            }


        }

        /// <summary>
        /// function to get the authorization master entry
        /// </summary>
        /// <param name="objFilter"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array ListEAuthorizationMasterDetails(EAuthorizationFilterModel objFilter, out long totalRecords)
        {
            try
            {
                return objIEAuthorizationDAL.ListEAuthorizationMasterDetails(objFilter, out totalRecords);
            }
            catch (Exception ex)
            {

                throw new Exception("Error while getting Authorization master details...");
            }
        }


        public Int32 AddPaymentTransactionDetails(EAuthorizationRequestDetailsModel model, string operationType, Int64 Bill_id, String AddorEdit, Int16 tranNumber)
        {
            try
            {
                return objIEAuthorizationDAL.AddPaymentTransactionDetails(model, operationType, Bill_id, AddorEdit, tranNumber);
            }
            catch (Exception ex)
            {

                throw new Exception("Error while adding  payment details...");
            }
        }

        public String SetPayeeName(TransactionParams objParam)
        {
            try
            {
                return objIEAuthorizationDAL.SetPayeeName(objParam);
            }
            catch (Exception ex)
            {

                throw new Exception("Error while getting Payee Name...");
            }

        }



        /// <summary>
        /// function to get the authorization master entry
        /// </summary>
        /// <param name="objFilter"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array GetPaymentDetailList(EAuthorizationFilterModel objFilter, out long totalRecords)
        {
            try
            {
                return objIEAuthorizationDAL.GetPaymentDetailList(objFilter, out totalRecords);
            }
            catch (Exception ex)
            {

                throw new Exception("Error while getting EAuthorization details...");
            }
        }

        #region Update EAuthorization Details
        public EAuthorizationRequestDetailsModel UpdateEAuthorizationDetails(EAuthorizationRequestDetailsModel model)
        {
            try
            {
                return objIEAuthorizationDAL.UpdateEAuthorizationDetails(model);
            }
            catch (Exception ex)
            {

                throw new Exception("Error while Updating EAuthorization details..");
            }
        }
        #endregion


        #region Delete  EAuthorization Details
        public EAuthorizationRequestDetailsModel DeleteEAuthorizationDetails(Int64 EAuthID, int TxNo)
        {
            try
            {
                return objIEAuthorizationDAL.DeleteEAuthorizationDetails(EAuthID,TxNo);
            }
            catch (Exception ex)
            {

                throw new Exception("Error while Deleting EAuthorization details..");
            }

        }
        #endregion


        #region Finalize eAuthorization Details
        public EAuthorizationRequestDetailsModel FinalizeEAuthorizationDetails(Int64 EAuthID)
        {
            try
            {
                return objIEAuthorizationDAL.FinalizeEAuthorizationDetails(EAuthID);
            }
            catch (Exception ex)
            {

                throw new Exception("Error while Deleting EAuthorization details..");
            }


        }
        #endregion


        #region Delete EAuthorization Master Details
        public EAuthorizationRequestDetailsModel DeleteEAuthorizationMaster(Int64 EAuthID)
        {
            try
            {
                return objIEAuthorizationDAL.DeleteEAuthorizationMaster(EAuthID);
            }
            catch (Exception ex)
            {

                throw new Exception("Error while Deleting EAuthorization Master..");
            }

        }
        #endregion


        #region  SRRDA Main View Data
        public Array SRRDAeAuthorizationRequestListData(EAuthorizationFilterModel objFilter, out long totalRecords)
        {
            try
            {
                return objIEAuthorizationDAL.SRRDAeAuthorizationRequestListData(objFilter,out totalRecords);
            }
            catch (Exception ex)
            {

                throw new Exception("Error while Getting EAuthorization Request Details...");
            }
        }
        #endregion

        #region Get SRRDA DPIU List
        public List<SelectListItem> PopulateDPIUForSRRDA(TransactionParams objParam)
        {
            try
            {
                return objIEAuthorizationDAL.PopulateDPIUForSRRDA(objParam);
            }
            catch (Exception ex)
            {

                throw new Exception("Error while Getting DPIU List...");
            }

        }
        #endregion
        #region GetSRRDA Status List
        public List<SelectListItem> PopulateSTATUSForSRRDA()
        {
            try
            {
                return objIEAuthorizationDAL.PopulateSTATUSForSRRDA();
            }
            catch (Exception ex)
            {

                throw new Exception("Error while Getting Status List...");
            }
        }
        #endregion


        #region SRRDA Details Grid
        public Array GetSRRDAeAuthDetailListForApproval(EAuthorizationFilterModel objFilter, out long totalRecords)
        {
            try
            {
                return objIEAuthorizationDAL.GetSRRDAeAuthDetailListForApproval(objFilter, out totalRecords);
            }
            catch (Exception ex)
            {

                throw new Exception("Error while Getting EAuthorization Request Details...");
            }

        }
         #endregion



        #region SRRDA SaveApprove Reject Details
        public EAuthorizationRequestDetailsModel ProceedForApproveRejectDetails(string ApproveArr, string RejectArr, Int64 EAuthID)
        {
            try
            {
                return objIEAuthorizationDAL.ProceedForApproveRejectDetails(ApproveArr, RejectArr, EAuthID);
            }
            catch (Exception ex)
            {

                throw new Exception("Error while Getting EAuthorization Request Details...");
            }
        }
        #endregion




        /// <summary>
        /// function to get the closing balance for the payment
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public UDF_ACC_GEN_GET_BA_CASH_CLOSING_BALANCES_Result GetBankAuthorizationAvailable(TransactionParams param)
        {
            try
            {
                return objIEAuthorizationDAL.GetBankAuthorizationAvailable(param);
            }
            catch (Exception Ex)
            {
                
                throw new Exception("Error while getting payment balance...");
            }

        }


        /// <summary>
        /// function to Save Notification After Sending Mail
        /// </summary>
        /// <param name="EncryptedEAuthID"></param>
        /// <returns></returns>
        public EAuthorizationRequestDetailsModel SaveNotificationDetailsAfterSendingMail(string EncryptedEAuthID)
        {
            try
            {
                return objIEAuthorizationDAL.SaveNotificationDetailsAfterSendingMail(EncryptedEAuthID);
            }
            catch (Exception Ex)
            {
                
                throw new Exception("Error while Saving Notification Details...");
            }


        }


        /// <summary>
        /// This Method Returns already Authorised Amount
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public EAuthorizationAmountRequestModel CheckAlreadyAuthorisedAmount(EAuthorizationAmountRequestModel model)
        {
            try
            {
                return objIEAuthorizationDAL.CheckAlreadyAuthorisedAmount(model);
            }
            catch (Exception Ex)
            {
                
                throw new Exception("Error while Saving Notification Details...");
            }


        }

        /// <summary>
        /// This Method Returns already Authorised Amount
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public EAuthorizationAmountRequestModel GetAddEAuthorizationLinkView(EAuthorizationAmountRequestModel model)
        {
            try
            {
                return objIEAuthorizationDAL.GetAddEAuthorizationLinkView(model);
            }
            catch (Exception Ex)
            {

                throw new Exception("Error while Geting Authorization Details...");
            }
        }

        public bool AddNewAuthorizationEntry(EAuthorizationAmountRequestModel model)
        {
            try
            {
                return objIEAuthorizationDAL.AddNewAuthorizationEntry(model);
            }
            catch (Exception Ex)
            {

                throw new Exception("Error while Adding  new Authorization Entry...");
            }

        }


        #endregion

    }



    public interface IEAuthorizationBAL
    {
        EAuthorizationRequestDetailsModel DeleteEAuthorizationMaster(Int64 EAuthID);
        EAuthorizationMasterModel AddEAuthorizationMasterDetails(EAuthorizationMasterModel model);
        string GetAuthorizationNumber(int month, int year, int stateCode, int adminNdCode);
        Array EAuthorizationRequestListView(EAuthorizationFilterModel objFilter, out long totalRecords);
        ACC_EAUTH_MASTER GetMasterAuthorizationDetails(long auth_id);
        Array ListEAuthorizationMasterDetails(EAuthorizationFilterModel objFilter, out long totalRecords);
        Array GetPaymentDetailList(EAuthorizationFilterModel objFilter, out long totalRecords);
        Int32 AddPaymentTransactionDetails(EAuthorizationRequestDetailsModel model, string operationType, Int64 Bill_id, String AddorEdit, Int16 tranNumber);
        String SetPayeeName(TransactionParams objParam);
        EAuthorizationRequestDetailsModel UpdateEAuthorizationDetails(EAuthorizationRequestDetailsModel model);
        EAuthorizationRequestDetailsModel DeleteEAuthorizationDetails(Int64 EAuthID, int TxNo);
        EAuthorizationRequestDetailsModel FinalizeEAuthorizationDetails(Int64 EAuthID);
        UDF_ACC_GEN_GET_BA_CASH_CLOSING_BALANCES_Result GetBankAuthorizationAvailable(TransactionParams param);
        EAuthorizationAmountRequestModel CheckAlreadyAuthorisedAmount(EAuthorizationAmountRequestModel model);
        EAuthorizationAmountRequestModel GetAddEAuthorizationLinkView(EAuthorizationAmountRequestModel model);
        bool AddNewAuthorizationEntry(EAuthorizationAmountRequestModel model);
        #region SRRDA
        Array SRRDAeAuthorizationRequestListData(EAuthorizationFilterModel objFilter, out long totalRecords);
        Array GetSRRDAeAuthDetailListForApproval(EAuthorizationFilterModel objFilter, out long totalRecords);
        EAuthorizationRequestDetailsModel ProceedForApproveRejectDetails(string ApproveArr, string RejectArr, Int64 EAuthID);
        List<SelectListItem> PopulateDPIUForSRRDA(TransactionParams objParam);
        List<SelectListItem> PopulateSTATUSForSRRDA();
        EAuthorizationRequestDetailsModel SaveNotificationDetailsAfterSendingMail(string EncryptedEAuthID);
        #endregion


    }
}