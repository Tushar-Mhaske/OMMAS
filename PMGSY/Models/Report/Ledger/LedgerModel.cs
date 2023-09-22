using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace PMGSY.Models.Report.Ledger
{
    public class LedgerModel
    {
        public LedgerModel()
        {
            this.YEAR =Convert.ToInt16(DateTime.Now.Year);
            this.MONTH = Convert.ToInt16(DateTime.Now.Month);
        }

        [Display(Name = "Month")]
        [Range(1, Int16.MaxValue, ErrorMessage = "Please Select  Month")]
        public short MONTH { get; set; }
        public List<SelectListItem> YEAR_LIST { get; set; }

        [Display(Name = "Year")]
        [Range(1, Int16.MaxValue, ErrorMessage = "Please Select Year")]
        public short YEAR { get; set; }
        public List<SelectListItem> MONTH_LIST { get; set; }
      
        [Required(ErrorMessage = "Credit / Debit required ")]
        [MaxLength(1, ErrorMessage = "Invalid Credit/debit")]
        public string CREDIT_DEBIT { get; set; }

        [Display(Name = "Head")]
        [Required(ErrorMessage = "Head is Required")]
        [Range(1, Int16.MaxValue, ErrorMessage = "Please Select Head")]
        public String HEAD { get; set; }
        public List<SelectListItem> HEAD_LIST { get; set; }

        [Display(Name = "DPIU")]
        [Range(-1, Int16.MaxValue, ErrorMessage = "Please Select DPIU")]
        public short DPIU { get; set; }
        public List<SelectListItem> PIU_LIST { get; set; }

        //added by abhishek kamble 4-oct-2013
        public int SRRDA { get; set; }
        public string SRRDA_DPIU { get; set; }
        public int DPIULevel { get; set; }

        //added by abhishek kamble 3-jan-2014
        public int SelectedHead { get; set; }


        public String ReportAnnex { get; set; }

        public String DistrictDepartment { get; set; }

        public string StateDepartment { get; set; }

        public bool isPiuLedger { get; set; }
        public short levelId { get; set; }
        public char RoadStatus { get; set; }

        public string ReportNumber { get; set; }
        public string ReportName { get; set; }
        public string ReporPara { get; set; }
        public string FundType { get; set; }
      

    }

   

    public class LedgerAmountModel
    {

        public string ReportNumber { get; set; }
        public string ReportName { get; set; }
        public string ReporPara { get; set; }
        public string FundType { get; set; }

        public String BILL_DATE { get; set; }

        public String TEO_NUMBER { get; set; }

        public String OPENING_BALANCE { get; set; }

        public String NARRATION { get; set; }

        public String CREDIT_AMOUNT { get; set; }

        public String DEBIT_AMOUNT { get; set; }

        public String CREDIT_TOTAL { get; set; }

        public String DEBIT_TOTAL { get; set; }

        public String CREDIT_DEBIT_BALANCE_TOTAL { get; set; }

        public String CREDIT_DEBIT_BALANCE { get; set; }

        public String END_MONTH_BALANCE { get; set; }

        public string DPIUName { get; set; }

    }
     
    public class ledgerListModel {

        public string ReportNumber { get; set; }
        public string ReportName { get; set; }
        public string ReporPara { get; set; }
        public string FundType { get; set; }
        public string DPIUName { get; set; }
        public String DistrictDepartment { get; set; }

        public string StateDepartment { get; set; }

       
        public List<LedgerAmountModel> ListLedger { get; set; }
        public String OPENING_BALANCE { get; set; }
        public String CR_DR { get; set; }

        //added by abhishek kamble 7-oct-2013
        public string MonthName { get; set; }
        public string Year { get; set; }
        public string previousMonthName { get; set; }
        public string currentMonthName { get; set; }
    }

}