using PMGSY.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.AccountsReports
{
    public class AnnualAccount
    {
        [DisplayName("Year")]
        [Required(ErrorMessage="Please select year.")]
        [Range(1990,2099,ErrorMessage="Please select year")]
        public Int16 Year { get; set; }

        [DisplayName("Balance")]
        [RegularExpression("[CD]", ErrorMessage = "Please select balance.")]
        [Required(ErrorMessage="Please select balance.")]
        
        public String CreditDebit { get; set; }

        [DisplayName("Nodal Agency")]
        [Required(ErrorMessage = "Please select nodal agency.")]
        [Range(1, Int64.MaxValue, ErrorMessage = "Please select nodal agency")]
        public String NodalAgency { get; set; }

        [DisplayName("Program Implementation Unit(PIU)")]
        //[Required(ErrorMessage = "Please select PIU.")]
        ////[RequiredDPIU("Selection",ErrorMessage="Please select PIU.")]
        //[Range(1, Int64.MaxValue, ErrorMessage = "Please select PIU")]
        public String PIU { get; set; }

        public List<USP_RPT_SHOW_YEARLY_ACCOUNT_SELF_Result> lstAnnualReport { get; set; }
        public List<USP_RPT_SHOW_YEARLY_ACCOUNT_ALLPIU_Result> lstReportDPIU { get; set; }

        public List<USP_RPT_SHOW_YEARLY_ACCOUNT_SELF_Result> lstReportSelfDPIU { get; set; }
        public Nullable<Decimal> TotalOpeningAmount { get; set; }

        public Nullable<Decimal> TotalCreditDebit { get; set; }


        public Nullable<Decimal> TotalOpeningAmountDPIU { get; set; }

        public Nullable<Decimal> TotalCreditDebitDPIU { get; set; }


        public Nullable<Decimal> TotalOpeningAmountSelf { get; set; }

        public Nullable<Decimal> TotalCreditDebitSelf { get; set; }

        public string FormNo { get; set; }

        public string ReportParaName { get; set; }

        public string ReportName { get; set; }

        public string FundType { get; set; }

        public List<SelectListItem> lstState { get; set; }
        public List<SelectListItem> lstSRRDA { get; set; }
        public List<SelectListItem> lstPIU { get; set; }

        public string Selection { get; set; }

        [RequiredState("Selection", ErrorMessage = "Please Select State")]
        public string State { get; set; }
        public string SRRDA { get; set; }
        public string DPIU { get; set; }

    }
    //public class RequiredDPIUAttribute : ValidationAttribute, IClientValidatable
    //{
    //    // private const string _defaultErrorMessage = "Start date must be less than end date.";
    //    private string DPIUCode;
      
    //    public RequiredDPIUAttribute(string dpiu)
    //    //: base(_defaultErrorMessage)
    //    {
    //        DPIUCode = dpiu;
    //    }

    //    //Override default FormatErrorMessage Method  
    //    public override string FormatErrorMessage(string name)
    //    {
    //        return string.Format(ErrorMessageString, name, DPIUCode);
    //    }

    //    //Override IsValid  
    //    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    //    {
    //        //Get PropertyInfo Object  
    //        var IsDPIUSel = validationContext.ObjectType.GetProperty(this.DPIUCode);
    //        var DPIUInfo = value;


    //        int dpiu = Convert.ToInt32(DPIUInfo);          
    //        //if (sDate != null && eDate != null)
    //        //{

    //        //Actual Validation 
    //        if ((IsDPIUSel == null) && (dpiu == 0))
    //        {
    //            return ValidationResult.Success;
    //        }

    //        if ((IsDPIUSel.ToString() == "D") && (dpiu == 0))
    //        {
    //            var message = FormatErrorMessage(validationContext.DisplayName);
    //            return new ValidationResult(message);
    //        }     

    //        return ValidationResult.Success;
    //    }


    //    public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
    //    {

    //        var rule = new ModelClientValidationRule
    //        {
    //            ErrorMessage = FormatErrorMessage(metadata.DisplayName),
    //            ValidationType = "requireddpiuinfo"
    //        };
    //        rule.ValidationParameters["dpiu"] = this.DPIUCode;
       
    //        yield return rule;

    //    }

    //}


    //added by abhishek kamble 13-dec-2013


    public class RequiredStateAttribute : ValidationAttribute, IClientValidatable
    {
        private string ReportSelection;
        public RequiredStateAttribute(string Selection)
        //: base(_defaultErrorMessage)
        {
            ReportSelection = Selection;
        }

        //Override default FormatErrorMessage Method  
        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, ReportSelection);
        }

        //Override IsValid  
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //Get PropertyInfo Object  
            var selection = validationContext.ObjectType.GetProperty(this.ReportSelection);
            var state = value;

            //Actual Validation 
            if (PMGSYSession.Current.LevelId!=5)
            {
                string RptSelection = selection.GetValue(validationContext.ObjectInstance, null).ToString();
                if ((RptSelection == "S" || RptSelection == "R") && (Convert.ToInt32(value) == 0))
                {
                    var message = FormatErrorMessage(validationContext.DisplayName);
                    return new ValidationResult(message);
                }
            }
            return ValidationResult.Success;
        }
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                ValidationType = "reqiredstate"
            };
            rule.ValidationParameters["state"] = this.ReportSelection;
            yield return rule;
        }
    }

}