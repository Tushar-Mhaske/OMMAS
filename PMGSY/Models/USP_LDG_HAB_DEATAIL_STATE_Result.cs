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
    
    public partial class USP_LDG_HAB_DEATAIL_STATE_Result
    {
        public int ROAD_ID { get; set; }
        public int HAB_CODE { get; set; }
        public string SCHEME { get; set; }
        public string CONNECTIVITY { get; set; }
        public int Completion_Stage { get; set; }
        public string STAGE { get; set; }
        public string PIU { get; set; }
        public string SANCTION_YEAR { get; set; }
        public Nullable<int> MAST_STATE_LDG_CODE { get; set; }
        public Nullable<int> MAST_DISTRICT_LDG_CODE { get; set; }
        public Nullable<int> MAST_BLOCK_LDG_CODE { get; set; }
        public string IDENTIFICATION { get; set; }
        public decimal SANCTION_LENGTH { get; set; }
        public Nullable<decimal> ESTIMATED_COST { get; set; }
        public decimal ACTUAL_COST { get; set; }
        public decimal BT_TYPE { get; set; }
        public Nullable<decimal> CC_TYPE { get; set; }
        public Nullable<decimal> COMPLETED_LENGTH { get; set; }
        public string PROJECT_END_DATE { get; set; }
    }
}
