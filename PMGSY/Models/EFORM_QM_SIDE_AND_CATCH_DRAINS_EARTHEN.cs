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
    
    public partial class EFORM_QM_SIDE_AND_CATCH_DRAINS_EARTHEN
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EFORM_QM_SIDE_AND_CATCH_DRAINS_EARTHEN()
        {
            this.EFORM_QM_CHILD_SD_AND_CW_DRAINS_DETAILS = new HashSet<EFORM_QM_CHILD_SD_AND_CW_DRAINS_DETAILS>();
        }
    
        public int DRAIN_ID { get; set; }
        public int EFORM_ID { get; set; }
        public int IMS_PR_ROAD_CODE { get; set; }
        public string IS_DPR_PROVISION { get; set; }
        public string IS_LONG_SLOPE_ADEQUATE { get; set; }
        public string ITEM_GRADING_17 { get; set; }
        public string IMPROVE_SUGGESTIONS { get; set; }
        public int QM_USER_ID { get; set; }
        public string IPADD { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EFORM_QM_CHILD_SD_AND_CW_DRAINS_DETAILS> EFORM_QM_CHILD_SD_AND_CW_DRAINS_DETAILS { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
        public virtual EFORM_MASTER EFORM_MASTER { get; set; }
        public virtual IMS_SANCTIONED_PROJECTS IMS_SANCTIONED_PROJECTS { get; set; }
    }
}
