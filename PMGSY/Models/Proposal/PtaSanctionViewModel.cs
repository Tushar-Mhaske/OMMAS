#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   PtaSanctionViewModel.cs
        * Description   :   This View Model is Used in Scrutinizing Proposals By PTA - StaSactionProposal.cshtml
        * Author        :   Shyam Yadav     
        * Creation Date :   21/Nov/2013
 **/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PMGSY.Models;
using System.Web.Mvc;
using PMGSY.Common;

namespace PMGSY.Models.Proposal
{
    public class PtaSanctionViewModel
    {
        public int IMS_PR_ROAD_CODE { get; set; }
        public string PTA_SANCTIONED { get; set; }
        public int? PTA_SANCTIONED_BY { get; set; }
        public string IMS_ISCOMPLETED { get; set; }
        public string IMS_SANCTIONED { get; set; }

        [Display(Name = "PTA Name")]
        [Required]
        [RegularExpression(@"^[a-zA-Z .]+$", ErrorMessage = "Invalid PTA Name,Can only contains Alphabet values and [.]")]
        public string NAME_OF_PTA { get; set; }

        [Display(Name = "Scrutiny Date")]
        [Required(ErrorMessage = "Please Enter Scrutiny Date")]
        [CompareSanctionDates("STA_SANCTIONED_DATE",ErrorMessage="PTA Scrutiny Date must be greater than or equal to STA Scrutiny Date.")]
        public string PTA_SANCTIONED_DATE { get; set; }

        [Display(Name = "Remarks")]
        [Required(ErrorMessage = "Please Enter Remarks")]
        [RegularExpression(@"^[a-zA-Z0-9 ,.()-]+$", ErrorMessage = "Invalid Remarks,Can only contains AlphaNumeric values and [,.()-]")]
        public string MS_PTA_REMARKS { get; set; }

        [Display(Name = "Un-Scrutiny Date")]
        [Required(ErrorMessage = "Please Enter Un-Scrutiny Date")]
        public string PTA_UNSCRUTINY_DATE { get; set; }

        [Display(Name = "Un-Scrutiny Remarks")]
        [Required(ErrorMessage = "Please Enter Un-Scrutiny Remarks")]
        [RegularExpression(@"^[a-zA-Z0-9 ,.()-]+$", ErrorMessage = "Invalid Remarks,Can only contains AlphaNumeric values and [,.()-]")]
        public string MS_PTA_UnScrutinised_REMARKS { get; set; }

        //Change done by Vikram for validating PTA Sanction date to be greater than STA_SANCTION_DATE
        public String STA_SANCTIONED_DATE { get; set; }

    }



    public class CompareSanctionDates : ValidationAttribute, IClientValidatable
    {
        CommonFunctions objCommon = new CommonFunctions();

        private readonly string STAScrutinyDate;

        public CompareSanctionDates(string staScrutinyDate)
        {
            this.STAScrutinyDate = staScrutinyDate;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertySTADate = validationContext.ObjectType.GetProperty(this.STAScrutinyDate);

            if (propertySTADate == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.STAScrutinyDate));
            }

            var staSanctionDate = propertySTADate.GetValue(validationContext.ObjectInstance, null);
            DateTime _staSanctionDate;
            DateTime _ptaSanctionDate;
            if (staSanctionDate == null)
            {
                return ValidationResult.Success;
            }
            else
            {
                _staSanctionDate = objCommon.GetStringToDateTime(staSanctionDate.ToString());
            }

            var ptaSanctionDate = value;

            _ptaSanctionDate = Convert.ToDateTime(ptaSanctionDate.ToString());

            if (_ptaSanctionDate >= _staSanctionDate)
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
                ValidationType = "comparesanctiondates"
            };
            yield return rule;
        }


    }
}