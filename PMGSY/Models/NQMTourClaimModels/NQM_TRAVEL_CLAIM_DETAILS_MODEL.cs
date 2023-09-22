using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.NQMTourClaimModels
{
    public class NQM_TRAVEL_CLAIM_DETAILS_MODEL
    {
        public int TRAVEL_CLAIM_ID { get; set; }

        public int TOUR_CLAIM_ID { get; set; }

        public int ADMIN_SCHEDULE_CODE { get; set; }

        [Display(Name = "Start Date: ")]
        [Required(ErrorMessage = "Date Of travel is required")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Date of travel must be in dd/mm/yyyy format.")]
        public System.DateTime START_DATE_OF_TRAVEL { get; set; }

        [Display(Name = "Start Time: ")]
        [Required(ErrorMessage = "Start Time is required")]
        [Range(0, 23, ErrorMessage ="Hours is not in correct format.")]
        public int START_HOURS { get; set; }

        [Required(ErrorMessage = "Start Time is required")]
        [Range(0, 59, ErrorMessage = "Minutes is not in correct format.")]
        public int START_MINUTES { get; set; }

        //[Range(1, 2)]
        //public int START_AM_PM { get; set; }

        [Display(Name = "End Date: ")]
        [Required(ErrorMessage = "Date Of travel is required")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Date of travel must be in dd/mm/yyyy format.")]
        public System.DateTime END_DATE_OF_TRAVEL { get; set; }

        [Display(Name = "End Time: ")]
        [Required(ErrorMessage = "End Time is required")]
        [Range(0, 23, ErrorMessage = "Hours is not in correct format.")]
        public int END_HOURS { get; set; }

        [Required]
        [Range(0, 59, ErrorMessage = "Minutes is not in correct format.")]
        public int END_MINUTES { get; set; }

        //[Range(1, 2)]
        //public int END_AM_PM { get; set; }

        //public List<SelectListItem> LST_AM_PM { set; get; }

        [Display(Name = "Departure From: ")]
        [Required(ErrorMessage = "Please Enter Departure From.")]
        [StringLength(100, ErrorMessage = "Departure from must be less than 100 characters.")]
        [RegularExpression(@"^([a-zA-Z ._',&()-]+)$", ErrorMessage = "Departure from is not in valid format.")]
        public string DEPARTURE_FROM { get; set; }

        [Display(Name = "Arrival At: ")]
        [Required(ErrorMessage = "Please Enter Arrival At.")]
        [StringLength(100, ErrorMessage = "Arrival at must be less than 100 characters.")]
        [RegularExpression(@"^([a-zA-Z ._',&()-]+)$", ErrorMessage = "Arrival at is not in valid format.")]
        public string ARRIVAL_AT { get; set; }

        [Display(Name = "Mode Of Travel: ")]
        [Required(ErrorMessage = "Mode of travel is required.")]
        [Range(2, 10, ErrorMessage = "Mode of travel is required.")]
        public string MODE_OF_TRAVEL { get; set; }

        public List<SelectListItem> lstModes { set; get; }

        [Display(Name = "Travel Class: ")]
        [Required(ErrorMessage = "Travel class is required.")]
        public string TRAVEL_CLASS { get; set; }

        public List<SelectListItem> lstClass { set; get; }

        [Display(Name = "Amount Claimed: ")]
        [Required(ErrorMessage = "Amount Claim is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Amount Claim is not in valid format. ")]
        [Range(0, 999999.99, ErrorMessage = "Amount Claim is not in valid format.")]
        public decimal AMOUNT_CLAIMED { get; set; }

        [Display(Name = "Date Of Claim: ")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Date of claim must be in dd/mm/yyyy format.")]
        public string DATE_OF_CLAIM { get; set; }

        [Display(Name = "Amount Proposed by NRRDA: ")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Proposed amount is not in valid format. ")]
        [Range(0, 999999.99, ErrorMessage = "Proposed amount is not in valid format.")]
        public Nullable<decimal> AMOUNT_PASSED { get; set; }

        [Display(Name = "Date of amount passed by NDDRA: ")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Date of passing must be in dd/mm/yyyy format.")]
        public Nullable<System.DateTime> DATE_OF_PASSING { get; set; }

        [Display(Name = "Remark: ")]
        [StringLength(255, ErrorMessage = "Remark must be less than 255 characters.")]
        [RegularExpression(@"^([a-zA-Z0-9 ._',&()-]+)$", ErrorMessage = "Remark is not in valid format.")]
        public string REMARK { get; set; }

        [Display(Name = "Upload Ticket: ")]
        [Required(ErrorMessage = "Pdf file is Required")]
        [DataType(DataType.Upload)]
        public HttpPostedFileBase TRAVEL_TICKET { get; set; }

        [Display(Name = "Upload Boarding Pass: ")]
        [Required(ErrorMessage = "Boarding Pass is Required")]
        [DataType(DataType.Upload)]
        public HttpPostedFileBase BOARDING_PASS { get; set; }

        public string UPLOADED_TICKET_NAME { get; set; }
        
        public string DEC_UPLOADED_TICKET_NAME { get; set; }

        public string UPLOADED_TICKET_PATH { get; set; }

        public string BOARDING_PASS_NAME { get; set; }
        
        public string DEC_BOARDING_PASS_NAME { get; set; }

        public string BOARDING_PASS_PATH { get; set; }

        public int ADD_EDIT { get; set; }

        public Nullable<int> USERID { get; set; }
        public string IPADD { get; set; }
    }
}