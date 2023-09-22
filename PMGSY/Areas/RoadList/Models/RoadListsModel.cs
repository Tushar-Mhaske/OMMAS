using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.RoadList.Models
{
    public class RoadListsModel
    {


        [Range(1, 2147483647, ErrorMessage = "Please select state.")]
        [RegularExpression("^([0-9]+)$", ErrorMessage = "State must be valid number.")]
        [Display(Name = "State : ")]
        public int StateCode { get; set; }
        public List<SelectListItem> lstStates { get; set; }

        [Range(1, 2147483647, ErrorMessage = "Please select MP Constituency.")]
        [RegularExpression("^([0-9]+)$", ErrorMessage = "Invalid MP Constituency.")]
        [Display(Name = "MP Constituency : ")]
        public int MPConstCode { get; set; }

        public List<SelectListItem> lstMPConst { get; set; }

        public String StateName { get; set; }
        public String MPConstName { get; set; }

    }
}