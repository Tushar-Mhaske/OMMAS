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
    
    public partial class MASTER_TKT_CATEGORY
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MASTER_TKT_CATEGORY()
        {
            this.TKT_TICKET_MASTER = new HashSet<TKT_TICKET_MASTER>();
            this.TKT_TICKET_MASTER1 = new HashSet<TKT_TICKET_MASTER>();
        }
    
        public int MAST_CATEGORY_CODE { get; set; }
        public string MAST_CATEGORY_NAME { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TKT_TICKET_MASTER> TKT_TICKET_MASTER { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TKT_TICKET_MASTER> TKT_TICKET_MASTER1 { get; set; }
    }
}