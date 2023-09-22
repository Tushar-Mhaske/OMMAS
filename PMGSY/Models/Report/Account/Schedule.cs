using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Report.Account
{
    public class Schedule
    {
        public Int16 ITEM_ID {get;set;}
        public string ITEM_HEADING {get;set;}
        public decimal? CURRENT_AMT {get;set;}
        public decimal? PREVIOUS_AMT { get; set; }
        public Int16 SORT_ORDER { get; set; }
    }

    public class ScheduleModel
    {
        [DisplayName("Select Month")]
        [Range(1, 12, ErrorMessage = "Please Select Month")]
        public short Month { get; set; }
        public List<SelectListItem> ListMonth { get; set; }

        [DisplayName("Select Year")]
        [Range(2000, short.MaxValue, ErrorMessage = "Please Select Year")]
        public short Year { get; set; }
        public List<SelectListItem> ListYear { get; set; }
       
        [DisplayName("Select PIU")]
        //Added By Abhishek Kamble 2-dec-2013
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select DPIU")]
        public int Piu { get; set; }
        public List<SelectListItem> ListPiu { get; set; }

        [DisplayName("Select State")]
        public int State { get; set; }
        public List<SelectListItem> ListState { get; set; }

        [DisplayName("Funding Agency")]
        public int FundingAgency { get; set; }
        public List<MASTER_FUNDING_AGENCY> lstFundingAgency { get; set; }

        [DisplayName("Head")]
        [Range(1, Int16.MaxValue, ErrorMessage = "Please Select Head")]
        public int HeadCode { get; set; }
        public List<ACC_MASTER_HEAD> lstHead { get; set; }

        public List<USP_ACC_RPT_SCHEDULE_OF_ROADS_Result> lstSchedule { get; set; }
        public int LevelId { get; set; }
        public string FundType { get; set; }

        public string Header { get; set; }
        public string FormNumber { get; set; } 
        public string Paragraph1 { get; set; }
        public string Paragraph2 { get; set; }
        public bool IsYearly { get; set; }
        public bool IsSrrdaPiu { get; set; }
        public string PiuName { get; set; }
        public string HeadName { get; set; }
        public string YearName { get; set; }
        public string AgencyName { get; set; }
        public string MonthName { get; set; }
        public string StateName { get; set; }
        public string ScheduleName { get; set; }

        //added by abhishek kamlbe 20-dec-2013 
        public int ReportType { get; set; }
        public int ReportLevel { get; set; }
        public string ReportName { get; set; }
        
    }
    public class AccountFilterModel
    {
        [DisplayName("Select Month")]
        //[Range(1, 12, ErrorMessage = "Please Select Month")]
        [RequiredMonth("ReportType", ErrorMessage = "Please select Month")]
        public short Month { get; set; }
        public List<SelectListItem> ListMonth { get; set; }

        [DisplayName("Select Year")]
        [Range(2000, short.MaxValue, ErrorMessage = "Please Select Year")]
        public short Year { get; set; }
        public List<SelectListItem> ListYear { get; set; }

        [DisplayName("Select PIU")]
        public int Piu { get; set; }
        
        public List<SelectListItem> ListPiu { get; set; }

        [DisplayName("Select Agency")]
        [Range(0, int.MaxValue, ErrorMessage = "Please Select Agency")]
        public int Agency { get; set; }
        public List<SelectListItem> ListAgency { get; set; }

        public int LevelId { get; set; }
        public int ReportType { get; set; }
        public int ReportLevel { get; set; }
    }

    //Added By Ashish Markande
    /// <summary>
    /// 
    /// </summary>
    public class ScheduleUtilization
    {
        //public string LineNo { get; set; }
        //public String Description { get; set; }
        //public decimal InnerColumn { get; set; }
        //public decimal OuterColumn { get; set; }

        public string LineNo { get; set; }
        public string Desc { get; set; }
        public Nullable<decimal> InnerCol { get; set; }
        public Nullable<decimal> OuterCol { get; set; }
    }

    public class ScheduleReconciliation
    {
        public int ID { get; set; }
        public string Desc { get; set; }
        public Nullable<decimal> This_Amt { get; set; }
        public Nullable<decimal> Prev_Amt { get; set; }
    }


    //Added by Abhishek kamble
    public class ScheduleCurrentAssets
    {
        public int ID { get; set; }
        public string Particulars { get; set; }
        public string AmountFlag { get; set; }
        public Nullable<decimal> Amount { get; set; }
    }                              
    //Added by Abhishek kamble
    public class ScheduleDurableAssets
    {
        public string HEAD_CODE { get; set; }
        public string Centarl_State { get; set; }
        public string HEAD_NAME { get; set; }
        public Nullable<decimal> OBAmt { get; set; }
        public Nullable<decimal> MonthlyAmt { get; set; }
        public Nullable<decimal> TotAmt { get; set; }    
    }

    public class ScheduleFundReconciliation
    {
        public int Sno { get; set; }
        public string Particulars { get; set; }
        public string IsAmt { get; set; }
        public Nullable<decimal> InnerAmt { get; set; }
        public Nullable<decimal> OuterAmt { get; set; }
    }

    public class ScheduleCurrentLiabilities
    {
        public string GROUP_ID { get; set; }
        public Nullable<int> ITEM_ID { get; set; }
        public string ITEM_HEADING { get; set; }
        public Nullable<decimal> CURRENT_AMT { get; set; }
        public Nullable<decimal> PREVIOUS_AMT { get; set; }
        public string LINK { get; set; }
        public Nullable<int> SORT_ORDER { get; set; }
    }

    public class ScheduleFundTransferred
    {
        public string GROUP_ID { get; set; }
        public Nullable<int> ITEM_ID { get; set; }
        public string ITEM_HEADING { get; set; }
        public Nullable<decimal> CURRENT_AMT { get; set; }
        public Nullable<decimal> PREVIOUS_AMT { get; set; }
        public string LINK { get; set; }
        public Nullable<int> SORT_ORDER { get; set; }
    }


    //Added By Abhishek Kamble   2-dec-2013
    public class RequiredMonthAttribute : ValidationAttribute, IClientValidatable
    {
        // private const string _defaultErrorMessage = "Start date must be less than end date.";
        private string _basePropertyName;

        public RequiredMonthAttribute(string basePropertyName)
        //: base(_defaultErrorMessage)
        {
            _basePropertyName = basePropertyName;
        }

        //Override default FormatErrorMessage Method  
        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, _basePropertyName);
        }

        //Override IsValid  
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //Get PropertyInfo Object  
            var basePropertyInfo = validationContext.ObjectType.GetProperty(_basePropertyName);

            var IsReportLevelMonthly = basePropertyInfo.GetValue(validationContext.ObjectInstance, null);
            //int month = (int)value;

            var month = value;

            //if (sDate != null && eDate != null)
            //{

            //Actual Validation 
            if ((Convert.ToInt32(IsReportLevelMonthly) == 2) && (Convert.ToInt32(month)==0))
            {
                var message = FormatErrorMessage(validationContext.DisplayName);
                return new ValidationResult(message);
            }

            return ValidationResult.Success;
        }
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                ValidationType = "monthvalidator"
            };
            rule.ValidationParameters["month"] = this._basePropertyName;
            yield return rule;
        }
    }

}