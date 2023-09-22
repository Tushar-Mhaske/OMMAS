using PMGSY.BLL.Common;
using PMGSY.Common;
using PMGSY.DAL.Receipt;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.Extensions;
using PMGSY.Models.Common;

namespace PMGSY.Models.Receipts
{
    public class BillMasterViewModel
    {
        public BillMasterViewModel()
        {
            this.ACC_BILL_DETAILS = new HashSet<ACC_BILL_DETAILS>();
            this.ACC_CANCELLED_CHEQUES = new HashSet<ACC_CANCELLED_CHEQUES>();
            this.ACC_CANCELLED_CHEQUES1 = new HashSet<ACC_CANCELLED_CHEQUES>();
            this.ACC_EPAY_MAIL_MASTER = new HashSet<ACC_EPAY_MAIL_MASTER>();
            this.ACC_TXN_BANK = new HashSet<ACC_TXN_BANK>();
            CommonFunctions objCommon = new CommonFunctions();
            this.CURRENT_DATE = objCommon.GetDateTimeToString(DateTime.Now);
            TransactionParams objParam = new TransactionParams();
            objParam.FUND_TYPE = PMGSYSession.Current.FundType;
            objParam.LVL_ID = PMGSYSession.Current.LevelId;
            objParam.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
            this.OB_DATE = objCommon.GetDateTimeToString(objCommon.GetOBDate(objParam));
        }

        public long BILL_ID { get; set; }

        [Display(Name = "Receipt Number")]
        [Required(ErrorMessage = "Receipt Number is Required")]
        [RegularExpression(@"^(?=.*[0-9])[a-zA-Z0-9-/]+$", ErrorMessage = "Only Alphanumeric with atleast one number,'-','/' Allowed")]
        [StringLength(50, ErrorMessage = "Maximum {1} and Minimum {2} character Allowed", MinimumLength = 1)]
        [IsValidBillNo("BILL_MONTH", "BILL_YEAR", ErrorMessage = "Receipt Number already exists")]
        public string BILL_NO { get; set; }

        [Range(1, 32267, ErrorMessage = "Select Month")]
        public short BILL_MONTH { get; set; }

        [Range(1, 32267, ErrorMessage = "Select Year")]
        public short BILL_YEAR { get; set; }

        [Display(Name = "Receipt Date")]
        [Required(ErrorMessage = "Receipt Date is Required")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Please Enter valid date")]
        [DataType(DataType.Date, ErrorMessage = "Invalid Date")]
        [IsDateAfter("CURRENT_DATE", true, ErrorMessage = "Receipt Date must be less than or equal to today's date")]
        [IsDateBeforeOB("OB_DATE", true, ErrorMessage = "Receipt Date must be greater than opening balance date")]
        [IsValidDate("BILL_MONTH", "BILL_YEAR", "CHQ_EPAY", ErrorMessage = "Receipt Date must be within selected month and year")]
        public String BILL_DATE { get; set; }

        [Display(Name = "Transaction Type")]
        [Required(ErrorMessage = "Transaction Type is Required")]
        //[Range(1,32267,ErrorMessage="Please Select Transaction Type")]
        public String TXN_ID { get; set; }

        public Nullable<int> CHQ_Book_ID { get; set; }

        [Display(Name = "Cheque/ Reference No")]
        [RegularExpression(@"^[a-zA-Z0-9/-]+$", ErrorMessage = "Only Alphanumeric,'-','/' Allowed")]
        [IsCheque("CHQ_EPAY", ErrorMessage = "Cheque Number is Required")]
        public String CHQ_NO { get; set; }

        [Display(Name = "Cheque/Reference Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Please Enter valid date")]
        [IsCheque("CHQ_EPAY", ErrorMessage = "Cheque Date is Required")]
        [IsChequeDateGreater("CHQ_EPAY", ErrorMessage = "Cheque Date must be less than current date")]
        [IsValidDate("BILL_MONTH", "BILL_YEAR", "CHQ_EPAY", ErrorMessage = "Cheque/Reference Date must be within selected month and year")]
        public String CHQ_DATE { get; set; }

        public decimal CHQ_AMOUNT { get; set; }
        public decimal CASH_AMOUNT { get; set; }

        [Display(Name = "Amount")]
        [Required(ErrorMessage = "Amount is Required")]
        [RegularExpression(@"^\d{0,11}(\.\d{0,2})?$", ErrorMessage = "Only 11 Digit Whole number and 2 Decimal Allowed")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Invalid Amount")]
        public Nullable<decimal> GROSS_AMOUNT { get; set; }

        public string CHALAN_NO { get; set; }
        public Nullable<System.DateTime> CHALAN_DATE { get; set; }
        public string PAYEE_NAME { get; set; }

        [Display(Name = "Mode")]
        [Required(ErrorMessage = "Mode is Required")]
        public string CHQ_EPAY { get; set; }

        public string CURRENT_DATE { get; set; }

        //public Nullable<DateTime> OB_DATE { get; set; }
        public string OB_DATE { get; set; }

        public Nullable<Int16> TEO_TRANSFER_TYPE { get; set; }
        public Nullable<Int16> REMIT_TYPE { get; set; }
        public string BILL_FINALIZED { get; set; }
        public string FUND_TYPE { get; set; }
        public int ADMIN_ND_CODE { get; set; }
        public short LVL_ID { get; set; }
        public Nullable<int> MAST_CON_ID { get; set; }
        public string BILL_TYPE { get; set; }
        public string DED_REQ { get; set; }
        public string EPAY_REQ { get; set; }
        public string BAR_REQ { get; set; }
        public string REM_REQ { get; set; }
        public string MTXN_REQ { get; set; }

        public virtual ICollection<ACC_BILL_DETAILS> ACC_BILL_DETAILS { get; set; }
        public virtual ACC_CHQ_BOOK_DETAILS ACC_CHQ_BOOK_DETAILS { get; set; }
        public virtual ACC_MASTER_FUND_TYPE ACC_MASTER_FUND_TYPE { get; set; }
        public virtual ACC_MASTER_LEVEL ACC_MASTER_LEVEL { get; set; }
        public virtual ACC_MASTER_REM_TYPE ACC_MASTER_REM_TYPE { get; set; }
        public virtual ACC_MASTER_TXN ACC_MASTER_TXN { get; set; }
        public virtual MASTER_CONTRACTOR MASTER_CONTRACTOR { get; set; }
        public virtual ICollection<ACC_CANCELLED_CHEQUES> ACC_CANCELLED_CHEQUES { get; set; }
        public virtual ICollection<ACC_CANCELLED_CHEQUES> ACC_CANCELLED_CHEQUES1 { get; set; }
        public virtual ACC_CHEQUES_ISSUED ACC_CHEQUES_ISSUED { get; set; }
        public virtual ICollection<ACC_EPAY_MAIL_MASTER> ACC_EPAY_MAIL_MASTER { get; set; }
        public virtual ICollection<ACC_TXN_BANK> ACC_TXN_BANK { get; set; }
        public virtual ICollection<MASTER_MONTH> MASTER_MONTH { get; set; }
        public virtual ICollection<MASTER_YEAR> MASTER_YEAR { get; set; }
    }
}

public class IsCheque : ValidationAttribute, IClientValidatable
{
    private readonly string PropertyName;

    public IsCheque(string propertyName)
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

        var chequeEpay = propertyTestedInfo.GetValue(validationContext.ObjectInstance, null);

        if (chequeEpay.ToString().ToLower() == "q" || chequeEpay.ToString().ToLower() == "a")//or condition Added by Abhishek kamble for Advice no 6Apr2015
        {
            if (value == null)
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
            else
            {
                return ValidationResult.Success;
            }
        }

        return ValidationResult.Success;
    }

    public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
    {
        var rule = new ModelClientValidationRule
        {
            ErrorMessage = this.ErrorMessageString,
            ValidationType = "ischeque"
        };
        rule.ValidationParameters["chequeepay"] = this.PropertyName;
        yield return rule;
    }

}

public class IsChequeDateGreater : ValidationAttribute, IClientValidatable
{
    private readonly string PropertyName;

    public IsChequeDateGreater(string propertyName)
    {
        this.PropertyName = propertyName;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        try
        {
            CommonFunctions objCommon = new CommonFunctions();
            var propertyTestedInfo = validationContext.ObjectType.GetProperty(this.PropertyName);
            if (propertyTestedInfo == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.PropertyName));
            }

            var chequeEpay = propertyTestedInfo.GetValue(validationContext.ObjectInstance, null);

            if (chequeEpay.ToString().ToLower() == "q")
            {
                if (value != null)
                {
                    if (objCommon.GetStringToDateTime(value.ToString()) > objCommon.GetStringToDateTime(objCommon.GetDateTimeToString(DateTime.Now)))
                    {
                        return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                    }
                    else
                    {
                        return ValidationResult.Success;
                    }
                }
            }

            return ValidationResult.Success;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
    {
        var rule = new ModelClientValidationRule
        {
            ErrorMessage = this.ErrorMessageString,
            ValidationType = "ischequedategreater"
        };
        rule.ValidationParameters["chequeepay"] = this.PropertyName;
        yield return rule;
    }

}


public class IsValidDate : ValidationAttribute, IClientValidatable
{
    private readonly string MonthProperty;
    private readonly string YearProperty;
    private readonly string ChequeProperty;


    public IsValidDate(string monthProperty, string yearProperty, string chqepayProperty)
    {
        this.MonthProperty = monthProperty;
        this.YearProperty = yearProperty;
        this.ChequeProperty = chqepayProperty;
    }

    public string FormatErrorMessage1(string month, string year)
    {
        return string.Format(month, year);
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        try
        {


            CommonFunctions objCommon = new CommonFunctions();
            var monthPropertyInfo = validationContext.ObjectType.GetProperty(this.MonthProperty);
            var yearPropertyInfo = validationContext.ObjectType.GetProperty(this.YearProperty);
            var chqepayPropertyInfo = validationContext.ObjectType.GetProperty(this.ChequeProperty);
            if (monthPropertyInfo == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.MonthProperty));
            }
            else if (yearPropertyInfo == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.YearProperty));
            }

            var month = monthPropertyInfo.GetValue(validationContext.ObjectInstance, null);
            var year = yearPropertyInfo.GetValue(validationContext.ObjectInstance, null);
            var chqepay = chqepayPropertyInfo.GetValue(validationContext.ObjectInstance, null);
            string test = validationContext.DisplayName.ToLower();
            bool stat = validationContext.DisplayName.ToLower().Equals("cheque date");
            if ((validationContext.DisplayName.ToLower().Equals("cheque date") && chqepay.ToString().ToLower().Equals("q")) || validationContext.DisplayName.ToLower().Equals("receipt date") || validationContext.DisplayName.ToLower().Equals("authorization date"))
            {

                DateTime billDate = objCommon.GetStringToDateTime(value.ToString());

                string monthText = objCommon.getMonthText(Convert.ToInt16(month));
                if (billDate.Month != Convert.ToInt32(month))
                {
                    if (validationContext.DisplayName.ToLower().Equals("cheque date"))
                    {
                        return new ValidationResult(String.Format("Cheque Date must be within {0} month and {1} year", monthText, year.ToString()));
                    }
                    else if (validationContext.DisplayName.ToLower().Equals("reciept date"))
                    {
                        return new ValidationResult(String.Format("Receipt Date must be within {0} month and {1} year", monthText, year.ToString()));
                    }
                    else
                    {
                        return new ValidationResult(String.Format("Authorization Date must be within {0} month and {1} year", monthText, year.ToString()));
                    }

                }
                else if (billDate.Year != Convert.ToInt32(year))
                {
                    if (validationContext.DisplayName.ToLower().Equals("cheque date"))
                    {
                        return new ValidationResult(String.Format("Cheque Date must be within {0} month and {1} year", monthText, year.ToString()));
                    }
                    else if (validationContext.DisplayName.ToLower().Equals("reciept date"))
                    {
                        return new ValidationResult(String.Format("Receipt Date must be within {0} month and {1} year", monthText, year.ToString()));
                    }
                    else
                    {
                        return new ValidationResult(String.Format("Authorization Date must be within {0} month and {1} year", monthText, year.ToString()));
                    }
                }
                else
                {
                    return ValidationResult.Success;
                }
            }
            else
            {
                return ValidationResult.Success;
            }
        }
        catch (Exception)
        {

            return null;
        }

        //return ValidationResult.Success;
    }

    public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
    {
        var rule = new ModelClientValidationRule
        {
            ErrorMessage = this.ErrorMessageString,
            ValidationType = "isvaliddate"
        };
        rule.ValidationParameters["month"] = this.MonthProperty;
        rule.ValidationParameters["year"] = this.YearProperty;
        rule.ValidationParameters["chqepay"] = this.ChequeProperty;
        yield return rule;
    }
}

public class IsValidBillNo : ValidationAttribute
{
    private readonly string MonthProperty;
    private readonly string YearProperty;

    public IsValidBillNo(string monthProperty, string yearProperty)
    {
        this.MonthProperty = monthProperty;
        this.YearProperty = yearProperty;
    }

    public string FormatErrorMessage1(string month, string year)
    {
        return string.Format(month, year);
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        ReceiptDAL objDAL = new ReceiptDAL();
        var monthPropertyInfo = validationContext.ObjectType.GetProperty(this.MonthProperty);
        var yearPropertyInfo = validationContext.ObjectType.GetProperty(this.YearProperty);

        if (monthPropertyInfo == null)
        {
            return new ValidationResult(string.Format("unknown property {0}", this.MonthProperty));
        }
        else if (yearPropertyInfo == null)
        {
            return new ValidationResult(string.Format("unknown property {0}", this.YearProperty));
        }

        var month = monthPropertyInfo.GetValue(validationContext.ObjectInstance, null);
        var year = yearPropertyInfo.GetValue(validationContext.ObjectInstance, null);



        if (objDAL.IsDuplicateBillNo(Convert.ToInt16(month), Convert.ToInt16(year), PMGSYSession.Current.AdminNdCode, PMGSYSession.Current.LevelId, PMGSYSession.Current.FundType, "R", value.ToString()))
        {
            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }
        else
        {
            return ValidationResult.Success;
        }

    }
}
