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
    
    public partial class MASTER_CONTRACTOR_GRIEVANCE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MASTER_CONTRACTOR_GRIEVANCE()
        {
            this.CONTRACTOR_GRIEVANCE_DETAILS = new HashSet<CONTRACTOR_GRIEVANCE_DETAILS>();
            this.CONTRACTOR_GRIEVANCE_ROLE_MAPPING = new HashSet<CONTRACTOR_GRIEVANCE_ROLE_MAPPING>();
        }
    
        public int GRIEVANCE_ID { get; set; }
        public string GRIEVANCE_TYPE { get; set; }
        public string GRIEVANCE_SUBTYPE { get; set; }
        public string IS_VALID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTRACTOR_GRIEVANCE_DETAILS> CONTRACTOR_GRIEVANCE_DETAILS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTRACTOR_GRIEVANCE_ROLE_MAPPING> CONTRACTOR_GRIEVANCE_ROLE_MAPPING { get; set; }
    }
}
