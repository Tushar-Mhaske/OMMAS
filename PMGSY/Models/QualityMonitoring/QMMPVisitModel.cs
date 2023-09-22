using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.QualityMonitoring
{
    public class QMMPVisitModel
    {
        [Display(Name = "State")]
        public int StateCode { get; set; }
        public List<SelectListItem> StateList { get; set; }
        public string StateName { get; set; }

        [Display(Name = "District")]
        public int DistrictCode { get; set; }
        public List<SelectListItem> DistrictList { get; set; }
        public string DistrictName { get; set; }

        [Display(Name = "Block")]
        public int BlockCode { get; set; }
        public List<SelectListItem> BlockList { get; set; }
    }


    public class FillMPVisitModel
    {
        public int PrRoadCode { get; set; }

        [Display(Name = "Constituency")]
        [Required]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please Select Constituency")]
        public int ConstituencyCode { get; set; }
        public List<SelectListItem> ConstituencyList { get; set; }
        public string ConstituencyName { get; set; }

        public string EncryptedVisitCode { get; set; }

        [Display(Name = "MP Name")]
        [Required]
        public string MPName { get; set; }

        [Display(Name = "MP House")]
        [Required]
        public string MPHouse { get; set; }

        //[Display(Name = "PIU Name")]
        //[Required]
        //public string PIUName { get; set; }


        [Display(Name = "PIU Name")]
        [Required]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please Select PIU Name")]
        public int PIUNameCode { get; set; }
        public List<SelectListItem> PIUNameList { get; set; }
        public string PIUName { get; set; }



        [Display(Name = "Date Of Visit")]
        [Required]
        public string DateOfVisit { get; set; }

        public string Operation { get; set; }

       // [Display(Name = "Remarks")]
        //[RegularExpression(@"[A-Za-z0-9._@ ]{3,100}", ErrorMessage = "Invalid Remarks, Minimum Length 4 Characters, Can be AlphaNumeric,Can Contain (. _ @) ")]

        //[StringLength(100, MinimumLength = 3, ErrorMessage = "Minimum Length 4 Characters.")]
        //[RegularExpression(@"^([a-zA-Z0-9 ._@,\r\n&()-]+)$", ErrorMessage = "Invalid Remarks.Can be AlphaNumeric,Can Contain ._@,&()- ")] 
        //public string  { get; set; }


        [Display(Name = "Remark")]
        [StringLength(8000, ErrorMessage = "Remark must be less than 8000 characters.")]
        [RegularExpression(@"^([a-zA-Z0-9 ._';/,\r\n&()-]+)$", ErrorMessage = "Remark is not in valid format.")]
        public string Remarks { get; set; }


        [Display(Name="Block Name")]
        public string BlockName { get; set; }

        [Display(Name = "Package Number")]
        public string PackageName { get; set;}

        [Display(Name = "Sanction Year")]
        public string SanctionYear { get; set;}

        [Display(Name = "Road Name")]
        public string RoadName { get; set;}

    }
}