using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.QMSSRSReports.Models
{
    public class CCBTViewModel
    {

        public int LevelCode { get; set; }
        public string StateName { get; set; }
        public string DistName { get; set; }
        public string BlockName { get; set; }
        public int Mast_State_Code { get; set; }
        public int Mast_Block_Code { get; set; }
        public int Mast_District_Code { get; set; }
        public string CollaborationName { get; set; }
        public string BatchName { get; set; }
        public string StatusName { get; set; }
        
        //[LocalizedDisplayName("lblState")]
        [Display(Name = "State")]
        [Required(ErrorMessage = "Please select State. ")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select  State.")]
        // [RegularExpression(@"^([0-9]+)$", ErrorMessage = "State must be valid number.")]
        public int StateCode { get; set; }
        public List<SelectListItem> StateList { get; set; }

       
        //[LocalizedDisplayName("lblDistrict")]
        [Display(Name = "District")]
        //[Required(ErrorMessage = "Please select District.")]
        //[Range(0, int.MaxValue, ErrorMessage = "Please select District.")]
        //[RegularExpression(@"^([0-9]+)$", ErrorMessage = "District must be valid number.")]
        public int DistrictCode { get; set; }
        public List<SelectListItem> DistrictList { get; set; }

        //
        //[LocalizedDisplayName("lblBlock")]
        [Display(Name = "Block")]
        [Required(ErrorMessage = "Please select Block.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select Block.")]
        // [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Block must be valid number.")]
        public int BlockCode { get; set; }
        public List<SelectListItem> BlockList { get; set; }

        // [Display(Name = "Year")]
        //[LocalizedDisplayName("lblYear")]
        [Range(0, 2090, ErrorMessage = "Please select Year.")]
        [Required(ErrorMessage = "Please select Year.")]
        [RegularExpression("^([0-9]+)$", ErrorMessage = "Year must be valid number.")]
        public int Year { get; set; }
        public List<SelectListItem> YearList { get; set; }

        //  [Display(Name = "Batch")]
        //[LocalizedDisplayName("lblBatch")]
        [Range(0, 10, ErrorMessage = "Please select Batch.")]
        [Required(ErrorMessage = "Please select Batch.")]
        [RegularExpression("^([0-9]+)$", ErrorMessage = "Batch must be valid number.")]
        public int Batch { get; set; }
        public List<SelectListItem> BatchList { get; set; }

        // [Display(Name = "Collaboration")]
        //[LocalizedDisplayName("lblCollaboration")]
        [Range(0, 2147483647, ErrorMessage = "Please select Collaboration.")]
        [Required(ErrorMessage = "Please select Collaboration.")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Collaboration must be valid number.")]
        public int FundingAgency { get; set; }
        public List<SelectListItem> FundingAgencyList { get; set; }
        //[Required(ErrorMessage = "Localized value required")]
        //[RegularExpression(@"^(\b(en|hi)\b+)$", ErrorMessage = "Invalid Localized Value")]
        ////[Display(Name = "Localized")]
        //public string localizedValue { get; set; }

        //[LocalizedDisplayName("lblRoadStatus")]
       // [RegularExpression(@"^([%PC]+)$", ErrorMessage = "Please Select Road Status.")]
        public string Status { get; set; }
        public List<SelectListItem> StatusList { get; set; }

        //[LocalizedDisplayName("lblRoadWise")]
        public bool RoadWise { get; set; }
       
    }
}