using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace PMGSY.Areas.StateProfileReports.Models
{
    public class SPFilters
    {

        [Display(Name = "State")]
        public int StateId { get; set; }
        public List<SelectListItem> StateList { get; set; }

        //[Display(Name="District")]
        //public int DistrictId { get; set; }
        //public List<SelectListItem> DistrictList { get; set; }

        //[Display(Name = "Collaboration")]
        //public int Collaboration { get; set; }
        //public List<SelectListItem> CollaborationList { get; set; }

        public int LevelCode { get; set; }

        [Display(Name = "DPIU")]
        public int DPIUCode { get; set; }
        public List<SelectListItem> DPIUList { get; set; }

        [Display(Name = "Agency")]
        [Range(1,Int32.MaxValue,ErrorMessage="Please Select Agency.")]
        public int Agency { get; set; }
        public List<SelectListItem> AgencyList { get; set; }
    }
}



