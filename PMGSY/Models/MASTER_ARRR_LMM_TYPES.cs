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
    
    public partial class MASTER_ARRR_LMM_TYPES
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MASTER_ARRR_LMM_TYPES()
        {
            this.MASTER_ARRR_ITEMS = new HashSet<MASTER_ARRR_ITEMS>();
            this.MASTER_ARRR_STATES_RATES = new HashSet<MASTER_ARRR_STATES_RATES>();
        }
    
        public int MAST_LMM_TYPE_CODE { get; set; }
        public int MAST_LMM_TYPE { get; set; }
        public int MAST_LMM_ACTY_CODE { get; set; }
        public string MAST_LMM_DESC { get; set; }
        public string MAST_LMM_ACTIVITY { get; set; }
        public int MAST_LMM_OUTPUT_UNIT { get; set; }
        public Nullable<decimal> MAST_LMM_OUTPUT_RATE { get; set; }
        public int MAST_LMM_USAGE_UNIT { get; set; }
        public string MAST_LMM_ACTIVE { get; set; }
        public string MAST_LMM_CODE { get; set; }
        public int MAST_LMM_CAT_CODE { get; set; }
        public Nullable<int> USERID { get; set; }
        public string IPADD { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MASTER_ARRR_ITEMS> MASTER_ARRR_ITEMS { get; set; }
        public virtual MASTER_ARRR_LMM_CATEGORY MASTER_ARRR_LMM_CATEGORY { get; set; }
        public virtual MASTER_UNITS MASTER_UNITS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MASTER_ARRR_STATES_RATES> MASTER_ARRR_STATES_RATES { get; set; }
    }
}
