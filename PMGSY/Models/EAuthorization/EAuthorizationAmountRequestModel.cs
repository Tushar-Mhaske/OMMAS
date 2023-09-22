using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PMGSY.Models.EAuthorization
{
    public class EAuthorizationAmountRequestModel
    {
        public Int32 ADMIN_ND_CODE { get; set; }
        public Int32 MAST_CONT_ID { get; set; }
        public Int32 AGREEMENT_CODE { get; set; }
        public string IMS_SANCTION_PACKAGE { get; set; }
        public decimal AMOUNT_AUTHORIZED { get; set; }

        public decimal AGREEMENT_AMOUNT { get; set; }


        public string Contractor_Name { get; set; }
        public string Aggrement_Number { get; set; }
        public string Package_Number { get; set; }

        public string DateAsOnNow { get; set; }
        public Int64 EAuth_ID { get; set; }


        [Display(Name = "Authorization Amount(In Lakhs.)")]
        [Required(ErrorMessage = "Authorization Amount is Required")]
        [RegularExpression(@"^\d{0,11}(\.\d{0,2})?$", ErrorMessage = "Onl  11 Digit Whole number and 2 Decimal Allowed")]
        public Nullable<decimal> AUTHORIZATION_AMOUNT { get; set; }

        





    }
}