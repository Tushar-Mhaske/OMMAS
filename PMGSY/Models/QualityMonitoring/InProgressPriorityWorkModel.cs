using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using PMGSY.Models;
using System.Web.Mvc;

namespace PMGSY.Models.QualityMonitoring
{
    public class InProgressPriorityWorkModel
    {
        [Display(Name = "State")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid state")]
        public int stateCode { get; set; }
        public List<SelectListItem> lstStates { set; get; }

        [Display(Name = "District")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid district")]
        public int districtCode { get; set; }
        public List<SelectListItem> lstDistricts { set; get; }
    }
}