using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.ProgressReport.Models
{
    public class DynamicSQL
    {

       

        [Display(Name = "SQL Query Statement")]
        //[StringLength(1000, ErrorMessage = "SQL Query should be maximum 10000 characters.")]
        [Required(ErrorMessage = "Please enter SQL Query Statement.")]
     //   [RegularExpression(@"^([a-zA-Z0-9-._/(), ]+)$", ErrorMessage = "Invalid Item Name.")]
        public string Query { get; set; }

    }
}