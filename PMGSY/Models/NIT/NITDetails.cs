using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using PMGSY.DAL.NIT;
namespace PMGSY.Models.NIT
{
    public class NITDetails
    {

        [UIHint("Hidden")]
        public string EncryptedTendNITCode { get; set; }
 
        public string CurrentDate {
            get
            {
                return DateTime.Now.ToString("dd/MM/yyyy");
            }

            set
            {
                this.CurrentDate = DateTime.Now.ToString("dd/MM/yyyy");
            }
        }

      //  public string CurrentDate = DateTime.Now.ToString("dd/MM/yyyy");

        [Display(Name = "Funding Agency")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select funding agency.")]
        public int TEND_COLLABORATION { get; set; }

        [Display(Name = "Draft NIT Number")]
        //[RegularExpression(@"^[a-zA-Z0-9 -/,._]+$", ErrorMessage = "Draft NIT Number is not in valid format.")]
        [RegularExpression(@"^[a-zA-Z0-9]+[a-zA-Z0-9 ,-/()._]*$", ErrorMessage = "Draft NIT Number should contains at least one character or number and starts with alphanumeric value.")]
        [Required(ErrorMessage = "Draft NIT Number is required.")]
        [StringLength(100, ErrorMessage = "Draft NIT Number must be less than 100 characters.")] 
        public string TEND_NIT_NUMBER { get; set; }

        [Display(Name = "NIT Publication Date")]
        [Required(ErrorMessage = "NIT Publication Date is required.")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "NIT Publication Date must be in dd/mm/yyyy format.")]
        [DateValidationVST("CurrentDate", ErrorMessage = "NIT Publication Date must be greater than or equal to current date.")]
        public string TEND_PUBLICATION_DATE { get; set; }

        [Display(Name = "Tender Form Issue Start Date")]
        [Required(ErrorMessage = "Tender Form Issue Start Date is required.")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Tender Form Issue Start Date must be in dd/mm/yyyy format.")]
        [DateValidationVST("CurrentDate", ErrorMessage = "Tender Form Issue Start Date must be greater than or equal to current date.")]
        public string TEND_ISSUE_START_DATE { get; set; }

        [Display(Name = "Tender Form Issue End Date")]
        [Required(ErrorMessage = "Tender Form Issue End Date is required.")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Tender Form Issue End Date must be in dd/mm/yyyy format.")]
        [DateValidationVST("TEND_ISSUE_START_DATE", ErrorMessage = "Tender Form Issue End Date must be greater than or equal to tender form issue start date.")]    
        public string TEND_ISSUE_END_DATE { get; set; }

        [Display(Name = "Time")]
        [Required(ErrorMessage = "Tender Form Issue Start Time is required.")]
       //[DataType(DataType.Time, ErrorMessage = "Tender Issue Start Time must be in hh:mm format.")]
        [RegularExpression(@"^([0-9]|0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "Tender Form Issue Start Time must be in hh:mm format.")]
        public string TEND_ISSUE_START_TIME { get; set; }

        [Display(Name = "Time")]
        [Required(ErrorMessage = "Tender Form Issue End Time is required.")]
        //[DataType(DataType.Time, ErrorMessage = "Tender Issue End Time must be in hh:mm format.")]
        [RegularExpression(@"^([0-9]|0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "Tender Form Issue End Time must be in hh:mm format.")]
        public string TEND_ISSUE_END_TIME { get; set; }

        [Display(Name = "[Designation of Authority Inviting Bids]")]
       // [RegularExpression(@"^[a-zA-Z0-9 -,().]+$", ErrorMessage = "Inviting Authority Name is not in valid format.")]
        [RegularExpression(@"^[a-zA-Z0-9 ,-.]+$", ErrorMessage = "Inviting Authority Name is not in valid format.")]
        [Required(ErrorMessage = "Inviting Authority Name is required.")]
        [StringLength(100, ErrorMessage = "Inviting Authority Name must be less than 100 characters.")]
        public string TEND_INVITING_AUTHORITY { get; set; }


        [Display(Name = "invites on behalf of")]
        [RegularExpression(@"^[a-zA-Z0-9 ,-.]+$", ErrorMessage = "Inviting Organization Name is not in valid format.")]
        [Required(ErrorMessage = "Inviting Organization Name is required.")]
        [StringLength(100, ErrorMessage = "Inviting Organization Name must be less than 100 characters.")]
        public string TEND_INVITING_ORG { get; set; }

        public string TEND_ITEM_RATE { get; set; }

        public bool TendItemRate { get; set; }

      
        [RegularExpression(@"^[a-zA-Z0-9 ,-.]+$", ErrorMessage = "Registered Organization Name is not in valid format.")]
        [Required(ErrorMessage = "Registered Organization Name is required.")]
        [StringLength(255, ErrorMessage = "Registered Organization Name must be less than 255 characters.")]
        public string CONT_REGN_WITH_ORG { get; set; }


        [Required(ErrorMessage = "Contractor Registration Date is required.")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Contractor Registration Date must be in dd/mm/yyyy format.")]
        [CurrentDateValidation("CurrentDate", ErrorMessage = "Contractor Registration Date must be less than or equal to current date.")] 
        public string CONT_REGN_VALIDITY_DATE { get; set; }

        [Required(ErrorMessage = "Contractor Registration Time is required.")]
       // [DataType(DataType.Time, ErrorMessage = "Contractor Registration Time must be in hh:mm format.")]
        [RegularExpression(@"^([0-9]|0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "Contractor Registration Time must be in hh:mm format.")]
        public string CONT_REGN_VALIDITY_TIME { get; set; }


        [RegularExpression(@"^[a-zA-Z0-9 ,.-]+$", ErrorMessage = "Tender Payment DD Issue in favour of is not in valid format.")]
        [Required(ErrorMessage = "Tender Payment DD Issue in favour of is required.")]
        [StringLength(100, ErrorMessage = "Tender Payment DD Issue in favour of must be less than 100 characters.")]
        public string TEND_DD_ISSUE_IN_FAVOUR { get; set; }


        [RegularExpression(@"^[a-zA-Z0-9 -]+$", ErrorMessage = "Tender Payment DD Payable at is not in valid format.")]
        [Required(ErrorMessage = "Tender Payment DD Payable at is required.")]
        [StringLength(100, ErrorMessage = "Tender Payment DD Payable at must be less than 100 characters.")]
        public string TEND_DD_PAYABLE_AT { get; set; }

       // [Display(Name = "Amount Per Package")]
        [Required(ErrorMessage = "Amount Per Package is required.")]
        [RegularExpression(@"^\d*(\.\d{1,4})?", ErrorMessage = "Amount Per Package is not in valid format. ")]
        [Range(0, 99999999999999.9999, ErrorMessage = "Amount Per Package is not in valid format.")]
        public decimal? TEND_AMOUNT_PER_PACKAGE { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9 ,-.]+$", ErrorMessage = "Inspection Office Name is not in valid format.")]
        [Required(ErrorMessage = "Inspection Office Name is required.")]
        [StringLength(255, ErrorMessage = "Inspection Office Name must be less than 255 characters.")]
        public string TEND_DOC_INSP_OFFICE { get; set; }

        [Display(Name = "Inspection Start Date")]
        [Required(ErrorMessage = "Inspection Start Date is required.")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Inspection Start Date must be in dd/mm/yyyy format.")]
        [DateValidationVST("TEND_ISSUE_END_DATE", ErrorMessage = "Inspection Start Date must be greater than or equal to tender issue end date.")]    
        public string TEND_INSP_START_DATE { get; set; }

        [Display(Name = "Inspection End Date")]
        [Required(ErrorMessage = "Inspection End Date is required.")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Inspection End Date must be in dd/mm/yyyy format.")]
        [DateValidationVST("TEND_INSP_START_DATE", ErrorMessage = "Inspection End Date must be greater than or equal to inspection start date.")]
        public string TEND_INSP_END_DATE { get; set; }


 
        public string TEND_SUBMIT_LAST_DATE { get; set; }

        [Display(Name = "Tender Amount")]
        [RegularExpression(@"^\d*(\.\d{1,4})?", ErrorMessage = "Tender Amount is not in valid format. ")]
        [Range(0, 99999999999999.9999, ErrorMessage = "Tender Amount is not in valid format.")]
        public decimal? TEND_TENDER_AMOUNT { get; set; }

       

        public string TEND_TENDER_TYPE { get; set; }
        public string TEND_PRE_BID_DETAILS { get; set; }
       
          
        public string TEND_PUBLISH_TENDER { get; set; }
        
        public string TEND_LOCK_STATUS { get; set; }



        public SelectList FundingAgencies
        {
            get
            {
                List<MASTER_FUNDING_AGENCY> fundingAgencyList = new List<MASTER_FUNDING_AGENCY>();

                NITDAL objNITDAL = new NITDAL();

                fundingAgencyList = objNITDAL.GetFundingAgencies(false);

                return new SelectList(fundingAgencyList, "MAST_FUNDING_AGENCY_CODE", "MAST_FUNDING_AGENCY_NAME");

            }
        }

    }


    public class CurrentDateValidationAttribute : ValidationAttribute, IClientValidatable
    {

        // private const string _defaultErrorMessage = "Start date must be less than end date.";
        private string _basePropertyName;

        public CurrentDateValidationAttribute(string basePropertyName)
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
                    if (thisDate > startDate)
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
                ValidationType = "currentdatevalidator"
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
}