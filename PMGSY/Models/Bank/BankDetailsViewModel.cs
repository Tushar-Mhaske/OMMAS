using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Bank
{
    public class BankDetailsViewModel
    {

        public BankDetailsViewModel()
        {
            STATE = new List<SelectListItem>();
            DISTRICT = new List<SelectListItem>();
        }

        public string EncryptedBankCode { get; set; }
        public string Operation { get; set; }
        public short BANK_CODE { get; set; }
        public int ADMIN_ND_CODE { get; set; }
        public string FUND_TYPE { get; set; }

        
        public string OldCloseDate { get; set; }

        // Added by Srishti 
        [Required(ErrorMessage = "Please select Account Type.")]
        [RegularExpression(@"^[SHD]?", ErrorMessage = "Please select valid Account Type.")]
        //[RegularExpression(@"^([SHD]$", ErrorMessage = "Please select valid Account Type.")]
        [Display(Name = "Account Type :")]
        public string BANK_ACC_TYPE { get; set; }
        public List<SelectListItem> lstBankAccType { get; set; }

        [Display(Name = "Account Holder Name :")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Please enter valid Name.")]
        [StringLength(100, ErrorMessage = "Account Holder Name cannot exceed by 100 characters.")]
        public string ACCOUNT_HOLDER_NAME { get; set; }


        [Required(ErrorMessage="Please enter Bank Account No.")]
        [RegularExpression(@"^([0-9]+){1,30}$", ErrorMessage = "Please enter valid account no.")]
        [Display(Name="Bank Account No.")]
        public string BANK_ACC_NO { get; set; }

        [Required(ErrorMessage="Please select state.")]
        [Display(Name="State")]
        [Range(1,40,ErrorMessage="Please select state.")]
        public int MAST_STATE_CODE { get; set; }
        
        [Required(ErrorMessage="Please enter Bank Name.")]
        [RegularExpression(@"^([a-zA-Z&,;'()-. ]+){1,50}$", ErrorMessage = "Please enter valid bank name.")]
        [Display(Name="Bank Name")]
        public string BANK_NAME { get; set; }
        public List<SelectListItem> lstBankNames { get; set; }

        [Required(ErrorMessage="Branch Name is required.")]
        [Display(Name="Branch Name")]
        [RegularExpression(@"^([a-zA-Z0-9.,() ]+){1,100}$", ErrorMessage = "Please enter valid branch name.")]
        public string BANK_BRANCH { get; set; }

        [Required(ErrorMessage="Address 1 is required.")]
        [Display(Name="Address 1")]
        [RegularExpression(@"^([a-zA-Z0-9\r\n,-.() /]+){1,150}$", ErrorMessage = "Please enter valid address.")]
        public string BANK_ADDRESS1 { get; set; }

        [Display(Name="Address 2")]
        [RegularExpression(@"^([a-zA-Z0-9\r\n,-.() /\r\n]+){1,150}$", ErrorMessage = "Please enter valid address.")]
        public string BANK_ADDRESS2 { get; set; }

        [Display(Name="Pin Code")]
        [RegularExpression(@"^([0-9]+){6,6}$", ErrorMessage = "Please enter valid pin no.")]
        //[Range(6,6,ErrorMessage="Please enter 6 digit Pin no.")]
        public string BANK_PIN { get; set; }

        [Required(ErrorMessage="STD Code is required.")]
        [Display(Name="Phone 1")]
        [RegularExpression(@"^([0-9]+){3,5}$", ErrorMessage = "Please Enter Valid STD No.")]
        public string BANK_STD1 { get; set; }

        [Required(ErrorMessage = "Phone 1 is required.")]
        [Display(Name="-")]
        [RegularExpression(@"^([0-9]+){6,8}$", ErrorMessage = "Please Enter Valid Phone No.")]
        public string BANK_PHONE1 { get; set; }

        [Display(Name="Phone 2")]
        [RegularExpression(@"^([0-9]+){3,5}$", ErrorMessage = "Please Enter Valid STD No.")]
        public string BANK_STD2 { get; set; }

        [Display(Name="-")]
        [RegularExpression(@"^([0-9]+){6,8}$", ErrorMessage = "Please Enter Valid Phone No.")]
        public string BANK_PHONE2 { get; set; }

        [Display(Name="Fax")]
        [RegularExpression(@"^([0-9]+){3,6}$", ErrorMessage = "Please enter valid fax no.")]
        public string BANK_STD_FAX { get; set; }
        
        [Display(Name="-")]
        [RegularExpression(@"^([0-9]+){6,8}$", ErrorMessage = "Please enter valid fax no.")]
        public string BANK_FAX { get; set; }
        
        [Required(ErrorMessage="Email is required.")]
        [Display(Name="Email")]
        [EmailAddress(ErrorMessage="Please enter valid email address.")]
        public string BANK_EMAIL { get; set; }

        [Display(Name="Remarks")]
        [RegularExpression(@"^([a-zA-Z0-9\r\n-,./\\() ]+){1,255}$", ErrorMessage = "Please enter valid remarks.")]
        public string BANK_REMARKS { get; set; }

        [Required(ErrorMessage="Account Opening Date is required.")]
        [Display(Name="Account Open Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Please Enter valid Account Open date")]
        [AgreementDateValidation("BANK_AGREEMENT_DATE", ErrorMessage = "Agreement date must be less than account opening date.")]
        [CompareOldDate("OldCloseDate",ErrorMessage="Account Opening Date must be greater than old Account Closing Date.")]
        [CompareOpeningDate(ErrorMessage="Account Opening Date must be less than or equal to current date.")]
        public string BANK_ACC_OPEN_DATE { get; set; }

        [Display(Name="Account Close Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Please Enter valid Account Close date")]
        [DateValidation("BANK_ACC_OPEN_DATE", ErrorMessage = "Account close date must be greater than Account Opening date.")]
        [CompareOldDate("OldCloseDate",ErrorMessage="Account close date must be greater than previous account close date.")]
        //[CompareOpeningDate(ErrorMessage = "Account Closing Date must be less than or equal to current date.")]
        public string BANK_ACC_CLOSE_DATE { get; set; }

        public Nullable<bool> BANK_ACC_STATUS { get; set; }

        [Display(Name="District")]
        [RegularExpression(@"^([a-zA-Z0-9,.()]+){1,11}$", ErrorMessage = "Please select valid district.")]
        public Nullable<int> MAST_DISTRICT_CODE { get; set; }

        [Display(Name="Agreement Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Please Enter valid Agreement date")]
        public string BANK_AGREEMENT_DATE { get; set; }

        [Display(Name="Secondary Code")]
        [RegularExpression(@"^([a-zA-Z0-9,.()]+){1,11}$", ErrorMessage = "Please enter valid secondary code.")]
        public string Bank_SEC_CODE { get; set; }

        public List<SelectListItem> STATE { get; set; }

        public List<SelectListItem> DISTRICT { get; set; }

        public string DistrictName { get; set; }

        //[RegularExpression(@"^([A-Z|a-z]{4}[0][\d]{6})$", ErrorMessage = "IFSC Code is not in valid format.")]
        [Required(ErrorMessage = "IFSC Code must be entered")]
        [RegularExpression(@"^([A-Z|a-z]{4}[0][A-Z|a-z|0-9]{6})$", ErrorMessage = "IFSC Code is not in valid format.")]
        [StringLength(11, ErrorMessage = "IFSC Code must be 11 characters only.")]
        public string MAST_IFSC_CODE { get; set; }
        public List<SelectListItem> lstIfscCodes { get; set; }

        public string pfmsErrorMessage { get; set; }
    }


    public class DateValidationAttribute : ValidationAttribute,IClientValidatable
    {

        private string _defaultErrorMessage = "Account closing date must be greater than account opening date";
        private string _basePropertyName;

        public DateValidationAttribute(string basePropertyName)           
        {
            _basePropertyName = basePropertyName;
        }

        //Override default FormatErrorMessage Method  
        public override string FormatErrorMessage(string name)
        {
            return string.Format(_defaultErrorMessage, name, _basePropertyName);
        }

        //Override IsValid  
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                //Get PropertyInfo Object  
                var basePropertyInfo = validationContext.ObjectType.GetProperty(_basePropertyName);

                var sDate = basePropertyInfo.GetValue(validationContext.ObjectInstance, null);
                var eDate = value;

                if (sDate != null && eDate != null)
                {
                    var startDate = ConvertStringToDate(sDate.ToString());
                    var thisDate = ConvertStringToDate(eDate.ToString());

                    //Actual comparision  
                    if (thisDate <= startDate)
                    {
                        var message = FormatErrorMessage(validationContext.DisplayName);
                        return new ValidationResult(message);
                    }
                }
                else if (sDate == null)
                {
                    _defaultErrorMessage = "Account opening date must be present to enter closing date";
                    var message = FormatErrorMessage(validationContext.DisplayName);
                    return new ValidationResult(message);
                }
                return ValidationResult.Success;
            }
            catch (Exception ex)
            {
               return  null;
            }

            //Default return - This means there were no validation error  
            
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "comparedate"
            };
            rule.ValidationParameters["startdate"] = this._basePropertyName;
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


    public class AgreementDateValidationAttribute : ValidationAttribute, IClientValidatable
    {

        private string _defaultErrorMessage = "Account Opening Date must be greater than Agreement Date.";
        private string _basePropertyName;

        public AgreementDateValidationAttribute(string basePropertyName)
        {
            _basePropertyName = basePropertyName;
        }

        //Override default FormatErrorMessage Method  
        public override string FormatErrorMessage(string name)
        {
            return string.Format(_defaultErrorMessage, name, _basePropertyName);
        }

        //Override IsValid  
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                //Get PropertyInfo Object  
                var basePropertyInfo = validationContext.ObjectType.GetProperty(_basePropertyName);

                var sDate = basePropertyInfo.GetValue(validationContext.ObjectInstance, null);
                var eDate = value;

                if (sDate != null && eDate != null)
                {
                    var startDate = ConvertStringToDate(sDate.ToString());
                    var thisDate = ConvertStringToDate(eDate.ToString());

                    //Actual comparision  
                    if (thisDate <= startDate)
                    {
                        var message = FormatErrorMessage(validationContext.DisplayName);
                        return new ValidationResult(message);
                    }
                }
                else if (sDate == null)
                {
                    return ValidationResult.Success;
                }
                return ValidationResult.Success;
            }
            catch (Exception ex)
            {
                return null;
            }

            //Default return - This means there were no validation error  

        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "compareagreementdate"
            };
            rule.ValidationParameters["startdate"] = this._basePropertyName;
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



    public class CompareOldDate : ValidationAttribute, IClientValidatable
    {
        private readonly string PropertyName;

        public CompareOldDate(string propertyName)
        {
            this.PropertyName = propertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                var propertyTestedInfo = validationContext.ObjectType.GetProperty(this.PropertyName);
                if (propertyTestedInfo == null)
                {
                    return new ValidationResult(string.Format("unknown property {0}", this.PropertyName));
                }

                var oldDate = propertyTestedInfo.GetValue(validationContext.ObjectInstance, null);
                var newDate = value;

                if (oldDate == null)
                {
                    return ValidationResult.Success;
                }

                //added by abhishek kamble
                if (oldDate != null)
                {
                    if (oldDate.ToString() == "1/1/0001")
                    {
                        return ValidationResult.Success;
                    }
                }

                if (oldDate != null && newDate != null)
                {
                    var oDate = ConvertStringToDate(oldDate.ToString());
                    var nDate = ConvertStringToDate(newDate.ToString());
                    if (nDate < oDate)
                    {
                        return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                    }
                }
                return ValidationResult.Success;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public DateTime? ConvertStringToDate(string dateToConvert)
        {
            DateTime MyDateTime;
            MyDateTime = new DateTime();
            MyDateTime = DateTime.ParseExact(dateToConvert, "dd/MM/yyyy", null);
            return MyDateTime;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "compareolddate"
            };
            rule.ValidationParameters["oldclosedate"] = this.PropertyName;
            yield return rule;
        }
    }


    public class CompareOpeningDate : ValidationAttribute//, IClientValidatable
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                //var openingDate = Convert.ToDateTime(value);
                //added by abhishek kamble     
                System.DateTime? openingDate = null;
                if (value != null)
                {
                    openingDate = ConvertStringToDate(value.ToString());
                }

                if (openingDate == null)
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }

                if (openingDate <= System.DateTime.Now)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public DateTime? ConvertStringToDate(string dateToConvert)
        {
            DateTime MyDateTime;
            MyDateTime = new DateTime();
            MyDateTime = DateTime.ParseExact(dateToConvert, "dd/MM/yyyy", null);
            return MyDateTime;
        }
    }

}