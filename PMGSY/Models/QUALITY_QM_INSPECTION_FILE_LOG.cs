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
    
    public partial class QUALITY_QM_INSPECTION_FILE_LOG
    {
        public int QM_FILE_LOG_ID { get; set; }
        public int QM_FILE_ID { get; set; }
        public Nullable<int> ADMIN_SCHEDULE_CODE { get; set; }
        public int IMS_PR_ROAD_CODE { get; set; }
        public Nullable<int> QM_OBSERVATION_ID { get; set; }
        public string QM_FILE_NAME { get; set; }
        public string QM_FILE_DESCR { get; set; }
        public Nullable<System.DateTime> QM_FILE_UPLOAD_DATE { get; set; }
        public Nullable<decimal> QM_LATITUDE { get; set; }
        public Nullable<decimal> QM_LONGITUDE { get; set; }
        public int QM_FILE_REMOVED_BY { get; set; }
        public System.DateTime QM_FILE_REMOVED_DATETIME { get; set; }
        public Nullable<int> USERID { get; set; }
        public string IPADD { get; set; }
    
        public virtual QUALITY_QM_SCHEDULE QUALITY_QM_SCHEDULE { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
        public virtual IMS_SANCTIONED_PROJECTS IMS_SANCTIONED_PROJECTS { get; set; }
    }
}
