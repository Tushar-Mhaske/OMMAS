using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.OtherReports.Models
{
    public class BenefittedHabsFacilityViewModel
    {
        public string stateName { get; set; }

        [Range(1, 50, ErrorMessage = "Please select valid State")]
        public int stateCode { get; set; }
        public List<SelectListItem> lstState { get; set; }

        [Range(1, 15, ErrorMessage = "Please select valid Batch")]
        public int batchCode { get; set; }
        public List<SelectListItem> lstBatch { get; set; }

        [Range(2000, 2099, ErrorMessage = "Please select valid Year")]
        public int Year { get; set; }
        public List<SelectListItem> lstYear { get; set; }
    }
}