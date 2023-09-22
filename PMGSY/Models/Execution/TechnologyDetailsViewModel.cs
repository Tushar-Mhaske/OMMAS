using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Execution
{
    public class TechnologyDetailsViewModel
    {
        public int hdnLayerCode { get; set; }
        public string EncryptedLayerCode { get; set; }

        public int techMonthlyCode { get; set; }

        public bool flg { set; get; }
        public int IMS_PR_ROAD_CODE { get; set; }
        public string EncryptedRoadCode { get; set; }

        public int TechnologyCode { get; set; }
        public string EncryptedTechCode { get; set; }
        public string EncryptedStChainage { get; set; }
        public string EncryptedEndChainage { get; set; }

        public string Operation { get; set; }

        public decimal startChainage { get; set; }
        public decimal endChainage { get; set; }

        [Required(ErrorMessage = "Please select year.")]
        [Range(2000, 2099, ErrorMessage = "Please select valid year.")]
        [Display(Name = "Year")]
        public int Year { get; set; }
        public List<SelectListItem> YearList { get; set; }

        [Required(ErrorMessage = "Please select month.")]
        [Range(1, 12, ErrorMessage = "Please select valid month.")]
        [Display(Name = "Month")]
        public int Month { get; set; }
        public List<SelectListItem> MonthList { get; set; }

        [Required(ErrorMessage = "Please select status.")]
        [RegularExpression("[CP]", ErrorMessage = "Please Select valid status")]
        [Display(Name = "Status")]
        public string Status { get; set; }
        public List<SelectListItem> StatusList { get; set; }

        [Required(ErrorMessage = "Please select date.")]
        //[RegularExpression("[^(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.]((19|20)\\d\\d)$]", ErrorMessage = "Please Select valid date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Date is not in valid format")]
        [Display(Name = "Date")]
        public string Date { get; set; }

        //[ComparePreviousLength("previousCompletedLength", "EXEC_ISCOMPLETED", ErrorMessage = "Preparatory work length must be greater than or equal to previous preparatory work length.")]
        //[CompareLengthValidation("startChainage", "endChainage", ErrorMessage = "completed length between start and chainage")]
        [Required(ErrorMessage = "Please enter completed length.")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "Invalid Completed Length,Can only contains Numeric values and Total 7 Digits and 3 digits after decimal place")]
        [Display(Name = "Completed Length")]
        public decimal completedLength { get; set; }

        public int previousMonth { get; set; }
        public int previousYear { get; set; }
        public decimal previousCompletedLength { get; set; }
        public string previousDate { get; set; }
        public string previousStatus { get; set; }

        public decimal sanctionLength { get; set; }
        public string currentStatus { get; set; }

        public string agreementDate { get; set; }

        public string technologyName { get; set; }
        public string layerName { get; set; }

        //Added by Pradip Patil [08/05/2017]

        [Display(Name="Quantity Of Material")]
        [RegularExpression(@"^\d*(\.\d{1,3})?", ErrorMessage = "Total Maintenance is not in valid format. ")]
        [Range(0, 999999.999, ErrorMessage = "Quantity Of Material is exceeding the limit. can be upto 6 digit.")]
        public Decimal TechQuantity { get; set; }

        [Range(1,3,ErrorMessage="Please select unit")]
        public int TechUnit { get; set; }
         public List<SelectListItem> UnitList { get; set; }

        [Display(Name="Technology/Material Supplier")]
        [StringLength(50,ErrorMessage="Technology/Material Supplier can be upto 50 characters.")]
        [RegularExpression(@"^([a-zA-z./ ]+)$",ErrorMessage="Invalid character are entered.")]
        public String TechSupplier { get; set; }

        [Display(Name="Rate/Unit")]
        [RegularExpression(@"^\d*(\.\d{1,3})?", ErrorMessage = "Rate/Unit is not in valid format. ")]
        [Range(0, 9999.999, ErrorMessage = "Rate/Unit is exceeding the limit.can be upto 4 digit.")]
        public decimal RatePerunit { get; set; }

        ///Changes by SAMMED A. PATIL on 20JULY2017 
        public Decimal totalTechQuantity { get; set; }
    }

    #region Custom Validation for two fields
    public class CompareLengthValidation : ValidationAttribute, IClientValidatable
    {
        private readonly string startChainage;
        private readonly string endChainage;
        //private readonly string WorkStatus;

        public CompareLengthValidation(string startChain, string endChain)
        {
            this.startChainage = startChain;
            this.endChainage = endChain;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var startChain = validationContext.ObjectType.GetProperty(this.startChainage);
            var endChain = validationContext.ObjectType.GetProperty(this.endChainage);

            var startChainValue = startChain.GetValue(validationContext.ObjectInstance, null);
            var endChainValue = endChain.GetValue(validationContext.ObjectInstance, null);

            var comparisonValue = value;

            if (startChainValue == null && endChainValue == null)
            {
                return new ValidationResult(FormatErrorMessage("Please enter start and end chainage"));
            }
            else if (Convert.ToDecimal(comparisonValue) < Convert.ToDecimal(startChainValue))
            {
                return new ValidationResult(FormatErrorMessage("Completed length should be less than or equal to end chainage."));
            }
            else if (Convert.ToDecimal(comparisonValue) > Convert.ToDecimal(endChainValue))
            {
                return new ValidationResult(FormatErrorMessage("Completed length should be less than or equal to end chainage."));
            }
            else
            {
                return ValidationResult.Success;
            }
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            yield return new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessage,
                ValidationType = "customlengthvalidator"
            };
        }
    }
    #endregion
}