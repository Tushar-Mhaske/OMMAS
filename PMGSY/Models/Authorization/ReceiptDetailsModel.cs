using PMGSY.BLL.Common;
using PMGSY.Common;
using PMGSY.DAL.Receipt;
using PMGSY.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PMGSY.Models.Authorization
{
    public class ReceiptDetailsModel
    {
        public ReceiptDetailsModel()
        {
            CommonFunctions objCommon = new CommonFunctions();
            this.CURRENT_DATE = objCommon.GetDateTimeToString(DateTime.Now);
        }

        public Int64 AUTH_ID { get; set; }

        [Display(Name = "Receipt Number")]
        [Required(ErrorMessage = "Receipt Number is Required")]
        [RegularExpression(@"^[a-zA-Z0-9-/]+$", ErrorMessage = "Only Alphanumeric,'-','/' Allowed")]
        [StringLength(50, ErrorMessage = "Maximum {1} and Minimum {2} character Allowed", MinimumLength = 1)]
        [IsValidBillNo("BILL_DATE", ErrorMessage = "Receipt Number already exists")]
        public string BILL_NO { get; set; }

        [Display(Name = "Receipt Date")]
        [Required(ErrorMessage = "Receipt Date is Required")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Please Enter valid date")]
        [DataType(DataType.Date, ErrorMessage = "Invalid Date")]
        [IsDateIn("CURRENT_DATE", "RHDN_AUTH_ID")]
        public String BILL_DATE { get; set; }

        public string CURRENT_DATE { get; set; }

        public String RHDN_AUTH_ID { get; set; }
    }

    public class IsValidBillNo : ValidationAttribute
    {
        private readonly string BillDateProperty;
        
        public IsValidBillNo(string billDateProperty)
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



            if (objDAL.IsDuplicateBillNo(Convert.ToInt16(new CommonFunctions().GetStringToDateTime(billDate.ToString()).Month), Convert.ToInt16(new CommonFunctions().GetStringToDateTime(billDate.ToString()).Year), PMGSYSession.Current.AdminNdCode, PMGSYSession.Current.LevelId, PMGSYSession.Current.FundType, "R", value.ToString()))
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
            else
            {
                return ValidationResult.Success;
            }
        }
    }
}