using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using PMGSY.Common;
using PMGSY.DAL;
using PMGSY.Extensions;
using PMGSY.DAL.Agreement;
using PMGSY.Models.Common;
using PMGSY.Models.Master;
using PMGSY.Models.MaintenanceAgreement;

namespace PMGSY.Models.NIT
{
    public class NITRoadDetails
    {
        [UIHint("Hidden")]
        public string EncryptedTendNITCode { get; set; }

        [UIHint("Hidden")]
        public string EncryptedTendNITID { get; set; }

        public string CurrentDate
        {
            get
            {
                return DateTime.Now.ToString("dd/MM/yyyy");
            }

            set
            {
                this.CurrentDate = DateTime.Now.ToString("dd/MM/yyyy");
            }
        }

        [Display(Name = "Sanctioned Year")]
       // [Range(1, Int32.MaxValue, ErrorMessage = "Please select sanctioned year.")]
        public int SanctionYear { get; set; }
        
        [Display(Name = "Package")]
        //[Required(ErrorMessage = "Please select package.")]
        public string PackageID { get; set; }

        [Display(Name = "Road")] 
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select road.")]
        public int IMS_PR_ROAD_CODE { get; set; }

        [Display(Name = "Work")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select work.")]
        public int? IMS_WORK_CODE { get; set; }

        [Display(Name = "Cost of Tender Form (Rs in Lakhs)")]
        [Required(ErrorMessage = "Cost of Tender Form is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Cost of Tender Form is not in valid format. ")]
        [Range(0, 99999999.99, ErrorMessage = "Cost of Tender Form is not in valid format.")]
        public decimal? TEND_COST_FORM { get; set; }

        [Display(Name = "Class of Contractor")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select class of contractor.")]
        public int MAST_CON_CLASS { get; set; }

        [Display(Name = "Earnest Money (Rs in Lakhs)")]
        [Required(ErrorMessage = "Earnest Money is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Earnest Money is not in valid format. ")]
        [Range(0, 99999999.99, ErrorMessage = "Earnest Money is not in valid format.")]
        public decimal? TEND_EARNEST_MONEY { get; set; }


        [Display(Name = "Deadline for Receiving Bids")]
        [Required(ErrorMessage = "Deadline for Receiving Bids Date is required.")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Deadline for Receiving Bids Date must be in dd/mm/yyyy format.")]
        [DateValidationVST("CurrentDate", ErrorMessage = "Deadline for Receiving Bids Date must be greater than or equal to current date.")]   
        public string TEND_RECEVING_DATE { get; set; }

        [Display(Name = "Time")]
        [Required(ErrorMessage = "Deadline for Receiving Bids Time is required.")]
        [RegularExpression(@"^([0-9]|0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "Deadline for Receiving Bids Time must be in hh:mm format.")]
        public string TEND_RECEVING_TIME { get; set; }

        [Display(Name = "Tender Opening Date")]
        [Required(ErrorMessage = "Tender Opening Date is required.")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Tender Opening Date must be in dd/mm/yyyy format.")]
        [DateValidationVST("TEND_RECEVING_DATE", ErrorMessage = "Tender Opening Date must be greater than or equal to receiving bids date.")]
        public string TEND_OPENING_DATE { get; set; }

        [Display(Name = "Time")]
        [Required(ErrorMessage = "Tender Opening Time is required.")]
        [RegularExpression(@"^([0-9]|0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "Tender Opening Time must be in hh:mm format.")]
        public string TEND_OPENING_TIME { get; set; }

        [Display(Name = "Date of Opening of Technical Bid")]
        //[Required(ErrorMessage = "Opening of Techical Bid Date is required.")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Opening of Technical Bid Date must be in dd/mm/yyyy format.")]
        [DateValidationVST("TEND_OPENING_DATE", ErrorMessage = "Opening of Technical Bid Date must be greater than or equal to tender opening date.")]
        public string TEND_DATE_OF_TECHNICAL_OPENING { get; set; }

        [Display(Name = "Time")]
       // [Required(ErrorMessage = "Opening of Techical Bid Time is required.")]
        [RegularExpression(@"^([0-9]|0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "Opening of Techical Bid Time must be in hh:mm format.")]
        public string TEND_TIME_OF_TECHNICAL_OPENING { get; set; }


        [Display(Name = "Date of Opening of Financial Bid")]
        [Required(ErrorMessage = "Opening of Financial Bid Date is required.")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Opening of Financial Bid Date must be in dd/mm/yyyy format.")]
        [DateValidationVST("TEND_DATE_OF_TECHNICAL_OPENING", ErrorMessage = "Opening of Financial Bid Date must be greater than or equal to opening of techical bid date.")]
        public string TEND_DATE_OF_FINANCIAL_OPENING { get; set; }

        [Display(Name = "Time")]
        [Required(ErrorMessage = "Opening of Financial Bid Time is required.")]
        [RegularExpression(@"^([0-9]|0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "Opening of Financial Bid Time must be in hh:mm format.")]
        public string TEND_TIME_OF_FINANCIAL_OPENING { get; set; }

        [Display(Name = "Place of Sale of Tender")]
        [RegularExpression(@"^[a-zA-Z0-9 ,-.]+$", ErrorMessage = "Place of Sale of Tender is not in valid format.")]
        [Required(ErrorMessage = "Place of Sale of Tender is required.")]
        [StringLength(255, ErrorMessage = "Place of Sale of Tender must be less than 255 characters.")]
        public string TEND_PLACE_OF_SALE { get; set; }

        [Display(Name = "Receiving Authority")]
        [RegularExpression(@"^[a-zA-Z0-9 ,-.]+$", ErrorMessage = "Receiving Authority Name is not in valid format.")]
        [Required(ErrorMessage = "Receiving Authority Name is required.")]
        [StringLength(255, ErrorMessage = "Receiving Authority Name must be less than 255 characters.")]
        public string TEND_RECEIVE_AUTH { get; set; }

        [Display(Name = "Pre Bid Details and Place")]
        [RegularExpression(@"^[a-zA-Z0-9 ,-.]+$", ErrorMessage = "Pre Bid Details and Place is not in valid format.")]
        [Required(ErrorMessage = "Pre Bid Details and Place is required.")]
        [StringLength(255, ErrorMessage = "Pre Bid Details and Place must be less than 255 characters.")]
        public string TEND_PRE_BID_DETAILS { get; set; }

        public string TenderIssueStartDate { get; set; }

        [Display(Name = "Date of Pre Bid Meeting")]
        [Required(ErrorMessage = "Pre Bid Meeting Date is required.")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Pre Bid Meeting Date must be in dd/mm/yyyy format.")]
        //[DateValidationVST("TEND_DATE_OF_TECHNICAL_OPENING", ErrorMessage = "Pre Bid Meeting Date must be greater than or equal to opening of techical bid date.")]    
        [MaintenanceDateValidation("TenderIssueStartDate", ErrorMessage = "Pre Bid Meeting date must be greater than or equal to form issue start date.")]
        public string TEND_DATE_OF_PREBID { get; set; }

        [Display(Name = "Time")]
        [Required(ErrorMessage = "Pre Bid Meeting Time is required.")]
        [RegularExpression(@"^([0-9]|0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "Pre Bid Meeting Time must be in hh:mm format.")]
        public string TEND_TIME_OF_PREBID { get; set; }

        [Display(Name = "Place of Opening Bids")]
        [RegularExpression(@"^[a-zA-Z0-9 ,-.]+$", ErrorMessage = "Place of Opening Bids is not in valid format.")]
        [Required(ErrorMessage = "Place of Opening Bids is required.")]
        [StringLength(255, ErrorMessage = "Place of Opening Bids must be less than 255 characters.")]
        public string TEND_PLACE_OF_OPENING { get; set; }

        [Display(Name = "Last Date of Bid Validity")]
        [Required(ErrorMessage = "Bid Validity Date is required.")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Bid Validity Date must be in dd/mm/yyyy format.")]
        [DateValidationVST("TEND_DATE_OF_FINANCIAL_OPENING", ErrorMessage = "Bid Validity Date must be greater than or equal to opening of financial bid date.")]
        public string TEND_DATE_OF_BID_VALIDITY { get; set; }

        [Display(Name = "Time")]
        [Required(ErrorMessage = "Bid Validity Time is required.")]
        [RegularExpression(@"^([0-9]|0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "Bid Validity Time must be in hh:mm format.")]
        public string TEND_TIME_OF_BID_VALIDITY { get; set; }


        [Display(Name = "Total Estimated Cost (Rs in Lakhs)")]
        [Required(ErrorMessage = "Total Estimated Cost is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Total Estimated Cost is not in valid format. ")] 
        [Range(0, 999999999999999999.99, ErrorMessage = "Total Estimated Cost is not in valid format.")]
        [CompareFieldValidator("TEND_EARNEST_MONEY", ErrorMessage = "Total Estimated Cost must be greater than earnest money .")]
        public decimal? TEND_EST_COST { get; set; }

        [Display(Name = "Time Allowed for Completion   (in Months)")]
        [Required(ErrorMessage = "Completion Time is required.")]
        [Range(1, 12, ErrorMessage = "Completion Time should be valid month,range between 1 to 12.")]
        public int? TEND_COMPLETION_TIME { get; set; }

        [Display(Name = "Total Maintenance Cost (Rs in Lakhs)")]
        [Required(ErrorMessage = "Total Maintenance Cost is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Total Maintenance Cost is not in valid format. ")]
        [Range(0, 999999999999999999.99, ErrorMessage = "Total Maintenance Cost is not in valid format.")]
        public decimal? TEND_MAINT_COST { get; set; }


        [Display(Name = "Name and Designation of Contact Person for Site Visit")]
        [RegularExpression(@"^[a-zA-Z0-9 ,-.]+$", ErrorMessage = "Contact Person Name and Designation is not in valid format.")]
        [Required(ErrorMessage = "Contact Person Name and Designation is required.")]
        [StringLength(255, ErrorMessage = "Contact Person Name and Designation must be less than 255 characters.")]
        public string TEND_SITE_CONTACT_PERSON_NAME { get; set; }

        [Display(Name = "Address of Contact Person for Site Visit")]
        [RegularExpression(@"^[a-zA-Z0-9 ,-.]+$", ErrorMessage = "Contact Person Address is not in valid format.")]
        [Required(ErrorMessage = "Contact Person Address is required.")]
        [StringLength(255, ErrorMessage = "Contact Person Address must be less than 255 characters.")]
        public string TEND_SITE_CONTACT_PERSON_ADDRESS { get; set; }

        [Display(Name = "Phone Number (of Contact Person for Site Visit) ")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "STD Code is not in valid format.")]
        [StringLength(5, ErrorMessage = "STD code must be between 3-5 digits.")]
        [Required(ErrorMessage = "STD code is required.")]
        public string TEND_TELE_CODE { get; set; }  

        [Display(Name = "-")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Phone Number should contains digits only.")]
        [StringLength(10, ErrorMessage = "Phone Number exceeds 10 digits")]
        [Required(ErrorMessage = "Phone Number is required.")]
        public string TEND_TELE_NUMBER { get; set; }

        [Display(Name = "Earnest Money to be/Pledged in the name of")]
        [RegularExpression(@"^[a-zA-Z0-9 ,-.]+$", ErrorMessage = "Pledged Earnest Money Name is not in valid format.")]
        [Required(ErrorMessage = "Pledged Earnest Money Name is required.")]
        [StringLength(255, ErrorMessage = "Pledged Earnest Money Name must be less than 255 characters.")]
        public string TEND_PLEDGED_EARN_MONEY_NAMEOF { get; set; }

        [Display(Name = "Extra Cost for Dispatch by Speed/Registered Post (in Rs.) ")]
        [Required(ErrorMessage = "Extra Cost is required.")]
        [RegularExpression(@"^\d+", ErrorMessage = "Extra Cost is not in valid format. ")]
        [StringLength(255, ErrorMessage = "Extra Cost is not in valid format.")]
        public string TEND_EXTRA_COST_FOR_POST { get; set; }

        [Display(Name = "Engineer for the Purposes of this Contract will be (fill up if Engineer is other than Officer Inviting Bids)")]
        [RegularExpression(@"^[a-zA-Z0-9 ,-.]+$", ErrorMessage = "Engineer Name is not in valid format.")]
        //[Required(ErrorMessage = "Pledged Earnest Money Name is required.")]
        [StringLength(255, ErrorMessage = "Engineer Name must be less than 255 characters.")]
        public string TEND_ENGINEER_FOR_CONTRACT { get; set; }

        [Display(Name = "The Submission of Bidding Document will also be allowed at following Places")]
        [RegularExpression(@"^[a-zA-Z0-9 ,-.]+$", ErrorMessage = "Submission of Bidding Document Places is not in valid format.")]
        //[Required(ErrorMessage = "Pledged Earnest Money Name is required.")]
        [StringLength(255, ErrorMessage = "Submission of Bidding Document Places must be less than 255 characters.")]
        public string TEND_OTHER_SUBMISSION_PLACES { get; set; }

        [Display(Name = "Section Completion as per Clause 2.2 of GCC (if any)")]
        [RegularExpression(@"^[a-zA-Z0-9 ,-.]+$", ErrorMessage = "Section Completion is not in valid format.")]
        //[Required(ErrorMessage = "Pledged Earnest Money Name is required.")]
        [StringLength(255, ErrorMessage = "Section Completion must be less than 255 characters.")]
        public string TEND_SECTION_COMPLETION { get; set; }

        [Display(Name = "Site Investigation Reports as per Clause 14 of GCC (if any)")]
        [RegularExpression(@"^[a-zA-Z0-9 ,-.]+$", ErrorMessage = "Site Investigation Reports is not in valid format.")]
        //[Required(ErrorMessage = "Pledged Earnest Money Name is required.")]
        [StringLength(255, ErrorMessage = "Site Investigation Reports must be less than 255 characters.")]
        public string TEND_SITE_INVESTIGATION_REPORT { get; set; }


        public decimal? TEND_FAILURE_DRAWINGS { get; set; }

        public bool isEdit { get; set; }

        public string TEND_APPLICABLE_SCHEDULE_RATE { get; set; }

        

        public SelectList SanctionYears
        {
            get
            {
                CommonFunctions commonFunction = new CommonFunctions();


                return commonFunction.PopulateFinancialYear(true, false);

            }
        }

        public SelectList Packages // List<SelectListItem>
        {
            get
            {
                //TransactionParams transactionParams = new TransactionParams();
                //transactionParams.STATE_CODE = PMGSYSession.Current.StateCode;
                //transactionParams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                //transactionParams.ISSearch = false;
                //transactionParams.SANC_YEAR = 0;// (Int16)DateTime.Now.Year;
                //CommonFunctions commonFunction = new CommonFunctions();
                //return commonFunction.PopulatePackage(transactionParams);


                CommonFunctions commonFunction = new CommonFunctions();
                return new SelectList(commonFunction.GetPackages(this.SanctionYear, 0, false), "IMS_PACKAGE_ID", "IMS_PACKAGE_ID");

            }
        }

        public SelectList ContractorClass
        {
            get
            {
                int stateCode = PMGSYSession.Current.StateCode;
                CommonFunctions commonFunction = new CommonFunctions();
                return new SelectList(commonFunction.GetContractorClassByStateCode(stateCode, false), "MAST_CON_CLASS", "MAST_CON_CLASS_TYPE_NAME");

            }
        }


        public SelectList Roads
        {
            get
            {
                CommonFunctions commonFunction = new CommonFunctions();

                return new SelectList(commonFunction.GetRoads(this.SanctionYear, this.PackageID, false, true, this.isEdit), "IMS_PR_ROAD_CODE", "IMS_ROAD_NAME");

            }
        }

        public SelectList Works
        {
            get
            {
                CommonFunctions commonFunction = new CommonFunctions();

                return new SelectList(commonFunction.GetWorks(this.IMS_PR_ROAD_CODE, false), "IMS_WORK_CODE", "IMS_WORK_DESC");
        
            }
        }



    }
}