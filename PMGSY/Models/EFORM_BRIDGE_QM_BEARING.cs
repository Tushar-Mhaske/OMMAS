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
    
    public partial class EFORM_BRIDGE_QM_BEARING
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EFORM_BRIDGE_QM_BEARING()
        {
            this.EFORM_BRIDGE_QM_CHILD_BEARING_TYPE = new HashSet<EFORM_BRIDGE_QM_CHILD_BEARING_TYPE>();
        }
    
        public int BEARING_ID { get; set; }
        public int EFORM_ID { get; set; }
        public int PR_ROAD_CODE { get; set; }
        public string ROLLER_ROCKER_BEARING { get; set; }
        public string ELASTOMERIC_BEARING { get; set; }
        public string POT_BEARING { get; set; }
        public string SPHERICAL_BEARING { get; set; }
        public string CYLINDRICAL_BEARING { get; set; }
        public string IS_OTHER_BEARING_TYPE { get; set; }
        public string OTHER_BEARING_TYPE { get; set; }
        public string ITEM_GRADING_8 { get; set; }
        public string IMPROVEMENT_REMARK { get; set; }
        public string IPADD { get; set; }
        public int USER_ID { get; set; }
    
        public virtual EFORM_MASTER EFORM_MASTER { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EFORM_BRIDGE_QM_CHILD_BEARING_TYPE> EFORM_BRIDGE_QM_CHILD_BEARING_TYPE { get; set; }
        public virtual IMS_SANCTIONED_PROJECTS IMS_SANCTIONED_PROJECTS { get; set; }
    }
}