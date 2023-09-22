using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PMGSY.Models
{
    public class FromToDateValidationAttribute:ValidationAttribute
    {

        private const string _defaultErrorMessage = "Valid to date must be greater than valid from date.";  
        private string _basePropertyName;

        public FromToDateValidationAttribute(string basePropertyName)
            : base(_defaultErrorMessage)  
        {  
            _basePropertyName = basePropertyName;  
        }  
   
        //Override default FormatErrorMessage Method  
        public override string FormatErrorMessage(string name)  
        {  
            return string.Format(_defaultErrorMessage, name, _basePropertyName);  
        }  
   
        //Override IsValid  
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)  
        {  
            //Get PropertyInfo Object  
            var basePropertyInfo = validationContext.ObjectType.GetProperty(_basePropertyName);  
   
            //Get Value of the property  

            var sDate = basePropertyInfo.GetValue(validationContext.ObjectInstance, null);
            var eDate = value;

            if (sDate != null && eDate != null)
            {
                var startDate = ConvertStringToDate(sDate.ToString());
                var thisDate = ConvertStringToDate(eDate.ToString());

                //Actual comparision  
                if (thisDate <= startDate)
                {
                    var message = FormatErrorMessage(validationContext.DisplayName);
                    return new ValidationResult(message);
                }
            }
   
            //Default return - This means there were no validation error  
            return null;  
        }

        public DateTime? ConvertStringToDate(string dateToConver)
        {
            DateTime MyDateTime;
            MyDateTime = new DateTime();
            MyDateTime = DateTime.ParseExact(dateToConver, "dd/MM/yyyy", null);
            return MyDateTime;
        }



    }
}