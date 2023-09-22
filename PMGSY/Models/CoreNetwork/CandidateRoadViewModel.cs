using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.CoreNetwork
{
    public class CandidateRoadViewModel
    {
        public CandidateRoadViewModel()
        {
            lstBlocks = new List<SelectListItem>();
            lstDRRP = new List<SelectListItem>();
            lstRoadCategory = new List<SelectListItem>();
        }

        [Required(ErrorMessage="Please select Block")]
        [Display(Name = "Block Name")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select Block")]
        public int BlockCode { get; set; }

        public String EncrptedCNCode { get; set; }

        [Required(ErrorMessage="Please select Category")]
        [Display(Name="Category")]
        [Range(1,Int32.MaxValue,ErrorMessage="Please select Category of Road")]
        public int RoadCatCode { get; set; }

        [Required(ErrorMessage="Please select Road Name")]
        [Display(Name="Road Name")]
        [Range(1,Int32.MaxValue,ErrorMessage="Please select Road Name")]
        public int DRRPCode { get; set; }

        [Required(ErrorMessage="Please select Length of Road")]
        [Display(Name="Length of Road")]
        [RegularExpression(@"^[PF]+$",ErrorMessage="Please select Length of Road")]
        public string LengthTypeOfRoad { get; set; }

        [Required(ErrorMessage="Road Length is required.")]
        [Display(Name="Road Length (in Kms.)")]
        public decimal LengthOfRoad { get ; set ; }

        [Required(ErrorMessage="Start Chainage is required.")]
        [Display(Name="Start Chainage (in Kms.)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "  Enter correct value of Start Chainage.")]
        [CompareChainage("EndChainage", ErrorMessage = "Start Chainage must be less than End Chainage")]
        [CompareStartChainage("ExistStartChainage", "LengthTypeOfRoad", ErrorMessage = "Start chainage must be greater than existing start chainage.")]
        public decimal StartChainage { get; set; }

        [Required(ErrorMessage="End Chainage is required.")]
        [Display(Name = "End Chainage (in Kms.)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "  Enter correct value of End Chainage.")]
        [CompareEndChainage("ExistEndChainage", "LengthTypeOfRoad", ErrorMessage = "End chainage must be less than existing end chainage")]
        [CompareValidation("StartChainage", ErrorMessage = "Start Chainage must be less than end chainage.")]
        public decimal EndChainage { get; set; }

        public int CNCode { get; set; }

        public String LockStatus { get; set; }

        // Population of Dropdownlist

        public List<SelectListItem> lstBlocks { get; set; }

        public List<SelectListItem> lstRoadCategory { get; set; }

        public List<SelectListItem> lstDRRP { get; set; }

        public decimal ExistStartChainage { get; set; }

        public decimal ExistEndChainage { get; set; }
    }
}