using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Dashboard
{
    public class NRRDADashboardFilterViewModel
    {
        public NRRDADashboardFilterViewModel()
        {
            lstStates = new List<SelectListItem>();
            lstYears = new List<SelectListItem>();
            lstAgency = new List<SelectListItem>();
            lstCollaborations = new List<SelectListItem>();
        }
        
        [Display(Name="Select State")]
        [Required]
        [Range(1,Int32.MaxValue,ErrorMessage="Please select State.")]
        public int State { get; set; }

        [Display(Name = "Select Financial Year")]
        [Required]
        [Range(2000,Int32.MaxValue,ErrorMessage="Please select Year.")]
        public int Year { get; set; }

        [Display(Name="Collaboration")]
        [Required]
        [Range(-1,Int32.MaxValue,ErrorMessage="Please select Collaboration.")]
        public int Collaboration { get; set; }

        [Required]
        [Display(Name = "Agency")]
        [Range(0, Int32.MaxValue,ErrorMessage="Please select Agency.")]
        public int Agency { get; set; }


        public List<SelectListItem> lstStates { get; set; }
        public List<SelectListItem> lstCollaborations { get;set; }
        public List<SelectListItem> lstAgency { get; set; }
        public List<SelectListItem> lstYears { get; set; }

    }

    public class ProposalTrendsArrayViewModel
    {
        public string MAST_YEAR_TEXT { get; set; }
        public List<ProposalTrendsViewModel> lstTrendDetails { get; set; }
    }

    public class ProposalTrendsViewModel
    {
        //public string MAST_YEAR_TEXT { get; set; }
        public Dictionary<string, string> ARR_JSON { get; set; }

        //public string LOCATION_NAME { get; set; }
        //public Nullable<decimal> ROAD_LEN { get; set; }
        //public Nullable<decimal> LSB_LEN { get; set; }
    }

    public class USP_PROP_HAB_PPT_LIST_Result
    {
        public int LOCATION_CODE { get; set; }
        public string LOCATION_NAME { get; set; }
        public int TOTAL_HAB_CLR_NC { get; set; }
        public int TOTAL_HAB_CLR { get; set; }
        public int TOTAL_HABS { get; set; }
        public int TOTAL_HAB_CN { get; set; }
        public int TOTAL_BAL_HA { get; set; }
    }

    public class USP_PROP_MAINT_PPT_LIST_Result
    {
        public int LOCATION_CODE { get; set; }
        public string LOCATION_NAME { get; set; }
        public decimal? FR_Year1 { get; set; }
        public decimal? FR_Year2 { get; set; }
        public decimal? FR_Year3 { get; set; }
        public decimal? FR_Year4 { get; set; }
        public decimal? FC_Year1 { get; set; }
        public decimal? FC_Year2 { get; set; }
        public decimal? FC_Year3 { get; set; }
        public decimal? FC_Year4 { get; set; }
        public decimal? FS_Year1 { get; set; }
        public decimal? FS_Year2 { get; set; }
        public decimal? FS_Year3 { get; set; }
        public decimal? FS_Year4 { get; set; }
        public decimal? FP_Year1 { get; set; }
        public decimal? FP_Year2 { get; set; }
        public decimal? FP_Year3 { get; set; }
        public decimal? FP_Year4 { get; set; }
    }

    public class USP_TECH_TRENDS_CHART_PPT_Result
    {

        public int MAST_YEAR_CODE { get; set; }
        public string MAST_YEAR_TEXT { get; set; }
        public int LOCATION_CODE { get; set; }
        public string LOCATION_NAME { get; set; }
        public decimal? ROAD_LEN { get; set; }
        public decimal? LSB_LEN { get; set; }
    }


}
