using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace PMGSY.Models.Master
{
    public class ViewAdminTechnicalAgencyMapping
    {
        [Display(Name = "Agency Type")]
        public int Agency { get; set; }
        public List<SelectListItem> AgencyList { set; get; }

        [Display(Name = "State")]
        public int State { get; set; }
        public List<SelectListItem> State_List { set; get; }

        [Display(Name = "District")]
        public int District { get; set; }
        public List<SelectListItem> District_List { set; get; }
    }
}