using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.QMSSRSReports.Models
{
    public class ContractorWiseReportModel
    {
        [Display(Name = "Inspection Year")]
        [Required]
        [RegularExpression("^[0-9]+$",ErrorMessage="Inspection Year is not in valid format")]
        public int YEAR_INSPECTION { get; set; }
        public List<SelectListItem> YEAR_INSPECTION_LIST { get; set; }
    }
}