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
    
    public partial class EFORM_QM_CHILD_SIDE_SLOPE_DETAIL
    {
        public int SS_DETAIL_ID { get; set; }
        public int SIDE_SLOP_ID { get; set; }
        public int EFORM_ID { get; set; }
        public int IMS_PR_ROAD_CODE { get; set; }
        public Nullable<decimal> LOCATION_RD { get; set; }
        public string SS_OBSERVED_BY_QM { get; set; }
        public string IS_SS_SATISFACTORY { get; set; }
        public string IS_PROFILE_SATISFACTORY { get; set; }
        public string GRADING { get; set; }
        public int QM_USER_ID { get; set; }
        public string IPADD { get; set; }
    
        public virtual EFORM_QM_SIDE_SLOPES EFORM_QM_SIDE_SLOPES { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
        public virtual EFORM_MASTER EFORM_MASTER { get; set; }
        public virtual IMS_SANCTIONED_PROJECTS IMS_SANCTIONED_PROJECTS { get; set; }
    }
}
