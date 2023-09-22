using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Fund
{
    public class FundReleaseFilterViewModel
    {

        public int RoleCode { get; set; }

        public int? State { get; set; }
        public List<SelectListItem> States { get; set; }

        public string FundType { get; set; }
        public List<SelectListItem> FundTypes { get; set; }

        public int? Year { get; set;}
        public List<SelectListItem> Years { get; set; }

        public int? Collaboration { get; set; }
        public List<SelectListItem> Collaborations { get; set; }

        public string UrlParameter { get; set; }

        public string ReleaseBy { get; set; }

    }
}