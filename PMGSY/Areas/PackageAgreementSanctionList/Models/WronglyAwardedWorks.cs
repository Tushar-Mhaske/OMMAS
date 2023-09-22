using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.PackageAgreementSanctionList.Models
{
    public class WronglyAwardedWorksViewModel
    {
        [Display(Name="State")]
        [Required(ErrorMessage = "Please select State.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select State.")]
        public int stateCode { get; set; }
        public List<SelectListItem> lstState { get; set; }

        [Display(Name = "District")]
        [Required(ErrorMessage = "Please select District.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select District.")]
        public int districtCode { get; set; }
        public List<SelectListItem> lstDistrict { get; set; }

        //[Display(Name = "Year")]
        //[Required(ErrorMessage = "Please select Year.")]
        //[Range(0, 2090, ErrorMessage = "Please select Year.")]
        //public int yearCode { get; set; }
        //public List<SelectListItem> lstYear { get; set; }

        [Range(1, 3, ErrorMessage = "Please select PMGSY Scheme.")]
        public int pmgsyScheme { get; set; }
    }
}