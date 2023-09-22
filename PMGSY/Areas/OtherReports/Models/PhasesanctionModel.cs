using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
 

namespace PMGSY.Areas.OtherReports.Models
{
    public class PhaseSanctionModel
    {

        [Range(0, int.MaxValue, ErrorMessage = "Please select valid State.")]
        public int StateCode { get; set; }
        public List<SelectListItem> StateList { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Year.")]
        public int Year { get; set; }
        public List<SelectListItem> YearList { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Batch.")]
        public int Batch { get; set; }
        public List<SelectListItem> BatchList { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid PMGSY scheme.")]
        public int PMGSY { get; set; }
        public List<SelectListItem> PMGSYList { get; set; }

    }
}