using PMGSY.BAL.Fund;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Fund
{
    public class FundAllocationViewModel
    {
        [UIHint("Hidden")]
        public string EncryptedFundCode { get; set; }

        public int MAST_TRANSACTION_NO { get; set; }

        [Required(ErrorMessage="  Please Select State.")]
        [Display(Name="State")]
        public int MAST_STATE_CODE { get; set; }
        
        [Required(ErrorMessage="  Please Select SRRDA.")]
        [Display(Name="Executing Agency")]
        public int ADMIN_NO_CODE { get; set; }

        [Required(ErrorMessage = "  Please Select Fund Type.")]
        [Display(Name="Fund Type")]
        [RegularExpression("[PAM]", ErrorMessage = "  Please Select Fund Type.")]
        public string MAST_FUND_TYPE { get; set; }

        [Required(ErrorMessage="  Please Select Collaboration.")]
        [Display(Name="Collaboration")]
        public int MAST_FUNDING_AGENCY_CODE { get; set; }

        [Required(ErrorMessage="  Please Select Financial Year.")]
        [Range(2000, 2099, ErrorMessage = "  Please Select Financial Year.")]
        [Display(Name="Phase")]
        public int MAST_YEAR { get; set; }

        [Required(ErrorMessage="  Please Enter Allocation Amount.")]
        [Display(Name="Allocation Amount (In Cr.)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid Allocation Amount.")]
        [Range(0.0001,99999999999999.9999,ErrorMessage="Please enter valid Allocation Amount.")]
        public decimal MAST_ALLOCATION_AMOUNT { get; set; }

        [Required(ErrorMessage="Allocation Date is required.")]
        [Display(Name="Allocation Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Please Enter valid date.")]
        [CompareReleaseDate(ErrorMessage="Allocation Date must be less than or equal to Current Date.")]
        public string MAST_ALLOCATION_DATE { get; set; }

        [Required(ErrorMessage="Sanction Order No. is required.")]
        [Display(Name = "Sanction Order No.")]
        [StringLength(255,ErrorMessage="Sanction Order No. must not be greater than 255 characters.")]
        [RegularExpression(@"^([a-zA-Z0-9-.#_/ ]+){1,150}$", ErrorMessage = "Please Enter Correct Sanction Order No.")]
        public string MAST_ALLOCATION_ORDER { get; set; }

        [Display(Name="Sanction Letter")]
        public string MAST_ALLOCATION_FILE { get; set; }

        public string releaseType { get;set;}

        public int transactionCount { get; set; }
        public string url { get; set; }
        public string thumbnail_url { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public int size { get; set; }
        public int NumberofFiles { get; set; }

        public decimal? TotalRelease { get; set; }

        //Added By Abhishek kamble 26-Apr-2014
        public string ErrorMessage { get; set; }    
    }


    //public class CompareReleaseAmount : ValidationAttribute, IClientValidatable
    //{
    //    private readonly string PropertyReleaseAmount;
    //    //private readonly string PropertyType;

    //    public CompareReleaseAmount(string propertyAmount)//, string propertyType)
    //    {
    //        this.PropertyReleaseAmount = propertyAmount;
    //        //this.PropertyType = propertyType;
    //    }

    //    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    //    {
    //        var propertyTestedAmount = validationContext.ObjectType.GetProperty(this.PropertyReleaseAmount);
    //        //var propertyTestedType = validationContext.ObjectType.GetProperty(this.PropertyType);
    //        if (propertyTestedAmount == null)
    //        {
    //            return new ValidationResult(string.Format("unknown property {0}", this.PropertyReleaseAmount));
    //        }

    //        //var releaseType = propertyTestedType.GetValue(validationContext.ObjectInstance, null);

    //        var totalRelease = Convert.ToDecimal(propertyTestedAmount.GetValue(validationContext.ObjectInstance, null));

    //        if (value == null)
    //        {
    //            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
    //        }

    //        var releaseAmount = Convert.ToDecimal(value);
    //        FundBAL objBAL = new FundBAL();

    //        if (releaseAmount <= totalRelease)
    //        {
    //            return ValidationResult.Success;
    //        }
    //        else
    //        {
    //            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
    //        }
    //    }

    //    public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
    //    {
    //        var rule = new ModelClientValidationRule
    //        {
    //            ErrorMessage = this.ErrorMessageString,
    //            ValidationType = "comparereleaseamount",

    //        };
    //        yield return rule;
    //    }
    //}
}