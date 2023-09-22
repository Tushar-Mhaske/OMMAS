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
    
    public partial class ACC_AUTH_REQUEST_MASTER
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ACC_AUTH_REQUEST_MASTER()
        {
            this.ACC_BILL_MASTER = new HashSet<ACC_BILL_MASTER>();
            this.ACC_AUTH_REQUEST_TRACKING = new HashSet<ACC_AUTH_REQUEST_TRACKING>();
        }
    
        public long AUTH_ID { get; set; }
        public string AUTH_NO { get; set; }
        public byte AUTH_MONTH { get; set; }
        public short AUTH_YEAR { get; set; }
        public System.DateTime AUTH_DATE { get; set; }
        public short TXN_ID { get; set; }
        public decimal CHQ_AMOUNT { get; set; }
        public decimal CASH_AMOUNT { get; set; }
        public decimal GROSS_AMOUNT { get; set; }
        public Nullable<int> MAST_CON_ID { get; set; }
        public string AUTH_FINALIZED { get; set; }
        public string FUND_TYPE { get; set; }
        public int ADMIN_ND_CODE { get; set; }
        public byte LVL_ID { get; set; }
        public string CURRENT_AUTH_STATUS { get; set; }
        public Nullable<int> USERID { get; set; }
        public string IPADD { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ACC_BILL_MASTER> ACC_BILL_MASTER { get; set; }
        public virtual ACC_MASTER_FUND_TYPE ACC_MASTER_FUND_TYPE { get; set; }
        public virtual ACC_MASTER_TXN ACC_MASTER_TXN { get; set; }
        public virtual ADMIN_DEPARTMENT ADMIN_DEPARTMENT { get; set; }
        public virtual MASTER_CONTRACTOR MASTER_CONTRACTOR { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ACC_AUTH_REQUEST_TRACKING> ACC_AUTH_REQUEST_TRACKING { get; set; }
    }
}