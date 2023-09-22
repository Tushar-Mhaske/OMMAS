using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.ProposalSSRSReports.Models
{
    public class WorksSanctionedCompletedViewModel
    {
        [Required(ErrorMessage = "Month required")]
        [Range(1, 12, ErrorMessage = "Invalid Month")]
        public int Month { get; set; }
        public List<SelectListItem> MonthList { get; set; }

        [Required(ErrorMessage = "Year required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid year")]
        public int Year { get; set; }
        public List<SelectListItem> YearList { get; set; }
    }
}