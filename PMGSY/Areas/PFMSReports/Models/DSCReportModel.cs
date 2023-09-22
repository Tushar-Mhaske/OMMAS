using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace PMGSY.Areas.PFMSReports.Models
{
    public class DSCReportModel
    {
           
            [Display(Name = "State")]
            [Required(ErrorMessage = "Please select State. ")]
            [Range(0, int.MaxValue, ErrorMessage = "Please select  State.")]
            public int StateCode { get; set; }
            public List<SelectListItem> StateList { get; set; }

             [Display(Name = "District")]
            [Required(ErrorMessage = "Please select District.")]
            [Range(0, int.MaxValue, ErrorMessage = "Please select District.")]
            public int DistrictCode { get; set; }
            public List<SelectListItem> DistrictList { get; set; }

            public string StateName { get; set; }
            public string DistrictName { get; set; }

    }
}