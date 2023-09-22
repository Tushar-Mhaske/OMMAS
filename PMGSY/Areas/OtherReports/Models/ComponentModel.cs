using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace PMGSY.Areas.OtherReports.Models
{
    public class ComponentModel
    {

        [Display(Name = "State")]
        [Range(1,int.MaxValue, ErrorMessage="Please select valid State")]
        public int StateCode { get; set; }
        public List<SelectListItem> StateList { get; set; }


        [Display(Name = "District")]
        [Range(0,int.MaxValue, ErrorMessage="Please select valid District")]
        public int DistrictCode { get; set; }
        public List<SelectListItem> DistrictList { get; set; }


        [Display(Name = "Block")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Block")]
        public int BlockCode { get; set; }
        public List<SelectListItem> BlockList { get; set; }

        [Display(Name = "Batch")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Batch")]
        public int BatchCode { get; set; }
        public List<SelectListItem> BatchList { get; set; }


        [Display(Name = "Year")]
        [Range(2000, 2999, ErrorMessage = "Please select valid Year")]
        public int Year { get; set; }
        public List<SelectListItem> YearList { get; set; }

        [Range(1, 4, ErrorMessage = "Please select valid Scheme")]
        public int schemeCode { get; set; }

        [RegularExpression("[0YN]", ErrorMessage = "Please All or Yes or No")]
        public string STAStatus { get; set; }
        public List<SelectListItem> lstSTAStatus { get; set; }
    }
}