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
    
    public partial class QM_IR_ATR_DETAILS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public QM_IR_ATR_DETAILS()
        {
            this.QM_IR_ATR_DETAILS1 = new HashSet<QM_IR_ATR_DETAILS>();
        }
    
        public int INSPECTION_ID { get; set; }
        public int IMS_PR_ROAD_CODE { get; set; }
        public string FILE_NAME { get; set; }
        public string FILE_PATH { get; set; }
        public System.DateTime FILE_UPLOADED_DATE { get; set; }
        public Nullable<System.DateTime> INSPECTION_DATE { get; set; }
        public string IS_STATE_OR_NRIDA { get; set; }
        public string IS_FINALIZE { get; set; }
        public string GRADE { get; set; }
        public Nullable<int> NRIDA_INSPECTION_ID { get; set; }
        public string FIRST_NAME { get; set; }
        public string MIDDLE_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public string DESIGNATION { get; set; }
        public string IS_ACCEPTED { get; set; }
        public string ACCEPT_REJECT_REMARK { get; set; }
        public Nullable<System.DateTime> ACCEPT_REJECT_DATE { get; set; }
        public Nullable<int> USERID { get; set; }
        public string IPADD { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<QM_IR_ATR_DETAILS> QM_IR_ATR_DETAILS1 { get; set; }
        public virtual QM_IR_ATR_DETAILS QM_IR_ATR_DETAILS2 { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
        public virtual IMS_SANCTIONED_PROJECTS IMS_SANCTIONED_PROJECTS { get; set; }
    }
}
