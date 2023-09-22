using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.QMSSRSReports.Models
{
    public class QMGradingAbstract
    {
        [Required(ErrorMessage = "From Month required")]
        [Range(1, 12, ErrorMessage = "Invalid From Month")]
        [Display(Name = "From Month")]
        public int FrmMonth { get; set; }
        public List<SelectListItem> FrmMonthList { get; set; }

        [Required(ErrorMessage = "From Year required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid From Year")]
        [Display(Name = "From Year")]
        public int FrmYear { get; set; }
        public List<SelectListItem> FrmYearList { get; set; }

        [Required(ErrorMessage = "To Month required")]
        [Range(1, 12, ErrorMessage = "Invalid To Month")]
        [Display(Name = "To Month")]
        public int ToMonth { get; set; }
        public List<SelectListItem> ToMonthList { get; set; }

        [Required(ErrorMessage = "To Year required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid To Year")]
        [Display(Name = "To Year")]
        [DateValidationVST("FrmYear", "FrmMonth", "ToMonth", ErrorMessage = "To Year must be greater than or equal to From Year.")]
        public int ToYear { get; set; }
        public List<SelectListItem> ToYearList { get; set; }

        [Required(ErrorMessage = "Localized value required")]
        [RegularExpression(@"^(\b(en|hi)\b+)$", ErrorMessage = "Invalid Localized Value")]
        //[Display(Name = "Localized")]
        public string localizedValue { get; set; }

        public int LevelCode { get; set; }
        public string StateName { get; set; }
        public string DistName { get; set; }
        public string BlockName { get; set; }
        public int Mast_State_Code { get; set; }
        public int Mast_Block_Code { get; set; }
        public int Mast_District_Code { get; set; }

        [Display(Name = "State")]
        [Required(ErrorMessage = "Please select State. ")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select  State.")]
        // [RegularExpression(@"^([0-9]+)$", ErrorMessage = "State must be valid number.")]
        public int StateCode { get; set; }
        public List<SelectListItem> StateList { get; set; }

        [Display(Name = "District")]
        [Required(ErrorMessage = "Please select District.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select District.")]
        //[RegularExpression(@"^([0-9]+)$", ErrorMessage = "District must be valid number.")]
        public int DistrictCode { get; set; }
        public List<SelectListItem> DistrictList { get; set; }

        [Display(Name = "Block")]
        [Required(ErrorMessage = "Please select Block.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select Block.")]
        // [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Block must be valid number.")]
        public int BlockCode { get; set; }
        public List<SelectListItem> BlockList { get; set; }


        [Display(Name = "Monitor Type")]
        //[RoadHabitationValidation("RH", "RSearch", ErrorMessage = "Please enter/select road habitation details")]
        public char qmtype { get; set; }
        public List<SelectListItem> qmTypeList { get; set; }

        [Display(Name = "Scheme")]
        public int schemeType { get; set; }
        public List<SelectListItem> schemeList { get; set; }

        public string FromMonthName { get; set; }
        public string ToMonthName { get; set; }
        public string FromYearName { get; set; }
        public string ToYearName { get; set; }
    }

    public class DateValidationVST1Attribute : ValidationAttribute, IClientValidatable
    {

        // private const string _defaultErrorMessage = "Start date must be less than end date.";
        private string _FrmMonth;
        private string _FrmYear;
        private string _ToMonth;

        public DateValidationVST1Attribute(string FrmYear1, string FrmMonth1, string ToMonth1)
        //: base(_defaultErrorMessage)
        {
            _FrmMonth = FrmMonth1;
            _FrmYear = FrmYear1;
            _ToMonth = ToMonth1;

        }

        //Override default FormatErrorMessage Method  
        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, _FrmMonth);
        }

        //Override IsValid  
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                //Get PropertyInfo Object  
                var basePropertyInfo = validationContext.ObjectType.GetProperty(_FrmMonth);

                //Get Value of the property  

                //var startDate = (DateTime)basePropertyInfo.GetValue(validationContext.ObjectInstance, null);            
                //var thisDate = (DateTime)value;  

                var sDate = basePropertyInfo.GetValue(validationContext.ObjectInstance, null);
                var eDate = value;

                if (sDate != null && eDate != null)
                {
                    //var startDate = ConvertStringToDate(sDate.ToString());
                    //var thisDate = ConvertStringToDate(eDate.ToString());
                    var startDate = Convert.ToInt32(sDate);
                    var thisDate = Convert.ToInt32(eDate);

                    //Actual comparision  
                    if (thisDate == startDate)
                    {
                        if (Convert.ToInt32(_FrmMonth) < Convert.ToInt32(_ToMonth))
                        {
                            var message = FormatErrorMessage(validationContext.DisplayName);
                            return new ValidationResult(message);
                        }
                    }
                    else if (thisDate < startDate)
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
                // var message = FormatErrorMessage(validationContext.DisplayName);
                return null; //new ValidationResult(message);
            }
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
            rule.ValidationParameters["date"] = this._FrmMonth;
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
}