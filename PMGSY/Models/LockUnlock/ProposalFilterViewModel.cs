using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PMGSY.Models.LockUnlock
{
    public class ProposalFilterViewModel
    {
        [Display(Name="Year")]
        public int Year { get; set; }

        [Display(Name = "State")]
        public int State { get; set; }

        [Display(Name = "Batch")]
        public int? Batch { get; set; }

        [Display(Name = "Package")]
        public string Package { get; set; }

        [Display(Name = "District")]
        public int? District { get; set; }

        [Display(Name = "ModuleCode")]
        public int ModuleCode { get; set; }

        [Display(Name = "SubModuleCode")]
        public string SubModuleCode { get; set; }

        [Display(Name = "LockStatus")]
        public string LockStatus { get; set; }
    }
}