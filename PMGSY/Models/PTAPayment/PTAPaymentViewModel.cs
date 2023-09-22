using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PMGSY.Models;
using System.Web.Mvc;

namespace PMGSY.Models.PTAPayment
{
    public class PTAPaymentViewModel
    {
        [Display(Name = "State")]
        public string MAST_STATE_NAME { get; set; }

        public string IMS_YEAR { get; set; }
        
        [Display(Name="Scrutinized Amount [In Lakhs]")]
        public Nullable<decimal> SANCTION_AMOUNT { get; set; }

        [Display(Name = "Total Honorarium of Scrutiny [In Lakhs]")]
        public decimal TOT_HON_OF_SCRUTINY { get; set; }
        [Display(Name = "Total Honorarium (Minimum) [In Rs]")]
        public decimal TOT_HON_MIN { get; set; }
        
        public string PTA_SANCTIONED_BY { get; set; }
        public string PTA_SERVICE_TAX_NO { get; set; }
        public string INSTITUTE_NAME { get; set; }
        public decimal? TOTAL_SCRUTNIZED_AMOUNT { get; set; }
        public decimal PER_TOT_VALUE { get; set; }
        public decimal HON_AMOUNT { get; set; }
        public decimal TOTAL_HON_AMOUNT_IN_RUPEES { get; set; }

        public int PMGSY_SCHEME { get; set; }
        public decimal PTA_SANCTION_AMOUNT { get; set; }
        public string PTA_SANCTION_AMOUNT_PER { get; set; }
    }

    public class PTAPaymentTotalModel
    {
        public decimal TOTAL_SANCTION_AMOUNT { get; set; }
        public decimal TOTAL_PER_TOT_VALUE { get; set; }
        public decimal TOTAL_HON_AMOUNT { get; set; }

        public string DIS_TOTAL_SANCTION_AMOUNT { get; set; }
        public string DIS_TOTAL_PER_TOT_VALUE { get; set; }
        public string DIS_TOTAL_HON_AMOUNT { get; set; }

    }
    public class PTAPaymentTotalViewModel
    {
        public decimal TOTAL_HONORARIUM_AMOUNT { get; set; }
        public decimal TOTAL_PENALTY_AMOUNT { get; set; }
        public decimal TOTAL_TDS_AMOUNT { get; set; }
        public decimal TOTAL_SC_AMOUNT { get; set; }
        public decimal TOTAL_SERVICE_TAX_AMOUNT { get; set; }
        public decimal TOTAL_AMOUNT { get; set; }

        public string DIS_TOTAL_HONORARIUM_AMOUNT { get; set; }
        public string DIS_TOTAL_PENALTY_AMOUNT { get; set; }
        public string DIS_TOTAL_TDS_AMOUNT { get; set; }
        public string DIS_TOTAL_SC_AMOUNT { get; set; }
        public string DIS_TOTAL_SERVICE_TAX_AMOUNT { get; set; }
        public string DIS_TOTAL_AMOUNT { get; set; }

    }

    public class PTAPayemntInvoiceModel
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
        [Required(ErrorMessage= "Please Enter Number")]
        [RegularExpression(@"^[a-zA-Z0-9/]+$", ErrorMessage = "Invalid  Number, Can only contains AlphaNumeric values")]
        public string IMS_NEFT_CHEQUE_NUMBER { get; set; }


        [Display(Name="Payment Type")]
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