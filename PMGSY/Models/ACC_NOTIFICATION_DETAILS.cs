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
    
    public partial class ACC_NOTIFICATION_DETAILS
    {
        public long DETAIL_ID { get; set; }
        public long INITIATION_BILL_ID { get; set; }
        public Nullable<short> NOTIFICATION_ID { get; set; }
        public int INITIATOR_ADMIN_ND_CODE { get; set; }
        public Nullable<int> RECEIVER_ADMIN_ND_CODE { get; set; }
        public string REQUEST_NARRATION { get; set; }
        public Nullable<long> RECEIVER_BILL_ID { get; set; }
        public string FUND_TYPE { get; set; }
        public System.DateTime DT_NOTIFICATION { get; set; }
        public Nullable<int> USERID { get; set; }
        public string IPADD { get; set; }
    
        public virtual ACC_MASTER_FUND_TYPE ACC_MASTER_FUND_TYPE { get; set; }
        public virtual ACC_NOTIFICATION_MASTER ACC_NOTIFICATION_MASTER { get; set; }
        public virtual ADMIN_DEPARTMENT ADMIN_DEPARTMENT { get; set; }
        public virtual ADMIN_DEPARTMENT ADMIN_DEPARTMENT1 { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
    }
}
