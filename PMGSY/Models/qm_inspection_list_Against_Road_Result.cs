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
    
    public partial class qm_inspection_list_Against_Road_Result
    {
        public int QM_OBSERVATION_ID { get; set; }
        public Nullable<int> ADMIN_SCHEDULE_CODE { get; set; }
        public int IMS_PR_ROAD_CODE { get; set; }
        public int ADMIN_QM_CODE { get; set; }
        public string MONITOR_NAME { get; set; }
        public int MAST_STATE_CODE { get; set; }
        public string STATE_NAME { get; set; }
        public int MAST_DISTRICT_CODE { get; set; }
        public string DISTRICT_NAME { get; set; }
        public int MAST_BLOCK_CODE { get; set; }
        public string BLOCK_NAME { get; set; }
        public string IMS_PACKAGE_ID { get; set; }
        public int IMS_YEAR { get; set; }
        public string IMS_ROAD_NAME { get; set; }
        public decimal QM_INSPECTED_START_CHAINAGE { get; set; }
        public decimal QM_INSPECTED_END_CHAINAGE { get; set; }
        public string QM_INSPECTION_DATE { get; set; }
        public string QM_SCHEDULE_DATE { get; set; }
        public int ADMIN_IM_YEAR { get; set; }
        public int ADMIN_IM_MONTH { get; set; }
        public string QM_OBS_UPLOAD_DATE { get; set; }
        public string IMS_ISCOMPLETED { get; set; }
        public string OVERALL_GRADE { get; set; }
        public Nullable<int> NO_OF_PHOTO_UPLOADED { get; set; }
        public string QM_ATR_STATUS { get; set; }
        public string UPLOAD_BY { get; set; }
        public string ADMIN_IS_ENQUIRY { get; set; }
        public string IMS_PROPOSAL_TYPE { get; set; }
        public string PMGSY_SCHEME { get; set; }
        public decimal IMS_PAV_LENGTH { get; set; }
        public Nullable<decimal> IMS_BRIDGE_LENGTH { get; set; }
        public string ADMIN_QM_TYPE { get; set; }
        public string ATR_VERIFICATION_FINALIZED { get; set; }
    }
}
