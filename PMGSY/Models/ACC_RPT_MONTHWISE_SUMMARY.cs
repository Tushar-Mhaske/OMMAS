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
    
    public partial class ACC_RPT_MONTHWISE_SUMMARY
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ACC_RPT_MONTHWISE_SUMMARY()
        {
            this.ACC_RPT_CONT_AGR_ROAD_HEAD_BALANCES = new HashSet<ACC_RPT_CONT_AGR_ROAD_HEAD_BALANCES>();
            this.ACC_RPT_FA_HEAD_BALANCES = new HashSet<ACC_RPT_FA_HEAD_BALANCES>();
            this.ACC_RPT_HEADWISE_BALANCES = new HashSet<ACC_RPT_HEADWISE_BALANCES>();
        }
    
        public int MONTH_ID { get; set; }
        public int MAST_STATE_CODE { get; set; }
        public int MAST_AGENCY_CODE { get; set; }
        public int ADMIN_ND_CODE { get; set; }
        public int ACC_MONTH { get; set; }
        public int ACC_YEAR { get; set; }
        public string FUND_TYPE { get; set; }
        public int LVL_ID { get; set; }
        public Nullable<int> USERID { get; set; }
        public string IPADD { get; set; }
    
        public virtual ACC_MASTER_FUND_TYPE ACC_MASTER_FUND_TYPE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ACC_RPT_CONT_AGR_ROAD_HEAD_BALANCES> ACC_RPT_CONT_AGR_ROAD_HEAD_BALANCES { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ACC_RPT_FA_HEAD_BALANCES> ACC_RPT_FA_HEAD_BALANCES { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ACC_RPT_HEADWISE_BALANCES> ACC_RPT_HEADWISE_BALANCES { get; set; }
        public virtual ADMIN_DEPARTMENT ADMIN_DEPARTMENT { get; set; }
        public virtual MASTER_AGENCY MASTER_AGENCY { get; set; }
        public virtual MASTER_STATE MASTER_STATE { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
    }
}