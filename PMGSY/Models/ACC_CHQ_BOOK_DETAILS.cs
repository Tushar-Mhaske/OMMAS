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
    
    public partial class ACC_CHQ_BOOK_DETAILS
    {
        public int CHQ_BOOK_ID { get; set; }
        public string LEAF_START { get; set; }
        public string LEAF_END { get; set; }
        public string FUND_TYPE { get; set; }
        public System.DateTime ISSUE_DATE { get; set; }
        public short BANK_CODE { get; set; }
        public int ADMIN_ND_CODE { get; set; }
        public short LVL_ID { get; set; }
        public Nullable<bool> IS_CHQBOOK_COMPLETED { get; set; }
        public string ACC_TYPE { get; set; }
        public Nullable<int> USERID { get; set; }
        public string IPADD { get; set; }
    
        public virtual UM_Level_Master UM_Level_Master { get; set; }
        public virtual ACC_MASTER_FUND_TYPE ACC_MASTER_FUND_TYPE { get; set; }
        public virtual ADMIN_DEPARTMENT ADMIN_DEPARTMENT { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
    }
}
