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
    
    public partial class MASTER_INFO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MASTER_INFO()
        {
            this.QUALITY_QM_DIRECTORS = new HashSet<QUALITY_QM_DIRECTORS>();
        }
    
        public int MAST_INFO_CODE { get; set; }
        public string MAST_INFO_NAME { get; set; }
        public string MAST_INFO_DESIGNATION { get; set; }
        public string MAST_INFO_OFFICE { get; set; }
        public Nullable<int> MAST_STATE_CODE { get; set; }
        public string MAST_INFO_TELE_OFF { get; set; }
        public string MAST_INFO_TELE_RES { get; set; }
        public string MAST_INFO_MOBILE { get; set; }
        public string MAST_INFO_FAX { get; set; }
        public string MAST_INFO_EMAIL { get; set; }
        public string MAST_INFO_TYPE { get; set; }
        public string MAST_INFO_ACTIVE { get; set; }
        public int MAST_INFO_SORT { get; set; }
    
        public virtual MASTER_STATE MASTER_STATE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<QUALITY_QM_DIRECTORS> QUALITY_QM_DIRECTORS { get; set; }
    }
}
