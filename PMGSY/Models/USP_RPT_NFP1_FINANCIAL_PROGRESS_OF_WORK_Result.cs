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
    
    public partial class USP_RPT_NFP1_FINANCIAL_PROGRESS_OF_WORK_Result
    {
        public string Name { get; set; }
        public int MAST_STATE_CODE { get; set; }
        public Nullable<int> MAST_DISTRICT_CODE { get; set; }
        public Nullable<int> Mast_Block_Code { get; set; }
        public Nullable<int> Pkg_No { get; set; }
        public Nullable<int> Sanction_Year { get; set; }
        public decimal Release_Amount { get; set; }
        public decimal Sanctioned_Amt { get; set; }
        public decimal Maintenance_Amt { get; set; }
        public decimal Value_of_Work { get; set; }
        public decimal Payment_Made { get; set; }
        public Nullable<int> Physical_Progress { get; set; }
        public Nullable<int> Completed_Status { get; set; }
    }
}
