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
    
    public partial class EFORM_QM_ROAD_FURNITURE_MARKINGS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EFORM_QM_ROAD_FURNITURE_MARKINGS()
        {
            this.EFORM_QM_CHILD_ROAD_FURNITURE_MARKINGS_OBSERVATION_DETAILS = new HashSet<EFORM_QM_CHILD_ROAD_FURNITURE_MARKINGS_OBSERVATION_DETAILS>();
        }
    
        public int MARK_ID { get; set; }
        public int EFORM_ID { get; set; }
        public int IMS_PR_ROAD_CODE { get; set; }
        public string IS_MAIN_INFO_BOARD_FIXED { get; set; }
        public string IS_CITIZEN_INFO_BOARD_FIXED { get; set; }
        public string IS_MAINTANANCE_BOARD_FIXED { get; set; }
        public string IS_BOARD_INFO_IN_LOCAL_LANG { get; set; }
        public string ITEM_GRADING_20 { get; set; }
        public string IMPROVE_SUGGESTIONS { get; set; }
        public int QM_USER_ID { get; set; }
        public string IPADD { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EFORM_QM_CHILD_ROAD_FURNITURE_MARKINGS_OBSERVATION_DETAILS> EFORM_QM_CHILD_ROAD_FURNITURE_MARKINGS_OBSERVATION_DETAILS { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
        public virtual EFORM_MASTER EFORM_MASTER { get; set; }
        public virtual IMS_SANCTIONED_PROJECTS IMS_SANCTIONED_PROJECTS { get; set; }
    }
}
