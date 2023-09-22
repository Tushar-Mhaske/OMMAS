using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Master
{
    public class MaintenancePolicyFilterViewModel
    {
        [Required(ErrorMessage="Please select State")]
        [Display(Name="State")]
        [Range(1,Int32.MaxValue,ErrorMessage="Please select State")]
        public int State { get; set; }
        public List<SelectListItem> lstStatesSearch { get; set; }

        [Required(ErrorMessage = "Please select Agency")]
        [Display(Name = "Agency")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Please select Agency")]
        public int Agency { get; set; }
        public List<SelectListItem> lstAgenciesSearch { get; set; }
    }
}