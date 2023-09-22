using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.Common;

namespace PMGSY.Areas.AccountReports.Models
{
    public class FundPostionModel
    {
       // public int fyear { get; set; }

        [Display(Name = "Financial Year: ")]
        //[DisplayName("Year")]
        [Range(2000, Int32.MaxValue, ErrorMessage = "Please Select Year")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Please Select valid year")]
        public Int16 Year { get; set; }

        public List<SelectListItem> YearList { get; set; }

    }
}