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
    
    public partial class MASTER_TRAFFIC_TYPE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MASTER_TRAFFIC_TYPE()
        {
            this.MANE_EMARG_CONTRACT = new HashSet<MANE_EMARG_CONTRACT>();
            this.IMS_SANCTIONED_PROJECTS = new HashSet<IMS_SANCTIONED_PROJECTS>();
        }
    
        public int MAST_TRAFFIC_CODE { get; set; }
        public string MAST_TRAFFIC_NAME { get; set; }
        public string MAST_TRAFFIC_STATUS { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MANE_EMARG_CONTRACT> MANE_EMARG_CONTRACT { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IMS_SANCTIONED_PROJECTS> IMS_SANCTIONED_PROJECTS { get; set; }
    }
}