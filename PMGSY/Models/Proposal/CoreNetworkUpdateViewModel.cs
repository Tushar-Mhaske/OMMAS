#region File Header
/*
        * Project Name  :   OMMAS II
        * Name          :   CoreNetworkUpdateViewModel.cs
        * Description   :   This View Model is Used for mapping the core network with the proposals
        * Author        :   Vikram Nandanwar        
        * Creation Date :   28/July/2014
 **/
#endregion

using PMGSY.Common;
using PMGSY.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Proposal
{
    public class CoreNetworkUpdateViewModel
    {
        public CoreNetworkUpdateViewModel()
        {
            lstCoreNetworks = new List<SelectListItem>();
            if (PMGSYSession.Current.PMGSYScheme == 1)
            {
                lstCoreNetworks.Insert(0, new SelectListItem { Value = "0", Text = "Select Core Network" });
            }
            else
            {
                lstCoreNetworks.Insert(0, new SelectListItem { Value = "0", Text = "Select Candidate Road" });
            }
        }
        
        public string EncryptedProposalCode { get; set; }

        [Display(Name="Block")]
        public int Block { get; set; }

        [Required(ErrorMessage="Please enter Road From")]
        [Display(Name="Road From")]
        public string RoadFrom { get; set; }

        [Required(ErrorMessage="Please enter Road To")]
        [Display(Name="Road To")]
        public string RoadTo { get; set; }

        [Required(ErrorMessage="Please select Core Network.")]
        [Range(1,Int32.MaxValue,ErrorMessage="Please select Core Network.")]
        public int CnCode { get; set; }

        public List<SelectListItem> lstCoreNetworks { get; set; }

        public List<SelectListItem> lstBlocks { get; set; }

    }

    public class ProposalBlockUpdateViewModel 
    {
        public ProposalBlockUpdateViewModel()
        {
            lstBlocks = new List<SelectListItem>();
            lstDistricts = new List<SelectListItem>();
            lstPIU = new List<SelectListItem>();
        }

        public int ProposalCode { get; set; }

        [Required]
        [Display(Name="Block")]
        [Range(1,Int32.MaxValue,ErrorMessage="Please select Block.")]
        public int Block { get; set; }
        public List<SelectListItem> lstBlocks { get; set; }

        [Required]
        [Display(Name = "District")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select District.")]
        public int District { get; set; }
        public List<SelectListItem> lstDistricts { get; set; }

        [Required]
        [Display(Name = "PIU")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select PIU.")]
        public int PIU { get; set; }
        public List<SelectListItem> lstPIU { get; set; }
    }
}