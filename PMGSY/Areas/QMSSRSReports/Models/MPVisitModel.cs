using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace PMGSY.Areas.QMSSRSReports.Models
{
    public class MPVisitModel
    {
        public MPVisitModel()
        {
            this.StateList = new List<SelectListItem>();
            this.DistrictList = new List<SelectListItem>();
            this.YearList = new List<SelectListItem>();
            this.VisitYearList = new List<SelectListItem>();
        }

        [Display(Name="State")]
        [Required(ErrorMessage="Please select state")]
        [Range(0, 36, ErrorMessage = "Please select state")]
        public int StateCode { get; set; }
        public List<SelectListItem> StateList { get; set; }

        [Display(Name = "District")]
        [Required(ErrorMessage = "Please select district")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Please select district")]
        public int DistrictCode { get; set; }
        public List<SelectListItem> DistrictList { get; set; }

        [Display(Name = "Year")]
        [Required(ErrorMessage = "Please select year")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select year")]
        public int YearCode { get; set; }
        public List<SelectListItem> YearList { get; set; }

        [Display(Name = "Visit Year")]
        [Required(ErrorMessage = "Please select visit year")]
        [Range(0,int.MaxValue, ErrorMessage = "Please select visit year")]
        public int VisitYearCode { get; set; }
        public List<SelectListItem> VisitYearList { get; set; }


    }
}