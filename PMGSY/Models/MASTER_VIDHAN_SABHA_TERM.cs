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
    
    public partial class MASTER_VIDHAN_SABHA_TERM
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MASTER_VIDHAN_SABHA_TERM()
        {
            this.MASTER_MLA_MEMBERS = new HashSet<MASTER_MLA_MEMBERS>();
        }
    
        public int MAST_STATE_CODE { get; set; }
        public int MAST_VS_TERM { get; set; }
        public Nullable<System.DateTime> MAST_VS_START_DATE { get; set; }
        public Nullable<System.DateTime> MAST_VS_END_DATE { get; set; }
        public string MAST_LOCK_STATUS { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MASTER_MLA_MEMBERS> MASTER_MLA_MEMBERS { get; set; }
        public virtual MASTER_STATE MASTER_STATE { get; set; }
    }
}