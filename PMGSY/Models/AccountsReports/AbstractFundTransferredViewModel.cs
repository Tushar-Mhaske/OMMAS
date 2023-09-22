using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PMGSY.Models.AccountsReports
{
    public class AbstractFundTransferredViewModel
    {
        [Required(ErrorMessage = "Please select year.")]
        [Range(1990, 2099, ErrorMessage = "Please select year.")]
        public string Year { get; set; }

        [Required(ErrorMessage = "Please select head.")]
        [Range(1, Int64.MaxValue, ErrorMessage = "Please select head.")]
        public string Head { get; set; }

        [Required(ErrorMessage = "Please select state.")]
        [Range(1, Int64.MaxValue, ErrorMessage = "Please select state.")]
        public string State { get; set; }

        //[Required(ErrorMessage = "Please select DPIU.")]
        //[Range(1, Int64.MaxValue, ErrorMessage = "Please select DPIU.")]
        public string DPIU { get; set; }

        public List<USP_ACC_RPT_REGISTER_ABSTRACT_PIUWISE_FUND_TRANSFERRED_Result> lstAbstractFund { get; set; }

        public string DPIUName { get; set; }

        public string HeadName { get; set; }

        public string YearName { get; set; }

        public string StateName { get; set; }

        public string ReportNumber { get; set; }

        public string ReportName { get; set; }

        public string ReportPara { get; set; }

        public string FundName { get; set; }

    }
}