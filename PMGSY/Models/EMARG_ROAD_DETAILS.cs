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
    
    public partial class EMARG_ROAD_DETAILS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EMARG_ROAD_DETAILS()
        {
            this.MANE_EMARG_CONTRACT = new HashSet<MANE_EMARG_CONTRACT>();
        }
    
        public int EMARG_ID { get; set; }
        public string MAST_STATE_NAME { get; set; }
        public string MAST_DISTRICT_NAME { get; set; }
        public string PACKAGE_NO { get; set; }
        public string CONTRACTOR { get; set; }
        public string PAN { get; set; }
        public string AGREEMENT_NO { get; set; }
        public string AGREEMENT_DATE { get; set; }
        public int ROAD_CODE { get; set; }
        public string ROAD_NAME { get; set; }
        public string COMPLETION_DATE { get; set; }
        public string SANCTIONED_LENGTH { get; set; }
        public string COMPLETED_LENGTH { get; set; }
        public string CARRIAGE_WIDTH { get; set; }
        public string TRAFFIC_DENSITY { get; set; }
        public string REMARKS { get; set; }
        public string EMARG_STATUS { get; set; }
        public Nullable<System.DateTime> DATA_RECEIPT_DATE { get; set; }
        public string OMMAS_REPUSHING_STATUS { get; set; }
        public Nullable<System.DateTime> DATE_OF_REPUSHING { get; set; }
        public string REJECTION_REASON { get; set; }
        public string EMARG_STATUS1 { get; set; }
        public Nullable<int> EMARG_STATUS1_REJECTION_CODE { get; set; }
        public Nullable<System.DateTime> EMARG_STATUS1_ACK_DATE { get; set; }
        public string EMARG_STATUS2 { get; set; }
        public Nullable<int> EMARG_STATUS2_REJECTION_CODE { get; set; }
        public Nullable<System.DateTime> EMARG_STATUS2_ACK_DATE { get; set; }
        public string IS_DEACTIVATED { get; set; }
        public string IS_REINSERTED { get; set; }
        public Nullable<int> DLP_TYPE { get; set; }
    
        public virtual EMARG_REJECTION_MASTER EMARG_REJECTION_MASTER { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MANE_EMARG_CONTRACT> MANE_EMARG_CONTRACT { get; set; }
        public virtual IMS_SANCTIONED_PROJECTS IMS_SANCTIONED_PROJECTS { get; set; }
    }
}
