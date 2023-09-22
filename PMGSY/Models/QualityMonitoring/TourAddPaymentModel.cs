using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.QualityMonitoring
{
    public class TourAddPaymentModel
    {
       
        public int IMS_INVOICE_CODE { get; set; }
        public int IMS_Payment_CODE { get; set; }

        [UIHint("Hidden")]
        public string EncryptedIMS_Payment_CODE { get; set; }

        [UIHint("Hidden")]
        public string EncryptedIMS_Invoice_Code { get; set; }

        public string Invoice_Generate_DATE { get; set; }

        [Display(Name = "Payment Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Payment Date must be in dd/mm/yyyy format.")]
        [Required(ErrorMessage = "Please select Payment Date.")]
        [DateValidationVST("Invoice_Generate_DATE", ErrorMessage = "Payment Date must be greater than or equal to invoice generate date.")]
        public string IMS_Payment_DATE { get; set; }

        [Display(Name = "Cheque Number")]
        [Required(ErrorMessage = "Please Enter Number")]
        [RegularExpression(@"^[a-zA-Z0-9/]+$", ErrorMessage = "Invalid  Number, Can only contains AlphaNumeric values")]
        public string IMS_NEFT_CHEQUE_NUMBER { get; set; }

        [Display(Name = "Payment Type")]
        [RegularExpression(@"^([CN]+)$", ErrorMessage = "Please select Check or NEFT.")]
        [Required(ErrorMessage = "Please select Payment Type")]
        public string Payment_Type { get; set; }

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

    }
}