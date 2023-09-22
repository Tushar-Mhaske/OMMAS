using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.BAL.Receipt;
using PMGSY.Common;

namespace PMGSY.Models.Receipts
{
    public class BillDetailsViewModel
    {
        public long BILL_ID { get; set; }
        public short TXN_NO { get; set; }
        
        [Display(Name = "Sub Transaction Type")]
        [Required(ErrorMessage = "Sub Transaction Type is Required")]
        [IsEditableTrans("TXN_ID")]
        public string TXN_ID { get; set; }

        [Display(Name = "Unsettled Imprest Voucher")]
        public string IMPREST_BILL_ID { get; set; }

        public short HEAD_ID { get; set; }

        [Display(Name = "Amount")]
        [Required(ErrorMessage = "Amount is Required")]
        [RegularExpression(@"^\d{0,11}(\.\d{0,2})?$", ErrorMessage = "Only 11 Digit Whole number and 2 Decimal Allowed")]
        [Range(0.01,double.MaxValue,ErrorMessage="Invalid Amount")]
        //[IsReceiptDetailsAmountGreater("BILL_ID")]
        public Nullable<decimal> AMOUNT { get; set; }
        public string CREDIT_DEBIT { get; set; }
        public string CASH_CHQ { get; set; }
        
        [Display(Name = "Narration")]
        [Required(ErrorMessage = "Narration is Required")]  
        [StringLength(255,ErrorMessage="Maximum 255 Characters Allowed")]
        //[RegularExpression(@"^[a-zA-Z0-9-/. ]+$", ErrorMessage = "Only Alphanumeric, Space and '-','/','.' Allowed")]
        //[RegularExpression(@"^([a-zA-Z0-9 ./,()-]+)$", ErrorMessage = "Only Alphanumeric, Space and '-','/','.','(',')',',' Allowed")] //added some special characters by koustubh nakate on 18/07/2013 
        //[RegularExpression(@"^([a-zA-Z0-9 ./,()-]+)$", ErrorMessage = "Only Alphanumeric, Space and '-','/','.','(',')',',' Allowed")] //modified by abhishek kamble on 11/nov/2013 
        [RegularExpression("^([a-zA-Z0-9 -.,/\r\n()]+)", ErrorMessage = "Only Alphanumeric, Space and '-','/','.','(',')',',' Allowed")]
        public string NARRATION { get; set; }

        [Display(Name = "PIU Name")]
        //[Required(ErrorMessage = "Department Name is Required")]
        //[Range(1,2147483647,ErrorMessage="Please Select Contractor")]
        //[Required(ErrorMessage = "Department Name is Required")]
        [IsDropdownSelected("ADMIN_ND_CODE",ErrorMessage="Please select PIU")]
        public Nullable<int> ADMIN_ND_CODE { get; set; }
        
        [Display(Name = "Company Name")]
        //[Required(ErrorMessage = "Company Name is Required")]
        //[Range(1, 2147483647, ErrorMessage = "Please Select Company")]
        //[Required(ErrorMessage = "Company Name is Required")]
        [IsDropdownSelected("MAST_CON_ID", ErrorMessage = "Please select Contractor")]
        public Nullable<int> MAST_CON_ID { get; set; }

        public Nullable<int> IMS_PR_ROAD_CODE { get; set; }
        
        [Display(Name = "Agreement Name")]
        //[Required(ErrorMessage = "Agreement Name is Required")]
        //[Range(1, 2147483647, ErrorMessage = "Please Select Agreement")]
        //[Required(ErrorMessage = "Agreement Name is Required")]
        [IsDropdownSelected("IMS_AGREEMENT_CODE", ErrorMessage = "Please select Agreement")]
        public Nullable<int> IMS_AGREEMENT_CODE { get; set; }
        
        public string MAS_FA_CODE { get; set; }
        public Nullable<bool> FINAL_PAYMENT { get; set; }

        public string CON_REQ { get; set; }
        public string AGREEMENT_REQ { get; set; }
        public string ROAD_REQ { get; set; }
        public string PIU_REQ { get; set; }
        public string SUPPLIER_REQ { get; set; }
        public string FUND_TYPE { get; set; }
        public short LVL_ID { get; set; }
        public string BILL_TYPE { get; set; }

        public string EncryptedBillId { get; set; }

        public virtual ACC_BILL_MASTER ACC_BILL_MASTER { get; set; }
        public virtual ACC_MASTER_HEAD ACC_MASTER_HEAD { get; set; }
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

            if (testedValue != null)
            {
                if (testedValue.ToString().Trim().Equals("0"))
                {
                    return ValidationResult.Success;
                }
                else if (value.ToString().Trim().Equals(""))
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
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

    public class IsEditableTrans : ValidationAttribute, IClientValidatable
    {
        private string DefaultErrorMessage = "Invalid Transaction";
        private readonly string ObProperty;

        public IsEditableTrans(string obProperty)
        {
            this.ObProperty = obProperty;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(DefaultErrorMessage, name, ObProperty);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            CommonFunctions commonFunc = new CommonFunctions();
            Boolean status = false;

            if (value != null)
            {
                value = value.ToString().Contains('$') ? value.ToString().Split('$')[0] : value.ToString();
                status = commonFunc.IsTransactionEditable(Convert.ToInt16(value));
            }

            if (status)
            {
                return ValidationResult.Success;
            }
            else
            {
                DefaultErrorMessage = "Invalid Sub Transaction for OB Entry";
                var message = FormatErrorMessage(validationContext.DisplayName);
                return new ValidationResult(message);
            }
        }


        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "iseditabletrans"
            };
            yield return rule;
        }
    }
}


