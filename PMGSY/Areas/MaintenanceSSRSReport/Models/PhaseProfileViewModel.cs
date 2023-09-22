using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.MaintenanceSSRSReport.Models
{
    public class PhaseProfileViewModel
    {
        [Display(Name = "State")]
        [Required(ErrorMessage = "Please select State.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select State.")]
        public int StateCode { get; set; }
        public List<SelectListItem> StateList { get; set; }

        [Display(Name = "District")]
        [Required(ErrorMessage = "Please select District.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select District.")]
        public int DistrictCode { get; set; }
        public List<SelectListItem> DistrictList { get; set; }

        [Display(Name = "Block")]
        [Required(ErrorMessage = "Please select Block.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select Block.")]
        public int BlockCode { get; set; }
        public List<SelectListItem> BlockList { get; set; }

        [Display(Name = "Month")]
        [Required(ErrorMessage = "Please select Month.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select Month.")]
        public int Month { get; set; }
        public List<SelectListItem> MonthList { get; set; }
        
        [Display(Name = "Sanction Year")]
        [Required(ErrorMessage = "Please select Year.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select Year.")]
        public int Year { get; set; }
        public List<SelectListItem> YearList { get; set; }

        [Display(Name = "Agency")]
        [Required(ErrorMessage = "Please select Agency.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select Agency.")]
        public int Agency { get; set; }
        public List<SelectListItem> AgencyList { get; set; }

        [Display(Name = "Collaboration")]
        [Required(ErrorMessage = "Please select Collaboration.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select Collaboration.")]
        public int CollabCode { get; set; }
        public List<SelectListItem> CollabList { get; set; }

        public int Level { get; set; }
        public string StateName { get; set; }
        public string DistrictName { get; set; }
        public string BlockName { get; set; }
        public string MonthName { get; set; }
        public string YearName { get; set; }
        public string AgencyName { get; set; }
        public string CollabName { get; set; }

    }
}