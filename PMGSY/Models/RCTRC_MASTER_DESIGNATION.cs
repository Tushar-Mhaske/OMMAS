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
    
    public partial class RCTRC_MASTER_DESIGNATION
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RCTRC_MASTER_DESIGNATION()
        {
            this.RCTRC_ADMIN_QUALITY_MONITORS = new HashSet<RCTRC_ADMIN_QUALITY_MONITORS>();
        }
    
        public int MAST_DESIG_CODE { get; set; }
        public string MAST_DESIG_NAME { get; set; }
        public string MAST_DESIG_TYPE { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RCTRC_ADMIN_QUALITY_MONITORS> RCTRC_ADMIN_QUALITY_MONITORS { get; set; }
    }
}
