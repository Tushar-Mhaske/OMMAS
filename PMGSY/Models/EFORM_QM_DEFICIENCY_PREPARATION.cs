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
    
    public partial class EFORM_QM_DEFICIENCY_PREPARATION
    {
        public int DEF_ID { get; set; }
        public int EFORM_ID { get; set; }
        public int IMS_PR_ROAD_CODE { get; set; }
        public string IS_NO_DEFICIENCY { get; set; }
        public string IS_BOQ_NOT_CLEAR { get; set; }
        public string IS_INVERT_LEVEL_INCORRECT { get; set; }
        public string IS_CD_STRUCT_INSUFFICE { get; set; }
        public string IS_NO_SIDE_DRAIN { get; set; }
        public string IS_DESIGN_NOT_PROVIDED { get; set; }
        public string IS_JUNCTION_DESIGN_INAP { get; set; }
        public string IS_GUARD_NOT_PROVIDED { get; set; }
        public string IS_DEVIATION { get; set; }
        public string IS_EARTHWORK_NOT_BAL { get; set; }
        public string IS_PAVMENT_NOT_ASPER { get; set; }
        public string ANY_OTHER_COMMENT { get; set; }
        public int QM_USER_ID { get; set; }
        public string IPADD { get; set; }
    
        public virtual UM_User_Master UM_User_Master { get; set; }
        public virtual EFORM_MASTER EFORM_MASTER { get; set; }
        public virtual IMS_SANCTIONED_PROJECTS IMS_SANCTIONED_PROJECTS { get; set; }
    }
}
