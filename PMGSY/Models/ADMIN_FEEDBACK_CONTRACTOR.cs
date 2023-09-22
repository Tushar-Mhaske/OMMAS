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
    
    public partial class ADMIN_FEEDBACK_CONTRACTOR
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ADMIN_FEEDBACK_CONTRACTOR()
        {
            this.ADMIN_FEEDBACK_CONTRACTOR_FILES = new HashSet<ADMIN_FEEDBACK_CONTRACTOR_FILES>();
        }
    
        public int FEED_ID { get; set; }
        public string FEED_CODE { get; set; }
        public int MAST_CON_ID { get; set; }
        public string MAST_CON_PAN { get; set; }
        public System.DateTime FEED_DATE { get; set; }
        public string FEED_CATEGORY { get; set; }
        public int STATE_CODE { get; set; }
        public int District_Code { get; set; }
        public Nullable<int> IMS_PR_ROAD_CODE { get; set; }
        public string FEED_SUBJECT { get; set; }
        public string FEED_COMMENT { get; set; }
        public string FEED_APPROVAL { get; set; }
        public Nullable<System.DateTime> FEED_APPROVAL_DATE { get; set; }
        public string FEED_STATUS { get; set; }
        public string IPADD { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ADMIN_FEEDBACK_CONTRACTOR_FILES> ADMIN_FEEDBACK_CONTRACTOR_FILES { get; set; }
        public virtual MASTER_STATE MASTER_STATE { get; set; }
        public virtual MASTER_CONTRACTOR MASTER_CONTRACTOR { get; set; }
        public virtual MASTER_DISTRICT MASTER_DISTRICT { get; set; }
        public virtual IMS_SANCTIONED_PROJECTS IMS_SANCTIONED_PROJECTS { get; set; }
    }
}