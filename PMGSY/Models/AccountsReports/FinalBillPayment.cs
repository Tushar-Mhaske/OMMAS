using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;                           
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace PMGSY.Models.AccountsReports
{
    public class FinalBillPaymentModel
    {                                     
        [Display(Name="State")]
        [Range(1,int.MaxValue,ErrorMessage="Please select State")]
        public int StateCode { get; set; }
        public List<SelectListItem> lstStates { get; set; }

        [Display(Name="Agency")]
        [Range(1,int.MaxValue,ErrorMessage="Please select Agency")]
        public int Agency{get;set;}
        public List<SelectListItem> lstAgency { get; set; }

        [Display(Name="Funding Agency")]
        public int FundingAgency { get; set; }
        public List<SelectListItem> lstFundingAgency { get; set; }

        public string DisplayStateName{get;set;}
        public string DisplayAgencyName{get;set;}
        public string DisplayFundingAgencyName { get; set; }

        //Report Header Parameter
        public string FundTypeName { get; set; }
        public string ReportName { get; set; }
        public string ReportParagraphName { get; set; }
        public string ReportFormNumber { get; set; }        
    }
}