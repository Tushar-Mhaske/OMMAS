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
    
    public partial class QUALITY_QM_LETTER
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public QUALITY_QM_LETTER()
        {
            this.QUALITY_QM_TEAM_LETTER = new HashSet<QUALITY_QM_TEAM_LETTER>();
        }
    
        public int LETTER_ID { get; set; }
        public Nullable<int> ADMIN_QM_CODE { get; set; }
        public Nullable<int> ADMIN_SQC_CODE { get; set; }
        public string TYPE { get; set; }
        public Nullable<int> ADMIN_SCHEDULE_CODE { get; set; }
        public short ADMIN_IM_MONTH { get; set; }
        public short ADMIN_IM_YEAR { get; set; }
        public string FILE_NAME { get; set; }
        public System.DateTime GENERATION_DATE { get; set; }
        public bool MAIL_DELIVERY_STATUS { get; set; }
        public Nullable<System.DateTime> MAIL_DELIVERY_DATE { get; set; }
        public Nullable<int> USERID { get; set; }
        public string IPADD { get; set; }
    
        public virtual ADMIN_QUALITY_MONITORS ADMIN_QUALITY_MONITORS { get; set; }
        public virtual ADMIN_SQC ADMIN_SQC { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<QUALITY_QM_TEAM_LETTER> QUALITY_QM_TEAM_LETTER { get; set; }
    }
}
