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
    
    public partial class EFORM_QM_ARRANGEMENTS_OBS_DETAILS
    {
        public int OBS_ID { get; set; }
        public int EFORM_ID { get; set; }
        public int QM_USER_ID { get; set; }
        public int PR_ROAD_CODE { get; set; }
        public string IS_FIELD_LAB_ESTD { get; set; }
        public string IS_LAB_LOC_SAME { get; set; }
        public string IS_EQUIP_AVAILABLE { get; set; }
        public string IS_EQUIP_USED { get; set; }
        public string IS_EQUIP_AVAIL_VERIFY { get; set; }
        public string IS_ENGG_AVAILABLE { get; set; }
        public string IS_ALT_ENGG_ARR_SATISFIED { get; set; }
        public string IS_LAB_TECH_AVAILABLE { get; set; }
        public string ITEM_GRADING_2 { get; set; }
        public string IMPROVEMENT_REMARK { get; set; }
        public string IPADD { get; set; }
    
        public virtual UM_User_Master UM_User_Master { get; set; }
        public virtual EFORM_MASTER EFORM_MASTER { get; set; }
        public virtual IMS_SANCTIONED_PROJECTS IMS_SANCTIONED_PROJECTS { get; set; }
    }
}
