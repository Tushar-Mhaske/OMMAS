using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace PMGSY.Models.AccountsReports
{
    public class AssetRegisterViewModel
    {
        [Display(Name="SRRDA")]
        public string SRRDA { get; set; }

        [Display(Name = "DPIU")]
        public int DPIU { get; set; }

        public string SRRDADPIU { get; set; }

        [Display(Name="Month")]
        public short Month { get; set; }

        [Display(Name = "Year")]
        public short Year { get; set; }

        public string monthlyPeriodicFundWise { get; set; }
        
        [Display(Name = "Periodic")]
        public string Period { get; set; }

        [Display(Name = "From Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "From Date is not in valid format")]
        public string FromDate { get; set; }

        [Display(Name = "To Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "To Date is not in valid format")]
        [AssetDateValidation("FromDate", ErrorMessage = "To Date must be greater than or equal to From Date.")]
        [AssetCurrentDateValidation("FromDate", ErrorMessage = "To Date must be less than or equal to Today's Date.")]

        public string ToDate { get; set; }

        [Display(Name="Fund Type")]
        public string FundCentralState { get; set; }

        [Display(Name = "Name Of DPIU")]
        public string DPIUName { get; set; }

        [Display(Name = "Name Of SRRDA")]
        public string NodalAgencyName { get; set; }
       
        public string MonthName{ get; set; } 

        public string FundStateCentralName{ get; set; }

        public string AssetPurchaseDetails { get; set; }

        [Display(Name = "Classification Code")]
        public string ClassificationCode { get; set; }
                
        public string FundDescription { get; set; }

        //lst Asset Register List contains Asset Register Details
        public List<USP_ACC_RPT_REGISTER_DURABLE_ASSETS_Result> lstAssetRegisterDetails { get; set; }

        //lst of Asset Register Classification Code 
        public List<UDF_ACC_GET_ASSET_HEADS_Result> lstAssetRegisterClassificationDetails { get; set; }

        public Decimal? TotalAmount { get; set; }

        //Report Header parameter
        public string FundTypeName { get; set; }
        public string ReportName { get; set; }
        public string ReportParagraphName { get; set; }
        public string ReportFormNumber { get; set; }
    }

    public class AssetDateValidationAttribute : ValidationAttribute, IClientValidatable
    {

        // private const string _defaultErrorMessage = "Start date must be less than end date.";
        private string _basePropertyName;

        public AssetDateValidationAttribute(string basePropertyName)
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

            //Get Value of the property  

            //var startDate = (DateTime)basePropertyInfo.GetValue(validationContext.ObjectInstance, null);            
            //var thisDate = (DateTime)value;  

            var sDate = basePropertyInfo.GetValue(validationContext.ObjectInstance, null);
            var eDate = value;

            if (sDate != null && eDate != null)
            {
                var startDate = ConvertStringToDate(sDate.ToString());
                var thisDate = ConvertStringToDate(eDate.ToString());

                //Actual comparision  
                if (thisDate < startDate)
                {
                    var message = FormatErrorMessage(validationContext.DisplayName);
                    return new ValidationResult(message);
                }

                //System.DateTime toDaysDate = System.DateTime.Now;

                //if (thisDate > toDaysDate)
                //{
                //    //var message = "To date must be less than or equal to today's date.";
                //    var message = FormatErrorMessage(validationContext.DisplayName);
                //    return new ValidationResult(message);
                //}
            }

            //Default return - This means there were no validation error  
            //return null;
            return ValidationResult.Success;
        }


        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            //yield return new ModelClientValidationRule
            //{
            //    ErrorMessage = FormatErrorMessage(metadata.DisplayName), 
            //    ValidationType = "datecomparefieldvalidator"
            //};

            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                ValidationType = "datecomparefieldvalidator"
            };
            rule.ValidationParameters["date"] = this._basePropertyName;
            yield return rule;

        }


        public DateTime? ConvertStringToDate(string dateToConvert)
        {
            DateTime MyDateTime;
            MyDateTime = new DateTime();
            // MyDateTime = DateTime.Parse(dateToConvert);
            MyDateTime = DateTime.ParseExact(dateToConvert, "dd/MM/yyyy", null);
            //Convert.ToDateTime(dateToConvert);         //DateTime.ParseExact(dateToConver, "dd/MM/yyyy",null);
            return MyDateTime;
        }

    }

    public class AssetCurrentDateValidationAttribute : ValidationAttribute, IClientValidatable
    {

        // private const string _defaultErrorMessage = "Start date must be less than end date.";
        private string _basePropertyName;

        public AssetCurrentDateValidationAttribute(string basePropertyName)
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

            //Get Value of the property  

            //var startDate = (DateTime)basePropertyInfo.GetValue(validationContext.ObjectInstance, null);            
            //var thisDate = (DateTime)value;  

            var sDate = basePropertyInfo.GetValue(validationContext.ObjectInstance, null);
            var eDate = value;

            if (eDate != null)
            {
                var thisDate = ConvertStringToDate(eDate.ToString());

                System.DateTime toDaysDate = System.DateTime.Now;  
                if (thisDate > toDaysDate)
                {
                    var message = FormatErrorMessage(validationContext.DisplayName);
                    return new ValidationResult(message);
                }
            }
            return ValidationResult.Success;
        }


        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            //yield return new ModelClientValidationRule
            //{
            //    ErrorMessage = FormatErrorMessage(metadata.DisplayName), 
            //    ValidationType = "datecomparefieldvalidator"
            //};

            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                ValidationType = "currentdatefieldvalidator"
            };
            rule.ValidationParameters["date"] = this._basePropertyName;
            yield return rule;

        }


        public DateTime? ConvertStringToDate(string dateToConvert)
        {
            DateTime MyDateTime;
            MyDateTime = new DateTime();
            MyDateTime = DateTime.ParseExact(dateToConvert, "dd/MM/yyyy", null);
            return MyDateTime;
        }

    }


}