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
    
    public partial class ADMIN_QUALITY_MONITORS_INTER_STATE
    {
        public int ADMIN_QM_CODE_MAP_ID { get; set; }
        public int ADMIN_QM_CODE { get; set; }
        public int ALLOWED_STATE_CODE { get; set; }
        public System.DateTime ASSIGNED_DATE { get; set; }
        public int ASSIGNED_BY { get; set; }
        public Nullable<int> APPROVED_BY { get; set; }
        public Nullable<System.DateTime> APPROVED_DATE { get; set; }
        public Nullable<System.DateTime> DEACTIVATED_DATE { get; set; }
        public string APPROVED { get; set; }
    
        public virtual ADMIN_QUALITY_MONITORS ADMIN_QUALITY_MONITORS { get; set; }
        public virtual MASTER_STATE MASTER_STATE { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
        public virtual UM_User_Master UM_User_Master1 { get; set; }
    }
}