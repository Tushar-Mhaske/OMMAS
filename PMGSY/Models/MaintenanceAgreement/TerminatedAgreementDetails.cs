using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using PMGSY.DAL.Agreement;
using PMGSY.Models.Agreement;
using PMGSY.Extensions;
using PMGSY.Models.Proposal;

namespace PMGSY.Models.MaintenanceAgreement
{
    public class TerminatedAgreementDetails
    {
        public TerminatedAgreementDetails()
        {
            PMGSYScheme = PMGSYSession.Current.PMGSYScheme;
        }

        public string EncryptedIMSPRRoadCode { get; set; }

        public int MANE_CONTRACT_ID { get; set; }

        [Display(Name = "Agreement Number")]
        public string MANE_AGREEMENT_NUMBER { get; set; }

        [Display(Name = "Agreement Date")]
        public string MANE_AGREEMENT_DATE { get; set; }

        [Display(Name = "Construction Completion Date")]
        [Required(ErrorMessage = "Construction Completion Date is required.")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Construction Completion Date must be in dd/mm/yyyy format.")]
        public string MANE_CONSTR_COMP_DATE { get; set; }

        [Display(Name = "Maintenance Start Date")]
        public string MANE_MAINTENANCE_START_DATE { get; set; }

        [Display(Name = "Maintenance End Date")]
        public string MANE_MAINTENANCE_END_DATE { get; set; }

        [Display(Name = "Maintenance Cost Year1 (Rs in Lakhs)")]
        [Required(ErrorMessage = "Maintenance Cost Year1 is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Maintenance Cost Year1 is not in valid format. ")]
        [Range(0, 999.99, ErrorMessage = "The valid format for Maintenance Cost Year1 is 999.99.")]
        public decimal? MANE_YEAR1_AMOUNT { get; set; }

        [Display(Name = "Maintenance Cost Year2 (Rs in Lakhs)")]
        [Required(ErrorMessage = "Maintenance Cost Year2 is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Maintenance Cost Year2 is not in valid format. ")]
        [Range(0, 999.99, ErrorMessage = "The valid format for Maintenance Cost Year2 is 999.99.")]
        public decimal? MANE_YEAR2_AMOUNT { get; set; }

        [Display(Name = "Maintenance Cost Year3 (Rs in Lakhs)")]
        [Required(ErrorMessage = "Maintenance Cost Year3 is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Maintenance Cost Year3 is not in valid format. ")]
        [Range(0, 999.99, ErrorMessage = "The valid format for Maintenance Cost Year3 is 999.99.")]
        public decimal? MANE_YEAR3_AMOUNT { get; set; }

        [Display(Name = "Maintenance Cost Year4 (Rs in Lakhs)")]
        [Required(ErrorMessage = "Maintenance Cost Year4 is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Maintenance Cost Year4 is not in valid format. ")]
        [Range(0, 999.99, ErrorMessage = "The valid format for Maintenance Cost Year4 is 999.99.")]
        public decimal? MANE_YEAR4_AMOUNT { get; set; }

        [Display(Name = "Maintenance Cost Year5 (Rs in Lakhs)")]
        [Required(ErrorMessage = "Maintenance Cost Year5 is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Maintenance Cost Year5 is not in valid format. ")]
        [Range(0, 999.99, ErrorMessage = "The valid format for Maintenance Cost Year5 is 999.99.")]
        public decimal? MANE_YEAR5_AMOUNT { get; set; }

        [Display(Name = "Renewal Cost (Rs in Lakhs)")]
        [CustomTerminatedRequiredAttribute("PMGSYScheme", ErrorMessage = "Renewal Cost is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Renewal Cost is not in valid format. ")]
        [Range(0, 999.99, ErrorMessage = "Renewal Cost is not in valid format.")]
        public decimal? MANE_YEAR6_AMOUNT { get; set; }

        [Display(Name = "Handover Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Handover Date must be in dd/mm/yyyy format.")]
        //[VerifyDateAttribute("MANE_MAINTENANCE_START_DATE", "MANE_CONSTR_COMP_DATE", ErrorMessage = "Handover date must be greater than or equal to maintenance start date and construction completion date.")]
        public string MANE_HANDOVER_DATE { get; set; }

        [Display(Name = "Handover To")]
        [RegularExpression(@"^([a-zA-Z0-9 ._',/()-]+)$", ErrorMessage = "Handover To is not in valid format.")]
        [StringLength(255, ErrorMessage = "Handover To must be less than 255 characters.")]
        public string MANE_HANDOVER_TO { get; set; }

        public short PMGSYScheme { get; set; }

        [Display(Name = "Contractor Name")]
        public string ContractorName { get; set; }

        [Display(Name = "Contractor PAN")]
        public string ContractorPAN { get; set; }

        public int recordId { get; set; }

    }

    public class VerifyDateAttribute : ValidationAttribute, IClientValidatable
    {
        private string _basePropertyName;
        private string _basePropertyName_Second;

        public VerifyDateAttribute(string basePropertyName, string basePropertyName_Second)
        {
            _basePropertyName = basePropertyName;
            _basePropertyName_Second = basePropertyName_Second;
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

                var basePropertyInfo_Second = validationContext.ObjectType.GetProperty(_basePropertyName_Second);


                //Get Value of the property  

                var sDate = basePropertyInfo.GetValue(validationContext.ObjectInstance, null);
                var date_Second = basePropertyInfo_Second.GetValue(validationContext.ObjectInstance, null);
                var eDate = value;

                if (sDate != null && eDate != null)
                {
                    var startDate = ConvertStringToDate(sDate.ToString());
                    var thisDate = ConvertStringToDate(eDate.ToString());
                    var secondDate = ConvertStringToDate(date_Second.ToString());

                    //Actual comparision  
                    if (thisDate < startDate || thisDate < secondDate)
                    {
                        var message = FormatErrorMessage(validationContext.DisplayName);
                        return new ValidationResult(message);
                    }
                }

                return ValidationResult.Success;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            yield return new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                //This is the name of the method aaded to the jQuery validator method (must be lower case)
                ValidationType = "checkdatevalidator"
            };

        }

        public DateTime? ConvertStringToDate(string dateToConvert)
        {
            DateTime MyDateTime;
            MyDateTime = new DateTime();
            MyDateTime = DateTime.ParseExact(dateToConvert, "dd/MM/yyyy", null);
            return MyDateTime;
        }

    }

    public class CustomTerminatedRequiredAttribute : ValidationAttribute, IClientValidatable
    {
        public string Property { get; set; }


        public CustomTerminatedRequiredAttribute(string Property)
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
                int scheme = Convert.ToInt32(PropertyInfo.GetValue(validationContext.ObjectInstance, null).ToString());

                if (scheme == 2 && value == null)
                {
                    return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
                }
            }

            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                //This is the name of the method aaded to the jQuery validator method (must be lower case)
                ValidationType = "custommanerequired"
            };

            rule.ValidationParameters["fieldvaluemane"] = this.Property;
            yield return rule;

        }
    }
}


   