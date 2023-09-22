using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace PMGSY.Areas.RCTRC.Models
{
    public class RCTRCRegistrationModel
    {
        [UIHint("Hidden")]
        public string EncryptedRCTRCCode { get; set; }


        //[LocalizedDisplayName("lblState")]
        [Display(Name = "State")]
        [Required(ErrorMessage = "Please select State. ")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select State.")]
        // [RegularExpression(@"^([0-9]+)$", ErrorMessage = "State must be valid number.")]
        public int StateCode { get; set; }
        public List<SelectListItem> StateList { get; set; }


        //[LocalizedDisplayName("lblDistrict")]
        [Display(Name = "District")]
        //[Required(ErrorMessage = "Please select District.")]
        //[Range(0, int.MaxValue, ErrorMessage = "Please select District.")]
        //[RegularExpression(@"^([0-9]+)$", ErrorMessage = "District must be valid number.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select District.")]
        public int DistrictCode { get; set; }
        public List<SelectListItem> DistrictList { get; set; }


        [Display(Name = "Mobile")]
        [Required(ErrorMessage = "Enter Mobile Number.")]
        [RegularExpression("^[0-9]{10,11}", ErrorMessage = "Enter 10 digits Mobile number.")]
        [StringLength(10, ErrorMessage = "Mobile number must be 10 digits.")]
        public string MOBILE { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Enter Email ID.")]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Email address is not in valid format.")]
        public string EMAIL { get; set; }

        [Display(Name = "Period of Deputation / Contract ( In Month(s) )")]
        [RegularExpression(@"^([0-9-]+)$", ErrorMessage = "Enter correct Period of Deputation.")]
        public int DEPUTATION { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "Please enter Name.")]
        [RegularExpression(@"^([a-zA-Z .-]+)$", ErrorMessage = "Only Alphabets are allowed")]
        [StringLength(100, ErrorMessage = "Name must be less than 100 characters.")]
        public string CONTACT_NAME { get; set; }

        [Display(Name = "Date of Birth")]
        [Required(ErrorMessage = "Select  Date of Birth")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Date is not in valid format")]
        public string DOB { get; set; }

        [Display(Name = "Date of Joining Services")]
        [Required(ErrorMessage = "Select Date of Joining Services")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Date is not in valid format")]
        public string JOINING_DATE { get; set; }

        //[Display(Name = "Designation")]
      //  [Range(1, int.MaxValue, ErrorMessage = "Select Designation.")]
        //public int DesignationCode { get; set; }
        //public List<SelectListItem> DesignationList { get; set; }



        //[Display(Name = "Contact Address")]
        //[RegularExpression("([a-zA-Z 0-9 _(),#$-.]{1,255})", ErrorMessage = "Address is not in valid format.")]
        //[StringLength(255, ErrorMessage = "Address must be less than 255 characters.")]
        //[Required(ErrorMessage = "Address is required.")]
        //public string MAST_CON_ADDR1 { get; set; }

        [Display(Name = "Designation")]
        [Required(ErrorMessage = "Please enter Designation.")]
        [RegularExpression(@"^([a-zA-Z .-]+)$", ErrorMessage = "Only Alphabets are allowed")]
        [StringLength(200, ErrorMessage = "Designation must be less than 200 characters.")]
        public string DesignationText { get; set; }


        [Display(Name = "Graduation")]
        [Range(1, int.MaxValue, ErrorMessage = "Select Graduation.")]
        public int GraduationCode { get; set; }
        public List<SelectListItem> GraduationList { get; set; }

        [Display(Name = "Date of Graduation Completion")]
        [Required(ErrorMessage = "Select  Date of Graduation Completion")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Date is not in valid format")]
        public string GraduationDate { get; set; }


        [Display(Name = "Post Graduation")]
        [Range(1, int.MaxValue, ErrorMessage = "Select Post Graduation.")]
        public int PostGraduationCode { get; set; }
        public List<SelectListItem> PostGraduationList { get; set; }


        [Display(Name = "Date of Post Graduation Completion")]
        [Required(ErrorMessage = "Select  Date of Post Graduation Completion")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Date is not in valid format")]
        public string PostGraduationDate { get; set; }

        [Display(Name = "Year")]
        [Range(0, int.MaxValue, ErrorMessage = "Select valid Year.")]
        public int Year { get; set; }
        public List<SelectListItem> YearList { get; set; }



        [Display(Name = "Computer at Home")]
        [RegularExpression(@"^([YN]+)$", ErrorMessage = "Invalid option is selected.")]
        public string ComputerAtHomeCode { get; set; }
        public List<SelectListItem> ComputerAtHomeCodeList { get; set; }

        [Display(Name = "Computer at Office")]
        [RegularExpression(@"^([YN]+)$", ErrorMessage = "Invalid option is selected.")]
        public string ComputerAtOfficeCode { get; set; }
        public List<SelectListItem> ComputerAtOfficeList { get; set; }


        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        //[StringLength(32, MinimumLength = 8, ErrorMessage = "Password should contain more than 8 characters")]
        public string Password { get; set; }

    }
}