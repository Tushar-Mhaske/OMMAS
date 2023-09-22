using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.MPRFileDownload.Models
{
    public class MPRFileUploadViewModel
    {
        [Display(Name = "State")]
        public int StateId { get; set; }
        public List<SelectListItem> StateList { get; set; }
    }
}