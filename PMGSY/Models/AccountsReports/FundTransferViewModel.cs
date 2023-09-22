using PMGSY.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.AccountsReports
{
    public class FundTransferViewModel
    {
        [Required(ErrorMessage="Please select month.")]
        [Range(1, 12, ErrorMessage = "Please select month.")]
        public string Month { get; set; }

        [Required(ErrorMessage = "Please select year.")]
        [Range(1990, 2099, ErrorMessage = "Please select year.")]
        public string Year { get; set; }

        [Required(ErrorMessage = "Please select head.")]
        [Range(1, Int64.MaxValue, ErrorMessage = "Please select head.")]
        public string HeadCode { get; set; }

        //[Required(ErrorMessage = "Please select state.")]
        //[Range(1, Int64.MaxValue, ErrorMessage = "Please select state.")]
        [RequredStateAttribute("LevelId", ErrorMessage = "Please select State")]
        public string StateCode { get; set; }

        //[Required(ErrorMessage = "Please select DPIU.")]
        //[Range(1, Int64.MaxValue, ErrorMessage = "Please select DPIU.")]

        //[RequredPiuAttribute("LevelId", ErrorMessage = "Please select DPIU")]
        public string DPIUCode { get; set; }

        public string LevelId {                                   
            get {
                return PMGSYSession.Current.LevelId.ToString();
            }
            set {
                PMGSYSession.Current.LevelId.ToString();
            }
        }

        public List<USP_ACC_RPT_REGISTER_PIUWISE_FUND_TRANSFERRED_Result> lstFundTransfer { get; set; }

        public Nullable<Decimal> TotalDebit { get; set; }

        public Nullable<Decimal> TotalCredit { get; set; }

        public Nullable<Decimal> OpeningBalance { get; set; }

        public string HeadName { get; set; }

        public string StateName { get; set; }

        public string DPIUName { get; set; }

        public string MonthName { get; set; }

        public string YearName { get; set; }

        public string ReportName { get; set; }

        public string ReportNumber { get; set; }
        public string ReportPara { get; set; }

        public string FundName { get; set; }

        public Int32 TotalRecord { get; set; }

        //public UDF_ACC_RPT_GET_DPIUWISEOB_FOR_BANK_AUTHORIZATION_Result OpeningBalnce { get; set; }

    }


    //Added By Abhishek Kamble   2-dec-2013
    public class RequredPiuAttribute : ValidationAttribute, IClientValidatable
    {
        // private const string _defaultErrorMessage = "Start date must be less than end date.";
        private string _basePropertyName;

        public RequredPiuAttribute(string basePropertyName)
        //: base(_defaultErrorMessage)
        {
            _basePropertyName = basePropertyName;
        }

        //Override default FormatErrorMessage Method  
        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, _basePropertyName);
        }

        //Override IsValid  
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //Get PropertyInfo Object  
            var basePropertyInfo = validationContext.ObjectType.GetProperty(_basePropertyName);

            var levelId = basePropertyInfo.GetValue(validationContext.ObjectInstance, null);
            var dpiu = value;

            //if (sDate != null && eDate != null)
            //{

            //Actual Validation 
            if (((Convert.ToInt32(levelId) == 4) || (Convert.ToInt32(levelId) == 6)) && (Convert.ToInt32(dpiu) == 0))
            {
                var message = FormatErrorMessage(validationContext.DisplayName);
                return new ValidationResult(message);
            }

            return ValidationResult.Success;
        }
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                ValidationType = "dpiuvalidator"
            };
            rule.ValidationParameters["dpiu"] = this._basePropertyName;
            yield return rule;
        }
    }


    //Added By Abhishek Kamble   2-dec-2013
    public class RequredStateAttribute : ValidationAttribute, IClientValidatable
    {
        // private const string _defaultErrorMessage = "Start date must be less than end date.";
        private string _basePropertyName;

        public RequredStateAttribute(string basePropertyName)
        //: base(_defaultErrorMessage)
        {
            _basePropertyName = basePropertyName;
        }

        //Override default FormatErrorMessage Method  
        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, _basePropertyName);
        }

        //Override IsValid  
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //Get PropertyInfo Object  
            var basePropertyInfo = validationContext.ObjectType.GetProperty(_basePropertyName);

            var levelId = basePropertyInfo.GetValue(validationContext.ObjectInstance, null);
            var dpiuSrrda = value;

            //if (sDate != null && eDate != null)
            //{

            //Actual Validation 
            if (((Convert.ToInt32(levelId) == 4) || (Convert.ToInt32(levelId) == 6)) && (Convert.ToInt32(dpiuSrrda) == 0))
            {
                var message = FormatErrorMessage(validationContext.DisplayName);
                return new ValidationResult(message);
            }

            return ValidationResult.Success;
        }
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                ValidationType = "statevalidator"
            };
            rule.ValidationParameters["state"] = this._basePropertyName;
            yield return rule;
        }
    }
}