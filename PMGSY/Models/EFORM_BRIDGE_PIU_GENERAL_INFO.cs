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
    
    public partial class EFORM_BRIDGE_PIU_GENERAL_INFO
    {
        public int INFO_ID { get; set; }
        public Nullable<int> EFORM_ID { get; set; }
        public Nullable<int> ADMIN_ND_CODE { get; set; }
        public Nullable<int> PR_ROAD_CODE { get; set; }
        public string WORK_STATUS { get; set; }
        public Nullable<System.DateTime> INSPECTION_DATE { get; set; }
        public string QM_CODE { get; set; }
        public Nullable<decimal> DEVIATION_LENGTH { get; set; }
        public string DEVIATION_REASON { get; set; }
        public Nullable<decimal> TECHNICAL_SANC_COST { get; set; }
        public Nullable<decimal> EXPENDITURE_DONE { get; set; }
        public Nullable<decimal> TOTAL_EXPENDITURE { get; set; }
        public Nullable<decimal> BILLS_PENDING { get; set; }
        public Nullable<System.DateTime> AWARD_OF_WORK_DATE { get; set; }
        public Nullable<System.DateTime> START_OF_WORK_DATE { get; set; }
        public Nullable<System.DateTime> STIPULATED_COMPLETION_DATE { get; set; }
        public Nullable<System.DateTime> ACTUAL_COMPLETION_DATE { get; set; }
        public string PIU_HEAD_NAME { get; set; }
        public string PIU_HEAD_MOBILE_NO { get; set; }
        public string PIU_HEAD_EMAIL { get; set; }
        public string PIU_ADDR { get; set; }
        public System.DateTime PIU_SIGN_DATE { get; set; }
        public Nullable<int> PIU_USER_ID { get; set; }
        public string IPADD { get; set; }
    
        public virtual ADMIN_DEPARTMENT ADMIN_DEPARTMENT { get; set; }
        public virtual ADMIN_DEPARTMENT ADMIN_DEPARTMENT1 { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
        public virtual IMS_SANCTIONED_PROJECTS IMS_SANCTIONED_PROJECTS { get; set; }
    }
}
