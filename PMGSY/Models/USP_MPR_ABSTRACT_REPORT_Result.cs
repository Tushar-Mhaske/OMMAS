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
    
    public partial class USP_MPR_ABSTRACT_REPORT_Result
    {
        public int MAST_STATE_CODE { get; set; }
        public string MAST_STATE_NAME { get; set; }
        public Nullable<int> NO_OF_ROADWORKS_SANCTIONED { get; set; }
        public Nullable<decimal> ROAD_LENGTH_SANCTIONED { get; set; }
        public Nullable<int> NO_OF_BRIDGE_WORKS_SANCTIONED { get; set; }
        public Nullable<int> NO_OF_ROADWORKS_COMPLETED { get; set; }
        public Nullable<decimal> ROADLENGTH_COMPLETED { get; set; }
        public Nullable<decimal> BALANCE_ROAD_LENGTH { get; set; }
    }
}