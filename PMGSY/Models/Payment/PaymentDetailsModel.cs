using PMGSY.BLL.Common;
using PMGSY.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.PaymentModel
{
    public class IsDateAfterSearchValidation : ValidationAttribute, IClientValidatable
    {
        private readonly string testedPropertyName;
        private readonly bool allowEqualDates;
        CommonFunctions objCommon = new CommonFunctions();
        public IsDateAfterSearchValidation(string testedPropertyName, bool allowEqualDates = false)
        {
            this.testedPropertyName = testedPropertyName;
            this.allowEqualDates = allowEqualDates;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyTestedInfo = validationContext.ObjectType.GetProperty(this.testedPropertyName);
            if (propertyTestedInfo == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.testedPropertyName));
            }

            var propertyTestedValue = propertyTestedInfo.GetValue(validationContext.ObjectInstance, null);

            if (value != null && value!= String.Empty)
            {
                // Compare values
                if (objCommon.GetStringToDateTime(value.ToString()) <= objCommon.GetStringToDateTime(propertyTestedValue.ToString()))
                {
                    if (this.allowEqualDates)
                    {
                        return ValidationResult.Success;
                    }
                    if ((DateTime)value < (DateTime)propertyTestedValue)
                    {
                        return ValidationResult.Success;
                    }
                }
                else
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
            }

            if (value == null || !(value is DateTime))
            {
                return ValidationResult.Success;
            }



            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "isdateaftersearchvalidation"
            };
            rule.ValidationParameters["propertytested"] = this.testedPropertyName;
            rule.ValidationParameters["allowequaldates"] = this.allowEqualDates;
            yield return rule;
        }

    }
    public class ListModel
    {
        public ListModel()
        {
            CommonFunctions objCommon = new CommonFunctions();
            this.CURRENT_DATE = objCommon.GetDateTimeToString(DateTime.Now);
        }

        [Display(Name = "From Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Please Enter valid From Date")]
        [IsDateAfterSearchValidation("toDate", true, ErrorMessage = "From Date must be less than or equal to To Date")]
        public String fromDate { get; set; }


        [Display(Name = "To Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Please Enter valid To date")]
        [IsDateAfterSearchValidation("CURRENT_DATE", true, ErrorMessage = "To Date must be less than or equal to today's date")]
        public String toDate { get; set; }

        public string CURRENT_DATE { get; set; }

        [Display(Name = "Transaction Type")]
         public String TXN_ID { get; set; }

        [Display(Name = "Cheque/EpayNumber")]
        [RegularExpression(@"^[a-zA-Z0-9-/ ]+$", ErrorMessage = "Only Alphanumeric, Space and '-','/' Allowed")]
        [MaxLength(30,ErrorMessage="Cheque/EpayNumber can be atmost 30 characters long")]
        public String Chq_Epay { get; set; }

        [Display(Name = "Authorization Status")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Only Alphabets Allowed")]
        [MaxLength(30, ErrorMessage = "Authorization Status can be atmost 30 characters long")]
        public String AUTH_STATUS { get; set; }

        
    }


    public partial class ACC_SCREEN_DESIGN_PARAM_DETAILS_CUSTOM
    {
        public short TXN_ID { get; set; }
        public string CON_REQ { get; set; }
        public string AGREEMENT_REQ { get; set; }
        public string ROAD_REQ { get; set; }
        public string PIU_REQ { get; set; }
        public string SUPPLIER_REQ { get; set; }
        public string YEAR_REQ { get; set; }
        public string PKG_REQ { get; set; }
        public short? MASTER_TXN_ID { get; set; }
        public bool SHOW_CON_AT_TRANSACTION { get; set; }
        public virtual ACC_MASTER_TXN ACC_MASTER_TXN { get; set; }
    }

    public class transactionList
    { 
                public string BILL_FINALIZED { get; set; }
                public long BILL_ID { get; set; }
                public string HEAD_ID { get; set; }
                public string HEAD_ID_Narration { get; set; }
                public string CASH_CHQ { get; set; }
                public Nullable<decimal> AMOUNT { get; set; }
                public string NARRATION { get; set; }
                public short TXN_NO { get; set; }
                public short TXN_ID { get; set; }
                public short MASTER_TXN_ID { get; set; }
                public short SERIAL_No { get; set; }
                public string AGREEMENT_CODE { get; set; }
                public string RODE_CODE { get; set; }
                public string MASTER_CHQEPAY { get; set; }
                public string ONLY_DEDUCTION { get; set; }
                public String paymentType { get; set; }
                public String FINALPAYMENT    { get; set; }
                public String PIUNAME { get; set; }
                public String CONTRACTORNAME { get; set; }
                public Boolean isOperational { get; set; }
                public Boolean isRequiredAfterPorting { get; set; }


    }
    
    public class masterpageTransactionDesignParam {
     
       public string CASH_CHQ { get; set; }
       public string DED_REQ { get; set; }
       public string EPAY_REQ { get; set; }
       public string BAR_REQ { get; set; }
       public string REM_REQ { get; set; }
       public string MTXN_REQ { get; set; }
       public string MAST_CON_REQ { get; set; }
       public string MAST_AGREEMENT_REQ { get; set; }
       public string MAST_SUPPLIER_REQ { get; set; }
     }
    
    public class PaymentDetailsModel
    {
        public PaymentDetailsModel()
        {

          

        }

        [Display(Name = "Company Name (Contractor)")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "invalid Company (Contractor)")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Invalid Company (Contractor)")]
        public Nullable<int> MAST_CON_ID_CON { get; set; }
        public List<SelectListItem> mast_CON_ID_CON1 { get; set; }
        
        public long BILL_ID { get; set; }
        public short TXN_NO { get; set; }
        public string HEAD_ID { get; set; }
        public Nullable<decimal> AMOUNT { get; set; }

        [Display(Name = "Sub Transaction Type (Payment)")]
        //[Required(ErrorMessage = "Sub Transaction is Required")]
        public string HEAD_ID_P { get; set; }
        public List<SelectListItem> HeadId_P { get; set; }

        [Display(Name = "Sub Transaction Type (Deduction)")]
        //[Required(ErrorMessage = "Sub Transaction is Required")]
        public string HEAD_ID_D { get; set; }
        public List<SelectListItem> HeadId_D { get; set; }

        [Display(Name = "Deduction Amount")]
       // [Required(ErrorMessage = "Deduction Amount is Required")]
        [RegularExpression(@"^\d{0,11}(\.\d{0,2})?$", ErrorMessage = "Only 11 Digit Whole number and 2 Decimal Allowed")]
        public Nullable<decimal> AMOUNT_D { get; set; }

        [Display(Name = "Cheque Amount")]
        [RegularExpression(@"^\d{0,11}(\.\d{0,2})?$", ErrorMessage = "Only 11 Digit Whole number and 2 Decimal Allowed")]
        public Nullable<decimal> AMOUNT_Q { get; set; }

        [Display(Name = "Cash Amount")]
        [RegularExpression(@"^\d{0,11}(\.\d{0,2})?$", ErrorMessage = "Only 11 Digit Whole number and 2 Decimal Allowed")]
        public Nullable<decimal> AMOUNT_C { get; set; }
        
        public string CREDIT_DEBIT { get; set; }
       
        public string CASH_CHQ { get; set; }

        [Display(Name = "Sanction Year")]
        public int SANCTION_YEAR { get; set; }
        public List<SelectListItem> IMS_SANCTION_YEAR_List { get; set; }


        [Display(Name = "Sanction Packages")]
        public String SANCTION_PACKAGE { get; set; }
        public List<SelectListItem> IMS_SANCTION_PACKAGE_List { get; set; }


        [Display(Name = "Narration")]
       // [Required(ErrorMessage = "Narration is Required")]
        [StringLength(255, ErrorMessage = "Maximum 255 Characters Allowed")]
        //[RegularExpression(@"^[a-zA-Z0-9-/()., ]+$", ErrorMessage = "Only Alphanumeric, Space and '-','/','.',',','(',')' Allowed")]

        [RegularExpression("^([a-zA-Z0-9 -.,/\r\n()]+)", ErrorMessage = "Only Alphanumeric, Space and '-','/','.','(',')',',' Allowed")]//added by abhishek kamble 11-nov-2013

        public string NARRATION_P { get; set; }


        [Display(Name = "Narration")]
       // [Required(ErrorMessage = "Narration is Required")]
        [StringLength(255, ErrorMessage = "Maximum 255 Characters Allowed")]
        [RegularExpression(@"^[a-zA-Z0-9-/()., ]+$", ErrorMessage = "Only Alphanumeric, Space and '-','/','.',',','(',')' Allowed")]
        public string NARRATION_D { get; set; }

        [Display(Name = "Road ")]
        public Nullable<int> IMS_PR_ROAD_CODE { get; set; }
        public List<SelectListItem> IMS_PR_ROAD_CODEList { get; set; }

        [Display(Name = "Agreement Name")]
        //[Required(ErrorMessage = "Agreement Name is Required")]
        //[Range(1, 2147483647, ErrorMessage = "Please Select Agreement")]
        public Nullable<int> IMS_AGREEMENT_CODE { get; set; }
      
        
        [Display(Name = "Agreement Name (Supplier)")]
        public Nullable<int> IMS_AGREEMENT_CODE_S { get; set; }
        public List<SelectListItem> AGREEMENT_S { get; set; }
         
        [Display(Name = "Agreement Name (Contractor) ")]
        public Nullable<int> IMS_AGREEMENT_CODE_C { get; set; }
        public List<SelectListItem> AGREEMENT_C { get; set; }

         [Display(Name = " Implementing Agency / Department ")]
         public Nullable<int> MAST_DPIU_CODE { get; set; }
         public List<SelectListItem> MAST_DPIU_CODEList { get; set; }

        [Display(Name = "District ")]
         public Nullable<int> MAST_DISTRICT_CODE { get; set; }
         public List<SelectListItem> MAST_DISTRICT_CODEList { get; set; }


        
         [Display(Name = "Is Final Payment ")]
          public Nullable<bool> FINAL_PAYMENT { get; set; }
      
        public List<SelectListItem> final_pay { get; set; }

        public Nullable<int> ND_CODE { get; set; }
         
        public Nullable<int> CON_ID { get; set; }


        [Display(Name = "Agreement Name (Deduction)")]
        public Nullable<int> IMS_AGREEMENT_CODE_DED { get; set; }
        public List<SelectListItem> AGREEMENT_DED{ get; set; }

        public string CON_REQ { get; set; }
        public string AGREEMENT_REQ { get; set; }
        public string ROAD_REQ { get; set; }
        public string PACKAGE_REQ { get; set; }
        public string PIU_REQ { get; set; }
        public string SUPPLIER_REQ { get; set; }
        public string FUND_TYPE { get; set; }
        public short LVL_ID { get; set; }
        public string BILL_TYPE { get; set; }

        public Nullable<int> CONTRACTOR_ID { get; set; }


        public virtual ACC_BILL_MASTER ACC_BILL_MASTER { get; set; }
        public virtual ACC_MASTER_HEAD ACC_MASTER_HEAD { get; set; }
        public virtual MASTER_CONTRACTOR MASTER_CONTRACTOR { get; set; }
    }
}