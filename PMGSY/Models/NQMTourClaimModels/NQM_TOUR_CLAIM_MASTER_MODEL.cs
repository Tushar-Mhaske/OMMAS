using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.NQMTourClaimModels
{
    public class NQM_TOUR_CLAIM_MASTER_MODEL
    {
        public int TOUR_CLAIM_ID { get; set; }

        public int ADMIN_QM_CODE { get; set; }

        public int ADMIN_SCHEDULE_CODE { get; set; }

        [Display(Name = "Month")]
        public int FROM_MONTH { get; set; }
        public List<SelectListItem> FROM_MONTHS_LIST { set; get; }

        [Display(Name = "Year")]
        public int FROM_YEAR { get; set; }
        public List<SelectListItem> FROM_YEARS_LIST { set; get; }

        [Display(Name = "Date Of Claim: ")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Date of claim must be in dd/mm/yyyy format.")]
        public string DATE_OF_CLAIM { get; set; }

        [Display(Name = "Scheduled Month: ")]
        public string MONTH_OF_INSPECTION { get; set; }

        [Display(Name = "Scheduled Year: ")]
        public int YEAR_OF_INSPECTION { get; set; }

        [Display(Name = "Date From: ")]
        [Required(ErrorMessage = "Date From is required.")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public string DATE_FROM { get; set; }

        [Display(Name = "Date To: ")]
        [Required(ErrorMessage = "Date To is required.")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public string DATE_TO { get; set; }

        [Display(Name = "Bank Name: ")]
        [Required(ErrorMessage = "Bank Name is required")]
        public string BANK_NAME { get; set; }

        public List<SelectListItem> LST_BANK_NAME { get; set; }

        [Display(Name = "Account Number :")]
        [Required(ErrorMessage = "Account Number is required")]
        [RegularExpression("^[a-zA-Z0-9]{9,18}$", ErrorMessage = "Account Number must be minimum 9 digits amd maximum 18 digits only.")]
        [StringLength(18, ErrorMessage = "Account Number must be 18 digits only.")]
        public string ACCOUNT_NUMBER { get; set; }

        [Display(Name = "IFSC Code: ")]
        [Required(ErrorMessage = "Please enter IFSC Code")]
        [RegularExpression(@"^([A-Z|a-z]{4}[0][A-Z|a-z|0-9]{6})$", ErrorMessage = "IFSC Code is not in valid format.")]
        [StringLength(11, ErrorMessage = "IFSC Code must be 11 characters only.")]
        public string IFSC_CODE { get; set; }

        [Display(Name = "NRRDA Letter Number: ")]
        public string NRRDA_LETTER_NUMBER { get; set; }

        [Display(Name = "Name: ")]
        public string ADMIN_QM_NAME { get; set; }

        [Display(Name = "Address: ")]
        public string ADMIN_QM_ADDRESS { get; set; }

        [Display(Name = "PAN Number: ")]        
        public string ADMIN_QM_PAN { get; set; }

        [Display(Name = "Mobile Number: ")]
        public string ADMIN_QM_MOBILE { get; set; }

        [Display(Name = "E-mail Address: ")]
        public string ADMIN_QM_EMAIL { get; set; }

        public int addEditCheck { get; set; }

        public int? finalizeFlag { get; set; }

        [Display(Name = "Total District Visited Allowance: ")]
        public decimal? DISTRICT_VISITED_ALLOWANCE { get; set; }

        [Display(Name = "Total Claimed Amount: ")]
        public decimal? TOTAL_AMOUNT_CLAIMED { get; set; }

        [Display(Name = "Total Sanctioned Amount: ")]
        public string TOTAL_AMOUNT_SANCTIONED { get; set; }

        [Display(Name = "Total Proposed Amount by Finance: ")]
        public string TOTAL_AMOUNT_PASSED_FIN1 { get; set; }

        [Display(Name = "Remark: ")]
        [StringLength(255, ErrorMessage = "Remark must be less than 255 characters.")]
        [RegularExpression(@"^([a-zA-Z0-9 ._',&()-]+)$", ErrorMessage = "Remark is not in valid format.")]
        public string REMARK { get; set; }

        public int ROUND_SEQUENCE { get; set; }

        public int MONTH_NUMBER { get; set; }

        public string OFFICE_ASSISTANT_P_III { get; set; }

        public string OFFICE_ASSISTANT_FINANCE { get; set; }

        public string ASSISTANT_DIRECTOR { get; set; }

        public string DIRECTOR { get; set; }

        public Nullable<int> USERID { get; set; }
        public string IPADD { get; set; }

    }
}