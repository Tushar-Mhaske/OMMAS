using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace PMGSY.Models.Master
{
    public class ConnectivityStatusViewModel
    {

        public int hdnStateCode { get; set; }

        public int hdnDistCode { get; set; }

        public int hdnOpr { get; set; }

        [RegularExpression(@"^([YN]+)$", ErrorMessage = "Flag should be Y or N")]
        public string flag { get; set; }

        public string hdnflag { get; set; }

        [Display(Name = "State")]
        [Required(ErrorMessage = "Please select State.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select State.")]
        public int StateCode { get; set; }
        public List<SelectListItem> StateList { get; set; }

        [Display(Name = "District")]
        [Required(ErrorMessage = "Please select District.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select District.")]
        public int DistrictCode { get; set; }
        public List<SelectListItem> DistrictList { get; set; }

        public string StateName { get; set; }
        public string DistrictName { get; set; }

        [Display(Name = "Particulars")]
        public string Particulars { get; set; }
        [Display(Name = "Total")]
        public string Total { get; set; }
        [Display(Name = "Total number of Habitations")]
        public string TotalHabs { get; set; }
        [Display(Name = "Total number of Connected Habitations (as on 01-04-2000)")]
        public string Connected { get; set; }
        [Display(Name = "Total number of UnConnected Habitations")]
        public string UnConnected { get; set; }
        [Display(Name = "Habitations covered under PMGSY 2000-2001")]
        public string Connected2000 { get; set; }
        [Display(Name = "Habitations covered under PMGSY 2001-2002")]
        public string Connected2001 { get; set; }

        [Display(Name = "1000+")]
        public string lb1000 { get; set; }
        [Display(Name = "500-999")]
        public string lb500 { get; set; }
        [Display(Name = "250-499")]
        public string lb250 { get; set; }
        [Display(Name = "<250")]
        public string lbless250 { get; set; }

        public string c11 { get; set; }
        public string c12 { get; set; }
        public string c13 { get; set; }
        public string c14 { get; set; }
        public string c1Tot { get; set; }

        public string c21 { get; set; }
        public string c22 { get; set; }
        public string c23 { get; set; }
        public string c24 { get; set; }
        public string c2Tot { get; set; }

        public string c31 { get; set; }
        public string c32 { get; set; }
        public string c33 { get; set; }
        public string c34 { get; set; }
        public string c3Tot { get; set; }

        public string c41 { get; set; }
        public string c42 { get; set; }
        public string c43 { get; set; }
        public string c44 { get; set; }
        public string c4Tot { get; set; }

        public string c51 { get; set; }
        public string c52 { get; set; }
        public string c53 { get; set; }
        public string c54 { get; set; }
        public string c5Tot { get; set; }

        public string c61 { get; set; }
        public string c62 { get; set; }
        public string c63 { get; set; }
        public string c64 { get; set; }
        public string c6Tot { get; set; }


        [Required(ErrorMessage = "Enter a numeric value")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Enter only numbers")]
        public string t11 { get; set; }
        [Required(ErrorMessage = "Enter a numeric value")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Enter only numbers")]
        public string t12 { get; set; }
        [Required(ErrorMessage = "Enter a numeric value")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Enter only numbers")]
        public string t13 { get; set; }
        [Required(ErrorMessage = "Enter a numeric value")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Enter only numbers")]
        public string t14 { get; set; }
        [Required(ErrorMessage = "Enter a numeric value")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Enter only numbers")]
        public string t1Tot { get; set; }
        
        [Required(ErrorMessage = "Enter a numeric value")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Enter only numbers")]
        public string t1El499 { get; set; }
        [Required(ErrorMessage = "Enter a numeric value")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Enter only numbers")]
        public string t1El249 { get; set; }

        
        [Required(ErrorMessage = "Enter a numeric value")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Enter only numbers")]
        [ConnectivityStatusValidation("t11", ErrorMessage = "Total Habitation should be greater than Connected Habitations.")]
        public string t21 { get; set; }
        
        [Required(ErrorMessage = "Enter a numeric value")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Enter only numbers")]
        [ConnectivityStatusValidation("t12", ErrorMessage = "Total Habitation should be greater than Connected Habitations.")]
        public string t22 { get; set; }

        [Required(ErrorMessage = "Enter a numeric value")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Enter only numbers")]
        [ConnectivityStatusValidation("t13", ErrorMessage = "Total Habitation should be greater than Connected Habitations.")]
        public string t23 { get; set; }

        [Required(ErrorMessage = "Enter a numeric value")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Enter only numbers")]
        [ConnectivityStatusValidation("t14", ErrorMessage = "Total Habitation should be greater than Connected Habitations.")]
        public string t24 { get; set; }

        [Required(ErrorMessage = "Enter a numeric value")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Enter only numbers")]
        public string t2Tot { get; set; }

        [Required(ErrorMessage = "Enter a numeric value")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Enter only numbers")]
        [ConnectivityStatusValidation("t1El499", ErrorMessage = "Total Habitation should be greater than Connected Habitations.")]
        public string t2El499 { get; set; }
        [Required(ErrorMessage = "Enter a numeric value")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Enter only numbers")]
        [ConnectivityStatusValidation("t1El249", ErrorMessage = "Total Habitation should be greater than Connected Habitations.")]
        public string t2El249 { get; set; }

        //[Required(ErrorMessage = "Enter a numeric value")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Enter only numbers")]
        public string t31 { get; set; }
        //[Required(ErrorMessage = "Enter a numeric value")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Enter only numbers")]
        public string t32 { get; set; }
        //[Required(ErrorMessage = "Enter a numeric value")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Enter only numbers")]
        public string t33 { get; set; }
        //[Required(ErrorMessage = "Enter a numeric value")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Enter only numbers")]
        public string t34 { get; set; }
        //[Required(ErrorMessage = "Enter a numeric value")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Enter only numbers")]
        public string t3Tot { get; set; }

        //[Required(ErrorMessage = "Enter a numeric value")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Enter only numbers")]
        public string t3El499 { get; set; }
        //[Required(ErrorMessage = "Enter a numeric value")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Enter only numbers")]
        public string t3El249 { get; set; }

        [Required(ErrorMessage = "Enter a numeric value")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Enter only numbers")]
        public string t41 { get; set; }
        [Required(ErrorMessage = "Enter a numeric value")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Enter only numbers")]
        public string t42 { get; set; }
        [Required(ErrorMessage = "Enter a numeric value")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Enter only numbers")]
        public string t43 { get; set; }
        [Required(ErrorMessage = "Enter a numeric value")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Enter only numbers")]
        public string t44 { get; set; }
        [Required(ErrorMessage = "Enter a numeric value")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Enter only numbers")]
        public string t4Tot { get; set; }

        [Required(ErrorMessage = "Enter a numeric value")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Enter only numbers")]
        public string t4El499 { get; set; }
        [Required(ErrorMessage = "Enter a numeric value")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Enter only numbers")]
        public string t4El249 { get; set; }

        [Required(ErrorMessage = "Enter a numeric value")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Enter only numbers")]
        public string t51 { get; set; }
        [Required(ErrorMessage = "Enter a numeric value")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Enter only numbers")]
        public string t52 { get; set; }
        [Required(ErrorMessage = "Enter a numeric value")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Enter only numbers")]
        public string t53 { get; set; }
        [Required(ErrorMessage = "Enter a numeric value")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Enter only numbers")]
        public string t54 { get; set; }
        [Required(ErrorMessage = "Enter a numeric value")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Enter only numbers")]
        public string t5Tot { get; set; }

        [Required(ErrorMessage = "Enter a numeric value")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Enter only numbers")]
        public string t5El499 { get; set; }
        [Required(ErrorMessage = "Enter a numeric value")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Enter only numbers")]
        public string t5El249 { get; set; }

        [Required(ErrorMessage = "Enter a numeric value")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Enter only numbers")]
        public string t61 { get; set; }
        [Required(ErrorMessage = "Enter a numeric value")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Enter only numbers")]
        public string t62 { get; set; }
        [Required(ErrorMessage = "Enter a numeric value")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Enter only numbers")]
        public string t63 { get; set; }
        [Required(ErrorMessage = "Enter a numeric value")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Enter only numbers")]
        public string t64 { get; set; }
        [Required(ErrorMessage = "Enter a numeric value")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Enter only numbers")]
        public string t6Tot { get; set; }

        [Required(ErrorMessage = "Enter a numeric value")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Enter only numbers")]
        public string t6El499 { get; set; }
        [Required(ErrorMessage = "Enter a numeric value")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Enter only numbers")]
        public string t6El249 { get; set; }
    }

    public class ConnectivityStatusValidation : ValidationAttribute, IClientValidatable
    {
        //private readonly string txtConHabs;
        private readonly string txtTotHabs;
        //private readonly string txtMob;

        public ConnectivityStatusValidation(/*string ConHabs,*/ string TotHabs)
        {
            //this.txtConHabs = ConHabs;
            this.txtTotHabs = TotHabs;
            //this.txtMob = txtMob;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //var tConHabs = validationContext.ObjectType.GetProperty(this.txtConHabs);
            var tTotHabs = validationContext.ObjectType.GetProperty(this.txtTotHabs);

            //var txtConHabsValue = tConHabs.GetValue(validationContext.ObjectInstance, null);
            var txtTotHabsValue = tTotHabs.GetValue(validationContext.ObjectInstance, null);

            var comparisonValue = value;

            if (/*txtConHabs == null && */txtTotHabsValue == null && comparisonValue == null)
            {
                return new ValidationResult("Connected Habitations cannot be more than total habitations");
            }
            else if (txtTotHabsValue != null && comparisonValue != null)
            {
                if (Convert.ToInt32(comparisonValue) > Convert.ToInt32(txtTotHabsValue))
                    return new ValidationResult("Connected Habitations cannot be more than total habitations");
            }
            //else if (txtTelCodeValue == null && txtTelNoValue != null)
            //{
            //    return new ValidationResult("Please enter Telephone Number");
            //}

            else
            {
                return ValidationResult.Success;
            }
            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            yield return new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessage,
                ValidationType = "connectivitystatusvalidator"
            };
        }
    }
}