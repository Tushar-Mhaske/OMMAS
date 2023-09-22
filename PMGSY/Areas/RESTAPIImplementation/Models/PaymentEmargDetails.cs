using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.RESTAPIImplementation.Models
{
    public class PaymentEmargDetails
    {
        public int BILL_ID { get; set; }
        public Nullable<int> BILL_ID_PREVIOUS { get; set; }
        public string VOUCHER_NO { get; set; }
        public int BILL_MONTH { get; set; }
        public int BILL_YEAR { get; set; }
        public System.DateTime BILL_DATE { get; set; }
        public string CHQ_NO { get; set; }
        public System.DateTime CHQ_DATE { get; set; }
        public decimal CHQ_AMOUNT { get; set; }
        public decimal CASH_AMOUNT { get; set; }
        public decimal GROSS_AMOUNT { get; set; }
        public int PIU_CODE { get; set; }
        public string MAST_CON_ID { get; set; }
        //public int MAST_CON_ID { get; set; }
        public string NARRATION { get; set; }
        public int ROAD_CODE { get; set; }
        public string AGREEMENT_CODE { get; set; }
        public string FUND_TYPE { get; set; }
        public string PAYEE_NAME { get; set; }
        public string ACCOUNT_NUMBER { get; set; }
        public string IFSC_CODE { get; set; }
        public string HEAD_CODE { get; set; }
        public decimal AMOUNT { get; set; }
        public int TXN_NO { get; set; }
        public string HEAD_CODE_DEDUCTIONS { get; set; }
        public decimal AMOUNT_DEDUCTIONS { get; set; }
        public string EMARG_VOUCHER_NO { get; set; }
        public string OMMAS_PACKAGE_NO { get; set; }
        public string EMARG_PACKAGE_NO { get; set; }
        public System.DateTime EMARG_DATE_TIME { get; set; }
    }



    public class EmargPaymentMasterViewModel
    {
        public int EMARG_VOUCHER_NO { get; set; }
        public Nullable<long> OMMAS_BILL_ID { get; set; }
    }
}