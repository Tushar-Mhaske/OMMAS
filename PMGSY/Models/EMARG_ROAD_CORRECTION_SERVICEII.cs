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
    using System.Collections.Generic;
    
    public partial class EMARG_ROAD_CORRECTION_SERVICEII
    {
        public int EID { get; set; }
        public string MAST_STATE_NAME { get; set; }
        public string MAST_DISTRICT_NAME { get; set; }
        public string PACKAGE_NO { get; set; }
        public string CONTRACTOR { get; set; }
        public string PAN { get; set; }
        public string AGREEMENT_NO { get; set; }
        public string AGREEMENT_DATE { get; set; }
        public int ROAD_CODE { get; set; }
        public string ROAD_NAME { get; set; }
        public string COMPLETION_DATE { get; set; }
        public string SANCTIONED_LENGTH { get; set; }
        public string COMPLETED_LENGTH { get; set; }
        public string CARRIAGE_WIDTH { get; set; }
        public string TRAFFIC_DENSITY { get; set; }
        public string REMARKS { get; set; }
        public Nullable<System.DateTime> ACK_MESSAGE_DATE_TIME { get; set; }
        public string ACK_STATUS { get; set; }
        public string ACK_REJECT_CODE { get; set; }
        public string ACK_REMARKS { get; set; }
    }
}
