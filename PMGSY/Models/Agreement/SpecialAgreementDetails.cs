using PMGSY.DAL.Agreement;
using PMGSY.Extensions;
using PMGSY.Models.MaintenanceAgreement;
using PMGSY.Models.Master;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Agreement
{
    public class SpecialAgreementDetails : IValidatableObject
    {

        public SpecialAgreementDetails()
        {
            PMGSYScheme = PMGSYSession.Current.PMGSYScheme;
        }

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

        // Display name change by rohit borse on 01-07-2022 // range change on 15-07-2022
        [Display(Name = "Tender Amount excluding GST (Rs in Lakhs)")]
        [Required(ErrorMessage = "Tender Amount is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Tender Amount is not in valid format. ")]
        [Range(0.01, 99999.99, ErrorMessage = "Tender Amount is not in valid format.")]
        public decimal? TEND_TENDER_AMOUNT { get; set; }

        [Display(Name = "Agreement Number")]
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

        // Display name change by rohit borse on 01-07-2022
        [Display(Name = "Agreement Amount  without (GST) for the Road/LSB (Rs in Lakhs)")]
        [Required(ErrorMessage = "Total Agreement Amount is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Amount is not in valid format. ")]
        [Range(0.01, 9999999999999999.99, ErrorMessage = "Amount is not in valid format.")]
        public decimal? TEND_AGREEMENT_AMOUNT { get; set; }


        public string TEND_IS_AGREEMENT_FINALIZED { get; set; }

        [Display(Name = "Award Work Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Award Work Date must be in dd/mm/yyyy format.")]
        public string TEND_DATE_OF_AWARD_WORK { get; set; }

        [Display(Name = "Work Order Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Work Order Date must be in dd/mm/yyyy format.")]
        [DateValidationVST("TEND_DATE_OF_AWARD_WORK", ErrorMessage = "Work order date must be greater than or equal to award of work order date.")]
        public string TEND_DATE_OF_WORK_ORDER { get; set; }

        [Display(Name = "Commencement Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Commencement Date must be in dd/mm/yyyy format.")]
        [DateValidationVST("TEND_AGREEMENT_START_DATE", ErrorMessage = "Commencement date must be greater than or equal to agreement start date.")]

        public string TEND_DATE_OF_COMMENCEMENT { get; set; }

        [Display(Name = "Expected Completion Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Expected Completion Date must be in dd/mm/yyyy format.")]
        [MaintenanceDateValidation("TEND_DATE_OF_COMMENCEMENT", ErrorMessage = "Expected Completion date must be greater than or equal to commencement date.")]
        public string TEND_DATE_OF_COMPLETION { get; set; }

        //[Display(Name = "Maintenance Cost Year1 (Rs in Lakhs)")]
        //[Required(ErrorMessage = "Maintenance Cost Year1 is required.")]
        //[RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Maintenance Cost Year1 is not in valid format. ")]
        //[Range(0, 9999999999999999.99, ErrorMessage = "Maintenance Cost Year1 is not in valid format.")]
        //public decimal? TEND_AMOUNT_YEAR1 { get; set; }

        //[Display(Name = "Maintenance Cost Year2 (Rs in Lakhs)")]
        //[Required(ErrorMessage = "Maintenance Cost Year2 is required.")]
        //[RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Maintenance Cost Year2 is not in valid format. ")]
        //[Range(0, 9999999999999999.99, ErrorMessage = "Maintenance Cost Year2 is not in valid format.")]
        //public decimal? TEND_AMOUNT_YEAR2 { get; set; }

        //[Display(Name = "Maintenance Cost Year3 (Rs in Lakhs)")]
        //[Required(ErrorMessage = "Maintenance Cost Year3 is required.")]
        //[RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Maintenance Cost Year3 is not in valid format. ")]
        //[Range(0, 9999999999999999.99, ErrorMessage = "Maintenance Cost Year3 is not in valid format.")]
        //public decimal? TEND_AMOUNT_YEAR3 { get; set; }

        //[Display(Name = "Maintenance Cost Year4 (Rs in Lakhs)")]
        //[Required(ErrorMessage = "Maintenance Cost Year4 is required.")]
        //[RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Maintenance Cost Year4 is not in valid format. ")]
        //[Range(0, 9999999999999999.99, ErrorMessage = "Maintenance Cost Year4 is not in valid format.")]
        //public decimal? TEND_AMOUNT_YEAR4 { get; set; }

        //[Display(Name = "Maintenance Cost Year5 (Rs in Lakhs)")]
        //[Required(ErrorMessage = "Maintenance Cost Year5 is required.")]
        //[RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Maintenance Cost Year5 is not in valid format. ")]
        //[Range(0, 9999999999999999.99, ErrorMessage = "Maintenance Cost Year5 is not in valid format.")]
        //public decimal? TEND_AMOUNT_YEAR5 { get; set; }

        //[Display(Name = "Renewal Cost (Rs in Lakhs)")]
        //[CustomRequired("PMGSYScheme", "EncryptedAgreementType_Add", ErrorMessage = "Renewal Cost is required.")]
        //[RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Renewal Cost is not in valid format. ")]
        //[Range(0, 9999999999999999.99, ErrorMessage = "Renewal Cost is not in valid format.")]
        //public decimal? TEND_AMOUNT_YEAR6 { get; set; }

        [Display(Name = "Split Work")]
        public string TEND_PART_AGREEMENT { get; set; }

        [Display(Name = "Start Chainage")]

        [RegularExpression(@"^\d*(\.\d{1,3})?", ErrorMessage = "Start Chainage is not in valid format e.g.[9.999].")]
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

        [Range(1, 4, ErrorMessage = "Scheme is invalid.")]// [Range(1, 2, ErrorMessage = "Scheme is invalid.")]
        public int PMGSYScheme { get; set; }

        [Display(Name = "State Cost (Rs in Lakhs)")]
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
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Higher Specification Cost is not in valid format. ")]
        [Range(0, 9999999999999999.99, ErrorMessage = "Higher Specification Cost is not in valid format.")]
        public Nullable<decimal> TEND_HIGHER_SPEC_AMT { get; set; }

        [Display(Name = "Proposal State Share %")]
        public string ProposalStateShare { get; set; }

        [Display(Name = "Proposal Mord Share %")]
        public string ProposalMordShare { get; set; }

        [Display(Name = "Proposal State Cost including Higher Specification (Rs. in Lakhs)")]
        public decimal ProposalStateCost { get; set; }

        [Display(Name = "Proposal Mord Cost (Rs. in Lakhs)")]
        public decimal ProposalMordCost { get; set; }

        public short SHARE_PERCENT { get; set; }

        #region ============================= Added By Rohit Borse on 01-07-2022 =============================

        [Display(Name = "GST Amount - Maintenance Agreement (Rs in Lakhs)")]
        [Required(ErrorMessage = "GST Amount - Maintenance Agreement is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "GST Amount - Maintenance Agreement is not in valid format. ")]
        [Range(0.00, 99999.99, ErrorMessage = "GST Amount - Maintenance Agreement is not in valid format.")]
        public decimal GST_AMT_MAINTAINANCE_AGREEMENT { get; set; }

        [Display(Name = "GST Amount - Maintenance Agreement DLP (Rs in Lakhs)")]
        [Required(ErrorMessage = "GST Amount - Maintenance Agreement DLP is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "GST Amount - Maintenance Agreement DLP is not in valid format. ")]
        [Range(0.00, 99999.99, ErrorMessage = "GST Amount - Maintenance Agreement DLP is not in valid format.")]
        public decimal GST_AMT_MAINTAINANCE_AGREEMENT_DLP { get; set; }


        [Display(Name = "Additional Performance Security (APS) Collected ")]
        public string APS_COLLECTED { get; set; }

        [Display(Name = "If APS collected, Amount (Rs in Lakhs)")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "APS collected, Amount is not in valid format. ")]
        [Range(0.01, 99999.99, ErrorMessage = "APS collected, Amount is not in valid format.")]
        public decimal? APS_COLLECTED_AMOUNT { get; set; }

        #endregion =======================================================================================

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

                agreementNumbersList = agreementDAL.GetAgreementNumbers(this.MAST_CON_ID, this.EncryptedAgreementType_Add, false);

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

                if ((agreementType == "O" || agreementType == "C"))
                {
                    yield return ValidationResult.Success;
                }
            }
            else
            {
                yield return ValidationResult.Success;
            }
        }
    }
}