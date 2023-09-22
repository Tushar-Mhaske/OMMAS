/*----------------------------------------------------------------------------------------
 * Project Id       :

 * Project Name     :OMMAS-II

 * File Name        :MasterTerrainTypeViewModel.cs
 
 * Author           :Abhishek Kamble.

 * Creation Date    :01/May/2013

 * Desc             :This class is used to declare the variables, lists that are used in the Details form.
 
 * ---------------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace PMGSY.Models.Master
{
    public class MasterTerrainTypeViewModel
    {

        [UIHint("hidden")]
        public string EncryptedTerrainCode { get; set; }

        public int MAST_TERRAIN_TYPE_CODE { get; set; }

        [Display(Name = "Terrain Name")]
        [Required(ErrorMessage = "Terrain Name is required.")]
        [RegularExpression(@"^([a-zA-Z _]+)$", ErrorMessage = "Terrain Name is not in valid format.")]
        [StringLength(50, ErrorMessage = "Terrain Name must be less than 50 characters.")] 
        public string MAST_TERRAIN_TYPE_NAME { get; set; }

        
        [Display(Name = "Slope From")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Slope From is not in valid format.")]
        [Range(0, 2147483647, ErrorMessage = "Slope From is not in valid format.")]
        public Nullable<int> MAST_TERRAIN_SLOP_FROM { get; set; }

        [Display(Name = "Slope To")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Slope To is not in valid format.")]        
        [Range(0, 2147483647, ErrorMessage = "Slope To is not in valid format.")]
        [CompareFieldValidator("MAST_TERRAIN_SLOP_FROM", ErrorMessage = "Slope To must be greater than Slope From.")]
     
        public Nullable<int> MAST_TERRAIN_SLOP_TO { get; set; }

        [Display(Name = "Roadway Width (In Mtr)")]
        [Required(ErrorMessage = "Roadway Width is required.")]
        //[RegularExpression(@"^\d{1,1}\.\d{0,2}$", ErrorMessage = "Roadway Width is not in valid format. ")]
        [RegularExpression(@"^\d{1,1}\.\d{0,2}$", ErrorMessage = "Invalid Roadway Width  Can only contains single Numeric value and 2 digits after decimal place")]

       
        public decimal MAST_TERRAIN_ROADWAY_WIDTH { get; set; }
            
        public virtual ICollection<MASTER_EXISTING_ROADS> MASTER_EXISTING_ROADS { get; set; }
    }

    //class for custom validation
    public class CompareFieldValidator : ValidationAttribute, IClientValidatable
    {
        //public string Value { get; set; }
        public string ComparingValue { get; set; }

        public CompareFieldValidator(string ComparingValue)
        {
            //this.Value = Value;
            this.ComparingValue = ComparingValue;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, ComparingValue);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

           // System.Reflection.PropertyInfo PropertyInfo = validationContext.ObjectInstance.GetType().GetProperty(Value);
            System.Reflection.PropertyInfo comparingPropertyInfo = validationContext.ObjectInstance.GetType().GetProperty(ComparingValue);

            if (comparingPropertyInfo == null)
            {
                return new ValidationResult(string.Format("Property '{0}' is undefined.", ComparingValue));
            }

            object comparingValue = comparingPropertyInfo.GetValue(validationContext.ObjectInstance, null);

            if (value != null && comparingValue != null)
            {
                //if (Convert.ToInt32(value)  <= Convert.ToInt32(comparingValue))
                if (Convert.ToDouble(value) <= Convert.ToDouble(comparingValue))
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
                ValidationType = "comparefieldvalidator"
            };

        }
    }//end custom validation

}