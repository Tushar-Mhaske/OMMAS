using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Dashboard
{

    public class WBankDashboardLayoutViewModel
    {
        [Display(Name = "Fund Type")]
        public Nullable<int> FUND_TYPE { get; set; }
        public List<SelectListItem> FUND_TYPE_LIST { get; set; }

        [Display(Name = "Funding Agency")]
        public Nullable<int> IMS_COLLABORATION { get; set; }
        public List<SelectListItem> COLLABORATIONS_LIST { get; set; }

        public Nullable<int> STATE_ND_CODE { get; set; }
        public Nullable<int> PIU_ND_CODE { get; set; }

        public Nullable<int> MAST_STATE_CODE_PHYSICAL { get; set; }
        public Nullable<int> MAST_DISTRICT_CODE_PHYSICAL { get; set; }
        public Nullable<int> MAST_STATE_CODE_QUALITY { get; set; }
        public Nullable<int> MAST_DISTRICT_CODE_QUALITY { get; set; }

        public string PROPOSAL_TYPE { get; set; }

        public string QM_TYPE { get; set; }
        public List<SelectListItem> QM_TYPE_LIST { get; set; }
        //public string PHYSICAL_CHART_TITLE { get; set; }
    }


    public class WBankDashboardFiltersViewModel
    {
        [Display(Name = "SRRDA")]
        public Nullable<int> STATE_ND_CODE { get; set; }
        public List<SelectListItem> STATE_DEPT_LIST { get; set; }

        [Display(Name = "District")]
        public Nullable<int> MAST_DISTRICT_CODE { get; set; }
        public List<SelectListItem> DISTRICTS_LIST { get; set; }

        [Display(Name = "PIU")]
        public int ADMIN_ND_CODE { get; set; }
        public List<SelectListItem> DPIU_LIST { get; set; }

        [Display(Name = "Funding Agency")]
        public Nullable<int> IMS_COLLABORATION { get; set; }
        public List<SelectListItem> COLLABORATIONS_LIST { get; set; }

        [Display(Name = "Duration Type")]
        public int DURATION_TYPE { get; set; }
        public List<SelectListItem> DURATION_TYPE_LIST { set; get; }

        [Display(Name = "Financial Year")]
        public int YEAR { get; set; }
        public List<SelectListItem> YEARS_LIST { set; get; }
        
        [Display(Name = "Duration")]
        public int DURATION { get; set; }
        public List<SelectListItem> DURATION_LIST { set; get; }
    }


    public class YearwiseCumExpnChartModel
    {
        public string Name { get; set; }
        public string MYear { get; set; }
        public string YearlyExpn { get; set; }
        public string CumExpn { get; set; }
    }

    public partial class USP_TECH_DASH_DISTRICT_REPORT_Result
    {
        public int MAST_DISTRICT_CODE { get; set; }
        public string MAST_DISTRICT_NAME { get; set; }
        public Nullable<int> TOTAL_WORKS { get; set; }
        public Nullable<decimal> TOTAL_LENGTH { get; set; }
        public Nullable<decimal> WORK_TOTAL { get; set; }
        public Nullable<decimal> MAINT_TOTAL { get; set; }
        public Nullable<int> AWARDED_WORKS { get; set; }
        public Nullable<decimal> AWARDED_LEN { get; set; }
        public Nullable<decimal> AWARDED_AMOUNT { get; set; }
        public Nullable<decimal> AWARDED_MAINT_AMOUNT { get; set; }
        public Nullable<int> COMP_WORKS { get; set; }
        public Nullable<decimal> COMP_LEN { get; set; }
        public Nullable<decimal> TOTAL_EXP { get; set; }
        public Nullable<int> PROG_WORKS { get; set; }
        public Nullable<decimal> PROG_LEN { get; set; }
        public Nullable<decimal> PROG_EXP { get; set; }
        public Nullable<decimal> MAINT_EXP { get; set; }
    }


    public partial class USP_TECH_DASH_BLOCK_REPORT_Result
    {
        public int MAST_BLOCK_CODE { get; set; }
        public string MAST_BLOCK_NAME { get; set; }
        public Nullable<int> TOTAL_WORKS { get; set; }
        public Nullable<decimal> TOTAL_LENGTH { get; set; }
        public Nullable<decimal> WORK_TOTAL { get; set; }
        public Nullable<decimal> MAINT_TOTAL { get; set; }
        public Nullable<int> AWARDED_WORKS { get; set; }
        public Nullable<decimal> AWARDED_LEN { get; set; }
        public Nullable<decimal> AWARDED_AMOUNT { get; set; }
        public Nullable<decimal> AWARDED_MAINT_AMOUNT { get; set; }
        public Nullable<int> COMP_WORKS { get; set; }
        public Nullable<decimal> COMP_LEN { get; set; }
        public Nullable<decimal> TOTAL_EXP { get; set; }
        public Nullable<int> PROG_WORKS { get; set; }
        public Nullable<decimal> PROG_LEN { get; set; }
        public Nullable<decimal> PROG_EXP { get; set; }
        public Nullable<decimal> MAINT_EXP { get; set; }
    }

    //public class WorksColumnChartModel
    //{
    //    public string Year { get; set; }
    //    public string SanctionWorks { get; set; }
    //    public string AwardedWorks { get; set; }
    //    public string CompletedWorks { get; set; }
    //    public string ProgWorks { get; set; }
    //}

    public class LengthColumnChartStoredProcModel
    {
        public int IMS_YEAR { get; set; }
        public int LOCATION_CODE { get; set; }
        public decimal ROAD_LEN { get; set; }
        public decimal LSB_LEN { get; set; }
        public decimal AWARD_RD_LEN { get; set; }
        public decimal AWARD_LSB_LEN { get; set; }
        public decimal COMP_RD_LEN { get; set; }
        public decimal PROG_RD_LEN { get; set; }
        public decimal COMP_LSB_LEN { get; set; }
        public decimal PROG_LSB_LEN { get; set; }
    }

    //public class LengthColumnChartModel
    //{
    //    public string Year { get; set; }
        
    //    public string SanctionRdLength { get; set; }
    //    public string AwardedRdLength { get; set; }
    //    public string CompletedRdLength { get; set; }
    //    public string ProgRdLength { get; set; }

    //    public string SanctionLSBLength { get; set; }
    //    public string AwardedLSBLength { get; set; }
    //    public string CompletedLSBLength { get; set; }
    //    public string ProgLSBLength { get; set; }
    //}

    public class CostColumnChartStoredProcModel
    {
        public int IMS_YEAR { get; set; }
        public int LOCATION_CODE { get; set; }
        public decimal WORK_TOTAL { get; set; }
        public decimal MAINT_TOTAL { get; set; }
        public decimal AWARD_WORK_AMT { get; set; }
        public decimal AWARD_MAINT_AMT { get; set; }
        public decimal WORK_EXP { get; set; }
    }


    //public class CostColumnChartModel
    //{
    //    public string Year { get; set; }

    //    public string SanctionTotal { get; set; }
    //    public string AwardedTotal { get; set; }
    //    public string Exp { get; set; }
    //}


    public class USP_QM_DISTRICT_DASH_S1_Result
    {
        public int MAST_DISTRICT_CODE { get; set; }
        public string MAST_DISTRICT_NAME { get; set; }
        public Nullable<int> I_TOTAL { get; set; }
        public Nullable<int> S_TOTAL { get; set; }
        public Nullable<int> I_GRADE_1_PER { get; set; }
        public Nullable<int> I_GRADE_2_PER { get; set; }
        public Nullable<int> I_GRADE_3_PER { get; set; }
        public Nullable<int> S_GRADE_1_PER { get; set; }
        public Nullable<int> S_GRADE_2_PER { get; set; }
        public Nullable<int> S_GRADE_3_PER { get; set; }
    }

    public class USP_QM_STATE_DISTRICT_DASH_S1_Result
    {
        public int INSP_YEAR { get; set; }
        public int MAST_STATE_CODE { get; set; }
        public string MAST_STATE_NAME { get; set; }
        public int MAST_DISTRICT_CODE { get; set; }
        public string MAST_DISTRICT_NAME { get; set; }
        public Nullable<int> I_TOTAL { get; set; }
        public Nullable<int> S_TOTAL { get; set; }
        public Nullable<int> I_GRADE_1_PER { get; set; }
        public Nullable<int> I_GRADE_2_PER { get; set; }
        public Nullable<int> I_GRADE_3_PER { get; set; }
        public Nullable<int> S_GRADE_1_PER { get; set; }
        public Nullable<int> S_GRADE_2_PER { get; set; }
        public Nullable<int> S_GRADE_3_PER { get; set; }
    }


    public class AllStatesQualityPieChartModel
    {
        public string Name { get; set; }
        public decimal Value { get; set; }
    }

    public class YearlyGradingLineChartModel
    {
        public string Year { get; set; }
        public decimal SPercent { get; set; }
        public decimal SRIPercent { get; set; }
        public decimal UPercent { get; set; }
    }


    public class USP_QM_STATE_DISTRICT_DASH_S3_Result
    {
        public int MAST_STATE_CODE { get; set; }
        public string MAST_STATE_NAME { get; set; }
        public int MAST_DISTRICT_CODE { get; set; }
        public string MAST_DISTRICT_NAME { get; set; }
        public Nullable<int> I_TOTAL { get; set; }
        public Nullable<int> S_TOTAL { get; set; }
        public Nullable<int> I_GRADE_1_PER { get; set; }
        public Nullable<int> I_GRADE_2_PER { get; set; }
        public Nullable<int> I_GRADE_3_PER { get; set; }
        public Nullable<int> S_GRADE_1_PER { get; set; }
        public Nullable<int> S_GRADE_2_PER { get; set; }
        public Nullable<int> S_GRADE_3_PER { get; set; }
    }


    public class MonitorsStatewiseGradingColumnChartModel
    {
        public string Name { get; set; }
        public decimal SPercent { get; set; }
        public decimal SRIPercent { get; set; }
        public decimal UPercent { get; set; }
    }


    public class WorkLengthExpYearWiseColumnChartModel
    {
        //public int LOCATION_CODE { get; set; }
        //public string LOCATION_NAME { get; set; }
        public string IMS_YEAR { get; set; }
        public Nullable<int> PROPOSALS { get; set; }
        public Nullable<decimal> LENGTH_COMPLETED { get; set; }
        public Nullable<decimal> EXPENDITURE { get; set; }    
    }
}