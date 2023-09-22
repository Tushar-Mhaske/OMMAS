/*------------------------------------------------------------------
 * File Name: AccountBillViewModel.cs
 * Project: OMMAS-II
 * Created By: Ashish Markande
 * Creation Date: 24/07/2013
 * Purpose: To set the properties of Account Bill model which are used to set or get the model values.
 * -----------------------------------------------------------------
 * */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.AccountReports
{
    public class AccountBillViewModel
    {

        [Display(Name = "Own")]
        public string SRRDA { get; set; }

        [Display(Name ="DPIU")]
        //[RequiredDPIU("NodalAgency",ErrorMessage="Please Select DPIU")]
        public int DPIU { get; set; }

        [Display(Name = "Month")]
        [RequiredMonthForMonthly("rType",ErrorMessage="Please select Month")]
        public short Month { get; set; }

        [Display(Name = "Year")]
        [RequiredYearForMonthly("rType",ErrorMessage="Please select Year")]
        [RequiredYearForYearly("rType", ErrorMessage = "Please select Year")]
        public short Year { get; set; }

        [Display(Name = "Periodic")]
        public string Period { get; set; }

        [Display(Name = "Bill Type")]        
        [RegularExpression("[0PRJ]",ErrorMessage="Please select Bill Type")]
        public string BillType { get; set; }

        [Display(Name = "Start Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Start Date is not in valid format")]
        [RequiredStartDateForPeriodic("rType",ErrorMessage="Start Date is required.")]
        public string StartDate { get; set; }

        [Display(Name = "End Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "End Date is not in valid format")]
        [AssetDateValidation("StartDate", ErrorMessage = "End Date must be greater than or equal to start date.")]
        [AssetCurrentDateValidation("StartDate", ErrorMessage = "End date must be less than or equals to todays date.")]
        [RequiredEndDateForPeriodic("rType", ErrorMessage = "End Date is required.")]
        public string EndDate { get; set; }


        public Int64 BillId { get; set; }
        public string rType { get; set; }

        public int AdminNdCode { get; set; }

        public string FundType { get; set; }

        public string BilltypeName { get; set; }
        public string MonthName { get; set; }
        public string DPIUName { get; set; }

        public int levelId { get; set; }
        
        public string NodalAgency { get; set; }
        public string DPIUByNO { get; set; }

        public int TotalRecords { get; set; }

        public string DPIUBySRRDA { get; set; }
        public List<SelectListItem> ddlDPIU { get; set; }

        public List<SelectListItem> ddlBillType { get; set; }

        public List<SelectListItem> GetBillType()
        {
            List<SelectListItem> lst = new List<SelectListItem>();
            //lst.Add(new SelectListItem { Text = "--Select--", Value = "O" });
            //Modifed for all txn by abhishek kamble 21-June-2014
            lst.Add(new SelectListItem { Text = "--All--", Value = "0" });
            lst.Add(new SelectListItem { Text = "Payment", Value = "P" });
            lst.Add(new SelectListItem { Text = "Receipts", Value = "R" });
            lst.Add(new SelectListItem { Text = "Transfer Entry Order", Value = "J" });
            return lst;
        }
        
        public AccountBillViewModel()
        {
            ddlBillType = new List<SelectListItem>();
        }
        
        public List<SP_ACC_RPT_DISPALY_Bill_DETAILS_Result> lstAccountBillDetails {get; set;}
      

        //Added By abhishek kamble 15-jan-2014
        public List<SelectListItem> MonthList { get; set; }
        public List<SelectListItem> YearList { get; set; }
    }

    public class AssetDateValidationAttribute : ValidationAttribute, IClientValidatable
    {
        // private const string _defaultErrorMessage = "Start date must be less than end date.";
        private string _basePropertyName;

        public AssetDateValidationAttribute(string basePropertyName)
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

            //Get Value of the property  

            //var startDate = (DateTime)basePropertyInfo.GetValue(validationContext.ObjectInstance, null);            
            //var thisDate = (DateTime)value;  

            var sDate = basePropertyInfo.GetValue(validationContext.ObjectInstance, null);
            var eDate = value;

            if (sDate != null && eDate != null)
            {
                var startDate = ConvertStringToDate(sDate.ToString());
                var thisDate = ConvertStringToDate(eDate.ToString());

                //Actual comparision  
                if (thisDate < startDate)
                {
                    var message = FormatErrorMessage(validationContext.DisplayName);
                    return new ValidationResult(message);
                }

                //System.DateTime toDaysDate = System.DateTime.Now;

                //if (thisDate > toDaysDate)
                //{
                //    //var message = "To date must be less than or equal to today's date.";
                //    var message = FormatErrorMessage(validationContext.DisplayName);
                //    return new ValidationResult(message);
                //}
            }

            //Default return - This means there were no validation error  
            //return null;
            return ValidationResult.Success;
        }


        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            //yield return new ModelClientValidationRule
            //{
            //    ErrorMessage = FormatErrorMessage(metadata.DisplayName), 
            //    ValidationType = "datecomparefieldvalidator"
            //};

            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                ValidationType = "datecomparefieldvalidator"
            };
            rule.ValidationParameters["date"] = this._basePropertyName;
            yield return rule;

        }


        public DateTime? ConvertStringToDate(string dateToConvert)
        {
            DateTime MyDateTime;
            MyDateTime = new DateTime();
            // MyDateTime = DateTime.Parse(dateToConvert);
            MyDateTime = DateTime.ParseExact(dateToConvert, "dd/MM/yyyy", null);
            //Convert.ToDateTime(dateToConvert);         //DateTime.ParseExact(dateToConver, "dd/MM/yyyy",null);
            return MyDateTime;
        }

    }

    public class AssetCurrentDateValidationAttribute : ValidationAttribute, IClientValidatable
    {

        // private const string _defaultErrorMessage = "Start date must be less than end date.";
        private string _basePropertyName;

        public AssetCurrentDateValidationAttribute(string basePropertyName)
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

            //Get Value of the property  

            //var startDate = (DateTime)basePropertyInfo.GetValue(validationContext.ObjectInstance, null);            
            //var thisDate = (DateTime)value;  

            var sDate = basePropertyInfo.GetValue(validationContext.ObjectInstance, null);
            var eDate = value;

            if (eDate != null)
            {
                var thisDate = ConvertStringToDate(eDate.ToString());

                System.DateTime toDaysDate = System.DateTime.Now;  
                if (thisDate > toDaysDate)
                {
                    var message = FormatErrorMessage(validationContext.DisplayName);
                    return new ValidationResult(message);
                }
            }
            return ValidationResult.Success;
        }


        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            //yield return new ModelClientValidationRule
            //{
            //    ErrorMessage = FormatErrorMessage(metadata.DisplayName), 
            //    ValidationType = "datecomparefieldvalidator"
            //};

            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                ValidationType = "currentdatefieldvalidator"
            };
            rule.ValidationParameters["date"] = this._basePropertyName;
            yield return rule;

        }


        public DateTime? ConvertStringToDate(string dateToConvert)
        {
            DateTime MyDateTime;
            MyDateTime = new DateTime();
            MyDateTime = DateTime.ParseExact(dateToConvert, "dd/MM/yyyy", null);
            return MyDateTime;
        }

    }



    //Added By Abhishek Kamble 12-Nov-2013

    public class RequiredMonthForMonthlyAttribute : ValidationAttribute, IClientValidatable
    {
        private string _baasePropertyName;

        public RequiredMonthForMonthlyAttribute(string basePropertyName)
        {
            _baasePropertyName = basePropertyName;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, _baasePropertyName);
        }

        //Override IsValid()      
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            //Get Property Info Object
            var basePropertyInfo = validationContext.ObjectType.GetProperty(_baasePropertyName);
            var IsMonthly = basePropertyInfo.GetValue(validationContext.ObjectInstance, null);
            int Month = Convert.ToInt32(value);

            //Actual Validation
            if ((IsMonthly.ToString() == "M") && (Month == 0))
            {
                var message = FormatErrorMessage(validationContext.DisplayName);
                return new ValidationResult(message);
            }                               
            return ValidationResult.Success;
        }//end of IsValid()          

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {

            var rule = new ModelClientValidationRule { 
              ErrorMessage=FormatErrorMessage(metadata.DisplayName),
              ValidationType="requiredmonthformonthly"
            
            };
            rule.ValidationParameters["month"] = this._baasePropertyName;
            yield return rule;
        }
    }

    public class RequiredYearForMonthlyAttribute : ValidationAttribute, IClientValidatable
    {
        private string _baasePropertyName;

        public RequiredYearForMonthlyAttribute(string basePropertyName)
        {
            _baasePropertyName = basePropertyName;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, _baasePropertyName);
        }

        //Override IsValid()      
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            //Get Property Info Object
            var basePropertyInfo = validationContext.ObjectType.GetProperty(_baasePropertyName);
            var IsMonthly = basePropertyInfo.GetValue(validationContext.ObjectInstance, null);
            int Year = Convert.ToInt32(value);

            //Actual Validation
            if ((IsMonthly.ToString() == "M") && (Year == 0))
            {
                var message = FormatErrorMessage(validationContext.DisplayName);
                return new ValidationResult(message);
            }
            return ValidationResult.Success;
        }//end of IsValid()          

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {

            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                ValidationType = "requiredyearformonthly"

            };
            rule.ValidationParameters["year"] = this._baasePropertyName;
            yield return rule;
        }
    }

    public class RequiredYearForYearlyAttribute : ValidationAttribute, IClientValidatable
    {
        private string _baasePropertyName;

        public RequiredYearForYearlyAttribute(string basePropertyName)
        {
            _baasePropertyName = basePropertyName;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, _baasePropertyName);
        }

        //Override IsValid()      
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            //Get Property Info Object
            var basePropertyInfo = validationContext.ObjectType.GetProperty(_baasePropertyName);
            var IsYearly = basePropertyInfo.GetValue(validationContext.ObjectInstance, null);
            int Year = Convert.ToInt32(value);

            //Actual Validation
            if ((IsYearly.ToString() == "Y") && (Year == 0))
            {
                var message = FormatErrorMessage(validationContext.DisplayName);
                return new ValidationResult(message);
            }
            return ValidationResult.Success;
        }//end of IsValid()          

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {

            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                ValidationType = "requiredyearforyearly"

            };
            rule.ValidationParameters["year"] = this._baasePropertyName;
            yield return rule;
        }
    }

    public class RequiredStartDateForPeriodicAttribute : ValidationAttribute, IClientValidatable
    {
        private string _baasePropertyName;

        public RequiredStartDateForPeriodicAttribute(string basePropertyName)
        {
            _baasePropertyName = basePropertyName;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, _baasePropertyName);
        }

        //Override IsValid()      
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            //Get Property Info Object
            var basePropertyInfo = validationContext.ObjectType.GetProperty(_baasePropertyName);
            var IsPeriodic = basePropertyInfo.GetValue(validationContext.ObjectInstance, null);
           // int Year = Convert.ToInt32(value);

            //Actual Validation
            if ((IsPeriodic.ToString() == "P") && (value == null))
            {
                var message = FormatErrorMessage(validationContext.DisplayName);
                return new ValidationResult(message);
            }
            return ValidationResult.Success;
        }//end of IsValid()          

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                ValidationType = "requiredstartdateforperiodic"

            };
            rule.ValidationParameters["startdate"] = this._baasePropertyName;
            yield return rule;
        }
    }

    public class RequiredEndDateForPeriodicAttribute : ValidationAttribute, IClientValidatable
    {
        private string _baasePropertyName;

        public RequiredEndDateForPeriodicAttribute(string basePropertyName)
        {
            _baasePropertyName = basePropertyName;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, _baasePropertyName);
        }

        //Override IsValid()      
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            //Get Property Info Object
            var basePropertyInfo = validationContext.ObjectType.GetProperty(_baasePropertyName);
            var IsPeriodic = basePropertyInfo.GetValue(validationContext.ObjectInstance, null);
            // int Year = Convert.ToInt32(value);

            //Actual Validation
            if ((IsPeriodic.ToString() == "P") && (value == null))
            {
                var message = FormatErrorMessage(validationContext.DisplayName);
                return new ValidationResult(message);
            }
            return ValidationResult.Success;
        }//end of IsValid()          

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                ValidationType = "requiredenddateforperiodic"

            };
            rule.ValidationParameters["enddate"] = this._baasePropertyName;
            yield return rule;
        }
    }
    public class RequiredDPIUAttribute : ValidationAttribute, IClientValidatable
    {
        private string _baasePropertyName;

        public RequiredDPIUAttribute(string basePropertyName)
        {
            _baasePropertyName = basePropertyName;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, _baasePropertyName);
        }

        //Override IsValid()      
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            //Get Property Info Object
            var basePropertyInfo = validationContext.ObjectType.GetProperty(_baasePropertyName);
            var IsDPIU = basePropertyInfo.GetValue(validationContext.ObjectInstance, null);
            int DPIU = Convert.ToInt32(value);

            //Actual Validation
            if (IsDPIU != null)
            {
                if ((IsDPIU.ToString() == "D") && (DPIU == 0))
                {
                    var message = FormatErrorMessage(validationContext.DisplayName);
                    return new ValidationResult(message);
                }
            }
            return ValidationResult.Success;
        }//end of IsValid()          

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                ValidationType = "requireddpiu"

            };
            rule.ValidationParameters["dpiu"] = this._baasePropertyName;
            yield return rule;
        }
    }

}
