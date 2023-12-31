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
    
    public partial class ADMIN_NODAL_OFFICERS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ADMIN_NODAL_OFFICERS()
        {
            this.ACC_BILL_MASTER = new HashSet<ACC_BILL_MASTER>();
            this.ADMIN_NO_BANK = new HashSet<ADMIN_NO_BANK>();
            this.EXEC_OFFICER_DETAILS = new HashSet<EXEC_OFFICER_DETAILS>();
            this.MANE_IMS_INSPECTION = new HashSet<MANE_IMS_INSPECTION>();
        }
    
        public int ADMIN_NO_OFFICER_CODE { get; set; }
        public int ADMIN_ND_CODE { get; set; }
        public string ADMIN_NO_FNAME { get; set; }
        public string ADMIN_NO_MNAME { get; set; }
        public string ADMIN_NO_LNAME { get; set; }
        public int ADMIN_NO_DESIGNATION { get; set; }
        public string ADMIN_NO_ADDRESS1 { get; set; }
        public string ADMIN_NO_ADDRESS2 { get; set; }
        public Nullable<int> MAST_DISTRICT_CODE { get; set; }
        public string ADMIN_NO_PIN { get; set; }
        public string ADMIN_NO_RESIDENCE_STD { get; set; }
        public string ADMIN_NO_RESIDENCE_PHONE { get; set; }
        public string ADMIN_NO_OFFICE_STD { get; set; }
        public string ADMIN_NO_OFFICE_PHONE { get; set; }
        public string ADMIN_NO_STD_FAX { get; set; }
        public string ADMIN_NO_FAX { get; set; }
        public string ADMIN_NO_MOBILE { get; set; }
        public string ADMIN_NO_EMAIL { get; set; }
        public string ADMIN_NO_MAIL_FLAG { get; set; }
        public int ADMIN_NO_TYPE { get; set; }
        public string ADMIN_AUTH_CODE { get; set; }
        public string ADMIN_NO_LEVEL { get; set; }
        public string ADMIN_ACTIVE_STATUS { get; set; }
        public Nullable<System.DateTime> ADMIN_ACTIVE_START_DATE { get; set; }
        public Nullable<System.DateTime> ADMIN_ACTIVE_END_DATE { get; set; }
        public string ADMIN_NO_REMARKS { get; set; }
        public string ADMIN_MODULE { get; set; }
        public string ADMIN_AADHAR_NO { get; set; }
        public string ADMIN_AADHAR_PAN_FLAG { get; set; }
        public Nullable<bool> IS_VALID_XML { get; set; }
        public Nullable<System.DateTime> XML_FINALIZATION_DATE { get; set; }
        public Nullable<int> USERID { get; set; }
        public string IPADD { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ACC_BILL_MASTER> ACC_BILL_MASTER { get; set; }
        public virtual ACC_CERTIFICATE_DETAILS ACC_CERTIFICATE_DETAILS { get; set; }
        public virtual ADMIN_DEPARTMENT ADMIN_DEPARTMENT { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ADMIN_NO_BANK> ADMIN_NO_BANK { get; set; }
        public virtual MASTER_DESIGNATION MASTER_DESIGNATION { get; set; }
        public virtual MASTER_DISTRICT MASTER_DISTRICT { get; set; }
        public virtual MASTER_PROFILE MASTER_PROFILE { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EXEC_OFFICER_DETAILS> EXEC_OFFICER_DETAILS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MANE_IMS_INSPECTION> MANE_IMS_INSPECTION { get; set; }
    }
}
