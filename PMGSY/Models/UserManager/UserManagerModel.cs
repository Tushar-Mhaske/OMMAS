using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.UserManager
{
    public class UserManagerModel
    {

    }

    public class LevelRolesMappingDTO
    {
        public String RoleID { get; set; }
        public Boolean AddChildren { get; set; }
    }

    public class LevelRolesListDTO
    {
        public String RoleID { get; set; }
        public String RoleName { get; set; }
        public Int16 Level { get; set; }
        public String ParentId { get; set; }
        public Boolean IsLeaf { get; set; }
    }


    public class RoleLevelMapping
    {
        public int RoleID { get; set; }
        public string LevelID { get; set; }
        public string LevelStr { get; set; }
    }

    public class RoleDetailsModel
    {
        public Int32 RoleId { get; set; }

        [Display(Name = "Role")]
        public String RoleName { get; set; }

        public Int16 LevelId { get; set; }

        [Display(Name = "Level")]
        public String LevelName { get; set; }

        [Display(Name = "Active")]
        public String IsActive { get; set; }

        [Display(Name = "Remarks")]
        public String Remarks { get; set; }
    }

    public class RoleMenuMappingModel
    {
        public Int32 RoleId { get; set; }

        [Display(Name = "Role")]
        public String RoleName { get; set; }

        public Int32 MenuId { get; set; }

        [Display(Name = "Menus")]
        public String MenuName { get; set; }

        public Int32 ParentId { get; set; }

        [Display(Name = "Parent Menu")]
        public String ParentName { get; set; }

        public string RightsPermitted { get; set; }
    }


    public class RoleUserDetailsModel
    {
        [Display(Name = "User Id")]
        public Int32 UserId { get; set; }

        [Display(Name = "Full Name")]
        public String FullName { get; set; }

        [Display(Name = "User Name")]
        public String UserName { get; set; }

        [Display(Name = "City")]
        public String City { get; set; }

        [Display(Name = "Creation Date")]
        public String CreationDate { get; set; }

        [Display(Name = "Is Active")]
        public String IsActive { get; set; }
    }


    public class User_Master
    {
        public User_Master()
        {
            this.UM_User_Log = new HashSet<UM_User_Log>();
            this.UM_User_Profile = new HashSet<UM_User_Profile>();
            this.UM_User_Role_Mapping = new HashSet<UM_User_Role_Mapping>();
        }

        public int UserID { get; set; }
        public int hiddenSQMId { get; set; }
        [Display(Name = "User Name")]
        [Required]
        [RegularExpression(@"[A-Za-z][A-Za-z0-9._@]{3,30}", ErrorMessage = "Invalid username, Minimum length 4 characters, can be AlphaNumeric, can contain (. _ @) ")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "User Level")]
        [System.ComponentModel.DataAnnotations.Range(1, double.MaxValue, ErrorMessage = "Please select level")]
        [RegularExpression(@"^\d{0,2}(\.\d{1,2})?$", ErrorMessage = "Please Select A User Level")]
        public short LevelID { get; set; }

        public List<SelectListItem> LevelList { get; set; }
        public List<SelectListItem> RoleList { get; set; }
        public List<SelectListItem> StateList { get; set; }
        public List<SelectListItem> DistrictList { get; set; }
        public List<SelectListItem> DepartmentList { get; set; }
        public List<SelectListItem> CSSList { get; set; }
        public List<SelectListItem> LanguageList { get; set; }
        public List<SelectListItem> SQMList { get; set; }

        public string LevelName { get; set; }

        [Display(Name = "State")]
        public Nullable<int> Mast_State_Code { get; set; }

        public string Mast_State_Name { get; set; }

        [Display(Name = "District")]
        public Nullable<int> Mast_District_Code { get; set; }

        public string Mast_District_Name { get; set; }

        public string Password { get; set; }
        public Nullable<int> FailedPasswordAttempts { get; set; }
        public Nullable<int> FailedPasswordAnswerAttempts { get; set; }
        public Nullable<System.DateTime> LastPasswordChangedDate { get; set; }
        public Nullable<System.DateTime> LastLoginDate { get; set; }

        [Display(Name = "Default Language")]
        [Required]
        [Range(1, Int16.MaxValue, ErrorMessage = "Please Select Default Language.")]
        public short PreferedLanguageID { get; set; }

        public string Prefered_Language_Name { get; set; }

        [Display(Name = "Prefered Css")]
        [Required]
        [Range(1, Int16.MaxValue, ErrorMessage = "Please Select Prefered Css.")]
        public short PreferedCssID { get; set; }

        public string Prefered_Css_Name { get; set; }

        public bool IsFirstLogin { get; set; }
        public bool IsLocked { get; set; }
        public bool IsActive { get; set; }

        public short ConcurrentLoginCount { get; set; }

        [Display(Name = "Concurrent Logins Allowed")]
        [Required]
        [Range(1, Int16.MaxValue, ErrorMessage = "Invalid Concurrent Logins Value.")]
        [RegularExpression(@"^[1-9][0-9]*$", ErrorMessage = "Concurrent Logins Allowed must be greater than 0")]
        public Nullable<short> MaxConcurrentLoginsAllowed { get; set; }


        public System.DateTime CreationDate { get; set; }

        [RegularExpression(@"[A-Za-z ][A-Za-z0-9._@ ]{3,100}", ErrorMessage = "Invalid Remarks,Minimum Length 4 Characters,Can be AlphaNumeric,Can Contain (. _ @) ")]
        public string Remarks { get; set; }


        public int CreatedBy { get; set; }

        [Required]
        [Display(Name = "Role")]
        [Range(1, double.MaxValue, ErrorMessage = "Please select role")]
        public Int16 RoleID { get; set; }

        [Required]
        [Display(Name = "SQM")]
        [Range(1, double.MaxValue, ErrorMessage = "Please select monitor")]
        public Int32 SQMID { get; set; }

         
        [Display(Name = "Department")]
        public Nullable<int> Admin_ND_Code { get; set; }

        public string Admin_ND_Name { get; set; }

        public string Edit { get; set; }

        public virtual MASTER_STATE MASTER_STATE { get; set; }
        public virtual UM_Css_Master UM_Css_Master { get; set; }
        public virtual UM_Level_Master UM_Level_Master { get; set; }
        public virtual ICollection<UM_User_Log> UM_User_Log { get; set; }
        public virtual ICollection<UM_User_Profile> UM_User_Profile { get; set; }
        public virtual ICollection<UM_User_Role_Mapping> UM_User_Role_Mapping { get; set; }
        public virtual ADMIN_DEPARTMENT ADMIN_DEPARTMENT { get; set; }
    }

    public class Role_Master
    {
        [ScaffoldColumn(false)]
        public short RoleID { get; set; }

        [Required]
        [Display(Name = "Role Name")]
        [RegularExpression(@"[A-Za-z ][A-Za-z0-9_ ]{2,100}", ErrorMessage = "Invalid Role Name, Minimum Length 3 Characters,Can be AlphaNumeric,Can Contain ( _ ) ")]
        public string RoleName { get; set; }

        public bool IsActive { get; set; }

        [RegularExpression(@"[A-Za-z ][A-Za-z0-9._@ ]{0,100}", ErrorMessage = "Invalid Remarks, Can be AlphaNumeric,Can Contain (. _ @) ")]
        public string Remark { get; set; }

        public string LevelId { get; set; }
    }

    public class UserProfileModel
    {
        public string Name { get; set; }

        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Display(Name = "Level")]
        public string LevelName { get; set; }

        public string State { get; set; }
        public string District { get; set; }
        public string Department { get; set; }
        [Display(Name = "Css")]
        public string PreferredCss { get; set; }
        [Display(Name = "Language")]
        public string PreferredLanguage { get; set; }
        public string IsActive { get; set; }
    }


    public class RoleHomePageModel
    {
        [ScaffoldColumn(false)]
        public short ID { get; set; }

        [Required]
        [Display(Name = "Role")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Role field is required")]
        public Int32 RoleID { get; set; }

        [Required]
        [Display(Name = "Module")]
        [Range(1, Int16.MaxValue, ErrorMessage = "Module field is required")]
        public short ModuleID { get; set; }

        [Required]
        [Display(Name = "Sub Module")]
        [Range(1, Int16.MaxValue, ErrorMessage = "Sub Module field is required")]
        public short SubModuleID { get; set; }

        public List<SelectListItem> RoleList { get; set; }
        public List<SelectListItem> ModuleList { get; set; }
        public List<SelectListItem> SubModuleList { get; set; }
    }

    public class IndependentUsersModel
    {
        [Required]
        [Display(Name = "Role")]
        public int UserRoleID { get; set; }

        [Required]
        [Display(Name = "Username")]
        public int UserID { get; set; }

        [Required(ErrorMessage = "Monitor is required.")]
        [Display(Name = "Name")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select Monitor")]
        public int UserProfileID { get; set; }

        public List<SelectListItem> UsersList { get; set; }

        [Required(ErrorMessage = "State is required.")]
        [Display(Name = "State")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Please select State")]
        public int Mast_State{ get; set; }
        public List<SelectListItem> StateList { get; set; }

    }


    public class SwitchAdminAsUserModel
    {
        public Int16 UserId { get; set; }
        public List<SelectListItem> UsersList { get; set; }
    }

    public class USP_UM_GET_LOGIN_DETAILS_Report_Result
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public long LogID { get; set; }
        public string LoginDateTime { get; set; }
        public string LogoutDateTime { get; set; }
        public string IpAddress { get; set; }
        public string Duration { get; set; }
    }
    public class USP_UM_GET_USER_ACCESS_REPORT_DETAILS_Result
    {
        public int UserID { get; set; }
        public long AuditId { get; set; }
        public string UserName { get; set; }
        public string ModuleName { get; set; }
        public string RequestType { get; set; }
        public string URLAccessed { get; set; }
        public string IPAddress { get; set; }
        public string TimeStamp { get; set; }
    }


    public class USP_GET_VILLAGE_DETAILS_LIST_Result
    {
        public System.Guid ErrorId { get; set; }
        public string Application { get; set; }
        public string Host { get; set; }
        public string Type { get; set; }
        public string Source { get; set; }
        public string Message { get; set; }
        public string User { get; set; }
        public int StatusCode { get; set; }
        public System.DateTime TimeUtc { get; set; }
        public int Sequence { get; set; }
        public string AllXml { get; set; }
    }
    public class USP_UM_GET_USER_LOGIN_YEAR_Result
    {
        public int YEAR { get; set; }
    }

    public class AccountRefreshData
    {
        public bool Report { get; set; }

    }
}