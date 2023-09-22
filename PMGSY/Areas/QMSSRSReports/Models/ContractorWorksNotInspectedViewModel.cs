using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.QMSSRSReports.Models
{
    public class ContractorWorksNotInspectedViewModel
    {
        public string StateName { get; set; }

        [Required(ErrorMessage = "Please select From Year. ")]
        [Range(2000, 2099, ErrorMessage = "Please select valid From Year.")]
        public int fromYear { get; set; }
        public List<SelectListItem> lstFromYear { get; set; }

        [Required(ErrorMessage = "Please select To Year. ")]
        [Range(2000, 2099, ErrorMessage = "Please select valid To Year.")]
        public int toYear { get; set; }
        public List<SelectListItem> lstToYear { get; set; }

        [Required(ErrorMessage = "Please select State. ")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select  State.")]
        public int StateCode { get; set; }
        public List<SelectListItem> lstState { get; set; }

    }
}