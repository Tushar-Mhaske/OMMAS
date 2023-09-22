using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PMGSY.Models.NQMTourClaimModels
{
    public class NQM_DAILY_ALLOWANCE_MASTER_MODEL
    {
        public int DAILY_CLAIM_ID { get; set; }

        public int TOUR_CLAIM_ID { get; set; }

        [Display(Name = "Expenses Description: ")]
        [Required(ErrorMessage = "Expenses Description is required.")]
        [StringLength(255, ErrorMessage = "Expenses Description must be less than 255 characters.")]
        [RegularExpression(@"^([a-zA-Z0-9 ._',&()-]+)$", ErrorMessage = "Expenses Description is not in valid format.")]
        public string EXPENDITURE_DESC { get; set; }

        public string UPLOADED_FILE_NAME { get; set; }

        public string UPLOADED_FILE_PATH { get; set; }

        [Display(Name = "Amount Claim: ")]
        [Required(ErrorMessage = "Amount Claim is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Amount Claim is not in valid format. ")]
        [Range(0, 999999.99, ErrorMessage = "Amount Claim is not in valid format.")]
        public decimal AMOUNT_CLAIMED { get; set; }

        [Display(Name = "Date of Claim: ")]
        [Required(ErrorMessage = "Date of Claim is required.")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Date of Claim must be in dd/mm/yyyy format.")]
        public string DATE_OF_CLAIM { get; set; }

        [Display(Name = "Date of Expense: ")]
        [Required(ErrorMessage = "Date of Expense is required.")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Date of expense must be in dd/mm/yyyy format.")]
        /*[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]*/
        public System.DateTime DATE_OF_EXPENSE { get; set; }

        [Display(Name = "Amount Propossed by NRRDA: ")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Proposed amount is not in valid format. ")]
        [Range(0, 999999.99, ErrorMessage = "Proposed amount is not in valid format.")]
        public Nullable<decimal> AMOUNT_PASSED { get; set; }

        [Display(Name = "Date of amount passed by NDDRA: ")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Date of passing must be in dd/mm/yyyy format.")]
        public Nullable<System.DateTime> DATE_OF_PASSING { get; set; }

        [Display(Name = "Upload Bill / Invoice: ")]
        [Required(ErrorMessage = "Pdf file is Required")]
        [DataType(DataType.Upload)]
        public HttpPostedFileBase BILL_FILE { get; set; }

        [StringLength(255, ErrorMessage = "Remark must be less than 255 characters.")]
        [RegularExpression(@"^([a-zA-Z0-9 ._',&()-]+)$", ErrorMessage = "Remark is not in valid format.")]
        public string REMARK { get; set; }

        public Nullable<int> USERID { get; set; }
        public string IPADD { get; set; }
    }
}