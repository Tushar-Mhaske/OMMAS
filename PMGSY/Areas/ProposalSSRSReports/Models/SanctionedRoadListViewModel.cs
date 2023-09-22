using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.ProposalSSRSReports.Models
{
    public class SanctionedRoadListViewModel
    {
        [Required(ErrorMessage = "Please select State. ")]
        [Range(1, short.MaxValue, ErrorMessage = "Please select valid State.")]
        public int stateCode { get; set; }
        public List<SelectListItem> lstState { get; set; }

        [Required(ErrorMessage = "Please select State.")]
        [Range(0, short.MaxValue, ErrorMessage = "Please select valid State.")]
        public int pmgsyScheme { get; set; }
        public List<SelectListItem> lstPmgsyScheme { get; set; }
    }
}