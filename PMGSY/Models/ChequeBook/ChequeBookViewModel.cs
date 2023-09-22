using PMGSY.BLL.Common;
using PMGSY.Common;
using PMGSY.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.ChequeBook
{
    public class ChequeBookViewModel
    {
        public ChequeBookViewModel()
        {
            CommonFunctions objCommon = new CommonFunctions();
            this.CURRENT_DATE = objCommon.GetDateTimeToString(DateTime.Now);
        }

        [ScaffoldColumn(false)]
        public int CHQ_BOOK_ID { get; set; }

        [Display(Name = "Start Leaf")]
        [Required(ErrorMessage = "Start Leaf is Required")]
        [RegularExpression(@"^\d{1,6}?$", ErrorMessage = "Only Numeric Allowed upto six digits")]
        public string LEAF_START { get; set; }

        [Display(Name = "End Leaf")]
        [Required(ErrorMessage = "End Leaf is Required")]
        [IsStartLeafGreater("LEAF_START", true, ErrorMessage = "End Leaf must be greater than Start Leaf")]
        [RegularExpression(@"^\d{1,6}?$", ErrorMessage = "Only Numeric Allowed upto six digits")]
        public string LEAF_END { get; set; }

        [ScaffoldColumn(false)]
        public string FUND_TYPE { get; set; }

        [Display(Name = "Cheque Issue Date")]
        [Required(ErrorMessage = "Cheque Issue Date is Required")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Please Enter valid Cheque Issue date")]
        [IsDateAfter("CURRENT_DATE", true, ErrorMessage = "Cheque Book Issue Date must be less than or equal to today's date")]
        [IsBankDetails(ErrorMessage = "Bank Details not Present")]
        [IsDateBeforeAccDate("ACC_OPEN_DATE", true, ErrorMessage = "Cheque issue date must be greater than bank account opening date.")]
        public string ISSUE_DATE { get; set; }

        [ScaffoldColumn(false)]
        public short BANK_CODE { get; set; }

        [ScaffoldColumn(false)]
        [Display(Name="DPIU")]
        [Range(1,Int32.MaxValue,ErrorMessage="Please select DPIU")]
        public int ADMIN_ND_CODE { get; set; }


        //Added By Abhishek kamble 4Feb2015 to show dpiu at SRRDA level
        public SelectList PopulateDPIU
        {
            get {
                CommonFunctions objComm = new CommonFunctions();
                List<SelectListItem> lstDPIU = objComm.PopulateDPIUOfSRRDA(PMGSYSession.Current.AdminNdCode);
                return new SelectList(lstDPIU,"Value","Text");            
            }        
        }

        [ScaffoldColumn(false)]
        public Int16 LVL_ID { get; set; }

        public string CURRENT_DATE { get; set; }

        public string ACC_OPEN_DATE { get; set; }

        //Added By Abhishek kamble To Add Cheque book at SRRDA level for AF
        public String IsSRRDADpiu { get; set; }

    }

    public class IsStartLeafGreater : ValidationAttribute, IClientValidatable
    {
        private readonly string testedPropertyName;

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name);
        }

        public IsStartLeafGreater(string testedPropertyName, bool allowEqualDates = false)
        {
            this.testedPropertyName = testedPropertyName;
        }

        protected override ValidationResult IsValid(object firstValue, ValidationContext validationContext)
        {
            Int32 leafEnd = Convert.ToInt32(firstValue);
            var propertyTestedInfo = validationContext.ObjectType.GetProperty(this.testedPropertyName);
            if (propertyTestedInfo == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.testedPropertyName));
            }

            Int32 leafStart = Convert.ToInt32(propertyTestedInfo.GetValue(validationContext.ObjectInstance, null));

            if (leafEnd > leafStart)
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
                ValidationType = "isstartleafgreater"
            };
            rule.ValidationParameters["propertytested"] = this.testedPropertyName;
            yield return rule;
        }
    }

    public class IsBankDetails : ValidationAttribute, IClientValidatable
    {
        private readonly Int16? BankCode;


        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name);
        }

        public IsBankDetails()
        {
            //old
            //if (PMGSYSession.Current.LevelId == 4)
            //{
            //    BankCode = dbContext.ACC_BANK_DETAILS.Where(m => m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && m.FUND_TYPE == PMGSYSession.Current.FundType && m.BANK_ACC_STATUS == true).Select(m => m.BANK_CODE).FirstOrDefault();
            //}
            //else
            //{
            //    BankCode = dbContext.ACC_BANK_DETAILS.Where(m => m.ADMIN_ND_CODE == (dbContext.ADMIN_DEPARTMENT.Where(p => p.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && m.BANK_ACC_STATUS == true).Select(p => p.MAST_PARENT_ND_CODE).FirstOrDefault()) && m.FUND_TYPE == PMGSYSession.Current.FundType).Select(m => m.BANK_CODE).FirstOrDefault();
            //}

            //new
            if (PMGSYSession.Current.LevelId == 4)
            {
                BankCode = getBankCode();
            }
            else
            {
                BankCode = getBankCode();
            }

            //if (PMGSYSession.Current.DistrictCode == 0)
            //{
            //    BankCode = dbContext.ACC_BANK_DETAILS.Where(m => m.MAST_STATE_CODE == PMGSYSession.Current.StateCode).Select(m => m.BANK_CODE).FirstOrDefault();
            //}
            //else
            //{
            //    BankCode = dbContext.ACC_BANK_DETAILS.Where(m => m.MAST_STATE_CODE == PMGSYSession.Current.StateCode && m.MAST_DISTRICT_CODE == PMGSYSession.Current.DistrictCode).Select(m => m.BANK_CODE).FirstOrDefault();
            //}
        }

        protected override ValidationResult IsValid(object firstValue, ValidationContext validationContext)
        {
            if (BankCode == null)
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }

            return ValidationResult.Success;
        }


        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "isbankdetails"
            };
            //rule.ValidationParameters["bankcode"] = this.BankCode;
            rule.ValidationParameters["bankcode"] = getBankCode();

            yield return rule;
        }


        public short getBankCode()
        {
            PMGSYEntities dbContext = new PMGSYEntities();

            try
            {
                if (PMGSYSession.Current.LevelId == 4)
                {
                    return dbContext.ACC_BANK_DETAILS.Where(m => m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && m.FUND_TYPE == PMGSYSession.Current.FundType && m.BANK_ACC_STATUS == true && m.ACCOUNT_TYPE == "S").Select(m => m.BANK_CODE).FirstOrDefault();
                }
                else
                {
                    return dbContext.ACC_BANK_DETAILS.Where(m => m.ADMIN_ND_CODE == (dbContext.ADMIN_DEPARTMENT.Where(p => p.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && m.BANK_ACC_STATUS == true && m.ACCOUNT_TYPE == "S").Select(p => p.MAST_PARENT_ND_CODE).FirstOrDefault()) && m.FUND_TYPE == PMGSYSession.Current.FundType).Select(m => m.BANK_CODE).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Models.ChequeBook.ChequeBookViewModel.getBankCode()");
                throw;
            }
            finally {
                if (dbContext != null)
                    dbContext.Dispose();
            }
          
        }
    }


    public class IsDateBeforeAccDate : ValidationAttribute//, IClientValidatable
    {
        private readonly string testedPropertyName;
        private readonly bool allowEqualDates;
        CommonFunctions objCommon = new CommonFunctions();
        public IsDateBeforeAccDate(string testedPropertyName, bool allowEqualDates = false)
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

            if (value != null)
            {
                // Compare values
                if (objCommon.GetStringToDateTime(value.ToString()) >= objCommon.GetStringToDateTime(propertyTestedValue.ToString()))
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

        //public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        //{
        //    var rule = new ModelClientValidationRule
        //    {
        //        ErrorMessage = this.ErrorMessageString,
        //        ValidationType = "isdateafter"
        //    };
        //    rule.ValidationParameters["propertytested"] = this.testedPropertyName;
        //    rule.ValidationParameters["allowequaldates"] = this.allowEqualDates;
        //    yield return rule;
        //}

    }


}