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
    
    public partial class PFMS_BANK_MASTER
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PFMS_BANK_MASTER()
        {
            this.PFMS_OMMS_BANK_MAPPING = new HashSet<PFMS_OMMS_BANK_MAPPING>();
        }
    
        public short PFMS_BANK_ID { get; set; }
        public string PFMS_BANK_NAME { get; set; }
        public string PFMS_SHORT_NAME { get; set; }
        public string BANK_INTEGRATED_PFMS { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PFMS_OMMS_BANK_MAPPING> PFMS_OMMS_BANK_MAPPING { get; set; }
    }
}
