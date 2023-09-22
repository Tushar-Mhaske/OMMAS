using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Proposal
{
    public class Stage1ToStage2ViewModel
    {
        public Stage1ToStage2ViewModel()
        {
            lstStagedPackages = new List<SelectListItem>();
            lstStagedPackages.Insert(0, new SelectListItem { Value = "0" , Text = "Select Staged Package"});
            lstStagedProposals = new List<SelectListItem>();
            lstStagedProposals.Insert(0, new SelectListItem { Value = "0", Text = "Select Staged Road" });
            lstStagedYears = new List<SelectListItem>();
        }

        public int ProposalCode { get; set; }

        public int Batch { get; set; }

        public int AdminCode { get; set; }

        public int DistrictCode { get; set; }

        [Required]
        [Range(1,Int32.MaxValue,ErrorMessage="Please select Staged Year")]
        [Display(Name="Staged Year")]
        public int STAGE_YEAR { get; set; }
        public List<SelectListItem> lstStagedYears { get; set; }

        [Required]
        [Display(Name="Staged Package Number")]
        public string STAGE_PACKAGE { get; set; }
        public List<SelectListItem> lstStagedPackages { get; set; }

        [Required]
        [Display(Name="Staged Road")]
        [Range(1,Int32.MaxValue,ErrorMessage="Please select Proposal")]
        public int STAGE_PROPOSAL { get; set; }
        public List<SelectListItem> lstStagedProposals { get; set; }

    }
}