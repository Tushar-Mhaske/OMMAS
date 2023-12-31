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
    
    public partial class QM_PROFICIENCY_TEST_SCORE
    {
        public int ID { get; set; }
        public int EXAM_ID { get; set; }
        public int ADMIN_QM_CODE { get; set; }
        public Nullable<int> MARKS { get; set; }
        public Nullable<decimal> PERCENTAGE { get; set; }
        public int ENTRY_BY_USERID { get; set; }
        public string ENTRY_BY_IPADD { get; set; }
        public System.DateTime DATE_OF_ENTRY { get; set; }
        public Nullable<System.DateTime> CQC_ENTRY_DATE { get; set; }
        public string CQC_REMARK { get; set; }
        public Nullable<int> CQC_USER_ID { get; set; }
        public string CQC_IP_ADDRESS { get; set; }
        public string EXAM_STATUS { get; set; }
        public string IS_LATEST { get; set; }
    
        public virtual ADMIN_QUALITY_MONITORS ADMIN_QUALITY_MONITORS { get; set; }
        public virtual QM_PROFICIENCY_TEST_MASTER QM_PROFICIENCY_TEST_MASTER { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
        public virtual UM_User_Master UM_User_Master1 { get; set; }
    }
}
