using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PMGSY.Models.Proposal
{
    public class DropOrderViewModel
    {
        [Required(ErrorMessage="Please select state")]
        [Range(1,Int32.MaxValue,ErrorMessage="Invalid State.")]
        public int StateCode { get; set; }

        [Required(ErrorMessage="Please select Year.")]
        //[Range(2000, 2100, ErrorMessage = "Invalid Year.")]
        [Range(0, 2100, ErrorMessage = "Invalid Year.")]
        public int YearCode { get; set; }
        
        [Required(ErrorMessage="Batch is required.")]
        //[Range(1,20,ErrorMessage="Please select Batch.")]
        [Range(0, 20, ErrorMessage = "Please select Batch.")]
        public int BatchCode { get; set; }

        [Required(ErrorMessage="Please select stream.")]
        //[Range(1,Int32.MaxValue,ErrorMessage="Please select stream.")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Please select stream.")]
        public int StreamCode { get; set; }

        [Required(ErrorMessage = "Please select scheme.")]
        [Range(1, 4, ErrorMessage = "Please select scheme.")]
        public int PMGSYScheme { get; set; }

        public string IMS_PDF_NAME { get; set; }
        public System.DateTime IMS_GENERATION_DATE { get; set; }

        //pp

        [Display(Name = "Drop Order Number")]
        [Required(ErrorMessage = "Drop Order Number is required.")]
        [RegularExpression(@"^([a-zA-Z0-9-._/() ]+)$", ErrorMessage = "Please Enter Correct Drop Order No.")]
        [StringLength(60, ErrorMessage = "Drop Order Number should be less than 60 character.")]
        public string IMS_DROP_ORDER_NUMBER { get; set; }

        [Display(Name = "Drop Order Date")]
        [Required(ErrorMessage = "Drop Order Date is required.")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Order Date must be in dd/mm/yyyy format.")]
        public String IMS_DROP_ORDER_DATE { get; set; }

        public int RequestCode { get; set; }

        public String IMS_REQUEST_ORDER_DATE { get; set; }
        //public bool IsDOGenerated { get; set; } //pp

    }
}