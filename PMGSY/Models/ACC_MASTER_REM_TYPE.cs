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
    
    public partial class ACC_MASTER_REM_TYPE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ACC_MASTER_REM_TYPE()
        {
            this.ACC_BILL_MASTER = new HashSet<ACC_BILL_MASTER>();
            this.ACC_REM_ACCOUNT_DETAILS = new HashSet<ACC_REM_ACCOUNT_DETAILS>();
        }
    
        public byte REM_TYPE { get; set; }
        public string REM_DESC { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ACC_BILL_MASTER> ACC_BILL_MASTER { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ACC_REM_ACCOUNT_DETAILS> ACC_REM_ACCOUNT_DETAILS { get; set; }
    }
}