using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using PMGSY.DAL.Agreement;
using PMGSY.Models.Master;
using PMGSY.Extensions;

namespace PMGSY.Models.Agreement
{
    public class ExistingAgreementDetails 
    {

        public ExistingAgreementDetails()
        {
            PMGSYScheme = PMGSYSession.Current.PMGSYScheme;
        }

        [UIHint("Hidden")]
        public string EncryptedIMSPRRoadCode_Existing { get; set; }
       
        //[UIHint("Hidden")]
        //public string EncryptedAgreementType_Existing { get; set; }

        [UIHint("Hidden")]
        public string EncryptedTendAgreementCode_Existing { get; set; }

        [UIHint("Hidden")]
        public string EncryptedTendAgreementID_Existing { get; set; }

        public int MAST_STATE_CODE { get; set; }
        public int MAST_DISTRICT_CODE { get; set; }

        [Display(Name = "Agreement Type")]
        public string TEND_AGREEMENT_TYPE { get; set; }

        
      
        //[Display(Name = "Tender Amount")]
        [Display(Name = "Tender Amount for Package excluding GST (Rs in Lakhs)")]   // added by saurabh
        public decimal? TEND_TENDER_AMOUNT { get; set; }

   
        [Display(Name = "Agreement Date")]
      
        public string TEND_DATE_OF_AGREEMENT { get; set; }

        [Display(Name = "Agreement Start Date")] 
        public string TEND_AGREEMENT_START_DATE { get; set; }

        [Display(Name = "Agreement End Date")] 
        public string TEND_AGREEMENT_END_DATE { get; set; }

        // Changes for display name by rohit borse on 01-07-2022
       // [Display(Name = "Agreement Amount without (GST) for the Road / LSB (Rs. in Lakhs)")]  // Agreement Amount for the Road/LSB (Rs in Lakhs)
        [Display(Name = "Agreement Amount for the Road/LSB (Rs in Lakhs)")]  // added by saurabh
        public decimal? TEND_AGREEMENT_AMOUNT_Existing { get; set; }


        public string TEND_IS_AGREEMENT_FINALIZED { get; set; }

        [Display(Name = "Award Work Date")] 
        public string TEND_DATE_OF_AWARD_WORK { get; set; }

        [Display(Name = "Work Order Date")]
        public string TEND_DATE_OF_WORK_ORDER { get; set; }

        [Display(Name = "Commencement Date")]
        public string TEND_DATE_OF_COMMENCEMENT { get; set; }

        [Display(Name = "Completion Date")] 
        public string TEND_DATE_OF_COMPLETION { get; set; }

        // Changes for display name by rohit borse on 01-07-2022 ==================================================================

        // [Display(Name = "Maintenance Cost Year1 without (GST)")]
        [Display(Name = "Maintenance Cost Year1 Including (GST)")]  // added by saurabh
        public decimal? TEND_AMOUNT_YEAR1_Existing { get; set; }

        [Display(Name = "Maintenance Cost Year2 Including (GST)")]  // added by saurabh
        public decimal? TEND_AMOUNT_YEAR2_Existing { get; set; }

        [Display(Name = "Maintenance Cost Year3 Including (GST)")]  // added by saurabh
        public decimal? TEND_AMOUNT_YEAR3_Existing { get; set; }

        [Display(Name = "Maintenance Cost Year4 Including (GST)")]  // added by saurabh
        public decimal? TEND_AMOUNT_YEAR4_Existing { get; set; }

        [Display(Name = "Maintenance Cost Year5 Including (GST)")]  // added by saurabh
        public decimal? TEND_AMOUNT_YEAR5_Existing { get; set; }

        //=======================================================================

        [Display(Name = "Renewal Cost Year6")]
        public decimal? TEND_AMOUNT_YEAR6_Existing { get; set; }

        [Display(Name = "State Cost (Rs in Lakhs)")]
        public Nullable<decimal> TEND_STATE_SHARE_Existing { get; set; }

        [Display(Name = "MoRD Cost (Rs in Lakhs)")]
        public Nullable<decimal> TEND_MORD_SHARE_Existing { get; set; }

        [Display(Name = "Higher Specification Cost (Rs in Lakhs)")]
        public Nullable<decimal> TEND_HIGHER_SPEC_AMT_Existing { get; set; }

        [Range(1, 4, ErrorMessage = "PMGSY Scheme is invalid.")] // [Range(1,3,ErrorMessage="PMGSY Scheme is invalid.")]
        public int PMGSYScheme { get; set; }


        // Changes for display name by rohit borse on 01-07-2022 ==================================================================

        [Display(Name = "Agreement Amount without (GST) for the Road/LSB (Rs in Lakhs)")]
        [Required(ErrorMessage = "Agreement Amount without (GST) is required.")]
        //[RegularExpression(@"^\d{1,7}\.\d{0,2}$", ErrorMessage = "Agreement Amount is not in valid format. ")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Agreement Amount is not in valid format. ")]
        [Range(0, 9999999999999999.99, ErrorMessage = "Agreement Amount is not in valid format.")]
        [CompareShare("EncryptedIMSPRRoadCode_Existing", "TEND_STATE_SHARE_NEW", "TEND_MORD_SHARE_NEW", "EncryptedAgreementType_Add", ErrorMessage = "State Share and MoRD Share percentage must be according to the proposal.")]
        public decimal? TEND_AGREEMENT_AMOUNT_NEW { get; set; }

        //[Display(Name = "Maintenance Cost Year1 without (GST) (Rs in Lakhs)")]
        [Display(Name = "Maintenance Cost Year1 Including (GST) (Rs in Lakhs)")]     // Added by Saurabh
        [Required(ErrorMessage = "Maintenance Cost Year1 Including (GST) is required.")]
        // [RegularExpression(@"^\d{1,7}\.\d{0,2}$", ErrorMessage = "Maintenance Cost is not in valid format. ")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Maintenance Cost Year1 Including (GST) is not in valid format. ")]
        [Range(0, 9999999999999999.99, ErrorMessage = "Maintenance Cost Year1 Including (GST) is not in valid format.")]
        public decimal? TEND_AMOUNT_YEAR1 { get; set; }

       // [Display(Name = "Maintenance Cost Year2 without (GST) (Rs in Lakhs)")]
        [Display(Name = "Maintenance Cost Year2 Including (GST) (Rs in Lakhs)")]    // Added by Saurabh
        [Required(ErrorMessage = "Maintenance Cost Year2 Including (GST) is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Maintenance Cost Year2 Including (GST) is not in valid format. ")]
        [Range(0, 9999999999999999.99, ErrorMessage = "Maintenance Cost Year2 Including (GST) is not in valid format.")]
        public decimal? TEND_AMOUNT_YEAR2 { get; set; }

        //[Display(Name = "Maintenance Cost Year3 without (GST) (Rs in Lakhs)")]
        [Display(Name = "Maintenance Cost Year3 Including (GST) (Rs in Lakhs)")]    // Added by Saurabh
        [Required(ErrorMessage = "Maintenance Cost Year3 Including (GST) is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Maintenance Cost Year3 Including (GST) is not in valid format. ")]
        [Range(0, 9999999999999999.99, ErrorMessage = "Maintenance Cost Year3 Including (GST) is not in valid format.")]
        public decimal? TEND_AMOUNT_YEAR3 { get; set; }

        //[Display(Name = "Maintenance Cost Year4 without (GST) (Rs in Lakhs)")]
        [Display(Name = "Maintenance Cost Year4 Including (GST) (Rs in Lakhs)")]    // Added by Saurabh
        [Required(ErrorMessage = "Maintenance Cost Year4 Including (GST) is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Maintenance Cost Year4 Including (GST) is not in valid format. ")]
        [Range(0, 9999999999999999.99, ErrorMessage = "Maintenance Cost Year4 Including (GST) is not in valid format.")]
        public decimal? TEND_AMOUNT_YEAR4 { get; set; }

        //[Display(Name = "Maintenance Cost Year5 without (GST) (Rs in Lakhs)")]
        [Display(Name = "Maintenance Cost Year5 Including (GST) (Rs in Lakhs)")]   // Added by Saurabh
        [Required(ErrorMessage = "Maintenance Cost Year5 Including (GST) is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Maintenance Cost Year5 Including (GST) is not in valid format. ")]
        [Range(0, 9999999999999999.99, ErrorMessage = "Maintenance Cost Year5 Including (GST) is not in valid format.")]     
        public decimal? TEND_AMOUNT_YEAR5 { get; set; }

        //===================================================================================================

        [Display(Name = "Renewal Cost Year6 (Rs in Lakhs)")]
        [CustomRequired("PMGSYScheme", "EncryptedAgreementType_Add", ErrorMessage = "Renewal Cost Year6 is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Renewal Cost Year6  is not in valid format. ")]
        [Range(0, 9999999999999999.99, ErrorMessage = "Renewal Cost Year6  is not in valid format.")]
        public decimal? TEND_AMOUNT_YEAR6_NEW { get; set; }

        [Display(Name = "Part Agreement")]
        public string TEND_PART_AGREEMENT_Existing { get; set; }

        [Display(Name = "Start Chainage")]
        // [RegularExpression(@"^\d{1,7}\.\d{0,2}$", ErrorMessage = "Start Chainage is not in valid format e.g.[9.99]. ")]
        [RegularExpression(@"^\d*(\.\d{1,3})?", ErrorMessage = "Start Chainage is not in valid formate.g.[9.999].")]
        [ChainageRequired("TEND_END_CHAINAGE_Existing", ErrorMessage = "Start Chainage is required.")]
        [Range(0, 9999.999, ErrorMessage = "Start Chainage is not in valid format.")]
        public decimal? TEND_START_CHAINAGE_Existing { get; set; }

        [Display(Name = "End Chainage")]
        // [RegularExpression(@"^\d{1,7}\.\d{0,2}$", ErrorMessage = "End Chainage is not in valid formate.g.[9.99].")]
        [RegularExpression(@"^\d*(\.\d{1,3})?", ErrorMessage = "End Chainage is not in valid formate.g.[9.999].")]
        [CompareFieldValidator("TEND_START_CHAINAGE_Existing", ErrorMessage = "End Chainage must be greater than start chainage.")]
        [ChainageRequired("TEND_START_CHAINAGE_Existing", ErrorMessage = "End Chainage is required.")]
       // [Range(0.001,Double.MaxValue,ErrorMessage = "End Chainage should be greater than 0.")]
        [Range(0.001, 9999.999, ErrorMessage = "End Chainage is not in valid format.")]
        public decimal? TEND_END_CHAINAGE_Existing { get; set; }

        [Display(Name = "State Cost (Rs in Lakhs)")]
        [CustomExistRequired("PMGSYScheme", ErrorMessage = "State Cost is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "State Cost is not in valid format. ")]
        [Range(0, 9999999999999999.99, ErrorMessage = "State Cost is not in valid format.")]
        public Nullable<decimal> TEND_STATE_SHARE_NEW { get; set; }

        [Display(Name = "MoRD Cost (Rs in Lakhs)")]
        [CustomExistRequired("PMGSYScheme", ErrorMessage = "MoRD Cost is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "MoRD Cost is not in valid format. ")]
        [Range(0, 9999999999999999.99, ErrorMessage = "MoRD Cost is not in valid format.")]
        public Nullable<decimal> TEND_MORD_SHARE_NEW { get; set; }

        [Display(Name = "Higher Specification Cost (Rs in Lakhs)")]
        //[CustomExistRequired("PMGSYScheme", ErrorMessage = "Higher Specification Cost is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Higher Specification Cost is not in valid format. ")]
        [Range(0, 9999999999999999.99, ErrorMessage = "Higher Specification Cost is not in valid format.")]
        public Nullable<decimal> TEND_HIGHER_SPEC_AMT_NEW { get; set; }

        public bool IsPartAgreement_Existing { get; set; }

        [Display(Name = "Work")]
        //[Range(1, Int32.MaxValue, ErrorMessage = "Please select work.")]
        public int IMS_WORK_CODE { get; set; }

        public string IMS_WORK_DESC { get; set; }

        [Display(Name = "Proposal State Share %")]
        public string ProposalStateShare { get; set; }

        [Display(Name = "Proposal Mord Share %")]
        public string ProposalMordShare { get; set; }

        [Display(Name = "Proposal State Cost (Rs. in Lakhs)")]
        public decimal ProposalStateCost { get; set; }

        [Display(Name = "Proposal Mord Cost (Rs. in Lakhs)")]
        public decimal ProposalMordCost { get; set; }

        #region ================================= Added By Rohit Borse on 01-07-2022 =================================

        // [Display(Name = "GST Amount - Maintenance Agreement (Rs in Lakhs)")]
        [Display(Name = "GST Amount of Tender(Rs in Lakhs)")]  // added by saurabh
        [Required(ErrorMessage = "GST Amount - Maintenance Agreement is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "GST Amount - Maintenance Agreement is not in valid format. ")]
        [Range(0, 9999999999999999.99, ErrorMessage = "GST Amount - Maintenance Agreement is not in valid format.")]
        public decimal GST_AMT_MAINTAINANCE_AGREEMENT { get; set; }

       // [Display(Name = "GST Amount - Maintenance Agreement DLP (Rs in Lakhs)")]  // GST Amount of Agreement for Road/LSB (Rs in Lakhs)
        [Display(Name = "GST Amount of Agreement for Road/LSB (Rs in Lakhs)")]   // added by saurabh
        [Required(ErrorMessage = "GST Amount - Maintenance Agreement DLP is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "GST Amount - Maintenance Agreement DLP is not in valid format. ")]
        [Range(0, 9999999999999999.99, ErrorMessage = "GST Amount - Maintenance Agreement DLP is not in valid format.")]
        public decimal GST_AMT_MAINTAINANCE_AGREEMENT_DLP_EXISTING { get; set; }

        //[Display(Name = "GST Amount - Maintenance Agreement DLP (Rs in Lakhs)")]
        [Display(Name = "GST Amount of Agreement for Road/LSB (Rs in Lakhs)")]  // added by saurabh
        [Required(ErrorMessage = "GST Amount - Maintenance Agreement DLP is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "GST Amount - Maintenance Agreement DLP is not in valid format. ")]
        [Range(0, 9999999999999999.99, ErrorMessage = "GST Amount - Maintenance Agreement DLP is not in valid format.")]
        public decimal GST_AMT_MAINTAINANCE_AGREEMENT_DLP_NEW { get; set; }

        [Display(Name = "Additional Performance Security (APS) Collected ")]
        public string APS_COLLECTED { get; set; }

        [Display(Name = "If APS collected, Amount (Rs in Lakhs)")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "APS collected, Amount is not in valid format. ")]
        [Range(0, 9999999999999999.99, ErrorMessage = "APS collected, Amount is not in valid format.")]
        public decimal? APS_COLLECTED_AMOUNT { get; set; }

        #endregion ==================================================================

        public SelectList ProposalWorks
        {
            get
            {
                List<IMS_PROPOSAL_WORK> proposalWorkList = new List<IMS_PROPOSAL_WORK>();

                AgreementDAL agreementDAL = new AgreementDAL();

                proposalWorkList = agreementDAL.GetProposalWorks(this.EncryptedIMSPRRoadCode_Existing, "C", false, true);

                return new SelectList(proposalWorkList, "IMS_WORK_CODE", "IMS_WORK_DESC");

            }
        }
    }

    public class CustomExistRequiredAttribute : ValidationAttribute, IClientValidatable
    {
        public string Property { get; set; }


        public CustomExistRequiredAttribute(string Property)
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
                ValidationType = "customexistrequired"
            };

            //rule.ValidationParameters["fieldexistvalue"] = this.Property;
            yield return rule;

        }
    }//end custom validation
}