using Mvc.Mailer;
using PMGSY.Common;
using PMGSY.DAL.Payment;
using PMGSY.Models;
using PMGSY.Models.Common;
using PMGSY.Models.Payment;
using PMGSY.Models.PaymentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Configuration;
using PMGSY.Extensions;


namespace PMGSY.BAL.Payment
{
    public class PaymentBAL : MailerBase, IPaymentBAL
    {

        IPaymentDAL objPaymentDAL = null;

        public PaymentBAL()
        {
            objPaymentDAL = new PaymentDAL();
        }

        #region master Payment
        /// <summary>
        /// function to add master payment details 
        /// Author: Amol Jadhav
        /// Date:26/04/2013
        /// 
        /// </summary>
        /// <param name="model">model to save</param>
        /// <returns> returns the Bill ID </returns>
        public Int64 AddEditMasterPaymentDetails(PaymentMasterModel model, string operationType, Int64 Bill_id, bool IsAdvicePayment)
        {
            try
            {
                return objPaymentDAL.AddEditMasterPaymentDetails(model, operationType, Bill_id,IsAdvicePayment);
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                      throw new Exception("Error while adding master payment details...");
            }
        }

        /// <summary>
        /// function to poplulate cheque book series
        /// </summary>
        /// <param name="admin_ND_Code"></param>
        /// <returns></returns>
        public List<SelectListItem> GetchequebookSeries(int admin_ND_Code, string fundType, Int16 levelID)
        {
            try
            {
                return objPaymentDAL.GetchequebookSeries(admin_ND_Code, fundType, levelID);
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                        throw new Exception("Error while getting cheque book series...");
            }

        }

        public String GetChequeBookIssueDate(Int64 chqBookId)
        {
            objPaymentDAL = new PaymentDAL();
            return objPaymentDAL.GetChequeBookIssueDate(chqBookId);
        }


        /// <summary>
        /// BAL function to get the cheque book numbers
        /// </summary>
        /// <param name="admin_ND_Code"></param>
        /// <param name="fundType"></param>
        /// <param name="levelID"></param>
        /// <returns></returns>
        //public List<SelectListItem> GetchequebookNumbers(int chequeBookId, int admin_nd_code,string fund_type)
        //{
        //    try
        //    {
        //        return objPaymentDAL.GetchequebookNumbers(chequeBookId, admin_nd_code, fund_type);
        //    }
        //    catch (Exception ex)
        //    {

        //        throw new Exception("Error while getting cheque book series...");
        //    }

        //}

        public String[] GetAllAvailableChequesArray(int chequeBookId, int admin_nd_code, string fund_type,string operation="A", Int64 billID=0)
        {
            try
            {
                return objPaymentDAL.GetAllAvailableChequesArray(chequeBookId, admin_nd_code, fund_type, operation, billID);
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                       throw new Exception("Error while getting cheque book series...");
            }

        }




        /// <summary>
        /// BAL function to get master payment list
        /// </summary>
        /// <param name="objFilter"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array ListMasterPaymentDetails(PaymentFilterModel objFilter, out long totalRecords)
        {
            try
            {
                return objPaymentDAL.ListMasterPaymentDetails(objFilter, out totalRecords);
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
               throw new Exception("Error while getting payment master list...");
            }
        }


        /// <summary>
        /// BAL function to get the master data  list
        /// </summary>
        /// <param name="objFilter">parameters for the grid </param>
        /// <param name="totalRecords">Number of Records</param>
        /// <returns></returns>
        public Array ListMasterPaymentDetailsForDataEntry(PaymentFilterModel objFilter, out long totalRecords, out long _MasterTxnID)
        {
            try
            {
                return objPaymentDAL.ListMasterPaymentDetailsForDataEntry(objFilter, out totalRecords,out _MasterTxnID);
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                        throw new Exception("Error while getting payment master list...");
            }
        }


        /// <summary>
        /// BAL function to check if cheque number allready entered
        /// </summary>
        /// <param name="objFilter"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public bool IschequeIssued(string chequeNumber, string operation, long billId)
        {
            try
            {
                return objPaymentDAL.IschequeIssued(chequeNumber, operation, billId);
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                     throw new Exception("Error while checking if cheque is already issued...");
            }
        }




        /// <summary>
        /// function to delete the master payment details
        /// </summary>
        /// <param name="Bill_Id">bill id of the master payement</param>
        /// <returns></returns>
        public int DeleteMasterPaymentDetails(Int64 Bill_Id)
        {
            try
            {
                return objPaymentDAL.DeleteMasterPaymentDetails(Bill_Id);
            }

            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                        throw new Exception("Error while deleting master details...");
            }

        }

        /// <summary>
        /// function to get the master payment details 
        /// </summary>
        /// <param name="Bill_id">bill id of the master payment</param>
        /// <returns></returns>
        public ACC_BILL_MASTER GetMasterPaymentDetails(Int64 Bill_id)
        {
            try
            {
                return objPaymentDAL.GetMasterPaymentDetails(Bill_id);
            }

            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
              throw new Exception("Error while getting  master details...");
            }
        }


        /// <summary>
        /// function to get the deduction details of the transactions
        /// </summary>
        /// <param name="objFilter"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array ListPaymentDeductionDetailsForDataEntry(PaymentFilterModel objFilter, out long totalRecords)
        {
            try
            {
                return objPaymentDAL.ListPaymentDeductionDetailsForDataEntry(objFilter, out totalRecords);
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw new Exception("Error while getting deduction payment list...");
            }
        }

        /// <summary>
        /// BAL function to get the reamining amount 
        /// </summary>
        /// <param name="Bill_id"></param>
        /// <returns></returns>
        public AmountCalculationModel CalculatePaymentAmounts(Int64 Bill_id)
        {
            try
            {
                return objPaymentDAL.CalculatePaymentAmounts(Bill_id);
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw new Exception("Error while getting payment Amounts...");
            }

        }


        //GetVoucherPayment() added by vikaram -28-8-2013 
        public decimal? GetVoucherPayment(Int64 billId, out string paymentType)
        {
            return objPaymentDAL.GetVoucherPayment(billId, out paymentType);
        }

       // public MASTER_CONTRACTOR_BANK GetContratorBankAccNoAndIFSCcode(int contractorId, string fundType, int txnID, ref bool isPFMSFinalized, bool isAdvicePayment, bool isChqPaymentAllowed)
        public List<SelectListItem>  GetContratorBankAccNoAndIFSCcode(int contractorId, string fundType, int txnID, ref bool isPFMSFinalized, bool isAdvicePayment, bool isChqPaymentAllowed)
        {
            return objPaymentDAL.GetContratorBankAccNoAndIFSCcode(contractorId, fundType, txnID, ref isPFMSFinalized, isAdvicePayment, isChqPaymentAllowed);
        }

          


        #endregion

        #region Transaction deduction Payment
        /// <summary>
        /// function to add transaction and deduction details 
        /// </summary>
        /// <param name="model">model to add</param>
        /// <param name="operationType"> deduction or payment</param>
        /// <param name="Bill_id">master bill id  </param>
        /// <returns>result of the process (true /false) </returns>
        public Boolean AddEditTransactionDeductionPaymentDetails(PaymentDetailsModel model, string operationType, Int64 Bill_id, String AddorEdit, Int16 tranNumber)
        {
            try
            {
                return objPaymentDAL.AddEditTransactionDeductionPaymentDetails(model, operationType, Bill_id, AddorEdit, tranNumber);
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
               throw new Exception("Error while adding  payment details...");
            }
        }

        /// <summary>
        /// function to check the asset amount validation
        /// </summary>
        /// <param name="bill_id"></param>
        /// <param name="headId"></param>
        /// <param name="transNumber"></param>
        /// <param name="newTransAmount"></param>
        /// <returns></returns>
        public String CheckForAssetPaymentValidation(long bill_id, int headId, short transNumber, decimal newTransAmount)
        {
            try
            {
                return objPaymentDAL.CheckForAssetPaymentValidation(bill_id, headId, transNumber, newTransAmount);
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw new Exception("Error while getting voucher details...");
            }
        }
        /// <summary>
        /// function to delete the transaction details (payment or deduction)
        /// </summary>
        /// <param name="master_Bill_Id"></param>
        /// <param name="tranNumber"></param>
        /// <param name="paymentDeduction"></param>
        /// <returns> result of the process</returns>
        public Int32 DeleteTransactionPaymentDetails(Int64 master_Bill_Id, Int32 tranNumber, String paymentDeduction)
        {
            try
            {
                return objPaymentDAL.DeleteTransactionPaymentDetails(master_Bill_Id, tranNumber, paymentDeduction);
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
               throw new Exception("Error while deleting  payment details...");
            }
        }

        /// <summary>
        /// function to get the payment transaction details
        /// </summary>
        /// <param name="BILL_ID"></param>
        /// <param name="tranNumber"></param>
        /// <param name="paymentDeduction"></param>
        /// <returns></returns>
        public ACC_BILL_DETAILS GetTransactionPaymentDetails(Int64 BILL_ID, Int16 tranNumber, String paymentDeduction)
        {

            try
            {
                return objPaymentDAL.GetTransactionPaymentDetails(BILL_ID, tranNumber, paymentDeduction);
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
               throw new Exception("Error while Getting transaction payment details...");
            }

        }

        /// <summary>
        /// function to get the agrrement number based on the voucher
        /// </summary>
        /// <param name="bill_id">bill id of the voucher</param>
        /// <returns></returns>
        public String GetAgreemntNumberForVoucher(Int64 bill_id)
        {
            try
            {
                return objPaymentDAL.GetAgreemntNumberForVoucher(bill_id);
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw new Exception("Error while Getting transaction payment details...");
            }

        }

        /// <summary>
        /// BAL method to fianlize the voucher
        /// </summary>
        /// <param name="bill_id"></param>
        /// <param name="Tofinalize"></param>
        /// <returns></returns>
        public Int32 FinalizeVoucher(Int64 bill_id, bool Tofinalize)
        {
            try
            {
                return objPaymentDAL.FinalizeVoucher(bill_id, Tofinalize);
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw new Exception("Error while finalizing payment details...");
            }
        }

        /// <summary>
        /// BAL method to get the is final payment details 
        /// </summary>
        /// <param name="BILL_ID"></param>
        /// <param name="roadID"></param>
        /// <returns></returns>
        public List<SelectListItem> GetFinalPaymentDetails(Int64 BILL_ID, Int32 roadID, Int32 subTxnID)
        {
            try
            {
                return objPaymentDAL.GetFinalPaymentDetails(BILL_ID, roadID,subTxnID);
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                  throw new Exception("Error while getting final payment details...");
            }

        }

        //function to get the epay code
        public String GetEpayNumber(Int16 month, Int16 year, Int16 stateCode, int adminNdCode)
        {
            try
            {
                return objPaymentDAL.GetEpayNumber(month, year, stateCode, adminNdCode);
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw new Exception("Error while getting Epay number...");
            }

        }

        /// <summary>
        /// BAL function to get Eremittance number
        /// </summary>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <param name="stateCode"></param>
        /// <param name="adminNdCode"></param>
        /// <returns></returns>
        public String GetEremittanceNumber(Int16 month, Int16 year, Int16 stateCode, int adminNdCode)
        {

            try
            {
                return objPaymentDAL.GetEremittanceNumber(month, year, stateCode, adminNdCode);
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw new Exception("Error while getting ERemittnace number...");
            }

        }

        /// <summary>
        /// function to get the cheque book series based on the cheque number
        /// </summary>
        /// <param name="admin_nd_code"></param>
        /// <param name="fund_type"></param>
        /// <param name="chequeNumber"></param>
        /// <returns> cheque book series (startleaf-endleaf )</returns>
        public string GetChequeBookSeriesBasedOnCheque(Int32 admin_nd_code, String fund_type, Int32 chequeNumber)
        {
            try
            {
                return objPaymentDAL.GetChequeBookSeriesBasedOnCheque(admin_nd_code, fund_type, chequeNumber);
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw new Exception("Error while getting cheque book series based on the cheque number...");
            }

        }

        /// <summary>
        /// function to get finalized cheques based on cheues series
        /// </summary>
        /// <param name="chequeBookId"></param>
        /// <param name="admin_nd_code"></param>
        /// <param name="fund_type"></param>
        /// <param name="chequeSeries"></param>
        /// <returns></returns>
        public List<SelectListItem> GetAllFinalizedCheques(int chequeBookId, int admin_nd_code, string fund_type, string chequeSeries)
        {
            try
            {
                return objPaymentDAL.GetAllFinalizedCheques(chequeBookId, admin_nd_code, fund_type, chequeSeries);
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw new Exception("Error while getting  finalied cheque  based on the cheque series...");
            }

        }

        /// <summary>
        /// function to get the closing balance for the payment
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        //public UDF_ACC_GEN_GET_BA_CASH_CLOSING_BALANCES_Result GetCloSingBalanceForPayment(TransactionParams param)
        public UDF_ACC_GEN_GET_BA_CASH_CLOSING_BALANCES_COMPOSITE_Result GetCloSingBalanceForPayment(TransactionParams param)
        {
            try
            {
                return objPaymentDAL.GetCloSingBalanceForPayment(param);
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw new Exception("Error while getting payment balance...");
            }

        }



        #endregion

        #region cheque Cancellation


        /// <summary>
        /// function to renew cheque details
        /// </summary>
        /// <param name="bill_Id"></param>
        /// <returns></returns>
        public String RenewCheque(Int64 bill_Id, ChequeRenewModel model)
        {
            try
            {
                return objPaymentDAL.RenewCheque(bill_Id, model);
            }
            catch (Exception Ex)
            {
                ErrorLog.LogError(Ex, "PaymentBAL.RenewCheque");
                //Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
               throw new Exception("Error while Renewing cheque Details...");
            }

        }

        /// <summary>
        /// function to populate the fund type
        /// </summary>
        /// <param name="fundType"></param>
        /// <returns></returns>
        //public List<SelectListItem> populateFundTypeForCancellation(String fundType)
        public List<SelectListItem> populateFundTypeForCancellation(String fundType, int level)
        {
            try
            {
                //return objPaymentDAL.populateFundTypeForCancellation(fundType);
                return objPaymentDAL.populateFundTypeForCancellation(fundType, level);
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
               throw new Exception("Error while getting fund type details for cancellation...");
            }

        }


        /// <summary>
        /// BAL function to cancel the cheque details
        /// </summary>
        /// <param name="bill_Id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public String CancelCheque(Int64 bill_Id, ChequeCancellModel model)
        {
            try
            {
                return objPaymentDAL.CancelCheque(bill_Id, model);
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw new Exception("Error while cancelling cheque Details...");
            }

        }



        /// <summary>
        /// BAL funcion to list the cheque payment details for cancellation based on cheque
        /// </summary>
        /// <param name="objFilter"></param>
        /// <param name="totalRecords"></param>
        /// <param name="bill_id"></param>
        /// <returns></returns>
        public Array ListChequeDetailsForRenewalbyCheque(PaymentFilterModel objFilter, out long totalRecords, out Int64 bill_id)
        {
            try
            {
                return objPaymentDAL.ListChequeDetailsForRenewalbyCheque(objFilter, out totalRecords, out bill_id);
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw new Exception("Error while getting payment master list for cheque cancellation");
            }
        }


        /// <summary>
        /// function to Get the Epayment details for epay order
        /// </summary>
        /// <param name="bill_id"></param>
        /// <returns></returns>
        public EpaymentOrderModel GetEpaymentDetails(Int64 bill_id)
        {
            try
            {
                return objPaymentDAL.GetEpaymentDetails(bill_id);
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
               throw new Exception("Error while getting Epayment details for epayment order");
            }
        }

        /// <summary>
        /// function to Get the Eremitance details for epay order
        /// </summary>
        /// <param name="bill_id"></param>
        /// <returns></returns>
        public EremittnaceOrderModel GetEremittanceDetails(Int64 bill_id)
        {
            try
            {
                return objPaymentDAL.GetEremittanceDetails(bill_id);
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
               throw new Exception("Error while getting Eremittance details for epayment order");
            }
        }

        /// <summary>
        /// function to get the eremitance details after it has been finalized by authorized signatory
        /// </summary>
        /// <param name="bill_id"></param>
        /// <returns></returns>
        public EremittnaceOrderModel GetEremittanceDetailsFinalizedByAuthSig(Int64 bill_id)
        {
            try
            {
                return objPaymentDAL.GetEremittanceDetailsFinalizedByAuthSig(bill_id);
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw new Exception("Error while getting Eremittance details for epayment order");
            }

        }


        /// <summary>
        /// BAL function to  get eayment details which is allready finalized bu auth signatory
        /// </summary>
        /// <param name="bill_id"></param>
        /// <returns></returns>
        public EpaymentOrderModel GetEpaymentDetailsFinalizedByAuthSig(Int64 bill_id)
        {
            try
            {
                return objPaymentDAL.GetEpaymentDetailsFinalizedByAuthSig(bill_id);
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
               throw new Exception("Error while getting Epayment details for epayment order");
            }
        }


        /// <summary>
        /// function to list Epayment Details for authorized signatory
        /// </summary>
        /// <param name="objFilter"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array ListEPaymentDetails(PaymentFilterModel objFilter, String TransactionType, out long totalRecords,string moduleType)
        {
            try
            {
                return objPaymentDAL.ListEPaymentDetails(objFilter, TransactionType, out totalRecords, moduleType);
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                 throw new Exception("Error while getting payment master list...");
            }
        }

        /// <summary>
        /// BAl function to unlock payment
        /// </summary>
        /// <param name="bill_ID"></param>
        /// <returns></returns>
        public string UnlockEpayment(Int64 bill_ID)
        {
            try
            {
                return objPaymentDAL.UnlockEpayment(bill_ID);
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                 throw new Exception("Error while unlocking Epayment...");
            }

        }

        /// <summary>
        /// function to finalize the epayment voucher
        /// </summary>
        /// <param name="bill_ID"></param>
        /// <param name="DblHasedPassword"></param>
        /// <returns></returns>
        public String FinalizeEpayment(Int64 bill_ID, string DblHasedPassword)
        {
            try
            {
                return objPaymentDAL.FinalizeEpayment(bill_ID, DblHasedPassword);
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw new Exception("Error while finalizing Epayment Voucher ...");
            }

        }

        /// <summary>
        /// BAL function to finalize the Eremittances
        /// </summary>
        /// <param name="bill_ID"></param>
        /// <param name="DblHasedPassword"></param>
        /// <returns></returns>
        public String FinalizeEremittance(Int64 bill_ID, string DblHasedPassword)
        {
            try
            {
                return objPaymentDAL.FinalizeEremittance(bill_ID, DblHasedPassword);
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
               throw new Exception("Error while finalizing Eremittance Voucher ...");
            }

        }

        /// <summary>
        ///BAL function to insert the Epayment mail details 
        /// </summary>
        /// <param name="bill_ID"></param>
        /// <returns></returns>
        public string InsertEpaymentMailDetails(Int64 bill_ID, String FileName)
        {

            try
            {
                return objPaymentDAL.InsertEpaymentMailDetails(bill_ID,FileName);
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw new Exception("Error while finalizing Voucher Epayment...");
            }

        }

        /// <summary>
        /// function to insert eremittance details 
        /// </summary>
        /// <param name="bill_ID"></param>
        /// <returns></returns>
        public String InsertEremittanceMailDetails(Int64 bill_ID, String FileName)
        {
            try
            {
                return objPaymentDAL.InsertEremittanceMailDetails(bill_ID,FileName);
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                 throw new Exception("Error while finalizing Voucher Epayment...");
            }

        }

        /// <summary>
        /// DAL method to create the Template of email along with the attachment
        /// </summary>
        /// <returns></returns>
        public virtual MailMessage EpayOrderMail(EpaymentOrderModel epaymodel, string Path, String headerPath,ref String ErrorMessage)
        {
            CommonFunctions commomFuncObj = new CommonFunctions();
            try
            {
                using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog.txt"))
                {
                    sw.WriteLine(" in EpayOrderMail :" + DateTime.Now.ToString() + " UserName -" + PMGSYSession.Current.UserName);
                    sw.Close();
                }

                // Changed by Srishti on 13-03-2023
                /*MvcMailMessage mailMessage;*/
                MailMessage mailMessage;

                if (epaymodel.IsNewResend.Equals("R"))
                {
                    // Changed by Srishti on 13-03-2023
                    //mailMessage = new MvcMailMessage
                    mailMessage = new MailMessage
                    {
                        Subject = "[Duplicate] " +  epaymodel.EmailSubject
                    };
                }
                else
                {
                    // Changed by Srishti on 13-03-2023
                    //mailMessage = new MvcMailMessage
                    mailMessage = new MailMessage
                    {
                        Subject =  epaymodel.EmailSubject
                    };

                }


                
                //Comment below line 
              //  mailMessage.To.Add("anitag@cdac.in");
               // mailMessage.To.Add("ommashelp@gmail.com");                  
                                
                
            
                if (epaymodel.BankEmail == string.Empty || epaymodel.BankEmail == null || epaymodel.BankEmail == "")
                {
                    ErrorMessage = "because Bank Email address ( To ) is not present.";
                    throw new Exception();
                }
                else
                {
                  
                     mailMessage.To.Add(epaymodel.BankEmail);
                    //Comment below line  and uncomment above line 
                  //  mailMessage.To.Add("jyotiz@cdac.in");
                }

                if (epaymodel.EmailCC != "")
                {
                 
                     mailMessage.CC.Add(epaymodel.EmailCC);
                    //Comment below line  and uncomment above line 
                  //  mailMessage.CC.Add("anitarghule@rediffmail.com");
                }

                
                mailMessage.Bcc.Add(epaymodel.EmailBCC);
                //Comment below line  and uncomment above line 
             //  mailMessage.Bcc.Add("anitag@cdac.in");
                
                
                mailMessage.Attachments.Add(new Attachment(Path));
                // Use a strongly typed model
                ViewData = new ViewDataDictionary(epaymodel);
                var resources = new Dictionary<string, string>();
                resources["logo"] = headerPath;
                PopulateBody(mailMessage, "SignEpayment", resources);

                using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog.txt"))
                {
                    sw.WriteLine(" out of EpayOrderMail :" + DateTime.Now.ToString() + " UserName -" + PMGSYSession.Current.UserName);
                    sw.Close();
                }
                return mailMessage;
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);                
                throw new Exception("Error while Finalizing Epayment.");
            }
            finally
            {

            }

        }


        /// <summary>
        /// DAl method to send the mail for the eremittances
        /// </summary>
        /// <param name="bill_id"></param>
        /// <param name="epaymodel"></param>
        /// <param name="Path"></param>
        /// <returns></returns>
        public MailMessage SendMailForEremOrder(EremittnaceOrderModel epaymodel, string Path, String headerPath, ref String ErrorMessage)
        {
            CommonFunctions commomFuncObj = new CommonFunctions();
            try
            {

                //var mailMessage = new MvcMailMessage
                //{
                //    Subject = epaymodel.EmailSubject
                //};

                // Changed by Srishti on 13-03-2023
                //MvcMailMessage mailMessage;
                MailMessage mailMessage;

                if (epaymodel.IsNewResend.Equals("R"))
                {
                    // Changed by Srishti on 13-03-2023
                    //mailMessage = new MvcMailMessage
                    mailMessage = new MailMessage
                    {
                        Subject = "[Duplicate] " + epaymodel.EmailSubject
                    };
                }
                else
                {
                    // Changed by Srishti on 13-03-2023
                    //mailMessage = new MvcMailMessage
                    mailMessage = new MailMessage
                    {
                        Subject = epaymodel.EmailSubject
                    };

                }

                //uncomment below 2 while deploying live application 

                if (epaymodel.BankEmail == string.Empty || epaymodel.BankEmail == null)
                {
                    ErrorMessage = "because Bank Email address ( To ) is not present.";
                    throw new Exception();
                }
                else
                {
                    mailMessage.To.Add(epaymodel.BankEmail);
                    //Uncomment Above line and comment below line 
                   //  mailMessage.To.Add("jyotiz@cdac.in");
                   //  mailMessage.To.Add("ommashelp@gmail.com");
                }
                if (epaymodel.EmailCC != "" || epaymodel.EmailCC == null)
                {

                    mailMessage.CC.Add(epaymodel.EmailCC);
                    //Comment below line  and uncomment above line 
                    //  mailMessage.CC.Add("anitarghule@rediffmail.com");
                }


               // mailMessage.Bcc.Add("omms.pmgsy@nic.in");
                //uncomment above line
                mailMessage.Attachments.Add(new Attachment(Path));

                // Use a strongly typed model
                ViewData = new ViewDataDictionary(epaymodel);
                var resources = new Dictionary<string, string>();
                resources["logo"] = headerPath;

                PopulateBody(mailMessage, "EremittanceOrder", resources);
                return mailMessage;

            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw new Exception("Error while Finalizing Eremittances");
            }
            finally
            {

            }
        }


        /// <summary>
        /// function to delete the email details based on the  epayment eremittance details
        /// </summary>
        /// <param name="billId"></param>
        /// <returns></returns>
        public String DeleteMailDetails(long billId)
        {
            try
            {
                return objPaymentDAL.DeleteMailDetails(billId);

            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw new Exception("Error while deleting email details");
            }
            finally
            {

            }

        }


        #region vikky
        public Array GetTransferDeductionAmtListBAL(PaymentFilterModel objFilter, String TransactionType, out long totalRecords, out List<string> voucherGeneratedList)
        {
            try
            {
                return objPaymentDAL.GetTransferDeductionAmtListDAL(objFilter, TransactionType, out totalRecords, out voucherGeneratedList);
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw new Exception("Error while getting payment master list...");
            }
        }

        public Array GetTransferDeductionAmtGeneratedVoucherListBAL(PaymentFilterModel objFilter, String TransactionType, out long totalRecords)
        {
            try
            {
                return objPaymentDAL.GetTransferDeductionAmtGeneratedVoucherListDAL(objFilter, TransactionType, out totalRecords);
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw new Exception("Error while getting payment master list...");
            }
        }


        public Boolean SubmitTransferDeductionAmountBAL(string[] billIdArray, string billNo, DateTime billDate, string chq_No, decimal deductionAmt)
        {
            try
            {
                return objPaymentDAL.SubmitTransferDeductionAmountDAL(billIdArray, billNo, billDate, chq_No, deductionAmt);
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw new Exception("Error while transfering amount...");
            }
        }


        public Array ListSecondLevelSuccessEPaymentDetails(PaymentFilterModel objFilter, String TransactionType, out long totalRecords, string moduleType)
        {
            try
            {
                return objPaymentDAL.ListSecondLevelSuccessEPaymentDetails(objFilter, TransactionType, out totalRecords, moduleType);
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw new Exception("Error while getting payment master list...");
            }
        }



        #endregion


        #endregion

        #region Reject / Resend Epayment

        public Array GetEpaymentRejectResendList(PaymentFilterModel objFilter, string TransactionType, out long totalRecords)
        {
            objPaymentDAL = new PaymentDAL();
            return objPaymentDAL.GetEpaymentRejectResendList(objFilter, TransactionType, out totalRecords);
        }

        public bool InsertResendEpaymentDetails(RejectResendFormModel model, long Bill_ID)
        {
            objPaymentDAL = new PaymentDAL();
            return objPaymentDAL.InsertResendEpaymentDetails(model, Bill_ID);
        }

        public bool CancelEpaymentEremDetails(RejectResendFormModel model, long Bill_ID,ref string message)
        {
            objPaymentDAL = new PaymentDAL();
            return objPaymentDAL.CancelEpaymentEremDetails(model, Bill_ID,ref message);        
        }

        #endregion Reject / Resend Epayment

        //new method added by Vikram on 29-08-2013
        public bool ValidateDPIUEpaymentBAL(int adminCode)
        {
            objPaymentDAL = new PaymentDAL();
            return objPaymentDAL.ValidateDPIUEpaymentDAL(adminCode);
        }

        public bool ValidateDPIUEremittenceBAL(int adminCode)
        {
            objPaymentDAL = new PaymentDAL();
            return objPaymentDAL.ValidateDPIUERemittenceDAL(adminCode);
        }

        public bool ValidateContractorStatus(int conId , int txnId)
        {
            objPaymentDAL = new PaymentDAL();
            return objPaymentDAL.ValidateContractorStatus(conId, txnId);
        }

        public bool ValidateAdviceNoExist(String ChqNo, long Bill_ID)
        {
            objPaymentDAL = new PaymentDAL();
            return objPaymentDAL.ValidateAdviceNoExist(ChqNo,Bill_ID);
        }

        public bool ValidateifMonthAcknowledged(int? srrdaCode, int dpiuCode, string fundType, Int16 BillMonth, Int16 BillYear)
        {
            objPaymentDAL = new PaymentDAL();
            return objPaymentDAL.ValidateifMonthAcknowledged( srrdaCode, dpiuCode,  fundType,  BillMonth,  BillYear);
        }

        //added by hrishikesh start--
        public Int64 AddSecurityDepositAccOpeningBalanceBAL(SecurityDepositAccOpeningBalanceEntryModel securitydepositaccopeningbalanceentry)
        {
            objPaymentDAL = new PaymentDAL();
            return objPaymentDAL.AddSecurityDepositAccOpeningBalanceDAL(securitydepositaccopeningbalanceentry);
        }

        public Array GetSecurityDepositAccOpeningBalanceJsonBAL(int page, int rows, string sidx, string sord, string filters, int month, int year, out long totalRecords)
        {
            objPaymentDAL = new PaymentDAL();
            return objPaymentDAL.GetSecurityDepositAccOpeningBalanceJsonDAL(page, rows, sidx, sord, filters, month, year, out totalRecords);
        }
        //added by hrishikesh end
    }

    interface IPaymentBAL
    {

        #region master Payment
        String GetChequeBookIssueDate(Int64 chqBookId);
        Int64 AddEditMasterPaymentDetails(PaymentMasterModel model, string operationType, Int64 Bill_id,bool IsAdvicePayment);
        List<SelectListItem> GetchequebookSeries(int admin_ND_Code, String fundType, Int16 levelID);
        //List<SelectListItem> GetchequebookNumbers(int chequeBookId, int admin_nd_code,string fund_type);
        Array ListMasterPaymentDetails(PaymentFilterModel objFilter, out long totalRecords);
        int DeleteMasterPaymentDetails(Int64 Bill_Id);
        Array ListMasterPaymentDetailsForDataEntry(PaymentFilterModel objFilter, out long totalRecords ,out long _MasterTxnID);
        ACC_BILL_MASTER GetMasterPaymentDetails(Int64 Bill_id);
        AmountCalculationModel CalculatePaymentAmounts(Int64 Bill_id);
        String GetEpayNumber(Int16 month, Int16 year, Int16 stateCode, int adminNdCode);
        String GetEremittanceNumber(Int16 month, Int16 year, Int16 stateCode, int adminNdCode);
        bool IschequeIssued(string chequeNumber, string operation, long billId);

        //UDF_ACC_GEN_GET_BA_CASH_CLOSING_BALANCES_Result GetCloSingBalanceForPayment(TransactionParams param);
        UDF_ACC_GEN_GET_BA_CASH_CLOSING_BALANCES_COMPOSITE_Result GetCloSingBalanceForPayment(TransactionParams param);
        Array ListPaymentDeductionDetailsForDataEntry(PaymentFilterModel objFilter, out long totalRecords);
        String CheckForAssetPaymentValidation(long bill_id, int headId, short transNumber, decimal newTransAmount);
        decimal? GetVoucherPayment(Int64 billId, out string paymentType);
      //  MASTER_CONTRACTOR_BANK GetContratorBankAccNoAndIFSCcode(int contractorId , string fundType , int txnID, ref bool isPFMSFinalized, bool isAdvicePayment, bool isChqPaymentAllowed);
        List<SelectListItem>  GetContratorBankAccNoAndIFSCcode(int contractorId, string fundType, int txnID, ref bool isPFMSFinalized, bool isAdvicePayment, bool isChqPaymentAllowed);
        #endregion

        #region Transaction deduction Payment
        Boolean AddEditTransactionDeductionPaymentDetails(PaymentDetailsModel model, string operationType, Int64 Bill_id, String AddorEdit, Int16 tranNumber);
        Int32 DeleteTransactionPaymentDetails(Int64 master_Bill_Id, Int32 tranNumber, String paymentDeduction);
        ACC_BILL_DETAILS GetTransactionPaymentDetails(Int64 BILL_ID, Int16 tranNumber, String paymentDeduction);
        String GetAgreemntNumberForVoucher(Int64 bill_id);
        Int32 FinalizeVoucher(Int64 bill_id, bool Tofinalize);
        List<SelectListItem> GetFinalPaymentDetails(Int64 BILL_ID, Int32 roadID, Int32 subTxnID);
        String[] GetAllAvailableChequesArray(int chequeBookId, int admin_nd_code, string fund_type, string operation = "A", Int64 billID = 0);
        string GetChequeBookSeriesBasedOnCheque(Int32 admin_nd_code, String fund_type, Int32 chequeNumber);
        List<SelectListItem> GetAllFinalizedCheques(int chequeBookId, int admin_nd_code, string fund_type, string chequeSeries);
        #endregion

        #region Cheque Renwal & cancellation

        String RenewCheque(Int64 bill_Id, ChequeRenewModel model);
        Array ListChequeDetailsForRenewalbyCheque(PaymentFilterModel objFilter, out long totalRecords, out Int64 bill_id);
        String CancelCheque(Int64 bill_Id, ChequeCancellModel model);
        //List<SelectListItem> populateFundTypeForCancellation(String fundType);
        List<SelectListItem> populateFundTypeForCancellation(String fundType, int level);
        #endregion

        #region EPAYMENT

        EpaymentOrderModel GetEpaymentDetails(Int64 bill_id);
        EremittnaceOrderModel GetEremittanceDetails(Int64 bill_id);
        EremittnaceOrderModel GetEremittanceDetailsFinalizedByAuthSig(Int64 bill_id);
        EpaymentOrderModel GetEpaymentDetailsFinalizedByAuthSig(Int64 bill_id);
        Array ListEPaymentDetails(PaymentFilterModel objFilter, string TransactionType, out long totalRecords , string moduleType);

        #region vikky
        Array GetTransferDeductionAmtListBAL(PaymentFilterModel objFilter, string TransactionType, out long totalRecords, out List<string> voucherGeneratedList);

        Array GetTransferDeductionAmtGeneratedVoucherListBAL(PaymentFilterModel objFilter, string TransactionType, out long totalRecords);

        Boolean SubmitTransferDeductionAmountBAL(string[] billIdArray, string billNo, DateTime billDate, string chq_No, decimal deductionAmt);

        Array ListSecondLevelSuccessEPaymentDetails(PaymentFilterModel objFilter, string TransactionType, out long totalRecords, string moduleType);

        #endregion
        String UnlockEpayment(Int64 bill_ID);
        String FinalizeEpayment(Int64 bill_ID, string DblHasedPassword);
        String FinalizeEremittance(Int64 bill_ID, string DblHasedPassword);
        String InsertEpaymentMailDetails(Int64 bill_ID,String FileName);
        String InsertEremittanceMailDetails(Int64 bill_ID, String FileName);
        MailMessage EpayOrderMail(EpaymentOrderModel epaymodel, string Path, String headerPath,ref String ErrorMessage);
        MailMessage SendMailForEremOrder(EremittnaceOrderModel epaymodel, string Path, String headerPath, ref String ErrorMessage);
        String DeleteMailDetails(long billId);





        #endregion



        #region Reject / Resend Epayment

        Array GetEpaymentRejectResendList(PaymentFilterModel objFilter, string TransactionType, out long totalRecords);
        bool InsertResendEpaymentDetails(RejectResendFormModel model, long Bill_ID);
        bool CancelEpaymentEremDetails(RejectResendFormModel model, long Bill_ID,ref string message);

        #endregion Reject / Resend Epayment

        //new method added by Vikram on 29-08-2013
        bool ValidateDPIUEpaymentBAL(int adminCode);
        bool ValidateDPIUEremittenceBAL(int adminCode);
        bool ValidateContractorStatus(int conId , int txnId) ;
        
        //Added By Abhishek kamble for Advice No 6Apr2015 start
        bool ValidateAdviceNoExist(String ChqNo, long Bill_ID);

        bool ValidateifMonthAcknowledged(int? srrdaCode, int dpiuCode, string fundType, Int16 BillMonth, Int16 BillYear);

        //added by hrishikesh start
        Int64 AddSecurityDepositAccOpeningBalanceBAL(SecurityDepositAccOpeningBalanceEntryModel securitydepositaccopeningbalanceentry);

        Array GetSecurityDepositAccOpeningBalanceJsonBAL(int page, int rows, string sidx, string sord, string filters, int month, int year, out long totalRecords);

        //added by hrishikesh start--end
    }
}
