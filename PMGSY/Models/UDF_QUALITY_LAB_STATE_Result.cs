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
    
    public partial class UDF_QUALITY_LAB_STATE_Result
    {
        public int IMS_YEAR { get; set; }
        public int MAST_STATE_CODE { get; set; }
        public string MAST_STATE_NAME { get; set; }
        public int MAST_DISTRICT_CODE { get; set; }
        public string MAST_DISTRICT_NAME { get; set; }
        public int MAST_BLOCK_CODE { get; set; }
        public string MAST_BLOCK_NAME { get; set; }
        public string IMS_PACKAGE_ID { get; set; }
        public int TEND_AGREEMENT_CODE { get; set; }
        public Nullable<System.DateTime> TEND_DATE_OF_COMPLETION { get; set; }
        public Nullable<System.DateTime> TEND_DATE_OF_AWARD_WORK { get; set; }
        public Nullable<System.DateTime> TEND_DATE_OF_COMMENCEMENT { get; set; }
        public Nullable<int> QM_LAB_ID { get; set; }
        public Nullable<System.DateTime> QM_LAB_ESTABLISHMENT_DATE { get; set; }
        public string QM_SQC_APPROVAL { get; set; }
        public Nullable<decimal> QM_LAB_FILE_LATITUDE { get; set; }
        public Nullable<decimal> QM_LAB_FILE_LONGITUDE { get; set; }
        public string QM_LOCK_STATUS { get; set; }
        public Nullable<System.DateTime> QM_LAB_FILE_UPLOAD_DATE { get; set; }
        public Nullable<int> PHOTO { get; set; }
    }
}