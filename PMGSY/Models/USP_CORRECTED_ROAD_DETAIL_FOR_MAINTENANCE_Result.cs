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
    
    public partial class USP_CORRECTED_ROAD_DETAIL_FOR_MAINTENANCE_Result
    {
        public int EMARG_ID { get; set; }
        public int MAST_STATE_CODE { get; set; }
        public string MAST_STATE_NAME { get; set; }
        public byte PMGSY_SCHEME { get; set; }
        public int MAST_DISTRICT_CODE { get; set; }
        public string MAST_DISTRICT_NAME { get; set; }
        public int MAST_BLOCK_CODE { get; set; }
        public string MAST_BLOCK_NAME { get; set; }
        public int PIU_CODE { get; set; }
        public string PIU_NAME { get; set; }
        public int ROAD_CODE { get; set; }
        public string SANCTION_YEAR { get; set; }
        public int SANCTION_BATCH { get; set; }
        public string PACKAGE_NO { get; set; }
        public string ROAD_NAME { get; set; }
        public decimal SANCTION_LENGTH { get; set; }
        public decimal COMPLETED_LENGTH { get; set; }
        public decimal CC_LENGTH { get; set; }
        public decimal BT_LENGTH { get; set; }
        public int SANCTION_CD_WORK { get; set; }
        public string CORE_NETWORK_CODE { get; set; }
        public string TRAFFIC_CATEGORY { get; set; }
        public Nullable<decimal> CARRIAGE_WAY_WIDTH { get; set; }
        public string STAGE { get; set; }
        public Nullable<System.DateTime> COMPLETION_DATE { get; set; }
        public string WORK_ORDER_NO { get; set; }
        public Nullable<System.DateTime> WORK_ORDER_DATE { get; set; }
        public string CONTRACTOR_NAME { get; set; }
        public string CONTRACTOR_PAN { get; set; }
        public Nullable<int> CONTRACTOR_ID { get; set; }
    }
}
