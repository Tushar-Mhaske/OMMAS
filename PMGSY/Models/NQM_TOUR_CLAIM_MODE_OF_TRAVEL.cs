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
    
    public partial class NQM_TOUR_CLAIM_MODE_OF_TRAVEL
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NQM_TOUR_CLAIM_MODE_OF_TRAVEL()
        {
            this.NQM_TRAVEL_CLAIM_DETAILS = new HashSet<NQM_TRAVEL_CLAIM_DETAILS>();
        }
    
        public int ID { get; set; }
        public string MODE_OF_TRAVEL { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NQM_TRAVEL_CLAIM_DETAILS> NQM_TRAVEL_CLAIM_DETAILS { get; set; }
    }
}