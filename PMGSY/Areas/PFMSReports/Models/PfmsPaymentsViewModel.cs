using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.PFMSReports.Models
{
    public class PfmsPaymentsViewModel
    {
        //Radio Button
        [RegularExpression("[SD]", ErrorMessage = "Please Select SRRDA or DPIU")]
        public string SRRDA_DPIU { get; set; }

        //[Range(1, int.MaxValue, ErrorMessage = "Please select valid SRRDA.")]
        public int SRRDA { get; set; }
        public List<SelectListItem> lstSrrda { get; set; }

        //[Range(1, int.MaxValue, ErrorMessage = "Please select valid DPIU.")]
        public int Dpiu { get; set; }
        public List<SelectListItem> lstDpiu { get; set; }

        [Range(1, 12, ErrorMessage = "Please select valid Month.")]
        public int Month { get; set; }
        public List<SelectListItem> MonthList { get; set; }

        [Range(2000, 2099, ErrorMessage = "Please select valid Year.")]
        public int Year { get; set; }
        public List<SelectListItem> YearList { get; set; }

        public int Level { get; set; }
        public int AdminCode { get; set; }
        public string AdminName { get; set; }
    }
}