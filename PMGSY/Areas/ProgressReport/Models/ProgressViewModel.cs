using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.ProgressReport.Models
{
    public class ProgressViewModel
    {
        public ProgressViewModel()
        {
            PhotoTypeList = new List<SelectListItem>()
            {
                new SelectListItem(){Text="Select Type",Value="0"},
                new SelectListItem(){Text="Work Photo",Value="1"},
                new SelectListItem(){Text="Lab Photo",Value="2"}
                
            };
        }
    
        [Display(Name = "State")]
        [Required(ErrorMessage = "Please select State.")]
        [Range(-1, int.MaxValue, ErrorMessage = "Please select State.")]
        public int StateCode { get; set; }
        public List<SelectListItem> StateList { get; set; }

        [Display(Name = "District")]
        [Required(ErrorMessage = "Please select District.")]
        [Range(-1, int.MaxValue, ErrorMessage = "Please select District.")]
        public int DistrictCode { get; set; }
        public List<SelectListItem> DistrictList { get; set; }

        

        [Display(Name = "Month")]
        [Required(ErrorMessage = "Please select Month.")]
        [Range(1, 12, ErrorMessage = "Please select Month.")]
        public int Month { get; set; }
        public List<SelectListItem> MonthList { get; set; }

        [Display(Name = "Year")]
        [Required(ErrorMessage = "Please select Year.")]
        [Range(-1, int.MaxValue, ErrorMessage = "Please select Year.")]
        public int Year { get; set; }
        public List<SelectListItem> YearList { get; set; }


        [Display(Name = "Photo Type")]
        [Required(ErrorMessage = "Please select Type.")]
        [Range(1, 2, ErrorMessage = "Please select Type.")]
        public int PhotoType { get; set; }
        public List<SelectListItem> PhotoTypeList { get; set; }

        public int Level { get; set; }
        

    }

    public class AwardedInProgressRoadworksViewModel
    {
        public int Level { get; set; }
        [Range(0, 5, ErrorMessage = "Please select PMGSY Scheme.")]
        public int SchemeCode { get; set; }
        public List<SelectListItem> SchemeList { get; set; }

        [Required(ErrorMessage = "Please select State. ")]
        [Range(-1, int.MaxValue, ErrorMessage = "Please select  State.")]
        public int StateCode { get; set; }
        public List<SelectListItem> StateList { get; set; }

        //[Range(0, 5, ErrorMessage = "Please select Phase.")]
        public string PhaseCode { get; set; }
        public List<SelectListItem> PhaseList { get; set; }

    }

}