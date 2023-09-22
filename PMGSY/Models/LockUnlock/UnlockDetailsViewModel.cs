using PMGSY.Common;
using PMGSY.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.LockUnlock
{
    public class UnlockDetailsViewModel
    {

        public int TransactionNo { get; set; }
        public string UnlockLevel { get; set; }
        public string UnlockTable { get; set; }
        public Nullable<int> StateCode { get; set; }
        public Nullable<int> DistrictCode { get; set; }
        public Nullable<int> BlockCode { get; set; }
        public Nullable<int> VillageCode { get; set; }
        public Nullable<int> HabCode { get; set; }
        public Nullable<int> ExistingRoadCode { get; set; }
        public Nullable<int> CoreNetworkCode { get; set; }
        public Nullable<int> ProposalCode { get; set; }
        public Nullable<int> NITCode { get; set; }
        public Nullable<int> AgreementCode { get; set; }
        public Nullable<int> YearCode { get; set; }
        public Nullable<int> BatchCode { get; set; }
        public Nullable<int> ProposalContractCode { get; set; }
        public Nullable<int> CoreNetworkContractCode { get; set; }
        
        [Required(ErrorMessage="Unlock Start Date is required.")]
        [Display(Name="Unlock Start Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Unlock Start Date must be in dd/mm/yyyy format.")]
        public string UnlockStartDate { get; set; }
        
        [Required(ErrorMessage="Unlock End Date is required.")]
        [Display(Name="Unlock End Date")]
        [DateValidation("UnlockStartDate", ErrorMessage = "Unlock Start Date must be greater than Unlock End Date.")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Unlock End Date must be in dd/mm/yyyy format.")]
        public string UnlockEndDate { get; set; }

        //[RegularExpression(@"^([YN]+)$", ErrorMessage = "Unlock Status is invalid.")]
        public string UnlockStatus { get; set; }

        //[RegularExpression(@"^([MN]+)$", ErrorMessage = "Invalid Value.")]
        public string UnlockBy { get; set; }
        
        [Required(ErrorMessage="Remarks is required.")]
        [Display(Name="Remarks")]
        [RegularExpression(@"^([a-zA-Z0-9 ()]+)$", ErrorMessage = "Enter valid remarks.Only alphanumeric characters with ( ) is allowed.")]
        public string UnlockRemarks { get; set; }

        ///Changes for RCPLWE unlock at ITNO/PMGSY3
        [Range(1, 5, ErrorMessage = "Scheme is invalid.")]
        public byte PMGSYScheme { get; set; }

        [Required(ErrorMessage="Please select Role to Unlock")]
        public int UnlockRoleCode { get; set; }

        public List<SelectListItem> lstRoles { get; set; }


        public int[] dataID { get; set; }


        public string sanctionType { get; set; }
        public string Package { get; set; }
    }


    public class DateValidationAttribute : ValidationAttribute
    {

        private const string _defaultErrorMessage = "Unlock End Date must be greater than or equal to Unlock Start Date.";
        private string _basePropertyName;

        public DateValidationAttribute(string basePropertyName)
            : base(_defaultErrorMessage)
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
            //Get PropertyInfo Object  
            var basePropertyInfo = validationContext.ObjectType.GetProperty(_basePropertyName);
            if (PMGSYSession.Current.RoleCode == 36 || PMGSYSession.Current.RoleCode == 47 || PMGSYSession.Current.RoleCode == 56)
            {
                return ValidationResult.Success;
            }
            //Get Value of the property  
            CommonFunctions objCommon = new CommonFunctions();
            //var startDate = (DateTime)basePropertyInfo.GetValue(validationContext.ObjectInstance, null);            
            //var thisDate = (DateTime)value;  

            var sDate = basePropertyInfo.GetValue(validationContext.ObjectInstance, null);
            var eDate = value;

            if (sDate != null && eDate != null)
            {
                DateTime startDate = objCommon.GetStringToDateTime(sDate.ToString());
                DateTime thisDate = objCommon.GetStringToDateTime(eDate.ToString());

                //Actual comparision  
                if (thisDate < startDate)
                {
                    var message = FormatErrorMessage(validationContext.DisplayName);
                    return new ValidationResult(message);
                }
            }

            //Default return - This means there were no validation error  
            return null;
        }

        public DateTime? ConvertStringToDate(string dateToConvert)
        {
            DateTime MyDateTime;
            MyDateTime = new DateTime();
            MyDateTime = DateTime.Parse(dateToConvert);

            // DateTime.ParseExact(dateToConvert, "dd/MM/yyyy", null);
            //Convert.ToDateTime(dateToConvert);         //DateTime.ParseExact(dateToConver, "dd/MM/yyyy",null);
            return MyDateTime;
        }

    }

    


}