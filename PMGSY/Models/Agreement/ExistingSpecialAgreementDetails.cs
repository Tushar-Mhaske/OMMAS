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
    public class ExistingSpecialAgreementDetails
    {

        public ExistingSpecialAgreementDetails()
        {
            PMGSYScheme = PMGSYSession.Current.PMGSYScheme;
        }

        [UIHint("Hidden")]
        public string EncryptedIMSPRRoadCode_Existing { get; set; }

        [UIHint("Hidden")]
        public string EncryptedTendAgreementCode_Existing { get; set; }

        [UIHint("Hidden")]
        public string EncryptedTendAgreementID_Existing { get; set; }

        public int MAST_STATE_CODE { get; set; }
        public int MAST_DISTRICT_CODE { get; set; }

        [Display(Name = "Agreement Type")]
        public string TEND_AGREEMENT_TYPE { get; set; }



        [Display(Name = "Tender Amount")]
        public decimal? TEND_TENDER_AMOUNT { get; set; }


        [Display(Name = "Agreement Date")]

        public string TEND_DATE_OF_AGREEMENT { get; set; }

        [Display(Name = "Agreement Start Date")]
        public string TEND_AGREEMENT_START_DATE { get; set; }

        [Display(Name = "Agreement End Date")]
        public string TEND_AGREEMENT_END_DATE { get; set; }

        [Display(Name = "Amount for the Road / LSB (Rs. in Lakhs)")]
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

        [Display(Name = "State Cost (Rs in Lakhs)")]
        public Nullable<decimal> TEND_STATE_SHARE_Existing { get; set; }

        [Display(Name = "MoRD Cost (Rs in Lakhs)")]
        public Nullable<decimal> TEND_MORD_SHARE_Existing { get; set; }

        [Display(Name = "Higher Specification Cost (Rs in Lakhs)")]
        public Nullable<decimal> TEND_HIGHER_SPEC_AMT_Existing { get; set; }

        [Range(1, 4, ErrorMessage = "PMGSY Scheme is invalid.")] // [Range(1, 2, ErrorMessage = "PMGSY Scheme is invalid.")]
        public int PMGSYScheme { get; set; }

        // change display name by rohit borse on 01-07-2022
        [Display(Name = "Agreement Amount  without (GST) for the Road / LSB (Rs. in Lakhs)")]
        [Required(ErrorMessage = "Agreement Amount is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Agreement Amount is not in valid format. ")]
        [Range(0, 9999999999999999.99, ErrorMessage = "Agreement Amount is not in valid format.")]
        [CompareShare("EncryptedIMSPRRoadCode_Existing", "TEND_STATE_SHARE_NEW", "TEND_MORD_SHARE_NEW", "EncryptedAgreementType_Add", ErrorMessage = "State Share and MoRD Share percentage must be according to the proposal.")]
        public decimal? TEND_AGREEMENT_AMOUNT_NEW { get; set; }

        [Display(Name = "Part Agreement")]
        public string TEND_PART_AGREEMENT_Existing { get; set; }

        [Display(Name = "Start Chainage")]
        [RegularExpression(@"^\d*(\.\d{1,3})?", ErrorMessage = "Start Chainage is not in valid formate.g.[9.999].")]
        [ChainageRequired("TEND_END_CHAINAGE_Existing", ErrorMessage = "Start Chainage is required.")]
        [Range(0, 9999.999, ErrorMessage = "Start Chainage is not in valid format.")]
        public decimal? TEND_START_CHAINAGE_Existing { get; set; }

        [Display(Name = "End Chainage")]
        [RegularExpression(@"^\d*(\.\d{1,3})?", ErrorMessage = "End Chainage is not in valid formate.g.[9.999].")]
        [CompareFieldValidator("TEND_START_CHAINAGE_Existing", ErrorMessage = "End Chainage must be greater than start chainage.")]
        [ChainageRequired("TEND_START_CHAINAGE_Existing", ErrorMessage = "End Chainage is required.")]
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
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Higher Specification Cost is not in valid format. ")]
        [Range(0, 9999999999999999.99, ErrorMessage = "Higher Specification Cost is not in valid format.")]
        public Nullable<decimal> TEND_HIGHER_SPEC_AMT_NEW { get; set; }

        public bool IsPartAgreement_Existing { get; set; }

        [Display(Name = "Work")]
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


        public SelectList ProposalWorks
        {
            get
            {
                List<IMS_PROPOSAL_WORK> proposalWorkList = new List<IMS_PROPOSAL_WORK>();

                AgreementDAL agreementDAL = new AgreementDAL();

                proposalWorkList = agreementDAL.GetProposalWorks(this.EncryptedIMSPRRoadCode_Existing, "R", false, true);

                return new SelectList(proposalWorkList, "IMS_WORK_CODE", "IMS_WORK_DESC");

            }
        }
    }
}