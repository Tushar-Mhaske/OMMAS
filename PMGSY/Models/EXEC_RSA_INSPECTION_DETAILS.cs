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
    
    public partial class EXEC_RSA_INSPECTION_DETAILS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EXEC_RSA_INSPECTION_DETAILS()
        {
            this.EXEC_RSA_INSPECTION_ATR = new HashSet<EXEC_RSA_INSPECTION_ATR>();
        }
    
        public int EXEC_RSA_ID { get; set; }
        public int EXEC_RSA_CODE { get; set; }
        public decimal EXEC_RSA_START_CHAINAGE { get; set; }
        public decimal EXEC_RSA_END_CHAINAGE { get; set; }
        public Nullable<decimal> EXEC_SEGMENT_LENGTH { get; set; }
        public string EXEC_RSA_SAFETY_ISSUE { get; set; }
        public string EXEC_RSA_FILE_NAME { get; set; }
        public string EXEC_RSA_FILE_DESC { get; set; }
        public string EXEC_RSA_RECOMMENDATION { get; set; }
        public string EXEC_RSA_GRADE { get; set; }
        public string EXEC_RSA_FREQUENCY { get; set; }
        public string EXEC_RSA_PRIORITY { get; set; }
        public Nullable<int> ISSUE_CODE { get; set; }
    
        public virtual EXEC_RSA_INSPECTION EXEC_RSA_INSPECTION { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EXEC_RSA_INSPECTION_ATR> EXEC_RSA_INSPECTION_ATR { get; set; }
        public virtual RSA_MASTER_ISSUE RSA_MASTER_ISSUE { get; set; }
    }
}
