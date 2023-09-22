using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using PMGSY.Controllers;

namespace PMGSY.Models.AccountDashboard
{
    public class AccountDashboardModel
    {
        [Display(Name = "State")]
        public int AgencyCode { get; set; }
        public List<SelectListItem> lstState { get; set; }

        [Display(Name = "DPIU")]
        public int DPIU { get; set; }
        public List<SelectListItem> lstDPIU { get; set; }

        [Display(Name = "Fund Type")]
        [RegularExpression("[PMA]", ErrorMessage = "Please Select Fund")]
        public String FundType { get; set; }
        public List<SelectListItem> lstFundType { get; set; }

        //[Display(Name="Month")]
        //[Range(1,int.MaxValue,ErrorMessage="Please select Month")]
        //public int Month { get; set; }
        //public List<SelectListItem> lstMonth { get; set; }

        //[Display(Name = "Year")]
        //[Range(2000, int.MaxValue, ErrorMessage = "Please select Year")]
        //public int Year { get; set; }
        //public List<SelectListItem> lstYear{ get; set; }        

        public string EncryptedProgramme { get; set; }
        public string EncryptedAdminFund { get; set; }
        public string EncryptedMaintenance { get; set; }
    }

    public class DeductionVsRemittanceChartModel
    {
        public List<Decimal?> HeadArrayDeductionsRemiAmount { get; set; }
        public String HeadName { get; set; }
        public String StatckName { get; set; }
    }
}