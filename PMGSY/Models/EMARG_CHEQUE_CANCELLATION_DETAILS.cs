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
    
    public partial class EMARG_CHEQUE_CANCELLATION_DETAILS
    {
        public int ID { get; set; }
        public long BILL_ID { get; set; }
        public string EMARG_VOUCHER_NO { get; set; }
        public Nullable<int> CANCELLATION_MONTH { get; set; }
        public Nullable<int> CANCELLATION_YEAR { get; set; }
        public Nullable<System.DateTime> CANCELLATION_DATE { get; set; }
        public decimal CHQ_AMOUNT { get; set; }
        public string CHEQ_NO { get; set; }
        public int PIU_CODE { get; set; }
        public string CANCELLATION_REMARKS { get; set; }
        public int ROAD_CODE { get; set; }
        public string SCROLL_GENERATION_TYPE { get; set; }
    }
}