using PMGSY.BLL.Common;
using PMGSY.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Authorization
{
    public class AuthorizationRequestMasterModel
    {
        public AuthorizationRequestMasterModel() {
            CommonFunctions objCommon = new CommonFunctions();
            this.CURRENT_DATE = objCommon.GetDateTimeToString(DateTime.Now);
            this.AUTH_DATE = this.CURRENT_DATE;
            Hidden_CURRENT_DATE = this.CURRENT_DATE;
            this.CHQ_EPAY = "Q";
        }


        public long AUTH_ID { get; set; }

        [Display(Name = "Authorization Number")]
        [Required(ErrorMessage = "Authorization Number is Required")]
        [MaxLength(50, ErrorMessage = "Authorization Number can be atmost 50 digit long")]
        [RegularExpression(@"^[a-zA-Z0-9-/]+$", ErrorMessage = "Only Alphanumeric, Space and '-','/' Allowed")]
        public string AUTH_NO { get; set; }

         [Required(ErrorMessage = "Month is Required")]
         [Range(1, 12, ErrorMessage = "Please Select Month")]
        public short AUTH_MONTH { get; set; }
        public List<SelectListItem> AUTH_MONTH_LIST { get; set; }

         [Required(ErrorMessage = "Year is Required")]
         [Range(1, Int16.MaxValue, ErrorMessage = "Please Select Year")]
          public short AUTH_YEAR { get; set; }
         public List<SelectListItem> AUTH_YEAR_LIST { get; set; }



         [Display(Name = "Authorization Date")]
         [Required(ErrorMessage = "Authorization Date is Required")]
         [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Please Enter valid Authorization Date")]
         [DataType(DataType.Date, ErrorMessage = "Invalid Date")]
         [IsDateAfter("CURRENT_DATE", true, ErrorMessage = "Authorization Date must be less than or equal to today's date")]
         [IsValidDate("AUTH_MONTH", "AUTH_YEAR", "CHQ_EPAY", ErrorMessage = "Authorization Date must be within {0} month and {1} year")]
         public String AUTH_DATE { get; set; }

         public String CHQ_EPAY { get; set; }

         [Display(Name = "Transaction Type")]
         [Required(ErrorMessage = "Transaction Type is Required")]
         public String TXN_ID { get; set; }
         public List<SelectListItem> TXN_ID_LIST { get; set; }

         [Display(Name = "Cash Amount")]
        [RegularExpression(@"^\d{0,11}(\.\d{0,2})?$", ErrorMessage = "Only 11 Digit Whole number and 2 Decimal Allowed")]
        [CompareCashAmount("CHEQUE_AMOUNT", ErrorMessage = "Cash Amount must be less than or equal to Request amount.")]
        public decimal CASH_AMOUNT { get; set; }

        [RegularExpression(@"^\d{0,11}(\.\d{0,2})?$", ErrorMessage = "Only 11 Digit Whole number and 2 Decimal Allowed")]
        [Display(Name = "Bank Authorization Request Amount")]
        [Range(0.01, Double.MaxValue, ErrorMessage = "Please Enter Valid Bank Authorization Request Amount")]
        public decimal CHEQUE_AMOUNT { get; set; }

        [Display(Name = "Company Name (Contractor)")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "invalid Company (Contractor)")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Invalid Company (Contractor)")]
        public Nullable<int> MAST_CON_ID_C { get; set; }
        public List<SelectListItem> MAST_CON_ID_C1 { get; set; }

        [Display(Name = "Payee Name")]
        [MaxLength(100, ErrorMessage = "Payee Name can be atmost 100 characters long")]
        public string PAYEE_NAME { get; set; }


        public decimal GROSS_AMOUNT { get; set; }

        public int MAST_CON_ID { get; set; }

        public string AUTH_FINALIZED { get; set; }

        public string FUND_TYPE { get; set; }

        public int ADMIN_ND_CODE { get; set; }

        public short LVL_ID { get; set; }

        public string CURRENT_DATE { get; set; }

        public string Hidden_CURRENT_DATE { get; set; }

        public virtual ICollection<ACC_AUTH_REQUEST_TRACKING> ACC_AUTH_REQUEST_TRACKING { get; set; }
        public virtual ACC_MASTER_FUND_TYPE ACC_MASTER_FUND_TYPE { get; set; }
        public virtual ACC_MASTER_LEVEL ACC_MASTER_LEVEL { get; set; }
        public virtual ACC_MASTER_TXN ACC_MASTER_TXN { get; set; }
        public virtual MASTER_CONTRACTOR MASTER_CONTRACTOR { get; set; }


    }


    public class AuthorizationRequestList
    {
        public AuthorizationRequestList()
        {


        }

       
    
        public long AUTH_ID { get; set; }
        public string AUTH_NO { get; set; }
        public byte AUTH_MONTH { get; set; }
        public short AUTH_YEAR { get; set; }
        public System.DateTime AUTH_DATE { get; set; }
        public short TXN_ID { get; set; }
        public decimal CASH_AMOUNT { get; set; }
        public decimal CHEQUE_AMOUNT { get; set; }
        public decimal GROSS_AMOUNT { get; set; }
        public int MAST_CON_ID { get; set; }
        public string AUTH_FINALIZED { get; set; }
        public string FUND_TYPE { get; set; }
        public int ADMIN_ND_CODE { get; set; }
        public byte LVL_ID { get; set; }

        public string AUTH_STATUS { get; set; }

        public virtual ICollection<ACC_AUTH_REQUEST_TRACKING> ACC_AUTH_REQUEST_TRACKING { get; set; }
        public virtual ACC_MASTER_FUND_TYPE ACC_MASTER_FUND_TYPE { get; set; }
        public virtual ACC_MASTER_LEVEL ACC_MASTER_LEVEL { get; set; }
        public virtual ACC_MASTER_TXN ACC_MASTER_TXN { get; set; }
        public virtual MASTER_CONTRACTOR MASTER_CONTRACTOR { get; set; }

    }

    public class CompareCashAmount : ValidationAttribute, IClientValidatable
    {
        private readonly string PropertyName;

        public CompareCashAmount(string propertyName)
        {
            this.PropertyName = propertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyTestedInfo = validationContext.ObjectType.GetProperty(this.PropertyName);
            if (propertyTestedInfo == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.PropertyName));
            }

            var requestValue = Convert.ToDecimal(propertyTestedInfo.GetValue(validationContext.ObjectInstance, null));
            var cashValue = Convert.ToDecimal(value);

            if (requestValue < cashValue)
            {
                if (cashValue == null)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
            }
            else
            {
                return ValidationResult.Success;
            }
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "comparecashamount"
            };
            //rule.ValidationParameters["compareworkpayment"] = this.PropertyName;
            yield return rule;
        }
    }

}