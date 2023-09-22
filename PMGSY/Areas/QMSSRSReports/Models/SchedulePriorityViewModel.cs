using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;


namespace PMGSY.Areas.QMSSRSReports.Models
{
    public class SchedulePriorityViewModel
    {
        public int priority { get; set; }
        public int ProgressMin { get; set; }
        public int CompletedMin { get; set; }
        public int ProgressMax { get; set; }
        public int CompletedMax { get; set; }
        public int FromMonth { get; set; }
        public int ToMonth { get; set; }
        public int StateCode { get; set; }
        public int DistrictCode { get; set; }
    }
}