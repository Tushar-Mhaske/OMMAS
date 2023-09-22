using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using PMGSY.Extensions;

namespace PMGSY.Models.MasterReports
{
    public class MasterReportsModel
    {
        public MasterReportsModel()
        {
            STATE_NAME = PMGSYSession.Current.StateCode == 0 ? string.Empty : PMGSYSession.Current.StateName;
            DISTRICT_NAME = PMGSYSession.Current.DistrictCode == 0 ? string.Empty : PMGSYSession.Current.DistrictName;
            BLOCK_NAME = string.Empty;

        }
        public string STATE_NAME { get; set; }
        public string DISTRICT_NAME { get; set; }
        public string BLOCK_NAME { get; set; }


        [Display(Name = "State")]
        public Nullable<int> MAST_STATE_CODE { get; set; }
        public List<SelectListItem> STATES { get; set; }

        [Display(Name = "District")]
        public Nullable<int> MAST_DISTRICT_CODE { get; set; }
        public List<SelectListItem> DISTRICTS { get; set; }

        [Display(Name = "Block")]
        public int MAST_BLOCK_CODE { get; set; }
        public List<SelectListItem> BLOCKS { get; set; }
    }
    public class USP_MAS_DATA_STATE_REPORT_Result
    {
        public int MAST_STATE_CODE { get; set; }
        public string MAST_STATE_NAME { get; set; }
        public string MAST_STATE_UT { get; set; }
        public string MAST_STATE_TYPE { get; set; }
        public string MAST_STATE_SHORT_CODE { get; set; }
        public string MAST_STATE_ACTIVE { get; set; } 
    }
    public class USP_MAS_DATA_DISTRICT_REPORT_Result
    {
        public int MAST_DISTRICT_CODE { get; set; }
        public string MAST_STATE_NAME { get; set; }
        public string MAST_DISTRICT_NAME { get; set; }
        public string MAST_PMGSY_INCLUDED { get; set; }
        public string MAST_IAP_DISTRICT { get; set; }
        public string MAST_DISTRICT_ACTIVE { get; set; } 
 
    }
  
    public class USP_MAS_DATA_BLOCK_REPORT_Result
    {
        public int MAST_BLOCK_CODE { get; set; }
        public string MAST_STATE_NAME { get; set; }
        public string MAST_DISTRICT_NAME { get; set; }
        public string MAST_BLOCK_NAME { get; set; }
        public string MAST_IS_DESERT { get; set; }
        public string MAST_IS_TRIBAL { get; set; }
        public string MAST_PMGSY_INCLUDED { get; set; }
        public string MAST_SCHEDULE5 { get; set; }
        public string MAST_BLOCK_ACTIVE { get; set; } 
 
    }

    public class USP_MAS_DATA_VILLAGE_REPORT_Result
    {
        public int MAST_VILLAGE_CODE { get; set; }
        public string MAST_STATE_NAME { get; set; }
        public string MAST_DISTRICT_NAME { get; set; }
        public string MAST_BLOCK_NAME { get; set; }
        public string MAST_VILLAGE_NAME { get; set; }
        public string MAST_SCHEDULE5 { get; set; }
        public int VSCST_POP { get; set; }
        public int VTOT_POP { get; set; }
        public string MAST_VILLAGE_ACTIVE { get; set; } 
 
    }

    public class USP_MAS_DATA_HABITATION_REPORT_Result
    { 
        public int MAST_HAB_CODE { get; set; }
        public string MAST_STATE_NAME { get; set; }
        public string MAST_DISTRICT_NAME { get; set; }
        public string MAST_BLOCK_NAME { get; set; }
        public string MAST_VILLAGE_NAME { get; set; }
        public string MAST_HAB_NAME	 { get; set; }
        public string MAST_MLA_CONST_NAME	 { get; set; }
        public int MAST_HAB_TOT_POP { get; set; }
        public int MAST_HAB_SCST_POP { get; set; }
        public string MAST_MP_CONST_NAME { get; set; }
        public string MAST_HAB_STATUS	 { get; set; }
        public string MAST_HAB_CONNECTED	 { get; set; }
        public string MAST_SCHEME	 { get; set; }
        public string MAST_PRIMARY_SCHOOL	 { get; set; }
        public string MAST_MIDDLE_SCHOOL	 { get; set; }
        public string MAST_HIGH_SCHOOL	 { get; set; }
        public string MAST_INTERMEDIATE_SCHOOL { get; set; }
        public string MAST_DEGREE_COLLEGE	 { get; set; }
        public string MAST_HEALTH_SERVICE	 { get; set; }
        public string MAST_DISPENSARY	 { get; set; }
        public string MAST_MCW_CENTERS	 { get; set; }
        public string MAST_PHCS	 { get; set; }
        public string MAST_VETNARY_HOSPITAL	 { get; set; }
        public string MAST_TELEGRAPH_OFFICE	 { get; set; }
        public string MAST_TELEPHONE_CONNECTION	 { get; set; }
        public string MAST_BUS_SERVICE	 { get; set; }
        public string MAST_RAILWAY_STATION	 { get; set; }
        public string MAST_ELECTRICTY	 { get; set; }
        public string MAST_PANCHAYAT_HQ	 { get; set; }
        public string MAST_TOURIST_PLACE { get; set; }
        public string MAST_ELECTRICITY_ADD		 { get; set; }
        public string MAST_BLOCK_HQ	 { get; set; }
        public string MAST_SUB_TEHSIL	 { get; set; }	
        public string MAST_PETROL_PUMP	 { get; set; }
        public string MAST_PUMP_ADD { get; set; }	
        public string MAST_MANDI	 { get; set; }
        public string MAST_WAREHOUSE	 { get; set; }
        public string MAST_RETAIL_SHOP { get; set; }
        public string MAST_HABITATION_ACTIVE { get; set; } 
 
    }

    public class USP_MAS_DATA_MP_CONSTITUENCY_REPORT_Result
    {
        public int MAST_MP_CONST_CODE { get; set; }
        public string MAST_STATE_NAME { get; set; }
        public string MAST_MP_CONST_NAME { get; set; }
        public string MAST_MP_CONST_ACTIVE { get; set; }

    }

    public class USP_MAS_DATA_MLA_CONSTITUENCY_REPORT_Result
    {
        public int MAST_MLA_CONST_CODE { get; set; }
        public string MAST_STATE_NAME { get; set; }
        public string MAST_MLA_CONST_NAME { get; set; }
        public string MAST_MLA_CONST_ACTIVE { get; set; }

    }

    public class USP_MAS_DATA_MP_BLOCK_REPORT_Result
    {
        public int MAST_BLOCK_CODE { get; set; }
        public string MAST_STATE_NAME { get; set; }
        public string MAST_MP_CONST_NAME { get; set; }
        public string MAST_BLOCK_NAME { get; set; }
        public string MAST_MP_CONST_ACTIVE { get; set; }

    }

    public class USP_MAS_DATA_MLA_BLOCK_REPORT_Result
    {
        public int MAST_BLOCK_CODE { get; set; }
        public string MAST_STATE_NAME { get; set; }
        public string MAST_MLA_CONST_NAME { get; set; }
        public string MAST_BLOCK_NAME { get; set; }
        public string MAST_MLA_CONST_ACTIVE { get; set; }

    }

    public class USP_MAS_PANCHAYAT_REPORT_Result
    {
        public int MAST_PANCHAYAT_CODE { get; set; }
        public string MAST_STATE_NAME { get; set; }
        public string MAST_DISTRICT_NAME { get; set; }
        public string MAST_BLOCK_NAME { get; set; }
        public string MAST_PANCHAYAT_NAME { get; set; }
        public string MAST_PANCHAYAT_ACTIVE { get; set; }

    }

    public class USP_MAS_PANCHAYAT_HAB_REPORT_Result
    {
        public int MAST_HAB_CODE { get; set; }
        public string MAST_STATE_NAME { get; set; }
        public string MAST_DISTRICT_NAME { get; set; }
        public string MAST_BLOCK_NAME { get; set; }
        public string MAST_VILLAGE_NAME { get; set; }
        public string MAST_PANCHAYAT_NAME { get; set; }
        public string MAST_HAB_NAME { get; set; }
        public string MAST_PANCHAYAT_ACTIVE { get; set; }

    }

    public class USP_MAS_REGION_REPORT_Result
    {
        public int MAST_REGION_CODE { get; set; }
        public string MAST_STATE_NAME { get; set; }
        public string MAST_REGION_NAME { get; set; }
        public string MAST_REGION_ACTIVE { get; set; }
    }

    public class USP_MAS_REGION_DISTRICT_REPORT_Result
    {
        public int MAST_DISTRICT_CODE { get; set; }
        public string MAST_STATE_NAME { get; set; }
        public string MAST_DISTRICT_NAME { get; set; }
        public string MAST_REGION_NAME { get; set; }
        public string MAST_REGION_ACTIVE { get; set; }
    }
    
    public class USP_MAS_UNIT_REPORT_Result
    {
        public int MAST_UNIT_CODE { get; set; }
        public string MAST_UNIT_NAME { get; set; }
        public string MAST_UNIT_SHORT_NAME { get; set; }
        public int MAST_UNIT_DIMENSION { get; set; }
    }
    
    public class USP_MAS_ROAD_CATEGORY_REPORT_Result
    {
        public int MAST_ROAD_CAT_CODE { get; set; }
        public string MAST_ROAD_CAT_NAME { get; set; }
        public string MAST_ROAD_SHORT_DESC { get; set; }
    }

    public class USP_MAS_SCOUR_FOUNDATION_REPORT_Result
    {
        public int IMS_SC_FD_CODE { get; set; }
        public string IMS_SC_FD_NAME { get; set; }
        public string IMS_SC_FD_TYPE { get; set; }
    }

    public class USP_MAS_SOIL_REPORT_Result
    {
        public int MAST_SOIL_TYPE_CODE { get; set; }
        public string MAST_SOIL_TYPE_NAME { get; set; }
    }
    
    public class USP_MAS_STREAM_REPORT_Result
    {
        public int MAST_STREAM_CODE { get; set; }
        public string MAST_STREAM_NAME { get; set; }
        public string MAST_STREAM_TYPE { get; set; }
    }
    
    public class USP_MAS_TERRIAN_REPORT_Result
    {
        public int MAST_TERRAIN_TYPE_CODE { get; set; }
        public string MAST_TERRAIN_TYPE_NAME { get; set; }
        public decimal MAST_TERRAIN_ROADWAY_WIDTH { get; set; }
        public Nullable<int> MAST_TERRAIN_SLOP_FROM {get;set;}
        public Nullable<int> MAST_TERRAIN_SLOP_TO { get; set; }
    }
    
    public class USP_MAS_CD_WORKS_LENGTH_REPORT_Result
    {
        public int MAST_CDWORKS_CODE { get; set; }
        public string MAST_CDWORKS_NAME { get; set; }
    }
    
    public class USP_MAS_CD_WORKS_TYPE_REPORT_Result
    {
        public int MAST_CDWORKS_CODE { get; set; }
        public string MAST_CDWORKS_NAME { get; set; }
    }
    
    public class USP_MAS_COMPONENT_REPORT_Result
    {
        public int MAST_COMPONENT_CODE { get; set; }
        public string MAST_COMPONENT_NAME { get; set; }
    }

    
    public class USP_MAS_GRADE_REPORT_Result
    {
        public int MAST_GRADE_CODE { get; set; }
        public string MAST_GRADE_NAME { get; set; }
        public string MAST_GRADE_SHORT_NAME { get; set; }
    }

    public class USP_MAS_DESIGNATION_REPORT_Result
    {
        public int MAST_DESIG_CODE { get; set; }
        public string MAST_DESIG_NAME { get; set; }
        public string MAST_DESIG_TYPE { get; set; }
    }

    public class USP_MAS_REASON_REPORT_Result
    {
        public int MAST_REASON_CODE { get; set; }
        public string MAST_REASON_NAME { get; set; }
        public string MAST_REASON_TYPE { get; set; }
    }
    
    public class USP_MAS_CHECKLIST_POINT_REPORT_Result
    {
        public int MAST_CHECKLIST_POINTID { get; set; }
        public string MAST_CHECKLIST_ISSUES { get; set; }
        public string MAST_CHECKLIST_ACTIVE { get; set; }        
    }
   
    public class USP_MAS_EXECUTION_ITEM_REPORT_Result
    {
        public int MAST_HEAD_CODE { get; set; }
        public string MAST_HEAD_DESC { get; set; }
        public string MAST_HEAD_SH_DESC { get; set; }
        public string MAST_HEAD_TYPE { get; set; }
    }
    
    public class USP_MAS_TAXES_REPORT_Result
    {
        public int MAST_TDS_ID { get; set; }
        public decimal MAST_TDS { get; set; }
        public decimal MAST_TDS_SC { get; set; }
        public DateTime MAST_EFFECTIVE_DATE { get; set; }
    }
   
     public class USP_MAS_AGENCY_REPORT_Result
     {
        public int MAST_AGENCY_CODE { get; set; }
        public string MAST_AGENCY_NAME { get; set; }
        public string MAST_AGENCY_TYPE { get; set; }
     }

     public class USP_MAS_TRAFFIC_REPORT_Result
     {
         public int MAST_TRAFFIC_CODE { get; set; }
         public string MAST_TRAFFIC_NAME { get; set; }
         public string MAST_TRAFFIC_STATUS { get; set; }
     }
    
     public class USP_MAS_FUNDING_AGENCY_REPORT_Result
     {
         public int MAST_FUNDING_AGENCY_CODE { get; set; }
         public string MAST_FUNDING_AGENCY_NAME { get; set; }
     }

    public class USP_MAS_QUALIFICATION_REPORT_Result
     {
         public int MAST_QUALIFICATION_CODE { get; set; }
         public string MAST_QUALIFICATION_NAME { get; set; }
     }
    
    public class USP_MAS_LOK_SABHA_TERM_REPORT_Result
     {
         public int MAST_LS_TERM { get; set; }
         public string MAST_LS_START_DATE { get; set; }
         public string MAST_LS_END_DATE { get; set; }
     }

    public class USP_MAS_Officer_Category_REPORT_Result
    {
        public string MAST_OFFICER_CATEGORY_NAME { get; set; }
      
    }

    public class USP_MAS_CONTRACTOR_Registration_Bank_REPORT_Result
    {
        public int MAST_CON_ID { get; set; }
        public string MAST_CON_SUP_FLAG { get; set; }
        public string MAST_CON_COMPANY_NAME { get; set; }
        public string MAST_CON_NAME { get; set; }
        public string MAST_CON_PAN { get; set; }
        public string MAST_CON_STATUS { get; set; }
        public int MAST_REG_CODE { get; set; }
        public string MAST_CON_REG_NO { get; set; }
        public int MAST_CON_CLASS { get; set; }
        public string MAST_CON_VALID_FROM { get; set; }
        public string MAST_CON_VALID_TO { get; set; }
        public int MAST_REG_STATE { get; set; }
        public string MAST_STATE_NAME { get; set; }
        public string MAST_REG_OFFICE { get; set; }
        public string MAST_REG_STATUS { get; set; }
        public int MAST_ACCOUNT_ID { get; set; }
        public int MAST_DISTRICT_CODE { get; set; }
        public string MAST_DISTRICT_NAME { get; set; }
        public string MAST_ACCOUNT_NUMBER { get; set; }
        public string MAST_BANK_NAME { get; set; }
        public string MAST_IFSC_CODE { get; set; }
        public string MAST_ACCOUNT_STATUS { get; set; }
    }

    public class USP_MAS_CONTRACTOR_SUPPLIER_REPORT_Result
    {

        public int MAST_CON_ID { get; set; }
        public string MAST_CON_SUP_FLAG { get; set; }
        public string MAST_CON_COMPANY_NAME { get; set; }
        public string MAST_CON_NAME { get; set; }
        public string MAST_CON_ADDR { get; set; }
        public string MAST_CON_CONTACT { get; set; }
        public string MAST_CON_PAN { get; set; }
        public string MAST_CON_LEGAL_HEIR_NAME { get; set; }
        public string MAST_CON_EXPIRY_DATE { get; set; }
        public string MAST_CON_REMARKS { get; set; }
        public string MAST_CON_STATUS { get; set; }       
   
    }

    public class USP_MAS_Department_District_REPORT_Result
    {

        public int ADMIN_ND_CODE { get; set; }
        public int MAST_STATE_CODE { get; set; }
        public string MAST_STATE_NAME { get; set; }
        public int MAST_AGENCY_CODE { get; set; }
        public string MAST_AGENCY_NAME { get; set; }
        public int MAST_PARENT_ND_CODE { get; set; }
        public string MAST_PARENT_ND_NAME { get; set; }
        public string MAST_ND_TYPE { get; set; }
        public string ADMIN_ND_NAME { get; set; }
        public int MAST_DISTRICT_CODE { get; set; }
        public string MAST_DISTRICT_NAME { get; set; }

    }
    public class USP_MAS_Carriage_REPORT_Result
    {
        public int MAST_CARRIAGE_CODE { get; set; }
        public decimal MAST_CARRIAGE_WIDTH { get; set; }
        public string MAST_CARRIAGE_STATUS { get; set; }
    }
    public class USP_QM_Items_REPORT_Result
    {

        public int MAST_ITEM_NO { get; set; }
        public string MAST_QM_TYPE { get; set; }
        public int MAST_ITEM_CODE { get; set; }
        public int MAST_SUB_ITEM_CODE { get; set; }
        public string MAST_ITEM_NAME { get; set; }
        public string MAST_ITEM_ACTIVE { get; set; }
        public string MAST_ITEM_ACTIVATION_DATE { get; set; }
        public string MAST_ITEM_DEACTIVATION_DATE { get; set; }
        public string MAST_ITEM_STATUS { get; set; }
        public int MAST_GRADE_CODE { get; set; }
        public string MAST_GRADE_NAME { get; set; }
        
    }
    public class USP_MAS_Technology_REPORT_Result
    {
        public int MAST_TECH_CODE { get; set; }
        public string MAST_TECH_NAME { get; set; }
        public string MAST_TECH_DESC { get; set; }
        public string MAST_TECH_STATUS { get; set; }
       
       
    }
    public class USP_MAS_Test_REPORT_Result
    {
        public int MAST_TEST_CODE { get; set; }
        public string MAST_TEST_NAME { get; set; }
        public string MAST_TEST_DESC { get; set; }
        public string MAST_TEST_STATUS { get; set; } 
    }
    public class USP_MAS_Feedback_REPORT_Result
    {
        public int MAST_FEED_ID{ get; set; }
        public string MAST_FEED_NAME { get; set; }    
    }
    public class USP_MAS_AUTONOMOUS_BODY_REPORT_Result
     {
         public int MAST_STATE_CODE { get; set; }
         public string MAST_STATE_NAME { get; set; }
         public string ADMIN_AUTONOMOUS_BODY { get; set; }
     }

    public class USP_MAS_CONTRACTOR_CLASS_TYPE_REPORT_Result
    {
        public int MAST_CON_CLASS { get;set; }
        public string MAST_STATE_NAME { get; set; }
        public string MAST_CON_CLASS_TYPE_NAME {get; set; }
    }


    public class USP_MAS_CONTRACTOR_REGISTRATION_REPORT_Result
    {
        public int MAST_CON_ID { get;set; }
        public string MAST_CON_SUP_FLAG { get;set; }
        public string MAST_CON_COMPANY_NAME { get; set; }
        public string MAST_CON_NAME { get;set; }
        public string MAST_CON_PAN { get;set; }
        public string MAST_CON_STATUS { get;set; }
        public string MAST_CON_REG_NO { get;set; }
        public string MAST_CON_CLASS_TYPE_NAME { get;set; }
        public string MAST_CON_VALID_FROM { get;set; }
        public string MAST_CON_VALID_TO { get; set; }
        public string MAST_STATE_NAME { get; set; }
        public string MAST_REG_OFFICE { get;set; }
        public string MAST_REG_STATUS { get;set; }

        
    }
    
    public class USP_MAS_SQC_REPORT_Result
    {
        public int ADMIN_QC_CODE { get; set; }
        public string MAST_STATE_NAME { get; set; }
        public string ADMIN_QC_NAME { get; set; }
        public string MAST_DESIG_NAME { get; set; }

        public string ADMIN_QC_ADDRESS { get; set; }
        public string ADMIN_QC_CONTACT { get; set; }
        public string ADMIN_ACTIVE_STATUS { get; set; }
        public string ADMIN_ACTIVE_ENDDATE { get; set; }


    }

    public class USP_MAS_SRRDA_DPIU_REPORT_Result
    { 
        public int ADMIN_ND_CODE { get; set; }
        public string MAST_STATE_NAME { get; set; }
        public string MAST_AGENCY_NAME { get; set; }
        public string MAST_PARENT_ND_NAME { get; set; }
        public string MAST_ND_TYPE { get; set; }
        public string ADMIN_ND_NAME { get; set; }
        public string ADMIN_ND_ADDRESS { get; set; }
        public string ADMIN_ND_CONTACT { get; set; }
        public string ADMIN_ND_TAN_NO { get; set; }
        public string ADMIN_ND_REMARKS { get; set; }
    }

    public class USP_MAS_VIDHAN_SABHA_TERM_REPORT_Result
    {        
        public int MAST_VS_TERM { get; set; }
        public string MAST_STATE_NAME { get; set; }
        public string MAST_VS_START_DATE { get; set; }
        public string MAST_VS_END_DATE { get; set; }
    }
     public class USP_MAS_NODAL_OFFICER_REPORT_Result
     {
        public  int ADMIN_ND_CODE { get; set; }
        public string MAST_STATE_NAME { get; set; }
        public string MAST_AGENCY_NAME { get; set; }
        public string MAST_PARENT_ND_NAME { get; set; }
        public string MAST_ND_TYPE { get; set; }
         public string ADMIN_ND_NAME { get; set; }
        public string ADMIN_NO_NAME { get; set; }
        public string MAST_DESIG_NAME { get; set; }
        public string ADMIN_NO_ADDRESS { get; set; }
        public string ADMIN_NO_CONTACT { get; set; }
        public string ADMIN_NO_LEVEL { get; set; }
        public string ADMIN_ACTIVE_STATUS { get; set; }
        public string ADMIN_ACTIVE_START_DATE { get; set; }
        public string ADMIN_ACTIVE_END_DATE { get; set; }
	

     }
    
    public class USP_MAS_QUALITY_MONITOR_REPORT_Result
    {
        public int ADMIN_QM_CODE { get; set; }
        public string MAST_STATE_NAME { get; set; }
        public string ADMIN_QM_TYPE { get; set; }
        public string ADMIN_QM_NAME { get; set; }
        public string MAST_DESIG_NAME { get; set; }
        public string ADMIN_QM_ADDRESS { get; set; }
        public string ADMIN_QM_CONTACT { get; set; }
        public string ADMIN_QM_PAN { get; set; }
        public string ADMIN_QM_EMPANELLED { get; set; }
        public int ADMIN_QM_EMPANELLED_YEAR { get; set; }  
    }

    public class USP_MAS_TECHNICAL_AGENCY_REPORT_Result
    { 
        public int ADMIN_TA_CODE { get; set; }
        public string ADMIN_TA_TYPE { get; set; }
        public string ADMIN_TA_NAME { get; set; }
        public string ADMIN_TA_ADDRESS { get; set; }
        public string ADMIN_TA_CONTACT { get; set; }
        public string MAST_DESIG_NAME { get; set; }
        public string ADMIN_TA_CONTACT_NAME { get; set; }
        //public string MAST_STATE_NAME { get; set; }
        //public string MAST_DISTRICT_NAME { get; set; }
        //public string MAST_START_DATE { get; set; }
        //public string MAST_END_DATE { get; set; }

    }

    public class USP_MAS_TECHNICAL_AGENCY_StateMapping_REPORT_Result
    {
        public int ADMIN_TA_CODE { get; set; }
        public string ADMIN_TA_TYPE { get; set; }
        public string ADMIN_TA_NAME { get; set; }
        public string MAST_DESIG_NAME { get; set; }
        public string ADMIN_TA_CONTACT_NAME { get; set; }
        public string MAST_STATE_NAME { get; set; }
        public string MAST_DISTRICT_NAME { get; set; }
        public string MAST_START_DATE { get; set; }
        public string MAST_END_DATE { get; set; }
        public string MAST_IS_ACTIVE { get; set; }

    }
    public class USP_MAS_MLA_MEMBER_REPORT_Result
    { 
        public int MAST_MLA_CONST_CODE  { get; set; }
        public string MAST_STATE_NAME { get; set; }
        public string MAST_MLA_CONST_NAME { get; set; }
        public int MAST_VS_TERM { get; set; }
        public string MAST_VS_START_DATE { get; set; }
        public string MAST_VS_END_DATE { get; set; }
        public string MAST_MEMBER { get; set; }
        public string MAST_MEMBER_PARTY { get; set; }
        public string MAST_MEMBER_START_DATE { get; set; }
        public string MAST_MEMBER_END_DATE { get; set; }
        public string MAST_MLA_CONST_ACTIVE { get; set; }
    }

    public class USP_MAS_MP_MEMBER_REPORT_Result
    {
        public int MAST_MP_CONST_CODE { get; set; }
        public string MAST_STATE_NAME { get; set; }
        public string MAST_MP_CONST_NAME { get; set; }
        public int MAST_LS_TERM { get; set; }
        public string MAST_VS_START_DATE { get; set; }
        public string MAST_VS_END_DATE { get; set; }
        public string MAST_MEMBER { get; set; }
        public string MAST_MEMBER_PARTY { get; set; }
        public string MAST_MEMBER_START_DATE { get; set; }
        public string MAST_MEMBER_END_DATE { get; set; }
        public string MAST_MP_CONST_ACTIVE { get; set; }
    }

    public class USP_MAS_SURFACE_REPORT_Result
    { 
        public int MAST_SURFACE_CODE { get; set; }
        public string MAST_SURFACE_NAME { get; set; }
    }
}