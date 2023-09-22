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
    
    public partial class ACC_EPAY_MAIL_MASTER
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ACC_EPAY_MAIL_MASTER()
        {
            this.ACC_EPAY_MAIL_DETAILS = new HashSet<ACC_EPAY_MAIL_DETAILS>();
            this.ACC_EPAY_MAIL_RESEND_DETAILS = new HashSet<ACC_EPAY_MAIL_RESEND_DETAILS>();
            this.ACC_EPAY_MAIL_RESEND_DETAILS1 = new HashSet<ACC_EPAY_MAIL_RESEND_DETAILS>();
        }
    
        public long EPAY_ID { get; set; }
        public long BILL_ID { get; set; }
        public string EPAY_NO { get; set; }
        public byte EPAY_MONTH { get; set; }
        public short EPAY_YEAR { get; set; }
        public System.DateTime EPAY_DATE { get; set; }
        public string EMAIL_FROM { get; set; }
        public string EMAIL_TO { get; set; }
        public string EMAIL_SUBJECT { get; set; }
        public string EMAIL_CC { get; set; }
        public string EMAIL_BCC { get; set; }
        public System.DateTime EMAIL_SENT_DATE { get; set; }
        public bool IS_EPAY_VALID { get; set; }
        public string REQUEST_IP { get; set; }
        public int ADMIN_ND_CODE { get; set; }
        public short LVL_ID { get; set; }
        public string EPAY_EREMITTANCE { get; set; }
        public Nullable<byte> DEPT_ID { get; set; }
        public string DEPT_BANK_ACC_NO { get; set; }
        public string DPIU_TAN_NO { get; set; }
        public Nullable<int> USERID { get; set; }
        public string IPADD { get; set; }
        public string FILE_NAME { get; set; }
    
        public virtual ACC_BILL_MASTER ACC_BILL_MASTER { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ACC_EPAY_MAIL_DETAILS> ACC_EPAY_MAIL_DETAILS { get; set; }
        public virtual UM_Level_Master UM_Level_Master { get; set; }
        public virtual ACC_MASTER_REMIT_DEPT ACC_MASTER_REMIT_DEPT { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ACC_EPAY_MAIL_RESEND_DETAILS> ACC_EPAY_MAIL_RESEND_DETAILS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ACC_EPAY_MAIL_RESEND_DETAILS> ACC_EPAY_MAIL_RESEND_DETAILS1 { get; set; }
    }
}