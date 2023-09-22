using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using PMGSY.Common;

namespace PMGSY.Areas.MPR.Models
{
    #region CMPR REPORT
    public class MPRModel
    {
        //[LocalizedDisplayName("lblMonth")]
        [Display(Name = "Month")]
        public int Month { get; set; }
        public List<SelectListItem> MonthList { get; set; }

        //[LocalizedDisplayName("lblYear")]
        [Display(Name = "Year")]
        public int Year { get; set; }
        public List<SelectListItem> YearList { get; set; }

        //[LocalizedDisplayName("lblQuarter")]
        [Display(Name = "Quarter")]
        public int Quarter  { get; set; }
        public List<SelectListItem> QuarterList { get; set; }

        //[LocalizedDisplayName("lblRoadCategory")]
        [Display(Name = "Road Category")]
        public string RoadCat { get; set; }
        public List<SelectListItem> RoadCatList { get; set; }

        //[LocalizedDisplayName("lblPopulation")]
        [Display(Name = "Population")]
        public string POP { get; set; }
        public List<SelectListItem> POPList { get; set; }

        //[LocalizedDisplayName("lblCollaboration")]
        [Display(Name = "Collaboration")]
        public string Collab { get; set; }
        public List<SelectListItem> CollabList { get; set; }

        //[LocalizedDisplayName("lblRoute")]
        [Display(Name = "Route")]
        public string Route { get; set; }
        public List<SelectListItem> RouteList { get; set; }

        [Display(Name = "PMGSY")]
        public string PMGSY { get; set; }

        [Required(ErrorMessage = "Localized value required")]
        [RegularExpression(@"^(\b(en|hi)\b+)$", ErrorMessage = "Invalid Localized Value")]
        //[Display(Name = "Localized")]
        public string localizedValue { get; set; }

        public int LevelCode { get; set; }
        public string StateName { get; set; }
        public string DistName { get; set; }
        public string BlockName { get; set; }
        public int Mast_State_Code { get; set; }
        public int Mast_Block_Code { get; set; }
        public int Mast_District_Code { get; set; }

        //[Display(Name = "State")]
        //[LocalizedDisplayName("lblState")]
        [Required(ErrorMessage = "Please select State. ")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select  State.")]
        // [RegularExpression(@"^([0-9]+)$", ErrorMessage = "State must be valid number.")]
        public int StateCode { get; set; }
        public List<SelectListItem> StateList { get; set; }

        //[LocalizedDisplayName("lblDistrict")]
        [Required(ErrorMessage = "Please select District.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select District.")]
        //[RegularExpression(@"^([0-9]+)$", ErrorMessage = "District must be valid number.")]
        public int DistrictCode { get; set; }
        public List<SelectListItem> DistrictList { get; set; }

        //[LocalizedDisplayName("lblBlock")]
        [Required(ErrorMessage = "Please select Block.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select Block.")]
        // [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Block must be valid number.")]
        public int BlockCode { get; set; }
        public List<SelectListItem> BlockList { get; set; }
    }
    #endregion

    public class MPR1Model
    {
        [Required(ErrorMessage = "PMGSY Scheme required")]
        [Range(0, 4, ErrorMessage = "Invalid PMGSY Scheme")]
        public int Scheme { get; set; }
        public List<SelectListItem> SchemeList { get; set; }

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


        [Display(Name = "Collaboration")]
        [Required(ErrorMessage = "Collaboration required")]
        [Range(0, int.MaxValue, ErrorMessage = "Invalid collaboration")]
        //[LocalizedDisplayName("lblCollaboration")]
        public string Collab { get; set; }
        public List<SelectListItem> CollabList { get; set; }

        [Required(ErrorMessage = "Localized value required")]
        [RegularExpression(@"^(\b(en|hi)\b+)$", ErrorMessage = "Invalid Localized Value")]
        //[Display(Name = "Localized")]
        public string localizedValue { get; set; }

        public int LevelCode { get; set; }
        public string StateName { get; set; }
        public string DistName { get; set; }
        public string BlockName { get; set; }
        public int Mast_State_Code { get; set; }
        public int Mast_Block_Code { get; set; }
        public int Mast_District_Code { get; set; }

        //[LocalizedDisplayName("lblState")]
        [Display(Name = "State")]
        [Required(ErrorMessage = "Please select State. ")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select  State.")]
        // [RegularExpression(@"^([0-9]+)$", ErrorMessage = "State must be valid number.")]
        public int StateCode { get; set; }
        public List<SelectListItem> StateList { get; set; }

        //[LocalizedDisplayName("lblDistrict")]
        [Required(ErrorMessage = "Please select District.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select District.")]
        //[RegularExpression(@"^([0-9]+)$", ErrorMessage = "District must be valid number.")]
        public int DistrictCode { get; set; }
        public List<SelectListItem> DistrictList { get; set; }

        //[LocalizedDisplayName("lblBlock")]
        [Required(ErrorMessage = "Please select Block.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select Block.")]
        // [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Block must be valid number.")]
        public int BlockCode { get; set; }
        public List<SelectListItem> BlockList { get; set; }

        public string MonthName { get; set; }
        public string YearName { get; set; }
        public string CollaborationName { get; set; }


        [Display(Name = "Agency")]
        [Required(ErrorMessage = "Please select Agency")]
        [Range(0, int.MaxValue, ErrorMessage = "Invalid Agency")]
        public int Agency { get; set; }
        public List<SelectListItem> AgencyList { get; set; }

        public string AgencyName { get; set; }
        

        [Display(Name = "Column Names")]
        public string ColumnCode { get; set; }
    }

    public class MPR2Model
    {
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

        [Required(ErrorMessage = "Collaboration required")]
        [Range(0, int.MaxValue, ErrorMessage = "Invalid collaboration")]
        //[LocalizedDisplayName("lblCollaboration")]
        public string Collab { get; set; }
        public List<SelectListItem> CollabList { get; set; }

        [Required(ErrorMessage = "Localized value required")]
        [RegularExpression(@"^(\b(en|hi)\b+)$", ErrorMessage = "Invalid Localized Value")]
        //[Display(Name = "Localized")]
        public string localizedValue { get; set; }

        public int LevelCode { get; set; }
        public string StateName { get; set; }
        public string DistName { get; set; }
        public string BlockName { get; set; }
        public int Mast_State_Code { get; set; }
        public int Mast_Block_Code { get; set; }
        public int Mast_District_Code { get; set; }

        //[LocalizedDisplayName("lblState")]
        [Required(ErrorMessage = "Please select State. ")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select  State.")]
        // [RegularExpression(@"^([0-9]+)$", ErrorMessage = "State must be valid number.")]
        public int StateCode { get; set; }
        public List<SelectListItem> StateList { get; set; }

        //[LocalizedDisplayName("lblDistrict")]
        [Required(ErrorMessage = "Please select District.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select District.")]
        //[RegularExpression(@"^([0-9]+)$", ErrorMessage = "District must be valid number.")]
        public int DistrictCode { get; set; }
        public List<SelectListItem> DistrictList { get; set; }

        //[LocalizedDisplayName("lblBlock")]
        [Required(ErrorMessage = "Please select Block.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select Block.")]
        // [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Block must be valid number.")]
        public int BlockCode { get; set; }
        public List<SelectListItem> BlockList { get; set; }

        public string MonthName { get; set; }
        public string YearName { get; set; }
        public string CollaborationName { get; set; }
    }

    public class MPRQtr6Model
    {
        //[Required(ErrorMessage = "State required")]
        //[Range(0, int.MaxValue, ErrorMessage = "Invalid state")]
        //[Display(Name = "State")]
        //public int StateCode { get; set; }
        //public List<SelectListItem> StateList { get; set; }

        [Required(ErrorMessage = "Quarter required")]
        [Range(0, int.MaxValue, ErrorMessage = "Invalid quarter")]
        //[LocalizedDisplayName("lblQuarter")]
        public int Quarter { get; set; }
        public List<SelectListItem> QuarterList { get; set; }

        [Required(ErrorMessage = "Year required")]
        [Range(0, int.MaxValue, ErrorMessage = "Invalid year")]
        //[LocalizedDisplayName("lblYear")]
        public int Year { get; set; }
        public List<SelectListItem> YearList { get; set; }

        [Required(ErrorMessage = "Localized value required")]
        [RegularExpression(@"^(\b(en|hi)\b+)$", ErrorMessage = "Invalid Localized Value")]
        //[Display(Name = "Localized")]
        public string localizedValue { get; set; }

        public int LevelCode { get; set; }
        public string StateName { get; set; }
        public string DistName { get; set; }
        public string BlockName { get; set; }
        public int Mast_State_Code { get; set; }
        public int Mast_Block_Code { get; set; }
        public int Mast_District_Code { get; set; }

        //[LocalizedDisplayName("lblState")]
        [Required(ErrorMessage = "Please select State. ")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select  State.")]
        // [RegularExpression(@"^([0-9]+)$", ErrorMessage = "State must be valid number.")]
        public int StateCode { get; set; }
        public List<SelectListItem> StateList { get; set; }

        //[LocalizedDisplayName("lblDistrict")]
        [Required(ErrorMessage = "Please select District.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select District.")]
        //[RegularExpression(@"^([0-9]+)$", ErrorMessage = "District must be valid number.")]
        public int DistrictCode { get; set; }
        public List<SelectListItem> DistrictList { get; set; }

        //[LocalizedDisplayName("lblBlock")]
        [Required(ErrorMessage = "Please select Block.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select Block.")]
        // [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Block must be valid number.")]
        public int BlockCode { get; set; }
        public List<SelectListItem> BlockList { get; set; }

        public string QuarterName { get; set; }
        public string YearName { get; set; }
    }

}