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
    
    public partial class EFORM_TR_TABLE_MASTER
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EFORM_TR_TABLE_MASTER()
        {
            this.EFORM_TR_TYPEA_SUMMARY = new HashSet<EFORM_TR_TYPEA_SUMMARY>();
            this.EFORM_TR_TYPEB_DETAIL = new HashSet<EFORM_TR_TYPEB_DETAIL>();
            this.EFORM_TR_TYPEB_SUMMARY = new HashSet<EFORM_TR_TYPEB_SUMMARY>();
            this.EFORM_TR_TYPEC_DETAIL = new HashSet<EFORM_TR_TYPEC_DETAIL>();
            this.EFORM_TR_TYPEC_SUMMARY = new HashSet<EFORM_TR_TYPEC_SUMMARY>();
        }
    
        public int TABLE_ID { get; set; }
        public int SUBITEM_ID { get; set; }
        public string TABLE_CODE { get; set; }
        public string TABLE_HEADING { get; set; }
        public Nullable<int> USER_ID { get; set; }
        public string IPADD { get; set; }
    
        public virtual EFORM_TR_SUBITEM_MASTER EFORM_TR_SUBITEM_MASTER { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EFORM_TR_TYPEA_SUMMARY> EFORM_TR_TYPEA_SUMMARY { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EFORM_TR_TYPEB_DETAIL> EFORM_TR_TYPEB_DETAIL { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EFORM_TR_TYPEB_SUMMARY> EFORM_TR_TYPEB_SUMMARY { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EFORM_TR_TYPEC_DETAIL> EFORM_TR_TYPEC_DETAIL { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EFORM_TR_TYPEC_SUMMARY> EFORM_TR_TYPEC_SUMMARY { get; set; }
    }
}
