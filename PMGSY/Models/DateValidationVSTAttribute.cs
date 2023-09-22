using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace PMGSY.Models
{
    public class DateValidationVSTAttribute : ValidationAttribute,IClientValidatable
    {

       // private const string _defaultErrorMessage = "Start date must be less than end date.";
        private string _basePropertyName;

        public DateValidationVSTAttribute(string basePropertyName)
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
            try
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
                }

                //Default return - This means there were no validation error  
                //return null;
                return ValidationResult.Success;
            }
            catch(Exception ex)
            {
               // var message = FormatErrorMessage(validationContext.DisplayName);
                return null; //new ValidationResult(message);
            }
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
}