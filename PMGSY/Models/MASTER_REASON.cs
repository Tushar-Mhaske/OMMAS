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
    
    public partial class MASTER_REASON
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MASTER_REASON()
        {
            this.IMS_MLA_PROPOSAL_STATUS = new HashSet<IMS_MLA_PROPOSAL_STATUS>();
            this.IMS_MLA_PROPOSAL_STATUS1 = new HashSet<IMS_MLA_PROPOSAL_STATUS>();
            this.IMS_MP_PROPOSAL_STATUS = new HashSet<IMS_MP_PROPOSAL_STATUS>();
            this.IMS_MP_PROPOSAL_STATUS1 = new HashSet<IMS_MP_PROPOSAL_STATUS>();
            this.IMS_PROPOSAL_DROP_REQUEST_DETAILS = new HashSet<IMS_PROPOSAL_DROP_REQUEST_DETAILS>();
            this.MASTER_EXISTING_ROADS = new HashSet<MASTER_EXISTING_ROADS>();
            this.MASTER_EXISTING_ROADS_LOG = new HashSet<MASTER_EXISTING_ROADS_LOG>();
            this.MASTER_EXISTING_ROADS_Temp = new HashSet<MASTER_EXISTING_ROADS_Temp>();
            this.IMS_SANCTIONED_PROJECTS = new HashSet<IMS_SANCTIONED_PROJECTS>();
            this.IMS_SANCTIONED_PROJECTS1 = new HashSet<IMS_SANCTIONED_PROJECTS>();
        }
    
        public int MAST_REASON_CODE { get; set; }
        public string MAST_REASON_NAME { get; set; }
        public string MAST_REASON_TYPE { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IMS_MLA_PROPOSAL_STATUS> IMS_MLA_PROPOSAL_STATUS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IMS_MLA_PROPOSAL_STATUS> IMS_MLA_PROPOSAL_STATUS1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IMS_MP_PROPOSAL_STATUS> IMS_MP_PROPOSAL_STATUS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IMS_MP_PROPOSAL_STATUS> IMS_MP_PROPOSAL_STATUS1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IMS_PROPOSAL_DROP_REQUEST_DETAILS> IMS_PROPOSAL_DROP_REQUEST_DETAILS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MASTER_EXISTING_ROADS> MASTER_EXISTING_ROADS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MASTER_EXISTING_ROADS_LOG> MASTER_EXISTING_ROADS_LOG { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MASTER_EXISTING_ROADS_Temp> MASTER_EXISTING_ROADS_Temp { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IMS_SANCTIONED_PROJECTS> IMS_SANCTIONED_PROJECTS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IMS_SANCTIONED_PROJECTS> IMS_SANCTIONED_PROJECTS1 { get; set; }
    }
}
