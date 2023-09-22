using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace PMGSY.Areas.ProposalSSRSReports.Models
{
    public class WronglyMappedHabitaions
    {
        [Range(0, 2147483647, ErrorMessage = "Please select state.")]
        [RegularExpression("^([0-9]+)$", ErrorMessage = "State must be valid number.")]
        [Display(Name = "State : ")]
        public int StateCode { get; set; }
        public List<SelectListItem> lstStates { get; set; }

        public String StateName { get; set; }

        public int LevelID { get; set; }
    }
}