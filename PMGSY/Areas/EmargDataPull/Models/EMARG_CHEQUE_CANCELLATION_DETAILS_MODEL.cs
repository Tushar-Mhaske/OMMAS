using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EmargDataPull.Models
{
    public class EMARG_CHEQUE_CANCELLATION_DETAILS_MODEL
    {
        public long BILL_ID { get; set; }
        public string EMARG_VOUCHER_NO { get; set; }
        public Nullable<int> CANCELLATION_MONTH { get; set; }
        public Nullable<int> CANCELLATION_YEAR { get; set; }
        public Nullable<System.DateTime> CANCELLATION_DATE { get; set; }
        public decimal CHEQUE_AMOUNT { get; set; }
        public string CHEQUE_NO { get; set; }
        public int PIU_CODE { get; set; }
        public string CANCELLATION_REMARKS { get; set; }
        public int ROAD_CODE { get; set; }
        public string SCROLL_GENERATION_TYPE { get; set; }
    }
}