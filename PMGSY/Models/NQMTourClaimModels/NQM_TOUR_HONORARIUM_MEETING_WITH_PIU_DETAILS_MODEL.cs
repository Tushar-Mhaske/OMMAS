using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.NQMTourClaimModels
{
    public class NQM_TOUR_HONORARIUM_MEETING_WITH_PIU_DETAILS_MODEL
    {
        public int HONORARIUM_MEETING_ID { get; set; }

        public int TOUR_CLAIM_ID { get; set; }

        public int ADMIN_SCHEDULE_CODE { get; set; }

        [Display(Name = "Date of Meeting: ")]
        [Required(ErrorMessage = "Date Of meeting is required")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Date of meeting must be in dd/mm/yyyy format.")]
        public System.DateTime DATE_OF_MEETING { get; set; }

        [Display(Name = "Amount Claimed: ")]
        [Required(ErrorMessage = "Amount Claim is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Amount Claim is not in valid format. ")]
        [Range(0, 999999.99, ErrorMessage = "Amount Claim is not in valid format.")]
        public decimal AMOUNT_CLAIMED { get; set; }

        [Display(Name = "Date of Claim: ")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Date of claim must be in dd/mm/yyyy format.")]
        public string DATE_OF_CLAIM { get; set; }

        [Display(Name = "Amount Passed by NRRDA: ")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Amount Passed is not in valid format. ")]
        [Range(0, 999999.99, ErrorMessage = "Amount Passed is not in valid format.")]
        public Nullable<decimal> AMOUNT_PASSED { get; set; }

        [Display(Name = "Date of amount passed by NDDRA: ")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Date of passing must be in dd/mm/yyyy format.")]
        public Nullable<System.DateTime> DATE_OF_PASSING { get; set; }

        [Display(Name = "Remark: ")]
        [StringLength(255, ErrorMessage = "Remark must be less than 255 characters.")]
        [RegularExpression(@"^([a-zA-Z0-9 ._',&()-]+)$", ErrorMessage = "Remark is not in valid format.")]
        public string REMARK { get; set; }

        [Display(Name = "Place of meeting: ")]
        [Required(ErrorMessage = "Please select State.")]
        [Range(1, 2147483647, ErrorMessage = "Please select State.")]
        public int STATE_CODE { get; set; }

        public List<SelectListItem> lstStates { set; get; }

        [Required(ErrorMessage = "Please select Distict.")]
        [Range(1, 2147483647, ErrorMessage = "Please select Distict.")]
        public int DISTRICT_CODE { get; set; }

        public List<SelectListItem> lstDistricts { set; get; }

        [Display(Name = "Upload Attendance Sheet: ")]
        [Required(ErrorMessage = "File is Required")]
        [DataType(DataType.Upload)]
        public HttpPostedFileBase ATTENDANCE_SHEET { get; set; }

        [Display(Name = "Upload Meeting Details: ")]
        [Required(ErrorMessage = "File is Required")]
        [DataType(DataType.Upload)]
        public HttpPostedFileBase MEETING_FILE { get; set; }

        [Display(Name = "Upload Geotagged Photograph: ")]
        [Required(ErrorMessage = "Photograph is Required")]
        [DataType(DataType.Upload)]
        public HttpPostedFileBase PHOTO_FILE { get; set; }

        public string UPLOADED_ATTENDANCE_SHEET_NAME { get; set; }

        public string DEC_UPLOADED_ATTENDANCE_SHEET_NAME { get; set; }

        public string UPLOADED_ATTENDANCE_SHEET_PATH { get; set; }

        public string UPLOADED_FILE_NAME { get; set; }

        public string DEC_UPLOADED_FILE_NAME { get; set; }

        public string UPLOADED_FILE_PATH { get; set; }

        public string UPLOADED_PHOTO_NAME { get; set; }

        public string DEC_UPLOADED_PHOTO_NAME { get; set; }

        public string UPLOADED_PHOTO_PATH { get; set; }

        public int ADD_EDIT { get; set; }

        public Nullable<int> USERID { get; set; }
        public string IPADD { get; set; }
    }
}