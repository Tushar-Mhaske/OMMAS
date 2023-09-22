#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   UnlockProposalViewModel.cs
        * Description   :   This View Model is Used in Road Proposal Views Create.cshtml,Details.cshtml
        * Author        :   Shivkumar Deshmukh        
        * Creation Date :   12/July/2013
 **/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PMGSY.Models;
using System.Web.Mvc;

namespace PMGSY.Models.Proposal
{
    public class UnlockProposalViewModel
    {
        public UnlockProposalViewModel()        
        {            
            BATCHS = new List<SelectListItem>();
            COLLABORATIONS = new List<SelectListItem>();
            STREAMS= new List<SelectListItem>();
            MP_CONSTITUENCY = new List<SelectListItem>();
            MLA_CONSTITUENCY = new List<SelectListItem>();
            PROPOSED_SURFACE = new List<SelectListItem>();
            TRAFFIC_TYPE = new List<SelectListItem>();
            CN_ROADS = new List<SelectListItem>();
        }
        public string stateType { get; set; }
        public string DistrictType { get; set; }

        public byte PMGSYScheme { get; set; }

        public string StateName { get; set; }
        public string DistrictName { get; set; }

        [Display(Name="Year")]      
        public int IMS_YEAR { get; set; }

        public List<SelectListItem> Years { set; get; }
        
        [Display(Name="Batch")]
        //[Required(ErrorMessage="Please Select Batch")]
        //[Range(1,5, ErrorMessage = "Please Select Batch.")]
        public int IMS_BATCH { get; set; }

        public List<SelectListItem> BATCHS { set; get; }

        [Display(Name = "Name of Block")]
        public int MAST_BLOCK_CODE { get; set; }

        public List<SelectListItem> BLOCKS { get; set; }
       
        public int MAST_DPIU_CODE { get; set; }

        public string MAST_BLOCK_NAME { get; set; }

        public bool isPaymentDone { get; set; }

        [Display(Name = "Stream Proposed")]
        public string MAST_STREAM_NAME { get; set; }
        public string PLAN_RD_NAME { get; set; }
        
        public string IMS_EXISTING_PACKAGE { get; set; }

        public List<SelectListItem> EXISTING_PACKAGES { get; set; }

        [Display(Name = "Package Number")]            
        [RegularExpression(@"^[a-zA-Z0-9 -/]+$", ErrorMessage = "Invalid Package ID,Can only contains AlphaNumeric values")]
        public string IMS_PACKAGE_ID { get; set; }
        
        public string PACKAGE_PREFIX { get; set; }

        [Display(Name="Staged Package Number")]
        
        public string Stage_2_Package_ID { get; set; }

        public List<SelectListItem> PACKAGES { get; set; }

        [Display(Name="Staged Year")]
        public int? Stage_2_Year { get; set; }

        [Display(Name= "Existing Package Number")]
        public string EXISTING_IMS_PACKAGE_ID { get; set; }
        
        [Display(Name = "Funding Agency")]
        [Required(ErrorMessage="Please Select Funding Agency")]
        [Range(1,20,ErrorMessage="Please Select Funding Agency")]
        public Nullable<int> IMS_COLLABORATION { get; set; }

        public string MAST_FUNDING_AGENCY_NAME { get; set; }
        
        public List<SelectListItem> COLLABORATIONS { get; set; }

        [Display(Name="Stream")]
        [Required(ErrorMessage="Please Select Stream") ]
        [Range(1, int.MaxValue, ErrorMessage = "Please Select Stream")]
        public Nullable<int> IMS_STREAMS { get; set; }

        public List<SelectListItem> STREAMS { get; set; }

        [Display(Name = "Link/Through")]
        public int? PLAN_CN_ROAD_CODE { get; set; }

        [Display(Name="Proposal Length")]
        [IsProposalLengthTypeValid("PLAN_CN_ROAD_CODE", "IMS_PAV_LENGTH", "IMS_PR_ROAD_CODE", "IMS_STAGE_PHASE", ErrorMessage = "Proposal Length Type is Invalid.")]
        public string IMS_PARTIAL_LEN { get; set; }

        public List<SelectListItem> CN_ROADS { get; set; }
        public List<SelectListItem> STAGE1_PROPOSAL_ROADS { get; set; }


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


        [Display(Name = "Road From")]
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9 ().,-]+$", ErrorMessage = "Invalid Road From,Can only contains AlphaNumeric values")]
        public string IMS_ROAD_FROM { get; set; }

        [Display(Name = "Road To")]
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9 ().,-]+$", ErrorMessage = "Invalid Road To,Can only contains AlphaNumeric values")]
        public string IMS_ROAD_TO { get; set; }

        [Display(Name = "Road Name")]        
        public string IMS_ROAD_NAME { get; set; }
        
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid State cost for projects under PMGSY ,Can only contains Numeric values and 4 digits after decimal place")]
        [Display(Name = "State Cost For Projects Under PMGSY(Rs Lakhs)")]
        public Nullable<decimal> IMS_SANCTIONED_RS_AMT { get; set; }
        
        [Display(Name = "Pavement Length")]       
        [RegularExpression(@"^(?=.+)(?:[0-9]\d*|0)?(?:\.\d+)?$", ErrorMessage = "Invalid Pavement Length,Can only contains Numeric values")]        
        [Range(0.100,double.MaxValue,ErrorMessage="Invalid Pavement Length,Must be greater than 0.1 km.")]
        public Nullable<decimal> IMS_PAV_LENGTH { get; set; }
        public Nullable<decimal> IMS_STAGE1_PAV_LENGTH { get; set; }
        public Nullable<decimal> DUP_IMS_PAV_LENGTH { get; set; }
        [Display(Name = "Number of CD Works")]
        [RegularExpression(@"\d+$", ErrorMessage = "Invalid Number of CD Works")]
        public int? IMS_NO_OF_CDWORKS { get; set; }

        [Display(Name = "Whether ZP Resolution Obtained (Y/N)")]
        public string IMS_ZP_RESO_OBTAINED { get; set; }

        [Display(Name = "If Proposed By MP")]
        public Nullable<int> MAST_MP_CONST_CODE { get; set; }
        
        public string MAST_MP_CONST_NAME { get; set; } 

        public List<SelectListItem> MP_CONSTITUENCY { get; set; }

        [Display(Name = "If Proposed By MLA")]
        public Nullable<int> MAST_MLA_CONST_CODE { get; set; }
        public string MAST_MLA_CONST_NAME { get; set; } 

        public List<SelectListItem> MLA_CONSTITUENCY { get; set; }

        [Display(Name = "Carriage Way Width")]
        [Range(1, int.MaxValue, ErrorMessage = "Select Carriage Way Width")]
        public Nullable<int> IMS_CARRIAGED_WIDTH { get; set; }

        public List<SelectListItem> CARRIAGED_WIDTH { get; set; }

        /// <summary>
        /// For only display purpose
        /// </summary>
        public string Display_Carriaged_Width { get; set; }


        [Display(Name = "Traffic Category")]        
        public Nullable<int> IMS_TRAFFIC_TYPE { get; set; }
        
        public string IMS_TRAFFIC_CATAGORY_NAME { get; set; }

        public List<SelectListItem> TRAFFIC_TYPE { get; set; }

        [Display(Name = "Proposed Surface")]
        [Required(ErrorMessage="Select Proposed Surface")]        
        public string IMS_PROPOSED_SURFACE { get; set; }

        public string IMS_PROPOSED_SURFACE_NAME { get; set; }

        public List<SelectListItem> PROPOSED_SURFACE { get; set; }

        [Display(Name = "Remarks")]
        [RegularExpression(@"^[a-zA-Z0-9 ,.()-]+$", ErrorMessage = "Invalid Remarks,Can only contains AlphaNumeric values and [,.()-]")]
        public string IMS_REMARKS { get; set; }

        [Display(Name = "Existing Surface Type")]       
        public Nullable<int> MAST_EXISTING_SURFACE_CODE { get; set; }

        public string MAST_EXISTING_SURFACE_NAME { get; set; }

        public List<SelectListItem> EXISTING_SURFACE { get; set; }

        [Display(Name = "Habitations Benifited")]
        public string IMS_ISBENEFITTED_HABS { get; set; }

        [Display(Name = "Reason")]
        public Nullable<int> IMS_REASON { get; set; }
        public string Reason { get; set; }

        [Display(Name = "CC Length")]
        [Range(0, 9999, ErrorMessage = "CC Length too Large.")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "Invalid CC Length,Can only contains Numeric values and Total 7 Digits and 3 digits after decimal place")]
        public Nullable<decimal> IMS_CC_LENGTH { get; set; }
            
        [Display(Name = "BT Length")]
        [Range(0, 9999, ErrorMessage = "BT Length too Large.")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "Invalid BT Length,Can only contains Numeric values and Total 7 Digits and 3 digits after decimal place")]
        public Nullable<decimal> IMS_BT_LENGTH { get; set; }

        
        public string IMS_STAGE_PHASE { get; set; }
        public string hdn_IMS_STAGE_PHASE { get; set; }
         
        public string IMS_IS_STAGED { get; set; }

        public string hdnISSTAGED { get; set; }
        
        public int IMS_PR_ROAD_CODE { get; set; }
        public int MAST_STATE_CODE { get; set; }
        public int MAST_DISTRICT_CODE { get; set; }
        public int MAST_AGENCY_CODE { get; set; }
        public string IMS_UPGRADE_CONNECT { get; set; }


        /// <summary>
        /// Total Maintenance Cost
        /// Only for the Display Purpose
        /// </summary>
        [Display(Name = "Total Maintenance Cost(Rs Lakhs)")]
        //[IsTotalMaintananceCostValid("IMS_STAGE_PHASE", ErrorMessage = "Total Maintenance Cost Should not be Zero, Please Enter Atleast One Maintenance Cost.")]
        public decimal TotalMaintenanceCost { get; set; }
  
        /// <summary>
        /// All the Sanctioned Amount 
        /// ONLY these amount will be edited 
        /// </summary>
        /// 

        [Display(Name = "Pavement Cost(Rs Lakhs)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid Pavement Cost,Can only contains Numeric values and Total 8 Digits and 4 digits after decimal place")]
        [Range(1, 99999, ErrorMessage = "Invalid Pavement Cost, Pavement Cost should be greator than 0.")]        
        public decimal IMS_SANCTIONED_PAV_AMT { get; set; }

        [Display(Name = "CD Works Cost(Rs Lakhs)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid CD Works Cost,Can only contains Numeric values and 4 digits after decimal place")]
        [Range(0, 99999, ErrorMessage = "CD Works Sanctioned Cost too Large.")]
        public decimal IMS_SANCTIONED_CD_AMT { get; set; }

        [Display(Name = "Protection Works Cost(Rs Lakhs)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid Protection Works Cost,Can only contains Numeric values and 4 digits after decimal place")]
        [Range(0, 99999, ErrorMessage = "Protection Works Cost too Large.")]
        public decimal IMS_SANCTIONED_PW_AMT { get; set; }

        [Display(Name = "Other Works Cost (if Any)(Rs Lakhs)")]
        [Range(0, 99999, ErrorMessage = "Other Works Cost too Large.")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Other Works Cost,Can only contains Numeric values and 4 digits after decimal place")]  
        public decimal IMS_SANCTIONED_OW_AMT { get; set; }
        
        // Incase of Bridge Work
        public decimal IMS_SANCTIONED_BW_AMT { get; set; }

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

        [Display(Name = "Renewal Cost(Rs Lakhs)")]
        [Range(0, 99999, ErrorMessage = "Renewal Cost too Large.")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Renewal Cost ,Can only contains Numeric values and 4 digits after decimal place")]
        public Nullable<decimal> IMS_RENEWAL_COST { get; set; }

        [Display(Name = "Total Shines (Furniture) Cost(Rs Lakhs)")]
        [Range(0, 99999, ErrorMessage = "Invalid Total Shines (Furniture) Cost (Range should be 0-99999)")]
        public Nullable<decimal> IMS_FURNITURE_COST { get; set; }

        [Display(Name = "Total Cost(Rs Lakhs / Excluding Higher Specification Cost)")]
        public decimal TotalCost { get; set; }

        [Display(Name = "Whether any higher specification beyond PMGSY2 Guidelines have been proposed(including utility shifting)?")]
        public string IMS_IS_HIGHER_SPECIFICATION { get; set; }

        [Display(Name = "Higher Specification Cost (Rs Lakhs)")]
        [Range(0, 99999, ErrorMessage = "Invalid Higher Specification Cost (Range should be 0-99999)")]
        [IsValidHigherSpecCost("IMS_IS_HIGHER_SPECIFICATION", ErrorMessage = "Higher Specification Cost should be greater than 0")]
        public Nullable<decimal> IMS_HIGHER_SPECIFICATION_COST { get; set; }

        [Display(Name = "Whether the road qualified for sharing of MoRD (90%) & State (10%) as per PMGSY2 Guidelines? (Hilly State, DDP Area, Schedule-V Habitations, BRGF & IAP Districts) ")]
        public Nullable<byte> IMS_SHARE_PERCENT { get; set; }

        //[Range(0, double.MaxValue, ErrorMessage = "Total Maintenance Cost Should not be Zero, Please Enter Atleast One Maintenance Cost.")]
        
        public decimal txtTotalMaintenance { get; set; }

        public Nullable<int> IMS_STAGED_YEAR { get; set; }
        public string IMS_STAGED_PACKAGE_ID { get; set; }
        public Nullable<int> IMS_STAGED_ROAD_ID { get; set; }
        public Nullable<int> IMS_NO_OF_BRIDGEWRKS { get; set; }
        public string IMS_BRIDGE_NAME { get; set; }
        public Nullable<decimal> IMS_BRIDGE_LENGTH { get; set; }
        public Nullable<decimal> IMS_BRIDGE_WORKS_EST_COST { get; set; }
        public Nullable<decimal> IMS_BRIDGE_EST_COST_STATE { get; set; }
        [Display(Name="Reason")]
        
        public Nullable<int> IMS_HABS_REASON { get; set; }
        public List<SelectListItem> HABS_REASON { get; set; }
        public string HABS_REASON_TEXT { get; set; }
        public string IMS_DPR_STATUS { get; set; }
        #region STA Sanction Region
        public string STA_SANCTIONED { get; set; }

        [Display(Name="STA Name")]
        public string STA_SANCTIONED_BY { get; set; }
        
        [Display(Name="Scrutiny Date")]
        //[Required( ErrorMessage="Please Enter Scrutiny Date")   ]
        public string STA_SANCTIONED_DATE { get; set; }
        
        [Display(Name="Remarks")]
        //[Required(ErrorMessage="Please Enter Remarks")]
        //[RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "Invalid Remarks,Can only contains AlphaNumeric values")]
        public string MS_STA_REMARKS { get; set; }
        #endregion

        /// STA SANCTION REGION END 
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
}