using PMGSY.Common;
using PMGSY.DAL.FundDAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Fund
{
    public class FundReleaseViewModel
    {
        [UIHint("Hidden")]
        public string EncryptedReleaseCode { get; set; }

        [Required(ErrorMessage = "  Please Select State.")]
        [Display(Name="State")]
        public int MAST_STATE_CODE { get; set; }

        [Required(ErrorMessage = "  Please Select Executing Agency.")]
        [Display(Name="Executing Agency")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please Select Executing Agency.")]
        public int ADMIN_NO_CODE { get; set; }
        
        [Required(ErrorMessage="  Please Select Fund Type.")]
        [RegularExpression("[PAM]", ErrorMessage = "  Please Select Fund Type.")]
        [Display(Name="Fund Type")]
        public string MAST_FUND_TYPE { get; set; }


        [Required(ErrorMessage="  Please Select Collaboration.")]
        [Display(Name="Collaboration")]
        public int MAST_FUNDING_AGENCY_CODE { get; set; }

        [Required(ErrorMessage="  Please Select Phase.")]
        [Range(2000, 2099, ErrorMessage = "  Please Select Phase.")]
        [Display(Name="Phase")]
        public int MAST_YEAR { get; set; }


        [Display(Name="Release Type")]
        [RegularExpression("[SC]", ErrorMessage = "  Please Enter Proper Value.")]
        public string MAST_RELEASE_TYPE { get; set; }

        [Display(Name="Sanction Order")]
        public int MAST_TRANSACTION_NO { get; set; }

        [Required(ErrorMessage = "  Please Select Financial Year.")]
        [Display(Name="Release Year")]
        [CompareValidation("MAST_YEAR",ErrorMessage="  Financial Year must be greater than or equal to Phase Year.")]
        [Range(2000, 2099, ErrorMessage = "  Please Select Financial Year.")]
        public int MAST_RELEASE_YEAR { get; set; }

        [Required(ErrorMessage="  Please Enter Release Amount.")]
        [Display(Name="Release Amount (in Cr.)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid Release Amount.")]
        [Range(0.1, 99999999999999.9999, ErrorMessage = "Invalid Release Amount.")]
        //[CompareAmount("TotalAvailable", "MAST_RELEASE_TYPE", ErrorMessage = "Release Amount should not be greater than Available Amount.")]
        public decimal MAST_RELEASE_AMOUNT { get; set; }

        [Required(ErrorMessage="Release Date is required.")]
        [Display(Name="Release Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Please Enter valid date.")]
        [CompareReleaseDate(ErrorMessage = "Release Date must be less than or equal to current date.")]
        [CompareReleaseDateWithReleaseYear("MAST_RELEASE_YEAR", ErrorMessage = "Release Date must be within the selected Release Year.")]
        public string MAST_RELEASE_DATE { get; set; }

        [Required(ErrorMessage="Sanction Order No. is required.")]
        [Display(Name="Sanction Order No.")]
        [RegularExpression(@"^([a-zA-Z0-9-.#_/ ]+){1,150}$", ErrorMessage = "Please Enter Correct Sanction Order No.")]
        [StringLength(255,ErrorMessage="Sanction Order No. must be less than 255 characters.")]
        public string MAST_RELEASE_ORDER { get; set; }

        [Display(Name="Upload Sanction Letter")]
        [StringLength(255, ErrorMessage = "File name must be less than 255 characters.")]
        public string MAST_RELEASE_FILE { get; set; }

        public decimal? TotalAvailable { get; set; }
        public decimal? TotalRelease { get; set; }
        public decimal? TotalAllocation { get; set; }

       

        public int transactionCountRelease { get; set; }
        public string url { get; set; }
        public string thumbnail_url { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public int size { get; set; }
        //public decimal totalAvailable { get; set; }
        public int NumberofFiles { get; set; }

        public string UrlParameter { get; set; }

        //Added By Abhishek kamble 26-Apr-2014
        public string ErrorMessage { get; set; }    

    }


    public class CompareValidationAttribute : ValidationAttribute,IClientValidatable
    {

        // private const string _defaultErrorMessage = "Road Renewal Year must be greater than Road Costruction Year";  
        private string _basePropertyName;

        public CompareValidationAttribute(string basePropertyName) //: base(_defaultErrorMessage)
        {
            _basePropertyName = basePropertyName;
        }


        //Override IsValid  
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //Get PropertyInfo Object  
            var basePropertyInfo = validationContext.ObjectType.GetProperty(_basePropertyName);

            var phaseYear = basePropertyInfo.GetValue(validationContext.ObjectInstance, null);
            var releaseYear = value;

            if (phaseYear != null && releaseYear != null)
            {
                int PhaseYear = Convert.ToInt32(phaseYear);
                var ReleaseYear = Convert.ToInt32(releaseYear);

                //Actual comparision  
                if (PhaseYear > ReleaseYear)
                {
                    var message = FormatErrorMessage(validationContext.DisplayName);
                    return new ValidationResult(message);
                }
            }

            //Default return - This means there were no validation error  
            return null;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "compareyear"
            };
            yield return rule;
        }

    }

    public class CompareAmountValidation : ValidationAttribute
    {
         // private const string _defaultErrorMessage = "Road Renewal Year must be greater than Road Costruction Year";  
        private string _basePropertyName;

        public CompareAmountValidation(string basePropertyName) //: base(_defaultErrorMessage)
        {
            _basePropertyName = basePropertyName;
        }


        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //Get PropertyInfo Object  
            var basePropertyInfo = validationContext.ObjectType.GetProperty(_basePropertyName);

            var releaseAmount = basePropertyInfo.GetValue(validationContext.ObjectInstance, null);
            var allocationAmount = value;

            if (releaseAmount != null && allocationAmount != null)
            {
                double ReleaseAmount = Convert.ToDouble(releaseAmount);
                var AllocationAmount = Convert.ToInt32(allocationAmount);

                //Actual comparision  
                if (ReleaseAmount > AllocationAmount)
                {
                    var message = FormatErrorMessage(validationContext.DisplayName);
                    return new ValidationResult(message);
                }
            }

            //Default return - This means there were no validation error  
            return null;
        }
    }

    public class CompareReleaseDate : ValidationAttribute, IClientValidatable
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }
            
            var releaseDate = new CommonFunctions().GetStringToDateTime(value.ToString());

            if (releaseDate < DateTime.Now)
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
                ValidationType = "comparereleasedate"
            };
            yield return rule;
        }
    }

    public class CompareReleaseDateWithReleaseYear : ValidationAttribute, IClientValidatable
    {

        private string _basePropertyReleaseYear;

        public CompareReleaseDateWithReleaseYear(string basePropertyReleaseYear) //: base(_defaultErrorMessage)
        {
            _basePropertyReleaseYear = basePropertyReleaseYear;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            var releaseDate = new CommonFunctions().GetStringToDateTime(value.ToString());

            var basePropertyReleaseYear = validationContext.ObjectType.GetProperty(_basePropertyReleaseYear);

            int releaseYear = Convert.ToInt32(basePropertyReleaseYear.GetValue(validationContext.ObjectInstance, null));

            DateTime startDate = new DateTime(releaseYear, 4, 1);
            DateTime endDate = new DateTime((releaseYear + 1), 3, 31);

            if (releaseDate >= startDate && releaseDate <= endDate )
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
                ValidationType = "comparereleasedatewithreleaseyear"
            };
            rule.ValidationParameters.Add("releaseyear", this._basePropertyReleaseYear);
            return new[] { rule };
        }
    }

    public class CompareAmount : ValidationAttribute, IClientValidatable
    {
        private readonly string PropertyAmount;
        private readonly string PropertyType;

        public CompareAmount(string propertyAmount,string propertyType)
        {
            this.PropertyAmount = propertyAmount;
            this.PropertyType = propertyType;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyTestedAmount = validationContext.ObjectType.GetProperty(this.PropertyAmount);
            var propertyTestedType = validationContext.ObjectType.GetProperty(this.PropertyType);
            if (propertyTestedAmount == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.PropertyAmount));
            }

            var releaseType = propertyTestedType.GetValue(validationContext.ObjectInstance, null);

            if (releaseType.ToString().ToLower() == "s")
            {
                return ValidationResult.Success;
            }

            var available = Convert.ToDecimal(propertyTestedAmount.GetValue(validationContext.ObjectInstance, null));
            
            if (value == null)
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }

            var releaseAmount = Convert.ToDecimal(value);

            if (releaseAmount <= available)
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
                ValidationType = "compareamount",
                
            };
            yield return rule;
        }
    }
}