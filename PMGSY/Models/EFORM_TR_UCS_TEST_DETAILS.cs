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
    
    public partial class EFORM_TR_UCS_TEST_DETAILS
    {
        public int DETAILS_ID { get; set; }
        public int MAIN_ITEM_ID { get; set; }
        public int SUBITEM_ID { get; set; }
        public int TYPEA_SUMM_ID { get; set; }
        public int EFORM_ID { get; set; }
        public int ROAD_ID { get; set; }
        public Nullable<decimal> CHAINAGE { get; set; }
        public Nullable<decimal> SAMPLE_WT { get; set; }
        public Nullable<decimal> DENSITY { get; set; }
        public Nullable<System.DateTime> TESTING_DATE { get; set; }
        public Nullable<decimal> SAMPLE_VOL { get; set; }
        public Nullable<decimal> LOAD_KN { get; set; }
        public Nullable<decimal> COMPR_STREANGTH { get; set; }
        public string IS_STD_CONFIRM { get; set; }
        public int USER_ID { get; set; }
        public string IPADD { get; set; }
    
        public virtual EFORM_MASTER EFORM_MASTER { get; set; }
        public virtual EFORM_TR_MAIN_ITEM_MASTER EFORM_TR_MAIN_ITEM_MASTER { get; set; }
        public virtual EFORM_TR_TYPEA_SUMMARY EFORM_TR_TYPEA_SUMMARY { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
        public virtual IMS_SANCTIONED_PROJECTS IMS_SANCTIONED_PROJECTS { get; set; }
    }
}
