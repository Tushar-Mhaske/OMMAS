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
    
    public partial class IMS_PROPOSAL_DROP_REQUEST_DETAILS
    {
        public int DROP_REQ_ID { get; set; }
        public Nullable<int> DROP_REQ_CODE { get; set; }
        public int IMS_PR_ROAD_CODE { get; set; }
        public string MORD_APPROVED { get; set; }
        public Nullable<System.DateTime> APPROVED_DATE_TIME { get; set; }
        public Nullable<int> DROP_PROJ_PDF_CODE { get; set; }
        public Nullable<int> APPROVED_USERID { get; set; }
        public string APPROVED_IPADD { get; set; }
        public Nullable<int> USERID { get; set; }
        public Nullable<System.DateTime> DROP_REQ_DATE { get; set; }
        public Nullable<double> EXP_INCURRED { get; set; }
        public Nullable<double> RECOUP_AMOUNT { get; set; }
        public Nullable<int> DROP_REASON { get; set; }
    
        public virtual IMS_PROPOSAL_DROP_REQUEST IMS_PROPOSAL_DROP_REQUEST { get; set; }
        public virtual MASTER_REASON MASTER_REASON { get; set; }
        public virtual IMS_SANCTIONED_PROJECTS IMS_SANCTIONED_PROJECTS { get; set; }
    }
}