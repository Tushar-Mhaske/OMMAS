using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace PMGSY.Models.Feedback.FeedbackDetails
{
    public class DetailsFB
    {
        public int hdnRoleId { get; set; }

        [Display(Name = "Month")]
        [Range(1, int.MaxValue, ErrorMessage = "Please Select Month")]
        public int MONTHs { get; set; }
        public List<SelectListItem> Months_List { set; get; }

        [Display(Name = "Year")]
        //[Range(1, int.MaxValue, ErrorMessage = "Please Select Year")]
        public int YEARs { get; set; }
        public List<SelectListItem> Years_List { set; get; }

        [Display(Name = "State")]
        public int State { get; set; }
        public List<SelectListItem> State_List { set; get; }

        [Display(Name = "Category")]
        public string Category { get; set; }
        public List<SelectListItem> Category_List { set; get; }

        [Display(Name = "Approved")]
        public string Approved { get; set; }
        public List<SelectListItem> Approved_List { set; get; }

        [Display(Name = "Status")]
        public string Status { get; set; }
        public List<SelectListItem> Status_List { set; get; }

        [RegularExpression("[0MW]", ErrorMessage = "Please Select feedback through")]
        //[Range(0, int.MaxValue, ErrorMessage = "Please Select feedback through")]
        [Display(Name = "Feedback Through")]
        public string feedbackThrough { get; set; }
    }
}