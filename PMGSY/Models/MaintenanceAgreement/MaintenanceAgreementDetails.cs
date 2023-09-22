using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using PMGSY.DAL.Agreement;
using PMGSY.Models.Agreement;
using PMGSY.Extensions;
using PMGSY.Models.Proposal;

namespace PMGSY.Models.MaintenanceAgreement
{
    public class MaintenanceAgreementDetails
    {
        public MaintenanceAgreementDetails()
        {
            PMGSYScheme = PMGSYSession.Current.PMGSYScheme;
        }


        [UIHint("Hidden")]
        public string EncryptedIMSPRRoadCode { get; set; }

        [UIHint("Hidden")]
        public string EncryptedPRContractCode { get; set; }

        public int MANE_CONTRACT_ID { get; set; }

        [Display(Name = "Contractor")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select contractor.")]
        public int MAST_CON_ID { get; set; }

        [Display(Name = "Agreement Number")]
        //[RegularExpression(@"^[a-zA-Z0-9-/._]+$", ErrorMessage = "Agreement Number is not in valid format.")]
        // [RegularExpression(@"^[a-zA-Z0-9 -/()._]+$", ErrorMessage = "Agreement Number is not in valid format.")]

        [RegularExpression(@"^[a-zA-Z0-9]+[a-zA-Z0-9-/()._ ]*$", ErrorMessage = "Agreement Number should contains at least one character or number and starts with alphanumeric value.")]
        //[RegularExpression(@"^[a-zA-Z0-9][a-zA-Z0-9 -/()._]+$", ErrorMessage = "Agreement Number should contains at least one character or number and starts with alphanumeric value.")]
        [Required(ErrorMessage = "Agreement Number is required.")]
        [StringLength(100, ErrorMessage = "Agreement Number must be less than 100 characters.")]
        public string MANE_AGREEMENT_NUMBER { get; set; }

        [Display(Name = "Agreement Date")]
        [Required(ErrorMessage = "Agreement Date is required.")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Agreement Date must be in dd/mm/yyyy format.")]
        //[DateValidationVST("MANE_CONSTR_COMP_DATE", ErrorMessage = "Agreement date must be greater than or equal to construction completion date.")]
        public string MANE_AGREEMENT_DATE { get; set; }

        [Display(Name = "Construction Completion Date")]
        [Required(ErrorMessage = "Construction Completion Date is required.")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Construction Completion Date must be in dd/mm/yyyy format.")]
        // [DateValidationVST("TEND_DATE_OF_AGREEMENT", ErrorMessage = "Agreement start date must be greater than or equal to agreement date.")]
        public string MANE_CONSTR_COMP_DATE { get; set; }

        [Display(Name = "Maintenance Start Date")]
        [Required(ErrorMessage = "Maintenance Start Date is required.")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Maintenance Start Date must be in dd/mm/yyyy format.")]
        [DateValidationVST("MANE_AGREEMENT_DATE", ErrorMessage = "Maintenance start date must be greater than or equal to agreement date.")]
        // [MaintenanceDateValidation("MANE_CONSTR_COMP_DATE", ErrorMessage = "Maintenance start date must be greater than or equal to construction completion date.")]        
        public string MANE_MAINTENANCE_START_DATE { get; set; }

        [Display(Name = "Maintenance End Date")]
        [MaintenanceEndDateValidation("MANE_MAINTENANCE_START_DATE", ErrorMessage = "Maintenance end date must be greater than or equal to Maintenance start date.")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Maintenance End Date must be in dd/mm/yyyy format.")]
        public string MANE_MAINTENANCE_END_DATE { get; set; }

        [Display(Name = "Maintenance Cost Year1 (Rs in Lakhs)")]
        [Required(ErrorMessage = "Maintenance Cost Year1 is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Maintenance Cost Year1 is not in valid format. ")]
        [Range(0, 9999999999999999.99, ErrorMessage = "Maintenance Cost Year1 is not in valid format.")]
        public decimal? MANE_YEAR1_AMOUNT { get; set; }

        [Display(Name = "Maintenance Cost Year2 (Rs in Lakhs)")]
        [Required(ErrorMessage = "Maintenance Cost Year2 is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Maintenance Cost Year2 is not in valid format. ")]
        [Range(0, 9999999999999999.99, ErrorMessage = "Maintenance Cost Year2 is not in valid format.")]
        public decimal? MANE_YEAR2_AMOUNT { get; set; }

        [Display(Name = "Maintenance Cost Year3 (Rs in Lakhs)")]
        [Required(ErrorMessage = "Maintenance Cost Year3 is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Maintenance Cost Year3 is not in valid format. ")]
        [Range(0, 9999999999999999.99, ErrorMessage = "Maintenance Cost Year3 is not in valid format.")]
        public decimal? MANE_YEAR3_AMOUNT { get; set; }

        [Display(Name = "Maintenance Cost Year4 (Rs in Lakhs)")]
        [Required(ErrorMessage = "Maintenance Cost Year4 is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Maintenance Cost Year4 is not in valid format. ")]
        [Range(0, 9999999999999999.99, ErrorMessage = "Maintenance Cost Year4 is not in valid format.")]
        public decimal? MANE_YEAR4_AMOUNT { get; set; }

        [Display(Name = "Maintenance Cost Year5 (Rs in Lakhs)")]
        [Required(ErrorMessage = "Maintenance Cost Year5 is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Maintenance Cost Year5 is not in valid format. ")]
        [Range(0, 9999999999999999.99, ErrorMessage = "Maintenance Cost Year5 is not in valid format.")]
        public decimal? MANE_YEAR5_AMOUNT { get; set; }

        [Display(Name = "Renewal Cost (Rs in Lakhs)")]
        [CustomMaintenanceRequired("PMGSYScheme", ErrorMessage = "Renewal Cost is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Renewal Cost is not in valid format. ")]
        [Range(0, 9999999999999999.99, ErrorMessage = "Renewal Cost is not in valid format.")]
        public decimal? MANE_YEAR6_AMOUNT { get; set; }

        [Display(Name = "Handover Date")]
        // [Required(ErrorMessage = "Handover Date is required.")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Handover Date must be in dd/mm/yyyy format.")]
        [CheckDate("MANE_MAINTENANCE_START_DATE", "MANE_MAINTENANCE_END_DATE", ErrorMessage = "Handover date must be greater than or equal to maintenance end date. ")]
        public string MANE_HANDOVER_DATE { get; set; }

        [Display(Name = "Handover To")]
        [RegularExpression(@"^([a-zA-Z0-9 ._',/()-]+)$", ErrorMessage = "Handover To is not in valid format.")]
        [StringLength(255, ErrorMessage = "Handover To must be less than 255 characters.")]
        public string MANE_HANDOVER_TO { get; set; }

        [Display(Name = "Do you want to continue with same contractor? ")]
        [IsBooleanValidator(ErrorMessage = "Please select valid option")]
        public bool IsNewContractor { get; set; }

        [Display(Name = "Work")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select work.")]
        public int IMS_WORK_CODE { get; set; }

        [Display(Name = "Incomplete Reason")]
        public string IncompleteReason { get; set; }

        [Display(Name = "Value of Work Done (Rs in Lakhs)")]
        public decimal? ValueOfWorkDone { get; set; }

        public short PMGSYScheme { get; set; }

        public int CompletionMonth { get; set; }

        public int CompletionYear { get; set; }

        public string AgreementType { get; set; }

        public SelectList Contractors
        {
            get
            {
                int stateCode = PMGSY.Extensions.PMGSYSession.Current.StateCode;
                List<MASTER_CONTRACTOR> contractorList = new List<MASTER_CONTRACTOR>();

                AgreementDAL agreementDAL = new AgreementDAL();

                contractorList = agreementDAL.GetAllContractor(stateCode, "C", false);

                return new SelectList(contractorList, "MAST_CON_ID", "MAST_CON_COMPANY_NAME", this.MAST_CON_ID);

            }
        }

        public bool IsEdit { get; set; }
        public SelectList ProposalWorks
        {
            get
            {
                List<IMS_PROPOSAL_WORK> proposalWorkList = new List<IMS_PROPOSAL_WORK>();

                AgreementDAL agreementDAL = new AgreementDAL();

                proposalWorkList = agreementDAL.GetProposalWorks(this.EncryptedIMSPRRoadCode, string.Empty, false, false, this.IsEdit);

                return new SelectList(proposalWorkList, "IMS_WORK_CODE", "IMS_WORK_DESC");

            }
        }

        public List<string> lstNote { get; set; }

    }

    public class CheckDateAttribute : ValidationAttribute, IClientValidatable
    {

        // private const string _defaultErrorMessage = "Start date must be less than end date.";
        private string _basePropertyName;
        private string _basePropertyName_Second;

        public CheckDateAttribute(string basePropertyName, string basePropertyName_Second)
        //: base(_defaultErrorMessage)
        {
            _basePropertyName = basePropertyName;
            _basePropertyName_Second = basePropertyName_Second;
        }

        //Override default FormatErrorMessage Method  
        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, _basePropertyName);
        }

        //Override IsValid  
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                //Get PropertyInfo Object  
                var basePropertyInfo = validationContext.ObjectType.GetProperty(_basePropertyName);

                var basePropertyInfo_Second = validationContext.ObjectType.GetProperty(_basePropertyName_Second);


                //Get Value of the property  

                var sDate = basePropertyInfo.GetValue(validationContext.ObjectInstance, null);
                var date_Second = basePropertyInfo_Second.GetValue(validationContext.ObjectInstance, null);
                var eDate = value;

                if (sDate != null && eDate != null)
                {
                    var startDate = ConvertStringToDate(sDate.ToString());
                    var thisDate = ConvertStringToDate(eDate.ToString());
                    var secondDate = ConvertStringToDate(date_Second.ToString());

                    //Actual comparision  
                    if (thisDate < startDate || thisDate < secondDate)
                    {
                        var message = FormatErrorMessage(validationContext.DisplayName);
                        return new ValidationResult(message);
                    }
                }

                return ValidationResult.Success;
            }
            catch (Exception ex)
            {
                //var message = FormatErrorMessage(validationContext.DisplayName);
                //return new ValidationResult(message);
                return null;
            }
        }


        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            yield return new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                //This is the name of the method aaded to the jQuery validator method (must be lower case)
                ValidationType = "checkdatevalidator"
            };

        }

        public DateTime? ConvertStringToDate(string dateToConvert)
        {
            DateTime MyDateTime;
            MyDateTime = new DateTime();
            // MyDateTime = DateTime.Parse(dateToConvert);
            MyDateTime = DateTime.ParseExact(dateToConvert, "dd/MM/yyyy", null);
            //Convert.ToDateTime(dateToConvert);         //DateTime.ParseExact(dateToConver, "dd/MM/yyyy",null);
            return MyDateTime;
        }

    }

    public class MaintenanceDateValidationAttribute : ValidationAttribute, IClientValidatable
    {

        // private const string _defaultErrorMessage = "Start date must be less than end date.";
        private string _basePropertyName;

        public MaintenanceDateValidationAttribute(string basePropertyName)
        //: base(_defaultErrorMessage)
        {
            _basePropertyName = basePropertyName;
        }

        //Override default FormatErrorMessage Method  
        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, _basePropertyName);
        }

        //Override IsValid  
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                //Get PropertyInfo Object  
                var basePropertyInfo = validationContext.ObjectType.GetProperty(_basePropertyName);

                //Get Value of the property  

                //var startDate = (DateTime)basePropertyInfo.GetValue(validationContext.ObjectInstance, null);            
                //var thisDate = (DateTime)value;  

                var sDate = basePropertyInfo.GetValue(validationContext.ObjectInstance, null);
                var eDate = value;

                if (sDate != null && eDate != null)
                {
                    var startDate = ConvertStringToDate(sDate.ToString());
                    var thisDate = ConvertStringToDate(eDate.ToString());

                    //Actual comparision  
                    if (thisDate < startDate)
                    {
                        var message = FormatErrorMessage(validationContext.DisplayName);
                        return new ValidationResult(message);
                    }
                }

                //Default return - This means there were no validation error  
                //return null;
                return ValidationResult.Success;
            }
            catch (Exception ex)
            {
                //var message = FormatErrorMessage(validationContext.DisplayName);
                //return new ValidationResult(message);
                return null;
            }
        }


        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            //yield return new ModelClientValidationRule
            //{
            //    ErrorMessage = FormatErrorMessage(metadata.DisplayName),
            //    //This is the name of the method aaded to the jQuery validator method (must be lower case)
            //    ValidationType = "maintenancedatevalidator"
            //};

            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                ValidationType = "maintenancedatevalidator"
            };
            rule.ValidationParameters["date"] = this._basePropertyName;
            yield return rule;
        }


        public DateTime? ConvertStringToDate(string dateToConvert)
        {
            DateTime MyDateTime;
            MyDateTime = new DateTime();
            // MyDateTime = DateTime.Parse(dateToConvert);
            MyDateTime = DateTime.ParseExact(dateToConvert, "dd/MM/yyyy", null);
            //Convert.ToDateTime(dateToConvert);         //DateTime.ParseExact(dateToConver, "dd/MM/yyyy",null);
            return MyDateTime;
        }

    }


    //added by abhishek kamble 21-nov-2013
    public class MaintenanceEndDateValidationAttribute : ValidationAttribute, IClientValidatable
    {

        // private const string _defaultErrorMessage = "Start date must be less than end date.";
        private string _basePropertyName;

        public MaintenanceEndDateValidationAttribute(string basePropertyName)
        //: base(_defaultErrorMessage)
        {
            _basePropertyName = basePropertyName;
        }

        //Override default FormatErrorMessage Method  
        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, _basePropertyName);
        }

        //Override IsValid  
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                //Get PropertyInfo Object  
                var basePropertyInfo = validationContext.ObjectType.GetProperty(_basePropertyName);

                //Get Value of the property  

                //var startDate = (DateTime)basePropertyInfo.GetValue(validationContext.ObjectInstance, null);            
                //var thisDate = (DateTime)value;  

                var sDate = basePropertyInfo.GetValue(validationContext.ObjectInstance, null);
                var eDate = value;

                if (eDate != null)
                {
                    var startDate = ConvertStringToDate(sDate.ToString());
                    var thisDate = ConvertStringToDate(eDate.ToString());

                    //Actual comparision  
                    if (thisDate < startDate)
                    {
                        var message = FormatErrorMessage(validationContext.DisplayName);
                        return new ValidationResult(message);
                    }
                }
                //Default return - This means there were no validation error  
                //return null;
                return ValidationResult.Success;
            }
            catch (Exception ex)
            {
                //var message = FormatErrorMessage(validationContext.DisplayName);
                //return new ValidationResult(message);
                return null;
            }
        }


        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                ValidationType = "maintenanceenddatevalidator"
            };
            rule.ValidationParameters["date"] = this._basePropertyName;
            yield return rule;
        }


        public DateTime? ConvertStringToDate(string dateToConvert)
        {
            DateTime MyDateTime;
            MyDateTime = new DateTime();
            // MyDateTime = DateTime.Parse(dateToConvert);
            MyDateTime = DateTime.ParseExact(dateToConvert, "dd/MM/yyyy", null);
            //Convert.ToDateTime(dateToConvert);         //DateTime.ParseExact(dateToConver, "dd/MM/yyyy",null);
            return MyDateTime;
        }

    }

    public class CustomMaintenanceRequiredAttribute : ValidationAttribute, IClientValidatable
    {
        public string Property { get; set; }


        public CustomMaintenanceRequiredAttribute(string Property)
        {
            this.Property = Property;

        }


        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, Property);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            System.Reflection.PropertyInfo PropertyInfo = validationContext.ObjectInstance.GetType().GetProperty(Property);


            if (PropertyInfo == null)
            {
                return new ValidationResult(string.Format("Property '{0}' is undefined.", Property));
            }


            object PropertyValue = PropertyInfo.GetValue(validationContext.ObjectInstance, null);

            if (PropertyValue != null)
            {
                int scheme = Convert.ToInt32(PropertyInfo.GetValue(validationContext.ObjectInstance, null).ToString());

                if (scheme == 2 && value == null)
                {
                    return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
                }
            }

            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                //This is the name of the method aaded to the jQuery validator method (must be lower case)
                ValidationType = "custommanerequired"
            };

            rule.ValidationParameters["fieldvaluemane"] = this.Property;
            yield return rule;

        }
    }//end custom validation


    public class MaintenanceTechnologyDetailsViewModel
    {
        public MaintenanceTechnologyDetailsViewModel()
        {
            ListTechnology = new List<MASTER_TECHNOLOGY>();
            ListLayers = new List<SelectListItem>();
        }

        public string EncryptedProposalSegmentCode { get; set; }
        public int MANE_CONTRACT_CODE { get; set; }
        public int IMS_PR_ROAD_CODE { get; set; }
        public int IMS_SEGMENT_NO { get; set; }
        public decimal IMS_PR_SANCTIONED_LENGTH { get; set; }

        [Required(ErrorMessage = "Start Chainage is required.")]
        [Display(Name = "Start Chainage")]
        [Range(0.000, 9999.999, ErrorMessage = "Please enter valid Start Chainage")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "Invalid Start Chainage,only 3 digits after decimal place is allowed.")]
        [CompareChainage("IMS_END_CHAINAGE", ErrorMessage = "Start chainage must be less than end chainage")]
        public decimal IMS_START_CHAINAGE { get; set; }

        [Required(ErrorMessage = "End Chainage is required.")]
        [Display(Name = "End Chainage")]
        [Range(0.001, 9999.999, ErrorMessage = "Please enter valid End Chainage")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "Invalid End Chainage,only 3 digits after decimal place is allowed.")]
        [CompareChainage("IMS_PR_SANCTIONED_LENGTH", ErrorMessage = "End chainage must be less than Sanctioned Length")]
        public decimal IMS_END_CHAINAGE { get; set; }

        [Required(ErrorMessage = "Total Cost of Technology  is required.")]
        [Display(Name = "Total Cost of Technology")]
        [Range(0.0100, 999999.9999, ErrorMessage = "Please enter valid Total Cost of Technology")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid Total Cost of Technology,only 4 digits after decimal place is allowed.")]
        public decimal IMS_TECH_COST { get; set; }

        [Required(ErrorMessage = "Please select Layer")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select Layer")]
        [Display(Name = "Layer")]
        public int MAST_LAYER_CODE { get; set; }

        [Required(ErrorMessage = "Cost of Technology for Layer is required.")]
        [Display(Name = "Cost of Technology for Layer")]
        [Range(0.0100, 999999.9999, ErrorMessage = "Please enter valid Cost of Technology for Layer")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid Cost of Technology for Layer,only 4 digits after decimal place is allowed.")]
        public decimal IMS_LAYER_COST { get; set; }

        [Required(ErrorMessage = "Please Select Technology")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select Technology")]
        [Display(Name = "Technology")]
        public int MAST_TECH_CODE { get; set; }

        public virtual IMS_SANCTIONED_PROJECTS IMS_SANCTIONED_PROJECTS { get; set; }
        public virtual MASTER_EXECUTION_ITEM MASTER_EXECUTION_ITEM { get; set; }
        public virtual MASTER_TECHNOLOGY MASTER_TECHNOLOGY { get; set; }

        public string Operation { get; set; }
        public string EncryptedProposalCode { get; set; }

        public List<MASTER_TECHNOLOGY> ListTechnology { get; set; }
        public List<SelectListItem> ListLayers { get; set; }


    }




    public class WorkStatusReport
    {
        [Display(Name = "State")]
        [Required(ErrorMessage = "Please select State")]
        [Range(1, 40, ErrorMessage = "Please select a valid State")]

        public int stateCode { get; set; }
        public List<SelectListItem> lstStates { set; get; }

        [Display(Name = "District")]
        public int districtCode { get; set; }
        public List<SelectListItem> lstDistricts { set; get; }

        [Display(Name = "Scheme")]
        public int Scheme { get; set; }
        public List<SelectListItem> lstScheme { set; get; }
    }

}