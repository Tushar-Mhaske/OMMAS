//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PMGSY.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class EMARG_ROADWISE_EXPENDITURE_BALANCE_WORK_DETAILS
    {
        public int id { get; set; }
        public int BILL_ID { get; set; }
        public Nullable<int> BILL_ID_PREVIOUS { get; set; }
        public string EMARG_VOUCHER_NO { get; set; }
        public string VOUCHER_NO { get; set; }
        public string OMMAS_PACKAGE_NO { get; set; }
        public System.DateTime EMARG_DATE_TIME { get; set; }
        public int TXN_NO { get; set; }
        public string HEAD_CODE_DEDUCTIONS { get; set; }
        public decimal AMOUNT_DEDUCTIONS { get; set; }
        public string VOUCHER_TYPE { get; set; }
    }
}