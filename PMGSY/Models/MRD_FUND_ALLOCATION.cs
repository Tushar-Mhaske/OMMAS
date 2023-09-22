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
    
    public partial class MRD_FUND_ALLOCATION
    {
        public int MAST_STATE_CODE { get; set; }
        public int ADMIN_NO_CODE { get; set; }
        public string MAST_FUND_TYPE { get; set; }
        public int MAST_FUNDING_AGENCY_CODE { get; set; }
        public int MAST_YEAR { get; set; }
        public int MAST_TRANSACTION_NO { get; set; }
        public decimal MAST_ALLOCATION_AMOUNT { get; set; }
        public Nullable<System.DateTime> MAST_ALLOCATION_DATE { get; set; }
        public string MAST_ALLOCATION_ORDER { get; set; }
        public string MAST_ALLOCATION_FILE { get; set; }
        public Nullable<int> USERID { get; set; }
        public string IPADD { get; set; }
    
        public virtual ADMIN_DEPARTMENT ADMIN_DEPARTMENT { get; set; }
        public virtual MASTER_FUNDING_AGENCY MASTER_FUNDING_AGENCY { get; set; }
        public virtual MASTER_STATE MASTER_STATE { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
    }
}
