using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using PMGSY.Models.Master;
using System.Web.Mvc;
using PMGSY.Extensions;
using System.Text;

namespace PMGSY.Models.SplitWork
{
    public class SplitWorkDetails:IValidatableObject
    {

        public SplitWorkDetails()
        {
            PMGSYScheme = PMGSYSession.Current.PMGSYScheme;
        }

        [UIHint("Hidden")]
        public string EncryptedIMSPRRoadCode { get; set; }

        [UIHint("Hidden")]
        public string EncryptedIMSWorkCode { get; set; }

        [Display(Name = "Work Name")]
      //  [RegularExpression(@"^[a-zA-Z0-9 -&-,()_/.]+$", ErrorMessage = "Work Name is not in valid format.")]
       //working
        [RegularExpression(@"^[a-zA-Z0-9]+[a-zA-Z0-9 ,-/()._]*$", ErrorMessage = "Work Name should contains at least one character or number and starts with alphanumeric value.")]
        [Required(ErrorMessage = "Work Name is required.")]
        [StringLength(255, ErrorMessage = "Work Name must be less than 255 characters.")]
        public string IMS_WORK_DESC { get; set; }

        [Display(Name = "Pavement Length")]
        //[Required(ErrorMessage = "Pavement Length is required.")]
        //[RegularExpression(@"^\d+(\.\d{1,3})?", ErrorMessage = "Pavement Length is not in valid format. ")]
        public decimal? IMS_PAV_LENGTH { get; set; }

        [Display(Name = "Start Chainage")]
        [Required(ErrorMessage = "Start Chainage is required.")] 
        [RegularExpression(@"^\d*(\.\d{1,3})?", ErrorMessage = "Start Chainage is not in valid formate.g.[9.999].")]
        [Range(0, 9999.999, ErrorMessage = "Start Chainage is not in valid format.")]
        public decimal? IMS_START_CHAINAGE { get; set; }

        [Display(Name = "End Chainage")]
        [Required(ErrorMessage = "End Chainage is required.")]
        [RegularExpression(@"^\d*(\.\d{1,3})?", ErrorMessage = "End Chainage is not in valid formate.g.[9.999].")]
        [CompareFieldValidator("IMS_START_CHAINAGE", ErrorMessage = "End Chainage must be greater than start chainage.")]
        [Range(0, 9999.999, ErrorMessage = "End Chainage is not in valid format.")]
        public decimal? IMS_END_CHAINAGE { get; set; }

        [Display(Name = "Pavement Cost")]
        [Required(ErrorMessage = "Pavement Cost is required.")]
        [RegularExpression(@"^\d*(\.\d{1,4})?", ErrorMessage = "Pavement Cost is not in valid format. ")]
        [Range(0.0001, 999999.9999, ErrorMessage = "Pavement Cost is not in valid format (should be greater than 0).")]
        public decimal? IMS_PAV_EST_COST { get; set; }


        [Display(Name = "CD Works Cost")]
        [Required(ErrorMessage = "CD Works Cost is required.")]
        [RegularExpression(@"^\d*(\.\d{1,4})?", ErrorMessage = "CD Works Cost is not in valid format. ")]
        [Range(0, 999999.9999, ErrorMessage = "CD Works Cost is not in valid format (should be greater than 0).")]
        public decimal? IMS_CD_WORKS_EST_COST { get; set; }

        [Display(Name = "Protection Cost")]
        [Required(ErrorMessage = "Protection Cost is required.")]
        [RegularExpression(@"^\d*(\.\d{1,4})?", ErrorMessage = "Protection Cost is not in valid format. ")]
        [Range(0, 999999.9999, ErrorMessage = "Protection Cost is not in valid format (should be greater than 0).")]
        public decimal? IMS_PROTECTION_WORKS { get; set; }

        [Display(Name = "Other Works Cost")]
        [Required(ErrorMessage = "Other Works Cost is required.")]
        [RegularExpression(@"^\d*(\.\d{1,4})?", ErrorMessage = "Other Works Cost is not in valid format. ")]
        [Range(0, 999999.9999, ErrorMessage = "Other Works Cost is not in valid format (should be greater than 0).")]
        public decimal? IMS_OTHER_WORK_COST { get; set; }


        [Display(Name = "State Share")]
        [Required(ErrorMessage = "State Share is required.")]
        [RegularExpression(@"^\d*(\.\d{1,4})?", ErrorMessage = "State Share is not in valid format. ")]
        [Range(0, 999999.9999, ErrorMessage = "State Share is not in valid format (should be greater than 0).")]
        public decimal? IMS_STATE_SHARE { get; set; }

        [Display(Name = "Higher Specification Cost")]
        [CustomRequired("PMGSYScheme", ErrorMessage = "Higher Specification Cost is required.")]
        [RegularExpression(@"^\d*(\.\d{1,4})?", ErrorMessage = "Higher Specification Cost is not in valid format. ")]
        [Range(0, 999999.9999, ErrorMessage = "Higher Specification Cost is not in valid format (should be greater than 0).")]
        public decimal? IMS_HIGHER_SPECIFICATION_COST { get; set; }

        [Display(Name = "Furniture Cost")]
        [CustomRequired("PMGSYScheme", ErrorMessage = "Furniture Cost is required.")]
        [RegularExpression(@"^\d*(\.\d{1,4})?", ErrorMessage = "Furniture Cost is not in valid format. ")]
        [Range(0, 999999.9999, ErrorMessage = "Furniture Cost is not in valid format (should be greater than 0).")]
        public decimal? IMS_FURNITURE_COST { get; set; }

        [Display(Name = "Bridge Work Cost")]
        [RegularExpression(@"^\d+(\.\d{1,4})?", ErrorMessage = "Bridge Work Cost is not in valid format. ")]
        [Range(0, 999999.9999, ErrorMessage = "Bridge Work Cost is not in valid format.")]
        public decimal? IMS_BRIDGE_WORKS_EST_COST { get; set; }

        [Display(Name = "Bridge State Share")]
        [RegularExpression(@"^\d*(\.\d{1,4})?", ErrorMessage = "Bridge State Share is not in valid format. ")]
        [Range(0, 999999.9999, ErrorMessage = "Bridge State Share is not in valid format.")]
        public decimal? IMS_BRIDGE_EST_COST_STATE { get; set; }

        [Display(Name = "Maintenance Cost Year1")]
        [Required(ErrorMessage = "Maintenance Cost Year1 is required.")]
        [RegularExpression(@"^\d*(\.\d{1,4})?", ErrorMessage = "Maintenance Cost Year1 is not in valid format. ")]
        [Range(0, 999999.9999, ErrorMessage = "Maintenance Cost Year1 is not in valid format.")]// (should be greater than 0).
        public decimal? IMS_MAINTENANCE_YEAR1 { get; set; }

        [Display(Name = "Maintenance Cost Year2")]
        [Required(ErrorMessage = "Maintenance Cost Year2 is required.")]
        [RegularExpression(@"^\d*(\.\d{1,4})?", ErrorMessage = "Maintenance Cost Year2 is not in valid format. ")]
        [Range(0, 999999.9999, ErrorMessage = "Maintenance Cost Year2 is not in valid format.")]// (should be greater than 0).
        public decimal? IMS_MAINTENANCE_YEAR2 { get; set; }

        [Display(Name = "Maintenance Cost Year3")]
        [Required(ErrorMessage = "Maintenance Cost Year3 is required.")]
        [RegularExpression(@"^\d*(\.\d{1,4})?", ErrorMessage = "Maintenance Cost Year3 is not in valid format. ")]
        [Range(0, 999999.9999, ErrorMessage = "Maintenance Cost Year3 is not in valid format.")]// (should be greater than 0).
        public decimal? IMS_MAINTENANCE_YEAR3 { get; set; }

        [Display(Name = "Maintenance Cost Year4")]
        [Required(ErrorMessage = "Maintenance Cost Year4 is required.")]
        [RegularExpression(@"^\d*(\.\d{1,4})?", ErrorMessage = "Maintenance Cost Year4 is not in valid format. ")]
        [Range(0, 999999.9999, ErrorMessage = "Maintenance Cost Year4 is not in valid format.")]// (should be greater than 0).
        public decimal? IMS_MAINTENANCE_YEAR4 { get; set; }

        [Display(Name = "Maintenance Cost Year5")]
        [Required(ErrorMessage = "Maintenance Cost Year5 is required.")]
        [RegularExpression(@"^\d*(\.\d{1,4})?", ErrorMessage = "Maintenance Cost Year5 is not in valid format. ")]
        [Range(0, 999999.9999, ErrorMessage = "Maintenance Cost Year5 is not in valid format.")]// (should be greater than 0).
        public decimal? IMS_MAINTENANCE_YEAR5 { get; set; }

        [Display(Name = "Renewal Cost Year6")]
        [CustomRequired("PMGSYScheme", ErrorMessage = "Renewal Cost Year6 is required.")]
        [RegularExpression(@"^\d*(\.\d{1,4})?", ErrorMessage = "Renewal Cost Year6 is not in valid format. ")]
        [Range(0, 999999.9999, ErrorMessage = "Renewal Cost Year6 is not in valid format.")]// (should be greater than 0).
        public decimal? IMS_MAINTENANCE_YEAR6 { get; set; }

        public short SharePercent { get; set; }

        public string SanctionedCostDetails { get; set; }

        public int PMGSYScheme { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            StringBuilder errorMessage = new StringBuilder();
            bool status = true;
            if (PMGSYSession.Current.PMGSYScheme == 2)
            {
                if (IMS_MAINTENANCE_YEAR6 == null)
                {
                    errorMessage = errorMessage.Append("Renewal Cost Year6 is required.");
                    status = false;
                }

                if (IMS_HIGHER_SPECIFICATION_COST == null)
                {
                    errorMessage = errorMessage.Append("Higher Specification Cost is required.");
                    status = false;
                }

                if (IMS_FURNITURE_COST == null)
                {
                    errorMessage = errorMessage.Append("Furniture Cost is required.");
                    status = false;
                }
            }
            else
            {
                status = true;
                
            }

            if (status == false)
            {
                yield return new ValidationResult(errorMessage.ToString());
            }
            else
            {
                yield return ValidationResult.Success;
            }

        }
    }


    public class CustomRequiredAttribute : ValidationAttribute, IClientValidatable
    {
        public string Property { get; set; }


        public CustomRequiredAttribute(string Property)
        {
            this.Property = Property;
        }


        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, Property);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            System.Reflection.PropertyInfo PropertyInfo = validationContext.ObjectInstance.GetType().GetProperty(Property);


            if (PropertyInfo == null)
            {
                return new ValidationResult(string.Format("Property '{0}' is undefined.", Property));
            }


            object PropertyValue = PropertyInfo.GetValue(validationContext.ObjectInstance, null);

            if (PropertyValue != null)
            {
                int propertyValue = Convert.ToInt32(PropertyInfo.GetValue(validationContext.ObjectInstance, null).ToString());

                if (propertyValue == 2 && value == null)
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
                ValidationType = "customrequired"
            };

        }
    }//end custom validation

}