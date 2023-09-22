using PMGSY.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Report
{
    public class ImprestSettlementViewModel
    {
        public ImprestSettlementViewModel()
        {
            lstFinancialYears = new List<SelectListItem>();
            lstSrrda = new List<SelectListItem>();
            lstDpiu = new List<SelectListItem>();
        }

        [Display(Name = "Report Type")]
        [IsValidReport("SrrdaAdminCode", "DpiuAdminCode",ErrorMessage="Please select proper options depending on report type.")]
        public string ReportLevel { get; set; }

        [Required(ErrorMessage = "Please select Financial Year.")]
        [Range(2000, 2099, ErrorMessage = "Please select Financial Year.")]
        [Display(Name = "Financial Year")]
        public short FinancialYear { get; set; }

        [Display(Name = "Nodal Agency")]
        public int SrrdaAdminCode { get; set; }

        [Display(Name = "DPIU")]
        public int DpiuAdminCode { get; set; }

        public int AdminCode { get; set; }

        public int LevelId { get; set; }

        public string ReportFormNumber { get; set; }

        public string FundTypeName { get; set; }

        public string ReportName { get; set; }

        public string ReportParagraphName { get; set; }

        public string StateName { get; set; }

        public string DPIUName { get; set; }

        public string NodalAgency { get; set; }

        public List<SelectListItem> lstFinancialYears { get; set; }

        public List<SelectListItem> lstSrrda { get; set; }

        public List<SelectListItem> lstDpiu { get; set; }

    }

    public class IsValidReport : ValidationAttribute//, IClientValidatable
    {
        private string DefaultErrorMessage = "Please select proper choice.";
        private readonly string _srrdaCode;
        private readonly string _dpiuCode;

        public IsValidReport(string srrdaCode, string dpiuCode)
        {
            this._srrdaCode = srrdaCode;
            this._dpiuCode = dpiuCode;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(DefaultErrorMessage, name, _srrdaCode);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            String amountToCompare = String.Empty;

            var propertyTestedSRRDA = validationContext.ObjectType.GetProperty(this._srrdaCode);
            var PropertyTestedDPIU = validationContext.ObjectType.GetProperty(this._dpiuCode);

            if (propertyTestedSRRDA == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this._srrdaCode));
            }

            if (PropertyTestedDPIU == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this._dpiuCode));
            }

            var testedValueSrrda = propertyTestedSRRDA.GetValue(validationContext.ObjectInstance, null);
            var testedValueDpiu = PropertyTestedDPIU.GetValue(validationContext.ObjectInstance, null);

            if (PMGSYSession.Current.LevelId == 5)
            {
                return ValidationResult.Success;
            }

            if (value == null)
            {
                return new ValidationResult("Please select proper report type.");
            }


            if (value.ToString().ToLower() == "s")
            {
                if (!String.IsNullOrEmpty(testedValueSrrda.ToString()) && Convert.ToInt32(testedValueSrrda) != 0)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult("Please select SRRDA.");
                }
            }

            if (value.ToString().ToLower() == "d")
            {
                if (!String.IsNullOrEmpty(testedValueSrrda.ToString()) && Convert.ToInt32(testedValueSrrda) != 0)
                {
                    if (!String.IsNullOrEmpty(testedValueDpiu.ToString()) && Convert.ToInt32(testedValueDpiu) != 0)
                    {
                        return ValidationResult.Success;
                    }
                    else
                    {
                        return new ValidationResult("Please select DPIU.");
                    }
                }
                else
                {
                    return new ValidationResult("Please select SRRDA.");
                }
            }

            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "isvalidreport"
            };
            yield return rule;
        }
    }
}