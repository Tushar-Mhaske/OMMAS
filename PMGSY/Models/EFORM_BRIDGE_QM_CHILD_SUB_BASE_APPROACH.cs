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
    
    public partial class EFORM_BRIDGE_QM_CHILD_SUB_BASE_APPROACH
    {
        public int SUB_BASE_ID { get; set; }
        public int EFORM_ID { get; set; }
        public int PR_ROAD_CODE { get; set; }
        public int APPROACH_ID { get; set; }
        public Nullable<decimal> RD_LOC { get; set; }
        public string IS_GRADING_CONFORM { get; set; }
        public string IS_MATERIAL_SUITABLE { get; set; }
        public Nullable<decimal> IS_DRY_DENSITY { get; set; }
        public Nullable<decimal> COMPACTION { get; set; }
        public string IS_COMPACTION_INADEQUATE { get; set; }
        public Nullable<decimal> THICKNESS_PER_DPR { get; set; }
        public Nullable<decimal> MEASURED_THICKNESS { get; set; }
        public string IS_PRESCRIBED_THICKNESS_PROV { get; set; }
        public int USER_ID { get; set; }
        public string IPADD { get; set; }
    
        public virtual EFORM_BRIDGE_QM_APPROACH EFORM_BRIDGE_QM_APPROACH { get; set; }
        public virtual EFORM_MASTER EFORM_MASTER { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
        public virtual IMS_SANCTIONED_PROJECTS IMS_SANCTIONED_PROJECTS { get; set; }
    }
}
