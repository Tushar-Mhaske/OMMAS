using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using PMGSY.Common;

namespace PMGSY.Areas.WorkAwardedArea.Models
{
    public class WorkAwardedModel
    {
        [Display(Name="State")]
        public string State { get; set; }

        [Display(Name = "District")]
        public string District { get; set; }

        [Required(ErrorMessage = "Collaboration required")]
        [Range(-1, int.MaxValue, ErrorMessage = "Invalid collaboration")]
        [Display(Name = "Collaboration")]
        public string Collab { get; set; }
        public List<SelectListItem> CollabList { get; set; }

        [Required(ErrorMessage = "Year required")]
        [Range(0, int.MaxValue, ErrorMessage = "Invalid year")]
        [Display(Name = "Year")]
        public int Year { get; set; }
        public List<SelectListItem> YearList { get; set; }

        public string StateName { get; set; }
        public string DistName { get; set; }
        public string BlockName { get; set; }
        public int Mast_State_Code { get; set; }
        public int Mast_Block_Code { get; set; }
        public int Mast_District_Code { get; set; }
        public int LevelCode { get; set; }

        [Display(Name = "State")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select  State.")]
        public int StateCode { get; set; }
        public List<SelectListItem> StateList { get; set; }

        [Display(Name = "District")]
        [Range(-1, int.MaxValue, ErrorMessage = "Please select District.")]
        public int DistrictCode { get; set; }
        public List<SelectListItem> DistrictList { get; set; }
  
        [Display(Name = "Block")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select Block.")]
        public int BlockCode { get; set; }
        public List<SelectListItem> BlockList { get; set; }

        public string CollaborationName { get; set; }
        public string YearName { get; set; }



        [Display(Name="Road Wise")]
        public bool RoadWise { get; set; }
    }

}