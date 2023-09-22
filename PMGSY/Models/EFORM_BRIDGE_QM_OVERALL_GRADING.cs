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
    
    public partial class EFORM_BRIDGE_QM_OVERALL_GRADING
    {
        public int GRADING_ID { get; set; }
        public int EFORM_ID { get; set; }
        public int PR_ROAD_CODE { get; set; }
        public string QUALITY_ARRANGEMENT { get; set; }
        public string ATT_TO_QUALITY { get; set; }
        public string FOUNDATION { get; set; }
        public string SUBSTRUCTURE { get; set; }
        public string SUPERSTRUCTURE { get; set; }
        public string LOAD_TEST { get; set; }
        public string BEARING { get; set; }
        public string BEARING_A { get; set; }
        public string BEARING_B { get; set; }
        public string EXPANSION_JOINTS { get; set; }
        public string EXPANSION_JOINTS_A { get; set; }
        public string EXPANSION_JOINTS_B { get; set; }
        public string APPROACH { get; set; }
        public string APPROACH_EMBANKMENT { get; set; }
        public string APPROACH_SUBBASE { get; set; }
        public string APPROACH_BASECOURSE { get; set; }
        public string APPROACH_WEARINGCOURSE { get; set; }
        public string APPROACH_PROTECTIONWORK { get; set; }
        public string BRIDGE_FURNITURE { get; set; }
        public string BRIDGE_FURNITURE_MAIN_INFO { get; set; }
        public string BRIDGE_FURN_CITIZEN_BOARD { get; set; }
        public string BRIDGE_FURN_GUARD_STONE { get; set; }
        public string BRIDGE_FURN_CAUTION_SIGNAGE { get; set; }
        public string OVERALL_GRADING { get; set; }
        public string QM_NAME { get; set; }
        public Nullable<System.DateTime> UPLOAD_DATE { get; set; }
        public int USER_ID { get; set; }
        public string IPADD { get; set; }
    
        public virtual EFORM_MASTER EFORM_MASTER { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
        public virtual IMS_SANCTIONED_PROJECTS IMS_SANCTIONED_PROJECTS { get; set; }
    }
}
