using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.ProposalSSRSReports.Models
{
    public class CompletionPlanReportModel
    {

        public CompletionPlanReportModel()
        {
            StateList = new List<SelectListItem>();
            YearList = new List<SelectListItem>();
        }

        [Required]
        [Range(0,Int32.MaxValue,ErrorMessage="Please select State.")]
        public int State { get; set; }
        public string StateName { get; set; }
        public List<SelectListItem> StateList { get; set; }

        [Required]
        [Range(1,Int32.MaxValue,ErrorMessage="Please select Year")]
        public int Year { get; set; }
        public string YearText { get; set; }
        public List<SelectListItem> YearList { get; set; }

    }
}