using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.PackageAgreementSanctionList.Models
{
    public class MonthwiseAgreement
    {
        public string StateName { get; set; }
        public string DistName { get; set; }

        public int Mast_State_Code { get; set; }   
        public int Mast_District_Code { get; set; }

        [Display(Name = "State : ")]
        [Required(ErrorMessage = "Please select State. ")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select  State.")]
        public int StateCode { get; set; }
        public List<SelectListItem> StateList { get; set; }
        
        [Display(Name = "District : ")]
        [Required(ErrorMessage = "Please select District.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select District.")]
        public int DistrictCode { get; set; }
        public List<SelectListItem> DistrictList { get; set; }
        
        [Range(0, 4, ErrorMessage = "Please select PMGSY Scheme.")]
        public int SchemeCode { get; set; }
        public List<SelectListItem> SchemeList { get; set; }
    }
}