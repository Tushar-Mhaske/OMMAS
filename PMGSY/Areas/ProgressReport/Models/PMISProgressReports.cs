using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.ProgressReport.Models
{
    public class PMISProgressReports
    {
        public int LevelCode { get; set; }
        public string StateName { get; set; }
        public string DistName { get; set; }
        public int Mast_State_Code { get; set; }
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
    }
}