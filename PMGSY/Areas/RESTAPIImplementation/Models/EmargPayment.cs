using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.RESTAPIImplementation.Models
{
    public class EmargPayment
    {
        public long BILL_ID { get; set; }
        public string EMARG_VOUCHER_NO { get; set; }
        public Nullable<System.DateTime> MESSAGE_DATE_TIME { get; set; }
        public string STATUS { get; set; }
        public string RJCT_CODE { get; set; }
        public string REMARKS { get; set; }

        public int ROAD_CODE { get; set; }
    public string VOUCHER_TYPE { get; set; }



        //public int BILL_MONTH { get; set; }
        //public int BILL_YEAR { get; set; }
        //public System.DateTime BILL_DATE { get; set; }
        //public decimal CHQ_AMOUNT { get; set; }
        //public string AGREEMENT_CODE { get; set; }
        //public string PAYEE_NAME { get; set; }
        //public int PIU_CODE { get; set; }
        //public string MAST_CON_ID { get; set; }
        //public int ROAD_CODE { get; set; }
        //public string ACCOUNT_NUMBER { get; set; }
        //public string IFSC_CODE { get; set; }
    }
}