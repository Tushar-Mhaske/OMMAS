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
    
    public partial class USP_GET_CUPL_PMGSY3_EXEMPTION_LIST_Result
    {
        public int CUPL_RANK { get; set; }
        public int CUPL_PMGSY3_ID { get; set; }
        public string MAST_DISTRICT_NAME { get; set; }
        public string MAST_BLOCK_NAME { get; set; }
        public string PLAN_CN_ROAD_NUMBER { get; set; }
        public decimal PLAN_RD_LENGTH { get; set; }
        public Nullable<decimal> ELIGIBLE_CANDIDATE_LENGTH { get; set; }
        public string PLAN_RD_NAME { get; set; }
        public string REQUEST_REMARKS { get; set; }
        public string REASON { get; set; }
        public Nullable<System.DateTime> DATE_OF_NON_INCLUSION { get; set; }
        public Nullable<System.DateTime> APPROVED_DATE { get; set; }
        public string APPROVAL { get; set; }
        public Nullable<int> TR_MRL_EXEMPTION_ID { get; set; }
    }
}
