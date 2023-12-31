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
    
    public partial class PMIS_PLAN_MASTER
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PMIS_PLAN_MASTER()
        {
            this.PMIS_CHAINAGEWISE_COMPLETION_DETAILS = new HashSet<PMIS_CHAINAGEWISE_COMPLETION_DETAILS>();
            this.PMIS_PLAN_DETAILS = new HashSet<PMIS_PLAN_DETAILS>();
            this.PMIS_PROGRESS_DETAILS = new HashSet<PMIS_PROGRESS_DETAILS>();
            this.PMIS_PROGRESS_MASTER = new HashSet<PMIS_PROGRESS_MASTER>();
        }
    
        public int PLAN_ID { get; set; }
        public int IMS_PR_ROAD_CODE { get; set; }
        public System.DateTime PLAN_CREATION_DATE { get; set; }
        public int USERID { get; set; }
        public string IPADD { get; set; }
        public string IS_LATEST { get; set; }
        public Nullable<int> BASELINE_NO { get; set; }
        public string IS_FINALISED { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PMIS_CHAINAGEWISE_COMPLETION_DETAILS> PMIS_CHAINAGEWISE_COMPLETION_DETAILS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PMIS_PLAN_DETAILS> PMIS_PLAN_DETAILS { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PMIS_PROGRESS_DETAILS> PMIS_PROGRESS_DETAILS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PMIS_PROGRESS_MASTER> PMIS_PROGRESS_MASTER { get; set; }
        public virtual IMS_SANCTIONED_PROJECTS IMS_SANCTIONED_PROJECTS { get; set; }
    }
}
