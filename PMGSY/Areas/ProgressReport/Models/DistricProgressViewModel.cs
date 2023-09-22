using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.ProgressReport.Models
{
    public class DistricProgressViewModel
    {
        public int LevelCode { get; set; }
        public string StateName { get; set; }
        public string DistName { get; set; }
        public string BlockName { get; set; }
        public int Mast_State_Code { get; set; }
        public int Mast_Block_Code { get; set; }
        public int Mast_District_Code { get; set; }
        
        //[Required(ErrorMessage = "Please select State. ")]
        //[Range(1, int.MaxValue, ErrorMessage = "Please select  State.")]
        [Display(Name = "State")]
        public int StateCode { get; set; }
        public List<SelectListItem> StateList { get; set; }

        //[Display(Name = "District")]

        //[Required(ErrorMessage = "Please select District.")]
        //[Range(0, int.MaxValue, ErrorMessage = "Please select District.")]
        //[RegularExpression(@"^([0-9]+)$", ErrorMessage = "District must be valid number.")]
        [Display(Name = "District")]
        public int DistrictCode { get; set; }
        public List<SelectListItem> DistrictList { get; set; }

        //[Display(Name = "Block")]

        //[Required(ErrorMessage = "Please select Block.")]
        //[Range(0, int.MaxValue, ErrorMessage = "Please select Block.")]
        // [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Block must be valid number.")]
        [Display(Name = "Block")]
        public int BlockCode { get; set; }
        public List<SelectListItem> BlockList { get; set; }

    }
}