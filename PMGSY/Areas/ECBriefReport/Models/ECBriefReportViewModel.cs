using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace PMGSY.Areas.ECBriefReport.Models
{
    public class ECBriefReportViewModel
    {

        //public ECBriefReportViewModel()
        //{
        //    StateList = new List<SelectListItem>();
        //    DistrictList = new List<SelectListItem>();
        //    CollaborationList = new List<SelectListItem>();
        //    AgencyList = new List<SelectListItem>();
        //    YearList = new List<SelectListItem>();
        //    BatchList = new List<SelectListItem>();
        //}

        //[Display(Name = "State")]

        public int LevelCode { get; set; }
        public string StateName { get; set; }
        public string State_Name { get; set; }

        public string DistName { get; set; }
        public string BlockName { get; set; }
        public string CollabName { get; set; }
        public string AgencyName { get; set; }
        public string StatusName { get; set; }
        public string YearName { get; set; }
        public string BatchName { get; set; }

        [Display(Name = "State")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid State.")]
        public int StateCode { get; set; }
        public List<SelectListItem> StateList { get; set; }

        [Display(Name = "District")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid District.")]
        public int DistrictCode { get; set; }
        public List<SelectListItem> DistrictList { get; set; }

        [Display(Name = "Block")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Block.")]
        public int BlockCode { get; set; }
        public List<SelectListItem> BlockList { get; set; }

        [Display(Name = "Collaboration")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Collaboration.")]
        public int CollaborationCode { get; set; }
        public List<SelectListItem> CollaborationList { get; set; }

        [Display(Name = "Agency")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Agency.")]
        public int AgencyCode { get; set; }
        public List<SelectListItem> AgencyList { get; set; }

        //public string AgencyName { get; set; }

        [Display(Name = "Year")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Year.")]
        public int Year { get; set; }
        public List<SelectListItem> YearList { get; set; }

        [Display(Name = "Proposal")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Proposal.")]
        public int Proposal { get; set; }
        public List<SelectListItem> ProposalList { get; set; }

        [Display(Name = "Type")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Type.")]
        public int Type { get; set; }
        public List<SelectListItem> TypeList { get; set; }

        [Display(Name = "Status")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Status.")]
        public int Status { get; set; }
        public List<SelectListItem> StatusList { get; set; }

        [Display(Name = "Batch")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Batch.")]
        public int BatchCode { get; set; }
        public List<SelectListItem> BatchList { get; set; }
    }

  public class RoadProgress
    {

        public List<SelectListItem> lstscheme { get; set; }
        [Display(Name = "Scheme")]
        public int schemeCode { get; set; }


        [Required(ErrorMessage = "Month required")]
        [Range(0, 12, ErrorMessage = "Invalid a month")]
        //[LocalizedDisplayName("lblMonth")]
        public int Month { get; set; }
        public List<SelectListItem> MonthList { get; set; }

        [Required(ErrorMessage = "Year required")]
        [Range(0, int.MaxValue, ErrorMessage = "Invalid year")]
        //[LocalizedDisplayName("lblYear")]
        public int Year { get; set; }
        public List<SelectListItem> YearList { get; set; }

        public int LevelCode { get; set; }
        public string StateName { get; set; }
        public string State_Name { get; set; }

        public string DistName { get; set; }
        public string BlockName { get; set; }
        public string CollabName { get; set; }
        public string AgencyName { get; set; }

        [Display(Name = "State")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid State.")]
        public int StateCode { get; set; }
        public List<SelectListItem> StateList { get; set; }

        [Display(Name = "District")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid District.")]
        public int DistrictCode { get; set; }
        public List<SelectListItem> DistrictList { get; set; }

        [Display(Name = "Block")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Block.")]
        public int BlockCode { get; set; }
        public List<SelectListItem> BlockList { get; set; }

        [Display(Name = "Collaboration")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Collaboration.")]
        public int CollaborationCode { get; set; }
        public List<SelectListItem> CollaborationList { get; set; }

        [Display(Name = "Agency")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Agency.")]
        public int AgencyCode { get; set; }
        public List<SelectListItem> AgencyList { get; set; }
    }


    public class FAPhaseProfileReportViewModel
    {
        public int LevelCode { get; set; }
        public string StateName { get; set; }
        public string State_Name { get; set; }

        public string DistName { get; set; }
        public string BlockName { get; set; }
        public string CollabName { get; set; }
        public string AgencyName { get; set; }
        
        [Display(Name = "State")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid State.")]
        public int StateCode { get; set; }
        public List<SelectListItem> StateList { get; set; }

        [Display(Name = "District")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid District.")]
        public int DistrictCode { get; set; }
        public List<SelectListItem> DistrictList { get; set; }

        [Display(Name = "Block")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Block.")]
        public int BlockCode { get; set; }
        public List<SelectListItem> BlockList { get; set; }

        [Display(Name = "Collaboration")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Collaboration.")]
        public int CollaborationCode { get; set; }
        public List<SelectListItem> CollaborationList { get; set; }

        [Display(Name = "Agency")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Agency.")]
        public int AgencyCode { get; set; }
        public List<SelectListItem> AgencyList { get; set; }
    }

    public class StateListWiseRoadsViewModel
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
        // [Display(Name = "State")]
        //[LocalizedDisplayName("lblState")]
        [Required(ErrorMessage = "Please select State. ")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select  State.")]
        // [RegularExpression(@"^([0-9]+)$", ErrorMessage = "State must be valid number.")]
        public int StateCode { get; set; }
        public List<SelectListItem> StateList { get; set; }

        // [Display(Name = "District")]
        //[LocalizedDisplayName("lblDistrict")]
        [Required(ErrorMessage = "Please select District.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select District.")]
        //[RegularExpression(@"^([0-9]+)$", ErrorMessage = "District must be valid number.")]
        public int DistrictCode { get; set; }
        public List<SelectListItem> DistrictList { get; set; }

        //[Display(Name = "Block")]
        //[LocalizedDisplayName("lblBlock")]
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
        [RegularExpression(@"^([%PC]+)$", ErrorMessage = "Please Select Road Status.")]
        public string Status { get; set; }
        public List<SelectListItem> StatusList { get; set; }

        //[LocalizedDisplayName("lblRoadWise")]
        public bool RoadWise { get; set; }
    }

    public class Category
    {
        [Display(Name = "Year")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select Year.")]
        public int Year { get; set; }
        public List<SelectListItem> YearList { get; set; }

        [Display(Name = "Month")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select Month.")]
        public int Batch { get; set; }
        public List<SelectListItem> BatchList { get; set; }

        [Display(Name = "Collaboration")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select Collaboration.")]
        public int Collaboration { get; set; }
        public List<SelectListItem> CollabList { get; set; }

        [Display(Name = "Status")]
        //[Range(0, int.MaxValue, ErrorMessage = "Please select Status.")]
        [RegularExpression(@"^([NYURDSA]+)$", ErrorMessage = "Invalid Status selected")]
        public string Status { get; set; }
        public List<SelectListItem> StatusList { get; set; }

        public int Level { get; set; }
        public int State { get; set; }
        public int District { get; set; }
        public int Block { get; set; }
        public int PMGSYScheme { get; set; }

        public string StateName { get; set; }
        public string DistrictName { get; set; }
        public string BlockName { get; set; }
        public string CollabName { get; set; }
        public string StatusName { get; set; }
    }
}