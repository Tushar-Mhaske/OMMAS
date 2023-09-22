#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   ExecutionProgressViewModel.cs
        * Description   :   This View Model is Used in CBR Views AddFinancialProgress.cshtml
        * Author        :   Vikram Nandanwar        
        * Creation Date :   25/June/2013
 **/
#endregion

using PMGSY.BAL.Execution;
using PMGSY.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Execution
{
    public class ExecutionProgressViewModel
    {
        public string EncyptedProgressCode { get; set; }

        public string Operation { get; set; }

        public int IMS_PR_ROAD_CODE { get; set; }

        public string EXEC_PROGRESS_TYPE { get; set; }

        [Required(ErrorMessage = "Please Select Year")]
        [Range(2000, 3000, ErrorMessage = "Please Select Proper Year.")]
        [CompareAgrementYear("AgreementYear", "AgreementDate", ErrorMessage = "Year must be greater than or equal to Agreement Year")]
        [Display(Name = "Year")]
        public int EXEC_PROG_YEAR { get; set; }

        [Required(ErrorMessage = "Please Select Month")]
        [Range(1, 12, ErrorMessage = "Please Select Proper Month")]
        [CompareAgrementMonth("EXEC_PROG_YEAR", "AgreementDate", ErrorMessage = "Month must be greater than or equal to Agreement Month")]
        [Display(Name = "Month")]
        public int EXEC_PROG_MONTH { get; set; }


        [Display(Name = "Value of Work Upto Last Month(Rs. in Lakh)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Invalid Value of Work,Can only contains 8 Numeric digits and 2 digit after decimal place.")]
        //[Range(0, 99999999.99, ErrorMessage = "Invalid Value of Work,Can only contains 8 Numeric digits and 2 digit after decimal place.")]
        public Nullable<decimal> EXEC_VALUEOFWORK_LASTMONTH { get; set; }

        [Required(ErrorMessage = "Please enter value of work during this month.")]
        [Display(Name = "Value of Work During This Month(Rs. in Lakh)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Invalid Value of work,Can only contains 8 Numeric digits and 2 digit after decimal place")]
        [Range(0, 99999999.99, ErrorMessage = "Invalid Value of Work,Can only contains 8 Numeric digits and 2 digit after decimal place.")]
        public Nullable<decimal> EXEC_VALUEOFWORK_THISMONTH { get; set; }

        [Display(Name = "Payment of Work Upto Last Month(Rs. in Lakh)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Invalid Value of payment,Can only contains 8 Numeric digits and 2 digit after decimal place")]
        //[Range(0, 99999999.99, ErrorMessage = "Invalid Value of Payment,Can only contains 8 Numeric digits and 2 digit after decimal place.")]
        public Nullable<decimal> EXEC_PAYMENT_LASTMONTH { get; set; }

        [Required(ErrorMessage = "Please enter payment of work during this month.")]
        [Display(Name = "Payment of Work During This Month(Rs. in Lakh)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Invalid Value of payment,Can only contains Numeric values and 2 digit after decimal place")]
        [Range(0, 99999999.99, ErrorMessage = "Invalid Value of Payment,Can only contains 8 Numeric digits and 2 digit after decimal place.")]
        //[CompareFinancialValidationWork("EXEC_VALUEOFWORK_THISMONTH", "IMS_PR_ROAD_CODE", ErrorMessage = "Total amount of Payment should not be greater than total value of Work Done.")]
        [CompareFinancialValidationWork("EXEC_VALUEOFWORK_LASTMONTH", "EXEC_PAYMENT_LASTMONTH", "EXEC_VALUEOFWORK_THISMONTH", ErrorMessage = "Total amount of Payment should not be greater than total value of Work Done.")]
        public Nullable<decimal> EXEC_PAYMENT_THISMONTH { get; set; }

        [Required(ErrorMessage = "Please select yes or no.")]
        [RegularExpression(@"^([YN]+)$", ErrorMessage = "Please select yes or no.")]
        [Display(Name = "Is Final Payment")]
        public string EXEC_FINAL_PAYMENT_FLAG { get; set; }

        [Display(Name = "Final Payment Date")]
        [CompareFinalPaymentDate("EXEC_PROG_YEAR", "EXEC_PROG_MONTH", "EXEC_FINAL_PAYMENT_FLAG", ErrorMessage = "Month and Year must be same as Payment date.")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Final Payment Date must be in dd/mm/yyyy format.")]
        public string EXEC_FINAL_PAYMENT_DATE { get; set; }

        public virtual IMS_SANCTIONED_PROJECTS IMS_SANCTIONED_PROJECTS { get; set; }

        public string BlockName { get; set; }

        public string Package { get; set; }

        public int RoadNo { get; set; }

        public string RoadName { get; set; }

        public double Sanction_Cost { get; set; }

        public double MaintananceCost { get; set; }

        public decimal Sanction_length { get; set; }

        //[ComparePreviousFinancialStatus("EXEC_FINAL_PAYMENT_FLAG","Operation",ErrorMessage="Final payment is already made.")]
        public string CompleteStatus { get; set; }

        [CompareSanctionValue("AgreementCost", "IMS_PR_ROAD_CODE", "EXEC_VALUEOFWORK_THISMONTH", "EXEC_PAYMENT_THISMONTH", "Operation", ErrorMessage = "Total value of work must not exceeds the agreement cost + 20% of agreement cost.")]
        public decimal? TotalValueofwork { get; set; }

        [CompareSanctionValue("AgreementCost", "IMS_PR_ROAD_CODE","EXEC_VALUEOFWORK_THISMONTH", "EXEC_PAYMENT_THISMONTH", "Operation", ErrorMessage = "Total payment must not exceeds the agreement cost + 20% of agreement cost.")]
        public decimal? TotalPayment { get; set; }

        [CompareMonth("EXEC_PROG_MONTH", "EXEC_PROG_YEAR", "PreviousYear","Operation", ErrorMessage = "Month and Year must be greater than previous entered month and year")]
        public int PreviousMonth { get; set; }

        public int PreviousYear { get; set; }

        public decimal? AgreementTotal { get; set; }

        public decimal? LastMonthValue { get; set; }

        public decimal? LastPaymentValue { get; set; }

        public decimal? AdditionalCost { get; set; }

        //[CompareAgreementYear("EXEC_PROG_YEAR",ErrorMessage="Year must be greater than Agreement Year.")]
        public int AgreementYear { get; set; }

        public string AgreementDate { get; set; }

        public int AgreementMonth { get; set; }

        public decimal? AgreementCost { get; set; }

        public string IsFinalPaymentBefore { get; set; }
    }

    public class CompareFinancialValidationWork : ValidationAttribute , IClientValidatable
    {
        private readonly string PropertyName;
        private readonly string PropertyRoad;
        private readonly string PropertyValuethismonth;

        public CompareFinancialValidationWork(string propertyName, string propertyRoad, string propertyvaluethisMonth)
        {
            this.PropertyName = propertyName;
            this.PropertyRoad = propertyRoad;
            this.PropertyValuethismonth = propertyvaluethisMonth;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyTestedInfo = validationContext.ObjectType.GetProperty(this.PropertyName);
            var propertyTestedRoad = validationContext.ObjectType.GetProperty(this.PropertyRoad);
            var propertyValueThisMonth = validationContext.ObjectType.GetProperty(this.PropertyValuethismonth);
            if (propertyTestedInfo == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.PropertyName));
            }

            var workValue = Convert.ToDecimal(propertyTestedInfo.GetValue(validationContext.ObjectInstance, null));
            //int proposalCode = Convert.ToInt32(propertyTestedRoad.GetValue(validationContext.ObjectInstance, null));
            var totalpaymentValue = Convert.ToDecimal(propertyTestedRoad.GetValue(validationContext.ObjectInstance, null));
            var totalValueThisMonth = Convert.ToDecimal(propertyValueThisMonth.GetValue(validationContext.ObjectInstance, null));
            var paymentValue = Convert.ToDecimal(value);
            ExecutionBAL objBAL = new ExecutionBAL();
            //var totalValueOfWorkDone = objBAL.CalculateTotalValueOfWorkDone(proposalCode);
            //var totalValueOfPayment = objBAL.CalculateTotalValueOfPayment(proposalCode);

            //if (paymentValue != null)
            //{
            //    totalValueOfPayment = totalValueOfPayment + paymentValue;
            //}

            //if (workValue != null)
            //{
            //    totalValueOfWorkDone = totalValueOfWorkDone + workValue;
            //}

            decimal totalValueOfWorkDone = totalValueThisMonth + workValue;// == 0 ? totalValueThisMonth : workValue;
            decimal totalValueOfPayment = paymentValue + totalpaymentValue;// == 0 ? paymentValue : totalpaymentValue;
            

            if (totalValueOfWorkDone < totalValueOfPayment)
            {
                if (paymentValue == null)
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
                ValidationType = "comparefinancialvalidationwork"
            };
            //rule.ValidationParameters["compareworkpayment"] = this.PropertyName;
            yield return rule;
        }
    }

    public class CompareSanctionValue : ValidationAttribute , IClientValidatable
    {
        private readonly string PropertyName;
        private readonly string PropertyProposalCode;
        //private readonly string PropertyProgressType;
        private readonly string PropertyValue;
        private readonly string PropertyPayment;
        private readonly string PropertyOperation;

        public CompareSanctionValue(string propertyName,string propertyProposalCode,string propertyValue,string propertyPayment,string propertyOperation)
        {
            this.PropertyName = propertyName;
            this.PropertyProposalCode = propertyProposalCode;
            //this.PropertyProgressType = propertyProgressType;
            this.PropertyValue = propertyValue;
            this.PropertyPayment = propertyPayment;
            this.PropertyOperation = propertyOperation;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyTestedInfo = validationContext.ObjectType.GetProperty(this.PropertyName);
            var propertyTestedCode = validationContext.ObjectType.GetProperty(this.PropertyProposalCode);
            //var propertyTestedType = validationContext.ObjectType.GetProperty(this.PropertyProgressType);
            var propertyTestedValue = validationContext.ObjectType.GetProperty(this.PropertyValue);
            var propertyTestedPayment = validationContext.ObjectType.GetProperty(this.PropertyPayment);
            var propertyTestedOperation = validationContext.ObjectType.GetProperty(this.PropertyOperation);
            if (propertyTestedInfo == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.PropertyName));
            }

            int proposalCode = Convert.ToInt32(propertyTestedCode.GetValue(validationContext.ObjectInstance, null));
            //string progressType = propertyTestedType.GetValue(validationContext.ObjectInstance, null).ToString();
            decimal valueOfWork = Convert.ToDecimal(propertyTestedValue.GetValue(validationContext.ObjectInstance, null));
            decimal valueOfPayment = Convert.ToDecimal(propertyTestedPayment.GetValue(validationContext.ObjectInstance, null));
            string operation = propertyTestedOperation.GetValue(validationContext.ObjectInstance, null).ToString();

            IExecutionBAL objBAL = new ExecutionBAL();
            bool status = objBAL.CheckSanctionValue(proposalCode,valueOfWork,valueOfPayment,operation);
            if (status)
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
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "comparesanctionvalue"
            };
            rule.ValidationParameters["sanctionvalue"] = this.PropertyName;
            yield return rule;
        }
    }


    public class CompareAgreementYear : ValidationAttribute , IClientValidatable
    {
        private readonly string PropertyYear;

        public CompareAgreementYear(string propertyYear)
        {
            this.PropertyYear = propertyYear;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyTestedInfo = validationContext.ObjectType.GetProperty(this.PropertyYear);
            if (propertyTestedInfo == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.PropertyYear));
            }

            var Year = Convert.ToInt32(propertyTestedInfo.GetValue(validationContext.ObjectInstance, null));
            if (Year == null)
            {
                return ValidationResult.Success;
            }

            if (value == null)
            {
                return ValidationResult.Success;
            }

            if (Year < Convert.ToInt32(value))
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
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
                ValidationType = "compareagreementyear"
            };
            rule.ValidationParameters["year"] = this.PropertyYear;
            yield return rule;
        }
    }


    public class ComparePreviousFinancialStatus : ValidationAttribute //, IClientValidatable
    {
        private readonly string PropertyName;
        private readonly string PropertyOperation;


        public ComparePreviousFinancialStatus(string propertyName, string propertyOperation)
        {
            this.PropertyName = propertyName;
            this.PropertyOperation = propertyOperation;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyTestedInfo = validationContext.ObjectType.GetProperty(this.PropertyName);
            var propertyTestedOperation = validationContext.ObjectType.GetProperty(this.PropertyOperation);

            if (propertyTestedInfo == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.PropertyName));
            }

            var existingstatus = propertyTestedInfo.GetValue(validationContext.ObjectInstance, null);
            var operation = propertyTestedOperation.GetValue(validationContext.ObjectInstance, null);
            if (operation.ToString().ToLower() == "e")
            {
                return ValidationResult.Success;
            }

            if (value == null)
            {
                return ValidationResult.Success;
            }
            var newStatus = value;

            if (newStatus.ToString().ToLower() == "c")
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
                ValidationType = "comparepreviousstatus"
            };
            yield return rule;
        }
    }


    public class CompareFinalPaymentDate : ValidationAttribute, IClientValidatable
    {
        private readonly string PropertyYear;
        private readonly string PropertyMonth;
        private readonly string PropertyFlag;


        public CompareFinalPaymentDate(string propertyYear, string propertyMonth, string propertyFlag)
        {
            this.PropertyYear = propertyYear;
            this.PropertyMonth = propertyMonth;
            this.PropertyFlag = propertyFlag;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //try
            //{
                CommonFunctions objCommon = new CommonFunctions();
                var propertyTestedInfo = validationContext.ObjectType.GetProperty(this.PropertyYear);
                var propertyTestedMonth = validationContext.ObjectType.GetProperty(this.PropertyMonth);
                var propertyTestedFlag = validationContext.ObjectType.GetProperty(this.PropertyFlag);

                if (propertyTestedInfo == null)
                {
                    return new ValidationResult(string.Format("unknown property {0}", this.PropertyYear));
                }

                int Year = Convert.ToInt32(propertyTestedInfo.GetValue(validationContext.ObjectInstance, null));
                int month = Convert.ToInt32(propertyTestedMonth.GetValue(validationContext.ObjectInstance, null));
                var flag = propertyTestedFlag.GetValue(validationContext.ObjectInstance, null);

                if (Year == System.DateTime.Now.Year)
                {
                    if (month > System.DateTime.Now.Month)
                    {
                        return new ValidationResult("Month and Year should not be greater than today's month and year.");
                    }
                }

                if (flag.ToString().ToLower() == "n")
                {
                    return ValidationResult.Success;
                }

                //DateTime paymentDate = Convert.ToDateTime(value.ToString());
                DateTime paymentDate = objCommon.GetStringToDateTime(value.ToString());

                if (paymentDate > System.DateTime.Now)
                {
                    return new ValidationResult(FormatErrorMessage("Final Payment Date should not be greater than today's date."));
                }


                if (Year == paymentDate.Year && month == paymentDate.Month)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception("test");
            //}
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "comparefinaldate"
            };
            rule.ValidationParameters["year"] = this.PropertyYear;
            yield return rule;
        }
    }


}