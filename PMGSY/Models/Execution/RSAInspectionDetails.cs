using PMGSY.Models.ExistingRoads;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Execution
{
    public class RSAInspectionDetails
    {
        public decimal TotalSegmentEntered { get; set; }
        public decimal RemainingSegmentLength { get; set; }

        public string encryptedURL { get; set; }
        public String IsFinalized { get; set; }


        public string IsFinalizedByAuditor { get; set; }

        
        [RegularExpression("[EHD]", ErrorMessage = "Please Select valid Priority")]
        public string PriorityCode { get; set; }
        public List<SelectListItem> PriorityList { get; set; }


        // Inspection Details Field
        [RegularExpression("[HML]", ErrorMessage = "Please Select Likelihood (Frequency) of Occurrence")]
        public string LikelihoodCode { get; set; }
        public List<SelectListItem> LikelihoodList { get; set; }


        #region PIU
        public string EncryptedATRId { get; set; }
        public string EncryptedRSAId { get; set; }

        public int ATRId { get; set; }
        public int RSAId { get; set; }

        [RegularExpression("[YN]", ErrorMessage = "Please Select Acceptance")]
        public string AccpetCode { get; set; }
        public List<SelectListItem> AccpetList { get; set; }


       public string ATRUploadDate { get; set; }


        [Display(Name = "Description of Action Taken By PIU")]
        [RegularExpression(@"^[a-zA-Z0-9 ,.()-]+$", ErrorMessage = "Invalid ATR , Can only contains AlphaNumeric values and [,.()-]")]
        [Required(ErrorMessage = "Please enter Description of Action Taken.")]
        public string ATR_By_PIU { get; set; }

        #endregion

        #region Inspection Master


        public string SanctionYear { get; set; }

        public decimal changedLength { get; set; }

        public string EncryptedRoadCode { get; set; }
        public int prRoadCode { get; set; }
        public int cnRoadCode { get; set; }


        [RegularExpression("[PALFXCDNGO]", ErrorMessage = "Please Select valid RSA Stage")]  // DNGO added on 08 Sept 2020
        public string stageCode { get; set; }
        public List<SelectListItem> stageList { get; set; }

        public bool isTSC { get; set; }
        public bool isPIC { get; set; }
        public bool isPIURRNMU { get; set; }

        [Required(ErrorMessage = "Please select Inspection Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Date is not in valid format")]
        public string auditDate { get; set; }



        #endregion 

        #region Inspection Details

        // [RegularExpression("[SRU]", ErrorMessage = "Please Select Option")]
        [Required(ErrorMessage = "Please select Issue Details.")]
        [Range(1, Int16.MaxValue, ErrorMessage = "Please select Issue Details.")]
        public int IssueCode { get; set; }
        public List<SelectListItem> IssueList { get; set; }



        public string RoadStatus { get; set; }
        public string InspectionDate { get; set; }
        
        public int EXEC_RSA_CODE { get; set; }
       


        public string EncyptedProgressCode { get; set; }

        public string Operation { get; set; }

        public int IMS_PR_ROAD_CODE { get; set; }

        public string EXEC_PROGRESS_TYPE { get; set; }


        [Display(Name = "Year")]
        public int EXEC_PROG_YEAR { get; set; }

        [Display(Name = "Month")]
        public int EXEC_PROG_MONTH { get; set; }


        [Display(Name = "Value of Work Upto Last Month(Rs. in Lakh)")]
        public Nullable<decimal> EXEC_VALUEOFWORK_LASTMONTH { get; set; }

        [Display(Name = "Value of Work During This Month(Rs. in Lakh)")]
        public Nullable<decimal> EXEC_VALUEOFWORK_THISMONTH { get; set; }

        [Display(Name = "Payment of Work Upto Last Month(Rs. in Lakh)")]
        public Nullable<decimal> EXEC_PAYMENT_LASTMONTH { get; set; }

        [Display(Name = "Payment of Work During This Month(Rs. in Lakh)")]
        public Nullable<decimal> EXEC_PAYMENT_THISMONTH { get; set; }

        [Display(Name = "Is Final Payment")]
        public string EXEC_FINAL_PAYMENT_FLAG { get; set; }

        [Display(Name = "Final Payment Date")]
        public string EXEC_FINAL_PAYMENT_DATE { get; set; }

        public virtual IMS_SANCTIONED_PROJECTS IMS_SANCTIONED_PROJECTS { get; set; }

        public string BlockName { get; set; }

        public string Package { get; set; }

        public int RoadNo { get; set; }

        public string RoadName { get; set; }

        public double Sanction_Cost { get; set; }

        public double MaintananceCost { get; set; }

        public decimal Sanction_length { get; set; }

        public string CompleteStatus { get; set; }

        public decimal? TotalValueofwork { get; set; }

        public decimal? TotalPayment { get; set; }

        public int PreviousMonth { get; set; }

        public int PreviousYear { get; set; }

        public decimal? AgreementTotal { get; set; }

        public decimal? LastMonthValue { get; set; }

        public decimal? LastPaymentValue { get; set; }

        public decimal? AdditionalCost { get; set; }

        public int AgreementYear { get; set; }

        public string AgreementDate { get; set; }

        public int AgreementMonth { get; set; }

        public decimal? AgreementCost { get; set; }

        public string IsFinalPaymentBefore { get; set; }

     

        //

        [Display(Name = "Start Chainage (Km.)")]
        [Required]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "Start Chainage can only contains Numeric values and 3 digits after decimal place")]
        [Range(0, 9999.999, ErrorMessage = "Invalid Start Chainage")]
        //    [IsValidStartChainage("IMS_PAV_LENGTH", "TO_ROAD_LENGTH", "IMS_PROPOSAL_TYPE", "IMS_ISCOMPLETED", ErrorMessage = "For In-Progress or Completed Road, Start Chainage should be greater than or equal to 0 and less than Road Length. Maximum chainage difference must be 3.000. For Maintenance Road, Start Chainage should be 0.")]
        public decimal StartChainage { get; set; }


        [Display(Name = "End Chainage (Km.)")]
        [Required]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "Invalid End Chainage, can only contains Numeric values and 3 digits after decimal place")]
        [Range(0, 9999.999, ErrorMessage = "Invalid End Chainage")]
        //  [IsValidEndChainage("IMS_PAV_LENGTH", "FROM_ROAD_LENGTH", "IMS_PROPOSAL_TYPE", "IMS_ISCOMPLETED", ErrorMessage = "For In-Progress or Completed Road, End Chainage should be greater than 0 and less than or equal to Road Length. Maximum chainage difference must be 3.000. For Maintenance Road, End Chainage should be equal to Road Length.")]
      //  [CompareValidation("StartChainage", ErrorMessage = "End Chainage must be greater than Start Chainage.")]
       // [CompareEndChainageValidation("StartChainage", "TotalAvailableRoadLength", "EncryptedCBRCode", ErrorMessage = "Segment Length exceeds the Remaining Length.")]
        public decimal EndChainage { get; set; }

        [Display(Name = "Safety Issue")]
        [RegularExpression(@"^[a-zA-Z0-9 :/,.()-]+$", ErrorMessage = "Invalid Safety Issue, Can only contains AlphaNumeric values and [,.()-]")]
        [Required(ErrorMessage = "Please enter Safety Issue details.")]
        public string Safety_Issue { get; set; }


        [Display(Name = "RSA Recommendation")]
        [RegularExpression(@"^[a-zA-Z0-9 :/,.()-]+$", ErrorMessage = "Invalid RSA Recommendation, Can only contains AlphaNumeric values and [,.()-]")]
        [Required(ErrorMessage = "Please enter RSA Recommendation details.")]
        public string RSA_Recommendation { get; set; }


        [RegularExpression("[SRU]", ErrorMessage = "Please Select Option")]
        public string GradeCode { get; set; }
        public List<SelectListItem> GradeList { get; set; }
        //


        [Display(Name = "Remaining Length: ")]
        public decimal? TotalAvailableRoadLength { get; set; }

        [Display(Name = "Total Entered Segment Length: ")]
        public decimal EnteredSegmentLength { get; set; }

        #endregion  Inspection Details

    }
}