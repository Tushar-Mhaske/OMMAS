using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.PackageAgreementSanctionList.Models
{
    public class Issues
    {
        public string StateName { get; set; }

        public int Mast_State_Code { get; set; }
 


        [Display(Name = "State : ")]
        [Required(ErrorMessage = "Please select State. ")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select  State.")]

        public int StateCode { get; set; }
        public List<SelectListItem> StateList { get; set; }

       

    }
}