using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.QualityMonitoring
{
    public class AllocateRoadsToNQMModel
    {

        [Display(Name = "State")]
        [Required(ErrorMessage = "State is Required")]
        public Int32 STATEID { get; set; }
        public List<SelectListItem> STATE_LIST { get; set; }


        [Display(Name = "Month")]
        [Required(ErrorMessage = "Month is Required")]
        public Int32 MONTHID { get; set; }
        public List<SelectListItem> MONTH_LIST { get; set; }


        [Display(Name = "Year")]
        [Required(ErrorMessage = "Year is Required")]
        public Int32 YEARID { get; set; }
        public List<SelectListItem> YEAR_LIST { get; set; }

        public Int32 page { get; set; }
        public Int32 rows { get; set; }
        public String sidx { get; set; }
        public String sord { get; set; }
    }
}