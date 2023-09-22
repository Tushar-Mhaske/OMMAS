using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.STAPayment
{
    public class STAInvoiceViewModel
    {
        public long IMS_INVOICE_ID { get; set; }
        public int MAST_STATE_CODE { get; set; }
        public short IMS_YEAR { get; set; }
        public int IMS_BATCH { get; set; }
        public int IMS_STREAM { get; set; }
        public string IMS_PROPOSAL_TYPE { get; set; }
        public string STA_SANCTIONED_BY { get; set; }
        public string SAS_ABBREVATION { get; set; }
        public string Invoice_Generate_DATE { get; set; }
        [Display(Name="Honorarium Amount")]
        public decimal HONORARIUM_AMOUNT { get; set; }
        
        [Display(Name="Deduction as Penalty [In Rs.]")]
        [Required]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Invalid Value of as Penalty [In Rs.],Can only contains 16 Numeric digits and 2 digit after decimal place")]
        [Range(0.00, 9999999999999999.99, ErrorMessage = "Invalid Value of Penalty [In Rs.],Can only contains 16 Numeric digits and 2 digit after decimal place.")]
        [CompareValidationWithHonoriumAmount("HONORARIUM_AMOUNT", ErrorMessage = "Penalty amount should not be greater than value of Honorarium Amount.")]
        public decimal PENALTY_AMOUNT { get; set; }
        
        public int MAST_TDS_ID { get; set; }
        
        [Display(Name="Less TDS [In Rs.]")]
        public decimal TDS_AMOUNT { get; set; }
        
        [Display(Name="Less SC [In Rs.]")]
        public decimal SC_AMOUNT { get; set; }

        [Display(Name = "Service Tax @ ")]
        public decimal SERVICE_TAX_AMOUNT { get; set; }
        
        public decimal TOTAL_AMOUNT { get; set; }
        
        public System.DateTime GENERATION_DATE { get; set; }

        [Display(Name = "Invoice File Number")]
        [Required]
        //[RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "Invalid Invoice File Number,Can only contains Alphabets Numbers.")]
        [RegularExpression(@"^[a-zA-Z0-9 ./&-]{2,}$", ErrorMessage = "Invalid Invoice File Number, Can only contains AlphaNumeric values and [./&-], should be minimum two characters in length")]
        public string INVOICE_FILE_NO { get; set; }

        [Display(Name = "Balance Honorarium Amount [In Rs.]")]
        public decimal Balance_Amount { get; set; }
        
        [Display(Name="Honorarium Amount After Deduction [In Rs.]")]
        public decimal Honorarium_Amount_After_Deduction { get; set; }
        
        [Display(Name="TDS%")]
        public decimal Per_Tds { get; set; }

        [Display(Name = "Service Tax %")]
        public decimal Per_Service_Tax { get; set; }

        [Display(Name="Service Tax No.")]
        public string ServiceTaxNo { get; set; }
        
        [Display(Name = "SC%")]
        public decimal Per_Sc { get; set; }

        [Display(Name = "Amount To Be Paid")]
        public decimal Amount_To_Be_Paid{get;set;}

        public int PMGSY_SCHEME { get; set; }

        //Properties for Report
        public string TOTAL_AMOUNT_WORDS { get; set; }
        public string TOTAL_AMOUNT_IND_FORMAT { get; set; }
        public string INVOICE_GEN_DATE { get; set; }
        public string HONORARIUM_AMOUNT_IND_FORMAT { get; set; }
        public string PENALTY_AMOUNT_IND_FORMAT { get; set; }
        public string TDS_AMOUNT_IND_FORMAT { get; set; }
        public string SC_AMOUNT_IND_FORMAT { get; set; }
    }
    public class CompareValidationWithHonoriumAmount : ValidationAttribute, IClientValidatable
    {
        private readonly string PropertyName;

        public CompareValidationWithHonoriumAmount(string propertyName)
        {
            this.PropertyName = propertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyTestedInfo = validationContext.ObjectType.GetProperty(this.PropertyName);
            if (propertyTestedInfo == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.PropertyName));
            }

            var honoriumValue = Convert.ToDecimal(propertyTestedInfo.GetValue(validationContext.ObjectInstance, null));
            var penaltyValue = Convert.ToDecimal(value);

            if (honoriumValue < penaltyValue)
            {
                if (penaltyValue == null)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
            }
            else
            {
                return ValidationResult.Success;
            }
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "comparevalidationwithhonoriumamount"
            };
            //rule.ValidationParameters["compareworkpayment"] = this.PropertyName;
            yield return rule;
        }
    }
}