using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace PMGSY.Models.Agreement
{
    public class IncompleteReason
    {
        [UIHint("Hidden")]
        public string EncryptedTendAgreementCode_IncompleteReason { get; set; }

        [Display(Name = "Reason")]
        [RegularExpression(@"^([a-zA-Z0-9 ._',/()-]+)$", ErrorMessage = "Reason is not in valid format.")]
        [Required(ErrorMessage = "Reason is required.")]
        [StringLength(255, ErrorMessage = "Reason must be less than 255 characters.")]
        public string TEND_INCOMPLETE_REASON { get; set; }

        [Display(Name = "Value of Work Done (Rs in Lakhs)")]
        //[Required(ErrorMessage = "Agreement Amount is required.")]
        //[RegularExpression(@"^\d{1,7}\.\d{0,2}$", ErrorMessage = "Agreement Amount is not in valid format. ")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Value of Work Done Amount is not in valid format. ")]
        [Range(0, 9999999999999999.99, ErrorMessage = "Value of Work Done Amount is not in valid format.")]
        public decimal? TEND_VALUE_WORK_DONE { get; set; }
    }
}