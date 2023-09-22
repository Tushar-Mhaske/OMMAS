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
    
    public partial class IMS_GENERATED_INVOICE_PTA
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public IMS_GENERATED_INVOICE_PTA()
        {
            this.IMS_PTA_PAYMENTS = new HashSet<IMS_PTA_PAYMENTS>();
        }
    
        public long IMS_INVOICE_ID { get; set; }
        public int MAST_STATE_CODE { get; set; }
        public int IMS_YEAR { get; set; }
        public byte MAST_PMGSY_SCHEME { get; set; }
        public int IMS_BATCH { get; set; }
        public int IMS_STREAM { get; set; }
        public string IMS_PROPOSAL_TYPE { get; set; }
        public string PTA_SANCTIONED_BY { get; set; }
        public string SAS_ABBREVATION { get; set; }
        public decimal HONORARIUM_AMOUNT { get; set; }
        public decimal PENALTY_AMOUNT { get; set; }
        public int MAST_TDS_ID { get; set; }
        public decimal TDS_AMOUNT { get; set; }
        public decimal SC_AMOUNT { get; set; }
        public decimal SERVICE_TAX_AMOUNT { get; set; }
        public decimal TOTAL_AMOUNT { get; set; }
        public System.DateTime GENERATION_DATE { get; set; }
        public string INVOICE_FILE_NO { get; set; }
        public Nullable<int> USERID { get; set; }
        public string IPADD { get; set; }
    
        public virtual MASTER_BATCH MASTER_BATCH { get; set; }
        public virtual MASTER_FUNDING_AGENCY MASTER_FUNDING_AGENCY { get; set; }
        public virtual MASTER_STATE MASTER_STATE { get; set; }
        public virtual MASTER_TAXES MASTER_TAXES { get; set; }
        public virtual MASTER_YEAR MASTER_YEAR { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IMS_PTA_PAYMENTS> IMS_PTA_PAYMENTS { get; set; }
    }
}
