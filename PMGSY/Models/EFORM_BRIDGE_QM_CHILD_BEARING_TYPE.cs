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
    
    public partial class EFORM_BRIDGE_QM_CHILD_BEARING_TYPE
    {
        public int CONDITION_ID { get; set; }
        public int EFORM_ID { get; set; }
        public int PR_ROAD_CODE { get; set; }
        public int BEARING_ID { get; set; }
        public string IS_METALLIC_BEARING { get; set; }
        public string IS_RUSTED { get; set; }
        public string IS_FUNCTIONING { get; set; }
        public string IS_GREASE_REQUIRED { get; set; }
        public string IS_CRACK_IN_SUPPORRT_MEMBER { get; set; }
        public string IS_EFFECTIVE_ANCHOR_BOLT { get; set; }
        public string OTHER_DEFECT_IN_METALLIC { get; set; }
        public string IS_ELASTOMETRIC_BEARING { get; set; }
        public string IS_PAD_COND_BAD { get; set; }
        public string IS_BEARING_CLEAN { get; set; }
        public string OTHER_DEFECT_IN_ELASTOMERIC { get; set; }
        public string OTHER_DEFECT_THAN_BOTH { get; set; }
        public string IS_TEST_RESULT_BEARING_AVL { get; set; }
        public string IPADD { get; set; }
        public int USER_ID { get; set; }
    
        public virtual EFORM_BRIDGE_QM_BEARING EFORM_BRIDGE_QM_BEARING { get; set; }
        public virtual EFORM_MASTER EFORM_MASTER { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
        public virtual IMS_SANCTIONED_PROJECTS IMS_SANCTIONED_PROJECTS { get; set; }
    }
}
