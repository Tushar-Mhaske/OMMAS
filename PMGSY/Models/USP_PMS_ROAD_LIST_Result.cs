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
    
    public partial class USP_PMS_ROAD_LIST_Result
    {
        public int PrRoadCode { get; set; }
        public string MAST_STATE_NAME { get; set; }
        public string MAST_DISTRICT_NAME { get; set; }
        public string MAST_BATCH_NAME { get; set; }
        public string BlockName { get; set; }
        public string PackageId { get; set; }
        public int SanctionYear { get; set; }
        public Nullable<decimal> SanctionLength { get; set; }
        public string RoadName { get; set; }
        public string ProposalType { get; set; }
        public string ProgressStatus { get; set; }
        public Nullable<decimal> COMPLETED_LENGTH { get; set; }
        public Nullable<System.DateTime> IMS_SANCTIONED_DATE { get; set; }
        public System.DateTime COMPLETED_DATE { get; set; }
        public string CONTRACTOR { get; set; }
        public int AgreementCode { get; set; }
        public Nullable<System.DateTime> AgreementDate { get; set; }
        public string TEND_AGREEMENT_NUMBER { get; set; }
        public decimal TEND_AGREEMENT_AMOUNT { get; set; }
        public Nullable<decimal> State_share { get; set; }
        public Nullable<decimal> TOTAL_COST { get; set; }
        public Nullable<decimal> Mord_Share { get; set; }
        public Nullable<System.DateTime> TEND_AGREEMENT_START_DATE { get; set; }
        public Nullable<System.DateTime> TEND_AGREEMENT_END_DATE { get; set; }
    }
}
