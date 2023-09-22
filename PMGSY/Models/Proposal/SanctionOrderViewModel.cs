using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PMGSY.Models.Proposal
{
    public class SanctionOrderViewModel
    {
        [Required(ErrorMessage="Please select state")]
        [Range(1,Int32.MaxValue,ErrorMessage="Invalid State.")]
        public int StateCode { get; set; }

        [Required(ErrorMessage="Please select Year.")]
        [Range(2000, 2100, ErrorMessage = "Invalid Year.")]
        public int YearCode { get; set; }
        
        [Required(ErrorMessage="Batch is required.")]
        [Range(1,20,ErrorMessage="Please select Batch.")]
        public int BatchCode { get; set; }

        [Required(ErrorMessage="Please select stream.")]
        [Range(1,Int32.MaxValue,ErrorMessage="Please select stream.")]
        public int StreamCode { get; set; }

        [Required(ErrorMessage = "Agency is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select Agency.")]
        public int Agency { get; set; }

        ///Changes for RCPLWE
        [Required(ErrorMessage = "Please select scheme.")]
        [Range(1, 4, ErrorMessage = "Please select valid scheme.")]
        public int PMGSYScheme { get; set; }

        public string IMS_PDF_NAME { get; set; }
        public System.DateTime IMS_GENERATION_DATE { get; set; }

        [Display(Name="Sanction Order Date")]
        [Required(ErrorMessage="Sanction Order Date is required.")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Order Date must be in dd/mm/yyyy format.")]
        public String IMS_ORDER_DATE { get; set; }

        [Display(Name="Sanction Order Number")]
        [Required(ErrorMessage="Sanction Order Number is required.")]
        [RegularExpression(@"^([a-zA-Z0-9-._/() ]+)$", ErrorMessage = "Please Enter Correct Sanction Order No.")]
        //[RegularExpression(@"^[a-zA-Z0-9 /-().]+$", ErrorMessage = "Invalid Sanction Order Number")]
        public string IMS_ORDER_NUMBER { get; set; }

        public bool IsSOGenerated { get; set; }


         

    }
}