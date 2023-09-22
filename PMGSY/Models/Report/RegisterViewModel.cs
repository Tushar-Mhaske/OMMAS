using PMGSY.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Report
{
    public class RegisterViewModel
    {

        public RegisterViewModel()
        {
            lstFinancialYears = new List<SelectListItem>();
            lstHeads = new List<SelectListItem>();
            lstMonths = new List<SelectListItem>();
            lstYears = new List<SelectListItem>();
            lstFundingAgency = new List<SelectListItem>();
            lstPIU = new List<SelectListItem>();
            lstSRRDA = new List<SelectListItem>();
        }

        [Required(ErrorMessage="Head is required.")]
        [Display(Name="Head")]
        [Range(1,Int32.MaxValue,ErrorMessage="Please select proper head.")]
        public int HeadId { get; set; }

        //[Required(ErrorMessage="Funding Agency is required.")]
        [Display(Name="Funding Agency")]
        //[Range(1,Int32.MaxValue,ErrorMessage="Funding Agency is required.")]
        public int FundingAgencyCode { get; set; }
        
        [Required(ErrorMessage="Month is required.")]
        [RequiredDependentMonthlyField("DurationType", ErrorMessage = "Please select Month.")]
        [Display(Name="Month")]
        public int Month { get; set; }

        [Display(Name="Year")]
        [RequiredDependentYearField("DurationType", ErrorMessage = "Please select Year.")]
        public int Year { get; set; }

        [Display(Name="Year")]
        //[RequiredDependentField("DurationType", ErrorMessage = "Please select Year.")]
        //[Range(2000, 2099, ErrorMessage = "Select proper financial year.")]
        [RequiredDependentYearlyField("FinancialYear", ErrorMessage = "Financial Year is required.")]
        public int FinancialYear { get; set; }

        [Display(Name="Duration")]
        [Required(ErrorMessage="Duration Type is required.")]
        
        public string DurationType { get; set; }

        [Display(Name="Report Type")]
        public string ReportType { get; set; }

        public string ReportTitle { get; set; }

        public List<SelectListItem> lstMonths { get; set; }

        public List<SelectListItem> lstYears { get; set; }

        public List<SelectListItem> lstFinancialYears { get; set; }

        public List<SelectListItem> lstHeads { get; set; }

        public List<SelectListItem> lstFundingAgency { get; set; }

        public List<SelectListItem> lstPIU { get; set; }

        public List<SelectListItem> lstSRRDA { get; set; }
        
        public string NodalAgency { get; set; }

        public int AdminCode { get; set; }

        public string ReportFormNumber { get; set; }

        public string FundTypeName { get; set; }

        public string ReportName { get; set; }

        public string ReportParagraphName { get; set; }

        public string StateName { get; set; }

        public string SRRDADPIU { get; set; }

        public string HeadName { get; set; }

        public string DPIUName { get; set; }

        public string MonthName { get; set; }

        public int LevelId { get; set; }

        public int StateCode { get;set;}

        public int Collaboration { get; set; }

        //[RequredDPIU("ReportType", ErrorMessage = "Please select DPIU")]
        public int DPIUCode { get; set; }

        public int HeadCategoryId { get; set; }

        public int SRRDACode { get; set; }

    }


    public class RequiredDependentMonthlyField : ValidationAttribute, IClientValidatable
    {
        private readonly string PropertyType;

        public RequiredDependentMonthlyField(string propertyType)
        {
            this.PropertyType = propertyType;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyTestedType = validationContext.ObjectType.GetProperty(this.PropertyType);

            if (propertyTestedType == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.PropertyType));
            }

            var type = propertyTestedType.GetValue(validationContext.ObjectInstance, null);
            int field = 0;
            if (value != null)
            {
                field = Convert.ToInt32(value);
            }
                                                            
          
                if (type.ToString().ToLower() == "m")
                {
                    if (field == null || field == 0)
                    {
                        return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                    }
                    else
                    {
                        return ValidationResult.Success;
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
                ValidationType = "requireddependentmonthlyfield"
            };
            rule.ValidationParameters.Add("type", this.PropertyType);
            yield return rule;
        }
    }

    public class RequiredDependentYearField : ValidationAttribute, IClientValidatable
    {
        private readonly string PropertyType;

        public RequiredDependentYearField(string propertyType)
        {
            this.PropertyType = propertyType;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyTestedType = validationContext.ObjectType.GetProperty(this.PropertyType);

            if (propertyTestedType == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.PropertyType));
            }

            var type = propertyTestedType.GetValue(validationContext.ObjectInstance, null);
            int field = 0;
            if (value != null)
            {
                field = Convert.ToInt32(value);
            }

           
                if (type.ToString().ToLower() == "m")
                {
                    if (field == null || field == 0)
                    {
                        return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                    }
                    else
                    {
                        return ValidationResult.Success;
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
                ValidationType = "requireddependentyearfield"
            };
            rule.ValidationParameters.Add("year", this.PropertyType);
            yield return rule;
        }
    }



    public class RequiredDependentYearlyField : ValidationAttribute, IClientValidatable
    {
        private readonly string PropertyFinYear;

        public RequiredDependentYearlyField(string propertyFinYear)
        {
            this.PropertyFinYear = propertyFinYear;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyTestedYear = validationContext.ObjectType.GetProperty(this.PropertyFinYear);

            if (propertyTestedYear == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.PropertyFinYear));
            }

            int year = Convert.ToInt32(propertyTestedYear.GetValue(validationContext.ObjectInstance, null));

            var durationType = value;

            if (durationType.ToString().ToLower() == "y")
            {
                if (year == 0 || year == null)
                {
                    return new ValidationResult("Financial Year is required.");
                }
                else
                {
                    return ValidationResult.Success;
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
                ValidationType = "requireddependentyearlyfield"
            };
            rule.ValidationParameters.Add("finyear", this.PropertyFinYear);
            yield return rule;
        }
    }


    //Added By Abhishek Kamble   2-dec-2013
    //public class RequredDPIUAttribute : ValidationAttribute, IClientValidatable
    //{
    //    // private const string _defaultErrorMessage = "Start date must be less than end date.";
    //    private string _basePropertyName;

    //    public RequredDPIUAttribute(string basePropertyName)
    //    //: base(_defaultErrorMessage)
    //    {
    //        _basePropertyName = basePropertyName;
    //    }

    //    //Override default FormatErrorMessage Method  
    //    public override string FormatErrorMessage(string name)
    //    {
    //        return string.Format(ErrorMessageString, name, _basePropertyName);
    //    }

    //    //Override IsValid  
    //    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    //    {
    //        //Get PropertyInfo Object  
    //        var basePropertyInfo = validationContext.ObjectType.GetProperty(_basePropertyName);

    //        var reportType= basePropertyInfo.GetValue(validationContext.ObjectInstance, null);
    //        var dpiu = value;

    //        //Actual Validation 
    //        if (((PMGSYSession.Current.LevelId == 4) || (PMGSYSession.Current.LevelId == 6)) && (Convert.ToInt32(dpiu) == 0))
    //        {
    //            var message = FormatErrorMessage(validationContext.DisplayName);
    //            return new ValidationResult(message);
    //        }

    //        return ValidationResult.Success;
    //    }
    //    public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
    //    {
    //        var rule = new ModelClientValidationRule
    //        {
    //            ErrorMessage = FormatErrorMessage(metadata.DisplayName),
    //            ValidationType = "dpiuvalidator"
    //        };
    //        rule.ValidationParameters["dpiu"] = this._basePropertyName;
    //        yield return rule;
    //    }
    //}
}