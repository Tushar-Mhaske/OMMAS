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
    
    public partial class UDF_STATE_RANK_QUALITY_Result
    {
        public int MAST_STATE_CODE { get; set; }
        public string MAST_STATE_NAME { get; set; }
        public Nullable<decimal> LAB { get; set; }
        public Nullable<decimal> COMPLETED_SATISFACTORY { get; set; }
        public Nullable<decimal> MAINTENANCE_SATISFACTORY { get; set; }
        public Nullable<decimal> NQM_VS_SQM { get; set; }
        public Nullable<decimal> ATR_STATE_PENDING_PERCENTAGE { get; set; }
        public Nullable<decimal> OVRALL_QUALITY { get; set; }
    }
}
