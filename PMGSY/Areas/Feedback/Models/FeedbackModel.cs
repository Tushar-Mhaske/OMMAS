using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.Common;
using PMGSY.Extensions;
using System.ComponentModel.DataAnnotations;

namespace PMGSY.Areas.Feedback.Models
{
    public class FeedbackModel
    {


        [Display(Name = "Month")]
        [Range(1, 12, ErrorMessage = "Please select Month.")]
        [Required(ErrorMessage = "Please select Month.")]
        [RegularExpression("^([0-9]+)$", ErrorMessage = "Month must be valid number.")]
        public int Month { get; set; }
        public List<SelectListItem> MonthList { get; set; }


        [Display(Name = "Year")]
        [Range(1, 2090, ErrorMessage = "Please select Year.")]
        [Required(ErrorMessage = "Please select Year.")]
        [RegularExpression("^([0-9]+)$", ErrorMessage = "Year must be valid number.")]
        public int Year { get; set; }
        public List<SelectListItem> YearList { get; set; }


    }
}