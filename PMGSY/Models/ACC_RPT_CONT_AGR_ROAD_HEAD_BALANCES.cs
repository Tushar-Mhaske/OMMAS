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
    
    public partial class ACC_RPT_CONT_AGR_ROAD_HEAD_BALANCES
    {
        public int MONTH_ID { get; set; }
        public int BALANCE_ID { get; set; }
        public short HEAD_ID { get; set; }
        public string HEAD_COMP_PROGRESS { get; set; }
        public Nullable<int> TEND_AGREEMENT_CODE { get; set; }
        public Nullable<int> IMS_PR_ROAD_CODE { get; set; }
        public Nullable<decimal> MONTHLY_BALANCE_AMT { get; set; }
        public Nullable<long> ROAD_FP_BILL_ID { get; set; }
        public Nullable<long> TEND_FIRST_BILL_ID { get; set; }
    
        public virtual ACC_BILL_MASTER ACC_BILL_MASTER { get; set; }
        public virtual ACC_BILL_MASTER ACC_BILL_MASTER1 { get; set; }
        public virtual ACC_RPT_MONTHWISE_SUMMARY ACC_RPT_MONTHWISE_SUMMARY { get; set; }
        public virtual TEND_AGREEMENT_MASTER TEND_AGREEMENT_MASTER { get; set; }
        public virtual IMS_SANCTIONED_PROJECTS IMS_SANCTIONED_PROJECTS { get; set; }
    }
}
