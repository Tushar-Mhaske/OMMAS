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
    
    public partial class PMIS_ADDITIVE_USED_DETAIL
    {
        public int ADDITIVE_DETAIL_ID { get; set; }
        public int TRIAL_STRETCH_ID { get; set; }
        public int ADDITIVE_ID { get; set; }
        public int IMS_PR_ROAD_CODE { get; set; }
        public Nullable<int> USERID { get; set; }
        public string IPADD { get; set; }
    
        public virtual MASTER_ADDITIVE MASTER_ADDITIVE { get; set; }
        public virtual PMIS_TRIAL_STRETCH_FDR_DETAIL PMIS_TRIAL_STRETCH_FDR_DETAIL { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
        public virtual IMS_SANCTIONED_PROJECTS IMS_SANCTIONED_PROJECTS { get; set; }
    }
}
