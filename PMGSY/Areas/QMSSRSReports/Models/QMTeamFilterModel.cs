using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace PMGSY.Areas.QMSSRSReports.Models
{
    public class QMTeamFilterModel
    {


            public QMTeamFilterModel()
        {
            StateList = new List<SelectListItem>();
            ScheduleYearList = new List<SelectListItem>();
            ScheduleMonthList = new List<SelectListItem>();
                 MonitorList = new List<SelectListItem>();
        }

        [Display(Name = "State")]
        public int StateCode { get; set; }
        public List<SelectListItem> StateList { set; get; }

        [Display(Name = "Schedule Year")]
        public int ScheduleYear { get; set; }
        public List<SelectListItem> ScheduleYearList { get; set; }

        [Display(Name = "Schedule Month")]
        public int ScheduleMonth { get; set; }
        public List<SelectListItem> ScheduleMonthList { get; set; }

        [Display(Name = "Monitor")]
        public int MonitorCode { get; set; }
        public List<SelectListItem> MonitorList { get; set; }

        public int RoleID { get; set; }


    }
}