#region File Header
/* 
     *  Name : ChequeAcknowledgementBAL.cs
     *  Path : ~BAL/ChequeAcknowledgement\ChequeAcknowledgementBAL.cs
     *  Description : ChequeAcknowledgementBAL.cs is BAL file for Cheuque Acknowledgment module.

                      
     *  Functions / Procedures Called : 
            
               
                                               

     * classes used 
        
        * ChequeAcknowledgementBAL
                                                   
 
     *  Author : Amol Jadhav (PE, e-gov)
     *  Company : C-DAC,E-GOV
     *  Dates of creation : 16/06/2013  
    
*/
#endregion


using Elmah;
using PMGSY.DAL.ChequeAcknowledgement;
using PMGSY.Models;
using PMGSY.Models.ChequeAcknowledgement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.BAL.ChequeAcknowledgement
{
    public class ChequeAcknowledgementBAL:IChequeAcknowledgementBAL
    {


        IChequeAcknowledgementDAL objChqAckDAL = null;

        public ChequeAcknowledgementBAL()
        {
              objChqAckDAL = new ChequeAcknowledgementDAL();
        }


        /// <summary>
        /// BAL method to list the chequ details for acknowledment
        /// </summary>
        /// <param name="objFilter"> filter options</param>
        /// <param name="totalRecords"> number of records of returned</param>
        /// <returns>list of cheques to acknowledge</returns>
        public Array ListChequeForAcknowledgment(PaymentFilterModel objFilter, out long totalRecords, out  List<String> SelectedIdList,ref string SrrdaBillId)
        {
            try
            {
                return objChqAckDAL.ListChequeForAcknowledgment(objFilter, out totalRecords, out   SelectedIdList,ref SrrdaBillId);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);          
                throw new Exception("Error while getting Cheque details for acknowledgement...");
            }
        }

       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="array_bill_id"></param>
        /// <param name="finalize"></param>
        /// <returns></returns>
        public String  AcknowledgeCheues(CheckAckModel model,long[] array_bill_id,bool finalize)
        {
            try
            {
                return objChqAckDAL.AcknowledgeCheues(model,array_bill_id,finalize);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw new Exception("Error while Cheque  acknowledgement...");
            }
    
        }
       
        /// <summary>
        /// BAL method to get ack details
        /// </summary>
        /// <param name="bill_Id"> bill id of the acknowledgement entry</param>
        /// <returns>bill number and date </returns>
        public String  GetAcknowledgedCheuesDetails(long bill_Id)
        {
            try
            {
                return objChqAckDAL.GetAcknowledgedCheuesDetails(bill_Id);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);    
                throw new Exception("Error while getting cheque acknowledgement details");
            }
    
        }

        public Array ListVoucherDetailsBAL(int? page, int? rows, string sidx, string sord, out long totalRecords, CheckAckSelectionModel model)
        {
            try
            {
                return objChqAckDAL.ListVoucherDetailsDAL(page, rows, sidx,sord,out totalRecords,model);
            }
            catch (Exception ex)
            {
                //ErrorLog.LogError(ex, "ChequeAcknowledgementBAL.ListVoucherDetails()");
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);         
                throw new Exception("Error while getting cheque acknowledgement details");
            }
        }

        public CheckAckSelectionModel GetSelectionDetails(int billId)
        {
            try
            {
                return objChqAckDAL.GetSelectionDetails(billId);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);        
                throw new Exception("Error while getting cheque acknowledgement details");
            }
        }

        public String ValidatePreviousCheques(CheckAckSelectionModel model,ref string message)
        {
            try
            {
                return objChqAckDAL.ValidatePreviousCheques(model, ref message);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);            
                throw new Exception("Error while getting cheque acknowledgement details");
            }
        }
        
        //Added By Abhishek kamble to delete cheque Ack Details.
        public bool DeleteChequeAckVocherDetails(int BIllMonth,int BIllYear,int AdminNdCode,Int64 BillID,ref string message)
        {
            try
            {
                return objChqAckDAL.DeleteChequeAckVocherDetails(BIllMonth,BIllYear,AdminNdCode,BillID,ref message);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
        
        }

    }
  
     interface IChequeAcknowledgementBAL
    {
         Array ListChequeForAcknowledgment(PaymentFilterModel objFilter, out long totalRecords, out  List<String> SelectedIdList,ref string SrrdaBillId);
         String AcknowledgeCheues(CheckAckModel model,long[] array_bill_id,bool finalize);
         String GetAcknowledgedCheuesDetails(long bill_Id);
         Array ListVoucherDetailsBAL(int? page, int? rows, string sidx, string sord, out long totalRecords, CheckAckSelectionModel model);
         CheckAckSelectionModel GetSelectionDetails(int billId);
         String ValidatePreviousCheques(CheckAckSelectionModel model,ref string message);
         bool DeleteChequeAckVocherDetails(int BIllMonth, int BIllYear, int AdminNdCode, Int64 BillID, ref string message);
    }
}