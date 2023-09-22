using PMGSY.Extensions;
using PMGSY.Models.Proposal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.BuildingProposal
{
    public class BuildingProposalViewModel
    {
        public BuildingProposalViewModel()        
        {
            
            Years = new List<SelectListItem>();
            BATCHS = new List<SelectListItem>();
            COLLABORATIONS = new List<SelectListItem>();
            STREAMS= new List<SelectListItem>();
            BLOCKS = new List<SelectListItem>();
            PACKAGES = new List<SelectListItem>();
            EXISTING_PACKAGES = new List<SelectListItem>();

            PMGSYScheme = PMGSYSession.Current.PMGSYScheme;
        }
        
        public byte PMGSYScheme { get; set; }

        public string operation {get;set;}
        
        public string stateType { get; set; }

        public string DistrictType { get; set; }

        [Display(Name = "Name of State")]
        public string StateName { get; set; }

        [Display(Name = "Name of District")]
        public string DistrictName { get; set; }

        public int RoleCode { get; set; }

        [Display(Name="Year")]
        [Required]
        [Range(2000, 2099, ErrorMessage = "Please Select Year.")]
        public int IMS_YEAR { get; set; }
        public List<SelectListItem> Years { set; get; }

        [Required(ErrorMessage = "Please select New Connectivity/Upgradation")]
        [RegularExpression(@"^[NU]+$", ErrorMessage = "Please select either New Connectivity or Upgradation")]
        public string UPGRADE_CONNECT { get; set; }

        public string IMS_YEAR_FINANCIAL { get; set; }

        [Display(Name = "Remarks")]
        [RegularExpression(@"^[a-zA-Z0-9 ,.()-]+$", ErrorMessage = "Invalid Remarks,Can only contains AlphaNumeric values and [,.()-]")]
        public string IMS_PROG_REMARKS { get; set; }

        [Display(Name="Batch")]
        [Required(ErrorMessage="Please Select Batch")]
        [Range(1,5, ErrorMessage = "Please Select Batch.")]
        public int IMS_BATCH { get; set; }
        public List<SelectListItem> BATCHS { set; get; }

        [Display(Name = "Name of Block")]
        public string MAST_BLOCK_NAME { get; set; }
        [Display(Name = "Name of Block")]
        [Required]
        [Range(1,double.MaxValue,ErrorMessage="Please Select Block")]
        public int MAST_BLOCK_CODE { get; set; }
        public List<SelectListItem> BLOCKS { get; set; }
        
        
        public int MAST_DPIU_CODE { get; set; }
        
        
        
        // Radio Button for displaying New Package or Existing Package
        // public string isNewPackage { get; set; }

        [Display(Name = "Reason")]
        public Nullable<int> IMS_REASON { get; set; }
        public string Reason { get; set; }

        [Display(Name = "Package Number")]            
        [RegularExpression(@"^[a-zA-Z0-9 -/]+$", ErrorMessage = "Invalid Package ID,Can only contains AlphaNumeric values")]
        [IsNewPackage("IMS_EXISTING_PACKAGE",ErrorMessage="Please Enter Package Number")]
        [IsPackageExists("IMS_YEAR", "IMS_PR_ROAD_CODE", "IMS_EXISTING_PACKAGE", "IMS_PACKAGE_ID", ErrorMessage = "Package Number Already Taken,Please Choose Different Package Number")]
        public string IMS_PACKAGE_ID { get; set; }
        public List<SelectListItem> PACKAGES { get; set; }
        
        public string IMS_EXISTING_PACKAGE { get; set; }
        public List<SelectListItem> EXISTING_PACKAGES { get; set; }

        [Display(Name = "Existing Package Number")]
        [IsExistingPackage("IMS_EXISTING_PACKAGE", ErrorMessage = "Please Select Package Number")]
        public string EXISTING_IMS_PACKAGE_ID { get; set; }
        
        public string PACKAGE_PREFIX { get; set; }





        public string MAST_FUNDING_AGENCY_NAME { get; set; }
        [Display(Name = "Funding Agency")]
        [Required(ErrorMessage="Please Select Funding Agency")]
        [Range(1,20,ErrorMessage="Please Select Funding Agency")]
        public Nullable<int> IMS_COLLABORATION { get; set; }
        public List<SelectListItem> COLLABORATIONS { get; set; }

        [Display(Name = "Stream")]
       // [Required(ErrorMessage = "Please Select Stream")]
        [Range(1, int.MaxValue, ErrorMessage = "Please Select Stream")]
        public Nullable<int> IMS_STREAMS { get; set; }
        public List<SelectListItem> STREAMS { get; set; }

        //public int MAST_STREAM_CODE { get; set; }

       
        [Display(Name = "Work Name")]
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9 ()]+$", ErrorMessage = "Invalid Work Name,Can only contains AlphaNumeric values")]
        public string IMS_ROAD_NAME { get; set; }

       
        
        
        [Display(Name = "Remarks")]
        [RegularExpression(@"^[a-zA-Z0-9 ,.()-]+$", ErrorMessage = "Invalid Remarks,Can only contains AlphaNumeric values and [,.()-]")]
        public string IMS_REMARKS { get; set; }

        
        //[Required(ErrorMessage="Please Select Proposal is Complete or Staged.")]
        //[IsNewProposal("IMS_UPGRADE_CONNECT", ErrorMessage = "Please Select Proposal is Complete or Staged.")]
        public string IMS_IS_STAGED { get; set; }

        [Display(Name = "Name of Work")]
        public int IMS_PR_ROAD_CODE { get; set; }

        [Display(Name = "Name of State")]
        public int MAST_STATE_CODE { get; set; }
        [Display(Name = "Name of District")]
        public int MAST_DISTRICT_CODE { get; set; }
        public int MAST_AGENCY_CODE { get; set; }
        public string IMS_UPGRADE_CONNECT { get; set; }

       

        


         
        /// <summary>
        /// All the Sanctioned Amount 
        /// ONLY these amount will be edited 
        /// </summary>
        /// 

        [Display(Name = "Total Cost(Rs Lakhs)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid Total Cost,Can only contains Numeric values and Total 8 Digits and 4 digits after decimal place")]
        [Range(1, 99999, ErrorMessage = "Invalid Total Cost, Total Cost should be greater than 0.")]
        public decimal IMS_PAV_EST_COST { get; set; }

       
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

        //[Range(0, double.MaxValue, ErrorMessage = "Total Maintenance Cost Should not be Zero, Please Enter Atleast One Maintenance Cost.")]


        [Display(Name = "Total Maintenance Cost(Rs Lakhs)")]
      //  [IsStageOneProposal("IMS_STAGE_PHASE", "TotalMaintenanceCost", ErrorMessage = "Total Maintenance Cost Should not be Zero, Please Enter Atleast One Maintenance Cost.")]
        //[IsTotalMaintananceCostValid("IMS_STAGE_PHASE", ErrorMessage = "Total Maintenance Cost Should not be Zero, Please Enter Atleast One Maintenance Cost.")]
        public decimal TotalMaintenanceCost { get; set; }
  
        
        
        


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

        


        public string IMS_SANCTIONED { get; set; }
        
        public string IMS_SANCTIONED_BY { get; set; }
        //public Nullable<System.DateTime> IMS_SANCTIONED_DATE { get; set; }
        public string IMS_SANCTIONED_DATE { get; set; }
        [Display(Name = "MoRD Cost (Rs Lakhs)")]
        public Nullable<decimal> IMS_SANCTIONED_AMOUNT { get; set; }
       
        public string IMS_ISCOMPLETED { get; set; }
        public string IMS_LOCK_STATUS { get; set; }
        public string IMS_FREEZE_STATUS { get; set; }
        public string IMS_PROPOSAL_TYPE { get; set; }

        public virtual ADMIN_DEPARTMENT ADMIN_DEPARTMENT { get; set; }
        public virtual MASTER_BLOCK MASTER_BLOCK { get; set; }
        public virtual MASTER_DISTRICT MASTER_DISTRICT { get; set; }
        public virtual MASTER_FUNDING_AGENCY MASTER_FUNDING_AGENCY { get; set; }
        public virtual MASTER_STATE MASTER_STATE { get; set; }
        public virtual MASTER_STREAMS MASTER_STREAMS { get; set; }
        

    }

    
}