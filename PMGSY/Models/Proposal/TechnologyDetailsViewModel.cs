using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Proposal
{
    public class TechnologyDetailsViewModel
    {
        public TechnologyDetailsViewModel()
        {
            ListTechnology = new List<MASTER_TECHNOLOGY>();
            ListLayers = new List<SelectListItem>();
        }

        public string convergence { get; set; }

        public string EncryptedProposalSegmentCode { get; set; }

        public int IMS_PR_ROAD_CODE { get; set; }
        public int IMS_SEGMENT_NO { get; set; }

        [Required(ErrorMessage = "Start Chainage is required.")]
        [Display(Name = "Start Chainage")]
        [Range(0.000, 9999.999, ErrorMessage = "Please enter valid Start Chainage")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "Invalid Start Chainage,only 3 digits after decimal place is allowed.")]
        [CompareChainage("IMS_END_CHAINAGE", ErrorMessage = "Start chainage must be less than end chainage")]
        public decimal IMS_START_CHAINAGE { get; set; }

        [Required(ErrorMessage = "End Chainage is required.")]
        [Display(Name = "End Chainage")]
        [Range(0.001, 9999.999, ErrorMessage = "Please enter valid End Chainage")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "Invalid End Chainage,only 3 digits after decimal place is allowed.")]
        public decimal IMS_END_CHAINAGE { get; set; }

        [Required(ErrorMessage = "Total Cost of Technology  is required.")]
        [Display(Name = "Total Cost of Technology")]
        [Range(0.0000, 999999.9999, ErrorMessage = "Please enter valid Total Cost of Technology")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid Total Cost of Technology,only 4 digits after decimal place is allowed.")]
        public decimal IMS_TECH_COST { get; set; }

        [Required(ErrorMessage = "Please select Layer")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select Layer")]
        [Display(Name = "Layer")]
        public int MAST_LAYER_CODE { get; set; }

        [Required(ErrorMessage = "Cost of Technology for Layer is required.")]
        [Display(Name = "Cost of Technology for Layer")]
        [Range(0.0000, 999999.9999, ErrorMessage = "Please enter valid Cost of Technology for Layer")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid Cost of Technology for Layer,only 4 digits after decimal place is allowed.")]
        public decimal IMS_LAYER_COST { get; set; }

        [Required(ErrorMessage = "Please Select Technology")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select Technology")]
        [Display(Name = "Technology")]
        public int MAST_TECH_CODE { get; set; }

        public virtual IMS_SANCTIONED_PROJECTS IMS_SANCTIONED_PROJECTS { get; set; }
        public virtual MASTER_EXECUTION_ITEM MASTER_EXECUTION_ITEM { get; set; }
        public virtual MASTER_TECHNOLOGY MASTER_TECHNOLOGY { get; set; }

        public string Operation { get; set; }
        public string EncryptedProposalCode { get; set; }

        public List<MASTER_TECHNOLOGY> ListTechnology { get; set; }
        public List<SelectListItem> ListLayers { get; set; }

        // Added on 01-06-2023 for FDR changes
        [Display(Name = "Additive Used")]
        public string Is_Additive_Used { get; set; }

        [Display(Name = "Crack Relief Layer Provided")]
        public string Is_CrackReliefLayerProvided { get; set; }

    }

    public class CompareChainage : ValidationAttribute, IClientValidatable
    {
        private readonly string PropertyEndChainage;

        public CompareChainage(string propertyEndChainage)
        {
            this.PropertyEndChainage = propertyEndChainage;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyTestedEndChainage = validationContext.ObjectType.GetProperty(this.PropertyEndChainage);

            if (propertyTestedEndChainage == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.PropertyEndChainage));
            }

            var endChainage = Convert.ToDecimal(propertyTestedEndChainage.GetValue(validationContext.ObjectInstance, null));
            if (value == null)
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
            var startChainage = Convert.ToDecimal(value);

            if (startChainage < endChainage)
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
                ValidationType = "comparechainage"
            };
            yield return rule;
        }
    }

}