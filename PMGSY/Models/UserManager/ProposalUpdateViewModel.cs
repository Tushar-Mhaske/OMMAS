using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.UserManager
{
    public class ProposalUpdateViewModel
    {

        public ProposalUpdateViewModel()
        {
            lstAgencies = new List<SelectListItem>();
            lstAgencies.Insert(0, new SelectListItem { Value = "0" , Text = "Select Agency"});
            lstDistricts = new List<SelectListItem>();
            lstStates = new List<SelectListItem>();
            lstPIU = new List<SelectListItem>();
        }

        [Display(Name="State")]
        [Range(1,Int32.MaxValue,ErrorMessage="Please select state.")]
        public int State { get; set; }

        [Display(Name="District")]
        [Range(1,Int32.MaxValue,ErrorMessage="Please select District")]
        public int District { get; set; }

        [Display(Name="Agency")]
        public int Agency { get; set; }

        [Display(Name="PIU")]
        public int PIUCode { get; set; }

        public List<SelectListItem> lstStates { get; set; }

        public List<SelectListItem> lstDistricts { get; set; }

        public List<SelectListItem> lstPIU { get; set; }

        public List<SelectListItem> lstAgencies { get; set; }

    }
}