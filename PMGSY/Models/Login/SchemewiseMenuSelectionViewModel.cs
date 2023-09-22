using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Login
{
    public class SchemewiseMenuSelectionViewModel
    {
        [Range(1, short.MaxValue, ErrorMessage = "Please select valid Scheme")]
        public int pmgsyScheme { get; set; }
        public List<SelectListItem> lstPmgsyScheme { get; set; }
    }
}