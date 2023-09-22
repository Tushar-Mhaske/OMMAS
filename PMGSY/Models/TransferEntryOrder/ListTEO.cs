using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Models.TransferEntryOrder
{
    public class ListTEO
    {
        public long BILL_ID { get; set; }
        public string BILL_NO { get; set; }
        public short BILL_MONTH { get; set; }
        public short BILL_YEAR { get; set; }
        public System.DateTime BILL_DATE { get; set; }
        public string TXN_DESC { get; set; }
        public decimal GROSS_AMOUNT { get; set; }
        public decimal ? CREDIT_AMOUNT { get; set; }
        public decimal? DEBIT_AMOUNT { get; set; }
        public string BILL_FINALIZED { get; set; }
        public string ACTION_REQUIRED { get; set; }
        public short TXN_ID { get; set; }
        public int ADMIN_ND_CODE { get; set; }
    }
}