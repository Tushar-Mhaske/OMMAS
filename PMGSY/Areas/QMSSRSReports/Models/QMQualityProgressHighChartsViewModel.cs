using PMGSY.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.QMSSRSReports.Models
{
    public class QMQualityProgressHighChartsViewModel
    {

        [Display(Name = "State")]
        public int StateCode { get; set; }
        public List<SelectListItem> StateList { get; set; }

        [Display(Name = "Year")]
        [Range(1,int.MaxValue)]
        public int Year { get; set; }
        public List<SelectListItem> YearList { get; set; }

        public List<USP_QUALITY_PROFILE_GRAPH_Result> List { get; set; }

        
    }
}



