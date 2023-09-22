using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.PackageAgreementSanctionList.Models
{
    public class GepnicBidModel
    {

        public int LevelCode { get; set; }
        public string StateName { get; set; }
        public string DistName { get; set; }
        public string BlockName { get; set; }
        public int Mast_State_Code { get; set; }
        public int Mast_Block_Code { get; set; }
        public int Mast_District_Code { get; set; }

        public string YearString { get; set; }

        [Display(Name = "State : ")]
        [Required(ErrorMessage = "Please select State. ")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select  State.")]

        public int StateCode { get; set; }
        public List<SelectListItem> StateList { get; set; }

        //[Display(Name = "District : ")]
        //[Required(ErrorMessage = "Please select District.")]
        //[Range(0, int.MaxValue, ErrorMessage = "Please select District.")]

        //public int DistrictCode { get; set; }
        //public List<SelectListItem> DistrictList { get; set; }

        //[Display(Name = "Block : ")]
        //[Required(ErrorMessage = "Please select Block.")]
        //[Range(0, int.MaxValue, ErrorMessage = "Please select Block.")]

        //public int BlockCode { get; set; }
        //public List<SelectListItem> BlockList { get; set; }

        [Display(Name = "Year : ")]
        [Range(0, 2090, ErrorMessage = "Please select Year.")]
        [Required(ErrorMessage = "Please select Year.")]
        [RegularExpression("^([0-9]+)$", ErrorMessage = "Year must be valid number.")]
        public int Year { get; set; }
        public List<SelectListItem> YearList { get; set; }

        [Required(ErrorMessage = "Batch required")]
        [Range(0, 6, ErrorMessage = "Invalid Batch")]
        public int Batch { get; set; }
        public List<SelectListItem> BatchList { get; set; }

        [Range(0, 5, ErrorMessage = "Please select PMGSY Scheme.")]
        public int SchemeCode { get; set; }
        public List<SelectListItem> SchemeList { get; set; }



    }
}