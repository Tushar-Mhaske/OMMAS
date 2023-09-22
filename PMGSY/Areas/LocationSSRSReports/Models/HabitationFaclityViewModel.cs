using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.LocationSSRSReports.Models
{
    public class HabitationFaclityViewModel
    {
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid State.")]
        public int StateCode { get; set; }
        public List<SelectListItem> StateList { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Please select valid District.")]
        public int DistrictCode { get; set; }
        public List<SelectListItem> DistrictList { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Block.")]
        public int BlockCode { get; set; }
        public List<SelectListItem> BlockList { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Year.")]
        public int Year { get; set; }
        public List<SelectListItem> YearList { get; set; }

        [RegularExpression("[SD]", ErrorMessage = "Please Select valid Active value")]
        public string Active { get; set; }
        public List<SelectListItem> ActiveList { get; set; }

        public string StateName { get; set; }
        public string DistrictName { get; set; }
        public string BlockName { get; set; }
    }
}