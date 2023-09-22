using PMGSY.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.ProposalReports
{
    public class ProposalReportsModel
    {
        public ProposalReportsModel()
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

// MPR_REPORT1

    //state
    //public partial class USP_MPR_REPORT1_Result
    //{
    //    public int MAST_STATE_CODE { get; set; }
    //    public string MAST_STATE_NAME { get; set; }
    //    public int IMS_YEAR { get; set; }
    //    public Nullable<int> TN_PROPOSALS { get; set; }
    //    public Nullable<decimal> TN_LEN { get; set; }
    //    public Nullable<decimal> TN_AMT { get; set; }
    //    public Nullable<int> POP1000 { get; set; }
    //    public Nullable<int> POP999 { get; set; }
    //    public Nullable<int> POP499 { get; set; }
    //    public Nullable<int> POP250 { get; set; }
    //    public Nullable<int> TU_PROPOSALS { get; set; }
    //    public Nullable<decimal> TU_LEN { get; set; }
    //    public Nullable<decimal> TU_AMT { get; set; }
    //    public Nullable<int> TOT_PROP { get; set; }
    //    public Nullable<decimal> TOT_LEN { get; set; }
    //    public Nullable<decimal> TOT_AMT { get; set; }
    //    public Nullable<decimal> FUNDS_RELEASED { get; set; }
    //    public Nullable<int> CN_PROPOSALS { get; set; }
    //    public Nullable<decimal> CN_LEN { get; set; }
    //    public Nullable<decimal> CN_AMT { get; set; }
    //    public Nullable<int> CPOP1000 { get; set; }
    //    public Nullable<int> CPOP999 { get; set; }
    //    public Nullable<int> CPOP499 { get; set; }
    //    public Nullable<int> CPOP250 { get; set; }
    //    public Nullable<int> CU_PROPOSALS { get; set; }
    //    public Nullable<decimal> CU_LEN { get; set; }
    //    public Nullable<decimal> CU_AMT { get; set; }
    //    public Nullable<int> CTOT_PROP { get; set; }
    //    public Nullable<decimal> CTOT_LEN { get; set; }
    //    public Nullable<decimal> CTOT_AMT { get; set; }
    //    public Nullable<int> CCN_PROPOSALS { get; set; }
    //    public Nullable<decimal> CCN_LEN { get; set; }
    //    public Nullable<decimal> CCN_AMT { get; set; }
    //    public Nullable<int> CCPOP1000 { get; set; }
    //    public Nullable<int> CCPOP999 { get; set; }
    //    public Nullable<int> CCPOP499 { get; set; }
    //    public Nullable<int> CCPOP250 { get; set; }
    //    public Nullable<int> CCU_PROPOSALS { get; set; }
    //    public Nullable<decimal> CCU_LEN { get; set; }
    //    public Nullable<decimal> CCU_AMT { get; set; }
    //    public Nullable<int> CCTOT_PROP { get; set; }
    //    public Nullable<decimal> CCTOT_LEN { get; set; }
    //    public Nullable<decimal> CCTOT_AMT { get; set; }
    //}

    //district
    //public class USP_CN_MPR1_DISTRICT_REPORT
    //{
    //    public int MAST_DISTRICT_CODE { get; set; }
    //    public string MAST_DISTRICT_NAME { get; set; }
    //    public int IMS_YEAR { get; set; }
    //    public Nullable<int> TN_PROPOSALS { get; set; }
    //    public Nullable<decimal> TN_LEN { get; set; }
    //    public Nullable<decimal> TN_AMT { get; set; }
    //    public Nullable<int> POP1000 { get; set; }
    //    public Nullable<int> POP999 { get; set; }
    //    public Nullable<int> POP499 { get; set; }
    //    public Nullable<int> POP250 { get; set; }
    //    public Nullable<int> TU_PROPOSALS { get; set; }
    //    public Nullable<decimal> TU_LEN { get; set; }
    //    public Nullable<decimal> TU_AMT { get; set; }
    //    public Nullable<int> TOT_PROP { get; set; }
    //    public Nullable<decimal> TOT_LEN { get; set; }
    //    public Nullable<decimal> TOT_AMT { get; set; }
    //    public Nullable<decimal> FUNDS_RELEASED { get; set; }
    //    public Nullable<int> CN_PROPOSALS { get; set; }
    //    public Nullable<decimal> CN_LEN { get; set; }
    //    public Nullable<decimal> CN_AMT { get; set; }
    //    public Nullable<int> CPOP1000 { get; set; }
    //    public Nullable<int> CPOP999 { get; set; }
    //    public Nullable<int> CPOP499 { get; set; }
    //    public Nullable<int> CPOP250 { get; set; }
    //    public Nullable<int> CU_PROPOSALS { get; set; }
    //    public Nullable<decimal> CU_LEN { get; set; }
    //    public Nullable<decimal> CU_AMT { get; set; }
    //    public Nullable<int> CTOT_PROP { get; set; }
    //    public Nullable<decimal> CTOT_LEN { get; set; }
    //    public Nullable<decimal> CTOT_AMT { get; set; }
    //    public Nullable<int> CCN_PROPOSALS { get; set; }
    //    public Nullable<decimal> CCN_LEN { get; set; }
    //    public Nullable<decimal> CCN_AMT { get; set; }
    //    public Nullable<int> CCPOP1000 { get; set; }
    //    public Nullable<int> CCPOP999 { get; set; }
    //    public Nullable<int> CCPOP499 { get; set; }
    //    public Nullable<int> CCPOP250 { get; set; }
    //    public Nullable<int> CCU_PROPOSALS { get; set; }
    //    public Nullable<decimal> CCU_LEN { get; set; }
    //    public Nullable<decimal> CCU_AMT { get; set; }
    //    public Nullable<int> CCTOT_PROP { get; set; }
    //    public Nullable<decimal> CCTOT_LEN { get; set; }
    //    public Nullable<decimal> CCTOT_AMT { get; set; }
    //}

    ////block
    //public class USP_CN_MPR1_BLOCK_REPORT
    //{
    //    public int MAST_BLOCK_CODE { get; set; }
    //    public string MAST_BLOCK_NAME { get; set; }
    //    public int IMS_YEAR { get; set; }
    //    public Nullable<int> TN_PROPOSALS { get; set; }
    //    public Nullable<decimal> TN_LEN { get; set; }
    //    public Nullable<decimal> TN_AMT { get; set; }
    //    public Nullable<int> POP1000 { get; set; }
    //    public Nullable<int> POP999 { get; set; }
    //    public Nullable<int> POP499 { get; set; }
    //    public Nullable<int> POP250 { get; set; }
    //    public Nullable<int> TU_PROPOSALS { get; set; }
    //    public Nullable<decimal> TU_LEN { get; set; }
    //    public Nullable<decimal> TU_AMT { get; set; }
    //    public Nullable<int> TOT_PROP { get; set; }
    //    public Nullable<decimal> TOT_LEN { get; set; }
    //    public Nullable<decimal> TOT_AMT { get; set; }
    //    public Nullable<decimal> FUNDS_RELEASED { get; set; }
    //    public Nullable<int> CN_PROPOSALS { get; set; }
    //    public Nullable<decimal> CN_LEN { get; set; }
    //    public Nullable<decimal> CN_AMT { get; set; }
    //    public Nullable<int> CPOP1000 { get; set; }
    //    public Nullable<int> CPOP999 { get; set; }
    //    public Nullable<int> CPOP499 { get; set; }
    //    public Nullable<int> CPOP250 { get; set; }
    //    public Nullable<int> CU_PROPOSALS { get; set; }
    //    public Nullable<decimal> CU_LEN { get; set; }
    //    public Nullable<decimal> CU_AMT { get; set; }
    //    public Nullable<int> CTOT_PROP { get; set; }
    //    public Nullable<decimal> CTOT_LEN { get; set; }
    //    public Nullable<decimal> CTOT_AMT { get; set; }
    //    public Nullable<int> CCN_PROPOSALS { get; set; }
    //    public Nullable<decimal> CCN_LEN { get; set; }
    //    public Nullable<decimal> CCN_AMT { get; set; }
    //    public Nullable<int> CCPOP1000 { get; set; }
    //    public Nullable<int> CCPOP999 { get; set; }
    //    public Nullable<int> CCPOP499 { get; set; }
    //    public Nullable<int> CCPOP250 { get; set; }
    //    public Nullable<int> CCU_PROPOSALS { get; set; }
    //    public Nullable<decimal> CCU_LEN { get; set; }
    //    public Nullable<decimal> CCU_AMT { get; set; }
    //    public Nullable<int> CCTOT_PROP { get; set; }
    //    public Nullable<decimal> CCTOT_LEN { get; set; }
    //    public Nullable<decimal> CCTOT_AMT { get; set; }
    //}

    //final
    public class USP_CN_MPR1_FINAL_REPORT
    { 
        public int MAST_BLOCK_CODE { get; set; }	
        public string MAST_BLOCK_NAME { get; set; }	
        public string IMS_YEAR { get; set; }	
        public int IMS_BATCH { get; set; }
        public int IMS_PACKAGE_ID { get; set; }	
        public int IMS_PR_ROAD_CODE { get; set; }	
        public string IMS_PROPOSAL_TYPE { get; set; }	
        public string IMS_ROAD_NAME { get; set; }	
        public string IMS_BRIDGE_NAME { get; set; }	
        public string IMS_UPGRADE_CONNECT { get; set; }	
        public Nullable<decimal> IMS_PAV_LENGTH { get; set; }	
        public Nullable<decimal> IMS_BRIDGE_LENGTH { get; set; }	
        public int IMS_COLLABORATION { get; set; }	
        public Nullable<decimal> ROAD_AMT { get; set; }	
        public Nullable<decimal> BRIDGE_AMT { get; set; }	
        public Nullable<decimal> MAINT_AMT { get; set; }	
        public Nullable<decimal> TOTAL_LENGTH_COMPLETED { get; set; }	
        public Nullable<decimal> TOTAL_EXP { get; set; }
    }

    //_MPR_REPORT2

    //state
    //public partial class USP_MPR_REPORT2_STATE_Result
    //{
    //    public int MAST_STATE_CODE { get; set; }
    //    public string MAST_STATE_NAME { get; set; }
    //    public int TN_LEN { get; set; }
    //    public Nullable<int> TNM_LEN { get; set; }
    //    public Nullable<decimal> TNY_LEN { get; set; }
    //    public int TPOP1000 { get; set; }
    //    public int TPOP999 { get; set; }
    //    public int TPOP499 { get; set; }
    //    public int TPOP250 { get; set; }
    //    public int TOT_TPOP { get; set; }
    //    public Nullable<int> POP1000 { get; set; }
    //    public Nullable<int> POP999 { get; set; }
    //    public Nullable<int> POP499 { get; set; }
    //    public Nullable<int> POP250 { get; set; }
    //    public Nullable<int> TOT_POP { get; set; }
    //    public Nullable<int> YPOP1000 { get; set; }
    //    public Nullable<int> YPOP999 { get; set; }
    //    public Nullable<int> YPOP499 { get; set; }
    //    public Nullable<int> YPOP250 { get; set; }
    //    public Nullable<int> TOT_YPOP { get; set; }
    //    public int TU_LEN { get; set; }
    //    public Nullable<decimal> TUM_LEN { get; set; }
    //    public int TRM_LEN { get; set; }
    //    public Nullable<decimal> TUY_LEN { get; set; }
    //    public int TRY_LEN { get; set; }
    //}

    //district
    //public partial class USP_MPR_REPORT2_DISTRICT_Result
    //{
    //    public int MAST_DISTRICT_CODE { get; set; }
    //    public string MAST_DISTRICT_NAME { get; set; }
    //    public int TN_LEN { get; set; }
    //    public Nullable<int> TNM_LEN { get; set; }
    //    public Nullable<decimal> TNY_LEN { get; set; }
    //    public int TPOP1000 { get; set; }
    //    public int TPOP999 { get; set; }
    //    public int TPOP499 { get; set; }
    //    public int TPOP250 { get; set; }
    //    public int TOT_TPOP { get; set; }
    //    public Nullable<int> POP1000 { get; set; }
    //    public Nullable<int> POP999 { get; set; }
    //    public Nullable<int> POP499 { get; set; }
    //    public Nullable<int> POP250 { get; set; }
    //    public Nullable<int> TOT_POP { get; set; }
    //    public Nullable<int> YPOP1000 { get; set; }
    //    public Nullable<int> YPOP999 { get; set; }
    //    public Nullable<int> YPOP499 { get; set; }
    //    public Nullable<int> YPOP250 { get; set; }
    //    public Nullable<int> TOT_YPOP { get; set; }
    //    public int TU_LEN { get; set; }
    //    public Nullable<decimal> TUM_LEN { get; set; }
    //    public int TRM_LEN { get; set; }
    //    public Nullable<decimal> TUY_LEN { get; set; }
    //    public int TRY_LEN { get; set; }
    //}

    //block
    //public partial class USP_MPR_REPORT2_BLOCK_Result
    //{
    //    public int MAST_BLOCK_CODE { get; set; }
    //    public string MAST_BLOCK_NAME { get; set; }
    //    public int TN_LEN { get; set; }
    //    public Nullable<int> TNM_LEN { get; set; }
    //    public Nullable<decimal> TNY_LEN { get; set; }
    //    public int TPOP1000 { get; set; }
    //    public int TPOP999 { get; set; }
    //    public int TPOP499 { get; set; }
    //    public int TPOP250 { get; set; }
    //    public int TOT_TPOP { get; set; }
    //    public Nullable<int> POP1000 { get; set; }
    //    public Nullable<int> POP999 { get; set; }
    //    public Nullable<int> POP499 { get; set; }
    //    public Nullable<int> POP250 { get; set; }
    //    public Nullable<int> TOT_POP { get; set; }
    //    public Nullable<int> YPOP1000 { get; set; }
    //    public Nullable<int> YPOP999 { get; set; }
    //    public Nullable<int> YPOP499 { get; set; }
    //    public Nullable<int> YPOP250 { get; set; }
    //    public Nullable<int> TOT_YPOP { get; set; }
    //    public int TU_LEN { get; set; }
    //    public Nullable<decimal> TUM_LEN { get; set; }
    //    public int TRM_LEN { get; set; }
    //    public Nullable<decimal> TUY_LEN { get; set; }
    //    public int TRY_LEN { get; set; }
    //}

    //final
    public  class USP_MPR_REPORT2_FINAL_Result
    {
        public int MAST_BLOCK_CODE { get; set; }
        public string MAST_BLOCK_NAME { get; set; }
        public string IMS_YEAR { get; set; }	
        public int IMS_BATCH { get; set; }	
        public string IMS_PACKAGE_ID { get; set; }	
        public int IMS_PR_ROAD_CODE { get; set; }	
        public string IMS_PROPOSAL_TYPE { get; set; }	
        public string IMS_ROAD_NAME { get; set; }	
        public string IMS_BRIDGE_NAME { get; set; }	
        public string IMS_UPGRADE_CONNECT { get; set; }	
        public decimal IMS_PAV_LENGTH { get; set; }	
        public decimal IMS_BRIDGE_LENGTH { get; set; }	
        public int IMS_COLLABORATION { get; set; }	
        public decimal ROAD_AMT { get; set; }	
        public decimal BRIDGE_AMT { get; set; }	
        public decimal MAINT_AMT { get; set; }	
        public decimal TOTAL_LENGTH_COMPLETED { get; set; }
        public decimal TOTAL_EXP { get; set; }
    }

    //MPR_HY1 and MPR_HY2
   

   
    //final
    public  class USP_PropList_REPORT_FINAL_Result
    {
        public int MAST_STATE_CODE { get; set; }
        public string MAST_STATE_NAME { get; set; }
        public int MAST_DISTRICT_CODE { get; set; }
        public string MAST_DISTRICT_NAME { get; set; }
        public int MAST_BLOCK_CODE { get; set; }
        public string MAST_BLOCK_NAME { get; set; }
        public string IMS_YEAR { get; set; }
        public int IMS_BATCH { get; set; }
        public string IMS_PACKAGE_ID { get; set; }
        public string ROAD_NAME { get; set; }
        public decimal ROAD_LENGTH { get; set; }
        public decimal IMS_BT_LENGTH { get; set; }
        public decimal IMS_CC_LENGTH { get; set; }
        public string IMS_COLLABORATION { get; set; }
        public decimal ROAD_AMT { get; set; }
        public decimal BRIDGE_AMT { get; set; }
        public decimal MAINT_AMT { get; set; }
        public int MAST_HAB_CODE{ get; set; }
        public string MAST_HAB_NAME { get; set; }
        public string MAST_HAB_STATUS { get; set; }
        public int MAST_HAB_TOT_POP { get; set; }
        public string PROPOSAL_STATUS { get; set; }
    }


    //final
    public  class USP_Prop_Length_REPORT_FINAL_Result
    {
        public int MAST_STATE_CODE { get; set; }
        public string MAST_STATE_NAME { get; set; }
        public int MAST_DISTRICT_CODE { get; set; }
        public string MAST_DISTRICT_NAME { get; set; }
        public int MAST_BLOCK_CODE { get; set; }
        public string MAST_BLOCK_NAME { get; set; }
        public int IMS_PR_ROADE_CODE { get; set; }
        public string IMS_YEAR { get; set; }
        public int IMS_BATCH { get; set; }
        public string IMS_PACKAGE_ID { get; set; }
        public string ROAD_NAME { get; set; }
        public string IMS_UPGRADE_CONNECT { get; set; }
        public decimal IMS_PAV_LENGTH { get; set; }
        public decimal IMS_CC_LENGTH { get; set; }
        public decimal IMS_BT_LENGTH { get; set; }
        public decimal ROAD_AMT { get; set; }
        public decimal BRIDGE_AMT { get; set; }     
        public decimal MAINT_AMT { get; set; }
        public decimal TOTAL_AMT { get; set; }
      
    }


    //final
    public  class USP_Prop_EMC_REPORT_FINAL_Result
    {
        public int MAST_STATE_CODE { get; set; }
        public string MAST_STATE_NAME { get; set; }
        public int MAST_DISTRICT_CODE { get; set; }
        public string MAST_DISTRICT_NAME { get; set; }
        public int MAST_BLOCK_CODE { get; set; }
        public string MAST_BLOCK_NAME { get; set; }
        public int IMS_PR_ROADE_CODE { get; set; }
        public string IMS_YEAR { get; set; }
        public int IMS_BATCH { get; set; }
        public string IMS_PACKAGE_ID { get; set; }
        public string ROAD_NAME { get; set; }
        public string IMS_UPGRADE_CONNECT { get; set; }   
        public decimal ROAD_AMT { get; set; }
        public decimal BRIDGE_AMT { get; set; }
        public decimal IMS_SANCTIONED_MAN_AMT1 { get; set; }
        public decimal IMS_SANCTIONED_MAN_AMT2 { get; set; }
        public decimal IMS_SANCTIONED_MAN_AMT3 { get; set; }
        public decimal IMS_SANCTIONED_MAN_AMT4 { get; set; }
        public decimal IMS_SANCTIONED_MAN_AMT5 { get; set; }
        public decimal MAINT_AMT { get; set; }
        public decimal TOTAL_AMT { get; set; }
    }

    public  class USP_PROP_SCRUTINY_List_REPORT_Result
    {
        public string MAST_STATE_NAME { get; set; }
        public int MAST_STATE_CODE { get; set; }
        public string MAST_DISTRICT_NAME { get; set; }
        public int MAST_DISTRICT_CODE { get; set; }
        public string IMS_YEAR { get; set; }
        public string MAST_FUNDING_AGENCY_NAME { get; set; }
        public int MAST_FUNDING_AGENCY_CODE { get; set; }
        public int TA_CODE { get; set; }
        public string TA_NAME { get; set; }
        public int TA_PROPOSALS { get; set; }
        public decimal TA_ROAD_AMT { get; set; }
        public decimal TA_BRIDGE_AMT { get; set; }
        public decimal TA_MAINT_AMT { get; set; }
        public int MRD_PROPOSALS { get; set; }
        public decimal MRD_ROAD_AMT { get; set; }
        public decimal MRD_BRIDGE_AMT { get; set; }
        public decimal MRD_MAINT_AMT { get; set; }
    }


    public class USP_PROPDATA_HAB_REPORT_Result
    {
        public string LOCATION_NAME { get; set; }
        public int LOCATION_CODE { get; set; }
         public int IMS_YEAR { get; set; }
         public string PHASE { get; set; }
         public int ROADS { get; set; }
      
    }
    public partial class USP_PROPDATA_CNNOTMAP_District_Result
    {
        public int LOCATION_CODE { get; set; }
        public string LOCATION_NAME { get; set; }
        public int PROPOSALS { get; set; }
        public int IMS_YEAR { get; set; }
        public string PHASE { get; set; }
    }
    public class USP_PROPDATA_CNDUP_REPORT_Result
    {
        public int MAST_STATE_CODE { get; set; }
        public string MAST_STATE_NAME { get; set; }
        public int MAST_DISTRICT_CODE { get; set; }
        public string MAST_DISTRICT_NAME { get; set; }
        public int MAST_BLOCK_CODE { get; set; }
        public string MAST_BLOCK_NAME { get; set; }
        public string PLAN_CN_ROAD_NUMBER { get; set; }
        public Nullable<int> DUPLICATES { get; set; }

    }
   
    public partial class USP_PROPDATA_CNDUP_DETAILS_Report_Result
    {
        public string MAST_STATE_NAME { get; set; }
        public string MAST_DISTRICT_NAME { get; set; }
        public string MAST_BLOCK_NAME { get; set; }
        public string PLAN_CN_ROAD_NUMBER { get; set; }
        public string PLAN_RD_NAME { get; set; }
        public Nullable<decimal> PLAN_RD_LENGTH { get; set; }
        public Nullable<decimal> PLAN_RD_FROM_CHAINAGE { get; set; }
        public Nullable<decimal> PLAN_RD_TO_CHAINAGE { get; set; }
    }
    public class USP_PROPDATA_MAINZ_REPORT_Result
    {
        public string MAST_DISTRICT_NAME { get; set; }
        public int MAST_DISTRICT_CODE { get; set; }
        public int IMS_YEAR { get; set; }
        public string PHASE { get; set; }
        public int PROPOSALS { get; set; }
    }

    public class USP_PROPDATA_CARRIAGE_Report
    {
        public int LOCATION_CODE { get; set; }
        public int MAST_CARRIAGE_CODE { get; set; }
        public string LOCATION_NAME { get; set; }
        public string CarriageWidth { get; set; }
        public int Proposals { get; set; }
        public string PHASE { get; set; }
        public int IMS_YEAR { get; set; }
    }
    public partial class USP_PROPOSAL_MAINTENANCE_FIN_REPORT_Result
    {
        public string MAST_STATE_NAME { get; set; }
        public string MAST_DISTRICT_NAME { get; set; }
        public string MAST_BLOCK_NAME { get; set; }
        public int IMS_PR_ROAD_CODE { get; set; }
        public string IMS_YEAR { get; set; }
        public int IMS_BATCH { get; set; }
        public string IMS_PACKAGE_ID { get; set; }
        public string ROAD_NAME { get; set; }     
        public decimal ROAD_AMT { get; set; }
        public decimal BRIDGE_AMT { get; set; }
        public decimal ROAD_STATE { get; set; }
        public decimal BRIDGE_STATE { get; set; }
        public decimal MAINT_AMT { get; set; }
        public decimal ROAD_LENGTH { get; set; }   
        public decimal MANE_VALUEOFWORK_LASTMONTH { get; set; }
        public decimal MANE_VALUEOFWORK_THISMONTH { get; set; }
        public decimal MANE_PAYMENT_LASTMONTH { get; set; }
        public decimal MANE_PAYMENT_THISMONTH { get; set; }
        public string MANE_FINAL_PAYMENT_FLAG { get; set; }        
        public Nullable<System.DateTime> MANE_FINAL_PAYMENT_DATE { get; set; }
        public string PROGRESS_PERIOD { get; set; }
        public string MRD_SANCTIONED { get; set; }
        public string STA_SANCTIONED { get; set; }
    }
    public class charModel
    {
        public string x { get; set; }
        public string name { get; set; }
        public string y { get; set; }
         public string z { get; set; }
        //  public charModel()
        //{

        //}
    }

    /// <summary>
    /// Added by SAMMED PATIL 14 JUNE 2014
    /// </summary>
    public class MRDProposalModel
    {
        public int State_Code { get; set; }

        [Display(Name = "State")]
        [Required(ErrorMessage = "Please select State")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid  State")]
        public int StateCode { get; set; }
        public string StateName { get; set; }
        public List<SelectListItem> StateList { get; set; }

        [Display(Name = "District")]
        [Required(ErrorMessage = "Please select District")]
        [Range(0, int.MaxValue, ErrorMessage = "Invalid  District")]
        public int DistrictCode { get; set; }
        public string DistrictName { get; set; }
        public List<SelectListItem> DistrictList { get; set; }

        [Display(Name = "Block")]
        [Required(ErrorMessage = "Please select Block")]
        [Range(0, int.MaxValue, ErrorMessage = "Invalid  Block")]
        public int BlockCode { get; set; }
        public string BlockName { get; set; }
        public List<SelectListItem> BlockList { get; set; }

        [Display(Name = "Sanctioned Year")]
        [Required(ErrorMessage = "Please select Year")]
        [Range(0, int.MaxValue, ErrorMessage = "Invalid  Year")]
        public int Year { get; set; }
        public SelectList YearList { get; set; }
        public string YearName { get; set; }

        [Display(Name = "Batch")]
        [Required(ErrorMessage = "Please select Batch")]
        [Range(0, int.MaxValue, ErrorMessage = "Invalid  Batch")]
        public int BatchCode { get; set; }
        public string BatchName { get; set; }
        public List<SelectListItem> BatchList { get; set; }

        [Display(Name = "Collaboration")]
        [Required(ErrorMessage = "Please select Collaboration")]
        [Range(-1, int.MaxValue, ErrorMessage = "Invalid  Collaboration")]
        public int CollabCode { get; set; }
        public string CollabName { get; set; }
        public List<SelectListItem> CollabList { get; set; }

        [Display(Name = "Package")]
        [Required(ErrorMessage = "Please select Package")]
        //[Range(0, int.MaxValue, ErrorMessage = "Invalid  Package")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Invalid Package ID,Can only contains AlphaNumeric values")]
        //[IsNewPackage("IMS_EXISTING_PACKAGE", ErrorMessage = "Please Enter Package Number")]
        //[IsPackageExists("IMS_YEAR", "IMS_PR_ROAD_CODE", "IMS_EXISTING_PACKAGE", "IMS_PACKAGE_ID", ErrorMessage = "Package Number Already Taken,Please Choose Different Package Number")]
        public string PackageCode { get; set; }
        public string PackageName { get; set; }
        public List<IMS_SANCTIONED_PROJECTS> PackageList { get; set; }

        [Display(Name = "MRD Status")]
        [Required(ErrorMessage = "Please select Status")]
        //[Range(0, int.MaxValue, ErrorMessage = "Please select Status")]
        [RegularExpression(@"^([NYURDSA]+)$", ErrorMessage = "Invalid Status selected")]
        public string StatusCode { get; set; }
        public string StatusName { get; set; }
        public List<SelectListItem> StatusList { get; set; }

        [Display(Name = "STA Status")]
        [Required(ErrorMessage = "Please select STA Status")]
        //[Range(0, int.MaxValue, ErrorMessage = "Invalid STA Status selected")]
        [RegularExpression(@"^([0NY]+)$", ErrorMessage = "Invalid Status selected")]
        public string STAStatusCode { get; set; }
        public string STAStatusName { get; set; }
        public List<SelectListItem> STAStatusList { get; set; }

        [Display(Name = "Proposal")]
        [Required(ErrorMessage = "Please select Proposals")]
        [RegularExpression(@"^([0LP]+)$", ErrorMessage = "Invalid Proposal selected")]
        public string ProposalCode { get; set; }
        public string ProposalName { get; set; }
        public List<SelectListItem> ProposalList { get; set; }

        [Display(Name = "Category")]
        [Required(ErrorMessage = "Please select Category")]
        //[Range(0, int.MaxValue, ErrorMessage = "Invalid Category")]
        [RegularExpression(@"^([0NU]+)$", ErrorMessage = "Invalid Status selected")]
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
        public List<SelectListItem> CategoryList { get; set; }

        [Display(Name = "Agency")]
        [Required(ErrorMessage = "Please select Agency")]
        [Range(0, int.MaxValue, ErrorMessage = "Invalid Agency")]
        //[RegularExpression(@"^([0NU]+)$", ErrorMessage = "Invalid Status selected")]
        public int Agency { get; set; }
        public string AgencyName { get; set; }
        public List<SelectListItem> AgencyList { get; set; }

        public int Level { get; set; }
        //public int BlockCode { get; set; }
        //public int Agency { get; set; }
        public int PMGSY { get; set; }
        public string PTAStatus { get; set; }
        public string MRDStatus { get; set; }

        public List<USP_MRD_PROPOSAL_REPORT_Result> lstMRDProposal = new List<USP_MRD_PROPOSAL_REPORT_Result>();

        public List<USP_MRD_HAB_COVERAGE_Result> lstMRDProposalHab = new List<USP_MRD_HAB_COVERAGE_Result>();
    }

    public class MRDProposalBridgeTypeDetailsModel
    {
        //[Display(Name = "State")]
        //[Required(ErrorMessage = "Please select State")]
        //[Range(1, int.MaxValue, ErrorMessage = "Invalid Road Code")]

        public int PMGSY { get; set; }

        public string StateName { get; set; }

        public int PrRoadCode { get; set; }
        
        public string DistrictName { get; set; }
        
        public string BlockName { get; set; }

        public string Package { get; set; }

        public string SanctionYear { get; set; }

        public string RoadName { get; set; }

        public string BatchName { get; set; }

        public string RoadLength { get; set; }

        public string CollabName { get; set; }

        public string BridgeName { get; set; }

        public string BridgeLength { get; set; }

        public string Proposal { get; set; }

        public List<SelectListItem> path { get; set; }

        public List<RoadCBRListing> RoadCBRDetails = new List<RoadCBRListing>();
        
        public List<BridgeListing> BridgeDetails = new List<BridgeListing>();

        public List<BridgeCostListing> BridgeCostDetails = new List<BridgeCostListing>();

        public List<BridgeEstCostListing> BridgeEstCostDetails = new List<BridgeEstCostListing>();
    }

    public class RoadCBRListing
    {
        public string IMS_SEGMENT_NO { get; set; }
        public string IMS_STR_CHAIN { get; set; }
        public string IMS_END_CHAIN { get; set; }
        public string SEGMENT_LENGTH { get; set; }
        public string IMS_CBR_VALUE { get; set; }
    }

    public class BridgeListing
    {
        public string IMS_ROAD_TYPE_LEVEL { get; set; }
        public string IMS_AVERAGE_GROUND_LEVEL { get; set; }
        public string IMS_NALA_BED_LEVEL { get; set; }

        public string IMS_HIGHEST_FLOOD_LEVEL { get; set; }

        public string IMS_ORDINARY_FLOOD_LEVEL { get; set; }

        public string IMS_FOUNDATION_LEVEL { get; set; }
        public string IMS_HGT_BIRDGE_NBL { get; set; }
        public string IMS_HGT_BRIDGE_FL { get; set; }
        public string IMS_BRG_SUBMERSIBLE { get; set; }
        public string IMS_BRG_BOX_CULVERT { get; set; }
        public string IMS_BRG_RCC_ABUMENT { get; set; }
        public string IMS_BRG_HLB { get; set; }
        public string IMS_SC_FD_CODE { get; set; }
        public string IMS_BEARING_CAPACITY { get; set; }
        public string IMS_ARG_TOT_SPANS { get; set; }
        public string IMS_NO_OF_VENTS { get; set; }
        public string IMS_SPAN_VENT { get; set; }
        public string IMS_SCOUR_DEPTH { get; set; }
        public string IMS_WIDTH_OF_BRIDGE { get; set; }
    }

    public class BridgeCostListing
    {
        

        public string Year1 { get; set; }
        public string Year2 { get; set; }
        public string Year3 { get; set; }

        public string Year4 { get; set; }

        public string Year5 { get; set; }

        public string IMS_RENEWAL_COST { get; set; }

        public string TotalCost { get; set; }
    }


    public class BridgeEstCostListing
    {
        public string IMS_BRGD_STRUCTURE_COST { get; set; }
        public string IMS_STRUCTURE_COST { get; set; }
        public string IMS_BRGD_OTHER_COST { get; set; }

        public string IMS_APPROACH_PER_MTR { get; set; }

        public string IMS_BRGD_STRUCTURE_PER_MTR { get; set; }
        public string IMS_STRUCTURE_PER_MTR { get; set; }

        public string IMS_BRGD_OTHER_PER_MTR { get; set; }
    
    }
}
