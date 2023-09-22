using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Execution
{
    public class SuperstructureViewModel
    {
        [Range(1, 12, ErrorMessage = "Please select Month")]
        public int Month { get; set; }
        public List<SelectListItem> lstMonth { get; set; }

        [Range(2000, 2099, ErrorMessage = "Please select Year")]
        public int Year { get; set; }
        public List<SelectListItem> lstYear { get; set; }

        [Range(1, 12, ErrorMessage = "Please select SubComponent")]
        public int SubComponent { get; set; }
        public List<SelectListItem> lstSubComponent { get; set; }

        //[Range(1, 12, ErrorMessage = "Please select Floor")]
        //public int Floor { get; set; }
        //public List<SelectListItem> lstFloor { get; set; }

        [RegularExpression(@"^[N|Y|NA]+$", ErrorMessage = "Please select valid value")]
        public string groundFloor { get; set; }
        public List<SelectListItem> lstGroundFloor { get; set; }

        [RegularExpression(@"^[N|Y|NA]+$", ErrorMessage = "Please select valid value")]
        public string firstFloor { get; set; }
        public List<SelectListItem> lstfirstFloor { get; set; }

        [RegularExpression(@"^[N|Y|NA]+$", ErrorMessage = "Please select valid value")]
        public string secondFloor { get; set; }
        public List<SelectListItem> lstSecondFloor { get; set; }

        [RegularExpression(@"^[N|Y|NA]+$", ErrorMessage = "Please select valid value")]
        public string thirdFloor { get; set; }
        public List<SelectListItem> lstThirdFloor { get; set; }

        [RegularExpression(@"^[N|Y|NA]+$", ErrorMessage = "Please select valid value")]
        public string coveredParking { get; set; }
        public List<SelectListItem> lstCoveredParking { get; set; }

        [RegularExpression(@"^[N|Y|NA]+$", ErrorMessage = "Please select valid value")]
        public string approachRoad { get; set; }
        public List<SelectListItem> lstApproachRoad { get; set; }

        public string EncrProposalCode { get; set; }
        public string EncrProgressCode { get; set; }
    }
}