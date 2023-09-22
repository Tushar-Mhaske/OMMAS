using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Proposal
{
    public class CoreNetworkMappingViewModel
    {
        public CoreNetworkMappingViewModel()
        {
            lstBlocks = new List<SelectListItem>();
            lstCoreNetworks = new List<SelectListItem>();
        }

        public int ProposalCode { get; set; }

        public string ProposalType { get; set; }

        public string UpgradeConnect { get; set; }

        [Display(Name="Block")]
        [Range(0,Int32.MaxValue,ErrorMessage="Please select Block.")]
        [Required(ErrorMessage="Please select Block.")]
        public int Block { get; set; }
        public List<SelectListItem> lstBlocks { get; set; }

        [Display(Name="Core Network")]
        [Required(ErrorMessage = "Please select Core Network.")]
        public int CnCode { get; set; }
        public List<SelectListItem> lstCoreNetworks { get; set; }

    }
}