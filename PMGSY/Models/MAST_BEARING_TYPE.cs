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
    
    public partial class MAST_BEARING_TYPE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MAST_BEARING_TYPE()
        {
            this.QUALITY_QM_OBSERVATION_DETAIL = new HashSet<QUALITY_QM_OBSERVATION_DETAIL>();
        }
    
        public int MAST_BEARING_CODE { get; set; }
        public string MAST_BEARING_NAME { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<QUALITY_QM_OBSERVATION_DETAIL> QUALITY_QM_OBSERVATION_DETAIL { get; set; }
    }
}