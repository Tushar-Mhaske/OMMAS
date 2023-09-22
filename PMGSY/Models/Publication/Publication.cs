using PMGSY.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Publication
{
    public class PublicationCategory
    {
        public int publicationCategoryCode { get; set; }
        public string publicationName { get; set; }
    }


    public class PublicationViewModel
    {
        public PublicationViewModel()
        {
            CommonFunctions commonFunctions = new CommonFunctions();
            YearList = commonFunctions.PopulateYears(true);
            MonthList = commonFunctions.PopulateMonths(true);
            Date_Type_List = new List<SelectListItem>();
            Date_Type_List.Add(new SelectListItem { Text = "Select Type", Value = "0" });
            Date_Type_List.Add(new SelectListItem { Text = "Date", Value = "D" });
            Date_Type_List.Add(new SelectListItem { Text = "Year", Value = "Y" });
            Date_Type_List.Add(new SelectListItem { Text = "Month", Value = "M" });
        }

        [StringLength(1)]
        public string Action { get; set; }

        [Range(0, 2147483647, ErrorMessage = "Please select Publication.")]
        public int publicationCode { get; set; }

        [Display(Name = "Period")]
        [RegularExpression(@"^[DYM]", ErrorMessage = "Please select Period.")]
        [Required(ErrorMessage = "Please select Period.")]
        [RequireFieldPublicationValidator("publicationDate", "Year", "Month", ErrorMessage = "Please select Period.")]
        public string Date_Type { get; set; }
        public List<SelectListItem> Date_Type_List { get; set; }


        [Display(Name = "Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Publication Date must be in dd/mm/yyyy format.")]
        public string publicationDate { get; set; }

        [Display(Name = "Year")]
        [Range(0, 2090, ErrorMessage = "Please select Year.")]
        public int Year { get; set; }
        public List<SelectListItem> YearList { get; set; }

        [Display(Name = "Month")]
        [Range(0, 12, ErrorMessage = "Please select Month.")]
        public int Month { get; set; }
        public List<SelectListItem> MonthList { get; set; }

        [Display(Name = "Title")]
        [Required(ErrorMessage = "Please enter Title.")]
        [StringLength(255, ErrorMessage = "Title is not greater than 255 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9_ \-]*$", ErrorMessage = "Title is not in valid format.")]
        public string publicationTitle { get; set; }

        [Display(Name = "Description")]
        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500, ErrorMessage = "Description is not greater than 500 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9_ ]*$", ErrorMessage = "Description is not in valid format.")]
        public string publicationDescription { get; set; }

        [Display(Name = "Author Name")]
        [Required(ErrorMessage = "Please enter Author.")]
        [StringLength(500, ErrorMessage = "Author is not greater than 500 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9_ \-]*$", ErrorMessage = "Author is not in valid format.")]
        public string publicationAuther { get; set; }



        [Display(Name = "Volume")]
        [Required(ErrorMessage = "Volume is required.")]
        [StringLength(100, ErrorMessage = "Volume is not greater than 100 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9_ \-]*$", ErrorMessage = "Volume is not in valid format.")]
        public string publicationVolume { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name is not greater than 100 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9_ \-]*$", ErrorMessage = "Name is not in valid format.")]
        public string publicationName { get; set; }

        [Display(Name = "Pagination")]
        [Required(ErrorMessage = "Pagination is required.")]
        [StringLength(100, ErrorMessage = "Pagination is not greater than 100 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9_ \-]*$", ErrorMessage = "Pagination is not in valid format.")]
        public string publicationPagination { get; set; }

        public string publicationStatus { get; set; }
        public string publicationFinalized { get; set; }

        [Required(ErrorMessage = "Please select Publication.")]
        [Range(1, 2147483647, ErrorMessage = "Please select Publication.")]
        [Display(Name = "Publication")]
        public int publicationCategoryCode { get; set; }
        public SelectList PublicationCategoryList
        {
            get
            {
                PMGSY.BAL.Publication.IPublicationBAL publicationBAL = new PMGSY.BAL.Publication.PublicationBAL();
                return new SelectList(publicationBAL.GetCategoryListBAL(), "publicationCategoryCode", "publicationName");
            }
            set
            {

            }
        }
    }

    public class PublicationUploadViewModel
    {
        public int? publicationCode { get; set; }
        public int numberofFileUpload { get; set; }
        public string publicationFinalized { get; set; }
        public string pubTitle { get; set; }
        public string pubAuthor { get; set; }
        public string pubDate { get; set; }
        public string pubVolume { get; set; }

        public int publicationFileCode { get; set; }
        public int? NumberofFiles { get; set; }
        public string url { get; set; }
        public string thumbnail_url { get; set; }
        public string publicationName { get; set; }
        public string type { get; set; }
        public int size { get; set; }
        public string delete_url { get; set; }
        public string delete_type { get; set; }
        public string ErrorMessage { get; set; }
    }

    //class for custom validation
    public class RequireFieldPublicationValidator : ValidationAttribute, IClientValidatable
    {
        //public string Value { get; set; }
        private string _basePropertyDate;
        private string _basePropertyYear;
        private string _basePropertyMonth;



        public RequireFieldPublicationValidator(string basePropertyDate, string basePropertyYear, string basePropertyMonth)
        {
            //this.Value = Value;
            this._basePropertyDate = basePropertyDate;
            this._basePropertyYear = basePropertyYear;
            this._basePropertyMonth = basePropertyMonth;

        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, _basePropertyYear);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            var basePropertyDate = validationContext.ObjectType.GetProperty(_basePropertyDate);
            var basePropertyYear = validationContext.ObjectType.GetProperty(_basePropertyYear);
            var basePropertyMonth = validationContext.ObjectType.GetProperty(_basePropertyMonth);

            var propertyDate = basePropertyDate.GetValue(validationContext.ObjectInstance, null); // check value is null or empty
            var propertyYear = basePropertyYear.GetValue(validationContext.ObjectInstance, null); // check value is null or empty
            var propertyMonth = basePropertyMonth.GetValue(validationContext.ObjectInstance, null); // check value is null or empty

            string DateType = (string)value;

            //if (sDate != null && eDate != null)
            //{

            //Actual Validation 
            if ((propertyDate == null) && (DateType.ToString().Trim() == "D"))
            {
                var message = FormatErrorMessage(validationContext.DisplayName);
                return new ValidationResult("Please select Date.");
            }
            if ((propertyYear.ToString().Trim() == "0") && (DateType.ToString().Trim() == "Y"))
            {
                var message = FormatErrorMessage(validationContext.DisplayName);
                return new ValidationResult("Please select Year.");
            }
            if (((propertyMonth.ToString().Trim() == "0") || (propertyYear.ToString().Trim() == "0")) && (DateType.ToString().Trim() == "M"))
            {
                var message = FormatErrorMessage(validationContext.DisplayName);
                return new ValidationResult("Please select Month and Year.");
            }

            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                //This is the name of the method aaded to the jQuery validator method (must be lower case)
                ValidationType = "requirefieldpublicationvalidator"
            };

            rule.ValidationParameters.Add("date", this._basePropertyDate);
            rule.ValidationParameters.Add("month", this._basePropertyMonth);
            rule.ValidationParameters.Add("year", this._basePropertyYear);

            return new[] { rule };
        }
    }//end custom validation
}