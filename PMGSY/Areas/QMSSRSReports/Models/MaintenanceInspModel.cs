using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.QMSSRSReports.Models
{
    public class MaintenanceInspModel
    {
        [Display(Name="State")]
        [Required(ErrorMessage="Please select a state")]
        [RegularExpression("^[0-9]+$",ErrorMessage="Invalid State Selection")]
        public int StateCode { get; set; }

        [Display(Name = "District")]
        [Required(ErrorMessage = "Please select a District")]
        [RegularExpression("^[0-9]+$",ErrorMessage="Invalid District Selection")]
        public int DistrictCode { get; set; }

        [Display(Name = "Monitor Type")]
        [Required(ErrorMessage = "Please select Type of Monitor")]
        [RegularExpression("^[a-zA-Z]+$",ErrorMessage="Invalid Type of Monitor")]
        public string MonitorType { get; set; }

        [Display(Name = "From Year")]
        [Required(ErrorMessage = "Please select From Year")]
        [RegularExpression("^[0-9]+$",ErrorMessage="Invalid From Year")]
        public int FromYear { get; set; }

        [Display(Name = "To Year")]
        [Required(ErrorMessage = "Please select To Year")]
        [RegularExpression("^[0-9]+$",ErrorMessage="Invalid To Year")]
        public int ToYear { get; set; }

        [Display(Name = "From Month")]
        [Required(ErrorMessage = "Please select From Month")]
        [RegularExpression("^[0-9]+$",ErrorMessage="Invalid From Month")]
        public int FromMonth { get; set; }

        [Display(Name = "To Month")]
        [Required(ErrorMessage = "Please select To Month")]
        [RegularExpression("^[0-9]+$",ErrorMessage="Invalid To Month")]
        public int ToMonth { get; set; }

        [Display(Name = "Grade")]
        [Required(ErrorMessage = "Please select Grade")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Invalid Grade")]
        public string Grade { get; set; }

        public string StateName { get; set; }


        public string DistrictName { get; set; }

        public List<SelectListItem> StateList { get; set; }
        public List<SelectListItem> DistrictList { get; set; }
        public List<SelectListItem> YearList { get; set; }
        public List<SelectListItem> MonthList { get; set; } 


    }
}