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
    
    public partial class MASTER_TECHNOLOGY
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MASTER_TECHNOLOGY()
        {
            this.EXEC_PERIODIC_MAINT_PHYSICAL = new HashSet<EXEC_PERIODIC_MAINT_PHYSICAL>();
            this.EXEC_TECH_MONTHLY_STATUS = new HashSet<EXEC_TECH_MONTHLY_STATUS>();
            this.MANE_IMS_TECH = new HashSet<MANE_IMS_TECH>();
            this.MAST_TECH_LAYER_MAPPING = new HashSet<MAST_TECH_LAYER_MAPPING>();
            this.EFORM_PIU_TECHNOLOGY_DETAIL = new HashSet<EFORM_PIU_TECHNOLOGY_DETAIL>();
            this.IMS_PROPOSAL_TECH = new HashSet<IMS_PROPOSAL_TECH>();
        }
    
        public int MAST_TECH_CODE { get; set; }
        public string MAST_TECH_NAME { get; set; }
        public string MAST_TECH_DESC { get; set; }
        public string MAST_TECH_TYPE { get; set; }
        public string MAST_TECH_STATUS { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EXEC_PERIODIC_MAINT_PHYSICAL> EXEC_PERIODIC_MAINT_PHYSICAL { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EXEC_TECH_MONTHLY_STATUS> EXEC_TECH_MONTHLY_STATUS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MANE_IMS_TECH> MANE_IMS_TECH { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MAST_TECH_LAYER_MAPPING> MAST_TECH_LAYER_MAPPING { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EFORM_PIU_TECHNOLOGY_DETAIL> EFORM_PIU_TECHNOLOGY_DETAIL { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IMS_PROPOSAL_TECH> IMS_PROPOSAL_TECH { get; set; }
    }
}