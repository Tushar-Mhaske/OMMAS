using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Models.Report
{
    public class SingleCBCollection
    {
        public List<SP_ACC_CASHBOOK_RECEIPT_SIDE_Result> ListReceiptCB { get; set; }
        public List<SP_ACC_CASHBOOK_PAYMENT_SIDE_Result> ListPaymentCB { get; set; } 
    }
}