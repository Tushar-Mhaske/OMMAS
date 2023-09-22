using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using PMGSY.Extensions;

namespace PMGSY.Models
{
    public class CommonFiltersModel
    {
        /// <summary>
        /// Boolean values for loading related drpDwn
        /// </summary>
        public string State { get; set; }
        public string District { get; set; }
        public string Block { get; set; }
        public string Village { get; set; }
        public string Habitation { get; set; }
        public string Stream { get; set; }
        public string Batch { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }

    }


    //class for custom validation for boolean
    public class IsBooleanValidator : ValidationAttribute, IClientValidatable
    {
        bool IsBool;
        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            if (value != null)
            {
                if (!Boolean.TryParse(value.ToString(), out IsBool))
                {
                    return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
                }
                    
            }


            return ValidationResult.Success;
        }



        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            yield return new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                //This is the name of the method aaded to the jQuery validator method (must be lower case)
                ValidationType = "isbooleanvalidator"
            };
        }
    }//end custom validation
}