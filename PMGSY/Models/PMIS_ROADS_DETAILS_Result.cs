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
    
    public partial class PMIS_ROADS_DETAILS_Result
    {
        public int PrRoadCode { get; set; }
        public int MAST_STATE_CODE { get; set; }
        public int MAST_DISTRICT_CODE { get; set; }
        public int MAST_BLOCK_CODE { get; set; }
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
    }
}
