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
    
    public partial class MASTER_MATRIX
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MASTER_MATRIX()
        {
            this.MATRIX_DISTRICT_MAPPING = new HashSet<MATRIX_DISTRICT_MAPPING>();
            this.MATRIX_HEAD_WEIGHTAGE = new HashSet<MATRIX_HEAD_WEIGHTAGE>();
        }
    
        public int MAST_MATRIX_ASSG_ID { get; set; }
        public int MATRIX_CODE { get; set; }
        public int MAST_STATE_CODE { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MATRIX_DISTRICT_MAPPING> MATRIX_DISTRICT_MAPPING { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MATRIX_HEAD_WEIGHTAGE> MATRIX_HEAD_WEIGHTAGE { get; set; }
        public virtual MASTER_STATE MASTER_STATE { get; set; }
    }
}
