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
    
    public partial class EFORM_QM_SIDE_SLOPES
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EFORM_QM_SIDE_SLOPES()
        {
            this.EFORM_QM_CHILD_CUT_SLOPE_DETAIL = new HashSet<EFORM_QM_CHILD_CUT_SLOPE_DETAIL>();
            this.EFORM_QM_CHILD_SIDE_SLOPE_DETAIL = new HashSet<EFORM_QM_CHILD_SIDE_SLOPE_DETAIL>();
        }
    
        public int SIDE_SLOP_ID { get; set; }
        public int EFORM_ID { get; set; }
        public int IMS_PR_ROAD_CODE { get; set; }
        public string SIDE_SLOPS_ASPER_DPR { get; set; }
        public string IS_ANALYSIS_DONE { get; set; }
        public string OBSERVATIONS { get; set; }
        public string SUBITEM_GRADING_5IV { get; set; }
        public string IMPROVE_SUGGESTIONS { get; set; }
        public string SUBITEM_GRADING_5 { get; set; }
        public string IMPROVE_SUGGESTIONS_5 { get; set; }
        public int QM_USER_ID { get; set; }
        public string IPADD { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EFORM_QM_CHILD_CUT_SLOPE_DETAIL> EFORM_QM_CHILD_CUT_SLOPE_DETAIL { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EFORM_QM_CHILD_SIDE_SLOPE_DETAIL> EFORM_QM_CHILD_SIDE_SLOPE_DETAIL { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
        public virtual EFORM_MASTER EFORM_MASTER { get; set; }
        public virtual IMS_SANCTIONED_PROJECTS IMS_SANCTIONED_PROJECTS { get; set; }
    }
}