using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Accounts
{
    public class AccountingATRModel
    {  
        
        [Display(Name = "State")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select State")]
        public int StateCode { get; set; }
        public List<SelectListItem> lstStates { get; set; }

        [Display(Name = "Agency")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select Agency")]
        public int Agency { get; set; }
        public List<SelectListItem> lstAgency { get; set; }

        //[RegularExpression(@"^([0-9]+)$", ErrorMessage = "Agency should contains digits only.")]
        [Display(Name = "Year")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select Financial year")]
        public int Year { get; set; }
        public List<SelectListItem> lstYear { get; set; }


    }
}