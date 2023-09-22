#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   LabFileUploadViewModel.cs        
        * Description   :   Properties for File Upload in quality module
        * Author        :   Anand Singh 
        * Creation Date :   16/Sept/2014
 **/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.QualityMonitoring
{
    public class LabFileUploadViewModel
    {
        public int? QM_FILE_ID { get; set; }
        public int QM_LAB_ID { get; set; }
        public string QM_SQC_APPROVAL { get; set; }
        public string QM_LOCK_STATUS { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "Invalid Image Description,Can only contains AlphaNumeric values")]
        public string Image_Description { get; set; }

        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "Invalid chainage,Can only contains Numeric values and Total 7 Digits and 3 digits after decimal place")]
        public decimal? chainage { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "Invalid Pdf Description,Can only contains AlphaNumeric values")]
        public string PdfDescription { get; set; }

        public int? NumberofFiles { get; set; }

        public int? NumberofImages { get; set; }
        public int? NumberofPdfs { get; set; }

        public string url { get; set; }
        public string thumbnail_url { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public int size { get; set; }
        public string delete_url { get; set; }
        public string delete_type { get; set; }

        public string ErrorMessage { get; set; }

        [Display(Name = "Latitude")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,12})?\s*$", ErrorMessage = "Invalid Latitude ,Can only contains Numeric values and 12 digit after decimal place")]
        [Range(0, 999999.999999999999, ErrorMessage = "Invalid Latitude ,Can only contains Numeric values and 12 digit after decimal place")]
        // [Required(ErrorMessage = "Please enter Total Road Length.")]
        public decimal Latitude { get; set; }

        [Display(Name = "Longitude")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,12})?\s*$", ErrorMessage = "Invalid Longitude ,Can only contains Numeric values and 12 digit after decimal place")]
        [Range(0, 999999.999999999999, ErrorMessage = "Invalid Longitude ,Can only contains Numeric values and 12 digit after decimal place")]
       // [Required(ErrorMessage = "Please enter Total Road Length.")]
        public decimal Longitude { get; set; }

    
    }
    public class LabDateViewModel
    {

        [Display(Name = "State")]
        public string State_Name { get; set; }

        [Display(Name = "Sanction Year")]
        public string Sanction_Year { get; set; }

        [Display(Name = "Package")]
        public string Package { get; set; }

        [Display(Name = "Date of Award")]
        public string Date_Of_Award { get; set; }

        [Display(Name = "Commencement Date")]
        public string Commencemend_Date { get; set; }

        [Display(Name = "Completed Date")]
        public string Completed_Date { get; set; }

        [Display(Name = "Agreement Date")]
        public string Agreement_Date { get; set; }

        public string hdCommencementDate { get; set; }
        public string hdAgreementDate { get; set; }

        public int Agreement_No { get; set; }

        public string Current_Date
        {
            get
            {
                return DateTime.Now.ToString("dd/MM/yyyy");
            }          
        }

        [Display(Name = "Enter Lab Established Date:")]
        [Required(ErrorMessage = "Lab Established Date is required.")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Inspection End Date must be in dd/mm/yyyy format.")]
        [LabCommenceDateValidationAttribute("hdAgreementDate", "Current_Date", "AGREEMENT_EST_DATE", ErrorMessage = "Lab Established Date must be greater than or equal to agreement date and less than or equal to current date.")]
        public string AGREEMENT_EST_DATE { get; set; }

    }

}
public class LabCommenceDateValidationAttribute : ValidationAttribute, IClientValidatable
{

    // private const string _defaultErrorMessage = "Start date must be less than end date.";
    private string _baseStartName;
    private string _baseEndName;
    private string _baseValue;


    public LabCommenceDateValidationAttribute(string basePropertyStartName, string basePropertyEndName, string baseValue)
    //: base(_defaultErrorMessage)
    {
        _baseStartName = basePropertyStartName;
        _baseEndName = basePropertyEndName;
        _baseValue = baseValue;
    }

    //Override default FormatErrorMessage Method  
    public override string FormatErrorMessage(string name)
    {
        return string.Format(ErrorMessageString, name, _baseStartName);
    }

    //Override IsValid  
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        try
        {
            //Get PropertyInfo Object  
            var baseStartPropertyInfo = validationContext.ObjectType.GetProperty(_baseStartName);
            var baseENDPropertyInfo = validationContext.ObjectType.GetProperty(_baseEndName);
            _baseValue = value.ToString();

            //Get Value of the property  

            //var startDate = (DateTime)basePropertyInfo.GetValue(validationContext.ObjectInstance, null);            
            //var thisDate = (DateTime)value;  

            var stDate = baseStartPropertyInfo.GetValue(validationContext.ObjectInstance, null);
            var enDate = baseENDPropertyInfo.GetValue(validationContext.ObjectInstance, null);

            var actDate = value;

            if (stDate != null && actDate != null && enDate != null)
            {
                var startDate = ConvertStringToDate(stDate.ToString()); //Agrrement Commencement Date
                var endDate = ConvertStringToDate(enDate.ToString()); //Current System te
                var thisDate = ConvertStringToDate(actDate.ToString()); // Establsihment Date

                //Actual comparision  
                if (thisDate < startDate || thisDate > endDate)
                {
                    var message = FormatErrorMessage(validationContext.DisplayName);
                    return new ValidationResult(message);
                }
            }

            //Default return - This means there were no validation error  
            //return null;
            return ValidationResult.Success;
        }
        catch (Exception ex)
        {
            //var message = FormatErrorMessage(validationContext.DisplayName);
            //return new ValidationResult(message);
            return null;
        }
    }


    public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
    {
        //yield return new ModelClientValidationRule
        //{
        //    ErrorMessage = FormatErrorMessage(metadata.DisplayName),
        //    //This is the name of the method aaded to the jQuery validator method (must be lower case)
        //    ValidationType = "maintenancedatevalidator"
        //};

        var rule = new ModelClientValidationRule
        {
            ErrorMessage = FormatErrorMessage(metadata.DisplayName),
            ValidationType = "labcommencedatevalidator"
        };
        //rule.ValidationParameters["date"] = this._baseStartName;
        rule.ValidationParameters["date"] = this._baseValue;
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