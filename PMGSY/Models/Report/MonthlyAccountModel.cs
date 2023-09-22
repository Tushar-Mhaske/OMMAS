using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Report
{
    public class MonthlyAccountModel
    {
        [DisplayName("Month")]
        //Modified By Abhishek kamble 3-dec-2013
        //[Required(ErrorMessage="Please Select Month")]
        [Range(1,12,ErrorMessage="Please Select month")]
        public Int16 Month { get; set; }

        [DisplayName("Year")]
        //Modified By Abhishek kamble 3-dec-2013
        //[Required(ErrorMessage = "Please Select Year")]
        [Range(2000, Int32.MaxValue, ErrorMessage = "Please Select Year")]
        public Int16 Year { get; set; }

        [DisplayName("Balance")]
        //Modified By Abhishek kamble 3-dec-2013
        //[Required(ErrorMessage = "Please Select Balance")]
        [RegularExpression("[CD]", ErrorMessage = "Please Select Balance")]
        public String CreditDebit { get; set; }

        [DisplayName("Nodal Agency")]
        public String NodalAgency { get; set; }

        public List<USP_RPT_SHOW_MONTHLY_ACCOUNT_SELF_Result> lstMonthlyAccountSelf{ get; set; }

        //added by abhishek 10-9-2013
        public List<USP_RPT_SHOW_MONTHLY_ACCOUNT_ALLPIU_Result> lstMonthlyAccountAllPIU { get; set; }

        public Nullable<Decimal> TotalOpeningAmountForPIU { get; set; }
        public Nullable<Decimal> TotalCreditDebitAmountForPIU { get; set; }
        
        public Nullable<Decimal> TotalOpeningAmount { get; set; }

        public Nullable<Decimal> TotalCreditDebit { get; set; }

        public string monthlyStateSrrdaDpiu { get; set; }

        [DisplayName("STATE")]
        public Int16 State { get; set; }
        [DisplayName("SRRDA")]
        public Int16 Srrda { get; set; }

        [DisplayName("DPIU")]
        public Int16 Dpiu { get; set; }

        public string MonthName { get; set; }

        public string BalanceName { get; set; }

        public string StateSRRDAName { get; set; }

        public string StateName { get; set; }

        
        //Report Header parameter
        public string FundTypeName { get; set; }
        public string ReportName { get; set; }
        public string ReportParagraphName { get; set; }
        public string ReportFormNumber { get; set; }
    }
}