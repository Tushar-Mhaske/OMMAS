using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace PMGSY.Areas.OtherReports.Models
{
    public class RoadCategoryModel
    {

        [Display(Name = "State")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select valid State")]
        public int StateCode { get; set; }
        public List<SelectListItem> StateList { get; set; }


        [Display(Name = "District")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select valid District")]
        public int DistrictCode { get; set; }
        public List<SelectListItem> DistrictList { get; set; }


        public string StateName { get; set; }

        public string DistName { get; set; }



    }
}