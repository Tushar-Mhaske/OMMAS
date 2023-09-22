using PMGSY.BLL.Common;
using PMGSY.Common;
using PMGSY.DAL.Receipt;
using PMGSY.DAL.OB;
using PMGSY.Extensions;
using PMGSY.Models.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.OB
{
    public class OBMasterModel
    {
        public OBMasterModel()
        {
            this.ACC_CANCELLED_CHEQUES = new HashSet<ACC_CANCELLED_CHEQUES>();
            this.ACC_CANCELLED_CHEQUES1 = new HashSet<ACC_CANCELLED_CHEQUES>();
            this.ACC_EPAY_MAIL_MASTER = new HashSet<ACC_EPAY_MAIL_MASTER>();
            this.ACC_TXN_BANK = new HashSet<ACC_TXN_BANK>();
            this.ACC_BILL_DETAILS = new HashSet<ACC_BILL_DETAILS>();
            CommonFunctions objCommon = new CommonFunctions();
            this.CURRENT_DATE = objCommon.GetDateTimeToString(DateTime.Now);
        }

        public long ASSET_BILL_ID { get; set; }

        public string ASSET_BILL_NO { get; set; }

        public long LIB_BILL_ID { get; set; }

        public string LIB_BILL_NO { get; set; }

        public short TXN_ID { get; set; }

        public short BILL_MONTH { get; set; }

        public short BILL_YEAR { get; set; }

        [Display(Name = "Opening Balance Date")]
        [Required(ErrorMessage = "Enter Asset OB Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Please Enter valid Asset date")]
        [IsDateAfter("CURRENT_DATE", true, ErrorMessage = "Asset Date must be less than or equal to today's date")]
        [IsValidOBDate("LIB_BILL_DATE")]
        public string ASSET_BILL_DATE { get; set; }

        [Display(Name = "Opening Balance Date")]
        [Required(ErrorMessage = "Enter Liability OB Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Please Enter valid Liability date")]
        [IsDateAfter("CURRENT_DATE", true, ErrorMessage = "Liability Date must be less than or equal to today's date")]
        [IsValidOBDate("ASSET_BILL_DATE")]
        public string LIB_BILL_DATE { get; set; }

        [Display(Name = "Opening Balance Amount")]
        [Required(ErrorMessage = "Asset Amount is Required")]
        [RegularExpression(@"^\d{0,11}(\.\d{0,2})?$", ErrorMessage = "Only 11 Digit Whole number and 2 Decimal Allowed")]
        [Range(0.00, double.MaxValue, ErrorMessage = "Invalid Asset Amount")]
        [IsValidOBAmount("LIB_GROSS_AMOUNT", ErrorMessage = "Opening Balance Amount is invalid.")]
        [IsMasterAmountLess("ASSET_GROSS_AMOUNT", ErrorMessage = "Asset master amount must be greater than details amount")]
        public Nullable<decimal> ASSET_GROSS_AMOUNT { get; set; }

        [Display(Name = "Opening Balance Amount")]
        [Required(ErrorMessage = "Liability Amount is Required")]
        [RegularExpression(@"^\d{0,11}(\.\d{0,2})?$", ErrorMessage = "Only 11 Digit Whole number and 2 Decimal Allowed")]
        [Range(0.00, double.MaxValue, ErrorMessage = "Invalid Liability Amount")]
        [IsValidOBAmount("ASSET_GROSS_AMOUNT", ErrorMessage = "Opening Balance Amount is invalid.")]
        [IsMasterAmountLess("LIB_GROSS_AMOUNT", ErrorMessage = "Liability master amount must be greater than details amount")]
        public Nullable<decimal> LIB_GROSS_AMOUNT { get; set; }

        public string CURRENT_DATE { get; set; }

        public string CHQ_EPAY { get; set; }
        public string BILL_FINALIZED { get; set; }
        public string FUND_TYPE { get; set; }
        public int ADMIN_ND_CODE { get; set; }
        public short LVL_ID { get; set; }
        public string BILL_TYPE { get; set; }
        public Nullable<int> MAST_CON_ID { get; set; }


        public virtual ACC_CHQ_BOOK_DETAILS ACC_CHQ_BOOK_DETAILS { get; set; }
        public virtual ACC_MASTER_BILL_TYPE ACC_MASTER_BILL_TYPE { get; set; }
        public virtual ACC_MASTER_FUND_TYPE ACC_MASTER_FUND_TYPE { get; set; }
        public virtual ACC_MASTER_LEVEL ACC_MASTER_LEVEL { get; set; }
        public virtual ACC_MASTER_REM_TYPE ACC_MASTER_REM_TYPE { get; set; }
        public virtual ACC_MASTER_TXN ACC_MASTER_TXN { get; set; }
        public virtual ADMIN_DEPARTMENT ADMIN_DEPARTMENT { get; set; }
        public virtual MASTER_CONTRACTOR MASTER_CONTRACTOR { get; set; }
        public virtual ICollection<ACC_CANCELLED_CHEQUES> ACC_CANCELLED_CHEQUES { get; set; }
        public virtual ICollection<ACC_CANCELLED_CHEQUES> ACC_CANCELLED_CHEQUES1 { get; set; }
        public virtual ACC_CHEQUES_ISSUED ACC_CHEQUES_ISSUED { get; set; }
        public virtual ICollection<ACC_EPAY_MAIL_MASTER> ACC_EPAY_MAIL_MASTER { get; set; }
        public virtual ICollection<ACC_TXN_BANK> ACC_TXN_BANK { get; set; }
        public virtual ICollection<ACC_BILL_DETAILS> ACC_BILL_DETAILS { get; set; }
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



            if (objDAL.IsDuplicateBillNo(Convert.ToInt16(month), Convert.ToInt16(year), PMGSYSession.Current.AdminNdCode, PMGSYSession.Current.LevelId, PMGSYSession.Current.FundType, "O", value.ToString()))
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
            else
            {
                return ValidationResult.Success;
            }
        }
    }

    public class IsValidDate : ValidationAttribute, IClientValidatable
    {
        private readonly string MonthProperty;
        private readonly string YearProperty;


        public IsValidDate(string monthProperty, string yearProperty)
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
            CommonFunctions objCommon = new CommonFunctions();
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

            DateTime billDate = objCommon.GetStringToDateTime(value.ToString());
            String monthText = objCommon.getMonthText(Convert.ToInt16(month));

            if (billDate.Month != Convert.ToInt32(month))
            {

                return new ValidationResult(String.Format("TEO Date must be in {0} month and {1} year", monthText, year.ToString()));
            }
            else if (billDate.Year != Convert.ToInt32(year))
            {
                return new ValidationResult(String.Format("TEO Date must be in {0} month and {1} year", monthText, year.ToString()));
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
                ValidationType = "isvaliddate"
            };
            rule.ValidationParameters["month"] = this.MonthProperty;
            rule.ValidationParameters["year"] = this.YearProperty;
            yield return rule;
        }
    }

    public class IsValidOBAmount : ValidationAttribute, IClientValidatable
    {
        private string DefaultErrorMessage = "Invalid Amount";
        private readonly string ObProperty;

        public IsValidOBAmount(string obProperty)
        {
            this.ObProperty = obProperty;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(DefaultErrorMessage, name, ObProperty);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var basePropertyInfo = validationContext.ObjectType.GetProperty(ObProperty);

            var libAmount = basePropertyInfo.GetValue(validationContext.ObjectInstance, null);
            var assetAmount = value;

            if (libAmount != null)
            {
                if (Convert.ToDecimal(assetAmount) != Convert.ToDecimal(libAmount))
                {
                    if (ObProperty.Equals("ASSET_GROSS_AMOUNT"))
                    {
                        DefaultErrorMessage = "Liability and Asset amount must be equal";
                    }
                    else
                    {
                        DefaultErrorMessage = "Asset and Liability amount must be equal";
                    }
                    var message = FormatErrorMessage(validationContext.DisplayName);
                    return new ValidationResult(message);
                }
            }

            return ValidationResult.Success;
        }


        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "isvalidobamount"
            };
            rule.ValidationParameters["obtype"] = this.ObProperty;
            yield return rule;
        }
    }

    public class IsValidOBDate : ValidationAttribute, IClientValidatable
    {
        private string DefaultErrorMessage = "Invalid Date";
        private readonly string ObProperty;

        public IsValidOBDate(string obProperty)
        {
            this.ObProperty = obProperty;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(DefaultErrorMessage, name, ObProperty);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var basePropertyInfo = validationContext.ObjectType.GetProperty(ObProperty);
            CommonFunctions objCommon = new CommonFunctions();
            DateTime libDate = objCommon.GetStringToDateTime(basePropertyInfo.GetValue(validationContext.ObjectInstance, null).ToString());
            DateTime assetDate = objCommon.GetStringToDateTime(value.ToString());

            if (libDate != null)
            {
                if (assetDate != libDate)
                {
                    if (ObProperty.Equals("ASSET_BILL_DATE"))
                    {
                        DefaultErrorMessage = "Liability and Asset date must be equal";
                    }
                    else
                    {
                        DefaultErrorMessage = "Asset and Liability date must be equal";
                    }
                    var message = FormatErrorMessage(validationContext.DisplayName);
                    return new ValidationResult(message);
                }
            }

            return ValidationResult.Success;
        }


        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "isvalidobdate"
            };
            rule.ValidationParameters["obtype"] = this.ObProperty;
            yield return rule;
        }
    }

    public class IsMasterAmountLess : ValidationAttribute, IClientValidatable
    {
        private string DefaultErrorMessage = "Invalid Amount";
        private readonly string ObProperty;

        public IsMasterAmountLess(string obProperty)
        {
            this.ObProperty = obProperty;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(DefaultErrorMessage, name, ObProperty);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            TransactionParams objParam = new TransactionParams();
            OpeningBalanceDAL objDAL = new OpeningBalanceDAL();
            String amountToCompare = String.Empty;
            objParam.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
            objParam.LVL_ID = PMGSYSession.Current.LevelId;
            objParam.FUND_TYPE = PMGSYSession.Current.FundType;
            objParam.BILL_TYPE = "O";
            if (ObProperty.Equals("ASSET_GROSS_AMOUNT"))
            {
                objParam.BILL_NO = "1";
                amountToCompare = objDAL.GetAssetAmountDetails(objParam);
                if (Convert.ToDecimal(value) < Convert.ToDecimal(amountToCompare.Split('$')[1]))
                {
                    DefaultErrorMessage = "Asset master amount must be greater than details amount";
                    var message = FormatErrorMessage(validationContext.DisplayName);
                    //return new ValidationResult(message);
                    return new ValidationResult(validationContext.DisplayName);
                }
            }
            else
            {
                objParam.BILL_NO = "2";
                amountToCompare = objDAL.GetAssetAmountDetails(objParam);
                if (Convert.ToDecimal(value) < Convert.ToDecimal(amountToCompare.Split('$')[1]))
                {
                    DefaultErrorMessage = "Liability master amount must be greater than details amount";
                    var message = FormatErrorMessage(validationContext.DisplayName);
                    //return new ValidationResult(message);
                    return new ValidationResult(validationContext.DisplayName);
                }
            }

            return ValidationResult.Success;
        }


        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "ismasteramountless"
            };
            rule.ValidationParameters["obtype"] = this.ObProperty;
            yield return rule;
        }
    }
}