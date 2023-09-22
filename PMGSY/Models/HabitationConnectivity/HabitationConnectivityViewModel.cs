using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using PMGSY.Models;
using System.Web.Mvc;


namespace PMGSY.Models.HabitationConnectivity
{
    public class HabitationConnectivityViewModel
    {
        [Display(Name = "State")]
        [Range(1, 2147483647, ErrorMessage = "Please select State")]
        public string State { get; set; }

        [Display(Name = "Distict")]
        [Range(1, 2147483647, ErrorMessage = "Please select Distict")]
        public string District { get; set; }


        [Display(Name = "Block")]
        [Range(1, 2147483647, ErrorMessage = "Please select Block")]
        public string Block { get; set; }





    }
}