using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.QualityMonitoring
{
    public class NQMNotAvailableInspectionViewModel
    {
        [Required]
        [Display(Name = "Monitor")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid Monitor Code.")]
        public Int32 AdminQmCode { get; set; }
        public List<SelectListItem> lstAdminQmCode { get; set; }

        public string ASSIGNED_lstAdminQmCode { get; set; }

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

        [RegularExpression("[SD]", ErrorMessage = "Please Select valid value for Inspection")]
        public string isInspection { get; set; }

    }
}