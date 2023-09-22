using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

/*
 *  Developer Name: Anand Singh
 *  Date: 27/072013
 *  Purpose: Model for Balace sheet for all three fund Admin Fund, Program Fund and Maintenance Fund at 
 *          SRRDA Login and DPIU Login.
 * 
 * 
 * */

namespace PMGSY.Models.Report.Account
{

    
   
    public class BalanceSheet
    {
        public BalanceSheet()
        {
            // new change done on 29-Aug-2013
            DPIUList = new List<SelectListItem>();

            //new change done in absence of vss by Vikram
            NodalAgencyList = new List<SelectListItem>();
        }

        [Display(Name = "Month")]
        //[Range(0, 12, ErrorMessage = "Please Select Month")]
        //Modified by abhishek kamble 3-nov-2013
        [RequiredMonth("IsMonthlyYearly", ErrorMessage = "Please select Month")]
        public short Month { get; set; }
        public List<SelectListItem> MonthList { get; set; }
        
        //added by abhishek kamble 3-dec-2013
        public int IsMonthlyYearly { get; set; }
        public string showMonthName { get; set; }
        public string showDPIUName { get; set; }


        [Display(Name = "Year")]
        [Range(1, short.MaxValue, ErrorMessage = "Please Select Year")]
        public short Year { get; set; }
        public List<SelectListItem> YearList { get; set; }

        // O (srrda and dpiu) , S--STATE , A-all piu
        public char ReportLevel { get; set; }
        public int ReportNumber { get; set; }
        public int ReportDPIU { get; set; }

        //new change done on 29-Aug-2013
        public int? AdminCode { get; set; }
        public List<SelectListItem> DPIUList { get; set; }

        //new change done in absence of vss by Vikram
        public List<SelectListItem> NodalAgencyList { get; set; } //list for population of Nodal Agency when MRD is in login

        //following parameters are used at MORD level
        public int NodalAdminCode { get; set; }
        public int StateAdminCode { get; set; }
        public int AllDPIUAdminCode { get; set; }

        //Added By Abhishek kamble 24-dec-2013
        public int? HeadCode { get; set; }
        public string HeadName { get; set; }

        public int SelectedMonth { get; set; }
        public int SelectedYear { get; set; }

    }
    public class BalanceSheetList
    {
        public List<USP_RPT_SHOW_BALSHEET_Result> ListBalanceSheet { get; set; }
        public string MonthName { get; set; }
        public int Year { get; set; }
        public string FundType { get; set; }
        public string ReportFormNumber { get; set; }
        public string ReportHeader { get; set; }
        public string Section { get; set; }
        public string SelectionHeader { get; set; }

        //added by abhishek kamble 3-dec-2013
        public int IsMonthlyYearly { get; set; }
        public BalanceSheet balanceSheet { get; set; }


        //new added
        public string Type { get; set; }
        public string DepartmentName { get; set; }
        public string NodalAgency { get; set; }
        //public decimal totalAssetCurrentAmount { get; set; }
        //public decimal totalAssetPreviousAmount { get; set; }
        //public decimal totalLiabilitiesCurrentAmount { get; set; }
        //public decimal totalLiabilitiesPreviousAmount { get; set; }
        //public short MONTH { get; set; }
        //public string ReportLevelCode { get; set; }


    }


    //Added By Abhishek Kamble   2-dec-2013
    //public class RequredMonthAttribute : ValidationAttribute, IClientValidatable
    //{
    //    // private const string _defaultErrorMessage = "Start date must be less than end date.";
    //    private string _basePropertyName;

    //    public RequredMonthAttribute(string basePropertyName)
    //    //: base(_defaultErrorMessage)
    //    {
    //        _basePropertyName = basePropertyName;
    //    }

    //    //Override default FormatErrorMessage Method  
    //    public override string FormatErrorMessage(string name)
    //    {
    //        return string.Format(ErrorMessageString, name, _basePropertyName);
    //    }

    //    //Override IsValid  
    //    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    //    {
    //        //Get PropertyInfo Object  
    //        var basePropertyInfo = validationContext.ObjectType.GetProperty(_basePropertyName);

    //        var IsReportLevelMonthly = basePropertyInfo.GetValue(validationContext.ObjectInstance, null);
    //        int month = (int)value;

    //        //if (sDate != null && eDate != null)
    //        //{

    //        //Actual Validation 
    //        if ((Convert.ToInt32(IsReportLevelMonthly) == 2) && (month == 0))
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
    //            ValidationType = "monthvalidator"
    //        };
    //        rule.ValidationParameters["month"] = this._basePropertyName;
    //        yield return rule;
    //    }
    //}


}