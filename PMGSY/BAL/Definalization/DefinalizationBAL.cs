using PMGSY.Common;
using PMGSY.DAL.Definalization;
using PMGSY.Models.Definalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.BAL.Definalization
{
    public class DefinalizationBAL:IDefinalizationBAL
    {
        IDefinalizationDAL objDefinalizationDAL = null;
        
        public DefinalizationBAL()
        {
            objDefinalizationDAL = new DefinalizationDAL();
        }

        /// <summary>
        /// BAL function to get the voucher list to definalize/delete
        /// </summary>
        /// <param name="objFilter"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
         public Array ListVoucherListtDetails(VoucherFilterModel objFilter, out long totalRecords)
         {
             try
             {
                 return objDefinalizationDAL.ListVoucherListtDetails(objFilter, out totalRecords);
             }
             catch (Exception ex)
             {
                 Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);          
                  throw new Exception("Error while getting Voucher list...");
             }
         }

        /// <summary>
        /// voucher transaction details
        /// </summary>
        /// <param name="objFilter"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
         public Array ListVoucherTransactionDetails(VoucherFilterModel objFilter, out long totalRecords)
         {
             try
             {
                 return objDefinalizationDAL.ListVoucherTransactionDetails(objFilter, out totalRecords);
             }
             catch (Exception ex)
             {
                 Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);          
                 throw new Exception("Error while getting Voucher transation details...");
             }
         }

        

        /// <summary>
        /// BAL function to definalize the voucher
        /// </summary>
        /// <param name="bill_id"></param>
        /// <returns>status of operation </returns>
         public String DefinalizeVoucher(long bill_id)
         {
             try
             {
                 return objDefinalizationDAL.DefinalizeVoucher(bill_id);
             }
             catch (Exception ex)
             {
                 Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);          
                 throw new Exception("Error while definalizing the voucher...");
             }
         }

        /// <summary>
        /// BAL function to delete the voucher
        /// </summary>
        /// <param name="bill_id"></param>
        /// <returns></returns>
        public String DeleteVoucher(long bill_id)
         {
             try
             {
                 return objDefinalizationDAL.DeleteVoucher(bill_id);
             }
             catch (Exception ex)
             {
                 //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);          
                 ErrorLog.LogError(ex, "DefinalizationBAL.DeleteVoucher()");
                 throw new Exception("Error while  deleting the voucher...");
             }
         }

        /// <summary>
        /// function to check if asset payment
        /// </summary>
        /// <param name="bill_id"></param>
        /// <returns></returns>
       public String CheckIfAssetPayment(long bill_id)
        {
            try
            {
                return objDefinalizationDAL.CheckIfAssetPayment(bill_id);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);          
               throw new Exception("Error while definalizing/deleting  the voucher...");
            }
        }

    }


    interface IDefinalizationBAL
    {
        Array ListVoucherListtDetails(VoucherFilterModel objFilter, out long totalRecords);
        String DefinalizeVoucher(long bill_id);
        String DeleteVoucher(long bill_id);
        Array ListVoucherTransactionDetails(VoucherFilterModel objFilter, out long totalRecords);
        String CheckIfAssetPayment(long bill_id);
    }
}