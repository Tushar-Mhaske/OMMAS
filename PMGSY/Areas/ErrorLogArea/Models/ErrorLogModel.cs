using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.ErrorLogArea.Models
{
    public class ErrorLogModel
    {
        [Display(Name = "Module Name")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid Module Name")]
        public int ModuleID { get; set; }
        public List<SelectListItem> lstModule { set; get; }

        [Required(ErrorMessage = "Please Select Date")]
        public string LogDate { set; get; }
    }
}