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
    
    public partial class ACC_EAUTH_DETAILS
    {
        public int EAUTH_ID { get; set; }
        public short EAUTH_TXN_NO { get; set; }
        public decimal AMOUNT { get; set; }
        public int MAST_CON_ID { get; set; }
        public int IMS_AGREEMENT_CODE { get; set; }
        public string APPROVAL_STAUS { get; set; }
        public Nullable<System.DateTime> APPOVAL_DATE_SRRDA { get; set; }
        public string APPROVAL_REMARKS { get; set; }
        public int USERID { get; set; }
        public string IPADD { get; set; }
        public string IMS_PACKAGE_ID { get; set; }
        public Nullable<decimal> TEND_AGREEMENT_AMOUNT { get; set; }
        public Nullable<decimal> ALREADY_AUTH_AMOUNT { get; set; }
    
        public virtual MASTER_CONTRACTOR MASTER_CONTRACTOR { get; set; }
        public virtual ACC_EAUTH_MASTER ACC_EAUTH_MASTER { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
    }
}