using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PMGSY.Models.Bank
{
    public class BankReconciliationModel
    {
          
        public long BILL_ID { get; set; }

        [Range(1, 12, ErrorMessage = "Please select month.")] 
        public short BILL_MONTH { get; set; }

        [Range(2000, Int16.MaxValue, ErrorMessage = "Please select year.")]
        public short BILL_YEAR { get; set; }

        public System.DateTime BILL_DATE { get; set; }
        public string CHQ_NO { get; set; }
        public Nullable<System.DateTime> CHQ_DATE { get; set; }
        public decimal CHQ_AMOUNT { get; set; }
        public string PAYEE_NAME { get; set; }
        public string CHQ_EPAY { get; set; }
        public string BILL_FINALIZED { get; set; }
        public string FUND_TYPE { get; set; }

        [Range(1, Int32.MaxValue, ErrorMessage = "Please select DPIU.")]
        public int ADMIN_ND_CODE { get; set; }

        [Range(0, Int32.MaxValue, ErrorMessage = "Selected DPIU is Invalid.")]
        public int DateWiseADMIN_ND_CODE { get; set; }
        
        public byte LVL_ID { get; set; }
        public string BILL_TYPE { get; set; }
        public bool IS_CHQ_RECONCILE_BANK { get; set; }
        public string IS_CHQ_RECONCILE { get; set; }
        public Nullable<System.DateTime> CHQ_RECONCILE_DATE { get; set; }
        public string CHQ_RECONCILE_REMARKS { get; set; }
        public String ENC_BILL_ID { get; set; }
        public string CHQEPAY_DATE { get; set; }
        public string CHQEPAY_RECONCILE_DATE { get; set; }

        public string SAS_REQUEST_IP { get; set; }
        public string SAS_LOGIN_NAME { get; set; }
        public Int16 BANK_CODE { get; set; }

        //Added By Abhishek to disp epay voucher popup 27-June-2014
        public Int16? REMIT_TYPE { get; set; }

        //Added By Abhishek to disp radio button Month Date Wise 17 Sep 2014
        [RegularExpression("[MD]",ErrorMessage="Please select Month wise or Date Wise.")]
        public String MonthDateWise { get; set; }

        [RegularExpression("[SD]",ErrorMessage="Please select SRRDA or DPIU")]
        public String SrrdaDpiuWise { get; set; }

        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Date is not in valid format")]
        [Required(ErrorMessage="Please enter date.")]
        public String SearchBillDATE { get; set; }

        //Added By Abhishek to disp radio button Month Date Wise 17 Sep 2014
        [RegularExpression("[OP]", ErrorMessage = "Please select Cheque/Epay.")]
        public String chequeEpay { get; set; }
    }
}