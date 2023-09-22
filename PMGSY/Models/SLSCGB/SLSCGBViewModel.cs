using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.SLSCGB
{
    public class SLSCGBViewModel
    {

        [Required(ErrorMessage = "Please select State")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid State")]
        [Display(Name = "State")]
        public int state { get; set; }
        public List<SelectListItem> stateList { get; set; }

        [Required(ErrorMessage = "Please select Meeting Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Date is not in valid format")]
        [Display(Name = "Meeting Date")]
        public string meetingDate { get; set; }

        [Required(ErrorMessage = "Please select either of the Meeting Type")]
        [RegularExpression("[SG]", ErrorMessage = "Please Select valid Meeting Type")]
        [Display(Name = "SLSC/GB")]
        public string meetingFlag { get; set; }

        public string FileName { get; set; }
        public string fileType { get; set; }
    }
}