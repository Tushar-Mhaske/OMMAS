using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace PMGSY.Areas.LocationSSRSReports.Models
{
    public class LocationMasterReports
    {
        public int RptNo { get; set; }

        public int Mast_State_Code { get; set; }
        public int Mast_District_Code { get; set; }
        public int Mast_Block_Code { get; set; }


        public int Year { get; set; }

        public string Type1 { get; set; }
        public string Type2 { get; set; }
        public string Type3 { get; set; }
        public string Type4 { get; set; }
        public string Type5 { get; set; }

        public string StateName { get; set; }
        public string DistName { get; set; }
        public string BlockName { get; set; }
        public string VillageName { get; set; }
        public string PanchayatName { get; set; }

        public string ActiveFlagName { get; set; }

        [Display(Name = "State")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid State.")]
        public int State { get; set; }
        public List<SelectListItem> StateList { get; set; }

        [Display(Name = "District")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid District.")]
        public int District { get; set; }
        public List<SelectListItem> DistrictList { get; set; }

        [Display(Name = "Block")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Block.")]
        public int Block { get; set; }
        public List<SelectListItem> BlockList { get; set; }

        [Display(Name = "State/Union")]
        [RegularExpression(@"^([%SU]+)$", ErrorMessage = "Invalid Territory selected")]
        public string Territory { get; set; }
        public List<SelectListItem> TerritoryTypeList { get; set; }

        [Display(Name = "State Type")]
        [RegularExpression(@"^([%RINHX]+)$", ErrorMessage = "Invalid State Type selected")]
        public string StateType { get; set; }
        public List<SelectListItem> StateTypeList { get; set; }

        [Display(Name = "Active")]
        [RegularExpression(@"^([%YN]+)$", ErrorMessage = "Invalid Active Flag selected")]
        public string ActiveFlag { get; set; }
        public List<SelectListItem> ActiveFlagList { get; set; }

        [Display(Name = "IAP District")]
        [RegularExpression(@"^([%YN]+)$", ErrorMessage = "Invalid IAP District selected")]
        public string iapDistrict { get; set; }
        public List<SelectListItem> iapDistrictList { get; set; }

        [Display(Name = "PMGSY Included")]
        [RegularExpression(@"^([%YN]+)$", ErrorMessage = "Invalid PMGSY selected")]
        public string pmgsyIncluded { get; set; }
        public List<SelectListItem> pmgsyIncludedList { get; set; }

        [Display(Name = "Desert")]
        [RegularExpression(@"^([%YN]+)$", ErrorMessage = "Invalid Desert value selected")]
        public string Desert { get; set; }
        public List<SelectListItem> DesertList { get; set; }

        [Display(Name = "Tribal")]
        [RegularExpression(@"^([%YN]+)$", ErrorMessage = "Invalid Tribal value selected")]
        public string Tribal { get; set; }
        public List<SelectListItem> TribalList { get; set; }

        [Display(Name = "Schedule")]
        [RegularExpression(@"^([%YN]+)$", ErrorMessage = "Invalid Schedule value selected")]
        public string Schedule { get; set; }
        public List<SelectListItem> ScheduleList { get; set; }

        [Display(Name = "Census Year")]
        [Range(0, int.MaxValue, ErrorMessage = "Invalid Census Year.")]
        public int CensusYear { get; set; }
        public List<SelectListItem> CensusYearList { get; set; }

        [Display(Name = "Habitation Status")]
        [RegularExpression(@"^([%FPSCUYN]+)$", ErrorMessage = "Invalid Habitation Status selected")]
        public string HabitationStatus { get; set; }
        public List<SelectListItem> HabitationStatusList { get; set; }

        [Display(Name = "Village")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Village.")]
        public int Village { get; set; }
        public List<SelectListItem> VillageList { get; set; }

        public string Flag { get; set; }

        [Display(Name = "MP Constituency")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid MP Constituency.")]
        public int MPConstituency { get; set; }
        public List<SelectListItem> MPConstituencyList { get; set; }

        [Display(Name = "MLA Constituency")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid MLA Constituency.")]
        public int MLAConstituency { get; set; }
        public List<SelectListItem> MLAConstituencyList { get; set; }

        public string MPConstituencyName { get; set; }

        [Display(Name = "Panchayat")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Panchayat.")]
        public int Panchayat { get; set; }
        public List<SelectListItem> PanchayatList { get; set; }

        [Display(Name = "Region")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Region.")]
        public int Region { get; set; }
        public List<SelectListItem> RegionList { get; set; }

        public string RegionName { get; set; }
    }

    public class OMMSSystemCodesViewModel
    {
        public string StateName { get; set; }
        public string DistrictName { get; set; }

        [Display(Name = "State")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid State.")]
        public int StateCode { get; set; }
        public List<SelectListItem> StateList { get; set; }

        [Display(Name = "District")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid District.")]
        public int DistrictCode { get; set; }
        public List<SelectListItem> DistrictList { get; set; }
    }

    public class MPMLAConCodesViewModel
    {
        public string StateName { get; set; }

        [Display(Name = "Constituency Type")]
        [RegularExpression(@"^([PL]+)$", ErrorMessage = "Invalid Tribal value selected")]
        public string ConstituencyType { get; set; }
        public List<SelectListItem> ConstituencyTypeList { get; set; }

        [Display(Name = "State")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid State.")]
        public int StateCode { get; set; }
        public List<SelectListItem> StateList { get; set; }
    }

    public class SystemCodesExcelViewModel
    {
        public string StateName { get; set; }
        public string DistrictName { get; set; }

        public string conType { get; set; }

        [Display(Name = "Codes For")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select valid report.")]
        public int Report { get; set; }
        public List<SelectListItem> ReportList { get; set; }

        [Display(Name = "State")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select valid State.")]
        public int StateCode { get; set; }
        public List<SelectListItem> StateList { get; set; }

        [Display(Name = "District")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid District.")]
        public int DistrictCode { get; set; }
        public List<SelectListItem> DistrictList { get; set; }
    }

    public class ForthNightelyReportViewModel
    {
        public string StateName { get; set; }

        [Display(Name = "From Month")]
        [Range(0, 12, ErrorMessage = "Please select valid From Month.")]
        public int FromMonth { get; set; }
        public List<SelectListItem> FromMonthList { get; set; }

        [Display(Name = "To Month")]
        [Range(0, 12, ErrorMessage = "Please select valid To Month.")]
        public int ToMonth { get; set; }
        public List<SelectListItem> ToMonthList { get; set; }


        [Display(Name = "From Year")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid From Year.")]
        public int FromYear { get; set; }
        public List<SelectListItem> FromYearList { get; set; }

        [Display(Name = "To Year")]
        //[DateValidationVST("FromYear", "FromMonth", "ToMonth", ErrorMessage = "To Year must be greater than or equal to From Year.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid To Year.")]
        public int ToYear { get; set; }
        public List<SelectListItem> ToYearList { get; set; }

        [Display(Name = "Stream")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Stream.")]
        public int Stream { get; set; }
        public List<SelectListItem> StreamList { get; set; }

        [Display(Name = "PMGSY Scheme")]
        [Range(0, 2, ErrorMessage = "Please select a valid PMGSY Scheme.")]
        public int pmgsyScheme { get; set; }
        public List<SelectListItem> pmgsySchemeList { get; set; }


    }
}