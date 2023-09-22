using PMGSY.BLL.Common;
using PMGSY.Common;
using PMGSY.DAL.Authorization;
using PMGSY.DAL.Receipt;
using PMGSY.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PMGSY.Models.Authorization
{
    public class PaymentDetailsModel
    {
        public PaymentDetailsModel()
        {
            CommonFunctions objCommon = new CommonFunctions();
            this.CURRENT_DATE = objCommon.GetDateTimeToString(DateTime.Now);
        }

        public Int64 AUTH_ID { get; set; }

        [Display(Name = "Voucher Number")]
        [Required(ErrorMessage = "Voucher Number is Required")]
        [RegularExpression(@"^[a-zA-Z0-9-/]+$", ErrorMessage = "Only Alphanumeric,'-','/' Allowed")]
        [StringLength(50, ErrorMessage = "Maximum {1} and Minimum {2} character Allowed", MinimumLength = 1)]
        [IsValidVoucherNo("BILL_DATE", ErrorMessage = "Voucher Number already exists")]
        public string BILL_NO { get; set; }

        [Display(Name = "Voucher Date")]
        [Required(ErrorMessage = "Voucher Date is Required")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Please Enter valid date")]
        [DataType(DataType.Date, ErrorMessage = "Invalid Date")]
        [IsDateIn("CURRENT_DATE", "PHDN_AUTH_ID")]
        public String BILL_DATE { get; set; }

        public string CURRENT_DATE { get; set; }

        public String PHDN_AUTH_ID { get; set; }
    }

    public class IsValidVoucherNo : ValidationAttribute
    {
        private readonly string BillDateProperty;

        public IsValidVoucherNo(string billDateProperty)
        {
            this.BillDateProperty = billDateProperty;
        }

        public string FormatErrorMessage1(string month, string year)
        {
            return string.Format(month, year);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ReceiptDAL objDAL = new ReceiptDAL();
            var billDatePropertyInfo = validationContext.ObjectType.GetProperty(this.BillDateProperty);


            if (billDatePropertyInfo == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.BillDateProperty));
            }

            var billDate = billDatePropertyInfo.GetValue(validationContext.ObjectInstance, null);



            if (objDAL.IsDuplicateBillNo(Convert.ToInt16(new CommonFunctions().GetStringToDateTime(billDate.ToString()).Month), Convert.ToInt16(new CommonFunctions().GetStringToDateTime(billDate.ToString()).Year), PMGSYSession.Current.AdminNdCode, PMGSYSession.Current.LevelId, PMGSYSession.Current.FundType, "P", value.ToString()))
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
            else
            {
                return ValidationResult.Success;
            }
        }
    }

    public class IsDateIn : ValidationAttribute
    {
        private readonly string currentDate;
        private readonly string authId;
        private string DefaultErrorMessage = "Invalid Date";  
        
        CommonFunctions objCommon = new CommonFunctions();
        public IsDateIn(string currentdate, string authid)
        {
            this.currentDate = currentdate;
            this.authId = authid;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(DefaultErrorMessage, name);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            AuthorizationDAL objAuthDAL = new AuthorizationDAL();

            var currentDateInfo = validationContext.ObjectType.GetProperty(this.currentDate);
            if (currentDateInfo == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.currentDate));
            }

            var authDateInfo = validationContext.ObjectType.GetProperty(this.authId);
            if (authDateInfo == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.authId));
            }

            var currentDateValue = currentDateInfo.GetValue(validationContext.ObjectInstance, null);
            var authIdValue = authDateInfo.GetValue(validationContext.ObjectInstance, null);
            String[] AuthId = new String[3];
            if (authIdValue != null)
            {
                AuthId = URLEncrypt.DecryptParameters(new string[] { authIdValue.ToString().Split('/')[0], authIdValue.ToString().Split('/')[1], authIdValue.ToString().Split('/')[2] });
            }

                
            ACC_AUTH_REQUEST_TRACKING tracking_master = new ACC_AUTH_REQUEST_TRACKING();
            tracking_master = objAuthDAL.GetAuthorizationTrackingDetails(Convert.ToInt64(AuthId[0]), "A");

            if (value != null)
            {
                if (objCommon.GetStringToDateTime(value.ToString()) > objCommon.GetStringToDateTime(currentDateValue.ToString()))
                {
                    DefaultErrorMessage = "Date must be less than or equal to Current Date";
                    var message = FormatErrorMessage(validationContext.DisplayName);
                    return new ValidationResult(message);
                }
                else if (objCommon.GetStringToDateTime(value.ToString()) < tracking_master.DATE_OF_OPERATION)
                {
                    DefaultErrorMessage = "Date must be greater than or equal to " + objCommon.GetDateTimeToString(tracking_master.DATE_OF_OPERATION);
                    var message = FormatErrorMessage(validationContext.DisplayName);
                    return new ValidationResult(message);
                }
                else
                {
                    return ValidationResult.Success;
                }
            }

            return ValidationResult.Success;
        }
    }
}