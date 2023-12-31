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
    
    public partial class EXEC_RSA_INSPECTION
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EXEC_RSA_INSPECTION()
        {
            this.EXEC_RSA_INSPECTION_DETAILS = new HashSet<EXEC_RSA_INSPECTION_DETAILS>();
        }
    
        public int EXEC_RSA_CODE { get; set; }
        public int IMS_PR_ROAD_CODE { get; set; }
        public string IMS_ROAD_STATUS { get; set; }
        public System.DateTime EXEC_RSA_INSP_DATE { get; set; }
        public string EXEC_RSA_AUDIT_SUB { get; set; }
        public Nullable<System.DateTime> EXEC_RSA_AUDIT_SUB_DATE { get; set; }
        public string EXEC_RSA_PIU_SUB { get; set; }
        public Nullable<System.DateTime> EXEC_RSA_PIU_SUB_DATE { get; set; }
        public string EXEC_FILE_NAME { get; set; }
        public string EXEC_FILE_DESC { get; set; }
        public Nullable<int> USERID { get; set; }
        public string IPADD { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EXEC_RSA_INSPECTION_DETAILS> EXEC_RSA_INSPECTION_DETAILS { get; set; }
        public virtual IMS_SANCTIONED_PROJECTS IMS_SANCTIONED_PROJECTS { get; set; }
    }
}
