using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Agreement
{
    public class BankGuaranteeDetailsModel
    {
        //public BankGuaranteeDetailsModel()
        //{
        //    this.BGFile = new HttpPostedFileBase();
        //}
       // [Required(ErrorMessage = "Please select Agreement.")]
        [Display(Name = "Agreement")]
      //  [Range(1, Int32.MaxValue, ErrorMessage = "Please select Agreement")]
        public string AGREEMENT_CODE { get; set; }

       // [Range(1, Int32.MaxValue)]
        public int BG_CODE { get; set; }

        [Required]
        [Display(Name = "Bank Name")]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Bank Name should contain only letters")]
        [StringLength(50,ErrorMessage="Bank Name can be upto 50 characters only")]
        public string BG_BANK_NAME { get; set; }

        [Required]
        [Display(Name = "Verified By")]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Verified By Name should contain only letters")]
        [StringLength(60, ErrorMessage = "Verified By can be upto 60 characters only")]
        public string VERIFIEDBY { get; set; }

        [Required]
        [Display(Name = "Amount (Rs. in Lakhs)")]
        [Range(0.01, 999999.99, ErrorMessage = "Amount can be of only six digit long.")]
        public decimal BG_AMOUNT { get; set; }

        [Required]
        [Display(Name = "Issue Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Issue Date must be in dd/mm/yyyy format.")]
        public string BG_ISSUE_DATE { get; set; }

        [Required]
        [Display(Name = "Expiry Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Expiry Date must be in dd/mm/yyyy format.")]
        public string BG_EXPIRY_DATE { get; set; }

        [Required]
        [Display(Name = "Verification Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Verification Date must be in dd/mm/yyyy format.")]
        public string BG_VERIFICATION_DATE { get; set; }

        public string TEND_BG_STATUS { get; set; }

        [RegularExpression(@"^[AU]", ErrorMessage = "Operation is invalid.")]
        public string Operation { get; set; }
        public int TendBgCode { get; set; }

        [Display(Name="File")]
        [Required(ErrorMessage="Pdf file is Required")]
        [DataType(DataType.Upload)]
        public HttpPostedFileBase  BGFile { get; set; }

        public String BGfileName { get; set; }
    }
}