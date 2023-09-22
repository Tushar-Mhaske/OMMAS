using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace PMGSY.Models.Report
{
    public class CBPaymentResultModel
    {
        [DisplayName("Date")]
        public string bill_date { get; set; }
        [DisplayName("Money Receipt")]
        public string bill_no { get; set; }
        [DisplayName("Perticulars of Transactions")]
        public string narration { get; set; }
        [DisplayName("Cash(Rs.)")]
        public decimal cash { get; set; }
        [DisplayName("Cheque Number")]
        public string cheque_epay { get; set; }
        [DisplayName("Bank(Rs.)")]
        public decimal bank_auth { get; set; }
        [DisplayName("Account Code")]
        public string head_code { get; set; }
    }
}