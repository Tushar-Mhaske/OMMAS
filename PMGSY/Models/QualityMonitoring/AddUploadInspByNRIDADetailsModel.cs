//using System;
using System.Collections.Generic;
//using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
//using PMGSY.Models;
using System.Web.Mvc;

namespace PMGSY.Models.QualityMonitoring
{
    public class AddUploadInspByNRIDADetailsModel
    {
        public string IMS_PR_ROAD_CODE { get; set; }

        public string FILE_NAME { get; set; }

        public string FILE_PATH { get; set; }

        [Display(Name = "Upload PDF File : ")]
        [Required(ErrorMessage = "Pdf file is Required")]
        [DataType(DataType.Upload)]
        public HttpPostedFileBase File { get; set; }

        [Required(ErrorMessage = "Inspection date is required")]
        [Display(Name = "Inspection Date : ")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Issue Date must be in dd/mm/yyyy format.")]
        public System.DateTime InspectionDate { get; set; }

        public string UploadedDate { get; set; }

        [Display(Name = "First Name : ")]
        [RegularExpression("^[a-zA-Z .]+$", ErrorMessage = "Name should contain alphabets only.")]
        [StringLength(40, ErrorMessage = "Name must be of 40 characters only.")]
        [Required(ErrorMessage = "First name is required")]
        public string FIRST_NAME { get; set; }

        [Display(Name = "Middle Name : ")]
        [RegularExpression("^[a-zA-Z .]+$", ErrorMessage = "Name should contain alphabets only.")]
        [StringLength(40, ErrorMessage = "Name must be of 40 characters only.")]
        public string MIDDLE_NAME { get; set; }

        [Display(Name = "Last Name : ")]
        [RegularExpression("^[a-zA-Z ]+$", ErrorMessage = "Name should contain alphabets only.")]
        [StringLength(40, ErrorMessage = "Name must be of 40 characters only.")]
        [Required(ErrorMessage = "Last name is required")]
        public string LAST_NAME { get; set; }

        [Display(Name = "Designation : ")]
        [RegularExpression(@"^[a-zA-Z ,.()]+$", ErrorMessage = "Only Alphanumeric, Space and , () and . are allowed")]
        [StringLength(60, ErrorMessage = "Designation must be of 60 characters only.")]
        [Required(ErrorMessage = "Designation is required")]
        public string DESIGNATION { get; set; }

        [Display(Name = "Inspection Grade : ")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Grade is required")]
        public string GRADE { get; set; }

        public List<SelectListItem> GRADE_LIST { get; set; }

        public string ACCEPTED_STATUS { get; set; }
        public string ACCEPTED_STATUS_NRIDA { get; set; }
    }
}