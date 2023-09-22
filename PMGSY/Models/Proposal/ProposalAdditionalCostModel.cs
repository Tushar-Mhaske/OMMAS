using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PMGSY.Models.Proposal
{
    public class ProposalAdditionalCostModel
    {
        public int IMS_PR_ROAD_CODE { get; set; }
        public int IMS_TRANSACTION_CODE { get; set; }

        [UIHint("Hidden")]
        public string EncryptedTransactionRoadCode { get; set; }

        [UIHint("Hidden")]
        public string EncryptedRoadCode { get; set; }  
      
        [Display(Name = "State Amount")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Invalid State Amount,Can only contains Numeric values and 2 digit after decimal place")]
        [Range(0, 999999999.99, ErrorMessage = "Invalid State Amount,Can only contains Numeric values and 2 digit after decimal place")]
        [Required(ErrorMessage = "Please enter State Amount.")]
        public decimal IMS_STATE_AMOUNT { get; set; }

        [Display(Name = "Mord Amount")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Invalid Mord Amount,Can only contains Numeric values and 2 digit after decimal place")]
        [Range(0, 999999999.99, ErrorMessage = "Invalid Mord Amount,Can only contains Numeric values and 2 digit after decimal place")]
        [Required(ErrorMessage = "Please enter Mord Amount.")]
        public decimal IMS_MORD_AMOUNT { get; set; }

        [Display(Name = "Total Amount")]
        public decimal TOTAL_AMOUNT { get; set; }

        [Display(Name = "Release Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Release Date must be in dd/mm/yyyy format.")]
        [Required(ErrorMessage = "Please select Release Date.")]
        public string IMS_RELEASE_DATE { get; set; }

        [Display(Name="Letter Number")]
        [RegularExpression(@"^([a-zA-Z0-9-.#_/() ]+){1,50}$", ErrorMessage = "Please Enter Correct Letter No.")]
        [Required(ErrorMessage = "Please Enter Letter Number")]
        [StringLength(50,ErrorMessage="Please enter valid Letter Number")]
        public string IMS_LETTER_NO { get; set; }

        [Display(Name="File")]
        [Required(ErrorMessage="Please select File to upload")]
        public string IMS_FILE_NAME { get; set; }

        [Display(Name = "Year")]
        public int IMS_YEAR { get; set; }

        [Display(Name = "Batch")]
        public int IMS_BATCH { get; set; }

        [Display(Name = "Package Number")]            
        public string IMS_PACKAGE_ID { get; set; }

        [Display(Name = "Road Name")]
        public string IMS_ROAD_NAME { get; set; }  

        [Display(Name = "Pavement Length")]
        public Nullable<decimal> IMS_PAV_LENGTH { get; set; }

        [Display(Name = "State Amount")]
        public Nullable<decimal> IMS_STATE_AMOUNT_TEXT { get; set; }

        [Display(Name = "Mord Amount")]
        public Nullable<decimal> IMS_MORD_AMOUNT_TEXT { get; set; }


        [Display(Name = "Total Amount")]
        public Nullable<decimal> TOTAL_AMOUNT_TEXT { get; set; }


    }
}