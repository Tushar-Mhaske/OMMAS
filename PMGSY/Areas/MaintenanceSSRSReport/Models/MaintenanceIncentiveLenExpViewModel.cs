using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.MaintenanceSSRSReport.Models
{
    public class MaintenanceIncentiveLenExpViewModel
    {
        public int Level { get; set; }
        public string LocationName { get; set; }

        [Display(Name = "State")]
        [Required(ErrorMessage = "Please select State.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select State.")]
        public int StateCode { get; set; }
        public List<SelectListItem> StateList { get; set; }



    }
}