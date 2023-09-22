#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   UnlockLSBViewModel.cs
        * Description   :   This View Model is Used in LSB Unlock Update Screen
        * Author        :   Shivkumar Deshmukh
        * Creation Date :   15-07-2013
 **/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PMGSY.Models;
using System.Web.Mvc;

namespace PMGSY.Models.Proposal
{
    public class UnlockLSBViewModel
    {
        public UnlockLSBViewModel()
        {
            BATCHS = new List<SelectListItem>();
            COLLABORATIONS = new List<SelectListItem>();
            STREAMS = new List<SelectListItem>();
        }

        public int IMS_PR_ROAD_CODE { get; set; }

        public string operation { get; set; }
        public bool isAllDetailsEntered { get; set; }
        public string IMS_UPGRADE_CONNECT { get; set; }

        #region STA Sanction Region

        public string STA_SANCTIONED { get; set; }

        [Display(Name = "STA Name")]
        public string STA_SANCTIONED_BY { get; set; }

        [Display(Name = "Scrutiny Date")]
        //[Required(ErrorMessage = "Please Enter Scrutiny Date")]
        public string STA_SANCTIONED_DATE { get; set; }

        [Display(Name = "Remarks")]
        //[Required(ErrorMessage = "Please Enter Remarks")]
        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "Invalid Remarks,Can only contains AlphaNumeric values")]
        public string MS_STA_REMARKS { get; set; }

        #endregion
        [Display(Name = "Year")]
        public int IMS_YEAR { get; set; }
        public List<SelectListItem> Years { set; get; }

        [Display(Name = "Batch")]
        //[Required(ErrorMessage = "Please Select Batch")]
        //[Range(1, 5, ErrorMessage = "Please Select Batch.")]
        public int IMS_BATCH { get; set; }

        public byte PMGSYScheme { get; set; }

        public List<SelectListItem> BATCHS { set; get; }

        [Display(Name = "Name of Block")]
        public int MAST_BLOCK_CODE { get; set; }

        public List<SelectListItem> BLOCKS { get; set; }

        public int MAST_DPIU_CODE { get; set; }

        public string MAST_BLOCK_NAME { get; set; }

        //[Display(Name = "Technology Proposed")]
        [Display(Name = "Stream Proposed")]
        public string MAST_STREAM_NAME { get; set; }
        public string PLAN_RD_NAME { get; set; }

        public string IMS_EXISTING_PACKAGE { get; set; }

        public string isExistingRoad { get; set; }

        [Display(Name = "Package Number")]
        public string IMS_PACKAGE_ID { get; set; }

        public string PACKAGE_PREFIX { get; set; }

        public List<SelectListItem> PACKAGES { get; set; }

        [Display(Name = "Existing Package Number")]
        public string EXISTING_IMS_PACKAGE_ID { get; set; }

        [Display(Name = "Funding Agency")]
        [Required(ErrorMessage = "Please Select Funding Agency")]
        [Range(1, 20, ErrorMessage = "Please Select Funding Agency")]
        public Nullable<int> IMS_COLLABORATION { get; set; }

        public string MAST_FUNDING_AGENCY_NAME { get; set; }

        public List<SelectListItem> COLLABORATIONS { get; set; }

        public bool isPaymentDone { get; set; }

        [Display(Name = "Stream")]
        [Required(ErrorMessage = "Please Select Stream")]
        [Range(1, int.MaxValue, ErrorMessage = "Please Select Stream")]
        public Nullable<int> IMS_STREAMS { get; set; }

        public List<SelectListItem> STREAMS { get; set; }

        public int MAST_STREAM_CODE { get; set; }

        [Display(Name = "Link/Through")]
        public int? PLAN_CN_ROAD_CODE { get; set; }

        public List<SelectListItem> CN_ROADS { get; set; }

        //---LSB Start
        public List<SelectListItem> Existing_Roads_LSB { get; set; }

        [Display(Name = "Staged Year")]
        public Nullable<int> IMS_STAGED_YEAR { get; set; }

        [Display(Name = "Staged Package")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Invalid Package ID, Can only contains AlphaNumeric values")]
        public string IMS_STAGED_PACKAGE_ID { get; set; }

        [Display(Name = "Staged Road")]
        public Nullable<int> IMS_STAGED_ROAD_ID { get; set; }


        [Display(Name = "Road From")]
        [RegularExpression(@"^[a-zA-Z0-9 -/]+$", ErrorMessage = "Invalid Road From, Can only contains AlphaNumeric values")]
        public string IMS_ROAD_FROM { get; set; }

        [Display(Name = "Road To")]
        [RegularExpression(@"^[a-zA-Z0-9 -/]+$", ErrorMessage = "Invalid Road To, Can only contains AlphaNumeric values")]
        public string IMS_ROAD_TO { get; set; }

        [Display(Name = "Road Name")]
        public string IMS_ROAD_NAME { get; set; }

        [Display(Name = "Remarks")]
        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "Invalid Remarks, Can only contains AlphaNumeric values")]
        public string IMS_REMARKS { get; set; }


        [Display(Name = "No. of Bridge Works")]
        public Nullable<int> IMS_NO_OF_BRIDGEWRKS { get; set; }

        [Display(Name = "Bridge Name")]
        // [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "Invalid Bridge Name, Can only contains AlphaNumeric values")]
        // [RegularExpression(@"^(?![0-9]*$)[a-zA-Z0-9 ._(),-/]+$", ErrorMessage = "Bridge Name is not in valid format.")]
        [RegularExpression(@"^(?![0-9]*$)[a-zA-Z0-9 ._(),-/\\]+$", ErrorMessage = "Bridge Name is not in valid format.")]
        [Required]
        public string IMS_BRIDGE_NAME { get; set; }

        [Display(Name = "Bridge Length(in Meters)")]
        public Nullable<decimal> IMS_BRIDGE_LENGTH { get; set; }

        [Display(Name = "MoRD Cost(Rs Lakhs)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid MoRD Cost, Can only contains Numeric values and 4 digits after decimal place")]
        [Range(0.0001, 99999.9999, ErrorMessage = "MoRD Cost too large..")]
        [Required]
        public decimal IMS_SANCTIONED_BW_AMT { get; set; }

        [Display(Name = "State Share(Rs Lakhs)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid State Share, Can only contains Numeric values and 4 digits after decimal place")]
        [Range(0.0000, 999999.9999, ErrorMessage = "State Share Cost too large or small.")]
        [Required]
        public decimal IMS_SANCTIONED_BS_AMT { get; set; }

        [Display(Name = "Total(Rs Lakhs)")]
        public decimal TotalEstimatedCost { get; set; }

        [Required(ErrorMessage = "Please select Share Percent")]
        [Display(Name = "Share Percent")]
        public Nullable<byte> IMS_SHARE_PERCENT_2015 { get; set; }

        public bool IsProposalFinanciallyClosed { get; set; }

        [Display(Name = "State Share (Rs. in Lakhs)")]
        [Required(ErrorMessage = "State Share is required.")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "State Share can only contains Numeric values and 4 digits after decimal place")]
        public decimal IMS_STATE_SHARE_2015 { get; set; }

        [Display(Name = "Mord Share (Rs. in Lakhs)")]
        [Required(ErrorMessage = "Mord Share is required.")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Mord Share can only contains Numeric values and 4 digits after decimal place")]
        public decimal IMS_MORD_SHARE_2015 { get; set; }

        [Display(Name = "Total Cost of Project (Rs. in Lakhs)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Mord Share can only contains Numeric values and 4 digits after decimal place")]
        public decimal IMS_TOTAL_COST_2015 { get; set; }

        [Display(Name = "Total State Share Cost (Rs. in Lakhs)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Total State Share can only contains Numeric values and 4 digits after decimal place")]
        public decimal IMS_TOTAL_STATE_SHARE_2015 { get; set; }


        public string IMS_SANCTIONED { get; set; }

        public Nullable<System.DateTime> IMS_CLEARANCE_DATE { get; set; }
        public string IMS_SANCTIONED_BY { get; set; }
        public Nullable<System.DateTime> IMS_SANCTIONED_DATE { get; set; }
        public Nullable<decimal> IMS_SANCTIONED_AMOUNT { get; set; }
        public string IMS_PROG_REMARKS { get; set; }
        public string IMS_OLD_PACKAGE_ID { get; set; }
        public Nullable<int> IMS_OLD_ROAD_ID { get; set; }
        public Nullable<decimal> IMS_VALUEOFWORK_DONE { get; set; }
        public Nullable<decimal> IMS_PAYMENT_MADE { get; set; }
        public string IMS_FINAL_PAYMENT_FLAG { get; set; }
        public Nullable<System.DateTime> IMS_FINAL_PAYMENT_DATE { get; set; }
        public Nullable<System.DateTime> IMS_ENTRY_DATE_FINANCIAL { get; set; }
        public Nullable<System.DateTime> IMS_ENTRY_DATE_PHYSICAL { get; set; }
        public string IMS_ISCOMPLETED { get; set; }
        public string IMS_LOCK_STATUS { get; set; }
        public string IMS_FREEZE_STATUS { get; set; }
        public string IMS_PROPOSAL_TYPE { get; set; }


        [Display(Name = "Year 1(Rs Lakhs)")]
        [Range(0, 99999, ErrorMessage = "Maintenance Year 1 Cost too Large.")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Maintenance Year 1 ,Can only contains Numeric values and 4 digits after decimal place")]
        public decimal IMS_SANCTIONED_MAN_AMT1 { get; set; }

        [Display(Name = "Year 2(Rs Lakhs)")]
        [Range(0, 99999, ErrorMessage = "Maintenance Year 2 Cost too Large.")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Maintenance Year 2 ,Can only contains Numeric values and 4 digits after decimal place")]
        public decimal IMS_SANCTIONED_MAN_AMT2 { get; set; }

        [Display(Name = "Year 3(Rs Lakhs)")]
        [Range(0, 99999, ErrorMessage = "Maintenance Year 3 Cost too Large.")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Maintenance Year 3 ,Can only contains Numeric values and 4 digits after decimal place")]
        public decimal IMS_SANCTIONED_MAN_AMT3 { get; set; }

        [Display(Name = "Year 4(Rs Lakhs)")]
        [Range(0, 99999, ErrorMessage = "Maintenance Year 4 Cost too Large.")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Maintenance Year 4 ,Can only contains Numeric values and 4 digits after decimal place")]
        public decimal IMS_SANCTIONED_MAN_AMT4 { get; set; }

        [Display(Name = "Year 5(Rs Lakhs)")]
        [Range(0, 99999, ErrorMessage = "Maintenance Year 5 Cost too Large.")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Maintenance Year 5 ,Can only contains Numeric values and 4 digits after decimal place")]
        public decimal IMS_SANCTIONED_MAN_AMT5 { get; set; }

        // <summary>
        /// Total Maintenance Cost
        /// Only for the Display Purpose
        /// </summary>
        [Display(Name = "Total Maintenance Cost(Rs Lakhs)")]
        //[IsStageOneProposal("IMS_STAGE_PHASE", "TotalMaintenanceCost", ErrorMessage = "Total Maintenance Cost Should not be Zero, Please Enter Atleast One Maintenance Cost.")]
        public decimal TotalMaintenanceCost { get; set; }

        [Display(Name = "Renewal Cost Year 6(Rs Lakhs)")]
        [Range(0, 99999, ErrorMessage = "Renewal Cost Year 6 is too Large.")]
        public decimal? IMS_RENEWAL_COST { get; set; }

        public string IMS_STAGE_PHASE { get; set; }

        #region PMGSY Scheme2

        [Display(Name = "Whether any higher specification beyond PMGSY2 Guidelines have been proposed(including utility shifting)?")]
        public string IMS_IS_HIGHER_SPECIFICATION { get; set; }

        [Display(Name = "Higher Specification Cost (Rs Lakhs)")]
        [Range(0, 99999, ErrorMessage = "Higher Specification Cost too small or too large.")]
        [IsValidHigherSpecCostLSB("IMS_IS_HIGHER_SPECIFICATION", ErrorMessage = "Higher Specification Cost should be greater than 0")]
        public Nullable<decimal> IMS_HIGHER_SPECIFICATION_COST { get; set; }

        public Nullable<decimal> IMS_SANCTIONED_HS_AMT { get; set; }

        [Display(Name = "Total Cost (Rs Lakhs) Including Higher Specification Cost")]
        public decimal TotalCostWithHigherSpecCost { get; set; }

        [Display(Name = "Whether the road qualified for sharing of MoRD (90%) & State (10%) as per PMGSY2 Guidelines? (Hilly State, DDP Area, Schedule-V Habitations, BRGF & IAP Districts) ")]
        public Nullable<byte> IMS_SHARE_PERCENT { get; set; }

        #endregion
    }
}