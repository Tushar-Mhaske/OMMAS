#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   ExecutingOfficerViewModel.cs
        * Description   :   This View Model is Used in Execution Views CDWorks.cshtml
        * Author        :   Vikram Nandanwar        
        * Creation Date :   25/June/2013
 **/
#endregion

using PMGSY.BAL.Execution;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Execution
{
    public class ExecutionCDWorksViewModel
    {
        public ExecutionCDWorksViewModel()
        {
            CD_WORKS = new List<SelectListItem>();
        }

        public string EncryptedCdWorksCode { get; set; }

        public int IMS_PR_ROAD_CODE { get; set; }

        public int EXEC_RCD_CODE { get; set; }

        [Required(ErrorMessage="Please Select Type")]
        [Range(1,20,ErrorMessage="Please Select Type.")]
        [Display(Name="Type")]
        public int MAST_CDWORKS_CODE { get; set; }
        public List<SelectListItem> CD_WORKS { get; set; }

        //[Required(ErrorMessage="Please Enter Chainage")]
        //[RegularExpression(@"^([0-9/]+)$", ErrorMessage = "Please enter valid chainage.")]
        [Display(Name="Chainage(in Km)[000/000]")]
        public decimal EXEC_RCD_CHAINAGE { get; set; }

        [Required(ErrorMessage="Chainage in Km is required.")]
        [Range(0,9999,ErrorMessage="Chainage in Km is invalid.")]
        public int ChainageMajor { get; set; }
        [Required(ErrorMessage="Chainage in Mtrs is required.")]
        [Range(0.1,999,ErrorMessage="Chainage in Mtrs. is invalid.")]
        public decimal ChainageMinor { get; set; }

        public string EXEC_RCD_STATUS { get; set; }

        public string Operation { get; set; }

        public string BlockName { get; set; }

        public string Package { get; set; }

        public int RoadNo { get; set; }

        public string RoadName { get; set; }

        public double Sanction_Cost { get; set; }

        public decimal Sanction_length { get; set; }

        public string CompleteStatus { get; set; }

        public int CDWorks_No { get; set; }

        public decimal CDWorks_Cost { get; set; }

        [CompareCdWorksCount("CDWorks_No", "IMS_PR_ROAD_CODE","Operation", ErrorMessage = "CDWorks count exceeds the total number of CDWorks")]
        public int PreviousCDWorksCount { get; set; }
    }

    public class CompareFinancialValidation : ValidationAttribute , IClientValidatable
    {
        private readonly string PropertyName;

        public CompareFinancialValidation(string propertyName)
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

            var workValue = Convert.ToDecimal(propertyTestedInfo.GetValue(validationContext.ObjectInstance, null));
            var paymentValue = Convert.ToDecimal(value);

            if (workValue < paymentValue)
            {
                if (paymentValue == null)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
            }
            else
            {
                return ValidationResult.Success;
            }
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "comparefinancialvalidation"
            };
            rule.ValidationParameters["workValue"] = this.PropertyName;
            yield return rule;
        }

    }

    public class CompareCdWorksCount : ValidationAttribute , IClientValidatable
    {
        private readonly string PropertyName;
        private readonly string PropertyProposalCode;
        private readonly string PropertyOperation;

        public CompareCdWorksCount(string propertyName,string proposalCode,string operation)
        {
            this.PropertyName = propertyName;
            this.PropertyProposalCode = proposalCode;
            this.PropertyOperation = operation;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyTestedInfo = validationContext.ObjectType.GetProperty(this.PropertyName);
            var propertyTestedProposal = validationContext.ObjectType.GetProperty(this.PropertyProposalCode);
            var propertyTestedOperation = validationContext.ObjectType.GetProperty(this.PropertyOperation);

            if (propertyTestedInfo == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.PropertyName));
            }
            var proposalCode = Convert.ToInt32(propertyTestedProposal.GetValue(validationContext.ObjectInstance, null));
            var operation = propertyTestedOperation.GetValue(validationContext.ObjectInstance, null);
            ExecutionBAL objBAL = new ExecutionBAL();
            if (operation.ToString().ToLower() == "e")
            {
                return ValidationResult.Success;
            }

            bool status = objBAL.CheckCDWorksCount(proposalCode,operation.ToString());
            if (status)
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
            
            var cdWorksCount = Convert.ToInt32(propertyTestedInfo.GetValue(validationContext.ObjectInstance, null));
            var newCount = Convert.ToInt32(value);

            if ((newCount + 1) <= cdWorksCount)
            {
                return ValidationResult.Success;
            }
            else
            {
                if (newCount == null)
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
                else
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
            }
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "comparecdworkscount"
            };
            rule.ValidationParameters["cdworkscount"] = this.PropertyName;
            yield return rule;
        }
    }

}