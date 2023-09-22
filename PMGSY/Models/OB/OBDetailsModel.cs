using PMGSY.DAL.OB;
using PMGSY.Extensions;
using PMGSY.Models.Common;
using PMGSY.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.OB
{
    public class OBDetailsModel
    {
        public long BILL_ID { get; set; }
        public short TXN_NO { get; set; }

        [Display(Name = "Transaction Type")]
        [Required(ErrorMessage = "Select Transaction")]
        [Range(1, Int16.MaxValue, ErrorMessage = "Please Select Transaction Type")]
        public short TXN_ID { get; set; }

        [Display(Name = "Sub Transaction Type")]
        [Required(ErrorMessage = "Select Sub Transaction")]
        [Range(1, Int16.MaxValue, ErrorMessage = "Please Select Sub Transaction Type")]
        [IsEditableTrans("SUB_TXN_ID")]
        public short SUB_TXN_ID { get; set; }

        public short HEAD_ID { get; set; }

        [Display(Name = "Amount")]
        [Required(ErrorMessage = "Amount is Required")]
        [RegularExpression(@"^\d{0,11}(\.\d{0,2})?$", ErrorMessage = "Only 11 Digit Whole number and 2 Decimal Allowed")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Invalid Amount")]
        [IsDetailsAmountGreater("HDN_TXN_NO", "CREDIT_DEBIT")]
        public Nullable<decimal> AMOUNT { get; set; }

        [Display(Name = "Narration")]
        [Required(ErrorMessage = "Narration is Required")]
        [StringLength(255, ErrorMessage = "Maximum 255 Characters Allowed")]
        //[RegularExpression(@"^[a-zA-Z0-9-/. ]+$", ErrorMessage = "Only Alphanumeric, Space and '-','/','.' Allowed")]
        [RegularExpression(@"^([a-zA-Z0-9 &./,()-]+)$", ErrorMessage = "Only Alphanumeric, Space and '-','/','.','(',')',',' Allowed")] //added some special characters by koustubh nakate on 18/07/2013 
        public string NARRATION { get; set; }

        [Display(Name = "PIU Name")]
        [IsDropdownSelected("SUB_TXN_ID", ErrorMessage = "Please select PIU")]
        public Nullable<int> ADMIN_ND_CODE { get; set; }

        [Display(Name = "Company Name")]
        [IsDropdownSelected("SUB_TXN_ID", ErrorMessage = "Please select Contractor")]
        public Nullable<int> MAST_CON_ID { get; set; }

        [Display(Name = "Road Name")]
        [IsDropdownSelected("SUB_TXN_ID", ErrorMessage = "Please select Road")]
        public Nullable<int> IMS_PR_ROAD_CODE { get; set; }

        [Display(Name = "Agreement Name")]
        [IsDropdownSelected("SUB_TXN_ID", ErrorMessage = "Please select Agreement")]
        public Nullable<int> IMS_AGREEMENT_CODE { get; set; }

        [Display(Name = "District Name")]
        public Nullable<int> MAST_DISTRICT_CODE { get; set; }

        [Display(Name = "Package")]
        [IsDropdownSelected("SUB_TXN_ID", ErrorMessage = "Please select Package")]
        public string IMS_PACKAGE_ID { get; set; }

        [Display(Name = "Sanction Year")]
        [IsDropdownSelected("SUB_TXN_ID", ErrorMessage = "Please select Year")]
        public Nullable<int> SANC_YEAR { get; set; }

        [Display(Name = "Type")]
        public string CREDIT_DEBIT { get; set; }

        public string HDN_TXN_NO { get; set; }

        public string CASH_CHQ { get; set; }

        [Display(Name = "Is Final Payment")]
        public bool FINAL_PAYMENT { get; set; }

        public virtual ACC_MASTER_HEAD ACC_MASTER_HEAD { get; set; }
        public virtual ACC_MASTER_TXN ACC_MASTER_TXN { get; set; }
        public virtual ADMIN_DEPARTMENT ADMIN_DEPARTMENT { get; set; }
        public virtual IMS_SANCTIONED_PROJECTS IMS_SANCTIONED_PROJECTS { get; set; }
        public virtual MASTER_AGENCY MASTER_AGENCY { get; set; }
        public virtual MASTER_CONTRACTOR MASTER_CONTRACTOR { get; set; }
        public virtual TEND_AGREEMENT_MASTER TEND_AGREEMENT_MASTER { get; set; }
        public virtual ACC_BILL_MASTER ACC_BILL_MASTER { get; set; }
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
            CommonFunctions objCommonFunction = new CommonFunctions();
            var propertyTestedInfo = validationContext.ObjectType.GetProperty(this.PropertyName);
            if (propertyTestedInfo == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.PropertyName));
            }

            var testedValue = propertyTestedInfo.GetValue(validationContext.ObjectInstance, null);

            TransactionParams objParam = new TransactionParams();
            objParam.TXN_ID = Convert.ToInt16(testedValue);
            ACC_SCREEN_DESIGN_PARAM_DETAILS designParams = objCommonFunction.getDetailsDesignParam(objParam);
            if ((designParams.PIU_REQ == "Y" && validationContext.DisplayName == "PIU Name" || //Department
                designParams.CON_REQ == "Y" && validationContext.DisplayName == "Company Name" ||
                designParams.AGREEMENT_REQ == "Y" && validationContext.DisplayName == "Agreement Name" ||
                designParams.YEAR_REQ == "Y" && validationContext.DisplayName == "Sanction Year" ||
                designParams.PKG_REQ == "Y" && validationContext.DisplayName == "Package" ||
                designParams.ROAD_REQ == "Y" && validationContext.DisplayName == "Road Name" 
                ) && value.ToString().Trim().Equals("0"))
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
            rule.ValidationParameters["parentdropdown"] = metadata.DisplayName;
            yield return rule;
        }

    }
    
    public class IsDetailsAmountGreater : ValidationAttribute, IClientValidatable
    {
        private string DefaultErrorMessage = "Invalid Amount";  
        private readonly string ObProperty;
        private readonly string ObCreditDebit;

        public IsDetailsAmountGreater(string obProperty, string obCreditDebit)
        {
            this.ObProperty = obProperty;
            this.ObCreditDebit = obCreditDebit;    
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(DefaultErrorMessage, name, ObProperty);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            TransactionParams objParam = new TransactionParams();
            OpeningBalanceDAL objDAL = new OpeningBalanceDAL();
            String amountToCompare = String.Empty;
            objParam.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
            objParam.LVL_ID = PMGSYSession.Current.LevelId;
            objParam.FUND_TYPE = PMGSYSession.Current.FundType;
            objParam.BILL_TYPE = "O";

            var propertyTestedInfo = validationContext.ObjectType.GetProperty(this.ObProperty);
            var obpropertyTestedInfo = validationContext.ObjectType.GetProperty(this.ObCreditDebit);

            if (propertyTestedInfo == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.ObProperty));
            }

            if (obpropertyTestedInfo == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.ObCreditDebit));
            }

            var testedValue1 = propertyTestedInfo.GetValue(validationContext.ObjectInstance, null);
            var testedValue2 = obpropertyTestedInfo.GetValue(validationContext.ObjectInstance, null);
            if (testedValue1 != null)
            {
                objParam.TXN_NO = Convert.ToInt16(testedValue1);
            }
            else
            {
                objParam.TXN_NO = 0;
            }
           
            if (testedValue2.Equals("D"))
            {
                objParam.BILL_NO = "1";
                amountToCompare = objDAL.GetAssetAmountDetails(objParam);

                if ((Convert.ToDecimal(value) + Convert.ToDecimal(amountToCompare.Split('$')[1]) > Convert.ToDecimal(amountToCompare.Split('$')[0])))
                {
                    DefaultErrorMessage = "Total Asset details amount must be less than Gross amount";
                    var message = FormatErrorMessage(validationContext.DisplayName);
                    return new ValidationResult(message);
                }
            }
            else
            {
                objParam.BILL_NO = "2";
                amountToCompare = objDAL.GetAssetAmountDetails(objParam);
                if ((Convert.ToDecimal(value) + Convert.ToDecimal(amountToCompare.Split('$')[1]) > Convert.ToDecimal(amountToCompare.Split('$')[0])))
                {
                    DefaultErrorMessage = "Total Liability details amount must be less than Gross amount";
                    var message = FormatErrorMessage(validationContext.DisplayName);
                    return new ValidationResult(message);
                }
            }

            return ValidationResult.Success;  
        }


        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "isdetailsamountgreater"
            };
            rule.ValidationParameters["obtype"] = this.ObCreditDebit;           
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