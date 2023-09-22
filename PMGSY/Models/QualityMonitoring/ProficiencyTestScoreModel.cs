using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.QualityMonitoring
{
    public class ProficiencyTestScoreModel
    {
        public int ID { get; set; }

        public int EXAM_ID { get; set; }

        [Display(Name = "Monitor Name : ")]
        [Required(ErrorMessage = "Monitor Name is required")]
        public string MONITOR_NAME { get; set; }

        public List<SelectListItem> MONITORS_LIST { get; set; }

        public int ADMIN_QM_CODE { get; set; }

        [Required(ErrorMessage = "Date of Exam is required")]
        [Display(Name = "Date of Exam : ")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Date must be in dd/mm/yyyy format.")]
        public System.DateTime DATE_OF_EXAM { get; set; }

        [Display(Name = "Marks : ")]
        [Required(ErrorMessage = "Marks is required")]
        [RegularExpression(@"(^([0-5]?[0-9]|60)$)", ErrorMessage = "Marks must be between 0 and 60.")]
        //[RegularExpression(@"(^60(\.0{1,2})?$)|(^([0-5]*([0-9])?|0)(\.[0-9]{1,2})?$)", ErrorMessage = "Marks must be between 0.00 and 60.00.")]
        public decimal? PERCENTAGE { get; set; }

        [Display(Name = "Monitor Type : ")]
        [Required(ErrorMessage = "Monitor type is required")]
        //[RegularExpression(@"^[IS]?", ErrorMessage = "Type of monitor is required.")]
        public string NQM_SQM { get; set; }

        public List<SelectListItem> NQM_SQM_LIST { get; set; }

        [Display(Name = "Remark")]
        [StringLength(250, ErrorMessage = "Maximum 250 Characters Allowed")]
        [RegularExpression(@"^[a-zA-Z0-9-/. ]+$", ErrorMessage = "Only Alphanumeric, Space and '-','/','.' Allowed")]
        public String CQC_REMARK { set; get; }

        public int ADD_EDIT { get; set; }

        [Display(Name = "State : ")]
        public int STATE_CODE { get; set; }

        public List<SelectListItem> STATE_LIST { get; set; }

        [Display(Name = "Institution Name : ")]
        //[RegularExpression(@"^\s*$", ErrorMessage = "Only Alphanumeric, Space and '-','/','.' Allowed")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select a valid institution")]
        public string INSTITUTION { get; set; }

        public List<SelectListItem> INSTITUTION_LIST { get; set; }

        [Display(Name = "Upload Score File: ")]
        [Required(ErrorMessage = "File is Required")]
        [DataType(DataType.Upload)]
        public HttpPostedFileBase UPLOADED_FILE { get; set; }

        [Required(ErrorMessage = "Monitor's exam status is required")]
        [Display(Name = "Exam Status : ")]
        //[Range(0, int.MaxValue, ErrorMessage = "Please select a valid exam status")]
        public string MONITOR_STATUS { get; set; }

        public List<SelectListItem> EXAM_STATUS_LIST { get; set; }
    }
}