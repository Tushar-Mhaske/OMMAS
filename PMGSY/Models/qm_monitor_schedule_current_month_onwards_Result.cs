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
    
    public partial class qm_monitor_schedule_current_month_onwards_Result
    {
        public int ADMIN_QM_CODE { get; set; }
        public string ADMIN_QM_TYPE { get; set; }
        public string MONITOR_NAME { get; set; }
        public int MAST_STATE_CODE { get; set; }
        public string STATE_NAME { get; set; }
        public int MAST_DISTRICT_CODE { get; set; }
        public string DISTRICT_NAME { get; set; }
        public Nullable<int> MAST_DISTRICT_CODE2 { get; set; }
        public string DISTRICT_NAME2 { get; set; }
        public Nullable<int> MAST_DISTRICT_CODE3 { get; set; }
        public string DISTRICT_NAME3 { get; set; }
        public Nullable<System.DateTime> SCHEDULE_DATE { get; set; }
        public int ADMIN_SCHEDULE_CODE { get; set; }
        public string FINALIZE_FLAG { get; set; }
        public string INSP_STATUS_FLAG { get; set; }
        public int ADMIN_IM_YEAR { get; set; }
        public int ADMIN_IM_MONTH { get; set; }
    }
}
