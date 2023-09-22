using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace PMGSY.Models.Master
{
    public class PMGSYStatesViewModel
    {
        
        public int[] statecodes { get; set; }

        public string[] statesnames { get; set; }
        public List<SelectListItem> stateslist { get; set; }
     }
}