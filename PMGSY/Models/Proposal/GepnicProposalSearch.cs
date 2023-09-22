using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Proposal
{
    public class GepnicProposalSearch
    {
        [Required(ErrorMessage = "Please select State")]
        [Display(Name = "State")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select State")]
        public int State { get; set; }
        public List<SelectListItem> lstState { get; set; }

        [Required(ErrorMessage="Please select District")]
        [Display(Name="District")]
        [Range(-1,Int32.MaxValue,ErrorMessage="Please select District")]
        public int District { get; set; }
        public List<SelectListItem> lstDistricts { get; set; }

        [Required(ErrorMessage="Please select Block")]
        [Display(Name="Block")]
        [Range(-1,Int32.MaxValue,ErrorMessage="Please select Block")]
        public int Block { get; set; }
        public List<SelectListItem> lstBlocks { get; set; }

        [Required(ErrorMessage="Please select Sanction Year")]
        [Display(Name="Sanction Year")]
        [Range(0,2100,ErrorMessage="Please select Sanction Year")]
        public int SanctionYear { get; set; }
        public List<SelectListItem> lstSanctionYears { get; set; }

        [Required(ErrorMessage="Please select Proposal Type")]
        [Display(Name="Proposal Type")]
        public string ProposalType { get; set; }
        public List<SelectListItem> lstProposalTypes { get; set; }

        [Required(ErrorMessage="Please select Package")]
        [Display(Name="Package")]
        public string Package { get; set; }
        public List<SelectListItem> lstPackages { get; set; }

        public int Roadcode { get; set; }
    }
}