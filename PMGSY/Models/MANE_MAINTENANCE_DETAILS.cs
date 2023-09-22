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
    
    public partial class MANE_MAINTENANCE_DETAILS
    {
        public int MANE_MAIN_ID { get; set; }
        public int IMS_PR_ROAD_CODE { get; set; }
        public string MANE_MAIN_TYPE { get; set; }
        public int MANE_MAIN_YEAR { get; set; }
        public Nullable<int> MANE_MAIN_MONTH { get; set; }
        public Nullable<decimal> MANE_START_CHAINAGE { get; set; }
        public Nullable<decimal> MANE_END_CHAINAGE { get; set; }
        public decimal MANE_PROFILE_COST { get; set; }
        public int MANE_RENEWAL_TYPE { get; set; }
        public int MANE_TECH_CODE { get; set; }
        public decimal MANE_PERIODIC_COST { get; set; }
        public decimal MANE_OTHER_ITEM_COSE { get; set; }
        public decimal MANE_TOTAL_COST { get; set; }
        public Nullable<System.DateTime> MANE_RENEWAL_COMPLETION_DATE { get; set; }
        public string MANE_IS_PER_INCENTIVE { get; set; }
        public Nullable<int> MANE_PER_INCENTIVE_YEAR { get; set; }
        public Nullable<decimal> MANE_YEAR1_COST { get; set; }
        public Nullable<decimal> MANE_YEAR2_COST { get; set; }
        public Nullable<decimal> MANE_YEAR3_COST { get; set; }
        public Nullable<decimal> MANE_YEAR4_COST { get; set; }
        public Nullable<decimal> MANE_YEAR5_COST { get; set; }
        public Nullable<decimal> MANE_TOTAL_YEAR_COST { get; set; }
        public Nullable<int> USERID { get; set; }
        public string IPADD { get; set; }
    
        public virtual MASTER_RENEWAL_TYPE MASTER_RENEWAL_TYPE { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
        public virtual IMS_SANCTIONED_PROJECTS IMS_SANCTIONED_PROJECTS { get; set; }
    }
}
