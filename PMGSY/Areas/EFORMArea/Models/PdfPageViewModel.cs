using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

 
using System.ComponentModel.DataAnnotations;
 

namespace PMGSY.Areas.EFORMArea.Model
{
    public class PdfPageViewModel
    {
        //[Required(ErrorMessage = "Please select State")]
        //[Range(1, 50, ErrorMessage = "Please select a valid State")]


        public PdfPageViewModel()
        {
            Eform_Type = new List<SelectListItem>(); // // added new on 28-07-2022

        }


        [RegularExpression("^[a-zA-Z/., ]{2,50}$", ErrorMessage = "Invalid Bank Name")]
        public string QMName { get; set; }


      

        public int ImsPrRoadCode { get; set; }
        public string PmgsyScheme { get; set; }
        public string ProposalType { get; set; }
        public int ImsYear { get; set; }
        public string ImsBatch { get; set; }
        public string ImsPackageId { get; set; }
        public string ImsRoadName { get; set; }

        [Display(Name = "Eform Status")]
        public int eformstatus_code { get; set; }
        public string Eform_Status { get; set; }

        public List<SelectListItem> lstEformStatus { set; get; }


        [Display(Name = "State Name")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid state")]
        public int stateCode { get; set; }
        public List<SelectListItem> lstState { set; get; }

        public String StateName { get; set; }


        [Display(Name = "District")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid district")]
        public int districtCode { get; set; }
        public List<SelectListItem> lstDistrict { set; get; }


        public String DistrictName { get; set; }

        public int RoleCode { get; set; }

        public String month { get; set; }
        public String Year { get; set; }


        [Display(Name = "Month")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid Month")]
        public int MonthCode { get; set; }

        [Display(Name = "Road/Bridge")]
        public string eFormType { get; set; }
        public List<SelectListItem> Eform_Type { get; set; }  // added new on 28-07-2022


        public List<SelectListItem> lstMonth { set; get; }

        [Display(Name = "Year")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid Year")]
        public int YearCode { get; set; }


        public List<SelectListItem> lstYear { set; get; }

        public string isInterstateSQM { get; set; }
    }
}