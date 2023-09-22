using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.QMSSRSReports.Models
{
    public class QMRegradeGradingAbstractViewModel1
    {
        //[Required(ErrorMessage = "Please select state")]
        //[Range(0, int.MaxValue, ErrorMessage = "Please select a valid state")]
        //public int agency { get; set; }
        //public List<SelectListItem> agencyList { get; set; }

        //[Required(ErrorMessage = "Please select collaboration")]
        //[Range(0, int.MaxValue, ErrorMessage = "Please select a valid collaboration")]
        //public int collaboration { get; set; }
        //public List<SelectListItem> collabList { get; set; }

        //[Required(ErrorMessage = "Please select Quality Monitoring Type")]
        //[RegularExpression(@"^([IS]+)$", ErrorMessage = "Please select a valid Quality Monitoring Type.")]
        //public string qmType { get; set; }
        //public List<SelectListItem> qmTypeList { get; set; }



        [Display(Name = "State")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid State.")]
        public int State { get; set; }
        public List<SelectListItem> StateList { get; set; }

        [Display(Name = "From Month")]
        [Range(1, 12, ErrorMessage = "Please select a valid From Month")]
        public int fromMonth { get; set; }
        public List<SelectListItem> lstFromMonth { set; get; }

        [Display(Name = "To Month")]
        [Range(1, 12, ErrorMessage = "Please select a valid To Month")]
        public int toMonth { get; set; }
        public List<SelectListItem> lstToMonth { set; get; }

        [Display(Name = "From Year")]
        [Range(2000, 2099, ErrorMessage = "Please select a valid From Year")]
        public int fromYear { get; set; }
        public List<SelectListItem> lstFromYear { set; get; }

        [Display(Name = "To Year")]
        [Range(2000, 2099, ErrorMessage = "Please select a valid To Year")]
        public int toYear { get; set; }
        public List<SelectListItem> lstToYear { set; get; }
    }
}