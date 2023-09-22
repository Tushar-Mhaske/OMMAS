using PMGSY.BLL.Common;
using PMGSY.Common;
using PMGSY.Views.Shared.App_LocalResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.AuthorizedSignatory
{
    public class AuthorizedSignatoryModel
    {

        public AuthorizedSignatoryModel()
        {
            CommonFunctions objCommon = new CommonFunctions();
            this.CURRENT_DATE = objCommon.GetDateTimeToString(DateTime.Now);
        }
        //added by pp

        public string IsDscDetailChanged { get; set; }

        public int STATE_CODE { get; set; }
        public int AGENCY_CODE { get; set; }
         
        public string ADMIN_FULL_NAME { get; set; }
        public string ADMIN_ND_NAME { get; set; }

        public int ND_CODE { get; set; }

        public int? ADMIN_NO_OFFICER_CODE { get; set; }

     
       // [RegularExpression(@"[a-zA-z]{1,25}", ErrorMessage = "Invalid  First Name")]
        [RegularExpression(@"^([a-zA-Z0-9.,() ]+){1,100}$", ErrorMessage = "Invalid  First Name .")]
        [Required(ErrorMessage = "First Name is Required")]
        [Display(Name = "First Name")]
        public string ADMIN_FNAME { get; set; }

        
        //[Display(Name = "lblmiddleName", ResourceType = typeof(authorizedSignatory))]
        [Display(Name = "Middle Name")]
        //[RegularExpression(@"[a-zA-z]{1,25}", ErrorMessage = "Invalid  Middle  Name")]
        [RegularExpression(@"^([a-zA-Z0-9.,() ]+){1,100}$", ErrorMessage = "Invalid  Middle Name .")]
            //[RegularExpression(@"[a-zA-z]{1,25}", ErrorMessageResourceName = "Invalid", ErrorMessageResourceType = typeof(authorizedSignatory))]
        public string ADMIN_MNAME { get; set; }

        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(authorizedSignatory))]
        // [Required(ErrorMessage = "Last Name is Required")]
       // [Display(Name = "lblLastName", ResourceType = typeof(authorizedSignatory))]
         [Display(Name = "Last Name")]
        //[RegularExpression(@"[a-zA-z]{1,25}", ErrorMessageResourceName = "Invalid", ErrorMessageResourceType = typeof(authorizedSignatory))]
        //[RegularExpression(@"[a-zA-z]{1,25}", ErrorMessage = "Invalid  Last Name")]
         [RegularExpression(@"^([a-zA-Z0-9.,() ]+){1,100}$", ErrorMessage = "Invalid  Last Name .")]
             public string ADMIN_LNAME { get; set; }


        //[Display(Name = "lblDesignation", ResourceType = typeof(authorizedSignatory))]
        //[Range(1, int.MaxValue, ErrorMessage = "Invalid Designation")]
       
        [Display(Name = "Designation")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Designation")]
        public int ADMIN_NO_DESIGNATION { get; set; }

        [StringLength(255)]
       // [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(authorizedSignatory))]
       // [Display(Name = "lblAddress1", ResourceType = typeof(authorizedSignatory))]
       // [RegularExpression(@"[^\n]+(?=[0-9a-zA-Z\\/\\s]{0,255})", ErrorMessageResourceName = "Invalid", ErrorMessageResourceType = typeof(authorizedSignatory))]
       
        [Required(ErrorMessage = "Address1  is Required")]
        [Display(Name = "Address 1")]
        [RegularExpression(@"[^\n]+(?=[0-9a-zA-Z\\/\\s]{0,255})", ErrorMessage = "Invalid  Address1 ")]
        public string ADMIN_ADDRESS1 { get; set; }

        
        [StringLength(255)]
        //[Display(Name = "lblAddress2", ResourceType = typeof(authorizedSignatory))]
        //[RegularExpression(@"[^\n]+(?=[0-9a-zA-Z\\/\\s]{0,255})", ErrorMessageResourceName = "Invalid", ErrorMessageResourceType = typeof(authorizedSignatory))]
        [Display(Name = "Address 2")]
        [RegularExpression(@"[^\n]+(?=[0-9a-zA-Z\\/\\s]{0,255})", ErrorMessage = "Invalid  Address2 ")]
        public string ADMIN_ADDRESS2 { get; set; }


        //[Display(Name = "lblDistrict", ResourceType = typeof(authorizedSignatory))]
        [Display(Name = "District")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select district")]
        public Nullable<int> MAST_DISTRICT_CODE { get; set; }


        //[RegularExpression(@"\d{6}", ErrorMessageResourceName = "Invalid", ErrorMessageResourceType = typeof(authorizedSignatory))]
        //[Display(Name = "lblPinCode", ResourceType = typeof(authorizedSignatory))]

        [RegularExpression(@"\d{6}", ErrorMessage = "Invalid Pin")]
        [Display(Name = "Pin Code")]
        public string ADMIN_PIN { get; set; }


        //[Display(Name = "lblResSTDNum", ResourceType = typeof(authorizedSignatory))]
        //[RegularExpression(@"[0-9]{3,6}", ErrorMessageResourceName = "Invalid", ErrorMessageResourceType = typeof(authorizedSignatory))]

        [Display(Name = "Residence STD Number")]
        [RegularExpression(@"[0-9]{3,6}", ErrorMessage = "Invalid Residence STD Number")]
        public string ADMIN_RESIDENCE_STD { get; set; }


        //[RegularExpression(@"[0-9]{6,8}", ErrorMessageResourceName = "Invalid", ErrorMessageResourceType = typeof(authorizedSignatory))]
        //[Display(Name = "lblResPhNum", ResourceType = typeof(authorizedSignatory))]
        [RegularExpression(@"[0-9]{6,8}", ErrorMessage = "Invalid Residence Phone Number")]
        [Display(Name = "Residence Phone Number")]
        public string ADMIN_RESIDENCE_PHONE { get; set; }


        //[RegularExpression(@"[0-9]{3,6}", ErrorMessageResourceName = "Invalid", ErrorMessageResourceType = typeof(authorizedSignatory))]
        //[Display(Name = "lblOffSTDNum", ResourceType = typeof(authorizedSignatory))]

        [RegularExpression(@"[0-9]{3,6}", ErrorMessage = "Invalid Office STD Number")]
        [Display(Name = "Office STD Number")]
        public string ADMIN_OFFICE_STD { get; set; }

    
        //[RegularExpression(@"[0-9]{6,8}", ErrorMessageResourceName = "Invalid", ErrorMessageResourceType = typeof(authorizedSignatory))]
        //[Display(Name = "lblOfficephNumNumber", ResourceType = typeof(authorizedSignatory))]

        [RegularExpression(@"[0-9]{6,8}", ErrorMessage = "Invalid Office Phone Number")]
        [Display(Name = "Office Phone Number")]
        public string ADMIN_OFFICE_PHONE { get; set; }


        //[RegularExpression(@"[0-9]{3,6}", ErrorMessageResourceName = "Invalid", ErrorMessageResourceType = typeof(authorizedSignatory))]
        //[Display(Name = "lblFaxStdNumber", ResourceType = typeof(authorizedSignatory))]
        [RegularExpression(@"[0-9]{3,6}", ErrorMessage = "Invalid Fax STD Number")]
        [Display(Name = "Fax STD Number")]
        public string ADMIN_STD_FAX { get; set; }



        //[RegularExpression(@"^([0-9]{6,8})$", ErrorMessageResourceName = "Invalid", ErrorMessageResourceType = typeof(authorizedSignatory))]
        //[Display(Name = "lblFaxNumber", ResourceType = typeof(authorizedSignatory))]

        [RegularExpression(@"^([0-9]{6,8})$", ErrorMessage = "Invalid Fax Number")]
        [Display(Name = "Fax Number")]
        public string ADMIN_FAX { get; set; }


        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(authorizedSignatory))]
        //[RegularExpression(@"\d{10,11}", ErrorMessageResourceName = "Invalid", ErrorMessageResourceType = typeof(authorizedSignatory))]
        //[Display(Name = "lblMobileNumber", ResourceType = typeof(authorizedSignatory))]

        [Required(ErrorMessage =  "Required Mobile Number")]
        [RegularExpression(@"\d{10,11}", ErrorMessage = "Invalid Mobile Number")]
        [Display(Name = "Mobile Number")]
        public string ADMIN_MOBILE { get; set; }

        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(authorizedSignatory))]
        //[EmailAddress(ErrorMessageResourceName = "Invalid", ErrorMessageResourceType = typeof(authorizedSignatory), ErrorMessage = null)]
        //[Display(Name = "lblEmail", ResourceType = typeof(authorizedSignatory))]
        [Required(ErrorMessage = "Required Email")]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        [Display(Name = "Email")]
        public string ADMIN_EMAIL { get; set; }

        public string ADMIN_ACTIVE_STATUS { get; set; }

        public int ADMIN_OFFICER_CODE { get; set; }

        //[Display(Name = "lblStartDate", ResourceType = typeof(authorizedSignatory))]
        [Display(Name = "Start Date")]
        //[DataType(DataType.Date, ErrorMessageResourceName = "Invalid", ErrorMessageResourceType = typeof(authorizedSignatory))]
        [DataType(DataType.Date, ErrorMessage = "Invalid Start Date" )]
        [Required(ErrorMessage = "Start Date is required")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Please Enter valid Start Date")]
        [IsDateAfter("CURRENT_DATE", true, ErrorMessage = "Start Date must be less than or equal to today's date")]
        public string START_DATE { get; set; }
        public string startDate { get; set; }

        //[Display(Name = "lblEndDate", ResourceType = typeof(authorizedSignatory))]
       [Display(Name = "End Date")]
        [DataType(DataType.Date, ErrorMessageResourceName = "Invalid", ErrorMessageResourceType = typeof(authorizedSignatory))]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Please Enter valid End Date")]
        [DateGreaterThanValidation("START_DATE", true, ErrorMessage = "End Date must be greater than Start Date")]
        [IsDateAfter("CURRENT_DATE", true, ErrorMessage = "End Date must be less than or equal to today's date")]
        public string END_DATE { get; set; }
        public string EndDate { get; set; }


       // [Display(Name = "lblRemarks", ResourceType = typeof(authorizedSignatory))]
        [Display(Name = "Remarks")]
        //[RegularExpression(@"[A-Za-z ][A-Za-z0-9 ,._@()/- ]{3,255}", ErrorMessageResourceName = "Invalid", ErrorMessageResourceType = typeof(authorizedSignatory))]
        [RegularExpression(@"^([a-zA-Z0-9 ./,()-]+)$", ErrorMessage = "Only Alphanumeric, Space and '-','/','.','(',')',',' Allowed")] //added some special characters by koustubh nakate on 18/07/2013 
        public String ADMIN_REMARKS { get; set; }


        public string CURRENT_DATE { get; set; }

        //Added by Abhishek kamble 15Apr2015
        [Display(Name="Aadhaar Number")]
        [Required(ErrorMessage="Aadhaar Number is required.")]                                                    
        //[RegularExpression(@"\d{12,12}", ErrorMessage = "Only Digits are allowed.")]                    
        [RegularExpression(@"([0-9]+)$", ErrorMessage = "Invalid Aadhaar Number.Only Digits are allowed.")]
        [StringLength(12, MinimumLength = 12, ErrorMessage = "Aadhaar Number must be 12 digits.")]                
        public string ADMIN_AADHAR_NO { get; set; }

        [RegularExpression("[AP]",ErrorMessage="Please select Aadhar / Pan Number")]
        [Required(ErrorMessage = "Please select Aadhar / Pan Number")]                                                            
        public string ADMIN_AADHAR_PAN_FLAG { get; set; }

        [Required(ErrorMessage="Pan Number is required.")]
        [StringLength(10,MinimumLength=10,ErrorMessage="Pan Number must be 10 alphanumeric characters only.")]
        [RegularExpression("^([a-zA-Z]){5}([0-9]){4}([a-zA-Z]){1}?$", ErrorMessage = "PAN is not in valid format.")]
        public string ADMIN_PAN_NO { get; set; }


        public string DSCRegistered { get; set; }
      
    }

    /// <summary>
    /// custom validation attribute to comapaire two dates
    /// </summary>
    public class DateGreaterThanValidation : ValidationAttribute, IClientValidatable
    {
        private readonly string testedPropertyName;
        private readonly bool allowEqualDates;
        CommonFunctions objCommon = new CommonFunctions();
        public DateGreaterThanValidation(string testedPropertyName, bool allowEqualDates = false)
        {
            this.testedPropertyName = testedPropertyName;
            this.allowEqualDates = allowEqualDates;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyTestedInfo = validationContext.ObjectType.GetProperty(this.testedPropertyName);
            if (propertyTestedInfo == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.testedPropertyName));
            }

            var propertyTestedValue = propertyTestedInfo.GetValue(validationContext.ObjectInstance, null);

            if (value != null)
            {
                // Compare values
                if (objCommon.GetStringToDateTime(value.ToString()) >= objCommon.GetStringToDateTime(propertyTestedValue.ToString()))
                {
                    if (this.allowEqualDates)
                    {
                        return ValidationResult.Success;
                    }
                    if ((DateTime)value < (DateTime)propertyTestedValue)
                    {
                        return ValidationResult.Success;
                    }
                }
                else
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
            }

            if (value == null || !(value is DateTime))
            {
                return ValidationResult.Success;
            }



            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "dategreaterthanvalidation"
            };
            rule.ValidationParameters["propertytested"] = this.testedPropertyName;
            rule.ValidationParameters["allowequaldates"] = this.allowEqualDates;
            yield return rule;
        }

    }


}