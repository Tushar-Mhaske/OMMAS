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
    
    public partial class ACC_CERTIFICATE_DETAILS_SHADOW
    {
        public long AuditId { get; set; }
        public int ADMIN_NO_OFFICER_CODE { get; set; }
        public string PUBLIC_KEY { get; set; }
        public string CERTIFICATE { get; set; }
        public string CERTIFICATE_CHAIN { get; set; }
        public Nullable<int> UserId { get; set; }
        public string IPADD { get; set; }
        public string AuditAction { get; set; }
        public System.DateTime AuditDate { get; set; }
        public string AuditUser { get; set; }
        public string AuditApp { get; set; }
    }
}