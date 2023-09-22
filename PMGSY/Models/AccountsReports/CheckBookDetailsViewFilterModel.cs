#region File Header...
/* 
     *  Name : CheckBookDetailsViewFilterModel.cs
     *  Path : ~\PMGSY\Models\CheckBookDetailsViewFilterModel.cs
     *  Description : CheckBookDetailsViewFilterModel Class used to set or get the model values, 
     *  Author : Abhishek Kamlble(PE, e-gov)
     *  Company : C-DAC,E-GOV
     *  Dates of Creation : 26/07/2013           
 */
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace PMGSY.Models.AccountReports
{
    public class CheckBookDetailsViewFilterModel
    {

        public CheckBookDetailsViewFilterModel() 
        {
            lstYears = new List<SelectListItem>();
            lstMonths = new List<SelectListItem>();
        }

        [Display(Name="SRRDA")]
        public int SRRDA { get; set; }
        
        [Display(Name = "Lower")]
        //[IsDPIURequired("LevelId", ErrorMessage = "Please Select DPIU")]
        public int DPIU { get; set; }

        public string UserLevel { get; set; }

        [Display(Name = "Month")]
        public short Month { get; set; }
        
        //[Display(Name = "Year")]

        public int LevelId { get; set; }

        [RequiredMonthYear("Month", "Year","CheckbookSeries", ErrorMessage = "Please Select Month and Year")]
        public string CheckbookMonthYearWise { get; set; }

        public short Year { get; set; }

        [Display(Name = "Cheque Book Series")]
        public int CheckbookSeries { get; set; }

        [Display(Name = "Name Of State")]
        public string StateName { get; set; }

        [Display(Name = "Name Of DPIU")]
        public string PIUName { get; set; }

        [Display(Name = "Name Of Bank")]
        public string BankName { get; set; }

        [Display(Name = "Month")]
        public string MonthName { get; set; }

        [Display(Name = "Year")]
        public string YearName { get; set; }

        [Display(Name = "Cheque Book Series")]
        public string CheckbookSeriesName { get; set; }

        [Display(Name="Name Of SRRDA")]
        public string NodalAgencyName { get; set; }

        [Display(Name = "Selection Type")]
        
        public string MonthlyOrChequebookWiseSelection{ get; set; }
        
        public int totalRecords { get; set; }

        public List<SelectListItem> lstMonths { get; set; }

        public List<SelectListItem> lstYears { get; set; }

        //lstCheckbookDetails List contains cheque book details
        public List<SP_ACC_RPT_PIU_CHEQUE_ISSUED_DETAILS_Result> lstCheckbookDetails { get; set; }

        //lstChequeIssuedAbstract List contains Cheque book abstract details.
        public List<SP_ACC_RPT_PIU_CHEQUE_ISSUED_ABSTRACT_Result> lstChequeIssuedAbstract { get; set; }

        //lstChequeOutstandingDetails List contains Cheque book outstanding details.
        public List<SP_ACC_RPT_PIU_CHEQUE_OUTSTANDING_DETAILS_Result> lstChequeOutstandingDetails { get; set; }


        //Report Header parameter
        public string FundTypeName { get; set; }
        public string ReportName { get; set; }
        public string ReportParagraphName { get; set; }
        public string ReportFormNumber { get; set; }
    }

    public class RequiredMonthYearAttribute : ValidationAttribute, IClientValidatable
    {
        // private const string _defaultErrorMessage = "Start date must be less than end date.";
        private string Month;
        private string Year;
        private string chqDetails;
        public RequiredMonthYearAttribute(string month,string year,string chqSeries)
        //: base(_defaultErrorMessage)
        {
            Month = month;
            Year = year;
            chqDetails = chqSeries;
            
        }

        //Override default FormatErrorMessage Method  
        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name,Month,Year);
        }

        //Override IsValid  
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //Get PropertyInfo Object  
            var MonthInfo =validationContext.ObjectType.GetProperty(this.Month);
            var YearInfo =validationContext.ObjectType.GetProperty(this.Year);
            var ChequeInfo = validationContext.ObjectType.GetProperty(this.chqDetails);
            var IsMonthChqWise = value;


            int month = Convert.ToInt32(MonthInfo.GetValue(validationContext.ObjectInstance, null));
            int year = Convert.ToInt32(YearInfo.GetValue(validationContext.ObjectInstance, null));

            int cheque = Convert.ToInt32(ChequeInfo.GetValue(validationContext.ObjectInstance, null));
            //if (sDate != null && eDate != null)
            //{

            //Actual Validation 
            if ((value == null) && (month == 0) &&(year == 0))
            {
                return ValidationResult.Success;
            }
            if ((value.ToString() == "M") && (month == 0) && (year == 0))
            {
                var message = FormatErrorMessage(validationContext.DisplayName);
                return new ValidationResult(message);
            }
            else if ((value.ToString() == "M") && (month > 0) && (year == 0))
            {
                var message = FormatErrorMessage(validationContext.DisplayName);
                return new ValidationResult("Please select year.");
            }
            else if ((value.ToString() == "M") && (month == 0) && (year > 0))
            {
                var message = FormatErrorMessage(validationContext.DisplayName);
                return new ValidationResult("Please select month.");
            }
            else if ((value.ToString() == "C") && (cheque==0))
            {
                var message = FormatErrorMessage(validationContext.DisplayName);
                return new ValidationResult("Please select cheque book series");
            }

            return ValidationResult.Success;
        }


        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {

            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                ValidationType = "reqiredmonthyear"
            };
            rule.ValidationParameters["month"] = this.Month;
            rule.ValidationParameters["year"] = this.Year;
            rule.ValidationParameters["cheque"] = this.chqDetails;
            yield return rule;

        }

    }

    //added by abhishek kamble 13-dec-2013
    public class IsDPIURequiredAttribute : ValidationAttribute, IClientValidatable
    {
        private string LevelID;
        public IsDPIURequiredAttribute(string levelID)
        //: base(_defaultErrorMessage)
        {
            LevelID = levelID;
        }

        //Override default FormatErrorMessage Method  
        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, LevelID);
        }

        //Override IsValid  
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //Get PropertyInfo Object  
            var LvlId = validationContext.ObjectType.GetProperty(this.LevelID);
            var DPIU= value;

            int LevelID = Convert.ToInt32(LvlId.GetValue(validationContext.ObjectInstance, null));
            
            //Actual Validation 
            if (LevelID != 5)
            {
                if (Convert.ToInt32(DPIU)== 0)
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
                ValidationType = "reqireddpiu"
            };
            rule.ValidationParameters["levelid"] = this.LevelID;            
            yield return rule;
        }                     
    }
}