using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.QualityMonitoring
{
    public class WorkListModel
    {
        [Display(Name = "State Name")]
        public String StateName { get; set; }

        [Display(Name = "State: ")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid state")]
        public int stateCode { get; set; }
        public List<SelectListItem> lstStates { set; get; }

        [Display(Name = "District: ")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid district")]
        public int districtCode { get; set; }
        public List<SelectListItem> lstDistricts { set; get; }

        [Display(Name = "Technology: ")]  // Added by Shreyas on 14-09-2023
        public int techSeleted { get; set; }
        public List<SelectListItem> lstTechnology { set; get; }

    }
}