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
    
    public partial class EFORM_TR_TYPEC_DETAIL
    {
        public int TYPEC_DETAIL_ID { get; set; }
        public int MAIN_ITEM_ID { get; set; }
        public int SUBITEM_ID { get; set; }
        public int EFORM_ID { get; set; }
        public int TABLE_ID { get; set; }
        public int DETAIL_ITEM_ID { get; set; }
        public int TYPEC_SUMM_ID { get; set; }
        public string SIEVE_DESIGNATION { get; set; }
        public Nullable<decimal> SAMPLE_WEIGHT { get; set; }
        public Nullable<decimal> RETAINED_WEIGHT { get; set; }
        public Nullable<decimal> CUMULATIVE_WEIGHT { get; set; }
        public Nullable<decimal> PASSING_WEIGHT { get; set; }
        public string PERMISSIBLE_RANGE { get; set; }
        public int USER_ID { get; set; }
        public string IPADD { get; set; }
    
        public virtual EFORM_MASTER EFORM_MASTER { get; set; }
        public virtual EFORM_TR_DETAIL_ITEM_MASTER EFORM_TR_DETAIL_ITEM_MASTER { get; set; }
        public virtual EFORM_TR_MAIN_ITEM_MASTER EFORM_TR_MAIN_ITEM_MASTER { get; set; }
        public virtual EFORM_TR_SUBITEM_MASTER EFORM_TR_SUBITEM_MASTER { get; set; }
        public virtual EFORM_TR_TABLE_MASTER EFORM_TR_TABLE_MASTER { get; set; }
        public virtual EFORM_TR_TYPEC_SUMMARY EFORM_TR_TYPEC_SUMMARY { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
    }
}
