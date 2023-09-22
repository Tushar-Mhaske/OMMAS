using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.AccountsReports
{
    public class StateAccountMonitoringViewModel
    {   
        [Display(Name="State")]                
        public int StateCode{get;set;}
        public List<SelectListItem> lstStates { get; set; }

        [Display(Name = "Agency")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select Agency")]
        public int Agency { get; set; }
        public List<SelectListItem> lstAgency { get; set; }

        [Display(Name = "Fund Type")]
        [Required(ErrorMessage="Please Select Fund Type")]
        [RegularExpression("[PAM]",ErrorMessage="Please Select Fund Type")]
        public string FundType { get; set; }
        public List<SelectListItem> lstFundType{ get; set; }

        public string DisplayStateName { get; set; }
        public string DisplayAgencyName { get; set; }
        public string DisplayFundType { get; set; }

        //Report Header Parameter
        public string FundTypeName { get; set; }
        public string ReportName { get; set; }
        public string ReportParagraphName { get; set; }
        public string ReportFormNumber { get; set; }        
    }

}