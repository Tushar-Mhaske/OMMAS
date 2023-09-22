using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.DAL.TransferEntryOrder;
using PMGSY.Models.TransferEntryOrder;
using PMGSY.Models.Receipts;
using PMGSY.Models;
using PMGSY.Common;

namespace PMGSY.BAL.TransferEntryOrder
{
    public class TransferEntryOrderBAL : ITransferEntryOrderBAL
    {
        private ITransferEntryOrderDAL objTeoDAL = new TransferEntryOrderDAL();

        public Array TEOList(ReceiptFilterModel objFilter, out long totalRecords, bool isTransferofBalances = false)
        {
            return objTeoDAL.TEOList(objFilter, out totalRecords, isTransferofBalances);
        }

        public Int64 AddTEOMaster(TeoMasterModel teoMasterModel)
        {
            return objTeoDAL.AddTEOMaster(teoMasterModel);
        }

        public Int64 AddImprestMaster(TeoMasterModel teoMasterModel)
        {
            return objTeoDAL.AddImprestMaster(teoMasterModel);
        }

        public List<ModelErrorList> GetAddTEODetailsModelErrors(TeoDetailsModel teoDetailsModel, String CrediDebit)
        {
            return objTeoDAL.GetAddTEODetailsModelErrors(teoDetailsModel, CrediDebit);
        }

        public String AddCreditTEODetails(TeoDetailsModel teoDetailsModel, out Int16 transId)
        {
            transId = 0;
            return objTeoDAL.AddCreditTEODetails(teoDetailsModel, out transId);
        }

        public String AddDebitTEODetails(TeoDetailsModel teoDetailsModel, out Int16 transId)
        {
            transId = 0;
            return objTeoDAL.AddDebitTEODetails(teoDetailsModel, out transId);
        }

        public String EditCreditTEODetails(TeoDetailsModel teoDetailsModel)
        {
            return objTeoDAL.EditCreditTEODetails(teoDetailsModel);
        }

        public String EditDebitTEODetails(TeoDetailsModel teoDetailsModel)
        {
            return objTeoDAL.EditDebitTEODetails(teoDetailsModel);
        }

        public Array TEOMasterList(ReceiptFilterModel objFilter)
        {
            return objTeoDAL.TEOMasterList(objFilter);
        }

        public Array TEODetailsList(ReceiptFilterModel objFilter, out long totalRecords, out decimal cTotalAmount, out decimal dTotalAmount, out decimal GrossAmount, bool isTransferofBalances = false)
        {
            totalRecords = 0;
            cTotalAmount = 0;
            dTotalAmount = 0;
            return objTeoDAL.TEODetailsList(objFilter, out totalRecords, out cTotalAmount, out dTotalAmount, out GrossAmount, isTransferofBalances);
        }

        public Array ImprestMasterList(ReceiptFilterModel objFilter, out long totalRecords)
        {
            totalRecords = 0;
            return objTeoDAL.ImprestMasterList(objFilter, out totalRecords);
        }

        /// <summary>
        /// BAL function to return the imprest settlement details
        /// </summary>
        /// <param name="objFilter"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array ImprestSettlementMasterList(ReceiptFilterModel objFilter, out long totalRecords)
        {
            totalRecords = 0;
            return objTeoDAL.ImprestSettlementMasterList(objFilter, out totalRecords);
        }


        public TeoMasterModel GetTEOMaster(Int64 billId)
        {
            return objTeoDAL.GetTEOMaster(billId);
        }

        public String ValidateEditTEOMaster(TeoMasterModel teoMasterModel)
        {
            return objTeoDAL.ValidateEditTEOMaster(teoMasterModel);
        }

        public String EditTEOMaster(TeoMasterModel teoMasterModel)
        {
            return objTeoDAL.EditTEOMaster(teoMasterModel);
        }

        public Int16 GetMasterTransId(Int64 billId)
        {
            return objTeoDAL.GetMasterTransId(billId);
        }

        public String DeleteTEO(Int64 billId)
        {
            return objTeoDAL.DeleteTEO(billId);
        }

        public String DeleteTEODetails(Int64 billId, Int16 transId)
        {
            return objTeoDAL.DeleteTEODetails(billId, transId);
        }

        public String FinalizeTEO(Int64 billId)
        {
            return objTeoDAL.FinalizeTEO(billId);
        }

        public TeoDetailsModel GetTEODetailByTransNo(Int64 billId, Int16 transId)
        {
            return objTeoDAL.GetTEODetailByTransNo(billId, transId);
        }

        public String IsTEOFinalized(Int64 billId)
        {
            return objTeoDAL.IsTEOFinalized(billId);
        }

        public ACC_BILL_DETAILS GetBillDetails(Int64 billId, string creditDebit)
        {
            return objTeoDAL.GetBillDetails(billId, creditDebit);
        }

        public String IsMultipleTransactionRequired(Int64 billId)
        {
            CommonFunctions objCommonFunc = new CommonFunctions();
            ACC_TEO_SCREEN_DESIGN_PARAM_DETAILS designParams = new ACC_TEO_SCREEN_DESIGN_PARAM_DETAILS();
            designParams = objCommonFunc.getTEODesignParamDetails(billId, 0);
            return designParams.MTXN_REQ;
        }

        public String IsFinalPayment(String parameters,string id)
        {
            return objTeoDAL.IsFinalPayment(parameters,id);
        }


        public string AddCreditDebitTEODetailsforTOB(TeoDetailsModelForTOB teoDetailsModelForTOB, out short transId)
        {
            return objTeoDAL.AddCreditDebitTEODetailsforTOB(teoDetailsModelForTOB, out transId);
        }


        public bool DeleteTEODetailsForTOB(long billID, short txnNo, ref string message)
        {
            return objTeoDAL.DeleteTEODetailsForTOB(billID, txnNo, ref message);
        }


        public bool CheckHeadAlreadyExistBAL(long billID, short headID, string creditDebit)
        {
            return objTeoDAL.CheckHeadAlreadyExistDAL(billID, headID, creditDebit);
        }


        public bool ValidateGrossAmount(TeoMasterModel teoMasterModel, ref string ValidationSummary)
        {
            return objTeoDAL.ValidateGrossAmount(teoMasterModel, ref ValidationSummary);
        }

        #region NEW_CHANGE_TOB_AUTO_ENTRY

        //new method added by Vikram on 07-10-2013 for auto entry of TOB
        public bool AddAutoEntryTOB(long billId)
        {
            return objTeoDAL.AddAutoEntryTOB(billId);
        }

        #endregion

        //added by Koustubh Nakate on 14/10/2013 to check transaction is already exist or not for given BILL ID  
        public bool CheckForTransactionAlreadyExistBAL(long billID, ref int DistrictC, ref int PIUC, ref int DistrictD, ref int PIUD, ref int? ContractorC, ref int? StateD)
        {
            return objTeoDAL.CheckForTransactionAlreadyExistDAL(billID, ref DistrictC, ref PIUC, ref DistrictD, ref  PIUD, ref ContractorC, ref StateD);
        }

        //added by Vikram on 15-10-2013 for validation of Transaction Head 
        public bool ValidateRoad(int proposalCode,string upagradeConnectFlag)
        {
            return objTeoDAL.ValidateRoad(proposalCode,upagradeConnectFlag);
        }


        //added by Koustubh Nakate on 17/10/2013 to check district has been shifted or not  
        public bool CheckIsDistrictShiftedBAL(int DistrictC, int DistrictD)
        {
            return objTeoDAL.CheckIsDistrictShiftedDAL(DistrictC, DistrictD);
        }

        //added by Vikram to check whether the selected DPIU has closed the month in which it is making the entry
        public bool ValidateDPIUMonths(int adminCode, string id)
        {
            return objTeoDAL.ValidateDPIUMonths(adminCode, id);
        }


        #region OLD_DATA_IMPREST_PAYMENT_MAPPING
        
        /// <summary>
        /// returns the old data imprest payments
        /// </summary>
        /// <param name="objFilter"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array OldImprestPaymentListBAL(ReceiptFilterModel objFilter,out long totalRecords)
        {
            return objTeoDAL.OldImprestPaymentListDAL(objFilter,out totalRecords);
        }

        /// <summary>
        /// returns the list of settled TEO and Reciept
        /// </summary>
        /// <param name="objFilter"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array ListSettledTEORecieptDetailsBAL(ReceiptFilterModel objFilter, out long totalRecords)
        {
            return objTeoDAL.ListSettledTEORecieptDetailsDAL(objFilter, out totalRecords);
        }

        public Boolean ValidatePaymentAmountBAL(String[] s_billIds,int P_BILL_ID,out string message)
        {
            return objTeoDAL.ValidatePaymentAmountDAL(s_billIds,P_BILL_ID,out message);
        }

        #endregion

        public bool ValidateTransaction(int txnId, string billDate,ref string message)
        {
            return objTeoDAL.ValidateTransaction(txnId,billDate,ref message);
        }


        //Added By Abhishek kamble To get Txn ID 16Oct2014 start
        public int GetMasterTXNId(Int64 billId)
        {
            return objTeoDAL.GetMasterTXNId(billId);
        }


        public string GetAgreementNumberForMF(int? ManeContractId)
        {
            return objTeoDAL.GetAgreementNumberForMF(ManeContractId); 
        }

    }

    public interface ITransferEntryOrderBAL
    {
        Int64 AddTEOMaster(TeoMasterModel teoMasterModel);
        List<ModelErrorList> GetAddTEODetailsModelErrors(TeoDetailsModel teoDetailsModel, String CreditDebit);
        String AddCreditTEODetails(TeoDetailsModel teoDetailsModel, out Int16 transId);
        String AddDebitTEODetails(TeoDetailsModel teoDetailsModel, out Int16 transId);
        String EditCreditTEODetails(TeoDetailsModel teoDetailsModel);
        String EditDebitTEODetails(TeoDetailsModel teoDetailsModel);
        Array TEOList(ReceiptFilterModel objFilter, out long totalRecords, bool isTransferofBalances = false);
        Array TEOMasterList(ReceiptFilterModel objFilter);
        Array TEODetailsList(ReceiptFilterModel objFilter, out long totalRecords, out decimal cTotalAmount, out decimal dTotalAmount, out decimal GrossAmount, bool isTransferofBalances = false);
        Array ImprestMasterList(ReceiptFilterModel objFilter, out long totalRecords);
        TeoMasterModel GetTEOMaster(Int64 billId);
        String ValidateEditTEOMaster(TeoMasterModel teoMasterModel);
        String EditTEOMaster(TeoMasterModel teoMasterModel);
        Int16 GetMasterTransId(Int64 billId);
        String DeleteTEODetails(Int64 billId, Int16 transId);
        String DeleteTEO(Int64 billId);
        String FinalizeTEO(Int64 billId);
        TeoDetailsModel GetTEODetailByTransNo(Int64 billId, Int16 transId);
        String IsTEOFinalized(Int64 billId);
        Int64 AddImprestMaster(TeoMasterModel teoMasterModel);
        ACC_BILL_DETAILS GetBillDetails(Int64 billId, string creditDebit);
        String IsMultipleTransactionRequired(Int64 billId);
        String IsFinalPayment(String parameters,string id);
        Array ImprestSettlementMasterList(ReceiptFilterModel objFilter, out long totalRecords);

        string AddCreditDebitTEODetailsforTOB(TeoDetailsModelForTOB teoDetailsModelForTOB, out short transId);

        bool DeleteTEODetailsForTOB(long billID, short txnNo, ref string message);
        //added by Koustubh Nakate on 30/09/2013 to check head is already exist or not for given BILL ID  
        bool CheckHeadAlreadyExistBAL(long billID, short headID, string creditDebit);

        //new method added by Vikram on 07-10-2013 for auto entry of TOB
        bool AddAutoEntryTOB(long billId);

        //added by Koustubh Nakate on 11/10/2013 to check entered amount exceeds its gross amount
        bool ValidateGrossAmount(TeoMasterModel teoMasterModel, ref string ValidationSummary);

        //added by Koustubh Nakate on 14/10/2013 to check transaction is already exist or not for given BILL ID  
        bool CheckForTransactionAlreadyExistBAL(long billID, ref int DistrictC, ref int PIUC, ref int DistrictD, ref int PIUD, ref int? ContractorC, ref int? StateD);

        //added by Vikram on 15-10-2013 for validation of Transaction Head 
        bool ValidateRoad(int proposalCode, string upagradeConnectFlag);


        //added by Koustubh Nakate on 17/10/2013 to check district has been shifted or not  
        bool CheckIsDistrictShiftedBAL(int DistrictC, int DistrictD);

        bool ValidateDPIUMonths(int adminCode, string id);

        #region OLD_DATA_IMPREST_PAYMENTS_MAPPING

        Array OldImprestPaymentListBAL(ReceiptFilterModel objFilter, out long totalRecords);
        Array ListSettledTEORecieptDetailsBAL(ReceiptFilterModel objFilter, out long totalRecords);
        Boolean ValidatePaymentAmountBAL(String[] s_billIds,int P_BILL_ID,out string message);

        #endregion

        bool ValidateTransaction(int txnId,string billDate,ref string message);

        //Added By Abhishek kamble To get Txn ID 16Oct2014 start
        int GetMasterTXNId(Int64 billId);

        string GetAgreementNumberForMF(int? ManeContractId);

    }
}