using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.CoreNetwork
{
    public class PCIEntryViewModel
    {
        [Range(1 , int.MaxValue , ErrorMessage="Please select the year")]
        public int Year { get; set; }
        public int BlockID { get; set; }
        public char RoadType {get;set;}
        public List<SelectListItem> FyearList { get; set; }
        public List<SelectListItem> BlockList { get; set; }
        public List<SelectListItem> RoadtypeList { get; set; }
        public List<SelectListItem> DistrictList { get; set; }
        public int DistrictID { get; set; }
    }
}