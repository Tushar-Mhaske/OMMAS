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
    
    public partial class ACC_EPAY_MAIL_RESEND_DETAILS
    {
        public long DETAIL_ID { get; set; }
        public long OLD_EPAY_ID { get; set; }
        public long NEW_EPAY_ID { get; set; }
        public string FLAG_DR { get; set; }
        public System.DateTime RESEND_DATE { get; set; }
        public string REASON { get; set; }
        public string FILE_NAME { get; set; }
        public string REMARKS { get; set; }
        public int USERID { get; set; }
        public string IPADD { get; set; }
    
        public virtual ACC_EPAY_MAIL_MASTER ACC_EPAY_MAIL_MASTER { get; set; }
        public virtual ACC_EPAY_MAIL_MASTER ACC_EPAY_MAIL_MASTER1 { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
    }
}
