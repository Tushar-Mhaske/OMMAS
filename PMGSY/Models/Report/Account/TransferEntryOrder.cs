using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Report.Account
{
    public class RptTransferEntryOrder
    {
        public RptTransferEntryOrder()
        {
        }
        [Display(Name = "Month")]
        [Range(1, 12, ErrorMessage = "Please Select Month")]
        public short Month { get; set; }
        public List<SelectListItem> MonthList { get; set; }


        [Display(Name = "Year")]
        [Range(1, short.MaxValue, ErrorMessage = "Please Select Year")]
        public short Year { get; set; }
        public List<SelectListItem> YearList { get; set; }

        [Display(Name = "DPIU")]
        [Range(1, short.MaxValue, ErrorMessage = "Please Select DPIU")]
        public short Dpiu { get; set; }
        public List<SelectListItem> DpiuList { get; set; }

        public bool isSRRDA { get; set; }
        public int AdminNDCode { get; set; }

        public string FundType{get; set;}



    }
    public class RptTrnasferEntryOrderList
    {
        public List<SP_ACC_RPT_DISPLAY_TEO_DETAILS_Result> ListTeo { get; set; }
        public double DebitAmt { get; set; }
        public double CreditAmt { get; set; }
        public string FormNumber { get; set; }
        public string ReportName { get; set; }
        public string Paragraph { get; set; }


    }


}