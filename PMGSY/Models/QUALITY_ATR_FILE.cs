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
    
    public partial class QUALITY_ATR_FILE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public QUALITY_ATR_FILE()
        {
            this.QUALITY_MAINTENANCE_ATR_IMAGE_FILE = new HashSet<QUALITY_MAINTENANCE_ATR_IMAGE_FILE>();
        }
    
        public int QM_ATR_ID { get; set; }
        public int QM_OBSERVATION_ID { get; set; }
        public string ATR_FILE_NAME { get; set; }
        public System.DateTime ATR_ENTRY_DATE { get; set; }
        public string ATR_REGRADE_STATUS { get; set; }
        public Nullable<System.DateTime> ATR_REGRADE_DATE { get; set; }
        public string ATR_REGRADE_REMARKS { get; set; }
        public string ATR_IS_DELETED { get; set; }
        public Nullable<int> USERID { get; set; }
        public string IPADD { get; set; }
        public Nullable<int> ATR_REASON_ID { get; set; }
        public string ATR_VERIFICATION_FINALIZED { get; set; }
        public Nullable<int> SQM_OBSERVATION_ID { get; set; }
    
        public virtual QUALITY_QM_OBSERVATION_MASTER QUALITY_QM_OBSERVATION_MASTER { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<QUALITY_MAINTENANCE_ATR_IMAGE_FILE> QUALITY_MAINTENANCE_ATR_IMAGE_FILE { get; set; }
        public virtual MASTER_ATR_REGRADE_REASONS MASTER_ATR_REGRADE_REASONS { get; set; }
    }
}
