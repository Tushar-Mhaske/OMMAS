using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using PMGSY.Models;
using System.Web.Mvc;





namespace PMGSY.Models.QualityMonitoring
{
    public class QCRReportViewModel
    {
      

        [Display(Name = "State Name")]
        public String StateName { get; set; }

        [Display(Name = "State")]
        //[Range(1, int.MaxValue, ErrorMessage = "Please select a valid state")]
        public int stateCode { get; set; }
        public List<SelectListItem> lstStates { set; get; }


        [Display(Name = "Sanctioned Year")]
        //[Range(-1, int.MaxValue, ErrorMessage = "Please select a valid year")]
        public int year { get; set; }
        public List<SelectListItem> Years { set; get; }

        [Display(Name = "District")]
        //[Range(1, int.MaxValue, ErrorMessage = "Please select a valid district")]
        public int districtCode { get; set; }
        public List<SelectListItem> lstDistricts { set; get; }

        [Display(Name = "Scheme")]
        //[Range(-1, int.MaxValue, ErrorMessage = "Please select a valid year")]
        public int Scheme { get; set; }
        public List<SelectListItem> lstScheme { set; get; }

    }
}