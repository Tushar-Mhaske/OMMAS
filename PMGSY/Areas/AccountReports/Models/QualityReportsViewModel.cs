using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using PMGSY.Models;

namespace PMGSY.Areas.AccountReports.Models
{
    public class QualityReportsViewModel
    {
        public string ValueType { get; set; }

        public int Grade { get; set; }

        public int hdnRole { get; set; }

        //public string qmType { get; set; }
        public int InspCount { get; set; }
        public string fundType { get; set; }

        public string QCName { get; set; }
        public string QCPhone { get; set; }
        public string CEOName { get; set; }
        public string CEOPhone { get; set; }

        public string StateName { get; set; }
        public string DistName { get; set; }
        public string BlockName { get; set; }

        public string RoadStat { get; set; }

        public string FromMonthName { get; set; }
        public string ToMonthName { get; set; }
        public string FromYearName { get; set; }
        public string ToYearName { get; set; }

        public string GradingItemName { get; set; }

        [Display(Name = "From Month")]
        [Range(0, 12, ErrorMessage = "Please select valid From Month.")]
        public int FromMonth { get; set; }
        public List<SelectListItem> FromMonthList { get; set; }

        [Display(Name = "Level")]
        [Range(0, 3, ErrorMessage = "Please select Level.")]
        public int Level { get; set; }
        public List<SelectListItem> LevelList { get; set; }


        [Display(Name = "To Month")]
        [Range(0, 12, ErrorMessage = "Please select valid To Month.")]
        public int ToMonth { get; set; }
        public List<SelectListItem> ToMonthList { get; set; }


        [Display(Name = "From Year")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid From Year.")]
        public int FromYear { get; set; }
        public List<SelectListItem> FromYearList { get; set; }

        [Display(Name = "To Year")]
        [DateValidationVST("FromYear", "FromMonth", "ToMonth", ErrorMessage = "To Year must be greater than or equal to From Year.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid To Year.")]
        public int ToYear { get; set; }
        public List<SelectListItem> ToYearList { get; set; }

        [Display(Name = "State")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid State.")]
        public int State { get; set; }
        public List<SelectListItem> StateList { get; set; }

        [Display(Name = "District")]
        //[Required(ErrorMessage = "Please select a District.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid District.")]
        public int District { get; set; }
        public List<SelectListItem> DistrictList { get; set; }

        [Display(Name = "Block")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Block.")]
        public int Block { get; set; }
        public List<SelectListItem> BlockList { get; set; }

        [Display(Name = "Year")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Year.")]
        public int Year { get; set; }
        public List<SelectListItem> YearList { get; set; }

        [Display(Name = "Month")]
        [Range(0, 12, ErrorMessage = "Please select valid Month.")]
        public int Month { get; set; }
        public List<SelectListItem> MonthList { get; set; }

        [Display(Name = "Grading Item")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Grading Item.")]
        public int GradingItem { get; set; }
        //public List<SelectListItem> GradingItemList { get; set; }
        public List<MASTER_QM_ITEM> GradingItemList { get; set; }

        [Display(Name = "Quality Monitor Type")]
        [RegularExpression(@"^([SI]+)$", ErrorMessage = "Invalid QM Type selected")]
        public string qmType { get; set; }
        public List<SelectListItem> qmTypeList { get; set; }

        public string FROM_DATE { get; set; }
        public string TO_DATE { get; set; }

        [Display(Name = "Regrade")]
        [RegularExpression(@"^([0AR]+)$", ErrorMessage = "Invalid Regrade selected")]
        public string Regrade { get; set; }
        public List<SelectListItem> RegradeList { get; set; }

        [Display(Name = "Grade")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Grade.")]
        public int RegradeATRGrade { get; set; }
        public List<SelectListItem> RegradeATRGradeList { get; set; }

        [Display(Name = "Road Status")]
        [RegularExpression(@"^([0CP]+)$", ErrorMessage = "Invalid Road Status selected")]
        public string RoadStatus { get; set; }
        public List<SelectListItem> RoadStatusList { get; set; }

        public string Status { get; set; }

        public int Monitor { get; set; }


        [Required(ErrorMessage = "Agency required")]
        [Range(0, int.MaxValue, ErrorMessage = "Invalid Agency")]
        [Display(Name = "Agency")]
        public int Agency { get; set; }
        public List<SelectListItem> AgencyList { get; set; }

        public string AgencyName { get; set; }
    }



    public class DateValidationVSTAttribute : ValidationAttribute, IClientValidatable
    {

        // private const string _defaultErrorMessage = "Start date must be less than end date.";
        private string _FrmMonth;
        private string _FrmYear;
        private string _ToMonth;

        public DateValidationVSTAttribute(string FrmYear, string FrmMonth, string ToMonth)
        //: base(_defaultErrorMessage)
        {
            _FrmMonth = FrmMonth;
            _FrmYear = FrmYear;
            _ToMonth = ToMonth;

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

    public class BlockRoad
    {

        public string RoadId { get; set; }
        public string RoadName { get; set; }
        public string Package { get; set; }
        public string SYear { get; set; }
        public Decimal RoadLength { get; set; }
        public string RoadStatus { get; set; }
        public string StipulatedCompletedDate { get; set; }
        public string ColorCode { get; set; }
        public List<RoadGeoPosition> GeoPosition { get; set; }

    }
    public class RoadGeoPosition
    {
        public string monitor { get; set; }
        public string Grade { get; set; }
        public string InspDate { get; set; }
        public string RoadStatus { get; set; }
        public string Description { get; set; }
        public Decimal Lattitude { get; set; }
        public Decimal Longitude { get; set; }
        public string PhotoURL { get; set; }
        public string PhotoURLThumb { get; set; }

    }

    public class BlockModel
    {
        [Display(Name = "State")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select valid State.")]
        public int State { get; set; }
        public List<SelectListItem> StateList { get; set; }

        [Display(Name = "District")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select valid District.")]
        public int District { get; set; }
        public List<SelectListItem> DistrictList { get; set; }

        [Display(Name = "Block")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select valid Block.")]
        public int Block { get; set; }
        public List<SelectListItem> BlockList { get; set; }

    }


    public class InProgressWorkViewModel
    {
        public string ValueType { get; set; }

        public int Grade { get; set; }

        public string StateName { get; set; }
        public string DistName { get; set; }
        public string BlockName { get; set; }

        public string RoadStat { get; set; }
        public string FromMonthName { get; set; }
        public string ToMonthName { get; set; }
        public string FromYearName { get; set; }
        public string ToYearName { get; set; }

        [Display(Name = "From Month")]
        [Range(0, 12, ErrorMessage = "Please select valid From Month.")]
        public int FromMonth { get; set; }
        public List<SelectListItem> FromMonthList { get; set; }

        [Display(Name = "Level")]
        [Range(0, 3, ErrorMessage = "Please select Level.")]
        public int Level { get; set; }
        public List<SelectListItem> LevelList { get; set; }


        [Display(Name = "To Month")]
        [Range(0, 12, ErrorMessage = "Please select valid To Month.")]
        public int ToMonth { get; set; }
        public List<SelectListItem> ToMonthList { get; set; }


        [Display(Name = "From Year")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid From Year.")]
        public int FromYear { get; set; }
        public List<SelectListItem> FromYearList { get; set; }

        [Display(Name = "To Year")]
        [DateValidationVST("FromYear", "FromMonth", "ToMonth", ErrorMessage = "To Year must be greater than or equal to From Year.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid To Year.")]
        public int ToYear { get; set; }
        public List<SelectListItem> ToYearList { get; set; }

        [Display(Name = "State")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid State.")]
        public int State { get; set; }
        public List<SelectListItem> StateList { get; set; }

        [Display(Name = "District")]
        //[Required(ErrorMessage = "Please select a District.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid District.")]
        public int District { get; set; }
        public List<SelectListItem> DistrictList { get; set; }

        [Display(Name = "Block")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Block.")]
        public int Block { get; set; }
        public List<SelectListItem> BlockList { get; set; }

        [Display(Name = "Year")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Year.")]
        public int Year { get; set; }
        public List<SelectListItem> YearList { get; set; }

        [Display(Name = "Month")]
        [Range(0, 12, ErrorMessage = "Please select valid Month.")]
        public int Month { get; set; }
        public List<SelectListItem> MonthList { get; set; }

        public string FROM_DATE { get; set; }
        public string TO_DATE { get; set; }



        public string schemeType { get; set; }

        public List<SelectListItem> schemeTypeList { get; set; }


        public string Status { get; set; }

        public int Monitor { get; set; }

    }

    public class DRRPReportModel
    {
        public string StateName { get; set; }
        public string DistName { get; set; }
        public string BlockName { get; set; }
        [Display(Name = "State")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select valid State.")]
        public int States { get; set; }
        public List<SelectListItem> StatesList { get; set; }

        [Display(Name = "District")]
        // [Range(1, int.MaxValue, ErrorMessage = "Please select valid District.")]
        public int Districts { get; set; }
        public List<SelectListItem> DistrictsList { get; set; }

        [Display(Name = "Block")]
        //[Range(1, int.MaxValue, ErrorMessage = "Please select valid Block.")]
        public int Blocks { get; set; }
        public List<SelectListItem> BlocksList { get; set; }

    }

    public class DRRPSurfaceWiseReportModel
    {
        public string StateName { get; set; }
        public string DistName { get; set; }
        public string BlockName { get; set; }
        [Display(Name = "State")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select valid State.")]
        public int States { get; set; }
        public List<SelectListItem> StatesList { get; set; }

        [Display(Name = "District")]
        // [Range(1, int.MaxValue, ErrorMessage = "Please select valid District.")]
        public int Districts { get; set; }
        public List<SelectListItem> DistrictsList { get; set; }

        [Display(Name = "Block")]
        //[Range(1, int.MaxValue, ErrorMessage = "Please select valid Block.")]
        public int PMGSY { get; set; }
        public List<SelectListItem> BlocksList { get; set; }

    }

    public class WorkNotInspectedByNQMModel
    {
        public string StateName { get; set; }

        [Display(Name = "State")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select valid State.")]
        public int States { get; set; }
        public List<SelectListItem> StatesList { get; set; }
    }
}