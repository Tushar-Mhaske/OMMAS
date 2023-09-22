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
    
    public partial class RCTRC_UM_User_Master
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RCTRC_UM_User_Master()
        {
            this.RCTRC_UM_User_Log = new HashSet<RCTRC_UM_User_Log>();
            this.RCTRC_UM_User_Profile = new HashSet<RCTRC_UM_User_Profile>();
            this.RCTRC_UM_User_Role_Mapping = new HashSet<RCTRC_UM_User_Role_Mapping>();
        }
    
        public int UserID { get; set; }
        public string UserName { get; set; }
        public short LevelID { get; set; }
        public short DefaultRoleID { get; set; }
        public Nullable<int> Mast_State_Code { get; set; }
        public Nullable<int> Mast_District_Code { get; set; }
        public Nullable<int> Admin_ND_Code { get; set; }
        public string Password { get; set; }
        public Nullable<int> FailedPasswordAttempts { get; set; }
        public Nullable<int> FailedPasswordAnswerAttempts { get; set; }
        public Nullable<System.DateTime> LastPasswordChangedDate { get; set; }
        public Nullable<System.DateTime> LastLoginDate { get; set; }
        public short PreferedLanguageID { get; set; }
        public short PreferedCssID { get; set; }
        public bool IsFirstLogin { get; set; }
        public bool IsLocked { get; set; }
        public bool IsActive { get; set; }
        public short ConcurrentLoginCount { get; set; }
        public Nullable<short> MaxConcurrentLoginsAllowed { get; set; }
        public System.DateTime CreationDate { get; set; }
        public string Remarks { get; set; }
        public int CreatedBy { get; set; }
    
        public virtual ADMIN_DEPARTMENT ADMIN_DEPARTMENT { get; set; }
        public virtual MASTER_STATE MASTER_STATE { get; set; }
        public virtual RCTRC_UM_Css_Master RCTRC_UM_Css_Master { get; set; }
        public virtual RCTRC_UM_Level_Master RCTRC_UM_Level_Master { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RCTRC_UM_User_Log> RCTRC_UM_User_Log { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RCTRC_UM_User_Profile> RCTRC_UM_User_Profile { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RCTRC_UM_User_Role_Mapping> RCTRC_UM_User_Role_Mapping { get; set; }
    }
}