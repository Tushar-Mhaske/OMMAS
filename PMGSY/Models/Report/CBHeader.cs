using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PMGSY.Models.Report
{
    public class CBHeader
    {
        [Display(Name = "Month")]
        public Int16 Month { get; set; }
        
        [Display(Name = "Year")]
        public Int16 Year { get; set; }

        [Display(Name = "Report Header")]
        public String ReportAnnex { get; set; }

        [Display(Name = "Nodal Agency")]
        public String StateDepartment { get; set; }

        [Display(Name = "Program Implementation Unit (PIU)")]
        public String DistrictDepartment { get; set; }

        public UDF_ACC_GEN_GET_BA_CASH_Opening_Balances_Result OpeningBalace { get; set; }
        public UDF_ACC_GEN_GET_BA_CASH_CLOSING_BALANCES_Result ClosingBalace { get; set; }
       
    }
}