using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace PMGSY.Models.Feedback
{
    public class DisplayFBFiles
    {
        //[Required(ErrorMessage="No Files for Selected News")]
        public List<SelectListItem> path { get; set; }

        public string IssuedBy { get; set; }
        public string IssuedDate { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string LatLong { get; set; }
        public decimal[] FileLat { get; set; }
        public decimal[] FileLong { get; set; }

        public bool IsLatLongAvailable { get; set; }
    }
}