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
    
    public partial class EFORM_BRIDGE_QM_CHILD_RCC_SUPERSTRUCTURE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EFORM_BRIDGE_QM_CHILD_RCC_SUPERSTRUCTURE()
        {
            this.EFORM_BRIDGE_QM_CHILD_COMPL_ACTIVITY_RCC_SUPERSTRUCTURE = new HashSet<EFORM_BRIDGE_QM_CHILD_COMPL_ACTIVITY_RCC_SUPERSTRUCTURE>();
            this.EFORM_BRIDGE_QM_CHILD_ON_QOM_RCC_SUPERSTRUCTURE = new HashSet<EFORM_BRIDGE_QM_CHILD_ON_QOM_RCC_SUPERSTRUCTURE>();
        }
    
        public int RCC_ID { get; set; }
        public int STRUCTURE_ID { get; set; }
        public int EFORM_ID { get; set; }
        public int PR_ROAD_CODE { get; set; }
        public string WORK_TYPE { get; set; }
        public string IS_CEMENT_TEST_CONDUCTED { get; set; }
        public string IS_NDT_CONDUCTED { get; set; }
        public int USER_ID { get; set; }
        public string IPADD { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EFORM_BRIDGE_QM_CHILD_COMPL_ACTIVITY_RCC_SUPERSTRUCTURE> EFORM_BRIDGE_QM_CHILD_COMPL_ACTIVITY_RCC_SUPERSTRUCTURE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EFORM_BRIDGE_QM_CHILD_ON_QOM_RCC_SUPERSTRUCTURE> EFORM_BRIDGE_QM_CHILD_ON_QOM_RCC_SUPERSTRUCTURE { get; set; }
        public virtual EFORM_BRIDGE_QM_SUPERSTRUCTURE EFORM_BRIDGE_QM_SUPERSTRUCTURE { get; set; }
        public virtual EFORM_MASTER EFORM_MASTER { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
        public virtual IMS_SANCTIONED_PROJECTS IMS_SANCTIONED_PROJECTS { get; set; }
    }
}
