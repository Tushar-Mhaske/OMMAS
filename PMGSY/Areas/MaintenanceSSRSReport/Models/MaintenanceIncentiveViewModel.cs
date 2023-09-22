using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.MaintenanceSSRSReport.Models
{
    public class MaintenanceIncentiveViewModel
    {
            public string StateName { get; set; }

            [Required(ErrorMessage = "Please select State.")]
            [Range(0, int.MaxValue, ErrorMessage = "Please select State.")]
            public int StateCode { get; set; }
            public List<SelectListItem> StateList { get; set; }

            [Required(ErrorMessage = "Please select Year.")]
            [Range(0, int.MaxValue, ErrorMessage = "Please select Year.")]
            public int YearCode { get; set; }
            public List<SelectListItem> YearList { get; set; }
    }
}