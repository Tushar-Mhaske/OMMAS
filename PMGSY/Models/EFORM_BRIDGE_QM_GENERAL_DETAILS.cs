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
    
    public partial class EFORM_BRIDGE_QM_GENERAL_DETAILS
    {
        public int GENERAL_ID { get; set; }
        public int EFORM_ID { get; set; }
        public int PR_ROAD_CODE { get; set; }
        public System.DateTime INSPECTION_DATE { get; set; }
        public string IS_WORK_STAT_LAYOUT { get; set; }
        public string IS_WORK_STAT_FOUNDATION { get; set; }
        public string IS_WORK_STAT_SUBSTRUCTURE { get; set; }
        public string IS_WORK_STAT_SUPERSTRUCTURE { get; set; }
        public string IS_WORK_STAT_PROT_WORK { get; set; }
        public string IS_WORK_STAT_APPROACH { get; set; }
        public string IS_WORK_STAT_FINISHING_STAGE { get; set; }
        public string IS_VIDEO_RECORDS { get; set; }
        public string IPADD { get; set; }
        public int USER_ID { get; set; }
    
        public virtual EFORM_MASTER EFORM_MASTER { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
        public virtual IMS_SANCTIONED_PROJECTS IMS_SANCTIONED_PROJECTS { get; set; }
    }
}
