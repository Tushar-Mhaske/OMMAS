using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PMGSY.Models.Report
{
    public class CBSingleModel
    {
        public CBSingleModel()
        {
            this.SingleCB = new SingleCBCollection();   
        }

        [Display(Name = "Month")]
        [Required(ErrorMessage="Please select month.")]
        [Range(0,12,ErrorMessage="Invalid Month")]
        public Int16 Month { get; set; }

        [Display(Name = "Year")]
        [Required(ErrorMessage = "Please select year.")]
        [Range(1990, 2099, ErrorMessage = "Invalid Year")]
        public Int16 Year { get; set; }

        [Display(Name = "Report Header")]
        public String ReportAnnex { get; set; }

        [Display(Name = "Nodal Agency")]
        public String StateDepartment { get; set; }

        [Display(Name = "Program Implementation Unit (PIU)")]
        public String DistrictDepartment { get; set; }

        public string SRRDA_DPIU { get; set; }

        [Display(Name="SRRDA")]
        public int SRRDA { get; set; }

        [Display(Name="DPIU")]
        //[Range(1,int.MaxValue,ErrorMessage="Please Select DPIU")]
        public int DPIU { get; set; }

        public SingleCBCollection SingleCB { get; set; }
        public Nullable<Decimal> TotalPayCash { get; set; }
        public Nullable<Decimal> TotalPayBank { get; set; }
        public UDF_ACC_GEN_GET_BA_CASH_CLOSING_BALANCES_Result ClosingBalace { get; set; }
        public Nullable<Decimal> TotalRecCash { get; set; }
        public Nullable<Decimal> TotalRecBank { get; set; }
        public UDF_ACC_GEN_GET_BA_CASH_Opening_Balances_Result OpeningBalace { get; set; }


        public string ReportNumber { get; set; }
        public string ReportName { get; set; }
        public string ReportParaName { get; set; }
        public string FundType { get; set; }
    }
}