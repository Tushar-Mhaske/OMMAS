using PMGSY.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.ProposalSSRSReports.Models
{
    public class DRRPRoadwiseReportModel
    {
        public DRRPRoadwiseReportModel()
        {
            StateList = new List<SelectListItem>();
            DistrictList = new List<SelectListItem>();
            BlockList = new List<SelectListItem>();
            SoilTypeList = new List<SelectListItem>();
            TerrainTypeList = new List<SelectListItem>();
            CategoryList = new List<SelectListItem>();
        }


        [Display(Name = "State")]
        [Required(ErrorMessage = "Please select State. ")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select  State.")]
        public int StateCode { get; set; }
        public string StateName { get; set; }
        public List<SelectListItem> StateList { get; set; }

        [Display(Name = "District")]
        [Required(ErrorMessage = "Please select District.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select District.")]
        public int DistrictCode { get; set; }
        public string DistrictName { get; set; }
        public List<SelectListItem> DistrictList { get; set; }

        [Display(Name = "Block")]
        [Required(ErrorMessage = "Please select Block.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select Block.")]
        public int BlockCode { get; set; }
        public string BlockName { get; set; }
        public List<SelectListItem> BlockList { get; set; }

        [Display(Name = "Category")]
        [Required(ErrorMessage = "Please select Category")]
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
        public List<SelectListItem> CategoryList { get; set; }

        [Display(Name="Soil Type")]
        [Required(ErrorMessage="Please select Soil Type.")]
        [Range(0,Int32.MaxValue,ErrorMessage="Please select Soil Type")]
        public int SoilType { get; set; }
        public string SoilTypeName { get; set; }
        public List<SelectListItem> SoilTypeList { get; set; }

        [Display(Name = "Terrain Type")]
        [Required(ErrorMessage = "Please select Terrain Type.")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Please select Terrain Type")]
        public int TerrainType { get; set; }
        public string TerrainTypeName { get; set; }
        public List<SelectListItem> TerrainTypeList { get; set; }

    }
}