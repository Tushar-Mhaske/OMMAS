using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.TransferEntryOrder
{
    public partial class TeoDetailsModel
    {
        public long BILL_ID { get; set; }
        public short TXN_NO { get; set; }
        public short TXN_ID { get; set; }
        
        [Display(Name = "Account Head")]
        [Required(ErrorMessage = "Account Head Required")]
        [Range(1,Int16.MaxValue,ErrorMessage="Account Head Required")]
        public short HEAD_ID { get; set; }

        [Display(Name = "Amount")]
        [Required(ErrorMessage = "Amount is Required")]
        [RegularExpression(@"^\d{0,11}(\.\d{0,2})?$", ErrorMessage = "Only 11 Digit Whole number and 2 Decimal Allowed")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Invalid Amount")]
        public Nullable<decimal> AMOUNT { get; set; }

        [Display(Name = "Narration")]
        [Required(ErrorMessage = "Narration is Required")]
        [StringLength(255, ErrorMessage = "Maximum 255 Characters Allowed")]
        [RegularExpression(@"^[a-zA-Z0-9-/\r\n. ]+$", ErrorMessage = "Only Alphanumeric, Space and '-','/','.' Allowed")]
        public string NARRATION { get; set; }
        
        [Display(Name = "PIU Name")]
        public Nullable<int> ADMIN_ND_CODE { get; set; }

        [Display(Name = "Company Name")]
        public Nullable<int> MAST_CON_ID { get; set; }

         [Display(Name = "Company Name")]
        public Nullable<int> MAST_CON_ID_TRANS { get; set; }
        

        [Display(Name = "Road Name")]
        public Nullable<int> IMS_PR_ROAD_CODE { get; set; }

        public Nullable<int> IMS_PR_ROAD_CODE_Head { get; set; }

        [Display(Name = "Agreement Name")]
        public Nullable<int> IMS_AGREEMENT_CODE { get; set; }

        [Display(Name = "District Name")]
        public Nullable<int> MAST_DISTRICT_CODE { get; set; }

        [Display(Name = "Package")]
        public string IMS_PACKAGE_ID { get; set; }

        [Display(Name = "Sanction Year")]
        public Nullable<int> SANC_YEAR { get; set; }

        public string CREDIT_DEBIT { get; set; }
        public string CASH_CHQ { get; set; }

        public Nullable<int> MAS_FA_CODE { get; set; }
        
        //public Nullable<int> MAS_FA_CODE { get; set; }
        [Display(Name = "Is Final Payment")]
        public bool FINAL_PAYMENT { get; set; }
        

        public virtual ACC_BILL_MASTER ACC_BILL_MASTER { get; set; }
        public virtual ACC_MASTER_HEAD ACC_MASTER_HEAD { get; set; }
        public virtual ACC_MASTER_TXN ACC_MASTER_TXN { get; set; }
        public virtual MASTER_CONTRACTOR MASTER_CONTRACTOR { get; set; }
    }

    public class IsDropdownSelected : ValidationAttribute, IClientValidatable
    {
        private readonly string PropertyName;

        public IsDropdownSelected(string propertyName)
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

            var testedValue = propertyTestedInfo.GetValue(validationContext.ObjectInstance, null);

            if (testedValue.ToString().Trim().Equals("0"))
            {
                return ValidationResult.Success;
            }
            else if (value.ToString().Trim().Equals(""))
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }

            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "isdropdownselected"
            };
            rule.ValidationParameters["parentdropdown"] = this.PropertyName;
            yield return rule;
        }

    }
}