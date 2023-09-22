#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   LSBOthDetailsModel.cs
        * Description   :   This View Model is Used in LSB Other Details Views in CreateLSB.cshtml        
        * Author        :   Shyam Yadav
        * Modified By   :   Shivkumar Deshmukh
        * Creation Date :   20-05-2013
 **/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
//using DataAnnotationsExtensions;
using PMGSY.Models;
using System.Web.Mvc;
using PMGSY.Extensions;

namespace PMGSY.Models.Proposal
{
    public class LSBViewModel
    {
        public LSBViewModel()
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
            PACKAGES = new List<SelectListItem>();
            PMGSYScheme = PMGSYSession.Current.PMGSYScheme;
        }
        public int StateCodeForComparision{get;set;}

        public byte PMGSYScheme { get; set; }

        public string operation { get; set; }
        public bool isAllDetailsEntered { get; set; }

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

        #region PTA Sanction Region

        public string PTA_SANCTIONED { get; set; }
        public int? PTA_SANCTIONED_BY { get; set; }

        [Display(Name = "STA Name")]
        public string NAME_OF_PTA { get; set; }

        [Display(Name = "Scrutiny Date")]
        public string PTA_SANCTIONED_DATE { get; set; }

        [Display(Name = "Remarks")]
        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "Invalid Remarks,Can only contains AlphaNumeric values")]
        public string MS_PTA_REMARKS { get; set; }

        #endregion


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


        public decimal? PMGSYIII_TotalCost_ExcludingHigherSpecificationRsinLakhs { get; set; }
        public decimal? PMGSYIII_TotalStateShareCost { get; set; }

        public int MAST_DPIU_CODE { get; set; }

        public string MAST_BLOCK_NAME { get; set; }

        [Display(Name = "Stream Proposed")]
        public string MAST_STREAM_NAME { get; set; }
        public string PLAN_RD_NAME { get; set; }

        //[Required(ErrorMessage="Please select New Connectivity/Upgradation")]
        //[RegularExpression(@"^[NU]+$", ErrorMessage = "Please select either New Connectivity or Upgradation")]
        //public string UPGRADE_CONNECT { get; set; }

        // Radio Button for displaying New Package or Existing Package
        //public string isNewPackage { get; set; }
        public string IMS_EXISTING_PACKAGE { get; set; }

        public string isExistingRoad { get; set; }

        [Display(Name = "Package Number")]
        [RegularExpression(@"^[a-zA-Z0-9 -/]+$", ErrorMessage = "Invalid Package ID,Can only contains AlphaNumeric values")]
        [IsNewPackage("IMS_EXISTING_PACKAGE", ErrorMessage = "Please Enter Package Number")]
        [IsPackageExists("IMS_YEAR", "IMS_PR_ROAD_CODE", "IMS_EXISTING_PACKAGE", "IMS_PACKAGE_ID", ErrorMessage = "Package Number Already Taken,Please Choose Different Package Number")]
        public string IMS_PACKAGE_ID { get; set; }

        public string PACKAGE_PREFIX { get; set; }

        public List<SelectListItem> PACKAGES { get; set; }

        [Display(Name = "Existing Package Number")]
        [IsExistingPackage("IMS_EXISTING_PACKAGE", ErrorMessage = "Please Select Package Number")]
        [IsPackageExists("IMS_YEAR", "IMS_PR_ROAD_CODE", "IMS_EXISTING_PACKAGE", "EXISTING_IMS_PACKAGE_ID", ErrorMessage = "Package Number Already Taken,Please Choose Different Package Number")]
        public string EXISTING_IMS_PACKAGE_ID { get; set; }

        [Display(Name = "Funding Agency")]
        [Required(ErrorMessage = "Please Select Funding Agency")]
        [Range(1, 20, ErrorMessage = "Please Select Funding Agency")]
        public Nullable<int> IMS_COLLABORATION { get; set; }

        public string MAST_FUNDING_AGENCY_NAME { get; set; }

        public List<SelectListItem> COLLABORATIONS { get; set; }

        public bool isPaymentDone { get; set; }

        [Display(Name = "Stream Proposed")]
        [Required(ErrorMessage = "Please Select Stream Proposed")]
        [Range(1, int.MaxValue, ErrorMessage = "Please Select Stream Proposed")]
        public Nullable<int> IMS_STREAMS { get; set; }

        public List<SelectListItem> STREAMS { get; set; }

        public int MAST_STREAM_CODE { get; set; }

        [Display(Name = "Link/Through")]
        [IsNonExistingRoad("isExistingRoad", ErrorMessage = "Please Select Road")]
        public int? PLAN_CN_ROAD_CODE { get; set; }

        public List<SelectListItem> CN_ROADS { get; set; }

        //---LSB Start
        public List<SelectListItem> Existing_Roads_LSB { get; set; }

        [Display(Name = "Proposal Year")]
        [IsExistingRoad("isExistingRoad", ErrorMessage = "Year field is required")]
        //[Range(2000, 2099, ErrorMessage = "Please Select Year.")]
        public Nullable<int> IMS_STAGED_YEAR { get; set; }

        [Display(Name = "Proposal Package")]
        [IsExistingRoad("isExistingRoad", ErrorMessage = "Package field is required")]
        [RegularExpression(@"^[a-zA-Z0-9 -/]+$", ErrorMessage = "Invalid Package ID, Can only contains AlphaNumeric values")]
        public string IMS_STAGED_PACKAGE_ID { get; set; }

        [Display(Name = "Proposal Road")]
        [IsExistingRoad("isExistingRoad", ErrorMessage = "Road field is required")]
        public Nullable<int> IMS_STAGED_ROAD_ID { get; set; }


        [Display(Name = "Road From")]
        [IsNonExistingRoad("isExistingRoad", ErrorMessage = "Road From field is required")]
        [RegularExpression(@"^[a-zA-Z0-9 -_/]+$", ErrorMessage = "Invalid Road From, Can only contains AlphaNumeric values")]
        public string IMS_ROAD_FROM { get; set; }

        [Display(Name = "Road To")]
        [IsNonExistingRoad("isExistingRoad", ErrorMessage = "Road To field is required")]
        [RegularExpression(@"^[a-zA-Z0-9 -_/]+$", ErrorMessage = "Invalid Road To, Can only contains AlphaNumeric values")]
        public string IMS_ROAD_TO { get; set; }

        [Display(Name = "Road Name")]
        public string IMS_ROAD_NAME { get; set; }

        [Display(Name = "Remarks")]
        [RegularExpression(@"^[a-zA-Z0-9 -/]+$", ErrorMessage = "Invalid Remarks, Can only contains AlphaNumeric values")]
        public string IMS_REMARKS { get; set; }


        [Display(Name = "No. of Bridge Works")]
        public Nullable<int> IMS_NO_OF_BRIDGEWRKS { get; set; }

        [Display(Name = "Bridge Name")]
        [RegularExpression(@"^[a-zA-Z0-9 -/]+$", ErrorMessage = "Invalid Bridge Name, Can only contains AlphaNumeric values")]
        [Required]
        public string IMS_BRIDGE_NAME { get; set; }

        [Display(Name = "Bridge Length")]
        [Range(15, 9999.999, ErrorMessage = "Invalid Bridge Length,it should be greater than 15mtrs and less than 1000 mtrs.")]
        [Required]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "Invalid Bridge Length, Can only contains Numeric values and 3 digits after decimal place")]
        public Nullable<decimal> IMS_BRIDGE_LENGTH { get; set; }

        [Display(Name = "MoRD Cost(Rs Lakhs)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid MoRD Cost, Can only contains Numeric values and 4 digits after decimal place")]
        [Range(0.0001, 99999.9999, ErrorMessage = "MoRD Cost too small or too large.")]
        [Required]
        public decimal IMS_BRIDGE_WORKS_EST_COST { get; set; }

        [Display(Name = "State Share(Rs Lakhs)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid State Share, Can only contains Numeric values and 4 digits after decimal place")]
        // Earlier validation  as mIN Range 0.0001 for State Share. Removed on 10/06/2014 as per Srinivas sir's request.
        [Range(0.0000, 999999.9999, ErrorMessage = "State Share Cost too small or too large.")]
        [Required]
        public decimal IMS_BRIDGE_EST_COST_STATE { get; set; }

        [Display(Name = "State Share(Rs Lakhs)")]
        public decimal IMS_SANCTIONED_BS_AMT { get; set; }
        [Display(Name = "MoRD Cost(Rs Lakhs)")]
        public decimal IMS_SANCTIONED_BW_AMT { get; set; }

        [Display(Name = "Total Bridge Cost(Rs Lakhs)")]
        [Range(0.0001, 999999.9999, ErrorMessage = "Total Cost too small or too large.")]
        public decimal TotalEstimatedCost { get; set; }

        [Display(Name = "Core Network No.")]
        public string CoreNetworkNumber { get; set; }

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

        #region PMGSY Scheme2
        [Display(Name = "Whether any higher specification beyond PMGSY2 Guidelines have been proposed(including utility shifting)?")]
        public string IMS_IS_HIGHER_SPECIFICATION { get; set; }

        [Display(Name = "Higher Specification Cost (Rs Lakhs)")]
        [Range(0, 99999, ErrorMessage = "Higher Specification Cost too small or too large.")]
        [IsValidHigherSpecCostLSB("IMS_IS_HIGHER_SPECIFICATION", ErrorMessage = "Higher Specification Cost should be greater than 0")]
        public Nullable<decimal> IMS_HIGHER_SPECIFICATION_COST { get; set; }

        public Nullable<decimal> IMS_SANCTIONED_HS_AMT { get; set; }

        [Display(Name = "Whether the road qualified for sharing of MoRD (90%) & State (10%) as per PMGSY2 Guidelines? (Hilly State, DDP Area, Schedule-V Habitations, BRGF & IAP Districts) ")]
        public Nullable<byte> IMS_SHARE_PERCENT { get; set; }

        [Display(Name = "Total Cost (Rs Lakhs) Including Higher Specification Cost")]
        public decimal TotalCostWithHigherSpecCost { get; set; }

        #endregion
        //----LSB End


        //-------------------------------------------------------------
        public Nullable<int> MAST_EXISTING_SURFACE_CODE { get; set; }
        public List<SelectListItem> EXISTING_SURFACE { get; set; }
        public string IMS_ISBENEFITTED_HABS { get; set; }
        public Nullable<int> IMS_REASON { get; set; }
        public string Reason { get; set; }

        public Nullable<decimal> IMS_CC_LENGTH { get; set; }
        public Nullable<decimal> IMS_BT_LENGTH { get; set; }
        public string IMS_STAGE_PHASE { get; set; }
        public string IMS_IS_STAGED { get; set; }
        public int IMS_PR_ROAD_CODE { get; set; }
        public int MAST_STATE_CODE { get; set; }
        public int MAST_DISTRICT_CODE { get; set; }
        public int MAST_AGENCY_CODE { get; set; }
        public string IMS_UPGRADE_CONNECT { get; set; }


        public string Stage_2_Package_ID { get; set; }
        public int? Stage_2_Year { get; set; }
        public Nullable<decimal> IMS_STATE_SHARE { get; set; }
        public Nullable<decimal> IMS_PAV_LENGTH { get; set; }
        public int? IMS_NO_OF_CDWORKS { get; set; }
        public string IMS_ZP_RESO_OBTAINED { get; set; }
        public Nullable<int> MAST_MP_CONST_CODE { get; set; }
        public string MAST_MP_CONST_NAME { get; set; }
        public List<SelectListItem> MP_CONSTITUENCY { get; set; }
        public string MAST_MLA_CONST_NAME { get; set; }
        public List<SelectListItem> MLA_CONSTITUENCY { get; set; }
        public Nullable<decimal> IMS_CARRIAGED_WIDTH { get; set; }
        public List<SelectListItem> CARRIAGED_WIDTH { get; set; }
        public Nullable<int> IMS_TRAFFIC_TYPE { get; set; }
        public string IMS_TRAFFIC_CATAGORY_NAME { get; set; }
        public List<SelectListItem> TRAFFIC_TYPE { get; set; }
        public string IMS_PROPOSED_SURFACE { get; set; }
        public string IMS_PROPOSED_SURFACE_NAME { get; set; }
        public List<SelectListItem> PROPOSED_SURFACE { get; set; }


        //-------------------------------------------------------------


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
        //[IsStageOneProposal("IMS_STAGE_PHASE", "TotalMaintenanceCost", ErrorMessage = "Total Maintenance Cost Should not be Zero, Please Enter Atleast One Maintenance Cost.")]
        //[IsTotalMaintananceCostValid("IMS_STAGE_PHASE", ErrorMessage = "Total Maintenance Cost Should not be Zero, Please Enter Atleast One Maintenance Cost.")]
        public decimal TotalMaintenanceCost { get; set; }

        [Display(Name = "Renewal Cost (Rs Lakhs)")]
        [Range(0, 99999, ErrorMessage = "Invalid Renewal Cost (Range should be 0-99999)")]
        public Nullable<decimal> IMS_RENEWAL_COST { get; set; }

        public decimal IMS_SANCTIONED_PAV_AMT { get; set; }
        public decimal IMS_SANCTIONED_CD_AMT { get; set; }
        public decimal IMS_SANCTIONED_PW_AMT { get; set; }
        public decimal IMS_SANCTIONED_OW_AMT { get; set; }


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

        public string IMS_DPR_STATUS { get; set; }
        public Nullable<int> IMS_HABS_REASON { get; set; }
        public List<SelectListItem> HABS_REASON { get; set; }


        //public string STA_SANCTIONED { get; set; }
        //public string STA_SANCTIONED_BY { get; set; }
        //public Nullable<System.DateTime> STA_SANCTIONED_DATE { get; set; }
        //public string MS_STA_REMARKS { get; set; }


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
    }



    public class IsExistingRoad : ValidationAttribute, IClientValidatable
    {
        private readonly string PropertyName;

        public IsExistingRoad(string propertyName)
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

            var imsexisting = propertyTestedInfo.GetValue(validationContext.ObjectInstance, null);

            if (imsexisting.ToString().ToLower() == "u")
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
            if (imsexisting.ToString().ToLower() == "n")
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
                ValidationType = "isexisting"
            };
            rule.ValidationParameters["imsexisting"] = this.PropertyName;
            yield return rule;
        }

    }



    public class IsNonExistingRoad : ValidationAttribute, IClientValidatable
    {
        private readonly string PropertyName;

        public IsNonExistingRoad(string propertyName)
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

            var imsexisting = propertyTestedInfo.GetValue(validationContext.ObjectInstance, null);

            if (imsexisting.ToString().ToLower() == "n")
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
            if (imsexisting.ToString().ToLower() == "u")
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
                ValidationType = "isnonexisting"
            };
            rule.ValidationParameters["imsnonexisting"] = this.PropertyName;
            yield return rule;
        }
    }


    /// <summary>
    /// Check for Higher Specification Cost
    /// It Higher Specification Cost is Yes then it should be greater than 0
    /// </summary>
    public class IsValidHigherSpecCostLSB : ValidationAttribute, IClientValidatable
    {
        private readonly string PropertyName;

        public IsValidHigherSpecCostLSB(string propertyName)
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


}