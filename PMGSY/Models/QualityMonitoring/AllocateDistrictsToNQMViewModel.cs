using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.QualityMonitoring
{
    public class AllocateDistrictsToNQMViewModel
    {
        [Required(ErrorMessage = "Please select State")]
        [Range(0, 50, ErrorMessage = "Please select a valid State")]
        public int stateCode { get; set; }
        public List<SelectListItem> lstState { set; get; }

        [Display(Name = "Year")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select Year.")]
        public int Year { get; set; }
        public List<SelectListItem> YearList { get; set; }

        [Display(Name = "Month")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select Month.")]
        public int Month { get; set; }
        public List<SelectListItem> MonthList { get; set; }

        public string StateName { get; set; }
    }
}