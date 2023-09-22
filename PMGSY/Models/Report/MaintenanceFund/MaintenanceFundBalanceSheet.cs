using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace PMGSY.Models.Report.MaintenanceFund
{
    public class MaintenanceFundBalanceSheet
    {
        public MaintenanceFundBalanceSheet()
        {
            this.YEAR =Convert.ToInt16(DateTime.Now.Year);
            this.MONTH = Convert.ToInt16(DateTime.Now.Month);
        }

        [Display(Name = "Month")]
        [Range(1, Int16.MaxValue, ErrorMessage = "please select Month")]
        public short MONTH { get; set; }
        public List<SelectListItem> YEAR_LIST { get; set; }

        [Display(Name = "Year")]
        [Range(1, Int16.MaxValue, ErrorMessage = "please select Year")]
        public short YEAR { get; set; }
        public List<SelectListItem> MONTH_LIST { get; set; }
 

        public bool IsSRRDA { get; set; }

        //[Required(ErrorMessage = "Credit / Debit required ")]
        //[MaxLength(1, ErrorMessage = "invalid Credit/debit")]
        //public string CREDIT_DEBIT { get; set; }
    }

    public class MaintenanceFundBalanceSheetList
    {
        public List<USP_RPT_SHOW_BALSHEET_Result> ListBalanceSheet { get; set; }
        public string MONTH { get; set; }
        public string Year { get; set; }
        public string Type { get; set; }
    }
}