using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PMGSY.Models.AccountsReports
{
    public class BankAuthrizationViewModel
    {
        [Display(Name="Month")]
        [Required(ErrorMessage="Please select month")]
        [Range(1, 12, ErrorMessage = "Please select month")]
        public string Month { get; set; }

        [Display(Name = "Year")]
        [Required(ErrorMessage = "Please select year")]
        [Range(2000, int.MaxValue, ErrorMessage = "Please select year")]
        public string Year { get; set; }

        [Display(Name = "State")]
        [Required(ErrorMessage = "Please select state")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select state")]
        public string State { get; set; }

        [Display(Name = "DPIU")]
        //[Required(ErrorMessage = "Please select DPIU")]
        //[Range(1, int.MaxValue, ErrorMessage = "Please select DPIU")]
        public string DPIU { get; set; }

        public List<USP_ACC_RPT_REGISTER_PIUWISE_BANK_AUTHORIZATION_ISSUED_Result> lstBankAuthrization { get; set; }

        public Nullable<Decimal> TotalOpeningBalance { get; set; }

        public Nullable<Decimal> TotalCredit { get; set; }

        public Nullable<Decimal> TotalDebit { get; set; }

        public string MonthName { get; set; }

        public string YearName { get; set; }

        public string SRRDAName { get; set; }

        public string DPIUName { get; set; }

        public string ReportNumber { get; set; }

        public string ReportName { get; set; }

        public string ReportPara { get; set; }

        public string FundName { get; set; }

        public Int64 TotalRecord { get; set; }
    }
}