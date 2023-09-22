using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.NQMTourClaimModels
{
    public class NQM_LODGE_CLAIM_DETAILS_MODEL
    {
        public int LODGE_CLAIM_ID { get; set; }

        public int TOUR_CLAIM_ID { get; set; }

        public int ADMIN_SCHEDULE_CODE { get; set; }

        [Display(Name = "Date of claim: ")]
        [Required(ErrorMessage = "Date of Claim is required.")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Date of claim must be in dd/mm/yyyy format.")]
        public string DATE_OF_CLAIM { get; set; }

        [Display(Name = "Type Of Accomodation: ")]
        [Required(ErrorMessage = "Type of Claim is required")]
        [RegularExpression(@"^[HGS]?", ErrorMessage = "Type of Claim is required.")]
        public string TYPE_OF_CLAIM { get; set; }

        public List<SelectListItem> LST_TYPE_OF_CLAIM { get; set; }

        [Display(Name = "Date From: ")]
        [Required(ErrorMessage = "Date From is required.")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Date from must be in dd/mm/yyyy format.")]
        public System.DateTime DATE_FROM { get; set; }

        [Display(Name = "Date To: ")]
        [Required(ErrorMessage = "Date To is required.")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Date to must be in dd/mm/yyyy format.")]
        public System.DateTime DATE_TO { get; set; }

        [Display(Name = "Hotel Name: ")]
        [Required(ErrorMessage = "Hotel Name is required.")]
        [StringLength(255, ErrorMessage = "Hotel Name must be less than 255 characters.")]
        [RegularExpression(@"^([a-zA-Z ._',&()-]+)$", ErrorMessage = "Hotel Name is not in valid format.")]
        public string HOTEL_NAME { get; set; }

        [Display(Name = "Amount Claimed: ")]
        [Required(ErrorMessage = "Amount Claim is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Amount Claim is not in valid format. ")]
        [Range(0, 999999.99, ErrorMessage = "Amount Claim is not in valid format.")]
        public decimal? AMOUNT_CLAIMED_HOTEL { get; set; }

        [Display(Name = "Guest House Name: ")]
        [Required(ErrorMessage = "Guest House Name is required.")]
        [StringLength(255, ErrorMessage = "Hotel Name must be less than 255 characters.")]
        [RegularExpression(@"^([a-zA-Z ._',&()-]+)$", ErrorMessage = "Hotel Name is not in valid format.")]
        public string GUEST_HOUSE_NAME { get; set; }

        [Display(Name = "Amount Claimed: ")]
        [Required(ErrorMessage = "Amount Claim is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Amount Claim is not in valid format. ")]
        [Range(0, 999999.99, ErrorMessage = "Amount Claim is not in valid format.")]
        public decimal? AMOUNT_CLAIMED_GUEST { get; set; }

        //[Display(Name = "Amount Claimed For Daily Allowance: ")]
        [Required(ErrorMessage = "Amount Claim is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Amount Claim is not in valid format. ")]
        [Range(0, 999999.99, ErrorMessage = "Amount Claim is not in valid format.")]
        public decimal AMOUNT_CLAIMED_DAILY { get; set; }

        [Display(Name = "Amount Proposed For Hotel by NRRDA: ")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Proposed amount is not in valid format. ")]
        [Range(0, 999999.99, ErrorMessage = "Proposed amount is not in valid format.")]
        public Nullable<decimal> AMOUNT_PASSED_HOTEL { get; set; }

        [Display(Name = "Amount Proposed For Daily Allowance by NRRDA: ")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Proposed amount is not in valid format. ")]
        [Range(0, 999999.99, ErrorMessage = "Proposed amount is not in valid format.")]
        public Nullable<decimal> AMOUNT_PASSED_DAILY { get; set; }

        [Display(Name = "Date Of Passing: ")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Date of passing must be in dd/mm/yyyy format.")]
        public Nullable<System.DateTime> DATE_OF_PASSING { get; set; }

        [Display(Name = "Upload Bill: ")]
        [Required(ErrorMessage = "Bill is Required")]
        [DataType(DataType.Upload)]
        public HttpPostedFileBase HOTEL_BILL { get; set; }

        [Display(Name = "Upload Bill: ")]
        [Required(ErrorMessage = "Bill is Required")]
        [DataType(DataType.Upload)]
        public HttpPostedFileBase GUEST_HOUSE_BILL { get; set; }

        public string UPLOADED_BILL_NAME { get; set; }

        public string DEC_UPLOADED_BILL_NAME { get; set; }

        public string UPLOADED_BILL_PATH { get; set; }

        [Display(Name = "Upload e-Receipt: ")]
        [Required(ErrorMessage = "e-Receipt is Required")]
        [DataType(DataType.Upload)]
        public HttpPostedFileBase HOTEL_RECEIPT { get; set; }

        public string UPLOADED_RECEIPT_NAME { get; set; }

        public string DEC_UPLOADED_RECEIPT_NAME { get; set; }

        public string UPLOADED_RECEIPT_PATH { get; set; }

        [Display(Name = "Remark: ")]
        [StringLength(255, ErrorMessage = "Remark must be less than 255 characters.")]
        [RegularExpression(@"^([a-zA-Z0-9 ._',&()-]+)$", ErrorMessage = "Remark is not in valid format.")]
        public string REMARK { get; set; }

        public int ADD_EDIT { get; set; }

        public Nullable<int> USERID { get; set; }
        public string IPADD { get; set; }
    }
}