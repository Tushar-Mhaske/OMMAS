using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.ProposalSSRSReports.Models
{
    public class AgreementAbstractViewModel
    {
        [Required(ErrorMessage = "Please select Month.")]
        [Range(1, 12, ErrorMessage = "Please select a valid Month.")]
        public int Month { get; set; }
        public List<SelectListItem> lstMonth { get; set; }

        [Required(ErrorMessage = "Please select Year.")]
        [Range(1, 2099, ErrorMessage = "Please select a valid Year.")]
        public int Year { get; set; }
        public List<SelectListItem> lstYear { get; set; }

        [Required(ErrorMessage = "Please select PMGSY Scheme.")]
        [Range(0, 10, ErrorMessage = "Please select a valid PMGSY Scheme.")]
        public int pmgsyScheme { get; set; }
        public List<SelectListItem> lstPmgsyScheme { get; set; }
    }
}