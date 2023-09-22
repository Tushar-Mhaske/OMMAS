using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Models.Bank
{
    public class BankFilterModel
    {
        public Int16 Month { get; set; }
        public Int16 Year { get; set; }
        public Int32 AdminNdCode { get; set; }
        public String FundType { get; set; }
        public Int32 page { get; set; }
        public Int32 rows { get; set; }
        public String sidx { get; set; }
        public String sord { get; set; }
        public BankReconciliationModel[] jqGridData { get; set; }
        public string ClientIP { get; set; }
        public string HeaderDate { get; set; }
        public string HeaderRemarks { get; set; }
        public string HeaderReconcile { get; set; }
        public string BillID { get; set; }
        public Int16 BankCode { get; set; }

        //Added by Abhishek kamble 17 Sep 2014 for search details Month/ Date Wise
        public string MonthDateWise { get; set; }
        public string SearchBillDate { get; set; }

        public int LevelID { get; set; }
    }
}