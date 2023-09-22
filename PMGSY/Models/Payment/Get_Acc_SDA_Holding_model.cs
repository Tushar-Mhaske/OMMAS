using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Models.Payment
{
    public class Get_Acc_SDA_Holding_model
    {
        public int TXN_ID { get; set; }
        public long BILL_ID { get; set; }
        public string BILL_NO { get; set; }
        public string BILL_DATE { get; set; }
        public string TXN_DESC { get; set; }
        public string CHQ_NO { get; set; }
        public string CHQ_AMOUNT { get; set; }
        public string CASH_AMOUNT { get; set; }
        public string GROSS_AMOUNT { get; set; }
        public string BANK_ACK_BILL_STATUS { get; set; }

        public string holdingStatus { get; set; }
    }
}