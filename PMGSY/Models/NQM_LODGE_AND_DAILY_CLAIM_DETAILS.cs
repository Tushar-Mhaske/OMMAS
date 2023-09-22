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
    
    public partial class NQM_LODGE_AND_DAILY_CLAIM_DETAILS
    {
        public int LODGE_CLAIM_ID { get; set; }
        public int TOUR_CLAIM_ID { get; set; }
        public System.DateTime DATE_FROM { get; set; }
        public System.DateTime DATE_TO { get; set; }
        public string TYPE_OF_CLAIM { get; set; }
        public string HOTEL_NAME { get; set; }
        public Nullable<decimal> AMOUNT_CLAIMED_HOTEL { get; set; }
        public Nullable<decimal> AMOUNT_CLAIMED_DAILY { get; set; }
        public System.DateTime DATE_OF_CLAIM { get; set; }
        public Nullable<decimal> AMOUNT_PASSED_HOTEL_CQC { get; set; }
        public string REMARK_CQC { get; set; }
        public Nullable<System.DateTime> DATE_OF_MODIFICATION_CQC { get; set; }
        public Nullable<decimal> AMOUNT_PASSED_DAILY { get; set; }
        public Nullable<decimal> AMOUNT_PASSED_HOTEL_FIN1 { get; set; }
        public string REMARK_FIN1 { get; set; }
        public Nullable<System.DateTime> DATE_OF_MODIFICATION_FIN1 { get; set; }
        public Nullable<decimal> AMOUNT_PASSED_HOTEL_FIN2 { get; set; }
        public string REMARK_FIN2 { get; set; }
        public Nullable<System.DateTime> DATE_OF_MODIFICATION_FIN2 { get; set; }
        public string UPLOADED_BILL_NAME { get; set; }
        public string UPLOADED_RECEIPT_NAME { get; set; }
        public string UPLOADED_BILL_PATH { get; set; }
        public string UPLOADED_RECEIPT_PATH { get; set; }
        public Nullable<int> USERID { get; set; }
        public string IPADD { get; set; }
    
        public virtual NQM_TOUR_CLAIM_MASTER NQM_TOUR_CLAIM_MASTER { get; set; }
    }
}
