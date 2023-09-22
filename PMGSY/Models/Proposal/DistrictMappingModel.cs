using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Proposal
{
    public class DistrictMappingModel
    {

        [Range(0, int.MaxValue, ErrorMessage = "Please select Cluster.")]
        public int DistrictCode { get; set; }
        public List<SelectListItem> DistrictList { get; set; }

        public string hiddenDistrictCode { get; set; }
        public bool isStateEntered { get; set; }
    }
}