using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.MasterReports
{
    public class LocationMasterReports
    {
        
    }

    public class LocationMasterHabitationReport
    {
        public string STATE_NAME { get; set; }
        public string DISTRICT_NAME { get; set; }
        public string BLOCK_NAME { get; set; }

        [Display(Name = "State")]
        [Required(ErrorMessage="Please select state")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select a valid state.")]
        public Nullable<int> MAST_STATE_CODE { get; set; }
        public List<SelectListItem> StateList { get; set; }

        [Display(Name = "District")]
        [Required(ErrorMessage = "Please select district")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select a valid district.")]
        public Nullable<int> MAST_DISTRICT_CODE { get; set; }
        public List<SelectListItem> DistrictList { get; set; }

        [Display(Name = "Block")]
        [Required(ErrorMessage = "Please select block")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select a valid block.")]
        public int MAST_BLOCK_CODE { get; set; }
        public List<SelectListItem> BlockList { get; set; }

        [Display(Name = "Village")]
        public int MAST_VILLAGE_CODE { get; set; }
        public List<SelectListItem> VillageList { get; set; }

        [Display(Name = "Habitation Status")]
        [RegularExpression("^([%|U|C|S|P|F]+)$", ErrorMessage = "Year must be valid Habitation Status.")]
        public string HabitationStatus { get; set; }
        public List<SelectListItem> HabitationStatusList { get; set; }

        [Display(Name = "Is Schedule5")]
        [RegularExpression("^([%|Y|N]+)$", ErrorMessage = "Year must be valid Schedule5 value.")]
        public string Schedule5 { get; set; }
        public List<SelectListItem> Schedule5List { get; set; }

        [Display(Name = "Census Year")]
        public int CensusYear { get; set; }
        public List<SelectListItem> CensusYearList { get; set; }

        [Display(Name = "Active")]
        [RegularExpression("^([%|Y|N]+)$", ErrorMessage = "Year must be valid Active flag.")]
        public string Active { get; set; }
        public List<SelectListItem> ActiveList { get; set; }

        public int RptNo { get; set; }
        public int Panchayat { get; set; }
        public string Type1 { get; set; }
        public string Type2 { get; set; }
        public string Type3 { get; set; }
        public string Type4 { get; set; }
        public string Type5 { get; set; }
    }
}