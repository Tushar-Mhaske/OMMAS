#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   ExecutingOfficerViewModel.cs
        * Description   :   This View Model is Used in CBR Views AddPhysicalLSBProgress.cshtml
        * Author        :   Vikram Nandanwar        
        * Creation Date :   25/June/2013
 **/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Execution
{
    public class ExecutionLSBStatusViewModel
    {
        public string EncryptedLSBCode { get;set;}

        public int IMS_PR_ROAD_CODE { get; set; }

        [Required(ErrorMessage="Please select year")]
        [Display(Name="Year")]
        [CompareAgrementYear("AgreementYear", "AgreementDate", ErrorMessage = "Year must be greater than or equal to Agreement Year")]
        [Range(2000,3000,ErrorMessage="Please select year.")]
        public int EXEC_PROG_YEAR { get; set; }

        [Required(ErrorMessage="Please select month")]
        [Range(1,12,ErrorMessage="Please select month.")]
        [CompareAgrementMonth("EXEC_PROG_YEAR", "AgreementDate", ErrorMessage = "Month must be greater than or equal to Agreement Month")]
        [Display(Name="Month")]
        public int EXEC_PROG_MONTH { get; set; }

        [Display(Name="Cutoff/Raft/Individual footing")]
        [Range(0, 99999.999, ErrorMessage = "Invalid Cutoff/Raft/Individual footing,Can only contains 5 Numeric values and 3 digit after decimal place")]
        public Nullable<decimal> EXEC_RAFT { get; set; }

        [Display(Name="Floor Protection(in Mtrs.)")]
        [Range(0, 99999.999, ErrorMessage = "Invalid Floor Protection,Can only contains 5 Numeric values and 3 digit after decimal place")]
        public Nullable<decimal> EXEC_FLOOR_PROTECTION { get; set; }

        //[Display(Name="Sinking(in Mtrs.)")]
        [Display(Name = "Sinking(in Mtrs.)/Pile Length (in Mtrs.)")]
        [Range(0, 99999.999, ErrorMessage = "Invalid Sinking,Can only contains 5 Numeric values and 3 digit after decimal place")]
        public Nullable<decimal> EXEC_SINKING { get; set; }

        //[Display(Name="Bottom Pluggings(in Nos.)")]
        [Display(Name = "Bottom Pluggings(in Nos.)/Piles (in Nos.)")]
        [RegularExpression(@"\d+$", ErrorMessage = "Please enter valid value.")]
        [Range(0, 999, ErrorMessage = "Invalid Bottom Pluggings(in nos.)")]
        public Nullable<int> EXEC_BOTTOM_PLUGGING { get; set; }

        [Display(Name="Top Pluggings(in Nos.)")]
        [RegularExpression(@"\d+$", ErrorMessage = "Please enter valid value.")]
        [Range(0, 999, ErrorMessage = "Invalid Top Pluggings(in nos.)")]
        public Nullable<int> EXEC_TOP_PLUGGING { get; set; }

        //[Display(Name="Well Caps(in Nos.)")]
        [Display(Name = "Well Caps(in Nos.)/Pile Caps (in Nos.)")]
        [RegularExpression(@"\d+$",ErrorMessage="Please enter valid value.")]
        [Range(0, 999, ErrorMessage = "Invalid Well Caps(in nos.)")]
        public Nullable<int> EXEC_WELL_CAP { get; set; }

        [Display(Name="Pier/Abutment Shaft(in Mtrs.)")]
        [Range(0, 99999.999, ErrorMessage = "Invalid Pier/Abutment Shaft,Can only contains 5 Numeric values and 3 digit after decimal place")]
        public Nullable<decimal> EXEC_PIER_SHAFT { get; set; }

        [Display(Name="Pier/Abbutment Caps(in Nos.)")]
        [RegularExpression(@"\d+$", ErrorMessage = "Please enter valid value.")]
        [Range(0, 999, ErrorMessage = "Invalid Pier/Abbutment Caps(in nos.)")]
        public Nullable<int> EXEC_PIER_CAP { get; set; }

        [Display(Name="Bearings(in Nos.)")]
        [RegularExpression(@"\d+$", ErrorMessage = "Please enter valid value.")]
        [Range(0, 999, ErrorMessage = "Invalid Bearings(in nos.)")]
        public Nullable<int> EXEC_BEARINGS { get; set; }

        [Display(Name="Deck Slab(in Nos.)")]
        [RegularExpression(@"\d+$", ErrorMessage = "Please enter valid value.")]
        [Range(0, 999, ErrorMessage = "Invalid Deck Slab(in nos.)")]
        public Nullable<int> EXEC_DECK_SLAB { get; set; }

        [Display(Name="Wearing Coat(in Mtrs.)")]
        [Range(0, 99999.999, ErrorMessage = "Invalid Wearing Coat,Can only contains 5 Numeric values and 3 digit after decimal place")]
        public Nullable<decimal> EXEC_WEARING_COAT { get; set; }

        [Display(Name="Posts & Railling(in Mtrs.)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "Invalid Posts & Railing,Can only contains Numeric values and 0 digit after decimal place")]
        [Range(0, 99999.999, ErrorMessage = "Invalid Posts & Railling,Can only contains 5 Numeric values and 3 digit after decimal place")]
        public Nullable<decimal> EXEC_POSTS_RAILING { get; set; }

        [Display(Name="Road Work(in Mtrs.)")]
        [Range(0, 99999.999, ErrorMessage = "Invalid Road Work,Can only contains 5 Numeric values and 3 digit after decimal place")]
        public Nullable<decimal> EXEC_APP_ROAD_WORK { get; set; }

        [Display(Name="CDWorks(in Nos.)")]
        [RegularExpression(@"\d+$", ErrorMessage = "Please enter valid value.")]
        [Range(0, 999, ErrorMessage = "Invalid CDWorks(in nos.)")]
        public Nullable<int> EXEC_APP_CD_WORKS { get; set; }

        [Required]
        [Display(Name="Bridge Length Completed(in Mtrs.)")]
        //[CompareSanctionLength("Bridge_Length", ErrorMessage = "Bridge Length Completed Value must be at least 10% of sanction length")]
        [Range(0, 99999.999, ErrorMessage = "Invalid Bridge Length Completed,Can only contains 5 Numeric values and 3 digit after decimal place")]
        public Nullable<decimal> EXEC_BRIDGE_COMPLETED { get; set; }

        [Required]
        [Display(Name="Approach Work Completed(in Mtrs.)")]
        //[CompareSanctionLength("Bridge_Length", ErrorMessage = "Approach Work Completed Value must be at least 10% of sanction length")]
        //[Range(0, 99999.999, ErrorMessage = "Invalid Approach Work Completed,Can only contains 5 Numeric values and 3 digit after decimal place")]
        [Range(0, 499.999, ErrorMessage = "Invalid Approach Work Completed,Can only contains 3 Numeric values upto 500 and 3 digit after decimal place")]
        public Nullable<decimal> EXEC_APP_COMPLETED { get; set; }
        
        [Required(ErrorMessage="Please select work status.")]
        [Display(Name="Work Status")]
        [RegularExpression(@"^([CPFAL]+)$", ErrorMessage = "Please select work status.")]
        [IsSplitWork("IMS_PR_ROAD_CODE", ErrorMessage = "The agreement has not been made against this road or the work is incomplete against this road.You can not select the status Completed.")]
        public string EXEC_ISCOMPLETED { get; set; }

        public string Operation { get; set; }

        public string BlockName { get; set; }

        public string Package { get; set; }

        public int RoadNo { get; set; }

        public string RoadName { get; set; }

        public double Sanction_Cost { get; set; }
        
        public decimal Sanction_length { get; set; }

        //LSB Change 05NOV2019
        public decimal changedLength { get; set; }
        //public decimal changed_SanctionedLength { get; set; }


        [ComparePreviousStatusLSB("EXEC_ISCOMPLETED","Operation", ErrorMessage = "Work status is already completed.")]
        public string CompleteStatus { get; set; }

        //[CompareLengthLSB("EXEC_BRIDGE_COMPLETED", "EXEC_APP_COMPLETED", "EXEC_ISCOMPLETED", ErrorMessage = "Bridge Length Completed/Approach Road Completed must be in between (Sanctioned Length - 10% Sanctioned length) and (Sanctioned Length + 10% Sanctioned length).")]
        [CompareLengthLSB("EXEC_BRIDGE_COMPLETED", "EXEC_APP_COMPLETED", "EXEC_ISCOMPLETED", "changedLength", ErrorMessage = "Bridge Length Completed/Approach Road Completed must be upto (Sanctioned Length + 10% Sanctioned length).")]
        //[CompareCompleteSanctionLength("EXEC_ISCOMPLETED", "EXEC_BRIDGE_COMPLETED", ErrorMessage = "Bridge length must be equal to sanction length.")]
        public decimal? Bridge_Length { get; set; }

        public decimal? Bridge_Cost { get; set; }

        [CompareMonth("EXEC_PROG_MONTH", "EXEC_PROG_YEAR", "previousYear","Operation",ErrorMessage="Month and Year must be greater than previous entered month and year")]
        public int previousMonth { get; set; }

        public int previousYear { get; set; }

        public int AgreementYear { get; set; }

        public int AgreementMonth { get; set; }

        public string AgreementDate { get; set; }

        [Display(Name="Completion Date")]
        [CompareCompletionDate("EXEC_PROG_YEAR", "EXEC_PROG_MONTH", "EXEC_ISCOMPLETED", ErrorMessage = "The completion date must match the selected Month and Year.")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Completion Date must be in dd/mm/yyyy format.")]
        public string ExecutionCompleteDate { get; set; }

        public int crYear { get; set; }
        public string currmonthName { get; set; }
        public string prevmonthName { get; set; }
    }

    public class CompareLengthLSB : ValidationAttribute //, IClientValidatable
    {
        private readonly string Bridge;
        private readonly string Road;
        private readonly string WorkStatus;
        private readonly string ChangedLength;

        public CompareLengthLSB(string bridgeName, string roadName, string workStatus, string changedLength)
        {
            this.Bridge = bridgeName;
            this.Road = roadName;
            this.WorkStatus = workStatus;
            this.ChangedLength = changedLength;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyTestedInfo = validationContext.ObjectType.GetProperty(this.Bridge);
            var propertyTestedInfoRoad = validationContext.ObjectType.GetProperty(this.Road);
            var propertyTestedInfoWorkStatus = validationContext.ObjectType.GetProperty(this.WorkStatus);
            var propertyTestedInfoLSBLength = validationContext.ObjectType.GetProperty(this.ChangedLength);
            if (propertyTestedInfo == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.Bridge));
            }

            var bridgeLength = Convert.ToDecimal(propertyTestedInfo.GetValue(validationContext.ObjectInstance, null));
            var roadLength = Convert.ToDecimal(propertyTestedInfoRoad.GetValue(validationContext.ObjectInstance, null));
            var workStatus = propertyTestedInfoWorkStatus.GetValue(validationContext.ObjectInstance, null);

            //var comparisonLengthMax = Convert.ToDecimal(value);
            //comparisonLengthMax = comparisonLengthMax + comparisonLengthMax / 10;
            //var comparisonLengthMin = Convert.ToDecimal(value);
            //comparisonLengthMin = comparisonLengthMin - comparisonLengthMin / 10;
            var comparisonValue = Convert.ToDecimal(value);

            var changedLSBLength = Convert.ToDecimal(propertyTestedInfoLSBLength.GetValue(validationContext.ObjectInstance, null));
            comparisonValue = Convert.ToDecimal(changedLSBLength) > 0 ? Convert.ToDecimal(changedLSBLength) : comparisonValue;

            if (workStatus.ToString().ToLower() == "c")
            {
                //if(bridgeLength < (comparisonValue - comparisonValue /10) || bridgeLength > (comparisonValue + comparisonValue / 10))
                if(bridgeLength > (comparisonValue + comparisonValue / 10))
                {
                    //return new ValidationResult("During complete status completed length must be between (Sanction length -10% of Sanction length) and (Sanction length + 10% of Sanction length)");
                    return new ValidationResult("During complete status completed length must be upto (Sanction length + 10% of Sanction length)");
                }

                //if (roadLength < (comparisonValue - comparisonValue / 10) || roadLength > (comparisonValue + comparisonValue / 10))
                //if (roadLength > (comparisonValue + comparisonValue / 10))
                //{
                //    //return new ValidationResult("During complete status approach work length must be between (Sanction length -10% of Sanction length) and (Sanction length + 10% of Sanction length)");
                //    return new ValidationResult("During complete status approach work length must be upto (Sanction length + 10% of Sanction length)");
                //}
            }

            if (workStatus.ToString().ToLower() == "p")
            {
                if (bridgeLength <= comparisonValue && roadLength <= comparisonValue)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult("In progress status,Completed length or Approach work length should not be greater than Bridge Length.");
                }
            }


            if (bridgeLength == 0 || bridgeLength == null)
            {
                return ValidationResult.Success;
            }

            if (roadLength == 0 || roadLength == null)
            {
                return ValidationResult.Success;
            }

            

            return ValidationResult.Success;
        }
    }


    public class ComparePreviousStatusLSB : ValidationAttribute //, IClientValidatable
    {
        private readonly string PropertyName;
        private readonly string PropertyOperation;

        public ComparePreviousStatusLSB(string propertyName,string propertyOperation)
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

            var newstatus = propertyTestedInfo.GetValue(validationContext.ObjectInstance, null);
            var operation = propertyTestedOperation.GetValue(validationContext.ObjectInstance, null);
            if (operation.ToString().ToLower() == "e")
            {
                return ValidationResult.Success;
            }

            if (value == null)
            {
                return ValidationResult.Success;
            }
            var existingstatus = value.ToString();
            if (existingstatus.ToString().ToLower() == "c")
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }

            return ValidationResult.Success;
        }
    }

    public class CompareMonth : ValidationAttribute, IClientValidatable
    {
        private readonly string PropertyMonth;
        private readonly string PropertyYear;
        private readonly string PropertyPreviousYear;
        private readonly string PropertyOperation;

        public CompareMonth(string propertyMonth, string PropertyYear, string PropertyPreviousYear,string PropertyOperation)
        {
            this.PropertyMonth= propertyMonth;
            this.PropertyYear = PropertyYear;
            this.PropertyPreviousYear = PropertyPreviousYear;
            this.PropertyOperation = PropertyOperation;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                var propertyTestedInfo = validationContext.ObjectType.GetProperty(this.PropertyMonth);
                var propertyTestedYear = validationContext.ObjectType.GetProperty(this.PropertyYear);
                var propertyTestedPreviousYear = validationContext.ObjectType.GetProperty(this.PropertyPreviousYear);
                var propertyTestedOperation = validationContext.ObjectType.GetProperty(this.PropertyOperation);
                if (propertyTestedInfo == null)
                {
                    return new ValidationResult(string.Format("unknown property {0}", this.PropertyMonth));
                }

                int execprogmonth = Convert.ToInt32(propertyTestedInfo.GetValue(validationContext.ObjectInstance, null));
                int execyear = Convert.ToInt32(propertyTestedYear.GetValue(validationContext.ObjectInstance, null));
                int previousyear = Convert.ToInt32(propertyTestedPreviousYear.GetValue(validationContext.ObjectInstance, null));
                var operation = propertyTestedOperation.GetValue(validationContext.ObjectInstance, null);

                if (Convert.ToInt32(value) == 0 || value == null)
                {
                    return ValidationResult.Success;
                }

                if (operation.ToString().ToLower() != "e")
                {
                    if (execyear < previousyear)
                    {
                        return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                    }
                    else
                    {
                        if (execyear == previousyear)
                        {
                            if (execprogmonth <= Convert.ToInt32(value))
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
                return ValidationResult.Success;
            }
            catch
            {
                return ValidationResult.Success;
            }
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "comparemonth"
            };
            rule.ValidationParameters["execprogmonth"] = this.PropertyMonth;
            rule.ValidationParameters["execyear"] = this.PropertyMonth;
            rule.ValidationParameters["previousyear"] = this.PropertyMonth;
            yield return rule;
        }

    }

    public class CompareSanctionLength : ValidationAttribute, IClientValidatable
    {
        private readonly string PropertyName;

        public CompareSanctionLength(string propertyName)
        {
            this.PropertyName = propertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                var propertyTestedInfo = validationContext.ObjectType.GetProperty(this.PropertyName);
                if (propertyTestedInfo == null)
                {
                    return new ValidationResult(string.Format("unknown property {0}", this.PropertyName));
                }

                var sanctionlength = Convert.ToDecimal(propertyTestedInfo.GetValue(validationContext.ObjectInstance, null));
                var percentSanction = sanctionlength /10;
                if (Convert.ToDecimal(value) == 0 || value == null)
                {
                    return ValidationResult.Success;
                }
                var newValue = (Convert.ToDecimal(value));

                if (percentSanction <= newValue)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
            }
            catch
            {
                return ValidationResult.Success;
            }
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "comparesanctionlength"
            };
            rule.ValidationParameters["sanctionlength"] = this.PropertyName;
            yield return rule;
        }
    }

    public class CompareCompleteSanctionLength : ValidationAttribute, IClientValidatable
    {
        private readonly string PropertyWorkStatus;
        private readonly string PropertyBridgeLength;

        public CompareCompleteSanctionLength(string propertyWorkStatus, string propertyBridgeLength)
        {
            this.PropertyWorkStatus = propertyWorkStatus;
            this.PropertyBridgeLength = propertyBridgeLength;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                var propertyTestedWorkStatus = validationContext.ObjectType.GetProperty(this.PropertyWorkStatus);
                var propertyTestedBridgeLength = validationContext.ObjectType.GetProperty(this.PropertyBridgeLength);


                if (propertyTestedWorkStatus == null)
                {
                    return new ValidationResult(string.Format("unknown property {0}", this.PropertyWorkStatus));
                }

                var workStatus = propertyTestedWorkStatus.GetValue(validationContext.ObjectInstance, null);
                var bridgeLength = Convert.ToDecimal(propertyTestedBridgeLength.GetValue(validationContext.ObjectInstance, null));
                
                if (Convert.ToDecimal(value) == 0 || value == null)
                {
                    return ValidationResult.Success;
                }

                if (workStatus.ToString().ToLower() == "c")
                {
                    if (bridgeLength >= Convert.ToDecimal(value))
                    {
                        return ValidationResult.Success;
                    }
                    else
                    {
                        return new ValidationResult("Bridge Length Completed must be equal to Sanctioned Bridge Length.");
                    }
                }
                else
                {
                    return ValidationResult.Success;
                }
            }
            catch
            {
                return ValidationResult.Success;
            }
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "comparecompletesanctionlength"
            };
            rule.ValidationParameters["workstatus"] = this.PropertyWorkStatus;
            rule.ValidationParameters["bridgelength"] = this.PropertyBridgeLength;
            //rule.ValidationParameters["sanctionlength"] = this.PropertyWorkStatus;
            yield return rule;
        }
    }


}