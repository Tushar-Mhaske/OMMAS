﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.REAT.Models
{
    public class REATDownloadXMLViewModel
    {
        [Required(ErrorMessage = "Please select State")]
        [Range(1, 50, ErrorMessage = "Please select a valid State")]
        public int stateCode { get; set; }
        public List<SelectListItem> lstState { set; get; }

        [Required(ErrorMessage = "Please select District")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select a valid District")]
        public int districtCode { get; set; }
        public List<SelectListItem> lstDistrict { set; get; }

        [Required(ErrorMessage = "Please select Agency")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid Agency")]
        public int agencyCode { get; set; }
        public List<SelectListItem> lstAgency { set; get; }

        public string[] mastContractorIds { set; get; }

        public bool isAllContractors { set; get; }

        public int Level { set; get; }

        [RegularExpression("[AUD]", ErrorMessage = "Please Select valid operation")]
        public string operation { get; set; }
    }

}