using PMGSY.BAL.MaintainanceInspection;
using PMGSY.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.MaintainanceInspection
{
    public class MaintenanceProgressViewModel
    {
        public MaintenanceProgressViewModel()
        {
            lstMaintenanceNo = new List<SelectListItem>();
            lstMANEAgreements = new List<SelectListItem>();
        }

        [Required(ErrorMessage = "Please Select Maintenance Type.")]
        [RegularExpression("[PR]", ErrorMessage = "Please Select either Periodic or Routine Maintenance")]
        public string maintenanceType { get; set; }

        public int ProposalCode { get; set; }

        public int ProposalContractCode { get; set; }

        [Required(ErrorMessage="Please Select Month.")]
        [Display(Name="Month")]
        [CompareAgrementMonth("ProgramYear", "AgreementDate", ErrorMessage = "Month must be greater than or equal to Agreement Start Date.")]
        [Range(1,12,ErrorMessage="Please Select Month.")]
        public int ProgramMonth { get; set; }

        [Required(ErrorMessage="Please Select Year.")]
        [CompareAgrementYear("AgreementYear", "AgreementDate", ErrorMessage = "Year must be greater than or equal to Agreement Start Year.")]
        [Display(Name="Year")]
        [Range(2000,2099,ErrorMessage="Please Select Year.")]
        public int ProgramYear { get; set; }

        public decimal? ValueOfWorkLastMonth { get; set; }

        [Required(ErrorMessage="Value of work done is required.")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Invalid Value of Work,Can only contains 8 Numeric digits and 2 digit after decimal place.")]
        [Display(Name="Value Of Work During This Month(Rs. in Lakh)")]
        public decimal? ValueOfWorkThisMonth { get; set; }

        public decimal? PaymentLastMonth { get; set; }

        [Required(ErrorMessage="Payment of work done is required.")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Invalid Value of payment,Can only contains 8 Numeric digits and 2 digit after decimal place")]
        [CompareFinancialValidation("ValueOfWorkThisMonth", ErrorMessage = "Payment amount should not be greater than value of work during this month.")]
        [Display(Name="Payment Of Work During This Month(Rs. in Lakh)")]
        public decimal? PaymentThisMonth { get; set; }

        [Required(ErrorMessage="Please Select Option.")]
        [Display(Name="Is Final Payment")]
        [RegularExpression("[YN]",ErrorMessage="Please Select Option.")]
        public string FinalPaymentFlag { get; set; }

        [Display(Name="Final Payment Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Final Payment Date must be in dd/mm/yyyy format.")]
        public string FinalPaymentDate { get; set; }

        public string Operation { get; set; }

        public int PreviousYear { get; set; }

        [CompareMonth("ProgramMonth", "ProgramYear", "PreviousYear", "Operation", "ProposalCode", "ProposalContractCode", ErrorMessage = "Month and Year must be greater than previous entered month and year")]
        public int PreviousMonth { get; set; }

        public List<SelectListItem> lstMaintenanceNo { get; set; }
        public List<SelectListItem> lstMANEAgreements { get; set; }

        public string IsFinalPaymentBefore { get; set; }

        public string EncyptedProgressCode { get; set; }
        public decimal? TotalValueofwork {get;set;}
        public decimal? TotalPayment {get;set;}
        public decimal? LastPaymentValue {get;set;}
        public decimal? LastMonthValue {get;set;}
        public string CompleteStatus{get;set;}
        public decimal? AgreementTotal {get;set;}
        public decimal? AgreementCost {get;set;}
        public string AgreementDate { get; set; }
        public int AgreementYear { get; set; }
        public int AgreementMonth { get; set; }

        public int ContractNumber { get; set; }
        public string BlockName { get; set; }
        public string Package { get; set; }
        public string RoadName { get; set; }
        public decimal? SanctionLength { get; set; }
        public decimal? OverallCost { get; set; }

        public int ContractCode { get; set; }


    }


    public class CompareAgrementYear : ValidationAttribute, IClientValidatable
    {
        private readonly string dateComponent;
        private readonly string date;

        public CompareAgrementYear(string dateValue, string date)
        {
            this.dateComponent = dateValue;
            this.date = date;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyTestedDateComponent = validationContext.ObjectType.GetProperty(this.dateComponent);
            var propertyTestedDate = validationContext.ObjectType.GetProperty(this.date);

            DateTime? dateToCompare = null;
            if (propertyTestedDateComponent == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.date));
            }

            int dateValue = Convert.ToInt32(propertyTestedDateComponent.GetValue(validationContext.ObjectInstance, null));
            var date = propertyTestedDate.GetValue(validationContext.ObjectInstance, null);

            if (date != null)
            {
                dateToCompare = new CommonFunctions().GetStringToDateTime(date.ToString());
            }

            if (value == null)
            {
                return ValidationResult.Success;
            }

            if (Convert.ToInt32(value) >= dateToCompare.Value.Year)
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "compareagreementyear"
            };
            yield return rule;
        }

    }


    public class CompareAgrementMonth : ValidationAttribute, IClientValidatable
    {
        private readonly string dateComponent;
        private readonly string date;

        public CompareAgrementMonth(string dateValue, string date)
        {
            this.dateComponent = dateValue;
            this.date = date;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyTestedDateComponent = validationContext.ObjectType.GetProperty(this.dateComponent);
            var propertyTestedDate = validationContext.ObjectType.GetProperty(this.date);


            if (propertyTestedDateComponent == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.date));
            }

            int progYear = Convert.ToInt32(propertyTestedDateComponent.GetValue(validationContext.ObjectInstance, null));
            var date = propertyTestedDate.GetValue(validationContext.ObjectInstance, null);

            DateTime? dateToCompare = new CommonFunctions().GetStringToDateTime(date.ToString());

            if (value == null)
            {
                return ValidationResult.Success;
            }

            if (dateToCompare.Value.Year < progYear)
            {
                return ValidationResult.Success;
            }
            else if (dateToCompare.Value.Year == progYear)
            {
                if (dateToCompare.Value.Month <= Convert.ToInt32(value))
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
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "compareagreementmonth"
            };
            yield return rule;
        }

    }

    public class CompareMonth : ValidationAttribute, IClientValidatable
    {
        private readonly string PropertyMonth;
        private readonly string PropertyYear;
        private readonly string PropertyPreviousYear;
        private readonly string PropertyOperation;
        private readonly string PropertyProposal;
        private readonly string PropertyContractCode;

        public CompareMonth(string propertyMonth, string PropertyYear, string PropertyPreviousYear, string PropertyOperation,string proposalCode,string contractCode)
        {
            this.PropertyMonth = propertyMonth;
            this.PropertyYear = PropertyYear;
            this.PropertyPreviousYear = PropertyPreviousYear;
            this.PropertyOperation = PropertyOperation;
            this.PropertyContractCode = contractCode;
            this.PropertyProposal = proposalCode;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                var propertyTestedInfo = validationContext.ObjectType.GetProperty(this.PropertyMonth);
                var propertyTestedYear = validationContext.ObjectType.GetProperty(this.PropertyYear);
                var propertyTestedPreviousYear = validationContext.ObjectType.GetProperty(this.PropertyPreviousYear);
                var propertyTestedOperation = validationContext.ObjectType.GetProperty(this.PropertyOperation);
                var propertyTestedProposal = validationContext.ObjectType.GetProperty(this.PropertyProposal);
                var propertyTestedContractCode = validationContext.ObjectType.GetProperty(this.PropertyContractCode);


                if (propertyTestedInfo == null)
                {
                    return new ValidationResult(string.Format("unknown property {0}", this.PropertyMonth));
                }

                int execprogmonth = Convert.ToInt32(propertyTestedInfo.GetValue(validationContext.ObjectInstance, null));
                int execyear = Convert.ToInt32(propertyTestedYear.GetValue(validationContext.ObjectInstance, null));
                int previousyear = Convert.ToInt32(propertyTestedPreviousYear.GetValue(validationContext.ObjectInstance, null));
                var operation = propertyTestedOperation.GetValue(validationContext.ObjectInstance, null);
                int proposalCode = Convert.ToInt32(propertyTestedProposal.GetValue(validationContext.ObjectInstance, null));
                int contractCode = Convert.ToInt32(propertyTestedContractCode.GetValue(validationContext.ObjectInstance, null));

                if (Convert.ToInt32(value) == 0 || value == null)
                {
                    return ValidationResult.Success;
                }

                MaintainanceInspectionBAL objBAL = new MaintainanceInspectionBAL();
                bool status = objBAL.CheckMonthYear(proposalCode,execprogmonth,execyear,contractCode);
                if (status == true)
                {
                    return ValidationResult.Success;
                }

                if (operation.ToString().ToLower() != "e")
                {
                    if (execyear < previousyear)
                    {
                        return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                    }
                    else
                    {
                        if (execyear == previousyear)
                        {
                            if (execprogmonth <= Convert.ToInt32(value))
                            {
                                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                            }
                            else
                            {
                                return ValidationResult.Success;
                            }
                        }
                    }
                }
                return ValidationResult.Success;
            }
            catch
            {
                return ValidationResult.Success;
            }
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "comparemonth"
            };
            rule.ValidationParameters["execprogmonth"] = this.PropertyMonth;
            rule.ValidationParameters["execyear"] = this.PropertyMonth;
            rule.ValidationParameters["previousyear"] = this.PropertyMonth;
            yield return rule;
        }

    }

    public class CompareFinancialValidation : ValidationAttribute, IClientValidatable
    {
        private readonly string PropertyName;

        public CompareFinancialValidation(string propertyName)
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

            var workValue = Convert.ToDecimal(propertyTestedInfo.GetValue(validationContext.ObjectInstance, null));
            var paymentValue = Convert.ToDecimal(value);

            if (workValue < paymentValue)
            {
                if (paymentValue == null)
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
                ValidationType = "comparefinancialvalidation"
            };
            //rule.ValidationParameters["compareworkpayment"] = this.PropertyName;
            yield return rule;
        }
    }
}