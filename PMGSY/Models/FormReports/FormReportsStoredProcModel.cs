using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Models.FormReports
{
    public class FormReportsStoredProcModel
    {
    }

    #region Form1 
    
    
    //public partial class USP_FORM1_REPORT_Results
    //{
    //    public int MAST_STATE_CODE { get; set; }
    //    public string MAST_STATE_NAME { get; set; }
    //    public Nullable<int> MAST_NIC_STATE_CODE { get; set; }
    //    public string STATE_TYPE { get; set; }
    //    public Nullable<int> TOTAL_DISTRICT { get; set; }
    //    public Nullable<int> TOTAL_BLOCK { get; set; }
    //    public Nullable<int> TOTAL_VILLAGE { get; set; }
    //    public Nullable<int> POP1000 { get; set; }
    //    public Nullable<int> POP999 { get; set; }
    //    public Nullable<int> POP499 { get; set; }
    //    public Nullable<int> POP250 { get; set; }
    //}

    //public partial class USP_FORM1_DISTRICT_REPORT_Results
    //{
    //    public int MAST_DISTRICT_CODE { get; set; }
    //    public string MAST_DISTRICT_NAME { get; set; }
    //    public Nullable<int> MAST_NIC_DISTRICT_CODE { get; set; }
    //    public string IAP_DISTRICT { get; set; }
    //    public Nullable<int> TOTAL_BLOCK { get; set; }
    //    public Nullable<int> TOTAL_VILLAGE { get; set; }
    //    public Nullable<int> POP1000 { get; set; }
    //    public Nullable<int> POP999 { get; set; }
    //    public Nullable<int> POP499 { get; set; }
    //    public Nullable<int> POP250 { get; set; }
    //}

    //public partial class USP_FORM1_BLOCK_REPORT_Results
    //{
    //    public int MAST_BLOCK_CODE { get; set; }
    //    public string MAST_BLOCK_NAME { get; set; }
    //    public Nullable<int> MAST_NIC_BLOCK_CODE { get; set; }
    //    public string IS_DESERT { get; set; }
    //    public string IS_TRIBAL { get; set; }
    //    public Nullable<int> TOTAL_VILLAGE { get; set; }
    //    public Nullable<int> POP1000 { get; set; }
    //    public Nullable<int> POP999 { get; set; }
    //    public Nullable<int> POP499 { get; set; }
    //    public Nullable<int> POP250 { get; set; }
    //}

    //public partial class USP_FORM1_VILLAGE_REPORT_Results
    //{
    //    public int MAST_VILLAGE_CODE { get; set; }
    //    public string MAST_VILLAGE_NAME { get; set; }
    //    public int MAST_HAB_CODE { get; set; }
    //    public string MAST_HAB_NAME { get; set; }
    //    public string IS_SCHEDULE5 { get; set; }
    //    public Nullable<int> MAST_HAB_TOT_POP { get; set; }
    //    public string HAB_CONNECTED { get; set; }
    //}

    #endregion


    #region Form2
   
    public partial class USP_FORM2_REPORT_Results
    {
        public int MAST_STATE_CODE { get; set; }
        public string MAST_STATE_NAME { get; set; }
        public Nullable<int> TOTAL_MP_CONST { get; set; }
        public Nullable<int> TOTAL_MLA_CONST { get; set; }
    }


    public partial class USP_FORM2_DISTRICT_REPORT_Results
    {
        public int CONST_CODE { get; set; }
        public string CONST_NAME { get; set; }
        public Nullable<int> TOTAL_DISTRICT { get; set; }
        public Nullable<int> TOTAL_BLOCK { get; set; }
    }


    public partial class USP_FORM2_CONSTITUENCY_REPORT_Results
    {
        public int CONST_CODE { get; set; }
        public string CONST_NAME { get; set; }
        public int MAST_DISTRICT_CODE { get; set; }
        public string MAST_DISTRICT_NAME { get; set; }
        public int MAST_BLOCK_CODE { get; set; }
        public string MAST_BLOCK_NAME { get; set; }
    }

    #endregion


    #region Form3
   
    //public partial class USP_FORM3_REPORT_Results
    //{
    //    public int MAST_STATE_CODE { get; set; }
    //    public string MAST_STATE_NAME { get; set; }
    //    public Nullable<int> TPOP1000 { get; set; }
    //    public Nullable<int> TPOP999 { get; set; }
    //    public Nullable<int> TPOP499 { get; set; }
    //    public Nullable<int> TPOP250 { get; set; }
    //    public Nullable<int> UPOP1000 { get; set; }
    //    public Nullable<int> UPOP999 { get; set; }
    //    public Nullable<int> UPOP499 { get; set; }
    //    public Nullable<int> UPOP250 { get; set; }
    //    public Nullable<int> BPOP1000 { get; set; }
    //    public Nullable<int> BPOP999 { get; set; }
    //    public Nullable<int> BPOP499 { get; set; }
    //    public Nullable<int> BPOP250 { get; set; }
    //    public Nullable<int> POP1000 { get; set; }
    //    public Nullable<int> POP999 { get; set; }
    //    public Nullable<int> POP499 { get; set; }
    //    public Nullable<int> POP250 { get; set; }
    //}


    //public partial class USP_FORM3_DISTRICT_REPORT_Results
    //{
    //    public int MAST_DISTRICT_CODE { get; set; }
    //    public string MAST_DISTRICT_NAME { get; set; }
    //    public Nullable<int> TPOP1000 { get; set; }
    //    public Nullable<int> TPOP999 { get; set; }
    //    public Nullable<int> TPOP499 { get; set; }
    //    public Nullable<int> TPOP250 { get; set; }
    //    public Nullable<int> UPOP1000 { get; set; }
    //    public Nullable<int> UPOP999 { get; set; }
    //    public Nullable<int> UPOP499 { get; set; }
    //    public Nullable<int> UPOP250 { get; set; }
    //    public Nullable<int> BPOP1000 { get; set; }
    //    public Nullable<int> BPOP999 { get; set; }
    //    public Nullable<int> BPOP499 { get; set; }
    //    public Nullable<int> BPOP250 { get; set; }
    //    public Nullable<int> POP1000 { get; set; }
    //    public Nullable<int> POP999 { get; set; }
    //    public Nullable<int> POP499 { get; set; }
    //    public Nullable<int> POP250 { get; set; }
    //}


    //public partial class USP_FORM3_BLOCK_REPORT_Results
    //{
    //    public int MAST_BLOCK_CODE { get; set; }
    //    public string MAST_BLOCK_NAME { get; set; }
    //    public Nullable<int> TPOP1000 { get; set; }
    //    public Nullable<int> TPOP999 { get; set; }
    //    public Nullable<int> TPOP499 { get; set; }
    //    public Nullable<int> TPOP250 { get; set; }
    //    public Nullable<int> UPOP1000 { get; set; }
    //    public Nullable<int> UPOP999 { get; set; }
    //    public Nullable<int> UPOP499 { get; set; }
    //    public Nullable<int> UPOP250 { get; set; }
    //    public Nullable<int> BPOP1000 { get; set; }
    //    public Nullable<int> BPOP999 { get; set; }
    //    public Nullable<int> BPOP499 { get; set; }
    //    public Nullable<int> BPOP250 { get; set; }
    //    public Nullable<int> POP1000 { get; set; }
    //    public Nullable<int> POP999 { get; set; }
    //    public Nullable<int> POP499 { get; set; }
    //    public Nullable<int> POP250 { get; set; }
    //}

    #endregion


    #region Form4

    //public partial class USP_FORM4_REPORT_Results
    //{
    //    public int MAST_STATE_CODE { get; set; }
    //    public string MAST_STATE_NAME { get; set; }
    //    public Nullable<int> TOTAL_PROPOSAL { get; set; }
    //    public decimal ROAD_LENGTH { get; set; }
    //    public decimal BRIDGE_LENGTH { get; set; }
    //    public decimal ROAD_AMOUNT { get; set; }
    //    public decimal BRIDGE_AMOUNT { get; set; }
    //    public decimal STATE_RD_AMOUNT { get; set; }
    //    public decimal STATE_BR_AMOUNT { get; set; }
    //    public decimal MAINT_AMOUNT { get; set; }
    //    public Nullable<int> POP1000 { get; set; }
    //    public Nullable<int> POP999 { get; set; }
    //    public Nullable<int> POP499 { get; set; }
    //    public Nullable<int> POP250 { get; set; }
    //}

    

    //public partial class USP_FORM4_DISTRICT_REPORT_Results
    //{
    //    public int MAST_DISTRICT_CODE { get; set; }
    //    public string MAST_DISTRICT_NAME { get; set; }
    //    public Nullable<int> TOTAL_PROPOSAL { get; set; }
    //    public decimal ROAD_LENGTH { get; set; }
    //    public decimal BRIDGE_LENGTH { get; set; }
    //    public decimal ROAD_AMOUNT { get; set; }
    //    public decimal BRIDGE_AMOUNT { get; set; }
    //    public decimal STATE_RD_AMOUNT { get; set; }
    //    public decimal STATE_BR_AMOUNT { get; set; }
    //    public decimal MAINT_AMOUNT { get; set; }
    //    public Nullable<int> POP1000 { get; set; }
    //    public Nullable<int> POP999 { get; set; }
    //    public Nullable<int> POP499 { get; set; }
    //    public Nullable<int> POP250 { get; set; }
    //}



    public partial class USP_FORM4_BLOCK_REPORT_Results
    {
        public int LOCATION_CODE { get; set; }
        public string LOCATION_NAME { get; set; }
        public string PLAN_CN_ROAD_CODE { get; set; }
        public int IMS_PR_ROAD_CODE { get; set; }
        public string IMS_ROAD_NAME { get; set; }
        public string BRIDGE_NAME { get; set; }
        public decimal CARRIAGE_WIDTH { get; set; }
        public string IS_STAGED { get; set; }
        public Nullable<int> IMS_NO_OF_CDWORKS { get; set; }
        public string UPGRADE_CONNECT { get; set; }
        public decimal ROAD_LENGTH { get; set; }
        public decimal BRIDGE_LENGTH { get; set; }
        public decimal ROAD_AMOUNT { get; set; }
        public decimal BRIDGE_AMOUNT { get; set; }
        public decimal STATE_RD_AMOUNT { get; set; }
        public decimal STATE_BR_AMOUNT { get; set; }
        public decimal MAINT_AMOUNT { get; set; }
        public Nullable<int> POP1000 { get; set; }
        public Nullable<int> POP999 { get; set; }
        public Nullable<int> POP499 { get; set; }
        public Nullable<int> POP250 { get; set; }
    }

    #endregion


    #region Form5/6

    public partial class USP_FORM5_REPORT_Results
    {
        public int MAST_STATE_CODE { get; set; }
        public string MAST_STATE_NAME { get; set; }
        public Nullable<int> MLA { get; set; }
        public Nullable<int> MP { get; set; }
        public Nullable<int> MLA_CN { get; set; }
        public Nullable<int> MP_CN { get; set; }
        public Nullable<int> MLA_PR { get; set; }
        public Nullable<int> MP_PR { get; set; }
    }


    public partial class USP_FORM5_DISTRICT_REPORT_Results
    {
        public int CONST_CODE { get; set; }
        public string CONST_NAME { get; set; }
        public Nullable<int> PROP { get; set; }
        public Nullable<int> CN { get; set; }
        public Nullable<int> PR { get; set; }
    }

    
    public partial class USP_FORM5_CONSTITUENCY_REPORT_Results
    {
        public int CONST_CODE { get; set; }
        public string CONST_NAME { get; set; }
        public Nullable<int> IMS_ROAD_ID { get; set; }
        public string IMS_ROAD_DETAILS { get; set; }
        public string INCLUDED_IN_CN { get; set; }
        public string IMS_REASON_ID_1 { get; set; }
        public string PLAN_CN_ROAD_CODE { get; set; }
        public string INCLUDED_IN_PR { get; set; }
        public string IMS_REASON_ID_2 { get; set; }
        public string IMS_PR_ROAD_CODE { get; set; }
    }

    #endregion


    #region Form7

    //public partial class USP_FORM7_REPORT_Results
    //{
    //    public int MAST_STATE_CODE { get; set; }
    //    public string MAST_STATE_NAME { get; set; }
    //    public int ROAD_PROPOSAL { get; set; }
    //    public decimal ROAD_COST { get; set; }
    //    public int BRIDGE_PROPOSAL { get; set; }
    //    public decimal BRIDGE_COST { get; set; }
    //    public int ROAD_AWARD { get; set; }
    //    public int BRIDGE_AWARD { get; set; }
    //    public decimal ROAD_AWARDED_AMOUNT { get; set; }
    //    public decimal BRIDGE_AWARDED_AMOUNT { get; set; }
    //}


    //public partial class USP_FORM7_DISTRICT_REPORT_Results
    //{
    //    public int MAST_DISTRICT_CODE { get; set; }
    //    public string MAST_DISTRICT_NAME { get; set; }
    //    public int ROAD_PROPOSAL { get; set; }
    //    public decimal ROAD_COST { get; set; }
    //    public int BRIDGE_PROPOSAL { get; set; }
    //    public decimal BRIDGE_COST { get; set; }
    //    public int ROAD_AWARD { get; set; }
    //    public int BRIDGE_AWARD { get; set; }
    //    public decimal ROAD_AWARDED_AMOUNT { get; set; }
    //    public decimal BRIDGE_AWARDED_AMOUNT { get; set; }
    //}


    public partial class USP_FORM7_BLOCK_REPORT_Results
    {
        //public int MAST_BLOCK_CODE { get; set; }
        //public string MAST_BLOCK_NAME { get; set; }
        public int LOCATION_CODE { get; set; }
        public string LOCATION_NAME { get; set; }
        public string PLAN_CN_ROAD_CODE { get; set; }
        public int IMS_PR_ROAD_CODE { get; set; }
        public string IMS_ROAD_NAME { get; set; }
        public int IMS_YEAR { get; set; }
        public string IMS_SANCTIONED_DATE { get; set; }
        public string BRIDGE_NAME { get; set; }
        public decimal ROAD_LENGTH { get; set; }
        public decimal BRIDGE_LENGTH { get; set; }
        public decimal ROAD_AMOUNT { get; set; }
        public decimal BRIDGE_AMOUNT { get; set; }
        public decimal STATE_RD_AMOUNT { get; set; }
        public decimal STATE_BR_AMOUNT { get; set; }
        public decimal MAINT_AMOUNT { get; set; }
        public decimal AGREEMENT_AMOUNT { get; set; }
        public decimal AGREEMENT_MAINT_AMOUNT { get; set; }
        public string CONTRACTOR_NAME { get; set; }
        public string TEND_AGREEMENT_NUMBER { get; set; }
        public string TEND_DATE_OF_AGREEMENT { get; set; }
        public string TEND_AGREEMENT_START_DATE { get; set; }
        public string TEND_AGREEMENT_END_DATE { get; set; }
        public string TEND_DATE_OF_COMMENCEMENT { get; set; }
        public string TEND_DATE_OF_COMPLETION { get; set; }
        public string TEND_DATE_OF_AWARD_WORK { get; set; }
        public string TEND_DATE_OF_WORK_ORDER { get; set; }
    }


    #endregion

     
    #region Form8

    public partial class USP_FORM8_REPORT_Results
    {
        public int MAST_STATE_CODE { get; set; }
        public string MAST_STATE_NAME { get; set; }
        public int TOTAL_PROPOSAL { get; set; }
        public int TOTAL_AWARD { get; set; }
        public int TOTAL_COMPLETED_PROPOSAL { get; set; }
        public decimal TOTAL_LENGTH_COMPLETED { get; set; }
        public int TOTAL_PAYMENT_PROPOSAL { get; set; }
        public decimal TOTAL_PHY_LENGTH { get; set; }
        public decimal TOTAL_EXP { get; set; }
    }


    public partial class USP_FORM8_DISTRICT_REPORT_Results
    {
        public int MAST_DISTRICT_CODE { get; set; }
        public string MAST_DISTRICT_NAME { get; set; }
        public int TOTAL_PROPOSAL { get; set; }
        public int TOTAL_AWARD { get; set; }
        public int TOTAL_COMPLETED_PROPOSAL { get; set; }
        public decimal TOTAL_LENGTH_COMPLETED { get; set; }
        public int TOTAL_PAYMENT_PROPOSAL { get; set; }
        public decimal TOTAL_PHY_LENGTH { get; set; }
        public decimal TOTAL_EXP { get; set; }
    }


    public partial class USP_FORM8_BLOCK_REPORT_Results
    {
        public int MAST_BLOCK_CODE { get; set; }
        public string MAST_BLOCK_NAME { get; set; }
        public string PLAN_CN_ROAD_CODE { get; set; }
        public int IMS_PR_ROAD_CODE { get; set; }
        public string IMS_ROAD_NAME { get; set; }
        public int IMS_YEAR { get; set; }
        public string BRIDGE_NAME { get; set; }
        public Nullable<decimal> EXEC_COMPLETED { get; set; }
        public string EXEC_ISCOMPLETED { get; set; }
        public string EXEC_COMPLETION_DATE { get; set; }
        public Nullable<decimal> EXEC_PAYMENT_LASTMONTH { get; set; }
        public Nullable<decimal> EXEC_PAYMENT_THISMONTH { get; set; }
        public Nullable<decimal> EXEC_VALUEOFWORK_LASTMONTH { get; set; }
        public Nullable<decimal> EXEC_VALUEOFWORK_THISMONTH { get; set; }
        public string EXEC_FINAL_PAYMENT_FLAG { get; set; }
        public string EXEC_FINAL_PAYMENT_DATE { get; set; }
    }

     #endregion
}