using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.LockUnlock
{
    public class FilterDetailsViewModel
    {

        public FilterDetailsViewModel()
        {
            ddlBlock = new List<SelectListItem>();
            ddlDistrict = new List<SelectListItem>();
            ddlHabitation = new List<SelectListItem>();
            ddlVillage = new List<SelectListItem>();
            ddlState = new List<SelectListItem>();
            ddlYear = new List<SelectListItem>();
            ddlPackage = new List<SelectListItem>();
            ddlBatch = new List<SelectListItem>();
            lstSchemes = new List<SelectListItem>();

            //lstSanction = new List<SelectListItem>();
            //lstSanction.Insert(0, new SelectListItem { Value = "M", Text = "MoRD Sanctioned" });
            //lstSanction.Insert(1, new SelectListItem { Value = "S", Text = "STA/PTA Scrutinized" });

            ///Changes by SAMMED A. PATIL on 20JULY2017 to display Type at ITNO login
            lstSanction = new List<SelectListItem>();
            if (PMGSY.Extensions.PMGSYSession.Current.RoleCode == 36 || PMGSY.Extensions.PMGSYSession.Current.RoleCode == 47 || PMGSY.Extensions.PMGSYSession.Current.RoleCode == 56)
            {
                lstSanction.Insert(0, new SelectListItem { Value = "S", Text = "STA UnScrutinized" });
                lstSanction.Insert(1, new SelectListItem { Value = "D", Text = "DPIU Finalized" });
            }
            else
            {
                lstSanction.Insert(0, new SelectListItem { Value = "M", Text = "MoRD Sanctioned" });
                lstSanction.Insert(1, new SelectListItem { Value = "S", Text = "STA/PTA Scrutinized" });
            }
        }

        public int State { get; set; }

        public int District { get; set; }

        public int Block { get; set; }

        public int Village { get; set; }

        public int Habitation { get; set; }

        // Added By to unlock Proposal Technology Details
        [Range(1, int.MaxValue, ErrorMessage = "Please select Year")]
        public int Year { get; set; }

        public string Package { get; set; }

        public int Batch { get; set; }

        public string Type { get; set; }

        public string ProposalType { get; set; }

        [Required(ErrorMessage="Please select Scheme.")]
        ///Changes for RCPLWE unlock at ITNO/PMGSY3
        [Range(1, 5, ErrorMessage = "Please select Scheme.")]
        public byte PMGSYScheme { get; set; }

        public List<SelectListItem> ddlDistrict { get; set; }

        public List<SelectListItem> ddlBlock { get; set; }

        public List<SelectListItem> ddlVillage { get; set; }

        public List<SelectListItem> ddlHabitation { get; set; }

        public List<SelectListItem> ddlState { get; set; }

        public List<SelectListItem> ddlYear { get; set; }

        public List<SelectListItem> ddlPackage { get; set; }

        public List<SelectListItem> ddlBatch { get; set; }

        public List<SelectListItem> lstSchemes { get; set; }

        public List<SelectListItem> lstSanction { get; set; }

        public List<SelectListItem> lstProposalTypes { get; set; }
        //Changes for Collaboration
        public int Collaboration { get; set; }
        public List<SelectListItem> lstCollaboration { get; set; }
    }
}