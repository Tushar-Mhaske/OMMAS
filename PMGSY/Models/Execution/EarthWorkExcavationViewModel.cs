using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Execution
{
    public class EarthWorkExcavationViewModel
    {
        //[Required]
        [Range(1, 12, ErrorMessage = "Please select Month")]
        public int Month { get; set; }
        public List<SelectListItem> lstMonth { get; set; }

        [Range(2000, 2099, ErrorMessage = "Please select Year")]
        public int Year { get; set; }
        public List<SelectListItem> lstYear { get; set; }

        [RegularExpression(@"^[N|Y|NA]+$", ErrorMessage = "Please select valid value")]
        public string EarthWorkExcavationPCC { get; set; }
        public List<SelectListItem> lstEarthWorkExcavationPCC { get; set; }

        public string EncrProposalCode { get; set; }
        public string EncrProgressCode { get; set; }
    }
}