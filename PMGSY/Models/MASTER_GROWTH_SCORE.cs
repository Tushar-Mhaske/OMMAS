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
    
    public partial class MASTER_GROWTH_SCORE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MASTER_GROWTH_SCORE()
        {
            this.MATRIX_HEAD_WEIGHTAGE = new HashSet<MATRIX_HEAD_WEIGHTAGE>();
        }
    
        public int MAST_SCORE_ID { get; set; }
        public int MAST_PARENT_ID { get; set; }
        public string MAST_SCORE_NAME { get; set; }
        public byte MAST_SCORE_LEVEL { get; set; }
        public int MAST_SCORE_VALUE { get; set; }
        public string MAST_SCORE_TYPE { get; set; }
        public string MAST_MATRIX_NO { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MATRIX_HEAD_WEIGHTAGE> MATRIX_HEAD_WEIGHTAGE { get; set; }
    }
}