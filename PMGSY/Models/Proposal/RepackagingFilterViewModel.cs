using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Proposal
{
    public class RepackagingFilterViewModel
    {

        public RepackagingFilterViewModel()
        {
            lstYears = new List<SelectListItem>();
            lstBatchs = new List<SelectListItem>();
            lstBlocks = new List<SelectListItem>();
            lstCollaborations = new List<SelectListItem>();
            lstPackages = new List<SelectListItem>();
            lstCollaborations = new List<SelectListItem>();
            lstProposalTypes = new List<SelectListItem>();
            lstProposalTypes.Insert(0, new SelectListItem { Value = "0" , Text = "All"});
            lstProposalTypes.Insert(1, new SelectListItem { Value = "P", Text = "Road" });
            lstProposalTypes.Insert(2, new SelectListItem { Value = "L", Text = "LSB" });
            lstUpgradationTypes = new List<SelectListItem>();
            lstUpgradationTypes.Insert(0, new SelectListItem { Value = "0" , Text  = "All"});
            lstUpgradationTypes.Insert(1, new SelectListItem { Value = "N", Text = "New" });
            lstUpgradationTypes.Insert(2, new SelectListItem { Value = "U", Text = "Upgradation" });
        }

        public int Year { get; set; }

        [Required]
        [Range(1,5,ErrorMessage="Please select Batch")]
        public int Batch { get; set; }

        public int Block { get; set; }

        [Display(Name="Package")]
        public string Package { get; set; }

        [Display(Name="Collaboration")]
        public int Collaboration { get; set; }

        [Display(Name="Proposal Type")]
        public string ProposalType { get; set; }

        [Display(Name="New / Upgradation")]
        public string UpgradationType { get; set; }

        public List<SelectListItem> lstYears { get; set; }

        public List<SelectListItem> lstBatchs { get; set; }

        public List<SelectListItem> lstBlocks { get; set; }

        public List<SelectListItem> lstPackages { get; set; }

        public List<SelectListItem> lstCollaborations { get; set; }

        public List<SelectListItem> lstProposalTypes { get; set; }

        public List<SelectListItem> lstUpgradationTypes { get; set; }

        public string EncProposalCode { get; set; }

    }
}