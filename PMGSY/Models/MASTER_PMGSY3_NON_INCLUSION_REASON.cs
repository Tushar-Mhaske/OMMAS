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
    
    public partial class MASTER_PMGSY3_NON_INCLUSION_REASON
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MASTER_PMGSY3_NON_INCLUSION_REASON()
        {
            this.TR_MRL_EXEMPTION = new HashSet<TR_MRL_EXEMPTION>();
        }
    
        public int MAST_PMGSY3_REASON_CODE { get; set; }
        public string MAST_PMGSY3_REASON { get; set; }
        public string MAST_PMGSY3_IS_VALID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TR_MRL_EXEMPTION> TR_MRL_EXEMPTION { get; set; }
    }
}
