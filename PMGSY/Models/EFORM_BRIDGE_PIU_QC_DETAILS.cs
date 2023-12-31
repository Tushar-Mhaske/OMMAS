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
    
    public partial class EFORM_BRIDGE_PIU_QC_DETAILS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EFORM_BRIDGE_PIU_QC_DETAILS()
        {
            this.EFORM_BRIDGE_QC_OFFICIAL_DETAILS = new HashSet<EFORM_BRIDGE_QC_OFFICIAL_DETAILS>();
        }
    
        public int QC_ID { get; set; }
        public Nullable<int> EFORM_ID { get; set; }
        public Nullable<int> ADMIN_ND_CODE { get; set; }
        public Nullable<int> PR_ROAD_CODE { get; set; }
        public string LAB_LOCATION { get; set; }
        public Nullable<System.DateTime> PHOTO_UPLOAD_DATE { get; set; }
        public string ESTB_DELAY_REASON { get; set; }
        public string LAB_EQUIP_AVBL { get; set; }
        public string EQUIP_WORKING { get; set; }
        public string EQUIP_NOT_WORKING { get; set; }
        public string LAB_EQUIP_NOT_AVBL { get; set; }
        public string REASON_LAB_EQUIP_NOT_AVBL { get; set; }
        public string CALIBRATION_DETAILS { get; set; }
        public string DOCUMENT_FOR_QM { get; set; }
        public Nullable<int> USER_ID { get; set; }
        public string IPADD { get; set; }
    
        public virtual ADMIN_DEPARTMENT ADMIN_DEPARTMENT { get; set; }
        public virtual EFORM_MASTER EFORM_MASTER { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EFORM_BRIDGE_QC_OFFICIAL_DETAILS> EFORM_BRIDGE_QC_OFFICIAL_DETAILS { get; set; }
        public virtual IMS_SANCTIONED_PROJECTS IMS_SANCTIONED_PROJECTS { get; set; }
    }
}
