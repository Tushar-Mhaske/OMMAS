using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.PFMSReports.Models
{
    public class ReatDscStatus
    {
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid State.")]
        public int StateCode { get; set; }
        public List<SelectListItem> StateList { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Please select valid District.")]
        public int DistrictCode { get; set; }
        public List<SelectListItem> DistrictList { get; set; }

        public string StateName { get; set; }
        public string DistrictName { get; set; }
        public short LEVEL { get; set; }
    }
}