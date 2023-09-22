using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Models.Common
{
    public class TransactionParams
    {
        public String BILL_TYPE { get; set; }
        public String FUND_TYPE { get; set; }
        public Int16 LVL_ID { get; set; }
        public Int16 TXN_ID { get; set; }
        public String CASH_CHQ { get; set; }
        public Int32 ADMIN_ND_CODE { get; set; }
        public Int32 MAST_CONT_ID { get; set; }
        public String MAST_CON_SUP_FLAG { get; set; }
        public Int32 STATE_CODE { get; set; }
        public Int32 DISTRICT_CODE { get; set; }
        public Int64 BILL_ID { get; set; }
        public Int32 AGREEMENT_CODE { get; set; }
        public String PACKAGE_ID { get; set; }
        public String CREDIT_DEBIT { get; set; }
        public Int16 HEAD_ID { get; set; }
        public Int16 SANC_YEAR { get; set; }
        public String BILL_NO { get; set; } // To find type of Opening Balance Payment
        public Int16 TXN_NO { get; set; }
        public String OP_MODE { get; set; }
        public Int32 BlockCode { get; set; }
        public Boolean ISSearch { get; set; }
        public Int16 MONTH { get; set; }
        public Int16 YEAR { get; set; }
        public Int32 ROAD_CODE { get; set; }
        public String AGREEMENT_NUMBER { get; set; }
    }
}