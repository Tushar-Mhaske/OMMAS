using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.RoadwiseQualityDetails.Models
{
    public class FilterDetailsViewModel
    {
        public string SearchType { get; set; }

        [Required]
        [Display(Name = "State")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select state")]
        public int State { get; set; }
        public List<SelectListItem> StateList { get; set; }

        [Required]
        [Display(Name = "District")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select district")]
        public int District { get; set; }
        public List<SelectListItem> DistrictList { get; set; }

        [Required]
        [Display(Name = "Block")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select block")]
        public int Block { get; set; }
        public List<SelectListItem> BlockList { get; set; }

        [Required]
        [Display(Name = "Road")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select Road.")]
        public int RoadCode { get; set; }
        public List<SelectListItem> RoadList { get; set; }
        
        public string ErrorMessage { get; set; }

    }
}