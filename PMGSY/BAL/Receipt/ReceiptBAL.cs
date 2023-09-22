using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.DAL.Receipt;
using PMGSY.Models.Receipts;
using System.Web.Mvc;

namespace PMGSY.BAL.Receipt
{
    public class ReceiptBAL : IReceiptBAL
    {
        IReceiptDAL objDAL = new ReceiptDAL();
        public Array ReceiptList(ReceiptFilterModel objFilter, out long totalRecords)
        {
            return objDAL.ReceiptList(objFilter, out totalRecords);
        }

        public Array ReceiptMasterList(ReceiptFilterModel objFilter, out long totalRecords)
        {
            return objDAL.ReceiptMasterList(objFilter, out totalRecords);
        }

        public Array ReceiptDetailsList(ReceiptFilterModel objFilter, out long totalRecords, out decimal totalAmount)
        {
            return objDAL.ReceiptDetailsList(objFilter, out totalRecords, out totalAmount);
        }

        public String AddReceiptDetails(BillDetailsViewModel billDetailsViewModel)
        {
            return objDAL.AddReceiptDetails(billDetailsViewModel);
        }

        public String EditReceiptDetails(BillDetailsViewModel billDetailsViewModel)
        {
            return objDAL.EditReceiptDetails(billDetailsViewModel);
        }

        public BillDetailsViewModel GetReceiptDetailByTransNo(Int64 billId, Int16 TransNo)
        {
            return objDAL.GetReceiptDetailByTransNo(billId, TransNo);
        }

        public String DeleteReceiptDetails(Int64 billId, Int16 TransNo)
        {
            return objDAL.DeleteReceiptDetails(billId, TransNo);
        }

        public String FinalizeReceipt(Int64 billId)
        {
            return objDAL.FinalizeReceipt(billId);
        }

        public String DeleteReceipt(Int64 billId)
        {
            return objDAL.DeleteReceipt(billId);
        }

        public Decimal GetAmountAvailable(Int64 billId, Int16 txnId)
        {
            return objDAL.GetAmountAvailable(billId, txnId);
        }

        public String ValidateAddReceiptDetails(BillDetailsViewModel billDetailsViewModel, string Op_mode)
        {
            return objDAL.ValidateAddReceiptDetails(billDetailsViewModel, Op_mode);
        }

        public String ValidateEditReceiptMaster(BillMasterViewModel billMasterViewModel)
        {
            return objDAL.ValidateEditReceiptMaster(billMasterViewModel);
        }

        public List<SelectListItem> GetUnSettledVouchers(Int16 month, Int16 year, Int64 pBillId)
        {
            return objDAL.GetUnSettledVouchers(month, year, pBillId);
        }

        public String GetImprestAmount(Int64 billId, Int16 TxnId)
        {
            return objDAL.GetImprestAmount(billId, TxnId);
        }
    }

    public interface IReceiptBAL
    {
        Array ReceiptList(ReceiptFilterModel objFilter, out long totalRecords);
        Array ReceiptMasterList(ReceiptFilterModel objFilter, out long totalRecords);
        Array ReceiptDetailsList(ReceiptFilterModel objFilter, out long totalRecords, out decimal totalAmount);
        String AddReceiptDetails(BillDetailsViewModel billDetailsViewModel);
        String EditReceiptDetails(BillDetailsViewModel billDetailsViewModel);
        BillDetailsViewModel GetReceiptDetailByTransNo(Int64 billId, Int16 TransNo);
        String DeleteReceiptDetails(Int64 billId, Int16 TransNo);
        String FinalizeReceipt(Int64 billId);
        String DeleteReceipt(Int64 billId);
        Decimal GetAmountAvailable(Int64 billId, Int16 txnId);
        String ValidateAddReceiptDetails(BillDetailsViewModel billDetailsViewModel, string Op_mode);
        String ValidateEditReceiptMaster(BillMasterViewModel billMasterViewModel);
        List<SelectListItem> GetUnSettledVouchers(Int16 month, Int16 year, Int64 pBillId);
        String GetImprestAmount(Int64 billId, Int16 TxnId);
    }
}