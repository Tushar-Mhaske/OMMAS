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
    
    public partial class UDF_SCHEDULE_PRIORITY_WORK_LIST_Result
    {
        public int IMS_PR_ROAD_CODE { get; set; }
        public int MAST_STATE_CODE { get; set; }
        public int MAST_DISTRICT_CODE { get; set; }
        public int MAST_BLOCK_CODE { get; set; }
        public string IMS_PACKAGE_ID { get; set; }
        public int IMS_YEAR { get; set; }
        public string IMS_UPGRADE_CONNECT { get; set; }
        public string EXEC_ISCOMPLETED { get; set; }
        public Nullable<System.DateTime> EXEC_COMPLETION_DATE { get; set; }
        public Nullable<decimal> TOTAL_COST { get; set; }
    }
}
