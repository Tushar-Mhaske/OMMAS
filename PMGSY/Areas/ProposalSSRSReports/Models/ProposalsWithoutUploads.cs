using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.ProposalSSRSReports.Models
{
    public class ProposalsWithoutUploads
    {
        [Range(0, int.MaxValue, ErrorMessage = "Please select state.")]
        [RegularExpression("^([0-9]+)$", ErrorMessage = "State must be valid number.")]
        [Display(Name = "State")]
        public int State { get; set; }
        public List<SelectListItem> lstStates { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Please select District.")]
        [RegularExpression("^([0-9]+)$", ErrorMessage = "District must be valid number.")]
        [Display(Name = "District")]
        public int District { get; set; }
        public List<SelectListItem> lstDistrict { get; set; }

        //[Range(0, int.MaxValue, ErrorMessage = "Please select Block.")]
        //[RegularExpression("^([0-9]+)$", ErrorMessage = "Block must be valid number.")]
        //[Display(Name = "Block")]
        //public int Block { get; set; }
        //public List<SelectListItem> lstBlock { get; set; }

        [Display(Name = "Year")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select Year.")]
        public int Year { get; set; }
        public List<SelectListItem> YearList { get; set; }

        [Display(Name = "Batch")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select Batch.")]
        public int Batch { get; set; }
        public List<SelectListItem> BatchList { get; set; }

        [Display(Name = "Upload Type")]
        [RegularExpression("^([I|C]+)$", ErrorMessage = "Please select a valid Upload Type.")]
        public string Type { get; set; }
        public List<SelectListItem> TypeList { get; set; }

        public string StateName { get; set; }
        public string DistrictName { get; set; }
        public int PIUCode { get; set; }
    }
}