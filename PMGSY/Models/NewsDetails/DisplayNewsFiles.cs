using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace PMGSY.Models.NewsDetails
{
    public class DisplayNewsFiles
    {
        //[Required(ErrorMessage="No Files for Selected News")]
        public List<SelectListItem> path { get; set; }

        public string IssuedBy { get; set; }
        public string IssuedDate { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

    }
}