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
    
    public partial class RSA_MASTER_ISSUE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RSA_MASTER_ISSUE()
        {
            this.EXEC_RSA_INSPECTION_DETAILS = new HashSet<EXEC_RSA_INSPECTION_DETAILS>();
        }
    
        public int ISSUE_CODE { get; set; }
        public string ISSUE_SHORT_DESC { get; set; }
        public string ISSUE_LONG_DESC { get; set; }
        public string ISSUE_SEVERITY { get; set; }
        public string ISSUE_RECOMMENDATION { get; set; }
        public string ISSUE_IS_ACTIVE { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EXEC_RSA_INSPECTION_DETAILS> EXEC_RSA_INSPECTION_DETAILS { get; set; }
    }
}