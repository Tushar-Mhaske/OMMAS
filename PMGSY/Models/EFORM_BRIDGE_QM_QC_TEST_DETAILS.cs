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
    
    public partial class EFORM_BRIDGE_QM_QC_TEST_DETAILS
    {
        public int WORK_ID { get; set; }
        public int EFORM_ID { get; set; }
        public int INFO_ID { get; set; }
        public int PR_ROAD_CODE { get; set; }
        public int ITEM_ID { get; set; }
        public Nullable<decimal> DPR_QUANTITY { get; set; }
        public Nullable<decimal> EXECUTED_QUANTITY { get; set; }
        public string TEST_NAME { get; set; }
        public Nullable<int> REQD_TEST_NUMBER { get; set; }
        public Nullable<int> CONDUCTED_TEST_NUMBER { get; set; }
        public string IS_TESTING_ADEQUATE { get; set; }
        public string IPADD { get; set; }
        public int USER_ID { get; set; }
    
        public virtual EFORM_BRIDGE_MASTER_WORK_ITEM EFORM_BRIDGE_MASTER_WORK_ITEM { get; set; }
        public virtual EFORM_BRIDGE_QM_QUALITY_ATTENTION EFORM_BRIDGE_QM_QUALITY_ATTENTION { get; set; }
        public virtual EFORM_MASTER EFORM_MASTER { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
        public virtual IMS_SANCTIONED_PROJECTS IMS_SANCTIONED_PROJECTS { get; set; }
    }
}
