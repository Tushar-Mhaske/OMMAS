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
    
    public partial class QUALITY_QM_JOINT_INSPECTION_FILE
    {
        public int QM_JNT_INSP_FILE_ID { get; set; }
        public int QM_JNT_INSP_CODE { get; set; }
        public string QM_JNT_INSP_FILE_NAME { get; set; }
        public string QM_JNT_INSP_FILE_DESCR { get; set; }
        public Nullable<System.DateTime> IMS_JNT_INSP_FILE_UPLOAD_DATE { get; set; }
        public string QM_JNT_INSP_FILE_TYPE { get; set; }
        public Nullable<int> USERID { get; set; }
        public string IPADD { get; set; }
    
        public virtual QUALITY_QM_JOINT_INSPECTION QUALITY_QM_JOINT_INSPECTION { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
    }
}
