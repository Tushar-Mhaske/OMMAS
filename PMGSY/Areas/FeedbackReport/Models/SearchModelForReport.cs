using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.FeedbackReport.Models
{
    public class SearchModelForReport
    {
        [Range(0, 36, ErrorMessage = "Please select a valid State.")]
        public int stateId { get; set; }
        public string stateName { get; set; }

        [RegularExpression(@"^([%MW]+)$", ErrorMessage = "Please Select Road Status.")]
        public string fbThrough { get; set; }
        public List<SelectListItem> fbThroughList { get; set; }
    }
}