#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   TrafficViewModel.cs
        * Description   :   This View Model is Used in Adding, Editing View of Traffic Intensity Details related to Road Proposal- TrafficIntensity.cshtml
        * Author        :   Shivkumar Deshmukh        
        * Creation Date :   27/April/2013
 **/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PMGSY.Models;
using System.Web.Mvc;


namespace PMGSY.Models.Proposal
{
    public class TrafficViewModel
    {
        public int IMS_PR_ROAD_CODE { get; set; }
        
        // U- Update , A-Add
        public string Operation { get; set; }

        public int MinValue { get; set; }
        public int MaxValue { get; set; }

        [Display(Name = "Year")]
        public int IMS_YEAR { get; set; }
        [Display(Name = "Batch")]
        public int IMS_BATCH { get; set; }
        [Display(Name = "Package No.")]
        public string IMS_PACKAGE_ID { get; set; }
        [Display(Name = "Road Name")]
        public string IMS_ROAD_NAME { get; set; }
        [Display(Name = "Pavement Length")]
        public decimal IMS_PAV_LENGTH { get; set; }
           
        [Display(Name="Year")]
        [Required]
        [Range(2000, 2099, ErrorMessage = "Please Select Year.")]
        public int IMS_TI_YEAR { get; set; }

        [Display(Name="Total Motarised Traffic/day")]
        [Required]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Invalid Total Motarised Traffic/day,Can only contains Numeric values")]
        [Range(1, 99999999, ErrorMessage = "Total Motarised Traffic/day must be a positive number.")]
        public int? IMS_TOTAL_TI { get; set; }

        [Display(Name="ESAL")]
        [Required]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Invalid ESAL,Can only contains Numeric values")]        
        [DynamicRangeValidator("MinValue", "MaxValue", ErrorMessage = "ESAL must be between {0} and {1}")]
        public int? IMS_COMM_TI { get; set; }

        [Display(Name="Distrct")]
        public string DistrictName { get; set; }

        [Display(Name="Block")]
        public string BlockName { get; set; }

        [Display(Name="Road Code")]
        public int RoadCode { get; set; }

        [Display(Name="Road Name")]
        public string RoadName { get; set; }
        
        [Display(Name="Traffic Catagory")]
        public string CurveType { get; set; }

        public virtual IMS_SANCTIONED_PROJECTS IMS_SANCTIONED_PROJECTS { get; set; }
    }

    public class DynamicRangeValidator : ValidationAttribute, IClientValidatable
    {
        private readonly string _minPropertyName;
        private readonly string _maxPropertyName;
        public DynamicRangeValidator(string minPropertyName, string maxPropertyName)
        {
            _minPropertyName = minPropertyName;
            _maxPropertyName = maxPropertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var minProperty = validationContext.ObjectType.GetProperty(_minPropertyName);
            var maxProperty = validationContext.ObjectType.GetProperty(_maxPropertyName);
            if (minProperty == null)
            {
                return new ValidationResult(string.Format("Unknown property {0}", _minPropertyName));
            }
            if (maxProperty == null)
            {
                return new ValidationResult(string.Format("Unknown property {0}", _maxPropertyName));
            }

            int minValue = (int)minProperty.GetValue(validationContext.ObjectInstance, null);
            int maxValue = (int)maxProperty.GetValue(validationContext.ObjectInstance, null);
            int currentValue = (int)value;
            if (currentValue < minValue || currentValue > maxValue)
            {
                return new ValidationResult(
                    string.Format(
                        ErrorMessage,
                        minValue,
                        maxValue
                    )
                );
            }

            return null;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ValidationType = "dynamicrange",
                ErrorMessage = this.ErrorMessage,
            };
            rule.ValidationParameters["minvalueproperty"] = _minPropertyName;
            rule.ValidationParameters["maxvalueproperty"] = _maxPropertyName;
            yield return rule;
        }
    }

}