#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   ExecutionRoadStatusViewModel.cs
        * Description   :   This View Model is Used in CBR Views AddPhysicalRoadProgress.cshtml
        * Author        :   Vikram Nandanwar        
        * Creation Date :   25/June/2013
 **/
#endregion


using PMGSY.BAL.Execution;
using PMGSY.Common;
using PMGSY.DAL.Execution;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Execution
{
    public class ExecutionRoadStatusViewModel
    {

        public int PLAN_CN_ROAD_CODE { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Please select Cluster.")]
        public List<SelectListItem> clusterList { get; set; }
        public int clusterCode { get; set; }

        [RegularExpression("[CH]", ErrorMessage = "Please Select Cluster or Habitation")]
        public string Cluster_Habitation { get; set; }

        //For Multiselect
        public List<SelectListItem> HABITATIONS { get; set; }
        public int HABITATION_LIST { get; set; }
        public string ASSIGNED_HABITATION_LIST { get; set; }

        public string EncryptedHabCodes { get; set; }

        public string EncryptedRoadCode { get; set; }

        public int IMS_PR_ROAD_CODE { get; set; }

        public string EncryptedPhysicalRoadCode { get; set; }

        [Required(ErrorMessage="Please select year.")]
        [Range(2000,2099,ErrorMessage="Please select year.")]
        [Display(Name="Year")]
        [CompareAgrementYear("AgreementYear", "AgreementDate", ErrorMessage = "Year must be greater than or equal to Agreement Year")]
        public int EXEC_PROG_YEAR { get; set; }

        [Required(ErrorMessage="Please select month.")]
        [Range(1,12,ErrorMessage="Please select month.")]
        [Display(Name="Month")]
        [CompareAgrementMonth("EXEC_PROG_YEAR", "AgreementDate", ErrorMessage = "Month must be greater than or equal to Agreement Month")]
        public int EXEC_PROG_MONTH { get; set; }
        
        [Display(Name="Preparatory Work/Setting out and Earth Work Stage (Length in Km)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "Invalid Cutoff/Raft/Individual footing,Can only contains Numeric values and 3 digit after decimal place")]
        //[Range(0, 9999.999, ErrorMessage = "Invalid Preparatory Work,Can only contains Numeric values and 3 digit after decimal place")]
        [Range(0, 90.000, ErrorMessage = "Invalid Preparatory Work,Can only contains Numeric values and 3 digit after decimal place")]
        [ComparePreviousLength("PreviousPreparatoryWork", "EXEC_ISCOMPLETED", "changed_SanctionedLength", ErrorMessage = "Preparatory work length must be greater than or equal to previous preparatory work length and less than Sanction Length.")]
        [CompareLengthValues("EXEC_ISCOMPLETED", "Operation",ErrorMessage = "Preparatory Work Length should not be zero.")]
        public Nullable<decimal> EXEC_PREPARATORY_WORK { get; set; }

        [Display(Name="Earthwork Subgrade Stage (Length in Km)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "Invalid Subgrade Stage,Can only contains Numeric values and 3 digit after decimal place")]
        //[Range(0, 9999.999, ErrorMessage = "Invalid Subgrade Stage,Can only contains Numeric values and 3 digit after decimal place")]
        [Range(0, 90.000, ErrorMessage = "Invalid Subgrade Stage,Can only contains Numeric values and 3 digit after decimal place")]
        [ComparePreviousLength("PreviousEarthWork", "EXEC_ISCOMPLETED", "changed_SanctionedLength", ErrorMessage = "Earthwork subgrade length must be greater than or equal to previous Earthwork subgrade length and less than Sanction Length.")]
        //[CompareLengthValues("EXEC_ISCOMPLETED","Operation", ErrorMessage = "Subgrade Stage Length should not be zero.")]
        public Nullable<decimal> EXEC_EARTHWORK_SUBGRADE { get; set; }

        [Display(Name = "Subbase/GSB Stage (Length in Km)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "Invalid Subbase/GSB Stage,Can only contains Numeric values and 3 digit after decimal place")]
        //[Range(0, 9999.999, ErrorMessage = "Invalid Subbase/GSB Stage,Can only contains Numeric values and 3 digit after decimal place")]
        [Range(0, 90.000, ErrorMessage = "Invalid Subbase/GSB Stage,Can only contains Numeric values and 3 digit after decimal place")]
        [ComparePreviousLength("PreviousSubbase", "EXEC_ISCOMPLETED", "changed_SanctionedLength", ErrorMessage = "Subbase stage length must be greater than or equal to previous Subbase stage length and less than Sanction Length.")]
        //[CompareLengthValues("EXEC_ISCOMPLETED","Operation", ErrorMessage = "Subbase Stage Length should not be zero.")]
        public Nullable<decimal> EXEC_SUBBASE_PREPRATION { get; set; }

        [Display(Name = "Base Course /G2-G3 Stage (Length in Km)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "Invalid Basecourse Length,Can only contains Numeric values and 3 digit after decimal place")]
        //[Range(0, 9999.999, ErrorMessage = "Invalid Basecourse Length,Can only contains Numeric values and 3 digit after decimal place")]
        [Range(0, 90.000, ErrorMessage = "Invalid Basecourse Length,Can only contains Numeric values and 3 digit after decimal place")]
        [ComparePreviousLength("PreviousBaseCourse", "EXEC_ISCOMPLETED", "changed_SanctionedLength", ErrorMessage = "Base Course length must be greater than or equal to previous Base Course work length and less than Sanction Length.")]
        //[CompareLengthValues("EXEC_ISCOMPLETED", "Operation",ErrorMessage = "Base Course Stage Length should not be zero.")]
        public Nullable<decimal> EXEC_BASE_COURSE { get; set; }

        [Display(Name="Completed(Length in Km.)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "Invalid Completed Length,Can only contains Numeric values and 3 digit after decimal place")]
        //[Range(0, 9999.999, ErrorMessage = "Invalid Completed Length,Can only contains Numeric values and 3 digit after decimal place")]
        [Range(0, 90.000, ErrorMessage = "Invalid Completed Length,Can only contains Numeric values and 3 digit after decimal place")]
        [CompareIsStage("EXEC_EARTHWORK_SUBGRADE", "IsStage",ErrorMessage="Road is a Stage construction so value should be equal to value of Earthwork Subgrade Stage.")]
        [CompareNoStage("EXEC_SURFACE_COURSE", "IsStage", ErrorMessage = "Value should be equal to value of Surface Course.")]
        [CompareRoadLength("changed_SanctionedLength", "EXEC_ISCOMPLETED", ErrorMessage = "On complete status road length completed must be between (sanction length + 10 %) and (sanction length - 10%)")]
        //[CompareRoadLength("changed_SanctionedLength", "EXEC_ISCOMPLETED", ErrorMessage = "On complete status road length completed must be upto 10 % of sanction length")]
        //[ComparePreviousCompletedLength("EXEC_PROG_MONTH", "EXEC_PROG_YEAR", "IMS_PR_ROAD_CODE", ErrorMessage = "Completed Length must be greater than previous completed length.")]
        [ComparePreviousLength("PreviousCompletedLength", "EXEC_ISCOMPLETED", "changed_SanctionedLength", ErrorMessage = "Completed length must be greater than or equal to previous completed length and less than Sanction Length.")]
        public Nullable<decimal> EXEC_COMPLETED { get; set; }

        [Display(Name="Surface Course/BT Stage (Length in Km)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "Invalid Surface Course/BT Stage,Can only contains Numeric values and 3 digit after decimal place")]
        //[Range(0, 9999.999, ErrorMessage = "Invalid Surface Course/BT Stage,Can only contains Numeric values and 3 digit after decimal place")]
        [Range(0, 90.000, ErrorMessage = "Invalid Surface Course/BT Stage,Can only contains Numeric values and 3 digit after decimal place")]
        [ComparePreviousLength("PreviousSurfaceCourse", "EXEC_ISCOMPLETED", "changed_SanctionedLength", ErrorMessage = "Surface Course length must be greater than or equal to previous Surface Course work length and less than Sanction Length.")]
        //[CompareLengthValues("EXEC_ISCOMPLETED","Operation", ErrorMessage = "Surface Course Length should not be zero.")]
        public Nullable<decimal> EXEC_SURFACE_COURSE { get; set; }

        [Display(Name="Road Signs Stones (Nos.)")]
        [Range(0, 9999, ErrorMessage = "Invalid Value.")]
        //[ComparePreviousValue("PreviousRoadSigns","Operation", ErrorMessage = "Road Signs Stones must be greater than or equal to previous Road Signs Stones values.")]
        [RegularExpression(@"\d+$", ErrorMessage = "Please enter valid value.")]
        public Nullable<decimal> EXEC_SIGNS_STONES { get; set; }

        [Display(Name="Cross Drainage Works(Nos.)")]
        [Range(0, 9999, ErrorMessage = "Invalid Value.")]
        //[ComparePreviousValue("PreviousCDWorks","Operation", ErrorMessage = "Cross Drainage Works must be greater than or equal to previous Cross Drainage Works values.")]
        [RegularExpression(@"\d+$", ErrorMessage = "Please enter valid value.")]
        public Nullable<decimal> EXEC_CD_WORKS { get; set; }

        [Display(Name="Long Span Bridges(Nos.)")]
        [Range(0,9999,ErrorMessage="Invalid Value.")]
        //[ComparePreviousValue("PreviousLSB","Operation", ErrorMessage = "Long Span Bridges must be greater than or equal to previous Long Span Bridges values.")]
        [RegularExpression(@"\d+$", ErrorMessage = "Please enter valid value.")]
        public Nullable<decimal> EXEC_LSB_WORKS { get; set; }

        [Display(Name="Miscellaneous(Length in Km)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "Invalid Miscellaneous length,Can only contains Numeric values and 3 digit after decimal place")]
        //[ComparePreviousLength("PreviousMiscellaneous", "EXEC_ISCOMPLETED", "changed_SanctionedLength", ErrorMessage = "Miscellaneous length must be greater than or equal to previous Miscellaneous length and less than Sanction Length.")]
        [CompareMiscellaneousLength("changed_SanctionedLength", ErrorMessage = "Miscellaneous length should not exceed sanction length.")]
        //[CompareLengthValues("EXEC_ISCOMPLETED","Operation", ErrorMessage = "Miscellaneous Length should not be zero.")]
        [Range(0, 90.000, ErrorMessage = "Invalid Miscelaneous Length,Can only contains Numeric values and 3 digit after decimal place")]
        public Nullable<decimal> EXEC_MISCELANEOUS { get; set; }

        [Required(ErrorMessage="Please select work status.")]
        [Display(Name="Work Status")]
        [RegularExpression(@"^([CPFAL]+)$", ErrorMessage = "Please select work status.")]
        [ComparePreviousStatus("CompleteStatus","Operation",ErrorMessage="Work status is already completed.")]
        //[CompareStatus("EXEC_ISCOMPLETED",ErrorMessage="Progress can not be updated for this status.")]
        [IsSplitWork("IMS_PR_ROAD_CODE", ErrorMessage = "The agreement or work is incomplete against this road or The agreement has not been made against this road, You can not select the status Complete.")]
        public string EXEC_ISCOMPLETED { get; set; }

        public string Operation { get; set; }
        
        public Nullable<decimal> OldCompleted { get; set; }

        public string BlockName { get; set; }

        public string Package { get; set; }

        public int RoadNo { get; set; }

        public string RoadName { get; set; }

        public double Sanction_Cost { get; set; }

        public double MaintananceCost { get; set; }

        //[CompareLengthRoad("EXEC_COMPLETED", "EXEC_ISCOMPLETED", ErrorMessage = "Sanctioned Length is less than Completed Length.")]
        public decimal Sanction_length { get; set; }
                
        public string CompleteStatus { get; set; }

        [CompareMonth("EXEC_PROG_MONTH", "EXEC_PROG_YEAR", "PreviousYear","Operation", ErrorMessage = "Month and Year must be greater than previous entered month and year")]
        public int PreviousMonth { get; set; }

        public int PreviousYear { get; set; }

        public decimal? PreviousPreparatoryWork { get; set; }

        public decimal? PreviousEarthWork { get; set; }

        public decimal? PreviousSubbase { get; set; }

        public decimal? PreviousBaseCourse { get; set; }

        public decimal? PreviousSurfaceCourse { get; set; }

        public decimal? PreviousMiscellaneous { get; set; }

        public int PreviousCDWorks { get; set; }

        public int PreviousLSB { get; set; }

        public int PreviousRoadSigns { get; set; }

        public decimal? PreviousCompletedLength { get; set; }

        public string IsStage { get; set; }

        public int AgreementYear { get; set; }

        public int AgreementMonth { get; set; }

        public string AgreementDate { get; set; }

        public int Year { get; set; }

        public decimal? AgreementCost { get; set; }

        [Display(Name="Completion Date")]
        [CompareCompletionDate("EXEC_PROG_YEAR","EXEC_PROG_MONTH","EXEC_ISCOMPLETED",ErrorMessage="The completion date must match the selected Month and Year.")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Completion Date must be in dd/mm/yyyy format.")]
        public string ExecutionCompleteDate { get; set; }

        public decimal changedLength { get; set; }

        public decimal changed_SanctionedLength { get; set; }

        public int crYear { get; set; }
        
        public string currmonthName { get; set; }
        public string prevmonthName { get; set; }

        public string SanctionYear { get; set; }

        //added by pradip on 9/03/2017

        public string mappedHabitaionDate { get; set; }
    }

    public class CompareStatus : ValidationAttribute //, IClientValidatable
    {
        private readonly string PropertyName;

        public CompareStatus(string propertyName)
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

            var existingpackage = propertyTestedInfo.GetValue(validationContext.ObjectInstance, null);

            if (existingpackage.ToString().ToLower() == "f")
            {
                if (value == null)
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
                else
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
            }

            if (existingpackage.ToString().ToLower() == "a")
            {
                if (value == null)
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
                else
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
            }

            if (existingpackage.ToString().ToLower() == "l")
            {
                if (value == null)
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
                else
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
            }

            return ValidationResult.Success;
        }
    }

    public class CompareValidation : ValidationAttribute , IClientValidatable
    {
        private readonly string PropertyName;

        public CompareValidation(string propertyName)
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

    public class CompareLengthRoad : ValidationAttribute , IClientValidatable
    {
        private readonly string status;
        private readonly string completed;

        public CompareLengthRoad(string completed, string status)
        {
            this.completed = completed;
            this.status = status;
            
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyTestedCompleted = validationContext.ObjectType.GetProperty(this.completed);
            var propertyTestedStatus = validationContext.ObjectType.GetProperty(this.status);

            if (propertyTestedCompleted == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.status));
            }

            var WorkStatus = propertyTestedStatus.GetValue(validationContext.ObjectInstance, null);
            var completedLength = Convert.ToDecimal(propertyTestedCompleted.GetValue(validationContext.ObjectInstance, null));
            var sanctionedLength = Convert.ToDecimal(value);

            if (WorkStatus.ToString() == "C")
            {
                if (sanctionedLength == null)
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
                else
                {
                    if (completedLength < sanctionedLength)
                    {
                        return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                    }
                    else
                    {
                        return ValidationResult.Success;
                    }
                }
            }
            else if(WorkStatus.ToString().ToLower() == "p")
            {
                if (sanctionedLength == null)
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
                else
                {
                    if (sanctionedLength < completedLength)
                    {
                        return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                    }
                    else
                    {
                        return ValidationResult.Success;
                    }
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
                ValidationType = "comparelengthroad"
            };
            yield return rule;
        }

    }

    public class CompareAgrementYear : ValidationAttribute, IClientValidatable
    {
        private readonly string dateComponent;
        private readonly string date;

        public CompareAgrementYear(string dateValue, string date)
        {
            this.dateComponent = dateValue;
            this.date = date;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyTestedDateComponent = validationContext.ObjectType.GetProperty(this.dateComponent);
            var propertyTestedDate = validationContext.ObjectType.GetProperty(this.date);

            DateTime? dateToCompare = null;
            if (propertyTestedDateComponent == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.date));
            }

            int dateValue = Convert.ToInt32(propertyTestedDateComponent.GetValue(validationContext.ObjectInstance, null));
            var date = propertyTestedDate.GetValue(validationContext.ObjectInstance, null);

            if (date != null)
            {
                 dateToCompare = new CommonFunctions().GetStringToDateTime(date.ToString());
            }

            if(value == null)
            {
                return ValidationResult.Success;
            }

            if (Convert.ToInt32(value) >= dateToCompare.Value.Year)
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
                ValidationType = "compareagreementyear"
            };
            yield return rule;
        }

    }


    public class CompareAgrementMonth : ValidationAttribute, IClientValidatable
    {
        private readonly string dateComponent;
        private readonly string date;

        public CompareAgrementMonth(string dateValue, string date)
        {
            this.dateComponent = dateValue;
            this.date = date;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyTestedDateComponent = validationContext.ObjectType.GetProperty(this.dateComponent);
            var propertyTestedDate = validationContext.ObjectType.GetProperty(this.date);


            if (propertyTestedDateComponent == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.date));
            }

            int progYear = Convert.ToInt32(propertyTestedDateComponent.GetValue(validationContext.ObjectInstance, null));
            var date = propertyTestedDate.GetValue(validationContext.ObjectInstance, null);

            DateTime? dateToCompare = new CommonFunctions().GetStringToDateTime(date.ToString());

            if(value == null)
            {
                return ValidationResult.Success;
            }

            if (dateToCompare.Value.Year < progYear)
            {
                return ValidationResult.Success;
            }
            else if (dateToCompare.Value.Year == progYear)
            {
                if (dateToCompare.Value.Month <= Convert.ToInt32(value))
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
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "compareagreementmonth"
            };
            yield return rule;
        }

    }

    public class ComparePreviousStatus : ValidationAttribute //, IClientValidatable
    {
        private readonly string PropertyName;
        private readonly string PropertyOperation;


        public ComparePreviousStatus(string propertyName,string propertyOperation)
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
            if(operation.ToString().ToLower() == "e")
            {
                return ValidationResult.Success;
            }

            if (value == null)
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
            var newStatus = value;

            if (existingstatus == null)
            {
                return ValidationResult.Success;
            }

            if (existingstatus.ToString().ToLower() == "c")
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

    public class ComparePreviousLength : ValidationAttribute , IClientValidatable
    {
        private readonly string PropertyPreviousLength;
        private readonly string PropertyStatus;
        //private readonly string changedSanctionedLength;

        public ComparePreviousLength(string previousLength,string status, string changedSanctionedLength)
        {
            this.PropertyPreviousLength = previousLength;
            this.PropertyStatus = status;
            //this.changedSanctionedLength = changedSanctionedLength;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyTestedLength = validationContext.ObjectType.GetProperty(this.PropertyPreviousLength);
            var propertyTestedStatus = validationContext.ObjectType.GetProperty(this.PropertyStatus);
            //var sancValue = validationContext.ObjectType.GetProperty(this.changedSanctionedLength);
            var status = propertyTestedStatus.GetValue(validationContext.ObjectInstance, null);

            if (status.ToString().ToLower() == "f" || status.ToString().ToLower() == "a" || status.ToString().ToLower() == "l")
            {
                return ValidationResult.Success;
            }

            if (propertyTestedLength == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.PropertyPreviousLength));
            }

            var existinglength = Convert.ToDecimal(propertyTestedLength.GetValue(validationContext.ObjectInstance, null));

            var newLength = Convert.ToDecimal(value);

            //var sanctionValue = Convert.ToDecimal(sancValue.GetValue(validationContext.ObjectInstance, null));///Get changedSanctionedLength value in decimal

            if (newLength == null)
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }

            if (newLength < existinglength)
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
            //decimal additionalsanctionValue = (Convert.ToDecimal(sanctionValue) * Convert.ToDecimal(0.1)) + Convert.ToDecimal(sanctionValue);
            //if (newLength > Convert.ToDecimal(sanctionValue)
            //if (newLength > additionalsanctionValue)
            //{
            //    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            //}

            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "comparepreviouslength"
            };
            rule.ValidationParameters.Add("previousval", this.PropertyPreviousLength);
            //yield 
            return new[]{rule};
        }
    }

    public class ComparePreviousValue : ValidationAttribute, IClientValidatable
    {
        private readonly string PropertyPreviousValue;
        private readonly string PropertyStatus;

        public ComparePreviousValue(string previousValue, string status)
        {
            this.PropertyPreviousValue = previousValue;
            this.PropertyStatus = status;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyTestedLength = validationContext.ObjectType.GetProperty(this.PropertyPreviousValue);
            var propertyTestedStatus = validationContext.ObjectType.GetProperty(this.PropertyStatus);
            var status = propertyTestedStatus.GetValue(validationContext.ObjectInstance, null);

            if (status.ToString().ToLower() == "f" || status.ToString().ToLower() == "a" || status.ToString().ToLower() == "l")
            {
                return ValidationResult.Success;
            }

            if (propertyTestedLength == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.PropertyPreviousValue));
            }

            var existingValue = Convert.ToInt32(propertyTestedLength.GetValue(validationContext.ObjectInstance, null));

            var newLength = Convert.ToInt32(value);

            if (newLength == null)
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }

            if (newLength < existingValue)
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
                ValidationType = "comparepreviousvalue"
            };
            rule.ValidationParameters.Add("previousno", this.PropertyPreviousValue);
            return new[] { rule };
        }
    }


    public class CompareIsStage : ValidationAttribute, IClientValidatable
    {
        private readonly string PropertyEarthWork;
        private readonly string PropertyStage;

        public CompareIsStage(string earthWorkValue,string stage)
        {
            this.PropertyEarthWork = earthWorkValue;
            this.PropertyStage = stage;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyTestedEarthWork = validationContext.ObjectType.GetProperty(this.PropertyEarthWork);
            var propertyTestedStage = validationContext.ObjectType.GetProperty(this.PropertyStage);

            if (propertyTestedEarthWork == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.PropertyEarthWork));
            }

            var earthWorkValue = Convert.ToDecimal(propertyTestedEarthWork.GetValue(validationContext.ObjectInstance, null));
            var stage = propertyTestedStage.GetValue(validationContext.ObjectInstance, null);
            var completedValue = Convert.ToDecimal(value);

            if (completedValue == null)
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }

            if (stage == null || stage == "")
            {
                return ValidationResult.Success;
            }
            else if (stage == "S1")
            {
                if (completedValue >= earthWorkValue)
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

            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "compareisstage"
            };
            yield return rule ;
        }
    }


    public class CompareNoStage : ValidationAttribute, IClientValidatable
    {
        private readonly string PropertySurfaceCourse;
        private readonly string PropertyStage;

        public CompareNoStage(string surfaceValue, string stage)
        {
            this.PropertySurfaceCourse = surfaceValue;
            this.PropertyStage = stage;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyTestedSurfaceCourse = validationContext.ObjectType.GetProperty(this.PropertySurfaceCourse);
            var propertyTestedStage = validationContext.ObjectType.GetProperty(this.PropertyStage);

            if (propertyTestedSurfaceCourse == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.PropertySurfaceCourse));
            }

            var surfaceValue = Convert.ToDecimal(propertyTestedSurfaceCourse.GetValue(validationContext.ObjectInstance, null));
            var stage = propertyTestedStage.GetValue(validationContext.ObjectInstance, null);
            var completedValue = Convert.ToDecimal(value);

            if (completedValue == null)
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }

            if (stage == null || stage == "" || stage == "S2")
            {
                if (completedValue >= surfaceValue)
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

            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "comparenostage"
            };
            yield return rule;
        }
    }

    public class CompareMiscellaneousLength : ValidationAttribute, IClientValidatable
    {
        private readonly string PropertySanctionLength;

        public CompareMiscellaneousLength(string sanctionValue)
        {
            this.PropertySanctionLength = sanctionValue;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyTestedSanctionValue = validationContext.ObjectType.GetProperty(this.PropertySanctionLength);

            if (propertyTestedSanctionValue == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.PropertySanctionLength));
            }

            var sancValue = Convert.ToDecimal(propertyTestedSanctionValue.GetValue(validationContext.ObjectInstance, null));
            
            var miscValue = Convert.ToDecimal(value);

            if (miscValue == null)
            {
                return ValidationResult.Success;
            }

            if (sancValue < miscValue)
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
                ValidationType = "comparemiscvalue"
            };
            yield return rule;
        }
    }

    public class CompareLengthValues : ValidationAttribute, IClientValidatable
    {

        private readonly string PropertyStatus;
        private readonly string PropertyOperation;


        public CompareLengthValues(string status,string operation)
        {
            this.PropertyStatus = status;
            this.PropertyOperation = operation;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyTestedStatus = validationContext.ObjectType.GetProperty(this.PropertyStatus);
            var propertyTestedOperation = validationContext.ObjectType.GetProperty(this.PropertyOperation);


            if (propertyTestedStatus == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.PropertyStatus));
            }

            var status = propertyTestedStatus.GetValue(validationContext.ObjectInstance, null);

            if (status.ToString().ToLower() == "f" || status.ToString().ToLower() == "a" || status.ToString().ToLower() == "l")
            {
                return ValidationResult.Success;
            }

            var operation = propertyTestedOperation.GetValue(validationContext.ObjectInstance, null);

            if (operation.ToString().ToLower() == "e")
            {
                return ValidationResult.Success;
            }

            var length = Convert.ToDecimal(value);

            if (value == null)
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }

            if (length == 0)
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
                ValidationType = "comparevalue"
            };
            yield return rule;
        }
    }


    public class IsSplitWork : ValidationAttribute, IClientValidatable
    {
     
        private readonly string PropertyRoadCode;

        public IsSplitWork(string proposalCode)
        {
            this.PropertyRoadCode = proposalCode;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyTestedSanctionValue = validationContext.ObjectType.GetProperty(this.PropertyRoadCode);
            
            if (propertyTestedSanctionValue == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.PropertyRoadCode));
            }

            int roadCode = Convert.ToInt32(propertyTestedSanctionValue.GetValue(validationContext.ObjectInstance, null));

            if (value == null)
            {
                return ValidationResult.Success;
            }

            if (value.ToString().ToLower() == "c")
            {
                ExecutionBAL objBAL = new ExecutionBAL();
                bool status = objBAL.CheckSplitWork(roadCode);
                if (status)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
            
            }

            return null;

        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "compareissplitwork"
            };
            yield return rule;
        }
    }

    public class CompareRoadLength : ValidationAttribute, IClientValidatable
    {
        private readonly string PropertySanctionLength;
        private readonly string PropertyStatus;

        public CompareRoadLength(string sanctionValue,string status)
        {
            this.PropertySanctionLength = sanctionValue;
            this.PropertyStatus = status;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyTestedSanctionValue = validationContext.ObjectType.GetProperty(this.PropertySanctionLength);
            var propertyTestedStatus = validationContext.ObjectType.GetProperty(this.PropertyStatus);

            if (propertyTestedSanctionValue == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.PropertySanctionLength));
            }

            var sancValue = Convert.ToDecimal(propertyTestedSanctionValue.GetValue(validationContext.ObjectInstance, null));
            sancValue = sancValue * 1000;
            var status = propertyTestedStatus.GetValue(validationContext.ObjectInstance, null);
            var completedValue = Convert.ToDecimal(value);
            completedValue = completedValue * 1000;

            if(status.ToString().ToLower() == "c")
            {
                if(completedValue < (sancValue - sancValue / 10) || completedValue > (sancValue + sancValue / 10))
                //if (completedValue > (sancValue + sancValue / 10))
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
                else
                {
                    return ValidationResult.Success;
                }
            }
            else if (status.ToString().ToLower() == "p")
            {
                if (completedValue > (sancValue + sancValue / 10))
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
                if (completedValue > sancValue)
                {
                    return new ValidationResult("The completed length should not be greater than Approved/Sanction Length in 'In Progress' status.");
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

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "compareroadlength"
            };
            yield return rule;
        }
    }


    public class CompareCompletionDate : ValidationAttribute, IClientValidatable
    {
        private readonly string PropertyYear;
        private readonly string PropertyMonth;
        private readonly string PropertyFlag;


        public CompareCompletionDate(string propertyYear, string propertyMonth, string propertyFlag)
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

                if (flag.ToString().ToLower() != "c")
                {
                    return ValidationResult.Success;
                }

                //DateTime paymentDate = Convert.ToDateTime(value.ToString());
                DateTime paymentDate = objCommon.GetStringToDateTime(value.ToString());

                if (paymentDate > System.DateTime.Now)
                {
                    return new ValidationResult(FormatErrorMessage("Completion Date should not be greater than today's date."));
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


    public class ComparePreviousCompletedLength : ValidationAttribute//, IClientValidatable
    {
        private readonly string PropertyMonth;
        private readonly string PropertyYear;
        private readonly string PropertyProposalCode;

        public ComparePreviousCompletedLength(string month, string year,string proposalCode)
        {
            this.PropertyMonth = month;
            this.PropertyYear = year;
            this.PropertyProposalCode = proposalCode;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyTestedMonth = validationContext.ObjectType.GetProperty(this.PropertyMonth);
            var propertyTestedYear = validationContext.ObjectType.GetProperty(this.PropertyYear);
            var propertyTestedProposalCode = validationContext.ObjectType.GetProperty(this.PropertyProposalCode);

            var month = Convert.ToInt32(propertyTestedMonth.GetValue(validationContext.ObjectInstance, null));
            var year = Convert.ToInt32(propertyTestedYear.GetValue(validationContext.ObjectInstance, null));
            var proposalCode = Convert.ToInt32(propertyTestedProposalCode.GetValue(validationContext.ObjectInstance, null));
            var completedLength = Convert.ToDecimal(value);

            IExecutionBAL objBAL = new ExecutionBAL();

            bool status = objBAL.CheckPreviousCompletedLength(month,year,proposalCode,completedLength);

            if (status == true)
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
        }
       
    }

}

