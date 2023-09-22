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
    
    public partial class EFORM_QM_CC_SR_PVAEMENTS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EFORM_QM_CC_SR_PVAEMENTS()
        {
            this.EFORM_QM_CHILD_CC_AND_SR_PAVEMENTS_OBSERVATION_DETAILS = new HashSet<EFORM_QM_CHILD_CC_AND_SR_PAVEMENTS_OBSERVATION_DETAILS>();
        }
    
        public int PAVE_ID { get; set; }
        public int EFORM_ID { get; set; }
        public int IMS_PR_ROAD_CODE { get; set; }
        public string ITEM_EXEC_STATUS { get; set; }
        public string CCP_TYPE { get; set; }
        public string CONCRETE_GRADE_ASPER_DPR { get; set; }
        public Nullable<decimal> CC_SR_PROPOSED_LENGTH { get; set; }
        public Nullable<decimal> CC_SR_EXECUTED_LENGTH { get; set; }
        public string IS_CC_CORE_STRENGTH_ACCEPTABLE { get; set; }
        public string IS_EXPANS_CONCTRUCT_PROVIDED { get; set; }
        public string IS_CUTS_JOINTS_ACCEPTABLE { get; set; }
        public string IS_JOINTS_FILLED { get; set; }
        public string IS_SURFACE_TEXTURE_ACCEPTABLE { get; set; }
        public string IS_EDGES_FREE { get; set; }
        public string IS_CAMBER_PROVIDED { get; set; }
        public string IS_CC_PAVEMENT_EXIST { get; set; }
        public string ITEM_GRADING_18 { get; set; }
        public string IMPROVE_SUGGESTIONS { get; set; }
        public int QM_USER_ID { get; set; }
        public string IPADD { get; set; }
    
        public virtual UM_User_Master UM_User_Master { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EFORM_QM_CHILD_CC_AND_SR_PAVEMENTS_OBSERVATION_DETAILS> EFORM_QM_CHILD_CC_AND_SR_PAVEMENTS_OBSERVATION_DETAILS { get; set; }
        public virtual EFORM_MASTER EFORM_MASTER { get; set; }
        public virtual IMS_SANCTIONED_PROJECTS IMS_SANCTIONED_PROJECTS { get; set; }
    }
}
