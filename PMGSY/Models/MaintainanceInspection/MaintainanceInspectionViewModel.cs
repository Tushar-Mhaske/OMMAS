using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.MaintainanceInspection
{
    public class MaintainanceInspectionViewModel
    {
        [UIHint("Hidden")]
        public string EncryptedIMSPRRoadCode { get; set; }

        [UIHint("Hidden")]
        public string EncryptedPRRoadCode { get; set; }
        public int IMS_PR_ROAD_CODE { get; set; }
        public int IMS_INSPECTION_CODE { get; set; }

        [Display(Name="Name")]
        [Required(ErrorMessage = "Select name.")]
        [Range(1, 2147483647, ErrorMessage = "Select name.")]
        public int MAST_OFFICER_CODE { get; set; }

        [Required]
        [Display(Name="Inspection Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Inspection date must be in dd/mm/yyyy format.")]
        public string MANE_INSP_DATE { get; set; }

        [Display(Name="Rectification Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Rectification date must be in dd/mm/yyyy format.")]
        [DateValidationVST("MANE_INSP_DATE", ErrorMessage = "Rectification Date must be greater than or equal to inspection date.")]
        [RequredRectificationDate("statusFlag", ErrorMessage = "Please enter rectification date")]
        public string MANE_RECTIFICATION_DATE { get; set; }
    
        public string MANE_RECTIFICATION_STATUS { get; set; }

        public string SactionedYear { get; set; }
        public string RoadName { get; set; }
        public string Package { get; set; }

        [Display(Name="Remark")]
        [RegularExpression(@"^([a-zA-Z0-9 ._',\r\n&()!#$%&*_:;?/\|-]+)$", ErrorMessage = "Remark is not in valid format.")]
        [StringLength(255,ErrorMessage="Remark field should not be greater than 255 characters.")]
        public string MANE_REMARKS { get; set; }

        [Display(Name="Designation")]
        [Required(ErrorMessage="Select designation.")]
        [Range(1, 2147483647, ErrorMessage = "Select designation.")]
        public string Designation { get; set; }

        public string BlockName { get; set; }

        public string InspectionDate { get; set; }

        public string StartDate { get; set; }
        
        //added by abhishek kamble 21-nov-2013
        
        public string statusFlag { get; set; }
             
        public virtual ADMIN_NODAL_OFFICERS ADMIN_NODAL_OFFICERS { get; set; }
        public virtual IMS_SANCTIONED_PROJECTS IMS_SANCTIONED_PROJECTS { get; set; }
    }


    public class RequredRectificationDateAttribute : ValidationAttribute, IClientValidatable
    {
        // private const string _defaultErrorMessage = "Start date must be less than end date.";
        private string _basePropertyName;

        public RequredRectificationDateAttribute(string basePropertyName)
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

            var IsRectificationDate = basePropertyInfo.GetValue(validationContext.ObjectInstance, null);
            var rectificationDate = value;

            //if (sDate != null && eDate != null)
            //{

            //Actual Validation 

            if (IsRectificationDate != null)
            {
                if ((IsRectificationDate.ToString() == "true") && (rectificationDate == null))
                {
                    var message = FormatErrorMessage(validationContext.DisplayName);
                    return new ValidationResult(message);
                }
                else
                {
                }
            }

            return ValidationResult.Success;
        }


        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {

            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                ValidationType = "rectificationdatevalidator"
            };
            rule.ValidationParameters["date"] = this._basePropertyName;
            yield return rule;

        }

        //public DateTime? ConvertStringToDate(string dateToConvert)
        //{
        //    DateTime MyDateTime;
        //    MyDateTime = new DateTime();
        //    // MyDateTime = DateTime.Parse(dateToConvert);
        //    MyDateTime = DateTime.ParseExact(dateToConvert, "dd/MM/yyyy", null);
        //    //Convert.ToDateTime(dateToConvert);         //DateTime.ParseExact(dateToConver, "dd/MM/yyyy",null);
        //    return MyDateTime;
        //}

    }
}