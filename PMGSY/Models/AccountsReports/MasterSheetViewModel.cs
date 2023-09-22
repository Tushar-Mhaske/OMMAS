using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.AccountsReports
{
    public class MasterSheetViewModel
    {
        public List<USP_ACC_MASTER_SHEET_PIU_RPT_Result> LIST_MASTER_SHEET { get; set; }
        public List<string> LIST_DISTINCT_HEAD_CODE { get; set; }
        public Dictionary<int, string> LIST_DISTINCT_ND_CODE { get; set; }
        public Dictionary<int, decimal> LiabilitiesTotalWithDepartments { get; set; }
        public Dictionary<int, decimal> AssetsTotalWithDepartments { get; set; }
        public Dictionary<string, decimal> HeadWiseTotalOfDepartments { get; set; }
        public decimal LiabilitiesGrandTotal { get; set; }
        public decimal AssetsGrandTotal { get; set; }
        public decimal SRRDAGrandTotal { get; set; }
        [DisplayName("Year")]
        public int Year { get; set; }
        public List<SelectListItem> YEARS_LIST { get; set; }

        //added by abhishek kamble 15-nov-2013
        
        //Report Header parameter
        public string FundTypeName { get; set; }
        public string ReportName { get; set; }
        public string ReportParagraphName { get; set; }
        public string ReportFormNumber { get; set; }
    }

    public class USP_ACC_MASTER_SHEET_PIU_RPT_Result
    {
        public string HEAD_CODE { get; set; }
        public string HEAD_NAME { get; set; }
        public string CREDIT_DEBIT { get; set; }
        public int ADMIN_ND_CODE { get; set; }
        public string ADMIN_ND_NAME { get; set; }
        public string MAST_ND_TYPE { get; set; }
        public Nullable<decimal> MONTHLY_BALANCE_AMT { get; set; }
        public Nullable<decimal> MONTHLY_CREDIT_AMT { get; set; }
        public Nullable<decimal> MONTHLY_DEBIT_AMT { get; set; }
        public Nullable<decimal> OB_BALANCE_AMT { get; set; }
        public Nullable<decimal> OB_CREDIT_AMT { get; set; }
        public Nullable<decimal> OB_DEBIT_AMT { get; set; }
    }
}