using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.GPSVTSInstallationDetails.Models
{
    public class GPSVTS_FiltersModel
    {
        [Display(Name = "State")]
        [Required(ErrorMessage = "Please Select State")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Please select State.")]
        public int StateCode { get; set; }
        public List<SelectListItem> StateList { get; set; }


        [Display(Name = "District")]
        [Required(ErrorMessage = "Please Select District")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Please select District.")]
        public int DistrictCode { get; set; }
        public List<SelectListItem> DistrictList { get; set; }

        [Display(Name = "Block")]
        [Required(ErrorMessage = "Please Select Block")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Please select Block.")]
        public int BlockCode { get; set; }
        public List<SelectListItem> BlockList { get; set; }

        [Display(Name = "Batch")]
        [Range(0, 10, ErrorMessage = "Please select Batch.")]
        [Required(ErrorMessage = "Please select Batch.")]
        [RegularExpression("^([0-9]+)$", ErrorMessage = "Batch must be valid number.")]
        public int Batch { get; set; }
        public List<SelectListItem> BatchList { get; set; }

        [Display(Name = "Year")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Year.")]
        public int Sanction_Year { get; set; }
        public List<SelectListItem> Sanction_Year_List { get; set; }

        [Display(Name = "Work Type")]
        [Required(ErrorMessage = "Please select work type.")]
        public string ProposalType { get; set; }
        public List<SelectListItem> ProposalTypeList { get; set; }


        [Display(Name = "Work Status")]
        [Required(ErrorMessage = "Please select work status")]
        public string WorkStatus { get; set; }
        public List<SelectListItem> WorkStatusList { get; set; }


    }
}