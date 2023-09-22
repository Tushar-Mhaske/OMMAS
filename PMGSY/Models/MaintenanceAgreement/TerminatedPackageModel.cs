using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using PMGSY.DAL.Agreement;
using PMGSY.Models.Agreement;
using PMGSY.Extensions;
using PMGSY.Models.Proposal;

namespace PMGSY.Models.MaintenanceAgreement
{
    public class TerminatedPackageModel
    {
        [Display(Name = "State Name")]
        public String StateName { get; set; }

        [Display(Name = "State")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid state")]
        public int stateCode { get; set; }
        public List<SelectListItem> lstStates { set; get; }

        [Display(Name = "District Name")]
        public String DistrictName { get; set; }

        [Display(Name = "District")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid district")]
        public int districtCode { get; set; }
        public List<SelectListItem> lstDistricts { set; get; }

        [Display(Name = "Block")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid block")]
        public int blockCode { get; set; }
        public List<SelectListItem> lstBlock { set; get; }

        [Display(Name = "Sanctioned Year")]
        [Range(-1, int.MaxValue, ErrorMessage = "Please select a valid year")]
        public int year { get; set; }
        public List<SelectListItem> Years { set; get; }      
    }

}