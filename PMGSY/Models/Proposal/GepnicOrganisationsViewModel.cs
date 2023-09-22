using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Proposal
{
    public class GepnicOrganisationsViewModel
    {
        [Required(ErrorMessage = "Please select a Organisation")]
        [Display(Name = "Organisation")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select a valid Organisation")]
        public int organisationCode { get; set; }
        public List<SelectListItem> lstOrganisation { get; set; }

        public string proposalsIds { get; set; }
    }
}