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
    
    public partial class MASTER_DESIGNATION
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MASTER_DESIGNATION()
        {
            this.ADMIN_NODAL_OFFICERS = new HashSet<ADMIN_NODAL_OFFICERS>();
            this.ADMIN_QUALITY_MONITORS = new HashSet<ADMIN_QUALITY_MONITORS>();
            this.ADMIN_SQC = new HashSet<ADMIN_SQC>();
            this.ADMIN_TECHNICAL_AGENCY = new HashSet<ADMIN_TECHNICAL_AGENCY>();
            this.EXEC_OFFICER_DETAILS = new HashSet<EXEC_OFFICER_DETAILS>();
            this.IMS_EC_TRAINING_PERSONS = new HashSet<IMS_EC_TRAINING_PERSONS>();
            this.IMS_EC_TRAININGS = new HashSet<IMS_EC_TRAININGS>();
        }
    
        public int MAST_DESIG_CODE { get; set; }
        public string MAST_DESIG_NAME { get; set; }
        public string MAST_DESIG_TYPE { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ADMIN_NODAL_OFFICERS> ADMIN_NODAL_OFFICERS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ADMIN_QUALITY_MONITORS> ADMIN_QUALITY_MONITORS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ADMIN_SQC> ADMIN_SQC { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ADMIN_TECHNICAL_AGENCY> ADMIN_TECHNICAL_AGENCY { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EXEC_OFFICER_DETAILS> EXEC_OFFICER_DETAILS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IMS_EC_TRAINING_PERSONS> IMS_EC_TRAINING_PERSONS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IMS_EC_TRAININGS> IMS_EC_TRAININGS { get; set; }
    }
}
