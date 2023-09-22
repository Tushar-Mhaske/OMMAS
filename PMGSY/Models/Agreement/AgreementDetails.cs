using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using PMGSY.DAL.Agreement;
using PMGSY.Models.Master;
using PMGSY.Models.MaintenanceAgreement;
using PMGSY.Extensions;

namespace PMGSY.Models.Agreement
{
    public class AgreementDetails:IValidatableObject
    {

        public AgreementDetails()
        {
            PMGSYScheme = PMGSYSession.Current.PMGSYScheme;
        }

        public string AgreementAllowOrNot { get; set; }

        [UIHint("Hidden")]
        
        public string EncryptedIMSPRRoadCode { get; set; }

        [UIHint("Hidden")]
        public string EncryptedTendAgreementCode { get; set; }

        [UIHint("Hidden")]
        public string EncryptedAgreementType_Add { get; set; }     

        public int MAST_STATE_CODE { get; set; }

        public int MAST_DISTRICT_CODE { get; set; }


        [Display(Name = "Agreement")]
        public string TEND_AGREEMENT_TYPE { get; set; }

        [Display(Name = "Contractor")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select contractor.")] 
        public int MAST_CON_ID { get; set; }

        [Display(Name = "Agreement Number")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Please select agreement number.")]
        public int TEND_AGREEMENT_CODE { get; set; }

        // name change by rohit borse on 01-07-2022 // range change on 15-07-2022
        [Display(Name = "Tender Amount for Package excluding GST (Rs in Lakhs)")]
        [Required(ErrorMessage = "Tender Amount for Package is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Tender Amount is not in valid format. ")]
        [Range(0.01, 99999.99, ErrorMessage = "Tender Amount is not in valid format.")]
        public decimal? TEND_TENDER_AMOUNT { get; set; }

        [Display(Name = "Agreement Number")]
        //[RegularExpression(@"^([a-zA-Z0-9 ._-]+)$", ErrorMessage = "Agreement Number is not in valid format.")]
        //[RegularExpression(@"^[a-zA-Z0-9 -/()._]+$", ErrorMessage = "Agreement Number is not in valid format.")]
        [RegularExpression(@"^[a-zA-Z0-9]+[a-zA-Z0-9-/()._ ]*$", ErrorMessage = "Agreement Number should contains at least one character or number and starts with alphanumeric value.")]
        [Required(ErrorMessage = "Agreement Number is required.")]
        [StringLength(100, ErrorMessage = "Agreement Number must be less than 100 characters.")]
        public string TEND_AGREEMENT_NUMBER { get; set; }

        public string SanctionedDate { get; set; }

        [Display(Name = "Agreement Date")]
        [Required(ErrorMessage = "Agreement Date is required.")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Agreement Date must be in dd/mm/yyyy format.")]
        [DateValidationVST("TEND_DATE_OF_WORK_ORDER", ErrorMessage = "Agreement date must be greater than or equal to work order date.")]
        [MaintenanceDateValidation("SanctionedDate", ErrorMessage = "Agreement date must be greater than or equal to proposal sanctioned date.")]
        public string TEND_DATE_OF_AGREEMENT { get; set; }

        [Display(Name = "Agreement Start Date")]
        [Required(ErrorMessage = "Agreement Start Date is required.")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Agreement Start Date must be in dd/mm/yyyy format.")]
        [DateValidationVST("TEND_DATE_OF_AGREEMENT", ErrorMessage = "Agreement start date must be greater than or equal to agreement date.")]
        public string TEND_AGREEMENT_START_DATE { get; set; }

        [Display(Name = "Agreement End Date")]
        [Required(ErrorMessage = "Agreement End Date is required.")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Agreement End Date must be in dd/mm/yyyy format.")]
        [DateValidationVST("TEND_AGREEMENT_START_DATE", ErrorMessage = "Agreement end date must be greater than or equal to agreement start date.")]
        [MaintenanceDateValidation("TEND_DATE_OF_COMPLETION", ErrorMessage = "Agreement end date must be greater than or equal to expected completion date.")]
        public string TEND_AGREEMENT_END_DATE { get; set; }

        // name change by rohit borse on 01-07-2022 // range change on 15-07-2022
        // [Display(Name = "Agreement Amount  without (GST) for the Road/LSB (Rs in Lakhs)")]
        [Display(Name = "Agreement Amount Without(GST) for the Road/LSB (Rs in Lakhs)")]
        //[Display(Name = "Total Agreement Amount (Rs in Lakhs)")]
       // [Required(ErrorMessage = "Agreement Amount  without (GST) for the Road/LSB (Rs in Lakhs) is required.")]
        [Required(ErrorMessage = "Agreement Amount Without(GST) for the Road/LSB (Rs in Lakhs) is required.")]
        //[RegularExpression(@"^\d{1,7}\.\d{0,2}$", ErrorMessage = "Agreement Amount is not in valid format. ")]
        //[CompareShare("EncryptedIMSPRRoadCode", "TEND_STATE_SHARE", "TEND_MORD_SHARE", "EncryptedAgreementType_Add", ErrorMessage = "State Share and MoRD Share percentage must be according to the proposal.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Amount is not in valid format. ")]
        //[Range(0.01, 9999999999999999.99, ErrorMessage = "Amount is not in valid format.")]
        [Range(0.01, 99999.99, ErrorMessage = "Amount is not in valid format.")]
        public decimal? TEND_AGREEMENT_AMOUNT { get; set; }


        public string TEND_IS_AGREEMENT_FINALIZED { get; set; }

        [Display(Name = "Award Work Date")]
        [Required(ErrorMessage = "Award Work Date is required.")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Award Work Date must be in dd/mm/yyyy format.")] 
        public string TEND_DATE_OF_AWARD_WORK { get; set; }

        [Display(Name = "Work Order Date")]
        [Required(ErrorMessage = "Work Order Date is required.")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Work Order Date must be in dd/mm/yyyy format.")]
        [DateValidationVST("TEND_DATE_OF_AWARD_WORK", ErrorMessage = "Work order date must be greater than or equal to award of work order date.")]
        public string TEND_DATE_OF_WORK_ORDER { get; set; }

        [Display(Name = "Commencement Date")]
        [Required(ErrorMessage = "Commencement Date is required.")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Commencement Date must be in dd/mm/yyyy format.")]
        [DateValidationVST("TEND_AGREEMENT_START_DATE", ErrorMessage = "Commencement date must be greater than or equal to agreement start date.")]
        
        public string TEND_DATE_OF_COMMENCEMENT { get; set; }
        
        [Display(Name = "Expected Completion Date")]
        [Required(ErrorMessage = "Expected Completion Date is required.")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Expected Completion Date must be in dd/mm/yyyy format.")]
        [MaintenanceDateValidation("TEND_DATE_OF_COMMENCEMENT", ErrorMessage = "Expected Completion date must be greater than or equal to commencement date.")]        
        public string TEND_DATE_OF_COMPLETION { get; set; }

        //================================= change by rohit borse on 01-07-2022 =================================
        [Display(Name = "Maintenance Cost Year1 Including (GST) (Rs in Lakhs)")]
        // [RegularExpression(@"^\d{1,7}\.\d{0,2}$", ErrorMessage = "Maintenance Cost is not in valid format. ")]
        [Required(ErrorMessage = "Maintenance Cost Year1 Including (GST) is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Maintenance Cost Year1 Including (GST) is not in valid format. ")]
        //[Range(0, 9999999999999999.99, ErrorMessage = "Maintenance Cost Year1 is not in valid format.")]
        [Range(0, 99999.99, ErrorMessage = "Maintenance Cost Year1 Including (GST) is not in valid format.")]
        public decimal? TEND_AMOUNT_YEAR1 { get; set; }

         [Display(Name = "Maintenance Cost Year2 Including (GST) (Rs in Lakhs)")]
        [Required(ErrorMessage = "Maintenance Cost Year2 Including (GST) (Rs in Lakhs)")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Maintenance Cost Year2 Including (GST) is not in valid format. ")]
        //[Range(0, 9999999999999999.99, ErrorMessage = "Maintenance Cost Year2 is not in valid format.")]
        [Range(0, 99999.99, ErrorMessage = "Maintenance Cost Year2 Including (GST) is not in valid format.")]
        public decimal? TEND_AMOUNT_YEAR2 { get; set; }

        [Display(Name = "Maintenance Cost Year3 Including (GST) (Rs in Lakhs) ")]
        [Required(ErrorMessage = "Maintenance Cost Year3 Including (GST) is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Maintenance Cost Year3 Including (GST) is not in valid format. ")]
        //[Range(0, 9999999999999999.99, ErrorMessage = "Maintenance Cost Year3 is not in valid format.")]
        [Range(0, 99999.99, ErrorMessage = "Maintenance Cost Year3 Including (GST) is not in valid format.")]
        public decimal? TEND_AMOUNT_YEAR3 { get; set; }

        [Display(Name = "Maintenance Cost Year4 Including (GST) (Rs in Lakhs)")]
        [Required(ErrorMessage = "Maintenance Cost Year4 Including (GST) is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Maintenance Cost Year4 Including (GST) is not in valid format. ")]
        //[Range(0, 9999999999999999.99, ErrorMessage = "Maintenance Cost Year4 is not in valid format.")]
        [Range(0, 99999.99, ErrorMessage = "Maintenance Cost Year4 Including (GST) is not in valid format.")]
        public decimal? TEND_AMOUNT_YEAR4 { get; set; }

        [Display(Name = "Maintenance Cost Year5 Including (GST) (Rs in Lakhs)")]
        [Required(ErrorMessage = "Maintenance Cost Year5 Including (GST) is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Maintenance Cost Year5 Including (GST) is not in valid format. ")]
        //[Range(0, 9999999999999999.99, ErrorMessage = "Maintenance Cost Year5 is not in valid format.")]
        [Range(0, 99999.99, ErrorMessage = "Maintenance Cost Year5 Including (GST) is not in valid format.")]
        public decimal? TEND_AMOUNT_YEAR5 { get; set; }
        
        //================================= Changes END ================================= 

        [Display(Name = "Renewal Cost (Rs in Lakhs)")]
        //[Required(ErrorMessage = "Renewal Cost is required.")]
        [CustomRequired("PMGSYScheme", "EncryptedAgreementType_Add", ErrorMessage = "Renewal Cost is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Renewal Cost is not in valid format. ")]
        //[Range(0, 9999999999999999.99, ErrorMessage = "Renewal Cost is not in valid format.")]
        [Range(0, 99999.99, ErrorMessage = "Renewal Cost is not in valid format.")]
        public decimal? TEND_AMOUNT_YEAR6 { get; set; }


       // [Display(Name = "Part Agreement")]
        [Display(Name = "Split Work")]
        public string TEND_PART_AGREEMENT { get; set; }

        [Display(Name = "Start Chainage")]

        [RegularExpression(@"^\d*(\.\d{1,3})?", ErrorMessage = "Start Chainage is not in valid format e.g.[9.999].")]
       // [CustomRequired("IsPartAgreement", ErrorMessage = "Start Chainage is required.")]
        [Range(0, 9999.999, ErrorMessage = "Start Chainage is not in valid format.")]
        [ChainageRequired("TEND_END_CHAINAGE", ErrorMessage = "Start Chainage is required.")]
        public decimal? TEND_START_CHAINAGE { get; set; }

        [Display(Name = "End Chainage")]

        [RegularExpression(@"^\d*(\.\d{1,3})?", ErrorMessage = "End Chainage is not in valid formate.g.[9.999].")]
        [CompareFieldValidator("TEND_START_CHAINAGE", ErrorMessage = "End Chainage must be greater than start chainage.")]
        [ChainageRequired("TEND_START_CHAINAGE", ErrorMessage = "End Chainage is required.")]
       // [Range(0.001, Double.MaxValue, ErrorMessage = "End Chainage should be greater than 0.")]
        [Range(0.001, 9999.999, ErrorMessage = "End Chainage is not in valid format.")]
        public decimal? TEND_END_CHAINAGE { get; set; }

        [Display(Name = "Remark")]
        [RegularExpression(@"^([a-zA-Z0-9 ._/,()-]+)$", ErrorMessage = "Remarks is not in valid format.")]
        [StringLength(2000, ErrorMessage = "Remark must be less than 2000 characters.")]
        public string TEND_AGREEMENT_REMARKS { get; set; }
        
        public string TEND_AGREEMENT_STATUS { get; set; }
        public string TEND_LOCK_STATUS { get; set; }

        [Display(Name = "Work")]
        //[Range(1, Int32.MaxValue, ErrorMessage = "Please select work.")]
        public int IMS_WORK_CODE { get; set; }

        [IsBooleanValidator(ErrorMessage = "Please select valid option")]
        public bool AgreementType { get; set; }
        public bool IsPartAgreement { get; set; }
        public string Mast_Con_Sup_Flag { get; set; }

        [Range(1, 4, ErrorMessage = "Scheme is invalid.")]//[Range(1,3,ErrorMessage = "Scheme is invalid.")]
        public int PMGSYScheme { get; set; }


        [MaxLength(100, ErrorMessage = "Payee Name can be atmost 100 characters long")]
        [RegularExpression(@"[a-zA-z./&() ]{1,100}", ErrorMessage = "invalid Payee Name  (Contractor)")]
        [Display(Name = "Payee Name(Contractor)")]
        public string PAYEE_NAME_C { get; set; }

        [Display(Name = "Payee Name (Supplier) ")]
        [RegularExpression(@"[a-zA-z./&() ]{1,100}", ErrorMessage = "invalid Payee Name  (Supplier)")]
        [MaxLength(100, ErrorMessage = "Payee Name can be atmost 100 characters long")]
        //[RequiredIf("MAST_SUPPLIER_REQ", true, ErrorMessage = "Payee Name is Required")]
        public string PAYEE_NAME_S { get; set; }



        [Display(Name="State Cost (Rs in Lakhs)")]
        [CustomRequired("PMGSYScheme", "EncryptedAgreementType_Add", ErrorMessage = "State Cost is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "State Cost is not in valid format. ")]
        [Range(0, 9999999999999999.99, ErrorMessage = "State Cost is not in valid format.")]
        public Nullable<decimal> TEND_STATE_SHARE { get; set; }

        [Display(Name = "MoRD Cost (Rs in Lakhs)")]
        [CustomRequired("PMGSYScheme", "EncryptedAgreementType_Add", ErrorMessage = "MoRD Cost is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "MoRD Cost is not in valid format. ")]
        [Range(0, 9999999999999999.99, ErrorMessage = "MoRD Cost is not in valid format.")]
        public Nullable<decimal> TEND_MORD_SHARE { get; set; }

        [Display(Name = "Higher Specification Cost (Rs in Lakhs)")]
        //[CustomRequired("PMGSYScheme", "EncryptedAgreementType_Add", ErrorMessage = "Higher Specification Cost is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Higher Specification Cost is not in valid format. ")]
        [Range(0, 9999999999999999.99, ErrorMessage = "Higher Specification Cost is not in valid format.")]
        public Nullable<decimal> TEND_HIGHER_SPEC_AMT { get; set; }

        [Display(Name="Proposal State Share %")]
        public string ProposalStateShare { get; set; }

        [Display(Name = "Proposal Mord Share %")]
        public string ProposalMordShare { get; set; }

        [Display(Name = "Proposal State Cost including Higher Specification (Rs. in Lakhs)")]
        public decimal ProposalStateCost { get; set; }

        [Display(Name = "Proposal Mord Cost (Rs. in Lakhs)")]
        public decimal ProposalMordCost { get; set; }
        
        public short SHARE_PERCENT { get; set; }

        public bool IncludeRoadAmount { get; set; }

        // added by saurabh jojare on 01-01-2022
        public int Awardstatus { get; set; }

        #region ================================================================ Added By Rohit Borse on 01-07-2022 ========================================================

        [Display(Name = "GST Amount of Tender(Rs in Lakhs)")]
        [Required(ErrorMessage = "GST Amount - Maintenance Agreement is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "GST Amount - Maintenance Agreement is not in valid format. ")]
        [Range(0.00, 99999.99, ErrorMessage = "GST Amount - Maintenance Agreement is not in valid format.")]
        public decimal GST_AMT_MAINTAINANCE_AGREEMENT { get; set; }

        [Display(Name = "GST Amount of Agreement for Road/LSB (Rs in Lakhs)")]
        [Required(ErrorMessage = "GST Amount - Maintenance Agreement DLP is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "GST Amount - Maintenance Agreement DLP is not in valid format. ")]
        [Range(0.00, 99999.99, ErrorMessage = "GST Amount - Maintenance Agreement DLP is not in valid format.")]
        public decimal GST_AMT_MAINTAINANCE_AGREEMENT_DLP { get; set; }


        [Display(Name = "Additional Performance Security (APS) Collected")]
        public string APS_COLLECTED { get; set; }

        [Display(Name = "If APS collected, Amount (Rs in Lakhs)")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "APS collected, Amount is not in valid format. ")]
        [Range(0.01, 99999.99, ErrorMessage = "APS collected, Amount is not in valid format.")]
        public decimal? APS_COLLECTED_AMOUNT { get; set; }

        #endregion

        public SelectList Contractors
        {
            get
            {
                int stateCode = PMGSY.Extensions.PMGSYSession.Current.StateCode;
                List<MASTER_CONTRACTOR> contractorList = new List<MASTER_CONTRACTOR>();

                AgreementDAL agreementDAL = new AgreementDAL();

                contractorList = agreementDAL.GetAllContractor(stateCode, this.Mast_Con_Sup_Flag, false);

                return new SelectList(contractorList, "MAST_CON_ID", "MAST_CON_COMPANY_NAME", this.MAST_CON_ID);

            }
        }

        public SelectList AgreementNumbers
        {
            get
            {
                List<TEND_AGREEMENT_MASTER> agreementNumbersList = new List<TEND_AGREEMENT_MASTER>();

                AgreementDAL agreementDAL = new AgreementDAL();

                agreementNumbersList = agreementDAL.GetAgreementNumbers(this.MAST_CON_ID,this.EncryptedAgreementType_Add, false);

                return new SelectList(agreementNumbersList, "TEND_AGREEMENT_CODE", "TEND_AGREEMENT_NUMBER");

            }
        }

        public SelectList ProposalWorks
        {
            get
            {
                List<IMS_PROPOSAL_WORK> proposalWorkList = new List<IMS_PROPOSAL_WORK>();

                AgreementDAL agreementDAL = new AgreementDAL();

                proposalWorkList = agreementDAL.GetProposalWorks(this.EncryptedIMSPRRoadCode, this.EncryptedAgreementType_Add, false, true);

                return new SelectList(proposalWorkList, "IMS_WORK_CODE", "IMS_WORK_DESC");

            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (PMGSYSession.Current.PMGSYScheme == 2)
            {
                string AgreementType = EncryptedAgreementType_Add;
                Dictionary<string, string> decryptedParameters = null;
                String[] encryptedParameters = null;
                encryptedParameters = AgreementType.Split('/');
                decryptedParameters = PMGSY.Common.URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                string agreementType = decryptedParameters["AgreementType"].ToString().Trim();

                if (TEND_AMOUNT_YEAR6 == null && (agreementType == "O" || agreementType == "C"))
                {
                    yield return new ValidationResult("Renewal Cost is required.");
                }
            }
            else
            {
                yield return ValidationResult.Success;
            }
        }
    }

    //class for custom validation
    public class CustomRequiredAttribute : ValidationAttribute, IClientValidatable
    {
        public string Property { get; set; }
        public string PropertyType { get; set; }


        public CustomRequiredAttribute(string Property,string propertyType)
        {
            this.Property = Property;
            this.PropertyType = propertyType;
        }


        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, Property);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            System.Reflection.PropertyInfo PropertyInfo = validationContext.ObjectInstance.GetType().GetProperty(Property);
            System.Reflection.PropertyInfo PropertyTypeInfo = validationContext.ObjectInstance.GetType().GetProperty(PropertyType);

            if (PropertyTypeInfo == null)
            {
                return ValidationResult.Success;
            }

            string AgreementType = PropertyTypeInfo.GetValue(validationContext.ObjectInstance, null).ToString();
            Dictionary<string, string> decryptedParameters = null;
            String[] encryptedParameters = null;
            encryptedParameters = AgreementType.Split('/');
            decryptedParameters = PMGSY.Common.URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
            string agreementType = decryptedParameters["AgreementType"].ToString().Trim();


            if (PropertyInfo == null)
            {
                return new ValidationResult(string.Format("Property '{0}' is undefined.", Property));
            }

            if (agreementType == "S" || agreementType == "D")
            {
                return ValidationResult.Success;
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
                ValidationType = "customrequired"
            };

            rule.ValidationParameters["fieldvalue"] = this.Property;
            yield return rule;

        }
    }//end custom validation


    public class ChainageRequiredAttribute : ValidationAttribute, IClientValidatable
    {
        public string Property { get; set; }


        public ChainageRequiredAttribute(string Property)
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
                //bool propertyValue = Convert.ToBoolean(PropertyInfo.GetValue(validationContext.ObjectInstance, null).ToString());

                if (value == null)
                {
                    return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
                }
            }

            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            //yield return new ModelClientValidationRule
            //{
            //    ErrorMessage = FormatErrorMessage(metadata.DisplayName),
            //    //This is the name of the method aaded to the jQuery validator method (must be lower case)
            //    ValidationType = "endchainagerequired"


            //};

            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                ValidationType = "chainagerequired"
            };
            rule.ValidationParameters["chainage"] = this.Property;
            yield return rule;

        }
    }//end custom validation

    public class CompareShareAttribute : ValidationAttribute, IClientValidatable
    {
        public string PropertyProposal { get; set; }
        public string PropertyStateShare { get; set; }
        public string PropertyMoRDShare { get; set; }
        public string PropertyAgreementType { get; set; }


        public CompareShareAttribute(string PropertyRoad,string PropertyState,String PropertyMord,string PropertyType)
        {
            this.PropertyProposal = PropertyRoad;
            this.PropertyStateShare = PropertyState;
            this.PropertyMoRDShare = PropertyMord;
            this.PropertyAgreementType = PropertyType;
        }


        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, PropertyStateShare);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            System.Reflection.PropertyInfo PropertyStateInfo = validationContext.ObjectInstance.GetType().GetProperty(PropertyStateShare);
            System.Reflection.PropertyInfo PropertyMordInfo = validationContext.ObjectInstance.GetType().GetProperty(PropertyMoRDShare);
            System.Reflection.PropertyInfo PropertyProposalInfo = validationContext.ObjectInstance.GetType().GetProperty(PropertyProposal);
            System.Reflection.PropertyInfo PropertyAgreementInfo = validationContext.ObjectInstance.GetType().GetProperty(PropertyAgreementType);

            if (PropertyAgreementInfo == null)
            {
                return ValidationResult.Success;
            }

            string AgreementType = PropertyAgreementInfo.GetValue(validationContext.ObjectInstance, null) == null ? "" : PropertyAgreementInfo.GetValue(validationContext.ObjectInstance, null).ToString();
            Dictionary<string, string> decryptedParameters = null;
            String[] encryptedParameters = null;
            encryptedParameters = AgreementType.Split('/');
            decryptedParameters = PMGSY.Common.URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
            string agreementType = decryptedParameters["AgreementType"].ToString().Trim();


            if (PMGSYSession.Current.PMGSYScheme == 1)
            {
                return ValidationResult.Success;
            }
            else if (agreementType.ToString().Trim() == "S" || agreementType.ToString().Trim() == "D")
            {
                return ValidationResult.Success;
            }
            
            

            if (PropertyStateInfo == null)
            {
                return new ValidationResult(string.Format("Property '{0}' is undefined.", PropertyStateInfo));
            }

            if (PropertyMordInfo == null)
            {
                return new ValidationResult(string.Format("Property '{0}' is undefined.", PropertyMordInfo));
            }


            string PropertyProposalValue = PropertyProposalInfo.GetValue(validationContext.ObjectInstance, null) == null ? "" : PropertyProposalInfo.GetValue(validationContext.ObjectInstance, null).ToString();
            object PropertyStateValue = PropertyStateInfo.GetValue(validationContext.ObjectInstance, null) == null ? 0 : PropertyStateInfo.GetValue(validationContext.ObjectInstance, null);
            object PropertyMoRDValue = PropertyMordInfo.GetValue(validationContext.ObjectInstance, null) == null ? 0 : PropertyMordInfo.GetValue(validationContext.ObjectInstance, null);

            AgreementDAL objDAL = new AgreementDAL();
            

            if (value != null)
            {
                short sharePercent = objDAL.GetSharePercent(PropertyProposalValue);
                if (PropertyStateValue != null && PropertyMoRDValue != null)
                {
                    decimal? stateValue = Convert.ToDecimal(PropertyStateInfo.GetValue(validationContext.ObjectInstance, null).ToString());
                    decimal? mordValue = Convert.ToDecimal(PropertyMordInfo.GetValue(validationContext.ObjectInstance, null).ToString());
                    decimal? totalValue = stateValue + mordValue;
                    if (sharePercent == 1)
                    {
                        if (stateValue != Math.Round(Convert.ToDecimal(totalValue * Convert.ToDecimal(0.10))) && mordValue != Math.Round(Convert.ToDecimal(totalValue * Convert.ToDecimal(0.90)),2))
                        {
                            return new ValidationResult(this.FormatErrorMessage("State Share & MoRD Share must be 25% & 75% of total agreement amount."));
                        }
                        else
                        {
                            return ValidationResult.Success;
                        }
                    }
                    else if (sharePercent == 2)
                    {
                        if (stateValue != Math.Round(Convert.ToDecimal(totalValue * Convert.ToDecimal(0.25))) && mordValue != Math.Round(Convert.ToDecimal(totalValue * Convert.ToDecimal(0.75))))
                        {
                            return new ValidationResult(this.FormatErrorMessage("State Share & MoRD Share must be 25% & 75% of total agreement amount."));
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
            }
            else
            {
                return new ValidationResult(string.Format("Share Percentage is invalid."));
            }

            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                //This is the name of the method aaded to the jQuery validator method (must be lower case)
                ValidationType = "compareshare"
            };

            rule.ValidationParameters["statevalue"] = this.PropertyStateShare;
            rule.ValidationParameters["mordvalue"] = this.PropertyMoRDShare;
            yield return rule;

        }
    }//end custom validation


}