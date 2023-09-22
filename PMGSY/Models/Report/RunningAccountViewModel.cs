using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Report
{
    public class RunningAccountViewModel
    {

        public RunningAccountViewModel() 
        {
            ddlMonth = new List<SelectListItem>();
            ddlYear = new List<SelectListItem>();
            ddlDPIU = new List<SelectListItem>();
        }

        [Display(Name = "Month")]
        [Required(ErrorMessage = "Month is required.")]
        [Range(1, 12, ErrorMessage = "Please select Month.")]
        public int Month { get; set; }

        [Display(Name = "Year")]
        [Required(ErrorMessage = "Year is required.")]
        [Range(2000, 2099, ErrorMessage = "Please select Year.")]
        public int Year { get; set; }

        [Display(Name = "Balance Type")]
        [Required(ErrorMessage = "Balance Type is required.")]
        [RegularExpression(@"^([CD]+)$", ErrorMessage = "Please select valid Balance Type")]
        public string Balance { get; set; }

        [Display(Name = "Report Type")]
        //[Required(ErrorMessage = "Report Type is required.")]
        [RegularExpression(@"^([SD]+)$", ErrorMessage = "Please select valid Report Type")]
        public string ReportType { get; set; }

        public List<SelectListItem> ddlMonth { get; set; }

        public List<SelectListItem> ddlYear { get; set; }

        public List<SelectListItem> ddlDPIU { get; set; }

        public List<SelectListItem> ddlBalance
        {
            get
            {
                List<SelectListItem> lstBalances = new List<SelectListItem>();
                lstBalances.Add(new SelectListItem { Value = "0",Text = "Select Balance Type"});
                lstBalances.Add(new SelectListItem { Value = "C", Text = "Credit" });
                lstBalances.Add(new SelectListItem { Value = "D", Text = "Debit" });
                return lstBalances;
            }
        }

        public int? page;
        public int? rows;
        public string sord;
        public string sidx;

        public string ReportFormNumber { get; set; }

        public string FundTypeName { get; set; }

        public string ReportName { get; set; }

        public string ReportParagraphName { get; set; }

        public string StateName { get; set; }

        public string NodalAgency { get; set; }

        public string SRRDADPIU { get; set; }

        public string BalanceName { get; set; }

        public int DPIUCode { get; set; }

        public string MonthName { get; set; }

        public int AdminCode { get; set; }

        public string DPIUName { get; set; }

        public string PreviousMonthName { get; set; }

    }

    public class RunningAccountList
    {


        public short HEAD_ID;

        public string HEAD_CODE;

        public string HEAD_NAME;

        public string HEAD_COMP_PROGRESS;

        public string IMS_COLLABORATION;

        public decimal? OB_BALANCE_AMT;

        public decimal MONTHLY_BALANCE_AMT;
    }

}