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
    
    public partial class UM_Action_Master
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public UM_Action_Master()
        {
            this.UM_HomePage_Master = new HashSet<UM_HomePage_Master>();
        }
    
        public int ActionID { get; set; }
        public int MenuID { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public short ModuleID { get; set; }
    
        public virtual UM_Module_Master UM_Module_Master { get; set; }
        public virtual UM_Module_Master UM_Module_Master1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UM_HomePage_Master> UM_HomePage_Master { get; set; }
    }
}
