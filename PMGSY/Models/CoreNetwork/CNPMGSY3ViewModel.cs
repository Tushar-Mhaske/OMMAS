using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.CoreNetwork
{
    public class CNPMGSY3ViewModel
    {
     
        [Range(1, short.MaxValue, ErrorMessage = "Please select valid State")]
        public int StateCode { get; set; }
        public List<SelectListItem> lstStateCode { get; set; }

        [Range(1, short.MaxValue, ErrorMessage = "Please select valid District")]
        public int DistrictCode { get; set; }
        public List<SelectListItem> lstDistrictCode { get; set; }

        [Range(1, short.MaxValue, ErrorMessage = "Please select valid Block")]
        public int BlockCode { get; set; }
        public List<SelectListItem> lstBlockCode { get; set; }

        [Range(0, short.MaxValue, ErrorMessage = "Please select valid Category")]
        public int Category { get; set; }
        public List<SelectListItem> lstCategory { get; set; }

        [Range(0, short.MaxValue, ErrorMessage = "Please select valid Route type")]
        public int RouteType { get; set; }
        public List<SelectListItem> lstRouteType { get; set; }

        [Range(1, 4, ErrorMessage = "Please select valid Pmgsy Scheme")]
        public int PmgsyScheme { get; set; }

        public bool isUnlocked { get; set; }

        public bool isPMGSY3 { get; set; }

        public bool isPMGSY3Finalized { get; set; }

        //added by abhinav pathak
        public bool isBlockFinalized { get; set; }
    }
}