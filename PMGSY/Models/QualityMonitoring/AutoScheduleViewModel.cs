using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.QualityMonitoring
{
    public class AutoScheduleViewModel
    {
        [Required]
        [Display(Name = "Month")]
        [Range(1, 12, ErrorMessage = "Please select Month.")]
        public Int32 Month { get; set; }
        public List<SelectListItem> lstMonth { get; set; }

        [Required]
        [Display(Name = "Year")]
        [Range(2000, 2099, ErrorMessage = "Please select Year.")]
        public Int32 Year { get; set; }
        public List<SelectListItem> lstYear { get; set; }

    }
}