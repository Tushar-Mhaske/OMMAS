using PMGSY.BLL.Common;
using PMGSY.Common;
using PMGSY.DAL.ChequeAcknowledgement;
using PMGSY.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.ChequeAcknowledgement
{
    public class CheckAckModel
    {

        public CheckAckModel()
        {
            this.CHQ_EPAY = "Q";
            CommonFunctions objCommon = new CommonFunctions();
            this.CURRENT_DATE = objCommon.GetDateTimeToString(DateTime.Now);
        }


        public string CURRENT_DATE { get; set; }

       //for validation of date
        public short BILL_MONTH_VOUCHER { get; set; }
       
       
        public short BILL_YEAR_VOUCHER { get; set; }


        public string CHQ_EPAY { get; set; }

        [Display(Name = "Voucher Number")]
        [Required(ErrorMessage = "Voucher Number is Required")]
        [RegularExpression(@"^(?=.*[0-9])[a-zA-Z0-9-/]+$", ErrorMessage = "Only Alphanumeric with atleast one number,'-','/' Allowed")]
        [StringLength(50, ErrorMessage = "Maximum {1} and Minimum {2} character Allowed", MinimumLength = 1)]
        [IsValidBillNo("BILL_MONTH_VOUCHER", "BILL_YEAR_VOUCHER", ErrorMessage = "Voucher Number already exists")]
        public string BILL_NO { get; set; }

        [Display(Name = "Voucher Date")]
        [Required(ErrorMessage = "Voucher Date is Required")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Please Enter valid Voucher date")]
        [DataType(DataType.Date, ErrorMessage = "Invalid Voucher Date")]
        [IsDateAfter("CURRENT_DATE", true, ErrorMessage = "Voucher Date must be less than or equal to today's date")]
       // [IsValidDate("BILL_MONTH_VOUCHER", "BILL_YEAR_VOUCHER", "CHQ_EPAY", ErrorMessage = "Voucher Date must be within {0} month and {1} year")]
        public String BILL_DATE { get; set; }
     

        public long NA_BILL_ID { get; set; }

        public String STR_NA_BILL_ID { get; set; }

        //[CheckPreviousAckCheques("BILL_DATE",ErrorMessage="Previous month cheques are not finalized.")]
        
        public string SelectedIDArray { get; set; }

        public string AckBillIDArray { get; set; }

        
        public bool Finalize { get; set; }

        public short DPIU_CODE { get; set; }

        // Added by Srishti on 08-03-2023
        public string ACC_TYPE { get; set; }
    }

    public class CheckAckSelectionModel
    {
        public CheckAckSelectionModel()
        {
           
            CommonFunctions objCommon = new CommonFunctions();
            this.BILL_MONTH = Convert.ToInt16(DateTime.Now.Month);
            this.BILL_YEAR = Convert.ToInt16(DateTime.Now.Year);
        }

        [Range(1, Int16.MaxValue, ErrorMessage = "Please Select Month")]
        public short BILL_MONTH { get; set; }
        public List<SelectListItem> BILL_YEAR_LIST { get; set; }

        public string Mode { get; set; }

        [Range(1, Int16.MaxValue, ErrorMessage = "Please Select Year")]
        public short BILL_YEAR { get; set; }
        public List<SelectListItem> BILL_MONTH_LIST { get; set; }

        [Range(0, Int16.MaxValue, ErrorMessage = "Please Select DPIU")]
        public short DPIU { get; set; }
        public List<SelectListItem> DPIU_LIST { get; set; }

        public string fundType { get; set; }

        //Added By Abhishek kamble to show cheque ack details 8-July-2014
        public string hdnBillID { get; set; }
        public string AckUnackFlag { get; set; }

        // Added by Srishti on 03-03-2023
        [Range(1, Int16.MaxValue, ErrorMessage = "Please Select Account Type")]
        public string ACCOUNT_TYPE { get; set; }
        public List<SelectListItem> ACCOUNT_TYPE_LIST { get; set; }
    }

    public class CheckPreviousAckCheques : ValidationAttribute, IClientValidatable
    {
        private readonly string PropertyName;

        public CheckPreviousAckCheques(string propertyName)
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

            var newvalue = Convert.ToDecimal(propertyTestedInfo.GetValue(validationContext.ObjectInstance, null));
            var oldvalue = Convert.ToDecimal(value);
            if (oldvalue > newvalue)
            {
                if (newvalue == null)
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
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
                ValidationType = "comparevalidation"
            };
            yield return rule;
        }


    }



}
//public class IsValidVoucherNo : ValidationAttribute
//{
//    private readonly string MonthProperty;
//    private readonly string YearProperty;
//    private readonly string Finalise;
//    public IsValidVoucherNo(string monthProperty, string yearProperty,string finalise)
//    {
//        this.MonthProperty = monthProperty;
//        this.YearProperty = yearProperty;
//        this.Finalise = finalise;
//    }

//    public string FormatErrorMessage1(string month, string year, string finalise)
//    {
//        return string.Format(month, year,finalise);
//    }

//    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
//    {
//        ChequeAcknowledgementDAL objChequeAck = new ChequeAcknowledgementDAL();
//        var monthPropertyInfo = validationContext.ObjectType.GetProperty(this.MonthProperty);
//        var yearPropertyInfo = validationContext.ObjectType.GetProperty(this.YearProperty);
//        var finaliseProperty=validationContext.ObjectType.GetProperty(this.Finalise);
//        if (monthPropertyInfo == null)
//        {
//            return new ValidationResult(string.Format("unknown property {0}", this.MonthProperty));
//        }
//        else if (yearPropertyInfo == null)
//        {
//            return new ValidationResult(string.Format("unknown property {0}", this.YearProperty));
//        }

//        if (value == null)
//        {
//            return ValidationResult.Success;
//        }

//        var month = monthPropertyInfo.GetValue(validationContext.ObjectInstance, null);
//        var year = yearPropertyInfo.GetValue(validationContext.ObjectInstance, null);
//        var finalise = finaliseProperty.GetValue(validationContext.ObjectInstance, null);


//        if (objChequeAck.CheckDuplicateVoucher(Convert.ToInt16(month), Convert.ToInt16(year), PMGSYSession.Current.AdminNdCode, PMGSYSession.Current.LevelId, PMGSYSession.Current.FundType, "P", value.ToString(),finalise.ToString()))
//        {
//            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
//        }
//        else
//        {
//            return ValidationResult.Success;
//        }
//    }

//        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
//        {
//            var rule = new ModelClientValidationRule
//            {
//                ErrorMessage = this.ErrorMessageString,
//                ValidationType = "duplicatevoucher"
//            };
//            yield return rule;
//        }
// }
