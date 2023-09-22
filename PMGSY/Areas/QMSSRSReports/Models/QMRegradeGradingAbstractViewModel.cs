using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.QMSSRSReports.Models
{
    public class QMRegradeGradingAbstractViewModel
    {
        [Required(ErrorMessage = "Please select state")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select a valid state")]
        public int agency { get; set; }
        public List<SelectListItem> agencyList { get; set; }

        [Required(ErrorMessage = "Please select collaboration")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select a valid collaboration")]
        public int collaboration { get; set; }
        public List<SelectListItem> collabList { get; set; }

        [Required(ErrorMessage = "Please select Quality Monitoring Type")]
        [RegularExpression(@"^([IS]+)$", ErrorMessage = "Please select a valid Quality Monitoring Type.")]
        public string qmType { get; set; }
        public List<SelectListItem> qmTypeList { get; set; }
    }
}