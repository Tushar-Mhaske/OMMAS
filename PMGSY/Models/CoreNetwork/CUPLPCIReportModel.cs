using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.CoreNetwork
{
    public class CUPLPCIReportModel
    {
        public int StateCode { get; set; }
        public int DistrictCode { get; set; }
        public int BlockID { get; set; }
        public char RoadType { get; set; }
        public List<SelectListItem> FyearList { get; set; }
        public List<SelectListItem> StateList { get; set; }
        public List<SelectListItem> DistrictList { get; set; }
        public List<SelectListItem> BlockList { get; set; }

        public string statename { get; set; }
        public string Districtname { get; set; }
        public string Blockname { get; set; }

        public List<SelectListItem> RoadtypeList { get; set; }
    }
}