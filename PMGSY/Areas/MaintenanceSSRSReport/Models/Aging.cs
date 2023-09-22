using PMGSY.Common;
using PMGSY.Extensions;
using PMGSY.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.MaintenanceSSRSReport.Models
{
    public class Aging
    {

        public Aging()
        {
            CommonFunctions commonFunctions = new CommonFunctions();
            StateList = new List<SelectListItem>();
          

            StateName = PMGSYSession.Current.StateCode == 0 ? "0" : PMGSYSession.Current.StateName.Trim();
            DistName = PMGSYSession.Current.DistrictCode == 0 ? "0" : PMGSYSession.Current.DistrictName.Trim();

            Mast_State_Code = PMGSYSession.Current.StateCode;
            Mast_District_Code = PMGSYSession.Current.DistrictCode;

            LevelCode = PMGSYSession.Current.DistrictCode > 0 ? 2 : 1;

            StateList = commonFunctions.PopulateStates(true);
            StateCode = PMGSYSession.Current.StateCode == 0 ? 0 : PMGSYSession.Current.StateCode;
            StateList.Find(x => x.Value == StateCode.ToString()).Selected = true;

        }
        public int LevelCode { get; set; }
        public string StateName { get; set; }
        public string DistName { get; set; }
        public string BlockName { get; set; }
        public int Mast_State_Code { get; set; }
        public int Mast_Block_Code { get; set; }
        public int Mast_District_Code { get; set; }


        [Display(Name = "Delay Group")]
        [Range(0, int.MaxValue, ErrorMessage = "Please Select Delay Group.")]
        public int DelayGroupCode { get; set; }
        public List<SelectListItem> DelayGroupList { get; set; }

        [Display(Name = "State")]
        [Required(ErrorMessage = "Please select State. ")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select  State.")]
        public int StateCode { get; set; }
        public List<SelectListItem> StateList { get; set; }

        [Required]
        [Display(Name = "Exp. Start Range")]
        [Range(0, 100000000, ErrorMessage = "Invalid Exp. Start Range")]
        public decimal EXP_START_RANGE { get; set; }

        [Required]
        [Display(Name = "Exp. End Range")]
        [Range(0, 100000000, ErrorMessage = "Invalid Exp. End Range")]
        public decimal EXP_END_RANGE { get; set; }


        //[Range(0, 9999999999999999.99, ErrorMessage = "Higher Specification Cost is not in valid format.")]
        //public Nullable<decimal> TEND_HIGHER_SPEC_AMT { get; set; }
    }
}