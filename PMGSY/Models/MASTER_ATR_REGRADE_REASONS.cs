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
    
    public partial class MASTER_ATR_REGRADE_REASONS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MASTER_ATR_REGRADE_REASONS()
        {
            this.QUALITY_ATR_FILE = new HashSet<QUALITY_ATR_FILE>();
            this.QUALITY_SQM_ATR_FILE = new HashSet<QUALITY_SQM_ATR_FILE>();
        }
    
        public int REASON_ID { get; set; }
        public string REASON_NAME { get; set; }
        public string REASON_STATUS { get; set; }
        public string IS_ACTIVE { get; set; }
        public short TIER_TYPE { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<QUALITY_ATR_FILE> QUALITY_ATR_FILE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<QUALITY_SQM_ATR_FILE> QUALITY_SQM_ATR_FILE { get; set; }
    }
}