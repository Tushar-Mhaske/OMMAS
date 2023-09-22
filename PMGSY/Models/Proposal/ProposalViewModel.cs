#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   ProposalViewModel.cs
        * Description   :   This View Model is Used in Road Proposal Views Create.cshtml,Details.cshtml
        * Author        :   Shivkumar Deshmukh        
        * Creation Date :   08/April/2013
        * Modified By   :   Shyam Yadav
 **/
#endregion


using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PMGSY.Models;
using System.Web.Mvc;
using PMGSY.Extensions;

namespace PMGSY.Models.Proposal
{
    public class ProposalViewModel
    {
        public ProposalViewModel()
        {
            this.IMS_BENEFITED_HABS = new HashSet<IMS_BENEFITED_HABS>();
            this.IMS_CBR_VALUE = new HashSet<IMS_CBR_VALUE>();
            this.IMS_LSB_BRIDGE_COMPONENT_DETAIL = new HashSet<IMS_LSB_BRIDGE_COMPONENT_DETAIL>();
            this.IMS_MLA_PROPOSAL_STATUS = new HashSet<IMS_MLA_PROPOSAL_STATUS>();
            this.IMS_MP_PROPOSAL_STATUS = new HashSet<IMS_MP_PROPOSAL_STATUS>();
            this.IMS_PROPOSAL_FILES = new HashSet<IMS_PROPOSAL_FILES>();
            this.IMS_TRAFFIC_INTENSITY = new HashSet<IMS_TRAFFIC_INTENSITY>();

            Years = new List<SelectListItem>();
            BATCHS = new List<SelectListItem>();
            COLLABORATIONS = new List<SelectListItem>();
            STREAMS = new List<SelectListItem>();
            CN_ROADS = new List<SelectListItem>();
            BLOCKS = new List<SelectListItem>();
            MP_CONSTITUENCY = new List<SelectListItem>();
            MLA_CONSTITUENCY = new List<SelectListItem>();
            CARRIAGED_WIDTH = new List<SelectListItem>();
            HABS_REASON = new List<SelectListItem>();
            EXISTING_SURFACE = new List<SelectListItem>();
            TRAFFIC_TYPE = new List<SelectListItem>();
            PROPOSED_SURFACE = new List<SelectListItem>();
            PACKAGES = new List<SelectListItem>();
            EXISTING_PACKAGES = new List<SelectListItem>();

            PMGSYScheme = PMGSYSession.Current.PMGSYScheme;
        }

        public byte PMGSYScheme { get; set; }

        public string operation { get; set; }

        public string stateType { get; set; }

        public string DistrictType { get; set; }

        public string StateName { get; set; }
        public string DistrictName { get; set; }

        [Display(Name = "Year")]
        [Required]
        [Range(2000, 2099, ErrorMessage = "Please Select Year.")]
        public int IMS_YEAR { get; set; }

        public List<SelectListItem> Years { set; get; }


        [Display(Name = "Batch")]
        [Required(ErrorMessage = "Please Select Batch")]
        [Range(1, 5, ErrorMessage = "Please Select Batch.")]
        public int IMS_BATCH { get; set; }

        public List<SelectListItem> BATCHS { set; get; }


        [Display(Name = "Name of Block")]
        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Please Select Block")]
        public int MAST_BLOCK_CODE { get; set; }

        public List<SelectListItem> BLOCKS { get; set; }


        public int MAST_DPIU_CODE { get; set; }

        public string MAST_BLOCK_NAME { get; set; }

        [Display(Name = "Stream Proposed")]
        public string MAST_STREAM_NAME { get; set; }
        public string PLAN_RD_NAME { get; set; }

        // Radio Button for displaying New Package or Existing Package
        // public string isNewPackage { get; set; }
        public string IMS_EXISTING_PACKAGE { get; set; }

        public List<SelectListItem> EXISTING_PACKAGES { get; set; }

        [Display(Name = "Package Number")]
        [RegularExpression(@"^[a-zA-Z0-9 -/]+$", ErrorMessage = "Invalid Package ID,Can only contains AlphaNumeric values")]
        [IsNewPackage("IMS_EXISTING_PACKAGE", ErrorMessage = "Please Enter Package Number")]
        [IsPackageExists("IMS_YEAR", "IMS_PR_ROAD_CODE", "IMS_EXISTING_PACKAGE", "IMS_PACKAGE_ID", ErrorMessage = "Package Number Already Taken,Please Choose Different Package Number")]
        public string IMS_PACKAGE_ID { get; set; }

        public string PACKAGE_PREFIX { get; set; }


        [Display(Name = "Staged Package Number")]
        [IsStageTwoProposal("IMS_STAGE_PHASE", "IMS_PR_ROAD_CODE", ErrorMessage = "Please Select Staged Package Number")]
        public string Stage_2_Package_ID { get; set; }

        public List<SelectListItem> PACKAGES { get; set; }

        [Display(Name = "Staged Year")]
        [IsStageTwoProposal("IMS_STAGE_PHASE", "IMS_PR_ROAD_CODE", ErrorMessage = "Please Select Staged Year")]
        public int? Stage_2_Year { get; set; }


        [Display(Name = "Existing Package Number")]
        [IsExistingPackage("IMS_EXISTING_PACKAGE", ErrorMessage = "Please Select Package Number")]

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

        //public int MAST_STREAM_CODE { get; set; }

        [Display(Name = "Link/Through")]
        [Required(ErrorMessage = ("Please Select Road"))]
        [Range(0, Int32.MaxValue, ErrorMessage = "Please Select Road")]
        public int? PLAN_CN_ROAD_CODE { get; set; }

        [Display(Name = "Proposal Length")]
        [Required(ErrorMessage = "Please Select Proposal Length")]
        [IsProposalLengthTypeValid("PLAN_CN_ROAD_CODE", "IMS_PAV_LENGTH", "IMS_PR_ROAD_CODE", "IMS_STAGE_PHASE", ErrorMessage = "Proposal Length Type is Invalid.")]
        public string IMS_PARTIAL_LEN { get; set; }

        public List<SelectListItem> CN_ROADS { get; set; }
        public List<SelectListItem> STAGE1_PROPOSAL_ROADS { get; set; }

        [Display(Name = "Road From")]
        [Required]
        //[RegularExpression(@"[A-Za-z ][A-Za-z0-9 ]", ErrorMessage = "Invalid Road From,Can only contains AlphaNumeric values")]

        [RegularExpression(@"^[a-zA-Z0-9 ().-]+$", ErrorMessage = "Invalid Road From,Can only contains AlphaNumeric values")]
        public string IMS_ROAD_FROM { get; set; }

        [Display(Name = "Road To")]
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9 ().-]+$", ErrorMessage = "Invalid Road To,Can only contains AlphaNumeric values")]
        public string IMS_ROAD_TO { get; set; }

        [Display(Name = "Road Name")]
        public string IMS_ROAD_NAME { get; set; }

        /// <summary>
        /// This is Estimated State Share
        /// </summary>
        [Display(Name = "State Cost For Projects Under PMGSY(Rs Lakhs)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid State cost for projects under PMGSY ,Can only contains Numeric values and 4 digits after decimal place")]
        public Nullable<decimal> IMS_STATE_SHARE { get; set; }

        /// <summary>
        ///  This is Sanctioned State Share
        /// </summary>
        [Display(Name = "State Cost For Projects Under PMGSY")]
        public Nullable<decimal> IMS_SANCTIONED_RS_AMT { get; set; }


        [Display(Name = "Pavement Length")]
        [RegularExpression(@"^(?=.+)(?:[0-9]\d*|0)?(?:\.\d+)?$", ErrorMessage = "Invalid Pavement Length,Can only contains Numeric values")]
        //[Range(0.500,double.MaxValue,ErrorMessage="Invalid Pavement Length,Must be greater than 0.5 km.")]
        [IsValidRangeOfPAVLength(ErrorMessage = "Invalid Pavement Length,In case of Scheme 1 it must be greater than 0.5 km. & in case of Scheme 2 it must be greater than 0 km.")]

        // Validation Removed as per Pankaj Kumar's request on 10/06/2014 (Verified by Dev Sir)
        //[IsPavLengthValid("DUP_IMS_PAV_LENGTH", "IMS_STAGE_PHASE", "IMS_CC_LENGTH", "IMS_BT_LENGTH", "IMS_UPGRADE_CONNECT", "operation", "IMS_STAGE1_PAV_LENGTH", ErrorMessage = "Invalid Pavement Length. Please Check the Length.")]        
        public Nullable<decimal> IMS_PAV_LENGTH { get; set; }
        public Nullable<decimal> IMS_STAGE1_PAV_LENGTH { get; set; }
        public Nullable<decimal> DUP_IMS_PAV_LENGTH { get; set; }

        [Display(Name = "Number of CD Works")]
        // [Integer]
        [RegularExpression(@"\d+$", ErrorMessage = "Invalid Number of CD Works")]
        public int? IMS_NO_OF_CDWORKS { get; set; }


        [Display(Name = "Whether ZP Resolution Obtained (Y/N)")]
        [Required]
        public string IMS_ZP_RESO_OBTAINED { get; set; }

        [Display(Name = "If Proposed By MP")]
        public Nullable<int> MAST_MP_CONST_CODE { get; set; }

        public string MAST_MP_CONST_NAME { get; set; }

        public List<SelectListItem> MP_CONSTITUENCY { get; set; }

        [Display(Name = "If Proposed By MLA")]
        public Nullable<int> MAST_MLA_CONST_CODE { get; set; }
        public string MAST_MLA_CONST_NAME { get; set; }

        public List<SelectListItem> MLA_CONSTITUENCY { get; set; }


        [Display(Name = "Proposed Carriage Way Width")]
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Select Proposed Carriage Way Width")]
        public Nullable<int> IMS_CARRIAGED_WIDTH { get; set; }

        public List<SelectListItem> CARRIAGED_WIDTH { get; set; }

        /// <summary>
        /// For only display purpose
        /// </summary>
        public string Display_Carriaged_Width { get; set; }


        [Display(Name = "Traffic Category")]
        [IsStageOneProposal("IMS_STAGE_PHASE", "IMS_TRAFFIC_TYPE", ErrorMessage = "Please Select Traffic Catagory.")]
        public Nullable<int> IMS_TRAFFIC_TYPE { get; set; }

        public string IMS_TRAFFIC_CATAGORY_NAME { get; set; }

        public List<SelectListItem> TRAFFIC_TYPE { get; set; }

        [Display(Name = "Proposed Surface")]
        [Required(ErrorMessage = "Select Proposed Surface")]
        public string IMS_PROPOSED_SURFACE { get; set; }

        public string IMS_PROPOSED_SURFACE_NAME { get; set; }

        public List<SelectListItem> PROPOSED_SURFACE { get; set; }

        [Display(Name = "Remarks")]
        [RegularExpression(@"^[a-zA-Z0-9 ,.()-]+$", ErrorMessage = "Invalid Remarks,Can only contains AlphaNumeric values and [,.()-]")]
        public string IMS_REMARKS { get; set; }

        [Display(Name = "Existing Surface Type")]
        //[Range(1, Int32.MaxValue, ErrorMessage = "Please Select Existing Surface Type")]
        [IsUpgradation("IMS_UPGRADE_CONNECT", ErrorMessage = "Please Select Existing Surface Type")]
        public Nullable<int> MAST_EXISTING_SURFACE_CODE { get; set; }

        public string MAST_EXISTING_SURFACE_NAME { get; set; }

        public List<SelectListItem> EXISTING_SURFACE { get; set; }

        [Display(Name = "Habitations Benifited")]
        public string IMS_ISBENEFITTED_HABS { get; set; }

        [Display(Name = "Reason")]
        public Nullable<int> IMS_REASON { get; set; }
        public string Reason { get; set; }

        [Display(Name = "CC Length")]
        [IsStageOneProposal("IMS_STAGE_PHASE", "IMS_CC_LENGTH", ErrorMessage = "CC Length is Required.")]
        [Range(0, 9999, ErrorMessage = "CC Length too Large.")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "Invalid CC Length,Can only contains Numeric values and Total 7 Digits and 3 digits after decimal place")]
        public Nullable<decimal> IMS_CC_LENGTH { get; set; }

        [Display(Name = "BT Length")]
        //[Required]
        [IsStageOneProposal("IMS_STAGE_PHASE", "IMS_BT_LENGTH", ErrorMessage = "BT Length is Required.")]
        [Range(0, 9999, ErrorMessage = "BT Length too Large.")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "Invalid BT Length,Can only contains Numeric values and Total 7 Digits and 3 digits after decimal place")]
        public Nullable<decimal> IMS_BT_LENGTH { get; set; }


        public string IMS_STAGE_PHASE { get; set; }

        //[Required(ErrorMessage="Please Select Proposal is Complete or Staged.")]
        [IsNewProposal("IMS_UPGRADE_CONNECT", ErrorMessage = "Please Select Proposal is Complete or Staged.")]
        public string IMS_IS_STAGED { get; set; }

        public int IMS_PR_ROAD_CODE { get; set; }
        public int MAST_STATE_CODE { get; set; }
        public int MAST_DISTRICT_CODE { get; set; }
        public int MAST_AGENCY_CODE { get; set; }
        public string IMS_UPGRADE_CONNECT { get; set; }

        /// <summary>
        /// These are the Amount which will never ever edited 
        /// There are Proposed Amounts 
        /// Only for Report Purpose
        /// </summary>

        public decimal IMS_PAV_EST_COST { get; set; }

        public decimal? IMS_CD_WORKS_EST_COST { get; set; }

        public decimal? IMS_PROTECTION_WORKS { get; set; }

        public decimal? IMS_OTHER_WORK_COST { get; set; }


        [Display(Name = "Year 1(Rs Lakhs)")]
        public decimal IMS_MAINTENANCE_YEAR1 { get; set; }
        [Display(Name = "Year 2(Rs Lakhs)")]
        public decimal IMS_MAINTENANCE_YEAR2 { get; set; }
        [Display(Name = "Year 3(Rs Lakhs)")]
        public decimal IMS_MAINTENANCE_YEAR3 { get; set; }
        [Display(Name = "Year 4(Rs Lakhs)")]
        public decimal IMS_MAINTENANCE_YEAR4 { get; set; }
        [Display(Name = "Year 5(Rs Lakhs)")]
        public decimal IMS_MAINTENANCE_YEAR5 { get; set; }

        /// <summary>
        /// Total Maintenance Cost
        /// Only for the Display Purpose
        /// </summary>
        [Display(Name = "Total Maintenance Cost(Rs Lakhs)")]
        [IsStageOneProposal("IMS_STAGE_PHASE", "TotalMaintenanceCost", ErrorMessage = "Total Maintenance Cost Should not be Zero, Please Enter Atleast One Maintenance Cost.")]
        //[IsTotalMaintananceCostValid("IMS_STAGE_PHASE", ErrorMessage = "Total Maintenance Cost Should not be Zero, Please Enter Atleast One Maintenance Cost.")]

        public decimal TotalMaintenanceCost { get; set; }

        /// <summary>
        /// All the Sanctioned Amount 
        /// ONLY these amount will be edited 
        /// </summary>
        /// 

        [Display(Name = "Pavement Cost(Rs Lakhs)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid Pavement Cost,Can only contains Numeric values and Total 8 Digits and 4 digits after decimal place")]
        [Range(1, 99999, ErrorMessage = "Invalid Pavement Cost, Pavement Cost should be greater than 0.")]
        public decimal IMS_SANCTIONED_PAV_AMT { get; set; }

        [Display(Name = "CD Works Cost(Rs Lakhs)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid CD Works Cost,Can only contains Numeric values and 4 digits after decimal place")]
        [Range(0, 99999, ErrorMessage = "Invalid CD Works Sanctioned Cost (Range should be 0-99999)")]
        public decimal IMS_SANCTIONED_CD_AMT { get; set; }

        [Display(Name = "Protection Works Cost(Rs Lakhs)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid Protection Works Cost,Can only contains Numeric values and 4 digits after decimal place")]
        [Range(0, 99999, ErrorMessage = "Invalid Protection Works Cost (Range should be 0-99999)")]
        public decimal IMS_SANCTIONED_PW_AMT { get; set; }

        [Display(Name = "Other Works Cost (if Any)(Rs Lakhs)")]
        [Range(0, 99999, ErrorMessage = "Invalid Other Works Cost (Range should be 0-99999)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Other Works Cost,Can only contains Numeric values and 4 digits after decimal place")]
        public decimal IMS_SANCTIONED_OW_AMT { get; set; }

        // Incase of Bridge Work
        public decimal IMS_SANCTIONED_BW_AMT { get; set; }

        /// <summary>
        ///  Total Cost Only For Display Purpose
        /// </summary>
        [Display(Name = "Total Cost")]
        public string IMS_TOTAL_COST { get; set; }

        [Display(Name = "Year 1(Rs Lakhs)")]
        [Range(0, 99999, ErrorMessage = "Invalid Maintenance Year 1 Cost (Range should be 0-99999)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Maintenance Year 1 ,Can only contains Numeric values and 4 digits after decimal place")]
        public decimal IMS_SANCTIONED_MAN_AMT1 { get; set; }

        [Display(Name = "Year 2(Rs Lakhs)")]
        [Range(0, 99999, ErrorMessage = "Invalid Maintenance Year 2 Cost (Range should be 0-99999)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Maintenance Year 2 ,Can only contains Numeric values and 4 digits after decimal place")]
        public decimal IMS_SANCTIONED_MAN_AMT2 { get; set; }

        [Display(Name = "Year 3(Rs Lakhs)")]
        [Range(0, 99999, ErrorMessage = "Invalid Maintenance Year 3 Cost (Range should be 0-99999)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Maintenance Year 3 ,Can only contains Numeric values and 4 digits after decimal place")]
        public decimal IMS_SANCTIONED_MAN_AMT3 { get; set; }

        [Display(Name = "Year 4(Rs Lakhs)")]
        [Range(0, 99999, ErrorMessage = "Invalid Maintenance Year 4 Cost (Range should be 0-99999)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Maintenance Year 4 ,Can only contains Numeric values and 4 digits after decimal place")]
        public decimal IMS_SANCTIONED_MAN_AMT4 { get; set; }

        [Display(Name = "Year 5(Rs Lakhs)")]
        [Range(0, 99999, ErrorMessage = "Invalid Maintenance Year 5 Cost (Range should be 0-99999)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Maintenance Year 5 ,Can only contains Numeric values and 4 digits after decimal place")]
        public decimal IMS_SANCTIONED_MAN_AMT5 { get; set; }

        #region PMGSY 3 Scheme

        [Display(Name = "Existing Carriageway Width(In Kms)")]
        [Required(ErrorMessage = "Existing Carriageway Width is Required.")]
        [RegularExpression(@"^(?=.+)(?:[0-9]\d*|0)?(?:\.\d+)?$", ErrorMessage = "Invalid Existing Carriageway Width,Can only contains Numeric values")]
        public Nullable<decimal> EXISTING_CARRIAGEWAY_WIDTH { get; set; }


        [Display(Name = "Existing Carriageway PUC")]
        [Required(ErrorMessage = "Existing Carriageway PUC is Required.")]
        [RegularExpression(@"^(?=.+)(?:[0-9]\d*|0)?(?:\.\d+)?$", ErrorMessage = "Invalid Existing Carriageway PUC,Can only contains Numeric values")]
        public Nullable<decimal> EXISTING_CARRIAGEWAY_PUC { get; set; }


        [Display(Name = "Year 6(Rs Lakhs)")]
        [Range(0, 99999, ErrorMessage = "Invalid Maintenance Year 6 Cost (Range should be 0-99999)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Maintenance Year 6 ,Can only contains Numeric values and 4 digits after decimal place")]
        public decimal IMS_MAINTENANCE_YEAR6 { get; set; }

        [Display(Name = "Year 7(Rs Lakhs)")]
        [Range(0, 99999, ErrorMessage = "Invalid Maintenance Year 7 Cost (Range should be 0-99999)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Maintenance Year 7 ,Can only contains Numeric values and 4 digits after decimal place")]
        public decimal IMS_MAINTENANCE_YEAR7 { get; set; }

        [Display(Name = "Year 8(Rs Lakhs)")]
        [Range(0, 99999, ErrorMessage = "Invalid Maintenance Year 8 Cost (Range should be 0-99999)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Maintenance Year 8 ,Can only contains Numeric values and 4 digits after decimal place")]
        public decimal IMS_MAINTENANCE_YEAR8 { get; set; }

        [Display(Name = "Year 9(Rs Lakhs)")]
        [Range(0, 99999, ErrorMessage = "Invalid Maintenance Year 9 Cost (Range should be 0-99999)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Maintenance Year 9 ,Can only contains Numeric values and 4 digits after decimal place")]
        public decimal IMS_MAINTENANCE_YEAR9 { get; set; }

        [Display(Name = "Year 10(Rs Lakhs)")]
        [Range(0, 99999, ErrorMessage = "Invalid Maintenance Year 10 Cost (Range should be 0-99999)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Maintenance Year 10 ,Can only contains Numeric values and 4 digits after decimal place")]
        public decimal IMS_MAINTENANCE_YEAR10 { get; set; }

        /// <summary>
        /// Total Post DPL Maintenance Cost
        /// Only for the Display Purpose
        /// </summary>
        [Display(Name = "Total Post DLP Maintenance Cost(Rs Lakhs)")]
        [IsStageOneProposal("IMS_STAGE_PHASE", "TotalPostDLPMaintenanceCost", ErrorMessage = "Total Post DLP Maintenance Cost Should not be Zero, Please Enter Atleast One Maintenance Cost.")]
        public decimal? TotalPostDLPMaintenanceCost { get; set; }

        [Display(Name = "Pucca Side Drains Length (In Kms)")]
        [Required(ErrorMessage = "Pucca Side Drains Length is Required.")]
        [RegularExpression(@"^(?=.+)(?:[0-9]\d*|0)?(?:\.\d+)?$", ErrorMessage = "Invalid Pucca Side Drains Length,Can only contains Numeric values")]
        public Nullable<decimal> PUCCA_SIDE_DRAIN_LENGTH { get; set; }

        [Display(Name = "Protection Length (In Kms)")]
        [Required(ErrorMessage = "Protection Length is Required.")]
        [RegularExpression(@"^(?=.+)(?:[0-9]\d*|0)?(?:\.\d+)?$", ErrorMessage = "Invalid Protection Length,Can only contains Numeric values")]
        public Nullable<decimal> PROTECTION_LENGTH { get; set; }

        [Display(Name = "Surface Brick Solling (In Kms)")]
        [Required(ErrorMessage = "Surface Brick Solling is Required.")]
        [RegularExpression(@"^(?=.+)(?:[0-9]\d*|0)?(?:\.\d+)?$", ErrorMessage = "Invalid Surface Brick Solling Length,Can only contains Numeric values")]
        public Nullable<decimal> SURFACE_BRICK_SOLLING { get; set; }

        [Display(Name = "Surface BT (In Kms)")]
        [Required(ErrorMessage = "Surface BT is Required.")]
        [RegularExpression(@"^(?=.+)(?:[0-9]\d*|0)?(?:\.\d+)?$", ErrorMessage = "Invalid Surface BT Length,Can only contains Numeric values")]
        public Nullable<decimal> SURFACE_BT { get; set; }

        [Display(Name = "Surface CC (In Kms)")]
        [Required(ErrorMessage = "Surface CC is Required.")]
        [RegularExpression(@"^(?=.+)(?:[0-9]\d*|0)?(?:\.\d+)?$", ErrorMessage = "Invalid Surface CC Length,Can only contains Numeric values")]
        public Nullable<decimal> SURFACE_CC { get; set; }

        [Display(Name = "Surface Gravel (In Kms)")]
        [Required(ErrorMessage = "Surface Gravel is Required.")]
        [RegularExpression(@"^(?=.+)(?:[0-9]\d*|0)?(?:\.\d+)?$", ErrorMessage = "Invalid Surface Gravel Length,Can only contains Numeric values")]
        public Nullable<decimal> SURFACE_GRAVEL { get; set; }

        [Display(Name = "Surface Moorum (In Kms)")]
        [Required(ErrorMessage = "Surface Moorum  is Required.")]
        [RegularExpression(@"^(?=.+)(?:[0-9]\d*|0)?(?:\.\d+)?$", ErrorMessage = "Invalid Surface Moorum  Length,Can only contains Numeric values")]
        public Nullable<decimal> SURFACE_MOORUM { get; set; }

        [Display(Name = "Surface Track (In Kms)")]
        [Required(ErrorMessage = "Surface Track is Required.")]
        [RegularExpression(@"^(?=.+)(?:[0-9]\d*|0)?(?:\.\d+)?$", ErrorMessage = "Invalid Surface Track  Length,Can only contains Numeric values")]
        public Nullable<decimal> SURFACE_TRACK { get; set; }

        [Display(Name = "Surface WBM (In Kms)")]
        [Required(ErrorMessage = "Surface WBM is Required.")]
        [RegularExpression(@"^(?=.+)(?:[0-9]\d*|0)?(?:\.\d+)?$", ErrorMessage = "Invalid Surface WBM Length,Can only contains Numeric values")]
        public Nullable<decimal> SURFACE_WBM { get; set; }
        #endregion

        //[Range(0, double.MaxValue, ErrorMessage = "Total Maintenance Cost Should not be Zero, Please Enter Atleast One Maintenance Cost.")]

        public decimal txtTotalMaintenance { get; set; }

        public Nullable<int> IMS_STAGED_YEAR { get; set; }
        public string IMS_STAGED_PACKAGE_ID { get; set; }

        [Display(Name = "Staged Road")]
        [IsStageTwoProposal("IMS_STAGE_PHASE", "IMS_PR_ROAD_CODE", ErrorMessage = "Please Select Staged Road")]
        public Nullable<int> IMS_STAGED_ROAD_ID { get; set; }

        public Nullable<int> IMS_NO_OF_BRIDGEWRKS { get; set; }
        public string IMS_BRIDGE_NAME { get; set; }
        public Nullable<decimal> IMS_BRIDGE_LENGTH { get; set; }
        public Nullable<decimal> IMS_BRIDGE_WORKS_EST_COST { get; set; }
        public Nullable<decimal> IMS_BRIDGE_EST_COST_STATE { get; set; }

        [Display(Name = "Reason")]
        [IsReasonSelected("IMS_ISBENEFITTED_HABS", ErrorMessage = "Please Select Reason")]
        public Nullable<int> IMS_HABS_REASON { get; set; }

        public List<SelectListItem> HABS_REASON { get; set; }

        public string HABS_REASON_TEXT { get; set; }

        public string IMS_DPR_STATUS { get; set; }

        public string IS_PAYMENT_MADE { get; set; }



        #region PMGSY2 Scheme
        [Display(Name = "Whether any higher specification beyond PMGSY2 Guidelines have been proposed(including utility shifting)?")]
        public string IMS_IS_HIGHER_SPECIFICATION { get; set; }

        [Display(Name = "Higher Specification Cost (Rs Lakhs)")]
        [Range(0, 99999, ErrorMessage = "Invalid Higher Specification Cost (Range should be 0-99999)")]
        [IsValidHigherSpecCost("IMS_IS_HIGHER_SPECIFICATION", ErrorMessage = "Higher Specification Cost should be greater than 0")]
        public Nullable<decimal> IMS_HIGHER_SPECIFICATION_COST { get; set; }

        [Display(Name = "Total Shines (Furniture) Cost(Rs Lakhs)")]
        [Range(0, 99999, ErrorMessage = "Invalid Total Shines (Furniture) Cost (Range should be 0-99999)")]
        public Nullable<decimal> IMS_FURNITURE_COST { get; set; }

        [Display(Name = "Whether the road qualified for sharing of MoRD (90%) & State (10%) as per PMGSY2 Guidelines? (Hilly State, DDP Area, Schedule-V Habitations, BRGF & IAP Districts) ")]
        public Nullable<byte> IMS_SHARE_PERCENT { get; set; }

        [Display(Name = "Renewal Cost (Rs Lakhs)")]
        [Range(0, 99999, ErrorMessage = "Invalid Renewal Cost (Range should be 0-99999)")]
        public Nullable<decimal> IMS_RENEWAL_COST { get; set; }

        [Display(Name = "Sanctioned Higher Specification Cost(Rs Lakhs)")]
        [Range(0, 99999, ErrorMessage = "Invalid Sanctioned Higher Specification Cost (Range should be 0-99999)")]
        public Nullable<decimal> IMS_SANCTIONED_HS_AMT { get; set; }

        [Display(Name = "Sanctioned Furniture Cost(Rs Lakhs)")]
        [Range(0, 99999, ErrorMessage = "Invalid Sanctioned Furniture Cost (Range should be 0-99999)")]
        public Nullable<decimal> IMS_SANCTIONED_FC_AMT { get; set; }

        [Display(Name = "Sanctioned Renewal Cost(Rs Lakhs)")]
        [Range(0, 99999, ErrorMessage = "Invalid Sanctioned Renewal Cost (Range should be 0-99999)")]
        public Nullable<decimal> IMS_SANCTIONED_RENEWAL_AMT { get; set; }

        [Display(Name = "Total Cost(Rs Lakhs / Excluding Higher Specification Cost)")]
        public decimal TotalCost { get; set; }
        #endregion


        #region STA Sanction Region

        public string STA_SANCTIONED { get; set; }

        [Display(Name = "STA Name")]
        public string STA_SANCTIONED_BY { get; set; }

        [Display(Name = "Scrutiny Date")]
        //[Required( ErrorMessage="Please Enter Scrutiny Date")   ]
        public string STA_SANCTIONED_DATE { get; set; }

        [Display(Name = "Remarks")]
        //[Required(ErrorMessage="Please Enter Remarks")]
        //[RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "Invalid Remarks,Can only contains AlphaNumeric values")]
        public string MS_STA_REMARKS { get; set; }

        #endregion

        #region PTA Sanction Region

        public string PTA_SANCTIONED { get; set; }
        public int? PTA_SANCTIONED_BY { get; set; }

        [Display(Name = "PTA Name")]
        public string NAME_OF_PTA { get; set; }

        [Display(Name = "Scrutiny Date")]
        public string PTA_SANCTIONED_DATE { get; set; }

        [Display(Name = "Remarks")]
        public string MS_PTA_REMARKS { get; set; }

        #endregion

        #region PMGSY_1_SHARE_PERCENT_CHANGE

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

        #endregion


        public string IMS_SANCTIONED { get; set; }

        public string IMS_SANCTIONED_BY { get; set; }
        //public Nullable<System.DateTime> IMS_SANCTIONED_DATE { get; set; }
        public string IMS_SANCTIONED_DATE { get; set; }
        [Display(Name = "MoRD Cost (Rs Lakhs)")]
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

        #region PMGSY Scheme 5 (Vibrant Village) - Srishti Tyagi 28/06/2023

        public int imsComponentId { get; set; }

        [IsValidRangeOfRidingQualityLength("IMS_PAV_LENGTH", ErrorMessage = "Riding Quality Length shoud be less than or equal to Pavement Length.")]
        public decimal ImsRidingQualityLength { get; set; }

        [Required(ErrorMessage = "Please enter Clearing Cost")]
        public decimal ImsClearing { get; set; }

        [Required(ErrorMessage = "Please enter Excavation Cost")]
        public decimal ImsExcavation { get; set; }

        [Required(ErrorMessage = "Please enter Filling Cost")]
        public decimal ImsFilling { get; set; }

        [Required(ErrorMessage = "Please enter SubGrade Cost")]
        public decimal ImsSubGrade { get; set; }

        [Required(ErrorMessage = "Please enter Shoulder Cost")]
        public decimal ImsShoulder { get; set; }

        [Required(ErrorMessage = "Please enter Granular Sub Base Cost")]
        public decimal ImsGranularSubBase { get; set; }

        [Required(ErrorMessage = "Please enter Soil Aggregate Cost")]
        public decimal ImsSoilAggregate { get; set; }
        [Required(ErrorMessage = "Please enter WBM Grade II Cost")]
        public decimal ImsWBMGradeII { get; set; }
        [Required(ErrorMessage = "Please enter WBM Grade III Cost")]
        public decimal ImsWBMGradeIII { get; set; }
        [Required(ErrorMessage = "Please enter WMM Cost")]
        public decimal ImsWMM { get; set; }
        [Required(ErrorMessage = "Please enter Prime Coat Cost")]
        public decimal ImsPrimeCoat { get; set; }
        [Required(ErrorMessage = "Please enter Tack Coat Cost")]
        public decimal ImsTackCoat { get; set; }
        [Required(ErrorMessage = "Please enter BDMBM Cost")]
        public decimal ImsBMDBM { get; set; }
        [Required(ErrorMessage = "Please enter OGPC SDBC BC Cost")]
        public decimal ImsOGPC_SDBC_BC { get; set; }
        [Required(ErrorMessage = "Please enter Seal Coat Cost")]
        public decimal ImsSealCoat { get; set; }
        [Required(ErrorMessage = "Please enter Surface Dressing Cost")]
        public decimal ImsSurfaceDressing { get; set; }
        [Required(ErrorMessage = "Please enter Dry Lean Concrete Cost")]
        public decimal ImsDryLeanConcrete { get; set; }
        [Required(ErrorMessage = "Please enter Concrete Payment Cost")]
        public decimal ImsConcretePavement { get; set; }

        [Required(ErrorMessage = "Please enter PUCCA Side Drains Cost")]
        public decimal ImsPuccaSideDrains { get; set; }

        [Required(ErrorMessage = "Please enter GST Cost")]
        public decimal ImsGSTCost { get; set; }
        public string encrProposalCode { get; set; }

        #endregion

        public virtual ADMIN_DEPARTMENT ADMIN_DEPARTMENT { get; set; }
        public virtual ICollection<IMS_BENEFITED_HABS> IMS_BENEFITED_HABS { get; set; }
        public virtual ICollection<IMS_CBR_VALUE> IMS_CBR_VALUE { get; set; }
        public virtual ICollection<IMS_LSB_BRIDGE_COMPONENT_DETAIL> IMS_LSB_BRIDGE_COMPONENT_DETAIL { get; set; }
        public virtual IMS_LSB_BRIDGE_DETAIL IMS_LSB_BRIDGE_DETAIL { get; set; }
        public virtual ICollection<IMS_MLA_PROPOSAL_STATUS> IMS_MLA_PROPOSAL_STATUS { get; set; }
        public virtual ICollection<IMS_MP_PROPOSAL_STATUS> IMS_MP_PROPOSAL_STATUS { get; set; }
        public virtual ICollection<IMS_PROPOSAL_FILES> IMS_PROPOSAL_FILES { get; set; }
        public virtual MASTER_BLOCK MASTER_BLOCK { get; set; }
        public virtual MASTER_DISTRICT MASTER_DISTRICT { get; set; }
        public virtual MASTER_FUNDING_AGENCY MASTER_FUNDING_AGENCY { get; set; }
        public virtual MASTER_MLA_CONSTITUENCY MASTER_MLA_CONSTITUENCY { get; set; }
        public virtual MASTER_MP_CONSTITUENCY MASTER_MP_CONSTITUENCY { get; set; }
        public virtual MASTER_REASON MASTER_REASON { get; set; }
        public virtual MASTER_REASON MASTER_REASON1 { get; set; }
        public virtual MASTER_STATE MASTER_STATE { get; set; }
        public virtual MASTER_STREAMS MASTER_STREAMS { get; set; }
        public virtual MASTER_SURFACE MASTER_SURFACE { get; set; }
        public virtual MASTER_TRAFFIC_TYPE MASTER_TRAFFIC_TYPE { get; set; }
        public virtual PLAN_ROAD PLAN_ROAD { get; set; }
        public virtual ICollection<IMS_TRAFFIC_INTENSITY> IMS_TRAFFIC_INTENSITY { get; set; }
        public virtual MASTER_CARRIAGE MASTER_CARRIAGE { get; set; }


    }

    /// <summary>
    /// This Validation Checks if Habitation is Not Benifitted then Reason is Selected or Not
    /// </summary>
    public class IsReasonSelected : ValidationAttribute, IClientValidatable
    {
        private readonly string PropertyName;

        public IsReasonSelected(string propertyName)
        {
            this.PropertyName = propertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyTestedInfo = validationContext.ObjectType.GetProperty(this.PropertyName);
            if (propertyTestedInfo == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.PropertyName));
            }

            var ishabitationbenifitted = propertyTestedInfo.GetValue(validationContext.ObjectInstance, null);

            if (ishabitationbenifitted == null)
            {
                return ValidationResult.Success;
            }

            if (ishabitationbenifitted.ToString().ToLower() == "n")
            {
                if (value == null || value == "" || Convert.ToInt16(value) == 0)
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
                else
                {
                    return ValidationResult.Success;
                }
            }
            if (ishabitationbenifitted.ToString().ToLower() == "y")
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "isreasonselected"
            };
            rule.ValidationParameters["ishabitationbenifitted"] = this.PropertyName;
            yield return rule;
        }

    }


    /// <summary>
    /// If Proposal is New then only Validate Required for IMS_IS_STAGED
    /// </summary>
    public class IsNewProposal : ValidationAttribute, IClientValidatable
    {
        private readonly string PropertyName;

        public IsNewProposal(string propertyName)
        {
            this.PropertyName = propertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyTestedInfo = validationContext.ObjectType.GetProperty(this.PropertyName);
            if (propertyTestedInfo == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.PropertyName));
            }

            var imsupgrade = propertyTestedInfo.GetValue(validationContext.ObjectInstance, null);

            if (imsupgrade.ToString() == "N")
            {
                if (value == null)
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
                else
                {
                    return ValidationResult.Success;
                }
            }

            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "isupgradation"
            };
            rule.ValidationParameters["imsupgrade"] = this.PropertyName;
            yield return rule;
        }

    }


    public class IsUpgradation : ValidationAttribute, IClientValidatable
    {
        private readonly string PropertyName;

        public IsUpgradation(string propertyName)
        {
            this.PropertyName = propertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyTestedInfo = validationContext.ObjectType.GetProperty(this.PropertyName);
            if (propertyTestedInfo == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.PropertyName));
            }

            var imsupgrade = propertyTestedInfo.GetValue(validationContext.ObjectInstance, null);

            if (imsupgrade.ToString().ToLower() == "u")
            {
                if (value == null)
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
                else
                {
                    return ValidationResult.Success;
                }
            }
            if (imsupgrade.ToString().ToLower() == "n")
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "isupgradation"
            };
            rule.ValidationParameters["imsupgrade"] = this.PropertyName;
            yield return rule;
        }

    }

    public class IsStageTwoProposal : ValidationAttribute, IClientValidatable
    {
        private readonly string PropertyName;
        private readonly string PrRoadCode;

        public IsStageTwoProposal(string propertyName, string prRoadCode)
        {
            this.PropertyName = propertyName;
            this.PrRoadCode = prRoadCode;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                var propertyTestedInfo = validationContext.ObjectType.GetProperty(this.PropertyName);
                var propertyPrRoadCode = validationContext.ObjectType.GetProperty(this.PrRoadCode);
                if (propertyTestedInfo == null)
                {
                    return new ValidationResult(string.Format("unknown property {0}", this.PropertyName));
                }

                var imsstagephase = propertyTestedInfo.GetValue(validationContext.ObjectInstance, null);
                var IMS_PR_ROAD_CODE = propertyPrRoadCode.GetValue(validationContext.ObjectInstance, null);

                if (imsstagephase.ToString().ToLower() == "2" && (Convert.ToInt32(IMS_PR_ROAD_CODE) == 0 || IMS_PR_ROAD_CODE == null))
                {
                    if (value == null)
                    {
                        return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                    }
                    else
                    {
                        return ValidationResult.Success;
                    }
                }
                return ValidationResult.Success;
            }
            catch
            {
                return ValidationResult.Success;
            }
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "isstagetwoproposal"
            };
            rule.ValidationParameters["imsstagephase"] = this.PropertyName;
            yield return rule;
        }

    }

    public class IsNewPackage : ValidationAttribute, IClientValidatable
    {
        private readonly string PropertyName;

        public IsNewPackage(string propertyName)
        {
            this.PropertyName = propertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyTestedInfo = validationContext.ObjectType.GetProperty(this.PropertyName);
            if (propertyTestedInfo == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.PropertyName));
            }

            var isnewpackage = propertyTestedInfo.GetValue(validationContext.ObjectInstance, null);

            if (isnewpackage.ToString() == "N")
            {
                if (value == null)
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
                else
                {
                    return ValidationResult.Success;
                }
            }

            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "isnewpackage"
            };
            rule.ValidationParameters["isnewpackage"] = this.PropertyName;
            yield return rule;
        }

    }

    public class IsExistingPackage : ValidationAttribute, IClientValidatable
    {
        private readonly string PropertyName;

        public IsExistingPackage(string propertyName)
        {
            this.PropertyName = propertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyTestedInfo = validationContext.ObjectType.GetProperty(this.PropertyName);
            if (propertyTestedInfo == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.PropertyName));
            }

            var existingpackage = propertyTestedInfo.GetValue(validationContext.ObjectInstance, null);

            if (existingpackage.ToString() == "E")
            {
                if (value == null || Convert.ToString(value) == "0")
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
                else
                {
                    return ValidationResult.Success;
                }
            }

            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "isexistingpackage"
            };
            rule.ValidationParameters["existingpackage"] = this.PropertyName;
            yield return rule;
        }

    }

    /// <summary>
    /// Validation For Package 
    /// Check if Package Number Already Exists in State, District and Year    
    /// </summary>
    public class IsPackageExists : ValidationAttribute
    {
        private readonly string PropertyName;
        private readonly string PrRoadCode;
        private readonly string IsNewPackage;
        private readonly string CurrentPropertyName;

        public IsPackageExists(string propertyName, string prRoadCode, string isNewPackage, string currentPropertyName)
        {
            this.PropertyName = propertyName;
            this.PrRoadCode = prRoadCode;
            this.IsNewPackage = isNewPackage;
            this.CurrentPropertyName = currentPropertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyTestedInfo = validationContext.ObjectType.GetProperty(this.PropertyName);
            var propertyPrRoadCode = validationContext.ObjectType.GetProperty(this.PrRoadCode);
            var propertyIsNewPackage = validationContext.ObjectType.GetProperty(this.IsNewPackage);

            if (propertyTestedInfo == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.PropertyName));
            }

            var IMS_YEAR = propertyTestedInfo.GetValue(validationContext.ObjectInstance, null);
            var IMS_PR_ROAD_CODE = propertyPrRoadCode.GetValue(validationContext.ObjectInstance, null);
            var IS_NEW_PACKAGE = propertyIsNewPackage.GetValue(validationContext.ObjectInstance, null);

            if ((IS_NEW_PACKAGE.ToString().ToUpper().Equals("N") && CurrentPropertyName.Equals("IMS_PACKAGE_ID")))
            {
                if (new PMGSY.Common.CommonFunctions().IsPackageExists(PMGSY.Extensions.PMGSYSession.Current.StateCode, PMGSY.Extensions.PMGSYSession.Current.AdminNdCode, Convert.ToInt32(IMS_YEAR), value == null ? "0" : value.ToString(), Convert.ToInt32(IMS_PR_ROAD_CODE)))
                {
                    if (value == null)
                    {
                        return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                    }
                    else
                    {
                        return ValidationResult.Success;
                    }
                }
            }
            else
            {
                return ValidationResult.Success;
            }


            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }
    }

    public class IsTotalMaintananceCostValid : ValidationAttribute, IClientValidatable
    {
        private readonly string PropertyName;

        public IsTotalMaintananceCostValid(string propertyName)
        {
            this.PropertyName = propertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                var propertyTestedInfo = validationContext.ObjectType.GetProperty(this.PropertyName);
                if (propertyTestedInfo == null)
                {
                    return new ValidationResult(string.Format("unknown property {0}", this.PropertyName));
                }

                var imsstagephase = propertyTestedInfo.GetValue(validationContext.ObjectInstance, null);

                if (imsstagephase.ToString().ToLower() == "2")
                {
                    if (Convert.ToInt32(value) == 0 || value == null || value.ToString() == "")
                    {
                        return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                    }
                    else
                    {
                        return ValidationResult.Success;
                    }
                }
                return ValidationResult.Success;
            }
            catch
            {
                return ValidationResult.Success;
            }
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "istotalmaintanancecostvalid"
            };
            rule.ValidationParameters["imsstagephase"] = this.PropertyName;
            yield return rule;
        }

    }

    /// <summary>
    /// Validation For Pavement Length
    /// </summary>
    public class IsPavLengthValid : ValidationAttribute, IClientValidatable
    {

        private readonly string DupImsPavLen;
        private readonly string ImsStagePhase;
        private readonly string CCLength;
        private readonly string BTLength;
        private readonly string IMS_UPGRADE_CONNECT;
        private readonly string OperationType;
        private readonly string Stage1PavLength;

        public IsPavLengthValid(string DupImsPavLen, string imsStagePhase, string ccLength, string btLength, string param_IMS_UPGRADE_CONNECT, string paramOperationType, string stage1PavLength)
        {
            this.DupImsPavLen = DupImsPavLen;
            this.ImsStagePhase = imsStagePhase;
            this.CCLength = ccLength;
            this.BTLength = btLength;
            this.IMS_UPGRADE_CONNECT = param_IMS_UPGRADE_CONNECT;
            this.OperationType = paramOperationType;
            this.Stage1PavLength = stage1PavLength;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyDupPavLen = validationContext.ObjectType.GetProperty(this.DupImsPavLen);
            var propertyStagePhase = validationContext.ObjectType.GetProperty(this.ImsStagePhase);
            var propertyCCLength = validationContext.ObjectType.GetProperty(this.CCLength);
            var propertyBTLength = validationContext.ObjectType.GetProperty(this.BTLength);
            var propertyUpgradeConnect = validationContext.ObjectType.GetProperty(this.IMS_UPGRADE_CONNECT);
            var propertyOperationType = validationContext.ObjectType.GetProperty(this.OperationType);
            var propertyStage1PavLength = validationContext.ObjectType.GetProperty(this.Stage1PavLength);

            var IMS_PAV_LEN = propertyDupPavLen.GetValue(validationContext.ObjectInstance, null);
            var IMS_STAGE_PHASE = propertyStagePhase.GetValue(validationContext.ObjectInstance, null);
            var IMS_CC_LEN = propertyCCLength.GetValue(validationContext.ObjectInstance, null);
            var IMS_BT_LEN = propertyBTLength.GetValue(validationContext.ObjectInstance, null);
            var IMS_UPGRADE_CONNECT = propertyUpgradeConnect.GetValue(validationContext.ObjectInstance, null);
            var OperationType = propertyOperationType.GetValue(validationContext.ObjectInstance, null);
            var IMS_STAGE1_PAV_LENGTH = propertyStage1PavLength.GetValue(validationContext.ObjectInstance, null);

            try
            {
                bool isValid = new PMGSY.BAL.Proposal.ProposalBAL().IsPavementLengthValidBAL(Convert.ToDecimal(value), Convert.ToDecimal(IMS_CC_LEN), Convert.ToDecimal(IMS_BT_LEN), IMS_UPGRADE_CONNECT.ToString(), Convert.ToDecimal(IMS_PAV_LEN), OperationType != null ? OperationType.ToString() : "", Convert.ToInt32(IMS_STAGE_PHASE), Convert.ToDecimal(IMS_STAGE1_PAV_LENGTH));

                if (isValid)
                {
                    if (value == null)
                    {
                        return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                    }
                    else
                    {
                        return ValidationResult.Success;
                    }
                }
                else
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
            }
            catch
            {
                return ValidationResult.Success;
            }

        }


        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "ispavlengthvalid"
            };
            rule.ValidationParameters["imspavlen"] = this.DupImsPavLen;
            yield return rule;
        }
    }

    /// <summary>
    /// Check if it is Stage One Proposal For Mandatory Validations
    /// </summary>
    public class IsStageOneProposal : ValidationAttribute, IClientValidatable
    {
        private readonly string PropertyName;
        private readonly string ControlOnWhichValidationApplied;

        public IsStageOneProposal(string propertyName, string propertySelfName)
        {
            this.PropertyName = propertyName;
            this.ControlOnWhichValidationApplied = propertySelfName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                var propertyTestedInfo = validationContext.ObjectType.GetProperty(this.PropertyName);
                if (propertyTestedInfo == null)
                {
                    return new ValidationResult(string.Format("unknown property {0}", this.PropertyName));
                }

                var imsstagephase = propertyTestedInfo.GetValue(validationContext.ObjectInstance, null);

                if (imsstagephase == null)
                {
                    /*
                    IMS_TRAFFIC_TYPE
                    IMS_CC_LENGTH
                    IMS_BT_LENGTH
                    TotalMaintenanceCost
                     */
                    if (ControlOnWhichValidationApplied == "IMS_CC_LENGTH" || ControlOnWhichValidationApplied == "IMS_BT_LENGTH")
                    {
                        /// If value = 0 return false
                        if (value == null || value == "")
                        {
                            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                        }
                        else
                        {
                            return ValidationResult.Success;
                        }
                    }
                    else if (ControlOnWhichValidationApplied == "TotalMaintenanceCost")
                    {
                        if (value == null || value == "" || Convert.ToDecimal(value) == 0)
                        {
                            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                        }
                        else
                        {
                            return ValidationResult.Success;
                        }
                    }
                    else
                    {
                        if (value == null || value == "" || Convert.ToInt32(value) == 0)
                        {
                            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                        }
                        else
                        {
                            return ValidationResult.Success;
                        }
                    }

                }

                if (imsstagephase.ToString().ToLower() == "1")
                {
                    if (value == null)
                    {
                        return ValidationResult.Success;
                    }

                }
                return ValidationResult.Success;
            }
            catch
            {
                return ValidationResult.Success;
            }
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "isstageoneproposal"
            };
            rule.ValidationParameters["imsstagephase"] = this.PropertyName;
            yield return rule;
        }

    }

    /// <summary>
    /// This is Server Side Validation which checks if the Road Construction is Full length then Length of Pavment should match with it.
    /// and if it is partial length construction then Length of road should be less than that of Pavement Length
    /// </summary>
    public class IsProposalLengthTypeValid : ValidationAttribute
    {
        /// IMS_PR_ROAD_CODE 
        private readonly string PropertyImsPrRoadCode;
        /// PLAN_CN_ROAD_CODE
        private readonly string PropertyPlanCnRoadCode;
        /// IMS_PAV_LEN   
        private readonly string PropertyPavmentLength;

        private readonly string PropertyImsStagePhase;

        public IsProposalLengthTypeValid(string PropertyCNRoadCode, string PropertPavLength, string PropertyProposalCode, string ParamStagePhase)
        {
            this.PropertyPlanCnRoadCode = PropertyCNRoadCode;
            this.PropertyPavmentLength = PropertPavLength;
            this.PropertyImsPrRoadCode = PropertyProposalCode;
            this.PropertyImsStagePhase = ParamStagePhase;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var Prop_PLAN_CN_ROAD_CODE = validationContext.ObjectType.GetProperty(PropertyPlanCnRoadCode);
            var Prop_IMS_PAV_LEN = validationContext.ObjectType.GetProperty(PropertyPavmentLength);
            var Prop_IMS_PR_ROAD_CODE = validationContext.ObjectType.GetProperty(PropertyImsPrRoadCode);
            var Prop_IMS_STAGE_PHASE = validationContext.ObjectType.GetProperty(PropertyImsStagePhase);

            var PLAN_CN_ROAD_CODE = Prop_PLAN_CN_ROAD_CODE.GetValue(validationContext.ObjectInstance, null);
            var IMS_PAV_LEN = Prop_IMS_PAV_LEN.GetValue(validationContext.ObjectInstance, null);
            var IMS_PR_ROAD_CODE = Prop_IMS_PR_ROAD_CODE.GetValue(validationContext.ObjectInstance, null);
            var IMS_STAGE_PHASE = Prop_IMS_STAGE_PHASE.GetValue(validationContext.ObjectInstance, null);


            if (value != null)
            {
                string status = new PMGSY.BAL.Proposal.ProposalBAL().isProposalLengthTypeValid(Convert.ToInt32(PLAN_CN_ROAD_CODE), Convert.ToDecimal(IMS_PAV_LEN), Convert.ToInt32(IMS_PR_ROAD_CODE), value.ToString(), Convert.ToString(IMS_STAGE_PHASE));
                if (status == string.Empty)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult(status);
                }
            }
            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }

    }


    /// <summary>
    /// Check for Higher Specification Cost
    /// It Higher Specification Cost is Yes then it should be greater than 0
    /// </summary>
    public class IsValidHigherSpecCost : ValidationAttribute, IClientValidatable
    {
        private readonly string PropertyName;

        public IsValidHigherSpecCost(string propertyName)
        {
            this.PropertyName = propertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            var propertyTestedInfo = validationContext.ObjectType.GetProperty(this.PropertyName);
            var isHigherSpecCost = propertyTestedInfo.GetValue(validationContext.ObjectInstance, null);
            if (PMGSYSession.Current.PMGSYScheme == 2)
            {
                if (isHigherSpecCost != null && isHigherSpecCost.Equals("Y"))
                {
                    if (value != null && Convert.ToDecimal(value) > 0)
                        return ValidationResult.Success;
                    else
                        return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
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

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "ishigherspeccostvalid"
            };
            rule.ValidationParameters["ishigherspec"] = this.PropertyName;
            yield return rule;
        }

    }

    /// <summary>
    /// In case of PMGSY Scheme 1, Pavment Length should be greater than 0.50 Mtrs.
    /// In case of PMGSY Scheme 2, Pavment Length shouls be greater than 0 mtrs.
    /// </summary>
    public class IsValidRangeOfPAVLength : ValidationAttribute, IClientValidatable
    {
        public IsValidRangeOfPAVLength()
        {

        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (PMGSYSession.Current.PMGSYScheme == 2)
            {
                if (value != null && Convert.ToDecimal(value) > 0)
                    return ValidationResult.Success;
                else
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
            else
            {
                if (value != null && Convert.ToDecimal(value) > 0.50M)
                    return ValidationResult.Success;
                else
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }

        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "isvalidrangeofpavlen"
            };
            rule.ValidationParameters["scheme"] = PMGSYSession.Current.PMGSYScheme;
            yield return rule;
        }

    }


}




