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
    
    public partial class OFP_REQUEST_DOCUMENT_MAPPING
    {
        public int REQUEST_DOCUMENT_ID { get; set; }
        public int REQUEST_ID { get; set; }
        public int DOCUMENT_ID { get; set; }
        public string UPLOAD_FILE_NAME { get; set; }
        public System.DateTime DOCUMENT_GENERATION_DATE { get; set; }
        public Nullable<System.DateTime> DOCUMENT_FINALIZATION_DATE { get; set; }
        public string DOCUMENT_FINALIZE { get; set; }
        public string REMARKS { get; set; }
        public Nullable<int> USERID { get; set; }
        public string IPADD { get; set; }
    
        public virtual OFP_REQUEST_MASTER OFP_REQUEST_MASTER { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
    }
}
