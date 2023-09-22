using PMGSY.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PMGSY.Models.LockUnlock
{
    public class LockDetailsViewModel
    {

        public int IMS_TRANSACTION_NO { get; set; }
        public string IMS_UNLOCK_TABLE { get; set; }
        
        public Nullable<int> MAST_ER_ROAD_CODE { get; set; }
        public Nullable<int> PLAN_CN_ROAD_CODE { get; set; }
        public Nullable<int> IMS_PR_ROAD_CODE { get; set; }
        public Nullable<int> TEND_NIT_NO { get; set; }
        public Nullable<int> TEND_AGREEMENT_CODE { get; set; }
        public Nullable<int> MANE_CONTRACT_CODE { get; set; }
        
        [Required(ErrorMessage="Please Enter Unlock Date.")]
        [Display(Name="Unlock Date")]
        public string IMS_UNLOCK_DATE { get; set; }

        [Required(ErrorMessage="Please Enter Valid To Date.")]
        [Display(Name="Valid Upto")]
        [DateValidationVST("IMS_UNLOCK_DATE",ErrorMessage="Valid To date must be greater than Unlock Date.")]
        public string IMS_AUTOLOCK_DATE { get; set; }

        public string IMS_LOCK_STATUS { get; set; }
        public string IMS_UNLOCK_BY { get; set; }
        public string IMS_DATA_FINALIZED { get; set; }


        [Required(ErrorMessage="Please Enter Remarks.")]
        [Display(Name="Remarks")]
        [RegularExpression(@"^([a-zA-Z0-9 ()]+)$", ErrorMessage = "Enter Correct Remarks.")]
        public string IMS_UNLOCK_REMARKS { get; set; }

        public int[] DataID { get; set; }


        public int? YearCode { get; set; }
        public int? StateCode { get; set; }
        public int? BatchCode { get; set; }
        public string PackageCode { get; set; }
        public int? DistrictCode { get; set; }
        public string LockStatus { get; set; }
        public int? BlockCode { get; set; }

        public virtual IMS_SANCTIONED_PROJECTS IMS_SANCTIONED_PROJECTS { get; set; }
        public virtual MASTER_EXISTING_ROADS MASTER_EXISTING_ROADS { get; set; }
        public virtual PLAN_ROAD PLAN_ROAD { get; set; }
        public virtual TEND_AGREEMENT_MASTER TEND_AGREEMENT_MASTER { get; set; }
        public virtual TEND_NIT_MASTER TEND_NIT_MASTER { get; set; }

    }

    public class DateValidationVSTAttribute : ValidationAttribute
    {

        private const string _defaultErrorMessage = "Valid Upto date must be greater than Unlock Date.";
        private string _basePropertyName;

        public DateValidationVSTAttribute(string basePropertyName)
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
                if (thisDate <= startDate)
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

