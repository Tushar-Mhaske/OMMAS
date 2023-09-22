using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.Common;
using PMGSY.Extensions;
using System.ComponentModel.DataAnnotations;

namespace PMGSY.Areas.Feedback.Models
{
    public class HabitationClusterModel
    {
        public HabitationClusterModel()
        {
            CommonFunctions commonFunctions = new CommonFunctions();
            StateList = new List<SelectListItem>();
            DistrictList = new List<SelectListItem>();
            BlockList = new List<SelectListItem>();

            StateName = PMGSYSession.Current.StateCode == 0 ? "0" : PMGSYSession.Current.StateName.Trim();
            DistName = PMGSYSession.Current.DistrictCode == 0 ? "0" : PMGSYSession.Current.DistrictName.Trim();

            Mast_State_Code = PMGSYSession.Current.StateCode;
            Mast_District_Code = PMGSYSession.Current.DistrictCode;

            //LevelCode = PMGSYSession.Current.BlockCode > 0 ? 3 : PMGSYSession.Current.DistrictCode > 0 ? 2 : 1;
            LevelCode = PMGSYSession.Current.DistrictCode > 0 ? 2 : 1;

            StateList = commonFunctions.PopulateStates(false);
            //StateList.Insert(0, (new SelectListItem { Text = "All States", Value = "0" }));
            StateList.Insert(0, (new SelectListItem { Text = "Select State", Value = "-1" }));
            StateCode = PMGSYSession.Current.StateCode;//== 0 ? 1 : PMGSYSession.Current.StateCode;
            //StateList.Find(x => x.Value == StateCode.ToString()).Selected = true;

            DistrictList = new List<SelectListItem>();
            if (StateCode == 0)
            {
                DistrictList.Insert(0, (new SelectListItem { Text = "All Districts", Value = "0", Selected = true }));
            }
            else
            {
                DistrictList = commonFunctions.PopulateDistrict(StateCode, true);
                DistrictCode = PMGSYSession.Current.DistrictCode == 0 ? 0 : PMGSYSession.Current.DistrictCode;
                DistrictList.Find(x => x.Value == "-1").Value = "0";
                DistrictList.Find(x => x.Value == DistrictCode.ToString()).Selected = true;

            }
            BlockList = new List<SelectListItem>();
            if (DistrictCode == 0)
            {
                BlockList.Insert(0, (new SelectListItem { Text = "All Blocks", Value = "0", Selected = true }));
            }
            else
            {
                BlockList = commonFunctions.PopulateBlocks(DistrictCode, true);
                BlockList.Find(x => x.Value == "-1").Value = "0";

                //BlockCode = PMGSYSession.Current.BlockCode == 0 ? 0 : PMGSYSession.Current.BlockCode;
                //BlockList.Find(x => x.Value == BlockCode.ToString()).Selected = true;
            }
            StatusList = new List<SelectListItem>();
            StatusList.Add(new SelectListItem { Text = "All", Value = "%" });
            StatusList.Add(new SelectListItem { Text = "Yes", Value = "Y" });
            StatusList.Add(new SelectListItem { Text = "No", Value = "N" });
            Status = "%";

        }
        public bool Roadwise { get; set; }
        public int LevelCode { get; set; }
        public string StateName { get; set; }
        public string DistName { get; set; }
        public string BlockName { get; set; }
        public int Mast_State_Code { get; set; }
        public int Mast_Block_Code { get; set; }
        public int Mast_District_Code { get; set; }
        public string StatusName { get; set; }

        [Display(Name = "State")]
        [Required(ErrorMessage = "Please select State. ")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid State.")]
        public int StateCode { get; set; }
        public List<SelectListItem> StateList { get; set; }

        [Display(Name = "District")]
        [Required(ErrorMessage = "Please select District.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid District.")]
        public int DistrictCode { get; set; }
        public List<SelectListItem> DistrictList { get; set; }

        [Display(Name = "Block")]
        [Required(ErrorMessage = "Please select Block.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Block.")]
        public int BlockCode { get; set; }
        public List<SelectListItem> BlockList { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }
        public List<SelectListItem> StatusList { get; set; }

        [Display(Name = "Month")]
        [Range(1, 12, ErrorMessage = "Please select Month.")]
        [Required(ErrorMessage = "Please select Month.")]
        [RegularExpression("^([0-9]+)$", ErrorMessage = "Month must be valid number.")]
        public int Month { get; set; }
        public List<SelectListItem> MonthList { get; set; }


        [Display(Name = "Year")]
        [Range(1, 2090, ErrorMessage = "Please select Year.")]
        [Required(ErrorMessage = "Please select Year.")]
        [RegularExpression("^([0-9]+)$", ErrorMessage = "Year must be valid number.")]
        public int Year { get; set; }
        public List<SelectListItem> YearList { get; set; }
    }

    public class ClusterViewModel
    {
        public int level { get; set; }

        [Display(Name = "State")]
        [Required(ErrorMessage = "Please select State. ")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select a valid State.")]
        public int stateCode { get; set; }
        public List<SelectListItem> stateList { get; set; }

        [Display(Name = "District")]
        [Required(ErrorMessage = "Please select District.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select a valid District.")]
        public int districtCode { get; set; }
        public List<SelectListItem> districtList { get; set; }

        public int pmgsyScheme { get; set; }
    }
}