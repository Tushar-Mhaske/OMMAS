using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using CaptchaLib;

//using PMGSY.CustomValidators;

namespace PMGSY.Models
{

    public class LoginModel
    {
        [Required]
        [Display(Name = "User name")]
        [RegularExpression(@"[A-Za-z][A-Za-z0-9._@]{0,30}", ErrorMessage = "Invalid User Name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        //[StringLength(32, MinimumLength = 8, ErrorMessage = "Password should contain more than 8 characters")]
        public string Password { get; set; }

        public Int32 DefaultRoleId { get; set; }

        //Added By Abhishek kamble 24-Apr-2014 for Captch
        //[Required(ErrorMessage="Please enter the text")]
        [ValidateCaptcha(ErrorMessage="Incorrect Text, Please try again")]
        public string Captcha { get; set; }   
        public bool? ShowCaptcha { get; set; }
        public bool? ValidateCaptcha { get; set; }


    }

    public class LoginRoleModel
    {
        public int RoleId { get; set; }
        public List<System.Web.Mvc.SelectListItem> RoleList { get; set; }
    }

    public class UserAuthModel
    {
        public bool IsUserLocked { get; set; }
        public bool IsFirstLogin { get; set; }
        public bool IsUserExist { get; set; }
        public bool IsValidPwdr { get; set; }
        public bool IsException { get; set; }
        public bool IsPwdrQuestionWrong { get; set; }
        public bool IsPwdrAnswerWrong { get; set; }
        public bool IsQuestionSelected { get; set; }

        public int UserId { get; set; }
        public string UserName { get; set; }
        //public int RoleId { get; set; }

        public bool IsOldPasswordWrong { get; set; }
        public bool IsOldAndNewPwdrSame { get; set; }
        public bool IsPwdrSameAsUName { get; set; }
        public bool IsConcurrentLoginsExceeded { get; set; }
        public bool IsActive { get; set; }

        public bool IsConcurrentLoginUpdated { get; set; }

        public string Message { get; set; }

        //Added By Abhishek kamble 25-Apr-2014 to show captcha
        public bool isCaptchaRequired { get; set; }
    }


    public class RecoverPwdrQuestionModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Question")]
        public Int32 PwdrQuestionId { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9 ,.()-_]+$", ErrorMessage = "Invalid Answer")]
        [Display(Name = "Answer")]
        public string PwdrAnswer { get; set; }

        //Added By Abhishek kamble 25-Apr-2014 for Captch
        [Required(ErrorMessage = "Please enter the text")]
        [ValidateCaptcha(ErrorMessage = "Incorrect Text, Please try again")]
        public string Captcha { get; set; } 
    }


    public class UserMasterModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
    }

    public class RecoverPasswordModel
    {
        public int UserId { get; set; }

        [Display(Name = "User name")]
        public string UserName { get; set; }

        public int RoleId { get; set; }

        [Required]
        [StringLength(32, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        //[RegularExpression(@"(?=^.{8,}$)((?=.*\d)|(?=.*\W+))(?![.\n])(?=.*[A-Z])(?=.*[a-z]).*$", ErrorMessage = "Password should contain at least 1 uppercase letter,<br /> 1 lowercase letter, 1 number or special character and 8 characters in length.")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }


    public class ChangePasswordModel
    {
        public int UserId { get; set; }

        [Display(Name = "User name")]
        public string UserName { get; set; }

        public int RoleId { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(32, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }


        [Required]
        [Display(Name = "Question")]
        public Int16 PwdrQuestionId { get; set; }
        public List<System.Web.Mvc.SelectListItem> QuestionList { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9 ,.()-_]+$", ErrorMessage = "Invalid Answer")]
        [Display(Name = "Answer")]
        [StringLength(30, MinimumLength = 1, ErrorMessage = "Answer must be greater than 1 and less than 30 characters.")]
        public string PwdrAnswer { get; set; }                                                       
         
    }


    public class NewUserDetailsModel
    {
        //[Required]
        //[Display(Name = "User Id")]
        //public string UserId { get; set; }

        [Required]
        [Display(Name = "Login name")]
        [RegularExpression(@"[A-Za-z][A-Za-z0-9._@]{3,30}", ErrorMessage = "Invalid User Name")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "First name")]
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$", ErrorMessage = "First Name should contain alphabets only (Max. characters 40)")]
        public string FName { get; set; }


        [Display(Name = "Middle name")]
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$", ErrorMessage = "Middle Name should contain alphabets only (Max. characters 40)")]
        public string MName { get; set; }

        [Required]
        [Display(Name = "Last name")]
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$", ErrorMessage = "Last Name should contain alphabets only (Max. characters 40)")]
        public string LName { get; set; }

        //[Required]
        [Display(Name = "Address")]
        [StringLength(40)]
        public string Address { get; set; }

        //[Required]
        [Display(Name = "Pin Code")]
        [RegularExpression(@"^[0-9]{1,6}$", ErrorMessage = "Pin Code should be in digits only (Max. characters 6)")]
        public string PinCode { get; set; }

        //[Required]
        [Display(Name = "Office STD")]
        [RegularExpression(@"^[0-9]{1,5}$", ErrorMessage = "Office STD should be in digits only (Max. characters 5)")]
        public string OfficeSTD { get; set; }

        //[Required]
        [Display(Name = "Office Phone No.")]
        [RegularExpression(@"^[0-9]{1,10}$", ErrorMessage = "Office Phone no. should be in digits only (Max. characters 10)")]
        public string OfficePhNo { get; set; }

        //[Required]
        [Display(Name = "Residence STD")]
        [RegularExpression(@"^[0-9]{1,5}$", ErrorMessage = "Residence STD should be in digits only (Max. characters 5)")]
        public string ResidenceSTD { get; set; }

        //[Required]
        [Display(Name = "Residence Phone No.")]
        [RegularExpression(@"^[0-9]{1,10}$", ErrorMessage = "Residence Phone no. should be in digits only (Max. characters 10)")]
        public string ResidencePhNo { get; set; }

        //[Required]
        [Display(Name = "Fax STD")]
        [RegularExpression(@"^[0-9]$", ErrorMessage = "Fax STD should be in digits only")]
        public string FaxSTD { get; set; }

        //[Required]
        [Display(Name = "Fax No.")]
        [RegularExpression(@"^[0-9]$", ErrorMessage = "Fax no. should be in digits only")]
        public string FaxNo { get; set; }

        [Required]
        [Display(Name = "Mobile No.")]
        [RegularExpression(@"^[0-9]{1,10}$", ErrorMessage = "Mobile no. should be in digits only (Max. characters 10)")]
        public string MobileNo { get; set; }

        [Required]
        [Display(Name = "Primary Email Id")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$", ErrorMessage = "Invalid Email Id")]
        public string PrimEmail { get; set; }

        //  [Required]
        [Display(Name = "Secondary Email Id")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$", ErrorMessage = "Invalid Email Id")]
        public string SecEmail { get; set; }

        [Required]
        //[DDL]
        public string StateId { get; set; }

        [Display(Name = "State Name (For This Account)")]
        public string StateName { get; set; }

        [Required]
        //[DDL]
        public string DistrictId { get; set; }

        [Required]
        [Display(Name = "District Name (For This Account)")]
        public string DistrictName { get; set; }

        [Required]
        [Display(Name = "Account Activation Date")]
        [DataType(DataType.Date)]
        public string AccActivateDate { get; set; }

        [Required]
        [Display(Name = "Account Deactivation Date")]
        [DataType(DataType.Date)]
        public string AccDeactivateDate { get; set; }

        [Required]
        public string PwdrRecoveryQuesId { get; set; }

        [Display(Name = "Password Recovery Question")]
        public string PwdrRecoveryQuesText { get; set; }

        [Required]
        [Display(Name = "Answer")]
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$", ErrorMessage = "Answer should contain alphabets & digits only (Max. characters 40)")]
        public string PwdrRecoveryAns { get; set; }

        [Required]
        [Display(Name = "Home page URL")]
        public string HomePgUrl { get; set; }

        [Required]
        [Display(Name = "Picture Path")]
        public string PictutrPath { get; set; }
    }


    public class AuthorizationModel
    {
        public Int16 RoleCode { get; set; }
        public String ActionURL { get; set; }
        public Int32 ActionId { get; set; }

        public String ActionName { get; set; }
        public String ControllerName { get; set; }
    }

    public class TendorPublicationReportModel
    {

        public int LevelCode { get; set; }
        public string StateName { get; set; }
        public string State_Name { get; set; }

        public string DistName { get; set; }
        public string BlockName { get; set; }
        public string CollabName { get; set; }
        public string AgencyName { get; set; }
        public string StatusName { get; set; }
        public string YearName { get; set; }
        public string BatchName { get; set; }

        [Display(Name = "State")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid State.")]
        public int StateCode { get; set; }
        public List<System.Web.Mvc.SelectListItem> StateList { get; set; }

        [Display(Name = "Scheme")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Scheme.")]
        public int SchemeCode { get; set; }
        public List<System.Web.Mvc.SelectListItem> SchemeList { get; set; }

        [Display(Name = "District")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid District.")]
        public int DistrictCode { get; set; }
        public List<System.Web.Mvc.SelectListItem> DistrictList { get; set; }

        [Display(Name = "Block")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Block.")]
        public int BlockCode { get; set; }
        public List<System.Web.Mvc.SelectListItem> BlockList { get; set; }

        [Display(Name = "Collaboration")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Collaboration.")]
        public int CollaborationCode { get; set; }
        public List<System.Web.Mvc.SelectListItem> CollaborationList { get; set; }

        [Display(Name = "Agency")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Agency.")]
        public int AgencyCode { get; set; }
        public List<System.Web.Mvc.SelectListItem> AgencyList { get; set; }

        //public string AgencyName { get; set; }

        [Display(Name = "Year")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Year.")]
        public int Year { get; set; }
        public List<System.Web.Mvc.SelectListItem> YearList { get; set; }

        [Display(Name = "Proposal")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Proposal.")]
        public int Proposal { get; set; }
        public List<System.Web.Mvc.SelectListItem> ProposalList { get; set; }

        [Display(Name = "Type")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Type.")]
        public int Type { get; set; }
        public List<System.Web.Mvc.SelectListItem> TypeList { get; set; }

        [Display(Name = "Status")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Status.")]
        public int Status { get; set; }
        public List<System.Web.Mvc.SelectListItem> StatusList { get; set; }

        [Display(Name = "Batch")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Batch.")]
        public int BatchCode { get; set; }
        public List<System.Web.Mvc.SelectListItem> BatchList { get; set; }
    }
   

}