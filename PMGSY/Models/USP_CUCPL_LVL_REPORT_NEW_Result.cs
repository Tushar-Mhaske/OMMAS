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
    
    public partial class USP_CUCPL_LVL_REPORT_NEW_Result
    {
        public int MAST_STATE_CODE { get; set; }
        public string MAST_STATE_NAME { get; set; }
        public int MAST_DISTRICT_CODE { get; set; }
        public string MAST_DISTRICT_NAME { get; set; }
        public int MAST_BLOCK_CODE { get; set; }
        public string MAST_BLOCK_NAME { get; set; }
        public string PLAN_CN_ROAD_NUMBER { get; set; }
        public string PLAN_RD_NAME { get; set; }
        public Nullable<decimal> PLAN_RD_LENGTH { get; set; }
        public int MAST_CONS_YEAR { get; set; }
        public int MAST_RENEW_YEAR { get; set; }
        public int MANE_PCI_YEAR { get; set; }
        public int MANE_SEGMENT_NO { get; set; }
        public decimal MANE_STR_CHAIN { get; set; }
        public decimal MANE_END_CHAIN { get; set; }
        public decimal SEGMENT_LENGTH_PCI { get; set; }
        public int MANE_SURFACE_TYPE { get; set; }
        public Nullable<int> CATEGORY { get; set; }
        public string MANE_SURFACE_NAME { get; set; }
        public int MANE_PCIINDEX { get; set; }
        public int POP { get; set; }
        public int MAST_TI_YEAR { get; set; }
        public int MAST_COMM_TI { get; set; }
        public int PLAN_CN_ROAD_CODE { get; set; }
        public string PLAN_RD_ROUTE { get; set; }
        public Nullable<int> PRIORITY_CATEGORY { get; set; }
        public Nullable<decimal> CATEGORY_SEGMENT_LENGTH { get; set; }
        public Nullable<decimal> CATEGORY_SEGMENT_PCI { get; set; }
    }
}
