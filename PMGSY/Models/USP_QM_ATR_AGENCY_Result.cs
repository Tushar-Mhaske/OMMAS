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
    
    public partial class USP_QM_ATR_AGENCY_Result
    {
        public int MAST_STATE_CODE { get; set; }
        public string MAST_STATE_NAME { get; set; }
        public int MAST_DISTRICT_CODE { get; set; }
        public string MAST_DISTRICT_NAME { get; set; }
        public string MAST_AGENCY_NAME { get; set; }
        public int MAST_AGENCY_CODE { get; set; }
        public Nullable<int> TOTAL_INSP { get; set; }
        public Nullable<int> ATR_REQUIRED_INITIALLY { get; set; }
        public Nullable<int> REJECTED { get; set; }
        public Nullable<int> PENDING_STATE { get; set; }
        public Nullable<int> PENDING_NRRDA { get; set; }
        public Nullable<int> ACCEPTED { get; set; }
        public Nullable<int> VERIFICATION { get; set; }
        public Nullable<int> COMMITEE { get; set; }
    }
}
