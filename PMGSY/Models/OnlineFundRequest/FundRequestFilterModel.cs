using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.OnlineFundRequest
{
    public class FundRequestFilterModel
    {

        public FundRequestFilterModel()
        {
            lstAgencies = new List<SelectListItem>();
            lstBatches = new List<SelectListItem>();
            lstCollaborations = new List<SelectListItem>();
            lstStates = new List<SelectListItem>();
            lstYears = new List<SelectListItem>();
            lstSchemes = new List<SelectListItem>();
            lstSchemes.Insert(0, new SelectListItem { Value = "0" , Text = "Select Scheme"});
            lstSchemes.Insert(1, new SelectListItem { Value = "1", Text = "PMGSY I" });
            lstSchemes.Insert(2, new SelectListItem { Value = "2", Text = "PMGSY II" });
        }

        [Required(ErrorMessage="Please select State")]
        [Display(Name="State")]
        [Range(-1, Int32.MaxValue, ErrorMessage = "Please select State.")]
        public int State { get; set; }
        public List<SelectListItem> lstStates { get; set; }

        [Required(ErrorMessage="Please select Financial Year")]
        [Display(Name="Financial Year")]
        [Range(-1, 2020, ErrorMessage = "Please select valid Year.")]
        public int Year { get; set; }
        public List<SelectListItem> lstYears { get; set; }

        [Required(ErrorMessage="Please select Batch")]
        [Display(Name="Batch")]
        [Range(-1, Int32.MaxValue, ErrorMessage = "Please select Batch.")]
        public int Batch { get; set; }
        public List<SelectListItem> lstBatches { get; set; }

        [Required(ErrorMessage="Please select Collaboration")]
        [Display(Name="Collaboration")]
        [Range(-1, Int32.MaxValue, ErrorMessage = "Please select Collaboration.")]
        public int Collaboration { get; set; }
        public List<SelectListItem> lstCollaborations { get; set; }

        [Required(ErrorMessage="Please select Agency")]
        [Display(Name="Agency")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select Agency.")]
        public int Agency { get; set; }
        public List<SelectListItem> lstAgencies { get; set; }

        [Required(ErrorMessage = "Please select Scheme")]
        [Display(Name = "Scheme")]
        [Range(1, 2, ErrorMessage = "Please select Scheme.")]
        public int PMGSYScheme { get; set; }
        public List<SelectListItem> lstSchemes { get; set; }

    }
}