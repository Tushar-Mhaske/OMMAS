using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.OtherReports.Models
{
    public class PhaseProfileModel
    {   
        [Display(Name="State")]
        [Range(1,int.MaxValue,ErrorMessage="Please Select State.")]
        [Required(ErrorMessage="Please select State.")]
        public int StateCode { get; set; }
        public List<SelectListItem> StateList { get; set; }

        [Display(Name = "Agency")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select Agency.")]
        public int AgencyCode { get; set; }
        public List<SelectListItem> AgencyList { get; set; }

        [Display(Name = "Collaboration")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select Collabration.")]
        public int Collaboration { get; set; }
        public List<SelectListItem> ColaborationList { get; set; }

        //[Display(Name = "PMGSY")]
        //[Required(ErrorMessage="Please select PMGSY scheme.")]
        //[Range(1, 2, ErrorMessage = "Please select PMGSY scheme.")]
        //public int PMGSY { get; set; }
        //public List<SelectListItem> PMGSYList { get; set; }
    }
}