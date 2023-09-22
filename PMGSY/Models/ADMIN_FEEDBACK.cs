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
    
    public partial class ADMIN_FEEDBACK
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ADMIN_FEEDBACK()
        {
            this.ADMIN_FEEDBACK_REPLY = new HashSet<ADMIN_FEEDBACK_REPLY>();
            this.ADMIN_FEEDBACK_FILES = new HashSet<ADMIN_FEEDBACK_FILES>();
            this.ADMIN_FEEDBACK_REPLY_FILES = new HashSet<ADMIN_FEEDBACK_REPLY_FILES>();
        }
    
        public int FEED_ID { get; set; }
        public string FEED_TYPE { get; set; }
        public string FEED_CODE { get; set; }
        public string FEED_PREFIX { get; set; }
        public string FEED_NAME { get; set; }
        public string FEED_ADDRESS { get; set; }
        public string FEED_TELE_CODE { get; set; }
        public string FEED_TELE_NUMBER { get; set; }
        public string FEED_MOBILE { get; set; }
        public string FEED_EMAIL { get; set; }
        public string FEED_IP { get; set; }
        public System.DateTime FEED_DATE { get; set; }
        public string FEED_CATEGORY { get; set; }
        public int FEED_AGAINST { get; set; }
        public Nullable<int> MAST_STATE_CODE { get; set; }
        public Nullable<int> MAST_DISTRICT_CODE { get; set; }
        public Nullable<int> MAST_BLOCK_CODE { get; set; }
        public string FEED_FOR { get; set; }
        public Nullable<int> MAST_HAB_CODE { get; set; }
        public Nullable<int> PLAN_CN_ROAD_CODE { get; set; }
        public string FEED_SUBJECT { get; set; }
        public string FEED_COMMENT { get; set; }
        public string IS_PMGSY_ROAD { get; set; }
        public string VILLAGE_NAME { get; set; }
        public string NEAREST_HAB { get; set; }
        public string ROAD_NAME { get; set; }
        public string FEED_APPROVAL { get; set; }
        public Nullable<System.DateTime> FEED_APPROVAL_DATE { get; set; }
        public string FEED_STATUS { get; set; }
        public Nullable<long> CITIZEN_ID { get; set; }
        public Nullable<int> PARENT_FEED_ID { get; set; }
        public Nullable<System.DateTime> FEED_UPLOAD_DATE { get; set; }
        public Nullable<int> ROAD_CODE { get; set; }
        public string CONTACT_ME { get; set; }
        public string LocationAddress { get; set; }
        public string VersionNo { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ADMIN_FEEDBACK_REPLY> ADMIN_FEEDBACK_REPLY { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ADMIN_FEEDBACK_FILES> ADMIN_FEEDBACK_FILES { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ADMIN_FEEDBACK_REPLY_FILES> ADMIN_FEEDBACK_REPLY_FILES { get; set; }
        public virtual UM_Citizen_User_Master UM_Citizen_User_Master { get; set; }
        public virtual MASTER_BLOCK MASTER_BLOCK { get; set; }
        public virtual MASTER_DISTRICT MASTER_DISTRICT { get; set; }
        public virtual MASTER_FEEDBACK_CATEGORY MASTER_FEEDBACK_CATEGORY { get; set; }
        public virtual MASTER_HABITATIONS MASTER_HABITATIONS { get; set; }
        public virtual MASTER_STATE MASTER_STATE { get; set; }
        public virtual PLAN_ROAD PLAN_ROAD { get; set; }
    }
}