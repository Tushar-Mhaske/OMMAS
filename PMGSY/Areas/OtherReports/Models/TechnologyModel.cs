using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.OtherReports.Models
{
    public class TechnologyModel
    {
         
            public int LevelCode { get; set; }
            public string StateName { get; set; }
            public string DistName { get; set; }
            public string BlockName { get; set; }
            
            
            [Display(Name = "State")]
            [Required(ErrorMessage = "Please select State. ")]
            [Range(0, int.MaxValue, ErrorMessage = "Please select  State.")]
            // [RegularExpression(@"^([0-9]+)$", ErrorMessage = "State must be valid number.")]
            public int StateCode { get; set; }
            public List<SelectListItem> StateList { get; set; }

             [Display(Name = "District")]
            [Required(ErrorMessage = "Please select District.")]
            [Range(0, int.MaxValue, ErrorMessage = "Please select District.")]
            //[RegularExpression(@"^([0-9]+)$", ErrorMessage = "District must be valid number.")]
            public int DistrictCode { get; set; }
            public List<SelectListItem> DistrictList { get; set; }

            [Display(Name = "Block")]
            [Required(ErrorMessage = "Please select Block.")]
            [Range(0, int.MaxValue, ErrorMessage = "Please select Block.")]
            // [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Block must be valid number.")]
            public int BlockCode { get; set; }
            public List<SelectListItem> BlockList { get; set; }

             [Display(Name = "Year")]
            [Range(0, 2090, ErrorMessage = "Please select Year.")]
            [Required(ErrorMessage = "Please select Year.")]
            [RegularExpression("^([0-9]+)$", ErrorMessage = "Year must be valid number.")]
            public int Year { get; set; }
            public SelectList YearList { get; set; }

             [Display(Name = "Batch")]
            [Range(0, 10, ErrorMessage = "Please select Batch.")]
            [Required(ErrorMessage = "Please select Batch.")]
            [RegularExpression("^([0-9]+)$", ErrorMessage = "Batch must be valid number.")]
            public int Batch { get; set; }
            public List<SelectListItem> BatchList { get; set; }

            [Display(Name = "Collaboration")]
            [Range(0, int.MaxValue, ErrorMessage = "Please select Collaboration.")]
            [Required(ErrorMessage = "Please select Collaboration.")]
            [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Collaboration must be valid number.")]
            public int FundingAgency { get; set; }
            
             public List<SelectListItem> FundingAgencyList { get; set; }
           
           
            [Display(Name = "Connectivity")]
            [RegularExpression(@"^([%NU]+)$", ErrorMessage = "Please Select  Connectivity.")]
            public string ConnectivityCode { get; set; }
            public List<SelectListItem> ConnectivitysList { get; set; }

            //[RegularExpression(@"^([%ST]+)$", ErrorMessage = "Please Select Report Type.")]
            //public string ReportTypeCode { get; set; }
            //public List<SelectListItem> ReportTypeList { get; set; }
         

            [RegularExpression(@"^([%CP]+)$", ErrorMessage = "Please Select Road Status.")]
            [Display(Name="Road Status")]
            public string RoadStatus { get; set; }
            public List<SelectListItem> RoadStatusList { get; set; }

           
 
    }
}