#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   PciIndexViewModel.cs      
        * Description   :   This View Model is Used in PCI Index Views AddPciForCNRoad.cshtml , AddPciForPmgsyRoad.cshtml,ListPciDetails.cshtml
        * Author        :   Shivkumar Deshmukh        
        * Creation Date :   19/June/2013
 **/
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PMGSY.Models;
using System.Web.Mvc;

namespace PMGSY.Models.Maintenance
{
    public class PciIndexViewModel
    {
        public PciIndexViewModel()
        {
            SURFACES = new List<SelectListItem>();
            YEARS = new List<SelectListItem>();
        }
        public int IMS_PR_ROAD_CODE { get; set; }
        public int PLAN_CN_ROAD_CODE { get; set; }

        [Display(Name="Road Name")]
        public string RoadName { get; set; }
           
        public decimal RoadLength { get; set; }
        // Encrypted IMS_PR_ROAD_CODE 
        public string ENC_IMS_PR_ROAD_CODE { get; set; }

        public string ENC_PLAN_CN_ROAD_CODE { get; set; }

        public int MANE_SEGMENT_NO { get; set; }
        
        [Display(Name="Year")]
        [Range(1, int.MaxValue,ErrorMessage="Please Select Year")]
        public int MANE_PCI_YEAR { get; set; }
        
        [Display(Name="Year")]
        public List<SelectListItem>  YEARS { get; set; }        

        [Display(Name="From Km")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "Invalid From Km ,Can only contains Numeric values and 3 digits after decimal place")]        
        public decimal MANE_STR_CHAIN { get; set; }

        [Display(Name = "To Km")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "Invalid To Km ,Can only contains Numeric values and 3 digits after decimal place")]
        [IsLessThan("MANE_STR_CHAIN", "RoadLength", ErrorMessage = "To Km must be greator than From Km and To Km must be less than or equal to Road Length")]
        public decimal MANE_END_CHAIN { get; set; }

        [Display(Name="PCI Value")]
        [RegularExpression(@"^\s*(?=.*[1-5])\d$", ErrorMessage = "Invalid PCI Value ,Can only contains Numeric values in Range (1,5)")]
        [Range(1,5,ErrorMessage="PCI Index must be in Range of 1 to 5.")]
        public int MANE_PCIINDEX { get; set; }
        
        [Display(Name="Surface Type")]
        [Range(1,int.MaxValue,ErrorMessage="Please Select Surface Type")]
        public Nullable<int> MANE_SURFACE_TYPE { get; set; }
        public List<SelectListItem> SURFACES { get; set; }

        [Display(Name="Date of PCI")]
        [Required]
        public string MANE_PCI_DATE { get; set; }

        public virtual IMS_SANCTIONED_PROJECTS IMS_SANCTIONED_PROJECTS { get; set; }
        public virtual MASTER_SURFACE MASTER_SURFACE { get; set; }
    }

    /// <summary>
    /// Check if it is Stage One Proposal For Mandatory Validations
    /// </summary>
    public class IsLessThan : ValidationAttribute, IClientValidatable
    {
        private readonly string PropertyName;
        private readonly string PropertyRoadLength;

        public IsLessThan(string propertyName,string roadLength)
        {
            this.PropertyName = propertyName;
            this.PropertyRoadLength = roadLength;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                var propertyTestedInfo = validationContext.ObjectType.GetProperty(this.PropertyName);
                var propertyRoadLength = validationContext.ObjectType.GetProperty(this.PropertyRoadLength);

                if (propertyTestedInfo == null)
                {
                    return new ValidationResult(string.Format("unknown property {0}", this.PropertyName));
                }
                
                // FROM Km
                var OtherValue = propertyTestedInfo.GetValue(validationContext.ObjectInstance, null);
                var RoadLength = propertyRoadLength.GetValue(validationContext.ObjectInstance, null);            
    
                //commented by Vikram as one condition for removing the check of Road Length with the End Chainage is removed
                //if (Convert.ToDecimal(OtherValue) < Convert.ToDecimal(value) && Convert.ToDecimal(value) <= Convert.ToDecimal(RoadLength))
                if (Convert.ToDecimal(OtherValue) < Convert.ToDecimal(value)) 
                {
                    return ValidationResult.Success;
                }

                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
            catch
            {
                return ValidationResult.Success;
            }
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "islessthan"
            };
            rule.ValidationParameters["othervalue"] = this.PropertyName;
            yield return rule;
        }

    }

}