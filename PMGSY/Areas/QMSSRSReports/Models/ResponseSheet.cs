using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.Areas.QMSSRSReports.Models;
using PMGSY.Extensions;
using PMGSY.Common;
using PMGSY.Models;
using PMGSY.Controllers;
using System.IO;
using System.Configuration;
using PMGSY.DAL.Master;
using PMGSY.Areas.QMSSRSReports.DAL;
using System.ComponentModel.DataAnnotations;


namespace PMGSY.Areas.QMSSRSReports.Models
{
    public class ResponseSheet
    {
      
        
        public string ValueType { get; set; }

        

        public int hdnRole { get; set; }

        //public string qmType { get; set; }
        public int InspCount { get; set; }

        public string StateName { get; set; }
        public string DistName { get; set; }
        public string BlockName { get; set; }


        [Display(Name = "Scheme")]
        public int SchemeCode { get; set; }
        public List<SelectListItem> SchemeList { get; set; }

        [Display(Name = "Monitor")]
        public int MonitorCode { get; set; }
        public List<SelectListItem> MonitorList { get; set; }


        [Display(Name = "Inspection Month")]
        [Range(0, 12, ErrorMessage = "Please select valid From Month.")]
        public int FromMonth { get; set; }
        public List<SelectListItem> FromMonthList { get; set; }


        //[Display(Name = "To Month")]
        //[Range(0, 12, ErrorMessage = "Please select valid To Month.")]
        //public int ToMonth { get; set; }
        //public List<SelectListItem> ToMonthList { get; set; }


        [Display(Name = "Inspection Year")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid From Year.")]
        public int FromYear { get; set; }
        public List<SelectListItem> FromYearList { get; set; }

        //[Display(Name = "To Year")]
        //[DateValidationVST("FromYear", "FromMonth", "ToMonth", ErrorMessage = "To Year must be greater than or equal to From Year.")]
        //[Range(0, int.MaxValue, ErrorMessage = "Please select valid To Year.")]
        //public int ToYear { get; set; }
        //public List<SelectListItem> ToYearList { get; set; }

        [Display(Name = "State")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid State.")]
        public int State { get; set; }
        public List<SelectListItem> StateList { get; set; }

        [Display(Name = "District")]
        //[Required(ErrorMessage = "Please select a District.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid District.")]
        public int District { get; set; }
        public List<SelectListItem> DistrictList { get; set; }

        [Display(Name = "Block")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Block.")]
        public int Block { get; set; }
        public List<SelectListItem> BlockList { get; set; }

        [Display(Name = "Year")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Year.")]
        public int Year { get; set; }
        public List<SelectListItem> YearList { get; set; }

        [Display(Name = "Month")]
        [Range(0, 12, ErrorMessage = "Please select valid Month.")]
        public int Month { get; set; }
        public List<SelectListItem> MonthList { get; set; }

  

   



        public int Monitor { get; set; }

    }
}