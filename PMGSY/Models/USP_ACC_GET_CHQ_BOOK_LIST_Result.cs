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
    
    public partial class USP_ACC_GET_CHQ_BOOK_LIST_Result
    {
        public int CHQ_BOOK_ID { get; set; }
        public string ACC_TYPE { get; set; }
        public string LEAF_START { get; set; }
        public string LEAF_END { get; set; }
        public string FUND_TYPE { get; set; }
        public string ISSUE_DATE { get; set; }
        public short BANK_CODE { get; set; }
        public int ADMIN_ND_CODE { get; set; }
        public short LVL_ID { get; set; }
        public Nullable<bool> IS_CHQBOOK_COMPLETED { get; set; }
    }
}
