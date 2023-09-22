using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PMGSY.Models.AccountsReports
{
    public class AbstractBankAuthViewModel
    {
        //const int currentYear =System.DateTime.Now.Year;

        [Display(Name="Year")]
        [Range(2000, int.MaxValue, ErrorMessage = "Please select Year")]
        public int Year { get; set; }

        [Display(Name="State")]
        [Range(1, int.MaxValue , ErrorMessage = "Please select State")]
        public int SRRDA { get; set; }

        [Display(Name="DPIU")]
        public int DPIU { get; set; }

        public string DisplayYear { get; set; }

        public string DisplayState { get; set; }

        public string DisplayDPIU { get; set; }

        //Report Header parameter
        public string FundTypeName { get; set; }
        public string ReportName { get; set; }
        public string ReportParagraphName { get; set; }
        public string ReportFormNumber { get; set; }        
        
    }
}