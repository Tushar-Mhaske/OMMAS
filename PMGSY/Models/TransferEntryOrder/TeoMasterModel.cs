using PMGSY.BLL.Common;
using PMGSY.Common;
using PMGSY.DAL.Receipt;
using PMGSY.DAL.TransferEntryOrder;
using PMGSY.Extensions;
using PMGSY.Models.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.TransferEntryOrder
{
    public partial class TeoMasterModel
    {
        public TeoMasterModel()
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

        //Avinash_Start
        public string EncryptedEAuthID { get; set; }
        public string Module { get; set; }
        //Avinash_End

        public long BILL_ID { get; set; }

        public long PBILL_ID { get; set; }

        public string ENC_PBILL_ID { get; set; }

        public Int16 TXN_NO { get; set; }

        [Display(Name = "TEO Number")]
        [Required(ErrorMessage = "Enter TEO Number")]
        [RegularExpression(@"^(?=.*[0-9])[a-zA-Z0-9-/]+$", ErrorMessage = "Only Alphanumeric with atleast one number,'-','/' Allowed")]
        [StringLength(50, ErrorMessage = "Maximum {1} and Minimum {2} character Allowed", MinimumLength = 1)]
        [IsValidBillNo("BILL_MONTH", "BILL_YEAR", ErrorMessage = "TEO number already exist in either Adjustment,Imprest Settlement or Transfer of Balances")]
        public string BILL_NO { get; set; }

        [Display(Name = "Month")]
       // [Required(ErrorMessage = "Select Month")]
        [Range(1, 12, ErrorMessage = "Please select month")]
        public short BILL_MONTH { get; set; }

        [Display(Name = "Year")]
        //[Required(ErrorMessage = "Select Year")]
        [Range(2000,Int16.MaxValue, ErrorMessage = "Please select year")]
        public short BILL_YEAR { get; set; }

        [Display(Name = "TEO Date")]
        [Required(ErrorMessage="Enter TEO Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Please Enter valid TEO date")]
        //[DataType(DataType.Date, ErrorMessage = "Invalid Date")]
        [IsDateAfter("CURRENT_DATE", true, ErrorMessage = "TEO Date must be less than or equal to today's date")]
        [IsDateBeforeOB("OB_DATE", true, ErrorMessage = "TEO Date must be greater than opening balance date")]
        [IsValidDate("BILL_MONTH", "BILL_YEAR", ErrorMessage = "TEO Date must be in selected month and year")]
        public string BILL_DATE { get; set; }

        [Display(Name = "Transaction Type")]
        [Required(ErrorMessage = "Select Transaction")]
        public short TXN_ID { get; set; }

        [Display(Name = "Sub Transaction Type")]
        [Required(ErrorMessage = "Select Sub Transaction")]
        public short SUB_TXN_ID { get; set; }

        public Nullable<int> CHQ_Book_ID { get; set; }
        public string CHQ_NO { get; set; }
        public Nullable<System.DateTime> CHQ_DATE { get; set; }
        public Nullable<decimal> CHQ_AMOUNT { get; set; }
        public Nullable<decimal> CASH_AMOUNT { get; set; }

        [Display(Name = "Amount")]
        [Required(ErrorMessage = "Amount is Required")]
        [RegularExpression(@"^\d{0,11}(\.\d{0,2})?$", ErrorMessage = "Only 11 Digit Whole number and 2 Decimal Allowed")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Invalid Amount")]
        [IsValidImprestAmount("ENC_PBILL_ID", "TXN_NO")]
        public Nullable<decimal> GROSS_AMOUNT { get; set; }

        public string CURRENT_DATE { get; set; }

        //new change done by Vikram on 12-09-2013
        //public Nullable<DateTime> OB_DATE { get; set; }
        public string OB_DATE { get; set; }
        //end of change

        public string CHQ_EPAY { get; set; }
        public Nullable<byte> TEO_TRANSFER_TYPE { get; set; }
        public string BILL_FINALIZED { get; set; }
        public string FUND_TYPE { get; set; }
        public int ADMIN_ND_CODE { get; set; }
        public short LVL_ID { get; set; }
        public string BILL_TYPE { get; set; }

        // Not Used for this functionality
        public string CHALAN_NO { get; set; }
        public Nullable<System.DateTime> CHALAN_DATE { get; set; }
        public string PAYEE_NAME { get; set; }
        public Nullable<byte> REMIT_TYPE { get; set; }
        public Nullable<int> MAST_CON_ID { get; set; }        
        public string ACTION_REQUIRED { get; set; }


        public string TEObillId { get; set; }

        public virtual ICollection<ACC_BILL_DETAILS> ACC_BILL_DETAILS { get; set; }
        public virtual ACC_CHQ_BOOK_DETAILS ACC_CHQ_BOOK_DETAILS { get; set; }
        public virtual ACC_MASTER_BILL_TYPE ACC_MASTER_BILL_TYPE { get; set; }
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
            try
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



                if (objDAL.IsDuplicateBillNo(Convert.ToInt16(month), Convert.ToInt16(year), PMGSYSession.Current.AdminNdCode, PMGSYSession.Current.LevelId, PMGSYSession.Current.FundType, "J", value.ToString()))
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
                else
                {
                    return ValidationResult.Success;
                }
            }
            catch (Exception ex)
            {
                return null;
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
            try
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
                ValidationType = "isvaliddate"
            };
            rule.ValidationParameters["month"] = this.MonthProperty;
            rule.ValidationParameters["year"] = this.YearProperty;
            yield return rule;
        }
    }

    public class IsValidImprestAmount : ValidationAttribute, IClientValidatable
    {
        private string DefaultErrorMessage = "Imprest amount must be less than Gross Amount";
        private readonly string PayBillIdProperty;
        private readonly string PayTxnNoProperty;


        public IsValidImprestAmount(string payBillIdProperty, string payTxnNoProperty)
        {
            this.PayBillIdProperty = payBillIdProperty;
            this.PayTxnNoProperty = payTxnNoProperty;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(DefaultErrorMessage, name, PayBillIdProperty);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            TransferEntryOrderDAL objDAL = new TransferEntryOrderDAL();
            var payBillIdPropertyInfo = validationContext.ObjectType.GetProperty(this.PayBillIdProperty);
            var payTxnNoPropertyInfo = validationContext.ObjectType.GetProperty(this.PayTxnNoProperty);

            if (payBillIdPropertyInfo == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.PayBillIdProperty));
            }

            var pBillId = payBillIdPropertyInfo.GetValue(validationContext.ObjectInstance, null);
            var pTxnNo = payTxnNoPropertyInfo.GetValue(validationContext.ObjectInstance, null);
            
            if (pBillId == null || pTxnNo == null)
            {
                return ValidationResult.Success;
            }

            String pbillid = pBillId.ToString();
            string[] strParameters = URLEncrypt.DecryptParameters(new string[] { pbillid.Split('/')[0], pbillid.Split('/')[1], pbillid.Split('/')[2] });
            Int64 PBillID = Convert.ToInt64(strParameters[0]);
            Int16 PTxnNo = Convert.ToInt16(pTxnNo.ToString());


            if (objDAL.IsImprestAmountValidMultiple(Convert.ToInt64(PBillID),PTxnNo, Convert.ToDecimal(value)))
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));                
            }
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = "Settlement amount must be less than or equal to Gross Amount",
                ValidationType = "isvalidimprestamount"
            };
            rule.ValidationParameters["pbillid"] = this.PayBillIdProperty;
            yield return rule;
        }

        public class ListImprest1
        {
            public long MAP_ID { get; set; }
            public long P_BILL_ID { get; set; }
            public Nullable<short> P_TXN_ID { get; set; }
            public long S_BIll_ID { get; set; }
            public Nullable<short> S_TXN_ID { get; set; }
        }
    }


    
}