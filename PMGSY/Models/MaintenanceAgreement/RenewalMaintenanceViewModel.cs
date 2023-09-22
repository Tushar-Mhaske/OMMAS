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
    public class RenewalMaintenanceViewModel
    {
        public RenewalMaintenanceViewModel()
        {
            PMGSYScheme = PMGSYSession.Current.PMGSYScheme;
        }

        public string ContractorName { get; set; }
        public string WorkName { get; set; }

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
        [MaintenanceDateValidation("MANE_CONSTR_COMP_DATE", ErrorMessage = "Maintenance start date must be greater than or equal to construction completion date.")]        
        public string MANE_MAINTENANCE_START_DATE { get; set; }

        [Display(Name = "Maintenance End Date")]
        [MaintenanceEndDateValidation("MANE_MAINTENANCE_START_DATE", ErrorMessage = "Maintenance end date must be greater than or equal to Maintenance start date.")]        
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Maintenance End Date must be in dd/mm/yyyy format.")]
        public string MANE_MAINTENANCE_END_DATE { get; set; }

        [Display(Name = "Maintenance Agreement Cost (Rs in Lakhs)")]
        [Required(ErrorMessage = "Maintenance Agreement Cost is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Renewal Agreement Cost is not in valid format. ")]
        [Range(0, 9999999999999999.99, ErrorMessage = "Renewal Agreement Cost is not in valid format.")]
        public decimal? MANE_RENEWAL_AMOUNT { get; set; }

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
        [CustomMaintenanceRequired("PMGSYScheme",ErrorMessage="Renewal Cost is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Renewal Cost is not in valid format. ")]
        [Range(0, 9999999999999999.99, ErrorMessage = "Renewal Cost is not in valid format.")]
        public decimal? MANE_YEAR6_AMOUNT { get; set; }

        [Display(Name = "Handover Date")]
       // [Required(ErrorMessage = "Handover Date is required.")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Handover Date must be in dd/mm/yyyy format.")]
        [CheckDate("MANE_MAINTENANCE_START_DATE", "MANE_CONSTR_COMP_DATE", ErrorMessage = "Handover date must be greater than or equal to maintenance start date and construction completion date.")]
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

        public short PMGSYScheme{get;set;}

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



    }
}