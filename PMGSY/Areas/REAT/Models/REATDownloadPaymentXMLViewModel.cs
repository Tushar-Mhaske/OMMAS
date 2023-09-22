using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.REAT.Models
{
    public class REATDownloadPaymentXMLViewModel
    {
        [Required(ErrorMessage = "Please select State")]
        [Range(1, 50, ErrorMessage = "Please select a valid State")]
        public int stateCode { get; set; }
        public List<SelectListItem> lstState { set; get; }

        [Required(ErrorMessage = "Please select Agency")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid Agency")]
        public int agencyCode { get; set; }
        public List<SelectListItem> lstAgency { set; get; }

        [Required(ErrorMessage = "Please select date")]
        [DataType(DataType.DateTime)]
        public String generationDate { get; set; }

        [Required(ErrorMessage = "Please provide file type")]
        [RegularExpression(@"^([NR]+)$", ErrorMessage = "Please select valid file type")]
        public string FileType { get; set; }

        public Int64 billId { get; set; }
    }
}