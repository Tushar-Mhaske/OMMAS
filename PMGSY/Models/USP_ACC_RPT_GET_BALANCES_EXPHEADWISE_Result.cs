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
    
    public partial class USP_ACC_RPT_GET_BALANCES_EXPHEADWISE_Result
    {
        public short HEAD_ID { get; set; }
        public string HEAD_COMP_PROGRESS { get; set; }
        public int IMS_COLLABORATION { get; set; }
        public decimal OB_CREDIT_AMT { get; set; }
        public decimal MONTHLY_CREDIT_AMT { get; set; }
        public decimal OB_DEBIT_AMT { get; set; }
        public decimal MONTHLY_DEBIT_AMT { get; set; }
        public decimal OB_BALANCE_AMT { get; set; }
        public decimal MONTHLY_BALANCE_AMT { get; set; }
        public int IS_FA_CHANGED { get; set; }
    }
}
