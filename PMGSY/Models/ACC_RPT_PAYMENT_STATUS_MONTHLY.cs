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
    
    public partial class ACC_RPT_PAYMENT_STATUS_MONTHLY
    {
        public long PAY_ID { get; set; }
        public int MAST_STATE_CODE { get; set; }
        public int MAST_AGENCY_CODE { get; set; }
        public int MAST_DISTRICT_CODE { get; set; }
        public int IMS_COLLABORATION { get; set; }
        public byte MAST_PMGSY_SCHEME { get; set; }
        public short BILL_MONTH { get; set; }
        public short BILL_YEAR { get; set; }
        public decimal EXPENDITURE { get; set; }
    
        public virtual MASTER_AGENCY MASTER_AGENCY { get; set; }
        public virtual MASTER_DISTRICT MASTER_DISTRICT { get; set; }
        public virtual MASTER_FUNDING_AGENCY MASTER_FUNDING_AGENCY { get; set; }
        public virtual MASTER_STATE MASTER_STATE { get; set; }
    }
}
