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
    
    public partial class PFMS_OMMAS_DSC_MAPPING
    {
        public int ID { get; set; }
        public int ADMIN_ND_CODE { get; set; }
        public string DSC_REQ_FILENAME { get; set; }
        public string ACK_RECEVIED_FILENAME { get; set; }
        public System.DateTime ACK_RECEIVED_DATE { get; set; }
        public string ACK_DSC_STATUS { get; set; }
        public string REJECTION_CODE { get; set; }
        public string REJECTION_NARRATION { get; set; }
        public Nullable<System.DateTime> FILE_PROCESS_DATE { get; set; }
        public Nullable<bool> IS_ACTIVE { get; set; }
    
        public virtual ADMIN_DEPARTMENT ADMIN_DEPARTMENT { get; set; }
    }
}
