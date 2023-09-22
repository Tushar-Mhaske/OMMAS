using PMGSY.Models.AccountsReports;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.PFMSReports.Models
{
    public class PfmsMISDashboardModel
    {
        [Display(Name = "SRRDA")]
        public int SRRDA { get; set; }

        public string SRRDA_DPIU { get; set; }

        [Display(Name = "DPIU")]
        public int DPIU { get; set; }

        [Display(Name = "NRRDA")]
        public string NRRDA { get; set; }

        public short Year { get; set; }

        [Display(Name = "Periodic")]
        public string Period { get; set; }

        public string FundType { get; set; }

        [Display(Name = "Start Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Start Date is not in valid format")]
        public string StartDate { get; set; }

        [Display(Name = "End Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "End Date is not in valid format")]
        [AssetDateValidation("StartDate", ErrorMessage = "End Date must be greater than or equal to start date.")]
        [AssetCurrentDateValidation("StartDate", ErrorMessage = "End date must be less than or equals to todays date.")]
        public string EndDate { get; set; }

        public string rType { get; set; }

        public int AdminNdCode { get; set; }

        public string MonthName { get; set; }
        public string DPIUName { get; set; }

        public int levelId { get; set; }

        public string NodalAgency { get; set; }
        public string DPIUByNO { get; set; }

        public int TotalRecords { get; set; }

        public string DPIUBySRRDA { get; set; }

        public List<SelectListItem> ddlDPIU { get; set; }

        public List<SelectListItem> States { get; set; }

        public List<SelectListItem> MonthList { get; set; }
        public List<SelectListItem> YearList { get; set; }

        public int isAllPiu { get; set; }
        public string YearName { get; set; }
        public string StateName { get; set; }
        public string Selection { get; set; }
    }
    
}