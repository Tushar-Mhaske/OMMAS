using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Models.CoreNetworkReports
{
    public class CoreNetworkReportsStoredProc
    {
    }

    //public partial class USP_CN1_DISTRICT_REPORT_Result
    //{
    //    public int MAST_DISTRICT_CODE { get; set; }
    //    public string MAST_DISTRICT_NAME { get; set; }
    //    public Nullable<int> TOTAL_CN { get; set; }
    //    public Nullable<decimal> TOTAL_LEN { get; set; }
    //    public Nullable<decimal> BT_LEN { get; set; }
    //    public Nullable<decimal> WBM_LEN { get; set; }
    //    public Nullable<decimal> GRAVEL_LEN { get; set; }
    //    public Nullable<decimal> TRACK_LEN { get; set; }
    //}

    //public partial class USP_CN1_BLOCK_REPORT_Result
    //{
    //    public int MAST_BLOCK_CODE { get; set; }
    //    public string MAST_BLOCK_NAME { get; set; }
    //    public Nullable<int> TOTAL_CN { get; set; }
    //    public Nullable<decimal> TOTAL_LEN { get; set; }
    //    public Nullable<decimal> BT_LEN { get; set; }
    //    public Nullable<decimal> WBM_LEN { get; set; }
    //    public Nullable<decimal> GRAVEL_LEN { get; set; }
    //    public Nullable<decimal> TRACK_LEN { get; set; }
    //}

    public partial class USP_CN1_FINAL_REPORT_Result
    {
        public string PLAN_CN_ROAD_NUMBER { get; set; }
        public string PLAN_RD_NAME { get; set; }
        public string RD_FROM { get; set; }
        public string RD_TO { get; set; }
        public Nullable<decimal> PLAN_RD_LENGTH { get; set; }
        public Nullable<decimal> BT_LEN { get; set; }
        public Nullable<decimal> WBM_LEN { get; set; }
        public Nullable<decimal> GRAVEL_LEN { get; set; }
        public Nullable<decimal> TRACK_LEN { get; set; }
        public Nullable<int> MAST_HAB_CODE { get; set; }
        public string MAST_HAB_NAME { get; set; }
        public Nullable<int> MAST_HAB_TOT_POP { get; set; }
        public string MAST_HAB_CONNECTED { get; set; }
        public string TOTAL_POPULATION_SERVED { get; set; }
        public string DOWNLOAD_KML_FILE { get; set; }
    }

    //public partial class USP_CN2_DISTRICT_REPORT_Result
    //{
    //    public int LOCATION_CODE { get; set; }
    //    public string LOCATION_NAME { get; set; }
    //    public int TPOP1000 { get; set; }
    //    public int TPOP999 { get; set; }
    //    public int TPOP499 { get; set; }
    //    public int TPOP250 { get; set; }
    //    public int UPOP1000 { get; set; }
    //    public int UPOP999 { get; set; }
    //    public int UPOP499 { get; set; }
    //    public int UPOP250 { get; set; }
    //    public int CPOP1000 { get; set; }
    //    public int CPOP999 { get; set; }
    //    public int CPOP499 { get; set; }
    //    public int CPOP250 { get; set; }
    //}

    //public partial class USP_CN2_BLOCK_REPORT_Result
    //{
    //    public int LOCATION_CODE { get; set; }
    //    public string LOCATION_NAME { get; set; }
    //    public int TPOP1000 { get; set; }
    //    public int TPOP999 { get; set; }
    //    public int TPOP499 { get; set; }
    //    public int TPOP250 { get; set; }
    //    public int UPOP1000 { get; set; }
    //    public int UPOP999 { get; set; }
    //    public int UPOP499 { get; set; }
    //    public int UPOP250 { get; set; }
    //    public int CPOP1000 { get; set; }
    //    public int CPOP999 { get; set; }
    //    public int CPOP499 { get; set; }
    //    public int CPOP250 { get; set; }
    //}

    public partial class USP_CN2_FINAL_REPORT_Result
    {
        public string PLAN_CN_ROAD_NUMBER { get; set; }
        public string PLAN_RD_NAME { get; set; }
        public string RD_FROM { get; set; }
        public string RD_TO { get; set; }
        public string PLAN_RD_ROUTE { get; set; }
        public Nullable<decimal> PLAN_RD_LENGTH { get; set; }
        public string MAST_ER_ROAD_NUMBER { get; set; }
        public string MAST_ER_ROAD_NAME { get; set; }
        public string PLAN_RD_LENG { get; set; }
        public string MAST_HAB_NAME { get; set; }
        public Nullable<int> MAST_HAB_TOT_POP { get; set; }
        public string MAST_HAB_CONNECTED { get; set; }
    }

    //public partial class USP_CN3_STATE_REPORT_Result
    //{
    //    public int MAST_STATE_CODE { get; set; }
    //    public string MAST_STATE_NAME { get; set; }
    //    public Nullable<int> TOTAL_CN { get; set; }
    //    public Nullable<decimal> TOTAL_LEN { get; set; }
    //    public Nullable<decimal> BT_LEN { get; set; }
    //    public Nullable<decimal> WBM_LEN { get; set; }
    //    public Nullable<decimal> GRAVEL_LEN { get; set; }
    //    public Nullable<decimal> TRACK_LEN { get; set; }
    //    public Nullable<int> CN_TOTAL { get; set; }
    //    public Nullable<decimal> CN_LEN { get; set; }
    //    public Nullable<decimal> CN_BT_LEN { get; set; }
    //    public Nullable<decimal> CN_WBM_LEN { get; set; }
    //    public Nullable<decimal> CN_GRAVEL_LEN { get; set; }
    //    public Nullable<decimal> CN_TRACK_LEN { get; set; }
    //}

    //public partial class USP_CN3_DISTRICT_REPORT_RESULT 
    //{
    //    public int MAST_DISTRICT_CODE { get; set; }
    //    public string MAST_DISTRICT_NAME { get; set; }
    //    public Nullable<int> TOTAL_CN { get; set; }
    //    public Nullable<decimal> TOTAL_LEN { get; set; }
    //    public Nullable<decimal> BT_LEN { get; set; }
    //    public Nullable<decimal> WBM_LEN { get; set; }
    //    public Nullable<decimal> GRAVEL_LEN { get; set; }
    //    public Nullable<decimal> TRACK_LEN { get; set; }
    //}

    //public partial class USP_CN3_BLOCK_REPORT_RESULT
    //{
    //    public int MAST_BLOCK_CODE { get; set; }
    //    public string MAST_BLOCK_NAME { get; set; }
    //    public Nullable<int> TOTAL_CN { get; set; }
    //    public Nullable<decimal> TOTAL_LEN { get; set; }
    //    public Nullable<decimal> BT_LEN { get; set; }
    //    public Nullable<decimal> WBM_LEN { get; set; }
    //    public Nullable<decimal> GRAVEL_LEN { get; set; }
    //    public Nullable<decimal> TRACK_LEN { get; set; }
    //}

    public partial class USP_CN3_FINAL_REPORT_Result
    {
        public string PLAN_CN_ROAD_NUMBER { get; set; }
        public string PLAN_RD_NAME { get; set; }
        public string PLAN_RD_FROM { get; set; }
        public string PLAN_RD_TO { get; set; }
        public Nullable<decimal> PLAN_RD_LENGTH { get; set; }
        public Nullable<int> PLAN_RD_TO_HAB { get; set; }
        public Nullable<int> TOTAL_POP { get; set; }
    }

    //public partial class USP_CN4_STATE_REPORT_Result
    //{
    //    public int MAST_STATE_CODE { get; set; }
    //    public string MAST_STATE_NAME { get; set; }
    //    public Nullable<int> TOTAL_CN { get; set; }
    //    public Nullable<decimal> TOTAL_LEN { get; set; }
    //    public Nullable<decimal> BT_LEN { get; set; }
    //    public Nullable<decimal> WBM_LEN { get; set; }
    //    public Nullable<decimal> GRAVEL_LEN { get; set; }
    //    public Nullable<decimal> TRACK_LEN { get; set; }
    //    public Nullable<decimal> NH_BT_LEN { get; set; }
    //    public Nullable<decimal> NH_WBM_LEN { get; set; }
    //    public Nullable<decimal> SH_BT_LEN { get; set; }
    //    public Nullable<decimal> SH_WBM_LEN { get; set; }
    //    public Nullable<decimal> MDR_BT_LEN { get; set; }
    //    public Nullable<decimal> MDR_WBM_LEN { get; set; }
    //    public Nullable<decimal> MDR_GRAVEL_LEN { get; set; }
    //    public Nullable<decimal> MDR_TRACK_LEN { get; set; }
    //    public Nullable<decimal> LR_BT_LEN { get; set; }
    //    public Nullable<decimal> LR_WBM_LEN { get; set; }
    //    public Nullable<decimal> LR_GRAVEL_LEN { get; set; }
    //    public Nullable<decimal> LR_TRACK_LEN { get; set; }
    //    public Nullable<decimal> TR_BT_LEN { get; set; }
    //    public Nullable<decimal> TR_WBM_LEN { get; set; }
    //    public Nullable<decimal> TR_GRAVEL_LEN { get; set; }
    //    public Nullable<decimal> TR_TRACK_LEN { get; set; }
    //}

    //public partial class USP_CN4_DISTRICT_REPORT_Result
    //{
    //    public int MAST_DISTRICT_CODE { get; set; }
    //    public string MAST_DISTRICT_NAME { get; set; }
    //    public Nullable<int> TOTAL_CN { get; set; }
    //    public Nullable<decimal> TOTAL_LEN { get; set; }
    //    public Nullable<decimal> BT_LEN { get; set; }
    //    public Nullable<decimal> WBM_LEN { get; set; }
    //    public Nullable<decimal> GRAVEL_LEN { get; set; }
    //    public Nullable<decimal> TRACK_LEN { get; set; }
    //    public Nullable<decimal> NH_BT_LEN { get; set; }
    //    public Nullable<decimal> NH_WBM_LEN { get; set; }
    //    public Nullable<decimal> SH_BT_LEN { get; set; }
    //    public Nullable<decimal> SH_WBM_LEN { get; set; }
    //    public Nullable<decimal> MDR_BT_LEN { get; set; }
    //    public Nullable<decimal> MDR_WBM_LEN { get; set; }
    //    public Nullable<decimal> MDR_GRAVEL_LEN { get; set; }
    //    public Nullable<decimal> MDR_TRACK_LEN { get; set; }
    //    public Nullable<decimal> LR_BT_LEN { get; set; }
    //    public Nullable<decimal> LR_WBM_LEN { get; set; }
    //    public Nullable<decimal> LR_GRAVEL_LEN { get; set; }
    //    public Nullable<decimal> LR_TRACK_LEN { get; set; }
    //    public Nullable<decimal> TR_BT_LEN { get; set; }
    //    public Nullable<decimal> TR_WBM_LEN { get; set; }
    //    public Nullable<decimal> TR_GRAVEL_LEN { get; set; }
    //    public Nullable<decimal> TR_TRACK_LEN { get; set; }
    //}

    //public partial class USP_CN4_BLOCK_REPORT_Result
    //{
    //    public int MAST_BLOCK_CODE { get; set; }
    //    public string MAST_BLOCK_NAME { get; set; }
    //    public Nullable<int> TOTAL_CN { get; set; }
    //    public Nullable<decimal> TOTAL_LEN { get; set; }
    //    public Nullable<decimal> BT_LEN { get; set; }
    //    public Nullable<decimal> WBM_LEN { get; set; }
    //    public Nullable<decimal> GRAVEL_LEN { get; set; }
    //    public Nullable<decimal> TRACK_LEN { get; set; }
    //    public Nullable<decimal> NH_BT_LEN { get; set; }
    //    public Nullable<decimal> NH_WBM_LEN { get; set; }
    //    public Nullable<decimal> SH_BT_LEN { get; set; }
    //    public Nullable<decimal> SH_WBM_LEN { get; set; }
    //    public Nullable<decimal> MDR_BT_LEN { get; set; }
    //    public Nullable<decimal> MDR_WBM_LEN { get; set; }
    //    public Nullable<decimal> MDR_GRAVEL_LEN { get; set; }
    //    public Nullable<decimal> MDR_TRACK_LEN { get; set; }
    //    public Nullable<decimal> LR_BT_LEN { get; set; }
    //    public Nullable<decimal> LR_WBM_LEN { get; set; }
    //    public Nullable<decimal> LR_GRAVEL_LEN { get; set; }
    //    public Nullable<decimal> LR_TRACK_LEN { get; set; }
    //    public Nullable<decimal> TR_BT_LEN { get; set; }
    //    public Nullable<decimal> TR_WBM_LEN { get; set; }
    //    public Nullable<decimal> TR_GRAVEL_LEN { get; set; }
    //    public Nullable<decimal> TR_TRACK_LEN { get; set; }
    //}

    public partial class USP_CN4_FINAL_REPORT_Result
    {
        public string PLAN_CN_ROAD_NUMBER { get; set; }
        public string PLAN_RD_NAME { get; set; }
        public string PLAN_RD_FROM { get; set; }
        public string PLAN_RD_TO { get; set; }
        public Nullable<decimal> PLAN_RD_LENGTH { get; set; }
        public Nullable<int> PLAN_RD_TO_HAB { get; set; }
        public Nullable<int> TOTAL_POP { get; set; }
    }

    //public partial class USP_CN6_STATE_REPORT_Result
    //{
    //    public int MAST_STATE_CODE { get; set; }
    //    public string MAST_STATE_NAME { get; set; }
    //    public Nullable<int> TOTAL_CN { get; set; }
    //    public Nullable<decimal> TOTAL_LEN { get; set; }
    //    public Nullable<int> TOTAL_HABS { get; set; }
    //    public Nullable<int> TOTAL_POP { get; set; }
    //}

    //public partial class USP_CN6_DISTRICT_REPORT_Result
    //{
    //    public int MAST_DISTRICT_CODE { get; set; }
    //    public string MAST_DISTRICT_NAME { get; set; }
    //    public Nullable<int> TOTAL_CN { get; set; }
    //    public Nullable<decimal> TOTAL_LEN { get; set; }
    //    public Nullable<int> TOTAL_HABS { get; set; }
    //    public Nullable<int> TOTAL_POP { get; set; }
    //}

    //public partial class USP_CN6_BLOCK_REPORT_Result
    //{
    //    public int MAST_BLOCK_CODE { get; set; }
    //    public string MAST_BLOCK_NAME { get; set; }
    //    public Nullable<int> TOTAL_CN { get; set; }
    //    public Nullable<decimal> TOTAL_LEN { get; set; }
    //    public Nullable<int> TOTAL_HABS { get; set; }
    //    public Nullable<int> TOTAL_POP { get; set; }
    //}

    public partial class USP_CN6_FINAL_REPORT_Result
    {
        public string PLAN_CN_ROAD_NUMBER { get; set; }
        public string PLAN_RD_NAME { get; set; }
        public string PLAN_RD_FROM { get; set; }
        public string PLAN_RD_TO { get; set; }
        public Nullable<decimal> PLAN_RD_LENGTH { get; set; }
        public Nullable<int> MAST_HAB_CODE { get; set; }
        public string MAST_HAB_NAME { get; set; }
        public Nullable<int> MAST_HAB_TOT_POP { get; set; }
    }
    //state
    //public partial class USP_CNCPL_REPORT_Result
    //{
    //    public int MAST_STATE_CODE { get; set; }
    //    public string MAST_STATE_NAME { get; set; }
    //    public Nullable<int> TOTAL_CN { get; set; }
    //    public decimal TOTAL_LEN { get; set; }
    //    public Nullable<int> TOTAL_HABS { get; set; }
    //    public int TOTAL_POP { get; set; }
    //}

    //district
    //public partial class USP_CNCPL_DISTRICT_REPORT_Result
    //{
    //    public int MAST_DISTRICT_CODE { get; set; }
    //    public string MAST_DISTRICT_NAME { get; set; }
    //    public Nullable<int> TOTAL_CN { get; set; }
    //    public decimal TOTAL_LEN { get; set; }
    //    public Nullable<int> TOTAL_HABS { get; set; }
    //    public int TOTAL_POP { get; set; }
    //}
    //block
    //public partial class USP_CNCPL_BLOCK_REPORT_Result
    //{
    //    public int MAST_BLOCK_CODE { get; set; }
    //    public string MAST_BLOCK_NAME { get; set; }
    //    public Nullable<int> TOTAL_CN { get; set; }
    //    public decimal TOTAL_LEN { get; set; }
    //    public Nullable<int> TOTAL_HABS { get; set; }
    //    public int TOTAL_POP { get; set; }
    //}
    //final
    public class USP_CNCPL_FINAL_REPORT_Result
    {
        public string PLAN_CN_ROAD_NUMBER { get; set; }
        public string PLAN_RD_NAME { get; set; }
        public string PLAN_RD_ROUTE { get; set; }
        public string PLAN_RD_LENG { get; set; }
        public decimal PLAN_RD_LENGTH { get; set; }
        public string RD_FROM { get; set; }
        public string RD_TO { get; set; }
        public string MAST_HAB_NAME { get; set; }
        public int MAST_HAB_TOT_POP { get; set; }
        public int TOTAL_POP_SERVERD { get; set; }
    }
    ////state
    //public partial class USP_CN_HAB_REPORT_Result
    //{
    //    public int MAST_STATE_CODE { get; set; }
    //    public string MAST_STATE_NAME { get; set; }
    //    public Nullable<int> TOTAL_CN { get; set; }
    //    public decimal TOTAL_LEN { get; set; }
    //    public Nullable<int> TOTAL_HABS { get; set; }
    //    public int TOTAL_POP { get; set; }
    //}
    //district
    //public partial class USP_CN_HAB_DISTRICT_REPORT_Result
    //{
    //    public int MAST_DISTRICT_CODE { get; set; }
    //    public string MAST_DISTRICT_NAME { get; set; }
    //    public Nullable<int> TOTAL_CN { get; set; }
    //    public decimal TOTAL_LEN { get; set; }
    //    public Nullable<int> TOTAL_HABS { get; set; }
    //    public int TOTAL_POP { get; set; }
    //}
    //block
    //public partial class USP_CN_HAB_BLOCK_REPORT_Result
    //{
    //    public int MAST_BLOCK_CODE { get; set; }
    //    public string MAST_BLOCK_NAME { get; set; }
    //    public Nullable<int> TOTAL_CN { get; set; }
    //    public decimal TOTAL_LEN { get; set; }
    //    public Nullable<int> TOTAL_HABS { get; set; }
    //    public int TOTAL_POP { get; set; }
    //}
    //final
    public partial class USP_CN_HAB_FINAL_REPORT_Result
    {
        public string MAST_HAB_NAME { get; set; }
        public string PLAN_CN_ROAD_NUMBER { get; set; }
        public string PLAN_RD_NAME { get; set; }
        public string PLAN_RD_ROUTE { get; set; }
        public decimal PLAN_RD_FROM_CHAINAGE { get; set; }
        public decimal PLAN_RD_TO_CHAINAGE { get; set; }
        public string PLAN_RD_LENG { get; set; }
        public decimal PLAN_RD_LENGTH { get; set; }
        public string RD_FROM { get; set; }
        public string RD_TO { get; set; }
        public int MAST_HAB_TOT_POP { get; set; }
    }

    //state
    //public partial class USP_CN_ROAD_STATE_REPORT
    //{
    //    public int MAST_STATE_CODE { get; set; }
    //    public string MAST_STATE_NAME { get; set; }
    //    public int TOTAL_NH	{ get; set; }
    //    public decimal TOTAL_NH_LEN	{ get; set; }
    //    public int TOTAL_SH	{ get; set; }
    //    public decimal TOTAL_SH_LEN	{ get; set; }
    //    public int TOTAL_MDR	{ get; set; }
    //    public decimal TOTAL_MDR_LEN	{ get; set; }
    //    public int TOTAL_OTHER { get; set; }
    //    public decimal TOTAL_OTHER_LEN { get; set; }
    //}
    //district
    //public partial class USP_CN_ROAD_DISTRICT_REPORT
    //{
    //    public int MAST_DISTRICT_CODE { get; set; }
    //    public string MAST_DISTRICT_NAME { get; set; }
    //    public int TOTAL_NH { get; set; }
    //    public decimal TOTAL_NH_LEN { get; set; }
    //    public int TOTAL_SH { get; set; }
    //    public decimal TOTAL_SH_LEN { get; set; }
    //    public int TOTAL_MDR { get; set; }
    //    public decimal TOTAL_MDR_LEN { get; set; }
    //    public int TOTAL_OTHER { get; set; }
    //    public decimal TOTAL_OTHER_LEN { get; set; }
    //}
    //block
    //public partial class USP_CN_ROAD_BLOCK_REPORT
    //{
    //    public int MAST_BLOCK_CODE { get; set; }
    //    public string MAST_BLOCK_NAME { get; set; }
    //    public int TOTAL_NH { get; set; }
    //    public decimal TOTAL_NH_LEN { get; set; }
    //    public int TOTAL_SH { get; set; }
    //    public decimal TOTAL_SH_LEN { get; set; }
    //    public int TOTAL_MDR { get; set; }
    //    public decimal TOTAL_MDR_LEN { get; set; }
    //    public int TOTAL_OTHER { get; set; }
    //    public decimal TOTAL_OTHER_LEN { get; set; }
    //}
    //final
    public partial class USP_CN_ROAD_FINAL_REPORT
    {
        public string MAST_ER_ROAD_NUMBER { get; set; }
        public string MAST_ER_ROAD_NAME { get; set; }
        public int MAST_ROAD_CAT_CODE { get; set; }
        public string MAST_ROAD_SHORT_DESC { get; set; }
        public string PLAN_CN_ROAD_NUMBER { get; set; }
        public string PLAN_RD_NAME { get; set; }
        public string PLAN_RD_ROUTE { get; set; }
        public decimal PLAN_RD_FROM_CHAINAGE { get; set; }
        public decimal PLAN_RD_TO_CHAINAGE { get; set; }
        public string PLAN_RD_LENG { get; set; }
        public decimal PLAN_RD_LENGTH { get; set; }
        public string RD_FROM { get; set; }
        public string RD_TO { get; set; }
    }


    public partial class USP_CN_PCILVL_REPORT_Result_FINAL_REPORT
    {
        public int MAST_STATE_CODE { get; set; }
        public string MAST_STATE_NAME { get; set; }
        public int MAST_DISTRICT_CODE { get; set; }
        public string MAST_DISTRICT_NAME { get; set; }
        public int MAST_BLOCK_CODE { get; set; }
        public string MAST_BLOCK_NAME { get; set; }
        public int PLAN_CN_ROAD_CODE { get; set; }
        public string PLAN_CN_ROAD_NUMBER { get; set; }
        public string PLAN_RD_NAME { get; set; }
        public string PLAN_RD_ROUTE { get; set; }
        public Nullable<decimal> PLAN_RD_LENGTH { get; set; }
        public int MANE_PCI_YEAR { get; set; }
        public int MANE_SEGMENT_NO { get; set; }
        public decimal MANE_STR_CHAIN { get; set; }
        public decimal MANE_END_CHAIN { get; set; }
        public Nullable<int> MANE_SURFACE_TYPE { get; set; }
        public int MANE_PCIINDEX { get; set; }
        public string MAST_SURFACE_NAME { get; set; }
    }

   
    public partial class USP_CN_CUCPL_FINAL_REPORT
    {
        public string PLAN_CN_ROAD_NUMBER { get; set; }
        public string PLAN_RD_NAME { get; set; }
        public decimal PLAN_RD_LENGTH { get; set; }
        public int MAST_CONS_YEAR { get; set; }
        public int MAST_RENEW_YEAR { get; set; }
        public int MANE_PCI_YEAR { get; set; }
        public int MANE_SEGMENT_NO { get; set; }
        public decimal MANE_STR_CHAIN { get; set; }
        public decimal MANE_END_CHAIN { get; set; }
        public int MANE_PCIINDEX { get; set; }
        public decimal AVG_PCI { get; set; }
        public int POP { get; set; }
        public int MAST_TI_YEAR { get; set; }
        public int MAST_COMM_TI { get; set; }
    }

    public class USP_CN_ROADWISE_REPORT_Result
    {
        public int MAST_STATE_CODE { get; set; }
        public string MAST_STATE_NAME { get; set; }
        public int MAST_DISTRICT_CODE { get; set; }
        public string MAST_DISTRICT_NAME { get; set; }
        public int MAST_BLOCK_CODE { get; set; }
        public string MAST_BLOCK_NAME { get; set; }
        public int MAST_ER_ROAD_CODE { get; set; }
        public string MAST_ER_ROAD_NUMBER { get; set; }
        public string MAST_ER_ROAD_NAME { get; set; }
        public int MAST_ROAD_CAT_CODE { get; set; }
        public string MAST_ROAD_SHORT_DESC { get; set; }
        public int PLAN_CN_ROAD_CODE { get; set; }
        public string PLAN_CN_ROAD_NUMBER { get; set; }
        public string PLAN_RD_NAME { get; set; }
        public string PLAN_RD_ROUTE { get; set; }
        public Nullable<decimal> PLAN_RD_FROM_CHAINAGE { get; set; }
        public Nullable<decimal> PLAN_RD_TO_CHAINAGE { get; set; }
        public string PLAN_RD_LENG { get; set; }
        public Nullable<decimal> PLAN_RD_LENGTH { get; set; }
        public decimal BT_LEN { get; set; }
        public string BT_CONDITION { get; set; }
        public decimal WBM_LEN { get; set; }
        public string WBM_CONDITION { get; set; }
        public decimal GRAVEL_LEN { get; set; }
        public string GRAVEL_CONDITION { get; set; }
        public decimal TRACK_LEN { get; set; }
        public string TARCK_CONDITION { get; set; }
        public string RD_FROM { get; set; }
        public string RD_TO { get; set; }
        // public Nullable<int> MAST_HAB_CODE { get; set; }
        public string MAST_HAB_NAME { get; set; }
        public int MAST_HAB_TOT_POP { get; set; }
        public string MAST_HAB_CONNECTED { get; set; }
        public int TOTAL_HABS { get; set; }
        public Nullable<int> KML_FILE { get; set; }
    }


    public partial class USP_CN6_FinalLevel_REPORT_Result
    {
        public int MAST_STATE_CODE { get; set; }
        public string MAST_STATE_NAME { get; set; }
        public int MAST_DISTRICT_CODE { get; set; }
        public string MAST_DISTRICT_NAME { get; set; }
        public int MAST_BLOCK_CODE { get; set; }
        public string MAST_BLOCK_NAME { get; set; }
        public int MAST_HAB_CODE { get; set; }
        public string MAST_HAB_NAME { get; set; }
        public int MAST_HAB_TOT_POP { get; set; }
        public int PLAN_CN_ROAD_CODE { get; set; }
        public string PLAN_CN_ROAD_NUMBER { get; set; }
        public string PLAN_RD_NAME { get; set; }
        public string PLAN_RD_ROUTE { get; set; }
        public Nullable<decimal> PLAN_RD_FROM_CHAINAGE { get; set; }
        public Nullable<decimal> PLAN_RD_TO_CHAINAGE { get; set; }
        public string PLAN_RD_LENG { get; set; }
        public Nullable<decimal> PLAN_RD_LENGTH { get; set; }
        public string RD_FROM { get; set; }
        public string RD_TO { get; set; }
        public string MAST_HAB_NAME1 { get; set; }
        public int MAST_HAB_TOT_POP1 { get; set; }
        public string MAST_INC_HAB_NAME { get; set; }
        public int MAST_INC_HAB_TOT_POP { get; set; }
    }

    public partial class USP_CNR1_REPORT_Result
    {
        public int MAST_STATE_CODE { get; set; }
        public string MAST_STATE_NAME { get; set; }
        public int MAST_DISTRICT_CODE { get; set; }
        public string MAST_DISTRICT_NAME { get; set; }
        public int MAST_BLOCK_CODE { get; set; }
        public string MAST_BLOCK_NAME { get; set; }
        public int MAST_ER_ROAD_CODE { get; set; }
        public string MAST_ER_ROAD_NUMBER { get; set; }
        public string MAST_ER_ROAD_NAME { get; set; }
        public int MAST_ROAD_CAT_CODE { get; set; }
        public string MAST_ROAD_SHORT_DESC { get; set; }
        public int PLAN_CN_ROAD_CODE { get; set; }
        public string PLAN_CN_ROAD_NUMBER { get; set; }
        public string PLAN_RD_NAME { get; set; }
        public string PLAN_RD_ROUTE { get; set; }
        public Nullable<decimal> PLAN_RD_FROM_CHAINAGE { get; set; }
        public Nullable<decimal> PLAN_RD_TO_CHAINAGE { get; set; }
        public string PLAN_RD_LENG { get; set; }
        public Nullable<decimal> PLAN_RD_LENGTH { get; set; }
        public decimal BT_LEN { get; set; }
        public string BT_CONDITION { get; set; }
        public decimal WBM_LEN { get; set; }
        public string WBM_CONDITION { get; set; }
        public decimal GRAVEL_LEN { get; set; }
        public string GRAVEL_CONDITION { get; set; }
        public decimal TRACK_LEN { get; set; }
        public string TARCK_CONDITION { get; set; }
        public string RD_FROM { get; set; }
        public string RD_TO { get; set; }
        public Nullable<int> MAST_HAB_CODE { get; set; }
        public string MAST_HAB_NAME { get; set; }
        public Nullable<int> TOTAL_HABS { get; set; }
        public int MAST_HAB_TOT_POP { get; set; }
        public string MAST_HAB_CONNECTED { get; set; }
        public Nullable<int> KML_FILE { get; set; }
    }
}