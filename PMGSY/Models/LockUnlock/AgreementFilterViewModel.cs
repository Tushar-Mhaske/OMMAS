using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.LockUnlock
{
    public class AgreementFilterViewModel
    {
        public int year { get; set; }
        public List<SelectListItem> Years { get; set; }

        public int state { get; set; }
        public List<SelectListItem> States { get; set; }

        public int district { get; set; }
        public List<SelectListItem> Districts { get; set; }

        public int block { get; set; }
        public List<SelectListItem> Blocks { get; set; }

        public int ModuleCode { get; set; }

        public string SubModuleCode { get; set; }
    }
}