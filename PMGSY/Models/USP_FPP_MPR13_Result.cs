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
    
    public partial class USP_FPP_MPR13_Result
    {
        public int LocationCode { get; set; }
        public string LocationName { get; set; }
        public Nullable<int> SANCTIONED_WORKS { get; set; }
        public Nullable<int> PHY_COMP { get; set; }
        public Nullable<int> ACC_COMP { get; set; }
        public Nullable<int> PEND_ACC_COMP { get; set; }
        public Nullable<int> pending_6m_above { get; set; }
        public Nullable<decimal> SANC_AMT { get; set; }
    }
}
