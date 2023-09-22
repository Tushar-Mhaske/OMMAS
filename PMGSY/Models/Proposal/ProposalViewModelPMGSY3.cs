using PMGSY.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Proposal
{
    public class ProposalViewModelPMGSY3 : ProposalViewModel
    {
        public int imsComponentId { get; set; }
        [IsValidRangeOfRidingQualityLength("IMS_PAV_LENGTH", ErrorMessage = "Riding Quality Length shoud be less than or equal to Pavement Length.")]
        public decimal ImsRidingQualityLength { get; set; }

        [Required(ErrorMessage = "Please enter Clearing Cost")]
        public decimal ImsClearing { get; set; }
        [Required(ErrorMessage = "Please enter Filling Cost")]
        public decimal ImsFilling { get; set; }
        [Required(ErrorMessage = "Please enter Excavation Cost")]
        public decimal ImsExcavation { get; set; }

        [Required(ErrorMessage = "Please enter SubGrade Cost")]
        public decimal ImsSubGrade { get; set; }
        [Required(ErrorMessage = "Please enter Shoulder Cost")]
        public decimal ImsShoulder { get; set; }
        [Required(ErrorMessage = "Please enter Granular Sub Base Cost")]
        public decimal ImsGranularSubBase { get; set; }
        [Required(ErrorMessage = "Please enter Soil Aggregate Cost")]
        public decimal ImsSoilAggregate { get; set; }
        [Required(ErrorMessage = "Please enter WBM Grade II Cost")]
        public decimal ImsWBMGradeII { get; set; }
        [Required(ErrorMessage = "Please enter WBM Grade III Cost")]
        public decimal ImsWBMGradeIII { get; set; }
        [Required(ErrorMessage = "Please enter WMM Cost")]
        public decimal ImsWMM { get; set; }
        [Required(ErrorMessage = "Please enter Prime Coat Cost")]
        public decimal ImsPrimeCoat { get; set; }
        [Required(ErrorMessage = "Please enter Tack Coat Cost")]
        public decimal ImsTackCoat { get; set; }
        [Required(ErrorMessage = "Please enter BDMBM Cost")]
        public decimal ImsBMDBM { get; set; }
        [Required(ErrorMessage = "Please enter OGPC SDBC BC Cost")]
        public decimal ImsOGPC_SDBC_BC { get; set; }
        [Required(ErrorMessage = "Please enter Seal Coat Cost")]
        public decimal ImsSealCoat { get; set; }
        [Required(ErrorMessage = "Please enter Surface Dressing Cost")]
        public decimal ImsSurfaceDressing { get; set; }
        [Required(ErrorMessage = "Please enter Dry Lean Concrete Cost")]
        public decimal ImsDryLeanConcrete { get; set; }
        [Required(ErrorMessage = "Please enter Concrete Payment Cost")]
        public decimal ImsConcretePavement { get; set; }
        [Required(ErrorMessage = "Please enter PUCCA Side Drains Cost")]
        public decimal ImsPuccaSideDrains { get; set; }
        [Required(ErrorMessage = "Please enter GST Cost")]
        public decimal ImsGSTCost { get; set; }

        public string encrProposalCode { get; set; }


        public Nullable<decimal> EXISTING_CARRIAGEWAY_WIDTH { get; set; }
        public Nullable<decimal> EXISTING_CARRIAGEWAY_PUC { get; set; }

    }

    public class IsValidRangeOfRidingQualityLength : ValidationAttribute, IClientValidatable
    {
        private readonly string PropertyName;

        public IsValidRangeOfRidingQualityLength(string propertyName)
        {
            this.PropertyName = propertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyTestedInfo = validationContext.ObjectType.GetProperty(this.PropertyName);
            if (propertyTestedInfo == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.PropertyName));
            }

            var imsPAVLength = propertyTestedInfo.GetValue(validationContext.ObjectInstance, null);

            if (Convert.ToDecimal(imsPAVLength) >= Convert.ToDecimal(value))
            {
                if (value == null)
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
                else
                {
                    return ValidationResult.Success;
                }
            }

            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "isvalidrangeofridingqualitylength"
            };
            rule.ValidationParameters["imsrqlength"] = this.PropertyName;
            yield return rule;
        }

    }
}
