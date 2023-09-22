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
    
    public partial class EFORM_QM_BASE_COURSE_III
    {
        public int LAYER_ID { get; set; }
        public int EFORM_ID { get; set; }
        public int IMS_PR_ROAD_CODE { get; set; }
        public string PROVISION_IN_DPR { get; set; }
        public string ITEM_EXECUTION_STATUS { get; set; }
        public string ACTUAL_EXECUTION { get; set; }
        public string IS_NEW_TECH_USED { get; set; }
        public short TECH_ID { get; set; }
        public string NEW_TECH_PROVIDER { get; set; }
        public string NAME_STABILISER_USED { get; set; }
        public string STABILISER_QTY_ASPER_DPR { get; set; }
        public string STABILISER_QTY_USED { get; set; }
        public Nullable<decimal> UCS_ASPER_DPR { get; set; }
        public string REASON_FOR_CHANGE { get; set; }
        public string ITEM_GRADING_9 { get; set; }
        public string IMPROVE_SUGGESTIONS { get; set; }
        public int QM_USER_ID { get; set; }
        public string IPADD { get; set; }
    
        public virtual EFORM_MASTER_NEW_TECH_ITEM EFORM_MASTER_NEW_TECH_ITEM { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
        public virtual EFORM_MASTER EFORM_MASTER { get; set; }
        public virtual IMS_SANCTIONED_PROJECTS IMS_SANCTIONED_PROJECTS { get; set; }
    }
}
