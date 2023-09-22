using PMGSY.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.LockUnlock
{
    public class LockUnlockReportModel
    {
        CommonFunctions objCommon = new CommonFunctions();

        public LockUnlockReportModel()
        {
            lstStates = objCommon.PopulateStates(true);
            lstDistricts = new List<SelectListItem>();
            lstBlocks = new List<SelectListItem>();
            lstModules = new List<SelectListItem>();
            lstSchemes = new List<SelectListItem>();
        }

        [Required(ErrorMessage="Please select State.")]
        [Range(1,Int32.MaxValue,ErrorMessage="Invalid State.")]
        public int StateCode { get; set; }

        [Required(ErrorMessage="Please select District.")]
        [Range(-1, Int32.MaxValue, ErrorMessage = "Invalid District.")]
        public int DistrictCode { get; set; }

        [Required(ErrorMessage="Please select Block.")]
        [Range(-1, Int32.MaxValue, ErrorMessage = "Invalid Block.")]
        public int BlockCode { get; set; }

        [Required(ErrorMessage="Please select scheme.")]
        ///Changes for RCPLWE/PMGSY3
        [Range(1, 4, ErrorMessage = "Please select scheme.")]
        public int PMGSYScheme { get; set; }

        [Required(ErrorMessage="Please select module.")]
        [Range(1, 5, ErrorMessage = "Please select module.")]
        public int ModuleCode { get; set; }

        public List<SelectListItem> lstStates { get; set; }

        public List<SelectListItem> lstDistricts { get; set; }

        public List<SelectListItem> lstBlocks { get; set; }

        public List<SelectListItem> lstModules { get; set; }

        public List<SelectListItem> lstSchemes { get; set; }
    }
}