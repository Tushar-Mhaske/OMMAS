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
    
    public partial class QUALITY_PIU_INSPECTION_FILES
    {
        public int QM_FILE_ID { get; set; }
        public int IMS_PR_ROAD_CODE { get; set; }
        public Nullable<int> QM_OBSERVATION_ID { get; set; }
        public string QM_FILE_NAME { get; set; }
        public string QM_FILE_DESCR { get; set; }
        public Nullable<System.DateTime> QM_FILE_UPLOAD_DATE { get; set; }
        public string QM_FILES_FINALIZED { get; set; }
        public Nullable<int> QM_FILE_FINALIZED_BY { get; set; }
        public Nullable<System.DateTime> QM_FILE_FINALIZED_DATE { get; set; }
        public Nullable<int> USERID { get; set; }
        public string IPADD { get; set; }
    }
}
