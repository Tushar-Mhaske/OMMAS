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
    
    public partial class IMS_FREEZE_DETAILS
    {
        public int MAST_STATE_CODE { get; set; }
        public int IMS_YEAR { get; set; }
        public byte MAST_PMGSY_SCHEME { get; set; }
        public int IMS_BATCH { get; set; }
        public int IMS_TRANSACTION_NO { get; set; }
        public System.DateTime IMS_FREEZE_DATE { get; set; }
        public string IMS_FREEZE_STATUS { get; set; }
        public Nullable<int> USERID { get; set; }
        public string IPADD { get; set; }
    
        public virtual MASTER_BATCH MASTER_BATCH { get; set; }
        public virtual MASTER_STATE MASTER_STATE { get; set; }
        public virtual MASTER_YEAR MASTER_YEAR { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
    }
}
