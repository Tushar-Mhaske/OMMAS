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
    
    public partial class EMARG_REJECTION_MASTER
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EMARG_REJECTION_MASTER()
        {
            this.EMARG_ROAD_DETAILS_TEST_LIVE = new HashSet<EMARG_ROAD_DETAILS_TEST_LIVE>();
            this.EMARG_ROAD_DETAILS = new HashSet<EMARG_ROAD_DETAILS>();
        }
    
        public int REJECTION_ID { get; set; }
        public string REJECTION_TYPE { get; set; }
        public string REJECTION_CODE { get; set; }
        public string REJECTION_DESC { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EMARG_ROAD_DETAILS_TEST_LIVE> EMARG_ROAD_DETAILS_TEST_LIVE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EMARG_ROAD_DETAILS> EMARG_ROAD_DETAILS { get; set; }
    }
}
