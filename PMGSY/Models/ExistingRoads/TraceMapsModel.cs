using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.ExistingRoads
{
    public class TraceMapsModel
    {
        public List<SelectListItem> DistrictList { get; set; }
        [Required]
        [Range(0 , int.MaxValue , ErrorMessage = "Please Select District")]
        [Display(Name = "District")]
        public int DistrictCode { get; set; }
        
        public List<SelectListItem> BlockList { get; set; }
        public int BlockCode { get; set; }

        public int StateCode { get; set; }
        public List<SelectListItem> StateList { get; set; }

    }
}