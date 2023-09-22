using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.QualityMonitoring
{
    public class NQMNAViewModel
    {
        [Required(ErrorMessage = "Please select Monitor")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select a valid Monitor")]
        public int Monitor { get; set; }
        public List<SelectListItem> lstMonitor { set; get; }

        [Required(ErrorMessage = "Please select Month")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select a valid Month")]
        public int Month { get; set; }
        public List<SelectListItem> lstMonth { set; get; }

        [Required(ErrorMessage = "Please select Year")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select a valid Year")]
        public int Year { get; set; }
        public List<SelectListItem> lstYear { set; get; }

        [RegularExpression("[SD]", ErrorMessage = "Please Select valid value for Inspection")]
        public string isInspection { get; set; }
        
    }
}